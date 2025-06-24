using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Cinemachine;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Input;
using Game.Settings;
using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class CameraUpdateSystem : GameSystemBase
{
	private RaycastSystem m_RaycastSystem;

	private Volume m_Volume;

	private DepthOfField m_DepthOfField;

	private HDShadowSettings m_ShadowSettings;

	private float4 m_StoredShadowSplitsAndDistance;

	private float4 m_StoredShadowBorders;

	private InputActivator[] m_CameraActionActivators;

	private InputBarrier[] m_CameraActionBarriers;

	public Viewer activeViewer { get; private set; }

	public CameraController gamePlayController { get; set; }

	public CinematicCameraController cinematicCameraController { get; set; }

	public OrbitCameraController orbitCameraController { get; set; }

	public Camera activeCamera
	{
		get
		{
			return activeViewer?.camera;
		}
		set
		{
			if ((Object)(object)value == (Object)null)
			{
				if (activeViewer != null)
				{
					activeViewer = null;
					COSystemBase.baseLog.DebugFormat("Resetting activeViewer to null", Array.Empty<object>());
				}
			}
			else
			{
				activeViewer = new Viewer(value);
				COSystemBase.baseLog.DebugFormat("Setting activeViewer with {0}", (object)((Object)value).name);
			}
		}
	}

	public float nearClipPlane { get; private set; }

	public float3 position { get; private set; }

	public float3 direction { get; private set; }

	public float zoom { get; private set; }

	public IGameCameraController activeCameraController
	{
		get
		{
			if ((Object)(object)gamePlayController != (Object)null && gamePlayController.controllerEnabled)
			{
				Assert.IsFalse((Object)(object)cinematicCameraController != (Object)null && cinematicCameraController.controllerEnabled);
				Assert.IsFalse((Object)(object)orbitCameraController != (Object)null && orbitCameraController.controllerEnabled);
				return gamePlayController;
			}
			if ((Object)(object)cinematicCameraController != (Object)null && cinematicCameraController.controllerEnabled)
			{
				Assert.IsFalse((Object)(object)gamePlayController != (Object)null && gamePlayController.controllerEnabled);
				Assert.IsFalse((Object)(object)orbitCameraController != (Object)null && orbitCameraController.controllerEnabled);
				return cinematicCameraController;
			}
			if ((Object)(object)orbitCameraController != (Object)null && orbitCameraController.controllerEnabled)
			{
				Assert.IsFalse((Object)(object)gamePlayController != (Object)null && gamePlayController.controllerEnabled);
				Assert.IsFalse((Object)(object)cinematicCameraController != (Object)null && cinematicCameraController.controllerEnabled);
				return orbitCameraController;
			}
			return null;
		}
		set
		{
			if ((Object)(object)gamePlayController != (Object)null && value != gamePlayController)
			{
				gamePlayController.controllerEnabled = false;
			}
			if ((Object)(object)cinematicCameraController != (Object)null && value != cinematicCameraController)
			{
				cinematicCameraController.controllerEnabled = false;
			}
			if ((Object)(object)orbitCameraController != (Object)null && value != orbitCameraController)
			{
				orbitCameraController.controllerEnabled = false;
			}
			if (value != null)
			{
				value.controllerEnabled = true;
			}
		}
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		activeCamera = Camera.main;
	}

	public CameraBlend GetBlendWeight(out float weight)
	{
		if (CinemachineCore.Instance.BrainCount > 0)
		{
			CinemachineBrain activeBrain = CinemachineCore.Instance.GetActiveBrain(0);
			if ((Object)(object)activeBrain != (Object)null && activeBrain.IsBlending)
			{
				CinemachineBlend activeBlend = activeBrain.ActiveBlend;
				if (activeBlend.IsValid && !activeBlend.IsComplete)
				{
					weight = activeBlend.BlendWeight;
					if (activeBlend.CamB == cinematicCameraController.virtualCamera)
					{
						return CameraBlend.ToCinematicCamera;
					}
					if (activeBlend.CamA == cinematicCameraController.virtualCamera)
					{
						return CameraBlend.FromCinematicCamera;
					}
				}
			}
		}
		weight = 1f;
		return CameraBlend.None;
	}

	public bool TryGetViewer(out Viewer viewer)
	{
		viewer = activeViewer;
		return activeViewer != null;
	}

	public bool TryGetLODParameters(out LODParameters lodParameters)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (activeViewer != null)
		{
			return activeViewer.TryGetLODParameters(out lodParameters);
		}
		lodParameters = default(LODParameters);
		return false;
	}

	private bool CheckOrCacheViewer()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (activeViewer != null && (Object)(object)activeViewer.camera != (Object)null)
		{
			nearClipPlane = activeViewer.nearClipPlane;
			position = activeViewer.position;
			direction = activeViewer.forward;
			zoom = activeCameraController?.zoom ?? zoom;
			activeViewer.Raycast(m_RaycastSystem);
			return true;
		}
		nearClipPlane = 0f;
		position = float3.zero;
		direction = new float3(0f, 0f, 1f);
		activeCamera = null;
		zoom = 0f;
		return false;
	}

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_RaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RaycastSystem>();
		m_Volume = VolumeHelper.CreateVolume("CameraControllerVolume", 51);
		VolumeHelper.GetOrCreateVolumeComponent<DepthOfField>(m_Volume, ref m_DepthOfField);
		VolumeHelper.GetOrCreateVolumeComponent<HDShadowSettings>(m_Volume, ref m_ShadowSettings);
		ProxyActionMap proxyActionMap = InputManager.instance.FindActionMap("Camera");
		m_CameraActionActivators = proxyActionMap.actions.Values.Select((ProxyAction a) => new InputActivator(ignoreIsBuiltIn: true, "CameraUpdateSystem(" + a.name + ")", a)).ToArray();
		m_CameraActionBarriers = proxyActionMap.actions.Values.Select((ProxyAction a) => new InputBarrier("CameraUpdateSystem(" + a.name + ")", a, InputManager.DeviceType.Mouse)).ToArray();
	}

	private void UpdateDepthOfField(float distance)
	{
		GraphicsSettings graphicsSettings = SharedSettings.instance?.graphics;
		if (graphicsSettings != null)
		{
			if (graphicsSettings.depthOfFieldMode == GraphicsSettings.DepthOfFieldMode.TiltShift)
			{
				((VolumeParameter<DepthOfFieldMode>)(object)m_DepthOfField.focusMode).Override((DepthOfFieldMode)2);
				((VolumeParameter<float>)(object)m_DepthOfField.nearFocusStart).Override(distance - distance * graphicsSettings.tiltShiftNearStart);
				((VolumeParameter<float>)(object)m_DepthOfField.nearFocusEnd).Override(distance - distance * graphicsSettings.tiltShiftNearEnd);
				((VolumeParameter<float>)(object)m_DepthOfField.farFocusStart).Override(distance + distance * graphicsSettings.tiltShiftFarStart);
				((VolumeParameter<float>)(object)m_DepthOfField.farFocusEnd).Override(distance + distance * graphicsSettings.tiltShiftFarEnd);
			}
			else if (graphicsSettings.depthOfFieldMode == GraphicsSettings.DepthOfFieldMode.Physical)
			{
				((VolumeParameter<DepthOfFieldMode>)(object)m_DepthOfField.focusMode).Override((DepthOfFieldMode)1);
				((VolumeParameter<FocusDistanceMode>)(object)m_DepthOfField.focusDistanceMode).Override((FocusDistanceMode)0);
				((VolumeParameter<float>)(object)m_DepthOfField.focusDistance).Override(distance);
			}
			else
			{
				((VolumeParameter<DepthOfFieldMode>)(object)m_DepthOfField.focusMode).Override((DepthOfFieldMode)0);
			}
		}
	}

	private void UpdateShadows(Viewer viewer)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		Camera camera = viewer.camera;
		if (!Object.op_Implicit((Object)(object)camera))
		{
			return;
		}
		if (math.lengthsq(m_StoredShadowSplitsAndDistance) == 0f)
		{
			HDCamera orCreate = HDCamera.GetOrCreate(camera, 0);
			if (orCreate != null)
			{
				HDShadowSettings component = orCreate.volumeStack.GetComponent<HDShadowSettings>();
				float value = ((VolumeParameter<float>)(object)component.maxShadowDistance).value;
				float[] cascadeShadowSplits = component.cascadeShadowSplits;
				float[] cascadeShadowBorders = component.cascadeShadowBorders;
				m_StoredShadowSplitsAndDistance = new float4(cascadeShadowSplits[0] * value, cascadeShadowSplits[1] * value, cascadeShadowSplits[2] * value, value);
				m_StoredShadowBorders = new float4(cascadeShadowBorders[0], cascadeShadowBorders[1], cascadeShadowBorders[2], cascadeShadowBorders[3]);
			}
		}
		if (!viewer.shadowsAdjustFarDistance)
		{
			((VolumeParameter)m_ShadowSettings.maxShadowDistance).overrideState = false;
			((VolumeParameter)m_ShadowSettings.cascadeShadowSplit0).overrideState = false;
			((VolumeParameter)m_ShadowSettings.cascadeShadowSplit1).overrideState = false;
			((VolumeParameter)m_ShadowSettings.cascadeShadowSplit2).overrideState = false;
			((VolumeParameter)m_ShadowSettings.cascadeShadowBorder0).overrideState = false;
			((VolumeParameter)m_ShadowSettings.cascadeShadowBorder1).overrideState = false;
			((VolumeParameter)m_ShadowSettings.cascadeShadowBorder2).overrideState = false;
			((VolumeParameter)m_ShadowSettings.cascadeShadowBorder3).overrideState = false;
			return;
		}
		float w = m_StoredShadowSplitsAndDistance.w;
		float num = math.lerp(viewer.viewerDistances.farthestSurface, viewer.viewerDistances.maxDistanceToSeaLevel, 0.2f) * 1.1f;
		w = math.min(w, num);
		float x = m_StoredShadowSplitsAndDistance.x;
		float y = m_StoredShadowSplitsAndDistance.y;
		float z = m_StoredShadowSplitsAndDistance.z;
		x = math.clamp(x, 15f, w * 0.15f);
		y = math.clamp(y, 45f, w * 0.3f);
		z = math.clamp(z, 135f, w * 0.6f);
		float ground = viewer.viewerDistances.ground;
		x = math.min(x, ground * 5f);
		y = math.min(y, ground * 30f);
		z = math.min(z, ground * 200f);
		w = math.max(w, z * 1.2f);
		((VolumeParameter<float>)(object)m_ShadowSettings.maxShadowDistance).Override(w);
		((VolumeParameter<float>)(object)m_ShadowSettings.cascadeShadowSplit0).Override(x / w);
		((VolumeParameter<float>)(object)m_ShadowSettings.cascadeShadowSplit1).Override(y / w);
		((VolumeParameter<float>)(object)m_ShadowSettings.cascadeShadowSplit2).Override(z / w);
		((VolumeParameter<float>)(object)m_ShadowSettings.cascadeShadowBorder0).Override(m_StoredShadowBorders.x);
		((VolumeParameter<float>)(object)m_ShadowSettings.cascadeShadowBorder1).Override(m_StoredShadowBorders.y);
		((VolumeParameter<float>)(object)m_ShadowSettings.cascadeShadowBorder2).Override(m_StoredShadowBorders.z);
		((VolumeParameter<float>)(object)m_ShadowSettings.cascadeShadowBorder3).Override(m_StoredShadowBorders.w);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		VolumeHelper.DestroyVolume(m_Volume);
		if ((Object)(object)gamePlayController != (Object)null)
		{
			gamePlayController.controllerEnabled = false;
		}
		if ((Object)(object)cinematicCameraController != (Object)null)
		{
			cinematicCameraController.controllerEnabled = false;
		}
		if ((Object)(object)orbitCameraController != (Object)null)
		{
			orbitCameraController.controllerEnabled = false;
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		bool num = activeViewer != null && (Object)(object)activeViewer.camera != (Object)null;
		float distance = 0f;
		if (num)
		{
			Viewer viewer = activeViewer;
			RaycastSystem raycast = m_RaycastSystem;
			WorldUnmanaged worldUnmanaged = ((SystemState)(ref ((SystemBase)this).CheckedStateRef)).WorldUnmanaged;
			viewer.UpdateRaycast(raycast, ((WorldUnmanaged)(ref worldUnmanaged)).Time.DeltaTime);
			distance = activeViewer.viewerDistances.focus;
			UpdateShadows(activeViewer);
		}
		UpdateDepthOfField(distance);
		activeCameraController?.UpdateCamera();
		for (int i = 0; i < CinemachineCore.Instance.BrainCount; i++)
		{
			CinemachineCore.Instance.GetActiveBrain(i).ManualUpdate();
		}
		CheckOrCacheViewer();
		RefreshInput();
	}

	private void RefreshInput()
	{
		InputActivator[] array = m_CameraActionActivators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = activeCameraController != null;
		}
		InputBarrier[] array2 = m_CameraActionBarriers;
		foreach (InputBarrier inputBarrier in array2)
		{
			if (activeCameraController == null)
			{
				inputBarrier.blocked = false;
			}
			else if (!InputManager.instance.mouseOverUI)
			{
				inputBarrier.blocked = false;
			}
			else if (inputBarrier.actions.All((ProxyAction a) => !a.IsInProgress()))
			{
				inputBarrier.blocked = true;
			}
		}
	}

	[Preserve]
	public CameraUpdateSystem()
	{
	}
}

using System;
using System.Collections.Generic;
using Cinemachine;
using Colossal.Mathematics;
using Game.Audio;
using Game.Input;
using Game.Rendering;
using Game.SceneFlow;
using Game.Settings;
using Game.Simulation;
using Game.UI.InGame;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game;

public class CameraController : MonoBehaviour, IGameCameraController
{
	[SerializeField]
	private float3 m_Pivot;

	[SerializeField]
	private float2 m_Angle;

	[SerializeField]
	private float m_Zoom;

	[SerializeField]
	private Bounds1 m_ZoomRange = new Bounds1(10f, 10000f);

	[SerializeField]
	private Bounds1 m_MapTileToolZoomRange = new Bounds1(10f, 20000f);

	[SerializeField]
	private bool m_MapTileToolViewEnabled;

	[SerializeField]
	private float m_MapTileToolFOV;

	[SerializeField]
	private float m_MapTileToolFarclip;

	[SerializeField]
	private float3 m_MapTileToolPivot;

	[SerializeField]
	private float2 m_MapTileToolAngle;

	[SerializeField]
	private float m_MapTileToolZoom;

	[SerializeField]
	private float m_MapTileToolTransitionTime;

	[SerializeField]
	private float m_MoveSmoothing = 1E-06f;

	[SerializeField]
	private float m_CollisionSmoothing = 0.001f;

	private ProxyActionMap m_CameraMap;

	private ProxyAction m_MoveAction;

	private ProxyAction m_MoveFastAction;

	private ProxyAction m_RotateAction;

	private ProxyAction m_ZoomAction;

	private CinemachineVirtualCamera m_VCam;

	private float m_InitialFarClip;

	private float m_InitialFov;

	private float m_LastGameViewZoom;

	private float2 m_LastGameViewAngle;

	private float3 m_LastGameViewPivot;

	private float m_LastMapViewZoom;

	private float2 m_LastMapViewAngle;

	private float3 m_LastMapViewPivot;

	private float m_MapViewTimer;

	private AudioManager m_AudioManager;

	private CameraUpdateSystem m_CameraSystem;

	private CameraCollisionSystem m_CollisionSystem;

	public IEnumerable<ProxyAction> inputActions
	{
		get
		{
			if (m_MoveAction != null)
			{
				yield return m_MoveAction;
			}
			if (m_MoveFastAction != null)
			{
				yield return m_MoveFastAction;
			}
			if (m_RotateAction != null)
			{
				yield return m_RotateAction;
			}
			if (m_ZoomAction != null)
			{
				yield return m_ZoomAction;
			}
		}
	}

	public Action<bool> EventCameraMovingChanged { get; set; }

	public bool moving { get; private set; }

	public ref LensSettings lens => ref m_VCam.m_Lens;

	public ICinemachineCamera virtualCamera => (ICinemachineCamera)(object)m_VCam;

	public Vector3 rotation
	{
		get
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(m_Angle.y, m_Angle.x, 0f);
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			m_Angle = new float2(value.y, value.x);
		}
	}

	public TerrainSystem terrainSystem
	{
		get
		{
			if (World.DefaultGameObjectInjectionWorld != null)
			{
				return World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TerrainSystem>();
			}
			return null;
		}
	}

	public WaterSystem waterSystem
	{
		get
		{
			if (World.DefaultGameObjectInjectionWorld != null)
			{
				return World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<WaterSystem>();
			}
			return null;
		}
	}

	public Vector3 pivot
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return float3.op_Implicit(m_Pivot);
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			m_Pivot = float3.op_Implicit(value);
		}
	}

	public Vector3 position
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)this).transform.position;
		}
		set
		{
		}
	}

	public float2 angle
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_Angle;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Angle = value;
		}
	}

	public float zoom
	{
		get
		{
			return m_Zoom;
		}
		set
		{
			m_Zoom = value;
		}
	}

	public bool controllerEnabled
	{
		get
		{
			return ((Behaviour)this).isActiveAndEnabled;
		}
		set
		{
			((Component)this).gameObject.SetActive(value);
		}
	}

	public bool inputEnabled { get; set; } = true;

	public Bounds1 zoomRange
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (MapTilesUISystem.mapTileViewActive)
			{
				return m_MapTileToolZoomRange;
			}
			return m_ZoomRange;
		}
	}

	public float3 cameraPosition { get; private set; }

	public float velocity { get; private set; }

	public bool edgeScrolling { get; set; }

	public float edgeScrollingSensitivity { get; set; }

	public float clipDistance { get; set; }

	public void TryMatchPosition(IGameCameraController other)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetTerrainHeight(other.position, out var terrainHeight))
		{
			float num = other.position.y - terrainHeight;
			float num2 = Mathf.Sin((float)Math.PI / 180f * other.rotation.x);
			float num3 = 1f / (2f - 4f * num2);
			float num4 = (8f * zoomRange.min - 20f * num) * num2 + zoomRange.min - 2f * num;
			zoom = Mathf.Clamp(Mathf.Abs(num3 * (Mathf.Sqrt(num4 * num4 - (4f - 8f * num2) * (-4f * zoomRange.min * zoomRange.min + 18f * zoomRange.min * num - 20f * num * num)) + num4)), zoomRange.min, zoomRange.max);
			Quaternion val = Quaternion.Euler(other.rotation.x, other.rotation.y, other.rotation.z);
			pivot = other.position + val * new Vector3(0f, 0f, zoom);
			angle = new float2(other.rotation.y, (other.rotation.x > 90f) ? (other.rotation.x - 360f) : other.rotation.x);
			((Component)this).transform.rotation = val;
			((Component)this).transform.position = other.position;
		}
	}

	private async void Awake()
	{
		if (!(await GameManager.instance.WaitForReadyState()))
		{
			return;
		}
		if (!Application.isEditor)
		{
			edgeScrolling = true;
			GameplaySettings gameplaySettings = SharedSettings.instance?.gameplay;
			if (gameplaySettings != null)
			{
				edgeScrolling = gameplaySettings.edgeScrolling;
				edgeScrollingSensitivity = gameplaySettings.edgeScrollingSensitivity;
			}
		}
		m_VCam = ((Component)this).GetComponent<CinemachineVirtualCamera>();
		m_InitialFarClip = m_VCam.m_Lens.FarClipPlane;
		m_InitialFov = m_VCam.m_Lens.FieldOfView;
		clipDistance = float.MaxValue;
		m_CameraMap = InputManager.instance.FindActionMap("Camera");
		m_MoveAction = m_CameraMap.FindAction("Move");
		m_MoveFastAction = m_CameraMap.FindAction("Move Fast");
		m_RotateAction = m_CameraMap.FindAction("Rotate");
		m_ZoomAction = m_CameraMap.FindAction("Zoom");
		m_CameraSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_CameraSystem.gamePlayController = this;
		m_CollisionSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CameraCollisionSystem>();
	}

	public void UpdateCamera()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		if (m_MapTileToolViewEnabled && HandleMapViewCamera())
		{
			return;
		}
		float2 val = float2.zero;
		float2 val2 = float2.zero;
		float num = 0f;
		bool flag = false;
		if (m_CameraMap.enabled)
		{
			val = MathUtils.MaxAbs(float2.op_Implicit(m_MoveAction.ReadValue<Vector2>()), float2.op_Implicit(m_MoveFastAction.ReadValue<Vector2>()));
			val2 = float2.op_Implicit(m_RotateAction.ReadValue<Vector2>());
			num = m_ZoomAction.ReadValue<float>();
			if (edgeScrolling && InputManager.instance.activeControlScheme == InputManager.ControlScheme.KeyboardAndMouse && InputManager.instance.mouseOnScreen)
			{
				float num2 = edgeScrollingSensitivity;
				float3 val3 = float3.op_Implicit(InputManager.instance.mousePosition);
				float2 xy = ((float3)(ref val3)).xy;
				xy *= 2f / new float2((float)Screen.width, (float)Screen.height);
				xy -= 1f;
				float num3 = 0.02f;
				float2 val4 = default(float2);
				((float2)(ref val4))._002Ector((float)Screen.height / (float)Screen.width * num3, num3);
				num2 *= math.saturate(math.cmax((math.abs(xy) - (1f - val4)) / val4));
				num2 *= Time.deltaTime;
				val += math.normalizesafe(xy, default(float2)) * num2;
			}
		}
		float num4 = m_Zoom;
		m_Zoom = MathUtils.Clamp(math.pow(m_Zoom, 1f + num), zoomRange);
		if (num4 != m_Zoom)
		{
			flag = true;
		}
		val2.y = 0f - val2.y;
		m_Angle += val2;
		m_Angle.y = math.clamp(m_Angle.y, -90f, 90f);
		if (m_Angle.x < -180f)
		{
			m_Angle.x += 360f;
		}
		if (m_Angle.x > 180f)
		{
			m_Angle.x -= 360f;
		}
		float2 val5 = math.radians(m_Angle);
		float3 val6 = default(float3);
		val6.x = 0f - math.sin(val5.x);
		val6.y = 0f;
		val6.z = 0f - math.cos(val5.x);
		float3 val7 = val6;
		val6 *= math.cos(val5.y);
		val6.y = math.sin(val5.y);
		float3 val8 = -val6;
		val6 *= m_Zoom;
		float3 val9 = math.cross(val7, new float3(0f, 1f, 0f));
		float3 val10 = math.cross(val8, val9);
		val *= m_Zoom;
		m_Pivot += val.x * val9;
		m_Pivot -= val.y * val7;
		float3 cameraPos = GetCameraPos(val6);
		if (terrainSystem != null)
		{
			TerrainHeightData data = terrainSystem.GetHeightData();
			WaterSurfaceData data2 = default(WaterSurfaceData);
			if (waterSystem != null && waterSystem.Loaded)
			{
				data2 = waterSystem.GetSurfaceData(out var deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (data.isCreated)
			{
				if (data2.isCreated)
				{
					m_Pivot.y = math.lerp(WaterUtils.SampleHeight(ref data2, ref data, m_Pivot), m_Pivot.y, m_MoveSmoothing);
				}
				else
				{
					m_Pivot.y = math.lerp(TerrainUtils.SampleHeight(ref data, m_Pivot), m_Pivot.y, m_MoveSmoothing);
				}
				Bounds3 val11 = (GameManager.instance.gameMode.IsEditor() ? TerrainUtils.GetEditorCameraBounds(terrainSystem, ref data) : TerrainUtils.GetBounds(ref data));
				m_Pivot = MathUtils.Clamp(m_Pivot, val11);
				cameraPos = GetCameraPos(val6);
				float num5 = ((!data2.isCreated) ? (TerrainUtils.SampleHeight(ref data, cameraPos) + zoomRange.min * 0.5f + (m_Zoom - zoomRange.min) * 0.1f) : (WaterUtils.SampleHeight(ref data2, ref data, cameraPos) + zoomRange.min * 0.5f + (m_Zoom - zoomRange.min) * 0.1f));
				float num6 = (cameraPos.y - num5) / m_Zoom;
				num6 = (math.sqrt(num6 * num6 + 0.2f) - num6) * (0.5f * m_Zoom);
				cameraPos.y += num6;
			}
		}
		float3 val12 = cameraPosition;
		quaternion val13 = quaternion.LookRotation(val8, val10);
		if (m_CollisionSystem != null && m_CameraSystem != null && (Object)(object)m_CameraSystem.activeCamera != (Object)null)
		{
			float nearClipPlane = m_CameraSystem.activeCamera.nearClipPlane;
			float2 val14 = default(float2);
			val14.y = m_CameraSystem.activeCamera.fieldOfView;
			val14.x = Camera.VerticalToHorizontalFieldOfView(val14.y, m_CameraSystem.activeCamera.aspect);
			m_CollisionSystem.CheckCollisions(ref cameraPos, val12, val13, math.min(m_Zoom - zoomRange.min, 200f), math.min(zoomRange.max - m_Zoom, 200f), math.max(nearClipPlane * 2f, zoomRange.min * 0.5f), nearClipPlane, m_CollisionSmoothing, val14);
		}
		Quaternion localRotation = ((Component)this).transform.localRotation;
		cameraPosition = cameraPos;
		((Component)this).transform.localPosition = float3.op_Implicit(cameraPos);
		((Component)this).transform.localRotation = quaternion.op_Implicit(val13);
		velocity = math.lengthsq(val12 - cameraPosition) / Time.deltaTime;
		if (!((Quaternion)(ref localRotation)).Equals(((Component)this).transform.localRotation) || !((float3)(ref val12)).Equals(cameraPosition))
		{
			flag = true;
		}
		if (moving != flag)
		{
			EventCameraMovingChanged?.Invoke(flag);
			moving = flag;
		}
		AudioManager.instance?.UpdateAudioListener(((Component)this).transform.position, ((Component)this).transform.rotation);
	}

	private float3 GetCameraPos(float3 cameraOffset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		float3 result = m_Pivot + cameraOffset;
		result.y += zoomRange.min * 0.5f;
		return result;
	}

	private bool HandleMapViewCamera()
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		float num;
		float2 lastMapViewAngle = default(float2);
		float3 val;
		float num2;
		if (!MapTilesUISystem.mapTileViewActive)
		{
			if (m_MapViewTimer == 0f)
			{
				return false;
			}
			if (Mathf.Abs(m_MapViewTimer - m_MapTileToolTransitionTime) < Mathf.Epsilon)
			{
				m_Zoom = m_LastGameViewZoom;
				m_Angle = m_LastGameViewAngle;
				m_Pivot = m_LastGameViewPivot;
			}
			num = m_LastMapViewZoom;
			lastMapViewAngle = m_LastMapViewAngle;
			val = m_LastMapViewPivot;
			m_MapViewTimer = math.max(m_MapViewTimer - Time.deltaTime, 0f);
			num2 = ((m_MapTileToolTransitionTime > 0f) ? (m_MapViewTimer / m_MapTileToolTransitionTime) : 0f);
		}
		else
		{
			m_LastMapViewAngle = m_Angle;
			m_LastMapViewZoom = m_Zoom;
			m_LastMapViewPivot = m_Pivot;
			if (Mathf.Abs(m_MapViewTimer - m_MapTileToolTransitionTime) < Mathf.Epsilon)
			{
				return false;
			}
			m_MapViewTimer = math.min(m_MapViewTimer + Time.deltaTime, m_MapTileToolTransitionTime);
			num2 = ((m_MapTileToolTransitionTime > 0f) ? (m_MapViewTimer / m_MapTileToolTransitionTime) : 1f);
			if (Mathf.Abs(num2 - 1f) < Mathf.Epsilon)
			{
				m_LastGameViewZoom = m_Zoom;
				m_LastGameViewAngle = m_Angle;
				m_LastGameViewPivot = m_Pivot;
				m_Zoom = m_MapTileToolZoom;
				m_Angle = new float2(Mathf.Round(m_Angle.x / 90f) * 90f, m_MapTileToolAngle.y);
				m_Pivot = m_MapTileToolPivot;
			}
			num = m_MapTileToolZoom;
			((float2)(ref lastMapViewAngle))._002Ector(Mathf.Round(m_Angle.x / 90f) * 90f, m_MapTileToolAngle.y);
			val = m_MapTileToolPivot;
		}
		if (TryGetTerrainHeight(float3.op_Implicit(val), out var terrainHeight))
		{
			val.y = terrainHeight;
		}
		num2 = Mathf.SmoothStep(0f, 1f, num2);
		float3 val2 = math.lerp(m_Pivot, val, num2);
		float2 val3 = LerpAngle(m_Angle, lastMapViewAngle, num2);
		float num3 = math.lerp(m_Zoom, num, num2);
		m_VCam.m_Lens.FarClipPlane = math.lerp(m_InitialFarClip, m_MapTileToolFarclip, num2);
		m_VCam.m_Lens.FieldOfView = math.lerp(m_InitialFov, m_MapTileToolFOV, num2);
		float2 val4 = math.radians(val3);
		float3 val5 = default(float3);
		val5.x = 0f - math.sin(val4.x);
		val5.y = 0f;
		val5.z = 0f - math.cos(val4.x);
		float3 val6 = val5;
		val5 *= math.cos(val4.y);
		val5.y = math.sin(val4.y);
		float3 val7 = -val5;
		val5 *= num3;
		float3 val8 = val2 + val5;
		val8.y += zoomRange.min * 0.5f;
		float3 val9 = math.cross(val6, new float3(0f, 1f, 0f));
		float3 val10 = math.cross(val7, val9);
		if (terrainSystem != null)
		{
			TerrainHeightData data = terrainSystem.GetHeightData();
			WaterSurfaceData data2 = default(WaterSurfaceData);
			if (waterSystem != null)
			{
				data2 = waterSystem.GetSurfaceData(out var deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (data.isCreated)
			{
				float num4 = ((!data2.isCreated) ? (TerrainUtils.SampleHeight(ref data, val8) + zoomRange.min * 0.5f + (num3 - zoomRange.min) * 0.1f) : (WaterUtils.SampleHeight(ref data2, ref data, val8) + zoomRange.min * 0.5f + (num3 - zoomRange.min) * 0.1f));
				float num5 = (val8.y - num4) / num3;
				num5 = (math.sqrt(num5 * num5 + 0.2f) - num5) * (0.5f * num3);
				val8.y += num5;
			}
		}
		((Component)this).transform.localPosition = float3.op_Implicit(val8);
		((Component)this).transform.localRotation = quaternion.op_Implicit(quaternion.LookRotation(val7, val10));
		return true;
	}

	public static float2 LerpAngle(float2 from, float2 to, float t)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		float num = ((to.x - from.x) % 360f + 540f) % 360f - 180f;
		return new float2(from.x + num * t % 360f, math.lerp(from.y, to.y, t));
	}

	private bool TryGetTerrainHeight(Vector3 pos, out float terrainHeight)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (terrainSystem != null)
		{
			TerrainHeightData data = terrainSystem.GetHeightData();
			WaterSurfaceData data2 = default(WaterSurfaceData);
			if (waterSystem != null)
			{
				data2 = waterSystem.GetSurfaceData(out var deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (data.isCreated)
			{
				if (data2.isCreated)
				{
					terrainHeight = WaterUtils.SampleHeight(ref data2, ref data, float3.op_Implicit(pos));
				}
				else
				{
					terrainHeight = TerrainUtils.SampleHeight(ref data, float3.op_Implicit(pos));
				}
				return true;
			}
		}
		terrainHeight = 0f;
		return false;
	}

	public static bool TryGet(out CameraController cameraController)
	{
		GameObject val = GameObject.FindGameObjectWithTag("GameplayCamera");
		if ((Object)(object)val != (Object)null)
		{
			cameraController = val.GetComponent<CameraController>();
			return (Object)(object)cameraController != (Object)null;
		}
		cameraController = null;
		return false;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.Mathematics;
using Colossal.PSI.Environment;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Assets;
using Game.CinematicCamera;
using Game.Input;
using Game.Rendering;
using Game.Rendering.CinematicCamera;
using Game.SceneFlow;
using Game.Settings;
using Game.Tutorials;
using Game.UI.Menu;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class CinematicCameraUISystem : UISystemBase
{
	private static readonly string kGroup = "cinematicCamera";

	private static readonly CinematicCameraSequence.CinematicCameraCurveModifier[] kEmptyModifierArray = Array.Empty<CinematicCameraSequence.CinematicCameraCurveModifier>();

	private static readonly string kCaptureKeyframeTutorialTag = "CinematicCameraPanelCaptureKey";

	private PhotoModeRenderSystem m_PhotoModeRenderSystem;

	private TutorialUITriggerSystem m_TutorialUITriggerSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private GetterValueBinding<CinematicCameraAsset[]> m_Assets;

	private ValueBinding<CinematicCameraAsset> m_LastLoaded;

	private ValueBinding<CinematicCameraSequence.CinematicCameraCurveModifier[]> m_TransformAnimationCurveBinding;

	private ValueBinding<CinematicCameraSequence.CinematicCameraCurveModifier[]> m_ModifierAnimationCurveBinding;

	private GetterValueBinding<List<string>> m_AvailableCloudTargetsBinding;

	private GetterValueBinding<string> m_SelectedCloudTargetBinding;

	private CinematicCameraSequence m_ActiveAutoplaySequence;

	private IGameCameraController m_PreviousController;

	private ProxyAction m_MoveAction;

	private ProxyAction m_ZoomAction;

	private ProxyAction m_RotateAction;

	private bool m_Playing;

	public CinematicCameraSequence activeSequence { get; set; } = new CinematicCameraSequence();

	private float m_TimelinePositionBindingValue => MathUtils.Snap(t, 0.05f);

	private float t { get; set; }

	private bool playing
	{
		get
		{
			return m_Playing;
		}
		set
		{
			if (value != m_Playing)
			{
				m_CameraUpdateSystem.cinematicCameraController.inputEnabled = !value;
				m_CameraUpdateSystem.orbitCameraController.inputEnabled = !value;
				if (!m_Playing)
				{
					m_PreviousController = m_CameraUpdateSystem.activeCameraController;
					m_CameraUpdateSystem.activeCameraController = m_CameraUpdateSystem.cinematicCameraController;
				}
				else
				{
					m_CameraUpdateSystem.activeCameraController = m_PreviousController;
				}
				m_Playing = value;
			}
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Expected O, but got Unknown
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		AddBinding((IBinding)(object)new TriggerBinding<float>(kGroup, "setPlaybackDuration", (Action<float>)OnSetPlaybackDuration, (IReader<float>)null));
		AddBinding((IBinding)(object)new TriggerBinding<float>(kGroup, "setTimelinePosition", (Action<float>)OnSetTimelinePosition, (IReader<float>)null));
		AddBinding((IBinding)new TriggerBinding(kGroup, "togglePlayback", (Action)TogglePlayback));
		AddBinding((IBinding)new TriggerBinding(kGroup, "stopPlayback", (Action)StopPlayback));
		AddBinding((IBinding)(object)new TriggerBinding<string, string>(kGroup, "captureKey", (Action<string, string>)OnCapture, (IReader<string>)null, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<int, int>(kGroup, "removeCameraTransformKey", (Action<int, int>)OnRemoveSelectedTransform, (IReader<int>)null, (IReader<int>)null));
		AddBinding((IBinding)(object)new CallBinding<string, int, int, Keyframe, int>(kGroup, "moveKeyFrame", (Func<string, int, int, Keyframe, int>)OnMoveKeyFrame, (IReader<string>)null, (IReader<int>)null, (IReader<int>)null, (IReader<Keyframe>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string, int, int>(kGroup, "removeKeyFrame", (Action<string, int, int>)OnRemoveKeyFrame, (IReader<string>)null, (IReader<int>)null, (IReader<int>)null));
		AddBinding((IBinding)(object)new CallBinding<string, float, float, int, int>(kGroup, "addKeyFrame", (Func<string, float, float, int, int>)OnAddKeyFrame, (IReader<string>)null, (IReader<float>)null, (IReader<float>)null, (IReader<int>)null));
		AddBinding((IBinding)new TriggerBinding(kGroup, "reset", (Action)Reset));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>(kGroup, "loop", (Func<bool>)(() => activeSequence.loop), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<bool>(kGroup, "toggleLoop", (Action<bool>)OnToggleLoop, (IReader<bool>)null));
		AddBinding((IBinding)(object)new CallBinding<float[]>(kGroup, "getControllerDelta", (Func<float[]>)GetControllerDelta));
		AddBinding((IBinding)(object)new CallBinding<float[]>(kGroup, "getControllerPanDelta", (Func<float[]>)GetControllerPanDelta));
		AddBinding((IBinding)(object)new CallBinding<float>(kGroup, "getControllerZoomDelta", (Func<float>)GetControllerZoomDelta));
		AddBinding((IBinding)(object)new TriggerBinding<bool>(kGroup, "toggleCurveEditorFocus", (Action<bool>)OnCurveEditorFocusChange, (IReader<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>(kGroup, "playbackDuration", (Func<float>)(() => activeSequence.playbackDuration), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddBinding((IBinding)new TriggerBinding(kGroup, "onAfterPlaybackDurationChange", (Action)delegate
		{
			activeSequence.AfterModifications();
		}));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>(kGroup, "timelinePosition", (Func<float>)(() => m_TimelinePositionBindingValue), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>(kGroup, "timelineLength", (Func<float>)(() => activeSequence.timelineLength), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>(kGroup, "playing", (Func<bool>)(() => playing), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string, string>(kGroup, "save", (Action<string, string>)Save, (IReader<string>)null, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string, string>(kGroup, "load", (Action<string, string>)Load, (IReader<string>)null, (IReader<string>)null));
		AddBinding((IBinding)(object)(m_LastLoaded = new ValueBinding<CinematicCameraAsset>(kGroup, "lastLoaded", (CinematicCameraAsset)null, (IWriter<CinematicCameraAsset>)(object)ValueWriters.Nullable<CinematicCameraAsset>((IWriter<CinematicCameraAsset>)(object)new ValueWriter<CinematicCameraAsset>()), (EqualityComparer<CinematicCameraAsset>)null)));
		AddBinding((IBinding)(object)(m_Assets = new GetterValueBinding<CinematicCameraAsset[]>(kGroup, "assets", (Func<CinematicCameraAsset[]>)UpdateAssets, (IWriter<CinematicCameraAsset[]>)(object)new ArrayWriter<CinematicCameraAsset>((IWriter<CinematicCameraAsset>)(object)new ValueWriter<CinematicCameraAsset>(), false), (EqualityComparer<CinematicCameraAsset[]>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<string, string>(kGroup, "delete", (Action<string, string>)Delete, (IReader<string>)null, (IReader<string>)null));
		AddBinding((IBinding)(object)(m_TransformAnimationCurveBinding = new ValueBinding<CinematicCameraSequence.CinematicCameraCurveModifier[]>(kGroup, "transformAnimationCurves", kEmptyModifierArray, (IWriter<CinematicCameraSequence.CinematicCameraCurveModifier[]>)(object)new ListWriter<CinematicCameraSequence.CinematicCameraCurveModifier>((IWriter<CinematicCameraSequence.CinematicCameraCurveModifier>)(object)new ValueWriter<CinematicCameraSequence.CinematicCameraCurveModifier>()), (EqualityComparer<CinematicCameraSequence.CinematicCameraCurveModifier[]>)null)));
		AddBinding((IBinding)(object)(m_ModifierAnimationCurveBinding = new ValueBinding<CinematicCameraSequence.CinematicCameraCurveModifier[]>(kGroup, "modifierAnimationCurves", kEmptyModifierArray, (IWriter<CinematicCameraSequence.CinematicCameraCurveModifier[]>)(object)new ListWriter<CinematicCameraSequence.CinematicCameraCurveModifier>((IWriter<CinematicCameraSequence.CinematicCameraCurveModifier>)(object)new ValueWriter<CinematicCameraSequence.CinematicCameraCurveModifier>()), (EqualityComparer<CinematicCameraSequence.CinematicCameraCurveModifier[]>)null)));
		AddBinding((IBinding)(object)(m_AvailableCloudTargetsBinding = new GetterValueBinding<List<string>>(kGroup, "availableCloudTargets", (Func<List<string>>)MenuHelpers.GetAvailableCloudTargets, (IWriter<List<string>>)(object)new ListWriter<string>((IWriter<string>)null), (EqualityComparer<List<string>>)null)));
		AddUpdateBinding((IUpdateBinding)(object)(m_SelectedCloudTargetBinding = new GetterValueBinding<string>(kGroup, "selectedCloudTarget", (Func<string>)(() => MenuHelpers.GetSanitizedCloudTarget(SharedSettings.instance.userState.lastCloudTarget).name), (IWriter<string>)null, (EqualityComparer<string>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<string>(kGroup, "selectCloudTarget", (Action<string>)delegate(string cloudTarget)
		{
			SharedSettings.instance.userState.lastCloudTarget = cloudTarget;
		}, (IReader<string>)null));
		m_TutorialUITriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialUITriggerSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_PhotoModeRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PhotoModeRenderSystem>();
		m_MoveAction = InputManager.instance.FindAction("Camera", "Move");
		m_ZoomAction = InputManager.instance.FindAction("Camera", "Zoom");
		m_RotateAction = InputManager.instance.FindAction("Camera", "Rotate");
		AssetDatabase.global.onAssetDatabaseChanged.Subscribe((EventDelegate<AssetChangedEventArgs>)OnCloudTargetsChanged, (Predicate<AssetChangedEventArgs>)delegate(AssetChangedEventArgs args)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Invalid comparison between Unknown and I4
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Invalid comparison between Unknown and I4
			ChangeType change = ((AssetChangedEventArgs)(ref args)).change;
			return (int)change == 0 || (int)change == 1 || (int)change == 2;
		}, AssetChangedEventArgs.Default);
		EventExtensions.Subscribe<CinematicCameraAsset>(AssetDatabase.global.onAssetDatabaseChanged, (EventDelegate<AssetChangedEventArgs>)OnAssetsChanged, AssetChangedEventArgs.Default);
		Reset();
	}

	public void ToggleModifier(PhotoModeProperty p)
	{
		m_TutorialUITriggerSystem.ActivateTrigger(kCaptureKeyframeTutorialTag);
		foreach (PhotoModeProperty item in PhotoModeUtils.ExtractMultiPropertyComponents(p, (IDictionary<string, PhotoModeProperty>)m_PhotoModeRenderSystem.photoModeProperties))
		{
			float min = item.min?.Invoke() ?? (-10000f);
			float max = item.max?.Invoke() ?? 10000f;
			activeSequence.AddModifierKey(item.id, t, item.getValue(), min, max);
		}
		m_ModifierAnimationCurveBinding.Update(activeSequence.modifiers.ToArray());
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Invalid comparison between Unknown and I4
		if ((Object)(object)m_CameraUpdateSystem.cinematicCameraController != (Object)null)
		{
			CinematicCameraController cinematicCameraController = m_CameraUpdateSystem.cinematicCameraController;
			cinematicCameraController.eventCameraMove = (Action)Delegate.Remove(cinematicCameraController.eventCameraMove, new Action(PausePlayback));
			CinematicCameraController cinematicCameraController2 = m_CameraUpdateSystem.cinematicCameraController;
			cinematicCameraController2.eventCameraMove = (Action)Delegate.Combine(cinematicCameraController2.eventCameraMove, new Action(PausePlayback));
		}
		if ((Object)(object)m_CameraUpdateSystem.orbitCameraController != (Object)null)
		{
			OrbitCameraController orbitCameraController = m_CameraUpdateSystem.orbitCameraController;
			orbitCameraController.EventCameraMove = (Action)Delegate.Remove(orbitCameraController.EventCameraMove, new Action(PausePlayback));
			OrbitCameraController orbitCameraController2 = m_CameraUpdateSystem.orbitCameraController;
			orbitCameraController2.EventCameraMove = (Action)Delegate.Combine(orbitCameraController2.EventCameraMove, new Action(PausePlayback));
		}
		if ((int)((Context)(ref serializationContext)).purpose != 6)
		{
			m_ActiveAutoplaySequence = null;
		}
		m_Playing = false;
		Reset();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (playing)
		{
			UpdatePlayback();
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		if ((Object)(object)m_CameraUpdateSystem?.cinematicCameraController != (Object)null)
		{
			CinematicCameraController cinematicCameraController = m_CameraUpdateSystem.cinematicCameraController;
			cinematicCameraController.eventCameraMove = (Action)Delegate.Remove(cinematicCameraController.eventCameraMove, new Action(PausePlayback));
		}
		if ((Object)(object)m_CameraUpdateSystem?.orbitCameraController != (Object)null)
		{
			OrbitCameraController orbitCameraController = m_CameraUpdateSystem.orbitCameraController;
			orbitCameraController.EventCameraMove = (Action)Delegate.Remove(orbitCameraController.EventCameraMove, new Action(PausePlayback));
		}
		base.OnDestroy();
	}

	private void OnToggleLoop(bool loop)
	{
		activeSequence.loop = loop;
		if (loop)
		{
			m_ModifierAnimationCurveBinding.Update(activeSequence.modifiers.ToArray());
			m_TransformAnimationCurveBinding.Update(GetTransformCurves());
		}
	}

	private int OnAddKeyFrame(string id, float time, float value, int curveIndex)
	{
		if (id == "Property")
		{
			CinematicCameraSequence.CinematicCameraCurveModifier cinematicCameraCurveModifier = activeSequence.modifiers[curveIndex];
			string id2 = cinematicCameraCurveModifier.id;
			int result = activeSequence.AddModifierKey(id2, time, value, cinematicCameraCurveModifier.min, cinematicCameraCurveModifier.max);
			m_ModifierAnimationCurveBinding.Update(activeSequence.modifiers.ToArray());
			return result;
		}
		int result2 = activeSequence.transforms[curveIndex].curve.AddKey(time, value);
		m_TransformAnimationCurveBinding.Update(GetTransformCurves());
		return result2;
	}

	private void Reset()
	{
		activeSequence.Reset();
		m_TransformAnimationCurveBinding.Update(GetTransformCurves());
		m_ModifierAnimationCurveBinding.Update(activeSequence.modifiers.ToArray());
	}

	private void OnSetTimelinePosition(float position)
	{
		playing = false;
		t = position;
		activeSequence.Refresh(position, (IDictionary<string, PhotoModeProperty>)m_PhotoModeRenderSystem.photoModeProperties, m_CameraUpdateSystem.activeCameraController);
	}

	private void OnSetPlaybackDuration(float duration)
	{
		activeSequence.playbackDuration = Mathf.Max(duration, activeSequence.timelineLength);
	}

	private void OnCapture(string id, string property)
	{
		if (id == "Property")
		{
			foreach (PhotoModeProperty value in m_PhotoModeRenderSystem.photoModeProperties.Values)
			{
				if (PhotoModeUtils.ExtractPropertyID(value) == property)
				{
					ToggleModifier(value);
					break;
				}
			}
			return;
		}
		OnCaptureTransform();
	}

	private void OnCaptureTransform()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		m_TutorialUITriggerSystem.ActivateTrigger(kCaptureKeyframeTutorialTag);
		Vector3 position = m_CameraUpdateSystem.activeCameraController.position;
		Vector3 rotation = m_CameraUpdateSystem.activeCameraController.rotation;
		activeSequence.AddCameraTransform(t, position, rotation);
		m_TransformAnimationCurveBinding.Update(GetTransformCurves());
	}

	private void Save(string name, string hash = null)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		ILocalAssetDatabase item = MenuHelpers.GetSanitizedCloudTarget(SharedSettings.instance.userState.lastCloudTarget).db;
		if (string.IsNullOrEmpty(hash))
		{
			AssetDataPath val = AssetDataPath.op_Implicit(name);
			if (!((IDataSourceAccessor)item).dataSource.isRemoteStorageSource)
			{
				string specialPath = EnvPath.GetSpecialPath<CinematicCameraAsset>();
				if (specialPath != null)
				{
					val = AssetDataPath.Create(specialPath, name, (EscapeStrategy)2);
				}
			}
			CinematicCameraAsset cinematicCameraAsset = item.AddAsset<CinematicCameraAsset>(val, default(Hash128));
			((Metadata<CinematicCameraSequence>)cinematicCameraAsset).target = activeSequence;
			((AssetData)cinematicCameraAsset).Save(false);
			m_LastLoaded.Update(cinematicCameraAsset);
			m_Assets.Update();
		}
		else
		{
			Hash128 val2 = default(Hash128);
			((Hash128)(ref val2))._002Ector(hash);
			CinematicCameraAsset asset = ((IAssetDatabase)item).GetAsset<CinematicCameraAsset>(val2);
			if ((AssetData)(object)asset != (IAssetData)null)
			{
				((Metadata<CinematicCameraSequence>)asset).target = activeSequence;
				((AssetData)asset).Save(false);
				m_LastLoaded.Update(asset);
				m_Assets.Update();
			}
		}
	}

	private void Load(string hash, string storage)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Hash128 val = default(Hash128);
		((Hash128)(ref val))._002Ector(hash);
		CinematicCameraAsset asset = ((IAssetDatabase)MenuHelpers.GetSanitizedCloudTarget(storage).db).GetAsset<CinematicCameraAsset>(val);
		if ((AssetData)(object)asset != (IAssetData)null)
		{
			((Metadata<CinematicCameraSequence>)asset).Load();
			if (((Metadata<CinematicCameraSequence>)asset).target != null)
			{
				activeSequence = ((Metadata<CinematicCameraSequence>)asset).target;
				m_LastLoaded.Update(asset);
				m_TransformAnimationCurveBinding.Update(GetTransformCurves());
				m_ModifierAnimationCurveBinding.Update(activeSequence.modifiers.ToArray());
			}
		}
	}

	private void Delete(string hash, string storage)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Hash128 val = default(Hash128);
		((Hash128)(ref val))._002Ector(hash);
		((IAssetDatabase)MenuHelpers.GetSanitizedCloudTarget(storage).db).DeleteAsset(val);
		m_Assets.Update();
	}

	private void OnAssetsChanged(AssetChangedEventArgs args)
	{
		GameManager.instance.RunOnMainThread(delegate
		{
			m_Assets.Update();
		});
	}

	private void OnCloudTargetsChanged(AssetChangedEventArgs args)
	{
		GameManager.instance.RunOnMainThread(delegate
		{
			m_AvailableCloudTargetsBinding.Update();
			m_SelectedCloudTargetBinding.Update();
		});
	}

	private CinematicCameraAsset[] UpdateAssets()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return AssetDatabase.global.GetAssets<CinematicCameraAsset>(default(SearchFilter<CinematicCameraAsset>)).ToArray();
	}

	private void OnRemoveSelectedTransform(int curveIndex, int index)
	{
		OnRemoveKeyFrame("Transform", curveIndex, index);
	}

	private void OnRemoveKeyFrame(string id, int curveIndex, int index)
	{
		if (id == "Property")
		{
			string id2 = activeSequence.modifiers[curveIndex].id;
			activeSequence.RemoveModifierKey(id2, index);
			m_ModifierAnimationCurveBinding.Update(activeSequence.modifiers.ToArray());
		}
		else
		{
			activeSequence.RemoveCameraTransform(curveIndex, index);
			m_TransformAnimationCurveBinding.Update(GetTransformCurves());
		}
	}

	private void GetData(string id, out CinematicCameraSequence.CinematicCameraCurveModifier[] modifiers, out ValueBinding<CinematicCameraSequence.CinematicCameraCurveModifier[]> binding)
	{
		if (id == "Position")
		{
			modifiers = activeSequence.transforms.ToArray();
			binding = m_TransformAnimationCurveBinding;
		}
		else
		{
			modifiers = activeSequence.modifiers.ToArray();
			binding = m_ModifierAnimationCurveBinding;
		}
	}

	private int OnMoveKeyFrame(string id, int curveIndex, int index, Keyframe keyframe)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		GetData(id, out var modifiers, out var binding);
		CinematicCameraSequence.CinematicCameraCurveModifier modifier = modifiers[curveIndex];
		int result = activeSequence.MoveKeyframe(modifier, index, keyframe);
		binding.Update(modifiers);
		activeSequence.Refresh(t, (IDictionary<string, PhotoModeProperty>)m_PhotoModeRenderSystem.photoModeProperties, m_CameraUpdateSystem.activeCameraController);
		return result;
	}

	private void UpdatePlayback()
	{
		t += Time.unscaledDeltaTime;
		CinematicCameraSequence cinematicCameraSequence = m_ActiveAutoplaySequence ?? activeSequence;
		if (t >= cinematicCameraSequence.playbackDuration)
		{
			if (cinematicCameraSequence.loop)
			{
				t -= cinematicCameraSequence.playbackDuration;
			}
			else
			{
				playing = false;
			}
		}
		t = Mathf.Min(t, cinematicCameraSequence.playbackDuration);
		cinematicCameraSequence.Refresh(t, (IDictionary<string, PhotoModeProperty>)m_PhotoModeRenderSystem.photoModeProperties, m_CameraUpdateSystem.activeCameraController);
	}

	private void PausePlayback()
	{
		if (playing && (m_CameraUpdateSystem.activeCameraController is CinematicCameraController || (m_CameraUpdateSystem.activeCameraController is OrbitCameraController && m_CameraUpdateSystem.orbitCameraController.mode == OrbitCameraController.Mode.PhotoMode)))
		{
			playing = false;
		}
	}

	private void TogglePlayback()
	{
		playing = !playing;
		if (t > activeSequence.playbackDuration - 0.1f)
		{
			t = 0f;
		}
	}

	private void StopPlayback()
	{
		t = 0f;
		activeSequence.Refresh(t, (IDictionary<string, PhotoModeProperty>)m_PhotoModeRenderSystem.photoModeProperties, m_CameraUpdateSystem.activeCameraController);
		playing = false;
	}

	public void Autoplay(CinematicCameraAsset sequence)
	{
		m_ActiveAutoplaySequence = ((Metadata<CinematicCameraSequence>)sequence).target;
		m_ActiveAutoplaySequence.loop = true;
		t = 0f;
		playing = true;
	}

	public void StopAutoplay()
	{
		m_ActiveAutoplaySequence = null;
		t = 0f;
		playing = false;
	}

	private CinematicCameraSequence.CinematicCameraCurveModifier[] GetTransformCurves()
	{
		if (activeSequence.transformCount > 0)
		{
			List<CinematicCameraSequence.CinematicCameraCurveModifier> list = new List<CinematicCameraSequence.CinematicCameraCurveModifier>();
			CinematicCameraSequence.CinematicCameraCurveModifier[] transforms = activeSequence.transforms;
			for (int i = 0; i < transforms.Length; i++)
			{
				CinematicCameraSequence.CinematicCameraCurveModifier item = transforms[i];
				if (item.curve != null)
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}
		return kEmptyModifierArray;
	}

	private float[] GetControllerDelta()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = m_MoveAction.ReadValue<Vector2>() * Time.deltaTime;
		return new float[2] { val.x, val.y };
	}

	private float[] GetControllerPanDelta()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = m_RotateAction.ReadValue<Vector2>() * Time.deltaTime;
		return new float[2] { val.x, val.y };
	}

	private float GetControllerZoomDelta()
	{
		return m_ZoomAction.ReadValue<float>() * Time.deltaTime;
	}

	private void OnCurveEditorFocusChange(bool focused)
	{
		m_CameraUpdateSystem.orbitCameraController.inputEnabled = !focused;
		m_CameraUpdateSystem.cinematicCameraController.inputEnabled = !focused;
	}

	[Preserve]
	public CinematicCameraUISystem()
	{
	}
}

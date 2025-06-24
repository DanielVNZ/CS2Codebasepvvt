using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Colossal.IO.AssetDatabase;
using Colossal.UI.Binding;
using Game.Audio;
using Game.Audio.Radio;
using Game.Prefabs;
using Game.Rendering;
using Game.Settings;
using Game.UI.Localization;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class RadioUISystem : UISystemBase
{
	public class ClipInfo : IJsonWritable
	{
		public string title;

		[CanBeNull]
		public string info;

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin("radio.Clip");
			writer.PropertyName("title");
			writer.Write(title);
			writer.PropertyName("info");
			writer.Write(info);
			writer.TypeEnd();
		}
	}

	private const string kGroup = "radio";

	private PrefabSystem m_PrefabSystem;

	private Radio m_Radio;

	private GamePanelUISystem m_GamePanelUISystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private ValueBinding<bool> m_PausedBinding;

	private ValueBinding<bool> m_MutedBinding;

	private ValueBinding<bool> m_SkipAds;

	private GetterValueBinding<Radio.RadioNetwork[]> m_NetworksBinding;

	private GetterValueBinding<Radio.RuntimeRadioChannel[]> m_StationsBinding;

	private ValueBinding<ClipInfo> m_CurrentSegmentBinding;

	private EventBinding m_SegmentChangedBinding;

	private Dictionary<string, string> m_LastSelectedStations;

	private CachedLocalizedStringBuilder<string> m_EmergencyMessages;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Expected O, but got Unknown
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Expected O, but got Unknown
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Expected O, but got Unknown
		//IL_0317: Expected O, but got Unknown
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Expected O, but got Unknown
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Expected O, but got Unknown
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_Radio = AudioManager.instance.radio;
		m_GamePanelUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GamePanelUISystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_GamePanelUISystem.SetDefaultArgs(new RadioPanel());
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("radio", "enabled", (Func<bool>)(() => SharedSettings.instance.audio.radioActive), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("radio", "volume", (Func<float>)(() => SharedSettings.instance.audio.radioVolume), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddBinding((IBinding)(object)(m_PausedBinding = new ValueBinding<bool>("radio", "paused", m_Radio.paused, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddBinding((IBinding)(object)(m_MutedBinding = new ValueBinding<bool>("radio", "muted", m_Radio.muted, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddBinding((IBinding)(object)(m_SkipAds = new ValueBinding<bool>("radio", "skipAds", m_Radio.skipAds, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("radio", "emergencyMode", (Func<bool>)(() => m_Radio.hasEmergency), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("radio", "emergencyFocusable", (Func<bool>)(() => m_Radio.emergencyTarget != Entity.Null), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<Entity>("radio", "emergencyMessage", (Func<Entity>)(() => m_Radio.emergency), (IWriter<Entity>)(object)new DelegateWriter<Entity>((WriterDelegate<Entity>)WriteEmergencyMessage), (EqualityComparer<Entity>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<string>("radio", "selectedNetwork", (Func<string>)(() => AudioManager.instance.radio.currentChannel?.network), (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<string>("radio", "selectedStation", (Func<string>)(() => AudioManager.instance.radio.currentChannel?.name), (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null));
		AddBinding((IBinding)(object)(m_NetworksBinding = new GetterValueBinding<Radio.RadioNetwork[]>("radio", "networks", (Func<Radio.RadioNetwork[]>)(() => AudioManager.instance.radio.networkDescriptors), (IWriter<Radio.RadioNetwork[]>)(object)new ArrayWriter<Radio.RadioNetwork>((IWriter<Radio.RadioNetwork>)(object)new ValueWriter<Radio.RadioNetwork>(), false), (EqualityComparer<Radio.RadioNetwork[]>)null)));
		AddBinding((IBinding)(object)(m_StationsBinding = new GetterValueBinding<Radio.RuntimeRadioChannel[]>("radio", "stations", (Func<Radio.RuntimeRadioChannel[]>)(() => AudioManager.instance.radio.radioChannelDescriptors), (IWriter<Radio.RuntimeRadioChannel[]>)(object)new ArrayWriter<Radio.RuntimeRadioChannel>((IWriter<Radio.RuntimeRadioChannel>)(object)new ValueWriter<Radio.RuntimeRadioChannel>(), false), (EqualityComparer<Radio.RuntimeRadioChannel[]>)null)));
		AddBinding((IBinding)(object)(m_CurrentSegmentBinding = new ValueBinding<ClipInfo>("radio", "currentSegment", GetCurrentClipInfo(), (IWriter<ClipInfo>)(object)ValueWriters.Nullable<ClipInfo>((IWriter<ClipInfo>)(object)new ValueWriter<ClipInfo>()), (EqualityComparer<ClipInfo>)null)));
		EventBinding val = new EventBinding("radio", "segmentChanged");
		EventBinding binding = val;
		m_SegmentChangedBinding = val;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)new TriggerBinding<float>("radio", "setVolume", (Action<float>)SetVolume, (IReader<float>)null));
		AddBinding((IBinding)(object)new TriggerBinding<bool>("radio", "setPaused", (Action<bool>)SetPaused, (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<bool>("radio", "setMuted", (Action<bool>)SetMuted, (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<bool>("radio", "setSkipAds", (Action<bool>)SetSkipAds, (IReader<bool>)null));
		AddBinding((IBinding)new TriggerBinding("radio", "playPrevious", (Action)PlayPrevious));
		AddBinding((IBinding)new TriggerBinding("radio", "playNext", (Action)PlayNext));
		AddBinding((IBinding)new TriggerBinding("radio", "focusEmergency", (Action)FocusEmergency));
		AddBinding((IBinding)(object)new TriggerBinding<string>("radio", "selectNetwork", (Action<string>)SelectNetwork, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("radio", "selectStation", (Action<string>)SelectStation, (IReader<string>)null));
		m_EmergencyMessages = CachedLocalizedStringBuilder<string>.Id((string name) => "Radio.EMERGENCY_MESSAGE[" + name + "]");
		m_LastSelectedStations = new Dictionary<string, string>();
		Radio radio = m_Radio;
		radio.Reloaded = (Radio.OnRadioEvent)Delegate.Combine(radio.Reloaded, new Radio.OnRadioEvent(OnRadioReloaded));
		Radio radio2 = m_Radio;
		radio2.ProgramChanged = (Radio.OnRadioEvent)Delegate.Combine(radio2.ProgramChanged, new Radio.OnRadioEvent(OnProgramChanged));
		Radio radio3 = m_Radio;
		radio3.ClipChanged = (Radio.OnClipChanged)Delegate.Combine(radio3.ClipChanged, new Radio.OnClipChanged(OnClipChanged));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		Radio radio = m_Radio;
		radio.Reloaded = (Radio.OnRadioEvent)Delegate.Remove(radio.Reloaded, new Radio.OnRadioEvent(OnRadioReloaded));
		Radio radio2 = m_Radio;
		radio2.ProgramChanged = (Radio.OnRadioEvent)Delegate.Remove(radio2.ProgramChanged, new Radio.OnRadioEvent(OnProgramChanged));
		Radio radio3 = m_Radio;
		radio3.ClipChanged = (Radio.OnClipChanged)Delegate.Remove(radio3.ClipChanged, new Radio.OnClipChanged(OnClipChanged));
		base.OnDestroy();
	}

	private void WriteEmergencyMessage(IJsonWriter writer, Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (entity != Entity.Null)
		{
			PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(m_Radio.emergency);
			JsonWriterExtensions.Write<LocalizedString>(writer, m_EmergencyMessages[((Object)prefab).name]);
		}
		else
		{
			writer.WriteNull();
		}
	}

	private Metatag GetMetaType(Radio.SegmentType type)
	{
		return (Metatag)(type switch
		{
			Radio.SegmentType.Playlist => 2, 
			Radio.SegmentType.Commercial => 4, 
			_ => 2, 
		});
	}

	private ClipInfo GetClipInfo(Radio radio, AudioAsset asset)
	{
		if ((AssetData)(object)asset != (IAssetData)null)
		{
			if (asset.GetMetaTag((Metatag)3) == "Music")
			{
				return new ClipInfo
				{
					title = asset.GetMetaTag((Metatag)0),
					info = asset.GetMetaTag((Metatag)2)
				};
			}
			return new ClipInfo
			{
				title = radio.currentChannel.name,
				info = radio.currentChannel.currentProgram.name
			};
		}
		return null;
	}

	private ClipInfo GetCurrentClipInfo()
	{
		return GetClipInfo(m_Radio, m_Radio.currentClip.m_Asset);
	}

	private void OnClipChanged(Radio radio, AudioAsset asset)
	{
		m_StationsBinding.TriggerUpdate();
		m_CurrentSegmentBinding.Update(GetClipInfo(radio, asset));
	}

	private void OnRadioReloaded(Radio radio)
	{
		m_NetworksBinding.Update();
		m_StationsBinding.Update();
		m_SkipAds.Update(radio.skipAds);
	}

	private void OnProgramChanged(Radio radio)
	{
		m_StationsBinding.TriggerUpdate();
	}

	private void SetVolume(float volume)
	{
		SharedSettings.instance.audio.radioVolume = volume;
		SharedSettings.instance.audio.Apply();
	}

	private void SetPaused(bool paused)
	{
		m_Radio.paused = paused;
		m_PausedBinding.Update(paused);
	}

	private void SetMuted(bool muted)
	{
		m_Radio.muted = muted;
		m_MutedBinding.Update(muted);
	}

	private void SetSkipAds(bool skipAds)
	{
		m_Radio.skipAds = skipAds;
		m_SkipAds.Update(skipAds);
	}

	private void PlayPrevious()
	{
		AudioManager.instance.radio.PreviousSong();
	}

	private void PlayNext()
	{
		AudioManager.instance.radio.NextSong();
	}

	private void FocusEmergency()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_CameraUpdateSystem.orbitCameraController != (Object)null && m_Radio.emergencyTarget != Entity.Null)
		{
			m_CameraUpdateSystem.orbitCameraController.followedEntity = m_Radio.emergencyTarget;
			m_CameraUpdateSystem.orbitCameraController.TryMatchPosition(m_CameraUpdateSystem.activeCameraController);
			m_CameraUpdateSystem.activeCameraController = m_CameraUpdateSystem.orbitCameraController;
		}
	}

	private void SelectNetwork(string name)
	{
		if (m_LastSelectedStations.TryGetValue(name, out var value))
		{
			SelectStation(value);
			return;
		}
		Radio.RuntimeRadioChannel[] radioChannelDescriptors = AudioManager.instance.radio.radioChannelDescriptors;
		foreach (Radio.RuntimeRadioChannel runtimeRadioChannel in radioChannelDescriptors)
		{
			if (runtimeRadioChannel.network == name)
			{
				SelectStation(runtimeRadioChannel.name);
				break;
			}
		}
	}

	private void SelectStation(string name)
	{
		Radio.RuntimeRadioChannel radioChannel = AudioManager.instance.radio.GetRadioChannel(name);
		if (radioChannel != null)
		{
			Radio.RuntimeRadioChannel currentChannel = AudioManager.instance.radio.currentChannel;
			if (currentChannel != null)
			{
				m_LastSelectedStations[currentChannel.network] = currentChannel.name;
			}
			AudioManager.instance.radio.currentChannel = radioChannel;
		}
	}

	[Preserve]
	public RadioUISystem()
	{
	}
}

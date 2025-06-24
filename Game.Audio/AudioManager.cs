using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.Serialization.Entities;
using Game.Audio.Radio;
using Game.Common;
using Game.Effects;
using Game.Objects;
using Game.Prefabs;
using Game.Prefabs.Effects;
using Game.SceneFlow;
using Game.Serialization;
using Game.Settings;
using Game.Simulation;
using Game.Tools;
using Game.UI.InGame;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Scripting;

namespace Game.Audio;

[CompilerGenerated]
public class AudioManager : GameSystemBase, IDefaultSerializable, ISerializable, IPreDeserialize, IPreSerialize
{
	public static class AudioSourcePool
	{
		private static int s_InstanceCount;

		private static int s_LoadedSize;

		private static int s_PlayingSize;

		private static int s_MaxLoadedSize = 268435456;

		private static Stack<AudioSource> s_Pool = new Stack<AudioSource>();

		private static Dictionary<AudioClip, int> s_PlayingClips = new Dictionary<AudioClip, int>();

		private static List<AudioClip> s_UnloadClips = new List<AudioClip>();

		public static int memoryBudget
		{
			get
			{
				return s_MaxLoadedSize;
			}
			set
			{
				s_MaxLoadedSize = value;
				UnloadClips();
			}
		}

		public static void Stats(out int loadedSize, out int maxLoadedSize, out int loadedCount, out int playingSize, out int playingCount)
		{
			loadedSize = s_LoadedSize;
			maxLoadedSize = s_MaxLoadedSize;
			playingCount = s_PlayingClips.Count;
			loadedCount = playingCount + s_UnloadClips.Count;
			playingSize = s_PlayingSize;
		}

		public static void Reset()
		{
			s_LoadedSize = 0;
			s_PlayingSize = 0;
			s_PlayingClips.Clear();
			s_UnloadClips.Clear();
		}

		public static AudioSource Get()
		{
			if (s_Pool.Count > 0)
			{
				AudioSource val = s_Pool.Pop();
				if ((Object)(object)val != (Object)null)
				{
					((Component)val).gameObject.SetActive(true);
					return val;
				}
			}
			return CreateAudioSource();
			static AudioSource CreateAudioSource()
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				return new GameObject("AudioSource" + s_InstanceCount++).AddComponent<AudioSource>();
			}
		}

		public static void Play(AudioSource audioSource)
		{
			AddClip(audioSource.clip);
			audioSource.Play();
		}

		public static void PlayDelayed(AudioSource audioSource, float delay)
		{
			AddClip(audioSource.clip);
			audioSource.PlayDelayed(delay);
		}

		private static void AddClip(AudioClip audioClip)
		{
			if (audioClip.preloadAudioData)
			{
				return;
			}
			if (s_PlayingClips.TryGetValue(audioClip, out var value))
			{
				s_PlayingClips[audioClip] = value + 1;
				return;
			}
			int clipSize = GetClipSize(audioClip);
			if (!s_UnloadClips.Remove(audioClip))
			{
				s_LoadedSize += clipSize;
				UnloadClips();
			}
			s_PlayingClips.Add(audioClip, 1);
			s_PlayingSize += clipSize;
		}

		private static void UnloadClips()
		{
			int num = 0;
			while (s_LoadedSize > s_MaxLoadedSize && num < s_UnloadClips.Count)
			{
				AudioClip val = s_UnloadClips[num++];
				val.UnloadAudioData();
				s_LoadedSize -= GetClipSize(val);
			}
			if (num > 0)
			{
				s_UnloadClips.RemoveRange(0, num);
			}
		}

		private static void RemoveClip(AudioClip audioClip)
		{
			if (!s_PlayingClips.TryGetValue(audioClip, out var value))
			{
				return;
			}
			if (--value == 0)
			{
				int clipSize = GetClipSize(audioClip);
				s_PlayingClips.Remove(audioClip);
				s_PlayingSize -= clipSize;
				if (s_LoadedSize > s_MaxLoadedSize)
				{
					audioClip.UnloadAudioData();
					s_LoadedSize -= clipSize;
				}
				else
				{
					s_UnloadClips.Add(audioClip);
				}
			}
			else
			{
				s_PlayingClips[audioClip] = value;
			}
		}

		private static int GetClipSize(AudioClip audioClip)
		{
			return audioClip.samples * audioClip.channels * 2;
		}

		public static void Release(AudioSource audioSource)
		{
			if ((Object)(object)audioSource != (Object)null)
			{
				AudioClip clip = audioSource.clip;
				audioSource.Stop();
				((Component)audioSource).gameObject.SetActive(false);
				audioSource.clip = null;
				audioSource.volume = 1f;
				audioSource.pitch = 0f;
				audioSource.loop = false;
				audioSource.spatialBlend = 1f;
				s_Pool.Push(audioSource);
				RemoveClip(clip);
			}
		}
	}

	private enum FadeStatus
	{
		None,
		FadeIn,
		FadeOut
	}

	private struct AudioInfo
	{
		public SourceInfo m_SourceInfo;

		public Entity m_SFXEntity;

		public AudioSource m_AudioSource;

		public FadeStatus m_Status;

		public float m_MaxVolume;

		public float3 m_Velocity;
	}

	private class CameraAmbientAudioInfo
	{
		public int id;

		public float height;

		public AudioSource source;

		public Transform transform;
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Game.Tools.EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<AudioSourceData> __Game_Prefabs_AudioSourceData_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Effect> __Game_Prefabs_Effect_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<EnabledEffect> __Game_Effects_EnabledEffect_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<AudioEffectData> __Game_Prefabs_AudioEffectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EffectInstance> __Game_Effects_EffectInstance_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<VehicleAudioEffectData> __Game_Prefabs_VehicleAudioEffectData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Tools.EditorContainer>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_AudioSourceData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AudioSourceData>(true);
			__Game_Prefabs_Effect_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Effect>(true);
			__Game_Effects_EnabledEffect_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<EnabledEffect>(true);
			__Game_Prefabs_AudioEffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AudioEffectData>(true);
			__Game_Effects_EffectInstance_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EffectInstance>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Prefabs_VehicleAudioEffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VehicleAudioEffectData>(true);
		}
	}

	private const float kDopplerLevelReduceFactor = 0.3f;

	private static readonly ILog log = LogManager.GetLogger("Audio");

	private List<AudioInfo> m_AudioInfos = new List<AudioInfo>();

	private const string kMasterVolumeProperty = "MasterVolume";

	private const string kRadioVolumeProperty = "RadioVolume";

	private const string kUIVolumeProperty = "UIVolume";

	private const string kMenuVolumeProperty = "MenuVolume";

	private const string kInGameVolumeProperty = "InGameVolume";

	private const string kAmbienceVolumeProperty = "AmbienceVolume";

	private const string kDisastersVolumeProperty = "DisastersVolume";

	private const string kWorldVolumeProperty = "WorldVolume";

	private const string kAudioGroupsVolumeProperty = "AudioGroupsVolume";

	private const string kServiceBuildingsVolumeProperty = "ServiceBuildingsVolume";

	private SynchronizationContext m_MainThreadContext;

	private AudioMixer m_Mixer;

	private AudioMixerGroup m_AmbientGroup;

	private AudioMixerGroup m_InGameGroup;

	private AudioMixerGroup m_RadioGroup;

	private AudioMixerGroup m_UIGroup;

	private AudioMixerGroup m_MenuGroup;

	private AudioMixerGroup m_WorldGroup;

	private AudioMixerGroup m_ServiceBuildingGroup;

	private AudioMixerGroup m_AudioGroupGroup;

	private AudioMixerGroup m_DisasterGroup;

	private AudioLoop m_MainMenuMusic;

	private AudioSource m_UIAudioSource;

	private AudioSource m_UIHtmlAudioSource;

	private AudioListener m_AudioListener;

	private NativeQueue<SourceUpdateInfo> m_SourceUpdateQueue;

	private SourceUpdateData m_SourceUpdateData;

	private JobHandle m_SourceUpdateWriter;

	private SimulationSystem m_SimulationSystem;

	private PrefabSystem m_PrefabSystem;

	private GameScreenUISystem m_GameScreenUISystem;

	private EffectControlSystem m_EffectControlSystem;

	private RandomSeed m_RandomSeed;

	private float m_FadeOutMenu;

	private float m_DeltaTime;

	private bool m_IsGamePausedLastUpdate;

	private bool m_IsMenuActivatedLastUpdate;

	private bool m_ShouldUnpauseRadioAfterGameUnpaused;

	private string m_LastSaveRadioChannel;

	private bool m_LastSaveRadioSkipAds;

	private FadeStatus m_AudioFadeStatus;

	private TimeSystem m_TimeSystem;

	private List<SFX> m_Clips = new List<SFX>();

	private NativeParallelHashMap<SourceInfo, int> m_CurrentEffects;

	private EntityQuery m_AmbientSettingsQuery;

	private EntityQuery m_SoundQuery;

	private EntityQuery m_WeatherAudioEntitiyQuery;

	private List<CameraAmbientAudioInfo> m_CameraAmbientSources = new List<CameraAmbientAudioInfo>();

	private List<AudioSource> m_TempAudioSources = new List<AudioSource>();

	private Game.Audio.Radio.Radio m_Radio;

	private int m_PlayCount;

	private TypeHandle __TypeHandle;

	public static AudioManager instance { get; private set; }

	public AudioSource UIHtmlAudioSource
	{
		get
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			if ((Object)(object)m_UIAudioSource == (Object)null)
			{
				GameObject val = new GameObject("UIHtmlAudioSource");
				m_UIHtmlAudioSource = val.AddComponent<AudioSource>();
				m_UIHtmlAudioSource.outputAudioMixerGroup = m_UIGroup;
				m_UIHtmlAudioSource.dopplerLevel = 0f;
				m_UIHtmlAudioSource.playOnAwake = false;
				m_UIHtmlAudioSource.spatialBlend = 0f;
				m_UIHtmlAudioSource.ignoreListenerPause = true;
				Object.DontDestroyOnLoad((Object)(object)val);
			}
			return m_UIHtmlAudioSource;
		}
	}

	public Game.Audio.Radio.Radio radio => m_Radio;

	public Entity followed { private get; set; }

	public float masterVolume
	{
		get
		{
			return GetVolume("MasterVolume");
		}
		set
		{
			SetVolume("MasterVolume", value);
		}
	}

	public float radioVolume
	{
		get
		{
			return GetVolume("RadioVolume");
		}
		set
		{
			SetVolume("RadioVolume", value);
		}
	}

	public float uiVolume
	{
		get
		{
			return GetVolume("UIVolume");
		}
		set
		{
			SetVolume("UIVolume", value);
		}
	}

	public float menuVolume
	{
		get
		{
			return GetVolume("MenuVolume");
		}
		set
		{
			SetVolume("MenuVolume", value);
		}
	}

	public float ingameVolume
	{
		get
		{
			return GetVolume("InGameVolume");
		}
		set
		{
			SetVolume("InGameVolume", value);
		}
	}

	public float ambienceVolume
	{
		get
		{
			return GetVolume("AmbienceVolume");
		}
		set
		{
			SetVolume("AmbienceVolume", value);
		}
	}

	public float disastersVolume
	{
		get
		{
			return GetVolume("DisastersVolume");
		}
		set
		{
			SetVolume("DisastersVolume", value);
		}
	}

	public float worldVolume
	{
		get
		{
			return GetVolume("WorldVolume");
		}
		set
		{
			SetVolume("WorldVolume", value);
		}
	}

	public float audioGroupsVolume
	{
		get
		{
			return GetVolume("AudioGroupsVolume");
		}
		set
		{
			SetVolume("AudioGroupsVolume", value);
		}
	}

	public float serviceBuildingsVolume
	{
		get
		{
			return GetVolume("ServiceBuildingsVolume");
		}
		set
		{
			SetVolume("ServiceBuildingsVolume", value);
		}
	}

	public int RegisterSFX(SFX sfx)
	{
		int num = m_Clips.IndexOf(sfx);
		if (num == -1)
		{
			int count = m_Clips.Count;
			m_Clips.Add(sfx);
			return count;
		}
		return num;
	}

	private void SetVolume(string volumeProperty, float value)
	{
		if (GameManager.instance.gameMode == GameMode.Game && m_GameScreenUISystem.isMenuActive && m_AudioFadeStatus == FadeStatus.None)
		{
			switch (volumeProperty)
			{
			case "WorldVolume":
				return;
			case "AudioGroupsVolume":
				return;
			case "AmbienceVolume":
				return;
			case "ServiceBuildingsVolume":
				return;
			case "RadioVolume":
				return;
			}
		}
		m_Mixer.SetFloat(volumeProperty, Mathf.Log10(Mathf.Min(Mathf.Max(value, 0.0001f), 1f)) * 20f);
	}

	private float GetVolume(string volumeProperty)
	{
		float num = default(float);
		if (m_Mixer.GetFloat(volumeProperty, ref num))
		{
			return Mathf.Pow(10f, num / 20f);
		}
		return 1f;
	}

	public void MoveAudioListenerForDoppler(float3 m_FollowOffset)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)m_AudioListener).transform;
		transform.position += new Vector3(m_FollowOffset.x, m_FollowOffset.y, m_FollowOffset.z);
	}

	public void UpdateAudioListener(Vector3 position, Quaternion rotation)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_AudioListener != (Object)null && GameManager.instance.gameMode == GameMode.Game && !GameManager.instance.isGameLoading)
		{
			((Behaviour)m_AudioListener).enabled = false;
			((Component)m_AudioListener).transform.position = position;
			((Component)m_AudioListener).transform.rotation = rotation;
			((Behaviour)m_AudioListener).enabled = true;
			if (m_CameraAmbientSources.Count > 0)
			{
				UpdateGlobalAudioSources(((Component)m_AudioListener).transform);
			}
		}
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		m_Radio.Disable();
		Reset();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		base.OnGameLoaded(serializationContext);
		Purpose purpose = ((Context)(ref serializationContext)).purpose;
		if ((int)purpose == 1 || (int)purpose == 2)
		{
			m_Radio.RestoreRadioSettings(m_LastSaveRadioChannel, m_LastSaveRadioSkipAds);
			m_Radio.Reload();
		}
	}

	protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoadingComplete(purpose, mode);
		if (mode.IsGameOrEditor())
		{
			StopMenuMusic();
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		instance = this;
		AudioSourcePool.Reset();
		m_MainThreadContext = SynchronizationContext.Current;
		m_AmbientSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<AmbientAudioSettingsData>(),
			ComponentType.ReadOnly<AmbientAudioEffect>()
		});
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		m_WeatherAudioEntitiyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WeatherAudioData>() });
		m_AudioListener = new GameObject("AudioListener").AddComponent<AudioListener>();
		Object.DontDestroyOnLoad((Object)(object)m_AudioListener);
		m_Mixer = Resources.Load<AudioMixer>("Audio/MasterMixer");
		m_AmbientGroup = m_Mixer.FindMatchingGroups("Ambience")[0];
		m_RadioGroup = m_Mixer.FindMatchingGroups("Radio")[0];
		m_InGameGroup = m_Mixer.FindMatchingGroups("InGame")[0];
		m_UIGroup = m_Mixer.FindMatchingGroups("UI")[0];
		m_MenuGroup = m_Mixer.FindMatchingGroups("Menu")[0];
		m_WorldGroup = m_Mixer.FindMatchingGroups("World")[0];
		m_ServiceBuildingGroup = m_Mixer.FindMatchingGroups("ServiceBuildings")[0];
		m_AudioGroupGroup = m_Mixer.FindMatchingGroups("AudioGroups")[0];
		m_DisasterGroup = m_Mixer.FindMatchingGroups("Disasters")[0];
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_GameScreenUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GameScreenUISystem>();
		m_EffectControlSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EffectControlSystem>();
		m_Radio = new Game.Audio.Radio.Radio(m_RadioGroup);
		m_SourceUpdateQueue = new NativeQueue<SourceUpdateInfo>(AllocatorHandle.op_Implicit((Allocator)4));
		m_SourceUpdateData = new SourceUpdateData(m_SourceUpdateQueue.AsParallelWriter());
		m_CurrentEffects = new NativeParallelHashMap<SourceInfo, int>(128, AllocatorHandle.op_Implicit((Allocator)4));
		m_RandomSeed = default(RandomSeed);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_MainMenuMusic?.Dispose();
		m_Radio?.Disable();
		m_CurrentEffects.Dispose();
		if (m_SourceUpdateQueue.IsCreated)
		{
			((JobHandle)(ref m_SourceUpdateWriter)).Complete();
			m_SourceUpdateQueue.Dispose();
		}
		m_TempAudioSources.Clear();
		base.OnDestroy();
		ClearCameraAmbientSources();
	}

	private void ClearCameraAmbientSources()
	{
		foreach (CameraAmbientAudioInfo item in m_CameraAmbientSources)
		{
			AudioSourcePool.Release(item.source);
		}
		m_CameraAmbientSources.Clear();
	}

	public void Reset()
	{
		ClearCameraAmbientSources();
		for (int i = 0; i < m_AudioInfos.Count; i++)
		{
			AudioSourcePool.Release(m_AudioInfos[i].m_AudioSource);
		}
		m_CurrentEffects.Clear();
		m_AudioInfos.Clear();
	}

	public async Task ResetAudioOnMainThread()
	{
		TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();
		m_MainThreadContext.Send(delegate
		{
			Reset();
			taskCompletion.SetResult(result: true);
		}, null);
		await taskCompletion.Task;
	}

	public void SetGlobalAudioSettings()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		ClearCameraAmbientSources();
		if (((EntityQuery)(ref m_AmbientSettingsQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		Entity singletonEntity = ((EntityQuery)(ref m_AmbientSettingsQuery)).GetSingletonEntity();
		EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
		DynamicBuffer<AmbientAudioEffect> buffer = ((EntityManager)(ref entityManager)).GetBuffer<AmbientAudioEffect>(singletonEntity, true);
		entityManager = ((ComponentSystemBase)this).World.EntityManager;
		AmbientAudioSettingsData componentData = ((EntityManager)(ref entityManager)).GetComponentData<AmbientAudioSettingsData>(singletonEntity);
		float num = (componentData.m_MaxHeight - componentData.m_MinHeight) / (float)(buffer.Length + 1);
		float num2 = componentData.m_MinHeight + num * (float)buffer.Length - num * (1f - componentData.m_OverlapRatio);
		for (int i = 0; i < buffer.Length; i++)
		{
			EffectPrefab prefab = m_PrefabSystem.GetPrefab<EffectPrefab>(buffer[i].m_Effect);
			if ((Object)(object)prefab != (Object)null)
			{
				SFX component = prefab.GetComponent<SFX>();
				AudioSource val = AudioSourcePool.Get();
				SetAudioSourceData(val, component, component.m_Volume);
				UpdateAudioSource(val, component, new Transform
				{
					m_Position = float3.zero,
					m_Rotation = quaternion.identity
				}, 1f, disableDoppler: true);
				List<CameraAmbientAudioInfo> list = m_CameraAmbientSources;
				CameraAmbientAudioInfo cameraAmbientAudioInfo = new CameraAmbientAudioInfo();
				cameraAmbientAudioInfo.id = i;
				cameraAmbientAudioInfo.height = num2;
				cameraAmbientAudioInfo.source = val;
				cameraAmbientAudioInfo.transform = ((Component)val).transform;
				list.Add(cameraAmbientAudioInfo);
				val.maxDistance = num * componentData.m_OverlapRatio;
				val.minDistance = val.maxDistance * componentData.m_MinDistanceRatio;
				AudioSourcePool.Play(val);
				num2 -= num;
			}
		}
	}

	private void SetGlobalAudioSourcePosition(CameraAmbientAudioInfo info, float3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		float3 val = position;
		if (info.id == 0)
		{
			val.y = math.max(info.height, position.y);
		}
		else
		{
			val.y = info.height;
		}
		info.transform.position = float3.op_Implicit(val);
	}

	public void UpdateGlobalAudioSources(Transform cameraTransform)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (m_CameraAmbientSources.Count > 0 && (Object)(object)m_CameraAmbientSources[0].source == (Object)null)
		{
			SetGlobalAudioSettings();
		}
		for (int i = 0; i < m_CameraAmbientSources.Count; i++)
		{
			SetGlobalAudioSourcePosition(m_CameraAmbientSources[i], float3.op_Implicit(cameraTransform.position));
		}
	}

	public SourceUpdateData GetSourceUpdateData(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		deps = m_SourceUpdateWriter;
		return m_SourceUpdateData;
	}

	public void AddSourceUpdateWriter(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_SourceUpdateWriter = JobHandle.CombineDependencies(m_SourceUpdateWriter, jobHandle);
	}

	public void StopMenuMusic()
	{
		m_MainMenuMusic?.FadeOut();
	}

	public bool PlayUISoundIfNotPlaying(Entity clipEntity, float volume = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<AudioRandomizeData> val = default(DynamicBuffer<AudioRandomizeData>);
		if (((EntityManager)(ref entityManager)).HasComponent<AudioRandomizeData>(clipEntity) && EntitiesExtensions.TryGetBuffer<AudioRandomizeData>(((ComponentSystemBase)this).EntityManager, clipEntity, true, ref val))
		{
			Random val2 = default(Random);
			((Random)(ref val2))._002Ector((uint)DateTime.Now.Ticks);
			int num = ((Random)(ref val2)).NextInt(val.Length);
			Entity sFXEntity = val[num].m_SFXEntity;
			List<SFX> list = m_Clips;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			SFX sFX = list[((EntityManager)(ref entityManager)).GetComponentData<AudioEffectData>(sFXEntity).m_AudioClipId];
			PlayUISoundIfNotPlaying(sFX.m_AudioClip, volume);
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<AudioEffectData>(clipEntity))
		{
			List<SFX> list2 = m_Clips;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			SFX sFX2 = list2[((EntityManager)(ref entityManager)).GetComponentData<AudioEffectData>(clipEntity).m_AudioClipId];
			volume = sFX2.m_Volume * volume;
			return PlayUISoundIfNotPlaying(sFX2.m_AudioClip, volume);
		}
		return false;
	}

	public void PlayUISound(Entity clipEntity, float volume = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<AudioRandomizeData> val = default(DynamicBuffer<AudioRandomizeData>);
		if (((EntityManager)(ref entityManager)).HasComponent<AudioRandomizeData>(clipEntity) && EntitiesExtensions.TryGetBuffer<AudioRandomizeData>(((ComponentSystemBase)this).EntityManager, clipEntity, true, ref val))
		{
			Random val2 = default(Random);
			((Random)(ref val2))._002Ector((uint)DateTime.Now.Ticks);
			int num = ((Random)(ref val2)).NextInt(val.Length);
			Entity sFXEntity = val[num].m_SFXEntity;
			List<SFX> list = m_Clips;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			SFX sFX = list[((EntityManager)(ref entityManager)).GetComponentData<AudioEffectData>(sFXEntity).m_AudioClipId];
			PlayUISound(sFX.m_AudioClip, sFX.m_Volume * volume);
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<AudioEffectData>(clipEntity))
		{
			List<SFX> list2 = m_Clips;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			SFX sFX2 = list2[((EntityManager)(ref entityManager)).GetComponentData<AudioEffectData>(clipEntity).m_AudioClipId];
			PlayUISound(sFX2.m_AudioClip, sFX2.m_Volume * volume);
		}
	}

	public bool PlayUISoundIfNotPlaying(AudioClip clipEntity, float volume = 1f)
	{
		if ((Object)(object)m_UIAudioSource == (Object)null || !m_UIAudioSource.isPlaying)
		{
			PlayUISound(clipEntity, volume);
			return true;
		}
		return false;
	}

	public void PlayUISound(AudioClip clip, float volume = 1f)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		if ((Object)(object)clip != (Object)null)
		{
			if ((Object)(object)m_UIAudioSource == (Object)null)
			{
				GameObject val = new GameObject("UIAudioSource");
				m_UIAudioSource = val.AddComponent<AudioSource>();
				m_UIAudioSource.outputAudioMixerGroup = m_UIGroup;
				m_UIAudioSource.dopplerLevel = 0f;
				m_UIAudioSource.playOnAwake = false;
				m_UIAudioSource.spatialBlend = 0f;
				m_UIAudioSource.ignoreListenerPause = true;
				Object.DontDestroyOnLoad((Object)(object)val);
			}
			m_UIAudioSource.PlayOneShot(clip, volume);
		}
		else
		{
			log.WarnFormat("PlayUISound invoked with no audio clip", Array.Empty<object>());
		}
	}

	public AudioSource PlayExclusiveUISound(Entity clipEntity)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		AudioSource val = null;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<AudioEffectData>(clipEntity))
		{
			List<SFX> list = m_Clips;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			SFX sFX = list[((EntityManager)(ref entityManager)).GetComponentData<AudioEffectData>(clipEntity).m_AudioClipId];
			if ((Object)(object)sFX.m_AudioClip != (Object)null)
			{
				val = AudioSourcePool.Get();
				val.loop = sFX.m_Loop;
				val.pitch = sFX.m_Pitch;
				val.volume = sFX.m_Volume;
				val.outputAudioMixerGroup = m_UIGroup;
				val.dopplerLevel = 0f;
				val.playOnAwake = false;
				val.spatialBlend = 0f;
				val.ignoreListenerPause = true;
				val.clip = sFX.m_AudioClip;
				val.timeSamples = 0;
				AudioSourcePool.Play(val);
			}
			else
			{
				log.WarnFormat("PlayUISound invoked with no audio clip", Array.Empty<object>());
			}
		}
		return val;
	}

	public void StopExclusiveUISound(AudioSource audioSource)
	{
		if ((Object)(object)audioSource != (Object)null)
		{
			AudioSourcePool.Release(audioSource);
		}
	}

	public async Task PlayMenuMusic(string tag)
	{
		AudioAsset randomAsset = AssetExtensions.GetRandomAsset<AudioAsset>((IAssetDatabase)(object)AssetDatabase.global, SearchFilter<AudioAsset>.ByCondition((Func<AudioAsset, bool>)((AudioAsset asset) => ((AssetData)asset).ContainsTag(tag)), false));
		if ((AssetData)(object)randomAsset != (IAssetData)null)
		{
			m_MainMenuMusic = new AudioLoop(randomAsset, m_Mixer, m_MenuGroup);
			await m_MainMenuMusic.Start(m_PlayCount > 0);
			m_Radio?.Disable();
			m_PlayCount++;
		}
	}

	private void UpdateMenuMusic()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		AudioLoop audioLoop = m_MainMenuMusic;
		if (audioLoop != null)
		{
			WorldUnmanaged worldUnmanaged = ((SystemState)(ref ((SystemBase)this).CheckedStateRef)).WorldUnmanaged;
			audioLoop.Update(((WorldUnmanaged)(ref worldUnmanaged)).Time.DeltaTime);
		}
	}

	private bool GetEffect(DynamicBuffer<EnabledEffect> effects, int effectIndex, out EnabledEffect effect)
	{
		for (int i = 0; i < effects.Length; i++)
		{
			effect = effects[i];
			if (effect.m_EffectIndex == effectIndex)
			{
				return true;
			}
		}
		effect = default(EnabledEffect);
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		m_DeltaTime = Time.deltaTime;
		UpdateMenuMusic();
		if (GameManager.instance.isGameLoading)
		{
			return;
		}
		if (GameManager.instance.gameMode != GameMode.Game)
		{
			((JobHandle)(ref m_SourceUpdateWriter)).Complete();
			m_SourceUpdateQueue.Clear();
			return;
		}
		if (m_CameraAmbientSources.Count == 0)
		{
			SetGlobalAudioSettings();
		}
		if (m_TempAudioSources.Count > 0)
		{
			UpdateTempAudioSources();
		}
		UpdateGameAudioSetting();
		m_Radio.Update(m_TimeSystem.normalizedTime);
		Camera main = Camera.main;
		if ((Object)(object)main == (Object)null)
		{
			return;
		}
		ComponentLookup<Game.Tools.EditorContainer> componentLookup = InternalCompilerInterface.GetComponentLookup<Game.Tools.EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<PrefabRef> componentLookup2 = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<AudioSourceData> bufferLookup = InternalCompilerInterface.GetBufferLookup<AudioSourceData>(ref __TypeHandle.__Game_Prefabs_AudioSourceData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<Effect> bufferLookup2 = InternalCompilerInterface.GetBufferLookup<Effect>(ref __TypeHandle.__Game_Prefabs_Effect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<EnabledEffect> bufferLookup3 = InternalCompilerInterface.GetBufferLookup<EnabledEffect>(ref __TypeHandle.__Game_Effects_EnabledEffect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<AudioEffectData> componentLookup3 = InternalCompilerInterface.GetComponentLookup<AudioEffectData>(ref __TypeHandle.__Game_Prefabs_AudioEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<EffectInstance> componentLookup4 = InternalCompilerInterface.GetComponentLookup<EffectInstance>(ref __TypeHandle.__Game_Effects_EffectInstance_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		((JobHandle)(ref m_SourceUpdateWriter)).Complete();
		Random random = m_RandomSeed.GetRandom((int)m_SimulationSystem.frameIndex);
		JobHandle dependencies;
		NativeList<EnabledEffectData> enabledData = m_EffectControlSystem.GetEnabledData(readOnly: true, out dependencies);
		((JobHandle)(ref dependencies)).Complete();
		SourceUpdateInfo sourceUpdateInfo = default(SourceUpdateInfo);
		DynamicBuffer<Effect> val2 = default(DynamicBuffer<Effect>);
		Game.Tools.EditorContainer editorContainer = default(Game.Tools.EditorContainer);
		while (m_SourceUpdateQueue.TryDequeue(ref sourceUpdateInfo))
		{
			SourceInfo sourceInfo = sourceUpdateInfo.m_SourceInfo;
			bool flag = m_CurrentEffects.ContainsKey(sourceInfo);
			if (sourceUpdateInfo.m_Type == SourceUpdateType.Add)
			{
				if (!flag)
				{
					if (!componentLookup2.HasComponent(sourceInfo.m_Entity))
					{
						continue;
					}
					Entity val = componentLookup2[sourceInfo.m_Entity].m_Prefab;
					float num = 0f;
					bool flag2 = false;
					if (sourceInfo.m_EffectIndex != -1)
					{
						DynamicBuffer<EnabledEffect> effects = bufferLookup3[sourceInfo.m_Entity];
						if (bufferLookup2.TryGetBuffer(val, ref val2))
						{
							if (GetEffect(effects, sourceInfo.m_EffectIndex, out var effect))
							{
								Effect effect2 = val2[sourceInfo.m_EffectIndex];
								EnabledEffectData enabledEffectData = enabledData[effect.m_EnabledIndex];
								num = enabledEffectData.m_Intensity;
								flag2 = (enabledEffectData.m_Flags & EnabledEffectFlags.AudioDisabled) != 0;
								val = effect2.m_Effect;
							}
						}
						else if (componentLookup.TryGetComponent(sourceInfo.m_Entity, ref editorContainer) && effects.Length != 0)
						{
							EnabledEffectData enabledEffectData2 = enabledData[effects[0].m_EnabledIndex];
							num = enabledEffectData2.m_Intensity;
							flag2 = (enabledEffectData2.m_Flags & EnabledEffectFlags.AudioDisabled) != 0;
							val = editorContainer.m_Prefab;
						}
					}
					else
					{
						num = componentLookup4[sourceInfo.m_Entity].m_Intensity;
					}
					if (bufferLookup.HasBuffer(val))
					{
						DynamicBuffer<AudioSourceData> val3 = bufferLookup[val];
						int num2 = ((Random)(ref random)).NextInt(val3.Length);
						Entity sFXEntity = val3[num2].m_SFXEntity;
						int audioClipId = componentLookup3[sFXEntity].m_AudioClipId;
						SFX sFX = m_Clips[audioClipId];
						float num3 = sFX.m_Volume * num;
						if (num3 > 0.001f && !flag2)
						{
							num3 = GetFadedVolume(FadeStatus.FadeIn, sFX.m_FadeTimes, 0f, num3);
							AudioSource audioSource = AudioSourcePool.Get();
							SetAudioSourceData(audioSource, m_Clips[audioClipId], num3);
							m_CurrentEffects.Add(sourceInfo, m_AudioInfos.Count);
							AudioSourcePool.Play(audioSource);
							m_AudioInfos.Add(new AudioInfo
							{
								m_SourceInfo = sourceInfo,
								m_SFXEntity = sFXEntity,
								m_AudioSource = audioSource,
								m_Status = FadeStatus.FadeIn
							});
						}
					}
				}
				else
				{
					int index = m_CurrentEffects[sourceInfo];
					AudioInfo value = m_AudioInfos[index];
					value.m_Status = FadeStatus.FadeIn;
					m_AudioInfos[index] = value;
				}
			}
			else if (sourceUpdateInfo.m_Type == SourceUpdateType.Remove)
			{
				if (flag)
				{
					int index2 = m_CurrentEffects[sourceInfo];
					Fadeout(sourceInfo, index2);
				}
			}
			else if (sourceUpdateInfo.m_Type == SourceUpdateType.WrongPrefab)
			{
				if (flag)
				{
					int num4 = m_CurrentEffects[sourceInfo];
					m_CurrentEffects.Remove(sourceInfo);
					sourceInfo.m_EffectIndex = -2 - sourceInfo.m_EffectIndex;
					while (!m_CurrentEffects.TryAdd(sourceInfo, num4))
					{
						sourceInfo.m_EffectIndex--;
					}
					Fadeout(sourceInfo, num4);
				}
			}
			else if (sourceUpdateInfo.m_Type == SourceUpdateType.Temp)
			{
				if (bufferLookup.HasBuffer(sourceInfo.m_Entity))
				{
					DynamicBuffer<AudioSourceData> val4 = bufferLookup[sourceInfo.m_Entity];
					int num5 = ((Random)(ref random)).NextInt(val4.Length);
					Entity sFXEntity2 = val4[num5].m_SFXEntity;
					int audioClipId2 = componentLookup3[sFXEntity2].m_AudioClipId;
					SFX sFX2 = m_Clips[audioClipId2];
					float volume = sFX2.m_Volume;
					float num6 = math.distance(float3.op_Implicit(((Component)main).transform.position), sourceUpdateInfo.m_Transform.m_Position);
					if (volume > 0.001f && num6 < sFX2.m_MinMaxDistance.y)
					{
						volume = GetFadedVolume(FadeStatus.FadeIn, sFX2.m_FadeTimes, 0f, volume);
						AudioSource val5 = AudioSourcePool.Get();
						SetAudioSourceData(val5, m_Clips[audioClipId2], volume);
						((Component)val5).transform.position = float3.op_Implicit(sourceUpdateInfo.m_Transform.m_Position);
						AudioSourcePool.Play(val5);
						m_TempAudioSources.Add(val5);
					}
				}
			}
			else if (sourceUpdateInfo.m_Type == SourceUpdateType.Snap)
			{
				Entity snapSound = ((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_SnapSound;
				PlayUISound(snapSound);
			}
		}
		JobHandle dependency = ((SystemBase)this).Dependency;
		((JobHandle)(ref dependency)).Complete();
		SyncAudioSources();
	}

	private void UpdateGameAudioSetting()
	{
		if (m_SimulationSystem.selectedSpeed == 0f && !m_IsGamePausedLastUpdate)
		{
			m_AudioFadeStatus = FadeStatus.FadeOut;
		}
		else if (m_SimulationSystem.selectedSpeed != 0f && m_IsGamePausedLastUpdate)
		{
			m_AudioFadeStatus = FadeStatus.FadeIn;
			disastersVolume = 0.0001f;
			worldVolume = 0.0001f;
		}
		if (!m_IsMenuActivatedLastUpdate && m_GameScreenUISystem.isMenuActive)
		{
			m_AudioFadeStatus = FadeStatus.FadeOut;
			m_ShouldUnpauseRadioAfterGameUnpaused = m_Radio.hasEmergency || !m_Radio.paused;
		}
		else if (m_IsMenuActivatedLastUpdate && !m_GameScreenUISystem.isMenuActive)
		{
			m_AudioFadeStatus = FadeStatus.FadeIn;
			ambienceVolume = 0.0001f;
			serviceBuildingsVolume = 0.0001f;
			audioGroupsVolume = 0.0001f;
			radioVolume = 0.0001f;
			m_Radio.ForceRadioPause(!m_ShouldUnpauseRadioAfterGameUnpaused);
		}
		if (m_AudioFadeStatus == FadeStatus.FadeOut)
		{
			m_AudioFadeStatus = FadeStatus.None;
			if (disastersVolume > 0.0001f)
			{
				m_Mixer.SetFloat("DisastersVolume", Mathf.Log10(Mathf.Min(Mathf.Max(disastersVolume - Time.deltaTime / 1f, 0.0001f), 1f)) * 20f);
				m_AudioFadeStatus = FadeStatus.FadeOut;
			}
			if (worldVolume > 0.0001f)
			{
				m_Mixer.SetFloat("WorldVolume", Mathf.Log10(Mathf.Min(Mathf.Max(worldVolume - Time.deltaTime / 1f, 0.0001f), 1f)) * 20f);
				m_AudioFadeStatus = FadeStatus.FadeOut;
			}
			if (m_GameScreenUISystem.isMenuActive)
			{
				if (serviceBuildingsVolume > 0.0001f)
				{
					m_Mixer.SetFloat("ServiceBuildingsVolume", Mathf.Log10(Mathf.Min(Mathf.Max(serviceBuildingsVolume - Time.deltaTime / 1f, 0.0001f), 1f)) * 20f);
					m_AudioFadeStatus = FadeStatus.FadeOut;
				}
				if (ambienceVolume > 0.0001f)
				{
					m_Mixer.SetFloat("AmbienceVolume", Mathf.Log10(Mathf.Min(Mathf.Max(ambienceVolume - Time.deltaTime / 1f, 0.0001f), 1f)) * 20f);
					m_AudioFadeStatus = FadeStatus.FadeOut;
				}
				if (audioGroupsVolume > 0.0001f)
				{
					m_Mixer.SetFloat("AudioGroupsVolume", Mathf.Log10(Mathf.Min(Mathf.Max(audioGroupsVolume - Time.deltaTime / 1f, 0.0001f), 1f)) * 20f);
					m_AudioFadeStatus = FadeStatus.FadeOut;
				}
				if (radioVolume > 0.0001f)
				{
					m_Mixer.SetFloat("RadioVolume", Mathf.Log10(Mathf.Min(Mathf.Max(radioVolume - Time.deltaTime / 1f, 0.0001f), 1f)) * 20f);
					m_AudioFadeStatus = FadeStatus.FadeOut;
				}
				else
				{
					m_Radio.ForceRadioPause(pause: true);
				}
			}
		}
		else if (m_AudioFadeStatus == FadeStatus.FadeIn)
		{
			m_AudioFadeStatus = FadeStatus.None;
			if (m_SimulationSystem.selectedSpeed != 0f)
			{
				if (disastersVolume < SharedSettings.instance.audio.disastersVolume)
				{
					m_Mixer.SetFloat("DisastersVolume", Mathf.Log10(Mathf.Min(Mathf.Max(disastersVolume + Time.deltaTime / 1f, 0.0001f), 1f)) * 20f);
					m_AudioFadeStatus = FadeStatus.FadeIn;
				}
				if (worldVolume < SharedSettings.instance.audio.worldVolume)
				{
					m_Mixer.SetFloat("WorldVolume", Mathf.Log10(Mathf.Min(Mathf.Max(worldVolume + Time.deltaTime / 1f, 0.0001f), 1f)) * 20f);
					m_AudioFadeStatus = FadeStatus.FadeIn;
				}
			}
			if (!m_GameScreenUISystem.isMenuActive)
			{
				if (serviceBuildingsVolume < SharedSettings.instance.audio.serviceBuildingsVolume)
				{
					m_Mixer.SetFloat("ServiceBuildingsVolume", Mathf.Log10(Mathf.Min(Mathf.Max(serviceBuildingsVolume + Time.deltaTime / 1f, 0.0001f), 1f)) * 20f);
					m_AudioFadeStatus = FadeStatus.FadeIn;
				}
				if (ambienceVolume < SharedSettings.instance.audio.ambienceVolume)
				{
					m_Mixer.SetFloat("AmbienceVolume", Mathf.Log10(Mathf.Min(Mathf.Max(ambienceVolume + Time.deltaTime / 1f, 0.0001f), 1f)) * 20f);
					m_AudioFadeStatus = FadeStatus.FadeIn;
				}
				if (audioGroupsVolume < SharedSettings.instance.audio.audioGroupsVolume)
				{
					m_Mixer.SetFloat("AudioGroupsVolume", Mathf.Log10(Mathf.Min(Mathf.Max(audioGroupsVolume + Time.deltaTime / 1f, 0.0001f), 1f)) * 20f);
					m_AudioFadeStatus = FadeStatus.FadeIn;
				}
				if (radioVolume < SharedSettings.instance.audio.radioVolume)
				{
					m_Mixer.SetFloat("RadioVolume", Mathf.Log10(Mathf.Min(Mathf.Max(radioVolume + Time.deltaTime / 1f, 0.0001f), 1f)) * 20f);
					m_AudioFadeStatus = FadeStatus.FadeIn;
				}
			}
		}
		m_IsGamePausedLastUpdate = m_SimulationSystem.selectedSpeed == 0f;
		m_IsMenuActivatedLastUpdate = m_GameScreenUISystem.isMenuActive;
	}

	private void UpdateTempAudioSources()
	{
		foreach (AudioSource item in m_TempAudioSources)
		{
			if ((Object)(object)item != (Object)null && !item.isPlaying)
			{
				AudioSourcePool.Release(item);
			}
		}
		m_TempAudioSources.RemoveAll((AudioSource audiosouce) => (Object)(object)audiosouce == (Object)null || (Object)(object)((Component)audiosouce).gameObject == (Object)null || !((Component)audiosouce).gameObject.activeSelf);
	}

	private float GetFadedVolume(FadeStatus status, float2 sfxFades, float currentVolume, float targetVolume)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (sfxFades.x != 0f && status == FadeStatus.FadeIn && math.abs(currentVolume - targetVolume) > float.Epsilon)
		{
			if (currentVolume > targetVolume)
			{
				return math.saturate(currentVolume - m_DeltaTime / sfxFades.x * targetVolume);
			}
			return math.saturate(currentVolume + m_DeltaTime / sfxFades.x * targetVolume);
		}
		if (status == FadeStatus.FadeOut)
		{
			if (sfxFades.y != 0f && currentVolume - 0f > float.Epsilon)
			{
				return math.saturate(currentVolume - m_DeltaTime / sfxFades.y * targetVolume);
			}
			targetVolume = 0f;
		}
		return targetVolume;
	}

	private void Fadeout(SourceInfo sourceInfo, int index)
	{
		if (index < m_AudioInfos.Count && (Object)(object)m_AudioInfos[index].m_AudioSource != (Object)null)
		{
			AudioInfo value = m_AudioInfos[index];
			value.m_SourceInfo = sourceInfo;
			value.m_Status = FadeStatus.FadeOut;
			m_AudioInfos[index] = value;
		}
	}

	private void RemoveAudio(SourceInfo sourceInfo, int index)
	{
		if ((Object)(object)m_AudioInfos[index].m_AudioSource != (Object)null)
		{
			AudioSourcePool.Release(m_AudioInfos[index].m_AudioSource);
		}
		m_CurrentEffects.Remove(sourceInfo);
		if (index < m_AudioInfos.Count - 1)
		{
			m_AudioInfos[index] = m_AudioInfos[m_AudioInfos.Count - 1];
			m_CurrentEffects[m_AudioInfos[index].m_SourceInfo] = index;
		}
		m_AudioInfos.RemoveAt(m_AudioInfos.Count - 1);
	}

	private void SyncAudioSources()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_AudioInfos.Count; i++)
		{
			AudioInfo audioInfo = m_AudioInfos[i];
			if ((Object)(object)audioInfo.m_AudioSource == (Object)null)
			{
				RemoveAudio(audioInfo.m_SourceInfo, i);
			}
			else if (!audioInfo.m_AudioSource.isPlaying && m_CurrentEffects.ContainsKey(audioInfo.m_SourceInfo))
			{
				RemoveAudio(audioInfo.m_SourceInfo, i);
			}
		}
		ComponentLookup<Game.Tools.EditorContainer> componentLookup = InternalCompilerInterface.GetComponentLookup<Game.Tools.EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Moving> componentLookup2 = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Temp> componentLookup3 = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<AudioEffectData> componentLookup4 = InternalCompilerInterface.GetComponentLookup<AudioEffectData>(ref __TypeHandle.__Game_Prefabs_AudioEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<VehicleAudioEffectData> componentLookup5 = InternalCompilerInterface.GetComponentLookup<VehicleAudioEffectData>(ref __TypeHandle.__Game_Prefabs_VehicleAudioEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<Effect> bufferLookup = InternalCompilerInterface.GetBufferLookup<Effect>(ref __TypeHandle.__Game_Prefabs_Effect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<EnabledEffect> bufferLookup2 = InternalCompilerInterface.GetBufferLookup<EnabledEffect>(ref __TypeHandle.__Game_Effects_EnabledEffect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<EffectInstance> componentLookup6 = InternalCompilerInterface.GetComponentLookup<EffectInstance>(ref __TypeHandle.__Game_Effects_EffectInstance_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		JobHandle dependencies;
		NativeList<EnabledEffectData> enabledData = m_EffectControlSystem.GetEnabledData(readOnly: true, out dependencies);
		((JobHandle)(ref dependencies)).Complete();
		PrefabRef prefabRef = default(PrefabRef);
		DynamicBuffer<Effect> val2 = default(DynamicBuffer<Effect>);
		for (int j = 0; j < m_AudioInfos.Count; j++)
		{
			AudioInfo value = m_AudioInfos[j];
			bool flag;
			Entity val;
			float num;
			Transform transform;
			if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, value.m_SourceInfo.m_Entity, ref prefabRef) && (Object)(object)value.m_AudioSource != (Object)null)
			{
				num = 0f;
				flag = false;
				transform = default(Transform);
				val = default(Entity);
				if (value.m_SourceInfo.m_EffectIndex >= 0)
				{
					DynamicBuffer<EnabledEffect> effects = bufferLookup2[value.m_SourceInfo.m_Entity];
					if (bufferLookup.TryGetBuffer(prefabRef.m_Prefab, ref val2))
					{
						if (GetEffect(effects, value.m_SourceInfo.m_EffectIndex, out var effect))
						{
							_ = val2[value.m_SourceInfo.m_EffectIndex];
							EnabledEffectData enabledEffectData = enabledData[effect.m_EnabledIndex];
							num = enabledEffectData.m_Intensity;
							flag = (enabledEffectData.m_Flags & EnabledEffectFlags.AudioDisabled) != 0;
							transform = new Transform(enabledEffectData.m_Position, enabledEffectData.m_Rotation);
							val = value.m_SourceInfo.m_Entity;
							goto IL_0326;
						}
					}
					else if (componentLookup.HasComponent(value.m_SourceInfo.m_Entity) && effects.Length != 0)
					{
						EnabledEffectData enabledEffectData2 = enabledData[effects[0].m_EnabledIndex];
						num = enabledEffectData2.m_Intensity;
						flag = (enabledEffectData2.m_Flags & EnabledEffectFlags.AudioDisabled) != 0;
						transform = new Transform(enabledEffectData2.m_Position, enabledEffectData2.m_Rotation);
						val = value.m_SourceInfo.m_Entity;
						goto IL_0326;
					}
				}
				else if (value.m_SourceInfo.m_EffectIndex == -1)
				{
					EffectInstance effectInstance = componentLookup6[value.m_SourceInfo.m_Entity];
					num = effectInstance.m_Intensity;
					transform = new Transform(effectInstance.m_Position, effectInstance.m_Rotation);
					goto IL_0326;
				}
			}
			value.m_Status = FadeStatus.FadeOut;
			m_AudioInfos[j] = value;
			if (!(value.m_AudioSource.volume < 0.001f) && !(value.m_MaxVolume < 0.001f))
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).Exists(value.m_SFXEntity))
				{
					int audioClipId = componentLookup4[value.m_SFXEntity].m_AudioClipId;
					Transform transform2 = ((Component)value.m_AudioSource).transform;
					transform2.position += float3.op_Implicit(value.m_Velocity * m_DeltaTime);
					value.m_AudioSource.volume = GetFadedVolume(value.m_Status, m_Clips[audioClipId].m_FadeTimes, value.m_AudioSource.volume, value.m_MaxVolume);
					continue;
				}
			}
			RemoveAudio(value.m_SourceInfo, j--);
			continue;
			IL_0326:
			int audioClipId2 = componentLookup4[value.m_SFXEntity].m_AudioClipId;
			if (flag)
			{
				value.m_Status = FadeStatus.FadeOut;
			}
			float3 val3 = float3.op_Implicit(((Component)value.m_AudioSource).transform.position);
			if (!UpdateAudioSource(value.m_AudioSource, m_Clips[audioClipId2], transform, num, val == followed && val != Entity.Null, j, value.m_Status, value.m_SourceInfo))
			{
				j--;
				continue;
			}
			value.m_MaxVolume = m_Clips[audioClipId2].m_Volume * num;
			value.m_Velocity = (float3.op_Implicit(((Component)value.m_AudioSource).transform.position) - val3) / math.max(1E-06f, m_DeltaTime);
			m_AudioInfos[j] = value;
			if (componentLookup5.HasComponent(value.m_SFXEntity))
			{
				if (componentLookup3.HasComponent(val))
				{
					val = componentLookup3[val].m_Original;
				}
				float velocity = 0f;
				if (componentLookup2.HasComponent(val))
				{
					velocity = math.length(componentLookup2[val].m_Velocity);
				}
				UpdateAudioSourceByVelocity(value.m_AudioSource, velocity, componentLookup5[value.m_SFXEntity], value.m_Status);
				float num2 = m_Clips[audioClipId2].m_Doppler * math.saturate(1f - (m_SimulationSystem.smoothSpeed - 1f) * 0.3f);
				if (value.m_AudioSource.dopplerLevel != num2)
				{
					value.m_AudioSource.dopplerLevel = num2;
				}
			}
		}
	}

	public static float3 GetClosestSourcePosition(float3 targetPosition, Transform sourceTransform, float3 sourceOffset, float3 sourceSize)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		float3 val = ObjectUtils.WorldToLocal(ObjectUtils.InverseTransform(sourceTransform), targetPosition);
		sourceOffset.x = math.clamp(val.x, sourceOffset.x - sourceSize.x, sourceOffset.x + sourceSize.x);
		sourceOffset.y = math.clamp(val.y, sourceOffset.y - sourceSize.y, sourceOffset.y + sourceSize.y);
		sourceOffset.z = math.clamp(val.z, sourceOffset.z - sourceSize.z, sourceOffset.z + sourceSize.z);
		return ObjectUtils.LocalToWorld(sourceTransform, sourceOffset);
	}

	private void UpdateAudioSourceByVelocity(AudioSource audioSource, float velocity, VehicleAudioEffectData vehicleData, FadeStatus status)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		velocity = math.saturate((velocity - vehicleData.m_SpeedLimits.x) / (vehicleData.m_SpeedLimits.y - vehicleData.m_SpeedLimits.x));
		audioSource.pitch = math.lerp(vehicleData.m_SpeedPitches.x, vehicleData.m_SpeedPitches.y, velocity);
		float num = math.lerp(vehicleData.m_SpeedVolumes.x, vehicleData.m_SpeedVolumes.y, velocity);
		if (status == FadeStatus.FadeOut)
		{
			audioSource.volume = math.min(audioSource.volume, num);
		}
		else
		{
			audioSource.volume = num;
		}
	}

	private bool UpdateAudioSource(AudioSource audioSource, SFX sfx, Transform transform, float intensity, bool disableDoppler, int i = -1, FadeStatus status = FadeStatus.None, SourceInfo sourceInfo = default(SourceInfo))
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)audioSource == (Object)null)
		{
			return false;
		}
		float3 val = transform.m_Position;
		if (sfx.m_SourceSize.x > 0f || sfx.m_SourceSize.y > 0f || sfx.m_SourceSize.z > 0f)
		{
			val = GetClosestSourcePosition(float3.op_Implicit(((Component)m_AudioListener).transform.position), transform, float3.zero, sfx.m_SourceSize);
			disableDoppler = true;
		}
		((Component)audioSource).transform.position = float3.op_Implicit(val);
		audioSource.dopplerLevel = (disableDoppler ? 0f : sfx.m_Doppler);
		float num = sfx.m_Volume * intensity;
		if (i >= 0)
		{
			if (status == FadeStatus.FadeOut && (audioSource.volume < 0.001f || num < 0.001f))
			{
				RemoveAudio(sourceInfo, i);
				return false;
			}
			num = GetFadedVolume(status, sfx.m_FadeTimes, audioSource.volume, num);
		}
		audioSource.volume = num;
		return true;
	}

	private AudioMixerGroup GetAudioMixerGroup(MixerGroup group)
	{
		return (AudioMixerGroup)(group switch
		{
			MixerGroup.Ambient => m_AmbientGroup, 
			MixerGroup.Menu => m_MenuGroup, 
			MixerGroup.Radio => m_RadioGroup, 
			MixerGroup.UI => m_UIGroup, 
			MixerGroup.World => m_WorldGroup, 
			MixerGroup.ServiceBuildings => m_ServiceBuildingGroup, 
			MixerGroup.AudioGroups => m_AudioGroupGroup, 
			MixerGroup.Disasters => m_DisasterGroup, 
			_ => null, 
		});
	}

	private void SetAudioSourceData(AudioSource audioSource, SFX sfx, float volume)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Invalid comparison between Unknown and I4
		audioSource.pitch = sfx.m_Pitch;
		audioSource.volume = volume;
		audioSource.clip = sfx.m_AudioClip;
		audioSource.loop = sfx.m_Loop;
		audioSource.minDistance = sfx.m_MinMaxDistance.x;
		audioSource.maxDistance = sfx.m_MinMaxDistance.y;
		audioSource.spatialBlend = sfx.m_SpatialBlend;
		audioSource.spread = sfx.m_Spread;
		audioSource.rolloffMode = sfx.m_RolloffMode;
		if ((int)sfx.m_RolloffMode == 2)
		{
			audioSource.SetCustomCurve((AudioSourceCurveType)0, sfx.m_RolloffCurve);
		}
		audioSource.outputAudioMixerGroup = GetAudioMixerGroup(sfx.m_MixerGroup);
		audioSource.dopplerLevel = 0f;
		audioSource.timeSamples = 0;
		if (sfx.m_RandomStartTime)
		{
			Random val = default(Random);
			((Random)(ref val))._002Ector((uint)DateTime.Now.Ticks);
			audioSource.timeSamples = ((Random)(ref val)).NextInt(sfx.m_AudioClip.samples);
		}
		audioSource.ignoreListenerPause = false;
		audioSource.priority = sfx.m_Priority;
	}

	public bool GetRandomizeAudio(Entity sfxEntity, out SFX sfx)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		sfx = null;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<AudioRandomizeData> val = default(DynamicBuffer<AudioRandomizeData>);
		if (((EntityManager)(ref entityManager)).HasComponent<AudioRandomizeData>(sfxEntity) && EntitiesExtensions.TryGetBuffer<AudioRandomizeData>(((ComponentSystemBase)this).EntityManager, sfxEntity, true, ref val))
		{
			Random val2 = default(Random);
			((Random)(ref val2))._002Ector((uint)DateTime.Now.Ticks);
			int num = ((Random)(ref val2)).NextInt(val.Length);
			Entity sFXEntity = val[num].m_SFXEntity;
			List<SFX> list = m_Clips;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			sfx = list[((EntityManager)(ref entityManager)).GetComponentData<AudioEffectData>(sFXEntity).m_AudioClipId];
			return true;
		}
		return false;
	}

	public void PlayLightningSFX(float3 targetPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Invalid comparison between Unknown and I4
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		WeatherAudioData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WeatherAudioData>(((EntityQuery)(ref m_WeatherAudioEntitiyQuery)).GetSingletonEntity());
		float delay = math.distance(float3.op_Implicit(((Component)m_AudioListener).transform.position), targetPos) / componentData.m_LightningSoundSpeed;
		if (!GetRandomizeAudio(componentData.m_LightningAudio, out var sfx))
		{
			List<SFX> list = m_Clips;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			sfx = list[((EntityManager)(ref entityManager)).GetComponentData<AudioEffectData>(componentData.m_LightningAudio).m_AudioClipId];
		}
		if ((Object)(object)sfx.m_AudioClip != (Object)null)
		{
			AudioSource val = AudioSourcePool.Get();
			val.clip = sfx.m_AudioClip;
			((Component)val).transform.position = float3.op_Implicit(targetPos);
			val.outputAudioMixerGroup = GetAudioMixerGroup(sfx.m_MixerGroup);
			val.pitch = sfx.m_Pitch;
			val.volume = sfx.m_Volume;
			val.spatialBlend = sfx.m_SpatialBlend;
			val.dopplerLevel = sfx.m_Doppler;
			val.spread = sfx.m_Spread;
			val.loop = sfx.m_Loop;
			val.minDistance = sfx.m_MinMaxDistance.x;
			val.maxDistance = sfx.m_MinMaxDistance.y;
			val.rolloffMode = sfx.m_RolloffMode;
			if ((int)sfx.m_RolloffMode == 2)
			{
				val.SetCustomCurve((AudioSourceCurveType)0, sfx.m_RolloffCurve);
			}
			val.timeSamples = 0;
			val.ignoreListenerPause = false;
			val.priority = sfx.m_Priority;
			AudioSourcePool.PlayDelayed(val, delay);
			m_TempAudioSources.Add(val);
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		if (radio != null && radio.currentChannel != null)
		{
			m_LastSaveRadioChannel = radio.currentChannel.name;
			m_LastSaveRadioSkipAds = m_Radio.skipAds;
		}
		string text = m_LastSaveRadioChannel;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(text);
		bool num = m_LastSaveRadioSkipAds;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.saveRadioStations)
		{
			ref string reference = ref m_LastSaveRadioChannel;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
			ref bool reference2 = ref m_LastSaveRadioSkipAds;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
		}
	}

	public void SetDefaults(Context context)
	{
		m_LastSaveRadioChannel = string.Empty;
		m_LastSaveRadioSkipAds = false;
	}

	public void PreDeserialize(Context context)
	{
		m_LastSaveRadioChannel = string.Empty;
		m_LastSaveRadioSkipAds = false;
	}

	public void PreSerialize(Context context)
	{
		m_LastSaveRadioChannel = string.Empty;
		m_LastSaveRadioSkipAds = false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public AudioManager()
	{
	}
}

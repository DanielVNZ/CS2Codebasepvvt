using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Audio;
using Game.Common;
using Game.Events;
using Game.Prefabs;
using Game.Prefabs.Climate;
using Game.Rendering.Climate;
using Game.Rendering.Utilities;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;
using UnityEngine.VFX;

namespace Game.Rendering;

[CompilerGenerated]
public class ClimateRenderSystem : GameSystemBase
{
	private static class VFXIDs
	{
		public static readonly int CameraPosition = Shader.PropertyToID("CameraPosition");

		public static readonly int CameraDirection = Shader.PropertyToID("CameraDirection");

		public static readonly int VolumeScale = Shader.PropertyToID("VolumeScale");

		public static readonly int WindTexture = Shader.PropertyToID("WindTexture");

		public static readonly int CloudsAltitude = Shader.PropertyToID("CloudsAltitude");

		public static readonly int MapOffsetScale = Shader.PropertyToID("MapOffsetScale");

		public static readonly int RainStrength = Shader.PropertyToID("RainStrength");

		public static readonly int SnowStrength = Shader.PropertyToID("SnowStrength");

		public static readonly int LightningOrigin = Shader.PropertyToID("LightningOrigin");

		public static readonly int LightningTarget = Shader.PropertyToID("LightningTarget");
	}

	private enum PrecipitationType
	{
		Rain,
		Snow,
		Hail
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct TypeHandle
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
		}
	}

	private RenderingSystem m_RenderingSystem;

	private ClimateSystem m_ClimateSystem;

	private SimulationSystem m_SimulationSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private PrefabSystem m_PrefabSystem;

	private WindTextureSystem m_WindTextureSystem;

	private TerrainSystem m_TerrainSystem;

	private TimeSystem m_TimeSystem;

	private AudioManager m_AudioManager;

	public bool globalEffectTimeStepFromSimulation;

	public bool weatherEffectTimeStepFromSimulation = true;

	private static VisualEffectAsset s_PrecipitationVFXAsset;

	private VisualEffect m_PrecipitationVFX;

	private static VisualEffectAsset s_LightningVFXAsset;

	private VisualEffect m_LightningVFX;

	private Volume m_ClimateControlVolume;

	private VolumetricClouds m_VolumetricClouds;

	private WindVolumeComponent m_Wind;

	private WindControl m_WindControl;

	private bool m_IsRaining;

	private bool m_IsSnowing;

	private bool m_HailStorm;

	private EntityQuery m_EventQuery;

	private NativeQueue<LightningStrike> m_LightningStrikeQueue;

	private JobHandle m_LightningStrikeDeps;

	private WeatherPropertiesStack m_PropertiesStack;

	private readonly List<WeatherPrefab> m_FromWeatherPrefabs = new List<WeatherPrefab>();

	private readonly List<WeatherPrefab> m_ToWeatherPrefabs = new List<WeatherPrefab>();

	private bool m_PropertiesChanged;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_100321765_0;

	private EntityQuery __query_100321765_1;

	public float precipitationVolumeScale { get; set; } = 30f;

	public bool editMode { get; set; }

	public bool pauseSimulationOnLightning { get; set; }

	internal WeatherPropertiesStack propertiesStack => m_PropertiesStack;

	public IReadOnlyList<WeatherPrefab> fromWeatherPrefabs => m_FromWeatherPrefabs;

	public IReadOnlyList<WeatherPrefab> toWeatherPrefabs => m_ToWeatherPrefabs;

	public bool IsAsync { get; set; }

	private void SetData(WeatherPropertiesStack stack, IReadOnlyList<WeatherPrefab> fromPrefab, IReadOnlyList<WeatherPrefab> toPrefab)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < fromPrefab.Count; i++)
		{
			foreach (OverrideablePropertiesComponent overrideableProperty in fromPrefab[i].overrideableProperties)
			{
				if (overrideableProperty.active && !overrideableProperty.hasTimeBasedInterpolation)
				{
					stack.SetFrom(((object)overrideableProperty).GetType(), overrideableProperty);
				}
			}
		}
		for (int j = 0; j < toPrefab.Count; j++)
		{
			WeatherPrefab weatherPrefab = toPrefab[j];
			foreach (OverrideablePropertiesComponent overrideableProperty2 in weatherPrefab.overrideableProperties)
			{
				if (overrideableProperty2.active)
				{
					if (overrideableProperty2.hasTimeBasedInterpolation)
					{
						stack.SetTarget(((object)overrideableProperty2).GetType(), overrideableProperty2);
					}
					else
					{
						stack.SetTo(((object)overrideableProperty2).GetType(), overrideableProperty2, j == 1, new Bounds1(weatherPrefab.m_CloudinessRange));
					}
				}
			}
		}
	}

	public void Clear()
	{
		m_PropertiesChanged = true;
		m_FromWeatherPrefabs.Clear();
		m_ToWeatherPrefabs.Clear();
	}

	public void ScheduleFrom(WeatherPrefab prefab)
	{
		m_FromWeatherPrefabs.Add(prefab);
	}

	public void ScheduleTo(WeatherPrefab prefab)
	{
		m_ToWeatherPrefabs.Add(prefab);
	}

	private float GetTimeOfYear()
	{
		if (m_ClimateSystem.currentDate.overrideState)
		{
			return m_ClimateSystem.currentDate.overrideValue;
		}
		TimeSettingsData settings = default(TimeSettingsData);
		TimeData data = default(TimeData);
		if (((EntityQuery)(ref __query_100321765_0)).TryGetSingleton<TimeSettingsData>(ref settings) && ((EntityQuery)(ref __query_100321765_1)).TryGetSingleton<TimeData>(ref data))
		{
			double renderingFrame = (float)(m_RenderingSystem.frameIndex - data.m_FirstFrame) + m_RenderingSystem.frameTime;
			return m_TimeSystem.GetTimeOfYear(settings, data, renderingFrame);
		}
		return 0.5f;
	}

	private void UpdateWeather()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		float timeOfYear = GetTimeOfYear();
		ClimateSystem.ClimateSample sample = m_ClimateSystem.SampleClimate(timeOfYear);
		if (m_PropertiesChanged)
		{
			SetData(m_PropertiesStack, m_FromWeatherPrefabs, m_ToWeatherPrefabs);
			m_PropertiesChanged = false;
		}
		float renderingDeltaTime = m_RenderingSystem.frameDelta / 60f;
		WorldUnmanaged worldUnmanaged = ((SystemState)(ref ((SystemBase)this).CheckedStateRef)).WorldUnmanaged;
		float deltaTime = ((WorldUnmanaged)(ref worldUnmanaged)).Time.DeltaTime;
		m_PropertiesStack.InterpolateOverrideData(deltaTime, renderingDeltaTime, sample, editMode);
	}

	private void UpdateEffectsState()
	{
		if (m_ClimateSystem.isPrecipitating && m_ClimateSystem.hail < 0.001f)
		{
			if ((float)m_ClimateSystem.temperature > 0f)
			{
				if (m_IsSnowing)
				{
					UpdateEffectState(PrecipitationType.Snow, start: false);
					m_IsSnowing = false;
				}
				if (!m_IsRaining)
				{
					UpdateEffectState(PrecipitationType.Rain, start: true);
					m_IsRaining = true;
				}
			}
			else
			{
				if (!m_IsSnowing)
				{
					UpdateEffectState(PrecipitationType.Snow, start: true);
					m_IsSnowing = true;
				}
				if (m_IsRaining)
				{
					UpdateEffectState(PrecipitationType.Rain, start: false);
					m_IsRaining = false;
				}
			}
		}
		else
		{
			if (m_IsRaining)
			{
				UpdateEffectState(PrecipitationType.Rain, start: false);
				m_IsRaining = false;
			}
			if (m_IsSnowing)
			{
				UpdateEffectState(PrecipitationType.Snow, start: false);
				m_IsSnowing = false;
			}
		}
		if (m_HailStorm && m_ClimateSystem.hail <= 0.001f)
		{
			UpdateEffectState(PrecipitationType.Hail, start: false);
			m_HailStorm = false;
		}
		else if (!m_HailStorm && m_ClimateSystem.hail > 0.001f)
		{
			UpdateEffectState(PrecipitationType.Hail, start: true);
			m_HailStorm = true;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_WindTextureSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindTextureSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_ClimateControlVolume = VolumeHelper.CreateVolume("ClimateControlVolume", 50);
		m_PropertiesStack = new WeatherPropertiesStack(m_ClimateControlVolume);
		VolumeHelper.GetOrCreateVolumeComponent<VolumetricClouds>(m_ClimateControlVolume, ref m_VolumetricClouds);
		VolumeHelper.GetOrCreateVolumeComponent(m_ClimateControlVolume, ref m_Wind);
		m_WindControl = WindControl.instance;
		ResetOverrides();
		s_PrecipitationVFXAsset = Resources.Load<VisualEffectAsset>("Precipitation/PrecipitationVFX");
		s_LightningVFXAsset = Resources.Load<VisualEffectAsset>("Lightning/LightningBolt");
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Events.WeatherPhenomenon>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_LightningStrikeQueue = new NativeQueue<LightningStrike>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	private void ResetOverrides()
	{
		((VolumeComponent)m_VolumetricClouds).SetAllOverridesTo(false);
	}

	public NativeQueue<LightningStrike> GetLightningStrikeQueue(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_LightningStrikeDeps;
		return m_LightningStrikeQueue;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		if (m_CameraUpdateSystem.activeViewer != null)
		{
			UpdateWeather();
			UpdateVolumetricClouds();
			CreateDynamicVFXIfNeeded();
			UpdateEffectsState();
			UpdateEffectsProperties();
			UpdateVFXSpeed();
		}
		NativeArray<Entity> val = ((EntityQuery)(ref m_EventQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				Entity val2 = val[i];
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).GetComponentData<Game.Events.WeatherPhenomenon>(val2).m_Intensity != 0f)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).GetComponentData<InterpolatedTransform>(val2);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val2);
					m_PrefabSystem.GetPrefab<EventPrefab>(componentData).GetComponent<Game.Prefabs.WeatherPhenomenon>();
				}
			}
		}
		finally
		{
			val.Dispose();
		}
		((JobHandle)(ref m_LightningStrikeDeps)).Complete();
		LightningStrike lightningStrike = default(LightningStrike);
		while (m_LightningStrikeQueue.TryDequeue(ref lightningStrike))
		{
			LightningStrike(lightningStrike.m_Position, lightningStrike.m_Position);
		}
	}

	public void AddLightningStrikeWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_LightningStrikeDeps = jobHandle;
	}

	private void UpdateVolumetricClouds()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f + (1f - math.abs(math.dot(m_CameraUpdateSystem.direction, new float3(0f, 1f, 0f))));
		((VolumeParameter<CloudFadeInMode>)(object)m_VolumetricClouds.fadeInMode).Override((CloudFadeInMode)1);
		((VolumeParameter<float>)(object)m_VolumetricClouds.fadeInStart).Override(math.max((m_CameraUpdateSystem.position.y - ((VolumeParameter<float>)(object)m_VolumetricClouds.bottomAltitude).value) * num, m_CameraUpdateSystem.nearClipPlane));
		((VolumeParameter<float>)(object)m_VolumetricClouds.fadeInDistance).Override(((VolumeParameter<float>)(object)m_VolumetricClouds.altitudeRange).value * 0.3f);
		((VolumeParameter<CloudHook>)(object)m_VolumetricClouds.renderHook).Override((CloudHook)(!(m_CameraUpdateSystem.position.y < ((VolumeParameter<float>)(object)m_VolumetricClouds.bottomAltitude).value)));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_LightningStrikeDeps)).Complete();
		m_LightningStrikeQueue.Dispose();
		m_WindControl.Dispose();
		m_PropertiesStack.Dispose();
		VolumeHelper.DestroyVolume(m_ClimateControlVolume);
		base.OnDestroy();
	}

	private void CreateDynamicVFXIfNeeded()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)s_PrecipitationVFXAsset != (Object)null && (Object)(object)m_PrecipitationVFX == (Object)null)
		{
			COSystemBase.baseLog.DebugFormat("Creating VFXs pool", Array.Empty<object>());
			m_PrecipitationVFX = new GameObject("PrecipitationVFX").AddComponent<VisualEffect>();
			m_PrecipitationVFX.visualEffectAsset = s_PrecipitationVFXAsset;
			m_LightningVFX = new GameObject("LightningVFX").AddComponent<VisualEffect>();
			m_LightningVFX.visualEffectAsset = s_LightningVFXAsset;
		}
	}

	public void LightningStrike(float3 start, float3 target, bool useCloudsAltitude = true)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (pauseSimulationOnLightning)
		{
			m_SimulationSystem.selectedSpeed = 0f;
		}
		if (useCloudsAltitude)
		{
			start.y = ((VolumeParameter<float>)(object)m_VolumetricClouds.bottomAltitude).value + ((VolumeParameter<float>)(object)m_VolumetricClouds.altitudeRange).value * 0.1f;
		}
		COSystemBase.baseLog.DebugFormat("Lightning strike {0}->{1}", (object)start, (object)target);
		m_LightningVFX.SetVector3(VFXIDs.LightningOrigin, float3.op_Implicit(start));
		m_LightningVFX.SetVector3(VFXIDs.LightningTarget, float3.op_Implicit(target));
		m_LightningVFX.SendEvent("OnPlay");
		m_AudioManager.PlayLightningSFX((start - target) / 2f);
	}

	private bool GetEventName(PrecipitationType type, bool start, out string name)
	{
		switch (type)
		{
		case PrecipitationType.Rain:
			name = (start ? "OnRainStart" : "OnRainStop");
			return true;
		case PrecipitationType.Snow:
			name = (start ? "OnSnowStart" : "OnSnowStop");
			return true;
		case PrecipitationType.Hail:
			name = (start ? "OnHailStart" : "OnHailStop");
			return true;
		default:
			name = null;
			return false;
		}
	}

	private void UpdateEffectState(PrecipitationType type, bool start)
	{
		if (GetEventName(type, start, out var name))
		{
			COSystemBase.baseLog.DebugFormat("PrecipitationVFX event {0}", (object)name);
			m_PrecipitationVFX.SendEvent(name);
		}
	}

	private void UpdateEffectsProperties()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		m_PrecipitationVFX.SetCheckedVector3(VFXIDs.CameraPosition, float3.op_Implicit(m_CameraUpdateSystem.position));
		m_PrecipitationVFX.SetCheckedVector3(VFXIDs.CameraDirection, float3.op_Implicit(m_CameraUpdateSystem.direction));
		m_PrecipitationVFX.SetCheckedVector3(VFXIDs.VolumeScale, new Vector3(precipitationVolumeScale, precipitationVolumeScale, precipitationVolumeScale));
		m_PrecipitationVFX.SetCheckedTexture(VFXIDs.WindTexture, (Texture)(object)m_WindTextureSystem.WindTexture);
		m_PrecipitationVFX.SetCheckedFloat(VFXIDs.CloudsAltitude, ((VolumeParameter<float>)(object)m_VolumetricClouds.bottomAltitude).value);
		m_PrecipitationVFX.SetCheckedVector4(VFXIDs.MapOffsetScale, m_TerrainSystem.mapOffsetScale);
		m_PrecipitationVFX.SetCheckedFloat(VFXIDs.RainStrength, m_ClimateSystem.precipitation);
		m_PrecipitationVFX.SetCheckedFloat(VFXIDs.SnowStrength, m_ClimateSystem.precipitation);
	}

	private void UpdateVFXSpeed()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (globalEffectTimeStepFromSimulation)
		{
			float num = m_RenderingSystem.frameDelta / 60f;
			float smoothSpeed = m_SimulationSystem.smoothSpeed;
			VFXManager.fixedTimeStep = num * smoothSpeed;
			Debug.Log((object)("smoothedRenderTimeStep: " + num + " simulationSpeedMultiplier: " + smoothSpeed));
		}
		else
		{
			float frameDelta = m_RenderingSystem.frameDelta;
			WorldUnmanaged worldUnmanaged = ((SystemState)(ref ((SystemBase)this).CheckedStateRef)).WorldUnmanaged;
			float num2 = frameDelta / math.max(1E-06f, ((WorldUnmanaged)(ref worldUnmanaged)).Time.DeltaTime * 60f);
			m_PrecipitationVFX.playRate = (weatherEffectTimeStepFromSimulation ? num2 : 1f);
			m_LightningVFX.playRate = (weatherEffectTimeStepFromSimulation ? num2 : 1f);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<TimeSettingsData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_100321765_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<TimeData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_100321765_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public ClimateRenderSystem()
	{
	}
}

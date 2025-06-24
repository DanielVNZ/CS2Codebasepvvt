using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Common;
using Game.Debug;
using Game.Effects;
using Game.Events;
using Game.Objects;
using Game.Prefabs;
using Game.Reflection;
using Game.SceneFlow;
using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Audio;

[CompilerGenerated]
public class AudioGroupingSystem : GameSystemBase
{
	[BurstCompile]
	private struct AudioGroupingJob : IJob
	{
		public ComponentLookup<EffectInstance> m_EffectInstances;

		[ReadOnly]
		public ComponentLookup<EffectData> m_EffectDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public NativeArray<TrafficAmbienceCell> m_TrafficAmbienceMap;

		[ReadOnly]
		public NativeArray<ZoneAmbienceCell> m_AmbienceMap;

		[ReadOnly]
		public NativeArray<AudioGroupingSettingsData> m_Settings;

		public SourceUpdateData m_SourceUpdateData;

		public EffectFlagSystem.EffectFlagData m_EffectFlagData;

		public float3 m_CameraPosition;

		public NativeArray<Entity> m_AmbienceEntities;

		public NativeArray<Entity> m_NearAmbienceEntities;

		public NativeArray<float> m_CurrentValues;

		[DeallocateOnJobCompletion]
		public NativeArray<Entity> m_OnFireTrees;

		[ReadOnly]
		public TerrainHeightData m_TerrainData;

		[ReadOnly]
		public float m_ForestFireDistance;

		[ReadOnly]
		public float m_Precipitation;

		[ReadOnly]
		public bool m_IsRaining;

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			float3 val = m_CameraPosition;
			float num = TerrainUtils.SampleHeight(ref m_TerrainData, m_CameraPosition);
			m_CameraPosition.y -= num;
			for (int i = 0; i < m_AmbienceEntities.Length; i++)
			{
				Entity val2 = m_AmbienceEntities[i];
				Entity val3 = m_NearAmbienceEntities[i];
				AudioGroupingSettingsData audioGroupingSettingsData = m_Settings[i];
				if (!m_EffectInstances.HasComponent(val2))
				{
					continue;
				}
				float num2 = 0f;
				float num3 = 0f;
				switch (audioGroupingSettingsData.m_Type)
				{
				case GroupAmbienceType.Traffic:
					num2 = TrafficAmbienceSystem.GetTrafficAmbience2(m_CameraPosition, m_TrafficAmbienceMap, 1f / audioGroupingSettingsData.m_Scale).m_Traffic;
					break;
				case GroupAmbienceType.Forest:
				case GroupAmbienceType.NightForest:
				{
					GroupAmbienceType groupAmbienceType = (m_EffectFlagData.m_IsNightTime ? GroupAmbienceType.NightForest : GroupAmbienceType.Forest);
					if (audioGroupingSettingsData.m_Type == groupAmbienceType && !IsNearForestOnFire(val))
					{
						num2 = ZoneAmbienceSystem.GetZoneAmbience(GroupAmbienceType.Forest, m_CameraPosition, m_AmbienceMap, 1f / m_Settings[i].m_Scale);
						if (val3 != Entity.Null)
						{
							num3 = ZoneAmbienceSystem.GetZoneAmbienceNear(GroupAmbienceType.Forest, m_CameraPosition, m_AmbienceMap, m_Settings[i].m_NearWeight, 1f / m_Settings[i].m_Scale);
						}
					}
					break;
				}
				case GroupAmbienceType.Rain:
					if (m_IsRaining)
					{
						num2 = math.min(1f / audioGroupingSettingsData.m_Scale, math.max(0f, m_Precipitation) * 2f);
						num3 = num2;
					}
					break;
				default:
					num2 = ZoneAmbienceSystem.GetZoneAmbience(audioGroupingSettingsData.m_Type, m_CameraPosition, m_AmbienceMap, 1f / audioGroupingSettingsData.m_Scale);
					if (val3 != Entity.Null)
					{
						num3 = ZoneAmbienceSystem.GetZoneAmbienceNear(audioGroupingSettingsData.m_Type, m_CameraPosition, m_AmbienceMap, m_Settings[i].m_NearWeight, 1f / audioGroupingSettingsData.m_Scale);
					}
					break;
				}
				m_CurrentValues[(int)audioGroupingSettingsData.m_Type] = num2;
				bool flag = true;
				Entity prefab = m_PrefabRefs[val2].m_Prefab;
				bool flag2 = (m_EffectDatas[prefab].m_Flags.m_RequiredFlags & EffectConditionFlags.Cold) != 0;
				bool flag3 = (m_EffectDatas[prefab].m_Flags.m_ForbiddenFlags & EffectConditionFlags.Cold) != 0;
				if (flag2 || flag3)
				{
					bool isColdSeason = m_EffectFlagData.m_IsColdSeason;
					flag = (flag2 && isColdSeason) || (flag3 && !isColdSeason);
				}
				if (num2 > 0.001f && flag)
				{
					EffectInstance effectInstance = m_EffectInstances[val2];
					float num4 = math.saturate(audioGroupingSettingsData.m_Scale * num2);
					num4 *= math.saturate((audioGroupingSettingsData.m_Height.y - m_CameraPosition.y) / (audioGroupingSettingsData.m_Height.y - audioGroupingSettingsData.m_Height.x));
					num4 = math.lerp(effectInstance.m_Intensity, num4, audioGroupingSettingsData.m_FadeSpeed);
					effectInstance.m_Position = val;
					effectInstance.m_Rotation = quaternion.identity;
					effectInstance.m_Intensity = math.saturate(num4);
					m_EffectInstances[val2] = effectInstance;
					m_SourceUpdateData.Add(val2, new Transform
					{
						m_Position = val,
						m_Rotation = quaternion.identity
					});
				}
				else
				{
					if (m_EffectInstances.HasComponent(val2))
					{
						EffectInstance effectInstance2 = m_EffectInstances[val2];
						effectInstance2.m_Intensity = 0f;
						m_EffectInstances[val2] = effectInstance2;
					}
					m_SourceUpdateData.Remove(val2);
				}
				flag = true;
				if (val3 != Entity.Null)
				{
					prefab = m_PrefabRefs[val3].m_Prefab;
					flag2 = (m_EffectDatas[prefab].m_Flags.m_RequiredFlags & EffectConditionFlags.Cold) != 0;
					flag3 = (m_EffectDatas[prefab].m_Flags.m_ForbiddenFlags & EffectConditionFlags.Cold) != 0;
					if (flag2 || flag3)
					{
						bool isColdSeason2 = m_EffectFlagData.m_IsColdSeason;
						flag = (flag2 && isColdSeason2) || (flag3 && !isColdSeason2);
					}
				}
				if (num3 > 0.001f && flag)
				{
					EffectInstance effectInstance3 = m_EffectInstances[val3];
					float num5 = math.saturate(audioGroupingSettingsData.m_Scale * num3);
					num5 *= math.saturate((audioGroupingSettingsData.m_NearHeight.y - m_CameraPosition.y) / (audioGroupingSettingsData.m_NearHeight.y - audioGroupingSettingsData.m_NearHeight.x));
					num5 = math.lerp(effectInstance3.m_Intensity, num5, audioGroupingSettingsData.m_FadeSpeed);
					effectInstance3.m_Position = val;
					effectInstance3.m_Rotation = quaternion.identity;
					effectInstance3.m_Intensity = math.saturate(num5);
					m_EffectInstances[val3] = effectInstance3;
					m_SourceUpdateData.Add(val3, new Transform
					{
						m_Position = val,
						m_Rotation = quaternion.identity
					});
				}
				else
				{
					if (m_EffectInstances.HasComponent(val3))
					{
						EffectInstance effectInstance4 = m_EffectInstances[val3];
						effectInstance4.m_Intensity = 0f;
						m_EffectInstances[val3] = effectInstance4;
					}
					m_SourceUpdateData.Remove(val3);
				}
			}
		}

		private bool IsNearForestOnFire(float3 cameraPosition)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_OnFireTrees.Length; i++)
			{
				Entity val = m_OnFireTrees[i];
				if (m_TransformData.HasComponent(val) && math.distancesq(m_TransformData[val].m_Position, cameraPosition) < m_ForestFireDistance * m_ForestFireDistance)
				{
					return true;
				}
			}
			return false;
		}
	}

	private struct TypeHandle
	{
		public ComponentLookup<EffectInstance> __Game_Effects_EffectInstance_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EffectData> __Game_Prefabs_EffectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

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
			__Game_Effects_EffectInstance_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EffectInstance>(false);
			__Game_Prefabs_EffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EffectData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
		}
	}

	private TrafficAmbienceSystem m_TrafficAmbienceSystem;

	private ZoneAmbienceSystem m_ZoneAmbienceSystem;

	private EffectFlagSystem m_EffectFlagSystem;

	private SimulationSystem m_SimulationSystem;

	private ClimateSystem m_ClimateSystem;

	private AudioManager m_AudioManager;

	private EntityQuery m_AudioGroupingConfigurationQuery;

	private EntityQuery m_AudioGroupingMiscSettingQuery;

	private NativeArray<Entity> m_AmbienceEntities;

	private NativeArray<Entity> m_NearAmbienceEntities;

	private NativeArray<AudioGroupingSettingsData> m_Settings;

	private TerrainSystem m_TerrainSystem;

	private EntityQuery m_OnFireTreeQuery;

	[EnumArray(typeof(GroupAmbienceType))]
	[DebugWatchValue]
	private NativeArray<float> m_CurrentValues;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_TrafficAmbienceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TrafficAmbienceSystem>();
		m_ZoneAmbienceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneAmbienceSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_EffectFlagSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EffectFlagSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_CurrentValues = new NativeArray<float>(22, (Allocator)4, (NativeArrayOptions)1);
		m_AudioGroupingConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AudioGroupingSettingsData>() });
		m_AudioGroupingMiscSettingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AudioGroupingMiscSetting>() });
		m_OnFireTreeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Tree>(),
			ComponentType.ReadOnly<OnFire>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_AudioGroupingConfigurationQuery);
	}

	private Entity CreateEffect(Entity sfx)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Entity val = ((EntityManager)(ref entityManager)).CreateEntity();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponentData<EffectInstance>(val, default(EffectInstance));
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponentData<PrefabRef>(val, new PrefabRef
		{
			m_Prefab = sfx
		});
		return val;
	}

	private void Initialize()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_AudioGroupingConfigurationQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		List<AudioGroupingSettingsData> list = new List<AudioGroupingSettingsData>();
		Enumerator<Entity> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
				list.AddRange((IEnumerable<AudioGroupingSettingsData>)(object)((EntityManager)(ref entityManager)).GetBuffer<AudioGroupingSettingsData>(current, true).AsNativeArray());
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		if (!m_Settings.IsCreated)
		{
			m_Settings = ListExtensions.ToNativeArray<AudioGroupingSettingsData>(list, AllocatorHandle.op_Implicit((Allocator)4));
		}
		val.Dispose();
		if (!m_AmbienceEntities.IsCreated)
		{
			m_AmbienceEntities = new NativeArray<Entity>(m_Settings.Length, (Allocator)4, (NativeArrayOptions)1);
		}
		if (!m_NearAmbienceEntities.IsCreated)
		{
			m_NearAmbienceEntities = new NativeArray<Entity>(m_Settings.Length, (Allocator)4, (NativeArrayOptions)1);
		}
		for (int i = 0; i < m_Settings.Length; i++)
		{
			m_AmbienceEntities[i] = CreateEffect(m_Settings[i].m_GroupSoundFar);
			m_NearAmbienceEntities[i] = ((m_Settings[i].m_GroupSoundNear != Entity.Null) ? CreateEffect(m_Settings[i].m_GroupSoundNear) : Entity.Null);
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		if (m_AmbienceEntities.IsCreated)
		{
			m_AmbienceEntities.Dispose();
		}
		if (m_NearAmbienceEntities.IsCreated)
		{
			m_NearAmbienceEntities.Dispose();
		}
		if (m_Settings.IsCreated)
		{
			m_Settings.Dispose();
		}
		m_CurrentValues.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode != GameMode.Game || GameManager.instance.isGameLoading)
		{
			return;
		}
		if (m_AmbienceEntities.Length != 0)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<EffectInstance>(m_AmbienceEntities[0]))
			{
				goto IL_004a;
			}
		}
		Initialize();
		goto IL_004a;
		IL_004a:
		Camera main = Camera.main;
		if (!((Object)(object)main == (Object)null))
		{
			float3 cameraPosition = float3.op_Implicit(((Component)main).transform.position);
			AudioGroupingMiscSetting singleton = ((EntityQuery)(ref m_AudioGroupingMiscSettingQuery)).GetSingleton<AudioGroupingMiscSetting>();
			JobHandle deps;
			JobHandle dependencies;
			JobHandle dependencies2;
			AudioGroupingJob audioGroupingJob = new AudioGroupingJob
			{
				m_CameraPosition = cameraPosition,
				m_SourceUpdateData = m_AudioManager.GetSourceUpdateData(out deps),
				m_TrafficAmbienceMap = m_TrafficAmbienceSystem.GetMap(readOnly: true, out dependencies),
				m_AmbienceMap = m_ZoneAmbienceSystem.GetMap(readOnly: true, out dependencies2),
				m_Settings = m_Settings,
				m_EffectFlagData = m_EffectFlagSystem.GetData(),
				m_AmbienceEntities = m_AmbienceEntities,
				m_NearAmbienceEntities = m_NearAmbienceEntities,
				m_OnFireTrees = ((EntityQuery)(ref m_OnFireTreeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3)),
				m_EffectInstances = InternalCompilerInterface.GetComponentLookup<EffectInstance>(ref __TypeHandle.__Game_Effects_EffectInstance_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EffectDatas = InternalCompilerInterface.GetComponentLookup<EffectData>(ref __TypeHandle.__Game_Prefabs_EffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TerrainData = m_TerrainSystem.GetHeightData(),
				m_ForestFireDistance = singleton.m_ForestFireDistance,
				m_Precipitation = m_ClimateSystem.precipitation,
				m_IsRaining = m_ClimateSystem.isRaining,
				m_CurrentValues = m_CurrentValues
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<AudioGroupingJob>(audioGroupingJob, JobHandle.CombineDependencies(JobHandle.CombineDependencies(dependencies2, deps), dependencies, ((SystemBase)this).Dependency));
			m_TerrainSystem.AddCPUHeightReader(((SystemBase)this).Dependency);
			m_AudioManager.AddSourceUpdateWriter(((SystemBase)this).Dependency);
			m_TrafficAmbienceSystem.AddReader(((SystemBase)this).Dependency);
		}
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
	public AudioGroupingSystem()
	{
	}
}

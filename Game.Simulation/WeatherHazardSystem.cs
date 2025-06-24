using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class WeatherHazardSystem : GameSystemBase
{
	[BurstCompile]
	private struct WeatherHazardJob : IJobChunk
	{
		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public float m_TimeDelta;

		[ReadOnly]
		public float m_Temperature;

		[ReadOnly]
		public float m_Rain;

		[ReadOnly]
		public bool m_NaturalDisasters;

		public ParallelWriter m_CommandBuffer;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<EventData> m_PrefabEventType;

		[ReadOnly]
		public ComponentTypeHandle<WeatherPhenomenonData> m_PrefabWeatherPhenomenonType;

		[ReadOnly]
		public ComponentTypeHandle<Locked> m_LockedType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<EventData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EventData>(ref m_PrefabEventType);
			NativeArray<WeatherPhenomenonData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WeatherPhenomenonData>(ref m_PrefabWeatherPhenomenonType);
			EnabledMask enabledMask = ((ArchetypeChunk)(ref chunk)).GetEnabledMask<Locked>(ref m_LockedType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				SafeBitRef enableBit = ((EnabledMask)(ref enabledMask)).EnableBit;
				if (((SafeBitRef)(ref enableBit)).IsValid && ((EnabledMask)(ref enabledMask))[i])
				{
					continue;
				}
				WeatherPhenomenonData weatherPhenomenonData = nativeArray3[i];
				if (weatherPhenomenonData.m_DamageSeverity != 0f && !m_NaturalDisasters)
				{
					continue;
				}
				float num = MathUtils.Center(weatherPhenomenonData.m_OccurenceTemperature);
				float num2 = math.max(0.5f, MathUtils.Extents(weatherPhenomenonData.m_OccurenceTemperature));
				float num3 = (m_Temperature - num) / num2;
				num3 = math.max(0f, 1f - num3 * num3);
				float num4 = 1f;
				if (weatherPhenomenonData.m_OccurenceRain.max > 0.999f)
				{
					if (weatherPhenomenonData.m_OccurenceRain.min >= 0.001f)
					{
						num4 = math.saturate((m_Rain - weatherPhenomenonData.m_OccurenceRain.min) / math.max(0.001f, weatherPhenomenonData.m_OccurenceRain.max - weatherPhenomenonData.m_OccurenceRain.min));
					}
				}
				else if (weatherPhenomenonData.m_OccurenceRain.min < 0.001f)
				{
					num4 = math.saturate((weatherPhenomenonData.m_OccurenceRain.max - m_Rain) / math.max(0.001f, weatherPhenomenonData.m_OccurenceRain.max - weatherPhenomenonData.m_OccurenceRain.min));
				}
				float num5 = weatherPhenomenonData.m_OccurenceProbability * num3 * num4 * m_TimeDelta;
				while (((Random)(ref random)).NextFloat(100f) < num5)
				{
					Entity eventPrefab = nativeArray[i];
					EventData eventData = nativeArray2[i];
					CreateWeatherEvent(unfilteredChunkIndex, eventPrefab, eventData);
					num5 -= 100f;
				}
			}
		}

		private void CreateWeatherEvent(int jobIndex, Entity eventPrefab, EventData eventData)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, eventData.m_Archetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(eventPrefab));
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EventData> __Game_Prefabs_EventData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WeatherPhenomenonData> __Game_Prefabs_WeatherPhenomenonData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Locked> __Game_Prefabs_Locked_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_EventData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EventData>(true);
			__Game_Prefabs_WeatherPhenomenonData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WeatherPhenomenonData>(true);
			__Game_Prefabs_Locked_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Locked>(true);
		}
	}

	private const int UPDATES_PER_DAY = 128;

	private ClimateSystem m_ClimateSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_PhenomenonQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 2048;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PhenomenonQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<EventData>(),
			ComponentType.ReadOnly<WeatherPhenomenonData>(),
			ComponentType.Exclude<Locked>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_PhenomenonQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		WeatherHazardJob weatherHazardJob = new WeatherHazardJob
		{
			m_RandomSeed = RandomSeed.Next(),
			m_TimeDelta = 34.133335f,
			m_Temperature = m_ClimateSystem.temperature,
			m_Rain = m_ClimateSystem.precipitation,
			m_NaturalDisasters = m_CityConfigurationSystem.naturalDisasters
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		weatherHazardJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		weatherHazardJob.m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		weatherHazardJob.m_PrefabEventType = InternalCompilerInterface.GetComponentTypeHandle<EventData>(ref __TypeHandle.__Game_Prefabs_EventData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		weatherHazardJob.m_PrefabWeatherPhenomenonType = InternalCompilerInterface.GetComponentTypeHandle<WeatherPhenomenonData>(ref __TypeHandle.__Game_Prefabs_WeatherPhenomenonData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		weatherHazardJob.m_LockedType = InternalCompilerInterface.GetComponentTypeHandle<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		WeatherHazardJob weatherHazardJob2 = weatherHazardJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<WeatherHazardJob>(weatherHazardJob2, m_PhenomenonQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public WeatherHazardSystem()
	{
	}
}

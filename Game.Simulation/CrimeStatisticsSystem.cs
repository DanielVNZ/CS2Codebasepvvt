using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CrimeStatisticsSystem : GameSystemBase
{
	[BurstCompile]
	private struct AverageCrimeJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<CrimeProducer> m_CrimeProducerType;

		public float m_MaxCrimeAccumulation;

		public ParallelWriter<AverageFloat> m_AverageCrime;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CrimeProducer> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CrimeProducer>(ref m_CrimeProducerType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				m_AverageCrime.Accumulate(new AverageFloat
				{
					m_Count = 1,
					m_Total = nativeArray[i].m_Crime / m_MaxCrimeAccumulation
				});
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct StatisticsJob : IJob
	{
		public NativeAccumulator<AverageFloat> m_AverageCrime;

		public NativeQueue<StatisticsEvent> m_StatisticsEventQueue;

		public void Execute()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			ref NativeQueue<StatisticsEvent> reference = ref m_StatisticsEventQueue;
			StatisticsEvent statisticsEvent = new StatisticsEvent
			{
				m_Statistic = StatisticType.CrimeRate
			};
			AverageFloat result = m_AverageCrime.GetResult(0);
			statisticsEvent.m_Change = 100f * ((AverageFloat)(ref result)).average;
			reference.Enqueue(statisticsEvent);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<CrimeProducer> __Game_Buildings_CrimeProducer_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Buildings_CrimeProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CrimeProducer>(true);
		}
	}

	private CityStatisticsSystem m_CityStatisticsSystem;

	private EntityQuery m_CrimeProducerQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_263205583_0;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 8192;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_CrimeProducerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<CrimeProducer>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CrimeProducerQuery);
		((ComponentSystemBase)this).RequireForUpdate<PoliceConfigurationData>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		PoliceConfigurationData singleton = ((EntityQuery)(ref __query_263205583_0)).GetSingleton<PoliceConfigurationData>();
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, singleton.m_PoliceServicePrefab))
		{
			NativeAccumulator<AverageFloat> averageCrime = default(NativeAccumulator<AverageFloat>);
			averageCrime._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			AverageCrimeJob averageCrimeJob = new AverageCrimeJob
			{
				m_CrimeProducerType = InternalCompilerInterface.GetComponentTypeHandle<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MaxCrimeAccumulation = singleton.m_MaxCrimeAccumulation,
				m_AverageCrime = averageCrime.AsParallelWriter()
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<AverageCrimeJob>(averageCrimeJob, m_CrimeProducerQuery, ((SystemBase)this).Dependency);
			JobHandle deps;
			StatisticsJob statisticsJob = new StatisticsJob
			{
				m_AverageCrime = averageCrime,
				m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps)
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<StatisticsJob>(statisticsJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
			m_CityStatisticsSystem.AddWriter(((SystemBase)this).Dependency);
			averageCrime.Dispose(((SystemBase)this).Dependency);
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
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<PoliceConfigurationData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_263205583_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public CrimeStatisticsSystem()
	{
	}
}

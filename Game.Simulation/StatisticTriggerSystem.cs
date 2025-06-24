using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.City;
using Game.Prefabs;
using Game.Triggers;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class StatisticTriggerSystem : GameSystemBase
{
	[BurstCompile]
	private struct SendTriggersJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<StatisticTriggerData> m_StatisticTriggerDataHandle;

		[ReadOnly]
		public ComponentLookup<StatisticsData> m_StatisticsDatas;

		[ReadOnly]
		public ComponentLookup<Locked> m_Locked;

		[ReadOnly]
		public BufferLookup<CityStatistic> m_CityStatistics;

		[ReadOnly]
		public NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> m_StatisticsLookup;

		public ParallelWriter<TriggerAction> m_ActionQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityTypeHandle);
			NativeArray<StatisticTriggerData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<StatisticTriggerData>(ref m_StatisticTriggerDataHandle);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				StatisticTriggerData statisticTriggerData = nativeArray2[i];
				NativeArray<int> statisticDataArray = CityStatisticsSystem.GetStatisticDataArray(m_StatisticsLookup, m_CityStatistics, m_StatisticsDatas[statisticTriggerData.m_StatisticEntity].m_StatisticType, statisticTriggerData.m_StatisticParameter);
				int num = math.max(1, statisticTriggerData.m_TimeFrame);
				TriggerAction triggerAction = new TriggerAction
				{
					m_TriggerType = TriggerType.StatisticsValue,
					m_TriggerPrefab = nativeArray[i]
				};
				if (statisticTriggerData.m_NormalizeWithPrefab != Entity.Null)
				{
					NativeArray<int> statisticDataArray2 = CityStatisticsSystem.GetStatisticDataArray(m_StatisticsLookup, m_CityStatistics, m_StatisticsDatas[statisticTriggerData.m_NormalizeWithPrefab].m_StatisticType, statisticTriggerData.m_NormalizeWithParameter);
					if (statisticDataArray.Length < num || statisticDataArray2.Length < num || statisticDataArray.Length < statisticTriggerData.m_MinSamples || statisticDataArray2.Length < statisticTriggerData.m_MinSamples || EntitiesExtensions.HasEnabledComponent<Locked>(m_Locked, statisticTriggerData.m_StatisticEntity) || EntitiesExtensions.HasEnabledComponent<Locked>(m_Locked, statisticTriggerData.m_NormalizeWithPrefab))
					{
						continue;
					}
					if (statisticTriggerData.m_Type == StatisticTriggerType.TotalValue)
					{
						if (NonZeroValues(statisticDataArray2, num))
						{
							float num2 = 0f;
							for (int j = 1; j <= num; j++)
							{
								num2 += (float)statisticDataArray[statisticDataArray.Length - j] / (float)statisticDataArray2[statisticDataArray2.Length - j];
							}
							triggerAction.m_Value = num2;
							m_ActionQueue.Enqueue(triggerAction);
						}
					}
					else if (statisticTriggerData.m_Type == StatisticTriggerType.AverageValue)
					{
						if (NonZeroValues(statisticDataArray2, num))
						{
							float num3 = 0f;
							for (int k = 1; k <= num; k++)
							{
								num3 += (float)statisticDataArray[statisticDataArray.Length - k] / (float)statisticDataArray2[statisticDataArray2.Length - k];
							}
							triggerAction.m_Value = num3 / (float)num;
							m_ActionQueue.Enqueue(triggerAction);
						}
					}
					else if (statisticTriggerData.m_Type == StatisticTriggerType.AbsoluteChange)
					{
						if (statisticDataArray2[statisticDataArray2.Length - num] != 0 && statisticDataArray2[statisticDataArray2.Length - 1] != 0)
						{
							float num4 = (float)statisticDataArray[statisticDataArray.Length - num] / (float)statisticDataArray2[statisticDataArray2.Length - num];
							float num5 = (float)statisticDataArray[statisticDataArray.Length - 1] / (float)statisticDataArray2[statisticDataArray2.Length - 1];
							triggerAction.m_Value = num5 - num4;
							m_ActionQueue.Enqueue(triggerAction);
						}
					}
					else if (statisticTriggerData.m_Type == StatisticTriggerType.RelativeChange && statisticDataArray2[statisticDataArray2.Length - num] != 0 && statisticDataArray2[statisticDataArray2.Length - 1] != 0)
					{
						float num6 = statisticDataArray[statisticDataArray.Length - num] / statisticDataArray2[statisticDataArray2.Length - num];
						float num7 = statisticDataArray[statisticDataArray.Length - 1] / statisticDataArray2[statisticDataArray2.Length - 1];
						if (num6 != 0f)
						{
							triggerAction.m_Value = (num7 - num6) / num6;
							m_ActionQueue.Enqueue(triggerAction);
						}
					}
				}
				else
				{
					if (statisticDataArray.Length < num || statisticDataArray.Length < statisticTriggerData.m_MinSamples || EntitiesExtensions.HasEnabledComponent<Locked>(m_Locked, statisticTriggerData.m_StatisticEntity))
					{
						continue;
					}
					if (statisticTriggerData.m_Type == StatisticTriggerType.TotalValue)
					{
						float num8 = 0f;
						for (int l = 1; l <= num; l++)
						{
							num8 += (float)statisticDataArray[statisticDataArray.Length - l];
						}
						triggerAction.m_Value = num8;
						m_ActionQueue.Enqueue(triggerAction);
					}
					else if (statisticTriggerData.m_Type == StatisticTriggerType.AverageValue)
					{
						float num9 = 0f;
						for (int m = 1; m <= num; m++)
						{
							num9 += (float)statisticDataArray[statisticDataArray.Length - m];
						}
						triggerAction.m_Value = num9 / (float)num;
						m_ActionQueue.Enqueue(triggerAction);
					}
					else if (statisticTriggerData.m_Type == StatisticTriggerType.AbsoluteChange)
					{
						float num10 = statisticDataArray[statisticDataArray.Length - num];
						float num11 = statisticDataArray[statisticDataArray.Length - 1];
						triggerAction.m_Value = num11 - num10;
						m_ActionQueue.Enqueue(triggerAction);
					}
					else if (statisticTriggerData.m_Type == StatisticTriggerType.RelativeChange)
					{
						float num12 = statisticDataArray[statisticDataArray.Length - num];
						float num13 = statisticDataArray[statisticDataArray.Length - 1];
						if (num12 != 0f)
						{
							triggerAction.m_Value = (num13 - num12) / num12;
							m_ActionQueue.Enqueue(triggerAction);
						}
					}
				}
			}
		}

		private bool NonZeroValues(NativeArray<int> values, int timeframe)
		{
			for (int i = 1; i <= timeframe; i++)
			{
				if (values[values.Length - i] == 0)
				{
					return false;
				}
			}
			return true;
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
		public ComponentTypeHandle<StatisticTriggerData> __Game_Prefabs_StatisticTriggerData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<StatisticsData> __Game_Prefabs_StatisticsData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Locked> __Game_Prefabs_Locked_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityStatistic> __Game_City_CityStatistic_RO_BufferLookup;

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
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_StatisticTriggerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StatisticTriggerData>(true);
			__Game_Prefabs_StatisticsData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StatisticsData>(true);
			__Game_Prefabs_Locked_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Locked>(true);
			__Game_City_CityStatistic_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityStatistic>(true);
		}
	}

	public const int kUpdatesPerDay = 32;

	private ICityStatisticsSystem m_CityStatisticsSystem;

	private EntityQuery m_PrefabQuery;

	private TriggerSystem m_TriggerSystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 8192;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 0;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<StatisticTriggerData>(),
			ComponentType.ReadOnly<TriggerData>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		SendTriggersJob sendTriggersJob = new SendTriggersJob
		{
			m_EntityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StatisticTriggerDataHandle = InternalCompilerInterface.GetComponentTypeHandle<StatisticTriggerData>(ref __TypeHandle.__Game_Prefabs_StatisticTriggerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StatisticsDatas = InternalCompilerInterface.GetComponentLookup<StatisticsData>(ref __TypeHandle.__Game_Prefabs_StatisticsData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Locked = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityStatistics = InternalCompilerInterface.GetBufferLookup<CityStatistic>(ref __TypeHandle.__Game_City_CityStatistic_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StatisticsLookup = m_CityStatisticsSystem.GetLookup(),
			m_ActionQueue = m_TriggerSystem.CreateActionBuffer().AsParallelWriter()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<SendTriggersJob>(sendTriggersJob, m_PrefabQuery, ((SystemBase)this).Dependency);
		m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
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
	public StatisticTriggerSystem()
	{
	}
}

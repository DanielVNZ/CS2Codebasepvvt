using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.City;
using Game.Prefabs;
using Game.Serialization;
using Game.Triggers;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CityStatisticsSystem : GameSystemBase, ICityStatisticsSystem, IDefaultSerializable, ISerializable, IPostDeserialize
{
	public readonly struct StatisticsKey : IEquatable<StatisticsKey>
	{
		public StatisticType type { get; }

		public int parameter { get; }

		public StatisticsKey(StatisticType type, int parameter)
		{
			this.type = type;
			this.parameter = parameter;
		}

		public override bool Equals(object obj)
		{
			if (obj is StatisticsKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public bool Equals(StatisticsKey other)
		{
			StatisticType num = type;
			int num2 = parameter;
			StatisticType statisticType = other.type;
			int num3 = other.parameter;
			if (num == statisticType)
			{
				return num2 == num3;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((int)type * 311 + parameter).GetHashCode();
		}
	}

	public struct SafeStatisticQueue
	{
		[NativeDisableContainerSafetyRestriction]
		private ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		[ReadOnly]
		public bool m_StatisticsEnabled;

		public SafeStatisticQueue(NativeQueue<StatisticsEvent> queue, bool enabled)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			m_StatisticsEventQueue = queue.AsParallelWriter();
			m_StatisticsEnabled = enabled;
		}

		public void Enqueue(StatisticsEvent statisticsEvent)
		{
			if (m_StatisticsEnabled)
			{
				m_StatisticsEventQueue.Enqueue(statisticsEvent);
			}
		}
	}

	[BurstCompile]
	private struct CityStatisticsJob : IJob
	{
		public NativeQueue<StatisticsEvent> m_StatisticsEventQueue;

		[ReadOnly]
		public CountHouseholdDataSystem.HouseholdData m_HouseholdData;

		[ReadOnly]
		public ComponentLookup<Tourism> m_Tourisms;

		[ReadOnly]
		public Entity m_City;

		public float m_Money;

		public void Execute()
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.Money,
				m_Change = m_Money
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.LodgingTotal,
				m_Change = m_Tourisms[m_City].m_Lodging.y
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.LodgingUsed,
				m_Change = m_Tourisms[m_City].m_Lodging.x
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.HouseholdCount,
				m_Change = m_HouseholdData.m_MovedInHouseholdCount
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.TouristCount,
				m_Change = m_HouseholdData.m_TouristCitizenCount
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.AdultsCount,
				m_Change = m_HouseholdData.m_AdultCount
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.WorkerCount,
				m_Change = m_HouseholdData.m_CityWorkerCount
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.Unemployed,
				m_Change = m_HouseholdData.Unemployed()
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.Population,
				m_Change = m_HouseholdData.Population()
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.Age,
				m_Change = m_HouseholdData.m_ChildrenCount,
				m_Parameter = 0
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.Age,
				m_Change = m_HouseholdData.m_TeenCount,
				m_Parameter = 1
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.Age,
				m_Change = m_HouseholdData.m_AdultCount,
				m_Parameter = 2
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.Age,
				m_Change = m_HouseholdData.m_SeniorCount,
				m_Parameter = 3
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.Wellbeing,
				m_Change = m_HouseholdData.m_TotalMovedInCitizenWellbeing
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.Health,
				m_Change = m_HouseholdData.m_TotalMovedInCitizenHealth
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.EducationCount,
				m_Change = m_HouseholdData.m_UneducatedCount,
				m_Parameter = 0
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.EducationCount,
				m_Change = m_HouseholdData.m_PoorlyEducatedCount,
				m_Parameter = 1
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.EducationCount,
				m_Change = m_HouseholdData.m_EducatedCount,
				m_Parameter = 2
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.EducationCount,
				m_Change = m_HouseholdData.m_WellEducatedCount,
				m_Parameter = 3
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.EducationCount,
				m_Change = m_HouseholdData.m_HighlyEducatedCount,
				m_Parameter = 4
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.HomelessCount,
				m_Change = m_HouseholdData.m_HomelessCitizenCount
			});
		}
	}

	[BurstCompile]
	private struct ProcessStatisticsJob : IJob
	{
		public NativeQueue<StatisticsEvent> m_Queue;

		[ReadOnly]
		public NativeParallelHashMap<StatisticsKey, Entity> m_StatisticsLookup;

		public BufferLookup<CityStatistic> m_Statistics;

		public void Execute()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			StatisticsEvent statisticsEvent = default(StatisticsEvent);
			while (m_Queue.TryDequeue(ref statisticsEvent))
			{
				if (statisticsEvent.m_Statistic == StatisticType.Count)
				{
					continue;
				}
				StatisticsKey statisticsKey = new StatisticsKey(statisticsEvent.m_Statistic, statisticsEvent.m_Parameter);
				if (!m_StatisticsLookup.ContainsKey(statisticsKey))
				{
					continue;
				}
				Entity val = m_StatisticsLookup[statisticsKey];
				if (m_Statistics.HasBuffer(val))
				{
					DynamicBuffer<CityStatistic> val2 = m_Statistics[val];
					if (val2.Length == 0)
					{
						val2.Add(new CityStatistic
						{
							m_TotalValue = 0.0,
							m_Value = 0.0
						});
					}
					CityStatistic cityStatistic = val2[val2.Length - 1];
					if (val2.Length == 1 && statisticsEvent.m_Statistic == StatisticType.Money)
					{
						cityStatistic.m_TotalValue = statisticsEvent.m_Change;
					}
					cityStatistic.m_Value += statisticsEvent.m_Change;
					val2[val2.Length - 1] = cityStatistic;
				}
			}
		}
	}

	[BurstCompile]
	private struct ResetEntityJob : IJob
	{
		public int m_Money;

		[ReadOnly]
		public NativeParallelHashMap<StatisticsKey, Entity> m_StatisticsLookup;

		public BufferLookup<CityStatistic> m_Statistics;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<StatisticsData> m_PrefabStats;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<StatisticsKey> keyArray = m_StatisticsLookup.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < keyArray.Length; i++)
			{
				Entity val = m_StatisticsLookup[keyArray[i]];
				if (!m_Statistics.HasBuffer(val))
				{
					continue;
				}
				DynamicBuffer<CityStatistic> val2 = m_Statistics[val];
				Entity prefab = m_Prefabs[val].m_Prefab;
				StatisticsData statisticsData = m_PrefabStats[prefab];
				if (val2.Length == 0)
				{
					val2.Add(new CityStatistic
					{
						m_TotalValue = 0.0,
						m_Value = ((statisticsData.m_StatisticType == StatisticType.Money) ? m_Money : 0)
					});
				}
				CityStatistic cityStatistic = val2[val2.Length - 1];
				if (statisticsData.m_CollectionType == StatisticCollectionType.Cumulative)
				{
					val2.Add(new CityStatistic
					{
						m_TotalValue = cityStatistic.m_TotalValue + cityStatistic.m_Value,
						m_Value = 0.0
					});
				}
				else if (statisticsData.m_CollectionType == StatisticCollectionType.Point)
				{
					val2.Add(new CityStatistic
					{
						m_TotalValue = cityStatistic.m_Value,
						m_Value = 0.0
					});
				}
				else if (statisticsData.m_CollectionType == StatisticCollectionType.Daily)
				{
					double num = 0.0;
					if (val2.Length >= 32)
					{
						num = val2[val2.Length - 32].m_Value;
					}
					val2.Add(new CityStatistic
					{
						m_TotalValue = cityStatistic.m_TotalValue + cityStatistic.m_Value - num,
						m_Value = 0.0
					});
				}
			}
		}
	}

	private struct TypeHandle
	{
		public ComponentLookup<Tourism> __Game_City_Tourism_RW_ComponentLookup;

		public BufferLookup<CityStatistic> __Game_City_CityStatistic_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StatisticsData> __Game_Prefabs_StatisticsData_RO_ComponentLookup;

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
			__Game_City_Tourism_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tourism>(false);
			__Game_City_CityStatistic_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityStatistic>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_StatisticsData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StatisticsData>(true);
		}
	}

	public const int kUpdatesPerDay = 32;

	private CitySystem m_CitySystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private CountHouseholdDataSystem m_CountHouseholdDataSystem;

	private TriggerSystem m_TriggerSystem;

	private EntityQuery m_StatisticsPrefabQuery;

	private EntityQuery m_StatisticsQuery;

	private EntityQuery m_CityQuery;

	private NativeParallelHashMap<StatisticsKey, Entity> m_StatisticsLookup;

	private NativeQueue<StatisticsEvent> m_StatisticsEventQueue;

	private JobHandle m_Writers;

	private bool m_Initialized;

	private int m_SampleCount = 1;

	private uint m_LastSampleFrameIndex;

	private TypeHandle __TypeHandle;

	public int sampleCount => m_SampleCount;

	public Action eventStatisticsUpdated { get; set; }

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 8192;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 0;
	}

	public NativeParallelHashMap<StatisticsKey, Entity> GetLookup()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_StatisticsLookup;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_CountHouseholdDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountHouseholdDataSystem>();
		m_StatisticsPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<StatisticsData>() });
		m_StatisticsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CityStatistic>() });
		m_CityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_CityQuery);
		m_StatisticsLookup = new NativeParallelHashMap<StatisticsKey, Entity>(64, AllocatorHandle.op_Implicit((Allocator)4));
		m_StatisticsEventQueue = new NativeQueue<StatisticsEvent>(AllocatorHandle.op_Implicit((Allocator)4));
		((ComponentSystemBase)this).Enabled = false;
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		((ComponentSystemBase)this).Enabled = mode.IsGame();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_StatisticsLookup.Dispose();
		m_StatisticsEventQueue.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Initialized)
		{
			InitializeLookup();
		}
		JobHandle val = IJobExtensions.Schedule<CityStatisticsJob>(new CityStatisticsJob
		{
			m_StatisticsEventQueue = m_StatisticsEventQueue,
			m_HouseholdData = m_CountHouseholdDataSystem.GetHouseholdCountData(),
			m_Tourisms = InternalCompilerInterface.GetComponentLookup<Tourism>(ref __TypeHandle.__Game_City_Tourism_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_City = m_CitySystem.City,
			m_Money = m_CitySystem.moneyAmount
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_Writers));
		JobHandle val2 = IJobExtensions.Schedule<ProcessStatisticsJob>(new ProcessStatisticsJob
		{
			m_Statistics = InternalCompilerInterface.GetBufferLookup<CityStatistic>(ref __TypeHandle.__Game_City_CityStatistic_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StatisticsLookup = m_StatisticsLookup,
			m_Queue = m_StatisticsEventQueue
		}, JobHandle.CombineDependencies(val, ((SystemBase)this).Dependency));
		JobHandle dependency = IJobExtensions.Schedule<ResetEntityJob>(new ResetEntityJob
		{
			m_Money = m_CitySystem.moneyAmount,
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStats = InternalCompilerInterface.GetComponentLookup<StatisticsData>(ref __TypeHandle.__Game_Prefabs_StatisticsData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Statistics = InternalCompilerInterface.GetBufferLookup<CityStatistic>(ref __TypeHandle.__Game_City_CityStatistic_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StatisticsLookup = m_StatisticsLookup
		}, val2);
		((SystemBase)this).Dependency = dependency;
		m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		AddWriter(((SystemBase)this).Dependency);
		m_SampleCount++;
		m_LastSampleFrameIndex = m_SimulationSystem.frameIndex;
		eventStatisticsUpdated?.Invoke();
	}

	public static int GetStatisticValue(NativeParallelHashMap<StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats, StatisticType type, int parameter = 0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		long statisticValueLong = GetStatisticValueLong(statisticsLookup, stats, type, parameter);
		if (statisticValueLong <= int.MaxValue)
		{
			if (statisticValueLong >= int.MinValue)
			{
				return (int)statisticValueLong;
			}
			return int.MinValue;
		}
		return int.MaxValue;
	}

	public static long GetStatisticValueLong(NativeParallelHashMap<StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats, StatisticType type, int parameter = 0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		double statisticValueDouble = GetStatisticValueDouble(statisticsLookup, stats, type, parameter);
		if (!(statisticValueDouble > 9.223372036854776E+18))
		{
			if (!(statisticValueDouble < -9.223372036854776E+18))
			{
				return (long)statisticValueDouble;
			}
			return long.MinValue;
		}
		return long.MaxValue;
	}

	private static double GetStatisticValueDouble(NativeParallelHashMap<StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats, StatisticType type, int parameter = 0)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		StatisticsKey statisticsKey = new StatisticsKey(type, parameter);
		if (statisticsLookup.ContainsKey(statisticsKey))
		{
			Entity val = statisticsLookup[statisticsKey];
			if (stats.HasBuffer(val))
			{
				DynamicBuffer<CityStatistic> val2 = stats[val];
				if (val2.Length > 0)
				{
					return Math.Round(val2[val2.Length - 1].m_TotalValue, MidpointRounding.AwayFromZero);
				}
				return 0.0;
			}
		}
		return 0.0;
	}

	public int GetStatisticValue(BufferLookup<CityStatistic> stats, StatisticType type, int parameter = 0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		long statisticValueLong = GetStatisticValueLong(m_StatisticsLookup, stats, type, parameter);
		if (statisticValueLong <= int.MaxValue)
		{
			if (statisticValueLong >= int.MinValue)
			{
				return (int)statisticValueLong;
			}
			return int.MinValue;
		}
		return int.MaxValue;
	}

	public long GetStatisticValueLong(BufferLookup<CityStatistic> stats, StatisticType type, int parameter = 0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		double statisticValueDouble = GetStatisticValueDouble(m_StatisticsLookup, stats, type, parameter);
		if (!(statisticValueDouble > 9.223372036854776E+18))
		{
			if (!(statisticValueDouble < -9.223372036854776E+18))
			{
				return (long)statisticValueDouble;
			}
			return long.MinValue;
		}
		return long.MaxValue;
	}

	private double GetStatisticValueDouble(StatisticType type, int parameter = 0)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		StatisticsKey statisticsKey = new StatisticsKey(type, parameter);
		((JobHandle)(ref m_Writers)).Complete();
		if (m_StatisticsLookup.ContainsKey(statisticsKey))
		{
			Entity val = m_StatisticsLookup[statisticsKey];
			DynamicBuffer<CityStatistic> val2 = default(DynamicBuffer<CityStatistic>);
			if (EntitiesExtensions.TryGetBuffer<CityStatistic>(((ComponentSystemBase)this).EntityManager, val, true, ref val2) && val2.Length > 0)
			{
				return Math.Round(val2[val2.Length - 1].m_TotalValue, MidpointRounding.AwayFromZero);
			}
		}
		return 0.0;
	}

	public int GetStatisticValue(StatisticType type, int parameter = 0)
	{
		long statisticValueLong = GetStatisticValueLong(type, parameter);
		if (statisticValueLong <= int.MaxValue)
		{
			if (statisticValueLong >= int.MinValue)
			{
				return (int)statisticValueLong;
			}
			return int.MinValue;
		}
		return int.MaxValue;
	}

	public long GetStatisticValueLong(StatisticType type, int parameter = 0)
	{
		double statisticValueDouble = GetStatisticValueDouble(type, parameter);
		if (!(statisticValueDouble > 9.223372036854776E+18))
		{
			if (!(statisticValueDouble < -9.223372036854776E+18))
			{
				return (long)statisticValueDouble;
			}
			return long.MinValue;
		}
		return long.MaxValue;
	}

	public static NativeArray<long> GetStatisticDataArrayLong(NativeParallelHashMap<StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats, StatisticType type, int parameter = 0)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		StatisticsKey statisticsKey = new StatisticsKey(type, parameter);
		if (statisticsLookup.ContainsKey(statisticsKey))
		{
			Entity val = statisticsLookup[statisticsKey];
			if (stats.HasBuffer(val))
			{
				DynamicBuffer<CityStatistic> val2 = stats[val];
				NativeArray<long> result = default(NativeArray<long>);
				result._002Ector(val2.Length, (Allocator)2, (NativeArrayOptions)1);
				for (int i = 0; i < val2.Length; i++)
				{
					double num = Math.Round(val2[i].m_TotalValue, MidpointRounding.AwayFromZero);
					result[i] = ((num > 9.223372036854776E+18) ? long.MaxValue : ((num < -9.223372036854776E+18) ? long.MinValue : ((long)num)));
				}
				return result;
			}
		}
		return new NativeArray<long>(1, (Allocator)2, (NativeArrayOptions)1);
	}

	public static NativeArray<int> GetStatisticDataArray(NativeParallelHashMap<StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats, StatisticType type, int parameter = 0)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		StatisticsKey statisticsKey = new StatisticsKey(type, parameter);
		if (statisticsLookup.ContainsKey(statisticsKey))
		{
			Entity val = statisticsLookup[statisticsKey];
			if (stats.HasBuffer(val))
			{
				DynamicBuffer<CityStatistic> val2 = stats[val];
				NativeArray<int> result = default(NativeArray<int>);
				result._002Ector(val2.Length, (Allocator)2, (NativeArrayOptions)1);
				for (int i = 0; i < val2.Length; i++)
				{
					double num = Math.Round(val2[i].m_TotalValue, MidpointRounding.AwayFromZero);
					result[i] = ((num > 2147483647.0) ? int.MaxValue : ((num < -2147483648.0) ? int.MinValue : ((int)num)));
				}
				return result;
			}
		}
		return new NativeArray<int>(1, (Allocator)2, (NativeArrayOptions)1);
	}

	public NativeArray<long> GetStatisticDataArrayLong(BufferLookup<CityStatistic> stats, StatisticType type, int parameter = 0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return GetStatisticDataArrayLong(m_StatisticsLookup, stats, type, parameter);
	}

	public NativeArray<int> GetStatisticDataArray(BufferLookup<CityStatistic> stats, StatisticType type, int parameter = 0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return GetStatisticDataArray(m_StatisticsLookup, stats, type, parameter);
	}

	public NativeArray<long> GetStatisticDataArrayLong(StatisticType type, int parameter = 0)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		StatisticsKey statisticsKey = new StatisticsKey(type, parameter);
		if (m_StatisticsLookup.ContainsKey(statisticsKey))
		{
			Entity val = m_StatisticsLookup[statisticsKey];
			DynamicBuffer<CityStatistic> val2 = default(DynamicBuffer<CityStatistic>);
			if (EntitiesExtensions.TryGetBuffer<CityStatistic>(((ComponentSystemBase)this).EntityManager, val, true, ref val2))
			{
				NativeArray<long> result = default(NativeArray<long>);
				result._002Ector(val2.Length, (Allocator)2, (NativeArrayOptions)1);
				for (int i = 0; i < val2.Length; i++)
				{
					double num = Math.Round(val2[i].m_TotalValue, MidpointRounding.AwayFromZero);
					result[i] = ((num > 9.223372036854776E+18) ? long.MaxValue : ((num < -9.223372036854776E+18) ? long.MinValue : ((long)num)));
				}
				return result;
			}
		}
		return new NativeArray<long>(1, (Allocator)2, (NativeArrayOptions)1);
	}

	public NativeArray<int> GetStatisticDataArray(StatisticType type, int parameter = 0)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		StatisticsKey statisticsKey = new StatisticsKey(type, parameter);
		if (m_StatisticsLookup.ContainsKey(statisticsKey))
		{
			Entity val = m_StatisticsLookup[statisticsKey];
			DynamicBuffer<CityStatistic> val2 = default(DynamicBuffer<CityStatistic>);
			if (EntitiesExtensions.TryGetBuffer<CityStatistic>(((ComponentSystemBase)this).EntityManager, val, true, ref val2))
			{
				NativeArray<int> result = default(NativeArray<int>);
				result._002Ector(val2.Length, (Allocator)2, (NativeArrayOptions)1);
				for (int i = 0; i < val2.Length; i++)
				{
					double num = Math.Round(val2[i].m_TotalValue, MidpointRounding.AwayFromZero);
					result[i] = ((num > 2147483647.0) ? int.MaxValue : ((num < -2147483648.0) ? int.MinValue : ((int)num)));
				}
				return result;
			}
		}
		return new NativeArray<int>(1, (Allocator)2, (NativeArrayOptions)1);
	}

	public NativeArray<CityStatistic> GetStatisticArray(StatisticType type, int parameter = 0)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		StatisticsKey statisticsKey = new StatisticsKey(type, parameter);
		((JobHandle)(ref m_Writers)).Complete();
		if (m_StatisticsLookup.ContainsKey(statisticsKey))
		{
			Entity val = m_StatisticsLookup[statisticsKey];
			DynamicBuffer<CityStatistic> val2 = default(DynamicBuffer<CityStatistic>);
			if (EntitiesExtensions.TryGetBuffer<CityStatistic>(((ComponentSystemBase)this).EntityManager, val, true, ref val2))
			{
				return val2.AsNativeArray();
			}
		}
		return new NativeArray<CityStatistic>(1, (Allocator)2, (NativeArrayOptions)1);
	}

	public uint GetSampleFrameIndex(int index)
	{
		int num = (sampleCount - index - 1) * 8192;
		return m_LastSampleFrameIndex - (uint)num;
	}

	private void InitializeLookup()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		m_StatisticsLookup.Clear();
		NativeArray<Entity> val = ((EntityQuery)(ref m_StatisticsPrefabQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<StatisticsData> val2 = ((EntityQuery)(ref m_StatisticsPrefabQuery)).ToComponentDataArray<StatisticsData>(AllocatorHandle.op_Implicit((Allocator)3));
		DynamicBuffer<StatisticParameterData> val3 = default(DynamicBuffer<StatisticParameterData>);
		for (int i = 0; i < val.Length; i++)
		{
			StatisticType statisticType = val2[i].m_StatisticType;
			if (EntitiesExtensions.TryGetBuffer<StatisticParameterData>(((ComponentSystemBase)this).EntityManager, val[i], true, ref val3))
			{
				for (int j = 0; j < val3.Length; j++)
				{
					m_StatisticsLookup.Add(new StatisticsKey(statisticType, val3[j].m_Value), Entity.Null);
				}
			}
			else
			{
				m_StatisticsLookup.Add(new StatisticsKey(statisticType, 0), Entity.Null);
			}
		}
		NativeArray<Entity> val4 = ((EntityQuery)(ref m_StatisticsQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		bool flag = true;
		PrefabRef prefabRef = default(PrefabRef);
		StatisticParameter statisticParameter = default(StatisticParameter);
		DynamicBuffer<StatisticParameterData> val5 = default(DynamicBuffer<StatisticParameterData>);
		for (int k = 0; k < val4.Length; k++)
		{
			if (!EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val4[k], ref prefabRef))
			{
				continue;
			}
			int num = 0;
			if (EntitiesExtensions.TryGetComponent<StatisticParameter>(((ComponentSystemBase)this).EntityManager, val4[k], ref statisticParameter))
			{
				num = statisticParameter.m_Value;
			}
			if (!EntitiesExtensions.TryGetBuffer<StatisticParameterData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref val5))
			{
				continue;
			}
			flag = false;
			for (int l = 0; l < val5.Length; l++)
			{
				if (num == val5[l].m_Value)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
		}
		EntityManager entityManager;
		if (flag)
		{
			PrefabRef prefabRef2 = default(PrefabRef);
			StatisticsData statisticsData = default(StatisticsData);
			StatisticParameter statisticParameter2 = default(StatisticParameter);
			for (int m = 0; m < val4.Length; m++)
			{
				if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val4[m], ref prefabRef2) && EntitiesExtensions.TryGetComponent<StatisticsData>(((ComponentSystemBase)this).EntityManager, prefabRef2.m_Prefab, ref statisticsData))
				{
					int parameter = 0;
					if (EntitiesExtensions.TryGetComponent<StatisticParameter>(((ComponentSystemBase)this).EntityManager, val4[m], ref statisticParameter2))
					{
						parameter = statisticParameter2.m_Value;
					}
					StatisticsKey statisticsKey = new StatisticsKey(statisticsData.m_StatisticType, parameter);
					if (m_StatisticsLookup.ContainsKey(statisticsKey))
					{
						m_StatisticsLookup[statisticsKey] = val4[m];
					}
				}
			}
		}
		else
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).DestroyEntity(m_StatisticsQuery);
			m_SampleCount = 0;
			m_StatisticsEventQueue.Clear();
		}
		val4.Dispose();
		NativeKeyValueArrays<StatisticsKey, Entity> keyValueArrays = m_StatisticsLookup.GetKeyValueArrays(AllocatorHandle.op_Implicit((Allocator)2));
		DynamicBuffer<StatisticParameterData> val7 = default(DynamicBuffer<StatisticParameterData>);
		StatisticsData statisticsData2 = default(StatisticsData);
		for (int n = 0; n < keyValueArrays.Length; n++)
		{
			if (!(keyValueArrays.Values[n] == Entity.Null))
			{
				continue;
			}
			StatisticsKey statisticsKey2 = keyValueArrays.Keys[n];
			StatisticType type = statisticsKey2.type;
			for (int num2 = 0; num2 < val.Length; num2++)
			{
				Entity val6 = val[num2];
				bool flag2 = EntitiesExtensions.TryGetBuffer<StatisticParameterData>(((ComponentSystemBase)this).EntityManager, val6, true, ref val7);
				if (!EntitiesExtensions.TryGetComponent<StatisticsData>(((ComponentSystemBase)this).EntityManager, val6, ref statisticsData2) || statisticsData2.m_StatisticType != type)
				{
					continue;
				}
				if (flag2)
				{
					bool flag3 = false;
					for (int num3 = 0; num3 < val7.Length; num3++)
					{
						if (val7[num3].m_Value == statisticsKey2.parameter)
						{
							flag3 = true;
						}
					}
					if (!flag3)
					{
						continue;
					}
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				ArchetypeData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ArchetypeData>(val6);
				m_StatisticsLookup[statisticsKey2] = StatisticsPrefab.CreateInstance(((ComponentSystemBase)this).World.EntityManager, val6, componentData, statisticsKey2.parameter);
				break;
			}
		}
		m_Initialized = true;
		val.Dispose();
		val2.Dispose();
	}

	public void CompleteWriters()
	{
		((JobHandle)(ref m_Writers)).Complete();
	}

	public NativeQueue<StatisticsEvent> GetStatisticsEventQueue(out JobHandle deps)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(((ComponentSystemBase)this).Enabled, "Can not write to queue when system isn't running");
		deps = m_Writers;
		return m_StatisticsEventQueue;
	}

	public SafeStatisticQueue GetSafeStatisticsQueue(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_Writers;
		return new SafeStatisticQueue(m_StatisticsEventQueue, ((ComponentSystemBase)this).Enabled);
	}

	public void AddWriter(JobHandle writer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_Writers = JobHandle.CombineDependencies(m_Writers, writer);
	}

	public void DiscardStatistics()
	{
		((JobHandle)(ref m_Writers)).Complete();
		m_StatisticsEventQueue.Clear();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_Writers)).Complete();
		int num = m_SampleCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		uint num2 = m_LastSampleFrameIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		int count = m_StatisticsEventQueue.Count;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(count);
		NativeArray<StatisticsEvent> val = m_StatisticsEventQueue.ToArray(AllocatorHandle.op_Implicit((Allocator)2));
		for (int i = 0; i < val.Length; i++)
		{
			StatisticsEvent statisticsEvent = val[i];
			((IWriter)writer/*cast due to .constrained prefix*/).Write<StatisticsEvent>(statisticsEvent);
		}
		val.Dispose();
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_Writers)).Complete();
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.statisticsRefactor)
		{
			ref int reference = ref m_SampleCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.statsLastFrameIndex)
			{
				ref uint reference2 = ref m_LastSampleFrameIndex;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
			}
			context = ((IReader)reader).context;
			if (!(((Context)(ref context)).version >= Version.statisticsFix2))
			{
				return;
			}
			m_StatisticsEventQueue.Clear();
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			StatisticsEvent statisticsEvent = default(StatisticsEvent);
			for (int i = 0; i < num; i++)
			{
				((IReader)reader/*cast due to .constrained prefix*/).Read<StatisticsEvent>(ref statisticsEvent);
				context = ((IReader)reader).context;
				if (!(((Context)(ref context)).version < Version.statisticUnifying) || (statisticsEvent.m_Statistic != StatisticType.Population && statisticsEvent.m_Statistic != StatisticType.Health && statisticsEvent.m_Statistic != StatisticType.Age && statisticsEvent.m_Statistic != StatisticType.Wellbeing && statisticsEvent.m_Statistic != StatisticType.AdultsCount && statisticsEvent.m_Statistic != StatisticType.HouseholdCount && statisticsEvent.m_Statistic != StatisticType.EducationCount && statisticsEvent.m_Statistic != StatisticType.Unemployed && statisticsEvent.m_Statistic != StatisticType.WorkerCount))
				{
					m_StatisticsEventQueue.Enqueue(statisticsEvent);
				}
			}
		}
		else
		{
			Entity val = default(Entity);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			int num2 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			int num3 = default(int);
			for (int j = 0; j < num2; j++)
			{
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
			}
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			NativeArray<Entity> val2 = default(NativeArray<Entity>);
			val2._002Ector(num2, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<Entity> val3 = val2;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val3);
			val2.Dispose();
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
		}
	}

	public void SetDefaults(Context context)
	{
		m_SampleCount = 0;
		m_StatisticsEventQueue.Clear();
	}

	public void PostDeserialize(Context context)
	{
		m_StatisticsLookup.Clear();
		InitializeLookup();
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
	public CityStatisticsSystem()
	{
	}
}

using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.City;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class BudgetApplySystem : GameSystemBase
{
	[BurstCompile]
	private struct BudgetApplyJob : IJob
	{
		public NativeArray<int> m_Income;

		public NativeArray<int> m_Expenses;

		public ComponentLookup<PlayerMoney> m_PlayerMoneys;

		public ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		public Entity m_City;

		public void Execute()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < 15; i++)
			{
				ExpenseSource parameter = (ExpenseSource)i;
				int expense = CityServiceBudgetSystem.GetExpense((ExpenseSource)i, m_Expenses);
				num -= expense;
				m_StatisticsEventQueue.Enqueue(new StatisticsEvent
				{
					m_Statistic = StatisticType.Expense,
					m_Change = math.abs((float)expense / (float)kUpdatesPerDay),
					m_Parameter = (int)parameter
				});
			}
			for (int j = 0; j < 14; j++)
			{
				IncomeSource parameter2 = (IncomeSource)j;
				int income = CityServiceBudgetSystem.GetIncome((IncomeSource)j, m_Income);
				num += income;
				m_StatisticsEventQueue.Enqueue(new StatisticsEvent
				{
					m_Statistic = StatisticType.Income,
					m_Change = math.abs((float)income / (float)kUpdatesPerDay),
					m_Parameter = (int)parameter2
				});
			}
			PlayerMoney playerMoney = m_PlayerMoneys[m_City];
			playerMoney.Add(num / kUpdatesPerDay);
			m_PlayerMoneys[m_City] = playerMoney;
		}
	}

	private struct TypeHandle
	{
		public ComponentLookup<PlayerMoney> __Game_City_PlayerMoney_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_City_PlayerMoney_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlayerMoney>(false);
		}
	}

	public static readonly int kUpdatesPerDay = 1024;

	private CitySystem m_CitySystem;

	private CityServiceBudgetSystem m_CityServiceBudgetSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CityServiceBudgetSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityServiceBudgetSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		JobHandle deps2;
		JobHandle deps3;
		BudgetApplyJob budgetApplyJob = new BudgetApplyJob
		{
			m_PlayerMoneys = InternalCompilerInterface.GetComponentLookup<PlayerMoney>(ref __TypeHandle.__Game_City_PlayerMoney_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_City = m_CitySystem.City,
			m_Expenses = m_CityServiceBudgetSystem.GetExpenseArray(out deps),
			m_Income = m_CityServiceBudgetSystem.GetIncomeArray(out deps2),
			m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps3).AsParallelWriter()
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<BudgetApplyJob>(budgetApplyJob, JobUtils.CombineDependencies(deps, deps2, deps3, ((SystemBase)this).Dependency));
		m_CityServiceBudgetSystem.AddArrayReader(((SystemBase)this).Dependency);
		m_CityStatisticsSystem.AddWriter(((SystemBase)this).Dependency);
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
	public BudgetApplySystem()
	{
	}
}

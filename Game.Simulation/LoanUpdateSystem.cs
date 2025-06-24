using System.Runtime.CompilerServices;
using Game.City;
using Game.Prefabs;
using Game.Tools;
using Game.Triggers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class LoanUpdateSystem : GameSystemBase
{
	[BurstCompile]
	private struct LoanUpdateJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<Loan> m_Loans;

		[ReadOnly]
		public ComponentLookup<Creditworthiness> m_Creditworthinesses;

		[ReadOnly]
		public ComponentLookup<PlayerMoney> m_PlayerMoneys;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityEffects;

		public NativeQueue<StatisticsEvent> m_StatisticsEventQueue;

		public NativeQueue<TriggerAction> m_TriggerBuffer;

		public EconomyParameterData m_EconomyParameters;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			Loan loan = m_Loans[m_City];
			if (loan.m_Amount > 0)
			{
				float targetInterest = LoanSystem.GetTargetInterest(loan.m_Amount, m_Creditworthinesses[m_City].m_Amount, m_CityEffects[m_City], m_EconomyParameters.m_LoanMinMaxInterestRate);
				Mathf.RoundToInt((float)loan.m_Amount * targetInterest / (float)kUpdatesPerDay);
				PlayerMoney playerMoney = m_PlayerMoneys[m_City];
				if (m_SimulationFrameIndex - loan.m_LastModified > 262144 && playerMoney.money > 0)
				{
					m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.UnpaidLoan, Entity.Null, loan.m_Amount));
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Loan> __Game_Simulation_Loan_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Creditworthiness> __Game_Simulation_Creditworthiness_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlayerMoney> __Game_City_PlayerMoney_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

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
			__Game_Simulation_Loan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Loan>(true);
			__Game_Simulation_Creditworthiness_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creditworthiness>(true);
			__Game_City_PlayerMoney_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlayerMoney>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
		}
	}

	public static readonly int kUpdatesPerDay = 32;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private CitySystem m_CitySystem;

	private TriggerSystem m_TriggerSystem;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_EconomyParametersQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EconomyParametersQuery = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParametersQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		LoanUpdateJob loanUpdateJob = new LoanUpdateJob
		{
			m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps),
			m_City = m_CitySystem.City,
			m_Loans = InternalCompilerInterface.GetComponentLookup<Loan>(ref __TypeHandle.__Game_Simulation_Loan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Creditworthinesses = InternalCompilerInterface.GetComponentLookup<Creditworthiness>(ref __TypeHandle.__Game_Simulation_Creditworthiness_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlayerMoneys = InternalCompilerInterface.GetComponentLookup<PlayerMoney>(ref __TypeHandle.__Game_City_PlayerMoney_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityEffects = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer(),
			m_EconomyParameters = ((EntityQuery)(ref m_EconomyParametersQuery)).GetSingleton<EconomyParameterData>(),
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<LoanUpdateJob>(loanUpdateJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_CityStatisticsSystem.AddWriter(((SystemBase)this).Dependency);
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
	public LoanUpdateSystem()
	{
	}
}

using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.City;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class LoanSystem : GameSystemBase, ILoanSystem
{
	private struct LoanActionJob : IJob
	{
		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		public NativeQueue<LoanAction> m_ActionQueue;

		public ComponentLookup<Loan> m_Loans;

		public ComponentLookup<PlayerMoney> m_Money;

		public void Execute()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			LoanAction loanAction = default(LoanAction);
			while (m_ActionQueue.TryDequeue(ref loanAction))
			{
				PlayerMoney playerMoney = m_Money[m_City];
				playerMoney.Add(loanAction.m_Amount - m_Loans[m_City].m_Amount);
				m_Money[m_City] = playerMoney;
				m_Loans[m_City] = new Loan
				{
					m_Amount = loanAction.m_Amount,
					m_LastModified = m_SimulationFrameIndex
				};
			}
		}
	}

	private struct TypeHandle
	{
		public ComponentLookup<Loan> __Game_Simulation_Loan_RW_ComponentLookup;

		public ComponentLookup<PlayerMoney> __Game_City_PlayerMoney_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Simulation_Loan_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Loan>(false);
			__Game_City_PlayerMoney_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlayerMoney>(false);
		}
	}

	private CitySystem m_CitySystem;

	private SimulationSystem m_SimulationSystem;

	private NativeQueue<LoanAction> m_ActionQueue;

	private JobHandle m_ActionQueueWriters;

	private EntityQuery m_EconomyParametersQuery;

	private TypeHandle __TypeHandle;

	public LoanInfo CurrentLoan
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			Loan loan = default(Loan);
			if (EntitiesExtensions.TryGetComponent<Loan>(((ComponentSystemBase)this).EntityManager, m_CitySystem.City, ref loan))
			{
				return CalculateLoan(loan.m_Amount);
			}
			return default(LoanInfo);
		}
	}

	public int Creditworthiness
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).GetComponentData<Creditworthiness>(m_CitySystem.City).m_Amount;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EconomyParametersQuery = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ActionQueue = new NativeQueue<LoanAction>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_ActionQueue.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (!m_ActionQueue.IsEmpty())
		{
			LoanActionJob loanActionJob = new LoanActionJob
			{
				m_City = m_CitySystem.City,
				m_SimulationFrameIndex = m_SimulationSystem.frameIndex,
				m_ActionQueue = m_ActionQueue,
				m_Loans = InternalCompilerInterface.GetComponentLookup<Loan>(ref __TypeHandle.__Game_Simulation_Loan_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Money = InternalCompilerInterface.GetComponentLookup<PlayerMoney>(ref __TypeHandle.__Game_City_PlayerMoney_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<LoanActionJob>(loanActionJob, JobHandle.CombineDependencies(m_ActionQueueWriters, ((SystemBase)this).Dependency));
			m_ActionQueueWriters = ((SystemBase)this).Dependency;
		}
	}

	public LoanInfo RequestLoanOffer(int amount)
	{
		return CalculateLoan(ClampLoanAmount(amount));
	}

	public void ChangeLoan(int amount)
	{
		((JobHandle)(ref m_ActionQueueWriters)).Complete();
		m_ActionQueue.Enqueue(new LoanAction
		{
			m_Amount = ClampLoanAmount(amount)
		});
	}

	private int ClampLoanAmount(int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PlayerMoney componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlayerMoney>(m_CitySystem.City);
		int num = math.max(0, CurrentLoan.m_Amount - math.max(0, componentData.money));
		return math.clamp(amount, num, Creditworthiness);
	}

	public LoanInfo CalculateLoan(int amount)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		float2 loanMinMaxInterestRate = ((EntityQuery)(ref m_EconomyParametersQuery)).GetSingleton<EconomyParameterData>().m_LoanMinMaxInterestRate;
		int creditworthiness = Creditworthiness;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return CalculateLoan(amount, creditworthiness, ((EntityManager)(ref entityManager)).GetBuffer<CityModifier>(m_CitySystem.City, true), loanMinMaxInterestRate);
	}

	public static LoanInfo CalculateLoan(int amount, int creditworthiness, DynamicBuffer<CityModifier> modifiers, float2 interestRange)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (amount > 0)
		{
			float targetInterest = GetTargetInterest(amount, creditworthiness, modifiers, interestRange);
			return new LoanInfo
			{
				m_Amount = amount,
				m_DailyInterestRate = targetInterest,
				m_DailyPayment = Mathf.RoundToInt((float)amount * targetInterest)
			};
		}
		return default(LoanInfo);
	}

	public static float GetTargetInterest(int loanAmount, int creditworthiness, DynamicBuffer<CityModifier> cityEffects, float2 interestRange)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		float value = 100f * math.lerp(interestRange.x, interestRange.y, math.saturate((float)loanAmount / math.max(1f, (float)creditworthiness)));
		CityUtils.ApplyModifier(ref value, cityEffects, CityModifierType.LoanInterest);
		return math.max(0f, 0.01f * value);
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
	public LoanSystem()
	{
	}
}

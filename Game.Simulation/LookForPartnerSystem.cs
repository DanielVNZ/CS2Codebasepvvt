using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Citizens;
using Game.Common;
using Game.Debug;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class LookForPartnerSystem : GameSystemBase
{
	[BurstCompile]
	private struct AddPartnerSeekerJob : IJob
	{
		public NativeValue<int> m_DebugLookingForPartner;

		public Entity m_LookingForPartnerEntity;

		public BufferLookup<LookingForPartner> m_LookingForPartners;

		public NativeQueue<LookingForPartner> m_Queue;

		public void Execute()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			m_DebugLookingForPartner.value = m_Queue.Count;
			DynamicBuffer<LookingForPartner> val = m_LookingForPartners[m_LookingForPartnerEntity];
			LookingForPartner lookingForPartner = default(LookingForPartner);
			while (m_Queue.TryDequeue(ref lookingForPartner))
			{
				val.Add(lookingForPartner);
			}
		}
	}

	[BurstCompile]
	private struct LookForPartnerJob : IJobChunk
	{
		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<int> m_DebugLookForPartnerQueue;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> m_HouseholdMemberType;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblems;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> m_Tourists;

		[ReadOnly]
		public ComponentLookup<CommuterHousehold> m_Commuters;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		public ParallelWriter<LookingForPartner> m_Queue;

		public RandomSeed m_RandomSeed;

		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public CitizenParametersData m_CitizenParametersData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<HouseholdMember> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdMember>(ref m_HouseholdMemberType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Citizen citizen = m_Citizens[val];
				CitizenAge age = citizen.GetAge();
				if (age == CitizenAge.Child || age == CitizenAge.Teen || (citizen.m_State & CitizenFlags.LookingForPartner) != CitizenFlags.None || CitizenUtils.IsDead(val, ref m_HealthProblems))
				{
					continue;
				}
				Entity household = nativeArray2[i].m_Household;
				if (!m_HouseholdCitizens.HasBuffer(household) || m_Tourists.HasComponent(household) || m_Commuters.HasComponent(household) || (m_Households[household].m_Flags & HouseholdFlags.MovedIn) == 0)
				{
					continue;
				}
				DynamicBuffer<HouseholdCitizen> val2 = m_HouseholdCitizens[household];
				int num = 0;
				for (int j = 0; j < val2.Length; j++)
				{
					Entity citizen2 = val2[j].m_Citizen;
					CitizenAge age2 = m_Citizens[citizen2].GetAge();
					if (age2 == CitizenAge.Adult || age2 == CitizenAge.Elderly)
					{
						num++;
					}
				}
				if (num < 2 && ((Random)(ref random)).NextFloat(1f) < m_CitizenParametersData.m_LookForPartnerRate)
				{
					Random pseudoRandom = citizen.GetPseudoRandom(CitizenPseudoRandom.PartnerType);
					float num2 = ((Random)(ref pseudoRandom)).NextFloat(1f);
					PartnerType partnerType = ((num2 < m_CitizenParametersData.m_LookForPartnerTypeRate.x) ? PartnerType.Same : ((!(num2 < m_CitizenParametersData.m_LookForPartnerTypeRate.y)) ? PartnerType.Other : PartnerType.Any));
					m_Queue.Enqueue(new LookingForPartner
					{
						m_Citizen = val,
						m_PartnerType = partnerType
					});
					citizen.m_State |= CitizenFlags.LookingForPartner;
					m_Citizens[val] = citizen;
					if (m_DebugLookForPartnerQueue.IsCreated)
					{
						m_DebugLookForPartnerQueue.Enqueue(1);
					}
				}
			}
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

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public ComponentTypeHandle<HouseholdMember> __Game_Citizens_HouseholdMember_RW_ComponentTypeHandle;

		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RW_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CommuterHousehold> __Game_Citizens_CommuterHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> __Game_Citizens_TouristHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		public BufferLookup<LookingForPartner> __Game_Citizens_LookingForPartner_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Citizens_HouseholdMember_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdMember>(false);
			__Game_Citizens_Citizen_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(false);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Citizens_CommuterHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CommuterHousehold>(true);
			__Game_Citizens_TouristHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TouristHousehold>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_LookingForPartner_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LookingForPartner>(false);
		}
	}

	public static readonly int kUpdatesPerDay = 4;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_CitizenQuery;

	private EntityQuery m_LookingQuery;

	private EntityQuery m_CitizenParametersQuery;

	private NativeQueue<LookingForPartner> m_Queue;

	[DebugWatchValue]
	private NativeValue<int> m_DebugLookingForPartner;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_DebugLookingForPartner = new NativeValue<int>((Allocator)4);
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Citizen>() };
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_CitizenQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_LookingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<LookingForPartner>() });
		m_CitizenParametersQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenParametersData>() });
		m_Queue = new NativeQueue<LookingForPartner>(AllocatorHandle.op_Implicit((Allocator)4));
		((ComponentSystemBase)this).RequireForUpdate(m_CitizenQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_CitizenParametersQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_Queue.Dispose();
		m_DebugLookingForPartner.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<int> debugLookForPartnerQueue = default(NativeQueue<int>);
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		LookForPartnerJob lookForPartnerJob = new LookForPartnerJob
		{
			m_DebugLookForPartnerQueue = debugLookForPartnerQueue,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMemberType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Commuters = InternalCompilerInterface.GetComponentLookup<CommuterHousehold>(ref __TypeHandle.__Game_Citizens_CommuterHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Tourists = InternalCompilerInterface.GetComponentLookup<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_UpdateFrameIndex = updateFrame,
			m_CitizenParametersData = ((EntityQuery)(ref m_CitizenParametersQuery)).GetSingleton<CitizenParametersData>(),
			m_Queue = m_Queue.AsParallelWriter()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<LookForPartnerJob>(lookForPartnerJob, m_CitizenQuery, ((SystemBase)this).Dependency);
		AddPartnerSeekerJob addPartnerSeekerJob = new AddPartnerSeekerJob
		{
			m_DebugLookingForPartner = m_DebugLookingForPartner,
			m_LookingForPartners = InternalCompilerInterface.GetBufferLookup<LookingForPartner>(ref __TypeHandle.__Game_Citizens_LookingForPartner_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LookingForPartnerEntity = ((EntityQuery)(ref m_LookingQuery)).GetSingletonEntity(),
			m_Queue = m_Queue
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<AddPartnerSeekerJob>(addPartnerSeekerJob, ((SystemBase)this).Dependency);
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
	public LookForPartnerSystem()
	{
	}
}

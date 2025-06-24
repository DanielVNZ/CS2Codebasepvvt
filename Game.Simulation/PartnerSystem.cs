using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Citizens;
using Game.Economy;
using Game.Serialization;
using Game.Triggers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class PartnerSystem : GameSystemBase, IPostDeserialize
{
	[BurstCompile]
	private struct MatchJob : IJob
	{
		public Entity m_PartnerEntity;

		public BufferLookup<LookingForPartner> m_Partners;

		public ComponentLookup<Citizen> m_Citizens;

		public ComponentLookup<Household> m_Households;

		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		public BufferLookup<HouseholdAnimal> m_HouseholdAnimals;

		public ComponentLookup<HouseholdMember> m_HouseholdMembers;

		public ComponentLookup<HouseholdPet> m_HouseholdPets;

		public BufferLookup<Resources> m_Resources;

		public EntityCommandBuffer m_CommandBuffer;

		public NativeQueue<TriggerAction> m_TriggerBuffer;

		private void MoveTogether(Entity citizen1, Entity citizen2)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			Entity household = m_HouseholdMembers[citizen1].m_Household;
			Entity household2 = m_HouseholdMembers[citizen2].m_Household;
			if (!m_Households.HasComponent(household) || !m_Households.HasComponent(household2))
			{
				return;
			}
			Household household3 = m_Households[household];
			Household household4 = m_Households[household2];
			household3.m_Resources = (int)math.clamp((long)household3.m_Resources + (long)household4.m_Resources, -2147483648L, 2147483647L);
			m_Households[household] = household3;
			DynamicBuffer<Resources> resources = m_Resources[household];
			DynamicBuffer<Resources> val = m_Resources[household2];
			for (int i = 0; i < val.Length; i++)
			{
				EconomyUtils.AddResources(val[i].m_Resource, val[i].m_Amount, resources);
			}
			DynamicBuffer<HouseholdCitizen> val2 = m_HouseholdCitizens[household];
			DynamicBuffer<HouseholdCitizen> val3 = m_HouseholdCitizens[household2];
			for (int j = 0; j < val3.Length; j++)
			{
				val2.Add(val3[j]);
				m_HouseholdMembers[val3[j].m_Citizen] = new HouseholdMember
				{
					m_Household = household
				};
			}
			val3.Clear();
			if (m_HouseholdAnimals.HasBuffer(household2))
			{
				DynamicBuffer<HouseholdAnimal> val4 = m_HouseholdAnimals[household2];
				for (int k = 0; k < val4.Length; k++)
				{
					DynamicBuffer<HouseholdAnimal> val5 = default(DynamicBuffer<HouseholdAnimal>);
					(m_HouseholdAnimals.HasBuffer(household) ? m_HouseholdAnimals[household] : ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<HouseholdAnimal>(household)).Add(val4[k]);
					m_HouseholdPets[val4[k].m_HouseholdPet] = new HouseholdPet
					{
						m_Household = household
					};
				}
				val4.Clear();
			}
		}

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<LookingForPartner> val = m_Partners[m_PartnerEntity];
			for (int num = val.Length - 1; num >= 0; num--)
			{
				LookingForPartner lookingForPartner = val[num];
				if (m_Citizens.HasComponent(lookingForPartner.m_Citizen))
				{
					bool flag = (m_Citizens[lookingForPartner.m_Citizen].m_State & CitizenFlags.Male) != 0;
					for (int num2 = num - 1; num2 >= 0; num2--)
					{
						LookingForPartner lookingForPartner2 = val[num2];
						if (m_Citizens.HasComponent(lookingForPartner2.m_Citizen))
						{
							bool flag2 = (m_Citizens[lookingForPartner2.m_Citizen].m_State & CitizenFlags.Male) != 0;
							bool flag3 = flag == flag2;
							if ((lookingForPartner2.m_PartnerType == PartnerType.Any || (flag3 && lookingForPartner2.m_PartnerType == PartnerType.Same) || (!flag3 && lookingForPartner2.m_PartnerType == PartnerType.Other)) && (lookingForPartner.m_PartnerType == PartnerType.Any || (flag3 && lookingForPartner.m_PartnerType == PartnerType.Same) || (!flag3 && lookingForPartner.m_PartnerType == PartnerType.Other)))
							{
								MoveTogether(val[num].m_Citizen, val[num2].m_Citizen);
								lookingForPartner.m_PartnerType = PartnerType.None;
								val[num] = lookingForPartner;
								lookingForPartner2.m_PartnerType = PartnerType.None;
								val[num2] = lookingForPartner2;
								Citizen citizen = m_Citizens[lookingForPartner.m_Citizen];
								citizen.m_State &= ~CitizenFlags.LookingForPartner;
								m_Citizens[lookingForPartner.m_Citizen] = citizen;
								citizen = m_Citizens[lookingForPartner2.m_Citizen];
								citizen.m_State &= ~CitizenFlags.LookingForPartner;
								m_Citizens[lookingForPartner2.m_Citizen] = citizen;
								m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenPartneredUp, Entity.Null, lookingForPartner.m_Citizen, lookingForPartner2.m_Citizen));
								m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenPartneredUp, Entity.Null, lookingForPartner2.m_Citizen, lookingForPartner.m_Citizen));
							}
						}
					}
				}
			}
			for (int num3 = val.Length - 1; num3 >= 0; num3--)
			{
				if (val[num3].m_PartnerType == PartnerType.None || !m_Citizens.HasComponent(val[num3].m_Citizen))
				{
					val[num3] = val[val.Length - 1];
					val.RemoveAt(val.Length - 1);
				}
			}
		}
	}

	private struct TypeHandle
	{
		public BufferLookup<LookingForPartner> __Game_Citizens_LookingForPartner_RW_BufferLookup;

		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RW_ComponentLookup;

		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RW_BufferLookup;

		public BufferLookup<HouseholdAnimal> __Game_Citizens_HouseholdAnimal_RW_BufferLookup;

		public ComponentLookup<Household> __Game_Citizens_Household_RW_ComponentLookup;

		public ComponentLookup<HouseholdPet> __Game_Citizens_HouseholdPet_RW_ComponentLookup;

		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RW_ComponentLookup;

		public BufferLookup<Resources> __Game_Economy_Resources_RW_BufferLookup;

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
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			__Game_Citizens_LookingForPartner_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LookingForPartner>(false);
			__Game_Citizens_Citizen_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(false);
			__Game_Citizens_HouseholdCitizen_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(false);
			__Game_Citizens_HouseholdAnimal_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdAnimal>(false);
			__Game_Citizens_Household_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(false);
			__Game_Citizens_HouseholdPet_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdPet>(false);
			__Game_Citizens_HouseholdMember_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(false);
			__Game_Economy_Resources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(false);
		}
	}

	public static readonly int kUpdatesPerDay = 4;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_PartnerQuery;

	private TriggerSystem m_TriggerSystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PartnerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<LookingForPartner>() });
	}

	public void PostDeserialize(Context context)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_PartnerQuery)).IsEmptyIgnoreFilter)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
			((EntityManager)(ref entityManager)).CreateEntity((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<LookingForPartner>() });
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		MatchJob matchJob = new MatchJob
		{
			m_PartnerEntity = ((EntityQuery)(ref m_PartnerQuery)).GetSingletonEntity(),
			m_Partners = InternalCompilerInterface.GetBufferLookup<LookingForPartner>(ref __TypeHandle.__Game_Citizens_LookingForPartner_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdAnimals = InternalCompilerInterface.GetBufferLookup<HouseholdAnimal>(ref __TypeHandle.__Game_Citizens_HouseholdAnimal_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdPets = InternalCompilerInterface.GetComponentLookup<HouseholdPet>(ref __TypeHandle.__Game_Citizens_HouseholdPet_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMembers = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer(),
			m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer()
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<MatchJob>(matchJob, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public PartnerSystem()
	{
	}
}

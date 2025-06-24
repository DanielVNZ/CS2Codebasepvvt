using System;
using System.Runtime.CompilerServices;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Game.Triggers;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class HouseholdMoveAwaySystem : GameSystemBase
{
	[BurstCompile]
	private struct MoveAwayJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> m_HouseholdCitizenType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_ResourceType;

		public ComponentTypeHandle<HomelessHousehold> m_HomelessHouseholdType;

		public ComponentTypeHandle<MovingAway> m_MovingAwayType;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblems;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> m_OutsideConnectionDatas;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public BufferLookup<Renter> m_RenterBufs;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public NativeList<Entity> m_OutsideConnectionEntities;

		public ParallelWriter<StatisticsEvent> m_StatisticsQueue;

		public ParallelWriter<TriggerAction> m_TriggerBuffer;

		[ReadOnly]
		public EntityArchetype m_RentEventArchetype;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<MovingAway> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MovingAway>(ref m_MovingAwayType);
			NativeArray<HomelessHousehold> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HomelessHousehold>(ref m_HomelessHouseholdType);
			BufferAccessor<HouseholdCitizen> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<HouseholdCitizen>(ref m_HouseholdCitizenType);
			BufferAccessor<Resources> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			DynamicBuffer<OwnedVehicle> val3 = default(DynamicBuffer<OwnedVehicle>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				MovingAway movingAway = nativeArray2[i];
				DynamicBuffer<HouseholdCitizen> val2 = bufferAccessor[i];
				if (!m_PrefabRefs.HasComponent(movingAway.m_Target))
				{
					movingAway.m_Target = Entity.Null;
					OutsideConnectionTransferType ocTransferType = OutsideConnectionTransferType.Train | OutsideConnectionTransferType.Air | OutsideConnectionTransferType.Ship;
					if (m_OwnedVehicles.TryGetBuffer(val, ref val3) && val3.Length > 0)
					{
						ocTransferType = OutsideConnectionTransferType.Road;
					}
					if (!BuildingUtils.GetRandomOutsideConnectionByTransferType(ref m_OutsideConnectionEntities, ref m_OutsideConnectionDatas, ref m_PrefabRefs, random, ocTransferType, out movingAway.m_Target) && m_OutsideConnectionEntities.Length != 0)
					{
						int num = ((Random)(ref random)).NextInt(m_OutsideConnectionEntities.Length);
						movingAway.m_Target = m_OutsideConnectionEntities[num];
					}
					nativeArray2[i] = movingAway;
				}
				for (int j = 0; j < val2.Length; j++)
				{
					Entity citizen = val2[j].m_Citizen;
					if (m_Workers.HasComponent(citizen))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Worker>(unfilteredChunkIndex, citizen);
					}
				}
				if (nativeArray3.Length > 0 && m_RenterBufs.HasBuffer(nativeArray3[i].m_TempHome))
				{
					Entity val4 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex, m_RentEventArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RentersUpdated>(unfilteredChunkIndex, val4, new RentersUpdated(nativeArray3[i].m_TempHome));
					nativeArray3[i] = new HomelessHousehold
					{
						m_TempHome = Entity.Null
					};
				}
				bool flag = ((ArchetypeChunk)(ref chunk)).Has<TouristHousehold>();
				bool flag2 = true;
				if (val2.Length > 0)
				{
					if (flag)
					{
						flag2 = false;
						for (int k = 0; k < val2.Length; k++)
						{
							Entity citizen2 = val2[k].m_Citizen;
							if (m_Citizens.HasComponent(citizen2) && (m_Citizens[citizen2].m_State & CitizenFlags.MovingAwayReachOC) != CitizenFlags.None)
							{
								flag2 = true;
								break;
							}
						}
					}
					else
					{
						for (int l = 0; l < val2.Length; l++)
						{
							Entity citizen3 = val2[l].m_Citizen;
							if (m_Citizens.HasComponent(citizen3))
							{
								Citizen citizen4 = m_Citizens[citizen3];
								if (!CitizenUtils.IsDead(val2[l].m_Citizen, ref m_HealthProblems) && (citizen4.m_State & CitizenFlags.MovingAwayReachOC) == 0)
								{
									flag2 = false;
								}
							}
						}
					}
				}
				if (!flag2)
				{
					continue;
				}
				if (flag)
				{
					DynamicBuffer<Resources> resources = bufferAccessor2[i];
					int resources2 = EconomyUtils.GetResources(Resource.Money, resources);
					m_StatisticsQueue.Enqueue(new StatisticsEvent
					{
						m_Statistic = StatisticType.TouristIncome,
						m_Change = -resources2
					});
				}
				if ((m_Households[val].m_Flags & HouseholdFlags.MovedIn) != HouseholdFlags.None)
				{
					m_StatisticsQueue.Enqueue(new StatisticsEvent
					{
						m_Statistic = StatisticType.CitizensMovedAway,
						m_Change = val2.Length
					});
				}
				if (m_PropertyRenters.HasComponent(val) && m_PropertyRenters[val].m_Property != Entity.Null)
				{
					Enumerator<HouseholdCitizen> enumerator = val2.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							HouseholdCitizen current = enumerator.Current;
							m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenMovedOutOfCity, Entity.Null, current.m_Citizen, m_PropertyRenters[val].m_Property));
						}
					}
					finally
					{
						((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
					}
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val, default(Deleted));
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

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Resources> __Game_Economy_Resources_RO_BufferTypeHandle;

		public ComponentTypeHandle<HomelessHousehold> __Game_Citizens_HomelessHousehold_RW_ComponentTypeHandle;

		public ComponentTypeHandle<MovingAway> __Game_Agents_MovingAway_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> __Game_Prefabs_OutsideConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

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
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<HouseholdCitizen>(true);
			__Game_Economy_Resources_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(true);
			__Game_Citizens_HomelessHousehold_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HomelessHousehold>(false);
			__Game_Agents_MovingAway_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MovingAway>(false);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnectionData>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
		}
	}

	private EntityQuery m_MoveAwayGroup;

	private EntityQuery m_OutsideConnectionQuery;

	private EntityArchetype m_RentEventArchetype;

	private EndFrameBarrier m_EndFrameBarrier;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private TriggerSystem m_TriggerSystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
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
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_MoveAwayGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Household>(),
			ComponentType.ReadOnly<HouseholdCitizen>(),
			ComponentType.ReadWrite<MovingAway>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_OutsideConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Game.Objects.ElectricityOutsideConnection>(),
			ComponentType.Exclude<Game.Objects.WaterPipeOutsideConnection>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_RentEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<RentersUpdated>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_MoveAwayGroup);
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
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		JobHandle deps;
		MoveAwayJob moveAwayJob = new MoveAwayJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenType = InternalCompilerInterface.GetBufferTypeHandle<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHouseholdType = InternalCompilerInterface.GetComponentTypeHandle<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MovingAwayType = InternalCompilerInterface.GetComponentTypeHandle<MovingAway>(ref __TypeHandle.__Game_Agents_MovingAway_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionDatas = InternalCompilerInterface.GetComponentLookup<OutsideConnectionData>(ref __TypeHandle.__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RenterBufs = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_RentEventArchetype = m_RentEventArchetype,
			m_OutsideConnectionEntities = ((EntityQuery)(ref m_OutsideConnectionQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val),
			m_StatisticsQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter(),
			m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer().AsParallelWriter()
		};
		EntityCommandBuffer val2 = m_EndFrameBarrier.CreateCommandBuffer();
		moveAwayJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		MoveAwayJob moveAwayJob2 = moveAwayJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<MoveAwayJob>(moveAwayJob2, m_MoveAwayGroup, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, deps));
		moveAwayJob2.m_OutsideConnectionEntities.Dispose(((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public HouseholdMoveAwaySystem()
	{
	}
}

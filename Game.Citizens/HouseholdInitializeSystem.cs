using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Agents;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Citizens;

[CompilerGenerated]
public class HouseholdInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeHouseholdJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		public BufferTypeHandle<Resources> m_ResourceType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<TouristHousehold> m_TouristHouseholdType;

		[ReadOnly]
		public ComponentTypeHandle<CommuterHousehold> m_CommuterHouseholdType;

		public ComponentTypeHandle<Household> m_HouseholdType;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<HouseholdData> m_HouseholdDatas;

		[ReadOnly]
		public ComponentLookup<DynamicHousehold> m_DynamicHouseholds;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> m_OutsideConnectionDatas;

		[ReadOnly]
		public NativeList<Entity> m_CitizenPrefabs;

		[ReadOnly]
		public NativeList<Entity> m_HouseholdPetPrefabs;

		[ReadOnly]
		public NativeList<ArchetypeData> m_CitizenPrefabArchetypes;

		[ReadOnly]
		public NativeList<ArchetypeData> m_HouseholdPetArchetypes;

		[ReadOnly]
		public PersonalCarSelectData m_PersonalCarSelectData;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		public CityStatisticsSystem.SafeStatisticQueue m_StatisticsQueue;

		public ParallelWriter m_CommandBuffer;

		private void SpawnCitizen(int index, Entity household, ref Random random, CurrentBuilding building, int age, bool tourist, bool commuter)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			int num = ((Random)(ref random)).NextInt(m_CitizenPrefabs.Length);
			Entity prefab = m_CitizenPrefabs[num];
			ArchetypeData archetypeData = m_CitizenPrefabArchetypes[num];
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(index, archetypeData.m_Archetype);
			PrefabRef prefabRef = new PrefabRef
			{
				m_Prefab = prefab
			};
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(index, val, prefabRef);
			HouseholdMember householdMember = new HouseholdMember
			{
				m_Household = household
			};
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<HouseholdMember>(index, val, householdMember);
			CitizenFlags citizenFlags = CitizenFlags.None;
			if (tourist)
			{
				citizenFlags |= CitizenFlags.Tourist;
			}
			if (commuter)
			{
				citizenFlags |= CitizenFlags.Commuter;
			}
			Citizen citizen = new Citizen
			{
				m_BirthDay = (short)age,
				m_State = citizenFlags
			};
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Citizen>(index, val, citizen);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentBuilding>(index, val, building);
		}

		private void SpawnHouseholdPet(int index, Entity household, ref Random random, CurrentBuilding building)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			int num = ((Random)(ref random)).NextInt(m_HouseholdPetPrefabs.Length);
			Entity prefab = m_HouseholdPetPrefabs[num];
			ArchetypeData archetypeData = m_HouseholdPetArchetypes[num];
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(index, archetypeData.m_Archetype);
			PrefabRef prefabRef = new PrefabRef
			{
				m_Prefab = prefab
			};
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(index, val, prefabRef);
			HouseholdPet householdPet = new HouseholdPet
			{
				m_Household = household
			};
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HouseholdPet>(index, val, householdPet);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentBuilding>(index, val, building);
		}

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
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			NativeArray<CurrentBuilding> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingType);
			NativeArray<Household> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Household>(ref m_HouseholdType);
			bool tourist = ((ArchetypeChunk)(ref chunk)).Has<TouristHousehold>(ref m_TouristHouseholdType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<CommuterHousehold>(ref m_CommuterHouseholdType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				if (m_DynamicHouseholds.HasComponent(prefab))
				{
					continue;
				}
				HouseholdData householdData = m_HouseholdDatas[prefab];
				DynamicBuffer<Resources> resources = bufferAccessor[i];
				int num = ((!((ArchetypeChunk)(ref chunk)).Has<TouristHousehold>(ref m_TouristHouseholdType)) ? (((Random)(ref random)).NextInt(householdData.m_InitialWealthRange) - householdData.m_InitialWealthRange / 2 + householdData.m_InitialWealthOffset) : Mathf.RoundToInt((2f + 19f * ((Random)(ref random)).NextFloat() + 19f * ((Random)(ref random)).NextFloat()) * 50f));
				EconomyUtils.AddResources(Resource.Money, num, resources);
				CurrentBuilding building = nativeArray3[i];
				for (int j = 0; j < householdData.m_StudentCount; j++)
				{
					SpawnCitizen(unfilteredChunkIndex, val, ref random, building, 4, tourist, flag);
				}
				for (int k = 0; k < householdData.m_AdultCount; k++)
				{
					SpawnCitizen(unfilteredChunkIndex, val, ref random, building, 1, tourist, flag);
				}
				int num2 = ((Random)(ref random)).NextInt(householdData.m_ChildCount);
				for (int l = 0; l < num2; l++)
				{
					SpawnCitizen(unfilteredChunkIndex, val, ref random, building, 2, tourist, flag);
				}
				for (int m = 0; m < householdData.m_ElderCount; m++)
				{
					SpawnCitizen(unfilteredChunkIndex, val, ref random, building, 3, tourist, flag);
				}
				int num3 = 0;
				if (!flag && ((Random)(ref random)).NextInt(100) < householdData.m_FirstPetProbability)
				{
					do
					{
						SpawnHouseholdPet(unfilteredChunkIndex, val, ref random, building);
					}
					while (++num3 < 4 && ((Random)(ref random)).NextInt(100) < householdData.m_NextPetProbability);
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<HouseholdAnimal>(unfilteredChunkIndex, val);
				}
				int num4 = householdData.m_AdultCount + num2 + householdData.m_ElderCount;
				bool flag2 = false;
				if ((BuildingUtils.GetOutsideConnectionType(building.m_CurrentBuilding, ref m_PrefabRefs, ref m_OutsideConnectionDatas) & OutsideConnectionTransferType.Road) != OutsideConnectionTransferType.None)
				{
					flag2 = true;
				}
				if (flag2)
				{
					Entity val2 = val;
					Entity currentBuilding = nativeArray3[i].m_CurrentBuilding;
					if (m_TransformData.HasComponent(currentBuilding))
					{
						Transform transform = m_TransformData[currentBuilding];
						int num5 = num4;
						int num6 = 1 + num3;
						if (((Random)(ref random)).NextInt(20) == 0)
						{
							num5 += 5;
							num6 += 5;
						}
						else if (((Random)(ref random)).NextInt(10) == 0)
						{
							num6 += 5;
							if (((Random)(ref random)).NextInt(10) == 0)
							{
								num6 += 5;
							}
						}
						Entity val3 = m_PersonalCarSelectData.CreateVehicle(m_CommandBuffer, unfilteredChunkIndex, ref random, num5, num6, avoidTrailers: false, noSlowVehicles: true, transform, currentBuilding, Entity.Null, (PersonalCarFlags)0u, stopped: true);
						if (val3 != Entity.Null)
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(unfilteredChunkIndex, val3, new Owner(val2));
							((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<OwnedVehicle>(unfilteredChunkIndex, val2);
						}
					}
				}
				Household household = nativeArray4[i];
				household.m_Resources = ((Random)(ref random)).NextInt(1000 * num4);
				nativeArray4[i] = household;
				if (((ArchetypeChunk)(ref chunk)).Has<TouristHousehold>(ref m_TouristHouseholdType))
				{
					TouristHousehold touristHousehold = new TouristHousehold
					{
						m_LeavingTime = 0u,
						m_Hotel = Entity.Null
					};
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TouristHousehold>(unfilteredChunkIndex, val, touristHousehold);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LodgingSeeker>(unfilteredChunkIndex, val, default(LodgingSeeker));
					m_StatisticsQueue.Enqueue(new StatisticsEvent
					{
						m_Statistic = StatisticType.TouristIncome,
						m_Change = num
					});
				}
				else if (!((ArchetypeChunk)(ref chunk)).Has<CommuterHousehold>(ref m_CommuterHouseholdType))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PropertySeeker>(unfilteredChunkIndex, val, default(PropertySeeker));
				}
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentBuilding>(unfilteredChunkIndex, val);
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
		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		public ComponentTypeHandle<TouristHousehold> __Game_Citizens_TouristHousehold_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CommuterHousehold> __Game_Citizens_CommuterHousehold_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Household> __Game_Citizens_Household_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdData> __Game_Prefabs_HouseholdData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DynamicHousehold> __Game_Prefabs_DynamicHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> __Game_Prefabs_OutsideConnectionData_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Citizens_TouristHousehold_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TouristHousehold>(false);
			__Game_Citizens_CommuterHousehold_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CommuterHousehold>(true);
			__Game_Citizens_Household_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Household>(false);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Prefabs_HouseholdData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdData>(true);
			__Game_Prefabs_DynamicHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DynamicHousehold>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnectionData>(true);
		}
	}

	private EntityQuery m_CarPrefabGroup;

	private EntityQuery m_CitizenPrefabGroup;

	private EntityQuery m_HouseholdPetPrefabGroup;

	private EntityQuery m_Additions;

	private ModificationBarrier4 m_EndFrameBarrier;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private PersonalCarSelectData m_PersonalCarSelectData;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_PersonalCarSelectData = new PersonalCarSelectData((SystemBase)(object)this);
		m_CarPrefabGroup = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { PersonalCarSelectData.GetEntityQueryDesc() });
		m_CitizenPrefabGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CitizenData>(),
			ComponentType.ReadOnly<ArchetypeData>()
		});
		m_HouseholdPetPrefabGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<HouseholdPetData>(),
			ComponentType.ReadOnly<ArchetypeData>()
		});
		m_Additions = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<Household>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadWrite<HouseholdCitizen>(),
			ComponentType.ReadOnly<CurrentBuilding>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_Additions);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		m_PersonalCarSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_CarPrefabGroup, (Allocator)3, out var jobHandle);
		InitializeHouseholdJob initializeHouseholdJob = new InitializeHouseholdJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TouristHouseholdType = InternalCompilerInterface.GetComponentTypeHandle<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CommuterHouseholdType = InternalCompilerInterface.GetComponentTypeHandle<CommuterHousehold>(ref __TypeHandle.__Game_Citizens_CommuterHousehold_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdType = InternalCompilerInterface.GetComponentTypeHandle<Household>(ref __TypeHandle.__Game_Citizens_Household_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		initializeHouseholdJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		initializeHouseholdJob.m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		initializeHouseholdJob.m_HouseholdDatas = InternalCompilerInterface.GetComponentLookup<HouseholdData>(ref __TypeHandle.__Game_Prefabs_HouseholdData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		initializeHouseholdJob.m_DynamicHouseholds = InternalCompilerInterface.GetComponentLookup<DynamicHousehold>(ref __TypeHandle.__Game_Prefabs_DynamicHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		initializeHouseholdJob.m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		initializeHouseholdJob.m_OutsideConnectionDatas = InternalCompilerInterface.GetComponentLookup<OutsideConnectionData>(ref __TypeHandle.__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		JobHandle val2 = default(JobHandle);
		initializeHouseholdJob.m_CitizenPrefabs = ((EntityQuery)(ref m_CitizenPrefabGroup)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2);
		JobHandle val3 = default(JobHandle);
		initializeHouseholdJob.m_HouseholdPetPrefabs = ((EntityQuery)(ref m_HouseholdPetPrefabGroup)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val3);
		JobHandle val4 = default(JobHandle);
		initializeHouseholdJob.m_CitizenPrefabArchetypes = ((EntityQuery)(ref m_CitizenPrefabGroup)).ToComponentDataListAsync<ArchetypeData>(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val4);
		JobHandle val5 = default(JobHandle);
		initializeHouseholdJob.m_HouseholdPetArchetypes = ((EntityQuery)(ref m_HouseholdPetPrefabGroup)).ToComponentDataListAsync<ArchetypeData>(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val5);
		initializeHouseholdJob.m_StatisticsQueue = m_CityStatisticsSystem.GetSafeStatisticsQueue(out var deps);
		initializeHouseholdJob.m_RandomSeed = RandomSeed.Next();
		initializeHouseholdJob.m_PersonalCarSelectData = m_PersonalCarSelectData;
		JobHandle val6 = JobChunkExtensions.ScheduleParallel<InitializeHouseholdJob>(initializeHouseholdJob, m_Additions, JobUtils.CombineDependencies(((SystemBase)this).Dependency, val2, val3, val4, val5, deps, jobHandle));
		m_PersonalCarSelectData.PostUpdate(val6);
		((EntityCommandBufferSystem)m_EndFrameBarrier).AddJobHandleForProducer(val6);
		m_CityStatisticsSystem.AddWriter(val6);
		((SystemBase)this).Dependency = val6;
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
	public HouseholdInitializeSystem()
	{
	}
}

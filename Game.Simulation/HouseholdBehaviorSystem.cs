using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.Prefabs.Modes;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class HouseholdBehaviorSystem : GameSystemBase
{
	[BurstCompile]
	private struct HouseholdTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<Household> m_HouseholdType;

		public ComponentTypeHandle<HouseholdNeed> m_HouseholdNeedType;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> m_HouseholdCitizenType;

		public BufferTypeHandle<Resources> m_ResourceType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public ComponentTypeHandle<TouristHousehold> m_TouristHouseholdType;

		[ReadOnly]
		public ComponentTypeHandle<CommuterHousehold> m_CommuterHouseholdType;

		[ReadOnly]
		public ComponentTypeHandle<LodgingSeeker> m_LodgingSeekerType;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHouseholds;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public BufferLookup<Renter> m_RenterBufs;

		[ReadOnly]
		public ComponentLookup<PropertySeeker> m_PropertySeekers;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<LodgingProvider> m_LodgingProviders;

		[ReadOnly]
		public ComponentLookup<Population> m_Populations;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenDatas;

		[ReadOnly]
		public ComponentLookup<ConsumptionData> m_ConsumptionDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblems;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		public RandomSeed m_RandomSeed;

		public float m_ResourceDemandPerCitizenMultiplier;

		public EconomyParameterData m_EconomyParameters;

		public ParallelWriter m_CommandBuffer;

		public uint m_UpdateFrameIndex;

		public Entity m_City;

		private bool NeedsCar(int spendableMoney, int familySize, int cars, ref Random random)
		{
			if (spendableMoney > kCarBuyingMinimumMoney)
			{
				return (double)((Random)(ref random)).NextFloat() < (double)((0f - math.log((float)cars + 0.1f)) / 10f) + 0.1;
			}
			return false;
		}

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Household> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Household>(ref m_HouseholdType);
			NativeArray<HouseholdNeed> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdNeed>(ref m_HouseholdNeedType);
			BufferAccessor<HouseholdCitizen> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<HouseholdCitizen>(ref m_HouseholdCitizenType);
			BufferAccessor<Resources> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			NativeArray<TouristHousehold> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TouristHousehold>(ref m_TouristHouseholdType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			int population = m_Populations[m_City].m_Population;
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				Household household = nativeArray2[i];
				DynamicBuffer<HouseholdCitizen> citizens = bufferAccessor[i];
				if (citizens.Length == 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val, default(Deleted));
					continue;
				}
				bool flag = true;
				int num = 0;
				for (int j = 0; j < citizens.Length; j++)
				{
					num += m_CitizenDatas[citizens[j].m_Citizen].Happiness;
					if (m_CitizenDatas[citizens[j].m_Citizen].GetAge() >= CitizenAge.Adult)
					{
						flag = false;
					}
				}
				num /= citizens.Length;
				bool flag2 = (float)((Random)(ref random)).NextInt(10000) < -3f * math.log((float)(-(100 - num) + 70)) + 9f;
				bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<HomelessHousehold>();
				DynamicBuffer<Resources> resources = bufferAccessor2[i];
				int householdTotalWealth = EconomyUtils.GetHouseholdTotalWealth(household, resources);
				int householdIncome = EconomyUtils.GetHouseholdIncome(citizens, ref m_Workers, ref m_CitizenDatas, ref m_HealthProblems, ref m_EconomyParameters, m_TaxRates);
				if (flag || flag2 || householdTotalWealth + householdIncome < -1000)
				{
					CitizenUtils.HouseholdMoveAway(m_CommandBuffer, unfilteredChunkIndex, val);
					continue;
				}
				if (!flag3)
				{
					PropertyRenter propertyRenter = (m_PropertyRenters.HasComponent(val) ? m_PropertyRenters[val] : default(PropertyRenter));
					UpdateHouseholdNeed(chunk, unfilteredChunkIndex, nativeArray3, i, ref household, householdTotalWealth, citizens, nativeArray4, val, propertyRenter, resources, population, ref random);
				}
				else
				{
					EconomyUtils.AddResources(Resource.Money, -1, resources);
				}
				if (!((ArchetypeChunk)(ref chunk)).Has<TouristHousehold>(ref m_TouristHouseholdType) && !((ArchetypeChunk)(ref chunk)).Has<CommuterHousehold>(ref m_CommuterHouseholdType) && !m_PropertySeekers.IsComponentEnabled(nativeArray[i]))
				{
					Entity householdHomeBuilding = BuildingUtils.GetHouseholdHomeBuilding(val, ref m_PropertyRenters, ref m_HomelessHouseholds);
					if (householdHomeBuilding == Entity.Null || !m_RenterBufs.HasBuffer(householdHomeBuilding))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<PropertySeeker>(unfilteredChunkIndex, nativeArray[i], true);
					}
					else
					{
						int num2 = math.clamp(Mathf.RoundToInt(0.06f * (float)population), 64, 1024);
						if (flag3)
						{
							num2 /= 10;
						}
						if (((Random)(ref random)).NextInt(num2) == 0)
						{
							((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<PropertySeeker>(unfilteredChunkIndex, nativeArray[i], true);
						}
					}
				}
				nativeArray2[i] = household;
			}
		}

		private void UpdateHouseholdNeed(ArchetypeChunk chunk, int unfilteredChunkIndex, NativeArray<HouseholdNeed> householdNeeds, int i, ref Household household, int totalWealth, DynamicBuffer<HouseholdCitizen> citizens, NativeArray<TouristHousehold> touristHouseholds, Entity entity, PropertyRenter propertyRenter, DynamicBuffer<Resources> resources, int population, ref Random random)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			HouseholdNeed householdNeed = householdNeeds[i];
			if (household.m_Resources > 0)
			{
				float num = GetConsumptionMultiplier(m_EconomyParameters.m_ResourceConsumptionMultiplier, totalWealth) * m_EconomyParameters.m_ResourceConsumptionPerCitizen * (float)citizens.Length;
				if (((ArchetypeChunk)(ref chunk)).Has<TouristHousehold>(ref m_TouristHouseholdType))
				{
					num *= m_EconomyParameters.m_TouristConsumptionMultiplier;
					if (!((ArchetypeChunk)(ref chunk)).Has<LodgingSeeker>(ref m_LodgingSeekerType))
					{
						TouristHousehold touristHousehold = touristHouseholds[i];
						if (((Entity)(ref touristHousehold.m_Hotel)).Equals(Entity.Null))
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LodgingSeeker>(unfilteredChunkIndex, entity, default(LodgingSeeker));
						}
						else if (!m_LodgingProviders.HasComponent(touristHousehold.m_Hotel))
						{
							touristHousehold.m_Hotel = Entity.Null;
							touristHouseholds[i] = touristHousehold;
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LodgingSeeker>(unfilteredChunkIndex, entity, default(LodgingSeeker));
						}
					}
				}
				int num2 = MathUtils.RoundToIntRandom(ref random, num);
				household.m_ConsumptionPerDay = (short)math.min(32767, kUpdatesPerDay * num2);
				household.m_Resources = math.max(household.m_Resources - num2, 0);
				return;
			}
			household.m_Resources = 0;
			household.m_ConsumptionPerDay = 0;
			if (householdNeed.m_Resource != Resource.NoResource)
			{
				return;
			}
			int householdSpendableMoney = EconomyUtils.GetHouseholdSpendableMoney(household, resources, ref m_RenterBufs, ref m_ConsumptionDatas, ref m_PrefabRefs, propertyRenter);
			int num3 = 0;
			if (m_OwnedVehicles.HasBuffer(entity))
			{
				num3 = m_OwnedVehicles[entity].Length;
			}
			int num4 = math.min(kMaxShoppingPossibility, Mathf.RoundToInt(200f / math.max(1f, math.sqrt(m_EconomyParameters.m_TrafficReduction * (float)population))));
			if (((Random)(ref random)).NextInt(100) >= num4)
			{
				return;
			}
			ResourceIterator iterator = ResourceIterator.GetIterator();
			int num5 = 0;
			while (iterator.Next())
			{
				num5 += GetResourceShopWeightWithAge(householdSpendableMoney, iterator.resource, m_ResourcePrefabs, ref m_ResourceDatas, num3, leisureIncluded: false, citizens, ref m_CitizenDatas);
			}
			int num6 = ((Random)(ref random)).NextInt(num5);
			iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				int resourceShopWeightWithAge = GetResourceShopWeightWithAge(householdSpendableMoney, iterator.resource, m_ResourcePrefabs, ref m_ResourceDatas, num3, leisureIncluded: false, citizens, ref m_CitizenDatas);
				num5 -= resourceShopWeightWithAge;
				if (resourceShopWeightWithAge > 0 && num5 <= num6)
				{
					if (iterator.resource == Resource.Vehicles && NeedsCar(householdSpendableMoney, citizens.Length, num3, ref random))
					{
						householdNeed.m_Resource = Resource.Vehicles;
						householdNeed.m_Amount = kCarAmount;
						householdNeeds[i] = householdNeed;
					}
					else
					{
						householdNeed.m_Resource = iterator.resource;
						float marketPrice = EconomyUtils.GetMarketPrice(m_ResourceDatas[m_ResourcePrefabs[iterator.resource]]);
						householdNeed.m_Amount = math.clamp((int)((float)householdSpendableMoney / marketPrice), 0, kMaxHouseholdNeedAmount);
						householdNeed.m_Amount = (int)((float)householdNeed.m_Amount * m_ResourceDemandPerCitizenMultiplier);
						householdNeeds[i] = householdNeed;
					}
					break;
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

		public ComponentTypeHandle<Household> __Game_Citizens_Household_RW_ComponentTypeHandle;

		public ComponentTypeHandle<HouseholdNeed> __Game_Citizens_HouseholdNeed_RW_ComponentTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle;

		public ComponentTypeHandle<TouristHousehold> __Game_Citizens_TouristHousehold_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CommuterHousehold> __Game_Citizens_CommuterHousehold_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<LodgingSeeker> __Game_Citizens_LodgingSeeker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertySeeker> __Game_Agents_PropertySeeker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LodgingProvider> __Game_Companies_LodgingProvider_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Population> __Game_City_Population_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ConsumptionData> __Game_Prefabs_ConsumptionData_RO_ComponentLookup;

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
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_Household_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Household>(false);
			__Game_Citizens_HouseholdNeed_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdNeed>(false);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<HouseholdCitizen>(true);
			__Game_Citizens_TouristHousehold_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TouristHousehold>(false);
			__Game_Citizens_CommuterHousehold_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CommuterHousehold>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Citizens_LodgingSeeker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LodgingSeeker>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Agents_PropertySeeker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertySeeker>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Companies_LodgingProvider_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LodgingProvider>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_City_Population_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Prefabs_ConsumptionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ConsumptionData>(true);
		}
	}

	public static readonly int kCarAmount = 50;

	public static readonly int kUpdatesPerDay = 256;

	public static readonly int kMaxShoppingPossibility = 80;

	public static readonly int kMaxHouseholdNeedAmount = 2000;

	public static readonly int kCarBuyingMinimumMoney = 10000;

	public static readonly int KMinimumShoppingAmount = 50;

	private EntityQuery m_HouseholdGroup;

	private EntityQuery m_EconomyParameterGroup;

	private EntityQuery m_GameModeSettingQuery;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private ResourceSystem m_ResourceSystem;

	private TaxSystem m_TaxSystem;

	private CitySystem m_CitySystem;

	private float m_ResourceDemandPerCitizenMultiplier;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		if (((EntityQuery)(ref m_GameModeSettingQuery)).IsEmptyIgnoreFilter)
		{
			m_ResourceDemandPerCitizenMultiplier = 1f;
			return;
		}
		ModeSettingData singleton = ((EntityQuery)(ref m_GameModeSettingQuery)).GetSingleton<ModeSettingData>();
		if (singleton.m_Enable)
		{
			m_ResourceDemandPerCitizenMultiplier = singleton.m_ResourceDemandPerCitizenMultiplier;
		}
		else
		{
			m_ResourceDemandPerCitizenMultiplier = 1f;
		}
	}

	public static float GetLastCommutePerCitizen(DynamicBuffer<HouseholdCitizen> householdCitizens, ComponentLookup<Worker> workers)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < householdCitizens.Length; i++)
		{
			Entity citizen = householdCitizens[i].m_Citizen;
			if (workers.HasComponent(citizen))
			{
				num2 += workers[citizen].m_LastCommuteTime;
			}
			num += 1f;
		}
		return num2 / num;
	}

	public static float GetConsumptionMultiplier(float2 parameter, int householdWealth)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return parameter.x + parameter.y * math.smoothstep(0f, 1f, (float)(math.max(0, householdWealth) + 1000) / 6000f);
	}

	public static bool GetFreeCar(Entity household, BufferLookup<OwnedVehicle> ownedVehicles, ComponentLookup<Game.Vehicles.PersonalCar> personalCars, ref Entity car)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (ownedVehicles.HasBuffer(household))
		{
			DynamicBuffer<OwnedVehicle> val = ownedVehicles[household];
			for (int i = 0; i < val.Length; i++)
			{
				car = val[i].m_Vehicle;
				if (personalCars.HasComponent(car))
				{
					Game.Vehicles.PersonalCar personalCar = personalCars[car];
					if (((Entity)(ref personalCar.m_Keeper)).Equals(Entity.Null))
					{
						return true;
					}
				}
			}
		}
		car = Entity.Null;
		return false;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_EconomyParameterGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_HouseholdGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[9]
		{
			ComponentType.ReadWrite<Household>(),
			ComponentType.ReadWrite<HouseholdNeed>(),
			ComponentType.ReadOnly<HouseholdCitizen>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<TouristHousehold>(),
			ComponentType.Exclude<MovingAway>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_GameModeSettingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ModeSettingData>() });
		m_ResourceDemandPerCitizenMultiplier = 1f;
		((ComponentSystemBase)this).RequireForUpdate(m_HouseholdGroup);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterGroup);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
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
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrameWithInterval = SimulationUtils.GetUpdateFrameWithInterval(m_SimulationSystem.frameIndex, (uint)GetUpdateInterval(SystemUpdatePhase.GameSimulation), 16);
		HouseholdTickJob householdTickJob = new HouseholdTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdType = InternalCompilerInterface.GetComponentTypeHandle<Household>(ref __TypeHandle.__Game_Citizens_Household_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdNeedType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdNeed>(ref __TypeHandle.__Game_Citizens_HouseholdNeed_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenType = InternalCompilerInterface.GetBufferTypeHandle<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TouristHouseholdType = InternalCompilerInterface.GetComponentTypeHandle<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CommuterHouseholdType = InternalCompilerInterface.GetComponentTypeHandle<CommuterHousehold>(ref __TypeHandle.__Game_Citizens_CommuterHousehold_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LodgingSeekerType = InternalCompilerInterface.GetComponentTypeHandle<LodgingSeeker>(ref __TypeHandle.__Game_Citizens_LodgingSeeker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RenterBufs = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterGroup)).GetSingleton<EconomyParameterData>(),
			m_HomelessHouseholds = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertySeekers = InternalCompilerInterface.GetComponentLookup<PropertySeeker>(ref __TypeHandle.__Game_Agents_PropertySeeker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LodgingProviders = InternalCompilerInterface.GetComponentLookup<LodgingProvider>(ref __TypeHandle.__Game_Companies_LodgingProvider_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenDatas = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Populations = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConsumptionDatas = InternalCompilerInterface.GetComponentLookup<ConsumptionData>(ref __TypeHandle.__Game_Prefabs_ConsumptionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_TaxRates = m_TaxSystem.GetTaxRates(),
			m_RandomSeed = RandomSeed.Next(),
			m_ResourceDemandPerCitizenMultiplier = m_ResourceDemandPerCitizenMultiplier
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		householdTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		householdTickJob.m_UpdateFrameIndex = updateFrameWithInterval;
		householdTickJob.m_City = m_CitySystem.City;
		HouseholdTickJob householdTickJob2 = householdTickJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<HouseholdTickJob>(householdTickJob2, m_HouseholdGroup, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
		m_TaxSystem.AddReader(((SystemBase)this).Dependency);
	}

	public static int GetAgeWeight(ResourceData resourceData, DynamicBuffer<HouseholdCitizen> citizens, ref ComponentLookup<Citizen> citizenDatas)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = 0; i < citizens.Length; i++)
		{
			Entity citizen = citizens[i].m_Citizen;
			num = citizenDatas[citizen].GetAge() switch
			{
				CitizenAge.Child => num + resourceData.m_ChildWeight, 
				CitizenAge.Teen => num + resourceData.m_TeenWeight, 
				CitizenAge.Elderly => num + resourceData.m_ElderlyWeight, 
				_ => num + resourceData.m_AdultWeight, 
			};
		}
		return num;
	}

	public static int GetResourceShopWeightWithAge(int wealth, Resource resource, ResourcePrefabs resourcePrefabs, ref ComponentLookup<ResourceData> resourceDatas, int carCount, bool leisureIncluded, DynamicBuffer<HouseholdCitizen> citizens, ref ComponentLookup<Citizen> citizenDatas)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		ResourceData resourceData = resourceDatas[resourcePrefabs[resource]];
		return GetResourceShopWeightWithAge(wealth, resourceData, carCount, leisureIncluded, citizens, ref citizenDatas);
	}

	public static int GetResourceShopWeightWithAge(int wealth, ResourceData resourceData, int carCount, bool leisureIncluded, DynamicBuffer<HouseholdCitizen> citizens, ref ComponentLookup<Citizen> citizenDatas)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		float num = ((leisureIncluded || !resourceData.m_IsLeisure) ? resourceData.m_BaseConsumption : 0f);
		num += (float)(carCount * resourceData.m_CarConsumption);
		float num2 = ((leisureIncluded || !resourceData.m_IsLeisure) ? resourceData.m_WealthModifier : 0f);
		float num3 = GetAgeWeight(resourceData, citizens, ref citizenDatas);
		return Mathf.RoundToInt(100f * num3 * num * math.smoothstep(num2, 1f, math.max(0.01f, ((float)wealth + 5000f) / 10000f)));
	}

	public static int GetWeight(int wealth, Resource resource, ResourcePrefabs resourcePrefabs, ref ComponentLookup<ResourceData> resourceDatas, int carCount, bool leisureIncluded)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		ResourceData resourceData = resourceDatas[resourcePrefabs[resource]];
		return GetWeight(wealth, resourceData, carCount, leisureIncluded);
	}

	public static int GetWeight(int wealth, ResourceData resourceData, int carCount, bool leisureIncluded)
	{
		float num = ((leisureIncluded || !resourceData.m_IsLeisure) ? resourceData.m_BaseConsumption : 0f) + (float)(carCount * resourceData.m_CarConsumption);
		float num2 = ((leisureIncluded || !resourceData.m_IsLeisure) ? resourceData.m_WealthModifier : 0f);
		return Mathf.RoundToInt(num * math.smoothstep(num2, 1f, math.clamp(((float)wealth + 5000f) / 10000f, 0.1f, 0.9f)));
	}

	public static int GetHighestEducation(DynamicBuffer<HouseholdCitizen> citizenBuffer, ref ComponentLookup<Citizen> citizens)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = 0; i < citizenBuffer.Length; i++)
		{
			Entity citizen = citizenBuffer[i].m_Citizen;
			if (citizens.HasComponent(citizen))
			{
				Citizen citizen2 = citizens[citizen];
				CitizenAge age = citizen2.GetAge();
				if (age == CitizenAge.Teen || age == CitizenAge.Adult)
				{
					num = math.max(num, citizen2.GetEducationLevel());
				}
			}
		}
		return num;
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
	public HouseholdBehaviorSystem()
	{
	}
}

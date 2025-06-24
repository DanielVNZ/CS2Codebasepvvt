using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Game.Vehicles;
using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ResourceBuyerSystem : GameSystemBase
{
	[Flags]
	private enum SaleFlags : byte
	{
		None = 0,
		CommercialSeller = 1,
		ImportFromOC = 2,
		Virtual = 4
	}

	private struct SalesEvent
	{
		public SaleFlags m_Flags;

		public Entity m_Buyer;

		public Entity m_Seller;

		public Resource m_Resource;

		public int m_Amount;

		public float m_Distance;
	}

	[BurstCompile]
	private struct BuyJob : IJob
	{
		public NativeQueue<SalesEvent> m_SalesQueue;

		public BufferLookup<Resources> m_Resources;

		public ComponentLookup<ServiceAvailable> m_Services;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Household> m_Households;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<BuyingCompany> m_BuyingCompanies;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformDatas;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<ServiceCompanyData> m_ServiceCompanies;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> m_HouseholdAnimals;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<Game.Companies.StorageCompany> m_Storages;

		public BufferLookup<TradeCost> m_TradeCosts;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public PersonalCarSelectData m_PersonalCarSelectData;

		[ReadOnly]
		public ComponentLookup<Population> m_PopulationData;

		public Entity m_PopulationEntity;

		public RandomSeed m_RandomSeed;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_0655: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(0);
			_ = m_PopulationData[m_PopulationEntity];
			SalesEvent salesEvent = default(SalesEvent);
			while (m_SalesQueue.TryDequeue(ref salesEvent))
			{
				if (!m_Resources.HasBuffer(salesEvent.m_Buyer) || salesEvent.m_Amount == 0)
				{
					continue;
				}
				bool flag = (salesEvent.m_Flags & SaleFlags.CommercialSeller) != 0;
				float num = (flag ? EconomyUtils.GetMarketPrice(salesEvent.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas) : EconomyUtils.GetIndustrialPrice(salesEvent.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas)) * (float)salesEvent.m_Amount;
				if (m_TradeCosts.HasBuffer(salesEvent.m_Seller))
				{
					DynamicBuffer<TradeCost> costs = m_TradeCosts[salesEvent.m_Seller];
					TradeCost tradeCost = EconomyUtils.GetTradeCost(salesEvent.m_Resource, costs);
					num += (float)salesEvent.m_Amount * tradeCost.m_BuyCost;
					float weight = EconomyUtils.GetWeight(salesEvent.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas);
					Assert.IsTrue(salesEvent.m_Amount != -1);
					float num2 = (float)EconomyUtils.GetTransportCost(salesEvent.m_Distance, salesEvent.m_Resource, salesEvent.m_Amount, weight) / (1f + (float)salesEvent.m_Amount);
					TradeCost newcost = default(TradeCost);
					if (m_TradeCosts.HasBuffer(salesEvent.m_Buyer))
					{
						newcost = EconomyUtils.GetTradeCost(salesEvent.m_Resource, m_TradeCosts[salesEvent.m_Buyer]);
					}
					if (!m_OutsideConnections.HasComponent(salesEvent.m_Seller) && !flag)
					{
						tradeCost.m_SellCost = math.lerp(tradeCost.m_SellCost, num2 + newcost.m_SellCost, 0.5f);
						EconomyUtils.SetTradeCost(salesEvent.m_Resource, tradeCost, costs, keepLastTime: true);
					}
					if (m_TradeCosts.HasBuffer(salesEvent.m_Buyer) && !m_OutsideConnections.HasComponent(salesEvent.m_Buyer))
					{
						newcost.m_BuyCost = math.lerp(newcost.m_BuyCost, num2 + tradeCost.m_BuyCost, 0.5f);
						EconomyUtils.SetTradeCost(salesEvent.m_Resource, newcost, m_TradeCosts[salesEvent.m_Buyer], keepLastTime: true);
					}
				}
				if (m_Resources.HasBuffer(salesEvent.m_Seller) && EconomyUtils.GetResources(salesEvent.m_Resource, m_Resources[salesEvent.m_Seller]) <= 0)
				{
					continue;
				}
				if (flag && m_Services.HasComponent(salesEvent.m_Seller) && m_PropertyRenters.HasComponent(salesEvent.m_Seller))
				{
					Entity prefab = m_Prefabs[salesEvent.m_Seller].m_Prefab;
					ServiceAvailable serviceAvailable = m_Services[salesEvent.m_Seller];
					ServiceCompanyData serviceCompanyData = m_ServiceCompanies[prefab];
					num *= EconomyUtils.GetServicePriceMultiplier(serviceAvailable.m_ServiceAvailable, serviceCompanyData.m_MaxService);
					serviceAvailable.m_ServiceAvailable = math.max(0, Mathf.RoundToInt((float)(serviceAvailable.m_ServiceAvailable - salesEvent.m_Amount)));
					if (serviceAvailable.m_MeanPriority > 0f)
					{
						serviceAvailable.m_MeanPriority = math.min(1f, math.lerp(serviceAvailable.m_MeanPriority, (float)serviceAvailable.m_ServiceAvailable / (float)serviceCompanyData.m_MaxService, 0.1f));
					}
					else
					{
						serviceAvailable.m_MeanPriority = math.min(1f, (float)serviceAvailable.m_ServiceAvailable / (float)serviceCompanyData.m_MaxService);
					}
					m_Services[salesEvent.m_Seller] = serviceAvailable;
				}
				if (m_Resources.HasBuffer(salesEvent.m_Seller) && !m_Storages.HasComponent(salesEvent.m_Seller))
				{
					DynamicBuffer<Resources> resources = m_Resources[salesEvent.m_Seller];
					int resources2 = EconomyUtils.GetResources(salesEvent.m_Resource, resources);
					EconomyUtils.AddResources(salesEvent.m_Resource, -math.min(resources2, Mathf.RoundToInt((float)salesEvent.m_Amount)), resources);
				}
				EconomyUtils.AddResources(Resource.Money, -Mathf.RoundToInt(num), m_Resources[salesEvent.m_Buyer]);
				if (m_Households.HasComponent(salesEvent.m_Buyer))
				{
					Household household = m_Households[salesEvent.m_Buyer];
					household.m_Resources = (int)math.clamp((long)((float)household.m_Resources + num), -2147483648L, 2147483647L);
					m_Households[salesEvent.m_Buyer] = household;
				}
				else if (m_BuyingCompanies.HasComponent(salesEvent.m_Buyer))
				{
					BuyingCompany buyingCompany = m_BuyingCompanies[salesEvent.m_Buyer];
					buyingCompany.m_LastTradePartner = salesEvent.m_Seller;
					m_BuyingCompanies[salesEvent.m_Buyer] = buyingCompany;
					if ((salesEvent.m_Flags & SaleFlags.Virtual) != SaleFlags.None)
					{
						EconomyUtils.AddResources(salesEvent.m_Resource, salesEvent.m_Amount, m_Resources[salesEvent.m_Buyer]);
					}
				}
				if (!m_Storages.HasComponent(salesEvent.m_Seller) && m_PropertyRenters.HasComponent(salesEvent.m_Seller))
				{
					DynamicBuffer<Resources> resources3 = m_Resources[salesEvent.m_Seller];
					EconomyUtils.AddResources(Resource.Money, Mathf.RoundToInt(num), resources3);
				}
				if (salesEvent.m_Resource != Resource.Vehicles || salesEvent.m_Amount != HouseholdBehaviorSystem.kCarAmount || !m_PropertyRenters.HasComponent(salesEvent.m_Seller))
				{
					continue;
				}
				Entity property = m_PropertyRenters[salesEvent.m_Seller].m_Property;
				if (!m_TransformDatas.HasComponent(property) || !m_HouseholdCitizens.HasBuffer(salesEvent.m_Buyer))
				{
					continue;
				}
				Entity val = salesEvent.m_Buyer;
				Transform transform = m_TransformDatas[property];
				int length = m_HouseholdCitizens[val].Length;
				int num3 = (m_HouseholdAnimals.HasBuffer(val) ? m_HouseholdAnimals[val].Length : 0);
				int passengerAmount;
				int num4;
				if (m_OwnedVehicles.HasBuffer(val) && m_OwnedVehicles[val].Length >= 1)
				{
					passengerAmount = ((Random)(ref random)).NextInt(1, 1 + length);
					num4 = ((Random)(ref random)).NextInt(1, 2 + num3);
				}
				else
				{
					passengerAmount = length;
					num4 = 1 + num3;
				}
				if (((Random)(ref random)).NextInt(20) == 0)
				{
					num4 += 5;
				}
				Entity val2 = m_PersonalCarSelectData.CreateVehicle(m_CommandBuffer, ref random, passengerAmount, num4, avoidTrailers: true, noSlowVehicles: false, transform, property, Entity.Null, (PersonalCarFlags)0u, stopped: true);
				if (val2 != Entity.Null)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val2, new Owner(val));
					if (!m_OwnedVehicles.HasBuffer(val))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<OwnedVehicle>(val);
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct HandleBuyersJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<ResourceBuyer> m_BuyerType;

		[ReadOnly]
		public ComponentTypeHandle<ResourceBought> m_BoughtType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public BufferTypeHandle<TripNeeded> m_TripType;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public ComponentTypeHandle<CreatureData> m_CreatureDataType;

		[ReadOnly]
		public ComponentTypeHandle<ResidentData> m_ResidentDataType;

		[ReadOnly]
		public ComponentTypeHandle<AttendingMeeting> m_AttendingMeetingType;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformation;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_Properties;

		[ReadOnly]
		public ComponentLookup<ServiceAvailable> m_ServiceAvailables;

		[ReadOnly]
		public ComponentLookup<CarKeeper> m_CarKeepers;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PersonalCar> m_PersonalCarData;

		[ReadOnly]
		public ComponentLookup<Target> m_Targets;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildings;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMembers;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> m_TouristHouseholds;

		[ReadOnly]
		public ComponentLookup<CommuterHousehold> m_CommuterHouseholds;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTrucks;

		[ReadOnly]
		public ComponentLookup<Game.Companies.StorageCompany> m_StorageCompanies;

		[ReadOnly]
		public BufferLookup<Resources> m_Resources;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<GuestVehicle> m_GuestVehicles;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CoordinatedMeeting> m_CoordinatedMeetings;

		[ReadOnly]
		public BufferLookup<HaveCoordinatedMeetingData> m_HaveCoordinatedMeetingDatas;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<HumanData> m_PrefabHumanData;

		[ReadOnly]
		public ComponentLookup<Population> m_Populations;

		[ReadOnly]
		public float m_TimeOfDay;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public ComponentTypeSet m_PathfindTypes;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_HumanChunks;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public EconomyParameterData m_EconomyParameterData;

		public Entity m_City;

		public ParallelWriter<SalesEvent> m_SalesQueue;

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
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_064a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0660: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ResourceBuyer> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ResourceBuyer>(ref m_BuyerType);
			NativeArray<ResourceBought> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ResourceBought>(ref m_BoughtType);
			BufferAccessor<TripNeeded> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TripNeeded>(ref m_TripType);
			NativeArray<Citizen> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Citizen>(ref m_CitizenType);
			NativeArray<AttendingMeeting> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AttendingMeeting>(ref m_AttendingMeetingType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				Entity val = nativeArray[i];
				ResourceBought resourceBought = nativeArray3[i];
				if (m_PrefabRefData.HasComponent(resourceBought.m_Payer) && m_PrefabRefData.HasComponent(resourceBought.m_Seller))
				{
					SalesEvent salesEvent = new SalesEvent
					{
						m_Amount = resourceBought.m_Amount,
						m_Buyer = resourceBought.m_Payer,
						m_Seller = resourceBought.m_Seller,
						m_Resource = resourceBought.m_Resource,
						m_Flags = SaleFlags.None,
						m_Distance = resourceBought.m_Distance
					};
					m_SalesQueue.Enqueue(salesEvent);
				}
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<ResourceBought>(unfilteredChunkIndex, val);
			}
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				ResourceBuyer resourceBuyer = nativeArray2[j];
				Entity val2 = nativeArray[j];
				DynamicBuffer<TripNeeded> val3 = bufferAccessor[j];
				bool flag = false;
				Entity val4 = m_ResourcePrefabs[resourceBuyer.m_ResourceNeeded];
				if (m_ResourceDatas.HasComponent(val4))
				{
					flag = EconomyUtils.GetWeight(resourceBuyer.m_ResourceNeeded, m_ResourcePrefabs, ref m_ResourceDatas) == 0f;
				}
				if (m_PathInformation.HasComponent(val2))
				{
					PathInformation pathInformation = m_PathInformation[val2];
					if ((pathInformation.m_State & PathFlags.Pending) != 0)
					{
						continue;
					}
					Entity destination = pathInformation.m_Destination;
					if (m_Properties.HasComponent(destination) || m_OutsideConnections.HasComponent(destination))
					{
						DynamicBuffer<Resources> resources = m_Resources[destination];
						int num = EconomyUtils.GetResources(resourceBuyer.m_ResourceNeeded, resources);
						if (m_StorageCompanies.HasComponent(destination))
						{
							int allBuyingResourcesTrucks = VehicleUtils.GetAllBuyingResourcesTrucks(destination, resourceBuyer.m_ResourceNeeded, ref m_DeliveryTrucks, ref m_GuestVehicles, ref m_LayoutElements);
							num -= allBuyingResourcesTrucks;
						}
						num = math.max(num, 0);
						if (num <= resourceBuyer.m_AmountNeeded / 2)
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val2, ref m_PathfindTypes);
							continue;
						}
						resourceBuyer.m_AmountNeeded = math.min(resourceBuyer.m_AmountNeeded, num);
						bool num2 = m_ServiceAvailables.HasComponent(destination);
						bool flag2 = m_StorageCompanies.HasComponent(destination);
						SaleFlags saleFlags = (num2 ? SaleFlags.CommercialSeller : SaleFlags.None);
						if (flag)
						{
							saleFlags |= SaleFlags.Virtual;
						}
						if (m_OutsideConnections.HasComponent(destination))
						{
							saleFlags |= SaleFlags.ImportFromOC;
						}
						SalesEvent salesEvent2 = new SalesEvent
						{
							m_Amount = resourceBuyer.m_AmountNeeded,
							m_Buyer = resourceBuyer.m_Payer,
							m_Seller = destination,
							m_Resource = resourceBuyer.m_ResourceNeeded,
							m_Flags = saleFlags,
							m_Distance = pathInformation.m_Distance
						};
						m_SalesQueue.Enqueue(salesEvent2);
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val2, ref m_PathfindTypes);
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<ResourceBuyer>(unfilteredChunkIndex, val2);
						int population = m_Populations[m_City].m_Population;
						bool flag3 = ((Random)(ref random)).NextInt(100) < 100 - Mathf.RoundToInt(100f / math.max(1f, math.sqrt(m_EconomyParameterData.m_TrafficReduction * (float)population * 0.1f)));
						if (!flag && !flag3)
						{
							val3.Add(new TripNeeded
							{
								m_TargetAgent = destination,
								m_Purpose = ((!flag2) ? Purpose.Shopping : Purpose.CompanyShopping),
								m_Data = resourceBuyer.m_AmountNeeded,
								m_Resource = resourceBuyer.m_ResourceNeeded
							});
							if (!m_Targets.HasComponent(nativeArray[j]))
							{
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Target>(unfilteredChunkIndex, val2, new Target
								{
									m_Target = destination
								});
							}
						}
						continue;
					}
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<ResourceBuyer>(unfilteredChunkIndex, val2);
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val2, ref m_PathfindTypes);
					if (nativeArray5.IsCreated)
					{
						AttendingMeeting attendingMeeting = nativeArray5[j];
						Entity prefab = m_PrefabRefData[attendingMeeting.m_Meeting].m_Prefab;
						CoordinatedMeeting coordinatedMeeting = m_CoordinatedMeetings[attendingMeeting.m_Meeting];
						if (m_HaveCoordinatedMeetingDatas[prefab][coordinatedMeeting.m_Phase].m_TravelPurpose.m_Purpose == Purpose.Shopping)
						{
							coordinatedMeeting.m_Status = MeetingStatus.Done;
							m_CoordinatedMeetings[attendingMeeting.m_Meeting] = coordinatedMeeting;
						}
					}
				}
				else if ((!m_HouseholdMembers.HasComponent(val2) || (!m_TouristHouseholds.HasComponent(m_HouseholdMembers[val2].m_Household) && !m_CommuterHouseholds.HasComponent(m_HouseholdMembers[val2].m_Household))) && m_CurrentBuildings.HasComponent(val2) && m_OutsideConnections.HasComponent(m_CurrentBuildings[val2].m_CurrentBuilding) && !nativeArray5.IsCreated)
				{
					SaleFlags flags = SaleFlags.ImportFromOC;
					SalesEvent salesEvent3 = new SalesEvent
					{
						m_Amount = resourceBuyer.m_AmountNeeded,
						m_Buyer = resourceBuyer.m_Payer,
						m_Seller = m_CurrentBuildings[val2].m_CurrentBuilding,
						m_Resource = resourceBuyer.m_ResourceNeeded,
						m_Flags = flags,
						m_Distance = 0f
					};
					m_SalesQueue.Enqueue(salesEvent3);
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<ResourceBuyer>(unfilteredChunkIndex, val2);
				}
				else
				{
					Citizen citizen = default(Citizen);
					if (nativeArray4.Length > 0)
					{
						citizen = nativeArray4[j];
						Entity household = m_HouseholdMembers[val2].m_Household;
						Household householdData = m_Households[household];
						DynamicBuffer<HouseholdCitizen> val5 = m_HouseholdCitizens[household];
						FindShopForCitizen(chunk, unfilteredChunkIndex, val2, resourceBuyer.m_ResourceNeeded, resourceBuyer.m_AmountNeeded, resourceBuyer.m_Flags, citizen, householdData, val5.Length, flag);
					}
					else
					{
						FindShopForCompany(chunk, unfilteredChunkIndex, val2, resourceBuyer.m_ResourceNeeded, resourceBuyer.m_AmountNeeded, resourceBuyer.m_Flags, flag);
					}
				}
			}
		}

		private void FindShopForCitizen(ArchetypeChunk chunk, int index, Entity buyer, Resource resource, int amount, SetupTargetFlags flags, Citizen citizenData, Household householdData, int householdCitizenCount, bool virtualGood)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(index, buyer, ref m_PathfindTypes);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(index, buyer, new PathInformation
			{
				m_State = PathFlags.Pending
			});
			CreatureData creatureData;
			PseudoRandomSeed randomSeed;
			Entity val = ObjectEmergeSystem.SelectResidentPrefab(citizenData, m_HumanChunks, m_EntityType, ref m_CreatureDataType, ref m_ResidentDataType, out creatureData, out randomSeed);
			HumanData humanData = default(HumanData);
			if (val != Entity.Null)
			{
				humanData = m_PrefabHumanData[val];
			}
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(277.77777f),
				m_WalkSpeed = float2.op_Implicit(humanData.m_WalkSpeed),
				m_Weights = CitizenUtils.GetPathfindWeights(citizenData, householdData, householdCitizenCount),
				m_Methods = (PathMethod.Pedestrian | PathMethod.Taxi | RouteUtils.GetPublicTransportMethods(m_TimeOfDay)),
				m_SecondaryIgnoredRules = VehicleUtils.GetIgnoredPathfindRulesTaxiDefaults(),
				m_MaxCost = CitizenBehaviorSystem.kMaxPathfindCost
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = PathMethod.Pedestrian,
				m_RandomCost = 30f
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.ResourceSeller,
				m_Methods = PathMethod.Pedestrian,
				m_Resource = resource,
				m_Value = amount,
				m_Flags = flags,
				m_RandomCost = 30f,
				m_ActivityMask = creatureData.m_SupportedActivities
			};
			if (virtualGood)
			{
				parameters.m_PathfindFlags |= PathfindFlags.SkipPathfind;
			}
			if (m_HouseholdMembers.HasComponent(buyer))
			{
				Entity household = m_HouseholdMembers[buyer].m_Household;
				if (m_Properties.HasComponent(household))
				{
					parameters.m_Authorization1 = m_Properties[household].m_Property;
				}
			}
			if (m_Workers.HasComponent(buyer))
			{
				Worker worker = m_Workers[buyer];
				if (m_Properties.HasComponent(worker.m_Workplace))
				{
					parameters.m_Authorization2 = m_Properties[worker.m_Workplace].m_Property;
				}
				else
				{
					parameters.m_Authorization2 = worker.m_Workplace;
				}
			}
			if (m_CarKeepers.IsComponentEnabled(buyer))
			{
				Entity car = m_CarKeepers[buyer].m_Car;
				if (m_ParkedCarData.HasComponent(car))
				{
					PrefabRef prefabRef = m_PrefabRefData[car];
					ParkedCar parkedCar = m_ParkedCarData[car];
					CarData carData = m_PrefabCarData[prefabRef.m_Prefab];
					parameters.m_MaxSpeed.x = carData.m_MaxSpeed;
					parameters.m_ParkingTarget = parkedCar.m_Lane;
					parameters.m_ParkingDelta = parkedCar.m_CurvePosition;
					parameters.m_ParkingSize = VehicleUtils.GetParkingSize(car, ref m_PrefabRefData, ref m_ObjectGeometryData);
					parameters.m_Methods |= VehicleUtils.GetPathMethods(carData) | PathMethod.Parking;
					parameters.m_IgnoredRules = VehicleUtils.GetIgnoredPathfindRules(carData);
					Game.Vehicles.PersonalCar personalCar = default(Game.Vehicles.PersonalCar);
					if (m_PersonalCarData.TryGetComponent(car, ref personalCar) && (personalCar.m_State & PersonalCarFlags.HomeTarget) == 0)
					{
						parameters.m_PathfindFlags |= PathfindFlags.ParkingReset;
					}
				}
			}
			SetupQueueItem setupQueueItem = new SetupQueueItem(buyer, parameters, origin, destination);
			m_PathfindQueue.Enqueue(setupQueueItem);
		}

		private void FindShopForCompany(ArchetypeChunk chunk, int index, Entity buyer, Resource resource, int amount, SetupTargetFlags flags, bool virtualGood)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(index, buyer, ref m_PathfindTypes);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(index, buyer, new PathInformation
			{
				m_State = PathFlags.Pending
			});
			float transportCost = EconomyUtils.GetTransportCost(100f, amount, m_ResourceDatas[m_ResourcePrefabs[resource]].m_Weight, StorageTransferFlags.Car);
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(111.111115f),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, transportCost, 1f),
				m_Methods = (PathMethod.Road | PathMethod.CargoLoading),
				m_IgnoredRules = RuleFlags.ForbidSlowTraffic
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = (PathMethod.Road | PathMethod.CargoLoading),
				m_RoadTypes = RoadTypes.Car
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.ResourceSeller,
				m_Methods = (PathMethod.Road | PathMethod.CargoLoading),
				m_RoadTypes = RoadTypes.Car,
				m_Resource = resource,
				m_Value = amount,
				m_Flags = flags
			};
			if (virtualGood)
			{
				parameters.m_PathfindFlags |= PathfindFlags.SkipPathfind;
			}
			SetupQueueItem setupQueueItem = new SetupQueueItem(buyer, parameters, origin, destination);
			m_PathfindQueue.Enqueue(setupQueueItem);
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
		public ComponentTypeHandle<ResourceBuyer> __Game_Companies_ResourceBuyer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ResourceBought> __Game_Citizens_ResourceBought_RO_ComponentTypeHandle;

		public BufferTypeHandle<TripNeeded> __Game_Citizens_TripNeeded_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CreatureData> __Game_Prefabs_CreatureData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ResidentData> __Game_Prefabs_ResidentData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AttendingMeeting> __Game_Citizens_AttendingMeeting_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ServiceAvailable> __Game_Companies_ServiceAvailable_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarKeeper> __Game_Citizens_CarKeeper_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PersonalCar> __Game_Vehicles_PersonalCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> __Game_Citizens_TouristHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CommuterHousehold> __Game_Citizens_CommuterHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Companies.StorageCompany> __Game_Companies_StorageCompany_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<GuestVehicle> __Game_Vehicles_GuestVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HumanData> __Game_Prefabs_HumanData_RO_ComponentLookup;

		public ComponentLookup<CoordinatedMeeting> __Game_Citizens_CoordinatedMeeting_RW_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HaveCoordinatedMeetingData> __Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup;

		public ComponentLookup<Population> __Game_City_Population_RW_ComponentLookup;

		public BufferLookup<Resources> __Game_Economy_Resources_RW_BufferLookup;

		public ComponentLookup<ServiceAvailable> __Game_Companies_ServiceAvailable_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> __Game_Citizens_HouseholdAnimal_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ServiceCompanyData> __Game_Companies_ServiceCompanyData_RO_ComponentLookup;

		public ComponentLookup<Household> __Game_Citizens_Household_RW_ComponentLookup;

		public ComponentLookup<BuyingCompany> __Game_Companies_BuyingCompany_RW_ComponentLookup;

		public BufferLookup<TradeCost> __Game_Companies_TradeCost_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Population> __Game_City_Population_RO_ComponentLookup;

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
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Companies_ResourceBuyer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResourceBuyer>(true);
			__Game_Citizens_ResourceBought_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResourceBought>(true);
			__Game_Citizens_TripNeeded_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TripNeeded>(false);
			__Game_Citizens_Citizen_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(true);
			__Game_Prefabs_CreatureData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreatureData>(true);
			__Game_Prefabs_ResidentData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResidentData>(true);
			__Game_Citizens_AttendingMeeting_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AttendingMeeting>(true);
			__Game_Companies_ServiceAvailable_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceAvailable>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Citizens_CarKeeper_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarKeeper>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_PersonalCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PersonalCar>(true);
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_TouristHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TouristHousehold>(true);
			__Game_Citizens_CommuterHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CommuterHousehold>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Companies_StorageCompany_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Companies.StorageCompany>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Vehicles_GuestVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<GuestVehicle>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_HumanData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanData>(true);
			__Game_Citizens_CoordinatedMeeting_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CoordinatedMeeting>(false);
			__Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HaveCoordinatedMeetingData>(true);
			__Game_City_Population_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(false);
			__Game_Economy_Resources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(false);
			__Game_Companies_ServiceAvailable_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceAvailable>(false);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Citizens_HouseholdAnimal_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdAnimal>(true);
			__Game_Companies_ServiceCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceCompanyData>(true);
			__Game_Citizens_Household_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(false);
			__Game_Companies_BuyingCompany_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuyingCompany>(false);
			__Game_Companies_TradeCost_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TradeCost>(false);
			__Game_City_Population_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(true);
		}
	}

	private const int UPDATE_INTERVAL = 16;

	private EntityQuery m_BuyerQuery;

	private EntityQuery m_CarPrefabQuery;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_ResidentPrefabQuery;

	private EntityQuery m_PopulationQuery;

	private ComponentTypeSet m_PathfindTypes;

	private EndFrameBarrier m_EndFrameBarrier;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private ResourceSystem m_ResourceSystem;

	private TaxSystem m_TaxSystem;

	private TimeSystem m_TimeSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private PersonalCarSelectData m_PersonalCarSelectData;

	private CitySystem m_CitySystem;

	private NativeQueue<SalesEvent> m_SalesQueue;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_PersonalCarSelectData = new PersonalCarSelectData((SystemBase)(object)this);
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_SalesQueue = new NativeQueue<SalesEvent>(AllocatorHandle.op_Implicit((Allocator)4));
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<ResourceBuyer>(),
			ComponentType.ReadWrite<TripNeeded>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<TravelPurpose>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ResourceBought>() };
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[1] = val;
		m_BuyerQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_CarPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { PersonalCarSelectData.GetEntityQueryDesc() });
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_PopulationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Population>() });
		m_ResidentPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<HumanData>(),
			ComponentType.ReadOnly<ResidentData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_PathfindTypes = new ComponentTypeSet(ComponentType.ReadWrite<PathInformation>(), ComponentType.ReadWrite<PathElement>());
		((ComponentSystemBase)this).RequireForUpdate(m_BuyerQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_PopulationQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_SalesQueue.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		((COSystemBase)this).OnStopRunning();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_078b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07da: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_BuyerQuery)).CalculateEntityCount() > 0)
		{
			m_PersonalCarSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_CarPrefabQuery, (Allocator)3, out var jobHandle);
			JobHandle val = default(JobHandle);
			HandleBuyersJob handleBuyersJob = new HandleBuyersJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BuyerType = InternalCompilerInterface.GetComponentTypeHandle<ResourceBuyer>(ref __TypeHandle.__Game_Companies_ResourceBuyer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BoughtType = InternalCompilerInterface.GetComponentTypeHandle<ResourceBought>(ref __TypeHandle.__Game_Citizens_ResourceBought_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TripType = InternalCompilerInterface.GetBufferTypeHandle<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CreatureDataType = InternalCompilerInterface.GetComponentTypeHandle<CreatureData>(ref __TypeHandle.__Game_Prefabs_CreatureData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ResidentDataType = InternalCompilerInterface.GetComponentTypeHandle<ResidentData>(ref __TypeHandle.__Game_Prefabs_ResidentData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AttendingMeetingType = InternalCompilerInterface.GetComponentTypeHandle<AttendingMeeting>(ref __TypeHandle.__Game_Citizens_AttendingMeeting_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceAvailables = InternalCompilerInterface.GetComponentLookup<ServiceAvailable>(ref __TypeHandle.__Game_Companies_ServiceAvailable_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathInformation = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Properties = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarKeepers = InternalCompilerInterface.GetComponentLookup<CarKeeper>(ref __TypeHandle.__Game_Citizens_CarKeeper_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PersonalCarData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Targets = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentBuildings = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdMembers = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TouristHouseholds = InternalCompilerInterface.GetComponentLookup<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CommuterHouseholds = InternalCompilerInterface.GetComponentLookup<CommuterHousehold>(ref __TypeHandle.__Game_Citizens_CommuterHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DeliveryTrucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StorageCompanies = InternalCompilerInterface.GetComponentLookup<Game.Companies.StorageCompany>(ref __TypeHandle.__Game_Companies_StorageCompany_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_GuestVehicles = InternalCompilerInterface.GetBufferLookup<GuestVehicle>(ref __TypeHandle.__Game_Vehicles_GuestVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
				m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabHumanData = InternalCompilerInterface.GetComponentLookup<HumanData>(ref __TypeHandle.__Game_Prefabs_HumanData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CoordinatedMeetings = InternalCompilerInterface.GetComponentLookup<CoordinatedMeeting>(ref __TypeHandle.__Game_Citizens_CoordinatedMeeting_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HaveCoordinatedMeetingDatas = InternalCompilerInterface.GetBufferLookup<HaveCoordinatedMeetingData>(ref __TypeHandle.__Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Populations = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TimeOfDay = m_TimeSystem.normalizedTime,
				m_RandomSeed = RandomSeed.Next(),
				m_PathfindTypes = m_PathfindTypes,
				m_HumanChunks = ((EntityQuery)(ref m_ResidentPrefabQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
				m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 80, 16).AsParallelWriter()
			};
			EntityCommandBuffer val2 = m_EndFrameBarrier.CreateCommandBuffer();
			handleBuyersJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			handleBuyersJob.m_EconomyParameterData = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>();
			handleBuyersJob.m_City = m_CitySystem.City;
			handleBuyersJob.m_SalesQueue = m_SalesQueue.AsParallelWriter();
			HandleBuyersJob handleBuyersJob2 = handleBuyersJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<HandleBuyersJob>(handleBuyersJob2, m_BuyerQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
			m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
			m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
			m_PathfindSetupSystem.AddQueueWriter(((SystemBase)this).Dependency);
			BuyJob buyJob = new BuyJob
			{
				m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SalesQueue = m_SalesQueue,
				m_Services = InternalCompilerInterface.GetComponentLookup<ServiceAvailable>(ref __TypeHandle.__Game_Companies_ServiceAvailable_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformDatas = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdAnimals = InternalCompilerInterface.GetBufferLookup<HouseholdAnimal>(ref __TypeHandle.__Game_Citizens_HouseholdAnimal_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceCompanies = InternalCompilerInterface.GetComponentLookup<ServiceCompanyData>(ref __TypeHandle.__Game_Companies_ServiceCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Storages = InternalCompilerInterface.GetComponentLookup<Game.Companies.StorageCompany>(ref __TypeHandle.__Game_Companies_StorageCompany_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuyingCompanies = InternalCompilerInterface.GetComponentLookup<BuyingCompany>(ref __TypeHandle.__Game_Companies_BuyingCompany_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TradeCosts = InternalCompilerInterface.GetBufferLookup<TradeCost>(ref __TypeHandle.__Game_Companies_TradeCost_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
				m_RandomSeed = RandomSeed.Next(),
				m_PersonalCarSelectData = m_PersonalCarSelectData,
				m_PopulationData = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PopulationEntity = ((EntityQuery)(ref m_PopulationQuery)).GetSingletonEntity(),
				m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<BuyJob>(buyJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, jobHandle));
			m_PersonalCarSelectData.PostUpdate(((SystemBase)this).Dependency);
			m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
			m_TaxSystem.AddReader(((SystemBase)this).Dependency);
			m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		}
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
	public ResourceBuyerSystem()
	{
	}
}

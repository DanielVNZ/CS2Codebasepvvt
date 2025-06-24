using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Entities;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

[CompilerGenerated]
public class EconomyDebugSystem : BaseDebugSystem
{
	private struct EconomyGizmoJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_ResourceType;

		[ReadOnly]
		public BufferTypeHandle<TradeCost> m_TradeCostBufType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_RenterType;

		[ReadOnly]
		public ComponentTypeHandle<Household> m_HouseholdType;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdNeed> m_HouseholdNeedType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceAvailable> m_ServiceType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Companies.ProcessingCompany> m_ProcessingType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Companies.StorageCompany> m_StorageType;

		[ReadOnly]
		public ComponentTypeHandle<Profitability> m_ProfitabilityType;

		[ReadOnly]
		public ComponentTypeHandle<TaxPayer> m_TaxPayerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.CargoTransportStation> m_CargoTransportstationType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_ProcessDatas;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_Trucks;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageDatas;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingDatas;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_StorageLimitDatas;

		public EconomyParameterData m_EconomyParameters;

		public GizmoBatcher m_GizmoBatcher;

		public bool m_ResidentialOption;

		public bool m_CommercialOption;

		public bool m_IndustrialOption;

		public bool m_UntaxedIncomeOption;

		public bool m_StorageUsedOption;

		public bool m_HouseholdNeedOption;

		public bool m_ProfitabilityOption;

		public bool m_TradeCostOption;

		public bool m_CommercialStorageOption;

		private void Draw(Entity building, float value, int offset)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			value /= 500f;
			if (m_Transforms.HasComponent(building))
			{
				Transform transform = m_Transforms[building];
				float3 position = transform.m_Position;
				position.y += value / 2f + 10f;
				position += 5f * (float)offset * math.rotate(quaternion.op_Implicit(transform.m_Rotation.value), new float3(1f, 0f, 0f));
				Color val = ((value > 0f) ? Color.white : Color.red);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(position, new float3(5f, value, 5f), val);
			}
		}

		private void Draw(Entity building, float value, int offset, Color color)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			value /= 500f;
			if (m_Transforms.HasComponent(building))
			{
				Transform transform = m_Transforms[building];
				float3 position = transform.m_Position;
				position.y += value / 2f + 10f;
				position += 5f * (float)offset * math.rotate(quaternion.op_Implicit(transform.m_Rotation.value), new float3(1f, 0f, 0f));
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(position, new float3(5f, value, 5f), color);
			}
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
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0732: Unknown result type (might be due to invalid IL or missing references)
			//IL_0737: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_070e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_074c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			BufferAccessor<TradeCost> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TradeCost>(ref m_TradeCostBufType);
			NativeArray<PropertyRenter> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_RenterType);
			NativeArray<Household> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Household>(ref m_HouseholdType);
			if (((ArchetypeChunk)(ref chunk)).Has<Household>(ref m_HouseholdType))
			{
				if (m_ResidentialOption)
				{
					for (int i = 0; i < nativeArray2.Length; i++)
					{
						Entity property = nativeArray2[i].m_Property;
						int householdTotalWealth = EconomyUtils.GetHouseholdTotalWealth(nativeArray3[i], bufferAccessor[i]);
						Draw(property, householdTotalWealth, 0);
					}
				}
				else if (m_HouseholdNeedOption)
				{
					NativeArray<HouseholdNeed> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdNeed>(ref m_HouseholdNeedType);
					for (int j = 0; j < nativeArray2.Length; j++)
					{
						Entity property2 = nativeArray2[j].m_Property;
						Draw(property2, nativeArray4[j].m_Amount, 0);
					}
				}
			}
			if (m_CommercialOption && ((ArchetypeChunk)(ref chunk)).Has<ServiceAvailable>(ref m_ServiceType))
			{
				NativeArray<ServiceAvailable> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceAvailable>(ref m_ServiceType);
				for (int k = 0; k < nativeArray2.Length; k++)
				{
					Entity val = nativeArray[k];
					Entity property3 = nativeArray2[k].m_Property;
					Entity prefab = m_PrefabRefs[val].m_Prefab;
					IndustrialProcessData industrialProcessData = m_ProcessDatas[prefab];
					float value = ((!m_OwnedVehicles.HasBuffer(val)) ? ((float)EconomyUtils.GetCompanyTotalWorth(bufferAccessor[k], m_ResourcePrefabs, ref m_ResourceDatas)) : ((float)EconomyUtils.GetCompanyTotalWorth(vehicles: m_OwnedVehicles[val], resources: bufferAccessor[k], layouts: ref m_LayoutElements, deliveryTrucks: ref m_Trucks, resourcePrefabs: m_ResourcePrefabs, resourceDatas: ref m_ResourceDatas)));
					float num = EconomyUtils.GetResources(industrialProcessData.m_Output.m_Resource, bufferAccessor[k]);
					float num2 = nativeArray5[k].m_ServiceAvailable;
					float marketPrice = EconomyUtils.GetMarketPrice(industrialProcessData.m_Output.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas);
					num *= marketPrice;
					num2 *= marketPrice;
					Draw(property3, value, -1);
					Draw(property3, num, 0);
					Draw(property3, num2, 1);
				}
			}
			if (m_IndustrialOption && !((ArchetypeChunk)(ref chunk)).Has<ServiceAvailable>(ref m_ServiceType) && ((ArchetypeChunk)(ref chunk)).Has<Game.Companies.ProcessingCompany>(ref m_ProcessingType))
			{
				for (int l = 0; l < nativeArray2.Length; l++)
				{
					Entity val2 = nativeArray[l];
					Entity property4 = nativeArray2[l].m_Property;
					Entity prefab2 = m_PrefabRefs[val2].m_Prefab;
					IndustrialProcessData industrialProcessData2 = m_ProcessDatas[prefab2];
					float value2;
					if (m_OwnedVehicles.HasBuffer(val2))
					{
						DynamicBuffer<OwnedVehicle> vehicles = m_OwnedVehicles[val2];
						value2 = EconomyUtils.GetCompanyTotalWorth(bufferAccessor[l], vehicles, ref m_LayoutElements, ref m_Trucks, m_ResourcePrefabs, ref m_ResourceDatas);
					}
					else
					{
						value2 = EconomyUtils.GetCompanyTotalWorth(bufferAccessor[l], m_ResourcePrefabs, ref m_ResourceDatas);
					}
					float num3 = EconomyUtils.GetResources(industrialProcessData2.m_Input1.m_Resource, bufferAccessor[l]);
					num3 *= EconomyUtils.GetMarketPrice(industrialProcessData2.m_Input1.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas);
					float num4 = EconomyUtils.GetResources(industrialProcessData2.m_Output.m_Resource, bufferAccessor[l]);
					num4 *= EconomyUtils.GetMarketPrice(industrialProcessData2.m_Output.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas);
					Draw(property4, value2, -1);
					Draw(property4, num3, 0);
					Draw(property4, num4, 1);
				}
			}
			else if (m_UntaxedIncomeOption && ((ArchetypeChunk)(ref chunk)).Has<TaxPayer>(ref m_TaxPayerType))
			{
				NativeArray<TaxPayer> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TaxPayer>(ref m_TaxPayerType);
				for (int m = 0; m < nativeArray2.Length; m++)
				{
					Entity property5 = nativeArray2[m].m_Property;
					Draw(property5, nativeArray6[m].m_UntaxedIncome, 0);
				}
			}
			else if (m_StorageUsedOption && ((ArchetypeChunk)(ref chunk)).Has<Game.Companies.StorageCompany>(ref m_StorageType) && !((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.CargoTransportStation>(ref m_CargoTransportstationType))
			{
				for (int n = 0; n < nativeArray2.Length; n++)
				{
					Entity val3 = nativeArray[n];
					Entity property6 = nativeArray2[n].m_Property;
					Entity prefab3 = m_PrefabRefs[val3].m_Prefab;
					Entity prefab4 = m_PrefabRefs[property6].m_Prefab;
					StorageCompanyData storageCompanyData = m_StorageDatas[prefab3];
					StorageLimitData storageLimitData = m_StorageLimitDatas[prefab3];
					int resources = EconomyUtils.GetResources(storageCompanyData.m_StoredResources, bufferAccessor[n]);
					int adjustedLimitForWarehouse = storageLimitData.GetAdjustedLimitForWarehouse(m_SpawnableBuildingDatas[prefab4], m_BuildingDatas[prefab4]);
					EconomyUtils.GetResourceIndex(storageCompanyData.m_StoredResources);
					Color resourceColor = EconomyUtils.GetResourceColor(storageCompanyData.m_StoredResources);
					Draw(property6, resources, -1, resourceColor);
					Draw(property6, adjustedLimitForWarehouse, 0, resourceColor);
				}
			}
			else if (m_ProfitabilityOption && ((ArchetypeChunk)(ref chunk)).Has<Profitability>(ref m_ProfitabilityType))
			{
				NativeArray<Profitability> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Profitability>(ref m_ProfitabilityType);
				for (int num5 = 0; num5 < nativeArray7.Length; num5++)
				{
					Profitability profitability = nativeArray7[num5];
					Entity property7 = nativeArray2[num5].m_Property;
					Draw(property7, (int)profitability.m_Profitability, 0);
				}
			}
			else if (m_TradeCostOption && ((ArchetypeChunk)(ref chunk)).Has<TradeCost>(ref m_TradeCostBufType))
			{
				for (int num6 = 0; num6 < bufferAccessor2.Length; num6++)
				{
					Entity property8 = nativeArray2[num6].m_Property;
					DynamicBuffer<TradeCost> val4 = bufferAccessor2[num6];
					for (int num7 = 0; num7 < val4.Length; num7++)
					{
						Draw(property8, val4[num7].m_BuyCost * 100f, num7 * 2, (val4[num7].m_BuyCost > 5f) ? Color.red : Color.white);
						Draw(property8, val4[num7].m_SellCost * 100f, num7 * 2 + 1, Color.green);
					}
				}
			}
			else
			{
				if (!m_CommercialStorageOption || !((ArchetypeChunk)(ref chunk)).Has<ServiceAvailable>(ref m_ServiceType))
				{
					return;
				}
				for (int num8 = 0; num8 < bufferAccessor.Length; num8++)
				{
					Entity property9 = nativeArray2[num8].m_Property;
					int num9 = 0;
					for (int num10 = 0; num10 < bufferAccessor[num8].Length; num10++)
					{
						if (EconomyUtils.IsMaterial(bufferAccessor[num8][num10].m_Resource, m_ResourcePrefabs, ref m_ResourceDatas))
						{
							num9 += bufferAccessor[num8][num10].m_Amount;
						}
					}
					if (num9 > 8000)
					{
						Draw(property9, num9, 0, Color.red);
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

		[ReadOnly]
		public BufferTypeHandle<Resources> __Game_Economy_Resources_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<TradeCost> __Game_Companies_TradeCost_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Household> __Game_Citizens_Household_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ServiceAvailable> __Game_Companies_ServiceAvailable_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Companies.ProcessingCompany> __Game_Companies_ProcessingCompany_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Companies.StorageCompany> __Game_Companies_StorageCompany_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Profitability> __Game_Companies_Profitability_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TaxPayer> __Game_Agents_TaxPayer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.CargoTransportStation> __Game_Buildings_CargoTransportStation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdNeed> __Game_Citizens_HouseholdNeed_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> __Game_Prefabs_StorageCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Economy_Resources_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(true);
			__Game_Companies_TradeCost_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TradeCost>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Citizens_Household_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Household>(true);
			__Game_Companies_ServiceAvailable_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceAvailable>(true);
			__Game_Companies_ProcessingCompany_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Companies.ProcessingCompany>(true);
			__Game_Companies_StorageCompany_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Companies.StorageCompany>(true);
			__Game_Companies_Profitability_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Profitability>(true);
			__Game_Agents_TaxPayer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TaxPayer>(true);
			__Game_Buildings_CargoTransportStation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.CargoTransportStation>(true);
			__Game_Citizens_HouseholdNeed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdNeed>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_StorageCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageCompanyData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
		}
	}

	private EntityQuery m_AgentQuery;

	private EntityQuery m_EconomyParameterQuery;

	private GizmosSystem m_GizmosSystem;

	private ResourceSystem m_ResourceSystem;

	private Option m_ResidentialOption;

	private Option m_CommercialOption;

	private Option m_CommercialStorageOption;

	private Option m_IndustrialOption;

	private Option m_UntaxedIncomeOption;

	private Option m_StorageUsedOption;

	private Option m_HouseholdNeedOption;

	private Option m_ProfitabilityOption;

	private Option m_TradeCostOption;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Resources>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<PropertyRenter>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Household>(),
			ComponentType.ReadOnly<ServiceAvailable>(),
			ComponentType.ReadOnly<Game.Companies.ProcessingCompany>(),
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<Profitability>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Hidden>()
		};
		array[0] = val;
		m_AgentQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		((ComponentSystemBase)this).Enabled = false;
		((ComponentSystemBase)this).RequireForUpdate(m_AgentQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
		m_ResidentialOption = AddOption("Residential worth", defaultEnabled: false);
		m_CommercialOption = AddOption("Commercial worth", defaultEnabled: false);
		m_IndustrialOption = AddOption("Industrial worth", defaultEnabled: false);
		m_UntaxedIncomeOption = AddOption("Untaxed income", defaultEnabled: false);
		m_StorageUsedOption = AddOption("Storage used", defaultEnabled: false);
		m_HouseholdNeedOption = AddOption("Household need", defaultEnabled: false);
		m_ProfitabilityOption = AddOption("Company Profitability", defaultEnabled: false);
		m_TradeCostOption = AddOption("Trade Cost Profitability", defaultEnabled: false);
		m_CommercialStorageOption = AddOption("Commercial storage", defaultEnabled: false);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_AgentQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val = default(JobHandle);
			EconomyGizmoJob economyGizmoJob = new EconomyGizmoJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TradeCostBufType = InternalCompilerInterface.GetBufferTypeHandle<TradeCost>(ref __TypeHandle.__Game_Companies_TradeCost_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RenterType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdType = InternalCompilerInterface.GetComponentTypeHandle<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceType = InternalCompilerInterface.GetComponentTypeHandle<ServiceAvailable>(ref __TypeHandle.__Game_Companies_ServiceAvailable_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ProcessingType = InternalCompilerInterface.GetComponentTypeHandle<Game.Companies.ProcessingCompany>(ref __TypeHandle.__Game_Companies_ProcessingCompany_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_StorageType = InternalCompilerInterface.GetComponentTypeHandle<Game.Companies.StorageCompany>(ref __TypeHandle.__Game_Companies_StorageCompany_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ProfitabilityType = InternalCompilerInterface.GetComponentTypeHandle<Profitability>(ref __TypeHandle.__Game_Companies_Profitability_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TaxPayerType = InternalCompilerInterface.GetComponentTypeHandle<TaxPayer>(ref __TypeHandle.__Game_Agents_TaxPayer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CargoTransportstationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.CargoTransportStation>(ref __TypeHandle.__Game_Buildings_CargoTransportStation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdNeedType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdNeed>(ref __TypeHandle.__Game_Citizens_HouseholdNeed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Trucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
				m_StorageDatas = InternalCompilerInterface.GetComponentLookup<StorageCompanyData>(ref __TypeHandle.__Game_Prefabs_StorageCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnableBuildingDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StorageLimitDatas = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResidentialOption = m_ResidentialOption.enabled,
				m_CommercialOption = m_CommercialOption.enabled,
				m_IndustrialOption = m_IndustrialOption.enabled,
				m_UntaxedIncomeOption = m_UntaxedIncomeOption.enabled,
				m_StorageUsedOption = m_StorageUsedOption.enabled,
				m_HouseholdNeedOption = m_HouseholdNeedOption.enabled,
				m_ProfitabilityOption = m_ProfitabilityOption.enabled,
				m_TradeCostOption = m_TradeCostOption.enabled,
				m_CommercialStorageOption = m_CommercialStorageOption.enabled,
				m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
				m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val)
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<EconomyGizmoJob>(economyGizmoJob, m_AgentQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
			m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
			m_GizmosSystem.AddGizmosBatcherWriter(((SystemBase)this).Dependency);
		}
	}

	public static void RemoveExtraCompanies()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<CompanyData>(),
			ComponentType.Exclude<PropertyRenter>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((EntityManager)(ref entityManager)).AddComponent<Deleted>(val);
	}

	public static void PrintTradeDebug(ITradeSystem tradeSystem, DynamicBuffer<CityModifier> cityEffects)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		ResourceIterator iterator = ResourceIterator.GetIterator();
		while (iterator.Next())
		{
			Debug.Log((object)(EconomyUtils.GetName(iterator.resource) + ":"));
			Debug.Log((object)("Road: " + tradeSystem.GetTradePrice(iterator.resource, OutsideConnectionTransferType.Road, import: true, cityEffects) + " / " + tradeSystem.GetTradePrice(iterator.resource, OutsideConnectionTransferType.Road, import: false, cityEffects)));
			Debug.Log((object)("Air: " + tradeSystem.GetTradePrice(iterator.resource, OutsideConnectionTransferType.Air, import: true, cityEffects) + " / " + tradeSystem.GetTradePrice(iterator.resource, OutsideConnectionTransferType.Air, import: false, cityEffects)));
			Debug.Log((object)("Rail: " + tradeSystem.GetTradePrice(iterator.resource, OutsideConnectionTransferType.Train, import: true, cityEffects) + " / " + tradeSystem.GetTradePrice(iterator.resource, OutsideConnectionTransferType.Train, import: false, cityEffects)));
			Debug.Log((object)("Ship: " + tradeSystem.GetTradePrice(iterator.resource, OutsideConnectionTransferType.Ship, import: true, cityEffects) + " / " + tradeSystem.GetTradePrice(iterator.resource, OutsideConnectionTransferType.Ship, import: false, cityEffects)));
		}
	}

	public static void PrintAgeDebug()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Household>(),
			ComponentType.ReadOnly<MovingAway>(),
			ComponentType.Exclude<CommuterHousehold>(),
			ComponentType.Exclude<TouristHousehold>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQuery val2 = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeSettingsData>() });
		EntityQuery val3 = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		TimeData singleton = ((EntityQuery)(ref val3)).GetSingleton<TimeData>();
		((EntityQuery)(ref val2)).GetSingleton<TimeSettingsData>();
		NativeArray<Entity> val4 = ((EntityQuery)(ref val)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		int num = 0;
		int[] array = new int[240];
		int day = TimeSystem.GetDay(World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<SimulationSystem>().frameIndex, singleton);
		for (int i = 0; i < val4.Length; i++)
		{
			DynamicBuffer<HouseholdCitizen> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HouseholdCitizen>(val4[i], true);
			for (int j = 0; j < buffer.Length; j++)
			{
				Entity citizen = buffer[j].m_Citizen;
				int num2 = day - ((EntityManager)(ref entityManager)).GetComponentData<Citizen>(citizen).m_BirthDay;
				array[num2]++;
				num = math.max(num, num2);
			}
		}
		val4.Dispose();
		for (int k = 0; k < num; k++)
		{
			Debug.Log((object)(k + ": " + array[k]));
		}
	}

	public static void PrintSchoolDebug()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Citizen>() });
		int num = 0;
		int num2 = 0;
		NativeArray<Entity> val2 = ((EntityQuery)(ref val)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		Citizen citizen = default(Citizen);
		for (int i = 0; i < val2.Length; i++)
		{
			if (EntitiesExtensions.TryGetComponent<Citizen>(entityManager, val2[i], ref citizen) && citizen.GetAge() == CitizenAge.Child)
			{
				num++;
				if (((EntityManager)(ref entityManager)).GetComponentData<Citizen>(val2[i]).GetEducationLevel() > 1)
				{
					Debug.Log((object)$"{val2[i].Index} level ");
				}
				else
				{
					num2++;
				}
			}
		}
		Debug.Log((object)$"Processed {num} children, {num2} ok");
	}

	public static void PrintCompanyDebug(ComponentLookup<ResourceData> resourceDatas)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<IndustrialProcessData>(),
			ComponentType.ReadOnly<ServiceCompanyData>(),
			ComponentType.ReadOnly<WorkplaceData>()
		});
		EntityQuery val2 = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<IndustrialProcessData>(),
			ComponentType.ReadOnly<IndustrialCompanyData>(),
			ComponentType.ReadOnly<WorkplaceData>(),
			ComponentType.Exclude<StorageCompanyData>()
		});
		ResourcePrefabs prefabs = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<ResourceSystem>().GetPrefabs();
		NativeArray<ServiceCompanyData> val3 = ((EntityQuery)(ref val)).ToComponentDataArray<ServiceCompanyData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<IndustrialProcessData> val4 = ((EntityQuery)(ref val)).ToComponentDataArray<IndustrialProcessData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<WorkplaceData> val5 = ((EntityQuery)(ref val)).ToComponentDataArray<WorkplaceData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<IndustrialProcessData> val6 = ((EntityQuery)(ref val2)).ToComponentDataArray<IndustrialProcessData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<WorkplaceData> val7 = ((EntityQuery)(ref val2)).ToComponentDataArray<WorkplaceData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Entity> val8 = ((EntityQuery)(ref val2)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityQuery val9 = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		NativeArray<EconomyParameterData> val10 = ((EntityQuery)(ref val9)).ToComponentDataArray<EconomyParameterData>(AllocatorHandle.op_Implicit((Allocator)3));
		EconomyParameterData economyParameters = val10[0];
		Debug.Log((object)"Company data per cell");
		for (int i = 0; i < val3.Length; i++)
		{
			ServiceCompanyData serviceCompanyData = val3[i];
			IndustrialProcessData industrialProcessData = val4[i];
			BuildingData buildingData = new BuildingData
			{
				m_LotSize = new int2(100, 10)
			};
			ServiceAvailable serviceAvailable = new ServiceAvailable
			{
				m_MeanPriority = 0.5f
			};
			WorkplaceData workplaceData = val5[i];
			SpawnableBuildingData spawnableBuildingData = new SpawnableBuildingData
			{
				m_Level = 1
			};
			SpawnableBuildingData spawnableBuildingData2 = new SpawnableBuildingData
			{
				m_Level = 5
			};
			EconomyUtils.BuildPseudoTradeCost(5000f, industrialProcessData, ref resourceDatas, prefabs);
			string text = "C " + EconomyUtils.GetName(industrialProcessData.m_Output.m_Resource) + ": ";
			int workerAmount = Mathf.RoundToInt(serviceCompanyData.m_MaxWorkersPerCell * 1000f);
			int companyProductionPerDay = EconomyUtils.GetCompanyProductionPerDay(1f, workerAmount, spawnableBuildingData.m_Level, isIndustrial: true, workplaceData, industrialProcessData, prefabs, ref resourceDatas, ref economyParameters);
			int companyProductionPerDay2 = EconomyUtils.GetCompanyProductionPerDay(1f, workerAmount, spawnableBuildingData2.m_Level, isIndustrial: true, workplaceData, industrialProcessData, prefabs, ref resourceDatas, ref economyParameters);
			text = text + "Production " + (float)companyProductionPerDay / 1000f + "|" + (float)companyProductionPerDay2 / 1000f + ")";
			Debug.Log((object)text);
		}
		for (int j = 0; j < val6.Length; j++)
		{
			IndustrialProcessData process = val6[j];
			BuildingData buildingData = new BuildingData
			{
				m_LotSize = new int2(100, 10)
			};
			EconomyUtils.BuildPseudoTradeCost(5000f, process, ref resourceDatas, prefabs);
			_ = val7[j];
			SpawnableBuildingData spawnableBuildingData3 = new SpawnableBuildingData
			{
				m_Level = 1
			};
			spawnableBuildingData3 = new SpawnableBuildingData
			{
				m_Level = 5
			};
			Debug.Log((object)("I " + EconomyUtils.GetName(process.m_Input1.m_Resource) + " => " + EconomyUtils.GetName(process.m_Output.m_Resource) + ": "));
		}
		val3.Dispose();
		val4.Dispose();
		val5.Dispose();
		val8.Dispose();
		val6.Dispose();
		val7.Dispose();
		val10.Dispose();
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
	public EconomyDebugSystem()
	{
	}
}

using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Debug;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Prefabs.Modes;
using Game.Reflection;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class IndustrialDemandSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	[BurstCompile]
	private struct UpdateIndustrialDemandJob : IJob
	{
		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<ZoneData> m_UnlockedZoneDatas;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_IndustrialPropertyChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_OfficePropertyChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_StorageCompanyChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_CityServiceChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<CityServiceUpkeep> m_ServiceUpkeepType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyOnMarket> m_PropertyOnMarketType;

		[ReadOnly]
		public ComponentLookup<Population> m_Populations;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyDatas;

		[ReadOnly]
		public ComponentLookup<Attached> m_Attached;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_StorageLimitDatas;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingDatas;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> m_ServiceUpkeeps;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> m_Upkeeps;

		public EconomyParameterData m_EconomyParameters;

		public DemandParameterData m_DemandParameters;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public NativeArray<int> m_EmployableByEducation;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		[ReadOnly]
		public Workplaces m_FreeWorkplaces;

		public Entity m_City;

		public NativeValue<int> m_IndustrialCompanyDemand;

		public NativeValue<int> m_IndustrialBuildingDemand;

		public NativeValue<int> m_StorageCompanyDemand;

		public NativeValue<int> m_StorageBuildingDemand;

		public NativeValue<int> m_OfficeCompanyDemand;

		public NativeValue<int> m_OfficeBuildingDemand;

		public NativeArray<int> m_IndustrialDemandFactors;

		public NativeArray<int> m_OfficeDemandFactors;

		public NativeArray<int> m_IndustrialCompanyDemands;

		public NativeArray<int> m_IndustrialBuildingDemands;

		public NativeArray<int> m_StorageBuildingDemands;

		public NativeArray<int> m_StorageCompanyDemands;

		[ReadOnly]
		public NativeArray<int> m_Productions;

		[ReadOnly]
		public NativeArray<int> m_CompanyResourceDemands;

		public NativeArray<int> m_FreeProperties;

		[ReadOnly]
		public NativeArray<int> m_Propertyless;

		public NativeArray<int> m_FreeStorages;

		public NativeArray<int> m_Storages;

		public NativeArray<int> m_StorageCapacities;

		public NativeArray<int> m_ResourceDemands;

		public float m_IndustrialOfficeTaxEffectDemandOffset;

		public void Execute()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0876: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f02: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0897: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			for (int i = 0; i < m_UnlockedZoneDatas.Length; i++)
			{
				if (m_UnlockedZoneDatas[i].m_AreaType == AreaType.Industrial)
				{
					flag = true;
					break;
				}
			}
			DynamicBuffer<CityModifier> modifiers = m_CityModifiers[m_City];
			ResourceIterator iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				int resourceIndex = EconomyUtils.GetResourceIndex(iterator.resource);
				ResourceData resourceData = m_ResourceDatas[m_ResourcePrefabs[iterator.resource]];
				m_ResourceDemands[resourceIndex] = ((m_CompanyResourceDemands[resourceIndex] == 0 && EconomyUtils.IsIndustrialResource(resourceData, includeMaterial: false, includeOffice: false)) ? 100 : m_CompanyResourceDemands[resourceIndex]);
				m_FreeProperties[resourceIndex] = 0;
				m_Storages[resourceIndex] = 0;
				m_FreeStorages[resourceIndex] = 0;
				m_StorageCapacities[resourceIndex] = 0;
			}
			for (int j = 0; j < m_IndustrialDemandFactors.Length; j++)
			{
				m_IndustrialDemandFactors[j] = 0;
			}
			for (int k = 0; k < m_OfficeDemandFactors.Length; k++)
			{
				m_OfficeDemandFactors[k] = 0;
			}
			int resourceIndex2;
			ref NativeArray<int> reference;
			for (int l = 0; l < m_CityServiceChunks.Length; l++)
			{
				ArchetypeChunk val = m_CityServiceChunks[l];
				if (!((ArchetypeChunk)(ref val)).Has<CityServiceUpkeep>(ref m_ServiceUpkeepType))
				{
					continue;
				}
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabType);
				for (int m = 0; m < nativeArray2.Length; m++)
				{
					Entity prefab = nativeArray2[m].m_Prefab;
					Entity val2 = nativeArray[m];
					if (m_ServiceUpkeeps.HasBuffer(prefab))
					{
						DynamicBuffer<ServiceUpkeepData> val3 = m_ServiceUpkeeps[prefab];
						for (int n = 0; n < val3.Length; n++)
						{
							ServiceUpkeepData serviceUpkeepData = val3[n];
							if (serviceUpkeepData.m_Upkeep.m_Resource != Resource.Money)
							{
								int amount = serviceUpkeepData.m_Upkeep.m_Amount;
								reference = ref m_ResourceDemands;
								resourceIndex2 = EconomyUtils.GetResourceIndex(serviceUpkeepData.m_Upkeep.m_Resource);
								reference[resourceIndex2] += amount;
							}
						}
					}
					if (!m_InstalledUpgrades.HasBuffer(val2))
					{
						continue;
					}
					DynamicBuffer<InstalledUpgrade> val4 = m_InstalledUpgrades[val2];
					for (int num = 0; num < val4.Length; num++)
					{
						Entity upgrade = val4[num].m_Upgrade;
						if (BuildingUtils.CheckOption(val4[num], BuildingOption.Inactive) || !m_Prefabs.HasComponent(upgrade))
						{
							continue;
						}
						Entity prefab2 = m_Prefabs[upgrade].m_Prefab;
						if (m_Upkeeps.HasBuffer(prefab2))
						{
							DynamicBuffer<ServiceUpkeepData> val5 = m_Upkeeps[prefab2];
							for (int num2 = 0; num2 < val5.Length; num2++)
							{
								ServiceUpkeepData serviceUpkeepData2 = val5[num2];
								reference = ref m_ResourceDemands;
								resourceIndex2 = EconomyUtils.GetResourceIndex(serviceUpkeepData2.m_Upkeep.m_Resource);
								reference[resourceIndex2] += serviceUpkeepData2.m_Upkeep.m_Amount;
							}
						}
					}
				}
			}
			int num3 = 0;
			int num4 = 0;
			for (int num5 = 0; num5 < m_Productions.Length; num5++)
			{
				Resource resource = EconomyUtils.GetResource(num5);
				ResourceData resourceData2 = m_ResourceDatas[m_ResourcePrefabs[resource]];
				if (resourceData2.m_IsProduceable)
				{
					if (resourceData2.m_Weight > 0f)
					{
						num3 += m_Productions[num5];
					}
					else
					{
						num4 += m_Productions[num5];
					}
				}
			}
			int num6 = num4 + num3;
			reference = ref m_ResourceDemands;
			resourceIndex2 = EconomyUtils.GetResourceIndex(Resource.Software);
			reference[resourceIndex2] += num6 / m_EconomyParameters.m_PerOfficeResourceNeededForIndustrial;
			reference = ref m_ResourceDemands;
			resourceIndex2 = EconomyUtils.GetResourceIndex(Resource.Financial);
			reference[resourceIndex2] += num6 / m_EconomyParameters.m_PerOfficeResourceNeededForIndustrial;
			reference = ref m_ResourceDemands;
			resourceIndex2 = EconomyUtils.GetResourceIndex(Resource.Telecom);
			reference[resourceIndex2] += num6 / m_EconomyParameters.m_PerOfficeResourceNeededForIndustrial;
			for (int num7 = 0; num7 < m_StorageCompanyChunks.Length; num7++)
			{
				ArchetypeChunk val6 = m_StorageCompanyChunks[num7];
				NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref val6)).GetNativeArray(m_EntityType);
				NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref val6)).GetNativeArray<PrefabRef>(ref m_PrefabType);
				for (int num8 = 0; num8 < nativeArray3.Length; num8++)
				{
					Entity val7 = nativeArray3[num8];
					Entity prefab3 = nativeArray4[num8].m_Prefab;
					if (m_IndustrialProcessDatas.HasComponent(prefab3))
					{
						int resourceIndex3 = EconomyUtils.GetResourceIndex(m_IndustrialProcessDatas[prefab3].m_Output.m_Resource);
						ref NativeArray<int> reference2 = ref m_Storages;
						resourceIndex2 = resourceIndex3;
						int num9 = reference2[resourceIndex2];
						reference2[resourceIndex2] = num9 + 1;
						StorageLimitData storageLimitData = m_StorageLimitDatas[prefab3];
						if (!m_PropertyRenters.HasComponent(val7) || !m_Prefabs.HasComponent(m_PropertyRenters[val7].m_Property))
						{
							ref NativeArray<int> reference3 = ref m_FreeStorages;
							num9 = resourceIndex3;
							resourceIndex2 = reference3[num9];
							reference3[num9] = resourceIndex2 - 1;
							reference = ref m_StorageCapacities;
							resourceIndex2 = resourceIndex3;
							reference[resourceIndex2] += kStorageCompanyEstimateLimit;
						}
						else
						{
							Entity property = m_PropertyRenters[val7].m_Property;
							Entity prefab4 = m_Prefabs[property].m_Prefab;
							reference = ref m_StorageCapacities;
							resourceIndex2 = resourceIndex3;
							reference[resourceIndex2] += storageLimitData.GetAdjustedLimitForWarehouse(m_SpawnableBuildingDatas[prefab4], m_BuildingDatas[prefab4]);
						}
					}
				}
			}
			Attached attached = default(Attached);
			PrefabRef prefabRef = default(PrefabRef);
			BuildingPropertyData buildingPropertyData2 = default(BuildingPropertyData);
			for (int num10 = 0; num10 < m_IndustrialPropertyChunks.Length; num10++)
			{
				ArchetypeChunk val8 = m_IndustrialPropertyChunks[num10];
				if (!((ArchetypeChunk)(ref val8)).Has<PropertyOnMarket>(ref m_PropertyOnMarketType))
				{
					continue;
				}
				NativeArray<Entity> nativeArray5 = ((ArchetypeChunk)(ref val8)).GetNativeArray(m_EntityType);
				NativeArray<PrefabRef> nativeArray6 = ((ArchetypeChunk)(ref val8)).GetNativeArray<PrefabRef>(ref m_PrefabType);
				for (int num11 = 0; num11 < nativeArray6.Length; num11++)
				{
					Entity prefab5 = nativeArray6[num11].m_Prefab;
					if (!m_BuildingPropertyDatas.HasComponent(prefab5))
					{
						continue;
					}
					BuildingPropertyData buildingPropertyData = m_BuildingPropertyDatas[prefab5];
					if (m_Attached.TryGetComponent(nativeArray5[num11], ref attached) && m_Prefabs.TryGetComponent(attached.m_Parent, ref prefabRef) && m_BuildingPropertyDatas.TryGetComponent(prefabRef.m_Prefab, ref buildingPropertyData2))
					{
						buildingPropertyData.m_AllowedManufactured &= buildingPropertyData2.m_AllowedManufactured;
					}
					ResourceIterator iterator2 = ResourceIterator.GetIterator();
					while (iterator2.Next())
					{
						int resourceIndex4 = EconomyUtils.GetResourceIndex(iterator2.resource);
						if ((buildingPropertyData.m_AllowedManufactured & iterator2.resource) != Resource.NoResource)
						{
							ref NativeArray<int> reference4 = ref m_FreeProperties;
							resourceIndex2 = resourceIndex4;
							int num9 = reference4[resourceIndex2];
							reference4[resourceIndex2] = num9 + 1;
						}
						if ((buildingPropertyData.m_AllowedStored & iterator2.resource) != Resource.NoResource)
						{
							ref NativeArray<int> reference5 = ref m_FreeStorages;
							int num9 = resourceIndex4;
							resourceIndex2 = reference5[num9];
							reference5[num9] = resourceIndex2 + 1;
						}
					}
				}
			}
			_ = m_IndustrialBuildingDemand.value;
			bool flag2 = m_OfficeBuildingDemand.value > 0;
			_ = m_StorageBuildingDemand.value;
			m_IndustrialCompanyDemand.value = 0;
			m_IndustrialBuildingDemand.value = 0;
			m_StorageCompanyDemand.value = 0;
			m_StorageBuildingDemand.value = 0;
			m_OfficeCompanyDemand.value = 0;
			m_OfficeBuildingDemand.value = 0;
			int num12 = 0;
			int num13 = 0;
			iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				int resourceIndex5 = EconomyUtils.GetResourceIndex(iterator.resource);
				if (!m_ResourceDatas.HasComponent(m_ResourcePrefabs[iterator.resource]))
				{
					continue;
				}
				ResourceData resourceData3 = m_ResourceDatas[m_ResourcePrefabs[iterator.resource]];
				bool isProduceable = resourceData3.m_IsProduceable;
				bool isMaterial = resourceData3.m_IsMaterial;
				bool isTradable = resourceData3.m_IsTradable;
				bool flag3 = resourceData3.m_Weight == 0f;
				if (isTradable && !flag3)
				{
					int num14 = m_ResourceDemands[resourceIndex5];
					m_StorageCompanyDemands[resourceIndex5] = 0;
					m_StorageBuildingDemands[resourceIndex5] = 0;
					if (num14 > kStorageProductionDemand && m_StorageCapacities[resourceIndex5] < num14)
					{
						m_StorageCompanyDemands[resourceIndex5] = 1;
					}
					if (m_FreeStorages[resourceIndex5] < 0)
					{
						m_StorageBuildingDemands[resourceIndex5] = 1;
					}
					ref NativeValue<int> reference6 = ref m_StorageCompanyDemand;
					reference6.value += m_StorageCompanyDemands[resourceIndex5];
					ref NativeValue<int> reference7 = ref m_StorageBuildingDemand;
					reference7.value += m_StorageBuildingDemands[resourceIndex5];
					reference = ref m_IndustrialDemandFactors;
					reference[17] = reference[17] + math.max(0, m_StorageBuildingDemands[resourceIndex5]);
				}
				if (!isProduceable)
				{
					continue;
				}
				float value = (isMaterial ? m_DemandParameters.m_ExtractorBaseDemand : m_DemandParameters.m_IndustrialBaseDemand);
				float num15 = (1f + (float)m_ResourceDemands[resourceIndex5] - (float)m_Productions[resourceIndex5]) / ((float)m_ResourceDemands[resourceIndex5] + 1f);
				if (iterator.resource == Resource.Electronics)
				{
					CityUtils.ApplyModifier(ref value, modifiers, CityModifierType.IndustrialElectronicsDemand);
				}
				else if (iterator.resource == Resource.Software)
				{
					CityUtils.ApplyModifier(ref value, modifiers, CityModifierType.OfficeSoftwareDemand);
				}
				int num16 = (flag3 ? TaxSystem.GetOfficeTaxRate(iterator.resource, m_TaxRates) : TaxSystem.GetIndustrialTaxRate(iterator.resource, m_TaxRates));
				float num17 = m_DemandParameters.m_TaxEffect.z * -0.05f * ((float)num16 - 10f);
				num17 += m_IndustrialOfficeTaxEffectDemandOffset;
				float num18 = 100f * num17;
				int num19 = 0;
				int num20 = 0;
				float num21 = m_DemandParameters.m_NeutralUnemployment / 100f;
				for (int num22 = 0; num22 < 5; num22++)
				{
					if (num22 < 2)
					{
						num20 += (int)((float)m_EmployableByEducation[num22] * (1f - num21)) - m_FreeWorkplaces[num22];
					}
					else
					{
						num19 += (int)((float)m_EmployableByEducation[num22] * (1f - num21)) - m_FreeWorkplaces[num22];
					}
				}
				float num23 = 50f * math.max(0f, value * num15);
				if (num18 > 0f)
				{
					num19 = (int)MapAndClaimWorkforceEffect(num19, 0f - math.max(10f + num18, 10f), 10f);
					num20 = (int)MapAndClaimWorkforceEffect(num20, 0f - math.max(10f + num18, 10f), 15f);
				}
				else
				{
					num19 = math.clamp(num19, -10, 10);
					num20 = math.clamp(num20, -10, 15);
				}
				if (flag3)
				{
					m_IndustrialCompanyDemands[resourceIndex5] = Mathf.RoundToInt(num23 + num18 + (float)num19);
					m_IndustrialCompanyDemands[resourceIndex5] = math.min(100, math.max(0, m_IndustrialCompanyDemands[resourceIndex5]));
					ref NativeValue<int> reference8 = ref m_OfficeCompanyDemand;
					reference8.value += Mathf.RoundToInt((float)m_IndustrialCompanyDemands[resourceIndex5]);
					num12++;
				}
				else
				{
					m_IndustrialCompanyDemands[resourceIndex5] = Mathf.RoundToInt(num23 + num18 + (float)num19 + (float)num20);
					m_IndustrialCompanyDemands[resourceIndex5] = math.min(100, math.max(0, m_IndustrialCompanyDemands[resourceIndex5]));
					ref NativeValue<int> reference9 = ref m_IndustrialCompanyDemand;
					reference9.value += Mathf.RoundToInt((float)m_IndustrialCompanyDemands[resourceIndex5]);
					if (!isMaterial)
					{
						num13++;
					}
				}
				if (m_ResourceDemands[resourceIndex5] > 0)
				{
					if (!isMaterial && m_IndustrialCompanyDemands[resourceIndex5] > 0)
					{
						m_IndustrialBuildingDemands[resourceIndex5] = ((m_FreeProperties[resourceIndex5] - m_Propertyless[resourceIndex5] <= 0) ? 50 : 0);
					}
					else if (m_IndustrialCompanyDemands[resourceIndex5] > 0)
					{
						m_IndustrialBuildingDemands[resourceIndex5] = 1;
					}
					else
					{
						m_IndustrialBuildingDemands[resourceIndex5] = 0;
					}
					if (m_IndustrialBuildingDemands[resourceIndex5] > 0)
					{
						if (flag3)
						{
							ref NativeValue<int> reference10 = ref m_OfficeBuildingDemand;
							reference10.value += ((m_IndustrialBuildingDemands[resourceIndex5] > 0) ? m_IndustrialCompanyDemands[resourceIndex5] : 0);
						}
						else if (!isMaterial)
						{
							ref NativeValue<int> reference11 = ref m_IndustrialBuildingDemand;
							reference11.value += ((m_IndustrialBuildingDemands[resourceIndex5] > 0) ? m_IndustrialCompanyDemands[resourceIndex5] : 0);
						}
					}
				}
				if (isMaterial)
				{
					continue;
				}
				if (flag3)
				{
					if (!flag2 || (m_IndustrialBuildingDemands[resourceIndex5] > 0 && m_IndustrialCompanyDemands[resourceIndex5] > 0))
					{
						reference = ref m_OfficeDemandFactors;
						reference[2] = reference[2] + num19;
						reference = ref m_OfficeDemandFactors;
						reference[4] = reference[4] + (int)num23;
						reference = ref m_OfficeDemandFactors;
						reference[11] = reference[11] + (int)num18;
						reference = ref m_OfficeDemandFactors;
						reference[13] = reference[13] + m_IndustrialBuildingDemands[resourceIndex5];
					}
				}
				else
				{
					reference = ref m_IndustrialDemandFactors;
					reference[2] = reference[2] + num19;
					reference = ref m_IndustrialDemandFactors;
					reference[1] = reference[1] + num20;
					reference = ref m_IndustrialDemandFactors;
					reference[4] = reference[4] + (int)num23;
					reference = ref m_IndustrialDemandFactors;
					reference[11] = reference[11] + (int)num18;
					reference = ref m_IndustrialDemandFactors;
					reference[13] = reference[13] + m_IndustrialBuildingDemands[resourceIndex5];
				}
			}
			if (m_Populations[m_City].m_Population <= 0)
			{
				m_OfficeDemandFactors[4] = 0;
				m_IndustrialDemandFactors[4] = 0;
			}
			if (m_IndustrialPropertyChunks.Length == 0)
			{
				m_IndustrialDemandFactors[13] = 0;
			}
			if (m_OfficePropertyChunks.Length == 0)
			{
				m_OfficeDemandFactors[13] = 0;
			}
			m_StorageBuildingDemand.value = Mathf.CeilToInt(math.pow(20f * (float)m_StorageBuildingDemand.value, 0.75f));
			m_IndustrialBuildingDemand.value = (flag ? (2 * m_IndustrialBuildingDemand.value / num13) : 0);
			ref NativeValue<int> reference12 = ref m_OfficeCompanyDemand;
			reference12.value *= 2 * m_OfficeCompanyDemand.value / num12;
			m_IndustrialBuildingDemand.value = math.clamp(m_IndustrialBuildingDemand.value, 0, 100);
			m_OfficeBuildingDemand.value = math.clamp(m_OfficeBuildingDemand.value, 0, 100);
		}

		private float MapAndClaimWorkforceEffect(float value, float min, float max)
		{
			if (value < 0f)
			{
				float num = math.unlerp(-2000f, 0f, value);
				num = math.clamp(num, 0f, 1f);
				return math.lerp(min, 0f, num);
			}
			float num2 = math.unlerp(0f, 20f, value);
			num2 = math.clamp(num2, 0f, 1f);
			return math.lerp(0f, max, num2);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CityServiceUpkeep> __Game_City_CityServiceUpkeep_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyOnMarket> __Game_Buildings_PropertyOnMarket_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Population> __Game_City_Population_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> __Game_Prefabs_ServiceUpkeepData_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_City_CityServiceUpkeep_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CityServiceUpkeep>(true);
			__Game_Buildings_PropertyOnMarket_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyOnMarket>(true);
			__Game_City_Population_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(true);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceUpkeepData>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
		}
	}

	private static readonly int kStorageProductionDemand = 2000;

	private static readonly int kStorageCompanyEstimateLimit = 864000;

	private ResourceSystem m_ResourceSystem;

	private CitySystem m_CitySystem;

	private ClimateSystem m_ClimateSystem;

	private TaxSystem m_TaxSystem;

	private CountHouseholdDataSystem m_CountHouseholdDataSystem;

	private CountWorkplacesSystem m_CountWorkplacesSystem;

	private CountCompanyDataSystem m_CountCompanyDataSystem;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_DemandParameterQuery;

	private EntityQuery m_IndustrialQuery;

	private EntityQuery m_OfficeQuery;

	private EntityQuery m_StorageCompanyQuery;

	private EntityQuery m_ProcessDataQuery;

	private EntityQuery m_CityServiceQuery;

	private EntityQuery m_UnlockedZoneDataQuery;

	private EntityQuery m_GameModeSettingQuery;

	private NativeValue<int> m_IndustrialCompanyDemand;

	private NativeValue<int> m_IndustrialBuildingDemand;

	private NativeValue<int> m_StorageCompanyDemand;

	private NativeValue<int> m_StorageBuildingDemand;

	private NativeValue<int> m_OfficeCompanyDemand;

	private NativeValue<int> m_OfficeBuildingDemand;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_ResourceDemands;

	[EnumArray(typeof(DemandFactor))]
	[DebugWatchValue]
	private NativeArray<int> m_IndustrialDemandFactors;

	[EnumArray(typeof(DemandFactor))]
	[DebugWatchValue]
	private NativeArray<int> m_OfficeDemandFactors;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_IndustrialCompanyDemands;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_IndustrialZoningDemands;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_IndustrialBuildingDemands;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_StorageBuildingDemands;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_StorageCompanyDemands;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_FreeProperties;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_FreeStorages;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_Storages;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_StorageCapacities;

	[DebugWatchDeps]
	private JobHandle m_WriteDependencies;

	private JobHandle m_ReadDependencies;

	private int m_LastIndustrialCompanyDemand;

	private int m_LastIndustrialBuildingDemand;

	private int m_LastStorageCompanyDemand;

	private int m_LastStorageBuildingDemand;

	private int m_LastOfficeCompanyDemand;

	private int m_LastOfficeBuildingDemand;

	private float m_IndustrialOfficeTaxEffectDemandOffset;

	private TypeHandle __TypeHandle;

	[DebugWatchValue(color = "#f7dc6f")]
	public int industrialCompanyDemand => m_LastIndustrialCompanyDemand;

	[DebugWatchValue(color = "#b7950b")]
	public int industrialBuildingDemand => m_LastIndustrialBuildingDemand;

	[DebugWatchValue(color = "#cccccc")]
	public int storageCompanyDemand => m_LastStorageCompanyDemand;

	[DebugWatchValue(color = "#999999")]
	public int storageBuildingDemand => m_LastStorageBuildingDemand;

	[DebugWatchValue(color = "#af7ac5")]
	public int officeCompanyDemand => m_LastOfficeCompanyDemand;

	[DebugWatchValue(color = "#6c3483")]
	public int officeBuildingDemand => m_LastOfficeBuildingDemand;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 7;
	}

	public NativeArray<int> GetConsumption(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_ResourceDemands;
	}

	public NativeArray<int> GetIndustrialDemandFactors(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_IndustrialDemandFactors;
	}

	public NativeArray<int> GetOfficeDemandFactors(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_OfficeDemandFactors;
	}

	public NativeArray<int> GetResourceDemands(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_IndustrialCompanyDemands;
	}

	public NativeArray<int> GetBuildingDemands(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_IndustrialBuildingDemands;
	}

	public NativeArray<int> GetStorageCompanyDemands(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_StorageCompanyDemands;
	}

	public NativeArray<int> GetStorageBuildingDemands(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_StorageBuildingDemands;
	}

	public NativeArray<int> GetIndustrialResourceDemands(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_ResourceDemands;
	}

	public void AddReader(JobHandle reader)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_ReadDependencies = JobHandle.CombineDependencies(m_ReadDependencies, reader);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		CollectionUtils.Fill<int>(m_ResourceDemands, 0);
		if (((EntityQuery)(ref m_GameModeSettingQuery)).IsEmptyIgnoreFilter)
		{
			m_IndustrialOfficeTaxEffectDemandOffset = 0f;
			return;
		}
		ModeSettingData singleton = ((EntityQuery)(ref m_GameModeSettingQuery)).GetSingleton<ModeSettingData>();
		if (singleton.m_Enable)
		{
			m_IndustrialOfficeTaxEffectDemandOffset = singleton.m_IndustrialOfficeTaxEffectDemandOffset;
		}
		else
		{
			m_IndustrialOfficeTaxEffectDemandOffset = 0f;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_CountHouseholdDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountHouseholdDataSystem>();
		m_CountWorkplacesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountWorkplacesSystem>();
		m_CountCompanyDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountCompanyDataSystem>();
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_DemandParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DemandParameterData>() });
		m_IndustrialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<IndustrialProperty>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Abandoned>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Condemned>()
		});
		m_OfficeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<OfficeProperty>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Abandoned>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Condemned>()
		});
		m_StorageCompanyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.Exclude<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_ProcessDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<IndustrialProcessData>(),
			ComponentType.Exclude<ServiceCompanyData>()
		});
		m_CityServiceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CityServiceUpkeep>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_UnlockedZoneDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ZoneData>(),
			ComponentType.Exclude<Locked>()
		});
		m_GameModeSettingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ModeSettingData>() });
		m_IndustrialCompanyDemand = new NativeValue<int>((Allocator)4);
		m_IndustrialBuildingDemand = new NativeValue<int>((Allocator)4);
		m_StorageCompanyDemand = new NativeValue<int>((Allocator)4);
		m_StorageBuildingDemand = new NativeValue<int>((Allocator)4);
		m_OfficeCompanyDemand = new NativeValue<int>((Allocator)4);
		m_OfficeBuildingDemand = new NativeValue<int>((Allocator)4);
		m_IndustrialDemandFactors = new NativeArray<int>(18, (Allocator)4, (NativeArrayOptions)1);
		m_OfficeDemandFactors = new NativeArray<int>(18, (Allocator)4, (NativeArrayOptions)1);
		int resourceCount = EconomyUtils.ResourceCount;
		m_IndustrialCompanyDemands = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_IndustrialZoningDemands = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_IndustrialBuildingDemands = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_ResourceDemands = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_StorageBuildingDemands = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_StorageCompanyDemands = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_FreeProperties = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_FreeStorages = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_Storages = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_StorageCapacities = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_IndustrialOfficeTaxEffectDemandOffset = 0f;
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_DemandParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_ProcessDataQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_IndustrialCompanyDemand.Dispose();
		m_IndustrialBuildingDemand.Dispose();
		m_StorageCompanyDemand.Dispose();
		m_StorageBuildingDemand.Dispose();
		m_OfficeCompanyDemand.Dispose();
		m_OfficeBuildingDemand.Dispose();
		m_IndustrialDemandFactors.Dispose();
		m_OfficeDemandFactors.Dispose();
		m_IndustrialCompanyDemands.Dispose();
		m_IndustrialZoningDemands.Dispose();
		m_IndustrialBuildingDemands.Dispose();
		m_StorageBuildingDemands.Dispose();
		m_StorageCompanyDemands.Dispose();
		m_ResourceDemands.Dispose();
		m_FreeProperties.Dispose();
		m_Storages.Dispose();
		m_FreeStorages.Dispose();
		m_StorageCapacities.Dispose();
		base.OnDestroy();
	}

	public void SetDefaults(Context context)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		m_IndustrialCompanyDemand.value = 0;
		m_IndustrialBuildingDemand.value = 0;
		m_StorageCompanyDemand.value = 0;
		m_StorageBuildingDemand.value = 0;
		m_OfficeCompanyDemand.value = 0;
		m_OfficeBuildingDemand.value = 0;
		CollectionUtils.Fill<int>(m_IndustrialDemandFactors, 0);
		CollectionUtils.Fill<int>(m_OfficeDemandFactors, 0);
		CollectionUtils.Fill<int>(m_IndustrialCompanyDemands, 0);
		CollectionUtils.Fill<int>(m_IndustrialZoningDemands, 0);
		CollectionUtils.Fill<int>(m_IndustrialBuildingDemands, 0);
		CollectionUtils.Fill<int>(m_StorageBuildingDemands, 0);
		CollectionUtils.Fill<int>(m_StorageCompanyDemands, 0);
		CollectionUtils.Fill<int>(m_FreeProperties, 0);
		CollectionUtils.Fill<int>(m_Storages, 0);
		CollectionUtils.Fill<int>(m_FreeStorages, 0);
		m_LastIndustrialCompanyDemand = 0;
		m_LastIndustrialBuildingDemand = 0;
		m_LastStorageCompanyDemand = 0;
		m_LastStorageBuildingDemand = 0;
		m_LastOfficeCompanyDemand = 0;
		m_LastOfficeBuildingDemand = 0;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		int value = m_IndustrialCompanyDemand.value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value);
		int value2 = m_IndustrialBuildingDemand.value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value2);
		int value3 = m_StorageCompanyDemand.value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value3);
		int value4 = m_StorageBuildingDemand.value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value4);
		int value5 = m_OfficeCompanyDemand.value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value5);
		int value6 = m_OfficeBuildingDemand.value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value6);
		int length = m_IndustrialDemandFactors.Length;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(length);
		NativeArray<int> val = m_IndustrialDemandFactors;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		NativeArray<int> val2 = m_OfficeDemandFactors;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val2);
		NativeArray<int> val3 = m_IndustrialCompanyDemands;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val3);
		NativeArray<int> val4 = m_IndustrialZoningDemands;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val4);
		NativeArray<int> val5 = m_IndustrialBuildingDemands;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val5);
		NativeArray<int> val6 = m_StorageBuildingDemands;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val6);
		NativeArray<int> val7 = m_StorageCompanyDemands;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val7);
		NativeArray<int> val8 = m_FreeProperties;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val8);
		NativeArray<int> val9 = m_Storages;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val9);
		NativeArray<int> val10 = m_FreeStorages;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val10);
		int num = m_LastIndustrialCompanyDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int num2 = m_LastIndustrialBuildingDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		int num3 = m_LastStorageCompanyDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num3);
		int num4 = m_LastStorageBuildingDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num4);
		int num5 = m_LastOfficeCompanyDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num5);
		int num6 = m_LastOfficeBuildingDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num6);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		int value = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref value);
		m_IndustrialCompanyDemand.value = value;
		int value2 = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref value2);
		m_IndustrialBuildingDemand.value = value2;
		int value3 = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref value3);
		m_StorageCompanyDemand.value = value3;
		int value4 = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref value4);
		m_StorageBuildingDemand.value = value4;
		int value5 = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref value5);
		m_OfficeCompanyDemand.value = value5;
		int value6 = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref value6);
		m_OfficeBuildingDemand.value = value6;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.demandFactorCountSerialization)
		{
			NativeArray<int> val = default(NativeArray<int>);
			val._002Ector(13, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val2 = val;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val2);
			CollectionUtils.CopySafe<int>(val, m_IndustrialDemandFactors);
			NativeArray<int> val3 = val;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val3);
			CollectionUtils.CopySafe<int>(val, m_OfficeDemandFactors);
			val.Dispose();
		}
		else
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			if (num == m_IndustrialDemandFactors.Length)
			{
				NativeArray<int> val4 = m_IndustrialDemandFactors;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val4);
				NativeArray<int> val5 = m_OfficeDemandFactors;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val5);
			}
			else
			{
				NativeArray<int> val6 = default(NativeArray<int>);
				val6._002Ector(num, (Allocator)2, (NativeArrayOptions)1);
				NativeArray<int> val7 = val6;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val7);
				CollectionUtils.CopySafe<int>(val6, m_IndustrialDemandFactors);
				NativeArray<int> val8 = val6;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val8);
				CollectionUtils.CopySafe<int>(val6, m_OfficeDemandFactors);
				val6.Dispose();
			}
		}
		context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.FishResource))
		{
			NativeArray<int> val9 = m_IndustrialCompanyDemands;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val9);
			NativeArray<int> val10 = m_IndustrialZoningDemands;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val10);
			NativeArray<int> val11 = m_IndustrialBuildingDemands;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val11);
			NativeArray<int> val12 = m_StorageBuildingDemands;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val12);
			NativeArray<int> val13 = m_StorageCompanyDemands;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val13);
		}
		else
		{
			NativeArray<int> subArray = m_IndustrialCompanyDemands.GetSubArray(0, 40);
			((IReader)reader/*cast due to .constrained prefix*/).Read(subArray);
			NativeArray<int> subArray2 = m_IndustrialZoningDemands.GetSubArray(0, 40);
			((IReader)reader/*cast due to .constrained prefix*/).Read(subArray2);
			NativeArray<int> subArray3 = m_IndustrialBuildingDemands.GetSubArray(0, 40);
			((IReader)reader/*cast due to .constrained prefix*/).Read(subArray3);
			NativeArray<int> subArray4 = m_StorageBuildingDemands.GetSubArray(0, 40);
			((IReader)reader/*cast due to .constrained prefix*/).Read(subArray4);
			NativeArray<int> subArray5 = m_StorageCompanyDemands.GetSubArray(0, 40);
			((IReader)reader/*cast due to .constrained prefix*/).Read(subArray5);
			m_IndustrialCompanyDemands[40] = 0;
			m_IndustrialZoningDemands[40] = 0;
			m_IndustrialBuildingDemands[40] = 0;
			m_StorageBuildingDemands[40] = 0;
			m_StorageCompanyDemands[40] = 0;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version <= Version.companyDemandOptimization)
		{
			NativeArray<int> val14 = default(NativeArray<int>);
			val14._002Ector(40, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val15 = val14;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val15);
			NativeArray<int> val16 = val14;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val16);
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version <= Version.demandFactorCountSerialization)
			{
				NativeArray<int> val17 = val14;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val17);
				NativeArray<int> val18 = val14;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val18);
			}
			NativeArray<int> val19 = val14;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val19);
			NativeArray<int> val20 = val14;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val20);
		}
		context = ((IReader)reader).context;
		format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.FishResource))
		{
			NativeArray<int> val21 = m_FreeProperties;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val21);
			NativeArray<int> val22 = m_Storages;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val22);
			NativeArray<int> val23 = m_FreeStorages;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val23);
		}
		else
		{
			NativeArray<int> subArray6 = m_FreeProperties.GetSubArray(0, 40);
			((IReader)reader/*cast due to .constrained prefix*/).Read(subArray6);
			NativeArray<int> subArray7 = m_Storages.GetSubArray(0, 40);
			((IReader)reader/*cast due to .constrained prefix*/).Read(subArray7);
			NativeArray<int> subArray8 = m_FreeStorages.GetSubArray(0, 40);
			((IReader)reader/*cast due to .constrained prefix*/).Read(subArray8);
			m_FreeProperties[40] = 0;
			m_Storages[40] = 0;
			m_FreeStorages[40] = 0;
		}
		ref int reference = ref m_LastIndustrialCompanyDemand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref int reference2 = ref m_LastIndustrialBuildingDemand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
		ref int reference3 = ref m_LastStorageCompanyDemand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference3);
		ref int reference4 = ref m_LastStorageBuildingDemand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference4);
		ref int reference5 = ref m_LastOfficeCompanyDemand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference5);
		ref int reference6 = ref m_LastOfficeBuildingDemand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference6);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_DemandParameterQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_EconomyParameterQuery)).IsEmptyIgnoreFilter)
		{
			m_LastIndustrialCompanyDemand = m_IndustrialCompanyDemand.value;
			m_LastIndustrialBuildingDemand = m_IndustrialBuildingDemand.value;
			m_LastStorageCompanyDemand = m_StorageCompanyDemand.value;
			m_LastStorageBuildingDemand = m_StorageBuildingDemand.value;
			m_LastOfficeCompanyDemand = m_OfficeCompanyDemand.value;
			m_LastOfficeBuildingDemand = m_OfficeBuildingDemand.value;
			JobHandle deps;
			CountCompanyDataSystem.IndustrialCompanyDatas industrialCompanyDatas = m_CountCompanyDataSystem.GetIndustrialCompanyDatas(out deps);
			JobHandle val = default(JobHandle);
			JobHandle val2 = default(JobHandle);
			JobHandle val3 = default(JobHandle);
			JobHandle val4 = default(JobHandle);
			UpdateIndustrialDemandJob updateIndustrialDemandJob = new UpdateIndustrialDemandJob
			{
				m_IndustrialPropertyChunks = ((EntityQuery)(ref m_IndustrialQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
				m_OfficePropertyChunks = ((EntityQuery)(ref m_OfficeQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
				m_StorageCompanyChunks = ((EntityQuery)(ref m_StorageCompanyQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val3),
				m_CityServiceChunks = ((EntityQuery)(ref m_CityServiceQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val4),
				m_UnlockedZoneDatas = ((EntityQuery)(ref m_UnlockedZoneDataQuery)).ToComponentDataArray<ZoneData>(AllocatorHandle.op_Implicit((Allocator)3)),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceUpkeepType = InternalCompilerInterface.GetComponentTypeHandle<CityServiceUpkeep>(ref __TypeHandle.__Game_City_CityServiceUpkeep_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyOnMarketType = InternalCompilerInterface.GetComponentTypeHandle<PropertyOnMarket>(ref __TypeHandle.__Game_Buildings_PropertyOnMarket_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Populations = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StorageLimitDatas = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnableBuildingDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingPropertyDatas = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IndustrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Attached = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceUpkeeps = InternalCompilerInterface.GetBufferLookup<ServiceUpkeepData>(ref __TypeHandle.__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Upkeeps = InternalCompilerInterface.GetBufferLookup<ServiceUpkeepData>(ref __TypeHandle.__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DemandParameters = ((EntityQuery)(ref m_DemandParameterQuery)).GetSingleton<DemandParameterData>(),
				m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
				m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
				m_EmployableByEducation = m_CountHouseholdDataSystem.GetEmployables(),
				m_TaxRates = m_TaxSystem.GetTaxRates(),
				m_FreeWorkplaces = m_CountWorkplacesSystem.GetFreeWorkplaces(),
				m_City = m_CitySystem.City,
				m_IndustrialCompanyDemand = m_IndustrialCompanyDemand,
				m_IndustrialBuildingDemand = m_IndustrialBuildingDemand,
				m_StorageCompanyDemand = m_StorageCompanyDemand,
				m_StorageBuildingDemand = m_StorageBuildingDemand,
				m_OfficeCompanyDemand = m_OfficeCompanyDemand,
				m_OfficeBuildingDemand = m_OfficeBuildingDemand,
				m_IndustrialCompanyDemands = m_IndustrialCompanyDemands,
				m_IndustrialBuildingDemands = m_IndustrialBuildingDemands,
				m_StorageBuildingDemands = m_StorageBuildingDemands,
				m_StorageCompanyDemands = m_StorageCompanyDemands,
				m_Propertyless = industrialCompanyDatas.m_ProductionPropertyless,
				m_CompanyResourceDemands = industrialCompanyDatas.m_Demand,
				m_FreeProperties = m_FreeProperties,
				m_Productions = industrialCompanyDatas.m_Production,
				m_Storages = m_Storages,
				m_FreeStorages = m_FreeStorages,
				m_StorageCapacities = m_StorageCapacities,
				m_IndustrialDemandFactors = m_IndustrialDemandFactors,
				m_OfficeDemandFactors = m_OfficeDemandFactors,
				m_ResourceDemands = m_ResourceDemands,
				m_IndustrialOfficeTaxEffectDemandOffset = m_IndustrialOfficeTaxEffectDemandOffset
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<UpdateIndustrialDemandJob>(updateIndustrialDemandJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, m_ReadDependencies, val, val2, deps, val3, val4));
			m_WriteDependencies = ((SystemBase)this).Dependency;
			m_CountCompanyDataSystem.AddReader(((SystemBase)this).Dependency);
			m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
			m_TaxSystem.AddReader(((SystemBase)this).Dependency);
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
	public IndustrialDemandSystem()
	{
	}
}

using Game.Agents;
using Game.Areas;
using Game.Citizens;
using Game.City;
using Game.Companies;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Zones;
using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Buildings;

public static class PropertyUtils
{
	[BurstCompile]
	public struct ExtractorFindCompanyJob : IJob
	{
		[ReadOnly]
		public NativeList<Entity> m_Entities;

		[ReadOnly]
		public NativeList<Entity> m_ExtractorCompanyEntities;

		[ReadOnly]
		public NativeList<Entity> m_CompanyPrefabs;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_Properties;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_Processes;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> m_WorkplaceDatas;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<Attached> m_Attached;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> m_Lots;

		[ReadOnly]
		public ComponentLookup<Geometry> m_Geometries;

		[ReadOnly]
		public ComponentLookup<Extractor> m_ExtractorAreas;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> m_ExtractorDatas;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		public ExtractorParameterData m_ExtractorParameters;

		public EconomyParameterData m_EconomyParameters;

		public NativeQueue<RentAction> m_RentActionQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public float m_AverageTemperature;

		[ReadOnly]
		public NativeArray<int> m_Productions;

		[ReadOnly]
		public NativeArray<int> m_Consumptions;

		private float Evaluate(Entity entity, Resource resource)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			IndustrialProcessData industrialProcessData = default(IndustrialProcessData);
			ResourceData resourceData = m_ResourceDatas[m_ResourcePrefabs[resource]];
			bool flag = false;
			for (int i = 0; i < m_CompanyPrefabs.Length; i++)
			{
				if (m_Processes.HasComponent(m_CompanyPrefabs[i]) && m_WorkplaceDatas.HasComponent(m_CompanyPrefabs[i]))
				{
					industrialProcessData = m_Processes[m_CompanyPrefabs[i]];
					if (industrialProcessData.m_Output.m_Resource == resource && industrialProcessData.m_Input1.m_Resource == Resource.NoResource)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag && m_Attached.HasComponent(entity))
			{
				Entity parent = m_Attached[entity].m_Parent;
				ExtractorCompanySystem.GetBestConcentration(resource, parent, ref m_SubAreas, ref m_InstalledUpgrades, ref m_ExtractorAreas, ref m_Geometries, ref m_Prefabs, ref m_ExtractorDatas, m_ExtractorParameters, m_ResourcePrefabs, ref m_ResourceDatas, out var concentration, out var _);
				if (resourceData.m_RequireTemperature && m_AverageTemperature < resourceData.m_RequiredTemperature)
				{
					concentration = 0f;
				}
				if (concentration == 0f)
				{
					return float.NegativeInfinity;
				}
				return concentration;
			}
			return float.NegativeInfinity;
		}

		public void Execute()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			if (m_Entities.Length == 0)
			{
				return;
			}
			Attached attached = default(Attached);
			PrefabRef prefabRef = default(PrefabRef);
			BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
			for (int i = 0; i < m_Entities.Length; i++)
			{
				Entity val = m_Entities[i];
				if (!m_Prefabs.HasComponent(val))
				{
					continue;
				}
				Entity prefab = m_Prefabs[val].m_Prefab;
				Resource resource = Resource.NoResource;
				if (m_Properties.HasComponent(prefab))
				{
					Resource resource2 = m_Properties[prefab].m_AllowedManufactured;
					if (m_Attached.TryGetComponent(val, ref attached) && m_Prefabs.TryGetComponent(attached.m_Parent, ref prefabRef) && m_Properties.TryGetComponent(prefabRef.m_Prefab, ref buildingPropertyData))
					{
						resource2 &= buildingPropertyData.m_AllowedManufactured;
					}
					ResourceIterator resourceIterator = default(ResourceIterator);
					float num = float.NegativeInfinity;
					while (resourceIterator.Next())
					{
						if ((resource2 & resourceIterator.resource) != Resource.NoResource)
						{
							float num2 = Evaluate(val, resourceIterator.resource);
							if (num2 > num)
							{
								num = num2;
								resource = resourceIterator.resource;
							}
						}
					}
				}
				for (int j = 0; j < m_ExtractorCompanyEntities.Length; j++)
				{
					Entity val2 = m_ExtractorCompanyEntities[j];
					if ((!m_PropertyRenters.HasComponent(val2) || !(m_PropertyRenters[val2].m_Property != Entity.Null)) && m_Prefabs.HasComponent(val2))
					{
						Entity prefab2 = m_Prefabs[val2].m_Prefab;
						if (m_Processes.HasComponent(prefab2) && m_Processes[prefab2].m_Output.m_Resource == resource)
						{
							m_RentActionQueue.Enqueue(new RentAction
							{
								m_Property = val,
								m_Renter = val2
							});
							((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponentEnabled<PropertySeeker>(val2, false);
							return;
						}
					}
				}
			}
		}
	}

	[BurstCompile]
	public struct CompanyFindPropertyJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<CompanyData> m_CompanyDataType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		public ComponentTypeHandle<PropertySeeker> m_PropertySeekerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Companies.StorageCompany> m_StorageCompanyType;

		[ReadOnly]
		public NativeList<Entity> m_FreePropertyEntities;

		[ReadOnly]
		public NativeList<PrefabRef> m_PropertyPrefabs;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyDatas;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> m_Availabilities;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public ComponentLookup<PropertyOnMarket> m_PropertiesOnMarket;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabFromEntity;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableDatas;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> m_WorkplaceDatas;

		[ReadOnly]
		public ComponentLookup<LandValue> m_LandValues;

		[ReadOnly]
		public ComponentLookup<ServiceCompanyData> m_ServiceCompanies;

		[ReadOnly]
		public ComponentLookup<CommercialCompany> m_CommercialCompanies;

		[ReadOnly]
		public ComponentLookup<Signature> m_Signatures;

		[ReadOnly]
		public BufferLookup<Renter> m_Renters;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		public EconomyParameterData m_EconomyParameters;

		public ZonePreferenceData m_ZonePreferences;

		public bool m_Commercial;

		public ParallelWriter<RentAction> m_RentActionQueue;

		public ParallelWriter m_CommandBuffer;

		private void Evaluate(int index, Entity company, ref ServiceCompanyData service, ref IndustrialProcessData process, Entity property, ref PropertySeeker propertySeeker, bool commercial, bool storage)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			float num = ((!commercial) ? IndustrialFindPropertySystem.Evaluate(company, property, ref process, ref propertySeeker, m_Buildings, m_PropertiesOnMarket, m_PrefabFromEntity, m_BuildingDatas, m_SpawnableDatas, m_WorkplaceDatas, m_LandValues, m_Availabilities, m_EconomyParameters, m_ResourcePrefabs, m_ResourceDatas, m_BuildingPropertyDatas, storage) : CommercialFindPropertySystem.Evaluate(company, property, ref service, ref process, ref propertySeeker, m_Buildings, m_PrefabFromEntity, m_BuildingDatas, m_Availabilities, m_LandValues, m_ResourcePrefabs, m_ResourceDatas, m_BuildingPropertyDatas, m_SpawnableDatas, m_Renters, m_CommercialCompanies, ref m_ZonePreferences));
			if (m_Signatures.HasComponent(property))
			{
				num += 5000f;
			}
			if (propertySeeker.m_BestProperty == Entity.Null || num > propertySeeker.m_BestPropertyScore)
			{
				propertySeeker.m_BestPropertyScore = num;
				propertySeeker.m_BestProperty = property;
			}
		}

		private void SelectProperty(int jobIndex, Entity company, ref PropertySeeker propertySeeker, bool storage)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			Entity bestProperty = propertySeeker.m_BestProperty;
			if (m_PropertiesOnMarket.HasComponent(bestProperty))
			{
				if (m_PropertyRenters.HasComponent(company))
				{
					PropertyRenter propertyRenter = m_PropertyRenters[company];
					if (((Entity)(ref propertyRenter.m_Property)).Equals(bestProperty))
					{
						goto IL_0081;
					}
				}
				m_RentActionQueue.Enqueue(new RentAction
				{
					m_Property = bestProperty,
					m_Renter = company,
					m_Flags = (storage ? RentActionFlags.Storage : ((RentActionFlags)0))
				});
				((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<PropertySeeker>(jobIndex, company, false);
				return;
			}
			goto IL_0081;
			IL_0081:
			if (m_PropertyRenters.HasComponent(company))
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<PropertySeeker>(jobIndex, company, false);
				return;
			}
			propertySeeker.m_BestProperty = Entity.Null;
			propertySeeker.m_BestPropertyScore = 0f;
		}

		private bool PropertyAllowsResource(int index, Resource resource, Resource input, bool storage)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			Entity prefab = m_PropertyPrefabs[index].m_Prefab;
			BuildingPropertyData buildingPropertyData = m_BuildingPropertyDatas[prefab];
			Resource resource2 = ((!storage) ? (m_Commercial ? buildingPropertyData.m_AllowedSold : buildingPropertyData.m_AllowedManufactured) : buildingPropertyData.m_AllowedStored);
			if ((resource & resource2) != Resource.NoResource)
			{
				return (input & buildingPropertyData.m_AllowedInput) == input;
			}
			return false;
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
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<PropertySeeker> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertySeeker>(ref m_PropertySeekerType);
			((ArchetypeChunk)(ref chunk)).GetNativeArray<CompanyData>(ref m_CompanyDataType);
			bool storage = ((ArchetypeChunk)(ref chunk)).Has<Game.Companies.StorageCompany>(ref m_StorageCompanyType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				if (!m_IndustrialProcessDatas.HasComponent(prefab))
				{
					break;
				}
				IndustrialProcessData process = m_IndustrialProcessDatas[prefab];
				PropertySeeker propertySeeker = nativeArray3[i];
				Resource resource = process.m_Output.m_Resource;
				Resource input = process.m_Input1.m_Resource | process.m_Input2.m_Resource;
				ServiceCompanyData service = default(ServiceCompanyData);
				if (m_Commercial)
				{
					service = m_ServiceCompanies[prefab];
				}
				if (m_PropertyRenters.HasComponent(val) && m_PropertyRenters[val].m_Property != Entity.Null)
				{
					continue;
				}
				for (int j = 0; j < m_FreePropertyEntities.Length; j++)
				{
					if (PropertyAllowsResource(j, resource, input, storage))
					{
						Evaluate(i, val, ref service, ref process, m_FreePropertyEntities[j], ref propertySeeker, m_Commercial, storage);
					}
				}
				SelectProperty(unfilteredChunkIndex, val, ref propertySeeker, storage);
				nativeArray3[i] = propertySeeker;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public static readonly float kHomelessApartmentSize = 0.01f;

	public static int GetRentPricePerRenter(BuildingPropertyData buildingPropertyData, int buildingLevel, int lotSize, float landValueBase, Game.Zones.AreaType areaType, ref EconomyParameterData economyParameterData, bool ignoreLandValue = false)
	{
		float num = economyParameterData.m_RentPriceBuildingZoneTypeBase.x;
		float num2 = economyParameterData.m_LandValueModifier.x;
		switch (areaType)
		{
		case Game.Zones.AreaType.Commercial:
			num = economyParameterData.m_RentPriceBuildingZoneTypeBase.y;
			num2 = economyParameterData.m_LandValueModifier.y;
			break;
		case Game.Zones.AreaType.Industrial:
			num = economyParameterData.m_RentPriceBuildingZoneTypeBase.z;
			num2 = economyParameterData.m_LandValueModifier.z;
			break;
		}
		float num3 = ((!ignoreLandValue) ? ((landValueBase * num2 + num * (float)buildingLevel) * (float)lotSize * buildingPropertyData.m_SpaceMultiplier) : (num * (float)buildingLevel * (float)lotSize * buildingPropertyData.m_SpaceMultiplier));
		float num4 = ((!IsMixedBuilding(buildingPropertyData)) ? ((float)buildingPropertyData.CountProperties()) : ((float)Mathf.RoundToInt((float)buildingPropertyData.m_ResidentialProperties / (1f - economyParameterData.m_MixedBuildingCompanyRentPercentage))));
		return Mathf.RoundToInt(num3 / num4);
	}

	public static float GetPropertyScore(Entity property, Entity household, DynamicBuffer<HouseholdCitizen> citizenBuffer, ref ComponentLookup<PrefabRef> prefabRefs, ref ComponentLookup<BuildingPropertyData> buildingProperties, ref ComponentLookup<Building> buildings, ref ComponentLookup<BuildingData> buildingDatas, ref ComponentLookup<Household> households, ref ComponentLookup<Citizen> citizens, ref ComponentLookup<Game.Citizens.Student> students, ref ComponentLookup<Worker> workers, ref ComponentLookup<SpawnableBuildingData> spawnableDatas, ref ComponentLookup<CrimeProducer> crimes, ref BufferLookup<Game.Net.ServiceCoverage> serviceCoverages, ref ComponentLookup<Locked> locked, ref ComponentLookup<ElectricityConsumer> electricityConsumers, ref ComponentLookup<WaterConsumer> waterConsumers, ref ComponentLookup<GarbageProducer> garbageProducers, ref ComponentLookup<MailProducer> mailProducers, ref ComponentLookup<Transform> transforms, ref ComponentLookup<Abandoned> abandoneds, ref ComponentLookup<Park> parks, ref BufferLookup<ResourceAvailability> availabilities, NativeArray<int> taxRates, NativeArray<GroundPollution> pollutionMap, NativeArray<AirPollution> airPollutionMap, NativeArray<NoisePollution> noiseMap, CellMapData<TelecomCoverage> telecomCoverages, DynamicBuffer<CityModifier> cityModifiers, Entity healthcareService, Entity entertainmentService, Entity educationService, Entity telecomService, Entity garbageService, Entity policeService, CitizenHappinessParameterData citizenHappinessParameterData, GarbageParameterData garbageParameterData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		if (!buildings.HasComponent(property))
		{
			return float.NegativeInfinity;
		}
		bool flag = (households[household].m_Flags & HouseholdFlags.MovedIn) != 0;
		bool flag2 = BuildingUtils.IsHomelessShelterBuilding(property, ref parks, ref abandoneds);
		if (flag2 && !flag)
		{
			return float.NegativeInfinity;
		}
		Building buildingData = buildings[property];
		Entity prefab = prefabRefs[property].m_Prefab;
		HouseholdFindPropertySystem.GenericApartmentQuality genericApartmentQuality = GetGenericApartmentQuality(property, prefab, ref buildingData, ref buildingProperties, ref buildingDatas, ref spawnableDatas, ref crimes, ref serviceCoverages, ref locked, ref electricityConsumers, ref waterConsumers, ref garbageProducers, ref mailProducers, ref transforms, ref abandoneds, pollutionMap, airPollutionMap, noiseMap, telecomCoverages, cityModifiers, healthcareService, entertainmentService, educationService, telecomService, garbageService, policeService, citizenHappinessParameterData, garbageParameterData);
		int length = citizenBuffer.Length;
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		for (int i = 0; i < citizenBuffer.Length; i++)
		{
			Entity citizen = citizenBuffer[i].m_Citizen;
			Citizen citizen2 = citizens[citizen];
			num4 += citizen2.Happiness;
			if (citizen2.GetAge() == CitizenAge.Child)
			{
				num5++;
			}
			else
			{
				num3++;
				num6 += CitizenHappinessSystem.GetTaxBonuses(citizen2.GetEducationLevel(), taxRates, in citizenHappinessParameterData).y;
			}
			if (students.HasComponent(citizen))
			{
				num2++;
				Game.Citizens.Student student = students[citizen];
				if (student.m_School != property)
				{
					num += student.m_LastCommuteTime;
				}
			}
			else if (workers.HasComponent(citizen))
			{
				num2++;
				Worker worker = workers[citizen];
				if (worker.m_Workplace != property)
				{
					num += worker.m_LastCommuteTime;
				}
			}
		}
		if (num2 > 0)
		{
			num /= (float)num2;
		}
		if (citizenBuffer.Length > 0)
		{
			num4 /= citizenBuffer.Length;
			if (num3 > 0)
			{
				num6 /= num3;
			}
		}
		float serviceAvailability = GetServiceAvailability(buildingData.m_RoadEdge, buildingData.m_CurvePosition, availabilities);
		float cachedApartmentQuality = GetCachedApartmentQuality(length, num5, num4, genericApartmentQuality);
		float num7 = (flag2 ? (-1000) : 0);
		return serviceAvailability + cachedApartmentQuality + (float)(2 * num6) - num + num7;
	}

	public static float GetServiceAvailability(Entity roadEdge, float curvePos, BufferLookup<ResourceAvailability> availabilities)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (availabilities.HasBuffer(roadEdge))
		{
			return NetUtils.GetAvailability(availabilities[roadEdge], AvailableResource.Services, curvePos);
		}
		return 0f;
	}

	public static int2 GetElectricityBonusForApartmentQuality(Entity building, ref ComponentLookup<ElectricityConsumer> electricityConsumers, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ElectricityConsumer electricityConsumer = default(ElectricityConsumer);
		if (electricityConsumers.TryGetComponent(building, ref electricityConsumer) && !electricityConsumer.electricityConnected)
		{
			return new int2
			{
				y = (int)math.round(0f - data.m_ElectricityWellbeingPenalty)
			};
		}
		return default(int2);
	}

	public static int2 GetWaterBonusForApartmentQuality(Entity building, ref ComponentLookup<WaterConsumer> waterConsumers, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		WaterConsumer waterConsumer = default(WaterConsumer);
		if (waterConsumers.TryGetComponent(building, ref waterConsumer) && !waterConsumer.waterConnected)
		{
			return new int2
			{
				x = (int)math.round((float)(-data.m_WaterHealthPenalty)),
				y = (int)math.round((float)(-data.m_WaterWellbeingPenalty))
			};
		}
		return default(int2);
	}

	public static int2 GetSewageBonusForApartmentQuality(Entity building, ref ComponentLookup<WaterConsumer> waterConsumers, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		WaterConsumer waterConsumer = default(WaterConsumer);
		if (waterConsumers.TryGetComponent(building, ref waterConsumer) && !waterConsumer.sewageConnected)
		{
			return new int2
			{
				x = (int)math.round((float)(-data.m_SewageHealthEffect)),
				y = (int)math.round((float)(-data.m_SewageWellbeingEffect))
			};
		}
		return default(int2);
	}

	public static HouseholdFindPropertySystem.GenericApartmentQuality GetGenericApartmentQuality(Entity building, Entity buildingPrefab, ref Building buildingData, ref ComponentLookup<BuildingPropertyData> buildingProperties, ref ComponentLookup<BuildingData> buildingDatas, ref ComponentLookup<SpawnableBuildingData> spawnableDatas, ref ComponentLookup<CrimeProducer> crimes, ref BufferLookup<Game.Net.ServiceCoverage> serviceCoverages, ref ComponentLookup<Locked> locked, ref ComponentLookup<ElectricityConsumer> electricityConsumers, ref ComponentLookup<WaterConsumer> waterConsumers, ref ComponentLookup<GarbageProducer> garbageProducers, ref ComponentLookup<MailProducer> mailProducers, ref ComponentLookup<Transform> transforms, ref ComponentLookup<Abandoned> abandoneds, NativeArray<GroundPollution> pollutionMap, NativeArray<AirPollution> airPollutionMap, NativeArray<NoisePollution> noiseMap, CellMapData<TelecomCoverage> telecomCoverages, DynamicBuffer<CityModifier> cityModifiers, Entity healthcareService, Entity entertainmentService, Entity educationService, Entity telecomService, Entity garbageService, Entity policeService, CitizenHappinessParameterData happinessParameterData, GarbageParameterData garbageParameterData)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		HouseholdFindPropertySystem.GenericApartmentQuality result = default(HouseholdFindPropertySystem.GenericApartmentQuality);
		bool flag = true;
		BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
		SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
		if (buildingProperties.HasComponent(buildingPrefab))
		{
			buildingPropertyData = buildingProperties[buildingPrefab];
			flag = false;
		}
		BuildingData buildingData2 = buildingDatas[buildingPrefab];
		if (spawnableDatas.HasComponent(buildingPrefab) && !abandoneds.HasComponent(building))
		{
			spawnableBuildingData = spawnableDatas[buildingPrefab];
		}
		else
		{
			flag = true;
		}
		result.apartmentSize = (flag ? kHomelessApartmentSize : (buildingPropertyData.m_SpaceMultiplier * (float)buildingData2.m_LotSize.x * (float)buildingData2.m_LotSize.y / math.max(1f, (float)buildingPropertyData.m_ResidentialProperties)));
		result.level = spawnableBuildingData.m_Level;
		int2 val = default(int2);
		int2 healthcareBonuses;
		if (serviceCoverages.HasBuffer(buildingData.m_RoadEdge))
		{
			DynamicBuffer<Game.Net.ServiceCoverage> serviceCoverage = serviceCoverages[buildingData.m_RoadEdge];
			healthcareBonuses = CitizenHappinessSystem.GetHealthcareBonuses(buildingData.m_CurvePosition, serviceCoverage, ref locked, healthcareService, in happinessParameterData);
			val += healthcareBonuses;
			healthcareBonuses = CitizenHappinessSystem.GetEntertainmentBonuses(buildingData.m_CurvePosition, serviceCoverage, cityModifiers, ref locked, entertainmentService, in happinessParameterData);
			val += healthcareBonuses;
			result.welfareBonus = CitizenHappinessSystem.GetWelfareValue(buildingData.m_CurvePosition, serviceCoverage, in happinessParameterData);
			result.educationBonus = float2.op_Implicit(CitizenHappinessSystem.GetEducationBonuses(buildingData.m_CurvePosition, serviceCoverage, ref locked, educationService, in happinessParameterData, 1));
		}
		int2 crimeBonuses = CitizenHappinessSystem.GetCrimeBonuses(default(CrimeVictim), building, ref crimes, ref locked, policeService, in happinessParameterData);
		healthcareBonuses = (int2)(flag ? new int2(0, -happinessParameterData.m_MaxCrimePenalty - crimeBonuses.y) : crimeBonuses);
		val += healthcareBonuses;
		healthcareBonuses = CitizenHappinessSystem.GetGroundPollutionBonuses(building, ref transforms, pollutionMap, cityModifiers, in happinessParameterData);
		val += healthcareBonuses;
		healthcareBonuses = CitizenHappinessSystem.GetAirPollutionBonuses(building, ref transforms, airPollutionMap, cityModifiers, in happinessParameterData);
		val += healthcareBonuses;
		healthcareBonuses = CitizenHappinessSystem.GetNoiseBonuses(building, ref transforms, noiseMap, in happinessParameterData);
		val += healthcareBonuses;
		healthcareBonuses = CitizenHappinessSystem.GetTelecomBonuses(building, ref transforms, telecomCoverages, ref locked, telecomService, in happinessParameterData);
		val += healthcareBonuses;
		healthcareBonuses = GetElectricityBonusForApartmentQuality(building, ref electricityConsumers, in happinessParameterData);
		val += healthcareBonuses;
		healthcareBonuses = GetWaterBonusForApartmentQuality(building, ref waterConsumers, in happinessParameterData);
		val += healthcareBonuses;
		healthcareBonuses = GetSewageBonusForApartmentQuality(building, ref waterConsumers, in happinessParameterData);
		val += healthcareBonuses;
		healthcareBonuses = CitizenHappinessSystem.GetWaterPollutionBonuses(building, ref waterConsumers, cityModifiers, in happinessParameterData);
		val += healthcareBonuses;
		healthcareBonuses = CitizenHappinessSystem.GetGarbageBonuses(building, ref garbageProducers, ref locked, garbageService, in garbageParameterData);
		val += healthcareBonuses;
		healthcareBonuses = CitizenHappinessSystem.GetMailBonuses(building, ref mailProducers, ref locked, telecomService, in happinessParameterData);
		val += healthcareBonuses;
		if (flag)
		{
			healthcareBonuses = CitizenHappinessSystem.GetHomelessBonuses(in happinessParameterData);
			val += healthcareBonuses;
		}
		result.score = val.x + val.y;
		return result;
	}

	public static float GetApartmentQuality(int familySize, int children, Entity building, ref Building buildingData, Entity buildingPrefab, ref ComponentLookup<BuildingPropertyData> buildingProperties, ref ComponentLookup<BuildingData> buildingDatas, ref ComponentLookup<SpawnableBuildingData> spawnableDatas, ref ComponentLookup<CrimeProducer> crimes, ref BufferLookup<Game.Net.ServiceCoverage> serviceCoverages, ref ComponentLookup<Locked> locked, ref ComponentLookup<ElectricityConsumer> electricityConsumers, ref ComponentLookup<WaterConsumer> waterConsumers, ref ComponentLookup<GarbageProducer> garbageProducers, ref ComponentLookup<MailProducer> mailProducers, ref ComponentLookup<PrefabRef> prefabs, ref ComponentLookup<Transform> transforms, ref ComponentLookup<Abandoned> abandoneds, NativeArray<GroundPollution> pollutionMap, NativeArray<AirPollution> airPollutionMap, NativeArray<NoisePollution> noiseMap, CellMapData<TelecomCoverage> telecomCoverages, DynamicBuffer<CityModifier> cityModifiers, Entity healthcareService, Entity entertainmentService, Entity educationService, Entity telecomService, Entity garbageService, Entity policeService, CitizenHappinessParameterData happinessParameterData, GarbageParameterData garbageParameterData, int averageHappiness)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		HouseholdFindPropertySystem.GenericApartmentQuality genericApartmentQuality = GetGenericApartmentQuality(building, buildingPrefab, ref buildingData, ref buildingProperties, ref buildingDatas, ref spawnableDatas, ref crimes, ref serviceCoverages, ref locked, ref electricityConsumers, ref waterConsumers, ref garbageProducers, ref mailProducers, ref transforms, ref abandoneds, pollutionMap, airPollutionMap, noiseMap, telecomCoverages, cityModifiers, healthcareService, entertainmentService, educationService, telecomService, garbageService, policeService, happinessParameterData, garbageParameterData);
		int2 cachedWelfareBonuses = CitizenHappinessSystem.GetCachedWelfareBonuses(genericApartmentQuality.welfareBonus, averageHappiness);
		return CitizenHappinessSystem.GetApartmentWellbeing(genericApartmentQuality.apartmentSize / (float)familySize, spawnableDatas[buildingPrefab].m_Level) + math.sqrt((float)children) * (genericApartmentQuality.educationBonus.x + genericApartmentQuality.educationBonus.y) + (float)cachedWelfareBonuses.x + (float)cachedWelfareBonuses.y + genericApartmentQuality.score;
	}

	public static float GetCachedApartmentQuality(int familySize, int children, int averageHappiness, HouseholdFindPropertySystem.GenericApartmentQuality quality)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		int2 cachedWelfareBonuses = CitizenHappinessSystem.GetCachedWelfareBonuses(quality.welfareBonus, averageHappiness);
		return CitizenHappinessSystem.GetApartmentWellbeing(quality.apartmentSize / (float)familySize, quality.level) + math.sqrt((float)children) * (quality.educationBonus.x + quality.educationBonus.y) + (float)cachedWelfareBonuses.x + (float)cachedWelfareBonuses.y + quality.score;
	}

	public static ZoneDensity GetZoneDensity(ZoneData zoneData, ZonePropertiesData zonePropertiesData)
	{
		if (zoneData.m_AreaType == Game.Zones.AreaType.Residential)
		{
			if (zonePropertiesData.m_ScaleResidentials)
			{
				if (zonePropertiesData.m_ResidentialProperties < zonePropertiesData.m_SpaceMultiplier)
				{
					return ZoneDensity.Medium;
				}
				return ZoneDensity.High;
			}
			return ZoneDensity.Low;
		}
		if (zoneData.m_AreaType == Game.Zones.AreaType.Commercial)
		{
			if (zonePropertiesData.m_SpaceMultiplier > 1f)
			{
				return ZoneDensity.High;
			}
			return ZoneDensity.Low;
		}
		if (zoneData.m_AreaType == Game.Zones.AreaType.Industrial)
		{
			if (zoneData.IsOffice())
			{
				if (zonePropertiesData.m_SpaceMultiplier < 10f)
				{
					return ZoneDensity.Low;
				}
				return ZoneDensity.High;
			}
			return ZoneDensity.Low;
		}
		Assert.IsTrue(false, $"Unknown Zone area type:{zoneData.m_AreaType}");
		return ZoneDensity.Low;
	}

	public static int GetResidentialProperties(BuildingPropertyData propertyData)
	{
		return propertyData.CountProperties(Game.Zones.AreaType.Residential);
	}

	public static bool IsMixedBuilding(Entity buildingPrefab, ref ComponentLookup<BuildingPropertyData> buildingPropertyDatas)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (buildingPropertyDatas.HasComponent(buildingPrefab))
		{
			return IsMixedBuilding(buildingPropertyDatas[buildingPrefab]);
		}
		return false;
	}

	public static int GetBuildingLevel(Entity prefabEntity, ComponentLookup<SpawnableBuildingData> spawnableBuildingDatas)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
		if (spawnableBuildingDatas.TryGetComponent(prefabEntity, ref spawnableBuildingData))
		{
			return spawnableBuildingData.m_Level;
		}
		return 1;
	}

	public static bool IsMixedBuilding(BuildingPropertyData buildingPropertyData)
	{
		if (buildingPropertyData.m_ResidentialProperties > 0)
		{
			if (buildingPropertyData.m_AllowedSold == Resource.NoResource)
			{
				return buildingPropertyData.m_AllowedManufactured != Resource.NoResource;
			}
			return true;
		}
		return false;
	}
}

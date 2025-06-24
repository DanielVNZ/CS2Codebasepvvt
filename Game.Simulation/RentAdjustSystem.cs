using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Agents;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Net;
using Game.Notifications;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
using Game.Zones;
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
public class RentAdjustSystem : GameSystemBase
{
	[BurstCompile]
	private struct AdjustRentJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingProperties;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<WorkProvider> m_WorkProviders;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> m_WorkplaceDatas;

		[ReadOnly]
		public ComponentLookup<CompanyNotifications> m_CompanyNotifications;

		[ReadOnly]
		public ComponentLookup<Attached> m_Attached;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> m_Lots;

		[ReadOnly]
		public ComponentLookup<Geometry> m_Geometries;

		[ReadOnly]
		public ComponentLookup<LandValue> m_LandValues;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<PropertyOnMarket> m_OnMarkets;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizenBufs;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_Abandoned;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_Destroyed;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblems;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingData;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneData;

		[ReadOnly]
		public BufferLookup<Resources> m_ResourcesBuf;

		[ReadOnly]
		public ComponentLookup<ExtractorProperty> m_ExtractorProperties;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_ProcessDatas;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTrucks;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<ZonePropertiesData> m_ZonePropertiesDatas;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<BuildingNotifications> m_BuildingNotifications;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		[ReadOnly]
		public NativeArray<AirPollution> m_AirPollutionMap;

		[ReadOnly]
		public NativeArray<GroundPollution> m_PollutionMap;

		[ReadOnly]
		public NativeArray<NoisePollution> m_NoiseMap;

		public CitizenHappinessParameterData m_CitizenHappinessParameterData;

		public BuildingConfigurationData m_BuildingConfigurationData;

		public PollutionParameterData m_PollutionParameters;

		public IconCommandBuffer m_IconCommandBuffer;

		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public Entity m_City;

		public EconomyParameterData m_EconomyParameterData;

		public ParallelWriter m_CommandBuffer;

		private bool CanDisplayHighRentWarnIcon(DynamicBuffer<Renter> renters)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			bool result = true;
			for (int i = 0; i < renters.Length; i++)
			{
				Entity renter = renters[i].m_Renter;
				if (m_CompanyNotifications.HasComponent(renter))
				{
					CompanyNotifications companyNotifications = m_CompanyNotifications[renter];
					if (companyNotifications.m_NoCustomersEntity != Entity.Null || companyNotifications.m_NoInputEntity != Entity.Null)
					{
						result = false;
						break;
					}
				}
				if (m_WorkProviders.HasComponent(renter))
				{
					WorkProvider workProvider = m_WorkProviders[renter];
					if (workProvider.m_EducatedNotificationEntity != Entity.Null || workProvider.m_UneducatedNotificationEntity != Entity.Null)
					{
						result = false;
						break;
					}
				}
				if (!m_HouseholdCitizenBufs.HasBuffer(renter))
				{
					continue;
				}
				DynamicBuffer<HouseholdCitizen> val = m_HouseholdCitizenBufs[renter];
				result = false;
				for (int j = 0; j < val.Length; j++)
				{
					if (!CitizenUtils.IsDead(val[j].m_Citizen, ref m_HealthProblems))
					{
						result = true;
						break;
					}
				}
			}
			return result;
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
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<Renter> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
			DynamicBuffer<CityModifier> cityModifiers = m_CityModifiers[m_City];
			ZonePropertiesData zonePropertiesData = default(ZonePropertiesData);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Entity prefab = m_Prefabs[val].m_Prefab;
				if (!m_BuildingProperties.HasComponent(prefab))
				{
					continue;
				}
				BuildingPropertyData buildingPropertyData = m_BuildingProperties[prefab];
				Building building = m_Buildings[val];
				DynamicBuffer<Renter> renters = bufferAccessor[i];
				BuildingData buildingData = m_BuildingDatas[prefab];
				int lotSize = buildingData.m_LotSize.x * buildingData.m_LotSize.y;
				float landValueBase = 0f;
				if (m_LandValues.HasComponent(building.m_RoadEdge))
				{
					landValueBase = m_LandValues[building.m_RoadEdge].m_LandValue;
				}
				Game.Zones.AreaType areaType = Game.Zones.AreaType.None;
				int buildingLevel = PropertyUtils.GetBuildingLevel(prefab, m_SpawnableBuildingData);
				bool ignoreLandValue = false;
				if (m_SpawnableBuildingData.HasComponent(prefab))
				{
					SpawnableBuildingData spawnableBuildingData = m_SpawnableBuildingData[prefab];
					areaType = m_ZoneData[spawnableBuildingData.m_ZonePrefab].m_AreaType;
					if (m_ZonePropertiesDatas.TryGetComponent(spawnableBuildingData.m_ZonePrefab, ref zonePropertiesData))
					{
						ignoreLandValue = zonePropertiesData.m_IgnoreLandValue;
					}
				}
				ProcessPollutionNotification(areaType, val, cityModifiers);
				int rentPricePerRenter = PropertyUtils.GetRentPricePerRenter(buildingPropertyData, buildingLevel, lotSize, landValueBase, areaType, ref m_EconomyParameterData, ignoreLandValue);
				if (m_OnMarkets.HasComponent(val))
				{
					PropertyOnMarket propertyOnMarket = m_OnMarkets[val];
					propertyOnMarket.m_AskingRent = rentPricePerRenter;
					m_OnMarkets[val] = propertyOnMarket;
				}
				int num = buildingPropertyData.CountProperties();
				bool flag = false;
				int2 val2 = default(int2);
				bool flag2 = m_ExtractorProperties.HasComponent(val);
				for (int num2 = renters.Length - 1; num2 >= 0; num2--)
				{
					Entity renter = renters[num2].m_Renter;
					if (m_PropertyRenters.HasComponent(renter))
					{
						PropertyRenter propertyRenter = m_PropertyRenters[renter];
						if (!m_ResourcesBuf.HasBuffer(renter))
						{
							Debug.Log((object)$"no resources:{renter.Index}");
							continue;
						}
						int num3 = 0;
						if (m_HouseholdCitizenBufs.HasBuffer(renter))
						{
							num3 = EconomyUtils.GetHouseholdIncome(m_HouseholdCitizenBufs[renter], ref m_Workers, ref m_Citizens, ref m_HealthProblems, ref m_EconomyParameterData, m_TaxRates) + math.max(0, EconomyUtils.GetResources(Resource.Money, m_ResourcesBuf[renter]));
						}
						else
						{
							Entity prefab2 = m_Prefabs[renter].m_Prefab;
							if (!m_ProcessDatas.HasComponent(prefab2) || !m_WorkProviders.HasComponent(renter) || !m_WorkplaceDatas.HasComponent(prefab2))
							{
								continue;
							}
							int companyMaxProfitPerDay = EconomyUtils.GetCompanyMaxProfitPerDay(m_WorkProviders[renter], areaType == Game.Zones.AreaType.Industrial, buildingLevel, m_ProcessDatas[prefab2], m_ResourcePrefabs, m_WorkplaceDatas[prefab2], ref m_ResourceDatas, ref m_EconomyParameterData);
							num3 = ((companyMaxProfitPerDay >= num3) ? companyMaxProfitPerDay : ((!m_OwnedVehicles.HasBuffer(renter)) ? EconomyUtils.GetCompanyTotalWorth(m_ResourcesBuf[renter], m_ResourcePrefabs, ref m_ResourceDatas) : EconomyUtils.GetCompanyTotalWorth(m_ResourcesBuf[renter], m_OwnedVehicles[renter], ref m_LayoutElements, ref m_DeliveryTrucks, m_ResourcePrefabs, ref m_ResourceDatas)));
						}
						propertyRenter.m_Rent = rentPricePerRenter;
						m_PropertyRenters[renter] = propertyRenter;
						if (rentPricePerRenter > num3)
						{
							((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<PropertySeeker>(unfilteredChunkIndex, renter, true);
						}
						val2.y++;
						if (rentPricePerRenter > num3)
						{
							val2.x++;
						}
					}
					else
					{
						renters.RemoveAt(num2);
						flag = true;
					}
				}
				if (!((float)val2.x / math.max(1f, (float)val2.y) > 0.7f) || !CanDisplayHighRentWarnIcon(renters))
				{
					m_IconCommandBuffer.Remove(val, m_BuildingConfigurationData.m_HighRentNotification);
					building.m_Flags &= ~Game.Buildings.BuildingFlags.HighRentWarning;
					m_Buildings[val] = building;
				}
				else if (renters.Length > 0 && !flag2 && num > renters.Length && (building.m_Flags & Game.Buildings.BuildingFlags.HighRentWarning) == 0)
				{
					m_IconCommandBuffer.Add(val, m_BuildingConfigurationData.m_HighRentNotification, IconPriority.Problem);
					building.m_Flags |= Game.Buildings.BuildingFlags.HighRentWarning;
					m_Buildings[val] = building;
				}
				if (renters.Length > num && m_PropertyRenters.HasComponent(renters[renters.Length - 1].m_Renter))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PropertyRenter>(unfilteredChunkIndex, renters[renters.Length - 1].m_Renter);
					renters.RemoveAt(renters.Length - 1);
					Debug.LogWarning((object)$"Removed extra renter from building:{val.Index}");
				}
				if (renters.Length == 0 && (building.m_Flags & Game.Buildings.BuildingFlags.HighRentWarning) != Game.Buildings.BuildingFlags.None)
				{
					m_IconCommandBuffer.Remove(val, m_BuildingConfigurationData.m_HighRentNotification);
					building.m_Flags &= ~Game.Buildings.BuildingFlags.HighRentWarning;
					m_Buildings[val] = building;
				}
				if (m_Prefabs.HasComponent(val) && !m_Abandoned.HasComponent(val) && !m_Destroyed.HasComponent(val) && flag && num > renters.Length)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PropertyOnMarket>(unfilteredChunkIndex, val, new PropertyOnMarket
					{
						m_AskingRent = rentPricePerRenter
					});
				}
			}
		}

		private void ProcessPollutionNotification(Game.Zones.AreaType areaType, Entity buildingEntity, DynamicBuffer<CityModifier> cityModifiers)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			if (areaType == Game.Zones.AreaType.Residential)
			{
				int2 groundPollutionBonuses = CitizenHappinessSystem.GetGroundPollutionBonuses(buildingEntity, ref m_Transforms, m_PollutionMap, cityModifiers, in m_CitizenHappinessParameterData);
				int2 noiseBonuses = CitizenHappinessSystem.GetNoiseBonuses(buildingEntity, ref m_Transforms, m_NoiseMap, in m_CitizenHappinessParameterData);
				int2 airPollutionBonuses = CitizenHappinessSystem.GetAirPollutionBonuses(buildingEntity, ref m_Transforms, m_AirPollutionMap, cityModifiers, in m_CitizenHappinessParameterData);
				bool flag = groundPollutionBonuses.x + groundPollutionBonuses.y < 2 * m_PollutionParameters.m_GroundPollutionNotificationLimit;
				bool flag2 = airPollutionBonuses.x + airPollutionBonuses.y < 2 * m_PollutionParameters.m_AirPollutionNotificationLimit;
				bool flag3 = noiseBonuses.x + noiseBonuses.y < 2 * m_PollutionParameters.m_NoisePollutionNotificationLimit;
				BuildingNotifications buildingNotifications = m_BuildingNotifications[buildingEntity];
				if (flag && !buildingNotifications.HasNotification(BuildingNotification.GroundPollution))
				{
					m_IconCommandBuffer.Add(buildingEntity, m_PollutionParameters.m_GroundPollutionNotification, IconPriority.Problem);
					buildingNotifications.m_Notifications |= BuildingNotification.GroundPollution;
					m_BuildingNotifications[buildingEntity] = buildingNotifications;
				}
				else if (!flag && buildingNotifications.HasNotification(BuildingNotification.GroundPollution))
				{
					m_IconCommandBuffer.Remove(buildingEntity, m_PollutionParameters.m_GroundPollutionNotification);
					buildingNotifications.m_Notifications &= ~BuildingNotification.GroundPollution;
					m_BuildingNotifications[buildingEntity] = buildingNotifications;
				}
				if (flag2 && !buildingNotifications.HasNotification(BuildingNotification.AirPollution))
				{
					m_IconCommandBuffer.Add(buildingEntity, m_PollutionParameters.m_AirPollutionNotification, IconPriority.Problem);
					buildingNotifications.m_Notifications |= BuildingNotification.AirPollution;
					m_BuildingNotifications[buildingEntity] = buildingNotifications;
				}
				else if (!flag2 && buildingNotifications.HasNotification(BuildingNotification.AirPollution))
				{
					m_IconCommandBuffer.Remove(buildingEntity, m_PollutionParameters.m_AirPollutionNotification);
					buildingNotifications.m_Notifications &= ~BuildingNotification.AirPollution;
					m_BuildingNotifications[buildingEntity] = buildingNotifications;
				}
				if (flag3 && !buildingNotifications.HasNotification(BuildingNotification.NoisePollution))
				{
					m_IconCommandBuffer.Add(buildingEntity, m_PollutionParameters.m_NoisePollutionNotification, IconPriority.Problem);
					buildingNotifications.m_Notifications |= BuildingNotification.NoisePollution;
					m_BuildingNotifications[buildingEntity] = buildingNotifications;
				}
				else if (!flag3 && buildingNotifications.HasNotification(BuildingNotification.NoisePollution))
				{
					m_IconCommandBuffer.Remove(buildingEntity, m_PollutionParameters.m_NoisePollutionNotification);
					buildingNotifications.m_Notifications &= ~BuildingNotification.NoisePollution;
					m_BuildingNotifications[buildingEntity] = buildingNotifications;
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

		public BufferTypeHandle<Renter> __Game_Buildings_Renter_RW_BufferTypeHandle;

		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RW_ComponentLookup;

		public ComponentLookup<PropertyOnMarket> __Game_Buildings_PropertyOnMarket_RW_ComponentLookup;

		public ComponentLookup<Building> __Game_Buildings_Building_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkProvider> __Game_Companies_WorkProvider_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CompanyNotifications> __Game_Companies_CompanyNotifications_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> __Game_Areas_Lot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Geometry> __Game_Areas_Geometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LandValue> __Game_Net_LandValue_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> __Game_Prefabs_WorkplaceData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Abandoned> __Game_Buildings_Abandoned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		public ComponentLookup<BuildingNotifications> __Game_Buildings_BuildingNotifications_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ExtractorProperty> __Game_Buildings_ExtractorProperty_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZonePropertiesData> __Game_Prefabs_ZonePropertiesData_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_Renter_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Renter>(false);
			__Game_Buildings_PropertyRenter_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(false);
			__Game_Buildings_PropertyOnMarket_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyOnMarket>(false);
			__Game_Buildings_Building_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Companies_WorkProvider_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkProvider>(true);
			__Game_Companies_CompanyNotifications_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CompanyNotifications>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Areas_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Areas.Lot>(true);
			__Game_Areas_Geometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Geometry>(true);
			__Game_Net_LandValue_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LandValue>(true);
			__Game_Prefabs_WorkplaceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkplaceData>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Buildings_Abandoned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Abandoned>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Buildings_BuildingNotifications_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingNotifications>(false);
			__Game_Buildings_ExtractorProperty_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ExtractorProperty>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Prefabs_ZonePropertiesData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZonePropertiesData>(true);
		}
	}

	public static readonly int kUpdatesPerDay = 16;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_DemandParameterQuery;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private ResourceSystem m_ResourceSystem;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private TelecomCoverageSystem m_TelecomCoverageSystem;

	private CitySystem m_CitySystem;

	private TaxSystem m_TaxSystem;

	private IconCommandSystem m_IconCommandSystem;

	private EntityQuery m_HealthcareParameterQuery;

	private EntityQuery m_ExtractorParameterQuery;

	private EntityQuery m_ParkParameterQuery;

	private EntityQuery m_EducationParameterQuery;

	private EntityQuery m_TelecomParameterQuery;

	private EntityQuery m_GarbageParameterQuery;

	private EntityQuery m_PoliceParameterQuery;

	private EntityQuery m_CitizenHappinessParameterQuery;

	private EntityQuery m_BuildingParameterQuery;

	private EntityQuery m_PollutionParameterQuery;

	private EntityQuery m_BuildingQuery;

	protected int cycles;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_TelecomCoverageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TelecomCoverageSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_DemandParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DemandParameterData>() });
		m_BuildingParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingConfigurationData>() });
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<Renter>(),
			ComponentType.Exclude<StorageProperty>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ExtractorParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ExtractorParameterData>() });
		m_HealthcareParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HealthcareParameterData>() });
		m_ParkParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ParkParameterData>() });
		m_EducationParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EducationParameterData>() });
		m_TelecomParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TelecomParameterData>() });
		m_GarbageParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageParameterData>() });
		m_PoliceParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PoliceConfigurationData>() });
		m_CitizenHappinessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenHappinessParameterData>() });
		m_PollutionParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PollutionParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_DemandParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_HealthcareParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_ParkParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EducationParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_TelecomParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_GarbageParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_PoliceParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_BuildingQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		AdjustRentJob adjustRentJob = new AdjustRentJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RenterType = InternalCompilerInterface.GetBufferTypeHandle<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = ((ComponentSystemBase)this).GetSharedComponentTypeHandle<UpdateFrame>(),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OnMarkets = InternalCompilerInterface.GetComponentLookup<PropertyOnMarket>(ref __TypeHandle.__Game_Buildings_PropertyOnMarket_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingProperties = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkProviders = InternalCompilerInterface.GetComponentLookup<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyNotifications = InternalCompilerInterface.GetComponentLookup<CompanyNotifications>(ref __TypeHandle.__Game_Companies_CompanyNotifications_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Attached = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lots = InternalCompilerInterface.GetComponentLookup<Game.Areas.Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Geometries = InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LandValues = InternalCompilerInterface.GetComponentLookup<LandValue>(ref __TypeHandle.__Game_Net_LandValue_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkplaceDatas = InternalCompilerInterface.GetComponentLookup<WorkplaceData>(ref __TypeHandle.__Game_Prefabs_WorkplaceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenBufs = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Abandoned = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Destroyed = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingData = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneData = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingNotifications = InternalCompilerInterface.GetComponentLookup<BuildingNotifications>(ref __TypeHandle.__Game_Buildings_BuildingNotifications_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorProperties = InternalCompilerInterface.GetComponentLookup<ExtractorProperty>(ref __TypeHandle.__Game_Buildings_ExtractorProperty_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcesBuf = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxRates = m_TaxSystem.GetTaxRates(),
			m_PollutionMap = m_GroundPollutionSystem.GetMap(readOnly: true, out dependencies),
			m_AirPollutionMap = m_AirPollutionSystem.GetMap(readOnly: true, out dependencies2),
			m_NoiseMap = m_NoisePollutionSystem.GetMap(readOnly: true, out dependencies3),
			m_CitizenHappinessParameterData = ((EntityQuery)(ref m_CitizenHappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>(),
			m_BuildingConfigurationData = ((EntityQuery)(ref m_BuildingParameterQuery)).GetSingleton<BuildingConfigurationData>(),
			m_PollutionParameters = ((EntityQuery)(ref m_PollutionParameterQuery)).GetSingleton<PollutionParameterData>(),
			m_DeliveryTrucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZonePropertiesDatas = InternalCompilerInterface.GetComponentLookup<ZonePropertiesData>(ref __TypeHandle.__Game_Prefabs_ZonePropertiesData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EconomyParameterData = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_City = m_CitySystem.City,
			m_UpdateFrameIndex = updateFrame
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		adjustRentJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		adjustRentJob.m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<AdjustRentJob>(adjustRentJob, m_BuildingQuery, JobUtils.CombineDependencies(dependencies, dependencies2, dependencies3, ((SystemBase)this).Dependency));
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		m_ResourceSystem.AddPrefabsReader(val2);
		m_GroundPollutionSystem.AddReader(val2);
		m_AirPollutionSystem.AddReader(val2);
		m_NoisePollutionSystem.AddReader(val2);
		m_TelecomCoverageSystem.AddReader(val2);
		m_TaxSystem.AddReader(val2);
		m_IconCommandSystem.AddCommandBufferWriter(val2);
		((SystemBase)this).Dependency = val2;
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
	public RentAdjustSystem()
	{
	}
}

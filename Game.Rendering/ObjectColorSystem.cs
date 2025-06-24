using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Economy;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
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

namespace Game.Rendering;

[CompilerGenerated]
public class ObjectColorSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateObjectColorsJob : IJobChunk
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_InfomodeChunks;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<InfomodeActive> m_InfomodeActiveType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewBuildingData> m_InfoviewBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewBuildingStatusData> m_InfoviewBuildingStatusType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewTransportStopData> m_InfoviewTransportStopType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewVehicleData> m_InfoviewVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewObjectStatusData> m_InfoviewObjectStatusType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetStatusData> m_InfoviewNetStatusType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Hospital> m_HospitalType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityProducer> m_ElectricityProducerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Transformer> m_TransformerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.WaterPumpingStation> m_WaterPumpingStationType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.WaterTower> m_WaterTowerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.SewageOutlet> m_SewageOutletType;

		[ReadOnly]
		public ComponentTypeHandle<WastewaterTreatmentPlant> m_WastewaterTreatmentPlantType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TransportDepot> m_TransportDepotType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TransportStation> m_TransportStationType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.GarbageFacility> m_GarbageFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.FireStation> m_FireStationType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.PoliceStation> m_PoliceStationType;

		[ReadOnly]
		public ComponentTypeHandle<CrimeProducer> m_CrimeProducerType;

		[ReadOnly]
		public ComponentTypeHandle<RoadMaintenance> m_RoadMaintenanceType;

		[ReadOnly]
		public ComponentTypeHandle<ParkMaintenance> m_ParkMaintenanceType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.PostFacility> m_PostFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TelecomFacility> m_TelecomFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.School> m_SchoolType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Park> m_ParkType;

		[ReadOnly]
		public ComponentTypeHandle<AttractivenessProvider> m_AttractivenessProviderType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.EmergencyShelter> m_EmergencyShelterType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.DisasterFacility> m_DisasterFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.FirewatchTower> m_FirewatchTowerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.DeathcareFacility> m_DeathcareFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Prison> m_PrisonType;

		[ReadOnly]
		public ComponentTypeHandle<AdminBuilding> m_AdminBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.WelfareOffice> m_WelfareOfficeType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.ResearchFacility> m_ResearchFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.ParkingFacility> m_ParkingFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Battery> m_BatteryType;

		[ReadOnly]
		public ComponentTypeHandle<MailProducer> m_MailProducerType;

		[ReadOnly]
		public ComponentTypeHandle<BuildingCondition> m_BuildingConditionType;

		[ReadOnly]
		public ComponentTypeHandle<ResidentialProperty> m_ResidentialPropertyType;

		[ReadOnly]
		public ComponentTypeHandle<CommercialProperty> m_CommercialPropertyType;

		[ReadOnly]
		public ComponentTypeHandle<IndustrialProperty> m_IndustrialPropertyType;

		[ReadOnly]
		public ComponentTypeHandle<OfficeProperty> m_OfficePropertyType;

		[ReadOnly]
		public ComponentTypeHandle<StorageProperty> m_StoragePropertyType;

		[ReadOnly]
		public ComponentTypeHandle<ExtractorProperty> m_ExtractorPropertyType;

		[ReadOnly]
		public ComponentTypeHandle<GarbageProducer> m_GarbageProducerType;

		[ReadOnly]
		public ComponentTypeHandle<Abandoned> m_AbandonedType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.LeisureProvider> m_LeisureProviderType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityConsumer> m_ElectricityConsumerType;

		[ReadOnly]
		public ComponentTypeHandle<WaterConsumer> m_WaterConsumerType;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_BuildingEfficiencyType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.TransportStop> m_TransportStopType;

		[ReadOnly]
		public ComponentTypeHandle<BusStop> m_BusStopType;

		[ReadOnly]
		public ComponentTypeHandle<TrainStop> m_TrainStopType;

		[ReadOnly]
		public ComponentTypeHandle<TaxiStand> m_TaxiStandType;

		[ReadOnly]
		public ComponentTypeHandle<TramStop> m_TramStopType;

		[ReadOnly]
		public ComponentTypeHandle<ShipStop> m_ShipStopType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.MailBox> m_MailBoxType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.WorkStop> m_WorkStopType;

		[ReadOnly]
		public ComponentTypeHandle<AirplaneStop> m_AirplaneStopType;

		[ReadOnly]
		public ComponentTypeHandle<SubwayStop> m_SubwayStopType;

		[ReadOnly]
		public ComponentTypeHandle<Vehicle> m_VehicleType;

		[ReadOnly]
		public ComponentTypeHandle<PassengerTransport> m_PassengerTransportType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.CargoTransport> m_CargoTransportType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.Taxi> m_TaxiType;

		[ReadOnly]
		public ComponentTypeHandle<ParkMaintenanceVehicle> m_ParkMaintenanceVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<RoadMaintenanceVehicle> m_RoadMaintenanceVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.Ambulance> m_AmbulanceType;

		[ReadOnly]
		public ComponentTypeHandle<EvacuatingTransport> m_EvacuatingTransportType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.FireEngine> m_FireEngineType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.GarbageTruck> m_GarbageTruckType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.Hearse> m_HearseType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.PoliceCar> m_PoliceCarType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.PostVan> m_PostVanType;

		[ReadOnly]
		public ComponentTypeHandle<PrisonerTransport> m_PrisonerTransportType;

		[ReadOnly]
		public BufferTypeHandle<Passenger> m_PassengerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Creatures.Resident> m_ResidentType;

		[ReadOnly]
		public ComponentTypeHandle<Tree> m_TreeType;

		[ReadOnly]
		public ComponentTypeHandle<Plant> m_PlantType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.UtilityObject> m_UtilityObjectType;

		[ReadOnly]
		public ComponentTypeHandle<Destroyed> m_DestroyedType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Damaged> m_DamagedType;

		[ReadOnly]
		public ComponentTypeHandle<UnderConstruction> m_UnderConstructionType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.UniqueObject> m_UniqueObjectType;

		[ReadOnly]
		public ComponentTypeHandle<Placeholder> m_PlaceholderType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> m_CurrentDistrictType;

		[ReadOnly]
		public ComponentTypeHandle<Attached> m_AttachedType;

		[ReadOnly]
		public ComponentTypeHandle<AccidentSite> m_AccidentSiteType;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> m_ParkData;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_AbandonedData;

		[ReadOnly]
		public ComponentLookup<TreeData> m_TreeData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyData;

		[ReadOnly]
		public ComponentLookup<PollutionData> m_PollutionData;

		[ReadOnly]
		public ComponentLookup<PollutionModifierData> m_PollutionModifierData;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingDatas;

		[ReadOnly]
		public ComponentLookup<PlaceholderBuildingData> m_PlaceholderBuildingData;

		[ReadOnly]
		public ComponentLookup<SewageOutletData> m_SewageOutletData;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Resident> m_ResidentData;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<CommercialCompany> m_CommercialCompanies;

		[ReadOnly]
		public ComponentLookup<Profitability> m_Profitabilities;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.LeisureProvider> m_LeisureProviders;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessData;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneDatas;

		[ReadOnly]
		public BufferLookup<Employee> m_Employees;

		[ReadOnly]
		public BufferLookup<Resources> m_ResourceBuffs;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<UtilityObjectData> m_PrefabUtilityObjectData;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_ElectricityConsumerData;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> m_WaterConsumerData;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> m_ElectricityNodeConnectionData;

		[ReadOnly]
		public ComponentLookup<WaterPipeNodeConnection> m_WaterPipeNodeConnectionData;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> m_ElectricityFlowEdgeData;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> m_WaterPipeEdgeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ResourceConnection> m_ResourceConnectionData;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> m_ConnectedFlowEdges;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public EventHelpers.FireHazardData m_FireHazardData;

		[ReadOnly]
		public CellMapData<TelecomCoverage> m_TelecomCoverageData;

		public PollutionParameterData m_PollutionParameters;

		public Entity m_City;

		public ComponentTypeHandle<Game.Objects.Color> m_ColorType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Game.Objects.Color> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Objects.Color>(ref m_ColorType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Building>(ref m_BuildingType);
			bool isSubBuilding = false;
			if (flag)
			{
				if (((ArchetypeChunk)(ref chunk)).Has<Owner>(ref m_OwnerType))
				{
					isSubBuilding = true;
				}
				else
				{
					if (GetBuildingColor(chunk, out var index))
					{
						for (int i = 0; i < nativeArray.Length; i++)
						{
							nativeArray[i] = new Game.Objects.Color((byte)index, 0);
						}
						CheckColors(nativeArray, chunk, flag);
						return;
					}
					if (GetBuildingStatusType(chunk, out var statusData, out var activeData))
					{
						GetBuildingStatusColors(nativeArray, chunk, statusData, activeData);
						CheckColors(nativeArray, chunk, flag);
						return;
					}
				}
			}
			int index3;
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Routes.TransportStop>(ref m_TransportStopType) && GetTransportStopColor(chunk, out var index2))
			{
				for (int j = 0; j < nativeArray.Length; j++)
				{
					nativeArray[j] = new Game.Objects.Color((byte)index2, 0);
				}
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Vehicle>(ref m_VehicleType) && GetVehicleColor(chunk, out index3))
			{
				for (int k = 0; k < nativeArray.Length; k++)
				{
					nativeArray[k] = new Game.Objects.Color((byte)index3, 0);
				}
			}
			else
			{
				if (((ArchetypeChunk)(ref chunk)).Has<Game.Objects.UtilityObject>(ref m_UtilityObjectType) && GetNetStatusColors(nativeArray, chunk))
				{
					return;
				}
				if (GetObjectStatusType(chunk, isSubBuilding, out var statusData2, out var activeData2))
				{
					GetObjectStatusColors(nativeArray, chunk, statusData2, activeData2);
					CheckColors(nativeArray, chunk, flag);
					return;
				}
				for (int l = 0; l < nativeArray.Length; l++)
				{
					nativeArray[l] = default(Game.Objects.Color);
				}
				CheckColors(nativeArray, chunk, flag);
			}
		}

		private void CheckColors(NativeArray<Game.Objects.Color> colors, ArchetypeChunk chunk, bool isBuilding)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			if (!isBuilding)
			{
				return;
			}
			NativeArray<Destroyed> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Destroyed>(ref m_DestroyedType);
			NativeArray<UnderConstruction> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<UnderConstruction>(ref m_UnderConstructionType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			Destroyed destroyed = default(Destroyed);
			UnderConstruction underConstruction = default(UnderConstruction);
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				PrefabRef prefabRef = nativeArray3[i];
				if ((m_BuildingData[prefabRef.m_Prefab].m_Flags & Game.Prefabs.BuildingFlags.ColorizeLot) != 0 || (CollectionUtils.TryGet<Destroyed>(nativeArray, i, ref destroyed) && destroyed.m_Cleared >= 0f) || (CollectionUtils.TryGet<UnderConstruction>(nativeArray2, i, ref underConstruction) && underConstruction.m_NewPrefab == Entity.Null))
				{
					Game.Objects.Color color = colors[i];
					color.m_SubColor = true;
					colors[i] = color;
				}
			}
		}

		private bool GetBuildingColor(ArchetypeChunk chunk, out int index)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			index = 0;
			int num = int.MaxValue;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewBuildingData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewBuildingData>(ref m_InfoviewBuildingType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfomodeActive infomodeActive = nativeArray2[j];
					int priority = infomodeActive.m_Priority;
					if (priority < num && HasBuildingColor(nativeArray[j], chunk))
					{
						index = infomodeActive.m_Index;
						num = priority;
					}
				}
			}
			return num != int.MaxValue;
		}

		private bool HasBuildingColor(InfoviewBuildingData infoviewBuildingData, ArchetypeChunk chunk)
		{
			switch (infoviewBuildingData.m_Type)
			{
			case BuildingType.Hospital:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Hospital>(ref m_HospitalType);
			case BuildingType.PowerPlant:
				return ((ArchetypeChunk)(ref chunk)).Has<ElectricityProducer>(ref m_ElectricityProducerType);
			case BuildingType.Transformer:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Transformer>(ref m_TransformerType);
			case BuildingType.FreshWaterBuilding:
				if (!((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.WaterPumpingStation>(ref m_WaterPumpingStationType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.WaterTower>(ref m_WaterTowerType);
				}
				return true;
			case BuildingType.SewageBuilding:
				if (!((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.SewageOutlet>(ref m_SewageOutletType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<WastewaterTreatmentPlant>(ref m_WastewaterTreatmentPlantType);
				}
				return true;
			case BuildingType.TransportDepot:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.TransportDepot>(ref m_TransportDepotType);
			case BuildingType.TransportStation:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.TransportStation>(ref m_TransportStationType);
			case BuildingType.GarbageFacility:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.GarbageFacility>(ref m_GarbageFacilityType);
			case BuildingType.FireStation:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.FireStation>(ref m_FireStationType);
			case BuildingType.PoliceStation:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.PoliceStation>(ref m_PoliceStationType);
			case BuildingType.RoadMaintenanceDepot:
				return ((ArchetypeChunk)(ref chunk)).Has<RoadMaintenance>(ref m_RoadMaintenanceType);
			case BuildingType.PostFacility:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.PostFacility>(ref m_PostFacilityType);
			case BuildingType.TelecomFacility:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.TelecomFacility>(ref m_TelecomFacilityType);
			case BuildingType.School:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.School>(ref m_SchoolType);
			case BuildingType.Park:
				if (!((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Park>(ref m_ParkType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<AttractivenessProvider>(ref m_AttractivenessProviderType);
				}
				return true;
			case BuildingType.EmergencyShelter:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.EmergencyShelter>(ref m_EmergencyShelterType);
			case BuildingType.DisasterFacility:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.DisasterFacility>(ref m_DisasterFacilityType);
			case BuildingType.FirewatchTower:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.FirewatchTower>(ref m_FirewatchTowerType);
			case BuildingType.DeathcareFacility:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.DeathcareFacility>(ref m_DeathcareFacilityType);
			case BuildingType.Prison:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Prison>(ref m_PrisonType);
			case BuildingType.AdminBuilding:
				return ((ArchetypeChunk)(ref chunk)).Has<AdminBuilding>(ref m_AdminBuildingType);
			case BuildingType.WelfareOffice:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.WelfareOffice>(ref m_WelfareOfficeType);
			case BuildingType.ResearchFacility:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.ResearchFacility>(ref m_ResearchFacilityType);
			case BuildingType.ParkMaintenanceDepot:
				return ((ArchetypeChunk)(ref chunk)).Has<ParkMaintenance>(ref m_ParkMaintenanceType);
			case BuildingType.ParkingFacility:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.ParkingFacility>(ref m_ParkingFacilityType);
			case BuildingType.Battery:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Battery>(ref m_BatteryType);
			case BuildingType.ResidentialBuilding:
				return ((ArchetypeChunk)(ref chunk)).Has<ResidentialProperty>(ref m_ResidentialPropertyType);
			case BuildingType.CommercialBuilding:
				return ((ArchetypeChunk)(ref chunk)).Has<CommercialProperty>(ref m_CommercialPropertyType);
			case BuildingType.IndustrialBuilding:
				if (((ArchetypeChunk)(ref chunk)).Has<IndustrialProperty>(ref m_IndustrialPropertyType))
				{
					return !((ArchetypeChunk)(ref chunk)).Has<OfficeProperty>(ref m_OfficePropertyType);
				}
				return false;
			case BuildingType.OfficeBuilding:
				return ((ArchetypeChunk)(ref chunk)).Has<OfficeProperty>(ref m_OfficePropertyType);
			case BuildingType.SignatureResidential:
				if (((ArchetypeChunk)(ref chunk)).Has<ResidentialProperty>(ref m_ResidentialPropertyType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.UniqueObject>(ref m_UniqueObjectType);
				}
				return false;
			case BuildingType.ExtractorBuilding:
				return ((ArchetypeChunk)(ref chunk)).Has<ExtractorProperty>(ref m_ExtractorPropertyType);
			case BuildingType.SignatureCommercial:
				if (((ArchetypeChunk)(ref chunk)).Has<CommercialProperty>(ref m_CommercialPropertyType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.UniqueObject>(ref m_UniqueObjectType);
				}
				return false;
			case BuildingType.SignatureIndustrial:
				if (((ArchetypeChunk)(ref chunk)).Has<IndustrialProperty>(ref m_IndustrialPropertyType) && ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.UniqueObject>(ref m_UniqueObjectType))
				{
					return !((ArchetypeChunk)(ref chunk)).Has<OfficeProperty>(ref m_OfficePropertyType);
				}
				return false;
			case BuildingType.SignatureOffice:
				if (((ArchetypeChunk)(ref chunk)).Has<OfficeProperty>(ref m_OfficePropertyType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.UniqueObject>(ref m_UniqueObjectType);
				}
				return false;
			case BuildingType.LandValueSources:
				if (!((ArchetypeChunk)(ref chunk)).Has<CommercialProperty>(ref m_CommercialPropertyType) && !((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.School>(ref m_SchoolType) && !((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Hospital>(ref m_HospitalType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<AttractivenessProvider>(ref m_AttractivenessProviderType);
				}
				return true;
			default:
				return false;
			}
		}

		private bool GetBuildingStatusType(ArchetypeChunk chunk, out InfoviewBuildingStatusData statusData, out InfomodeActive activeData)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			statusData = default(InfoviewBuildingStatusData);
			activeData = default(InfomodeActive);
			int num = int.MaxValue;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewBuildingStatusData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewBuildingStatusData>(ref m_InfoviewBuildingStatusType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfomodeActive infomodeActive = nativeArray2[j];
					int priority = infomodeActive.m_Priority;
					if (priority < num)
					{
						InfoviewBuildingStatusData infoviewBuildingStatusData = nativeArray[j];
						if (HasBuildingStatus(nativeArray[j], chunk))
						{
							statusData = infoviewBuildingStatusData;
							activeData = infomodeActive;
							num = priority;
						}
					}
				}
			}
			return num != int.MaxValue;
		}

		private bool HasBuildingStatus(InfoviewBuildingStatusData infoviewBuildingStatusData, ArchetypeChunk chunk)
		{
			switch (infoviewBuildingStatusData.m_Type)
			{
			case BuildingStatusType.CrimeProbability:
				return ((ArchetypeChunk)(ref chunk)).Has<CrimeProducer>(ref m_CrimeProducerType);
			case BuildingStatusType.MailAccumulation:
				return ((ArchetypeChunk)(ref chunk)).Has<MailProducer>(ref m_MailProducerType);
			case BuildingStatusType.Wealth:
			case BuildingStatusType.Education:
			case BuildingStatusType.Health:
			case BuildingStatusType.Age:
			case BuildingStatusType.Happiness:
			case BuildingStatusType.Wellbeing:
				return ((ArchetypeChunk)(ref chunk)).Has<ResidentialProperty>(ref m_ResidentialPropertyType);
			case BuildingStatusType.Level:
				if (((ArchetypeChunk)(ref chunk)).Has<BuildingCondition>(ref m_BuildingConditionType) && !((ArchetypeChunk)(ref chunk)).Has<Abandoned>(ref m_AbandonedType) && !((ArchetypeChunk)(ref chunk)).Has<Destroyed>(ref m_DestroyedType))
				{
					return !((ArchetypeChunk)(ref chunk)).Has<Game.Objects.UniqueObject>(ref m_UniqueObjectType);
				}
				return false;
			case BuildingStatusType.GarbageAccumulation:
				return ((ArchetypeChunk)(ref chunk)).Has<GarbageProducer>(ref m_GarbageProducerType);
			case BuildingStatusType.Profitability:
				if (((ArchetypeChunk)(ref chunk)).Has<CommercialProperty>(ref m_CommercialPropertyType) || ((ArchetypeChunk)(ref chunk)).Has<IndustrialProperty>(ref m_IndustrialPropertyType))
				{
					return !((ArchetypeChunk)(ref chunk)).Has<StorageProperty>(ref m_StoragePropertyType);
				}
				return false;
			case BuildingStatusType.LeisureProvider:
				if (!((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.LeisureProvider>(ref m_LeisureProviderType))
				{
					if (((ArchetypeChunk)(ref chunk)).Has<Renter>(ref m_RenterType))
					{
						return ((ArchetypeChunk)(ref chunk)).Has<CommercialProperty>(ref m_CommercialPropertyType);
					}
					return false;
				}
				return true;
			case BuildingStatusType.ElectricityConsumption:
				return ((ArchetypeChunk)(ref chunk)).Has<ElectricityConsumer>(ref m_ElectricityConsumerType);
			case BuildingStatusType.NetworkQuality:
				return true;
			case BuildingStatusType.AirPollutionSource:
			case BuildingStatusType.GroundPollutionSource:
			case BuildingStatusType.NoisePollutionSource:
				return true;
			case BuildingStatusType.LodgingProvider:
				return ((ArchetypeChunk)(ref chunk)).Has<CommercialProperty>(ref m_CommercialPropertyType);
			case BuildingStatusType.WaterPollutionSource:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.SewageOutlet>(ref m_SewageOutletType);
			case BuildingStatusType.LandValue:
				return ((ArchetypeChunk)(ref chunk)).Has<Renter>(ref m_RenterType);
			case BuildingStatusType.WaterConsumption:
				return ((ArchetypeChunk)(ref chunk)).Has<WaterConsumer>(ref m_WaterConsumerType);
			case BuildingStatusType.ResidentialBuilding:
				return ((ArchetypeChunk)(ref chunk)).Has<ResidentialProperty>(ref m_ResidentialPropertyType);
			case BuildingStatusType.CommercialBuilding:
				return ((ArchetypeChunk)(ref chunk)).Has<CommercialProperty>(ref m_CommercialPropertyType);
			case BuildingStatusType.IndustrialBuilding:
				if (((ArchetypeChunk)(ref chunk)).Has<IndustrialProperty>(ref m_IndustrialPropertyType))
				{
					return !((ArchetypeChunk)(ref chunk)).Has<OfficeProperty>(ref m_OfficePropertyType);
				}
				return false;
			case BuildingStatusType.OfficeBuilding:
				return ((ArchetypeChunk)(ref chunk)).Has<OfficeProperty>(ref m_OfficePropertyType);
			case BuildingStatusType.SignatureResidential:
				if (((ArchetypeChunk)(ref chunk)).Has<ResidentialProperty>(ref m_ResidentialPropertyType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.UniqueObject>(ref m_UniqueObjectType);
				}
				return false;
			case BuildingStatusType.SignatureCommercial:
				if (((ArchetypeChunk)(ref chunk)).Has<CommercialProperty>(ref m_CommercialPropertyType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.UniqueObject>(ref m_UniqueObjectType);
				}
				return false;
			case BuildingStatusType.SignatureIndustrial:
				if (((ArchetypeChunk)(ref chunk)).Has<IndustrialProperty>(ref m_IndustrialPropertyType) && ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.UniqueObject>(ref m_UniqueObjectType))
				{
					return !((ArchetypeChunk)(ref chunk)).Has<OfficeProperty>(ref m_OfficePropertyType);
				}
				return false;
			case BuildingStatusType.SignatureOffice:
				if (((ArchetypeChunk)(ref chunk)).Has<OfficeProperty>(ref m_OfficePropertyType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.UniqueObject>(ref m_UniqueObjectType);
				}
				return false;
			case BuildingStatusType.HomelessCount:
				if (!((ArchetypeChunk)(ref chunk)).Has<Abandoned>(ref m_AbandonedType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Park>(ref m_ParkType);
				}
				return true;
			default:
				return false;
			}
		}

		private void GetBuildingStatusColors(NativeArray<Game.Objects.Color> results, ArchetypeChunk chunk, InfoviewBuildingStatusData statusData, InfomodeActive activeData)
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_089d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0968: Unknown result type (might be due to invalid IL or missing references)
			//IL_096d: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d88: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0701: Unknown result type (might be due to invalid IL or missing references)
			//IL_0706: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f47: Unknown result type (might be due to invalid IL or missing references)
			//IL_101a: Unknown result type (might be due to invalid IL or missing references)
			//IL_101f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_071b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			switch (statusData.m_Type)
			{
			case BuildingStatusType.CrimeProbability:
			{
				NativeArray<CrimeProducer> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CrimeProducer>(ref m_CrimeProducerType);
				NativeArray<AccidentSite> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AccidentSite>(ref m_AccidentSiteType);
				AccidentSite accidentSite = default(AccidentSite);
				for (int num12 = 0; num12 < nativeArray9.Length; num12++)
				{
					float num13 = nativeArray9[num12].m_Crime;
					if (CollectionUtils.TryGet<AccidentSite>(nativeArray10, num12, ref accidentSite) && (accidentSite.m_Flags & AccidentSiteFlags.CrimeScene) != 0)
					{
						num13 *= 3.3333333f;
					}
					results[num12] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, num13) * 255f), 0, 255));
				}
				break;
			}
			case BuildingStatusType.MailAccumulation:
			{
				NativeArray<MailProducer> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MailProducer>(ref m_MailProducerType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					MailProducer mailProducer = nativeArray2[j];
					float status = math.max((int)mailProducer.m_SendingMail, mailProducer.receivingMail);
					results[j] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, status) * 255f), 0, 255));
				}
				break;
			}
			case BuildingStatusType.Wealth:
			{
				BufferAccessor<Renter> bufferAccessor6 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
				for (int num9 = 0; num9 < bufferAccessor6.Length; num9++)
				{
					float2 val5 = default(float2);
					DynamicBuffer<Renter> val6 = bufferAccessor6[num9];
					for (int num10 = 0; num10 < val6.Length; num10++)
					{
						Entity renter = val6[num10].m_Renter;
						if (m_Households.HasComponent(renter) && m_HouseholdCitizens.HasBuffer(renter) && m_ResourceBuffs.HasBuffer(renter))
						{
							int householdTotalWealth = EconomyUtils.GetHouseholdTotalWealth(m_Households[renter], m_ResourceBuffs[renter]);
							val5.x += householdTotalWealth;
							val5.y += 1f;
						}
					}
					if (val5.y > 0f)
					{
						results[num9] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, val5.x / val5.y) * 255f), 0, 255));
					}
					else
					{
						results[num9] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case BuildingStatusType.Education:
			case BuildingStatusType.Health:
			case BuildingStatusType.Age:
			case BuildingStatusType.Happiness:
			case BuildingStatusType.Wellbeing:
			{
				BufferAccessor<Renter> bufferAccessor8 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
				for (int num17 = 0; num17 < bufferAccessor8.Length; num17++)
				{
					DynamicBuffer<Renter> val8 = bufferAccessor8[num17];
					float2 val9 = default(float2);
					for (int num18 = 0; num18 < val8.Length; num18++)
					{
						Entity renter2 = val8[num18].m_Renter;
						if (!m_HouseholdCitizens.HasBuffer(renter2))
						{
							continue;
						}
						DynamicBuffer<HouseholdCitizen> val10 = m_HouseholdCitizens[renter2];
						for (int num19 = 0; num19 < val10.Length; num19++)
						{
							Entity citizen = val10[num19].m_Citizen;
							if (!m_Citizens.HasComponent(citizen))
							{
								continue;
							}
							Citizen citizen2 = m_Citizens[citizen];
							CitizenAge age = citizen2.GetAge();
							switch (statusData.m_Type)
							{
							case BuildingStatusType.Education:
								if (age == CitizenAge.Adult)
								{
									val9 += new float2((float)citizen2.GetEducationLevel(), 1f);
								}
								break;
							case BuildingStatusType.Health:
								val9 += new float2((float)(int)citizen2.m_Health, 1f);
								break;
							case BuildingStatusType.Wellbeing:
								val9 += new float2((float)(int)citizen2.m_WellBeing, 1f);
								break;
							case BuildingStatusType.Happiness:
								val9 += new float2((float)citizen2.Happiness, 1f);
								break;
							case BuildingStatusType.Age:
								val9 += new float2((float)age, 1f);
								break;
							}
						}
					}
					if (val9.y > 0f)
					{
						results[num17] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, val9.x / val9.y) * 255f), 0, 255));
					}
					else
					{
						results[num17] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case BuildingStatusType.Level:
			{
				NativeArray<PrefabRef> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				for (int num15 = 0; num15 < nativeArray12.Length; num15++)
				{
					Entity prefab2 = nativeArray12[num15].m_Prefab;
					if (m_SpawnableBuildingDatas.HasComponent(prefab2))
					{
						float status6 = (int)m_SpawnableBuildingDatas[prefab2].m_Level;
						results[num15] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, status6) * 255f), 0, 255));
					}
					else
					{
						results[num15] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case BuildingStatusType.GarbageAccumulation:
			{
				NativeArray<GarbageProducer> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GarbageProducer>(ref m_GarbageProducerType);
				for (int num2 = 0; num2 < nativeArray6.Length; num2++)
				{
					results[num2] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, nativeArray6[num2].m_Garbage) * 255f), 0, 255));
				}
				break;
			}
			case BuildingStatusType.Profitability:
			{
				BufferAccessor<Renter> bufferAccessor12 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
				for (int num21 = 0; num21 < bufferAccessor12.Length; num21++)
				{
					DynamicBuffer<Renter> val11 = bufferAccessor12[num21];
					bool flag3 = false;
					for (int num22 = 0; num22 < val11.Length; num22++)
					{
						Entity renter3 = val11[num22].m_Renter;
						if (m_Profitabilities.HasComponent(renter3))
						{
							results[num21] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, (int)m_Profitabilities[renter3].m_Profitability) * 255f), 0, 255));
							flag3 = true;
							break;
						}
					}
					if (!flag3)
					{
						results[num21] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case BuildingStatusType.LeisureProvider:
				if (((ArchetypeChunk)(ref chunk)).Has<Renter>(ref m_RenterType))
				{
					BufferAccessor<Renter> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
					for (int num6 = 0; num6 < bufferAccessor4.Length; num6++)
					{
						DynamicBuffer<Renter> val3 = bufferAccessor4[num6];
						bool flag = false;
						Enumerator<Renter> enumerator = val3.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								Renter current = enumerator.Current;
								if (m_LeisureProviders.HasComponent(current.m_Renter))
								{
									results[num6] = new Game.Objects.Color((byte)activeData.m_Index, byte.MaxValue);
									flag = true;
								}
							}
						}
						finally
						{
							((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
						}
						if (!flag)
						{
							results[num6] = default(Game.Objects.Color);
						}
					}
				}
				else
				{
					for (int num7 = 0; num7 < ((ArchetypeChunk)(ref chunk)).Count; num7++)
					{
						results[num7] = new Game.Objects.Color((byte)activeData.m_Index, byte.MaxValue);
					}
				}
				break;
			case BuildingStatusType.ElectricityConsumption:
			{
				NativeArray<ElectricityConsumer> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityConsumer>(ref m_ElectricityConsumerType);
				for (int l = 0; l < nativeArray5.Length; l++)
				{
					ElectricityConsumer electricityConsumer = nativeArray5[l];
					if (electricityConsumer.m_WantedConsumption > 0)
					{
						if (electricityConsumer.m_CooldownCounter >= DispatchElectricitySystem.kAlertCooldown)
						{
							results[l] = new Game.Objects.Color((byte)activeData.m_Index, 0);
							continue;
						}
						float status3 = math.log10((float)math.max(electricityConsumer.m_WantedConsumption, 1)) / math.log10(20000f);
						results[l] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, status3) * 255f), 0, 255));
					}
					else
					{
						results[l] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case BuildingStatusType.NetworkQuality:
			{
				NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				for (int k = 0; k < nativeArray3.Length; k++)
				{
					Transform transform = nativeArray3[k];
					PrefabRef prefabRef = nativeArray4[k];
					if ((m_BuildingData[prefabRef.m_Prefab].m_Flags & Game.Prefabs.BuildingFlags.RequireRoad) != 0)
					{
						float status2 = TelecomCoverage.SampleNetworkQuality(m_TelecomCoverageData, transform.m_Position);
						results[k] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, status2) * 255f), 0, 255));
					}
					else
					{
						results[k] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case BuildingStatusType.AirPollutionSource:
			case BuildingStatusType.GroundPollutionSource:
			case BuildingStatusType.NoisePollutionSource:
			{
				NativeArray<PrefabRef> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				bool destroyed = ((ArchetypeChunk)(ref chunk)).Has<Destroyed>(ref m_DestroyedType);
				bool abandoned = ((ArchetypeChunk)(ref chunk)).Has<Abandoned>(ref m_AbandonedType);
				bool isPark = ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Park>(ref m_ParkType);
				BufferAccessor<Efficiency> bufferAccessor9 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_BuildingEfficiencyType);
				BufferAccessor<Renter> bufferAccessor10 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
				BufferAccessor<InstalledUpgrade> bufferAccessor11 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
				DynamicBuffer<CityModifier> cityModifiers = default(DynamicBuffer<CityModifier>);
				m_CityModifiers.TryGetBuffer(m_City, ref cityModifiers);
				for (int num20 = 0; num20 < nativeArray13.Length; num20++)
				{
					Entity prefab3 = nativeArray13[num20].m_Prefab;
					float efficiency = BuildingUtils.GetEfficiency(bufferAccessor9, num20);
					DynamicBuffer<Renter> renters = ((bufferAccessor10.Length != 0) ? bufferAccessor10[num20] : default(DynamicBuffer<Renter>));
					DynamicBuffer<InstalledUpgrade> installedUpgrades = ((bufferAccessor11.Length != 0) ? bufferAccessor11[num20] : default(DynamicBuffer<InstalledUpgrade>));
					float value = BuildingPollutionAddSystem.GetBuildingPollution(prefab3, destroyed, abandoned, isPark, efficiency, renters, installedUpgrades, m_PollutionParameters, cityModifiers, ref m_Prefabs, ref m_BuildingData, ref m_SpawnableBuildingDatas, ref m_PollutionData, ref m_PollutionModifierData, ref m_ZoneDatas, ref m_Employees, ref m_HouseholdCitizens, ref m_Citizens).GetValue(statusData.m_Type);
					if (value > 0f)
					{
						results[num20] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, value) * 255f), 0, 255));
					}
					else
					{
						results[num20] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case BuildingStatusType.LodgingProvider:
			{
				BufferAccessor<Renter> bufferAccessor7 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
				PrefabRef prefabRef3 = default(PrefabRef);
				IndustrialProcessData industrialProcessData = default(IndustrialProcessData);
				for (int num16 = 0; num16 < bufferAccessor7.Length; num16++)
				{
					DynamicBuffer<Renter> val7 = bufferAccessor7[num16];
					bool flag2 = false;
					Enumerator<Renter> enumerator = val7.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							Renter current2 = enumerator.Current;
							if (m_Prefabs.TryGetComponent(current2.m_Renter, ref prefabRef3) && m_IndustrialProcessData.TryGetComponent(prefabRef3.m_Prefab, ref industrialProcessData) && (industrialProcessData.m_Output.m_Resource & Resource.Lodging) != Resource.NoResource)
							{
								results[num16] = new Game.Objects.Color((byte)activeData.m_Index, byte.MaxValue);
								flag2 = true;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
					}
					if (!flag2)
					{
						results[num16] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case BuildingStatusType.WaterPollutionSource:
			{
				NativeArray<PrefabRef> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				SewageOutletData sewageOutletData = default(SewageOutletData);
				for (int num14 = 0; num14 < nativeArray11.Length; num14++)
				{
					Entity prefab = nativeArray11[num14].m_Prefab;
					if (m_SewageOutletData.TryGetComponent(prefab, ref sewageOutletData))
					{
						float status5 = 1f - sewageOutletData.m_Purification;
						results[num14] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, status5) * 255f), 0, 255));
					}
					else
					{
						results[num14] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case BuildingStatusType.WaterConsumption:
			{
				NativeArray<WaterConsumer> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterConsumer>(ref m_WaterConsumerType);
				for (int num11 = 0; num11 < nativeArray8.Length; num11++)
				{
					WaterConsumer waterConsumer = nativeArray8[num11];
					if (waterConsumer.m_WantedConsumption > 0)
					{
						if (waterConsumer.m_FreshCooldownCounter >= DispatchWaterSystem.kAlertCooldown || waterConsumer.m_SewageCooldownCounter >= DispatchWaterSystem.kAlertCooldown)
						{
							results[num11] = new Game.Objects.Color((byte)activeData.m_Index, 0);
							continue;
						}
						float status4 = math.log10((float)math.max(waterConsumer.m_WantedConsumption, 1)) / math.log10(20000f);
						results[num11] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, status4) * 255f), 0, 255));
					}
					else
					{
						results[num11] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case BuildingStatusType.IndustrialBuilding:
			case BuildingStatusType.OfficeBuilding:
			case BuildingStatusType.SignatureIndustrial:
			case BuildingStatusType.SignatureOffice:
			{
				BufferAccessor<Renter> bufferAccessor5 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
				((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				for (int num8 = 0; num8 < bufferAccessor5.Length; num8++)
				{
					DynamicBuffer<Renter> val4 = bufferAccessor5[num8];
					results[num8] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, val4.Length) * 255f), 0, 255));
				}
				break;
			}
			case BuildingStatusType.ResidentialBuilding:
			case BuildingStatusType.SignatureResidential:
			{
				BufferAccessor<Renter> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
				NativeArray<PrefabRef> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
				Bounds1 range = default(Bounds1);
				for (int num3 = 0; num3 < bufferAccessor3.Length; num3++)
				{
					DynamicBuffer<Renter> val2 = bufferAccessor3[num3];
					PrefabRef prefabRef2 = nativeArray7[num3];
					float max = statusData.m_Range.max;
					if (((ArchetypeChunk)(ref chunk)).Has<ResidentialProperty>(ref m_ResidentialPropertyType) && m_BuildingPropertyData.TryGetComponent(prefabRef2.m_Prefab, ref buildingPropertyData))
					{
						max = buildingPropertyData.m_ResidentialProperties;
					}
					int num4 = 0;
					for (int num5 = 0; num5 < val2.Length; num5++)
					{
						if (m_Households.HasComponent(val2[num5].m_Renter))
						{
							num4++;
						}
					}
					range.min = statusData.m_Range.min;
					range.max = max;
					statusData.m_Range = range;
					results[num3] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, num4) * 255f), 0, 255));
				}
				break;
			}
			case BuildingStatusType.CommercialBuilding:
			case BuildingStatusType.SignatureCommercial:
			{
				BufferAccessor<Renter> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
				for (int m = 0; m < bufferAccessor2.Length; m++)
				{
					DynamicBuffer<Renter> val = bufferAccessor2[m];
					int num = 0;
					for (int n = 0; n < val.Length; n++)
					{
						if (m_CommercialCompanies.HasComponent(val[n].m_Renter))
						{
							num++;
						}
					}
					results[m] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, num) * 255f), 0, 255));
				}
				break;
			}
			case BuildingStatusType.HomelessCount:
			{
				if (!((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Park>(ref m_ParkType) && !((ArchetypeChunk)(ref chunk)).Has<Abandoned>(ref m_AbandonedType))
				{
					break;
				}
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				BufferAccessor<Renter> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					if (BuildingUtils.IsHomelessShelterBuilding(nativeArray[i], ref m_ParkData, ref m_AbandonedData) && bufferAccessor[i].Length > 0)
					{
						results[i] = new Game.Objects.Color((byte)activeData.m_Index, (byte)Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, 1f) * 255f));
					}
					else
					{
						results[i] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case BuildingStatusType.LandValue:
				break;
			}
		}

		private bool GetTransportStopColor(ArchetypeChunk chunk, out int index)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			index = 0;
			int num = int.MaxValue;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewTransportStopData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewTransportStopData>(ref m_InfoviewTransportStopType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfomodeActive infomodeActive = nativeArray2[j];
					int priority = infomodeActive.m_Priority;
					if (priority < num && HasTransportStopColor(nativeArray[j], chunk))
					{
						index = infomodeActive.m_Index;
						num = priority;
					}
				}
			}
			return num != int.MaxValue;
		}

		private bool HasTransportStopColor(InfoviewTransportStopData infoviewTransportStopData, ArchetypeChunk chunk)
		{
			return infoviewTransportStopData.m_Type switch
			{
				TransportType.Bus => ((ArchetypeChunk)(ref chunk)).Has<BusStop>(ref m_BusStopType), 
				TransportType.Train => ((ArchetypeChunk)(ref chunk)).Has<TrainStop>(ref m_TrainStopType), 
				TransportType.Taxi => ((ArchetypeChunk)(ref chunk)).Has<TaxiStand>(ref m_TaxiStandType), 
				TransportType.Tram => ((ArchetypeChunk)(ref chunk)).Has<TramStop>(ref m_TramStopType), 
				TransportType.Ship => ((ArchetypeChunk)(ref chunk)).Has<ShipStop>(ref m_ShipStopType), 
				TransportType.Post => ((ArchetypeChunk)(ref chunk)).Has<Game.Routes.MailBox>(ref m_MailBoxType), 
				TransportType.Airplane => ((ArchetypeChunk)(ref chunk)).Has<AirplaneStop>(ref m_AirplaneStopType), 
				TransportType.Subway => ((ArchetypeChunk)(ref chunk)).Has<SubwayStop>(ref m_SubwayStopType), 
				_ => false, 
			};
		}

		private bool GetVehicleColor(ArchetypeChunk chunk, out int index)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			index = 0;
			int num = int.MaxValue;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewVehicleData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewVehicleData>(ref m_InfoviewVehicleType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfomodeActive infomodeActive = nativeArray2[j];
					int priority = infomodeActive.m_Priority;
					if (priority < num && HasVehicleColor(nativeArray[j], chunk))
					{
						index = infomodeActive.m_Index;
						num = priority;
					}
				}
			}
			return num != int.MaxValue;
		}

		private bool HasVehicleColor(InfoviewVehicleData infoviewVehicleData, ArchetypeChunk chunk)
		{
			return infoviewVehicleData.m_Type switch
			{
				VehicleType.PassengerTransport => ((ArchetypeChunk)(ref chunk)).Has<PassengerTransport>(ref m_PassengerTransportType), 
				VehicleType.CargoTransport => ((ArchetypeChunk)(ref chunk)).Has<Game.Vehicles.CargoTransport>(ref m_CargoTransportType), 
				VehicleType.Taxi => ((ArchetypeChunk)(ref chunk)).Has<Game.Vehicles.Taxi>(ref m_TaxiType), 
				VehicleType.ParkMaintenance => ((ArchetypeChunk)(ref chunk)).Has<ParkMaintenanceVehicle>(ref m_ParkMaintenanceVehicleType), 
				VehicleType.RoadMaintenance => ((ArchetypeChunk)(ref chunk)).Has<RoadMaintenanceVehicle>(ref m_RoadMaintenanceVehicleType), 
				VehicleType.Ambulance => ((ArchetypeChunk)(ref chunk)).Has<Game.Vehicles.Ambulance>(ref m_AmbulanceType), 
				VehicleType.EvacuatingTransport => ((ArchetypeChunk)(ref chunk)).Has<EvacuatingTransport>(ref m_EvacuatingTransportType), 
				VehicleType.FireEngine => ((ArchetypeChunk)(ref chunk)).Has<Game.Vehicles.FireEngine>(ref m_FireEngineType), 
				VehicleType.GarbageTruck => ((ArchetypeChunk)(ref chunk)).Has<Game.Vehicles.GarbageTruck>(ref m_GarbageTruckType), 
				VehicleType.Hearse => ((ArchetypeChunk)(ref chunk)).Has<Game.Vehicles.Hearse>(ref m_HearseType), 
				VehicleType.PoliceCar => ((ArchetypeChunk)(ref chunk)).Has<Game.Vehicles.PoliceCar>(ref m_PoliceCarType), 
				VehicleType.PostVan => ((ArchetypeChunk)(ref chunk)).Has<Game.Vehicles.PostVan>(ref m_PostVanType), 
				VehicleType.PrisonerTransport => ((ArchetypeChunk)(ref chunk)).Has<PrisonerTransport>(ref m_PrisonerTransportType), 
				_ => false, 
			};
		}

		private bool GetObjectStatusType(ArchetypeChunk chunk, bool isSubBuilding, out InfoviewObjectStatusData statusData, out InfomodeActive activeData)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			statusData = default(InfoviewObjectStatusData);
			activeData = default(InfomodeActive);
			int num = int.MaxValue;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewObjectStatusData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewObjectStatusData>(ref m_InfoviewObjectStatusType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfomodeActive infomodeActive = nativeArray2[j];
					int priority = infomodeActive.m_Priority;
					if (priority < num)
					{
						InfoviewObjectStatusData infoviewObjectStatusData = nativeArray[j];
						if (HasObjectStatus(nativeArray[j], chunk, isSubBuilding))
						{
							statusData = infoviewObjectStatusData;
							activeData = infomodeActive;
							num = priority;
						}
					}
				}
			}
			return num != int.MaxValue;
		}

		private bool HasObjectStatus(InfoviewObjectStatusData infoviewObjectStatusData, ArchetypeChunk chunk, bool isSubBuilding)
		{
			switch (infoviewObjectStatusData.m_Type)
			{
			case ObjectStatusType.WoodResource:
				return ((ArchetypeChunk)(ref chunk)).Has<Tree>(ref m_TreeType);
			case ObjectStatusType.FireHazard:
				if (!isSubBuilding)
				{
					if (!((ArchetypeChunk)(ref chunk)).Has<Building>(ref m_BuildingType))
					{
						return ((ArchetypeChunk)(ref chunk)).Has<Tree>(ref m_TreeType);
					}
					return true;
				}
				return false;
			case ObjectStatusType.Damage:
				return ((ArchetypeChunk)(ref chunk)).Has<Damaged>(ref m_DamagedType);
			case ObjectStatusType.Destroyed:
				return ((ArchetypeChunk)(ref chunk)).Has<Destroyed>(ref m_DestroyedType);
			case ObjectStatusType.ExtractorPlaceholder:
				return ((ArchetypeChunk)(ref chunk)).Has<Placeholder>(ref m_PlaceholderType);
			case ObjectStatusType.Tourist:
				if (!((ArchetypeChunk)(ref chunk)).Has<Game.Creatures.Resident>(ref m_ResidentType))
				{
					if (((ArchetypeChunk)(ref chunk)).Has<Vehicle>(ref m_VehicleType))
					{
						return ((ArchetypeChunk)(ref chunk)).Has<Passenger>(ref m_PassengerType);
					}
					return false;
				}
				return true;
			default:
				return false;
			}
		}

		private void GetObjectStatusColors(NativeArray<Game.Objects.Color> results, ArchetypeChunk chunk, InfoviewObjectStatusData statusData, InfomodeActive activeData)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0520: Unknown result type (might be due to invalid IL or missing references)
			switch (statusData.m_Type)
			{
			case ObjectStatusType.WoodResource:
			{
				NativeArray<Tree> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Tree>(ref m_TreeType);
				NativeArray<Plant> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Plant>(ref m_PlantType);
				NativeArray<Damaged> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Damaged>(ref m_DamagedType);
				NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				Damaged damaged = default(Damaged);
				for (int k = 0; k < nativeArray5.Length; k++)
				{
					Tree tree = nativeArray2[k];
					Plant plant = nativeArray3[k];
					PrefabRef prefabRef = nativeArray5[k];
					CollectionUtils.TryGet<Damaged>(nativeArray4, k, ref damaged);
					if (m_TreeData.HasComponent(prefabRef.m_Prefab))
					{
						TreeData treeData = m_TreeData[prefabRef.m_Prefab];
						float status = ObjectUtils.CalculateWoodAmount(tree, plant, damaged, treeData);
						results[k] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, status) * 255f), 0, 255));
					}
					else
					{
						results[k] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case ObjectStatusType.FireHazard:
			{
				NativeArray<Building> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
				NativeArray<PrefabRef> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				float fireHazard;
				if (nativeArray6.Length != 0)
				{
					NativeArray<CurrentDistrict> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentDistrict>(ref m_CurrentDistrictType);
					NativeArray<Damaged> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Damaged>(ref m_DamagedType);
					NativeArray<UnderConstruction> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<UnderConstruction>(ref m_UnderConstructionType);
					Damaged damaged2 = default(Damaged);
					UnderConstruction underConstruction = default(UnderConstruction);
					for (int l = 0; l < nativeArray6.Length; l++)
					{
						PrefabRef prefabRef2 = nativeArray7[l];
						Building building = nativeArray6[l];
						CurrentDistrict currentDistrict = nativeArray8[l];
						CollectionUtils.TryGet<Damaged>(nativeArray9, l, ref damaged2);
						if (!CollectionUtils.TryGet<UnderConstruction>(nativeArray10, l, ref underConstruction))
						{
							underConstruction = new UnderConstruction
							{
								m_Progress = byte.MaxValue
							};
						}
						if (m_FireHazardData.GetFireHazard(prefabRef2, building, currentDistrict, damaged2, underConstruction, out fireHazard, out var riskFactor))
						{
							results[l] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, riskFactor) * 255f), 0, 255));
						}
						else
						{
							results[l] = default(Game.Objects.Color);
						}
					}
					break;
				}
				NativeArray<Damaged> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Damaged>(ref m_DamagedType);
				NativeArray<Transform> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				Damaged damaged3 = default(Damaged);
				for (int m = 0; m < nativeArray12.Length; m++)
				{
					PrefabRef prefabRef3 = nativeArray7[m];
					Transform transform = nativeArray12[m];
					CollectionUtils.TryGet<Damaged>(nativeArray11, m, ref damaged3);
					if (m_FireHazardData.GetFireHazard(prefabRef3, default(Tree), transform, damaged3, out fireHazard, out var riskFactor2))
					{
						results[m] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, riskFactor2) * 255f), 0, 255));
					}
					else
					{
						results[m] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case ObjectStatusType.Damage:
			{
				NativeArray<Damaged> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Damaged>(ref m_DamagedType);
				for (int num = 0; num < nativeArray13.Length; num++)
				{
					float totalDamage = ObjectUtils.GetTotalDamage(nativeArray13[num]);
					results[num] = new Game.Objects.Color((byte)activeData.m_Index, (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, totalDamage) * 255f), 0, 255));
				}
				break;
			}
			case ObjectStatusType.Destroyed:
			{
				for (int n = 0; n < results.Length; n++)
				{
					results[n] = new Game.Objects.Color((byte)activeData.m_Index, 0);
				}
				break;
			}
			case ObjectStatusType.ExtractorPlaceholder:
			{
				NativeArray<PrefabRef> nativeArray14 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				PlaceholderBuildingData placeholderBuildingData = default(PlaceholderBuildingData);
				for (int num2 = 0; num2 < nativeArray14.Length; num2++)
				{
					PrefabRef prefabRef4 = nativeArray14[num2];
					if (m_PlaceholderBuildingData.TryGetComponent(prefabRef4.m_Prefab, ref placeholderBuildingData) && placeholderBuildingData.m_Type == BuildingType.ExtractorBuilding)
					{
						results[num2] = new Game.Objects.Color((byte)activeData.m_Index, byte.MaxValue);
					}
					else
					{
						results[num2] = default(Game.Objects.Color);
					}
				}
				break;
			}
			case ObjectStatusType.Tourist:
			{
				NativeArray<Game.Creatures.Resident> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Creatures.Resident>(ref m_ResidentType);
				if (nativeArray.Length != 0)
				{
					Citizen citizen = default(Citizen);
					for (int i = 0; i < nativeArray.Length; i++)
					{
						if (m_Citizens.TryGetComponent(nativeArray[i].m_Citizen, ref citizen) && (citizen.m_State & CitizenFlags.Tourist) != CitizenFlags.None)
						{
							results[i] = new Game.Objects.Color((byte)activeData.m_Index, byte.MaxValue);
						}
						else
						{
							results[i] = default(Game.Objects.Color);
						}
					}
					break;
				}
				BufferAccessor<Passenger> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Passenger>(ref m_PassengerType);
				Game.Creatures.Resident resident = default(Game.Creatures.Resident);
				Citizen citizen2 = default(Citizen);
				for (int j = 0; j < bufferAccessor.Length; j++)
				{
					DynamicBuffer<Passenger> val = bufferAccessor[j];
					bool flag = false;
					Enumerator<Passenger> enumerator = val.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							Passenger current = enumerator.Current;
							if (m_ResidentData.TryGetComponent(current.m_Passenger, ref resident) && m_Citizens.TryGetComponent(resident.m_Citizen, ref citizen2) && (citizen2.m_State & CitizenFlags.Tourist) != CitizenFlags.None)
							{
								results[j] = new Game.Objects.Color((byte)activeData.m_Index, byte.MaxValue);
								flag = true;
								break;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
					}
					if (!flag)
					{
						results[j] = default(Game.Objects.Color);
					}
				}
				break;
			}
			}
		}

		private bool GetNetStatusColors(NativeArray<Game.Objects.Color> results, ArchetypeChunk chunk)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = int.MaxValue;
			int num7 = int.MaxValue;
			int num8 = int.MaxValue;
			int num9 = int.MaxValue;
			int num10 = int.MaxValue;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewNetStatusData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewNetStatusData>(ref m_InfoviewNetStatusType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfoviewNetStatusData infoviewNetStatusData = nativeArray[j];
					InfomodeActive infomodeActive = nativeArray2[j];
					int priority = infomodeActive.m_Priority;
					switch (infoviewNetStatusData.m_Type)
					{
					case NetStatusType.LowVoltageFlow:
						if (priority < num6)
						{
							num = infomodeActive.m_Index;
							num6 = priority;
						}
						break;
					case NetStatusType.HighVoltageFlow:
						if (priority < num7)
						{
							num2 = infomodeActive.m_Index;
							num7 = priority;
						}
						break;
					case NetStatusType.PipeWaterFlow:
						if (priority < num8)
						{
							num3 = infomodeActive.m_Index;
							num8 = priority;
						}
						break;
					case NetStatusType.PipeSewageFlow:
						if (priority < num9)
						{
							num4 = infomodeActive.m_Index;
							num9 = priority;
						}
						break;
					case NetStatusType.OilFlow:
						if (priority < num10)
						{
							num5 = infomodeActive.m_Index;
							num10 = priority;
						}
						break;
					}
				}
			}
			if (num == 0 && num2 == 0 && num3 == 0 && num4 == 0 && num5 == 0)
			{
				return false;
			}
			NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			UtilityObjectData utilityObjectData = default(UtilityObjectData);
			ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
			DynamicBuffer<ConnectedFlowEdge> val2 = default(DynamicBuffer<ConnectedFlowEdge>);
			ElectricityFlowEdge electricityFlowEdge = default(ElectricityFlowEdge);
			ElectricityConsumer electricityConsumer = default(ElectricityConsumer);
			WaterPipeNodeConnection waterPipeNodeConnection = default(WaterPipeNodeConnection);
			DynamicBuffer<ConnectedFlowEdge> val3 = default(DynamicBuffer<ConnectedFlowEdge>);
			WaterPipeEdge waterPipeEdge = default(WaterPipeEdge);
			WaterConsumer waterConsumer = default(WaterConsumer);
			DynamicBuffer<ConnectedEdge> val4 = default(DynamicBuffer<ConnectedEdge>);
			bool2 val5 = default(bool2);
			Game.Net.ResourceConnection resourceConnection = default(Game.Net.ResourceConnection);
			Game.Net.ResourceConnection resourceConnection2 = default(Game.Net.ResourceConnection);
			Game.Net.ResourceConnection resourceConnection3 = default(Game.Net.ResourceConnection);
			for (int k = 0; k < results.Length; k++)
			{
				PrefabRef prefabRef = nativeArray4[k];
				if (m_PrefabUtilityObjectData.TryGetComponent(prefabRef.m_Prefab, ref utilityObjectData))
				{
					int num11 = 0;
					if ((utilityObjectData.m_UtilityTypes & UtilityTypes.LowVoltageLine) != UtilityTypes.None && num != 0)
					{
						num11 = num;
					}
					else if ((utilityObjectData.m_UtilityTypes & UtilityTypes.HighVoltageLine) != UtilityTypes.None && num2 != 0)
					{
						num11 = num2;
					}
					else if ((utilityObjectData.m_UtilityTypes & UtilityTypes.WaterPipe) != UtilityTypes.None && num3 != 0)
					{
						num11 = num3;
					}
					else if ((utilityObjectData.m_UtilityTypes & UtilityTypes.SewagePipe) != UtilityTypes.None && num4 != 0)
					{
						num11 = num4;
					}
					else if ((utilityObjectData.m_UtilityTypes & UtilityTypes.Resource) != UtilityTypes.None && num5 != 0)
					{
						num11 = num5;
					}
					if (num11 != 0)
					{
						int num12 = 0;
						if (nativeArray3.Length != 0)
						{
							Owner owner = nativeArray3[k];
							if (num11 == num || num11 == num2)
							{
								if (m_ElectricityNodeConnectionData.TryGetComponent(owner.m_Owner, ref electricityNodeConnection) && m_ConnectedFlowEdges.TryGetBuffer(electricityNodeConnection.m_ElectricityNode, ref val2))
								{
									ElectricityFlowEdgeFlags electricityFlowEdgeFlags = ElectricityFlowEdgeFlags.None;
									for (int l = 0; l < val2.Length; l++)
									{
										if (m_ElectricityFlowEdgeData.TryGetComponent(val2[l].m_Edge, ref electricityFlowEdge))
										{
											num12 = math.max(num12, math.select(0, 128, electricityFlowEdge.m_Flow != 0));
											electricityFlowEdgeFlags |= electricityFlowEdge.m_Flags;
										}
									}
									num12 = math.select(num12, 224, num12 != 0 && (electricityFlowEdgeFlags & ElectricityFlowEdgeFlags.BeyondBottleneck) != 0);
									num12 = math.select(num12, 255, num12 != 0 && (electricityFlowEdgeFlags & ElectricityFlowEdgeFlags.Bottleneck) != 0);
								}
								else if (m_ElectricityConsumerData.TryGetComponent(owner.m_Owner, ref electricityConsumer))
								{
									num12 = math.max(num12, math.select(0, 128, electricityConsumer.m_FulfilledConsumption != 0));
									num12 = math.select(num12, 224, num12 != 0 && (electricityConsumer.m_Flags & ElectricityConsumerFlags.BottleneckWarning) != 0);
								}
							}
							else if (num11 == num3 || num11 == num4)
							{
								if (m_WaterPipeNodeConnectionData.TryGetComponent(owner.m_Owner, ref waterPipeNodeConnection) && m_ConnectedFlowEdges.TryGetBuffer(waterPipeNodeConnection.m_WaterPipeNode, ref val3))
								{
									float num13 = 0f;
									for (int m = 0; m < val3.Length; m++)
									{
										if (m_WaterPipeEdgeData.TryGetComponent(val3[m].m_Edge, ref waterPipeEdge))
										{
											if (num11 == num3)
											{
												num12 = math.max(num12, math.select(0, 128, waterPipeEdge.m_FreshFlow != 0));
												num13 = math.max(num13, waterPipeEdge.m_FreshPollution);
											}
											else
											{
												num12 = math.max(num12, math.select(0, 128, waterPipeEdge.m_SewageFlow != 0));
											}
										}
									}
									num12 = math.select(num12, 255, num12 != 0 && num13 > 0f);
								}
								else if (m_WaterConsumerData.TryGetComponent(owner.m_Owner, ref waterConsumer))
								{
									if (num11 == num3)
									{
										num12 = math.max(num12, math.select(0, 128, waterConsumer.m_FulfilledFresh != 0));
										num12 = math.select(num12, 255, num12 != 0 && waterConsumer.m_Pollution > 0f);
									}
									else
									{
										num12 = math.max(num12, math.select(0, 128, waterConsumer.m_FulfilledSewage != 0));
									}
								}
							}
							else if (num11 == num5)
							{
								if (m_ConnectedEdges.TryGetBuffer(owner.m_Owner, ref val4))
								{
									bool flag = false;
									for (int n = 0; n < val4.Length; n++)
									{
										ConnectedEdge connectedEdge = val4[n];
										Edge edge = m_EdgeData[connectedEdge.m_Edge];
										((bool2)(ref val5))._002Ector(edge.m_Start == owner.m_Owner, edge.m_End == owner.m_Owner);
										if (math.any(val5))
										{
											flag = true;
											if (m_ResourceConnectionData.TryGetComponent(connectedEdge.m_Edge, ref resourceConnection))
											{
												int num14 = math.select(resourceConnection.m_Flow.x, resourceConnection.m_Flow.y, val5.y);
												num12 = math.max(num12, math.select(0, 128, num14 != 0));
											}
										}
									}
									if (!flag && m_ResourceConnectionData.TryGetComponent(owner.m_Owner, ref resourceConnection2))
									{
										num12 = math.max(num12, math.select(0, 128, resourceConnection2.m_Flow.x != 0));
									}
								}
								else if (m_ResourceConnectionData.TryGetComponent(owner.m_Owner, ref resourceConnection3))
								{
									num12 = math.max(num12, math.select(0, 128, resourceConnection3.m_Flow.x != 0 || resourceConnection3.m_Flow.y != 0));
								}
							}
						}
						results[k] = new Game.Objects.Color((byte)num11, (byte)num12);
						continue;
					}
				}
				results[k] = default(Game.Objects.Color);
			}
			return true;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateMiddleObjectColorsJob : IJobChunk
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_InfomodeChunks;

		[ReadOnly]
		public ComponentTypeHandle<InfomodeActive> m_InfomodeActiveType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewObjectStatusData> m_InfoviewObjectStatusType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Controller> m_ControllerType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Objects.Color> m_ColorData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			if (((ArchetypeChunk)(ref chunk)).Has<Building>(ref m_BuildingType))
			{
				NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				int2 excludedSubBuildingActiveIndices = GetExcludedSubBuildingActiveIndices();
				Game.Objects.Color color2 = default(Game.Objects.Color);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity val = nativeArray[i];
					Owner owner = nativeArray2[i];
					Game.Objects.Color color = m_ColorData[val];
					if (color.m_Index == 0 && m_ColorData.TryGetComponent(owner.m_Owner, ref color2) && math.all((int)color2.m_Index != excludedSubBuildingActiveIndices))
					{
						color.m_Index = color2.m_Index;
						color.m_Value = color2.m_Value;
						m_ColorData[val] = color;
					}
				}
				return;
			}
			NativeArray<Controller> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Controller>(ref m_ControllerType);
			if (nativeArray3.Length != 0)
			{
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val2 = nativeArray[j];
					Controller controller = nativeArray3[j];
					if (controller.m_Controller != val2)
					{
						Game.Objects.Color color3 = m_ColorData[val2];
						if (color3.m_Index == 0 && m_ColorData.TryGetComponent(controller.m_Controller, ref color3))
						{
							m_ColorData[val2] = color3;
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Entity val3 = nativeArray[k];
					m_ColorData[val3] = default(Game.Objects.Color);
				}
			}
		}

		private int2 GetExcludedSubBuildingActiveIndices()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			int2 result = int2.op_Implicit(-1);
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewObjectStatusData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewObjectStatusData>(ref m_InfoviewObjectStatusType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfomodeActive infomodeActive = nativeArray2[j];
					switch (nativeArray[j].m_Type)
					{
					case ObjectStatusType.Damage:
						result.x = infomodeActive.m_Index;
						break;
					case ObjectStatusType.Destroyed:
						result.y = infomodeActive.m_Index;
						break;
					}
				}
			}
			return result;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateTempObjectColorsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Objects.Color> m_ColorData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			Game.Objects.Color color = default(Game.Objects.Color);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Temp temp = nativeArray2[i];
				if (m_ColorData.TryGetComponent(temp.m_Original, ref color))
				{
					m_ColorData[nativeArray[i]] = color;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateSubObjectColorsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Elevation> m_ElevationType;

		[ReadOnly]
		public ComponentTypeHandle<Tree> m_TreeType;

		[ReadOnly]
		public ComponentTypeHandle<Plant> m_PlantType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Objects.Color> m_ColorData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			if (((ArchetypeChunk)(ref chunk)).Has<Tree>(ref m_TreeType))
			{
				NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<Game.Objects.Elevation> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Objects.Elevation>(ref m_ElevationType);
				Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
				Owner owner2 = default(Owner);
				Game.Objects.Color color2 = default(Game.Objects.Color);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity val = nativeArray[i];
					Owner owner = nativeArray2[i];
					bool flag = CollectionUtils.TryGet<Game.Objects.Elevation>(nativeArray3, i, ref elevation) && (elevation.m_Flags & ElevationFlags.OnGround) == 0;
					bool flag2 = flag && !m_ColorData.HasComponent(owner.m_Owner);
					while (m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2) && !m_BuildingData.HasComponent(owner.m_Owner) && !m_VehicleData.HasComponent(owner.m_Owner))
					{
						if (flag2)
						{
							if (m_ColorData.HasComponent(owner.m_Owner))
							{
								flag2 = false;
							}
							else
							{
								flag &= m_ElevationData.TryGetComponent(owner.m_Owner, ref elevation) && (elevation.m_Flags & ElevationFlags.OnGround) == 0;
							}
						}
						owner = owner2;
					}
					Game.Objects.Color color = default(Game.Objects.Color);
					if (m_ColorData.TryGetComponent(owner.m_Owner, ref color2) && (flag || color2.m_SubColor))
					{
						color = color2;
					}
					m_ColorData[val] = color;
				}
				return;
			}
			NativeArray<Owner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			Owner owner4 = default(Owner);
			Game.Objects.Color color3 = default(Game.Objects.Color);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				Entity val2 = nativeArray[j];
				Owner owner3 = nativeArray4[j];
				while (m_OwnerData.TryGetComponent(owner3.m_Owner, ref owner4) && !m_BuildingData.HasComponent(owner3.m_Owner) && !m_VehicleData.HasComponent(owner3.m_Owner))
				{
					owner3 = owner4;
				}
				if (m_ColorData.TryGetComponent(owner3.m_Owner, ref color3))
				{
					m_ColorData[val2] = color3;
				}
				else
				{
					m_ColorData[val2] = default(Game.Objects.Color);
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfomodeActive> __Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewBuildingData> __Game_Prefabs_InfoviewBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewBuildingStatusData> __Game_Prefabs_InfoviewBuildingStatusData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewTransportStopData> __Game_Prefabs_InfoviewTransportStopData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewVehicleData> __Game_Prefabs_InfoviewVehicleData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewObjectStatusData> __Game_Prefabs_InfoviewObjectStatusData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetStatusData> __Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Hospital> __Game_Buildings_Hospital_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityProducer> __Game_Buildings_ElectricityProducer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Transformer> __Game_Buildings_Transformer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.WaterPumpingStation> __Game_Buildings_WaterPumpingStation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.WaterTower> __Game_Buildings_WaterTower_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.SewageOutlet> __Game_Buildings_SewageOutlet_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WastewaterTreatmentPlant> __Game_Buildings_WastewaterTreatmentPlant_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TransportDepot> __Game_Buildings_TransportDepot_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TransportStation> __Game_Buildings_TransportStation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.GarbageFacility> __Game_Buildings_GarbageFacility_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.FireStation> __Game_Buildings_FireStation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.PoliceStation> __Game_Buildings_PoliceStation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CrimeProducer> __Game_Buildings_CrimeProducer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RoadMaintenance> __Game_Buildings_RoadMaintenance_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkMaintenance> __Game_Buildings_ParkMaintenance_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.PostFacility> __Game_Buildings_PostFacility_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TelecomFacility> __Game_Buildings_TelecomFacility_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.School> __Game_Buildings_School_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Park> __Game_Buildings_Park_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AttractivenessProvider> __Game_Buildings_AttractivenessProvider_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.EmergencyShelter> __Game_Buildings_EmergencyShelter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.DisasterFacility> __Game_Buildings_DisasterFacility_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.FirewatchTower> __Game_Buildings_FirewatchTower_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.DeathcareFacility> __Game_Buildings_DeathcareFacility_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Prison> __Game_Buildings_Prison_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AdminBuilding> __Game_Buildings_AdminBuilding_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.WelfareOffice> __Game_Buildings_WelfareOffice_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.ResearchFacility> __Game_Buildings_ResearchFacility_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.ParkingFacility> __Game_Buildings_ParkingFacility_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Battery> __Game_Buildings_Battery_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MailProducer> __Game_Buildings_MailProducer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BuildingCondition> __Game_Buildings_BuildingCondition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ResidentialProperty> __Game_Buildings_ResidentialProperty_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CommercialProperty> __Game_Buildings_CommercialProperty_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<IndustrialProperty> __Game_Buildings_IndustrialProperty_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<OfficeProperty> __Game_Buildings_OfficeProperty_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<StorageProperty> __Game_Buildings_StorageProperty_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ExtractorProperty> __Game_Buildings_ExtractorProperty_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GarbageProducer> __Game_Buildings_GarbageProducer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Abandoned> __Game_Buildings_Abandoned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.LeisureProvider> __Game_Buildings_LeisureProvider_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Renter> __Game_Buildings_Renter_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.TransportStop> __Game_Routes_TransportStop_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BusStop> __Game_Routes_BusStop_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrainStop> __Game_Routes_TrainStop_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TaxiStand> __Game_Routes_TaxiStand_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TramStop> __Game_Routes_TramStop_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ShipStop> __Game_Routes_ShipStop_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.MailBox> __Game_Routes_MailBox_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.WorkStop> __Game_Routes_WorkStop_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AirplaneStop> __Game_Routes_AirplaneStop_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SubwayStop> __Game_Routes_SubwayStop_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PassengerTransport> __Game_Vehicles_PassengerTransport_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.CargoTransport> __Game_Vehicles_CargoTransport_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.Taxi> __Game_Vehicles_Taxi_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkMaintenanceVehicle> __Game_Vehicles_ParkMaintenanceVehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RoadMaintenanceVehicle> __Game_Vehicles_RoadMaintenanceVehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.Ambulance> __Game_Vehicles_Ambulance_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EvacuatingTransport> __Game_Vehicles_EvacuatingTransport_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.FireEngine> __Game_Vehicles_FireEngine_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.GarbageTruck> __Game_Vehicles_GarbageTruck_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.Hearse> __Game_Vehicles_Hearse_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.PoliceCar> __Game_Vehicles_PoliceCar_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.PostVan> __Game_Vehicles_PostVan_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrisonerTransport> __Game_Vehicles_PrisonerTransport_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Passenger> __Game_Vehicles_Passenger_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Creatures.Resident> __Game_Creatures_Resident_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Tree> __Game_Objects_Tree_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Plant> __Game_Objects_Plant_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.UtilityObject> __Game_Objects_UtilityObject_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Destroyed> __Game_Common_Destroyed_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Damaged> __Game_Objects_Damaged_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<UnderConstruction> __Game_Objects_UnderConstruction_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.UniqueObject> __Game_Objects_UniqueObject_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Placeholder> __Game_Objects_Placeholder_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Attached> __Game_Objects_Attached_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AccidentSite> __Game_Events_AccidentSite_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<TreeData> __Game_Prefabs_TreeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> __Game_Buildings_Park_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Abandoned> __Game_Buildings_Abandoned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PollutionData> __Game_Prefabs_PollutionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PollutionModifierData> __Game_Prefabs_PollutionModifierData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CommercialCompany> __Game_Companies_CommercialCompany_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceholderBuildingData> __Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SewageOutletData> __Game_Prefabs_SewageOutletData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Resident> __Game_Creatures_Resident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Profitability> __Game_Companies_Profitability_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.LeisureProvider> __Game_Buildings_LeisureProvider_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityObjectData> __Game_Prefabs_UtilityObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> __Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeNodeConnection> __Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> __Game_Simulation_WaterPipeEdge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ResourceConnection> __Game_Net_ResourceConnection_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		public ComponentTypeHandle<Game.Objects.Color> __Game_Objects_Color_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Controller> __Game_Vehicles_Controller_RO_ComponentTypeHandle;

		public ComponentLookup<Game.Objects.Color> __Game_Objects_Color_RW_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

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
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0627: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_0639: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0660: Unknown result type (might be due to invalid IL or missing references)
			//IL_0668: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0682: Unknown result type (might be due to invalid IL or missing references)
			//IL_0687: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfomodeActive>(true);
			__Game_Prefabs_InfoviewBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewBuildingData>(true);
			__Game_Prefabs_InfoviewBuildingStatusData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewBuildingStatusData>(true);
			__Game_Prefabs_InfoviewTransportStopData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewTransportStopData>(true);
			__Game_Prefabs_InfoviewVehicleData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewVehicleData>(true);
			__Game_Prefabs_InfoviewObjectStatusData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewObjectStatusData>(true);
			__Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewNetStatusData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Buildings_Hospital_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Hospital>(true);
			__Game_Buildings_ElectricityProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityProducer>(true);
			__Game_Buildings_Transformer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Transformer>(true);
			__Game_Buildings_WaterPumpingStation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.WaterPumpingStation>(true);
			__Game_Buildings_WaterTower_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.WaterTower>(true);
			__Game_Buildings_SewageOutlet_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.SewageOutlet>(true);
			__Game_Buildings_WastewaterTreatmentPlant_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WastewaterTreatmentPlant>(true);
			__Game_Buildings_TransportDepot_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.TransportDepot>(true);
			__Game_Buildings_TransportStation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.TransportStation>(true);
			__Game_Buildings_GarbageFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.GarbageFacility>(true);
			__Game_Buildings_FireStation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.FireStation>(true);
			__Game_Buildings_PoliceStation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.PoliceStation>(true);
			__Game_Buildings_CrimeProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CrimeProducer>(true);
			__Game_Buildings_RoadMaintenance_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RoadMaintenance>(true);
			__Game_Buildings_ParkMaintenance_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkMaintenance>(true);
			__Game_Buildings_PostFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.PostFacility>(true);
			__Game_Buildings_TelecomFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.TelecomFacility>(true);
			__Game_Buildings_School_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.School>(true);
			__Game_Buildings_Park_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Park>(true);
			__Game_Buildings_AttractivenessProvider_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AttractivenessProvider>(true);
			__Game_Buildings_EmergencyShelter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.EmergencyShelter>(true);
			__Game_Buildings_DisasterFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.DisasterFacility>(true);
			__Game_Buildings_FirewatchTower_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.FirewatchTower>(true);
			__Game_Buildings_DeathcareFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.DeathcareFacility>(true);
			__Game_Buildings_Prison_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Prison>(true);
			__Game_Buildings_AdminBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AdminBuilding>(true);
			__Game_Buildings_WelfareOffice_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.WelfareOffice>(true);
			__Game_Buildings_ResearchFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.ResearchFacility>(true);
			__Game_Buildings_ParkingFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.ParkingFacility>(true);
			__Game_Buildings_Battery_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Battery>(true);
			__Game_Buildings_MailProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MailProducer>(true);
			__Game_Buildings_BuildingCondition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingCondition>(true);
			__Game_Buildings_ResidentialProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResidentialProperty>(true);
			__Game_Buildings_CommercialProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CommercialProperty>(true);
			__Game_Buildings_IndustrialProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<IndustrialProperty>(true);
			__Game_Buildings_OfficeProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<OfficeProperty>(true);
			__Game_Buildings_StorageProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StorageProperty>(true);
			__Game_Buildings_ExtractorProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ExtractorProperty>(true);
			__Game_Buildings_GarbageProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GarbageProducer>(true);
			__Game_Buildings_Abandoned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Abandoned>(true);
			__Game_Buildings_LeisureProvider_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.LeisureProvider>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityConsumer>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterConsumer>(true);
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Buildings_Renter_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Renter>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Routes_TransportStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.TransportStop>(true);
			__Game_Routes_BusStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BusStop>(true);
			__Game_Routes_TrainStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrainStop>(true);
			__Game_Routes_TaxiStand_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TaxiStand>(true);
			__Game_Routes_TramStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TramStop>(true);
			__Game_Routes_ShipStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ShipStop>(true);
			__Game_Routes_MailBox_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.MailBox>(true);
			__Game_Routes_WorkStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.WorkStop>(true);
			__Game_Routes_AirplaneStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AirplaneStop>(true);
			__Game_Routes_SubwayStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SubwayStop>(true);
			__Game_Vehicles_Vehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Vehicle>(true);
			__Game_Vehicles_PassengerTransport_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PassengerTransport>(true);
			__Game_Vehicles_CargoTransport_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.CargoTransport>(true);
			__Game_Vehicles_Taxi_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.Taxi>(true);
			__Game_Vehicles_ParkMaintenanceVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkMaintenanceVehicle>(true);
			__Game_Vehicles_RoadMaintenanceVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RoadMaintenanceVehicle>(true);
			__Game_Vehicles_Ambulance_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.Ambulance>(true);
			__Game_Vehicles_EvacuatingTransport_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EvacuatingTransport>(true);
			__Game_Vehicles_FireEngine_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.FireEngine>(true);
			__Game_Vehicles_GarbageTruck_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.GarbageTruck>(true);
			__Game_Vehicles_Hearse_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.Hearse>(true);
			__Game_Vehicles_PoliceCar_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.PoliceCar>(true);
			__Game_Vehicles_PostVan_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.PostVan>(true);
			__Game_Vehicles_PrisonerTransport_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrisonerTransport>(true);
			__Game_Vehicles_Passenger_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Passenger>(true);
			__Game_Creatures_Resident_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Creatures.Resident>(true);
			__Game_Objects_Tree_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Tree>(true);
			__Game_Objects_Plant_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Plant>(true);
			__Game_Objects_UtilityObject_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.UtilityObject>(true);
			__Game_Common_Destroyed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Destroyed>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Damaged_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Damaged>(true);
			__Game_Objects_UnderConstruction_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UnderConstruction>(true);
			__Game_Objects_UniqueObject_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.UniqueObject>(true);
			__Game_Objects_Placeholder_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Placeholder>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentDistrict>(true);
			__Game_Objects_Attached_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Attached>(true);
			__Game_Events_AccidentSite_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AccidentSite>(true);
			__Game_Prefabs_TreeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TreeData>(true);
			__Game_Buildings_Park_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Park>(true);
			__Game_Buildings_Abandoned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Abandoned>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Prefabs_PollutionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PollutionData>(true);
			__Game_Prefabs_PollutionModifierData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PollutionModifierData>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Companies_CommercialCompany_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CommercialCompany>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceholderBuildingData>(true);
			__Game_Prefabs_SewageOutletData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SewageOutletData>(true);
			__Game_Creatures_Resident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Creatures.Resident>(true);
			__Game_Companies_Profitability_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Profitability>(true);
			__Game_Buildings_LeisureProvider_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.LeisureProvider>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Prefabs_UtilityObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityObjectData>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConsumer>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterConsumer>(true);
			__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityNodeConnection>(true);
			__Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeNodeConnection>(true);
			__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(true);
			__Game_Simulation_WaterPipeEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeEdge>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_ResourceConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ResourceConnection>(true);
			__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Objects_Color_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.Color>(false);
			__Game_Vehicles_Controller_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Controller>(true);
			__Game_Objects_Color_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Color>(false);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Objects_Elevation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.Elevation>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Elevation>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
		}
	}

	private EntityQuery m_ObjectQuery;

	private EntityQuery m_MiddleObjectQuery;

	private EntityQuery m_TempObjectQuery;

	private EntityQuery m_SubObjectQuery;

	private EntityQuery m_InfomodeQuery;

	private EntityQuery m_HappinessParameterQuery;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_PollutionParameterQuery;

	private EntityQuery m_FireConfigQuery;

	private EventHelpers.FireHazardData m_FireHazardData;

	private ToolSystem m_ToolSystem;

	private LocalEffectSystem m_LocalEffectSystem;

	private ClimateSystem m_ClimateSystem;

	private FireHazardSystem m_FireHazardSystem;

	private TelecomCoverageSystem m_TelecomCoverageSystem;

	private CitySystem m_CitySystem;

	private PrefabSystem m_PrefabSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Expected O, but got Unknown
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected O, but got Unknown
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Expected O, but got Unknown
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Expected O, but got Unknown
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Expected O, but got Unknown
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_LocalEffectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LocalEffectSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_FireHazardSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<FireHazardSystem>();
		m_TelecomCoverageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TelecomCoverageSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.Object>(),
			ComponentType.ReadWrite<Game.Objects.Color>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Hidden>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Owner>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Objects.Object>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadWrite<Game.Objects.Color>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Vehicle>(),
			ComponentType.ReadOnly<Creature>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Objects.UtilityObject>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Hidden>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[1] = val;
		m_ObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[2];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadWrite<Game.Objects.Color>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Hidden>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array2[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Vehicle>(),
			ComponentType.ReadOnly<Controller>(),
			ComponentType.ReadWrite<Game.Objects.Color>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Hidden>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array2[1] = val;
		m_MiddleObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Objects.Object>(),
			ComponentType.ReadWrite<Game.Objects.Color>(),
			ComponentType.ReadOnly<Temp>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Hidden>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array3[0] = val;
		m_TempObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		EntityQueryDesc[] array4 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Objects.Object>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadWrite<Game.Objects.Color>()
		};
		val.None = (ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Hidden>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Vehicle>(),
			ComponentType.ReadOnly<Creature>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Objects.UtilityObject>()
		};
		array4[0] = val;
		m_SubObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array4);
		EntityQueryDesc[] array5 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<InfomodeActive>() };
		val.Any = (ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<InfoviewBuildingData>(),
			ComponentType.ReadOnly<InfoviewBuildingStatusData>(),
			ComponentType.ReadOnly<InfoviewTransportStopData>(),
			ComponentType.ReadOnly<InfoviewVehicleData>(),
			ComponentType.ReadOnly<InfoviewObjectStatusData>(),
			ComponentType.ReadOnly<InfoviewNetStatusData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[0];
		array5[0] = val;
		m_InfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array5);
		m_HappinessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenHappinessParameterData>() });
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_PollutionParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PollutionParameterData>() });
		m_FireConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<FireConfigurationData>() });
		m_FireHazardData = new EventHelpers.FireHazardData((SystemBase)(object)this);
		((ComponentSystemBase)this).RequireForUpdate(m_HappinessParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0861: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0896: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_090a: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0927: Unknown result type (might be due to invalid IL or missing references)
		//IL_092c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0944: Unknown result type (might be due to invalid IL or missing references)
		//IL_0949: Unknown result type (might be due to invalid IL or missing references)
		//IL_0961: Unknown result type (might be due to invalid IL or missing references)
		//IL_0966: Unknown result type (might be due to invalid IL or missing references)
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0983: Unknown result type (might be due to invalid IL or missing references)
		//IL_099b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09da: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ada: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0caa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0caf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0daf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1013: Unknown result type (might be due to invalid IL or missing references)
		//IL_1018: Unknown result type (might be due to invalid IL or missing references)
		//IL_1030: Unknown result type (might be due to invalid IL or missing references)
		//IL_1035: Unknown result type (might be due to invalid IL or missing references)
		//IL_104d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1052: Unknown result type (might be due to invalid IL or missing references)
		//IL_106a: Unknown result type (might be due to invalid IL or missing references)
		//IL_106f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1093: Unknown result type (might be due to invalid IL or missing references)
		//IL_1098: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1113: Unknown result type (might be due to invalid IL or missing references)
		//IL_1118: Unknown result type (might be due to invalid IL or missing references)
		//IL_1130: Unknown result type (might be due to invalid IL or missing references)
		//IL_1135: Unknown result type (might be due to invalid IL or missing references)
		//IL_114d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1152: Unknown result type (might be due to invalid IL or missing references)
		//IL_116a: Unknown result type (might be due to invalid IL or missing references)
		//IL_116f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1187: Unknown result type (might be due to invalid IL or missing references)
		//IL_118c: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_11de: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1200: Unknown result type (might be due to invalid IL or missing references)
		//IL_120a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1210: Unknown result type (might be due to invalid IL or missing references)
		//IL_1215: Unknown result type (might be due to invalid IL or missing references)
		//IL_1217: Unknown result type (might be due to invalid IL or missing references)
		//IL_1218: Unknown result type (might be due to invalid IL or missing references)
		//IL_1219: Unknown result type (might be due to invalid IL or missing references)
		//IL_121e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1223: Unknown result type (might be due to invalid IL or missing references)
		//IL_1228: Unknown result type (might be due to invalid IL or missing references)
		//IL_122d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1232: Unknown result type (might be due to invalid IL or missing references)
		//IL_1234: Unknown result type (might be due to invalid IL or missing references)
		//IL_1239: Unknown result type (might be due to invalid IL or missing references)
		//IL_123e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1243: Unknown result type (might be due to invalid IL or missing references)
		//IL_1245: Unknown result type (might be due to invalid IL or missing references)
		//IL_124a: Unknown result type (might be due to invalid IL or missing references)
		//IL_124d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1252: Unknown result type (might be due to invalid IL or missing references)
		//IL_1254: Unknown result type (might be due to invalid IL or missing references)
		//IL_1259: Unknown result type (might be due to invalid IL or missing references)
		//IL_125d: Unknown result type (might be due to invalid IL or missing references)
		//IL_125f: Unknown result type (might be due to invalid IL or missing references)
		//IL_126b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1278: Unknown result type (might be due to invalid IL or missing references)
		//IL_1280: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)m_ToolSystem.activeInfoview == (Object)null) && !((EntityQuery)(ref m_ObjectQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle dependencies;
			LocalEffectSystem.ReadData readData = m_LocalEffectSystem.GetReadData(out dependencies);
			JobHandle val = default(JobHandle);
			NativeList<ArchetypeChunk> infomodeChunks = ((EntityQuery)(ref m_InfomodeQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
			FireConfigurationPrefab prefab = m_PrefabSystem.GetPrefab<FireConfigurationPrefab>(((EntityQuery)(ref m_FireConfigQuery)).GetSingletonEntity());
			m_FireHazardData.Update((SystemBase)(object)this, readData, prefab, m_ClimateSystem.temperature, m_FireHazardSystem.noRainDays);
			JobHandle dependencies2;
			UpdateObjectColorsJob updateObjectColorsJob = new UpdateObjectColorsJob
			{
				m_InfomodeChunks = infomodeChunks,
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InfomodeActiveType = InternalCompilerInterface.GetComponentTypeHandle<InfomodeActive>(ref __TypeHandle.__Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InfoviewBuildingType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewBuildingData>(ref __TypeHandle.__Game_Prefabs_InfoviewBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InfoviewBuildingStatusType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewBuildingStatusData>(ref __TypeHandle.__Game_Prefabs_InfoviewBuildingStatusData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InfoviewTransportStopType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewTransportStopData>(ref __TypeHandle.__Game_Prefabs_InfoviewTransportStopData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InfoviewVehicleType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewVehicleData>(ref __TypeHandle.__Game_Prefabs_InfoviewVehicleData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InfoviewObjectStatusType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewObjectStatusData>(ref __TypeHandle.__Game_Prefabs_InfoviewObjectStatusData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InfoviewNetStatusType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewNetStatusData>(ref __TypeHandle.__Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HospitalType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Hospital>(ref __TypeHandle.__Game_Buildings_Hospital_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityProducerType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityProducer>(ref __TypeHandle.__Game_Buildings_ElectricityProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Transformer>(ref __TypeHandle.__Game_Buildings_Transformer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_WaterPumpingStationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.WaterPumpingStation>(ref __TypeHandle.__Game_Buildings_WaterPumpingStation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_WaterTowerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.WaterTower>(ref __TypeHandle.__Game_Buildings_WaterTower_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SewageOutletType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.SewageOutlet>(ref __TypeHandle.__Game_Buildings_SewageOutlet_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_WastewaterTreatmentPlantType = InternalCompilerInterface.GetComponentTypeHandle<WastewaterTreatmentPlant>(ref __TypeHandle.__Game_Buildings_WastewaterTreatmentPlant_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransportDepotType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.TransportDepot>(ref __TypeHandle.__Game_Buildings_TransportDepot_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransportStationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.TransportStation>(ref __TypeHandle.__Game_Buildings_TransportStation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_GarbageFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.GarbageFacility>(ref __TypeHandle.__Game_Buildings_GarbageFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_FireStationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.FireStation>(ref __TypeHandle.__Game_Buildings_FireStation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PoliceStationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.PoliceStation>(ref __TypeHandle.__Game_Buildings_PoliceStation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CrimeProducerType = InternalCompilerInterface.GetComponentTypeHandle<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RoadMaintenanceType = InternalCompilerInterface.GetComponentTypeHandle<RoadMaintenance>(ref __TypeHandle.__Game_Buildings_RoadMaintenance_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ParkMaintenanceType = InternalCompilerInterface.GetComponentTypeHandle<ParkMaintenance>(ref __TypeHandle.__Game_Buildings_ParkMaintenance_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PostFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.PostFacility>(ref __TypeHandle.__Game_Buildings_PostFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TelecomFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.TelecomFacility>(ref __TypeHandle.__Game_Buildings_TelecomFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SchoolType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.School>(ref __TypeHandle.__Game_Buildings_School_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ParkType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AttractivenessProviderType = InternalCompilerInterface.GetComponentTypeHandle<AttractivenessProvider>(ref __TypeHandle.__Game_Buildings_AttractivenessProvider_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EmergencyShelterType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.EmergencyShelter>(ref __TypeHandle.__Game_Buildings_EmergencyShelter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DisasterFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.DisasterFacility>(ref __TypeHandle.__Game_Buildings_DisasterFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_FirewatchTowerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.FirewatchTower>(ref __TypeHandle.__Game_Buildings_FirewatchTower_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeathcareFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.DeathcareFacility>(ref __TypeHandle.__Game_Buildings_DeathcareFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrisonType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Prison>(ref __TypeHandle.__Game_Buildings_Prison_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AdminBuildingType = InternalCompilerInterface.GetComponentTypeHandle<AdminBuilding>(ref __TypeHandle.__Game_Buildings_AdminBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_WelfareOfficeType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.WelfareOffice>(ref __TypeHandle.__Game_Buildings_WelfareOffice_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ResearchFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.ResearchFacility>(ref __TypeHandle.__Game_Buildings_ResearchFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ParkingFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.ParkingFacility>(ref __TypeHandle.__Game_Buildings_ParkingFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BatteryType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Battery>(ref __TypeHandle.__Game_Buildings_Battery_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MailProducerType = InternalCompilerInterface.GetComponentTypeHandle<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingConditionType = InternalCompilerInterface.GetComponentTypeHandle<BuildingCondition>(ref __TypeHandle.__Game_Buildings_BuildingCondition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ResidentialPropertyType = InternalCompilerInterface.GetComponentTypeHandle<ResidentialProperty>(ref __TypeHandle.__Game_Buildings_ResidentialProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CommercialPropertyType = InternalCompilerInterface.GetComponentTypeHandle<CommercialProperty>(ref __TypeHandle.__Game_Buildings_CommercialProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_IndustrialPropertyType = InternalCompilerInterface.GetComponentTypeHandle<IndustrialProperty>(ref __TypeHandle.__Game_Buildings_IndustrialProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OfficePropertyType = InternalCompilerInterface.GetComponentTypeHandle<OfficeProperty>(ref __TypeHandle.__Game_Buildings_OfficeProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_StoragePropertyType = InternalCompilerInterface.GetComponentTypeHandle<StorageProperty>(ref __TypeHandle.__Game_Buildings_StorageProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ExtractorPropertyType = InternalCompilerInterface.GetComponentTypeHandle<ExtractorProperty>(ref __TypeHandle.__Game_Buildings_ExtractorProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_GarbageProducerType = InternalCompilerInterface.GetComponentTypeHandle<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AbandonedType = InternalCompilerInterface.GetComponentTypeHandle<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LeisureProviderType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.LeisureProvider>(ref __TypeHandle.__Game_Buildings_LeisureProvider_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityConsumerType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_WaterConsumerType = InternalCompilerInterface.GetComponentTypeHandle<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingEfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RenterType = InternalCompilerInterface.GetBufferTypeHandle<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransportStopType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.TransportStop>(ref __TypeHandle.__Game_Routes_TransportStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BusStopType = InternalCompilerInterface.GetComponentTypeHandle<BusStop>(ref __TypeHandle.__Game_Routes_BusStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TrainStopType = InternalCompilerInterface.GetComponentTypeHandle<TrainStop>(ref __TypeHandle.__Game_Routes_TrainStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TaxiStandType = InternalCompilerInterface.GetComponentTypeHandle<TaxiStand>(ref __TypeHandle.__Game_Routes_TaxiStand_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TramStopType = InternalCompilerInterface.GetComponentTypeHandle<TramStop>(ref __TypeHandle.__Game_Routes_TramStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ShipStopType = InternalCompilerInterface.GetComponentTypeHandle<ShipStop>(ref __TypeHandle.__Game_Routes_ShipStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MailBoxType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.MailBox>(ref __TypeHandle.__Game_Routes_MailBox_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_WorkStopType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.WorkStop>(ref __TypeHandle.__Game_Routes_WorkStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AirplaneStopType = InternalCompilerInterface.GetComponentTypeHandle<AirplaneStop>(ref __TypeHandle.__Game_Routes_AirplaneStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SubwayStopType = InternalCompilerInterface.GetComponentTypeHandle<SubwayStop>(ref __TypeHandle.__Game_Routes_SubwayStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_VehicleType = InternalCompilerInterface.GetComponentTypeHandle<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PassengerTransportType = InternalCompilerInterface.GetComponentTypeHandle<PassengerTransport>(ref __TypeHandle.__Game_Vehicles_PassengerTransport_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CargoTransportType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.CargoTransport>(ref __TypeHandle.__Game_Vehicles_CargoTransport_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TaxiType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.Taxi>(ref __TypeHandle.__Game_Vehicles_Taxi_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ParkMaintenanceVehicleType = InternalCompilerInterface.GetComponentTypeHandle<ParkMaintenanceVehicle>(ref __TypeHandle.__Game_Vehicles_ParkMaintenanceVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RoadMaintenanceVehicleType = InternalCompilerInterface.GetComponentTypeHandle<RoadMaintenanceVehicle>(ref __TypeHandle.__Game_Vehicles_RoadMaintenanceVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AmbulanceType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.Ambulance>(ref __TypeHandle.__Game_Vehicles_Ambulance_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EvacuatingTransportType = InternalCompilerInterface.GetComponentTypeHandle<EvacuatingTransport>(ref __TypeHandle.__Game_Vehicles_EvacuatingTransport_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_FireEngineType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.FireEngine>(ref __TypeHandle.__Game_Vehicles_FireEngine_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_GarbageTruckType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.GarbageTruck>(ref __TypeHandle.__Game_Vehicles_GarbageTruck_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HearseType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.Hearse>(ref __TypeHandle.__Game_Vehicles_Hearse_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PoliceCarType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.PoliceCar>(ref __TypeHandle.__Game_Vehicles_PoliceCar_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PostVanType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.PostVan>(ref __TypeHandle.__Game_Vehicles_PostVan_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrisonerTransportType = InternalCompilerInterface.GetComponentTypeHandle<PrisonerTransport>(ref __TypeHandle.__Game_Vehicles_PrisonerTransport_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PassengerType = InternalCompilerInterface.GetBufferTypeHandle<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ResidentType = InternalCompilerInterface.GetComponentTypeHandle<Game.Creatures.Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TreeType = InternalCompilerInterface.GetComponentTypeHandle<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PlantType = InternalCompilerInterface.GetComponentTypeHandle<Plant>(ref __TypeHandle.__Game_Objects_Plant_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UtilityObjectType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.UtilityObject>(ref __TypeHandle.__Game_Objects_UtilityObject_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DestroyedType = InternalCompilerInterface.GetComponentTypeHandle<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DamagedType = InternalCompilerInterface.GetComponentTypeHandle<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UnderConstructionType = InternalCompilerInterface.GetComponentTypeHandle<UnderConstruction>(ref __TypeHandle.__Game_Objects_UnderConstruction_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UniqueObjectType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.UniqueObject>(ref __TypeHandle.__Game_Objects_UniqueObject_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceholderType = InternalCompilerInterface.GetComponentTypeHandle<Placeholder>(ref __TypeHandle.__Game_Objects_Placeholder_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentDistrictType = InternalCompilerInterface.GetComponentTypeHandle<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AttachedType = InternalCompilerInterface.GetComponentTypeHandle<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AccidentSiteType = InternalCompilerInterface.GetComponentTypeHandle<AccidentSite>(ref __TypeHandle.__Game_Events_AccidentSite_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TreeData = InternalCompilerInterface.GetComponentLookup<TreeData>(ref __TypeHandle.__Game_Prefabs_TreeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ParkData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AbandonedData = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingPropertyData = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PollutionData = InternalCompilerInterface.GetComponentLookup<PollutionData>(ref __TypeHandle.__Game_Prefabs_PollutionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PollutionModifierData = InternalCompilerInterface.GetComponentLookup<PollutionModifierData>(ref __TypeHandle.__Game_Prefabs_PollutionModifierData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CommercialCompanies = InternalCompilerInterface.GetComponentLookup<CommercialCompany>(ref __TypeHandle.__Game_Companies_CommercialCompany_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnableBuildingDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceholderBuildingData = InternalCompilerInterface.GetComponentLookup<PlaceholderBuildingData>(ref __TypeHandle.__Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SewageOutletData = InternalCompilerInterface.GetComponentLookup<SewageOutletData>(ref __TypeHandle.__Game_Prefabs_SewageOutletData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResidentData = InternalCompilerInterface.GetComponentLookup<Game.Creatures.Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Profitabilities = InternalCompilerInterface.GetComponentLookup<Profitability>(ref __TypeHandle.__Game_Companies_Profitability_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LeisureProviders = InternalCompilerInterface.GetComponentLookup<Game.Buildings.LeisureProvider>(ref __TypeHandle.__Game_Buildings_LeisureProvider_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IndustrialProcessData = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Employees = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceBuffs = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ZoneDatas = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabUtilityObjectData = InternalCompilerInterface.GetComponentLookup<UtilityObjectData>(ref __TypeHandle.__Game_Prefabs_UtilityObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityConsumerData = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WaterConsumerData = InternalCompilerInterface.GetComponentLookup<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityNodeConnectionData = InternalCompilerInterface.GetComponentLookup<ElectricityNodeConnection>(ref __TypeHandle.__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WaterPipeNodeConnectionData = InternalCompilerInterface.GetComponentLookup<WaterPipeNodeConnection>(ref __TypeHandle.__Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityFlowEdgeData = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WaterPipeEdgeData = InternalCompilerInterface.GetComponentLookup<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Net.ResourceConnection>(ref __TypeHandle.__Game_Net_ResourceConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedFlowEdges = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FireHazardData = m_FireHazardData,
				m_TelecomCoverageData = m_TelecomCoverageSystem.GetData(readOnly: true, out dependencies2),
				m_PollutionParameters = ((EntityQuery)(ref m_PollutionParameterQuery)).GetSingleton<PollutionParameterData>(),
				m_City = m_CitySystem.City,
				m_ColorType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.Color>(ref __TypeHandle.__Game_Objects_Color_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
			};
			UpdateMiddleObjectColorsJob updateMiddleObjectColorsJob = new UpdateMiddleObjectColorsJob
			{
				m_InfomodeChunks = infomodeChunks,
				m_InfomodeActiveType = InternalCompilerInterface.GetComponentTypeHandle<InfomodeActive>(ref __TypeHandle.__Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InfoviewObjectStatusType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewObjectStatusData>(ref __TypeHandle.__Game_Prefabs_InfoviewObjectStatusData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ControllerType = InternalCompilerInterface.GetComponentTypeHandle<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ColorData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Color>(ref __TypeHandle.__Game_Objects_Color_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			UpdateTempObjectColorsJob updateTempObjectColorsJob = new UpdateTempObjectColorsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ColorData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Color>(ref __TypeHandle.__Game_Objects_Color_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			UpdateSubObjectColorsJob obj = new UpdateSubObjectColorsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TreeType = InternalCompilerInterface.GetComponentTypeHandle<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PlantType = InternalCompilerInterface.GetComponentTypeHandle<Plant>(ref __TypeHandle.__Game_Objects_Plant_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ColorData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Color>(ref __TypeHandle.__Game_Objects_Color_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateObjectColorsJob>(updateObjectColorsJob, m_ObjectQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, JobHandle.CombineDependencies(dependencies2, dependencies, val)));
			JobHandle val3 = JobChunkExtensions.ScheduleParallel<UpdateMiddleObjectColorsJob>(updateMiddleObjectColorsJob, m_MiddleObjectQuery, val2);
			JobHandle val4 = JobChunkExtensions.ScheduleParallel<UpdateTempObjectColorsJob>(updateTempObjectColorsJob, m_TempObjectQuery, val3);
			JobHandle dependency = JobChunkExtensions.ScheduleParallel<UpdateSubObjectColorsJob>(obj, m_SubObjectQuery, val4);
			infomodeChunks.Dispose(val3);
			m_LocalEffectSystem.AddLocalEffectReader(val2);
			m_TelecomCoverageSystem.AddReader(val2);
			((SystemBase)this).Dependency = dependency;
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
	public ObjectColorSystem()
	{
	}
}

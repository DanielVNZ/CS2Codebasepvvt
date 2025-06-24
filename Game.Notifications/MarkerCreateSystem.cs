using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Common;
using Game.Creatures;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Serialization;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Notifications;

[CompilerGenerated]
public class MarkerCreateSystem : GameSystemBase, IPostDeserialize
{
	[BurstCompile]
	private struct MarkerCreateJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Hidden> m_HiddenType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewTransportStopData> m_InfoviewTransportStopType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewBuildingData> m_InfoviewBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewBuildingStatusData> m_InfoviewBuildingStatusType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewVehicleData> m_InfoviewVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewMarkerData> m_InfoviewMarkerType;

		[ReadOnly]
		public ComponentTypeHandle<InfomodeActive> m_InfomodeActiveType;

		[ReadOnly]
		public ComponentTypeHandle<TransportStopMarkerData> m_TransportStopMarkerType;

		[ReadOnly]
		public ComponentTypeHandle<BuildingMarkerData> m_BuildingMarkerType;

		[ReadOnly]
		public ComponentTypeHandle<VehicleMarkerData> m_VehicleMarkerType;

		[ReadOnly]
		public ComponentTypeHandle<MarkerMarkerData> m_MarkerMarkerType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.TransportStop> m_TransportStopType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<Vehicle> m_VehicleType;

		[ReadOnly]
		public ComponentTypeHandle<Marker> m_MarkerType;

		[ReadOnly]
		public ComponentTypeHandle<Controller> m_ControllerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.UniqueObject> m_UniqueObjectType;

		[ReadOnly]
		public ComponentTypeHandle<ParkedCar> m_ParkedCarType;

		[ReadOnly]
		public ComponentTypeHandle<ParkedTrain> m_ParkedTrainType;

		[ReadOnly]
		public BufferTypeHandle<IconElement> m_IconElementType;

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
		public ComponentTypeHandle<Game.Buildings.Hospital> m_HospitalType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityProducer> m_ElectricityProducerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Transformer> m_TransformerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Battery> m_BatteryType;

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
		public ComponentTypeHandle<Game.Buildings.EmergencyShelter> m_EmergencyShelterType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.DisasterFacility> m_DisasterFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.FirewatchTower> m_FirewatchTowerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Park> m_ParkType;

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
		public ComponentTypeHandle<ResidentialProperty> m_ResidentialPropertyType;

		[ReadOnly]
		public ComponentTypeHandle<CommercialProperty> m_CommercialPropertyType;

		[ReadOnly]
		public ComponentTypeHandle<IndustrialProperty> m_IndustrialPropertyType;

		[ReadOnly]
		public ComponentTypeHandle<OfficeProperty> m_OfficePropertyType;

		[ReadOnly]
		public ComponentTypeHandle<ExtractorProperty> m_ExtractorPropertyType;

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
		public ComponentTypeHandle<Game.Creatures.CreatureSpawner> m_CreatureSpawnerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.ElectricityOutsideConnection> m_ElectricityOutsideConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.WaterPipeOutsideConnection> m_WaterPipeOutsideConnectionType;

		[ReadOnly]
		public ComponentLookup<Icon> m_IconData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<TransportStopData> m_TransportStopData;

		[ReadOnly]
		public ComponentLookup<WorkStopData> m_WorkStopData;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_InfomodeChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_IconChunks;

		[ReadOnly]
		public TransportType m_RequiredTransportStopType;

		[ReadOnly]
		public MarkerType m_RequiredMarkerType;

		[ReadOnly]
		public bool m_RequireStandaloneStops;

		public IconCommandBuffer m_IconCommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			Entity val2 = Entity.Null;
			bool disallowCluster = false;
			bool flag = false;
			bool flag2 = false;
			VehicleType vehicleType;
			Entity markerPrefab3;
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Routes.TransportStop>(ref m_TransportStopType))
			{
				if (GetTransportStopType(chunk, out var transportType))
				{
					Entity markerPrefabA;
					Entity markerPrefabB;
					if (((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType))
					{
						if (GetMarkerType(transportType, out var markerType) && FindMarkerPrefab(markerType, out var markerPrefab))
						{
							val = markerPrefab;
						}
					}
					else if (FindMarkerPrefab(transportType, out markerPrefabA, out markerPrefabB))
					{
						val = markerPrefabA;
						val2 = markerPrefabB;
						flag2 = true;
					}
				}
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Building>(ref m_BuildingType))
			{
				if (GetBuildingType(chunk, out var buildingType) && FindMarkerPrefab(buildingType, out var markerPrefab2))
				{
					val = markerPrefab2;
				}
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Vehicle>(ref m_VehicleType) && GetVehicleType(chunk, out vehicleType) && FindMarkerPrefab(vehicleType, out markerPrefab3))
			{
				val = markerPrefab3;
				disallowCluster = true;
				flag = true;
			}
			if (val == Entity.Null && val2 == Entity.Null && ((ArchetypeChunk)(ref chunk)).Has<Marker>(ref m_MarkerType) && GetMarkerType(chunk, out var markerType2) && FindMarkerPrefab(markerType2, out var markerPrefab4))
			{
				val = markerPrefab4;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<IconElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<IconElement>(ref m_IconElementType);
			if (val != Entity.Null || (flag2 && val2 != Entity.Null))
			{
				NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
				bool isHidden = ((ArchetypeChunk)(ref chunk)).Has<Hidden>(ref m_HiddenType);
				bool flag3 = nativeArray2.Length != 0;
				if (flag)
				{
					NativeArray<Controller> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Controller>(ref m_ControllerType);
					if (nativeArray3.Length != 0)
					{
						Controller controller = default(Controller);
						for (int i = 0; i < nativeArray.Length; i++)
						{
							Entity val3 = nativeArray[i];
							if (flag3)
							{
								Entity original = nativeArray2[i].m_Original;
								if (m_ControllerData.TryGetComponent(original, ref controller))
								{
									if (controller.m_Controller != original)
									{
										continue;
									}
								}
								else if (nativeArray3[i].m_Controller != val3)
								{
									continue;
								}
							}
							else if (nativeArray3[i].m_Controller != val3)
							{
								continue;
							}
							m_IconCommandBuffer.Add(val3, val, IconPriority.Info, IconClusterLayer.Marker, IconFlags.Unique, Entity.Null, flag3, isHidden, disallowCluster);
						}
					}
					else
					{
						flag = false;
					}
				}
				if (flag2)
				{
					NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
					WorkStopData workStopData = default(WorkStopData);
					TransportStopData transportStopData = default(TransportStopData);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Entity owner = nativeArray[j];
						PrefabRef prefabRef = nativeArray4[j];
						Entity prefab;
						if (m_WorkStopData.TryGetComponent(prefabRef.m_Prefab, ref workStopData))
						{
							if (!workStopData.m_WorkLocation && val != Entity.Null)
							{
								prefab = val;
							}
							else
							{
								if (!workStopData.m_WorkLocation || !(val2 != Entity.Null))
								{
									continue;
								}
								prefab = val2;
							}
						}
						else
						{
							if (!m_TransportStopData.TryGetComponent(prefabRef.m_Prefab, ref transportStopData))
							{
								continue;
							}
							if (transportStopData.m_PassengerTransport && val != Entity.Null)
							{
								prefab = val;
							}
							else
							{
								if (!transportStopData.m_CargoTransport || !(val2 != Entity.Null))
								{
									continue;
								}
								prefab = val2;
							}
						}
						m_IconCommandBuffer.Add(owner, prefab, IconPriority.Info, IconClusterLayer.Marker, IconFlags.Unique, Entity.Null, flag3, isHidden, disallowCluster);
					}
				}
				if (!flag && !flag2)
				{
					for (int k = 0; k < nativeArray.Length; k++)
					{
						Entity owner2 = nativeArray[k];
						m_IconCommandBuffer.Add(owner2, val, IconPriority.Info, IconClusterLayer.Marker, IconFlags.Unique, Entity.Null, flag3, isHidden, disallowCluster);
					}
				}
			}
			for (int l = 0; l < bufferAccessor.Length; l++)
			{
				Entity owner3 = nativeArray[l];
				DynamicBuffer<IconElement> val4 = bufferAccessor[l];
				for (int m = 0; m < val4.Length; m++)
				{
					IconElement iconElement = val4[m];
					if (m_IconData[iconElement.m_Icon].m_ClusterLayer == IconClusterLayer.Marker)
					{
						PrefabRef prefabRef2 = m_PrefabRefData[iconElement.m_Icon];
						if (prefabRef2.m_Prefab != val && prefabRef2.m_Prefab != val2)
						{
							m_IconCommandBuffer.Remove(owner3, prefabRef2.m_Prefab);
						}
					}
				}
			}
		}

		private bool GetTransportStopType(ArchetypeChunk chunk, out TransportType transportType)
		{
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			transportType = TransportType.None;
			int num = int.MaxValue;
			if (m_InfomodeChunks.IsCreated)
			{
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
						int priority = nativeArray2[j].m_Priority;
						if (priority < num)
						{
							TransportType type = nativeArray[j].m_Type;
							if (IsTransportStopType(chunk, type))
							{
								transportType = type;
								num = priority;
							}
						}
					}
				}
			}
			if (transportType == TransportType.None && IsTransportStopType(chunk, m_RequiredTransportStopType))
			{
				transportType = m_RequiredTransportStopType;
			}
			if (transportType == TransportType.None && m_RequireStandaloneStops && !((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType) && !((ArchetypeChunk)(ref chunk)).Has<Owner>(ref m_OwnerType))
			{
				if (((ArchetypeChunk)(ref chunk)).Has<BusStop>(ref m_BusStopType))
				{
					transportType = TransportType.Bus;
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<TaxiStand>(ref m_TaxiStandType))
				{
					transportType = TransportType.Taxi;
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<TramStop>(ref m_TramStopType))
				{
					transportType = TransportType.Tram;
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<Game.Routes.MailBox>(ref m_MailBoxType))
				{
					transportType = TransportType.Post;
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<Game.Routes.WorkStop>(ref m_WorkStopType))
				{
					transportType = TransportType.Work;
				}
			}
			return transportType != TransportType.None;
		}

		private bool IsTransportStopType(ArchetypeChunk chunk, TransportType transportType)
		{
			return transportType switch
			{
				TransportType.Bus => ((ArchetypeChunk)(ref chunk)).Has<BusStop>(ref m_BusStopType), 
				TransportType.Train => ((ArchetypeChunk)(ref chunk)).Has<TrainStop>(ref m_TrainStopType), 
				TransportType.Taxi => ((ArchetypeChunk)(ref chunk)).Has<TaxiStand>(ref m_TaxiStandType), 
				TransportType.Tram => ((ArchetypeChunk)(ref chunk)).Has<TramStop>(ref m_TramStopType), 
				TransportType.Ship => ((ArchetypeChunk)(ref chunk)).Has<ShipStop>(ref m_ShipStopType), 
				TransportType.Post => ((ArchetypeChunk)(ref chunk)).Has<Game.Routes.MailBox>(ref m_MailBoxType), 
				TransportType.Work => ((ArchetypeChunk)(ref chunk)).Has<Game.Routes.WorkStop>(ref m_WorkStopType), 
				TransportType.Airplane => ((ArchetypeChunk)(ref chunk)).Has<AirplaneStop>(ref m_AirplaneStopType), 
				TransportType.Subway => ((ArchetypeChunk)(ref chunk)).Has<SubwayStop>(ref m_SubwayStopType), 
				_ => false, 
			};
		}

		private bool FindMarkerPrefab(TransportType transportType, out Entity markerPrefabA, out Entity markerPrefabB)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			markerPrefabA = Entity.Null;
			markerPrefabB = Entity.Null;
			for (int i = 0; i < m_IconChunks.Length; i++)
			{
				ArchetypeChunk val = m_IconChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<TransportStopMarkerData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<TransportStopMarkerData>(ref m_TransportStopMarkerType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					TransportStopMarkerData transportStopMarkerData = nativeArray2[j];
					if (transportStopMarkerData.m_TransportType == transportType)
					{
						if (transportStopMarkerData.m_StopTypeA)
						{
							markerPrefabA = nativeArray[j];
							result = true;
						}
						else if (transportStopMarkerData.m_StopTypeB)
						{
							markerPrefabB = nativeArray[j];
							result = true;
						}
					}
				}
			}
			return result;
		}

		private bool GetBuildingType(ArchetypeChunk chunk, out BuildingType buildingType)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			buildingType = BuildingType.None;
			int num = int.MaxValue;
			if (m_InfomodeChunks.IsCreated)
			{
				for (int i = 0; i < m_InfomodeChunks.Length; i++)
				{
					ArchetypeChunk val = m_InfomodeChunks[i];
					NativeArray<InfoviewBuildingData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewBuildingData>(ref m_InfoviewBuildingType);
					NativeArray<InfoviewBuildingStatusData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewBuildingStatusData>(ref m_InfoviewBuildingStatusType);
					if (nativeArray.Length == 0 && nativeArray2.Length == 0)
					{
						continue;
					}
					NativeArray<InfomodeActive> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						int priority = nativeArray3[j].m_Priority;
						if (priority < num)
						{
							BuildingType type = nativeArray[j].m_Type;
							if (IsBuildingType(chunk, type))
							{
								buildingType = type;
								num = priority;
							}
						}
					}
					for (int k = 0; k < nativeArray2.Length; k++)
					{
						int priority2 = nativeArray3[k].m_Priority;
						if (priority2 < num)
						{
							BuildingType buildingType2 = GetBuildingType(nativeArray2[k].m_Type);
							if (IsBuildingType(chunk, buildingType2))
							{
								buildingType = buildingType2;
								num = priority2;
							}
						}
					}
				}
			}
			return buildingType != BuildingType.None;
		}

		private BuildingType GetBuildingType(BuildingStatusType buildingStatusType)
		{
			return buildingStatusType switch
			{
				BuildingStatusType.SignatureResidential => BuildingType.SignatureResidential, 
				BuildingStatusType.SignatureCommercial => BuildingType.SignatureCommercial, 
				BuildingStatusType.SignatureIndustrial => BuildingType.SignatureIndustrial, 
				BuildingStatusType.SignatureOffice => BuildingType.SignatureOffice, 
				_ => BuildingType.None, 
			};
		}

		private bool IsBuildingType(ArchetypeChunk chunk, BuildingType buildingType)
		{
			switch (buildingType)
			{
			case BuildingType.Hospital:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Hospital>(ref m_HospitalType);
			case BuildingType.PowerPlant:
				return ((ArchetypeChunk)(ref chunk)).Has<ElectricityProducer>(ref m_ElectricityProducerType);
			case BuildingType.Transformer:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Transformer>(ref m_TransformerType);
			case BuildingType.Battery:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Battery>(ref m_BatteryType);
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
			case BuildingType.ParkMaintenanceDepot:
				return ((ArchetypeChunk)(ref chunk)).Has<ParkMaintenance>(ref m_ParkMaintenanceType);
			case BuildingType.PostFacility:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.PostFacility>(ref m_PostFacilityType);
			case BuildingType.TelecomFacility:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.TelecomFacility>(ref m_TelecomFacilityType);
			case BuildingType.School:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.School>(ref m_SchoolType);
			case BuildingType.EmergencyShelter:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.EmergencyShelter>(ref m_EmergencyShelterType);
			case BuildingType.DisasterFacility:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.DisasterFacility>(ref m_DisasterFacilityType);
			case BuildingType.FirewatchTower:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.FirewatchTower>(ref m_FirewatchTowerType);
			case BuildingType.Park:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Park>(ref m_ParkType);
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
			case BuildingType.ParkingFacility:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.ParkingFacility>(ref m_ParkingFacilityType);
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
			default:
				return false;
			}
		}

		private bool FindMarkerPrefab(BuildingType buildingType, out Entity markerPrefab)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_IconChunks.Length; i++)
			{
				ArchetypeChunk val = m_IconChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<BuildingMarkerData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<BuildingMarkerData>(ref m_BuildingMarkerType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					if (nativeArray2[j].m_BuildingType == buildingType)
					{
						markerPrefab = nativeArray[j];
						return true;
					}
				}
			}
			markerPrefab = Entity.Null;
			return false;
		}

		private bool GetVehicleType(ArchetypeChunk chunk, out VehicleType vehicleType)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			vehicleType = VehicleType.None;
			int num = int.MaxValue;
			if (m_InfomodeChunks.IsCreated && !((ArchetypeChunk)(ref chunk)).Has<ParkedCar>(ref m_ParkedCarType) && !((ArchetypeChunk)(ref chunk)).Has<ParkedTrain>(ref m_ParkedTrainType))
			{
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
						int priority = nativeArray2[j].m_Priority;
						if (priority < num)
						{
							VehicleType type = nativeArray[j].m_Type;
							if (IsVehicleType(chunk, type))
							{
								vehicleType = type;
								num = priority;
							}
						}
					}
				}
			}
			return vehicleType != VehicleType.None;
		}

		private bool IsVehicleType(ArchetypeChunk chunk, VehicleType vehicleType)
		{
			return vehicleType switch
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

		private bool FindMarkerPrefab(VehicleType vehicleType, out Entity markerPrefab)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_IconChunks.Length; i++)
			{
				ArchetypeChunk val = m_IconChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<VehicleMarkerData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<VehicleMarkerData>(ref m_VehicleMarkerType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					if (nativeArray2[j].m_VehicleType == vehicleType)
					{
						markerPrefab = nativeArray[j];
						return true;
					}
				}
			}
			markerPrefab = Entity.Null;
			return false;
		}

		private bool GetMarkerType(ArchetypeChunk chunk, out MarkerType markerType)
		{
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			markerType = MarkerType.None;
			int num = int.MaxValue;
			if (m_InfomodeChunks.IsCreated)
			{
				for (int i = 0; i < m_InfomodeChunks.Length; i++)
				{
					ArchetypeChunk val = m_InfomodeChunks[i];
					NativeArray<InfoviewMarkerData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewMarkerData>(ref m_InfoviewMarkerType);
					if (nativeArray.Length == 0)
					{
						continue;
					}
					NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						int priority = nativeArray2[j].m_Priority;
						if (priority < num)
						{
							MarkerType type = nativeArray[j].m_Type;
							if (IsMarkerType(chunk, type))
							{
								markerType = type;
								num = priority;
							}
						}
					}
				}
			}
			if (markerType == MarkerType.None && IsMarkerType(chunk, m_RequiredMarkerType))
			{
				markerType = m_RequiredMarkerType;
			}
			return markerType != MarkerType.None;
		}

		private bool GetMarkerType(TransportType transportType, out MarkerType markerType)
		{
			switch (transportType)
			{
			case TransportType.Bus:
				markerType = MarkerType.RoadOutsideConnection;
				return true;
			case TransportType.Train:
				markerType = MarkerType.TrainOutsideConnection;
				return true;
			case TransportType.Ship:
				markerType = MarkerType.ShipOutsideConnection;
				return true;
			case TransportType.Airplane:
				markerType = MarkerType.AirplaneOutsideConnection;
				return true;
			default:
				markerType = MarkerType.None;
				return false;
			}
		}

		private bool IsMarkerType(ArchetypeChunk chunk, MarkerType markerType)
		{
			switch (markerType)
			{
			case MarkerType.CreatureSpawner:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Creatures.CreatureSpawner>(ref m_CreatureSpawnerType);
			case MarkerType.RoadOutsideConnection:
				if (((ArchetypeChunk)(ref chunk)).Has<BusStop>(ref m_BusStopType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType);
				}
				return false;
			case MarkerType.TrainOutsideConnection:
				if (((ArchetypeChunk)(ref chunk)).Has<TrainStop>(ref m_TrainStopType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType);
				}
				return false;
			case MarkerType.ShipOutsideConnection:
				if (((ArchetypeChunk)(ref chunk)).Has<ShipStop>(ref m_ShipStopType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType);
				}
				return false;
			case MarkerType.AirplaneOutsideConnection:
				if (((ArchetypeChunk)(ref chunk)).Has<AirplaneStop>(ref m_AirplaneStopType))
				{
					return ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType);
				}
				return false;
			case MarkerType.ElectricityOutsideConnection:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.ElectricityOutsideConnection>(ref m_ElectricityOutsideConnectionType);
			case MarkerType.WaterPipeOutsideConnection:
				return ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.WaterPipeOutsideConnection>(ref m_WaterPipeOutsideConnectionType);
			default:
				return false;
			}
		}

		private bool FindMarkerPrefab(MarkerType markerType, out Entity markerPrefab)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_IconChunks.Length; i++)
			{
				ArchetypeChunk val = m_IconChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<MarkerMarkerData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<MarkerMarkerData>(ref m_MarkerMarkerType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					if (nativeArray2[j].m_MarkerType == markerType)
					{
						markerPrefab = nativeArray[j];
						return true;
					}
				}
			}
			markerPrefab = Entity.Null;
			return false;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<InfoviewTransportStopData> __Game_Prefabs_InfoviewTransportStopData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewBuildingData> __Game_Prefabs_InfoviewBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewBuildingStatusData> __Game_Prefabs_InfoviewBuildingStatusData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewVehicleData> __Game_Prefabs_InfoviewVehicleData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewMarkerData> __Game_Prefabs_InfoviewMarkerData_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Hidden> __Game_Tools_Hidden_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfomodeActive> __Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TransportStopMarkerData> __Game_Prefabs_TransportStopMarkerData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BuildingMarkerData> __Game_Prefabs_BuildingMarkerData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<VehicleMarkerData> __Game_Prefabs_VehicleMarkerData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MarkerMarkerData> __Game_Prefabs_MarkerMarkerData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.TransportStop> __Game_Routes_TransportStop_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Marker> __Game_Objects_Marker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Controller> __Game_Vehicles_Controller_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.UniqueObject> __Game_Objects_UniqueObject_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<IconElement> __Game_Notifications_IconElement_RO_BufferTypeHandle;

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
		public ComponentTypeHandle<Game.Buildings.Hospital> __Game_Buildings_Hospital_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityProducer> __Game_Buildings_ElectricityProducer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Transformer> __Game_Buildings_Transformer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Battery> __Game_Buildings_Battery_RO_ComponentTypeHandle;

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
		public ComponentTypeHandle<Game.Buildings.EmergencyShelter> __Game_Buildings_EmergencyShelter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.DisasterFacility> __Game_Buildings_DisasterFacility_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.FirewatchTower> __Game_Buildings_FirewatchTower_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Park> __Game_Buildings_Park_RO_ComponentTypeHandle;

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
		public ComponentTypeHandle<ResidentialProperty> __Game_Buildings_ResidentialProperty_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CommercialProperty> __Game_Buildings_CommercialProperty_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<IndustrialProperty> __Game_Buildings_IndustrialProperty_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<OfficeProperty> __Game_Buildings_OfficeProperty_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ExtractorProperty> __Game_Buildings_ExtractorProperty_RO_ComponentTypeHandle;

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
		public ComponentTypeHandle<Game.Creatures.CreatureSpawner> __Game_Creatures_CreatureSpawner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.ElectricityOutsideConnection> __Game_Objects_ElectricityOutsideConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.WaterPipeOutsideConnection> __Game_Objects_WaterPipeOutsideConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Icon> __Game_Notifications_Icon_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportStopData> __Game_Prefabs_TransportStopData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkStopData> __Game_Prefabs_WorkStopData_RO_ComponentLookup;

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
			__Game_Prefabs_InfoviewTransportStopData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewTransportStopData>(true);
			__Game_Prefabs_InfoviewBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewBuildingData>(true);
			__Game_Prefabs_InfoviewBuildingStatusData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewBuildingStatusData>(true);
			__Game_Prefabs_InfoviewVehicleData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewVehicleData>(true);
			__Game_Prefabs_InfoviewMarkerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewMarkerData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Hidden_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Hidden>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfomodeActive>(true);
			__Game_Prefabs_TransportStopMarkerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TransportStopMarkerData>(true);
			__Game_Prefabs_BuildingMarkerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingMarkerData>(true);
			__Game_Prefabs_VehicleMarkerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<VehicleMarkerData>(true);
			__Game_Prefabs_MarkerMarkerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MarkerMarkerData>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Routes_TransportStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.TransportStop>(true);
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Vehicles_Vehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Vehicle>(true);
			__Game_Objects_Marker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Marker>(true);
			__Game_Vehicles_Controller_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Controller>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkedCar>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkedTrain>(true);
			__Game_Objects_UniqueObject_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.UniqueObject>(true);
			__Game_Notifications_IconElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<IconElement>(true);
			__Game_Routes_BusStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BusStop>(true);
			__Game_Routes_TrainStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrainStop>(true);
			__Game_Routes_TaxiStand_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TaxiStand>(true);
			__Game_Routes_TramStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TramStop>(true);
			__Game_Routes_ShipStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ShipStop>(true);
			__Game_Routes_MailBox_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.MailBox>(true);
			__Game_Routes_WorkStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.WorkStop>(true);
			__Game_Routes_AirplaneStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AirplaneStop>(true);
			__Game_Routes_SubwayStop_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SubwayStop>(true);
			__Game_Buildings_Hospital_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Hospital>(true);
			__Game_Buildings_ElectricityProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityProducer>(true);
			__Game_Buildings_Transformer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Transformer>(true);
			__Game_Buildings_Battery_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Battery>(true);
			__Game_Buildings_WaterPumpingStation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.WaterPumpingStation>(true);
			__Game_Buildings_WaterTower_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.WaterTower>(true);
			__Game_Buildings_SewageOutlet_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.SewageOutlet>(true);
			__Game_Buildings_WastewaterTreatmentPlant_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WastewaterTreatmentPlant>(true);
			__Game_Buildings_TransportDepot_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.TransportDepot>(true);
			__Game_Buildings_TransportStation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.TransportStation>(true);
			__Game_Buildings_GarbageFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.GarbageFacility>(true);
			__Game_Buildings_FireStation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.FireStation>(true);
			__Game_Buildings_PoliceStation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.PoliceStation>(true);
			__Game_Buildings_RoadMaintenance_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RoadMaintenance>(true);
			__Game_Buildings_ParkMaintenance_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkMaintenance>(true);
			__Game_Buildings_PostFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.PostFacility>(true);
			__Game_Buildings_TelecomFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.TelecomFacility>(true);
			__Game_Buildings_School_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.School>(true);
			__Game_Buildings_EmergencyShelter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.EmergencyShelter>(true);
			__Game_Buildings_DisasterFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.DisasterFacility>(true);
			__Game_Buildings_FirewatchTower_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.FirewatchTower>(true);
			__Game_Buildings_Park_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Park>(true);
			__Game_Buildings_DeathcareFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.DeathcareFacility>(true);
			__Game_Buildings_Prison_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Prison>(true);
			__Game_Buildings_AdminBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AdminBuilding>(true);
			__Game_Buildings_WelfareOffice_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.WelfareOffice>(true);
			__Game_Buildings_ResearchFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.ResearchFacility>(true);
			__Game_Buildings_ParkingFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.ParkingFacility>(true);
			__Game_Buildings_ResidentialProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResidentialProperty>(true);
			__Game_Buildings_CommercialProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CommercialProperty>(true);
			__Game_Buildings_IndustrialProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<IndustrialProperty>(true);
			__Game_Buildings_OfficeProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<OfficeProperty>(true);
			__Game_Buildings_ExtractorProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ExtractorProperty>(true);
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
			__Game_Creatures_CreatureSpawner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Creatures.CreatureSpawner>(true);
			__Game_Objects_OutsideConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.OutsideConnection>(true);
			__Game_Objects_ElectricityOutsideConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.ElectricityOutsideConnection>(true);
			__Game_Objects_WaterPipeOutsideConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.WaterPipeOutsideConnection>(true);
			__Game_Notifications_Icon_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Icon>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_TransportStopData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportStopData>(true);
			__Game_Prefabs_WorkStopData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkStopData>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private IconCommandSystem m_IconCommandSystem;

	private EntityQuery m_EntityQuery;

	private EntityQuery m_UpdatedQuery;

	private EntityQuery m_InfomodeQuery;

	private EntityQuery m_IconQuery;

	private uint m_TransportTypeMask;

	private uint m_BuildingTypeMask;

	private uint m_BuildingStatusTypeMask;

	private uint m_VehicleTypeMask;

	private uint m_MarkerTypeMask;

	private bool m_Loaded;

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
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Expected O, but got Unknown
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Expected O, but got Unknown
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Expected O, but got Unknown
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Expected O, but got Unknown
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Expected O, but got Unknown
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[3];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Routes.TransportStop>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Vehicle>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<CarTrailer>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Marker>() };
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Owner>()
		};
		array[1] = val;
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Objects.OutsideConnection>() };
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array[2] = val;
		m_EntityQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[5];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Routes.TransportStop>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array2[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Building>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>() };
		array2[1] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Vehicle>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CarTrailer>() };
		array2[2] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Marker>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Owner>() };
		array2[3] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Objects.OutsideConnection>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array2[4] = val;
		m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<InfomodeActive>() };
		val.Any = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<InfoviewTransportStopData>(),
			ComponentType.ReadOnly<InfoviewBuildingData>(),
			ComponentType.ReadOnly<InfoviewBuildingStatusData>(),
			ComponentType.ReadOnly<InfoviewVehicleData>(),
			ComponentType.ReadOnly<InfoviewMarkerData>()
		};
		array3[0] = val;
		m_InfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		EntityQueryDesc[] array4 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<TransportStopMarkerData>(),
			ComponentType.ReadOnly<BuildingMarkerData>(),
			ComponentType.ReadOnly<VehicleMarkerData>(),
			ComponentType.ReadOnly<MarkerMarkerData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		array4[0] = val;
		m_IconQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array4);
	}

	public void PostDeserialize(Context context)
	{
		m_TransportTypeMask = uint.MaxValue;
		m_BuildingTypeMask = uint.MaxValue;
		m_BuildingStatusTypeMask = uint.MaxValue;
		m_VehicleTypeMask = uint.MaxValue;
		m_MarkerTypeMask = uint.MaxValue;
		m_Loaded = true;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0632: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_076c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_0871: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0902: Unknown result type (might be due to invalid IL or missing references)
		//IL_0907: Unknown result type (might be due to invalid IL or missing references)
		//IL_091f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0924: Unknown result type (might be due to invalid IL or missing references)
		//IL_093c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0941: Unknown result type (might be due to invalid IL or missing references)
		//IL_0959: Unknown result type (might be due to invalid IL or missing references)
		//IL_095e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0976: Unknown result type (might be due to invalid IL or missing references)
		//IL_097b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0993: Unknown result type (might be due to invalid IL or missing references)
		//IL_0998: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0daf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da5: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (GetLoaded() ? m_EntityQuery : m_UpdatedQuery);
		TransportType requiredTransportStopType = TransportType.None;
		MarkerType requiredMarkerType = MarkerType.None;
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint num4 = 0u;
		uint num5 = 0u;
		if (m_ToolSystem.activeTool != null)
		{
			if (m_ToolSystem.activeTool.requireStops != TransportType.None)
			{
				num |= (uint)(1 << (int)m_ToolSystem.activeTool.requireStops);
				requiredTransportStopType = m_ToolSystem.activeTool.requireStops;
			}
			if (m_ToolSystem.activeTool.requireStopIcons)
			{
				num = uint.MaxValue;
			}
		}
		if (m_ToolSystem.actionMode.IsEditor())
		{
			num5 |= 1;
			requiredMarkerType = MarkerType.CreatureSpawner;
		}
		NativeArray<ArchetypeChunk> infomodeChunks = default(NativeArray<ArchetypeChunk>);
		if (!((EntityQuery)(ref m_InfomodeQuery)).IsEmptyIgnoreFilter)
		{
			ComponentTypeHandle<InfoviewTransportStopData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<InfoviewTransportStopData>(ref __TypeHandle.__Game_Prefabs_InfoviewTransportStopData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<InfoviewBuildingData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<InfoviewBuildingData>(ref __TypeHandle.__Game_Prefabs_InfoviewBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<InfoviewBuildingStatusData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<InfoviewBuildingStatusData>(ref __TypeHandle.__Game_Prefabs_InfoviewBuildingStatusData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<InfoviewVehicleData> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<InfoviewVehicleData>(ref __TypeHandle.__Game_Prefabs_InfoviewVehicleData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<InfoviewMarkerData> componentTypeHandle5 = InternalCompilerInterface.GetComponentTypeHandle<InfoviewMarkerData>(ref __TypeHandle.__Game_Prefabs_InfoviewMarkerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			infomodeChunks = ((EntityQuery)(ref m_InfomodeQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int i = 0; i < infomodeChunks.Length; i++)
			{
				ArchetypeChunk val2 = infomodeChunks[i];
				NativeArray<InfoviewTransportStopData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<InfoviewTransportStopData>(ref componentTypeHandle);
				NativeArray<InfoviewBuildingData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<InfoviewBuildingData>(ref componentTypeHandle2);
				NativeArray<InfoviewBuildingStatusData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<InfoviewBuildingStatusData>(ref componentTypeHandle3);
				NativeArray<InfoviewVehicleData> nativeArray4 = ((ArchetypeChunk)(ref val2)).GetNativeArray<InfoviewVehicleData>(ref componentTypeHandle4);
				NativeArray<InfoviewMarkerData> nativeArray5 = ((ArchetypeChunk)(ref val2)).GetNativeArray<InfoviewMarkerData>(ref componentTypeHandle5);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfoviewTransportStopData infoviewTransportStopData = nativeArray[j];
					if (infoviewTransportStopData.m_Type != TransportType.None)
					{
						num |= (uint)(1 << (int)infoviewTransportStopData.m_Type);
					}
				}
				for (int k = 0; k < nativeArray2.Length; k++)
				{
					num2 |= (uint)(1 << (int)nativeArray2[k].m_Type);
				}
				for (int l = 0; l < nativeArray3.Length; l++)
				{
					num3 |= (uint)(1 << (int)nativeArray3[l].m_Type);
				}
				for (int m = 0; m < nativeArray4.Length; m++)
				{
					num4 |= (uint)(1 << (int)nativeArray4[m].m_Type);
				}
				for (int n = 0; n < nativeArray5.Length; n++)
				{
					num5 |= (uint)(1 << (int)nativeArray5[n].m_Type);
				}
			}
		}
		if (num == m_TransportTypeMask && num2 == m_BuildingTypeMask && num3 == m_BuildingStatusTypeMask && num4 == m_VehicleTypeMask && num5 == m_MarkerTypeMask && ((num == 0 && num2 == 0 && num3 == 0 && num4 == 0 && num5 == 0) || ((EntityQuery)(ref val)).IsEmptyIgnoreFilter))
		{
			if (infomodeChunks.IsCreated)
			{
				infomodeChunks.Dispose();
			}
			return;
		}
		m_TransportTypeMask = num;
		m_BuildingTypeMask = num2;
		m_BuildingStatusTypeMask = num3;
		m_VehicleTypeMask = num4;
		m_MarkerTypeMask = num5;
		JobHandle val3 = default(JobHandle);
		NativeList<ArchetypeChunk> iconChunks = ((EntityQuery)(ref m_IconQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3);
		JobHandle val4 = JobChunkExtensions.ScheduleParallel<MarkerCreateJob>(new MarkerCreateJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenType = InternalCompilerInterface.GetComponentTypeHandle<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewTransportStopType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewTransportStopData>(ref __TypeHandle.__Game_Prefabs_InfoviewTransportStopData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewBuildingType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewBuildingData>(ref __TypeHandle.__Game_Prefabs_InfoviewBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewBuildingStatusType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewBuildingStatusData>(ref __TypeHandle.__Game_Prefabs_InfoviewBuildingStatusData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewVehicleType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewVehicleData>(ref __TypeHandle.__Game_Prefabs_InfoviewVehicleData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewMarkerType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewMarkerData>(ref __TypeHandle.__Game_Prefabs_InfoviewMarkerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfomodeActiveType = InternalCompilerInterface.GetComponentTypeHandle<InfomodeActive>(ref __TypeHandle.__Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStopMarkerType = InternalCompilerInterface.GetComponentTypeHandle<TransportStopMarkerData>(ref __TypeHandle.__Game_Prefabs_TransportStopMarkerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingMarkerType = InternalCompilerInterface.GetComponentTypeHandle<BuildingMarkerData>(ref __TypeHandle.__Game_Prefabs_BuildingMarkerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleMarkerType = InternalCompilerInterface.GetComponentTypeHandle<VehicleMarkerData>(ref __TypeHandle.__Game_Prefabs_VehicleMarkerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MarkerMarkerType = InternalCompilerInterface.GetComponentTypeHandle<MarkerMarkerData>(ref __TypeHandle.__Game_Prefabs_MarkerMarkerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStopType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.TransportStop>(ref __TypeHandle.__Game_Routes_TransportStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleType = InternalCompilerInterface.GetComponentTypeHandle<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MarkerType = InternalCompilerInterface.GetComponentTypeHandle<Marker>(ref __TypeHandle.__Game_Objects_Marker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerType = InternalCompilerInterface.GetComponentTypeHandle<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarType = InternalCompilerInterface.GetComponentTypeHandle<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainType = InternalCompilerInterface.GetComponentTypeHandle<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UniqueObjectType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.UniqueObject>(ref __TypeHandle.__Game_Objects_UniqueObject_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IconElementType = InternalCompilerInterface.GetBufferTypeHandle<IconElement>(ref __TypeHandle.__Game_Notifications_IconElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BusStopType = InternalCompilerInterface.GetComponentTypeHandle<BusStop>(ref __TypeHandle.__Game_Routes_BusStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrainStopType = InternalCompilerInterface.GetComponentTypeHandle<TrainStop>(ref __TypeHandle.__Game_Routes_TrainStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiStandType = InternalCompilerInterface.GetComponentTypeHandle<TaxiStand>(ref __TypeHandle.__Game_Routes_TaxiStand_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TramStopType = InternalCompilerInterface.GetComponentTypeHandle<TramStop>(ref __TypeHandle.__Game_Routes_TramStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ShipStopType = InternalCompilerInterface.GetComponentTypeHandle<ShipStop>(ref __TypeHandle.__Game_Routes_ShipStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MailBoxType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.MailBox>(ref __TypeHandle.__Game_Routes_MailBox_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WorkStopType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.WorkStop>(ref __TypeHandle.__Game_Routes_WorkStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AirplaneStopType = InternalCompilerInterface.GetComponentTypeHandle<AirplaneStop>(ref __TypeHandle.__Game_Routes_AirplaneStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubwayStopType = InternalCompilerInterface.GetComponentTypeHandle<SubwayStop>(ref __TypeHandle.__Game_Routes_SubwayStop_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HospitalType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Hospital>(ref __TypeHandle.__Game_Buildings_Hospital_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityProducerType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityProducer>(ref __TypeHandle.__Game_Buildings_ElectricityProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Transformer>(ref __TypeHandle.__Game_Buildings_Transformer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BatteryType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Battery>(ref __TypeHandle.__Game_Buildings_Battery_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPumpingStationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.WaterPumpingStation>(ref __TypeHandle.__Game_Buildings_WaterPumpingStation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterTowerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.WaterTower>(ref __TypeHandle.__Game_Buildings_WaterTower_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SewageOutletType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.SewageOutlet>(ref __TypeHandle.__Game_Buildings_SewageOutlet_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WastewaterTreatmentPlantType = InternalCompilerInterface.GetComponentTypeHandle<WastewaterTreatmentPlant>(ref __TypeHandle.__Game_Buildings_WastewaterTreatmentPlant_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportDepotType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.TransportDepot>(ref __TypeHandle.__Game_Buildings_TransportDepot_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.TransportStation>(ref __TypeHandle.__Game_Buildings_TransportStation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.GarbageFacility>(ref __TypeHandle.__Game_Buildings_GarbageFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FireStationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.FireStation>(ref __TypeHandle.__Game_Buildings_FireStation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceStationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.PoliceStation>(ref __TypeHandle.__Game_Buildings_PoliceStation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RoadMaintenanceType = InternalCompilerInterface.GetComponentTypeHandle<RoadMaintenance>(ref __TypeHandle.__Game_Buildings_RoadMaintenance_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkMaintenanceType = InternalCompilerInterface.GetComponentTypeHandle<ParkMaintenance>(ref __TypeHandle.__Game_Buildings_ParkMaintenance_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PostFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.PostFacility>(ref __TypeHandle.__Game_Buildings_PostFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TelecomFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.TelecomFacility>(ref __TypeHandle.__Game_Buildings_TelecomFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SchoolType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.School>(ref __TypeHandle.__Game_Buildings_School_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EmergencyShelterType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.EmergencyShelter>(ref __TypeHandle.__Game_Buildings_EmergencyShelter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DisasterFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.DisasterFacility>(ref __TypeHandle.__Game_Buildings_DisasterFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FirewatchTowerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.FirewatchTower>(ref __TypeHandle.__Game_Buildings_FirewatchTower_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeathcareFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.DeathcareFacility>(ref __TypeHandle.__Game_Buildings_DeathcareFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrisonType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Prison>(ref __TypeHandle.__Game_Buildings_Prison_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AdminBuildingType = InternalCompilerInterface.GetComponentTypeHandle<AdminBuilding>(ref __TypeHandle.__Game_Buildings_AdminBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WelfareOfficeType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.WelfareOffice>(ref __TypeHandle.__Game_Buildings_WelfareOffice_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResearchFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.ResearchFacility>(ref __TypeHandle.__Game_Buildings_ResearchFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.ParkingFacility>(ref __TypeHandle.__Game_Buildings_ParkingFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentialPropertyType = InternalCompilerInterface.GetComponentTypeHandle<ResidentialProperty>(ref __TypeHandle.__Game_Buildings_ResidentialProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CommercialPropertyType = InternalCompilerInterface.GetComponentTypeHandle<CommercialProperty>(ref __TypeHandle.__Game_Buildings_CommercialProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IndustrialPropertyType = InternalCompilerInterface.GetComponentTypeHandle<IndustrialProperty>(ref __TypeHandle.__Game_Buildings_IndustrialProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OfficePropertyType = InternalCompilerInterface.GetComponentTypeHandle<OfficeProperty>(ref __TypeHandle.__Game_Buildings_OfficeProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorPropertyType = InternalCompilerInterface.GetComponentTypeHandle<ExtractorProperty>(ref __TypeHandle.__Game_Buildings_ExtractorProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
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
			m_CreatureSpawnerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Creatures.CreatureSpawner>(ref __TypeHandle.__Game_Creatures_CreatureSpawner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityOutsideConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.ElectricityOutsideConnection>(ref __TypeHandle.__Game_Objects_ElectricityOutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPipeOutsideConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.WaterPipeOutsideConnection>(ref __TypeHandle.__Game_Objects_WaterPipeOutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IconData = InternalCompilerInterface.GetComponentLookup<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStopData = InternalCompilerInterface.GetComponentLookup<TransportStopData>(ref __TypeHandle.__Game_Prefabs_TransportStopData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkStopData = InternalCompilerInterface.GetComponentLookup<WorkStopData>(ref __TypeHandle.__Game_Prefabs_WorkStopData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InfomodeChunks = infomodeChunks,
			m_IconChunks = iconChunks,
			m_RequiredTransportStopType = requiredTransportStopType,
			m_RequiredMarkerType = requiredMarkerType,
			m_RequireStandaloneStops = (num == uint.MaxValue),
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer()
		}, m_EntityQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3));
		if (infomodeChunks.IsCreated)
		{
			infomodeChunks.Dispose(val4);
		}
		iconChunks.Dispose(val4);
		m_IconCommandSystem.AddCommandBufferWriter(val4);
		((SystemBase)this).Dependency = val4;
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
	public MarkerCreateSystem()
	{
	}
}

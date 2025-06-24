using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Achievements;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
using Game.Tools;
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
public class TransportCarAISystem : GameSystemBase
{
	[BurstCompile]
	private struct TransportCarTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentRoute> m_CurrentRouteType;

		[ReadOnly]
		public BufferTypeHandle<Passenger> m_PassengerType;

		public ComponentTypeHandle<Game.Vehicles.CargoTransport> m_CargoTransportType;

		public ComponentTypeHandle<Game.Vehicles.PublicTransport> m_PublicTransportType;

		public ComponentTypeHandle<Car> m_CarType;

		public ComponentTypeHandle<CarCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<Target> m_TargetType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public ComponentTypeHandle<Odometer> m_OdometerType;

		public BufferTypeHandle<CarNavigationLane> m_CarNavigationLaneType;

		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> m_PublicTransportVehicleData;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> m_CargoTransportVehicleData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		[ReadOnly]
		public ComponentLookup<TransportVehicleRequest> m_TransportVehicleRequestData;

		[ReadOnly]
		public ComponentLookup<EvacuationRequest> m_EvacuationRequestData;

		[ReadOnly]
		public ComponentLookup<PrisonerTransportRequest> m_PrisonerTransportRequestData;

		[ReadOnly]
		public ComponentLookup<Waypoint> m_WaypointData;

		[ReadOnly]
		public ComponentLookup<Connected> m_ConnectedData;

		[ReadOnly]
		public ComponentLookup<BoardingVehicle> m_BoardingVehicleData;

		[ReadOnly]
		public ComponentLookup<RouteLane> m_RouteLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.Color> m_RouteColorData;

		[ReadOnly]
		public ComponentLookup<Game.Companies.StorageCompany> m_StorageCompanyData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TransportStation> m_TransportStationData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TransportDepot> m_TransportDepotData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Prison> m_PrisonData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.EmergencyShelter> m_EmergencyShelterData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<GarageLane> m_GarageLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<LaneOverlap> m_LaneOverlaps;

		[NativeDisableParallelForRestriction]
		public BufferLookup<PathElement> m_PathElements;

		[NativeDisableParallelForRestriction]
		public BufferLookup<LoadingResources> m_LoadingResources;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_TransportVehicleRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_EvacuationRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_PrisonerTransportRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedCarRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedAddTypes;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public TransportBoardingHelpers.BoardingData.Concurrent m_BoardingData;

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
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PathInformation> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<CurrentRoute> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentRoute>(ref m_CurrentRouteType);
			NativeArray<CarCurrentLane> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CurrentLaneType);
			NativeArray<Game.Vehicles.CargoTransport> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.CargoTransport>(ref m_CargoTransportType);
			NativeArray<Game.Vehicles.PublicTransport> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.PublicTransport>(ref m_PublicTransportType);
			NativeArray<Car> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Car>(ref m_CarType);
			NativeArray<Target> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PathOwner> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<Odometer> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Odometer>(ref m_OdometerType);
			BufferAccessor<CarNavigationLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<CarNavigationLane>(ref m_CarNavigationLaneType);
			BufferAccessor<Passenger> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Passenger>(ref m_PassengerType);
			BufferAccessor<ServiceDispatch> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Owner owner = nativeArray2[i];
				PrefabRef prefabRef = nativeArray4[i];
				PathInformation pathInformation = nativeArray3[i];
				Car car = nativeArray9[i];
				CarCurrentLane currentLane = nativeArray6[i];
				PathOwner pathOwner = nativeArray11[i];
				Target target = nativeArray10[i];
				Odometer odometer = nativeArray12[i];
				DynamicBuffer<CarNavigationLane> navigationLanes = bufferAccessor[i];
				DynamicBuffer<ServiceDispatch> serviceDispatches = bufferAccessor3[i];
				CurrentRoute currentRoute = default(CurrentRoute);
				if (nativeArray5.Length != 0)
				{
					currentRoute = nativeArray5[i];
				}
				Game.Vehicles.CargoTransport cargoTransport = default(Game.Vehicles.CargoTransport);
				if (nativeArray7.Length != 0)
				{
					cargoTransport = nativeArray7[i];
				}
				Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
				DynamicBuffer<Passenger> passengers = default(DynamicBuffer<Passenger>);
				if (nativeArray8.Length != 0)
				{
					publicTransport = nativeArray8[i];
					passengers = bufferAccessor2[i];
				}
				VehicleUtils.CheckUnspawned(unfilteredChunkIndex, val, currentLane, isUnspawned, m_CommandBuffer);
				Tick(unfilteredChunkIndex, val, owner, pathInformation, prefabRef, currentRoute, navigationLanes, passengers, serviceDispatches, ref random, ref cargoTransport, ref publicTransport, ref car, ref currentLane, ref pathOwner, ref target, ref odometer);
				nativeArray9[i] = car;
				nativeArray6[i] = currentLane;
				nativeArray11[i] = pathOwner;
				nativeArray10[i] = target;
				nativeArray12[i] = odometer;
				if (nativeArray7.Length != 0)
				{
					nativeArray7[i] = cargoTransport;
				}
				if (nativeArray8.Length != 0)
				{
					nativeArray8[i] = publicTransport;
				}
			}
		}

		private void Tick(int jobIndex, Entity vehicleEntity, Owner owner, PathInformation pathInformation, PrefabRef prefabRef, CurrentRoute currentRoute, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<Passenger> passengers, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Random random, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Car car, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, ref Odometer odometer)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0671: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_0710: Unknown result type (might be due to invalid IL or missing references)
			PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
			bool flag = m_PublicTransportVehicleData.TryGetComponent(prefabRef.m_Prefab, ref publicTransportVehicleData);
			CargoTransportVehicleData cargoTransportVehicleData = default(CargoTransportVehicleData);
			bool flag2 = m_CargoTransportVehicleData.TryGetComponent(prefabRef.m_Prefab, ref cargoTransportVehicleData);
			if (VehicleUtils.ResetUpdatedPath(ref pathOwner))
			{
				ResetPath(jobIndex, vehicleEntity, pathInformation, serviceDispatches, ref random, ref cargoTransport, ref publicTransport, ref car, ref currentLane, ref pathOwner, flag);
				DynamicBuffer<LoadingResources> loadingResources = default(DynamicBuffer<LoadingResources>);
				if (((publicTransport.m_State & PublicTransportFlags.DummyTraffic) != 0 || (cargoTransport.m_State & CargoTransportFlags.DummyTraffic) != 0) && m_LoadingResources.TryGetBuffer(vehicleEntity, ref loadingResources))
				{
					CheckDummyResources(jobIndex, vehicleEntity, prefabRef, loadingResources);
				}
			}
			bool flag3 = (cargoTransport.m_State & CargoTransportFlags.EnRoute) == 0 && (publicTransport.m_State & PublicTransportFlags.EnRoute) == 0;
			bool flag4 = false;
			if (flag)
			{
				if ((publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport)) != 0)
				{
					if (!passengers.IsCreated || passengers.Length >= publicTransportVehicleData.m_PassengerCapacity)
					{
						publicTransport.m_State |= PublicTransportFlags.Full;
						flag3 = false;
					}
					else
					{
						publicTransport.m_State &= ~PublicTransportFlags.Full;
					}
					flag4 = true;
				}
				if (odometer.m_Distance >= publicTransportVehicleData.m_MaintenanceRange && publicTransportVehicleData.m_MaintenanceRange > 0.1f && (publicTransport.m_State & PublicTransportFlags.Refueling) == 0)
				{
					publicTransport.m_State |= PublicTransportFlags.RequiresMaintenance;
				}
			}
			if (flag2 && odometer.m_Distance >= cargoTransportVehicleData.m_MaintenanceRange && cargoTransportVehicleData.m_MaintenanceRange > 0.1f && (cargoTransport.m_State & CargoTransportFlags.Refueling) == 0)
			{
				cargoTransport.m_State |= CargoTransportFlags.RequiresMaintenance;
			}
			if (flag3)
			{
				CheckServiceDispatches(vehicleEntity, serviceDispatches, flag4, ref cargoTransport, ref publicTransport, ref pathOwner);
				if (serviceDispatches.Length <= math.select(0, 1, flag4) && (cargoTransport.m_State & (CargoTransportFlags.RequiresMaintenance | CargoTransportFlags.DummyTraffic | CargoTransportFlags.Disabled)) == 0 && (publicTransport.m_State & (PublicTransportFlags.RequiresMaintenance | PublicTransportFlags.DummyTraffic | PublicTransportFlags.Disabled)) == 0)
				{
					RequestTargetIfNeeded(jobIndex, vehicleEntity, ref publicTransport, ref cargoTransport);
				}
			}
			else
			{
				serviceDispatches.Clear();
				cargoTransport.m_RequestCount = 0;
				publicTransport.m_RequestCount = 0;
			}
			bool flag5 = false;
			Entity target2 = target.m_Target;
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(target.m_Target) || VehicleUtils.PathfindFailed(pathOwner))
			{
				if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
				{
					flag5 = true;
					StopBoarding(vehicleEntity, currentRoute, passengers, ref cargoTransport, ref publicTransport, ref target, ref odometer, forcedStop: true);
				}
				if (VehicleUtils.IsStuck(pathOwner) || (cargoTransport.m_State & (CargoTransportFlags.Returning | CargoTransportFlags.DummyTraffic)) != 0 || (publicTransport.m_State & (PublicTransportFlags.Returning | PublicTransportFlags.DummyTraffic)) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
					return;
				}
				ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref car, ref pathOwner, ref target);
			}
			else if (VehicleUtils.PathEndReached(currentLane) || VehicleUtils.ParkingSpaceReached(currentLane, pathOwner))
			{
				if ((cargoTransport.m_State & (CargoTransportFlags.Returning | CargoTransportFlags.DummyTraffic)) != 0 || (publicTransport.m_State & (PublicTransportFlags.Returning | PublicTransportFlags.DummyTraffic)) != 0)
				{
					if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
					{
						if (StopBoarding(vehicleEntity, currentRoute, passengers, ref cargoTransport, ref publicTransport, ref target, ref odometer, forcedStop: false))
						{
							flag5 = true;
							if (!SelectNextDispatch(jobIndex, vehicleEntity, currentRoute, navigationLanes, serviceDispatches, ref cargoTransport, ref publicTransport, ref car, ref currentLane, ref pathOwner, ref target, flag))
							{
								if (VehicleUtils.ParkingSpaceReached(currentLane, pathOwner))
								{
									ParkCar(jobIndex, vehicleEntity, owner, ref cargoTransport, ref publicTransport, ref car, ref currentLane);
								}
								else
								{
									((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
								}
								return;
							}
						}
					}
					else if ((!passengers.IsCreated || passengers.Length <= 0 || !StartBoarding(jobIndex, vehicleEntity, currentRoute, prefabRef, ref cargoTransport, ref publicTransport, ref target, flag2)) && !SelectNextDispatch(jobIndex, vehicleEntity, currentRoute, navigationLanes, serviceDispatches, ref cargoTransport, ref publicTransport, ref car, ref currentLane, ref pathOwner, ref target, flag))
					{
						if (VehicleUtils.ParkingSpaceReached(currentLane, pathOwner))
						{
							ParkCar(jobIndex, vehicleEntity, owner, ref cargoTransport, ref publicTransport, ref car, ref currentLane);
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
						}
						return;
					}
				}
				else if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
				{
					if (StopBoarding(vehicleEntity, currentRoute, passengers, ref cargoTransport, ref publicTransport, ref target, ref odometer, forcedStop: false))
					{
						flag5 = true;
						if ((publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport)) != 0)
						{
							if (!SelectNextDispatch(jobIndex, vehicleEntity, currentRoute, navigationLanes, serviceDispatches, ref cargoTransport, ref publicTransport, ref car, ref currentLane, ref pathOwner, ref target, flag))
							{
								ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref car, ref pathOwner, ref target);
							}
						}
						else if ((cargoTransport.m_State & CargoTransportFlags.EnRoute) == 0 && (publicTransport.m_State & PublicTransportFlags.EnRoute) == 0)
						{
							ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref car, ref pathOwner, ref target);
						}
						else
						{
							SetNextWaypointTarget(currentRoute, ref pathOwner, ref target);
						}
					}
				}
				else if ((publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport)) == 0 && (!m_RouteWaypoints.HasBuffer(currentRoute.m_Route) || !m_WaypointData.HasComponent(target.m_Target)))
				{
					ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref car, ref pathOwner, ref target);
				}
				else if (!StartBoarding(jobIndex, vehicleEntity, currentRoute, prefabRef, ref cargoTransport, ref publicTransport, ref target, flag2))
				{
					if ((publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport)) != 0)
					{
						if (!SelectNextDispatch(jobIndex, vehicleEntity, currentRoute, navigationLanes, serviceDispatches, ref cargoTransport, ref publicTransport, ref car, ref currentLane, ref pathOwner, ref target, flag))
						{
							ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref car, ref pathOwner, ref target);
						}
					}
					else if ((cargoTransport.m_State & CargoTransportFlags.EnRoute) == 0 && (publicTransport.m_State & PublicTransportFlags.EnRoute) == 0)
					{
						ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref car, ref pathOwner, ref target);
					}
					else
					{
						SetNextWaypointTarget(currentRoute, ref pathOwner, ref target);
					}
				}
			}
			else if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
			{
				flag5 = true;
				StopBoarding(vehicleEntity, currentRoute, passengers, ref cargoTransport, ref publicTransport, ref target, ref odometer, forcedStop: true);
			}
			publicTransport.m_State &= ~(PublicTransportFlags.StopLeft | PublicTransportFlags.StopRight);
			Entity skipWaypoint = Entity.Null;
			if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
			{
				if (!flag5)
				{
					UpdateStop(navigationLanes, ref currentLane, ref publicTransport, ref target);
				}
			}
			else if ((cargoTransport.m_State & CargoTransportFlags.Returning) != 0 || (publicTransport.m_State & PublicTransportFlags.Returning) != 0)
			{
				if (!passengers.IsCreated || passengers.Length == 0)
				{
					SelectNextDispatch(jobIndex, vehicleEntity, currentRoute, navigationLanes, serviceDispatches, ref cargoTransport, ref publicTransport, ref car, ref currentLane, ref pathOwner, ref target, flag);
				}
			}
			else if ((cargoTransport.m_State & CargoTransportFlags.Arriving) != 0 || (publicTransport.m_State & PublicTransportFlags.Arriving) != 0)
			{
				UpdateStop(navigationLanes, ref currentLane, ref publicTransport, ref target);
			}
			else
			{
				CheckNavigationLanes(vehicleEntity, currentRoute, navigationLanes, ref cargoTransport, ref publicTransport, ref currentLane, ref pathOwner, ref target, out skipWaypoint);
			}
			cargoTransport.m_State &= ~CargoTransportFlags.Testing;
			publicTransport.m_State &= ~PublicTransportFlags.Testing;
			if (((cargoTransport.m_State & CargoTransportFlags.Boarding) == 0 && (publicTransport.m_State & PublicTransportFlags.Boarding) == 0) || flag5)
			{
				if (VehicleUtils.RequireNewPath(pathOwner))
				{
					FindNewPath(vehicleEntity, prefabRef, skipWaypoint, target2, ref currentLane, ref cargoTransport, ref publicTransport, ref pathOwner, ref target);
				}
				else if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Stuck)) == 0)
				{
					CheckParkingSpace(vehicleEntity, ref random, ref currentLane, ref pathOwner, navigationLanes);
				}
			}
		}

		private void UpdateStop(DynamicBuffer<CarNavigationLane> navigationLanes, ref CarCurrentLane currentLane, ref Game.Vehicles.PublicTransport publicTransport, ref Target target)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			Connected connected = default(Connected);
			Transform transform = default(Transform);
			if (!m_ConnectedData.TryGetComponent(target.m_Target, ref connected) || !m_TransformData.TryGetComponent(connected.m_Connected, ref transform))
			{
				return;
			}
			Entity val = Entity.Null;
			float2 val2 = float2.op_Implicit(0f);
			for (int num = navigationLanes.Length - 1; num >= 0; num--)
			{
				CarNavigationLane carNavigationLane = navigationLanes[num];
				if (carNavigationLane.m_CurvePosition.y - carNavigationLane.m_CurvePosition.x != 0f)
				{
					val = carNavigationLane.m_Lane;
					val2 = carNavigationLane.m_CurvePosition;
					break;
				}
			}
			if (val2.x == val2.y)
			{
				val = currentLane.m_Lane;
				val2 = ((float3)(ref currentLane.m_CurvePosition)).xz;
			}
			Curve curve = default(Curve);
			if (val2.x != val2.y && m_CurveData.TryGetComponent(val, ref curve))
			{
				float3 val3 = MathUtils.Position(curve.m_Bezier, val2.y);
				float2 xz = ((float3)(ref val3)).xz;
				val3 = MathUtils.Tangent(curve.m_Bezier, val2.y);
				float2 xz2 = ((float3)(ref val3)).xz;
				float2 val4 = ((float3)(ref transform.m_Position)).xz - xz;
				if (math.dot(MathUtils.Left(math.select(xz2, -xz2, val2.y < val2.x)), val4) > 0f)
				{
					publicTransport.m_State |= PublicTransportFlags.StopLeft;
					currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.TurnLeft;
				}
				else
				{
					publicTransport.m_State |= PublicTransportFlags.StopRight;
					currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.TurnRight;
				}
			}
		}

		private void CheckParkingSpace(Entity entity, ref Random random, ref CarCurrentLane currentLane, ref PathOwner pathOwner, DynamicBuffer<CarNavigationLane> navigationLanes)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PathElement> path = m_PathElements[entity];
			ComponentLookup<Blocker> blockerData = default(ComponentLookup<Blocker>);
			VehicleUtils.ValidateParkingSpace(entity, ref random, ref currentLane, ref pathOwner, navigationLanes, path, ref m_ParkedCarData, ref blockerData, ref m_CurveData, ref m_UnspawnedData, ref m_ParkingLaneData, ref m_GarageLaneData, ref m_ConnectionLaneData, ref m_PrefabRefData, ref m_PrefabParkingLaneData, ref m_PrefabObjectGeometryData, ref m_LaneObjects, ref m_LaneOverlaps, ignoreDriveways: false, ignoreDisabled: false, boardingOnly: false);
		}

		private void ParkCar(int jobIndex, Entity entity, Owner owner, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Car car, ref CarCurrentLane currentLane)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			car.m_Flags &= ~CarFlags.Emergency;
			cargoTransport.m_State &= CargoTransportFlags.RequiresMaintenance;
			publicTransport.m_State &= PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport | PublicTransportFlags.RequiresMaintenance;
			Game.Buildings.TransportDepot transportDepot = default(Game.Buildings.TransportDepot);
			if ((publicTransport.m_State & PublicTransportFlags.PrisonerTransport) != 0)
			{
				Game.Buildings.Prison prison = default(Game.Buildings.Prison);
				if (m_PrisonData.TryGetComponent(owner.m_Owner, ref prison) && (prison.m_Flags & (PrisonFlags.HasAvailablePrisonVans | PrisonFlags.HasPrisonerSpace)) != (PrisonFlags.HasAvailablePrisonVans | PrisonFlags.HasPrisonerSpace))
				{
					cargoTransport.m_State |= CargoTransportFlags.Disabled;
					publicTransport.m_State |= PublicTransportFlags.Disabled;
				}
			}
			else if ((publicTransport.m_State & PublicTransportFlags.Evacuating) != 0)
			{
				Game.Buildings.EmergencyShelter emergencyShelter = default(Game.Buildings.EmergencyShelter);
				if (m_EmergencyShelterData.TryGetComponent(owner.m_Owner, ref emergencyShelter) && (emergencyShelter.m_Flags & (EmergencyShelterFlags.HasAvailableVehicles | EmergencyShelterFlags.HasShelterSpace)) != (EmergencyShelterFlags.HasAvailableVehicles | EmergencyShelterFlags.HasShelterSpace))
				{
					cargoTransport.m_State |= CargoTransportFlags.Disabled;
					publicTransport.m_State |= PublicTransportFlags.Disabled;
				}
			}
			else if (m_TransportDepotData.TryGetComponent(owner.m_Owner, ref transportDepot) && (transportDepot.m_Flags & TransportDepotFlags.HasAvailableVehicles) == 0)
			{
				cargoTransport.m_State |= CargoTransportFlags.Disabled;
				publicTransport.m_State |= PublicTransportFlags.Disabled;
			}
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, entity, ref m_MovingToParkedCarRemoveTypes);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, entity, ref m_MovingToParkedAddTypes);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkedCar>(jobIndex, entity, new ParkedCar(currentLane.m_Lane, currentLane.m_CurvePosition.x));
			if (m_ParkingLaneData.HasComponent(currentLane.m_Lane) && currentLane.m_ChangeLane == Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, currentLane.m_Lane);
			}
			else if (m_GarageLaneData.HasComponent(currentLane.m_Lane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, currentLane.m_Lane);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<FixParkingLocation>(jobIndex, entity, new FixParkingLocation(currentLane.m_ChangeLane, entity));
			}
			else
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<FixParkingLocation>(jobIndex, entity, new FixParkingLocation(currentLane.m_ChangeLane, entity));
			}
		}

		private void FindNewPath(Entity vehicleEntity, PrefabRef prefabRef, Entity skipWaypoint, Entity previousTarget, ref CarCurrentLane currentLane, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			CarData carData = m_PrefabCarData[prefabRef.m_Prefab];
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(carData.m_MaxSpeed),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Methods = (VehicleUtils.GetPathMethods(carData) | PathMethod.SpecialParking),
				m_ParkingTarget = VehicleUtils.GetParkingSource(vehicleEntity, currentLane, ref m_ParkingLaneData, ref m_ConnectionLaneData),
				m_ParkingDelta = currentLane.m_CurvePosition.z,
				m_ParkingSize = VehicleUtils.GetParkingSize(vehicleEntity, ref m_PrefabRefData, ref m_PrefabObjectGeometryData),
				m_IgnoredRules = (RuleFlags.ForbidPrivateTraffic | VehicleUtils.GetIgnoredPathfindRules(carData))
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = (VehicleUtils.GetPathMethods(carData) | PathMethod.SpecialParking),
				m_RoadTypes = RoadTypes.Car
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = VehicleUtils.GetPathMethods(carData),
				m_RoadTypes = RoadTypes.Car,
				m_Entity = target.m_Target
			};
			if ((publicTransport.m_State & (PublicTransportFlags.Returning | PublicTransportFlags.Evacuating)) == PublicTransportFlags.Evacuating)
			{
				parameters.m_Weights = new PathfindWeights(1f, 0.2f, 0f, 0.1f);
				parameters.m_IgnoredRules |= RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic;
			}
			else
			{
				parameters.m_Weights = new PathfindWeights(1f, 1f, 1f, 1f);
			}
			if (skipWaypoint != Entity.Null)
			{
				origin.m_Entity = skipWaypoint;
				pathOwner.m_State |= PathFlags.Append;
			}
			else
			{
				if (previousTarget != target.m_Target)
				{
					Entity val = currentLane.m_Lane;
					SlaveLane slaveLane = default(SlaveLane);
					Owner owner = default(Owner);
					DynamicBuffer<Game.Net.SubLane> val2 = default(DynamicBuffer<Game.Net.SubLane>);
					if (m_SlaveLaneData.TryGetComponent(val, ref slaveLane) && m_OwnerData.TryGetComponent(val, ref owner) && m_SubLanes.TryGetBuffer(owner.m_Owner, ref val2) && val2.Length > slaveLane.m_MasterIndex)
					{
						val = val2[(int)slaveLane.m_MasterIndex].m_SubLane;
					}
					RouteLane routeLane = default(RouteLane);
					if (m_RouteLaneData.TryGetComponent(previousTarget, ref routeLane) && routeLane.m_EndLane == val)
					{
						origin.m_Entity = previousTarget;
					}
				}
				pathOwner.m_State &= ~PathFlags.Append;
			}
			if ((cargoTransport.m_State & (CargoTransportFlags.EnRoute | CargoTransportFlags.RouteSource)) == (CargoTransportFlags.EnRoute | CargoTransportFlags.RouteSource) || (publicTransport.m_State & (PublicTransportFlags.EnRoute | PublicTransportFlags.RouteSource)) == (PublicTransportFlags.EnRoute | PublicTransportFlags.RouteSource))
			{
				parameters.m_PathfindFlags = PathfindFlags.Stable | PathfindFlags.IgnoreFlow;
			}
			else if ((cargoTransport.m_State & CargoTransportFlags.EnRoute) == 0 && (publicTransport.m_State & PublicTransportFlags.EnRoute) == 0)
			{
				cargoTransport.m_State &= ~CargoTransportFlags.RouteSource;
				publicTransport.m_State &= ~PublicTransportFlags.RouteSource;
			}
			if ((cargoTransport.m_State & CargoTransportFlags.Returning) != 0 || (publicTransport.m_State & PublicTransportFlags.Returning) != 0)
			{
				destination.m_Methods |= PathMethod.SpecialParking;
				destination.m_RandomCost = 30f;
			}
			VehicleUtils.SetupPathfind(item: new SetupQueueItem(vehicleEntity, parameters, origin, destination), currentLane: ref currentLane, pathOwner: ref pathOwner, queue: m_PathfindQueue);
		}

		private void CheckNavigationLanes(Entity vehicleEntity, CurrentRoute currentRoute, DynamicBuffer<CarNavigationLane> navigationLanes, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, out Entity skipWaypoint)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			skipWaypoint = Entity.Null;
			if (navigationLanes.Length >= 8)
			{
				return;
			}
			CarNavigationLane carNavigationLane = default(CarNavigationLane);
			if (navigationLanes.Length != 0)
			{
				carNavigationLane = navigationLanes[navigationLanes.Length - 1];
				if ((carNavigationLane.m_Flags & Game.Vehicles.CarLaneFlags.EndOfPath) == 0)
				{
					return;
				}
			}
			else if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.EndOfPath) == 0)
			{
				return;
			}
			if (m_WaypointData.HasComponent(target.m_Target) && m_RouteWaypoints.HasBuffer(currentRoute.m_Route) && (!m_ConnectedData.HasComponent(target.m_Target) || !m_BoardingVehicleData.HasComponent(m_ConnectedData[target.m_Target].m_Connected)))
			{
				if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete)) != 0)
				{
					return;
				}
				skipWaypoint = target.m_Target;
				SetNextWaypointTarget(currentRoute, ref pathOwner, ref target);
				if (navigationLanes.Length != 0)
				{
					if ((carNavigationLane.m_Flags & Game.Vehicles.CarLaneFlags.GroupTarget) != 0)
					{
						navigationLanes.RemoveAt(navigationLanes.Length - 1);
					}
					else
					{
						carNavigationLane.m_Flags &= ~Game.Vehicles.CarLaneFlags.EndOfPath;
						navigationLanes[navigationLanes.Length - 1] = carNavigationLane;
					}
				}
				else
				{
					currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.EndOfPath;
				}
				cargoTransport.m_State |= CargoTransportFlags.RouteSource;
				publicTransport.m_State |= PublicTransportFlags.RouteSource;
				return;
			}
			if (m_WaypointData.HasComponent(target.m_Target) && m_RouteWaypoints.HasBuffer(currentRoute.m_Route))
			{
				Connected connected = m_ConnectedData[target.m_Target];
				if (GetTransportStationFromStop(connected.m_Connected) == Entity.Null && (cargoTransport.m_State & (CargoTransportFlags.RequiresMaintenance | CargoTransportFlags.AbandonRoute)) == 0 && (publicTransport.m_State & (PublicTransportFlags.RequiresMaintenance | PublicTransportFlags.AbandonRoute)) == 0)
				{
					if (!(m_BoardingVehicleData[connected.m_Connected].m_Testing == vehicleEntity))
					{
						if (navigationLanes.Length != 0 && (carNavigationLane.m_Flags & Game.Vehicles.CarLaneFlags.Reserved) == 0)
						{
							if (navigationLanes.Length < 2)
							{
								return;
							}
							CarNavigationLane carNavigationLane2 = navigationLanes[navigationLanes.Length - 2];
							Owner owner = default(Owner);
							Owner owner2 = default(Owner);
							if ((carNavigationLane2.m_Flags & Game.Vehicles.CarLaneFlags.Reserved) == 0 || !m_OwnerData.TryGetComponent(carNavigationLane.m_Lane, ref owner) || !m_OwnerData.TryGetComponent(carNavigationLane2.m_Lane, ref owner2) || owner.m_Owner != owner2.m_Owner)
							{
								return;
							}
						}
						m_BoardingData.BeginTesting(vehicleEntity, currentRoute.m_Route, connected.m_Connected, target.m_Target);
						return;
					}
					m_BoardingData.EndTesting(vehicleEntity, currentRoute.m_Route, connected.m_Connected, target.m_Target);
					if ((cargoTransport.m_State & CargoTransportFlags.RequireStop) == 0 && (publicTransport.m_State & PublicTransportFlags.RequireStop) == 0)
					{
						if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete)) != 0)
						{
							return;
						}
						skipWaypoint = target.m_Target;
						SetNextWaypointTarget(currentRoute, ref pathOwner, ref target);
						if (navigationLanes.Length != 0)
						{
							if ((carNavigationLane.m_Flags & Game.Vehicles.CarLaneFlags.GroupTarget) != 0)
							{
								navigationLanes.RemoveAt(navigationLanes.Length - 1);
							}
							else
							{
								carNavigationLane.m_Flags &= ~Game.Vehicles.CarLaneFlags.EndOfPath;
								navigationLanes[navigationLanes.Length - 1] = carNavigationLane;
							}
						}
						else
						{
							currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.EndOfPath;
						}
						cargoTransport.m_State |= CargoTransportFlags.RouteSource;
						publicTransport.m_State |= PublicTransportFlags.RouteSource;
						return;
					}
				}
			}
			cargoTransport.m_State |= CargoTransportFlags.Arriving;
			publicTransport.m_State |= PublicTransportFlags.Arriving;
			if (!m_RouteLaneData.HasComponent(target.m_Target))
			{
				return;
			}
			RouteLane routeLane = m_RouteLaneData[target.m_Target];
			if (routeLane.m_StartLane != routeLane.m_EndLane)
			{
				CarNavigationLane carNavigationLane3 = default(CarNavigationLane);
				if (navigationLanes.Length != 0)
				{
					carNavigationLane.m_CurvePosition.y = 1f;
					carNavigationLane3.m_Lane = carNavigationLane.m_Lane;
				}
				else
				{
					currentLane.m_CurvePosition.z = 1f;
					carNavigationLane3.m_Lane = currentLane.m_Lane;
				}
				if (NetUtils.FindNextLane(ref carNavigationLane3.m_Lane, ref m_OwnerData, ref m_LaneData, ref m_SubLanes))
				{
					if (navigationLanes.Length != 0)
					{
						carNavigationLane.m_Flags &= ~Game.Vehicles.CarLaneFlags.EndOfPath;
						navigationLanes[navigationLanes.Length - 1] = carNavigationLane;
					}
					else
					{
						currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.EndOfPath;
					}
					carNavigationLane3.m_Flags |= Game.Vehicles.CarLaneFlags.EndOfPath | Game.Vehicles.CarLaneFlags.FixedLane;
					carNavigationLane3.m_CurvePosition = new float2(0f, routeLane.m_EndCurvePos);
					navigationLanes.Add(carNavigationLane3);
				}
				else if (navigationLanes.Length != 0)
				{
					navigationLanes[navigationLanes.Length - 1] = carNavigationLane;
				}
			}
			else if (navigationLanes.Length != 0)
			{
				carNavigationLane.m_CurvePosition.y = routeLane.m_EndCurvePos;
				navigationLanes[navigationLanes.Length - 1] = carNavigationLane;
			}
			else
			{
				currentLane.m_CurvePosition.z = routeLane.m_EndCurvePos;
			}
		}

		private void ResetPath(int jobIndex, Entity vehicleEntity, PathInformation pathInformation, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Random random, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Car car, ref CarCurrentLane currentLane, ref PathOwner pathOwner, bool isPublicTransport)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			cargoTransport.m_State &= ~CargoTransportFlags.Arriving;
			publicTransport.m_State &= ~PublicTransportFlags.Arriving;
			DynamicBuffer<PathElement> path = m_PathElements[vehicleEntity];
			if ((pathOwner.m_State & PathFlags.Append) == 0)
			{
				PathUtils.ResetPath(ref currentLane, path, m_SlaveLaneData, m_OwnerData, m_SubLanes);
				VehicleUtils.ResetParkingLaneStatus(vehicleEntity, ref currentLane, ref pathOwner, path, ref m_EntityLookup, ref m_CurveData, ref m_ParkingLaneData, ref m_CarLaneData, ref m_ConnectionLaneData, ref m_SpawnLocationData, ref m_PrefabRefData, ref m_PrefabSpawnLocationData);
			}
			VehicleUtils.SetParkingCurvePos(vehicleEntity, ref random, currentLane, pathOwner, path, ref m_ParkedCarData, ref m_UnspawnedData, ref m_CurveData, ref m_ParkingLaneData, ref m_ConnectionLaneData, ref m_PrefabRefData, ref m_PrefabObjectGeometryData, ref m_PrefabParkingLaneData, ref m_LaneObjects, ref m_LaneOverlaps, ignoreDriveways: false);
			if ((cargoTransport.m_State & (CargoTransportFlags.Returning | CargoTransportFlags.DummyTraffic)) != 0 || (publicTransport.m_State & (PublicTransportFlags.Returning | PublicTransportFlags.DummyTraffic)) != 0)
			{
				car.m_Flags &= ~CarFlags.StayOnRoad;
			}
			else if (cargoTransport.m_RequestCount + publicTransport.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				if (m_EvacuationRequestData.HasComponent(request))
				{
					car.m_Flags |= CarFlags.Emergency | CarFlags.StayOnRoad;
				}
				else if (m_PrisonerTransportRequestData.HasComponent(request))
				{
					car.m_Flags &= ~(CarFlags.Emergency | CarFlags.StayOnRoad);
				}
				else
				{
					car.m_Flags &= ~CarFlags.Emergency;
					car.m_Flags |= CarFlags.StayOnRoad;
				}
			}
			else
			{
				car.m_Flags &= ~CarFlags.Emergency;
				car.m_Flags |= CarFlags.StayOnRoad;
			}
			if (isPublicTransport)
			{
				car.m_Flags |= CarFlags.UsePublicTransportLanes | CarFlags.PreferPublicTransportLanes | CarFlags.Interior;
			}
			cargoTransport.m_PathElementTime = pathInformation.m_Duration / (float)math.max(1, path.Length);
			publicTransport.m_PathElementTime = cargoTransport.m_PathElementTime;
		}

		private void CheckDummyResources(int jobIndex, Entity vehicleEntity, PrefabRef prefabRef, DynamicBuffer<LoadingResources> loadingResources)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			if (loadingResources.Length == 0)
			{
				return;
			}
			if (m_CargoTransportVehicleData.HasComponent(prefabRef.m_Prefab))
			{
				CargoTransportVehicleData cargoTransportVehicleData = m_CargoTransportVehicleData[prefabRef.m_Prefab];
				DynamicBuffer<Resources> val = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<Resources>(jobIndex, vehicleEntity);
				for (int i = 0; i < loadingResources.Length; i++)
				{
					if (val.Length >= cargoTransportVehicleData.m_MaxResourceCount)
					{
						break;
					}
					LoadingResources loadingResources2 = loadingResources[i];
					int num = math.min(loadingResources2.m_Amount, cargoTransportVehicleData.m_CargoCapacity);
					loadingResources2.m_Amount -= num;
					cargoTransportVehicleData.m_CargoCapacity -= num;
					if (num > 0)
					{
						val.Add(new Resources
						{
							m_Resource = loadingResources2.m_Resource,
							m_Amount = num
						});
					}
				}
			}
			loadingResources.Clear();
		}

		private void SetNextWaypointTarget(CurrentRoute currentRoute, ref PathOwner pathOwnerData, ref Target targetData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<RouteWaypoint> val = m_RouteWaypoints[currentRoute.m_Route];
			int num = m_WaypointData[targetData.m_Target].m_Index + 1;
			num = math.select(num, 0, num >= val.Length);
			VehicleUtils.SetTarget(ref pathOwnerData, ref targetData, val[num].m_Waypoint);
		}

		private void CheckServiceDispatches(Entity vehicleEntity, DynamicBuffer<ServiceDispatch> serviceDispatches, bool allowQueued, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref PathOwner pathOwner)
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			if (!allowQueued)
			{
				if (serviceDispatches.Length > 1)
				{
					serviceDispatches.RemoveRange(1, serviceDispatches.Length - 1);
				}
				cargoTransport.m_RequestCount = math.min(1, cargoTransport.m_RequestCount);
				publicTransport.m_RequestCount = math.min(1, publicTransport.m_RequestCount);
			}
			int num = math.max(cargoTransport.m_RequestCount, publicTransport.m_RequestCount);
			if (serviceDispatches.Length <= num)
			{
				return;
			}
			float num2 = -1f;
			Entity val = Entity.Null;
			PathElement pathElement = default(PathElement);
			bool flag = false;
			int num3 = 0;
			if (num >= 1 && (cargoTransport.m_State & CargoTransportFlags.Returning) == 0 && (publicTransport.m_State & PublicTransportFlags.Returning) == 0)
			{
				DynamicBuffer<PathElement> val2 = m_PathElements[vehicleEntity];
				num3 = 1;
				if (pathOwner.m_ElementIndex < val2.Length)
				{
					pathElement = val2[val2.Length - 1];
					flag = true;
				}
			}
			DynamicBuffer<PathElement> val3 = default(DynamicBuffer<PathElement>);
			for (int i = num3; i < num; i++)
			{
				Entity request = serviceDispatches[i].m_Request;
				if (m_PathElements.TryGetBuffer(request, ref val3) && val3.Length != 0)
				{
					pathElement = val3[val3.Length - 1];
					flag = true;
				}
			}
			DynamicBuffer<PathElement> val4 = default(DynamicBuffer<PathElement>);
			DynamicBuffer<PathElement> val5 = default(DynamicBuffer<PathElement>);
			for (int j = num; j < serviceDispatches.Length; j++)
			{
				Entity request2 = serviceDispatches[j].m_Request;
				if (m_TransportVehicleRequestData.HasComponent(request2))
				{
					TransportVehicleRequest transportVehicleRequest = m_TransportVehicleRequestData[request2];
					if (m_PrefabRefData.HasComponent(transportVehicleRequest.m_Route) && transportVehicleRequest.m_Priority > num2)
					{
						num2 = transportVehicleRequest.m_Priority;
						val = request2;
					}
				}
				else if (m_EvacuationRequestData.HasComponent(request2))
				{
					EvacuationRequest evacuationRequest = m_EvacuationRequestData[request2];
					if (flag && m_PathElements.TryGetBuffer(request2, ref val4) && val4.Length != 0)
					{
						PathElement pathElement2 = val4[0];
						if (pathElement2.m_Target != pathElement.m_Target || pathElement2.m_TargetDelta.x != pathElement.m_TargetDelta.y)
						{
							continue;
						}
					}
					if (m_PrefabRefData.HasComponent(evacuationRequest.m_Target) && evacuationRequest.m_Priority > num2)
					{
						num2 = evacuationRequest.m_Priority;
						val = request2;
					}
				}
				else
				{
					if (!m_PrisonerTransportRequestData.HasComponent(request2))
					{
						continue;
					}
					PrisonerTransportRequest prisonerTransportRequest = m_PrisonerTransportRequestData[request2];
					if (flag && m_PathElements.TryGetBuffer(request2, ref val5) && val5.Length != 0)
					{
						PathElement pathElement3 = val5[0];
						if (pathElement3.m_Target != pathElement.m_Target || pathElement3.m_TargetDelta.x != pathElement.m_TargetDelta.y)
						{
							continue;
						}
					}
					if (m_PrefabRefData.HasComponent(prisonerTransportRequest.m_Target) && (float)prisonerTransportRequest.m_Priority > num2)
					{
						num2 = prisonerTransportRequest.m_Priority;
						val = request2;
					}
				}
			}
			if (val != Entity.Null)
			{
				serviceDispatches[num++] = new ServiceDispatch(val);
				publicTransport.m_RequestCount++;
				cargoTransport.m_RequestCount++;
			}
			if (serviceDispatches.Length > num)
			{
				serviceDispatches.RemoveRange(num, serviceDispatches.Length - num);
			}
		}

		private void RequestTargetIfNeeded(int jobIndex, Entity entity, ref Game.Vehicles.PublicTransport publicTransport, ref Game.Vehicles.CargoTransport cargoTransport)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			if (m_ServiceRequestData.HasComponent(publicTransport.m_TargetRequest) || m_ServiceRequestData.HasComponent(cargoTransport.m_TargetRequest))
			{
				return;
			}
			if ((publicTransport.m_State & PublicTransportFlags.Evacuating) != 0)
			{
				uint num = math.max(64u, 16u);
				if ((m_SimulationFrameIndex & (num - 1)) == 1)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_EvacuationRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<EvacuationRequest>(jobIndex, val, new EvacuationRequest(entity, 1f));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(4u));
				}
			}
			else if ((publicTransport.m_State & PublicTransportFlags.PrisonerTransport) != 0)
			{
				uint num2 = math.max(256u, 16u);
				if ((m_SimulationFrameIndex & (num2 - 1)) == 1)
				{
					Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_PrisonerTransportRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val2, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrisonerTransportRequest>(jobIndex, val2, new PrisonerTransportRequest(entity, 1));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val2, new RequestGroup(16u));
				}
			}
			else
			{
				uint num3 = math.max(256u, 16u);
				if ((m_SimulationFrameIndex & (num3 - 1)) == 1)
				{
					Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_TransportVehicleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val3, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TransportVehicleRequest>(jobIndex, val3, new TransportVehicleRequest(entity, 1f));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val3, new RequestGroup(8u));
				}
			}
		}

		private bool SelectNextDispatch(int jobIndex, Entity vehicleEntity, CurrentRoute currentRoute, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Car car, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, bool isPublicTransport)
		{
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			if ((cargoTransport.m_State & CargoTransportFlags.Returning) == 0 && (publicTransport.m_State & PublicTransportFlags.Returning) == 0 && cargoTransport.m_RequestCount + publicTransport.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				serviceDispatches.RemoveAt(0);
				cargoTransport.m_RequestCount = math.max(0, cargoTransport.m_RequestCount - 1);
				publicTransport.m_RequestCount = math.max(0, publicTransport.m_RequestCount - 1);
			}
			if ((cargoTransport.m_State & (CargoTransportFlags.RequiresMaintenance | CargoTransportFlags.Disabled)) != 0 || (publicTransport.m_State & (PublicTransportFlags.RequiresMaintenance | PublicTransportFlags.Disabled)) != 0)
			{
				cargoTransport.m_RequestCount = 0;
				publicTransport.m_RequestCount = 0;
				serviceDispatches.Clear();
				return false;
			}
			Game.Routes.Color color = default(Game.Routes.Color);
			while (cargoTransport.m_RequestCount + publicTransport.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				Entity val = Entity.Null;
				Entity val2 = Entity.Null;
				CarFlags carFlags = car.m_Flags;
				if (isPublicTransport)
				{
					carFlags |= CarFlags.UsePublicTransportLanes | CarFlags.PreferPublicTransportLanes;
				}
				if (m_TransportVehicleRequestData.HasComponent(request))
				{
					val = m_TransportVehicleRequestData[request].m_Route;
					if (m_PathInformationData.HasComponent(request))
					{
						val2 = m_PathInformationData[request].m_Destination;
					}
					carFlags = (CarFlags)((uint)carFlags & 0xFFFFFFFEu);
					carFlags |= CarFlags.StayOnRoad;
				}
				else if (m_EvacuationRequestData.HasComponent(request))
				{
					val2 = m_EvacuationRequestData[request].m_Target;
					carFlags |= CarFlags.Emergency | CarFlags.StayOnRoad;
				}
				else if (m_PrisonerTransportRequestData.HasComponent(request))
				{
					val2 = m_PrisonerTransportRequestData[request].m_Target;
					carFlags = (CarFlags)((uint)carFlags & 0xFFFFFFFCu);
				}
				if (!m_PrefabRefData.HasComponent(val2))
				{
					serviceDispatches.RemoveAt(0);
					cargoTransport.m_RequestCount = math.max(0, cargoTransport.m_RequestCount - 1);
					publicTransport.m_RequestCount = math.max(0, publicTransport.m_RequestCount - 1);
					continue;
				}
				if (m_TransportVehicleRequestData.HasComponent(request))
				{
					serviceDispatches.Clear();
					cargoTransport.m_RequestCount = 0;
					publicTransport.m_RequestCount = 0;
					if (m_PrefabRefData.HasComponent(val))
					{
						if (currentRoute.m_Route != val)
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentRoute>(jobIndex, vehicleEntity, new CurrentRoute(val));
							((ParallelWriter)(ref m_CommandBuffer)).AppendToBuffer<RouteVehicle>(jobIndex, val, new RouteVehicle(vehicleEntity));
							if (m_RouteColorData.TryGetComponent(val, ref color))
							{
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Routes.Color>(jobIndex, vehicleEntity, color);
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, vehicleEntity);
							}
						}
						cargoTransport.m_State |= CargoTransportFlags.EnRoute;
						publicTransport.m_State |= PublicTransportFlags.EnRoute;
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentRoute>(jobIndex, vehicleEntity);
					}
					Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val3, new HandleRequest(request, vehicleEntity, completed: true));
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentRoute>(jobIndex, vehicleEntity);
					Entity val4 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val4, new HandleRequest(request, vehicleEntity, completed: false, pathConsumed: true));
				}
				cargoTransport.m_State &= ~CargoTransportFlags.Returning;
				publicTransport.m_State &= ~PublicTransportFlags.Returning;
				car.m_Flags = carFlags;
				if (m_ServiceRequestData.HasComponent(publicTransport.m_TargetRequest))
				{
					Entity val5 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val5, new HandleRequest(publicTransport.m_TargetRequest, Entity.Null, completed: true));
				}
				if (m_ServiceRequestData.HasComponent(cargoTransport.m_TargetRequest))
				{
					Entity val6 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val6, new HandleRequest(cargoTransport.m_TargetRequest, Entity.Null, completed: true));
				}
				if (m_PathElements.HasBuffer(request))
				{
					DynamicBuffer<PathElement> appendPath = m_PathElements[request];
					if (appendPath.Length != 0)
					{
						DynamicBuffer<PathElement> val7 = m_PathElements[vehicleEntity];
						PathUtils.TrimPath(val7, ref pathOwner);
						float num = math.max(cargoTransport.m_PathElementTime, publicTransport.m_PathElementTime) * (float)val7.Length + m_PathInformationData[request].m_Duration;
						if (PathUtils.TryAppendPath(ref currentLane, navigationLanes, val7, appendPath, m_SlaveLaneData, m_OwnerData, m_SubLanes))
						{
							cargoTransport.m_PathElementTime = num / (float)math.max(1, val7.Length);
							publicTransport.m_PathElementTime = cargoTransport.m_PathElementTime;
							target.m_Target = val2;
							VehicleUtils.ClearEndOfPath(ref currentLane, navigationLanes);
							cargoTransport.m_State &= ~CargoTransportFlags.Arriving;
							publicTransport.m_State &= ~PublicTransportFlags.Arriving;
							return true;
						}
					}
				}
				VehicleUtils.SetTarget(ref pathOwner, ref target, val2);
				return true;
			}
			return false;
		}

		private void ReturnToDepot(int jobIndex, Entity vehicleEntity, CurrentRoute currentRoute, Owner ownerData, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Car car, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			serviceDispatches.Clear();
			cargoTransport.m_RequestCount = 0;
			cargoTransport.m_State &= ~(CargoTransportFlags.EnRoute | CargoTransportFlags.Refueling | CargoTransportFlags.AbandonRoute);
			cargoTransport.m_State |= CargoTransportFlags.Returning;
			publicTransport.m_RequestCount = 0;
			publicTransport.m_State &= ~(PublicTransportFlags.EnRoute | PublicTransportFlags.Refueling | PublicTransportFlags.AbandonRoute);
			publicTransport.m_State |= PublicTransportFlags.Returning;
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentRoute>(jobIndex, vehicleEntity);
			car.m_Flags &= ~CarFlags.Emergency;
			VehicleUtils.SetTarget(ref pathOwner, ref target, ownerData.m_Owner);
		}

		private bool StartBoarding(int jobIndex, Entity vehicleEntity, CurrentRoute currentRoute, PrefabRef prefabRef, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Target target, bool isCargoVehicle)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			if ((publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport)) != 0)
			{
				publicTransport.m_State |= PublicTransportFlags.Boarding;
				if ((publicTransport.m_State & PublicTransportFlags.Returning) != 0)
				{
					publicTransport.m_DepartureFrame = m_SimulationFrameIndex + 64;
				}
				else
				{
					publicTransport.m_DepartureFrame = m_SimulationFrameIndex + 4096;
				}
				publicTransport.m_MaxBoardingDistance = 0f;
				publicTransport.m_MinWaitingDistance = float.MaxValue;
				return true;
			}
			if (m_ConnectedData.HasComponent(target.m_Target))
			{
				Connected connected = m_ConnectedData[target.m_Target];
				if (m_BoardingVehicleData.HasComponent(connected.m_Connected))
				{
					Entity transportStationFromStop = GetTransportStationFromStop(connected.m_Connected);
					Entity nextStation = Entity.Null;
					bool flag = false;
					if (m_TransportStationData.HasComponent(transportStationFromStop))
					{
						CarData carData = m_PrefabCarData[prefabRef.m_Prefab];
						flag = (m_TransportStationData[transportStationFromStop].m_CarRefuelTypes & carData.m_EnergyType) != 0;
					}
					if ((!flag && ((cargoTransport.m_State & CargoTransportFlags.RequiresMaintenance) != 0 || (publicTransport.m_State & PublicTransportFlags.RequiresMaintenance) != 0)) || (cargoTransport.m_State & CargoTransportFlags.AbandonRoute) != 0 || (publicTransport.m_State & PublicTransportFlags.AbandonRoute) != 0)
					{
						cargoTransport.m_State &= ~(CargoTransportFlags.EnRoute | CargoTransportFlags.AbandonRoute);
						publicTransport.m_State &= ~(PublicTransportFlags.EnRoute | PublicTransportFlags.AbandonRoute);
						if (currentRoute.m_Route != Entity.Null)
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentRoute>(jobIndex, vehicleEntity);
						}
					}
					else
					{
						cargoTransport.m_State &= ~CargoTransportFlags.RequiresMaintenance;
						publicTransport.m_State &= ~PublicTransportFlags.RequiresMaintenance;
						cargoTransport.m_State |= CargoTransportFlags.EnRoute;
						publicTransport.m_State |= PublicTransportFlags.EnRoute;
						if (isCargoVehicle)
						{
							nextStation = GetNextStorageCompany(currentRoute.m_Route, target.m_Target);
						}
					}
					cargoTransport.m_State |= CargoTransportFlags.RouteSource;
					publicTransport.m_State |= PublicTransportFlags.RouteSource;
					transportStationFromStop = Entity.Null;
					if (isCargoVehicle)
					{
						transportStationFromStop = GetStorageCompanyFromStop(connected.m_Connected);
					}
					m_BoardingData.BeginBoarding(vehicleEntity, currentRoute.m_Route, connected.m_Connected, target.m_Target, transportStationFromStop, nextStation, flag);
					return true;
				}
			}
			if (m_WaypointData.HasComponent(target.m_Target))
			{
				cargoTransport.m_State |= CargoTransportFlags.RouteSource;
				publicTransport.m_State |= PublicTransportFlags.RouteSource;
				return false;
			}
			cargoTransport.m_State &= ~(CargoTransportFlags.EnRoute | CargoTransportFlags.AbandonRoute);
			publicTransport.m_State &= ~(PublicTransportFlags.EnRoute | PublicTransportFlags.AbandonRoute);
			if (currentRoute.m_Route != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentRoute>(jobIndex, vehicleEntity);
			}
			return false;
		}

		private bool StopBoarding(Entity vehicleEntity, CurrentRoute currentRoute, DynamicBuffer<Passenger> passengers, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Target target, ref Odometer odometer, bool forcedStop)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			Connected connected = default(Connected);
			BoardingVehicle boardingVehicle = default(BoardingVehicle);
			if (m_ConnectedData.TryGetComponent(target.m_Target, ref connected) && m_BoardingVehicleData.TryGetComponent(connected.m_Connected, ref boardingVehicle))
			{
				flag = boardingVehicle.m_Vehicle == vehicleEntity;
			}
			if (!forcedStop)
			{
				publicTransport.m_MaxBoardingDistance = math.select(publicTransport.m_MinWaitingDistance + 1f, float.MaxValue, publicTransport.m_MinWaitingDistance == float.MaxValue || publicTransport.m_MinWaitingDistance == 0f);
				publicTransport.m_MinWaitingDistance = float.MaxValue;
				if ((flag || (publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport)) != 0) && (m_SimulationFrameIndex < cargoTransport.m_DepartureFrame || m_SimulationFrameIndex < publicTransport.m_DepartureFrame || publicTransport.m_MaxBoardingDistance != float.MaxValue))
				{
					return false;
				}
				if (passengers.IsCreated)
				{
					CurrentVehicle currentVehicle = default(CurrentVehicle);
					for (int i = 0; i < passengers.Length; i++)
					{
						Entity passenger = passengers[i].m_Passenger;
						if (m_CurrentVehicleData.TryGetComponent(passenger, ref currentVehicle) && (currentVehicle.m_Flags & CreatureVehicleFlags.Ready) == 0)
						{
							return false;
						}
					}
				}
			}
			if ((cargoTransport.m_State & CargoTransportFlags.Refueling) != 0 || (publicTransport.m_State & PublicTransportFlags.Refueling) != 0)
			{
				odometer.m_Distance = 0f;
			}
			if ((publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport)) == 0 && flag)
			{
				Entity currentStation = Entity.Null;
				Entity nextStation = Entity.Null;
				if (!forcedStop && (cargoTransport.m_State & CargoTransportFlags.Boarding) != 0)
				{
					currentStation = GetStorageCompanyFromStop(connected.m_Connected);
					if ((cargoTransport.m_State & CargoTransportFlags.EnRoute) != 0)
					{
						nextStation = GetNextStorageCompany(currentRoute.m_Route, target.m_Target);
					}
				}
				m_BoardingData.EndBoarding(vehicleEntity, currentRoute.m_Route, connected.m_Connected, target.m_Target, currentStation, nextStation);
				return true;
			}
			cargoTransport.m_State &= ~(CargoTransportFlags.Boarding | CargoTransportFlags.Refueling);
			publicTransport.m_State &= ~(PublicTransportFlags.Boarding | PublicTransportFlags.Refueling);
			return true;
		}

		private Entity GetTransportStationFromStop(Entity stop)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			while (true)
			{
				if (m_TransportStationData.HasComponent(stop))
				{
					if (m_OwnerData.HasComponent(stop))
					{
						Entity owner = m_OwnerData[stop].m_Owner;
						if (m_TransportStationData.HasComponent(owner))
						{
							return owner;
						}
					}
					return stop;
				}
				if (!m_OwnerData.HasComponent(stop))
				{
					break;
				}
				stop = m_OwnerData[stop].m_Owner;
			}
			return Entity.Null;
		}

		private Entity GetStorageCompanyFromStop(Entity stop)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			while (true)
			{
				if (m_StorageCompanyData.HasComponent(stop))
				{
					return stop;
				}
				if (!m_OwnerData.HasComponent(stop))
				{
					break;
				}
				stop = m_OwnerData[stop].m_Owner;
			}
			return Entity.Null;
		}

		private Entity GetNextStorageCompany(Entity route, Entity currentWaypoint)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<RouteWaypoint> val = m_RouteWaypoints[route];
			int num = m_WaypointData[currentWaypoint].m_Index + 1;
			for (int i = 0; i < val.Length; i++)
			{
				num = math.select(num, 0, num >= val.Length);
				Entity waypoint = val[num].m_Waypoint;
				if (m_ConnectedData.HasComponent(waypoint))
				{
					Entity connected = m_ConnectedData[waypoint].m_Connected;
					Entity storageCompanyFromStop = GetStorageCompanyFromStop(connected);
					if (storageCompanyFromStop != Entity.Null)
					{
						return storageCompanyFromStop;
					}
				}
				num++;
			}
			return Entity.Null;
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
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> __Game_Objects_Unspawned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentRoute> __Game_Routes_CurrentRoute_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Passenger> __Game_Vehicles_Passenger_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Vehicles.CargoTransport> __Game_Vehicles_CargoTransport_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Car> __Game_Vehicles_Car_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Odometer> __Game_Vehicles_Odometer_RW_ComponentTypeHandle;

		public BufferTypeHandle<CarNavigationLane> __Game_Vehicles_CarNavigationLane_RW_BufferTypeHandle;

		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> __Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> __Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportVehicleRequest> __Game_Simulation_TransportVehicleRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EvacuationRequest> __Game_Simulation_EvacuationRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrisonerTransportRequest> __Game_Simulation_PrisonerTransportRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Waypoint> __Game_Routes_Waypoint_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BoardingVehicle> __Game_Routes_BoardingVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteLane> __Game_Routes_RouteLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.Color> __Game_Routes_Color_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Companies.StorageCompany> __Game_Companies_StorageCompany_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TransportStation> __Game_Buildings_TransportStation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TransportDepot> __Game_Buildings_TransportDepot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Prison> __Game_Buildings_Prison_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.EmergencyShelter> __Game_Buildings_EmergencyShelter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarageLane> __Game_Net_GarageLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferLookup;

		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RW_BufferLookup;

		public BufferLookup<LoadingResources> __Game_Vehicles_LoadingResources_RW_BufferLookup;

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
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Routes_CurrentRoute_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentRoute>(true);
			__Game_Vehicles_Passenger_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Passenger>(true);
			__Game_Vehicles_CargoTransport_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.CargoTransport>(false);
			__Game_Vehicles_PublicTransport_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.PublicTransport>(false);
			__Game_Vehicles_Car_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Car>(false);
			__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarCurrentLane>(false);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Vehicles_Odometer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Odometer>(false);
			__Game_Vehicles_CarNavigationLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CarNavigationLane>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PublicTransportVehicleData>(true);
			__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CargoTransportVehicleData>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Simulation_ServiceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(true);
			__Game_Simulation_TransportVehicleRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportVehicleRequest>(true);
			__Game_Simulation_EvacuationRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EvacuationRequest>(true);
			__Game_Simulation_PrisonerTransportRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrisonerTransportRequest>(true);
			__Game_Routes_Waypoint_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Waypoint>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Routes_BoardingVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BoardingVehicle>(true);
			__Game_Routes_RouteLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteLane>(true);
			__Game_Routes_Color_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.Color>(true);
			__Game_Companies_StorageCompany_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Companies.StorageCompany>(true);
			__Game_Buildings_TransportStation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.TransportStation>(true);
			__Game_Buildings_TransportDepot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.TransportDepot>(true);
			__Game_Buildings_Prison_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Prison>(true);
			__Game_Buildings_EmergencyShelter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.EmergencyShelter>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_GarageLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarageLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
			__Game_Vehicles_LoadingResources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LoadingResources>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private AchievementTriggerSystem m_AchievementTriggerSystem;

	private EntityQuery m_VehicleQuery;

	private EntityArchetype m_TransportVehicleRequestArchetype;

	private EntityArchetype m_EvacuationRequestArchetype;

	private EntityArchetype m_PrisonerTransportRequestArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_MovingToParkedCarRemoveTypes;

	private ComponentTypeSet m_MovingToParkedAddTypes;

	private TransportBoardingHelpers.BoardingLookupData m_BoardingLookupData;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 1;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_AchievementTriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AchievementTriggerSystem>();
		m_BoardingLookupData = new TransportBoardingHelpers.BoardingLookupData((SystemBase)(object)this);
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Vehicles.CargoTransport>(),
			ComponentType.ReadWrite<Game.Vehicles.PublicTransport>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<TripSource>(),
			ComponentType.ReadOnly<OutOfControl>()
		};
		array[0] = val;
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_TransportVehicleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<TransportVehicleRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EvacuationRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<EvacuationRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_PrisonerTransportRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<PrisonerTransportRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_HandleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<HandleRequest>(),
			ComponentType.ReadWrite<Event>()
		});
		m_MovingToParkedCarRemoveTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[13]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<Swaying>(),
			ComponentType.ReadWrite<CarNavigation>(),
			ComponentType.ReadWrite<CarNavigationLane>(),
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<PathElement>(),
			ComponentType.ReadWrite<PathInformation>(),
			ComponentType.ReadWrite<ServiceDispatch>()
		});
		m_MovingToParkedAddTypes = new ComponentTypeSet(ComponentType.ReadWrite<ParkedCar>(), ComponentType.ReadWrite<Stopped>(), ComponentType.ReadWrite<Updated>());
		((ComponentSystemBase)this).RequireForUpdate(m_VehicleQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		TransportBoardingHelpers.BoardingData boardingData = new TransportBoardingHelpers.BoardingData((Allocator)3);
		m_BoardingLookupData.Update((SystemBase)(object)this);
		TransportCarTickJob transportCarTickJob = new TransportCarTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentRouteType = InternalCompilerInterface.GetComponentTypeHandle<CurrentRoute>(ref __TypeHandle.__Game_Routes_CurrentRoute_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PassengerType = InternalCompilerInterface.GetBufferTypeHandle<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CargoTransportType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.CargoTransport>(ref __TypeHandle.__Game_Vehicles_CargoTransport_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarType = InternalCompilerInterface.GetComponentTypeHandle<Car>(ref __TypeHandle.__Game_Vehicles_Car_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OdometerType = InternalCompilerInterface.GetComponentTypeHandle<Odometer>(ref __TypeHandle.__Game_Vehicles_Odometer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<CarNavigationLane>(ref __TypeHandle.__Game_Vehicles_CarNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatchType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportVehicleData = InternalCompilerInterface.GetComponentLookup<PublicTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CargoTransportVehicleData = InternalCompilerInterface.GetComponentLookup<CargoTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportVehicleRequestData = InternalCompilerInterface.GetComponentLookup<TransportVehicleRequest>(ref __TypeHandle.__Game_Simulation_TransportVehicleRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EvacuationRequestData = InternalCompilerInterface.GetComponentLookup<EvacuationRequest>(ref __TypeHandle.__Game_Simulation_EvacuationRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrisonerTransportRequestData = InternalCompilerInterface.GetComponentLookup<PrisonerTransportRequest>(ref __TypeHandle.__Game_Simulation_PrisonerTransportRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaypointData = InternalCompilerInterface.GetComponentLookup<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoardingVehicleData = InternalCompilerInterface.GetComponentLookup<BoardingVehicle>(ref __TypeHandle.__Game_Routes_BoardingVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteLaneData = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteColorData = InternalCompilerInterface.GetComponentLookup<Game.Routes.Color>(ref __TypeHandle.__Game_Routes_Color_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanyData = InternalCompilerInterface.GetComponentLookup<Game.Companies.StorageCompany>(ref __TypeHandle.__Game_Companies_StorageCompany_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStationData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.TransportStation>(ref __TypeHandle.__Game_Buildings_TransportStation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportDepotData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.TransportDepot>(ref __TypeHandle.__Game_Buildings_TransportDepot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrisonData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Prison>(ref __TypeHandle.__Game_Buildings_Prison_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EmergencyShelterData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.EmergencyShelter>(ref __TypeHandle.__Game_Buildings_EmergencyShelter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarageLaneData = InternalCompilerInterface.GetComponentLookup<GarageLane>(ref __TypeHandle.__Game_Net_GarageLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LoadingResources = InternalCompilerInterface.GetBufferLookup<LoadingResources>(ref __TypeHandle.__Game_Vehicles_LoadingResources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex,
			m_RandomSeed = RandomSeed.Next(),
			m_TransportVehicleRequestArchetype = m_TransportVehicleRequestArchetype,
			m_EvacuationRequestArchetype = m_EvacuationRequestArchetype,
			m_PrisonerTransportRequestArchetype = m_PrisonerTransportRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_MovingToParkedCarRemoveTypes = m_MovingToParkedCarRemoveTypes,
			m_MovingToParkedAddTypes = m_MovingToParkedAddTypes
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		transportCarTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		transportCarTickJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		transportCarTickJob.m_BoardingData = boardingData.ToConcurrent();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<TransportCarTickJob>(transportCarTickJob, m_VehicleQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = boardingData.ScheduleBoarding((SystemBase)(object)this, m_CityStatisticsSystem, m_AchievementTriggerSystem, m_BoardingLookupData, m_SimulationSystem.frameIndex, val2);
		boardingData.Dispose(val3);
		m_PathfindSetupSystem.AddQueueWriter(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val3;
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
	public TransportCarAISystem()
	{
	}
}

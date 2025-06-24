using System.Runtime.CompilerServices;
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
public class TransportWatercraftAISystem : GameSystemBase
{
	[BurstCompile]
	private struct TransportWatercraftTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentRoute> m_CurrentRouteType;

		[ReadOnly]
		public BufferTypeHandle<Passenger> m_PassengerType;

		public ComponentTypeHandle<Game.Vehicles.CargoTransport> m_CargoTransportType;

		public ComponentTypeHandle<Game.Vehicles.PublicTransport> m_PublicTransportType;

		public ComponentTypeHandle<Watercraft> m_WatercraftType;

		public ComponentTypeHandle<WatercraftCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<Target> m_TargetType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public ComponentTypeHandle<Odometer> m_OdometerType;

		public BufferTypeHandle<WatercraftNavigationLane> m_WatercraftNavigationLaneType;

		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<TransportVehicleRequest> m_TransportVehicleRequestData;

		[ReadOnly]
		public ComponentLookup<WatercraftData> m_PrefabWatercraftData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> m_PublicTransportVehicleData;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> m_CargoTransportVehicleData;

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
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[NativeDisableParallelForRestriction]
		public BufferLookup<PathElement> m_PathElements;

		[NativeDisableParallelForRestriction]
		public BufferLookup<LoadingResources> m_LoadingResources;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		[ReadOnly]
		public EntityArchetype m_TransportVehicleRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

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
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<CurrentRoute> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentRoute>(ref m_CurrentRouteType);
			NativeArray<WatercraftCurrentLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftCurrentLane>(ref m_CurrentLaneType);
			NativeArray<Game.Vehicles.CargoTransport> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.CargoTransport>(ref m_CargoTransportType);
			NativeArray<Game.Vehicles.PublicTransport> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.PublicTransport>(ref m_PublicTransportType);
			NativeArray<Watercraft> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Watercraft>(ref m_WatercraftType);
			NativeArray<Target> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PathOwner> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<Odometer> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Odometer>(ref m_OdometerType);
			BufferAccessor<WatercraftNavigationLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<WatercraftNavigationLane>(ref m_WatercraftNavigationLaneType);
			BufferAccessor<Passenger> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Passenger>(ref m_PassengerType);
			BufferAccessor<ServiceDispatch> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Owner owner = nativeArray2[i];
				PrefabRef prefabRef = nativeArray3[i];
				Watercraft watercraft = nativeArray8[i];
				WatercraftCurrentLane currentLane = nativeArray5[i];
				PathOwner pathOwner = nativeArray10[i];
				Target target = nativeArray9[i];
				Odometer odometer = nativeArray11[i];
				DynamicBuffer<WatercraftNavigationLane> navigationLanes = bufferAccessor[i];
				DynamicBuffer<ServiceDispatch> serviceDispatches = bufferAccessor3[i];
				CurrentRoute currentRoute = default(CurrentRoute);
				if (nativeArray4.Length != 0)
				{
					currentRoute = nativeArray4[i];
				}
				Game.Vehicles.CargoTransport cargoTransport = default(Game.Vehicles.CargoTransport);
				if (nativeArray6.Length != 0)
				{
					cargoTransport = nativeArray6[i];
				}
				Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
				DynamicBuffer<Passenger> passengers = default(DynamicBuffer<Passenger>);
				if (nativeArray7.Length != 0)
				{
					publicTransport = nativeArray7[i];
					passengers = bufferAccessor2[i];
				}
				VehicleUtils.CheckUnspawned(unfilteredChunkIndex, val, currentLane, isUnspawned, m_CommandBuffer);
				Tick(unfilteredChunkIndex, val, owner, prefabRef, currentRoute, navigationLanes, passengers, serviceDispatches, ref cargoTransport, ref publicTransport, ref watercraft, ref currentLane, ref pathOwner, ref target, ref odometer);
				nativeArray8[i] = watercraft;
				nativeArray5[i] = currentLane;
				nativeArray10[i] = pathOwner;
				nativeArray9[i] = target;
				nativeArray11[i] = odometer;
				if (nativeArray6.Length != 0)
				{
					nativeArray6[i] = cargoTransport;
				}
				if (nativeArray7.Length != 0)
				{
					nativeArray7[i] = publicTransport;
				}
			}
		}

		private void Tick(int jobIndex, Entity vehicleEntity, Owner owner, PrefabRef prefabRef, CurrentRoute currentRoute, DynamicBuffer<WatercraftNavigationLane> navigationLanes, DynamicBuffer<Passenger> passengers, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Watercraft watercraft, ref WatercraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, ref Odometer odometer)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			if (VehicleUtils.ResetUpdatedPath(ref pathOwner))
			{
				ResetPath(vehicleEntity, ref cargoTransport, ref publicTransport, ref watercraft, ref currentLane, ref pathOwner);
				DynamicBuffer<LoadingResources> loadingResources = default(DynamicBuffer<LoadingResources>);
				if (((publicTransport.m_State & PublicTransportFlags.DummyTraffic) != 0 || (cargoTransport.m_State & CargoTransportFlags.DummyTraffic) != 0) && m_LoadingResources.TryGetBuffer(vehicleEntity, ref loadingResources))
				{
					CheckDummyResources(jobIndex, vehicleEntity, prefabRef, loadingResources);
				}
			}
			bool num = (cargoTransport.m_State & CargoTransportFlags.EnRoute) == 0 && (publicTransport.m_State & PublicTransportFlags.EnRoute) == 0;
			if (m_PublicTransportVehicleData.HasComponent(prefabRef.m_Prefab))
			{
				PublicTransportVehicleData publicTransportVehicleData = m_PublicTransportVehicleData[prefabRef.m_Prefab];
				if (odometer.m_Distance >= publicTransportVehicleData.m_MaintenanceRange && publicTransportVehicleData.m_MaintenanceRange > 0.1f && (publicTransport.m_State & PublicTransportFlags.Refueling) == 0)
				{
					publicTransport.m_State |= PublicTransportFlags.RequiresMaintenance;
				}
			}
			bool isCargoVehicle = false;
			if (m_CargoTransportVehicleData.HasComponent(prefabRef.m_Prefab))
			{
				CargoTransportVehicleData cargoTransportVehicleData = m_CargoTransportVehicleData[prefabRef.m_Prefab];
				if (odometer.m_Distance >= cargoTransportVehicleData.m_MaintenanceRange && cargoTransportVehicleData.m_MaintenanceRange > 0.1f && (cargoTransport.m_State & CargoTransportFlags.Refueling) == 0)
				{
					cargoTransport.m_State |= CargoTransportFlags.RequiresMaintenance;
				}
				isCargoVehicle = true;
			}
			watercraft.m_Flags |= WatercraftFlags.DeckLights;
			if (num)
			{
				CheckServiceDispatches(vehicleEntity, serviceDispatches, ref cargoTransport, ref publicTransport);
				if (serviceDispatches.Length == 0 && (cargoTransport.m_State & (CargoTransportFlags.RequiresMaintenance | CargoTransportFlags.DummyTraffic | CargoTransportFlags.Disabled)) == 0 && (publicTransport.m_State & (PublicTransportFlags.RequiresMaintenance | PublicTransportFlags.DummyTraffic | PublicTransportFlags.Disabled)) == 0)
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
			if (!m_PrefabRefData.HasComponent(target.m_Target) || VehicleUtils.PathfindFailed(pathOwner))
			{
				if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
				{
					StopBoarding(jobIndex, vehicleEntity, currentRoute, passengers, ref cargoTransport, ref publicTransport, ref target, ref odometer, isCargoVehicle, forcedStop: true);
				}
				if (VehicleUtils.IsStuck(pathOwner) || (cargoTransport.m_State & (CargoTransportFlags.Returning | CargoTransportFlags.DummyTraffic)) != 0 || (publicTransport.m_State & (PublicTransportFlags.Returning | PublicTransportFlags.DummyTraffic)) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
					return;
				}
				ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref pathOwner, ref target);
			}
			else if (VehicleUtils.PathEndReached(currentLane))
			{
				if ((cargoTransport.m_State & (CargoTransportFlags.Returning | CargoTransportFlags.DummyTraffic)) != 0 || (publicTransport.m_State & (PublicTransportFlags.Returning | PublicTransportFlags.DummyTraffic)) != 0)
				{
					if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
					{
						if (StopBoarding(jobIndex, vehicleEntity, currentRoute, passengers, ref cargoTransport, ref publicTransport, ref target, ref odometer, isCargoVehicle, forcedStop: false) && !SelectNextDispatch(jobIndex, vehicleEntity, currentRoute, navigationLanes, serviceDispatches, ref cargoTransport, ref publicTransport, ref watercraft, ref currentLane, ref pathOwner, ref target))
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
							return;
						}
					}
					else if ((!passengers.IsCreated || passengers.Length <= 0 || !StartBoarding(jobIndex, vehicleEntity, currentRoute, prefabRef, ref cargoTransport, ref publicTransport, ref target, isCargoVehicle)) && !SelectNextDispatch(jobIndex, vehicleEntity, currentRoute, navigationLanes, serviceDispatches, ref cargoTransport, ref publicTransport, ref watercraft, ref currentLane, ref pathOwner, ref target))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
						return;
					}
				}
				else if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
				{
					if (StopBoarding(jobIndex, vehicleEntity, currentRoute, passengers, ref cargoTransport, ref publicTransport, ref target, ref odometer, isCargoVehicle, forcedStop: false))
					{
						if ((cargoTransport.m_State & CargoTransportFlags.EnRoute) == 0 && (publicTransport.m_State & PublicTransportFlags.EnRoute) == 0)
						{
							ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref pathOwner, ref target);
						}
						else
						{
							SetNextWaypointTarget(currentRoute, ref pathOwner, ref target);
						}
					}
				}
				else if (!m_RouteWaypoints.HasBuffer(currentRoute.m_Route) || !m_WaypointData.HasComponent(target.m_Target))
				{
					ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref pathOwner, ref target);
				}
				else if (!StartBoarding(jobIndex, vehicleEntity, currentRoute, prefabRef, ref cargoTransport, ref publicTransport, ref target, isCargoVehicle))
				{
					if ((cargoTransport.m_State & CargoTransportFlags.EnRoute) == 0 && (publicTransport.m_State & PublicTransportFlags.EnRoute) == 0)
					{
						ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref pathOwner, ref target);
					}
					else
					{
						SetNextWaypointTarget(currentRoute, ref pathOwner, ref target);
					}
				}
			}
			else if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
			{
				StopBoarding(jobIndex, vehicleEntity, currentRoute, passengers, ref cargoTransport, ref publicTransport, ref target, ref odometer, isCargoVehicle, forcedStop: true);
			}
			Entity skipWaypoint = Entity.Null;
			if ((cargoTransport.m_State & CargoTransportFlags.Boarding) == 0 && (publicTransport.m_State & PublicTransportFlags.Boarding) == 0)
			{
				if ((cargoTransport.m_State & CargoTransportFlags.Returning) != 0 || (publicTransport.m_State & PublicTransportFlags.Returning) != 0)
				{
					if (!passengers.IsCreated || passengers.Length == 0)
					{
						SelectNextDispatch(jobIndex, vehicleEntity, currentRoute, navigationLanes, serviceDispatches, ref cargoTransport, ref publicTransport, ref watercraft, ref currentLane, ref pathOwner, ref target);
					}
				}
				else if ((cargoTransport.m_State & CargoTransportFlags.Arriving) == 0 && (publicTransport.m_State & PublicTransportFlags.Arriving) == 0)
				{
					CheckNavigationLanes(currentRoute, navigationLanes, ref cargoTransport, ref publicTransport, ref currentLane, ref pathOwner, ref target, out skipWaypoint);
				}
			}
			FindPathIfNeeded(vehicleEntity, prefabRef, skipWaypoint, ref currentLane, ref cargoTransport, ref publicTransport, ref pathOwner, ref target);
		}

		private void FindPathIfNeeded(Entity vehicleEntity, PrefabRef prefabRef, Entity skipWaypoint, ref WatercraftCurrentLane currentLane, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			if (VehicleUtils.RequireNewPath(pathOwner))
			{
				WatercraftData watercraftData = m_PrefabWatercraftData[prefabRef.m_Prefab];
				PathfindParameters parameters = new PathfindParameters
				{
					m_MaxSpeed = float2.op_Implicit(watercraftData.m_MaxSpeed),
					m_WalkSpeed = float2.op_Implicit(5.555556f),
					m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
					m_Methods = VehicleUtils.GetPathMethods(watercraftData),
					m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
				};
				SetupQueueTarget origin = new SetupQueueTarget
				{
					m_Type = SetupTargetType.CurrentLocation,
					m_Methods = VehicleUtils.GetPathMethods(watercraftData),
					m_RoadTypes = RoadTypes.Watercraft
				};
				SetupQueueTarget destination = new SetupQueueTarget
				{
					m_Type = SetupTargetType.CurrentLocation,
					m_Methods = VehicleUtils.GetPathMethods(watercraftData),
					m_RoadTypes = RoadTypes.Watercraft,
					m_Entity = target.m_Target
				};
				if (skipWaypoint != Entity.Null)
				{
					origin.m_Entity = skipWaypoint;
					pathOwner.m_State |= PathFlags.Append;
				}
				else
				{
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
				VehicleUtils.SetupPathfind(item: new SetupQueueItem(vehicleEntity, parameters, origin, destination), currentLane: ref currentLane, pathOwner: ref pathOwner, queue: m_PathfindQueue);
			}
		}

		private void CheckNavigationLanes(CurrentRoute currentRoute, DynamicBuffer<WatercraftNavigationLane> navigationLanes, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref WatercraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, out Entity skipWaypoint)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			skipWaypoint = Entity.Null;
			if (navigationLanes.Length == 0 || navigationLanes.Length == 8)
			{
				return;
			}
			WatercraftNavigationLane watercraftNavigationLane = navigationLanes[navigationLanes.Length - 1];
			if ((watercraftNavigationLane.m_Flags & WatercraftLaneFlags.EndOfPath) == 0)
			{
				return;
			}
			if (m_WaypointData.HasComponent(target.m_Target) && m_RouteWaypoints.HasBuffer(currentRoute.m_Route) && (!m_ConnectedData.HasComponent(target.m_Target) || !m_BoardingVehicleData.HasComponent(m_ConnectedData[target.m_Target].m_Connected)))
			{
				if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete)) == 0)
				{
					skipWaypoint = target.m_Target;
					SetNextWaypointTarget(currentRoute, ref pathOwner, ref target);
					if ((watercraftNavigationLane.m_Flags & WatercraftLaneFlags.GroupTarget) != 0)
					{
						navigationLanes.RemoveAt(navigationLanes.Length - 1);
					}
					else
					{
						watercraftNavigationLane.m_Flags &= ~WatercraftLaneFlags.EndOfPath;
						navigationLanes[navigationLanes.Length - 1] = watercraftNavigationLane;
					}
					cargoTransport.m_State |= CargoTransportFlags.RouteSource;
					publicTransport.m_State |= PublicTransportFlags.RouteSource;
				}
				return;
			}
			cargoTransport.m_State |= CargoTransportFlags.Arriving;
			publicTransport.m_State |= PublicTransportFlags.Arriving;
			RouteLane routeLane = default(RouteLane);
			if (!m_RouteLaneData.TryGetComponent(target.m_Target, ref routeLane))
			{
				return;
			}
			if (routeLane.m_StartLane != routeLane.m_EndLane)
			{
				watercraftNavigationLane.m_CurvePosition.y = 1f;
				WatercraftNavigationLane watercraftNavigationLane2 = new WatercraftNavigationLane
				{
					m_Lane = watercraftNavigationLane.m_Lane
				};
				if (FindNextLane(ref watercraftNavigationLane2.m_Lane))
				{
					watercraftNavigationLane.m_Flags &= ~WatercraftLaneFlags.EndOfPath;
					navigationLanes[navigationLanes.Length - 1] = watercraftNavigationLane;
					watercraftNavigationLane2.m_Flags |= WatercraftLaneFlags.EndOfPath | WatercraftLaneFlags.FixedLane;
					watercraftNavigationLane2.m_CurvePosition = new float2(0f, routeLane.m_EndCurvePos);
					navigationLanes.Add(watercraftNavigationLane2);
				}
				else
				{
					navigationLanes[navigationLanes.Length - 1] = watercraftNavigationLane;
				}
			}
			else
			{
				watercraftNavigationLane.m_CurvePosition.y = routeLane.m_EndCurvePos;
				navigationLanes[navigationLanes.Length - 1] = watercraftNavigationLane;
			}
		}

		private bool FindNextLane(ref Entity lane)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			if (!m_OwnerData.HasComponent(lane) || !m_LaneData.HasComponent(lane))
			{
				return false;
			}
			Owner owner = m_OwnerData[lane];
			Lane lane2 = m_LaneData[lane];
			if (!m_SubLanes.HasBuffer(owner.m_Owner))
			{
				return false;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[owner.m_Owner];
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				Lane lane3 = m_LaneData[subLane];
				if (lane2.m_EndNode.Equals(lane3.m_StartNode))
				{
					lane = subLane;
					return true;
				}
			}
			return false;
		}

		private void ResetPath(Entity vehicleEntity, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Watercraft watercraft, ref WatercraftCurrentLane currentLane, ref PathOwner pathOwner)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			cargoTransport.m_State &= ~CargoTransportFlags.Arriving;
			publicTransport.m_State &= ~PublicTransportFlags.Arriving;
			if ((pathOwner.m_State & PathFlags.Append) == 0)
			{
				DynamicBuffer<PathElement> path = m_PathElements[vehicleEntity];
				PathUtils.ResetPath(ref currentLane, path, m_SlaveLaneData, m_OwnerData, m_SubLanes);
			}
			if ((cargoTransport.m_State & (CargoTransportFlags.Returning | CargoTransportFlags.DummyTraffic)) != 0 || (publicTransport.m_State & (PublicTransportFlags.Returning | PublicTransportFlags.DummyTraffic)) != 0)
			{
				watercraft.m_Flags &= ~WatercraftFlags.StayOnWaterway;
			}
			else
			{
				watercraft.m_Flags |= WatercraftFlags.StayOnWaterway;
			}
		}

		private void CheckDummyResources(int jobIndex, Entity vehicleEntity, PrefabRef prefabRef, DynamicBuffer<LoadingResources> loadingResources)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
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
			QuantityUpdated(jobIndex, vehicleEntity);
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

		private void CheckServiceDispatches(Entity vehicleEntity, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			if (serviceDispatches.Length > 1)
			{
				serviceDispatches.RemoveRange(1, serviceDispatches.Length - 1);
			}
			cargoTransport.m_RequestCount = math.min(1, cargoTransport.m_RequestCount);
			publicTransport.m_RequestCount = math.min(1, publicTransport.m_RequestCount);
			int num = cargoTransport.m_RequestCount + publicTransport.m_RequestCount;
			if (serviceDispatches.Length <= num)
			{
				return;
			}
			float num2 = -1f;
			Entity val = Entity.Null;
			for (int i = num; i < serviceDispatches.Length; i++)
			{
				Entity request = serviceDispatches[i].m_Request;
				if (m_TransportVehicleRequestData.HasComponent(request))
				{
					TransportVehicleRequest transportVehicleRequest = m_TransportVehicleRequestData[request];
					if (m_PrefabRefData.HasComponent(transportVehicleRequest.m_Route) && transportVehicleRequest.m_Priority > num2)
					{
						num2 = transportVehicleRequest.m_Priority;
						val = request;
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
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			if (!m_TransportVehicleRequestData.HasComponent(publicTransport.m_TargetRequest) && !m_TransportVehicleRequestData.HasComponent(cargoTransport.m_TargetRequest))
			{
				uint num = math.max(256u, 16u);
				if ((m_SimulationFrameIndex & (num - 1)) == 8)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_TransportVehicleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TransportVehicleRequest>(jobIndex, val, new TransportVehicleRequest(entity, 1f));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(8u));
				}
			}
		}

		private bool SelectNextDispatch(int jobIndex, Entity vehicleEntity, CurrentRoute currentRoute, DynamicBuffer<WatercraftNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Watercraft watercraft, ref WatercraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
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
				WatercraftFlags watercraftFlags = watercraft.m_Flags;
				if (m_TransportVehicleRequestData.HasComponent(request))
				{
					val = m_TransportVehicleRequestData[request].m_Route;
					if (m_PathInformationData.HasComponent(request))
					{
						val2 = m_PathInformationData[request].m_Destination;
					}
					watercraftFlags |= WatercraftFlags.StayOnWaterway;
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
				watercraft.m_Flags = watercraftFlags;
				if (m_TransportVehicleRequestData.HasComponent(publicTransport.m_TargetRequest))
				{
					Entity val5 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val5, new HandleRequest(publicTransport.m_TargetRequest, Entity.Null, completed: true));
				}
				if (m_TransportVehicleRequestData.HasComponent(cargoTransport.m_TargetRequest))
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

		private void ReturnToDepot(int jobIndex, Entity vehicleEntity, CurrentRoute currentRoute, Owner owner, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			serviceDispatches.Clear();
			cargoTransport.m_RequestCount = 0;
			cargoTransport.m_State &= ~(CargoTransportFlags.EnRoute | CargoTransportFlags.Refueling | CargoTransportFlags.AbandonRoute);
			cargoTransport.m_State |= CargoTransportFlags.Returning;
			publicTransport.m_RequestCount = 0;
			publicTransport.m_State &= ~(PublicTransportFlags.EnRoute | PublicTransportFlags.Refueling | PublicTransportFlags.AbandonRoute);
			publicTransport.m_State |= PublicTransportFlags.Returning;
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentRoute>(jobIndex, vehicleEntity);
			VehicleUtils.SetTarget(ref pathOwner, ref target, owner.m_Owner);
		}

		private bool StartBoarding(int jobIndex, Entity vehicleEntity, CurrentRoute currentRoute, PrefabRef prefabRef, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Target target, bool isCargoVehicle)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
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
						WatercraftData watercraftData = m_PrefabWatercraftData[prefabRef.m_Prefab];
						Game.Buildings.TransportStation transportStation = m_TransportStationData[transportStationFromStop];
						flag = (transportStation.m_WatercraftRefuelTypes & watercraftData.m_EnergyType) != 0;
						if ((transportStation.m_Flags & TransportStationFlags.TransportStopsActive) != TransportStationFlags.TransportStopsActive)
						{
							return false;
						}
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

		private bool StopBoarding(int jobIndex, Entity vehicleEntity, CurrentRoute currentRoute, DynamicBuffer<Passenger> passengers, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Target target, ref Odometer odometer, bool isCargoVehicle, bool forcedStop)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
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
				if (flag && (m_SimulationFrameIndex < cargoTransport.m_DepartureFrame || m_SimulationFrameIndex < publicTransport.m_DepartureFrame || publicTransport.m_MaxBoardingDistance != float.MaxValue))
				{
					return false;
				}
				if (passengers.IsCreated)
				{
					for (int i = 0; i < passengers.Length; i++)
					{
						Entity passenger = passengers[i].m_Passenger;
						if (m_CurrentVehicleData.HasComponent(passenger) && (m_CurrentVehicleData[passenger].m_Flags & CreatureVehicleFlags.Ready) == 0)
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
			if (isCargoVehicle)
			{
				QuantityUpdated(jobIndex, vehicleEntity);
			}
			else
			{
				PassengersUpdated(jobIndex, vehicleEntity);
			}
			if (flag)
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

		private void QuantityUpdated(int jobIndex, Entity vehicleEntity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, vehicleEntity, default(Updated));
		}

		private void PassengersUpdated(int jobIndex, Entity vehicleEntity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, vehicleEntity, default(BatchesUpdated));
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentRoute> __Game_Routes_CurrentRoute_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Passenger> __Game_Vehicles_Passenger_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Vehicles.CargoTransport> __Game_Vehicles_CargoTransport_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Watercraft> __Game_Vehicles_Watercraft_RW_ComponentTypeHandle;

		public ComponentTypeHandle<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Odometer> __Game_Vehicles_Odometer_RW_ComponentTypeHandle;

		public BufferTypeHandle<WatercraftNavigationLane> __Game_Vehicles_WatercraftNavigationLane_RW_BufferTypeHandle;

		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportVehicleRequest> __Game_Simulation_TransportVehicleRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WatercraftData> __Game_Prefabs_WatercraftData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> __Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> __Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup;

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
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

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
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Routes_CurrentRoute_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentRoute>(true);
			__Game_Vehicles_Passenger_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Passenger>(true);
			__Game_Vehicles_CargoTransport_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.CargoTransport>(false);
			__Game_Vehicles_PublicTransport_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.PublicTransport>(false);
			__Game_Vehicles_Watercraft_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Watercraft>(false);
			__Game_Vehicles_WatercraftCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WatercraftCurrentLane>(false);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Vehicles_Odometer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Odometer>(false);
			__Game_Vehicles_WatercraftNavigationLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<WatercraftNavigationLane>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Simulation_TransportVehicleRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportVehicleRequest>(true);
			__Game_Prefabs_WatercraftData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PublicTransportVehicleData>(true);
			__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CargoTransportVehicleData>(true);
			__Game_Routes_Waypoint_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Waypoint>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Routes_BoardingVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BoardingVehicle>(true);
			__Game_Routes_RouteLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteLane>(true);
			__Game_Routes_Color_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.Color>(true);
			__Game_Companies_StorageCompany_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Companies.StorageCompany>(true);
			__Game_Buildings_TransportStation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.TransportStation>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
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

	private EntityArchetype m_HandleRequestArchetype;

	private TransportBoardingHelpers.BoardingLookupData m_BoardingLookupData;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 8;
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
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
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
			ComponentType.ReadWrite<WatercraftCurrentLane>(),
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
		m_HandleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<HandleRequest>(),
			ComponentType.ReadWrite<Event>()
		});
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
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		TransportBoardingHelpers.BoardingData boardingData = new TransportBoardingHelpers.BoardingData((Allocator)3);
		m_BoardingLookupData.Update((SystemBase)(object)this);
		TransportWatercraftTickJob transportWatercraftTickJob = new TransportWatercraftTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentRouteType = InternalCompilerInterface.GetComponentTypeHandle<CurrentRoute>(ref __TypeHandle.__Game_Routes_CurrentRoute_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PassengerType = InternalCompilerInterface.GetBufferTypeHandle<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CargoTransportType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.CargoTransport>(ref __TypeHandle.__Game_Vehicles_CargoTransport_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftType = InternalCompilerInterface.GetComponentTypeHandle<Watercraft>(ref __TypeHandle.__Game_Vehicles_Watercraft_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OdometerType = InternalCompilerInterface.GetComponentTypeHandle<Odometer>(ref __TypeHandle.__Game_Vehicles_Odometer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<WatercraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_WatercraftNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatchType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportVehicleRequestData = InternalCompilerInterface.GetComponentLookup<TransportVehicleRequest>(ref __TypeHandle.__Game_Simulation_TransportVehicleRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWatercraftData = InternalCompilerInterface.GetComponentLookup<WatercraftData>(ref __TypeHandle.__Game_Prefabs_WatercraftData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportVehicleData = InternalCompilerInterface.GetComponentLookup<PublicTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CargoTransportVehicleData = InternalCompilerInterface.GetComponentLookup<CargoTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaypointData = InternalCompilerInterface.GetComponentLookup<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoardingVehicleData = InternalCompilerInterface.GetComponentLookup<BoardingVehicle>(ref __TypeHandle.__Game_Routes_BoardingVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteLaneData = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteColorData = InternalCompilerInterface.GetComponentLookup<Game.Routes.Color>(ref __TypeHandle.__Game_Routes_Color_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanyData = InternalCompilerInterface.GetComponentLookup<Game.Companies.StorageCompany>(ref __TypeHandle.__Game_Companies_StorageCompany_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStationData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.TransportStation>(ref __TypeHandle.__Game_Buildings_TransportStation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LoadingResources = InternalCompilerInterface.GetBufferLookup<LoadingResources>(ref __TypeHandle.__Game_Vehicles_LoadingResources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex,
			m_TransportVehicleRequestArchetype = m_TransportVehicleRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		transportWatercraftTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		transportWatercraftTickJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		transportWatercraftTickJob.m_BoardingData = boardingData.ToConcurrent();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<TransportWatercraftTickJob>(transportWatercraftTickJob, m_VehicleQuery, ((SystemBase)this).Dependency);
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
	public TransportWatercraftAISystem()
	{
	}
}

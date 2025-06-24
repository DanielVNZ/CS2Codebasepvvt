using System.Runtime.CompilerServices;
using Game.Achievements;
using Game.Buildings;
using Game.City;
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
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TransportTrainAISystem : GameSystemBase
{
	[BurstCompile]
	private struct TransportTrainTickJob : IJobChunk
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

		public ComponentTypeHandle<Game.Vehicles.CargoTransport> m_CargoTransportType;

		public ComponentTypeHandle<Game.Vehicles.PublicTransport> m_PublicTransportType;

		public ComponentTypeHandle<Target> m_TargetType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public ComponentTypeHandle<Odometer> m_OdometerType;

		public BufferTypeHandle<LayoutElement> m_LayoutElementType;

		public BufferTypeHandle<TrainNavigationLane> m_NavigationLaneType;

		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<TransportVehicleRequest> m_TransportVehicleRequestData;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<TrainData> m_PrefabTrainData;

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
		public ComponentLookup<Game.Routes.Color> m_RouteColorData;

		[ReadOnly]
		public ComponentLookup<Game.Companies.StorageCompany> m_StorageCompanyData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TransportStation> m_TransportStationData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TransportDepot> m_TransportDepotData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public BufferLookup<Passenger> m_Passengers;

		[ReadOnly]
		public BufferLookup<Resources> m_EconomyResources;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Train> m_TrainData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<TrainCurrentLane> m_CurrentLaneData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<TrainNavigation> m_NavigationData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Blocker> m_BlockerData;

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
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedTrainRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedTrainAddTypes;

		[ReadOnly]
		public TransportTrainCarriageSelectData m_TransportTrainCarriageSelectData;

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
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<CurrentRoute> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentRoute>(ref m_CurrentRouteType);
			NativeArray<Game.Vehicles.CargoTransport> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.CargoTransport>(ref m_CargoTransportType);
			NativeArray<Game.Vehicles.PublicTransport> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.PublicTransport>(ref m_PublicTransportType);
			NativeArray<Target> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PathOwner> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<Odometer> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Odometer>(ref m_OdometerType);
			BufferAccessor<LayoutElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LayoutElement>(ref m_LayoutElementType);
			BufferAccessor<TrainNavigationLane> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TrainNavigationLane>(ref m_NavigationLaneType);
			BufferAccessor<ServiceDispatch> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity vehicleEntity = nativeArray[i];
				Owner owner = nativeArray2[i];
				PrefabRef prefabRef = nativeArray3[i];
				PathOwner pathOwner = nativeArray8[i];
				Target target = nativeArray7[i];
				Odometer odometer = nativeArray9[i];
				DynamicBuffer<LayoutElement> layout = bufferAccessor[i];
				DynamicBuffer<TrainNavigationLane> navigationLanes = bufferAccessor2[i];
				DynamicBuffer<ServiceDispatch> serviceDispatches = bufferAccessor3[i];
				CurrentRoute currentRoute = default(CurrentRoute);
				if (nativeArray4.Length != 0)
				{
					currentRoute = nativeArray4[i];
				}
				Game.Vehicles.CargoTransport cargoTransport = default(Game.Vehicles.CargoTransport);
				if (nativeArray5.Length != 0)
				{
					cargoTransport = nativeArray5[i];
				}
				Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
				if (nativeArray6.Length != 0)
				{
					publicTransport = nativeArray6[i];
				}
				Tick(unfilteredChunkIndex, ref random, vehicleEntity, owner, prefabRef, currentRoute, layout, navigationLanes, serviceDispatches, isUnspawned, ref cargoTransport, ref publicTransport, ref pathOwner, ref target, ref odometer);
				nativeArray8[i] = pathOwner;
				nativeArray7[i] = target;
				nativeArray9[i] = odometer;
				if (nativeArray5.Length != 0)
				{
					nativeArray5[i] = cargoTransport;
				}
				if (nativeArray6.Length != 0)
				{
					nativeArray6[i] = publicTransport;
				}
			}
		}

		private void Tick(int jobIndex, ref Random random, Entity vehicleEntity, Owner owner, PrefabRef prefabRef, CurrentRoute currentRoute, DynamicBuffer<LayoutElement> layout, DynamicBuffer<TrainNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, bool isUnspawned, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref PathOwner pathOwner, ref Target target, ref Odometer odometer)
		{
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_078a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_0792: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0804: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07df: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0836: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0671: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0860: Unknown result type (might be due to invalid IL or missing references)
			//IL_0869: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0894: Unknown result type (might be due to invalid IL or missing references)
			//IL_0895: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_0633: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_096b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0978: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0937: Unknown result type (might be due to invalid IL or missing references)
			//IL_093a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0743: Unknown result type (might be due to invalid IL or missing references)
			//IL_0748: Unknown result type (might be due to invalid IL or missing references)
			//IL_095a: Unknown result type (might be due to invalid IL or missing references)
			if (VehicleUtils.ResetUpdatedPath(ref pathOwner))
			{
				DynamicBuffer<LoadingResources> loadingResources = default(DynamicBuffer<LoadingResources>);
				if (((cargoTransport.m_State & CargoTransportFlags.DummyTraffic) != 0 || (publicTransport.m_State & PublicTransportFlags.DummyTraffic) != 0) && m_LoadingResources.TryGetBuffer(vehicleEntity, ref loadingResources))
				{
					if (loadingResources.Length != 0)
					{
						QuantityUpdated(jobIndex, vehicleEntity, layout);
					}
					if (CheckLoadingResources(jobIndex, ref random, vehicleEntity, dummyTraffic: true, layout, loadingResources))
					{
						pathOwner.m_State |= PathFlags.Updated;
						return;
					}
				}
				cargoTransport.m_State &= ~CargoTransportFlags.Arriving;
				publicTransport.m_State &= ~PublicTransportFlags.Arriving;
				DynamicBuffer<PathElement> path = m_PathElements[vehicleEntity];
				float num = VehicleUtils.CalculateLength(vehicleEntity, layout, ref m_PrefabRefData, ref m_PrefabTrainData);
				PathElement prevElement = default(PathElement);
				if ((pathOwner.m_State & PathFlags.Append) != 0)
				{
					if (navigationLanes.Length != 0)
					{
						TrainNavigationLane trainNavigationLane = navigationLanes[navigationLanes.Length - 1];
						prevElement = new PathElement(trainNavigationLane.m_Lane, trainNavigationLane.m_CurvePosition);
					}
				}
				else if (VehicleUtils.IsReversedPath(path, pathOwner, vehicleEntity, layout, m_CurveData, m_CurrentLaneData, m_TrainData, m_TransformData))
				{
					VehicleUtils.ReverseTrain(vehicleEntity, layout, ref m_TrainData, ref m_CurrentLaneData, ref m_NavigationData);
				}
				PathUtils.ExtendReverseLocations(prevElement, path, pathOwner, num, m_CurveData, m_LaneData, m_EdgeLaneData, m_OwnerData, m_EdgeData, m_ConnectedEdges, m_SubLanes);
				if (!m_WaypointData.HasComponent(target.m_Target) || (m_ConnectedData.HasComponent(target.m_Target) && m_BoardingVehicleData.HasComponent(m_ConnectedData[target.m_Target].m_Connected)))
				{
					float distance = num * 0.5f;
					PathUtils.ExtendPath(path, pathOwner, ref distance, ref m_CurveData, ref m_LaneData, ref m_EdgeLaneData, ref m_OwnerData, ref m_EdgeData, ref m_ConnectedEdges, ref m_SubLanes);
				}
				UpdatePantograph(layout);
			}
			Entity val = vehicleEntity;
			if (layout.Length != 0)
			{
				val = layout[0].m_Vehicle;
			}
			Train train = m_TrainData[val];
			TrainCurrentLane currentLane = m_CurrentLaneData[val];
			VehicleUtils.CheckUnspawned(jobIndex, vehicleEntity, currentLane, isUnspawned, m_CommandBuffer);
			bool num2 = (cargoTransport.m_State & CargoTransportFlags.EnRoute) == 0 && (publicTransport.m_State & PublicTransportFlags.EnRoute) == 0;
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
			if (num2)
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
			bool flag = false;
			if (VehicleUtils.IsStuck(pathOwner))
			{
				Blocker blocker = m_BlockerData[vehicleEntity];
				bool num3 = m_ParkedTrainData.HasComponent(blocker.m_Blocker);
				if (num3)
				{
					Entity val2 = blocker.m_Blocker;
					Controller controller = default(Controller);
					if (m_ControllerData.TryGetComponent(val2, ref controller))
					{
						val2 = controller.m_Controller;
					}
					DynamicBuffer<LayoutElement> layout2 = default(DynamicBuffer<LayoutElement>);
					m_LayoutElements.TryGetBuffer(val2, ref layout2);
					VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, val2, layout2);
				}
				if (num3 || blocker.m_Blocker == Entity.Null)
				{
					pathOwner.m_State &= ~PathFlags.Stuck;
					m_BlockerData[vehicleEntity] = default(Blocker);
				}
			}
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(target.m_Target) || VehicleUtils.PathfindFailed(pathOwner))
			{
				if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
				{
					flag = true;
					StopBoarding(jobIndex, ref random, vehicleEntity, currentRoute, layout, ref cargoTransport, ref publicTransport, ref target, ref odometer, isCargoVehicle, forcedStop: true);
				}
				if (VehicleUtils.IsStuck(pathOwner) || (cargoTransport.m_State & (CargoTransportFlags.Returning | CargoTransportFlags.DummyTraffic)) != 0 || (publicTransport.m_State & (PublicTransportFlags.Returning | PublicTransportFlags.DummyTraffic)) != 0)
				{
					VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, vehicleEntity, layout);
					m_TrainData[val] = train;
					m_CurrentLaneData[val] = currentLane;
					return;
				}
				ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref train, ref pathOwner, ref target);
			}
			else if (VehicleUtils.PathEndReached(currentLane))
			{
				if ((cargoTransport.m_State & (CargoTransportFlags.Returning | CargoTransportFlags.DummyTraffic)) != 0 || (publicTransport.m_State & (PublicTransportFlags.Returning | PublicTransportFlags.DummyTraffic)) != 0)
				{
					if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
					{
						if (StopBoarding(jobIndex, ref random, vehicleEntity, currentRoute, layout, ref cargoTransport, ref publicTransport, ref target, ref odometer, isCargoVehicle, forcedStop: false))
						{
							flag = true;
							if (!SelectNextDispatch(jobIndex, vehicleEntity, currentRoute, layout, navigationLanes, serviceDispatches, ref cargoTransport, ref publicTransport, ref train, ref currentLane, ref pathOwner, ref target))
							{
								if (!TryParkTrain(jobIndex, vehicleEntity, owner, layout, navigationLanes, ref train, ref cargoTransport, ref publicTransport, ref currentLane))
								{
									VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, vehicleEntity, layout);
								}
								m_TrainData[val] = train;
								m_CurrentLaneData[val] = currentLane;
								return;
							}
						}
					}
					else if ((CountPassengers(vehicleEntity, layout) <= 0 || !StartBoarding(jobIndex, vehicleEntity, currentRoute, prefabRef, ref cargoTransport, ref publicTransport, ref target, isCargoVehicle)) && !SelectNextDispatch(jobIndex, vehicleEntity, currentRoute, layout, navigationLanes, serviceDispatches, ref cargoTransport, ref publicTransport, ref train, ref currentLane, ref pathOwner, ref target))
					{
						if (!TryParkTrain(jobIndex, vehicleEntity, owner, layout, navigationLanes, ref train, ref cargoTransport, ref publicTransport, ref currentLane))
						{
							VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, vehicleEntity, layout);
						}
						m_TrainData[val] = train;
						m_CurrentLaneData[val] = currentLane;
						return;
					}
				}
				else if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
				{
					if (StopBoarding(jobIndex, ref random, vehicleEntity, currentRoute, layout, ref cargoTransport, ref publicTransport, ref target, ref odometer, isCargoVehicle, forcedStop: false))
					{
						flag = true;
						if ((cargoTransport.m_State & CargoTransportFlags.EnRoute) == 0 && (publicTransport.m_State & PublicTransportFlags.EnRoute) == 0)
						{
							ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref train, ref pathOwner, ref target);
						}
						else
						{
							SetNextWaypointTarget(currentRoute, ref pathOwner, ref target);
						}
					}
				}
				else if (!m_RouteWaypoints.HasBuffer(currentRoute.m_Route) || !m_WaypointData.HasComponent(target.m_Target))
				{
					ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref train, ref pathOwner, ref target);
				}
				else if (!StartBoarding(jobIndex, vehicleEntity, currentRoute, prefabRef, ref cargoTransport, ref publicTransport, ref target, isCargoVehicle))
				{
					if ((cargoTransport.m_State & CargoTransportFlags.EnRoute) == 0 && (publicTransport.m_State & PublicTransportFlags.EnRoute) == 0)
					{
						ReturnToDepot(jobIndex, vehicleEntity, currentRoute, owner, serviceDispatches, ref cargoTransport, ref publicTransport, ref train, ref pathOwner, ref target);
					}
					else
					{
						SetNextWaypointTarget(currentRoute, ref pathOwner, ref target);
					}
				}
			}
			else if (VehicleUtils.ReturnEndReached(currentLane))
			{
				m_TrainData[val] = train;
				m_CurrentLaneData[val] = currentLane;
				VehicleUtils.ReverseTrain(vehicleEntity, layout, ref m_TrainData, ref m_CurrentLaneData, ref m_NavigationData);
				UpdatePantograph(layout);
				val = vehicleEntity;
				if (layout.Length != 0)
				{
					val = layout[0].m_Vehicle;
				}
				train = m_TrainData[val];
				currentLane = m_CurrentLaneData[val];
			}
			else if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
			{
				flag = true;
				StopBoarding(jobIndex, ref random, vehicleEntity, currentRoute, layout, ref cargoTransport, ref publicTransport, ref target, ref odometer, isCargoVehicle, forcedStop: true);
			}
			train.m_Flags &= ~(Game.Vehicles.TrainFlags.BoardingLeft | Game.Vehicles.TrainFlags.BoardingRight);
			publicTransport.m_State &= ~(PublicTransportFlags.StopLeft | PublicTransportFlags.StopRight);
			Entity skipWaypoint = Entity.Null;
			if ((cargoTransport.m_State & CargoTransportFlags.Boarding) != 0 || (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)
			{
				if (!flag)
				{
					Train controllerTrain = m_TrainData[vehicleEntity];
					UpdateStop(val, controllerTrain, isBoarding: true, ref train, ref publicTransport, ref target);
				}
			}
			else if ((cargoTransport.m_State & CargoTransportFlags.Returning) != 0 || (publicTransport.m_State & PublicTransportFlags.Returning) != 0)
			{
				if (CountPassengers(vehicleEntity, layout) == 0)
				{
					SelectNextDispatch(jobIndex, vehicleEntity, currentRoute, layout, navigationLanes, serviceDispatches, ref cargoTransport, ref publicTransport, ref train, ref currentLane, ref pathOwner, ref target);
				}
			}
			else if ((cargoTransport.m_State & CargoTransportFlags.Arriving) != 0 || (publicTransport.m_State & PublicTransportFlags.Arriving) != 0)
			{
				Train controllerTrain2 = m_TrainData[vehicleEntity];
				UpdateStop(val, controllerTrain2, isBoarding: false, ref train, ref publicTransport, ref target);
			}
			else
			{
				CheckNavigationLanes(currentRoute, navigationLanes, ref cargoTransport, ref publicTransport, ref currentLane, ref pathOwner, ref target, out skipWaypoint);
			}
			if (((cargoTransport.m_State & CargoTransportFlags.Boarding) == 0 && (publicTransport.m_State & PublicTransportFlags.Boarding) == 0) || flag)
			{
				if (VehicleUtils.RequireNewPath(pathOwner))
				{
					FindNewPath(vehicleEntity, prefabRef, skipWaypoint, ref currentLane, ref cargoTransport, ref publicTransport, ref pathOwner, ref target);
				}
				else if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Stuck)) == 0)
				{
					CheckParkingSpace(navigationLanes, ref train, ref pathOwner);
				}
			}
			m_TrainData[val] = train;
			m_CurrentLaneData[val] = currentLane;
		}

		private void CheckParkingSpace(DynamicBuffer<TrainNavigationLane> navigationLanes, ref Train train, ref PathOwner pathOwner)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (navigationLanes.Length == 0)
			{
				return;
			}
			TrainNavigationLane trainNavigationLane = navigationLanes[navigationLanes.Length - 1];
			Game.Objects.SpawnLocation spawnLocation = default(Game.Objects.SpawnLocation);
			if ((trainNavigationLane.m_Flags & TrainLaneFlags.ParkingSpace) == 0 || !m_SpawnLocationData.TryGetComponent(trainNavigationLane.m_Lane, ref spawnLocation))
			{
				return;
			}
			if ((spawnLocation.m_Flags & SpawnLocationFlags.ParkedVehicle) != 0)
			{
				if ((train.m_Flags & Game.Vehicles.TrainFlags.IgnoreParkedVehicle) == 0)
				{
					train.m_Flags |= Game.Vehicles.TrainFlags.IgnoreParkedVehicle;
					pathOwner.m_State |= PathFlags.Obsolete;
				}
			}
			else
			{
				train.m_Flags &= ~Game.Vehicles.TrainFlags.IgnoreParkedVehicle;
			}
		}

		private bool TryParkTrain(int jobIndex, Entity entity, Owner owner, DynamicBuffer<LayoutElement> layout, DynamicBuffer<TrainNavigationLane> navigationLanes, ref Train train, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref TrainCurrentLane currentLane)
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			if (navigationLanes.Length == 0)
			{
				return false;
			}
			TrainNavigationLane trainNavigationLane = navigationLanes[navigationLanes.Length - 1];
			if ((trainNavigationLane.m_Flags & TrainLaneFlags.ParkingSpace) == 0)
			{
				return false;
			}
			train.m_Flags &= ~(Game.Vehicles.TrainFlags.BoardingLeft | Game.Vehicles.TrainFlags.BoardingRight | Game.Vehicles.TrainFlags.Pantograph | Game.Vehicles.TrainFlags.IgnoreParkedVehicle);
			cargoTransport.m_State &= CargoTransportFlags.RequiresMaintenance;
			publicTransport.m_State &= PublicTransportFlags.RequiresMaintenance;
			Game.Buildings.TransportDepot transportDepot = default(Game.Buildings.TransportDepot);
			if (m_TransportDepotData.TryGetComponent(owner.m_Owner, ref transportDepot) && (transportDepot.m_Flags & TransportDepotFlags.HasAvailableVehicles) == 0)
			{
				cargoTransport.m_State |= CargoTransportFlags.Disabled;
				publicTransport.m_State |= PublicTransportFlags.Disabled;
			}
			for (int i = 0; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, vehicle, ref m_MovingToParkedTrainRemoveTypes);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, vehicle, ref m_MovingToParkedTrainAddTypes);
				if (i == 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkedTrain>(jobIndex, vehicle, new ParkedTrain(trainNavigationLane.m_Lane, currentLane));
					continue;
				}
				TrainCurrentLane currentLane2 = m_CurrentLaneData[vehicle];
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkedTrain>(jobIndex, vehicle, new ParkedTrain(trainNavigationLane.m_Lane, currentLane2));
			}
			if (m_SpawnLocationData.HasComponent(trainNavigationLane.m_Lane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, trainNavigationLane.m_Lane);
			}
			else
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<FixParkingLocation>(jobIndex, entity, new FixParkingLocation(Entity.Null, entity));
			}
			return true;
		}

		private void UpdatePantograph(DynamicBuffer<LayoutElement> layout)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			for (int i = 0; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				Train train = m_TrainData[vehicle];
				PrefabRef prefabRef = m_PrefabRefData[vehicle];
				TrainData trainData = m_PrefabTrainData[prefabRef.m_Prefab];
				if (flag || (trainData.m_TrainFlags & Game.Prefabs.TrainFlags.Pantograph) == 0)
				{
					train.m_Flags &= ~Game.Vehicles.TrainFlags.Pantograph;
				}
				else
				{
					train.m_Flags |= Game.Vehicles.TrainFlags.Pantograph;
					flag = (trainData.m_TrainFlags & Game.Prefabs.TrainFlags.MultiUnit) != 0;
				}
				m_TrainData[vehicle] = train;
			}
		}

		private void UpdateStop(Entity vehicleEntity, Train controllerTrain, bool isBoarding, ref Train train, ref Game.Vehicles.PublicTransport publicTransport, ref Target target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[vehicleEntity];
			Connected connected = default(Connected);
			Transform transform2 = default(Transform);
			if (!m_ConnectedData.TryGetComponent(target.m_Target, ref connected) || !m_TransformData.TryGetComponent(connected.m_Connected, ref transform2))
			{
				return;
			}
			bool flag = math.dot(math.mul(transform.m_Rotation, math.right()), transform2.m_Position - transform.m_Position) < 0f;
			if (isBoarding)
			{
				if (flag)
				{
					train.m_Flags |= Game.Vehicles.TrainFlags.BoardingLeft;
				}
				else
				{
					train.m_Flags |= Game.Vehicles.TrainFlags.BoardingRight;
				}
			}
			if (flag ^ (((controllerTrain.m_Flags ^ train.m_Flags) & Game.Vehicles.TrainFlags.Reversed) != 0))
			{
				publicTransport.m_State |= PublicTransportFlags.StopLeft;
			}
			else
			{
				publicTransport.m_State |= PublicTransportFlags.StopRight;
			}
		}

		private void FindNewPath(Entity vehicleEntity, PrefabRef prefabRef, Entity skipWaypoint, ref TrainCurrentLane currentLane, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			TrainData trainData = m_PrefabTrainData[prefabRef.m_Prefab];
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(trainData.m_MaxSpeed),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_Methods = PathMethod.Track,
				m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = PathMethod.Track,
				m_TrackTypes = trainData.m_TrackType
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = PathMethod.Track,
				m_TrackTypes = trainData.m_TrackType,
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
			if ((cargoTransport.m_State & CargoTransportFlags.Returning) != 0 || (publicTransport.m_State & PublicTransportFlags.Returning) != 0)
			{
				destination.m_RandomCost = 30f;
			}
			VehicleUtils.SetupPathfind(item: new SetupQueueItem(vehicleEntity, parameters, origin, destination), currentLane: ref currentLane, pathOwner: ref pathOwner, queue: m_PathfindQueue);
		}

		private void CheckNavigationLanes(CurrentRoute currentRoute, DynamicBuffer<TrainNavigationLane> navigationLanes, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref TrainCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, out Entity skipWaypoint)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			skipWaypoint = Entity.Null;
			if (navigationLanes.Length == 0 || navigationLanes.Length >= 10)
			{
				return;
			}
			TrainNavigationLane trainNavigationLane = navigationLanes[navigationLanes.Length - 1];
			if ((trainNavigationLane.m_Flags & TrainLaneFlags.EndOfPath) == 0)
			{
				return;
			}
			if (m_WaypointData.HasComponent(target.m_Target) && m_RouteWaypoints.HasBuffer(currentRoute.m_Route) && (!m_ConnectedData.HasComponent(target.m_Target) || !m_BoardingVehicleData.HasComponent(m_ConnectedData[target.m_Target].m_Connected)))
			{
				if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete)) == 0)
				{
					skipWaypoint = target.m_Target;
					SetNextWaypointTarget(currentRoute, ref pathOwner, ref target);
					trainNavigationLane.m_Flags &= ~TrainLaneFlags.EndOfPath;
					navigationLanes[navigationLanes.Length - 1] = trainNavigationLane;
					cargoTransport.m_State |= CargoTransportFlags.RouteSource;
					publicTransport.m_State |= PublicTransportFlags.RouteSource;
				}
			}
			else
			{
				cargoTransport.m_State |= CargoTransportFlags.Arriving;
				publicTransport.m_State |= PublicTransportFlags.Arriving;
			}
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
				if ((m_SimulationFrameIndex & (num - 1)) == 3)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_TransportVehicleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TransportVehicleRequest>(jobIndex, val, new TransportVehicleRequest(entity, 1f));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(8u));
				}
			}
		}

		private bool SelectNextDispatch(int jobIndex, Entity vehicleEntity, CurrentRoute currentRoute, DynamicBuffer<LayoutElement> layout, DynamicBuffer<TrainNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Train train, ref TrainCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
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
				if (m_TransportVehicleRequestData.HasComponent(request))
				{
					val = m_TransportVehicleRequestData[request].m_Route;
					if (m_PathInformationData.HasComponent(request))
					{
						val2 = m_PathInformationData[request].m_Destination;
					}
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
								UpdateBatches(jobIndex, vehicleEntity, layout);
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
				train.m_Flags &= ~Game.Vehicles.TrainFlags.IgnoreParkedVehicle;
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
						if (PathUtils.TryAppendPath(ref currentLane, navigationLanes, val7, appendPath))
						{
							cargoTransport.m_PathElementTime = num / (float)math.max(1, val7.Length);
							publicTransport.m_PathElementTime = cargoTransport.m_PathElementTime;
							target.m_Target = val2;
							VehicleUtils.ClearEndOfPath(ref currentLane, navigationLanes);
							cargoTransport.m_State &= ~CargoTransportFlags.Arriving;
							publicTransport.m_State &= ~PublicTransportFlags.Arriving;
							float num2 = VehicleUtils.CalculateLength(vehicleEntity, layout, ref m_PrefabRefData, ref m_PrefabTrainData);
							PathElement prevElement = default(PathElement);
							if (navigationLanes.Length != 0)
							{
								TrainNavigationLane trainNavigationLane = navigationLanes[navigationLanes.Length - 1];
								prevElement = new PathElement(trainNavigationLane.m_Lane, trainNavigationLane.m_CurvePosition);
							}
							PathUtils.ExtendReverseLocations(prevElement, val7, pathOwner, num2, m_CurveData, m_LaneData, m_EdgeLaneData, m_OwnerData, m_EdgeData, m_ConnectedEdges, m_SubLanes);
							if (!m_WaypointData.HasComponent(target.m_Target) || (m_ConnectedData.HasComponent(target.m_Target) && m_BoardingVehicleData.HasComponent(m_ConnectedData[target.m_Target].m_Connected)))
							{
								float distance = num2 * 0.5f;
								PathUtils.ExtendPath(val7, pathOwner, ref distance, ref m_CurveData, ref m_LaneData, ref m_EdgeLaneData, ref m_OwnerData, ref m_EdgeData, ref m_ConnectedEdges, ref m_SubLanes);
							}
							return true;
						}
					}
				}
				VehicleUtils.SetTarget(ref pathOwner, ref target, val2);
				return true;
			}
			return false;
		}

		private void UpdateBatches(int jobIndex, Entity vehicleEntity, DynamicBuffer<LayoutElement> layout)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (layout.Length != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, layout.Reinterpret<Entity>().AsNativeArray());
			}
			else
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, vehicleEntity);
			}
		}

		private void ReturnToDepot(int jobIndex, Entity vehicleEntity, CurrentRoute currentRoute, Owner ownerData, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Train train, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			serviceDispatches.Clear();
			cargoTransport.m_RequestCount = 0;
			cargoTransport.m_State &= ~(CargoTransportFlags.EnRoute | CargoTransportFlags.Refueling | CargoTransportFlags.AbandonRoute);
			cargoTransport.m_State |= CargoTransportFlags.Returning;
			publicTransport.m_RequestCount = 0;
			publicTransport.m_State &= ~(PublicTransportFlags.EnRoute | PublicTransportFlags.Refueling | PublicTransportFlags.AbandonRoute);
			publicTransport.m_State |= PublicTransportFlags.Returning;
			train.m_Flags &= ~Game.Vehicles.TrainFlags.IgnoreParkedVehicle;
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentRoute>(jobIndex, vehicleEntity);
			VehicleUtils.SetTarget(ref pathOwner, ref target, ownerData.m_Owner);
		}

		private bool StartBoarding(int jobIndex, Entity vehicleEntity, CurrentRoute currentRoute, PrefabRef prefabRef, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Target target, bool isCargoVehicle)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
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
						TrainData trainData = m_PrefabTrainData[prefabRef.m_Prefab];
						flag = (m_TransportStationData[transportStationFromStop].m_TrainRefuelTypes & trainData.m_EnergyType) != 0;
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

		private bool TryChangeCarriagePrefab(int jobIndex, ref Random random, Entity vehicleEntity, bool dummyTraffic, DynamicBuffer<LoadingResources> loadingResources)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			if (m_EconomyResources.HasBuffer(vehicleEntity))
			{
				DynamicBuffer<Resources> val = m_EconomyResources[vehicleEntity];
				PrefabRef prefabRef = m_PrefabRefData[vehicleEntity];
				if (val.Length == 0 && m_CargoTransportVehicleData.HasComponent(prefabRef.m_Prefab))
				{
					while (loadingResources.Length > 0)
					{
						LoadingResources loadingResources2 = loadingResources[0];
						Entity val2 = m_TransportTrainCarriageSelectData.SelectCarriagePrefab(ref random, loadingResources2.m_Resource, loadingResources2.m_Amount);
						if (val2 != Entity.Null)
						{
							CargoTransportVehicleData cargoTransportVehicleData = m_CargoTransportVehicleData[val2];
							int amount = math.min(loadingResources2.m_Amount, cargoTransportVehicleData.m_CargoCapacity);
							loadingResources2.m_Amount -= cargoTransportVehicleData.m_CargoCapacity;
							if (loadingResources2.m_Amount <= 0)
							{
								loadingResources.RemoveAt(0);
							}
							else
							{
								loadingResources[0] = loadingResources2;
							}
							if (dummyTraffic)
							{
								((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<Resources>(jobIndex, vehicleEntity).Add(new Resources
								{
									m_Resource = loadingResources2.m_Resource,
									m_Amount = amount
								});
							}
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, vehicleEntity, new PrefabRef(val2));
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, vehicleEntity, default(Updated));
							return true;
						}
						loadingResources.RemoveAt(0);
					}
				}
			}
			return false;
		}

		private bool CheckLoadingResources(int jobIndex, ref Random random, Entity vehicleEntity, bool dummyTraffic, DynamicBuffer<LayoutElement> layout, DynamicBuffer<LoadingResources> loadingResources)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			if (loadingResources.Length != 0)
			{
				if (layout.Length != 0)
				{
					for (int i = 0; i < layout.Length; i++)
					{
						if (loadingResources.Length == 0)
						{
							break;
						}
						flag |= TryChangeCarriagePrefab(jobIndex, ref random, layout[i].m_Vehicle, dummyTraffic, loadingResources);
					}
				}
				else
				{
					flag |= TryChangeCarriagePrefab(jobIndex, ref random, vehicleEntity, dummyTraffic, loadingResources);
				}
				loadingResources.Clear();
			}
			return flag;
		}

		private bool StopBoarding(int jobIndex, ref Random random, Entity vehicleEntity, CurrentRoute currentRoute, DynamicBuffer<LayoutElement> layout, ref Game.Vehicles.CargoTransport cargoTransport, ref Game.Vehicles.PublicTransport publicTransport, ref Target target, ref Odometer odometer, bool isCargoVehicle, bool forcedStop)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			if (m_LoadingResources.HasBuffer(vehicleEntity))
			{
				DynamicBuffer<LoadingResources> loadingResources = m_LoadingResources[vehicleEntity];
				if (forcedStop)
				{
					loadingResources.Clear();
				}
				else
				{
					bool dummyTraffic = (cargoTransport.m_State & CargoTransportFlags.DummyTraffic) != 0 || (publicTransport.m_State & PublicTransportFlags.DummyTraffic) != 0;
					flag |= CheckLoadingResources(jobIndex, ref random, vehicleEntity, dummyTraffic, layout, loadingResources);
				}
			}
			if (flag)
			{
				return false;
			}
			bool flag2 = false;
			Connected connected = default(Connected);
			BoardingVehicle boardingVehicle = default(BoardingVehicle);
			if (m_ConnectedData.TryGetComponent(target.m_Target, ref connected) && m_BoardingVehicleData.TryGetComponent(connected.m_Connected, ref boardingVehicle))
			{
				flag2 = boardingVehicle.m_Vehicle == vehicleEntity;
			}
			if (!forcedStop)
			{
				publicTransport.m_MaxBoardingDistance = math.select(publicTransport.m_MinWaitingDistance + 1f, float.MaxValue, publicTransport.m_MinWaitingDistance == float.MaxValue || publicTransport.m_MinWaitingDistance == 0f);
				publicTransport.m_MinWaitingDistance = float.MaxValue;
				if (flag2 && (m_SimulationFrameIndex < cargoTransport.m_DepartureFrame || m_SimulationFrameIndex < publicTransport.m_DepartureFrame || publicTransport.m_MaxBoardingDistance != float.MaxValue))
				{
					return false;
				}
				if (layout.Length != 0)
				{
					for (int i = 0; i < layout.Length; i++)
					{
						if (!ArePassengersReady(layout[i].m_Vehicle))
						{
							return false;
						}
					}
				}
				else if (!ArePassengersReady(vehicleEntity))
				{
					return false;
				}
			}
			if ((cargoTransport.m_State & CargoTransportFlags.Refueling) != 0 || (publicTransport.m_State & PublicTransportFlags.Refueling) != 0)
			{
				odometer.m_Distance = 0f;
			}
			if (isCargoVehicle)
			{
				QuantityUpdated(jobIndex, vehicleEntity, layout);
			}
			if (flag2)
			{
				Entity currentStation = Entity.Null;
				Entity nextStation = Entity.Null;
				if (isCargoVehicle && !forcedStop)
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

		private void QuantityUpdated(int jobIndex, Entity vehicleEntity, DynamicBuffer<LayoutElement> layout)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if (layout.Length != 0)
			{
				for (int i = 0; i < layout.Length; i++)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, layout[i].m_Vehicle, default(Updated));
				}
			}
			else
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, vehicleEntity, default(Updated));
			}
		}

		private int CountPassengers(Entity vehicleEntity, DynamicBuffer<LayoutElement> layout)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			if (layout.Length != 0)
			{
				for (int i = 0; i < layout.Length; i++)
				{
					Entity vehicle = layout[i].m_Vehicle;
					if (m_Passengers.HasBuffer(vehicle))
					{
						num += m_Passengers[vehicle].Length;
					}
				}
			}
			else if (m_Passengers.HasBuffer(vehicleEntity))
			{
				num += m_Passengers[vehicleEntity].Length;
			}
			return num;
		}

		private bool ArePassengersReady(Entity vehicleEntity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if (!m_Passengers.HasBuffer(vehicleEntity))
			{
				return true;
			}
			DynamicBuffer<Passenger> val = m_Passengers[vehicleEntity];
			for (int i = 0; i < val.Length; i++)
			{
				Entity passenger = val[i].m_Passenger;
				if (m_CurrentVehicleData.HasComponent(passenger) && (m_CurrentVehicleData[passenger].m_Flags & CreatureVehicleFlags.Ready) == 0)
				{
					return false;
				}
			}
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentRoute> __Game_Routes_CurrentRoute_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Vehicles.CargoTransport> __Game_Vehicles_CargoTransport_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Odometer> __Game_Vehicles_Odometer_RW_ComponentTypeHandle;

		public BufferTypeHandle<LayoutElement> __Game_Vehicles_LayoutElement_RW_BufferTypeHandle;

		public BufferTypeHandle<TrainNavigationLane> __Game_Vehicles_TrainNavigationLane_RW_BufferTypeHandle;

		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportVehicleRequest> __Game_Simulation_TransportVehicleRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainData> __Game_Prefabs_TrainData_RO_ComponentLookup;

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
		public ComponentLookup<Game.Routes.Color> __Game_Routes_Color_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Companies.StorageCompany> __Game_Companies_StorageCompany_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TransportStation> __Game_Buildings_TransportStation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TransportDepot> __Game_Buildings_TransportDepot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Passenger> __Game_Vehicles_Passenger_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		public ComponentLookup<Train> __Game_Vehicles_Train_RW_ComponentLookup;

		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RW_ComponentLookup;

		public ComponentLookup<TrainNavigation> __Game_Vehicles_TrainNavigation_RW_ComponentLookup;

		public ComponentLookup<Blocker> __Game_Vehicles_Blocker_RW_ComponentLookup;

		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RW_BufferLookup;

		public BufferLookup<LoadingResources> __Game_Vehicles_LoadingResources_RW_BufferLookup;

		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RW_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Routes_CurrentRoute_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentRoute>(true);
			__Game_Vehicles_CargoTransport_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.CargoTransport>(false);
			__Game_Vehicles_PublicTransport_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.PublicTransport>(false);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Vehicles_Odometer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Odometer>(false);
			__Game_Vehicles_LayoutElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LayoutElement>(false);
			__Game_Vehicles_TrainNavigationLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TrainNavigationLane>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Simulation_TransportVehicleRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportVehicleRequest>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Edge>(true);
			__Game_Prefabs_TrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PublicTransportVehicleData>(true);
			__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CargoTransportVehicleData>(true);
			__Game_Routes_Waypoint_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Waypoint>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Routes_BoardingVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BoardingVehicle>(true);
			__Game_Routes_Color_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.Color>(true);
			__Game_Companies_StorageCompany_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Companies.StorageCompany>(true);
			__Game_Buildings_TransportStation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.TransportStation>(true);
			__Game_Buildings_TransportDepot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.TransportDepot>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Vehicles_Passenger_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Passenger>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Vehicles_Train_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(false);
			__Game_Vehicles_TrainCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(false);
			__Game_Vehicles_TrainNavigation_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainNavigation>(false);
			__Game_Vehicles_Blocker_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Blocker>(false);
			__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
			__Game_Vehicles_LoadingResources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LoadingResources>(false);
			__Game_Vehicles_LayoutElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private AchievementTriggerSystem m_AchievementTriggerSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EntityQuery m_VehicleQuery;

	private EntityQuery m_CarriagePrefabQuery;

	private EntityArchetype m_TransportVehicleRequestArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_MovingToParkedTrainRemoveTypes;

	private ComponentTypeSet m_MovingToParkedTrainAddTypes;

	private TransportTrainCarriageSelectData m_TransportTrainCarriageSelectData;

	private TransportBoardingHelpers.BoardingLookupData m_BoardingLookupData;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 3;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_AchievementTriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AchievementTriggerSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_TransportTrainCarriageSelectData = new TransportTrainCarriageSelectData((SystemBase)(object)this);
		m_BoardingLookupData = new TransportBoardingHelpers.BoardingLookupData((SystemBase)(object)this);
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadWrite<TrainCurrentLane>(),
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
		m_MovingToParkedTrainRemoveTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[13]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<TrainNavigation>(),
			ComponentType.ReadWrite<TrainNavigationLane>(),
			ComponentType.ReadWrite<TrainCurrentLane>(),
			ComponentType.ReadWrite<TrainBogieFrame>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<PathElement>(),
			ComponentType.ReadWrite<PathInformation>(),
			ComponentType.ReadWrite<ServiceDispatch>()
		});
		m_MovingToParkedTrainAddTypes = new ComponentTypeSet(ComponentType.ReadWrite<ParkedTrain>(), ComponentType.ReadWrite<Stopped>(), ComponentType.ReadWrite<Updated>());
		m_CarriagePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { TransportTrainCarriageSelectData.GetEntityQueryDesc() });
		((ComponentSystemBase)this).RequireForUpdate(m_VehicleQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		TransportBoardingHelpers.BoardingData boardingData = new TransportBoardingHelpers.BoardingData((Allocator)3);
		m_TransportTrainCarriageSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_CarriagePrefabQuery, (Allocator)3, out var jobHandle);
		m_BoardingLookupData.Update((SystemBase)(object)this);
		TransportTrainTickJob transportTrainTickJob = new TransportTrainTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentRouteType = InternalCompilerInterface.GetComponentTypeHandle<CurrentRoute>(ref __TypeHandle.__Game_Routes_CurrentRoute_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CargoTransportType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.CargoTransport>(ref __TypeHandle.__Game_Vehicles_CargoTransport_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OdometerType = InternalCompilerInterface.GetComponentTypeHandle<Odometer>(ref __TypeHandle.__Game_Vehicles_Odometer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElementType = InternalCompilerInterface.GetBufferTypeHandle<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<TrainNavigationLane>(ref __TypeHandle.__Game_Vehicles_TrainNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatchType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportVehicleRequestData = InternalCompilerInterface.GetComponentLookup<TransportVehicleRequest>(ref __TypeHandle.__Game_Simulation_TransportVehicleRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainData = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportVehicleData = InternalCompilerInterface.GetComponentLookup<PublicTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CargoTransportVehicleData = InternalCompilerInterface.GetComponentLookup<CargoTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaypointData = InternalCompilerInterface.GetComponentLookup<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoardingVehicleData = InternalCompilerInterface.GetComponentLookup<BoardingVehicle>(ref __TypeHandle.__Game_Routes_BoardingVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteColorData = InternalCompilerInterface.GetComponentLookup<Game.Routes.Color>(ref __TypeHandle.__Game_Routes_Color_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanyData = InternalCompilerInterface.GetComponentLookup<Game.Companies.StorageCompany>(ref __TypeHandle.__Game_Companies_StorageCompany_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStationData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.TransportStation>(ref __TypeHandle.__Game_Buildings_TransportStation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportDepotData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.TransportDepot>(ref __TypeHandle.__Game_Buildings_TransportDepot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Passengers = InternalCompilerInterface.GetBufferLookup<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EconomyResources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneData = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationData = InternalCompilerInterface.GetComponentLookup<TrainNavigation>(ref __TypeHandle.__Game_Vehicles_TrainNavigation_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerData = InternalCompilerInterface.GetComponentLookup<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LoadingResources = InternalCompilerInterface.GetBufferLookup<LoadingResources>(ref __TypeHandle.__Game_Vehicles_LoadingResources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex,
			m_RandomSeed = RandomSeed.Next(),
			m_TransportVehicleRequestArchetype = m_TransportVehicleRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_MovingToParkedTrainRemoveTypes = m_MovingToParkedTrainRemoveTypes,
			m_MovingToParkedTrainAddTypes = m_MovingToParkedTrainAddTypes,
			m_TransportTrainCarriageSelectData = m_TransportTrainCarriageSelectData
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		transportTrainTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		transportTrainTickJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		transportTrainTickJob.m_BoardingData = boardingData.ToConcurrent();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<TransportTrainTickJob>(transportTrainTickJob, m_VehicleQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, jobHandle));
		JobHandle val3 = boardingData.ScheduleBoarding((SystemBase)(object)this, m_CityStatisticsSystem, m_AchievementTriggerSystem, m_BoardingLookupData, m_SimulationSystem.frameIndex, val2);
		m_TransportTrainCarriageSelectData.PostUpdate(val2);
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
	public TransportTrainAISystem()
	{
	}
}

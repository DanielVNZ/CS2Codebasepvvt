using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
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
public class TransportVehicleDispatchSystem : GameSystemBase
{
	private struct VehicleDispatch
	{
		public Entity m_Request;

		public Entity m_Source;

		public VehicleDispatch(Entity request, Entity source)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Request = request;
			m_Source = source;
		}
	}

	[BurstCompile]
	private struct TransportVehicleDispatchJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<TransportVehicleRequest> m_TransportVehicleRequestType;

		[ReadOnly]
		public ComponentTypeHandle<Dispatched> m_DispatchedType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public ComponentLookup<TransportVehicleRequest> m_TransportVehicleRequestData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> m_PrefabRouteConnectionData;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> m_PrefabTransportDepotData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<WatercraftData> m_PrefabWatercraftData;

		[ReadOnly]
		public ComponentLookup<AircraftData> m_PrefabAircraftData;

		[ReadOnly]
		public ComponentLookup<AirplaneData> m_PrefabAirplaneData;

		[ReadOnly]
		public ComponentLookup<HelicopterData> m_PrefabHelicopterData;

		[ReadOnly]
		public ComponentLookup<TrainData> m_PrefabTrainData;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> m_ServiceDispatches;

		[ReadOnly]
		public BufferLookup<DispatchedRequest> m_DispatchedRequests;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<TransportLine> m_TransportLineData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Buildings.TransportDepot> m_TransportDepotData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Vehicles.CargoTransport> m_CargoTransportData;

		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public uint m_NextUpdateFrameIndex;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<VehicleDispatch> m_VehicleDispatches;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
			if (index == m_NextUpdateFrameIndex && !((ArchetypeChunk)(ref chunk)).Has<Dispatched>(ref m_DispatchedType) && !((ArchetypeChunk)(ref chunk)).Has<PathInformation>(ref m_PathInformationType))
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<TransportVehicleRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TransportVehicleRequest>(ref m_TransportVehicleRequestType);
				NativeArray<ServiceRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity val = nativeArray[i];
					TransportVehicleRequest transportVehicleRequest = nativeArray2[i];
					ServiceRequest serviceRequest = nativeArray3[i];
					if ((serviceRequest.m_Flags & ServiceRequestFlags.Reversed) != 0)
					{
						if (!ValidateReversed(val, transportVehicleRequest.m_Route))
						{
							((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val);
							continue;
						}
						if (SimulationUtils.TickServiceRequest(ref serviceRequest))
						{
							FindVehicleTarget(unfilteredChunkIndex, val, transportVehicleRequest.m_Route);
						}
					}
					else
					{
						if (!ValidateTarget(val, transportVehicleRequest.m_Route, dispatched: false))
						{
							((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val);
							continue;
						}
						if (SimulationUtils.TickServiceRequest(ref serviceRequest))
						{
							FindVehicleSource(unfilteredChunkIndex, val, transportVehicleRequest.m_Route);
						}
					}
					nativeArray3[i] = serviceRequest;
				}
			}
			if (index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Dispatched> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Dispatched>(ref m_DispatchedType);
			NativeArray<TransportVehicleRequest> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TransportVehicleRequest>(ref m_TransportVehicleRequestType);
			NativeArray<ServiceRequest> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			if (nativeArray4.Length != 0)
			{
				NativeArray<Entity> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int j = 0; j < nativeArray4.Length; j++)
				{
					Entity val2 = nativeArray7[j];
					Dispatched dispatched = nativeArray4[j];
					TransportVehicleRequest transportVehicleRequest2 = nativeArray5[j];
					ServiceRequest serviceRequest2 = nativeArray6[j];
					if (ValidateHandler(val2, dispatched.m_Handler))
					{
						serviceRequest2.m_Cooldown = 0;
					}
					else if (serviceRequest2.m_Cooldown == 0)
					{
						serviceRequest2.m_Cooldown = 1;
					}
					else
					{
						if (!ValidateTarget(val2, transportVehicleRequest2.m_Route, dispatched: true))
						{
							((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val2);
							continue;
						}
						ResetFailedRequest(unfilteredChunkIndex, val2, dispatched: true, ref serviceRequest2);
					}
					nativeArray6[j] = serviceRequest2;
				}
				return;
			}
			NativeArray<PathInformation> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
			if (nativeArray8.Length == 0)
			{
				return;
			}
			NativeArray<Entity> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int k = 0; k < nativeArray5.Length; k++)
			{
				Entity val3 = nativeArray9[k];
				TransportVehicleRequest transportVehicleRequest3 = nativeArray5[k];
				PathInformation pathInformation = nativeArray8[k];
				ServiceRequest serviceRequest3 = nativeArray6[k];
				if ((serviceRequest3.m_Flags & ServiceRequestFlags.Reversed) != 0)
				{
					if (!ValidateReversed(val3, transportVehicleRequest3.m_Route))
					{
						((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val3);
						continue;
					}
					if (pathInformation.m_Destination != Entity.Null)
					{
						ResetReverseRequest(unfilteredChunkIndex, val3, pathInformation, ref serviceRequest3);
					}
					else
					{
						ResetFailedRequest(unfilteredChunkIndex, val3, dispatched: false, ref serviceRequest3);
					}
				}
				else
				{
					if (!ValidateTarget(val3, transportVehicleRequest3.m_Route, dispatched: false))
					{
						((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val3);
						continue;
					}
					if (pathInformation.m_Origin != Entity.Null)
					{
						DispatchVehicle(unfilteredChunkIndex, val3, pathInformation);
					}
					else
					{
						ResetFailedRequest(unfilteredChunkIndex, val3, dispatched: false, ref serviceRequest3);
					}
				}
				nativeArray6[k] = serviceRequest3;
			}
		}

		private bool ValidateReversed(Entity entity, Entity source)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			Game.Buildings.TransportDepot transportDepot = default(Game.Buildings.TransportDepot);
			if (m_TransportDepotData.TryGetComponent(source, ref transportDepot))
			{
				if ((transportDepot.m_Flags & TransportDepotFlags.HasAvailableVehicles) == 0)
				{
					return false;
				}
				if (transportDepot.m_TargetRequest != entity)
				{
					if (m_TransportVehicleRequestData.HasComponent(transportDepot.m_TargetRequest))
					{
						return false;
					}
					transportDepot.m_TargetRequest = entity;
					m_TransportDepotData[source] = transportDepot;
				}
				return true;
			}
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			if (m_PublicTransportData.TryGetComponent(source, ref publicTransport))
			{
				if ((publicTransport.m_State & (PublicTransportFlags.EnRoute | PublicTransportFlags.RequiresMaintenance | PublicTransportFlags.Disabled)) != 0 || publicTransport.m_RequestCount > 0 || m_ParkedCarData.HasComponent(source))
				{
					return false;
				}
				if (publicTransport.m_TargetRequest != entity)
				{
					if (m_TransportVehicleRequestData.HasComponent(publicTransport.m_TargetRequest))
					{
						return false;
					}
					publicTransport.m_TargetRequest = entity;
					m_PublicTransportData[source] = publicTransport;
				}
				return true;
			}
			Game.Vehicles.CargoTransport cargoTransport = default(Game.Vehicles.CargoTransport);
			if (m_CargoTransportData.TryGetComponent(source, ref cargoTransport))
			{
				if ((cargoTransport.m_State & (CargoTransportFlags.EnRoute | CargoTransportFlags.RequiresMaintenance | CargoTransportFlags.Disabled)) != 0 || cargoTransport.m_RequestCount > 0 || m_ParkedCarData.HasComponent(source) || m_ParkedTrainData.HasComponent(source))
				{
					return false;
				}
				if (cargoTransport.m_TargetRequest != entity)
				{
					if (m_TransportVehicleRequestData.HasComponent(cargoTransport.m_TargetRequest))
					{
						return false;
					}
					cargoTransport.m_TargetRequest = entity;
					m_CargoTransportData[source] = cargoTransport;
				}
				return true;
			}
			return false;
		}

		private bool ValidateHandler(Entity entity, Entity handler)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ServiceDispatch> val = default(DynamicBuffer<ServiceDispatch>);
			if (m_ServiceDispatches.TryGetBuffer(handler, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (val[i].m_Request == entity)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool ValidateTarget(Entity entity, Entity target, bool dispatched)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			TransportLine transportLine = default(TransportLine);
			if (!m_TransportLineData.TryGetComponent(target, ref transportLine))
			{
				return false;
			}
			if ((transportLine.m_Flags & TransportLineFlags.RequireVehicles) == 0)
			{
				return false;
			}
			DynamicBuffer<DispatchedRequest> val = default(DynamicBuffer<DispatchedRequest>);
			if (m_DispatchedRequests.TryGetBuffer(target, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (val[i].m_VehicleRequest == entity)
					{
						return !dispatched;
					}
				}
			}
			if (transportLine.m_VehicleRequest != entity)
			{
				if (m_TransportVehicleRequestData.HasComponent(transportLine.m_VehicleRequest))
				{
					return false;
				}
				transportLine.m_VehicleRequest = entity;
				m_TransportLineData[target] = transportLine;
			}
			return true;
		}

		private void ResetReverseRequest(int jobIndex, Entity entity, PathInformation pathInformation, ref ServiceRequest serviceRequest)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			VehicleDispatch vehicleDispatch = new VehicleDispatch(entity, pathInformation.m_Destination);
			m_VehicleDispatches.Enqueue(vehicleDispatch);
			SimulationUtils.ResetReverseRequest(ref serviceRequest);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathInformation>(jobIndex, entity);
		}

		private void ResetFailedRequest(int jobIndex, Entity entity, bool dispatched, ref ServiceRequest serviceRequest)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			SimulationUtils.ResetFailedRequest(ref serviceRequest);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathInformation>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathElement>(jobIndex, entity);
			if (dispatched)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Dispatched>(jobIndex, entity);
			}
		}

		private void DispatchVehicle(int jobIndex, Entity entity, PathInformation pathInformation)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			Entity val = pathInformation.m_Origin;
			Owner owner = default(Owner);
			if ((m_ParkedCarData.HasComponent(val) || m_ParkedTrainData.HasComponent(val)) && m_OwnerData.TryGetComponent(val, ref owner))
			{
				val = owner.m_Owner;
			}
			VehicleDispatch vehicleDispatch = new VehicleDispatch(entity, val);
			m_VehicleDispatches.Enqueue(vehicleDispatch);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Dispatched>(jobIndex, entity, new Dispatched(val));
		}

		private void FindVehicleSource(int jobIndex, Entity requestEntity, Entity targetRoute)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[targetRoute];
			RouteConnectionData routeConnectionData = m_PrefabRouteConnectionData[prefabRef.m_Prefab];
			PathMethod pathMethods = RouteUtils.GetPathMethods(routeConnectionData.m_RouteConnectionType, RouteType.TransportLine, routeConnectionData.m_RouteTrackType, routeConnectionData.m_RouteRoadType, routeConnectionData.m_RouteSizeClass);
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(277.77777f),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_Methods = pathMethods,
				m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic),
				m_PathfindFlags = PathfindFlags.IgnoreExtraEndAccessRequirements
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.TransportVehicle,
				m_Methods = pathMethods,
				m_TrackTypes = routeConnectionData.m_RouteTrackType,
				m_RoadTypes = routeConnectionData.m_RouteRoadType
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.RouteWaypoints,
				m_Methods = pathMethods,
				m_TrackTypes = routeConnectionData.m_RouteTrackType,
				m_RoadTypes = routeConnectionData.m_RouteRoadType,
				m_Entity = targetRoute
			};
			m_PathfindQueue.Enqueue(new SetupQueueItem(requestEntity, parameters, origin, destination));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathInformation>(jobIndex, requestEntity, default(PathInformation));
			((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<PathElement>(jobIndex, requestEntity);
		}

		private void FindVehicleTarget(int jobIndex, Entity requestEntity, Entity vehicleSource)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[vehicleSource];
			PathMethod methods = (PathMethod)0;
			TrackTypes trackTypes = TrackTypes.None;
			RoadTypes roadTypes = RoadTypes.None;
			TransportDepotData transportDepotData = default(TransportDepotData);
			CarData carData = default(CarData);
			WatercraftData watercraftData = default(WatercraftData);
			TrainData trainData = default(TrainData);
			if (m_PrefabTransportDepotData.TryGetComponent(prefabRef.m_Prefab, ref transportDepotData))
			{
				switch (transportDepotData.m_TransportType)
				{
				case TransportType.Bus:
					methods = VehicleUtils.GetPathMethods(new CarData
					{
						m_SizeClass = transportDepotData.m_SizeClass
					});
					roadTypes = RoadTypes.Car;
					break;
				case TransportType.Train:
					methods = PathMethod.Track;
					trackTypes = TrackTypes.Train;
					break;
				case TransportType.Tram:
					methods = PathMethod.Track;
					trackTypes = TrackTypes.Tram;
					break;
				case TransportType.Subway:
					methods = PathMethod.Track;
					trackTypes = TrackTypes.Subway;
					break;
				case TransportType.Ship:
					methods = VehicleUtils.GetPathMethods(new WatercraftData
					{
						m_SizeClass = transportDepotData.m_SizeClass
					});
					roadTypes = RoadTypes.Watercraft;
					break;
				case TransportType.Helicopter:
					methods = VehicleUtils.GetPathMethods(new AircraftData
					{
						m_SizeClass = transportDepotData.m_SizeClass
					});
					roadTypes = RoadTypes.Helicopter;
					break;
				case TransportType.Airplane:
					methods = VehicleUtils.GetPathMethods(new AircraftData
					{
						m_SizeClass = transportDepotData.m_SizeClass
					});
					roadTypes = RoadTypes.Airplane;
					break;
				}
			}
			else if (m_PrefabCarData.TryGetComponent(prefabRef.m_Prefab, ref carData))
			{
				methods = VehicleUtils.GetPathMethods(carData);
				roadTypes = RoadTypes.Car;
			}
			else if (m_PrefabWatercraftData.TryGetComponent(prefabRef.m_Prefab, ref watercraftData))
			{
				methods = VehicleUtils.GetPathMethods(watercraftData);
				roadTypes = RoadTypes.Watercraft;
			}
			else if (m_PrefabAirplaneData.HasComponent(prefabRef.m_Prefab))
			{
				methods = VehicleUtils.GetPathMethods(m_PrefabAircraftData[prefabRef.m_Prefab]);
				roadTypes = RoadTypes.Airplane;
			}
			else if (m_PrefabHelicopterData.HasComponent(prefabRef.m_Prefab))
			{
				methods = VehicleUtils.GetPathMethods(m_PrefabAircraftData[prefabRef.m_Prefab]);
				roadTypes = RoadTypes.Helicopter;
			}
			else if (m_PrefabTrainData.TryGetComponent(prefabRef.m_Prefab, ref trainData))
			{
				methods = PathMethod.Track;
				trackTypes = trainData.m_TrackType;
			}
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(277.77777f),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_Methods = methods,
				m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = methods,
				m_TrackTypes = trackTypes,
				m_RoadTypes = roadTypes,
				m_Entity = vehicleSource
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.TransportVehicleRequest,
				m_Methods = methods,
				m_TrackTypes = trackTypes,
				m_RoadTypes = roadTypes
			};
			m_PathfindQueue.Enqueue(new SetupQueueItem(requestEntity, parameters, origin, destination));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathInformation>(jobIndex, requestEntity, default(PathInformation));
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct DispatchVehiclesJob : IJob
	{
		public NativeQueue<VehicleDispatch> m_VehicleDispatches;

		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		public BufferLookup<ServiceDispatch> m_ServiceDispatches;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			VehicleDispatch vehicleDispatch = default(VehicleDispatch);
			DynamicBuffer<ServiceDispatch> val = default(DynamicBuffer<ServiceDispatch>);
			ServiceRequest serviceRequest = default(ServiceRequest);
			while (m_VehicleDispatches.TryDequeue(ref vehicleDispatch))
			{
				if (m_ServiceDispatches.TryGetBuffer(vehicleDispatch.m_Source, ref val))
				{
					val.Add(new ServiceDispatch(vehicleDispatch.m_Request));
				}
				else if (m_ServiceRequestData.TryGetComponent(vehicleDispatch.m_Source, ref serviceRequest))
				{
					serviceRequest.m_Flags |= ServiceRequestFlags.SkipCooldown;
					m_ServiceRequestData[vehicleDispatch.m_Source] = serviceRequest;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TransportVehicleRequest> __Game_Simulation_TransportVehicleRequest_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Dispatched> __Game_Simulation_Dispatched_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public ComponentTypeHandle<ServiceRequest> __Game_Simulation_ServiceRequest_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<TransportVehicleRequest> __Game_Simulation_TransportVehicleRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> __Game_Prefabs_RouteConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> __Game_Prefabs_TransportDepotData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WatercraftData> __Game_Prefabs_WatercraftData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AircraftData> __Game_Prefabs_AircraftData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AirplaneData> __Game_Prefabs_AirplaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HelicopterData> __Game_Prefabs_HelicopterData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainData> __Game_Prefabs_TrainData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> __Game_Simulation_ServiceDispatch_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<DispatchedRequest> __Game_Routes_DispatchedRequest_RO_BufferLookup;

		public ComponentLookup<TransportLine> __Game_Routes_TransportLine_RW_ComponentLookup;

		public ComponentLookup<Game.Buildings.TransportDepot> __Game_Buildings_TransportDepot_RW_ComponentLookup;

		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RW_ComponentLookup;

		public ComponentLookup<Game.Vehicles.CargoTransport> __Game_Vehicles_CargoTransport_RW_ComponentLookup;

		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RW_ComponentLookup;

		public BufferLookup<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferLookup;

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
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_TransportVehicleRequest_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TransportVehicleRequest>(true);
			__Game_Simulation_Dispatched_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Dispatched>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Simulation_ServiceRequest_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceRequest>(false);
			__Game_Simulation_TransportVehicleRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportVehicleRequest>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_RouteConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteConnectionData>(true);
			__Game_Prefabs_TransportDepotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportDepotData>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_WatercraftData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftData>(true);
			__Game_Prefabs_AircraftData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AircraftData>(true);
			__Game_Prefabs_AirplaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AirplaneData>(true);
			__Game_Prefabs_HelicopterData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HelicopterData>(true);
			__Game_Prefabs_TrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainData>(true);
			__Game_Simulation_ServiceDispatch_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDispatch>(true);
			__Game_Routes_DispatchedRequest_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<DispatchedRequest>(true);
			__Game_Routes_TransportLine_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLine>(false);
			__Game_Buildings_TransportDepot_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.TransportDepot>(false);
			__Game_Vehicles_PublicTransport_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(false);
			__Game_Vehicles_CargoTransport_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.CargoTransport>(false);
			__Game_Simulation_ServiceRequest_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDispatch>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private EntityQuery m_RequestQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_RequestQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<TransportVehicleRequest>(),
			ComponentType.ReadOnly<UpdateFrame>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_RequestQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		uint num = (m_SimulationSystem.frameIndex >> 4) & 7;
		uint nextUpdateFrameIndex = (num + 4) & 7;
		NativeQueue<VehicleDispatch> vehicleDispatches = default(NativeQueue<VehicleDispatch>);
		vehicleDispatches._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		TransportVehicleDispatchJob transportVehicleDispatchJob = new TransportVehicleDispatchJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportVehicleRequestType = InternalCompilerInterface.GetComponentTypeHandle<TransportVehicleRequest>(ref __TypeHandle.__Game_Simulation_TransportVehicleRequest_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DispatchedType = InternalCompilerInterface.GetComponentTypeHandle<Dispatched>(ref __TypeHandle.__Game_Simulation_Dispatched_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestType = InternalCompilerInterface.GetComponentTypeHandle<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportVehicleRequestData = InternalCompilerInterface.GetComponentLookup<TransportVehicleRequest>(ref __TypeHandle.__Game_Simulation_TransportVehicleRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRouteConnectionData = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransportDepotData = InternalCompilerInterface.GetComponentLookup<TransportDepotData>(ref __TypeHandle.__Game_Prefabs_TransportDepotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWatercraftData = InternalCompilerInterface.GetComponentLookup<WatercraftData>(ref __TypeHandle.__Game_Prefabs_WatercraftData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAircraftData = InternalCompilerInterface.GetComponentLookup<AircraftData>(ref __TypeHandle.__Game_Prefabs_AircraftData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAirplaneData = InternalCompilerInterface.GetComponentLookup<AirplaneData>(ref __TypeHandle.__Game_Prefabs_AirplaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabHelicopterData = InternalCompilerInterface.GetComponentLookup<HelicopterData>(ref __TypeHandle.__Game_Prefabs_HelicopterData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainData = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatches = InternalCompilerInterface.GetBufferLookup<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DispatchedRequests = InternalCompilerInterface.GetBufferLookup<DispatchedRequest>(ref __TypeHandle.__Game_Routes_DispatchedRequest_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLine>(ref __TypeHandle.__Game_Routes_TransportLine_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportDepotData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.TransportDepot>(ref __TypeHandle.__Game_Buildings_TransportDepot_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CargoTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.CargoTransport>(ref __TypeHandle.__Game_Vehicles_CargoTransport_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameIndex = num,
			m_NextUpdateFrameIndex = nextUpdateFrameIndex
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		transportVehicleDispatchJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		transportVehicleDispatchJob.m_VehicleDispatches = vehicleDispatches.AsParallelWriter();
		transportVehicleDispatchJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		TransportVehicleDispatchJob transportVehicleDispatchJob2 = transportVehicleDispatchJob;
		DispatchVehiclesJob obj = new DispatchVehiclesJob
		{
			m_VehicleDispatches = vehicleDispatches,
			m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatches = InternalCompilerInterface.GetBufferLookup<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<TransportVehicleDispatchJob>(transportVehicleDispatchJob2, m_RequestQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = IJobExtensions.Schedule<DispatchVehiclesJob>(obj, val2);
		vehicleDispatches.Dispose(val3);
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
	public TransportVehicleDispatchSystem()
	{
	}
}

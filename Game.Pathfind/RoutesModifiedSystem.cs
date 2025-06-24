using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Pathfind;

[CompilerGenerated]
public class RoutesModifiedSystem : GameSystemBase
{
	[BurstCompile]
	private struct AddPathEdgeJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Waypoint> m_WaypointType;

		[ReadOnly]
		public ComponentTypeHandle<Position> m_PositionType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<AccessLane> m_AccessLaneType;

		[ReadOnly]
		public ComponentTypeHandle<RouteLane> m_RouteLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.Segment> m_SegmentType;

		[ReadOnly]
		public ComponentTypeHandle<TaxiStand> m_TaxiStandType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.TakeoffLocation> m_TakeoffLocationType;

		[ReadOnly]
		public ComponentTypeHandle<Connected> m_ConnectedType;

		[ReadOnly]
		public ComponentTypeHandle<RouteInfo> m_RouteInfoType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.SpawnLocation> m_SpawnLocationType;

		[ReadOnly]
		public ComponentTypeHandle<WaitingPassengers> m_WaitingPassengersType;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TransportStop> m_TransportStopData;

		[ReadOnly]
		public ComponentLookup<TransportLine> m_TransportLineData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_NetLaneData;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_PrefabTransportLineData;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> m_PrefabRouteConnectionData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public ComponentLookup<PathfindTransportData> m_TransportPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindPedestrianData> m_PedestrianPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindCarData> m_CarPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindTrackData> m_TrackPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindConnectionData> m_ConnectionPathfindData;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_Waypoints;

		[WriteOnly]
		public NativeArray<CreateActionData> m_Actions;

		public void Execute()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_0787: Unknown result type (might be due to invalid IL or missing references)
			//IL_078c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_079f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_07db: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0828: Unknown result type (might be due to invalid IL or missing references)
			//IL_082d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0835: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_0850: Unknown result type (might be due to invalid IL or missing references)
			//IL_0872: Unknown result type (might be due to invalid IL or missing references)
			//IL_0874: Unknown result type (might be due to invalid IL or missing references)
			//IL_087b: Unknown result type (might be due to invalid IL or missing references)
			//IL_088a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0899: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0622: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			Game.Net.CarLane carLane = default(Game.Net.CarLane);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<AccessLane> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<AccessLane>(ref m_AccessLaneType);
				NativeArray<RouteLane> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<RouteLane>(ref m_RouteLaneType);
				NativeArray<Game.Objects.SpawnLocation> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Objects.SpawnLocation>(ref m_SpawnLocationType);
				if (nativeArray3.Length != 0 || nativeArray4.Length != 0 || nativeArray5.Length != 0)
				{
					NativeArray<Waypoint> nativeArray6 = ((ArchetypeChunk)(ref val)).GetNativeArray<Waypoint>(ref m_WaypointType);
					NativeArray<Position> nativeArray7 = ((ArchetypeChunk)(ref val)).GetNativeArray<Position>(ref m_PositionType);
					NativeArray<Connected> nativeArray8 = ((ArchetypeChunk)(ref val)).GetNativeArray<Connected>(ref m_ConnectedType);
					NativeArray<Transform> nativeArray9 = ((ArchetypeChunk)(ref val)).GetNativeArray<Transform>(ref m_TransformType);
					NativeArray<Game.Routes.TakeoffLocation> nativeArray10 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Routes.TakeoffLocation>(ref m_TakeoffLocationType);
					NativeArray<TaxiStand> nativeArray11 = ((ArchetypeChunk)(ref val)).GetNativeArray<TaxiStand>(ref m_TaxiStandType);
					NativeArray<WaitingPassengers> nativeArray12 = ((ArchetypeChunk)(ref val)).GetNativeArray<WaitingPassengers>(ref m_WaitingPassengersType);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Entity val2 = nativeArray[j];
						AccessLane accessLane = default(AccessLane);
						if (nativeArray3.Length != 0)
						{
							accessLane = nativeArray3[j];
						}
						Game.Objects.SpawnLocation spawnLocation = default(Game.Objects.SpawnLocation);
						if (nativeArray5.Length != 0)
						{
							spawnLocation = nativeArray5[j];
						}
						CreateActionData createActionData = new CreateActionData
						{
							m_Owner = val2
						};
						Entity lane = Entity.Null;
						if (m_LaneData.HasComponent(spawnLocation.m_ConnectedLane1))
						{
							lane = spawnLocation.m_ConnectedLane1;
							createActionData.m_StartNode = new PathNode(m_LaneData[spawnLocation.m_ConnectedLane1].m_MiddleNode, spawnLocation.m_CurvePosition1);
						}
						else if (m_LaneData.HasComponent(accessLane.m_Lane))
						{
							lane = accessLane.m_Lane;
							createActionData.m_StartNode = new PathNode(m_LaneData[accessLane.m_Lane].m_MiddleNode, accessLane.m_CurvePos);
						}
						else if (m_TransportStopData.HasComponent(accessLane.m_Lane))
						{
							lane = accessLane.m_Lane;
							createActionData.m_StartNode = new PathNode(accessLane.m_Lane, 2);
						}
						else
						{
							createActionData.m_StartNode = new PathNode(val2, 2);
						}
						createActionData.m_MiddleNode = new PathNode(val2, 1);
						if (nativeArray7.Length != 0)
						{
							createActionData.m_Location = PathUtils.GetLocationSpecification(nativeArray7[j].m_Position);
						}
						else
						{
							createActionData.m_Location = PathUtils.GetLocationSpecification(nativeArray9[j].m_Position);
						}
						if (nativeArray6.Length != 0)
						{
							createActionData.m_EndNode = new PathNode(val2, 0);
							Owner owner = default(Owner);
							if (nativeArray2.Length != 0)
							{
								owner = nativeArray2[j];
							}
							Game.Routes.TransportStop transportStop = default(Game.Routes.TransportStop);
							bool isWaypoint = true;
							if (nativeArray8.Length != 0)
							{
								Connected connected = nativeArray8[j];
								if (m_TransportStopData.HasComponent(connected.m_Connected))
								{
									transportStop = m_TransportStopData[connected.m_Connected];
									isWaypoint = false;
								}
							}
							WaitingPassengers waitingPassengers = default(WaitingPassengers);
							if (nativeArray12.Length != 0)
							{
								waitingPassengers = nativeArray12[j];
							}
							TransportLine transportLine;
							TransportLineData transportLineData = GetTransportLineData(owner.m_Owner, out transportLine);
							PathfindTransportData transportLinePathfindData = GetTransportLinePathfindData(transportLineData);
							createActionData.m_Specification = PathUtils.GetTransportStopSpecification(transportStop, transportLine, waitingPassengers, transportLineData, transportLinePathfindData, isWaypoint);
						}
						else
						{
							RouteLane routeLane = default(RouteLane);
							if (nativeArray4.Length != 0)
							{
								routeLane = nativeArray4[j];
							}
							if (m_LaneData.HasComponent(routeLane.m_EndLane))
							{
								createActionData.m_EndNode = new PathNode(m_LaneData[routeLane.m_EndLane].m_MiddleNode, routeLane.m_EndCurvePos);
							}
							else
							{
								createActionData.m_EndNode = new PathNode(val2, 0);
							}
							createActionData.m_SecondaryEndNode = createActionData.m_EndNode;
							if (nativeArray5.Length != 0)
							{
								SpawnLocationData spawnLocationData = GetSpawnLocationData(val2);
								if (spawnLocationData.m_ConnectionType != RouteConnectionType.None)
								{
									createActionData.m_Specification = GetSpawnLocationPathSpecification(createActionData.m_Location.m_Line.a, spawnLocationData.m_ConnectionType, spawnLocationData.m_RoadTypes, spawnLocation.m_ConnectedLane1, spawnLocation.m_CurvePosition1, 0, spawnLocation.m_AccessRestriction, spawnLocationData.m_RequireAuthorization, (spawnLocation.m_Flags & SpawnLocationFlags.AllowEnter) != 0, (spawnLocation.m_Flags & SpawnLocationFlags.AllowExit) != 0);
									if (nativeArray11.Length != 0)
									{
										createActionData.m_EndNode = new PathNode(val2, 0);
									}
									else
									{
										RouteConnectionData routeConnectionData = GetRouteConnectionData(val2);
										if ((spawnLocationData.m_ConnectionType == RouteConnectionType.Road || spawnLocationData.m_ConnectionType == RouteConnectionType.Cargo || spawnLocationData.m_ConnectionType == RouteConnectionType.Parking) && spawnLocationData.m_ConnectionType != routeConnectionData.m_AccessConnectionType)
										{
											int laneCrossCount = 1;
											if (m_MasterLaneData.HasComponent(spawnLocation.m_ConnectedLane1))
											{
												MasterLane masterLane = m_MasterLaneData[spawnLocation.m_ConnectedLane1];
												laneCrossCount = masterLane.m_MaxIndex - masterLane.m_MinIndex + 1;
											}
											bool flag = false;
											if (m_LaneData.HasComponent(spawnLocation.m_ConnectedLane2))
											{
												createActionData.m_SecondaryStartNode = new PathNode(m_LaneData[spawnLocation.m_ConnectedLane2].m_MiddleNode, spawnLocation.m_CurvePosition2);
												createActionData.m_SecondaryEndNode = createActionData.m_EndNode;
											}
											else
											{
												flag = true;
												createActionData.m_SecondaryStartNode = new PathNode(val2, 3);
												createActionData.m_SecondaryEndNode = createActionData.m_EndNode;
											}
											createActionData.m_SecondarySpecification = GetSpawnLocationPathSpecification(createActionData.m_Location.m_Line.a, spawnLocationData.m_ConnectionType, spawnLocationData.m_RoadTypes, spawnLocation.m_ConnectedLane2, spawnLocation.m_CurvePosition2, laneCrossCount, spawnLocation.m_AccessRestriction, spawnLocationData.m_RequireAuthorization, (spawnLocation.m_Flags & SpawnLocationFlags.AllowEnter) != 0, (spawnLocation.m_Flags & SpawnLocationFlags.AllowExit) != 0);
											if (flag)
											{
												createActionData.m_SecondarySpecification.m_Flags &= ~(EdgeFlags.Forward | EdgeFlags.Backward);
											}
										}
									}
								}
							}
							else if (nativeArray10.Length != 0)
							{
								Game.Routes.TakeoffLocation takeoffLocation = nativeArray10[j];
								RouteConnectionData routeConnectionData2 = GetRouteConnectionData(val2);
								createActionData.m_Specification = GetSpawnLocationPathSpecification(createActionData.m_Location.m_Line.a, routeConnectionData2.m_RouteConnectionType, routeConnectionData2.m_RouteRoadType, routeLane.m_EndLane, routeLane.m_EndCurvePos, 0, takeoffLocation.m_AccessRestriction, requireAuthorization: false, (takeoffLocation.m_Flags & TakeoffLocationFlags.AllowEnter) != 0, (takeoffLocation.m_Flags & TakeoffLocationFlags.AllowExit) != 0);
								if (routeConnectionData2.m_RouteConnectionType == RouteConnectionType.Air && routeConnectionData2.m_RouteRoadType == RoadTypes.Airplane && m_CarLaneData.TryGetComponent(accessLane.m_Lane, ref carLane) && (carLane.m_Flags & CarLaneFlags.Twoway) == 0)
								{
									createActionData.m_Specification.m_Flags &= (EdgeFlags)((accessLane.m_CurvePos >= 0.5f) ? 65533 : 65534);
								}
							}
							if (nativeArray11.Length != 0)
							{
								TaxiStand taxiStand = nativeArray11[j];
								Game.Routes.TransportStop transportStop2 = default(Game.Routes.TransportStop);
								if (m_TransportStopData.HasComponent(val2))
								{
									transportStop2 = m_TransportStopData[val2];
								}
								WaitingPassengers waitingPassengers2 = default(WaitingPassengers);
								if (nativeArray12.Length != 0)
								{
									waitingPassengers2 = nativeArray12[j];
								}
								PathfindTransportData netLaneTransportPathfindData = GetNetLaneTransportPathfindData(lane);
								createActionData.m_SecondaryStartNode = createActionData.m_StartNode;
								createActionData.m_SecondarySpecification = PathUtils.GetTaxiStopSpecification(transportStop2, taxiStand, waitingPassengers2, netLaneTransportPathfindData);
							}
						}
						m_Actions[num++] = createActionData;
					}
				}
				NativeArray<Game.Routes.Segment> nativeArray13 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Routes.Segment>(ref m_SegmentType);
				if (nativeArray13.Length == 0)
				{
					continue;
				}
				NativeArray<RouteInfo> nativeArray14 = ((ArchetypeChunk)(ref val)).GetNativeArray<RouteInfo>(ref m_RouteInfoType);
				for (int k = 0; k < nativeArray13.Length; k++)
				{
					Entity owner2 = nativeArray[k];
					Owner owner3 = nativeArray2[k];
					Game.Routes.Segment segment = nativeArray13[k];
					RouteInfo routeInfo = default(RouteInfo);
					if (nativeArray14.Length != 0)
					{
						routeInfo = nativeArray14[k];
					}
					DynamicBuffer<RouteWaypoint> val3 = m_Waypoints[owner3.m_Owner];
					int num2 = math.select(segment.m_Index + 1, 0, segment.m_Index == val3.Length - 1);
					Entity waypoint = val3[segment.m_Index].m_Waypoint;
					Entity waypoint2 = val3[num2].m_Waypoint;
					Position position = m_PositionData[waypoint];
					Position position2 = m_PositionData[waypoint2];
					TransportLine transportLine2;
					TransportLineData transportLineData2 = GetTransportLineData(owner3.m_Owner, out transportLine2);
					PathfindTransportData transportLinePathfindData2 = GetTransportLinePathfindData(transportLineData2);
					CreateActionData createActionData2 = new CreateActionData
					{
						m_Owner = owner2,
						m_StartNode = new PathNode(waypoint, 0),
						m_MiddleNode = new PathNode(owner2, 0),
						m_EndNode = new PathNode(waypoint2, 0),
						m_Specification = PathUtils.GetTransportLineSpecification(transportLineData2, transportLinePathfindData2, routeInfo),
						m_Location = PathUtils.GetLocationSpecification(position.m_Position, position2.m_Position)
					};
					m_Actions[num++] = createActionData2;
				}
			}
		}

		private SpawnLocationData GetSpawnLocationData(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[entity];
			if (m_PrefabSpawnLocationData.HasComponent(prefabRef.m_Prefab))
			{
				return m_PrefabSpawnLocationData[prefabRef.m_Prefab];
			}
			return default(SpawnLocationData);
		}

		private RouteConnectionData GetRouteConnectionData(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[entity];
			if (m_PrefabRouteConnectionData.HasComponent(prefabRef.m_Prefab))
			{
				return m_PrefabRouteConnectionData[prefabRef.m_Prefab];
			}
			return default(RouteConnectionData);
		}

		private TransportLineData GetTransportLineData(Entity owner, out TransportLine transportLine)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (m_TransportLineData.HasComponent(owner))
			{
				transportLine = m_TransportLineData[owner];
				PrefabRef prefabRef = m_PrefabRefData[owner];
				return m_PrefabTransportLineData[prefabRef.m_Prefab];
			}
			transportLine = default(TransportLine);
			return default(TransportLineData);
		}

		private PathfindTransportData GetTransportLinePathfindData(TransportLineData transportLineData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_TransportPathfindData.HasComponent(transportLineData.m_PathfindPrefab))
			{
				return m_TransportPathfindData[transportLineData.m_PathfindPrefab];
			}
			return default(PathfindTransportData);
		}

		private PathfindTransportData GetNetLaneTransportPathfindData(Entity lane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			if (m_PrefabRefData.HasComponent(lane))
			{
				PrefabRef prefabRef = m_PrefabRefData[lane];
				if (m_NetLaneData.HasComponent(prefabRef.m_Prefab))
				{
					NetLaneData netLaneData = m_NetLaneData[prefabRef.m_Prefab];
					if (m_TransportPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
					{
						return m_TransportPathfindData[netLaneData.m_PathfindPrefab];
					}
				}
			}
			return default(PathfindTransportData);
		}

		private PathSpecification GetSpawnLocationPathSpecification(float3 position, RouteConnectionType connectionType, RoadTypes roadType, Entity lane, float curvePos, int laneCrossCount, Entity accessRestriction, bool requireAuthorization, bool allowEnter, bool allowExit)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			NetLaneData netLaneData = default(NetLaneData);
			if (m_PrefabRefData.HasComponent(lane))
			{
				PrefabRef prefabRef = m_PrefabRefData[lane];
				if (m_NetLaneData.HasComponent(prefabRef.m_Prefab))
				{
					netLaneData = m_NetLaneData[prefabRef.m_Prefab];
				}
			}
			switch (connectionType)
			{
			case RouteConnectionType.Pedestrian:
			{
				float distance2 = 0f;
				if (m_CurveData.HasComponent(lane))
				{
					distance2 = math.distance(position, MathUtils.Position(m_CurveData[lane].m_Bezier, curvePos));
				}
				if (m_ConnectionPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					PathfindConnectionData connectionPathfindData3 = m_ConnectionPathfindData[netLaneData.m_PathfindPrefab];
					return PathUtils.GetSpawnLocationSpecification(connectionType, connectionPathfindData3, roadType, distance2, accessRestriction, requireAuthorization, allowEnter, allowExit);
				}
				PathfindPedestrianData pedestrianPathfindData = default(PathfindPedestrianData);
				if (m_PedestrianPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					pedestrianPathfindData = m_PedestrianPathfindData[netLaneData.m_PathfindPrefab];
				}
				return PathUtils.GetSpawnLocationSpecification(pedestrianPathfindData, distance2, accessRestriction, requireAuthorization, allowEnter, allowExit);
			}
			case RouteConnectionType.Road:
			case RouteConnectionType.Cargo:
			case RouteConnectionType.Parking:
			{
				float distance = 0f;
				if (m_CurveData.HasComponent(lane))
				{
					distance = math.distance(position, MathUtils.Position(m_CurveData[lane].m_Bezier, curvePos));
				}
				if (m_ConnectionPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					PathfindConnectionData connectionPathfindData2 = m_ConnectionPathfindData[netLaneData.m_PathfindPrefab];
					return PathUtils.GetSpawnLocationSpecification(connectionType, connectionPathfindData2, roadType, distance, accessRestriction, requireAuthorization, allowEnter, allowExit);
				}
				Game.Net.CarLane carLane = default(Game.Net.CarLane);
				if (m_CarLaneData.HasComponent(lane))
				{
					carLane = m_CarLaneData[lane];
				}
				else
				{
					carLane.m_SpeedLimit = 277.77777f;
				}
				PathfindCarData carPathfindData = default(PathfindCarData);
				if (m_CarPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					carPathfindData = m_CarPathfindData[netLaneData.m_PathfindPrefab];
				}
				return PathUtils.GetSpawnLocationSpecification(connectionType, carPathfindData, carLane, distance, laneCrossCount, accessRestriction, requireAuthorization, allowEnter, allowExit);
			}
			case RouteConnectionType.Track:
			{
				if (m_ConnectionPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					PathfindConnectionData connectionPathfindData4 = m_ConnectionPathfindData[netLaneData.m_PathfindPrefab];
					return PathUtils.GetSpawnLocationSpecification(connectionType, connectionPathfindData4, roadType, 0f, accessRestriction, requireAuthorization, allowEnter, allowExit);
				}
				PathfindTrackData trackPathfindData = default(PathfindTrackData);
				if (m_TrackPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					trackPathfindData = m_TrackPathfindData[netLaneData.m_PathfindPrefab];
				}
				return PathUtils.GetSpawnLocationSpecification(trackPathfindData, accessRestriction);
			}
			case RouteConnectionType.Air:
			{
				PathfindConnectionData connectionPathfindData = default(PathfindConnectionData);
				if (m_ConnectionPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					connectionPathfindData = m_ConnectionPathfindData[netLaneData.m_PathfindPrefab];
				}
				return PathUtils.GetSpawnLocationSpecification(connectionType, connectionPathfindData, roadType, 0f, accessRestriction, requireAuthorization, allowEnter, allowExit);
			}
			default:
				return default(PathSpecification);
			}
		}
	}

	[BurstCompile]
	private struct UpdatePathEdgeJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Waypoint> m_WaypointType;

		[ReadOnly]
		public ComponentTypeHandle<Position> m_PositionType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<AccessLane> m_AccessLaneType;

		[ReadOnly]
		public ComponentTypeHandle<RouteLane> m_RouteLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.Segment> m_SegmentType;

		[ReadOnly]
		public ComponentTypeHandle<TaxiStand> m_TaxiStandType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.TakeoffLocation> m_TakeoffLocationType;

		[ReadOnly]
		public ComponentTypeHandle<Connected> m_ConnectedType;

		[ReadOnly]
		public ComponentTypeHandle<RouteInfo> m_RouteInfoType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.SpawnLocation> m_SpawnLocationType;

		[ReadOnly]
		public ComponentTypeHandle<WaitingPassengers> m_WaitingPassengersType;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TransportStop> m_TransportStopData;

		[ReadOnly]
		public ComponentLookup<TransportLine> m_TransportLineData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_NetLaneData;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_PrefabTransportLineData;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> m_PrefabRouteConnectionData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public ComponentLookup<PathfindTransportData> m_TransportPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindPedestrianData> m_PedestrianPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindCarData> m_CarPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindTrackData> m_TrackPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindConnectionData> m_ConnectionPathfindData;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_Waypoints;

		[WriteOnly]
		public NativeArray<UpdateActionData> m_Actions;

		public void Execute()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_0787: Unknown result type (might be due to invalid IL or missing references)
			//IL_078c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_079f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_07db: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0828: Unknown result type (might be due to invalid IL or missing references)
			//IL_082d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0835: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_0850: Unknown result type (might be due to invalid IL or missing references)
			//IL_0872: Unknown result type (might be due to invalid IL or missing references)
			//IL_0874: Unknown result type (might be due to invalid IL or missing references)
			//IL_087b: Unknown result type (might be due to invalid IL or missing references)
			//IL_088a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0899: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0622: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			Game.Net.CarLane carLane = default(Game.Net.CarLane);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<AccessLane> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<AccessLane>(ref m_AccessLaneType);
				NativeArray<RouteLane> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<RouteLane>(ref m_RouteLaneType);
				NativeArray<Game.Objects.SpawnLocation> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Objects.SpawnLocation>(ref m_SpawnLocationType);
				if (nativeArray3.Length != 0 || nativeArray4.Length != 0 || nativeArray5.Length != 0)
				{
					NativeArray<Waypoint> nativeArray6 = ((ArchetypeChunk)(ref val)).GetNativeArray<Waypoint>(ref m_WaypointType);
					NativeArray<Position> nativeArray7 = ((ArchetypeChunk)(ref val)).GetNativeArray<Position>(ref m_PositionType);
					NativeArray<Connected> nativeArray8 = ((ArchetypeChunk)(ref val)).GetNativeArray<Connected>(ref m_ConnectedType);
					NativeArray<Transform> nativeArray9 = ((ArchetypeChunk)(ref val)).GetNativeArray<Transform>(ref m_TransformType);
					NativeArray<Game.Routes.TakeoffLocation> nativeArray10 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Routes.TakeoffLocation>(ref m_TakeoffLocationType);
					NativeArray<TaxiStand> nativeArray11 = ((ArchetypeChunk)(ref val)).GetNativeArray<TaxiStand>(ref m_TaxiStandType);
					NativeArray<WaitingPassengers> nativeArray12 = ((ArchetypeChunk)(ref val)).GetNativeArray<WaitingPassengers>(ref m_WaitingPassengersType);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Entity val2 = nativeArray[j];
						AccessLane accessLane = default(AccessLane);
						if (nativeArray3.Length != 0)
						{
							accessLane = nativeArray3[j];
						}
						Game.Objects.SpawnLocation spawnLocation = default(Game.Objects.SpawnLocation);
						if (nativeArray5.Length != 0)
						{
							spawnLocation = nativeArray5[j];
						}
						UpdateActionData updateActionData = new UpdateActionData
						{
							m_Owner = val2
						};
						Entity lane = Entity.Null;
						if (m_LaneData.HasComponent(spawnLocation.m_ConnectedLane1))
						{
							lane = spawnLocation.m_ConnectedLane1;
							updateActionData.m_StartNode = new PathNode(m_LaneData[spawnLocation.m_ConnectedLane1].m_MiddleNode, spawnLocation.m_CurvePosition1);
						}
						else if (m_LaneData.HasComponent(accessLane.m_Lane))
						{
							lane = accessLane.m_Lane;
							updateActionData.m_StartNode = new PathNode(m_LaneData[accessLane.m_Lane].m_MiddleNode, accessLane.m_CurvePos);
						}
						else if (m_TransportStopData.HasComponent(accessLane.m_Lane))
						{
							lane = accessLane.m_Lane;
							updateActionData.m_StartNode = new PathNode(accessLane.m_Lane, 2);
						}
						else
						{
							updateActionData.m_StartNode = new PathNode(val2, 2);
						}
						updateActionData.m_MiddleNode = new PathNode(val2, 1);
						if (nativeArray7.Length != 0)
						{
							updateActionData.m_Location = PathUtils.GetLocationSpecification(nativeArray7[j].m_Position);
						}
						else
						{
							updateActionData.m_Location = PathUtils.GetLocationSpecification(nativeArray9[j].m_Position);
						}
						if (nativeArray6.Length != 0)
						{
							updateActionData.m_EndNode = new PathNode(val2, 0);
							Owner owner = default(Owner);
							if (nativeArray2.Length != 0)
							{
								owner = nativeArray2[j];
							}
							Game.Routes.TransportStop transportStop = default(Game.Routes.TransportStop);
							bool isWaypoint = true;
							if (nativeArray8.Length != 0)
							{
								Connected connected = nativeArray8[j];
								if (m_TransportStopData.HasComponent(connected.m_Connected))
								{
									transportStop = m_TransportStopData[connected.m_Connected];
									isWaypoint = false;
								}
							}
							WaitingPassengers waitingPassengers = default(WaitingPassengers);
							if (nativeArray12.Length != 0)
							{
								waitingPassengers = nativeArray12[j];
							}
							TransportLine transportLine;
							TransportLineData transportLineData = GetTransportLineData(owner.m_Owner, out transportLine);
							PathfindTransportData transportLinePathfindData = GetTransportLinePathfindData(transportLineData);
							updateActionData.m_Specification = PathUtils.GetTransportStopSpecification(transportStop, transportLine, waitingPassengers, transportLineData, transportLinePathfindData, isWaypoint);
						}
						else
						{
							RouteLane routeLane = default(RouteLane);
							if (nativeArray4.Length != 0)
							{
								routeLane = nativeArray4[j];
							}
							if (m_LaneData.HasComponent(routeLane.m_EndLane))
							{
								updateActionData.m_EndNode = new PathNode(m_LaneData[routeLane.m_EndLane].m_MiddleNode, routeLane.m_EndCurvePos);
							}
							else
							{
								updateActionData.m_EndNode = new PathNode(val2, 0);
							}
							updateActionData.m_SecondaryEndNode = updateActionData.m_EndNode;
							if (nativeArray5.Length != 0)
							{
								SpawnLocationData spawnLocationData = GetSpawnLocationData(val2);
								if (spawnLocationData.m_ConnectionType != RouteConnectionType.None)
								{
									updateActionData.m_Specification = GetSpawnLocationPathSpecification(updateActionData.m_Location.m_Line.a, spawnLocationData.m_ConnectionType, spawnLocationData.m_RoadTypes, spawnLocation.m_ConnectedLane1, spawnLocation.m_CurvePosition1, 0, spawnLocation.m_AccessRestriction, spawnLocationData.m_RequireAuthorization, (spawnLocation.m_Flags & SpawnLocationFlags.AllowEnter) != 0, (spawnLocation.m_Flags & SpawnLocationFlags.AllowExit) != 0);
									if (nativeArray11.Length != 0)
									{
										updateActionData.m_EndNode = new PathNode(val2, 0);
									}
									else
									{
										RouteConnectionData routeConnectionData = GetRouteConnectionData(val2);
										if ((spawnLocationData.m_ConnectionType == RouteConnectionType.Road || spawnLocationData.m_ConnectionType == RouteConnectionType.Cargo || spawnLocationData.m_ConnectionType == RouteConnectionType.Parking) && spawnLocationData.m_ConnectionType != routeConnectionData.m_AccessConnectionType)
										{
											int laneCrossCount = 1;
											if (m_MasterLaneData.HasComponent(spawnLocation.m_ConnectedLane1))
											{
												MasterLane masterLane = m_MasterLaneData[spawnLocation.m_ConnectedLane1];
												laneCrossCount = masterLane.m_MaxIndex - masterLane.m_MinIndex + 1;
											}
											bool flag = false;
											if (m_LaneData.HasComponent(spawnLocation.m_ConnectedLane2))
											{
												updateActionData.m_SecondaryStartNode = new PathNode(m_LaneData[spawnLocation.m_ConnectedLane2].m_MiddleNode, spawnLocation.m_CurvePosition2);
												updateActionData.m_SecondaryEndNode = updateActionData.m_EndNode;
											}
											else
											{
												flag = true;
												updateActionData.m_SecondaryStartNode = new PathNode(val2, 3);
												updateActionData.m_SecondaryEndNode = updateActionData.m_EndNode;
											}
											updateActionData.m_SecondarySpecification = GetSpawnLocationPathSpecification(updateActionData.m_Location.m_Line.a, spawnLocationData.m_ConnectionType, spawnLocationData.m_RoadTypes, spawnLocation.m_ConnectedLane2, spawnLocation.m_CurvePosition2, laneCrossCount, spawnLocation.m_AccessRestriction, spawnLocationData.m_RequireAuthorization, (spawnLocation.m_Flags & SpawnLocationFlags.AllowEnter) != 0, (spawnLocation.m_Flags & SpawnLocationFlags.AllowExit) != 0);
											if (flag)
											{
												updateActionData.m_SecondarySpecification.m_Flags &= ~(EdgeFlags.Forward | EdgeFlags.Backward);
											}
										}
									}
								}
							}
							else if (nativeArray10.Length != 0)
							{
								Game.Routes.TakeoffLocation takeoffLocation = nativeArray10[j];
								RouteConnectionData routeConnectionData2 = GetRouteConnectionData(val2);
								updateActionData.m_Specification = GetSpawnLocationPathSpecification(updateActionData.m_Location.m_Line.a, routeConnectionData2.m_RouteConnectionType, routeConnectionData2.m_RouteRoadType, routeLane.m_EndLane, routeLane.m_EndCurvePos, 0, takeoffLocation.m_AccessRestriction, requireAuthorization: false, (takeoffLocation.m_Flags & TakeoffLocationFlags.AllowEnter) != 0, (takeoffLocation.m_Flags & TakeoffLocationFlags.AllowExit) != 0);
								if (routeConnectionData2.m_RouteConnectionType == RouteConnectionType.Air && routeConnectionData2.m_RouteRoadType == RoadTypes.Airplane && m_CarLaneData.TryGetComponent(accessLane.m_Lane, ref carLane) && (carLane.m_Flags & CarLaneFlags.Twoway) == 0)
								{
									updateActionData.m_Specification.m_Flags &= (EdgeFlags)((accessLane.m_CurvePos >= 0.5f) ? 65533 : 65534);
								}
							}
							if (nativeArray11.Length != 0)
							{
								TaxiStand taxiStand = nativeArray11[j];
								Game.Routes.TransportStop transportStop2 = default(Game.Routes.TransportStop);
								if (m_TransportStopData.HasComponent(val2))
								{
									transportStop2 = m_TransportStopData[val2];
								}
								WaitingPassengers waitingPassengers2 = default(WaitingPassengers);
								if (nativeArray12.Length != 0)
								{
									waitingPassengers2 = nativeArray12[j];
								}
								PathfindTransportData netLaneTransportPathfindData = GetNetLaneTransportPathfindData(lane);
								updateActionData.m_SecondaryStartNode = updateActionData.m_StartNode;
								updateActionData.m_SecondarySpecification = PathUtils.GetTaxiStopSpecification(transportStop2, taxiStand, waitingPassengers2, netLaneTransportPathfindData);
							}
						}
						m_Actions[num++] = updateActionData;
					}
				}
				NativeArray<Game.Routes.Segment> nativeArray13 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Routes.Segment>(ref m_SegmentType);
				if (nativeArray13.Length == 0)
				{
					continue;
				}
				NativeArray<RouteInfo> nativeArray14 = ((ArchetypeChunk)(ref val)).GetNativeArray<RouteInfo>(ref m_RouteInfoType);
				for (int k = 0; k < nativeArray13.Length; k++)
				{
					Entity owner2 = nativeArray[k];
					Owner owner3 = nativeArray2[k];
					Game.Routes.Segment segment = nativeArray13[k];
					RouteInfo routeInfo = default(RouteInfo);
					if (nativeArray14.Length != 0)
					{
						routeInfo = nativeArray14[k];
					}
					DynamicBuffer<RouteWaypoint> val3 = m_Waypoints[owner3.m_Owner];
					int num2 = math.select(segment.m_Index + 1, 0, segment.m_Index == val3.Length - 1);
					Entity waypoint = val3[segment.m_Index].m_Waypoint;
					Entity waypoint2 = val3[num2].m_Waypoint;
					Position position = m_PositionData[waypoint];
					Position position2 = m_PositionData[waypoint2];
					TransportLine transportLine2;
					TransportLineData transportLineData2 = GetTransportLineData(owner3.m_Owner, out transportLine2);
					PathfindTransportData transportLinePathfindData2 = GetTransportLinePathfindData(transportLineData2);
					UpdateActionData updateActionData2 = new UpdateActionData
					{
						m_Owner = owner2,
						m_StartNode = new PathNode(waypoint, 0),
						m_MiddleNode = new PathNode(owner2, 0),
						m_EndNode = new PathNode(waypoint2, 0),
						m_Specification = PathUtils.GetTransportLineSpecification(transportLineData2, transportLinePathfindData2, routeInfo),
						m_Location = PathUtils.GetLocationSpecification(position.m_Position, position2.m_Position)
					};
					m_Actions[num++] = updateActionData2;
				}
			}
		}

		private SpawnLocationData GetSpawnLocationData(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[entity];
			if (m_PrefabSpawnLocationData.HasComponent(prefabRef.m_Prefab))
			{
				return m_PrefabSpawnLocationData[prefabRef.m_Prefab];
			}
			return default(SpawnLocationData);
		}

		private RouteConnectionData GetRouteConnectionData(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[entity];
			if (m_PrefabRouteConnectionData.HasComponent(prefabRef.m_Prefab))
			{
				return m_PrefabRouteConnectionData[prefabRef.m_Prefab];
			}
			return default(RouteConnectionData);
		}

		private TransportLineData GetTransportLineData(Entity owner, out TransportLine transportLine)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (m_TransportLineData.HasComponent(owner))
			{
				transportLine = m_TransportLineData[owner];
				PrefabRef prefabRef = m_PrefabRefData[owner];
				return m_PrefabTransportLineData[prefabRef.m_Prefab];
			}
			transportLine = default(TransportLine);
			return default(TransportLineData);
		}

		private PathfindTransportData GetTransportLinePathfindData(TransportLineData transportLineData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_TransportPathfindData.HasComponent(transportLineData.m_PathfindPrefab))
			{
				return m_TransportPathfindData[transportLineData.m_PathfindPrefab];
			}
			return default(PathfindTransportData);
		}

		private PathfindTransportData GetNetLaneTransportPathfindData(Entity lane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			if (m_PrefabRefData.HasComponent(lane))
			{
				PrefabRef prefabRef = m_PrefabRefData[lane];
				if (m_NetLaneData.HasComponent(prefabRef.m_Prefab))
				{
					NetLaneData netLaneData = m_NetLaneData[prefabRef.m_Prefab];
					if (m_TransportPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
					{
						return m_TransportPathfindData[netLaneData.m_PathfindPrefab];
					}
				}
			}
			return default(PathfindTransportData);
		}

		private PathfindConnectionData GetNetLaneConnectionPathfindData(Entity lane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			if (m_PrefabRefData.HasComponent(lane))
			{
				PrefabRef prefabRef = m_PrefabRefData[lane];
				if (m_NetLaneData.HasComponent(prefabRef.m_Prefab))
				{
					NetLaneData netLaneData = m_NetLaneData[prefabRef.m_Prefab];
					if (m_ConnectionPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
					{
						return m_ConnectionPathfindData[netLaneData.m_PathfindPrefab];
					}
				}
			}
			return default(PathfindConnectionData);
		}

		private PathSpecification GetSpawnLocationPathSpecification(float3 position, RouteConnectionType connectionType, RoadTypes roadType, Entity lane, float curvePos, int laneCrossCount, Entity accessRestriction, bool requireAuthorization, bool allowEnter, bool allowExit)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			NetLaneData netLaneData = default(NetLaneData);
			if (m_PrefabRefData.HasComponent(lane))
			{
				PrefabRef prefabRef = m_PrefabRefData[lane];
				if (m_NetLaneData.HasComponent(prefabRef.m_Prefab))
				{
					netLaneData = m_NetLaneData[prefabRef.m_Prefab];
				}
			}
			switch (connectionType)
			{
			case RouteConnectionType.Pedestrian:
			{
				float distance2 = 0f;
				if (m_CurveData.HasComponent(lane))
				{
					distance2 = math.distance(position, MathUtils.Position(m_CurveData[lane].m_Bezier, curvePos));
				}
				if (m_ConnectionPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					PathfindConnectionData connectionPathfindData3 = m_ConnectionPathfindData[netLaneData.m_PathfindPrefab];
					return PathUtils.GetSpawnLocationSpecification(connectionType, connectionPathfindData3, roadType, distance2, accessRestriction, requireAuthorization, allowEnter, allowExit);
				}
				PathfindPedestrianData pedestrianPathfindData = default(PathfindPedestrianData);
				if (m_PedestrianPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					pedestrianPathfindData = m_PedestrianPathfindData[netLaneData.m_PathfindPrefab];
				}
				return PathUtils.GetSpawnLocationSpecification(pedestrianPathfindData, distance2, accessRestriction, requireAuthorization, allowEnter, allowExit);
			}
			case RouteConnectionType.Road:
			case RouteConnectionType.Cargo:
			case RouteConnectionType.Parking:
			{
				float distance = 0f;
				if (m_CurveData.HasComponent(lane))
				{
					distance = math.distance(position, MathUtils.Position(m_CurveData[lane].m_Bezier, curvePos));
				}
				if (m_ConnectionPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					PathfindConnectionData connectionPathfindData2 = m_ConnectionPathfindData[netLaneData.m_PathfindPrefab];
					return PathUtils.GetSpawnLocationSpecification(connectionType, connectionPathfindData2, roadType, distance, accessRestriction, requireAuthorization, allowEnter, allowExit);
				}
				Game.Net.CarLane carLane = default(Game.Net.CarLane);
				if (m_CarLaneData.HasComponent(lane))
				{
					carLane = m_CarLaneData[lane];
				}
				else
				{
					carLane.m_SpeedLimit = 277.77777f;
				}
				PathfindCarData carPathfindData = default(PathfindCarData);
				if (m_CarPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					carPathfindData = m_CarPathfindData[netLaneData.m_PathfindPrefab];
				}
				return PathUtils.GetSpawnLocationSpecification(connectionType, carPathfindData, carLane, distance, laneCrossCount, accessRestriction, requireAuthorization, allowEnter, allowExit);
			}
			case RouteConnectionType.Track:
			{
				if (m_ConnectionPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					PathfindConnectionData connectionPathfindData4 = m_ConnectionPathfindData[netLaneData.m_PathfindPrefab];
					return PathUtils.GetSpawnLocationSpecification(connectionType, connectionPathfindData4, roadType, 0f, accessRestriction, requireAuthorization, allowEnter, allowExit);
				}
				PathfindTrackData trackPathfindData = default(PathfindTrackData);
				if (m_TrackPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					trackPathfindData = m_TrackPathfindData[netLaneData.m_PathfindPrefab];
				}
				return PathUtils.GetSpawnLocationSpecification(trackPathfindData, accessRestriction);
			}
			case RouteConnectionType.Air:
			{
				PathfindConnectionData connectionPathfindData = default(PathfindConnectionData);
				if (m_ConnectionPathfindData.HasComponent(netLaneData.m_PathfindPrefab))
				{
					connectionPathfindData = m_ConnectionPathfindData[netLaneData.m_PathfindPrefab];
				}
				return PathUtils.GetSpawnLocationSpecification(connectionType, connectionPathfindData, roadType, 0f, accessRestriction, requireAuthorization, allowEnter, allowExit);
			}
			default:
				return default(PathSpecification);
			}
		}
	}

	[BurstCompile]
	private struct RemovePathEdgeJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[WriteOnly]
		public NativeArray<DeleteActionData> m_Actions;

		public void Execute()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					DeleteActionData deleteActionData = new DeleteActionData
					{
						m_Owner = nativeArray[j]
					};
					m_Actions[num++] = deleteActionData;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Waypoint> __Game_Routes_Waypoint_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Position> __Game_Routes_Position_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AccessLane> __Game_Routes_AccessLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RouteLane> __Game_Routes_RouteLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.Segment> __Game_Routes_Segment_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TaxiStand> __Game_Routes_TaxiStand_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.TakeoffLocation> __Game_Routes_TakeoffLocation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Connected> __Game_Routes_Connected_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RouteInfo> __Game_Routes_RouteInfo_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaitingPassengers> __Game_Routes_WaitingPassengers_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TransportStop> __Game_Routes_TransportStop_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportLine> __Game_Routes_TransportLine_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportLineData> __Game_Prefabs_TransportLineData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> __Game_Prefabs_RouteConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindTransportData> __Game_Prefabs_PathfindTransportData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindPedestrianData> __Game_Prefabs_PathfindPedestrianData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindCarData> __Game_Prefabs_PathfindCarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindTrackData> __Game_Prefabs_PathfindTrackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindConnectionData> __Game_Prefabs_PathfindConnectionData_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Routes_Waypoint_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Waypoint>(true);
			__Game_Routes_Position_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Position>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Routes_AccessLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AccessLane>(true);
			__Game_Routes_RouteLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RouteLane>(true);
			__Game_Routes_Segment_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.Segment>(true);
			__Game_Routes_TaxiStand_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TaxiStand>(true);
			__Game_Routes_TakeoffLocation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.TakeoffLocation>(true);
			__Game_Routes_Connected_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Connected>(true);
			__Game_Routes_RouteInfo_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RouteInfo>(true);
			__Game_Objects_SpawnLocation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.SpawnLocation>(true);
			__Game_Routes_WaitingPassengers_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaitingPassengers>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Routes_TransportStop_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.TransportStop>(true);
			__Game_Routes_TransportLine_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLine>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_TransportLineData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLineData>(true);
			__Game_Prefabs_RouteConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteConnectionData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Prefabs_PathfindTransportData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindTransportData>(true);
			__Game_Prefabs_PathfindPedestrianData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindPedestrianData>(true);
			__Game_Prefabs_PathfindCarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindCarData>(true);
			__Game_Prefabs_PathfindTrackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindTrackData>(true);
			__Game_Prefabs_PathfindConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindConnectionData>(true);
		}
	}

	private PathfindQueueSystem m_PathfindQueueSystem;

	private EntityQuery m_CreatedSubElementQuery;

	private EntityQuery m_UpdatedSubElementQuery;

	private EntityQuery m_DeletedSubElementQuery;

	private EntityQuery m_AllSubElementQuery;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Expected O, but got Unknown
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Expected O, but got Unknown
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PathfindQueueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindQueueSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Created>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<AccessLane>(),
			ComponentType.ReadOnly<RouteLane>(),
			ComponentType.ReadOnly<Game.Routes.Segment>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Routes.MailBox>(),
			ComponentType.ReadOnly<LivePath>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_CreatedSubElementQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[2];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<AccessLane>(),
			ComponentType.ReadOnly<RouteLane>(),
			ComponentType.ReadOnly<Game.Routes.Segment>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Game.Routes.MailBox>(),
			ComponentType.ReadOnly<LivePath>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PathfindUpdated>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<AccessLane>(),
			ComponentType.ReadOnly<RouteLane>(),
			ComponentType.ReadOnly<Game.Routes.Segment>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Game.Routes.MailBox>(),
			ComponentType.ReadOnly<LivePath>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[1] = val;
		m_UpdatedSubElementQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<AccessLane>(),
			ComponentType.ReadOnly<RouteLane>(),
			ComponentType.ReadOnly<Game.Routes.Segment>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Routes.MailBox>(),
			ComponentType.ReadOnly<LivePath>(),
			ComponentType.ReadOnly<Temp>()
		};
		array3[0] = val;
		m_DeletedSubElementQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		EntityQueryDesc[] array4 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<AccessLane>(),
			ComponentType.ReadOnly<RouteLane>(),
			ComponentType.ReadOnly<Game.Routes.Segment>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Routes.MailBox>(),
			ComponentType.ReadOnly<LivePath>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array4[0] = val;
		m_AllSubElementQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array4);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_0852: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_085e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_0862: Unknown result type (might be due to invalid IL or missing references)
		//IL_0867: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_091d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0901: Unknown result type (might be due to invalid IL or missing references)
		//IL_0905: Unknown result type (might be due to invalid IL or missing references)
		//IL_0907: Unknown result type (might be due to invalid IL or missing references)
		//IL_0915: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val;
		int num;
		if (GetLoaded())
		{
			val = m_AllSubElementQuery;
			num = 0;
		}
		else
		{
			val = m_CreatedSubElementQuery;
			num = ((EntityQuery)(ref m_UpdatedSubElementQuery)).CalculateEntityCount();
		}
		int num2 = ((EntityQuery)(ref val)).CalculateEntityCount();
		int num3 = ((EntityQuery)(ref m_DeletedSubElementQuery)).CalculateEntityCount();
		if (num2 != 0 || num != 0 || num3 != 0)
		{
			JobHandle val2 = ((SystemBase)this).Dependency;
			if (num2 != 0)
			{
				CreateAction action = new CreateAction(num2, (Allocator)4);
				JobHandle val3 = default(JobHandle);
				NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref val)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3);
				JobHandle val4 = IJobExtensions.Schedule<AddPathEdgeJob>(new AddPathEdgeJob
				{
					m_Chunks = chunks,
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_WaypointType = InternalCompilerInterface.GetComponentTypeHandle<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PositionType = InternalCompilerInterface.GetComponentTypeHandle<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_AccessLaneType = InternalCompilerInterface.GetComponentTypeHandle<AccessLane>(ref __TypeHandle.__Game_Routes_AccessLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_RouteLaneType = InternalCompilerInterface.GetComponentTypeHandle<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_SegmentType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TaxiStandType = InternalCompilerInterface.GetComponentTypeHandle<TaxiStand>(ref __TypeHandle.__Game_Routes_TaxiStand_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TakeoffLocationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.TakeoffLocation>(ref __TypeHandle.__Game_Routes_TakeoffLocation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectedType = InternalCompilerInterface.GetComponentTypeHandle<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_RouteInfoType = InternalCompilerInterface.GetComponentTypeHandle<RouteInfo>(ref __TypeHandle.__Game_Routes_RouteInfo_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_SpawnLocationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_WaitingPassengersType = InternalCompilerInterface.GetComponentTypeHandle<WaitingPassengers>(ref __TypeHandle.__Game_Routes_WaitingPassengers_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TransportStopData = InternalCompilerInterface.GetComponentLookup<Game.Routes.TransportStop>(ref __TypeHandle.__Game_Routes_TransportStop_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLine>(ref __TypeHandle.__Game_Routes_TransportLine_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Waypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_NetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabTransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRouteConnectionData = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TransportPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindTransportData>(ref __TypeHandle.__Game_Prefabs_PathfindTransportData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PedestrianPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindPedestrianData>(ref __TypeHandle.__Game_Prefabs_PathfindPedestrianData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CarPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindCarData>(ref __TypeHandle.__Game_Prefabs_PathfindCarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TrackPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindTrackData>(ref __TypeHandle.__Game_Prefabs_PathfindTrackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectionPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindConnectionData>(ref __TypeHandle.__Game_Prefabs_PathfindConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Actions = action.m_CreateData
				}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3));
				val2 = JobHandle.CombineDependencies(val2, val4);
				chunks.Dispose(val4);
				m_PathfindQueueSystem.Enqueue(action, val4);
			}
			if (num != 0)
			{
				UpdateAction action2 = new UpdateAction(num, (Allocator)4);
				JobHandle val5 = default(JobHandle);
				NativeList<ArchetypeChunk> chunks2 = ((EntityQuery)(ref m_UpdatedSubElementQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val5);
				JobHandle val6 = IJobExtensions.Schedule<UpdatePathEdgeJob>(new UpdatePathEdgeJob
				{
					m_Chunks = chunks2,
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_WaypointType = InternalCompilerInterface.GetComponentTypeHandle<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PositionType = InternalCompilerInterface.GetComponentTypeHandle<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_AccessLaneType = InternalCompilerInterface.GetComponentTypeHandle<AccessLane>(ref __TypeHandle.__Game_Routes_AccessLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_RouteLaneType = InternalCompilerInterface.GetComponentTypeHandle<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_SegmentType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TaxiStandType = InternalCompilerInterface.GetComponentTypeHandle<TaxiStand>(ref __TypeHandle.__Game_Routes_TaxiStand_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TakeoffLocationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.TakeoffLocation>(ref __TypeHandle.__Game_Routes_TakeoffLocation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectedType = InternalCompilerInterface.GetComponentTypeHandle<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_RouteInfoType = InternalCompilerInterface.GetComponentTypeHandle<RouteInfo>(ref __TypeHandle.__Game_Routes_RouteInfo_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_SpawnLocationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_WaitingPassengersType = InternalCompilerInterface.GetComponentTypeHandle<WaitingPassengers>(ref __TypeHandle.__Game_Routes_WaitingPassengers_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TransportStopData = InternalCompilerInterface.GetComponentLookup<Game.Routes.TransportStop>(ref __TypeHandle.__Game_Routes_TransportStop_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLine>(ref __TypeHandle.__Game_Routes_TransportLine_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Waypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_NetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabTransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRouteConnectionData = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TransportPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindTransportData>(ref __TypeHandle.__Game_Prefabs_PathfindTransportData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PedestrianPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindPedestrianData>(ref __TypeHandle.__Game_Prefabs_PathfindPedestrianData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CarPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindCarData>(ref __TypeHandle.__Game_Prefabs_PathfindCarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TrackPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindTrackData>(ref __TypeHandle.__Game_Prefabs_PathfindTrackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectionPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindConnectionData>(ref __TypeHandle.__Game_Prefabs_PathfindConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Actions = action2.m_UpdateData
				}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val5));
				val2 = JobHandle.CombineDependencies(val2, val6);
				chunks2.Dispose(val6);
				m_PathfindQueueSystem.Enqueue(action2, val6);
			}
			if (num3 != 0)
			{
				DeleteAction action3 = new DeleteAction(num3, (Allocator)4);
				JobHandle val7 = default(JobHandle);
				NativeList<ArchetypeChunk> chunks3 = ((EntityQuery)(ref m_DeletedSubElementQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val7);
				JobHandle val8 = IJobExtensions.Schedule<RemovePathEdgeJob>(new RemovePathEdgeJob
				{
					m_Chunks = chunks3,
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_Actions = action3.m_DeleteData
				}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val7));
				val2 = JobHandle.CombineDependencies(val2, val8);
				chunks3.Dispose(val8);
				m_PathfindQueueSystem.Enqueue(action3, val8);
			}
			((SystemBase)this).Dependency = val2;
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
	public RoutesModifiedSystem()
	{
	}
}

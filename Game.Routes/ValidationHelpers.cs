using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Routes;

public static class ValidationHelpers
{
	public static void ValidateRoute(Entity entity, Temp temp, PrefabRef prefabRef, DynamicBuffer<RouteWaypoint> waypoints, DynamicBuffer<RouteSegment> segments, ValidationSystem.EntityData data, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		TransportLineData transportLineData = default(TransportLineData);
		if (data.m_TransportLineData.TryGetComponent(prefabRef.m_Prefab, ref transportLineData))
		{
			Connected connected = default(Connected);
			PrefabRef prefabRef2 = default(PrefabRef);
			TransportStopData transportStopData = default(TransportStopData);
			for (int i = 0; i < waypoints.Length; i++)
			{
				Entity waypoint = waypoints[i].m_Waypoint;
				if (data.m_RouteConnected.TryGetComponent(waypoint, ref connected) && data.m_PrefabRef.TryGetComponent(connected.m_Connected, ref prefabRef2) && data.m_TransportStopData.TryGetComponent(prefabRef2.m_Prefab, ref transportStopData))
				{
					if (transportLineData.m_PassengerTransport & !transportStopData.m_PassengerTransport)
					{
						errorQueue.Enqueue(new ErrorData
						{
							m_ErrorSeverity = ErrorSeverity.Error,
							m_ErrorType = ErrorType.NoPedestrianAccess,
							m_Position = float3.op_Implicit(float.NaN),
							m_TempEntity = waypoint
						});
					}
					if (transportLineData.m_CargoTransport & !transportStopData.m_CargoTransport)
					{
						errorQueue.Enqueue(new ErrorData
						{
							m_ErrorSeverity = ErrorSeverity.Error,
							m_ErrorType = ErrorType.NoCargoAccess,
							m_Position = float3.op_Implicit(float.NaN),
							m_TempEntity = waypoint
						});
					}
				}
			}
		}
		bool flag = false;
		Route route = default(Route);
		if (data.m_Route.TryGetComponent(temp.m_Original, ref route))
		{
			flag = (route.m_Flags & RouteFlags.Complete) != 0;
		}
		for (int j = 0; j < segments.Length; j++)
		{
			Entity segment = segments[j].m_Segment;
			if (data.m_PathInformation.HasComponent(segment) && data.m_PathInformation[segment].m_Distance < 0f)
			{
				Entity waypoint2 = waypoints[j].m_Waypoint;
				Entity waypoint3 = waypoints[math.select(j + 1, 0, j + 1 == waypoints.Length)].m_Waypoint;
				ErrorData errorData = new ErrorData
				{
					m_ErrorSeverity = (flag ? ErrorSeverity.Warning : ErrorSeverity.Error),
					m_ErrorType = ErrorType.PathfindFailed,
					m_Position = float3.op_Implicit(float.NaN),
					m_TempEntity = waypoint2
				};
				errorQueue.Enqueue(errorData);
				errorData.m_TempEntity = waypoint3;
				errorQueue.Enqueue(errorData);
			}
		}
	}

	public static void ValidateStop(bool editorMode, Entity entity, Temp temp, Owner owner, Transform transform, PrefabRef prefabRef, Attached attached, ValidationSystem.EntityData data, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		if ((temp.m_Flags & (TempFlags.Create | TempFlags.Modify)) == 0)
		{
			return;
		}
		PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
		if (data.m_PlaceableObject.HasComponent(prefabRef.m_Prefab))
		{
			placeableObjectData = data.m_PlaceableObject[prefabRef.m_Prefab];
		}
		if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.NetObject) == 0 || (attached.m_Parent != Entity.Null && owner.m_Owner != Entity.Null))
		{
			return;
		}
		RouteConnectionData connectionData = data.m_RouteConnectionData[prefabRef.m_Prefab];
		bool2 val = FindStopLanes(attached, connectionData, data);
		if (math.all(val))
		{
			return;
		}
		ErrorData errorData = default(ErrorData);
		if (editorMode)
		{
			errorData.m_ErrorSeverity = ErrorSeverity.Warning;
		}
		else
		{
			errorData.m_ErrorSeverity = ErrorSeverity.Error;
		}
		if (!math.any(val))
		{
			if (!editorMode && connectionData.m_RouteConnectionType == RouteConnectionType.Road && connectionData.m_AccessConnectionType == RouteConnectionType.Pedestrian)
			{
				errorData.m_ErrorType = ErrorType.NoRoadAccess;
			}
			else if (!editorMode && connectionData.m_RouteConnectionType == RouteConnectionType.Track && connectionData.m_AccessConnectionType == RouteConnectionType.Pedestrian)
			{
				errorData.m_ErrorType = ErrorType.NoTrackAccess;
			}
			else
			{
				errorData.m_ErrorType = RouteConnectionToError(connectionData.m_RouteConnectionType);
			}
		}
		else if (val.x)
		{
			errorData.m_ErrorType = RouteConnectionToError(connectionData.m_AccessConnectionType);
		}
		else
		{
			errorData.m_ErrorType = RouteConnectionToError(connectionData.m_RouteConnectionType);
		}
		errorData.m_TempEntity = entity;
		errorData.m_Position = float3.op_Implicit(float.NaN);
		errorQueue.Enqueue(errorData);
	}

	private static ErrorType RouteConnectionToError(RouteConnectionType type)
	{
		return type switch
		{
			RouteConnectionType.Road => ErrorType.NoCarAccess, 
			RouteConnectionType.Track => ErrorType.NoTrainAccess, 
			RouteConnectionType.Pedestrian => ErrorType.NoPedestrianAccess, 
			RouteConnectionType.Cargo => ErrorType.NoCargoAccess, 
			_ => ErrorType.None, 
		};
	}

	private static bool2 FindStopLanes(Attached attached, RouteConnectionData connectionData, ValidationSystem.EntityData data)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		bool2 val = new bool2
		{
			x = (connectionData.m_RouteConnectionType == RouteConnectionType.None),
			y = (connectionData.m_AccessConnectionType == RouteConnectionType.None)
		};
		if (!data.m_Lanes.HasBuffer(attached.m_Parent))
		{
			return val;
		}
		DynamicBuffer<Game.Net.SubLane> val2 = data.m_Lanes[attached.m_Parent];
		bool2 val3 = new bool2
		{
			x = (connectionData.m_RouteConnectionType == RouteConnectionType.Road),
			y = (connectionData.m_AccessConnectionType == RouteConnectionType.Road)
		};
		bool2 val4 = new bool2
		{
			x = (connectionData.m_RouteConnectionType == RouteConnectionType.Track),
			y = (connectionData.m_AccessConnectionType == RouteConnectionType.Track)
		};
		bool2 val5 = new bool2
		{
			x = (connectionData.m_RouteConnectionType == RouteConnectionType.Pedestrian),
			y = (connectionData.m_AccessConnectionType == RouteConnectionType.Pedestrian)
		};
		for (int i = 0; i < val2.Length; i++)
		{
			Entity subLane = val2[i].m_SubLane;
			if (math.any(val3) && data.m_CarLane.HasComponent(subLane))
			{
				PrefabRef prefabRef = data.m_PrefabRef[subLane];
				CarLaneData carLaneData = data.m_CarLaneData[prefabRef.m_Prefab];
				bool2 val6 = new bool2
				{
					x = ((carLaneData.m_RoadTypes & connectionData.m_RouteRoadType) != 0),
					y = ((carLaneData.m_RoadTypes & connectionData.m_AccessRoadType) != 0)
				};
				val |= val3 & val6;
			}
			if (math.any(val4) && data.m_TrackLane.HasComponent(subLane))
			{
				PrefabRef prefabRef2 = data.m_PrefabRef[subLane];
				TrackLaneData trackLaneData = data.m_TrackLaneData[prefabRef2.m_Prefab];
				bool2 val7 = new bool2
				{
					x = ((trackLaneData.m_TrackTypes & connectionData.m_RouteTrackType) != 0),
					y = ((trackLaneData.m_TrackTypes & connectionData.m_AccessTrackType) != 0)
				};
				val |= val4 & val7;
			}
			if (math.any(val5) && data.m_PedestrianLane.HasComponent(subLane))
			{
				val |= val5;
			}
			if (math.all(val))
			{
				return val;
			}
		}
		return val;
	}
}

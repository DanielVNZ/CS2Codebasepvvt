using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Audio;
using Game.Common;
using Game.Input;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class RouteToolSystem : ToolBaseSystem
{
	public enum State
	{
		Default,
		Create,
		Modify,
		Remove
	}

	public enum Tooltip
	{
		None,
		CreateRoute,
		ModifyWaypoint,
		ModifySegment,
		CreateOrModify,
		AddWaypoint,
		InsertWaypoint,
		MoveWaypoint,
		MergeWaypoints,
		CompleteRoute,
		DeleteRoute,
		RemoveWaypoint
	}

	[BurstCompile]
	private struct SnapJob : IJob
	{
		[ReadOnly]
		public Snap m_Snap;

		[ReadOnly]
		public State m_State;

		[ReadOnly]
		public Entity m_Prefab;

		[ReadOnly]
		public Entity m_ApplyTempRoute;

		[ReadOnly]
		public ControlPoint m_MoveStartPosition;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RouteData> m_PrefabRouteData;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_PrefabTransportLineData;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> m_PrefabRouteConnectionData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

		[ReadOnly]
		public ComponentLookup<TransportStopData> m_PrefabTransportStopData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> m_ConnectedRoutes;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_Waypoints;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		public NativeList<ControlPoint> m_ControlPoints;

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Prefab;
			int num = m_ControlPoints.Length - 1;
			ControlPoint currentPoint = m_ControlPoints[num];
			if ((m_State == State.Modify || m_State == State.Remove) && m_Waypoints.HasBuffer(m_MoveStartPosition.m_OriginalEntity))
			{
				val = m_PrefabRefData[m_MoveStartPosition.m_OriginalEntity].m_Prefab;
			}
			RouteData routeData = m_PrefabRouteData[val];
			TransportLineData transportLineData = default(TransportLineData);
			m_PrefabTransportLineData.TryGetComponent(val, ref transportLineData);
			RouteConnectionData routeConnectionData = default(RouteConnectionData);
			m_PrefabRouteConnectionData.TryGetComponent(val, ref routeConnectionData);
			if (routeData.m_Type == RouteType.WorkRoute)
			{
				transportLineData.m_TransportType = TransportType.Work;
			}
			switch (m_State)
			{
			case State.Default:
			{
				if (!FindWaypointLocation(routeData, transportLineData, routeConnectionData, ref currentPoint) || !(m_ApplyTempRoute != Entity.Null) || m_ConnectedRoutes.HasBuffer(currentPoint.m_OriginalEntity))
				{
					break;
				}
				Temp temp = m_TempData[m_ApplyTempRoute];
				if ((temp.m_Flags & TempFlags.Delete) != 0)
				{
					if (temp.m_Original != Entity.Null && currentPoint.m_OriginalEntity == temp.m_Original)
					{
						currentPoint.m_OriginalEntity = Entity.Null;
					}
					break;
				}
				if (temp.m_Original != Entity.Null && currentPoint.m_OriginalEntity == temp.m_Original)
				{
					if (currentPoint.m_ElementIndex.x < 0)
					{
						break;
					}
					DynamicBuffer<RouteWaypoint> val3 = m_Waypoints[temp.m_Original];
					DynamicBuffer<RouteWaypoint> val4 = m_Waypoints[m_ApplyTempRoute];
					float3 position3 = m_PositionData[val3[currentPoint.m_ElementIndex.x].m_Waypoint].m_Position;
					currentPoint.m_ElementIndex.x = -1;
					for (int i = 0; i < val4.Length; i++)
					{
						Position position4 = m_PositionData[val4[i].m_Waypoint];
						if (((float3)(ref position4.m_Position)).Equals(position3))
						{
							currentPoint.m_ElementIndex.x = i;
							break;
						}
					}
					break;
				}
				DynamicBuffer<RouteWaypoint> val5 = m_Waypoints[m_ApplyTempRoute];
				for (int j = 0; j < val5.Length; j++)
				{
					float3 position5 = m_PositionData[val5[j].m_Waypoint].m_Position;
					if (math.distance(position5, currentPoint.m_Position) < routeData.m_SnapDistance)
					{
						currentPoint.m_Position = position5;
						currentPoint.m_OriginalEntity = ((temp.m_Original != Entity.Null) ? temp.m_Original : m_ApplyTempRoute);
						currentPoint.m_ElementIndex = new int2(j, -1);
						break;
					}
				}
				break;
			}
			case State.Create:
				if (FindWaypointLocation(routeData, transportLineData, routeConnectionData, ref currentPoint) && m_ControlPoints.Length >= 3)
				{
					ControlPoint controlPoint = m_ControlPoints[0];
					if (math.distance(controlPoint.m_Position, currentPoint.m_Position) < routeData.m_SnapDistance)
					{
						currentPoint.m_Position = controlPoint.m_Position;
					}
				}
				break;
			case State.Modify:
				if (FindWaypointLocation(routeData, transportLineData, routeConnectionData, ref currentPoint) && m_Waypoints.HasBuffer(m_MoveStartPosition.m_OriginalEntity) && m_MoveStartPosition.m_ElementIndex.x >= 0)
				{
					DynamicBuffer<RouteWaypoint> val2 = m_Waypoints[m_MoveStartPosition.m_OriginalEntity];
					int num2 = math.select(m_MoveStartPosition.m_ElementIndex.x - 1, val2.Length - 1, m_MoveStartPosition.m_ElementIndex.x == 0);
					int num3 = math.select(m_MoveStartPosition.m_ElementIndex.x + 1, 0, m_MoveStartPosition.m_ElementIndex.x == val2.Length - 1);
					float3 position = m_PositionData[val2[num2].m_Waypoint].m_Position;
					float3 position2 = m_PositionData[val2[num3].m_Waypoint].m_Position;
					float num4 = math.distance(currentPoint.m_Position, position);
					float num5 = math.distance(currentPoint.m_Position, position2);
					if (num4 < routeData.m_SnapDistance && num4 <= num5)
					{
						currentPoint.m_Position = position;
					}
					else if (num5 < routeData.m_SnapDistance)
					{
						currentPoint.m_Position = position2;
					}
				}
				break;
			case State.Remove:
				if (FindWaypointLocation(routeData, transportLineData, routeConnectionData, ref currentPoint) && (currentPoint.m_OriginalEntity != m_MoveStartPosition.m_OriginalEntity || math.any(currentPoint.m_ElementIndex != m_MoveStartPosition.m_ElementIndex)))
				{
					currentPoint = m_MoveStartPosition;
					currentPoint.m_OriginalEntity = Entity.Null;
				}
				break;
			}
			currentPoint.m_HitPosition = currentPoint.m_Position;
			currentPoint.m_CurvePosition = 0f;
			m_ControlPoints[num] = currentPoint;
		}

		private bool ValidateStop(TransportLineData transportLineData, Entity stopEntity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = default(PrefabRef);
			TransportStopData transportStopData = default(TransportStopData);
			if (m_PrefabRefData.TryGetComponent(stopEntity, ref prefabRef) && m_PrefabTransportStopData.TryGetComponent(prefabRef.m_Prefab, ref transportStopData))
			{
				return transportStopData.m_TransportType == transportLineData.m_TransportType;
			}
			return false;
		}

		private bool FindWaypointLocation(RouteData routeData, TransportLineData transportLineData, RouteConnectionData routeConnectionData, ref ControlPoint currentPoint)
		{
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			ControlPoint controlPoint = default(ControlPoint);
			bool flag = false;
			float num8 = default(float);
			while (currentPoint.m_OriginalEntity != Entity.Null)
			{
				if (m_ConnectedRoutes.HasBuffer(currentPoint.m_OriginalEntity) && ValidateStop(transportLineData, currentPoint.m_OriginalEntity))
				{
					return true;
				}
				if (m_Waypoints.HasBuffer(currentPoint.m_OriginalEntity) && math.any(currentPoint.m_ElementIndex >= 0))
				{
					if (currentPoint.m_ElementIndex.y >= 0)
					{
						DynamicBuffer<RouteWaypoint> val = m_Waypoints[currentPoint.m_OriginalEntity];
						int y = currentPoint.m_ElementIndex.y;
						int num = math.select(currentPoint.m_ElementIndex.y + 1, 0, currentPoint.m_ElementIndex.y == val.Length - 1);
						float3 position = m_PositionData[val[y].m_Waypoint].m_Position;
						float3 position2 = m_PositionData[val[num].m_Waypoint].m_Position;
						float num2 = math.distance(currentPoint.m_Position, position);
						float num3 = math.distance(currentPoint.m_Position, position2);
						if (num2 < routeData.m_SnapDistance && num2 <= num3)
						{
							currentPoint.m_ElementIndex = new int2(y, -1);
							currentPoint.m_Position = position;
						}
						else if (num3 < routeData.m_SnapDistance)
						{
							currentPoint.m_ElementIndex = new int2(num, -1);
							currentPoint.m_Position = position2;
						}
					}
					if (currentPoint.m_ElementIndex.x >= 0 || m_State == State.Default)
					{
						return true;
					}
				}
				if (m_SubObjects.HasBuffer(currentPoint.m_OriginalEntity))
				{
					DynamicBuffer<Game.Objects.SubObject> val2 = m_SubObjects[currentPoint.m_OriginalEntity];
					float num4 = routeData.m_SnapDistance;
					for (int i = 0; i < val2.Length; i++)
					{
						Entity subObject = val2[i].m_SubObject;
						if (m_ConnectedRoutes.HasBuffer(subObject) && ValidateStop(transportLineData, currentPoint.m_OriginalEntity))
						{
							Transform transform = m_TransformData[subObject];
							float num5 = math.distance(transform.m_Position, currentPoint.m_HitPosition);
							if (num5 < num4)
							{
								num4 = num5;
								currentPoint.m_Position = transform.m_Position;
								currentPoint.m_OriginalEntity = subObject;
							}
						}
					}
					if (num4 < routeData.m_SnapDistance)
					{
						return true;
					}
				}
				if (!flag && m_State != State.Default && m_SubLanes.HasBuffer(currentPoint.m_OriginalEntity))
				{
					DynamicBuffer<Game.Net.SubLane> val3 = m_SubLanes[currentPoint.m_OriginalEntity];
					float num6 = routeData.m_SnapDistance;
					for (int j = 0; j < val3.Length; j++)
					{
						Entity subLane = val3[j].m_SubLane;
						if (CheckLaneType(routeData, transportLineData, routeConnectionData, subLane))
						{
							Curve curve = m_CurveData[subLane];
							float num7 = MathUtils.Distance(curve.m_Bezier, currentPoint.m_HitPosition, ref num8);
							if (num7 < num6)
							{
								num6 = num7;
								controlPoint = currentPoint;
								controlPoint.m_OriginalEntity = Entity.Null;
								controlPoint.m_Position = MathUtils.Position(curve.m_Bezier, num8);
								flag = true;
							}
						}
					}
				}
				if (m_OwnerData.HasComponent(currentPoint.m_OriginalEntity))
				{
					currentPoint.m_OriginalEntity = m_OwnerData[currentPoint.m_OriginalEntity].m_Owner;
					if (m_TransformData.HasComponent(currentPoint.m_OriginalEntity))
					{
						currentPoint.m_Position = m_TransformData[currentPoint.m_OriginalEntity].m_Position;
					}
				}
				else
				{
					currentPoint.m_OriginalEntity = Entity.Null;
				}
			}
			if (flag)
			{
				currentPoint = controlPoint;
				return true;
			}
			if (m_State == State.Default && m_ControlPoints.Length == 1)
			{
				currentPoint = default(ControlPoint);
			}
			else if (m_State == State.Modify && m_ControlPoints.Length >= 1)
			{
				currentPoint = m_MoveStartPosition;
			}
			else if (m_State == State.Remove && m_ControlPoints.Length >= 1)
			{
				currentPoint = m_MoveStartPosition;
				currentPoint.m_OriginalEntity = Entity.Null;
			}
			else if (m_ControlPoints.Length >= 2)
			{
				currentPoint = m_ControlPoints[m_ControlPoints.Length - 2];
			}
			return false;
		}

		private bool CheckLaneType(RouteData routeData, TransportLineData transportLineData, RouteConnectionData routeConnectionData, Entity lane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			if (m_SlaveLaneData.HasComponent(lane))
			{
				return false;
			}
			PrefabRef prefabRef = m_PrefabRefData[lane];
			if (routeData.m_Type == RouteType.TransportLine)
			{
				switch (transportLineData.m_TransportType)
				{
				case TransportType.Bus:
					if (m_PrefabCarLaneData.HasComponent(prefabRef.m_Prefab))
					{
						CarLaneData carLaneData2 = m_PrefabCarLaneData[prefabRef.m_Prefab];
						if ((carLaneData2.m_RoadTypes & RoadTypes.Car) != RoadTypes.None)
						{
							return (int)carLaneData2.m_MaxSize >= (int)transportLineData.m_SizeClass;
						}
						return false;
					}
					return false;
				case TransportType.Tram:
					if (m_PrefabTrackLaneData.HasComponent(prefabRef.m_Prefab))
					{
						return (m_PrefabTrackLaneData[prefabRef.m_Prefab].m_TrackTypes & TrackTypes.Tram) != 0;
					}
					return false;
				case TransportType.Train:
					if (m_PrefabTrackLaneData.HasComponent(prefabRef.m_Prefab))
					{
						return (m_PrefabTrackLaneData[prefabRef.m_Prefab].m_TrackTypes & TrackTypes.Train) != 0;
					}
					return false;
				case TransportType.Subway:
					if (m_PrefabTrackLaneData.HasComponent(prefabRef.m_Prefab))
					{
						return (m_PrefabTrackLaneData[prefabRef.m_Prefab].m_TrackTypes & TrackTypes.Subway) != 0;
					}
					return false;
				case TransportType.Ship:
					if (m_PrefabCarLaneData.HasComponent(prefabRef.m_Prefab))
					{
						CarLaneData carLaneData3 = m_PrefabCarLaneData[prefabRef.m_Prefab];
						if ((carLaneData3.m_RoadTypes & RoadTypes.Watercraft) != RoadTypes.None)
						{
							return (int)carLaneData3.m_MaxSize >= (int)transportLineData.m_SizeClass;
						}
						return false;
					}
					return false;
				case TransportType.Airplane:
					if (m_PrefabCarLaneData.HasComponent(prefabRef.m_Prefab))
					{
						CarLaneData carLaneData = m_PrefabCarLaneData[prefabRef.m_Prefab];
						if ((carLaneData.m_RoadTypes & RoadTypes.Airplane) != RoadTypes.None)
						{
							return (int)carLaneData.m_MaxSize >= (int)transportLineData.m_SizeClass;
						}
						return false;
					}
					return false;
				}
			}
			else if (routeData.m_Type == RouteType.WorkRoute && m_PrefabCarLaneData.HasComponent(prefabRef.m_Prefab))
			{
				CarLaneData carLaneData4 = m_PrefabCarLaneData[prefabRef.m_Prefab];
				if ((carLaneData4.m_RoadTypes & routeConnectionData.m_RouteRoadType) != RoadTypes.None)
				{
					return (int)carLaneData4.m_MaxSize >= (int)routeConnectionData.m_RouteSizeClass;
				}
				return false;
			}
			return false;
		}
	}

	[BurstCompile]
	private struct CreateDefinitionsJob : IJob
	{
		[ReadOnly]
		public State m_State;

		[ReadOnly]
		public Entity m_Prefab;

		[ReadOnly]
		public Entity m_ApplyTempRoute;

		[ReadOnly]
		public Entity m_ServiceUpgradeOwner;

		[ReadOnly]
		public ControlPoint m_MoveStartPosition;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RouteData> m_PrefabRouteData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<Connected> m_ConnectedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> m_ConnectedRoutes;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_Waypoints;

		[ReadOnly]
		public NativeList<ControlPoint> m_ControlPoints;

		public Color32 m_Color;

		public NativeValue<Tooltip> m_Tooltip;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08db: Unknown result type (might be due to invalid IL or missing references)
			//IL_089f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0990: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0784: Unknown result type (might be due to invalid IL or missing references)
			//IL_0793: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
			m_Tooltip.value = Tooltip.None;
			int length = m_ControlPoints.Length;
			ControlPoint controlPoint = m_ControlPoints[0];
			ControlPoint other;
			if (length == 1)
			{
				other = default(ControlPoint);
				if (controlPoint.Equals(other))
				{
					return;
				}
			}
			if (m_State != State.Default)
			{
				controlPoint = m_MoveStartPosition;
			}
			Entity val = m_ControlPoints[length - 1].m_OriginalEntity;
			if (m_Waypoints.HasBuffer(val) && m_ControlPoints[length - 1].m_ElementIndex.x >= 0)
			{
				DynamicBuffer<RouteWaypoint> val2 = ((!(m_ApplyTempRoute != Entity.Null)) ? m_Waypoints[val] : ((!(val == m_TempData[m_ApplyTempRoute].m_Original)) ? m_Waypoints[val] : m_Waypoints[m_ApplyTempRoute]));
				if (m_ControlPoints[length - 1].m_ElementIndex.x < val2.Length)
				{
					val = val2[m_ControlPoints[length - 1].m_ElementIndex.x].m_Waypoint;
					Connected connected = default(Connected);
					if (m_ConnectedData.TryGetComponent(val, ref connected))
					{
						val = connected.m_Connected;
					}
				}
			}
			if (m_TransformData.HasComponent(val))
			{
				CreateTempWaypointObject(val);
			}
			Entity val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Prefab = m_Prefab,
				m_Owner = m_ServiceUpgradeOwner
			};
			ColorDefinition colorDefinition = new ColorDefinition
			{
				m_Color = m_Color
			};
			if (m_Waypoints.HasBuffer(controlPoint.m_OriginalEntity))
			{
				creationDefinition.m_Prefab = m_PrefabRefData[controlPoint.m_OriginalEntity].m_Prefab;
			}
			float minWaypointDistance = RouteUtils.GetMinWaypointDistance(m_PrefabRouteData[creationDefinition.m_Prefab]);
			if (m_Waypoints.HasBuffer(controlPoint.m_OriginalEntity) && math.any(controlPoint.m_ElementIndex >= 0))
			{
				creationDefinition.m_Original = controlPoint.m_OriginalEntity;
				DynamicBuffer<RouteWaypoint> val4;
				if (m_ApplyTempRoute != Entity.Null)
				{
					Temp temp = m_TempData[m_ApplyTempRoute];
					val4 = ((!(controlPoint.m_OriginalEntity == temp.m_Original)) ? m_Waypoints[controlPoint.m_OriginalEntity] : m_Waypoints[m_ApplyTempRoute]);
				}
				else
				{
					val4 = m_Waypoints[controlPoint.m_OriginalEntity];
				}
				if (controlPoint.m_ElementIndex.y >= 0)
				{
					int y = controlPoint.m_ElementIndex.y;
					int num = math.select(controlPoint.m_ElementIndex.y + 1, 0, controlPoint.m_ElementIndex.y == val4.Length - 1);
					float3 position = m_ControlPoints[length - 1].m_Position;
					bool flag = math.any(new float2(math.distance(position, m_PositionData[val4[y].m_Waypoint].m_Position), math.distance(position, m_PositionData[val4[num].m_Waypoint].m_Position)) < minWaypointDistance);
					bool flag2 = !m_MoveStartPosition.Equals(m_ControlPoints[length - 1]);
					bool flag3 = m_State == State.Default || flag || !flag2;
					int num2 = math.select(length, length - 1, flag3);
					int num3 = val4.Length + num2;
					DynamicBuffer<WaypointDefinition> val5 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<WaypointDefinition>(val3);
					val5.ResizeUninitialized(num3);
					int num4 = 0;
					for (int i = 0; i <= controlPoint.m_ElementIndex.y; i++)
					{
						val5[num4++] = GetWaypointDefinition(val4[i].m_Waypoint);
					}
					for (int j = 0; j < num2; j++)
					{
						val5[num4++] = GetWaypointDefinition(m_ControlPoints[j]);
					}
					for (int k = controlPoint.m_ElementIndex.y + 1; k < val4.Length; k++)
					{
						val5[num4++] = GetWaypointDefinition(val4[k].m_Waypoint);
					}
					switch (m_State)
					{
					case State.Default:
						m_Tooltip.value = Tooltip.ModifySegment;
						break;
					case State.Create:
					case State.Remove:
						m_Tooltip.value = Tooltip.None;
						break;
					case State.Modify:
						if (flag || !flag2)
						{
							m_Tooltip.value = Tooltip.None;
						}
						else
						{
							m_Tooltip.value = Tooltip.InsertWaypoint;
						}
						break;
					}
				}
				else
				{
					float3 position2 = m_ControlPoints[length - 1].m_Position;
					bool flag4;
					bool flag5;
					if (m_State == State.Remove)
					{
						flag4 = !(m_ControlPoints[length - 1].m_OriginalEntity != m_MoveStartPosition.m_OriginalEntity) && math.distance(position2, m_PositionData[val4[controlPoint.m_ElementIndex.x].m_Waypoint].m_Position) < minWaypointDistance;
						flag5 = false;
					}
					else
					{
						int num5 = math.select(controlPoint.m_ElementIndex.x - 1, val4.Length - 1, controlPoint.m_ElementIndex.x == 0);
						int num6 = math.select(controlPoint.m_ElementIndex.x + 1, 0, controlPoint.m_ElementIndex.x == val4.Length - 1);
						flag4 = math.any(new float2(math.distance(position2, m_PositionData[val4[num5].m_Waypoint].m_Position), math.distance(position2, m_PositionData[val4[num6].m_Waypoint].m_Position)) < minWaypointDistance);
						flag5 = !m_MoveStartPosition.Equals(m_ControlPoints[length - 1]);
					}
					bool flag6 = flag4;
					int num7 = math.select(length, length - 1, flag6);
					int num8 = val4.Length + num7 - 1;
					DynamicBuffer<WaypointDefinition> val6 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<WaypointDefinition>(val3);
					val6.ResizeUninitialized(num8);
					int num9 = 0;
					for (int l = 0; l < controlPoint.m_ElementIndex.x; l++)
					{
						val6[num9++] = GetWaypointDefinition(val4[l].m_Waypoint);
					}
					for (int m = 0; m < num7; m++)
					{
						WaypointDefinition waypointDefinition = GetWaypointDefinition(m_ControlPoints[m]);
						waypointDefinition.m_Original = val4[controlPoint.m_ElementIndex.x].m_Waypoint;
						val6[num9++] = waypointDefinition;
					}
					for (int n = controlPoint.m_ElementIndex.x + 1; n < val4.Length; n++)
					{
						val6[num9++] = GetWaypointDefinition(val4[n].m_Waypoint);
					}
					if (num8 <= 1)
					{
						creationDefinition.m_Flags |= CreationFlags.Delete;
					}
					switch (m_State)
					{
					case State.Default:
					{
						Entity waypoint = val4[controlPoint.m_ElementIndex.x].m_Waypoint;
						if (m_ConnectedData.HasComponent(waypoint) && m_ConnectedData[waypoint].m_Connected != Entity.Null)
						{
							m_Tooltip.value = Tooltip.CreateOrModify;
						}
						else
						{
							m_Tooltip.value = Tooltip.ModifyWaypoint;
						}
						break;
					}
					case State.Create:
						m_Tooltip.value = Tooltip.None;
						break;
					case State.Modify:
						if (num8 <= 1)
						{
							m_Tooltip.value = Tooltip.DeleteRoute;
						}
						else if (flag4)
						{
							m_Tooltip.value = Tooltip.MergeWaypoints;
						}
						else if (flag5)
						{
							m_Tooltip.value = Tooltip.MoveWaypoint;
						}
						else
						{
							m_Tooltip.value = Tooltip.None;
						}
						break;
					case State.Remove:
						if (num8 <= 1)
						{
							m_Tooltip.value = Tooltip.DeleteRoute;
						}
						else if (flag4)
						{
							m_Tooltip.value = Tooltip.RemoveWaypoint;
						}
						else
						{
							m_Tooltip.value = Tooltip.None;
						}
						break;
					}
				}
			}
			else
			{
				bool flag7 = false;
				if (length >= 2)
				{
					flag7 = math.distance(m_ControlPoints[length - 2].m_Position, m_ControlPoints[length - 1].m_Position) < minWaypointDistance;
				}
				int num10 = math.select(length, length - 1, flag7);
				DynamicBuffer<WaypointDefinition> val7 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<WaypointDefinition>(val3);
				val7.ResizeUninitialized(num10);
				for (int num11 = 0; num11 < num10; num11++)
				{
					val7[num11] = GetWaypointDefinition(m_ControlPoints[num11]);
				}
				switch (m_State)
				{
				case State.Default:
					if (length == 1)
					{
						m_Tooltip.value = Tooltip.CreateRoute;
					}
					else
					{
						m_Tooltip.value = Tooltip.None;
					}
					break;
				case State.Create:
					if (flag7)
					{
						m_Tooltip.value = Tooltip.None;
						break;
					}
					if (length >= 3)
					{
						other = m_ControlPoints[0];
						if (((float3)(ref other.m_Position)).Equals(m_ControlPoints[length - 1].m_Position))
						{
							m_Tooltip.value = Tooltip.CompleteRoute;
							break;
						}
					}
					m_Tooltip.value = Tooltip.AddWaypoint;
					break;
				case State.Modify:
				case State.Remove:
					m_Tooltip.value = Tooltip.None;
					break;
				}
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val3, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ColorDefinition>(val3, colorDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val3, default(Updated));
		}

		private void CreateTempWaypointObject(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[entity];
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Original = entity
			};
			creationDefinition.m_Flags |= CreationFlags.Select;
			ObjectDefinition objectDefinition = new ObjectDefinition
			{
				m_Position = transform.m_Position,
				m_Rotation = transform.m_Rotation
			};
			Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
			if (m_ElevationData.TryGetComponent(entity, ref elevation))
			{
				objectDefinition.m_Elevation = elevation.m_Elevation;
				objectDefinition.m_ParentMesh = ObjectUtils.GetSubParentMesh(elevation.m_Flags);
			}
			else
			{
				objectDefinition.m_ParentMesh = -1;
			}
			if (m_AttachedData.HasComponent(entity))
			{
				creationDefinition.m_Attached = m_AttachedData[entity].m_Parent;
				creationDefinition.m_Flags |= CreationFlags.Attach;
			}
			objectDefinition.m_Probability = 100;
			objectDefinition.m_PrefabSubIndex = -1;
			objectDefinition.m_LocalPosition = transform.m_Position;
			objectDefinition.m_LocalRotation = transform.m_Rotation;
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ObjectDefinition>(val, objectDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
		}

		private WaypointDefinition GetWaypointDefinition(Entity original)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			WaypointDefinition result = new WaypointDefinition(m_PositionData[original].m_Position)
			{
				m_Original = original
			};
			if (m_ConnectedData.HasComponent(original))
			{
				result.m_Connection = m_ConnectedData[original].m_Connected;
			}
			return result;
		}

		private WaypointDefinition GetWaypointDefinition(ControlPoint controlPoint)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			WaypointDefinition result = new WaypointDefinition(controlPoint.m_Position);
			if (m_ConnectedData.HasComponent(controlPoint.m_OriginalEntity))
			{
				result.m_Connection = m_ConnectedData[controlPoint.m_OriginalEntity].m_Connected;
			}
			else if (m_ConnectedRoutes.HasBuffer(controlPoint.m_OriginalEntity))
			{
				result.m_Connection = controlPoint.m_OriginalEntity;
			}
			else if (m_Waypoints.HasBuffer(controlPoint.m_OriginalEntity) && controlPoint.m_ElementIndex.x >= 0)
			{
				DynamicBuffer<RouteWaypoint> val;
				if (m_ApplyTempRoute != Entity.Null)
				{
					Temp temp = m_TempData[m_ApplyTempRoute];
					val = ((!(controlPoint.m_OriginalEntity == temp.m_Original)) ? m_Waypoints[controlPoint.m_OriginalEntity] : m_Waypoints[m_ApplyTempRoute]);
				}
				else
				{
					val = m_Waypoints[controlPoint.m_OriginalEntity];
				}
				if (controlPoint.m_ElementIndex.x < val.Length)
				{
					Entity waypoint = val[controlPoint.m_ElementIndex.x].m_Waypoint;
					if (m_ConnectedData.HasComponent(waypoint))
					{
						result.m_Connection = m_ConnectedData[waypoint].m_Connected;
					}
				}
			}
			return result;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Route> __Game_Routes_Route_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathUpdated> __Game_Pathfind_PathUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteData> __Game_Prefabs_RouteData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportLineData> __Game_Prefabs_TransportLineData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> __Game_Prefabs_RouteConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportStopData> __Game_Prefabs_TransportStopData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> __Game_Routes_ConnectedRoute_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

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
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			__Game_Routes_Route_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Route>(true);
			__Game_Pathfind_PathUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathUpdated>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_RouteData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteData>(true);
			__Game_Prefabs_TransportLineData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLineData>(true);
			__Game_Prefabs_RouteConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteConnectionData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Prefabs_TransportStopData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportStopData>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Routes_ConnectedRoute_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedRoute>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Elevation>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
		}
	}

	public const string kToolID = "Route Tool";

	private AudioManager m_AudioManager;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_TempRouteQuery;

	private EntityQuery m_EventQuery;

	private EntityQuery m_SoundQuery;

	private IProxyAction m_AddWaypoint;

	private IProxyAction m_InsertWaypoint;

	private IProxyAction m_MoveWaypoint;

	private IProxyAction m_MergeWaypoint;

	private IProxyAction m_RemoveWaypoint;

	private IProxyAction m_UndoWaypoint;

	private IProxyAction m_CreateRoute;

	private IProxyAction m_CompleteRoute;

	private IProxyAction m_DeleteRoute;

	private IProxyAction m_DiscardInsertWaypoint;

	private IProxyAction m_DiscardMoveWaypoint;

	private IProxyAction m_DiscardMergeWaypoint;

	private bool m_ApplyBlocked;

	private ControlPoint m_LastRaycastPoint;

	private NativeList<ControlPoint> m_ControlPoints;

	private NativeValue<Tooltip> m_Tooltip;

	private State m_State;

	private bool m_ControlPointsMoved;

	private bool m_ForceApply;

	private bool m_ForceCancel;

	private bool m_CanApplyModify;

	private ControlPoint m_MoveStartPosition;

	private ToolOutputBarrier m_ToolOutputBarrier;

	private RoutePrefab m_SelectedPrefab;

	private TypeHandle __TypeHandle;

	public override string toolID => "Route Tool";

	public RoutePrefab prefab
	{
		get
		{
			return m_SelectedPrefab;
		}
		set
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)value != (Object)(object)m_SelectedPrefab)
			{
				m_SelectedPrefab = value;
				m_ForceUpdate = true;
				color = Color32.op_Implicit(m_SelectedPrefab.m_Color);
				serviceUpgrade = m_PrefabSystem.HasComponent<ServiceUpgradeData>(m_SelectedPrefab);
				m_ToolSystem.EventPrefabChanged?.Invoke(value);
			}
		}
	}

	public State state => m_State;

	public ControlPoint moveStartPosition => m_MoveStartPosition;

	public Tooltip tooltip => m_Tooltip.value;

	public bool underground { get; set; }

	public bool serviceUpgrade { get; private set; }

	private protected override IEnumerable<IProxyAction> toolActions
	{
		get
		{
			yield return m_AddWaypoint;
			yield return m_InsertWaypoint;
			yield return m_MoveWaypoint;
			yield return m_MergeWaypoint;
			yield return m_RemoveWaypoint;
			yield return m_UndoWaypoint;
			yield return m_CreateRoute;
			yield return m_CompleteRoute;
			yield return m_DeleteRoute;
			yield return m_DiscardInsertWaypoint;
			yield return m_DiscardMoveWaypoint;
			yield return m_DiscardMergeWaypoint;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_DefinitionQuery = GetDefinitionQuery();
		m_TempRouteQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadOnly<Temp>()
		});
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Event>(),
			ComponentType.ReadOnly<PathUpdated>()
		});
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		m_AddWaypoint = InputManager.instance.toolActionCollection.GetActionState("Add Waypoint", "RouteToolSystem");
		m_InsertWaypoint = InputManager.instance.toolActionCollection.GetActionState("Insert Waypoint", "RouteToolSystem");
		m_MoveWaypoint = InputManager.instance.toolActionCollection.GetActionState("Move Waypoint", "RouteToolSystem");
		m_MergeWaypoint = InputManager.instance.toolActionCollection.GetActionState("Merge Waypoint", "RouteToolSystem");
		m_RemoveWaypoint = InputManager.instance.toolActionCollection.GetActionState("Remove Waypoint", "RouteToolSystem");
		m_UndoWaypoint = InputManager.instance.toolActionCollection.GetActionState("Undo Waypoint", "RouteToolSystem");
		m_CreateRoute = InputManager.instance.toolActionCollection.GetActionState("Create Route", "RouteToolSystem");
		m_CompleteRoute = InputManager.instance.toolActionCollection.GetActionState("Complete Route", "RouteToolSystem");
		m_DeleteRoute = InputManager.instance.toolActionCollection.GetActionState("Delete Route", "RouteToolSystem");
		m_DiscardInsertWaypoint = InputManager.instance.toolActionCollection.GetActionState("Discard Insert Waypoint", "RouteToolSystem");
		m_DiscardMoveWaypoint = InputManager.instance.toolActionCollection.GetActionState("Discard Move Waypoint", "RouteToolSystem");
		m_DiscardMergeWaypoint = InputManager.instance.toolActionCollection.GetActionState("Discard Merge Waypoint", "RouteToolSystem");
		m_ControlPoints = new NativeList<ControlPoint>(20, AllocatorHandle.op_Implicit((Allocator)4));
		m_Tooltip = new NativeValue<Tooltip>((Allocator)4);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_ControlPoints.Dispose();
		m_Tooltip.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		base.OnStartRunning();
		m_ControlPoints.Clear();
		m_LastRaycastPoint = default(ControlPoint);
		m_State = State.Default;
		m_Tooltip.value = Tooltip.None;
		m_ForceApply = false;
		m_ForceCancel = false;
		m_ApplyBlocked = false;
		base.requireUnderground = false;
		base.requireNetArrows = false;
		base.requireRoutes = RouteType.None;
		base.requireNet = Layer.None;
	}

	private protected override void UpdateActions()
	{
		using (ProxyAction.DeferStateUpdating())
		{
			UpdateApplyAction();
			UpdateSecondaryApplyAction();
			UpdateCancelAction();
		}
	}

	private void UpdateApplyAction()
	{
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		ControlPoint controlPoint;
		switch (state)
		{
		case State.Default:
			if (m_ControlPoints.Length >= 1)
			{
				controlPoint = m_ControlPoints[0];
				if (!controlPoint.Equals(default(ControlPoint)))
				{
					Route route2 = default(Route);
					DynamicBuffer<RouteWaypoint> val2 = default(DynamicBuffer<RouteWaypoint>);
					if (m_ControlPoints.Length == 1 && EntitiesExtensions.TryGetComponent<Route>(((ComponentSystemBase)this).EntityManager, m_ControlPoints[0].m_OriginalEntity, ref route2) && route2.m_Flags == RouteFlags.Complete && EntitiesExtensions.TryGetBuffer<RouteWaypoint>(((ComponentSystemBase)this).EntityManager, m_ControlPoints[0].m_OriginalEntity, true, ref val2))
					{
						Position position3 = default(Position);
						for (int j = 0; j < val2.Length; j++)
						{
							Entity waypoint2 = val2[j].m_Waypoint;
							if (EntitiesExtensions.TryGetComponent<Position>(((ComponentSystemBase)this).EntityManager, waypoint2, ref position3) && ((float3)(ref position3.m_Position)).Equals(m_ControlPoints[0].m_Position))
							{
								base.applyAction.enabled = base.actionsEnabled;
								base.applyActionOverride = m_MoveWaypoint;
								return;
							}
						}
						base.applyAction.enabled = base.actionsEnabled;
						base.applyActionOverride = m_InsertWaypoint;
					}
					else
					{
						EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<RouteWaypoint>(m_ControlPoints[0].m_OriginalEntity) || !math.any(m_ControlPoints[0].m_ElementIndex >= 0))
						{
							base.applyAction.enabled = base.actionsEnabled;
							base.applyActionOverride = m_CreateRoute;
						}
						else
						{
							base.applyAction.enabled = false;
							base.applyActionOverride = null;
						}
					}
					break;
				}
			}
			base.applyAction.enabled = base.actionsEnabled;
			base.applyActionOverride = null;
			break;
		case State.Create:
		{
			ref NativeList<ControlPoint> reference = ref m_ControlPoints;
			controlPoint = reference[reference.Length - 1];
			if (controlPoint.Equals(default(ControlPoint)))
			{
				base.applyAction.enabled = base.actionsEnabled;
				base.applyActionOverride = null;
				break;
			}
			ref NativeList<ControlPoint> reference2 = ref m_ControlPoints;
			controlPoint = reference2[reference2.Length - 1];
			ref float3 position2 = ref controlPoint.m_Position;
			ref NativeList<ControlPoint> reference3 = ref m_ControlPoints;
			if (!((float3)(ref position2)).Equals(reference3[reference3.Length - 2].m_Position))
			{
				ref NativeList<ControlPoint> reference4 = ref m_ControlPoints;
				controlPoint = reference4[reference4.Length - 1];
				if (!((float3)(ref controlPoint.m_Position)).Equals(m_ControlPoints[0].m_Position))
				{
					base.applyAction.enabled = base.actionsEnabled;
					base.applyActionOverride = (GetAllowApply() ? m_AddWaypoint : null);
					break;
				}
			}
			if (m_ControlPoints.Length >= 3)
			{
				ref NativeList<ControlPoint> reference5 = ref m_ControlPoints;
				controlPoint = reference5[reference5.Length - 1];
				if (((float3)(ref controlPoint.m_Position)).Equals(m_ControlPoints[0].m_Position))
				{
					base.applyAction.enabled = base.actionsEnabled;
					base.applyActionOverride = (GetAllowApply() ? m_CompleteRoute : null);
					break;
				}
			}
			base.applyAction.enabled = base.actionsEnabled;
			base.applyActionOverride = null;
			break;
		}
		case State.Modify:
		{
			Route route = default(Route);
			DynamicBuffer<RouteWaypoint> val = default(DynamicBuffer<RouteWaypoint>);
			if (EntitiesExtensions.TryGetComponent<Route>(((ComponentSystemBase)this).EntityManager, m_MoveStartPosition.m_OriginalEntity, ref route) && route.m_Flags == RouteFlags.Complete && EntitiesExtensions.TryGetBuffer<RouteWaypoint>(((ComponentSystemBase)this).EntityManager, m_MoveStartPosition.m_OriginalEntity, true, ref val))
			{
				Position position = default(Position);
				for (int i = 0; i < val.Length; i++)
				{
					Entity waypoint = val[i].m_Waypoint;
					if (EntitiesExtensions.TryGetComponent<Position>(((ComponentSystemBase)this).EntityManager, waypoint, ref position))
					{
						if (!((float3)(ref position.m_Position)).Equals(m_MoveStartPosition.m_Position) && ((float3)(ref position.m_Position)).Equals(m_ControlPoints[0].m_Position))
						{
							base.applyAction.enabled = base.actionsEnabled;
							base.applyActionOverride = m_MergeWaypoint;
							return;
						}
						if (((float3)(ref position.m_Position)).Equals(m_MoveStartPosition.m_Position))
						{
							base.applyAction.enabled = base.actionsEnabled;
							base.applyActionOverride = m_MoveWaypoint;
							return;
						}
					}
				}
				base.applyAction.enabled = base.actionsEnabled;
				base.applyActionOverride = m_InsertWaypoint;
			}
			else
			{
				base.applyAction.enabled = false;
				base.applyActionOverride = null;
			}
			break;
		}
		case State.Remove:
			base.applyAction.enabled = base.actionsEnabled;
			base.applyActionOverride = null;
			break;
		default:
			base.applyAction.enabled = false;
			base.applyActionOverride = null;
			break;
		}
	}

	private void UpdateSecondaryApplyAction()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		switch (state)
		{
		case State.Default:
		{
			Route route = default(Route);
			DynamicBuffer<RouteWaypoint> val = default(DynamicBuffer<RouteWaypoint>);
			if (m_ControlPoints.Length == 1 && EntitiesExtensions.TryGetComponent<Route>(((ComponentSystemBase)this).EntityManager, m_ControlPoints[0].m_OriginalEntity, ref route) && route.m_Flags == RouteFlags.Complete && EntitiesExtensions.TryGetBuffer<RouteWaypoint>(((ComponentSystemBase)this).EntityManager, m_ControlPoints[0].m_OriginalEntity, true, ref val))
			{
				Position position = default(Position);
				for (int i = 0; i < val.Length; i++)
				{
					Entity waypoint = val[i].m_Waypoint;
					if (EntitiesExtensions.TryGetComponent<Position>(((ComponentSystemBase)this).EntityManager, waypoint, ref position) && ((float3)(ref position.m_Position)).Equals(m_ControlPoints[0].m_Position))
					{
						base.secondaryApplyAction.enabled = base.actionsEnabled;
						base.secondaryApplyActionOverride = ((val.Length >= 3) ? m_RemoveWaypoint : m_DeleteRoute);
						return;
					}
				}
			}
			base.secondaryApplyAction.enabled = false;
			base.secondaryApplyActionOverride = null;
			break;
		}
		case State.Create:
			if (m_ControlPoints.Length > 1)
			{
				base.secondaryApplyAction.enabled = base.actionsEnabled;
				base.secondaryApplyActionOverride = m_UndoWaypoint;
			}
			else
			{
				base.secondaryApplyAction.enabled = base.actionsEnabled;
				base.secondaryApplyActionOverride = null;
			}
			break;
		case State.Remove:
			base.secondaryApplyAction.enabled = base.actionsEnabled;
			base.secondaryApplyActionOverride = m_RemoveWaypoint;
			break;
		default:
			base.secondaryApplyAction.enabled = false;
			base.secondaryApplyActionOverride = null;
			break;
		}
	}

	private void UpdateCancelAction()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (state == State.Modify)
		{
			Route route = default(Route);
			DynamicBuffer<RouteWaypoint> val = default(DynamicBuffer<RouteWaypoint>);
			if (EntitiesExtensions.TryGetComponent<Route>(((ComponentSystemBase)this).EntityManager, m_MoveStartPosition.m_OriginalEntity, ref route) && route.m_Flags == RouteFlags.Complete && EntitiesExtensions.TryGetBuffer<RouteWaypoint>(((ComponentSystemBase)this).EntityManager, m_MoveStartPosition.m_OriginalEntity, true, ref val))
			{
				Position position = default(Position);
				for (int i = 0; i < val.Length; i++)
				{
					Entity waypoint = val[i].m_Waypoint;
					if (EntitiesExtensions.TryGetComponent<Position>(((ComponentSystemBase)this).EntityManager, waypoint, ref position))
					{
						if (!((float3)(ref position.m_Position)).Equals(m_MoveStartPosition.m_Position) && ((float3)(ref position.m_Position)).Equals(m_ControlPoints[0].m_Position))
						{
							base.cancelAction.enabled = base.actionsEnabled;
							base.cancelActionOverride = m_DiscardMergeWaypoint;
							return;
						}
						if (((float3)(ref position.m_Position)).Equals(m_MoveStartPosition.m_Position))
						{
							base.cancelAction.enabled = base.actionsEnabled;
							base.cancelActionOverride = m_DiscardMoveWaypoint;
							return;
						}
					}
				}
				base.cancelAction.enabled = base.actionsEnabled;
				base.cancelActionOverride = m_DiscardInsertWaypoint;
			}
			else
			{
				base.cancelAction.enabled = false;
				base.cancelActionOverride = null;
			}
		}
		else
		{
			base.cancelAction.enabled = false;
			base.cancelActionOverride = null;
		}
	}

	public NativeList<ControlPoint> GetControlPoints(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = ((SystemBase)this).Dependency;
		return m_ControlPoints;
	}

	public override PrefabBase GetPrefab()
	{
		return prefab;
	}

	public override bool TrySetPrefab(PrefabBase prefab)
	{
		if (prefab is RoutePrefab routePrefab)
		{
			this.prefab = routePrefab;
			return true;
		}
		return false;
	}

	public override void SetUnderground(bool underground)
	{
		this.underground = underground;
	}

	public override void ElevationUp()
	{
		underground = false;
	}

	public override void ElevationDown()
	{
		underground = true;
	}

	public override void ElevationScroll()
	{
		underground = !underground;
	}

	public override void InitializeRaycast()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		base.InitializeRaycast();
		if ((Object)(object)prefab != (Object)null)
		{
			bool flag = false;
			RouteData componentData = m_PrefabSystem.GetComponentData<RouteData>((PrefabBase)prefab);
			Entity entity = m_PrefabSystem.GetEntity(prefab);
			if (m_State == State.Modify || m_State == State.Remove)
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Route>(m_MoveStartPosition.m_OriginalEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					entity = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(m_MoveStartPosition.m_OriginalEntity).m_Prefab;
				}
			}
			TransportLineData transportLineData = default(TransportLineData);
			RouteConnectionData routeConnectionData = default(RouteConnectionData);
			if (EntitiesExtensions.TryGetComponent<TransportLineData>(((ComponentSystemBase)this).EntityManager, entity, ref transportLineData))
			{
				m_ToolRaycastSystem.typeMask = TypeMask.StaticObjects | TypeMask.Net | TypeMask.RouteWaypoints;
				m_ToolRaycastSystem.transportType = transportLineData.m_TransportType;
				m_ToolRaycastSystem.raycastFlags |= RaycastFlags.BuildingLots;
				if (transportLineData.m_PassengerTransport)
				{
					m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Passenger;
				}
				if (transportLineData.m_CargoTransport)
				{
					m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Cargo;
				}
				switch (transportLineData.m_TransportType)
				{
				case TransportType.Bus:
					m_ToolRaycastSystem.netLayerMask = Layer.Road | Layer.Pathway | Layer.MarkerPathway | Layer.PublicTransportRoad;
					flag = true;
					break;
				case TransportType.Tram:
					m_ToolRaycastSystem.netLayerMask = Layer.Road | Layer.TramTrack | Layer.PublicTransportRoad;
					flag = true;
					break;
				case TransportType.Train:
					m_ToolRaycastSystem.netLayerMask = Layer.TrainTrack;
					flag = true;
					break;
				case TransportType.Subway:
					m_ToolRaycastSystem.netLayerMask = Layer.SubwayTrack;
					flag = true;
					break;
				case TransportType.Ship:
					m_ToolRaycastSystem.netLayerMask = Layer.Waterway;
					break;
				case TransportType.Airplane:
					m_ToolRaycastSystem.netLayerMask = Layer.Taxiway | Layer.MarkerTaxiway;
					break;
				default:
					m_ToolRaycastSystem.netLayerMask = Layer.None;
					break;
				}
			}
			else if (EntitiesExtensions.TryGetComponent<RouteConnectionData>(((ComponentSystemBase)this).EntityManager, entity, ref routeConnectionData))
			{
				m_ToolRaycastSystem.typeMask = TypeMask.StaticObjects | TypeMask.Net | TypeMask.RouteWaypoints;
				m_ToolRaycastSystem.transportType = TransportType.Work;
				m_ToolRaycastSystem.raycastFlags |= RaycastFlags.BuildingLots;
				if ((routeConnectionData.m_RouteRoadType & RoadTypes.Car) != RoadTypes.None)
				{
					m_ToolRaycastSystem.netLayerMask |= Layer.Road | Layer.Pathway | Layer.MarkerPathway;
				}
				if ((routeConnectionData.m_RouteRoadType & RoadTypes.Watercraft) != RoadTypes.None)
				{
					m_ToolRaycastSystem.netLayerMask |= Layer.Waterway;
				}
			}
			else
			{
				m_ToolRaycastSystem.typeMask = TypeMask.Terrain | TypeMask.RouteWaypoints;
				m_ToolRaycastSystem.netLayerMask = Layer.None;
			}
			if (flag && underground)
			{
				m_ToolRaycastSystem.collisionMask = CollisionMask.Underground;
			}
			else
			{
				m_ToolRaycastSystem.collisionMask = CollisionMask.OnGround | CollisionMask.Overground;
			}
			if (m_State == State.Default)
			{
				m_ToolRaycastSystem.typeMask |= TypeMask.RouteSegments;
			}
			m_ToolRaycastSystem.routeType = componentData.m_Type;
		}
		else
		{
			m_ToolRaycastSystem.typeMask = TypeMask.None;
			m_ToolRaycastSystem.netLayerMask = Layer.None;
			m_ToolRaycastSystem.routeType = RouteType.None;
		}
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		UpdateActions();
		bool flag = m_ForceApply;
		bool flag2 = m_ForceCancel;
		m_ForceApply = false;
		m_ForceCancel = false;
		if ((Object)(object)prefab != (Object)null)
		{
			allowUnderground = false;
			base.requireUnderground = false;
			base.requireNetArrows = false;
			base.requireNet = Layer.None;
			base.requireStops = TransportType.None;
			RouteData componentData = m_PrefabSystem.GetComponentData<RouteData>((PrefabBase)prefab);
			base.requireRoutes = componentData.m_Type;
			if (componentData.m_Type == RouteType.TransportLine)
			{
				TransportLineData componentData2 = m_PrefabSystem.GetComponentData<TransportLineData>((PrefabBase)prefab);
				base.requireNetArrows = true;
				switch (componentData2.m_TransportType)
				{
				case TransportType.Bus:
					base.requireNet |= Layer.Road | Layer.Pathway | Layer.MarkerPathway | Layer.PublicTransportRoad;
					allowUnderground = true;
					break;
				case TransportType.Tram:
					base.requireNet |= Layer.Road | Layer.TramTrack | Layer.PublicTransportRoad;
					allowUnderground = true;
					break;
				case TransportType.Train:
					base.requireNet |= Layer.TrainTrack;
					allowUnderground = true;
					break;
				case TransportType.Subway:
					base.requireNet |= Layer.SubwayTrack;
					allowUnderground = true;
					break;
				case TransportType.Ship:
					base.requireNet |= Layer.Waterway;
					break;
				case TransportType.Airplane:
					base.requireNet |= Layer.Taxiway | Layer.MarkerTaxiway;
					break;
				}
				base.requireStops = componentData2.m_TransportType;
			}
			else if (componentData.m_Type == RouteType.WorkRoute)
			{
				RouteConnectionData componentData3 = m_PrefabSystem.GetComponentData<RouteConnectionData>((PrefabBase)prefab);
				if ((componentData3.m_RouteRoadType & RoadTypes.Car) != RoadTypes.None)
				{
					base.requireNet |= Layer.Road | Layer.Pathway | Layer.MarkerPathway;
				}
				if ((componentData3.m_RouteRoadType & RoadTypes.Watercraft) != RoadTypes.None)
				{
					base.requireNet |= Layer.Waterway;
				}
				base.requireStops = TransportType.Work;
			}
			if (allowUnderground)
			{
				base.requireUnderground = underground;
			}
			UpdateInfoview(m_PrefabSystem.GetEntity(prefab));
			if (m_State != State.Default && !base.applyAction.enabled)
			{
				m_State = State.Default;
			}
			if ((m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
			{
				switch (m_State)
				{
				case State.Default:
				case State.Create:
					if (m_ApplyBlocked)
					{
						if (base.applyAction.WasReleasedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame())
						{
							m_ApplyBlocked = false;
						}
						return Update(inputDeps, fullUpdate: false);
					}
					if (base.applyAction.WasPressedThisFrame())
					{
						return Apply(inputDeps, base.applyAction.WasReleasedThisFrame());
					}
					if (base.secondaryApplyAction.WasPressedThisFrame())
					{
						return Cancel(inputDeps, base.secondaryApplyAction.WasReleasedThisFrame());
					}
					break;
				case State.Modify:
					if (base.cancelAction.WasPressedThisFrame())
					{
						m_ApplyBlocked = true;
						m_State = State.Default;
						return Update(inputDeps, fullUpdate: true);
					}
					if (flag || base.applyAction.WasReleasedThisFrame())
					{
						return Apply(inputDeps);
					}
					break;
				case State.Remove:
					if (base.cancelAction.WasPressedThisFrame())
					{
						m_ApplyBlocked = true;
						m_State = State.Default;
						return Update(inputDeps, fullUpdate: true);
					}
					if (flag2 || base.secondaryApplyAction.WasReleasedThisFrame())
					{
						return Cancel(inputDeps);
					}
					break;
				}
				return Update(inputDeps, fullUpdate: false);
			}
		}
		else
		{
			base.requireUnderground = false;
			base.requireNetArrows = false;
			base.requireRoutes = RouteType.None;
			base.requireNet = Layer.None;
			UpdateInfoview(Entity.Null);
			m_Tooltip.value = Tooltip.None;
		}
		if (m_State == State.Modify && base.applyAction.WasReleasedThisFrame())
		{
			if ((m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
			{
				return Cancel(inputDeps);
			}
			m_ControlPoints.Clear();
			m_State = State.Default;
		}
		else if (m_State == State.Remove && base.secondaryApplyAction.WasReleasedThisFrame())
		{
			if ((m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
			{
				return Apply(inputDeps);
			}
			m_ControlPoints.Clear();
			m_State = State.Default;
		}
		return Clear(inputDeps);
	}

	protected override bool GetRaycastResult(out ControlPoint controlPoint)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out Entity entity, out RaycastHit hit))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<ConnectedRoute>(hit.m_HitEntity))
			{
				entity = hit.m_HitEntity;
			}
			controlPoint = new ControlPoint(entity, hit);
			return true;
		}
		controlPoint = default(ControlPoint);
		return false;
	}

	protected override bool GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out var entity, out var hit, out forceUpdate))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<ConnectedRoute>(hit.m_HitEntity))
			{
				entity = hit.m_HitEntity;
			}
			controlPoint = new ControlPoint(entity, hit);
			return true;
		}
		controlPoint = default(ControlPoint);
		return false;
	}

	private JobHandle Clear(JobHandle inputDeps)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.applyMode = ApplyMode.Clear;
		return inputDeps;
	}

	private JobHandle Cancel(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_TransportLineRemoveSound);
		switch (m_State)
		{
		case State.Default:
			if (GetAllowApply() && m_ControlPoints.Length > 0)
			{
				base.applyMode = ApplyMode.Clear;
				ControlPoint controlPoint2 = m_ControlPoints[0];
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Route>(controlPoint2.m_OriginalEntity) && controlPoint2.m_ElementIndex.x >= 0)
				{
					m_State = State.Remove;
					m_MoveStartPosition = controlPoint2;
					m_ForceCancel = singleFrameOnly;
					if (GetRaycastResult(out var controlPoint3))
					{
						m_LastRaycastPoint = controlPoint3;
						m_ControlPoints[0] = controlPoint3;
						inputDeps = SnapControlPoints(inputDeps, Entity.Null);
						inputDeps = UpdateDefinitions(inputDeps, Entity.Null);
					}
					else
					{
						m_Tooltip.value = Tooltip.None;
					}
					return inputDeps;
				}
				return Update(inputDeps, fullUpdate: false);
			}
			return Update(inputDeps, fullUpdate: false);
		case State.Create:
		{
			m_ControlPoints.RemoveAtSwapBack(m_ControlPoints.Length - 1);
			base.applyMode = ApplyMode.Clear;
			if (m_ControlPoints.Length <= 1)
			{
				m_State = State.Default;
			}
			if (GetRaycastResult(out var controlPoint5))
			{
				m_LastRaycastPoint = controlPoint5;
				m_ControlPoints[m_ControlPoints.Length - 1] = controlPoint5;
				inputDeps = SnapControlPoints(inputDeps, Entity.Null);
				inputDeps = UpdateDefinitions(inputDeps, Entity.Null);
			}
			else if (m_ControlPoints.Length >= 2)
			{
				m_ControlPoints[m_ControlPoints.Length - 1] = m_ControlPoints[m_ControlPoints.Length - 2];
				inputDeps = UpdateDefinitions(inputDeps, Entity.Null);
			}
			else
			{
				m_Tooltip.value = Tooltip.None;
			}
			return inputDeps;
		}
		case State.Modify:
		{
			m_ControlPoints.Clear();
			base.applyMode = ApplyMode.Clear;
			m_State = State.Default;
			if (GetRaycastResult(out var controlPoint4))
			{
				m_LastRaycastPoint = controlPoint4;
				m_ControlPoints.Add(ref controlPoint4);
				inputDeps = SnapControlPoints(inputDeps, Entity.Null);
				inputDeps = UpdateDefinitions(inputDeps, Entity.Null);
			}
			else
			{
				m_Tooltip.value = Tooltip.None;
			}
			return inputDeps;
		}
		case State.Remove:
		{
			Entity applyTempRoute = Entity.Null;
			if (GetAllowApply() && !((EntityQuery)(ref m_TempRouteQuery)).IsEmptyIgnoreFilter)
			{
				base.applyMode = ApplyMode.Apply;
				NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_TempRouteQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
				ArchetypeChunk val2 = val[0];
				applyTempRoute = ((ArchetypeChunk)(ref val2)).GetNativeArray(((ComponentSystemBase)this).GetEntityTypeHandle())[0];
				val.Dispose();
			}
			else
			{
				base.applyMode = ApplyMode.Clear;
			}
			m_State = State.Default;
			m_ControlPoints.Clear();
			if (GetRaycastResult(out var controlPoint))
			{
				m_LastRaycastPoint = controlPoint;
				m_ControlPoints.Add(ref controlPoint);
				inputDeps = SnapControlPoints(inputDeps, applyTempRoute);
				inputDeps = UpdateDefinitions(inputDeps, applyTempRoute);
			}
			else
			{
				m_Tooltip.value = Tooltip.None;
			}
			return inputDeps;
		}
		default:
			return Update(inputDeps, fullUpdate: false);
		}
	}

	private JobHandle Apply(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		ArchetypeChunk val2;
		switch (m_State)
		{
		case State.Default:
			if (GetAllowApply() && m_ControlPoints.Length > 0)
			{
				base.applyMode = ApplyMode.Clear;
				ControlPoint controlPoint2 = m_ControlPoints[0];
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_TransportLineStartSound);
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Route>(controlPoint2.m_OriginalEntity) && math.any(controlPoint2.m_ElementIndex >= 0))
				{
					m_State = State.Modify;
					m_ControlPointsMoved = controlPoint2.m_ElementIndex.y >= 0;
					m_MoveStartPosition = controlPoint2;
					m_ForceApply = singleFrameOnly;
					m_CanApplyModify = false;
					if (GetRaycastResult(out var controlPoint3))
					{
						m_LastRaycastPoint = controlPoint3;
						m_ControlPoints[0] = controlPoint3;
						inputDeps = SnapControlPoints(inputDeps, Entity.Null);
						JobHandle.ScheduleBatchedJobs();
						((JobHandle)(ref inputDeps)).Complete();
						ControlPoint other = m_ControlPoints[0];
						m_ControlPointsMoved |= !m_MoveStartPosition.Equals(other);
						inputDeps = UpdateDefinitions(inputDeps, Entity.Null);
					}
					else
					{
						m_Tooltip.value = Tooltip.None;
					}
					return inputDeps;
				}
				if (!controlPoint2.Equals(default(ControlPoint)))
				{
					m_State = State.Create;
					m_MoveStartPosition = default(ControlPoint);
					if (GetRaycastResult(out var controlPoint4))
					{
						m_LastRaycastPoint = controlPoint4;
						m_ControlPoints.Add(ref controlPoint4);
						inputDeps = SnapControlPoints(inputDeps, Entity.Null);
						inputDeps = UpdateDefinitions(inputDeps, Entity.Null);
					}
					else
					{
						m_ControlPoints.Add(ref controlPoint2);
						m_Tooltip.value = Tooltip.None;
					}
					return inputDeps;
				}
				return Update(inputDeps, fullUpdate: false);
			}
			return Update(inputDeps, fullUpdate: false);
		case State.Create:
			if (GetAllowApply() && !((EntityQuery)(ref m_TempRouteQuery)).IsEmptyIgnoreFilter && GetPathfindCompleted())
			{
				RouteData componentData = m_PrefabSystem.GetComponentData<RouteData>((PrefabBase)prefab);
				float num = math.distance(m_ControlPoints[m_ControlPoints.Length - 2].m_Position, m_ControlPoints[m_ControlPoints.Length - 1].m_Position);
				float minWaypointDistance = RouteUtils.GetMinWaypointDistance(componentData);
				if (num >= minWaypointDistance)
				{
					Entity applyTempRoute = Entity.Null;
					NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_TempRouteQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
					ComponentTypeHandle<Route> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Route>(ref __TypeHandle.__Game_Routes_Route_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
					val2 = val[0];
					if ((((ArchetypeChunk)(ref val2)).GetNativeArray<Route>(ref componentTypeHandle)[0].m_Flags & RouteFlags.Complete) != 0)
					{
						base.applyMode = ApplyMode.Apply;
						m_State = State.Default;
						m_ControlPoints.Clear();
						val2 = val[0];
						applyTempRoute = ((ArchetypeChunk)(ref val2)).GetNativeArray(((ComponentSystemBase)this).GetEntityTypeHandle())[0];
						m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_TransportLineCompleteSound);
					}
					else
					{
						m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_TransportLineBuildSound);
						base.applyMode = ApplyMode.Clear;
					}
					val.Dispose();
					if (GetRaycastResult(out var controlPoint5))
					{
						m_LastRaycastPoint = controlPoint5;
						m_ControlPoints.Add(ref controlPoint5);
						inputDeps = SnapControlPoints(inputDeps, applyTempRoute);
						inputDeps = UpdateDefinitions(inputDeps, applyTempRoute);
					}
					else
					{
						m_Tooltip.value = Tooltip.None;
					}
					return inputDeps;
				}
			}
			return Update(inputDeps, fullUpdate: false);
		case State.Modify:
		{
			bool allowApply = GetAllowApply();
			if (!m_ControlPointsMoved && allowApply && m_ControlPoints.Length > 0)
			{
				base.applyMode = ApplyMode.Clear;
				m_State = State.Create;
				m_MoveStartPosition = default(ControlPoint);
				if (GetRaycastResult(out var controlPoint6))
				{
					m_LastRaycastPoint = controlPoint6;
					m_ControlPoints.Add(ref controlPoint6);
					inputDeps = SnapControlPoints(inputDeps, Entity.Null);
					inputDeps = UpdateDefinitions(inputDeps, Entity.Null);
				}
				else
				{
					ref NativeList<ControlPoint> reference = ref m_ControlPoints;
					ControlPoint controlPoint7 = m_ControlPoints[0];
					reference.Add(ref controlPoint7);
					m_Tooltip.value = Tooltip.None;
				}
				return inputDeps;
			}
			if (m_CanApplyModify)
			{
				Entity applyTempRoute2 = Entity.Null;
				if (allowApply && !((EntityQuery)(ref m_TempRouteQuery)).IsEmptyIgnoreFilter)
				{
					base.applyMode = ApplyMode.Apply;
					NativeArray<ArchetypeChunk> val3 = ((EntityQuery)(ref m_TempRouteQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
					val2 = val3[0];
					applyTempRoute2 = ((ArchetypeChunk)(ref val2)).GetNativeArray(((ComponentSystemBase)this).GetEntityTypeHandle())[0];
					val3.Dispose();
				}
				else
				{
					base.applyMode = ApplyMode.Clear;
				}
				m_State = State.Default;
				m_ControlPoints.Clear();
				if (GetRaycastResult(out var controlPoint8))
				{
					m_LastRaycastPoint = controlPoint8;
					m_ControlPoints.Add(ref controlPoint8);
					inputDeps = SnapControlPoints(inputDeps, applyTempRoute2);
					inputDeps = UpdateDefinitions(inputDeps, applyTempRoute2);
				}
				else
				{
					m_Tooltip.value = Tooltip.None;
				}
				return inputDeps;
			}
			m_ForceApply = true;
			return Update(inputDeps, fullUpdate: false);
		}
		case State.Remove:
		{
			m_ControlPoints.Clear();
			base.applyMode = ApplyMode.Clear;
			m_State = State.Default;
			if (GetRaycastResult(out var controlPoint))
			{
				m_LastRaycastPoint = controlPoint;
				m_ControlPoints.Add(ref controlPoint);
				inputDeps = SnapControlPoints(inputDeps, Entity.Null);
				inputDeps = UpdateDefinitions(inputDeps, Entity.Null);
			}
			else
			{
				m_Tooltip.value = Tooltip.None;
			}
			return inputDeps;
		}
		default:
			return Update(inputDeps, fullUpdate: false);
		}
	}

	private bool GetPathfindCompleted()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_TempRouteQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			PathTargets pathTargets = default(PathTargets);
			Position position = default(Position);
			Position position2 = default(Position);
			for (int i = 0; i < val.Length; i++)
			{
				Entity val2 = val[i];
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<RouteWaypoint> buffer = ((EntityManager)(ref entityManager)).GetBuffer<RouteWaypoint>(val2, true);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<RouteSegment> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<RouteSegment>(val2, true);
				for (int j = 0; j < buffer2.Length; j++)
				{
					Entity segment = buffer2[j].m_Segment;
					if (EntitiesExtensions.TryGetComponent<PathTargets>(((ComponentSystemBase)this).EntityManager, segment, ref pathTargets))
					{
						RouteWaypoint routeWaypoint = buffer[j];
						RouteWaypoint routeWaypoint2 = buffer[math.select(j + 1, 0, j + 1 >= buffer.Length)];
						if (EntitiesExtensions.TryGetComponent<Position>(((ComponentSystemBase)this).EntityManager, routeWaypoint.m_Waypoint, ref position) && math.distancesq(pathTargets.m_ReadyStartPosition, position.m_Position) >= 1f)
						{
							return false;
						}
						if (EntitiesExtensions.TryGetComponent<Position>(((ComponentSystemBase)this).EntityManager, routeWaypoint2.m_Waypoint, ref position2) && math.distancesq(pathTargets.m_ReadyEndPosition, position2.m_Position) >= 1f)
						{
							return false;
						}
					}
				}
			}
		}
		finally
		{
			val.Dispose();
		}
		return true;
	}

	private bool CheckPathUpdates()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_EventQuery)).IsEmptyIgnoreFilter)
		{
			return false;
		}
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_EventQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			ComponentTypeHandle<PathUpdated> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PathUpdated>(ref __TypeHandle.__Game_Pathfind_PathUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Temp> componentLookup = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<PathUpdated> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PathUpdated>(ref componentTypeHandle);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					if (componentLookup.HasComponent(nativeArray[j].m_Owner))
					{
						return true;
					}
				}
			}
		}
		finally
		{
			val.Dispose();
		}
		return false;
	}

	private JobHandle Update(JobHandle inputDeps, bool fullUpdate)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		bool flag = CheckPathUpdates();
		if (m_State == State.Modify)
		{
			m_CanApplyModify = true;
		}
		if (GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate))
		{
			forceUpdate = forceUpdate || fullUpdate;
			if (m_ControlPoints.Length == 0)
			{
				m_LastRaycastPoint = controlPoint;
				m_ControlPoints.Add(ref controlPoint);
				inputDeps = SnapControlPoints(inputDeps, Entity.Null);
				base.applyMode = ApplyMode.Clear;
				return UpdateDefinitions(inputDeps, Entity.Null);
			}
			if (m_LastRaycastPoint.Equals(controlPoint) && !flag && !forceUpdate)
			{
				base.applyMode = ApplyMode.None;
				return inputDeps;
			}
			m_LastRaycastPoint = controlPoint;
			ControlPoint controlPoint2 = m_ControlPoints[m_ControlPoints.Length - 1];
			m_ControlPoints[m_ControlPoints.Length - 1] = controlPoint;
			inputDeps = SnapControlPoints(inputDeps, Entity.Null);
			JobHandle.ScheduleBatchedJobs();
			((JobHandle)(ref inputDeps)).Complete();
			ControlPoint other = m_ControlPoints[m_ControlPoints.Length - 1];
			if (controlPoint2.EqualsIgnoreHit(other) && !flag && !forceUpdate)
			{
				base.applyMode = ApplyMode.None;
			}
			else
			{
				m_ControlPointsMoved = true;
				base.applyMode = ApplyMode.Clear;
				inputDeps = UpdateDefinitions(inputDeps, Entity.Null);
			}
			return inputDeps;
		}
		if (m_LastRaycastPoint.Equals(controlPoint))
		{
			forceUpdate = forceUpdate || fullUpdate;
			if (flag || forceUpdate)
			{
				base.applyMode = ApplyMode.Clear;
				if (m_ControlPoints.Length > 0)
				{
					return UpdateDefinitions(inputDeps, Entity.Null);
				}
				return inputDeps;
			}
			base.applyMode = ApplyMode.None;
			return inputDeps;
		}
		m_LastRaycastPoint = controlPoint;
		if (m_State == State.Default && m_ControlPoints.Length == 1)
		{
			base.applyMode = ApplyMode.Clear;
			m_ControlPoints[m_ControlPoints.Length - 1] = default(ControlPoint);
			return UpdateDefinitions(inputDeps, Entity.Null);
		}
		if (m_State == State.Modify && m_ControlPoints.Length >= 1)
		{
			m_ControlPointsMoved = true;
			base.applyMode = ApplyMode.Clear;
			m_ControlPoints[m_ControlPoints.Length - 1] = m_MoveStartPosition;
			return UpdateDefinitions(inputDeps, Entity.Null);
		}
		if (m_State == State.Remove && m_ControlPoints.Length >= 1)
		{
			m_ControlPointsMoved = true;
			base.applyMode = ApplyMode.Clear;
			ControlPoint controlPoint3 = m_MoveStartPosition;
			controlPoint3.m_OriginalEntity = Entity.Null;
			m_ControlPoints[m_ControlPoints.Length - 1] = controlPoint3;
			return UpdateDefinitions(inputDeps, Entity.Null);
		}
		if (m_ControlPoints.Length >= 2)
		{
			m_ControlPointsMoved = true;
			base.applyMode = ApplyMode.Clear;
			m_ControlPoints[m_ControlPoints.Length - 1] = m_ControlPoints[m_ControlPoints.Length - 2];
			return UpdateDefinitions(inputDeps, Entity.Null);
		}
		m_Tooltip.value = Tooltip.None;
		return inputDeps;
	}

	private Entity GetUpgradable(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Attached attached = default(Attached);
		if (EntitiesExtensions.TryGetComponent<Attached>(((ComponentSystemBase)this).EntityManager, entity, ref attached))
		{
			return attached.m_Parent;
		}
		return entity;
	}

	private JobHandle SnapControlPoints(JobHandle inputDeps, Entity applyTempRoute)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		return IJobExtensions.Schedule<SnapJob>(new SnapJob
		{
			m_Snap = GetActualSnap(),
			m_State = m_State,
			m_Prefab = m_PrefabSystem.GetEntity(prefab),
			m_ApplyTempRoute = applyTempRoute,
			m_MoveStartPosition = m_MoveStartPosition,
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRouteData = InternalCompilerInterface.GetComponentLookup<RouteData>(ref __TypeHandle.__Game_Prefabs_RouteData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRouteConnectionData = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransportStopData = InternalCompilerInterface.GetComponentLookup<TransportStopData>(ref __TypeHandle.__Game_Prefabs_TransportStopData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedRoutes = InternalCompilerInterface.GetBufferLookup<ConnectedRoute>(ref __TypeHandle.__Game_Routes_ConnectedRoute_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Waypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControlPoints = m_ControlPoints
		}, inputDeps);
	}

	private JobHandle UpdateDefinitions(JobHandle inputDeps, Entity applyTempRoute)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		if ((Object)(object)prefab != (Object)null)
		{
			CreateDefinitionsJob createDefinitionsJob = new CreateDefinitionsJob
			{
				m_State = m_State,
				m_Prefab = m_PrefabSystem.GetEntity(prefab),
				m_ApplyTempRoute = applyTempRoute,
				m_MoveStartPosition = m_MoveStartPosition,
				m_PrefabRouteData = InternalCompilerInterface.GetComponentLookup<RouteData>(ref __TypeHandle.__Game_Prefabs_RouteData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedRoutes = InternalCompilerInterface.GetBufferLookup<ConnectedRoute>(ref __TypeHandle.__Game_Routes_ConnectedRoute_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Waypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ControlPoints = m_ControlPoints,
				m_Tooltip = m_Tooltip,
				m_Color = color,
				m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
			};
			if (serviceUpgrade)
			{
				createDefinitionsJob.m_ServiceUpgradeOwner = GetUpgradable(m_ToolSystem.selected);
			}
			JobHandle val2 = IJobExtensions.Schedule<CreateDefinitionsJob>(createDefinitionsJob, inputDeps);
			((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val2);
			val = JobHandle.CombineDependencies(val, val2);
		}
		return val;
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
		base.OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public RouteToolSystem()
	{
	}
}

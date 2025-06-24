using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Pathfind;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Routes;

public static class RouteUtils
{
	public interface ITransportEstimateBuffer
	{
		void AddWaitEstimate(Entity waypoint, int seconds);
	}

	public const float WAYPOINT_CONNECTION_DISTANCE = 10f;

	public const float ROUTE_VISIBLE_THROUGH_DISTANCE = 100f;

	public const float TRANSPORT_DAY_START_TIME = 0.25f;

	public const float TRANSPORT_DAY_END_TIME = 11f / 12f;

	public const float DEFAULT_TRAVEL_TIME = 1f / 48f;

	public const float TAXI_DISTANCE_FEE = 0.03f;

	public static float GetMinWaypointDistance(RouteData routeData)
	{
		return routeData.m_SnapDistance * 0.5f;
	}

	public static Bounds3 CalculateBounds(Position waypointPosition, RouteData routeData)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		float snapDistance = routeData.m_SnapDistance;
		return new Bounds3(waypointPosition.m_Position - snapDistance, waypointPosition.m_Position + snapDistance);
	}

	public static Bounds3 CalculateBounds(CurveElement curveElement, RouteData routeData)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		float snapDistance = routeData.m_SnapDistance;
		return MathUtils.Expand(MathUtils.Bounds(curveElement.m_Curve), float3.op_Implicit(snapDistance));
	}

	public static void StripTransportSegments<TTransportEstimateBuffer>(ref Random random, int length, DynamicBuffer<PathElement> path, ComponentLookup<Connected> connectedData, ComponentLookup<BoardingVehicle> boardingVehicleData, ComponentLookup<Owner> ownerData, ComponentLookup<Lane> laneData, ComponentLookup<Game.Net.ConnectionLane> connectionLaneData, ComponentLookup<Curve> curveData, ComponentLookup<PrefabRef> prefabRefData, ComponentLookup<TransportStopData> prefabTransportStopData, BufferLookup<Game.Net.SubLane> subLanes, BufferLookup<Game.Areas.Node> areaNodes, BufferLookup<Triangle> areaTriangles, TTransportEstimateBuffer transportEstimateBuffer) where TTransportEstimateBuffer : unmanaged, ITransportEstimateBuffer
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		TransportStopData transportStopData = default(TransportStopData);
		Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
		while (num < length)
		{
			PathElement pathElement = path[num++];
			Entity val = Entity.Null;
			int num2 = -1;
			if (connectedData.HasComponent(pathElement.m_Target))
			{
				Connected connected = connectedData[pathElement.m_Target];
				if (boardingVehicleData.HasComponent(connected.m_Connected))
				{
					val = connected.m_Connected;
					num2 = num - 2;
				}
				int i;
				for (i = num; i < length && !connectedData.HasComponent(path[i].m_Target); i++)
				{
				}
				if (i > num)
				{
					path.RemoveRange(num, i - num);
					length -= i - num;
				}
				num = i;
			}
			else if (boardingVehicleData.HasComponent(pathElement.m_Target))
			{
				val = pathElement.m_Target;
				num2 = num - 2;
			}
			if (!(val != Entity.Null) || !prefabTransportStopData.TryGetComponent(prefabRefData[val].m_Prefab, ref transportStopData))
			{
				continue;
			}
			if (num2 >= 0 && transportStopData.m_AccessDistance > 0f)
			{
				PathElement pathElement2 = path[num2];
				int length2 = path.Length;
				if (connectionLaneData.TryGetComponent(pathElement2.m_Target, ref connectionLane))
				{
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Area) != 0)
					{
						OffsetPathTarget_AreaLane(ref random, transportStopData.m_AccessDistance, num2, path, ownerData, curveData, laneData, connectionLaneData, subLanes, areaNodes, areaTriangles);
					}
				}
				else if (curveData.HasComponent(pathElement2.m_Target))
				{
					OffsetPathTarget_EdgeLane(ref random, transportStopData.m_AccessDistance, num2, path, ownerData, laneData, curveData, subLanes);
				}
				num += path.Length - length2;
				length += path.Length - length2;
			}
			if (transportStopData.m_BoardingTime > 0f)
			{
				int num3 = MathUtils.RoundToIntRandom(ref random, transportStopData.m_BoardingTime);
				if (num3 > 0)
				{
					transportEstimateBuffer.AddWaitEstimate(pathElement.m_Target, num3);
				}
			}
		}
	}

	private static void OffsetPathTarget_AreaLane(ref Random random, float distance, int elementIndex, DynamicBuffer<PathElement> path, ComponentLookup<Owner> ownerData, ComponentLookup<Curve> curveData, ComponentLookup<Lane> laneData, ComponentLookup<Game.Net.ConnectionLane> connectionLaneData, BufferLookup<Game.Net.SubLane> subLanes, BufferLookup<Game.Areas.Node> areaNodes, BufferLookup<Triangle> areaTriangles)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		PathElement pathElement = path[elementIndex];
		Curve curve = curveData[pathElement.m_Target];
		Entity owner = ownerData[pathElement.m_Target].m_Owner;
		float3 val = MathUtils.Position(curve.m_Bezier, pathElement.m_TargetDelta.y);
		DynamicBuffer<Game.Areas.Node> nodes = areaNodes[owner];
		DynamicBuffer<Triangle> val2 = areaTriangles[owner];
		int num = -1;
		float num2 = 0f;
		float2 val3 = default(float2);
		for (int i = 0; i < val2.Length; i++)
		{
			Triangle3 triangle = AreaUtils.GetTriangle3(nodes, val2[i]);
			if (!(MathUtils.Distance(triangle, val, ref val3) >= distance))
			{
				float num3 = MathUtils.Area(((Triangle3)(ref triangle)).xz);
				num2 += num3;
				if (((Random)(ref random)).NextFloat(num2) < num3)
				{
					num = i;
				}
			}
		}
		if (num == -1)
		{
			return;
		}
		DynamicBuffer<Game.Net.SubLane> lanes = subLanes[owner];
		float2 val4 = ((Random)(ref random)).NextFloat2(float2.op_Implicit(1f));
		val4 = math.select(val4, 1f - val4, math.csum(val4) > 1f);
		Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, val2[num]);
		float3 val5 = MathUtils.Position(triangle2, val4);
		float num4 = float.MaxValue;
		Entity val6 = Entity.Null;
		float endCurvePos = 0f;
		bool2 val7 = default(bool2);
		float num6 = default(float);
		for (int j = 0; j < lanes.Length; j++)
		{
			Entity subLane = lanes[j].m_SubLane;
			if (!connectionLaneData.HasComponent(subLane) || (connectionLaneData[subLane].m_Flags & ConnectionLaneFlags.Pedestrian) == 0)
			{
				continue;
			}
			curve = curveData[subLane];
			((bool2)(ref val7))._002Ector(MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.a)).xz, ref val3), MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.d)).xz, ref val3));
			if (math.any(val7))
			{
				float num5 = MathUtils.Distance(curve.m_Bezier, val5, ref num6);
				if (num5 < num4)
				{
					float2 val8 = math.select(new float2(0f, 0.49f), math.select(new float2(0.51f, 1f), new float2(0f, 1f), val7.x), val7.y);
					num4 = num5;
					val6 = subLane;
					endCurvePos = ((Random)(ref random)).NextFloat(val8.x, val8.y);
				}
			}
		}
		if (val6 == Entity.Null)
		{
			Debug.Log((object)$"Target path lane not found ({val5.x}, {val5.y}, {val5.z})");
			return;
		}
		int num7 = elementIndex;
		Owner owner2 = default(Owner);
		while (num7 > 0 && ownerData.TryGetComponent(path[num7 - 1].m_Target, ref owner2) && !(owner2.m_Owner != owner))
		{
			num7--;
		}
		NativeList<PathElement> path2 = default(NativeList<PathElement>);
		path2._002Ector(lanes.Length, AllocatorHandle.op_Implicit((Allocator)2));
		PathElement pathElement2 = path[num7];
		AreaUtils.FindAreaPath(ref random, path2, lanes, pathElement2.m_Target, pathElement2.m_TargetDelta.x, val6, endCurvePos, laneData, curveData);
		if (path2.Length != 0)
		{
			int num8 = elementIndex - num7 + 1;
			int num9 = math.min(num8, path2.Length);
			for (int k = 0; k < num9; k++)
			{
				path[num7 + k] = path2[k];
			}
			if (path2.Length < num8)
			{
				path.RemoveRange(num7 + path2.Length, num8 - path2.Length);
			}
			else
			{
				for (int l = num8; l < path2.Length; l++)
				{
					path.Insert(num7 + l, path2[l]);
				}
			}
		}
		path2.Dispose();
	}

	private static void OffsetPathTarget_EdgeLane(ref Random random, float distance, int elementIndex, DynamicBuffer<PathElement> path, ComponentLookup<Owner> ownerData, ComponentLookup<Lane> laneData, ComponentLookup<Curve> curveData, BufferLookup<Game.Net.SubLane> subLanes)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		PathElement pathElement = path[elementIndex];
		Curve curve = curveData[pathElement.m_Target];
		float num = ((Random)(ref random)).NextFloat(0f - distance, distance);
		if (num >= 0f)
		{
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(pathElement.m_TargetDelta.y, 1f);
			float num2 = num;
			if (MathUtils.ClampLength(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val, ref num2))
			{
				pathElement.m_TargetDelta.y = val.max;
				path[elementIndex] = pathElement;
				return;
			}
			Entity entity = pathElement.m_Target;
			if (NetUtils.FindNextLane(ref entity, ref ownerData, ref laneData, ref subLanes))
			{
				num = math.max(0f, num - num2);
				((Bounds1)(ref val))._002Ector(0f, 1f);
				curve = curveData[entity];
				MathUtils.ClampLength(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val, num);
				if (elementIndex > 0 && path[elementIndex - 1].m_Target == entity)
				{
					path.RemoveAt(elementIndex--);
					pathElement = path[elementIndex];
					pathElement.m_TargetDelta.y = val.max;
					path[elementIndex] = pathElement;
				}
				else
				{
					PathElement pathElement2 = new PathElement
					{
						m_Target = pathElement.m_Target,
						m_TargetDelta = new float2(pathElement.m_TargetDelta.x, 1f)
					};
					path.Insert(elementIndex++, pathElement2);
					pathElement.m_Target = entity;
					pathElement.m_TargetDelta = new float2(0f, val.max);
					path[elementIndex] = pathElement;
				}
			}
			else
			{
				pathElement.m_TargetDelta.y = math.saturate(pathElement.m_TargetDelta.y + (1f - pathElement.m_TargetDelta.y) * num / distance);
				path[elementIndex] = pathElement;
			}
			return;
		}
		num = 0f - num;
		Bounds1 val2 = default(Bounds1);
		((Bounds1)(ref val2))._002Ector(0f, pathElement.m_TargetDelta.y);
		float num3 = num;
		if (MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val2, ref num3))
		{
			pathElement.m_TargetDelta.y = val2.min;
			path[elementIndex] = pathElement;
			return;
		}
		Entity entity2 = pathElement.m_Target;
		if (NetUtils.FindPrevLane(ref entity2, ref ownerData, ref laneData, ref subLanes))
		{
			num = math.max(0f, num - num3);
			((Bounds1)(ref val2))._002Ector(0f, 1f);
			curve = curveData[entity2];
			MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val2, num);
			if (elementIndex > 0 && path[elementIndex - 1].m_Target == entity2)
			{
				path.RemoveAt(elementIndex--);
				pathElement = path[elementIndex];
				pathElement.m_TargetDelta.y = val2.min;
				path[elementIndex] = pathElement;
			}
			else
			{
				PathElement pathElement3 = new PathElement
				{
					m_Target = pathElement.m_Target,
					m_TargetDelta = new float2(pathElement.m_TargetDelta.x, 0f)
				};
				path.Insert(elementIndex++, pathElement3);
				pathElement.m_Target = entity2;
				pathElement.m_TargetDelta = new float2(1f, val2.min);
				path[elementIndex] = pathElement;
			}
		}
		else
		{
			pathElement.m_TargetDelta.y = math.saturate(pathElement.m_TargetDelta.y - pathElement.m_TargetDelta.y * num / distance);
			path[elementIndex] = pathElement;
		}
	}

	public static bool GetBoardingVehicle(Entity currentLane, Entity currentWaypoint, Entity targetWaypoint, uint minDeparture, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Connected> connectedData, ref ComponentLookup<BoardingVehicle> boardingVehicleData, ref ComponentLookup<CurrentRoute> currentRouteData, ref ComponentLookup<AccessLane> accessLaneData, ref ComponentLookup<Game.Vehicles.PublicTransport> publicTransportData, ref ComponentLookup<Game.Vehicles.Taxi> taxiData, ref BufferLookup<ConnectedRoute> connectedRoutes, out Entity vehicle, out bool testing, out bool obsolete)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		AccessLane accessLane = default(AccessLane);
		if (currentLane != currentWaypoint && accessLaneData.TryGetComponent(currentWaypoint, ref accessLane))
		{
			Entity val = Entity.Null;
			Entity val2 = Entity.Null;
			Owner owner = default(Owner);
			if (ownerData.TryGetComponent(currentLane, ref owner))
			{
				val = owner.m_Owner;
			}
			Owner owner2 = default(Owner);
			if (ownerData.TryGetComponent(accessLane.m_Lane, ref owner2))
			{
				val2 = owner2.m_Owner;
			}
			Connected connected = default(Connected);
			if (val != val2 && (!connectedData.TryGetComponent(currentWaypoint, ref connected) || connected.m_Connected != currentLane))
			{
				vehicle = Entity.Null;
				testing = false;
				obsolete = true;
				return false;
			}
		}
		BoardingVehicle boardingVehicle = default(BoardingVehicle);
		if (boardingVehicleData.TryGetComponent(currentWaypoint, ref boardingVehicle))
		{
			Game.Vehicles.Taxi taxi = default(Game.Vehicles.Taxi);
			if (boardingVehicle.m_Vehicle != Entity.Null && taxiData.TryGetComponent(boardingVehicle.m_Vehicle, ref taxi) && (taxi.m_State & TaxiFlags.Boarding) != 0)
			{
				vehicle = boardingVehicle.m_Vehicle;
				testing = false;
				obsolete = false;
				return true;
			}
			vehicle = Entity.Null;
			testing = false;
			obsolete = false;
			return false;
		}
		Connected connected2 = default(Connected);
		Connected connected3 = default(Connected);
		if (connectedData.TryGetComponent(currentWaypoint, ref connected2) && connectedData.TryGetComponent(targetWaypoint, ref connected3))
		{
			Entity connected4 = connected2.m_Connected;
			Entity connected5 = connected3.m_Connected;
			DynamicBuffer<ConnectedRoute> val3 = default(DynamicBuffer<ConnectedRoute>);
			if (boardingVehicleData.TryGetComponent(connected4, ref boardingVehicle) && connectedRoutes.TryGetBuffer(connected5, ref val3))
			{
				CurrentRoute currentRoute = default(CurrentRoute);
				Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
				if (currentRouteData.TryGetComponent(boardingVehicle.m_Vehicle, ref currentRoute) && (!publicTransportData.TryGetComponent(boardingVehicle.m_Vehicle, ref publicTransport) || ((publicTransport.m_State & (PublicTransportFlags.EnRoute | PublicTransportFlags.Boarding)) == (PublicTransportFlags.EnRoute | PublicTransportFlags.Boarding) && (publicTransport.m_DepartureFrame >= minDeparture || publicTransport.m_MaxBoardingDistance != float.MaxValue))))
				{
					for (int i = 0; i < val3.Length; i++)
					{
						if (ownerData[val3[i].m_Waypoint].m_Owner == currentRoute.m_Route)
						{
							vehicle = boardingVehicle.m_Vehicle;
							testing = false;
							obsolete = false;
							return true;
						}
					}
				}
				Game.Vehicles.PublicTransport publicTransport2 = default(Game.Vehicles.PublicTransport);
				if (currentRouteData.TryGetComponent(boardingVehicle.m_Testing, ref currentRoute) && (!publicTransportData.TryGetComponent(boardingVehicle.m_Testing, ref publicTransport2) || (publicTransport2.m_State & (PublicTransportFlags.EnRoute | PublicTransportFlags.Testing | PublicTransportFlags.RequireStop)) == (PublicTransportFlags.EnRoute | PublicTransportFlags.Testing)))
				{
					for (int j = 0; j < val3.Length; j++)
					{
						if (ownerData[val3[j].m_Waypoint].m_Owner == currentRoute.m_Route)
						{
							vehicle = boardingVehicle.m_Testing;
							testing = true;
							obsolete = false;
							return false;
						}
					}
				}
				vehicle = Entity.Null;
				testing = false;
				obsolete = false;
				return false;
			}
		}
		vehicle = Entity.Null;
		testing = false;
		obsolete = true;
		return false;
	}

	public static bool ShouldExitVehicle(Entity nextLane, Entity targetWaypoint, Entity currentVehicle, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Connected> connectedData, ref ComponentLookup<BoardingVehicle> boardingVehicleData, ref ComponentLookup<CurrentRoute> currentRouteData, ref ComponentLookup<AccessLane> accessLaneData, ref ComponentLookup<Game.Vehicles.PublicTransport> publicTransportData, ref BufferLookup<ConnectedRoute> connectedRoutes, bool testing, out bool obsolete)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Connected connected = default(Connected);
		CurrentRoute currentRoute = default(CurrentRoute);
		if (connectedData.TryGetComponent(targetWaypoint, ref connected) && currentRouteData.TryGetComponent(currentVehicle, ref currentRoute))
		{
			Entity connected2 = connected.m_Connected;
			BoardingVehicle boardingVehicle = default(BoardingVehicle);
			DynamicBuffer<ConnectedRoute> val = default(DynamicBuffer<ConnectedRoute>);
			if (boardingVehicleData.TryGetComponent(connected2, ref boardingVehicle) && connectedRoutes.TryGetBuffer(connected2, ref val))
			{
				if ((testing ? boardingVehicle.m_Testing : boardingVehicle.m_Vehicle) == currentVehicle)
				{
					obsolete = false;
					AccessLane accessLane = default(AccessLane);
					if (nextLane != Entity.Null && accessLaneData.TryGetComponent(targetWaypoint, ref accessLane))
					{
						Entity val2 = Entity.Null;
						Entity val3 = Entity.Null;
						Owner owner = default(Owner);
						if (ownerData.TryGetComponent(nextLane, ref owner))
						{
							val2 = owner.m_Owner;
						}
						Owner owner2 = default(Owner);
						if (ownerData.TryGetComponent(accessLane.m_Lane, ref owner2))
						{
							val3 = owner2.m_Owner;
						}
						if (val2 != val3)
						{
							obsolete = true;
						}
					}
					return true;
				}
				Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
				if (publicTransportData.TryGetComponent(currentVehicle, ref publicTransport) && (publicTransport.m_State & PublicTransportFlags.EnRoute) == 0)
				{
					obsolete = true;
					return true;
				}
				for (int i = 0; i < val.Length; i++)
				{
					if (ownerData[val[i].m_Waypoint].m_Owner == currentRoute.m_Route)
					{
						obsolete = false;
						return false;
					}
				}
			}
		}
		obsolete = true;
		return true;
	}

	public static float UpdateAverageTravelTime(float oldTravelTime, uint departureFrame, uint arrivalFrame)
	{
		if (departureFrame == 0)
		{
			return oldTravelTime;
		}
		float num = (float)(arrivalFrame - departureFrame) / 60f;
		if (oldTravelTime == 0f)
		{
			return num;
		}
		return math.lerp(oldTravelTime, num, 0.5f);
	}

	public static float GetStopDuration(TransportLineData prefabLineData, TransportStop transportStop)
	{
		return prefabLineData.m_StopDuration / math.max(0.25f, transportStop.m_LoadingFactor);
	}

	public static uint CalculateDepartureFrame(TransportLine transportLine, TransportLineData prefabLineData, DynamicBuffer<RouteModifier> routeModifiers, float targetStopTime, uint lastDepartureFrame, uint simulationFrame)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)(simulationFrame - lastDepartureFrame) / 60f;
		if (num >= 0f)
		{
			float value = prefabLineData.m_DefaultVehicleInterval;
			ApplyModifier(ref value, routeModifiers, RouteModifierType.VehicleInterval);
			float vehicleInterval = transportLine.m_VehicleInterval;
			float unbunchingFactor = transportLine.m_UnbunchingFactor;
			float num2 = math.min(value, 2f * vehicleInterval * vehicleInterval / (num + vehicleInterval) - vehicleInterval) * unbunchingFactor;
			num2 = math.max(num2 + targetStopTime, 1f);
			return simulationFrame + (uint)(num2 * 60f);
		}
		return simulationFrame;
	}

	public static PathMethod GetPathMethods(RouteConnectionType routeConnectionType, RouteType routeType, TrackTypes trackTypes, RoadTypes roadTypes, SizeClass sizeClass)
	{
		switch (routeConnectionType)
		{
		case RouteConnectionType.Pedestrian:
			return PathMethod.Pedestrian;
		case RouteConnectionType.Road:
		case RouteConnectionType.Air:
		{
			PathMethod pathMethod = PathMethod.Road;
			if (routeType == RouteType.WorkRoute)
			{
				pathMethod |= PathMethod.Offroad;
			}
			if ((int)sizeClass <= 1)
			{
				pathMethod |= PathMethod.MediumRoad;
			}
			if ((roadTypes & (RoadTypes.Helicopter | RoadTypes.Airplane)) != RoadTypes.None)
			{
				pathMethod |= PathMethod.Flying;
			}
			return pathMethod;
		}
		case RouteConnectionType.Track:
			return PathMethod.Track;
		default:
			return (PathMethod)0;
		}
	}

	public static bool CheckOption(Route route, RouteOption option)
	{
		return (route.m_OptionMask & (uint)(1 << (int)option)) != 0;
	}

	public static bool HasOption(RouteOptionData optionData, RouteOption option)
	{
		return (optionData.m_OptionMask & (uint)(1 << (int)option)) != 0;
	}

	public static void ApplyModifier(ref float value, DynamicBuffer<RouteModifier> modifiers, RouteModifierType type)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (modifiers.Length > (int)type)
		{
			float2 delta = modifiers[(int)type].m_Delta;
			value += delta.x;
			value += value * delta.y;
		}
	}

	public static PathMethod GetTaxiMethods(Game.Creatures.Resident resident)
	{
		if ((resident.m_Flags & ResidentFlags.IgnoreTaxi) != ResidentFlags.None)
		{
			return (PathMethod)0;
		}
		return PathMethod.Taxi;
	}

	public static PathMethod GetPublicTransportMethods(float timeOfDay, float predictionOffset = 1f / 48f)
	{
		timeOfDay = math.frac(timeOfDay + predictionOffset);
		if (!(timeOfDay >= 0.25f) || !(timeOfDay < 11f / 12f))
		{
			return PathMethod.PublicTransportNight;
		}
		return PathMethod.PublicTransportDay;
	}

	public static PathMethod GetPublicTransportMethods(Game.Creatures.Resident resident, float timeOfDay, float predictionOffset = 1f / 48f)
	{
		if ((resident.m_Flags & ResidentFlags.IgnoreTransport) != ResidentFlags.None)
		{
			return (PathMethod)0;
		}
		timeOfDay = math.frac(timeOfDay + predictionOffset);
		if (!(timeOfDay >= 0.25f) || !(timeOfDay < 11f / 12f))
		{
			return PathMethod.PublicTransportNight;
		}
		return PathMethod.PublicTransportDay;
	}

	public static bool CheckVehicleModel(VehicleModel vehicleModel, PrefabRef prefabRef, DynamicBuffer<LayoutElement> layout, ref ComponentLookup<PrefabRef> prefabRefData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (vehicleModel.m_PrimaryPrefab != Entity.Null)
		{
			if (prefabRef.m_Prefab != vehicleModel.m_PrimaryPrefab)
			{
				return false;
			}
			if (vehicleModel.m_SecondaryPrefab != Entity.Null)
			{
				if (layout.IsCreated)
				{
					for (int i = 0; i < layout.Length; i++)
					{
						prefabRef = prefabRefData[layout[i].m_Vehicle];
						if ((Entity)prefabRef == vehicleModel.m_SecondaryPrefab)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		return true;
	}

	public static int GetMaxTaxiCount(WaitingPassengers waitingPassengers)
	{
		return 3 + (waitingPassengers.m_Count + 3 >> 2);
	}
}

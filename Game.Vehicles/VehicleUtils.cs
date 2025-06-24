using System;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Vehicles;

public static class VehicleUtils
{
	public const float MAX_VEHICLE_SPEED = 277.77777f;

	public const float MAX_CAR_SPEED = 111.111115f;

	public const float MAX_TRAIN_SPEED = 138.88889f;

	public const float MAX_WATERCRAFT_SPEED = 55.555557f;

	public const float MAX_HELICOPTER_SPEED = 83.333336f;

	public const float MAX_AIRPLANE_SPEED = 277.77777f;

	public const float MAX_CAR_LENGTH = 20f;

	public const float PARALLEL_PARKING_OFFSET = 1f;

	public const float CAR_CRAWL_SPEED = 3f;

	public const float CAR_AREA_SPEED = 11.111112f;

	public const float MIN_HIGHWAY_SPEED = 22.222223f;

	public const float MAX_WATERCRAFT_LENGTH = 150f;

	public const float WATERCRAFT_AREA_SPEED = 11.111112f;

	public const float MAX_FIRE_ENGINE_EXTINGUISH_DISTANCE = 30f;

	public const float MAX_POLICE_ACCIDENT_TARGET_DISTANCE = 30f;

	public const float MAX_MAINTENANCE_TARGET_DISTANCE = 30f;

	public const uint MAINTENANCE_DESTROYED_CLEAR_AMOUNT = 500u;

	public const float MAX_TRAIN_LENGTH = 200f;

	public const float TRAIN_CRAWL_SPEED = 3f;

	public const float MAX_TRAIN_CARRIAGE_LENGTH = 20f;

	public const float MAX_TRAM_CARRIAGE_LENGTH = 16f;

	public const float MAX_SUBWAY_LENGTH = 200f;

	public const float MAX_SUBWAY_CARRIAGE_LENGTH = 18f;

	public const int CAR_NAVIGATION_LANE_CAPACITY = 8;

	public const int CAR_PARALLEL_LANE_CAPACITY = 8;

	public const int WATERCRAFT_NAVIGATION_LANE_CAPACITY = 8;

	public const int WATERCRAFT_PARALLEL_LANE_CAPACITY = 8;

	public const int AIRCRAFT_NAVIGATION_LANE_CAPACITY = 8;

	public const float MIN_HELICOPTER_NAVIGATION_DISTANCE = 750f;

	public const float MIN_AIRPLANE_NAVIGATION_DISTANCE = 1500f;

	public const float AIRPLANE_FLY_HEIGHT = 1000f;

	public const float HELICOPTER_FLY_HEIGHT = 100f;

	public const float ROCKET_FLY_HEIGHT = 10000f;

	public const uint BOTTLENECK_LIMIT = 50u;

	public const uint STUCK_MAX_COUNT = 100u;

	public const int STUCK_MAX_SPEED = 6;

	public const float TEMP_WAIT_TIME = 5f;

	public const float DELIVERY_PATHFIND_RANDOM_COST = 30f;

	public const float SERVICE_PATHFIND_RANDOM_COST = 30f;

	public const int PRIORITY_OFFSET = 2;

	public const int NORMAL_CAR_PRIORITY = 100;

	public const int TRACK_RESERVE_PRIORITY = 98;

	public const int REQUEST_SPACE_PRIORITY = 96;

	public const int EMERGENCY_YIELD_PRIORITY = 102;

	public const int NORMAL_TRAIN_PRIORITY = 104;

	public const int EMERGENCY_FLEE_PRIORITY = 106;

	public const int EMERGENCY_CAR_PRIORITY = 108;

	public const int PRIMARY_TRAIN_PRIORITY = 110;

	public const int SMALL_WATERCRAFT_PRIORITY = 100;

	public const int MEDIUM_WATERCRAFT_PRIORITY = 102;

	public const int LARGE_WATERCRAFT_PRIORITY = 104;

	public const int NORMAL_AIRCRAFT_PRIORITY = 104;

	public static bool PathfindFailed(PathOwner pathOwner)
	{
		return (pathOwner.m_State & (PathFlags.Failed | PathFlags.Stuck)) != 0;
	}

	public static bool PathEndReached(CarCurrentLane currentLane)
	{
		return (currentLane.m_LaneFlags & (CarLaneFlags.EndOfPath | CarLaneFlags.EndReached | CarLaneFlags.ParkingSpace | CarLaneFlags.Waypoint)) == (CarLaneFlags.EndOfPath | CarLaneFlags.EndReached);
	}

	public static bool PathEndReached(TrainCurrentLane currentLane)
	{
		return (currentLane.m_Front.m_LaneFlags & (TrainLaneFlags.EndOfPath | TrainLaneFlags.EndReached)) == (TrainLaneFlags.EndOfPath | TrainLaneFlags.EndReached);
	}

	public static bool ReturnEndReached(TrainCurrentLane currentLane)
	{
		return (currentLane.m_Front.m_LaneFlags & (TrainLaneFlags.EndReached | TrainLaneFlags.Return)) == (TrainLaneFlags.EndReached | TrainLaneFlags.Return);
	}

	public static bool PathEndReached(WatercraftCurrentLane currentLane)
	{
		return (currentLane.m_LaneFlags & (WatercraftLaneFlags.EndOfPath | WatercraftLaneFlags.EndReached)) == (WatercraftLaneFlags.EndOfPath | WatercraftLaneFlags.EndReached);
	}

	public static bool PathEndReached(AircraftCurrentLane currentLane)
	{
		return (currentLane.m_LaneFlags & (AircraftLaneFlags.EndOfPath | AircraftLaneFlags.EndReached)) == (AircraftLaneFlags.EndOfPath | AircraftLaneFlags.EndReached);
	}

	public static bool ParkingSpaceReached(CarCurrentLane currentLane, PathOwner pathOwner)
	{
		if ((currentLane.m_LaneFlags & (CarLaneFlags.EndReached | CarLaneFlags.ParkingSpace)) == (CarLaneFlags.EndReached | CarLaneFlags.ParkingSpace))
		{
			return (pathOwner.m_State & PathFlags.Pending) == 0;
		}
		return false;
	}

	public static bool ParkingSpaceReached(AircraftCurrentLane currentLane, PathOwner pathOwner)
	{
		if ((currentLane.m_LaneFlags & (AircraftLaneFlags.EndReached | AircraftLaneFlags.ParkingSpace)) == (AircraftLaneFlags.EndReached | AircraftLaneFlags.ParkingSpace))
		{
			return (pathOwner.m_State & PathFlags.Pending) == 0;
		}
		return false;
	}

	public static bool WaypointReached(CarCurrentLane currentLane)
	{
		return (currentLane.m_LaneFlags & (CarLaneFlags.EndReached | CarLaneFlags.Waypoint)) == (CarLaneFlags.EndReached | CarLaneFlags.Waypoint);
	}

	public static bool QueueReached(CarCurrentLane currentLane)
	{
		return (currentLane.m_LaneFlags & (CarLaneFlags.Queue | CarLaneFlags.QueueReached)) == (CarLaneFlags.Queue | CarLaneFlags.QueueReached);
	}

	public static bool RequireNewPath(PathOwner pathOwner)
	{
		if ((pathOwner.m_State & (PathFlags.Obsolete | PathFlags.DivertObsolete)) != 0)
		{
			return (pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Stuck)) == 0;
		}
		return false;
	}

	public static bool IsStuck(PathOwner pathOwner)
	{
		return (pathOwner.m_State & PathFlags.Stuck) != 0;
	}

	public static void SetTarget(ref PathOwner pathOwner, ref Target targetData, Entity newTarget)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		targetData.m_Target = newTarget;
		pathOwner.m_State &= ~PathFlags.Failed;
		pathOwner.m_State |= PathFlags.Obsolete;
	}

	public static void SetupPathfind(ref CarCurrentLane currentLane, ref PathOwner pathOwner, ParallelWriter<SetupQueueItem> queue, SetupQueueItem item)
	{
		if ((pathOwner.m_State & (PathFlags.Obsolete | PathFlags.Divert)) == (PathFlags.Obsolete | PathFlags.Divert))
		{
			pathOwner.m_State |= PathFlags.CachedObsolete;
		}
		pathOwner.m_State &= ~(PathFlags.Failed | PathFlags.Obsolete | PathFlags.DivertObsolete);
		pathOwner.m_State |= PathFlags.Pending;
		currentLane.m_LaneFlags &= ~CarLaneFlags.EndOfPath;
		currentLane.m_LaneFlags |= CarLaneFlags.FixedLane;
		queue.Enqueue(item);
	}

	public static void SetupPathfind(ref TrainCurrentLane currentLane, ref PathOwner pathOwner, ParallelWriter<SetupQueueItem> queue, SetupQueueItem item)
	{
		if ((pathOwner.m_State & (PathFlags.Obsolete | PathFlags.Divert)) == (PathFlags.Obsolete | PathFlags.Divert))
		{
			pathOwner.m_State |= PathFlags.CachedObsolete;
		}
		pathOwner.m_State &= ~(PathFlags.Failed | PathFlags.Obsolete | PathFlags.DivertObsolete);
		pathOwner.m_State |= PathFlags.Pending;
		currentLane.m_Front.m_LaneFlags &= ~TrainLaneFlags.EndOfPath;
		currentLane.m_Rear.m_LaneFlags &= ~TrainLaneFlags.EndOfPath;
		queue.Enqueue(item);
	}

	public static void SetupPathfind(ref WatercraftCurrentLane currentLane, ref PathOwner pathOwner, ParallelWriter<SetupQueueItem> queue, SetupQueueItem item)
	{
		if ((pathOwner.m_State & (PathFlags.Obsolete | PathFlags.Divert)) == (PathFlags.Obsolete | PathFlags.Divert))
		{
			pathOwner.m_State |= PathFlags.CachedObsolete;
		}
		pathOwner.m_State &= ~(PathFlags.Failed | PathFlags.Obsolete | PathFlags.DivertObsolete);
		pathOwner.m_State |= PathFlags.Pending;
		currentLane.m_LaneFlags &= ~WatercraftLaneFlags.EndOfPath;
		currentLane.m_LaneFlags |= WatercraftLaneFlags.FixedLane;
		queue.Enqueue(item);
	}

	public static void SetupPathfind(ref AircraftCurrentLane currentLane, ref PathOwner pathOwner, ParallelWriter<SetupQueueItem> queue, SetupQueueItem item)
	{
		if ((pathOwner.m_State & (PathFlags.Obsolete | PathFlags.Divert)) == (PathFlags.Obsolete | PathFlags.Divert))
		{
			pathOwner.m_State |= PathFlags.CachedObsolete;
		}
		pathOwner.m_State &= ~(PathFlags.Failed | PathFlags.Obsolete | PathFlags.DivertObsolete);
		pathOwner.m_State |= PathFlags.Pending;
		currentLane.m_LaneFlags &= ~AircraftLaneFlags.EndOfPath;
		queue.Enqueue(item);
	}

	public static bool ResetUpdatedPath(ref PathOwner pathOwner)
	{
		bool result = (pathOwner.m_State & PathFlags.Updated) != 0;
		pathOwner.m_State &= ~PathFlags.Updated;
		return result;
	}

	public static Transform CalculateParkingSpaceTarget(Game.Net.ParkingLane parkingLane, ParkingLaneData parkingLaneData, ObjectGeometryData prefabGeometryData, Curve curve, Transform ownerTransform, float curvePos)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Transform result = default(Transform);
		CalculateParkingSpaceTarget(parkingLane, parkingLaneData, prefabGeometryData, curve, ownerTransform, curvePos, out result.m_Position, out var forward, out var up);
		result.m_Rotation = quaternion.LookRotationSafe(forward, up);
		return result;
	}

	public static void CalculateParkingSpaceTarget(Game.Net.ParkingLane parkingLane, ParkingLaneData parkingLaneData, ObjectGeometryData prefabGeometryData, Curve curve, Transform ownerTransform, float curvePos, out float3 position, out float3 forward, out float3 up)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		position = MathUtils.Position(curve.m_Bezier, curvePos);
		float3 val = MathUtils.Tangent(curve.m_Bezier, curvePos);
		val = math.select(val, -val, (parkingLane.m_Flags & ParkingLaneFlags.ParkingInverted) != 0);
		float3 val2 = default(float3);
		((float3)(ref val2)).xz = MathUtils.Right(((float3)(ref val)).xz);
		if (!((quaternion)(ref ownerTransform.m_Rotation)).Equals(default(quaternion)))
		{
			val2.y -= math.dot(val2, math.rotate(ownerTransform.m_Rotation, math.up()));
		}
		float num = math.select(parkingLaneData.m_SlotAngle, 0f - parkingLaneData.m_SlotAngle, (parkingLane.m_Flags & ParkingLaneFlags.ParkingLeft) != 0);
		up = math.cross(val, val2);
		forward = math.rotate(quaternion.AxisAngle(math.normalizesafe(up, default(float3)), num), val);
		if (parkingLaneData.m_SlotAngle > 0.25f)
		{
			float num2 = math.max(0f, MathUtils.Size(((Bounds3)(ref prefabGeometryData.m_Bounds)).z) - parkingLaneData.m_SlotSize.y);
			position += math.normalizesafe(forward, default(float3)) * ((parkingLaneData.m_SlotSize.y + num2) * 0.5f - prefabGeometryData.m_Bounds.max.z);
		}
		else
		{
			float num3 = math.select(0f, -0.5f, (parkingLane.m_Flags & ParkingLaneFlags.ParkingLeft) != 0);
			num3 = math.select(num3, 0.5f, (parkingLane.m_Flags & ParkingLaneFlags.ParkingRight) != 0);
			position += math.normalizesafe(val2, default(float3)) * ((parkingLaneData.m_SlotSize.x - prefabGeometryData.m_Size.x) * num3);
		}
	}

	public static Transform CalculateTransform(Curve curve, float curvePos)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		return new Transform
		{
			m_Position = MathUtils.Position(curve.m_Bezier, curvePos),
			m_Rotation = quaternion.LookRotationSafe(MathUtils.Tangent(curve.m_Bezier, curvePos), math.up())
		};
	}

	public static int SetParkingCurvePos(Entity entity, ref Random random, CarCurrentLane currentLane, PathOwner pathOwner, DynamicBuffer<PathElement> path, ref ComponentLookup<ParkedCar> parkedCarData, ref ComponentLookup<Unspawned> unspawnedData, ref ComponentLookup<Curve> curveData, ref ComponentLookup<Game.Net.ParkingLane> parkingLaneData, ref ComponentLookup<Game.Net.ConnectionLane> connectionLaneData, ref ComponentLookup<PrefabRef> prefabRefData, ref ComponentLookup<ObjectGeometryData> prefabObjectGeometryData, ref ComponentLookup<ParkingLaneData> prefabParkingLaneData, ref BufferLookup<LaneObject> laneObjectData, ref BufferLookup<LaneOverlap> laneOverlapData, bool ignoreDriveways)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		for (int i = pathOwner.m_ElementIndex; i < path.Length; i++)
		{
			PathElement pathElement = path[i];
			if (!IsParkingLane(pathElement.m_Target, ref parkingLaneData, ref connectionLaneData))
			{
				continue;
			}
			float curvePos = -1f;
			if (parkingLaneData.HasComponent(pathElement.m_Target))
			{
				float offset;
				float y = GetParkingSize(entity, ref prefabRefData, ref prefabObjectGeometryData, out offset).y;
				if (!FindFreeParkingSpace(ref random, pathElement.m_Target, pathElement.m_TargetDelta.x, y, offset, ref curvePos, ref parkedCarData, ref curveData, ref unspawnedData, ref parkingLaneData, ref prefabRefData, ref prefabParkingLaneData, ref prefabObjectGeometryData, ref laneObjectData, ref laneOverlapData, ignoreDriveways, ignoreDisabled: false))
				{
					curvePos = ((Random)(ref random)).NextFloat(0.05f, 0.95f);
				}
			}
			else
			{
				curvePos = ((Random)(ref random)).NextFloat(0.05f, 0.95f);
			}
			SetParkingCurvePos(path, pathOwner, i, currentLane.m_Lane, curvePos, ref curveData);
			return i;
		}
		return path.Length;
	}

	public static void SetParkingCurvePos(DynamicBuffer<PathElement> path, PathOwner pathOwner, int index, Entity currentLane, float curvePos, ref ComponentLookup<Curve> curveData)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (index >= pathOwner.m_ElementIndex)
		{
			PathElement pathElement = path[index];
			pathElement.m_TargetDelta = float2.op_Implicit(curvePos);
			path[index] = pathElement;
			currentLane = pathElement.m_Target;
		}
		Curve curve = default(Curve);
		if (!curveData.TryGetComponent(currentLane, ref curve))
		{
			return;
		}
		float3 val = MathUtils.Position(curve.m_Bezier, curvePos);
		if (index > pathOwner.m_ElementIndex)
		{
			PathElement pathElement2 = path[index - 1];
			if (curveData.TryGetComponent(pathElement2.m_Target, ref curve))
			{
				MathUtils.Distance(curve.m_Bezier, val, ref curvePos);
				pathElement2.m_TargetDelta.y = curvePos;
				path[index - 1] = pathElement2;
			}
		}
		if (index < path.Length - 1)
		{
			PathElement pathElement3 = path[index + 1];
			if (curveData.TryGetComponent(pathElement3.m_Target, ref curve))
			{
				MathUtils.Distance(curve.m_Bezier, val, ref curvePos);
				pathElement3.m_TargetDelta.x = curvePos;
				path[index + 1] = pathElement3;
			}
		}
	}

	public static void ResetParkingLaneStatus(Entity entity, ref CarCurrentLane currentLane, ref PathOwner pathOwner, DynamicBuffer<PathElement> path, ref EntityStorageInfoLookup entityLookup, ref ComponentLookup<Curve> curveData, ref ComponentLookup<Game.Net.ParkingLane> parkingLaneData, ref ComponentLookup<Game.Net.CarLane> carLaneData, ref ComponentLookup<Game.Net.ConnectionLane> connectionLaneData, ref ComponentLookup<Game.Objects.SpawnLocation> spawnLocationData, ref ComponentLookup<PrefabRef> prefabRefData, ref ComponentLookup<SpawnLocationData> prefabSpawnLocationData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (IsParkingLane(currentLane.m_Lane, ref parkingLaneData, ref connectionLaneData))
		{
			currentLane.m_LaneFlags |= CarLaneFlags.ParkingSpace;
			bool flag = false;
			while (pathOwner.m_ElementIndex < path.Length)
			{
				PathElement pathElement = path[pathOwner.m_ElementIndex];
				if (IsParkingLane(pathElement.m_Target, ref parkingLaneData, ref connectionLaneData))
				{
					SetParkingCurvePos(path, pathOwner, pathOwner.m_ElementIndex++, currentLane.m_Lane, currentLane.m_CurvePosition.z, ref curveData);
					flag = true;
					continue;
				}
				if (!flag)
				{
					SetParkingCurvePos(path, pathOwner, pathOwner.m_ElementIndex - 1, currentLane.m_Lane, currentLane.m_CurvePosition.z, ref curveData);
				}
				if (IsCarLane(pathElement.m_Target, ref carLaneData, ref connectionLaneData, ref spawnLocationData, ref prefabRefData, ref prefabSpawnLocationData) || !((EntityStorageInfoLookup)(ref entityLookup)).Exists(pathElement.m_Target))
				{
					currentLane.m_LaneFlags &= ~CarLaneFlags.ParkingSpace;
				}
				break;
			}
		}
		else if (IsCarLane(currentLane.m_Lane, ref carLaneData, ref connectionLaneData, ref spawnLocationData, ref prefabRefData, ref prefabSpawnLocationData))
		{
			currentLane.m_LaneFlags &= ~CarLaneFlags.ParkingSpace;
		}
	}

	public static bool IsParkingLane(Entity lane, ref ComponentLookup<Game.Net.ParkingLane> parkingLaneData, ref ComponentLookup<Game.Net.ConnectionLane> connectionLaneData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (parkingLaneData.HasComponent(lane))
		{
			return true;
		}
		Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
		if (connectionLaneData.TryGetComponent(lane, ref connectionLane))
		{
			return (connectionLane.m_Flags & ConnectionLaneFlags.Parking) != 0;
		}
		return false;
	}

	public static bool IsCarLane(Entity lane, ref ComponentLookup<Game.Net.CarLane> carLaneData, ref ComponentLookup<Game.Net.ConnectionLane> connectionLaneData, ref ComponentLookup<Game.Objects.SpawnLocation> spawnLocationData, ref ComponentLookup<PrefabRef> prefabRefData, ref ComponentLookup<SpawnLocationData> prefabSpawnLocationData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (carLaneData.HasComponent(lane))
		{
			return true;
		}
		Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
		if (connectionLaneData.TryGetComponent(lane, ref connectionLane))
		{
			return (connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0;
		}
		SpawnLocationData spawnLocationData2 = default(SpawnLocationData);
		if (spawnLocationData.HasComponent(lane) && prefabSpawnLocationData.TryGetComponent(prefabRefData[lane].m_Prefab, ref spawnLocationData2))
		{
			if (spawnLocationData2.m_ConnectionType != RouteConnectionType.Road)
			{
				return spawnLocationData2.m_ConnectionType == RouteConnectionType.Parking;
			}
			return true;
		}
		return false;
	}

	public static void SetParkingCurvePos(DynamicBuffer<PathElement> path, PathOwner pathOwner, ref CarCurrentLane currentLaneData, DynamicBuffer<CarNavigationLane> navLanes, int navIndex, float curvePos, ref ComponentLookup<Curve> curveData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		Entity lane = currentLaneData.m_Lane;
		if (navIndex >= 0)
		{
			CarNavigationLane carNavigationLane = navLanes[navIndex];
			carNavigationLane.m_CurvePosition = float2.op_Implicit(curvePos);
			navLanes[navIndex] = carNavigationLane;
			lane = carNavigationLane.m_Lane;
		}
		if (!curveData.HasComponent(lane))
		{
			return;
		}
		float3 val = MathUtils.Position(curveData[lane].m_Bezier, curvePos);
		if (navIndex > 0)
		{
			CarNavigationLane carNavigationLane2 = navLanes[navIndex - 1];
			if (curveData.HasComponent(carNavigationLane2.m_Lane))
			{
				MathUtils.Distance(curveData[carNavigationLane2.m_Lane].m_Bezier, val, ref curvePos);
				carNavigationLane2.m_CurvePosition.y = curvePos;
				navLanes[navIndex - 1] = carNavigationLane2;
			}
		}
		else if (navIndex == 0 && curveData.HasComponent(currentLaneData.m_Lane))
		{
			MathUtils.Distance(curveData[currentLaneData.m_Lane].m_Bezier, val, ref curvePos);
			currentLaneData.m_CurvePosition.z = curvePos;
		}
		if (navIndex < navLanes.Length - 1)
		{
			CarNavigationLane carNavigationLane3 = navLanes[navIndex + 1];
			if (curveData.HasComponent(carNavigationLane3.m_Lane))
			{
				MathUtils.Distance(curveData[carNavigationLane3.m_Lane].m_Bezier, val, ref curvePos);
				carNavigationLane3.m_CurvePosition.x = curvePos;
				navLanes[navIndex + 1] = carNavigationLane3;
			}
		}
		else if (navIndex == navLanes.Length - 1 && path.Length > pathOwner.m_ElementIndex)
		{
			PathElement pathElement = path[pathOwner.m_ElementIndex];
			if (curveData.HasComponent(pathElement.m_Target))
			{
				MathUtils.Distance(curveData[pathElement.m_Target].m_Bezier, val, ref curvePos);
				pathElement.m_TargetDelta.x = curvePos;
				path[pathOwner.m_ElementIndex] = pathElement;
			}
		}
	}

	public static void CalculateTrainNavigationPivots(Transform transform, TrainData prefabTrainData, out float3 pivot1, out float3 pivot2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.forward(transform.m_Rotation);
		pivot1 = transform.m_Position + val * prefabTrainData.m_BogieOffsets.x;
		pivot2 = transform.m_Position - val * prefabTrainData.m_BogieOffsets.y;
	}

	public static void CalculateShipNavigationPivots(Transform transform, ObjectGeometryData prefabGeometryData, out float3 pivot1, out float3 pivot2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.forward(transform.m_Rotation) * math.max(1f, (prefabGeometryData.m_Size.z - prefabGeometryData.m_Size.x) * 0.5f);
		pivot1 = transform.m_Position + val;
		pivot2 = transform.m_Position - val;
	}

	public static bool CalculateTransformPosition(ref float3 position, Entity entity, ComponentLookup<Transform> transforms, ComponentLookup<Position> positions, ComponentLookup<PrefabRef> prefabRefs, ComponentLookup<BuildingData> prefabBuildingDatas)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (transforms.HasComponent(entity))
		{
			Transform transform = transforms[entity];
			PrefabRef prefabRef = prefabRefs[entity];
			if (prefabBuildingDatas.HasComponent(prefabRef.m_Prefab))
			{
				BuildingData buildingData = prefabBuildingDatas[prefabRef.m_Prefab];
				position = BuildingUtils.CalculateFrontPosition(transform, buildingData.m_LotSize.y);
				return true;
			}
			position = transform.m_Position;
			return true;
		}
		if (positions.HasComponent(entity))
		{
			position = positions[entity].m_Position;
			return true;
		}
		return false;
	}

	public static float GetNavigationSize(ObjectGeometryData prefabObjectGeometryData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return prefabObjectGeometryData.m_Bounds.max.x - prefabObjectGeometryData.m_Bounds.min.x + 2f;
	}

	public static float GetMaxDriveSpeed(CarData prefabCarData, Game.Net.CarLane carLaneData)
	{
		return GetMaxDriveSpeed(prefabCarData, carLaneData.m_SpeedLimit, carLaneData.m_Curviness);
	}

	public static float GetMaxDriveSpeed(CarData prefabCarData, float speedLimit, float curviness)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		float num = prefabCarData.m_Turning.x * prefabCarData.m_MaxSpeed / math.max(1E-06f, curviness * prefabCarData.m_MaxSpeed + prefabCarData.m_Turning.x - prefabCarData.m_Turning.y);
		num = math.max(1f, num);
		return math.min(speedLimit, num);
	}

	public static void ModifyDriveSpeed(ref float driveSpeed, LaneCondition condition)
	{
		float num = math.saturate((condition.m_Wear - 2.5f) * (2f / 15f));
		driveSpeed *= 1f - num * num * 0.5f;
	}

	public static float GetMaxBrakingSpeed(CarData prefabCarData, float distance, float timeStep)
	{
		float num = timeStep * prefabCarData.m_Braking;
		return math.max(0f, math.sqrt(math.max(0f, num * num + 2f * prefabCarData.m_Braking * distance)) - num);
	}

	public static float GetMaxBrakingSpeed(CarData prefabCarData, float distance, float maxResultSpeed, float timeStep)
	{
		float num = timeStep * prefabCarData.m_Braking;
		return math.max(0f, math.sqrt(math.max(0f, num * num + 2f * prefabCarData.m_Braking * distance + maxResultSpeed * maxResultSpeed)) - num);
	}

	public static float GetBrakingDistance(CarData prefabCarData, float speed, float timeStep)
	{
		return 0.5f * speed * speed / prefabCarData.m_Braking + speed * timeStep;
	}

	public static Bounds1 CalculateSpeedRange(CarData prefabCarData, float currentSpeed, float timeStep)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		float num = MathUtils.InverseSmoothStep(prefabCarData.m_MaxSpeed, 0f, currentSpeed) * prefabCarData.m_Acceleration;
		float2 val = currentSpeed + new float2(0f - prefabCarData.m_Braking, num) * timeStep;
		val.x = math.max(0f, val.x);
		val.y = math.min(val.y, math.max(val.x, prefabCarData.m_MaxSpeed));
		return new Bounds1(val.x, val.y);
	}

	public static int GetPriority(Car carData)
	{
		return math.select(100, 108, (carData.m_Flags & CarFlags.Emergency) != 0);
	}

	public static Game.Net.CarLaneFlags GetForbiddenLaneFlags(Car carData)
	{
		if ((carData.m_Flags & CarFlags.UsePublicTransportLanes) == 0)
		{
			return Game.Net.CarLaneFlags.Unsafe | Game.Net.CarLaneFlags.PublicOnly | Game.Net.CarLaneFlags.Forbidden;
		}
		return Game.Net.CarLaneFlags.Unsafe | Game.Net.CarLaneFlags.Forbidden;
	}

	public static Game.Net.CarLaneFlags GetPreferredLaneFlags(Car carData)
	{
		if ((carData.m_Flags & CarFlags.PreferPublicTransportLanes) == 0)
		{
			return ~(Game.Net.CarLaneFlags.Unsafe | Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.Invert | Game.Net.CarLaneFlags.SideConnection | Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.Twoway | Game.Net.CarLaneFlags.IsSecured | Game.Net.CarLaneFlags.Runway | Game.Net.CarLaneFlags.Yield | Game.Net.CarLaneFlags.Stop | Game.Net.CarLaneFlags.ForbidCombustionEngines | Game.Net.CarLaneFlags.ForbidTransitTraffic | Game.Net.CarLaneFlags.ForbidHeavyTraffic | Game.Net.CarLaneFlags.PublicOnly | Game.Net.CarLaneFlags.Highway | Game.Net.CarLaneFlags.UTurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight | Game.Net.CarLaneFlags.Forward | Game.Net.CarLaneFlags.Approach | Game.Net.CarLaneFlags.Roundabout | Game.Net.CarLaneFlags.RightLimit | Game.Net.CarLaneFlags.LeftLimit | Game.Net.CarLaneFlags.ForbidPassing | Game.Net.CarLaneFlags.RightOfWay | Game.Net.CarLaneFlags.TrafficLights | Game.Net.CarLaneFlags.ParkingLeft | Game.Net.CarLaneFlags.ParkingRight | Game.Net.CarLaneFlags.Forbidden | Game.Net.CarLaneFlags.AllowEnter);
		}
		return Game.Net.CarLaneFlags.PublicOnly;
	}

	public static float GetSpeedLimitFactor(Car carData)
	{
		return math.select(1f, 2f, (carData.m_Flags & CarFlags.Emergency) != 0);
	}

	public static void GetDrivingStyle(uint simulationFrame, PseudoRandomSeed randomSeed, out float safetyTime)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Random random = randomSeed.GetRandom(PseudoRandomSeed.kDrivingStyle);
		float num = (float)(simulationFrame & 0xFFF) * 0.0015339808f + ((Random)(ref random)).NextFloat((float)Math.PI * 2f);
		safetyTime = 0.3f + 0.2f * math.sin(num);
	}

	public static float GetMaxDriveSpeed(TrainData prefabTrainData, Game.Net.TrackLane trackLaneData)
	{
		return GetMaxDriveSpeed(prefabTrainData, trackLaneData.m_SpeedLimit, trackLaneData.m_Curviness);
	}

	public static float GetMaxDriveSpeed(TrainData prefabTrainData, float speedLimit, float curviness)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		float num = prefabTrainData.m_Turning.x * prefabTrainData.m_MaxSpeed / math.max(1E-06f, curviness * prefabTrainData.m_MaxSpeed + prefabTrainData.m_Turning.x - prefabTrainData.m_Turning.y);
		num = math.max(1f, num);
		return math.min(speedLimit, num);
	}

	public static float GetMaxBrakingSpeed(TrainData prefabTrainData, float distance, float timeStep)
	{
		float num = timeStep * prefabTrainData.m_Braking;
		return math.max(0f, math.sqrt(math.max(0f, num * num + 2f * prefabTrainData.m_Braking * distance)) - num);
	}

	public static float GetMaxBrakingSpeed(TrainData prefabTrainData, float distance, float maxResultSpeed, float timeStep)
	{
		float num = timeStep * prefabTrainData.m_Braking;
		return math.max(0f, math.sqrt(math.max(0f, num * num + 2f * prefabTrainData.m_Braking * distance + maxResultSpeed * maxResultSpeed)) - num);
	}

	public static int GetAllBuyingResourcesTrucks(Entity destination, Resource resource, ref ComponentLookup<DeliveryTruck> trucks, ref BufferLookup<GuestVehicle> guestVehiclesBufs, ref BufferLookup<LayoutElement> layoutsBufs)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		DynamicBuffer<GuestVehicle> val = default(DynamicBuffer<GuestVehicle>);
		if (guestVehiclesBufs.HasBuffer(destination))
		{
			val = guestVehiclesBufs[destination];
		}
		if (val.IsCreated)
		{
			for (int i = 0; i < val.Length; i++)
			{
				Entity vehicle = val[i].m_Vehicle;
				num += GetBuyingTrucksLoad(vehicle, resource, ref trucks, ref layoutsBufs);
			}
		}
		return num;
	}

	public static int GetBuyingTrucksLoad(Entity vehicle, Resource resource, ref ComponentLookup<DeliveryTruck> trucks, ref BufferLookup<LayoutElement> layouts)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		if (trucks.HasComponent(vehicle))
		{
			DeliveryTruck deliveryTruck = trucks[vehicle];
			DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
			if (layouts.HasBuffer(vehicle))
			{
				val = layouts[vehicle];
			}
			if (val.IsCreated && layouts[vehicle].Length != 0)
			{
				for (int i = 0; i < val.Length; i++)
				{
					Entity vehicle2 = val[i].m_Vehicle;
					if (trucks.HasComponent(vehicle2))
					{
						DeliveryTruck deliveryTruck2 = trucks[vehicle2];
						if (deliveryTruck2.m_Resource == resource && (deliveryTruck.m_State & DeliveryTruckFlags.Buying) != 0)
						{
							num += deliveryTruck2.m_Amount;
						}
					}
				}
			}
			else if (deliveryTruck.m_Resource == resource && (deliveryTruck.m_State & DeliveryTruckFlags.Buying) != 0)
			{
				num += deliveryTruck.m_Amount;
			}
		}
		return num;
	}

	public static float GetBrakingDistance(TrainData prefabTrainData, float speed, float timeStep)
	{
		return 0.5f * speed * speed / prefabTrainData.m_Braking + speed * timeStep;
	}

	public static float GetSignalDistance(TrainData prefabTrainData, float speed)
	{
		return math.select(0f, speed * 4f, (prefabTrainData.m_TrackType & (TrackTypes.Train | TrackTypes.Subway)) != 0);
	}

	public static Bounds1 CalculateSpeedRange(TrainData prefabTrainData, float currentSpeed, float timeStep)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		float num = MathUtils.InverseSmoothStep(prefabTrainData.m_MaxSpeed, 0f, currentSpeed) * prefabTrainData.m_Acceleration;
		float2 val = currentSpeed + new float2(0f - prefabTrainData.m_Braking, num) * timeStep;
		val.x = math.max(0f, val.x);
		val.y = math.min(val.y, math.max(val.x, prefabTrainData.m_MaxSpeed));
		return new Bounds1(val.x, val.y);
	}

	public static int GetPriority(TrainData trainData)
	{
		return math.select(104, 110, (trainData.m_TrackType & (TrackTypes.Train | TrackTypes.Subway)) != 0);
	}

	public static float GetMaxDriveSpeed(WatercraftData prefabWatercraftData, Game.Net.CarLane carLaneData)
	{
		return GetMaxDriveSpeed(prefabWatercraftData, carLaneData.m_SpeedLimit, carLaneData.m_Curviness);
	}

	public static float GetMaxDriveSpeed(WatercraftData prefabWatercraftData, float speedLimit, float curviness)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		float num = prefabWatercraftData.m_Turning.x * prefabWatercraftData.m_MaxSpeed / math.max(1E-06f, curviness * prefabWatercraftData.m_MaxSpeed + prefabWatercraftData.m_Turning.x - prefabWatercraftData.m_Turning.y);
		num = math.max(1f, num);
		return math.min(speedLimit, num);
	}

	public static float GetMaxBrakingSpeed(WatercraftData prefabWatercraftData, float distance, float timeStep)
	{
		float num = timeStep * prefabWatercraftData.m_Braking;
		return math.max(0f, math.sqrt(math.max(0f, num * num + 2f * prefabWatercraftData.m_Braking * distance)) - num);
	}

	public static float GetMaxBrakingSpeed(WatercraftData prefabWatercraftData, float distance, float maxResultSpeed, float timeStep)
	{
		float num = timeStep * prefabWatercraftData.m_Braking;
		return math.max(0f, math.sqrt(math.max(0f, num * num + 2f * prefabWatercraftData.m_Braking * distance + maxResultSpeed * maxResultSpeed)) - num);
	}

	public static float GetBrakingDistance(WatercraftData prefabWatercraftData, float speed, float timeStep)
	{
		return 0.5f * speed * speed / prefabWatercraftData.m_Braking + speed * timeStep;
	}

	public static Bounds1 CalculateSpeedRange(WatercraftData prefabWatercraftData, float currentSpeed, float timeStep)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		float num = MathUtils.InverseSmoothStep(prefabWatercraftData.m_MaxSpeed, 0f, currentSpeed) * prefabWatercraftData.m_Acceleration;
		float2 val = currentSpeed + new float2(0f - prefabWatercraftData.m_Braking, num) * timeStep;
		val.y = math.min(val.y, math.max(val.x, prefabWatercraftData.m_MaxSpeed));
		return new Bounds1(val.x, val.y);
	}

	public static int GetPriority(WatercraftData prefabWatercraftData)
	{
		return prefabWatercraftData.m_SizeClass switch
		{
			SizeClass.Small => 100, 
			SizeClass.Medium => 102, 
			SizeClass.Large => 104, 
			_ => 100, 
		};
	}

	public static float GetMaxDriveSpeed(AircraftData prefabAircraftData, Game.Net.CarLane carLaneData)
	{
		return GetMaxDriveSpeed(prefabAircraftData, carLaneData.m_SpeedLimit, carLaneData.m_Curviness);
	}

	public static float GetMaxDriveSpeed(AircraftData prefabAircraftData, float speedLimit, float curviness)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		float num = prefabAircraftData.m_GroundTurning.x * prefabAircraftData.m_GroundMaxSpeed / math.max(1E-06f, curviness * prefabAircraftData.m_GroundMaxSpeed + prefabAircraftData.m_GroundTurning.x - prefabAircraftData.m_GroundTurning.y);
		num = math.max(1f, num);
		return math.min(speedLimit, num);
	}

	public static float GetMaxBrakingSpeed(AircraftData prefabAircraftData, float distance, float timeStep)
	{
		float num = timeStep * prefabAircraftData.m_GroundBraking;
		return math.max(0f, math.sqrt(math.max(0f, num * num + 2f * prefabAircraftData.m_GroundBraking * distance)) - num);
	}

	public static float GetMaxBrakingSpeed(HelicopterData prefabHelicopterData, float distance, float timeStep)
	{
		float num = timeStep * prefabHelicopterData.m_FlyingAcceleration;
		return math.max(0f, math.sqrt(math.max(0f, num * num + 2f * prefabHelicopterData.m_FlyingAcceleration * distance)) - num);
	}

	public static float GetMaxBrakingSpeed(AirplaneData prefabAirplaneData, float distance, float maxResultSpeed, float timeStep)
	{
		float num = timeStep * prefabAirplaneData.m_FlyingBraking;
		return math.max(0f, math.sqrt(math.max(0f, num * num + 2f * prefabAirplaneData.m_FlyingBraking * distance + maxResultSpeed * maxResultSpeed)) - num);
	}

	public static float GetMaxBrakingSpeed(AircraftData prefabAircraftData, float distance, float maxResultSpeed, float timeStep)
	{
		float num = timeStep * prefabAircraftData.m_GroundBraking;
		return math.max(0f, math.sqrt(math.max(0f, num * num + 2f * prefabAircraftData.m_GroundBraking * distance + maxResultSpeed * maxResultSpeed)) - num);
	}

	public static float GetBrakingDistance(AircraftData prefabAircraftData, float speed, float timeStep)
	{
		return 0.5f * speed * speed / prefabAircraftData.m_GroundBraking + speed * timeStep;
	}

	public static float GetBrakingDistance(HelicopterData prefabHelicopterData, float speed, float timeStep)
	{
		return 0.5f * speed * speed / prefabHelicopterData.m_FlyingAcceleration + speed * timeStep;
	}

	public static float GetBrakingDistance(AirplaneData prefabAirplaneData, float speed, float timeStep)
	{
		return 0.5f * speed * speed / prefabAirplaneData.m_FlyingBraking + speed * timeStep;
	}

	public static Bounds1 CalculateSpeedRange(AircraftData prefabAircraftData, float currentSpeed, float timeStep)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		float num = MathUtils.InverseSmoothStep(prefabAircraftData.m_GroundMaxSpeed, 0f, currentSpeed) * prefabAircraftData.m_GroundAcceleration;
		float2 val = currentSpeed + new float2(0f - prefabAircraftData.m_GroundBraking, num) * timeStep;
		val.x = math.max(0f, val.x);
		val.y = math.min(val.y, math.max(val.x, prefabAircraftData.m_GroundMaxSpeed));
		return new Bounds1(val.x, val.y);
	}

	public static Bounds1 CalculateSpeedRange(HelicopterData prefabHelicopterData, float currentSpeed, float timeStep)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		float num = MathUtils.InverseSmoothStep(prefabHelicopterData.m_FlyingMaxSpeed, 0f, currentSpeed) * prefabHelicopterData.m_FlyingAcceleration;
		float2 val = currentSpeed + new float2(0f - prefabHelicopterData.m_FlyingAcceleration, num) * timeStep;
		val.x = math.max(0f, val.x);
		val.y = math.min(val.y, math.max(val.x, prefabHelicopterData.m_FlyingMaxSpeed));
		return new Bounds1(val.x, val.y);
	}

	public static Bounds1 CalculateSpeedRange(AirplaneData prefabAirplaneData, float currentSpeed, float timeStep)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		float num = MathUtils.InverseSmoothStep(prefabAirplaneData.m_FlyingSpeed.y, 0f, currentSpeed) * prefabAirplaneData.m_FlyingAcceleration;
		float2 val = currentSpeed + new float2(0f - prefabAirplaneData.m_FlyingBraking, num) * timeStep;
		((float2)(ref val))._002Ector(math.max(val.x, math.min(val.y, prefabAirplaneData.m_FlyingSpeed.x)), math.min(val.y, math.max(val.x, prefabAirplaneData.m_FlyingSpeed.y)));
		return new Bounds1(val.x, val.y);
	}

	public static int GetPriority(AircraftData prefabAircraftData)
	{
		return 104;
	}

	public static void DeleteVehicle(EntityCommandBuffer commandBuffer, Entity vehicle, DynamicBuffer<LayoutElement> layout)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (layout.IsCreated && layout.Length != 0)
		{
			for (int i = 0; i < layout.Length; i++)
			{
				((EntityCommandBuffer)(ref commandBuffer)).AddComponent<Deleted>(layout[i].m_Vehicle, default(Deleted));
			}
		}
		else
		{
			((EntityCommandBuffer)(ref commandBuffer)).AddComponent<Deleted>(vehicle, default(Deleted));
		}
	}

	public static void DeleteVehicle(ParallelWriter commandBuffer, int jobIndex, Entity vehicle, DynamicBuffer<LayoutElement> layout)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (layout.IsCreated && layout.Length != 0)
		{
			for (int i = 0; i < layout.Length; i++)
			{
				((ParallelWriter)(ref commandBuffer)).AddComponent<Deleted>(jobIndex, layout[i].m_Vehicle, default(Deleted));
			}
		}
		else
		{
			((ParallelWriter)(ref commandBuffer)).AddComponent<Deleted>(jobIndex, vehicle, default(Deleted));
		}
	}

	public static float2 GetParkingSize(ParkingLaneData parkingLaneData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		return math.select(new float2(parkingLaneData.m_SlotSize.x, parkingLaneData.m_MaxCarLength), new float2(parkingLaneData.m_SlotSize.x * 1.25f, 1000000f), new bool2(parkingLaneData.m_SlotAngle < 0.01f, parkingLaneData.m_MaxCarLength == 0f));
	}

	public static Entity GetParkingSource(Entity entity, CarCurrentLane currentLane, ref ComponentLookup<Game.Net.ParkingLane> parkingLaneData, ref ComponentLookup<Game.Net.ConnectionLane> connectionLaneData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (parkingLaneData.HasComponent(currentLane.m_Lane))
		{
			return currentLane.m_Lane;
		}
		Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
		if (connectionLaneData.TryGetComponent(currentLane.m_Lane, ref connectionLane) && (connectionLane.m_Flags & ConnectionLaneFlags.Parking) != 0)
		{
			return currentLane.m_Lane;
		}
		return entity;
	}

	public static float2 GetParkingSize(Entity car, ref ComponentLookup<PrefabRef> prefabRefData, ref ComponentLookup<ObjectGeometryData> objectGeometryData)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		float offset;
		return GetParkingSize(car, ref prefabRefData, ref objectGeometryData, out offset);
	}

	public static float2 GetParkingSize(Entity car, ref ComponentLookup<PrefabRef> prefabRefData, ref ComponentLookup<ObjectGeometryData> objectGeometryData, out float offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef prefabRef = default(PrefabRef);
		ObjectGeometryData objectGeometry = default(ObjectGeometryData);
		if (prefabRefData.TryGetComponent(car, ref prefabRef) && objectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometry))
		{
			return GetParkingSize(objectGeometry, out offset);
		}
		offset = 0f;
		return float2.op_Implicit(0f);
	}

	public static float2 GetParkingSize(ObjectGeometryData objectGeometry, out float offset)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		offset = 0f - MathUtils.Center(((Bounds3)(ref objectGeometry.m_Bounds)).z);
		return math.max(new float2(0.01f, 1.01f), MathUtils.Size(((Bounds3)(ref objectGeometry.m_Bounds)).xz));
	}

	public static float2 GetParkingOffsets(Entity car, ref ComponentLookup<PrefabRef> prefabRefData, ref ComponentLookup<ObjectGeometryData> objectGeometryData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef prefabRef = default(PrefabRef);
		ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
		if (prefabRefData.TryGetComponent(car, ref prefabRef) && objectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData2))
		{
			return math.max(float2.op_Implicit(0.1f), new float2(0f - objectGeometryData2.m_Bounds.min.z, objectGeometryData2.m_Bounds.max.z));
		}
		return float2.op_Implicit(0f);
	}

	public static RuleFlags GetIgnoredPathfindRules(CarData carData)
	{
		RuleFlags ruleFlags = (RuleFlags)0;
		if ((int)carData.m_SizeClass < 2)
		{
			ruleFlags |= RuleFlags.ForbidHeavyTraffic;
		}
		if (carData.m_EnergyType == EnergyTypes.Electricity)
		{
			ruleFlags |= RuleFlags.ForbidCombustionEngines;
		}
		if (carData.m_MaxSpeed >= 22.222223f)
		{
			ruleFlags |= RuleFlags.ForbidSlowTraffic;
		}
		return ruleFlags;
	}

	public static RuleFlags GetIgnoredPathfindRulesTaxiDefaults()
	{
		return RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic;
	}

	public static bool IsReversedPath(DynamicBuffer<PathElement> path, PathOwner pathOwner, Entity vehicle, DynamicBuffer<LayoutElement> layout, ComponentLookup<Curve> curveData, ComponentLookup<TrainCurrentLane> currentLaneData, ComponentLookup<Train> trainData, ComponentLookup<Transform> transformData)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		if (path.Length <= pathOwner.m_ElementIndex)
		{
			return false;
		}
		PathElement pathElement = path[pathOwner.m_ElementIndex];
		if (!curveData.HasComponent(pathElement.m_Target))
		{
			return false;
		}
		float3 val = MathUtils.Position(curveData[pathElement.m_Target].m_Bezier, pathElement.m_TargetDelta.x);
		Entity val2 = vehicle;
		Entity val3 = vehicle;
		if (layout.Length != 0)
		{
			val2 = layout[0].m_Vehicle;
			val3 = layout[layout.Length - 1].m_Vehicle;
		}
		TrainCurrentLane trainCurrentLane = currentLaneData[val2];
		TrainCurrentLane trainCurrentLane2 = currentLaneData[val3];
		float3 val4;
		float3 val5;
		if ((trainCurrentLane.m_Front.m_Lane != trainCurrentLane2.m_Rear.m_Lane || trainCurrentLane.m_Front.m_CurvePosition.w != trainCurrentLane2.m_Rear.m_CurvePosition.y) && curveData.HasComponent(trainCurrentLane.m_Front.m_Lane) && curveData.HasComponent(trainCurrentLane2.m_Rear.m_Lane))
		{
			Curve curve = curveData[trainCurrentLane.m_Front.m_Lane];
			Curve curve2 = curveData[trainCurrentLane2.m_Rear.m_Lane];
			val4 = MathUtils.Position(curve.m_Bezier, trainCurrentLane.m_Front.m_CurvePosition.w);
			val5 = MathUtils.Position(curve2.m_Bezier, trainCurrentLane2.m_Rear.m_CurvePosition.y);
		}
		else
		{
			Train train = trainData[val2];
			Train train2 = trainData[val3];
			Transform transform = transformData[val2];
			Transform transform2 = transformData[val3];
			float3 val6 = math.forward(transform.m_Rotation);
			float3 val7 = math.forward(transform2.m_Rotation);
			val6 = math.select(val6, -val6, (train.m_Flags & TrainFlags.Reversed) != 0);
			val7 = math.select(val7, -val7, (train2.m_Flags & TrainFlags.Reversed) != 0);
			val4 = transform.m_Position + val6;
			val5 = transform2.m_Position - val7;
		}
		return math.distancesq(val, val4) > math.distancesq(val, val5);
	}

	public static void ReverseTrain(Entity vehicle, DynamicBuffer<LayoutElement> layout, ref ComponentLookup<Train> trainData, ref ComponentLookup<TrainCurrentLane> currentLaneData, ref ComponentLookup<TrainNavigation> navigationData)
	{
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		if (layout.Length != 0)
		{
			TrainCurrentLane trainCurrentLane = currentLaneData[layout[0].m_Vehicle];
			TrainCurrentLane trainCurrentLane2 = currentLaneData[layout[layout.Length - 1].m_Vehicle];
			TrainBogieCache rearCache = new TrainBogieCache(trainCurrentLane.m_Front);
			TrainFlags trainFlags = trainData[layout[0].m_Vehicle].m_Flags & TrainFlags.IgnoreParkedVehicle;
			for (int i = 0; i < layout.Length; i++)
			{
				ReverseCarriage(layout[i].m_Vehicle, trainCurrentLane2.m_Rear.m_Lane, trainCurrentLane2.m_Rear.m_CurvePosition.y, ref trainData, ref currentLaneData, ref navigationData, ref rearCache);
			}
			CollectionUtils.Reverse<LayoutElement>(layout.AsNativeArray());
			Train train = trainData[layout[0].m_Vehicle];
			train.m_Flags |= trainFlags;
			trainData[layout[0].m_Vehicle] = train;
		}
		else
		{
			TrainCurrentLane trainCurrentLane3 = currentLaneData[vehicle];
			TrainBogieCache rearCache2 = new TrainBogieCache(trainCurrentLane3.m_Front);
			ReverseCarriage(vehicle, trainCurrentLane3.m_Rear.m_Lane, trainCurrentLane3.m_Rear.m_CurvePosition.y, ref trainData, ref currentLaneData, ref navigationData, ref rearCache2);
		}
	}

	public static void ReverseCarriage(Entity vehicle, Entity lastLane, float lastCurvePos, ref ComponentLookup<Train> trainData, ref ComponentLookup<TrainCurrentLane> currentLaneData, ref ComponentLookup<TrainNavigation> navigationData, ref TrainBogieCache rearCache)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		Train train = trainData[vehicle];
		TrainCurrentLane trainCurrentLane = currentLaneData[vehicle];
		TrainNavigation trainNavigation = navigationData[vehicle];
		train.m_Flags &= ~TrainFlags.IgnoreParkedVehicle;
		train.m_Flags ^= TrainFlags.Reversed;
		CommonUtils.Swap(ref trainCurrentLane.m_Front, ref trainCurrentLane.m_Rear);
		CommonUtils.Swap(ref trainCurrentLane.m_RearCache, ref rearCache);
		trainCurrentLane.m_Front.m_CurvePosition = ((float4)(ref trainCurrentLane.m_Front.m_CurvePosition)).wyyx;
		trainCurrentLane.m_FrontCache.m_CurvePosition = ((float2)(ref trainCurrentLane.m_FrontCache.m_CurvePosition)).yx;
		trainCurrentLane.m_Rear.m_CurvePosition = ((float4)(ref trainCurrentLane.m_Rear.m_CurvePosition)).wyyx;
		trainCurrentLane.m_RearCache.m_CurvePosition = ((float2)(ref trainCurrentLane.m_RearCache.m_CurvePosition)).yx;
		if (trainCurrentLane.m_Front.m_Lane == lastLane)
		{
			trainCurrentLane.m_Front.m_CurvePosition.w = lastCurvePos;
		}
		if (trainCurrentLane.m_FrontCache.m_Lane == lastLane)
		{
			trainCurrentLane.m_FrontCache.m_CurvePosition.y = lastCurvePos;
		}
		if (trainCurrentLane.m_Rear.m_Lane == lastLane)
		{
			trainCurrentLane.m_Rear.m_CurvePosition.w = lastCurvePos;
		}
		if (trainCurrentLane.m_RearCache.m_Lane == lastLane)
		{
			trainCurrentLane.m_RearCache.m_CurvePosition.y = lastCurvePos;
		}
		CommonUtils.Swap(ref trainNavigation.m_Front, ref trainNavigation.m_Rear);
		trainNavigation.m_Front.m_Direction = -trainNavigation.m_Front.m_Direction;
		trainNavigation.m_Rear.m_Direction = -trainNavigation.m_Rear.m_Direction;
		trainCurrentLane.m_Front.m_LaneFlags &= ~TrainLaneFlags.Return;
		trainCurrentLane.m_FrontCache.m_LaneFlags &= ~TrainLaneFlags.Return;
		trainCurrentLane.m_Rear.m_LaneFlags &= ~TrainLaneFlags.Return;
		trainCurrentLane.m_RearCache.m_LaneFlags &= ~TrainLaneFlags.Return;
		trainData[vehicle] = train;
		currentLaneData[vehicle] = trainCurrentLane;
		navigationData[vehicle] = trainNavigation;
	}

	public static float CalculateLength(Entity vehicle, DynamicBuffer<LayoutElement> layout, ref ComponentLookup<PrefabRef> prefabRefData, ref ComponentLookup<TrainData> prefabTrainData)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (layout.Length != 0)
		{
			float num = 0f;
			for (int i = 0; i < layout.Length; i++)
			{
				num += math.csum(prefabTrainData[prefabRefData[layout[i].m_Vehicle].m_Prefab].m_AttachOffsets);
			}
			return num;
		}
		return math.csum(prefabTrainData[prefabRefData[vehicle].m_Prefab].m_AttachOffsets);
	}

	public static void UpdateCarriageLocations(DynamicBuffer<LayoutElement> layout, NativeList<PathElement> laneBuffer, ref ComponentLookup<Train> trainData, ref ComponentLookup<ParkedTrain> parkedTrainData, ref ComponentLookup<TrainCurrentLane> currentLaneData, ref ComponentLookup<TrainNavigation> navigationData, ref ComponentLookup<Transform> transformData, ref ComponentLookup<Curve> curveData, ref ComponentLookup<Game.Net.ConnectionLane> connectionLaneData, ref ComponentLookup<PrefabRef> prefabRefData, ref ComponentLookup<TrainData> prefabTrainData)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		if (laneBuffer.Length == 0)
		{
			return;
		}
		int num = 0;
		PathElement pathElement = laneBuffer[num++];
		float y = pathElement.m_TargetDelta.y;
		float3 val = MathUtils.Position(curveData[pathElement.m_Target].m_Bezier, pathElement.m_TargetDelta.y);
		float num2 = 0f;
		Transform transform = default(Transform);
		ParkedTrain parkedTrain = default(ParkedTrain);
		for (int i = 0; i < layout.Length; i++)
		{
			Entity vehicle = layout[i].m_Vehicle;
			Train train = trainData[vehicle];
			TrainData trainData2 = prefabTrainData[prefabRefData[vehicle].m_Prefab];
			bool flag = (train.m_Flags & TrainFlags.Reversed) != 0;
			if (flag)
			{
				trainData2.m_BogieOffsets = ((float2)(ref trainData2.m_BogieOffsets)).yx;
				trainData2.m_AttachOffsets = ((float2)(ref trainData2.m_AttachOffsets)).yx;
			}
			TrainCurrentLane trainCurrentLane = default(TrainCurrentLane);
			TrainNavigation trainNavigation = default(TrainNavigation);
			num2 += trainData2.m_AttachOffsets.x - trainData2.m_BogieOffsets.x;
			while (true)
			{
				Curve curve = curveData[pathElement.m_Target];
				if (MoveBufferPosition(val, ref trainNavigation.m_Front, num2, curve.m_Bezier, ref pathElement.m_TargetDelta) || num >= laneBuffer.Length)
				{
					break;
				}
				pathElement = laneBuffer[num++];
				y = pathElement.m_TargetDelta.y;
			}
			TrainLaneFlags trainLaneFlags = (TrainLaneFlags)0u;
			if (connectionLaneData.HasComponent(pathElement.m_Target))
			{
				trainLaneFlags |= TrainLaneFlags.Connection;
			}
			trainCurrentLane.m_Front = new TrainBogieLane(pathElement.m_Target, new float4(((float2)(ref pathElement.m_TargetDelta)).xyy, y), trainLaneFlags);
			trainCurrentLane.m_FrontCache = new TrainBogieCache(trainCurrentLane.m_Front);
			if (i != 0)
			{
				ClampPosition(ref trainNavigation.m_Front.m_Position, val, num2);
			}
			val = trainNavigation.m_Front.m_Position;
			num2 = math.csum(trainData2.m_BogieOffsets);
			while (true)
			{
				Curve curve = curveData[pathElement.m_Target];
				if (MoveBufferPosition(val, ref trainNavigation.m_Rear, num2, curve.m_Bezier, ref pathElement.m_TargetDelta) || num >= laneBuffer.Length)
				{
					break;
				}
				pathElement = laneBuffer[num++];
				y = pathElement.m_TargetDelta.y;
			}
			TrainLaneFlags trainLaneFlags2 = (TrainLaneFlags)0u;
			if (connectionLaneData.HasComponent(pathElement.m_Target))
			{
				trainLaneFlags2 |= TrainLaneFlags.Connection;
			}
			trainCurrentLane.m_Rear = new TrainBogieLane(pathElement.m_Target, new float4(((float2)(ref pathElement.m_TargetDelta)).xyy, y), trainLaneFlags2);
			trainCurrentLane.m_RearCache = new TrainBogieCache(trainCurrentLane.m_Rear);
			ClampPosition(ref trainNavigation.m_Rear.m_Position, val, num2);
			val = trainNavigation.m_Rear.m_Position;
			num2 = trainData2.m_AttachOffsets.y - trainData2.m_BogieOffsets.y;
			float3 val2 = trainNavigation.m_Rear.m_Position - trainNavigation.m_Front.m_Position;
			MathUtils.TryNormalize(ref val2, trainData2.m_BogieOffsets.x);
			transform.m_Position = trainNavigation.m_Front.m_Position + val2;
			float3 val3 = math.select(-val2, val2, flag);
			if (MathUtils.TryNormalize(ref val3))
			{
				transform.m_Rotation = quaternion.LookRotationSafe(val3, math.up());
			}
			else
			{
				transform.m_Rotation = quaternion.identity;
			}
			transformData[vehicle] = transform;
			if (parkedTrainData.TryGetComponent(vehicle, ref parkedTrain))
			{
				parkedTrain.m_FrontLane = trainCurrentLane.m_Front.m_Lane;
				parkedTrain.m_RearLane = trainCurrentLane.m_Rear.m_Lane;
				parkedTrain.m_CurvePosition = new float2(trainCurrentLane.m_Front.m_CurvePosition.y, trainCurrentLane.m_Rear.m_CurvePosition.y);
				parkedTrainData[vehicle] = parkedTrain;
			}
			else if (currentLaneData.HasComponent(vehicle))
			{
				currentLaneData[vehicle] = trainCurrentLane;
				navigationData[vehicle] = trainNavigation;
			}
		}
	}

	private static void ClampPosition(ref float3 position, float3 original, float maxDistance)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		position = original + MathUtils.ClampLength(position - original, maxDistance);
	}

	private static bool MoveBufferPosition(float3 comparePosition, ref TrainBogiePosition targetPosition, float minDistance, Bezier4x3 curve, ref float2 curveDelta)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		float3 val = MathUtils.Position(curve, curveDelta.x);
		if (math.distance(comparePosition, val) < minDistance)
		{
			curveDelta.y = curveDelta.x;
			targetPosition.m_Position = val;
			targetPosition.m_Direction = MathUtils.Tangent(curve, curveDelta.x);
			ref float3 direction = ref targetPosition.m_Direction;
			direction *= math.sign(curveDelta.y - curveDelta.x);
			return false;
		}
		float2 val2 = curveDelta;
		for (int i = 0; i < 8; i++)
		{
			float num = math.lerp(val2.x, val2.y, 0.5f);
			float3 val3 = MathUtils.Position(curve, num);
			if (math.distance(comparePosition, val3) < minDistance)
			{
				val2.y = num;
			}
			else
			{
				val2.x = num;
			}
		}
		curveDelta.y = val2.x;
		targetPosition.m_Position = MathUtils.Position(curve, val2.x);
		targetPosition.m_Direction = MathUtils.Tangent(curve, val2.x);
		ref float3 direction2 = ref targetPosition.m_Direction;
		direction2 *= math.sign(curveDelta.y - curveDelta.x);
		return true;
	}

	public static void ClearEndOfPath(ref CarCurrentLane currentLane, DynamicBuffer<CarNavigationLane> navigationLanes)
	{
		if (navigationLanes.Length != 0)
		{
			CarNavigationLane carNavigationLane = navigationLanes[navigationLanes.Length - 1];
			carNavigationLane.m_Flags &= ~CarLaneFlags.EndOfPath;
			navigationLanes[navigationLanes.Length - 1] = carNavigationLane;
		}
		else
		{
			currentLane.m_LaneFlags &= ~CarLaneFlags.EndOfPath;
		}
	}

	public static void ClearEndOfPath(ref WatercraftCurrentLane currentLane, DynamicBuffer<WatercraftNavigationLane> navigationLanes)
	{
		if (navigationLanes.Length != 0)
		{
			WatercraftNavigationLane watercraftNavigationLane = navigationLanes[navigationLanes.Length - 1];
			watercraftNavigationLane.m_Flags &= ~WatercraftLaneFlags.EndOfPath;
			navigationLanes[navigationLanes.Length - 1] = watercraftNavigationLane;
		}
		else
		{
			currentLane.m_LaneFlags &= ~WatercraftLaneFlags.EndOfPath;
		}
	}

	public static void ClearEndOfPath(ref AircraftCurrentLane currentLane, DynamicBuffer<AircraftNavigationLane> navigationLanes)
	{
		if (navigationLanes.Length != 0)
		{
			AircraftNavigationLane aircraftNavigationLane = navigationLanes[navigationLanes.Length - 1];
			aircraftNavigationLane.m_Flags &= ~AircraftLaneFlags.EndOfPath;
			navigationLanes[navigationLanes.Length - 1] = aircraftNavigationLane;
		}
		else
		{
			currentLane.m_LaneFlags &= ~AircraftLaneFlags.EndOfPath;
		}
	}

	public static void ClearEndOfPath(ref TrainCurrentLane currentLane, DynamicBuffer<TrainNavigationLane> navigationLanes)
	{
		while (navigationLanes.Length != 0)
		{
			TrainNavigationLane trainNavigationLane = navigationLanes[navigationLanes.Length - 1];
			if ((trainNavigationLane.m_Flags & TrainLaneFlags.ParkingSpace) != 0)
			{
				navigationLanes.RemoveAt(navigationLanes.Length - 1);
				continue;
			}
			trainNavigationLane.m_Flags &= ~TrainLaneFlags.EndOfPath;
			navigationLanes[navigationLanes.Length - 1] = trainNavigationLane;
			return;
		}
		currentLane.m_Front.m_LaneFlags &= ~TrainLaneFlags.EndOfPath;
	}

	public static Entity ValidateParkingSpace(Entity entity, ref Random random, ref CarCurrentLane currentLane, ref PathOwner pathOwner, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<PathElement> path, ref ComponentLookup<ParkedCar> parkedCarData, ref ComponentLookup<Blocker> blockerData, ref ComponentLookup<Curve> curveData, ref ComponentLookup<Unspawned> unspawnedData, ref ComponentLookup<Game.Net.ParkingLane> parkingLaneData, ref ComponentLookup<GarageLane> garageLaneData, ref ComponentLookup<Game.Net.ConnectionLane> connectionLaneData, ref ComponentLookup<PrefabRef> prefabRefData, ref ComponentLookup<ParkingLaneData> prefabParkingLaneData, ref ComponentLookup<ObjectGeometryData> prefabObjectGeometryData, ref BufferLookup<LaneObject> laneObjectData, ref BufferLookup<LaneOverlap> laneOverlapData, bool ignoreDriveways, bool ignoreDisabled, bool boardingOnly)
	{
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		Blocker blocker = default(Blocker);
		ParkedCar parkedCar = default(ParkedCar);
		GarageLane garageLane = default(GarageLane);
		for (int i = 0; i < navigationLanes.Length; i++)
		{
			CarNavigationLane carNavigationLane = navigationLanes[i];
			if ((carNavigationLane.m_Flags & CarLaneFlags.ParkingSpace) == 0)
			{
				continue;
			}
			if (parkingLaneData.HasComponent(carNavigationLane.m_Lane))
			{
				float minT;
				if (i == 0)
				{
					minT = currentLane.m_CurvePosition.y;
				}
				else
				{
					CarNavigationLane carNavigationLane2 = navigationLanes[i - 1];
					minT = (((carNavigationLane2.m_Flags & CarLaneFlags.Reserved) == 0) ? carNavigationLane2.m_CurvePosition.x : carNavigationLane2.m_CurvePosition.y);
				}
				float offset;
				float y = GetParkingSize(entity, ref prefabRefData, ref prefabObjectGeometryData, out offset).y;
				float curvePos = carNavigationLane.m_CurvePosition.x;
				if (FindFreeParkingSpace(ref random, carNavigationLane.m_Lane, minT, y, offset, ref curvePos, ref parkedCarData, ref curveData, ref unspawnedData, ref parkingLaneData, ref prefabRefData, ref prefabParkingLaneData, ref prefabObjectGeometryData, ref laneObjectData, ref laneOverlapData, ignoreDriveways, ignoreDisabled))
				{
					if ((carNavigationLane.m_Flags & CarLaneFlags.Validated) == 0)
					{
						carNavigationLane.m_Flags |= CarLaneFlags.Validated;
						navigationLanes[i] = carNavigationLane;
					}
					if (curvePos != carNavigationLane.m_CurvePosition.x)
					{
						SetParkingCurvePos(path, pathOwner, ref currentLane, navigationLanes, i, curvePos, ref curveData);
					}
				}
				else
				{
					if ((carNavigationLane.m_Flags & CarLaneFlags.Validated) != 0)
					{
						carNavigationLane.m_Flags &= ~CarLaneFlags.Validated;
						navigationLanes[i] = carNavigationLane;
					}
					if (boardingOnly)
					{
						if (i == 0)
						{
							currentLane.m_LaneFlags |= CarLaneFlags.EndOfPath;
							navigationLanes.Clear();
						}
						else if (blockerData.TryGetComponent(entity, ref blocker) && parkedCarData.TryGetComponent(blocker.m_Blocker, ref parkedCar) && parkedCar.m_Lane == carNavigationLane.m_Lane)
						{
							CarNavigationLane carNavigationLane3 = navigationLanes[i - 1];
							carNavigationLane3.m_Flags |= CarLaneFlags.EndOfPath;
							navigationLanes[i - 1] = carNavigationLane3;
							navigationLanes.RemoveRange(i, navigationLanes.Length - i);
						}
					}
					else
					{
						if (i == 0)
						{
							currentLane.m_CurvePosition.z = 1f;
						}
						pathOwner.m_State |= PathFlags.Obsolete;
					}
				}
			}
			else if (!boardingOnly && garageLaneData.TryGetComponent(carNavigationLane.m_Lane, ref garageLane))
			{
				Game.Net.ConnectionLane connectionLane = connectionLaneData[carNavigationLane.m_Lane];
				if (garageLane.m_VehicleCount < garageLane.m_VehicleCapacity && (ignoreDisabled || (connectionLane.m_Flags & ConnectionLaneFlags.Disabled) == 0))
				{
					if ((carNavigationLane.m_Flags & CarLaneFlags.Validated) == 0)
					{
						carNavigationLane.m_Flags |= CarLaneFlags.Validated;
						navigationLanes[i] = carNavigationLane;
					}
				}
				else
				{
					if ((carNavigationLane.m_Flags & CarLaneFlags.Validated) != 0)
					{
						carNavigationLane.m_Flags &= ~CarLaneFlags.Validated;
						navigationLanes[i] = carNavigationLane;
					}
					pathOwner.m_State |= PathFlags.Obsolete;
				}
			}
			else if ((carNavigationLane.m_Flags & CarLaneFlags.Validated) == 0)
			{
				carNavigationLane.m_Flags |= CarLaneFlags.Validated;
				navigationLanes[i] = carNavigationLane;
			}
			return carNavigationLane.m_Lane;
		}
		return Entity.Null;
	}

	public static bool FindFreeParkingSpace(ref Random random, Entity lane, float minT, float parkingLength, float parkingOffset, ref float curvePos, ref ComponentLookup<ParkedCar> parkedCarData, ref ComponentLookup<Curve> curveData, ref ComponentLookup<Unspawned> unspawnedData, ref ComponentLookup<Game.Net.ParkingLane> parkingLaneData, ref ComponentLookup<PrefabRef> prefabRefData, ref ComponentLookup<ParkingLaneData> prefabParkingLaneData, ref ComponentLookup<ObjectGeometryData> prefabObjectGeometryData, ref BufferLookup<LaneObject> laneObjectData, ref BufferLookup<LaneOverlap> laneOverlapData, bool ignoreDriveways, bool ignoreDisabled)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_087a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_088f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0896: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0904: Unknown result type (might be due to invalid IL or missing references)
		//IL_090b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_092f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0936: Unknown result type (might be due to invalid IL or missing references)
		//IL_0917: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0841: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		Curve curve = curveData[lane];
		Game.Net.ParkingLane parkingLane = parkingLaneData[lane];
		if ((parkingLane.m_Flags & ParkingLaneFlags.VirtualLane) != 0)
		{
			return false;
		}
		if (ignoreDisabled && (parkingLane.m_Flags & ParkingLaneFlags.ParkingDisabled) != 0)
		{
			return false;
		}
		PrefabRef prefabRef = prefabRefData[lane];
		DynamicBuffer<LaneObject> val = laneObjectData[lane];
		DynamicBuffer<LaneOverlap> val2 = default(DynamicBuffer<LaneOverlap>);
		if (!ignoreDriveways)
		{
			val2 = laneOverlapData[lane];
		}
		ParkingLaneData prefabParkingLane = prefabParkingLaneData[prefabRef.m_Prefab];
		if (prefabParkingLane.m_MaxCarLength != 0f && prefabParkingLane.m_MaxCarLength < parkingLength)
		{
			return false;
		}
		if (prefabParkingLane.m_SlotInterval != 0f)
		{
			int parkingSlotCount = NetUtils.GetParkingSlotCount(curve, parkingLane, prefabParkingLane);
			float parkingSlotInterval = NetUtils.GetParkingSlotInterval(curve, parkingLane, prefabParkingLane, parkingSlotCount);
			float3 val3 = curve.m_Bezier.a;
			float2 val4 = float2.op_Implicit(0f);
			float num = 0f;
			float num2 = math.max((parkingLane.m_Flags & (ParkingLaneFlags.StartingLane | ParkingLaneFlags.EndingLane)) switch
			{
				ParkingLaneFlags.StartingLane => curve.m_Length - (float)parkingSlotCount * parkingSlotInterval, 
				ParkingLaneFlags.EndingLane => 0f, 
				_ => (curve.m_Length - (float)parkingSlotCount * parkingSlotInterval) * 0.5f, 
			}, 0f);
			int i = -1;
			int num3 = 0;
			float num4 = curvePos;
			float num5 = 2f;
			int num6 = 0;
			while (num6 < val.Length)
			{
				LaneObject laneObject = val[num6++];
				if (parkedCarData.HasComponent(laneObject.m_LaneObject) && !unspawnedData.HasComponent(laneObject.m_LaneObject))
				{
					num5 = laneObject.m_CurvePosition.x;
					break;
				}
			}
			float2 val5 = float2.op_Implicit(2f);
			int num7 = 0;
			if (!ignoreDriveways && num7 < val2.Length)
			{
				LaneOverlap laneOverlap = val2[num7++];
				val5 = new float2((float)(int)laneOverlap.m_ThisStart, (float)(int)laneOverlap.m_ThisEnd) * 0.003921569f;
			}
			for (int j = 1; j <= 16; j++)
			{
				float num8 = (float)j * 0.0625f;
				float3 val6 = MathUtils.Position(curve.m_Bezier, num8);
				for (num += math.distance(val3, val6); num >= num2 || (j == 16 && i < parkingSlotCount); i++)
				{
					val4.y = math.select(num8, math.lerp(val4.x, num8, num2 / num), num2 < num);
					bool flag = false;
					if (num5 <= val4.y)
					{
						num5 = 2f;
						flag = true;
						while (num6 < val.Length)
						{
							LaneObject laneObject2 = val[num6++];
							if (parkedCarData.HasComponent(laneObject2.m_LaneObject) && !unspawnedData.HasComponent(laneObject2.m_LaneObject) && laneObject2.m_CurvePosition.x > val4.y)
							{
								num5 = laneObject2.m_CurvePosition.x;
								break;
							}
						}
					}
					if (!ignoreDriveways && val5.x < val4.y)
					{
						flag = true;
						if (val5.y <= val4.y)
						{
							val5 = float2.op_Implicit(2f);
							while (num7 < val2.Length)
							{
								LaneOverlap laneOverlap2 = val2[num7++];
								float2 val7 = new float2((float)(int)laneOverlap2.m_ThisStart, (float)(int)laneOverlap2.m_ThisEnd) * 0.003921569f;
								if (val7.y > val4.y)
								{
									val5 = val7;
									break;
								}
							}
						}
					}
					if (!flag && i >= 0 && i < parkingSlotCount)
					{
						if (curvePos >= val4.x && curvePos <= val4.y)
						{
							curvePos = math.lerp(val4.x, val4.y, 0.5f);
							return true;
						}
						if (val4.y > minT)
						{
							num3++;
							if (((Random)(ref random)).NextInt(num3) == 0)
							{
								num4 = math.lerp(val4.x, val4.y, 0.5f);
							}
						}
					}
					num -= num2;
					val4.x = val4.y;
					num2 = parkingSlotInterval;
				}
				val3 = val6;
			}
			if (num4 != curvePos && prefabParkingLane.m_SlotAngle <= 0.25f)
			{
				if (parkingOffset > 0f)
				{
					Bounds1 val8 = default(Bounds1);
					((Bounds1)(ref val8))._002Ector(num4, 1f);
					MathUtils.ClampLength(curve.m_Bezier, ref val8, parkingOffset);
					num4 = val8.max;
				}
				else if (parkingOffset < 0f)
				{
					Bounds1 val9 = default(Bounds1);
					((Bounds1)(ref val9))._002Ector(0f, num4);
					MathUtils.ClampLengthInverse(curve.m_Bezier, ref val9, 0f - parkingOffset);
					num4 = val9.min;
				}
			}
			curvePos = num4;
			return num3 != 0;
		}
		float2 val10 = default(float2);
		float2 val11 = default(float2);
		int num9 = 0;
		float3 val12 = default(float3);
		float2 val13 = float2.op_Implicit(math.select(0f, 0.5f, (parkingLane.m_Flags & ParkingLaneFlags.StartingLane) == 0));
		float3 val14 = curve.m_Bezier.a;
		float num10 = 2f;
		float2 val15 = float2.op_Implicit(0f);
		int num11 = 0;
		while (num11 < val.Length)
		{
			LaneObject laneObject3 = val[num11++];
			if (parkedCarData.HasComponent(laneObject3.m_LaneObject) && !unspawnedData.HasComponent(laneObject3.m_LaneObject))
			{
				num10 = laneObject3.m_CurvePosition.x;
				val15 = GetParkingOffsets(laneObject3.m_LaneObject, ref prefabRefData, ref prefabObjectGeometryData) + 1f;
				break;
			}
		}
		float2 val16 = float2.op_Implicit(2f);
		int num12 = 0;
		if (!ignoreDriveways && num12 < val2.Length)
		{
			LaneOverlap laneOverlap3 = val2[num12++];
			val16 = new float2((float)(int)laneOverlap3.m_ThisStart, (float)(int)laneOverlap3.m_ThisEnd) * 0.003921569f;
		}
		while (true)
		{
			if (num10 != 2f || val16.x != 2f)
			{
				float x;
				if (ignoreDriveways || num10 <= val16.x)
				{
					((float3)(ref val12)).yz = float2.op_Implicit(num10);
					val13.y = val15.x;
					x = val15.y;
					num10 = 2f;
					while (num11 < val.Length)
					{
						LaneObject laneObject4 = val[num11++];
						if (parkedCarData.HasComponent(laneObject4.m_LaneObject) && !unspawnedData.HasComponent(laneObject4.m_LaneObject))
						{
							num10 = laneObject4.m_CurvePosition.x;
							val15 = GetParkingOffsets(laneObject4.m_LaneObject, ref prefabRefData, ref prefabObjectGeometryData) + 1f;
							break;
						}
					}
				}
				else
				{
					((float3)(ref val12)).yz = val16;
					val13.y = 0.5f;
					x = 0.5f;
					val16 = float2.op_Implicit(2f);
					while (num12 < val2.Length)
					{
						LaneOverlap laneOverlap4 = val2[num12++];
						float2 val17 = new float2((float)(int)laneOverlap4.m_ThisStart, (float)(int)laneOverlap4.m_ThisEnd) * 0.003921569f;
						if (val17.x <= val12.z)
						{
							val12.z = math.max(val12.z, val17.y);
							continue;
						}
						val16 = val17;
						break;
					}
				}
				float3 val18 = MathUtils.Position(curve.m_Bezier, val12.y);
				if (math.distance(val14, val18) - math.csum(val13) >= parkingLength)
				{
					if (curvePos > val12.x && curvePos < val12.y)
					{
						num9++;
						val10 = ((float3)(ref val12)).xy;
						val11 = val13;
						break;
					}
					if (val12.y > minT)
					{
						num9++;
						if (((Random)(ref random)).NextInt(num9) == 0)
						{
							val10 = ((float3)(ref val12)).xy;
							val11 = val13;
						}
					}
				}
				val12.x = val12.z;
				val13.x = x;
				val14 = MathUtils.Position(curve.m_Bezier, val12.z);
				continue;
			}
			val12.y = 1f;
			val13.y = math.select(0f, 0.5f, (parkingLane.m_Flags & ParkingLaneFlags.EndingLane) == 0);
			if (!(math.distance(val14, curve.m_Bezier.d) - math.csum(val13) >= parkingLength))
			{
				break;
			}
			if (curvePos > val12.x && curvePos < val12.y)
			{
				num9++;
				val10 = ((float3)(ref val12)).xy;
				val11 = val13;
			}
			else if (val12.y > minT)
			{
				num9++;
				if (((Random)(ref random)).NextInt(num9) == 0)
				{
					val10 = ((float3)(ref val12)).xy;
					val11 = val13;
				}
			}
			break;
		}
		if (num9 != 0)
		{
			val11 += parkingLength * 0.5f;
			val11.x += parkingOffset;
			val11.y -= parkingOffset;
			Bounds1 val19 = default(Bounds1);
			((Bounds1)(ref val19))._002Ector(val10.x, val10.y);
			Bounds1 val20 = default(Bounds1);
			((Bounds1)(ref val20))._002Ector(val10.x, val10.y);
			MathUtils.ClampLength(curve.m_Bezier, ref val19, val11.x);
			MathUtils.ClampLengthInverse(curve.m_Bezier, ref val20, val11.y);
			if (curvePos < val19.max || curvePos > val20.min)
			{
				val19.max = math.min(math.max(val19.max, minT), val20.min);
				if (val19.max < val20.min)
				{
					curvePos = ((Random)(ref random)).NextFloat(val19.max, val20.min);
				}
				else
				{
					curvePos = math.lerp(val19.max, val20.min, 0.5f);
				}
			}
			return true;
		}
		return false;
	}

	public static float GetLaneOffset(ObjectGeometryData prefabObjectGeometryData, NetLaneData prefabLaneData, float lanePosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float num = prefabObjectGeometryData.m_Bounds.max.x - prefabObjectGeometryData.m_Bounds.min.x;
		float num2 = math.max(0f, prefabLaneData.m_Width - num);
		return lanePosition * num2;
	}

	public static float3 GetLanePosition(Bezier4x3 curve, float curvePosition, float laneOffset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float3 result = MathUtils.Position(curve, curvePosition);
		float3 val = MathUtils.Tangent(curve, curvePosition);
		float2 val2 = math.normalizesafe(((float3)(ref val)).xz, default(float2));
		((float3)(ref result)).xz = ((float3)(ref result)).xz + MathUtils.Right(val2) * laneOffset;
		return result;
	}

	public static float3 GetConnectionParkingPosition(Game.Net.ConnectionLane connectionLane, Bezier4x3 curve, float curvePosition)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.frac(curvePosition * new float3(100f, 10000f, 1000000f));
		if ((connectionLane.m_Flags & ConnectionLaneFlags.Outside) != 0)
		{
			val.z -= 0.5f;
			val *= new float3(40f, 10f, 50f);
		}
		else
		{
			((float3)(ref val)).xz = ((float3)(ref val)).xz - 0.5f;
			val *= new float3(25f, 10f, 25f);
		}
		float3 result = MathUtils.Position(curve, curvePosition);
		float2 val2 = math.sign(((float3)(ref result)).xz);
		float2 val3 = math.abs(((float3)(ref result)).xz);
		float2 val4 = math.select(new float2(val2.x, 0f), new float2(0f, val2.y), val3.y > val3.x);
		float2 val5 = MathUtils.Right(val4);
		((float3)(ref result)).xz = ((float3)(ref result)).xz + (val4 * val.x + val5 * val.z);
		result.y += val.y;
		return result;
	}

	public static Bounds3 GetConnectionParkingBounds(Game.Net.ConnectionLane connectionLane, Bezier4x3 curve)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 result = MathUtils.Bounds(curve);
		float3 val = math.select(new float3(25f, 10f, 25f), new float3(80f, 10f, 80f), (connectionLane.m_Flags & ConnectionLaneFlags.Outside) != 0);
		((float3)(ref val)).xz = ((float3)(ref val)).xz * 0.5f;
		ref float3 min = ref result.min;
		((float3)(ref min)).xz = ((float3)(ref min)).xz - ((float3)(ref val)).xz;
		ref float3 max = ref result.max;
		max += val;
		return result;
	}

	public static void CheckUnspawned(int jobIndex, Entity entity, CarCurrentLane currentLane, bool isUnspawned, ParallelWriter commandBuffer)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if ((currentLane.m_LaneFlags & (CarLaneFlags.Connection | CarLaneFlags.ResetSpeed)) != 0)
		{
			if (!isUnspawned)
			{
				((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, entity, default(Unspawned));
				((ParallelWriter)(ref commandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
			}
		}
		else if ((currentLane.m_LaneFlags & (CarLaneFlags.TransformTarget | CarLaneFlags.ParkingSpace)) == 0 && isUnspawned)
		{
			((ParallelWriter)(ref commandBuffer)).RemoveComponent<Unspawned>(jobIndex, entity);
			((ParallelWriter)(ref commandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
		}
	}

	public static void CheckUnspawned(int jobIndex, Entity entity, TrainCurrentLane currentLane, bool isUnspawned, ParallelWriter commandBuffer)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if ((currentLane.m_Front.m_LaneFlags & (TrainLaneFlags.ResetSpeed | TrainLaneFlags.Connection)) != 0)
		{
			if (!isUnspawned)
			{
				((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, entity, default(Unspawned));
				((ParallelWriter)(ref commandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
			}
		}
		else if (isUnspawned)
		{
			((ParallelWriter)(ref commandBuffer)).RemoveComponent<Unspawned>(jobIndex, entity);
			((ParallelWriter)(ref commandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
		}
	}

	public static void CheckUnspawned(int jobIndex, Entity entity, AircraftCurrentLane currentLane, bool isUnspawned, ParallelWriter commandBuffer)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if ((currentLane.m_LaneFlags & (AircraftLaneFlags.Connection | AircraftLaneFlags.ResetSpeed)) != 0)
		{
			if (!isUnspawned)
			{
				((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, entity, default(Unspawned));
				((ParallelWriter)(ref commandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
			}
		}
		else if ((currentLane.m_LaneFlags & AircraftLaneFlags.TransformTarget) == 0 && isUnspawned)
		{
			((ParallelWriter)(ref commandBuffer)).RemoveComponent<Unspawned>(jobIndex, entity);
			((ParallelWriter)(ref commandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
		}
	}

	public static void CheckUnspawned(int jobIndex, Entity entity, WatercraftCurrentLane currentLane, bool isUnspawned, ParallelWriter commandBuffer)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((currentLane.m_LaneFlags & (WatercraftLaneFlags.ResetSpeed | WatercraftLaneFlags.Connection)) != 0)
		{
			if (!isUnspawned)
			{
				((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, entity, default(Unspawned));
				((ParallelWriter)(ref commandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
			}
		}
		else if ((currentLane.m_LaneFlags & WatercraftLaneFlags.TransformTarget) == 0 && isUnspawned)
		{
			((ParallelWriter)(ref commandBuffer)).RemoveComponent<Unspawned>(jobIndex, entity);
			((ParallelWriter)(ref commandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
		}
	}

	public static bool GetPathElement(int elementIndex, DynamicBuffer<CarNavigationLane> navigationLanes, NativeArray<PathElement> pathElements, out PathElement pathElement)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (elementIndex < navigationLanes.Length)
		{
			CarNavigationLane carNavigationLane = navigationLanes[elementIndex];
			pathElement = new PathElement(carNavigationLane.m_Lane, carNavigationLane.m_CurvePosition);
			return true;
		}
		elementIndex -= navigationLanes.Length;
		if (elementIndex < pathElements.Length)
		{
			pathElement = pathElements[elementIndex];
			return true;
		}
		pathElement = default(PathElement);
		return false;
	}

	public static bool GetPathElement(int elementIndex, DynamicBuffer<WatercraftNavigationLane> navigationLanes, NativeArray<PathElement> pathElements, out PathElement pathElement)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (elementIndex < navigationLanes.Length)
		{
			WatercraftNavigationLane watercraftNavigationLane = navigationLanes[elementIndex];
			pathElement = new PathElement(watercraftNavigationLane.m_Lane, watercraftNavigationLane.m_CurvePosition);
			return true;
		}
		elementIndex -= navigationLanes.Length;
		if (elementIndex < pathElements.Length)
		{
			pathElement = pathElements[elementIndex];
			return true;
		}
		pathElement = default(PathElement);
		return false;
	}

	public static bool SetTriangleTarget(float3 left, float3 right, float3 next, float3 comparePosition, int elementIndex, DynamicBuffer<CarNavigationLane> navigationLanes, NativeArray<PathElement> pathElements, ref float3 targetPosition, float minDistance, float lanePosition, float curveDelta, float navigationSize, bool isSingle, ComponentLookup<Transform> transforms, ComponentLookup<AreaLane> areaLanes, ComponentLookup<Curve> curves)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		targetPosition = CalculateTriangleTarget(left, right, next, targetPosition, elementIndex, navigationLanes, pathElements, lanePosition, curveDelta, navigationSize, isSingle, transforms, areaLanes, curves);
		return math.distance(((float3)(ref comparePosition)).xz, ((float3)(ref targetPosition)).xz) >= minDistance;
	}

	public static bool SetTriangleTarget(float3 left, float3 right, float3 next, float3 comparePosition, int elementIndex, DynamicBuffer<WatercraftNavigationLane> navigationLanes, NativeArray<PathElement> pathElements, ref float3 targetPosition, float minDistance, float lanePosition, float curveDelta, float navigationSize, bool isSingle, ComponentLookup<Transform> transforms, ComponentLookup<AreaLane> areaLanes, ComponentLookup<Curve> curves)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		targetPosition = CalculateTriangleTarget(left, right, next, targetPosition, elementIndex, navigationLanes, pathElements, lanePosition, curveDelta, navigationSize, isSingle, transforms, areaLanes, curves);
		return math.distance(((float3)(ref comparePosition)).xz, ((float3)(ref targetPosition)).xz) >= minDistance;
	}

	public static bool SetTriangleTarget(float3 left, float3 right, float3 next, float3 comparePosition, float3 lastTarget, ref float3 targetPosition, float minDistance, float navigationSize, bool isSingle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		targetPosition = CalculateTriangleTarget(left, right, next, lastTarget, navigationSize, isSingle);
		return math.distance(((float3)(ref comparePosition)).xz, ((float3)(ref targetPosition)).xz) >= minDistance;
	}

	private static float3 CalculateTriangleTarget(float3 left, float3 right, float3 next, float3 lastTarget, int elementIndex, DynamicBuffer<CarNavigationLane> navigationLanes, NativeArray<PathElement> pathElements, float lanePosition, float curveDelta, float navigationSize, bool isSingle, ComponentLookup<Transform> transforms, ComponentLookup<AreaLane> areaLanes, ComponentLookup<Curve> curves)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (GetPathElement(elementIndex, navigationLanes, pathElements, out var pathElement))
		{
			Transform transform = default(Transform);
			if (transforms.TryGetComponent(pathElement.m_Target, ref transform))
			{
				return CalculateTriangleTarget(left, right, next, transform.m_Position, navigationSize, isSingle);
			}
			if (areaLanes.HasComponent(pathElement.m_Target))
			{
				return CalculateTriangleTarget(left, right, next, lastTarget, navigationSize, isSingle);
			}
			Curve curve = default(Curve);
			if (curves.TryGetComponent(pathElement.m_Target, ref curve))
			{
				float3 target = MathUtils.Position(curve.m_Bezier, pathElement.m_TargetDelta.x);
				return CalculateTriangleTarget(left, right, next, target, navigationSize, isSingle);
			}
		}
		return CalculateTriangleTarget(left, right, next, lanePosition, curveDelta, navigationSize, isSingle);
	}

	private static float3 CalculateTriangleTarget(float3 left, float3 right, float3 next, float3 lastTarget, int elementIndex, DynamicBuffer<WatercraftNavigationLane> navigationLanes, NativeArray<PathElement> pathElements, float lanePosition, float curveDelta, float navigationSize, bool isSingle, ComponentLookup<Transform> transforms, ComponentLookup<AreaLane> areaLanes, ComponentLookup<Curve> curves)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (GetPathElement(elementIndex, navigationLanes, pathElements, out var pathElement))
		{
			Transform transform = default(Transform);
			if (transforms.TryGetComponent(pathElement.m_Target, ref transform))
			{
				return CalculateTriangleTarget(left, right, next, transform.m_Position, navigationSize, isSingle);
			}
			if (areaLanes.HasComponent(pathElement.m_Target))
			{
				return CalculateTriangleTarget(left, right, next, lastTarget, navigationSize, isSingle);
			}
			Curve curve = default(Curve);
			if (curves.TryGetComponent(pathElement.m_Target, ref curve))
			{
				float3 target = MathUtils.Position(curve.m_Bezier, pathElement.m_TargetDelta.x);
				return CalculateTriangleTarget(left, right, next, target, navigationSize, isSingle);
			}
		}
		return CalculateTriangleTarget(left, right, next, lanePosition, curveDelta, navigationSize, isSingle);
	}

	private static float3 CalculateTriangleTarget(float3 left, float3 right, float3 next, float3 target, float navigationSize, bool isSingle)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		float num = navigationSize * 0.5f;
		Triangle3 val = default(Triangle3);
		((Triangle3)(ref val))._002Ector(next, left, right);
		if (isSingle)
		{
			float num2 = default(float);
			float3 val2 = MathUtils.Incenter(val, ref num2);
			float num3 = default(float);
			MathUtils.Incenter(((Triangle3)(ref val)).xz, ref num3);
			float num4 = math.saturate(num / num3);
			ref float3 a = ref val.a;
			a += (val2 - val.a) * num4;
			ref float3 b = ref val.b;
			b += (val2 - val.b) * num4;
			ref float3 c = ref val.c;
			c += (val2 - val.c) * num4;
			float2 val3 = default(float2);
			if (MathUtils.Distance(((Triangle3)(ref val)).xz, ((float3)(ref target)).xz, ref val3) != 0f)
			{
				target = MathUtils.Position(val, val3);
			}
		}
		else
		{
			Segment val4 = ((Triangle3)(ref val)).ba;
			float2 val6 = default(float2);
			float2 val5 = default(float2);
			val5.x = MathUtils.Distance(((Segment)(ref val4)).xz, ((float3)(ref target)).xz, ref val6.x);
			val4 = ((Triangle3)(ref val)).ca;
			val5.y = MathUtils.Distance(((Segment)(ref val4)).xz, ((float3)(ref target)).xz, ref val6.y);
			val5 = ((!MathUtils.Intersect(((Triangle3)(ref val)).xz, ((float3)(ref target)).xz)) ? math.select(new float2(val5.x, 0f - val5.y), new float2(0f - val5.x, val5.y), val5.x > val5.y) : (-val5));
			if (math.any(val5 > 0f - num))
			{
				if (val5.y <= 0f - num)
				{
					float2 val7 = math.normalizesafe(MathUtils.Right(((float3)(ref left)).xz - ((float3)(ref next)).xz), default(float2)) * num;
					target = MathUtils.Position(((Triangle3)(ref val)).ba, val6.x);
					((float3)(ref target)).xz = ((float3)(ref target)).xz + math.select(val7, -val7, math.dot(val7, ((float3)(ref right)).xz - ((float3)(ref next)).xz) < 0f);
				}
				else if (val5.x <= 0f - num)
				{
					float2 val8 = math.normalizesafe(MathUtils.Left(((float3)(ref right)).xz - ((float3)(ref next)).xz), default(float2)) * num;
					target = MathUtils.Position(((Triangle3)(ref val)).ca, val6.y);
					((float3)(ref target)).xz = ((float3)(ref target)).xz + math.select(val8, -val8, math.dot(val8, ((float3)(ref left)).xz - ((float3)(ref next)).xz) < 0f);
				}
				else
				{
					target = math.lerp(MathUtils.Position(((Triangle3)(ref val)).ba, val6.x), MathUtils.Position(((Triangle3)(ref val)).ca, val6.y), 0.5f);
				}
			}
		}
		return target;
	}

	private static float3 CalculateTriangleTarget(float3 left, float3 right, float3 next, float lanePosition, float curveDelta, float navigationSize, bool isSingle)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		float num = navigationSize * 0.5f;
		Segment val = default(Segment);
		((Segment)(ref val))._002Ector(left, right);
		float num2 = lanePosition * math.saturate(1f - navigationSize / MathUtils.Length(((Segment)(ref val)).xz));
		val.a = MathUtils.Position(val, num2 + 0.5f);
		val.b = next;
		float num3;
		if (isSingle)
		{
			num3 = (math.sqrt(math.saturate(1f - curveDelta)) - 0.5f) * math.saturate(1f - navigationSize / MathUtils.Length(((Segment)(ref val)).xz)) + 0.5f;
		}
		else
		{
			float num4 = curveDelta * 2f;
			num4 = math.select(1f - num4, num4 - 1f, curveDelta > 0.5f);
			num3 = math.sqrt(math.saturate(1f - num4)) * math.saturate(1f - num / MathUtils.Length(((Segment)(ref val)).xz));
		}
		return MathUtils.Position(val, num3);
	}

	public static bool SetAreaTarget(float3 prev2, float3 prev, float3 left, float3 right, float3 next, Entity areaEntity, DynamicBuffer<Game.Areas.Node> nodes, float3 comparePosition, int elementIndex, DynamicBuffer<CarNavigationLane> navigationLanes, NativeArray<PathElement> pathElements, ref float3 targetPosition, float minDistance, float lanePosition, float curveDelta, float navigationSize, bool isBackward, ComponentLookup<Transform> transforms, ComponentLookup<AreaLane> areaLanes, ComponentLookup<Curve> curves, ComponentLookup<Owner> owners)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		float num = navigationSize * 0.5f;
		Segment val = default(Segment);
		((Segment)(ref val))._002Ector(left, right);
		float num2 = 1f / MathUtils.Length(((Segment)(ref val)).xz);
		Bounds1 val2 = default(Bounds1);
		((Bounds1)(ref val2))._002Ector(math.min(0.5f, num * num2), math.max(0.5f, 1f - num * num2));
		int num3 = elementIndex;
		PathElement pathElement;
		Owner owner = default(Owner);
		bool4 val3 = default(bool4);
		Segment val4 = default(Segment);
		Segment val5 = default(Segment);
		float2 val8 = default(float2);
		while (GetPathElement(elementIndex, navigationLanes, pathElements, out pathElement) && owners.TryGetComponent(pathElement.m_Target, ref owner) && owner.m_Owner == areaEntity)
		{
			AreaLane areaLane = areaLanes[pathElement.m_Target];
			((bool4)(ref val3))._002Ector(pathElement.m_TargetDelta < 0.5f, pathElement.m_TargetDelta > 0.5f);
			if (math.any(((bool4)(ref val3)).xy & ((bool4)(ref val3)).wz))
			{
				((Segment)(ref val4))._002Ector(comparePosition, nodes[areaLane.m_Nodes.y].m_Position);
				((Segment)(ref val5))._002Ector(comparePosition, nodes[areaLane.m_Nodes.z].m_Position);
				Bounds1 val6 = val2;
				Bounds1 val7 = val2;
				if (MathUtils.Intersect(Line2.op_Implicit(((Segment)(ref val)).xz), Line2.op_Implicit(((Segment)(ref val4)).xz), ref val8))
				{
					float num4 = math.max(math.max(0f, 0.4f * math.min(val8.y, 1f - val8.y) * MathUtils.Length(((Segment)(ref val4)).xz) * num2), math.max(val8.x - val2.max, val2.min - val8.x));
					if (num4 < val2.max - val2.min)
					{
						((Bounds1)(ref val6))._002Ector(math.max(val2.min, math.min(val2.max, val8.x) - num4), math.min(val2.max, math.max(val2.min, val8.x) + num4));
					}
				}
				if (MathUtils.Intersect(Line2.op_Implicit(((Segment)(ref val)).xz), Line2.op_Implicit(((Segment)(ref val5)).xz), ref val8))
				{
					float num5 = math.max(math.max(0f, 0.4f * math.min(val8.y, 1f - val8.y) * MathUtils.Length(((Segment)(ref val4)).xz) * num2), math.max(val8.x - val2.max, val2.min - val8.x));
					if (num5 < val2.max - val2.min)
					{
						((Bounds1)(ref val7))._002Ector(math.max(val2.min, math.min(val2.max, val8.x) - num5), math.min(val2.max, math.max(val2.min, val8.x) + num5));
					}
				}
				if (!(((Bounds1)(ref val6)).Equals(val2) & ((Bounds1)(ref val7)).Equals(val2)))
				{
					val2 = val6 | val7;
					elementIndex++;
					continue;
				}
				elementIndex = navigationLanes.Length + pathElements.Length;
			}
			elementIndex++;
			break;
		}
		if (elementIndex - 1 < navigationLanes.Length + pathElements.Length)
		{
			float3 val9;
			if (elementIndex > num3)
			{
				GetPathElement(elementIndex - 1, navigationLanes, pathElements, out var pathElement2);
				AreaLane areaLane2 = areaLanes[pathElement2.m_Target];
				bool flag = pathElement2.m_TargetDelta.y > 0.5f;
				val9 = CalculateTriangleTarget(nodes[areaLane2.m_Nodes.y].m_Position, nodes[areaLane2.m_Nodes.z].m_Position, nodes[math.select(areaLane2.m_Nodes.x, areaLane2.m_Nodes.w, flag)].m_Position, lanePosition: math.select(lanePosition, 0f - lanePosition, pathElement2.m_TargetDelta.y < pathElement2.m_TargetDelta.x != isBackward), lastTarget: targetPosition, elementIndex: elementIndex, navigationLanes: navigationLanes, pathElements: pathElements, curveDelta: pathElement2.m_TargetDelta.y, navigationSize: navigationSize, isSingle: false, transforms: transforms, areaLanes: areaLanes, curves: curves);
			}
			else
			{
				val9 = CalculateTriangleTarget(left, right, next, targetPosition, elementIndex, navigationLanes, pathElements, lanePosition, curveDelta, navigationSize, isSingle: false, transforms, areaLanes, curves);
			}
			Segment val10 = default(Segment);
			((Segment)(ref val10))._002Ector(comparePosition, val9);
			float2 val11 = default(float2);
			if (MathUtils.Intersect(Line2.op_Implicit(((Segment)(ref val)).xz), Line2.op_Implicit(((Segment)(ref val10)).xz), ref val11))
			{
				float num6 = math.max(math.max(0f, 0.4f * math.min(val11.y, 1f - val11.y) * MathUtils.Length(((Segment)(ref val10)).xz) * num2), math.max(val11.x - val2.max, val2.min - val11.x));
				if (num6 < val2.max - val2.min)
				{
					((Bounds1)(ref val2))._002Ector(math.max(val2.min, math.min(val2.max, val11.x) - num6), math.min(val2.max, math.max(val2.min, val11.x) + num6));
				}
			}
		}
		float lanePosition2 = math.lerp(val2.min, val2.max, lanePosition + 0.5f);
		targetPosition = CalculateAreaTarget(prev2, prev, left, right, comparePosition, minDistance, lanePosition2, navigationSize, out var farEnough);
		if (!farEnough)
		{
			return math.distance(((float3)(ref comparePosition)).xz, ((float3)(ref targetPosition)).xz) >= minDistance;
		}
		return true;
	}

	public static bool SetAreaTarget(float3 prev2, float3 prev, float3 left, float3 right, float3 next, Entity areaEntity, DynamicBuffer<Game.Areas.Node> nodes, float3 comparePosition, int elementIndex, DynamicBuffer<WatercraftNavigationLane> navigationLanes, NativeArray<PathElement> pathElements, ref float3 targetPosition, float minDistance, float lanePosition, float curveDelta, float navigationSize, bool isBackward, ComponentLookup<Transform> transforms, ComponentLookup<AreaLane> areaLanes, ComponentLookup<Curve> curves, ComponentLookup<Owner> owners)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		float num = navigationSize * 0.5f;
		Segment val = default(Segment);
		((Segment)(ref val))._002Ector(left, right);
		float num2 = 1f / MathUtils.Length(((Segment)(ref val)).xz);
		Bounds1 val2 = default(Bounds1);
		((Bounds1)(ref val2))._002Ector(math.min(0.5f, num * num2), math.max(0.5f, 1f - num * num2));
		int num3 = elementIndex;
		PathElement pathElement;
		Owner owner = default(Owner);
		bool4 val3 = default(bool4);
		Segment val4 = default(Segment);
		Segment val5 = default(Segment);
		float2 val8 = default(float2);
		while (GetPathElement(elementIndex, navigationLanes, pathElements, out pathElement) && owners.TryGetComponent(pathElement.m_Target, ref owner) && owner.m_Owner == areaEntity)
		{
			AreaLane areaLane = areaLanes[pathElement.m_Target];
			((bool4)(ref val3))._002Ector(pathElement.m_TargetDelta < 0.5f, pathElement.m_TargetDelta > 0.5f);
			if (math.any(((bool4)(ref val3)).xy & ((bool4)(ref val3)).wz))
			{
				((Segment)(ref val4))._002Ector(comparePosition, nodes[areaLane.m_Nodes.y].m_Position);
				((Segment)(ref val5))._002Ector(comparePosition, nodes[areaLane.m_Nodes.z].m_Position);
				Bounds1 val6 = val2;
				Bounds1 val7 = val2;
				if (MathUtils.Intersect(Line2.op_Implicit(((Segment)(ref val)).xz), Line2.op_Implicit(((Segment)(ref val4)).xz), ref val8))
				{
					float num4 = math.max(math.max(0f, 0.4f * math.min(val8.y, 1f - val8.y) * MathUtils.Length(((Segment)(ref val4)).xz) * num2), math.max(val8.x - val2.max, val2.min - val8.x));
					if (num4 < val2.max - val2.min)
					{
						((Bounds1)(ref val6))._002Ector(math.max(val2.min, math.min(val2.max, val8.x) - num4), math.min(val2.max, math.max(val2.min, val8.x) + num4));
					}
				}
				if (MathUtils.Intersect(Line2.op_Implicit(((Segment)(ref val)).xz), Line2.op_Implicit(((Segment)(ref val5)).xz), ref val8))
				{
					float num5 = math.max(math.max(0f, 0.4f * math.min(val8.y, 1f - val8.y) * MathUtils.Length(((Segment)(ref val4)).xz) * num2), math.max(val8.x - val2.max, val2.min - val8.x));
					if (num5 < val2.max - val2.min)
					{
						((Bounds1)(ref val7))._002Ector(math.max(val2.min, math.min(val2.max, val8.x) - num5), math.min(val2.max, math.max(val2.min, val8.x) + num5));
					}
				}
				if (!(((Bounds1)(ref val6)).Equals(val2) & ((Bounds1)(ref val7)).Equals(val2)))
				{
					val2 = val6 | val7;
					elementIndex++;
					continue;
				}
				elementIndex = navigationLanes.Length + pathElements.Length;
			}
			elementIndex++;
			break;
		}
		if (elementIndex - 1 < navigationLanes.Length + pathElements.Length)
		{
			float3 val9;
			if (elementIndex > num3)
			{
				GetPathElement(elementIndex - 1, navigationLanes, pathElements, out var pathElement2);
				AreaLane areaLane2 = areaLanes[pathElement2.m_Target];
				bool flag = pathElement2.m_TargetDelta.y > 0.5f;
				val9 = CalculateTriangleTarget(nodes[areaLane2.m_Nodes.y].m_Position, nodes[areaLane2.m_Nodes.z].m_Position, nodes[math.select(areaLane2.m_Nodes.x, areaLane2.m_Nodes.w, flag)].m_Position, lanePosition: math.select(lanePosition, 0f - lanePosition, pathElement2.m_TargetDelta.y < pathElement2.m_TargetDelta.x != isBackward), lastTarget: targetPosition, elementIndex: elementIndex, navigationLanes: navigationLanes, pathElements: pathElements, curveDelta: pathElement2.m_TargetDelta.y, navigationSize: navigationSize, isSingle: false, transforms: transforms, areaLanes: areaLanes, curves: curves);
			}
			else
			{
				val9 = CalculateTriangleTarget(left, right, next, targetPosition, elementIndex, navigationLanes, pathElements, lanePosition, curveDelta, navigationSize, isSingle: false, transforms, areaLanes, curves);
			}
			Segment val10 = default(Segment);
			((Segment)(ref val10))._002Ector(comparePosition, val9);
			float2 val11 = default(float2);
			if (MathUtils.Intersect(Line2.op_Implicit(((Segment)(ref val)).xz), Line2.op_Implicit(((Segment)(ref val10)).xz), ref val11))
			{
				float num6 = math.max(math.max(0f, 0.4f * math.min(val11.y, 1f - val11.y) * MathUtils.Length(((Segment)(ref val10)).xz) * num2), math.max(val11.x - val2.max, val2.min - val11.x));
				if (num6 < val2.max - val2.min)
				{
					((Bounds1)(ref val2))._002Ector(math.max(val2.min, math.min(val2.max, val11.x) - num6), math.min(val2.max, math.max(val2.min, val11.x) + num6));
				}
			}
		}
		float lanePosition2 = math.lerp(val2.min, val2.max, lanePosition + 0.5f);
		targetPosition = CalculateAreaTarget(prev2, prev, left, right, comparePosition, minDistance, lanePosition2, navigationSize, out var farEnough);
		if (!farEnough)
		{
			return math.distance(((float3)(ref comparePosition)).xz, ((float3)(ref targetPosition)).xz) >= minDistance;
		}
		return true;
	}

	private static float3 CalculateAreaTarget(float3 prev2, float3 prev, float3 left, float3 right, float3 comparePosition, float minDistance, float lanePosition, float navigationSize, out bool farEnough)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		float num = navigationSize * 0.5f;
		Segment val = default(Segment);
		((Segment)(ref val))._002Ector(left, right);
		val.a = MathUtils.Position(val, lanePosition);
		if (!((float3)(ref prev2)).Equals(prev))
		{
			Segment val2 = default(Segment);
			((Segment)(ref val2))._002Ector(prev2, prev);
			val.b = comparePosition;
			float2 val3 = default(float2);
			if (MathUtils.Intersect(((Segment)(ref val)).xz, ((Segment)(ref val2)).xz, ref val3) && math.min(val3.y, 1f - val3.y) >= num / MathUtils.Length(((Segment)(ref val2)).xz))
			{
				farEnough = false;
				return val.a;
			}
		}
		Triangle3 val4 = default(Triangle3);
		((Triangle3)(ref val4))._002Ector(prev, left, right);
		Segment val5 = ((Triangle3)(ref val4)).ba;
		float2 val7 = default(float2);
		float2 val6 = default(float2);
		val6.x = MathUtils.Distance(((Segment)(ref val5)).xz, ((float3)(ref comparePosition)).xz, ref val7.x);
		val5 = ((Triangle3)(ref val4)).ca;
		val6.y = MathUtils.Distance(((Segment)(ref val5)).xz, ((float3)(ref comparePosition)).xz, ref val7.y);
		val6 = ((!MathUtils.Intersect(((Triangle3)(ref val4)).xz, ((float3)(ref comparePosition)).xz)) ? math.select(new float2(val6.x, 0f - val6.y), new float2(0f - val6.x, val6.y), val6.x > val6.y) : (-val6));
		if (math.all(val6 <= 0f - num))
		{
			farEnough = false;
			return val.a;
		}
		if (val6.y <= 0f - num)
		{
			float2 val8 = math.normalizesafe(MathUtils.Right(((float3)(ref left)).xz - ((float3)(ref prev)).xz), default(float2)) * num;
			val.b = MathUtils.Position(((Triangle3)(ref val4)).ba, val7.x);
			ref float3 b = ref val.b;
			((float3)(ref b)).xz = ((float3)(ref b)).xz + math.select(val8, -val8, math.dot(val8, ((float3)(ref right)).xz - ((float3)(ref prev)).xz) < 0f);
		}
		else if (val6.x <= 0f - num)
		{
			float2 val9 = math.normalizesafe(MathUtils.Left(((float3)(ref right)).xz - ((float3)(ref prev)).xz), default(float2)) * num;
			val.b = MathUtils.Position(((Triangle3)(ref val4)).ca, val7.y);
			ref float3 b2 = ref val.b;
			((float3)(ref b2)).xz = ((float3)(ref b2)).xz + math.select(val9, -val9, math.dot(val9, ((float3)(ref left)).xz - ((float3)(ref prev)).xz) < 0f);
		}
		else
		{
			val.b = prev;
		}
		float num3 = default(float);
		float num2 = MathUtils.Distance(((Segment)(ref val)).xz, ((float3)(ref comparePosition)).xz, ref num3);
		num3 -= math.sqrt(math.max(0f, minDistance * minDistance - num2 * num2) / MathUtils.LengthSquared(((Segment)(ref val)).xz));
		if (num3 >= 0f)
		{
			farEnough = true;
			return MathUtils.Position(val, num3);
		}
		farEnough = false;
		return val.a;
	}

	public static void ClearNavigationForPathfind(Moving moving, CarData prefabCarData, ref CarCurrentLane currentLane, DynamicBuffer<CarNavigationLane> navigationLanes, ref ComponentLookup<Game.Net.CarLane> carLaneLookup, ref ComponentLookup<Curve> curveLookup)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		float num = 4f / 15f;
		float num2 = 1.0666667f + num;
		float num3 = math.min(math.length(moving.m_Velocity), prefabCarData.m_MaxSpeed);
		if (carLaneLookup.HasComponent(currentLane.m_Lane))
		{
			Curve curve = curveLookup[currentLane.m_Lane];
			bool flag = currentLane.m_CurvePosition.z < currentLane.m_CurvePosition.x;
			float num4 = math.max(0f, GetBrakingDistance(prefabCarData, num3, num) + num3 * num2 - 0.01f);
			float num5 = num4 / math.max(1E-06f, curve.m_Length) + 1E-06f;
			float num6 = currentLane.m_CurvePosition.x + math.select(num5, 0f - num5, flag);
			currentLane.m_LaneFlags |= CarLaneFlags.ClearedForPathfind;
			if (flag ? (currentLane.m_CurvePosition.z <= num6) : (num6 <= currentLane.m_CurvePosition.z))
			{
				currentLane.m_CurvePosition.z = num6;
				navigationLanes.Clear();
				return;
			}
			num4 -= curve.m_Length * math.abs(currentLane.m_CurvePosition.z - currentLane.m_CurvePosition.x);
			int num7 = 0;
			while (num7 < navigationLanes.Length && num4 > 0f)
			{
				ref CarNavigationLane reference = ref navigationLanes.ElementAt(num7);
				if (!carLaneLookup.HasComponent(reference.m_Lane))
				{
					break;
				}
				curve = curveLookup[reference.m_Lane];
				flag = reference.m_CurvePosition.y < reference.m_CurvePosition.x;
				num5 = num4 / math.max(1E-06f, curve.m_Length);
				num6 = reference.m_CurvePosition.x + math.select(num5, 0f - num5, flag);
				reference.m_Flags |= CarLaneFlags.ClearedForPathfind;
				num7++;
				if (flag ? (reference.m_CurvePosition.y <= num6) : (num6 <= reference.m_CurvePosition.y))
				{
					reference.m_CurvePosition.y = num6;
					break;
				}
				num4 -= curve.m_Length * math.abs(reference.m_CurvePosition.y - reference.m_CurvePosition.x);
			}
			if (num7 < navigationLanes.Length)
			{
				navigationLanes.RemoveRange(num7, navigationLanes.Length - num7);
			}
		}
		else
		{
			currentLane.m_CurvePosition.z = currentLane.m_CurvePosition.y;
		}
	}

	public static bool CanUseLane(PathMethod methods, RoadTypes roadTypes, CarLaneData carLaneData)
	{
		if ((roadTypes & carLaneData.m_RoadTypes) == 0)
		{
			return false;
		}
		if ((methods & PathMethod.MediumRoad) != 0)
		{
			return (int)carLaneData.m_MaxSize >= 1;
		}
		if ((methods & PathMethod.Road) != 0)
		{
			return (int)carLaneData.m_MaxSize >= 2;
		}
		return false;
	}

	public static PathMethod GetPathMethods(CarLaneData carLaneData)
	{
		return carLaneData.m_MaxSize switch
		{
			SizeClass.Medium => PathMethod.MediumRoad, 
			SizeClass.Large => PathMethod.Road, 
			_ => (PathMethod)0, 
		};
	}

	public static PathMethod GetPathMethods(CarData carData)
	{
		return carData.m_SizeClass switch
		{
			SizeClass.Small => PathMethod.Road | PathMethod.MediumRoad, 
			SizeClass.Medium => PathMethod.Road | PathMethod.MediumRoad, 
			SizeClass.Large => PathMethod.Road, 
			_ => (PathMethod)0, 
		};
	}

	public static PathMethod GetPathMethods(WatercraftData watercraftData)
	{
		return watercraftData.m_SizeClass switch
		{
			SizeClass.Small => PathMethod.Road | PathMethod.MediumRoad, 
			SizeClass.Medium => PathMethod.Road | PathMethod.MediumRoad, 
			SizeClass.Large => PathMethod.Road, 
			_ => (PathMethod)0, 
		};
	}

	public static PathMethod GetPathMethods(AircraftData aircraftData)
	{
		return aircraftData.m_SizeClass switch
		{
			SizeClass.Small => PathMethod.Road | PathMethod.Flying | PathMethod.MediumRoad, 
			SizeClass.Medium => PathMethod.Road | PathMethod.Flying | PathMethod.MediumRoad, 
			SizeClass.Large => PathMethod.Road | PathMethod.Flying, 
			_ => (PathMethod)0, 
		};
	}
}

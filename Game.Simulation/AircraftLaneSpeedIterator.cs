using System;
using Colossal.Mathematics;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct AircraftLaneSpeedIterator
{
	public ComponentLookup<Transform> m_TransformData;

	public ComponentLookup<Moving> m_MovingData;

	public ComponentLookup<Aircraft> m_AircraftData;

	public ComponentLookup<LaneReservation> m_LaneReservationData;

	public ComponentLookup<Curve> m_CurveData;

	public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

	public ComponentLookup<PrefabRef> m_PrefabRefData;

	public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

	public ComponentLookup<AircraftData> m_PrefabAircraftData;

	public BufferLookup<LaneOverlap> m_LaneOverlapData;

	public BufferLookup<LaneObject> m_LaneObjectData;

	public Entity m_Entity;

	public Entity m_Ignore;

	public int m_Priority;

	public float m_TimeStep;

	public float m_SafeTimeStep;

	public AircraftData m_PrefabAircraft;

	public ObjectGeometryData m_PrefabObjectGeometry;

	public Bounds1 m_SpeedRange;

	public float m_MaxSpeed;

	public float m_CanChangeLane;

	public float3 m_CurrentPosition;

	public float m_Distance;

	public Entity m_Blocker;

	public BlockerType m_BlockerType;

	private Entity m_Lane;

	private Curve m_Curve;

	private float2 m_CurveOffset;

	private float3 m_PrevPosition;

	private float m_PrevDistance;

	public bool IterateFirstLane(Entity lane, float3 curveOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		Curve curve = m_CurveData[lane];
		float3 val = MathUtils.Position(curve.m_Bezier, curveOffset.x);
		m_PrevPosition = m_CurrentPosition;
		m_Distance = math.distance(((float3)(ref m_CurrentPosition)).xz, ((float3)(ref val)).xz);
		if (m_CarLaneData.HasComponent(lane))
		{
			Game.Net.CarLane carLaneData = m_CarLaneData[lane];
			float num = VehicleUtils.GetMaxDriveSpeed(m_PrefabAircraft, carLaneData);
			if (m_Priority < 102 && m_LaneReservationData.HasComponent(lane) && m_LaneReservationData[lane].GetPriority() == 102)
			{
				num *= 0.5f;
			}
			if (num < m_MaxSpeed)
			{
				m_MaxSpeed = MathUtils.Clamp(num, m_SpeedRange);
				m_Blocker = Entity.Null;
				m_BlockerType = BlockerType.Limit;
			}
			float2 xy = ((float3)(ref curveOffset)).xy;
			float num2 = 0f - m_PrefabObjectGeometry.m_Bounds.max.z;
			float num3 = m_Distance + num2;
			m_Lane = lane;
			m_Curve = curve;
			m_CurveOffset = ((float3)(ref curveOffset)).xz;
			m_CurrentPosition = val;
			CheckCurrentLane(num3, xy);
			CheckOverlappingLanes(num3, xy.y);
		}
		float3 val2 = MathUtils.Position(curve.m_Bezier, curveOffset.z);
		float num4 = math.abs(curveOffset.z - curveOffset.x);
		float num5 = math.max(0.001f, math.lerp(math.distance(val, val2), curve.m_Length * num4, num4));
		if (num5 > 1f)
		{
			m_PrevPosition = m_CurrentPosition;
			m_PrevDistance = m_Distance;
		}
		m_CurrentPosition = val2;
		m_Distance += num5;
		float brakingDistance = VehicleUtils.GetBrakingDistance(m_PrefabAircraft, m_MaxSpeed, m_SafeTimeStep);
		return (m_Distance - 20f >= brakingDistance) | (m_MaxSpeed == m_SpeedRange.min);
	}

	public bool IterateNextLane(Entity lane, float2 curveOffset, float minOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (!m_CurveData.HasComponent(lane))
		{
			return false;
		}
		Curve curve = m_CurveData[lane];
		if (m_CarLaneData.HasComponent(lane))
		{
			Game.Net.CarLane carLaneData = m_CarLaneData[lane];
			float maxDriveSpeed = VehicleUtils.GetMaxDriveSpeed(m_PrefabAircraft, carLaneData);
			float num = 0f - m_PrefabObjectGeometry.m_Bounds.max.z;
			float num2 = m_Distance + num;
			float num3 = ((maxDriveSpeed != 0f) ? math.max(maxDriveSpeed, VehicleUtils.GetMaxBrakingSpeed(m_PrefabAircraft, m_Distance, maxDriveSpeed, m_TimeStep)) : VehicleUtils.GetMaxBrakingSpeed(m_PrefabAircraft, num2, m_SafeTimeStep));
			if (num3 < m_MaxSpeed)
			{
				m_MaxSpeed = MathUtils.Clamp(num3, m_SpeedRange);
				m_Blocker = Entity.Null;
				m_BlockerType = BlockerType.Limit;
			}
			m_Curve = curve;
			m_CurveOffset = curveOffset;
			m_Lane = lane;
			minOffset = math.select(minOffset, curveOffset.x, curveOffset.x > 0f);
			CheckCurrentLane(num2, float2.op_Implicit(minOffset));
			CheckOverlappingLanes(num2, minOffset);
		}
		float3 val = MathUtils.Position(curve.m_Bezier, curveOffset.y);
		float num4 = math.abs(curveOffset.y - curveOffset.x);
		float num5 = math.max(0.001f, math.lerp(math.distance(m_CurrentPosition, val), curve.m_Length * num4, num4));
		if (num5 > 1f)
		{
			m_PrevPosition = m_CurrentPosition;
			m_PrevDistance = m_Distance;
		}
		m_CurrentPosition = val;
		m_Distance += num5;
		float brakingDistance = VehicleUtils.GetBrakingDistance(m_PrefabAircraft, m_MaxSpeed, m_SafeTimeStep);
		return (m_Distance - 20f >= brakingDistance) | (m_MaxSpeed == m_SpeedRange.min);
	}

	public void IterateTarget(float3 targetPosition)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		float maxDriveSpeed = VehicleUtils.GetMaxDriveSpeed(m_PrefabAircraft, 11.111112f, (float)Math.PI / 12f);
		float maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabAircraft, m_Distance, maxDriveSpeed, m_TimeStep);
		m_Distance += math.distance(((float3)(ref m_CurrentPosition)).xz, ((float3)(ref targetPosition)).xz);
		maxBrakingSpeed = math.min(maxBrakingSpeed, VehicleUtils.GetMaxBrakingSpeed(m_PrefabAircraft, m_Distance, m_TimeStep));
		if (maxBrakingSpeed < m_MaxSpeed)
		{
			m_MaxSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
			m_Blocker = Entity.Null;
			m_BlockerType = BlockerType.None;
		}
	}

	private void CheckCurrentLane(float distance, float2 minOffset)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (!m_LaneObjectData.HasBuffer(m_Lane))
		{
			return;
		}
		DynamicBuffer<LaneObject> val = m_LaneObjectData[m_Lane];
		if (val.Length == 0)
		{
			return;
		}
		distance -= 1f;
		for (int i = 0; i < val.Length; i++)
		{
			LaneObject laneObject = val[i];
			if (!(laneObject.m_LaneObject == m_Entity) && !(laneObject.m_LaneObject == m_Ignore))
			{
				float2 curvePosition = laneObject.m_CurvePosition;
				if (!(curvePosition.y <= minOffset.y) || (!(curvePosition.y < 1f) && !(curvePosition.x <= minOffset.x)))
				{
					float objectSpeed = GetObjectSpeed(laneObject.m_LaneObject, curvePosition.x);
					UpdateMaxSpeed(laneObject.m_LaneObject, BlockerType.Continuing, objectSpeed, curvePosition.x, 1f, distance);
				}
			}
		}
	}

	private void CheckOverlappingLanes(float origDistance, float origMinOffset)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		if (!m_LaneOverlapData.HasBuffer(m_Lane))
		{
			return;
		}
		DynamicBuffer<LaneOverlap> val = m_LaneOverlapData[m_Lane];
		if (val.Length == 0)
		{
			return;
		}
		origDistance -= 1f;
		float2 curveOffset = m_CurveOffset;
		float length = m_Curve.m_Length;
		int priority = m_Priority;
		for (int i = 0; i < val.Length; i++)
		{
			LaneOverlap laneOverlap = val[i];
			float4 val2 = new float4((float)(int)laneOverlap.m_ThisStart, (float)(int)laneOverlap.m_ThisEnd, (float)(int)laneOverlap.m_OtherStart, (float)(int)laneOverlap.m_OtherEnd) * 0.003921569f;
			if (val2.y <= curveOffset.x)
			{
				continue;
			}
			m_Lane = laneOverlap.m_Other;
			m_Curve = m_CurveData[m_Lane];
			m_CurveOffset = ((float4)(ref val2)).zw;
			float num = math.max(0f, origMinOffset - val2.x) + val2.z;
			float num2 = origDistance + length * (val2.x - curveOffset.x);
			float distanceFactor = (float)(int)laneOverlap.m_Parallelism * (1f / 128f);
			int num3 = priority;
			BlockerType blockerType = (((laneOverlap.m_Flags & (OverlapFlags.MergeEnd | OverlapFlags.MergeMiddleEnd)) != 0) ? BlockerType.Continuing : BlockerType.Crossing);
			if ((laneOverlap.m_Flags & (OverlapFlags.MergeStart | OverlapFlags.MergeMiddleStart)) == 0 && val2.x > origMinOffset)
			{
				num3 -= laneOverlap.m_PriorityDelta;
				if (m_LaneReservationData.HasComponent(m_Lane))
				{
					LaneReservation laneReservation = m_LaneReservationData[m_Lane];
					float offset = laneReservation.GetOffset();
					int priority2 = laneReservation.GetPriority();
					if (offset > math.max(num, m_CurveOffset.x) || priority2 > num3)
					{
						float maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabAircraft, num2, m_SafeTimeStep);
						maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
						if (maxBrakingSpeed < m_MaxSpeed)
						{
							m_MaxSpeed = maxBrakingSpeed;
							m_Blocker = Entity.Null;
							m_BlockerType = blockerType;
						}
					}
				}
			}
			if (!m_LaneObjectData.HasBuffer(m_Lane))
			{
				continue;
			}
			DynamicBuffer<LaneObject> val3 = m_LaneObjectData[m_Lane];
			if (val3.Length == 0)
			{
				continue;
			}
			m_CurrentPosition = MathUtils.Position(m_Curve.m_Bezier, m_CurveOffset.x);
			for (int j = 0; j < val3.Length; j++)
			{
				LaneObject laneObject = val3[j];
				if (laneObject.m_LaneObject == m_Ignore)
				{
					continue;
				}
				float2 curvePosition = laneObject.m_CurvePosition;
				float objectSpeed = GetObjectSpeed(laneObject.m_LaneObject, curvePosition.x);
				if ((laneOverlap.m_Flags & (OverlapFlags.MergeStart | OverlapFlags.MergeMiddleStart)) == 0 && (val2.x >= origMinOffset || curvePosition.y > val2.z))
				{
					int num4;
					if (m_AircraftData.HasComponent(laneObject.m_LaneObject))
					{
						PrefabRef prefabRef = m_PrefabRefData[laneObject.m_LaneObject];
						num4 = VehicleUtils.GetPriority(m_PrefabAircraftData[prefabRef.m_Prefab]);
					}
					else
					{
						num4 = 0;
					}
					int num5 = num4 - num3;
					if (num5 > 0)
					{
						curvePosition.y += objectSpeed * 2f / math.max(1f, m_Curve.m_Length);
					}
					else if (num5 < 0)
					{
						curvePosition.y -= math.max(0f, val2.z - num);
					}
				}
				if (!(curvePosition.y <= num))
				{
					UpdateMaxSpeed(laneObject.m_LaneObject, blockerType, objectSpeed, curvePosition.x, distanceFactor, num2);
				}
			}
		}
	}

	private float GetObjectSpeed(Entity obj, float curveOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (!m_MovingData.HasComponent(obj))
		{
			return 0f;
		}
		Moving moving = m_MovingData[obj];
		float3 val = math.normalizesafe(MathUtils.Tangent(m_Curve.m_Bezier, curveOffset), default(float3));
		return math.dot(moving.m_Velocity, val);
	}

	private void UpdateMaxSpeed(Entity obj, BlockerType blockerType, float objectSpeed, float laneOffset, float distanceFactor, float distanceOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef prefabRef = m_PrefabRefData[obj];
		float num = 0f;
		if (m_PrefabObjectGeometryData.HasComponent(prefabRef.m_Prefab))
		{
			num = math.max(0f, 0f - m_PrefabObjectGeometryData[prefabRef.m_Prefab].m_Bounds.min.z);
		}
		if ((laneOffset - m_CurveOffset.y) * m_Curve.m_Length >= num)
		{
			return;
		}
		Transform transform = m_TransformData[obj];
		float3 val = MathUtils.Position(m_Curve.m_Bezier, math.max(m_CurveOffset.x, laneOffset));
		float num2 = math.distance(((float3)(ref val)).xz, ((float3)(ref m_CurrentPosition)).xz);
		num2 -= math.max(0f, m_CurveOffset.x - laneOffset) * m_Curve.m_Length;
		num2 = ((!(math.dot(((float3)(ref transform.m_Position)).xz - ((float3)(ref m_CurrentPosition)).xz, ((float3)(ref m_CurrentPosition)).xz - ((float3)(ref m_PrevPosition)).xz) < 0f)) ? math.min(num2, math.distance(((float3)(ref transform.m_Position)).xz, ((float3)(ref m_CurrentPosition)).xz)) : math.min(num2, math.distance(((float3)(ref transform.m_Position)).xz, ((float3)(ref m_PrevPosition)).xz) + m_PrevDistance - m_Distance));
		num2 -= num;
		num2 *= distanceFactor;
		num2 += distanceOffset;
		float maxBrakingSpeed;
		if (objectSpeed > 0.001f && m_PrefabAircraftData.HasComponent(prefabRef.m_Prefab))
		{
			AircraftData prefabAircraftData = m_PrefabAircraftData[prefabRef.m_Prefab];
			objectSpeed = math.max(0f, objectSpeed - prefabAircraftData.m_GroundBraking * m_TimeStep * 2f) * distanceFactor;
			if (m_PrefabAircraft.m_GroundBraking >= prefabAircraftData.m_GroundBraking)
			{
				num2 += objectSpeed * m_SafeTimeStep;
				maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabAircraft, num2, objectSpeed, m_SafeTimeStep);
			}
			else
			{
				num2 += VehicleUtils.GetBrakingDistance(prefabAircraftData, objectSpeed, m_SafeTimeStep);
				maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabAircraft, num2, m_SafeTimeStep);
			}
		}
		else
		{
			maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabAircraft, num2, m_SafeTimeStep);
		}
		maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
		if (maxBrakingSpeed < m_MaxSpeed)
		{
			m_MaxSpeed = maxBrakingSpeed;
			m_Blocker = obj;
			m_BlockerType = blockerType;
		}
	}
}

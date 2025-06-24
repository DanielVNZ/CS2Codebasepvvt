using System;
using Colossal.Mathematics;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct WatercraftLaneSpeedIterator
{
	public ComponentLookup<Transform> m_TransformData;

	public ComponentLookup<Moving> m_MovingData;

	public ComponentLookup<Watercraft> m_WatercraftData;

	public ComponentLookup<LaneReservation> m_LaneReservationData;

	public ComponentLookup<LaneSignal> m_LaneSignalData;

	public ComponentLookup<Curve> m_CurveData;

	public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

	public ComponentLookup<PrefabRef> m_PrefabRefData;

	public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

	public ComponentLookup<WatercraftData> m_PrefabWatercraftData;

	public BufferLookup<LaneOverlap> m_LaneOverlapData;

	public BufferLookup<LaneObject> m_LaneObjectData;

	public Entity m_Entity;

	public Entity m_Ignore;

	public int m_Priority;

	public float m_TimeStep;

	public float m_SafeTimeStep;

	public float m_SpeedLimitFactor;

	public float m_CurrentSpeed;

	public WatercraftData m_PrefabWatercraft;

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
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		Curve curve = m_CurveData[lane];
		float3 val = MathUtils.Position(curve.m_Bezier, curveOffset.x);
		m_PrevPosition = m_CurrentPosition;
		m_Distance = math.distance(((float3)(ref m_CurrentPosition)).xz, ((float3)(ref val)).xz);
		if (m_CarLaneData.HasComponent(lane))
		{
			Game.Net.CarLane carLaneData = m_CarLaneData[lane];
			carLaneData.m_SpeedLimit *= m_SpeedLimitFactor;
			float num = VehicleUtils.GetMaxDriveSpeed(m_PrefabWatercraft, carLaneData);
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
		float brakingDistance = VehicleUtils.GetBrakingDistance(m_PrefabWatercraft, m_MaxSpeed, m_SafeTimeStep);
		return (m_Distance - 150f >= brakingDistance) | (m_MaxSpeed == m_SpeedRange.min);
	}

	public bool IterateFirstLane(Entity lane1, Entity lane2, float3 curveOffset, float laneDelta)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		laneDelta = math.saturate(laneDelta);
		Curve curve = m_CurveData[lane1];
		Curve curve2 = m_CurveData[lane2];
		float3 val = MathUtils.Position(curve.m_Bezier, curveOffset.x);
		float3 val2 = MathUtils.Position(curve2.m_Bezier, curveOffset.x);
		float3 val3 = math.lerp(val, val2, laneDelta);
		m_PrevPosition = m_CurrentPosition;
		m_Distance = math.distance(((float3)(ref m_CurrentPosition)).xz, ((float3)(ref val3)).xz);
		if (m_CarLaneData.HasComponent(lane1))
		{
			Game.Net.CarLane carLaneData = m_CarLaneData[lane1];
			carLaneData.m_SpeedLimit *= m_SpeedLimitFactor;
			float num = VehicleUtils.GetMaxDriveSpeed(m_PrefabWatercraft, carLaneData);
			if (m_Priority < 102 && m_LaneReservationData.HasComponent(lane1) && m_LaneReservationData.HasComponent(lane2))
			{
				if (laneDelta < 0.9f)
				{
					LaneReservation laneReservation = m_LaneReservationData[lane1];
					_ = m_LaneReservationData[lane2];
					if (math.any(new int2(laneReservation.GetPriority() == 102)))
					{
						num *= 0.5f;
					}
				}
				else if (m_LaneReservationData[lane2].GetPriority() == 102)
				{
					num *= 0.5f;
				}
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
			if (laneDelta < 0.9f)
			{
				m_Lane = lane1;
				m_Curve = curve;
				m_CurveOffset = ((float3)(ref curveOffset)).xz;
				m_CurrentPosition = val;
				CheckCurrentLane(num3, xy);
				CheckOverlappingLanes(num3, xy.y);
			}
			m_Lane = lane2;
			m_Curve = curve2;
			m_CurveOffset = ((float3)(ref curveOffset)).xz;
			m_CurrentPosition = val2;
			if (laneDelta == 0f)
			{
				CheckCurrentLane(num3, xy, ref m_CanChangeLane);
			}
			else
			{
				CheckCurrentLane(num3, xy);
			}
			CheckOverlappingLanes(num3, xy.y);
		}
		float3 val4 = MathUtils.Position(curve2.m_Bezier, curveOffset.z);
		float num4 = math.lerp(curve.m_Length, curve2.m_Length, laneDelta);
		float num5 = math.abs(curveOffset.z - curveOffset.x);
		float num6 = math.max(0.001f, math.lerp(math.distance(val3, val4), num4 * num5, num5));
		if (num6 > 1f)
		{
			m_PrevPosition = m_CurrentPosition;
			m_PrevDistance = m_Distance;
		}
		m_CurrentPosition = val4;
		m_Distance += num6;
		float brakingDistance = VehicleUtils.GetBrakingDistance(m_PrefabWatercraft, m_MaxSpeed, m_SafeTimeStep);
		return (m_Distance - 150f >= brakingDistance) | (m_MaxSpeed == m_SpeedRange.min);
	}

	public bool IterateNextLane(Entity lane, float2 curveOffset, float minOffset, out bool needSignal)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		needSignal = false;
		Curve curve = default(Curve);
		if (!m_CurveData.TryGetComponent(lane, ref curve))
		{
			return false;
		}
		Game.Net.CarLane carLaneData = default(Game.Net.CarLane);
		if (m_CarLaneData.TryGetComponent(lane, ref carLaneData))
		{
			carLaneData.m_SpeedLimit *= m_SpeedLimitFactor;
			float num = VehicleUtils.GetMaxDriveSpeed(m_PrefabWatercraft, carLaneData);
			float num2 = 0f - m_PrefabObjectGeometry.m_Bounds.max.z;
			float num3 = m_Distance + num2;
			Entity blocker = Entity.Null;
			BlockerType blockerType = BlockerType.Limit;
			LaneSignal laneSignal = default(LaneSignal);
			if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.Approach) == 0 && (carLaneData.m_Flags & Game.Net.CarLaneFlags.LevelCrossing) != 0 && m_LaneSignalData.TryGetComponent(lane, ref laneSignal))
			{
				float brakingDistance = VehicleUtils.GetBrakingDistance(m_PrefabWatercraft, m_CurrentSpeed, 0f);
				needSignal = true;
				switch (laneSignal.m_Signal)
				{
				case LaneSignalType.Stop:
					if ((m_Priority < 108 || (laneSignal.m_Flags & LaneSignalFlags.Physical) != 0) && brakingDistance <= num3 + 1f)
					{
						num = 0f;
						blocker = laneSignal.m_Blocker;
						blockerType = BlockerType.Signal;
					}
					break;
				case LaneSignalType.SafeStop:
					if ((m_Priority < 108 || (laneSignal.m_Flags & LaneSignalFlags.Physical) != 0) && brakingDistance <= num3)
					{
						num = 0f;
						blocker = laneSignal.m_Blocker;
						blockerType = BlockerType.Signal;
					}
					break;
				}
			}
			float num4 = ((num != 0f) ? math.max(num, VehicleUtils.GetMaxBrakingSpeed(m_PrefabWatercraft, m_Distance, num, m_TimeStep)) : VehicleUtils.GetMaxBrakingSpeed(m_PrefabWatercraft, num3, m_SafeTimeStep));
			if (num4 < m_MaxSpeed)
			{
				m_MaxSpeed = MathUtils.Clamp(num4, m_SpeedRange);
				m_Blocker = blocker;
				m_BlockerType = blockerType;
			}
			m_Curve = curve;
			m_CurveOffset = curveOffset;
			m_Lane = lane;
			minOffset = math.select(minOffset, curveOffset.x, curveOffset.x > 0f);
			CheckCurrentLane(num3, float2.op_Implicit(minOffset));
			CheckOverlappingLanes(num3, minOffset);
		}
		float3 val = MathUtils.Position(curve.m_Bezier, curveOffset.y);
		float num5 = math.abs(curveOffset.y - curveOffset.x);
		float num6 = math.max(0.001f, math.lerp(math.distance(m_CurrentPosition, val), curve.m_Length * num5, num5));
		if (num6 > 1f)
		{
			m_PrevPosition = m_CurrentPosition;
			m_PrevDistance = m_Distance;
		}
		m_CurrentPosition = val;
		m_Distance += num6;
		float brakingDistance2 = VehicleUtils.GetBrakingDistance(m_PrefabWatercraft, m_MaxSpeed, m_SafeTimeStep);
		return (m_Distance - 150f >= brakingDistance2) | (m_MaxSpeed == m_SpeedRange.min);
	}

	public void IterateTarget(float3 targetPosition)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		float maxDriveSpeed = VehicleUtils.GetMaxDriveSpeed(m_PrefabWatercraft, 11.111112f, (float)Math.PI / 12f);
		IterateTarget(targetPosition, maxDriveSpeed);
	}

	public void IterateTarget(float3 targetPosition, float maxLaneSpeed)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		float maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabWatercraft, m_Distance, maxLaneSpeed, m_TimeStep);
		m_Distance += math.distance(((float3)(ref m_CurrentPosition)).xz, ((float3)(ref targetPosition)).xz);
		maxBrakingSpeed = math.min(maxBrakingSpeed, VehicleUtils.GetMaxBrakingSpeed(m_PrefabWatercraft, m_Distance, m_TimeStep));
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
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
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
			if (!(laneObject.m_LaneObject == m_Entity))
			{
				float2 curvePosition = laneObject.m_CurvePosition;
				if (!(curvePosition.y <= minOffset.y) || (!(curvePosition.y < 1f) && !(curvePosition.x <= minOffset.x)))
				{
					float objectSpeed = GetObjectSpeed(laneObject.m_LaneObject, curvePosition.x);
					UpdateMaxSpeed(laneObject.m_LaneObject, BlockerType.Continuing, objectSpeed, curvePosition.x, 1f, distance, laneObject.m_LaneObject == m_Ignore);
				}
			}
		}
	}

	private void CheckCurrentLane(float distance, float2 minOffset, ref float canUseLane)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
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
			if (laneObject.m_LaneObject == m_Entity)
			{
				continue;
			}
			float2 curvePosition = laneObject.m_CurvePosition;
			if (curvePosition.y <= minOffset.y && (curvePosition.y < 1f || curvePosition.x <= minOffset.x))
			{
				PrefabRef prefabRef = m_PrefabRefData[laneObject.m_LaneObject];
				float num = 0f;
				if (m_PrefabObjectGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					num = 0f - m_PrefabObjectGeometryData[prefabRef.m_Prefab].m_Bounds.max.z;
				}
				if ((curvePosition.x - minOffset.x) * m_Curve.m_Length > num)
				{
					canUseLane = 0f;
				}
			}
			else
			{
				float objectSpeed = GetObjectSpeed(laneObject.m_LaneObject, curvePosition.x);
				UpdateMaxSpeed(laneObject.m_LaneObject, BlockerType.Continuing, objectSpeed, curvePosition.x, 1f, distance, laneObject.m_LaneObject == m_Ignore);
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
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
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
						float maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabWatercraft, num2, m_SafeTimeStep);
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
				float2 curvePosition = laneObject.m_CurvePosition;
				float objectSpeed = GetObjectSpeed(laneObject.m_LaneObject, curvePosition.x);
				if ((laneOverlap.m_Flags & (OverlapFlags.MergeStart | OverlapFlags.MergeMiddleStart)) == 0 && (val2.x >= origMinOffset || curvePosition.y > val2.z))
				{
					int num4;
					if (m_WatercraftData.HasComponent(laneObject.m_LaneObject))
					{
						PrefabRef prefabRef = m_PrefabRefData[laneObject.m_LaneObject];
						num4 = VehicleUtils.GetPriority(m_PrefabWatercraftData[prefabRef.m_Prefab]);
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
					UpdateMaxSpeed(laneObject.m_LaneObject, blockerType, objectSpeed, curvePosition.x, distanceFactor, num2, laneObject.m_LaneObject == m_Ignore);
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

	private void UpdateMaxSpeed(Entity obj, BlockerType blockerType, float objectSpeed, float laneOffset, float distanceFactor, float distanceOffset, bool ignore)
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
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
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
		if (objectSpeed > 0.001f && m_PrefabWatercraftData.HasComponent(prefabRef.m_Prefab))
		{
			WatercraftData prefabWatercraftData = m_PrefabWatercraftData[prefabRef.m_Prefab];
			objectSpeed = math.max(0f, objectSpeed - prefabWatercraftData.m_Braking * m_TimeStep * 2f) * distanceFactor;
			if (m_PrefabWatercraft.m_Braking >= prefabWatercraftData.m_Braking)
			{
				num2 += objectSpeed * m_SafeTimeStep;
				maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabWatercraft, num2, objectSpeed, m_SafeTimeStep);
			}
			else
			{
				num2 += VehicleUtils.GetBrakingDistance(prefabWatercraftData, objectSpeed, m_SafeTimeStep);
				maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabWatercraft, num2, m_SafeTimeStep);
			}
		}
		else
		{
			maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabWatercraft, num2, m_SafeTimeStep);
		}
		maxBrakingSpeed = math.select(maxBrakingSpeed, 1f, ignore && maxBrakingSpeed < 1f);
		maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
		if (maxBrakingSpeed < m_MaxSpeed)
		{
			m_MaxSpeed = maxBrakingSpeed;
			m_Blocker = obj;
			m_BlockerType = blockerType;
		}
	}
}

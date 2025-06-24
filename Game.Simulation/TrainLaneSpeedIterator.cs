using Colossal.Mathematics;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct TrainLaneSpeedIterator
{
	public ComponentLookup<Transform> m_TransformData;

	public ComponentLookup<Moving> m_MovingData;

	public ComponentLookup<Car> m_CarData;

	public ComponentLookup<Train> m_TrainData;

	public ComponentLookup<Curve> m_CurveData;

	public ComponentLookup<Game.Net.TrackLane> m_TrackLaneData;

	public ComponentLookup<Controller> m_ControllerData;

	public ComponentLookup<LaneReservation> m_LaneReservationData;

	public ComponentLookup<LaneSignal> m_LaneSignalData;

	public ComponentLookup<Creature> m_CreatureData;

	public ComponentLookup<PrefabRef> m_PrefabRefData;

	public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

	public ComponentLookup<CarData> m_PrefabCarData;

	public ComponentLookup<TrainData> m_PrefabTrainData;

	public BufferLookup<LaneOverlap> m_LaneOverlapData;

	public BufferLookup<LaneObject> m_LaneObjectData;

	public Entity m_Controller;

	public int m_Priority;

	public float m_TimeStep;

	public float m_SafeTimeStep;

	public float m_CurrentSpeed;

	public TrainData m_PrefabTrain;

	public ObjectGeometryData m_PrefabObjectGeometry;

	public Bounds1 m_SpeedRange;

	public float3 m_RearPosition;

	public bool m_PushBlockers;

	public float m_MaxSpeed;

	public float3 m_CurrentPosition;

	public float m_Distance;

	public Entity m_Blocker;

	public BlockerType m_BlockerType;

	private Entity m_Lane;

	private Curve m_Curve;

	private float2 m_CurveOffset;

	private float3 m_PrevPosition;

	private float m_PrevDistance;

	public bool IterateFirstLane(Entity lane, float4 curveOffset, bool exclusive, bool ignoreObstacles, bool skipCurrent, out bool needSignal)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		Curve curve = m_CurveData[lane];
		needSignal = false;
		float3 val = MathUtils.Position(curve.m_Bezier, curveOffset.y);
		m_PrevPosition = m_CurrentPosition;
		m_PrevDistance = 0f - (m_PrefabTrain.m_AttachOffsets.x - m_PrefabTrain.m_BogieOffsets.x);
		m_Distance = math.distance(m_CurrentPosition, val);
		m_Distance = math.min(m_Distance, math.distance(m_RearPosition, val) - math.max(1f, math.csum(m_PrefabTrain.m_BogieOffsets)));
		m_Distance -= m_PrefabTrain.m_AttachOffsets.x - m_PrefabTrain.m_BogieOffsets.x;
		Game.Net.TrackLane trackLaneData = default(Game.Net.TrackLane);
		if (m_TrackLaneData.TryGetComponent(lane, ref trackLaneData))
		{
			int yieldOverride = 0;
			LaneSignal laneSignal = default(LaneSignal);
			if (m_LaneSignalData.TryGetComponent(lane, ref laneSignal))
			{
				switch (laneSignal.m_Signal)
				{
				case LaneSignalType.Stop:
					yieldOverride = -1;
					break;
				case LaneSignalType.Yield:
					yieldOverride = 1;
					break;
				}
				if (lane != m_Lane)
				{
					needSignal = true;
				}
			}
			m_Lane = lane;
			m_Curve = curve;
			m_CurveOffset = ((float4)(ref curveOffset)).yw;
			m_CurrentPosition = val;
			float num = VehicleUtils.GetMaxDriveSpeed(m_PrefabTrain, trackLaneData);
			LaneReservation laneReservation = default(LaneReservation);
			if (!exclusive && m_LaneReservationData.TryGetComponent(lane, ref laneReservation) && laneReservation.GetPriority() == 102)
			{
				num *= 0.5f;
			}
			if (num < m_MaxSpeed)
			{
				m_MaxSpeed = MathUtils.Clamp(num, m_SpeedRange);
				m_Blocker = Entity.Null;
				m_BlockerType = BlockerType.Limit;
			}
			if (!ignoreObstacles)
			{
				if (!exclusive && !skipCurrent)
				{
					CheckCurrentLane(m_Distance, ((float4)(ref curveOffset)).yz, exclusive);
				}
				CheckOverlappingLanes(m_Distance, curveOffset.z, yieldOverride, exclusive);
			}
		}
		float3 val2 = MathUtils.Position(curve.m_Bezier, curveOffset.w);
		float num2 = math.abs(curveOffset.w - curveOffset.y);
		float num3 = math.max(0.001f, math.lerp(math.distance(val, val2), curve.m_Length * num2, num2));
		if (num3 > 1f)
		{
			m_PrevPosition = m_CurrentPosition;
			m_PrevDistance = m_Distance;
		}
		m_CurrentPosition = val2;
		m_Distance += num3;
		float brakingDistance = VehicleUtils.GetBrakingDistance(m_PrefabTrain, m_MaxSpeed, m_SafeTimeStep);
		brakingDistance += VehicleUtils.GetSignalDistance(m_PrefabTrain, m_MaxSpeed);
		return (m_Distance - 10f >= brakingDistance) | (m_MaxSpeed == m_SpeedRange.min);
	}

	public void IteratePrevLane(Entity lane, out bool needSignal)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		needSignal = false;
		Game.Net.TrackLane trackLaneData = default(Game.Net.TrackLane);
		if (lane != m_Lane && m_TrackLaneData.TryGetComponent(lane, ref trackLaneData))
		{
			if (m_LaneSignalData.HasComponent(lane))
			{
				needSignal = true;
			}
			m_Lane = lane;
			float maxDriveSpeed = VehicleUtils.GetMaxDriveSpeed(m_PrefabTrain, trackLaneData);
			if (maxDriveSpeed < m_MaxSpeed)
			{
				m_MaxSpeed = MathUtils.Clamp(maxDriveSpeed, m_SpeedRange);
				m_Blocker = Entity.Null;
				m_BlockerType = BlockerType.Limit;
			}
		}
	}

	public bool IterateNextLane(Entity lane, float2 curveOffset, float minOffset, bool exclusive, bool ignoreObstacles, out bool needSignal)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		needSignal = false;
		Curve curve = default(Curve);
		if (!m_CurveData.TryGetComponent(lane, ref curve))
		{
			return false;
		}
		Game.Net.TrackLane trackLaneData = default(Game.Net.TrackLane);
		if (m_TrackLaneData.TryGetComponent(lane, ref trackLaneData))
		{
			float num = VehicleUtils.GetMaxDriveSpeed(m_PrefabTrain, trackLaneData);
			int yieldOverride = 0;
			Entity blocker = Entity.Null;
			BlockerType blockerType = BlockerType.Limit;
			LaneSignal laneSignal = default(LaneSignal);
			if (m_LaneSignalData.TryGetComponent(lane, ref laneSignal))
			{
				needSignal = true;
				switch (laneSignal.m_Signal)
				{
				case LaneSignalType.Stop:
					if ((m_Priority < 108 || (laneSignal.m_Flags & LaneSignalFlags.Physical) != 0) && VehicleUtils.GetBrakingDistance(m_PrefabTrain, m_CurrentSpeed, 0f) <= m_Distance + 1f)
					{
						num = 0f;
						blocker = laneSignal.m_Blocker;
						blockerType = BlockerType.Signal;
						yieldOverride = 1;
					}
					else
					{
						yieldOverride = -1;
					}
					break;
				case LaneSignalType.SafeStop:
					if ((m_Priority < 108 || (laneSignal.m_Flags & LaneSignalFlags.Physical) != 0) && VehicleUtils.GetBrakingDistance(m_PrefabTrain, m_CurrentSpeed, 0f) <= m_Distance)
					{
						num = 0f;
						blocker = laneSignal.m_Blocker;
						blockerType = BlockerType.Signal;
					}
					break;
				case LaneSignalType.Yield:
					yieldOverride = 1;
					break;
				}
			}
			float num3;
			if (num == 0f)
			{
				float num2 = m_Distance - math.select(10f, 0.5f, (m_PrefabTrain.m_TrackType & TrackTypes.Tram) != 0);
				num3 = VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, math.max(0f, num2), m_SafeTimeStep);
			}
			else
			{
				num3 = math.max(num, VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, m_Distance, num, m_TimeStep));
			}
			if (num3 < m_MaxSpeed)
			{
				m_MaxSpeed = MathUtils.Clamp(num3, m_SpeedRange);
				m_Blocker = blocker;
				m_BlockerType = blockerType;
			}
			if (!ignoreObstacles)
			{
				m_Lane = lane;
				m_Curve = curve;
				m_CurveOffset = curveOffset;
				CheckCurrentLane(m_Distance, float2.op_Implicit(minOffset), exclusive);
				CheckOverlappingLanes(m_Distance, minOffset, yieldOverride, exclusive);
			}
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
		float brakingDistance = VehicleUtils.GetBrakingDistance(m_PrefabTrain, m_MaxSpeed, m_SafeTimeStep);
		brakingDistance += VehicleUtils.GetSignalDistance(m_PrefabTrain, m_MaxSpeed);
		return (m_Distance - 10f >= brakingDistance) | (m_MaxSpeed == m_SpeedRange.min);
	}

	public bool IterateTarget(Entity lane, bool ignoreObstacles)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		float maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, m_Distance, m_TimeStep);
		if (maxBrakingSpeed < m_MaxSpeed)
		{
			m_MaxSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
			LaneReservation laneReservation = default(LaneReservation);
			if (m_LaneReservationData.TryGetComponent(lane, ref laneReservation))
			{
				Controller controller = default(Controller);
				if (ignoreObstacles || (m_ControllerData.TryGetComponent(laneReservation.m_Blocker, ref controller) && controller.m_Controller == m_Controller))
				{
					m_Blocker = Entity.Null;
					m_BlockerType = BlockerType.None;
				}
				else
				{
					m_Blocker = laneReservation.m_Blocker;
					m_BlockerType = BlockerType.Continuing;
				}
			}
			else
			{
				m_Blocker = Entity.Null;
				m_BlockerType = BlockerType.None;
			}
			return true;
		}
		return false;
	}

	public bool IterateTarget()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		float maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, m_Distance, m_TimeStep);
		if (maxBrakingSpeed < m_MaxSpeed)
		{
			m_MaxSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
			m_Blocker = Entity.Null;
			m_BlockerType = BlockerType.None;
			return true;
		}
		return false;
	}

	private void CheckCurrentLane(float distance, float2 minOffset, bool exclusive)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
		if (!m_LaneObjectData.TryGetBuffer(m_Lane, ref val) || val.Length == 0)
		{
			return;
		}
		distance = ((!exclusive) ? (distance - 1f) : (distance - 10f));
		Controller controller = default(Controller);
		for (int i = 0; i < val.Length; i++)
		{
			LaneObject laneObject = val[i];
			if (laneObject.m_LaneObject == m_Controller || (m_ControllerData.TryGetComponent(laneObject.m_LaneObject, ref controller) && controller.m_Controller == m_Controller))
			{
				continue;
			}
			if (exclusive)
			{
				float maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, distance, m_SafeTimeStep);
				maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
				if (maxBrakingSpeed < m_MaxSpeed)
				{
					m_MaxSpeed = maxBrakingSpeed;
					m_Blocker = laneObject.m_LaneObject;
					m_BlockerType = BlockerType.Continuing;
				}
			}
			else
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

	private void CheckOverlappingLanes(float origDistance, float origMinOffset, int yieldOverride, bool exclusive)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<LaneOverlap> val = default(DynamicBuffer<LaneOverlap>);
		if (!m_LaneOverlapData.TryGetBuffer(m_Lane, ref val) || val.Length == 0)
		{
			return;
		}
		float distance = origDistance - 10f;
		origDistance -= 1f;
		Bezier4x3 bezier = m_Curve.m_Bezier;
		float2 curveOffset = m_CurveOffset;
		float length = m_Curve.m_Length;
		int num = m_Priority;
		LaneReservation laneReservation = default(LaneReservation);
		if (m_LaneReservationData.TryGetComponent(m_Lane, ref laneReservation))
		{
			int priority = laneReservation.GetPriority();
			num = math.select(num, 106, priority >= 108 && 106 > num);
		}
		Game.Net.TrackLane trackLane = default(Game.Net.TrackLane);
		DynamicBuffer<LaneObject> val3 = default(DynamicBuffer<LaneObject>);
		Controller controller = default(Controller);
		Controller controller2 = default(Controller);
		Car carData = default(Car);
		for (int i = 0; i < val.Length; i++)
		{
			LaneOverlap laneOverlap = val[i];
			float4 val2 = new float4((float)(int)laneOverlap.m_ThisStart, (float)(int)laneOverlap.m_ThisEnd, (float)(int)laneOverlap.m_OtherStart, (float)(int)laneOverlap.m_OtherEnd) * 0.003921569f;
			if (val2.y <= curveOffset.x)
			{
				continue;
			}
			BlockerType blockerType = (((laneOverlap.m_Flags & (OverlapFlags.MergeEnd | OverlapFlags.MergeMiddleEnd)) != 0) ? BlockerType.Continuing : BlockerType.Crossing);
			if (exclusive && m_TrackLaneData.TryGetComponent(laneOverlap.m_Other, ref trackLane) && (trackLane.m_Flags & TrackLaneFlags.Exclusive) != 0)
			{
				if (m_LaneReservationData.TryGetComponent(laneOverlap.m_Other, ref laneReservation) && laneReservation.GetPriority() >= m_Priority)
				{
					float maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, distance, m_SafeTimeStep);
					maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
					if (maxBrakingSpeed < m_MaxSpeed)
					{
						m_MaxSpeed = maxBrakingSpeed;
						m_Blocker = laneReservation.m_Blocker;
						m_BlockerType = blockerType;
					}
				}
				if (!m_LaneObjectData.TryGetBuffer(laneOverlap.m_Other, ref val3) || val3.Length == 0)
				{
					continue;
				}
				for (int j = 0; j < val3.Length; j++)
				{
					LaneObject laneObject = val3[j];
					if (!(laneObject.m_LaneObject == m_Controller) && (!m_ControllerData.TryGetComponent(laneObject.m_LaneObject, ref controller) || !(controller.m_Controller == m_Controller)))
					{
						float maxBrakingSpeed2 = VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, distance, m_SafeTimeStep);
						maxBrakingSpeed2 = MathUtils.Clamp(maxBrakingSpeed2, m_SpeedRange);
						if (maxBrakingSpeed2 < m_MaxSpeed)
						{
							m_MaxSpeed = maxBrakingSpeed2;
							m_Blocker = laneObject.m_LaneObject;
							m_BlockerType = blockerType;
						}
					}
				}
				continue;
			}
			m_Lane = laneOverlap.m_Other;
			m_Curve = m_CurveData[m_Lane];
			m_CurveOffset = ((float4)(ref val2)).zw;
			Segment overlapLine = MathUtils.Line(bezier, ((float4)(ref val2)).xy);
			float num2 = math.max(0f, origMinOffset - val2.x) + val2.z;
			float num3 = origDistance + length * (val2.x - curveOffset.x);
			float distanceFactor = (float)(int)laneOverlap.m_Parallelism * (1f / 128f);
			int num4 = num;
			if ((laneOverlap.m_Flags & (OverlapFlags.MergeStart | OverlapFlags.MergeMiddleStart)) == 0 && val2.x > origMinOffset)
			{
				int num5 = yieldOverride;
				if (m_LaneSignalData.HasComponent(m_Lane))
				{
					switch (m_LaneSignalData[m_Lane].m_Signal)
					{
					case LaneSignalType.Stop:
						num5++;
						break;
					case LaneSignalType.Yield:
						num5--;
						break;
					}
				}
				int num6 = math.select((int)laneOverlap.m_PriorityDelta, num5, num5 != 0);
				num4 -= num6;
				if (m_LaneReservationData.TryGetComponent(m_Lane, ref laneReservation))
				{
					float offset = laneReservation.GetOffset();
					int priority2 = laneReservation.GetPriority();
					if (offset > math.max(num2, m_CurveOffset.x) || priority2 > num4)
					{
						float maxBrakingSpeed3 = VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, num3, m_SafeTimeStep);
						maxBrakingSpeed3 = MathUtils.Clamp(maxBrakingSpeed3, m_SpeedRange);
						if (maxBrakingSpeed3 < m_MaxSpeed)
						{
							m_MaxSpeed = maxBrakingSpeed3;
							m_Blocker = laneReservation.m_Blocker;
							m_BlockerType = blockerType;
						}
					}
				}
			}
			if (!m_LaneObjectData.TryGetBuffer(m_Lane, ref val3) || val3.Length == 0)
			{
				continue;
			}
			m_CurrentPosition = MathUtils.Position(m_Curve.m_Bezier, m_CurveOffset.x);
			for (int k = 0; k < val3.Length; k++)
			{
				LaneObject laneObject2 = val3[k];
				if (laneObject2.m_LaneObject == m_Controller)
				{
					continue;
				}
				Entity val4 = laneObject2.m_LaneObject;
				if (m_ControllerData.TryGetComponent(laneObject2.m_LaneObject, ref controller2))
				{
					if (controller2.m_Controller == m_Controller)
					{
						continue;
					}
					val4 = controller2.m_Controller;
				}
				if (m_CreatureData.HasComponent(laneObject2.m_LaneObject))
				{
					CheckPedestrian(overlapLine, laneObject2.m_LaneObject, laneObject2.m_CurvePosition.y, num3, giveSpace: false);
					continue;
				}
				float2 curvePosition = laneObject2.m_CurvePosition;
				float objectSpeed = GetObjectSpeed(laneObject2.m_LaneObject, curvePosition.x);
				if ((laneOverlap.m_Flags & (OverlapFlags.MergeStart | OverlapFlags.MergeMiddleStart)) == 0 && (val2.x >= origMinOffset || curvePosition.y > val2.z))
				{
					int num7;
					if (m_CarData.TryGetComponent(val4, ref carData))
					{
						num7 = VehicleUtils.GetPriority(carData);
					}
					else if (m_TrainData.HasComponent(laneObject2.m_LaneObject))
					{
						PrefabRef prefabRef = m_PrefabRefData[laneObject2.m_LaneObject];
						num7 = VehicleUtils.GetPriority(m_PrefabTrainData[prefabRef.m_Prefab]);
					}
					else
					{
						num7 = 0;
					}
					if (num7 - num4 > 0)
					{
						curvePosition.y += objectSpeed * 2f / math.max(1f, m_Curve.m_Length);
					}
				}
				if (!(curvePosition.y <= num2))
				{
					UpdateMaxSpeed(laneObject2.m_LaneObject, blockerType, objectSpeed, curvePosition.x, distanceFactor, num3);
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

	private void CheckPedestrian(Segment overlapLine, Entity obj, float targetOffset, float distanceOffset, bool giveSpace)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		if ((targetOffset <= m_CurveOffset.x) | (targetOffset >= m_CurveOffset.y))
		{
			PrefabRef prefabRef = m_PrefabRefData[obj];
			Transform transform = m_TransformData[obj];
			float num = m_PrefabObjectGeometry.m_Size.x * 0.5f;
			if (m_PrefabObjectGeometryData.HasComponent(prefabRef.m_Prefab))
			{
				num += m_PrefabObjectGeometryData[prefabRef.m_Prefab].m_Size.z * 0.5f;
			}
			float num3 = default(float);
			float num2 = MathUtils.Distance(((Segment)(ref overlapLine)).xz, ((float3)(ref transform.m_Position)).xz, ref num3);
			float3 val = math.forward(transform.m_Rotation);
			float2 xz = ((float3)(ref val)).xz;
			val = MathUtils.Position(overlapLine, num3);
			float num4 = math.dot(xz, math.normalizesafe(((float3)(ref val)).xz - ((float3)(ref transform.m_Position)).xz, default(float2)));
			if (num2 - math.select(math.min(0f - num4, 0f), math.max(num4, 0f), giveSpace) >= num)
			{
				return;
			}
		}
		Moving moving = default(Moving);
		float num5 = ((m_PushBlockers || !m_MovingData.TryGetComponent(obj, ref moving) || !(math.lengthsq(moving.m_Velocity) >= 0.01f)) ? math.max(3f, VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, distanceOffset, 3f, m_SafeTimeStep)) : VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, distanceOffset, m_SafeTimeStep));
		num5 = MathUtils.Clamp(num5, m_SpeedRange);
		if (num5 < m_MaxSpeed)
		{
			m_MaxSpeed = num5;
			m_Blocker = obj;
			m_BlockerType = BlockerType.Temporary;
		}
	}

	private void UpdateMaxSpeed(Entity obj, BlockerType blockerType, float objectSpeed, float laneOffset, float distanceFactor, float distanceOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef prefabRef = m_PrefabRefData[obj];
		float num = 0f;
		if (m_PrefabTrainData.HasComponent(prefabRef.m_Prefab))
		{
			Train train = m_TrainData[obj];
			TrainData trainData = m_PrefabTrainData[prefabRef.m_Prefab];
			float2 val = trainData.m_AttachOffsets - trainData.m_BogieOffsets;
			num = math.select(val.y, val.x, (train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0);
		}
		else if (m_PrefabObjectGeometryData.HasComponent(prefabRef.m_Prefab))
		{
			num = 0f - m_PrefabObjectGeometryData[prefabRef.m_Prefab].m_Bounds.min.z;
		}
		if ((laneOffset - m_CurveOffset.y) * m_Curve.m_Length >= num)
		{
			return;
		}
		Transform transform = m_TransformData[obj];
		float num2 = math.distance(MathUtils.Position(m_Curve.m_Bezier, math.max(m_CurveOffset.x, laneOffset)), m_CurrentPosition);
		num2 -= math.max(0f, m_CurveOffset.x - laneOffset) * m_Curve.m_Length;
		num2 = ((!(math.dot(transform.m_Position - m_CurrentPosition, m_CurrentPosition - m_PrevPosition) < 0f)) ? math.min(num2, math.distance(transform.m_Position, m_CurrentPosition)) : math.min(num2, math.distance(transform.m_Position, m_PrevPosition) + m_PrevDistance - m_Distance));
		num2 -= num;
		num2 *= distanceFactor;
		num2 += distanceOffset;
		float maxBrakingSpeed;
		if (objectSpeed > 0.001f && m_PrefabCarData.HasComponent(prefabRef.m_Prefab))
		{
			CarData prefabCarData = m_PrefabCarData[prefabRef.m_Prefab];
			objectSpeed = math.max(0f, objectSpeed - prefabCarData.m_Braking * m_TimeStep * 2f) * distanceFactor;
			if (m_PrefabTrain.m_Braking >= prefabCarData.m_Braking)
			{
				num2 += objectSpeed * m_SafeTimeStep;
				maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, num2, objectSpeed, m_SafeTimeStep);
			}
			else
			{
				num2 += VehicleUtils.GetBrakingDistance(prefabCarData, objectSpeed, m_SafeTimeStep);
				maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, num2, m_SafeTimeStep);
			}
		}
		else if (objectSpeed > 0.001f && m_PrefabTrainData.HasComponent(prefabRef.m_Prefab))
		{
			TrainData prefabTrainData = m_PrefabTrainData[prefabRef.m_Prefab];
			objectSpeed = math.max(0f, objectSpeed - prefabTrainData.m_Braking * m_TimeStep * 2f) * distanceFactor;
			if (m_PrefabTrain.m_Braking >= prefabTrainData.m_Braking)
			{
				num2 += objectSpeed * m_SafeTimeStep;
				maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, num2, objectSpeed, m_SafeTimeStep);
			}
			else
			{
				num2 += VehicleUtils.GetBrakingDistance(prefabTrainData, objectSpeed, m_SafeTimeStep);
				maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, num2, m_SafeTimeStep);
			}
		}
		else
		{
			maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabTrain, num2, m_SafeTimeStep);
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

using System;
using Colossal.Mathematics;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct CarLaneSpeedIterator
{
	public ComponentLookup<Transform> m_TransformData;

	public ComponentLookup<Moving> m_MovingData;

	public ComponentLookup<Car> m_CarData;

	public ComponentLookup<Train> m_TrainData;

	public ComponentLookup<Controller> m_ControllerData;

	public ComponentLookup<LaneReservation> m_LaneReservationData;

	public ComponentLookup<LaneCondition> m_LaneConditionData;

	public ComponentLookup<LaneSignal> m_LaneSignalData;

	public ComponentLookup<Curve> m_CurveData;

	public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

	public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

	public ComponentLookup<Unspawned> m_UnspawnedData;

	public ComponentLookup<Creature> m_CreatureData;

	public ComponentLookup<PrefabRef> m_PrefabRefData;

	public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

	public ComponentLookup<CarData> m_PrefabCarData;

	public ComponentLookup<TrainData> m_PrefabTrainData;

	public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

	public BufferLookup<LaneOverlap> m_LaneOverlapData;

	public BufferLookup<LaneObject> m_LaneObjectData;

	public Entity m_Entity;

	public Entity m_Ignore;

	public NativeList<Entity> m_TempBuffer;

	public int m_Priority;

	public float m_TimeStep;

	public float m_SafeTimeStep;

	public float m_DistanceOffset;

	public float m_SpeedLimitFactor;

	public float m_CurrentSpeed;

	public CarData m_PrefabCar;

	public ObjectGeometryData m_PrefabObjectGeometry;

	public Bounds1 m_SpeedRange;

	public bool m_PushBlockers;

	public float m_MaxSpeed;

	public float m_CanChangeLane;

	public float3 m_CurrentPosition;

	public float m_Oncoming;

	public Entity m_Blocker;

	public BlockerType m_BlockerType;

	private Entity m_Lane;

	private Entity m_NextLane;

	private Curve m_Curve;

	private float2 m_CurveOffset;

	private float2 m_NextOffset;

	private float3 m_PrevPosition;

	private float m_PrevDistance;

	private float m_Distance;

	public bool IterateFirstLane(Entity lane, float3 curveOffset, Entity nextLane, float2 nextOffset, float laneOffset, bool requestSpace, out Game.Net.CarLaneFlags laneFlags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		Curve curve = m_CurveData[lane];
		bool flag = curveOffset.z < curveOffset.x;
		laneOffset = math.select(laneOffset, 0f - laneOffset, flag);
		laneFlags = ~(Game.Net.CarLaneFlags.Unsafe | Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.Invert | Game.Net.CarLaneFlags.SideConnection | Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.Twoway | Game.Net.CarLaneFlags.IsSecured | Game.Net.CarLaneFlags.Runway | Game.Net.CarLaneFlags.Yield | Game.Net.CarLaneFlags.Stop | Game.Net.CarLaneFlags.ForbidCombustionEngines | Game.Net.CarLaneFlags.ForbidTransitTraffic | Game.Net.CarLaneFlags.ForbidHeavyTraffic | Game.Net.CarLaneFlags.PublicOnly | Game.Net.CarLaneFlags.Highway | Game.Net.CarLaneFlags.UTurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight | Game.Net.CarLaneFlags.Forward | Game.Net.CarLaneFlags.Approach | Game.Net.CarLaneFlags.Roundabout | Game.Net.CarLaneFlags.RightLimit | Game.Net.CarLaneFlags.LeftLimit | Game.Net.CarLaneFlags.ForbidPassing | Game.Net.CarLaneFlags.RightOfWay | Game.Net.CarLaneFlags.TrafficLights | Game.Net.CarLaneFlags.ParkingLeft | Game.Net.CarLaneFlags.ParkingRight | Game.Net.CarLaneFlags.Forbidden | Game.Net.CarLaneFlags.AllowEnter);
		float3 val = MathUtils.Position(curve.m_Bezier, curveOffset.x);
		float3 lanePosition = VehicleUtils.GetLanePosition(curve.m_Bezier, curveOffset.x, laneOffset);
		m_PrevPosition = m_CurrentPosition;
		m_Distance = math.distance(m_CurrentPosition, lanePosition);
		Game.Net.CarLane carLaneData = default(Game.Net.CarLane);
		if (m_CarLaneData.TryGetComponent(lane, ref carLaneData))
		{
			carLaneData.m_SpeedLimit *= m_SpeedLimitFactor;
			laneFlags = carLaneData.m_Flags;
			float driveSpeed = VehicleUtils.GetMaxDriveSpeed(m_PrefabCar, carLaneData);
			int yieldOverride = 0;
			bool isRoundabout = false;
			if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.Approach) == 0)
			{
				isRoundabout = (carLaneData.m_Flags & Game.Net.CarLaneFlags.Roundabout) != 0;
				if ((carLaneData.m_Flags & (Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.TrafficLights)) != 0 && m_LaneSignalData.HasComponent(lane))
				{
					switch (m_LaneSignalData[lane].m_Signal)
					{
					case LaneSignalType.Stop:
						yieldOverride = -1;
						break;
					case LaneSignalType.Yield:
						yieldOverride = 1;
						break;
					}
				}
			}
			if (m_LaneConditionData.HasComponent(lane))
			{
				VehicleUtils.ModifyDriveSpeed(ref driveSpeed, m_LaneConditionData[lane]);
			}
			if (m_Priority < 102 && m_LaneReservationData.HasComponent(lane) && m_LaneReservationData[lane].GetPriority() == 102)
			{
				driveSpeed *= 0.5f;
			}
			if (driveSpeed < m_MaxSpeed)
			{
				m_MaxSpeed = MathUtils.Clamp(driveSpeed, m_SpeedRange);
				m_Blocker = Entity.Null;
				m_BlockerType = BlockerType.Limit;
			}
			float2 xy = ((float3)(ref curveOffset)).xy;
			float num = 0f - m_PrefabObjectGeometry.m_Bounds.max.z;
			float num2 = m_Distance + num + m_DistanceOffset;
			if (carLaneData.m_CautionEnd >= carLaneData.m_CautionStart)
			{
				Bounds1 cautionBounds = carLaneData.cautionBounds;
				float2 val2 = math.select(((float3)(ref curveOffset)).xz, ((float3)(ref curveOffset)).zx, flag);
				if (cautionBounds.max > val2.x && cautionBounds.min < val2.y)
				{
					float distance = num2 + curve.m_Length * math.max(0f, math.select(cautionBounds.min - val2.x, val2.y - cautionBounds.max, flag));
					float num3 = carLaneData.m_SpeedLimit * math.select(0.5f, 0.8f, (carLaneData.m_Flags & Game.Net.CarLaneFlags.IsSecured) != 0);
					driveSpeed = math.max(num3, VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, distance, num3, m_SafeTimeStep));
					if (driveSpeed < m_MaxSpeed)
					{
						m_MaxSpeed = MathUtils.Clamp(driveSpeed, m_SpeedRange);
						m_Blocker = Entity.Null;
						m_BlockerType = BlockerType.Caution;
					}
				}
			}
			m_Lane = lane;
			m_NextLane = nextLane;
			m_Curve = curve;
			m_CurveOffset = ((float3)(ref curveOffset)).xz;
			m_NextOffset = nextOffset;
			m_CurrentPosition = val;
			CheckCurrentLane(num2, xy, flag);
			CheckOverlappingLanes(num2, xy.y, yieldOverride, carLaneData.m_SpeedLimit, isRoundabout, flag, requestSpace);
		}
		float3 val3 = MathUtils.Position(curve.m_Bezier, curveOffset.z);
		float num4 = math.abs(curveOffset.z - curveOffset.x);
		float num5 = math.max(0.001f, math.lerp(math.distance(val, val3), curve.m_Length * num4, num4));
		if (num5 > 1f)
		{
			m_PrevPosition = m_CurrentPosition;
			m_PrevDistance = m_Distance;
		}
		m_CurrentPosition = val3;
		m_Distance += num5;
		float brakingDistance = VehicleUtils.GetBrakingDistance(m_PrefabCar, m_MaxSpeed, m_SafeTimeStep);
		return (m_Distance + m_DistanceOffset - 20f >= brakingDistance) | (m_MaxSpeed == m_SpeedRange.min);
	}

	public bool IterateFirstLane(Entity lane1, Entity lane2, float3 curveOffset, Entity nextLane, float2 nextOffset, float laneDelta, float laneOffset1, float laneOffset2, bool requestSpace, out Game.Net.CarLaneFlags laneFlags)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		laneDelta = math.saturate(laneDelta);
		laneFlags = ~(Game.Net.CarLaneFlags.Unsafe | Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.Invert | Game.Net.CarLaneFlags.SideConnection | Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.Twoway | Game.Net.CarLaneFlags.IsSecured | Game.Net.CarLaneFlags.Runway | Game.Net.CarLaneFlags.Yield | Game.Net.CarLaneFlags.Stop | Game.Net.CarLaneFlags.ForbidCombustionEngines | Game.Net.CarLaneFlags.ForbidTransitTraffic | Game.Net.CarLaneFlags.ForbidHeavyTraffic | Game.Net.CarLaneFlags.PublicOnly | Game.Net.CarLaneFlags.Highway | Game.Net.CarLaneFlags.UTurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight | Game.Net.CarLaneFlags.Forward | Game.Net.CarLaneFlags.Approach | Game.Net.CarLaneFlags.Roundabout | Game.Net.CarLaneFlags.RightLimit | Game.Net.CarLaneFlags.LeftLimit | Game.Net.CarLaneFlags.ForbidPassing | Game.Net.CarLaneFlags.RightOfWay | Game.Net.CarLaneFlags.TrafficLights | Game.Net.CarLaneFlags.ParkingLeft | Game.Net.CarLaneFlags.ParkingRight | Game.Net.CarLaneFlags.Forbidden | Game.Net.CarLaneFlags.AllowEnter);
		Curve curve = m_CurveData[lane1];
		Curve curve2 = m_CurveData[lane2];
		float3 val = MathUtils.Position(curve.m_Bezier, curveOffset.x);
		float3 val2 = MathUtils.Position(curve2.m_Bezier, curveOffset.x);
		float3 val3 = math.lerp(val, val2, laneDelta);
		float3 lanePosition = VehicleUtils.GetLanePosition(curve.m_Bezier, curveOffset.x, laneOffset1);
		float3 lanePosition2 = VehicleUtils.GetLanePosition(curve2.m_Bezier, curveOffset.x, laneOffset2);
		float3 val4 = math.lerp(lanePosition, lanePosition2, laneDelta);
		m_PrevPosition = m_CurrentPosition;
		m_Distance = math.distance(m_CurrentPosition, val4);
		Game.Net.CarLane carLaneData = default(Game.Net.CarLane);
		if (m_CarLaneData.TryGetComponent(lane1, ref carLaneData))
		{
			carLaneData.m_SpeedLimit *= m_SpeedLimitFactor;
			laneFlags = carLaneData.m_Flags;
			float driveSpeed = VehicleUtils.GetMaxDriveSpeed(m_PrefabCar, carLaneData);
			int yieldOverride = 0;
			bool isRoundabout = false;
			bool flag = curveOffset.z < curveOffset.x;
			if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.Approach) == 0)
			{
				isRoundabout = (carLaneData.m_Flags & Game.Net.CarLaneFlags.Roundabout) != 0;
				if ((carLaneData.m_Flags & (Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.TrafficLights)) != 0 && m_LaneSignalData.HasComponent(lane1))
				{
					switch (m_LaneSignalData[lane1].m_Signal)
					{
					case LaneSignalType.Stop:
						yieldOverride = -1;
						break;
					case LaneSignalType.Yield:
						yieldOverride = 1;
						break;
					}
				}
			}
			if (m_LaneConditionData.HasComponent(lane1))
			{
				VehicleUtils.ModifyDriveSpeed(ref driveSpeed, m_LaneConditionData[lane1]);
			}
			if (m_Priority < 102 && m_LaneReservationData.HasComponent(lane1) && m_LaneReservationData.HasComponent(lane2))
			{
				if (laneDelta < 0.9f)
				{
					LaneReservation laneReservation = m_LaneReservationData[lane1];
					LaneReservation laneReservation2 = m_LaneReservationData[lane2];
					if (math.any(new int2(laneReservation.GetPriority(), laneReservation2.GetPriority()) == 102))
					{
						driveSpeed *= 0.5f;
					}
				}
				else if (m_LaneReservationData[lane2].GetPriority() == 102)
				{
					driveSpeed *= 0.5f;
				}
			}
			if (driveSpeed < m_MaxSpeed)
			{
				m_MaxSpeed = MathUtils.Clamp(driveSpeed, m_SpeedRange);
				m_Blocker = Entity.Null;
				m_BlockerType = BlockerType.Limit;
			}
			float2 xy = ((float3)(ref curveOffset)).xy;
			float num = 0f - m_PrefabObjectGeometry.m_Bounds.max.z;
			float num2 = m_Distance + num + m_DistanceOffset;
			if (carLaneData.m_CautionEnd >= carLaneData.m_CautionStart)
			{
				Bounds1 cautionBounds = carLaneData.cautionBounds;
				float2 val5 = math.select(((float3)(ref curveOffset)).xz, ((float3)(ref curveOffset)).zx, flag);
				if (cautionBounds.max > val5.x && cautionBounds.min < val5.y)
				{
					float distance = num2 + curve.m_Length * math.max(0f, math.select(cautionBounds.min - val5.x, val5.y - cautionBounds.max, flag));
					float num3 = carLaneData.m_SpeedLimit * math.select(0.5f, 0.8f, (carLaneData.m_Flags & Game.Net.CarLaneFlags.IsSecured) != 0);
					driveSpeed = math.max(num3, VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, distance, num3, m_SafeTimeStep));
					if (driveSpeed < m_MaxSpeed)
					{
						m_MaxSpeed = MathUtils.Clamp(driveSpeed, m_SpeedRange);
						m_Blocker = Entity.Null;
						m_BlockerType = BlockerType.Caution;
					}
				}
			}
			if (laneDelta < 0.9f)
			{
				m_Lane = lane1;
				m_Curve = curve;
				m_CurveOffset = ((float3)(ref curveOffset)).xz;
				m_CurrentPosition = val;
				CheckCurrentLane(num2, xy, flag);
				CheckOverlappingLanes(num2, xy.y, yieldOverride, carLaneData.m_SpeedLimit, isRoundabout, flag, requestSpace);
			}
			m_Lane = lane2;
			m_NextLane = nextLane;
			m_Curve = curve2;
			m_CurveOffset = ((float3)(ref curveOffset)).xz;
			m_NextOffset = nextOffset;
			m_CurrentPosition = val2;
			if (laneDelta == 0f)
			{
				CheckCurrentLane(num2, xy, flag, ref m_CanChangeLane);
			}
			else
			{
				CheckCurrentLane(num2, xy, flag);
			}
			CheckOverlappingLanes(num2, xy.y, 0, carLaneData.m_SpeedLimit, isRoundabout, flag, requestSpace);
		}
		float3 val6 = MathUtils.Position(curve2.m_Bezier, curveOffset.z);
		float num4 = math.lerp(curve.m_Length, curve2.m_Length, laneDelta);
		float num5 = math.abs(curveOffset.z - curveOffset.x);
		float num6 = math.max(0.001f, math.lerp(math.distance(val3, val6), num4 * num5, num5));
		if (num6 > 1f)
		{
			m_PrevPosition = m_CurrentPosition;
			m_PrevDistance = m_Distance;
		}
		m_CurrentPosition = val6;
		m_Distance += num6;
		float brakingDistance = VehicleUtils.GetBrakingDistance(m_PrefabCar, m_MaxSpeed, m_SafeTimeStep);
		return (m_Distance + m_DistanceOffset - 20f >= brakingDistance) | (m_MaxSpeed == m_SpeedRange.min);
	}

	public bool IterateNextLane(Entity lane, float2 curveOffset, float minOffset, NativeArray<CarNavigationLane> nextLanes, bool requestSpace, ref Game.Net.CarLaneFlags laneFlags, out bool needSignal)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		needSignal = false;
		Game.Net.CarLaneFlags carLaneFlags = laneFlags;
		laneFlags = ~(Game.Net.CarLaneFlags.Unsafe | Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.Invert | Game.Net.CarLaneFlags.SideConnection | Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.Twoway | Game.Net.CarLaneFlags.IsSecured | Game.Net.CarLaneFlags.Runway | Game.Net.CarLaneFlags.Yield | Game.Net.CarLaneFlags.Stop | Game.Net.CarLaneFlags.ForbidCombustionEngines | Game.Net.CarLaneFlags.ForbidTransitTraffic | Game.Net.CarLaneFlags.ForbidHeavyTraffic | Game.Net.CarLaneFlags.PublicOnly | Game.Net.CarLaneFlags.Highway | Game.Net.CarLaneFlags.UTurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight | Game.Net.CarLaneFlags.Forward | Game.Net.CarLaneFlags.Approach | Game.Net.CarLaneFlags.Roundabout | Game.Net.CarLaneFlags.RightLimit | Game.Net.CarLaneFlags.LeftLimit | Game.Net.CarLaneFlags.ForbidPassing | Game.Net.CarLaneFlags.RightOfWay | Game.Net.CarLaneFlags.TrafficLights | Game.Net.CarLaneFlags.ParkingLeft | Game.Net.CarLaneFlags.ParkingRight | Game.Net.CarLaneFlags.Forbidden | Game.Net.CarLaneFlags.AllowEnter);
		Curve curve = default(Curve);
		if (!m_CurveData.TryGetComponent(lane, ref curve))
		{
			return false;
		}
		Game.Net.CarLane carLaneData = default(Game.Net.CarLane);
		if (m_CarLaneData.TryGetComponent(lane, ref carLaneData))
		{
			carLaneData.m_SpeedLimit *= m_SpeedLimitFactor;
			laneFlags = carLaneData.m_Flags;
			float driveSpeed = VehicleUtils.GetMaxDriveSpeed(m_PrefabCar, carLaneData);
			float num = 0f - m_PrefabObjectGeometry.m_Bounds.max.z;
			float num2 = m_Distance + num;
			int yieldOverride = 0;
			bool flag = false;
			bool flag2 = curveOffset.y < curveOffset.x;
			Entity blocker = Entity.Null;
			BlockerType blockerType = BlockerType.Limit;
			if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.Approach) == 0)
			{
				if ((carLaneFlags & Game.Net.CarLaneFlags.Approach) == 0)
				{
					carLaneData.m_Flags &= ~(Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.Stop | Game.Net.CarLaneFlags.TrafficLights);
					if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.SideConnection) == 0)
					{
						carLaneData.m_Flags &= ~(Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.UTurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight | Game.Net.CarLaneFlags.Forward);
					}
				}
				flag = (carLaneData.m_Flags & Game.Net.CarLaneFlags.Roundabout) != 0;
				LaneSignal laneSignal = default(LaneSignal);
				if ((carLaneData.m_Flags & (Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.TrafficLights)) != 0 && m_LaneSignalData.TryGetComponent(lane, ref laneSignal))
				{
					float brakingDistance = VehicleUtils.GetBrakingDistance(m_PrefabCar, m_CurrentSpeed, 0f);
					if (!flag && brakingDistance <= num2 && !CheckSpace(lane, curveOffset, nextLanes, out blocker))
					{
						driveSpeed = 0f;
						blockerType = BlockerType.Continuing;
					}
					else
					{
						needSignal = true;
						switch (laneSignal.m_Signal)
						{
						case LaneSignalType.Stop:
							if ((m_Priority < 108 || (laneSignal.m_Flags & LaneSignalFlags.Physical) != 0) && brakingDistance <= num2 + 1f)
							{
								driveSpeed = 0f;
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
							if ((m_Priority < 108 || (laneSignal.m_Flags & LaneSignalFlags.Physical) != 0) && brakingDistance <= num2)
							{
								driveSpeed = 0f;
								blocker = laneSignal.m_Blocker;
								blockerType = BlockerType.Signal;
							}
							break;
						case LaneSignalType.Yield:
							yieldOverride = 1;
							break;
						}
					}
				}
				else if ((carLaneData.m_Flags & Game.Net.CarLaneFlags.Stop) != 0)
				{
					if (m_Priority < 108 && num2 >= 1.1f)
					{
						driveSpeed = 0f;
						blockerType = BlockerType.Limit;
					}
					else if (!flag && VehicleUtils.GetBrakingDistance(m_PrefabCar, m_CurrentSpeed, 0f) <= num2 && !CheckSpace(lane, curveOffset, nextLanes, out blocker))
					{
						driveSpeed = 0f;
						blockerType = BlockerType.Continuing;
					}
					yieldOverride = 1;
				}
				else if ((carLaneData.m_Flags & (Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.UTurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight | Game.Net.CarLaneFlags.Forward)) != 0 && !flag && VehicleUtils.GetBrakingDistance(m_PrefabCar, m_CurrentSpeed, 0f) <= num2 && !CheckSpace(lane, curveOffset, nextLanes, out blocker))
				{
					driveSpeed = 0f;
					blockerType = BlockerType.Continuing;
				}
			}
			num2 += m_DistanceOffset;
			float num3;
			if (driveSpeed == 0f)
			{
				num3 = VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, math.max(0f, num2 - 0.5f), m_SafeTimeStep);
			}
			else
			{
				if (m_LaneConditionData.HasComponent(lane))
				{
					VehicleUtils.ModifyDriveSpeed(ref driveSpeed, m_LaneConditionData[lane]);
				}
				num3 = math.max(driveSpeed, VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, m_Distance, driveSpeed, m_TimeStep));
			}
			if (num3 < m_MaxSpeed)
			{
				m_MaxSpeed = MathUtils.Clamp(num3, m_SpeedRange);
				m_Blocker = blocker;
				m_BlockerType = blockerType;
			}
			if (carLaneData.m_CautionEnd >= carLaneData.m_CautionStart)
			{
				Bounds1 cautionBounds = carLaneData.cautionBounds;
				float2 val = math.select(curveOffset, ((float2)(ref curveOffset)).yx, flag2);
				if (cautionBounds.max > val.x && cautionBounds.min < val.y)
				{
					float distance = num2 + curve.m_Length * math.max(0f, math.select(cautionBounds.min - val.x, val.y - cautionBounds.max, flag2));
					float num4 = carLaneData.m_SpeedLimit * math.select(0.5f, 0.8f, (carLaneData.m_Flags & Game.Net.CarLaneFlags.IsSecured) != 0);
					num3 = math.max(num4, VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, distance, num4, m_SafeTimeStep));
					if (num3 < m_MaxSpeed)
					{
						m_MaxSpeed = MathUtils.Clamp(num3, m_SpeedRange);
						m_Blocker = Entity.Null;
						m_BlockerType = BlockerType.Caution;
					}
				}
			}
			m_Curve = curve;
			m_CurveOffset = curveOffset;
			m_Lane = lane;
			if (nextLanes.Length != 0)
			{
				CarNavigationLane carNavigationLane = nextLanes[0];
				m_NextOffset = carNavigationLane.m_CurvePosition;
				m_NextLane = carNavigationLane.m_Lane;
			}
			else
			{
				m_NextOffset = float2.op_Implicit(0f);
				m_NextLane = Entity.Null;
			}
			minOffset = math.select(minOffset, curveOffset.x, flag2 ? (curveOffset.x < 1f) : (curveOffset.x > 0f));
			CheckCurrentLane(num2, float2.op_Implicit(minOffset), flag2);
			CheckOverlappingLanes(num2, minOffset, yieldOverride, carLaneData.m_SpeedLimit, flag, flag2, requestSpace);
		}
		else if (m_ParkingLaneData.HasComponent(lane))
		{
			float num5 = 0f - m_PrefabObjectGeometry.m_Bounds.max.z;
			float distance2 = m_Distance + num5 + m_DistanceOffset;
			m_Curve = curve;
			m_CurveOffset = curveOffset;
			m_Lane = lane;
			CheckParkingLane(distance2);
		}
		float3 val2 = MathUtils.Position(curve.m_Bezier, curveOffset.y);
		float num6 = math.abs(curveOffset.y - curveOffset.x);
		float num7 = math.max(0.001f, math.lerp(math.distance(m_CurrentPosition, val2), curve.m_Length * num6, num6));
		if (num7 > 1f)
		{
			m_PrevPosition = m_CurrentPosition;
			m_PrevDistance = m_Distance;
		}
		m_CurrentPosition = val2;
		m_Distance += num7;
		float brakingDistance2 = VehicleUtils.GetBrakingDistance(m_PrefabCar, m_MaxSpeed, m_SafeTimeStep);
		return (m_Distance + m_DistanceOffset - 20f >= brakingDistance2) | (m_MaxSpeed == m_SpeedRange.min);
	}

	public void IterateTarget(float3 targetPosition)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		float maxDriveSpeed = VehicleUtils.GetMaxDriveSpeed(m_PrefabCar, 11.111112f, (float)Math.PI / 12f);
		IterateTarget(targetPosition, maxDriveSpeed);
	}

	public void IterateTarget(float3 targetPosition, float maxLaneSpeed)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		float maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, m_Distance, maxLaneSpeed, m_TimeStep);
		m_Distance += math.distance(m_CurrentPosition, targetPosition);
		maxBrakingSpeed = math.min(maxBrakingSpeed, VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, m_Distance, m_TimeStep));
		if (maxBrakingSpeed < m_MaxSpeed)
		{
			m_MaxSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
			m_Blocker = Entity.Null;
			m_BlockerType = BlockerType.None;
		}
	}

	private bool CheckSpace(Entity currentLane, float2 curveOffset, NativeArray<CarNavigationLane> nextLanes, out Entity blocker)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_096a: Unknown result type (might be due to invalid IL or missing references)
		//IL_096f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0993: Unknown result type (might be due to invalid IL or missing references)
		//IL_099e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_076c: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_086c: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_0883: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_089c: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0840: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_090a: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		blocker = Entity.Null;
		if (nextLanes.Length == 0)
		{
			return true;
		}
		CarNavigationLane carNavigationLane = nextLanes[0];
		bool flag = carNavigationLane.m_CurvePosition.y < carNavigationLane.m_CurvePosition.x;
		Game.Net.CarLane carLane = default(Game.Net.CarLane);
		if (carNavigationLane.m_CurvePosition.x != math.select(0f, 1f, flag) || !m_CarLaneData.TryGetComponent(carNavigationLane.m_Lane, ref carLane))
		{
			return true;
		}
		if ((carLane.m_Flags & (Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.Yield | Game.Net.CarLaneFlags.Stop | Game.Net.CarLaneFlags.UTurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight | Game.Net.CarLaneFlags.Forward | Game.Net.CarLaneFlags.Roundabout | Game.Net.CarLaneFlags.RightOfWay | Game.Net.CarLaneFlags.TrafficLights)) != 0 && (carLane.m_Flags & Game.Net.CarLaneFlags.Approach) == 0)
		{
			return true;
		}
		DynamicBuffer<LaneOverlap> val = default(DynamicBuffer<LaneOverlap>);
		if (!m_LaneOverlapData.TryGetBuffer(currentLane, ref val))
		{
			return true;
		}
		Curve curve = m_CurveData[carNavigationLane.m_Lane];
		bool num = curveOffset.y < curveOffset.x;
		bool flag2 = false;
		float num2 = float.MaxValue;
		float num3 = MathUtils.Size(((Bounds3)(ref m_PrefabObjectGeometry.m_Bounds)).z);
		float num4 = num3;
		int num5 = 1;
		OverlapFlags overlapFlags = (num ? OverlapFlags.MergeStart : OverlapFlags.MergeEnd);
		float3 val2 = math.normalizesafe(MathUtils.Tangent(curve.m_Bezier, carNavigationLane.m_CurvePosition.x), default(float3));
		float3 val3 = MathUtils.Position(curve.m_Bezier, carNavigationLane.m_CurvePosition.x);
		val2 = math.select(val2, -val2, flag);
		DynamicBuffer<LaneObject> val4 = default(DynamicBuffer<LaneObject>);
		Controller controller = default(Controller);
		Car carData = default(Car);
		Moving moving = default(Moving);
		ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
		TrainData prefabTrainData = default(TrainData);
		CarData prefabCarData = default(CarData);
		for (int i = 0; i < val.Length; i++)
		{
			LaneOverlap laneOverlap = val[i];
			if ((laneOverlap.m_Flags & (OverlapFlags.MergeStart | OverlapFlags.MergeEnd | OverlapFlags.MergeMiddleStart | OverlapFlags.MergeMiddleEnd | OverlapFlags.Unsafe | OverlapFlags.Water)) == 0)
			{
				flag2 = true;
			}
			else
			{
				if ((laneOverlap.m_Flags & overlapFlags) == 0 || !(laneOverlap.m_Other != carNavigationLane.m_Lane) || !m_LaneObjectData.TryGetBuffer(laneOverlap.m_Other, ref val4) || val4.Length == 0)
				{
					continue;
				}
				float2 val5 = new float2((float)(int)laneOverlap.m_OtherStart, (float)(int)laneOverlap.m_OtherEnd) * 0.003921569f;
				for (int j = 0; j < val4.Length; j++)
				{
					LaneObject laneObject = val4[j];
					if (laneObject.m_LaneObject == m_Entity)
					{
						continue;
					}
					Entity val6 = laneObject.m_LaneObject;
					if (m_ControllerData.TryGetComponent(laneObject.m_LaneObject, ref controller))
					{
						if (controller.m_Controller == m_Entity)
						{
							continue;
						}
						val6 = controller.m_Controller;
					}
					float2 curvePosition = laneObject.m_CurvePosition;
					if ((curvePosition.y < curvePosition.x) ? (curvePosition.y >= val5.y) : (curvePosition.y <= val5.x))
					{
						int num6;
						if (m_CarData.TryGetComponent(val6, ref carData))
						{
							num6 = VehicleUtils.GetPriority(carData);
						}
						else if (m_TrainData.HasComponent(laneObject.m_LaneObject))
						{
							PrefabRef prefabRef = m_PrefabRefData[laneObject.m_LaneObject];
							num6 = VehicleUtils.GetPriority(m_PrefabTrainData[prefabRef.m_Prefab]);
						}
						else
						{
							num6 = 0;
						}
						if (num6 < m_Priority)
						{
							continue;
						}
					}
					if (!m_MovingData.TryGetComponent(laneObject.m_LaneObject, ref moving))
					{
						continue;
					}
					PrefabRef prefabRef2 = m_PrefabRefData[laneObject.m_LaneObject];
					if (!m_PrefabObjectGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref objectGeometryData))
					{
						continue;
					}
					num4 += MathUtils.Size(((Bounds3)(ref objectGeometryData.m_Bounds)).z) + 1f;
					num5++;
					blocker = laneObject.m_LaneObject;
					if (!(num2 >= num3))
					{
						continue;
					}
					float num7 = 0f - objectGeometryData.m_Bounds.min.z;
					bool flag3 = false;
					if (m_PrefabTrainData.TryGetComponent(prefabRef2.m_Prefab, ref prefabTrainData))
					{
						Train train = m_TrainData[laneObject.m_LaneObject];
						float2 val7 = prefabTrainData.m_AttachOffsets - prefabTrainData.m_BogieOffsets;
						num7 = math.select(val7.y, val7.x, (train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0);
						flag3 = true;
					}
					num2 = math.dot(m_TransformData[laneObject.m_LaneObject].m_Position - val3, val2) - num7;
					float num8 = math.dot(moving.m_Velocity, val2);
					if (num8 > 0.001f)
					{
						if (m_PrefabCarData.TryGetComponent(prefabRef2.m_Prefab, ref prefabCarData))
						{
							num2 += VehicleUtils.GetBrakingDistance(prefabCarData, num8, m_SafeTimeStep);
						}
						else if (flag3)
						{
							num2 += VehicleUtils.GetBrakingDistance(prefabTrainData, num8, m_SafeTimeStep);
						}
					}
				}
			}
		}
		if (!flag2)
		{
			return true;
		}
		if (m_LaneObjectData.TryGetBuffer(currentLane, ref val4))
		{
			Controller controller2 = default(Controller);
			Moving moving2 = default(Moving);
			ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
			TrainData prefabTrainData2 = default(TrainData);
			CarData prefabCarData2 = default(CarData);
			for (int k = 0; k < val4.Length; k++)
			{
				LaneObject laneObject2 = val4[k];
				if (laneObject2.m_LaneObject == m_Entity || (m_ControllerData.TryGetComponent(laneObject2.m_LaneObject, ref controller2) && controller2.m_Controller == m_Entity) || !m_MovingData.TryGetComponent(laneObject2.m_LaneObject, ref moving2))
				{
					continue;
				}
				PrefabRef prefabRef3 = m_PrefabRefData[laneObject2.m_LaneObject];
				if (!m_PrefabObjectGeometryData.TryGetComponent(prefabRef3.m_Prefab, ref objectGeometryData2))
				{
					continue;
				}
				num4 += MathUtils.Size(((Bounds3)(ref objectGeometryData2.m_Bounds)).z) + 1f;
				num5++;
				blocker = laneObject2.m_LaneObject;
				if (!(num2 >= num3))
				{
					continue;
				}
				float num9 = 0f - objectGeometryData2.m_Bounds.min.z;
				bool flag4 = false;
				if (m_PrefabTrainData.TryGetComponent(prefabRef3.m_Prefab, ref prefabTrainData2))
				{
					Train train2 = m_TrainData[laneObject2.m_LaneObject];
					float2 val8 = prefabTrainData2.m_AttachOffsets - prefabTrainData2.m_BogieOffsets;
					num9 = math.select(val8.y, val8.x, (train2.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0);
					flag4 = true;
				}
				num2 = math.dot(m_TransformData[laneObject2.m_LaneObject].m_Position - val3, val2) - num9;
				float num10 = math.dot(moving2.m_Velocity, val2);
				if (num10 > 0.001f)
				{
					if (m_PrefabCarData.TryGetComponent(prefabRef3.m_Prefab, ref prefabCarData2))
					{
						num2 += VehicleUtils.GetBrakingDistance(prefabCarData2, num10, m_SafeTimeStep);
					}
					else if (flag4)
					{
						num2 += VehicleUtils.GetBrakingDistance(prefabTrainData2, num10, m_SafeTimeStep);
					}
				}
			}
		}
		if (num2 != float.MaxValue && num2 >= num3)
		{
			blocker = Entity.Null;
			return true;
		}
		num2 = 0f;
		int num11 = 1;
		Controller controller3 = default(Controller);
		Moving moving3 = default(Moving);
		ObjectGeometryData objectGeometryData3 = default(ObjectGeometryData);
		TrainData prefabTrainData3 = default(TrainData);
		CarData prefabCarData3 = default(CarData);
		while (true)
		{
			if (m_LaneObjectData.TryGetBuffer(carNavigationLane.m_Lane, ref val4))
			{
				for (int l = 0; l < val4.Length; l++)
				{
					LaneObject laneObject3 = val4[math.select(l, val4.Length - 1 - l, flag)];
					bool flag5 = laneObject3.m_CurvePosition.y < laneObject3.m_CurvePosition.x;
					if (flag != flag5)
					{
						continue;
					}
					if (flag)
					{
						if (laneObject3.m_CurvePosition.x > carNavigationLane.m_CurvePosition.x)
						{
							continue;
						}
					}
					else if (laneObject3.m_CurvePosition.x < carNavigationLane.m_CurvePosition.x)
					{
						continue;
					}
					if (laneObject3.m_LaneObject == m_Entity || (m_ControllerData.TryGetComponent(laneObject3.m_LaneObject, ref controller3) && controller3.m_Controller == m_Entity) || !m_MovingData.TryGetComponent(laneObject3.m_LaneObject, ref moving3))
					{
						continue;
					}
					PrefabRef prefabRef4 = m_PrefabRefData[laneObject3.m_LaneObject];
					m_PrefabObjectGeometryData.TryGetComponent(prefabRef4.m_Prefab, ref objectGeometryData3);
					float num12 = 0f - objectGeometryData3.m_Bounds.min.z;
					bool flag6 = false;
					if (m_PrefabTrainData.TryGetComponent(prefabRef4.m_Prefab, ref prefabTrainData3))
					{
						Train train3 = m_TrainData[laneObject3.m_LaneObject];
						float2 val9 = prefabTrainData3.m_AttachOffsets - prefabTrainData3.m_BogieOffsets;
						num12 = math.select(val9.y, val9.x, (train3.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0);
						flag6 = true;
					}
					float num13 = num2 + math.dot(m_TransformData[laneObject3.m_LaneObject].m_Position - val3, val2) - num12;
					float num14 = math.dot(moving3.m_Velocity, val2);
					if (num14 > 0.001f)
					{
						if (m_PrefabCarData.TryGetComponent(prefabRef4.m_Prefab, ref prefabCarData3))
						{
							num13 += VehicleUtils.GetBrakingDistance(prefabCarData3, num14, m_SafeTimeStep);
						}
						else if (flag6)
						{
							num13 += VehicleUtils.GetBrakingDistance(prefabTrainData3, num14, m_SafeTimeStep);
						}
					}
					if (num13 >= num4)
					{
						blocker = Entity.Null;
						return true;
					}
					blocker = laneObject3.m_LaneObject;
					if (--num5 == 0)
					{
						return false;
					}
					num4 += MathUtils.Size(((Bounds3)(ref objectGeometryData3.m_Bounds)).z) + 1f;
				}
			}
			num2 += curve.m_Length;
			if (math.max(num2, num3) >= num4)
			{
				blocker = Entity.Null;
				return true;
			}
			if (num11 >= nextLanes.Length)
			{
				return false;
			}
			carNavigationLane = nextLanes[num11++];
			flag = carNavigationLane.m_CurvePosition.y < carNavigationLane.m_CurvePosition.x;
			if (carNavigationLane.m_CurvePosition.x != math.select(0f, 1f, flag) || !m_CarLaneData.TryGetComponent(carNavigationLane.m_Lane, ref carLane))
			{
				return false;
			}
			if ((carLane.m_Flags & (Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.Yield | Game.Net.CarLaneFlags.Stop | Game.Net.CarLaneFlags.UTurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight | Game.Net.CarLaneFlags.Forward | Game.Net.CarLaneFlags.Roundabout | Game.Net.CarLaneFlags.RightOfWay | Game.Net.CarLaneFlags.TrafficLights)) != 0 && (carLane.m_Flags & Game.Net.CarLaneFlags.Approach) == 0)
			{
				break;
			}
			curve = m_CurveData[carNavigationLane.m_Lane];
			val2 = math.normalizesafe(MathUtils.Tangent(curve.m_Bezier, carNavigationLane.m_CurvePosition.x), default(float3));
			val3 = MathUtils.Position(curve.m_Bezier, carNavigationLane.m_CurvePosition.x);
			val2 = math.select(val2, -val2, flag);
		}
		return false;
	}

	private bool CheckOverlapSpace(Entity currentLane, float2 curCurvePos, Entity nextLane, float2 nextCurvePos, float2 overlapPos, out Entity blocker)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		blocker = Entity.Null;
		Entity val = Entity.Null;
		Curve curve = m_CurveData[currentLane];
		float num = curve.m_Length * (1f - curCurvePos.x);
		float num2 = MathUtils.Size(((Bounds3)(ref m_PrefabObjectGeometry.m_Bounds)).z);
		float num3 = num2;
		DynamicBuffer<LaneObject> val2 = default(DynamicBuffer<LaneObject>);
		if (m_LaneObjectData.TryGetBuffer(currentLane, ref val2))
		{
			Controller controller = default(Controller);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			for (int i = 0; i < val2.Length; i++)
			{
				LaneObject laneObject = val2[i];
				if (laneObject.m_CurvePosition.x < curCurvePos.x || laneObject.m_LaneObject == m_Entity || (m_ControllerData.TryGetComponent(laneObject.m_LaneObject, ref controller) && controller.m_Controller == m_Entity))
				{
					continue;
				}
				if (m_MovingData.HasComponent(laneObject.m_LaneObject))
				{
					PrefabRef prefabRef = m_PrefabRefData[laneObject.m_LaneObject];
					if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
					{
						num3 += MathUtils.Size(((Bounds3)(ref objectGeometryData.m_Bounds)).z) + 1f;
						blocker = laneObject.m_LaneObject;
					}
				}
				if (laneObject.m_CurvePosition.y >= overlapPos.y)
				{
					val = laneObject.m_LaneObject;
					break;
				}
			}
		}
		if (val == Entity.Null && m_CarLaneData.HasComponent(nextLane))
		{
			num += m_CurveData[nextLane].m_Length;
			if (m_LaneObjectData.TryGetBuffer(nextLane, ref val2))
			{
				Controller controller2 = default(Controller);
				for (int j = 0; j < val2.Length; j++)
				{
					LaneObject laneObject2 = val2[j];
					if (!(laneObject2.m_CurvePosition.x < nextCurvePos.x) && !(laneObject2.m_LaneObject == m_Entity) && (!m_ControllerData.TryGetComponent(laneObject2.m_LaneObject, ref controller2) || !(controller2.m_Controller == m_Entity)))
					{
						val = laneObject2.m_LaneObject;
						break;
					}
				}
			}
		}
		num = math.max(num, num2);
		if (val != Entity.Null)
		{
			Moving moving = default(Moving);
			if (m_MovingData.TryGetComponent(val, ref moving))
			{
				PrefabRef prefabRef2 = m_PrefabRefData[val];
				float num4 = 0f;
				bool flag = false;
				TrainData prefabTrainData = default(TrainData);
				ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
				if (m_PrefabTrainData.TryGetComponent(prefabRef2.m_Prefab, ref prefabTrainData))
				{
					Train train = m_TrainData[val];
					float2 val3 = prefabTrainData.m_AttachOffsets - prefabTrainData.m_BogieOffsets;
					num4 = math.select(val3.y, val3.x, (train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0);
					flag = true;
				}
				else if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref objectGeometryData2))
				{
					num4 = 0f - objectGeometryData2.m_Bounds.min.z;
				}
				Transform transform = m_TransformData[val];
				float3 val4 = math.normalizesafe(MathUtils.Tangent(curve.m_Bezier, overlapPos.y), default(float3));
				num = math.dot(transform.m_Position - MathUtils.Position(curve.m_Bezier, overlapPos.y), val4) - num4;
				float num5 = math.dot(moving.m_Velocity, val4);
				if (num5 > 0.001f)
				{
					CarData prefabCarData = default(CarData);
					if (m_PrefabCarData.TryGetComponent(prefabRef2.m_Prefab, ref prefabCarData))
					{
						num += VehicleUtils.GetBrakingDistance(prefabCarData, num5, m_SafeTimeStep);
					}
					else if (flag)
					{
						num += VehicleUtils.GetBrakingDistance(prefabTrainData, num5, m_SafeTimeStep);
					}
				}
			}
			blocker = val;
		}
		if (num >= num3)
		{
			blocker = Entity.Null;
			return true;
		}
		return false;
	}

	private void CheckParkingLane(float distance)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
		if (!m_LaneObjectData.TryGetBuffer(m_Lane, ref val) || val.Length == 0)
		{
			return;
		}
		PrefabRef prefabRef = m_PrefabRefData[m_Lane];
		ParkingLaneData parkingLaneData = m_PrefabParkingLaneData[prefabRef.m_Prefab];
		float3 val2 = MathUtils.Position(m_Curve.m_Bezier, m_CurveOffset.x);
		float2 val3;
		if (parkingLaneData.m_SlotInterval == 0f)
		{
			val3 = float2.op_Implicit(VehicleUtils.GetParkingSize(m_PrefabObjectGeometry, out var offset).y * 0.5f);
			val3.x += 0.9f + offset;
			val3.y += 0.9f - offset;
		}
		else
		{
			val3 = float2.op_Implicit(0.1f);
		}
		Controller controller = default(Controller);
		for (int i = 0; i < val.Length; i++)
		{
			LaneObject laneObject = val[i];
			if (laneObject.m_LaneObject == m_Entity || (m_ControllerData.TryGetComponent(laneObject.m_LaneObject, ref controller) && controller.m_Controller == m_Entity) || m_UnspawnedData.HasComponent(laneObject.m_LaneObject))
			{
				continue;
			}
			bool flag = laneObject.m_CurvePosition.y >= m_CurveOffset.x;
			float3 val4 = MathUtils.Position(m_Curve.m_Bezier, laneObject.m_CurvePosition.y);
			float num = math.select(val3.x, val3.y, flag);
			if (parkingLaneData.m_SlotInterval == 0f)
			{
				float2 parkingOffsets = VehicleUtils.GetParkingOffsets(laneObject.m_LaneObject, ref m_PrefabRefData, ref m_PrefabObjectGeometryData);
				num += math.select(parkingOffsets.y, parkingOffsets.x, flag);
			}
			if (math.distance(val2, val4) < num)
			{
				float maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, distance, m_SafeTimeStep);
				maxBrakingSpeed = math.select(maxBrakingSpeed, 3f, laneObject.m_LaneObject == m_Ignore && maxBrakingSpeed < 1f);
				maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
				if (maxBrakingSpeed < m_MaxSpeed)
				{
					m_MaxSpeed = maxBrakingSpeed;
					m_Blocker = laneObject.m_LaneObject;
					m_BlockerType = BlockerType.Continuing;
				}
			}
		}
	}

	private void CheckCurrentLane(float distance, float2 minOffset, bool inverse)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
		if (!m_LaneObjectData.TryGetBuffer(m_Lane, ref val) || val.Length == 0)
		{
			return;
		}
		distance -= 0.9f;
		Controller controller = default(Controller);
		for (int i = 0; i < val.Length; i++)
		{
			LaneObject laneObject = val[i];
			if (!(laneObject.m_LaneObject == m_Entity))
			{
				float2 curvePosition = laneObject.m_CurvePosition;
				bool flag = curvePosition.y < curvePosition.x;
				bool flag2 = false;
				flag2 = (inverse ? ((!flag) ? (curvePosition.x >= minOffset.x) : (curvePosition.y >= minOffset.y && (curvePosition.y > 0f || curvePosition.x >= minOffset.x))) : ((!flag) ? (curvePosition.y <= minOffset.y && (curvePosition.y < 1f || curvePosition.x <= minOffset.x)) : (curvePosition.x <= minOffset.x)));
				if (!flag2 && (!m_ControllerData.TryGetComponent(laneObject.m_LaneObject, ref controller) || !(controller.m_Controller == m_Entity)))
				{
					float objectSpeed = GetObjectSpeed(laneObject.m_LaneObject, curvePosition.x);
					objectSpeed = math.select(objectSpeed, 0f - objectSpeed, inverse);
					BlockerType blockerType = ((inverse == flag) ? BlockerType.Continuing : BlockerType.Oncoming);
					UpdateMaxSpeed(laneObject.m_LaneObject, blockerType, objectSpeed, curvePosition.x, 1f, distance, laneObject.m_LaneObject == m_Ignore, inverse, flag, m_CurrentPosition);
				}
			}
		}
	}

	private void CheckCurrentLane(float distance, float2 minOffset, bool inverse, ref float canUseLane)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
		if (!m_LaneObjectData.TryGetBuffer(m_Lane, ref val) || val.Length == 0)
		{
			return;
		}
		distance -= 0.9f;
		Controller controller = default(Controller);
		ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
		for (int i = 0; i < val.Length; i++)
		{
			LaneObject laneObject = val[i];
			if (laneObject.m_LaneObject == m_Entity || (m_ControllerData.TryGetComponent(laneObject.m_LaneObject, ref controller) && controller.m_Controller == m_Entity))
			{
				continue;
			}
			float2 curvePosition = laneObject.m_CurvePosition;
			bool flag = curvePosition.y < curvePosition.x;
			bool flag2 = false;
			if (inverse ? ((!flag) ? (curvePosition.x >= minOffset.x) : (curvePosition.y >= minOffset.y && (curvePosition.y > 0f || curvePosition.x >= minOffset.x))) : ((!flag) ? (curvePosition.y <= minOffset.y && (curvePosition.y < 1f || curvePosition.x <= minOffset.x)) : (curvePosition.x <= minOffset.x)))
			{
				PrefabRef prefabRef = m_PrefabRefData[laneObject.m_LaneObject];
				float num = 0f;
				if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
				{
					num = 0f - objectGeometryData.m_Bounds.max.z;
				}
				if ((curvePosition.x - minOffset.x) * m_Curve.m_Length > num)
				{
					canUseLane = 0f;
				}
			}
			else
			{
				float objectSpeed = GetObjectSpeed(laneObject.m_LaneObject, curvePosition.x);
				objectSpeed = math.select(objectSpeed, 0f - objectSpeed, inverse);
				BlockerType blockerType = ((inverse == flag) ? BlockerType.Continuing : BlockerType.Oncoming);
				UpdateMaxSpeed(laneObject.m_LaneObject, blockerType, objectSpeed, curvePosition.x, 1f, distance, laneObject.m_LaneObject == m_Ignore, inverse, flag, m_CurrentPosition);
			}
		}
	}

	private void CheckOverlappingLanes(float origDistance, float origMinOffset, int yieldOverride, float speedLimit, bool isRoundabout, bool inverse, bool requestSpace)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<LaneOverlap> val = default(DynamicBuffer<LaneOverlap>);
		if (!m_LaneOverlapData.TryGetBuffer(m_Lane, ref val) || val.Length == 0)
		{
			return;
		}
		origDistance -= 0.9f;
		Entity lane = m_Lane;
		Bezier4x3 bezier = m_Curve.m_Bezier;
		float2 curveOffset = m_CurveOffset;
		float length = m_Curve.m_Length;
		float num = 1f;
		int num2 = m_Priority;
		LaneReservation laneReservation = default(LaneReservation);
		if (m_LaneReservationData.TryGetComponent(m_Lane, ref laneReservation))
		{
			int priority = laneReservation.GetPriority();
			num2 = math.select(num2, 106, priority >= 108 && 106 > num2);
		}
		LaneSignal laneSignal = default(LaneSignal);
		DynamicBuffer<LaneObject> val3 = default(DynamicBuffer<LaneObject>);
		Controller controller = default(Controller);
		Car carData = default(Car);
		for (int i = 0; i < val.Length; i++)
		{
			LaneOverlap laneOverlap = val[i];
			float4 val2 = new float4((float)(int)laneOverlap.m_ThisStart, (float)(int)laneOverlap.m_ThisEnd, (float)(int)laneOverlap.m_OtherStart, (float)(int)laneOverlap.m_OtherEnd) * 0.003921569f;
			if (inverse)
			{
				if (val2.x >= curveOffset.x)
				{
					continue;
				}
			}
			else if (val2.y <= curveOffset.x)
			{
				continue;
			}
			m_Lane = laneOverlap.m_Other;
			m_Curve = m_CurveData[m_Lane];
			m_CurveOffset = math.select(((float4)(ref val2)).zw, ((float4)(ref val2)).wz, inverse);
			Segment overlapLine = MathUtils.Line(bezier, ((float4)(ref val2)).xy);
			float num3;
			OverlapFlags overlapFlags;
			OverlapFlags overlapFlags2;
			if (inverse)
			{
				num3 = math.max(0f, val2.y - origMinOffset);
				overlapFlags = OverlapFlags.MergeEnd | OverlapFlags.MergeMiddleEnd;
				overlapFlags2 = OverlapFlags.MergeStart | OverlapFlags.MergeMiddleStart;
			}
			else
			{
				num3 = math.max(0f, origMinOffset - val2.x);
				overlapFlags = OverlapFlags.MergeStart | OverlapFlags.MergeMiddleStart;
				overlapFlags2 = OverlapFlags.MergeEnd | OverlapFlags.MergeMiddleEnd;
			}
			if (isRoundabout && laneOverlap.m_PriorityDelta > 0 && (laneOverlap.m_Flags & OverlapFlags.Road) != 0 && val2.x >= curveOffset.x)
			{
				num = (val2.x = math.min(num, val2.x));
			}
			float num4 = origDistance + length * math.select(val2.x - curveOffset.x, curveOffset.x - val2.y, inverse);
			float distanceFactor = (float)(int)laneOverlap.m_Parallelism * (1f / 128f);
			bool flag = VehicleUtils.GetBrakingDistance(m_PrefabCar, m_CurrentSpeed, m_TimeStep) <= num4;
			int num5 = num2;
			BlockerType blockerType = (((laneOverlap.m_Flags & overlapFlags2) != 0) ? BlockerType.Continuing : BlockerType.Crossing);
			if ((laneOverlap.m_Flags & overlapFlags) == 0)
			{
				if (inverse ? (val2.y >= origMinOffset) : (val2.x <= origMinOffset))
				{
					if (isRoundabout)
					{
						if (!m_TempBuffer.IsCreated)
						{
							m_TempBuffer = new NativeList<Entity>(16, AllocatorHandle.op_Implicit((Allocator)2));
						}
						m_TempBuffer.Add(ref m_Lane);
					}
				}
				else
				{
					if (isRoundabout && m_TempBuffer.IsCreated)
					{
						int num6 = 0;
						while (num6 < m_TempBuffer.Length)
						{
							if (!(m_TempBuffer[num6] == m_Lane))
							{
								num6++;
								continue;
							}
							goto IL_04e8;
						}
					}
					int num7 = yieldOverride;
					if (m_LaneSignalData.TryGetComponent(m_Lane, ref laneSignal))
					{
						switch (laneSignal.m_Signal)
						{
						case LaneSignalType.Stop:
							num7++;
							break;
						case LaneSignalType.Yield:
							num7--;
							break;
						}
					}
					int num8 = math.select((int)laneOverlap.m_PriorityDelta, num7, num7 != 0);
					num8 = math.select(num8, 0, requestSpace && num8 > 0);
					num5 -= num8;
					if (m_LaneReservationData.TryGetComponent(m_Lane, ref laneReservation))
					{
						float offset = laneReservation.GetOffset();
						float num9 = math.select(math.max(num3 + val2.z, m_CurveOffset.x), 0f, inverse);
						int priority2 = laneReservation.GetPriority();
						if (offset > num9 || priority2 > num5)
						{
							float maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, num4, m_SafeTimeStep);
							maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
							if (maxBrakingSpeed < m_MaxSpeed)
							{
								m_MaxSpeed = maxBrakingSpeed;
								m_Blocker = Entity.Null;
								m_BlockerType = blockerType;
							}
						}
						else if (math.select(math.select((int)laneOverlap.m_PriorityDelta, yieldOverride, num7 != 0), 1, (laneOverlap.m_Flags & OverlapFlags.Slow) != 0) > 0)
						{
							float maxBrakingSpeed2 = VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, num4, m_SafeTimeStep);
							if (maxBrakingSpeed2 >= speedLimit * 0.5f && maxBrakingSpeed2 < m_MaxSpeed)
							{
								m_MaxSpeed = maxBrakingSpeed2;
								m_Blocker = Entity.Null;
								m_BlockerType = blockerType;
							}
						}
						if (flag && priority2 == 96 && !CheckOverlapSpace(lane, curveOffset, m_NextLane, m_NextOffset, ((float4)(ref val2)).xy, out var blocker))
						{
							float maxBrakingSpeed3 = VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, num4, m_SafeTimeStep);
							if (maxBrakingSpeed3 < m_MaxSpeed)
							{
								m_MaxSpeed = maxBrakingSpeed3;
								m_Blocker = blocker;
								m_BlockerType = blockerType;
							}
						}
					}
				}
			}
			goto IL_04e8;
			IL_04e8:
			if (!m_LaneObjectData.TryGetBuffer(m_Lane, ref val3) || val3.Length == 0)
			{
				continue;
			}
			int num10 = 100;
			bool giveSpace = flag && num10 > num5;
			for (int j = 0; j < val3.Length; j++)
			{
				LaneObject laneObject = val3[j];
				if (laneObject.m_LaneObject == m_Entity)
				{
					continue;
				}
				Entity val4 = laneObject.m_LaneObject;
				if (m_ControllerData.TryGetComponent(laneObject.m_LaneObject, ref controller))
				{
					if (controller.m_Controller == m_Entity)
					{
						continue;
					}
					val4 = controller.m_Controller;
				}
				if (m_CreatureData.HasComponent(laneObject.m_LaneObject))
				{
					CheckPedestrian(overlapLine, laneObject.m_LaneObject, laneObject.m_CurvePosition.y, num4, giveSpace, inverse);
					continue;
				}
				float2 curvePosition = laneObject.m_CurvePosition;
				bool flag2 = curvePosition.y < curvePosition.x;
				float objectSpeed = GetObjectSpeed(laneObject.m_LaneObject, curvePosition.x);
				if ((laneOverlap.m_Flags & overlapFlags) == 0 && ((inverse ? (val2.y <= origMinOffset) : (val2.x >= origMinOffset)) | (flag2 ? (curvePosition.y < val2.w) : (curvePosition.y > val2.z))))
				{
					int num11;
					if (m_CarData.TryGetComponent(val4, ref carData))
					{
						num11 = VehicleUtils.GetPriority(carData);
					}
					else if (m_TrainData.HasComponent(laneObject.m_LaneObject))
					{
						PrefabRef prefabRef = m_PrefabRefData[laneObject.m_LaneObject];
						num11 = VehicleUtils.GetPriority(m_PrefabTrainData[prefabRef.m_Prefab]);
					}
					else
					{
						num11 = 0;
					}
					if (num11 - num5 > 0)
					{
						curvePosition.y += objectSpeed * 2f / math.max(1f, m_Curve.m_Length);
					}
				}
				if (flag2)
				{
					if (curvePosition.y >= val2.w - num3)
					{
						continue;
					}
				}
				else if (curvePosition.y <= val2.z + num3)
				{
					continue;
				}
				objectSpeed = math.select(objectSpeed, 0f - objectSpeed, inverse);
				float3 currentPos = MathUtils.Position(m_Curve.m_Bezier, math.select(m_CurveOffset.x, m_CurveOffset.y, flag2 != inverse));
				UpdateMaxSpeed(laneObject.m_LaneObject, blockerType, objectSpeed, curvePosition.x, distanceFactor, num4, laneObject.m_LaneObject == m_Ignore, inverse, flag2, currentPos);
			}
		}
	}

	private float GetObjectSpeed(Entity obj, float curveOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Moving moving = default(Moving);
		if (!m_MovingData.TryGetComponent(obj, ref moving))
		{
			return 0f;
		}
		float3 val = math.normalizesafe(MathUtils.Tangent(m_Curve.m_Bezier, curveOffset), default(float3));
		return math.dot(moving.m_Velocity, val);
	}

	private void CheckPedestrian(Segment overlapLine, Entity obj, float targetOffset, float distanceOffset, bool giveSpace, bool inverse)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		float2 val = math.select(m_CurveOffset, ((float2)(ref m_CurveOffset)).yx, inverse);
		if ((targetOffset <= val.x) | (targetOffset >= val.y))
		{
			PrefabRef prefabRef = m_PrefabRefData[obj];
			Transform transform = m_TransformData[obj];
			float num = m_PrefabObjectGeometry.m_Size.x * 0.5f;
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
			{
				num += objectGeometryData.m_Size.z * 0.5f;
			}
			float num3 = default(float);
			float num2 = MathUtils.Distance(((Segment)(ref overlapLine)).xz, ((float3)(ref transform.m_Position)).xz, ref num3);
			float3 val2 = math.forward(transform.m_Rotation);
			float2 xz = ((float3)(ref val2)).xz;
			val2 = MathUtils.Position(overlapLine, num3);
			float num4 = math.dot(xz, math.normalizesafe(((float3)(ref val2)).xz - ((float3)(ref transform.m_Position)).xz, default(float2)));
			if (num2 - math.select(math.min(0f - num4, 0f), math.max(num4, 0f), giveSpace) >= num)
			{
				return;
			}
		}
		Moving moving = default(Moving);
		float num5 = ((m_PushBlockers || !m_MovingData.TryGetComponent(obj, ref moving) || !(math.lengthsq(moving.m_Velocity) >= 0.01f)) ? math.max(3f, VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, distanceOffset, 3f, m_SafeTimeStep)) : VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, distanceOffset, m_SafeTimeStep));
		num5 = MathUtils.Clamp(num5, m_SpeedRange);
		if (num5 < m_MaxSpeed)
		{
			m_MaxSpeed = num5;
			m_Blocker = obj;
			m_BlockerType = BlockerType.Temporary;
		}
	}

	private void UpdateMaxSpeed(Entity obj, BlockerType blockerType, float objectSpeed, float laneOffset, float distanceFactor, float distanceOffset, bool ignore, bool inverse1, bool inverse2, float3 currentPos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef prefabRef = m_PrefabRefData[obj];
		float num = 0f;
		bool flag = false;
		TrainData prefabTrainData = default(TrainData);
		ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
		if (m_PrefabTrainData.TryGetComponent(prefabRef.m_Prefab, ref prefabTrainData))
		{
			Train train = m_TrainData[obj];
			float2 val = prefabTrainData.m_AttachOffsets - prefabTrainData.m_BogieOffsets;
			num = math.select(val.y, val.x, (train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0);
			flag = true;
		}
		else if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
		{
			num = 0f - objectGeometryData.m_Bounds.min.z;
		}
		float2 val2 = math.select(m_CurveOffset, ((float2)(ref m_CurveOffset)).yx, inverse1 != inverse2);
		val2 = math.select(laneOffset - val2, val2 - laneOffset, inverse2);
		if (val2.y * m_Curve.m_Length >= num)
		{
			return;
		}
		val2.x = math.min(0f, val2.x);
		Transform transform = m_TransformData[obj];
		float num2 = math.distance(MathUtils.Position(m_Curve.m_Bezier, laneOffset + math.select(0f - val2.x, val2.x, inverse2)), currentPos);
		num2 += val2.x * m_Curve.m_Length;
		num2 = ((!(math.dot(transform.m_Position - currentPos, currentPos - m_PrevPosition) < 0f)) ? math.min(num2, math.distance(transform.m_Position, currentPos)) : math.min(num2, math.distance(transform.m_Position, m_PrevPosition) + m_PrevDistance - m_Distance));
		num2 -= num;
		num2 *= distanceFactor;
		num2 += distanceOffset;
		CarData prefabCarData = default(CarData);
		float maxBrakingSpeed;
		if (objectSpeed > 0.001f && m_PrefabCarData.TryGetComponent(prefabRef.m_Prefab, ref prefabCarData))
		{
			objectSpeed = math.max(0f, objectSpeed - prefabCarData.m_Braking * m_TimeStep * 2f) * distanceFactor;
			if (m_PrefabCar.m_Braking >= prefabCarData.m_Braking)
			{
				num2 += objectSpeed * m_SafeTimeStep;
				maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, num2, objectSpeed, m_SafeTimeStep);
			}
			else
			{
				num2 += VehicleUtils.GetBrakingDistance(prefabCarData, objectSpeed, m_SafeTimeStep);
				maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, num2, m_SafeTimeStep);
			}
		}
		else if (objectSpeed > 0.001f && flag)
		{
			objectSpeed = math.max(0f, objectSpeed - prefabTrainData.m_Braking * m_TimeStep * 2f) * distanceFactor;
			if (m_PrefabCar.m_Braking >= prefabTrainData.m_Braking)
			{
				num2 += objectSpeed * m_SafeTimeStep;
				maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, num2, objectSpeed, m_SafeTimeStep);
			}
			else
			{
				num2 += VehicleUtils.GetBrakingDistance(prefabTrainData, objectSpeed, m_SafeTimeStep);
				maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, num2, m_SafeTimeStep);
			}
		}
		else
		{
			maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(m_PrefabCar, num2, m_SafeTimeStep);
		}
		if (blockerType == BlockerType.Oncoming)
		{
			float num3 = 2f - maxBrakingSpeed * (1f / 6f);
			m_Oncoming = math.max(m_Oncoming, num3);
			maxBrakingSpeed = math.max(maxBrakingSpeed, 3f);
			maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
			if (maxBrakingSpeed < m_MaxSpeed)
			{
				m_MaxSpeed = maxBrakingSpeed;
				m_Blocker = Entity.Null;
				m_BlockerType = blockerType;
			}
		}
		else
		{
			maxBrakingSpeed = math.select(maxBrakingSpeed, 3f, ignore && maxBrakingSpeed < 3f);
			maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, m_SpeedRange);
			if (maxBrakingSpeed < m_MaxSpeed)
			{
				m_MaxSpeed = maxBrakingSpeed;
				m_Blocker = obj;
				m_BlockerType = blockerType;
			}
		}
	}
}

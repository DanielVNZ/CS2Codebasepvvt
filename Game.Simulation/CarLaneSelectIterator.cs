using Colossal;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation;

public struct CarLaneSelectIterator
{
	public ComponentLookup<Owner> m_OwnerData;

	public ComponentLookup<Lane> m_LaneData;

	public ComponentLookup<CarLane> m_CarLaneData;

	public ComponentLookup<SlaveLane> m_SlaveLaneData;

	public ComponentLookup<LaneReservation> m_LaneReservationData;

	public ComponentLookup<Moving> m_MovingData;

	public ComponentLookup<Car> m_CarData;

	public ComponentLookup<Controller> m_ControllerData;

	public BufferLookup<SubLane> m_Lanes;

	public BufferLookup<LaneObject> m_LaneObjects;

	public Entity m_Entity;

	public Entity m_Blocker;

	public int m_Priority;

	public Game.Net.CarLaneFlags m_ForbidLaneFlags;

	public Game.Net.CarLaneFlags m_PreferLaneFlags;

	private NativeArray<float> m_Buffer;

	private int m_BufferPos;

	private float m_LaneSwitchCost;

	private float m_LaneSwitchBaseCost;

	private Entity m_PrevLane;

	public void SetBuffer(ref CarLaneSelectBuffer buffer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_Buffer = buffer.Ensure();
	}

	public void CalculateLaneCosts(CarNavigationLane navLaneData, int index)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		if ((navLaneData.m_Flags & (Game.Vehicles.CarLaneFlags.Reserved | Game.Vehicles.CarLaneFlags.FixedLane)) != 0 || !m_SlaveLaneData.HasComponent(navLaneData.m_Lane))
		{
			return;
		}
		SlaveLane slaveLane = m_SlaveLaneData[navLaneData.m_Lane];
		Owner owner = m_OwnerData[navLaneData.m_Lane];
		DynamicBuffer<SubLane> val = m_Lanes[owner.m_Owner];
		int num = math.min((int)slaveLane.m_MaxIndex, val.Length - 1);
		float laneObjectCost = math.abs(navLaneData.m_CurvePosition.y - navLaneData.m_CurvePosition.x) * 0.49f;
		for (int i = slaveLane.m_MinIndex; i <= num; i++)
		{
			Entity subLane = val[i].m_SubLane;
			float num2 = CalculateLaneObjectCost(laneObjectCost, index, subLane, navLaneData.m_Flags);
			if (m_LaneReservationData.HasComponent(subLane))
			{
				num2 += GetLanePriorityCost(m_LaneReservationData[subLane].GetPriority());
			}
			if (m_CarLaneData.HasComponent(subLane))
			{
				num2 += GetLaneDriveCost(m_CarLaneData[subLane].m_Flags);
			}
			m_Buffer[m_BufferPos++] = num2;
		}
	}

	private float CalculateLaneObjectCost(float laneObjectCost, int index, Entity lane, Game.Vehicles.CarLaneFlags laneFlags)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (m_LaneObjects.HasBuffer(lane))
		{
			DynamicBuffer<LaneObject> val = m_LaneObjects[lane];
			if (index < 2 && m_Blocker != Entity.Null)
			{
				for (int i = 0; i < val.Length; i++)
				{
					LaneObject laneObject = val[i];
					num = ((!(laneObject.m_LaneObject == m_Blocker)) ? (num + laneObjectCost) : (num + CalculateLaneObjectCost(laneObject, laneObjectCost, laneFlags)));
				}
			}
			else
			{
				num += laneObjectCost * (float)val.Length;
			}
		}
		return num;
	}

	private float CalculateLaneObjectCost(float laneObjectCost, Entity lane, float minCurvePosition, Game.Vehicles.CarLaneFlags laneFlags)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (m_LaneObjects.HasBuffer(lane))
		{
			DynamicBuffer<LaneObject> val = m_LaneObjects[lane];
			for (int i = 0; i < val.Length; i++)
			{
				LaneObject laneObject = val[i];
				if (laneObject.m_CurvePosition.y > minCurvePosition && !(laneObject.m_LaneObject == m_Entity) && (!m_ControllerData.HasComponent(laneObject.m_LaneObject) || !(m_ControllerData[laneObject.m_LaneObject].m_Controller == m_Entity)))
				{
					num += CalculateLaneObjectCost(laneObject, laneObjectCost, laneFlags);
				}
			}
		}
		return num;
	}

	private float CalculateLaneObjectCost(LaneObject laneObject, float laneObjectCost, Game.Vehicles.CarLaneFlags laneFlags)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!m_MovingData.HasComponent(laneObject.m_LaneObject))
		{
			if (m_CarData.HasComponent(laneObject.m_LaneObject) && (m_CarData[laneObject.m_LaneObject].m_Flags & CarFlags.Queueing) != 0 && (laneFlags & Game.Vehicles.CarLaneFlags.Queue) != 0)
			{
				return laneObjectCost;
			}
			return math.lerp(10000000f, 9000000f, laneObject.m_CurvePosition.y);
		}
		return laneObjectCost;
	}

	public void CalculateLaneCosts(CarNavigationLane navLaneData, CarNavigationLane nextNavLaneData, int index)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		SlaveLane slaveLane = default(SlaveLane);
		if ((navLaneData.m_Flags & (Game.Vehicles.CarLaneFlags.Reserved | Game.Vehicles.CarLaneFlags.FixedLane)) == 0 && m_SlaveLaneData.TryGetComponent(navLaneData.m_Lane, ref slaveLane))
		{
			Owner owner = m_OwnerData[navLaneData.m_Lane];
			DynamicBuffer<SubLane> val = m_Lanes[owner.m_Owner];
			int num = math.min((int)slaveLane.m_MaxIndex, val.Length - 1);
			m_LaneSwitchCost = m_LaneSwitchBaseCost + math.select(1f, 5f, (slaveLane.m_Flags & SlaveLaneFlags.AllowChange) == 0);
			float laneObjectCost = math.abs(navLaneData.m_CurvePosition.y - navLaneData.m_CurvePosition.x) * 0.49f * (2f / (float)(2 + index));
			SlaveLane slaveLane2 = default(SlaveLane);
			if ((nextNavLaneData.m_Flags & (Game.Vehicles.CarLaneFlags.Reserved | Game.Vehicles.CarLaneFlags.FixedLane)) == 0 && m_SlaveLaneData.TryGetComponent(nextNavLaneData.m_Lane, ref slaveLane2))
			{
				Owner owner2 = m_OwnerData[nextNavLaneData.m_Lane];
				DynamicBuffer<SubLane> val2 = m_Lanes[owner2.m_Owner];
				int num2 = math.min((int)slaveLane2.m_MaxIndex, val2.Length - 1);
				int num3 = m_BufferPos - (num2 - slaveLane2.m_MinIndex + 1);
				LaneReservation laneReservation = default(LaneReservation);
				CarLane carLane = default(CarLane);
				for (int i = slaveLane.m_MinIndex; i <= num; i++)
				{
					Entity subLane = val[i].m_SubLane;
					Lane lane = m_LaneData[subLane];
					float num4 = 1000000f;
					int num5;
					int num6;
					if ((nextNavLaneData.m_Flags & Game.Vehicles.CarLaneFlags.GroupTarget) != 0)
					{
						num5 = slaveLane2.m_MinIndex;
						num6 = num2;
					}
					else
					{
						num5 = 100000;
						num6 = -100000;
						if ((slaveLane.m_Flags & SlaveLaneFlags.MiddleEnd) != 0)
						{
							for (int j = slaveLane2.m_MinIndex; j <= num2; j++)
							{
								Lane lane2 = m_LaneData[val2[j].m_SubLane];
								if (lane.m_EndNode.EqualsIgnoreCurvePos(lane2.m_MiddleNode))
								{
									num5 = math.min(num5, j);
									num6 = j;
								}
							}
						}
						else if ((slaveLane2.m_Flags & SlaveLaneFlags.MiddleStart) != 0)
						{
							for (int k = slaveLane2.m_MinIndex; k <= num2; k++)
							{
								Lane lane3 = m_LaneData[val2[k].m_SubLane];
								if (lane.m_MiddleNode.EqualsIgnoreCurvePos(lane3.m_StartNode))
								{
									num5 = math.min(num5, k);
									num6 = k;
								}
							}
						}
						else
						{
							for (int l = slaveLane2.m_MinIndex; l <= num2; l++)
							{
								Lane lane4 = m_LaneData[val2[l].m_SubLane];
								if (lane.m_EndNode.Equals(lane4.m_StartNode))
								{
									num5 = math.min(num5, l);
									num6 = l;
								}
							}
						}
					}
					if (num5 <= num6)
					{
						int num7 = num3;
						for (int m = slaveLane2.m_MinIndex; m < num5; m++)
						{
							num4 = math.min(num4, m_Buffer[num7++] + GetLaneSwitchCost(num5 - m));
						}
						for (int n = num5; n <= num6; n++)
						{
							num4 = math.min(num4, m_Buffer[num7++]);
						}
						for (int num8 = num6 + 1; num8 <= num2; num8++)
						{
							num4 = math.min(num4, m_Buffer[num7++] + GetLaneSwitchCost(num8 - num6));
						}
						num4 += CalculateLaneObjectCost(laneObjectCost, index, subLane, navLaneData.m_Flags);
						if (m_LaneReservationData.TryGetComponent(subLane, ref laneReservation))
						{
							num4 += GetLanePriorityCost(laneReservation.GetPriority());
						}
						if (m_CarLaneData.TryGetComponent(subLane, ref carLane))
						{
							num4 += GetLaneDriveCost(carLane.m_Flags);
						}
					}
					m_Buffer[m_BufferPos++] = num4;
				}
			}
			else if ((nextNavLaneData.m_Flags & Game.Vehicles.CarLaneFlags.TransformTarget) != 0)
			{
				LaneReservation laneReservation2 = default(LaneReservation);
				CarLane carLane2 = default(CarLane);
				for (int num9 = slaveLane.m_MinIndex; num9 <= num; num9++)
				{
					Entity subLane2 = val[num9].m_SubLane;
					float num10 = CalculateLaneObjectCost(laneObjectCost, index, subLane2, navLaneData.m_Flags);
					if (m_LaneReservationData.TryGetComponent(subLane2, ref laneReservation2))
					{
						num10 += GetLanePriorityCost(laneReservation2.GetPriority());
					}
					if (m_CarLaneData.TryGetComponent(subLane2, ref carLane2))
					{
						num10 += GetLaneDriveCost(carLane2.m_Flags);
					}
					m_Buffer[m_BufferPos++] = num10;
				}
			}
			else
			{
				int num11 = 100000;
				int num12 = -100000;
				if ((nextNavLaneData.m_Flags & Game.Vehicles.CarLaneFlags.GroupTarget) != 0)
				{
					for (int num13 = slaveLane.m_MinIndex; num13 <= num; num13++)
					{
						if (val[num13].m_SubLane == nextNavLaneData.m_Lane)
						{
							num11 = num13;
							num12 = num13;
							break;
						}
					}
				}
				else
				{
					Lane lane5 = m_LaneData[nextNavLaneData.m_Lane];
					for (int num14 = slaveLane.m_MinIndex; num14 <= num; num14++)
					{
						Lane lane6 = m_LaneData[val[num14].m_SubLane];
						if ((slaveLane.m_Flags & SlaveLaneFlags.MiddleEnd) != 0)
						{
							if (lane6.m_EndNode.EqualsIgnoreCurvePos(lane5.m_MiddleNode))
							{
								num11 = math.min(num11, num14);
								num12 = num14;
							}
						}
						else if (lane6.m_EndNode.Equals(lane5.m_StartNode))
						{
							num11 = math.min(num11, num14);
							num12 = num14;
						}
					}
				}
				LaneReservation laneReservation3 = default(LaneReservation);
				CarLane carLane3 = default(CarLane);
				for (int num15 = slaveLane.m_MinIndex; num15 <= num; num15++)
				{
					Entity subLane3 = val[num15].m_SubLane;
					float num16 = 0f;
					if (num11 <= num12)
					{
						num16 += GetLaneSwitchCost(math.max(0, math.max(num11 - num15, num15 - num12)));
					}
					num16 += CalculateLaneObjectCost(laneObjectCost, index, subLane3, navLaneData.m_Flags);
					if (m_LaneReservationData.TryGetComponent(subLane3, ref laneReservation3))
					{
						num16 += GetLanePriorityCost(laneReservation3.GetPriority());
					}
					if (m_CarLaneData.TryGetComponent(subLane3, ref carLane3))
					{
						num16 += GetLaneDriveCost(carLane3.m_Flags);
					}
					m_Buffer[m_BufferPos++] = num16;
				}
			}
		}
		m_LaneSwitchBaseCost += 0.01f;
	}

	private float GetLaneSwitchCost(int numLanes)
	{
		return (float)(numLanes * numLanes * numLanes) * m_LaneSwitchCost;
	}

	private float GetLanePriorityCost(int lanePriority)
	{
		return (float)math.max(0, lanePriority - m_Priority) * 1f;
	}

	private float GetLaneDriveCost(Game.Net.CarLaneFlags flags)
	{
		float num = math.select(0f, 0.4f, ((flags & m_PreferLaneFlags) == 0) & (m_PreferLaneFlags != ~(Game.Net.CarLaneFlags.Unsafe | Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.Invert | Game.Net.CarLaneFlags.SideConnection | Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.Twoway | Game.Net.CarLaneFlags.IsSecured | Game.Net.CarLaneFlags.Runway | Game.Net.CarLaneFlags.Yield | Game.Net.CarLaneFlags.Stop | Game.Net.CarLaneFlags.ForbidCombustionEngines | Game.Net.CarLaneFlags.ForbidTransitTraffic | Game.Net.CarLaneFlags.ForbidHeavyTraffic | Game.Net.CarLaneFlags.PublicOnly | Game.Net.CarLaneFlags.Highway | Game.Net.CarLaneFlags.UTurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight | Game.Net.CarLaneFlags.Forward | Game.Net.CarLaneFlags.Approach | Game.Net.CarLaneFlags.Roundabout | Game.Net.CarLaneFlags.RightLimit | Game.Net.CarLaneFlags.LeftLimit | Game.Net.CarLaneFlags.ForbidPassing | Game.Net.CarLaneFlags.RightOfWay | Game.Net.CarLaneFlags.TrafficLights | Game.Net.CarLaneFlags.ParkingLeft | Game.Net.CarLaneFlags.ParkingRight | Game.Net.CarLaneFlags.Forbidden | Game.Net.CarLaneFlags.AllowEnter)));
		float num2 = math.select(0.9f, 4.9f, m_Priority < 108);
		return math.select(num, num2, (flags & m_ForbidLaneFlags) != 0);
	}

	public void UpdateOptimalLane(ref CarCurrentLane currentLane, CarNavigationLane nextNavLaneData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		Entity val = ((currentLane.m_ChangeLane != Entity.Null) ? currentLane.m_ChangeLane : currentLane.m_Lane);
		int2 val2 = int2.op_Implicit(0);
		int changeIndex = 0;
		SlaveLane slaveLane = default(SlaveLane);
		if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.FixedLane) == 0 && m_SlaveLaneData.TryGetComponent(val, ref slaveLane))
		{
			Owner owner = m_OwnerData[val];
			DynamicBuffer<SubLane> val3 = m_Lanes[owner.m_Owner];
			int num = math.min((int)slaveLane.m_MaxIndex, val3.Length - 1);
			m_LaneSwitchCost = m_LaneSwitchBaseCost + math.select(1f, 5f, (slaveLane.m_Flags & SlaveLaneFlags.AllowChange) == 0);
			float laneObjectCost = 0.49f;
			for (int i = slaveLane.m_MinIndex; i <= num; i++)
			{
				if (val3[i].m_SubLane == val)
				{
					val2 = int2.op_Implicit(i);
					break;
				}
			}
			if (currentLane.m_ChangeLane != Entity.Null)
			{
				for (int j = slaveLane.m_MinIndex; j <= num; j++)
				{
					if (val3[j].m_SubLane == currentLane.m_Lane)
					{
						val2.y = j;
						break;
					}
				}
			}
			float num2 = float.MaxValue;
			SlaveLane slaveLane2 = default(SlaveLane);
			if ((nextNavLaneData.m_Flags & (Game.Vehicles.CarLaneFlags.Reserved | Game.Vehicles.CarLaneFlags.FixedLane)) == 0 && m_SlaveLaneData.TryGetComponent(nextNavLaneData.m_Lane, ref slaveLane2))
			{
				Owner owner2 = m_OwnerData[nextNavLaneData.m_Lane];
				DynamicBuffer<SubLane> val4 = m_Lanes[owner2.m_Owner];
				int num3 = math.min((int)slaveLane2.m_MaxIndex, val4.Length - 1);
				int num4 = m_BufferPos - (num3 - slaveLane2.m_MinIndex + 1);
				LaneReservation laneReservation = default(LaneReservation);
				for (int k = slaveLane.m_MinIndex; k <= num; k++)
				{
					Entity subLane = val3[k].m_SubLane;
					Lane lane = m_LaneData[subLane];
					float num5 = 1000000f;
					int num6;
					int num7;
					if ((nextNavLaneData.m_Flags & Game.Vehicles.CarLaneFlags.GroupTarget) != 0)
					{
						num6 = slaveLane2.m_MinIndex;
						num7 = num3;
					}
					else
					{
						num6 = 100000;
						num7 = -100000;
						if ((slaveLane.m_Flags & SlaveLaneFlags.MiddleEnd) != 0)
						{
							for (int l = slaveLane2.m_MinIndex; l <= num3; l++)
							{
								Lane lane2 = m_LaneData[val4[l].m_SubLane];
								if (lane.m_EndNode.EqualsIgnoreCurvePos(lane2.m_MiddleNode))
								{
									num6 = math.min(num6, l);
									num7 = l;
								}
							}
						}
						else if ((slaveLane2.m_Flags & SlaveLaneFlags.MiddleStart) != 0)
						{
							for (int m = slaveLane2.m_MinIndex; m <= num3; m++)
							{
								Lane lane3 = m_LaneData[val4[m].m_SubLane];
								if (lane.m_MiddleNode.EqualsIgnoreCurvePos(lane3.m_StartNode))
								{
									num6 = math.min(num6, m);
									num7 = m;
								}
							}
						}
						else
						{
							for (int n = slaveLane2.m_MinIndex; n <= num3; n++)
							{
								Lane lane4 = m_LaneData[val4[n].m_SubLane];
								if (lane.m_EndNode.Equals(lane4.m_StartNode))
								{
									num6 = math.min(num6, n);
									num7 = n;
								}
							}
						}
					}
					if (num6 <= num7)
					{
						int num8 = num4 + (num6 - slaveLane2.m_MinIndex);
						for (int num9 = num6; num9 <= num7; num9++)
						{
							num5 = math.min(num5, m_Buffer[num8++]);
						}
						num5 += CalculateLaneObjectCost(laneObjectCost, subLane, currentLane.m_CurvePosition.x, currentLane.m_LaneFlags);
						if (m_LaneReservationData.TryGetComponent(subLane, ref laneReservation))
						{
							num5 += GetLanePriorityCost(laneReservation.GetPriority());
						}
					}
					int2 val5 = math.abs(k - val2);
					num5 += GetLaneSwitchCost(math.select(val5.x, val5.y, val5.x != 0 && val5.y > val5.x));
					if (num5 < num2)
					{
						num2 = num5;
						val = subLane;
						changeIndex = k;
					}
				}
			}
			else if ((nextNavLaneData.m_Flags & Game.Vehicles.CarLaneFlags.TransformTarget) != 0 || nextNavLaneData.m_Lane == Entity.Null)
			{
				LaneReservation laneReservation2 = default(LaneReservation);
				for (int num10 = slaveLane.m_MinIndex; num10 <= num; num10++)
				{
					Entity subLane2 = val3[num10].m_SubLane;
					float num11 = CalculateLaneObjectCost(laneObjectCost, subLane2, currentLane.m_CurvePosition.x, currentLane.m_LaneFlags);
					if (m_LaneReservationData.TryGetComponent(subLane2, ref laneReservation2))
					{
						num11 += GetLanePriorityCost(laneReservation2.GetPriority());
					}
					int2 val6 = math.abs(num10 - val2);
					num11 += GetLaneSwitchCost(math.select(val6.x, val6.y, val6.x != 0 && val6.y > val6.x));
					if (num11 < num2)
					{
						num2 = num11;
						val = subLane2;
						changeIndex = num10;
					}
				}
			}
			else
			{
				int num12 = 100000;
				int num13 = -100000;
				if ((nextNavLaneData.m_Flags & Game.Vehicles.CarLaneFlags.GroupTarget) != 0)
				{
					for (int num14 = slaveLane.m_MinIndex; num14 <= num; num14++)
					{
						if (val3[num14].m_SubLane == nextNavLaneData.m_Lane)
						{
							num12 = num14;
							num13 = num14;
							break;
						}
					}
				}
				else
				{
					Lane lane5 = m_LaneData[nextNavLaneData.m_Lane];
					for (int num15 = slaveLane.m_MinIndex; num15 <= num; num15++)
					{
						Lane lane6 = m_LaneData[val3[num15].m_SubLane];
						if ((slaveLane.m_Flags & SlaveLaneFlags.MiddleEnd) != 0)
						{
							if (lane6.m_EndNode.EqualsIgnoreCurvePos(lane5.m_MiddleNode))
							{
								num12 = math.min(num12, num15);
								num13 = num15;
							}
						}
						else if (lane6.m_EndNode.Equals(lane5.m_StartNode))
						{
							num12 = math.min(num12, num15);
							num13 = num15;
						}
					}
				}
				LaneReservation laneReservation3 = default(LaneReservation);
				for (int num16 = slaveLane.m_MinIndex; num16 <= num; num16++)
				{
					Entity subLane3 = val3[num16].m_SubLane;
					float num17;
					if ((num16 >= num12 && num16 <= num13) || num12 > num13)
					{
						num17 = CalculateLaneObjectCost(laneObjectCost, subLane3, currentLane.m_CurvePosition.x, currentLane.m_LaneFlags);
						if (m_LaneReservationData.TryGetComponent(subLane3, ref laneReservation3))
						{
							num17 += GetLanePriorityCost(laneReservation3.GetPriority());
						}
					}
					else
					{
						num17 = 1000000f;
					}
					int2 val7 = math.abs(num16 - val2);
					num17 += GetLaneSwitchCost(math.select(val7.x, val7.y, val7.x != 0 && val7.y > val7.x));
					if (num17 < num2)
					{
						num2 = num17;
						val = subLane3;
						changeIndex = num16;
					}
				}
			}
		}
		if (val != currentLane.m_Lane)
		{
			if (val != currentLane.m_ChangeLane)
			{
				currentLane.m_ChangeLane = val;
				currentLane.m_ChangeProgress = 0f;
				currentLane.m_LaneFlags &= ~(Game.Vehicles.CarLaneFlags.TurnLeft | Game.Vehicles.CarLaneFlags.TurnRight);
				currentLane.m_LaneFlags |= GetTurnFlags(currentLane.m_Lane, val2.y, changeIndex);
			}
		}
		else if (currentLane.m_ChangeLane != Entity.Null)
		{
			currentLane.m_LaneFlags &= ~(Game.Vehicles.CarLaneFlags.TurnLeft | Game.Vehicles.CarLaneFlags.TurnRight);
			if (currentLane.m_ChangeProgress == 0f)
			{
				currentLane.m_ChangeLane = Entity.Null;
			}
			else
			{
				currentLane.m_Lane = currentLane.m_ChangeLane;
				currentLane.m_ChangeLane = val;
				currentLane.m_ChangeProgress = math.saturate(1f - currentLane.m_ChangeProgress);
				currentLane.m_LaneFlags |= GetTurnFlags(currentLane.m_Lane, val2.y, changeIndex);
			}
		}
		if (currentLane.m_ChangeLane == Entity.Null)
		{
			m_PrevLane = currentLane.m_Lane;
		}
		else
		{
			m_PrevLane = currentLane.m_ChangeLane;
		}
		m_LaneSwitchCost = 10000000f;
	}

	private Game.Vehicles.CarLaneFlags GetTurnFlags(Entity currentLane, int currentIndex, int changeIndex)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (changeIndex != currentIndex)
		{
			bool flag = false;
			CarLane carLane = default(CarLane);
			if (m_CarLaneData.TryGetComponent(currentLane, ref carLane))
			{
				flag = (carLane.m_Flags & Game.Net.CarLaneFlags.Invert) != 0;
			}
			if (changeIndex < currentIndex != flag)
			{
				return Game.Vehicles.CarLaneFlags.TurnLeft;
			}
			return Game.Vehicles.CarLaneFlags.TurnRight;
		}
		return (Game.Vehicles.CarLaneFlags)0u;
	}

	public void UpdateOptimalLane(ref CarNavigationLane navLaneData)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		if (m_SlaveLaneData.HasComponent(navLaneData.m_Lane))
		{
			SlaveLane slaveLane = m_SlaveLaneData[navLaneData.m_Lane];
			if ((navLaneData.m_Flags & (Game.Vehicles.CarLaneFlags.Reserved | Game.Vehicles.CarLaneFlags.FixedLane | Game.Vehicles.CarLaneFlags.FixedStart)) == 0 && m_LaneData.HasComponent(m_PrevLane))
			{
				Owner owner = m_OwnerData[navLaneData.m_Lane];
				DynamicBuffer<SubLane> val = m_Lanes[owner.m_Owner];
				int num = math.min((int)slaveLane.m_MaxIndex, val.Length - 1);
				m_BufferPos -= num - slaveLane.m_MinIndex + 1;
				int num2 = 100000;
				int num3 = -100000;
				if ((navLaneData.m_Flags & Game.Vehicles.CarLaneFlags.GroupTarget) == 0)
				{
					Lane lane = m_LaneData[m_PrevLane];
					SlaveLane slaveLane2 = default(SlaveLane);
					if (m_SlaveLaneData.HasComponent(m_PrevLane))
					{
						slaveLane2 = m_SlaveLaneData[m_PrevLane];
					}
					if ((slaveLane2.m_Flags & SlaveLaneFlags.MiddleEnd) != 0)
					{
						for (int i = slaveLane.m_MinIndex; i <= num; i++)
						{
							Lane lane2 = m_LaneData[val[i].m_SubLane];
							if (lane.m_EndNode.EqualsIgnoreCurvePos(lane2.m_MiddleNode))
							{
								num2 = math.min(num2, i);
								num3 = i;
							}
						}
					}
					else if ((slaveLane.m_Flags & SlaveLaneFlags.MiddleStart) != 0)
					{
						for (int j = slaveLane.m_MinIndex; j <= num; j++)
						{
							Lane lane3 = m_LaneData[val[j].m_SubLane];
							if (lane.m_MiddleNode.EqualsIgnoreCurvePos(lane3.m_StartNode))
							{
								num2 = math.min(num2, j);
								num3 = j;
							}
						}
					}
					else
					{
						for (int k = slaveLane.m_MinIndex; k <= num; k++)
						{
							Lane lane4 = m_LaneData[val[k].m_SubLane];
							if (lane.m_EndNode.Equals(lane4.m_StartNode))
							{
								num2 = math.min(num2, k);
								num3 = k;
							}
						}
					}
				}
				if (num2 > num3)
				{
					num2 = slaveLane.m_MinIndex;
					num3 = num;
				}
				int bufferPos = m_BufferPos;
				float num4 = float.MaxValue;
				int num5 = slaveLane.m_MinIndex;
				for (int l = slaveLane.m_MinIndex; l < num2; l++)
				{
					float num6 = m_Buffer[bufferPos++] + GetLaneSwitchCost(num2 - l);
					if (num6 < num4)
					{
						num4 = num6;
						num5 = l;
					}
				}
				for (int m = num2; m <= num3; m++)
				{
					float num7 = m_Buffer[bufferPos++];
					if (num7 < num4)
					{
						num4 = num7;
						num5 = m;
					}
				}
				for (int n = num3 + 1; n <= num; n++)
				{
					float num8 = m_Buffer[bufferPos++] + GetLaneSwitchCost(n - num3);
					if (num8 < num4)
					{
						num4 = num8;
						num5 = n;
					}
				}
				navLaneData.m_Lane = val[num5].m_SubLane;
			}
			m_LaneSwitchCost = m_LaneSwitchBaseCost + math.select(1f, 5f, (slaveLane.m_Flags & SlaveLaneFlags.AllowChange) == 0);
		}
		else
		{
			m_LaneSwitchCost = 10000000f;
		}
		m_PrevLane = navLaneData.m_Lane;
		m_LaneSwitchBaseCost -= 0.01f;
	}

	public void DrawLaneCosts(CarCurrentLane currentLaneData, CarNavigationLane nextNavLaneData, ComponentLookup<Curve> curveData, GizmoBatcher gizmoBatcher)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (currentLaneData.m_ChangeProgress == 0f || currentLaneData.m_ChangeLane == Entity.Null)
		{
			m_PrevLane = currentLaneData.m_Lane;
		}
		else
		{
			m_PrevLane = currentLaneData.m_ChangeLane;
		}
	}

	public void DrawLaneCosts(CarNavigationLane navLaneData, ComponentLookup<Curve> curveData, GizmoBatcher gizmoBatcher)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (m_SlaveLaneData.HasComponent(navLaneData.m_Lane))
		{
			SlaveLane slaveLane = m_SlaveLaneData[navLaneData.m_Lane];
			Owner owner = m_OwnerData[navLaneData.m_Lane];
			DynamicBuffer<SubLane> val = m_Lanes[owner.m_Owner];
			int num = math.min((int)slaveLane.m_MaxIndex, val.Length - 1);
			if ((navLaneData.m_Flags & (Game.Vehicles.CarLaneFlags.Reserved | Game.Vehicles.CarLaneFlags.FixedLane)) == 0)
			{
				m_BufferPos -= num - slaveLane.m_MinIndex + 1;
				int bufferPos = m_BufferPos;
				for (int i = slaveLane.m_MinIndex; i <= num; i++)
				{
					float cost = m_Buffer[bufferPos++];
					DrawLane(val[i].m_SubLane, navLaneData.m_CurvePosition, curveData, gizmoBatcher, cost);
				}
			}
			else
			{
				for (int j = slaveLane.m_MinIndex; j <= num; j++)
				{
					Entity subLane = val[j].m_SubLane;
					float cost2 = math.select(1000000f, 0f, subLane == navLaneData.m_Lane);
					DrawLane(subLane, navLaneData.m_CurvePosition, curveData, gizmoBatcher, cost2);
				}
			}
		}
		m_PrevLane = navLaneData.m_Lane;
	}

	private void DrawLane(Entity lane, float2 curvePos, ComponentLookup<Curve> curveData, GizmoBatcher gizmoBatcher, float cost)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Curve curve = curveData[lane];
		Color val;
		if (cost >= 100000f)
		{
			val = Color.black;
		}
		else
		{
			cost = math.sqrt(cost);
			val = ((!(cost < 2f)) ? Color.Lerp(Color.yellow, Color.red, (cost - 2f) * 0.5f) : Color.Lerp(Color.cyan, Color.yellow, cost * 0.5f));
		}
		Bezier4x3 val2 = MathUtils.Cut(curve.m_Bezier, curvePos);
		float num = curve.m_Length * math.abs(curvePos.y - curvePos.x);
		((GizmoBatcher)(ref gizmoBatcher)).DrawCurve(val2, num, val, -1);
	}
}

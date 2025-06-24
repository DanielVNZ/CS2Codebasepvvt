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

public struct WatercraftLaneSelectIterator
{
	public ComponentLookup<Owner> m_OwnerData;

	public ComponentLookup<Lane> m_LaneData;

	public ComponentLookup<SlaveLane> m_SlaveLaneData;

	public ComponentLookup<LaneReservation> m_LaneReservationData;

	public ComponentLookup<Moving> m_MovingData;

	public ComponentLookup<Watercraft> m_WatercraftData;

	public BufferLookup<SubLane> m_Lanes;

	public BufferLookup<LaneObject> m_LaneObjects;

	public Entity m_Entity;

	public Entity m_Blocker;

	public int m_Priority;

	private NativeArray<float> m_Buffer;

	private int m_BufferPos;

	private float m_LaneSwitchCost;

	private float m_LaneSwitchBaseCost;

	private Entity m_PrevLane;

	public void SetBuffer(ref WatercraftLaneSelectBuffer buffer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_Buffer = buffer.Ensure();
	}

	public void CalculateLaneCosts(WatercraftNavigationLane navLaneData, int index)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if ((navLaneData.m_Flags & (WatercraftLaneFlags.Reserved | WatercraftLaneFlags.FixedLane)) == 0 && m_SlaveLaneData.HasComponent(navLaneData.m_Lane))
		{
			SlaveLane slaveLane = m_SlaveLaneData[navLaneData.m_Lane];
			Owner owner = m_OwnerData[navLaneData.m_Lane];
			DynamicBuffer<SubLane> val = m_Lanes[owner.m_Owner];
			int num = math.min((int)slaveLane.m_MaxIndex, val.Length - 1);
			float laneObjectCost = math.abs(navLaneData.m_CurvePosition.y - navLaneData.m_CurvePosition.x) * 0.49f;
			for (int i = slaveLane.m_MinIndex; i <= num; i++)
			{
				Entity subLane = val[i].m_SubLane;
				float num2 = CalculateLaneObjectCost(laneObjectCost, index, subLane, navLaneData.m_Flags);
				m_Buffer[m_BufferPos++] = num2;
			}
		}
	}

	private float CalculateLaneObjectCost(float laneObjectCost, int index, Entity lane, WatercraftLaneFlags laneFlags)
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

	private float CalculateLaneObjectCost(float laneObjectCost, Entity lane, float minCurvePosition, WatercraftLaneFlags laneFlags)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (m_LaneObjects.HasBuffer(lane))
		{
			DynamicBuffer<LaneObject> val = m_LaneObjects[lane];
			for (int i = 0; i < val.Length; i++)
			{
				LaneObject laneObject = val[i];
				if (laneObject.m_CurvePosition.y > minCurvePosition)
				{
					num += CalculateLaneObjectCost(laneObject, laneObjectCost, laneFlags);
				}
			}
		}
		return num;
	}

	private float CalculateLaneObjectCost(LaneObject laneObject, float laneObjectCost, WatercraftLaneFlags laneFlags)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!m_MovingData.HasComponent(laneObject.m_LaneObject))
		{
			if (m_WatercraftData.HasComponent(laneObject.m_LaneObject) && (m_WatercraftData[laneObject.m_LaneObject].m_Flags & WatercraftFlags.Queueing) != 0 && (laneFlags & WatercraftLaneFlags.Queue) != 0)
			{
				return laneObjectCost;
			}
			return math.lerp(10000000f, 9000000f, laneObject.m_CurvePosition.y);
		}
		return laneObjectCost;
	}

	public void CalculateLaneCosts(WatercraftNavigationLane navLaneData, WatercraftNavigationLane nextNavLaneData, int index)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		if ((navLaneData.m_Flags & (WatercraftLaneFlags.Reserved | WatercraftLaneFlags.FixedLane)) == 0 && m_SlaveLaneData.HasComponent(navLaneData.m_Lane))
		{
			SlaveLane slaveLane = m_SlaveLaneData[navLaneData.m_Lane];
			Owner owner = m_OwnerData[navLaneData.m_Lane];
			DynamicBuffer<SubLane> val = m_Lanes[owner.m_Owner];
			int num = math.min((int)slaveLane.m_MaxIndex, val.Length - 1);
			m_LaneSwitchCost = m_LaneSwitchBaseCost + math.select(1f, 5f, (slaveLane.m_Flags & SlaveLaneFlags.AllowChange) == 0);
			float laneObjectCost = math.abs(navLaneData.m_CurvePosition.y - navLaneData.m_CurvePosition.x) * 0.49f;
			if ((nextNavLaneData.m_Flags & (WatercraftLaneFlags.Reserved | WatercraftLaneFlags.FixedLane)) == 0 && m_SlaveLaneData.HasComponent(nextNavLaneData.m_Lane))
			{
				SlaveLane slaveLane2 = m_SlaveLaneData[nextNavLaneData.m_Lane];
				Owner owner2 = m_OwnerData[nextNavLaneData.m_Lane];
				DynamicBuffer<SubLane> val2 = m_Lanes[owner2.m_Owner];
				int num2 = math.min((int)slaveLane2.m_MaxIndex, val2.Length - 1);
				int num3 = m_BufferPos - (num2 - slaveLane2.m_MinIndex + 1);
				for (int i = slaveLane.m_MinIndex; i <= num; i++)
				{
					Entity subLane = val[i].m_SubLane;
					Lane lane = m_LaneData[subLane];
					float num4 = 1000000f;
					int num5;
					int num6;
					if ((nextNavLaneData.m_Flags & WatercraftLaneFlags.GroupTarget) != 0)
					{
						num5 = slaveLane2.m_MinIndex;
						num6 = num2;
					}
					else
					{
						num5 = 100000;
						num6 = -100000;
						for (int j = slaveLane2.m_MinIndex; j <= num2; j++)
						{
							Lane lane2 = m_LaneData[val2[j].m_SubLane];
							if (lane.m_EndNode.Equals(lane2.m_StartNode))
							{
								num5 = math.min(num5, j);
								num6 = j;
							}
						}
					}
					if (num5 <= num6)
					{
						int num7 = num3;
						for (int k = slaveLane2.m_MinIndex; k < num5; k++)
						{
							num4 = math.min(num4, m_Buffer[num7++] + GetLaneSwitchCost(num5 - k));
						}
						for (int l = num5; l <= num6; l++)
						{
							num4 = math.min(num4, m_Buffer[num7++]);
						}
						for (int m = num6 + 1; m <= num2; m++)
						{
							num4 = math.min(num4, m_Buffer[num7++] + GetLaneSwitchCost(m - num6));
						}
						num4 += CalculateLaneObjectCost(laneObjectCost, index, subLane, navLaneData.m_Flags);
						if (m_LaneReservationData.HasComponent(subLane))
						{
							num4 += GetLanePriorityCost(m_LaneReservationData[subLane].GetPriority());
						}
					}
					m_Buffer[m_BufferPos++] = num4;
				}
			}
			else if ((nextNavLaneData.m_Flags & WatercraftLaneFlags.TransformTarget) != 0)
			{
				for (int n = slaveLane.m_MinIndex; n <= num; n++)
				{
					Entity subLane2 = val[n].m_SubLane;
					float num8 = CalculateLaneObjectCost(laneObjectCost, index, subLane2, navLaneData.m_Flags);
					if (m_LaneReservationData.HasComponent(subLane2))
					{
						num8 += GetLanePriorityCost(m_LaneReservationData[subLane2].GetPriority());
					}
					m_Buffer[m_BufferPos++] = num8;
				}
			}
			else
			{
				int num9 = 100000;
				int num10 = -100000;
				if ((nextNavLaneData.m_Flags & WatercraftLaneFlags.GroupTarget) != 0)
				{
					for (int num11 = slaveLane.m_MinIndex; num11 <= num; num11++)
					{
						if (val[num11].m_SubLane == nextNavLaneData.m_Lane)
						{
							num9 = num11;
							num10 = num11;
							break;
						}
					}
				}
				else
				{
					Lane lane3 = m_LaneData[nextNavLaneData.m_Lane];
					for (int num12 = slaveLane.m_MinIndex; num12 <= num; num12++)
					{
						if (m_LaneData[val[num12].m_SubLane].m_EndNode.Equals(lane3.m_StartNode))
						{
							num9 = math.min(num9, num12);
							num10 = num12;
						}
					}
				}
				for (int num13 = slaveLane.m_MinIndex; num13 <= num; num13++)
				{
					Entity subLane3 = val[num13].m_SubLane;
					float num14 = 0f;
					if (num9 <= num10)
					{
						num14 += GetLaneSwitchCost(math.max(0, math.max(num9 - num13, num13 - num10)));
					}
					num14 += CalculateLaneObjectCost(laneObjectCost, index, subLane3, navLaneData.m_Flags);
					if (m_LaneReservationData.HasComponent(subLane3))
					{
						num14 += GetLanePriorityCost(m_LaneReservationData[subLane3].GetPriority());
					}
					m_Buffer[m_BufferPos++] = num14;
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

	public void UpdateOptimalLane(ref WatercraftCurrentLane currentLaneData, WatercraftNavigationLane nextNavLaneData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		Entity val = currentLaneData.m_Lane;
		if ((currentLaneData.m_LaneFlags & WatercraftLaneFlags.FixedLane) == 0 && m_SlaveLaneData.HasComponent(currentLaneData.m_Lane))
		{
			SlaveLane slaveLane = m_SlaveLaneData[currentLaneData.m_Lane];
			Owner owner = m_OwnerData[currentLaneData.m_Lane];
			DynamicBuffer<SubLane> val2 = m_Lanes[owner.m_Owner];
			int num = math.min((int)slaveLane.m_MaxIndex, val2.Length - 1);
			m_LaneSwitchCost = m_LaneSwitchBaseCost + math.select(1f, 5f, (slaveLane.m_Flags & SlaveLaneFlags.AllowChange) == 0);
			float laneObjectCost = 0.49f;
			int num2 = 0;
			for (int i = slaveLane.m_MinIndex; i <= num; i++)
			{
				if (val2[i].m_SubLane == currentLaneData.m_Lane)
				{
					num2 = i;
					break;
				}
			}
			float num3 = float.MaxValue;
			if ((nextNavLaneData.m_Flags & (WatercraftLaneFlags.Reserved | WatercraftLaneFlags.FixedLane)) == 0 && m_SlaveLaneData.HasComponent(nextNavLaneData.m_Lane))
			{
				SlaveLane slaveLane2 = m_SlaveLaneData[nextNavLaneData.m_Lane];
				Owner owner2 = m_OwnerData[nextNavLaneData.m_Lane];
				DynamicBuffer<SubLane> val3 = m_Lanes[owner2.m_Owner];
				int num4 = math.min((int)slaveLane2.m_MaxIndex, val3.Length - 1);
				int num5 = m_BufferPos - (num4 - slaveLane2.m_MinIndex + 1);
				for (int j = slaveLane.m_MinIndex; j <= num; j++)
				{
					Entity subLane = val2[j].m_SubLane;
					Lane lane = m_LaneData[subLane];
					float num6 = 1000000f;
					int num7;
					int num8;
					if ((nextNavLaneData.m_Flags & WatercraftLaneFlags.GroupTarget) != 0)
					{
						num7 = slaveLane2.m_MinIndex;
						num8 = num4;
					}
					else
					{
						num7 = 100000;
						num8 = -100000;
						for (int k = slaveLane2.m_MinIndex; k <= num4; k++)
						{
							Lane lane2 = m_LaneData[val3[k].m_SubLane];
							if (lane.m_EndNode.Equals(lane2.m_StartNode))
							{
								num7 = math.min(num7, k);
								num8 = k;
							}
						}
					}
					if (num7 <= num8)
					{
						int num9 = num5 + (num7 - slaveLane2.m_MinIndex);
						for (int l = num7; l <= num8; l++)
						{
							num6 = math.min(num6, m_Buffer[num9++]);
						}
						num6 += CalculateLaneObjectCost(laneObjectCost, subLane, currentLaneData.m_CurvePosition.x, currentLaneData.m_LaneFlags);
						if (m_LaneReservationData.HasComponent(subLane))
						{
							num6 += GetLanePriorityCost(m_LaneReservationData[subLane].GetPriority());
						}
					}
					num6 += GetLaneSwitchCost(math.abs(j - num2));
					if (num6 < num3)
					{
						num3 = num6;
						val = subLane;
					}
				}
			}
			else if ((nextNavLaneData.m_Flags & WatercraftLaneFlags.TransformTarget) != 0 || nextNavLaneData.m_Lane == Entity.Null)
			{
				for (int m = slaveLane.m_MinIndex; m <= num; m++)
				{
					Entity subLane2 = val2[m].m_SubLane;
					float num10 = CalculateLaneObjectCost(laneObjectCost, subLane2, currentLaneData.m_CurvePosition.x, currentLaneData.m_LaneFlags);
					if (m_LaneReservationData.HasComponent(subLane2))
					{
						num10 += GetLanePriorityCost(m_LaneReservationData[subLane2].GetPriority());
					}
					num10 += GetLaneSwitchCost(math.abs(m - num2));
					if (num10 < num3)
					{
						num3 = num10;
						val = subLane2;
					}
				}
			}
			else
			{
				int num11 = 100000;
				int num12 = -100000;
				if ((nextNavLaneData.m_Flags & WatercraftLaneFlags.GroupTarget) != 0)
				{
					for (int n = slaveLane.m_MinIndex; n <= num; n++)
					{
						if (val2[n].m_SubLane == nextNavLaneData.m_Lane)
						{
							num11 = n;
							num12 = n;
							break;
						}
					}
				}
				else
				{
					Lane lane3 = m_LaneData[nextNavLaneData.m_Lane];
					for (int num13 = slaveLane.m_MinIndex; num13 <= num; num13++)
					{
						if (m_LaneData[val2[num13].m_SubLane].m_EndNode.Equals(lane3.m_StartNode))
						{
							num11 = math.min(num11, num13);
							num12 = num13;
						}
					}
				}
				for (int num14 = slaveLane.m_MinIndex; num14 <= num; num14++)
				{
					Entity subLane3 = val2[num14].m_SubLane;
					float num15;
					if ((num14 >= num11 && num14 <= num12) || num11 > num12)
					{
						num15 = CalculateLaneObjectCost(laneObjectCost, subLane3, currentLaneData.m_CurvePosition.x, currentLaneData.m_LaneFlags);
						if (m_LaneReservationData.HasComponent(subLane3))
						{
							num15 += GetLanePriorityCost(m_LaneReservationData[subLane3].GetPriority());
						}
					}
					else
					{
						num15 = 1000000f;
					}
					num15 += GetLaneSwitchCost(math.abs(num14 - num2));
					if (num15 < num3)
					{
						num3 = num15;
						val = subLane3;
					}
				}
			}
		}
		if (val != currentLaneData.m_Lane)
		{
			if (val != currentLaneData.m_ChangeLane)
			{
				currentLaneData.m_ChangeLane = val;
				currentLaneData.m_ChangeProgress = 0f;
			}
		}
		else if (currentLaneData.m_ChangeLane != Entity.Null)
		{
			if (currentLaneData.m_ChangeProgress == 0f)
			{
				currentLaneData.m_ChangeLane = Entity.Null;
			}
			else
			{
				currentLaneData.m_Lane = currentLaneData.m_ChangeLane;
				currentLaneData.m_ChangeLane = val;
				currentLaneData.m_ChangeProgress = math.saturate(1f - currentLaneData.m_ChangeProgress);
			}
		}
		if (currentLaneData.m_ChangeLane == Entity.Null)
		{
			m_PrevLane = currentLaneData.m_Lane;
		}
		else
		{
			m_PrevLane = currentLaneData.m_ChangeLane;
		}
		m_LaneSwitchCost = 10000000f;
	}

	public void UpdateOptimalLane(ref WatercraftNavigationLane navLaneData)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		if (m_SlaveLaneData.HasComponent(navLaneData.m_Lane))
		{
			SlaveLane slaveLane = m_SlaveLaneData[navLaneData.m_Lane];
			if ((navLaneData.m_Flags & (WatercraftLaneFlags.FixedStart | WatercraftLaneFlags.Reserved | WatercraftLaneFlags.FixedLane)) == 0 && m_LaneData.HasComponent(m_PrevLane))
			{
				Owner owner = m_OwnerData[navLaneData.m_Lane];
				DynamicBuffer<SubLane> val = m_Lanes[owner.m_Owner];
				int num = math.min((int)slaveLane.m_MaxIndex, val.Length - 1);
				m_BufferPos -= num - slaveLane.m_MinIndex + 1;
				int num2 = 100000;
				int num3 = -100000;
				if ((navLaneData.m_Flags & WatercraftLaneFlags.GroupTarget) == 0)
				{
					Lane lane = m_LaneData[m_PrevLane];
					for (int i = slaveLane.m_MinIndex; i <= num; i++)
					{
						Lane lane2 = m_LaneData[val[i].m_SubLane];
						if (lane.m_EndNode.Equals(lane2.m_StartNode))
						{
							num2 = math.min(num2, i);
							num3 = i;
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
				for (int j = slaveLane.m_MinIndex; j < num2; j++)
				{
					float num6 = m_Buffer[bufferPos++] + GetLaneSwitchCost(num2 - j);
					if (num6 < num4)
					{
						num4 = num6;
						num5 = j;
					}
				}
				for (int k = num2; k <= num3; k++)
				{
					float num7 = m_Buffer[bufferPos++];
					if (num7 < num4)
					{
						num4 = num7;
						num5 = k;
					}
				}
				for (int l = num3 + 1; l <= num; l++)
				{
					float num8 = m_Buffer[bufferPos++] + GetLaneSwitchCost(l - num3);
					if (num8 < num4)
					{
						num4 = num8;
						num5 = l;
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

	public void DrawLaneCosts(WatercraftCurrentLane currentLaneData, WatercraftNavigationLane nextNavLaneData, ComponentLookup<Curve> curveData, GizmoBatcher gizmoBatcher)
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

	public void DrawLaneCosts(WatercraftNavigationLane navLaneData, ComponentLookup<Curve> curveData, GizmoBatcher gizmoBatcher)
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
			if ((navLaneData.m_Flags & (WatercraftLaneFlags.Reserved | WatercraftLaneFlags.FixedLane)) == 0)
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

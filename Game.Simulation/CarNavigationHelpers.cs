using System;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public static class CarNavigationHelpers
{
	public struct LaneSignal
	{
		public Entity m_Petitioner;

		public Entity m_Lane;

		public sbyte m_Priority;

		public LaneSignal(Entity petitioner, Entity lane, int priority)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Petitioner = petitioner;
			m_Lane = lane;
			m_Priority = (sbyte)priority;
		}
	}

	public struct LaneReservation : IComparable<LaneReservation>
	{
		public Entity m_Lane;

		public byte m_Offset;

		public byte m_Priority;

		public LaneReservation(Entity lane, float offset, int priority)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Lane = lane;
			m_Offset = (byte)math.clamp((int)math.round(offset * 255f), 0, 255);
			m_Priority = (byte)priority;
		}

		public int CompareTo(LaneReservation other)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return m_Lane.Index - other.m_Lane.Index;
		}
	}

	public struct LaneEffects
	{
		public Entity m_Lane;

		public float3 m_SideEffects;

		public float2 m_Flow;

		public LaneEffects(Entity lane, float3 sideEffects, float2 flow)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			m_Lane = lane;
			m_SideEffects = sideEffects;
			m_Flow = flow;
		}
	}

	public struct CurrentLaneCache
	{
		private NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		private Entity m_WasCurrentLane;

		private Entity m_WasChangeLane;

		private float2 m_WasCurvePosition;

		private DynamicBuffer<BlockedLane> m_WasBlockedLanes;

		public CurrentLaneCache(ref CarCurrentLane currentLane, DynamicBuffer<BlockedLane> blockedLanes, EntityStorageInfoLookup entityLookup, NativeQuadTree<Entity, QuadTreeBoundsXZ> searchTree)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			if (!((EntityStorageInfoLookup)(ref entityLookup)).Exists(currentLane.m_Lane))
			{
				currentLane.m_Lane = Entity.Null;
			}
			if (currentLane.m_ChangeLane != Entity.Null && !((EntityStorageInfoLookup)(ref entityLookup)).Exists(currentLane.m_ChangeLane))
			{
				currentLane.m_ChangeLane = Entity.Null;
				currentLane.m_LaneFlags &= ~(Game.Vehicles.CarLaneFlags.TurnLeft | Game.Vehicles.CarLaneFlags.TurnRight);
			}
			m_WasBlockedLanes = blockedLanes;
			m_SearchTree = searchTree;
			m_WasCurrentLane = currentLane.m_Lane;
			m_WasChangeLane = currentLane.m_ChangeLane;
			m_WasCurvePosition = ((float3)(ref currentLane.m_CurvePosition)).xy;
		}

		public void CheckChanges(Entity entity, ref CarCurrentLane currentLane, NativeList<BlockedLane> newBlockedLanes, LaneObjectCommandBuffer buffer, BufferLookup<LaneObject> laneObjects, Transform transform, Moving moving, CarNavigation navigation, ObjectGeometryData objectGeometryData)
		{
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			if (newBlockedLanes.IsCreated && newBlockedLanes.Length != 0)
			{
				QuadTreeBoundsXZ quadTreeBoundsXZ = default(QuadTreeBoundsXZ);
				if (laneObjects.HasBuffer(m_WasCurrentLane))
				{
					m_WasBlockedLanes.Add(new BlockedLane(m_WasCurrentLane, m_WasCurvePosition));
					buffer.Add(entity, CalculateMaxBounds(transform, moving, navigation, objectGeometryData));
				}
				else if (m_SearchTree.TryGet(entity, ref quadTreeBoundsXZ))
				{
					Bounds3 val = CalculateMinBounds(transform, moving, navigation, objectGeometryData);
					if (math.any(val.min < quadTreeBoundsXZ.m_Bounds.min) | math.any(val.max > quadTreeBoundsXZ.m_Bounds.max))
					{
						buffer.Update(entity, CalculateMaxBounds(transform, moving, navigation, objectGeometryData));
					}
				}
				if (laneObjects.HasBuffer(m_WasChangeLane))
				{
					m_WasBlockedLanes.Add(new BlockedLane(m_WasChangeLane, m_WasCurvePosition));
				}
				int num = 0;
				while (num < m_WasBlockedLanes.Length)
				{
					BlockedLane blockedLane = m_WasBlockedLanes[num];
					int num2 = 0;
					while (true)
					{
						if (num2 < newBlockedLanes.Length)
						{
							BlockedLane blockedLane2 = newBlockedLanes[num2];
							if (blockedLane2.m_Lane == blockedLane.m_Lane)
							{
								if (!((float2)(ref blockedLane2.m_CurvePosition)).Equals(blockedLane.m_CurvePosition))
								{
									m_WasBlockedLanes[num] = blockedLane2;
									buffer.Update(blockedLane2.m_Lane, entity, blockedLane2.m_CurvePosition);
								}
								newBlockedLanes.RemoveAtSwapBack(num2);
								num++;
								break;
							}
							num2++;
							continue;
						}
						m_WasBlockedLanes.RemoveAt(num);
						buffer.Remove(blockedLane.m_Lane, entity);
						break;
					}
				}
				for (int i = 0; i < newBlockedLanes.Length; i++)
				{
					BlockedLane blockedLane3 = newBlockedLanes[i];
					m_WasBlockedLanes.Add(blockedLane3);
					buffer.Add(blockedLane3.m_Lane, entity, blockedLane3.m_CurvePosition);
				}
				return;
			}
			if (m_WasBlockedLanes.Length != 0)
			{
				for (int j = 0; j < m_WasBlockedLanes.Length; j++)
				{
					Entity lane = m_WasBlockedLanes[j].m_Lane;
					if (laneObjects.HasBuffer(lane))
					{
						buffer.Remove(lane, entity);
					}
				}
				m_WasBlockedLanes.Clear();
				m_WasCurrentLane = Entity.Null;
				m_WasChangeLane = Entity.Null;
			}
			if (currentLane.m_Lane == m_WasChangeLane)
			{
				QuadTreeBoundsXZ quadTreeBoundsXZ2 = default(QuadTreeBoundsXZ);
				if (laneObjects.HasBuffer(m_WasCurrentLane))
				{
					buffer.Remove(m_WasCurrentLane, entity);
					if (laneObjects.HasBuffer(currentLane.m_Lane))
					{
						if (!((float2)(ref m_WasCurvePosition)).Equals(((float3)(ref currentLane.m_CurvePosition)).xy))
						{
							buffer.Update(currentLane.m_Lane, entity, ((float3)(ref currentLane.m_CurvePosition)).xy);
						}
					}
					else
					{
						buffer.Add(entity, CalculateMaxBounds(transform, moving, navigation, objectGeometryData));
					}
				}
				else if (laneObjects.HasBuffer(currentLane.m_Lane))
				{
					buffer.Remove(entity);
					if (!((float2)(ref m_WasCurvePosition)).Equals(((float3)(ref currentLane.m_CurvePosition)).xy))
					{
						buffer.Update(currentLane.m_Lane, entity, ((float3)(ref currentLane.m_CurvePosition)).xy);
					}
				}
				else if (m_SearchTree.TryGet(entity, ref quadTreeBoundsXZ2))
				{
					Bounds3 val2 = CalculateMinBounds(transform, moving, navigation, objectGeometryData);
					if (math.any(val2.min < quadTreeBoundsXZ2.m_Bounds.min) | math.any(val2.max > quadTreeBoundsXZ2.m_Bounds.max))
					{
						buffer.Update(entity, CalculateMaxBounds(transform, moving, navigation, objectGeometryData));
					}
				}
				if (laneObjects.HasBuffer(currentLane.m_ChangeLane))
				{
					buffer.Add(currentLane.m_ChangeLane, entity, ((float3)(ref currentLane.m_CurvePosition)).xy);
				}
				return;
			}
			QuadTreeBoundsXZ quadTreeBoundsXZ3 = default(QuadTreeBoundsXZ);
			if (currentLane.m_Lane != m_WasCurrentLane)
			{
				if (laneObjects.HasBuffer(m_WasCurrentLane))
				{
					buffer.Remove(m_WasCurrentLane, entity);
				}
				else
				{
					buffer.Remove(entity);
				}
				if (laneObjects.HasBuffer(currentLane.m_Lane))
				{
					buffer.Add(currentLane.m_Lane, entity, ((float3)(ref currentLane.m_CurvePosition)).xy);
				}
				else
				{
					buffer.Add(entity, CalculateMaxBounds(transform, moving, navigation, objectGeometryData));
				}
			}
			else if (laneObjects.HasBuffer(m_WasCurrentLane))
			{
				if (!((float2)(ref m_WasCurvePosition)).Equals(((float3)(ref currentLane.m_CurvePosition)).xy))
				{
					buffer.Update(m_WasCurrentLane, entity, ((float3)(ref currentLane.m_CurvePosition)).xy);
				}
			}
			else if (m_SearchTree.TryGet(entity, ref quadTreeBoundsXZ3))
			{
				Bounds3 val3 = CalculateMinBounds(transform, moving, navigation, objectGeometryData);
				if (math.any(val3.min < quadTreeBoundsXZ3.m_Bounds.min) | math.any(val3.max > quadTreeBoundsXZ3.m_Bounds.max))
				{
					buffer.Update(entity, CalculateMaxBounds(transform, moving, navigation, objectGeometryData));
				}
			}
			if (currentLane.m_ChangeLane != m_WasChangeLane)
			{
				if (laneObjects.HasBuffer(m_WasChangeLane))
				{
					buffer.Remove(m_WasChangeLane, entity);
				}
				if (laneObjects.HasBuffer(currentLane.m_ChangeLane))
				{
					buffer.Add(currentLane.m_ChangeLane, entity, ((float3)(ref currentLane.m_CurvePosition)).xy);
				}
			}
			else if (laneObjects.HasBuffer(m_WasChangeLane) && !((float2)(ref m_WasCurvePosition)).Equals(((float3)(ref currentLane.m_CurvePosition)).xy))
			{
				buffer.Update(m_WasChangeLane, entity, ((float3)(ref currentLane.m_CurvePosition)).xy);
			}
		}

		private Bounds3 CalculateMinBounds(Transform transform, Moving moving, CarNavigation navigation, ObjectGeometryData objectGeometryData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			float3 val = moving.m_Velocity * num;
			float3 val2 = math.normalizesafe(navigation.m_TargetPosition - transform.m_Position, default(float3)) * math.abs(navigation.m_MaxSpeed * num);
			Bounds3 result = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, objectGeometryData);
			ref float3 min = ref result.min;
			min += math.min(float3.op_Implicit(0f), math.min(val, val2));
			ref float3 max = ref result.max;
			max += math.max(float3.op_Implicit(0f), math.max(val, val2));
			return result;
		}

		private Bounds3 CalculateMaxBounds(Transform transform, Moving moving, CarNavigation navigation, ObjectGeometryData objectGeometryData)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			float num = -1.0666667f;
			float num2 = 2f;
			float num3 = math.length(objectGeometryData.m_Size) * 0.5f;
			float3 val = moving.m_Velocity * num;
			float3 val2 = moving.m_Velocity * num2;
			float3 val3 = math.normalizesafe(navigation.m_TargetPosition - transform.m_Position, default(float3)) * math.abs(navigation.m_MaxSpeed * num2);
			float3 position = transform.m_Position;
			position.y += objectGeometryData.m_Size.y * 0.5f;
			Bounds3 result = default(Bounds3);
			result.min = position - num3 + math.min(val, math.min(val2, val3));
			result.max = position + num3 + math.max(val, math.max(val2, val3));
			return result;
		}
	}

	public struct TrailerLaneCache
	{
		private NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		private Entity m_WasCurrentLane;

		private Entity m_WasNextLane;

		private float2 m_WasCurrentPosition;

		private float2 m_WasNextPosition;

		private DynamicBuffer<BlockedLane> m_WasBlockedLanes;

		public TrailerLaneCache(ref CarTrailerLane trailerLane, DynamicBuffer<BlockedLane> blockedLanes, ComponentLookup<PrefabRef> prefabRefData, NativeQuadTree<Entity, QuadTreeBoundsXZ> searchTree)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (!prefabRefData.HasComponent(trailerLane.m_Lane))
			{
				trailerLane.m_Lane = Entity.Null;
			}
			if (!prefabRefData.HasComponent(trailerLane.m_NextLane))
			{
				trailerLane.m_NextLane = Entity.Null;
			}
			m_WasBlockedLanes = blockedLanes;
			m_SearchTree = searchTree;
			m_WasCurrentLane = trailerLane.m_Lane;
			m_WasNextLane = trailerLane.m_NextLane;
			m_WasCurrentPosition = trailerLane.m_CurvePosition;
			m_WasNextPosition = trailerLane.m_NextPosition;
		}

		public void CheckChanges(Entity entity, ref CarTrailerLane trailerLane, NativeList<BlockedLane> newBlockedLanes, LaneObjectCommandBuffer buffer, BufferLookup<LaneObject> laneObjects, Transform transform, Moving moving, CarNavigation tractorNavigation, ObjectGeometryData objectGeometryData)
		{
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			if (newBlockedLanes.IsCreated && newBlockedLanes.Length != 0)
			{
				QuadTreeBoundsXZ quadTreeBoundsXZ = default(QuadTreeBoundsXZ);
				if (laneObjects.HasBuffer(m_WasCurrentLane))
				{
					m_WasBlockedLanes.Add(new BlockedLane(m_WasCurrentLane, m_WasCurrentPosition));
					buffer.Add(entity, CalculateMaxBounds(transform, moving, tractorNavigation, objectGeometryData));
				}
				else if (m_SearchTree.TryGet(entity, ref quadTreeBoundsXZ))
				{
					Bounds3 val = CalculateMinBounds(transform, moving, tractorNavigation, objectGeometryData);
					if (math.any(val.min < quadTreeBoundsXZ.m_Bounds.min) | math.any(val.max > quadTreeBoundsXZ.m_Bounds.max))
					{
						buffer.Update(entity, CalculateMaxBounds(transform, moving, tractorNavigation, objectGeometryData));
					}
				}
				if (laneObjects.HasBuffer(m_WasNextLane))
				{
					m_WasBlockedLanes.Add(new BlockedLane(m_WasNextLane, m_WasNextPosition));
				}
				int num = 0;
				while (num < m_WasBlockedLanes.Length)
				{
					BlockedLane blockedLane = m_WasBlockedLanes[num];
					int num2 = 0;
					while (true)
					{
						if (num2 < newBlockedLanes.Length)
						{
							BlockedLane blockedLane2 = newBlockedLanes[num2];
							if (blockedLane2.m_Lane == blockedLane.m_Lane)
							{
								if (!((float2)(ref blockedLane2.m_CurvePosition)).Equals(blockedLane.m_CurvePosition))
								{
									m_WasBlockedLanes[num] = blockedLane2;
									buffer.Update(blockedLane2.m_Lane, entity, blockedLane2.m_CurvePosition);
								}
								newBlockedLanes.RemoveAtSwapBack(num2);
								num++;
								break;
							}
							num2++;
							continue;
						}
						m_WasBlockedLanes.RemoveAt(num);
						buffer.Remove(blockedLane.m_Lane, entity);
						break;
					}
				}
				for (int i = 0; i < newBlockedLanes.Length; i++)
				{
					BlockedLane blockedLane3 = newBlockedLanes[i];
					m_WasBlockedLanes.Add(blockedLane3);
					buffer.Add(blockedLane3.m_Lane, entity, blockedLane3.m_CurvePosition);
				}
				return;
			}
			if (m_WasBlockedLanes.Length != 0)
			{
				for (int j = 0; j < m_WasBlockedLanes.Length; j++)
				{
					Entity lane = m_WasBlockedLanes[j].m_Lane;
					if (laneObjects.HasBuffer(lane))
					{
						buffer.Remove(lane, entity);
					}
				}
				m_WasBlockedLanes.Clear();
				m_WasCurrentLane = Entity.Null;
				m_WasNextLane = Entity.Null;
			}
			if (trailerLane.m_Lane == m_WasNextLane)
			{
				QuadTreeBoundsXZ quadTreeBoundsXZ2 = default(QuadTreeBoundsXZ);
				if (laneObjects.HasBuffer(m_WasCurrentLane))
				{
					buffer.Remove(m_WasCurrentLane, entity);
					if (laneObjects.HasBuffer(trailerLane.m_Lane))
					{
						if (!((float2)(ref m_WasNextPosition)).Equals(trailerLane.m_CurvePosition))
						{
							buffer.Update(trailerLane.m_Lane, entity, trailerLane.m_CurvePosition);
						}
					}
					else
					{
						buffer.Add(entity, CalculateMaxBounds(transform, moving, tractorNavigation, objectGeometryData));
					}
				}
				else if (laneObjects.HasBuffer(trailerLane.m_Lane))
				{
					buffer.Remove(entity);
					if (!((float2)(ref m_WasNextPosition)).Equals(trailerLane.m_CurvePosition))
					{
						buffer.Update(trailerLane.m_Lane, entity, trailerLane.m_CurvePosition);
					}
				}
				else if (m_SearchTree.TryGet(entity, ref quadTreeBoundsXZ2))
				{
					Bounds3 val2 = CalculateMinBounds(transform, moving, tractorNavigation, objectGeometryData);
					if (math.any(val2.min < quadTreeBoundsXZ2.m_Bounds.min) | math.any(val2.max > quadTreeBoundsXZ2.m_Bounds.max))
					{
						buffer.Update(entity, CalculateMaxBounds(transform, moving, tractorNavigation, objectGeometryData));
					}
				}
				if (laneObjects.HasBuffer(trailerLane.m_NextLane))
				{
					buffer.Add(trailerLane.m_NextLane, entity, trailerLane.m_NextPosition);
				}
				return;
			}
			QuadTreeBoundsXZ quadTreeBoundsXZ3 = default(QuadTreeBoundsXZ);
			if (trailerLane.m_Lane != m_WasCurrentLane)
			{
				if (laneObjects.HasBuffer(m_WasCurrentLane))
				{
					buffer.Remove(m_WasCurrentLane, entity);
				}
				else
				{
					buffer.Remove(entity);
				}
				if (laneObjects.HasBuffer(trailerLane.m_Lane))
				{
					buffer.Add(trailerLane.m_Lane, entity, trailerLane.m_CurvePosition);
				}
				else
				{
					buffer.Add(entity, CalculateMaxBounds(transform, moving, tractorNavigation, objectGeometryData));
				}
			}
			else if (laneObjects.HasBuffer(m_WasCurrentLane))
			{
				if (!((float2)(ref m_WasCurrentPosition)).Equals(trailerLane.m_CurvePosition))
				{
					buffer.Update(m_WasCurrentLane, entity, trailerLane.m_CurvePosition);
				}
			}
			else if (m_SearchTree.TryGet(entity, ref quadTreeBoundsXZ3))
			{
				Bounds3 val3 = CalculateMinBounds(transform, moving, tractorNavigation, objectGeometryData);
				if (math.any(val3.min < quadTreeBoundsXZ3.m_Bounds.min) | math.any(val3.max > quadTreeBoundsXZ3.m_Bounds.max))
				{
					buffer.Update(entity, CalculateMaxBounds(transform, moving, tractorNavigation, objectGeometryData));
				}
			}
			if (trailerLane.m_NextLane != m_WasNextLane)
			{
				if (laneObjects.HasBuffer(m_WasNextLane))
				{
					buffer.Remove(m_WasNextLane, entity);
				}
				if (laneObjects.HasBuffer(trailerLane.m_NextLane))
				{
					buffer.Add(trailerLane.m_NextLane, entity, trailerLane.m_NextPosition);
				}
			}
			else if (laneObjects.HasBuffer(m_WasNextLane) && !((float2)(ref m_WasNextPosition)).Equals(trailerLane.m_NextPosition))
			{
				buffer.Update(m_WasNextLane, entity, trailerLane.m_NextPosition);
			}
		}

		private Bounds3 CalculateMinBounds(Transform transform, Moving moving, CarNavigation tractorNavigation, ObjectGeometryData objectGeometryData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			float3 val = moving.m_Velocity * num;
			float3 val2 = math.normalizesafe(tractorNavigation.m_TargetPosition - transform.m_Position, default(float3)) * math.abs(tractorNavigation.m_MaxSpeed * num);
			Bounds3 result = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, objectGeometryData);
			ref float3 min = ref result.min;
			min += math.min(float3.op_Implicit(0f), math.min(val, val2));
			ref float3 max = ref result.max;
			max += math.max(float3.op_Implicit(0f), math.max(val, val2));
			return result;
		}

		private Bounds3 CalculateMaxBounds(Transform transform, Moving moving, CarNavigation tractorNavigation, ObjectGeometryData objectGeometryData)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			float num = -1.0666667f;
			float num2 = 2f;
			float num3 = math.length(objectGeometryData.m_Size) * 0.5f;
			float3 val = moving.m_Velocity * num;
			float3 val2 = moving.m_Velocity * num2;
			float3 val3 = math.normalizesafe(tractorNavigation.m_TargetPosition - transform.m_Position, default(float3)) * math.abs(tractorNavigation.m_MaxSpeed * num2);
			float3 position = transform.m_Position;
			position.y += objectGeometryData.m_Size.y * 0.5f;
			Bounds3 result = default(Bounds3);
			result.min = position - num3 + math.min(val, math.min(val2, val3));
			result.max = position + num3 + math.max(val, math.max(val2, val3));
			return result;
		}
	}

	public struct FindLaneIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
	{
		public Bounds3 m_Bounds;

		public float3 m_Position;

		public float m_MinDistance;

		public CarCurrentLane m_Result;

		public RoadTypes m_CarType;

		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		public BufferLookup<Triangle> m_AreaTriangles;

		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		public ComponentLookup<MasterLane> m_MasterLaneData;

		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		public ComponentLookup<Curve> m_CurveData;

		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity ownerEntity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_SubLanes.HasBuffer(ownerEntity))
			{
				return;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[ownerEntity];
			float num2 = default(float);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				Game.Vehicles.CarLaneFlags carLaneFlags = m_Result.m_LaneFlags | (Game.Vehicles.CarLaneFlags.EnteringRoad | Game.Vehicles.CarLaneFlags.FixedLane);
				carLaneFlags = (Game.Vehicles.CarLaneFlags)((uint)carLaneFlags & 0xE2FFFFEFu);
				if (m_CarLaneData.HasComponent(subLane))
				{
					if (m_MasterLaneData.HasComponent(subLane))
					{
						continue;
					}
					PrefabRef prefabRef = m_PrefabRefData[subLane];
					if (m_PrefabCarLaneData[prefabRef.m_Prefab].m_RoadTypes != m_CarType)
					{
						continue;
					}
					Game.Net.CarLane carLane = m_CarLaneData[subLane];
					if ((carLane.m_Flags & Game.Net.CarLaneFlags.Twoway) != 0)
					{
						carLaneFlags |= Game.Vehicles.CarLaneFlags.CanReverse;
					}
					if ((carLane.m_Flags & (Game.Net.CarLaneFlags.Approach | Game.Net.CarLaneFlags.Roundabout)) == Game.Net.CarLaneFlags.Roundabout)
					{
						carLaneFlags |= Game.Vehicles.CarLaneFlags.Roundabout;
					}
				}
				else
				{
					if (!m_ConnectionLaneData.HasComponent(subLane))
					{
						continue;
					}
					Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[subLane];
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Road) == 0 || (connectionLane.m_RoadTypes & m_CarType) == 0)
					{
						continue;
					}
					carLaneFlags |= Game.Vehicles.CarLaneFlags.Connection;
				}
				Bezier4x3 bezier = m_CurveData[subLane].m_Bezier;
				float num = MathUtils.Distance(MathUtils.Bounds(bezier), m_Position);
				if (num < m_MinDistance)
				{
					num = MathUtils.Distance(bezier, m_Position, ref num2);
					if (num < m_MinDistance)
					{
						m_Bounds = new Bounds3(m_Position - num, m_Position + num);
						m_MinDistance = num;
						m_Result.m_Lane = subLane;
						m_Result.m_CurvePosition = float3.op_Implicit(num2);
						m_Result.m_LaneFlags = carLaneFlags;
					}
				}
			}
		}

		public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem item)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_SubLanes.HasBuffer(item.m_Area))
			{
				return;
			}
			DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[item.m_Area];
			Triangle triangle = m_AreaTriangles[item.m_Area][item.m_Triangle];
			Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
			float2 val = default(float2);
			float num = MathUtils.Distance(triangle2, m_Position, ref val);
			if (num >= m_MinDistance)
			{
				return;
			}
			float3 val2 = MathUtils.Position(triangle2, val);
			DynamicBuffer<Game.Net.SubLane> val3 = m_SubLanes[item.m_Area];
			float num2 = float.MaxValue;
			Entity val4 = Entity.Null;
			float num3 = 0f;
			float2 val5 = default(float2);
			float num5 = default(float);
			for (int i = 0; i < val3.Length; i++)
			{
				Entity subLane = val3[i].m_SubLane;
				if (!m_ConnectionLaneData.HasComponent(subLane))
				{
					continue;
				}
				Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[subLane];
				if ((connectionLane.m_Flags & ConnectionLaneFlags.Road) == 0 || (connectionLane.m_RoadTypes & m_CarType) == 0)
				{
					continue;
				}
				Curve curve = m_CurveData[subLane];
				if (MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.a)).xz, ref val5) || MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.d)).xz, ref val5))
				{
					float num4 = MathUtils.Distance(curve.m_Bezier, val2, ref num5);
					if (num4 < num2)
					{
						num2 = num4;
						val4 = subLane;
						num3 = num5;
					}
				}
			}
			if (val4 != Entity.Null)
			{
				Game.Vehicles.CarLaneFlags carLaneFlags = m_Result.m_LaneFlags | (Game.Vehicles.CarLaneFlags.EnteringRoad | Game.Vehicles.CarLaneFlags.FixedLane | Game.Vehicles.CarLaneFlags.Area);
				carLaneFlags = (Game.Vehicles.CarLaneFlags)((uint)carLaneFlags & 0xE6FFFFEFu);
				m_Bounds = new Bounds3(m_Position - num, m_Position + num);
				m_MinDistance = num;
				m_Result.m_Lane = val4;
				m_Result.m_CurvePosition = float3.op_Implicit(num3);
				m_Result.m_LaneFlags = carLaneFlags;
			}
		}
	}

	public struct FindBlockedLanesIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Bounds3 m_Bounds;

		public Segment m_Line;

		public float m_Radius;

		public NativeList<BlockedLane> m_BlockedLanes;

		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		public ComponentLookup<MasterLane> m_MasterLaneData;

		public ComponentLookup<Curve> m_CurveData;

		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public ComponentLookup<NetLaneData> m_PrefabLaneData;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity edgeEntity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_SubLanes.HasBuffer(edgeEntity))
			{
				return;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[edgeEntity];
			float2 val2 = default(float2);
			float2 val3 = default(float2);
			Bounds1 val4 = default(Bounds1);
			Bounds1 val5 = default(Bounds1);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (m_MasterLaneData.HasComponent(subLane))
				{
					continue;
				}
				Entity prefab = m_PrefabRefData[subLane].m_Prefab;
				Bezier4x3 bezier = m_CurveData[subLane].m_Bezier;
				NetLaneData netLaneData = m_PrefabLaneData[prefab];
				float num = m_Radius + netLaneData.m_Width * 0.4f;
				if (MathUtils.Intersect(MathUtils.Expand(MathUtils.Bounds(bezier), float3.op_Implicit(num)), m_Line, ref val2))
				{
					float num2 = MathUtils.Distance(bezier, m_Line, ref val3);
					if (num2 < num)
					{
						num2 = math.max(0f, num2 - netLaneData.m_Width * 0.4f);
						float num3 = math.sqrt(math.max(0f, m_Radius * m_Radius - num2 * num2)) + netLaneData.m_Width * 0.4f;
						((Bounds1)(ref val4))._002Ector(0f, val3.x);
						((Bounds1)(ref val5))._002Ector(val3.x, 1f);
						MathUtils.ClampLengthInverse(bezier, ref val4, num3);
						MathUtils.ClampLength(bezier, ref val5, num3);
						ref NativeList<BlockedLane> blockedLanes = ref m_BlockedLanes;
						BlockedLane blockedLane = new BlockedLane(subLane, new float2(val4.min, val5.max));
						blockedLanes.Add(ref blockedLane);
					}
				}
			}
		}
	}
}

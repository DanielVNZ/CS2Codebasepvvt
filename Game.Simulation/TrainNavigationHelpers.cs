using System;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public static class TrainNavigationHelpers
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

		public byte m_Priority;

		public LaneReservation(Entity lane, int priority)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Lane = lane;
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
		private Entity m_WasCurrentLane1;

		private Entity m_WasCurrentLane2;

		private float2 m_WasCurvePosition1;

		private float2 m_WasCurvePosition2;

		public CurrentLaneCache(ref TrainCurrentLane currentLane, ComponentLookup<Lane> laneData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			if (currentLane.m_Front.m_Lane != Entity.Null && !laneData.HasComponent(currentLane.m_Front.m_Lane))
			{
				currentLane.m_Front.m_Lane = Entity.Null;
			}
			if (currentLane.m_Rear.m_Lane != Entity.Null && !laneData.HasComponent(currentLane.m_Rear.m_Lane))
			{
				currentLane.m_Rear.m_Lane = Entity.Null;
			}
			if (currentLane.m_FrontCache.m_Lane != Entity.Null && !laneData.HasComponent(currentLane.m_FrontCache.m_Lane))
			{
				currentLane.m_FrontCache.m_Lane = Entity.Null;
			}
			if (currentLane.m_RearCache.m_Lane != Entity.Null && !laneData.HasComponent(currentLane.m_RearCache.m_Lane))
			{
				currentLane.m_RearCache.m_Lane = Entity.Null;
			}
			m_WasCurrentLane1 = currentLane.m_Front.m_Lane;
			m_WasCurrentLane2 = currentLane.m_Rear.m_Lane;
			GetCurvePositions(ref currentLane, out m_WasCurvePosition1, out m_WasCurvePosition2);
		}

		public void CheckChanges(Entity entity, TrainCurrentLane currentLane, LaneObjectCommandBuffer buffer)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			GetCurvePositions(ref currentLane, out var pos, out var pos2);
			if (currentLane.m_Rear.m_Lane == m_WasCurrentLane1)
			{
				if (currentLane.m_Front.m_Lane == m_WasCurrentLane2)
				{
					if (currentLane.m_Front.m_Lane != Entity.Null && !((float2)(ref m_WasCurvePosition2)).Equals(pos))
					{
						buffer.Update(currentLane.m_Front.m_Lane, entity, pos);
					}
					if (currentLane.m_Rear.m_Lane != currentLane.m_Front.m_Lane && currentLane.m_Rear.m_Lane != Entity.Null && !((float2)(ref m_WasCurvePosition1)).Equals(pos2))
					{
						buffer.Update(currentLane.m_Rear.m_Lane, entity, pos2);
					}
					return;
				}
				if (currentLane.m_Rear.m_Lane != m_WasCurrentLane2 && m_WasCurrentLane2 != Entity.Null)
				{
					buffer.Remove(m_WasCurrentLane2, entity);
				}
				if (currentLane.m_Rear.m_Lane != Entity.Null && !((float2)(ref m_WasCurvePosition1)).Equals(pos2))
				{
					buffer.Update(currentLane.m_Rear.m_Lane, entity, pos2);
				}
				if (currentLane.m_Front.m_Lane != m_WasCurrentLane1 && currentLane.m_Front.m_Lane != Entity.Null)
				{
					buffer.Add(currentLane.m_Front.m_Lane, entity, pos);
				}
				return;
			}
			if (currentLane.m_Front.m_Lane == m_WasCurrentLane2)
			{
				if (currentLane.m_Front.m_Lane != m_WasCurrentLane1 && m_WasCurrentLane1 != Entity.Null)
				{
					buffer.Remove(m_WasCurrentLane1, entity);
				}
				if (currentLane.m_Front.m_Lane != Entity.Null && !((float2)(ref m_WasCurvePosition2)).Equals(pos))
				{
					buffer.Update(currentLane.m_Front.m_Lane, entity, pos);
				}
				if (currentLane.m_Rear.m_Lane != m_WasCurrentLane2 && currentLane.m_Rear.m_Lane != Entity.Null)
				{
					buffer.Add(currentLane.m_Rear.m_Lane, entity, pos2);
				}
				return;
			}
			if (m_WasCurrentLane1 == m_WasCurrentLane2)
			{
				if (m_WasCurrentLane1 != Entity.Null)
				{
					buffer.Remove(m_WasCurrentLane1, entity);
				}
				if (currentLane.m_Front.m_Lane != Entity.Null)
				{
					buffer.Add(currentLane.m_Front.m_Lane, entity, pos);
				}
				if (currentLane.m_Rear.m_Lane != currentLane.m_Front.m_Lane && currentLane.m_Rear.m_Lane != Entity.Null)
				{
					buffer.Add(currentLane.m_Rear.m_Lane, entity, pos2);
				}
				return;
			}
			if (currentLane.m_Front.m_Lane == currentLane.m_Rear.m_Lane)
			{
				if (m_WasCurrentLane1 != Entity.Null)
				{
					buffer.Remove(m_WasCurrentLane1, entity);
				}
				if (m_WasCurrentLane2 != Entity.Null)
				{
					buffer.Remove(m_WasCurrentLane2, entity);
				}
				if (currentLane.m_Front.m_Lane != Entity.Null)
				{
					buffer.Add(currentLane.m_Front.m_Lane, entity, pos);
				}
				return;
			}
			if (currentLane.m_Front.m_Lane != m_WasCurrentLane1)
			{
				if (m_WasCurrentLane1 != Entity.Null)
				{
					buffer.Remove(m_WasCurrentLane1, entity);
				}
				if (currentLane.m_Front.m_Lane != Entity.Null)
				{
					buffer.Add(currentLane.m_Front.m_Lane, entity, pos);
				}
			}
			else if (m_WasCurrentLane1 != Entity.Null && !((float2)(ref m_WasCurvePosition1)).Equals(pos))
			{
				buffer.Update(m_WasCurrentLane1, entity, pos);
			}
			if (currentLane.m_Rear.m_Lane != m_WasCurrentLane2)
			{
				if (m_WasCurrentLane2 != Entity.Null)
				{
					buffer.Remove(m_WasCurrentLane2, entity);
				}
				if (currentLane.m_Rear.m_Lane != Entity.Null)
				{
					buffer.Add(currentLane.m_Rear.m_Lane, entity, pos2);
				}
			}
			else if (m_WasCurrentLane2 != Entity.Null && !((float2)(ref m_WasCurvePosition2)).Equals(pos2))
			{
				buffer.Update(m_WasCurrentLane2, entity, pos2);
			}
		}
	}

	public struct FindLaneIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Bounds3 m_Bounds;

		public float3 m_FrontPivot;

		public float3 m_RearPivot;

		public float2 m_MinDistance;

		public TrainCurrentLane m_Result;

		public TrackTypes m_TrackType;

		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		public ComponentLookup<Game.Net.TrackLane> m_TrackLaneData;

		public ComponentLookup<Curve> m_CurveData;

		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

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
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_SubLanes.HasBuffer(edgeEntity))
			{
				return;
			}
			TrainLaneFlags trainLaneFlags = (TrainLaneFlags)((uint)m_Result.m_Front.m_LaneFlags & 0xFFFEFFFFu);
			TrainLaneFlags trainLaneFlags2 = (TrainLaneFlags)((uint)m_Result.m_Rear.m_LaneFlags & 0xFFFEFFFFu);
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[edgeEntity];
			float num3 = default(float);
			float num4 = default(float);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (!m_TrackLaneData.HasComponent(subLane))
				{
					continue;
				}
				PrefabRef prefabRef = m_PrefabRefData[subLane];
				if ((m_PrefabTrackLaneData[prefabRef.m_Prefab].m_TrackTypes & m_TrackType) == 0)
				{
					continue;
				}
				Bezier4x3 bezier = m_CurveData[subLane].m_Bezier;
				Bounds3 val2 = MathUtils.Bounds(bezier);
				float num = MathUtils.Distance(val2, m_FrontPivot);
				float num2 = MathUtils.Distance(val2, m_RearPivot);
				if (num < m_MinDistance.x)
				{
					num = MathUtils.Distance(bezier, m_FrontPivot, ref num3);
					if (num < m_MinDistance.x)
					{
						TrainLaneFlags trainLaneFlags3 = trainLaneFlags;
						if (m_ConnectionLaneData.HasComponent(subLane))
						{
							trainLaneFlags3 |= TrainLaneFlags.Connection;
						}
						m_MinDistance.x = num;
						m_Result.m_Front = new TrainBogieLane(subLane, float4.op_Implicit(num3), trainLaneFlags3);
						m_Result.m_FrontCache = new TrainBogieCache(m_Result.m_Front);
					}
				}
				if (!(num2 < m_MinDistance.y))
				{
					continue;
				}
				num2 = MathUtils.Distance(bezier, m_RearPivot, ref num4);
				if (num2 < m_MinDistance.y)
				{
					TrainLaneFlags trainLaneFlags4 = trainLaneFlags2;
					if (m_ConnectionLaneData.HasComponent(subLane))
					{
						trainLaneFlags4 |= TrainLaneFlags.Connection;
					}
					m_MinDistance.y = num2;
					m_Result.m_Rear = new TrainBogieLane(subLane, float4.op_Implicit(num4), trainLaneFlags4);
					m_Result.m_RearCache = new TrainBogieCache(m_Result.m_Rear);
				}
			}
			if (m_Result.m_Front.m_Lane != Entity.Null && m_Result.m_Rear.m_Lane == Entity.Null)
			{
				TrainLaneFlags laneFlags = trainLaneFlags2 | (m_Result.m_Front.m_LaneFlags & TrainLaneFlags.Connection);
				m_Result.m_Rear = new TrainBogieLane(m_Result.m_Front.m_Lane, m_Result.m_Front.m_CurvePosition, laneFlags);
				m_Result.m_RearCache = new TrainBogieCache(m_Result.m_Rear);
			}
			else if (m_Result.m_Front.m_Lane == Entity.Null && m_Result.m_Rear.m_Lane != Entity.Null)
			{
				TrainLaneFlags laneFlags2 = trainLaneFlags | (m_Result.m_Rear.m_LaneFlags & TrainLaneFlags.Connection);
				m_Result.m_Front = new TrainBogieLane(m_Result.m_Rear.m_Lane, m_Result.m_Rear.m_CurvePosition, laneFlags2);
				m_Result.m_FrontCache = new TrainBogieCache(m_Result.m_Front);
			}
		}
	}

	public static void GetCurvePositions(ref TrainCurrentLane currentLane, out float2 pos1, out float2 pos2)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		pos1 = ((float4)(ref currentLane.m_Front.m_CurvePosition)).yz;
		pos2 = ((float4)(ref currentLane.m_Rear.m_CurvePosition)).yz;
		if (currentLane.m_Front.m_Lane == currentLane.m_Rear.m_Lane)
		{
			if (pos1.y < pos1.x)
			{
				pos1.y = math.min(pos1.y, pos2.y);
				pos1.x = math.max(pos1.x, pos2.x);
			}
			else
			{
				pos1.x = math.min(pos1.x, pos2.x);
				pos1.y = math.max(pos1.y, pos2.y);
			}
			pos2 = pos1;
		}
	}

	public static void GetCurvePositions(ref ParkedTrain parkedTrain, out float2 pos1, out float2 pos2)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		pos1 = float2.op_Implicit(parkedTrain.m_CurvePosition.x);
		pos2 = float2.op_Implicit(parkedTrain.m_CurvePosition.y);
		if (parkedTrain.m_FrontLane == parkedTrain.m_RearLane)
		{
			pos1.x = math.min(pos1.x, pos2.x);
			pos1.y = math.max(pos1.y, pos2.y);
			pos2 = pos1;
		}
	}
}

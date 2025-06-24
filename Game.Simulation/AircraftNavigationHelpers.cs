using System;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public static class AircraftNavigationHelpers
{
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

		public float m_RelativeSpeed;

		public LaneEffects(Entity lane, float3 sideEffects, float relativeSpeed)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Lane = lane;
			m_SideEffects = sideEffects;
			m_RelativeSpeed = relativeSpeed;
		}
	}

	public struct CurrentLaneCache
	{
		private NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		private Entity m_WasCurrentLane;

		private float2 m_WasCurvePosition;

		private bool m_WasFlying;

		public CurrentLaneCache(ref AircraftCurrentLane currentLane, ComponentLookup<PrefabRef> prefabRefData, NativeQuadTree<Entity, QuadTreeBoundsXZ> searchTree)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (!prefabRefData.HasComponent(currentLane.m_Lane))
			{
				currentLane.m_Lane = Entity.Null;
			}
			m_SearchTree = searchTree;
			m_WasCurrentLane = currentLane.m_Lane;
			m_WasCurvePosition = ((float3)(ref currentLane.m_CurvePosition)).xy;
			m_WasFlying = (currentLane.m_LaneFlags & AircraftLaneFlags.Flying) != 0;
		}

		public void CheckChanges(Entity entity, ref AircraftCurrentLane currentLane, LaneObjectCommandBuffer buffer, BufferLookup<LaneObject> laneObjects, Transform transform, Moving moving, AircraftNavigation navigation, ObjectGeometryData objectGeometryData)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			bool2 val = default(bool2);
			((bool2)(ref val))._002Ector(m_WasFlying, (currentLane.m_LaneFlags & AircraftLaneFlags.Flying) != 0);
			if (currentLane.m_Lane != m_WasCurrentLane)
			{
				if (laneObjects.HasBuffer(m_WasCurrentLane))
				{
					buffer.Remove(m_WasCurrentLane, entity);
				}
				else
				{
					val.x = true;
				}
				if (laneObjects.HasBuffer(currentLane.m_Lane))
				{
					buffer.Add(currentLane.m_Lane, entity, ((float3)(ref currentLane.m_CurvePosition)).xy);
				}
				else
				{
					val.y = true;
				}
			}
			else if (laneObjects.HasBuffer(m_WasCurrentLane))
			{
				if (!((float2)(ref m_WasCurvePosition)).Equals(((float3)(ref currentLane.m_CurvePosition)).xy))
				{
					buffer.Update(m_WasCurrentLane, entity, ((float3)(ref currentLane.m_CurvePosition)).xy);
				}
			}
			else
			{
				val = bool2.op_Implicit(true);
			}
			if (!math.any(val))
			{
				return;
			}
			if (math.all(val))
			{
				QuadTreeBoundsXZ quadTreeBoundsXZ = default(QuadTreeBoundsXZ);
				if (m_SearchTree.TryGet(entity, ref quadTreeBoundsXZ))
				{
					Bounds3 val2 = CalculateMinBounds(transform, moving, navigation, objectGeometryData);
					if (math.any(val2.min < quadTreeBoundsXZ.m_Bounds.min) | math.any(val2.max > quadTreeBoundsXZ.m_Bounds.max))
					{
						buffer.Update(entity, CalculateMaxBounds(transform, moving, navigation, objectGeometryData));
					}
				}
			}
			else if (val.x)
			{
				buffer.Remove(entity);
			}
			else
			{
				buffer.Add(entity, CalculateMaxBounds(transform, moving, navigation, objectGeometryData));
			}
		}

		private Bounds3 CalculateMinBounds(Transform transform, Moving moving, AircraftNavigation navigation, ObjectGeometryData objectGeometryData)
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
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			float3 val = moving.m_Velocity * num;
			float3 val2 = math.normalizesafe(navigation.m_TargetPosition - transform.m_Position, default(float3)) * (navigation.m_MaxSpeed * num);
			Bounds3 result = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, objectGeometryData);
			ref float3 min = ref result.min;
			min += math.min(float3.op_Implicit(0f), math.min(val, val2));
			ref float3 max = ref result.max;
			max += math.max(float3.op_Implicit(0f), math.max(val, val2));
			return result;
		}

		private Bounds3 CalculateMaxBounds(Transform transform, Moving moving, AircraftNavigation navigation, ObjectGeometryData objectGeometryData)
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
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			float num = -1.0666667f;
			float num2 = 2f;
			float num3 = math.length(objectGeometryData.m_Size) * 0.5f;
			float3 val = moving.m_Velocity * num;
			float3 val2 = moving.m_Velocity * num2;
			float3 val3 = math.normalizesafe(navigation.m_TargetPosition - transform.m_Position, default(float3)) * (navigation.m_MaxSpeed * num2);
			float3 position = transform.m_Position;
			position.y += objectGeometryData.m_Size.y * 0.5f;
			Bounds3 result = default(Bounds3);
			result.min = position - num3 + math.min(val, math.min(val2, val3));
			result.max = position + num3 + math.max(val, math.max(val2, val3));
			return result;
		}
	}

	public struct FindLaneIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Bounds3 m_Bounds;

		public float3 m_Position;

		public float m_MinDistance;

		public AircraftCurrentLane m_Result;

		public RoadTypes m_CarType;

		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

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
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_SubLanes.HasBuffer(edgeEntity))
			{
				return;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[edgeEntity];
			float num2 = default(float);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				AircraftLaneFlags laneFlags = m_Result.m_LaneFlags;
				laneFlags = (AircraftLaneFlags)((uint)laneFlags & 0xFFFDFFFBu);
				if (m_CarLaneData.HasComponent(subLane))
				{
					PrefabRef prefabRef = m_PrefabRefData[subLane];
					if (m_PrefabCarLaneData[prefabRef.m_Prefab].m_RoadTypes != m_CarType)
					{
						continue;
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
					laneFlags |= AircraftLaneFlags.Connection;
				}
				Bezier4x3 bezier = m_CurveData[subLane].m_Bezier;
				float num = MathUtils.Distance(MathUtils.Bounds(bezier), m_Position);
				if (num < m_MinDistance)
				{
					num = MathUtils.Distance(bezier, m_Position, ref num2);
					if (num < m_MinDistance)
					{
						m_MinDistance = num;
						m_Result.m_Lane = subLane;
						m_Result.m_CurvePosition = float3.op_Implicit(num2);
						m_Result.m_LaneFlags = laneFlags;
					}
				}
			}
		}

		public void Iterate(ref AirwayHelpers.AirwayData airwayData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			Entity lane = Entity.Null;
			float curvePos = 0f;
			float distance = math.select(m_MinDistance, float.MaxValue, m_Result.m_Lane == Entity.Null);
			if ((m_CarType & RoadTypes.Helicopter) != RoadTypes.None)
			{
				airwayData.helicopterMap.FindClosestLane(m_Position, m_CurveData, ref lane, ref curvePos, ref distance);
			}
			if ((m_CarType & RoadTypes.Airplane) != RoadTypes.None)
			{
				airwayData.airplaneMap.FindClosestLane(m_Position, m_CurveData, ref lane, ref curvePos, ref distance);
			}
			AircraftLaneFlags laneFlags = m_Result.m_LaneFlags;
			laneFlags = (AircraftLaneFlags)((uint)laneFlags & 0xFFFFFFFBu);
			if (lane != Entity.Null)
			{
				m_Result.m_Lane = lane;
				m_Result.m_CurvePosition = float3.op_Implicit(curvePos);
				m_Result.m_LaneFlags = laneFlags | AircraftLaneFlags.Airway;
				m_MinDistance = math.min(m_MinDistance, distance);
			}
		}
	}

	public struct AircraftCollisionIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Entity m_Ignore;

		public Segment m_Line;

		public ComponentLookup<Aircraft> m_AircraftData;

		public ComponentLookup<Transform> m_TransformData;

		public Entity m_Result;

		public float m_ClosestT;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			return MathUtils.Intersect(bounds.m_Bounds, m_Line, ref val);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			if (!MathUtils.Intersect(bounds.m_Bounds, m_Line, ref val) || entity == m_Ignore || !m_AircraftData.HasComponent(entity))
			{
				return;
			}
			Transform transform = m_TransformData[entity];
			if (!(transform.m_Position.y >= m_Line.a.y))
			{
				float num = default(float);
				MathUtils.Distance(m_Line, transform.m_Position, ref num);
				if (num < m_ClosestT)
				{
					m_Result = entity;
					m_ClosestT = num;
				}
			}
		}
	}
}

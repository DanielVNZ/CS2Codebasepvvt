using System;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public static class WatercraftNavigationHelpers
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

		private Entity m_WasChangeLane;

		private float2 m_WasCurvePosition;

		public CurrentLaneCache(ref WatercraftCurrentLane currentLane, EntityStorageInfoLookup entityLookup, NativeQuadTree<Entity, QuadTreeBoundsXZ> searchTree)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (!((EntityStorageInfoLookup)(ref entityLookup)).Exists(currentLane.m_Lane))
			{
				currentLane.m_Lane = Entity.Null;
			}
			if (!((EntityStorageInfoLookup)(ref entityLookup)).Exists(currentLane.m_ChangeLane))
			{
				currentLane.m_ChangeLane = Entity.Null;
			}
			m_SearchTree = searchTree;
			m_WasCurrentLane = currentLane.m_Lane;
			m_WasChangeLane = currentLane.m_ChangeLane;
			m_WasCurvePosition = ((float3)(ref currentLane.m_CurvePosition)).xy;
		}

		public void CheckChanges(Entity entity, ref WatercraftCurrentLane currentLane, LaneObjectCommandBuffer buffer, BufferLookup<LaneObject> laneObjects, Transform transform, Moving moving, WatercraftNavigation navigation, ObjectGeometryData objectGeometryData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			if (currentLane.m_Lane == m_WasChangeLane)
			{
				QuadTreeBoundsXZ quadTreeBoundsXZ = default(QuadTreeBoundsXZ);
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
				else if (m_SearchTree.TryGet(entity, ref quadTreeBoundsXZ))
				{
					Bounds3 val = CalculateMinBounds(transform, moving, navigation, objectGeometryData);
					if (math.any(val.min < quadTreeBoundsXZ.m_Bounds.min) | math.any(val.max > quadTreeBoundsXZ.m_Bounds.max))
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
			QuadTreeBoundsXZ quadTreeBoundsXZ2 = default(QuadTreeBoundsXZ);
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
			else if (m_SearchTree.TryGet(entity, ref quadTreeBoundsXZ2))
			{
				Bounds3 val2 = CalculateMinBounds(transform, moving, navigation, objectGeometryData);
				if (math.any(val2.min < quadTreeBoundsXZ2.m_Bounds.min) | math.any(val2.max > quadTreeBoundsXZ2.m_Bounds.max))
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

		private Bounds3 CalculateMinBounds(Transform transform, Moving moving, WatercraftNavigation navigation, ObjectGeometryData objectGeometryData)
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

		private Bounds3 CalculateMaxBounds(Transform transform, Moving moving, WatercraftNavigation navigation, ObjectGeometryData objectGeometryData)
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

	public struct FindLaneIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
	{
		public Bounds3 m_Bounds;

		public float3 m_Position;

		public float m_MinDistance;

		public WatercraftCurrentLane m_Result;

		public RoadTypes m_CarType;

		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		public BufferLookup<Triangle> m_AreaTriangles;

		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		public ComponentLookup<MasterLane> m_MasterLaneData;

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
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_SubLanes.HasBuffer(edgeEntity))
			{
				return;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[edgeEntity];
			float num2 = default(float);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				WatercraftLaneFlags watercraftLaneFlags = m_Result.m_LaneFlags | WatercraftLaneFlags.FixedLane;
				watercraftLaneFlags = (WatercraftLaneFlags)((uint)watercraftLaneFlags & 0xFFFCFFFFu);
				if (!m_CarLaneData.HasComponent(subLane) || m_MasterLaneData.HasComponent(subLane))
				{
					continue;
				}
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
					watercraftLaneFlags |= WatercraftLaneFlags.Connection;
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
						m_Result.m_LaneFlags = watercraftLaneFlags;
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
				WatercraftLaneFlags watercraftLaneFlags = m_Result.m_LaneFlags | (WatercraftLaneFlags.FixedLane | WatercraftLaneFlags.Area);
				watercraftLaneFlags = (WatercraftLaneFlags)((uint)watercraftLaneFlags & 0xFFFEFFFFu);
				m_Bounds = new Bounds3(m_Position - num, m_Position + num);
				m_MinDistance = num;
				m_Result.m_Lane = val4;
				m_Result.m_CurvePosition = float3.op_Implicit(num3);
				m_Result.m_LaneFlags = watercraftLaneFlags;
			}
		}
	}
}

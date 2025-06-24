using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public static class AnimalNavigationHelpers
{
	public struct CurrentLaneCache
	{
		private NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		private Entity m_WasCurrentLane;

		private float m_WasCurvePosition;

		private bool m_WasEndReached;

		public CurrentLaneCache(ref AnimalCurrentLane currentLane, ComponentLookup<PrefabRef> prefabRefData, NativeQuadTree<Entity, QuadTreeBoundsXZ> searchTree)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			if (!prefabRefData.HasComponent(currentLane.m_Lane))
			{
				currentLane.m_Lane = Entity.Null;
			}
			if (currentLane.m_NextLane != Entity.Null && !prefabRefData.HasComponent(currentLane.m_NextLane))
			{
				currentLane.m_NextLane = Entity.Null;
			}
			m_SearchTree = searchTree;
			m_WasCurrentLane = currentLane.m_Lane;
			m_WasCurvePosition = currentLane.m_CurvePosition.x;
			m_WasEndReached = (currentLane.m_Flags & CreatureLaneFlags.EndReached) != 0;
		}

		public void CheckChanges(Entity entity, ref AnimalCurrentLane currentLane, LaneObjectCommandBuffer buffer, BufferLookup<LaneObject> laneObjects, Transform transform, Moving moving, AnimalNavigation navigation, ObjectGeometryData objectGeometryData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			QuadTreeBoundsXZ quadTreeBoundsXZ = default(QuadTreeBoundsXZ);
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
					buffer.Add(currentLane.m_Lane, entity, ((float2)(ref currentLane.m_CurvePosition)).xx);
				}
				else
				{
					buffer.Add(entity, CalculateMaxBounds(transform, moving, navigation, objectGeometryData));
				}
			}
			else if (laneObjects.HasBuffer(m_WasCurrentLane))
			{
				if (!m_WasCurvePosition.Equals(currentLane.m_CurvePosition.x))
				{
					buffer.Update(m_WasCurrentLane, entity, ((float2)(ref currentLane.m_CurvePosition)).xx);
				}
			}
			else if (m_SearchTree.TryGet(entity, ref quadTreeBoundsXZ))
			{
				Bounds3 val = CalculateMinBounds(transform, moving, navigation, objectGeometryData);
				if (math.any(val.min < quadTreeBoundsXZ.m_Bounds.min) | math.any(val.max > quadTreeBoundsXZ.m_Bounds.max) | (((currentLane.m_Flags & CreatureLaneFlags.EndReached) != 0) & !m_WasEndReached))
				{
					buffer.Update(entity, CalculateMaxBounds(transform, moving, navigation, objectGeometryData));
				}
			}
		}

		private Bounds3 CalculateMinBounds(Transform transform, Moving moving, AnimalNavigation navigation, ObjectGeometryData objectGeometryData)
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

		private Bounds3 CalculateMaxBounds(Transform transform, Moving moving, AnimalNavigation navigation, ObjectGeometryData objectGeometryData)
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

		public bool m_UnspawnedEmerge;

		public AnimalCurrentLane m_Result;

		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		public BufferLookup<Triangle> m_AreaTriangles;

		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		public ComponentLookup<Curve> m_CurveData;

		public ComponentLookup<HangaroundLocation> m_HangaroundLocationData;

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
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_SubLanes.HasBuffer(ownerEntity))
			{
				return;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[ownerEntity];
			float num2 = default(float);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				CreatureLaneFlags creatureLaneFlags = (CreatureLaneFlags)((uint)m_Result.m_Flags & 0xFFFFCFBFu);
				if (!m_PedestrianLaneData.HasComponent(subLane))
				{
					if (!m_ConnectionLaneData.HasComponent(subLane) || (m_ConnectionLaneData[subLane].m_Flags & ConnectionLaneFlags.Pedestrian) == 0)
					{
						continue;
					}
					creatureLaneFlags |= CreatureLaneFlags.Connection;
				}
				if (m_UnspawnedEmerge && (creatureLaneFlags & CreatureLaneFlags.Connection) == 0)
				{
					continue;
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
						m_Result.m_CurvePosition = float2.op_Implicit(num2);
						m_Result.m_Flags = creatureLaneFlags;
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
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
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
			CreatureLaneFlags flags = (CreatureLaneFlags)0u;
			CreatureLaneFlags creatureLaneFlags = (CreatureLaneFlags)(((uint)m_Result.m_Flags & 0xFFFFDFBFu) | 0x1000);
			if (m_HangaroundLocationData.HasComponent(item.m_Area))
			{
				creatureLaneFlags |= CreatureLaneFlags.Hangaround;
			}
			float2 val5 = default(float2);
			float num5 = default(float);
			for (int i = 0; i < val3.Length; i++)
			{
				Entity subLane = val3[i].m_SubLane;
				if (!m_ConnectionLaneData.HasComponent(subLane) || (m_ConnectionLaneData[subLane].m_Flags & ConnectionLaneFlags.Pedestrian) == 0)
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
						flags = creatureLaneFlags;
					}
				}
			}
			if (val4 != Entity.Null)
			{
				m_Bounds = new Bounds3(m_Position - num, m_Position + num);
				m_MinDistance = num;
				m_Result.m_Lane = val4;
				m_Result.m_CurvePosition = float2.op_Implicit(num3);
				m_Result.m_Flags = flags;
			}
		}
	}
}

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

public static class HumanNavigationHelpers
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

	public struct CurrentLaneCache
	{
		private NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		private Entity m_WasCurrentLane;

		private float m_WasCurvePosition;

		private bool m_WasEndReached;

		public CurrentLaneCache(ref HumanCurrentLane currentLane, EntityStorageInfoLookup entityLookup, NativeQuadTree<Entity, QuadTreeBoundsXZ> searchTree)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (!((EntityStorageInfoLookup)(ref entityLookup)).Exists(currentLane.m_Lane))
			{
				currentLane.m_Lane = Entity.Null;
			}
			m_SearchTree = searchTree;
			m_WasCurrentLane = currentLane.m_Lane;
			m_WasCurvePosition = currentLane.m_CurvePosition.x;
			m_WasEndReached = (currentLane.m_Flags & CreatureLaneFlags.EndReached) != 0;
		}

		public void CheckChanges(Entity entity, ref HumanCurrentLane currentLane, LaneObjectCommandBuffer buffer, BufferLookup<LaneObject> laneObjects, Transform transform, Moving moving, HumanNavigation navigation, ObjectGeometryData objectGeometryData)
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

		private Bounds3 CalculateMinBounds(Transform transform, Moving moving, HumanNavigation navigation, ObjectGeometryData objectGeometryData)
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

		private Bounds3 CalculateMaxBounds(Transform transform, Moving moving, HumanNavigation navigation, ObjectGeometryData objectGeometryData)
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

		public HumanCurrentLane m_Result;

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
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
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
			bool2 val5 = default(bool2);
			float2 val6 = default(float2);
			float num5 = default(float);
			for (int i = 0; i < val3.Length; i++)
			{
				Entity subLane = val3[i].m_SubLane;
				if (!m_ConnectionLaneData.HasComponent(subLane) || (m_ConnectionLaneData[subLane].m_Flags & ConnectionLaneFlags.Pedestrian) == 0)
				{
					continue;
				}
				Curve curve = m_CurveData[subLane];
				((bool2)(ref val5))._002Ector(MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.a)).xz, ref val6), MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.d)).xz, ref val6));
				if (math.any(val5))
				{
					float num4 = MathUtils.Distance(curve.m_Bezier, val2, ref num5);
					if (num4 < num2)
					{
						float2 val7 = math.select(new float2(0f, 0.49f), math.select(new float2(0.51f, 1f), new float2(0f, 1f), val5.x), val5.y);
						num2 = num4;
						val4 = subLane;
						num3 = math.clamp(num5, val7.x, val7.y);
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

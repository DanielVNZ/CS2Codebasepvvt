using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Areas;

public static class ValidationHelpers
{
	private struct OriginalAreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
	{
		public Entity m_AreaEntity;

		public Bounds2 m_Bounds;

		public float2 m_Position;

		public float m_Offset;

		public bool m_Result;

		public DynamicBuffer<Node> m_Nodes;

		public DynamicBuffer<Triangle> m_Triangles;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (!m_Result)
			{
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}
			return false;
		}

		public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem2)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			if (!m_Result && !(areaItem2.m_Area != m_AreaEntity) && MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds))
			{
				Triangle2 triangle = AreaUtils.GetTriangle2(m_Nodes, m_Triangles[areaItem2.m_Triangle]);
				m_Result = MathUtils.Intersect(triangle, new Circle2(m_Offset, m_Position));
			}
		}
	}

	private struct ObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Entity m_AreaEntity;

		public Entity m_OriginalAreaEntity;

		public Entity m_IgnoreEntity;

		public Entity m_IgnoreEntity2;

		public Bounds3 m_TriangleBounds;

		public Bounds1 m_HeightRange;

		public Triangle3 m_Triangle;

		public ErrorSeverity m_ErrorSeverity;

		public CollisionMask m_CollisionMask;

		public AreaGeometryData m_PrefabAreaData;

		public ValidationSystem.EntityData m_Data;

		public ParallelWriter<ErrorData> m_ErrorQueue;

		public bool m_EditorMode;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if ((bounds.m_Mask & BoundsMask.NotOverridden) == 0)
			{
				return false;
			}
			return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_TriangleBounds)).xz);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity objectEntity2)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0520: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_099a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08db: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0824: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0856: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0929: Unknown result type (might be due to invalid IL or missing references)
			//IL_0931: Unknown result type (might be due to invalid IL or missing references)
			//IL_0908: Unknown result type (might be due to invalid IL or missing references)
			//IL_0910: Unknown result type (might be due to invalid IL or missing references)
			//IL_0915: Unknown result type (might be due to invalid IL or missing references)
			//IL_0919: Unknown result type (might be due to invalid IL or missing references)
			//IL_0889: Unknown result type (might be due to invalid IL or missing references)
			//IL_088e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0862: Unknown result type (might be due to invalid IL or missing references)
			//IL_086a: Unknown result type (might be due to invalid IL or missing references)
			//IL_086f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0873: Unknown result type (might be due to invalid IL or missing references)
			//IL_064a: Unknown result type (might be due to invalid IL or missing references)
			//IL_064f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_066c: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0944: Unknown result type (might be due to invalid IL or missing references)
			//IL_094a: Unknown result type (might be due to invalid IL or missing references)
			//IL_094f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0954: Unknown result type (might be due to invalid IL or missing references)
			//IL_0959: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0733: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_075f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_0773: Unknown result type (might be due to invalid IL or missing references)
			//IL_0778: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
			if ((bounds.m_Mask & BoundsMask.NotOverridden) == 0 || !MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_TriangleBounds)).xz) || m_Data.m_Hidden.HasComponent(objectEntity2) || objectEntity2 == m_IgnoreEntity)
			{
				return;
			}
			Entity val = objectEntity2;
			bool flag = false;
			Owner owner = default(Owner);
			while (m_Data.m_Owner.TryGetComponent(val, ref owner))
			{
				val = owner.m_Owner;
				flag = true;
				if (val == m_AreaEntity || val == m_OriginalAreaEntity)
				{
					return;
				}
			}
			if (val == m_IgnoreEntity || val == m_IgnoreEntity2)
			{
				return;
			}
			PrefabRef prefabRef = m_Data.m_PrefabRef[objectEntity2];
			Transform transform = m_Data.m_Transform[objectEntity2];
			if (!m_Data.m_PrefabObjectGeometry.HasComponent(prefabRef.m_Prefab))
			{
				return;
			}
			ObjectGeometryData objectGeometryData = m_Data.m_PrefabObjectGeometry[prefabRef.m_Prefab];
			CollisionMask collisionMask = ((!m_Data.m_ObjectElevation.HasComponent(objectEntity2)) ? ObjectUtils.GetCollisionMask(objectGeometryData, !m_EditorMode || flag) : ObjectUtils.GetCollisionMask(objectGeometryData, m_Data.m_ObjectElevation[objectEntity2], !m_EditorMode || flag));
			if ((m_CollisionMask & collisionMask) == 0)
			{
				return;
			}
			ErrorData errorData = new ErrorData
			{
				m_ErrorSeverity = m_ErrorSeverity,
				m_TempEntity = m_AreaEntity,
				m_PermanentEntity = objectEntity2
			};
			if (val != objectEntity2)
			{
				if ((objectGeometryData.m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden)) == Game.Objects.GeometryFlags.Overridable)
				{
					if ((m_PrefabAreaData.m_Flags & GeometryFlags.CanOverrideObjects) == 0)
					{
						return;
					}
					errorData.m_ErrorSeverity = ErrorSeverity.Override;
				}
				else
				{
					PrefabRef prefabRef2 = m_Data.m_PrefabRef[val];
					if (m_Data.m_PrefabObjectGeometry.HasComponent(prefabRef2.m_Prefab))
					{
						ObjectGeometryData objectGeometryData2 = m_Data.m_PrefabObjectGeometry[prefabRef2.m_Prefab];
						if ((objectGeometryData2.m_Flags & Game.Objects.GeometryFlags.Overridable) != Game.Objects.GeometryFlags.None)
						{
							if ((objectGeometryData2.m_Flags & Game.Objects.GeometryFlags.DeleteOverridden) == 0)
							{
								return;
							}
							if (!m_Data.m_Attached.HasComponent(val))
							{
								errorData.m_ErrorSeverity = ErrorSeverity.Warning;
								errorData.m_PermanentEntity = val;
							}
						}
					}
				}
			}
			else if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Overridable) != Game.Objects.GeometryFlags.None)
			{
				if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.DeleteOverridden) != Game.Objects.GeometryFlags.None)
				{
					if (!m_Data.m_Attached.HasComponent(objectEntity2))
					{
						errorData.m_ErrorSeverity = ErrorSeverity.Warning;
					}
				}
				else
				{
					if ((m_PrefabAreaData.m_Flags & GeometryFlags.CanOverrideObjects) == 0)
					{
						return;
					}
					errorData.m_ErrorSeverity = ErrorSeverity.Override;
				}
			}
			if ((collisionMask & CollisionMask.OnGround) == 0 || MathUtils.Intersect(m_TriangleBounds, bounds.m_Bounds))
			{
				float3 val2 = math.mul(math.inverse(transform.m_Rotation), transform.m_Position);
				Bounds3 bounds2 = objectGeometryData.m_Bounds;
				if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.IgnoreBottomCollision) != Game.Objects.GeometryFlags.None)
				{
					bounds2.min.y = math.max(bounds2.min.y, 0f);
				}
				if (ObjectUtils.GetStandingLegCount(objectGeometryData, out var legCount))
				{
					Bounds3 val4 = default(Bounds3);
					Bounds3 val5 = default(Bounds3);
					for (int i = 0; i < legCount; i++)
					{
						if ((objectGeometryData.m_Flags & (Game.Objects.GeometryFlags.CircularLeg | Game.Objects.GeometryFlags.IgnoreLegCollision)) == Game.Objects.GeometryFlags.CircularLeg)
						{
							float3 val3 = val2 + ObjectUtils.GetStandingLegOffset(objectGeometryData, i);
							if (Game.Net.ValidationHelpers.TriangleCylinderIntersect(cylinder2: new Cylinder3
							{
								circle = new Circle2(objectGeometryData.m_LegSize.x * 0.5f, ((float3)(ref val3)).xz),
								height = new Bounds1(bounds2.min.y, objectGeometryData.m_LegSize.y) + val3.y,
								rotation = transform.m_Rotation
							}, triangle1: m_Triangle, intersection1: out var intersection, intersection2: out var intersection2))
							{
								intersection = Game.Net.ValidationHelpers.SetHeightRange(intersection, m_HeightRange);
								if (MathUtils.Intersect(intersection, intersection2, ref val4))
								{
									errorData.m_Position = MathUtils.Center(val4);
									errorData.m_ErrorType = ErrorType.OverlapExisting;
								}
							}
						}
						else
						{
							if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.IgnoreLegCollision) != Game.Objects.GeometryFlags.None)
							{
								continue;
							}
							float3 standingLegPosition = ObjectUtils.GetStandingLegPosition(objectGeometryData, transform, i);
							((float3)(ref bounds2.min)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * -0.5f;
							((float3)(ref bounds2.max)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f;
							if (Game.Net.ValidationHelpers.QuadTriangleIntersect(ObjectUtils.CalculateBaseCorners(standingLegPosition, transform.m_Rotation, bounds2), m_Triangle, out var intersection3, out var intersection4))
							{
								intersection4 = Game.Net.ValidationHelpers.SetHeightRange(intersection4, m_HeightRange);
								intersection3 = Game.Net.ValidationHelpers.SetHeightRange(intersection3, ((Bounds3)(ref bounds2)).y);
								if (MathUtils.Intersect(intersection4, intersection3, ref val5))
								{
									errorData.m_Position = MathUtils.Center(val5);
									errorData.m_ErrorType = ErrorType.OverlapExisting;
								}
							}
						}
					}
					bounds2.min.y = objectGeometryData.m_LegSize.y;
				}
				Bounds3 intersection8;
				Bounds3 intersection7;
				if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
				{
					if (Game.Net.ValidationHelpers.TriangleCylinderIntersect(cylinder2: new Cylinder3
					{
						circle = new Circle2(objectGeometryData.m_Size.x * 0.5f, ((float3)(ref val2)).xz),
						height = new Bounds1(bounds2.min.y, bounds2.max.y) + val2.y,
						rotation = transform.m_Rotation
					}, triangle1: m_Triangle, intersection1: out var intersection5, intersection2: out var intersection6))
					{
						intersection5 = Game.Net.ValidationHelpers.SetHeightRange(intersection5, m_HeightRange);
						Bounds3 val6 = default(Bounds3);
						if (MathUtils.Intersect(intersection5, intersection6, ref val6))
						{
							errorData.m_Position = MathUtils.Center(val6);
							errorData.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
				}
				else if (Game.Net.ValidationHelpers.QuadTriangleIntersect(ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, objectGeometryData.m_Bounds), m_Triangle, out intersection7, out intersection8))
				{
					intersection8 = Game.Net.ValidationHelpers.SetHeightRange(intersection8, m_HeightRange);
					intersection7 = Game.Net.ValidationHelpers.SetHeightRange(intersection7, ((Bounds3)(ref bounds2)).y);
					Bounds3 val7 = default(Bounds3);
					if (MathUtils.Intersect(intersection8, intersection7, ref val7))
					{
						errorData.m_Position = MathUtils.Center(val7);
						errorData.m_ErrorType = ErrorType.OverlapExisting;
					}
				}
			}
			if (errorData.m_ErrorType == ErrorType.None && CommonUtils.ExclusiveGroundCollision(collisionMask, m_CollisionMask))
			{
				Triangle2 xz;
				float2 val10 = default(float2);
				Quad3 val11;
				if (ObjectUtils.GetStandingLegCount(objectGeometryData, out var legCount2))
				{
					Circle2 val8 = default(Circle2);
					for (int j = 0; j < legCount2; j++)
					{
						float3 standingLegPosition2 = ObjectUtils.GetStandingLegPosition(objectGeometryData, transform, j);
						if ((objectGeometryData.m_Flags & (Game.Objects.GeometryFlags.CircularLeg | Game.Objects.GeometryFlags.IgnoreLegCollision)) == Game.Objects.GeometryFlags.CircularLeg)
						{
							((Circle2)(ref val8))._002Ector(objectGeometryData.m_LegSize.x * 0.5f, ((float3)(ref standingLegPosition2)).xz);
							bool flag2;
							if (((float3)(ref m_Triangle.c)).Equals(m_Triangle.b))
							{
								Circle2 val9 = val8;
								xz = ((Triangle3)(ref m_Triangle)).xz;
								flag2 = MathUtils.Intersect(val9, ((Triangle2)(ref xz)).ab, ref val10);
							}
							else
							{
								flag2 = MathUtils.Intersect(((Triangle3)(ref m_Triangle)).xz, val8);
							}
							if (flag2)
							{
								errorData.m_Position = MathUtils.Center(bounds.m_Bounds & m_TriangleBounds);
								errorData.m_ErrorType = ErrorType.OverlapExisting;
							}
						}
						else if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.IgnoreLegCollision) == 0)
						{
							Bounds3 bounds3 = default(Bounds3);
							((float3)(ref bounds3.min)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * -0.5f;
							((float3)(ref bounds3.max)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f;
							val11 = ObjectUtils.CalculateBaseCorners(standingLegPosition2, transform.m_Rotation, bounds3);
							Quad2 xz2 = ((Quad3)(ref val11)).xz;
							bool flag3;
							if (((float3)(ref m_Triangle.c)).Equals(m_Triangle.b))
							{
								xz = ((Triangle3)(ref m_Triangle)).xz;
								flag3 = MathUtils.Intersect(xz2, ((Triangle2)(ref xz)).ab, ref val10);
							}
							else
							{
								flag3 = MathUtils.Intersect(xz2, ((Triangle3)(ref m_Triangle)).xz);
							}
							if (flag3)
							{
								errorData.m_Position = MathUtils.Center(bounds.m_Bounds & m_TriangleBounds);
								errorData.m_ErrorType = ErrorType.OverlapExisting;
							}
						}
					}
				}
				else if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
				{
					Circle2 val12 = default(Circle2);
					((Circle2)(ref val12))._002Ector(objectGeometryData.m_Size.x * 0.5f, ((float3)(ref transform.m_Position)).xz);
					bool flag4;
					if (((float3)(ref m_Triangle.c)).Equals(m_Triangle.b))
					{
						Circle2 val13 = val12;
						xz = ((Triangle3)(ref m_Triangle)).xz;
						flag4 = MathUtils.Intersect(val13, ((Triangle2)(ref xz)).ab, ref val10);
					}
					else
					{
						flag4 = MathUtils.Intersect(((Triangle3)(ref m_Triangle)).xz, val12);
					}
					if (flag4)
					{
						errorData.m_Position = MathUtils.Center(bounds.m_Bounds & m_TriangleBounds);
						errorData.m_ErrorType = ErrorType.OverlapExisting;
					}
				}
				else
				{
					val11 = ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, objectGeometryData.m_Bounds);
					Quad2 xz3 = ((Quad3)(ref val11)).xz;
					bool flag5;
					if (((float3)(ref m_Triangle.c)).Equals(m_Triangle.b))
					{
						xz = ((Triangle3)(ref m_Triangle)).xz;
						flag5 = MathUtils.Intersect(xz3, ((Triangle2)(ref xz)).ab, ref val10);
					}
					else
					{
						flag5 = MathUtils.Intersect(xz3, ((Triangle3)(ref m_Triangle)).xz);
					}
					if (flag5)
					{
						errorData.m_Position = MathUtils.Center(bounds.m_Bounds & m_TriangleBounds);
						errorData.m_ErrorType = ErrorType.OverlapExisting;
					}
				}
			}
			if (errorData.m_ErrorType != ErrorType.None)
			{
				if ((errorData.m_ErrorSeverity == ErrorSeverity.Override || errorData.m_ErrorSeverity == ErrorSeverity.Warning) && errorData.m_ErrorType == ErrorType.OverlapExisting && m_Data.m_OnFire.HasComponent(errorData.m_PermanentEntity))
				{
					errorData.m_ErrorType = ErrorType.OnFire;
					errorData.m_ErrorSeverity = ErrorSeverity.Error;
				}
				m_ErrorQueue.Enqueue(errorData);
			}
		}
	}

	private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Entity m_AreaEntity;

		public Entity m_OriginalAreaEntity;

		public Entity m_IgnoreEntity;

		public Entity m_IgnoreEntity2;

		public Bounds3 m_TriangleBounds;

		public Bounds1 m_HeightRange;

		public Triangle3 m_Triangle;

		public ErrorSeverity m_ErrorSeverity;

		public CollisionMask m_CollisionMask;

		public ValidationSystem.EntityData m_Data;

		public ParallelWriter<ErrorData> m_ErrorQueue;

		public bool m_EditorMode;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_TriangleBounds)).xz);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity edgeEntity2)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_TriangleBounds)).xz) || m_Data.m_Hidden.HasComponent(edgeEntity2))
			{
				return;
			}
			Entity val = edgeEntity2;
			bool flag = false;
			Owner owner = default(Owner);
			while (m_Data.m_Owner.TryGetComponent(val, ref owner))
			{
				val = owner.m_Owner;
				flag = true;
				if (val == m_AreaEntity || val == m_OriginalAreaEntity)
				{
					return;
				}
			}
			if (val == m_IgnoreEntity || val == m_IgnoreEntity2 || !m_Data.m_Composition.HasComponent(edgeEntity2))
			{
				return;
			}
			Edge edge = m_Data.m_Edge[edgeEntity2];
			Composition composition = m_Data.m_Composition[edgeEntity2];
			EdgeGeometry edgeGeometry = m_Data.m_EdgeGeometry[edgeEntity2];
			StartNodeGeometry startNodeGeometry = m_Data.m_StartNodeGeometry[edgeEntity2];
			EndNodeGeometry endNodeGeometry = m_Data.m_EndNodeGeometry[edgeEntity2];
			NetCompositionData netCompositionData = m_Data.m_PrefabComposition[composition.m_Edge];
			NetCompositionData netCompositionData2 = m_Data.m_PrefabComposition[composition.m_StartNode];
			NetCompositionData netCompositionData3 = m_Data.m_PrefabComposition[composition.m_EndNode];
			CollisionMask collisionMask = NetUtils.GetCollisionMask(netCompositionData, !m_EditorMode || flag);
			CollisionMask collisionMask2 = NetUtils.GetCollisionMask(netCompositionData2, !m_EditorMode || flag);
			CollisionMask collisionMask3 = NetUtils.GetCollisionMask(netCompositionData3, !m_EditorMode || flag);
			CollisionMask collisionMask4 = collisionMask | collisionMask2 | collisionMask3;
			if ((m_CollisionMask & collisionMask4) == 0)
			{
				return;
			}
			ErrorData errorData = default(ErrorData);
			Bounds3 intersection = default(Bounds3);
			intersection.min = float3.op_Implicit(float.MaxValue);
			intersection.max = float3.op_Implicit(float.MinValue);
			bool flag2 = ((float3)(ref m_Triangle.c)).Equals(m_Triangle.b);
			if ((collisionMask4 & CollisionMask.OnGround) == 0 || MathUtils.Intersect(m_TriangleBounds, bounds.m_Bounds))
			{
				if ((collisionMask & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge, m_AreaEntity, edgeGeometry, m_Triangle, netCompositionData, m_HeightRange, ref intersection))
				{
					errorData.m_ErrorType = ErrorType.OverlapExisting;
				}
				if ((collisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge.m_Start, m_AreaEntity, startNodeGeometry.m_Geometry, m_Triangle, netCompositionData2, m_HeightRange, ref intersection))
				{
					errorData.m_ErrorType = ErrorType.OverlapExisting;
				}
				if ((collisionMask3 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge.m_End, m_AreaEntity, endNodeGeometry.m_Geometry, m_Triangle, netCompositionData3, m_HeightRange, ref intersection))
				{
					errorData.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			if (errorData.m_ErrorType == ErrorType.None && CommonUtils.ExclusiveGroundCollision(collisionMask4, m_CollisionMask))
			{
				Triangle2 xz;
				if ((collisionMask & m_CollisionMask) != 0)
				{
					if (flag2)
					{
						Edge edge2 = edge;
						Entity areaEntity = m_AreaEntity;
						xz = ((Triangle3)(ref m_Triangle)).xz;
						if (Game.Net.ValidationHelpers.Intersect(edge2, areaEntity, edgeGeometry, ((Triangle2)(ref xz)).ab, m_TriangleBounds, netCompositionData, ref intersection))
						{
							errorData.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
					else if (Game.Net.ValidationHelpers.Intersect(edge, m_AreaEntity, edgeGeometry, ((Triangle3)(ref m_Triangle)).xz, m_TriangleBounds, netCompositionData, ref intersection))
					{
						errorData.m_ErrorType = ErrorType.OverlapExisting;
					}
				}
				if ((collisionMask2 & m_CollisionMask) != 0)
				{
					if (flag2)
					{
						Entity start = edge.m_Start;
						Entity areaEntity2 = m_AreaEntity;
						EdgeNodeGeometry geometry = startNodeGeometry.m_Geometry;
						xz = ((Triangle3)(ref m_Triangle)).xz;
						if (Game.Net.ValidationHelpers.Intersect(start, areaEntity2, geometry, ((Triangle2)(ref xz)).ab, m_TriangleBounds, netCompositionData2, ref intersection))
						{
							errorData.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
					else if (Game.Net.ValidationHelpers.Intersect(edge.m_Start, m_AreaEntity, startNodeGeometry.m_Geometry, ((Triangle3)(ref m_Triangle)).xz, m_TriangleBounds, netCompositionData2, ref intersection))
					{
						errorData.m_ErrorType = ErrorType.OverlapExisting;
					}
				}
				if ((collisionMask3 & m_CollisionMask) != 0)
				{
					if (flag2)
					{
						Entity end = edge.m_End;
						Entity areaEntity3 = m_AreaEntity;
						EdgeNodeGeometry geometry2 = endNodeGeometry.m_Geometry;
						xz = ((Triangle3)(ref m_Triangle)).xz;
						if (Game.Net.ValidationHelpers.Intersect(end, areaEntity3, geometry2, ((Triangle2)(ref xz)).ab, m_TriangleBounds, netCompositionData3, ref intersection))
						{
							errorData.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
					else if (Game.Net.ValidationHelpers.Intersect(edge.m_End, m_AreaEntity, endNodeGeometry.m_Geometry, ((Triangle3)(ref m_Triangle)).xz, m_TriangleBounds, netCompositionData3, ref intersection))
					{
						errorData.m_ErrorType = ErrorType.OverlapExisting;
					}
				}
			}
			if (errorData.m_ErrorType != ErrorType.None)
			{
				errorData.m_ErrorSeverity = m_ErrorSeverity;
				errorData.m_TempEntity = m_AreaEntity;
				errorData.m_PermanentEntity = edgeEntity2;
				errorData.m_Position = MathUtils.Center(intersection);
				m_ErrorQueue.Enqueue(errorData);
			}
		}
	}

	private struct AreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
	{
		public Entity m_AreaEntity;

		public Entity m_IgnoreEntity;

		public Entity m_IgnoreEntity2;

		public Entity m_TopLevelEntity;

		public Bounds3 m_TriangleBounds;

		public Triangle2 m_Triangle;

		public bool m_IgnoreCollisions;

		public bool m_EditorMode;

		public bool m_Essential;

		public AreaGeometryData m_PrefabAreaData;

		public ErrorSeverity m_ErrorSeverity;

		public ValidationSystem.EntityData m_Data;

		public ParallelWriter<ErrorData> m_ErrorQueue;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_TriangleBounds)).xz);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem2)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_TriangleBounds)).xz) || m_Data.m_Hidden.HasComponent(areaItem2.m_Area))
			{
				return;
			}
			Area area = m_Data.m_Area[areaItem2.m_Area];
			if ((area.m_Flags & AreaFlags.Slave) != 0)
			{
				return;
			}
			Entity val = areaItem2.m_Area;
			bool flag = false;
			Owner owner = default(Owner);
			while (m_Data.m_Owner.TryGetComponent(val, ref owner) && !m_Data.m_Building.HasComponent(val))
			{
				Entity owner2 = owner.m_Owner;
				flag = true;
				if (m_Data.m_AssetStamp.HasComponent(owner2))
				{
					break;
				}
				val = owner2;
			}
			if (val == m_TopLevelEntity)
			{
				return;
			}
			if (m_IgnoreEntity != Entity.Null)
			{
				Entity val2 = val;
				while (m_Data.m_Owner.HasComponent(val2))
				{
					val2 = m_Data.m_Owner[val2].m_Owner;
				}
				if (val2 == m_IgnoreEntity || val2 == m_IgnoreEntity2)
				{
					return;
				}
			}
			PrefabRef prefabRef = m_Data.m_PrefabRef[areaItem2.m_Area];
			AreaGeometryData areaGeometryData = m_Data.m_PrefabAreaGeometry[prefabRef.m_Prefab];
			AreaUtils.SetCollisionFlags(ref areaGeometryData, !m_EditorMode || flag);
			if (areaGeometryData.m_Type != m_PrefabAreaData.m_Type)
			{
				if ((areaGeometryData.m_Flags & (GeometryFlags.PhysicalGeometry | GeometryFlags.ProtectedArea)) == 0)
				{
					return;
				}
				if ((areaGeometryData.m_Flags & GeometryFlags.ProtectedArea) != 0)
				{
					if (!m_Data.m_Native.HasComponent(areaItem2.m_Area))
					{
						return;
					}
				}
				else if (m_IgnoreCollisions || ((areaGeometryData.m_Flags & GeometryFlags.PhysicalGeometry) != 0 && (m_PrefabAreaData.m_Flags & GeometryFlags.PhysicalGeometry) == 0))
				{
					return;
				}
			}
			else if ((areaGeometryData.m_Flags & (GeometryFlags.PhysicalGeometry | GeometryFlags.ProtectedArea)) == 0 && val != areaItem2.m_Area && m_TopLevelEntity != m_AreaEntity && (m_EditorMode || m_IgnoreCollisions || !m_Essential))
			{
				return;
			}
			DynamicBuffer<Node> nodes = m_Data.m_AreaNodes[areaItem2.m_Area];
			DynamicBuffer<Triangle> val3 = m_Data.m_AreaTriangles[areaItem2.m_Area];
			Triangle2 val4 = AreaUtils.GetTriangle2(isCounterClockwise: (area.m_Flags & AreaFlags.CounterClockwise) != 0, nodes: nodes, triangle: val3[areaItem2.m_Triangle], expandAmount: -0.1f);
			float2 val5 = default(float2);
			if (!((!((float2)(ref m_Triangle.c)).Equals(m_Triangle.b)) ? MathUtils.Intersect(m_Triangle, val4) : MathUtils.Intersect(val4, ((Triangle2)(ref m_Triangle)).ab, ref val5)))
			{
				return;
			}
			ErrorData errorData = default(ErrorData);
			errorData.m_ErrorSeverity = m_ErrorSeverity;
			errorData.m_ErrorType = ((areaGeometryData.m_Type != AreaType.MapTile || m_EditorMode) ? ErrorType.OverlapExisting : ErrorType.ExceedsCityLimits);
			errorData.m_TempEntity = m_AreaEntity;
			errorData.m_PermanentEntity = areaItem2.m_Area;
			errorData.m_Position = MathUtils.Center(bounds.m_Bounds & m_TriangleBounds);
			errorData.m_Position.y = MathUtils.Clamp(errorData.m_Position.y, ((Bounds3)(ref m_TriangleBounds)).y);
			if (errorData.m_ErrorType == ErrorType.OverlapExisting)
			{
				if (val != areaItem2.m_Area && val != Entity.Null)
				{
					PrefabRef prefabRef2 = m_Data.m_PrefabRef[val];
					if (m_Data.m_PrefabObjectGeometry.HasComponent(prefabRef2.m_Prefab) && (m_Data.m_PrefabObjectGeometry[prefabRef2.m_Prefab].m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden)) == (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden) && !m_Data.m_Attached.HasComponent(val) && (!m_Data.m_Temp.HasComponent(val) || (m_Data.m_Temp[val].m_Flags & TempFlags.Essential) == 0))
					{
						errorData.m_ErrorSeverity = ErrorSeverity.Warning;
						errorData.m_PermanentEntity = val;
					}
				}
				if (!m_Essential && m_TopLevelEntity != m_AreaEntity && m_TopLevelEntity != Entity.Null)
				{
					PrefabRef prefabRef3 = m_Data.m_PrefabRef[m_TopLevelEntity];
					if (m_Data.m_PrefabObjectGeometry.HasComponent(prefabRef3.m_Prefab) && (m_Data.m_PrefabObjectGeometry[prefabRef3.m_Prefab].m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden)) == (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden) && !m_Data.m_Attached.HasComponent(m_TopLevelEntity) && (!m_Data.m_Temp.HasComponent(m_TopLevelEntity) || (m_Data.m_Temp[m_TopLevelEntity].m_Flags & TempFlags.Essential) == 0))
					{
						errorData.m_ErrorSeverity = ErrorSeverity.Warning;
						errorData.m_TempEntity = areaItem2.m_Area;
						errorData.m_PermanentEntity = m_TopLevelEntity;
					}
				}
			}
			m_ErrorQueue.Enqueue(errorData);
		}
	}

	public struct BrushAreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
	{
		public Entity m_BrushEntity;

		public Brush m_Brush;

		public Bounds3 m_BrushBounds;

		public ValidationSystem.EntityData m_Data;

		public ParallelWriter<ErrorData> m_ErrorQueue;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_BrushBounds)).xz);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem2)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_BrushBounds)).xz) || m_Data.m_Hidden.HasComponent(areaItem2.m_Area) || (m_Data.m_Area[areaItem2.m_Area].m_Flags & AreaFlags.Slave) != 0)
			{
				return;
			}
			PrefabRef prefabRef = m_Data.m_PrefabRef[areaItem2.m_Area];
			AreaGeometryData areaGeometryData = m_Data.m_PrefabAreaGeometry[prefabRef.m_Prefab];
			if ((areaGeometryData.m_Flags & GeometryFlags.ProtectedArea) == 0 || !m_Data.m_Native.HasComponent(areaItem2.m_Area))
			{
				return;
			}
			DynamicBuffer<Node> nodes = m_Data.m_AreaNodes[areaItem2.m_Area];
			Triangle triangle = m_Data.m_AreaTriangles[areaItem2.m_Area][areaItem2.m_Triangle];
			Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
			ErrorData errorData = default(ErrorData);
			if (areaGeometryData.m_Type == AreaType.MapTile)
			{
				Circle2 val = default(Circle2);
				((Circle2)(ref val))._002Ector(m_Brush.m_Size * 0.4f, ((float3)(ref m_Brush.m_Position)).xz);
				if (MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, val))
				{
					errorData.m_Position = MathUtils.Center(m_BrushBounds & bounds.m_Bounds);
					errorData.m_ErrorType = ErrorType.ExceedsCityLimits;
				}
			}
			if (errorData.m_ErrorType != ErrorType.None)
			{
				errorData.m_Position.y = MathUtils.Clamp(errorData.m_Position.y, ((Bounds3)(ref m_BrushBounds)).y);
				errorData.m_ErrorSeverity = ErrorSeverity.Error;
				errorData.m_TempEntity = m_BrushEntity;
				errorData.m_PermanentEntity = areaItem2.m_Area;
				m_ErrorQueue.Enqueue(errorData);
			}
		}
	}

	public static void ValidateArea(bool editorMode, Entity entity, Temp temp, Owner owner, Area area, Geometry geometry, Storage storage, DynamicBuffer<Node> nodes, PrefabRef prefabRef, ValidationSystem.EntityData data, NativeQuadTree<Entity, QuadTreeBoundsXZ> objectSearchTree, NativeQuadTree<Entity, QuadTreeBoundsXZ> netSearchTree, NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> areaSearchTree, WaterSurfaceData waterSurfaceData, TerrainHeightData terrainHeightData, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_0824: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		if ((area.m_Flags & AreaFlags.Slave) != 0)
		{
			return;
		}
		float minNodeDistance = AreaUtils.GetMinNodeDistance(data.m_PrefabAreaGeometry[prefabRef.m_Prefab]);
		bool flag = true;
		bool flag2 = (area.m_Flags & AreaFlags.Complete) != 0;
		bool isCounterClockwise = (area.m_Flags & AreaFlags.CounterClockwise) != 0;
		Node node;
		if (nodes.Length == 2)
		{
			ValidateTriangle(editorMode, noErrors: false, isCounterClockwise, entity, temp, owner, new Triangle(0, 1, 1), data, objectSearchTree, netSearchTree, areaSearchTree, waterSurfaceData, terrainHeightData, errorQueue);
		}
		else if (nodes.Length == 3)
		{
			if ((temp.m_Flags & TempFlags.Delete) == 0)
			{
				Segment line = default(Segment);
				((Segment)(ref line))._002Ector(nodes[0].m_Position, nodes[1].m_Position);
				Segment line2 = default(Segment);
				((Segment)(ref line2))._002Ector(nodes[1].m_Position, nodes[2].m_Position);
				flag &= CheckShape(line, nodes[2].m_Position, entity, minNodeDistance, errorQueue);
				flag &= CheckShape(line2, nodes[0].m_Position, entity, minNodeDistance, errorQueue);
				if (flag2)
				{
					Segment line3 = default(Segment);
					((Segment)(ref line3))._002Ector(nodes[2].m_Position, nodes[0].m_Position);
					flag &= CheckShape(line3, nodes[1].m_Position, entity, minNodeDistance, errorQueue);
				}
			}
		}
		else if (nodes.Length > 3 && (temp.m_Flags & TempFlags.Delete) == 0)
		{
			int num = 0;
			int num2 = math.select(nodes.Length - 1, nodes.Length, flag2);
			NativeArray<Bounds2> val = default(NativeArray<Bounds2>);
			int num3 = num2 - num - 2;
			int num4 = 0;
			float2 val2 = default(float2);
			if (num3 > 10)
			{
				int num5 = -1;
				int num6 = 0;
				while (num3 >= 2)
				{
					num5 += 1 << num6++;
					num3 >>= 1;
				}
				val._002Ector(num5, (Allocator)2, (NativeArrayOptions)1);
				num4 = --num6;
				int num7 = 1 << num6;
				int num8 = num5 - num7;
				num3 = num2 - num - 2;
				Bounds2 val3 = default(Bounds2);
				for (int i = 0; i < num7; i++)
				{
					int num9 = i * num3 >> num6;
					int num10 = (i + 1) * num3 >> num6;
					node = nodes[num9++];
					val2 = (val3.min = (val3.max = ((float3)(ref node.m_Position)).xz));
					for (int j = num9; j <= num10; j++)
					{
						Bounds2 val4 = val3;
						node = nodes[j];
						val3 = val4 | ((float3)(ref node.m_Position)).xz;
					}
					val[num8 + i] = MathUtils.Expand(val3, float2.op_Implicit(minNodeDistance));
				}
				while (--num6 > 0)
				{
					int num11 = num8;
					num7 = 1 << num6;
					num8 -= num7;
					for (int k = 0; k < num7; k++)
					{
						val[num8 + k] = val[num11 + (k << 1)] | val[num11 + (k << 1) + 1];
					}
				}
			}
			Segment val5 = new Segment
			{
				a = nodes[num++].m_Position
			};
			for (int l = num; l <= num2; l++)
			{
				int num12 = math.select(l, 0, l == nodes.Length);
				val5.b = nodes[num12].m_Position;
				int num13 = math.select(0, 1, l == nodes.Length);
				int num14 = l - 2;
				if (val.IsCreated)
				{
					int num15 = 0;
					int num16 = 1;
					int num17 = 0;
					while (num16 > 0)
					{
						if (MathUtils.Intersect(val[num15 + num17], ((Segment)(ref val5)).xz, ref val2))
						{
							if (num16 != num4)
							{
								num17 <<= 1;
								num15 += 1 << num16++;
								continue;
							}
							int num18 = math.max(num13, num17 * num3 >> num16);
							int num19 = math.min(num14, (num17 + 1) * num3 >> num16);
							if (num19 > num18)
							{
								Segment val6 = new Segment
								{
									a = nodes[num18++].m_Position
								};
								for (int m = num18; m <= num19; m++)
								{
									val6.b = nodes[m].m_Position;
									flag &= CheckShape(val5, val6, entity, minNodeDistance, errorQueue, nodes, num12, m, flag2, isCounterClockwise);
									val6.a = val6.b;
								}
							}
						}
						while ((num17 & 1) != 0)
						{
							num17 >>= 1;
							num15 -= 1 << --num16;
						}
						num17++;
					}
				}
				else
				{
					Segment val7 = new Segment
					{
						a = nodes[num13++].m_Position
					};
					for (int n = num13; n <= num14; n++)
					{
						val7.b = nodes[n].m_Position;
						flag &= CheckShape(val5, val7, entity, minNodeDistance, errorQueue, nodes, num12, n, flag2, isCounterClockwise);
						val7.a = val7.b;
					}
				}
				if (l > num || flag2)
				{
					int num20 = l - 2;
					num20 += math.select(0, nodes.Length, num20 < 0);
					flag &= CheckShape(val5, nodes[num20].m_Position, entity, minNodeDistance, errorQueue);
				}
				if (l < num2 || flag2)
				{
					int num21 = l + 1;
					num21 -= math.select(0, nodes.Length, num21 >= nodes.Length);
					flag &= CheckShape(val5, nodes[num21].m_Position, entity, minNodeDistance, errorQueue);
				}
				if (!flag2)
				{
					if (l > num)
					{
						flag &= CheckShape(val5, nodes[0].m_Position, entity, minNodeDistance, errorQueue, nodes, num12, 0, flag2, isCounterClockwise);
					}
					if (l < num2)
					{
						flag &= CheckShape(val5, nodes[nodes.Length - 1].m_Position, entity, minNodeDistance, errorQueue, nodes, num12, nodes.Length - 1, flag2, isCounterClockwise);
					}
				}
				val5.a = val5.b;
			}
			if (val.IsCreated)
			{
				val.Dispose();
			}
		}
		if (!flag2 && nodes.Length >= 3)
		{
			ValidateTriangle(editorMode, noErrors: false, isCounterClockwise, entity, temp, owner, new Triangle(nodes.Length - 2, nodes.Length - 1, nodes.Length - 1), data, objectSearchTree, netSearchTree, areaSearchTree, waterSurfaceData, terrainHeightData, errorQueue);
		}
		if (flag && flag2 && (area.m_Flags & AreaFlags.NoTriangles) != 0 && nodes.Length >= 3)
		{
			float3 val8 = float3.op_Implicit(0);
			for (int num22 = 0; num22 < nodes.Length; num22++)
			{
				val8 += nodes[num22].m_Position;
			}
			val8 /= (float)nodes.Length;
			errorQueue.Enqueue(new ErrorData
			{
				m_ErrorSeverity = ErrorSeverity.Error,
				m_ErrorType = ErrorType.InvalidShape,
				m_TempEntity = entity,
				m_Position = val8
			});
			flag = false;
		}
		if ((temp.m_Flags & TempFlags.Delete) == 0 && data.m_Transform.HasComponent(owner.m_Owner) && data.m_PrefabLotData.HasComponent(prefabRef.m_Prefab))
		{
			Transform transform = data.m_Transform[owner.m_Owner];
			float2 xz = ((float3)(ref transform.m_Position)).xz;
			float maxRadius = data.m_PrefabLotData[prefabRef.m_Prefab].m_MaxRadius;
			if (maxRadius > 0f)
			{
				for (int num23 = 0; num23 < nodes.Length; num23++)
				{
					node = nodes[num23];
					if (math.distance(xz, ((float3)(ref node.m_Position)).xz) > maxRadius)
					{
						errorQueue.Enqueue(new ErrorData
						{
							m_ErrorSeverity = ErrorSeverity.Error,
							m_ErrorType = ErrorType.LongDistance,
							m_TempEntity = entity,
							m_PermanentEntity = owner.m_Owner,
							m_Position = nodes[num23].m_Position
						});
					}
				}
			}
		}
		if ((temp.m_Flags & TempFlags.Delete) == 0 && flag && flag2 && data.m_PrefabStorageArea.HasComponent(prefabRef.m_Prefab))
		{
			StorageAreaData prefabStorageData = data.m_PrefabStorageArea[prefabRef.m_Prefab];
			int num24 = AreaUtils.CalculateStorageCapacity(geometry, prefabStorageData);
			if (storage.m_Amount > num24)
			{
				errorQueue.Enqueue(new ErrorData
				{
					m_ErrorSeverity = ErrorSeverity.Error,
					m_ErrorType = ErrorType.SmallArea,
					m_TempEntity = entity,
					m_Position = geometry.m_CenterPosition
				});
			}
		}
	}

	private static bool CheckShape(Segment line1, float3 node2, Entity entity, float minNodeDistance, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		float num = default(float);
		if (MathUtils.Distance(((Segment)(ref line1)).xz, ((float3)(ref node2)).xz, ref num) < minNodeDistance)
		{
			errorQueue.Enqueue(new ErrorData
			{
				m_ErrorSeverity = ErrorSeverity.Error,
				m_ErrorType = ErrorType.InvalidShape,
				m_TempEntity = entity,
				m_Position = math.lerp(MathUtils.Position(line1, num), node2, 0.5f)
			});
			return false;
		}
		return true;
	}

	private static bool CheckShape(Segment line1, float3 node2, Entity entity, float minNodeDistance, ParallelWriter<ErrorData> errorQueue, DynamicBuffer<Node> nodes, int index1, int index2, bool isComplete, bool isCounterClockwise)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		float num = default(float);
		if (MathUtils.Distance(((Segment)(ref line1)).xz, ((float3)(ref node2)).xz, ref num) < minNodeDistance)
		{
			Quad2 edgeQuad = GetEdgeQuad(minNodeDistance, nodes, index1, isComplete, isCounterClockwise);
			Segment edgeLine = GetEdgeLine(minNodeDistance, nodes, index2, isComplete, isCounterClockwise);
			float2 val = default(float2);
			if (MathUtils.Intersect(edgeQuad, ((float3)(ref node2)).xz) || MathUtils.Intersect(edgeLine, ((Segment)(ref line1)).xz, ref val))
			{
				errorQueue.Enqueue(new ErrorData
				{
					m_ErrorSeverity = ErrorSeverity.Error,
					m_ErrorType = ErrorType.InvalidShape,
					m_TempEntity = entity,
					m_Position = math.lerp(MathUtils.Position(line1, num), node2, 0.5f)
				});
				return false;
			}
		}
		return true;
	}

	private static bool CheckShape(Segment line1, Segment line2, Entity entity, float minNodeDistance, ParallelWriter<ErrorData> errorQueue, DynamicBuffer<Node> nodes, int index1, int index2, bool isComplete, bool isCounterClockwise)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		float2 val = default(float2);
		if (MathUtils.Distance(((Segment)(ref line1)).xz, ((Segment)(ref line2)).xz, ref val) < minNodeDistance)
		{
			Quad2 edgeQuad = GetEdgeQuad(minNodeDistance, nodes, index1, isComplete, isCounterClockwise);
			Quad2 edgeQuad2 = GetEdgeQuad(minNodeDistance, nodes, index2, isComplete, isCounterClockwise);
			float2 val2 = default(float2);
			if (MathUtils.Intersect(edgeQuad, ((Segment)(ref line2)).xz, ref val2) || MathUtils.Intersect(edgeQuad2, ((Segment)(ref line1)).xz, ref val2))
			{
				errorQueue.Enqueue(new ErrorData
				{
					m_ErrorSeverity = ErrorSeverity.Error,
					m_ErrorType = ErrorType.InvalidShape,
					m_TempEntity = entity,
					m_Position = math.lerp(MathUtils.Position(line1, val.x), MathUtils.Position(line2, val.y), 0.5f)
				});
				return false;
			}
		}
		return true;
	}

	private static Segment GetEdgeLine(float minNodeDistance, DynamicBuffer<Node> nodes, int index, bool isComplete, bool isCounterClockwise)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		float3 expandedNode = AreaUtils.GetExpandedNode(nodes, index, -0.1f, isComplete, isCounterClockwise);
		Segment result = default(Segment);
		result.a = ((float3)(ref expandedNode)).xz;
		expandedNode = AreaUtils.GetExpandedNode(nodes, index, 0f - minNodeDistance, isComplete, isCounterClockwise);
		result.b = ((float3)(ref expandedNode)).xz;
		return result;
	}

	private static Quad2 GetEdgeQuad(float minNodeDistance, DynamicBuffer<Node> nodes, int index, bool isComplete, bool isCounterClockwise)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		int index2 = math.select(index - 1, index + nodes.Length - 1, index == 0);
		float3 expandedNode = AreaUtils.GetExpandedNode(nodes, index2, 0f - minNodeDistance, isComplete, isCounterClockwise);
		Quad2 result = default(Quad2);
		result.a = ((float3)(ref expandedNode)).xz;
		expandedNode = AreaUtils.GetExpandedNode(nodes, index2, -0.1f, isComplete, isCounterClockwise);
		result.b = ((float3)(ref expandedNode)).xz;
		expandedNode = AreaUtils.GetExpandedNode(nodes, index, -0.1f, isComplete, isCounterClockwise);
		result.c = ((float3)(ref expandedNode)).xz;
		expandedNode = AreaUtils.GetExpandedNode(nodes, index, 0f - minNodeDistance, isComplete, isCounterClockwise);
		result.d = ((float3)(ref expandedNode)).xz;
		return result;
	}

	public static void ValidateTriangle(bool editorMode, bool noErrors, bool isCounterClockwise, Entity entity, Temp temp, Owner owner, Triangle triangle, ValidationSystem.EntityData data, NativeQuadTree<Entity, QuadTreeBoundsXZ> objectSearchTree, NativeQuadTree<Entity, QuadTreeBoundsXZ> netSearchTree, NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> areaSearchTree, WaterSurfaceData waterSurfaceData, TerrainHeightData terrainHeightData, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Node> nodes = data.m_AreaNodes[entity];
		PrefabRef prefabRef = data.m_PrefabRef[entity];
		AreaGeometryData areaGeometryData = data.m_PrefabAreaGeometry[prefabRef.m_Prefab];
		Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
		Bounds3 bounds = AreaUtils.GetBounds(triangle, triangle2, areaGeometryData);
		Bounds1 heightRange = triangle.m_HeightRange;
		heightRange.max += areaGeometryData.m_MaxHeight;
		Entity val = entity;
		if (owner.m_Owner != Entity.Null && !data.m_AssetStamp.HasComponent(owner.m_Owner))
		{
			val = owner.m_Owner;
			while (data.m_Owner.HasComponent(val) && !data.m_Building.HasComponent(val))
			{
				Entity owner2 = data.m_Owner[val].m_Owner;
				if (data.m_AssetStamp.HasComponent(owner2))
				{
					break;
				}
				val = owner2;
			}
		}
		Entity val2 = Entity.Null;
		Entity ignoreEntity = Entity.Null;
		ErrorSeverity errorSeverity = ErrorSeverity.Error;
		if (noErrors)
		{
			val2 = val;
			while (data.m_Owner.HasComponent(val2))
			{
				val2 = data.m_Owner[val2].m_Owner;
			}
			Attachment attachment = default(Attachment);
			if (data.m_Attachment.TryGetComponent(val2, ref attachment))
			{
				ignoreEntity = attachment.m_Attached;
			}
			errorSeverity = ErrorSeverity.Warning;
		}
		AreaUtils.SetCollisionFlags(ref areaGeometryData, !editorMode || owner.m_Owner != Entity.Null);
		if ((areaGeometryData.m_Flags & GeometryFlags.PhysicalGeometry) != 0)
		{
			CollisionMask collisionMask = AreaUtils.GetCollisionMask(areaGeometryData);
			if ((temp.m_Flags & TempFlags.Delete) == 0)
			{
				ObjectIterator objectIterator = new ObjectIterator
				{
					m_AreaEntity = entity,
					m_OriginalAreaEntity = temp.m_Original,
					m_IgnoreEntity = val2,
					m_IgnoreEntity2 = ignoreEntity,
					m_TriangleBounds = bounds,
					m_HeightRange = heightRange,
					m_Triangle = triangle2,
					m_ErrorSeverity = errorSeverity,
					m_CollisionMask = collisionMask,
					m_PrefabAreaData = areaGeometryData,
					m_Data = data,
					m_ErrorQueue = errorQueue,
					m_EditorMode = editorMode
				};
				objectSearchTree.Iterate<ObjectIterator>(ref objectIterator, 0);
			}
			if ((temp.m_Flags & TempFlags.Delete) == 0)
			{
				NetIterator netIterator = new NetIterator
				{
					m_AreaEntity = entity,
					m_OriginalAreaEntity = temp.m_Original,
					m_IgnoreEntity = val2,
					m_IgnoreEntity2 = ignoreEntity,
					m_TriangleBounds = bounds,
					m_HeightRange = heightRange,
					m_Triangle = triangle2,
					m_ErrorSeverity = errorSeverity,
					m_CollisionMask = collisionMask,
					m_Data = data,
					m_ErrorQueue = errorQueue,
					m_EditorMode = editorMode
				};
				netSearchTree.Iterate<NetIterator>(ref netIterator, 0);
			}
		}
		if ((areaGeometryData.m_Flags & (GeometryFlags.PhysicalGeometry | GeometryFlags.ProtectedArea)) != 0 || val == entity || (!editorMode && ((temp.m_Flags & (TempFlags.Delete | TempFlags.Essential)) == TempFlags.Essential || (temp.m_Flags & TempFlags.Create) != 0)))
		{
			AreaIterator areaIterator = new AreaIterator
			{
				m_AreaEntity = entity,
				m_IgnoreEntity = val2,
				m_IgnoreEntity2 = ignoreEntity,
				m_TopLevelEntity = val,
				m_TriangleBounds = bounds,
				m_IgnoreCollisions = ((temp.m_Flags & TempFlags.Delete) != 0),
				m_EditorMode = editorMode,
				m_Essential = ((temp.m_Flags & TempFlags.Essential) != 0),
				m_PrefabAreaData = areaGeometryData,
				m_ErrorSeverity = errorSeverity,
				m_Data = data,
				m_ErrorQueue = errorQueue
			};
			if (triangle.m_Indices.y == triangle.m_Indices.z)
			{
				areaIterator.m_Triangle = AreaUtils.GetTriangle2(nodes, triangle);
			}
			else
			{
				areaIterator.m_Triangle = AreaUtils.GetTriangle2(nodes, triangle, -0.1f, isCounterClockwise);
			}
			areaSearchTree.Iterate<AreaIterator>(ref areaIterator, 0);
		}
		if ((areaGeometryData.m_Flags & GeometryFlags.PhysicalGeometry) == 0 || (areaGeometryData.m_Flags & (GeometryFlags.OnWaterSurface | GeometryFlags.RequireWater)) == GeometryFlags.OnWaterSurface)
		{
			return;
		}
		float sampleInterval = WaterUtils.GetSampleInterval(ref waterSurfaceData);
		int2 val3 = (int2)math.ceil(new float2(math.distance(((float3)(ref triangle2.a)).xz, ((float3)(ref triangle2.b)).xz), math.distance(((float3)(ref triangle2.a)).xz, ((float3)(ref triangle2.c)).xz)) / sampleInterval);
		float num = 1f / (float)math.max(1, val3.x);
		float num2 = areaGeometryData.m_SnapDistance * 0.01f;
		Bounds3 val4 = default(Bounds3);
		val4.min = float3.op_Implicit(float.MaxValue);
		val4.max = float3.op_Implicit(float.MinValue);
		bool flag = false;
		bool flag2 = false;
		OriginalAreaIterator originalAreaIterator = default(OriginalAreaIterator);
		DynamicBuffer<Node> nodes2 = default(DynamicBuffer<Node>);
		if (data.m_AreaNodes.TryGetBuffer(temp.m_Original, ref nodes2))
		{
			originalAreaIterator = new OriginalAreaIterator
			{
				m_AreaEntity = temp.m_Original,
				m_Offset = num2,
				m_Nodes = nodes2,
				m_Triangles = data.m_AreaTriangles[temp.m_Original]
			};
		}
		for (int i = 0; i <= val3.x; i++)
		{
			float2 val5 = new float2
			{
				x = (float)i * num
			};
			int num3 = ((val3.x - i) * val3.y + (val3.x >> 1)) / math.max(1, val3.x);
			float num4 = (1f - val5.x) / (float)math.max(1, num3);
			if ((areaGeometryData.m_Flags & GeometryFlags.RequireWater) != 0)
			{
				for (int j = 0; j <= num3; j++)
				{
					val5.y = (float)j * num4;
					float3 val6 = MathUtils.Position(triangle2, val5);
					if (!(WaterUtils.SampleDepth(ref waterSurfaceData, val6) < 0.2f))
					{
						continue;
					}
					if (originalAreaIterator.m_Nodes.IsCreated)
					{
						originalAreaIterator.m_Bounds = new Bounds2(((float3)(ref val6)).xz - num2, ((float3)(ref val6)).xz + num2);
						originalAreaIterator.m_Position = ((float3)(ref val6)).xz;
						originalAreaIterator.m_Result = false;
						areaSearchTree.Iterate<OriginalAreaIterator>(ref originalAreaIterator, 0);
						if (originalAreaIterator.m_Result)
						{
							continue;
						}
					}
					val4 |= val6;
					flag2 = true;
				}
				continue;
			}
			for (int k = 0; k <= num3; k++)
			{
				val5.y = (float)k * num4;
				float3 val7 = MathUtils.Position(triangle2, val5);
				if (!(WaterUtils.SampleDepth(ref waterSurfaceData, val7) >= 0.2f))
				{
					continue;
				}
				if (originalAreaIterator.m_Nodes.IsCreated)
				{
					originalAreaIterator.m_Bounds = new Bounds2(((float3)(ref val7)).xz - num2, ((float3)(ref val7)).xz + num2);
					originalAreaIterator.m_Position = ((float3)(ref val7)).xz;
					originalAreaIterator.m_Result = false;
					areaSearchTree.Iterate<OriginalAreaIterator>(ref originalAreaIterator, 0);
					if (originalAreaIterator.m_Result)
					{
						continue;
					}
				}
				val4 |= val7;
				flag = true;
			}
		}
		if (flag)
		{
			errorQueue.Enqueue(new ErrorData
			{
				m_ErrorType = ErrorType.InWater,
				m_ErrorSeverity = ErrorSeverity.Error,
				m_TempEntity = entity,
				m_Position = MathUtils.Center(val4)
			});
		}
		if (flag2)
		{
			errorQueue.Enqueue(new ErrorData
			{
				m_ErrorType = ErrorType.NoWater,
				m_ErrorSeverity = ErrorSeverity.Error,
				m_TempEntity = entity,
				m_Position = MathUtils.Center(val4)
			});
		}
	}
}

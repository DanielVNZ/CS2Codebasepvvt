using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Objects;

public static class ValidationHelpers
{
	private struct ObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Entity m_ObjectEntity;

		public Entity m_TopLevelEntity;

		public Entity m_AssetStampEntity;

		public Bounds3 m_ObjectBounds;

		public Transform m_Transform;

		public Stack m_ObjectStack;

		public CollisionMask m_CollisionMask;

		public ObjectGeometryData m_PrefabObjectGeometryData;

		public StackData m_ObjectStackData;

		public bool m_CanOverride;

		public bool m_Optional;

		public bool m_EditorMode;

		public ValidationSystem.EntityData m_Data;

		public ParallelWriter<ErrorData> m_ErrorQueue;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if ((bounds.m_Mask & BoundsMask.NotOverridden) == 0)
			{
				return false;
			}
			if ((m_CollisionMask & CollisionMask.OnGround) != 0)
			{
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_ObjectBounds)).xz);
			}
			return MathUtils.Intersect(bounds.m_Bounds, m_ObjectBounds);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity objectEntity2)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			if ((bounds.m_Mask & BoundsMask.NotOverridden) == 0)
			{
				return;
			}
			if ((m_CollisionMask & CollisionMask.OnGround) != 0)
			{
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_ObjectBounds)).xz))
				{
					return;
				}
			}
			else if (!MathUtils.Intersect(bounds.m_Bounds, m_ObjectBounds))
			{
				return;
			}
			if (m_Data.m_Hidden.HasComponent(objectEntity2) || objectEntity2 == m_AssetStampEntity)
			{
				return;
			}
			Entity val = objectEntity2;
			bool hasOwner = false;
			Owner owner = default(Owner);
			while (m_Data.m_Owner.TryGetComponent(val, ref owner) && !m_Data.m_Building.HasComponent(val))
			{
				Entity owner2 = owner.m_Owner;
				hasOwner = true;
				if (m_Data.m_AssetStamp.HasComponent(owner2))
				{
					if (!(owner2 == m_ObjectEntity))
					{
						break;
					}
					return;
				}
				val = owner2;
			}
			if (!(m_TopLevelEntity == val))
			{
				CheckOverlap(val, objectEntity2, bounds.m_Bounds, essential: false, hasOwner);
			}
		}

		public void CheckOverlap(Entity topLevelEntity2, Entity objectEntity2, Bounds3 bounds2, bool essential, bool hasOwner)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_Data.m_PrefabRef[objectEntity2];
			Transform transform = m_Data.m_Transform[objectEntity2];
			if (!m_Data.m_PrefabObjectGeometry.HasComponent(prefabRef.m_Prefab))
			{
				return;
			}
			ObjectGeometryData objectGeometryData = m_Data.m_PrefabObjectGeometry[prefabRef.m_Prefab];
			if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreSecondaryCollision) != GeometryFlags.None && m_Data.m_Secondary.HasComponent(objectEntity2))
			{
				return;
			}
			Elevation elevation = default(Elevation);
			CollisionMask collisionMask = ((!m_Data.m_ObjectElevation.TryGetComponent(objectEntity2, ref elevation)) ? ObjectUtils.GetCollisionMask(objectGeometryData, !m_EditorMode || hasOwner) : ObjectUtils.GetCollisionMask(objectGeometryData, elevation, !m_EditorMode || hasOwner));
			if ((m_CollisionMask & collisionMask) == 0)
			{
				return;
			}
			ErrorData error = new ErrorData
			{
				m_ErrorSeverity = ErrorSeverity.Error,
				m_TempEntity = m_ObjectEntity,
				m_PermanentEntity = objectEntity2
			};
			if (m_CanOverride)
			{
				error.m_ErrorSeverity = ErrorSeverity.Override;
				error.m_PermanentEntity = Entity.Null;
			}
			else if (!essential)
			{
				if (topLevelEntity2 != objectEntity2)
				{
					if (topLevelEntity2 != Entity.Null)
					{
						if ((objectGeometryData.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) == GeometryFlags.Overridable)
						{
							if ((m_PrefabObjectGeometryData.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) == GeometryFlags.Overridable)
							{
								if (m_Optional)
								{
									error.m_ErrorSeverity = ErrorSeverity.Warning;
								}
							}
							else
							{
								error.m_ErrorSeverity = ErrorSeverity.Override;
							}
						}
						else
						{
							PrefabRef prefabRef2 = m_Data.m_PrefabRef[topLevelEntity2];
							if (m_Data.m_PrefabObjectGeometry.HasComponent(prefabRef2.m_Prefab) && (m_Data.m_PrefabObjectGeometry[prefabRef2.m_Prefab].m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) == (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden) && !m_Data.m_Attached.HasComponent(topLevelEntity2) && (!m_Data.m_Temp.HasComponent(topLevelEntity2) || (m_Data.m_Temp[topLevelEntity2].m_Flags & TempFlags.Essential) == 0) && (m_Optional || (m_PrefabObjectGeometryData.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) != GeometryFlags.Overridable))
							{
								error.m_ErrorSeverity = ErrorSeverity.Warning;
								error.m_PermanentEntity = topLevelEntity2;
							}
						}
					}
				}
				else if ((objectGeometryData.m_Flags & GeometryFlags.Overridable) != GeometryFlags.None)
				{
					if ((objectGeometryData.m_Flags & GeometryFlags.DeleteOverridden) != GeometryFlags.None)
					{
						if (!m_Data.m_Attached.HasComponent(objectEntity2) && (m_Optional || (m_PrefabObjectGeometryData.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) != GeometryFlags.Overridable))
						{
							error.m_ErrorSeverity = ErrorSeverity.Warning;
						}
					}
					else if ((m_PrefabObjectGeometryData.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) == GeometryFlags.Overridable)
					{
						if (m_Optional)
						{
							error.m_ErrorSeverity = ErrorSeverity.Warning;
						}
					}
					else
					{
						error.m_ErrorSeverity = ErrorSeverity.Override;
					}
				}
			}
			float3 origin = MathUtils.Center(bounds2);
			StackData stackData = default(StackData);
			Stack stack = default(Stack);
			if (m_Data.m_Stack.TryGetComponent(objectEntity2, ref stack))
			{
				m_Data.m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData);
			}
			if ((m_CollisionMask & CollisionMask.OnGround) == 0 || MathUtils.Intersect(bounds2, m_ObjectBounds))
			{
				CheckOverlap3D(ref error, transform, stack, objectGeometryData, stackData, origin);
			}
			if (error.m_ErrorType == ErrorType.None && CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask))
			{
				CheckOverlap2D(ref error, transform, objectGeometryData, bounds2, origin);
			}
			if (error.m_ErrorType != ErrorType.None)
			{
				if ((error.m_ErrorSeverity == ErrorSeverity.Override || error.m_ErrorSeverity == ErrorSeverity.Warning) && error.m_ErrorType == ErrorType.OverlapExisting && m_Data.m_OnFire.HasComponent(error.m_PermanentEntity))
				{
					error.m_ErrorType = ErrorType.OnFire;
					error.m_ErrorSeverity = ErrorSeverity.Error;
				}
				m_ErrorQueue.Enqueue(error);
			}
		}

		private void CheckOverlap3D(ref ErrorData error, Transform transform2, Stack stack2, ObjectGeometryData prefabObjectGeometryData2, StackData stackData2, float3 origin)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aeb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_1354: Unknown result type (might be due to invalid IL or missing references)
			//IL_135c: Unknown result type (might be due to invalid IL or missing references)
			//IL_135e: Unknown result type (might be due to invalid IL or missing references)
			//IL_135f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1364: Unknown result type (might be due to invalid IL or missing references)
			//IL_136b: Unknown result type (might be due to invalid IL or missing references)
			//IL_136d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1377: Unknown result type (might be due to invalid IL or missing references)
			//IL_137c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1381: Unknown result type (might be due to invalid IL or missing references)
			//IL_1389: Unknown result type (might be due to invalid IL or missing references)
			//IL_138e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1393: Unknown result type (might be due to invalid IL or missing references)
			//IL_1395: Unknown result type (might be due to invalid IL or missing references)
			//IL_127a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1284: Unknown result type (might be due to invalid IL or missing references)
			//IL_129c: Unknown result type (might be due to invalid IL or missing references)
			//IL_12a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_12a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_12af: Unknown result type (might be due to invalid IL or missing references)
			//IL_12bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_12c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_12d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_12d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_12dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_12e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_12e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ecb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ecd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ece: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0edc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eeb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0efd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_13a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_13a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_13a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_13be: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_13cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_13cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_13dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_13e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1305: Unknown result type (might be due to invalid IL or missing references)
			//IL_1307: Unknown result type (might be due to invalid IL or missing references)
			//IL_130c: Unknown result type (might be due to invalid IL or missing references)
			//IL_130e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1313: Unknown result type (might be due to invalid IL or missing references)
			//IL_1318: Unknown result type (might be due to invalid IL or missing references)
			//IL_131a: Unknown result type (might be due to invalid IL or missing references)
			//IL_131c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1321: Unknown result type (might be due to invalid IL or missing references)
			//IL_1323: Unknown result type (might be due to invalid IL or missing references)
			//IL_1328: Unknown result type (might be due to invalid IL or missing references)
			//IL_132d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1330: Unknown result type (might be due to invalid IL or missing references)
			//IL_1332: Unknown result type (might be due to invalid IL or missing references)
			//IL_1334: Unknown result type (might be due to invalid IL or missing references)
			//IL_133b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1340: Unknown result type (might be due to invalid IL or missing references)
			//IL_1345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1003: Unknown result type (might be due to invalid IL or missing references)
			//IL_1008: Unknown result type (might be due to invalid IL or missing references)
			//IL_125b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ead: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eaf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0682: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_1023: Unknown result type (might be due to invalid IL or missing references)
			//IL_102d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1045: Unknown result type (might be due to invalid IL or missing references)
			//IL_104a: Unknown result type (might be due to invalid IL or missing references)
			//IL_104f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1056: Unknown result type (might be due to invalid IL or missing references)
			//IL_1058: Unknown result type (might be due to invalid IL or missing references)
			//IL_106a: Unknown result type (might be due to invalid IL or missing references)
			//IL_107a: Unknown result type (might be due to invalid IL or missing references)
			//IL_107f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1086: Unknown result type (might be due to invalid IL or missing references)
			//IL_108b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1093: Unknown result type (might be due to invalid IL or missing references)
			//IL_1098: Unknown result type (might be due to invalid IL or missing references)
			//IL_109d: Unknown result type (might be due to invalid IL or missing references)
			//IL_109f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c64: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1114: Unknown result type (might be due to invalid IL or missing references)
			//IL_1126: Unknown result type (might be due to invalid IL or missing references)
			//IL_1128: Unknown result type (might be due to invalid IL or missing references)
			//IL_1150: Unknown result type (might be due to invalid IL or missing references)
			//IL_115a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1164: Unknown result type (might be due to invalid IL or missing references)
			//IL_117c: Unknown result type (might be due to invalid IL or missing references)
			//IL_11a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_11b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_11cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_11d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_11d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_11de: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_10af: Unknown result type (might be due to invalid IL or missing references)
			//IL_10b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_10b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_10b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_10bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_10cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_10cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_10da: Unknown result type (might be due to invalid IL or missing references)
			//IL_10dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_10de: Unknown result type (might be due to invalid IL or missing references)
			//IL_10e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0934: Unknown result type (might be due to invalid IL or missing references)
			//IL_093e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0956: Unknown result type (might be due to invalid IL or missing references)
			//IL_095b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0960: Unknown result type (might be due to invalid IL or missing references)
			//IL_0967: Unknown result type (might be due to invalid IL or missing references)
			//IL_0969: Unknown result type (might be due to invalid IL or missing references)
			//IL_0979: Unknown result type (might be due to invalid IL or missing references)
			//IL_097b: Unknown result type (might be due to invalid IL or missing references)
			//IL_098b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0990: Unknown result type (might be due to invalid IL or missing references)
			//IL_0996: Unknown result type (might be due to invalid IL or missing references)
			//IL_099b: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_09af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_11f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_11f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_11fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1200: Unknown result type (might be due to invalid IL or missing references)
			//IL_1205: Unknown result type (might be due to invalid IL or missing references)
			//IL_120a: Unknown result type (might be due to invalid IL or missing references)
			//IL_120c: Unknown result type (might be due to invalid IL or missing references)
			//IL_120e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1213: Unknown result type (might be due to invalid IL or missing references)
			//IL_1215: Unknown result type (might be due to invalid IL or missing references)
			//IL_121a: Unknown result type (might be due to invalid IL or missing references)
			//IL_121f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1222: Unknown result type (might be due to invalid IL or missing references)
			//IL_1224: Unknown result type (might be due to invalid IL or missing references)
			//IL_1226: Unknown result type (might be due to invalid IL or missing references)
			//IL_122d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1232: Unknown result type (might be due to invalid IL or missing references)
			//IL_1237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0daf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dcc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09db: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0915: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_0710: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0724: Unknown result type (might be due to invalid IL or missing references)
			//IL_0734: Unknown result type (might be due to invalid IL or missing references)
			//IL_0739: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_074d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_0757: Unknown result type (might be due to invalid IL or missing references)
			//IL_0759: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_080a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0814: Unknown result type (might be due to invalid IL or missing references)
			//IL_081e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0836: Unknown result type (might be due to invalid IL or missing references)
			//IL_085e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0868: Unknown result type (might be due to invalid IL or missing references)
			//IL_0872: Unknown result type (might be due to invalid IL or missing references)
			//IL_0884: Unknown result type (might be due to invalid IL or missing references)
			//IL_0889: Unknown result type (might be due to invalid IL or missing references)
			//IL_088b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0890: Unknown result type (might be due to invalid IL or missing references)
			//IL_0898: Unknown result type (might be due to invalid IL or missing references)
			//IL_089d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_076b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0770: Unknown result type (might be due to invalid IL or missing references)
			//IL_0772: Unknown result type (might be due to invalid IL or missing references)
			//IL_0777: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_077e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0780: Unknown result type (might be due to invalid IL or missing references)
			//IL_0785: Unknown result type (might be due to invalid IL or missing references)
			//IL_0787: Unknown result type (might be due to invalid IL or missing references)
			//IL_078c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_0796: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Unknown result type (might be due to invalid IL or missing references)
			//IL_079f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08de: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
			quaternion val = math.inverse(m_Transform.m_Rotation);
			quaternion val2 = math.inverse(transform2.m_Rotation);
			float3 val3 = math.mul(val, m_Transform.m_Position - origin);
			float3 val4 = math.mul(val2, transform2.m_Position - origin);
			Bounds3 bounds = ObjectUtils.GetBounds(m_ObjectStack, m_PrefabObjectGeometryData, m_ObjectStackData);
			if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
			{
				bounds.min.y = math.max(bounds.min.y, 0f);
			}
			if (ObjectUtils.GetStandingLegCount(m_PrefabObjectGeometryData, out var legCount))
			{
				Bounds3 val9 = default(Bounds3);
				Bounds3 val10 = default(Bounds3);
				Bounds3 val14 = default(Bounds3);
				Bounds3 val15 = default(Bounds3);
				Bounds3 val21 = default(Bounds3);
				Bounds3 val22 = default(Bounds3);
				Bounds3 val26 = default(Bounds3);
				Bounds3 val27 = default(Bounds3);
				Bounds3 val31 = default(Bounds3);
				Bounds3 val32 = default(Bounds3);
				Bounds3 val36 = default(Bounds3);
				Bounds3 val37 = default(Bounds3);
				for (int i = 0; i < legCount; i++)
				{
					float3 val5 = val3 + ObjectUtils.GetStandingLegOffset(m_PrefabObjectGeometryData, i);
					if ((m_PrefabObjectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
					{
						Cylinder3 val6 = new Cylinder3
						{
							circle = new Circle2(m_PrefabObjectGeometryData.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref val5)).xz),
							height = new Bounds1(bounds.min.y + 0.01f, m_PrefabObjectGeometryData.m_LegSize.y + 0.01f) + val5.y,
							rotation = m_Transform.m_Rotation
						};
						Bounds3 bounds2 = ObjectUtils.GetBounds(stack2, prefabObjectGeometryData2, stackData2);
						if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
						{
							bounds2.min.y = math.max(bounds2.min.y, 0f);
						}
						if (ObjectUtils.GetStandingLegCount(prefabObjectGeometryData2, out var legCount2))
						{
							for (int j = 0; j < legCount2; j++)
							{
								float3 val7 = val4 + ObjectUtils.GetStandingLegOffset(prefabObjectGeometryData2, j);
								if ((prefabObjectGeometryData2.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
								{
									if (ValidationHelpers.Intersect(cylinder2: new Cylinder3
									{
										circle = new Circle2(prefabObjectGeometryData2.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref val7)).xz),
										height = new Bounds1(bounds2.min.y + 0.01f, prefabObjectGeometryData2.m_LegSize.y + 0.01f) + val7.y,
										rotation = transform2.m_Rotation
									}, cylinder1: val6, pos: ref error.m_Position))
									{
										ref float3 position = ref error.m_Position;
										position += origin;
										error.m_ErrorType = ErrorType.OverlapExisting;
									}
								}
								else if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
								{
									Box3 val8 = new Box3
									{
										bounds = 
										{
											min = 
											{
												y = bounds2.min.y + 0.01f
											}
										}
									};
									((float3)(ref val8.bounds.min)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * -0.5f + 0.01f;
									val8.bounds.max.y = prefabObjectGeometryData2.m_LegSize.y + 0.01f;
									((float3)(ref val8.bounds.max)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * 0.5f - 0.01f;
									ref Bounds3 bounds3 = ref val8.bounds;
									bounds3 += val7;
									val8.rotation = transform2.m_Rotation;
									if (MathUtils.Intersect(val6, val8, ref val9, ref val10))
									{
										float3 val11 = math.mul(val6.rotation, MathUtils.Center(val9));
										float3 val12 = math.mul(val8.rotation, MathUtils.Center(val10));
										error.m_Position = origin + math.lerp(val11, val12, 0.5f);
										error.m_ErrorType = ErrorType.OverlapExisting;
									}
								}
							}
							bounds2.min.y = prefabObjectGeometryData2.m_LegSize.y;
						}
						if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
						{
							if (ValidationHelpers.Intersect(cylinder2: new Cylinder3
							{
								circle = new Circle2(prefabObjectGeometryData2.m_Size.x * 0.5f - 0.01f, ((float3)(ref val4)).xz),
								height = new Bounds1(bounds2.min.y + 0.01f, bounds2.max.y - 0.01f) + val4.y,
								rotation = transform2.m_Rotation
							}, cylinder1: val6, pos: ref error.m_Position))
							{
								ref float3 position2 = ref error.m_Position;
								position2 += origin;
								error.m_ErrorType = ErrorType.OverlapExisting;
							}
							continue;
						}
						Box3 val13 = default(Box3);
						val13.bounds = bounds2 + val4;
						val13.bounds = MathUtils.Expand(val13.bounds, float3.op_Implicit(-0.01f));
						val13.rotation = transform2.m_Rotation;
						if (MathUtils.Intersect(val6, val13, ref val14, ref val15))
						{
							float3 val16 = math.mul(val6.rotation, MathUtils.Center(val14));
							float3 val17 = math.mul(val13.rotation, MathUtils.Center(val15));
							error.m_Position = origin + math.lerp(val16, val17, 0.5f);
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
					else
					{
						if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) != GeometryFlags.None)
						{
							continue;
						}
						Box3 val18 = new Box3
						{
							bounds = 
							{
								min = 
								{
									y = bounds.min.y + 0.01f
								}
							}
						};
						((float3)(ref val18.bounds.min)).xz = ((float3)(ref m_PrefabObjectGeometryData.m_LegSize)).xz * -0.5f + 0.01f;
						val18.bounds.max.y = m_PrefabObjectGeometryData.m_LegSize.y + 0.01f;
						((float3)(ref val18.bounds.max)).xz = ((float3)(ref m_PrefabObjectGeometryData.m_LegSize)).xz * 0.5f - 0.01f;
						ref Bounds3 bounds4 = ref val18.bounds;
						bounds4 += val5;
						val18.rotation = m_Transform.m_Rotation;
						Bounds3 bounds5 = ObjectUtils.GetBounds(stack2, prefabObjectGeometryData2, stackData2);
						if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
						{
							bounds5.min.y = math.max(bounds5.min.y, 0f);
						}
						if (ObjectUtils.GetStandingLegCount(prefabObjectGeometryData2, out var legCount3))
						{
							for (int k = 0; k < legCount3; k++)
							{
								float3 val19 = val4 + ObjectUtils.GetStandingLegOffset(prefabObjectGeometryData2, k);
								if ((prefabObjectGeometryData2.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
								{
									Cylinder3 val20 = new Cylinder3
									{
										circle = new Circle2(prefabObjectGeometryData2.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref val19)).xz),
										height = new Bounds1(bounds5.min.y + 0.01f, prefabObjectGeometryData2.m_LegSize.y + 0.01f) + val19.y,
										rotation = transform2.m_Rotation
									};
									if (MathUtils.Intersect(val20, val18, ref val21, ref val22))
									{
										float3 val23 = math.mul(val18.rotation, MathUtils.Center(val22));
										float3 val24 = math.mul(val20.rotation, MathUtils.Center(val21));
										error.m_Position = origin + math.lerp(val23, val24, 0.5f);
										error.m_ErrorType = ErrorType.OverlapExisting;
									}
								}
								else if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
								{
									Box3 val25 = new Box3
									{
										bounds = 
										{
											min = 
											{
												y = bounds5.min.y + 0.01f
											}
										}
									};
									((float3)(ref val25.bounds.min)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * -0.5f + 0.01f;
									val25.bounds.max.y = prefabObjectGeometryData2.m_LegSize.y + 0.01f;
									((float3)(ref val25.bounds.max)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * 0.5f - 0.01f;
									ref Bounds3 bounds6 = ref val25.bounds;
									bounds6 += val19;
									val25.rotation = transform2.m_Rotation;
									if (MathUtils.Intersect(val18, val25, ref val26, ref val27))
									{
										float3 val28 = math.mul(val18.rotation, MathUtils.Center(val26));
										float3 val29 = math.mul(val25.rotation, MathUtils.Center(val27));
										error.m_Position = origin + math.lerp(val28, val29, 0.5f);
										error.m_ErrorType = ErrorType.OverlapExisting;
									}
								}
							}
							bounds5.min.y = prefabObjectGeometryData2.m_LegSize.y;
						}
						if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
						{
							Cylinder3 val30 = new Cylinder3
							{
								circle = new Circle2(prefabObjectGeometryData2.m_Size.x * 0.5f - 0.01f, ((float3)(ref val4)).xz),
								height = new Bounds1(bounds5.min.y + 0.01f, bounds5.max.y - 0.01f) + val4.y,
								rotation = transform2.m_Rotation
							};
							if (MathUtils.Intersect(val30, val18, ref val31, ref val32))
							{
								float3 val33 = math.mul(val18.rotation, MathUtils.Center(val32));
								float3 val34 = math.mul(val30.rotation, MathUtils.Center(val31));
								error.m_Position = origin + math.lerp(val33, val34, 0.5f);
								error.m_ErrorType = ErrorType.OverlapExisting;
							}
						}
						else
						{
							Box3 val35 = default(Box3);
							val35.bounds = bounds5 + val4;
							val35.bounds = MathUtils.Expand(val35.bounds, float3.op_Implicit(-0.01f));
							val35.rotation = transform2.m_Rotation;
							if (MathUtils.Intersect(val18, val35, ref val36, ref val37))
							{
								float3 val38 = math.mul(val18.rotation, MathUtils.Center(val36));
								float3 val39 = math.mul(val35.rotation, MathUtils.Center(val37));
								error.m_Position = origin + math.lerp(val38, val39, 0.5f);
								error.m_ErrorType = ErrorType.OverlapExisting;
							}
						}
					}
				}
				bounds.min.y = m_PrefabObjectGeometryData.m_LegSize.y;
			}
			if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
			{
				Cylinder3 val40 = new Cylinder3
				{
					circle = new Circle2(m_PrefabObjectGeometryData.m_Size.x * 0.5f - 0.01f, ((float3)(ref val3)).xz),
					height = new Bounds1(bounds.min.y + 0.01f, bounds.max.y - 0.01f) + val3.y,
					rotation = m_Transform.m_Rotation
				};
				Bounds3 bounds7 = ObjectUtils.GetBounds(stack2, prefabObjectGeometryData2, stackData2);
				if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
				{
					bounds7.min.y = math.max(bounds7.min.y, 0f);
				}
				if (ObjectUtils.GetStandingLegCount(prefabObjectGeometryData2, out var legCount4))
				{
					Bounds3 val43 = default(Bounds3);
					Bounds3 val44 = default(Bounds3);
					for (int l = 0; l < legCount4; l++)
					{
						float3 val41 = val4 + ObjectUtils.GetStandingLegOffset(prefabObjectGeometryData2, l);
						if ((prefabObjectGeometryData2.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
						{
							if (ValidationHelpers.Intersect(cylinder2: new Cylinder3
							{
								circle = new Circle2(prefabObjectGeometryData2.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref val41)).xz),
								height = new Bounds1(bounds7.min.y + 0.01f, prefabObjectGeometryData2.m_LegSize.y + 0.01f) + val41.y,
								rotation = transform2.m_Rotation
							}, cylinder1: val40, pos: ref error.m_Position))
							{
								ref float3 position3 = ref error.m_Position;
								position3 += origin;
								error.m_ErrorType = ErrorType.OverlapExisting;
							}
						}
						else if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
						{
							Box3 val42 = new Box3
							{
								bounds = 
								{
									min = 
									{
										y = bounds7.min.y + 0.01f
									}
								}
							};
							((float3)(ref val42.bounds.min)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * -0.5f + 0.01f;
							val42.bounds.max.y = prefabObjectGeometryData2.m_LegSize.y + 0.01f;
							((float3)(ref val42.bounds.max)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * 0.5f - 0.01f;
							ref Bounds3 bounds8 = ref val42.bounds;
							bounds8 += val41;
							val42.rotation = transform2.m_Rotation;
							if (MathUtils.Intersect(val40, val42, ref val43, ref val44))
							{
								float3 val45 = math.mul(val40.rotation, MathUtils.Center(val43));
								float3 val46 = math.mul(val42.rotation, MathUtils.Center(val44));
								error.m_Position = origin + math.lerp(val45, val46, 0.5f);
								error.m_ErrorType = ErrorType.OverlapExisting;
							}
						}
					}
					bounds7.min.y = prefabObjectGeometryData2.m_LegSize.y;
				}
				if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
				{
					if (ValidationHelpers.Intersect(cylinder2: new Cylinder3
					{
						circle = new Circle2(prefabObjectGeometryData2.m_Size.x * 0.5f - 0.01f, ((float3)(ref val4)).xz),
						height = new Bounds1(bounds7.min.y + 0.01f, bounds7.max.y - 0.01f) + val4.y,
						rotation = transform2.m_Rotation
					}, cylinder1: val40, pos: ref error.m_Position))
					{
						ref float3 position4 = ref error.m_Position;
						position4 += origin;
						error.m_ErrorType = ErrorType.OverlapExisting;
					}
					return;
				}
				Box3 val47 = default(Box3);
				val47.bounds = bounds7 + val4;
				val47.bounds = MathUtils.Expand(val47.bounds, float3.op_Implicit(-0.01f));
				val47.rotation = transform2.m_Rotation;
				Bounds3 val48 = default(Bounds3);
				Bounds3 val49 = default(Bounds3);
				if (MathUtils.Intersect(val40, val47, ref val48, ref val49))
				{
					float3 val50 = math.mul(val40.rotation, MathUtils.Center(val48));
					float3 val51 = math.mul(val47.rotation, MathUtils.Center(val49));
					error.m_Position = origin + math.lerp(val50, val51, 0.5f);
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
				return;
			}
			Box3 val52 = default(Box3);
			val52.bounds = bounds + val3;
			val52.bounds = MathUtils.Expand(val52.bounds, float3.op_Implicit(-0.01f));
			val52.rotation = m_Transform.m_Rotation;
			Bounds3 bounds9 = ObjectUtils.GetBounds(stack2, prefabObjectGeometryData2, stackData2);
			if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
			{
				bounds9.min.y = math.max(bounds9.min.y, 0f);
			}
			if (ObjectUtils.GetStandingLegCount(prefabObjectGeometryData2, out var legCount5))
			{
				Bounds3 val55 = default(Bounds3);
				Bounds3 val56 = default(Bounds3);
				Bounds3 val60 = default(Bounds3);
				Bounds3 val61 = default(Bounds3);
				for (int m = 0; m < legCount5; m++)
				{
					float3 val53 = val4 + ObjectUtils.GetStandingLegOffset(prefabObjectGeometryData2, m);
					if ((prefabObjectGeometryData2.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
					{
						Cylinder3 val54 = new Cylinder3
						{
							circle = new Circle2(prefabObjectGeometryData2.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref val53)).xz),
							height = new Bounds1(bounds9.min.y + 0.01f, prefabObjectGeometryData2.m_LegSize.y + 0.01f) + val53.y,
							rotation = transform2.m_Rotation
						};
						if (MathUtils.Intersect(val54, val52, ref val55, ref val56))
						{
							float3 val57 = math.mul(val52.rotation, MathUtils.Center(val56));
							float3 val58 = math.mul(val54.rotation, MathUtils.Center(val55));
							error.m_Position = origin + math.lerp(val57, val58, 0.5f);
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
					else if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
					{
						Box3 val59 = new Box3
						{
							bounds = 
							{
								min = 
								{
									y = bounds9.min.y + 0.01f
								}
							}
						};
						((float3)(ref val59.bounds.min)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * -0.5f + 0.01f;
						val59.bounds.max.y = prefabObjectGeometryData2.m_LegSize.y + 0.01f;
						((float3)(ref val59.bounds.max)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * 0.5f - 0.01f;
						ref Bounds3 bounds10 = ref val59.bounds;
						bounds10 += val53;
						val59.rotation = transform2.m_Rotation;
						if (MathUtils.Intersect(val52, val59, ref val60, ref val61))
						{
							float3 val62 = math.mul(val52.rotation, MathUtils.Center(val60));
							float3 val63 = math.mul(val59.rotation, MathUtils.Center(val61));
							error.m_Position = origin + math.lerp(val62, val63, 0.5f);
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
				}
				bounds9.min.y = prefabObjectGeometryData2.m_LegSize.y;
			}
			if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
			{
				Cylinder3 val64 = new Cylinder3
				{
					circle = new Circle2(prefabObjectGeometryData2.m_Size.x * 0.5f - 0.01f, ((float3)(ref val4)).xz),
					height = new Bounds1(bounds9.min.y + 0.01f, bounds9.max.y - 0.01f) + val4.y,
					rotation = transform2.m_Rotation
				};
				Bounds3 val65 = default(Bounds3);
				Bounds3 val66 = default(Bounds3);
				if (MathUtils.Intersect(val64, val52, ref val65, ref val66))
				{
					float3 val67 = math.mul(val52.rotation, MathUtils.Center(val66));
					float3 val68 = math.mul(val64.rotation, MathUtils.Center(val65));
					error.m_Position = origin + math.lerp(val67, val68, 0.5f);
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			else
			{
				Box3 val69 = default(Box3);
				val69.bounds = bounds9 + val4;
				val69.bounds = MathUtils.Expand(val69.bounds, float3.op_Implicit(-0.01f));
				val69.rotation = transform2.m_Rotation;
				Bounds3 val70 = default(Bounds3);
				Bounds3 val71 = default(Bounds3);
				if (MathUtils.Intersect(val52, val69, ref val70, ref val71))
				{
					float3 val72 = math.mul(val52.rotation, MathUtils.Center(val70));
					float3 val73 = math.mul(val69.rotation, MathUtils.Center(val71));
					error.m_Position = origin + math.lerp(val72, val73, 0.5f);
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
		}

		private void CheckOverlap2D(ref ErrorData error, Transform transformData2, ObjectGeometryData prefabObjectGeometryData2, Bounds3 bounds2, float3 origin)
		{
			//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
			//IL_073b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_0742: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_074b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ced: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0922: Unknown result type (might be due to invalid IL or missing references)
			//IL_0939: Unknown result type (might be due to invalid IL or missing references)
			//IL_093e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0940: Unknown result type (might be due to invalid IL or missing references)
			//IL_0945: Unknown result type (might be due to invalid IL or missing references)
			//IL_0949: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_0955: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0969: Unknown result type (might be due to invalid IL or missing references)
			//IL_096e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0970: Unknown result type (might be due to invalid IL or missing references)
			//IL_0975: Unknown result type (might be due to invalid IL or missing references)
			//IL_0977: Unknown result type (might be due to invalid IL or missing references)
			//IL_097c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0981: Unknown result type (might be due to invalid IL or missing references)
			//IL_0986: Unknown result type (might be due to invalid IL or missing references)
			//IL_0998: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_076e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0773: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bde: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_083a: Unknown result type (might be due to invalid IL or missing references)
			//IL_084e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0858: Unknown result type (might be due to invalid IL or missing references)
			//IL_0870: Unknown result type (might be due to invalid IL or missing references)
			//IL_087a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0884: Unknown result type (might be due to invalid IL or missing references)
			//IL_0887: Unknown result type (might be due to invalid IL or missing references)
			//IL_088c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0893: Unknown result type (might be due to invalid IL or missing references)
			//IL_0898: Unknown result type (might be due to invalid IL or missing references)
			//IL_089d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07db: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_0676: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c23: Unknown result type (might be due to invalid IL or missing references)
			//IL_08be: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_0626: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			Quad3 val4;
			float3 val6;
			if (ObjectUtils.GetStandingLegCount(m_PrefabObjectGeometryData, out var legCount))
			{
				Circle2 val = default(Circle2);
				Circle2 val2 = default(Circle2);
				Bounds2 val5 = default(Bounds2);
				Circle2 val7 = default(Circle2);
				Bounds2 val8 = default(Bounds2);
				Circle2 val10 = default(Circle2);
				Bounds2 val11 = default(Bounds2);
				Bounds2 val13 = default(Bounds2);
				Circle2 val14 = default(Circle2);
				Bounds2 val15 = default(Bounds2);
				Bounds2 val16 = default(Bounds2);
				for (int i = 0; i < legCount; i++)
				{
					float3 position = ObjectUtils.GetStandingLegPosition(m_PrefabObjectGeometryData, m_Transform, i) - origin;
					if ((m_PrefabObjectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
					{
						((Circle2)(ref val))._002Ector(m_PrefabObjectGeometryData.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref position)).xz);
						if (ObjectUtils.GetStandingLegCount(prefabObjectGeometryData2, out var legCount2))
						{
							for (int j = 0; j < legCount2; j++)
							{
								float3 position2 = ObjectUtils.GetStandingLegPosition(prefabObjectGeometryData2, transformData2, j) - origin;
								if ((prefabObjectGeometryData2.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
								{
									((Circle2)(ref val2))._002Ector(prefabObjectGeometryData2.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref position2)).xz);
									if (MathUtils.Intersect(val, val2))
									{
										((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(MathUtils.Bounds(val) & MathUtils.Bounds(val2));
										error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
										error.m_ErrorType = ErrorType.OverlapExisting;
									}
								}
								else if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
								{
									Bounds3 val3 = default(Bounds3);
									((float3)(ref val3.min)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * -0.5f;
									((float3)(ref val3.max)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * 0.5f;
									val4 = ObjectUtils.CalculateBaseCorners(position2, transformData2.m_Rotation, MathUtils.Expand(val3, float3.op_Implicit(-0.01f)));
									if (MathUtils.Intersect(((Quad3)(ref val4)).xz, val, ref val5))
									{
										((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(val5);
										error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
										error.m_ErrorType = ErrorType.OverlapExisting;
									}
								}
							}
						}
						else if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
						{
							float num = prefabObjectGeometryData2.m_Size.x * 0.5f - 0.01f;
							val6 = transformData2.m_Position - origin;
							((Circle2)(ref val7))._002Ector(num, ((float3)(ref val6)).xz);
							if (MathUtils.Intersect(val, val7))
							{
								((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(MathUtils.Bounds(val) & MathUtils.Bounds(val7));
								error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
								error.m_ErrorType = ErrorType.OverlapExisting;
							}
						}
						else
						{
							val4 = ObjectUtils.CalculateBaseCorners(transformData2.m_Position - origin, transformData2.m_Rotation, MathUtils.Expand(prefabObjectGeometryData2.m_Bounds, float3.op_Implicit(-0.01f)));
							if (MathUtils.Intersect(((Quad3)(ref val4)).xz, val, ref val8))
							{
								((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(val8);
								error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
								error.m_ErrorType = ErrorType.OverlapExisting;
							}
						}
					}
					else
					{
						if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) != GeometryFlags.None)
						{
							continue;
						}
						Bounds3 val9 = default(Bounds3);
						((float3)(ref val9.min)).xz = ((float3)(ref m_PrefabObjectGeometryData.m_LegSize)).xz * -0.5f;
						((float3)(ref val9.max)).xz = ((float3)(ref m_PrefabObjectGeometryData.m_LegSize)).xz * 0.5f;
						val4 = ObjectUtils.CalculateBaseCorners(position, m_Transform.m_Rotation, MathUtils.Expand(val9, float3.op_Implicit(-0.01f)));
						Quad2 xz = ((Quad3)(ref val4)).xz;
						if (ObjectUtils.GetStandingLegCount(prefabObjectGeometryData2, out var legCount3))
						{
							for (int k = 0; k < legCount3; k++)
							{
								float3 position3 = ObjectUtils.GetStandingLegPosition(prefabObjectGeometryData2, transformData2, k) - origin;
								if ((prefabObjectGeometryData2.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
								{
									((Circle2)(ref val10))._002Ector(prefabObjectGeometryData2.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref position3)).xz);
									if (MathUtils.Intersect(xz, val10, ref val11))
									{
										((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(val11);
										error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
										error.m_ErrorType = ErrorType.OverlapExisting;
									}
								}
								else if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
								{
									Bounds3 val12 = default(Bounds3);
									((float3)(ref val12.min)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * -0.5f;
									((float3)(ref val12.max)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * 0.5f;
									val4 = ObjectUtils.CalculateBaseCorners(position3, transformData2.m_Rotation, MathUtils.Expand(val12, float3.op_Implicit(-0.01f)));
									Quad2 xz2 = ((Quad3)(ref val4)).xz;
									if (MathUtils.Intersect(xz, xz2, ref val13))
									{
										((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(val13);
										error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
										error.m_ErrorType = ErrorType.OverlapExisting;
									}
								}
							}
						}
						else if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
						{
							float num2 = prefabObjectGeometryData2.m_Size.x * 0.5f - 0.01f;
							val6 = transformData2.m_Position - origin;
							((Circle2)(ref val14))._002Ector(num2, ((float3)(ref val6)).xz);
							if (MathUtils.Intersect(xz, val14, ref val15))
							{
								((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(val15);
								error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
								error.m_ErrorType = ErrorType.OverlapExisting;
							}
						}
						else
						{
							val4 = ObjectUtils.CalculateBaseCorners(transformData2.m_Position - origin, transformData2.m_Rotation, MathUtils.Expand(prefabObjectGeometryData2.m_Bounds, float3.op_Implicit(-0.01f)));
							Quad2 xz3 = ((Quad3)(ref val4)).xz;
							if (MathUtils.Intersect(xz, xz3, ref val16))
							{
								((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(val16);
								error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
								error.m_ErrorType = ErrorType.OverlapExisting;
							}
						}
					}
				}
				return;
			}
			if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
			{
				float num3 = m_PrefabObjectGeometryData.m_Size.x * 0.5f - 0.01f;
				val6 = m_Transform.m_Position - origin;
				Circle2 val17 = default(Circle2);
				((Circle2)(ref val17))._002Ector(num3, ((float3)(ref val6)).xz);
				if (ObjectUtils.GetStandingLegCount(prefabObjectGeometryData2, out var legCount4))
				{
					Circle2 val18 = default(Circle2);
					Bounds2 val20 = default(Bounds2);
					for (int l = 0; l < legCount4; l++)
					{
						float3 position4 = ObjectUtils.GetStandingLegPosition(prefabObjectGeometryData2, transformData2, l) - origin;
						if ((prefabObjectGeometryData2.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
						{
							((Circle2)(ref val18))._002Ector(prefabObjectGeometryData2.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref position4)).xz);
							if (MathUtils.Intersect(val17, val18))
							{
								((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(MathUtils.Bounds(val17) & MathUtils.Bounds(val18));
								error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
								error.m_ErrorType = ErrorType.OverlapExisting;
							}
						}
						else if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
						{
							Bounds3 val19 = default(Bounds3);
							((float3)(ref val19.min)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * -0.5f;
							((float3)(ref val19.max)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * 0.5f;
							val4 = ObjectUtils.CalculateBaseCorners(position4, transformData2.m_Rotation, MathUtils.Expand(val19, float3.op_Implicit(-0.01f)));
							if (MathUtils.Intersect(((Quad3)(ref val4)).xz, val17, ref val20))
							{
								((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(val20);
								error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
								error.m_ErrorType = ErrorType.OverlapExisting;
							}
						}
					}
				}
				else if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
				{
					float num4 = prefabObjectGeometryData2.m_Size.x * 0.5f - 0.01f;
					val6 = transformData2.m_Position - origin;
					Circle2 val21 = default(Circle2);
					((Circle2)(ref val21))._002Ector(num4, ((float3)(ref val6)).xz);
					if (MathUtils.Intersect(val17, val21))
					{
						((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(MathUtils.Bounds(val17) & MathUtils.Bounds(val21));
						error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
						error.m_ErrorType = ErrorType.OverlapExisting;
					}
				}
				else
				{
					val4 = ObjectUtils.CalculateBaseCorners(transformData2.m_Position - origin, transformData2.m_Rotation, MathUtils.Expand(prefabObjectGeometryData2.m_Bounds, float3.op_Implicit(-0.01f)));
					Bounds2 val22 = default(Bounds2);
					if (MathUtils.Intersect(((Quad3)(ref val4)).xz, val17, ref val22))
					{
						((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(val22);
						error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
						error.m_ErrorType = ErrorType.OverlapExisting;
					}
				}
				return;
			}
			val4 = ObjectUtils.CalculateBaseCorners(m_Transform.m_Position - origin, m_Transform.m_Rotation, MathUtils.Expand(m_PrefabObjectGeometryData.m_Bounds, float3.op_Implicit(-0.01f)));
			Quad2 xz4 = ((Quad3)(ref val4)).xz;
			if (ObjectUtils.GetStandingLegCount(prefabObjectGeometryData2, out var legCount5))
			{
				Circle2 val23 = default(Circle2);
				Bounds2 val24 = default(Bounds2);
				Bounds2 val26 = default(Bounds2);
				for (int m = 0; m < legCount5; m++)
				{
					float3 position5 = ObjectUtils.GetStandingLegPosition(prefabObjectGeometryData2, transformData2, m) - origin;
					if ((prefabObjectGeometryData2.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
					{
						((Circle2)(ref val23))._002Ector(prefabObjectGeometryData2.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref position5)).xz);
						if (MathUtils.Intersect(xz4, val23, ref val24))
						{
							((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(val24);
							error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
					else if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
					{
						Bounds3 val25 = default(Bounds3);
						((float3)(ref val25.min)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * -0.5f;
						((float3)(ref val25.max)).xz = ((float3)(ref prefabObjectGeometryData2.m_LegSize)).xz * 0.5f;
						val4 = ObjectUtils.CalculateBaseCorners(position5, transformData2.m_Rotation, MathUtils.Expand(val25, float3.op_Implicit(-0.01f)));
						Quad2 xz5 = ((Quad3)(ref val4)).xz;
						if (MathUtils.Intersect(xz4, xz5, ref val26))
						{
							((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(val26);
							error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
				}
			}
			else if ((prefabObjectGeometryData2.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
			{
				float num5 = prefabObjectGeometryData2.m_Size.x * 0.5f - 0.01f;
				val6 = transformData2.m_Position - origin;
				Circle2 val27 = default(Circle2);
				((Circle2)(ref val27))._002Ector(num5, ((float3)(ref val6)).xz);
				Bounds2 val28 = default(Bounds2);
				if (MathUtils.Intersect(xz4, val27, ref val28))
				{
					((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(val28);
					error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			else
			{
				val4 = ObjectUtils.CalculateBaseCorners(transformData2.m_Position - origin, transformData2.m_Rotation, MathUtils.Expand(prefabObjectGeometryData2.m_Bounds, float3.op_Implicit(-0.01f)));
				Quad2 xz6 = ((Quad3)(ref val4)).xz;
				Bounds2 val29 = default(Bounds2);
				if (MathUtils.Intersect(xz4, xz6, ref val29))
				{
					((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(val29);
					error.m_Position.y = MathUtils.Center(((Bounds3)(ref bounds2)).y & ((Bounds3)(ref m_ObjectBounds)).y);
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
		}
	}

	private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Entity m_ObjectEntity;

		public Entity m_AttachedParent;

		public Entity m_TopLevelEntity;

		public Entity m_EdgeEntity;

		public Entity m_NodeEntity;

		public Entity m_IgnoreNode;

		public Edge m_OwnerNodes;

		public Bounds3 m_ObjectBounds;

		public Transform m_Transform;

		public Stack m_ObjectStack;

		public CollisionMask m_CollisionMask;

		public ObjectGeometryData m_PrefabObjectGeometryData;

		public StackData m_ObjectStackData;

		public bool m_Optional;

		public bool m_EditorMode;

		public ValidationSystem.EntityData m_Data;

		public ParallelWriter<ErrorData> m_ErrorQueue;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if ((m_CollisionMask & CollisionMask.OnGround) != 0)
			{
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_ObjectBounds)).xz);
			}
			return MathUtils.Intersect(bounds.m_Bounds, m_ObjectBounds);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity edgeEntity2)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			if ((m_CollisionMask & CollisionMask.OnGround) != 0)
			{
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_ObjectBounds)).xz))
				{
					return;
				}
			}
			else if (!MathUtils.Intersect(bounds.m_Bounds, m_ObjectBounds))
			{
				return;
			}
			if (m_Data.m_Hidden.HasComponent(edgeEntity2) || !m_Data.m_EdgeGeometry.HasComponent(edgeEntity2))
			{
				return;
			}
			Edge edgeData = m_Data.m_Edge[edgeEntity2];
			bool flag = true;
			bool flag2 = true;
			if (edgeEntity2 == m_AttachedParent || edgeData.m_Start == m_AttachedParent || edgeData.m_End == m_AttachedParent)
			{
				return;
			}
			Entity val = m_ObjectEntity;
			while (m_Data.m_Owner.HasComponent(val))
			{
				val = m_Data.m_Owner[val].m_Owner;
				if (m_Data.m_Temp.HasComponent(val))
				{
					Temp temp = m_Data.m_Temp[val];
					if (temp.m_Original != Entity.Null)
					{
						val = temp.m_Original;
					}
				}
				if (edgeEntity2 == val || edgeData.m_Start == val || edgeData.m_End == val)
				{
					return;
				}
			}
			Entity val2 = edgeEntity2;
			bool hasOwner = false;
			Owner owner = default(Owner);
			while (m_Data.m_Owner.TryGetComponent(val2, ref owner) && !m_Data.m_Building.HasComponent(val2))
			{
				Entity owner2 = owner.m_Owner;
				hasOwner = true;
				if (m_Data.m_AssetStamp.HasComponent(owner2))
				{
					if (!(owner2 == m_ObjectEntity))
					{
						break;
					}
					return;
				}
				val2 = owner2;
			}
			Owner owner3 = default(Owner);
			Edge edge = default(Edge);
			if (!(m_TopLevelEntity == val2) && ((!(m_EdgeEntity != Entity.Null) && !(m_NodeEntity != Entity.Null)) || ((!m_Data.m_Owner.TryGetComponent(edgeEntity2, ref owner3) || !m_Data.m_Edge.TryGetComponent(owner3.m_Owner, ref edge) || (!(m_NodeEntity == edge.m_Start) && !(m_NodeEntity == edge.m_End))) && (!m_Data.m_Owner.TryGetComponent(edgeData.m_Start, ref owner3) || (!(owner3.m_Owner == m_EdgeEntity) && (!m_Data.m_Edge.TryGetComponent(owner3.m_Owner, ref edge) || (!(m_NodeEntity == edge.m_Start) && !(m_NodeEntity == edge.m_End))))) && (!m_Data.m_Owner.TryGetComponent(edgeData.m_End, ref owner3) || (!(owner3.m_Owner == m_EdgeEntity) && (!m_Data.m_Edge.TryGetComponent(owner3.m_Owner, ref edge) || (!(m_NodeEntity == edge.m_Start) && !(m_NodeEntity == edge.m_End))))))))
			{
				Composition compositionData = m_Data.m_Composition[edgeEntity2];
				EdgeGeometry edgeGeometryData = m_Data.m_EdgeGeometry[edgeEntity2];
				StartNodeGeometry startNodeGeometryData = m_Data.m_StartNodeGeometry[edgeEntity2];
				EndNodeGeometry endNodeGeometryData = m_Data.m_EndNodeGeometry[edgeEntity2];
				float3 origin = MathUtils.Center(bounds.m_Bounds);
				flag &= edgeData.m_Start != m_OwnerNodes.m_Start && edgeData.m_Start != m_OwnerNodes.m_End;
				flag2 &= edgeData.m_End != m_OwnerNodes.m_Start && edgeData.m_End != m_OwnerNodes.m_End;
				if (edgeData.m_Start == m_IgnoreNode)
				{
					flag &= (m_Data.m_PrefabComposition[compositionData.m_StartNode].m_Flags.m_General & CompositionFlags.General.Roundabout) == 0;
				}
				if (edgeData.m_End == m_IgnoreNode)
				{
					flag2 &= (m_Data.m_PrefabComposition[compositionData.m_EndNode].m_Flags.m_General & CompositionFlags.General.Roundabout) == 0;
				}
				CheckOverlap(val2, edgeEntity2, bounds.m_Bounds, edgeData, compositionData, edgeGeometryData, startNodeGeometryData, endNodeGeometryData, origin, flag, flag2, essential: false, hasOwner);
			}
		}

		public void CheckOverlap(Entity topLevelEntity2, Entity edgeEntity2, Bounds3 bounds2, Edge edgeData2, Composition compositionData2, EdgeGeometry edgeGeometryData2, StartNodeGeometry startNodeGeometryData2, EndNodeGeometry endNodeGeometryData2, float3 origin, bool checkStartNode, bool checkEndNode, bool essential, bool hasOwner)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			NetCompositionData netCompositionData = m_Data.m_PrefabComposition[compositionData2.m_Edge];
			NetCompositionData netCompositionData2 = m_Data.m_PrefabComposition[compositionData2.m_StartNode];
			NetCompositionData netCompositionData3 = m_Data.m_PrefabComposition[compositionData2.m_EndNode];
			CollisionMask collisionMask = NetUtils.GetCollisionMask(netCompositionData, !m_EditorMode || hasOwner);
			CollisionMask collisionMask2 = NetUtils.GetCollisionMask(netCompositionData2, !m_EditorMode || hasOwner);
			CollisionMask collisionMask3 = NetUtils.GetCollisionMask(netCompositionData3, !m_EditorMode || hasOwner);
			if (!checkStartNode)
			{
				collisionMask2 = (CollisionMask)0;
			}
			if (!checkEndNode)
			{
				collisionMask3 = (CollisionMask)0;
			}
			CollisionMask collisionMask4 = collisionMask | collisionMask2 | collisionMask3;
			if ((m_CollisionMask & collisionMask4) == 0)
			{
				return;
			}
			DynamicBuffer<NetCompositionArea> edgeCompositionAreas = default(DynamicBuffer<NetCompositionArea>);
			DynamicBuffer<NetCompositionArea> startCompositionAreas = default(DynamicBuffer<NetCompositionArea>);
			DynamicBuffer<NetCompositionArea> endCompositionAreas = default(DynamicBuffer<NetCompositionArea>);
			if ((m_PrefabObjectGeometryData.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) != GeometryFlags.Overridable)
			{
				edgeCompositionAreas = m_Data.m_PrefabCompositionAreas[compositionData2.m_Edge];
				startCompositionAreas = m_Data.m_PrefabCompositionAreas[compositionData2.m_StartNode];
				endCompositionAreas = m_Data.m_PrefabCompositionAreas[compositionData2.m_EndNode];
			}
			ErrorData error = default(ErrorData);
			if ((m_CollisionMask & CollisionMask.OnGround) == 0 || MathUtils.Intersect(bounds2, m_ObjectBounds))
			{
				CheckOverlap3D(ref error, collisionMask, collisionMask2, collisionMask3, edgeData2, edgeGeometryData2, startNodeGeometryData2, endNodeGeometryData2, netCompositionData, netCompositionData2, netCompositionData3, edgeCompositionAreas, startCompositionAreas, endCompositionAreas, origin);
			}
			if (error.m_ErrorType == ErrorType.None && CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask4))
			{
				CheckOverlap2D(ref error, collisionMask, collisionMask2, collisionMask3, edgeData2, edgeGeometryData2, startNodeGeometryData2, endNodeGeometryData2, netCompositionData, netCompositionData2, netCompositionData3, edgeCompositionAreas, startCompositionAreas, endCompositionAreas, origin);
			}
			if (error.m_ErrorType == ErrorType.None)
			{
				return;
			}
			if (m_Optional)
			{
				error.m_ErrorSeverity = ErrorSeverity.Override;
				error.m_TempEntity = m_ObjectEntity;
			}
			else
			{
				error.m_ErrorSeverity = ErrorSeverity.Error;
				error.m_TempEntity = m_ObjectEntity;
				error.m_PermanentEntity = edgeEntity2;
				if (!essential && topLevelEntity2 != edgeEntity2 && topLevelEntity2 != Entity.Null)
				{
					PrefabRef prefabRef = m_Data.m_PrefabRef[topLevelEntity2];
					if (m_Data.m_PrefabObjectGeometry.HasComponent(prefabRef.m_Prefab) && (m_Data.m_PrefabObjectGeometry[prefabRef.m_Prefab].m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) == (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden) && !m_Data.m_Attached.HasComponent(topLevelEntity2) && (!m_Data.m_Temp.HasComponent(topLevelEntity2) || (m_Data.m_Temp[topLevelEntity2].m_Flags & TempFlags.Essential) == 0))
					{
						error.m_ErrorSeverity = ErrorSeverity.Warning;
						error.m_PermanentEntity = topLevelEntity2;
					}
				}
			}
			m_ErrorQueue.Enqueue(error);
		}

		private void CheckOverlap3D(ref ErrorData error, CollisionMask edgeCollisionMask2, CollisionMask startCollisionMask2, CollisionMask endCollisionMask2, Edge edgeData2, EdgeGeometry edgeGeometryData2, StartNodeGeometry startNodeGeometryData2, EndNodeGeometry endNodeGeometryData2, NetCompositionData edgeCompositionData2, NetCompositionData startCompositionData2, NetCompositionData endCompositionData2, DynamicBuffer<NetCompositionArea> edgeCompositionAreas2, DynamicBuffer<NetCompositionArea> startCompositionAreas2, DynamicBuffer<NetCompositionArea> endCompositionAreas2, float3 origin)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_058d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_060f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			Bounds3 intersection = default(Bounds3);
			intersection.min = float3.op_Implicit(float.MaxValue);
			intersection.max = float3.op_Implicit(float.MinValue);
			float3 val = math.mul(math.inverse(m_Transform.m_Rotation), m_Transform.m_Position - origin);
			Bounds3 bounds = ObjectUtils.GetBounds(m_ObjectStack, m_PrefabObjectGeometryData, m_ObjectStackData);
			if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
			{
				bounds.min.y = math.max(bounds.min.y, 0f);
			}
			if (ObjectUtils.GetStandingLegCount(m_PrefabObjectGeometryData, out var legCount))
			{
				for (int i = 0; i < legCount; i++)
				{
					float3 val2 = val + ObjectUtils.GetStandingLegOffset(m_PrefabObjectGeometryData, i);
					if ((m_PrefabObjectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
					{
						Cylinder3 cylinder = new Cylinder3
						{
							circle = new Circle2(m_PrefabObjectGeometryData.m_LegSize.x * 0.5f, ((float3)(ref val2)).xz),
							height = new Bounds1(bounds.min.y, m_PrefabObjectGeometryData.m_LegSize.y) + val2.y,
							rotation = m_Transform.m_Rotation
						};
						if ((edgeCollisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edgeData2, m_TopLevelEntity, edgeGeometryData2, -origin, cylinder, m_ObjectBounds, edgeCompositionData2, edgeCompositionAreas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
						if ((startCollisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edgeData2.m_Start, m_TopLevelEntity, startNodeGeometryData2.m_Geometry, -origin, cylinder, m_ObjectBounds, startCompositionData2, startCompositionAreas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
						if ((endCollisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edgeData2.m_End, m_TopLevelEntity, endNodeGeometryData2.m_Geometry, -origin, cylinder, m_ObjectBounds, endCompositionData2, endCompositionAreas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
					else if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
					{
						Box3 box = new Box3
						{
							bounds = 
							{
								min = 
								{
									y = bounds.min.y
								}
							}
						};
						((float3)(ref box.bounds.min)).xz = ((float3)(ref m_PrefabObjectGeometryData.m_LegSize)).xz * -0.5f;
						box.bounds.max.y = m_PrefabObjectGeometryData.m_LegSize.y;
						((float3)(ref box.bounds.max)).xz = ((float3)(ref m_PrefabObjectGeometryData.m_LegSize)).xz * 0.5f;
						ref Bounds3 bounds2 = ref box.bounds;
						bounds2 += val2;
						box.rotation = m_Transform.m_Rotation;
						if ((edgeCollisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edgeData2, m_TopLevelEntity, edgeGeometryData2, -origin, box, m_ObjectBounds, edgeCompositionData2, edgeCompositionAreas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
						if ((startCollisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edgeData2.m_Start, m_TopLevelEntity, startNodeGeometryData2.m_Geometry, -origin, box, m_ObjectBounds, startCompositionData2, startCompositionAreas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
						if ((endCollisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edgeData2.m_End, m_TopLevelEntity, endNodeGeometryData2.m_Geometry, -origin, box, m_ObjectBounds, endCompositionData2, endCompositionAreas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
						}
					}
				}
				bounds.min.y = m_PrefabObjectGeometryData.m_LegSize.y;
			}
			if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
			{
				Cylinder3 cylinder2 = new Cylinder3
				{
					circle = new Circle2(m_PrefabObjectGeometryData.m_Size.x * 0.5f, ((float3)(ref val)).xz),
					height = new Bounds1(bounds.min.y, bounds.max.y) + val.y,
					rotation = m_Transform.m_Rotation
				};
				if ((edgeCollisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edgeData2, m_TopLevelEntity, edgeGeometryData2, -origin, cylinder2, m_ObjectBounds, edgeCompositionData2, edgeCompositionAreas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
				if ((startCollisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edgeData2.m_Start, m_TopLevelEntity, startNodeGeometryData2.m_Geometry, -origin, cylinder2, m_ObjectBounds, startCompositionData2, startCompositionAreas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
				if ((endCollisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edgeData2.m_End, m_TopLevelEntity, endNodeGeometryData2.m_Geometry, -origin, cylinder2, m_ObjectBounds, endCompositionData2, endCompositionAreas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			else
			{
				Box3 box2 = new Box3
				{
					bounds = bounds + val,
					rotation = m_Transform.m_Rotation
				};
				if ((edgeCollisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edgeData2, m_TopLevelEntity, edgeGeometryData2, -origin, box2, m_ObjectBounds, edgeCompositionData2, edgeCompositionAreas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
				if ((startCollisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edgeData2.m_Start, m_TopLevelEntity, startNodeGeometryData2.m_Geometry, -origin, box2, m_ObjectBounds, startCompositionData2, startCompositionAreas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
				if ((endCollisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edgeData2.m_End, m_TopLevelEntity, endNodeGeometryData2.m_Geometry, -origin, box2, m_ObjectBounds, endCompositionData2, endCompositionAreas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
				}
			}
			if (error.m_ErrorType != ErrorType.None)
			{
				error.m_Position = origin + MathUtils.Center(intersection);
			}
		}

		private void CheckOverlap2D(ref ErrorData error, CollisionMask edgeCollisionMask2, CollisionMask startCollisionMask2, CollisionMask endCollisionMask2, Edge edgeData2, EdgeGeometry edgeGeometryData2, StartNodeGeometry startNodeGeometryData2, EndNodeGeometry endNodeGeometryData2, NetCompositionData edgeCompositionData2, NetCompositionData startCompositionData2, NetCompositionData endCompositionData2, DynamicBuffer<NetCompositionArea> edgeCompositionAreas2, DynamicBuffer<NetCompositionArea> startCompositionAreas2, DynamicBuffer<NetCompositionArea> endCompositionAreas2, float3 origin)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0693: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0655: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0668: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0710: Unknown result type (might be due to invalid IL or missing references)
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0723: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_0730: Unknown result type (might be due to invalid IL or missing references)
			//IL_0737: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Unknown result type (might be due to invalid IL or missing references)
			//IL_078c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0749: Unknown result type (might be due to invalid IL or missing references)
			//IL_0756: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_0766: Unknown result type (might be due to invalid IL or missing references)
			//IL_0770: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			Bounds2 intersection = default(Bounds2);
			intersection.min = float2.op_Implicit(float.MaxValue);
			intersection.max = float2.op_Implicit(float.MinValue);
			Bounds1 val = default(Bounds1);
			val.min = float.MaxValue;
			val.max = float.MinValue;
			Quad3 val2;
			if (ObjectUtils.GetStandingLegCount(m_PrefabObjectGeometryData, out var legCount))
			{
				Circle2 circle = default(Circle2);
				for (int i = 0; i < legCount; i++)
				{
					float3 position = ObjectUtils.GetStandingLegPosition(m_PrefabObjectGeometryData, m_Transform, i) - origin;
					if ((m_PrefabObjectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
					{
						((Circle2)(ref circle))._002Ector(m_PrefabObjectGeometryData.m_LegSize.x * 0.5f, ((float3)(ref position)).xz);
						if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, edgeCollisionMask2) && Game.Net.ValidationHelpers.Intersect(edgeData2, m_TopLevelEntity, edgeGeometryData2, -((float3)(ref origin)).xz, circle, ((Bounds3)(ref m_ObjectBounds)).xz, edgeCompositionData2, edgeCompositionAreas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
							val |= MathUtils.Center(((Bounds3)(ref edgeGeometryData2.m_Bounds)).y & ((Bounds3)(ref m_ObjectBounds)).y);
						}
						if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, startCollisionMask2) && Game.Net.ValidationHelpers.Intersect(edgeData2.m_Start, m_TopLevelEntity, startNodeGeometryData2.m_Geometry, -((float3)(ref origin)).xz, circle, ((Bounds3)(ref m_ObjectBounds)).xz, startCompositionData2, startCompositionAreas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
							val |= MathUtils.Center(((Bounds3)(ref startNodeGeometryData2.m_Geometry.m_Bounds)).y & ((Bounds3)(ref m_ObjectBounds)).y);
						}
						if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, endCollisionMask2) && Game.Net.ValidationHelpers.Intersect(edgeData2.m_End, m_TopLevelEntity, endNodeGeometryData2.m_Geometry, -((float3)(ref origin)).xz, circle, ((Bounds3)(ref m_ObjectBounds)).xz, endCompositionData2, endCompositionAreas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
							val |= MathUtils.Center(((Bounds3)(ref endNodeGeometryData2.m_Geometry.m_Bounds)).y & ((Bounds3)(ref m_ObjectBounds)).y);
						}
					}
					else if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
					{
						Bounds3 bounds = default(Bounds3);
						((float3)(ref bounds.min)).xz = ((float3)(ref m_PrefabObjectGeometryData.m_LegSize)).xz * -0.5f;
						((float3)(ref bounds.max)).xz = ((float3)(ref m_PrefabObjectGeometryData.m_LegSize)).xz * 0.5f;
						val2 = ObjectUtils.CalculateBaseCorners(position, m_Transform.m_Rotation, bounds);
						Quad2 xz = ((Quad3)(ref val2)).xz;
						if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, edgeCollisionMask2) && Game.Net.ValidationHelpers.Intersect(edgeData2, m_TopLevelEntity, edgeGeometryData2, -((float3)(ref origin)).xz, xz, ((Bounds3)(ref m_ObjectBounds)).xz, edgeCompositionData2, edgeCompositionAreas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
							val |= MathUtils.Center(((Bounds3)(ref edgeGeometryData2.m_Bounds)).y & ((Bounds3)(ref m_ObjectBounds)).y);
						}
						if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, startCollisionMask2) && Game.Net.ValidationHelpers.Intersect(edgeData2.m_Start, m_TopLevelEntity, startNodeGeometryData2.m_Geometry, -((float3)(ref origin)).xz, xz, ((Bounds3)(ref m_ObjectBounds)).xz, startCompositionData2, startCompositionAreas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
							val |= MathUtils.Center(((Bounds3)(ref startNodeGeometryData2.m_Geometry.m_Bounds)).y & ((Bounds3)(ref m_ObjectBounds)).y);
						}
						if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, endCollisionMask2) && Game.Net.ValidationHelpers.Intersect(edgeData2.m_End, m_TopLevelEntity, endNodeGeometryData2.m_Geometry, -((float3)(ref origin)).xz, xz, ((Bounds3)(ref m_ObjectBounds)).xz, endCompositionData2, endCompositionAreas2, ref intersection))
						{
							error.m_ErrorType = ErrorType.OverlapExisting;
							val |= MathUtils.Center(((Bounds3)(ref endNodeGeometryData2.m_Geometry.m_Bounds)).y & ((Bounds3)(ref m_ObjectBounds)).y);
						}
					}
				}
			}
			else if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
			{
				float num = m_PrefabObjectGeometryData.m_Size.x * 0.5f;
				float3 val3 = m_Transform.m_Position - origin;
				Circle2 circle2 = default(Circle2);
				((Circle2)(ref circle2))._002Ector(num, ((float3)(ref val3)).xz);
				if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, edgeCollisionMask2) && Game.Net.ValidationHelpers.Intersect(edgeData2, m_TopLevelEntity, edgeGeometryData2, -((float3)(ref origin)).xz, circle2, ((Bounds3)(ref m_ObjectBounds)).xz, edgeCompositionData2, edgeCompositionAreas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
					val |= MathUtils.Center(((Bounds3)(ref edgeGeometryData2.m_Bounds)).y & ((Bounds3)(ref m_ObjectBounds)).y);
				}
				if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, startCollisionMask2) && Game.Net.ValidationHelpers.Intersect(edgeData2.m_Start, m_TopLevelEntity, startNodeGeometryData2.m_Geometry, -((float3)(ref origin)).xz, circle2, ((Bounds3)(ref m_ObjectBounds)).xz, startCompositionData2, startCompositionAreas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
					val |= MathUtils.Center(((Bounds3)(ref startNodeGeometryData2.m_Geometry.m_Bounds)).y & ((Bounds3)(ref m_ObjectBounds)).y);
				}
				if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, endCollisionMask2) && Game.Net.ValidationHelpers.Intersect(edgeData2.m_End, m_TopLevelEntity, endNodeGeometryData2.m_Geometry, -((float3)(ref origin)).xz, circle2, ((Bounds3)(ref m_ObjectBounds)).xz, endCompositionData2, endCompositionAreas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
					val |= MathUtils.Center(((Bounds3)(ref endNodeGeometryData2.m_Geometry.m_Bounds)).y & ((Bounds3)(ref m_ObjectBounds)).y);
				}
			}
			else
			{
				val2 = ObjectUtils.CalculateBaseCorners(m_Transform.m_Position - origin, m_Transform.m_Rotation, m_PrefabObjectGeometryData.m_Bounds);
				Quad2 xz2 = ((Quad3)(ref val2)).xz;
				if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, edgeCollisionMask2) && Game.Net.ValidationHelpers.Intersect(edgeData2, m_TopLevelEntity, edgeGeometryData2, -((float3)(ref origin)).xz, xz2, ((Bounds3)(ref m_ObjectBounds)).xz, edgeCompositionData2, edgeCompositionAreas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
					val |= MathUtils.Center(((Bounds3)(ref edgeGeometryData2.m_Bounds)).y & ((Bounds3)(ref m_ObjectBounds)).y);
				}
				if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, startCollisionMask2) && Game.Net.ValidationHelpers.Intersect(edgeData2.m_Start, m_TopLevelEntity, startNodeGeometryData2.m_Geometry, -((float3)(ref origin)).xz, xz2, ((Bounds3)(ref m_ObjectBounds)).xz, startCompositionData2, startCompositionAreas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
					val |= MathUtils.Center(((Bounds3)(ref startNodeGeometryData2.m_Geometry.m_Bounds)).y & ((Bounds3)(ref m_ObjectBounds)).y);
				}
				if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, endCollisionMask2) && Game.Net.ValidationHelpers.Intersect(edgeData2.m_End, m_TopLevelEntity, endNodeGeometryData2.m_Geometry, -((float3)(ref origin)).xz, xz2, ((Bounds3)(ref m_ObjectBounds)).xz, endCompositionData2, endCompositionAreas2, ref intersection))
				{
					error.m_ErrorType = ErrorType.OverlapExisting;
					val |= MathUtils.Center(((Bounds3)(ref endNodeGeometryData2.m_Geometry.m_Bounds)).y & ((Bounds3)(ref m_ObjectBounds)).y);
				}
			}
			if (error.m_ErrorType != ErrorType.None)
			{
				((float3)(ref error.m_Position)).xz = ((float3)(ref origin)).xz + MathUtils.Center(intersection);
				error.m_Position.y = MathUtils.Center(val);
			}
		}
	}

	private struct AreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
	{
		public Entity m_ObjectEntity;

		public Bounds3 m_ObjectBounds;

		public bool m_IgnoreCollisions;

		public bool m_IgnoreProtectedAreas;

		public bool m_Optional;

		public bool m_EditorMode;

		public Transform m_TransformData;

		public CollisionMask m_CollisionMask;

		public ObjectGeometryData m_PrefabObjectGeometryData;

		public ValidationSystem.EntityData m_Data;

		public ParallelWriter<ErrorData> m_ErrorQueue;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_ObjectBounds)).xz);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem2)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_076d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0778: Unknown result type (might be due to invalid IL or missing references)
			//IL_0783: Unknown result type (might be due to invalid IL or missing references)
			//IL_0788: Unknown result type (might be due to invalid IL or missing references)
			//IL_078d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Unknown result type (might be due to invalid IL or missing references)
			//IL_0723: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0734: Unknown result type (might be due to invalid IL or missing references)
			//IL_07db: Unknown result type (might be due to invalid IL or missing references)
			//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0743: Unknown result type (might be due to invalid IL or missing references)
			//IL_0749: Unknown result type (might be due to invalid IL or missing references)
			//IL_074e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_0758: Unknown result type (might be due to invalid IL or missing references)
			//IL_0828: Unknown result type (might be due to invalid IL or missing references)
			//IL_082d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0835: Unknown result type (might be due to invalid IL or missing references)
			//IL_083a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0811: Unknown result type (might be due to invalid IL or missing references)
			//IL_0816: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_ObjectBounds)).xz) || m_Data.m_Hidden.HasComponent(areaItem2.m_Area) || (m_Data.m_Area[areaItem2.m_Area].m_Flags & AreaFlags.Slave) != 0)
			{
				return;
			}
			PrefabRef prefabRef = m_Data.m_PrefabRef[areaItem2.m_Area];
			AreaGeometryData areaGeometryData = m_Data.m_PrefabAreaGeometry[prefabRef.m_Prefab];
			AreaUtils.SetCollisionFlags(ref areaGeometryData, !m_EditorMode || m_Data.m_Owner.HasComponent(areaItem2.m_Area));
			if ((areaGeometryData.m_Flags & (Game.Areas.GeometryFlags.PhysicalGeometry | Game.Areas.GeometryFlags.ProtectedArea)) == 0)
			{
				return;
			}
			if ((areaGeometryData.m_Flags & Game.Areas.GeometryFlags.ProtectedArea) != 0)
			{
				if (!m_Data.m_Native.HasComponent(areaItem2.m_Area) || m_IgnoreProtectedAreas)
				{
					return;
				}
			}
			else if (m_IgnoreCollisions)
			{
				return;
			}
			CollisionMask collisionMask = AreaUtils.GetCollisionMask(areaGeometryData);
			if ((m_CollisionMask & collisionMask) == 0)
			{
				return;
			}
			ErrorType errorType = ((areaGeometryData.m_Type != AreaType.MapTile) ? ErrorType.OverlapExisting : ErrorType.ExceedsCityLimits);
			DynamicBuffer<Game.Areas.Node> nodes = m_Data.m_AreaNodes[areaItem2.m_Area];
			Triangle triangle = m_Data.m_AreaTriangles[areaItem2.m_Area][areaItem2.m_Triangle];
			Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
			ErrorData errorData = default(ErrorData);
			if (areaGeometryData.m_Type != AreaType.MapTile && ((m_CollisionMask & CollisionMask.OnGround) == 0 || MathUtils.Intersect(bounds.m_Bounds, m_ObjectBounds)))
			{
				Bounds1 heightRange = triangle.m_HeightRange;
				heightRange.max += areaGeometryData.m_MaxHeight;
				float3 val = math.mul(math.inverse(m_TransformData.m_Rotation), m_TransformData.m_Position);
				Bounds3 bounds2 = m_PrefabObjectGeometryData.m_Bounds;
				if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
				{
					bounds2.min.y = math.max(bounds2.min.y, 0f);
				}
				if (ObjectUtils.GetStandingLegCount(m_PrefabObjectGeometryData, out var legCount))
				{
					Bounds3 val3 = default(Bounds3);
					Bounds3 val4 = default(Bounds3);
					for (int i = 0; i < legCount; i++)
					{
						float3 val2 = val + ObjectUtils.GetStandingLegOffset(m_PrefabObjectGeometryData, i);
						if ((m_PrefabObjectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
						{
							if (Game.Net.ValidationHelpers.TriangleCylinderIntersect(cylinder2: new Cylinder3
							{
								circle = new Circle2(m_PrefabObjectGeometryData.m_LegSize.x * 0.5f, ((float3)(ref val2)).xz),
								height = new Bounds1(bounds2.min.y, m_PrefabObjectGeometryData.m_LegSize.y) + val2.y,
								rotation = m_TransformData.m_Rotation
							}, triangle1: triangle2, intersection1: out var intersection, intersection2: out var intersection2))
							{
								intersection = Game.Net.ValidationHelpers.SetHeightRange(intersection, heightRange);
								if (MathUtils.Intersect(intersection2, intersection, ref val3))
								{
									errorData.m_Position = MathUtils.Center(val3);
									errorData.m_ErrorType = errorType;
								}
							}
						}
						else
						{
							if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) != GeometryFlags.None)
							{
								continue;
							}
							float3 standingLegPosition = ObjectUtils.GetStandingLegPosition(m_PrefabObjectGeometryData, m_TransformData, i);
							((float3)(ref bounds2.min)).xz = ((float3)(ref m_PrefabObjectGeometryData.m_LegSize)).xz * -0.5f;
							((float3)(ref bounds2.max)).xz = ((float3)(ref m_PrefabObjectGeometryData.m_LegSize)).xz * 0.5f;
							if (Game.Net.ValidationHelpers.QuadTriangleIntersect(ObjectUtils.CalculateBaseCorners(standingLegPosition, m_TransformData.m_Rotation, bounds2), triangle2, out var intersection3, out var intersection4))
							{
								intersection3 = Game.Net.ValidationHelpers.SetHeightRange(intersection3, ((Bounds3)(ref bounds2)).y);
								intersection4 = Game.Net.ValidationHelpers.SetHeightRange(intersection4, heightRange);
								if (MathUtils.Intersect(intersection3, intersection4, ref val4))
								{
									errorData.m_Position = MathUtils.Center(val4);
									errorData.m_ErrorType = errorType;
								}
							}
						}
					}
					bounds2.min.y = m_PrefabObjectGeometryData.m_LegSize.y;
				}
				Bounds3 intersection7;
				Bounds3 intersection8;
				if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
				{
					if (Game.Net.ValidationHelpers.TriangleCylinderIntersect(cylinder2: new Cylinder3
					{
						circle = new Circle2(m_PrefabObjectGeometryData.m_Size.x * 0.5f, ((float3)(ref val)).xz),
						height = new Bounds1(bounds2.min.y, bounds2.max.y) + val.y,
						rotation = m_TransformData.m_Rotation
					}, triangle1: triangle2, intersection1: out var intersection5, intersection2: out var intersection6))
					{
						intersection5 = Game.Net.ValidationHelpers.SetHeightRange(intersection5, heightRange);
						Bounds3 val5 = default(Bounds3);
						if (MathUtils.Intersect(intersection6, intersection5, ref val5))
						{
							errorData.m_Position = MathUtils.Center(val5);
							errorData.m_ErrorType = errorType;
						}
					}
				}
				else if (Game.Net.ValidationHelpers.QuadTriangleIntersect(ObjectUtils.CalculateBaseCorners(m_TransformData.m_Position, m_TransformData.m_Rotation, m_PrefabObjectGeometryData.m_Bounds), triangle2, out intersection7, out intersection8))
				{
					intersection7 = Game.Net.ValidationHelpers.SetHeightRange(intersection7, ((Bounds3)(ref bounds2)).y);
					intersection8 = Game.Net.ValidationHelpers.SetHeightRange(intersection8, heightRange);
					Bounds3 val6 = default(Bounds3);
					if (MathUtils.Intersect(intersection7, intersection8, ref val6))
					{
						errorData.m_Position = MathUtils.Center(val6);
						errorData.m_ErrorType = errorType;
					}
				}
			}
			if (areaGeometryData.m_Type == AreaType.MapTile || (errorData.m_ErrorType == ErrorType.None && CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask)))
			{
				Quad3 val8;
				if (areaGeometryData.m_Type != AreaType.MapTile && ObjectUtils.GetStandingLegCount(m_PrefabObjectGeometryData, out var legCount2))
				{
					Circle2 val7 = default(Circle2);
					for (int j = 0; j < legCount2; j++)
					{
						float3 standingLegPosition2 = ObjectUtils.GetStandingLegPosition(m_PrefabObjectGeometryData, m_TransformData, j);
						if ((m_PrefabObjectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
						{
							((Circle2)(ref val7))._002Ector(m_PrefabObjectGeometryData.m_LegSize.x * 0.5f, ((float3)(ref standingLegPosition2)).xz);
							if (MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, val7))
							{
								errorData.m_Position = MathUtils.Center(m_ObjectBounds & bounds.m_Bounds);
								errorData.m_ErrorType = errorType;
							}
						}
						else if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
						{
							Bounds3 bounds3 = default(Bounds3);
							((float3)(ref bounds3.min)).xz = ((float3)(ref m_PrefabObjectGeometryData.m_LegSize)).xz * -0.5f;
							((float3)(ref bounds3.max)).xz = ((float3)(ref m_PrefabObjectGeometryData.m_LegSize)).xz * 0.5f;
							val8 = ObjectUtils.CalculateBaseCorners(standingLegPosition2, m_TransformData.m_Rotation, bounds3);
							if (MathUtils.Intersect(((Quad3)(ref val8)).xz, ((Triangle3)(ref triangle2)).xz))
							{
								errorData.m_Position = MathUtils.Center(m_ObjectBounds & bounds.m_Bounds);
								errorData.m_ErrorType = errorType;
							}
						}
					}
				}
				else if ((m_PrefabObjectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
				{
					Circle2 val9 = default(Circle2);
					((Circle2)(ref val9))._002Ector(m_PrefabObjectGeometryData.m_Size.x * 0.5f, ((float3)(ref m_TransformData.m_Position)).xz);
					if (MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, val9))
					{
						errorData.m_Position = MathUtils.Center(m_ObjectBounds & bounds.m_Bounds);
						errorData.m_ErrorType = errorType;
					}
				}
				else
				{
					val8 = ObjectUtils.CalculateBaseCorners(m_TransformData.m_Position, m_TransformData.m_Rotation, m_PrefabObjectGeometryData.m_Bounds);
					if (MathUtils.Intersect(((Quad3)(ref val8)).xz, ((Triangle3)(ref triangle2)).xz))
					{
						errorData.m_Position = MathUtils.Center(m_ObjectBounds & bounds.m_Bounds);
						errorData.m_ErrorType = errorType;
					}
				}
			}
			if (errorData.m_ErrorType != ErrorType.None)
			{
				errorData.m_Position.y = MathUtils.Clamp(errorData.m_Position.y, ((Bounds3)(ref m_ObjectBounds)).y);
				if (m_Optional && errorType == ErrorType.OverlapExisting)
				{
					errorData.m_ErrorSeverity = ErrorSeverity.Override;
					errorData.m_TempEntity = m_ObjectEntity;
				}
				else
				{
					errorData.m_ErrorSeverity = ErrorSeverity.Error;
					errorData.m_TempEntity = m_ObjectEntity;
					errorData.m_PermanentEntity = areaItem2.m_Area;
				}
				m_ErrorQueue.Enqueue(errorData);
			}
		}
	}

	public const float COLLISION_TOLERANCE = 0.01f;

	public static void ValidateObject(Entity entity, Temp temp, Owner owner, Transform transform, PrefabRef prefabRef, Attached attached, bool isOutsideConnection, bool editorMode, ValidationSystem.EntityData data, NativeList<ValidationSystem.BoundsData> edgeList, NativeList<ValidationSystem.BoundsData> objectList, NativeQuadTree<Entity, QuadTreeBoundsXZ> objectSearchTree, NativeQuadTree<Entity, QuadTreeBoundsXZ> netSearchTree, NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> areaSearchTree, NativeParallelHashMap<Entity, int> instanceCounts, WaterSurfaceData waterSurfaceData, TerrainHeightData terrainHeightData, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_1124: Unknown result type (might be due to invalid IL or missing references)
		//IL_1129: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1107: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_1156: Unknown result type (might be due to invalid IL or missing references)
		//IL_1158: Unknown result type (might be due to invalid IL or missing references)
		//IL_115d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1135: Unknown result type (might be due to invalid IL or missing references)
		//IL_113d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dda: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e40: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08de: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_081d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e69: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0903: Unknown result type (might be due to invalid IL or missing references)
		//IL_0908: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		//IL_0836: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0929: Unknown result type (might be due to invalid IL or missing references)
		//IL_092e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec3: Unknown result type (might be due to invalid IL or missing references)
		//IL_093c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0941: Unknown result type (might be due to invalid IL or missing references)
		//IL_0945: Unknown result type (might be due to invalid IL or missing references)
		//IL_094a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Unknown result type (might be due to invalid IL or missing references)
		//IL_087d: Unknown result type (might be due to invalid IL or missing references)
		//IL_087f: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0870: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f10: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0975: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_09db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0980: Unknown result type (might be due to invalid IL or missing references)
		//IL_0985: Unknown result type (might be due to invalid IL or missing references)
		//IL_098e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0997: Unknown result type (might be due to invalid IL or missing references)
		//IL_0999: Unknown result type (might be due to invalid IL or missing references)
		//IL_104c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09be: Unknown result type (might be due to invalid IL or missing references)
		//IL_1068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_108e: Unknown result type (might be due to invalid IL or missing references)
		//IL_109c: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1078: Unknown result type (might be due to invalid IL or missing references)
		//IL_107d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1002: Unknown result type (might be due to invalid IL or missing references)
		//IL_1007: Unknown result type (might be due to invalid IL or missing references)
		//IL_1026: Unknown result type (might be due to invalid IL or missing references)
		//IL_102b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0add: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1039: Unknown result type (might be due to invalid IL or missing references)
		//IL_103e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8c: Unknown result type (might be due to invalid IL or missing references)
		ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
		if (!data.m_PrefabObjectGeometry.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData) || ((objectGeometryData.m_Flags & GeometryFlags.IgnoreSecondaryCollision) != GeometryFlags.None && data.m_Secondary.HasComponent(entity)))
		{
			return;
		}
		StackData stackData = default(StackData);
		Stack stack = default(Stack);
		Bounds3 val = ((!data.m_Stack.TryGetComponent(entity, ref stack) || !data.m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData)) ? ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, objectGeometryData) : ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, stack, objectGeometryData, stackData));
		PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
		data.m_PlaceableObject.TryGetComponent(prefabRef.m_Prefab, ref placeableObjectData);
		bool flag = false;
		if ((objectGeometryData.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) == GeometryFlags.Overridable)
		{
			flag = (temp.m_Flags & TempFlags.Essential) == 0;
		}
		Elevation elevation = default(Elevation);
		CollisionMask collisionMask;
		bool flag2;
		if (data.m_ObjectElevation.TryGetComponent(entity, ref elevation))
		{
			collisionMask = ObjectUtils.GetCollisionMask(objectGeometryData, elevation, !editorMode || owner.m_Owner != Entity.Null);
			flag2 = (elevation.m_Flags & ElevationFlags.OnGround) != 0 && flag;
			Owner owner2 = owner;
			ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
			Temp temp2 = default(Temp);
			while (flag && !flag2 && owner2.m_Owner != Entity.Null)
			{
				PrefabRef prefabRef2 = data.m_PrefabRef[owner2.m_Owner];
				if (!data.m_PrefabObjectGeometry.TryGetComponent(prefabRef2.m_Prefab, ref objectGeometryData2) || (objectGeometryData2.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) != GeometryFlags.Overridable || !data.m_Temp.TryGetComponent(owner2.m_Owner, ref temp2) || (temp2.m_Flags & TempFlags.Essential) != 0)
				{
					break;
				}
				if (!data.m_ObjectElevation.TryGetComponent(owner2.m_Owner, ref elevation) || (elevation.m_Flags & ElevationFlags.OnGround) != 0)
				{
					flag2 = true;
					break;
				}
				if (!data.m_Owner.TryGetComponent(owner2.m_Owner, ref owner2))
				{
					break;
				}
			}
		}
		else
		{
			collisionMask = ObjectUtils.GetCollisionMask(objectGeometryData, !editorMode || owner.m_Owner != Entity.Null);
			flag2 = flag;
		}
		Entity val2 = Entity.Null;
		Entity ignoreNode = Entity.Null;
		if ((placeableObjectData.m_Flags & PlacementFlags.RoadNode) != PlacementFlags.None)
		{
			if (data.m_Node.HasComponent(attached.m_Parent))
			{
				val2 = attached.m_Parent;
			}
			if (data.m_Temp.HasComponent(attached.m_Parent))
			{
				Entity original = data.m_Temp[attached.m_Parent].m_Original;
				if (data.m_Node.HasComponent(original))
				{
					ignoreNode = original;
				}
			}
			else
			{
				ignoreNode = val2;
				val2 = Entity.Null;
			}
		}
		if (temp.m_Original == Entity.Null && (placeableObjectData.m_Flags & PlacementFlags.Unique) != PlacementFlags.None && instanceCounts.ContainsKey(prefabRef.m_Prefab))
		{
			errorQueue.Enqueue(new ErrorData
			{
				m_ErrorType = ErrorType.AlreadyExists,
				m_ErrorSeverity = ErrorSeverity.Error,
				m_TempEntity = entity,
				m_Position = float3.op_Implicit(float.NaN)
			});
		}
		ObjectIterator objectIterator = default(ObjectIterator);
		Entity attachedParent = default(Entity);
		Edge tempNodes = default(Edge);
		Edge ownerNodes = default(Edge);
		Entity edgeOwner = default(Entity);
		Entity nodeOwner = default(Entity);
		if ((temp.m_Flags & TempFlags.Delete) == 0)
		{
			Entity assetStamp;
			Entity owner3 = GetOwner(entity, temp, data, out tempNodes, out ownerNodes, out attachedParent, out assetStamp, out edgeOwner, out nodeOwner);
			objectIterator = new ObjectIterator
			{
				m_ObjectEntity = entity,
				m_TopLevelEntity = owner3,
				m_AssetStampEntity = assetStamp,
				m_ObjectBounds = val,
				m_Transform = transform,
				m_ObjectStack = stack,
				m_CollisionMask = collisionMask,
				m_PrefabObjectGeometryData = objectGeometryData,
				m_ObjectStackData = stackData,
				m_CanOverride = flag,
				m_Optional = ((temp.m_Flags & TempFlags.Optional) != 0),
				m_EditorMode = editorMode,
				m_Data = data,
				m_ErrorQueue = errorQueue
			};
			objectSearchTree.Iterate<ObjectIterator>(ref objectIterator, 0);
		}
		NetIterator netIterator = default(NetIterator);
		if ((temp.m_Flags & TempFlags.Delete) == 0)
		{
			netIterator = new NetIterator
			{
				m_ObjectEntity = entity,
				m_AttachedParent = attachedParent,
				m_TopLevelEntity = objectIterator.m_TopLevelEntity,
				m_EdgeEntity = edgeOwner,
				m_NodeEntity = nodeOwner,
				m_IgnoreNode = ignoreNode,
				m_OwnerNodes = ownerNodes,
				m_ObjectBounds = val,
				m_Transform = transform,
				m_ObjectStack = stack,
				m_CollisionMask = collisionMask,
				m_PrefabObjectGeometryData = objectGeometryData,
				m_ObjectStackData = stackData,
				m_Optional = flag,
				m_EditorMode = editorMode,
				m_Data = data,
				m_ErrorQueue = errorQueue
			};
			netSearchTree.Iterate<NetIterator>(ref netIterator, 0);
		}
		AreaIterator areaIterator = new AreaIterator
		{
			m_ObjectEntity = entity,
			m_ObjectBounds = val,
			m_IgnoreCollisions = ((temp.m_Flags & TempFlags.Delete) != 0),
			m_IgnoreProtectedAreas = ((temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Modify | TempFlags.Replace | TempFlags.Upgrade)) == 0),
			m_Optional = flag,
			m_EditorMode = editorMode,
			m_TransformData = transform,
			m_CollisionMask = collisionMask,
			m_PrefabObjectGeometryData = objectGeometryData,
			m_Data = data,
			m_ErrorQueue = errorQueue
		};
		areaSearchTree.Iterate<AreaIterator>(ref areaIterator, 0);
		if ((temp.m_Flags & TempFlags.Delete) == 0 && (edgeList.Length != 0 || objectList.Length != 0))
		{
			Entity val3 = entity;
			Entity val4 = Entity.Null;
			Entity val5 = Entity.Null;
			attachedParent = Entity.Null;
			if (owner.m_Owner != Entity.Null && !data.m_Building.HasComponent(entity))
			{
				if (data.m_Node.HasComponent(owner.m_Owner))
				{
					val5 = owner.m_Owner;
				}
				if (data.m_AssetStamp.HasComponent(owner.m_Owner))
				{
					val4 = owner.m_Owner;
				}
				else
				{
					Attached attached2 = default(Attached);
					if (data.m_Attached.TryGetComponent(owner.m_Owner, ref attached2))
					{
						attachedParent = attached2.m_Parent;
					}
					val3 = owner.m_Owner;
					Owner owner4 = default(Owner);
					while (data.m_Owner.TryGetComponent(val3, ref owner4) && !data.m_Building.HasComponent(val3))
					{
						Entity owner5 = owner4.m_Owner;
						if (data.m_Node.HasComponent(owner5))
						{
							val5 = owner5;
						}
						if (data.m_AssetStamp.HasComponent(owner5))
						{
							val4 = owner5;
							break;
						}
						if (data.m_Attached.TryGetComponent(owner4.m_Owner, ref attached2))
						{
							attachedParent = attached2.m_Parent;
						}
						val3 = owner5;
					}
				}
			}
			DynamicBuffer<ConnectedEdge> val6 = default(DynamicBuffer<ConnectedEdge>);
			DynamicBuffer<ConnectedNode> val7 = default(DynamicBuffer<ConnectedNode>);
			Edge edge = default(Edge);
			if (data.m_ConnectedEdges.HasBuffer(val3))
			{
				val6 = data.m_ConnectedEdges[val3];
			}
			else if (data.m_ConnectedNodes.HasBuffer(val3))
			{
				val7 = data.m_ConnectedNodes[val3];
				edge = data.m_Edge[val3];
			}
			bool flag3 = false;
			NetObjectData netObjectData = default(NetObjectData);
			if ((placeableObjectData.m_Flags & PlacementFlags.RoadNode) != PlacementFlags.None && data.m_PrefabNetObject.TryGetComponent(prefabRef.m_Prefab, ref netObjectData) && (netObjectData.m_CompositionFlags.m_General & CompositionFlags.General.FixedNodeSize) != 0)
			{
				flag3 = true;
			}
			netIterator.m_TopLevelEntity = val3;
			if (edgeList.Length != 0)
			{
				float3 val8 = edgeList[edgeList.Length - 1].m_Bounds.max - edgeList[0].m_Bounds.min;
				bool flag4 = val8.z > val8.x;
				Owner owner6 = default(Owner);
				Edge edge2 = default(Edge);
				Owner owner8 = default(Owner);
				Edge edge5 = default(Edge);
				Owner owner10 = default(Owner);
				Edge edge6 = default(Edge);
				Owner owner13 = default(Owner);
				for (int i = 0; i < edgeList.Length; i++)
				{
					ValidationSystem.BoundsData boundsData = edgeList[i];
					bool2 val9 = ((float3)(ref boundsData.m_Bounds.min)).xz > ((float3)(ref val.max)).xz;
					if (flag4 ? val9.y : val9.x)
					{
						break;
					}
					if ((collisionMask & CollisionMask.OnGround) != 0)
					{
						if (!MathUtils.Intersect(((Bounds3)(ref val)).xz, ((Bounds3)(ref boundsData.m_Bounds)).xz))
						{
							continue;
						}
					}
					else if (!MathUtils.Intersect(val, boundsData.m_Bounds))
					{
						continue;
					}
					Entity val10 = boundsData.m_Entity;
					Entity owner7;
					if (data.m_Owner.TryGetComponent(boundsData.m_Entity, ref owner6))
					{
						owner7 = owner6.m_Owner;
						if (data.m_AssetStamp.HasComponent(owner7))
						{
							if (owner7 == entity)
							{
								continue;
							}
						}
						else
						{
							val10 = owner7;
							while (data.m_Owner.HasComponent(val10) && !data.m_Building.HasComponent(val10))
							{
								owner7 = data.m_Owner[val10].m_Owner;
								if (!data.m_AssetStamp.HasComponent(owner7))
								{
									val10 = owner7;
									continue;
								}
								goto IL_086e;
							}
						}
						goto IL_08a1;
					}
					goto IL_08de;
					IL_086e:
					if (owner7 == entity)
					{
						continue;
					}
					goto IL_08a1;
					IL_08a1:
					if (data.m_Edge.TryGetComponent(owner6.m_Owner, ref edge2) && (val5 == edge2.m_Start || val5 == edge2.m_End))
					{
						continue;
					}
					goto IL_08de;
					IL_08de:
					if (val3 == val10)
					{
						continue;
					}
					Edge edgeData = data.m_Edge[boundsData.m_Entity];
					if (boundsData.m_Entity == attachedParent || edgeData.m_Start == attachedParent || edgeData.m_End == attachedParent)
					{
						continue;
					}
					Entity val11 = edgeData.m_Start;
					Entity val12 = edgeData.m_End;
					Edge edge3 = default(Edge);
					Edge edge4 = default(Edge);
					while (true)
					{
						if (data.m_Owner.TryGetComponent(val11, ref owner8) && !data.m_Building.HasComponent(val11))
						{
							Entity owner9 = owner8.m_Owner;
							if (!data.m_AssetStamp.HasComponent(owner9))
							{
								if (data.m_Edge.TryGetComponent(owner9, ref edge5))
								{
									edge3 = edge5;
								}
								val11 = owner9;
								continue;
							}
							if (owner9 == entity)
							{
								break;
							}
						}
						while (true)
						{
							if (data.m_Owner.TryGetComponent(val12, ref owner10) && !data.m_Building.HasComponent(val12))
							{
								Entity owner11 = owner10.m_Owner;
								if (!data.m_AssetStamp.HasComponent(owner11))
								{
									if (data.m_Edge.TryGetComponent(owner11, ref edge6))
									{
										edge4 = edge6;
									}
									val12 = owner11;
									continue;
								}
								if (owner11 == entity)
								{
									break;
								}
							}
							Composition compositionData = data.m_Composition[boundsData.m_Entity];
							if (flag3)
							{
								if (owner.m_Owner == edgeData.m_Start && (data.m_PrefabComposition[compositionData.m_StartNode].m_Flags.m_General & CompositionFlags.General.FixedNodeSize) == 0)
								{
									edgeData.m_Start = boundsData.m_Entity;
									val11 = boundsData.m_Entity;
								}
								if (owner.m_Owner == edgeData.m_End && (data.m_PrefabComposition[compositionData.m_EndNode].m_Flags.m_General & CompositionFlags.General.FixedNodeSize) == 0)
								{
									edgeData.m_End = boundsData.m_Entity;
									val12 = boundsData.m_Entity;
								}
							}
							if (owner.m_Owner != Entity.Null)
							{
								Entity owner12 = owner.m_Owner;
								while (!(owner12 == edgeData.m_Start) && !(owner12 == edgeData.m_End) && !(owner12 == edge3.m_Start) && !(owner12 == edge3.m_End) && !(owner12 == edge4.m_Start) && !(owner12 == edge4.m_End))
								{
									if (data.m_Owner.TryGetComponent(owner12, ref owner13))
									{
										owner12 = owner13.m_Owner;
										continue;
									}
									goto IL_0b93;
								}
								break;
							}
							goto IL_0b93;
							IL_0b93:
							EdgeGeometry edgeGeometryData = data.m_EdgeGeometry[boundsData.m_Entity];
							StartNodeGeometry startNodeGeometryData = data.m_StartNodeGeometry[boundsData.m_Entity];
							EndNodeGeometry endNodeGeometryData = data.m_EndNodeGeometry[boundsData.m_Entity];
							bool flag5 = val11 != val3 && edgeData.m_Start != tempNodes.m_Start && edgeData.m_Start != tempNodes.m_End;
							bool flag6 = val12 != val3 && edgeData.m_End != tempNodes.m_Start && edgeData.m_End != tempNodes.m_End;
							if (flag5 && edgeData.m_Start == val2)
							{
								flag5 &= (data.m_PrefabComposition[compositionData.m_StartNode].m_Flags.m_General & CompositionFlags.General.Roundabout) == 0;
							}
							if (flag6 && edgeData.m_End == val2)
							{
								flag6 &= (data.m_PrefabComposition[compositionData.m_EndNode].m_Flags.m_General & CompositionFlags.General.Roundabout) == 0;
							}
							edgeData.m_Start = val11;
							edgeData.m_End = val12;
							Temp temp3 = data.m_Temp[boundsData.m_Entity];
							netIterator.CheckOverlap(val10, boundsData.m_Entity, boundsData.m_Bounds, edgeData, compositionData, edgeGeometryData, startNodeGeometryData, endNodeGeometryData, transform.m_Position, flag5, flag6, (temp3.m_Flags & TempFlags.Essential) != 0, owner6.m_Owner != Entity.Null);
							break;
						}
						break;
					}
				}
			}
			if (objectList.Length != 0)
			{
				float3 val13 = objectList[objectList.Length - 1].m_Bounds.max - objectList[0].m_Bounds.min;
				bool flag7 = val13.z > val13.x;
				int num = 0;
				int num2 = objectList.Length;
				while (num < num2)
				{
					int num3 = num + num2 >> 1;
					ValidationSystem.BoundsData boundsData2 = objectList[num3];
					bool2 val14 = ((float3)(ref boundsData2.m_Bounds.min)).xz < ((float3)(ref val.min)).xz;
					if (flag7 ? val14.y : val14.x)
					{
						num = num3 + 1;
					}
					else
					{
						num2 = num3;
					}
				}
				Owner owner14 = default(Owner);
				Attached attached3 = default(Attached);
				for (int j = num; j < objectList.Length; j++)
				{
					ValidationSystem.BoundsData boundsData3 = objectList[j];
					bool2 val15 = ((float3)(ref boundsData3.m_Bounds.min)).xz > ((float3)(ref val.max)).xz;
					if (flag7 ? val15.y : val15.x)
					{
						break;
					}
					if ((collisionMask & CollisionMask.OnGround) != 0)
					{
						if (!MathUtils.Intersect(((Bounds3)(ref val)).xz, ((Bounds3)(ref boundsData3.m_Bounds)).xz))
						{
							continue;
						}
					}
					else if (!MathUtils.Intersect(val, boundsData3.m_Bounds))
					{
						continue;
					}
					if (boundsData3.m_Entity == entity || boundsData3.m_Entity == val4 || (boundsData3.m_Bounds.min.x == val.min.x && boundsData3.m_Entity.Index < entity.Index))
					{
						continue;
					}
					Entity val16 = boundsData3.m_Entity;
					Entity owner15;
					if (data.m_Owner.TryGetComponent(boundsData3.m_Entity, ref owner14) && !data.m_Building.HasComponent(val16))
					{
						owner15 = owner14.m_Owner;
						if (data.m_AssetStamp.HasComponent(owner15))
						{
							if (owner15 == entity)
							{
								continue;
							}
						}
						else
						{
							val16 = owner15;
							while (data.m_Owner.HasComponent(val16) && !data.m_Building.HasComponent(val16))
							{
								owner15 = data.m_Owner[val16].m_Owner;
								if (!data.m_AssetStamp.HasComponent(owner15))
								{
									val16 = owner15;
									continue;
								}
								goto IL_0f6f;
							}
						}
					}
					goto IL_0fa2;
					IL_0fa2:
					if (val3 == val16)
					{
						continue;
					}
					if (val6.IsCreated)
					{
						int num4 = 0;
						while (num4 < val6.Length)
						{
							if (!(val6[num4].m_Edge == val16))
							{
								num4++;
								continue;
							}
							goto IL_10ce;
						}
					}
					else if (val7.IsCreated)
					{
						int num5 = 0;
						while (num5 < val7.Length)
						{
							if (!(val7[num5].m_Node == val16))
							{
								num5++;
								continue;
							}
							goto IL_10ce;
						}
						if (edge.m_Start == val16 || edge.m_End == val16)
						{
							continue;
						}
					}
					if (!(attached.m_Parent == boundsData3.m_Entity) && (!data.m_Attached.TryGetComponent(boundsData3.m_Entity, ref attached3) || !(attached3.m_Parent == entity)))
					{
						Temp temp4 = data.m_Temp[boundsData3.m_Entity];
						objectIterator.CheckOverlap(val16, boundsData3.m_Entity, boundsData3.m_Bounds, (temp4.m_Flags & TempFlags.Essential) != 0, owner14.m_Owner != Entity.Null);
					}
					continue;
					IL_0f6f:
					if (owner15 == entity)
					{
						continue;
					}
					goto IL_0fa2;
					IL_10ce:;
				}
			}
		}
		if ((temp.m_Flags & (TempFlags.Create | TempFlags.Modify)) != 0 && placeableObjectData.m_Flags != PlacementFlags.None && !flag2)
		{
			CheckSurface(entity, transform, collisionMask, objectGeometryData, placeableObjectData, data, waterSurfaceData, terrainHeightData, errorQueue);
		}
		if ((temp.m_Flags & TempFlags.Essential) != 0 && (temp.m_Flags & (TempFlags.Create | TempFlags.Modify)) != 0 && owner.m_Owner != Entity.Null)
		{
			ValidateSubPlacement(entity, owner, transform, prefabRef, objectGeometryData, data, errorQueue);
		}
		if ((temp.m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Replace | TempFlags.Upgrade)) != 0 && !isOutsideConnection)
		{
			ValidateWorldBounds(entity, owner, val, data, terrainHeightData, errorQueue);
		}
	}

	public static void ValidateWorldBounds(Entity entity, Owner owner, Bounds3 bounds, ValidationSystem.EntityData data, TerrainHeightData terrainHeightData, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = MathUtils.Expand(TerrainUtils.GetBounds(ref terrainHeightData), float3.op_Implicit(0.1f));
		Bounds2 xz = ((Bounds3)(ref bounds)).xz;
		if (((Bounds2)(ref xz)).Equals(((Bounds3)(ref bounds)).xz & ((Bounds3)(ref val)).xz))
		{
			return;
		}
		Owner owner2 = default(Owner);
		while (owner.m_Owner != Entity.Null)
		{
			if (data.m_Node.HasComponent(owner.m_Owner) || data.m_Edge.HasComponent(owner.m_Owner))
			{
				return;
			}
			data.m_Owner.TryGetComponent(owner.m_Owner, ref owner2);
			owner = owner2;
		}
		Bounds3 val2 = bounds;
		((float3)(ref val2.min)).xz = math.select(((float3)(ref bounds.min)).xz, ((float3)(ref val.max)).xz, (((float3)(ref val.max)).xz > ((float3)(ref bounds.min)).xz) & (((float3)(ref bounds.min)).xz >= ((float3)(ref val.min)).xz) & (((float3)(ref bounds.max)).xz > ((float3)(ref val.max)).xz));
		((float3)(ref val2.max)).xz = math.select(((float3)(ref bounds.max)).xz, ((float3)(ref val.min)).xz, (((float3)(ref val.min)).xz < ((float3)(ref bounds.max)).xz) & (((float3)(ref bounds.max)).xz <= ((float3)(ref val.max)).xz) & (((float3)(ref bounds.min)).xz < ((float3)(ref val.min)).xz));
		errorQueue.Enqueue(new ErrorData
		{
			m_Position = MathUtils.Center(val2),
			m_ErrorType = ErrorType.ExceedsCityLimits,
			m_ErrorSeverity = ErrorSeverity.Error,
			m_TempEntity = entity
		});
	}

	public static void ValidateSubPlacement(Entity entity, Owner owner, Transform transform, PrefabRef prefabRef, ObjectGeometryData prefabObjectGeometryData, ValidationSystem.EntityData data, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		if (!data.m_Building.HasComponent(owner.m_Owner))
		{
			return;
		}
		Transform transform2 = data.m_Transform[owner.m_Owner];
		PrefabRef prefabRef2 = data.m_PrefabRef[owner.m_Owner];
		BuildingData ownerBuildingData = data.m_PrefabBuilding[prefabRef2.m_Prefab];
		if (data.m_Building.HasComponent(entity))
		{
			BuildingData buildingData = default(BuildingData);
			ServiceUpgradeData serviceUpgradeData = default(ServiceUpgradeData);
			if (data.m_PrefabBuilding.TryGetComponent(prefabRef.m_Prefab, ref buildingData) && data.m_ServiceUpgradeData.TryGetComponent(prefabRef.m_Prefab, ref serviceUpgradeData) && serviceUpgradeData.m_MaxPlacementDistance != 0f)
			{
				BuildingUtils.CalculateUpgradeRangeValues(transform2.m_Rotation, ownerBuildingData, buildingData, serviceUpgradeData, out var forward, out var width, out var length, out var roundness, out var circular);
				float2 halfLotSize = float2.op_Implicit(buildingData.m_LotSize) * 4f - 0.4f;
				Quad3 val = BuildingUtils.CalculateCorners(transform.m_Position, transform.m_Rotation, halfLotSize);
				float4 val2 = default(float4);
				if (ExceedRange(transform2.m_Position, forward, width, length, roundness, circular, ((float3)(ref val.a)).xz))
				{
					val2 += new float4(val.a, 1f);
				}
				if (ExceedRange(transform2.m_Position, forward, width, length, roundness, circular, ((float3)(ref val.b)).xz))
				{
					val2 += new float4(val.b, 1f);
				}
				if (ExceedRange(transform2.m_Position, forward, width, length, roundness, circular, ((float3)(ref val.c)).xz))
				{
					val2 += new float4(val.c, 1f);
				}
				if (ExceedRange(transform2.m_Position, forward, width, length, roundness, circular, ((float3)(ref val.d)).xz))
				{
					val2 += new float4(val.d, 1f);
				}
				if (val2.w != 0f)
				{
					errorQueue.Enqueue(new ErrorData
					{
						m_ErrorType = ErrorType.LongDistance,
						m_ErrorSeverity = ErrorSeverity.Error,
						m_TempEntity = entity,
						m_PermanentEntity = owner.m_Owner,
						m_Position = ((float4)(ref val2)).xyz / val2.w
					});
				}
			}
		}
		else
		{
			float2 val3 = float2.op_Implicit(ownerBuildingData.m_LotSize);
			val3 *= 4f;
			Bounds2 val4 = default(Bounds2);
			((Bounds2)(ref val4))._002Ector(-val3, val3);
			Transform transform3 = ObjectUtils.WorldToLocal(ObjectUtils.InverseTransform(transform2), transform);
			Bounds3 val5 = ObjectUtils.CalculateBounds(transform3.m_Position, transform3.m_Rotation, prefabObjectGeometryData);
			Bounds2 xz = ((Bounds3)(ref val5)).xz;
			if (!((Bounds2)(ref xz)).Equals(((Bounds3)(ref val5)).xz & val4))
			{
				float3 position = default(float3);
				((float3)(ref position)).xz = (math.select(((float3)(ref val5.min)).xz, val4.max, (((float3)(ref val5.min)).xz >= val4.min) & (((float3)(ref val5.max)).xz > val4.max)) + math.select(((float3)(ref val5.max)).xz, val4.min, (((float3)(ref val5.max)).xz <= val4.max) & (((float3)(ref val5.min)).xz < val4.min))) * 0.5f;
				position.y = MathUtils.Center(((Bounds3)(ref val5)).y);
				errorQueue.Enqueue(new ErrorData
				{
					m_ErrorType = ErrorType.ExceedsLotLimits,
					m_ErrorSeverity = ErrorSeverity.Warning,
					m_TempEntity = entity,
					m_Position = ObjectUtils.LocalToWorld(transform2, position)
				});
			}
		}
	}

	private static bool ExceedRange(float3 position, float3 forward, float width, float length, float roundness, bool circular, float2 checkPosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		float2 val = checkPosition - ((float3)(ref position)).xz;
		if (!circular)
		{
			roundness -= 8f;
			val = math.abs(new float2(math.dot(val, MathUtils.Right(((float3)(ref forward)).xz)), math.dot(val, ((float3)(ref forward)).xz)));
			val = math.max(float2.op_Implicit(0f), val - new float2(width * 0.5f, length * 0.5f) + roundness);
		}
		return math.length(val) > roundness;
	}

	public static void ValidateNetObject(Entity entity, NetObject netObject, Transform transform, PrefabRef prefabRef, Attached attached, ValidationSystem.EntityData data, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		RoadTypes roadTypes = RoadTypes.None;
		NetObjectData netObjectData = default(NetObjectData);
		if (data.m_PrefabNetObject.TryGetComponent(prefabRef.m_Prefab, ref netObjectData) && netObjectData.m_RequireRoad != RoadTypes.None)
		{
			roadTypes = netObjectData.m_RequireRoad;
			DynamicBuffer<Game.Net.SubLane> val = default(DynamicBuffer<Game.Net.SubLane>);
			if (data.m_Lanes.TryGetBuffer(attached.m_Parent, ref val))
			{
				CarLaneData carLaneData = default(CarLaneData);
				for (int i = 0; i < val.Length; i++)
				{
					Game.Net.SubLane subLane = val[i];
					if (!data.m_CarLane.HasComponent(subLane.m_SubLane))
					{
						continue;
					}
					PrefabRef prefabRef2 = data.m_PrefabRef[subLane.m_SubLane];
					if (data.m_CarLaneData.TryGetComponent(prefabRef2.m_Prefab, ref carLaneData))
					{
						roadTypes = (RoadTypes)((uint)roadTypes & (uint)(byte)(~(int)carLaneData.m_RoadTypes));
						if (roadTypes == RoadTypes.None)
						{
							break;
						}
					}
				}
			}
			if (roadTypes == RoadTypes.Watercraft && netObjectData.m_RequireRoad == (RoadTypes.Car | RoadTypes.Watercraft))
			{
				roadTypes = RoadTypes.None;
			}
		}
		if (roadTypes != RoadTypes.None)
		{
			errorQueue.Enqueue(new ErrorData
			{
				m_ErrorType = ((roadTypes == (RoadTypes.Car | RoadTypes.Watercraft)) ? ErrorType.NoPortAccess : ErrorType.NoRoadAccess),
				m_ErrorSeverity = ErrorSeverity.Error,
				m_TempEntity = entity,
				m_Position = transform.m_Position
			});
		}
		else if ((netObject.m_Flags & (NetObjectFlags.IsClear | NetObjectFlags.TrackPassThrough)) == 0)
		{
			errorQueue.Enqueue(new ErrorData
			{
				m_ErrorType = ErrorType.OverlapExisting,
				m_ErrorSeverity = ErrorSeverity.Error,
				m_TempEntity = entity,
				m_Position = transform.m_Position
			});
		}
	}

	public static void ValidateOutsideConnection(Entity entity, Transform transform, TerrainHeightData terrainHeightData, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = TerrainUtils.GetBounds(ref terrainHeightData);
		val = MathUtils.Expand(val, float3.op_Implicit(-0.1f));
		if (MathUtils.Intersect(((Bounds3)(ref val)).xz, ((float3)(ref transform.m_Position)).xz))
		{
			errorQueue.Enqueue(new ErrorData
			{
				m_ErrorType = ErrorType.NotOnBorder,
				m_ErrorSeverity = ErrorSeverity.Error,
				m_TempEntity = entity,
				m_Position = transform.m_Position
			});
		}
	}

	public static void ValidateWaterSource(Entity entity, Transform transform, Game.Simulation.WaterSourceData waterSourceData, TerrainHeightData terrainHeightData, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 bounds = TerrainUtils.GetBounds(ref terrainHeightData);
		Bounds3 val = MathUtils.Expand(bounds, float3.op_Implicit(0f - waterSourceData.m_Radius));
		Bounds3 val2 = MathUtils.Expand(bounds, float3.op_Implicit(waterSourceData.m_Radius));
		if (waterSourceData.m_ConstantDepth < 2)
		{
			if (!MathUtils.Intersect(((Bounds3)(ref val)).xz, ((float3)(ref transform.m_Position)).xz))
			{
				errorQueue.Enqueue(new ErrorData
				{
					m_ErrorType = ErrorType.ExceedsCityLimits,
					m_ErrorSeverity = ErrorSeverity.Error,
					m_TempEntity = entity,
					m_Position = transform.m_Position
				});
			}
		}
		else if (!MathUtils.Intersect(((Bounds3)(ref val2)).xz, ((float3)(ref transform.m_Position)).xz) || MathUtils.Intersect(((Bounds3)(ref val)).xz, ((float3)(ref transform.m_Position)).xz))
		{
			errorQueue.Enqueue(new ErrorData
			{
				m_ErrorType = ErrorType.NotOnBorder,
				m_ErrorSeverity = ErrorSeverity.Error,
				m_TempEntity = entity,
				m_Position = transform.m_Position
			});
		}
	}

	private static Entity GetOwner(Entity entity, Temp temp, ValidationSystem.EntityData data, out Edge tempNodes, out Edge ownerNodes, out Entity attachedParent, out Entity assetStamp, out Entity edgeOwner, out Entity nodeOwner)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		tempNodes = default(Edge);
		ownerNodes = default(Edge);
		attachedParent = Entity.Null;
		assetStamp = Entity.Null;
		edgeOwner = Entity.Null;
		nodeOwner = Entity.Null;
		Owner owner = default(Owner);
		if (!data.m_Owner.TryGetComponent(entity, ref owner) || data.m_Building.HasComponent(entity))
		{
			entity = temp.m_Original;
		}
		else
		{
			Edge edge = default(Edge);
			Temp temp2 = default(Temp);
			Temp temp3 = default(Temp);
			Attached attached = default(Attached);
			do
			{
				if (data.m_AssetStamp.HasComponent(owner.m_Owner))
				{
					assetStamp = owner.m_Owner;
					break;
				}
				entity = owner.m_Owner;
				if (data.m_Edge.TryGetComponent(entity, ref edge))
				{
					edgeOwner = entity;
					ownerNodes = edge;
					if (data.m_Temp.TryGetComponent(edgeOwner, ref temp2))
					{
						edgeOwner = temp2.m_Original;
					}
					if (data.m_Temp.TryGetComponent(ownerNodes.m_Start, ref temp))
					{
						tempNodes.m_Start = ownerNodes.m_Start;
						ownerNodes.m_Start = temp.m_Original;
					}
					if (data.m_Temp.TryGetComponent(ownerNodes.m_End, ref temp))
					{
						tempNodes.m_End = ownerNodes.m_End;
						ownerNodes.m_End = temp.m_Original;
					}
				}
				else if (data.m_Node.HasComponent(entity))
				{
					nodeOwner = entity;
					if (data.m_Temp.TryGetComponent(nodeOwner, ref temp3))
					{
						nodeOwner = temp3.m_Original;
					}
				}
				if (data.m_Temp.TryGetComponent(entity, ref temp))
				{
					entity = temp.m_Original;
				}
				if (data.m_Attached.TryGetComponent(entity, ref attached))
				{
					attachedParent = attached.m_Parent;
				}
			}
			while (data.m_Owner.TryGetComponent(entity, ref owner) && !data.m_Building.HasComponent(entity));
		}
		return entity;
	}

	private static void CheckSurface(Entity entity, Transform transform, CollisionMask collisionMask, ObjectGeometryData prefabObjectGeometryData, PlaceableObjectData placeableObjectData, ValidationSystem.EntityData data, WaterSurfaceData waterSurfaceData, TerrainHeightData terrainHeightData, ParallelWriter<ErrorData> errorQueue)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		float sampleInterval = WaterUtils.GetSampleInterval(ref waterSurfaceData);
		int2 val = (int2)math.ceil((((float3)(ref prefabObjectGeometryData.m_Bounds.max)).xz - ((float3)(ref prefabObjectGeometryData.m_Bounds.min)).xz) / sampleInterval);
		Quad3 val2 = ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, prefabObjectGeometryData.m_Bounds);
		Bounds3 val3 = default(Bounds3);
		val3.min = float3.op_Implicit(float.MaxValue);
		val3.max = float3.op_Implicit(float.MinValue);
		Bounds3 val4 = default(Bounds3);
		val4.min = float3.op_Implicit(float.MaxValue);
		val4.max = float3.op_Implicit(float.MinValue);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < val.x; i++)
		{
			float num = ((float)i + 0.5f) / (float)val.x;
			float3 val5 = math.lerp(val2.a, val2.b, num);
			float3 val6 = math.lerp(val2.d, val2.c, num);
			if ((placeableObjectData.m_Flags & PlacementFlags.Shoreline) != PlacementFlags.None)
			{
				float num2 = WaterUtils.SampleDepth(ref waterSurfaceData, val5);
				float num3 = WaterUtils.SampleDepth(ref waterSurfaceData, val6);
				if (num2 >= 0.2f)
				{
					val3 |= val5;
					flag = (placeableObjectData.m_Flags & PlacementFlags.Floating) == 0;
				}
				if (num3 < 0.2f)
				{
					val4 |= val6;
					flag2 = true;
				}
			}
			else if ((placeableObjectData.m_Flags & (PlacementFlags.Floating | PlacementFlags.Underwater)) != PlacementFlags.None)
			{
				if ((placeableObjectData.m_Flags & PlacementFlags.OnGround) != PlacementFlags.None)
				{
					continue;
				}
				for (int j = 0; j < val.y; j++)
				{
					float num4 = ((float)j + 0.5f) / (float)val.y;
					float3 val7 = math.lerp(val5, val6, num4);
					if (WaterUtils.SampleDepth(ref waterSurfaceData, val7) < 0.2f)
					{
						val4 |= val7;
						flag2 = true;
					}
				}
			}
			else
			{
				if ((prefabObjectGeometryData.m_Flags & GeometryFlags.CanSubmerge) != GeometryFlags.None)
				{
					continue;
				}
				for (int k = 0; k < val.y; k++)
				{
					float num5 = ((float)k + 0.5f) / (float)val.y;
					float3 val8 = math.lerp(val5, val6, num5);
					float waterDepth;
					if ((collisionMask & CollisionMask.ExclusiveGround) != 0)
					{
						waterDepth = WaterUtils.SampleDepth(ref waterSurfaceData, val8);
					}
					else
					{
						float num6 = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, val8, out waterDepth);
						waterDepth = math.min(waterDepth, num6 - transform.m_Position.y);
					}
					if (waterDepth >= 0.2f)
					{
						val3 |= val8;
						flag = true;
					}
				}
			}
		}
		if (flag)
		{
			errorQueue.Enqueue(new ErrorData
			{
				m_ErrorType = ErrorType.InWater,
				m_ErrorSeverity = ErrorSeverity.Error,
				m_TempEntity = entity,
				m_Position = MathUtils.Center(val3)
			});
		}
		if (flag2)
		{
			ErrorData errorData = default(ErrorData);
			if ((placeableObjectData.m_Flags & (PlacementFlags.OnGround | PlacementFlags.Shoreline)) == (PlacementFlags.OnGround | PlacementFlags.Shoreline))
			{
				errorData.m_ErrorType = ErrorType.NotOnShoreline;
			}
			else
			{
				errorData.m_ErrorType = ErrorType.NoWater;
			}
			if ((placeableObjectData.m_Flags & PlacementFlags.OnGround) == 0)
			{
				errorData.m_ErrorSeverity = ErrorSeverity.Error;
			}
			else
			{
				errorData.m_ErrorSeverity = ErrorSeverity.Warning;
			}
			errorData.m_TempEntity = entity;
			errorData.m_Position = MathUtils.Center(val4);
			errorQueue.Enqueue(errorData);
		}
	}

	public static bool Intersect(Cylinder3 cylinder1, Cylinder3 cylinder2, ref float3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		quaternion val = math.mul(cylinder2.rotation, math.inverse(cylinder1.rotation));
		ref Circle2 circle = ref cylinder2.circle;
		float3 val2 = math.mul(val, new float3(cylinder2.circle.position.x, 0f, cylinder2.circle.position.y));
		circle.position = ((float3)(ref val2)).xz;
		cylinder2.height.min = math.mul(val, new float3(0f, cylinder2.height.min, 0f)).y;
		cylinder2.height.max = math.mul(val, new float3(0f, cylinder2.height.max, 0f)).y;
		float2 val3 = cylinder1.circle.position - cylinder2.circle.position;
		float num = cylinder1.circle.radius + cylinder2.circle.radius;
		if (math.lengthsq(val3) < num * num && MathUtils.Intersect(cylinder1.height, cylinder2.height))
		{
			MathUtils.TryNormalize(ref val3);
			float2 val4 = cylinder1.circle.position + val3 * cylinder1.circle.radius;
			float2 val5 = cylinder2.circle.position - val3 * cylinder2.circle.radius;
			pos.y = MathUtils.Center(cylinder1.height & cylinder2.height);
			((float3)(ref pos)).xz = math.lerp(val4, val5, 0.5f);
			pos = math.mul(cylinder1.rotation, pos);
			return true;
		}
		return false;
	}
}

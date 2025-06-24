using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Buildings;

[CompilerGenerated]
public class LotHeightSystem : GameSystemBase
{
	[BurstCompile]
	private struct AddUpdatedLotsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public NativeList<Entity> m_ResultList;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			m_ResultList.AddRange(nativeArray);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FindUpdatedLotsJob : IJobParallelForDefer
	{
		private struct Iterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<Lot> m_LotData;

			public ParallelWriter<Entity> m_ResultQueue;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) && m_LotData.HasComponent(entity))
				{
					m_ResultQueue.Enqueue(entity);
				}
			}
		}

		[ReadOnly]
		public NativeArray<Bounds2> m_Bounds;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		[ReadOnly]
		public ComponentLookup<Lot> m_LotData;

		public ParallelWriter<Entity> m_ResultQueue;

		public void Execute(int index)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			Iterator iterator = new Iterator
			{
				m_Bounds = MathUtils.Expand(m_Bounds[index], float2.op_Implicit(8f)),
				m_LotData = m_LotData,
				m_ResultQueue = m_ResultQueue
			};
			m_SearchTree.Iterate<Iterator>(ref iterator, 0);
		}
	}

	[BurstCompile]
	private struct CollectLotsJob : IJob
	{
		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeQueue<Entity> m_Queue1;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeQueue<Entity> m_Queue2;

		public NativeList<Entity> m_ResultList;

		public void Execute()
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			if (m_Queue1.IsCreated)
			{
				num += m_Queue1.Count;
			}
			if (m_Queue2.IsCreated)
			{
				num += m_Queue2.Count;
			}
			m_ResultList.Capacity = m_ResultList.Length + num;
			if (m_Queue1.IsCreated)
			{
				NativeArray<Entity> val = m_Queue1.ToArray(AllocatorHandle.op_Implicit((Allocator)2));
				m_ResultList.AddRange(val);
				val.Dispose();
			}
			if (m_Queue2.IsCreated)
			{
				NativeArray<Entity> val2 = m_Queue2.ToArray(AllocatorHandle.op_Implicit((Allocator)2));
				m_ResultList.AddRange(val2);
				val2.Dispose();
			}
			NativeSortExtension.Sort<Entity>(m_ResultList);
			Entity val3 = Entity.Null;
			int num2 = 0;
			int num3 = 0;
			while (num2 < m_ResultList.Length)
			{
				Entity val4 = m_ResultList[num2++];
				if (val4 != val3)
				{
					m_ResultList[num3++] = val4;
					val3 = val4;
				}
			}
			if (num3 < m_ResultList.Length)
			{
				m_ResultList.RemoveRange(num3, m_ResultList.Length - num3);
			}
		}
	}

	private struct Heights
	{
		public Bounds1 m_FlexibleBounds;

		public Bounds1 m_RigidBounds;

		public float m_FlexibleStrength;

		public float m_RigidStrength;

		public void Reset()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			m_FlexibleBounds = default(Bounds1);
			m_RigidBounds = default(Bounds1);
			m_FlexibleStrength = 0f;
			m_RigidStrength = 0f;
		}

		public float Center()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtils.Center(m_FlexibleBounds);
			if (m_RigidStrength != 0f)
			{
				float num2 = MathUtils.Center(m_RigidBounds);
				float num3 = math.min(m_FlexibleStrength, 1f - m_RigidStrength);
				num = (num * num3 + num2 * m_RigidStrength) / (num3 + m_RigidStrength);
			}
			return num;
		}
	}

	[BurstCompile]
	private struct UpdateLotHeightsJob : IJobParallelForDefer
	{
		private struct LotIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public float m_Radius;

			public float3 m_Position;

			public Quad3 m_Quad;

			public Entity m_Ignore;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Lot> m_LotData;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

			public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

			public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

			public ComponentLookup<Composition> m_CompositionData;

			public ComponentLookup<Orphan> m_OrphanData;

			public ComponentLookup<Node> m_NodeData;

			public ComponentLookup<Edge> m_EdgeData;

			public ComponentLookup<Game.Net.Elevation> m_ElevationData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<BuildingData> m_PrefabBuildingData;

			public ComponentLookup<BuildingExtensionData> m_PrefabBuildingExtensionData;

			public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

			public ComponentLookup<NetCompositionData> m_PrefabNetCompositionData;

			public ComponentLookup<BuildingTerraformData> m_PrefabBuildingTerraformData;

			public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

			public Heights m_Heights1;

			public Heights m_Heights2;

			public Heights m_Heights3;

			public Heights m_Heights4;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Quad3)(ref m_Quad)).xz);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0170: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0274: Unknown result type (might be due to invalid IL or missing references)
				//IL_0181: Unknown result type (might be due to invalid IL or missing references)
				//IL_0191: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0285: Unknown result type (might be due to invalid IL or missing references)
				//IL_0295: Unknown result type (might be due to invalid IL or missing references)
				//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_0204: Unknown result type (might be due to invalid IL or missing references)
				//IL_0212: Unknown result type (might be due to invalid IL or missing references)
				//IL_0220: Unknown result type (might be due to invalid IL or missing references)
				//IL_022e: Unknown result type (might be due to invalid IL or missing references)
				//IL_023b: Unknown result type (might be due to invalid IL or missing references)
				//IL_024f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0263: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0151: Unknown result type (might be due to invalid IL or missing references)
				//IL_0152: Unknown result type (might be due to invalid IL or missing references)
				//IL_012a: Unknown result type (might be due to invalid IL or missing references)
				//IL_012f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0332: Unknown result type (might be due to invalid IL or missing references)
				//IL_033d: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Quad3)(ref m_Quad)).xz) || entity == m_Ignore)
				{
					return;
				}
				if (m_LotData.HasComponent(entity))
				{
					Transform transform = m_TransformData[entity];
					PrefabRef prefabRef = m_PrefabRefData[entity];
					int2 val = int2.op_Implicit(0);
					BuildingTerraformData buildingTerraformData = default(BuildingTerraformData);
					if (!m_PrefabBuildingTerraformData.TryGetComponent(prefabRef.m_Prefab, ref buildingTerraformData) || !buildingTerraformData.m_DontRaise || !buildingTerraformData.m_DontLower)
					{
						BuildingData buildingData = default(BuildingData);
						BuildingExtensionData buildingExtensionData = default(BuildingExtensionData);
						if (m_PrefabBuildingData.TryGetComponent(prefabRef.m_Prefab, ref buildingData))
						{
							val = buildingData.m_LotSize;
						}
						else if (m_PrefabBuildingExtensionData.TryGetComponent(prefabRef.m_Prefab, ref buildingExtensionData))
						{
							val = math.select(int2.op_Implicit(0), buildingData.m_LotSize, buildingExtensionData.m_External);
						}
					}
					if (math.all(val > 0))
					{
						bool flag = false;
						ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
						if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
						{
							Game.Objects.GeometryFlags geometryFlags = (((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) == 0) ? Game.Objects.GeometryFlags.Circular : Game.Objects.GeometryFlags.CircularLeg);
							flag = (objectGeometryData.m_Flags & geometryFlags) != 0;
						}
						if (flag)
						{
							CheckCircle(transform.m_Position, (float)val.x * 4f, rigid: false, buildingTerraformData.m_DontRaise, buildingTerraformData.m_DontLower);
						}
						else
						{
							CheckQuad(BuildingUtils.CalculateCorners(transform, val), rigid: false, buildingTerraformData.m_DontRaise, buildingTerraformData.m_DontLower);
						}
					}
				}
				else if (m_CompositionData.HasComponent(entity))
				{
					PrefabRef prefabRef2 = m_PrefabRefData[entity];
					if ((m_PrefabNetGeometryData[prefabRef2.m_Prefab].m_Flags & Game.Net.GeometryFlags.FlattenTerrain) == 0)
					{
						return;
					}
					if (HasLotOwner(entity, out var ignore))
					{
						if (ignore || m_ElevationData.HasComponent(entity))
						{
							return;
						}
						Edge edge = m_EdgeData[entity];
						if (m_ElevationData.HasComponent(edge.m_Start) || m_ElevationData.HasComponent(edge.m_End))
						{
							return;
						}
					}
					Composition composition = m_CompositionData[entity];
					EdgeGeometry geometry = m_EdgeGeometryData[entity];
					StartNodeGeometry startNodeGeometry = m_StartNodeGeometryData[entity];
					EndNodeGeometry endNodeGeometry = m_EndNodeGeometryData[entity];
					CheckEdgeGeometry(geometry, composition.m_Edge);
					CheckNodeGeometry(startNodeGeometry.m_Geometry, composition.m_StartNode);
					CheckNodeGeometry(endNodeGeometry.m_Geometry, composition.m_EndNode);
				}
				else
				{
					if (!m_OrphanData.HasComponent(entity))
					{
						return;
					}
					PrefabRef prefabRef3 = m_PrefabRefData[entity];
					if ((m_PrefabNetGeometryData[prefabRef3.m_Prefab].m_Flags & Game.Net.GeometryFlags.FlattenTerrain) == 0 || (HasLotOwner(entity, out var ignore2) && (ignore2 || m_ElevationData.HasComponent(entity))))
					{
						return;
					}
					Orphan orphan = m_OrphanData[entity];
					if (m_PrefabNetCompositionData.HasComponent(orphan.m_Composition))
					{
						NetCompositionData netCompositionData = m_PrefabNetCompositionData[orphan.m_Composition];
						if ((netCompositionData.m_State & CompositionState.ExclusiveGround) != 0 && ((netCompositionData.m_Flags.m_Left | netCompositionData.m_Flags.m_Right) & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0)
						{
							CheckCircle(m_NodeData[entity].m_Position, netCompositionData.m_Width * 0.5f, rigid: true, dontRaise: false, dontLower: false);
						}
					}
				}
			}

			private bool HasLotOwner(Entity entity, out bool ignore)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				Entity val = entity;
				bool flag = false;
				while (m_OwnerData.HasComponent(val))
				{
					val = m_OwnerData[val].m_Owner;
					if (val == m_Ignore)
					{
						ignore = true;
						return true;
					}
					flag |= m_LotData.HasComponent(val);
				}
				ignore = false;
				return flag;
			}

			private void CheckEdgeGeometry(EdgeGeometry geometry, Entity composition)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref geometry.m_Bounds)).xz, ((Quad3)(ref m_Quad)).xz) || !m_PrefabNetCompositionData.HasComponent(composition))
				{
					return;
				}
				NetCompositionData netCompositionData = m_PrefabNetCompositionData[composition];
				if ((netCompositionData.m_State & CompositionState.ExclusiveGround) == 0)
				{
					return;
				}
				if ((netCompositionData.m_Flags.m_Left & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0)
				{
					if (geometry.m_Start.m_Length.x > 0.05f)
					{
						CheckCurve(geometry.m_Start.m_Left, rigid: true);
					}
					if (geometry.m_End.m_Length.x > 0.05f)
					{
						CheckCurve(geometry.m_End.m_Left, rigid: true);
					}
				}
				if ((netCompositionData.m_Flags.m_Right & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0)
				{
					if (geometry.m_Start.m_Length.y > 0.05f)
					{
						CheckCurve(geometry.m_Start.m_Right, rigid: true);
					}
					if (geometry.m_End.m_Length.y > 0.05f)
					{
						CheckCurve(geometry.m_End.m_Right, rigid: true);
					}
				}
			}

			private void CheckNodeGeometry(EdgeNodeGeometry geometry, Entity composition)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref geometry.m_Bounds)).xz, ((Quad3)(ref m_Quad)).xz) || !m_PrefabNetCompositionData.HasComponent(composition))
				{
					return;
				}
				NetCompositionData netCompositionData = m_PrefabNetCompositionData[composition];
				if ((netCompositionData.m_State & CompositionState.ExclusiveGround) == 0)
				{
					return;
				}
				if ((netCompositionData.m_Flags.m_Left & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0)
				{
					if (geometry.m_Left.m_Length.x > 0.05f)
					{
						CheckCurve(geometry.m_Left.m_Left, rigid: true);
					}
					if (geometry.m_Right.m_Length.x > 0.05f && (netCompositionData.m_Flags.m_General & CompositionFlags.General.Roundabout) != 0)
					{
						CheckCurve(geometry.m_Right.m_Left, rigid: true);
					}
				}
				if ((netCompositionData.m_Flags.m_Right & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0)
				{
					if (geometry.m_Right.m_Length.y > 0.05f)
					{
						CheckCurve(geometry.m_Right.m_Right, rigid: true);
					}
					if (geometry.m_Left.m_Length.y > 0.05f && (netCompositionData.m_Flags.m_General & CompositionFlags.General.Roundabout) != 0)
					{
						CheckCurve(geometry.m_Left.m_Right, rigid: true);
					}
				}
			}

			private void CheckCurve(Bezier4x3 curve, bool rigid)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Unknown result type (might be due to invalid IL or missing references)
				//IL_0134: Unknown result type (might be due to invalid IL or missing references)
				//IL_0135: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_014e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0155: Unknown result type (might be due to invalid IL or missing references)
				//IL_015a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(MathUtils.Bounds(((Bezier4x3)(ref curve)).xz), ((Quad3)(ref m_Quad)).xz))
				{
					float3 val = math.lerp(m_Quad.a, m_Quad.b, 1f / 6f);
					float3 val2 = math.lerp(m_Quad.a, m_Quad.b, 0.5f);
					float3 val3 = math.lerp(m_Quad.a, m_Quad.b, 5f / 6f);
					if (m_Radius != 0f)
					{
						val = m_Position + (val - m_Position) * 1.2247449f;
						val2 = m_Position + (val2 - m_Position) * 1.4142135f;
						val3 = m_Position + (val3 - m_Position) * 1.2247449f;
					}
					CheckCurve(new Segment(m_Quad.a, val), 0f, rigid, curve, ref m_Heights1);
					CheckCurve(new Segment(val, val2), 0.5f, rigid, curve, ref m_Heights2);
					CheckCurve(new Segment(val2, val3), 0.5f, rigid, curve, ref m_Heights3);
					CheckCurve(new Segment(val3, m_Quad.b), 1f, rigid, curve, ref m_Heights4);
				}
			}

			private void CheckQuad(Quad3 quad, bool rigid, bool dontRaise, bool dontLower)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0118: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_0135: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_0137: Unknown result type (might be due to invalid IL or missing references)
				//IL_0145: Unknown result type (might be due to invalid IL or missing references)
				//IL_0152: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				//IL_016c: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Quad3)(ref quad)).xz, ((Quad3)(ref m_Quad)).xz))
				{
					float3 val = math.lerp(m_Quad.a, m_Quad.b, 1f / 6f);
					float3 val2 = math.lerp(m_Quad.a, m_Quad.b, 0.5f);
					float3 val3 = math.lerp(m_Quad.a, m_Quad.b, 5f / 6f);
					if (m_Radius != 0f)
					{
						val = m_Position + (val - m_Position) * 1.2247449f;
						val2 = m_Position + (val2 - m_Position) * 1.4142135f;
						val3 = m_Position + (val3 - m_Position) * 1.2247449f;
					}
					CheckQuad(new Segment(m_Quad.a, val), 0f, rigid, dontRaise, dontLower, quad, ref m_Heights1);
					CheckQuad(new Segment(val, val2), 0.5f, rigid, dontRaise, dontLower, quad, ref m_Heights2);
					CheckQuad(new Segment(val2, val3), 0.5f, rigid, dontRaise, dontLower, quad, ref m_Heights3);
					CheckQuad(new Segment(val3, m_Quad.b), 1f, rigid, dontRaise, dontLower, quad, ref m_Heights4);
				}
			}

			private void CheckCircle(float3 position, float radius, bool rigid, bool dontRaise, bool dontLower)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_0116: Unknown result type (might be due to invalid IL or missing references)
				//IL_0117: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_012f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_013b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				float3 val = math.lerp(m_Quad.a, m_Quad.b, 1f / 6f);
				float3 val2 = math.lerp(m_Quad.a, m_Quad.b, 0.5f);
				float3 val3 = math.lerp(m_Quad.a, m_Quad.b, 5f / 6f);
				if (m_Radius != 0f)
				{
					val = m_Position + (val - m_Position) * 1.2247449f;
					val2 = m_Position + (val2 - m_Position) * 1.4142135f;
					val3 = m_Position + (val3 - m_Position) * 1.2247449f;
				}
				CheckCircle(new Segment(m_Quad.a, val), position, radius, rigid, dontRaise, dontLower, ref m_Heights1);
				CheckCircle(new Segment(val, val2), position, radius, rigid, dontRaise, dontLower, ref m_Heights2);
				CheckCircle(new Segment(val2, val3), position, radius, rigid, dontRaise, dontLower, ref m_Heights3);
				CheckCircle(new Segment(val3, m_Quad.b), position, radius, rigid, dontRaise, dontLower, ref m_Heights4);
			}

			private void CheckCurve(Segment line, float pivotT, bool rigid, Bezier4x3 curve, ref Heights heights)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0004: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				Segment val = default(Segment);
				val.a = curve.a;
				for (int i = 1; i <= 16; i++)
				{
					val.b = MathUtils.Position(curve, (float)i * 0.0625f);
					CheckLine(line, pivotT, rigid, dontRaise: false, dontLower: false, val, ref heights);
					val.a = val.b;
				}
			}

			private void CheckQuad(Segment line, float pivotT, bool rigid, bool dontRaise, bool dontLower, Quad3 quad, ref Heights heights)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				CheckLine(line, pivotT, rigid, dontRaise, dontLower, ((Quad3)(ref quad)).ab, ref heights);
				CheckLine(line, pivotT, rigid, dontRaise, dontLower, ((Quad3)(ref quad)).bc, ref heights);
				CheckLine(line, pivotT, rigid, dontRaise, dontLower, ((Quad3)(ref quad)).cd, ref heights);
				CheckLine(line, pivotT, rigid, dontRaise, dontLower, ((Quad3)(ref quad)).da, ref heights);
			}

			private void CheckLine(Segment line, float pivotT, bool rigid, bool dontRaise, bool dontLower, Segment other, ref Heights heights)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				float3 val = MathUtils.Position(line, pivotT);
				float num2 = default(float);
				float num = MathUtils.Distance(((Segment)(ref line)).xz, ((float3)(ref other.a)).xz, ref num2);
				float num3 = MathUtils.Distance(((Segment)(ref line)).xz, ((float3)(ref other.b)).xz, ref num2);
				float num4 = MathUtils.Distance(((Segment)(ref other)).xz, ((float3)(ref val)).xz, ref num2);
				num4 = math.min(num4, math.min(num, num3));
				float num5 = MathUtils.Length(((Segment)(ref other)).xz);
				float num6 = math.min(8f, num5 * 16f);
				if (num4 < num6)
				{
					float num7 = default(float);
					MathUtils.Distance(Line2.op_Implicit(((Segment)(ref other)).xz), ((float3)(ref val)).xz, ref num7);
					float num8 = math.max(0f, math.max(num7 - 1f, 0f - num7)) * num5;
					float offset = MathUtils.Position(((Segment)(ref other)).y, num7) - val.y;
					float strength = (1f - num4 / num6) / (1f + num8 / num6);
					AddHeight(offset, strength, rigid, dontRaise, dontLower, ref heights);
				}
			}

			private void CheckCircle(Segment line, float3 position, float radius, bool rigid, bool dontRaise, bool dontLower, ref Heights heights)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				float num2 = default(float);
				float num = math.max(0f, MathUtils.Distance(((Segment)(ref line)).xz, ((float3)(ref position)).xz, ref num2) - radius);
				if (num < 8f)
				{
					float offset = position.y - MathUtils.Position(((Segment)(ref line)).y, num2);
					float strength = 1f - num / 8f;
					AddHeight(offset, strength, rigid, dontRaise, dontLower, ref heights);
				}
			}

			private void AddHeight(float offset, float strength, bool rigid, bool dontRaise, bool dontLower, ref Heights heights)
			{
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				if (!((offset > 0f && dontRaise) || (offset < 0f && dontLower)))
				{
					if (rigid)
					{
						ref Bounds1 reference = ref heights.m_RigidBounds;
						reference |= offset * strength * 2f;
						heights.m_RigidStrength = math.max(heights.m_RigidStrength, strength);
					}
					else
					{
						ref Bounds1 reference2 = ref heights.m_FlexibleBounds;
						reference2 |= offset * strength;
						heights.m_FlexibleStrength = math.max(heights.m_FlexibleStrength, strength);
					}
				}
			}
		}

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<Orphan> m_OrphanData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> m_PrefabBuildingExtensionData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabNetCompositionData;

		[ReadOnly]
		public ComponentLookup<BuildingTerraformData> m_PrefabBuildingTerraformData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Lot> m_LotData;

		[ReadOnly]
		public bool m_IsLoaded;

		[ReadOnly]
		public NativeList<Entity> m_LotList;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0627: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_0639: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0660: Unknown result type (might be due to invalid IL or missing references)
			//IL_0668: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0682: Unknown result type (might be due to invalid IL or missing references)
			//IL_0687: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_0749: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0801: Unknown result type (might be due to invalid IL or missing references)
			//IL_0806: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0813: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0822: Unknown result type (might be due to invalid IL or missing references)
			//IL_0825: Unknown result type (might be due to invalid IL or missing references)
			//IL_082c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_0836: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0845: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_084f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0854: Unknown result type (might be due to invalid IL or missing references)
			//IL_0856: Unknown result type (might be due to invalid IL or missing references)
			//IL_0858: Unknown result type (might be due to invalid IL or missing references)
			//IL_085d: Unknown result type (might be due to invalid IL or missing references)
			//IL_085f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0861: Unknown result type (might be due to invalid IL or missing references)
			//IL_0866: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0701: Unknown result type (might be due to invalid IL or missing references)
			//IL_0706: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_071a: Unknown result type (might be due to invalid IL or missing references)
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_087d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0893: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_LotList[index];
			Lot lot = m_LotData[val];
			Transform transform = m_TransformData[val];
			PrefabRef prefabRef = m_PrefabRefData[val];
			Lot lot2 = lot;
			int2 val2 = int2.op_Implicit(1);
			BuildingData buildingData = default(BuildingData);
			BuildingExtensionData buildingExtensionData = default(BuildingExtensionData);
			if (m_PrefabBuildingData.TryGetComponent(prefabRef.m_Prefab, ref buildingData))
			{
				val2 = buildingData.m_LotSize;
			}
			else if (m_PrefabBuildingExtensionData.TryGetComponent(prefabRef.m_Prefab, ref buildingExtensionData))
			{
				if (!buildingExtensionData.m_External)
				{
					return;
				}
				val2 = buildingExtensionData.m_LotSize;
			}
			bool flag = false;
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
			{
				Game.Objects.GeometryFlags geometryFlags = (((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) == 0) ? Game.Objects.GeometryFlags.Circular : Game.Objects.GeometryFlags.CircularLeg);
				flag = (objectGeometryData.m_Flags & geometryFlags) != 0;
			}
			Quad3 val3 = BuildingUtils.CalculateCorners(transform, val2);
			Quad3 val4 = BuildingUtils.CalculateCorners(transform, val2 + 2);
			LotIterator lotIterator = new LotIterator
			{
				m_Ignore = val,
				m_OwnerData = m_OwnerData,
				m_LotData = m_LotData,
				m_TransformData = m_TransformData,
				m_EdgeGeometryData = m_EdgeGeometryData,
				m_StartNodeGeometryData = m_StartNodeGeometryData,
				m_EndNodeGeometryData = m_EndNodeGeometryData,
				m_CompositionData = m_CompositionData,
				m_OrphanData = m_OrphanData,
				m_NodeData = m_NodeData,
				m_EdgeData = m_EdgeData,
				m_ElevationData = m_ElevationData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabBuildingData = m_PrefabBuildingData,
				m_PrefabBuildingExtensionData = m_PrefabBuildingExtensionData,
				m_PrefabNetGeometryData = m_PrefabNetGeometryData,
				m_PrefabNetCompositionData = m_PrefabNetCompositionData,
				m_PrefabBuildingTerraformData = m_PrefabBuildingTerraformData,
				m_PrefabObjectGeometryData = m_PrefabObjectGeometryData
			};
			if (flag)
			{
				val3.a = transform.m_Position + (val3.a - transform.m_Position) * 0.70710677f;
				val3.b = transform.m_Position + (val3.b - transform.m_Position) * 0.70710677f;
				val3.c = transform.m_Position + (val3.c - transform.m_Position) * 0.70710677f;
				val3.d = transform.m_Position + (val3.d - transform.m_Position) * 0.70710677f;
				lotIterator.m_Radius = (float)val2.x * 4f;
				lotIterator.m_Position = transform.m_Position;
			}
			lotIterator.m_Heights1.Reset();
			lotIterator.m_Heights2.Reset();
			lotIterator.m_Heights3.Reset();
			lotIterator.m_Heights4.Reset();
			lotIterator.m_Quad = new Quad3(val3.a, val3.b, val4.b, val4.a);
			m_StaticObjectSearchTree.Iterate<LotIterator>(ref lotIterator, 0);
			m_NetSearchTree.Iterate<LotIterator>(ref lotIterator, 0);
			Heights heights = lotIterator.m_Heights1;
			lot.m_FrontHeights.y = lotIterator.m_Heights2.Center();
			lot.m_FrontHeights.z = lotIterator.m_Heights3.Center();
			lotIterator.m_Heights1 = lotIterator.m_Heights4;
			lotIterator.m_Heights2.Reset();
			lotIterator.m_Heights3.Reset();
			lotIterator.m_Heights4.Reset();
			lotIterator.m_Quad = new Quad3(val3.b, val3.c, val4.c, val4.b);
			m_StaticObjectSearchTree.Iterate<LotIterator>(ref lotIterator, 0);
			m_NetSearchTree.Iterate<LotIterator>(ref lotIterator, 0);
			lot.m_RightHeights.x = lotIterator.m_Heights1.Center();
			lot.m_RightHeights.y = lotIterator.m_Heights2.Center();
			lot.m_RightHeights.z = lotIterator.m_Heights3.Center();
			lotIterator.m_Heights1 = lotIterator.m_Heights4;
			lotIterator.m_Heights2.Reset();
			lotIterator.m_Heights3.Reset();
			lotIterator.m_Heights4.Reset();
			lotIterator.m_Quad = new Quad3(val3.c, val3.d, val4.d, val4.c);
			m_StaticObjectSearchTree.Iterate<LotIterator>(ref lotIterator, 0);
			m_NetSearchTree.Iterate<LotIterator>(ref lotIterator, 0);
			lot.m_BackHeights.x = lotIterator.m_Heights1.Center();
			lot.m_BackHeights.y = lotIterator.m_Heights2.Center();
			lot.m_BackHeights.z = lotIterator.m_Heights3.Center();
			lotIterator.m_Heights1 = lotIterator.m_Heights4;
			lotIterator.m_Heights2.Reset();
			lotIterator.m_Heights3.Reset();
			lotIterator.m_Heights4 = heights;
			lotIterator.m_Quad = new Quad3(val3.d, val3.a, val4.a, val4.d);
			m_StaticObjectSearchTree.Iterate<LotIterator>(ref lotIterator, 0);
			m_NetSearchTree.Iterate<LotIterator>(ref lotIterator, 0);
			lot.m_LeftHeights.x = lotIterator.m_Heights1.Center();
			lot.m_LeftHeights.y = lotIterator.m_Heights2.Center();
			lot.m_LeftHeights.z = lotIterator.m_Heights3.Center();
			lot.m_FrontHeights.x = lotIterator.m_Heights4.Center();
			m_OwnerData = lotIterator.m_OwnerData;
			m_LotData = lotIterator.m_LotData;
			m_TransformData = lotIterator.m_TransformData;
			m_EdgeGeometryData = lotIterator.m_EdgeGeometryData;
			m_StartNodeGeometryData = lotIterator.m_StartNodeGeometryData;
			m_EndNodeGeometryData = lotIterator.m_EndNodeGeometryData;
			m_CompositionData = lotIterator.m_CompositionData;
			m_OrphanData = lotIterator.m_OrphanData;
			m_NodeData = lotIterator.m_NodeData;
			m_EdgeData = lotIterator.m_EdgeData;
			m_ElevationData = lotIterator.m_ElevationData;
			m_PrefabRefData = lotIterator.m_PrefabRefData;
			m_PrefabBuildingData = lotIterator.m_PrefabBuildingData;
			m_PrefabBuildingExtensionData = lotIterator.m_PrefabBuildingExtensionData;
			m_PrefabNetGeometryData = lotIterator.m_PrefabNetGeometryData;
			m_PrefabNetCompositionData = lotIterator.m_PrefabNetCompositionData;
			m_PrefabBuildingTerraformData = lotIterator.m_PrefabBuildingTerraformData;
			m_PrefabObjectGeometryData = lotIterator.m_PrefabObjectGeometryData;
			if (flag)
			{
				float3 val5 = default(float3);
				((float3)(ref val5))._002Ector(1.4142135f, 1.0352762f, 1.0352762f);
				ref float3 frontHeights = ref lot.m_FrontHeights;
				frontHeights *= val5;
				ref float3 rightHeights = ref lot.m_RightHeights;
				rightHeights *= val5;
				ref float3 backHeights = ref lot.m_BackHeights;
				backHeights *= val5;
				ref float3 leftHeights = ref lot.m_LeftHeights;
				leftHeights *= val5;
			}
			CalculateMiddleHeights(lot.m_FrontHeights.x, ref lot.m_FrontHeights.y, ref lot.m_FrontHeights.z, lot.m_RightHeights.x);
			CalculateMiddleHeights(lot.m_RightHeights.x, ref lot.m_RightHeights.y, ref lot.m_RightHeights.z, lot.m_BackHeights.x);
			CalculateMiddleHeights(lot.m_BackHeights.x, ref lot.m_BackHeights.y, ref lot.m_BackHeights.z, lot.m_LeftHeights.x);
			CalculateMiddleHeights(lot.m_LeftHeights.x, ref lot.m_LeftHeights.y, ref lot.m_LeftHeights.z, lot.m_FrontHeights.x);
			float3 val6 = math.abs(lot.m_FrontHeights - lot2.m_FrontHeights);
			float3 val7 = math.abs(lot.m_RightHeights - lot2.m_RightHeights);
			float3 val8 = math.abs(lot.m_BackHeights - lot2.m_BackHeights);
			float3 val9 = math.abs(lot.m_LeftHeights - lot2.m_LeftHeights);
			if (math.cmax(math.max(math.max(val6, val7), math.max(val8, val9))) >= 0.01f)
			{
				m_LotData[val] = lot;
				if (!m_IsLoaded)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
				}
			}
		}

		private void CalculateMiddleHeights(float a, ref float b, ref float c, float d)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			float num = b - MathUtils.Position(new Bezier4x1(a, b, c, d), 1f / 3f);
			float num2 = c - MathUtils.Position(new Bezier4x1(a, b, c, d), 2f / 3f);
			b += num * 3f - num2 * 1.5f;
			c += num2 * 3f - num * 1.5f;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentLookup<Lot> __Game_Buildings_Lot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Orphan> __Game_Net_Orphan_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingTerraformData> __Game_Prefabs_BuildingTerraformData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		public ComponentLookup<Lot> __Game_Buildings_Lot_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lot>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_Orphan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Orphan>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingExtensionData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_BuildingTerraformData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingTerraformData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Buildings_Lot_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lot>(false);
		}
	}

	private Game.Objects.UpdateCollectSystem m_ObjectUpdateCollectSystem;

	private Game.Net.UpdateCollectSystem m_NetUpdateCollectSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private ModificationBarrier5 m_ModificationBarrier;

	private EntityQuery m_UpdateQuery;

	private EntityQuery m_AllQuery;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ObjectUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.UpdateCollectSystem>();
		m_NetUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.UpdateCollectSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_UpdateQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<Lot>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.Exclude<Deleted>()
		});
		m_AllQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Lot>(),
			ComponentType.Exclude<Deleted>()
		});
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Loaded = true;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		EntityQuery val = (loaded ? m_AllQuery : m_UpdateQuery);
		bool flag = !((EntityQuery)(ref val)).IsEmptyIgnoreFilter;
		if (m_ObjectUpdateCollectSystem.isUpdated || m_NetUpdateCollectSystem.netsUpdated || flag)
		{
			JobHandle dependencies;
			NativeQuadTree<Entity, QuadTreeBoundsXZ> staticSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies);
			NativeList<Entity> val2 = default(NativeList<Entity>);
			val2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<Entity> queue = default(NativeQueue<Entity>);
			NativeQueue<Entity> queue2 = default(NativeQueue<Entity>);
			JobHandle val3 = default(JobHandle);
			if (flag)
			{
				JobHandle val4 = JobChunkExtensions.Schedule<AddUpdatedLotsJob>(new AddUpdatedLotsJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ResultList = val2
				}, val, ((SystemBase)this).Dependency);
				val3 = JobHandle.CombineDependencies(val3, val4);
			}
			if (m_ObjectUpdateCollectSystem.isUpdated)
			{
				JobHandle dependencies2;
				NativeList<Bounds2> updatedBounds = m_ObjectUpdateCollectSystem.GetUpdatedBounds(out dependencies2);
				queue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
				JobHandle val5 = IJobParallelForDeferExtensions.Schedule<FindUpdatedLotsJob, Bounds2>(new FindUpdatedLotsJob
				{
					m_Bounds = updatedBounds.AsDeferredJobArray(),
					m_SearchTree = staticSearchTree,
					m_LotData = InternalCompilerInterface.GetComponentLookup<Lot>(ref __TypeHandle.__Game_Buildings_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ResultQueue = queue.AsParallelWriter()
				}, updatedBounds, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies2, dependencies));
				m_ObjectUpdateCollectSystem.AddBoundsReader(val5);
				val3 = JobHandle.CombineDependencies(val3, val5);
			}
			if (m_NetUpdateCollectSystem.netsUpdated)
			{
				JobHandle dependencies3;
				NativeList<Bounds2> updatedNetBounds = m_NetUpdateCollectSystem.GetUpdatedNetBounds(out dependencies3);
				queue2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
				JobHandle val6 = IJobParallelForDeferExtensions.Schedule<FindUpdatedLotsJob, Bounds2>(new FindUpdatedLotsJob
				{
					m_Bounds = updatedNetBounds.AsDeferredJobArray(),
					m_SearchTree = staticSearchTree,
					m_LotData = InternalCompilerInterface.GetComponentLookup<Lot>(ref __TypeHandle.__Game_Buildings_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ResultQueue = queue2.AsParallelWriter()
				}, updatedNetBounds, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies3, dependencies));
				m_NetUpdateCollectSystem.AddNetBoundsReader(val6);
				val3 = JobHandle.CombineDependencies(val3, val6);
			}
			CollectLotsJob collectLotsJob = new CollectLotsJob
			{
				m_Queue1 = queue,
				m_Queue2 = queue2,
				m_ResultList = val2
			};
			JobHandle dependencies4;
			UpdateLotHeightsJob updateLotHeightsJob = new UpdateLotHeightsJob
			{
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OrphanData = InternalCompilerInterface.GetComponentLookup<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBuildingExtensionData = InternalCompilerInterface.GetComponentLookup<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBuildingTerraformData = InternalCompilerInterface.GetComponentLookup<BuildingTerraformData>(ref __TypeHandle.__Game_Prefabs_BuildingTerraformData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LotData = InternalCompilerInterface.GetComponentLookup<Lot>(ref __TypeHandle.__Game_Buildings_Lot_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IsLoaded = loaded,
				m_LotList = val2,
				m_StaticObjectSearchTree = staticSearchTree,
				m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies4)
			};
			EntityCommandBuffer val7 = m_ModificationBarrier.CreateCommandBuffer();
			updateLotHeightsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val7)).AsParallelWriter();
			UpdateLotHeightsJob updateLotHeightsJob2 = updateLotHeightsJob;
			JobHandle val8 = IJobExtensions.Schedule<CollectLotsJob>(collectLotsJob, val3);
			JobHandle val9 = IJobParallelForDeferExtensions.Schedule<UpdateLotHeightsJob, Entity>(updateLotHeightsJob2, val2, 1, JobHandle.CombineDependencies(val8, dependencies, dependencies4));
			if (queue.IsCreated)
			{
				queue.Dispose(val8);
			}
			if (queue2.IsCreated)
			{
				queue2.Dispose(val8);
			}
			val2.Dispose(val9);
			m_ObjectSearchSystem.AddStaticSearchTreeReader(val9);
			m_NetSearchSystem.AddNetSearchTreeReader(val9);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val9);
			((SystemBase)this).Dependency = val9;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public LotHeightSystem()
	{
	}
}

using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class AnimalMoveSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateTransformDataJob : IJobChunk
	{
		private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public Quad3 m_CornerPositions;

			public float4 m_GroundHeights;

			public ComponentLookup<Composition> m_CompositionData;

			public ComponentLookup<Orphan> m_OrphanData;

			public ComponentLookup<Node> m_NodeData;

			public ComponentLookup<NodeGeometry> m_NodeGeometryData;

			public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

			public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

			public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

			public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds))
				{
					if (m_CompositionData.HasComponent(item))
					{
						CheckEdge(item);
					}
					else if (m_OrphanData.HasComponent(item))
					{
						CheckNode(item);
					}
				}
			}

			private void CheckNode(Entity entity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(m_NodeGeometryData[entity].m_Bounds, m_Bounds))
				{
					Node node = m_NodeData[entity];
					Orphan orphan = m_OrphanData[entity];
					NetCompositionData netCompositionData = m_PrefabCompositionData[orphan.m_Composition];
					float3 position = node.m_Position;
					position.y += netCompositionData.m_SurfaceHeight.max;
					CheckCircle(position, netCompositionData.m_Width * 0.5f);
				}
			}

			private void CheckEdge(Entity entity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_020c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01db: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Unknown result type (might be due to invalid IL or missing references)
				//IL_0137: Unknown result type (might be due to invalid IL or missing references)
				//IL_0144: Unknown result type (might be due to invalid IL or missing references)
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0170: Unknown result type (might be due to invalid IL or missing references)
				//IL_0175: Unknown result type (might be due to invalid IL or missing references)
				//IL_0182: Unknown result type (might be due to invalid IL or missing references)
				//IL_0187: Unknown result type (might be due to invalid IL or missing references)
				//IL_018c: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_024b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0256: Unknown result type (might be due to invalid IL or missing references)
				//IL_0260: Unknown result type (might be due to invalid IL or missing references)
				//IL_0265: Unknown result type (might be due to invalid IL or missing references)
				//IL_0272: Unknown result type (might be due to invalid IL or missing references)
				//IL_0277: Unknown result type (might be due to invalid IL or missing references)
				//IL_027c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0285: Unknown result type (might be due to invalid IL or missing references)
				//IL_028a: Unknown result type (might be due to invalid IL or missing references)
				EdgeGeometry edgeGeometry = m_EdgeGeometryData[entity];
				EdgeNodeGeometry geometry = m_StartNodeGeometryData[entity].m_Geometry;
				EdgeNodeGeometry geometry2 = m_EndNodeGeometryData[entity].m_Geometry;
				bool3 val = default(bool3);
				val.x = MathUtils.Intersect(edgeGeometry.m_Bounds, m_Bounds);
				val.y = MathUtils.Intersect(geometry.m_Bounds, m_Bounds);
				val.z = MathUtils.Intersect(geometry2.m_Bounds, m_Bounds);
				if (!math.any(val))
				{
					return;
				}
				Composition composition = m_CompositionData[entity];
				if (val.x)
				{
					NetCompositionData prefabCompositionData = m_PrefabCompositionData[composition.m_Edge];
					CheckSegment(edgeGeometry.m_Start, prefabCompositionData);
					CheckSegment(edgeGeometry.m_End, prefabCompositionData);
				}
				if (val.y)
				{
					NetCompositionData prefabCompositionData2 = m_PrefabCompositionData[composition.m_StartNode];
					if (geometry.m_MiddleRadius > 0f)
					{
						CheckSegment(geometry.m_Left, prefabCompositionData2);
						Segment right = geometry.m_Right;
						Segment right2 = geometry.m_Right;
						right.m_Right = MathUtils.Lerp(geometry.m_Right.m_Left, geometry.m_Right.m_Right, 0.5f);
						right2.m_Left = MathUtils.Lerp(geometry.m_Right.m_Left, geometry.m_Right.m_Right, 0.5f);
						right.m_Right.d = geometry.m_Middle.d;
						right2.m_Left.d = geometry.m_Middle.d;
						CheckSegment(right, prefabCompositionData2);
						CheckSegment(right2, prefabCompositionData2);
					}
					else
					{
						Segment left = geometry.m_Left;
						Segment right3 = geometry.m_Right;
						CheckSegment(left, prefabCompositionData2);
						CheckSegment(right3, prefabCompositionData2);
						left.m_Right = geometry.m_Middle;
						right3.m_Left = geometry.m_Middle;
						CheckSegment(left, prefabCompositionData2);
						CheckSegment(right3, prefabCompositionData2);
					}
				}
				if (val.z)
				{
					NetCompositionData prefabCompositionData3 = m_PrefabCompositionData[composition.m_EndNode];
					if (geometry2.m_MiddleRadius > 0f)
					{
						CheckSegment(geometry2.m_Left, prefabCompositionData3);
						Segment right4 = geometry2.m_Right;
						Segment right5 = geometry2.m_Right;
						right4.m_Right = MathUtils.Lerp(geometry2.m_Right.m_Left, geometry2.m_Right.m_Right, 0.5f);
						right4.m_Right.d = geometry2.m_Middle.d;
						right5.m_Left = right4.m_Right;
						CheckSegment(right4, prefabCompositionData3);
						CheckSegment(right5, prefabCompositionData3);
					}
					else
					{
						Segment left2 = geometry2.m_Left;
						Segment right6 = geometry2.m_Right;
						CheckSegment(left2, prefabCompositionData3);
						CheckSegment(right6, prefabCompositionData3);
						left2.m_Right = geometry2.m_Middle;
						right6.m_Left = geometry2.m_Middle;
						CheckSegment(left2, prefabCompositionData3);
						CheckSegment(right6, prefabCompositionData3);
					}
				}
			}

			private void CheckSegment(Segment segment, NetCompositionData prefabCompositionData)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				float3 val = segment.m_Left.a;
				float3 val2 = segment.m_Right.a;
				val.y += prefabCompositionData.m_SurfaceHeight.max;
				val2.y += prefabCompositionData.m_SurfaceHeight.max;
				Bounds3 val3 = MathUtils.Bounds(val, val2);
				for (int i = 1; i <= 8; i++)
				{
					float num = (float)i / 8f;
					float3 val4 = MathUtils.Position(segment.m_Left, num);
					float3 val5 = MathUtils.Position(segment.m_Right, num);
					val4.y += prefabCompositionData.m_SurfaceHeight.max;
					val5.y += prefabCompositionData.m_SurfaceHeight.max;
					Bounds3 val6 = MathUtils.Bounds(val4, val5);
					if (MathUtils.Intersect(val3 | val6, m_Bounds))
					{
						CheckTriangle(new Triangle3(val, val2, val4));
						CheckTriangle(new Triangle3(val5, val4, val2));
					}
					val = val4;
					val2 = val5;
					val3 = val6;
				}
			}

			private void CheckCircle(float3 center, float radius)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				CheckCircle(center, radius, m_CornerPositions.a, ref m_GroundHeights.x);
				CheckCircle(center, radius, m_CornerPositions.b, ref m_GroundHeights.y);
				CheckCircle(center, radius, m_CornerPositions.c, ref m_GroundHeights.z);
				CheckCircle(center, radius, m_CornerPositions.d, ref m_GroundHeights.w);
			}

			private void CheckTriangle(Triangle3 triangle)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				CheckTriangle(triangle, m_CornerPositions.a, ref m_GroundHeights.x);
				CheckTriangle(triangle, m_CornerPositions.b, ref m_GroundHeights.y);
				CheckTriangle(triangle, m_CornerPositions.c, ref m_GroundHeights.z);
				CheckTriangle(triangle, m_CornerPositions.d, ref m_GroundHeights.w);
			}

			private void CheckCircle(float3 center, float radius, float3 position, ref float groundHeight)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				if (math.distance(((float3)(ref center)).xz, ((float3)(ref position)).xz) <= radius)
				{
					float y = center.y;
					groundHeight = math.select(groundHeight, y, (y < position.y + 4f) & (y > groundHeight));
				}
			}

			private void CheckTriangle(Triangle3 triangle, float3 position, ref float groundHeight)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				if (MathUtils.Intersect(((Triangle3)(ref triangle)).xz, ((float3)(ref position)).xz, ref val))
				{
					float y = MathUtils.Position(triangle, val).y;
					groundHeight = math.select(groundHeight, y, (y < position.y + 4f) & (y > groundHeight));
				}
			}
		}

		[ReadOnly]
		public ComponentTypeHandle<Animal> m_AnimalType;

		[ReadOnly]
		public ComponentTypeHandle<AnimalNavigation> m_NavigationType;

		[ReadOnly]
		public ComponentTypeHandle<Stumbling> m_StumblingType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> m_MeshGroupType;

		public ComponentTypeHandle<Moving> m_MovingType;

		public ComponentTypeHandle<Transform> m_TransformType;

		public BufferTypeHandle<TransformFrame> m_TransformFrameType;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<Orphan> m_OrphanData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> m_NodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<AnimalData> m_PrefabAnimalData;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_SubMeshGroups;

		[ReadOnly]
		public BufferLookup<CharacterElement> m_CharacterElements;

		[ReadOnly]
		public BufferLookup<AnimationClip> m_AnimationClips;

		[ReadOnly]
		public BufferLookup<AnimationMotion> m_AnimationMotions;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public uint m_TransformFrameIndex;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Stumbling>(ref m_StumblingType))
			{
				StumblingMove(chunk);
			}
			else
			{
				NormalMove(chunk);
			}
		}

		private void NormalMove(ArchetypeChunk chunk)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Animal> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Animal>(ref m_AnimalType);
			NativeArray<AnimalNavigation> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalNavigation>(ref m_NavigationType);
			NativeArray<Moving> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
			NativeArray<Transform> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<TransformFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TransformFrame>(ref m_TransformFrameType);
			BufferAccessor<MeshGroup> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<MeshGroup>(ref m_MeshGroupType);
			float num = 4f / 15f;
			int num2 = (int)m_TransformFrameIndex;
			int num3 = math.select((int)(m_TransformFrameIndex - 1), 3, m_TransformFrameIndex == 0);
			DynamicBuffer<MeshGroup> meshGroups = default(DynamicBuffer<MeshGroup>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Animal animal = nativeArray[i];
				AnimalNavigation animalNavigation = nativeArray2[i];
				Moving moving = nativeArray3[i];
				Transform transform = nativeArray4[i];
				PrefabRef prefabRef = nativeArray5[i];
				DynamicBuffer<TransformFrame> val = bufferAccessor[i];
				TransformFrame oldFrameData = val[num3];
				if ((animal.m_Flags & AnimalFlags.Roaming) != 0)
				{
					float2 val2 = ((float3)(ref animalNavigation.m_TargetPosition)).xz - ((float3)(ref transform.m_Position)).xz;
					MathUtils.TryNormalize(ref val2, animalNavigation.m_MaxSpeed * num);
					((float3)(ref animalNavigation.m_TargetPosition)).xz = ((float3)(ref transform.m_Position)).xz + val2;
					if (animalNavigation.m_TargetActivity == 8)
					{
						AnimalData animalData = m_PrefabAnimalData[prefabRef.m_Prefab];
						if ((animal.m_Flags & AnimalFlags.SwimmingTarget) == 0)
						{
							animalData.m_SwimDepth.min = 0f;
						}
						Bounds1 val3 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, animalNavigation.m_TargetPosition) - MathUtils.Invert(animalData.m_SwimDepth);
						animalNavigation.m_TargetPosition.y = MathUtils.Clamp(animalNavigation.m_TargetPosition.y, val3);
					}
					else if (animalNavigation.m_TargetActivity == 9)
					{
						AnimalData animalData2 = m_PrefabAnimalData[prefabRef.m_Prefab];
						if ((animal.m_Flags & AnimalFlags.FlyingTarget) == 0)
						{
							animalData2.m_FlyHeight.min = 0f;
						}
						Bounds1 val4 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, animalNavigation.m_TargetPosition) + animalData2.m_FlyHeight;
						animalNavigation.m_TargetPosition.y = MathUtils.Clamp(animalNavigation.m_TargetPosition.y, val4);
					}
					else
					{
						animalNavigation.m_TargetPosition.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, animalNavigation.m_TargetPosition);
					}
				}
				float3 val5 = animalNavigation.m_TargetPosition - transform.m_Position;
				MathUtils.TryNormalize(ref val5, animalNavigation.m_MaxSpeed);
				TransformFrame newFrameData = default(TransformFrame);
				switch ((ActivityType)animalNavigation.m_TargetActivity)
				{
				case ActivityType.Walking:
				case ActivityType.Running:
				case ActivityType.Swimming:
				case ActivityType.Flying:
					newFrameData.m_State = TransformState.Move;
					newFrameData.m_Activity = animalNavigation.m_TargetActivity;
					break;
				case ActivityType.None:
					newFrameData.m_State = ((!(animalNavigation.m_MaxSpeed >= 0.1f)) ? TransformState.Idle : TransformState.Move);
					break;
				default:
					newFrameData.m_State = TransformState.Idle;
					newFrameData.m_Activity = animalNavigation.m_TargetActivity;
					break;
				}
				CollectionUtils.TryGet<MeshGroup>(bufferAccessor2, i, ref meshGroups);
				ObjectUtils.UpdateAnimation(oldPropID: new AnimatedPropID(-1), newPropID: new AnimatedPropID(-1), prefab: prefabRef.m_Prefab, timeStep: num, meshGroups: meshGroups, subMeshGroupBuffers: ref m_SubMeshGroups, characterElementBuffers: ref m_CharacterElements, subMeshBuffers: ref m_SubMeshes, animationClipBuffers: ref m_AnimationClips, animationMotionBuffers: ref m_AnimationMotions, conditions: (ActivityCondition)0u, maxSpeed: ref animalNavigation.m_MaxSpeed, activity: ref animalNavigation.m_TargetActivity, targetPosition: ref animalNavigation.m_TargetPosition, targetDirection: ref animalNavigation.m_TargetDirection, transform: ref transform, oldFrameData: ref oldFrameData, newFrameData: ref newFrameData);
				animalNavigation.m_TransformState = newFrameData.m_State;
				animalNavigation.m_LastActivity = newFrameData.m_Activity;
				newFrameData.m_Position = transform.m_Position;
				newFrameData.m_Velocity = moving.m_Velocity;
				newFrameData.m_Rotation = transform.m_Rotation;
				float3 val6 = val5 * 8f + moving.m_Velocity;
				MathUtils.TryNormalize(ref val6, animalNavigation.m_MaxSpeed);
				moving.m_Velocity = val6;
				float3 val7 = moving.m_Velocity * (num * 0.5f);
				ref float3 position = ref transform.m_Position;
				position += val7;
				if (math.length(((float3)(ref moving.m_Velocity)).xz) >= 0.1f)
				{
					transform.m_Rotation = quaternion.LookRotationSafe(val6, math.up());
				}
				ref float3 position2 = ref transform.m_Position;
				position2 += val7;
				val[num2] = newFrameData;
				nativeArray3[i] = moving;
				nativeArray4[i] = transform;
			}
		}

		private void StumblingMove(ArchetypeChunk chunk)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<AnimalNavigation> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalNavigation>(ref m_NavigationType);
			NativeArray<Moving> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
			NativeArray<Transform> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			BufferAccessor<TransformFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TransformFrame>(ref m_TransformFrameType);
			int num = 4;
			float num2 = 4f / 15f;
			float num3 = num2 / (float)num;
			float grip = 10f;
			float num4 = 10f;
			float num5 = math.pow(0.9f, num3);
			Quad3 val2 = default(Quad3);
			Quad3 val3 = default(Quad3);
			Quad3 val4 = default(Quad3);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				PrefabRef prefabRef = nativeArray[i];
				_ = nativeArray2[i];
				Moving moving = nativeArray3[i];
				Transform transform = nativeArray4[i];
				ObjectGeometryData objectGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
				for (int j = 0; j < num; j++)
				{
					float3 momentOfInertia = ObjectUtils.CalculateMomentOfInertia(transform.m_Rotation, objectGeometryData.m_Size);
					float3 val = transform.m_Position + math.mul(transform.m_Rotation, new float3(0f, objectGeometryData.m_Bounds.max.y * 0.5f, 0f));
					val2.a = ObjectUtils.LocalToWorld(transform, new float3(0f, objectGeometryData.m_Bounds.min.y + 0.2f, objectGeometryData.m_Bounds.min.z + 0.2f));
					val2.b = ObjectUtils.LocalToWorld(transform, new float3(0f, objectGeometryData.m_Bounds.min.y + 0.2f, objectGeometryData.m_Bounds.max.z - 0.2f));
					val2.c = ObjectUtils.LocalToWorld(transform, new float3(0f, objectGeometryData.m_Bounds.max.y - 0.2f, objectGeometryData.m_Bounds.min.z + 0.2f));
					val2.d = ObjectUtils.LocalToWorld(transform, new float3(0f, objectGeometryData.m_Bounds.max.y - 0.2f, objectGeometryData.m_Bounds.max.z - 0.2f));
					GetGroundHeight(val2, moving.m_Velocity, num2, num4, out var heights);
					heights += 0.2f;
					val3.a = CalculatePointVelocityDelta(val2.a, val, moving, grip, num3, heights.x, num4);
					val3.b = CalculatePointVelocityDelta(val2.b, val, moving, grip, num3, heights.y, num4);
					val3.c = CalculatePointVelocityDelta(val2.c, val, moving, grip, num3, heights.z, num4);
					val3.d = CalculatePointVelocityDelta(val2.d, val, moving, grip, num3, heights.w, num4);
					val4.a = CalculatePointAngularVelocityDelta(val2.a, val, val3.a, momentOfInertia);
					val4.b = CalculatePointAngularVelocityDelta(val2.b, val, val3.b, momentOfInertia);
					val4.c = CalculatePointAngularVelocityDelta(val2.c, val, val3.c, momentOfInertia);
					val4.d = CalculatePointAngularVelocityDelta(val2.d, val, val3.d, momentOfInertia);
					float3 val5 = (val3.a + val3.b + val3.c + val3.d) * 0.125f;
					float3 val6 = (val4.a + val4.b + val4.c + val4.d) * 0.125f;
					val5.y -= num4 * num3;
					ref float3 velocity = ref moving.m_Velocity;
					velocity *= num5;
					ref float3 angularVelocity = ref moving.m_AngularVelocity;
					angularVelocity *= num5;
					ref float3 velocity2 = ref moving.m_Velocity;
					velocity2 += val5;
					ref float3 angularVelocity2 = ref moving.m_AngularVelocity;
					angularVelocity2 += val6;
					float num6 = math.length(moving.m_AngularVelocity);
					if (num6 > 1E-05f)
					{
						quaternion val7 = quaternion.AxisAngle(moving.m_AngularVelocity / num6, num6 * num3);
						transform.m_Rotation = math.normalize(math.mul(val7, transform.m_Rotation));
						float3 val8 = transform.m_Position + math.mul(transform.m_Rotation, new float3(0f, objectGeometryData.m_Bounds.max.y * 0.5f, 0f));
						ref float3 position = ref transform.m_Position;
						position += val - val8;
					}
					ref float3 position2 = ref transform.m_Position;
					position2 += moving.m_Velocity * num3;
				}
				TransformFrame transformFrame = new TransformFrame
				{
					m_Position = transform.m_Position - moving.m_Velocity * (num2 * 0.5f),
					m_Velocity = moving.m_Velocity,
					m_Rotation = transform.m_Rotation
				};
				DynamicBuffer<TransformFrame> val9 = bufferAccessor[i];
				val9[(int)m_TransformFrameIndex] = transformFrame;
				nativeArray3[i] = moving;
				nativeArray4[i] = transform;
			}
		}

		private float3 CalculatePointVelocityDelta(float3 position, float3 origin, Moving moving, float grip, float timeStep, float groundHeight, float gravity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			float3 val2;
			float3 val = (val2 = ObjectUtils.CalculatePointVelocity(position - origin, moving));
			val2.y = 0f;
			float num = val.y - gravity * timeStep;
			position.y += num * timeStep;
			float num2 = math.max(0f, groundHeight - position.y) / timeStep;
			float3 result = MathUtils.ClampLength(-val2, grip * math.min(timeStep, num2 / gravity));
			result.y += num2;
			return result;
		}

		private float3 CalculatePointAngularVelocityDelta(float3 position, float3 origin, float3 velocityDelta, float3 momentOfInertia)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return math.cross(position - origin, velocityDelta) / momentOfInertia;
		}

		private void GetGroundHeight(Quad3 cornerPositions, float3 velocity, float timeStep, float gravity, out float4 heights)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			float4 val = default(float4);
			val.x = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cornerPositions.a);
			val.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cornerPositions.b);
			val.z = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cornerPositions.c);
			val.w = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cornerPositions.d);
			float4 val2 = default(float4);
			((float4)(ref val2))._002Ector(cornerPositions.a.y, cornerPositions.b.y, cornerPositions.c.y, cornerPositions.d.y);
			heights = math.select(float4.op_Implicit(float.MinValue), val, val < val2 + 4f);
			NetIterator netIterator = new NetIterator
			{
				m_Bounds = MathUtils.Bounds(cornerPositions)
			};
			netIterator.m_Bounds.min.y += (math.min(0f, velocity.y) - gravity * timeStep) * timeStep;
			netIterator.m_Bounds.max.y += math.max(0f, velocity.y) * timeStep;
			netIterator.m_CornerPositions = cornerPositions;
			netIterator.m_GroundHeights = heights;
			netIterator.m_CompositionData = m_CompositionData;
			netIterator.m_OrphanData = m_OrphanData;
			netIterator.m_NodeData = m_NodeData;
			netIterator.m_NodeGeometryData = m_NodeGeometryData;
			netIterator.m_EdgeGeometryData = m_EdgeGeometryData;
			netIterator.m_StartNodeGeometryData = m_StartNodeGeometryData;
			netIterator.m_EndNodeGeometryData = m_EndNodeGeometryData;
			netIterator.m_PrefabCompositionData = m_PrefabCompositionData;
			m_NetSearchTree.Iterate<NetIterator>(ref netIterator, 0);
			heights = netIterator.m_GroundHeights;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Animal> __Game_Creatures_Animal_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AnimalNavigation> __Game_Creatures_AnimalNavigation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stumbling> __Game_Creatures_Stumbling_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferTypeHandle;

		public ComponentTypeHandle<Moving> __Game_Objects_Moving_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RW_ComponentTypeHandle;

		public BufferTypeHandle<TransformFrame> __Game_Objects_TransformFrame_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Orphan> __Game_Net_Orphan_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> __Game_Net_NodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AnimalData> __Game_Prefabs_AnimalData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> __Game_Prefabs_SubMeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CharacterElement> __Game_Prefabs_CharacterElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AnimationClip> __Game_Prefabs_AnimationClip_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AnimationMotion> __Game_Prefabs_AnimationMotion_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			__Game_Creatures_Animal_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Animal>(true);
			__Game_Creatures_AnimalNavigation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalNavigation>(true);
			__Game_Creatures_Stumbling_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stumbling>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Rendering_MeshGroup_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MeshGroup>(true);
			__Game_Objects_Moving_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Moving>(false);
			__Game_Objects_Transform_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(false);
			__Game_Objects_TransformFrame_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TransformFrame>(false);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_Orphan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Orphan>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_NodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NodeGeometry>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_AnimalData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalData>(true);
			__Game_Prefabs_SubMeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(true);
			__Game_Prefabs_CharacterElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CharacterElement>(true);
			__Game_Prefabs_AnimationClip_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationClip>(true);
			__Game_Prefabs_AnimationMotion_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationMotion>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private EntityQuery m_AnimalQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_AnimalQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Animal>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		uint num = m_SimulationSystem.frameIndex % 16;
		if (num == 5 || num == 9 || num == 13)
		{
			((EntityQuery)(ref m_AnimalQuery)).ResetFilter();
			((EntityQuery)(ref m_AnimalQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(num));
			JobHandle deps;
			JobHandle dependencies;
			JobHandle val = JobChunkExtensions.ScheduleParallel<UpdateTransformDataJob>(new UpdateTransformDataJob
			{
				m_AnimalType = InternalCompilerInterface.GetComponentTypeHandle<Animal>(ref __TypeHandle.__Game_Creatures_Animal_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NavigationType = InternalCompilerInterface.GetComponentTypeHandle<AnimalNavigation>(ref __TypeHandle.__Game_Creatures_AnimalNavigation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_StumblingType = InternalCompilerInterface.GetComponentTypeHandle<Stumbling>(ref __TypeHandle.__Game_Creatures_Stumbling_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MeshGroupType = InternalCompilerInterface.GetBufferTypeHandle<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MovingType = InternalCompilerInterface.GetComponentTypeHandle<Moving>(ref __TypeHandle.__Game_Objects_Moving_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformFrameType = InternalCompilerInterface.GetBufferTypeHandle<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OrphanData = InternalCompilerInterface.GetComponentLookup<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeGeometryData = InternalCompilerInterface.GetComponentLookup<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabAnimalData = InternalCompilerInterface.GetComponentLookup<AnimalData>(ref __TypeHandle.__Game_Prefabs_AnimalData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CharacterElements = InternalCompilerInterface.GetBufferLookup<CharacterElement>(ref __TypeHandle.__Game_Prefabs_CharacterElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AnimationClips = InternalCompilerInterface.GetBufferLookup<AnimationClip>(ref __TypeHandle.__Game_Prefabs_AnimationClip_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AnimationMotions = InternalCompilerInterface.GetBufferLookup<AnimationMotion>(ref __TypeHandle.__Game_Prefabs_AnimationMotion_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
				m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
				m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
				m_TransformFrameIndex = m_SimulationSystem.frameIndex / 16 % 4
			}, m_AnimalQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps, dependencies));
			m_TerrainSystem.AddCPUHeightReader(val);
			m_WaterSystem.AddSurfaceReader(val);
			m_NetSearchSystem.AddNetSearchTreeReader(val);
			((SystemBase)this).Dependency = val;
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
	public AnimalMoveSystem()
	{
	}
}

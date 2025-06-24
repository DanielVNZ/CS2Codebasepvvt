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
public class HumanMoveSystem : GameSystemBase
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
		public ComponentTypeHandle<Human> m_HumanType;

		[ReadOnly]
		public ComponentTypeHandle<Stumbling> m_StumblingType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> m_MeshGroupType;

		public ComponentTypeHandle<HumanNavigation> m_NavigationType;

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
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabGeometryData;

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
		public BufferLookup<ActivityLocationElement> m_ActivityLocations;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

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
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Human> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Human>(ref m_HumanType);
			NativeArray<CurrentVehicle> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentVehicle>(ref m_CurrentVehicleType);
			NativeArray<HumanNavigation> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanNavigation>(ref m_NavigationType);
			NativeArray<Moving> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
			NativeArray<Transform> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<PrefabRef> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<TransformFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TransformFrame>(ref m_TransformFrameType);
			BufferAccessor<MeshGroup> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<MeshGroup>(ref m_MeshGroupType);
			float num = 4f / 15f;
			int num2 = (int)m_TransformFrameIndex;
			int num3 = math.select((int)(m_TransformFrameIndex - 1), 3, m_TransformFrameIndex == 0);
			float3 targetDirection = default(float3);
			CurrentVehicle currentVehicle = default(CurrentVehicle);
			DynamicBuffer<MeshGroup> meshGroups = default(DynamicBuffer<MeshGroup>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Human human = nativeArray[i];
				HumanNavigation humanNavigation = nativeArray3[i];
				Moving moving = nativeArray4[i];
				Transform transform = nativeArray5[i];
				PrefabRef prefabRef = nativeArray6[i];
				DynamicBuffer<TransformFrame> val = bufferAccessor[i];
				TransformFrame transformFrame = val[num3];
				float3 val2 = humanNavigation.m_TargetPosition - transform.m_Position;
				MathUtils.TryNormalize(ref val2, humanNavigation.m_MaxSpeed);
				TransformFrame transformFrame2 = default(TransformFrame);
				if (humanNavigation.m_MaxSpeed >= 0.1f && humanNavigation.m_TargetActivity == 0)
				{
					transformFrame2.m_State = TransformState.Move;
				}
				else
				{
					transformFrame2.m_State = TransformState.Idle;
					transformFrame2.m_Activity = humanNavigation.m_TargetActivity;
				}
				((float3)(ref targetDirection))._002Ector(humanNavigation.m_TargetDirection.x, 0f, humanNavigation.m_TargetDirection.y);
				if (!((float3)(ref targetDirection)).Equals(default(float3)))
				{
					transform.m_Rotation = quaternion.LookRotation(targetDirection, math.up());
				}
				CollectionUtils.TryGet<CurrentVehicle>(nativeArray2, i, ref currentVehicle);
				CollectionUtils.TryGet<MeshGroup>(bufferAccessor2, i, ref meshGroups);
				ObjectUtils.UpdateAnimation(oldPropID: GetPropID(in transformFrame, currentVehicle), newPropID: GetPropID(in transformFrame2, currentVehicle), conditions: CreatureUtils.GetConditions(human), prefab: prefabRef.m_Prefab, timeStep: num, meshGroups: meshGroups, subMeshGroupBuffers: ref m_SubMeshGroups, characterElementBuffers: ref m_CharacterElements, subMeshBuffers: ref m_SubMeshes, animationClipBuffers: ref m_AnimationClips, animationMotionBuffers: ref m_AnimationMotions, maxSpeed: ref humanNavigation.m_MaxSpeed, activity: ref humanNavigation.m_TargetActivity, targetPosition: ref humanNavigation.m_TargetPosition, targetDirection: ref targetDirection, transform: ref transform, oldFrameData: ref transformFrame, newFrameData: ref transformFrame2);
				humanNavigation.m_TransformState = transformFrame2.m_State;
				humanNavigation.m_LastActivity = transformFrame2.m_Activity;
				if (math.any(((float3)(ref targetDirection)).xz != humanNavigation.m_TargetDirection))
				{
					humanNavigation.m_TargetDirection = math.normalizesafe(((float3)(ref targetDirection)).xz, default(float2));
					if (!((float3)(ref targetDirection)).Equals(default(float3)))
					{
						transform.m_Rotation = quaternion.LookRotation(targetDirection, math.up());
					}
				}
				float3 velocity = val2 * 8f + moving.m_Velocity;
				MathUtils.TryNormalize(ref velocity, humanNavigation.m_MaxSpeed);
				moving.m_Velocity = velocity;
				float3 val3 = moving.m_Velocity * (num * 0.5f);
				ref float3 position = ref transform.m_Position;
				position += val3;
				float2 xz = ((float3)(ref moving.m_Velocity)).xz;
				if (math.length(xz) >= 0.1f)
				{
					float2 val4 = math.normalize(xz);
					transform.m_Rotation = quaternion.LookRotation(new float3(val4.x, 0f, val4.y), math.up());
				}
				transformFrame2.m_Position = transform.m_Position;
				ref float3 velocity2 = ref transformFrame2.m_Velocity;
				velocity2 += moving.m_Velocity;
				transformFrame2.m_Rotation = transform.m_Rotation;
				ref float3 position2 = ref transform.m_Position;
				position2 += val3;
				val[num2] = transformFrame2;
				nativeArray3[i] = humanNavigation;
				nativeArray4[i] = moving;
				nativeArray5[i] = transform;
			}
		}

		private AnimatedPropID GetPropID(in TransformFrame transformFrame, CurrentVehicle currentVehicle)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			AnimatedPropID result = new AnimatedPropID(-1);
			PrefabRef prefabRef = default(PrefabRef);
			DynamicBuffer<ActivityLocationElement> val = default(DynamicBuffer<ActivityLocationElement>);
			if ((transformFrame.m_Activity == 10 || transformFrame.m_Activity == 11) && m_PrefabRefData.TryGetComponent(currentVehicle.m_Vehicle, ref prefabRef) && m_ActivityLocations.TryGetBuffer(prefabRef.m_Prefab, ref val) && val.Length != 0)
			{
				return val[0].m_PropID;
			}
			return result;
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
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0539: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<HumanNavigation> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanNavigation>(ref m_NavigationType);
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
					val2.a = ObjectUtils.LocalToWorld(transform, new float3(0f, objectGeometryData.m_Bounds.min.y + 0.2f, 0f));
					val2.b = ObjectUtils.LocalToWorld(transform, new float3(0f, objectGeometryData.m_Bounds.max.y - 0.2f, 0f));
					val2.c = ObjectUtils.LocalToWorld(transform, new float3(objectGeometryData.m_Bounds.min.x, objectGeometryData.m_Bounds.max.y * 0.5f, 0f));
					val2.d = ObjectUtils.LocalToWorld(transform, new float3(objectGeometryData.m_Bounds.max.x, objectGeometryData.m_Bounds.max.y * 0.5f, 0f));
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
		public ComponentTypeHandle<Human> __Game_Creatures_Human_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stumbling> __Game_Creatures_Stumbling_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferTypeHandle;

		public ComponentTypeHandle<HumanNavigation> __Game_Creatures_HumanNavigation_RW_ComponentTypeHandle;

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
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

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

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

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
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			__Game_Creatures_Human_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Human>(true);
			__Game_Creatures_Stumbling_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stumbling>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentVehicle>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Rendering_MeshGroup_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MeshGroup>(true);
			__Game_Creatures_HumanNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanNavigation>(false);
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
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_SubMeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(true);
			__Game_Prefabs_CharacterElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CharacterElement>(true);
			__Game_Prefabs_AnimationClip_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationClip>(true);
			__Game_Prefabs_AnimationMotion_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationMotion>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private TerrainSystem m_TerrainSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private EntityQuery m_HumanQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_HumanQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Human>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		uint index = m_SimulationSystem.frameIndex % 16;
		((EntityQuery)(ref m_HumanQuery)).ResetFilter();
		((EntityQuery)(ref m_HumanQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(index));
		JobHandle dependencies;
		JobHandle val = JobChunkExtensions.ScheduleParallel<UpdateTransformDataJob>(new UpdateTransformDataJob
		{
			m_HumanType = InternalCompilerInterface.GetComponentTypeHandle<Human>(ref __TypeHandle.__Game_Creatures_Human_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StumblingType = InternalCompilerInterface.GetComponentTypeHandle<Stumbling>(ref __TypeHandle.__Game_Creatures_Stumbling_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleType = InternalCompilerInterface.GetComponentTypeHandle<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MeshGroupType = InternalCompilerInterface.GetBufferTypeHandle<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationType = InternalCompilerInterface.GetComponentTypeHandle<HumanNavigation>(ref __TypeHandle.__Game_Creatures_HumanNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
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
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CharacterElements = InternalCompilerInterface.GetBufferLookup<CharacterElement>(ref __TypeHandle.__Game_Prefabs_CharacterElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimationClips = InternalCompilerInterface.GetBufferLookup<AnimationClip>(ref __TypeHandle.__Game_Prefabs_AnimationClip_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimationMotions = InternalCompilerInterface.GetBufferLookup<AnimationMotion>(ref __TypeHandle.__Game_Prefabs_AnimationMotion_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActivityLocations = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_TransformFrameIndex = m_SimulationSystem.frameIndex / 16 % 4
		}, m_HumanQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		m_TerrainSystem.AddCPUHeightReader(val);
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		((SystemBase)this).Dependency = val;
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
	public HumanMoveSystem()
	{
	}
}

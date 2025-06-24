using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
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
public class VehicleOutOfControlSystem : GameSystemBase
{
	[BurstCompile]
	private struct VehicleOutOfControlMoveJob : IJobChunk
	{
		private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public Quad3 m_CornerPositions;

			public Quad3 m_CornerPositions2;

			public float4 m_GroundHeights;

			public float4 m_GroundHeights2;

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
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				CheckCircle(center, radius, m_CornerPositions.a, ref m_GroundHeights.x);
				CheckCircle(center, radius, m_CornerPositions.b, ref m_GroundHeights.y);
				CheckCircle(center, radius, m_CornerPositions.c, ref m_GroundHeights.z);
				CheckCircle(center, radius, m_CornerPositions.d, ref m_GroundHeights.w);
				CheckCircle(center, radius, m_CornerPositions2.a, ref m_GroundHeights2.x);
				CheckCircle(center, radius, m_CornerPositions2.b, ref m_GroundHeights2.y);
				CheckCircle(center, radius, m_CornerPositions2.c, ref m_GroundHeights2.z);
				CheckCircle(center, radius, m_CornerPositions2.d, ref m_GroundHeights2.w);
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
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				CheckTriangle(triangle, m_CornerPositions.a, ref m_GroundHeights.x);
				CheckTriangle(triangle, m_CornerPositions.b, ref m_GroundHeights.y);
				CheckTriangle(triangle, m_CornerPositions.c, ref m_GroundHeights.z);
				CheckTriangle(triangle, m_CornerPositions.d, ref m_GroundHeights.w);
				CheckTriangle(triangle, m_CornerPositions2.a, ref m_GroundHeights2.x);
				CheckTriangle(triangle, m_CornerPositions2.b, ref m_GroundHeights2.y);
				CheckTriangle(triangle, m_CornerPositions2.c, ref m_GroundHeights2.z);
				CheckTriangle(triangle, m_CornerPositions2.d, ref m_GroundHeights2.w);
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
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

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
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public uint m_TransformFrameIndex;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0628: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Moving> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			BufferAccessor<TransformFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TransformFrame>(ref m_TransformFrameType);
			int num = 4;
			float num2 = 4f / 15f;
			float num3 = num2 / (float)num;
			float num4 = 10f;
			float num5 = math.pow(0.95f, num3);
			Quad3 val4 = default(Quad3);
			Quad3 val5 = default(Quad3);
			Quad3 val6 = default(Quad3);
			Quad3 val7 = default(Quad3);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				PrefabRef prefabRef = nativeArray[i];
				Moving moving = nativeArray2[i];
				Transform transform = nativeArray3[i];
				CarData carData = m_PrefabCarData[prefabRef.m_Prefab];
				ObjectGeometryData objectGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
				for (int j = 0; j < num; j++)
				{
					float3 momentOfInertia = ObjectUtils.CalculateMomentOfInertia(transform.m_Rotation, objectGeometryData.m_Size);
					float3 forward = math.forward(transform.m_Rotation);
					float3 val = transform.m_Position + math.mul(transform.m_Rotation, new float3(0f, objectGeometryData.m_Bounds.max.y * 0.25f, 0f));
					Quad3 val2 = ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, objectGeometryData.m_Bounds);
					Quad3 val3 = val2 + math.mul(transform.m_Rotation, new float3(0f, objectGeometryData.m_Bounds.max.y, 0f));
					GetGroundHeight(val2, val3, moving.m_Velocity, num2, num4, out var heights, out var heights2);
					val4.a = CalculatePointVelocityDelta(val2.a, val, moving, forward, carData.m_Braking, num3, heights.x, num4);
					val4.b = CalculatePointVelocityDelta(val2.b, val, moving, forward, carData.m_Braking, num3, heights.y, num4);
					val4.c = CalculatePointVelocityDelta(val2.c, val, moving, forward, carData.m_Braking, num3, heights.z, num4);
					val4.d = CalculatePointVelocityDelta(val2.d, val, moving, forward, carData.m_Braking, num3, heights.w, num4);
					val5.a = CalculatePointVelocityDelta(val3.a, val, moving, carData.m_Braking, num3, heights2.x, num4);
					val5.b = CalculatePointVelocityDelta(val3.b, val, moving, carData.m_Braking, num3, heights2.y, num4);
					val5.c = CalculatePointVelocityDelta(val3.c, val, moving, carData.m_Braking, num3, heights2.z, num4);
					val5.d = CalculatePointVelocityDelta(val3.d, val, moving, carData.m_Braking, num3, heights2.w, num4);
					val6.a = CalculatePointAngularVelocityDelta(val2.a, val, val4.a, momentOfInertia);
					val6.b = CalculatePointAngularVelocityDelta(val2.b, val, val4.b, momentOfInertia);
					val6.c = CalculatePointAngularVelocityDelta(val2.c, val, val4.c, momentOfInertia);
					val6.d = CalculatePointAngularVelocityDelta(val2.d, val, val4.d, momentOfInertia);
					val7.a = CalculatePointAngularVelocityDelta(val3.a, val, val5.a, momentOfInertia);
					val7.b = CalculatePointAngularVelocityDelta(val3.b, val, val5.b, momentOfInertia);
					val7.c = CalculatePointAngularVelocityDelta(val3.c, val, val5.c, momentOfInertia);
					val7.d = CalculatePointAngularVelocityDelta(val3.d, val, val5.d, momentOfInertia);
					float3 val8 = (val4.a + val4.b + val4.c + val4.d + val5.a + val5.b + val5.c + val5.d) * 0.125f;
					float3 val9 = (val6.a + val6.b + val6.c + val6.d + val7.a + val7.b + val7.c + val7.d) * 0.125f;
					val8.y -= num4 * num3;
					ref float3 velocity = ref moving.m_Velocity;
					velocity *= num5;
					ref float3 angularVelocity = ref moving.m_AngularVelocity;
					angularVelocity *= num5;
					ref float3 velocity2 = ref moving.m_Velocity;
					velocity2 += val8;
					ref float3 angularVelocity2 = ref moving.m_AngularVelocity;
					angularVelocity2 += val9;
					float num6 = math.length(moving.m_AngularVelocity);
					if (num6 > 1E-05f)
					{
						quaternion val10 = quaternion.AxisAngle(moving.m_AngularVelocity / num6, num6 * num3);
						transform.m_Rotation = math.normalize(math.mul(val10, transform.m_Rotation));
						float3 val11 = transform.m_Position + math.mul(transform.m_Rotation, new float3(0f, objectGeometryData.m_Bounds.max.y * 0.25f, 0f));
						ref float3 position = ref transform.m_Position;
						position += val - val11;
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
				DynamicBuffer<TransformFrame> val12 = bufferAccessor[i];
				val12[(int)m_TransformFrameIndex] = transformFrame;
				nativeArray2[i] = moving;
				nativeArray3[i] = transform;
			}
		}

		private float3 CalculatePointVelocityDelta(float3 position, float3 origin, Moving moving, float3 forward, float grip, float timeStep, float groundHeight, float gravity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			float3 val = ObjectUtils.CalculatePointVelocity(position - origin, moving);
			float num = math.dot(forward, val);
			float3 val2 = val - forward * (num * 0.5f);
			val2.y = 0f;
			float num2 = val.y - gravity * timeStep;
			position.y += num2 * timeStep;
			float num3 = math.max(0f, groundHeight - position.y) / timeStep;
			float3 result = MathUtils.ClampLength(-val2, grip * math.min(timeStep, num3 / gravity));
			result.y += num3;
			return result;
		}

		private float3 CalculatePointVelocityDelta(float3 position, float3 origin, Moving moving, float grip, float timeStep, float groundHeight, float gravity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			float3 val = ObjectUtils.CalculatePointVelocity(position - origin, moving);
			float3 val2 = val * 0.5f;
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

		private void GetGroundHeight(Quad3 cornerPositions, Quad3 cornerPositions2, float3 velocity, float timeStep, float gravity, out float4 heights, out float4 heights2)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			float4 val = default(float4);
			val.x = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cornerPositions.a);
			val.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cornerPositions.b);
			val.z = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cornerPositions.c);
			val.w = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cornerPositions.d);
			float4 val2 = default(float4);
			val2.x = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cornerPositions2.a);
			val2.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cornerPositions2.b);
			val2.z = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cornerPositions2.c);
			val2.w = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cornerPositions2.d);
			float4 val3 = default(float4);
			((float4)(ref val3))._002Ector(cornerPositions.a.y, cornerPositions.b.y, cornerPositions.c.y, cornerPositions.d.y);
			float4 val4 = default(float4);
			((float4)(ref val4))._002Ector(cornerPositions2.a.y, cornerPositions2.b.y, cornerPositions2.c.y, cornerPositions2.d.y);
			heights = math.select(float4.op_Implicit(float.MinValue), val, val < val3 + 4f);
			heights2 = math.select(float4.op_Implicit(float.MinValue), val2, val2 < val4 + 4f);
			NetIterator netIterator = new NetIterator
			{
				m_Bounds = (MathUtils.Bounds(cornerPositions) | MathUtils.Bounds(cornerPositions2))
			};
			netIterator.m_Bounds.min.y += (math.min(0f, velocity.y) - gravity * timeStep) * timeStep;
			netIterator.m_Bounds.max.y += math.max(0f, velocity.y) * timeStep;
			netIterator.m_CornerPositions = cornerPositions;
			netIterator.m_CornerPositions2 = cornerPositions2;
			netIterator.m_GroundHeights = heights;
			netIterator.m_GroundHeights2 = heights2;
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
			heights2 = netIterator.m_GroundHeights2;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

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
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

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
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
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
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private TerrainSystem m_TerrainSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private EntityQuery m_VehicleQuery;

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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<OutOfControl>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<Transform>(),
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_VehicleQuery);
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
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		uint index = m_SimulationSystem.frameIndex & 0xF;
		((EntityQuery)(ref m_VehicleQuery)).ResetFilter();
		((EntityQuery)(ref m_VehicleQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(index));
		JobHandle dependencies;
		JobHandle val = JobChunkExtensions.ScheduleParallel<VehicleOutOfControlMoveJob>(new VehicleOutOfControlMoveJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
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
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_TransformFrameIndex = m_SimulationSystem.frameIndex / 16 % 4
		}, m_VehicleQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
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
	public VehicleOutOfControlSystem()
	{
	}
}

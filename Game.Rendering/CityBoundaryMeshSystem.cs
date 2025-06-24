using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Common;
using Game.Prefabs;
using Game.Serialization;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class CityBoundaryMeshSystem : GameSystemBase, IPreDeserialize
{
	private struct Boundary
	{
		public Segment m_Line;

		public Color32 m_Color;
	}

	[BurstCompile]
	private struct FillBoundaryQueueJob : IJobChunk
	{
		private struct Iterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public Segment m_Line;

			public Entity m_Area;

			public NativeParallelHashSet<Entity> m_StartTiles;

			public ComponentLookup<MapTile> m_MapTileData;

			public ComponentLookup<Native> m_NativeData;

			public BufferLookup<Node> m_Nodes;

			public BufferLookup<Triangle> m_Triangles;

			public bool m_EditorMode;

			public bool m_IsNative;

			public bool m_TileFound;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				return MathUtils.Intersect(MathUtils.Expand(((Bounds3)(ref bounds.m_Bounds)).xz, float2.op_Implicit(1f)), m_Line, ref val);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				if (!MathUtils.Intersect(MathUtils.Expand(((Bounds3)(ref bounds.m_Bounds)).xz, float2.op_Implicit(1f)), m_Line, ref val) || m_Area == areaItem.m_Area || !m_MapTileData.HasComponent(areaItem.m_Area))
				{
					return;
				}
				if (!m_IsNative)
				{
					if (m_EditorMode)
					{
						if (!m_StartTiles.Contains(areaItem.m_Area))
						{
							return;
						}
					}
					else if (m_NativeData.HasComponent(areaItem.m_Area))
					{
						return;
					}
				}
				DynamicBuffer<Node> nodes = m_Nodes[areaItem.m_Area];
				Triangle triangle = m_Triangles[areaItem.m_Area][areaItem.m_Triangle];
				Triangle2 triangle2 = AreaUtils.GetTriangle2(nodes, triangle);
				bool3 val2 = AreaUtils.IsEdge(nodes, triangle);
				if (val2.x)
				{
					CheckLine(((Triangle2)(ref triangle2)).ab);
				}
				if (val2.y)
				{
					CheckLine(((Triangle2)(ref triangle2)).bc);
				}
				if (val2.z)
				{
					CheckLine(((Triangle2)(ref triangle2)).ca);
				}
			}

			private void CheckLine(Segment line)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				m_TileFound |= (math.distancesq(line.a, m_Line.a) < 1f && math.distancesq(line.b, m_Line.b) < 1f) || (math.distancesq(line.b, m_Line.a) < 1f && math.distancesq(line.a, m_Line.b) < 1f);
			}
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Native> m_NativeType;

		[ReadOnly]
		public BufferTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentLookup<MapTile> m_MapTileData;

		[ReadOnly]
		public ComponentLookup<Native> m_NativeData;

		[ReadOnly]
		public BufferLookup<Node> m_Nodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_SearchTree;

		[ReadOnly]
		public NativeList<Entity> m_StartTiles;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public Color32 m_CityBorderColor;

		[ReadOnly]
		public Color32 m_MapBorderColor;

		public ParallelWriter<Boundary> m_BoundaryQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<Node> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Node>(ref m_NodeType);
			if (m_EditorMode)
			{
				NativeParallelHashSet<Entity> startTiles = default(NativeParallelHashSet<Entity>);
				startTiles._002Ector(m_StartTiles.Length, AllocatorHandle.op_Implicit((Allocator)2));
				for (int i = 0; i < m_StartTiles.Length; i++)
				{
					startTiles.Add(m_StartTiles[i]);
				}
				Segment val3 = default(Segment);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					bool isNative = !startTiles.Contains(val);
					DynamicBuffer<Node> val2 = bufferAccessor[j];
					if (val2.Length >= 2)
					{
						val3.a = val2[val2.Length - 1].m_Position;
						for (int k = 0; k < val2.Length; k++)
						{
							val3.b = val2[k].m_Position;
							CheckLine(val, val3, isNative, startTiles);
							val3.a = val3.b;
						}
					}
				}
				startTiles.Dispose();
				return;
			}
			bool isNative2 = ((ArchetypeChunk)(ref chunk)).Has<Native>(ref m_NativeType);
			Segment val5 = default(Segment);
			for (int l = 0; l < nativeArray.Length; l++)
			{
				Entity area = nativeArray[l];
				DynamicBuffer<Node> val4 = bufferAccessor[l];
				if (val4.Length >= 2)
				{
					val5.a = val4[val4.Length - 1].m_Position;
					for (int m = 0; m < val4.Length; m++)
					{
						val5.b = val4[m].m_Position;
						CheckLine(area, val5, isNative2, default(NativeParallelHashSet<Entity>));
						val5.a = val5.b;
					}
				}
			}
		}

		private void CheckLine(Entity area, Segment line, bool isNative, NativeParallelHashSet<Entity> startTiles)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			Iterator iterator = new Iterator
			{
				m_Line = ((Segment)(ref line)).xz,
				m_Area = area,
				m_StartTiles = startTiles,
				m_MapTileData = m_MapTileData,
				m_NativeData = m_NativeData,
				m_Nodes = m_Nodes,
				m_Triangles = m_Triangles,
				m_EditorMode = m_EditorMode,
				m_IsNative = isNative
			};
			m_SearchTree.Iterate<Iterator>(ref iterator, 0);
			if (!iterator.m_TileFound)
			{
				m_BoundaryQueue.Enqueue(new Boundary
				{
					m_Line = line,
					m_Color = (isNative ? m_MapBorderColor : m_CityBorderColor)
				});
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FillBoundaryMeshDataJob : IJob
	{
		[ReadOnly]
		public float m_Width;

		[ReadOnly]
		public float m_TilingLength;

		public NativeQueue<Boundary> m_BoundaryQueue;

		public NativeList<float3> m_Vertices;

		public NativeList<float2> m_UVs;

		public NativeList<Color32> m_Colors;

		public NativeList<int> m_Indices;

		public NativeValue<Bounds3> m_Bounds;

		public void Execute()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			int count = m_BoundaryQueue.Count;
			m_Vertices.Clear();
			m_UVs.Clear();
			m_Colors.Clear();
			m_Indices.Clear();
			int num = 0;
			Bounds3 val = default(Bounds3);
			((Bounds3)(ref val))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			for (int i = 0; i < count; i++)
			{
				Boundary boundary = m_BoundaryQueue.Dequeue();
				float num2 = MathUtils.Length(boundary.m_Line);
				if (num2 >= 1f)
				{
					float3 val2 = default(float3);
					float3 val3 = ((Segment)(ref boundary.m_Line)).ab;
					((float3)(ref val2)).xz = MathUtils.Right(((float3)(ref val3)).xz) * (m_Width * 0.5f / num2);
					int num3 = math.max(1, Mathf.RoundToInt(num2 / m_TilingLength));
					float num4 = 1f / (float)num3;
					val |= boundary.m_Line.a + val2;
					val |= boundary.m_Line.a - val2;
					val |= boundary.m_Line.b + val2;
					val |= boundary.m_Line.b - val2;
					for (int j = 0; j < num3; j++)
					{
						float2 val4 = new float2((float)j + 0.25f, (float)j + 0.75f) * num4;
						Segment val5 = MathUtils.Cut(boundary.m_Line, val4);
						m_Indices.Add(ref num);
						ref NativeList<int> reference = ref m_Indices;
						int num5 = num + 1;
						reference.Add(ref num5);
						ref NativeList<int> reference2 = ref m_Indices;
						num5 = num + 2;
						reference2.Add(ref num5);
						ref NativeList<int> reference3 = ref m_Indices;
						num5 = num + 2;
						reference3.Add(ref num5);
						ref NativeList<int> reference4 = ref m_Indices;
						num5 = num + 1;
						reference4.Add(ref num5);
						ref NativeList<int> reference5 = ref m_Indices;
						num5 = num + 3;
						reference5.Add(ref num5);
						ref NativeList<float3> reference6 = ref m_Vertices;
						val3 = val5.a + val2;
						reference6.Add(ref val3);
						ref NativeList<float2> reference7 = ref m_UVs;
						float2 val6 = new float2(1f, 0f);
						reference7.Add(ref val6);
						m_Colors.Add(ref boundary.m_Color);
						ref NativeList<float3> reference8 = ref m_Vertices;
						val3 = val5.a - val2;
						reference8.Add(ref val3);
						ref NativeList<float2> reference9 = ref m_UVs;
						val6 = new float2(0f, 0f);
						reference9.Add(ref val6);
						m_Colors.Add(ref boundary.m_Color);
						ref NativeList<float3> reference10 = ref m_Vertices;
						val3 = val5.b + val2;
						reference10.Add(ref val3);
						ref NativeList<float2> reference11 = ref m_UVs;
						val6 = new float2(1f, 1f);
						reference11.Add(ref val6);
						m_Colors.Add(ref boundary.m_Color);
						ref NativeList<float3> reference12 = ref m_Vertices;
						val3 = val5.b - val2;
						reference12.Add(ref val3);
						ref NativeList<float2> reference13 = ref m_UVs;
						val6 = new float2(0f, 1f);
						reference13.Add(ref val6);
						m_Colors.Add(ref boundary.m_Color);
						num += 4;
					}
				}
			}
			m_Bounds.value = val;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Native> __Game_Common_Native_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Node> __Game_Areas_Node_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<MapTile> __Game_Areas_MapTile_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Native_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Native>(true);
			__Game_Areas_Node_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Node>(true);
			__Game_Areas_MapTile_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MapTile>(true);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
		}
	}

	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private MapTileSystem m_MapTileSystem;

	private TerrainSystem m_TerrainSystem;

	private SearchSystem m_AreaSearchSystem;

	private EntityQuery m_UpdatedQuery;

	private EntityQuery m_MapTileQuery;

	private EntityQuery m_SettingsQuery;

	private Mesh m_BoundaryMesh;

	private Material m_BoundaryMaterial;

	private JobHandle m_MeshDependencies;

	private NativeList<float3> m_Vertices;

	private NativeList<float2> m_UVs;

	private NativeList<Color32> m_Colors;

	private NativeList<int> m_Indices;

	private NativeValue<Bounds3> m_Bounds;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_MapTileSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapTileSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<MapTile>(),
			ComponentType.ReadOnly<Area>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_MapTileQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<MapTile>(),
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<Node>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_SettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CityBoundaryData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_SettingsQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		Clear();
		base.OnDestroy();
	}

	public void PreDeserialize(Context context)
	{
		Clear();
		m_Loaded = true;
	}

	private void Clear()
	{
		DisposeMeshData();
		DestroyMesh();
		m_BoundaryMaterial = null;
	}

	private void DestroyMesh()
	{
		if ((Object)(object)m_BoundaryMesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_BoundaryMesh);
			m_BoundaryMesh = null;
		}
	}

	private void DisposeMeshData()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (m_Vertices.IsCreated)
		{
			((JobHandle)(ref m_MeshDependencies)).Complete();
			m_MeshDependencies = default(JobHandle);
			m_Vertices.Dispose();
			m_UVs.Dispose();
			m_Colors.Dispose();
			m_Indices.Dispose();
			m_Bounds.Dispose();
		}
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
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		if (GetLoaded() || !((EntityQuery)(ref m_UpdatedQuery)).IsEmptyIgnoreFilter)
		{
			DisposeMeshData();
			CityBoundaryPrefab prefab = m_PrefabSystem.GetPrefab<CityBoundaryPrefab>(((EntityQuery)(ref m_SettingsQuery)).GetSingletonEntity());
			m_BoundaryMaterial = prefab.m_Material;
			NativeQueue<Boundary> boundaryQueue = default(NativeQueue<Boundary>);
			boundaryQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			m_Vertices = new NativeList<float3>(AllocatorHandle.op_Implicit((Allocator)3));
			m_UVs = new NativeList<float2>(AllocatorHandle.op_Implicit((Allocator)3));
			m_Colors = new NativeList<Color32>(AllocatorHandle.op_Implicit((Allocator)3));
			m_Indices = new NativeList<int>(AllocatorHandle.op_Implicit((Allocator)3));
			m_Bounds = new NativeValue<Bounds3>((Allocator)3);
			JobHandle dependencies;
			FillBoundaryQueueJob fillBoundaryQueueJob = new FillBoundaryQueueJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NativeType = InternalCompilerInterface.GetComponentTypeHandle<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeType = InternalCompilerInterface.GetBufferTypeHandle<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MapTileData = InternalCompilerInterface.GetComponentLookup<MapTile>(ref __TypeHandle.__Game_Areas_MapTile_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NativeData = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Nodes = InternalCompilerInterface.GetBufferLookup<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies),
				m_StartTiles = m_MapTileSystem.GetStartTiles(),
				m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
				m_CityBorderColor = Color32.op_Implicit(((Color)(ref prefab.m_CityBorderColor)).linear),
				m_MapBorderColor = Color32.op_Implicit(((Color)(ref prefab.m_MapBorderColor)).linear),
				m_BoundaryQueue = boundaryQueue.AsParallelWriter()
			};
			FillBoundaryMeshDataJob obj = new FillBoundaryMeshDataJob
			{
				m_Width = prefab.m_Width,
				m_TilingLength = prefab.m_TilingLength,
				m_BoundaryQueue = boundaryQueue,
				m_Vertices = m_Vertices,
				m_UVs = m_UVs,
				m_Colors = m_Colors,
				m_Indices = m_Indices,
				m_Bounds = m_Bounds
			};
			JobHandle val = JobChunkExtensions.ScheduleParallel<FillBoundaryQueueJob>(fillBoundaryQueueJob, m_MapTileQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
			JobHandle val2 = IJobExtensions.Schedule<FillBoundaryMeshDataJob>(obj, val);
			boundaryQueue.Dispose(val2);
			m_AreaSearchSystem.AddSearchTreeReader(val);
			m_MeshDependencies = val2;
			((SystemBase)this).Dependency = val;
		}
	}

	public bool GetBoundaryMesh(out Mesh mesh, out Material material)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		if (m_Vertices.IsCreated)
		{
			((JobHandle)(ref m_MeshDependencies)).Complete();
			m_MeshDependencies = default(JobHandle);
			if (m_Vertices.Length != 0)
			{
				if ((Object)(object)m_BoundaryMesh == (Object)null)
				{
					m_BoundaryMesh = new Mesh();
					((Object)m_BoundaryMesh).name = "City boundaries";
				}
				else
				{
					m_BoundaryMesh.Clear();
				}
				m_BoundaryMesh.SetVertices<float3>(m_Vertices.AsArray());
				m_BoundaryMesh.SetUVs<float2>(0, m_UVs.AsArray());
				m_BoundaryMesh.SetColors<Color32>(m_Colors.AsArray());
				m_BoundaryMesh.SetIndices<int>(m_Indices.AsArray(), (MeshTopology)0, 0, false, 0);
				float2 heightScaleOffset = m_TerrainSystem.heightScaleOffset;
				Bounds3 value = m_Bounds.value;
				value.min.y = heightScaleOffset.y;
				value.max.y = heightScaleOffset.y + heightScaleOffset.x;
				m_BoundaryMesh.bounds = RenderingUtils.ToBounds(value);
			}
			else
			{
				DestroyMesh();
			}
			m_Vertices.Dispose();
			m_UVs.Dispose();
			m_Colors.Dispose();
			m_Indices.Dispose();
			m_Bounds.Dispose();
		}
		mesh = m_BoundaryMesh;
		material = m_BoundaryMaterial;
		return (Object)(object)m_BoundaryMesh != (Object)null;
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
	public CityBoundaryMeshSystem()
	{
	}
}

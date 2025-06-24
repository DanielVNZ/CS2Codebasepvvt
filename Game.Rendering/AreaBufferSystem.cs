using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Common;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Serialization;
using Game.Tools;
using Game.UI;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class AreaBufferSystem : GameSystemBase, IPreDeserialize
{
	private struct AreaTriangleData
	{
		public Vector3 m_APos;

		public Vector3 m_BPos;

		public Vector3 m_CPos;

		public Vector2 m_APrevXZ;

		public Vector2 m_BPrevXZ;

		public Vector2 m_CPrevXZ;

		public Vector2 m_ANextXZ;

		public Vector2 m_BNextXZ;

		public Vector2 m_CNextXZ;

		public Vector2 m_YMinMax;

		public Vector4 m_FillColor;

		public Vector4 m_EdgeColor;
	}

	private struct MaterialData
	{
		public Material m_Material;

		public bool m_HasMesh;
	}

	private class AreaTypeData
	{
		public EntityQuery m_UpdatedQuery;

		public EntityQuery m_AreaQuery;

		public NativeList<AreaTriangleData> m_BufferData;

		public NativeValue<Bounds3> m_Bounds;

		public JobHandle m_DataDependencies;

		public ComputeBuffer m_Buffer;

		public Material m_Material;

		public Material m_OriginalNameMaterial;

		public List<MaterialData> m_NameMaterials;

		public Mesh m_NameMesh;

		public MeshDataArray m_NameMeshData;

		public bool m_BufferDataDirty;

		public bool m_BufferDirty;

		public bool m_HasNameMeshData;

		public bool m_HasNameMesh;
	}

	private struct ChunkData
	{
		public int m_TriangleOffset;

		public Bounds3 m_Bounds;
	}

	[BurstCompile]
	private struct ResetChunkDataJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<Area> m_AreaType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Hidden> m_HiddenType;

		[ReadOnly]
		public BufferTypeHandle<Triangle> m_TriangleType;

		public NativeList<ChunkData> m_ChunkData;

		public NativeList<AreaTriangleData> m_AreaTriangleData;

		public void Execute()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			m_ChunkData.ResizeUninitialized(m_Chunks.Length);
			ChunkData chunkData = new ChunkData
			{
				m_Bounds = new Bounds3(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue))
			};
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				m_ChunkData[i] = chunkData;
				ArchetypeChunk val = m_Chunks[i];
				if (((ArchetypeChunk)(ref val)).Has<Hidden>(ref m_HiddenType))
				{
					continue;
				}
				NativeArray<Area> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<Area>(ref m_AreaType);
				NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Temp>(ref m_TempType);
				BufferAccessor<Triangle> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Triangle>(ref m_TriangleType);
				for (int j = 0; j < bufferAccessor.Length; j++)
				{
					if ((nativeArray[j].m_Flags & AreaFlags.Slave) == 0 && (nativeArray2.Length == 0 || (nativeArray2[j].m_Flags & TempFlags.Hidden) == 0))
					{
						DynamicBuffer<Triangle> val2 = bufferAccessor[j];
						chunkData.m_TriangleOffset += val2.Length;
					}
				}
			}
			m_AreaTriangleData.ResizeUninitialized(chunkData.m_TriangleOffset);
		}
	}

	[BurstCompile]
	private struct FillMeshDataJob : IJobParallelForDefer
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Hidden> m_HiddenType;

		[ReadOnly]
		public ComponentTypeHandle<Area> m_AreaType;

		[ReadOnly]
		public ComponentTypeHandle<Native> m_NativeType;

		[ReadOnly]
		public BufferTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public BufferTypeHandle<Triangle> m_TriangleType;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_GeometryData;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.AreaColorData> m_ColorData;

		[ReadOnly]
		public BufferLookup<SelectionElement> m_SelectionElements;

		[ReadOnly]
		public Entity m_SelectionEntity;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_Chunks;

		[NativeDisableParallelForRestriction]
		public NativeList<ChunkData> m_ChunkData;

		[NativeDisableParallelForRestriction]
		public NativeList<AreaTriangleData> m_AreaTriangleData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			ArchetypeChunk val = m_Chunks[index];
			if (((ArchetypeChunk)(ref val)).Has<Hidden>(ref m_HiddenType))
			{
				return;
			}
			ChunkData chunkData = m_ChunkData[index];
			NativeArray<Area> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<Area>(ref m_AreaType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<Node> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Node>(ref m_NodeType);
			BufferAccessor<Triangle> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Triangle>(ref m_TriangleType);
			bool flag = m_EditorMode || ((ArchetypeChunk)(ref val)).Has<Native>(ref m_NativeType);
			Color val5;
			if (m_SelectionElements.HasBuffer(m_SelectionEntity))
			{
				DynamicBuffer<SelectionElement> val2 = m_SelectionElements[m_SelectionEntity];
				NativeParallelHashSet<Entity> val3 = default(NativeParallelHashSet<Entity>);
				val3._002Ector(val2.Length, AllocatorHandle.op_Implicit((Allocator)2));
				for (int i = 0; i < val2.Length; i++)
				{
					val3.Add(val2[i].m_Entity);
				}
				NativeArray<Entity> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				Game.Prefabs.AreaColorData defaults = default(Game.Prefabs.AreaColorData);
				for (int j = 0; j < nativeArray4.Length; j++)
				{
					if ((nativeArray[j].m_Flags & AreaFlags.Slave) != 0)
					{
						continue;
					}
					PrefabRef prefabRef = nativeArray3[j];
					DynamicBuffer<Node> nodes = bufferAccessor[j];
					DynamicBuffer<Triangle> triangles = bufferAccessor2[j];
					AreaGeometryData geometryData = m_GeometryData[prefabRef.m_Prefab];
					if (!m_ColorData.TryGetComponent(prefabRef.m_Prefab, ref defaults))
					{
						defaults = Game.Prefabs.AreaColorData.GetDefaults();
					}
					Entity val4;
					if (nativeArray2.Length != 0)
					{
						Temp temp = nativeArray2[j];
						if ((temp.m_Flags & TempFlags.Hidden) != 0)
						{
							continue;
						}
						val4 = temp.m_Original;
					}
					else
					{
						val4 = nativeArray4[j];
					}
					Color val6;
					Color val7;
					if (val3.Contains(val4))
					{
						val5 = Color32.op_Implicit(defaults.m_SelectionFillColor);
						val6 = ((Color)(ref val5)).linear;
						val5 = Color32.op_Implicit(defaults.m_SelectionEdgeColor);
						val7 = ((Color)(ref val5)).linear;
					}
					else
					{
						val5 = Color32.op_Implicit(defaults.m_FillColor);
						val6 = ((Color)(ref val5)).linear;
						val5 = Color32.op_Implicit(defaults.m_EdgeColor);
						val7 = ((Color)(ref val5)).linear;
					}
					if (!flag)
					{
						val6 = GetDisabledColor(val6);
						val7 = GetDisabledColor(val7);
					}
					AddTriangles(nodes, triangles, Color.op_Implicit(val6), Color.op_Implicit(val7), geometryData, ref chunkData);
				}
				val3.Dispose();
			}
			else
			{
				Game.Prefabs.AreaColorData defaults2 = default(Game.Prefabs.AreaColorData);
				for (int k = 0; k < bufferAccessor.Length; k++)
				{
					if ((nativeArray[k].m_Flags & AreaFlags.Slave) == 0 && (nativeArray2.Length == 0 || (nativeArray2[k].m_Flags & TempFlags.Hidden) == 0))
					{
						PrefabRef prefabRef2 = nativeArray3[k];
						DynamicBuffer<Node> nodes2 = bufferAccessor[k];
						DynamicBuffer<Triangle> triangles2 = bufferAccessor2[k];
						AreaGeometryData geometryData2 = m_GeometryData[prefabRef2.m_Prefab];
						if (!m_ColorData.TryGetComponent(prefabRef2.m_Prefab, ref defaults2))
						{
							defaults2 = Game.Prefabs.AreaColorData.GetDefaults();
						}
						val5 = Color32.op_Implicit(defaults2.m_FillColor);
						Color val8 = ((Color)(ref val5)).linear;
						val5 = Color32.op_Implicit(defaults2.m_EdgeColor);
						Color val9 = ((Color)(ref val5)).linear;
						if (!flag)
						{
							val8 = GetDisabledColor(val8);
							val9 = GetDisabledColor(val9);
						}
						AddTriangles(nodes2, triangles2, Color.op_Implicit(val8), Color.op_Implicit(val9), geometryData2, ref chunkData);
					}
				}
			}
			m_ChunkData[index] = chunkData;
		}

		private static Color GetDisabledColor(Color color)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			color.a *= 0.25f;
			return color;
		}

		private void AddTriangles(DynamicBuffer<Node> nodes, DynamicBuffer<Triangle> triangles, Vector4 fillColor, Vector4 edgeColor, AreaGeometryData geometryData, ref ChunkData chunkData)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < triangles.Length; i++)
			{
				Triangle triangle = triangles[i];
				Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
				Bounds3 val = MathUtils.Bounds(triangle2);
				int3 val2 = math.select(triangle.m_Indices - 1, int3.op_Implicit(nodes.Length - 1), triangle.m_Indices == 0);
				int3 val3 = math.select(triangle.m_Indices + 1, int3.op_Implicit(0), triangle.m_Indices == nodes.Length - 1);
				val.min.y += triangle.m_HeightRange.min - geometryData.m_SnapDistance * 2f;
				val.max.y += triangle.m_HeightRange.max + geometryData.m_SnapDistance * 2f;
				AreaTriangleData areaTriangleData = new AreaTriangleData
				{
					m_APos = float3.op_Implicit(triangle2.a),
					m_BPos = float3.op_Implicit(triangle2.b),
					m_CPos = float3.op_Implicit(triangle2.c)
				};
				Node node = nodes[val2.x];
				areaTriangleData.m_APrevXZ = float2.op_Implicit(((float3)(ref node.m_Position)).xz);
				node = nodes[val2.y];
				areaTriangleData.m_BPrevXZ = float2.op_Implicit(((float3)(ref node.m_Position)).xz);
				node = nodes[val2.z];
				areaTriangleData.m_CPrevXZ = float2.op_Implicit(((float3)(ref node.m_Position)).xz);
				node = nodes[val3.x];
				areaTriangleData.m_ANextXZ = float2.op_Implicit(((float3)(ref node.m_Position)).xz);
				node = nodes[val3.y];
				areaTriangleData.m_BNextXZ = float2.op_Implicit(((float3)(ref node.m_Position)).xz);
				node = nodes[val3.z];
				areaTriangleData.m_CNextXZ = float2.op_Implicit(((float3)(ref node.m_Position)).xz);
				areaTriangleData.m_YMinMax.x = val.min.y;
				areaTriangleData.m_YMinMax.y = val.max.y;
				areaTriangleData.m_FillColor = fillColor;
				areaTriangleData.m_EdgeColor = edgeColor;
				m_AreaTriangleData[chunkData.m_TriangleOffset++] = areaTriangleData;
				ref Bounds3 reference = ref chunkData.m_Bounds;
				reference |= val;
			}
		}
	}

	[BurstCompile]
	private struct CalculateBoundsJob : IJob
	{
		[ReadOnly]
		public NativeList<ChunkData> m_ChunkData;

		public NativeValue<Bounds3> m_Bounds;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			Bounds3 val = default(Bounds3);
			((Bounds3)(ref val))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			for (int i = 0; i < m_ChunkData.Length; i++)
			{
				val |= m_ChunkData[i].m_Bounds;
			}
			m_Bounds.value = val;
		}
	}

	private struct LabelVertexData
	{
		public Vector3 m_Position;

		public Color32 m_Color;

		public Vector2 m_UV0;

		public Vector2 m_UV1;

		public Vector3 m_UV2;
	}

	[BurstCompile]
	private struct FillNameDataJob : IJob
	{
		[ReadOnly]
		public ComponentTypeHandle<Geometry> m_GeometryType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Hidden> m_HiddenType;

		[ReadOnly]
		public BufferTypeHandle<LabelVertex> m_LabelVertexType;

		[ReadOnly]
		public ComponentLookup<AreaNameData> m_AreaNameData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public int m_SubMeshCount;

		public MeshDataArray m_NameMeshData;

		public void Execute()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<int> val = default(NativeArray<int>);
			val._002Ector(m_SubMeshCount, (Allocator)2, (NativeArrayOptions)1);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val2 = m_Chunks[i];
				if (((ArchetypeChunk)(ref val2)).Has<Hidden>(ref m_HiddenType))
				{
					continue;
				}
				BufferAccessor<LabelVertex> bufferAccessor = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<LabelVertex>(ref m_LabelVertexType);
				for (int j = 0; j < bufferAccessor.Length; j++)
				{
					DynamicBuffer<LabelVertex> val3 = bufferAccessor[j];
					for (int k = 0; k < val3.Length; k += 4)
					{
						int material = val3[k].m_Material;
						CollectionUtils.ElementAt<int>(val, material) += 4;
					}
				}
			}
			int num = 0;
			for (int l = 0; l < m_SubMeshCount; l++)
			{
				num += val[l];
			}
			MeshData val4 = ((MeshDataArray)(ref m_NameMeshData))[0];
			NativeArray<VertexAttributeDescriptor> val5 = default(NativeArray<VertexAttributeDescriptor>);
			val5._002Ector(5, (Allocator)2, (NativeArrayOptions)0);
			val5[0] = new VertexAttributeDescriptor((VertexAttribute)0, (VertexAttributeFormat)0, 3, 0);
			val5[1] = new VertexAttributeDescriptor((VertexAttribute)3, (VertexAttributeFormat)2, 4, 0);
			val5[2] = new VertexAttributeDescriptor((VertexAttribute)4, (VertexAttributeFormat)0, 2, 0);
			val5[3] = new VertexAttributeDescriptor((VertexAttribute)5, (VertexAttributeFormat)0, 2, 0);
			val5[4] = new VertexAttributeDescriptor((VertexAttribute)6, (VertexAttributeFormat)0, 3, 0);
			((MeshData)(ref val4)).SetVertexBufferParams(num, val5);
			((MeshData)(ref val4)).SetIndexBufferParams((num >> 2) * 6, (IndexFormat)1);
			val5.Dispose();
			num = 0;
			((MeshData)(ref val4)).subMeshCount = m_SubMeshCount;
			for (int m = 0; m < m_SubMeshCount; m++)
			{
				ref int reference = ref CollectionUtils.ElementAt<int>(val, m);
				int num2 = m;
				SubMeshDescriptor val6 = default(SubMeshDescriptor);
				((SubMeshDescriptor)(ref val6)).firstVertex = num;
				((SubMeshDescriptor)(ref val6)).indexStart = (num >> 2) * 6;
				((SubMeshDescriptor)(ref val6)).vertexCount = reference;
				((SubMeshDescriptor)(ref val6)).indexCount = (reference >> 2) * 6;
				((SubMeshDescriptor)(ref val6)).topology = (MeshTopology)0;
				((MeshData)(ref val4)).SetSubMesh(num2, val6, (MeshUpdateFlags)13);
				num += reference;
				reference = 0;
			}
			NativeArray<LabelVertexData> vertexData = ((MeshData)(ref val4)).GetVertexData<LabelVertexData>(0);
			NativeArray<uint> indexData = ((MeshData)(ref val4)).GetIndexData<uint>();
			AreaNameData defaults = default(AreaNameData);
			LabelVertexData labelVertexData = default(LabelVertexData);
			for (int n = 0; n < m_Chunks.Length; n++)
			{
				ArchetypeChunk val7 = m_Chunks[n];
				if (((ArchetypeChunk)(ref val7)).Has<Hidden>(ref m_HiddenType))
				{
					continue;
				}
				NativeArray<Geometry> nativeArray = ((ArchetypeChunk)(ref val7)).GetNativeArray<Geometry>(ref m_GeometryType);
				NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val7)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref val7)).GetNativeArray<Temp>(ref m_TempType);
				BufferAccessor<LabelVertex> bufferAccessor2 = ((ArchetypeChunk)(ref val7)).GetBufferAccessor<LabelVertex>(ref m_LabelVertexType);
				for (int num3 = 0; num3 < bufferAccessor2.Length; num3++)
				{
					Geometry geometry = nativeArray[num3];
					PrefabRef prefabRef = nativeArray2[num3];
					DynamicBuffer<LabelVertex> val8 = bufferAccessor2[num3];
					if (!m_AreaNameData.TryGetComponent(prefabRef.m_Prefab, ref defaults))
					{
						defaults = AreaNameData.GetDefaults();
					}
					float3 val9 = AreaUtils.CalculateLabelPosition(geometry);
					Color32 val10 = defaults.m_Color;
					if (nativeArray3.Length != 0 && (nativeArray3[num3].m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace)) != 0)
					{
						val10 = defaults.m_SelectedColor;
					}
					SubMeshDescriptor val11 = default(SubMeshDescriptor);
					int num4 = -1;
					for (int num5 = 0; num5 < val8.Length; num5 += 4)
					{
						int material2 = val8[num5].m_Material;
						ref int reference2 = ref CollectionUtils.ElementAt<int>(val, material2);
						if (material2 != num4)
						{
							val11 = ((MeshData)(ref val4)).GetSubMesh(material2);
							num4 = material2;
						}
						int num6 = ((SubMeshDescriptor)(ref val11)).firstVertex + reference2;
						int num7 = ((SubMeshDescriptor)(ref val11)).indexStart + (reference2 >> 2) * 6;
						reference2 += 4;
						indexData[num7] = (uint)num6;
						indexData[num7 + 1] = (uint)(num6 + 1);
						indexData[num7 + 2] = (uint)(num6 + 2);
						indexData[num7 + 3] = (uint)(num6 + 2);
						indexData[num7 + 4] = (uint)(num6 + 3);
						indexData[num7 + 5] = (uint)num6;
						for (int num8 = 0; num8 < 4; num8++)
						{
							LabelVertex labelVertex = val8[num5 + num8];
							labelVertexData.m_Position = float3.op_Implicit(labelVertex.m_Position);
							labelVertexData.m_Color = new Color32((byte)(labelVertex.m_Color.r * val10.r >> 8), (byte)(labelVertex.m_Color.g * val10.g >> 8), (byte)(labelVertex.m_Color.b * val10.b >> 8), (byte)(labelVertex.m_Color.a * val10.a >> 8));
							labelVertexData.m_UV0 = float2.op_Implicit(labelVertex.m_UV0);
							labelVertexData.m_UV1 = float2.op_Implicit(labelVertex.m_UV1);
							labelVertexData.m_UV2 = float3.op_Implicit(val9);
							vertexData[num6 + num8] = labelVertexData;
						}
					}
				}
			}
			for (int num9 = 0; num9 < m_SubMeshCount; num9++)
			{
				SubMeshDescriptor subMesh = ((MeshData)(ref val4)).GetSubMesh(num9);
				((MeshData)(ref val4)).SetSubMesh(num9, subMesh, (MeshUpdateFlags)0);
			}
			val.Dispose();
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Area> __Game_Areas_Area_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Hidden> __Game_Tools_Hidden_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Triangle> __Game_Areas_Triangle_RO_BufferTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Native> __Game_Common_Native_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Node> __Game_Areas_Node_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.AreaColorData> __Game_Prefabs_AreaColorData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SelectionElement> __Game_Tools_SelectionElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Geometry> __Game_Areas_Geometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LabelVertex> __Game_Areas_LabelVertex_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<AreaNameData> __Game_Prefabs_AreaNameData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Updated> __Game_Common_Updated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BatchesUpdated> __Game_Common_BatchesUpdated_RO_ComponentTypeHandle;

		public BufferTypeHandle<LabelExtents> __Game_Areas_LabelExtents_RW_BufferTypeHandle;

		public BufferTypeHandle<LabelVertex> __Game_Areas_LabelVertex_RW_BufferTypeHandle;

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
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Areas_Area_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Area>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Tools_Hidden_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Hidden>(true);
			__Game_Areas_Triangle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Triangle>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Common_Native_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Native>(true);
			__Game_Areas_Node_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Node>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Prefabs_AreaColorData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Prefabs.AreaColorData>(true);
			__Game_Tools_SelectionElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SelectionElement>(true);
			__Game_Areas_Geometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Geometry>(true);
			__Game_Areas_LabelVertex_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LabelVertex>(true);
			__Game_Prefabs_AreaNameData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaNameData>(true);
			__Game_Common_Updated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Updated>(true);
			__Game_Common_BatchesUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BatchesUpdated>(true);
			__Game_Areas_LabelExtents_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LabelExtents>(false);
			__Game_Areas_LabelVertex_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LabelVertex>(false);
		}
	}

	private EntityQuery m_SettingsQuery;

	private RenderingSystem m_RenderingSystem;

	private OverlayRenderSystem m_OverlayRenderSystem;

	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private NameSystem m_NameSystem;

	private AreaTypeData[] m_AreaTypeData;

	private AreaType m_LastSelectionAreaType;

	private EntityQuery m_SelectionQuery;

	private bool m_Loaded;

	private int m_AreaParameters;

	private Dictionary<Entity, string> m_CachedLabels;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_OverlayRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<OverlayRenderSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_NameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NameSystem>();
		m_AreaTypeData = new AreaTypeData[5];
		m_AreaTypeData[0] = InitializeAreaData<Lot>();
		m_AreaTypeData[1] = InitializeAreaData<District>();
		m_AreaTypeData[2] = InitializeAreaData<MapTile>();
		m_AreaTypeData[3] = InitializeAreaData<Space>();
		m_AreaTypeData[4] = InitializeAreaData<Surface>();
		m_SettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Game.Prefabs.AreaTypeData>()
		});
		m_SelectionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<SelectionInfo>(),
			ComponentType.ReadOnly<SelectionElement>()
		});
		m_AreaParameters = Shader.PropertyToID("colossal_AreaParameters");
		GameManager.instance.localizationManager.onActiveDictionaryChanged += OnDictionaryChanged;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		for (int i = 0; i < m_AreaTypeData.Length; i++)
		{
			AreaTypeData areaTypeData = m_AreaTypeData[i];
			if (areaTypeData.m_NameMaterials != null)
			{
				for (int j = 0; j < areaTypeData.m_NameMaterials.Count; j++)
				{
					MaterialData materialData = areaTypeData.m_NameMaterials[j];
					if ((Object)(object)materialData.m_Material != (Object)null)
					{
						Object.Destroy((Object)(object)materialData.m_Material);
					}
				}
			}
			if (areaTypeData.m_BufferData.IsCreated)
			{
				areaTypeData.m_BufferData.Dispose();
			}
			if (areaTypeData.m_Bounds.IsCreated)
			{
				areaTypeData.m_Bounds.Dispose();
			}
			if ((Object)(object)areaTypeData.m_Material != (Object)null)
			{
				Object.Destroy((Object)(object)areaTypeData.m_Material);
			}
			if ((Object)(object)areaTypeData.m_NameMesh != (Object)null)
			{
				Object.Destroy((Object)(object)areaTypeData.m_NameMesh);
			}
			if (areaTypeData.m_Buffer != null)
			{
				areaTypeData.m_Buffer.Release();
			}
			if (areaTypeData.m_HasNameMeshData)
			{
				((MeshDataArray)(ref areaTypeData.m_NameMeshData)).Dispose();
			}
		}
		GameManager.instance.localizationManager.onActiveDictionaryChanged -= OnDictionaryChanged;
		base.OnDestroy();
	}

	private void OnDictionaryChanged()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponent<Updated>(m_AreaTypeData[1].m_AreaQuery);
	}

	private AreaTypeData InitializeAreaData<T>() where T : struct, IComponentData
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		AreaTypeData areaTypeData = new AreaTypeData();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<T>(),
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Triangle>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<BatchesUpdated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		areaTypeData.m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		areaTypeData.m_AreaQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<T>(),
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Triangle>(),
			ComponentType.Exclude<Deleted>()
		});
		return areaTypeData;
	}

	public void PreDeserialize(Context context)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_AreaTypeData.Length; i++)
		{
			AreaTypeData areaTypeData = m_AreaTypeData[i];
			if (areaTypeData.m_BufferData.IsCreated)
			{
				areaTypeData.m_BufferData.Dispose();
				areaTypeData.m_BufferData = default(NativeList<AreaTriangleData>);
			}
			if (areaTypeData.m_Buffer != null)
			{
				areaTypeData.m_Buffer.Release();
				areaTypeData.m_Buffer = null;
			}
			if ((Object)(object)areaTypeData.m_NameMesh != (Object)null)
			{
				Object.Destroy((Object)(object)areaTypeData.m_NameMesh);
				areaTypeData.m_NameMesh = null;
			}
			if (areaTypeData.m_HasNameMeshData)
			{
				((MeshDataArray)(ref areaTypeData.m_NameMeshData)).Dispose();
				areaTypeData.m_HasNameMeshData = false;
			}
		}
		if (m_CachedLabels != null)
		{
			m_CachedLabels.Clear();
		}
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
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_078b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_0794: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		if (!((EntityQuery)(ref m_SettingsQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_SettingsQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					AreaTypePrefab prefab = m_PrefabSystem.GetPrefab<AreaTypePrefab>(nativeArray[j]);
					AreaTypeData areaTypeData = m_AreaTypeData[(int)prefab.m_Type];
					float minNodeDistance = AreaUtils.GetMinNodeDistance(prefab.m_Type);
					if ((Object)(object)areaTypeData.m_Material != (Object)null)
					{
						Object.Destroy((Object)(object)areaTypeData.m_Material);
					}
					areaTypeData.m_Material = new Material(prefab.m_Material);
					((Object)areaTypeData.m_Material).name = "Area buffer (" + ((Object)prefab.m_Material).name + ")";
					areaTypeData.m_Material.SetVector(m_AreaParameters, new Vector4(minNodeDistance * (1f / 32f), minNodeDistance * 0.25f, minNodeDistance * 2f, 0f));
					if (areaTypeData.m_NameMaterials != null)
					{
						for (int k = 0; k < areaTypeData.m_NameMaterials.Count; k++)
						{
							MaterialData materialData = areaTypeData.m_NameMaterials[k];
							if ((Object)(object)materialData.m_Material != (Object)null)
							{
								Object.Destroy((Object)(object)materialData.m_Material);
							}
						}
						areaTypeData.m_NameMaterials = null;
					}
					areaTypeData.m_OriginalNameMaterial = prefab.m_NameMaterial;
					if ((Object)(object)prefab.m_NameMaterial != (Object)null)
					{
						areaTypeData.m_NameMaterials = new List<MaterialData>(1);
					}
				}
			}
			val.Dispose();
		}
		JobHandle val3 = default(JobHandle);
		AreaType areaType = AreaType.None;
		Entity val4 = Entity.Null;
		if (!((EntityQuery)(ref m_SelectionQuery)).IsEmptyIgnoreFilter)
		{
			val4 = ((EntityQuery)(ref m_SelectionQuery)).GetSingletonEntity();
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			areaType = ((EntityManager)(ref entityManager)).GetComponentData<SelectionInfo>(val4).m_AreaType;
		}
		if (m_LastSelectionAreaType != AreaType.None)
		{
			m_AreaTypeData[(int)m_LastSelectionAreaType].m_BufferDataDirty = true;
		}
		if (areaType != AreaType.None)
		{
			m_AreaTypeData[(int)areaType].m_BufferDataDirty = true;
		}
		m_LastSelectionAreaType = areaType;
		for (int l = 0; l < m_AreaTypeData.Length; l++)
		{
			AreaTypeData areaTypeData2 = m_AreaTypeData[l];
			EntityQuery val5 = (loaded ? areaTypeData2.m_AreaQuery : areaTypeData2.m_UpdatedQuery);
			if (!areaTypeData2.m_BufferDataDirty && ((EntityQuery)(ref val5)).IsEmptyIgnoreFilter)
			{
				continue;
			}
			if (((EntityQuery)(ref areaTypeData2.m_AreaQuery)).IsEmptyIgnoreFilter)
			{
				areaTypeData2.m_BufferDataDirty = false;
				areaTypeData2.m_BufferDirty = false;
				if (areaTypeData2.m_Buffer != null)
				{
					areaTypeData2.m_Buffer.Release();
					areaTypeData2.m_Buffer = null;
				}
				if ((Object)(object)areaTypeData2.m_NameMesh != (Object)null)
				{
					Object.Destroy((Object)(object)areaTypeData2.m_NameMesh);
					areaTypeData2.m_NameMesh = null;
				}
			}
			else
			{
				areaTypeData2.m_BufferDataDirty = true;
			}
			if (areaTypeData2.m_NameMaterials != null && !((EntityQuery)(ref val5)).IsEmptyIgnoreFilter)
			{
				UpdateLabelVertices(areaTypeData2, loaded);
			}
		}
		if (!m_RenderingSystem.hideOverlay)
		{
			JobHandle val7 = default(JobHandle);
			NativeList<ChunkData> chunkData = default(NativeList<ChunkData>);
			for (int m = 0; m < m_AreaTypeData.Length; m++)
			{
				AreaTypeData areaTypeData3 = m_AreaTypeData[m];
				if (!areaTypeData3.m_BufferDataDirty || (areaTypeData3.m_NameMaterials == null && (m_ToolSystem.activeTool == null || ((uint)m_ToolSystem.activeTool.requireAreas & (uint)(1 << m)) == 0)))
				{
					continue;
				}
				areaTypeData3.m_BufferDataDirty = false;
				areaTypeData3.m_BufferDirty = true;
				NativeList<ArchetypeChunk> val6 = ((EntityQuery)(ref areaTypeData3.m_AreaQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val7);
				chunkData._002Ector(0, AllocatorHandle.op_Implicit((Allocator)3));
				if (!areaTypeData3.m_BufferData.IsCreated)
				{
					areaTypeData3.m_BufferData = new NativeList<AreaTriangleData>(AllocatorHandle.op_Implicit((Allocator)4));
				}
				if (!areaTypeData3.m_Bounds.IsCreated)
				{
					areaTypeData3.m_Bounds = new NativeValue<Bounds3>((Allocator)4);
				}
				ResetChunkDataJob resetChunkDataJob = new ResetChunkDataJob
				{
					m_Chunks = val6,
					m_AreaType = InternalCompilerInterface.GetComponentTypeHandle<Area>(ref __TypeHandle.__Game_Areas_Area_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_HiddenType = InternalCompilerInterface.GetComponentTypeHandle<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TriangleType = InternalCompilerInterface.GetBufferTypeHandle<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ChunkData = chunkData,
					m_AreaTriangleData = areaTypeData3.m_BufferData
				};
				FillMeshDataJob fillMeshDataJob = new FillMeshDataJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_HiddenType = InternalCompilerInterface.GetComponentTypeHandle<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_AreaType = InternalCompilerInterface.GetComponentTypeHandle<Area>(ref __TypeHandle.__Game_Areas_Area_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_NativeType = InternalCompilerInterface.GetComponentTypeHandle<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_NodeType = InternalCompilerInterface.GetBufferTypeHandle<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TriangleType = InternalCompilerInterface.GetBufferTypeHandle<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_GeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ColorData = InternalCompilerInterface.GetComponentLookup<Game.Prefabs.AreaColorData>(ref __TypeHandle.__Game_Prefabs_AreaColorData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_SelectionElements = InternalCompilerInterface.GetBufferLookup<SelectionElement>(ref __TypeHandle.__Game_Tools_SelectionElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_SelectionEntity = val4,
					m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
					m_Chunks = val6.AsDeferredJobArray(),
					m_ChunkData = chunkData,
					m_AreaTriangleData = areaTypeData3.m_BufferData
				};
				CalculateBoundsJob obj = new CalculateBoundsJob
				{
					m_ChunkData = chunkData,
					m_Bounds = areaTypeData3.m_Bounds
				};
				JobHandle val8 = JobHandle.CombineDependencies(((SystemBase)this).Dependency, val7);
				JobHandle val9 = IJobExtensions.Schedule<ResetChunkDataJob>(resetChunkDataJob, val8);
				JobHandle val10 = IJobParallelForDeferExtensions.Schedule<FillMeshDataJob, ArchetypeChunk>(fillMeshDataJob, val6, 1, val9);
				JobHandle val11 = IJobExtensions.Schedule<CalculateBoundsJob>(obj, val10);
				chunkData.Dispose(val11);
				if (areaTypeData3.m_NameMaterials != null)
				{
					if (!areaTypeData3.m_HasNameMeshData)
					{
						areaTypeData3.m_HasNameMeshData = true;
						areaTypeData3.m_NameMeshData = Mesh.AllocateWritableMeshData(1);
					}
					JobHandle val12 = IJobExtensions.Schedule<FillNameDataJob>(new FillNameDataJob
					{
						m_GeometryType = InternalCompilerInterface.GetComponentTypeHandle<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_HiddenType = InternalCompilerInterface.GetComponentTypeHandle<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_LabelVertexType = InternalCompilerInterface.GetBufferTypeHandle<LabelVertex>(ref __TypeHandle.__Game_Areas_LabelVertex_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_AreaNameData = InternalCompilerInterface.GetComponentLookup<AreaNameData>(ref __TypeHandle.__Game_Prefabs_AreaNameData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
						m_Chunks = val6,
						m_SubMeshCount = areaTypeData3.m_NameMaterials.Count,
						m_NameMeshData = areaTypeData3.m_NameMeshData
					}, val8);
					val6.Dispose(JobHandle.CombineDependencies(val10, val12));
					areaTypeData3.m_DataDependencies = JobHandle.CombineDependencies(val11, val12);
				}
				else
				{
					val6.Dispose(val10);
					areaTypeData3.m_DataDependencies = val11;
				}
				val3 = JobHandle.CombineDependencies(val3, areaTypeData3.m_DataDependencies);
			}
		}
		((SystemBase)this).Dependency = val3;
	}

	public bool GetAreaBuffer(AreaType type, out ComputeBuffer buffer, out Material material, out Bounds bounds)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		AreaTypeData areaTypeData = m_AreaTypeData[(int)type];
		if (areaTypeData.m_BufferDirty)
		{
			areaTypeData.m_BufferDirty = false;
			((JobHandle)(ref areaTypeData.m_DataDependencies)).Complete();
			areaTypeData.m_DataDependencies = default(JobHandle);
			if (areaTypeData.m_BufferData.IsCreated)
			{
				if (areaTypeData.m_Buffer != null && areaTypeData.m_Buffer.count != areaTypeData.m_BufferData.Length)
				{
					areaTypeData.m_Buffer.Release();
					areaTypeData.m_Buffer = null;
				}
				if (areaTypeData.m_BufferData.Length > 0)
				{
					if (areaTypeData.m_Buffer == null)
					{
						areaTypeData.m_Buffer = new ComputeBuffer(areaTypeData.m_BufferData.Length, System.Runtime.CompilerServices.Unsafe.SizeOf<AreaTriangleData>());
					}
					areaTypeData.m_Buffer.SetData<AreaTriangleData>(areaTypeData.m_BufferData.AsArray());
				}
				areaTypeData.m_BufferData.Dispose();
			}
		}
		buffer = areaTypeData.m_Buffer;
		material = areaTypeData.m_Material;
		if (areaTypeData.m_Bounds.IsCreated)
		{
			bounds = RenderingUtils.ToBounds(areaTypeData.m_Bounds.value);
		}
		else
		{
			bounds = default(Bounds);
		}
		if (areaTypeData.m_Buffer != null)
		{
			return areaTypeData.m_Buffer.count != 0;
		}
		return false;
	}

	private void UpdateLabelVertices(AreaTypeData data, bool isLoaded)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Expected O, but got Unknown
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (isLoaded ? data.m_AreaQuery : data.m_UpdatedQuery);
		NativeArray<ArchetypeChunk> val2 = ((EntityQuery)(ref val)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			TextMeshPro textMesh = m_OverlayRenderSystem.GetTextMesh();
			((TMP_Text)textMesh).rectTransform.sizeDelta = new Vector2(250f, 100f);
			((TMP_Text)textMesh).fontSize = 200f;
			((TMP_Text)textMesh).alignment = (TextAlignmentOptions)514;
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Updated> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<BatchesUpdated> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<BatchesUpdated>(ref __TypeHandle.__Game_Common_BatchesUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Temp> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<LabelExtents> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<LabelExtents>(ref __TypeHandle.__Game_Areas_LabelExtents_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<LabelVertex> bufferTypeHandle2 = InternalCompilerInterface.GetBufferTypeHandle<LabelVertex>(ref __TypeHandle.__Game_Areas_LabelVertex_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			LabelVertex labelVertex = default(LabelVertex);
			for (int i = 0; i < val2.Length; i++)
			{
				ArchetypeChunk val3 = val2[i];
				if (isLoaded || ((ArchetypeChunk)(ref val3)).Has<Updated>(ref componentTypeHandle) || ((ArchetypeChunk)(ref val3)).Has<BatchesUpdated>(ref componentTypeHandle2))
				{
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray(entityTypeHandle);
					NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<Temp>(ref componentTypeHandle3);
					BufferAccessor<LabelExtents> bufferAccessor = ((ArchetypeChunk)(ref val3)).GetBufferAccessor<LabelExtents>(ref bufferTypeHandle);
					BufferAccessor<LabelVertex> bufferAccessor2 = ((ArchetypeChunk)(ref val3)).GetBufferAccessor<LabelVertex>(ref bufferTypeHandle2);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Entity val4 = nativeArray[j];
						DynamicBuffer<LabelExtents> val5 = bufferAccessor[j];
						DynamicBuffer<LabelVertex> val6 = bufferAccessor2[j];
						string renderedLabelName;
						if (nativeArray2.Length != 0)
						{
							Temp temp = nativeArray2[j];
							if (!(temp.m_Original != Entity.Null))
							{
								if (m_CachedLabels != null && m_CachedLabels.ContainsKey(val4))
								{
									m_CachedLabels.Remove(val4);
								}
								val6.Clear();
								continue;
							}
							renderedLabelName = m_NameSystem.GetRenderedLabelName(temp.m_Original);
						}
						else
						{
							renderedLabelName = m_NameSystem.GetRenderedLabelName(val4);
						}
						if (m_CachedLabels != null)
						{
							if (m_CachedLabels.TryGetValue(val4, out var value))
							{
								if (value == renderedLabelName)
								{
									continue;
								}
								m_CachedLabels[val4] = renderedLabelName;
							}
							else
							{
								m_CachedLabels.Add(val4, renderedLabelName);
							}
						}
						else
						{
							m_CachedLabels = new Dictionary<Entity, string>();
							m_CachedLabels.Add(val4, renderedLabelName);
						}
						TMP_TextInfo textInfo = ((TMP_Text)textMesh).GetTextInfo(renderedLabelName);
						int num = 0;
						for (int k = 0; k < textInfo.meshInfo.Length; k++)
						{
							TMP_MeshInfo val7 = textInfo.meshInfo[k];
							num += val7.vertexCount;
						}
						val6.ResizeUninitialized(num);
						num = 0;
						for (int l = 0; l < textInfo.meshInfo.Length; l++)
						{
							TMP_MeshInfo val8 = textInfo.meshInfo[l];
							if (val8.vertexCount == 0)
							{
								continue;
							}
							Texture mainTexture = val8.material.mainTexture;
							int num2 = -1;
							for (int m = 0; m < data.m_NameMaterials.Count; m++)
							{
								if ((Object)(object)data.m_NameMaterials[m].m_Material.mainTexture == (Object)(object)mainTexture)
								{
									num2 = m;
									break;
								}
							}
							if (num2 == -1)
							{
								MaterialData item = new MaterialData
								{
									m_Material = new Material(data.m_OriginalNameMaterial)
								};
								m_OverlayRenderSystem.CopyFontAtlasParameters(val8.material, item.m_Material);
								num2 = data.m_NameMaterials.Count;
								data.m_NameMaterials.Add(item);
								((Object)item.m_Material).name = $"Area names {num2} ({((Object)data.m_OriginalNameMaterial).name})";
							}
							Vector3[] vertices = val8.vertices;
							Vector2[] uvs = val8.uvs0;
							Vector2[] uvs2 = val8.uvs2;
							Color32[] colors = val8.colors32;
							for (int n = 0; n < val8.vertexCount; n++)
							{
								labelVertex.m_Position = float3.op_Implicit(vertices[n]);
								labelVertex.m_Color = colors[n];
								labelVertex.m_UV0 = float2.op_Implicit(uvs[n]);
								labelVertex.m_UV1 = float2.op_Implicit(uvs2[n]);
								labelVertex.m_Material = num2;
								val6[num + n] = labelVertex;
							}
							num += val8.vertexCount;
						}
						val5.ResizeUninitialized(textInfo.lineCount);
						for (int num3 = 0; num3 < textInfo.lineCount; num3++)
						{
							Extents lineExtents = textInfo.lineInfo[num3].lineExtents;
							val5[num3] = new LabelExtents(float2.op_Implicit(lineExtents.min), float2.op_Implicit(lineExtents.max));
						}
					}
				}
				else if (m_CachedLabels != null)
				{
					NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref val3)).GetNativeArray(entityTypeHandle);
					for (int num4 = 0; num4 < nativeArray3.Length; num4++)
					{
						Entity key = nativeArray3[num4];
						m_CachedLabels.Remove(key);
					}
				}
			}
		}
		finally
		{
			val2.Dispose();
		}
	}

	public bool GetNameMesh(AreaType type, out Mesh mesh, out int subMeshCount)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		AreaTypeData areaTypeData = m_AreaTypeData[(int)type];
		if (areaTypeData.m_NameMaterials != null)
		{
			subMeshCount = areaTypeData.m_NameMaterials.Count;
		}
		else
		{
			subMeshCount = 0;
		}
		if (areaTypeData.m_HasNameMeshData)
		{
			areaTypeData.m_HasNameMeshData = false;
			((JobHandle)(ref areaTypeData.m_DataDependencies)).Complete();
			areaTypeData.m_DataDependencies = default(JobHandle);
			if ((Object)(object)areaTypeData.m_NameMesh == (Object)null)
			{
				areaTypeData.m_NameMesh = new Mesh();
				((Object)areaTypeData.m_NameMesh).name = $"Area names ({type})";
			}
			Mesh.ApplyAndDisposeWritableMeshData(areaTypeData.m_NameMeshData, areaTypeData.m_NameMesh, (MeshUpdateFlags)9);
			if (areaTypeData.m_Bounds.IsCreated && math.all(areaTypeData.m_Bounds.value.max >= areaTypeData.m_Bounds.value.min))
			{
				areaTypeData.m_NameMesh.bounds = RenderingUtils.ToBounds(areaTypeData.m_Bounds.value);
			}
			else
			{
				areaTypeData.m_NameMesh.RecalculateBounds();
			}
			areaTypeData.m_HasNameMesh = false;
			for (int i = 0; i < subMeshCount; i++)
			{
				MaterialData value = areaTypeData.m_NameMaterials[i];
				SubMeshDescriptor subMesh = areaTypeData.m_NameMesh.GetSubMesh(i);
				value.m_HasMesh = ((SubMeshDescriptor)(ref subMesh)).vertexCount > 0;
				areaTypeData.m_HasNameMesh |= value.m_HasMesh;
				areaTypeData.m_NameMaterials[i] = value;
			}
		}
		mesh = areaTypeData.m_NameMesh;
		return areaTypeData.m_HasNameMesh;
	}

	public bool GetNameMaterial(AreaType type, int subMeshIndex, out Material material)
	{
		MaterialData materialData = m_AreaTypeData[(int)type].m_NameMaterials[subMeshIndex];
		material = materialData.m_Material;
		return materialData.m_HasMesh;
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
	public AreaBufferSystem()
	{
	}
}

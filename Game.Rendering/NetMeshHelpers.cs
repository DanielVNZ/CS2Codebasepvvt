using System;
using Colossal.IO.AssetDatabase;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Rendering;

public static class NetMeshHelpers
{
	[BurstCompile]
	private struct CacheMeshDataJob : IJob
	{
		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeSlice<byte> m_Positions;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeSlice<byte> m_Normals;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeSlice<byte> m_Tangents;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeSlice<byte> m_TexCoords0;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeArray<byte> m_Indices;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public Data m_Data;

		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public IndexFormat m_IndexFormat;

		[ReadOnly]
		public int m_VertexCount;

		[ReadOnly]
		public int m_IndexCount;

		[ReadOnly]
		public VertexAttributeFormat m_PositionsFormat;

		[ReadOnly]
		public VertexAttributeFormat m_NormalsFormat;

		[ReadOnly]
		public VertexAttributeFormat m_TangentsFormat;

		[ReadOnly]
		public VertexAttributeFormat m_TexCoords0Format;

		[ReadOnly]
		public int m_PositionsDim;

		[ReadOnly]
		public int m_NormalsDim;

		[ReadOnly]
		public int m_TangentsDim;

		[ReadOnly]
		public int m_TexCoords0Dim;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshVertex> dst = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshVertex>(m_Entity);
			DynamicBuffer<MeshNormal> dst2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshNormal>(m_Entity);
			DynamicBuffer<MeshTangent> dst3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshTangent>(m_Entity);
			DynamicBuffer<MeshUV0> dst4 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshUV0>(m_Entity);
			DynamicBuffer<MeshIndex> dst5 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshIndex>(m_Entity);
			if (((Data)(ref m_Data)).IsValid)
			{
				int allVertexCount = GeometryAsset.GetAllVertexCount(ref m_Data);
				int allIndicesCount = GeometryAsset.GetAllIndicesCount(ref m_Data);
				dst.ResizeUninitialized(allVertexCount);
				dst2.ResizeUninitialized(allVertexCount);
				dst3.ResizeUninitialized(allVertexCount);
				dst4.ResizeUninitialized(allVertexCount);
				dst5.ResizeUninitialized(allIndicesCount);
				allVertexCount = 0;
				allIndicesCount = 0;
				VertexAttributeFormat format = default(VertexAttributeFormat);
				int num = default(int);
				VertexAttributeFormat format2 = default(VertexAttributeFormat);
				int num2 = default(int);
				VertexAttributeFormat format3 = default(VertexAttributeFormat);
				int num3 = default(int);
				VertexAttributeFormat format4 = default(VertexAttributeFormat);
				int num4 = default(int);
				for (int i = 0; i < m_Data.meshCount; i++)
				{
					int vertexCount = GeometryAsset.GetVertexCount(ref m_Data, i);
					int indicesCount = GeometryAsset.GetIndicesCount(ref m_Data, i);
					GeometryAsset.GetAttributeFormat(ref m_Data, i, (VertexAttribute)0, ref format, ref num);
					GeometryAsset.GetAttributeFormat(ref m_Data, i, (VertexAttribute)1, ref format2, ref num2);
					GeometryAsset.GetAttributeFormat(ref m_Data, i, (VertexAttribute)2, ref format3, ref num3);
					GeometryAsset.GetAttributeFormat(ref m_Data, i, (VertexAttribute)4, ref format4, ref num4);
					IndexFormat indexFormat = GeometryAsset.GetIndexFormat(ref m_Data, i);
					if (num == 0)
					{
						throw new Exception("Cannot cache geometry asset: mesh do not have a position");
					}
					if (num2 == 0)
					{
						throw new Exception("Cannot cache geometry asset: mesh do not have a normal");
					}
					if (num3 == 0)
					{
						throw new Exception("Cannot cache geometry asset: mesh do not have a tangent");
					}
					if (num4 == 0)
					{
						throw new Exception("Cannot cache geometry asset: mesh do not have a UV0");
					}
					NativeSlice<byte> attributeData = GeometryAsset.GetAttributeData(ref m_Data, i, (VertexAttribute)0);
					NativeSlice<byte> attributeData2 = GeometryAsset.GetAttributeData(ref m_Data, i, (VertexAttribute)1);
					NativeSlice<byte> attributeData3 = GeometryAsset.GetAttributeData(ref m_Data, i, (VertexAttribute)2);
					NativeSlice<byte> attributeData4 = GeometryAsset.GetAttributeData(ref m_Data, i, (VertexAttribute)4);
					NativeArray<byte> indices = GeometryAsset.GetIndices(ref m_Data, i);
					NativeArray<MeshVertex> subArray = dst.AsNativeArray().GetSubArray(allVertexCount, vertexCount);
					NativeArray<MeshNormal> subArray2 = dst2.AsNativeArray().GetSubArray(allVertexCount, vertexCount);
					NativeArray<MeshTangent> subArray3 = dst3.AsNativeArray().GetSubArray(allVertexCount, vertexCount);
					NativeArray<MeshUV0> subArray4 = dst4.AsNativeArray().GetSubArray(allVertexCount, vertexCount);
					NativeArray<MeshIndex> subArray5 = dst5.AsNativeArray().GetSubArray(allIndicesCount, indicesCount);
					MeshVertex.Unpack(attributeData, subArray, vertexCount, format, num);
					MeshNormal.Unpack(attributeData2, subArray2, vertexCount, format2, num2);
					MeshTangent.Unpack(attributeData3, subArray3, vertexCount, format3, num3);
					MeshUV0.Unpack(attributeData4, subArray4, vertexCount, format4, num4);
					MeshIndex.Unpack(indices, subArray5, indicesCount, indexFormat, allVertexCount);
					allVertexCount += vertexCount;
					allIndicesCount += indicesCount;
				}
			}
			else
			{
				MeshVertex.Unpack(m_Positions, dst, m_VertexCount, m_PositionsFormat, m_PositionsDim);
				MeshNormal.Unpack(m_Normals, dst2, m_VertexCount, m_NormalsFormat, m_NormalsDim);
				MeshTangent.Unpack(m_Tangents, dst3, m_VertexCount, m_TangentsFormat, m_TangentsDim);
				MeshUV0.Unpack(m_TexCoords0, dst4, m_VertexCount, m_TexCoords0Format, m_TexCoords0Dim);
				MeshIndex.Unpack(m_Indices, dst5, m_IndexCount, m_IndexFormat);
			}
		}
	}

	private static readonly float3 v_left = new float3(-1f, 0f, 0f);

	private static readonly float3 v_up = new float3(0f, 1f, 0f);

	private static readonly float3 v_right = new float3(1f, 0f, 0f);

	private static readonly float3 v_down = new float3(0f, -1f, 0f);

	private static readonly float3 v_forward = new float3(0f, 0f, 1f);

	private static readonly float3 v_backward = new float3(0f, 0f, -1f);

	public static Mesh CreateDefaultLaneMesh()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Expected O, but got Unknown
		int num = 4;
		int num2 = num * 8 + 16;
		int num3 = num * 24 + 12;
		Vector3[] vertices = (Vector3[])(object)new Vector3[num2];
		Vector3[] normals = (Vector3[])(object)new Vector3[num2];
		Vector4[] tangents = (Vector4[])(object)new Vector4[num2];
		Vector2[] array = (Vector2[])(object)new Vector2[num2];
		int[] array2 = new int[num3];
		int vertexIndex = 0;
		int indexIndex = 0;
		AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(-1f, -1f, -1f), v_backward, v_right, new float2(0f, 0f));
		AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(-1f, 1f, -1f), v_backward, v_right, new float2(0f, 1f));
		AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(1f, 1f, -1f), v_backward, v_right, new float2(1f, 1f));
		AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(1f, -1f, -1f), v_backward, v_right, new float2(1f, 0f));
		AddQuad(array2, ref indexIndex, vertexIndex - 4, vertexIndex - 3, vertexIndex - 2, vertexIndex - 1);
		for (int i = 0; i <= num; i++)
		{
			float num4 = (float)i / (float)num;
			float num5 = num4 * 2f - 1f;
			AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(-1f, -1f, num5), v_left, v_up, new float2(0f, num4));
			AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(-1f, 1f, num5), v_left, v_up, new float2(1f, num4));
			AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(-1f, 1f, num5), v_up, v_right, new float2(0f, num4));
			AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(1f, 1f, num5), v_up, v_right, new float2(1f, num4));
			AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(1f, 1f, num5), v_right, v_down, new float2(0f, num4));
			AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(1f, -1f, num5), v_right, v_down, new float2(1f, num4));
			AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(1f, -1f, num5), v_down, v_left, new float2(0f, num4));
			AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(-1f, -1f, num5), v_down, v_left, new float2(1f, num4));
			if (i != 0)
			{
				AddQuad(array2, ref indexIndex, vertexIndex - 16, vertexIndex - 8, vertexIndex - 7, vertexIndex - 15);
				AddQuad(array2, ref indexIndex, vertexIndex - 14, vertexIndex - 6, vertexIndex - 5, vertexIndex - 13);
				AddQuad(array2, ref indexIndex, vertexIndex - 12, vertexIndex - 4, vertexIndex - 3, vertexIndex - 11);
				AddQuad(array2, ref indexIndex, vertexIndex - 10, vertexIndex - 2, vertexIndex - 1, vertexIndex - 9);
			}
		}
		AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(1f, -1f, 1f), v_forward, v_left, new float2(0f, 0f));
		AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(1f, 1f, 1f), v_forward, v_left, new float2(0f, 1f));
		AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(-1f, 1f, 1f), v_forward, v_left, new float2(1f, 1f));
		AddVertex(vertices, normals, tangents, array, ref vertexIndex, new float3(-1f, -1f, 1f), v_forward, v_left, new float2(1f, 0f));
		AddQuad(array2, ref indexIndex, vertexIndex - 4, vertexIndex - 3, vertexIndex - 2, vertexIndex - 1);
		return new Mesh
		{
			name = "Default lane",
			vertices = vertices,
			normals = normals,
			tangents = tangents,
			uv = array,
			triangles = array2
		};
	}

	public static Mesh CreateDefaultRoundaboutMesh()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		int num = 4;
		int num2 = num * 10 + 22;
		int num3 = num * 36 + 24;
		Vector3[] vertices = (Vector3[])(object)new Vector3[num2];
		Vector3[] normals = (Vector3[])(object)new Vector3[num2];
		Vector4[] tangents = (Vector4[])(object)new Vector4[num2];
		Color32[] colors = (Color32[])(object)new Color32[num2];
		Vector4[] uvs = (Vector4[])(object)new Vector4[num2];
		int[] indices = new int[num3];
		int vertexIndex = 0;
		int indexIndex = 0;
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0f, -2f), new int2(0, 4), 0f, -2f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0f, -1f), new int2(0, 4), 0f, 0f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0.5f, -1f), new int2(4, 2), 0f, 0f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(1f, -1f), new int2(4, 2), 1f, 0f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(1f, -2f), new int2(4, 2), 1f, -2f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0.5f, -2f), new int2(4, 2), 0f, -2f, 0f);
		AddQuad(indices, ref indexIndex, vertexIndex - 6, vertexIndex - 5, vertexIndex - 4, vertexIndex - 1);
		AddQuad(indices, ref indexIndex, vertexIndex - 1, vertexIndex - 4, vertexIndex - 3, vertexIndex - 2);
		int3 val = default(int3);
		for (int i = 0; i <= num; i++)
		{
			((int3)(ref val))._002Ector(0, 4, 2);
			float num4 = (float)i / ((float)num * 0.5f);
			float num5 = num4 - 3f;
			if (i >= num >> 1)
			{
				val += 1;
				num4 -= 1f;
			}
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_left, v_up, new float2(0f, num5), ((int3)(ref val)).xy, 0f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_left, v_up, new float2(1f, num5), ((int3)(ref val)).xy, 0f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_up, v_right, new float2(0f, num5), ((int3)(ref val)).xy, 0f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_up, v_right, new float2(0.5f, num5), ((int3)(ref val)).yz, 0f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_up, v_right, new float2(1f, num5), ((int3)(ref val)).yz, 1f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_right, v_down, new float2(0f, num5), ((int3)(ref val)).yz, 1f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_right, v_down, new float2(1f, num5), ((int3)(ref val)).yz, 1f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_down, v_left, new float2(0f, num5), ((int3)(ref val)).yz, 1f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_down, v_left, new float2(0.5f, num5), ((int3)(ref val)).yz, 0f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_down, v_left, new float2(1f, num5), ((int3)(ref val)).xy, 0f, -2f, num4);
			if (i != 0)
			{
				AddQuad(indices, ref indexIndex, vertexIndex - 20, vertexIndex - 10, vertexIndex - 9, vertexIndex - 19);
				AddQuad(indices, ref indexIndex, vertexIndex - 18, vertexIndex - 8, vertexIndex - 7, vertexIndex - 17);
				AddQuad(indices, ref indexIndex, vertexIndex - 17, vertexIndex - 7, vertexIndex - 6, vertexIndex - 16);
				AddQuad(indices, ref indexIndex, vertexIndex - 15, vertexIndex - 5, vertexIndex - 4, vertexIndex - 14);
				AddQuad(indices, ref indexIndex, vertexIndex - 13, vertexIndex - 3, vertexIndex - 2, vertexIndex - 12);
				AddQuad(indices, ref indexIndex, vertexIndex - 12, vertexIndex - 2, vertexIndex - 1, vertexIndex - 11);
			}
		}
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0f, -2f), new int2(5, 3), 1f, -2f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0f, -1f), new int2(5, 3), 1f, 0f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0.5f, -1f), new int2(5, 3), 0f, 0f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(1f, -1f), new int2(1, 5), 0f, 0f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(1f, -2f), new int2(1, 5), 0f, -2f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0.5f, -2f), new int2(5, 3), 0f, -2f, 1f);
		AddQuad(indices, ref indexIndex, vertexIndex - 6, vertexIndex - 5, vertexIndex - 4, vertexIndex - 1);
		AddQuad(indices, ref indexIndex, vertexIndex - 1, vertexIndex - 4, vertexIndex - 3, vertexIndex - 2);
		return CreateMesh("Default roundabout", vertices, normals, tangents, colors, uvs, indices);
	}

	public static Mesh CreateDefaultNodeMesh()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0776: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_080d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_0882: Unknown result type (might be due to invalid IL or missing references)
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0905: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_093c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0943: Unknown result type (might be due to invalid IL or missing references)
		//IL_0966: Unknown result type (might be due to invalid IL or missing references)
		//IL_096b: Unknown result type (might be due to invalid IL or missing references)
		//IL_097a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0981: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
		int num = 2;
		int num2 = num * 14 + 34;
		int num3 = num * 60 + 48;
		Vector3[] vertices = (Vector3[])(object)new Vector3[num2];
		Vector3[] normals = (Vector3[])(object)new Vector3[num2];
		Vector4[] tangents = (Vector4[])(object)new Vector4[num2];
		Color32[] colors = (Color32[])(object)new Color32[num2];
		Vector4[] uvs = (Vector4[])(object)new Vector4[num2];
		int[] indices = new int[num3];
		int vertexIndex = 0;
		int indexIndex = 0;
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0f, -2f), new int2(0, 2), 0f, -2f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0f, -1f), new int2(0, 2), 0f, 0f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0.25f, -1f), new int2(0, 2), 1f, 0f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0.5f, -1f), new int2(4, 0), 0f, 0f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0.75f, -1f), new int2(1, 3), 0f, 0f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(1f, -1f), new int2(1, 3), 1f, 0f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(1f, -2f), new int2(1, 3), 1f, -2f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0.75f, -2f), new int2(1, 3), 0f, -2f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0.5f, -2f), new int2(4, 0), 0f, -2f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0.25f, -2f), new int2(0, 2), 1f, -2f, 0f);
		AddQuad(indices, ref indexIndex, vertexIndex - 10, vertexIndex - 9, vertexIndex - 8, vertexIndex - 1);
		AddQuad(indices, ref indexIndex, vertexIndex - 1, vertexIndex - 8, vertexIndex - 7, vertexIndex - 2);
		AddQuad(indices, ref indexIndex, vertexIndex - 2, vertexIndex - 7, vertexIndex - 6, vertexIndex - 3);
		AddQuad(indices, ref indexIndex, vertexIndex - 3, vertexIndex - 6, vertexIndex - 5, vertexIndex - 4);
		for (int i = 0; i <= num; i++)
		{
			float num4 = (float)i / (float)num;
			float num5 = num4 - 2f;
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_left, v_up, new float2(0f, num5), new int2(0, 2), 0f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_left, v_up, new float2(1f, num5), new int2(0, 2), 0f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_up, v_right, new float2(0f, num5), new int2(0, 2), 0f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_up, v_right, new float2(0.25f, num5), new int2(0, 2), 1f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_up, v_right, new float2(0.5f, num5), new int2(4, 0), 0f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_up, v_right, new float2(0.75f, num5), new int2(1, 3), 0f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_up, v_right, new float2(1f, num5), new int2(1, 3), 1f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_right, v_down, new float2(0f, num5), new int2(1, 3), 1f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_right, v_down, new float2(1f, num5), new int2(1, 3), 1f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_down, v_left, new float2(0f, num5), new int2(1, 3), 1f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_down, v_left, new float2(0.25f, num5), new int2(1, 3), 0f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_down, v_left, new float2(0.5f, num5), new int2(4, 0), 0f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_down, v_left, new float2(0.75f, num5), new int2(0, 2), 1f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_down, v_left, new float2(1f, num5), new int2(0, 2), 0f, -2f, num4);
			if (i != 0)
			{
				AddQuad(indices, ref indexIndex, vertexIndex - 28, vertexIndex - 14, vertexIndex - 13, vertexIndex - 27);
				AddQuad(indices, ref indexIndex, vertexIndex - 26, vertexIndex - 12, vertexIndex - 11, vertexIndex - 25);
				AddQuad(indices, ref indexIndex, vertexIndex - 25, vertexIndex - 11, vertexIndex - 10, vertexIndex - 24);
				AddQuad(indices, ref indexIndex, vertexIndex - 24, vertexIndex - 10, vertexIndex - 9, vertexIndex - 23);
				AddQuad(indices, ref indexIndex, vertexIndex - 23, vertexIndex - 9, vertexIndex - 8, vertexIndex - 22);
				AddQuad(indices, ref indexIndex, vertexIndex - 21, vertexIndex - 7, vertexIndex - 6, vertexIndex - 20);
				AddQuad(indices, ref indexIndex, vertexIndex - 19, vertexIndex - 5, vertexIndex - 4, vertexIndex - 18);
				AddQuad(indices, ref indexIndex, vertexIndex - 18, vertexIndex - 4, vertexIndex - 3, vertexIndex - 17);
				AddQuad(indices, ref indexIndex, vertexIndex - 17, vertexIndex - 3, vertexIndex - 2, vertexIndex - 16);
				AddQuad(indices, ref indexIndex, vertexIndex - 16, vertexIndex - 2, vertexIndex - 1, vertexIndex - 15);
			}
		}
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0f, -2f), new int2(1, 3), 1f, -2f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0f, -1f), new int2(1, 3), 1f, 0f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0.25f, -1f), new int2(1, 3), 0f, 0f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0.5f, -1f), new int2(4, 0), 0f, 0f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0.75f, -1f), new int2(0, 2), 1f, 0f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(1f, -1f), new int2(0, 2), 0f, 0f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(1f, -2f), new int2(0, 2), 0f, -2f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0.75f, -2f), new int2(0, 2), 1f, -2f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0.5f, -2f), new int2(4, 0), 0f, -2f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0.25f, -2f), new int2(1, 3), 0f, -2f, 1f);
		AddQuad(indices, ref indexIndex, vertexIndex - 10, vertexIndex - 9, vertexIndex - 8, vertexIndex - 1);
		AddQuad(indices, ref indexIndex, vertexIndex - 1, vertexIndex - 8, vertexIndex - 7, vertexIndex - 2);
		AddQuad(indices, ref indexIndex, vertexIndex - 2, vertexIndex - 7, vertexIndex - 6, vertexIndex - 3);
		AddQuad(indices, ref indexIndex, vertexIndex - 3, vertexIndex - 6, vertexIndex - 5, vertexIndex - 4);
		return CreateMesh("Default node", vertices, normals, tangents, colors, uvs, indices);
	}

	public static Mesh CreateDefaultEdgeMesh()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		int num = 4;
		int num2 = num * 8 + 16;
		int num3 = num * 24 + 12;
		Vector3[] vertices = (Vector3[])(object)new Vector3[num2];
		Vector3[] normals = (Vector3[])(object)new Vector3[num2];
		Vector4[] tangents = (Vector4[])(object)new Vector4[num2];
		Color32[] colors = (Color32[])(object)new Color32[num2];
		Vector4[] uvs = (Vector4[])(object)new Vector4[num2];
		int[] indices = new int[num3];
		int vertexIndex = 0;
		int indexIndex = 0;
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0f, -2f), new int2(0, 2), 0f, -2f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(0f, -1f), new int2(0, 2), 0f, 0f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(1f, -1f), new int2(0, 2), 1f, 0f, 0f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_backward, v_right, new float2(1f, -2f), new int2(0, 2), 1f, -2f, 0f);
		AddQuad(indices, ref indexIndex, vertexIndex - 4, vertexIndex - 3, vertexIndex - 2, vertexIndex - 1);
		int2 val = default(int2);
		for (int i = 0; i <= num; i++)
		{
			((int2)(ref val))._002Ector(0, 2);
			float num4 = (float)i / ((float)num * 0.5f);
			float num5 = num4 - 3f;
			if (i >= num >> 1)
			{
				val += 1;
				num4 -= 1f;
			}
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_left, v_up, new float2(0f, num5), val, 0f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_left, v_up, new float2(1f, num5), val, 0f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_up, v_right, new float2(0f, num5), val, 0f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_up, v_right, new float2(1f, num5), val, 1f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_right, v_down, new float2(0f, num5), val, 1f, 0f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_right, v_down, new float2(1f, num5), val, 1f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_down, v_left, new float2(0f, num5), val, 1f, -2f, num4);
			AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_down, v_left, new float2(1f, num5), val, 0f, -2f, num4);
			if (i != 0)
			{
				AddQuad(indices, ref indexIndex, vertexIndex - 16, vertexIndex - 8, vertexIndex - 7, vertexIndex - 15);
				AddQuad(indices, ref indexIndex, vertexIndex - 14, vertexIndex - 6, vertexIndex - 5, vertexIndex - 13);
				AddQuad(indices, ref indexIndex, vertexIndex - 12, vertexIndex - 4, vertexIndex - 3, vertexIndex - 11);
				AddQuad(indices, ref indexIndex, vertexIndex - 10, vertexIndex - 2, vertexIndex - 1, vertexIndex - 9);
			}
		}
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0f, -2f), new int2(1, 3), 1f, -2f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(0f, -1f), new int2(1, 3), 1f, 0f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(1f, -1f), new int2(1, 3), 0f, 0f, 1f);
		AddVertex(vertices, normals, tangents, colors, uvs, ref vertexIndex, v_forward, v_left, new float2(1f, -2f), new int2(1, 3), 0f, -2f, 1f);
		AddQuad(indices, ref indexIndex, vertexIndex - 4, vertexIndex - 3, vertexIndex - 2, vertexIndex - 1);
		return CreateMesh("Default edge", vertices, normals, tangents, colors, uvs, indices);
	}

	public static JobHandle CacheMeshData(GeometryAsset meshData, Entity entity, EntityManager entityManager, EntityCommandBuffer commandBuffer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<MeshMaterial> buffer = ((EntityManager)(ref entityManager)).GetBuffer<MeshMaterial>(entity, false);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < meshData.meshCount; i++)
		{
			int subMeshCount = meshData.GetSubMeshCount(i);
			for (int j = 0; j < subMeshCount; j++)
			{
				SubMeshDescriptor subMeshDesc = meshData.GetSubMeshDesc(i, j);
				ref MeshMaterial reference = ref buffer.ElementAt(num++);
				reference.m_StartIndex = num2 + ((SubMeshDescriptor)(ref subMeshDesc)).indexStart;
				reference.m_IndexCount = ((SubMeshDescriptor)(ref subMeshDesc)).indexCount;
				reference.m_StartVertex = num3 + ((SubMeshDescriptor)(ref subMeshDesc)).firstVertex;
				reference.m_VertexCount = ((SubMeshDescriptor)(ref subMeshDesc)).vertexCount;
			}
			num2 += meshData.GetIndicesCount(i);
			num3 += meshData.GetVertexCount(i);
		}
		return IJobExtensions.Schedule<CacheMeshDataJob>(new CacheMeshDataJob
		{
			m_Data = meshData.data,
			m_Entity = entity,
			m_CommandBuffer = commandBuffer
		}, default(JobHandle));
	}

	public static void CacheMeshData(Mesh mesh, Entity entity, EntityManager entityManager, EntityCommandBuffer commandBuffer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<MeshVertex> val = ((EntityCommandBuffer)(ref commandBuffer)).AddBuffer<MeshVertex>(entity);
		DynamicBuffer<MeshNormal> val2 = ((EntityCommandBuffer)(ref commandBuffer)).AddBuffer<MeshNormal>(entity);
		DynamicBuffer<MeshTangent> val3 = ((EntityCommandBuffer)(ref commandBuffer)).AddBuffer<MeshTangent>(entity);
		DynamicBuffer<MeshUV0> val4 = ((EntityCommandBuffer)(ref commandBuffer)).AddBuffer<MeshUV0>(entity);
		DynamicBuffer<MeshIndex> val5 = ((EntityCommandBuffer)(ref commandBuffer)).AddBuffer<MeshIndex>(entity);
		DynamicBuffer<MeshMaterial> buffer = ((EntityManager)(ref entityManager)).GetBuffer<MeshMaterial>(entity, false);
		MeshDataArray val6 = Mesh.AcquireReadOnlyMeshData(mesh);
		MeshData val7 = ((MeshDataArray)(ref val6))[0];
		int num = 0;
		int subMeshCount = ((MeshData)(ref val7)).subMeshCount;
		for (int i = 0; i < subMeshCount; i++)
		{
			int num2 = num;
			SubMeshDescriptor subMesh = ((MeshData)(ref val7)).GetSubMesh(i);
			num = num2 + ((SubMeshDescriptor)(ref subMesh)).indexCount;
		}
		val.ResizeUninitialized(((MeshData)(ref val7)).vertexCount);
		val2.ResizeUninitialized(((MeshData)(ref val7)).vertexCount);
		val3.ResizeUninitialized(((MeshData)(ref val7)).vertexCount);
		val4.ResizeUninitialized(((MeshData)(ref val7)).vertexCount);
		val5.ResizeUninitialized(num);
		((MeshData)(ref val7)).GetVertices(val.AsNativeArray().Reinterpret<Vector3>());
		((MeshData)(ref val7)).GetNormals(val2.AsNativeArray().Reinterpret<Vector3>());
		((MeshData)(ref val7)).GetTangents(val3.AsNativeArray().Reinterpret<Vector4>());
		((MeshData)(ref val7)).GetUVs(0, val4.AsNativeArray().Reinterpret<Vector2>());
		num = 0;
		for (int j = 0; j < subMeshCount; j++)
		{
			SubMeshDescriptor subMesh2 = ((MeshData)(ref val7)).GetSubMesh(j);
			((MeshData)(ref val7)).GetIndices(val5.AsNativeArray().GetSubArray(num, ((SubMeshDescriptor)(ref subMesh2)).indexCount).Reinterpret<int>(), j, true);
			MeshMaterial meshMaterial = buffer[j];
			meshMaterial.m_StartIndex = ((SubMeshDescriptor)(ref subMesh2)).indexStart;
			meshMaterial.m_IndexCount = ((SubMeshDescriptor)(ref subMesh2)).indexCount;
			meshMaterial.m_StartVertex = ((SubMeshDescriptor)(ref subMesh2)).firstVertex;
			meshMaterial.m_VertexCount = ((SubMeshDescriptor)(ref subMesh2)).vertexCount;
			buffer[j] = meshMaterial;
			num += ((SubMeshDescriptor)(ref subMesh2)).indexCount;
		}
		((MeshDataArray)(ref val6)).Dispose();
	}

	public static void UncacheMeshData(Entity entity, EntityCommandBuffer commandBuffer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		((EntityCommandBuffer)(ref commandBuffer)).RemoveComponent<MeshVertex>(entity);
		((EntityCommandBuffer)(ref commandBuffer)).RemoveComponent<MeshNormal>(entity);
		((EntityCommandBuffer)(ref commandBuffer)).RemoveComponent<MeshTangent>(entity);
		((EntityCommandBuffer)(ref commandBuffer)).RemoveComponent<MeshUV0>(entity);
		((EntityCommandBuffer)(ref commandBuffer)).RemoveComponent<MeshIndex>(entity);
	}

	private static void AddVertex(Vector3[] vertices, Vector3[] normals, Vector4[] tangents, Vector2[] uvs, ref int vertexIndex, float3 position, float3 normal, float3 tangent, float2 uv)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		vertices[vertexIndex] = float3.op_Implicit(position);
		normals[vertexIndex] = float3.op_Implicit(normal);
		tangents[vertexIndex] = float4.op_Implicit(new float4(tangent, -1f));
		uvs[vertexIndex] = float2.op_Implicit(uv);
		vertexIndex++;
	}

	private static void AddQuad(int[] indices, ref int indexIndex, int a, int b, int c, int d)
	{
		indices[indexIndex++] = a;
		indices[indexIndex++] = b;
		indices[indexIndex++] = c;
		indices[indexIndex++] = c;
		indices[indexIndex++] = d;
		indices[indexIndex++] = a;
	}

	private static Mesh CreateMesh(string name, Vector3[] vertices, Vector3[] normals, Vector4[] tangents, Color32[] colors, Vector4[] uvs, int[] indices)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		Mesh val = new Mesh
		{
			name = name,
			vertices = vertices,
			normals = normals,
			tangents = tangents,
			colors32 = colors
		};
		val.SetUVs(0, uvs);
		val.triangles = indices;
		val.bounds = new Bounds(Vector3.zero, new Vector3(1000f, 1000f, 1000f));
		return val;
	}

	private static void AddVertex(Vector3[] vertices, Vector3[] normals, Vector4[] tangents, Color32[] colors, Vector4[] uvs, ref int vertexIndex, float3 normal, float3 tangent, float2 uv, int2 m, float tx, float y, float tz)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		vertices[vertexIndex] = new Vector3(tx, y, tz);
		normals[vertexIndex] = float3.op_Implicit(normal);
		tangents[vertexIndex] = float4.op_Implicit(new float4(tangent, -1f));
		colors[vertexIndex] = new Color32((byte)m.x, (byte)m.y, (byte)0, (byte)0);
		uvs[vertexIndex] = new Vector4(uv.x, uv.y, 0.5f, 0f);
		vertexIndex++;
	}
}

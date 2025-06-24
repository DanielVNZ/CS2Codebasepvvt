using System;
using Colossal.IO.AssetDatabase;
using Colossal.Mathematics;
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

public static class ObjectMeshHelpers
{
	private struct TreeNode
	{
		public Bounds3 m_Bounds;

		public int m_FirstTriangle;

		public int m_ItemCount;

		public int m_NodeIndex;
	}

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
		public NativeArray<byte> m_Indices;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public Data m_Data;

		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public IndexFormat m_IndexFormat;

		[ReadOnly]
		public Bounds3 m_MeshBounds;

		[ReadOnly]
		public int m_VertexCount;

		[ReadOnly]
		public int m_IndexCount;

		[ReadOnly]
		public bool m_CacheNormals;

		public EntityCommandBuffer m_CommandBuffer;

		public unsafe void Execute()
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
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Invalid comparison between Unknown and I4
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Invalid comparison between Unknown and I4
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshVertex> dst = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshVertex>(m_Entity);
			DynamicBuffer<MeshIndex> val = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshIndex>(m_Entity);
			DynamicBuffer<MeshNode> val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshNode>(m_Entity);
			DynamicBuffer<MeshNormal> dst2 = default(DynamicBuffer<MeshNormal>);
			if (m_CacheNormals)
			{
				dst2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshNormal>(m_Entity);
			}
			IndexFormat val3 = (IndexFormat)0;
			NativeArray<byte> val4 = default(NativeArray<byte>);
			int num = 0;
			if (((Data)(ref m_Data)).IsValid)
			{
				int allVertexCount = GeometryAsset.GetAllVertexCount(ref m_Data);
				dst.ResizeUninitialized(allVertexCount);
				if (m_CacheNormals)
				{
					dst2.ResizeUninitialized(allVertexCount);
				}
				allVertexCount = 0;
				VertexAttributeFormat format = default(VertexAttributeFormat);
				int num2 = default(int);
				VertexAttributeFormat format2 = default(VertexAttributeFormat);
				int num3 = default(int);
				for (int i = 0; i < m_Data.meshCount; i++)
				{
					int vertexCount = GeometryAsset.GetVertexCount(ref m_Data, i);
					GeometryAsset.GetAttributeFormat(ref m_Data, i, (VertexAttribute)0, ref format, ref num2);
					if (num2 == 0)
					{
						throw new Exception("Cannot cache geometry asset: mesh do not have a position");
					}
					NativeSlice<byte> attributeData = GeometryAsset.GetAttributeData(ref m_Data, i, (VertexAttribute)0);
					NativeArray<MeshVertex> subArray = dst.AsNativeArray().GetSubArray(allVertexCount, vertexCount);
					MeshVertex.Unpack(attributeData, subArray, vertexCount, format, num2);
					if (m_CacheNormals)
					{
						GeometryAsset.GetAttributeFormat(ref m_Data, i, (VertexAttribute)1, ref format2, ref num3);
						if (num3 == 0)
						{
							throw new Exception("Cannot cache geometry asset: mesh do not have a normal");
						}
						NativeSlice<byte> attributeData2 = GeometryAsset.GetAttributeData(ref m_Data, i, (VertexAttribute)1);
						NativeArray<MeshNormal> subArray2 = dst2.AsNativeArray().GetSubArray(allVertexCount, vertexCount);
						MeshNormal.Unpack(attributeData2, subArray2, vertexCount, format2, num3);
					}
					allVertexCount += vertexCount;
				}
				val3 = (IndexFormat)1;
				val4 = GeometryAsset.ConvertAllIndicesTo32(ref m_Data, (Allocator)2).Reinterpret<byte>(4);
				num = GeometryAsset.GetAllIndicesCount(ref m_Data);
			}
			else
			{
				MeshVertex.Unpack(m_Positions, dst, m_VertexCount, (VertexAttributeFormat)0, 3);
				if (m_CacheNormals)
				{
					MeshNormal.Unpack(m_Normals, dst2, m_VertexCount, (VertexAttributeFormat)0, 3);
				}
				val3 = m_IndexFormat;
				val4 = m_Indices;
				num = m_IndexCount;
			}
			if (num != 0)
			{
				val.ResizeUninitialized(num);
				CalculateTreeSize(num, m_MeshBounds, out var treeDepth, out var treeSize, out var sizeFactor, out var sizeOffset);
				NativeArray<TreeNode> val5 = default(NativeArray<TreeNode>);
				val5._002Ector(treeSize, (Allocator)2, (NativeArrayOptions)1);
				NativeArray<int> nextTriangle = default(NativeArray<int>);
				nextTriangle._002Ector(num / 3, (Allocator)2, (NativeArrayOptions)1);
				InitializeTree(val5, treeSize);
				if ((int)val3 == 1)
				{
					FillTreeNodes(val5, nextTriangle, dst.AsNativeArray(), val4.Reinterpret<int>(1), sizeOffset, sizeFactor, treeDepth);
				}
				else
				{
					FillTreeNodes(val5, nextTriangle, dst.AsNativeArray(), val4.Reinterpret<ushort>(1), sizeOffset, sizeFactor, treeDepth);
				}
				int* ptr = stackalloc int[16];
				UpdateNodes(val5, treeDepth, ptr);
				val2.ResizeUninitialized(ptr[treeDepth]);
				if ((int)val3 == 1)
				{
					FillMeshData(val5, nextTriangle, val2.AsNativeArray(), val4.Reinterpret<int>(1), val.AsNativeArray(), treeDepth, ptr);
				}
				else
				{
					FillMeshData(val5, nextTriangle, val2.AsNativeArray(), val4.Reinterpret<ushort>(1), val.AsNativeArray(), treeDepth, ptr);
				}
				nextTriangle.Dispose();
				val5.Dispose();
				if (((Data)(ref m_Data)).IsValid)
				{
					val4.Dispose();
				}
			}
		}
	}

	[BurstCompile]
	private struct CacheProceduralMeshDataJob : IJob
	{
		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeSlice<byte> m_Positions;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeSlice<byte> m_Normals;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeSlice<byte> m_BoneIds;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeSlice<byte> m_BoneInfluences;

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
		public int m_BoneCount;

		[ReadOnly]
		public bool m_CacheNormals;

		public EntityCommandBuffer m_CommandBuffer;

		public unsafe void Execute()
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
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Invalid comparison between Unknown and I4
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Invalid comparison between Unknown and I4
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_0651: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Invalid comparison between Unknown and I4
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Invalid comparison between Unknown and I4
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshVertex> dst = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshVertex>(m_Entity);
			DynamicBuffer<MeshIndex> val = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshIndex>(m_Entity);
			DynamicBuffer<MeshNode> val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshNode>(m_Entity);
			DynamicBuffer<MeshNormal> dst2 = default(DynamicBuffer<MeshNormal>);
			if (m_CacheNormals)
			{
				dst2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<MeshNormal>(m_Entity);
			}
			IndexFormat val3 = (IndexFormat)0;
			NativeArray<byte> val4 = default(NativeArray<byte>);
			int num = 0;
			NativeArray<BoneData> bones = default(NativeArray<BoneData>);
			NativeArray<int> boneIndex = default(NativeArray<int>);
			if (((Data)(ref m_Data)).IsValid)
			{
				int allVertexCount = GeometryAsset.GetAllVertexCount(ref m_Data);
				dst.ResizeUninitialized(allVertexCount);
				if (m_CacheNormals)
				{
					dst2.ResizeUninitialized(allVertexCount);
				}
				val3 = (IndexFormat)1;
				val4 = GeometryAsset.ConvertAllIndicesTo32(ref m_Data, (Allocator)2).Reinterpret<byte>(4);
				num = GeometryAsset.GetAllIndicesCount(ref m_Data);
				bones._002Ector(m_BoneCount, (Allocator)2, (NativeArrayOptions)1);
				boneIndex._002Ector(num / 3, (Allocator)2, (NativeArrayOptions)1);
				InitializeBones(bones, num);
				allVertexCount = 0;
				num = 0;
				VertexAttributeFormat format = default(VertexAttributeFormat);
				int num2 = default(int);
				VertexAttributeFormat format2 = default(VertexAttributeFormat);
				int num3 = default(int);
				VertexAttributeFormat val5 = default(VertexAttributeFormat);
				int num4 = default(int);
				VertexAttributeFormat val6 = default(VertexAttributeFormat);
				int num5 = default(int);
				for (int i = 0; i < m_Data.meshCount; i++)
				{
					int vertexCount = GeometryAsset.GetVertexCount(ref m_Data, i);
					int indicesCount = GeometryAsset.GetIndicesCount(ref m_Data, i);
					GeometryAsset.GetAttributeFormat(ref m_Data, i, (VertexAttribute)0, ref format, ref num2);
					if (num2 == 0)
					{
						throw new Exception("Cannot cache geometry asset: mesh do not have a position");
					}
					NativeSlice<byte> attributeData = GeometryAsset.GetAttributeData(ref m_Data, i, (VertexAttribute)0);
					NativeArray<MeshVertex> subArray = dst.AsNativeArray().GetSubArray(allVertexCount, vertexCount);
					MeshVertex.Unpack(attributeData, subArray, vertexCount, format, num2);
					if (m_CacheNormals)
					{
						GeometryAsset.GetAttributeFormat(ref m_Data, i, (VertexAttribute)1, ref format2, ref num3);
						if (num3 == 0)
						{
							throw new Exception("Cannot cache geometry asset: mesh do not have a normal");
						}
						NativeSlice<byte> attributeData2 = GeometryAsset.GetAttributeData(ref m_Data, i, (VertexAttribute)1);
						NativeArray<MeshNormal> subArray2 = dst2.AsNativeArray().GetSubArray(allVertexCount, vertexCount);
						MeshNormal.Unpack(attributeData2, subArray2, vertexCount, format2, num3);
					}
					GeometryAsset.GetAttributeFormat(ref m_Data, i, (VertexAttribute)13, ref val5, ref num4);
					GeometryAsset.GetAttributeFormat(ref m_Data, i, (VertexAttribute)12, ref val6, ref num5);
					if (num4 == 0)
					{
						throw new Exception("Cannot cache geometry asset: mesh do not have bone ID data");
					}
					if ((int)val5 != 10 && (int)val5 != 6)
					{
						throw new Exception("Cannot cache geometry asset: only UInt32 or UInt8 bone IDs format is supported");
					}
					if (num5 != 0 && (int)val6 != 0 && (int)val6 != 2)
					{
						throw new Exception("Cannot cache geometry asset: only Float32 or UNorm8 bone weights formats are supported");
					}
					NativeSlice<byte> attributeData3 = GeometryAsset.GetAttributeData(ref m_Data, i, (VertexAttribute)13);
					NativeSlice<byte> attributeData4 = GeometryAsset.GetAttributeData(ref m_Data, i, (VertexAttribute)12);
					FillBoneData(bones, boneIndex, subArray, allVertexCount, attributeData3, num4, val5, attributeData4, num5, val6, val4.GetSubArray(num * 4, indicesCount * 4), (IndexFormat)1, num);
					allVertexCount += vertexCount;
					num += indicesCount;
				}
			}
			else
			{
				MeshVertex.Unpack(m_Positions, dst, m_VertexCount, (VertexAttributeFormat)0, 3);
				if (m_CacheNormals)
				{
					MeshNormal.Unpack(m_Normals, dst2, m_VertexCount, (VertexAttributeFormat)0, 3);
				}
				val3 = m_IndexFormat;
				val4 = m_Indices;
				num = m_IndexCount;
				NativeSlice<byte> boneIds = m_BoneIds;
				NativeSlice<byte> boneInfluences = m_BoneInfluences;
				bones._002Ector(m_BoneCount, (Allocator)2, (NativeArrayOptions)1);
				boneIndex._002Ector(num / 3, (Allocator)2, (NativeArrayOptions)1);
				InitializeBones(bones, num);
				FillBoneData(bones, boneIndex, dst.AsNativeArray(), 0, boneIds, 4, (VertexAttributeFormat)10, boneInfluences, 4, (VertexAttributeFormat)0, val4, val3, 0);
			}
			if (num == 0)
			{
				return;
			}
			val.ResizeUninitialized(num);
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			for (int j = 0; j < m_BoneCount; j++)
			{
				BoneData boneData = bones[j];
				if (boneData.m_TriangleCount != 0)
				{
					num6 += boneData.m_TriangleCount;
					num8 = math.max(num8, boneData.m_TriangleCount);
					CalculateTreeSize(boneData.m_TriangleCount * 3, boneData.m_Bounds, out var _, out var treeSize, out var _, out var _);
					num7 += treeSize;
					num9 = math.max(num9, treeSize);
				}
				else
				{
					num7++;
				}
			}
			NativeArray<TreeNode> val7 = default(NativeArray<TreeNode>);
			val7._002Ector(num9, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val8 = default(NativeArray<int>);
			val8._002Ector(num8, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val9 = default(NativeArray<int>);
			val9._002Ector(num8 * 3, (Allocator)2, (NativeArrayOptions)1);
			val.ResizeUninitialized(num6 * 3);
			val2.ResizeUninitialized(num7);
			int num10 = 0;
			int num11 = m_BoneCount - 1;
			int* ptr = stackalloc int[16];
			for (int k = 0; k < m_BoneCount; k++)
			{
				BoneData boneData2 = bones[k];
				if (boneData2.m_TriangleCount != 0)
				{
					CalculateTreeSize(boneData2.m_TriangleCount * 3, boneData2.m_Bounds, out var treeDepth2, out var treeSize2, out var sizeFactor2, out var sizeOffset2);
					NativeArray<TreeNode> subArray3 = val7.GetSubArray(0, treeSize2);
					NativeArray<int> subArray4 = val8.GetSubArray(0, boneData2.m_TriangleCount);
					NativeArray<int> subArray5 = val9.GetSubArray(0, boneData2.m_TriangleCount * 3);
					InitializeTree(subArray3, treeSize2);
					if ((int)val3 == 1)
					{
						FillIndices(boneData2, boneIndex, val4.Reinterpret<int>(1), subArray5, k);
					}
					else
					{
						FillIndices(boneData2, boneIndex, val4.Reinterpret<ushort>(1), subArray5, k);
					}
					FillTreeNodes(subArray3, subArray4, dst.AsNativeArray(), subArray5, sizeOffset2, sizeFactor2, treeDepth2);
					UpdateNodes(subArray3, treeDepth2, ptr);
					NativeArray<MeshIndex> subArray6 = val.AsNativeArray().GetSubArray(num10, boneData2.m_TriangleCount * 3);
					NativeArray<MeshNode> subArray7 = val2.AsNativeArray().GetSubArray(num11, ptr[treeDepth2]);
					MeshNode meshNode = subArray7[0];
					FillMeshData(subArray3, subArray4, subArray7, subArray5, subArray6, treeDepth2, ptr);
					for (int l = 0; l < subArray7.Length; l++)
					{
						MeshNode meshNode2 = subArray7[l];
						ref int2 indexRange = ref meshNode2.m_IndexRange;
						indexRange += num10;
						meshNode2.m_SubNodes1 = math.select(meshNode2.m_SubNodes1, meshNode2.m_SubNodes1 + num11, meshNode2.m_SubNodes1 != -1);
						meshNode2.m_SubNodes2 = math.select(meshNode2.m_SubNodes2, meshNode2.m_SubNodes2 + num11, meshNode2.m_SubNodes2 != -1);
						subArray7[l] = meshNode2;
					}
					if (num11 != k)
					{
						val2[k] = subArray7[0];
						subArray7[0] = meshNode;
					}
					num10 += subArray6.Length;
					num11 += subArray7.Length - 1;
				}
				else
				{
					val2[k] = new MeshNode
					{
						m_IndexRange = int2.op_Implicit(num10),
						m_SubNodes1 = int4.op_Implicit(-1),
						m_SubNodes2 = int4.op_Implicit(-1)
					};
				}
			}
			val8.Dispose();
			val7.Dispose();
			bones.Dispose();
			boneIndex.Dispose();
			val2.Length = num11 + 1;
			val2.TrimExcess();
			if (((Data)(ref m_Data)).IsValid)
			{
				val4.Dispose();
			}
		}
	}

	private struct BoneData
	{
		public Bounds3 m_Bounds;

		public int2 m_TriangleRange;

		public int m_TriangleCount;
	}

	public static Mesh CreateDefaultMesh()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Expected O, but got Unknown
		int num = 36;
		Vector3[] vertices = (Vector3[])(object)new Vector3[24];
		Vector3[] normals = (Vector3[])(object)new Vector3[24];
		Vector4[] tangents = (Vector4[])(object)new Vector4[24];
		Vector2[] uv = (Vector2[])(object)new Vector2[24];
		int[] array = new int[num];
		int vertexIndex = 0;
		int indexIndex = 0;
		AddFace(vertices, normals, tangents, uv, array, ref vertexIndex, ref indexIndex, new float3(-1f, 0f, 0f), new float3(0f, 1f, 0f));
		AddFace(vertices, normals, tangents, uv, array, ref vertexIndex, ref indexIndex, new float3(0f, -1f, 0f), new float3(0f, 0f, 1f));
		AddFace(vertices, normals, tangents, uv, array, ref vertexIndex, ref indexIndex, new float3(0f, 0f, -1f), new float3(1f, 0f, 0f));
		AddFace(vertices, normals, tangents, uv, array, ref vertexIndex, ref indexIndex, new float3(1f, 0f, 0f), new float3(0f, -1f, 0f));
		AddFace(vertices, normals, tangents, uv, array, ref vertexIndex, ref indexIndex, new float3(0f, 1f, 0f), new float3(0f, 0f, -1f));
		AddFace(vertices, normals, tangents, uv, array, ref vertexIndex, ref indexIndex, new float3(0f, 0f, 1f), new float3(-1f, 0f, 0f));
		return new Mesh
		{
			name = "Default object",
			vertices = vertices,
			normals = normals,
			tangents = tangents,
			uv = uv,
			triangles = array
		};
	}

	public static Mesh CreateDefaultBaseMesh()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		int num = 24;
		Vector3[] vertices = (Vector3[])(object)new Vector3[16];
		Vector3[] normals = (Vector3[])(object)new Vector3[16];
		Vector4[] tangents = (Vector4[])(object)new Vector4[16];
		Vector2[] uv = (Vector2[])(object)new Vector2[16];
		int[] array = new int[num];
		int vertexIndex = 0;
		int indexIndex = 0;
		AddBaseFace(vertices, normals, tangents, uv, array, ref vertexIndex, ref indexIndex, new float3(-1f, 0f, 0f), new float3(0f, 0f, -1f));
		AddBaseFace(vertices, normals, tangents, uv, array, ref vertexIndex, ref indexIndex, new float3(0f, 0f, -1f), new float3(1f, 0f, 0f));
		AddBaseFace(vertices, normals, tangents, uv, array, ref vertexIndex, ref indexIndex, new float3(1f, 0f, 0f), new float3(0f, 0f, 1f));
		AddBaseFace(vertices, normals, tangents, uv, array, ref vertexIndex, ref indexIndex, new float3(0f, 0f, 1f), new float3(-1f, 0f, 0f));
		return new Mesh
		{
			name = "Default base",
			vertices = vertices,
			normals = normals,
			tangents = tangents,
			uv = uv,
			triangles = array
		};
	}

	public static JobHandle CacheMeshData(RenderPrefab meshPrefab, GeometryAsset meshData, Entity entity, int boneCount, bool cacheNormals, EntityCommandBuffer commandBuffer)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (boneCount != 0)
		{
			return IJobExtensions.Schedule<CacheProceduralMeshDataJob>(new CacheProceduralMeshDataJob
			{
				m_Data = meshData.data,
				m_Entity = entity,
				m_BoneCount = boneCount,
				m_CacheNormals = cacheNormals,
				m_CommandBuffer = commandBuffer
			}, default(JobHandle));
		}
		return IJobExtensions.Schedule<CacheMeshDataJob>(new CacheMeshDataJob
		{
			m_Data = meshData.data,
			m_Entity = entity,
			m_MeshBounds = meshPrefab.bounds,
			m_CacheNormals = cacheNormals,
			m_CommandBuffer = commandBuffer
		}, default(JobHandle));
	}

	public static void CacheMeshData(Mesh mesh, Entity entity, bool cacheNormals, EntityCommandBuffer commandBuffer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<MeshVertex> val = ((EntityCommandBuffer)(ref commandBuffer)).AddBuffer<MeshVertex>(entity);
		DynamicBuffer<MeshIndex> val2 = ((EntityCommandBuffer)(ref commandBuffer)).AddBuffer<MeshIndex>(entity);
		DynamicBuffer<MeshNormal> val3 = default(DynamicBuffer<MeshNormal>);
		if (cacheNormals)
		{
			val3 = ((EntityCommandBuffer)(ref commandBuffer)).AddBuffer<MeshNormal>(entity);
		}
		MeshDataArray val4 = Mesh.AcquireReadOnlyMeshData(mesh);
		MeshData val5 = ((MeshDataArray)(ref val4))[0];
		int num = 0;
		int subMeshCount = ((MeshData)(ref val5)).subMeshCount;
		SubMeshDescriptor subMesh;
		for (int i = 0; i < subMeshCount; i++)
		{
			int num2 = num;
			subMesh = ((MeshData)(ref val5)).GetSubMesh(i);
			num = num2 + ((SubMeshDescriptor)(ref subMesh)).indexCount;
		}
		val.ResizeUninitialized(((MeshData)(ref val5)).vertexCount);
		val2.ResizeUninitialized(num);
		((MeshData)(ref val5)).GetVertices(val.AsNativeArray().Reinterpret<Vector3>());
		if (cacheNormals)
		{
			val3.ResizeUninitialized(((MeshData)(ref val5)).vertexCount);
			((MeshData)(ref val5)).GetNormals(val3.AsNativeArray().Reinterpret<Vector3>());
		}
		num = 0;
		for (int j = 0; j < subMeshCount; j++)
		{
			subMesh = ((MeshData)(ref val5)).GetSubMesh(j);
			int indexCount = ((SubMeshDescriptor)(ref subMesh)).indexCount;
			((MeshData)(ref val5)).GetIndices(val2.AsNativeArray().GetSubArray(num, indexCount).Reinterpret<int>(), j, true);
			num += indexCount;
		}
		((MeshDataArray)(ref val4)).Dispose();
	}

	public static void UncacheMeshData(Entity entity, EntityCommandBuffer commandBuffer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		((EntityCommandBuffer)(ref commandBuffer)).RemoveComponent<MeshVertex>(entity);
		((EntityCommandBuffer)(ref commandBuffer)).RemoveComponent<MeshNormal>(entity);
		((EntityCommandBuffer)(ref commandBuffer)).RemoveComponent<MeshIndex>(entity);
		((EntityCommandBuffer)(ref commandBuffer)).RemoveComponent<MeshNode>(entity);
	}

	private static void AddFace(Vector3[] vertices, Vector3[] normals, Vector4[] tangents, Vector2[] uv, int[] indices, ref int vertexIndex, ref int indexIndex, float3 normal, float3 tangent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.cross(normal, tangent);
		vertices[vertexIndex] = float3.op_Implicit(normal + tangent + val);
		normals[vertexIndex] = float3.op_Implicit(normal);
		tangents[vertexIndex] = float4.op_Implicit(new float4(tangent, -1f));
		uv[vertexIndex] = new Vector2(1f, 0f);
		vertexIndex++;
		vertices[vertexIndex] = float3.op_Implicit(normal - tangent + val);
		normals[vertexIndex] = float3.op_Implicit(normal);
		tangents[vertexIndex] = float4.op_Implicit(new float4(tangent, -1f));
		uv[vertexIndex] = new Vector2(0f, 0f);
		vertexIndex++;
		vertices[vertexIndex] = float3.op_Implicit(normal - tangent - val);
		normals[vertexIndex] = float3.op_Implicit(normal);
		tangents[vertexIndex] = float4.op_Implicit(new float4(tangent, -1f));
		uv[vertexIndex] = new Vector2(0f, 1f);
		vertexIndex++;
		vertices[vertexIndex] = float3.op_Implicit(normal + tangent - val);
		normals[vertexIndex] = float3.op_Implicit(normal);
		tangents[vertexIndex] = float4.op_Implicit(new float4(tangent, -1f));
		uv[vertexIndex] = new Vector2(1f, 1f);
		vertexIndex++;
		indices[indexIndex++] = vertexIndex - 4;
		indices[indexIndex++] = vertexIndex - 3;
		indices[indexIndex++] = vertexIndex - 2;
		indices[indexIndex++] = vertexIndex - 2;
		indices[indexIndex++] = vertexIndex - 1;
		indices[indexIndex++] = vertexIndex - 4;
	}

	private static void AddBaseFace(Vector3[] vertices, Vector3[] normals, Vector4[] tangents, Vector2[] uv, int[] indices, ref int vertexIndex, ref int indexIndex, float3 normal, float3 tangent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.cross(normal, tangent) * 0.5f;
		float3 val2 = default(float3);
		((float3)(ref val2))._002Ector(0f, -1.5f, 0f);
		vertices[vertexIndex] = float3.op_Implicit(normal + tangent + val + val2);
		normals[vertexIndex] = float3.op_Implicit(normal);
		tangents[vertexIndex] = float4.op_Implicit(new float4(tangent, -1f));
		uv[vertexIndex] = new Vector2(1f, 0f);
		vertexIndex++;
		vertices[vertexIndex] = float3.op_Implicit(normal - tangent + val + val2);
		normals[vertexIndex] = float3.op_Implicit(normal);
		tangents[vertexIndex] = float4.op_Implicit(new float4(tangent, -1f));
		uv[vertexIndex] = new Vector2(0f, 0f);
		vertexIndex++;
		vertices[vertexIndex] = float3.op_Implicit(normal - tangent - val + val2);
		normals[vertexIndex] = float3.op_Implicit(normal);
		tangents[vertexIndex] = float4.op_Implicit(new float4(tangent, -1f));
		uv[vertexIndex] = new Vector2(0f, 1f);
		vertexIndex++;
		vertices[vertexIndex] = float3.op_Implicit(normal + tangent - val + val2);
		normals[vertexIndex] = float3.op_Implicit(normal);
		tangents[vertexIndex] = float4.op_Implicit(new float4(tangent, -1f));
		uv[vertexIndex] = new Vector2(1f, 1f);
		vertexIndex++;
		indices[indexIndex++] = vertexIndex - 4;
		indices[indexIndex++] = vertexIndex - 3;
		indices[indexIndex++] = vertexIndex - 2;
		indices[indexIndex++] = vertexIndex - 2;
		indices[indexIndex++] = vertexIndex - 1;
		indices[indexIndex++] = vertexIndex - 4;
	}

	private static void InitializeBones(NativeArray<BoneData> bones, int indexCount)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		int length = bones.Length;
		int num = indexCount / 3;
		for (int i = 0; i < length; i++)
		{
			bones[i] = new BoneData
			{
				m_Bounds = new Bounds3(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue)),
				m_TriangleRange = new int2(num, -1)
			};
		}
	}

	private unsafe static void FillBoneData(NativeArray<BoneData> bones, NativeArray<int> boneIndex, NativeArray<MeshVertex> vertices, int vertexOffset, NativeSlice<byte> boneIdsData, int boneIdsDim, VertexAttributeFormat boneIdsFormat, NativeSlice<byte> weightsData, int weightsDim, VertexAttributeFormat weightsFormat, NativeArray<byte> indexData, IndexFormat indexFormat, int indexOffset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		int* ptr = (int*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<byte>(indexData);
		ushort* ptr2 = (ushort*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<byte>(indexData);
		bool flag = (int)boneIdsFormat == 6;
		byte* unsafeReadOnlyPtr = (byte*)NativeSliceUnsafeUtility.GetUnsafeReadOnlyPtr<byte>(boneIdsData);
		bool flag2 = (int)weightsFormat == 2;
		byte* unsafeReadOnlyPtr2 = (byte*)NativeSliceUnsafeUtility.GetUnsafeReadOnlyPtr<byte>(weightsData);
		int num = (((int)indexFormat == 0) ? 2 : 4);
		int num2 = indexData.Length / (3 * num);
		indexOffset /= 3;
		int3 val = default(int3);
		Triangle3 triangle = default(Triangle3);
		int3 val3 = default(int3);
		for (int i = 0; i < num2; i++)
		{
			if (num == 2)
			{
				val.x = *ptr2;
				val.y = ptr2[1];
				val.z = ptr2[2];
			}
			else
			{
				val.x = *ptr;
				val.y = ptr[1];
				val.z = ptr[2];
			}
			val -= vertexOffset;
			((Triangle3)(ref triangle))._002Ector(vertices[val.x].m_Vertex, vertices[val.y].m_Vertex, vertices[val.z].m_Vertex);
			int3 val2 = val * boneIdsDim;
			if (flag)
			{
				((int3)(ref val3))._002Ector((int)unsafeReadOnlyPtr[val2.x], (int)unsafeReadOnlyPtr[val2.y], (int)unsafeReadOnlyPtr[val2.z]);
			}
			else
			{
				((int3)(ref val3))._002Ector(((int*)unsafeReadOnlyPtr)[val2.x], ((int*)unsafeReadOnlyPtr)[val2.y], ((int*)unsafeReadOnlyPtr)[val2.z]);
			}
			int3 val4 = val * weightsDim;
			bool3 val5 = ((weightsDim != 0) ? ((!flag2) ? (new float3(((float*)unsafeReadOnlyPtr2)[val4.x], ((float*)unsafeReadOnlyPtr2)[val4.y], ((float*)unsafeReadOnlyPtr2)[val4.z]) < new float3(0.5f)) : (new float3((float)(int)unsafeReadOnlyPtr2[val4.x], (float)(int)unsafeReadOnlyPtr2[val4.y], (float)(int)unsafeReadOnlyPtr2[val4.z]) < new float3(128))) : bool3.op_Implicit(false));
			val3 = math.select(val3, new int3(-1), val5);
			AddTriangle(bones, boneIndex, triangle, val3, indexOffset + i);
			ptr += 3;
			ptr2 += 3;
		}
	}

	private static void AddTriangle(NativeArray<BoneData> bones, NativeArray<int> boneIndex, Triangle3 triangle, int3 boneID, int triangleIndex)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (boneID.x >= 0 && boneID.x < bones.Length && math.all(((int3)(ref boneID)).xx == ((int3)(ref boneID)).yz))
		{
			BoneData boneData = bones[boneID.x];
			ref Bounds3 bounds = ref boneData.m_Bounds;
			bounds |= MathUtils.Bounds(triangle);
			boneData.m_TriangleRange.x = math.min(boneData.m_TriangleRange.x, triangleIndex);
			boneData.m_TriangleRange.y = math.max(boneData.m_TriangleRange.y, triangleIndex);
			boneData.m_TriangleCount++;
			bones[boneID.x] = boneData;
			boneIndex[triangleIndex] = boneID.x;
		}
		else
		{
			boneIndex[triangleIndex] = -1;
		}
	}

	private static void FillIndices(BoneData boneData, NativeArray<int> boneIndex, NativeArray<int> sourceIndices, NativeArray<int> targetIndices, int bone)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = boneData.m_TriangleRange.x; i <= boneData.m_TriangleRange.y; i++)
		{
			if (boneIndex[i] == bone)
			{
				int3 val = i * 3 + new int3(0, 1, 2);
				targetIndices[num++] = sourceIndices[val.x];
				targetIndices[num++] = sourceIndices[val.y];
				targetIndices[num++] = sourceIndices[val.z];
			}
		}
	}

	private static void FillIndices(BoneData boneData, NativeArray<int> boneIndex, NativeArray<ushort> sourceIndices, NativeArray<int> targetIndices, int bone)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = boneData.m_TriangleRange.x; i <= boneData.m_TriangleRange.y; i++)
		{
			if (boneIndex[i] == bone)
			{
				int3 val = i * 3 + new int3(0, 1, 2);
				targetIndices[num++] = sourceIndices[val.x];
				targetIndices[num++] = sourceIndices[val.y];
				targetIndices[num++] = sourceIndices[val.z];
			}
		}
	}

	private static void CalculateTreeSize(int indexCount, Bounds3 bounds, out int treeDepth, out int treeSize, out float3 sizeFactor, out float3 sizeOffset)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		treeDepth = 1;
		treeSize = 1;
		for (int num = indexCount / 3; num >= 32; num >>= 3)
		{
			treeSize += 1 << 3 * treeDepth++;
		}
		sizeFactor = 1f / math.max(float3.op_Implicit(0.001f), MathUtils.Size(bounds));
		sizeOffset = 0.5f - MathUtils.Center(bounds) * sizeFactor;
	}

	private static void InitializeTree(NativeArray<TreeNode> treeNodes, int treeSize)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < treeSize; i++)
		{
			treeNodes[i] = new TreeNode
			{
				m_Bounds = new Bounds3(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue)),
				m_FirstTriangle = -1
			};
		}
	}

	private static void FillTreeNodes(NativeArray<TreeNode> treeNodes, NativeArray<int> nextTriangle, NativeArray<MeshVertex> vertices, NativeArray<int> indices, float3 sizeOffset, float3 sizeFactor, int treeDepth)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		int length = nextTriangle.Length;
		Triangle3 triangle = default(Triangle3);
		for (int i = 0; i < length; i++)
		{
			int3 val = i * 3 + new int3(0, 1, 2);
			((Triangle3)(ref triangle))._002Ector(vertices[indices[val.x]].m_Vertex, vertices[indices[val.y]].m_Vertex, vertices[indices[val.z]].m_Vertex);
			AddTriangle(treeNodes, nextTriangle, sizeOffset, sizeFactor, treeDepth, triangle, i);
		}
	}

	private static void FillTreeNodes(NativeArray<TreeNode> treeNodes, NativeArray<int> nextTriangle, NativeArray<MeshVertex> vertices, NativeArray<ushort> indices, float3 sizeOffset, float3 sizeFactor, int treeDepth)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		int length = nextTriangle.Length;
		Triangle3 triangle = default(Triangle3);
		for (int i = 0; i < length; i++)
		{
			int3 val = i * 3 + new int3(0, 1, 2);
			((Triangle3)(ref triangle))._002Ector(vertices[(int)indices[val.x]].m_Vertex, vertices[(int)indices[val.y]].m_Vertex, vertices[(int)indices[val.z]].m_Vertex);
			AddTriangle(treeNodes, nextTriangle, sizeOffset, sizeFactor, treeDepth, triangle, i);
		}
	}

	private static void AddTriangle(NativeArray<TreeNode> treeNodes, NativeArray<int> nextTriangle, float3 sizeOffset, float3 sizeFactor, int treeDepth, Triangle3 triangle, int index)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = MathUtils.Bounds(triangle);
		float3 val2 = MathUtils.Center(val) * sizeFactor + sizeOffset;
		float num = math.cmax(MathUtils.Size(val) * sizeFactor);
		int num2 = treeDepth - 1;
		int num3 = 0;
		int num4 = 0;
		while (num <= 0.5f && num4 < num2)
		{
			num3 += 1 << 3 * num4++;
			num *= 2f;
		}
		int num5 = 1 << num4;
		int3 val3 = math.clamp((int3)(val2 * (float)num5), int3.op_Implicit(0), int3.op_Implicit(num5 - 1));
		num3 += math.dot(val3, new int3(1, num5, num5 * num5));
		TreeNode treeNode = treeNodes[num3];
		nextTriangle[index] = treeNode.m_FirstTriangle;
		ref Bounds3 bounds = ref treeNode.m_Bounds;
		bounds |= val;
		treeNode.m_FirstTriangle = index;
		treeNode.m_ItemCount++;
		treeNodes[num3] = treeNode;
	}

	private unsafe static void UpdateNodes(NativeArray<TreeNode> treeNodes, int treeDepth, int* depthOffsets)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		int num = treeDepth - 1;
		int num2 = 0;
		int num3 = 0;
		while (num3 < num)
		{
			num2 += 1 << 3 * num3++;
		}
		int3 val = default(int3);
		int3 val2 = default(int3);
		int4 val3 = default(int4);
		int3 val5 = default(int3);
		while (num3 > 0)
		{
			int num4 = 1 << num3;
			int num5 = num2;
			num2 -= 1 << 3 * --num3;
			int num6 = 1 << num3;
			int num7 = num2;
			((int3)(ref val))._002Ector(2, num4 << 1, num4 * num4 << 1);
			((int3)(ref val2))._002Ector(1, num6, num6 * num6);
			((int4)(ref val3))._002Ector(0, 1, num4, num4 + 1);
			int4 val4 = num4 * num4 + val3;
			int sourceSize = 0;
			val5.z = 0;
			while (val5.z < num6)
			{
				val5.y = 0;
				while (val5.y < num6)
				{
					val5.x = 0;
					while (val5.x < num6)
					{
						int num8 = num5 + math.dot(val5, val);
						int num9 = num7 + math.dot(val5, val2);
						int4 val6 = num8 + val3;
						int4 val7 = num8 + val4;
						TreeNode targetNode = treeNodes[num9];
						AddBounds(ref targetNode, ref sourceSize, treeNodes, val6.x);
						AddBounds(ref targetNode, ref sourceSize, treeNodes, val6.y);
						AddBounds(ref targetNode, ref sourceSize, treeNodes, val6.z);
						AddBounds(ref targetNode, ref sourceSize, treeNodes, val6.w);
						AddBounds(ref targetNode, ref sourceSize, treeNodes, val7.x);
						AddBounds(ref targetNode, ref sourceSize, treeNodes, val7.y);
						AddBounds(ref targetNode, ref sourceSize, treeNodes, val7.z);
						AddBounds(ref targetNode, ref sourceSize, treeNodes, val7.w);
						treeNodes[num9] = targetNode;
						val5.x++;
					}
					val5.y++;
				}
				val5.z++;
			}
			depthOffsets[num3 + 2] = sourceSize;
		}
		*depthOffsets = 0;
		depthOffsets[1] = 1;
		for (int i = 1; i <= treeDepth; i++)
		{
			depthOffsets[i] += depthOffsets[i - 1];
		}
	}

	private static void AddBounds(ref TreeNode targetNode, ref int sourceSize, NativeArray<TreeNode> treeNodes, int sourceIndex)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		TreeNode treeNode = treeNodes[sourceIndex];
		if (treeNode.m_ItemCount != 0)
		{
			ref Bounds3 bounds = ref targetNode.m_Bounds;
			bounds |= treeNode.m_Bounds;
			targetNode.m_ItemCount++;
			if (treeNode.m_ItemCount != 1)
			{
				treeNode.m_NodeIndex = sourceSize++;
				treeNodes[sourceIndex] = treeNode;
			}
		}
	}

	private unsafe static void FillMeshData(NativeArray<TreeNode> sourceNodes, NativeArray<int> nextTriangle, NativeArray<MeshNode> targetNodes, NativeArray<int> sourceIndices, NativeArray<MeshIndex> targetIndices, int treeDepth, int* depthOffsets)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		int* ptr = stackalloc int[128];
		int* ptr2 = stackalloc int[128];
		int* ptr3 = stackalloc int[128];
		int num = 1;
		int num2 = 0;
		*ptr = 0;
		*ptr2 = 0;
		*ptr3 = 0;
		while (--num >= 0)
		{
			int num3 = ptr[num];
			int num4 = ptr2[num];
			int num5 = ptr3[num];
			int num6 = 1 << num5;
			TreeNode treeNode = sourceNodes[num3 + num4];
			int num7 = num2;
			int num8 = treeNode.m_FirstTriangle;
			while (num8 >= 0)
			{
				int3 val = num8 * 3 + new int3(0, 1, 2);
				int3 val2 = num2 + new int3(0, 1, 2);
				targetIndices[val2.x] = new MeshIndex(sourceIndices[val.x]);
				targetIndices[val2.y] = new MeshIndex(sourceIndices[val.y]);
				targetIndices[val2.z] = new MeshIndex(sourceIndices[val.z]);
				num8 = nextTriangle[num8];
				num2 += 3;
			}
			int num9 = 0;
			int4 subNodes = int4.op_Implicit(-1);
			int4 subNodes2 = int4.op_Implicit(-1);
			if (num5 + 1 < treeDepth)
			{
				int3 val3 = new int3(num4, num4 >> num5, num4 >> num5 + num5) & (num6 - 1);
				for (int i = 0; i < 8; i++)
				{
					int num10 = num3 + (1 << 3 * num5);
					int num11 = num5 + 1;
					int3 val4 = val3 * 2 + math.select(int3.op_Implicit(0), int3.op_Implicit(1), (i & new int3(1, 2, 4)) != 0);
					while (num11 < treeDepth)
					{
						int num12 = 1 << num11;
						int num13 = math.dot(val4, new int3(1, num12, num12 * num12));
						TreeNode treeNode2 = sourceNodes[num10 + num13];
						if (treeNode2.m_ItemCount == 1)
						{
							if (treeNode2.m_FirstTriangle != -1)
							{
								int3 val5 = treeNode2.m_FirstTriangle * 3 + new int3(0, 1, 2);
								int3 val6 = num2 + new int3(0, 1, 2);
								targetIndices[val6.x] = new MeshIndex(sourceIndices[val5.x]);
								targetIndices[val6.y] = new MeshIndex(sourceIndices[val5.y]);
								targetIndices[val6.z] = new MeshIndex(sourceIndices[val5.z]);
								num2 += 3;
								break;
							}
							num10 += 1 << 3 * num11++;
							val4 *= 2;
							continue;
						}
						if (treeNode2.m_ItemCount != 0)
						{
							if (num9 < 4)
							{
								((int4)(ref subNodes))[num9++] = depthOffsets[num11] + treeNode2.m_NodeIndex;
							}
							else
							{
								((int4)(ref subNodes2))[num9++ - 4] = depthOffsets[num11] + treeNode2.m_NodeIndex;
							}
							ptr[num] = num10;
							ptr2[num] = num13;
							ptr3[num] = num11;
							num++;
							break;
						}
						if (num11 == num5 + 1)
						{
							break;
						}
						if ((val4.x & 1) == 0)
						{
							val4.x++;
							continue;
						}
						if ((val4.y & 1) == 0)
						{
							((int3)(ref val4)).xy = ((int3)(ref val4)).xy + new int2(-1, 1);
							continue;
						}
						if ((val4.z & 1) != 0)
						{
							break;
						}
						val4 += new int3(-1, -1, 1);
					}
				}
			}
			int num14 = depthOffsets[num5] + treeNode.m_NodeIndex;
			targetNodes[num14] = new MeshNode
			{
				m_Bounds = treeNode.m_Bounds,
				m_IndexRange = new int2(num7, num2),
				m_SubNodes1 = subNodes,
				m_SubNodes2 = subNodes2
			};
		}
	}

	private unsafe static void FillMeshData(NativeArray<TreeNode> sourceNodes, NativeArray<int> nextTriangle, NativeArray<MeshNode> targetNodes, NativeArray<ushort> sourceIndices, NativeArray<MeshIndex> targetIndices, int treeDepth, int* depthOffsets)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		int* ptr = stackalloc int[128];
		int* ptr2 = stackalloc int[128];
		int* ptr3 = stackalloc int[128];
		int num = 1;
		int num2 = 0;
		*ptr = 0;
		*ptr2 = 0;
		*ptr3 = 0;
		while (--num >= 0)
		{
			int num3 = ptr[num];
			int num4 = ptr2[num];
			int num5 = ptr3[num];
			int num6 = 1 << num5;
			TreeNode treeNode = sourceNodes[num3 + num4];
			int num7 = num2;
			int num8 = treeNode.m_FirstTriangle;
			while (num8 >= 0)
			{
				int3 val = num8 * 3 + new int3(0, 1, 2);
				int3 val2 = num2 + new int3(0, 1, 2);
				targetIndices[val2.x] = new MeshIndex(sourceIndices[val.x]);
				targetIndices[val2.y] = new MeshIndex(sourceIndices[val.y]);
				targetIndices[val2.z] = new MeshIndex(sourceIndices[val.z]);
				num8 = nextTriangle[num8];
				num2 += 3;
			}
			int num9 = 0;
			int4 subNodes = int4.op_Implicit(-1);
			int4 subNodes2 = int4.op_Implicit(-1);
			if (num5 + 1 < treeDepth)
			{
				int3 val3 = new int3(num4, num4 >> num5, num4 >> num5 + num5) & (num6 - 1);
				for (int i = 0; i < 8; i++)
				{
					int num10 = num3 + (1 << 3 * num5);
					int num11 = num5 + 1;
					int3 val4 = val3 * 2 + math.select(int3.op_Implicit(0), int3.op_Implicit(1), (i & new int3(1, 2, 4)) != 0);
					while (num11 < treeDepth)
					{
						int num12 = 1 << num11;
						int num13 = math.dot(val4, new int3(1, num12, num12 * num12));
						TreeNode treeNode2 = sourceNodes[num10 + num13];
						if (treeNode2.m_ItemCount == 1)
						{
							if (treeNode2.m_FirstTriangle != -1)
							{
								int3 val5 = treeNode2.m_FirstTriangle * 3 + new int3(0, 1, 2);
								int3 val6 = num2 + new int3(0, 1, 2);
								targetIndices[val6.x] = new MeshIndex(sourceIndices[val5.x]);
								targetIndices[val6.y] = new MeshIndex(sourceIndices[val5.y]);
								targetIndices[val6.z] = new MeshIndex(sourceIndices[val5.z]);
								num2 += 3;
								break;
							}
							num10 += 1 << 3 * num11++;
							val4 *= 2;
							continue;
						}
						if (treeNode2.m_ItemCount != 0)
						{
							if (num9 < 4)
							{
								((int4)(ref subNodes))[num9++] = depthOffsets[num11] + treeNode2.m_NodeIndex;
							}
							else
							{
								((int4)(ref subNodes2))[num9++ - 4] = depthOffsets[num11] + treeNode2.m_NodeIndex;
							}
							ptr[num] = num10;
							ptr2[num] = num13;
							ptr3[num] = num11;
							num++;
							break;
						}
						if (num11 == num5 + 1)
						{
							break;
						}
						if ((val4.x & 1) == 0)
						{
							val4.x++;
							continue;
						}
						if ((val4.y & 1) == 0)
						{
							((int3)(ref val4)).xy = ((int3)(ref val4)).xy + new int2(-1, 1);
							continue;
						}
						if ((val4.z & 1) != 0)
						{
							break;
						}
						val4 += new int3(-1, -1, 1);
					}
				}
			}
			int num14 = depthOffsets[num5] + treeNode.m_NodeIndex;
			targetNodes[num14] = new MeshNode
			{
				m_Bounds = treeNode.m_Bounds,
				m_IndexRange = new int2(num7, num2),
				m_SubNodes1 = subNodes,
				m_SubNodes2 = subNodes2
			};
		}
	}
}

using Colossal.Mathematics;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Rendering;

public static class BatchMeshHelpers
{
	[BurstCompile]
	private struct GenerateBatchMeshJob : IJobParallelFor
	{
		private struct BasePoint
		{
			public float2 m_Position;

			public float2 m_Direction;

			public float2 m_PrevPos;

			public float m_Distance;
		}

		private struct BaseLine
		{
			public int m_StartIndex;

			public int m_EndIndex;
		}

		private struct VertexData
		{
			public float3 m_Position;

			public uint m_Normal;

			public uint m_Tangent;

			public Color32 m_Color;

			public half4 m_UV0;
		}

		[ReadOnly]
		public NativeList<Entity> m_Entities;

		[ReadOnly]
		public ComponentLookup<MeshData> m_MeshData;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshData> m_CompositionMeshData;

		[ReadOnly]
		public BufferLookup<NetCompositionPiece> m_CompositionPieces;

		[ReadOnly]
		public BufferLookup<MeshVertex> m_MeshVertices;

		[ReadOnly]
		public BufferLookup<MeshNormal> m_MeshNormals;

		[ReadOnly]
		public BufferLookup<MeshTangent> m_MeshTangents;

		[ReadOnly]
		public BufferLookup<MeshUV0> m_MeshUV0s;

		[ReadOnly]
		public BufferLookup<MeshIndex> m_MeshIndices;

		[ReadOnly]
		public BufferLookup<MeshNode> m_MeshNodes;

		[NativeDisableParallelForRestriction]
		public BufferLookup<MeshMaterial> m_MeshMaterials;

		public MeshDataArray m_MeshDataArray;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			MeshData objectMeshData = default(MeshData);
			NetCompositionMeshData compositionMeshData = default(NetCompositionMeshData);
			if (m_MeshData.TryGetComponent(val, ref objectMeshData))
			{
				DynamicBuffer<MeshVertex> cachedVertices = m_MeshVertices[val];
				DynamicBuffer<MeshIndex> cachedIndices = m_MeshIndices[val];
				DynamicBuffer<MeshNormal> cachedNormals = default(DynamicBuffer<MeshNormal>);
				if (m_MeshNormals.HasBuffer(val))
				{
					cachedNormals = m_MeshNormals[val];
				}
				DynamicBuffer<MeshNode> nodes = default(DynamicBuffer<MeshNode>);
				if (m_MeshNodes.HasBuffer(val))
				{
					nodes = m_MeshNodes[val];
				}
				GenerateObjectMesh(objectMeshData, cachedVertices, cachedNormals, cachedIndices, nodes, ((MeshDataArray)(ref m_MeshDataArray))[index]);
			}
			else if (m_CompositionMeshData.TryGetComponent(val, ref compositionMeshData))
			{
				DynamicBuffer<NetCompositionPiece> pieces = m_CompositionPieces[val];
				DynamicBuffer<MeshMaterial> materials = m_MeshMaterials[val];
				GenerateCompositionMesh(compositionMeshData, pieces, materials, ((MeshDataArray)(ref m_MeshDataArray))[index]);
			}
		}

		private int GetMaterial(DynamicBuffer<MeshMaterial> materials, MeshMaterial pieceMaterial)
		{
			int length = materials.Length;
			for (int i = 0; i < length; i++)
			{
				if (materials[i].m_MaterialIndex == pieceMaterial.m_MaterialIndex)
				{
					return i;
				}
			}
			materials.Add(new MeshMaterial
			{
				m_MaterialIndex = pieceMaterial.m_MaterialIndex
			});
			return length;
		}

		private void GenerateObjectMesh(MeshData objectMeshData, DynamicBuffer<MeshVertex> cachedVertices, DynamicBuffer<MeshNormal> cachedNormals, DynamicBuffer<MeshIndex> cachedIndices, DynamicBuffer<MeshNode> nodes, MeshData meshData)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			NativeList<BasePoint> basePoints = default(NativeList<BasePoint>);
			NativeList<BaseLine> baseLines = default(NativeList<BaseLine>);
			float baseOffset = 0f;
			if ((objectMeshData.m_State & MeshFlags.Base) != 0)
			{
				basePoints._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
				baseLines._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
				baseOffset = math.select(0f, objectMeshData.m_Bounds.min.y, (objectMeshData.m_State & MeshFlags.MinBounds) != 0);
				AddBaseLines(basePoints, baseLines, cachedVertices, cachedNormals, cachedIndices, nodes, baseOffset);
				num += basePoints.Length * 2;
				num2 += baseLines.Length * 6;
				num3++;
			}
			NativeArray<VertexAttributeDescriptor> val = default(NativeArray<VertexAttributeDescriptor>);
			val._002Ector(5, (Allocator)2, (NativeArrayOptions)0);
			SetupGeneratedMeshAttributes(val);
			bool flag = num2 >= 65536;
			((MeshData)(ref meshData)).SetVertexBufferParams(num, val);
			((MeshData)(ref meshData)).SetIndexBufferParams(num2, (IndexFormat)(flag ? 1 : 0));
			val.Dispose();
			((MeshData)(ref meshData)).subMeshCount = num3;
			num = 0;
			num2 = 0;
			num3 = 0;
			if ((objectMeshData.m_State & MeshFlags.Base) != 0)
			{
				int num4 = num3++;
				SubMeshDescriptor val2 = default(SubMeshDescriptor);
				((SubMeshDescriptor)(ref val2)).firstVertex = num;
				((SubMeshDescriptor)(ref val2)).indexStart = num2;
				((SubMeshDescriptor)(ref val2)).vertexCount = basePoints.Length * 2;
				((SubMeshDescriptor)(ref val2)).indexCount = baseLines.Length * 6;
				((SubMeshDescriptor)(ref val2)).topology = (MeshTopology)0;
				((MeshData)(ref meshData)).SetSubMesh(num4, val2, (MeshUpdateFlags)13);
			}
			NativeArray<VertexData> vertexData = ((MeshData)(ref meshData)).GetVertexData<VertexData>(0);
			NativeArray<uint> indices = default(NativeArray<uint>);
			NativeArray<ushort> indices2 = default(NativeArray<ushort>);
			if (flag)
			{
				indices = ((MeshData)(ref meshData)).GetIndexData<uint>();
			}
			else
			{
				indices2 = ((MeshData)(ref meshData)).GetIndexData<ushort>();
			}
			num = 0;
			num2 = 0;
			if ((objectMeshData.m_State & MeshFlags.Base) != 0)
			{
				AddBaseVertices(basePoints, baseLines, vertexData, indices, indices2, flag, baseOffset, ref num, ref num2);
				basePoints.Dispose();
				baseLines.Dispose();
			}
		}

		private void AddBaseVertices(NativeList<BasePoint> basePoints, NativeList<BaseLine> baseLines, NativeArray<VertexData> vertices, NativeArray<uint> indices32, NativeArray<ushort> indices16, bool use32bitIndices, float baseOffset, ref int vertexCount, ref int indexCount)
		{
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < baseLines.Length; i++)
			{
				BaseLine baseLine = baseLines[i];
				int num = vertexCount + baseLine.m_StartIndex * 2;
				int num2 = vertexCount + baseLine.m_EndIndex * 2;
				if (use32bitIndices)
				{
					indices32[indexCount++] = (uint)num;
					indices32[indexCount++] = (uint)(num + 1);
					indices32[indexCount++] = (uint)num2;
					indices32[indexCount++] = (uint)num2;
					indices32[indexCount++] = (uint)(num + 1);
					indices32[indexCount++] = (uint)(num2 + 1);
				}
				else
				{
					indices16[indexCount++] = (ushort)num;
					indices16[indexCount++] = (ushort)(num + 1);
					indices16[indexCount++] = (ushort)num2;
					indices16[indexCount++] = (ushort)num2;
					indices16[indexCount++] = (ushort)(num + 1);
					indices16[indexCount++] = (ushort)(num2 + 1);
				}
			}
			float3 val = default(float3);
			for (int j = 0; j < basePoints.Length; j++)
			{
				BasePoint basePoint = basePoints[j];
				((float3)(ref val))._002Ector(basePoint.m_Direction.x, 0f, basePoint.m_Direction.y);
				float4 val2 = new float4(val.z, 0f, 0f - val.x, 1f);
				uint normal = MathUtils.NormalToOctahedral(val);
				uint tangent = MathUtils.TangentToOctahedral(val2);
				vertices[vertexCount++] = new VertexData
				{
					m_Position = new float3(basePoint.m_Position.x, baseOffset, basePoint.m_Position.y),
					m_Color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
					m_Normal = normal,
					m_Tangent = tangent,
					m_UV0 = new half4(new float4(basePoint.m_Distance, 1f, 0f, 0f))
				};
				vertices[vertexCount++] = new VertexData
				{
					m_Position = new float3(basePoint.m_Position.x, baseOffset - 1f, basePoint.m_Position.y),
					m_Color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
					m_Normal = normal,
					m_Tangent = tangent,
					m_UV0 = new half4(new float4(basePoint.m_Distance, 0f, 0f, 0f))
				};
			}
		}

		private unsafe void AddBaseLines(NativeList<BasePoint> basePoints, NativeList<BaseLine> baseLines, DynamicBuffer<MeshVertex> vertices, DynamicBuffer<MeshNormal> normals, DynamicBuffer<MeshIndex> indices, DynamicBuffer<MeshNode> nodes, float baseOffset)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0944: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b48: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_095d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0970: Unknown result type (might be due to invalid IL or missing references)
			//IL_0991: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0adc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0671: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_071c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Unknown result type (might be due to invalid IL or missing references)
			//IL_073a: Unknown result type (might be due to invalid IL or missing references)
			//IL_073f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0759: Unknown result type (might be due to invalid IL or missing references)
			//IL_075e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0767: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_068e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0693: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bac: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0803: Unknown result type (might be due to invalid IL or missing references)
			//IL_0864: Unknown result type (might be due to invalid IL or missing references)
			//IL_0837: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c79: Unknown result type (might be due to invalid IL or missing references)
			//IL_087d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0900: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08da: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
			NativeHashSet<float4> val = default(NativeHashSet<float4>);
			val._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			NativeHashMap<float2, int> val2 = default(NativeHashMap<float2, int>);
			val2._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<int> val3 = default(NativeList<int>);
			val3._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			float2 val4 = default(float2);
			((float2)(ref val4))._002Ector(baseOffset - 0.01f, baseOffset + 0.01f);
			int* ptr = stackalloc int[128];
			int num = 0;
			if (nodes.IsCreated && nodes.Length != 0)
			{
				ptr[num++] = 0;
			}
			int3 val5 = default(int3);
			Triangle3 val6 = default(Triangle3);
			Triangle1 y;
			while (--num >= 0)
			{
				int num2 = ptr[num];
				MeshNode meshNode = nodes[num2];
				if (!(meshNode.m_Bounds.min.y < val4.y) || !(meshNode.m_Bounds.max.y > val4.x))
				{
					continue;
				}
				for (int i = meshNode.m_IndexRange.x; i < meshNode.m_IndexRange.y; i += 3)
				{
					((int3)(ref val5))._002Ector(i, i + 1, i + 2);
					((int3)(ref val5))._002Ector(indices[val5.x].m_Index, indices[val5.y].m_Index, indices[val5.z].m_Index);
					((Triangle3)(ref val6))._002Ector(vertices[val5.x].m_Vertex, vertices[val5.y].m_Vertex, vertices[val5.z].m_Vertex);
					y = ((Triangle3)(ref val6)).y;
					bool3 val7 = ((Triangle1)(ref y)).abc > val4.x;
					y = ((Triangle3)(ref val6)).y;
					bool3 val8 = val7 & (((Triangle1)(ref y)).abc < val4.y);
					y = ((Triangle3)(ref val6)).y;
					bool3 val9 = (((Triangle1)(ref y)).abc < val4.y) & ((bool3)(ref val8)).yzx & ((bool3)(ref val8)).zxy;
					if (math.any(val9))
					{
						if (val9.x)
						{
							val.Add(new float4(((float3)(ref val6.b)).xz, ((float3)(ref val6.c)).xz));
						}
						if (val9.y)
						{
							val.Add(new float4(((float3)(ref val6.c)).xz, ((float3)(ref val6.a)).xz));
						}
						if (val9.z)
						{
							val.Add(new float4(((float3)(ref val6.a)).xz, ((float3)(ref val6.b)).xz));
						}
					}
				}
				ptr[num] = meshNode.m_SubNodes1.x;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.x != -1);
				ptr[num] = meshNode.m_SubNodes1.y;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.y != -1);
				ptr[num] = meshNode.m_SubNodes1.z;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.z != -1);
				ptr[num] = meshNode.m_SubNodes1.w;
				num = math.select(num, num + 1, meshNode.m_SubNodes1.w != -1);
				ptr[num] = meshNode.m_SubNodes2.x;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.x != -1);
				ptr[num] = meshNode.m_SubNodes2.y;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.y != -1);
				ptr[num] = meshNode.m_SubNodes2.z;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.z != -1);
				ptr[num] = meshNode.m_SubNodes2.w;
				num = math.select(num, num + 1, meshNode.m_SubNodes2.w != -1);
			}
			num = 0;
			if (nodes.IsCreated && nodes.Length != 0)
			{
				ptr[num++] = 0;
			}
			int3 val10 = default(int3);
			Triangle3 val11 = default(Triangle3);
			int num4 = default(int);
			int num5 = default(int);
			while (--num >= 0)
			{
				int num3 = ptr[num];
				MeshNode meshNode2 = nodes[num3];
				if (!(meshNode2.m_Bounds.min.y < val4.y) || !(meshNode2.m_Bounds.max.y > val4.x))
				{
					continue;
				}
				for (int j = meshNode2.m_IndexRange.x; j < meshNode2.m_IndexRange.y; j += 3)
				{
					((int3)(ref val10))._002Ector(j, j + 1, j + 2);
					((int3)(ref val10))._002Ector(indices[val10.x].m_Index, indices[val10.y].m_Index, indices[val10.z].m_Index);
					((Triangle3)(ref val11))._002Ector(vertices[val10.x].m_Vertex, vertices[val10.y].m_Vertex, vertices[val10.z].m_Vertex);
					y = ((Triangle3)(ref val11)).y;
					bool3 val12 = ((Triangle1)(ref y)).abc > val4.x;
					y = ((Triangle3)(ref val11)).y;
					bool3 val13 = val12 & (((Triangle1)(ref y)).abc < val4.y);
					y = ((Triangle3)(ref val11)).y;
					bool3 val14 = (((Triangle1)(ref y)).abc >= val4.y) & ((bool3)(ref val13)).yzx & ((bool3)(ref val13)).zxy;
					if (!math.any(val14))
					{
						continue;
					}
					MeshNormal meshNormal;
					BasePoint basePoint2;
					BasePoint basePoint3;
					if (val14.x)
					{
						BasePoint basePoint = new BasePoint
						{
							m_Position = ((float3)(ref val11.b)).xz
						};
						meshNormal = normals[val10.y];
						basePoint.m_Direction = ((float3)(ref meshNormal.m_Normal)).xz;
						basePoint2 = basePoint;
						basePoint = new BasePoint
						{
							m_Position = ((float3)(ref val11.c)).xz
						};
						meshNormal = normals[val10.z];
						basePoint.m_Direction = ((float3)(ref meshNormal.m_Normal)).xz;
						basePoint3 = basePoint;
					}
					else if (val14.y)
					{
						BasePoint basePoint = new BasePoint
						{
							m_Position = ((float3)(ref val11.c)).xz
						};
						meshNormal = normals[val10.z];
						basePoint.m_Direction = ((float3)(ref meshNormal.m_Normal)).xz;
						basePoint2 = basePoint;
						basePoint = new BasePoint
						{
							m_Position = ((float3)(ref val11.a)).xz
						};
						meshNormal = normals[val10.x];
						basePoint.m_Direction = ((float3)(ref meshNormal.m_Normal)).xz;
						basePoint3 = basePoint;
					}
					else
					{
						BasePoint basePoint = new BasePoint
						{
							m_Position = ((float3)(ref val11.a)).xz
						};
						meshNormal = normals[val10.x];
						basePoint.m_Direction = ((float3)(ref meshNormal.m_Normal)).xz;
						basePoint2 = basePoint;
						basePoint = new BasePoint
						{
							m_Position = ((float3)(ref val11.b)).xz
						};
						meshNormal = normals[val10.y];
						basePoint.m_Direction = ((float3)(ref meshNormal.m_Normal)).xz;
						basePoint3 = basePoint;
					}
					if (val.Contains(new float4(basePoint2.m_Position, basePoint3.m_Position)) || val.Contains(new float4(basePoint3.m_Position, basePoint2.m_Position)))
					{
						continue;
					}
					basePoint2.m_Distance = -1f;
					basePoint3.m_Distance = -1f;
					basePoint2.m_PrevPos = float2.op_Implicit(float.NaN);
					basePoint3.m_PrevPos = basePoint2.m_Position;
					if (val2.TryGetValue(basePoint2.m_Position, ref num4))
					{
						num4 = math.select(num4, -1 - num4, num4 < 0);
						BasePoint basePoint4 = basePoints[num4];
						if (!((float2)(ref basePoint4.m_Direction)).Equals(basePoint2.m_Direction))
						{
							num4 = basePoints.Length;
							basePoints.Add(ref basePoint2);
						}
					}
					else
					{
						num4 = basePoints.Length;
						val2.Add(basePoint2.m_Position, num4);
						basePoints.Add(ref basePoint2);
					}
					if (val2.TryGetValue(basePoint3.m_Position, ref num5))
					{
						num5 = math.select(num5, -1 - num5, num5 < 0);
						ref BasePoint reference = ref basePoints.ElementAt(num5);
						if (!((float2)(ref reference.m_Direction)).Equals(basePoint3.m_Direction))
						{
							num5 = basePoints.Length;
							basePoints.Add(ref basePoint3);
						}
						else
						{
							reference.m_PrevPos = basePoint2.m_Position;
						}
						val2[basePoint3.m_Position] = -1 - num5;
					}
					else
					{
						num5 = basePoints.Length;
						val2.Add(basePoint3.m_Position, -1 - num5);
						basePoints.Add(ref basePoint3);
					}
					BaseLine baseLine = new BaseLine
					{
						m_StartIndex = num4,
						m_EndIndex = num5
					};
					baseLines.Add(ref baseLine);
				}
				ptr[num] = meshNode2.m_SubNodes1.x;
				num = math.select(num, num + 1, meshNode2.m_SubNodes1.x != -1);
				ptr[num] = meshNode2.m_SubNodes1.y;
				num = math.select(num, num + 1, meshNode2.m_SubNodes1.y != -1);
				ptr[num] = meshNode2.m_SubNodes1.z;
				num = math.select(num, num + 1, meshNode2.m_SubNodes1.z != -1);
				ptr[num] = meshNode2.m_SubNodes1.w;
				num = math.select(num, num + 1, meshNode2.m_SubNodes1.w != -1);
				ptr[num] = meshNode2.m_SubNodes2.x;
				num = math.select(num, num + 1, meshNode2.m_SubNodes2.x != -1);
				ptr[num] = meshNode2.m_SubNodes2.y;
				num = math.select(num, num + 1, meshNode2.m_SubNodes2.y != -1);
				ptr[num] = meshNode2.m_SubNodes2.z;
				num = math.select(num, num + 1, meshNode2.m_SubNodes2.z != -1);
				ptr[num] = meshNode2.m_SubNodes2.w;
				num = math.select(num, num + 1, meshNode2.m_SubNodes2.w != -1);
			}
			for (int k = 0; k < baseLines.Length; k++)
			{
				BaseLine baseLine2 = baseLines[k];
				ref BasePoint reference2 = ref basePoints.ElementAt(baseLine2.m_StartIndex);
				ref BasePoint reference3 = ref basePoints.ElementAt(baseLine2.m_EndIndex);
				if (reference2.m_Distance < 0f)
				{
					int num6 = val2[reference2.m_Position];
					if (num6 >= 0)
					{
						reference2.m_Distance = 0f;
					}
					else
					{
						num6 = -1 - num6;
						ref BasePoint reference4 = ref basePoints.ElementAt(num6);
						if (reference4.m_Distance < 0f)
						{
							int num7 = num6;
							if (!math.isnan(reference4.m_PrevPos.x))
							{
								for (int l = 0; l <= basePoints.Length; l++)
								{
									int num8 = val2[reference4.m_PrevPos];
									if (num8 >= 0)
									{
										num6 = num8;
										reference4 = ref basePoints.ElementAt(num6);
										break;
									}
									val3.Add(ref num6);
									num6 = -1 - num8;
									reference4 = ref basePoints.ElementAt(num6);
									if (reference4.m_Distance >= 0f || num6 == num7 || math.isnan(reference4.m_PrevPos.x))
									{
										break;
									}
								}
							}
							if (reference4.m_Distance < 0f)
							{
								reference4.m_Distance = 0f;
							}
							for (int num9 = val3.Length - 1; num9 >= 0; num9--)
							{
								num6 = val3[num9];
								ref BasePoint reference5 = ref basePoints.ElementAt(num6);
								if (num9 != 0 || num6 != num7)
								{
									reference5.m_Distance = reference4.m_Distance + math.distance(reference4.m_Position, reference5.m_Position);
								}
								reference4 = ref reference5;
							}
							val3.Clear();
						}
						reference2.m_Distance = reference4.m_Distance;
					}
				}
				float num10 = reference2.m_Distance + math.distance(reference2.m_Position, reference3.m_Position);
				if (reference3.m_Distance >= 0f && num10 != reference3.m_Distance)
				{
					BasePoint basePoint5 = reference3;
					basePoint5.m_Distance = num10;
					baseLine2.m_EndIndex = basePoints.Length;
					basePoints.Add(ref basePoint5);
					baseLines[k] = baseLine2;
				}
				else
				{
					reference3.m_Distance = num10;
				}
			}
			val.Dispose();
			val2.Dispose();
			val3.Dispose();
		}

		private void GenerateCompositionMesh(NetCompositionMeshData compositionMeshData, DynamicBuffer<NetCompositionPiece> pieces, DynamicBuffer<MeshMaterial> materials, MeshData meshData)
		{
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0700: Unknown result type (might be due to invalid IL or missing references)
			//IL_0727: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_0758: Unknown result type (might be due to invalid IL or missing references)
			//IL_075d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_0766: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_0785: Unknown result type (might be due to invalid IL or missing references)
			//IL_078a: Unknown result type (might be due to invalid IL or missing references)
			//IL_078f: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0817: Unknown result type (might be due to invalid IL or missing references)
			//IL_0837: Unknown result type (might be due to invalid IL or missing references)
			//IL_084c: Unknown result type (might be due to invalid IL or missing references)
			//IL_086b: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0902: Unknown result type (might be due to invalid IL or missing references)
			//IL_0907: Unknown result type (might be due to invalid IL or missing references)
			//IL_090c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0916: Unknown result type (might be due to invalid IL or missing references)
			//IL_091b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0920: Unknown result type (might be due to invalid IL or missing references)
			//IL_092a: Unknown result type (might be due to invalid IL or missing references)
			//IL_092f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0934: Unknown result type (might be due to invalid IL or missing references)
			//IL_093e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0943: Unknown result type (might be due to invalid IL or missing references)
			//IL_0948: Unknown result type (might be due to invalid IL or missing references)
			//IL_0952: Unknown result type (might be due to invalid IL or missing references)
			//IL_0957: Unknown result type (might be due to invalid IL or missing references)
			//IL_095c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0984: Unknown result type (might be due to invalid IL or missing references)
			//IL_0986: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dcc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ddc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0efd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ecf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eed: Unknown result type (might be due to invalid IL or missing references)
			//IL_1428: Unknown result type (might be due to invalid IL or missing references)
			//IL_142f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1434: Unknown result type (might be due to invalid IL or missing references)
			//IL_1445: Unknown result type (might be due to invalid IL or missing references)
			//IL_145c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1461: Unknown result type (might be due to invalid IL or missing references)
			//IL_1463: Unknown result type (might be due to invalid IL or missing references)
			//IL_1468: Unknown result type (might be due to invalid IL or missing references)
			//IL_146a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1476: Unknown result type (might be due to invalid IL or missing references)
			//IL_1491: Unknown result type (might be due to invalid IL or missing references)
			//IL_149f: Unknown result type (might be due to invalid IL or missing references)
			//IL_14a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_14b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_14b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_14c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_14c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_14e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_14e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_14f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1503: Unknown result type (might be due to invalid IL or missing references)
			//IL_1511: Unknown result type (might be due to invalid IL or missing references)
			//IL_1513: Unknown result type (might be due to invalid IL or missing references)
			//IL_1518: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0faa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fcb: Unknown result type (might be due to invalid IL or missing references)
			//IL_1410: Unknown result type (might be due to invalid IL or missing references)
			//IL_1415: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_13e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_13e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_13e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_13fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_134b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1357: Unknown result type (might be due to invalid IL or missing references)
			//IL_135c: Unknown result type (might be due to invalid IL or missing references)
			//IL_136d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1384: Unknown result type (might be due to invalid IL or missing references)
			//IL_1389: Unknown result type (might be due to invalid IL or missing references)
			//IL_138b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1390: Unknown result type (might be due to invalid IL or missing references)
			//IL_1392: Unknown result type (might be due to invalid IL or missing references)
			//IL_139e: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_12fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_1312: Unknown result type (might be due to invalid IL or missing references)
			//IL_1317: Unknown result type (might be due to invalid IL or missing references)
			//IL_1319: Unknown result type (might be due to invalid IL or missing references)
			//IL_131e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1320: Unknown result type (might be due to invalid IL or missing references)
			//IL_132c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1277: Unknown result type (might be due to invalid IL or missing references)
			//IL_1283: Unknown result type (might be due to invalid IL or missing references)
			//IL_1288: Unknown result type (might be due to invalid IL or missing references)
			//IL_1299: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_12bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_12be: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_12e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1079: Unknown result type (might be due to invalid IL or missing references)
			//IL_1080: Unknown result type (might be due to invalid IL or missing references)
			//IL_1088: Unknown result type (might be due to invalid IL or missing references)
			//IL_1097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1000: Unknown result type (might be due to invalid IL or missing references)
			//IL_1005: Unknown result type (might be due to invalid IL or missing references)
			//IL_1016: Unknown result type (might be due to invalid IL or missing references)
			//IL_102e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1035: Unknown result type (might be due to invalid IL or missing references)
			//IL_103d: Unknown result type (might be due to invalid IL or missing references)
			//IL_104c: Unknown result type (might be due to invalid IL or missing references)
			//IL_121d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1234: Unknown result type (might be due to invalid IL or missing references)
			//IL_1239: Unknown result type (might be due to invalid IL or missing references)
			//IL_123b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1240: Unknown result type (might be due to invalid IL or missing references)
			//IL_1242: Unknown result type (might be due to invalid IL or missing references)
			//IL_124e: Unknown result type (might be due to invalid IL or missing references)
			//IL_119c: Unknown result type (might be due to invalid IL or missing references)
			//IL_11a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_11be: Unknown result type (might be due to invalid IL or missing references)
			//IL_11d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_11da: Unknown result type (might be due to invalid IL or missing references)
			//IL_11dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_120a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1149: Unknown result type (might be due to invalid IL or missing references)
			//IL_1160: Unknown result type (might be due to invalid IL or missing references)
			//IL_1165: Unknown result type (might be due to invalid IL or missing references)
			//IL_1167: Unknown result type (might be due to invalid IL or missing references)
			//IL_116c: Unknown result type (might be due to invalid IL or missing references)
			//IL_116e: Unknown result type (might be due to invalid IL or missing references)
			//IL_117a: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_10e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_10fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1103: Unknown result type (might be due to invalid IL or missing references)
			//IL_1105: Unknown result type (might be due to invalid IL or missing references)
			//IL_110a: Unknown result type (might be due to invalid IL or missing references)
			//IL_110c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1118: Unknown result type (might be due to invalid IL or missing references)
			//IL_1133: Unknown result type (might be due to invalid IL or missing references)
			bool flag = (compositionMeshData.m_Flags.m_General & CompositionFlags.General.Node) != 0;
			bool flag2 = (compositionMeshData.m_Flags.m_General & CompositionFlags.General.Roundabout) != 0;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			float middleOffset = compositionMeshData.m_MiddleOffset;
			for (int i = 0; i < materials.Length; i++)
			{
				MeshMaterial meshMaterial = materials[i];
				meshMaterial.m_StartVertex = 0;
				meshMaterial.m_StartIndex = 0;
				meshMaterial.m_VertexCount = 0;
				meshMaterial.m_IndexCount = 0;
				materials[i] = meshMaterial;
			}
			float3 val6 = default(float3);
			float3 val9 = default(float3);
			for (int j = 0; j < pieces.Length; j++)
			{
				NetCompositionPiece netCompositionPiece = pieces[j];
				bool flag3 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.Invert) != 0;
				bool flag4 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.FlipMesh) != 0;
				bool flag5 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.HalfLength) != 0;
				bool flag6 = (netCompositionPiece.m_PieceFlags & NetPieceFlags.PreserveShape) != 0;
				bool flag7 = (flag && !flag2 && !flag6) || flag5;
				bool flag8 = (netCompositionPiece.m_PieceFlags & NetPieceFlags.SkipBottomHalf) != 0;
				if (!m_MeshVertices.HasBuffer(netCompositionPiece.m_Piece))
				{
					continue;
				}
				DynamicBuffer<MeshVertex> val = m_MeshVertices[netCompositionPiece.m_Piece];
				DynamicBuffer<MeshIndex> val2 = m_MeshIndices[netCompositionPiece.m_Piece];
				DynamicBuffer<MeshMaterial> val3 = m_MeshMaterials[netCompositionPiece.m_Piece];
				for (int k = 0; k < val3.Length; k++)
				{
					MeshMaterial pieceMaterial = val3[k];
					int material = GetMaterial(materials, pieceMaterial);
					MeshMaterial meshMaterial2 = materials[material];
					meshMaterial2.m_VertexCount += pieceMaterial.m_VertexCount;
					meshMaterial2.m_IndexCount += pieceMaterial.m_IndexCount;
					num2 = math.max(num2, pieceMaterial.m_VertexCount);
					if (flag7)
					{
						for (int l = 0; l < pieceMaterial.m_VertexCount; l++)
						{
							meshMaterial2.m_VertexCount -= math.select(1, 0, val[pieceMaterial.m_StartVertex + l].m_Vertex.z >= -0.01f);
						}
						int3 val4 = pieceMaterial.m_StartIndex + math.select(new int3(0, 1, 2), new int3(2, 1, 0), flag3 != flag4);
						for (int m = 0; m < pieceMaterial.m_IndexCount; m += 3)
						{
							int3 val5 = m + val4;
							val6.x = val[val2[val5.x].m_Index].m_Vertex.z;
							val6.y = val[val2[val5.y].m_Index].m_Vertex.z;
							val6.z = val[val2[val5.z].m_Index].m_Vertex.z;
							meshMaterial2.m_IndexCount -= math.select(3, 0, math.all(val6 >= -0.01f));
						}
					}
					else if (flag8)
					{
						for (int n = 0; n < pieceMaterial.m_VertexCount; n++)
						{
							meshMaterial2.m_VertexCount -= math.select(1, 0, val[pieceMaterial.m_StartVertex + n].m_Vertex.z <= 0.01f);
						}
						int3 val7 = pieceMaterial.m_StartIndex + math.select(new int3(0, 1, 2), new int3(2, 1, 0), flag3 != flag4);
						for (int num4 = 0; num4 < pieceMaterial.m_IndexCount; num4 += 3)
						{
							int3 val8 = num4 + val7;
							val9.x = val[val2[val8.x].m_Index].m_Vertex.z;
							val9.y = val[val2[val8.y].m_Index].m_Vertex.z;
							val9.z = val[val2[val8.z].m_Index].m_Vertex.z;
							meshMaterial2.m_IndexCount -= math.select(3, 0, math.all(val9 >= -0.01f));
						}
					}
					materials[material] = meshMaterial2;
				}
			}
			for (int num5 = 0; num5 < materials.Length; num5++)
			{
				MeshMaterial meshMaterial3 = materials[num5];
				meshMaterial3.m_StartVertex = num;
				meshMaterial3.m_StartIndex = num3;
				num += meshMaterial3.m_VertexCount;
				num3 += meshMaterial3.m_IndexCount;
				materials[num5] = meshMaterial3;
			}
			NativeArray<VertexAttributeDescriptor> val10 = default(NativeArray<VertexAttributeDescriptor>);
			val10._002Ector(5, (Allocator)2, (NativeArrayOptions)0);
			SetupGeneratedMeshAttributes(val10);
			bool flag9 = num3 >= 65536;
			((MeshData)(ref meshData)).SetVertexBufferParams(num, val10);
			((MeshData)(ref meshData)).SetIndexBufferParams(num3, (IndexFormat)(flag9 ? 1 : 0));
			val10.Dispose();
			((MeshData)(ref meshData)).subMeshCount = materials.Length;
			for (int num6 = 0; num6 < materials.Length; num6++)
			{
				MeshMaterial meshMaterial4 = materials[num6];
				int num7 = num6;
				SubMeshDescriptor val11 = default(SubMeshDescriptor);
				((SubMeshDescriptor)(ref val11)).firstVertex = meshMaterial4.m_StartVertex;
				((SubMeshDescriptor)(ref val11)).indexStart = meshMaterial4.m_StartIndex;
				((SubMeshDescriptor)(ref val11)).vertexCount = meshMaterial4.m_VertexCount;
				((SubMeshDescriptor)(ref val11)).indexCount = meshMaterial4.m_IndexCount;
				((SubMeshDescriptor)(ref val11)).topology = (MeshTopology)0;
				((MeshData)(ref meshData)).SetSubMesh(num7, val11, (MeshUpdateFlags)13);
				meshMaterial4.m_VertexCount = 0;
				meshMaterial4.m_IndexCount = 0;
				materials[num6] = meshMaterial4;
			}
			NativeArray<VertexData> vertexData = ((MeshData)(ref meshData)).GetVertexData<VertexData>(0);
			NativeArray<uint> val12 = default(NativeArray<uint>);
			NativeArray<ushort> val13 = default(NativeArray<ushort>);
			if (flag9)
			{
				val12 = ((MeshData)(ref meshData)).GetIndexData<uint>();
			}
			else
			{
				val13 = ((MeshData)(ref meshData)).GetIndexData<ushort>();
			}
			NativeArray<int> val14 = default(NativeArray<int>);
			val14._002Ector(num2, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<Bounds1> heightBounds = default(NativeArray<Bounds1>);
			float num8 = compositionMeshData.m_Width * 0.5f + middleOffset;
			float num9 = compositionMeshData.m_Width * 0.5f - middleOffset;
			float2 val19 = default(float2);
			float2 val20 = default(float2);
			float2 val21 = default(float2);
			float3 val30 = default(float3);
			float3 val31 = default(float3);
			float4 val34 = default(float4);
			Color32 color = default(Color32);
			for (int num10 = 0; num10 < pieces.Length; num10++)
			{
				NetCompositionPiece compositionPiece = pieces[num10];
				if (!m_MeshVertices.HasBuffer(compositionPiece.m_Piece))
				{
					continue;
				}
				bool flag10 = (compositionPiece.m_SectionFlags & NetSectionFlags.Invert) != 0;
				bool flag11 = (compositionPiece.m_SectionFlags & NetSectionFlags.FlipMesh) != 0;
				bool flag12 = (compositionPiece.m_SectionFlags & NetSectionFlags.Median) != 0;
				bool flag13 = (compositionPiece.m_SectionFlags & NetSectionFlags.Right) != 0;
				bool flag14 = (compositionPiece.m_SectionFlags & NetSectionFlags.HalfLength) != 0;
				bool flag15 = (compositionPiece.m_PieceFlags & NetPieceFlags.PreserveShape) != 0;
				bool flag16 = (compositionPiece.m_PieceFlags & NetPieceFlags.DisableTiling) != 0;
				bool flag17 = (compositionPiece.m_PieceFlags & NetPieceFlags.LowerBottomToTerrain) != 0;
				bool flag18 = (compositionPiece.m_PieceFlags & NetPieceFlags.RaiseTopToTerrain) != 0;
				bool flag19 = (compositionPiece.m_PieceFlags & NetPieceFlags.SmoothTopNormal) != 0;
				bool flag20 = (flag && !flag2 && !flag15) || flag14;
				bool flag21 = (compositionPiece.m_PieceFlags & NetPieceFlags.SkipBottomHalf) != 0;
				bool flag22 = compositionPiece.m_Size.x == 0f;
				float4 val15 = math.select(new float4(-1f, 1f, -1f, 1f), new float4(1f, 1f, 1f, -1f), new bool4(flag10, false, flag11, flag10 != flag11));
				float3 offset = compositionPiece.m_Offset;
				float2 val16 = 1f / new float2(compositionMeshData.m_Width, compositionPiece.m_Size.z * 0.5f);
				float2 val17 = 1f / new float2(num8, compositionPiece.m_Size.z * 0.5f);
				float2 val18 = 1f / new float2(num9, compositionPiece.m_Size.z * 0.5f);
				((float2)(ref val19))._002Ector(0.5f, 1f);
				((float2)(ref val20))._002Ector(1f - middleOffset / num8, 1f);
				((float2)(ref val21))._002Ector((0f - middleOffset) / num9, 1f);
				float z = compositionPiece.m_Offset.x / compositionMeshData.m_Width + 0.5f;
				float z2 = 1f + (compositionPiece.m_Offset.x - middleOffset) / num8;
				float z3 = (compositionPiece.m_Offset.x - middleOffset) / num9;
				if (flag && flag15)
				{
					float num11 = 0.5f * compositionPiece.m_Size.z / compositionMeshData.m_Width;
					val16.y *= num11;
					val19.y *= num11;
					z = 0.5f;
				}
				else if (flag2)
				{
					z2 = 0f;
					z3 = 1f;
				}
				else if (flag14)
				{
					val15.z *= 2f;
					offset.z += 0.5f * compositionPiece.m_Size.z;
				}
				DynamicBuffer<MeshVertex> pieceVertices = m_MeshVertices[compositionPiece.m_Piece];
				DynamicBuffer<MeshNormal> val22 = m_MeshNormals[compositionPiece.m_Piece];
				DynamicBuffer<MeshTangent> val23 = m_MeshTangents[compositionPiece.m_Piece];
				DynamicBuffer<MeshUV0> val24 = m_MeshUV0s[compositionPiece.m_Piece];
				DynamicBuffer<MeshIndex> pieceIndices = m_MeshIndices[compositionPiece.m_Piece];
				DynamicBuffer<MeshMaterial> val25 = m_MeshMaterials[compositionPiece.m_Piece];
				if (flag17 || flag18 || flag19)
				{
					if (!heightBounds.IsCreated)
					{
						heightBounds._002Ector(257, (Allocator)2, (NativeArrayOptions)0);
					}
					InitializeHeightBounds(heightBounds, compositionPiece, pieceVertices, pieceIndices);
				}
				for (int num12 = 0; num12 < val25.Length; num12++)
				{
					MeshMaterial pieceMaterial2 = val25[num12];
					int material2 = GetMaterial(materials, pieceMaterial2);
					MeshMaterial meshMaterial5 = materials[material2];
					for (int num13 = 0; num13 < pieceMaterial2.m_VertexCount; num13++)
					{
						float z4 = pieceVertices[pieceMaterial2.m_StartVertex + num13].m_Vertex.z;
						if ((!flag20 || z4 >= -0.01f) && (!flag21 || z4 <= 0.01f))
						{
							val14[num13] = meshMaterial5.m_StartVertex + meshMaterial5.m_VertexCount++;
						}
						else
						{
							val14[num13] = -1;
						}
					}
					int3 val26 = pieceMaterial2.m_StartIndex + math.select(new int3(0, 1, 2), new int3(2, 1, 0), flag10 != flag11);
					float2 val27 = float2.op_Implicit(0f);
					float num14 = 0f;
					bool flag23 = !flag16;
					for (int num15 = 0; num15 < pieceMaterial2.m_IndexCount; num15 += 3)
					{
						int3 val28 = num15 + val26;
						val28.x = pieceIndices[val28.x].m_Index;
						val28.y = pieceIndices[val28.y].m_Index;
						val28.z = pieceIndices[val28.z].m_Index;
						int3 val29 = val28 - pieceMaterial2.m_StartVertex;
						val29.x = val14[val29.x];
						val29.y = val14[val29.y];
						val29.z = val14[val29.z];
						if (math.all(val29 >= 0))
						{
							if (flag9)
							{
								val12[meshMaterial5.m_StartIndex + meshMaterial5.m_IndexCount++] = (uint)val29.x;
								val12[meshMaterial5.m_StartIndex + meshMaterial5.m_IndexCount++] = (uint)val29.y;
								val12[meshMaterial5.m_StartIndex + meshMaterial5.m_IndexCount++] = (uint)val29.z;
							}
							else
							{
								val13[meshMaterial5.m_StartIndex + meshMaterial5.m_IndexCount++] = (ushort)val29.x;
								val13[meshMaterial5.m_StartIndex + meshMaterial5.m_IndexCount++] = (ushort)val29.y;
								val13[meshMaterial5.m_StartIndex + meshMaterial5.m_IndexCount++] = (ushort)val29.z;
							}
							((float3)(ref val30))._002Ector(val24[val28.x].m_Uv.y, val24[val28.y].m_Uv.y, val24[val28.z].m_Uv.y);
							if (flag23 & math.all(val30 >= 0f))
							{
								((float3)(ref val31))._002Ector(pieceVertices[val28.x].m_Vertex.z, pieceVertices[val28.y].m_Vertex.z, pieceVertices[val28.z].m_Vertex.z);
								val27 += new float2(math.csum(math.abs(((float3)(ref val30)).yzx - val30)), math.csum(math.abs(((float3)(ref val31)).yzx - val31)));
							}
							else
							{
								num14 = math.max(math.max(num14, val30.x), math.max(val30.y, val30.z));
							}
						}
					}
					float num16 = val27.x / val27.y;
					num14 = math.select(math.ceil(num14), 0f, flag23);
					for (int num17 = 0; num17 < pieceMaterial2.m_VertexCount; num17++)
					{
						int num18 = val14[num17];
						if (num18 == -1)
						{
							continue;
						}
						int num19 = pieceMaterial2.m_StartVertex + num17;
						float3 val32 = pieceVertices[num19].m_Vertex;
						float3 val33 = val22[num19].m_Normal;
						float4 tangent = val23[num19].m_Tangent;
						((float4)(ref val34))._002Ector(val24[num19].m_Uv, 0f, 0f);
						int4 val35 = default(int4);
						if (flag17)
						{
							Bounds1 heightBounds2 = GetHeightBounds(heightBounds, compositionPiece, val32.z);
							val35.z = math.select(val35.z, 1, val32.y <= heightBounds2.min + 0.01f);
						}
						if (flag18)
						{
							Bounds1 heightBounds3 = GetHeightBounds(heightBounds, compositionPiece, val32.z);
							val35.z = math.select(val35.z, 2, val32.y >= heightBounds3.max - 0.01f);
						}
						if (flag19)
						{
							Bounds1 heightBounds4 = GetHeightBounds(heightBounds, compositionPiece, val32.z);
							val33 = math.select(val33, new float3(0f, 1f, 0f), val32.y >= heightBounds4.max - 0.01f);
						}
						val32 = val32 * ((float4)(ref val15)).xyz + offset;
						val33 *= ((float4)(ref val15)).xyz;
						tangent *= val15;
						val34.y = math.select(val34.y - num14, num16, flag23 & (val34.y >= 0f));
						if (flag)
						{
							if (flag15)
							{
								((int4)(ref val35)).xy = new int2(6, 7);
								val34.z = z;
								((float3)(ref val32)).xz = ((float3)(ref val32)).xz * val16 + val19;
								val34.w = val32.z * 20f;
							}
							else if (flag12)
							{
								float num20 = val32.x - compositionPiece.m_Offset.x;
								val32.x = math.select(val32.x, compositionPiece.m_Offset.x, flag22);
								if (math.abs(num20) < 0.01f)
								{
									if (flag2)
									{
										((int4)(ref val35)).xy = math.select(new int2(4, 2), new int2(5, 3), val32.z >= 0f);
										val32.x = 0f;
										val32.z = val32.z * val16.y + val19.y;
										val34.w = val32.z * 40f;
									}
									else
									{
										val35.x = 4;
										val32.x = 0f;
										val32.z = val32.z * val16.y + val19.y;
										val34.w = val32.z * 20f;
									}
								}
								else if (num20 > 0f)
								{
									if (flag2)
									{
										((int4)(ref val35)).xyw = math.select(new int3(4, 2, 4), new int3(5, 3, 132), val32.z >= 0f);
										val34.z = z3;
										((float3)(ref val32)).xz = ((float3)(ref val32)).xz * val18 + val21;
										val34.w = val32.z * 40f;
										val32.z -= val35.y - 2;
									}
									else
									{
										((int4)(ref val35)).xyw = new int3(1, 3, 4);
										val34.z = z3;
										((float3)(ref val32)).xz = ((float3)(ref val32)).xz * val18 + val21;
										val34.w = val32.z * 20f;
									}
								}
								else if (flag2)
								{
									((int4)(ref val35)).xyw = math.select(new int3(0, 4, 2), new int3(1, 5, 130), val32.z >= 0f);
									val34.z = z2;
									((float3)(ref val32)).xz = ((float3)(ref val32)).xz * val17 + val20;
									val34.w = val32.z * 40f;
									val32.z -= val35.x;
								}
								else
								{
									((int4)(ref val35)).yw = new int2(2, 2);
									val34.z = z2;
									((float3)(ref val32)).xz = ((float3)(ref val32)).xz * val17 + val20;
									val34.w = val32.z * 20f;
								}
							}
							else if (flag13)
							{
								if (flag2)
								{
									((int4)(ref val35)).xyw = math.select(new int3(4, 2, 4), new int3(5, 3, 132), val32.z >= 0f);
									val34.z = z3;
									((float3)(ref val32)).xz = ((float3)(ref val32)).xz * val18 + val21;
									val34.w = val32.z * 40f;
									val32.z -= val35.y - 2;
								}
								else
								{
									((int4)(ref val35)).xyw = new int3(1, 3, 4);
									val34.z = z3;
									((float3)(ref val32)).xz = ((float3)(ref val32)).xz * val18 + val21;
									val34.w = val32.z * 20f;
								}
							}
							else if (flag2)
							{
								((int4)(ref val35)).xyw = math.select(new int3(0, 4, 2), new int3(1, 5, 130), val32.z >= 0f);
								val34.z = z2;
								((float3)(ref val32)).xz = ((float3)(ref val32)).xz * val17 + val20;
								val34.w = val32.z * 40f;
								val32.z -= val35.x;
							}
							else
							{
								((int4)(ref val35)).yw = new int2(2, 2);
								val34.z = z2;
								((float3)(ref val32)).xz = ((float3)(ref val32)).xz * val17 + val20;
								val34.w = val32.z * 20f;
							}
							((float3)(ref val32)).xz = math.saturate(((float3)(ref val32)).xz);
						}
						else
						{
							((int4)(ref val35)).xy = math.select(new int2(0, 2), new int2(1, 3), val32.z >= 0f);
							val34.z = z;
							((float3)(ref val32)).xz = ((float3)(ref val32)).xz * val16 + val19;
							val34.w = val32.z * 0.5f;
							val32.z -= val35.x;
							((float3)(ref val32)).xz = math.saturate(((float3)(ref val32)).xz);
						}
						((Color32)(ref color))._002Ector((byte)val35.x, (byte)val35.y, (byte)val35.z, (byte)val35.w);
						vertexData[num18] = new VertexData
						{
							m_Position = val32,
							m_Color = color,
							m_Normal = MathUtils.NormalToOctahedral(val33),
							m_Tangent = MathUtils.TangentToOctahedral(tangent),
							m_UV0 = new half4(val34)
						};
					}
					materials[material2] = meshMaterial5;
				}
			}
			if (val14.IsCreated)
			{
				val14.Dispose();
			}
			if (heightBounds.IsCreated)
			{
				heightBounds.Dispose();
			}
		}

		private static void SetupGeneratedMeshAttributes(NativeArray<VertexAttributeDescriptor> attrs)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			attrs[0] = new VertexAttributeDescriptor((VertexAttribute)0, (VertexAttributeFormat)0, 3, 0);
			attrs[1] = new VertexAttributeDescriptor((VertexAttribute)1, (VertexAttributeFormat)5, 2, 0);
			attrs[2] = new VertexAttributeDescriptor((VertexAttribute)2, (VertexAttributeFormat)0, 1, 0);
			attrs[3] = new VertexAttributeDescriptor((VertexAttribute)3, (VertexAttributeFormat)2, 4, 0);
			attrs[4] = new VertexAttributeDescriptor((VertexAttribute)4, (VertexAttributeFormat)1, 4, 0);
		}

		private void InitializeHeightBounds(NativeArray<Bounds1> heightBounds, NetCompositionPiece compositionPiece, DynamicBuffer<MeshVertex> pieceVertices, DynamicBuffer<MeshIndex> pieceIndices)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			int num = heightBounds.Length - 1;
			float num2 = (float)num / compositionPiece.m_Size.z;
			for (int i = 0; i <= num; i++)
			{
				heightBounds[i] = new Bounds1(float.MaxValue, float.MinValue);
			}
			for (int j = 0; j < pieceIndices.Length; j += 3)
			{
				float3 vertex = pieceVertices[pieceIndices[j].m_Index].m_Vertex;
				float3 vertex2 = pieceVertices[pieceIndices[j + 1].m_Index].m_Vertex;
				float3 vertex3 = pieceVertices[pieceIndices[j + 2].m_Index].m_Vertex;
				int num3 = math.clamp(Mathf.RoundToInt(vertex.z * num2) + (num >> 1), 0, num);
				int num4 = math.clamp(Mathf.RoundToInt(vertex2.z * num2) + (num >> 1), 0, num);
				int num5 = math.clamp(Mathf.RoundToInt(vertex3.z * num2) + (num >> 1), 0, num);
				AddHeightBounds(heightBounds, vertex, vertex2, num3, num4);
				AddHeightBounds(heightBounds, vertex2, vertex3, num4, num5);
				AddHeightBounds(heightBounds, vertex3, vertex, num5, num3);
			}
		}

		private void AddHeightBounds(NativeArray<Bounds1> heightBounds, float3 aVertex, float3 bVertex, int aIndex, int bIndex)
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if (aIndex <= bIndex)
			{
				float num = 1f / (float)(bIndex - aIndex + 1);
				for (int i = aIndex; i <= bIndex; i++)
				{
					float num2 = math.lerp(aVertex.y, bVertex.y, (float)(i - aIndex) * num);
					heightBounds[i] |= num2;
				}
			}
			else
			{
				float num3 = 1f / (float)(aIndex - bIndex + 1);
				for (int j = bIndex; j <= aIndex; j++)
				{
					float num4 = math.lerp(bVertex.y, aVertex.y, (float)(j - bIndex) * num3);
					heightBounds[j] |= num4;
				}
			}
		}

		private Bounds1 GetHeightBounds(NativeArray<Bounds1> heightBounds, NetCompositionPiece compositionPiece, float z)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			int num = heightBounds.Length - 1;
			float num2 = (float)num / compositionPiece.m_Size.z;
			int num3 = math.clamp(Mathf.RoundToInt(z * num2) + (num >> 1), 0, num);
			return heightBounds[num3];
		}
	}

	public static JobHandle GenerateMeshes(BatchMeshSystem meshSystem, NativeList<Entity> meshes, MeshDataArray meshDataArray, JobHandle dependencies)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		return IJobParallelForExtensions.Schedule<GenerateBatchMeshJob>(new GenerateBatchMeshJob
		{
			m_Entities = meshes,
			m_MeshData = ((SystemBase)meshSystem).GetComponentLookup<MeshData>(true),
			m_CompositionMeshData = ((SystemBase)meshSystem).GetComponentLookup<NetCompositionMeshData>(true),
			m_CompositionPieces = ((SystemBase)meshSystem).GetBufferLookup<NetCompositionPiece>(true),
			m_MeshVertices = ((SystemBase)meshSystem).GetBufferLookup<MeshVertex>(true),
			m_MeshNormals = ((SystemBase)meshSystem).GetBufferLookup<MeshNormal>(true),
			m_MeshTangents = ((SystemBase)meshSystem).GetBufferLookup<MeshTangent>(true),
			m_MeshUV0s = ((SystemBase)meshSystem).GetBufferLookup<MeshUV0>(true),
			m_MeshIndices = ((SystemBase)meshSystem).GetBufferLookup<MeshIndex>(true),
			m_MeshNodes = ((SystemBase)meshSystem).GetBufferLookup<MeshNode>(true),
			m_MeshMaterials = ((SystemBase)meshSystem).GetBufferLookup<MeshMaterial>(false),
			m_MeshDataArray = meshDataArray
		}, meshes.Length, 1, dependencies);
	}
}

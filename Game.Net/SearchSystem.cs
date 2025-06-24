using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Game.Rendering;
using Game.Serialization;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class SearchSystem : GameSystemBase, IPreDeserialize
{
	[BurstCompile]
	private struct UpdateNetSearchTreeJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> m_EdgeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<StartNodeGeometry> m_StartGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<EndNodeGeometry> m_EndGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<NodeGeometry> m_NodeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<Composition> m_CompositionType;

		[ReadOnly]
		public ComponentTypeHandle<Orphan> m_OrphanType;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Marker> m_MarkerType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<CullingInfo> m_CullingInfoType;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshRef> m_PrefabCompositionMeshRef;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshData> m_PrefabCompositionMeshData;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_Loaded;

		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a10: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aca: Unknown result type (might be due to invalid IL or missing references)
			//IL_065c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0917: Unknown result type (might be due to invalid IL or missing references)
			//IL_091c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0927: Unknown result type (might be due to invalid IL or missing references)
			//IL_092c: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0738: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_094d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_075b: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0972: Unknown result type (might be due to invalid IL or missing references)
			//IL_0986: Unknown result type (might be due to invalid IL or missing references)
			//IL_0821: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0846: Unknown result type (might be due to invalid IL or missing references)
			//IL_07db: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09df: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_086f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_059b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b46: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity val = nativeArray[i];
					m_SearchTree.TryRemove(val);
				}
				return;
			}
			if (m_Loaded || ((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType))
			{
				NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				bool flag = ((ArchetypeChunk)(ref chunk)).Has<CullingInfo>(ref m_CullingInfoType);
				NativeArray<EdgeGeometry> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeGeometry>(ref m_EdgeGeometryType);
				if (nativeArray3.Length != 0)
				{
					NativeArray<StartNodeGeometry> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<StartNodeGeometry>(ref m_StartGeometryType);
					NativeArray<EndNodeGeometry> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EndNodeGeometry>(ref m_EndGeometryType);
					NativeArray<Composition> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
					bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<Marker>(ref m_MarkerType);
					NetCompositionMeshData netCompositionMeshData = default(NetCompositionMeshData);
					NetCompositionMeshData netCompositionMeshData2 = default(NetCompositionMeshData);
					NetCompositionMeshData netCompositionMeshData3 = default(NetCompositionMeshData);
					for (int j = 0; j < nativeArray2.Length; j++)
					{
						Entity val2 = nativeArray2[j];
						EdgeGeometry edgeGeometry = nativeArray3[j];
						EdgeNodeGeometry geometry = nativeArray4[j].m_Geometry;
						EdgeNodeGeometry geometry2 = nativeArray5[j].m_Geometry;
						Bounds3 bounds = edgeGeometry.m_Bounds | geometry.m_Bounds | geometry2.m_Bounds;
						Composition composition = nativeArray6[j];
						NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_Edge];
						NetCompositionData netCompositionData2 = m_PrefabCompositionData[composition.m_StartNode];
						NetCompositionData netCompositionData3 = m_PrefabCompositionData[composition.m_EndNode];
						int lod = math.min(netCompositionData.m_MinLod, math.min(netCompositionData2.m_MinLod, netCompositionData3.m_MinLod));
						BoundsMask boundsMask = BoundsMask.Debug;
						if (!flag2 || m_EditorMode)
						{
							if (math.any(edgeGeometry.m_Start.m_Length + edgeGeometry.m_End.m_Length > 0.1f))
							{
								NetCompositionMeshRef netCompositionMeshRef = m_PrefabCompositionMeshRef[composition.m_Edge];
								if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef.m_Mesh, ref netCompositionMeshData))
								{
									boundsMask |= ((netCompositionMeshData.m_DefaultLayers == (MeshLayer)0) ? BoundsMask.NormalLayers : CommonUtils.GetBoundsMask(netCompositionMeshData.m_DefaultLayers));
								}
							}
							if (math.any(geometry.m_Left.m_Length > 0.05f) | math.any(geometry.m_Right.m_Length > 0.05f))
							{
								NetCompositionMeshRef netCompositionMeshRef2 = m_PrefabCompositionMeshRef[composition.m_StartNode];
								if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef2.m_Mesh, ref netCompositionMeshData2))
								{
									boundsMask |= ((netCompositionMeshData2.m_DefaultLayers == (MeshLayer)0) ? BoundsMask.NormalLayers : CommonUtils.GetBoundsMask(netCompositionMeshData2.m_DefaultLayers));
								}
							}
							if (math.any(geometry2.m_Left.m_Length > 0.05f) | math.any(geometry2.m_Right.m_Length > 0.05f))
							{
								NetCompositionMeshRef netCompositionMeshRef3 = m_PrefabCompositionMeshRef[composition.m_EndNode];
								if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef3.m_Mesh, ref netCompositionMeshData3))
								{
									boundsMask |= ((netCompositionMeshData3.m_DefaultLayers == (MeshLayer)0) ? BoundsMask.NormalLayers : CommonUtils.GetBoundsMask(netCompositionMeshData3.m_DefaultLayers));
								}
							}
						}
						if (!flag)
						{
							boundsMask &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
						}
						m_SearchTree.Add(val2, new QuadTreeBoundsXZ(bounds, boundsMask, lod));
					}
					return;
				}
				NativeArray<NodeGeometry> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NodeGeometry>(ref m_NodeGeometryType);
				if (nativeArray7.Length != 0)
				{
					NativeArray<Orphan> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Orphan>(ref m_OrphanType);
					bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<Marker>(ref m_MarkerType);
					NetCompositionMeshData netCompositionMeshData4 = default(NetCompositionMeshData);
					for (int k = 0; k < nativeArray2.Length; k++)
					{
						Entity val3 = nativeArray2[k];
						Bounds3 bounds2 = nativeArray7[k].m_Bounds;
						BoundsMask boundsMask2 = BoundsMask.Debug;
						int lod2;
						if (nativeArray8.Length != 0)
						{
							Orphan orphan = nativeArray8[k];
							lod2 = m_PrefabCompositionData[orphan.m_Composition].m_MinLod;
							if (!flag3 || m_EditorMode)
							{
								NetCompositionMeshRef netCompositionMeshRef4 = m_PrefabCompositionMeshRef[orphan.m_Composition];
								if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef4.m_Mesh, ref netCompositionMeshData4))
								{
									boundsMask2 |= ((netCompositionMeshData4.m_DefaultLayers == (MeshLayer)0) ? BoundsMask.NormalLayers : CommonUtils.GetBoundsMask(netCompositionMeshData4.m_DefaultLayers));
								}
							}
						}
						else
						{
							lod2 = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(2f)));
						}
						if (!flag)
						{
							boundsMask2 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
						}
						m_SearchTree.Add(val3, new QuadTreeBoundsXZ(bounds2, boundsMask2, lod2));
					}
					return;
				}
				NativeArray<Node> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Node>(ref m_NodeType);
				if (nativeArray9.Length != 0)
				{
					BoundsMask boundsMask3 = ((!m_EditorMode) ? BoundsMask.Debug : (BoundsMask.Debug | BoundsMask.NormalLayers));
					if (!flag)
					{
						boundsMask3 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
					}
					Bounds3 bounds3 = default(Bounds3);
					for (int l = 0; l < nativeArray2.Length; l++)
					{
						Entity val4 = nativeArray2[l];
						Node node = nativeArray9[l];
						((Bounds3)(ref bounds3))._002Ector(node.m_Position - 1f, node.m_Position + 1f);
						int lod3 = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(2f)));
						m_SearchTree.Add(val4, new QuadTreeBoundsXZ(bounds3, boundsMask3, lod3));
					}
					return;
				}
				NativeArray<Curve> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
				if (nativeArray10.Length != 0)
				{
					BoundsMask boundsMask4 = ((!m_EditorMode) ? BoundsMask.Debug : (BoundsMask.Debug | BoundsMask.NormalLayers));
					if (!flag)
					{
						boundsMask4 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
					}
					for (int m = 0; m < nativeArray2.Length; m++)
					{
						Entity val5 = nativeArray2[m];
						Bounds3 bounds4 = MathUtils.Expand(MathUtils.Bounds(nativeArray10[m].m_Bezier), float3.op_Implicit(0.5f));
						int lod4 = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float3(2f)));
						m_SearchTree.Add(val5, new QuadTreeBoundsXZ(bounds4, boundsMask4, lod4));
					}
				}
				return;
			}
			NativeArray<Entity> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			bool flag4 = ((ArchetypeChunk)(ref chunk)).Has<CullingInfo>(ref m_CullingInfoType);
			NativeArray<EdgeGeometry> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeGeometry>(ref m_EdgeGeometryType);
			if (nativeArray12.Length != 0)
			{
				NativeArray<StartNodeGeometry> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<StartNodeGeometry>(ref m_StartGeometryType);
				NativeArray<EndNodeGeometry> nativeArray14 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EndNodeGeometry>(ref m_EndGeometryType);
				NativeArray<Composition> nativeArray15 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
				bool flag5 = ((ArchetypeChunk)(ref chunk)).Has<Marker>(ref m_MarkerType);
				NetCompositionMeshData netCompositionMeshData5 = default(NetCompositionMeshData);
				NetCompositionMeshData netCompositionMeshData6 = default(NetCompositionMeshData);
				NetCompositionMeshData netCompositionMeshData7 = default(NetCompositionMeshData);
				for (int n = 0; n < nativeArray11.Length; n++)
				{
					Entity val6 = nativeArray11[n];
					EdgeGeometry edgeGeometry2 = nativeArray12[n];
					EdgeNodeGeometry geometry3 = nativeArray13[n].m_Geometry;
					EdgeNodeGeometry geometry4 = nativeArray14[n].m_Geometry;
					Bounds3 bounds5 = edgeGeometry2.m_Bounds | geometry3.m_Bounds | geometry4.m_Bounds;
					Composition composition2 = nativeArray15[n];
					NetCompositionData netCompositionData4 = m_PrefabCompositionData[composition2.m_Edge];
					NetCompositionData netCompositionData5 = m_PrefabCompositionData[composition2.m_StartNode];
					NetCompositionData netCompositionData6 = m_PrefabCompositionData[composition2.m_EndNode];
					int lod5 = math.min(netCompositionData4.m_MinLod, math.min(netCompositionData5.m_MinLod, netCompositionData6.m_MinLod));
					BoundsMask boundsMask5 = BoundsMask.Debug;
					if (!flag5 || m_EditorMode)
					{
						if (math.any(edgeGeometry2.m_Start.m_Length + edgeGeometry2.m_End.m_Length > 0.1f))
						{
							NetCompositionMeshRef netCompositionMeshRef5 = m_PrefabCompositionMeshRef[composition2.m_Edge];
							if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef5.m_Mesh, ref netCompositionMeshData5))
							{
								boundsMask5 |= ((netCompositionMeshData5.m_DefaultLayers == (MeshLayer)0) ? BoundsMask.NormalLayers : CommonUtils.GetBoundsMask(netCompositionMeshData5.m_DefaultLayers));
							}
						}
						if (math.any(geometry3.m_Left.m_Length > 0.05f) | math.any(geometry3.m_Right.m_Length > 0.05f))
						{
							NetCompositionMeshRef netCompositionMeshRef6 = m_PrefabCompositionMeshRef[composition2.m_StartNode];
							if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef6.m_Mesh, ref netCompositionMeshData6))
							{
								boundsMask5 |= ((netCompositionMeshData6.m_DefaultLayers == (MeshLayer)0) ? BoundsMask.NormalLayers : CommonUtils.GetBoundsMask(netCompositionMeshData6.m_DefaultLayers));
							}
						}
						if (math.any(geometry4.m_Left.m_Length > 0.05f) | math.any(geometry4.m_Right.m_Length > 0.05f))
						{
							NetCompositionMeshRef netCompositionMeshRef7 = m_PrefabCompositionMeshRef[composition2.m_EndNode];
							if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef7.m_Mesh, ref netCompositionMeshData7))
							{
								boundsMask5 |= ((netCompositionMeshData7.m_DefaultLayers == (MeshLayer)0) ? BoundsMask.NormalLayers : CommonUtils.GetBoundsMask(netCompositionMeshData7.m_DefaultLayers));
							}
						}
					}
					if (!flag4)
					{
						boundsMask5 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
					}
					m_SearchTree.Update(val6, new QuadTreeBoundsXZ(bounds5, boundsMask5, lod5));
				}
				return;
			}
			NativeArray<NodeGeometry> nativeArray16 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NodeGeometry>(ref m_NodeGeometryType);
			if (nativeArray16.Length != 0)
			{
				NativeArray<Orphan> nativeArray17 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Orphan>(ref m_OrphanType);
				bool flag6 = ((ArchetypeChunk)(ref chunk)).Has<Marker>(ref m_MarkerType);
				NetCompositionMeshData netCompositionMeshData8 = default(NetCompositionMeshData);
				for (int num = 0; num < nativeArray11.Length; num++)
				{
					Entity val7 = nativeArray11[num];
					Bounds3 bounds6 = nativeArray16[num].m_Bounds;
					BoundsMask boundsMask6 = BoundsMask.Debug;
					int lod6;
					if (nativeArray17.Length != 0)
					{
						Orphan orphan2 = nativeArray17[num];
						lod6 = m_PrefabCompositionData[orphan2.m_Composition].m_MinLod;
						if (!flag6 || m_EditorMode)
						{
							NetCompositionMeshRef netCompositionMeshRef8 = m_PrefabCompositionMeshRef[orphan2.m_Composition];
							if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef8.m_Mesh, ref netCompositionMeshData8))
							{
								boundsMask6 |= ((netCompositionMeshData8.m_DefaultLayers == (MeshLayer)0) ? BoundsMask.NormalLayers : CommonUtils.GetBoundsMask(netCompositionMeshData8.m_DefaultLayers));
							}
						}
					}
					else
					{
						lod6 = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(2f)));
					}
					if (!flag4)
					{
						boundsMask6 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
					}
					m_SearchTree.Update(val7, new QuadTreeBoundsXZ(bounds6, boundsMask6, lod6));
				}
				return;
			}
			NativeArray<Node> nativeArray18 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Node>(ref m_NodeType);
			if (nativeArray18.Length != 0)
			{
				BoundsMask boundsMask7 = ((!m_EditorMode) ? BoundsMask.Debug : (BoundsMask.Debug | BoundsMask.NormalLayers));
				if (!flag4)
				{
					boundsMask7 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
				}
				Bounds3 bounds7 = default(Bounds3);
				for (int num2 = 0; num2 < nativeArray11.Length; num2++)
				{
					Entity val8 = nativeArray11[num2];
					Node node2 = nativeArray18[num2];
					((Bounds3)(ref bounds7))._002Ector(node2.m_Position - 1f, node2.m_Position + 1f);
					int lod7 = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(2f)));
					m_SearchTree.Update(val8, new QuadTreeBoundsXZ(bounds7, boundsMask7, lod7));
				}
				return;
			}
			NativeArray<Curve> nativeArray19 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			if (nativeArray19.Length != 0)
			{
				BoundsMask boundsMask8 = ((!m_EditorMode) ? BoundsMask.Debug : (BoundsMask.Debug | BoundsMask.NormalLayers));
				if (!flag4)
				{
					boundsMask8 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
				}
				for (int num3 = 0; num3 < nativeArray11.Length; num3++)
				{
					Entity val9 = nativeArray11[num3];
					Bounds3 bounds8 = MathUtils.Expand(MathUtils.Bounds(nativeArray19[num3].m_Bezier), float3.op_Implicit(0.5f));
					int lod8 = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(2f)));
					m_SearchTree.Update(val9, new QuadTreeBoundsXZ(bounds8, boundsMask8, lod8));
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	public struct UpdateLaneSearchTreeJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Overridden> m_OverriddenType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<UtilityLane> m_UtilityLaneType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetLaneGeometryData> m_PrefabLaneGeometryData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_Loaded;

		[ReadOnly]
		public UtilityTypes m_DilatedUtilityTypes;

		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity val = nativeArray[i];
					m_SearchTree.TryRemove(val);
				}
				return;
			}
			if (m_Loaded || ((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType))
			{
				NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<Curve> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
				NativeArray<Owner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<UtilityLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<UtilityLane>(ref m_UtilityLaneType);
				NativeArray<PrefabRef> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				bool flag = ((ArchetypeChunk)(ref chunk)).Has<Overridden>(ref m_OverriddenType);
				bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<CullingInfo>();
				Owner owner = default(Owner);
				UtilityLane utilityLane = default(UtilityLane);
				UtilityLaneData utilityLaneData = default(UtilityLaneData);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity val2 = nativeArray2[j];
					Curve curve = nativeArray3[j];
					PrefabRef prefabRef = nativeArray6[j];
					Bounds3 val3 = MathUtils.Bounds(curve.m_Bezier);
					if (m_PrefabLaneGeometryData.HasComponent(prefabRef.m_Prefab))
					{
						NetLaneGeometryData netLaneGeometryData = m_PrefabLaneGeometryData[prefabRef.m_Prefab];
						val3 = MathUtils.Expand(val3, ((float3)(ref netLaneGeometryData.m_Size)).xyx * 0.5f);
						BoundsMask boundsMask = BoundsMask.Debug;
						if (!flag)
						{
							boundsMask |= BoundsMask.NotOverridden;
							if (curve.m_Length > 0.1f)
							{
								MeshLayer defaultLayers = (m_EditorMode ? netLaneGeometryData.m_EditorLayers : netLaneGeometryData.m_GameLayers);
								CollectionUtils.TryGet<Owner>(nativeArray4, j, ref owner);
								CollectionUtils.TryGet<UtilityLane>(nativeArray5, j, ref utilityLane);
								boundsMask |= CommonUtils.GetBoundsMask(GetLayers(owner, utilityLane, defaultLayers, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData));
							}
						}
						int num = netLaneGeometryData.m_MinLod;
						if (m_PrefabUtilityLaneData.TryGetComponent(prefabRef.m_Prefab, ref utilityLaneData) && (utilityLaneData.m_UtilityTypes & m_DilatedUtilityTypes) != UtilityTypes.None)
						{
							float renderingSize = RenderingUtils.GetRenderingSize(new float2(utilityLaneData.m_VisualCapacity));
							num = math.min(num, RenderingUtils.CalculateLodLimit(renderingSize));
						}
						if (!flag2)
						{
							boundsMask &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
						}
						m_SearchTree.Add(val2, new QuadTreeBoundsXZ(val3, boundsMask, num));
					}
					else
					{
						val3 = MathUtils.Expand(val3, float3.op_Implicit(0.5f));
						int lod = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(1f)));
						BoundsMask boundsMask2 = BoundsMask.Debug;
						if (!flag2)
						{
							boundsMask2 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
						}
						m_SearchTree.Add(val2, new QuadTreeBoundsXZ(val3, boundsMask2, lod));
					}
				}
				return;
			}
			NativeArray<Entity> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Curve> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			NativeArray<Owner> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<UtilityLane> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<UtilityLane>(ref m_UtilityLaneType);
			NativeArray<PrefabRef> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<Overridden>(ref m_OverriddenType);
			bool flag4 = ((ArchetypeChunk)(ref chunk)).Has<CullingInfo>();
			Owner owner2 = default(Owner);
			UtilityLane utilityLane2 = default(UtilityLane);
			UtilityLaneData utilityLaneData2 = default(UtilityLaneData);
			for (int k = 0; k < nativeArray7.Length; k++)
			{
				Entity val4 = nativeArray7[k];
				Curve curve2 = nativeArray8[k];
				PrefabRef prefabRef2 = nativeArray11[k];
				Bounds3 val5 = MathUtils.Bounds(curve2.m_Bezier);
				if (m_PrefabLaneGeometryData.HasComponent(prefabRef2.m_Prefab))
				{
					NetLaneGeometryData netLaneGeometryData2 = m_PrefabLaneGeometryData[prefabRef2.m_Prefab];
					val5 = MathUtils.Expand(val5, ((float3)(ref netLaneGeometryData2.m_Size)).xyx * 0.5f);
					BoundsMask boundsMask3 = BoundsMask.Debug;
					if (!flag3)
					{
						boundsMask3 |= BoundsMask.NotOverridden;
						if (curve2.m_Length > 0.1f)
						{
							MeshLayer defaultLayers2 = (m_EditorMode ? netLaneGeometryData2.m_EditorLayers : netLaneGeometryData2.m_GameLayers);
							CollectionUtils.TryGet<Owner>(nativeArray9, k, ref owner2);
							CollectionUtils.TryGet<UtilityLane>(nativeArray10, k, ref utilityLane2);
							boundsMask3 |= CommonUtils.GetBoundsMask(GetLayers(owner2, utilityLane2, defaultLayers2, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData));
						}
					}
					int num2 = netLaneGeometryData2.m_MinLod;
					if (m_PrefabUtilityLaneData.TryGetComponent(prefabRef2.m_Prefab, ref utilityLaneData2) && (utilityLaneData2.m_UtilityTypes & m_DilatedUtilityTypes) != UtilityTypes.None)
					{
						float renderingSize2 = RenderingUtils.GetRenderingSize(new float2(utilityLaneData2.m_VisualCapacity));
						num2 = math.min(num2, RenderingUtils.CalculateLodLimit(renderingSize2));
					}
					if (!flag4)
					{
						boundsMask3 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
					}
					m_SearchTree.Update(val4, new QuadTreeBoundsXZ(val5, boundsMask3, num2));
				}
				else
				{
					val5 = MathUtils.Expand(val5, float3.op_Implicit(0.5f));
					int lod2 = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(1f)));
					BoundsMask boundsMask4 = BoundsMask.Debug;
					if (!flag4)
					{
						boundsMask4 &= ~(BoundsMask.AllLayers | BoundsMask.Debug);
					}
					m_SearchTree.Update(val4, new QuadTreeBoundsXZ(val5, boundsMask4, lod2));
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<NodeGeometry> __Game_Net_NodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Composition> __Game_Net_Composition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Orphan> __Game_Net_Orphan_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Node> __Game_Net_Node_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Marker> __Game_Net_Marker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshRef> __Game_Prefabs_NetCompositionMeshRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshData> __Game_Prefabs_NetCompositionMeshData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Overridden> __Game_Common_Overridden_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<UtilityLane> __Game_Net_UtilityLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneGeometryData> __Game_Prefabs_NetLaneGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

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
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_EdgeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EndNodeGeometry>(true);
			__Game_Net_NodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NodeGeometry>(true);
			__Game_Net_Composition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Composition>(true);
			__Game_Net_Orphan_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Orphan>(true);
			__Game_Net_Node_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Node>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Net_Marker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Marker>(true);
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Rendering_CullingInfo_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CullingInfo>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_NetCompositionMeshRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionMeshRef>(true);
			__Game_Prefabs_NetCompositionMeshData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionMeshData>(true);
			__Game_Common_Overridden_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Overridden>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_UtilityLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UtilityLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetLaneGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneGeometryData>(true);
			__Game_Prefabs_UtilityLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLaneData>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private UndergroundViewSystem m_UndergroundViewSystem;

	private EntityQuery m_UpdatedNetsQuery;

	private EntityQuery m_UpdatedLanesQuery;

	private EntityQuery m_AllNetsQuery;

	private EntityQuery m_AllLanesQuery;

	private NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

	private NativeQuadTree<Entity, QuadTreeBoundsXZ> m_LaneSearchTree;

	private JobHandle m_NetReadDependencies;

	private JobHandle m_NetWriteDependencies;

	private JobHandle m_LaneReadDependencies;

	private JobHandle m_LaneWriteDependencies;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Expected O, but got Unknown
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_UndergroundViewSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UndergroundViewSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Edge>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Node>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[1] = val;
		m_UpdatedNetsQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[2];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<LaneGeometry>(),
			ComponentType.ReadOnly<ParkingLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array2[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<LaneGeometry>(),
			ComponentType.ReadOnly<ParkingLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array2[1] = val;
		m_UpdatedLanesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<Node>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array3[0] = val;
		m_AllNetsQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		EntityQueryDesc[] array4 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<LaneGeometry>(),
			ComponentType.ReadOnly<ParkingLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array4[0] = val;
		m_AllLanesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array4);
		m_NetSearchTree = new NativeQuadTree<Entity, QuadTreeBoundsXZ>(1f, (Allocator)4);
		m_LaneSearchTree = new NativeQuadTree<Entity, QuadTreeBoundsXZ>(1f, (Allocator)4);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_NetSearchTree.Dispose();
		m_LaneSearchTree.Dispose();
		base.OnDestroy();
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
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		EntityQuery val = (loaded ? m_AllNetsQuery : m_UpdatedNetsQuery);
		EntityQuery val2 = (loaded ? m_AllLanesQuery : m_UpdatedLanesQuery);
		bool flag = !((EntityQuery)(ref val)).IsEmptyIgnoreFilter;
		bool flag2 = !((EntityQuery)(ref val2)).IsEmptyIgnoreFilter;
		if (flag || flag2)
		{
			JobHandle val3 = default(JobHandle);
			if (flag)
			{
				JobHandle dependencies;
				JobHandle val4 = JobChunkExtensions.Schedule<UpdateNetSearchTreeJob>(new UpdateNetSearchTreeJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_EdgeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_StartGeometryType = InternalCompilerInterface.GetComponentTypeHandle<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_EndGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_NodeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OrphanType = InternalCompilerInterface.GetComponentTypeHandle<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_MarkerType = InternalCompilerInterface.GetComponentTypeHandle<Marker>(ref __TypeHandle.__Game_Net_Marker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CullingInfoType = InternalCompilerInterface.GetComponentTypeHandle<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabCompositionMeshRef = InternalCompilerInterface.GetComponentLookup<NetCompositionMeshRef>(ref __TypeHandle.__Game_Prefabs_NetCompositionMeshRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabCompositionMeshData = InternalCompilerInterface.GetComponentLookup<NetCompositionMeshData>(ref __TypeHandle.__Game_Prefabs_NetCompositionMeshData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
					m_Loaded = loaded,
					m_SearchTree = GetNetSearchTree(readOnly: false, out dependencies)
				}, val, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
				AddNetSearchTreeWriter(val4);
				val3 = JobHandle.CombineDependencies(val3, val4);
			}
			if (flag2)
			{
				JobHandle dependencies2;
				JobHandle val5 = JobChunkExtensions.Schedule<UpdateLaneSearchTreeJob>(new UpdateLaneSearchTreeJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OverriddenType = InternalCompilerInterface.GetComponentTypeHandle<Overridden>(ref __TypeHandle.__Game_Common_Overridden_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_UtilityLaneType = InternalCompilerInterface.GetComponentTypeHandle<UtilityLane>(ref __TypeHandle.__Game_Net_UtilityLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabLaneGeometryData = InternalCompilerInterface.GetComponentLookup<NetLaneGeometryData>(ref __TypeHandle.__Game_Prefabs_NetLaneGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
					m_Loaded = loaded,
					m_DilatedUtilityTypes = m_UndergroundViewSystem.utilityTypes,
					m_SearchTree = GetLaneSearchTree(readOnly: false, out dependencies2)
				}, val2, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies2));
				AddLaneSearchTreeWriter(val5);
				val3 = JobHandle.CombineDependencies(val3, val5);
			}
			((SystemBase)this).Dependency = val3;
		}
	}

	public NativeQuadTree<Entity, QuadTreeBoundsXZ> GetNetSearchTree(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_NetWriteDependencies : JobHandle.CombineDependencies(m_NetReadDependencies, m_NetWriteDependencies));
		return m_NetSearchTree;
	}

	public NativeQuadTree<Entity, QuadTreeBoundsXZ> GetLaneSearchTree(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_LaneWriteDependencies : JobHandle.CombineDependencies(m_LaneReadDependencies, m_LaneWriteDependencies));
		return m_LaneSearchTree;
	}

	public void AddNetSearchTreeReader(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_NetReadDependencies = JobHandle.CombineDependencies(m_NetReadDependencies, jobHandle);
	}

	public void AddNetSearchTreeWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_NetWriteDependencies = jobHandle;
	}

	public void AddLaneSearchTreeReader(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_LaneReadDependencies = JobHandle.CombineDependencies(m_LaneReadDependencies, jobHandle);
	}

	public void AddLaneSearchTreeWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_LaneWriteDependencies = jobHandle;
	}

	public void PreDeserialize(Context context)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeQuadTree<Entity, QuadTreeBoundsXZ> netSearchTree = GetNetSearchTree(readOnly: false, out dependencies);
		JobHandle dependencies2;
		NativeQuadTree<Entity, QuadTreeBoundsXZ> laneSearchTree = GetLaneSearchTree(readOnly: false, out dependencies2);
		((JobHandle)(ref dependencies)).Complete();
		((JobHandle)(ref dependencies2)).Complete();
		netSearchTree.Clear();
		laneSearchTree.Clear();
		m_Loaded = true;
	}

	public static MeshLayer GetLayers(Owner owner, UtilityLane utilityLane, MeshLayer defaultLayers, ref ComponentLookup<PrefabRef> prefabRefs, ref ComponentLookup<NetData> netDatas, ref ComponentLookup<NetGeometryData> netGeometryDatas)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (defaultLayers == (MeshLayer.Pipeline | MeshLayer.SubPipeline))
		{
			if ((owner.m_Owner != Entity.Null && IsNetOwnerPipeline(owner, ref prefabRefs, ref netDatas, ref netGeometryDatas)) || (utilityLane.m_Flags & UtilityLaneFlags.PipelineConnection) != 0)
			{
				return MeshLayer.Pipeline;
			}
			return MeshLayer.SubPipeline;
		}
		return defaultLayers;
	}

	public static bool IsNetOwnerPipeline(Owner owner, ref ComponentLookup<PrefabRef> prefabRefs, ref ComponentLookup<NetData> netDatas, ref ComponentLookup<NetGeometryData> netGeometryDatas)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef prefabRef = default(PrefabRef);
		NetData netData = default(NetData);
		NetGeometryData netGeometryData = default(NetGeometryData);
		if (prefabRefs.TryGetComponent(owner.m_Owner, ref prefabRef) && netDatas.TryGetComponent(prefabRef.m_Prefab, ref netData) && netGeometryDatas.TryGetComponent(prefabRef.m_Prefab, ref netGeometryData))
		{
			if ((netData.m_RequiredLayers & (Layer.PowerlineLow | Layer.PowerlineHigh | Layer.WaterPipe | Layer.SewagePipe)) != Layer.None)
			{
				return (netGeometryData.m_Flags & GeometryFlags.Marker) == 0;
			}
			return false;
		}
		return false;
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
	public SearchSystem()
	{
	}
}

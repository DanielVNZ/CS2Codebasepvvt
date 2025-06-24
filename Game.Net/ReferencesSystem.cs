using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
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
public class ReferencesSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateNodeReferencesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		public BufferTypeHandle<ConnectedEdge> m_ConnectedEdgeType;

		public BufferLookup<ConnectedNode> m_Nodes;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<ConnectedEdge> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedEdge>(ref m_ConnectedEdgeType);
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				for (int i = 0; i < bufferAccessor.Length; i++)
				{
					Entity node = nativeArray[i];
					DynamicBuffer<ConnectedEdge> val = bufferAccessor[i];
					for (int j = 0; j < val.Length; j++)
					{
						Entity edge = val[j].m_Edge;
						CollectionUtils.RemoveValue<ConnectedNode>(m_Nodes[edge], new ConnectedNode(node, 0.5f));
					}
				}
				return;
			}
			for (int k = 0; k < bufferAccessor.Length; k++)
			{
				DynamicBuffer<ConnectedEdge> val2 = bufferAccessor[k];
				for (int l = 0; l < val2.Length; l++)
				{
					Entity edge2 = val2[l].m_Edge;
					if (m_UpdatedData.HasComponent(edge2))
					{
						val2.RemoveAt(l--);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct ValidateConnectedNodesJob : IJobChunk
	{
		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		public BufferTypeHandle<ConnectedNode> m_ConnectedNodeType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<ConnectedNode> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedNode>(ref m_ConnectedNodeType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<ConnectedNode> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					Entity node = val[j].m_Node;
					if (m_DeletedData.HasComponent(node))
					{
						val.RemoveAt(j--);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateEdgeReferencesJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_EdgeChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedNode> m_ConnectedNodeType;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		public BufferLookup<ConnectedEdge> m_Edges;

		public NativeParallelMultiHashMap<Entity, ConnectedNodeValue> m_ConnectedNodes;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_EdgeChunks.Length; i++)
			{
				ArchetypeChunk val = m_EdgeChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<Edge> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Edge>(ref m_EdgeType);
				BufferAccessor<ConnectedNode> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<ConnectedNode>(ref m_ConnectedNodeType);
				if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref m_DeletedType))
				{
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Entity edge = nativeArray[j];
						Edge edge2 = nativeArray2[j];
						DynamicBuffer<ConnectedNode> val2 = bufferAccessor[j];
						DynamicBuffer<ConnectedEdge> val3 = m_Edges[edge2.m_Start];
						DynamicBuffer<ConnectedEdge> val4 = m_Edges[edge2.m_End];
						CollectionUtils.RemoveValue<ConnectedEdge>(val3, new ConnectedEdge(edge));
						CollectionUtils.RemoveValue<ConnectedEdge>(val4, new ConnectedEdge(edge));
						for (int k = 0; k < val2.Length; k++)
						{
							CollectionUtils.RemoveValue<ConnectedEdge>(m_Edges[val2[k].m_Node], new ConnectedEdge(edge));
						}
					}
					continue;
				}
				for (int l = 0; l < nativeArray.Length; l++)
				{
					Entity edge3 = nativeArray[l];
					Edge edge4 = nativeArray2[l];
					DynamicBuffer<ConnectedNode> val5 = bufferAccessor[l];
					DynamicBuffer<ConnectedEdge> val6 = m_Edges[edge4.m_Start];
					DynamicBuffer<ConnectedEdge> val7 = m_Edges[edge4.m_End];
					CollectionUtils.TryAddUniqueValue<ConnectedEdge>(val6, new ConnectedEdge(edge3));
					CollectionUtils.TryAddUniqueValue<ConnectedEdge>(val7, new ConnectedEdge(edge3));
					for (int m = 0; m < val5.Length; m++)
					{
						ConnectedNode connectedNode = val5[m];
						CollectionUtils.RemoveValue<ConnectedEdge>(m_Edges[connectedNode.m_Node], new ConnectedEdge(edge3));
						if (!m_DeletedData.HasComponent(connectedNode.m_Node))
						{
							m_ConnectedNodes.Add(connectedNode.m_Node, new ConnectedNodeValue(edge3, connectedNode.m_CurvePosition));
						}
					}
				}
			}
		}
	}

	private struct ConnectedNodeValue
	{
		public Entity m_Edge;

		public float m_CurvePosition;

		public ConnectedNodeValue(Entity edge, float curvePosition)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Edge = edge;
			m_CurvePosition = curvePosition;
		}
	}

	[BurstCompile]
	private struct RationalizeConnectedNodesJob : IJobParallelForDefer
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct LineComparer : IComparer<Segment>
		{
			public int Compare(Segment x, Segment y)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				return math.csum(math.select(int4.op_Implicit(0), math.select(new int4(-8, -4, -2, -1), new int4(8, 4, 2, 1), ((Segment)(ref x)).ab > ((Segment)(ref y)).ab), ((Segment)(ref x)).ab != ((Segment)(ref y)).ab));
			}
		}

		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_EdgeChunks;

		[ReadOnly]
		public NativeParallelMultiHashMap<Entity, ConnectedNodeValue> m_ConnectedNodes;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Standalone> m_StandaloneData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> m_PrefabLocalConnectData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<ServiceUpgradeData> m_PrefabServiceUpgradeData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		public BufferTypeHandle<ConnectedNode> m_ConnectedNodeType;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			ArchetypeChunk val = m_EdgeChunks[index];
			if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref m_DeletedType))
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
			NativeArray<Edge> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Edge>(ref m_EdgeType);
			NativeArray<Curve> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<Curve>(ref m_CurveType);
			NativeArray<Owner> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Temp> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<PrefabRef> nativeArray6 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<ConnectedNode> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<ConnectedNode>(ref m_ConnectedNodeType);
			NativeParallelHashMap<Entity, Bounds1> curveBoundsMap = default(NativeParallelHashMap<Entity, Bounds1>);
			NativeParallelHashMap<Entity, float2> nodePosMap = default(NativeParallelHashMap<Entity, float2>);
			NativeList<Segment> lineBuffer = default(NativeList<Segment>);
			NetGeometryData prefabGeometryData = default(NetGeometryData);
			Owner owner = default(Owner);
			ConnectedNodeValue connectedNodeValue = default(ConnectedNodeValue);
			NativeParallelMultiHashMapIterator<Entity> val8 = default(NativeParallelMultiHashMapIterator<Entity>);
			Temp temp = default(Temp);
			NetGeometryData prefabGeometryData2 = default(NetGeometryData);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				Entity val2 = nativeArray[i];
				Edge edge = nativeArray2[i];
				Curve curve = nativeArray3[i];
				PrefabRef prefabRef = nativeArray6[i];
				DynamicBuffer<ConnectedNode> val3 = bufferAccessor[i];
				Entity val4 = Entity.Null;
				if (nativeArray4.Length != 0)
				{
					val4 = nativeArray4[i].m_Owner;
				}
				Entity val5 = Entity.Null;
				if (nativeArray5.Length != 0)
				{
					val5 = nativeArray5[i].m_Original;
				}
				float num = 0f;
				if (m_PrefabNetGeometryData.TryGetComponent(prefabRef.m_Prefab, ref prefabGeometryData))
				{
					num = prefabGeometryData.m_DefaultWidth * 0.5f;
				}
				for (int num2 = val3.Length - 1; num2 >= 0; num2--)
				{
					ConnectedNode connectedNode = val3[num2];
					LocalConnectData localConnectData;
					float3 val6;
					float num5;
					Game.Prefabs.BuildingFlags allowDirections;
					float3 allowForward;
					if (!m_DeletedData.HasComponent(connectedNode.m_Node))
					{
						PrefabRef prefabRef2 = m_PrefabRefData[connectedNode.m_Node];
						if (m_PrefabLocalConnectData.HasComponent(prefabRef2.m_Prefab))
						{
							localConnectData = m_PrefabLocalConnectData[prefabRef2.m_Prefab];
							float num3 = 0f;
							if ((localConnectData.m_Flags & LocalConnectFlags.ChooseSides) != 0)
							{
								num3 = m_PrefabNetGeometryData[prefabRef2.m_Prefab].m_DefaultWidth * 0.5f + 0.1f;
							}
							float num4 = ClampCurvePosition(val2, edge, curve, connectedNode.m_CurvePosition, ref curveBoundsMap, ref nodePosMap, ref lineBuffer);
							val6 = MathUtils.Position(curve.m_Bezier, num4);
							float3 position = m_NodeData[connectedNode.m_Node].m_Position;
							num5 = math.distance(val6, position);
							Entity val7 = Entity.Null;
							allowDirections = (Game.Prefabs.BuildingFlags)0u;
							allowForward = default(float3);
							Game.Prefabs.BuildingFlags allowDirections2;
							float3 allowForward2;
							if (m_OwnerData.TryGetComponent(connectedNode.m_Node, ref owner))
							{
								val7 = owner.m_Owner;
								if (val4 != val7 && (GetElevationFlags(val2, edge, prefabGeometryData).m_General & CompositionFlags.General.Tunnel) == 0 && (!AllowConnection(val7, new Segment(position, val6), out allowDirections, out allowForward) || (val4 != Entity.Null && !AllowConnection(val4, new Segment(val6, position), out allowDirections2, out allowForward2))))
								{
									goto IL_0569;
								}
							}
							if ((localConnectData.m_Flags & LocalConnectFlags.ChooseBest) != 0)
							{
								bool flag = false;
								if (m_ConnectedNodes.TryGetFirstValue(connectedNode.m_Node, ref connectedNodeValue, ref val8))
								{
									while (true)
									{
										Entity val9 = Entity.Null;
										if (m_TempData.TryGetComponent(connectedNodeValue.m_Edge, ref temp))
										{
											val9 = temp.m_Original;
										}
										if (connectedNodeValue.m_Edge == val2 || connectedNodeValue.m_Edge == val5 || val9 == val2)
										{
											flag = true;
										}
										else
										{
											Edge edge2 = m_EdgeData[connectedNodeValue.m_Edge];
											Curve curve2 = m_CurveData[connectedNodeValue.m_Edge];
											PrefabRef prefabRef3 = m_PrefabRefData[connectedNodeValue.m_Edge];
											float num6 = 0f;
											if (m_PrefabNetGeometryData.TryGetComponent(prefabRef3.m_Prefab, ref prefabGeometryData2))
											{
												num6 = prefabGeometryData2.m_DefaultWidth * 0.5f;
											}
											float num7 = ClampCurvePosition(connectedNodeValue.m_Edge, edge2, curve2, connectedNodeValue.m_CurvePosition, ref curveBoundsMap, ref nodePosMap, ref lineBuffer);
											float3 val10 = MathUtils.Position(curve2.m_Bezier, num7);
											float num8 = math.distance(val10, position);
											float num9 = num5 - num - (num8 - num6);
											if ((localConnectData.m_Flags & LocalConnectFlags.ChooseSides) != 0 && num9 >= 0f && num9 - num3 <= 0f && (AreNeighbors(edge, edge2) || (GetOriginals(edge, out var originals) && AreNeighbors(originals, edge2)) || (GetOriginals(edge2, out var originals2) && AreNeighbors(edge, originals2))))
											{
												num9 -= math.sqrt(num * num + num3 * num3) - num;
											}
											if ((num9 > 0f || (num9 == 0f && !flag)) && (!(val7 != Entity.Null) || !(val4 != val7) || (GetElevationFlags(connectedNodeValue.m_Edge, edge2, prefabGeometryData2).m_General & CompositionFlags.General.Tunnel) != 0 || (AllowConnection(val7, new Segment(position, val10), out allowDirections2, out allowForward2) && (!(val4 != Entity.Null) || AllowConnection(val4, new Segment(val10, position), out allowDirections2, out allowForward2)))) && ValidateConnection(connectedNodeValue.m_Edge, connectedNode.m_Node, val9, edge2, localConnectData, val10, num8, num6, allowDirections, allowForward))
											{
												break;
											}
										}
										if (m_ConnectedNodes.TryGetNextValue(ref connectedNodeValue, ref val8))
										{
											continue;
										}
										goto IL_0548;
									}
									goto IL_0569;
								}
							}
							goto IL_0548;
						}
					}
					goto IL_0569;
					IL_0569:
					val3.RemoveAt(num2);
					continue;
					IL_0548:
					if (ValidateConnection(val2, connectedNode.m_Node, val5, edge, localConnectData, val6, num5, num, allowDirections, allowForward))
					{
						continue;
					}
					goto IL_0569;
				}
			}
			if (curveBoundsMap.IsCreated)
			{
				curveBoundsMap.Dispose();
			}
			if (nodePosMap.IsCreated)
			{
				nodePosMap.Dispose();
			}
			if (lineBuffer.IsCreated)
			{
				lineBuffer.Dispose();
			}
		}

		private CompositionFlags GetElevationFlags(Entity entity, Edge edge, NetGeometryData prefabGeometryData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			Elevation startElevation = default(Elevation);
			m_ElevationData.TryGetComponent(edge.m_Start, ref startElevation);
			Elevation middleElevation = default(Elevation);
			m_ElevationData.TryGetComponent(entity, ref middleElevation);
			Elevation endElevation = default(Elevation);
			m_ElevationData.TryGetComponent(edge.m_End, ref endElevation);
			return NetCompositionHelpers.GetElevationFlags(startElevation, middleElevation, endElevation, prefabGeometryData);
		}

		private float ClampCurvePosition(Entity entity, Edge edge, Curve curve, float curvePos, ref NativeParallelHashMap<Entity, Bounds1> curveBoundsMap, ref NativeParallelHashMap<Entity, float2> nodePosMap, ref NativeList<Segment> lineBuffer)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			Bounds1 val = default(Bounds1);
			if (curveBoundsMap.IsCreated)
			{
				if (curveBoundsMap.TryGetValue(entity, ref val))
				{
					return MathUtils.Clamp(curvePos, val);
				}
			}
			else
			{
				curveBoundsMap = new NativeParallelHashMap<Entity, Bounds1>(100, AllocatorHandle.op_Implicit((Allocator)2));
			}
			if (curve.m_Length >= 0.2f)
			{
				float2 nodePosition = GetNodePosition(edge.m_Start, ref nodePosMap, ref lineBuffer);
				float2 nodePosition2 = GetNodePosition(edge.m_End, ref nodePosMap, ref lineBuffer);
				MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, nodePosition, ref val.min);
				MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, nodePosition2, ref val.max);
				float num = 0.1f / curve.m_Length;
				val.min += num;
				val.max -= num;
				if (val.max < val.min)
				{
					val.min = (val.max = MathUtils.Center(val));
				}
			}
			else
			{
				((Bounds1)(ref val))._002Ector(0.5f, 0.5f);
			}
			curveBoundsMap.Add(entity, val);
			return MathUtils.Clamp(curvePos, val);
		}

		private float2 GetNodePosition(Entity entity, ref NativeParallelHashMap<Entity, float2> nodePosMap, ref NativeList<Segment> lineBuffer)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			if (nodePosMap.IsCreated)
			{
				float2 result = default(float2);
				if (nodePosMap.TryGetValue(entity, ref result))
				{
					return result;
				}
			}
			else
			{
				nodePosMap = new NativeParallelHashMap<Entity, float2>(100, AllocatorHandle.op_Implicit((Allocator)2));
			}
			Node node = m_NodeData[entity];
			if (!m_StandaloneData.HasComponent(entity))
			{
				PrefabRef prefabRef = m_PrefabRefData[entity];
				NetData netData = default(NetData);
				m_PrefabNetData.TryGetComponent(prefabRef.m_Prefab, ref netData);
				float3 val = default(float3);
				float3 position = node.m_Position;
				int num = 0;
				if (lineBuffer.IsCreated)
				{
					lineBuffer.Clear();
				}
				EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, entity, m_Edges, m_EdgeData, m_TempData, m_HiddenData);
				EdgeIteratorValue value;
				NetData netData2 = default(NetData);
				while (edgeIterator.GetNext(out value))
				{
					Curve curve = m_CurveData[value.m_Edge];
					PrefabRef prefabRef2 = m_PrefabRefData[value.m_Edge];
					m_PrefabNetData.TryGetComponent(prefabRef2.m_Prefab, ref netData2);
					if ((netData.m_RequiredLayers & netData2.m_RequiredLayers) == 0)
					{
						continue;
					}
					float3 val2;
					float3 val3;
					float2 val4;
					if (value.m_End)
					{
						val2 = curve.m_Bezier.d - position;
						val3 = MathUtils.EndTangent(curve.m_Bezier);
						val4 = -((float3)(ref val3)).xz;
					}
					else
					{
						val2 = curve.m_Bezier.a - position;
						val3 = MathUtils.StartTangent(curve.m_Bezier);
						val4 = ((float3)(ref val3)).xz;
					}
					val += val2;
					num++;
					if (MathUtils.TryNormalize(ref val4))
					{
						if (!lineBuffer.IsCreated)
						{
							lineBuffer = new NativeList<Segment>(10, AllocatorHandle.op_Implicit((Allocator)2));
						}
						Segment val5 = new Segment(((float3)(ref val2)).xz, val4);
						lineBuffer.Add(ref val5);
					}
				}
				if (num > 0)
				{
					node.m_Position = position + val / (float)num;
					val = default(float3);
					num = 0;
				}
				if (lineBuffer.IsCreated && lineBuffer.Length >= 2)
				{
					NativeSortExtension.Sort<Segment, LineComparer>(lineBuffer, default(LineComparer));
					Segment val10 = default(Segment);
					Segment val11 = default(Segment);
					float2 val12 = default(float2);
					for (int i = 1; i < lineBuffer.Length; i++)
					{
						Segment val6 = lineBuffer[i];
						for (int j = 0; j < i; j++)
						{
							Segment val7 = lineBuffer[j];
							float num2 = math.dot(val6.b, val7.b);
							float3 val8 = default(float3);
							if (math.abs(num2) > 0.999f)
							{
								((float3)(ref val8)).xy = val6.a + val7.a;
								val8.z = 2f;
							}
							else
							{
								float2 val9 = math.distance(val6.a, val7.a) * new float2(math.abs(num2) - 1f, 1f - math.abs(num2));
								((Segment)(ref val10))._002Ector(val6.a - val6.b * val9.x, val6.a - val6.b * val9.y);
								((Segment)(ref val11))._002Ector(val7.a - val7.b * val9.x, val7.a - val7.b * val9.y);
								MathUtils.Distance(val10, val11, ref val12);
								((float3)(ref val8)).xy = MathUtils.Position(val10, val12.x) + MathUtils.Position(val11, val12.y);
								val8.z = 2f;
							}
							float num3 = 1.01f - math.abs(num2);
							val += val8 * num3;
							num++;
						}
					}
					if (num > 0)
					{
						((float3)(ref node.m_Position)).xz = ((float3)(ref position)).xz + ((float3)(ref val)).xy / val.z;
					}
				}
			}
			nodePosMap.Add(entity, ((float3)(ref node.m_Position)).xz);
			return ((float3)(ref node.m_Position)).xz;
		}

		private bool AreNeighbors(Edge edge1, Edge edge2)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (!(edge1.m_Start == edge2.m_Start) && !(edge1.m_End == edge2.m_Start) && !(edge1.m_Start == edge2.m_End))
			{
				return edge1.m_End == edge2.m_End;
			}
			return true;
		}

		private bool GetOriginals(Edge edge, out Edge originals)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			Temp temp = default(Temp);
			Temp temp2 = default(Temp);
			if (m_TempData.TryGetComponent(edge.m_Start, ref temp) && m_TempData.TryGetComponent(edge.m_End, ref temp2))
			{
				originals.m_Start = temp.m_Original;
				originals.m_End = temp2.m_Original;
				return true;
			}
			originals = default(Edge);
			return false;
		}

		private bool ValidateConnection(Entity entity, Entity node, Entity original, Edge edge, LocalConnectData localConnectData, float3 edgePosition, float nodeDistance, float size, Game.Prefabs.BuildingFlags allowDirections, float3 allowForward)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, node, m_Edges, m_EdgeData, m_TempData, m_HiddenData, (localConnectData.m_Flags & LocalConnectFlags.ChooseBest) != 0);
			int num = 0;
			EdgeIteratorValue value;
			bool4 val10 = default(bool4);
			while (edgeIterator.GetNext(out value))
			{
				if (value.m_Middle)
				{
					if (value.m_Edge != entity && value.m_Edge != original)
					{
						if ((localConnectData.m_Flags & LocalConnectFlags.ChooseSides) == 0)
						{
							return false;
						}
						Edge edge2 = m_EdgeData[value.m_Edge];
						if (!AreNeighbors(edge, edge2) && (!GetOriginals(edge, out var originals) || !AreNeighbors(originals, edge2)))
						{
							return false;
						}
					}
					continue;
				}
				Edge edge3 = m_EdgeData[value.m_Edge];
				Entity val = (value.m_End ? edge3.m_Start : edge3.m_End);
				if (val == edge.m_Start || val == edge.m_End)
				{
					return false;
				}
				if (++num >= 2 && (localConnectData.m_Flags & LocalConnectFlags.RequireDeadend) != 0)
				{
					return false;
				}
				float3 position = m_NodeData[val].m_Position;
				if (math.distance(edgePosition, position) < nodeDistance)
				{
					return false;
				}
				Curve curve = m_CurveData[value.m_Edge];
				float3 val2;
				float2 val3;
				if (!value.m_End)
				{
					val2 = MathUtils.StartTangent(curve.m_Bezier);
					val3 = -((float3)(ref val2)).xz;
				}
				else
				{
					val2 = MathUtils.EndTangent(curve.m_Bezier);
					val3 = ((float3)(ref val2)).xz;
				}
				float2 val4 = math.normalizesafe(val3, default(float2));
				float2 val5 = (value.m_End ? ((float3)(ref curve.m_Bezier.d)).xz : ((float3)(ref curve.m_Bezier.a)).xz) - val4 * size;
				float2 val6 = math.normalizesafe(((float3)(ref edgePosition)).xz - val5, default(float2));
				if (math.dot(val4, val6) < 0.70710677f)
				{
					return false;
				}
				val2 = default(float3);
				if (!((float3)(ref allowForward)).Equals(val2))
				{
					float2 val7 = math.normalizesafe(((float3)(ref allowForward)).xz, default(float2));
					float2 val8 = new float2(math.dot(val4, val7), math.dot(val4, MathUtils.Left(val7)));
					bool4 val9 = new float4(val8, -val8) >= 0.70710677f;
					((bool4)(ref val10))._002Ector(true, (allowDirections & Game.Prefabs.BuildingFlags.RightAccess) != 0, (allowDirections & Game.Prefabs.BuildingFlags.BackAccess) != 0, (allowDirections & Game.Prefabs.BuildingFlags.LeftAccess) != 0);
					if (!math.any(val9 & val10))
					{
						return false;
					}
				}
			}
			PrefabRef prefabRef = m_PrefabRefData[entity];
			PrefabRef prefabRef2 = m_PrefabRefData[node];
			if ((m_PrefabServiceUpgradeData.HasComponent(prefabRef.m_Prefab) || m_PrefabServiceUpgradeData.HasComponent(prefabRef2.m_Prefab)) && GetTopOwner(entity) != GetTopOwner(node))
			{
				return false;
			}
			return true;
		}

		private Entity GetTopOwner(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			Temp temp = default(Temp);
			Owner owner = default(Owner);
			while (true)
			{
				if (m_TempData.TryGetComponent(entity, ref temp) && temp.m_Original != Entity.Null)
				{
					entity = temp.m_Original;
				}
				if (!m_OwnerData.TryGetComponent(entity, ref owner))
				{
					break;
				}
				entity = owner.m_Owner;
			}
			return entity;
		}

		private bool AllowConnection(Entity owner, Segment line, out Game.Prefabs.BuildingFlags allowDirections, out float3 allowForward)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			allowDirections = (Game.Prefabs.BuildingFlags)0u;
			allowForward = default(float3);
			Owner owner2 = default(Owner);
			while (m_OwnerData.TryGetComponent(owner, ref owner2) && !m_BuildingData.HasComponent(owner))
			{
				owner = owner2.m_Owner;
			}
			PrefabRef prefabRef = default(PrefabRef);
			Transform transform = default(Transform);
			if (!m_PrefabRefData.TryGetComponent(owner, ref prefabRef) || !m_TransformData.TryGetComponent(owner, ref transform))
			{
				return true;
			}
			BuildingData buildingData = default(BuildingData);
			if (!m_PrefabBuildingData.TryGetComponent(prefabRef.m_Prefab, ref buildingData))
			{
				return true;
			}
			float2 size = float2.op_Implicit(buildingData.m_LotSize) * 8f;
			Quad3 val = ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, size);
			Quad2 xz = ((Quad3)(ref val)).xz;
			float2 val2 = default(float2);
			if ((buildingData.m_Flags & Game.Prefabs.BuildingFlags.LeftAccess) == 0 && MathUtils.Intersect(((Segment)(ref line)).xz, ((Quad2)(ref xz)).bc, ref val2))
			{
				return false;
			}
			if ((buildingData.m_Flags & Game.Prefabs.BuildingFlags.BackAccess) == 0 && MathUtils.Intersect(((Segment)(ref line)).xz, ((Quad2)(ref xz)).cd, ref val2))
			{
				return false;
			}
			if ((buildingData.m_Flags & Game.Prefabs.BuildingFlags.RightAccess) == 0 && MathUtils.Intersect(((Segment)(ref line)).xz, ((Quad2)(ref xz)).da, ref val2))
			{
				return false;
			}
			allowDirections = buildingData.m_Flags;
			allowForward = math.forward(transform.m_Rotation);
			return true;
		}
	}

	[BurstCompile]
	private struct AddConnectedNodeReferencesJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_EdgeChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedNode> m_ConnectedNodeType;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		public BufferLookup<ConnectedEdge> m_Edges;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_EdgeChunks.Length; i++)
			{
				ArchetypeChunk val = m_EdgeChunks[i];
				if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref m_DeletedType))
				{
					continue;
				}
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				BufferAccessor<ConnectedNode> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<ConnectedNode>(ref m_ConnectedNodeType);
				bool flag = ((ArchetypeChunk)(ref val)).Has<Temp>(ref m_TempType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity edge = nativeArray[j];
					DynamicBuffer<ConnectedNode> val2 = bufferAccessor[j];
					for (int k = 0; k < val2.Length; k++)
					{
						Entity node = val2[k].m_Node;
						if (!flag || m_TempData.HasComponent(node))
						{
							CollectionUtils.TryAddUniqueValue<ConnectedEdge>(m_Edges[node], new ConnectedEdge(edge));
						}
					}
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		public BufferTypeHandle<ConnectedEdge> __Game_Net_ConnectedEdge_RW_BufferTypeHandle;

		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		public BufferTypeHandle<ConnectedNode> __Game_Net_ConnectedNode_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferTypeHandle;

		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RW_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Standalone> __Game_Net_Standalone_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> __Game_Prefabs_LocalConnectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceUpgradeData> __Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

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
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Net_ConnectedEdge_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedEdge>(false);
			__Game_Net_ConnectedNode_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(false);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Net_ConnectedNode_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedNode>(false);
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Net_ConnectedNode_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedNode>(true);
			__Game_Net_ConnectedEdge_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(false);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Elevation>(true);
			__Game_Net_Standalone_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Standalone>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_LocalConnectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalConnectData>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceUpgradeData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
		}
	}

	private EntityQuery m_EdgeQuery;

	private EntityQuery m_NodeQuery;

	private EntityQuery m_TempEdgeQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Edge>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_EdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Node>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array2[0] = val;
		m_NodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Edge>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array3[0] = val;
		m_TempEdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_NodeQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle dependency = JobChunkExtensions.Schedule<UpdateNodeReferencesJob>(new UpdateNodeReferencesJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdgeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Nodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			}, m_NodeQuery, ((SystemBase)this).Dependency);
			((SystemBase)this).Dependency = dependency;
			if (!((EntityQuery)(ref m_TempEdgeQuery)).IsEmptyIgnoreFilter)
			{
				JobHandle dependency2 = JobChunkExtensions.ScheduleParallel<ValidateConnectedNodesJob>(new ValidateConnectedNodesJob
				{
					m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectedNodeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
				}, m_TempEdgeQuery, ((SystemBase)this).Dependency);
				((SystemBase)this).Dependency = dependency2;
			}
		}
		if (!((EntityQuery)(ref m_EdgeQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val2 = default(JobHandle);
			NativeList<ArchetypeChunk> val = ((EntityQuery)(ref m_EdgeQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
			NativeParallelMultiHashMap<Entity, ConnectedNodeValue> connectedNodes = default(NativeParallelMultiHashMap<Entity, ConnectedNodeValue>);
			connectedNodes._002Ector(32, AllocatorHandle.op_Implicit((Allocator)3));
			UpdateEdgeReferencesJob updateEdgeReferencesJob = new UpdateEdgeReferencesJob
			{
				m_EdgeChunks = val,
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedNodeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedNodes = connectedNodes
			};
			RationalizeConnectedNodesJob rationalizeConnectedNodesJob = new RationalizeConnectedNodesJob
			{
				m_EdgeChunks = val.AsDeferredJobArray(),
				m_ConnectedNodes = connectedNodes,
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationData = InternalCompilerInterface.GetComponentLookup<Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StandaloneData = InternalCompilerInterface.GetComponentLookup<Standalone>(ref __TypeHandle.__Game_Net_Standalone_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabLocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnectData>(ref __TypeHandle.__Game_Prefabs_LocalConnectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<ServiceUpgradeData>(ref __TypeHandle.__Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedNodeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
			};
			AddConnectedNodeReferencesJob obj = new AddConnectedNodeReferencesJob
			{
				m_EdgeChunks = val,
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedNodeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			JobHandle val3 = IJobExtensions.Schedule<UpdateEdgeReferencesJob>(updateEdgeReferencesJob, JobHandle.CombineDependencies(val2, ((SystemBase)this).Dependency));
			JobHandle val4 = IJobParallelForDeferExtensions.Schedule<RationalizeConnectedNodesJob, ArchetypeChunk>(rationalizeConnectedNodesJob, val, 1, val3);
			JobHandle val5 = IJobExtensions.Schedule<AddConnectedNodeReferencesJob>(obj, val4);
			val.Dispose(val5);
			connectedNodes.Dispose(val4);
			((SystemBase)this).Dependency = val5;
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
	public ReferencesSystem()
	{
	}
}

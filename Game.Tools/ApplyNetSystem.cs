using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ApplyNetSystem : GameSystemBase
{
	[BurstCompile]
	private struct PatchTempReferencesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NodeType;

		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		public ComponentLookup<Owner> m_OwnerData;

		public BufferLookup<Game.Net.SubNet> m_SubNets;

		public BufferLookup<ConnectedEdge> m_Edges;

		public BufferLookup<ConnectedNode> m_Nodes;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Edge> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
			NativeArray<Node> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Node>(ref m_NodeType);
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				Entity val = nativeArray[i];
				Edge edge = nativeArray3[i];
				Temp temp = nativeArray2[i];
				if ((temp.m_Flags & TempFlags.Delete) != 0)
				{
					continue;
				}
				if (temp.m_Original != Entity.Null && (temp.m_Flags & (TempFlags.Replace | TempFlags.Combine)) == 0)
				{
					Temp temp2 = m_TempData[edge.m_Start];
					if (temp2.m_Original != Entity.Null && (temp2.m_Flags & (TempFlags.Delete | TempFlags.Replace)) == 0)
					{
						edge.m_Start = temp2.m_Original;
					}
					Temp temp3 = m_TempData[edge.m_End];
					if (temp3.m_Original != Entity.Null && (temp3.m_Flags & (TempFlags.Delete | TempFlags.Replace)) == 0)
					{
						edge.m_End = temp3.m_Original;
					}
					DynamicBuffer<ConnectedNode> val2 = m_Nodes[temp.m_Original];
					DynamicBuffer<ConnectedNode> val3 = m_Nodes[val];
					for (int j = 0; j < val2.Length; j++)
					{
						ConnectedNode connectedNode = val2[j];
						CollectionUtils.RemoveValue<ConnectedEdge>(m_Edges[connectedNode.m_Node], new ConnectedEdge(temp.m_Original));
					}
					val2.Clear();
					for (int k = 0; k < val3.Length; k++)
					{
						ConnectedNode connectedNode2 = val3[k];
						if (m_TempData.HasComponent(connectedNode2.m_Node))
						{
							Temp temp4 = m_TempData[connectedNode2.m_Node];
							if (temp4.m_Original != Entity.Null && (temp4.m_Flags & (TempFlags.Delete | TempFlags.Replace)) == 0)
							{
								connectedNode2.m_Node = temp4.m_Original;
							}
						}
						DynamicBuffer<ConnectedEdge> val4 = m_Edges[connectedNode2.m_Node];
						CollectionUtils.TryAddUniqueValue<ConnectedNode>(val2, connectedNode2);
						CollectionUtils.TryAddUniqueValue<ConnectedEdge>(val4, new ConnectedEdge(temp.m_Original));
					}
				}
				else
				{
					Temp temp5 = m_TempData[edge.m_Start];
					if (temp5.m_Original != Entity.Null && (temp5.m_Flags & (TempFlags.Delete | TempFlags.Replace)) == 0)
					{
						CollectionUtils.RemoveValue<ConnectedEdge>(m_Edges[edge.m_Start], new ConnectedEdge(val));
						edge.m_Start = temp5.m_Original;
						CollectionUtils.TryAddUniqueValue<ConnectedEdge>(m_Edges[edge.m_Start], new ConnectedEdge(val));
					}
					Temp temp6 = m_TempData[edge.m_End];
					if (temp6.m_Original != Entity.Null && (temp6.m_Flags & (TempFlags.Delete | TempFlags.Replace)) == 0)
					{
						CollectionUtils.RemoveValue<ConnectedEdge>(m_Edges[edge.m_End], new ConnectedEdge(val));
						edge.m_End = temp6.m_Original;
						CollectionUtils.TryAddUniqueValue<ConnectedEdge>(m_Edges[edge.m_End], new ConnectedEdge(val));
					}
					DynamicBuffer<ConnectedNode> val5 = m_Nodes[val];
					for (int l = 0; l < val5.Length; l++)
					{
						ConnectedNode connectedNode3 = val5[l];
						if (m_TempData.HasComponent(connectedNode3.m_Node))
						{
							Temp temp7 = m_TempData[connectedNode3.m_Node];
							if (temp7.m_Original != Entity.Null && (temp7.m_Flags & (TempFlags.Delete | TempFlags.Replace)) == 0)
							{
								connectedNode3.m_Node = temp7.m_Original;
								val5[l] = connectedNode3;
							}
						}
						CollectionUtils.TryAddUniqueValue<ConnectedEdge>(m_Edges[connectedNode3.m_Node], new ConnectedEdge(val));
					}
				}
				nativeArray3[i] = edge;
			}
			if (nativeArray4.Length == 0 && nativeArray3.Length == 0)
			{
				return;
			}
			for (int m = 0; m < nativeArray2.Length; m++)
			{
				Entity val6 = nativeArray[m];
				Temp temp8 = nativeArray2[m];
				Entity val7 = Entity.Null;
				Entity val8 = Entity.Null;
				bool flag = false;
				if (m_OwnerData.HasComponent(val6))
				{
					val8 = m_OwnerData[val6].m_Owner;
					val7 = val8;
					if (m_TempData.HasComponent(val8))
					{
						Temp temp9 = m_TempData[val8];
						if (temp9.m_Original != Entity.Null && (temp9.m_Flags & (TempFlags.Replace | TempFlags.Combine)) == 0)
						{
							flag = true;
							val8 = temp9.m_Original;
							m_OwnerData[val6] = new Owner(val8);
						}
						else if ((temp9.m_Flags & (TempFlags.Delete | TempFlags.Cancel)) == 0)
						{
							flag = true;
						}
					}
				}
				if (temp8.m_Original != Entity.Null && (temp8.m_Flags & (TempFlags.Delete | TempFlags.Replace)) == 0)
				{
					if (flag && m_SubNets.HasBuffer(val7))
					{
						CollectionUtils.RemoveValue<Game.Net.SubNet>(m_SubNets[val7], new Game.Net.SubNet(val6));
					}
					val6 = temp8.m_Original;
					val7 = ((!m_OwnerData.HasComponent(val6)) ? Entity.Null : m_OwnerData[val6].m_Owner);
				}
				if (val7 != val8)
				{
					if (m_SubNets.HasBuffer(val7))
					{
						CollectionUtils.RemoveValue<Game.Net.SubNet>(m_SubNets[val7], new Game.Net.SubNet(val6));
					}
					if (m_SubNets.HasBuffer(val8))
					{
						CollectionUtils.TryAddUniqueValue<Game.Net.SubNet>(m_SubNets[val8], new Game.Net.SubNet(val6));
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
	private struct FixConnectedEdgesJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_Nodes;

		[NativeDisableParallelForRestriction]
		public BufferLookup<ConnectedEdge> m_Edges;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			if (!((ArchetypeChunk)(ref chunk)).Has<Node>(ref m_NodeType))
			{
				return;
			}
			NativeArray<Temp> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Temp temp = nativeArray[i];
				if (!(temp.m_Original != Entity.Null) || (temp.m_Flags & (TempFlags.Delete | TempFlags.Replace)) != 0 || !m_Edges.HasBuffer(temp.m_Original))
				{
					continue;
				}
				DynamicBuffer<ConnectedEdge> val = m_Edges[temp.m_Original];
				for (int num = val.Length - 1; num >= 0; num--)
				{
					Entity edge = val[num].m_Edge;
					Edge edge2 = m_EdgeData[edge];
					if (edge2.m_Start == temp.m_Original || edge2.m_End == temp.m_Original)
					{
						continue;
					}
					if (m_Nodes.HasBuffer(edge))
					{
						DynamicBuffer<ConnectedNode> val2 = m_Nodes[edge];
						int num2 = 0;
						while (num2 < val2.Length)
						{
							if (!(val2[num2].m_Node == temp.m_Original))
							{
								num2++;
								continue;
							}
							goto IL_0129;
						}
					}
					val.RemoveAt(num);
					IL_0129:;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct HandleTempEntitiesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NetNodeType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_NetEdgeType;

		[ReadOnly]
		public ComponentTypeHandle<Lane> m_NetLaneType;

		[ReadOnly]
		public ComponentLookup<Edge> m_NetEdgeData;

		[ReadOnly]
		public ComponentLookup<LandValue> m_LandValueData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Native> m_NativeData;

		[ReadOnly]
		public ComponentLookup<Upgraded> m_UpgradedData;

		[ReadOnly]
		public ComponentLookup<LocalConnect> m_LocalConnectData;

		[ReadOnly]
		public ComponentLookup<TramTrack> m_TramTrackData;

		[ReadOnly]
		public ComponentLookup<TrainTrack> m_TrainTrackData;

		[ReadOnly]
		public ComponentLookup<Waterway> m_WaterwayData;

		[ReadOnly]
		public ComponentLookup<Road> m_RoadData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Pollution> m_PollutionData;

		[ReadOnly]
		public ComponentLookup<TrafficLights> m_TrafficLightsData;

		[ReadOnly]
		public ComponentLookup<Orphan> m_OrphanData;

		[ReadOnly]
		public ComponentLookup<EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<Aggregated> m_AggregatedData;

		[ReadOnly]
		public ComponentLookup<Standalone> m_StandaloneData;

		[ReadOnly]
		public ComponentLookup<Marker> m_MarkerData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Recent> m_RecentData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabData;

		[ReadOnly]
		public BufferLookup<SubReplacement> m_SubReplacements;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public EconomyParameterData m_EconomyParameterData;

		[ReadOnly]
		public ComponentTypeSet m_ApplyCreatedTypes;

		[ReadOnly]
		public ComponentTypeSet m_ApplyUpdatedTypes;

		[ReadOnly]
		public ComponentTypeSet m_ApplyDeletedTypes;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0762: Unknown result type (might be due to invalid IL or missing references)
			//IL_0723: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_073c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0751: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_065a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Node> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Node>(ref m_NetNodeType);
			if (nativeArray3.Length != 0)
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity val = nativeArray[i];
					Temp temp = nativeArray2[i];
					if ((temp.m_Flags & TempFlags.Delete) != 0)
					{
						Delete(unfilteredChunkIndex, val, temp);
					}
					else if ((temp.m_Flags & TempFlags.Replace) != 0)
					{
						if (temp.m_Original != Entity.Null)
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, temp.m_Original, default(Deleted));
						}
						Create(unfilteredChunkIndex, val, temp);
					}
					else if (temp.m_Original != Entity.Null)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Node>(unfilteredChunkIndex, temp.m_Original, nativeArray3[i]);
						UpdateComponent<Owner>(unfilteredChunkIndex, val, temp.m_Original, m_OwnerData, updateValue: true);
						UpdateComponent<PrefabRef>(unfilteredChunkIndex, val, temp.m_Original, m_PrefabRefData, updateValue: true);
						UpdateComponent<Upgraded>(unfilteredChunkIndex, val, temp.m_Original, m_UpgradedData, updateValue: true);
						UpdateComponent<LocalConnect>(unfilteredChunkIndex, val, temp.m_Original, m_LocalConnectData, updateValue: false);
						UpdateComponent<TrafficLights>(unfilteredChunkIndex, val, temp.m_Original, m_TrafficLightsData, updateValue: false);
						UpdateComponent<Orphan>(unfilteredChunkIndex, val, temp.m_Original, m_OrphanData, updateValue: false);
						UpdateComponent<EditorContainer>(unfilteredChunkIndex, val, temp.m_Original, m_EditorContainerData, updateValue: true);
						UpdateComponent<Native>(unfilteredChunkIndex, val, temp.m_Original, m_NativeData, updateValue: false);
						UpdateComponent<Standalone>(unfilteredChunkIndex, val, temp.m_Original, m_StandaloneData, updateValue: false);
						if (m_PrefabData.IsComponentEnabled(m_PrefabRefData[val].m_Prefab))
						{
							UpdateComponent<LandValue>(unfilteredChunkIndex, val, temp.m_Original, m_LandValueData, updateValue: false);
							UpdateComponent<TramTrack>(unfilteredChunkIndex, val, temp.m_Original, m_TramTrackData, updateValue: false);
							UpdateComponent<TrainTrack>(unfilteredChunkIndex, val, temp.m_Original, m_TrainTrackData, updateValue: false);
							UpdateComponent<Waterway>(unfilteredChunkIndex, val, temp.m_Original, m_WaterwayData, updateValue: false);
							UpdateComponent(unfilteredChunkIndex, val, temp.m_Original, m_RoadData);
							UpdateComponent<Game.Net.Pollution>(unfilteredChunkIndex, val, temp.m_Original, m_PollutionData, updateValue: false);
							UpdateComponent<Marker>(unfilteredChunkIndex, val, temp.m_Original, m_MarkerData, updateValue: false);
						}
						Update(unfilteredChunkIndex, val, temp);
					}
					else
					{
						Create(unfilteredChunkIndex, val, temp);
					}
				}
				return;
			}
			if (((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_NetEdgeType).Length != 0)
			{
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val2 = nativeArray[j];
					Temp temp2 = nativeArray2[j];
					if (m_AggregatedData.HasComponent(val2))
					{
						Aggregated aggregated = m_AggregatedData[val2];
						if (m_TempData.HasComponent(aggregated.m_Aggregate))
						{
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Aggregated>(unfilteredChunkIndex, val2, default(Aggregated));
						}
					}
					if ((temp2.m_Flags & TempFlags.Delete) != 0)
					{
						Delete(unfilteredChunkIndex, val2, temp2);
					}
					else if ((temp2.m_Flags & (TempFlags.Replace | TempFlags.Combine)) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, temp2.m_Original, default(Deleted));
						Create(unfilteredChunkIndex, val2, temp2);
					}
					else if (temp2.m_Original != Entity.Null)
					{
						UpdateComponent<PrefabRef>(unfilteredChunkIndex, val2, temp2.m_Original, m_PrefabRefData, updateValue: true);
						UpdateComponent<Upgraded>(unfilteredChunkIndex, val2, temp2.m_Original, m_UpgradedData, updateValue: true);
						UpdateBuffer<SubReplacement>(unfilteredChunkIndex, val2, temp2.m_Original, m_SubReplacements, out DynamicBuffer<SubReplacement> _, updateValue: true);
						UpdateComponent<Curve>(unfilteredChunkIndex, val2, temp2.m_Original, m_CurveData, updateValue: true);
						UpdateComponent<Edge>(unfilteredChunkIndex, val2, temp2.m_Original, m_NetEdgeData, updateValue: true);
						UpdateComponent<EditorContainer>(unfilteredChunkIndex, val2, temp2.m_Original, m_EditorContainerData, updateValue: true);
						UpdateComponent<Native>(unfilteredChunkIndex, val2, temp2.m_Original, m_NativeData, updateValue: false);
						if (m_PrefabData.IsComponentEnabled(m_PrefabRefData[val2].m_Prefab))
						{
							UpdateComponent(unfilteredChunkIndex, val2, temp2.m_Original, m_RoadData);
							UpdateComponent<Game.Net.Pollution>(unfilteredChunkIndex, val2, temp2.m_Original, m_PollutionData, updateValue: false);
							UpdateComponent<TramTrack>(unfilteredChunkIndex, val2, temp2.m_Original, m_TramTrackData, updateValue: false);
							UpdateComponent<LandValue>(unfilteredChunkIndex, val2, temp2.m_Original, m_LandValueData, updateValue: false);
						}
						Update(unfilteredChunkIndex, val2, temp2);
					}
					else
					{
						Create(unfilteredChunkIndex, val2, temp2);
					}
				}
				return;
			}
			if (((ArchetypeChunk)(ref chunk)).GetNativeArray<Lane>(ref m_NetLaneType).Length != 0)
			{
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Entity val3 = nativeArray[k];
					Temp temp3 = nativeArray2[k];
					if ((temp3.m_Flags & TempFlags.Delete) != 0)
					{
						Delete(unfilteredChunkIndex, val3, temp3);
						continue;
					}
					if ((temp3.m_Flags & TempFlags.Replace) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, temp3.m_Original, default(Deleted));
						Create(unfilteredChunkIndex, val3, temp3);
						continue;
					}
					if (temp3.m_Original != Entity.Null)
					{
						if (m_OwnerData.HasComponent(val3))
						{
							Owner owner = m_OwnerData[val3];
							if (m_TempData.HasComponent(owner.m_Owner))
							{
								Temp temp4 = m_TempData[owner.m_Owner];
								if (temp4.m_Original != Entity.Null && (temp4.m_Flags & (TempFlags.Replace | TempFlags.Combine)) != 0)
								{
									((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, temp3.m_Original, default(Deleted));
									Create(unfilteredChunkIndex, val3, temp3);
									continue;
								}
							}
						}
						Update(unfilteredChunkIndex, val3, temp3);
						continue;
					}
					if (m_OwnerData.HasComponent(val3))
					{
						Owner owner2 = m_OwnerData[val3];
						if (m_TempData.HasComponent(owner2.m_Owner))
						{
							Temp temp5 = m_TempData[owner2.m_Owner];
							if (temp5.m_Original != Entity.Null && (temp5.m_Flags & (TempFlags.Replace | TempFlags.Combine)) == 0)
							{
								if ((temp5.m_Flags & TempFlags.Upgrade) != 0)
								{
									((ParallelWriter)(ref m_CommandBuffer)).AddComponent(unfilteredChunkIndex, val3, ref m_ApplyDeletedTypes);
								}
								else
								{
									((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val3, default(Deleted));
								}
								continue;
							}
						}
					}
					Create(unfilteredChunkIndex, val3, temp3);
				}
				return;
			}
			for (int l = 0; l < nativeArray.Length; l++)
			{
				Entity val4 = nativeArray[l];
				Temp temp6 = nativeArray2[l];
				if ((temp6.m_Flags & TempFlags.Delete) == 0 && temp6.m_Original != Entity.Null && m_HiddenData.HasComponent(temp6.m_Original))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Hidden>(unfilteredChunkIndex, temp6.m_Original);
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val4, default(Deleted));
			}
		}

		private void Delete(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (temp.m_Original != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, temp.m_Original, default(Deleted));
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
		}

		private void UpdateComponent<T>(int chunkIndex, Entity entity, Entity original, ComponentLookup<T> data, bool updateValue) where T : unmanaged, IComponentData
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (data.HasComponent(entity))
			{
				if (data.HasComponent(original))
				{
					if (updateValue)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<T>(chunkIndex, original, data[entity]);
					}
				}
				else if (updateValue)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<T>(chunkIndex, original, data[entity]);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<T>(chunkIndex, original, default(T));
				}
			}
			else if (data.HasComponent(original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<T>(chunkIndex, original);
			}
		}

		private bool UpdateBuffer<T>(int chunkIndex, Entity entity, Entity original, BufferLookup<T> data, out DynamicBuffer<T> oldBuffer, bool updateValue) where T : unmanaged, IBufferElementData
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (data.HasBuffer(entity))
			{
				if (data.HasBuffer(original))
				{
					if (updateValue)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<T>(chunkIndex, original).CopyFrom(data[entity]);
					}
				}
				else if (updateValue)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<T>(chunkIndex, original).CopyFrom(data[entity]);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<T>(chunkIndex, original);
				}
			}
			else if (data.TryGetBuffer(original, ref oldBuffer))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<T>(chunkIndex, original);
				return true;
			}
			oldBuffer = default(DynamicBuffer<T>);
			return false;
		}

		private void UpdateComponent(int chunkIndex, Entity entity, Entity original, ComponentLookup<Road> data)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (data.HasComponent(entity))
			{
				if (data.HasComponent(original))
				{
					Road road = data[original];
					road.m_Flags = data[entity].m_Flags;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Road>(chunkIndex, original, road);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Road>(chunkIndex, original, data[entity]);
				}
			}
			else if (data.HasComponent(original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Road>(chunkIndex, original);
			}
		}

		private void Update(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			if (m_HiddenData.HasComponent(temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Hidden>(chunkIndex, temp.m_Original);
			}
			if (temp.m_Cost != 0)
			{
				Recent recent = new Recent
				{
					m_ModificationFrame = m_SimulationFrame,
					m_ModificationCost = temp.m_Cost
				};
				Recent recent2 = default(Recent);
				if (m_RecentData.TryGetComponent(temp.m_Original, ref recent2))
				{
					recent.m_ModificationCost += recent2.m_ModificationCost;
					recent.m_ModificationCost += NetUtils.GetRefundAmount(recent2, m_SimulationFrame, m_EconomyParameterData);
					recent2.m_ModificationFrame = m_SimulationFrame;
					recent.m_ModificationCost -= NetUtils.GetRefundAmount(recent2, m_SimulationFrame, m_EconomyParameterData);
					recent.m_ModificationCost = math.min(recent.m_ModificationCost, temp.m_Value);
					if (recent.m_ModificationCost > 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Recent>(chunkIndex, temp.m_Original, recent);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Recent>(chunkIndex, temp.m_Original);
					}
				}
				else if (recent.m_ModificationCost > 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Recent>(chunkIndex, temp.m_Original, recent);
				}
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(chunkIndex, temp.m_Original, ref m_ApplyUpdatedTypes);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
		}

		private void Create(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Temp>(chunkIndex, entity);
			if (temp.m_Cost > 0)
			{
				Recent recent = new Recent
				{
					m_ModificationFrame = m_SimulationFrame,
					m_ModificationCost = temp.m_Cost
				};
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Recent>(chunkIndex, entity, recent);
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(chunkIndex, entity, ref m_ApplyCreatedTypes);
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
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Node> __Game_Net_Node_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Edge> __Game_Net_Edge_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		public ComponentLookup<Owner> __Game_Common_Owner_RW_ComponentLookup;

		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RW_BufferLookup;

		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RW_BufferLookup;

		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Lane> __Game_Net_Lane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<LandValue> __Game_Net_LandValue_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Upgraded> __Game_Net_Upgraded_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalConnect> __Game_Net_LocalConnect_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TramTrack> __Game_Net_TramTrack_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainTrack> __Game_Net_TrainTrack_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Waterway> __Game_Net_Waterway_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Road> __Game_Net_Road_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Pollution> __Game_Net_Pollution_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrafficLights> __Game_Net_TrafficLights_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Orphan> __Game_Net_Orphan_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Aggregated> __Game_Net_Aggregated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Standalone> __Game_Net_Standalone_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Marker> __Game_Net_Marker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Recent> __Game_Tools_Recent_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubReplacement> __Game_Net_SubReplacement_RO_BufferLookup;

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
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Net_Node_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Node>(true);
			__Game_Net_Edge_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(false);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Common_Owner_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(false);
			__Game_Net_SubNet_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(false);
			__Game_Net_ConnectedEdge_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(false);
			__Game_Net_ConnectedNode_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(false);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Net_Lane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lane>(true);
			__Game_Net_LandValue_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LandValue>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Net_Upgraded_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Upgraded>(true);
			__Game_Net_LocalConnect_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalConnect>(true);
			__Game_Net_TramTrack_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TramTrack>(true);
			__Game_Net_TrainTrack_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainTrack>(true);
			__Game_Net_Waterway_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Waterway>(true);
			__Game_Net_Road_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Pollution_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Pollution>(true);
			__Game_Net_TrafficLights_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficLights>(true);
			__Game_Net_Orphan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Orphan>(true);
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EditorContainer>(true);
			__Game_Net_Aggregated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Aggregated>(true);
			__Game_Net_Standalone_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Standalone>(true);
			__Game_Net_Marker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Marker>(true);
			__Game_Tools_Recent_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Recent>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
			__Game_Net_SubReplacement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubReplacement>(true);
		}
	}

	private ToolOutputBarrier m_ToolOutputBarrier;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_TempQuery;

	private EntityQuery m_EconomyParameterQuery;

	private ComponentTypeSet m_ApplyCreatedTypes;

	private ComponentTypeSet m_ApplyUpdatedTypes;

	private ComponentTypeSet m_ApplyDeletedTypes;

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
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<Lane>(),
			ComponentType.ReadOnly<Aggregate>()
		};
		val.None = (ComponentType[])(object)new ComponentType[0];
		array[0] = val;
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_ApplyCreatedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Created>(), ComponentType.ReadWrite<Updated>());
		m_ApplyUpdatedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Updated>());
		m_ApplyDeletedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Deleted>());
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_TempQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		PatchTempReferencesJob patchTempReferencesJob = new PatchTempReferencesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Nodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		FixConnectedEdgesJob fixConnectedEdgesJob = new FixConnectedEdgesJob
		{
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Nodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		HandleTempEntitiesJob handleTempEntitiesJob = new HandleTempEntitiesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetNodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetEdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetLaneType = InternalCompilerInterface.GetComponentTypeHandle<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetEdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LandValueData = InternalCompilerInterface.GetComponentLookup<LandValue>(ref __TypeHandle.__Game_Net_LandValue_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NativeData = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedData = InternalCompilerInterface.GetComponentLookup<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnect>(ref __TypeHandle.__Game_Net_LocalConnect_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TramTrackData = InternalCompilerInterface.GetComponentLookup<TramTrack>(ref __TypeHandle.__Game_Net_TramTrack_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainTrackData = InternalCompilerInterface.GetComponentLookup<TrainTrack>(ref __TypeHandle.__Game_Net_TrainTrack_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterwayData = InternalCompilerInterface.GetComponentLookup<Waterway>(ref __TypeHandle.__Game_Net_Waterway_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionData = InternalCompilerInterface.GetComponentLookup<Game.Net.Pollution>(ref __TypeHandle.__Game_Net_Pollution_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficLightsData = InternalCompilerInterface.GetComponentLookup<TrafficLights>(ref __TypeHandle.__Game_Net_TrafficLights_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OrphanData = InternalCompilerInterface.GetComponentLookup<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AggregatedData = InternalCompilerInterface.GetComponentLookup<Aggregated>(ref __TypeHandle.__Game_Net_Aggregated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StandaloneData = InternalCompilerInterface.GetComponentLookup<Standalone>(ref __TypeHandle.__Game_Net_Standalone_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MarkerData = InternalCompilerInterface.GetComponentLookup<Marker>(ref __TypeHandle.__Game_Net_Marker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RecentData = InternalCompilerInterface.GetComponentLookup<Recent>(ref __TypeHandle.__Game_Tools_Recent_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubReplacements = InternalCompilerInterface.GetBufferLookup<SubReplacement>(ref __TypeHandle.__Game_Net_SubReplacement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_EconomyParameterData = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_ApplyCreatedTypes = m_ApplyCreatedTypes,
			m_ApplyUpdatedTypes = m_ApplyUpdatedTypes,
			m_ApplyDeletedTypes = m_ApplyDeletedTypes
		};
		EntityCommandBuffer val = m_ToolOutputBarrier.CreateCommandBuffer();
		handleTempEntitiesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		HandleTempEntitiesJob handleTempEntitiesJob2 = handleTempEntitiesJob;
		JobHandle val2 = JobChunkExtensions.Schedule<PatchTempReferencesJob>(patchTempReferencesJob, m_TempQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<FixConnectedEdgesJob>(fixConnectedEdgesJob, m_TempQuery, val2);
		JobHandle val4 = JobChunkExtensions.ScheduleParallel<HandleTempEntitiesJob>(handleTempEntitiesJob2, m_TempQuery, val2);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val3, val4);
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val4);
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
	public ApplyNetSystem()
	{
	}
}

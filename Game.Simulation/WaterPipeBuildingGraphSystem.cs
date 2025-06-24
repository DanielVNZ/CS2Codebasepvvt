using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class WaterPipeBuildingGraphSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateBuildingConnectionsJob : IJobChunk
	{
		private struct BuildingNodes
		{
			public BufferedEntity m_ProducerNode;

			public BufferedEntity m_ConsumerNode;
		}

		private struct MarkerNodeData
		{
			public Entity m_NetNode;

			public int m_FreshCapacity;

			public int m_SewageCapacity;
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubNet> m_SubNetType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentTypeHandle<WaterPipeBuildingConnection> m_BuildingConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.WaterPumpingStation> m_PumpingStationType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.SewageOutlet> m_SewageOutletType;

		[ReadOnly]
		public ComponentTypeHandle<WaterConsumer> m_ConsumerType;

		[ReadOnly]
		public ComponentTypeHandle<Destroyed> m_DestroyedType;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public ComponentLookup<Node> m_NetNodes;

		[ReadOnly]
		public ComponentLookup<Orphan> m_NetOrphans;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedNetEdges;

		[ReadOnly]
		public ComponentLookup<Deleted> m_Deleted;

		[ReadOnly]
		public ComponentLookup<Owner> m_Owners;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<WaterPipeConnectionData> m_WaterPipeConnectionDatas;

		[ReadOnly]
		public ComponentLookup<WaterPipeNodeConnection> m_WaterPipeNodeConnections;

		[ReadOnly]
		public ComponentLookup<WaterPipeValveConnection> m_WaterPipeValveConnections;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> m_FlowConnections;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> m_FlowEdges;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<Entity> m_UpdatedRoadEdges;

		public EntityArchetype m_NodeArchetype;

		public EntityArchetype m_EdgeArchetype;

		public Entity m_SourceNode;

		public Entity m_SinkNode;

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
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<Game.Net.SubNet> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.SubNet>(ref m_SubNetType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			NativeArray<WaterPipeBuildingConnection> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPipeBuildingConnection>(ref m_BuildingConnectionType);
			NativeArray<Building> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.WaterPumpingStation>(ref m_PumpingStationType);
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.SewageOutlet>(ref m_SewageOutletType);
			bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<WaterConsumer>(ref m_ConsumerType);
			bool flag4 = ((ArchetypeChunk)(ref chunk)).Has<Destroyed>(ref m_DestroyedType);
			NativeList<MarkerNodeData> result = default(NativeList<MarkerNodeData>);
			result._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
			WaterPipeNodeConnection waterPipeNodeConnection = default(WaterPipeNodeConnection);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				result.Clear();
				if (bufferAccessor.Length != 0)
				{
					FindMarkerNodes(bufferAccessor[i], result);
				}
				if (bufferAccessor2.Length != 0)
				{
					FindMarkerNodes(bufferAccessor2[i], result);
				}
				if (result.Length > 0)
				{
					BuildingNodes buildingNodes;
					if (!flag4 && (flag || flag2 || flag3))
					{
						WaterPipeBuildingConnection connection = ((nativeArray2.Length != 0) ? nativeArray2[i] : default(WaterPipeBuildingConnection));
						buildingNodes = CreateOrUpdateBuildingNodes(unfilteredChunkIndex, flag || flag2, flag3, ref connection);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<WaterPipeBuildingConnection>(unfilteredChunkIndex, val, connection);
					}
					else
					{
						buildingNodes = default(BuildingNodes);
						if (nativeArray2.Length != 0)
						{
							DeleteBuildingNodes(unfilteredChunkIndex, val, nativeArray2[i]);
						}
					}
					Entity roadEdge = nativeArray3[i].m_RoadEdge;
					if (roadEdge != Entity.Null && m_WaterPipeNodeConnections.TryGetComponent(roadEdge, ref waterPipeNodeConnection) && flag3)
					{
						m_UpdatedRoadEdges.Enqueue(roadEdge);
					}
					Enumerator<MarkerNodeData> enumerator = result.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							MarkerNodeData current = enumerator.Current;
							CreateOrUpdateMarkerNode(unfilteredChunkIndex, current, buildingNodes);
						}
					}
					finally
					{
						((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
					}
				}
				else if (nativeArray2.Length != 0)
				{
					DeleteBuildingNodes(unfilteredChunkIndex, val, nativeArray2[i]);
				}
			}
		}

		private void FindMarkerNodes(DynamicBuffer<InstalledUpgrade> upgrades, NativeList<MarkerNodeData> result)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Net.SubNet> subNets = default(DynamicBuffer<Game.Net.SubNet>);
			for (int i = 0; i < upgrades.Length; i++)
			{
				InstalledUpgrade installedUpgrade = upgrades[i];
				if (!BuildingUtils.CheckOption(installedUpgrade, BuildingOption.Inactive) && m_SubNets.TryGetBuffer(installedUpgrade.m_Upgrade, ref subNets))
				{
					FindMarkerNodes(subNets, result);
				}
			}
		}

		private void FindMarkerNodes(DynamicBuffer<Game.Net.SubNet> subNets, NativeList<MarkerNodeData> result)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = default(PrefabRef);
			WaterPipeConnectionData waterPipeConnectionData = default(WaterPipeConnectionData);
			for (int i = 0; i < subNets.Length; i++)
			{
				Entity subNet = subNets[i].m_SubNet;
				if (m_NetNodes.HasComponent(subNet) && (m_WaterPipeValveConnections.HasComponent(subNet) || IsOrphan(subNet)) && !m_Deleted.HasComponent(subNet) && m_PrefabRefs.TryGetComponent(subNet, ref prefabRef) && m_WaterPipeConnectionDatas.TryGetComponent(prefabRef.m_Prefab, ref waterPipeConnectionData))
				{
					MarkerNodeData markerNodeData = new MarkerNodeData
					{
						m_NetNode = subNet,
						m_FreshCapacity = waterPipeConnectionData.m_FreshCapacity,
						m_SewageCapacity = waterPipeConnectionData.m_SewageCapacity
					};
					result.Add(ref markerNodeData);
				}
			}
		}

		private bool IsOrphan(Entity netNode)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			if (m_NetOrphans.HasComponent(netNode))
			{
				return true;
			}
			DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
			if (m_ConnectedNetEdges.TryGetBuffer(netNode, ref val))
			{
				Enumerator<ConnectedEdge> enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ConnectedEdge current = enumerator.Current;
						if (m_Owners.HasComponent(current.m_Edge))
						{
							return false;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			return true;
		}

		private BuildingNodes CreateOrUpdateBuildingNodes(int jobIndex, bool isProducer, bool isConsumer, ref WaterPipeBuildingConnection connection)
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			BuildingNodes result = default(BuildingNodes);
			if (isProducer)
			{
				if (connection.m_ProducerEdge == Entity.Null)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_NodeArchetype);
					connection.m_ProducerEdge = CreateFlowEdge(jobIndex, m_SourceNode, val, 0, 0);
					result.m_ProducerNode = new BufferedEntity(val, stored: false);
				}
				else
				{
					result.m_ProducerNode = new BufferedEntity(connection.GetProducerNode(ref m_FlowEdges), stored: true);
				}
			}
			else if (connection.m_ProducerEdge != Entity.Null)
			{
				connection.m_ProducerEdge = Entity.Null;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, connection.GetProducerNode(ref m_FlowEdges));
			}
			if (isConsumer)
			{
				if (connection.m_ConsumerEdge == Entity.Null)
				{
					Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_NodeArchetype);
					connection.m_ConsumerEdge = CreateFlowEdge(jobIndex, val2, m_SinkNode, 0, 0);
					result.m_ConsumerNode = new BufferedEntity(val2, stored: false);
				}
				else
				{
					result.m_ConsumerNode = new BufferedEntity(connection.GetConsumerNode(ref m_FlowEdges), stored: true);
				}
			}
			else if (connection.m_ConsumerEdge != Entity.Null)
			{
				connection.m_ConsumerEdge = Entity.Null;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, connection.GetConsumerNode(ref m_FlowEdges));
			}
			if (isProducer && isConsumer)
			{
				CreateOrUpdateFlowEdge(jobIndex, result.m_ProducerNode, result.m_ConsumerNode, 1073741823, 1073741823);
			}
			return result;
		}

		private void DeleteBuildingNodes(int jobIndex, Entity building, WaterPipeBuildingConnection connection)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			WaterPipeGraphUtils.DeleteBuildingNodes(m_CommandBuffer, jobIndex, connection, ref m_FlowConnections, ref m_FlowEdges);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<WaterPipeBuildingConnection>(jobIndex, building);
		}

		private void CreateOrUpdateMarkerNode(int jobIndex, MarkerNodeData markerNodeData, BuildingNodes buildingNodes)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			WaterPipeNodeConnection waterPipeNodeConnection = default(WaterPipeNodeConnection);
			bool flag = m_WaterPipeNodeConnections.TryGetComponent(markerNodeData.m_NetNode, ref waterPipeNodeConnection);
			if (!flag)
			{
				waterPipeNodeConnection = new WaterPipeNodeConnection
				{
					m_WaterPipeNode = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_NodeArchetype)
				};
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<WaterPipeNodeConnection>(jobIndex, markerNodeData.m_NetNode, waterPipeNodeConnection);
			}
			BufferedEntity bufferedEntity = new BufferedEntity(waterPipeNodeConnection.m_WaterPipeNode, flag);
			WaterPipeValveConnection waterPipeValveConnection = default(WaterPipeValveConnection);
			bool flag2 = m_WaterPipeValveConnections.TryGetComponent(markerNodeData.m_NetNode, ref waterPipeValveConnection);
			if (!flag2)
			{
				waterPipeValveConnection = new WaterPipeValveConnection
				{
					m_ValveNode = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_NodeArchetype)
				};
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<WaterPipeValveConnection>(jobIndex, markerNodeData.m_NetNode, waterPipeValveConnection);
			}
			BufferedEntity bufferedEntity2 = new BufferedEntity(waterPipeValveConnection.m_ValveNode, flag2);
			CreateOrUpdateFlowEdge(jobIndex, bufferedEntity2, bufferedEntity, markerNodeData.m_FreshCapacity, markerNodeData.m_SewageCapacity);
			if (buildingNodes.m_ProducerNode.m_Value != Entity.Null)
			{
				CreateOrUpdateFlowEdge(jobIndex, buildingNodes.m_ProducerNode, bufferedEntity2, markerNodeData.m_FreshCapacity, markerNodeData.m_SewageCapacity);
			}
			if (buildingNodes.m_ConsumerNode.m_Value != Entity.Null)
			{
				CreateOrUpdateFlowEdge(jobIndex, bufferedEntity2, buildingNodes.m_ConsumerNode, markerNodeData.m_FreshCapacity, markerNodeData.m_SewageCapacity);
			}
			EnsureMarkerNodeEdgeConnections(jobIndex, markerNodeData, bufferedEntity);
		}

		private void EnsureMarkerNodeEdgeConnections(int jobIndex, MarkerNodeData markerNodeData, BufferedEntity markerNode)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<ConnectedEdge> enumerator = m_ConnectedNetEdges[markerNodeData.m_NetNode].GetEnumerator();
			try
			{
				WaterPipeNodeConnection waterPipeNodeConnection = default(WaterPipeNodeConnection);
				while (enumerator.MoveNext())
				{
					ConnectedEdge current = enumerator.Current;
					if (m_WaterPipeNodeConnections.TryGetComponent(current.m_Edge, ref waterPipeNodeConnection))
					{
						Entity waterPipeNode = waterPipeNodeConnection.m_WaterPipeNode;
						if (!markerNode.m_Stored || !WaterPipeGraphUtils.HasAnyFlowEdge(markerNode.m_Value, waterPipeNode, ref m_FlowConnections, ref m_FlowEdges))
						{
							CreateFlowEdge(jobIndex, markerNode.m_Value, waterPipeNodeConnection.m_WaterPipeNode, markerNodeData.m_FreshCapacity, markerNodeData.m_SewageCapacity);
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}

		private void CreateOrUpdateFlowEdge(int jobIndex, BufferedEntity startNode, BufferedEntity endNode, int freshCapacity, int sewageCapacity)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			if (startNode.m_Stored && endNode.m_Stored && WaterPipeGraphUtils.TryGetFlowEdge(startNode.m_Value, endNode.m_Value, ref m_FlowConnections, ref m_FlowEdges, out var entity, out var edge))
			{
				if (edge.m_FreshCapacity != freshCapacity || edge.m_SewageCapacity != sewageCapacity)
				{
					edge.m_FreshCapacity = freshCapacity;
					edge.m_SewageCapacity = sewageCapacity;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<WaterPipeEdge>(jobIndex, entity, edge);
				}
			}
			else
			{
				CreateFlowEdge(jobIndex, startNode.m_Value, endNode.m_Value, freshCapacity, sewageCapacity);
			}
		}

		private Entity CreateFlowEdge(int jobIndex, Entity startNode, Entity endNode, int freshCapacity, int sewageCapacity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return WaterPipeGraphUtils.CreateFlowEdge(m_CommandBuffer, jobIndex, m_EdgeArchetype, startNode, endNode, freshCapacity, sewageCapacity);
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
		public BufferTypeHandle<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterPipeBuildingConnection> __Game_Simulation_WaterPipeBuildingConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.WaterPumpingStation> __Game_Buildings_WaterPumpingStation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.SewageOutlet> __Game_Buildings_SewageOutlet_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Destroyed> __Game_Common_Destroyed_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Orphan> __Game_Net_Orphan_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeConnectionData> __Game_Prefabs_WaterPipeConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeNodeConnection> __Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeValveConnection> __Game_Simulation_WaterPipeValveConnection_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> __Game_Simulation_WaterPipeEdge_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_SubNet_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.SubNet>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Simulation_WaterPipeBuildingConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPipeBuildingConnection>(true);
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Buildings_WaterPumpingStation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.WaterPumpingStation>(true);
			__Game_Buildings_SewageOutlet_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.SewageOutlet>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterConsumer>(true);
			__Game_Common_Destroyed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Destroyed>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Orphan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Orphan>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_WaterPipeConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeConnectionData>(true);
			__Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeNodeConnection>(true);
			__Game_Simulation_WaterPipeValveConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeValveConnection>(true);
			__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(true);
			__Game_Simulation_WaterPipeEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeEdge>(true);
		}
	}

	private WaterPipeRoadConnectionGraphSystem m_WaterPipeRoadConnectionGraphSystem;

	private WaterPipeFlowSystem m_WaterPipeFlowSystem;

	private ModificationBarrier4B m_ModificationBarrier;

	private EntityQuery m_UpdatedBuildingQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_WaterPipeRoadConnectionGraphSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterPipeRoadConnectionGraphSystem>();
		m_WaterPipeFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterPipeFlowSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4B>();
		m_UpdatedBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[3]
		{
			CreatedUpdatedBuildingDesc((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Buildings.WaterPumpingStation>() }),
			CreatedUpdatedBuildingDesc((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Buildings.SewageOutlet>() }),
			CreatedUpdatedBuildingDesc((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WaterPipeBuildingConnection>() })
		});
		((ComponentSystemBase)this).RequireForUpdate(m_UpdatedBuildingQuery);
		static EntityQueryDesc CreatedUpdatedBuildingDesc(ComponentType[] all)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			EntityQueryDesc val = new EntityQueryDesc();
			val.All = all.Concat((IEnumerable<ComponentType>)(object)new ComponentType[1] { ComponentType.ReadOnly<Building>() }).ToArray();
			val.Any = (ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<Created>(),
				ComponentType.ReadOnly<Updated>()
			};
			val.None = (ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
				ComponentType.ReadOnly<Temp>()
			};
			return val;
		}
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
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		UpdateBuildingConnectionsJob updateBuildingConnectionsJob = new UpdateBuildingConnectionsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubNetType = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingConnectionType = InternalCompilerInterface.GetComponentTypeHandle<WaterPipeBuildingConnection>(ref __TypeHandle.__Game_Simulation_WaterPipeBuildingConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PumpingStationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.WaterPumpingStation>(ref __TypeHandle.__Game_Buildings_WaterPumpingStation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SewageOutletType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.SewageOutlet>(ref __TypeHandle.__Game_Buildings_SewageOutlet_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConsumerType = InternalCompilerInterface.GetComponentTypeHandle<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedType = InternalCompilerInterface.GetComponentTypeHandle<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetNodes = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetOrphans = InternalCompilerInterface.GetComponentLookup<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedNetEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Deleted = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Owners = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPipeConnectionDatas = InternalCompilerInterface.GetComponentLookup<WaterPipeConnectionData>(ref __TypeHandle.__Game_Prefabs_WaterPipeConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPipeNodeConnections = InternalCompilerInterface.GetComponentLookup<WaterPipeNodeConnection>(ref __TypeHandle.__Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPipeValveConnections = InternalCompilerInterface.GetComponentLookup<WaterPipeValveConnection>(ref __TypeHandle.__Game_Simulation_WaterPipeValveConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowConnections = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowEdges = InternalCompilerInterface.GetComponentLookup<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		updateBuildingConnectionsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		updateBuildingConnectionsJob.m_UpdatedRoadEdges = m_WaterPipeRoadConnectionGraphSystem.GetEdgeUpdateQueue(out var deps).AsParallelWriter();
		updateBuildingConnectionsJob.m_NodeArchetype = m_WaterPipeFlowSystem.nodeArchetype;
		updateBuildingConnectionsJob.m_EdgeArchetype = m_WaterPipeFlowSystem.edgeArchetype;
		updateBuildingConnectionsJob.m_SourceNode = m_WaterPipeFlowSystem.sourceNode;
		updateBuildingConnectionsJob.m_SinkNode = m_WaterPipeFlowSystem.sinkNode;
		UpdateBuildingConnectionsJob updateBuildingConnectionsJob2 = updateBuildingConnectionsJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<UpdateBuildingConnectionsJob>(updateBuildingConnectionsJob2, m_UpdatedBuildingQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_WaterPipeRoadConnectionGraphSystem.AddQueueWriter(((SystemBase)this).Dependency);
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
	public WaterPipeBuildingGraphSystem()
	{
	}
}

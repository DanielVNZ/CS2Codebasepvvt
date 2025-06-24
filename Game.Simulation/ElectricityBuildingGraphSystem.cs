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
public class ElectricityBuildingGraphSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateBuildingConnectionsJob : IJobChunk
	{
		private struct BuildingNodes
		{
			public BufferedEntity m_TransformerNode;

			public BufferedEntity m_ProducerNode;

			public BufferedEntity m_ConsumerNode;

			public BufferedEntity m_ChargeNode;

			public BufferedEntity m_DischargeNode;
		}

		private struct MarkerNodeData
		{
			public Entity m_NetNode;

			public int m_Capacity;

			public FlowDirection m_Direction;

			public Game.Prefabs.ElectricityConnection.Voltage m_Voltage;
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubNet> m_SubNetType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityBuildingConnection> m_BuildingConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Transformer> m_TransformerType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityProducer> m_ProducerType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityConsumer> m_ConsumerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Battery> m_BatteryType;

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
		public ComponentLookup<ElectricityConnectionData> m_ElectricityConnectionDatas;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> m_ElectricityNodeConnections;

		[ReadOnly]
		public ComponentLookup<ElectricityValveConnection> m_ElectricityValveConnections;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> m_FlowConnections;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> m_FlowEdges;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<Entity> m_UpdatedRoadEdges;

		public EntityArchetype m_NodeArchetype;

		public EntityArchetype m_ChargeNodeArchetype;

		public EntityArchetype m_DischargeNodeArchetype;

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
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<Game.Net.SubNet> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.SubNet>(ref m_SubNetType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			NativeArray<ElectricityBuildingConnection> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityBuildingConnection>(ref m_BuildingConnectionType);
			NativeArray<Building> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Transformer>(ref m_TransformerType);
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<ElectricityProducer>(ref m_ProducerType);
			bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<ElectricityConsumer>(ref m_ConsumerType);
			bool flag4 = ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Battery>(ref m_BatteryType);
			bool flag5 = ((ArchetypeChunk)(ref chunk)).Has<Destroyed>(ref m_DestroyedType);
			NativeList<MarkerNodeData> result = default(NativeList<MarkerNodeData>);
			result._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
			ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
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
					if (!flag5 && (flag || flag2 || flag3 || flag4))
					{
						ElectricityBuildingConnection connection = ((nativeArray2.Length != 0) ? nativeArray2[i] : default(ElectricityBuildingConnection));
						buildingNodes = CreateOrUpdateBuildingNodes(unfilteredChunkIndex, flag, flag2, flag3, flag4, ref connection);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<ElectricityBuildingConnection>(unfilteredChunkIndex, val, connection);
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
					if (roadEdge != Entity.Null && m_ElectricityNodeConnections.TryGetComponent(roadEdge, ref electricityNodeConnection) && flag3)
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
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = default(PrefabRef);
			ElectricityConnectionData electricityConnectionData = default(ElectricityConnectionData);
			for (int i = 0; i < subNets.Length; i++)
			{
				Entity subNet = subNets[i].m_SubNet;
				if (m_NetNodes.HasComponent(subNet) && (m_ElectricityValveConnections.HasComponent(subNet) || IsOrphan(subNet)) && !m_Deleted.HasComponent(subNet) && m_PrefabRefs.TryGetComponent(subNet, ref prefabRef) && m_ElectricityConnectionDatas.TryGetComponent(prefabRef.m_Prefab, ref electricityConnectionData))
				{
					MarkerNodeData markerNodeData = new MarkerNodeData
					{
						m_NetNode = subNet,
						m_Capacity = electricityConnectionData.m_Capacity,
						m_Direction = electricityConnectionData.m_Direction,
						m_Voltage = electricityConnectionData.m_Voltage
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

		private BuildingNodes CreateOrUpdateBuildingNodes(int jobIndex, bool isTransformer, bool isProducer, bool isConsumer, bool isBattery, ref ElectricityBuildingConnection connection)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			BuildingNodes result = default(BuildingNodes);
			if (isTransformer)
			{
				if (connection.m_TransformerNode == Entity.Null)
				{
					connection.m_TransformerNode = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_NodeArchetype);
					result.m_TransformerNode = new BufferedEntity(connection.m_TransformerNode, stored: false);
				}
				else
				{
					result.m_TransformerNode = new BufferedEntity(connection.m_TransformerNode, stored: true);
				}
			}
			else if (connection.m_TransformerNode != Entity.Null)
			{
				connection.m_TransformerNode = Entity.Null;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, connection.m_TransformerNode);
			}
			if (isProducer)
			{
				if (connection.m_ProducerEdge == Entity.Null)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_NodeArchetype);
					connection.m_ProducerEdge = CreateFlowEdge(jobIndex, m_SourceNode, val, FlowDirection.Forward, 0);
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
					connection.m_ConsumerEdge = CreateFlowEdge(jobIndex, val2, m_SinkNode, FlowDirection.Forward, 0);
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
			if (isBattery)
			{
				if (connection.m_ChargeEdge == Entity.Null)
				{
					Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_ChargeNodeArchetype);
					connection.m_ChargeEdge = CreateFlowEdge(jobIndex, val3, m_SinkNode, FlowDirection.None, 0);
					result.m_ChargeNode = new BufferedEntity(val3, stored: false);
				}
				else
				{
					result.m_ChargeNode = new BufferedEntity(connection.GetChargeNode(ref m_FlowEdges), stored: true);
				}
				if (connection.m_DischargeEdge == Entity.Null)
				{
					Entity val4 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_DischargeNodeArchetype);
					connection.m_DischargeEdge = CreateFlowEdge(jobIndex, m_SourceNode, val4, FlowDirection.None, 0);
					result.m_DischargeNode = new BufferedEntity(val4, stored: false);
				}
				else
				{
					result.m_DischargeNode = new BufferedEntity(connection.GetDischargeNode(ref m_FlowEdges), stored: true);
				}
			}
			else
			{
				if (connection.m_ChargeEdge != Entity.Null)
				{
					connection.m_ChargeEdge = Entity.Null;
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, connection.GetChargeNode(ref m_FlowEdges));
				}
				if (connection.m_DischargeEdge != Entity.Null)
				{
					connection.m_DischargeEdge = Entity.Null;
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, connection.GetDischargeNode(ref m_FlowEdges));
				}
			}
			if (isProducer && isConsumer)
			{
				CreateOrUpdateFlowEdge(jobIndex, result.m_ProducerNode, result.m_ConsumerNode, FlowDirection.Forward, 1073741823);
			}
			if (isProducer && isBattery)
			{
				CreateOrUpdateFlowEdge(jobIndex, result.m_ProducerNode, result.m_ChargeNode, FlowDirection.None, 1073741823);
			}
			if (isConsumer && isBattery)
			{
				CreateOrUpdateFlowEdge(jobIndex, result.m_DischargeNode, result.m_ConsumerNode, FlowDirection.None, 1073741823);
			}
			return result;
		}

		private void DeleteBuildingNodes(int jobIndex, Entity building, ElectricityBuildingConnection connection)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			ElectricityGraphUtils.DeleteBuildingNodes(m_CommandBuffer, jobIndex, connection, ref m_FlowConnections, ref m_FlowEdges);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<ElectricityBuildingConnection>(jobIndex, building);
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
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
			bool flag = m_ElectricityNodeConnections.TryGetComponent(markerNodeData.m_NetNode, ref electricityNodeConnection);
			if (!flag)
			{
				electricityNodeConnection = new ElectricityNodeConnection
				{
					m_ElectricityNode = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_NodeArchetype)
				};
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<ElectricityNodeConnection>(jobIndex, markerNodeData.m_NetNode, electricityNodeConnection);
			}
			BufferedEntity bufferedEntity = new BufferedEntity(electricityNodeConnection.m_ElectricityNode, flag);
			ElectricityValveConnection electricityValveConnection = default(ElectricityValveConnection);
			bool flag2 = m_ElectricityValveConnections.TryGetComponent(markerNodeData.m_NetNode, ref electricityValveConnection);
			if (!flag2)
			{
				electricityValveConnection = new ElectricityValveConnection
				{
					m_ValveNode = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_NodeArchetype)
				};
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<ElectricityValveConnection>(jobIndex, markerNodeData.m_NetNode, electricityValveConnection);
			}
			BufferedEntity bufferedEntity2 = new BufferedEntity(electricityValveConnection.m_ValveNode, flag2);
			CreateOrUpdateFlowEdge(jobIndex, bufferedEntity2, bufferedEntity, markerNodeData.m_Direction, markerNodeData.m_Capacity);
			if (buildingNodes.m_TransformerNode.m_Value != Entity.Null)
			{
				CreateOrUpdateFlowEdge(jobIndex, buildingNodes.m_TransformerNode, bufferedEntity2, markerNodeData.m_Direction, markerNodeData.m_Capacity);
			}
			if (buildingNodes.m_ProducerNode.m_Value != Entity.Null)
			{
				CreateOrUpdateFlowEdge(jobIndex, buildingNodes.m_ProducerNode, bufferedEntity2, FlowDirection.Forward, markerNodeData.m_Capacity);
			}
			if (buildingNodes.m_ConsumerNode.m_Value != Entity.Null)
			{
				CreateOrUpdateFlowEdge(jobIndex, bufferedEntity2, buildingNodes.m_ConsumerNode, FlowDirection.Forward, markerNodeData.m_Capacity);
			}
			if (buildingNodes.m_ChargeNode.m_Value != Entity.Null)
			{
				CreateOrUpdateFlowEdge(jobIndex, bufferedEntity2, buildingNodes.m_ChargeNode, FlowDirection.None, markerNodeData.m_Capacity);
			}
			if (buildingNodes.m_DischargeNode.m_Value != Entity.Null)
			{
				CreateOrUpdateFlowEdge(jobIndex, buildingNodes.m_DischargeNode, bufferedEntity2, FlowDirection.None, markerNodeData.m_Capacity);
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
				ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
				while (enumerator.MoveNext())
				{
					ConnectedEdge current = enumerator.Current;
					if (m_ElectricityNodeConnections.TryGetComponent(current.m_Edge, ref electricityNodeConnection))
					{
						Entity electricityNode = electricityNodeConnection.m_ElectricityNode;
						if (!markerNode.m_Stored || !ElectricityGraphUtils.HasAnyFlowEdge(markerNode.m_Value, electricityNode, ref m_FlowConnections, ref m_FlowEdges))
						{
							CreateFlowEdge(jobIndex, markerNode.m_Value, electricityNodeConnection.m_ElectricityNode, markerNodeData.m_Direction, markerNodeData.m_Capacity);
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}

		private void CreateOrUpdateFlowEdge(int jobIndex, BufferedEntity startNode, BufferedEntity endNode, FlowDirection direction, int capacity)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			if (startNode.m_Stored && endNode.m_Stored && ElectricityGraphUtils.TryGetFlowEdge(startNode.m_Value, endNode.m_Value, ref m_FlowConnections, ref m_FlowEdges, out var entity, out var edge))
			{
				if (edge.direction != direction || edge.m_Capacity != capacity)
				{
					edge.direction = direction;
					edge.m_Capacity = capacity;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ElectricityFlowEdge>(jobIndex, entity, edge);
				}
			}
			else
			{
				CreateFlowEdge(jobIndex, startNode.m_Value, endNode.m_Value, direction, capacity);
			}
		}

		private Entity CreateFlowEdge(int jobIndex, Entity startNode, Entity endNode, FlowDirection direction, int capacity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return ElectricityGraphUtils.CreateFlowEdge(m_CommandBuffer, jobIndex, m_EdgeArchetype, startNode, endNode, direction, capacity);
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
		public ComponentTypeHandle<ElectricityBuildingConnection> __Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Transformer> __Game_Buildings_Transformer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityProducer> __Game_Buildings_ElectricityProducer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Battery> __Game_Buildings_Battery_RO_ComponentTypeHandle;

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
		public ComponentLookup<ElectricityConnectionData> __Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> __Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityValveConnection> __Game_Simulation_ElectricityValveConnection_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_SubNet_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.SubNet>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityBuildingConnection>(true);
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Buildings_Transformer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Transformer>(true);
			__Game_Buildings_ElectricityProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityProducer>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityConsumer>(true);
			__Game_Buildings_Battery_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Battery>(true);
			__Game_Common_Destroyed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Destroyed>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Orphan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Orphan>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConnectionData>(true);
			__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityNodeConnection>(true);
			__Game_Simulation_ElectricityValveConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityValveConnection>(true);
			__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(true);
			__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(true);
		}
	}

	private ElectricityRoadConnectionGraphSystem m_ElectricityRoadConnectionGraphSystem;

	private ElectricityFlowSystem m_ElectricityFlowSystem;

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
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ElectricityRoadConnectionGraphSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityRoadConnectionGraphSystem>();
		m_ElectricityFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityFlowSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4B>();
		m_UpdatedBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[6]
		{
			CreatedUpdatedBuildingDesc((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ElectricityProducer>() }),
			CreatedUpdatedBuildingDesc((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<ElectricityConsumer>(),
				ComponentType.ReadOnly<Game.Net.SubNet>()
			}),
			CreatedUpdatedBuildingDesc((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<ElectricityConsumer>(),
				ComponentType.ReadOnly<InstalledUpgrade>()
			}),
			CreatedUpdatedBuildingDesc((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Buildings.Battery>() }),
			CreatedUpdatedBuildingDesc((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Buildings.Transformer>() }),
			CreatedUpdatedBuildingDesc((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ElectricityBuildingConnection>() })
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
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		UpdateBuildingConnectionsJob updateBuildingConnectionsJob = new UpdateBuildingConnectionsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubNetType = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingConnectionType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityBuildingConnection>(ref __TypeHandle.__Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Transformer>(ref __TypeHandle.__Game_Buildings_Transformer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ProducerType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityProducer>(ref __TypeHandle.__Game_Buildings_ElectricityProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConsumerType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BatteryType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Battery>(ref __TypeHandle.__Game_Buildings_Battery_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedType = InternalCompilerInterface.GetComponentTypeHandle<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetNodes = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetOrphans = InternalCompilerInterface.GetComponentLookup<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedNetEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Deleted = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Owners = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConnectionDatas = InternalCompilerInterface.GetComponentLookup<ElectricityConnectionData>(ref __TypeHandle.__Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityNodeConnections = InternalCompilerInterface.GetComponentLookup<ElectricityNodeConnection>(ref __TypeHandle.__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityValveConnections = InternalCompilerInterface.GetComponentLookup<ElectricityValveConnection>(ref __TypeHandle.__Game_Simulation_ElectricityValveConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowConnections = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		updateBuildingConnectionsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		updateBuildingConnectionsJob.m_UpdatedRoadEdges = m_ElectricityRoadConnectionGraphSystem.GetEdgeUpdateQueue(out var deps).AsParallelWriter();
		updateBuildingConnectionsJob.m_NodeArchetype = m_ElectricityFlowSystem.nodeArchetype;
		updateBuildingConnectionsJob.m_ChargeNodeArchetype = m_ElectricityFlowSystem.chargeNodeArchetype;
		updateBuildingConnectionsJob.m_DischargeNodeArchetype = m_ElectricityFlowSystem.dischargeNodeArchetype;
		updateBuildingConnectionsJob.m_EdgeArchetype = m_ElectricityFlowSystem.edgeArchetype;
		updateBuildingConnectionsJob.m_SourceNode = m_ElectricityFlowSystem.sourceNode;
		updateBuildingConnectionsJob.m_SinkNode = m_ElectricityFlowSystem.sinkNode;
		UpdateBuildingConnectionsJob updateBuildingConnectionsJob2 = updateBuildingConnectionsJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<UpdateBuildingConnectionsJob>(updateBuildingConnectionsJob2, m_UpdatedBuildingQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_ElectricityRoadConnectionGraphSystem.AddQueueWriter(((SystemBase)this).Dependency);
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
	public ElectricityBuildingGraphSystem()
	{
	}
}

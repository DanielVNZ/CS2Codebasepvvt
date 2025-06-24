using System;
using System.Runtime.CompilerServices;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ElectricityEdgeGraphSystem : GameSystemBase
{
	[BurstCompile]
	private struct CreateEdgeConnectionsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_NetEdgeType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_ConnectedNetNodes;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedNetEdges;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> m_FlowConnections;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> m_FlowEdges;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<ElectricityConnectionData> m_ElectricityConnectionDatas;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> m_ElectricityNodeConnections;

		public EntityCommandBuffer m_CommandBuffer;

		public NativeParallelHashMap<Entity, Entity> m_NodeMap;

		public EntityArchetype m_NodeArchetype;

		public EntityArchetype m_EdgeArchetype;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Edge> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_NetEdgeType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				Entity start = nativeArray2[i].m_Start;
				Entity end = nativeArray2[i].m_End;
				BufferedEntity orCreateNetNodeConnection = GetOrCreateNetNodeConnection(start);
				BufferedEntity orCreateNetNodeConnection2 = GetOrCreateNetNodeConnection(end);
				Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_NodeArchetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ElectricityNodeConnection>(val, new ElectricityNodeConnection
				{
					m_ElectricityNode = val2
				});
				PrefabRef prefabRef = nativeArray3[i];
				ElectricityConnectionData connectionData = m_ElectricityConnectionDatas[prefabRef.m_Prefab];
				CreateFlowEdge(orCreateNetNodeConnection.m_Value, val2, connectionData);
				CreateFlowEdge(val2, orCreateNetNodeConnection2.m_Value, connectionData);
				CreateEdgeMiddleNodeConnections(val, val2);
				EnsureNodeEdgeConnections(start, orCreateNetNodeConnection, connectionData);
				EnsureNodeEdgeConnections(end, orCreateNetNodeConnection2, connectionData);
			}
		}

		private void CreateEdgeMiddleNodeConnections(Entity netEdge, Entity flowMiddleNode)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedNode> val = default(DynamicBuffer<ConnectedNode>);
			if (!m_ConnectedNetNodes.TryGetBuffer(netEdge, ref val))
			{
				return;
			}
			Enumerator<ConnectedNode> enumerator = val.GetEnumerator();
			try
			{
				PrefabRef prefabRef = default(PrefabRef);
				ElectricityConnectionData connectionData = default(ElectricityConnectionData);
				while (enumerator.MoveNext())
				{
					ConnectedNode current = enumerator.Current;
					if (m_PrefabRefs.TryGetComponent(current.m_Node, ref prefabRef) && m_ElectricityConnectionDatas.TryGetComponent(prefabRef.m_Prefab, ref connectionData))
					{
						CreateFlowEdge(GetOrCreateNetNodeConnection(current.m_Node).m_Value, flowMiddleNode, connectionData);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}

		private void EnsureNodeEdgeConnections(Entity netNode, BufferedEntity flowNode, ElectricityConnectionData connectionData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<ConnectedEdge> enumerator = m_ConnectedNetEdges[netNode].GetEnumerator();
			try
			{
				ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
				while (enumerator.MoveNext())
				{
					ConnectedEdge current = enumerator.Current;
					if (m_ElectricityNodeConnections.TryGetComponent(current.m_Edge, ref electricityNodeConnection))
					{
						Entity electricityNode = electricityNodeConnection.m_ElectricityNode;
						if (!flowNode.m_Stored || !ElectricityGraphUtils.HasAnyFlowEdge(flowNode.m_Value, electricityNode, ref m_FlowConnections, ref m_FlowEdges))
						{
							CreateFlowEdge(flowNode.m_Value, electricityNodeConnection.m_ElectricityNode, connectionData);
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}

		private BufferedEntity GetOrCreateNetNodeConnection(Entity netNode)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
			if (m_ElectricityNodeConnections.TryGetComponent(netNode, ref electricityNodeConnection))
			{
				return new BufferedEntity(electricityNodeConnection.m_ElectricityNode, stored: true);
			}
			Entity value = default(Entity);
			if (m_NodeMap.TryGetValue(netNode, ref value))
			{
				return new BufferedEntity(value, stored: false);
			}
			value = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_NodeArchetype);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ElectricityNodeConnection>(netNode, new ElectricityNodeConnection
			{
				m_ElectricityNode = value
			});
			m_NodeMap.Add(netNode, value);
			return new BufferedEntity(value, stored: false);
		}

		private void CreateFlowEdge(Entity startNode, Entity endNode, ElectricityConnectionData connectionData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			ElectricityGraphUtils.CreateFlowEdge(m_CommandBuffer, m_EdgeArchetype, startNode, endNode, connectionData.m_Direction, connectionData.m_Capacity);
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
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConnectionData> __Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> __Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(true);
			__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConnectionData>(true);
			__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityNodeConnection>(true);
		}
	}

	private ElectricityFlowSystem m_ElectricityFlowSystem;

	private ModificationBarrier2B m_ModificationBarrier;

	private EntityQuery m_CreatedEdgeQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ElectricityFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityFlowSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier2B>();
		m_CreatedEdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Net.ElectricityConnection>(),
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CreatedEdgeQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashMap<Entity, Entity> nodeMap = default(NativeParallelHashMap<Entity, Entity>);
		nodeMap._002Ector(32, AllocatorHandle.op_Implicit((Allocator)3));
		CreateEdgeConnectionsJob createEdgeConnectionsJob = new CreateEdgeConnectionsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetEdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedNetNodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedNetEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowConnections = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConnectionDatas = InternalCompilerInterface.GetComponentLookup<ElectricityConnectionData>(ref __TypeHandle.__Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityNodeConnections = InternalCompilerInterface.GetComponentLookup<ElectricityNodeConnection>(ref __TypeHandle.__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer(),
			m_NodeMap = nodeMap,
			m_NodeArchetype = m_ElectricityFlowSystem.nodeArchetype,
			m_EdgeArchetype = m_ElectricityFlowSystem.edgeArchetype
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<CreateEdgeConnectionsJob>(createEdgeConnectionsJob, m_CreatedEdgeQuery, ((SystemBase)this).Dependency);
		nodeMap.Dispose(((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public ElectricityEdgeGraphSystem()
	{
	}
}

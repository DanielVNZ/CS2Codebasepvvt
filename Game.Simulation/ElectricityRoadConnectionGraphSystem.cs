using System;
using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ElectricityRoadConnectionGraphSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateRoadConnectionsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<RoadConnectionUpdated> m_RoadConnectionUpdatedType;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_ElectricityConsumers;

		[ReadOnly]
		public ComponentLookup<Deleted> m_Deleted;

		public ParallelWriter<Entity> m_UpdatedEdges;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<RoadConnectionUpdated> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RoadConnectionUpdated>(ref m_RoadConnectionUpdatedType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				RoadConnectionUpdated roadConnectionUpdated = nativeArray[i];
				if (m_ElectricityConsumers.HasComponent(roadConnectionUpdated.m_Building))
				{
					if (roadConnectionUpdated.m_Old != Entity.Null && !m_Deleted.HasComponent(roadConnectionUpdated.m_Old))
					{
						m_UpdatedEdges.Enqueue(roadConnectionUpdated.m_Old);
					}
					if (roadConnectionUpdated.m_New != Entity.Null)
					{
						m_UpdatedEdges.Enqueue(roadConnectionUpdated.m_New);
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
	private struct UpdateRoadEdgesJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_ElectricityConsumers;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> m_NodeConnections;

		[ReadOnly]
		public ComponentLookup<ElectricityBuildingConnection> m_BuildingConnections;

		[ReadOnly]
		public BufferLookup<ConnectedBuilding> m_ConnectedBuildings;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> m_FlowConnections;

		public ComponentLookup<ElectricityFlowEdge> m_FlowEdges;

		public EntityCommandBuffer m_CommandBuffer;

		public NativeQueue<Entity> m_UpdatedEdges;

		public Entity m_SinkNode;

		public EntityArchetype m_EdgeArchetype;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashSet<Entity> val = default(NativeParallelHashSet<Entity>);
			val._002Ector(m_UpdatedEdges.Count, AllocatorHandle.op_Implicit((Allocator)2));
			Entity val2 = default(Entity);
			ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
			while (m_UpdatedEdges.TryDequeue(ref val2))
			{
				if (val.Add(val2) && m_NodeConnections.TryGetComponent(val2, ref electricityNodeConnection))
				{
					if (HasConsumersWithoutBuildingSinkConnection(val2, out var consumption))
					{
						CreateOrUpdateRoadEdgeSinkConnection(electricityNodeConnection.m_ElectricityNode, consumption);
					}
					else
					{
						ClearRoadEdgeSinkConnection(electricityNodeConnection.m_ElectricityNode);
					}
				}
			}
		}

		private bool HasConsumersWithoutBuildingSinkConnection(Entity roadEdge, out int consumption)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			consumption = 0;
			DynamicBuffer<ConnectedBuilding> val = default(DynamicBuffer<ConnectedBuilding>);
			if (m_ConnectedBuildings.TryGetBuffer(roadEdge, ref val))
			{
				Enumerator<ConnectedBuilding> enumerator = val.GetEnumerator();
				try
				{
					ElectricityConsumer electricityConsumer = default(ElectricityConsumer);
					while (enumerator.MoveNext())
					{
						ConnectedBuilding current = enumerator.Current;
						if (m_ElectricityConsumers.TryGetComponent(current.m_Building, ref electricityConsumer) && !m_BuildingConnections.HasComponent(current.m_Building))
						{
							result = true;
							consumption += electricityConsumer.m_WantedConsumption;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			return result;
		}

		private void CreateOrUpdateRoadEdgeSinkConnection(Entity roadEdgeFlowNode, int capacity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (!ElectricityGraphUtils.TrySetFlowEdge(roadEdgeFlowNode, m_SinkNode, FlowDirection.Forward, capacity, ref m_FlowConnections, ref m_FlowEdges))
			{
				ElectricityGraphUtils.CreateFlowEdge(m_CommandBuffer, m_EdgeArchetype, roadEdgeFlowNode, m_SinkNode, FlowDirection.Forward, capacity);
			}
		}

		private void ClearRoadEdgeSinkConnection(Entity roadEdgeFlowNode)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (ElectricityGraphUtils.TryGetFlowEdge(roadEdgeFlowNode, m_SinkNode, ref m_FlowConnections, ref m_FlowEdges, out Entity entity))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(entity);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<RoadConnectionUpdated> __Game_Buildings_RoadConnectionUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> __Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityBuildingConnection> __Game_Simulation_ElectricityBuildingConnection_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedBuilding> __Game_Buildings_ConnectedBuilding_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferLookup;

		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup;

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
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			__Game_Buildings_RoadConnectionUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RoadConnectionUpdated>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConsumer>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityNodeConnection>(true);
			__Game_Simulation_ElectricityBuildingConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityBuildingConnection>(true);
			__Game_Buildings_ConnectedBuilding_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedBuilding>(true);
			__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(true);
			__Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(false);
		}
	}

	private ElectricityFlowSystem m_ElectricityFlowSystem;

	private ModificationBarrier5 m_ModificationBarrier;

	private EntityQuery m_EventQuery;

	private NativeQueue<Entity> m_UpdatedEdges;

	private JobHandle m_WriteDependencies;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ElectricityFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityFlowSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<RoadConnectionUpdated>() });
		m_UpdatedEdges = new NativeQueue<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_UpdatedEdges.Dispose();
		base.OnDestroy();
	}

	public NativeQueue<Entity> GetEdgeUpdateQueue(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_UpdatedEdges;
	}

	public void AddQueueWriter(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_WriteDependencies = JobHandle.CombineDependencies(m_WriteDependencies, handle);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_WriteDependencies);
		if (!((EntityQuery)(ref m_EventQuery)).IsEmptyIgnoreFilter)
		{
			UpdateRoadConnectionsJob updateRoadConnectionsJob = new UpdateRoadConnectionsJob
			{
				m_RoadConnectionUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<RoadConnectionUpdated>(ref __TypeHandle.__Game_Buildings_RoadConnectionUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityConsumers = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Deleted = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedEdges = m_UpdatedEdges.AsParallelWriter()
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<UpdateRoadConnectionsJob>(updateRoadConnectionsJob, m_EventQuery, ((SystemBase)this).Dependency);
		}
		UpdateRoadEdgesJob updateRoadEdgesJob = new UpdateRoadEdgesJob
		{
			m_ElectricityConsumers = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeConnections = InternalCompilerInterface.GetComponentLookup<ElectricityNodeConnection>(ref __TypeHandle.__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingConnections = InternalCompilerInterface.GetComponentLookup<ElectricityBuildingConnection>(ref __TypeHandle.__Game_Simulation_ElectricityBuildingConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedBuildings = InternalCompilerInterface.GetBufferLookup<ConnectedBuilding>(ref __TypeHandle.__Game_Buildings_ConnectedBuilding_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowConnections = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer(),
			m_UpdatedEdges = m_UpdatedEdges,
			m_SinkNode = m_ElectricityFlowSystem.sinkNode,
			m_EdgeArchetype = m_ElectricityFlowSystem.edgeArchetype
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<UpdateRoadEdgesJob>(updateRoadEdgesJob, ((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_WriteDependencies = ((SystemBase)this).Dependency;
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
	public ElectricityRoadConnectionGraphSystem()
	{
	}
}

using System.Runtime.CompilerServices;
using Game.Common;
using Game.Objects;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class WaterPipeOutsideConnectionGraphSystem : GameSystemBase
{
	[BurstCompile]
	private struct CreateOutsideConnectionsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentLookup<WaterPipeNodeConnection> m_WaterPipeNodeConnections;

		public ParallelWriter m_CommandBuffer;

		public EntityArchetype m_EdgeArchetype;

		public Entity m_SourceNode;

		public Entity m_SinkNode;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Owner> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			WaterPipeNodeConnection waterPipeNodeConnection = default(WaterPipeNodeConnection);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				if (m_WaterPipeNodeConnections.TryGetComponent(nativeArray[i].m_Owner, ref waterPipeNodeConnection))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TradeNode>(unfilteredChunkIndex, waterPipeNodeConnection.m_WaterPipeNode);
					CreateOutsideFlowEdge(unfilteredChunkIndex, m_SourceNode, waterPipeNodeConnection.m_WaterPipeNode);
					CreateOutsideFlowEdge(unfilteredChunkIndex, waterPipeNodeConnection.m_WaterPipeNode, m_SinkNode);
				}
			}
		}

		private void CreateOutsideFlowEdge(int jobIndex, Entity startNode, Entity endNode)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			WaterPipeGraphUtils.CreateFlowEdge(m_CommandBuffer, jobIndex, m_EdgeArchetype, startNode, endNode, 0, 0);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<WaterPipeNodeConnection> __Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeNodeConnection>(true);
		}
	}

	private WaterPipeFlowSystem m_WaterPipeFlowSystem;

	private ModificationBarrier3 m_ModificationBarrier;

	private EntityQuery m_CreatedConnectionQuery;

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
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_WaterPipeFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterPipeFlowSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier3>();
		m_CreatedConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<WaterPipeOutsideConnection>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CreatedConnectionQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		CreateOutsideConnectionsJob createOutsideConnectionsJob = new CreateOutsideConnectionsJob
		{
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPipeNodeConnections = InternalCompilerInterface.GetComponentLookup<WaterPipeNodeConnection>(ref __TypeHandle.__Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		createOutsideConnectionsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		createOutsideConnectionsJob.m_EdgeArchetype = m_WaterPipeFlowSystem.edgeArchetype;
		createOutsideConnectionsJob.m_SourceNode = m_WaterPipeFlowSystem.sourceNode;
		createOutsideConnectionsJob.m_SinkNode = m_WaterPipeFlowSystem.sinkNode;
		CreateOutsideConnectionsJob createOutsideConnectionsJob2 = createOutsideConnectionsJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CreateOutsideConnectionsJob>(createOutsideConnectionsJob2, m_CreatedConnectionQuery, ((SystemBase)this).Dependency);
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
	public WaterPipeOutsideConnectionGraphSystem()
	{
	}
}

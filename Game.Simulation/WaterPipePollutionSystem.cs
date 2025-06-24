using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class WaterPipePollutionSystem : GameSystemBase
{
	[BurstCompile]
	public struct NodePollutionJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedFlowEdge> m_FlowConnectionType;

		public ComponentTypeHandle<WaterPipeNode> m_NodeType;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> m_FlowEdges;

		public float m_StaleWaterPipePurification;

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
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<ConnectedFlowEdge> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedFlowEdge>(ref m_FlowConnectionType);
			NativeArray<WaterPipeNode> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPipeNode>(ref m_NodeType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				ref WaterPipeNode reference = ref CollectionUtils.ElementAt<WaterPipeNode>(nativeArray2, i);
				DynamicBuffer<ConnectedFlowEdge> val2 = bufferAccessor[i];
				int num = 0;
				float num2 = 0f;
				for (int j = 0; j < val2.Length; j++)
				{
					WaterPipeEdge waterPipeEdge = m_FlowEdges[val2[j].m_Edge];
					int num3 = ((waterPipeEdge.m_Start == val) ? (-waterPipeEdge.m_FreshFlow) : waterPipeEdge.m_FreshFlow);
					if (num3 > 0)
					{
						num += num3;
						num2 += waterPipeEdge.m_FreshPollution * (float)num3;
					}
				}
				if (num > 0)
				{
					reference.m_FreshPollution = num2 / (float)num;
				}
				else
				{
					reference.m_FreshPollution = math.max(0f, reference.m_FreshPollution - m_StaleWaterPipePurification);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	public struct EdgePollutionJob : IJobChunk
	{
		public ComponentTypeHandle<WaterPipeEdge> m_FlowEdgeType;

		[ReadOnly]
		public ComponentLookup<WaterPipeNode> m_FlowNodes;

		public Entity m_SourceNode;

		public bool m_Purify;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<WaterPipeEdge> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPipeEdge>(ref m_FlowEdgeType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				ref WaterPipeEdge reference = ref CollectionUtils.ElementAt<WaterPipeEdge>(nativeArray, i);
				WaterPipeNode waterPipeNode = m_FlowNodes[reference.m_Start];
				WaterPipeNode waterPipeNode2 = m_FlowNodes[reference.m_End];
				if (reference.m_Start != m_SourceNode)
				{
					float num = ((reference.m_FreshFlow > 0) ? waterPipeNode.m_FreshPollution : ((reference.m_FreshFlow >= 0) ? ((waterPipeNode.m_FreshPollution + waterPipeNode2.m_FreshPollution) / 2f) : waterPipeNode2.m_FreshPollution));
					if (!m_Purify || num == 0f)
					{
						reference.m_FreshPollution = num;
					}
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
		public BufferTypeHandle<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle;

		public ComponentTypeHandle<WaterPipeNode> __Game_Simulation_WaterPipeNode_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> __Game_Simulation_WaterPipeEdge_RO_ComponentLookup;

		public ComponentTypeHandle<WaterPipeEdge> __Game_Simulation_WaterPipeEdge_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<WaterPipeNode> __Game_Simulation_WaterPipeNode_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedFlowEdge>(true);
			__Game_Simulation_WaterPipeNode_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPipeNode>(false);
			__Game_Simulation_WaterPipeEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeEdge>(true);
			__Game_Simulation_WaterPipeEdge_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPipeEdge>(false);
			__Game_Simulation_WaterPipeNode_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeNode>(true);
		}
	}

	private const int kUpdateInterval = 64;

	private WaterPipeFlowSystem m_WaterPipeFlowSystem;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_NodeQuery;

	private EntityQuery m_EdgeQuery;

	private EntityQuery m_ParameterQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 64;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_WaterPipeFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterPipeFlowSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_NodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<WaterPipeNode>(),
			ComponentType.ReadOnly<ConnectedFlowEdge>(),
			ComponentType.Exclude<Deleted>()
		});
		m_EdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<WaterPipeEdge>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WaterPipeParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_NodeQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EdgeQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_ParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		WaterPipeParameterData singleton = ((EntityQuery)(ref m_ParameterQuery)).GetSingleton<WaterPipeParameterData>();
		bool purify = m_SimulationSystem.frameIndex / 64 % singleton.m_WaterPipePollutionSpreadInterval != 0;
		JobHandle val = JobChunkExtensions.ScheduleParallel<NodePollutionJob>(new NodePollutionJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FlowConnectionType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<WaterPipeNode>(ref __TypeHandle.__Game_Simulation_WaterPipeNode_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FlowEdges = InternalCompilerInterface.GetComponentLookup<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StaleWaterPipePurification = singleton.m_StaleWaterPipePurification
		}, m_NodeQuery, ((SystemBase)this).Dependency);
		EdgePollutionJob edgePollutionJob = new EdgePollutionJob
		{
			m_FlowEdgeType = InternalCompilerInterface.GetComponentTypeHandle<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FlowNodes = InternalCompilerInterface.GetComponentLookup<WaterPipeNode>(ref __TypeHandle.__Game_Simulation_WaterPipeNode_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SourceNode = m_WaterPipeFlowSystem.sourceNode,
			m_Purify = purify
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<EdgePollutionJob>(edgePollutionJob, m_EdgeQuery, val);
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
	public WaterPipePollutionSystem()
	{
	}
}

using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Serialization;
using Game.Simulation.Flow;
using Game.Tools;
using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ElectricityFlowSystem : GameSystemBase, IDefaultSerializable, ISerializable, IPostDeserialize
{
	private enum Phase
	{
		Prepare,
		Flow,
		Apply
	}

	[BurstCompile]
	private struct PrepareNetworkJob : IJob
	{
		public NativeList<Game.Simulation.Flow.Node> m_Nodes;

		public NativeList<Game.Simulation.Flow.Edge> m_Edges;

		public NativeList<Connection> m_Connections;

		public NativeList<int> m_ChargeNodes;

		public NativeList<int> m_DischargeNodes;

		public NativeList<int> m_TradeNodes;

		public int m_NodeCount;

		public int m_EdgeCount;

		public void Execute()
		{
			m_Nodes.ResizeUninitialized(m_NodeCount + 1);
			m_Nodes[0] = default(Game.Simulation.Flow.Node);
			m_Edges.ResizeUninitialized(m_EdgeCount + 1);
			m_Edges[0] = default(Game.Simulation.Flow.Edge);
			m_Connections.Clear();
			m_Connections.Capacity = 2 * m_EdgeCount + 1;
			ref NativeList<Connection> reference = ref m_Connections;
			Connection connection = default(Connection);
			reference.Add(ref connection);
			m_ChargeNodes.Clear();
			m_DischargeNodes.Clear();
			m_TradeNodes.Clear();
		}
	}

	[BurstCompile]
	private struct PrepareNodesJob : IJobChunk
	{
		public ComponentTypeHandle<ElectricityFlowNode> m_FlowNodeType;

		public int m_MaxChunkCapacity;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			int num = unfilteredChunkIndex * m_MaxChunkCapacity;
			NativeArray<ElectricityFlowNode> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityFlowNode>(ref m_FlowNodeType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CollectionUtils.ElementAt<ElectricityFlowNode>(nativeArray, i).m_Index = num + i + 1;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct PrepareEdgesJob : IJobChunk
	{
		public ComponentTypeHandle<ElectricityFlowEdge> m_FlowEdgeType;

		[NativeDisableParallelForRestriction]
		public NativeArray<Game.Simulation.Flow.Edge> m_Edges;

		public int m_MaxChunkCapacity;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			int num = unfilteredChunkIndex * m_MaxChunkCapacity;
			NativeArray<ElectricityFlowEdge> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityFlowEdge>(ref m_FlowEdgeType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				ref ElectricityFlowEdge reference = ref CollectionUtils.ElementAt<ElectricityFlowEdge>(nativeArray, i);
				int num2 = num + i + 1;
				m_Edges[num2] = new Game.Simulation.Flow.Edge
				{
					m_Capacity = reference.m_Capacity,
					m_Direction = reference.direction
				};
				reference.m_Index = num2;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct PrepareConnectionsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedFlowEdge> m_ConnectedFlowEdgeType;

		[ReadOnly]
		public ComponentTypeHandle<BatteryChargeNode> m_ChargeNodeType;

		[ReadOnly]
		public ComponentTypeHandle<BatteryDischargeNode> m_DischargeNodeType;

		[ReadOnly]
		public ComponentTypeHandle<TradeNode> m_TradeNodeType;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowNode> m_FlowNodes;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> m_FlowEdges;

		public NativeArray<Game.Simulation.Flow.Node> m_Nodes;

		public NativeList<Connection> m_Connections;

		public NativeList<int> m_ChargeNodes;

		public NativeList<int> m_DischargeNodes;

		public NativeList<int> m_TradeNodes;

		public int m_MaxChunkCapacity;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			int num = unfilteredChunkIndex * m_MaxChunkCapacity;
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<ConnectedFlowEdge> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedFlowEdge>(ref m_ConnectedFlowEdgeType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<BatteryChargeNode>(ref m_ChargeNodeType);
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<BatteryDischargeNode>(ref m_DischargeNodeType);
			bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<TradeNode>(ref m_TradeNodeType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				DynamicBuffer<ConnectedFlowEdge> val2 = bufferAccessor[i];
				int num2 = num + i + 1;
				ref Game.Simulation.Flow.Node reference = ref CollectionUtils.ElementAt<Game.Simulation.Flow.Node>(m_Nodes, num2);
				reference.m_FirstConnection = m_Connections.Length;
				reference.m_LastConnection = m_Connections.Length + val2.Length;
				Enumerator<ConnectedFlowEdge> enumerator = val2.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ConnectedFlowEdge current = enumerator.Current;
						ElectricityFlowEdge electricityFlowEdge = m_FlowEdges[current.m_Edge];
						bool flag4 = electricityFlowEdge.m_End == val;
						ElectricityFlowNode electricityFlowNode = m_FlowNodes[flag4 ? electricityFlowEdge.m_Start : electricityFlowEdge.m_End];
						ref NativeList<Connection> reference2 = ref m_Connections;
						Connection connection = new Connection
						{
							m_StartNode = num2,
							m_EndNode = electricityFlowNode.m_Index,
							m_Edge = electricityFlowEdge.m_Index,
							m_Backwards = flag4
						};
						reference2.Add(ref connection);
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
				if (flag)
				{
					m_ChargeNodes.Add(ref num2);
				}
				if (flag2)
				{
					m_DischargeNodes.Add(ref num2);
				}
				if (flag3)
				{
					m_TradeNodes.Add(ref num2);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct PopulateNodeIndicesJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<ElectricityFlowNode> m_FlowNodes;

		public NativeReference<NodeIndices> m_NodeIndices;

		public Entity m_SourceNode;

		public Entity m_SinkNode;

		public void Execute()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			m_NodeIndices.Value = new NodeIndices
			{
				m_SourceNode = m_FlowNodes[m_SourceNode].m_Index,
				m_SinkNode = m_FlowNodes[m_SinkNode].m_Index
			};
		}
	}

	[BurstCompile]
	private struct ApplyEdgesJob : IJobChunk
	{
		public ComponentTypeHandle<ElectricityFlowEdge> m_FlowEdgeType;

		[ReadOnly]
		public NativeArray<Game.Simulation.Flow.Edge> m_Edges;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ElectricityFlowEdge> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityFlowEdge>(ref m_FlowEdgeType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				ref ElectricityFlowEdge reference = ref CollectionUtils.ElementAt<ElectricityFlowEdge>(nativeArray, i);
				Game.Simulation.Flow.Edge edge = m_Edges[reference.m_Index];
				reference.m_Flow = edge.flow;
				reference.m_Flags &= ElectricityFlowEdgeFlags.ForwardBackward;
				reference.m_Flags |= GetBottleneckFlag(edge.m_CutElementId.m_Version);
			}
		}

		private ElectricityFlowEdgeFlags GetBottleneckFlag(int label)
		{
			return label switch
			{
				-2 => ElectricityFlowEdgeFlags.Bottleneck, 
				-3 => ElectricityFlowEdgeFlags.BeyondBottleneck, 
				-1 => ElectricityFlowEdgeFlags.None, 
				_ => ElectricityFlowEdgeFlags.Disconnected, 
			};
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		public ComponentTypeHandle<ElectricityFlowNode> __Game_Simulation_ElectricityFlowNode_RW_ComponentTypeHandle;

		public ComponentTypeHandle<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RW_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BatteryChargeNode> __Game_Simulation_BatteryChargeNode_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BatteryDischargeNode> __Game_Simulation_BatteryDischargeNode_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TradeNode> __Game_Simulation_TradeNode_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowNode> __Game_Simulation_ElectricityFlowNode_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Simulation_ElectricityFlowNode_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityFlowNode>(false);
			__Game_Simulation_ElectricityFlowEdge_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityFlowEdge>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedFlowEdge>(true);
			__Game_Simulation_BatteryChargeNode_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BatteryChargeNode>(true);
			__Game_Simulation_BatteryDischargeNode_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BatteryDischargeNode>(true);
			__Game_Simulation_TradeNode_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TradeNode>(true);
			__Game_Simulation_ElectricityFlowNode_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowNode>(true);
			__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(true);
		}
	}

	public const int kUpdateInterval = 128;

	public const int kUpdatesPerDay = 2048;

	public const int kUpdatesPerHour = 85;

	public const int kStartFrames = 2;

	public const int kAdjustFrame = 0;

	public const int kPrepareFrame = 1;

	public const int kFlowFrames = 124;

	public const int kFlowCompletionFrame = 125;

	public const int kEndFrames = 2;

	public const int kApplyFrame = 126;

	public const int kStatusFrame = 127;

	public const int kMaxEdgeCapacity = 1073741823;

	private const int kLayerHeight = 20;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_NodeGroup;

	private EntityQuery m_EdgeGroup;

	private EntityArchetype m_NodeArchetype;

	private EntityArchetype m_ChargeNodeArchetype;

	private EntityArchetype m_DischargeNodeArchetype;

	private EntityArchetype m_EdgeArchetype;

	private NativeList<Game.Simulation.Flow.Node> m_Nodes;

	private NativeList<Game.Simulation.Flow.Edge> m_Edges;

	private NativeList<Connection> m_Connections;

	private NativeReference<NodeIndices> m_NodeIndices;

	private NativeList<int> m_ChargeNodes;

	private NativeList<int> m_DischargeNodes;

	private NativeList<int> m_TradeNodes;

	private NativeReference<ElectricityFlowJob.State> m_FlowJobState;

	private NativeReference<MaxFlowSolverState> m_SolverState;

	private NativeList<LayerState> m_LayerStates;

	private NativeList<CutElement> m_LayerElements;

	private NativeList<CutElementRef> m_LayerElementRefs;

	private Entity m_SourceNode;

	private Entity m_SinkNode;

	private Entity m_LegacyOutsideSourceNode;

	private Entity m_LegacyOutsideSinkNode;

	private Phase m_NextPhase;

	private JobHandle m_DataDependency;

	private TypeHandle __TypeHandle;

	public bool ready { get; private set; }

	public EntityArchetype nodeArchetype => m_NodeArchetype;

	public EntityArchetype chargeNodeArchetype => m_ChargeNodeArchetype;

	public EntityArchetype dischargeNodeArchetype => m_DischargeNodeArchetype;

	public EntityArchetype edgeArchetype => m_EdgeArchetype;

	public Entity sourceNode => m_SourceNode;

	public Entity sinkNode => m_SinkNode;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_NodeGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ElectricityFlowNode>(),
			ComponentType.ReadOnly<ConnectedFlowEdge>(),
			ComponentType.Exclude<Deleted>()
		});
		m_EdgeGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<ElectricityFlowEdge>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_NodeArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ElectricityFlowNode>(),
			ComponentType.ReadOnly<ConnectedFlowEdge>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_ChargeNodeArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ElectricityFlowNode>(),
			ComponentType.ReadOnly<ConnectedFlowEdge>(),
			ComponentType.ReadOnly<BatteryChargeNode>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_DischargeNodeArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ElectricityFlowNode>(),
			ComponentType.ReadOnly<ConnectedFlowEdge>(),
			ComponentType.ReadOnly<BatteryDischargeNode>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EdgeArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ElectricityFlowEdge>() });
		m_Nodes = new NativeList<Game.Simulation.Flow.Node>(AllocatorHandle.op_Implicit((Allocator)4));
		m_Edges = new NativeList<Game.Simulation.Flow.Edge>(AllocatorHandle.op_Implicit((Allocator)4));
		m_Connections = new NativeList<Connection>(AllocatorHandle.op_Implicit((Allocator)4));
		m_NodeIndices = new NativeReference<NodeIndices>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		m_ChargeNodes = new NativeList<int>(AllocatorHandle.op_Implicit((Allocator)4));
		m_DischargeNodes = new NativeList<int>(AllocatorHandle.op_Implicit((Allocator)4));
		m_TradeNodes = new NativeList<int>(AllocatorHandle.op_Implicit((Allocator)4));
		m_FlowJobState = new NativeReference<ElectricityFlowJob.State>(new ElectricityFlowJob.State(20000), AllocatorHandle.op_Implicit((Allocator)4));
		m_SolverState = new NativeReference<MaxFlowSolverState>(default(MaxFlowSolverState), AllocatorHandle.op_Implicit((Allocator)4));
		m_LayerStates = new NativeList<LayerState>(AllocatorHandle.op_Implicit((Allocator)4));
		m_LayerElements = new NativeList<CutElement>(AllocatorHandle.op_Implicit((Allocator)4));
		m_LayerElementRefs = new NativeList<CutElementRef>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_DataDependency)).Complete();
		m_Nodes.Dispose();
		m_Edges.Dispose();
		m_Connections.Dispose();
		m_NodeIndices.Dispose();
		m_ChargeNodes.Dispose();
		m_DischargeNodes.Dispose();
		m_TradeNodes.Dispose();
		m_FlowJobState.Dispose();
		m_SolverState.Dispose();
		m_LayerStates.Dispose();
		m_LayerElements.Dispose();
		m_LayerElementRefs.Dispose();
		base.OnDestroy();
	}

	public void Reset()
	{
		((JobHandle)(ref m_DataDependency)).Complete();
		m_NextPhase = Phase.Prepare;
		m_FlowJobState.Value = new ElectricityFlowJob.State(20000);
		ready = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (m_NextPhase == Phase.Prepare)
		{
			PreparePhase();
		}
		else if (m_NextPhase == Phase.Flow)
		{
			FlowPhase();
		}
		else if (m_NextPhase == Phase.Apply)
		{
			ApplyPhase();
		}
	}

	private void PreparePhase()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		if (m_SimulationSystem.frameIndex % 128 == 1)
		{
			int chunkCapacity = ((EntityArchetype)(ref m_NodeArchetype)).ChunkCapacity;
			int chunkCapacity2 = ((EntityArchetype)(ref m_EdgeArchetype)).ChunkCapacity;
			int nodeCount = chunkCapacity * ((EntityQuery)(ref m_NodeGroup)).CalculateChunkCountWithoutFiltering();
			int edgeCount = chunkCapacity2 * ((EntityQuery)(ref m_EdgeGroup)).CalculateChunkCountWithoutFiltering();
			JobHandle val = IJobExtensions.Schedule<PrepareNetworkJob>(new PrepareNetworkJob
			{
				m_Nodes = m_Nodes,
				m_Edges = m_Edges,
				m_Connections = m_Connections,
				m_ChargeNodes = m_ChargeNodes,
				m_DischargeNodes = m_DischargeNodes,
				m_TradeNodes = m_TradeNodes,
				m_NodeCount = nodeCount,
				m_EdgeCount = edgeCount
			}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_DataDependency));
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<PrepareNodesJob>(new PrepareNodesJob
			{
				m_FlowNodeType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityFlowNode>(ref __TypeHandle.__Game_Simulation_ElectricityFlowNode_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MaxChunkCapacity = chunkCapacity
			}, m_NodeGroup, ((SystemBase)this).Dependency);
			JobHandle val3 = JobChunkExtensions.ScheduleParallel<PrepareEdgesJob>(new PrepareEdgesJob
			{
				m_FlowEdgeType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Edges = m_Edges.AsDeferredJobArray(),
				m_MaxChunkCapacity = chunkCapacity2
			}, m_EdgeGroup, val);
			JobHandle val4 = JobChunkExtensions.Schedule<PrepareConnectionsJob>(new PrepareConnectionsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedFlowEdgeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ChargeNodeType = InternalCompilerInterface.GetComponentTypeHandle<BatteryChargeNode>(ref __TypeHandle.__Game_Simulation_BatteryChargeNode_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DischargeNodeType = InternalCompilerInterface.GetComponentTypeHandle<BatteryDischargeNode>(ref __TypeHandle.__Game_Simulation_BatteryDischargeNode_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TradeNodeType = InternalCompilerInterface.GetComponentTypeHandle<TradeNode>(ref __TypeHandle.__Game_Simulation_TradeNode_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_FlowNodes = InternalCompilerInterface.GetComponentLookup<ElectricityFlowNode>(ref __TypeHandle.__Game_Simulation_ElectricityFlowNode_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Nodes = m_Nodes.AsDeferredJobArray(),
				m_Connections = m_Connections,
				m_ChargeNodes = m_ChargeNodes,
				m_DischargeNodes = m_DischargeNodes,
				m_TradeNodes = m_TradeNodes,
				m_MaxChunkCapacity = chunkCapacity
			}, m_NodeGroup, JobHandle.CombineDependencies(val2, val3));
			JobHandle val5 = IJobExtensions.Schedule<PopulateNodeIndicesJob>(new PopulateNodeIndicesJob
			{
				m_FlowNodes = InternalCompilerInterface.GetComponentLookup<ElectricityFlowNode>(ref __TypeHandle.__Game_Simulation_ElectricityFlowNode_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeIndices = m_NodeIndices,
				m_SourceNode = m_SourceNode,
				m_SinkNode = m_SinkNode
			}, JobHandle.CombineDependencies(val2, m_DataDependency));
			((SystemBase)this).Dependency = (m_DataDependency = JobHandle.CombineDependencies(val4, val5));
			m_NextPhase = Phase.Flow;
		}
	}

	private void FlowPhase()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		uint num = m_SimulationSystem.frameIndex % 128;
		Assert.IsTrue(num != 0 && num != 1 && num != 126 && num != 127);
		bool flag = num >= 125;
		ElectricityFlowJob electricityFlowJob = new ElectricityFlowJob
		{
			m_State = m_FlowJobState,
			m_Nodes = m_Nodes.AsDeferredJobArray(),
			m_Edges = m_Edges.AsDeferredJobArray(),
			m_Connections = m_Connections.AsDeferredJobArray(),
			m_NodeIndices = m_NodeIndices,
			m_ChargeNodes = m_ChargeNodes.AsDeferredJobArray(),
			m_DischargeNodes = m_DischargeNodes.AsDeferredJobArray(),
			m_TradeNodes = m_TradeNodes.AsDeferredJobArray(),
			m_SolverState = m_SolverState,
			m_LayerStates = m_LayerStates,
			m_LayerElements = m_LayerElements,
			m_LayerElementRefs = m_LayerElementRefs,
			m_LabelQueue = new NativeQueue<int>(AllocatorHandle.op_Implicit((Allocator)3)),
			m_LayerHeight = 20,
			m_FrameCount = 1,
			m_FinalFrame = flag
		};
		JobHandle val = IJobExtensions.Schedule<ElectricityFlowJob>(electricityFlowJob, m_DataDependency);
		electricityFlowJob.m_LabelQueue.Dispose(val);
		m_DataDependency = val;
		if (flag)
		{
			m_NextPhase = Phase.Apply;
		}
	}

	private void ApplyPhase()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsFalse(m_SimulationSystem.frameIndex % 128 > 126);
		if (m_SimulationSystem.frameIndex % 128 == 126)
		{
			JobHandle dataDependency = JobChunkExtensions.ScheduleParallel<ApplyEdgesJob>(new ApplyEdgesJob
			{
				m_FlowEdgeType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Edges = m_Edges.AsDeferredJobArray()
			}, m_EdgeGroup, JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_DataDependency));
			((SystemBase)this).Dependency = (m_DataDependency = dataDependency);
			m_NextPhase = Phase.Prepare;
			ready = true;
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_DataDependency)).Complete();
		Entity val = m_SourceNode;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		Entity val2 = m_SinkNode;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val2);
		int lastTotalSteps = m_FlowJobState.Value.m_LastTotalSteps;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastTotalSteps);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_DataDependency)).Complete();
		ref Entity reference = ref m_SourceNode;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref Entity reference2 = ref m_SinkNode;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.electricityTrading)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.batteryRework2)
			{
				ref Entity reference3 = ref m_LegacyOutsideSourceNode;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference3);
				ref Entity reference4 = ref m_LegacyOutsideSinkNode;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference4);
				goto IL_00b1;
			}
		}
		m_LegacyOutsideSourceNode = Entity.Null;
		m_LegacyOutsideSinkNode = Entity.Null;
		goto IL_00b1;
		IL_00b1:
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterElectricityID)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.electricityImprovements)
			{
				int num = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
				Entity val = default(Entity);
				for (int i = 0; i < num; i++)
				{
					((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
				}
				NativeList<int> val2 = new NativeList<int>(AllocatorHandle.op_Implicit((Allocator)2));
				((IReader)reader/*cast due to .constrained prefix*/).Read(val2);
			}
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.electricityImprovements)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.electricityImprovements2)
			{
				int num2 = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			}
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.electricityImprovements2)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.flowJobImprovements)
			{
				int num3 = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
				int num4 = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num4);
				int num5 = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num5);
			}
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version > Version.flowJobImprovements)
		{
			ref int lastTotalSteps = ref CollectionUtils.ValueAsRef<ElectricityFlowJob.State>(m_FlowJobState).m_LastTotalSteps;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastTotalSteps);
		}
	}

	public void SetDefaults(Context context)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Reset();
		m_SourceNode = Entity.Null;
		m_SinkNode = Entity.Null;
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager;
		if ((int)((Context)(ref context)).purpose == 4 || ((int)((Context)(ref context)).purpose == 1 && ((Context)(ref context)).version < Version.timoSerializationFlow) || ((int)((Context)(ref context)).purpose == 5 && m_SourceNode == Entity.Null))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			m_SourceNode = ((EntityManager)(ref entityManager)).CreateEntity(m_NodeArchetype);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			m_SinkNode = ((EntityManager)(ref entityManager)).CreateEntity(m_NodeArchetype);
		}
		if (m_LegacyOutsideSourceNode != Entity.Null || m_LegacyOutsideSinkNode != Entity.Null)
		{
			if (m_LegacyOutsideSourceNode != Entity.Null)
			{
				ElectricityGraphUtils.DeleteFlowNode(((ComponentSystemBase)this).EntityManager, m_LegacyOutsideSourceNode);
			}
			if (m_LegacyOutsideSinkNode != Entity.Null)
			{
				ElectricityGraphUtils.DeleteFlowNode(((ComponentSystemBase)this).EntityManager, m_LegacyOutsideSinkNode);
			}
			NativeArray<Entity> val = ((EntityQuery)(ref m_EdgeGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<ElectricityFlowEdge> val2 = ((EntityQuery)(ref m_EdgeGroup)).ToComponentDataArray<ElectricityFlowEdge>(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				for (int i = 0; i < val.Length; i++)
				{
					Entity val3 = val[i];
					ElectricityFlowEdge electricityFlowEdge = val2[i];
					if (electricityFlowEdge.m_Start == m_LegacyOutsideSourceNode || electricityFlowEdge.m_End == m_LegacyOutsideSourceNode)
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).AddComponent<Deleted>(val3);
					}
					else if (electricityFlowEdge.m_Start == m_LegacyOutsideSinkNode || electricityFlowEdge.m_End == m_LegacyOutsideSinkNode)
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).AddComponent<Deleted>(val3);
					}
				}
			}
			finally
			{
				val.Dispose();
				val2.Dispose();
			}
			m_LegacyOutsideSourceNode = Entity.Null;
			m_LegacyOutsideSinkNode = Entity.Null;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			EntityQuery val4 = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[3]
			{
				ComponentType.ReadOnly<ElectricityOutsideConnection>(),
				ComponentType.ReadOnly<Owner>(),
				ComponentType.Exclude<Temp>()
			});
			NativeArray<Owner> val5 = ((EntityQuery)(ref val4)).ToComponentDataArray<Owner>(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				Enumerator<Owner> enumerator = val5.GetEnumerator();
				try
				{
					ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
					while (enumerator.MoveNext())
					{
						Owner current = enumerator.Current;
						if (EntitiesExtensions.TryGetComponent<ElectricityNodeConnection>(((ComponentSystemBase)this).EntityManager, current.m_Owner, ref electricityNodeConnection))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							((EntityManager)(ref entityManager)).AddComponent<TradeNode>(electricityNodeConnection.m_ElectricityNode);
							ElectricityGraphUtils.CreateFlowEdge(((ComponentSystemBase)this).EntityManager, m_EdgeArchetype, m_SourceNode, electricityNodeConnection.m_ElectricityNode, FlowDirection.None, 1073741823);
							ElectricityGraphUtils.CreateFlowEdge(((ComponentSystemBase)this).EntityManager, m_EdgeArchetype, electricityNodeConnection.m_ElectricityNode, m_SinkNode, FlowDirection.None, 1073741823);
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			finally
			{
				((EntityQuery)(ref val4)).Dispose();
				val5.Dispose();
			}
		}
		Reset();
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
	public ElectricityFlowSystem()
	{
	}
}

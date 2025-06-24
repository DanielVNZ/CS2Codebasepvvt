using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Game.Serialization;
using Game.Simulation.Flow;
using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class WaterPipeFlowSystem : GameSystemBase, IDefaultSerializable, ISerializable, IPostDeserialize
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
		public NativeList<Game.Simulation.Flow.Node> m_FreshNodes;

		public NativeList<Game.Simulation.Flow.Node> m_SewageNodes;

		public NativeList<Game.Simulation.Flow.Edge> m_FreshEdges;

		public NativeList<Game.Simulation.Flow.Edge> m_SewageEdges;

		public NativeList<Connection> m_Connections;

		public NativeList<int> m_TradeNodes;

		public int m_NodeCount;

		public int m_EdgeCount;

		public void Execute()
		{
			m_FreshNodes.ResizeUninitialized(m_NodeCount + 1);
			m_SewageNodes.ResizeUninitialized(m_NodeCount + 1);
			ref NativeList<Game.Simulation.Flow.Node> reference = ref m_FreshNodes;
			Game.Simulation.Flow.Node node = (m_SewageNodes[0] = default(Game.Simulation.Flow.Node));
			reference[0] = node;
			m_FreshEdges.ResizeUninitialized(m_EdgeCount + 1);
			m_SewageEdges.ResizeUninitialized(m_EdgeCount + 1);
			ref NativeList<Game.Simulation.Flow.Edge> reference2 = ref m_FreshEdges;
			Game.Simulation.Flow.Edge edge = (m_SewageEdges[0] = default(Game.Simulation.Flow.Edge));
			reference2[0] = edge;
			m_Connections.Clear();
			m_Connections.Capacity = 2 * m_EdgeCount + 1;
			ref NativeList<Connection> reference3 = ref m_Connections;
			Connection connection = default(Connection);
			reference3.Add(ref connection);
			m_TradeNodes.Clear();
		}
	}

	[BurstCompile]
	private struct PrepareNodesJob : IJobChunk
	{
		public ComponentTypeHandle<WaterPipeNode> m_FlowNodeType;

		public int m_MaxChunkCapacity;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			int num = unfilteredChunkIndex * m_MaxChunkCapacity;
			NativeArray<WaterPipeNode> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPipeNode>(ref m_FlowNodeType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CollectionUtils.ElementAt<WaterPipeNode>(nativeArray, i).m_Index = num + i + 1;
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
		public ComponentTypeHandle<WaterPipeEdge> m_FlowEdgeType;

		[NativeDisableParallelForRestriction]
		public NativeArray<Game.Simulation.Flow.Edge> m_FreshEdges;

		[NativeDisableParallelForRestriction]
		public NativeArray<Game.Simulation.Flow.Edge> m_SewageEdges;

		public int m_MaxChunkCapacity;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			int num = unfilteredChunkIndex * m_MaxChunkCapacity;
			NativeArray<WaterPipeEdge> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPipeEdge>(ref m_FlowEdgeType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				ref WaterPipeEdge reference = ref CollectionUtils.ElementAt<WaterPipeEdge>(nativeArray, i);
				int num2 = num + i + 1;
				m_FreshEdges[num2] = new Game.Simulation.Flow.Edge
				{
					m_Capacity = reference.m_FreshCapacity,
					m_Direction = FlowDirection.Both
				};
				m_SewageEdges[num2] = new Game.Simulation.Flow.Edge
				{
					m_Capacity = reference.m_SewageCapacity,
					m_Direction = FlowDirection.Both
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
		public ComponentTypeHandle<TradeNode> m_TradeNodeType;

		[ReadOnly]
		public ComponentLookup<WaterPipeNode> m_FlowNodes;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> m_FlowEdges;

		public NativeArray<Game.Simulation.Flow.Node> m_FreshNodes;

		public NativeArray<Game.Simulation.Flow.Node> m_SewageNodes;

		public NativeList<Connection> m_Connections;

		public NativeList<int> m_TradeNodes;

		public int m_MaxChunkCapacity;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			int num = unfilteredChunkIndex * m_MaxChunkCapacity;
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<ConnectedFlowEdge> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedFlowEdge>(ref m_ConnectedFlowEdgeType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<TradeNode>(ref m_TradeNodeType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				DynamicBuffer<ConnectedFlowEdge> val2 = bufferAccessor[i];
				int num2 = num + i + 1;
				ref Game.Simulation.Flow.Node reference = ref CollectionUtils.ElementAt<Game.Simulation.Flow.Node>(m_FreshNodes, num2);
				ref Game.Simulation.Flow.Node reference2 = ref CollectionUtils.ElementAt<Game.Simulation.Flow.Node>(m_SewageNodes, num2);
				reference.m_FirstConnection = (reference2.m_FirstConnection = m_Connections.Length);
				reference.m_LastConnection = (reference2.m_LastConnection = m_Connections.Length + val2.Length);
				Enumerator<ConnectedFlowEdge> enumerator = val2.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ConnectedFlowEdge current = enumerator.Current;
						WaterPipeEdge waterPipeEdge = m_FlowEdges[current.m_Edge];
						bool flag2 = waterPipeEdge.m_End == val;
						WaterPipeNode waterPipeNode = m_FlowNodes[flag2 ? waterPipeEdge.m_Start : waterPipeEdge.m_End];
						ref NativeList<Connection> reference3 = ref m_Connections;
						Connection connection = new Connection
						{
							m_StartNode = num2,
							m_EndNode = waterPipeNode.m_Index,
							m_Edge = waterPipeEdge.m_Index,
							m_Backwards = flag2
						};
						reference3.Add(ref connection);
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
				if (flag)
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
		public ComponentLookup<WaterPipeNode> m_FlowNodes;

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
		public ComponentTypeHandle<WaterPipeEdge> m_FlowEdgeType;

		[ReadOnly]
		public NativeArray<Game.Simulation.Flow.Edge> m_FreshEdges;

		[ReadOnly]
		public NativeArray<Game.Simulation.Flow.Edge> m_SewageEdges;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<WaterPipeEdge> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPipeEdge>(ref m_FlowEdgeType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				ref WaterPipeEdge reference = ref CollectionUtils.ElementAt<WaterPipeEdge>(nativeArray, i);
				Game.Simulation.Flow.Edge edge = m_FreshEdges[reference.m_Index];
				Game.Simulation.Flow.Edge edge2 = m_SewageEdges[reference.m_Index];
				reference.m_FreshFlow = edge.m_FinalFlow;
				reference.m_SewageFlow = edge2.m_FinalFlow;
				reference.m_Flags = WaterPipeEdgeFlags.None;
				if (edge.m_CutElementId.m_Version == -1)
				{
					reference.m_Flags |= WaterPipeEdgeFlags.WaterShortage;
				}
				else if (edge.m_CutElementId.m_Version != -2)
				{
					reference.m_Flags |= WaterPipeEdgeFlags.WaterDisconnected;
				}
				if (edge2.m_CutElementId.m_Version == -1)
				{
					reference.m_Flags |= WaterPipeEdgeFlags.SewageBackup;
				}
				else if (edge2.m_CutElementId.m_Version != -2)
				{
					reference.m_Flags |= WaterPipeEdgeFlags.SewageDisconnected;
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
		public ComponentTypeHandle<WaterPipeNode> __Game_Simulation_WaterPipeNode_RW_ComponentTypeHandle;

		public ComponentTypeHandle<WaterPipeEdge> __Game_Simulation_WaterPipeEdge_RW_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TradeNode> __Game_Simulation_TradeNode_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<WaterPipeNode> __Game_Simulation_WaterPipeNode_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> __Game_Simulation_WaterPipeEdge_RO_ComponentLookup;

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
			__Game_Simulation_WaterPipeNode_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPipeNode>(false);
			__Game_Simulation_WaterPipeEdge_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPipeEdge>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedFlowEdge>(true);
			__Game_Simulation_TradeNode_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TradeNode>(true);
			__Game_Simulation_WaterPipeNode_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeNode>(true);
			__Game_Simulation_WaterPipeEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeEdge>(true);
		}
	}

	public const int kUpdateInterval = 128;

	public const int kUpdateOffset = 64;

	public const int kUpdatesPerDay = 2048;

	public const int kStartFrames = 2;

	public const int kAdjustFrame = 64;

	public const int kPrepareFrame = 65;

	public const int kFlowFrames = 124;

	public const int kFlowCompletionFrame = 61;

	public const int kEndFrames = 2;

	public const int kApplyFrame = 62;

	public const int kStatusFrame = 63;

	public const int kMaxEdgeCapacity = 1073741823;

	private const int kLayerHeight = 20;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_NodeGroup;

	private EntityQuery m_EdgeGroup;

	private EntityArchetype m_NodeArchetype;

	private EntityArchetype m_EdgeArchetype;

	private WaterPipeFlowJob.Data m_FreshData;

	private WaterPipeFlowJob.Data m_SewageData;

	private NativeList<Connection> m_Connections;

	private NativeReference<NodeIndices> m_NodeIndices;

	private NativeList<int> m_TradeNodes;

	private Entity m_SourceNode;

	private Entity m_SinkNode;

	private Phase m_NextPhase;

	private JobHandle m_DataDependency;

	private TypeHandle __TypeHandle;

	public bool ready { get; private set; }

	public EntityArchetype nodeArchetype => m_NodeArchetype;

	public EntityArchetype edgeArchetype => m_EdgeArchetype;

	public Entity sourceNode => m_SourceNode;

	public Entity sinkNode => m_SinkNode;

	public bool fluidFlowEnabled { get; set; } = true;

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
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_NodeGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<WaterPipeNode>(),
			ComponentType.ReadOnly<ConnectedFlowEdge>(),
			ComponentType.Exclude<Deleted>()
		});
		m_EdgeGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<WaterPipeEdge>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_NodeArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<WaterPipeNode>(),
			ComponentType.ReadOnly<ConnectedFlowEdge>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EdgeArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WaterPipeEdge>() });
		m_FreshData = new WaterPipeFlowJob.Data(200000, (Allocator)4);
		m_SewageData = new WaterPipeFlowJob.Data(200000, (Allocator)4);
		m_Connections = new NativeList<Connection>(AllocatorHandle.op_Implicit((Allocator)4));
		m_NodeIndices = new NativeReference<NodeIndices>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		m_TradeNodes = new NativeList<int>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_DataDependency)).Complete();
		m_FreshData.Dispose();
		m_SewageData.Dispose();
		m_Connections.Dispose();
		m_NodeIndices.Dispose();
		m_TradeNodes.Dispose();
		base.OnDestroy();
	}

	public void Reset()
	{
		((JobHandle)(ref m_DataDependency)).Complete();
		m_NextPhase = Phase.Prepare;
		m_FreshData.m_State.Value = new WaterPipeFlowJob.State(200000);
		m_SewageData.m_State.Value = new WaterPipeFlowJob.State(200000);
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
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		if (m_SimulationSystem.frameIndex % 128 == 65)
		{
			int chunkCapacity = ((EntityArchetype)(ref m_NodeArchetype)).ChunkCapacity;
			int chunkCapacity2 = ((EntityArchetype)(ref m_EdgeArchetype)).ChunkCapacity;
			int nodeCount = chunkCapacity * ((EntityQuery)(ref m_NodeGroup)).CalculateChunkCountWithoutFiltering();
			int edgeCount = chunkCapacity2 * ((EntityQuery)(ref m_EdgeGroup)).CalculateChunkCountWithoutFiltering();
			JobHandle val = IJobExtensions.Schedule<PrepareNetworkJob>(new PrepareNetworkJob
			{
				m_FreshNodes = m_FreshData.m_Nodes,
				m_SewageNodes = m_SewageData.m_Nodes,
				m_FreshEdges = m_FreshData.m_Edges,
				m_SewageEdges = m_SewageData.m_Edges,
				m_Connections = m_Connections,
				m_TradeNodes = m_TradeNodes,
				m_NodeCount = nodeCount,
				m_EdgeCount = edgeCount
			}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_DataDependency));
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<PrepareNodesJob>(new PrepareNodesJob
			{
				m_FlowNodeType = InternalCompilerInterface.GetComponentTypeHandle<WaterPipeNode>(ref __TypeHandle.__Game_Simulation_WaterPipeNode_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MaxChunkCapacity = chunkCapacity
			}, m_NodeGroup, ((SystemBase)this).Dependency);
			JobHandle val3 = JobChunkExtensions.ScheduleParallel<PrepareEdgesJob>(new PrepareEdgesJob
			{
				m_FlowEdgeType = InternalCompilerInterface.GetComponentTypeHandle<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_FreshEdges = m_FreshData.m_Edges.AsDeferredJobArray(),
				m_SewageEdges = m_SewageData.m_Edges.AsDeferredJobArray(),
				m_MaxChunkCapacity = chunkCapacity2
			}, m_EdgeGroup, val);
			JobHandle val4 = JobChunkExtensions.Schedule<PrepareConnectionsJob>(new PrepareConnectionsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedFlowEdgeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TradeNodeType = InternalCompilerInterface.GetComponentTypeHandle<TradeNode>(ref __TypeHandle.__Game_Simulation_TradeNode_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_FlowNodes = InternalCompilerInterface.GetComponentLookup<WaterPipeNode>(ref __TypeHandle.__Game_Simulation_WaterPipeNode_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FlowEdges = InternalCompilerInterface.GetComponentLookup<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FreshNodes = m_FreshData.m_Nodes.AsDeferredJobArray(),
				m_SewageNodes = m_SewageData.m_Nodes.AsDeferredJobArray(),
				m_Connections = m_Connections,
				m_TradeNodes = m_TradeNodes,
				m_MaxChunkCapacity = chunkCapacity
			}, m_NodeGroup, JobHandle.CombineDependencies(val2, val3));
			JobHandle val5 = IJobExtensions.Schedule<PopulateNodeIndicesJob>(new PopulateNodeIndicesJob
			{
				m_FlowNodes = InternalCompilerInterface.GetComponentLookup<WaterPipeNode>(ref __TypeHandle.__Game_Simulation_WaterPipeNode_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
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
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		uint num = m_SimulationSystem.frameIndex % 128;
		Assert.IsTrue(num != 64 && num != 65 && num != 62 && num != 63);
		bool flag = num == 61;
		JobHandle val = ScheduleFlowJob(m_FreshData, 1073741823, 1073741823, flag);
		JobHandle val2 = ScheduleFlowJob(m_SewageData, 1073741823, 0, flag);
		m_DataDependency = JobHandle.CombineDependencies(val, val2);
		if (flag)
		{
			m_NextPhase = Phase.Apply;
		}
	}

	private JobHandle ScheduleFlowJob(WaterPipeFlowJob.Data jobData, int importCapacity, int exportCapacity, bool finalFrame)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		return IJobExtensions.Schedule<WaterPipeFlowJob>(new WaterPipeFlowJob
		{
			m_State = jobData.m_State,
			m_Nodes = jobData.m_Nodes.AsDeferredJobArray(),
			m_Edges = jobData.m_Edges.AsDeferredJobArray(),
			m_Connections = m_Connections.AsDeferredJobArray(),
			m_NodeIndices = m_NodeIndices,
			m_TradeNodes = m_TradeNodes.AsDeferredJobArray(),
			m_MaxFlowState = jobData.m_MaxFlowState,
			m_LayerStates = jobData.m_LayerStates,
			m_LayerElements = jobData.m_LayerElements,
			m_LayerElementRefs = jobData.m_LayerElementRefs,
			m_FluidFlowState = jobData.m_FluidFlowState,
			m_ImportCapacity = importCapacity,
			m_ExportCapacity = exportCapacity,
			m_FluidFlowEnabled = fluidFlowEnabled,
			m_LayerHeight = 20,
			m_FrameCount = 1,
			m_FinalFrame = finalFrame
		}, m_DataDependency);
	}

	private void ApplyPhase()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (m_SimulationSystem.frameIndex % 128 == 62)
		{
			JobHandle dataDependency = JobChunkExtensions.ScheduleParallel<ApplyEdgesJob>(new ApplyEdgesJob
			{
				m_FlowEdgeType = InternalCompilerInterface.GetComponentTypeHandle<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_FreshEdges = m_FreshData.m_Edges.AsDeferredJobArray(),
				m_SewageEdges = m_SewageData.m_Edges.AsDeferredJobArray()
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
		int lastTotalSteps = m_FreshData.m_State.Value.m_LastTotalSteps;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastTotalSteps);
		int lastTotalSteps2 = m_SewageData.m_State.Value.m_LastTotalSteps;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastTotalSteps2);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_DataDependency)).Complete();
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterPipeFlowSim)
		{
			ref Entity reference = ref m_SourceNode;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
			ref Entity reference2 = ref m_SinkNode;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
		}
		else
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.waterTrade)
			{
				Entity val = default(Entity);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			}
			m_SourceNode = Entity.Null;
			m_SinkNode = Entity.Null;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version > Version.flowJobImprovements)
		{
			ref int lastTotalSteps = ref CollectionUtils.ValueAsRef<WaterPipeFlowJob.State>(m_FreshData.m_State).m_LastTotalSteps;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastTotalSteps);
			ref int lastTotalSteps2 = ref CollectionUtils.ValueAsRef<WaterPipeFlowJob.State>(m_SewageData.m_State).m_LastTotalSteps;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastTotalSteps2);
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Invalid comparison between Unknown and I4
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager;
		if (m_SourceNode == Entity.Null && m_SinkNode == Entity.Null)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			m_SourceNode = ((EntityManager)(ref entityManager)).CreateEntity(m_NodeArchetype);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			m_SinkNode = ((EntityManager)(ref entityManager)).CreateEntity(m_NodeArchetype);
		}
		Reset();
		DispatchWaterSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DispatchWaterSystem>();
		if (((Context)(ref context)).version < Version.waterPipeFlowSim && (int)((Context)(ref context)).purpose == 2)
		{
			Debug.LogWarning((object)"Detected legacy water pipes, disabling water & sewage notifications!");
			orCreateSystemManaged.freshConsumptionDisabled = true;
			orCreateSystemManaged.sewageConsumptionDisabled = true;
		}
		else
		{
			orCreateSystemManaged.freshConsumptionDisabled = false;
			orCreateSystemManaged.sewageConsumptionDisabled = false;
		}
		if (!(((Context)(ref context)).version < Version.waterPipePollution))
		{
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WaterPipeNodeConnection>() });
		try
		{
			NativeArray<Entity> val2 = ((EntityQuery)(ref val)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
			NativeArray<WaterPipeNodeConnection> val3 = ((EntityQuery)(ref val)).ToComponentDataArray<WaterPipeNodeConnection>(AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < val3.Length; i++)
			{
				if (val3[i].m_WaterPipeNode == Entity.Null)
				{
					COSystemBase.baseLog.WarnFormat("{0} has null WaterPipeNode! Removing...", (object)val2[i]);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).RemoveComponent<WaterPipeNodeConnection>(val2[i]);
				}
			}
		}
		finally
		{
			((EntityQuery)(ref val)).Dispose();
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
	public WaterPipeFlowSystem()
	{
	}
}

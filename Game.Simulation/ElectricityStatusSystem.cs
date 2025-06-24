using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Notifications;
using Game.Objects;
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
public class ElectricityStatusSystem : GameSystemBase
{
	[BurstCompile]
	private struct NetEdgeNotificationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityNodeConnection> m_ElectricityNodeConnectionType;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> m_ConnectedFlowEdges;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> m_FlowEdges;

		public Entity m_SourceNode;

		public Entity m_SinkNode;

		public ElectricityParameterData m_ElectricityParameters;

		public IconCommandBuffer m_IconCommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ElectricityNodeConnection> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityNodeConnection>(ref m_ElectricityNodeConnectionType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity owner = nativeArray[i];
				Entity electricityNode = nativeArray2[i].m_ElectricityNode;
				bool flag = false;
				DynamicBuffer<ConnectedFlowEdge> val = m_ConnectedFlowEdges[electricityNode];
				for (int j = 0; j < val.Length; j++)
				{
					ElectricityFlowEdge electricityFlowEdge = m_FlowEdges[val[j].m_Edge];
					if (electricityFlowEdge.isBottleneck && electricityFlowEdge.m_Start != m_SourceNode && electricityFlowEdge.m_End != m_SinkNode)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					m_IconCommandBuffer.Add(owner, m_ElectricityParameters.m_BottleneckNotificationPrefab, IconPriority.Problem);
				}
				else
				{
					m_IconCommandBuffer.Remove(owner, m_ElectricityParameters.m_BottleneckNotificationPrefab);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct BuildingNotificationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityBuildingConnection> m_ElectricityBuildingConnectionType;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> m_ConnectedFlowEdges;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> m_FlowEdges;

		public Entity m_SourceNode;

		public ElectricityParameterData m_ElectricityParameters;

		public IconCommandBuffer m_IconCommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ElectricityBuildingConnection> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityBuildingConnection>(ref m_ElectricityBuildingConnectionType);
			DynamicBuffer<ConnectedFlowEdge> val = default(DynamicBuffer<ConnectedFlowEdge>);
			ElectricityFlowEdge electricityFlowEdge = default(ElectricityFlowEdge);
			ElectricityFlowEdge electricityFlowEdge2 = default(ElectricityFlowEdge);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity netEntity = nativeArray[i];
				ElectricityBuildingConnection electricityBuildingConnection = nativeArray2[i];
				bool flag = false;
				if (electricityBuildingConnection.m_TransformerNode != Entity.Null && m_ConnectedFlowEdges.TryGetBuffer(electricityBuildingConnection.m_TransformerNode, ref val))
				{
					for (int j = 0; j < val.Length; j++)
					{
						if (m_FlowEdges[val[j].m_Edge].isBottleneck)
						{
							flag = true;
							break;
						}
					}
				}
				bool flag2 = false;
				if (electricityBuildingConnection.m_ProducerEdge != Entity.Null && m_FlowEdges.TryGetComponent(electricityBuildingConnection.m_ProducerEdge, ref electricityFlowEdge))
				{
					flag2 = electricityFlowEdge.isBottleneck;
				}
				bool enabled = false;
				if (electricityBuildingConnection.m_ConsumerEdge != Entity.Null && m_FlowEdges.TryGetComponent(electricityBuildingConnection.m_ConsumerEdge, ref electricityFlowEdge2))
				{
					enabled = electricityFlowEdge2.isBeyondBottleneck;
				}
				SetProblemNotification(netEntity, m_ElectricityParameters.m_TransformerNotificationPrefab, !flag2 && flag);
				SetProblemNotification(netEntity, m_ElectricityParameters.m_NotEnoughProductionNotificationPrefab, flag2);
				SetProblemNotification(netEntity, m_ElectricityParameters.m_BottleneckNotificationPrefab, enabled);
			}
		}

		private void SetProblemNotification(Entity netEntity, Entity prefab, bool enabled)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (enabled)
			{
				m_IconCommandBuffer.Add(netEntity, prefab, IconPriority.Problem);
			}
			else
			{
				m_IconCommandBuffer.Remove(netEntity, prefab);
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
		public ComponentTypeHandle<ElectricityNodeConnection> __Game_Simulation_ElectricityNodeConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityBuildingConnection> __Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_ElectricityNodeConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityNodeConnection>(true);
			__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(true);
			__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(true);
			__Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityBuildingConnection>(true);
		}
	}

	private ElectricityFlowSystem m_ElectricityFlowSystem;

	private IconCommandSystem m_IconCommandSystem;

	private EntityQuery m_EdgeQuery;

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_ElectricityParameterQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 127;
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ElectricityFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityFlowSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_EdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<ElectricityNodeConnection>(),
			ComponentType.Exclude<Temp>()
		});
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<ElectricityBuildingConnection>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<Game.Net.SubNet>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ElectricityParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ElectricityParameterData>() });
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_EdgeQuery, m_BuildingQuery });
		((ComponentSystemBase)this).RequireForUpdate(m_ElectricityParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		IconCommandBuffer iconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer();
		ElectricityParameterData singleton = ((EntityQuery)(ref m_ElectricityParameterQuery)).GetSingleton<ElectricityParameterData>();
		JobHandle val = JobChunkExtensions.ScheduleParallel<NetEdgeNotificationJob>(new NetEdgeNotificationJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityNodeConnectionType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityNodeConnection>(ref __TypeHandle.__Game_Simulation_ElectricityNodeConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedFlowEdges = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SourceNode = m_ElectricityFlowSystem.sourceNode,
			m_SinkNode = m_ElectricityFlowSystem.sinkNode,
			m_ElectricityParameters = singleton,
			m_IconCommandBuffer = iconCommandBuffer
		}, m_EdgeQuery, ((SystemBase)this).Dependency);
		JobHandle dependency = JobChunkExtensions.ScheduleParallel<BuildingNotificationJob>(new BuildingNotificationJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityBuildingConnectionType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityBuildingConnection>(ref __TypeHandle.__Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedFlowEdges = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SourceNode = m_ElectricityFlowSystem.sourceNode,
			m_ElectricityParameters = singleton,
			m_IconCommandBuffer = iconCommandBuffer
		}, m_BuildingQuery, val);
		((SystemBase)this).Dependency = dependency;
		m_IconCommandSystem.AddCommandBufferWriter(((SystemBase)this).Dependency);
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
	public ElectricityStatusSystem()
	{
	}
}

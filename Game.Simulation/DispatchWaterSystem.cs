using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Common;
using Game.Notifications;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class DispatchWaterSystem : GameSystemBase
{
	[BurstCompile]
	private struct DispatchWaterJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<WaterPipeBuildingConnection> m_BuildingConnectionType;

		public ComponentTypeHandle<WaterConsumer> m_ConsumerType;

		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentLookup<WaterPipeNodeConnection> m_NodeConnections;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> m_FlowEdges;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> m_FlowConnections;

		public IconCommandBuffer m_IconCommandBuffer;

		public WaterPipeParameterData m_Parameters;

		public BuildingEfficiencyParameterData m_EfficiencyParameters;

		public Entity m_SinkNode;

		public RandomSeed m_RandomSeed;

		public bool m_FreshConsumptionDisabled;

		public bool m_SewageConsumptionDisabled;

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
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Building> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
			NativeArray<WaterPipeBuildingConnection> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPipeBuildingConnection>(ref m_BuildingConnectionType);
			NativeArray<WaterConsumer> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterConsumer>(ref m_ConsumerType);
			BufferAccessor<Efficiency> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			WaterPipeNodeConnection waterPipeNodeConnection = default(WaterPipeNodeConnection);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				ref WaterConsumer reference = ref CollectionUtils.ElementAt<WaterConsumer>(nativeArray4, i);
				int num = 0;
				int fulfilledSewage = 0;
				float num2 = 0f;
				WaterPipeEdgeFlags waterPipeEdgeFlags = WaterPipeEdgeFlags.None;
				if (nativeArray3.Length != 0)
				{
					if (nativeArray3[i].m_ConsumerEdge != Entity.Null)
					{
						WaterPipeEdge waterPipeEdge = m_FlowEdges[nativeArray3[i].m_ConsumerEdge];
						num = waterPipeEdge.m_FreshFlow;
						fulfilledSewage = waterPipeEdge.m_SewageFlow;
						num2 = waterPipeEdge.m_FreshPollution;
						waterPipeEdgeFlags = waterPipeEdge.m_Flags;
					}
					else
					{
						Debug.LogError((object)"WaterBuildingConnection is missing consumer edge!");
					}
				}
				else
				{
					Entity roadEdge = nativeArray2[i].m_RoadEdge;
					if (roadEdge != Entity.Null && m_NodeConnections.TryGetComponent(roadEdge, ref waterPipeNodeConnection) && WaterPipeGraphUtils.TryGetFlowEdge(waterPipeNodeConnection.m_WaterPipeNode, m_SinkNode, ref m_FlowConnections, ref m_FlowEdges, out WaterPipeEdge edge))
					{
						if (edge.m_FreshCapacity == edge.m_FreshFlow)
						{
							num = reference.m_WantedConsumption;
						}
						else if (edge.m_FreshCapacity > 0)
						{
							float num3 = (float)edge.m_FreshFlow / (float)edge.m_FreshCapacity;
							num = (int)math.floor((float)reference.m_WantedConsumption * num3);
						}
						if (edge.m_SewageCapacity == edge.m_SewageFlow)
						{
							fulfilledSewage = reference.m_WantedConsumption;
						}
						else if (edge.m_SewageCapacity > 0)
						{
							float num4 = (float)edge.m_SewageFlow / (float)edge.m_SewageCapacity;
							fulfilledSewage = (int)math.floor((float)reference.m_WantedConsumption * num4);
						}
						num2 = edge.m_FreshPollution;
						waterPipeEdgeFlags = edge.m_Flags;
					}
				}
				if (m_FreshConsumptionDisabled)
				{
					num = reference.m_WantedConsumption;
					waterPipeEdgeFlags &= ~(WaterPipeEdgeFlags.WaterShortage | WaterPipeEdgeFlags.WaterDisconnected);
				}
				if (m_SewageConsumptionDisabled)
				{
					fulfilledSewage = reference.m_WantedConsumption;
					waterPipeEdgeFlags &= ~(WaterPipeEdgeFlags.SewageBackup | WaterPipeEdgeFlags.SewageDisconnected);
				}
				reference.m_FulfilledFresh = num;
				reference.m_FulfilledSewage = fulfilledSewage;
				bool flag = reference.m_FulfilledFresh < reference.m_WantedConsumption;
				bool flag2 = reference.m_FulfilledSewage < reference.m_WantedConsumption;
				HandleCooldown(nativeArray[i], m_Parameters.m_WaterNotification, flag, ref reference.m_FreshCooldownCounter, ref random);
				HandleCooldown(nativeArray[i], m_Parameters.m_SewageNotification, flag2, ref reference.m_SewageCooldownCounter, ref random);
				bool flag3 = reference.m_Pollution > m_Parameters.m_MaxToleratedPollution;
				reference.m_Pollution = ((num > 0) ? num2 : 0f);
				if (reference.m_WantedConsumption == 0)
				{
					flag = (waterPipeEdgeFlags & (WaterPipeEdgeFlags.WaterShortage | WaterPipeEdgeFlags.WaterDisconnected)) != 0;
					flag2 = (waterPipeEdgeFlags & (WaterPipeEdgeFlags.SewageBackup | WaterPipeEdgeFlags.SewageDisconnected)) != 0;
				}
				reference.m_Flags = WaterConsumerFlags.None;
				if (!flag)
				{
					reference.m_Flags |= WaterConsumerFlags.WaterConnected;
				}
				if (!flag2)
				{
					reference.m_Flags |= WaterConsumerFlags.SewageConnected;
				}
				if (reference.m_Pollution > m_Parameters.m_MaxToleratedPollution)
				{
					if (!flag3)
					{
						m_IconCommandBuffer.Add(nativeArray[i], m_Parameters.m_DirtyWaterNotification, IconPriority.Problem);
					}
				}
				else if (flag3)
				{
					m_IconCommandBuffer.Remove(nativeArray[i], m_Parameters.m_DirtyWaterNotification);
				}
				if (bufferAccessor.Length != 0)
				{
					float efficiency = 1f - m_EfficiencyParameters.m_WaterPenalty * math.saturate((float)(int)reference.m_FreshCooldownCounter / m_EfficiencyParameters.m_WaterPenaltyDelay);
					BuildingUtils.SetEfficiencyFactor(bufferAccessor[i], EfficiencyFactor.WaterSupply, efficiency);
					float efficiency2 = 1f - m_EfficiencyParameters.m_WaterPollutionPenalty * math.round(reference.m_Pollution * 100f) / 100f;
					BuildingUtils.SetEfficiencyFactor(bufferAccessor[i], EfficiencyFactor.DirtyWater, efficiency2);
					float efficiency3 = 1f - m_EfficiencyParameters.m_SewagePenalty * math.saturate((float)(int)reference.m_SewageCooldownCounter / m_EfficiencyParameters.m_SewagePenaltyDelay);
					BuildingUtils.SetEfficiencyFactor(bufferAccessor[i], EfficiencyFactor.SewageHandling, efficiency3);
				}
			}
		}

		private void HandleCooldown(Entity building, Entity notificationPrefab, bool enabled, ref byte cooldown, ref Random random)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			bool flag = cooldown >= kAlertCooldown;
			if (enabled)
			{
				if (cooldown < byte.MaxValue)
				{
					cooldown++;
				}
				if (!flag && cooldown >= kAlertCooldown)
				{
					m_IconCommandBuffer.Add(building, notificationPrefab, IconPriority.Problem);
				}
			}
			else
			{
				cooldown = 0;
				if (flag)
				{
					m_IconCommandBuffer.Remove(building, notificationPrefab);
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
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterPipeBuildingConnection> __Game_Simulation_WaterPipeBuildingConnection_RO_ComponentTypeHandle;

		public ComponentTypeHandle<WaterConsumer> __Game_Buildings_WaterConsumer_RW_ComponentTypeHandle;

		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<WaterPipeNodeConnection> __Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> __Game_Simulation_WaterPipeEdge_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Simulation_WaterPipeBuildingConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPipeBuildingConnection>(true);
			__Game_Buildings_WaterConsumer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterConsumer>(false);
			__Game_Buildings_Efficiency_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(false);
			__Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeNodeConnection>(true);
			__Game_Simulation_WaterPipeEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeEdge>(true);
			__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(true);
		}
	}

	public static readonly short kAlertCooldown = 2;

	public static readonly short kHealthPenaltyCooldown = 10;

	private const float kNotificationMaxDelay = 2f;

	private WaterPipeFlowSystem m_WaterPipeFlowSystem;

	private IconCommandSystem m_IconCommandSystem;

	private EntityQuery m_ConsumerQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1010455350_0;

	private EntityQuery __query_1010455350_1;

	public bool freshConsumptionDisabled { get; set; }

	public bool sewageConsumptionDisabled { get; set; }

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 62;
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
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_WaterPipeFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterPipeFlowSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_ConsumerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadWrite<WaterConsumer>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_ConsumerQuery);
		((ComponentSystemBase)this).RequireForUpdate<WaterPipeParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<BuildingEfficiencyParameterData>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		if (m_WaterPipeFlowSystem.ready)
		{
			DispatchWaterJob dispatchWaterJob = new DispatchWaterJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingConnectionType = InternalCompilerInterface.GetComponentTypeHandle<WaterPipeBuildingConnection>(ref __TypeHandle.__Game_Simulation_WaterPipeBuildingConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ConsumerType = InternalCompilerInterface.GetComponentTypeHandle<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeConnections = InternalCompilerInterface.GetComponentLookup<WaterPipeNodeConnection>(ref __TypeHandle.__Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FlowEdges = InternalCompilerInterface.GetComponentLookup<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FlowConnections = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer(),
				m_Parameters = ((EntityQuery)(ref __query_1010455350_0)).GetSingleton<WaterPipeParameterData>(),
				m_EfficiencyParameters = ((EntityQuery)(ref __query_1010455350_1)).GetSingleton<BuildingEfficiencyParameterData>(),
				m_SinkNode = m_WaterPipeFlowSystem.sinkNode,
				m_RandomSeed = RandomSeed.Next(),
				m_FreshConsumptionDisabled = freshConsumptionDisabled,
				m_SewageConsumptionDisabled = sewageConsumptionDisabled
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<DispatchWaterJob>(dispatchWaterJob, m_ConsumerQuery, ((SystemBase)this).Dependency);
			m_IconCommandSystem.AddCommandBufferWriter(((SystemBase)this).Dependency);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<WaterPipeParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1010455350_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<BuildingEfficiencyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1010455350_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public DispatchWaterSystem()
	{
	}
}

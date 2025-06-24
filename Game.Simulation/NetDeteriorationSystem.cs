using System.Runtime.CompilerServices;
using Colossal.Collections;
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
using Unity.Mathematics;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class NetDeteriorationSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateLaneConditionJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<LaneDeteriorationData> m_LaneDeteriorationData;

		public ComponentTypeHandle<LaneCondition> m_LaneConditionType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			float num = 1f / (float)kUpdatesPerDay;
			NativeArray<LaneCondition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LaneCondition>(ref m_LaneConditionType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				LaneCondition laneCondition = nativeArray[i];
				PrefabRef prefabRef = nativeArray2[i];
				LaneDeteriorationData laneDeteriorationData = m_LaneDeteriorationData[prefabRef.m_Prefab];
				laneCondition.m_Wear = math.min(laneCondition.m_Wear + num * laneDeteriorationData.m_TimeFactor, 10f);
				nativeArray[i] = laneCondition;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateNetConditionJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Native> m_NativeType;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubLane> m_SubLaneType;

		public ComponentTypeHandle<MaintenanceConsumer> m_MaintenanceConsumerType;

		public ComponentTypeHandle<NetCondition> m_NetConditionType;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> m_MaintenanceRequestData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<LaneCondition> m_LaneConditionData;

		[ReadOnly]
		public EntityArchetype m_MaintenanceRequestArchetype;

		public ParallelWriter m_CommandBuffer;

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
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<NetCondition> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetCondition>(ref m_NetConditionType);
			NativeArray<MaintenanceConsumer> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MaintenanceConsumer>(ref m_MaintenanceConsumerType);
			BufferAccessor<Game.Net.SubLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.SubLane>(ref m_SubLaneType);
			if (((ArchetypeChunk)(ref chunk)).Has<Native>(ref m_NativeType))
			{
				LaneCondition laneCondition = default(LaneCondition);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					ref NetCondition reference = ref CollectionUtils.ElementAt<NetCondition>(nativeArray2, i);
					DynamicBuffer<Game.Net.SubLane> val = bufferAccessor[i];
					reference.m_Wear = float2.op_Implicit(0f);
					for (int j = 0; j < val.Length; j++)
					{
						Entity subLane = val[j].m_SubLane;
						if (m_LaneConditionData.TryGetComponent(subLane, ref laneCondition))
						{
							laneCondition.m_Wear = 0f;
							m_LaneConditionData[subLane] = laneCondition;
						}
					}
				}
				return;
			}
			LaneCondition laneCondition2 = default(LaneCondition);
			EdgeLane edgeLane = default(EdgeLane);
			for (int k = 0; k < nativeArray2.Length; k++)
			{
				Entity entity = nativeArray[k];
				ref NetCondition reference2 = ref CollectionUtils.ElementAt<NetCondition>(nativeArray2, k);
				DynamicBuffer<Game.Net.SubLane> val2 = bufferAccessor[k];
				reference2.m_Wear = float2.op_Implicit(0f);
				for (int l = 0; l < val2.Length; l++)
				{
					Entity subLane2 = val2[l].m_SubLane;
					if (m_LaneConditionData.TryGetComponent(subLane2, ref laneCondition2))
					{
						if (m_EdgeLaneData.TryGetComponent(subLane2, ref edgeLane))
						{
							reference2.m_Wear = math.select(reference2.m_Wear, float2.op_Implicit(laneCondition2.m_Wear), new bool2(math.any(edgeLane.m_EdgeDelta == 0f), math.any(edgeLane.m_EdgeDelta == 1f)) & (laneCondition2.m_Wear > reference2.m_Wear));
						}
						else
						{
							reference2.m_Wear = math.max(reference2.m_Wear, float2.op_Implicit(laneCondition2.m_Wear));
						}
					}
				}
				if (nativeArray3.Length != 0)
				{
					MaintenanceConsumer maintenanceConsumer = nativeArray3[k];
					RequestMaintenanceIfNeeded(unfilteredChunkIndex, entity, reference2, ref maintenanceConsumer);
					nativeArray3[k] = maintenanceConsumer;
				}
			}
		}

		private void RequestMaintenanceIfNeeded(int jobIndex, Entity entity, NetCondition condition, ref MaintenanceConsumer maintenanceConsumer)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			int maintenancePriority = GetMaintenancePriority(condition);
			MaintenanceRequest maintenanceRequest = default(MaintenanceRequest);
			if (maintenancePriority > 0 && (!m_MaintenanceRequestData.TryGetComponent(maintenanceConsumer.m_Request, ref maintenanceRequest) || (!(maintenanceRequest.m_Target == entity) && maintenanceRequest.m_DispatchIndex != maintenanceConsumer.m_DispatchIndex)))
			{
				maintenanceConsumer.m_Request = Entity.Null;
				maintenanceConsumer.m_DispatchIndex = 0;
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_MaintenanceRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MaintenanceRequest>(jobIndex, val, new MaintenanceRequest(entity, maintenancePriority));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(32u));
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<LaneDeteriorationData> __Game_Prefabs_LaneDeteriorationData_RO_ComponentLookup;

		public ComponentTypeHandle<LaneCondition> __Game_Net_LaneCondition_RW_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Native> __Game_Common_Native_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferTypeHandle;

		public ComponentTypeHandle<MaintenanceConsumer> __Game_Simulation_MaintenanceConsumer_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetCondition> __Game_Net_NetCondition_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> __Game_Simulation_MaintenanceRequest_RO_ComponentLookup;

		public ComponentLookup<LaneCondition> __Game_Net_LaneCondition_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_LaneDeteriorationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneDeteriorationData>(true);
			__Game_Net_LaneCondition_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LaneCondition>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Native_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Native>(true);
			__Game_Net_SubLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.SubLane>(true);
			__Game_Simulation_MaintenanceConsumer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MaintenanceConsumer>(false);
			__Game_Net_NetCondition_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCondition>(false);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Simulation_MaintenanceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceRequest>(true);
			__Game_Net_LaneCondition_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneCondition>(false);
		}
	}

	public static readonly int kUpdatesPerDay = 16;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_LaneQuery;

	private EntityQuery m_EdgeQuery;

	private EntityArchetype m_MaintenanceRequestArchetype;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
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
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_LaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<LaneCondition>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_EdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<NetCondition>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_MaintenanceRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<MaintenanceRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_LaneQuery, m_EdgeQuery });
		Assert.IsTrue((long)(262144 / kUpdatesPerDay) >= 512L);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		((EntityQuery)(ref m_LaneQuery)).ResetFilter();
		((EntityQuery)(ref m_LaneQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16)));
		((EntityQuery)(ref m_EdgeQuery)).ResetFilter();
		((EntityQuery)(ref m_EdgeQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16)));
		UpdateLaneConditionJob updateLaneConditionJob = new UpdateLaneConditionJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneDeteriorationData = InternalCompilerInterface.GetComponentLookup<LaneDeteriorationData>(ref __TypeHandle.__Game_Prefabs_LaneDeteriorationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneConditionType = InternalCompilerInterface.GetComponentTypeHandle<LaneCondition>(ref __TypeHandle.__Game_Net_LaneCondition_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		UpdateNetConditionJob updateNetConditionJob = new UpdateNetConditionJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NativeType = InternalCompilerInterface.GetComponentTypeHandle<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubLaneType = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceConsumerType = InternalCompilerInterface.GetComponentTypeHandle<MaintenanceConsumer>(ref __TypeHandle.__Game_Simulation_MaintenanceConsumer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetConditionType = InternalCompilerInterface.GetComponentTypeHandle<NetCondition>(ref __TypeHandle.__Game_Net_NetCondition_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceRequestData = InternalCompilerInterface.GetComponentLookup<MaintenanceRequest>(ref __TypeHandle.__Game_Simulation_MaintenanceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneConditionData = InternalCompilerInterface.GetComponentLookup<LaneCondition>(ref __TypeHandle.__Game_Net_LaneCondition_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceRequestArchetype = m_MaintenanceRequestArchetype
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		updateNetConditionJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		UpdateNetConditionJob updateNetConditionJob2 = updateNetConditionJob;
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateLaneConditionJob>(updateLaneConditionJob, m_LaneQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<UpdateNetConditionJob>(updateNetConditionJob2, m_EdgeQuery, val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		((SystemBase)this).Dependency = val3;
	}

	public static int GetMaintenancePriority(NetCondition condition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return (int)(math.cmax(condition.m_Wear) / 10f * 100f) - 10;
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
	public NetDeteriorationSystem()
	{
	}
}

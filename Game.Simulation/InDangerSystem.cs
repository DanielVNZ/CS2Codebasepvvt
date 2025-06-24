using System.Runtime.CompilerServices;
using Game.Common;
using Game.Events;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class InDangerSystem : GameSystemBase
{
	[BurstCompile]
	private struct InDangerJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<InDanger> m_InDangerType;

		[ReadOnly]
		public ComponentLookup<Duration> m_DurationData;

		[ReadOnly]
		public ComponentLookup<EvacuationRequest> m_EvacuationRequestData;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public EntityArchetype m_EvacuationRequestArchetype;

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
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<InDanger> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<InDanger>(ref m_InDangerType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				InDanger inDanger = nativeArray2[i];
				if (!IsStillInDanger(ref inDanger))
				{
					inDanger.m_Flags = (DangerFlags)0u;
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<InDanger>(unfilteredChunkIndex, val);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(unfilteredChunkIndex, val, default(EffectsUpdated));
				}
				if ((inDanger.m_Flags & (DangerFlags.Evacuate | DangerFlags.UseTransport | DangerFlags.WaitingCitizens)) == (DangerFlags.Evacuate | DangerFlags.UseTransport | DangerFlags.WaitingCitizens))
				{
					RequestEvacuationIfNeeded(unfilteredChunkIndex, val, ref inDanger);
				}
				nativeArray2[i] = inDanger;
			}
		}

		private bool IsStillInDanger(ref InDanger inDanger)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (m_SimulationFrame >= inDanger.m_EndFrame)
			{
				return false;
			}
			if (!m_DurationData.HasComponent(inDanger.m_Event))
			{
				return false;
			}
			Duration duration = m_DurationData[inDanger.m_Event];
			return m_SimulationFrame < duration.m_EndFrame;
		}

		private void RequestEvacuationIfNeeded(int jobIndex, Entity entity, ref InDanger inDanger)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (!m_EvacuationRequestData.HasComponent(inDanger.m_EvacuationRequest))
			{
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_EvacuationRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<EvacuationRequest>(jobIndex, val, new EvacuationRequest(entity, 1f));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(4u));
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

		public ComponentTypeHandle<InDanger> __Game_Events_InDanger_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Duration> __Game_Events_Duration_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EvacuationRequest> __Game_Simulation_EvacuationRequest_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Events_InDanger_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InDanger>(false);
			__Game_Events_Duration_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Duration>(true);
			__Game_Simulation_EvacuationRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EvacuationRequest>(true);
		}
	}

	public const uint UPDATE_INTERVAL = 64u;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_InDangerQuery;

	private EntityArchetype m_EvacuationRequestArchetype;

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
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_InDangerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<InDanger>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EvacuationRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<EvacuationRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_InDangerQuery);
		Assert.IsTrue(true);
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
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		InDangerJob inDangerJob = new InDangerJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InDangerType = InternalCompilerInterface.GetComponentTypeHandle<InDanger>(ref __TypeHandle.__Game_Events_InDanger_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DurationData = InternalCompilerInterface.GetComponentLookup<Duration>(ref __TypeHandle.__Game_Events_Duration_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EvacuationRequestData = InternalCompilerInterface.GetComponentLookup<EvacuationRequest>(ref __TypeHandle.__Game_Simulation_EvacuationRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_EvacuationRequestArchetype = m_EvacuationRequestArchetype
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		inDangerJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<InDangerJob>(inDangerJob, m_InDangerQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
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
	public InDangerSystem()
	{
	}
}

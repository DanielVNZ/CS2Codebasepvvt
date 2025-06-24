using System.Runtime.CompilerServices;
using Game.Common;
using Game.Economy;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class ProcessingRequirementSystem : GameSystemBase
{
	[BurstCompile]
	private struct ProcessingRequirementJob : IJobChunk
	{
		[ReadOnly]
		public NativeArray<long> m_ProducedResources;

		[ReadOnly]
		public EntityArchetype m_UnlockEventArchetype;

		public ParallelWriter m_CommandBuffer;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ProcessingRequirementData> m_ProcessingRequirementType;

		public ComponentTypeHandle<UnlockRequirementData> m_UnlockRequirementType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ProcessingRequirementData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ProcessingRequirementData>(ref m_ProcessingRequirementType);
			NativeArray<UnlockRequirementData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<UnlockRequirementData>(ref m_UnlockRequirementType);
			ChunkEntityEnumerator val = default(ChunkEntityEnumerator);
			((ChunkEntityEnumerator)(ref val))._002Ector(useEnabledMask, chunkEnabledMask, ((ArchetypeChunk)(ref chunk)).Count);
			int num = default(int);
			while (((ChunkEntityEnumerator)(ref val)).NextEntityIndex(ref num))
			{
				ProcessingRequirementData processingRequirement = nativeArray2[num];
				UnlockRequirementData unlockRequirement = nativeArray3[num];
				if (ShouldUnlock(processingRequirement, ref unlockRequirement))
				{
					Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex, m_UnlockEventArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Unlock>(unfilteredChunkIndex, val2, new Unlock(nativeArray[num]));
				}
				nativeArray3[num] = unlockRequirement;
			}
		}

		private bool ShouldUnlock(ProcessingRequirementData processingRequirement, ref UnlockRequirementData unlockRequirement)
		{
			long num = ((processingRequirement.m_ResourceType == Resource.NoResource) ? 0 : m_ProducedResources[EconomyUtils.GetResourceIndex(processingRequirement.m_ResourceType)]);
			if (num >= processingRequirement.m_MinimumProducedAmount)
			{
				unlockRequirement.m_Progress = processingRequirement.m_MinimumProducedAmount;
				return true;
			}
			unlockRequirement.m_Progress = (int)num;
			return false;
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
		public ComponentTypeHandle<ProcessingRequirementData> __Game_Prefabs_ProcessingRequirementData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<UnlockRequirementData> __Game_Prefabs_UnlockRequirementData_RW_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_ProcessingRequirementData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ProcessingRequirementData>(true);
			__Game_Prefabs_UnlockRequirementData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UnlockRequirementData>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private ProcessingCompanySystem m_ProcessingCompanySystem;

	private EntityQuery m_RequirementQuery;

	private EntityArchetype m_UnlockEventArchetype;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
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
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ProcessingCompanySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProcessingCompanySystem>();
		m_RequirementQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ProcessingRequirementData>(),
			ComponentType.ReadWrite<UnlockRequirementData>(),
			ComponentType.ReadOnly<Locked>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_RequirementQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		ProcessingRequirementJob processingRequirementJob = new ProcessingRequirementJob
		{
			m_ProducedResources = m_ProcessingCompanySystem.GetProducedResourcesArray(out dependencies),
			m_UnlockEventArchetype = m_UnlockEventArchetype
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		processingRequirementJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		processingRequirementJob.m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		processingRequirementJob.m_ProcessingRequirementType = InternalCompilerInterface.GetComponentTypeHandle<ProcessingRequirementData>(ref __TypeHandle.__Game_Prefabs_ProcessingRequirementData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		processingRequirementJob.m_UnlockRequirementType = InternalCompilerInterface.GetComponentTypeHandle<UnlockRequirementData>(ref __TypeHandle.__Game_Prefabs_UnlockRequirementData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<ProcessingRequirementJob>(processingRequirementJob, m_RequirementQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		m_ProcessingCompanySystem.AddProducedResourcesReader(val2);
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
	public ProcessingRequirementSystem()
	{
	}
}

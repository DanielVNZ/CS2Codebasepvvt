using System.Runtime.CompilerServices;
using Game.Common;
using Game.Input;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Tutorials;

[CompilerGenerated]
public class TutorialControlSchemeActivationSystem : GameSystemBase
{
	[BurstCompile]
	private struct ActivateJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<ControlSchemeActivationData> m_ControlSchemeActivationType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public InputManager.ControlScheme m_ControlScheme;

		public ParallelWriter m_Writer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ControlSchemeActivationData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ControlSchemeActivationData>(ref m_ControlSchemeActivationType);
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if (nativeArray[i].m_ControlScheme == m_ControlScheme)
				{
					((ParallelWriter)(ref m_Writer)).AddComponent<TutorialActivated>(unfilteredChunkIndex, nativeArray2[i]);
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
		public ComponentTypeHandle<ControlSchemeActivationData> __Game_Tutorials_ControlSchemeActivationData_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tutorials_ControlSchemeActivationData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ControlSchemeActivationData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
		}
	}

	protected EntityCommandBufferSystem m_BarrierSystem;

	private EntityQuery m_TutorialQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_BarrierSystem = (EntityCommandBufferSystem)(object)((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_TutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<ControlSchemeActivationData>(),
			ComponentType.Exclude<TutorialCompleted>(),
			ComponentType.Exclude<TutorialActivated>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_TutorialQuery)).IsEmptyIgnoreFilter && InputManager.instance != null)
		{
			ActivateJob activateJob = new ActivateJob
			{
				m_ControlSchemeActivationType = InternalCompilerInterface.GetComponentTypeHandle<ControlSchemeActivationData>(ref __TypeHandle.__Game_Tutorials_ControlSchemeActivationData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ControlScheme = InputManager.instance.activeControlScheme
			};
			EntityCommandBuffer val = m_BarrierSystem.CreateCommandBuffer();
			activateJob.m_Writer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
			ActivateJob activateJob2 = activateJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<ActivateJob>(activateJob2, m_TutorialQuery, ((SystemBase)this).Dependency);
			m_BarrierSystem.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public TutorialControlSchemeActivationSystem()
	{
	}
}

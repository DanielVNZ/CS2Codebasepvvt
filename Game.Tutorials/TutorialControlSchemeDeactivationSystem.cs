using System.Runtime.CompilerServices;
using Game.Input;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Tutorials;

[CompilerGenerated]
public class TutorialControlSchemeDeactivationSystem : TutorialDeactivationSystemBase
{
	[BurstCompile]
	private struct DeactivateJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<ControlSchemeActivationData> m_ControlSchemeDeactivationType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public InputManager.ControlScheme m_ControlScheme;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ControlSchemeActivationData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ControlSchemeActivationData>(ref m_ControlSchemeDeactivationType);
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if (nativeArray[i].m_ControlScheme != m_ControlScheme)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TutorialActivated>(unfilteredChunkIndex, nativeArray2[i]);
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

	private EntityQuery m_PendingTutorialQuery;

	private EntityQuery m_ActiveTutorialQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PendingTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<TutorialActivated>(),
			ComponentType.ReadOnly<ControlSchemeActivationData>(),
			ComponentType.Exclude<TutorialActive>(),
			ComponentType.Exclude<TutorialCompleted>(),
			ComponentType.Exclude<ForceActivation>()
		});
		m_ActiveTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<TutorialActivated>(),
			ComponentType.ReadOnly<ControlSchemeActivationData>(),
			ComponentType.ReadOnly<TutorialActive>(),
			ComponentType.Exclude<TutorialCompleted>(),
			ComponentType.Exclude<ForceActivation>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (InputManager.instance != null)
		{
			if (!((EntityQuery)(ref m_PendingTutorialQuery)).IsEmptyIgnoreFilter)
			{
				CheckDeactivate(m_PendingTutorialQuery);
			}
			if (!((EntityQuery)(ref m_ActiveTutorialQuery)).IsEmptyIgnoreFilter && base.phaseCanDeactivate)
			{
				CheckDeactivate(m_ActiveTutorialQuery);
			}
		}
	}

	private void CheckDeactivate(EntityQuery query)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref query)).IsEmptyIgnoreFilter)
		{
			DeactivateJob deactivateJob = new DeactivateJob
			{
				m_ControlSchemeDeactivationType = InternalCompilerInterface.GetComponentTypeHandle<ControlSchemeActivationData>(ref __TypeHandle.__Game_Tutorials_ControlSchemeActivationData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ControlScheme = InputManager.instance.activeControlScheme
			};
			EntityCommandBuffer val = m_BarrierSystem.CreateCommandBuffer();
			deactivateJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
			DeactivateJob deactivateJob2 = deactivateJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<DeactivateJob>(deactivateJob2, query, ((SystemBase)this).Dependency);
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
	public TutorialControlSchemeDeactivationSystem()
	{
	}
}

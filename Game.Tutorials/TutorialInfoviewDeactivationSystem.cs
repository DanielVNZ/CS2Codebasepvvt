using System.Runtime.CompilerServices;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tutorials;

[CompilerGenerated]
public class TutorialInfoviewDeactivationSystem : TutorialDeactivationSystemBase
{
	[BurstCompile]
	private struct CheckDeactivationJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<InfoviewActivationData> m_ActivationType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public Entity m_Infoview;

		public ParallelWriter m_Buffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<InfoviewActivationData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<InfoviewActivationData>(ref m_ActivationType);
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if (nativeArray[i].m_Infoview != m_Infoview)
				{
					((ParallelWriter)(ref m_Buffer)).RemoveComponent<TutorialActivated>(unfilteredChunkIndex, nativeArray2[i]);
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
		public ComponentTypeHandle<InfoviewActivationData> __Game_Tutorials_InfoviewActivationData_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tutorials_InfoviewActivationData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewActivationData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
		}
	}

	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private EntityQuery m_PendingTutorialQuery;

	private EntityQuery m_ActiveTutorialQuery;

	private TypeHandle __TypeHandle;

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
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_PendingTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<InfoviewActivationData>(),
			ComponentType.ReadOnly<TutorialActivated>(),
			ComponentType.Exclude<TutorialActive>(),
			ComponentType.Exclude<TutorialCompleted>(),
			ComponentType.Exclude<ForceActivation>()
		});
		m_ActiveTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<InfoviewActivationData>(),
			ComponentType.ReadOnly<TutorialActivated>(),
			ComponentType.ReadOnly<TutorialActive>(),
			ComponentType.Exclude<TutorialCompleted>(),
			ComponentType.Exclude<ForceActivation>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_PendingTutorialQuery)).IsEmptyIgnoreFilter)
		{
			CheckDeactivation(m_PendingTutorialQuery);
		}
		if (!((EntityQuery)(ref m_ActiveTutorialQuery)).IsEmptyIgnoreFilter && base.phaseCanDeactivate)
		{
			CheckDeactivation(m_ActiveTutorialQuery);
		}
	}

	private void CheckDeactivation(EntityQuery query)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		CheckDeactivationJob checkDeactivationJob = new CheckDeactivationJob
		{
			m_ActivationType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewActivationData>(ref __TypeHandle.__Game_Tutorials_InfoviewActivationData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Infoview = GetActiveInfoview()
		};
		EntityCommandBuffer val = m_BarrierSystem.CreateCommandBuffer();
		checkDeactivationJob.m_Buffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		CheckDeactivationJob checkDeactivationJob2 = checkDeactivationJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckDeactivationJob>(checkDeactivationJob2, query, ((SystemBase)this).Dependency);
		m_BarrierSystem.AddJobHandleForProducer(((SystemBase)this).Dependency);
	}

	private Entity GetActiveInfoview()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_ToolSystem.activeInfoview == (Object)null)
		{
			return Entity.Null;
		}
		return m_PrefabSystem.GetEntity(m_ToolSystem.activeInfoview);
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
	public TutorialInfoviewDeactivationSystem()
	{
	}
}

using System.Runtime.CompilerServices;
using Game.Common;
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
public class TutorialInfoviewActivationSystem : GameSystemBase
{
	[BurstCompile]
	private struct CheckActivationJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<InfoviewActivationData> m_ActivationType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public Entity m_Infoview;

		public ParallelWriter m_Writer;

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
				if (nativeArray[i].m_Infoview == m_Infoview)
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

	protected EntityCommandBufferSystem m_BarrierSystem;

	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private EntityQuery m_TutorialQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_BarrierSystem = (EntityCommandBufferSystem)(object)((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_TutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<InfoviewActivationData>(),
			ComponentType.Exclude<TutorialActivated>(),
			ComponentType.Exclude<TutorialCompleted>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_TutorialQuery)).IsEmptyIgnoreFilter && (Object)(object)m_ToolSystem.activeInfoview != (Object)null)
		{
			CheckActivationJob checkActivationJob = new CheckActivationJob
			{
				m_ActivationType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewActivationData>(ref __TypeHandle.__Game_Tutorials_InfoviewActivationData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Infoview = m_PrefabSystem.GetEntity(m_ToolSystem.infoview)
			};
			EntityCommandBuffer val = m_BarrierSystem.CreateCommandBuffer();
			checkActivationJob.m_Writer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
			CheckActivationJob checkActivationJob2 = checkActivationJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckActivationJob>(checkActivationJob2, m_TutorialQuery, ((SystemBase)this).Dependency);
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
	public TutorialInfoviewActivationSystem()
	{
	}
}

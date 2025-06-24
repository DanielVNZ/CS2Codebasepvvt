using System.Runtime.CompilerServices;
using Colossal.Entities;
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
public class TutorialObjectSelectionDeactivationSystem : TutorialDeactivationSystemBase
{
	[BurstCompile]
	private struct CheckTutorialsJob : IJobChunk
	{
		[ReadOnly]
		public BufferTypeHandle<ObjectSelectionActivationData> m_DeactivationDataType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public Entity m_Selection;

		public bool m_Tool;

		public ParallelWriter m_Buffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<ObjectSelectionActivationData> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ObjectSelectionActivationData>(ref m_DeactivationDataType);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				if (m_Selection == Entity.Null || ShouldDeactivate(bufferAccessor[i]))
				{
					((ParallelWriter)(ref m_Buffer)).RemoveComponent<TutorialActivated>(unfilteredChunkIndex, nativeArray[i]);
				}
			}
		}

		private bool ShouldDeactivate(DynamicBuffer<ObjectSelectionActivationData> selections)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < selections.Length; i++)
			{
				if (selections[i].m_Prefab == m_Selection && (selections[i].m_AllowTool || !m_Tool))
				{
					return false;
				}
			}
			return true;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<ObjectSelectionActivationData> __Game_Tutorials_ObjectSelectionActivationData_RO_BufferTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tutorials_ObjectSelectionActivationData_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ObjectSelectionActivationData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
		}
	}

	private PrefabSystem m_PrefabSystem;

	private ObjectToolSystem m_ObjectToolSystem;

	private NetToolSystem m_NetToolSystem;

	private ToolSystem m_ToolSystem;

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
			ComponentType.ReadOnly<ObjectSelectionActivationData>(),
			ComponentType.ReadOnly<TutorialActivated>(),
			ComponentType.Exclude<TutorialActive>(),
			ComponentType.Exclude<TutorialCompleted>(),
			ComponentType.Exclude<ForceActivation>()
		});
		m_ActiveTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<ObjectSelectionActivationData>(),
			ComponentType.ReadOnly<TutorialActivated>(),
			ComponentType.ReadOnly<TutorialActive>(),
			ComponentType.Exclude<TutorialCompleted>(),
			ComponentType.Exclude<ForceActivation>()
		});
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_NetToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NetToolSystem>();
		m_ObjectToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ObjectToolSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_PendingTutorialQuery)).IsEmptyIgnoreFilter || !((EntityQuery)(ref m_ActiveTutorialQuery)).IsEmptyIgnoreFilter)
		{
			bool tool;
			Entity selection = GetSelection(out tool);
			if (!((EntityQuery)(ref m_PendingTutorialQuery)).IsEmptyIgnoreFilter)
			{
				CheckDeactivate(m_PendingTutorialQuery, selection, tool);
			}
			if (!((EntityQuery)(ref m_ActiveTutorialQuery)).IsEmptyIgnoreFilter && base.phaseCanDeactivate)
			{
				CheckDeactivate(m_ActiveTutorialQuery, selection, tool);
			}
		}
	}

	private Entity GetSelection(out bool tool)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		tool = true;
		PrefabRef prefabRef = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, m_ToolSystem.selected, ref prefabRef))
		{
			tool = false;
			return prefabRef.m_Prefab;
		}
		if (m_ToolSystem.activeTool == m_ObjectToolSystem && (Object)(object)m_ObjectToolSystem.prefab != (Object)null)
		{
			return m_PrefabSystem.GetEntity(m_ObjectToolSystem.prefab);
		}
		if (m_ToolSystem.activeTool == m_NetToolSystem && (Object)(object)m_NetToolSystem.prefab != (Object)null)
		{
			return m_PrefabSystem.GetEntity(m_NetToolSystem.prefab);
		}
		tool = false;
		return Entity.Null;
	}

	private void CheckDeactivate(EntityQuery query, Entity selection, bool tool)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		CheckTutorialsJob checkTutorialsJob = new CheckTutorialsJob
		{
			m_DeactivationDataType = InternalCompilerInterface.GetBufferTypeHandle<ObjectSelectionActivationData>(ref __TypeHandle.__Game_Tutorials_ObjectSelectionActivationData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Selection = selection,
			m_Tool = tool
		};
		EntityCommandBuffer val = m_BarrierSystem.CreateCommandBuffer();
		checkTutorialsJob.m_Buffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		CheckTutorialsJob checkTutorialsJob2 = checkTutorialsJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckTutorialsJob>(checkTutorialsJob2, query, ((SystemBase)this).Dependency);
		m_BarrierSystem.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public TutorialObjectSelectionDeactivationSystem()
	{
	}
}

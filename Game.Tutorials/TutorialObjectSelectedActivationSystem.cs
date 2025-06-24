using System.Runtime.CompilerServices;
using Colossal.Entities;
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
public class TutorialObjectSelectedActivationSystem : GameSystemBase
{
	[BurstCompile]
	private struct ActivateJob : IJobChunk
	{
		[ReadOnly]
		public BufferTypeHandle<ObjectSelectionActivationData> m_ActivationDataType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public Entity m_Selection;

		public bool m_Tool;

		public ParallelWriter m_Writer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<ObjectSelectionActivationData> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ObjectSelectionActivationData>(ref m_ActivationDataType);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				if (Check(bufferAccessor[i]))
				{
					((ParallelWriter)(ref m_Writer)).AddComponent<TutorialActivated>(unfilteredChunkIndex, nativeArray[i]);
				}
			}
		}

		private bool Check(DynamicBuffer<ObjectSelectionActivationData> datas)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < datas.Length; i++)
			{
				if (datas[i].m_Prefab == m_Selection && (!m_Tool || datas[i].m_AllowTool))
				{
					return true;
				}
			}
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

	protected EntityCommandBufferSystem m_BarrierSystem;

	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private ObjectToolSystem m_ObjectToolSystem;

	private NetToolSystem m_NetToolSystem;

	private AreaToolSystem m_AreaToolSystem;

	private RouteToolSystem m_RouteToolSystem;

	private EntityQuery m_TutorialQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_BarrierSystem = (EntityCommandBufferSystem)(object)((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_ObjectToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ObjectToolSystem>();
		m_NetToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NetToolSystem>();
		m_AreaToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaToolSystem>();
		m_RouteToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RouteToolSystem>();
		m_TutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ObjectSelectionActivationData>(),
			ComponentType.Exclude<TutorialActivated>(),
			ComponentType.Exclude<TutorialCompleted>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_TutorialQuery)).IsEmptyIgnoreFilter)
		{
			bool tool;
			Entity selection = GetSelection(out tool);
			if (selection != Entity.Null)
			{
				ActivateJob activateJob = new ActivateJob
				{
					m_ActivationDataType = InternalCompilerInterface.GetBufferTypeHandle<ObjectSelectionActivationData>(ref __TypeHandle.__Game_Tutorials_ObjectSelectionActivationData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_Selection = selection,
					m_Tool = tool
				};
				EntityCommandBuffer val = m_BarrierSystem.CreateCommandBuffer();
				activateJob.m_Writer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
				ActivateJob activateJob2 = activateJob;
				((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<ActivateJob>(activateJob2, m_TutorialQuery, ((SystemBase)this).Dependency);
				m_BarrierSystem.AddJobHandleForProducer(((SystemBase)this).Dependency);
			}
		}
	}

	private Entity GetSelection(out bool tool)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
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
		if (m_ToolSystem.activeTool == m_AreaToolSystem && (Object)(object)m_AreaToolSystem.prefab != (Object)null)
		{
			return m_PrefabSystem.GetEntity(m_AreaToolSystem.prefab);
		}
		if (m_ToolSystem.activeTool == m_RouteToolSystem && (Object)(object)m_RouteToolSystem.prefab != (Object)null)
		{
			return m_PrefabSystem.GetEntity(m_RouteToolSystem.prefab);
		}
		tool = false;
		return Entity.Null;
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
	public TutorialObjectSelectedActivationSystem()
	{
	}
}

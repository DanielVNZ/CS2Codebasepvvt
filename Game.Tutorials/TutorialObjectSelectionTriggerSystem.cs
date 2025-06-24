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
using UnityEngine.Scripting;

namespace Game.Tutorials;

[CompilerGenerated]
public class TutorialObjectSelectionTriggerSystem : TutorialTriggerSystemBase
{
	[BurstCompile]
	private struct CheckSelectionJob : IJobChunk
	{
		[ReadOnly]
		public BufferLookup<ForceUIGroupUnlockData> m_ForcedUnlockDataFromEntity;

		[ReadOnly]
		public BufferLookup<UnlockRequirement> m_UnlockRequirementFromEntity;

		[ReadOnly]
		public BufferTypeHandle<ObjectSelectionTriggerData> m_TriggerType;

		[ReadOnly]
		public EntityArchetype m_UnlockEventArchetype;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public Entity m_Selection;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<ObjectSelectionTriggerData> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ObjectSelectionTriggerData>(ref m_TriggerType);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				DynamicBuffer<ObjectSelectionTriggerData> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					ObjectSelectionTriggerData objectSelectionTriggerData = val[j];
					if (objectSelectionTriggerData.m_Prefab == m_Selection)
					{
						if (objectSelectionTriggerData.m_GoToPhase != Entity.Null)
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TutorialNextPhase>(unfilteredChunkIndex, nativeArray[i], new TutorialNextPhase
							{
								m_NextPhase = objectSelectionTriggerData.m_GoToPhase
							});
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TriggerPreCompleted>(unfilteredChunkIndex, nativeArray[i]);
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TriggerCompleted>(unfilteredChunkIndex, nativeArray[i]);
							TutorialSystem.ManualUnlock(nativeArray[i], m_UnlockEventArchetype, ref m_ForcedUnlockDataFromEntity, ref m_UnlockRequirementFromEntity, m_CommandBuffer, unfilteredChunkIndex);
						}
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TriggerPreCompleted>(unfilteredChunkIndex, nativeArray[i]);
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TutorialNextPhase>(unfilteredChunkIndex, nativeArray[i]);
					}
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
		public BufferLookup<ForceUIGroupUnlockData> __Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<UnlockRequirement> __Game_Prefabs_UnlockRequirement_RO_BufferLookup;

		[ReadOnly]
		public BufferTypeHandle<ObjectSelectionTriggerData> __Game_Tutorials_ObjectSelectionTriggerData_RO_BufferTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

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
			__Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ForceUIGroupUnlockData>(true);
			__Game_Prefabs_UnlockRequirement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<UnlockRequirement>(true);
			__Game_Tutorials_ObjectSelectionTriggerData_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ObjectSelectionTriggerData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
		}
	}

	private ToolSystem m_ToolSystem;

	private EntityArchetype m_UnlockEventArchetype;

	private Entity m_LastSelection;

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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_ActiveTriggerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ObjectSelectionTriggerData>(),
			ComponentType.ReadOnly<TriggerActive>(),
			ComponentType.Exclude<TriggerCompleted>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_ActiveTriggerQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		base.OnUpdate();
		if (base.triggersChanged)
		{
			m_LastSelection = m_ToolSystem.selected;
		}
		if (m_ToolSystem.selected != Entity.Null && m_ToolSystem.selected != m_LastSelection)
		{
			m_LastSelection = m_ToolSystem.selected;
			PrefabRef prefabRef = default(PrefabRef);
			if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, m_ToolSystem.selected, ref prefabRef))
			{
				CheckSelectionJob checkSelectionJob = new CheckSelectionJob
				{
					m_ForcedUnlockDataFromEntity = InternalCompilerInterface.GetBufferLookup<ForceUIGroupUnlockData>(ref __TypeHandle.__Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_UnlockRequirementFromEntity = InternalCompilerInterface.GetBufferLookup<UnlockRequirement>(ref __TypeHandle.__Game_Prefabs_UnlockRequirement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TriggerType = InternalCompilerInterface.GetBufferTypeHandle<ObjectSelectionTriggerData>(ref __TypeHandle.__Game_Tutorials_ObjectSelectionTriggerData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_UnlockEventArchetype = m_UnlockEventArchetype,
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_Selection = prefabRef.m_Prefab
				};
				EntityCommandBuffer val = m_BarrierSystem.CreateCommandBuffer();
				checkSelectionJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
				CheckSelectionJob checkSelectionJob2 = checkSelectionJob;
				((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckSelectionJob>(checkSelectionJob2, m_ActiveTriggerQuery, ((SystemBase)this).Dependency);
				((EntityCommandBufferSystem)m_BarrierSystem).AddJobHandleForProducer(((SystemBase)this).Dependency);
			}
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
	public TutorialObjectSelectionTriggerSystem()
	{
	}
}

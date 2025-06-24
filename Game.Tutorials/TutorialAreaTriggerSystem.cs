using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Tutorials;

[CompilerGenerated]
public class TutorialAreaTriggerSystem : TutorialTriggerSystemBase
{
	[BurstCompile]
	private struct CheckModifiedAreasJob : IJobChunk
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_AreaModificationChunks;

		[ReadOnly]
		public BufferTypeHandle<AreaTriggerData> m_TriggerType;

		[ReadOnly]
		public BufferLookup<UnlockRequirement> m_UnlockRequirementFromEntity;

		[ReadOnly]
		public BufferLookup<ForceUIGroupUnlockData> m_ForcedUnlockDataFromEntity;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Updated> m_UpdatedType;

		[ReadOnly]
		public EntityArchetype m_UnlockEventArchetype;

		public ParallelWriter m_CommandBuffer;

		public bool m_FirstTimeCheck;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<AreaTriggerData> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<AreaTriggerData>(ref m_TriggerType);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				if (Check(bufferAccessor[i]))
				{
					if (m_FirstTimeCheck)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TriggerPreCompleted>(unfilteredChunkIndex, nativeArray[i]);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TriggerCompleted>(unfilteredChunkIndex, nativeArray[i]);
					}
					TutorialSystem.ManualUnlock(nativeArray[i], m_UnlockEventArchetype, ref m_ForcedUnlockDataFromEntity, ref m_UnlockRequirementFromEntity, m_CommandBuffer, unfilteredChunkIndex);
				}
			}
		}

		private bool Check(DynamicBuffer<AreaTriggerData> triggerDatas)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_AreaModificationChunks.Length; i++)
			{
				AreaTriggerFlags areaTriggerFlags = (AreaTriggerFlags)0;
				ArchetypeChunk val = m_AreaModificationChunks[i];
				if (((ArchetypeChunk)(ref val)).Has<Created>(ref m_CreatedType))
				{
					areaTriggerFlags |= AreaTriggerFlags.Created;
				}
				val = m_AreaModificationChunks[i];
				if (((ArchetypeChunk)(ref val)).Has<Updated>(ref m_UpdatedType))
				{
					areaTriggerFlags |= AreaTriggerFlags.Modified;
				}
				for (int j = 0; j < triggerDatas.Length; j++)
				{
					AreaTriggerData areaTriggerData = triggerDatas[j];
					if ((areaTriggerData.m_Flags & areaTriggerFlags) == 0)
					{
						continue;
					}
					val = m_AreaModificationChunks[i];
					NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
					for (int k = 0; k < nativeArray.Length; k++)
					{
						if (areaTriggerData.m_Prefab == nativeArray[k].m_Prefab)
						{
							return true;
						}
					}
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
		public BufferTypeHandle<AreaTriggerData> __Game_Tutorials_AreaTriggerData_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferLookup<UnlockRequirement> __Game_Prefabs_UnlockRequirement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ForceUIGroupUnlockData> __Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Updated> __Game_Common_Updated_RO_ComponentTypeHandle;

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
			__Game_Tutorials_AreaTriggerData_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AreaTriggerData>(true);
			__Game_Prefabs_UnlockRequirement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<UnlockRequirement>(true);
			__Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ForceUIGroupUnlockData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Updated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Updated>(true);
		}
	}

	private EntityQuery m_AreaModificationQuery;

	private EntityQuery m_AreaQuery;

	private EntityArchetype m_UnlockEventArchetype;

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
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_AreaModificationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Native>()
		});
		m_AreaQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Area>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Native>()
		});
		m_ActiveTriggerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<AreaTriggerData>(),
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
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		base.OnUpdate();
		EntityCommandBuffer val2;
		if (base.triggersChanged)
		{
			JobHandle val = default(JobHandle);
			CheckModifiedAreasJob checkModifiedAreasJob = new CheckModifiedAreasJob
			{
				m_AreaModificationChunks = ((EntityQuery)(ref m_AreaQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val),
				m_TriggerType = InternalCompilerInterface.GetBufferTypeHandle<AreaTriggerData>(ref __TypeHandle.__Game_Tutorials_AreaTriggerData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UnlockRequirementFromEntity = InternalCompilerInterface.GetBufferLookup<UnlockRequirement>(ref __TypeHandle.__Game_Prefabs_UnlockRequirement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ForcedUnlockDataFromEntity = InternalCompilerInterface.GetBufferLookup<ForceUIGroupUnlockData>(ref __TypeHandle.__Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedType = InternalCompilerInterface.GetComponentTypeHandle<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UnlockEventArchetype = m_UnlockEventArchetype
			};
			val2 = m_BarrierSystem.CreateCommandBuffer();
			checkModifiedAreasJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			checkModifiedAreasJob.m_FirstTimeCheck = true;
			CheckModifiedAreasJob checkModifiedAreasJob2 = checkModifiedAreasJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckModifiedAreasJob>(checkModifiedAreasJob2, m_ActiveTriggerQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
			checkModifiedAreasJob2.m_AreaModificationChunks.Dispose(((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_BarrierSystem).AddJobHandleForProducer(((SystemBase)this).Dependency);
		}
		else if (!((EntityQuery)(ref m_AreaModificationQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val3 = default(JobHandle);
			CheckModifiedAreasJob checkModifiedAreasJob = new CheckModifiedAreasJob
			{
				m_AreaModificationChunks = ((EntityQuery)(ref m_AreaModificationQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3),
				m_TriggerType = InternalCompilerInterface.GetBufferTypeHandle<AreaTriggerData>(ref __TypeHandle.__Game_Tutorials_AreaTriggerData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UnlockRequirementFromEntity = InternalCompilerInterface.GetBufferLookup<UnlockRequirement>(ref __TypeHandle.__Game_Prefabs_UnlockRequirement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ForcedUnlockDataFromEntity = InternalCompilerInterface.GetBufferLookup<ForceUIGroupUnlockData>(ref __TypeHandle.__Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedType = InternalCompilerInterface.GetComponentTypeHandle<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UnlockEventArchetype = m_UnlockEventArchetype
			};
			val2 = m_BarrierSystem.CreateCommandBuffer();
			checkModifiedAreasJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			checkModifiedAreasJob.m_FirstTimeCheck = false;
			CheckModifiedAreasJob checkModifiedAreasJob3 = checkModifiedAreasJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckModifiedAreasJob>(checkModifiedAreasJob3, m_ActiveTriggerQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3));
			checkModifiedAreasJob3.m_AreaModificationChunks.Dispose(((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_BarrierSystem).AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public TutorialAreaTriggerSystem()
	{
	}
}

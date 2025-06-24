using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Common;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class PrefabUnlockedRequirementSystem : GameSystemBase
{
	[BurstCompile]
	private struct UnlockJob : IJobChunk
	{
		public EntityArchetype m_UnlockEventArchetype;

		public ParallelWriter m_Buffer;

		[ReadOnly]
		public NativeList<Entity> m_PrefabUnlockedRequirementEntities;

		[ReadOnly]
		public ComponentTypeHandle<Unlock> m_UnlockTypeHandle;

		[ReadOnly]
		public ComponentLookup<Locked> m_LockedDataFromEntity;

		[ReadOnly]
		public BufferLookup<PrefabUnlockedRequirement> m_PrefabUnlockedRequirementFromEntity;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Unlock> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Unlock>(ref m_UnlockTypeHandle);
			DynamicBuffer<PrefabUnlockedRequirement> val2 = default(DynamicBuffer<PrefabUnlockedRequirement>);
			for (int i = 0; i < m_PrefabUnlockedRequirementEntities.Length; i++)
			{
				Entity val = m_PrefabUnlockedRequirementEntities[i];
				if (!EntitiesExtensions.HasEnabledComponent<Locked>(m_LockedDataFromEntity, val) || !m_PrefabUnlockedRequirementFromEntity.TryGetBuffer(val, ref val2))
				{
					continue;
				}
				for (int j = 0; j < val2.Length; j++)
				{
					Entity requirement = val2[j].m_Requirement;
					for (int k = 0; k < nativeArray.Length; k++)
					{
						if (requirement == nativeArray[k].m_Prefab)
						{
							Entity val3 = ((ParallelWriter)(ref m_Buffer)).CreateEntity(i, m_UnlockEventArchetype);
							((ParallelWriter)(ref m_Buffer)).SetComponent<Unlock>(i, val3, new Unlock(val));
						}
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
		public BufferLookup<PrefabUnlockedRequirement> __Game_Prefabs_PrefabUnlockedRequirement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Locked> __Game_Prefabs_Locked_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Unlock> __Game_Prefabs_Unlock_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabUnlockedRequirement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PrefabUnlockedRequirement>(true);
			__Game_Prefabs_Locked_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Locked>(true);
			__Game_Prefabs_Unlock_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unlock>(true);
		}
	}

	private ModificationEndBarrier m_ModificationEndBarrier;

	private EntityQuery m_UnlockQuery;

	private EntityQuery m_PrefabUnlockedQuery;

	private EntityArchetype m_UnlockEventArchetype;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationEndBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationEndBarrier>();
		m_UnlockQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		m_PrefabUnlockedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabUnlockedRequirement>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_UnlockQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val = default(JobHandle);
			NativeList<Entity> prefabUnlockedRequirementEntities = ((EntityQuery)(ref m_PrefabUnlockedQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
			UnlockJob unlockJob = new UnlockJob
			{
				m_UnlockEventArchetype = m_UnlockEventArchetype
			};
			EntityCommandBuffer val2 = m_ModificationEndBarrier.CreateCommandBuffer();
			unlockJob.m_Buffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			unlockJob.m_PrefabUnlockedRequirementEntities = prefabUnlockedRequirementEntities;
			unlockJob.m_PrefabUnlockedRequirementFromEntity = InternalCompilerInterface.GetBufferLookup<PrefabUnlockedRequirement>(ref __TypeHandle.__Game_Prefabs_PrefabUnlockedRequirement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			unlockJob.m_LockedDataFromEntity = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			unlockJob.m_UnlockTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Unlock>(ref __TypeHandle.__Game_Prefabs_Unlock_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			JobHandle val3 = JobChunkExtensions.ScheduleParallel<UnlockJob>(unlockJob, m_UnlockQuery, JobHandle.CombineDependencies(val, ((SystemBase)this).Dependency));
			prefabUnlockedRequirementEntities.Dispose(val3);
			((EntityCommandBufferSystem)m_ModificationEndBarrier).AddJobHandleForProducer(val3);
			((SystemBase)this).Dependency = val3;
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
	public PrefabUnlockedRequirementSystem()
	{
	}
}

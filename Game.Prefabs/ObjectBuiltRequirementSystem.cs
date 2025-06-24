using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class ObjectBuiltRequirementSystem : GameSystemBase
{
	[BurstCompile]
	private struct UnlockOnBuildJob : IJobChunk
	{
		public EntityArchetype m_UnlockEventArchetype;

		public ParallelWriter m_Buffer;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedTypeHandle;

		[ReadOnly]
		public ComponentLookup<Locked> m_LockedDataFromEntity;

		[ReadOnly]
		public BufferLookup<UnlockOnBuildData> m_UnlockOnBuildDataFromEntity;

		[ReadOnly]
		public ComponentLookup<UnlockRequirementData> m_UnlockRequirementDataFromEntity;

		[ReadOnly]
		public ComponentLookup<ObjectBuiltRequirementData> m_UnlockOnBuildRequirementDataFromEntity;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefTypeHandle);
			DynamicBuffer<UnlockOnBuildData> val = default(DynamicBuffer<UnlockOnBuildData>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity prefab = nativeArray[i].m_Prefab;
				if (!m_UnlockOnBuildDataFromEntity.TryGetBuffer(prefab, ref val))
				{
					continue;
				}
				bool flag = ((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedTypeHandle);
				for (int j = 0; j < val.Length; j++)
				{
					Entity entity = val[j].m_Entity;
					ObjectBuiltRequirementData objectBuiltRequirementData = m_UnlockOnBuildRequirementDataFromEntity[entity];
					UnlockRequirementData unlockRequirementData = m_UnlockRequirementDataFromEntity[entity];
					int num = math.max(unlockRequirementData.m_Progress + ((!flag) ? 1 : (-1)), 0);
					unlockRequirementData.m_Progress = math.min(objectBuiltRequirementData.m_MinimumCount, num);
					((ParallelWriter)(ref m_Buffer)).SetComponent<UnlockRequirementData>(unfilteredChunkIndex, entity, unlockRequirementData);
					if (EntitiesExtensions.HasEnabledComponent<Locked>(m_LockedDataFromEntity, entity) && objectBuiltRequirementData.m_MinimumCount <= num)
					{
						Entity val2 = ((ParallelWriter)(ref m_Buffer)).CreateEntity(i, m_UnlockEventArchetype);
						((ParallelWriter)(ref m_Buffer)).SetComponent<Unlock>(i, val2, new Unlock(entity));
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Locked> __Game_Prefabs_Locked_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<UnlockOnBuildData> __Game_Prefabs_UnlockOnBuildData_RO_BufferLookup;

		public ComponentLookup<UnlockRequirementData> __Game_Prefabs_UnlockRequirementData_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectBuiltRequirementData> __Game_Prefabs_ObjectBuiltRequirementData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Prefabs_Locked_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Locked>(true);
			__Game_Prefabs_UnlockOnBuildData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<UnlockOnBuildData>(true);
			__Game_Prefabs_UnlockRequirementData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UnlockRequirementData>(false);
			__Game_Prefabs_ObjectBuiltRequirementData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectBuiltRequirementData>(true);
		}
	}

	private ModificationEndBarrier m_ModificationEndBarrier;

	private EntityQuery m_ChangedQuery;

	private EntityQuery m_AllQuery;

	private EntityArchetype m_UnlockEventArchetype;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationEndBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationEndBarrier>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabRef>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Native>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_ChangedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_AllQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Native>(),
			ComponentType.Exclude<Temp>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Loaded = true;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (GetLoaded() ? m_AllQuery : m_ChangedQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			UnlockOnBuildJob unlockOnBuildJob = new UnlockOnBuildJob
			{
				m_UnlockEventArchetype = m_UnlockEventArchetype
			};
			EntityCommandBuffer val2 = m_ModificationEndBarrier.CreateCommandBuffer();
			unlockOnBuildJob.m_Buffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			unlockOnBuildJob.m_PrefabRefTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			unlockOnBuildJob.m_DeletedTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			unlockOnBuildJob.m_LockedDataFromEntity = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			unlockOnBuildJob.m_UnlockOnBuildDataFromEntity = InternalCompilerInterface.GetBufferLookup<UnlockOnBuildData>(ref __TypeHandle.__Game_Prefabs_UnlockOnBuildData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			unlockOnBuildJob.m_UnlockRequirementDataFromEntity = InternalCompilerInterface.GetComponentLookup<UnlockRequirementData>(ref __TypeHandle.__Game_Prefabs_UnlockRequirementData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			unlockOnBuildJob.m_UnlockOnBuildRequirementDataFromEntity = InternalCompilerInterface.GetComponentLookup<ObjectBuiltRequirementData>(ref __TypeHandle.__Game_Prefabs_ObjectBuiltRequirementData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			UnlockOnBuildJob unlockOnBuildJob2 = unlockOnBuildJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<UnlockOnBuildJob>(unlockOnBuildJob2, val, ((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_ModificationEndBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public ObjectBuiltRequirementSystem()
	{
	}
}

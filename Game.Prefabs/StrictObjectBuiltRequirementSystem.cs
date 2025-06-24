using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class StrictObjectBuiltRequirementSystem : GameSystemBase
{
	[BurstCompile]
	private struct TrackObjectsJob : IJobChunk
	{
		public ParallelWriter m_Buffer;

		[ReadOnly]
		public NativeParallelHashMap<Entity, int> m_InstanceCounts;

		[ReadOnly]
		public ComponentTypeHandle<StrictObjectBuiltRequirementData> m_ObjectBuiltRequirementDataHandle;

		public ComponentTypeHandle<UnlockRequirementData> m_RequirementDataHandle;

		[ReadOnly]
		public EntityTypeHandle m_EntityTypeHandle;

		public EntityArchetype m_UnlockEventArchetype;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<StrictObjectBuiltRequirementData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<StrictObjectBuiltRequirementData>(ref m_ObjectBuiltRequirementDataHandle);
			NativeArray<UnlockRequirementData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<UnlockRequirementData>(ref m_RequirementDataHandle);
			NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityTypeHandle);
			ChunkEntityEnumerator val = default(ChunkEntityEnumerator);
			((ChunkEntityEnumerator)(ref val))._002Ector(useEnabledMask, chunkEnabledMask, ((ArchetypeChunk)(ref chunk)).Count);
			int num = default(int);
			int num2 = default(int);
			while (((ChunkEntityEnumerator)(ref val)).NextEntityIndex(ref num))
			{
				if (m_InstanceCounts.TryGetValue(nativeArray[num].m_Requirement, ref num2))
				{
					UnlockRequirementData unlockRequirementData = nativeArray2[num];
					unlockRequirementData.m_Progress = math.min(nativeArray[num].m_MinimumCount, num2);
					nativeArray2[num] = unlockRequirementData;
					if (nativeArray[num].m_MinimumCount <= num2)
					{
						Entity val2 = ((ParallelWriter)(ref m_Buffer)).CreateEntity(unfilteredChunkIndex, m_UnlockEventArchetype);
						((ParallelWriter)(ref m_Buffer)).SetComponent<Unlock>(unfilteredChunkIndex, val2, new Unlock(nativeArray3[num]));
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
		public ComponentTypeHandle<StrictObjectBuiltRequirementData> __Game_Prefabs_StrictObjectBuiltRequirementData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<UnlockRequirementData> __Game_Prefabs_UnlockRequirementData_RW_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_StrictObjectBuiltRequirementData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StrictObjectBuiltRequirementData>(true);
			__Game_Prefabs_UnlockRequirementData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UnlockRequirementData>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
		}
	}

	private InstanceCountSystem m_InstanceCountSystem;

	private ModificationEndBarrier m_ModificationEndBarrier;

	private EntityQuery m_ChangedQuery;

	private EntityQuery m_RequirementQuery;

	private EntityArchetype m_UnlockEventArchetype;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_InstanceCountSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<InstanceCountSystem>();
		m_ModificationEndBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationEndBarrier>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabRef>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_ChangedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_RequirementQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<StrictObjectBuiltRequirementData>(),
			ComponentType.ReadWrite<UnlockRequirementData>(),
			ComponentType.ReadOnly<Locked>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_RequirementQuery);
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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (GetLoaded() || !((EntityQuery)(ref m_ChangedQuery)).IsEmptyIgnoreFilter)
		{
			TrackObjectsJob trackObjectsJob = default(TrackObjectsJob);
			EntityCommandBuffer val = m_ModificationEndBarrier.CreateCommandBuffer();
			trackObjectsJob.m_Buffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
			trackObjectsJob.m_InstanceCounts = m_InstanceCountSystem.GetInstanceCounts(readOnly: true, out var dependencies);
			trackObjectsJob.m_ObjectBuiltRequirementDataHandle = InternalCompilerInterface.GetComponentTypeHandle<StrictObjectBuiltRequirementData>(ref __TypeHandle.__Game_Prefabs_StrictObjectBuiltRequirementData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			trackObjectsJob.m_RequirementDataHandle = InternalCompilerInterface.GetComponentTypeHandle<UnlockRequirementData>(ref __TypeHandle.__Game_Prefabs_UnlockRequirementData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			trackObjectsJob.m_EntityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			trackObjectsJob.m_UnlockEventArchetype = m_UnlockEventArchetype;
			TrackObjectsJob trackObjectsJob2 = trackObjectsJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<TrackObjectsJob>(trackObjectsJob2, m_RequirementQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
			m_InstanceCountSystem.AddCountReader(((SystemBase)this).Dependency);
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
	public StrictObjectBuiltRequirementSystem()
	{
	}
}

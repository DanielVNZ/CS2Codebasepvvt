using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Logging;
using Colossal.Serialization.Entities;
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
public class UnlockSystem : GameSystemBase
{
	[BurstCompile]
	private struct CheckUnlockRequirementsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<UnlockRequirement> m_UnlockRequirementType;

		[ReadOnly]
		public ComponentLookup<Locked> m_LockedData;

		public ParallelWriter<Entity> m_UnlockQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<UnlockRequirement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<UnlockRequirement>(ref m_UnlockRequirementType);
			ChunkEntityEnumerator val = default(ChunkEntityEnumerator);
			((ChunkEntityEnumerator)(ref val))._002Ector(useEnabledMask, chunkEnabledMask, ((ArchetypeChunk)(ref chunk)).Count);
			int num = default(int);
			while (((ChunkEntityEnumerator)(ref val)).NextEntityIndex(ref num))
			{
				DynamicBuffer<UnlockRequirement> val2 = bufferAccessor[num];
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				for (int i = 0; i < val2.Length; i++)
				{
					if (flag)
					{
						break;
					}
					UnlockRequirement unlockRequirement = val2[i];
					bool flag4 = EntitiesExtensions.HasEnabledComponent<Locked>(m_LockedData, unlockRequirement.m_Prefab);
					bool flag5 = (unlockRequirement.m_Flags & UnlockFlags.RequireAll) != 0;
					bool flag6 = (unlockRequirement.m_Flags & UnlockFlags.RequireAny) != 0;
					flag = flag || (flag4 && flag5);
					flag2 = flag2 || (flag4 && flag6);
					flag3 = flag3 || (!flag4 && flag6);
				}
				if (!flag && (flag3 || !flag2))
				{
					Entity val3 = nativeArray[num];
					m_UnlockQueue.Enqueue(val3);
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
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<UnlockRequirement> __Game_Prefabs_UnlockRequirement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Locked> __Game_Prefabs_Locked_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_UnlockRequirement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<UnlockRequirement>(true);
			__Game_Prefabs_Locked_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Locked>(true);
		}
	}

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_LockedQuery;

	private EntityQuery m_UpdatedQuery;

	private EntityQuery m_EventQuery;

	private EntityArchetype m_UnlockEventArchetype;

	private bool m_Loaded;

	private ILog m_Log;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_LockedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Locked>(),
			ComponentType.ReadOnly<UnlockRequirement>()
		});
		m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Locked>(),
			ComponentType.ReadOnly<UnlockRequirement>(),
			ComponentType.ReadOnly<Updated>()
		});
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		m_Log = LogManager.GetLogger("Unlocking");
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		((ComponentSystemBase)this).Enabled = mode.IsGame();
	}

	protected override void OnGameLoaded(Context context)
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
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		if (!ProcessEvents() && !loaded && ((EntityQuery)(ref m_UpdatedQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		NativeQueue<Entity> val = default(NativeQueue<Entity>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			Entity unlock = default(Entity);
			while (true)
			{
				JobHandle val2 = JobChunkExtensions.ScheduleParallel<CheckUnlockRequirementsJob>(new CheckUnlockRequirementsJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_UnlockRequirementType = InternalCompilerInterface.GetBufferTypeHandle<UnlockRequirement>(ref __TypeHandle.__Game_Prefabs_UnlockRequirement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_LockedData = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_UnlockQueue = val.AsParallelWriter()
				}, m_LockedQuery, default(JobHandle));
				((JobHandle)(ref val2)).Complete();
				if (val.Count == 0)
				{
					break;
				}
				while (val.TryDequeue(ref unlock))
				{
					UnlockPrefab(unlock, createEvent: true);
				}
			}
		}
		finally
		{
			val.Dispose();
		}
	}

	private bool ProcessEvents()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_EventQuery)).IsEmptyIgnoreFilter)
		{
			return false;
		}
		bool result = false;
		NativeArray<Unlock> val = ((EntityQuery)(ref m_EventQuery)).ToComponentDataArray<Unlock>(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				Entity prefab = val[i].m_Prefab;
				if (EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, prefab))
				{
					UnlockPrefab(prefab, createEvent: false);
					result = true;
				}
			}
			return result;
		}
		finally
		{
			val.Dispose();
		}
	}

	private void UnlockPrefab(Entity unlock, bool createEvent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentEnabled<Locked>(unlock, false);
		if (createEvent)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Entity val = ((EntityManager)(ref entityManager)).CreateEntity(m_UnlockEventArchetype);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<Unlock>(val, new Unlock(unlock));
		}
		if (!EntitiesExtensions.HasEnabledComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, unlock))
		{
			PrefabID obsoleteID = m_PrefabSystem.GetObsoleteID(unlock);
			m_Log.DebugFormat("Prefab unlocked: {0}", (object)obsoleteID);
		}
		else
		{
			PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(unlock);
			m_Log.DebugFormat("Prefab unlocked: {0}", (object)prefab);
		}
	}

	public bool IsLocked(PrefabBase prefab)
	{
		return m_PrefabSystem.HasEnabledComponent<Locked>(prefab);
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
	public UnlockSystem()
	{
	}
}

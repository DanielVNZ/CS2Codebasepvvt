using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Citizens;
using Game.Common;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Triggers;

[CompilerGenerated]
public class LifePathEventSystem : GameSystemBase
{
	[BurstCompile]
	private struct CreateLifePathEventJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<LifePathEventData> m_LifePathEventDatas;

		[ReadOnly]
		public BufferLookup<LifePathEntry> m_LifePathEntries;

		public EntityCommandBuffer m_CommandBuffer;

		[ReadOnly]
		public EntityArchetype m_EventArchetype;

		public NativeQueue<LifePathEventCreationData> m_Queue;

		public NativeQueue<ChirpCreationData> m_ChirpQueue;

		[ReadOnly]
		public TimeData m_TimeData;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public bool m_DebugLifePathChirps;

		public void Execute()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			LifePathEventCreationData lifePathEventCreationData = default(LifePathEventCreationData);
			while (m_Queue.TryDequeue(ref lifePathEventCreationData))
			{
				LifePathEventData lifePathEventData = m_LifePathEventDatas[lifePathEventCreationData.m_EventPrefab];
				bool flag = m_LifePathEntries.HasBuffer(lifePathEventCreationData.m_Sender);
				if (m_DebugLifePathChirps || flag)
				{
					if (lifePathEventData.m_IsChirp)
					{
						m_ChirpQueue.Enqueue(new ChirpCreationData
						{
							m_TriggerPrefab = lifePathEventCreationData.m_EventPrefab,
							m_Sender = lifePathEventCreationData.m_Sender,
							m_Target = lifePathEventCreationData.m_Target
						});
					}
					else if (flag)
					{
						Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_EventArchetype);
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<LifePathEvent>(val, new LifePathEvent
						{
							m_EventPrefab = lifePathEventCreationData.m_EventPrefab,
							m_Target = lifePathEventCreationData.m_Target,
							m_Date = (uint)TimeSystem.GetDay(m_SimulationFrame, m_TimeData)
						});
						((EntityCommandBuffer)(ref m_CommandBuffer)).AppendToBuffer<LifePathEntry>(lifePathEventCreationData.m_Sender, new LifePathEntry(val));
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(lifePathEventCreationData.m_Sender);
				}
			}
		}
	}

	[BurstCompile]
	private struct CleanupLifePathEntriesJob : IJobChunk
	{
		[ReadOnly]
		public BufferTypeHandle<LifePathEntry> m_EntryType;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<LifePathEntry> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LifePathEntry>(ref m_EntryType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				for (int j = 0; j < bufferAccessor[i].Length; j++)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, bufferAccessor[i][j].m_Entity);
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
		public BufferTypeHandle<LifePathEntry> __Game_Triggers_LifePathEntry_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<LifePathEventData> __Game_Prefabs_LifePathEventData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LifePathEntry> __Game_Triggers_LifePathEntry_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Triggers_LifePathEntry_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LifePathEntry>(true);
			__Game_Prefabs_LifePathEventData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LifePathEventData>(true);
			__Game_Triggers_LifePathEntry_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LifePathEntry>(true);
		}
	}

	public static readonly int kMaxFollowed = 50;

	private SimulationSystem m_SimulationSystem;

	private ModificationEndBarrier m_ModificationBarrier;

	private CreateChirpSystem m_CreateChirpSystem;

	private EntityQuery m_FollowedQuery;

	private EntityQuery m_DeletedFollowedQuery;

	private EntityQuery m_TimeDataQuery;

	private EntityArchetype m_EventArchetype;

	private NativeQueue<LifePathEventCreationData> m_Queue;

	private JobHandle m_WriteDependencies;

	private TypeHandle __TypeHandle;

	public bool m_DebugLifePathChirps { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationEndBarrier>();
		m_CreateChirpSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CreateChirpSystem>();
		m_FollowedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Followed>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_DeletedFollowedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Followed>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<LifePathEvent>() });
		m_Queue = new NativeQueue<LifePathEventCreationData>(AllocatorHandle.op_Implicit((Allocator)4));
		((ComponentSystemBase)this).RequireForUpdate(m_TimeDataQuery);
		((ComponentSystemBase)this).Enabled = false;
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		((ComponentSystemBase)this).Enabled = mode.IsGame();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_WriteDependencies)).Complete();
		m_Queue.Dispose();
		base.OnDestroy();
	}

	public NativeQueue<LifePathEventCreationData> GetQueue(out JobHandle deps)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(((ComponentSystemBase)this).Enabled, "Can not write to queue when system isn't running");
		deps = m_WriteDependencies;
		return m_Queue;
	}

	public void AddQueueWriter(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_WriteDependencies = JobHandle.CombineDependencies(m_WriteDependencies, handle);
	}

	public bool FollowCitizen(Entity citizen)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_FollowedQuery)).CalculateEntityCount() < kMaxFollowed)
		{
			Citizen citizen2 = default(Citizen);
			bool startedFollowingAsChild = EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, citizen, ref citizen2) && citizen2.GetAge() == CitizenAge.Child;
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponentData<Followed>(citizen, new Followed
			{
				m_Priority = m_SimulationSystem.frameIndex,
				m_StartedFollowingAsChild = startedFollowingAsChild
			});
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddBuffer<LifePathEntry>(citizen);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Updated>(citizen);
			return true;
		}
		return false;
	}

	public bool UnfollowCitizen(Entity citizen)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Followed>(citizen))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).RemoveComponent<Followed>(citizen);
			DynamicBuffer<LifePathEntry> val = default(DynamicBuffer<LifePathEntry>);
			if (EntitiesExtensions.TryGetBuffer<LifePathEntry>(((ComponentSystemBase)this).EntityManager, citizen, true, ref val))
			{
				Enumerator<LifePathEntry> enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						LifePathEntry current = enumerator.Current;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).AddComponent<Deleted>(current.m_Entity);
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).RemoveComponent<LifePathEntry>(citizen);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Updated>(citizen);
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		CleanupLifePathEntriesJob cleanupLifePathEntriesJob = new CleanupLifePathEntriesJob
		{
			m_EntryType = InternalCompilerInterface.GetBufferTypeHandle<LifePathEntry>(ref __TypeHandle.__Game_Triggers_LifePathEntry_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		cleanupLifePathEntriesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		CleanupLifePathEntriesJob cleanupLifePathEntriesJob2 = cleanupLifePathEntriesJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CleanupLifePathEntriesJob>(cleanupLifePathEntriesJob2, m_DeletedFollowedQuery, ((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
		JobHandle deps;
		CreateLifePathEventJob createLifePathEventJob = new CreateLifePathEventJob
		{
			m_LifePathEventDatas = InternalCompilerInterface.GetComponentLookup<LifePathEventData>(ref __TypeHandle.__Game_Prefabs_LifePathEventData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LifePathEntries = InternalCompilerInterface.GetBufferLookup<LifePathEntry>(ref __TypeHandle.__Game_Triggers_LifePathEntry_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer(),
			m_EventArchetype = m_EventArchetype,
			m_Queue = m_Queue,
			m_ChirpQueue = m_CreateChirpSystem.GetQueue(out deps),
			m_TimeData = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>(),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_DebugLifePathChirps = m_DebugLifePathChirps
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<CreateLifePathEventJob>(createLifePathEventJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_WriteDependencies, deps));
		m_CreateChirpSystem.AddQueueWriter(((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_WriteDependencies = ((SystemBase)this).Dependency;
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
	public LifePathEventSystem()
	{
	}
}

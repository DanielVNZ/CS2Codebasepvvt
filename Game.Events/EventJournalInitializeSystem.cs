using System.Runtime.CompilerServices;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Events;

[CompilerGenerated]
public class EventJournalInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitEventJournalEntriesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Duration> m_DurationType;

		[ReadOnly]
		public ComponentLookup<JournalEventPrefabData> m_JournalEventPrefabDatas;

		public EntityArchetype m_JournalArchetype;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			if (((ArchetypeChunk)(ref chunk)).Has<Duration>(ref m_DurationType))
			{
				NativeArray<Duration> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Duration>(ref m_DurationType);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity val = CreateJournalEntry(nativeArray2[i], nativeArray[i].m_Prefab, unfilteredChunkIndex);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<JournalEvent>(unfilteredChunkIndex, nativeArray2[i], new JournalEvent
					{
						m_JournalEntity = val
					});
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EventJournalPending>(unfilteredChunkIndex, val, new EventJournalPending
					{
						m_StartFrame = nativeArray3[i].m_StartFrame
					});
				}
			}
			else
			{
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity journalEntity = CreateJournalEntry(nativeArray2[j], nativeArray[j].m_Prefab, unfilteredChunkIndex);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<JournalEvent>(unfilteredChunkIndex, nativeArray2[j], new JournalEvent
					{
						m_JournalEntity = journalEntity
					});
				}
			}
		}

		private Entity CreateJournalEntry(Entity eventEntity, Entity eventPrefab, int chunkIndex)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(chunkIndex, m_JournalArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<EventJournalEntry>(chunkIndex, val, new EventJournalEntry
			{
				m_Event = eventEntity
			});
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(chunkIndex, val, new PrefabRef
			{
				m_Prefab = eventPrefab
			});
			JournalEventPrefabData journalEventPrefabData = m_JournalEventPrefabDatas[eventPrefab];
			if (journalEventPrefabData.m_DataFlags != 0)
			{
				DynamicBuffer<EventJournalData> datas = ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<EventJournalData>(chunkIndex, val);
				InitDataBuffer(datas, journalEventPrefabData.m_DataFlags);
			}
			if (journalEventPrefabData.m_EffectFlags != 0)
			{
				DynamicBuffer<EventJournalCityEffect> effects = ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<EventJournalCityEffect>(chunkIndex, val);
				InitEffectBuffer(effects, journalEventPrefabData.m_EffectFlags);
			}
			return val;
		}

		private void InitDataBuffer(DynamicBuffer<EventJournalData> datas, int dataFlags)
		{
			for (int i = 0; i < 3; i++)
			{
				if (((1 << i) & dataFlags) != 0)
				{
					datas.Add(new EventJournalData
					{
						m_Type = (EventDataTrackingType)i,
						m_Value = 0
					});
				}
			}
		}

		private void InitEffectBuffer(DynamicBuffer<EventJournalCityEffect> effects, int effectFlags)
		{
			for (int i = 0; i < 5; i++)
			{
				if (((1 << i) & effectFlags) != 0)
				{
					effects.Add(new EventJournalCityEffect
					{
						m_Type = (EventCityEffectTrackingType)i,
						m_StartValue = 0,
						m_Value = 0
					});
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Duration> __Game_Events_Duration_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<JournalEventPrefabData> __Game_Prefabs_JournalEventPrefabData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Events_Duration_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Duration>(true);
			__Game_Prefabs_JournalEventPrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<JournalEventPrefabData>(true);
		}
	}

	private EntityQuery m_CreatedEventQuery;

	private EntityArchetype m_EventJournalArchetype;

	private ModificationBarrier4 m_ModificationBarrier;

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
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CreatedEventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Event>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<JournalEvent>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EventJournalArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<EventJournalEntry>(),
			ComponentType.ReadWrite<Created>(),
			ComponentType.ReadWrite<PrefabRef>()
		});
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		((ComponentSystemBase)this).RequireForUpdate(m_CreatedEventQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		InitEventJournalEntriesJob initEventJournalEntriesJob = new InitEventJournalEntriesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DurationType = InternalCompilerInterface.GetComponentTypeHandle<Duration>(ref __TypeHandle.__Game_Events_Duration_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_JournalEventPrefabDatas = InternalCompilerInterface.GetComponentLookup<JournalEventPrefabData>(ref __TypeHandle.__Game_Prefabs_JournalEventPrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_JournalArchetype = m_EventJournalArchetype
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		initEventJournalEntriesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		InitEventJournalEntriesJob initEventJournalEntriesJob2 = initEventJournalEntriesJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<InitEventJournalEntriesJob>(initEventJournalEntriesJob2, m_CreatedEventQuery, ((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public EventJournalInitializeSystem()
	{
	}
}

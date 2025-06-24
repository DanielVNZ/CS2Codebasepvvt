using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Events;

[CompilerGenerated]
public class EventJournalSystem : GameSystemBase, IEventJournalSystem
{
	[BurstCompile]
	private struct StartedEventsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<EventJournalPending> m_PendingType;

		public BufferTypeHandle<EventJournalCityEffect> m_CityEffectType;

		public ComponentTypeHandle<EventJournalEntry> m_EntryType;

		public uint m_SimulationFrame;

		[ReadOnly]
		public NativeArray<int> m_CityEffects;

		public ParallelWriter<Entity> m_Started;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<EventJournalEntry> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EventJournalEntry>(ref m_EntryType);
			if (((ArchetypeChunk)(ref chunk)).Has<EventJournalCityEffect>(ref m_CityEffectType))
			{
				BufferAccessor<EventJournalCityEffect> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<EventJournalCityEffect>(ref m_CityEffectType);
				if (((ArchetypeChunk)(ref chunk)).Has<EventJournalPending>(ref m_PendingType))
				{
					NativeArray<EventJournalPending> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EventJournalPending>(ref m_PendingType);
					for (int i = 0; i < nativeArray.Length; i++)
					{
						if (nativeArray3[i].m_StartFrame <= m_SimulationFrame)
						{
							Init(bufferAccessor[i]);
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<EventJournalPending>(unfilteredChunkIndex, nativeArray[i]);
							EventJournalEntry eventJournalEntry = nativeArray2[i];
							eventJournalEntry.m_StartFrame = m_SimulationFrame;
							nativeArray2[i] = eventJournalEntry;
							m_Started.Enqueue(nativeArray[i]);
						}
					}
				}
				else
				{
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Init(bufferAccessor[j]);
						EventJournalEntry eventJournalEntry2 = nativeArray2[j];
						eventJournalEntry2.m_StartFrame = m_SimulationFrame;
						nativeArray2[j] = eventJournalEntry2;
						m_Started.Enqueue(nativeArray[j]);
					}
				}
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<EventJournalPending>(ref m_PendingType))
			{
				NativeArray<EventJournalPending> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EventJournalPending>(ref m_PendingType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					if (nativeArray4[k].m_StartFrame <= m_SimulationFrame)
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<EventJournalPending>(unfilteredChunkIndex, nativeArray[k]);
						EventJournalEntry eventJournalEntry3 = nativeArray2[k];
						eventJournalEntry3.m_StartFrame = m_SimulationFrame;
						nativeArray2[k] = eventJournalEntry3;
						m_Started.Enqueue(nativeArray[k]);
					}
				}
			}
			else
			{
				for (int l = 0; l < nativeArray.Length; l++)
				{
					EventJournalEntry eventJournalEntry4 = nativeArray2[l];
					eventJournalEntry4.m_StartFrame = m_SimulationFrame;
					nativeArray2[l] = eventJournalEntry4;
					m_Started.Enqueue(nativeArray[l]);
				}
			}
		}

		private void Init(DynamicBuffer<EventJournalCityEffect> effects)
		{
			for (int i = 0; i < effects.Length; i++)
			{
				EventJournalCityEffect eventJournalCityEffect = effects[i];
				eventJournalCityEffect.m_StartValue = m_CityEffects[(int)eventJournalCityEffect.m_Type];
				effects[i] = eventJournalCityEffect;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct DeletedEventsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<JournalEvent> m_JournalEventType;

		[ReadOnly]
		public ComponentLookup<EventJournalCompleted> m_CompletedData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<EventJournalEntry> m_EntryData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<JournalEvent> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<JournalEvent>(ref m_JournalEventType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity journalEntity = nativeArray[i].m_JournalEntity;
				if (m_EntryData.HasComponent(journalEntity))
				{
					EventJournalEntry eventJournalEntry = m_EntryData[journalEntity];
					eventJournalEntry.m_Event = Entity.Null;
					m_EntryData[journalEntity] = eventJournalEntry;
					if (!m_CompletedData.HasComponent(journalEntity))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EventJournalCompleted>(unfilteredChunkIndex, nativeArray[i].m_JournalEntity);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct TrackCityEffectsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public BufferTypeHandle<EventJournalCityEffect> m_CityEffectType;

		[ReadOnly]
		public NativeArray<int> m_CityEffects;

		public ParallelWriter<Entity> m_Changes;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<EventJournalCityEffect> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<EventJournalCityEffect>(ref m_CityEffectType);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				if (Update(bufferAccessor[i]))
				{
					m_Changes.Enqueue(nativeArray[i]);
				}
			}
		}

		private bool Update(DynamicBuffer<EventJournalCityEffect> effects)
		{
			bool result = false;
			for (int i = 0; i < effects.Length; i++)
			{
				EventJournalCityEffect eventJournalCityEffect = effects[i];
				int num = m_CityEffects[(int)eventJournalCityEffect.m_Type];
				if (eventJournalCityEffect.m_Value != num)
				{
					result = true;
					eventJournalCityEffect.m_Value = num;
					effects[i] = eventJournalCityEffect;
				}
			}
			return result;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct TrackDataJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<AddEventJournalData> m_AddDataType;

		[ReadOnly]
		public ComponentLookup<JournalEvent> m_JournalEvents;

		public BufferLookup<EventJournalData> m_EventJournalDatas;

		public NativeQueue<Entity> m_Changes;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<AddEventJournalData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AddEventJournalData>(ref m_AddDataType);
			Execute(nativeArray);
		}

		private void Execute(NativeArray<AddEventJournalData> addedDatas)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < addedDatas.Length; i++)
			{
				AddEventJournalData addEventJournalData = addedDatas[i];
				if (!m_JournalEvents.HasComponent(addEventJournalData.m_Event))
				{
					continue;
				}
				Entity journalEntity = m_JournalEvents[addedDatas[i].m_Event].m_JournalEntity;
				if (m_EventJournalDatas.HasBuffer(journalEntity))
				{
					DynamicBuffer<EventJournalData> eventJournalDatas = m_EventJournalDatas[journalEntity];
					if (TryAddData(eventJournalDatas, addEventJournalData.m_Type, addEventJournalData.m_Count))
					{
						m_Changes.Enqueue(journalEntity);
					}
				}
			}
		}

		private bool TryAddData(DynamicBuffer<EventJournalData> eventJournalDatas, EventDataTrackingType type, int count)
		{
			for (int i = 0; i < eventJournalDatas.Length; i++)
			{
				EventJournalData eventJournalData = eventJournalDatas[i];
				if (eventJournalData.m_Type == type)
				{
					eventJournalData.m_Value += count;
					eventJournalDatas[i] = eventJournalData;
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

	[BurstCompile]
	private struct CheckJournalTrackingEndJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<EventJournalEntry> m_EntryType;

		[ReadOnly]
		public ComponentLookup<Fire> m_FireData;

		[ReadOnly]
		public BufferLookup<TargetElement> m_TargetElementData;

		[ReadOnly]
		public ComponentLookup<OnFire> m_OnFireData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<EventJournalEntry> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EventJournalEntry>(ref m_EntryType);
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i].m_Event;
				if (m_FireData.HasComponent(val) && m_TargetElementData.HasBuffer(val) && CheckFireEnded(m_TargetElementData[val]))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EventJournalCompleted>(unfilteredChunkIndex, nativeArray2[i]);
				}
			}
		}

		private bool CheckFireEnded(DynamicBuffer<TargetElement> targetElements)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < targetElements.Length; i++)
			{
				if (m_OnFireData.HasComponent(targetElements[i].m_Entity))
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

	private struct JournalSortingInfo : IComparable<JournalSortingInfo>
	{
		public Entity m_Entity;

		public uint m_StartFrame;

		public int CompareTo(JournalSortingInfo other)
		{
			return (int)(m_StartFrame - other.m_StartFrame);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EventJournalPending> __Game_Events_EventJournalPending_RO_ComponentTypeHandle;

		public BufferTypeHandle<EventJournalCityEffect> __Game_Events_EventJournalCityEffect_RW_BufferTypeHandle;

		public ComponentTypeHandle<EventJournalEntry> __Game_Events_EventJournalEntry_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<JournalEvent> __Game_Events_JournalEvent_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<EventJournalCompleted> __Game_Events_EventJournalCompleted_RO_ComponentLookup;

		public ComponentLookup<EventJournalEntry> __Game_Events_EventJournalEntry_RW_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<EventJournalEntry> __Game_Events_EventJournalEntry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Fire> __Game_Events_Fire_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<TargetElement> __Game_Events_TargetElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<OnFire> __Game_Events_OnFire_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<AddEventJournalData> __Game_Events_AddEventJournalData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<JournalEvent> __Game_Events_JournalEvent_RO_ComponentLookup;

		public BufferLookup<EventJournalData> __Game_Events_EventJournalData_RW_BufferLookup;

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
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Events_EventJournalPending_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EventJournalPending>(true);
			__Game_Events_EventJournalCityEffect_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<EventJournalCityEffect>(false);
			__Game_Events_EventJournalEntry_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EventJournalEntry>(false);
			__Game_Events_JournalEvent_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<JournalEvent>(true);
			__Game_Events_EventJournalCompleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EventJournalCompleted>(true);
			__Game_Events_EventJournalEntry_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EventJournalEntry>(false);
			__Game_Events_EventJournalEntry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EventJournalEntry>(true);
			__Game_Events_Fire_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Fire>(true);
			__Game_Events_TargetElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TargetElement>(true);
			__Game_Events_OnFire_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OnFire>(true);
			__Game_Events_AddEventJournalData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AddEventJournalData>(true);
			__Game_Events_JournalEvent_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<JournalEvent>(true);
			__Game_Events_EventJournalData_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<EventJournalData>(false);
		}
	}

	private EntityQuery m_StartedJournalQuery;

	private EntityQuery m_DeletedEventQuery;

	private EntityQuery m_JournalDataEventQuery;

	private EntityQuery m_ActiveJournalEffectQuery;

	private EntityQuery m_JournalEventPrefabQuery;

	private EntityQuery m_LoadedJournalQuery;

	private ISimulationSystem m_SimulationSystem;

	private IBudgetSystem m_BudgetSystem;

	private ICityServiceBudgetSystem m_CityServiceBudgetSystem;

	private CitySystem m_CitySystem;

	private ModificationBarrier5 m_ModificationBarrier;

	private NativeQueue<Entity> m_Started;

	private NativeQueue<Entity> m_Changed;

	private NativeArray<int> m_CityEffects;

	private TypeHandle __TypeHandle;

	public NativeList<Entity> eventJournal { get; private set; }

	public Action<Entity> eventEventDataChanged { get; set; }

	public Action eventEntryAdded { get; set; }

	public IEnumerable<JournalEventComponent> eventPrefabs
	{
		get
		{
			PrefabSystem prefabSystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<PrefabSystem>();
			NativeArray<PrefabData> prefabs = ((EntityQuery)(ref m_JournalEventPrefabQuery)).ToComponentDataArray<PrefabData>(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				int i = 0;
				while (i < prefabs.Length)
				{
					EventPrefab prefab = prefabSystem.GetPrefab<EventPrefab>(prefabs[i]);
					yield return prefab.GetComponent<JournalEventComponent>();
					int num = i + 1;
					i = num;
				}
			}
			finally
			{
				prefabs.Dispose();
			}
		}
	}

	public EventJournalEntry GetInfo(Entity journalEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).GetComponentData<EventJournalEntry>(journalEntity);
	}

	public Entity GetPrefab(Entity journalEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(journalEntity).m_Prefab;
	}

	public bool TryGetData(Entity journalEntity, out DynamicBuffer<EventJournalData> data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<EventJournalData>(journalEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			data = ((EntityManager)(ref entityManager)).GetBuffer<EventJournalData>(journalEntity, true);
			return true;
		}
		data = default(DynamicBuffer<EventJournalData>);
		return false;
	}

	public bool TryGetCityEffects(Entity journalEntity, out DynamicBuffer<EventJournalCityEffect> data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<EventJournalCityEffect>(journalEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			data = ((EntityManager)(ref entityManager)).GetBuffer<EventJournalCityEffect>(journalEntity, true);
			return true;
		}
		data = default(DynamicBuffer<EventJournalCityEffect>);
		return false;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<EventJournalEntry>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<EventJournalPending>()
		};
		array[0] = val;
		m_StartedJournalQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_DeletedEventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<JournalEvent>(),
			ComponentType.ReadOnly<Deleted>()
		});
		m_ActiveJournalEffectQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<EventJournalCityEffect>(),
			ComponentType.Exclude<EventJournalPending>(),
			ComponentType.Exclude<EventJournalCompleted>()
		});
		m_JournalDataEventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<AddEventJournalData>(),
			ComponentType.ReadOnly<Game.Common.Event>()
		});
		m_JournalEventPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<EventPrefab>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_LoadedJournalQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<EventJournalEntry>(),
			ComponentType.Exclude<EventJournalPending>()
		});
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_BudgetSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BudgetSystem>();
		m_CityServiceBudgetSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityServiceBudgetSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		eventJournal = new NativeList<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		m_Started = new NativeQueue<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		m_Changed = new NativeQueue<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		m_CityEffects = new NativeArray<int>(5, (Allocator)4, (NativeArrayOptions)1);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		eventJournal.Clear();
		m_Changed.Clear();
		m_Started.Clear();
		if (!((EntityQuery)(ref m_LoadedJournalQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val = ((EntityQuery)(ref m_LoadedJournalQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<EventJournalEntry> val2 = ((EntityQuery)(ref m_LoadedJournalQuery)).ToComponentDataArray<EventJournalEntry>(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<JournalSortingInfo> val3 = default(NativeArray<JournalSortingInfo>);
			val3._002Ector(val.Length, (Allocator)3, (NativeArrayOptions)1);
			for (int i = 0; i < val.Length; i++)
			{
				val3[i] = new JournalSortingInfo
				{
					m_Entity = val[i],
					m_StartFrame = val2[i].m_StartFrame
				};
			}
			val.Dispose();
			val2.Dispose();
			NativeSortExtension.Sort<JournalSortingInfo>(val3);
			for (int j = 0; j < val3.Length; j++)
			{
				NativeList<Entity> val4 = eventJournal;
				JournalSortingInfo journalSortingInfo = val3[j];
				val4.Add(ref journalSortingInfo.m_Entity);
			}
			val3.Dispose();
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.OnDestroy();
		eventJournal.Dispose();
		m_Changed.Dispose();
		m_Started.Dispose();
		m_CityEffects.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Entity val = default(Entity);
		while (m_Started.TryDequeue(ref val))
		{
			eventJournal.Add(ref val);
			flag = true;
		}
		if (flag)
		{
			eventEntryAdded?.Invoke();
		}
		Entity obj = default(Entity);
		while (m_Changed.TryDequeue(ref obj))
		{
			eventEventDataChanged?.Invoke(obj);
		}
		Population population = default(Population);
		Tourism tourism = default(Tourism);
		if ((!((EntityQuery)(ref m_StartedJournalQuery)).IsEmptyIgnoreFilter || !((EntityQuery)(ref m_ActiveJournalEffectQuery)).IsEmptyIgnoreFilter) && EntitiesExtensions.TryGetComponent<Population>(((ComponentSystemBase)this).EntityManager, m_CitySystem.City, ref population) && EntitiesExtensions.TryGetComponent<Tourism>(((ComponentSystemBase)this).EntityManager, m_CitySystem.City, ref tourism))
		{
			m_CityEffects[0] = 0;
			m_CityEffects[1] = population.m_AverageHappiness;
			m_CityEffects[2] = m_CityServiceBudgetSystem.GetTotalTaxIncome();
			m_CityEffects[3] = m_BudgetSystem.GetTotalTradeWorth();
			m_CityEffects[4] = tourism.m_CurrentTourists;
		}
		EntityCommandBuffer val2;
		if (!((EntityQuery)(ref m_StartedJournalQuery)).IsEmptyIgnoreFilter)
		{
			StartedEventsJob startedEventsJob = new StartedEventsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PendingType = InternalCompilerInterface.GetComponentTypeHandle<EventJournalPending>(ref __TypeHandle.__Game_Events_EventJournalPending_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CityEffectType = InternalCompilerInterface.GetBufferTypeHandle<EventJournalCityEffect>(ref __TypeHandle.__Game_Events_EventJournalCityEffect_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EntryType = InternalCompilerInterface.GetComponentTypeHandle<EventJournalEntry>(ref __TypeHandle.__Game_Events_EventJournalEntry_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SimulationFrame = m_SimulationSystem.frameIndex,
				m_CityEffects = m_CityEffects,
				m_Started = m_Started.AsParallelWriter()
			};
			val2 = m_ModificationBarrier.CreateCommandBuffer();
			startedEventsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			StartedEventsJob startedEventsJob2 = startedEventsJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<StartedEventsJob>(startedEventsJob2, m_StartedJournalQuery, ((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
		}
		if (!((EntityQuery)(ref m_DeletedEventQuery)).IsEmptyIgnoreFilter)
		{
			DeletedEventsJob deletedEventsJob = new DeletedEventsJob
			{
				m_JournalEventType = InternalCompilerInterface.GetComponentTypeHandle<JournalEvent>(ref __TypeHandle.__Game_Events_JournalEvent_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CompletedData = InternalCompilerInterface.GetComponentLookup<EventJournalCompleted>(ref __TypeHandle.__Game_Events_EventJournalCompleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EntryData = InternalCompilerInterface.GetComponentLookup<EventJournalEntry>(ref __TypeHandle.__Game_Events_EventJournalEntry_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			val2 = m_ModificationBarrier.CreateCommandBuffer();
			deletedEventsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			DeletedEventsJob deletedEventsJob2 = deletedEventsJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<DeletedEventsJob>(deletedEventsJob2, m_DeletedEventQuery, ((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
		}
		if (!((EntityQuery)(ref m_ActiveJournalEffectQuery)).IsEmptyIgnoreFilter)
		{
			CheckJournalTrackingEndJob checkJournalTrackingEndJob = new CheckJournalTrackingEndJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EntryType = InternalCompilerInterface.GetComponentTypeHandle<EventJournalEntry>(ref __TypeHandle.__Game_Events_EventJournalEntry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_FireData = InternalCompilerInterface.GetComponentLookup<Fire>(ref __TypeHandle.__Game_Events_Fire_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TargetElementData = InternalCompilerInterface.GetBufferLookup<TargetElement>(ref __TypeHandle.__Game_Events_TargetElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OnFireData = InternalCompilerInterface.GetComponentLookup<OnFire>(ref __TypeHandle.__Game_Events_OnFire_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			val2 = m_ModificationBarrier.CreateCommandBuffer();
			checkJournalTrackingEndJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			CheckJournalTrackingEndJob checkJournalTrackingEndJob2 = checkJournalTrackingEndJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckJournalTrackingEndJob>(checkJournalTrackingEndJob2, m_ActiveJournalEffectQuery, ((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
			TrackCityEffectsJob trackCityEffectsJob = new TrackCityEffectsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CityEffectType = InternalCompilerInterface.GetBufferTypeHandle<EventJournalCityEffect>(ref __TypeHandle.__Game_Events_EventJournalCityEffect_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CityEffects = m_CityEffects,
				m_Changes = m_Changed.AsParallelWriter()
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<TrackCityEffectsJob>(trackCityEffectsJob, m_ActiveJournalEffectQuery, ((SystemBase)this).Dependency);
		}
		if (!((EntityQuery)(ref m_JournalDataEventQuery)).IsEmptyIgnoreFilter)
		{
			TrackDataJob trackDataJob = new TrackDataJob
			{
				m_AddDataType = InternalCompilerInterface.GetComponentTypeHandle<AddEventJournalData>(ref __TypeHandle.__Game_Events_AddEventJournalData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_JournalEvents = InternalCompilerInterface.GetComponentLookup<JournalEvent>(ref __TypeHandle.__Game_Events_JournalEvent_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EventJournalDatas = InternalCompilerInterface.GetBufferLookup<EventJournalData>(ref __TypeHandle.__Game_Events_EventJournalData_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Changes = m_Changed
			};
			((SystemBase)this).Dependency = JobChunkExtensions.Schedule<TrackDataJob>(trackDataJob, m_JournalDataEventQuery, ((SystemBase)this).Dependency);
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
	public EventJournalSystem()
	{
	}
}

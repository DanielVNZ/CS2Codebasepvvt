using System.Runtime.CompilerServices;
using Game.Citizens;
using Game.Common;
using Game.Events;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class FindEventAttendantsSystem : GameSystemBase
{
	private struct Attend
	{
		public Entity m_Event;

		public Entity m_Participant;
	}

	[BurstCompile]
	private struct AttendJob : IJob
	{
		public NativeQueue<Attend> m_AttendQueue;

		public BufferLookup<TargetElement> m_TargetElements;

		[ReadOnly]
		public NativeList<Entity> m_EventEntities;

		[ReadOnly]
		public ComponentLookup<Duration> m_Durations;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<CalendarEventData> m_CalendarEventDatas;

		[ReadOnly]
		public BufferLookup<HaveCoordinatedMeetingData> m_HaveCoordinatedMeetings;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_Citizens;

		public EntityArchetype m_MeetingArchetype;

		public EntityArchetype m_JournalDataArchetype;

		[ReadOnly]
		public uint m_SimulationFrame;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			Attend attend = default(Attend);
			while (m_AttendQueue.TryDequeue(ref attend))
			{
				if (m_Prefabs.HasComponent(attend.m_Event))
				{
					Entity prefab = m_Prefabs[attend.m_Event].m_Prefab;
					CalendarEventData calendarEventData = m_CalendarEventDatas[prefab];
					if (!m_HaveCoordinatedMeetings.HasBuffer(prefab))
					{
						continue;
					}
					Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_MeetingArchetype);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val, new PrefabRef
					{
						m_Prefab = prefab
					});
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<CoordinatedMeeting>(val, new CoordinatedMeeting
					{
						m_Phase = 0,
						m_Status = MeetingStatus.Waiting,
						m_Target = Entity.Null
					});
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val, new PrefabRef
					{
						m_Prefab = prefab
					});
					if (calendarEventData.m_RandomTargetType == EventTargetType.Couple && m_Citizens.HasBuffer(attend.m_Participant))
					{
						DynamicBuffer<TargetElement> val2 = m_TargetElements[attend.m_Event];
						DynamicBuffer<HouseholdCitizen> val3 = m_Citizens[attend.m_Participant];
						DynamicBuffer<CoordinatedMeetingAttendee> val4 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<CoordinatedMeetingAttendee>(val);
						for (int i = 0; i < val3.Length; i++)
						{
							val2.Add(new TargetElement
							{
								m_Entity = val3[i].m_Citizen
							});
							val4.Add(new CoordinatedMeetingAttendee
							{
								m_Attendee = val3[i].m_Citizen
							});
						}
						AddJournalData(attend.m_Event, val3.Length);
					}
				}
				else
				{
					Debug.LogWarning((object)$"Event {attend.m_Event} does not have a prefab");
				}
			}
			for (int j = 0; j < m_EventEntities.Length; j++)
			{
				Entity val5 = m_EventEntities[j];
				Duration duration = m_Durations[val5];
				if (m_SimulationFrame > duration.m_StartFrame + 240)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<FindingEventParticipants>(val5);
				}
			}
		}

		private void AddJournalData(Entity eventEntity, int count)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (eventEntity != Entity.Null && count > 0)
			{
				Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_JournalDataArchetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<AddEventJournalData>(val, new AddEventJournalData(eventEntity, EventDataTrackingType.Attendants, count));
			}
		}
	}

	[BurstCompile]
	private struct ConsiderAttendanceJob : IJobChunk
	{
		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public NativeList<Entity> m_EventEntities;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> m_HouseholdCitizenType;

		[ReadOnly]
		public ComponentTypeHandle<CommuterHousehold> m_CommuterHouseholdType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<CalendarEventData> m_Events;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenDatas;

		public ParallelWriter<Attend> m_AttendQueue;

		public ParallelWriter m_CommandBuffer;

		public uint m_UpdateFrameIndex;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeList<Entity> val = default(NativeList<Entity>);
			val._002Ector(0, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_EventEntities.Length; i++)
			{
				Entity val2 = m_EventEntities[i];
				Entity prefab = m_Prefabs[val2].m_Prefab;
				if (m_Events[prefab].m_RandomTargetType == EventTargetType.Couple && !((ArchetypeChunk)(ref chunk)).Has<CommuterHousehold>(ref m_CommuterHouseholdType))
				{
					val.Add(ref val2);
				}
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<HouseholdCitizen> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<HouseholdCitizen>(ref m_HouseholdCitizenType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				Entity val3 = nativeArray[j];
				DynamicBuffer<HouseholdCitizen> val4 = bufferAccessor[j];
				int num = 0;
				int num2 = 0;
				for (int k = 0; k < val4.Length; k++)
				{
					Entity citizen = val4[k].m_Citizen;
					switch (m_CitizenDatas[citizen].GetAge())
					{
					case CitizenAge.Child:
						num2++;
						break;
					default:
						num++;
						break;
					case CitizenAge.Teen:
						break;
					}
				}
				if (num < 2 || num2 != 0)
				{
					continue;
				}
				for (int l = 0; l < val.Length; l++)
				{
					Entity val5 = val[l];
					Entity prefab2 = m_Prefabs[val5].m_Prefab;
					CalendarEventData calendarEventData = m_Events[prefab2];
					if ((float)((Random)(ref random)).NextInt(100) < calendarEventData.m_AffectedProbability.min)
					{
						m_AttendQueue.Enqueue(new Attend
						{
							m_Event = val[l],
							m_Participant = val3
						});
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<AttendingEvent>(unfilteredChunkIndex, val3, new AttendingEvent
					{
						m_Event = val[l]
					});
				}
			}
			val.Dispose();
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

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CommuterHousehold> __Game_Citizens_CommuterHousehold_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CalendarEventData> __Game_Prefabs_CalendarEventData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		public BufferLookup<TargetElement> __Game_Events_TargetElement_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Duration> __Game_Events_Duration_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HaveCoordinatedMeetingData> __Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<HouseholdCitizen>(true);
			__Game_Citizens_CommuterHousehold_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CommuterHousehold>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Prefabs_CalendarEventData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CalendarEventData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Events_TargetElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TargetElement>(false);
			__Game_Events_Duration_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Duration>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HaveCoordinatedMeetingData>(true);
		}
	}

	private const uint UPDATE_INTERVAL = 16u;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_EventQuery;

	private EntityQuery m_HouseholdQuery;

	private NativeQueue<Attend> m_AttendQueue;

	private EntityArchetype m_MeetingArchetype;

	private EntityArchetype m_JournalDataArchetype;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<Game.Events.CalendarEvent>(),
			ComponentType.ReadWrite<FindingEventParticipants>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_HouseholdQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadWrite<Household>(),
			ComponentType.ReadWrite<UpdateFrame>(),
			ComponentType.ReadWrite<HouseholdCitizen>(),
			ComponentType.Exclude<AttendingEvent>(),
			ComponentType.Exclude<Deleted>()
		});
		m_AttendQueue = new NativeQueue<Attend>(AllocatorHandle.op_Implicit((Allocator)4));
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_MeetingArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<CoordinatedMeeting>(),
			ComponentType.ReadWrite<CoordinatedMeetingAttendee>(),
			ComponentType.ReadWrite<PrefabRef>(),
			ComponentType.ReadWrite<Created>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_JournalDataArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<AddEventJournalData>(),
			ComponentType.ReadWrite<Game.Common.Event>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_EventQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_AttendQueue.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrameWithInterval = SimulationUtils.GetUpdateFrameWithInterval(m_SimulationSystem.frameIndex, (uint)GetUpdateInterval(SystemUpdatePhase.GameSimulation), 16);
		JobHandle val = default(JobHandle);
		ConsiderAttendanceJob considerAttendanceJob = new ConsiderAttendanceJob
		{
			m_EventEntities = ((EntityQuery)(ref m_EventQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenType = InternalCompilerInterface.GetBufferTypeHandle<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CommuterHouseholdType = InternalCompilerInterface.GetComponentTypeHandle<CommuterHousehold>(ref __TypeHandle.__Game_Citizens_CommuterHousehold_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameIndex = updateFrameWithInterval,
			m_CitizenDatas = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Events = InternalCompilerInterface.GetComponentLookup<CalendarEventData>(ref __TypeHandle.__Game_Prefabs_CalendarEventData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttendQueue = m_AttendQueue.AsParallelWriter(),
			m_RandomSeed = RandomSeed.Next()
		};
		EntityCommandBuffer val2 = m_EndFrameBarrier.CreateCommandBuffer();
		considerAttendanceJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		ConsiderAttendanceJob considerAttendanceJob2 = considerAttendanceJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<ConsiderAttendanceJob>(considerAttendanceJob2, m_HouseholdQuery, JobHandle.CombineDependencies(val, ((SystemBase)this).Dependency));
		AttendJob attendJob = new AttendJob
		{
			m_EventEntities = ((EntityQuery)(ref m_EventQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
			m_AttendQueue = m_AttendQueue,
			m_TargetElements = InternalCompilerInterface.GetBufferLookup<TargetElement>(ref __TypeHandle.__Game_Events_TargetElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Durations = InternalCompilerInterface.GetComponentLookup<Duration>(ref __TypeHandle.__Game_Events_Duration_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HaveCoordinatedMeetings = InternalCompilerInterface.GetBufferLookup<HaveCoordinatedMeetingData>(ref __TypeHandle.__Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CalendarEventDatas = InternalCompilerInterface.GetComponentLookup<CalendarEventData>(ref __TypeHandle.__Game_Prefabs_CalendarEventData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeetingArchetype = m_MeetingArchetype,
			m_JournalDataArchetype = m_JournalDataArchetype,
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<AttendJob>(attendJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public FindEventAttendantsSystem()
	{
	}
}

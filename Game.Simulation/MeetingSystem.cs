using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Notifications;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class MeetingSystem : GameSystemBase
{
	[BurstCompile]
	private struct MeetingJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<CoordinatedMeeting> m_MeetingType;

		[ReadOnly]
		public BufferTypeHandle<CoordinatedMeetingAttendee> m_AttendeeType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblems;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildings;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public BufferLookup<HaveCoordinatedMeetingData> m_MeetingDatas;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMembers;

		[ReadOnly]
		public ComponentLookup<AttendingMeeting> m_AttendingMeetings;

		[ReadOnly]
		public ComponentLookup<CalendarEventData> m_CalendarEvents;

		public RandomSeed m_RandomSeed;

		public uint m_SimulationFrame;

		public ParallelWriter m_CommandBuffer;

		public IconCommandBuffer m_IconCommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CoordinatedMeeting> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CoordinatedMeeting>(ref m_MeetingType);
			BufferAccessor<CoordinatedMeetingAttendee> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<CoordinatedMeetingAttendee>(ref m_AttendeeType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CoordinatedMeeting coordinatedMeeting = nativeArray2[i];
				Entity prefab = nativeArray3[i].m_Prefab;
				DynamicBuffer<HaveCoordinatedMeetingData> val = m_MeetingDatas[prefab];
				CalendarEventData calendarEventData = default(CalendarEventData);
				if (m_CalendarEvents.HasComponent(prefab))
				{
					calendarEventData = m_CalendarEvents[prefab];
				}
				bool flag = calendarEventData.m_RandomTargetType == EventTargetType.Couple;
				HaveCoordinatedMeetingData haveCoordinatedMeetingData = default(HaveCoordinatedMeetingData);
				if (coordinatedMeeting.m_Status != MeetingStatus.Done)
				{
					haveCoordinatedMeetingData = val[coordinatedMeeting.m_Phase];
				}
				DynamicBuffer<CoordinatedMeetingAttendee> val2 = bufferAccessor[i];
				Entity val3 = coordinatedMeeting.m_Target;
				if (m_PropertyRenters.HasComponent(val3))
				{
					val3 = m_PropertyRenters[val3].m_Property;
				}
				Entity val4 = default(Entity);
				for (int j = 0; j < val2.Length; j++)
				{
					Entity attendee = val2[j].m_Attendee;
					if (!m_Citizens.HasComponent(attendee) || (m_Citizens[attendee].m_State & CitizenFlags.MovingAwayReachOC) != CitizenFlags.None || m_HealthProblems.HasComponent(attendee))
					{
						coordinatedMeeting.m_Status = MeetingStatus.Done;
						nativeArray2[i] = coordinatedMeeting;
						break;
					}
					if (flag)
					{
						if (j == 0)
						{
							val4 = m_HouseholdMembers[attendee].m_Household;
						}
						else if (val4 != m_HouseholdMembers[attendee].m_Household)
						{
							coordinatedMeeting.m_Status = MeetingStatus.Done;
							nativeArray2[i] = coordinatedMeeting;
							break;
						}
					}
				}
				if (coordinatedMeeting.m_Status == MeetingStatus.Waiting)
				{
					if (coordinatedMeeting.m_Target != Entity.Null)
					{
						coordinatedMeeting.m_Status = MeetingStatus.Traveling;
						nativeArray2[i] = coordinatedMeeting;
					}
				}
				else if (coordinatedMeeting.m_Status == MeetingStatus.Traveling)
				{
					bool flag2 = false;
					for (int k = 0; k < val2.Length; k++)
					{
						Entity attendee2 = val2[k].m_Attendee;
						if (!m_CurrentBuildings.HasComponent(attendee2) || m_CurrentBuildings[attendee2].m_CurrentBuilding != val3)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						if (haveCoordinatedMeetingData.m_Notification != Entity.Null)
						{
							m_IconCommandBuffer.Add(m_CurrentBuildings[val2[0].m_Attendee].m_CurrentBuilding, haveCoordinatedMeetingData.m_Notification, IconPriority.Info, IconClusterLayer.Transaction);
						}
						coordinatedMeeting.m_Status = MeetingStatus.Attending;
						uint num = m_SimulationFrame;
						Random random = m_RandomSeed.GetRandom((int)m_SimulationFrame);
						coordinatedMeeting.m_PhaseEndTime = num + ((Random)(ref random)).NextUInt(haveCoordinatedMeetingData.m_Delay.x, haveCoordinatedMeetingData.m_Delay.y);
						nativeArray2[i] = coordinatedMeeting;
					}
				}
				else if (coordinatedMeeting.m_Status == MeetingStatus.Attending)
				{
					bool flag3 = m_SimulationFrame <= coordinatedMeeting.m_PhaseEndTime;
					if (flag3)
					{
						for (int l = 0; l < val2.Length; l++)
						{
							Entity attendee3 = val2[l].m_Attendee;
							if (m_Citizens.HasComponent(attendee3) && (m_Citizens[attendee3].m_State & CitizenFlags.MovingAwayReachOC) == 0 && !m_HealthProblems.HasComponent(attendee3) && (!m_CurrentBuildings.HasComponent(attendee3) || m_CurrentBuildings[attendee3].m_CurrentBuilding != val3))
							{
								flag3 = false;
								break;
							}
						}
					}
					if (!flag3)
					{
						coordinatedMeeting.m_Phase++;
						if (coordinatedMeeting.m_Phase >= val.Length)
						{
							coordinatedMeeting.m_Status = MeetingStatus.Done;
						}
						else
						{
							coordinatedMeeting.m_Target = default(Entity);
							coordinatedMeeting.m_Status = MeetingStatus.Waiting;
						}
						nativeArray2[i] = coordinatedMeeting;
					}
				}
				else
				{
					if (coordinatedMeeting.m_Status != MeetingStatus.Done)
					{
						continue;
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, nativeArray[i]);
					for (int m = 0; m < val2.Length; m++)
					{
						Entity attendee4 = val2[m].m_Attendee;
						if (m_HouseholdMembers.HasComponent(attendee4))
						{
							val4 = m_HouseholdMembers[attendee4].m_Household;
							if (m_AttendingMeetings.HasComponent(val4))
							{
								((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<AttendingMeeting>(unfilteredChunkIndex, val4);
							}
						}
						if (attendee4 != Entity.Null)
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<AttendingMeeting>(unfilteredChunkIndex, attendee4);
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
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<CoordinatedMeetingAttendee> __Game_Citizens_CoordinatedMeetingAttendee_RO_BufferTypeHandle;

		public ComponentTypeHandle<CoordinatedMeeting> __Game_Citizens_CoordinatedMeeting_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HaveCoordinatedMeetingData> __Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<AttendingMeeting> __Game_Citizens_AttendingMeeting_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CalendarEventData> __Game_Prefabs_CalendarEventData_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_CoordinatedMeetingAttendee_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CoordinatedMeetingAttendee>(true);
			__Game_Citizens_CoordinatedMeeting_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CoordinatedMeeting>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HaveCoordinatedMeetingData>(true);
			__Game_Citizens_AttendingMeeting_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AttendingMeeting>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Prefabs_CalendarEventData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CalendarEventData>(true);
		}
	}

	private EntityQuery m_MeetingGroup;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private IconCommandSystem m_IconCommandSystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_MeetingGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<CoordinatedMeeting>(),
			ComponentType.ReadWrite<CoordinatedMeetingAttendee>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_MeetingGroup);
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
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		MeetingJob meetingJob = new MeetingJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AttendeeType = InternalCompilerInterface.GetBufferTypeHandle<CoordinatedMeetingAttendee>(ref __TypeHandle.__Game_Citizens_CoordinatedMeetingAttendee_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MeetingType = InternalCompilerInterface.GetComponentTypeHandle<CoordinatedMeeting>(ref __TypeHandle.__Game_Citizens_CoordinatedMeeting_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildings = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeetingDatas = InternalCompilerInterface.GetBufferLookup<HaveCoordinatedMeetingData>(ref __TypeHandle.__Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttendingMeetings = InternalCompilerInterface.GetComponentLookup<AttendingMeeting>(ref __TypeHandle.__Game_Citizens_AttendingMeeting_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMembers = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CalendarEvents = InternalCompilerInterface.GetComponentLookup<CalendarEventData>(ref __TypeHandle.__Game_Prefabs_CalendarEventData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_SimulationFrame = m_SimulationSystem.frameIndex
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		meetingJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		meetingJob.m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer();
		MeetingJob meetingJob2 = meetingJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<MeetingJob>(meetingJob2, m_MeetingGroup, ((SystemBase)this).Dependency);
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
	public MeetingSystem()
	{
	}
}

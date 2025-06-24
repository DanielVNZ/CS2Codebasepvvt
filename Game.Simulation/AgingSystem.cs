using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Collections;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Debug;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class AgingSystem : GameSystemBase
{
	[BurstCompile]
	private struct AgingJob : IJobChunk
	{
		public Concurrent m_BecomeTeenCounter;

		public Concurrent m_BecomeAdultCounter;

		public Concurrent m_BecomeElderCounter;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> m_HouseholdCitizenType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_TravelPurposes;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> m_Students;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public TimeData m_TimeData;

		public ParallelWriter m_CommandBuffer;

		[ReadOnly]
		public bool m_DebugAgeAllCitizens;

		private void LeaveSchool(int chunkIndex, Entity citizenEntity, ComponentLookup<Game.Citizens.Student> students)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			Entity school = students[citizenEntity].m_School;
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Citizens.Student>(chunkIndex, citizenEntity);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<StudentsRemoved>(chunkIndex, school);
		}

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			if (!m_DebugAgeAllCitizens && ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			BufferAccessor<HouseholdCitizen> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<HouseholdCitizen>(ref m_HouseholdCitizenType);
			int day = TimeSystem.GetDay(m_SimulationFrame, m_TimeData);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<HouseholdCitizen> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					Entity citizen = val[j].m_Citizen;
					Citizen citizen2 = m_Citizens[citizen];
					CitizenAge age = citizen2.GetAge();
					int num = day - citizen2.m_BirthDay;
					int num2;
					if (age == CitizenAge.Child)
					{
						num2 = GetTeenAgeLimitInDays();
					}
					else if (age == CitizenAge.Teen)
					{
						num2 = GetAdultAgeLimitInDays();
					}
					else
					{
						if (age != CitizenAge.Adult)
						{
							continue;
						}
						num2 = GetElderAgeLimitInDays();
					}
					if (num < num2)
					{
						continue;
					}
					switch (age)
					{
					case CitizenAge.Child:
						if (m_Students.HasComponent(citizen))
						{
							LeaveSchool(unfilteredChunkIndex, citizen, m_Students);
						}
						((Concurrent)(ref m_BecomeTeenCounter)).Increment();
						citizen2.SetAge(CitizenAge.Teen);
						m_Citizens[citizen] = citizen2;
						break;
					case CitizenAge.Teen:
						if (m_Students.HasComponent(citizen))
						{
							LeaveSchool(unfilteredChunkIndex, citizen, m_Students);
						}
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LeaveHouseholdTag>(unfilteredChunkIndex, citizen);
						((Concurrent)(ref m_BecomeAdultCounter)).Increment();
						citizen2.SetAge(CitizenAge.Adult);
						m_Citizens[citizen] = citizen2;
						break;
					case CitizenAge.Adult:
						if (m_TravelPurposes.HasComponent(citizen) && (m_TravelPurposes[citizen].m_Purpose == Purpose.GoingToWork || m_TravelPurposes[citizen].m_Purpose == Purpose.Working))
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, citizen);
						}
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Worker>(unfilteredChunkIndex, citizen);
						((Concurrent)(ref m_BecomeElderCounter)).Increment();
						citizen2.SetAge(CitizenAge.Elderly);
						m_Citizens[citizen] = citizen2;
						break;
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
		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentLookup;

		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RW_ComponentLookup;

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
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<HouseholdCitizen>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_Citizens_Student_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Citizens.Student>(true);
			__Game_Citizens_Citizen_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(false);
		}
	}

	public static readonly int kUpdatesPerDay = 1;

	private EntityQuery m_HouseholdQuery;

	private EntityQuery m_TimeDataQuery;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	public static bool s_DebugAgeAllCitizens = false;

	[DebugWatchValue]
	public NativeValue<int> m_BecomeTeen;

	[DebugWatchValue]
	public NativeValue<int> m_BecomeAdult;

	[DebugWatchValue]
	public NativeValue<int> m_BecomeElder;

	public NativeCounter m_BecomeTeenCounter;

	public NativeCounter m_BecomeAdultCounter;

	public NativeCounter m_BecomeElderCounter;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

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
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Household>() };
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_HouseholdQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		m_BecomeTeen = new NativeValue<int>((Allocator)4);
		m_BecomeAdult = new NativeValue<int>((Allocator)4);
		m_BecomeElder = new NativeValue<int>((Allocator)4);
		m_BecomeTeenCounter = new NativeCounter((Allocator)4);
		m_BecomeAdultCounter = new NativeCounter((Allocator)4);
		m_BecomeElderCounter = new NativeCounter((Allocator)4);
		((ComponentSystemBase)this).RequireForUpdate(m_HouseholdQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_BecomeTeen.Dispose();
		m_BecomeAdult.Dispose();
		m_BecomeElder.Dispose();
		((NativeCounter)(ref m_BecomeTeenCounter)).Dispose();
		((NativeCounter)(ref m_BecomeAdultCounter)).Dispose();
		((NativeCounter)(ref m_BecomeElderCounter)).Dispose();
	}

	public static int GetTeenAgeLimitInDays()
	{
		return 21;
	}

	public static int GetAdultAgeLimitInDays()
	{
		return 36;
	}

	public static int GetElderAgeLimitInDays()
	{
		return 84;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		AgingJob agingJob = new AgingJob
		{
			m_BecomeTeenCounter = ((NativeCounter)(ref m_BecomeTeenCounter)).ToConcurrent(),
			m_BecomeAdultCounter = ((NativeCounter)(ref m_BecomeAdultCounter)).ToConcurrent(),
			m_BecomeElderCounter = ((NativeCounter)(ref m_BecomeElderCounter)).ToConcurrent(),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenType = InternalCompilerInterface.GetBufferTypeHandle<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TravelPurposes = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Students = InternalCompilerInterface.GetComponentLookup<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_TimeData = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>(),
			m_UpdateFrameIndex = updateFrame
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		agingJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		agingJob.m_DebugAgeAllCitizens = s_DebugAgeAllCitizens;
		AgingJob agingJob2 = agingJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<AgingJob>(agingJob2, m_HouseholdQuery, ((SystemBase)this).Dependency);
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
	public AgingSystem()
	{
	}
}

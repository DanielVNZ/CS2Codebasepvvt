using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Game.Triggers;
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
public class WorkerSystem : GameSystemBase
{
	[BurstCompile]
	private struct GoToWorkJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public ComponentTypeHandle<Worker> m_WorkerType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingType;

		public BufferTypeHandle<TripNeeded> m_TripType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_Purposes;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_Properties;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public ComponentLookup<CarKeeper> m_CarKeepers;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<AttendingMeeting> m_Attendings;

		[ReadOnly]
		public ComponentLookup<Population> m_PopulationData;

		public ParallelWriter<TriggerAction> m_TriggerBuffer;

		public uint m_Frame;

		public TimeData m_TimeData;

		public uint m_UpdateFrameIndex;

		public float m_TimeOfDay;

		public Entity m_PopulationEntity;

		public EconomyParameterData m_EconomyParameters;

		public ParallelWriter<Entity> m_CarReserverQueue;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Citizen> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Citizen>(ref m_CitizenType);
			NativeArray<Worker> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Worker>(ref m_WorkerType);
			NativeArray<CurrentBuilding> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingType);
			BufferAccessor<TripNeeded> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TripNeeded>(ref m_TripType);
			int population = m_PopulationData[m_PopulationEntity].m_Population;
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Citizen citizen = nativeArray2[i];
				if (IsTodayOffDay(citizen, ref m_EconomyParameters, m_Frame, m_TimeData, population) || !IsTimeToWork(citizen, nativeArray3[i], ref m_EconomyParameters, m_TimeOfDay))
				{
					continue;
				}
				DynamicBuffer<TripNeeded> val2 = bufferAccessor[i];
				if (m_Attendings.HasComponent(val) || (citizen.m_State & CitizenFlags.MovingAwayReachOC) != CitizenFlags.None)
				{
					continue;
				}
				Entity workplace = nativeArray3[i].m_Workplace;
				Entity val3 = Entity.Null;
				if (m_Properties.HasComponent(workplace))
				{
					val3 = m_Properties[workplace].m_Property;
				}
				else if (m_Buildings.HasComponent(workplace))
				{
					val3 = workplace;
				}
				else if (m_OutsideConnections.HasComponent(workplace))
				{
					val3 = workplace;
				}
				if (val3 != Entity.Null)
				{
					if (nativeArray4[i].m_CurrentBuilding != val3)
					{
						if (!m_CarKeepers.IsComponentEnabled(val))
						{
							m_CarReserverQueue.Enqueue(val);
						}
						val2.Add(new TripNeeded
						{
							m_TargetAgent = workplace,
							m_Purpose = Purpose.GoingToWork
						});
					}
				}
				else
				{
					citizen.SetFailedEducationCount(0);
					nativeArray2[i] = citizen;
					if (m_Purposes.HasComponent(val) && (m_Purposes[val].m_Purpose == Purpose.GoingToWork || m_Purposes[val].m_Purpose == Purpose.Working))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
					}
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Worker>(unfilteredChunkIndex, val);
					m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenBecameUnemployed, Entity.Null, val, workplace));
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct WorkJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Worker> m_WorkerType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<TravelPurpose> m_PurposeType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public ComponentLookup<WorkProvider> m_Workplaces;

		[ReadOnly]
		public ComponentLookup<AttendingMeeting> m_Attendings;

		public EconomyParameterData m_EconomyParameters;

		public ParallelWriter<TriggerAction> m_TriggerBuffer;

		public float m_TimeOfDay;

		public uint m_UpdateFrameIndex;

		public uint m_Frame;

		public TimeData m_TimeData;

		public int m_Population;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Worker> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Worker>(ref m_WorkerType);
			NativeArray<TravelPurpose> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TravelPurpose>(ref m_PurposeType);
			NativeArray<Citizen> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Citizen>(ref m_CitizenType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Entity workplace = nativeArray2[i].m_Workplace;
				Worker worker = nativeArray2[i];
				Citizen citizen = nativeArray4[i];
				if (!m_Workplaces.HasComponent(workplace))
				{
					citizen.SetFailedEducationCount(0);
					nativeArray4[i] = citizen;
					TravelPurpose travelPurpose = nativeArray3[i];
					if (travelPurpose.m_Purpose == Purpose.GoingToWork || travelPurpose.m_Purpose == Purpose.Working)
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
					}
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Worker>(unfilteredChunkIndex, val);
					m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenBecameUnemployed, Entity.Null, val, workplace));
				}
				else if ((!IsTimeToWork(citizen, worker, ref m_EconomyParameters, m_TimeOfDay) || m_Attendings.HasComponent(val)) && nativeArray3[i].m_Purpose == Purpose.Working)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
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

		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Worker> __Game_Citizens_Worker_RO_ComponentTypeHandle;

		public BufferTypeHandle<TripNeeded> __Game_Citizens_TripNeeded_RW_BufferTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarKeeper> __Game_Citizens_CarKeeper_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AttendingMeeting> __Game_Citizens_AttendingMeeting_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Population> __Game_City_Population_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<WorkProvider> __Game_Companies_WorkProvider_RO_ComponentLookup;

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
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_Citizen_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(false);
			__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(true);
			__Game_Citizens_Worker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Worker>(true);
			__Game_Citizens_TripNeeded_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TripNeeded>(false);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Citizens_CarKeeper_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarKeeper>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_Citizens_AttendingMeeting_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AttendingMeeting>(true);
			__Game_City_Population_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TravelPurpose>(true);
			__Game_Companies_WorkProvider_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkProvider>(true);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private TimeSystem m_TimeSystem;

	private CitizenBehaviorSystem m_CitizenBehaviorSystem;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_GotoWorkQuery;

	private EntityQuery m_WorkerQuery;

	private EntityQuery m_TimeDataQuery;

	private EntityQuery m_PopulationQuery;

	private SimulationSystem m_SimulationSystem;

	private TriggerSystem m_TriggerSystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public static float GetWorkOffset(Citizen citizen)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Random pseudoRandom = citizen.GetPseudoRandom(CitizenPseudoRandom.WorkOffset);
		return (float)(-10922 + ((Random)(ref pseudoRandom)).NextInt(21845)) / 262144f;
	}

	public static bool IsTodayOffDay(Citizen citizen, ref EconomyParameterData economyParameters, uint frame, TimeData timeData, int population)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		int num = math.min(40, Mathf.RoundToInt(100f / math.max(1f, math.sqrt(economyParameters.m_TrafficReduction * (float)population))));
		int day = TimeSystem.GetDay(frame, timeData);
		Random val = Random.CreateFromIndex((uint)(citizen.m_PseudoRandom + day));
		if (((Random)(ref val)).NextInt(100) > num)
		{
			return true;
		}
		return false;
	}

	public static bool IsTimeToWork(Citizen citizen, Worker worker, ref EconomyParameterData economyParameters, float timeOfDay)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		float2 timeToWork = GetTimeToWork(citizen, worker, ref economyParameters, includeCommute: true);
		if (!(timeToWork.x < timeToWork.y))
		{
			if (!(timeOfDay >= timeToWork.x))
			{
				return timeOfDay <= timeToWork.y;
			}
			return true;
		}
		if (timeOfDay >= timeToWork.x)
		{
			return timeOfDay <= timeToWork.y;
		}
		return false;
	}

	public static float2 GetTimeToWork(Citizen citizen, Worker worker, ref EconomyParameterData economyParameters, bool includeCommute)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		float num = GetWorkOffset(citizen);
		if (worker.m_Shift == Workshift.Evening)
		{
			num += 0.33f;
		}
		else if (worker.m_Shift == Workshift.Night)
		{
			num += 0.67f;
		}
		float num2 = math.frac((float)Mathf.RoundToInt(24f * (economyParameters.m_WorkDayStart + num)) / 24f);
		float num3 = math.frac((float)Mathf.RoundToInt(24f * (economyParameters.m_WorkDayEnd + num)) / 24f);
		float num4 = 0f;
		if (includeCommute)
		{
			num4 = 60f * worker.m_LastCommuteTime;
			if (num4 < 60f)
			{
				num4 = 40000f;
			}
			num4 /= 262144f;
		}
		return new float2(math.frac(num2 - num4), num3);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitizenBehaviorSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitizenBehaviorSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_WorkerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Worker>(),
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<TravelPurpose>(),
			ComponentType.ReadOnly<CurrentBuilding>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_GotoWorkQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[9]
		{
			ComponentType.ReadOnly<Worker>(),
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<CurrentBuilding>(),
			ComponentType.Exclude<TravelPurpose>(),
			ComponentType.Exclude<HealthProblem>(),
			ComponentType.Exclude<ResourceBuyer>(),
			ComponentType.ReadWrite<TripNeeded>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		m_PopulationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Population>() });
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_GotoWorkQuery, m_WorkerQuery });
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrameWithInterval = SimulationUtils.GetUpdateFrameWithInterval(m_SimulationSystem.frameIndex, (uint)GetUpdateInterval(SystemUpdatePhase.GameSimulation), 16);
		JobHandle deps;
		GoToWorkJob goToWorkJob = new GoToWorkJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WorkerType = InternalCompilerInterface.GetComponentTypeHandle<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TripType = InternalCompilerInterface.GetBufferTypeHandle<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarKeepers = InternalCompilerInterface.GetComponentLookup<CarKeeper>(ref __TypeHandle.__Game_Citizens_CarKeeper_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Properties = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Purposes = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Attendings = InternalCompilerInterface.GetComponentLookup<AttendingMeeting>(ref __TypeHandle.__Game_Citizens_AttendingMeeting_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PopulationData = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer().AsParallelWriter(),
			m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_TimeOfDay = m_TimeSystem.normalizedTime,
			m_UpdateFrameIndex = updateFrameWithInterval,
			m_Frame = m_SimulationSystem.frameIndex,
			m_TimeData = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>(),
			m_PopulationEntity = ((EntityQuery)(ref m_PopulationQuery)).GetSingletonEntity(),
			m_CarReserverQueue = m_CitizenBehaviorSystem.GetCarReserveQueue(out deps)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		goToWorkJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<GoToWorkJob>(goToWorkJob, m_GotoWorkQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		m_CitizenBehaviorSystem.AddCarReserveWriter(val2);
		m_TriggerSystem.AddActionBufferWriter(val2);
		WorkJob workJob = new WorkJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WorkerType = InternalCompilerInterface.GetComponentTypeHandle<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PurposeType = InternalCompilerInterface.GetComponentTypeHandle<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Attendings = InternalCompilerInterface.GetComponentLookup<AttendingMeeting>(ref __TypeHandle.__Game_Citizens_AttendingMeeting_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Workplaces = InternalCompilerInterface.GetComponentLookup<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer().AsParallelWriter(),
			m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_UpdateFrameIndex = updateFrameWithInterval,
			m_TimeOfDay = m_TimeSystem.normalizedTime,
			m_Frame = m_SimulationSystem.frameIndex,
			m_TimeData = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>()
		};
		val = m_EndFrameBarrier.CreateCommandBuffer();
		workJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<WorkJob>(workJob, m_WorkerQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val2));
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		m_TriggerSystem.AddActionBufferWriter(val3);
		((SystemBase)this).Dependency = val3;
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
	public WorkerSystem()
	{
	}
}

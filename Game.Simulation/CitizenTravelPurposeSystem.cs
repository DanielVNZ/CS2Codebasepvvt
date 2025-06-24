using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CitizenTravelPurposeSystem : GameSystemBase
{
	[BurstCompile]
	private struct CitizenArriveJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingType;

		public ComponentTypeHandle<TravelPurpose> m_TravelPurposeType;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> m_HealthProblemType;

		[ReadOnly]
		public ComponentTypeHandle<Arrived> m_ArrivedType;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> m_Students;

		[ReadOnly]
		public ComponentLookup<WorkProvider> m_WorkProviders;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.School> m_Schools;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.PoliceStation> m_PoliceStationData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Prison> m_PrisonData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Hospital> m_HospitalData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.DeathcareFacility> m_DeathcareFacilityData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.EmergencyShelter> m_EmergencyShelterData;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblems;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<Arrive> m_ArriveQueue;

		public EconomyParameterData m_EconomyParameters;

		public float m_NormalizedTime;

		public RandomSeed m_RandomSeed;

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
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<TravelPurpose> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TravelPurpose>(ref m_TravelPurposeType);
			NativeArray<CurrentBuilding> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<HealthProblem>(ref m_HealthProblemType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				bool flag2 = ((ArchetypeChunk)(ref chunk)).IsComponentEnabled<Arrived>(ref m_ArrivedType, i);
				Entity val = nativeArray[i];
				TravelPurpose travelPurpose = nativeArray2[i];
				if (flag && CitizenUtils.IsDead(val, ref m_HealthProblems) && travelPurpose.m_Purpose != Purpose.Deathcare && travelPurpose.m_Purpose != Purpose.InDeathcare && travelPurpose.m_Purpose != Purpose.Hospital && travelPurpose.m_Purpose != Purpose.InHospital)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
				}
				else if (travelPurpose.m_Purpose == Purpose.Sleeping)
				{
					Citizen citizen = m_Citizens[val];
					if (!CitizenBehaviorSystem.IsSleepTime(val, citizen, ref m_EconomyParameters, m_NormalizedTime, ref m_Workers, ref m_Students))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
						if (nativeArray3.Length != 0 && m_BuildingData.HasComponent(nativeArray3[i].m_CurrentBuilding))
						{
							m_ArriveQueue.Enqueue(new Arrive(val, nativeArray3[i].m_CurrentBuilding, ArriveType.WakeUp));
						}
					}
				}
				else if (travelPurpose.m_Purpose == Purpose.VisitAttractions)
				{
					if (flag2)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<Arrived>(unfilteredChunkIndex, val, false);
					}
					if (((Random)(ref random)).NextInt(100) == 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
					}
				}
				else
				{
					if (!flag2)
					{
						continue;
					}
					((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<Arrived>(unfilteredChunkIndex, val, false);
					switch (travelPurpose.m_Purpose)
					{
					case Purpose.GoingHome:
						if (nativeArray3.Length != 0 && m_BuildingData.HasComponent(nativeArray3[i].m_CurrentBuilding))
						{
							m_ArriveQueue.Enqueue(new Arrive(val, nativeArray3[i].m_CurrentBuilding, ArriveType.Resident));
						}
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
						break;
					case Purpose.None:
					case Purpose.Shopping:
					case Purpose.Leisure:
					case Purpose.Exporting:
					case Purpose.MovingAway:
					case Purpose.Safety:
					case Purpose.Escape:
					case Purpose.Traveling:
					case Purpose.SendMail:
					case Purpose.Disappear:
					case Purpose.WaitingHome:
					case Purpose.PathFailed:
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
						break;
					case Purpose.EmergencyShelter:
						if (nativeArray3.Length != 0 && m_EmergencyShelterData.HasComponent(nativeArray3[i].m_CurrentBuilding))
						{
							travelPurpose.m_Purpose = Purpose.InEmergencyShelter;
							nativeArray2[i] = travelPurpose;
							m_ArriveQueue.Enqueue(new Arrive(val, nativeArray3[i].m_CurrentBuilding, ArriveType.Occupant));
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
						}
						break;
					case Purpose.GoingToWork:
						if (nativeArray3.Length != 0 && m_BuildingData.HasComponent(nativeArray3[i].m_CurrentBuilding))
						{
							m_ArriveQueue.Enqueue(new Arrive(val, nativeArray3[i].m_CurrentBuilding, ArriveType.Worker));
						}
						if (m_Workers.HasComponent(val))
						{
							Entity workplace = m_Workers[val].m_Workplace;
							if (m_WorkProviders.HasComponent(workplace))
							{
								travelPurpose.m_Purpose = Purpose.Working;
								nativeArray2[i] = travelPurpose;
							}
							else
							{
								((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
							}
						}
						break;
					case Purpose.GoingToSchool:
						if (m_Students.HasComponent(val))
						{
							Entity school = m_Students[val].m_School;
							if (m_Schools.HasComponent(school))
							{
								travelPurpose.m_Purpose = Purpose.Studying;
								nativeArray2[i] = travelPurpose;
							}
							else
							{
								((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
							}
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
						}
						break;
					case Purpose.GoingToJail:
						if (nativeArray3.Length != 0 && m_PoliceStationData.HasComponent(nativeArray3[i].m_CurrentBuilding))
						{
							travelPurpose.m_Purpose = Purpose.InJail;
							nativeArray2[i] = travelPurpose;
							m_ArriveQueue.Enqueue(new Arrive(val, nativeArray3[i].m_CurrentBuilding, ArriveType.Occupant));
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
						}
						break;
					case Purpose.GoingToPrison:
						if (nativeArray3.Length != 0 && m_PrisonData.HasComponent(nativeArray3[i].m_CurrentBuilding))
						{
							travelPurpose.m_Purpose = Purpose.InPrison;
							nativeArray2[i] = travelPurpose;
							m_ArriveQueue.Enqueue(new Arrive(val, nativeArray3[i].m_CurrentBuilding, ArriveType.Occupant));
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
						}
						break;
					case Purpose.Hospital:
						if (nativeArray3.Length != 0 && m_HospitalData.HasComponent(nativeArray3[i].m_CurrentBuilding))
						{
							travelPurpose.m_Purpose = Purpose.InHospital;
							nativeArray2[i] = travelPurpose;
							m_ArriveQueue.Enqueue(new Arrive(val, nativeArray3[i].m_CurrentBuilding, ArriveType.Patient));
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
						}
						break;
					case Purpose.Deathcare:
						if (nativeArray3.Length != 0 && m_DeathcareFacilityData.HasComponent(nativeArray3[i].m_CurrentBuilding))
						{
							travelPurpose.m_Purpose = Purpose.InDeathcare;
							nativeArray2[i] = travelPurpose;
							m_ArriveQueue.Enqueue(new Arrive(val, nativeArray3[i].m_CurrentBuilding, ArriveType.Patient));
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
						}
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

	private struct Arrive
	{
		public Entity m_Citizen;

		public Entity m_Target;

		public ArriveType m_Type;

		public Arrive(Entity citizen, Entity target, ArriveType type)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Citizen = citizen;
			m_Target = target;
			m_Type = type;
		}
	}

	private enum ArriveType
	{
		Patient,
		Occupant,
		Resident,
		Worker,
		WakeUp
	}

	[BurstCompile]
	private struct ArriveJob : IJob
	{
		public ComponentLookup<CitizenPresence> m_CitizenPresenceData;

		public BufferLookup<Patient> m_Patients;

		public BufferLookup<Occupant> m_Occupants;

		public ComponentLookup<Household> m_Households;

		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMembers;

		public NativeQueue<StatisticsEvent> m_StatisticsQueue;

		public NativeQueue<Arrive> m_ArriveQueue;

		private void SetPresent(Arrive arrive)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if (m_CitizenPresenceData.HasComponent(arrive.m_Target))
			{
				CitizenPresence citizenPresence = m_CitizenPresenceData[arrive.m_Target];
				citizenPresence.m_Delta = (sbyte)math.min(127, citizenPresence.m_Delta + 1);
				m_CitizenPresenceData[arrive.m_Target] = citizenPresence;
			}
		}

		public void Execute()
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			int count = m_ArriveQueue.Count;
			for (int i = 0; i < count; i++)
			{
				Arrive present = m_ArriveQueue.Dequeue();
				switch (present.m_Type)
				{
				case ArriveType.Patient:
					if (m_Patients.HasBuffer(present.m_Target))
					{
						CollectionUtils.TryAddUniqueValue<Patient>(m_Patients[present.m_Target], new Patient(present.m_Citizen));
					}
					break;
				case ArriveType.Occupant:
					if (m_Occupants.HasBuffer(present.m_Target))
					{
						CollectionUtils.TryAddUniqueValue<Occupant>(m_Occupants[present.m_Target], new Occupant(present.m_Citizen));
					}
					break;
				case ArriveType.Resident:
				{
					Entity household = m_HouseholdMembers[present.m_Citizen].m_Household;
					if (m_PropertyRenters.HasComponent(household) && m_PropertyRenters[household].m_Property == present.m_Target)
					{
						Household household2 = m_Households[household];
						if (m_HouseholdCitizens.HasBuffer(household) && (household2.m_Flags & HouseholdFlags.MovedIn) == 0)
						{
							m_StatisticsQueue.Enqueue(new StatisticsEvent
							{
								m_Statistic = StatisticType.CitizensMovedIn,
								m_Change = m_HouseholdCitizens[household].Length
							});
						}
						household2.m_Flags |= HouseholdFlags.MovedIn;
						m_Households[household] = household2;
					}
					SetPresent(present);
					break;
				}
				case ArriveType.Worker:
				case ArriveType.WakeUp:
					SetPresent(present);
					break;
				}
			}
		}
	}

	[BurstCompile]
	private struct CitizenStuckJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> m_HouseholdMemberType;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> m_HealthProblemType;

		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public ComponentLookup<MovingAway> m_MovingAways;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public NativeList<Entity> m_OutsideConnections;

		[ReadOnly]
		public NativeList<Entity> m_ServiceBuildings;

		public ParallelWriter m_CommandBuffer;

		public RandomSeed m_RandomSeed;

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
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<HouseholdMember> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdMember>(ref m_HouseholdMemberType);
			NativeArray<HealthProblem> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HealthProblem>(ref m_HealthProblemType);
			NativeArray<Citizen> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Citizen>(ref m_CitizenType);
			if (nativeArray2.Length < ((ArchetypeChunk)(ref chunk)).Count || m_OutsideConnections.Length == 0)
			{
				return;
			}
			HealthProblem healthProblem = default(HealthProblem);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				Entity household = nativeArray2[i].m_Household;
				bool flag = (m_Households[household].m_Flags & HouseholdFlags.MovedIn) != HouseholdFlags.None && !m_MovingAways.HasComponent(household);
				if (CollectionUtils.TryGet<HealthProblem>(nativeArray3, i, ref healthProblem) && (healthProblem.m_Flags & HealthProblemFlags.Dead) != HealthProblemFlags.None)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, nativeArray[i]);
					continue;
				}
				Entity val2 = Entity.Null;
				Random random = m_RandomSeed.GetRandom((1 + i) * (val.Index + 1));
				if (flag)
				{
					if (m_PropertyRenters.HasComponent(household))
					{
						val2 = m_PropertyRenters[household].m_Property;
					}
					if (val2 == Entity.Null && m_ServiceBuildings.Length > 0)
					{
						int num = 0;
						do
						{
							num++;
							val2 = m_ServiceBuildings[((Random)(ref random)).NextInt(m_ServiceBuildings.Length)];
						}
						while ((!m_Buildings.HasComponent(val2) || m_Buildings[val2].m_RoadEdge == Entity.Null) && num < 10);
					}
					if (!m_Buildings.HasComponent(val2) || m_Buildings[val2].m_RoadEdge == Entity.Null)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, nativeArray[i]);
					}
				}
				else
				{
					val2 = m_OutsideConnections[((Random)(ref random)).NextInt(m_OutsideConnections.Length)];
				}
				if (val2 != Entity.Null)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentBuilding>(unfilteredChunkIndex, nativeArray[i], new CurrentBuilding
					{
						m_CurrentBuilding = val2
					});
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, nativeArray[i]);
					Citizen citizen = nativeArray4[i];
					citizen.m_PenaltyCounter = byte.MaxValue;
					nativeArray4[i] = citizen;
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
		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle;

		public ComponentTypeHandle<TravelPurpose> __Game_Citizens_TravelPurpose_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Arrived> __Game_Citizens_Arrived_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.School> __Game_Buildings_School_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkProvider> __Game_Companies_WorkProvider_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.PoliceStation> __Game_Buildings_PoliceStation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Prison> __Game_Buildings_Prison_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Hospital> __Game_Buildings_Hospital_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.DeathcareFacility> __Game_Buildings_DeathcareFacility_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.EmergencyShelter> __Game_Buildings_EmergencyShelter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		public ComponentLookup<CitizenPresence> __Game_Buildings_CitizenPresence_RW_ComponentLookup;

		public BufferLookup<Patient> __Game_Buildings_Patient_RW_BufferLookup;

		public BufferLookup<Occupant> __Game_Buildings_Occupant_RW_BufferLookup;

		public ComponentLookup<Household> __Game_Citizens_Household_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MovingAway> __Game_Agents_MovingAway_RO_ComponentLookup;

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
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(true);
			__Game_Citizens_TravelPurpose_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TravelPurpose>(false);
			__Game_Citizens_HealthProblem_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HealthProblem>(true);
			__Game_Citizens_Arrived_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Arrived>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_School_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.School>(true);
			__Game_Companies_WorkProvider_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkProvider>(true);
			__Game_Citizens_Student_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Citizens.Student>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Buildings_PoliceStation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.PoliceStation>(true);
			__Game_Buildings_Prison_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Prison>(true);
			__Game_Buildings_Hospital_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Hospital>(true);
			__Game_Buildings_DeathcareFacility_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.DeathcareFacility>(true);
			__Game_Buildings_EmergencyShelter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.EmergencyShelter>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Buildings_CitizenPresence_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CitizenPresence>(false);
			__Game_Buildings_Patient_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Patient>(false);
			__Game_Buildings_Occupant_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Occupant>(false);
			__Game_Citizens_Household_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(false);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdMember>(true);
			__Game_Citizens_Citizen_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(false);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Agents_MovingAway_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MovingAway>(true);
		}
	}

	private TimeSystem m_TimeSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_ArrivedGroup;

	private EntityQuery m_StuckGroup;

	private EntityQuery m_EconomyParameterGroup;

	private EntityQuery m_OutsideConnectionQuery;

	private EntityQuery m_ServiceBuildingQuery;

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
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ArrivedGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadWrite<TravelPurpose>(),
			ComponentType.ReadWrite<TripNeeded>(),
			ComponentType.ReadOnly<CurrentBuilding>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_StuckGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadWrite<TravelPurpose>(),
			ComponentType.ReadWrite<TripNeeded>(),
			ComponentType.Exclude<CurrentTransport>(),
			ComponentType.Exclude<CurrentBuilding>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_OutsideConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadWrite<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Game.Objects.ElectricityOutsideConnection>(),
			ComponentType.Exclude<Game.Objects.WaterPipeOutsideConnection>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_ServiceBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadWrite<CityServiceUpkeep>(),
			ComponentType.ReadWrite<Building>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Temp>()
		});
		m_EconomyParameterGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_ArrivedGroup, m_StuckGroup });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<Arrive> arriveQueue = default(NativeQueue<Arrive>);
		arriveQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		CitizenArriveJob citizenArriveJob = new CitizenArriveJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TravelPurposeType = InternalCompilerInterface.GetComponentTypeHandle<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemType = InternalCompilerInterface.GetComponentTypeHandle<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ArrivedType = InternalCompilerInterface.GetComponentTypeHandle<Arrived>(ref __TypeHandle.__Game_Citizens_Arrived_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Schools = InternalCompilerInterface.GetComponentLookup<Game.Buildings.School>(ref __TypeHandle.__Game_Buildings_School_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkProviders = InternalCompilerInterface.GetComponentLookup<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Students = InternalCompilerInterface.GetComponentLookup<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceStationData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.PoliceStation>(ref __TypeHandle.__Game_Buildings_PoliceStation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrisonData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Prison>(ref __TypeHandle.__Game_Buildings_Prison_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HospitalData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Hospital>(ref __TypeHandle.__Game_Buildings_Hospital_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeathcareFacilityData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.DeathcareFacility>(ref __TypeHandle.__Game_Buildings_DeathcareFacility_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EmergencyShelterData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.EmergencyShelter>(ref __TypeHandle.__Game_Buildings_EmergencyShelter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterGroup)).GetSingleton<EconomyParameterData>()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		citizenArriveJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		citizenArriveJob.m_ArriveQueue = arriveQueue.AsParallelWriter();
		citizenArriveJob.m_NormalizedTime = m_TimeSystem.normalizedTime;
		citizenArriveJob.m_RandomSeed = RandomSeed.Next();
		CitizenArriveJob citizenArriveJob2 = citizenArriveJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CitizenArriveJob>(citizenArriveJob2, m_ArrivedGroup, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		JobHandle deps;
		JobHandle val2 = IJobExtensions.Schedule<ArriveJob>(new ArriveJob
		{
			m_CitizenPresenceData = InternalCompilerInterface.GetComponentLookup<CitizenPresence>(ref __TypeHandle.__Game_Buildings_CitizenPresence_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Patients = InternalCompilerInterface.GetBufferLookup<Patient>(ref __TypeHandle.__Game_Buildings_Patient_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Occupants = InternalCompilerInterface.GetBufferLookup<Occupant>(ref __TypeHandle.__Game_Buildings_Occupant_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMembers = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StatisticsQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps),
			m_ArriveQueue = arriveQueue
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		arriveQueue.Dispose(val2);
		m_CityStatisticsSystem.AddWriter(val2);
		JobHandle val3 = default(JobHandle);
		JobHandle val4 = default(JobHandle);
		CitizenStuckJob citizenStuckJob = new CitizenStuckJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMemberType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemType = InternalCompilerInterface.GetComponentTypeHandle<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingAways = InternalCompilerInterface.GetComponentLookup<MovingAway>(ref __TypeHandle.__Game_Agents_MovingAway_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_ServiceBuildings = ((EntityQuery)(ref m_ServiceBuildingQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val3),
			m_OutsideConnections = ((EntityQuery)(ref m_OutsideConnectionQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val4)
		};
		val = m_EndFrameBarrier.CreateCommandBuffer();
		citizenStuckJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val5 = JobChunkExtensions.ScheduleParallel<CitizenStuckJob>(citizenStuckJob, m_StuckGroup, JobUtils.CombineDependencies(val4, val3, val2, ((SystemBase)this).Dependency));
		m_EndFrameBarrier.AddJobHandleForProducer(val5);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val2, val5);
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
	public CitizenTravelPurposeSystem()
	{
	}
}

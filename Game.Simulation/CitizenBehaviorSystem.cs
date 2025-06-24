using System.Runtime.CompilerServices;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Events;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
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
public class CitizenBehaviorSystem : GameSystemBase
{
	[BurstCompile]
	private struct CitizenReserveHouseholdCarJob : IJob
	{
		public ComponentLookup<CarKeeper> m_CarKeepers;

		public ComponentLookup<Game.Vehicles.PersonalCar> m_PersonalCars;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMembers;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		public NativeQueue<Entity> m_ReserverQueue;

		public void Execute()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			Entity val = default(Entity);
			while (m_ReserverQueue.TryDequeue(ref val))
			{
				if (m_HouseholdMembers.HasComponent(val))
				{
					Entity household = m_HouseholdMembers[val].m_Household;
					Entity car = Entity.Null;
					if (m_Citizens[val].GetAge() != CitizenAge.Child && HouseholdBehaviorSystem.GetFreeCar(household, m_OwnedVehicles, m_PersonalCars, ref car) && !m_CarKeepers.IsComponentEnabled(val))
					{
						m_CarKeepers.SetComponentEnabled(val, true);
						m_CarKeepers[val] = new CarKeeper
						{
							m_Car = car
						};
						Game.Vehicles.PersonalCar personalCar = m_PersonalCars[car];
						personalCar.m_Keeper = val;
						m_PersonalCars[car] = personalCar;
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct CitizenTryCollectMailJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildingData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingData;

		[ReadOnly]
		public ComponentLookup<MailAccumulationData> m_MailAccumulationData;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> m_ServiceObjectData;

		public ComponentLookup<MailSender> m_MailSenderData;

		public ComponentLookup<MailProducer> m_MailProducerData;

		public NativeQueue<Entity> m_MailSenderQueue;

		public void Execute()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			Entity val = default(Entity);
			CurrentBuilding currentBuilding = default(CurrentBuilding);
			MailProducer mailProducer = default(MailProducer);
			while (m_MailSenderQueue.TryDequeue(ref val))
			{
				if (!m_CurrentBuildingData.TryGetComponent(val, ref currentBuilding) || !m_MailProducerData.TryGetComponent(currentBuilding.m_CurrentBuilding, ref mailProducer) || mailProducer.m_SendingMail < 15 || RequireCollect(m_PrefabRefData[currentBuilding.m_CurrentBuilding].m_Prefab))
				{
					continue;
				}
				bool flag = m_MailSenderData.IsComponentEnabled(val);
				MailSender mailSender = (flag ? m_MailSenderData[val] : default(MailSender));
				int num = math.min((int)mailProducer.m_SendingMail, 100 - mailSender.m_Amount);
				if (num > 0)
				{
					mailSender.m_Amount = (ushort)(mailSender.m_Amount + num);
					mailProducer.m_SendingMail = (ushort)(mailProducer.m_SendingMail - num);
					m_MailProducerData[currentBuilding.m_CurrentBuilding] = mailProducer;
					if (!flag)
					{
						m_MailSenderData.SetComponentEnabled(val, true);
					}
					m_MailSenderData[val] = mailSender;
				}
			}
		}

		private bool RequireCollect(Entity prefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			if (m_SpawnableBuildingData.HasComponent(prefab))
			{
				SpawnableBuildingData spawnableBuildingData = m_SpawnableBuildingData[prefab];
				if (m_MailAccumulationData.HasComponent(spawnableBuildingData.m_ZonePrefab))
				{
					return m_MailAccumulationData[spawnableBuildingData.m_ZonePrefab].m_RequireCollect;
				}
			}
			else if (m_ServiceObjectData.HasComponent(prefab))
			{
				ServiceObjectData serviceObjectData = m_ServiceObjectData[prefab];
				if (m_MailAccumulationData.HasComponent(serviceObjectData.m_Service))
				{
					return m_MailAccumulationData[serviceObjectData.m_Service].m_RequireCollect;
				}
			}
			return false;
		}
	}

	[BurstCompile]
	private struct CitizeSleepJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildingData;

		public ComponentLookup<CitizenPresence> m_CitizenPresenceData;

		public NativeQueue<Entity> m_SleepQueue;

		public void Execute()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			Entity val = default(Entity);
			while (m_SleepQueue.TryDequeue(ref val))
			{
				if (m_CurrentBuildingData.HasComponent(val))
				{
					CurrentBuilding currentBuilding = m_CurrentBuildingData[val];
					if (m_CitizenPresenceData.HasComponent(currentBuilding.m_CurrentBuilding))
					{
						CitizenPresence citizenPresence = m_CitizenPresenceData[currentBuilding.m_CurrentBuilding];
						citizenPresence.m_Delta = (sbyte)math.max(-127, citizenPresence.m_Delta - 1);
						m_CitizenPresenceData[currentBuilding.m_CurrentBuilding] = citizenPresence;
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct CitizenAITickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> m_HouseholdMemberType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> m_HealthProblemType;

		public BufferTypeHandle<TripNeeded> m_TripType;

		[ReadOnly]
		public ComponentTypeHandle<Leisure> m_LeisureType;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<HouseholdNeed> m_HouseholdNeeds;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public ComponentLookup<CarKeeper> m_CarKeepers;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Vehicles.PersonalCar> m_PersonalCars;

		[ReadOnly]
		public ComponentLookup<MovingAway> m_MovingAway;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> m_Students;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> m_TouristHouseholds;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHouseholds;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> m_OutsideConnectionDatas;

		[ReadOnly]
		public ComponentLookup<InDanger> m_InDangerData;

		[ReadOnly]
		public ComponentLookup<AttendingMeeting> m_AttendingMeetings;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CoordinatedMeeting> m_Meetings;

		[ReadOnly]
		public BufferLookup<CoordinatedMeetingAttendee> m_Attendees;

		[ReadOnly]
		public BufferLookup<HaveCoordinatedMeetingData> m_MeetingDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public BufferLookup<Game.Buildings.Student> m_BuildingStudents;

		[ReadOnly]
		public ComponentLookup<Population> m_PopulationData;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public ComponentLookup<CommuterHousehold> m_CommuterHouseholds;

		[ReadOnly]
		public BufferLookup<Employee> m_EmployeeBufs;

		[ReadOnly]
		public EntityArchetype m_HouseholdArchetype;

		[ReadOnly]
		public NativeList<Entity> m_OutsideConnectionEntities;

		[ReadOnly]
		public EconomyParameterData m_EconomyParameters;

		[ReadOnly]
		public LeisureParametersData m_LeisureParameters;

		public uint m_UpdateFrameIndex;

		public float m_NormalizedTime;

		public uint m_SimulationFrame;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<Entity> m_CarReserverQueue;

		public ParallelWriter<Entity> m_MailSenderQueue;

		public ParallelWriter<Entity> m_SleepQueue;

		public TimeData m_TimeData;

		public Entity m_PopulationEntity;

		public RandomSeed m_RandomSeed;

		private bool CheckSleep(int index, Entity entity, ref Citizen citizen, Entity currentBuilding, Entity household, Entity home, DynamicBuffer<TripNeeded> trips, ref EconomyParameterData economyParameters, ref Random random)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			if (home != Entity.Null && IsSleepTime(entity, citizen, ref economyParameters, m_NormalizedTime, ref m_Workers, ref m_Students))
			{
				if (currentBuilding == home)
				{
					TravelPurpose travelPurpose = new TravelPurpose
					{
						m_Purpose = Purpose.Sleeping
					};
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TravelPurpose>(index, entity, travelPurpose);
					m_SleepQueue.Enqueue(entity);
					ReleaseCar(index, entity);
				}
				else
				{
					GoHome(entity, home, trips, currentBuilding);
				}
				return true;
			}
			return false;
		}

		private void GoHome(Entity entity, Entity target, DynamicBuffer<TripNeeded> trips, Entity currentBuilding)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if (!(target == Entity.Null) && !(currentBuilding == target))
			{
				if (!m_CarKeepers.IsComponentEnabled(entity))
				{
					m_CarReserverQueue.Enqueue(entity);
				}
				m_MailSenderQueue.Enqueue(entity);
				TripNeeded tripNeeded = new TripNeeded
				{
					m_TargetAgent = target,
					m_Purpose = Purpose.GoingHome
				};
				trips.Add(tripNeeded);
			}
		}

		private void GoToOutsideConnection(Entity entity, Entity household, Entity currentBuilding, Entity targetBuilding, ref Citizen citizen, DynamicBuffer<TripNeeded> trips, Purpose purpose, ref Random random)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			if (purpose == Purpose.MovingAway)
			{
				for (int i = 0; i < trips.Length; i++)
				{
					if (trips[i].m_Purpose == Purpose.MovingAway)
					{
						return;
					}
				}
			}
			if (!m_OutsideConnections.HasComponent(currentBuilding))
			{
				if (!m_CarKeepers.IsComponentEnabled(entity))
				{
					m_CarReserverQueue.Enqueue(entity);
				}
				m_MailSenderQueue.Enqueue(entity);
				if (targetBuilding == Entity.Null)
				{
					OutsideConnectionTransferType outsideConnectionTransferType = OutsideConnectionTransferType.Train | OutsideConnectionTransferType.Air | OutsideConnectionTransferType.Ship;
					if (m_OwnedVehicles.HasBuffer(household) && m_OwnedVehicles[household].Length > 0)
					{
						outsideConnectionTransferType |= OutsideConnectionTransferType.Road;
					}
					BuildingUtils.GetRandomOutsideConnectionByTransferType(ref m_OutsideConnectionEntities, ref m_OutsideConnectionDatas, ref m_Prefabs, random, outsideConnectionTransferType, out targetBuilding);
				}
				if (targetBuilding == Entity.Null && m_OutsideConnectionEntities.Length != 0)
				{
					int num = ((Random)(ref random)).NextInt(m_OutsideConnectionEntities.Length);
					targetBuilding = m_OutsideConnectionEntities[num];
				}
				trips.Add(new TripNeeded
				{
					m_TargetAgent = targetBuilding,
					m_Purpose = purpose
				});
			}
			else if (purpose == Purpose.MovingAway)
			{
				citizen.m_State |= CitizenFlags.MovingAwayReachOC;
			}
		}

		private void GoShopping(int chunkIndex, Entity citizen, Entity household, HouseholdNeed need, float3 position)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (!m_CarKeepers.IsComponentEnabled(citizen))
			{
				m_CarReserverQueue.Enqueue(citizen);
			}
			m_MailSenderQueue.Enqueue(citizen);
			ResourceBuyer resourceBuyer = new ResourceBuyer
			{
				m_Payer = household,
				m_Flags = SetupTargetFlags.Commercial,
				m_Location = position,
				m_ResourceNeeded = need.m_Resource,
				m_AmountNeeded = need.m_Amount
			};
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<ResourceBuyer>(chunkIndex, citizen, resourceBuyer);
		}

		private float GetTimeLeftUntilInterval(float2 interval)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (!(m_NormalizedTime < interval.x))
			{
				return 1f - m_NormalizedTime + interval.x;
			}
			return interval.x - m_NormalizedTime;
		}

		private bool DoLeisure(int chunkIndex, Entity citizenEntity, Entity householdEntity, Entity currentBuilding, Entity homeEntity, bool isTourist, ref Citizen citizenData, int population, ref Random random, ref EconomyParameterData economyParameters)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			bool flag = CitizenUtils.HasMovedIn(householdEntity, m_Households) && homeEntity == Entity.Null;
			if (isTourist)
			{
				if (m_OutsideConnections.HasComponent(currentBuilding) && m_TouristHouseholds[householdEntity].m_Hotel != Entity.Null)
				{
					return false;
				}
			}
			else if (!flag)
			{
				int num = 128 - citizenData.m_LeisureCounter;
				if (m_OutsideConnections.HasComponent(currentBuilding) || ((Random)(ref random)).NextInt(m_LeisureParameters.m_LeisureRandomFactor) > num)
				{
					return false;
				}
			}
			int num2 = math.min(kMinLeisurePossibility, Mathf.RoundToInt(200f / math.max(1f, math.sqrt(economyParameters.m_TrafficReduction * (float)population))));
			if (!isTourist && !flag && ((Random)(ref random)).NextInt(100) > num2)
			{
				citizenData.m_LeisureCounter = byte.MaxValue;
				return true;
			}
			float2 sleepTime = GetSleepTime(citizenEntity, citizenData, ref economyParameters, ref m_Workers, ref m_Students);
			float num3 = GetTimeLeftUntilInterval(sleepTime);
			if (m_Workers.HasComponent(citizenEntity))
			{
				Worker worker = m_Workers[citizenEntity];
				float2 timeToWork = WorkerSystem.GetTimeToWork(citizenData, worker, ref economyParameters, includeCommute: true);
				num3 = math.min(num3, GetTimeLeftUntilInterval(timeToWork));
			}
			else if (m_Students.HasComponent(citizenEntity))
			{
				Game.Citizens.Student student = m_Students[citizenEntity];
				float2 timeToStudy = StudentSystem.GetTimeToStudy(citizenData, student, ref economyParameters);
				num3 = math.min(num3, GetTimeLeftUntilInterval(timeToStudy));
			}
			if (isTourist)
			{
				citizenData.m_LeisureCounter = 0;
			}
			uint num4 = (uint)(num3 * 262144f);
			Leisure leisure = new Leisure
			{
				m_LastPossibleFrame = m_SimulationFrame + num4
			};
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Leisure>(chunkIndex, citizenEntity, leisure);
			return true;
		}

		private void ReleaseCar(int chunkIndex, Entity citizen)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			if (m_CarKeepers.IsComponentEnabled(citizen))
			{
				Entity car = m_CarKeepers[citizen].m_Car;
				if (m_PersonalCars.HasComponent(car))
				{
					Game.Vehicles.PersonalCar personalCar = m_PersonalCars[car];
					personalCar.m_Keeper = Entity.Null;
					m_PersonalCars[car] = personalCar;
				}
				((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<CarKeeper>(chunkIndex, citizen, false);
			}
		}

		private bool AttendMeeting(int chunkIndex, Entity entity, ref Citizen citizen, Entity household, Entity currentBuilding, DynamicBuffer<TripNeeded> trips, ref Random random)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			if (!m_CarKeepers.IsComponentEnabled(entity))
			{
				m_CarReserverQueue.Enqueue(entity);
			}
			Entity meeting = m_AttendingMeetings[entity].m_Meeting;
			if (m_Attendees.HasBuffer(meeting) && m_Meetings.HasComponent(meeting))
			{
				CoordinatedMeeting coordinatedMeeting = m_Meetings[meeting];
				if (m_Prefabs.HasComponent(meeting) && coordinatedMeeting.m_Status != MeetingStatus.Done)
				{
					HaveCoordinatedMeetingData haveCoordinatedMeetingData = m_MeetingDatas[m_Prefabs[meeting].m_Prefab][coordinatedMeeting.m_Phase];
					DynamicBuffer<CoordinatedMeetingAttendee> val = m_Attendees[meeting];
					if (coordinatedMeeting.m_Status == MeetingStatus.Waiting && coordinatedMeeting.m_Target == Entity.Null)
					{
						if (val.Length > 0 && val[0].m_Attendee == entity)
						{
							if (haveCoordinatedMeetingData.m_TravelPurpose.m_Purpose == Purpose.Shopping)
							{
								float3 position = m_Transforms[currentBuilding].m_Position;
								GoShopping(chunkIndex, entity, household, new HouseholdNeed
								{
									m_Resource = haveCoordinatedMeetingData.m_TravelPurpose.m_Resource,
									m_Amount = haveCoordinatedMeetingData.m_TravelPurpose.m_Data
								}, position);
								return true;
							}
							if (haveCoordinatedMeetingData.m_TravelPurpose.m_Purpose == Purpose.Traveling)
							{
								Citizen citizen2 = default(Citizen);
								GoToOutsideConnection(entity, household, currentBuilding, Entity.Null, ref citizen2, trips, haveCoordinatedMeetingData.m_TravelPurpose.m_Purpose, ref random);
							}
							else
							{
								if (haveCoordinatedMeetingData.m_TravelPurpose.m_Purpose != Purpose.GoingHome)
								{
									trips.Add(new TripNeeded
									{
										m_Purpose = haveCoordinatedMeetingData.m_TravelPurpose.m_Purpose,
										m_Resource = haveCoordinatedMeetingData.m_TravelPurpose.m_Resource,
										m_Data = haveCoordinatedMeetingData.m_TravelPurpose.m_Data,
										m_TargetAgent = default(Entity)
									});
									return true;
								}
								if (m_PropertyRenters.HasComponent(household))
								{
									coordinatedMeeting.m_Target = m_PropertyRenters[household].m_Property;
									m_Meetings[meeting] = coordinatedMeeting;
									GoHome(entity, m_PropertyRenters[household].m_Property, trips, currentBuilding);
								}
							}
						}
					}
					else if (coordinatedMeeting.m_Status == MeetingStatus.Waiting || coordinatedMeeting.m_Status == MeetingStatus.Traveling)
					{
						for (int i = 0; i < val.Length; i++)
						{
							if (val[i].m_Attendee == entity)
							{
								if (coordinatedMeeting.m_Target != Entity.Null && currentBuilding != coordinatedMeeting.m_Target && (!m_PropertyRenters.HasComponent(coordinatedMeeting.m_Target) || m_PropertyRenters[coordinatedMeeting.m_Target].m_Property != currentBuilding))
								{
									trips.Add(new TripNeeded
									{
										m_Purpose = haveCoordinatedMeetingData.m_TravelPurpose.m_Purpose,
										m_Resource = haveCoordinatedMeetingData.m_TravelPurpose.m_Resource,
										m_Data = haveCoordinatedMeetingData.m_TravelPurpose.m_Data,
										m_TargetAgent = coordinatedMeeting.m_Target
									});
								}
								return true;
							}
						}
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<AttendingMeeting>(chunkIndex, entity);
						return false;
					}
				}
				return coordinatedMeeting.m_Status != MeetingStatus.Done;
			}
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<AttendingMeeting>(chunkIndex, entity);
			return false;
		}

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_0647: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_067f: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Citizen> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Citizen>(ref m_CitizenType);
			NativeArray<HouseholdMember> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdMember>(ref m_HouseholdMemberType);
			NativeArray<CurrentBuilding> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingType);
			NativeArray<HealthProblem> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HealthProblem>(ref m_HealthProblemType);
			BufferAccessor<TripNeeded> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TripNeeded>(ref m_TripType);
			bool flag = nativeArray5.Length > 0;
			int population = m_PopulationData[m_PopulationEntity].m_Population;
			MovingAway movingAway = default(MovingAway);
			CommuterHousehold commuterHousehold = default(CommuterHousehold);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Citizen citizen = nativeArray2[i];
				if (flag && CitizenUtils.IsDead(nativeArray5[i]))
				{
					continue;
				}
				Entity household = nativeArray3[i].m_Household;
				Entity val = nativeArray[i];
				bool flag2 = m_TouristHouseholds.HasComponent(household);
				bool flag3 = m_HomelessHouseholds.HasComponent(household);
				DynamicBuffer<TripNeeded> trips = bufferAccessor[i];
				if (household == Entity.Null)
				{
					household = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex, m_HouseholdArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HouseholdMember>(unfilteredChunkIndex, val, new HouseholdMember
					{
						m_Household = household
					});
					((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<HouseholdCitizen>(unfilteredChunkIndex, household).Add(new HouseholdCitizen
					{
						m_Citizen = val
					});
					Debug.LogWarning((object)$"Citizen:{val.Index} don't have valid household");
					continue;
				}
				if (!m_Households.HasComponent(household))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val, default(Deleted));
					continue;
				}
				Entity currentBuilding = nativeArray4[i].m_CurrentBuilding;
				if (currentBuilding == Entity.Null && m_MovingAway.HasComponent(household))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, household, default(Deleted));
				}
				else
				{
					if (!m_Transforms.HasComponent(currentBuilding) || (m_InDangerData.HasComponent(currentBuilding) && (m_InDangerData[currentBuilding].m_Flags & DangerFlags.StayIndoors) != 0))
					{
						continue;
					}
					bool flag4 = (citizen.m_State & CitizenFlags.Commuter) != 0;
					CitizenAge age = citizen.GetAge();
					if (flag4 && (age == CitizenAge.Elderly || age == CitizenAge.Child))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val, default(Deleted));
					}
					if ((citizen.m_State & CitizenFlags.MovingAwayReachOC) != CitizenFlags.None)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val, default(Deleted));
						continue;
					}
					if (m_MovingAway.TryGetComponent(household, ref movingAway))
					{
						GoToOutsideConnection(val, household, currentBuilding, movingAway.m_Target, ref citizen, trips, Purpose.MovingAway, ref random);
						if (((ArchetypeChunk)(ref chunk)).Has<Leisure>(ref m_LeisureType))
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Leisure>(unfilteredChunkIndex, val);
						}
						if (m_Workers.HasComponent(val))
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Worker>(unfilteredChunkIndex, val);
						}
						if (m_Students.HasComponent(val))
						{
							if (m_BuildingStudents.HasBuffer(m_Students[val].m_School))
							{
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<StudentsRemoved>(unfilteredChunkIndex, m_Students[val].m_School);
							}
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Citizens.Student>(unfilteredChunkIndex, val);
						}
						nativeArray2[i] = citizen;
						continue;
					}
					Entity val2 = Entity.Null;
					if (m_PropertyRenters.HasComponent(household))
					{
						val2 = m_PropertyRenters[household].m_Property;
					}
					else if (flag3)
					{
						val2 = m_HomelessHouseholds[household].m_TempHome;
					}
					else if (flag2)
					{
						Entity hotel = m_TouristHouseholds[household].m_Hotel;
						if (m_PropertyRenters.HasComponent(hotel))
						{
							val2 = m_PropertyRenters[hotel].m_Property;
						}
					}
					else if (flag4)
					{
						if (m_OutsideConnections.HasComponent(currentBuilding))
						{
							val2 = currentBuilding;
						}
						else
						{
							if (m_CommuterHouseholds.TryGetComponent(household, ref commuterHousehold))
							{
								val2 = commuterHousehold.m_OriginalFrom;
							}
							if (val2 == Entity.Null)
							{
								val2 = m_OutsideConnectionEntities[((Random)(ref random)).NextInt(m_OutsideConnectionEntities.Length)];
							}
						}
					}
					if (flag)
					{
						if (((ArchetypeChunk)(ref chunk)).Has<Leisure>(ref m_LeisureType))
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Leisure>(unfilteredChunkIndex, val);
						}
					}
					else
					{
						if (m_AttendingMeetings.HasComponent(val) && AttendMeeting(unfilteredChunkIndex, val, ref citizen, household, currentBuilding, trips, ref random))
						{
							continue;
						}
						if ((m_Workers.HasComponent(val) && !WorkerSystem.IsTodayOffDay(citizen, ref m_EconomyParameters, m_SimulationFrame, m_TimeData, population) && WorkerSystem.IsTimeToWork(citizen, m_Workers[val], ref m_EconomyParameters, m_NormalizedTime)) || (m_Students.HasComponent(val) && StudentSystem.IsTimeToStudy(citizen, m_Students[val], ref m_EconomyParameters, m_NormalizedTime, m_SimulationFrame, m_TimeData, population)))
						{
							if (((ArchetypeChunk)(ref chunk)).Has<Leisure>(ref m_LeisureType))
							{
								((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Leisure>(unfilteredChunkIndex, val);
							}
							continue;
						}
						if (CheckSleep(i, val, ref citizen, currentBuilding, household, val2, trips, ref m_EconomyParameters, ref random))
						{
							if (((ArchetypeChunk)(ref chunk)).Has<Leisure>(ref m_LeisureType))
							{
								((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Leisure>(unfilteredChunkIndex, val);
							}
							continue;
						}
						if (age == CitizenAge.Adult || age == CitizenAge.Elderly)
						{
							HouseholdNeed householdNeed = m_HouseholdNeeds[household];
							if (householdNeed.m_Resource != Resource.NoResource && m_Transforms.HasComponent(currentBuilding))
							{
								GoShopping(unfilteredChunkIndex, val, household, householdNeed, m_Transforms[currentBuilding].m_Position);
								householdNeed.m_Resource = Resource.NoResource;
								m_HouseholdNeeds[household] = householdNeed;
								if (((ArchetypeChunk)(ref chunk)).Has<Leisure>(ref m_LeisureType))
								{
									((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Leisure>(unfilteredChunkIndex, val);
								}
								continue;
							}
						}
						if (!((ArchetypeChunk)(ref chunk)).Has<Leisure>(ref m_LeisureType) && DoLeisure(unfilteredChunkIndex, val, household, currentBuilding, val2, flag2, ref citizen, population, ref random, ref m_EconomyParameters))
						{
							nativeArray2[i] = citizen;
						}
						else if (!((ArchetypeChunk)(ref chunk)).Has<Leisure>(ref m_LeisureType))
						{
							if (currentBuilding != val2)
							{
								GoHome(val, val2, trips, currentBuilding);
							}
							else
							{
								ReleaseCar(unfilteredChunkIndex, val);
							}
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
		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentTypeHandle;

		public BufferTypeHandle<TripNeeded> __Game_Citizens_TripNeeded_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Leisure> __Game_Citizens_Leisure_RO_ComponentTypeHandle;

		public ComponentLookup<HouseholdNeed> __Game_Citizens_HouseholdNeed_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarKeeper> __Game_Citizens_CarKeeper_RO_ComponentLookup;

		public ComponentLookup<Game.Vehicles.PersonalCar> __Game_Vehicles_PersonalCar_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MovingAway> __Game_Agents_MovingAway_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> __Game_Citizens_TouristHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InDanger> __Game_Events_InDanger_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CoordinatedMeetingAttendee> __Game_Citizens_CoordinatedMeetingAttendee_RO_BufferLookup;

		public ComponentLookup<CoordinatedMeeting> __Game_Citizens_CoordinatedMeeting_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AttendingMeeting> __Game_Citizens_AttendingMeeting_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HaveCoordinatedMeetingData> __Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Buildings.Student> __Game_Buildings_Student_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Population> __Game_City_Population_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> __Game_Prefabs_OutsideConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<CommuterHousehold> __Game_Citizens_CommuterHousehold_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		public ComponentLookup<CarKeeper> __Game_Citizens_CarKeeper_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MailAccumulationData> __Game_Prefabs_MailAccumulationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> __Game_Prefabs_ServiceObjectData_RO_ComponentLookup;

		public ComponentLookup<MailSender> __Game_Citizens_MailSender_RW_ComponentLookup;

		public ComponentLookup<MailProducer> __Game_Buildings_MailProducer_RW_ComponentLookup;

		public ComponentLookup<CitizenPresence> __Game_Buildings_CitizenPresence_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			__Game_Citizens_Citizen_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(false);
			__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdMember>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Citizens_HealthProblem_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HealthProblem>(true);
			__Game_Citizens_TripNeeded_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TripNeeded>(false);
			__Game_Citizens_Leisure_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Leisure>(true);
			__Game_Citizens_HouseholdNeed_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdNeed>(false);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Citizens_CarKeeper_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarKeeper>(true);
			__Game_Vehicles_PersonalCar_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PersonalCar>(false);
			__Game_Agents_MovingAway_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MovingAway>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Citizens_Student_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Citizens.Student>(true);
			__Game_Citizens_TouristHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TouristHousehold>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Events_InDanger_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InDanger>(true);
			__Game_Citizens_CoordinatedMeetingAttendee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CoordinatedMeetingAttendee>(true);
			__Game_Citizens_CoordinatedMeeting_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CoordinatedMeeting>(false);
			__Game_Citizens_AttendingMeeting_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AttendingMeeting>(true);
			__Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HaveCoordinatedMeetingData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Buildings_Student_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Buildings.Student>(true);
			__Game_City_Population_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(true);
			__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnectionData>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Citizens_CommuterHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CommuterHousehold>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Citizens_CarKeeper_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarKeeper>(false);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_MailAccumulationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailAccumulationData>(true);
			__Game_Prefabs_ServiceObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceObjectData>(true);
			__Game_Citizens_MailSender_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailSender>(false);
			__Game_Buildings_MailProducer_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailProducer>(false);
			__Game_Buildings_CitizenPresence_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CitizenPresence>(false);
		}
	}

	public static readonly float kMaxPathfindCost = 17000f;

	public static readonly float kMaxMovingAwayCost = kMaxPathfindCost * 10f;

	public static readonly int kMinLeisurePossibility = 80;

	private JobHandle m_CarReserveWriters;

	private EntityQuery m_CitizenQuery;

	private EntityQuery m_OutsideConnectionQuery;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_LeisureParameterQuery;

	private EntityQuery m_TimeDataQuery;

	private EntityQuery m_PopulationQuery;

	private SimulationSystem m_SimulationSystem;

	private TimeSystem m_TimeSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityArchetype m_HouseholdArchetype;

	private NativeQueue<Entity> m_CarReserveQueue;

	private ParallelWriter<Entity> m_ParallelCarReserveQueue;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 11;
	}

	public static float2 GetSleepTime(Entity entity, Citizen citizen, ref EconomyParameterData economyParameters, ref ComponentLookup<Worker> workers, ref ComponentLookup<Game.Citizens.Student> students)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		CitizenAge age = citizen.GetAge();
		float2 val = default(float2);
		((float2)(ref val))._002Ector(0.875f, 0.175f);
		float num = val.y - val.x;
		Random pseudoRandom = citizen.GetPseudoRandom(CitizenPseudoRandom.SleepOffset);
		val += ((Random)(ref pseudoRandom)).NextFloat(0f, 0.2f);
		if (age == CitizenAge.Elderly)
		{
			val -= 0.05f;
		}
		if (age == CitizenAge.Child)
		{
			val -= 0.1f;
		}
		if (age == CitizenAge.Teen)
		{
			val += 0.05f;
		}
		val = math.frac(val);
		float2 val2;
		if (workers.HasComponent(entity))
		{
			val2 = WorkerSystem.GetTimeToWork(citizen, workers[entity], ref economyParameters, includeCommute: true);
		}
		else
		{
			if (!students.HasComponent(entity))
			{
				return val;
			}
			val2 = StudentSystem.GetTimeToStudy(citizen, students[entity], ref economyParameters);
		}
		if (val2.x < val2.y)
		{
			if (val.x > val.y && val2.y > val.x)
			{
				val += val2.y - val.x;
			}
			else if (val.y > val2.x)
			{
				val += 1f - (val.y - val2.x);
			}
		}
		else
		{
			((float2)(ref val))._002Ector(val2.y, val2.y + num);
		}
		val = math.frac(val);
		return val;
	}

	public static bool IsSleepTime(Entity entity, Citizen citizen, ref EconomyParameterData economyParameters, float normalizedTime, ref ComponentLookup<Worker> workers, ref ComponentLookup<Game.Citizens.Student> students)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		float2 sleepTime = GetSleepTime(entity, citizen, ref economyParameters, ref workers, ref students);
		if (sleepTime.y < sleepTime.x)
		{
			if (!(normalizedTime > sleepTime.x))
			{
				return normalizedTime < sleepTime.y;
			}
			return true;
		}
		if (normalizedTime > sleepTime.x)
		{
			return normalizedTime < sleepTime.y;
		}
		return false;
	}

	public ParallelWriter<Entity> GetCarReserveQueue(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_CarReserveWriters;
		return m_ParallelCarReserveQueue;
	}

	public void AddCarReserveWriter(JobHandle writer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_CarReserveWriters = JobHandle.CombineDependencies(m_CarReserveWriters, writer);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CarReserveQueue = new NativeQueue<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		m_ParallelCarReserveQueue = m_CarReserveQueue.AsParallelWriter();
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_LeisureParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<LeisureParametersData>() });
		m_PopulationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Population>() });
		m_CitizenQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadWrite<Citizen>(),
			ComponentType.Exclude<TravelPurpose>(),
			ComponentType.Exclude<ResourceBuyer>(),
			ComponentType.ReadOnly<CurrentBuilding>(),
			ComponentType.ReadOnly<HouseholdMember>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_OutsideConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Game.Objects.ElectricityOutsideConnection>(),
			ComponentType.Exclude<Game.Objects.WaterPipeOutsideConnection>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
		m_HouseholdArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadWrite<Household>(),
			ComponentType.ReadWrite<HouseholdNeed>(),
			ComponentType.ReadWrite<HouseholdCitizen>(),
			ComponentType.ReadWrite<TaxPayer>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.ReadWrite<UpdateFrame>(),
			ComponentType.ReadWrite<Created>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CitizenQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_LeisureParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_TimeDataQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_PopulationQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_CarReserveQueue.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrameWithInterval = SimulationUtils.GetUpdateFrameWithInterval(m_SimulationSystem.frameIndex, (uint)GetUpdateInterval(SystemUpdatePhase.GameSimulation), 16);
		NativeQueue<Entity> mailSenderQueue = default(NativeQueue<Entity>);
		mailSenderQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<Entity> sleepQueue = default(NativeQueue<Entity>);
		sleepQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle val = default(JobHandle);
		CitizenAITickJob citizenAITickJob = new CitizenAITickJob
		{
			m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMemberType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemType = InternalCompilerInterface.GetComponentTypeHandle<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TripType = InternalCompilerInterface.GetBufferTypeHandle<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LeisureType = InternalCompilerInterface.GetComponentTypeHandle<Leisure>(ref __TypeHandle.__Game_Citizens_Leisure_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdNeeds = InternalCompilerInterface.GetComponentLookup<HouseholdNeed>(ref __TypeHandle.__Game_Citizens_HouseholdNeed_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarKeepers = InternalCompilerInterface.GetComponentLookup<CarKeeper>(ref __TypeHandle.__Game_Citizens_CarKeeper_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PersonalCars = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingAway = InternalCompilerInterface.GetComponentLookup<MovingAway>(ref __TypeHandle.__Game_Agents_MovingAway_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Students = InternalCompilerInterface.GetComponentLookup<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TouristHouseholds = InternalCompilerInterface.GetComponentLookup<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHouseholds = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InDangerData = InternalCompilerInterface.GetComponentLookup<InDanger>(ref __TypeHandle.__Game_Events_InDanger_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Attendees = InternalCompilerInterface.GetBufferLookup<CoordinatedMeetingAttendee>(ref __TypeHandle.__Game_Citizens_CoordinatedMeetingAttendee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Meetings = InternalCompilerInterface.GetComponentLookup<CoordinatedMeeting>(ref __TypeHandle.__Game_Citizens_CoordinatedMeeting_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttendingMeetings = InternalCompilerInterface.GetComponentLookup<AttendingMeeting>(ref __TypeHandle.__Game_Citizens_AttendingMeeting_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeetingDatas = InternalCompilerInterface.GetBufferLookup<HaveCoordinatedMeetingData>(ref __TypeHandle.__Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingStudents = InternalCompilerInterface.GetBufferLookup<Game.Buildings.Student>(ref __TypeHandle.__Game_Buildings_Student_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PopulationData = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionDatas = InternalCompilerInterface.GetComponentLookup<OutsideConnectionData>(ref __TypeHandle.__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommuterHouseholds = InternalCompilerInterface.GetComponentLookup<CommuterHousehold>(ref __TypeHandle.__Game_Citizens_CommuterHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EmployeeBufs = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdArchetype = m_HouseholdArchetype,
			m_OutsideConnectionEntities = ((EntityQuery)(ref m_OutsideConnectionQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val),
			m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_LeisureParameters = ((EntityQuery)(ref m_LeisureParameterQuery)).GetSingleton<LeisureParametersData>()
		};
		EntityCommandBuffer val2 = m_EndFrameBarrier.CreateCommandBuffer();
		citizenAITickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		citizenAITickJob.m_UpdateFrameIndex = updateFrameWithInterval;
		citizenAITickJob.m_SimulationFrame = m_SimulationSystem.frameIndex;
		citizenAITickJob.m_NormalizedTime = m_TimeSystem.normalizedTime;
		citizenAITickJob.m_TimeData = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>();
		citizenAITickJob.m_PopulationEntity = ((EntityQuery)(ref m_PopulationQuery)).GetSingletonEntity();
		citizenAITickJob.m_CarReserverQueue = m_ParallelCarReserveQueue;
		citizenAITickJob.m_MailSenderQueue = mailSenderQueue.AsParallelWriter();
		citizenAITickJob.m_SleepQueue = sleepQueue.AsParallelWriter();
		citizenAITickJob.m_RandomSeed = RandomSeed.Next();
		CitizenAITickJob citizenAITickJob2 = citizenAITickJob;
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<CitizenAITickJob>(citizenAITickJob2, m_CitizenQuery, JobHandle.CombineDependencies(m_CarReserveWriters, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val)));
		citizenAITickJob2.m_OutsideConnectionEntities.Dispose(val3);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		AddCarReserveWriter(val3);
		JobHandle val4 = IJobExtensions.Schedule<CitizenReserveHouseholdCarJob>(new CitizenReserveHouseholdCarJob
		{
			m_CarKeepers = InternalCompilerInterface.GetComponentLookup<CarKeeper>(ref __TypeHandle.__Game_Citizens_CarKeeper_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMembers = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PersonalCars = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ReserverQueue = m_CarReserveQueue
		}, JobHandle.CombineDependencies(val3, m_CarReserveWriters));
		m_EndFrameBarrier.AddJobHandleForProducer(val4);
		AddCarReserveWriter(val4);
		JobHandle val5 = IJobExtensions.Schedule<CitizenTryCollectMailJob>(new CitizenTryCollectMailJob
		{
			m_CurrentBuildingData = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingData = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailAccumulationData = InternalCompilerInterface.GetComponentLookup<MailAccumulationData>(ref __TypeHandle.__Game_Prefabs_MailAccumulationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceObjectData = InternalCompilerInterface.GetComponentLookup<ServiceObjectData>(ref __TypeHandle.__Game_Prefabs_ServiceObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailSenderData = InternalCompilerInterface.GetComponentLookup<MailSender>(ref __TypeHandle.__Game_Citizens_MailSender_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailProducerData = InternalCompilerInterface.GetComponentLookup<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailSenderQueue = mailSenderQueue
		}, val3);
		m_EndFrameBarrier.AddJobHandleForProducer(val5);
		mailSenderQueue.Dispose(val5);
		JobHandle val6 = IJobExtensions.Schedule<CitizeSleepJob>(new CitizeSleepJob
		{
			m_CurrentBuildingData = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenPresenceData = InternalCompilerInterface.GetComponentLookup<CitizenPresence>(ref __TypeHandle.__Game_Buildings_CitizenPresence_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SleepQueue = sleepQueue
		}, val3);
		sleepQueue.Dispose(val6);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val4, val5, val6);
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
	public CitizenBehaviorSystem()
	{
	}
}

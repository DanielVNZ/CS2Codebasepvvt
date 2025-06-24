using Colossal.Entities;
using Game.Agents;
using Game.Pathfind;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Citizens;

public static class CitizenUtils
{
	public static bool IsDead(Entity citizen, ref ComponentLookup<HealthProblem> healthProblems)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		HealthProblem healthProblem = default(HealthProblem);
		if (healthProblems.TryGetComponent(citizen, ref healthProblem))
		{
			return IsDead(healthProblem);
		}
		return false;
	}

	public static bool IsDead(EntityManager entityManager, Entity citizen)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		HealthProblem healthProblem = default(HealthProblem);
		if (EntitiesExtensions.TryGetComponent<HealthProblem>(entityManager, citizen, ref healthProblem))
		{
			return IsDead(healthProblem);
		}
		return false;
	}

	public static bool IsDead(HealthProblem healthProblem)
	{
		return (healthProblem.m_Flags & HealthProblemFlags.Dead) != 0;
	}

	public static bool IsCorpsePickedByHearse(Entity citizen, ref ComponentLookup<HealthProblem> healthProblems, ref ComponentLookup<TravelPurpose> travelPurposes)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		TravelPurpose travelPurpose = default(TravelPurpose);
		if (IsDead(citizen, ref healthProblems) && travelPurposes.TryGetComponent(citizen, ref travelPurpose) && (travelPurpose.m_Purpose == Purpose.Deathcare || travelPurpose.m_Purpose == Purpose.InDeathcare))
		{
			return true;
		}
		return false;
	}

	public static bool IsCorpsePickedByHearse(EntityManager entityManager, Entity citizen)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		TravelPurpose travelPurpose = default(TravelPurpose);
		if (IsDead(entityManager, citizen) && EntitiesExtensions.TryGetComponent<TravelPurpose>(entityManager, citizen, ref travelPurpose) && (travelPurpose.m_Purpose == Purpose.Deathcare || travelPurpose.m_Purpose == Purpose.InDeathcare))
		{
			return true;
		}
		return false;
	}

	public static bool TryGetResident(Entity entity, ComponentLookup<Citizen> citizenFromEntity, out Citizen citizen)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (citizenFromEntity.TryGetComponent(entity, ref citizen))
		{
			return (citizen.m_State & (CitizenFlags.MovingAwayReachOC | CitizenFlags.Tourist | CitizenFlags.Commuter)) == 0;
		}
		return false;
	}

	public static PathfindWeights GetPathfindWeights(Citizen citizen, Household household, int householdCitizens)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		float time = 5f * (4f - 3.75f * (float)(int)citizen.m_LeisureCounter / 255f);
		float behaviour = 2f;
		float num = 2500f * math.max(1f, (float)householdCitizens) / (float)math.max(250, (int)household.m_ConsumptionPerDay);
		Random pseudoRandom = citizen.GetPseudoRandom(CitizenPseudoRandom.TrafficComfort);
		float comfort = 1f + 2f * ((Random)(ref pseudoRandom)).NextFloat();
		num = math.select(num, num * 0.1f, (household.m_Flags & HouseholdFlags.MovedIn) == 0 && (citizen.m_State & (CitizenFlags.MovingAwayReachOC | CitizenFlags.Tourist | CitizenFlags.Commuter)) == 0);
		return new PathfindWeights(time, behaviour, num, comfort);
	}

	public static bool IsCommuter(Entity citizenEntity, ref ComponentLookup<Citizen> citizens)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return (citizens[citizenEntity].m_State & CitizenFlags.Commuter) != 0;
	}

	public static bool IsResident(EntityManager entityManager, Entity entity, out Citizen citizen)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!EntitiesExtensions.TryGetComponent<Citizen>(entityManager, entity, ref citizen))
		{
			return false;
		}
		HouseholdMember householdMember = default(HouseholdMember);
		if (!EntitiesExtensions.TryGetComponent<HouseholdMember>(entityManager, entity, ref householdMember))
		{
			return false;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<TouristHousehold>(householdMember.m_Household))
		{
			return false;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<CommuterHousehold>(householdMember.m_Household))
		{
			return false;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<MovingAway>(householdMember.m_Household))
		{
			return false;
		}
		return (citizen.m_State & (CitizenFlags.MovingAwayReachOC | CitizenFlags.Tourist | CitizenFlags.Commuter)) == 0;
	}

	public static bool IsResident(Entity entity, Citizen citizen, ComponentLookup<HouseholdMember> householdMemberFromEntity, ComponentLookup<MovingAway> movingAwayFromEntity, ComponentLookup<TouristHousehold> touristHouseholdFromEntity, ComponentLookup<CommuterHousehold> commuterHouseholdFromEntity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		HouseholdMember householdMember = default(HouseholdMember);
		if (!householdMemberFromEntity.TryGetComponent(entity, ref householdMember))
		{
			return false;
		}
		if (touristHouseholdFromEntity.HasComponent(householdMember.m_Household))
		{
			return false;
		}
		if (commuterHouseholdFromEntity.HasComponent(householdMember.m_Household))
		{
			return false;
		}
		if (movingAwayFromEntity.HasComponent(householdMember.m_Household))
		{
			return false;
		}
		return (citizen.m_State & (CitizenFlags.MovingAwayReachOC | CitizenFlags.Tourist | CitizenFlags.Commuter)) == 0;
	}

	public static bool HasMovedIn(Entity householdEntity, ComponentLookup<Household> householdDatas)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Household household = default(Household);
		if (householdDatas.TryGetComponent(householdEntity, ref household))
		{
			return (household.m_Flags & HouseholdFlags.MovedIn) != 0;
		}
		return false;
	}

	public static bool HasMovedIn(Entity citizen, ref ComponentLookup<HouseholdMember> householdMembers, ref ComponentLookup<Household> households, ref ComponentLookup<HomelessHousehold> homelessHouseholds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		HouseholdMember householdMember = default(HouseholdMember);
		Household household = default(Household);
		if (householdMembers.TryGetComponent(citizen, ref householdMember) && households.TryGetComponent(householdMember.m_Household, ref household) && (household.m_Flags & HouseholdFlags.MovedIn) != HouseholdFlags.None)
		{
			return !homelessHouseholds.HasComponent(householdMember.m_Household);
		}
		return false;
	}

	public static CitizenHappiness GetHappinessKey(int happiness)
	{
		if (happiness > 70)
		{
			return CitizenHappiness.Happy;
		}
		if (happiness > 55)
		{
			return CitizenHappiness.Content;
		}
		if (happiness > 40)
		{
			return CitizenHappiness.Neutral;
		}
		if (happiness > 25)
		{
			return CitizenHappiness.Sad;
		}
		return CitizenHappiness.Depressed;
	}

	public static Entity GetCitizenSelectedSound(EntityManager entityManager, Entity entity, Citizen citizen, Entity citizenPrefabRef)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityManager)(ref entityManager)).HasComponent<CitizenSelectedSoundData>(citizenPrefabRef))
		{
			return Entity.Null;
		}
		CitizenHappiness happinessKey = GetHappinessKey(citizen.Happiness);
		bool isSickOrInjured = false;
		HealthProblem healthProblem = default(HealthProblem);
		if (EntitiesExtensions.TryGetComponent<HealthProblem>(entityManager, entity, ref healthProblem) && (healthProblem.m_Flags & (HealthProblemFlags.Sick | HealthProblemFlags.Injured)) != HealthProblemFlags.None)
		{
			isSickOrInjured = true;
		}
		DynamicBuffer<CitizenSelectedSoundData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<CitizenSelectedSoundData>(citizenPrefabRef, true);
		for (int i = 0; i < buffer.Length; i++)
		{
			if (buffer[i].Equals(new CitizenSelectedSoundData(isSickOrInjured, citizen.GetAge(), happinessKey, Entity.Null)))
			{
				return buffer[i].m_SelectedSound;
			}
		}
		return Entity.Null;
	}

	public static bool IsHouseholdNeedSupport(DynamicBuffer<HouseholdCitizen> householdCitizens, ref ComponentLookup<Citizen> citizens, ref ComponentLookup<Student> students)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		bool result = true;
		for (int i = 0; i < householdCitizens.Length; i++)
		{
			Entity citizen = householdCitizens[i].m_Citizen;
			if (citizens[citizen].GetAge() == CitizenAge.Adult && !students.HasComponent(citizen))
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public static bool IsWorkableCitizen(Entity citizenEntity, ref ComponentLookup<Citizen> citizens, ref ComponentLookup<Student> m_Students, ref ComponentLookup<HealthProblem> healthProblems)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if ((!healthProblems.HasComponent(citizenEntity) || !IsDead(healthProblems[citizenEntity])) && !m_Students.HasComponent(citizenEntity) && (citizens[citizenEntity].m_State & (CitizenFlags.Tourist | CitizenFlags.Commuter)) == 0 && (citizens[citizenEntity].GetAge() == CitizenAge.Teen || citizens[citizenEntity].GetAge() == CitizenAge.Adult))
		{
			return true;
		}
		return false;
	}

	public static Entity GetCitizenPrefabFromCitizen(NativeList<Entity> citizenPrefabs, Citizen citizen, ComponentLookup<CitizenData> citizenDatas, Random rnd)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = 0; i < citizenPrefabs.Length; i++)
		{
			CitizenData citizenData = citizenDatas[citizenPrefabs[i]];
			if (((citizen.m_State & CitizenFlags.Male) == 0) ^ citizenData.m_Male)
			{
				num++;
			}
		}
		if (num > 0)
		{
			int num2 = ((Random)(ref rnd)).NextInt(num);
			for (int j = 0; j < citizenPrefabs.Length; j++)
			{
				CitizenData citizenData2 = citizenDatas[citizenPrefabs[j]];
				if (((citizen.m_State & CitizenFlags.Male) == 0) ^ citizenData2.m_Male)
				{
					num2--;
					if (num2 < 0)
					{
						PrefabRef prefabRef = new PrefabRef
						{
							m_Prefab = citizenPrefabs[j]
						};
						return prefabRef.m_Prefab;
					}
				}
			}
		}
		return Entity.Null;
	}

	public static void HouseholdMoveAway(ParallelWriter commandBuffer, int sortKey, Entity householdEntity)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((ParallelWriter)(ref commandBuffer)).AddComponent<MovingAway>(sortKey, householdEntity, default(MovingAway));
	}

	public static void HouseholdMoveAway(EntityCommandBuffer commandBuffer, Entity householdEntity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityCommandBuffer)(ref commandBuffer)).AddComponent<MovingAway>(householdEntity, default(MovingAway));
	}
}

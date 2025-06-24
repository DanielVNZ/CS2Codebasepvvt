using Colossal.Entities;
using Game.Buildings;
using Game.Citizens;
using Game.Companies;
using Game.Creatures;
using Game.Economy;
using Game.Events;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.UI.InGame;

public static class CitizenUIUtils
{
	public static HouseholdWealthKey GetHouseholdWealth(EntityManager entityManager, Entity householdEntity, CitizenHappinessParameterData happinessParameters)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).Exists(householdEntity))
		{
			int num = 0;
			DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
			Household householdData = default(Household);
			if (EntitiesExtensions.TryGetBuffer<Resources>(entityManager, householdEntity, true, ref resources) && EntitiesExtensions.TryGetComponent<Household>(entityManager, householdEntity, ref householdData))
			{
				num += EconomyUtils.GetHouseholdTotalWealth(householdData, resources);
			}
			if (num < happinessParameters.m_WealthyMoneyAmount.x)
			{
				return HouseholdWealthKey.Wretched;
			}
			if (num < happinessParameters.m_WealthyMoneyAmount.y)
			{
				return HouseholdWealthKey.Poor;
			}
			if (num < happinessParameters.m_WealthyMoneyAmount.z)
			{
				return HouseholdWealthKey.Modest;
			}
			if (num < happinessParameters.m_WealthyMoneyAmount.w)
			{
				return HouseholdWealthKey.Comfortable;
			}
			return HouseholdWealthKey.Wealthy;
		}
		return HouseholdWealthKey.Modest;
	}

	public static HouseholdWealthKey GetAverageHouseholdWealth(EntityManager entityManager, NativeList<Entity> households, CitizenHappinessParameterData happinessParameters)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
		Household householdData = default(Household);
		for (int i = 0; i < households.Length; i++)
		{
			Entity val = households[i];
			if (EntitiesExtensions.TryGetBuffer<Resources>(entityManager, val, true, ref resources) && EntitiesExtensions.TryGetComponent<Household>(entityManager, val, ref householdData))
			{
				num += EconomyUtils.GetHouseholdTotalWealth(householdData, resources);
			}
		}
		num /= math.select(households.Length, 1, households.Length == 0);
		if (num < happinessParameters.m_WealthyMoneyAmount.x)
		{
			return HouseholdWealthKey.Wretched;
		}
		if (num < happinessParameters.m_WealthyMoneyAmount.y)
		{
			return HouseholdWealthKey.Poor;
		}
		if (num < happinessParameters.m_WealthyMoneyAmount.z)
		{
			return HouseholdWealthKey.Modest;
		}
		if (num < happinessParameters.m_WealthyMoneyAmount.w)
		{
			return HouseholdWealthKey.Comfortable;
		}
		return HouseholdWealthKey.Wealthy;
	}

	public static CitizenJobLevelKey GetJobLevel(EntityManager entityManager, Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Worker worker = default(Worker);
		if (EntitiesExtensions.TryGetComponent<Worker>(entityManager, entity, ref worker))
		{
			return (CitizenJobLevelKey)worker.m_Level;
		}
		return CitizenJobLevelKey.Unknown;
	}

	public static CitizenAgeKey GetAge(EntityManager entityManager, Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Citizen citizen = default(Citizen);
		if (EntitiesExtensions.TryGetComponent<Citizen>(entityManager, entity, ref citizen))
		{
			return (CitizenAgeKey)citizen.GetAge();
		}
		return CitizenAgeKey.Adult;
	}

	public static CitizenOccupationKey GetOccupation(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		Entity household = ((EntityManager)(ref entityManager)).GetComponentData<HouseholdMember>(entity).m_Household;
		if (((EntityManager)(ref entityManager)).Exists(household) && ((EntityManager)(ref entityManager)).HasComponent<TouristHousehold>(household))
		{
			return CitizenOccupationKey.Tourist;
		}
		Criminal criminal = default(Criminal);
		if (EntitiesExtensions.TryGetComponent<Criminal>(entityManager, entity, ref criminal))
		{
			if ((criminal.m_Flags & CriminalFlags.Robber) == 0)
			{
				return CitizenOccupationKey.Criminal;
			}
			return CitizenOccupationKey.Robber;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<Worker>(entity))
		{
			return CitizenOccupationKey.Worker;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Citizens.Student>(entity))
		{
			return CitizenOccupationKey.Student;
		}
		Citizen citizen = default(Citizen);
		if (EntitiesExtensions.TryGetComponent<Citizen>(entityManager, entity, ref citizen))
		{
			switch (citizen.GetAge())
			{
			case CitizenAge.Adult:
				return CitizenOccupationKey.Unemployed;
			case CitizenAge.Child:
			case CitizenAge.Teen:
				return CitizenOccupationKey.None;
			case CitizenAge.Elderly:
				return CitizenOccupationKey.Retired;
			}
		}
		return CitizenOccupationKey.Unknown;
	}

	public static CitizenEducationKey GetEducation(Citizen citizen)
	{
		return citizen.GetEducationLevel() switch
		{
			1 => CitizenEducationKey.PoorlyEducated, 
			2 => CitizenEducationKey.Educated, 
			3 => CitizenEducationKey.WellEducated, 
			4 => CitizenEducationKey.HighlyEducated, 
			_ => CitizenEducationKey.Uneducated, 
		};
	}

	public static Entity GetResidenceEntity(EntityManager entityManager, Entity citizenEntity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Entity household = ((EntityManager)(ref entityManager)).GetComponentData<HouseholdMember>(citizenEntity).m_Household;
		TouristHousehold touristHousehold = default(TouristHousehold);
		if (EntitiesExtensions.TryGetComponent<TouristHousehold>(entityManager, household, ref touristHousehold) && ((EntityManager)(ref entityManager)).Exists(touristHousehold.m_Hotel))
		{
			return touristHousehold.m_Hotel;
		}
		PropertyRenter propertyRenter = default(PropertyRenter);
		if (EntitiesExtensions.TryGetComponent<PropertyRenter>(entityManager, household, ref propertyRenter) && ((EntityManager)(ref entityManager)).Exists(propertyRenter.m_Property))
		{
			return propertyRenter.m_Property;
		}
		HomelessHousehold homelessHousehold = default(HomelessHousehold);
		if (EntitiesExtensions.TryGetComponent<HomelessHousehold>(entityManager, household, ref homelessHousehold) && ((EntityManager)(ref entityManager)).Exists(homelessHousehold.m_TempHome))
		{
			return homelessHousehold.m_TempHome;
		}
		return Entity.Null;
	}

	public static CitizenResidenceKey GetResidenceType(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Citizen componentData = ((EntityManager)(ref entityManager)).GetComponentData<Citizen>(entity);
		HouseholdMember componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<HouseholdMember>(entity);
		bool num = (componentData.m_State & CitizenFlags.Tourist) != 0;
		bool flag = ((EntityManager)(ref entityManager)).HasComponent<HomelessHousehold>(componentData2.m_Household);
		if (!num)
		{
			if (!flag)
			{
				return CitizenResidenceKey.Home;
			}
			return CitizenResidenceKey.Shelter;
		}
		return CitizenResidenceKey.Hotel;
	}

	public static Entity GetWorkplaceEntity(EntityManager entityManager, Entity citizenEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Worker worker = default(Worker);
		if (!EntitiesExtensions.TryGetComponent<Worker>(entityManager, citizenEntity, ref worker))
		{
			return Entity.Null;
		}
		PropertyRenter propertyRenter = default(PropertyRenter);
		if (!EntitiesExtensions.TryGetComponent<PropertyRenter>(entityManager, worker.m_Workplace, ref propertyRenter))
		{
			return worker.m_Workplace;
		}
		return propertyRenter.m_Property;
	}

	public static Entity GetCompanyEntity(EntityManager entityManager, Entity citizenEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Worker worker = default(Worker);
		if (!EntitiesExtensions.TryGetComponent<Worker>(entityManager, citizenEntity, ref worker))
		{
			return Entity.Null;
		}
		return worker.m_Workplace;
	}

	public static CitizenWorkplaceKey GetWorkplaceType(EntityManager entityManager, Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Worker worker = default(Worker);
		if (!EntitiesExtensions.TryGetComponent<Worker>(entityManager, entity, ref worker) || !((EntityManager)(ref entityManager)).HasComponent<CompanyData>(worker.m_Workplace))
		{
			return CitizenWorkplaceKey.Building;
		}
		return CitizenWorkplaceKey.Company;
	}

	public static Entity GetSchoolEntity(EntityManager entityManager, Entity citizenEntity, out int level)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Game.Citizens.Student student = default(Game.Citizens.Student);
		if (EntitiesExtensions.TryGetComponent<Game.Citizens.Student>(entityManager, citizenEntity, ref student))
		{
			level = student.m_Level;
			return student.m_School;
		}
		level = 0;
		return Entity.Null;
	}

	public static CitizenStateKey GetStateKey(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		Citizen componentData = ((EntityManager)(ref entityManager)).GetComponentData<Citizen>(entity);
		Household componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<Household>(((EntityManager)(ref entityManager)).GetComponentData<HouseholdMember>(entity).m_Household);
		if (CitizenUtils.IsDead(entityManager, entity))
		{
			return CitizenStateKey.Dead;
		}
		CurrentTransport currentTransport = default(CurrentTransport);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(entityManager, entity, ref currentTransport) && ((EntityManager)(ref entityManager)).HasComponent<Creature>(currentTransport.m_CurrentTransport) && ((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(currentTransport.m_CurrentTransport))
		{
			return CitizenStateKey.InvolvedInAccident;
		}
		if (TryGetTravelPurpose(entityManager, entity, out var purpose))
		{
			bool flag = (componentData.m_State & CitizenFlags.Tourist) != 0;
			bool flag2 = (componentData.m_State & CitizenFlags.Commuter) != 0;
			bool flag3 = (componentData2.m_Flags & HouseholdFlags.MovedIn) != 0;
			switch (purpose)
			{
			case Purpose.Shopping:
				return CitizenStateKey.Shopping;
			case Purpose.Leisure:
			case Purpose.VisitAttractions:
				if (!flag)
				{
					return CitizenStateKey.FreeTime;
				}
				return CitizenStateKey.Sightseeing;
			case Purpose.GoingHome:
				if (!flag)
				{
					if (!(flag3 || flag2))
					{
						return CitizenStateKey.MovingIn;
					}
					return CitizenStateKey.GoingHome;
				}
				return CitizenStateKey.GoingBackToHotel;
			case Purpose.GoingToWork:
				return CitizenStateKey.GoingToWork;
			case Purpose.Working:
				return CitizenStateKey.Working;
			case Purpose.Sleeping:
				return CitizenStateKey.Sleeping;
			case Purpose.Exporting:
				return CitizenStateKey.Traveling;
			case Purpose.MovingAway:
				if (!flag)
				{
					return CitizenStateKey.MovingAway;
				}
				return CitizenStateKey.LeavingCity;
			case Purpose.Studying:
				return CitizenStateKey.Studying;
			case Purpose.GoingToSchool:
				return CitizenStateKey.GoingToSchool;
			case Purpose.Hospital:
				return CitizenStateKey.SeekingMedicalHelp;
			case Purpose.Safety:
				if (!PathEndReached(entityManager, entity))
				{
					return CitizenStateKey.GettingToSafety;
				}
				return CitizenStateKey.Safe;
			case Purpose.EmergencyShelter:
				return CitizenStateKey.Evacuating;
			case Purpose.Crime:
				return CitizenStateKey.CommittingCrime;
			case Purpose.GoingToJail:
				return CitizenStateKey.GoingToJail;
			case Purpose.GoingToPrison:
				return CitizenStateKey.GoingToPrison;
			case Purpose.InJail:
				return CitizenStateKey.InJail;
			case Purpose.InPrison:
				return CitizenStateKey.InPrison;
			case Purpose.Escape:
				return CitizenStateKey.Escaping;
			case Purpose.InHospital:
				return CitizenStateKey.InHospital;
			case Purpose.SendMail:
				return CitizenStateKey.SendMail;
			case Purpose.InEmergencyShelter:
				return CitizenStateKey.InEmergencyShelter;
			default:
				return CitizenStateKey.Idling;
			}
		}
		return CitizenStateKey.Idling;
	}

	private static bool TryGetTravelPurpose(EntityManager entityManager, Entity entity, out Purpose purpose)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		CurrentTransport currentTransport = default(CurrentTransport);
		Divert divert = default(Divert);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(entityManager, entity, ref currentTransport) && EntitiesExtensions.TryGetComponent<Divert>(entityManager, currentTransport.m_CurrentTransport, ref divert))
		{
			Purpose purpose2 = divert.m_Purpose;
			if (purpose2 == Purpose.Safety || purpose2 == Purpose.Shopping || purpose2 == Purpose.SendMail)
			{
				purpose = divert.m_Purpose;
				return true;
			}
		}
		TravelPurpose travelPurpose = default(TravelPurpose);
		if (EntitiesExtensions.TryGetComponent<TravelPurpose>(entityManager, entity, ref travelPurpose))
		{
			purpose = travelPurpose.m_Purpose;
			return true;
		}
		purpose = Purpose.None;
		return false;
	}

	private static bool PathEndReached(EntityManager entityManager, Entity citizen)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		CurrentTransport currentTransport = default(CurrentTransport);
		HumanCurrentLane currentLane = default(HumanCurrentLane);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(entityManager, citizen, ref currentTransport) && EntitiesExtensions.TryGetComponent<HumanCurrentLane>(entityManager, currentTransport.m_CurrentTransport, ref currentLane))
		{
			return CreatureUtils.PathEndReached(currentLane);
		}
		return false;
	}

	public static NativeList<CitizenCondition> GetCitizenConditions(EntityManager entityManager, Entity entity, Citizen citizen, HouseholdMember householdMember, NativeList<CitizenCondition> conditions)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		HealthProblem healthProblem = default(HealthProblem);
		if (EntitiesExtensions.TryGetComponent<HealthProblem>(entityManager, entity, ref healthProblem))
		{
			if ((healthProblem.m_Flags & HealthProblemFlags.Sick) != HealthProblemFlags.None)
			{
				CitizenCondition citizenCondition = new CitizenCondition(CitizenConditionKey.Sick);
				conditions.Add(ref citizenCondition);
			}
			if ((healthProblem.m_Flags & HealthProblemFlags.Injured) != HealthProblemFlags.None)
			{
				CitizenCondition citizenCondition = new CitizenCondition(CitizenConditionKey.Injured);
				conditions.Add(ref citizenCondition);
			}
			if ((healthProblem.m_Flags & (HealthProblemFlags.InDanger | HealthProblemFlags.Trapped)) != HealthProblemFlags.None)
			{
				CitizenCondition citizenCondition = new CitizenCondition(CitizenConditionKey.InDistress);
				conditions.Add(ref citizenCondition);
			}
		}
		if (!((EntityManager)(ref entityManager)).HasComponent<CommuterHousehold>(householdMember.m_Household) && !((EntityManager)(ref entityManager)).HasComponent<TouristHousehold>(householdMember.m_Household) && BuildingUtils.IsHomelessHousehold(entityManager, householdMember.m_Household))
		{
			CitizenCondition citizenCondition = new CitizenCondition(CitizenConditionKey.Homeless);
			conditions.Add(ref citizenCondition);
		}
		CurrentBuilding currentBuilding = default(CurrentBuilding);
		if (EntitiesExtensions.TryGetComponent<CurrentBuilding>(entityManager, entity, ref currentBuilding) && ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.EmergencyShelter>(currentBuilding.m_CurrentBuilding))
		{
			CitizenCondition citizenCondition = new CitizenCondition(CitizenConditionKey.Evacuated);
			conditions.Add(ref citizenCondition);
		}
		if (citizen.m_Health <= 25)
		{
			CitizenCondition citizenCondition = new CitizenCondition(CitizenConditionKey.Weak);
			conditions.Add(ref citizenCondition);
		}
		if (citizen.m_WellBeing <= 25)
		{
			CitizenCondition citizenCondition = new CitizenCondition(CitizenConditionKey.Unwell);
			conditions.Add(ref citizenCondition);
		}
		NativeSortExtension.Sort<CitizenCondition>(conditions);
		return conditions;
	}

	public static CitizenHappiness GetCitizenHappiness(Citizen citizen)
	{
		return new CitizenHappiness((CitizenHappinessKey)CitizenUtils.GetHappinessKey(citizen.Happiness));
	}

	public static CitizenHappiness GetCitizenHappiness(int happiness)
	{
		return new CitizenHappiness((CitizenHappinessKey)CitizenUtils.GetHappinessKey(happiness));
	}
}

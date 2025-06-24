using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Objects;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class CitizenSection : InfoSectionBase
{
	private EntityQuery m_HappinessParameterQuery;

	protected override string group => "CitizenSection";

	private CitizenKey citizenKey { get; set; }

	private CitizenStateKey stateKey { get; set; }

	private Entity householdEntity { get; set; }

	private Entity residenceEntity { get; set; }

	private CitizenResidenceKey residenceKey { get; set; }

	private Entity workplaceEntity { get; set; }

	private Entity companyEntity { get; set; }

	private CitizenWorkplaceKey workplaceKey { get; set; }

	private CitizenOccupationKey occupationKey { get; set; }

	private CitizenJobLevelKey jobLevelKey { get; set; }

	private Entity schoolEntity { get; set; }

	private int schoolLevel { get; set; }

	private CitizenEducationKey educationKey { get; set; }

	private CitizenAgeKey ageKey { get; set; }

	private HouseholdWealthKey wealthKey { get; set; }

	private Entity destinationEntity { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_HappinessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenHappinessParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_HappinessParameterQuery);
	}

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		householdEntity = Entity.Null;
		residenceEntity = Entity.Null;
		workplaceEntity = Entity.Null;
		schoolEntity = Entity.Null;
		destinationEntity = Entity.Null;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<HouseholdMember>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Citizen>(selectedEntity);
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Citizen componentData = ((EntityManager)(ref entityManager)).GetComponentData<Citizen>(selectedEntity);
		HouseholdMember householdMember = default(HouseholdMember);
		if (EntitiesExtensions.TryGetComponent<HouseholdMember>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref householdMember))
		{
			householdEntity = householdMember.m_Household;
			citizenKey = CitizenKey.Citizen;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<CommuterHousehold>(householdMember.m_Household))
			{
				citizenKey = CitizenKey.Commuter;
			}
			else
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<TouristHousehold>(householdMember.m_Household))
				{
					citizenKey = CitizenKey.Tourist;
				}
			}
			wealthKey = CitizenUIUtils.GetHouseholdWealth(((ComponentSystemBase)this).EntityManager, householdEntity, ((EntityQuery)(ref m_HappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>());
		}
		stateKey = CitizenUIUtils.GetStateKey(((ComponentSystemBase)this).EntityManager, selectedEntity);
		residenceEntity = CitizenUIUtils.GetResidenceEntity(((ComponentSystemBase)this).EntityManager, selectedEntity);
		residenceKey = CitizenUIUtils.GetResidenceType(((ComponentSystemBase)this).EntityManager, selectedEntity);
		workplaceEntity = CitizenUIUtils.GetWorkplaceEntity(((ComponentSystemBase)this).EntityManager, selectedEntity);
		companyEntity = CitizenUIUtils.GetCompanyEntity(((ComponentSystemBase)this).EntityManager, selectedEntity);
		workplaceKey = CitizenUIUtils.GetWorkplaceType(((ComponentSystemBase)this).EntityManager, selectedEntity);
		schoolEntity = CitizenUIUtils.GetSchoolEntity(((ComponentSystemBase)this).EntityManager, selectedEntity, out var level);
		schoolLevel = level;
		occupationKey = CitizenUIUtils.GetOccupation(((ComponentSystemBase)this).EntityManager, selectedEntity);
		jobLevelKey = CitizenUIUtils.GetJobLevel(((ComponentSystemBase)this).EntityManager, selectedEntity);
		ageKey = CitizenUIUtils.GetAge(((ComponentSystemBase)this).EntityManager, selectedEntity);
		educationKey = CitizenUIUtils.GetEducation(componentData);
		destinationEntity = GetDestination();
		if ((componentData.m_State & CitizenFlags.Male) != CitizenFlags.None)
		{
			base.tooltipTags.Add(TooltipTags.Male.ToString());
		}
	}

	private Entity GetDestination()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		CurrentTransport currentTransport = default(CurrentTransport);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref currentTransport))
		{
			Entity val = Entity.Null;
			Divert divert = default(Divert);
			if (EntitiesExtensions.TryGetComponent<Divert>(((ComponentSystemBase)this).EntityManager, currentTransport.m_CurrentTransport, ref divert))
			{
				Purpose purpose = divert.m_Purpose;
				if (purpose == Purpose.Safety || purpose == Purpose.Shopping || purpose == Purpose.SendMail)
				{
					val = divert.m_Target;
				}
			}
			Target target = default(Target);
			if (val == Entity.Null && EntitiesExtensions.TryGetComponent<Target>(((ComponentSystemBase)this).EntityManager, currentTransport.m_CurrentTransport, ref target))
			{
				val = target.m_Target;
			}
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(val))
			{
				return val;
			}
			Owner owner = default(Owner);
			if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val, ref owner))
			{
				return owner.m_Owner;
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).Exists(val))
			{
				return val;
			}
		}
		return Entity.Null;
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("citizenKey");
		writer.Write(Enum.GetName(typeof(CitizenKey), citizenKey));
		writer.PropertyName("stateKey");
		writer.Write(Enum.GetName(typeof(CitizenStateKey), stateKey));
		writer.PropertyName("household");
		if (householdEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, householdEntity);
		}
		writer.PropertyName("householdEntity");
		if (householdEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			UnityWriters.Write(writer, householdEntity);
		}
		writer.PropertyName("residence");
		if (residenceEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, residenceEntity);
		}
		writer.PropertyName("residenceEntity");
		if (residenceEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			UnityWriters.Write(writer, residenceEntity);
		}
		writer.PropertyName("residenceKey");
		writer.Write(Enum.GetName(typeof(CitizenResidenceKey), residenceKey));
		writer.PropertyName("workplace");
		if (companyEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, companyEntity);
		}
		writer.PropertyName("workplaceEntity");
		if (workplaceEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			UnityWriters.Write(writer, workplaceEntity);
		}
		writer.PropertyName("workplaceKey");
		writer.Write(Enum.GetName(typeof(CitizenWorkplaceKey), workplaceKey));
		writer.PropertyName("occupationKey");
		writer.Write(Enum.GetName(typeof(CitizenOccupationKey), occupationKey));
		writer.PropertyName("jobLevelKey");
		writer.Write(Enum.GetName(typeof(CitizenJobLevelKey), jobLevelKey));
		writer.PropertyName("school");
		if (schoolEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, schoolEntity);
		}
		writer.PropertyName("schoolEntity");
		if (schoolEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			UnityWriters.Write(writer, schoolEntity);
		}
		writer.PropertyName("schoolLevel");
		writer.Write(schoolLevel);
		writer.PropertyName("educationKey");
		writer.Write(Enum.GetName(typeof(CitizenEducationKey), educationKey));
		writer.PropertyName("ageKey");
		writer.Write(Enum.GetName(typeof(CitizenAgeKey), ageKey));
		writer.PropertyName("wealthKey");
		writer.Write(Enum.GetName(typeof(HouseholdWealthKey), wealthKey));
		writer.PropertyName("destination");
		if (destinationEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, destinationEntity);
		}
		writer.PropertyName("destinationEntity");
		if (destinationEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			UnityWriters.Write(writer, destinationEntity);
		}
	}

	[Preserve]
	public CitizenSection()
	{
	}
}

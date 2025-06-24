using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Citizens;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Game.Triggers;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class LifePathUISystem : UISystemBase
{
	private struct FollowedCitizenComparer : IComparer<Entity>
	{
		private EntityManager m_EntityManager;

		public FollowedCitizenComparer(EntityManager entityManager)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_EntityManager = entityManager;
		}

		public int Compare(Entity a, Entity b)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			int num = ((EntityManager)(ref m_EntityManager)).GetComponentData<Followed>(a).m_Priority.CompareTo(((EntityManager)(ref m_EntityManager)).GetComponentData<Followed>(b).m_Priority);
			if (num == 0)
			{
				return ((Entity)(ref a)).CompareTo(b);
			}
			return num;
		}
	}

	private const string kGroup = "lifePath";

	private LifePathEventSystem m_LifePathEventSystem;

	private NameSystem m_NameSystem;

	private SelectedInfoUISystem m_SelectedInfoUISystem;

	private ChirperUISystem m_ChirperUISystem;

	private EntityQuery m_FollowedQuery;

	private EntityQuery m_HappinessParameterQuery;

	private int m_FollowedVersion;

	private int m_LifePathEntryVersion;

	private int m_ChirpVersion;

	private RawValueBinding m_FollowedCitizensBinding;

	private RawMapBinding<Entity> m_LifePathDetailsBinding;

	private RawMapBinding<Entity> m_LifePathItemsBinding;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00ce: Expected O, but got Unknown
		base.OnCreate();
		m_LifePathEventSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LifePathEventSystem>();
		m_NameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NameSystem>();
		m_SelectedInfoUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
		m_ChirperUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ChirperUISystem>();
		m_FollowedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<Followed>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_HappinessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenHappinessParameterData>() });
		RawValueBinding val = new RawValueBinding("lifePath", "followedCitizens", (Action<IJsonWriter>)BindFollowedCitizens);
		RawValueBinding binding = val;
		m_FollowedCitizensBinding = val;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_LifePathDetailsBinding = new RawMapBinding<Entity>("lifePath", "lifePathDetails", (Action<IJsonWriter, Entity>)delegate(IJsonWriter binder, Entity citizen)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			BindLifePathDetails(binder, citizen);
		}, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)(m_LifePathItemsBinding = new RawMapBinding<Entity>("lifePath", "lifePathItems", (Action<IJsonWriter, Entity>)delegate(IJsonWriter binder, Entity citizen)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			BindLifePathItems(binder, citizen);
		}, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("lifePath", "followCitizen", (Action<Entity>)FollowCitizen, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("lifePath", "unfollowCitizen", (Action<Entity>)UnfollowCitizen, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new ValueBinding<int>("lifePath", "maxFollowedCitizens", LifePathEventSystem.kMaxFollowed, (IWriter<int>)null, (EqualityComparer<int>)null));
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<Followed>();
		if (m_FollowedVersion != componentOrderVersion)
		{
			m_FollowedCitizensBinding.Update();
			((MapBindingBase<Entity>)(object)m_LifePathDetailsBinding).UpdateAll();
			m_FollowedVersion = componentOrderVersion;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion2 = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<LifePathEntry>();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion3 = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<Game.Triggers.Chirp>();
		if (m_LifePathEntryVersion != componentOrderVersion2 || m_ChirpVersion != componentOrderVersion3)
		{
			((MapBindingBase<Entity>)(object)m_LifePathItemsBinding).UpdateAll();
			m_LifePathEntryVersion = componentOrderVersion2;
			m_ChirpVersion = componentOrderVersion3;
		}
	}

	private void FollowCitizen(Entity citizen)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_LifePathEventSystem.FollowCitizen(citizen);
	}

	private void UnfollowCitizen(Entity citizen)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_LifePathEventSystem.UnfollowCitizen(citizen);
		m_SelectedInfoUISystem.SetDirty();
	}

	private void BindFollowedCitizens(IJsonWriter binder)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> sortedFollowedCitizens = GetSortedFollowedCitizens();
		JsonWriterExtensions.ArrayBegin(binder, sortedFollowedCitizens.Length);
		for (int i = 0; i < sortedFollowedCitizens.Length; i++)
		{
			Entity val = sortedFollowedCitizens[i];
			binder.TypeBegin("lifePath.FollowedCitizen");
			binder.PropertyName("entity");
			UnityWriters.Write(binder, val);
			binder.PropertyName("name");
			m_NameSystem.BindName(binder, val);
			binder.PropertyName("age");
			binder.Write(Enum.GetName(typeof(CitizenAgeKey), CitizenUIUtils.GetAge(((ComponentSystemBase)this).EntityManager, val)));
			binder.PropertyName("dead");
			binder.Write(CitizenUtils.IsDead(((ComponentSystemBase)this).EntityManager, val));
			binder.TypeEnd();
		}
		binder.ArrayEnd();
		sortedFollowedCitizens.Dispose();
	}

	private NativeArray<Entity> GetSortedFollowedCitizens()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_FollowedQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		NativeSortExtension.Sort<Entity, FollowedCitizenComparer>(val, new FollowedCitizenComparer(((ComponentSystemBase)this).EntityManager));
		return val;
	}

	private void BindLifePathDetails(IJsonWriter binder, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		Citizen citizen = default(Citizen);
		if (EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, entity, ref citizen))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Followed>(entity))
			{
				Entity residenceEntity = CitizenUIUtils.GetResidenceEntity(((ComponentSystemBase)this).EntityManager, entity);
				CitizenResidenceKey residenceType = CitizenUIUtils.GetResidenceType(((ComponentSystemBase)this).EntityManager, entity);
				Entity workplaceEntity = CitizenUIUtils.GetWorkplaceEntity(((ComponentSystemBase)this).EntityManager, entity);
				Entity companyEntity = CitizenUIUtils.GetCompanyEntity(((ComponentSystemBase)this).EntityManager, entity);
				CitizenWorkplaceKey workplaceType = CitizenUIUtils.GetWorkplaceType(((ComponentSystemBase)this).EntityManager, entity);
				int level;
				Entity schoolEntity = CitizenUIUtils.GetSchoolEntity(((ComponentSystemBase)this).EntityManager, entity, out level);
				CitizenOccupationKey occupation = CitizenUIUtils.GetOccupation(((ComponentSystemBase)this).EntityManager, entity);
				CitizenJobLevelKey jobLevel = CitizenUIUtils.GetJobLevel(((ComponentSystemBase)this).EntityManager, entity);
				CitizenAgeKey age = CitizenUIUtils.GetAge(((ComponentSystemBase)this).EntityManager, entity);
				CitizenEducationKey education = CitizenUIUtils.GetEducation(citizen);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				HouseholdMember componentData = ((EntityManager)(ref entityManager)).GetComponentData<HouseholdMember>(entity);
				NativeList<CitizenCondition> citizenConditions = CitizenUIUtils.GetCitizenConditions(((ComponentSystemBase)this).EntityManager, entity, citizen, componentData, new NativeList<CitizenCondition>(AllocatorHandle.op_Implicit((Allocator)3)));
				HouseholdWealthKey householdWealth = CitizenUIUtils.GetHouseholdWealth(((ComponentSystemBase)this).EntityManager, componentData.m_Household, ((EntityQuery)(ref m_HappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>());
				bool flag = CitizenUtils.IsDead(((ComponentSystemBase)this).EntityManager, entity);
				binder.TypeBegin("lifePath.LifePathDetails");
				binder.PropertyName("entity");
				UnityWriters.Write(binder, entity);
				binder.PropertyName("name");
				m_NameSystem.BindName(binder, entity);
				binder.PropertyName("avatar");
				binder.WriteNull();
				binder.PropertyName("randomIndex");
				binder.Write(GetRandomIndex(entity));
				binder.PropertyName("birthDay");
				binder.Write((int)citizen.m_BirthDay);
				binder.PropertyName("age");
				binder.Write(Enum.GetName(typeof(CitizenAgeKey), age));
				binder.PropertyName("education");
				binder.Write(Enum.GetName(typeof(CitizenEducationKey), education));
				binder.PropertyName("wealth");
				binder.Write(Enum.GetName(typeof(HouseholdWealthKey), householdWealth));
				binder.PropertyName("occupation");
				binder.Write(Enum.GetName(typeof(CitizenOccupationKey), occupation));
				binder.PropertyName("jobLevel");
				binder.Write(Enum.GetName(typeof(CitizenJobLevelKey), jobLevel));
				binder.PropertyName("residenceName");
				if (residenceEntity == Entity.Null)
				{
					binder.WriteNull();
				}
				else
				{
					m_NameSystem.BindName(binder, residenceEntity);
				}
				binder.PropertyName("residenceEntity");
				if (residenceEntity == Entity.Null)
				{
					binder.WriteNull();
				}
				else
				{
					UnityWriters.Write(binder, residenceEntity);
				}
				binder.PropertyName("residenceKey");
				binder.Write(Enum.GetName(typeof(CitizenResidenceKey), residenceType));
				binder.PropertyName("workplaceName");
				if (companyEntity == Entity.Null)
				{
					binder.WriteNull();
				}
				else
				{
					m_NameSystem.BindName(binder, companyEntity);
				}
				binder.PropertyName("workplaceEntity");
				if (workplaceEntity == Entity.Null)
				{
					binder.WriteNull();
				}
				else
				{
					UnityWriters.Write(binder, workplaceEntity);
				}
				binder.PropertyName("workplaceKey");
				binder.Write(Enum.GetName(typeof(CitizenWorkplaceKey), workplaceType));
				binder.PropertyName("schoolName");
				if (schoolEntity == Entity.Null)
				{
					binder.WriteNull();
				}
				else
				{
					m_NameSystem.BindName(binder, schoolEntity);
				}
				binder.PropertyName("schoolEntity");
				if (schoolEntity == Entity.Null)
				{
					binder.WriteNull();
				}
				else
				{
					UnityWriters.Write(binder, schoolEntity);
				}
				binder.PropertyName("conditions");
				if (flag)
				{
					JsonWriterExtensions.WriteEmptyArray(binder);
				}
				else
				{
					JsonWriterExtensions.ArrayBegin(binder, citizenConditions.Length);
					for (int i = 0; i < citizenConditions.Length; i++)
					{
						JsonWriterExtensions.Write<CitizenCondition>(binder, citizenConditions[i]);
					}
					binder.ArrayEnd();
				}
				binder.PropertyName("happiness");
				if (flag)
				{
					binder.WriteNull();
				}
				else
				{
					JsonWriterExtensions.Write<CitizenHappiness>(binder, CitizenUIUtils.GetCitizenHappiness(citizen));
				}
				binder.PropertyName("state");
				binder.Write(Enum.GetName(typeof(CitizenStateKey), CitizenUIUtils.GetStateKey(((ComponentSystemBase)this).EntityManager, entity)));
				binder.TypeEnd();
				citizenConditions.Dispose();
				return;
			}
		}
		binder.WriteNull();
	}

	private void BindLifePathItems(IJsonWriter binder, Entity citizen)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<LifePathEntry> val = default(DynamicBuffer<LifePathEntry>);
		if (EntitiesExtensions.TryGetBuffer<LifePathEntry>(((ComponentSystemBase)this).EntityManager, citizen, true, ref val))
		{
			JsonWriterExtensions.ArrayBegin(binder, val.Length);
			for (int num = val.Length - 1; num >= 0; num--)
			{
				Entity entity = val[num].m_Entity;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Deleted>(entity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Triggers.Chirp>(entity))
					{
						m_ChirperUISystem.BindChirp(binder, entity);
					}
					else
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<Game.Triggers.LifePathEvent>(entity))
						{
							BindLifePathEvent(binder, entity);
						}
						else
						{
							binder.WriteNull();
						}
					}
				}
				else
				{
					binder.WriteNull();
				}
			}
			binder.ArrayEnd();
		}
		else
		{
			JsonWriterExtensions.WriteEmptyArray(binder);
		}
	}

	private void BindLifePathEvent(IJsonWriter binder, Entity entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		string messageID = m_ChirperUISystem.GetMessageID(entity);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Triggers.LifePathEvent componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Triggers.LifePathEvent>(entity);
		binder.TypeBegin("lifePath.LifePathEvent");
		binder.PropertyName("entity");
		UnityWriters.Write(binder, entity);
		binder.PropertyName("date");
		binder.Write(componentData.m_Date);
		binder.PropertyName("messageId");
		binder.Write(messageID);
		binder.TypeEnd();
	}

	private int GetRandomIndex(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<RandomLocalizationIndex> val = default(DynamicBuffer<RandomLocalizationIndex>);
		if (EntitiesExtensions.TryGetBuffer<RandomLocalizationIndex>(((ComponentSystemBase)this).EntityManager, entity, true, ref val) && val.Length > 0)
		{
			return val[0].m_Index;
		}
		return 0;
	}

	[Preserve]
	public LifePathUISystem()
	{
	}
}

using System;
using System.Collections.Generic;
using Colossal.UI.Binding;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Simulation;
using Game.Tools;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class PopulationInfoviewUISystem : InfoviewUISystemBase
{
	private const string kGroup = "populationInfo";

	private CityStatisticsSystem m_CityStatisticsSystem;

	private CitySystem m_CitySystem;

	private CountWorkplacesSystem m_CountWorkplacesSystem;

	private CountHouseholdDataSystem m_CountHouseholdDataSystem;

	private ValueBinding<int> m_Population;

	private ValueBinding<int> m_Employed;

	private ValueBinding<int> m_Jobs;

	private ValueBinding<float> m_Unemployment;

	private ValueBinding<float> m_Homelessness;

	private ValueBinding<int> m_BirthRate;

	private ValueBinding<int> m_DeathRate;

	private ValueBinding<int> m_MovedIn;

	private ValueBinding<int> m_MovedAway;

	private ValueBinding<int> m_Homeless;

	private RawValueBinding m_AgeData;

	private EntityQuery m_WorkProviderModifiedQuery;

	private EntityQuery m_PopulationModifiedQuery;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_Population).active && !((EventBindingBase)m_Employed).active && !((EventBindingBase)m_Jobs).active && !((EventBindingBase)m_Unemployment).active && !((EventBindingBase)m_BirthRate).active && !((EventBindingBase)m_DeathRate).active && !((EventBindingBase)m_MovedIn).active && !((EventBindingBase)m_MovedAway).active && !((EventBindingBase)m_AgeData).active)
			{
				return ((EventBindingBase)m_Homeless).active;
			}
			return true;
		}
	}

	protected override bool Modified
	{
		get
		{
			if (((EntityQuery)(ref m_WorkProviderModifiedQuery)).IsEmptyIgnoreFilter)
			{
				return ((EntityQuery)(ref m_PopulationModifiedQuery)).IsEmptyIgnoreFilter;
			}
			return true;
		}
	}

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
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Expected O, but got Unknown
		//IL_025c: Expected O, but got Unknown
		base.OnCreate();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CountWorkplacesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountWorkplacesSystem>();
		m_CountHouseholdDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountHouseholdDataSystem>();
		m_WorkProviderModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<WorkProvider>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.Exclude<Temp>()
		});
		m_PopulationModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Population>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.Exclude<Temp>()
		});
		AddBinding((IBinding)(object)(m_Population = new ValueBinding<int>("populationInfo", "population", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_Employed = new ValueBinding<int>("populationInfo", "employed", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_Jobs = new ValueBinding<int>("populationInfo", "jobs", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_Unemployment = new ValueBinding<float>("populationInfo", "unemployment", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_BirthRate = new ValueBinding<int>("populationInfo", "birthRate", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_DeathRate = new ValueBinding<int>("populationInfo", "deathRate", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_MovedIn = new ValueBinding<int>("populationInfo", "movedIn", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_MovedAway = new ValueBinding<int>("populationInfo", "movedAway", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_Homeless = new ValueBinding<int>("populationInfo", "homeless", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_Homelessness = new ValueBinding<float>("populationInfo", "homelessness", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		RawValueBinding val = new RawValueBinding("populationInfo", "ageData", (Action<IJsonWriter>)UpdateAgeData);
		RawValueBinding binding = val;
		m_AgeData = val;
		AddBinding((IBinding)(object)binding);
	}

	protected override void PerformUpdate()
	{
		UpdateBindings();
	}

	private void UpdateBindings()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		m_Jobs.Update(m_CountWorkplacesSystem.GetTotalWorkplaces().TotalCount);
		m_Employed.Update(m_CountHouseholdDataSystem.CityWorkerCount);
		m_Unemployment.Update(m_CountHouseholdDataSystem.UnemploymentRate);
		m_Homelessness.Update(m_CountHouseholdDataSystem.HomelessnessRate);
		m_Homeless.Update(m_CountHouseholdDataSystem.HomelessCitizenCount);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Population componentData = ((EntityManager)(ref entityManager)).GetComponentData<Population>(m_CitySystem.City);
		m_Population.Update(componentData.m_Population);
		m_AgeData.Update();
		UpdateStatistics();
	}

	private void UpdateAgeData(IJsonWriter binder)
	{
		binder.TypeBegin("infoviews.ChartData");
		binder.PropertyName("values");
		binder.ArrayBegin(4u);
		binder.Write(m_CountHouseholdDataSystem.ChildrenCount);
		binder.Write(m_CountHouseholdDataSystem.TeenCount);
		binder.Write(m_CountHouseholdDataSystem.AdultCount);
		binder.Write(m_CountHouseholdDataSystem.SeniorCount);
		binder.ArrayEnd();
		binder.PropertyName("total");
		binder.Write(m_CountHouseholdDataSystem.MovedInCitizenCount);
		binder.TypeEnd();
	}

	private void UpdateStatistics()
	{
		m_BirthRate.Update(m_CityStatisticsSystem.GetStatisticValue(StatisticType.BirthRate));
		m_DeathRate.Update(m_CityStatisticsSystem.GetStatisticValue(StatisticType.DeathRate));
		m_MovedIn.Update(m_CityStatisticsSystem.GetStatisticValue(StatisticType.CitizensMovedIn));
		m_MovedAway.Update(m_CityStatisticsSystem.GetStatisticValue(StatisticType.CitizensMovedAway));
	}

	[Preserve]
	public PopulationInfoviewUISystem()
	{
	}
}

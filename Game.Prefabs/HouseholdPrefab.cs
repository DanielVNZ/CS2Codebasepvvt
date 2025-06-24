using System;
using System.Collections.Generic;
using Game.Agents;
using Game.Citizens;
using Game.Economy;
using Game.Simulation;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Citizens/", new Type[] { })]
public class HouseholdPrefab : ArchetypePrefab
{
	public int m_ResourceConsumption;

	public int m_InitialWealthRange;

	public int m_InitialWealthOffset;

	[Tooltip("Percentage chance of arriving with a car")]
	public int m_InitialCarProbability;

	public int m_ChildCount;

	public int m_AdultCount;

	public int m_ElderlyCount;

	[Tooltip("Guaranteed to be in college/uni age and have education level 2")]
	public int m_StudentCount;

	public int m_FirstPetProbability = 20;

	public int m_NextPetProbability = 10;

	[Tooltip("Is this prefab only for households that are created when kids move out of home or people divorce, which do not need random citizens")]
	public bool m_DynamicHousehold;

	public int m_Weight = 1;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<HouseholdData>());
		if (m_DynamicHousehold)
		{
			components.Add(ComponentType.ReadWrite<DynamicHousehold>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Household>());
		components.Add(ComponentType.ReadWrite<HouseholdNeed>());
		components.Add(ComponentType.ReadWrite<HouseholdCitizen>());
		components.Add(ComponentType.ReadWrite<TaxPayer>());
		components.Add(ComponentType.ReadWrite<Resources>());
		components.Add(ComponentType.ReadWrite<PropertySeeker>());
		components.Add(ComponentType.ReadWrite<UpdateFrame>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<HouseholdData>(entity, new HouseholdData
		{
			m_InitialCarProbability = m_InitialCarProbability,
			m_InitialWealthOffset = m_InitialWealthOffset,
			m_InitialWealthRange = m_InitialWealthRange,
			m_ChildCount = m_ChildCount,
			m_AdultCount = m_AdultCount,
			m_ElderCount = m_ElderlyCount,
			m_StudentCount = m_StudentCount,
			m_FirstPetProbability = m_FirstPetProbability,
			m_NextPetProbability = m_NextPetProbability,
			m_Weight = m_Weight
		});
	}
}

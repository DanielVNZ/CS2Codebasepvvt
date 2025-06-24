using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class LeisureParametersPrefab : PrefabBase
{
	public EventPrefab m_TravelingEvent;

	public EventPrefab m_AttractionPrefab;

	public EventPrefab m_SightseeingPrefab;

	public int m_LeisureRandomFactor = 512;

	[Tooltip("The lodging resource consuming speed of tourist.")]
	public int m_TouristLodgingConsumePerDay = 100;

	[Tooltip("The service available consuming speed of tourist")]
	public int m_TouristServiceConsumePerDay = 100;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_TravelingEvent);
		prefabs.Add(m_AttractionPrefab);
		prefabs.Add(m_SightseeingPrefab);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<LeisureParametersData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		base.GetArchetypeComponents(components);
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		LeisureParametersData leisureParametersData = default(LeisureParametersData);
		leisureParametersData.m_TravelingPrefab = orCreateSystemManaged.GetEntity(m_TravelingEvent);
		leisureParametersData.m_AttractionPrefab = orCreateSystemManaged.GetEntity(m_AttractionPrefab);
		leisureParametersData.m_SightseeingPrefab = orCreateSystemManaged.GetEntity(m_SightseeingPrefab);
		leisureParametersData.m_LeisureRandomFactor = m_LeisureRandomFactor;
		leisureParametersData.m_TouristLodgingConsumePerDay = m_TouristLodgingConsumePerDay;
		leisureParametersData.m_TouristServiceConsumePerDay = m_TouristServiceConsumePerDay;
		((EntityManager)(ref entityManager)).SetComponentData<LeisureParametersData>(entity, leisureParametersData);
	}
}

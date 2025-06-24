using System;
using System.Collections.Generic;
using Game.City;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Statistics/", new Type[] { })]
public class StatisticsPrefab : ArchetypePrefab
{
	public UIStatisticsCategoryPrefab m_Category;

	public UIStatisticsGroupPrefab m_Group;

	public StatisticType m_StatisticsType;

	public StatisticCollectionType m_CollectionType;

	public StatisticUnitType m_UnitType;

	public Color m_Color = Color.grey;

	public bool m_Stacked = true;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<StatisticsData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<CityStatistic>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		Entity category = (((Object)(object)m_Category != (Object)null) ? existingSystemManaged.GetEntity(m_Category) : Entity.Null);
		Entity val = (((Object)(object)m_Group != (Object)null) ? existingSystemManaged.GetEntity(m_Group) : Entity.Null);
		StatisticsData statisticsData = new StatisticsData
		{
			m_Group = val,
			m_Category = category,
			m_CollectionType = m_CollectionType,
			m_StatisticType = m_StatisticsType,
			m_UnitType = m_UnitType,
			m_Color = m_Color,
			m_Stacked = m_Stacked
		};
		((EntityManager)(ref entityManager)).SetComponentData<StatisticsData>(entity, statisticsData);
	}

	public static Entity CreateInstance(EntityManager entityManager, Entity entity, ArchetypeData archetypeData, int parameter = 0)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Entity val = ((EntityManager)(ref entityManager)).CreateEntity(archetypeData.m_Archetype);
		PrefabRef prefabRef = new PrefabRef
		{
			m_Prefab = entity
		};
		((EntityManager)(ref entityManager)).AddComponentData<PrefabRef>(val, prefabRef);
		if (((EntityManager)(ref entityManager)).HasComponent<StatisticParameter>(val))
		{
			((EntityManager)(ref entityManager)).SetComponentData<StatisticParameter>(val, new StatisticParameter
			{
				m_Value = parameter
			});
		}
		return val;
	}
}

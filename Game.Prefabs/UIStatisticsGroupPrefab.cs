using System;
using System.Collections.Generic;
using Game.City;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("UI/", new Type[] { })]
public class UIStatisticsGroupPrefab : UIGroupPrefab
{
	public Color m_Color = Color.black;

	public UIStatisticsCategoryPrefab m_Category;

	public StatisticUnitType m_UnitType;

	public bool m_Stacked;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<UIStatisticsGroupData>());
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_Category != (Object)null)
		{
			prefabs.Add(m_Category);
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		Entity category = (((Object)(object)m_Category != (Object)null) ? existingSystemManaged.GetEntity(m_Category) : Entity.Null);
		((EntityManager)(ref entityManager)).SetComponentData<UIStatisticsGroupData>(entity, new UIStatisticsGroupData
		{
			m_Category = category,
			m_Color = m_Color,
			m_UnitType = m_UnitType,
			m_Stacked = m_Stacked
		});
	}
}

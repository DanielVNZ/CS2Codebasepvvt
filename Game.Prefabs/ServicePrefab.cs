using System;
using System.Collections.Generic;
using Game.City;
using Game.Simulation;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Services/", new Type[] { })]
public class ServicePrefab : PrefabBase
{
	[SerializeField]
	private PlayerResource[] m_CityResources;

	[SerializeField]
	private CityService m_Service;

	[SerializeField]
	private bool m_BudgetAdjustable = true;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ServiceData>());
		components.Add(ComponentType.ReadWrite<CollectedCityServiceBudgetData>());
		components.Add(ComponentType.ReadWrite<CollectedCityServiceUpkeepData>());
		if (m_CityResources != null && m_CityResources.Length != 0)
		{
			components.Add(ComponentType.ReadWrite<CollectedCityServiceFeeData>());
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if (m_CityResources != null && m_CityResources.Length != 0)
		{
			DynamicBuffer<CollectedCityServiceFeeData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<CollectedCityServiceFeeData>(entity, false);
			for (int i = 0; i < m_CityResources.Length; i++)
			{
				buffer.Add(new CollectedCityServiceFeeData
				{
					m_PlayerResource = (int)m_CityResources[i]
				});
			}
		}
		((EntityManager)(ref entityManager)).SetComponentData<ServiceData>(entity, new ServiceData
		{
			m_Service = m_Service,
			m_BudgetAdjustable = m_BudgetAdjustable
		});
	}
}

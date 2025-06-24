using System;
using System.Collections.Generic;
using Game.Citizens;
using Game.City;
using Game.Economy;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class CityServiceBuilding : ComponentBase, IServiceUpgrade
{
	public ServiceUpkeepItem[] m_Upkeeps;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if ((m_Upkeeps != null && m_Upkeeps.Length != 0) || (base.prefab.TryGet<ServiceConsumption>(out var component) && component.m_Upkeep > 0))
		{
			components.Add(ComponentType.ReadWrite<ServiceUpkeepData>());
		}
		components.Add(ComponentType.ReadWrite<CollectedServiceBuildingBudgetData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			components.Add(ComponentType.ReadWrite<CityServiceUpkeep>());
			components.Add(ComponentType.ReadWrite<Resources>());
			components.Add(ComponentType.ReadWrite<TripNeeded>());
			components.Add(ComponentType.ReadWrite<GuestVehicle>());
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CityServiceUpkeep>());
		components.Add(ComponentType.ReadWrite<Resources>());
		components.Add(ComponentType.ReadWrite<TripNeeded>());
		components.Add(ComponentType.ReadWrite<GuestVehicle>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		DynamicBuffer<ServiceUpkeepData> val = ((EntityManager)(ref entityManager)).AddBuffer<ServiceUpkeepData>(entity);
		if (m_Upkeeps != null)
		{
			ServiceUpkeepItem[] upkeeps = m_Upkeeps;
			foreach (ServiceUpkeepItem serviceUpkeepItem in upkeeps)
			{
				val.Add(new ServiceUpkeepData
				{
					m_Upkeep = new ResourceStack
					{
						m_Resource = EconomyUtils.GetResource(serviceUpkeepItem.m_Resources.m_Resource),
						m_Amount = serviceUpkeepItem.m_Resources.m_Amount
					},
					m_ScaleWithUsage = serviceUpkeepItem.m_ScaleWithUsage
				});
			}
		}
	}
}

using System;
using System.Collections.Generic;
using Game.Buildings;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class PowerPlant : ComponentBase, IServiceUpgrade
{
	public int m_ElectricityProduction;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PowerPlantData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			if ((Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<Efficiency>());
			}
			components.Add(ComponentType.ReadWrite<ElectricityProducer>());
			components.Add(ComponentType.ReadWrite<ServiceUsage>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ElectricityProducer>());
		components.Add(ComponentType.ReadWrite<Efficiency>());
		components.Add(ComponentType.ReadWrite<ServiceUsage>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<PowerPlantData>(entity, new PowerPlantData
		{
			m_ElectricityProduction = m_ElectricityProduction
		});
	}
}

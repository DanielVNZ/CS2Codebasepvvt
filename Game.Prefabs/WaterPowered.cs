using System;
using System.Collections.Generic;
using Game.Buildings;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[RequireComponent(typeof(PowerPlant))]
[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class WaterPowered : ComponentBase, IServiceUpgrade
{
	public float m_ProductionFactor;

	public float m_CapacityFactor;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<WaterPoweredData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			components.Add(ComponentType.ReadWrite<Game.Buildings.WaterPowered>());
			components.Add(ComponentType.ReadWrite<Efficiency>());
			components.Add(ComponentType.ReadWrite<RenewableElectricityProduction>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.WaterPowered>());
		components.Add(ComponentType.ReadWrite<Efficiency>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (base.prefab.TryGet<PowerPlant>(out var component) && component.m_ElectricityProduction != 0)
		{
			Debug.LogErrorFormat((Object)(object)base.prefab, "WaterPowered has non-zero electricity production: {0}", new object[1] { ((Object)base.prefab).name });
		}
		((EntityManager)(ref entityManager)).SetComponentData<WaterPoweredData>(entity, new WaterPoweredData
		{
			m_ProductionFactor = m_ProductionFactor,
			m_CapacityFactor = m_CapacityFactor
		});
	}
}

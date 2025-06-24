using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[RequireComponent(typeof(GarbageFacility), typeof(PowerPlant))]
[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class GarbagePowered : ComponentBase
{
	public float m_ProductionPerUnit;

	public int m_Capacity;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<GarbagePoweredData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<GarbagePoweredData>(entity, new GarbagePoweredData
		{
			m_ProductionPerUnit = m_ProductionPerUnit,
			m_Capacity = m_Capacity
		});
	}
}

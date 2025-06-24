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
public class ParkingFacility : ComponentBase, IServiceUpgrade
{
	public float m_ComfortFactor = 0.5f;

	public int m_GarageMarkerCapacity;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ParkingFacilityData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.ParkingFacility>());
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null && (Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
		{
			components.Add(ComponentType.ReadWrite<Efficiency>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.ParkingFacility>());
		components.Add(ComponentType.ReadWrite<Efficiency>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<ParkingFacilityData>(entity, new ParkingFacilityData
		{
			m_ComfortFactor = m_ComfortFactor,
			m_GarageMarkerCapacity = m_GarageMarkerCapacity
		});
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(12));
	}
}

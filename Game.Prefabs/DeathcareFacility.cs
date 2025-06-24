using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Buildings;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab),
	typeof(MarkerObjectPrefab)
})]
public class DeathcareFacility : ComponentBase, IServiceUpgrade
{
	public int m_HearseCapacity = 5;

	public int m_StorageCapacity = 100;

	public float m_ProcessingRate = 10f;

	public bool m_LongTermStorage;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<DeathcareFacilityData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.DeathcareFacility>());
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			if ((Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<Efficiency>());
			}
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
			components.Add(ComponentType.ReadWrite<ServiceDistrict>());
			if (m_StorageCapacity != 0)
			{
				components.Add(ComponentType.ReadWrite<Patient>());
			}
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.DeathcareFacility>());
		components.Add(ComponentType.ReadWrite<ServiceDispatch>());
		components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		if (m_StorageCapacity != 0)
		{
			components.Add(ComponentType.ReadWrite<Patient>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<DeathcareFacilityData>(entity, new DeathcareFacilityData
		{
			m_HearseCapacity = m_HearseCapacity,
			m_StorageCapacity = m_StorageCapacity,
			m_LongTermStorage = m_LongTermStorage,
			m_ProcessingRate = m_ProcessingRate
		});
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(2));
	}
}

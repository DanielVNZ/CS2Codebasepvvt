using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Buildings;
using Game.Economy;
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
public class GarbageFacility : ComponentBase, IServiceUpgrade
{
	public int m_GarbageCapacity = 100000;

	public int m_VehicleCapacity = 10;

	public int m_TransportCapacity;

	public int m_ProcessingSpeed;

	public bool m_IndustrialWasteOnly;

	public bool m_LongTermStorage;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<GarbageFacilityData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.GarbageFacility>());
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			components.Add(ComponentType.ReadWrite<Resources>());
			if ((Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<GuestVehicle>());
				components.Add(ComponentType.ReadWrite<Efficiency>());
			}
			if (m_VehicleCapacity > 0 || m_TransportCapacity > 0)
			{
				components.Add(ComponentType.ReadWrite<ServiceDispatch>());
				components.Add(ComponentType.ReadWrite<OwnedVehicle>());
			}
			if ((Object)(object)GetComponent<UniqueObject>() == (Object)null)
			{
				components.Add(ComponentType.ReadWrite<ServiceDistrict>());
			}
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.GarbageFacility>());
		components.Add(ComponentType.ReadWrite<Resources>());
		if (m_VehicleCapacity > 0 || m_TransportCapacity > 0)
		{
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		GarbageFacilityData garbageFacilityData = default(GarbageFacilityData);
		garbageFacilityData.m_GarbageCapacity = m_GarbageCapacity;
		garbageFacilityData.m_VehicleCapacity = m_VehicleCapacity;
		garbageFacilityData.m_TransportCapacity = m_TransportCapacity;
		garbageFacilityData.m_ProcessingSpeed = m_ProcessingSpeed;
		garbageFacilityData.m_IndustrialWasteOnly = m_IndustrialWasteOnly;
		garbageFacilityData.m_LongTermStorage = m_LongTermStorage;
		((EntityManager)(ref entityManager)).SetComponentData<GarbageFacilityData>(entity, garbageFacilityData);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(5));
	}
}

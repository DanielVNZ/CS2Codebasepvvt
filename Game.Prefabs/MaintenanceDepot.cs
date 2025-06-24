using System;
using System.Collections.Generic;
using Game.Buildings;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class MaintenanceDepot : ComponentBase, IServiceUpgrade
{
	public MaintenanceType m_MaintenanceType = MaintenanceType.Park;

	public int m_VehicleCapacity = 10;

	public float m_VehicleEfficiency = 1f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<MaintenanceDepotData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.MaintenanceDepot>());
		if ((m_MaintenanceType & MaintenanceType.Park) != MaintenanceType.None)
		{
			components.Add(ComponentType.ReadWrite<ParkMaintenance>());
		}
		if ((m_MaintenanceType & (MaintenanceType.Road | MaintenanceType.Snow | MaintenanceType.Vehicle)) != MaintenanceType.None)
		{
			components.Add(ComponentType.ReadWrite<RoadMaintenance>());
		}
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			if ((Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<Efficiency>());
			}
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.MaintenanceDepot>());
		components.Add(ComponentType.ReadWrite<ServiceDispatch>());
		components.Add(ComponentType.ReadWrite<OwnedVehicle>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		MaintenanceDepotData maintenanceDepotData = default(MaintenanceDepotData);
		maintenanceDepotData.m_MaintenanceType = m_MaintenanceType;
		maintenanceDepotData.m_VehicleCapacity = m_VehicleCapacity;
		maintenanceDepotData.m_VehicleEfficiency = m_VehicleEfficiency;
		((EntityManager)(ref entityManager)).SetComponentData<MaintenanceDepotData>(entity, maintenanceDepotData);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(10));
	}
}

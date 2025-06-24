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
public class TransportDepot : ComponentBase, IServiceUpgrade
{
	public TransportType m_TransportType;

	public EnergyTypes m_EnergyTypes = EnergyTypes.Fuel;

	public SizeClass m_SizeClass = SizeClass.Undefined;

	public int m_VehicleCapacity = 10;

	public float m_ProductionDuration;

	public float m_MaintenanceDuration;

	public bool m_DispatchCenter;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TransportDepotData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.TransportDepot>());
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			if ((Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<Efficiency>());
			}
			if (m_TransportType == TransportType.Taxi)
			{
				components.Add(ComponentType.ReadWrite<ServiceDistrict>());
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
		components.Add(ComponentType.ReadWrite<Game.Buildings.TransportDepot>());
		components.Add(ComponentType.ReadWrite<ServiceDispatch>());
		components.Add(ComponentType.ReadWrite<OwnedVehicle>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		TransportDepotData transportDepotData = default(TransportDepotData);
		transportDepotData.m_TransportType = m_TransportType;
		transportDepotData.m_DispatchCenter = m_DispatchCenter;
		transportDepotData.m_EnergyTypes = m_EnergyTypes;
		transportDepotData.m_SizeClass = m_SizeClass;
		transportDepotData.m_VehicleCapacity = m_VehicleCapacity;
		transportDepotData.m_ProductionDuration = m_ProductionDuration;
		transportDepotData.m_MaintenanceDuration = m_MaintenanceDuration;
		if (m_SizeClass == SizeClass.Undefined)
		{
			transportDepotData.m_SizeClass = ((m_TransportType != TransportType.Taxi) ? SizeClass.Large : SizeClass.Small);
		}
		((EntityManager)(ref entityManager)).SetComponentData<TransportDepotData>(entity, transportDepotData);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(2));
	}
}

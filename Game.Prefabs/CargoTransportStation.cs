using System;
using System.Collections.Generic;
using Game.Buildings;
using Game.Companies;
using Game.Economy;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
[RequireComponent(typeof(StorageLimit))]
public class CargoTransportStation : ComponentBase, IServiceUpgrade
{
	public ResourceInEditor[] m_TradedResources;

	public int transports;

	public EnergyTypes m_CarRefuelTypes;

	public EnergyTypes m_TrainRefuelTypes;

	public EnergyTypes m_WatercraftRefuelTypes;

	public EnergyTypes m_AircraftRefuelTypes;

	public float m_LoadingFactor;

	public float m_WorkMultiplier;

	public int2 m_TransportInterval;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TransportStationData>());
		components.Add(ComponentType.ReadWrite<CargoTransportStationData>());
		components.Add(ComponentType.ReadWrite<StorageCompanyData>());
		components.Add(ComponentType.ReadWrite<TransportCompanyData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.TransportStation>());
		components.Add(ComponentType.ReadWrite<Game.Buildings.CargoTransportStation>());
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			if ((Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<Efficiency>());
			}
			components.Add(ComponentType.ReadWrite<Game.Companies.StorageCompany>());
			components.Add(ComponentType.ReadWrite<TradeCost>());
			components.Add(ComponentType.ReadWrite<StorageTransferRequest>());
			components.Add(ComponentType.ReadWrite<Resources>());
			if (transports > 0)
			{
				components.Add(ComponentType.ReadWrite<TransportCompany>());
				components.Add(ComponentType.ReadWrite<OwnedVehicle>());
			}
		}
		if (m_WorkMultiplier > 0f)
		{
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.TransportStation>());
		components.Add(ComponentType.ReadWrite<Game.Buildings.CargoTransportStation>());
		components.Add(ComponentType.ReadWrite<Game.Companies.StorageCompany>());
		components.Add(ComponentType.ReadWrite<TradeCost>());
		components.Add(ComponentType.ReadWrite<StorageTransferRequest>());
		components.Add(ComponentType.ReadWrite<Resources>());
		if (transports > 0)
		{
			components.Add(ComponentType.ReadWrite<TransportCompany>());
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		StorageCompanyData storageCompanyData = new StorageCompanyData
		{
			m_StoredResources = Resource.NoResource
		};
		if (m_TradedResources != null && m_TradedResources.Length != 0)
		{
			for (int i = 0; i < m_TradedResources.Length; i++)
			{
				storageCompanyData.m_StoredResources |= EconomyUtils.GetResource(m_TradedResources[i]);
				storageCompanyData.m_TransportInterval = m_TransportInterval;
			}
		}
		((EntityManager)(ref entityManager)).SetComponentData<StorageCompanyData>(entity, storageCompanyData);
		if (transports > 0)
		{
			((EntityManager)(ref entityManager)).SetComponentData<TransportCompanyData>(entity, new TransportCompanyData
			{
				m_MaxTransports = transports
			});
		}
		TransportStationData componentData = ((EntityManager)(ref entityManager)).GetComponentData<TransportStationData>(entity);
		componentData.m_CarRefuelTypes |= m_CarRefuelTypes;
		componentData.m_TrainRefuelTypes |= m_TrainRefuelTypes;
		componentData.m_WatercraftRefuelTypes |= m_WatercraftRefuelTypes;
		componentData.m_AircraftRefuelTypes |= m_AircraftRefuelTypes;
		componentData.m_LoadingFactor = m_LoadingFactor;
		((EntityManager)(ref entityManager)).SetComponentData<TransportStationData>(entity, componentData);
		CargoTransportStationData cargoTransportStationData = default(CargoTransportStationData);
		cargoTransportStationData.m_WorkMultiplier = m_WorkMultiplier;
		((EntityManager)(ref entityManager)).SetComponentData<CargoTransportStationData>(entity, cargoTransportStationData);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(0));
	}
}

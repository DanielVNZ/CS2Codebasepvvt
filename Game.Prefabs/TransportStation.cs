using System;
using System.Collections.Generic;
using Game.Buildings;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class TransportStation : ComponentBase, IServiceUpgrade
{
	public EnergyTypes m_CarRefuelTypes;

	public EnergyTypes m_TrainRefuelTypes;

	public EnergyTypes m_WatercraftRefuelTypes;

	public EnergyTypes m_AircraftRefuelTypes;

	public float m_ComfortFactor;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TransportStationData>());
		components.Add(ComponentType.ReadWrite<PublicTransportStationData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.TransportStation>());
		components.Add(ComponentType.ReadWrite<PublicTransportStation>());
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null && (Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
		{
			components.Add(ComponentType.ReadWrite<Efficiency>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.TransportStation>());
		components.Add(ComponentType.ReadWrite<PublicTransportStation>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		TransportStationData componentData = ((EntityManager)(ref entityManager)).GetComponentData<TransportStationData>(entity);
		componentData.m_CarRefuelTypes |= m_CarRefuelTypes;
		componentData.m_TrainRefuelTypes |= m_TrainRefuelTypes;
		componentData.m_WatercraftRefuelTypes |= m_WatercraftRefuelTypes;
		componentData.m_AircraftRefuelTypes |= m_AircraftRefuelTypes;
		componentData.m_ComfortFactor = m_ComfortFactor;
		((EntityManager)(ref entityManager)).SetComponentData<TransportStationData>(entity, componentData);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(0));
	}
}

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
public class PoliceStation : ComponentBase, IServiceUpgrade
{
	public int m_PatrolCarCapacity = 10;

	public int m_PoliceHelicopterCapacity;

	public int m_JailCapacity = 15;

	[EnumFlag]
	public PolicePurpose m_Purposes = PolicePurpose.Patrol | PolicePurpose.Emergency;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PoliceStationData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.PoliceStation>());
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			if ((Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<Efficiency>());
			}
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
			if ((Object)(object)GetComponent<UniqueObject>() == (Object)null)
			{
				components.Add(ComponentType.ReadWrite<ServiceDistrict>());
			}
			if (m_JailCapacity != 0)
			{
				components.Add(ComponentType.ReadWrite<Occupant>());
			}
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.PoliceStation>());
		components.Add(ComponentType.ReadWrite<ServiceDispatch>());
		components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		if (m_JailCapacity != 0)
		{
			components.Add(ComponentType.ReadWrite<Occupant>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		PoliceStationData policeStationData = default(PoliceStationData);
		policeStationData.m_PatrolCarCapacity = m_PatrolCarCapacity;
		policeStationData.m_PoliceHelicopterCapacity = m_PoliceHelicopterCapacity;
		policeStationData.m_JailCapacity = m_JailCapacity;
		policeStationData.m_PurposeMask = m_Purposes;
		((EntityManager)(ref entityManager)).SetComponentData<PoliceStationData>(entity, policeStationData);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(8));
	}
}

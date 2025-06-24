using System;
using System.Collections.Generic;
using Game.Buildings;
using Game.Net;
using Game.Simulation;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab),
	typeof(MarkerObjectPrefab)
})]
public class TrafficSpawner : ComponentBase
{
	public RoadTypes m_RoadType = RoadTypes.Car;

	public TrackTypes m_TrackType;

	public float m_SpawnRate = 0.5f;

	public bool m_NoSlowVehicles;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TrafficSpawnerData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.TrafficSpawner>());
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<TrafficSpawnerData>(entity, new TrafficSpawnerData
		{
			m_SpawnRate = m_SpawnRate,
			m_RoadType = m_RoadType,
			m_TrackType = m_TrackType,
			m_NoSlowVehicles = m_NoSlowVehicles
		});
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(2));
	}
}

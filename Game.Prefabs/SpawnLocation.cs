using System;
using System.Collections.Generic;
using Game.Net;
using Game.Objects;
using Game.Routes;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(MarkerObjectPrefab) })]
public class SpawnLocation : ComponentBase
{
	public RouteConnectionType m_ConnectionType = RouteConnectionType.Pedestrian;

	public TrackTypes m_TrackTypes;

	public RoadTypes m_RoadTypes;

	public bool m_RequireAuthorization;

	public bool m_HangaroundOnLane;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<SpawnLocationData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Objects.SpawnLocation>());
		if (m_ConnectionType == RouteConnectionType.Air)
		{
			components.Add(ComponentType.ReadWrite<Game.Routes.TakeoffLocation>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		SpawnLocationData spawnLocationData = default(SpawnLocationData);
		spawnLocationData.m_ConnectionType = m_ConnectionType;
		spawnLocationData.m_ActivityMask = default(ActivityMask);
		spawnLocationData.m_TrackTypes = m_TrackTypes;
		spawnLocationData.m_RoadTypes = m_RoadTypes;
		spawnLocationData.m_RequireAuthorization = m_RequireAuthorization;
		spawnLocationData.m_HangaroundOnLane = m_HangaroundOnLane;
		((EntityManager)(ref entityManager)).SetComponentData<SpawnLocationData>(entity, spawnLocationData);
	}
}

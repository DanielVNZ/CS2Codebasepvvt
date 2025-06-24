using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Areas/", new Type[]
{
	typeof(LotPrefab),
	typeof(SpacePrefab)
})]
public class NavigationArea : ComponentBase
{
	public RouteConnectionType m_ConnectionType = RouteConnectionType.Pedestrian;

	public RouteConnectionType m_SecondaryType;

	public TrackTypes m_TrackTypes;

	public RoadTypes m_RoadTypes;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<NavigationAreaData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Navigation>());
		components.Add(ComponentType.ReadWrite<Game.Net.SubLane>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		NavigationAreaData navigationAreaData = default(NavigationAreaData);
		navigationAreaData.m_ConnectionType = m_ConnectionType;
		navigationAreaData.m_SecondaryType = m_SecondaryType;
		navigationAreaData.m_TrackTypes = m_TrackTypes;
		navigationAreaData.m_RoadTypes = m_RoadTypes;
		((EntityManager)(ref entityManager)).SetComponentData<NavigationAreaData>(entity, navigationAreaData);
	}
}

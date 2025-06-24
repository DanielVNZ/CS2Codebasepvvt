using System;
using System.Collections.Generic;
using Game.Net;
using Game.Pathfind;
using Game.Routes;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Routes/", new Type[] { })]
public class WorkRoutePrefab : RoutePrefab
{
	public RoadTypes m_RouteRoadType = RoadTypes.Car;

	public SizeClass m_SizeClass = SizeClass.Large;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<RouteConnectionData>());
		components.Add(ComponentType.ReadWrite<WorkRouteData>());
		components.Add(ComponentType.ReadWrite<PlaceableInfoviewItem>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		if (components.Contains(ComponentType.ReadWrite<Route>()))
		{
			components.Add(ComponentType.ReadWrite<WorkRoute>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Waypoint>()))
		{
			components.Add(ComponentType.ReadWrite<AccessLane>());
			components.Add(ComponentType.ReadWrite<RouteLane>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Game.Routes.Segment>()))
		{
			components.Add(ComponentType.ReadWrite<PathTargets>());
			components.Add(ComponentType.ReadWrite<RouteInfo>());
			components.Add(ComponentType.ReadWrite<PathElement>());
			components.Add(ComponentType.ReadWrite<PathInformation>());
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<RouteConnectionData>(entity, new RouteConnectionData
		{
			m_AccessConnectionType = RouteConnectionType.Pedestrian,
			m_RouteConnectionType = RouteConnectionType.Road,
			m_AccessTrackType = TrackTypes.None,
			m_RouteTrackType = TrackTypes.None,
			m_AccessRoadType = RoadTypes.None,
			m_RouteRoadType = m_RouteRoadType,
			m_RouteSizeClass = m_SizeClass,
			m_StartLaneOffset = 0f,
			m_EndMargin = 0f
		});
		((EntityManager)(ref entityManager)).SetComponentData<WorkRouteData>(entity, new WorkRouteData
		{
			m_SizeClass = m_SizeClass
		});
	}
}

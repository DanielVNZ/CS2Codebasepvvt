using System;
using System.Collections.Generic;
using Game.Net;
using Game.Routes;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Routes/", new Type[] { typeof(MarkerObjectPrefab) })]
public class TakeoffLocation : ComponentBase
{
	public RouteConnectionType m_ConnectionType1 = RouteConnectionType.Road;

	public RouteConnectionType m_ConnectionType2 = RouteConnectionType.Air;

	public RoadTypes m_RoadType;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<RouteConnectionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Routes.TakeoffLocation>());
		components.Add(ComponentType.ReadWrite<AccessLane>());
		components.Add(ComponentType.ReadWrite<RouteLane>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		RouteConnectionData routeConnectionData = default(RouteConnectionData);
		routeConnectionData.m_AccessConnectionType = m_ConnectionType1;
		routeConnectionData.m_RouteConnectionType = m_ConnectionType2;
		routeConnectionData.m_AccessTrackType = TrackTypes.None;
		routeConnectionData.m_RouteTrackType = TrackTypes.None;
		routeConnectionData.m_AccessRoadType = m_RoadType;
		routeConnectionData.m_RouteRoadType = m_RoadType;
		routeConnectionData.m_RouteSizeClass = SizeClass.Undefined;
		routeConnectionData.m_StartLaneOffset = 0f;
		routeConnectionData.m_EndMargin = 0f;
		((EntityManager)(ref entityManager)).SetComponentData<RouteConnectionData>(entity, routeConnectionData);
	}
}

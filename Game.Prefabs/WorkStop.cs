using System;
using System.Collections.Generic;
using Game.Net;
using Game.Objects;
using Game.Routes;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Routes/", new Type[] { typeof(ObjectPrefab) })]
public class WorkStop : ComponentBase
{
	public RoadTypes m_RouteRoadType = RoadTypes.Car;

	public bool m_WorkLocation;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<WorkStopData>());
		components.Add(ComponentType.ReadWrite<TransportStopData>());
		components.Add(ComponentType.ReadWrite<RouteConnectionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Objects.Color>());
		components.Add(ComponentType.ReadWrite<Game.Routes.TransportStop>());
		components.Add(ComponentType.ReadWrite<Game.Routes.WorkStop>());
		components.Add(ComponentType.ReadWrite<ConnectedRoute>());
		components.Add(ComponentType.ReadWrite<BoardingVehicle>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		RouteConnectionData routeConnectionData = default(RouteConnectionData);
		routeConnectionData.m_AccessConnectionType = RouteConnectionType.Pedestrian;
		routeConnectionData.m_RouteConnectionType = RouteConnectionType.Road;
		routeConnectionData.m_AccessTrackType = TrackTypes.None;
		routeConnectionData.m_RouteTrackType = TrackTypes.None;
		routeConnectionData.m_AccessRoadType = RoadTypes.None;
		routeConnectionData.m_RouteRoadType = m_RouteRoadType;
		routeConnectionData.m_RouteSizeClass = SizeClass.Undefined;
		routeConnectionData.m_StartLaneOffset = 0f;
		routeConnectionData.m_EndMargin = 0f;
		TransportStopData transportStopData = default(TransportStopData);
		transportStopData.m_ComfortFactor = 0f;
		transportStopData.m_LoadingFactor = 0f;
		transportStopData.m_AccessDistance = 0f;
		transportStopData.m_BoardingTime = 0f;
		transportStopData.m_TransportType = TransportType.Work;
		transportStopData.m_PassengerTransport = false;
		transportStopData.m_CargoTransport = true;
		WorkStopData workStopData = default(WorkStopData);
		workStopData.m_WorkLocation = m_WorkLocation;
		((EntityManager)(ref entityManager)).SetComponentData<RouteConnectionData>(entity, routeConnectionData);
		((EntityManager)(ref entityManager)).SetComponentData<TransportStopData>(entity, transportStopData);
		((EntityManager)(ref entityManager)).SetComponentData<WorkStopData>(entity, workStopData);
	}
}

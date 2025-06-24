using System;
using System.Collections.Generic;
using Game.Net;
using Game.Objects;
using Game.Routes;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Routes/", new Type[] { typeof(ObjectPrefab) })]
public class TransportStop : ComponentBase
{
	public TransportType m_TransportType;

	public RouteConnectionType m_AccessConnectionType = RouteConnectionType.Pedestrian;

	public RouteConnectionType m_RouteConnectionType = RouteConnectionType.Road;

	public TrackTypes m_AccessTrackType;

	public TrackTypes m_RouteTrackType;

	public RoadTypes m_AccessRoadType;

	public RoadTypes m_RouteRoadType;

	public float m_EnterDistance;

	public float m_ExitDistance;

	public float m_AccessDistance;

	public float m_BoardingTime;

	public float m_ComfortFactor;

	public float m_LoadingFactor;

	public bool m_PassengerTransport = true;

	public bool m_CargoTransport;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TransportStopData>());
		components.Add(ComponentType.ReadWrite<RouteConnectionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Routes.TransportStop>());
		components.Add(ComponentType.ReadWrite<Game.Objects.Color>());
		switch (m_TransportType)
		{
		case TransportType.Bus:
			components.Add(ComponentType.ReadWrite<ConnectedRoute>());
			components.Add(ComponentType.ReadWrite<BoardingVehicle>());
			components.Add(ComponentType.ReadWrite<BusStop>());
			break;
		case TransportType.Train:
			components.Add(ComponentType.ReadWrite<ConnectedRoute>());
			components.Add(ComponentType.ReadWrite<BoardingVehicle>());
			components.Add(ComponentType.ReadWrite<TrainStop>());
			break;
		case TransportType.Taxi:
			components.Add(ComponentType.ReadWrite<BoardingVehicle>());
			components.Add(ComponentType.ReadWrite<RouteVehicle>());
			components.Add(ComponentType.ReadWrite<TaxiStand>());
			components.Add(ComponentType.ReadWrite<DispatchedRequest>());
			if (m_AccessConnectionType != RouteConnectionType.None)
			{
				components.Add(ComponentType.ReadWrite<AccessLane>());
			}
			if (m_RouteConnectionType != RouteConnectionType.None)
			{
				components.Add(ComponentType.ReadWrite<RouteLane>());
			}
			if (m_PassengerTransport)
			{
				components.Add(ComponentType.ReadWrite<WaitingPassengers>());
			}
			break;
		case TransportType.Tram:
			components.Add(ComponentType.ReadWrite<ConnectedRoute>());
			components.Add(ComponentType.ReadWrite<BoardingVehicle>());
			components.Add(ComponentType.ReadWrite<TramStop>());
			break;
		case TransportType.Ship:
			components.Add(ComponentType.ReadWrite<ConnectedRoute>());
			components.Add(ComponentType.ReadWrite<BoardingVehicle>());
			components.Add(ComponentType.ReadWrite<ShipStop>());
			break;
		case TransportType.Helicopter:
		case TransportType.Rocket:
			components.Add(ComponentType.ReadWrite<BoardingVehicle>());
			components.Add(ComponentType.ReadWrite<ConnectedRoute>());
			break;
		case TransportType.Airplane:
			components.Add(ComponentType.ReadWrite<BoardingVehicle>());
			components.Add(ComponentType.ReadWrite<ConnectedRoute>());
			components.Add(ComponentType.ReadWrite<AirplaneStop>());
			if ((Object)(object)GetComponent<OutsideConnection>() != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<Game.Net.SubLane>());
			}
			break;
		case TransportType.Subway:
			components.Add(ComponentType.ReadWrite<ConnectedRoute>());
			components.Add(ComponentType.ReadWrite<BoardingVehicle>());
			components.Add(ComponentType.ReadWrite<SubwayStop>());
			break;
		case TransportType.Post:
			break;
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		RouteConnectionData routeConnectionData = default(RouteConnectionData);
		routeConnectionData.m_AccessConnectionType = m_AccessConnectionType;
		routeConnectionData.m_RouteConnectionType = m_RouteConnectionType;
		routeConnectionData.m_AccessTrackType = m_AccessTrackType;
		routeConnectionData.m_RouteTrackType = m_RouteTrackType;
		routeConnectionData.m_AccessRoadType = m_AccessRoadType;
		routeConnectionData.m_RouteRoadType = m_RouteRoadType;
		routeConnectionData.m_RouteSizeClass = SizeClass.Undefined;
		routeConnectionData.m_StartLaneOffset = m_EnterDistance;
		routeConnectionData.m_EndMargin = m_ExitDistance;
		TransportStopData transportStopData = new TransportStopData
		{
			m_ComfortFactor = m_ComfortFactor,
			m_LoadingFactor = m_LoadingFactor,
			m_AccessDistance = m_AccessDistance,
			m_BoardingTime = m_BoardingTime,
			m_TransportType = m_TransportType,
			m_PassengerTransport = m_PassengerTransport,
			m_CargoTransport = m_CargoTransport
		};
		((EntityManager)(ref entityManager)).SetComponentData<RouteConnectionData>(entity, routeConnectionData);
		((EntityManager)(ref entityManager)).SetComponentData<TransportStopData>(entity, transportStopData);
	}
}

using System;
using System.Collections.Generic;
using Game.Net;
using Game.Pathfind;
using Game.Policies;
using Game.Routes;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Routes/", new Type[] { })]
public class TransportLinePrefab : RoutePrefab
{
	public RouteConnectionType m_AccessConnectionType = RouteConnectionType.Pedestrian;

	public RouteConnectionType m_RouteConnectionType = RouteConnectionType.Road;

	public TrackTypes m_AccessTrackType;

	public TrackTypes m_RouteTrackType;

	public RoadTypes m_AccessRoadType;

	public RoadTypes m_RouteRoadType;

	public TransportType m_TransportType;

	public float m_DefaultVehicleInterval = 15f;

	public float m_DefaultUnbunchingFactor = 0.75f;

	public float m_StopDuration = 1f;

	public SizeClass m_SizeClass = SizeClass.Large;

	public bool m_PassengerTransport = true;

	public bool m_CargoTransport;

	public PathfindPrefab m_PathfindPrefab;

	public NotificationIconPrefab m_VehicleNotification;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_PathfindPrefab);
		prefabs.Add(m_VehicleNotification);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<RouteConnectionData>());
		components.Add(ComponentType.ReadWrite<TransportLineData>());
		components.Add(ComponentType.ReadWrite<PlaceableInfoviewItem>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		if (components.Contains(ComponentType.ReadWrite<Route>()))
		{
			components.Add(ComponentType.ReadWrite<TransportLine>());
			components.Add(ComponentType.ReadWrite<VehicleModel>());
			components.Add(ComponentType.ReadWrite<DispatchedRequest>());
			components.Add(ComponentType.ReadWrite<RouteNumber>());
			components.Add(ComponentType.ReadWrite<RouteVehicle>());
			components.Add(ComponentType.ReadWrite<RouteModifier>());
			components.Add(ComponentType.ReadWrite<Policy>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Waypoint>()))
		{
			if (m_AccessConnectionType != RouteConnectionType.None)
			{
				components.Add(ComponentType.ReadWrite<AccessLane>());
			}
			if (m_RouteConnectionType != RouteConnectionType.None)
			{
				components.Add(ComponentType.ReadWrite<RouteLane>());
			}
			if (components.Contains(ComponentType.ReadWrite<Connected>()))
			{
				components.Add(ComponentType.ReadWrite<VehicleTiming>());
			}
			if (m_PassengerTransport)
			{
				components.Add(ComponentType.ReadWrite<WaitingPassengers>());
			}
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<RouteConnectionData>(entity, new RouteConnectionData
		{
			m_AccessConnectionType = m_AccessConnectionType,
			m_RouteConnectionType = m_RouteConnectionType,
			m_AccessTrackType = m_AccessTrackType,
			m_RouteTrackType = m_RouteTrackType,
			m_AccessRoadType = m_AccessRoadType,
			m_RouteRoadType = m_RouteRoadType,
			m_RouteSizeClass = m_SizeClass,
			m_StartLaneOffset = 0f,
			m_EndMargin = 0f
		});
		((EntityManager)(ref entityManager)).SetComponentData<TransportLineData>(entity, new TransportLineData
		{
			m_PathfindPrefab = existingSystemManaged.GetEntity(m_PathfindPrefab),
			m_TransportType = m_TransportType,
			m_DefaultVehicleInterval = m_DefaultVehicleInterval,
			m_DefaultUnbunchingFactor = m_DefaultUnbunchingFactor,
			m_StopDuration = m_StopDuration,
			m_SizeClass = m_SizeClass,
			m_PassengerTransport = m_PassengerTransport,
			m_CargoTransport = m_CargoTransport,
			m_VehicleNotification = existingSystemManaged.GetEntity(m_VehicleNotification)
		});
	}
}

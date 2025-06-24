using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;

namespace Game.Pathfind;

public struct PathfindTargetSeekerData
{
	[ReadOnly]
	public AirwayHelpers.AirwayData m_AirwayData;

	[ReadOnly]
	public ComponentLookup<Owner> m_Owner;

	[ReadOnly]
	public ComponentLookup<Transform> m_Transform;

	[ReadOnly]
	public ComponentLookup<Attached> m_Attached;

	[ReadOnly]
	public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocation;

	[ReadOnly]
	public ComponentLookup<Stopped> m_Stopped;

	[ReadOnly]
	public ComponentLookup<HumanCurrentLane> m_HumanCurrentLane;

	[ReadOnly]
	public ComponentLookup<CarCurrentLane> m_CarCurrentLane;

	[ReadOnly]
	public ComponentLookup<TrainCurrentLane> m_TrainCurrentLane;

	[ReadOnly]
	public ComponentLookup<WatercraftCurrentLane> m_WatercraftCurrentLane;

	[ReadOnly]
	public ComponentLookup<AircraftCurrentLane> m_AircraftCurrentLane;

	[ReadOnly]
	public ComponentLookup<ParkedCar> m_ParkedCar;

	[ReadOnly]
	public ComponentLookup<ParkedTrain> m_ParkedTrain;

	[ReadOnly]
	public ComponentLookup<Train> m_Train;

	[ReadOnly]
	public ComponentLookup<Airplane> m_Airplane;

	[ReadOnly]
	public ComponentLookup<Building> m_Building;

	[ReadOnly]
	public ComponentLookup<PropertyRenter> m_PropertyRenter;

	[ReadOnly]
	public ComponentLookup<CurrentBuilding> m_CurrentBuilding;

	[ReadOnly]
	public ComponentLookup<CurrentTransport> m_CurrentTransport;

	[ReadOnly]
	public ComponentLookup<Curve> m_Curve;

	[ReadOnly]
	public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLane;

	[ReadOnly]
	public ComponentLookup<Game.Net.ParkingLane> m_ParkingLane;

	[ReadOnly]
	public ComponentLookup<Game.Net.CarLane> m_CarLane;

	[ReadOnly]
	public ComponentLookup<MasterLane> m_MasterLane;

	[ReadOnly]
	public ComponentLookup<SlaveLane> m_SlaveLane;

	[ReadOnly]
	public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLane;

	[ReadOnly]
	public ComponentLookup<NodeLane> m_NodeLane;

	[ReadOnly]
	public ComponentLookup<LaneConnection> m_LaneConnection;

	[ReadOnly]
	public ComponentLookup<RouteLane> m_RouteLane;

	[ReadOnly]
	public ComponentLookup<AccessLane> m_AccessLane;

	[ReadOnly]
	public ComponentLookup<PrefabRef> m_PrefabRef;

	[ReadOnly]
	public ComponentLookup<BuildingData> m_BuildingData;

	[ReadOnly]
	public ComponentLookup<PathfindCarData> m_CarPathfindData;

	[ReadOnly]
	public ComponentLookup<SpawnLocationData> m_SpawnLocationData;

	[ReadOnly]
	public ComponentLookup<NetLaneData> m_NetLaneData;

	[ReadOnly]
	public ComponentLookup<CarLaneData> m_CarLaneData;

	[ReadOnly]
	public ComponentLookup<ParkingLaneData> m_ParkingLaneData;

	[ReadOnly]
	public ComponentLookup<TrackLaneData> m_TrackLaneData;

	[ReadOnly]
	public BufferLookup<Game.Net.SubLane> m_SubLane;

	[ReadOnly]
	public BufferLookup<Game.Areas.Node> m_AreaNode;

	[ReadOnly]
	public BufferLookup<Triangle> m_AreaTriangle;

	[ReadOnly]
	public BufferLookup<SpawnLocationElement> m_SpawnLocations;

	[ReadOnly]
	public BufferLookup<LayoutElement> m_VehicleLayout;

	[ReadOnly]
	public BufferLookup<CarNavigationLane> m_CarNavigationLanes;

	[ReadOnly]
	public BufferLookup<WatercraftNavigationLane> m_WatercraftNavigationLanes;

	[ReadOnly]
	public BufferLookup<AircraftNavigationLane> m_AircraftNavigationLanes;

	public PathfindTargetSeekerData(SystemBase system)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		m_AirwayData = default(AirwayHelpers.AirwayData);
		m_Owner = system.GetComponentLookup<Owner>(true);
		m_Transform = system.GetComponentLookup<Transform>(true);
		m_Attached = system.GetComponentLookup<Attached>(true);
		m_SpawnLocation = system.GetComponentLookup<Game.Objects.SpawnLocation>(true);
		m_Stopped = system.GetComponentLookup<Stopped>(true);
		m_HumanCurrentLane = system.GetComponentLookup<HumanCurrentLane>(true);
		m_CarCurrentLane = system.GetComponentLookup<CarCurrentLane>(true);
		m_TrainCurrentLane = system.GetComponentLookup<TrainCurrentLane>(true);
		m_WatercraftCurrentLane = system.GetComponentLookup<WatercraftCurrentLane>(true);
		m_AircraftCurrentLane = system.GetComponentLookup<AircraftCurrentLane>(true);
		m_ParkedCar = system.GetComponentLookup<ParkedCar>(true);
		m_ParkedTrain = system.GetComponentLookup<ParkedTrain>(true);
		m_Train = system.GetComponentLookup<Train>(true);
		m_Airplane = system.GetComponentLookup<Airplane>(true);
		m_Building = system.GetComponentLookup<Building>(true);
		m_PropertyRenter = system.GetComponentLookup<PropertyRenter>(true);
		m_CurrentBuilding = system.GetComponentLookup<CurrentBuilding>(true);
		m_CurrentTransport = system.GetComponentLookup<CurrentTransport>(true);
		m_Curve = system.GetComponentLookup<Curve>(true);
		m_PedestrianLane = system.GetComponentLookup<Game.Net.PedestrianLane>(true);
		m_ParkingLane = system.GetComponentLookup<Game.Net.ParkingLane>(true);
		m_CarLane = system.GetComponentLookup<Game.Net.CarLane>(true);
		m_MasterLane = system.GetComponentLookup<MasterLane>(true);
		m_SlaveLane = system.GetComponentLookup<SlaveLane>(true);
		m_ConnectionLane = system.GetComponentLookup<Game.Net.ConnectionLane>(true);
		m_NodeLane = system.GetComponentLookup<NodeLane>(true);
		m_LaneConnection = system.GetComponentLookup<LaneConnection>(true);
		m_RouteLane = system.GetComponentLookup<RouteLane>(true);
		m_AccessLane = system.GetComponentLookup<AccessLane>(true);
		m_PrefabRef = system.GetComponentLookup<PrefabRef>(true);
		m_BuildingData = system.GetComponentLookup<BuildingData>(true);
		m_CarPathfindData = system.GetComponentLookup<PathfindCarData>(true);
		m_SpawnLocationData = system.GetComponentLookup<SpawnLocationData>(true);
		m_NetLaneData = system.GetComponentLookup<NetLaneData>(true);
		m_CarLaneData = system.GetComponentLookup<CarLaneData>(true);
		m_ParkingLaneData = system.GetComponentLookup<ParkingLaneData>(true);
		m_TrackLaneData = system.GetComponentLookup<TrackLaneData>(true);
		m_SubLane = system.GetBufferLookup<Game.Net.SubLane>(true);
		m_AreaNode = system.GetBufferLookup<Game.Areas.Node>(true);
		m_AreaTriangle = system.GetBufferLookup<Triangle>(true);
		m_SpawnLocations = system.GetBufferLookup<SpawnLocationElement>(true);
		m_VehicleLayout = system.GetBufferLookup<LayoutElement>(true);
		m_CarNavigationLanes = system.GetBufferLookup<CarNavigationLane>(true);
		m_WatercraftNavigationLanes = system.GetBufferLookup<WatercraftNavigationLane>(true);
		m_AircraftNavigationLanes = system.GetBufferLookup<AircraftNavigationLane>(true);
	}

	public void Update(SystemBase system, AirwayHelpers.AirwayData airwayData)
	{
		m_AirwayData = airwayData;
		m_Owner.Update(system);
		m_Transform.Update(system);
		m_Attached.Update(system);
		m_SpawnLocation.Update(system);
		m_Stopped.Update(system);
		m_HumanCurrentLane.Update(system);
		m_CarCurrentLane.Update(system);
		m_TrainCurrentLane.Update(system);
		m_WatercraftCurrentLane.Update(system);
		m_AircraftCurrentLane.Update(system);
		m_ParkedCar.Update(system);
		m_ParkedTrain.Update(system);
		m_Train.Update(system);
		m_Airplane.Update(system);
		m_Building.Update(system);
		m_PropertyRenter.Update(system);
		m_CurrentBuilding.Update(system);
		m_CurrentTransport.Update(system);
		m_Curve.Update(system);
		m_PedestrianLane.Update(system);
		m_ParkingLane.Update(system);
		m_CarLane.Update(system);
		m_MasterLane.Update(system);
		m_SlaveLane.Update(system);
		m_ConnectionLane.Update(system);
		m_NodeLane.Update(system);
		m_LaneConnection.Update(system);
		m_RouteLane.Update(system);
		m_AccessLane.Update(system);
		m_PrefabRef.Update(system);
		m_BuildingData.Update(system);
		m_CarPathfindData.Update(system);
		m_SpawnLocationData.Update(system);
		m_NetLaneData.Update(system);
		m_CarLaneData.Update(system);
		m_ParkingLaneData.Update(system);
		m_TrackLaneData.Update(system);
		m_SubLane.Update(system);
		m_AreaNode.Update(system);
		m_AreaTriangle.Update(system);
		m_SpawnLocations.Update(system);
		m_VehicleLayout.Update(system);
		m_CarNavigationLanes.Update(system);
		m_WatercraftNavigationLanes.Update(system);
		m_AircraftNavigationLanes.Update(system);
	}
}

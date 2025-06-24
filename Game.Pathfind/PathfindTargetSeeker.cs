using Colossal.Mathematics;
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
using Unity.Mathematics;
using UnityEngine;

namespace Game.Pathfind;

public struct PathfindTargetSeeker<TBuffer> where TBuffer : IPathfindTargetBuffer
{
	public PathfindParameters m_PathfindParameters;

	public bool m_IsStartTarget;

	public SetupQueueTarget m_SetupQueueTarget;

	public TBuffer m_Buffer;

	[ReadOnly]
	public RandomSeed m_RandomSeed;

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

	public PathfindTargetSeeker(PathfindTargetSeekerData data, PathfindParameters pathfindParameters, SetupQueueTarget setupQueueTarget, TBuffer buffer, RandomSeed randomSeed, bool isStartTarget)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		m_PathfindParameters = pathfindParameters;
		m_IsStartTarget = isStartTarget;
		m_SetupQueueTarget = setupQueueTarget;
		m_Buffer = buffer;
		m_RandomSeed = randomSeed;
		m_AirwayData = data.m_AirwayData;
		m_Owner = data.m_Owner;
		m_Transform = data.m_Transform;
		m_Attached = data.m_Attached;
		m_SpawnLocation = data.m_SpawnLocation;
		m_Stopped = data.m_Stopped;
		m_HumanCurrentLane = data.m_HumanCurrentLane;
		m_CarCurrentLane = data.m_CarCurrentLane;
		m_TrainCurrentLane = data.m_TrainCurrentLane;
		m_WatercraftCurrentLane = data.m_WatercraftCurrentLane;
		m_AircraftCurrentLane = data.m_AircraftCurrentLane;
		m_ParkedCar = data.m_ParkedCar;
		m_ParkedTrain = data.m_ParkedTrain;
		m_Train = data.m_Train;
		m_Airplane = data.m_Airplane;
		m_Building = data.m_Building;
		m_PropertyRenter = data.m_PropertyRenter;
		m_CurrentBuilding = data.m_CurrentBuilding;
		m_CurrentTransport = data.m_CurrentTransport;
		m_Curve = data.m_Curve;
		m_PedestrianLane = data.m_PedestrianLane;
		m_ParkingLane = data.m_ParkingLane;
		m_CarLane = data.m_CarLane;
		m_MasterLane = data.m_MasterLane;
		m_SlaveLane = data.m_SlaveLane;
		m_ConnectionLane = data.m_ConnectionLane;
		m_NodeLane = data.m_NodeLane;
		m_LaneConnection = data.m_LaneConnection;
		m_RouteLane = data.m_RouteLane;
		m_AccessLane = data.m_AccessLane;
		m_PrefabRef = data.m_PrefabRef;
		m_BuildingData = data.m_BuildingData;
		m_CarPathfindData = data.m_CarPathfindData;
		m_SpawnLocationData = data.m_SpawnLocationData;
		m_NetLaneData = data.m_NetLaneData;
		m_CarLaneData = data.m_CarLaneData;
		m_ParkingLaneData = data.m_ParkingLaneData;
		m_TrackLaneData = data.m_TrackLaneData;
		m_SubLane = data.m_SubLane;
		m_AreaNode = data.m_AreaNode;
		m_AreaTriangle = data.m_AreaTriangle;
		m_SpawnLocations = data.m_SpawnLocations;
		m_VehicleLayout = data.m_VehicleLayout;
		m_CarNavigationLanes = data.m_CarNavigationLanes;
		m_WatercraftNavigationLanes = data.m_WatercraftNavigationLanes;
		m_AircraftNavigationLanes = data.m_AircraftNavigationLanes;
	}

	public void AddTarget(ref Random random, Entity target, Entity entity, float delta, float cost, EdgeFlags flags)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		cost += ((Random)(ref random)).NextFloat(m_SetupQueueTarget.m_RandomCost);
		ref TBuffer buffer = ref m_Buffer;
		PathTarget pathTarget = new PathTarget(target, entity, delta, cost, flags);
		buffer.Enqueue(pathTarget);
	}

	public int FindTargets(Entity entity, float cost)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return FindTargets(entity, entity, cost, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: false);
	}

	public int FindTargets(Entity target, Entity entity, float cost, EdgeFlags flags, bool allowAccessRestriction, bool navigationEnd)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0910: Unknown result type (might be due to invalid IL or missing references)
		//IL_091f: Unknown result type (might be due to invalid IL or missing references)
		//IL_092a: Unknown result type (might be due to invalid IL or missing references)
		//IL_092f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0943: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_0956: Unknown result type (might be due to invalid IL or missing references)
		//IL_0960: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_098c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0991: Unknown result type (might be due to invalid IL or missing references)
		//IL_0970: Unknown result type (might be due to invalid IL or missing references)
		//IL_0975: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0997: Unknown result type (might be due to invalid IL or missing references)
		//IL_099d: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_0816: Unknown result type (might be due to invalid IL or missing references)
		Random random = m_RandomSeed.GetRandom(entity.Index);
		int num = 0;
		if ((m_PathfindParameters.m_PathfindFlags & PathfindFlags.SkipPathfind) != 0)
		{
			AddTarget(ref random, target, entity, 0f, cost, flags);
			return 1;
		}
		if (m_CurrentTransport.HasComponent(entity))
		{
			entity = m_CurrentTransport[entity].m_CurrentTransport;
		}
		if (m_HumanCurrentLane.HasComponent(entity))
		{
			HumanCurrentLane humanCurrentLane = m_HumanCurrentLane[entity];
			if (m_Curve.HasComponent(humanCurrentLane.m_Lane))
			{
				if ((m_SetupQueueTarget.m_Methods & PathMethod.Pedestrian) != 0)
				{
					return num + AddPedestrianLaneTargets(ref random, target, humanCurrentLane.m_Lane, humanCurrentLane.m_CurvePosition.y, cost, 0f, flags, allowAccessRestriction);
				}
				float3 comparePosition = MathUtils.Position(m_Curve[humanCurrentLane.m_Lane].m_Bezier, humanCurrentLane.m_CurvePosition.y);
				if (GetEdge(humanCurrentLane.m_Lane, out var edge))
				{
					return num + AddEdgeTargets(ref random, target, cost, flags, edge, comparePosition, 0f, allowLaneGroupSwitch: false, allowAccessRestriction);
				}
				entity = humanCurrentLane.m_Lane;
			}
			else if (m_SpawnLocation.HasComponent(humanCurrentLane.m_Lane))
			{
				num += AddSpawnLocation(ref random, target, humanCurrentLane.m_Lane, cost, flags, ignoreActivityMask: true, allowAccessRestriction);
				if (num != 0)
				{
					return num;
				}
				entity = humanCurrentLane.m_Lane;
			}
			else
			{
				entity = humanCurrentLane.m_Lane;
			}
		}
		if (m_CarCurrentLane.HasComponent(entity))
		{
			GetCarLane(entity, navigationEnd, out var lane, out var curvePos, out var flags2);
			if (m_Curve.HasComponent(lane))
			{
				float3 comparePosition2 = MathUtils.Position(m_Curve[lane].m_Bezier, curvePos);
				if ((m_SetupQueueTarget.m_Methods & PathMethod.Road) != 0 && (m_SetupQueueTarget.m_RoadTypes & RoadTypes.Car) != RoadTypes.None)
				{
					return num + AddCarLaneTargets(ref random, target, lane, comparePosition2, 0f, curvePos, cost, flags, (flags2 & (Game.Vehicles.CarLaneFlags.EnteringRoad | Game.Vehicles.CarLaneFlags.IsBlocked)) != 0, m_Stopped.HasComponent(entity), allowAccessRestriction);
				}
				if (GetEdge(lane, out var edge2))
				{
					return num + AddEdgeTargets(ref random, target, cost, flags, edge2, comparePosition2, 0f, allowLaneGroupSwitch: false, allowAccessRestriction);
				}
				entity = lane;
			}
			else if (m_SpawnLocation.HasComponent(lane))
			{
				num += AddSpawnLocation(ref random, target, lane, cost, flags, ignoreActivityMask: true, allowAccessRestriction);
				if (num != 0)
				{
					return num;
				}
				entity = lane;
			}
			else
			{
				entity = lane;
			}
		}
		ParkedCar parkedCar = default(ParkedCar);
		if (m_ParkedCar.TryGetComponent(entity, ref parkedCar))
		{
			Curve curve = default(Curve);
			if (m_Curve.TryGetComponent(parkedCar.m_Lane, ref curve))
			{
				if ((m_SetupQueueTarget.m_Methods & (PathMethod.Parking | PathMethod.SpecialParking)) != 0)
				{
					return num + AddParkingLaneTargets(ref random, target, parkedCar.m_Lane, parkedCar.m_CurvePosition, cost, flags, allowAccessRestriction);
				}
				float3 comparePosition3 = MathUtils.Position(curve.m_Bezier, parkedCar.m_CurvePosition);
				LaneConnection laneConnection = default(LaneConnection);
				if (m_LaneConnection.TryGetComponent(parkedCar.m_Lane, ref laneConnection))
				{
					return num + AddLaneConnectionTargets(ref random, target, cost, flags, laneConnection, comparePosition3, 0f, allowLaneGroupSwitch: true, allowAccessRestriction);
				}
				if (GetEdge(parkedCar.m_Lane, out var edge3))
				{
					return num + AddEdgeTargets(ref random, target, cost, flags, edge3, comparePosition3, 0f, allowLaneGroupSwitch: false, allowAccessRestriction);
				}
				entity = parkedCar.m_Lane;
			}
			else if (m_SpawnLocation.HasComponent(parkedCar.m_Lane))
			{
				num += AddSpawnLocation(ref random, target, parkedCar.m_Lane, cost, flags, ignoreActivityMask: false, allowAccessRestriction);
				if (num != 0)
				{
					return num;
				}
				entity = parkedCar.m_Lane;
			}
			else
			{
				entity = parkedCar.m_Lane;
			}
		}
		if (m_WatercraftCurrentLane.HasComponent(entity))
		{
			GetWatercraftLane(entity, navigationEnd, out var lane2, out var curvePos2, out var flags3);
			if (m_Curve.HasComponent(lane2))
			{
				float3 comparePosition4 = MathUtils.Position(m_Curve[lane2].m_Bezier, curvePos2);
				return num + AddCarLaneTargets(ref random, target, lane2, comparePosition4, 0f, curvePos2, cost, flags, (flags3 & WatercraftLaneFlags.IsBlocked) != 0, m_Stopped.HasComponent(entity), allowAccessRestriction);
			}
			entity = lane2;
		}
		if (m_AircraftCurrentLane.HasComponent(entity))
		{
			GetAircraftLane(entity, navigationEnd, out var lane3, out var curvePos3, out var flags4);
			if ((flags4 & (AircraftLaneFlags.TransformTarget | AircraftLaneFlags.Flying)) != 0)
			{
				if (m_Transform.HasComponent(lane3))
				{
					float3 position = m_Transform[lane3].m_Position;
					AirwayHelpers.AirwayMap airwayMap = (m_Airplane.HasComponent(entity) ? m_AirwayData.airplaneMap : m_AirwayData.helicopterMap);
					lane3 = Entity.Null;
					float distance = float.MaxValue;
					airwayMap.FindClosestLane(position, m_Curve, ref lane3, ref curvePos3, ref distance);
					if (lane3 != Entity.Null)
					{
						AddTarget(ref random, entity, lane3, curvePos3, cost, ~(EdgeFlags.DefaultMask | EdgeFlags.Secondary));
						num++;
					}
				}
				else if (m_Curve.HasComponent(lane3))
				{
					AddTarget(ref random, entity, lane3, curvePos3, cost, flags);
					num++;
				}
				return num;
			}
			if (m_Curve.HasComponent(lane3))
			{
				float3 comparePosition5 = MathUtils.Position(m_Curve[lane3].m_Bezier, curvePos3);
				return num + AddCarLaneTargets(ref random, target, lane3, comparePosition5, 0f, curvePos3, cost, flags, allowLaneGroupSwitch: true, m_Stopped.HasComponent(entity), allowAccessRestriction);
			}
			entity = lane3;
		}
		if (m_Train.HasComponent(entity))
		{
			return num + AddTrainTargets(ref random, target, entity, cost, flags);
		}
		bool flag = false;
		if (m_RouteLane.HasComponent(entity))
		{
			RouteLane routeLane = m_RouteLane[entity];
			if (m_IsStartTarget)
			{
				if (routeLane.m_EndLane != Entity.Null)
				{
					num += AddLaneTarget(ref random, target, Entity.Null, routeLane.m_EndLane, routeLane.m_EndCurvePos, cost, flags, allowAccessRestriction);
				}
			}
			else if (routeLane.m_StartLane != Entity.Null)
			{
				num += AddLaneTarget(ref random, target, Entity.Null, routeLane.m_StartLane, routeLane.m_StartCurvePos, cost, flags, allowAccessRestriction);
			}
			flag = true;
		}
		if (m_AccessLane.HasComponent(entity))
		{
			AccessLane accessLane = m_AccessLane[entity];
			if (accessLane.m_Lane != Entity.Null)
			{
				num = ((!m_SpawnLocation.HasComponent(accessLane.m_Lane)) ? (num + AddLaneTarget(ref random, target, Entity.Null, accessLane.m_Lane, accessLane.m_CurvePos, cost, flags, allowAccessRestriction)) : (num + AddSpawnLocation(ref random, target, accessLane.m_Lane, cost, flags, ignoreActivityMask: true, allowAccessRestriction)));
			}
			flag = true;
		}
		if (flag && num != 0)
		{
			return num;
		}
		if (m_Attached.HasComponent(entity) && !m_Building.HasComponent(entity))
		{
			Attached attached = m_Attached[entity];
			if (m_SubLane.HasBuffer(attached.m_Parent))
			{
				Transform transform = m_Transform[entity];
				return num + AddEdgeTargets(ref random, target, cost, flags, attached.m_Parent, transform.m_Position, 0f, allowLaneGroupSwitch: false, allowAccessRestriction);
			}
		}
		Entity val = (m_PropertyRenter.HasComponent(entity) ? m_PropertyRenter[entity].m_Property : ((!m_CurrentBuilding.HasComponent(entity)) ? entity : m_CurrentBuilding[entity].m_CurrentBuilding));
		while (!m_SpawnLocations.HasBuffer(val))
		{
			if (m_SubLane.HasBuffer(val))
			{
				num += AddSubLaneTargets(ref random, target, val, cost, randomCurvePos: false, allowAccessRestriction: false, flags);
			}
			if (m_Owner.HasComponent(val))
			{
				val = m_Owner[val].m_Owner;
				continue;
			}
			return num;
		}
		bool addFrontConnection = num == 0;
		if ((m_PathfindParameters.m_PathfindFlags & PathfindFlags.Simplified) == 0 && m_SpawnLocations.HasBuffer(val))
		{
			DynamicBuffer<SpawnLocationElement> val2 = m_SpawnLocations[val];
			int num2 = 0;
			if (m_SetupQueueTarget.m_RandomCost != 0f)
			{
				int num3 = 0;
				for (int i = 0; i < val2.Length; i++)
				{
					Entity spawnLocation = val2[i].m_SpawnLocation;
					num3 = ((val2[i].m_Type != SpawnLocationType.ParkingLane) ? (num3 + AddSpawnLocation(ref random, target, spawnLocation, cost, flags, ignoreActivityMask: false, allowAccessRestriction, countOnly: true, ignoreParked: false, ref addFrontConnection)) : (num3 + AddParkingLane(ref random, target, spawnLocation, cost, flags, allowAccessRestriction, countOnly: true, ref addFrontConnection)));
				}
				num2 = ((Random)(ref random)).NextInt(num3);
			}
			for (int j = 0; j < val2.Length; j++)
			{
				Entity spawnLocation2 = val2[j].m_SpawnLocation;
				float cost2 = math.select(cost, cost + m_SetupQueueTarget.m_RandomCost, num2 != 0);
				int num4 = ((val2[j].m_Type != SpawnLocationType.ParkingLane) ? AddSpawnLocation(ref random, target, spawnLocation2, cost2, flags, ignoreActivityMask: false, allowAccessRestriction, countOnly: false, ignoreParked: false, ref addFrontConnection) : AddParkingLane(ref random, target, spawnLocation2, cost, flags, allowAccessRestriction, countOnly: false, ref addFrontConnection));
				num += num4;
				num2 -= num4;
			}
		}
		Building building = default(Building);
		if (addFrontConnection && m_Building.TryGetComponent(val, ref building))
		{
			PrefabRef prefabRef = m_PrefabRef[val];
			Transform transform2 = m_Transform[val];
			if (building.m_RoadEdge != Entity.Null)
			{
				BuildingData buildingData = m_BuildingData[prefabRef.m_Prefab];
				float3 comparePosition6 = transform2.m_Position;
				Owner owner = default(Owner);
				if (!m_Owner.TryGetComponent(building.m_RoadEdge, ref owner) || owner.m_Owner != val)
				{
					comparePosition6 = BuildingUtils.CalculateFrontPosition(transform2, buildingData.m_LotSize.y);
				}
				num += AddEdgeTargets(ref random, target, cost, flags, building.m_RoadEdge, comparePosition6, 0f, allowLaneGroupSwitch: true, allowAccessRestriction: false);
			}
		}
		return num;
	}

	private bool CheckAccessRestriction(bool allowAccessRestriction, Game.Objects.SpawnLocation spawnLocation)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!allowAccessRestriction && !(spawnLocation.m_AccessRestriction == Entity.Null))
		{
			return (spawnLocation.m_Flags & (SpawnLocationFlags.AllowEnter | SpawnLocationFlags.AllowExit)) == SpawnLocationFlags.AllowEnter;
		}
		return true;
	}

	private bool CheckAccessRestriction(bool allowAccessRestriction, Game.Net.PedestrianLane pedestrianLane)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!allowAccessRestriction && !(pedestrianLane.m_AccessRestriction == Entity.Null))
		{
			return (pedestrianLane.m_Flags & (PedestrianLaneFlags.AllowEnter | PedestrianLaneFlags.AllowExit)) == PedestrianLaneFlags.AllowEnter;
		}
		return true;
	}

	private bool CheckAccessRestriction(bool allowAccessRestriction, Game.Net.CarLane carLane)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!allowAccessRestriction && !(carLane.m_AccessRestriction == Entity.Null))
		{
			return ((uint)carLane.m_Flags & 0x80000000u) != 0;
		}
		return true;
	}

	private bool CheckAccessRestriction(bool allowAccessRestriction, Game.Net.ParkingLane parkingLane)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!allowAccessRestriction && !(parkingLane.m_AccessRestriction == Entity.Null))
		{
			return (parkingLane.m_Flags & (ParkingLaneFlags.AllowEnter | ParkingLaneFlags.AllowExit)) == ParkingLaneFlags.AllowEnter;
		}
		return true;
	}

	private bool CheckAccessRestriction(bool allowAccessRestriction, Game.Net.ConnectionLane connectionLane)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!allowAccessRestriction && !(connectionLane.m_AccessRestriction == Entity.Null))
		{
			return (connectionLane.m_Flags & (ConnectionLaneFlags.AllowEnter | ConnectionLaneFlags.AllowExit)) == ConnectionLaneFlags.AllowEnter;
		}
		return true;
	}

	private int AddParkingLane(ref Random random, Entity target, Entity parkingLaneEntity, float cost, EdgeFlags flags, bool allowAccessRestriction, bool countOnly, ref bool addFrontConnection)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if ((m_SetupQueueTarget.m_Methods & (PathMethod.Parking | PathMethod.SpecialParking)) == 0)
		{
			return 0;
		}
		PrefabRef prefabRef = m_PrefabRef[parkingLaneEntity];
		ParkingLaneData parkingLaneData = default(ParkingLaneData);
		if (!m_ParkingLaneData.TryGetComponent(prefabRef.m_Prefab, ref parkingLaneData))
		{
			return 0;
		}
		if ((m_SetupQueueTarget.m_RoadTypes & parkingLaneData.m_RoadTypes) == 0)
		{
			return 0;
		}
		Game.Net.ParkingLane parkingLane = m_ParkingLane[parkingLaneEntity];
		PathMethod pathMethod = (((parkingLane.m_Flags & ParkingLaneFlags.SpecialVehicles) != 0) ? PathMethod.SpecialParking : PathMethod.Parking);
		float x = VehicleUtils.GetParkingSize(parkingLaneData).x;
		float num = math.max(1f, parkingLane.m_FreeSpace);
		if ((m_SetupQueueTarget.m_Methods & pathMethod) == 0)
		{
			return 0;
		}
		if (math.any(m_PathfindParameters.m_ParkingSize > new float2(x, num)))
		{
			return 0;
		}
		if (!CheckAccessRestriction(allowAccessRestriction, parkingLane))
		{
			return 0;
		}
		if (!countOnly)
		{
			AddTarget(ref random, target, parkingLaneEntity, 0.5f, cost, flags);
			addFrontConnection = false;
		}
		return 1;
	}

	private int AddSpawnLocation(ref Random random, Entity target, Entity spawnLocationEntity, float cost, EdgeFlags flags, bool ignoreActivityMask, bool allowAccessRestriction)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		bool addFrontConnection = false;
		return AddSpawnLocation(ref random, target, spawnLocationEntity, cost, flags, ignoreActivityMask, allowAccessRestriction, countOnly: false, ignoreParked: true, ref addFrontConnection);
	}

	private int AddSpawnLocation(ref Random random, Entity target, Entity spawnLocationEntity, float cost, EdgeFlags flags, bool ignoreActivityMask, bool allowAccessRestriction, bool countOnly, bool ignoreParked, ref bool addFrontConnection)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef prefabRef = m_PrefabRef[spawnLocationEntity];
		SpawnLocationData spawnLocationData = default(SpawnLocationData);
		if (!m_SpawnLocationData.TryGetComponent(prefabRef.m_Prefab, ref spawnLocationData))
		{
			return 0;
		}
		bool flag = false;
		switch (spawnLocationData.m_ConnectionType)
		{
		case RouteConnectionType.Pedestrian:
			if ((m_SetupQueueTarget.m_Methods & PathMethod.Pedestrian) == 0)
			{
				return 0;
			}
			break;
		case RouteConnectionType.Cargo:
			if ((m_SetupQueueTarget.m_Methods & PathMethod.CargoLoading) == 0 || (m_SetupQueueTarget.m_RoadTypes & spawnLocationData.m_RoadTypes) == 0)
			{
				return 0;
			}
			if ((m_PathfindParameters.m_Methods & PathMethod.CargoLoading) == 0)
			{
				flag = true;
			}
			break;
		case RouteConnectionType.Road:
			if ((m_SetupQueueTarget.m_Methods & PathMethod.Road) == 0 || (m_SetupQueueTarget.m_RoadTypes & spawnLocationData.m_RoadTypes) == 0)
			{
				return 0;
			}
			if (!ignoreParked && (m_SetupQueueTarget.m_Methods & PathMethod.SpecialParking) != 0)
			{
				cost += m_SetupQueueTarget.m_RandomCost;
			}
			break;
		case RouteConnectionType.Air:
			if ((m_SetupQueueTarget.m_Methods & PathMethod.Road) == 0 || (m_SetupQueueTarget.m_RoadTypes & spawnLocationData.m_RoadTypes) == 0)
			{
				return 0;
			}
			break;
		case RouteConnectionType.Track:
			if ((m_SetupQueueTarget.m_Methods & PathMethod.Track) == 0 || (m_SetupQueueTarget.m_TrackTypes & spawnLocationData.m_TrackTypes) == 0)
			{
				return 0;
			}
			break;
		case RouteConnectionType.Parking:
			if ((m_SetupQueueTarget.m_Methods & PathMethod.Parking) == 0 || (m_SetupQueueTarget.m_RoadTypes & spawnLocationData.m_RoadTypes) == 0)
			{
				return 0;
			}
			break;
		default:
			return 0;
		}
		if (!ignoreActivityMask && spawnLocationData.m_ActivityMask.m_Mask != 0 && (spawnLocationData.m_ActivityMask.m_Mask & m_SetupQueueTarget.m_ActivityMask.m_Mask) == 0)
		{
			return 0;
		}
		Game.Objects.SpawnLocation spawnLocation = default(Game.Objects.SpawnLocation);
		DynamicBuffer<Game.Net.SubLane> val = default(DynamicBuffer<Game.Net.SubLane>);
		if (m_SpawnLocation.TryGetComponent(spawnLocationEntity, ref spawnLocation))
		{
			if (CheckAccessRestriction(allowAccessRestriction, spawnLocation))
			{
				if (!countOnly)
				{
					cost += math.select(math.max(m_SetupQueueTarget.m_RandomCost * 3f, 30f), 0f, ignoreParked || (spawnLocation.m_Flags & SpawnLocationFlags.ParkedVehicle) == 0);
					if (flag)
					{
						if (spawnLocation.m_ConnectedLane1 != Entity.Null)
						{
							AddTarget(ref random, target, spawnLocation.m_ConnectedLane1, spawnLocation.m_CurvePosition1, cost, flags);
						}
						if (spawnLocation.m_ConnectedLane2 != Entity.Null)
						{
							AddTarget(ref random, target, spawnLocation.m_ConnectedLane2, spawnLocation.m_CurvePosition2, cost, flags);
						}
					}
					else
					{
						AddTarget(ref random, target, spawnLocationEntity, 1f, cost, flags);
					}
					addFrontConnection &= spawnLocation.m_ConnectedLane1 == Entity.Null;
				}
				return 1;
			}
		}
		else if (m_SubLane.TryGetBuffer(spawnLocationEntity, ref val))
		{
			int num = 0;
			int2 val2 = default(int2);
			((int2)(ref val2))._002Ector(0, val.Length - 1);
			if (val.Length == 0)
			{
				Debug.Log((object)$"Empty subLanes: {spawnLocationEntity.Index}");
			}
			else if (m_SetupQueueTarget.m_RandomCost != 0f)
			{
				val2 = int2.op_Implicit(((Random)(ref random)).NextInt(val.Length));
			}
			Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
			for (int i = val2.x; i <= val2.y; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (m_ConnectionLane.TryGetComponent(subLane, ref connectionLane) && CheckAccessRestriction(allowAccessRestriction, connectionLane) && (((m_SetupQueueTarget.m_Methods & PathMethod.Pedestrian) != 0 && (connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0) || ((m_SetupQueueTarget.m_Methods & PathMethod.Offroad) != 0 && (connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0 && (m_SetupQueueTarget.m_RoadTypes & connectionLane.m_RoadTypes) != RoadTypes.None)))
				{
					if (!countOnly)
					{
						AddTarget(ref random, target, subLane, 0.5f, cost, flags);
						addFrontConnection = false;
					}
					num++;
				}
			}
			return num;
		}
		return 0;
	}

	private void GetCarLane(Entity entity, bool navigationEnd, out Entity lane, out float curvePos, out Game.Vehicles.CarLaneFlags flags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		CarCurrentLane carCurrentLane = m_CarCurrentLane[entity];
		lane = carCurrentLane.m_Lane;
		curvePos = math.select(carCurrentLane.m_CurvePosition.y, carCurrentLane.m_CurvePosition.z, navigationEnd || (carCurrentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.ClearedForPathfind) != 0);
		flags = carCurrentLane.m_LaneFlags;
		DynamicBuffer<CarNavigationLane> val = default(DynamicBuffer<CarNavigationLane>);
		if (!m_CarNavigationLanes.TryGetBuffer(entity, ref val))
		{
			return;
		}
		if (navigationEnd)
		{
			if (val.Length != 0)
			{
				CarNavigationLane carNavigationLane = val[val.Length - 1];
				lane = carNavigationLane.m_Lane;
				curvePos = carNavigationLane.m_CurvePosition.y;
				flags = carNavigationLane.m_Flags;
			}
			return;
		}
		for (int i = 0; i < val.Length; i++)
		{
			CarNavigationLane carNavigationLane2 = val[i];
			if ((carNavigationLane2.m_Flags & (Game.Vehicles.CarLaneFlags.Reserved | Game.Vehicles.CarLaneFlags.ClearedForPathfind)) == 0)
			{
				break;
			}
			lane = carNavigationLane2.m_Lane;
			curvePos = carNavigationLane2.m_CurvePosition.y;
			flags = carNavigationLane2.m_Flags;
		}
	}

	private void GetWatercraftLane(Entity entity, bool navigationEnd, out Entity lane, out float curvePos, out WatercraftLaneFlags flags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		WatercraftCurrentLane watercraftCurrentLane = m_WatercraftCurrentLane[entity];
		lane = watercraftCurrentLane.m_Lane;
		curvePos = math.select(watercraftCurrentLane.m_CurvePosition.y, watercraftCurrentLane.m_CurvePosition.z, navigationEnd);
		flags = watercraftCurrentLane.m_LaneFlags;
		if (!m_WatercraftNavigationLanes.HasBuffer(entity))
		{
			return;
		}
		DynamicBuffer<WatercraftNavigationLane> val = m_WatercraftNavigationLanes[entity];
		if (navigationEnd)
		{
			if (val.Length != 0)
			{
				WatercraftNavigationLane watercraftNavigationLane = val[val.Length - 1];
				lane = watercraftNavigationLane.m_Lane;
				curvePos = watercraftNavigationLane.m_CurvePosition.y;
				flags = watercraftNavigationLane.m_Flags;
			}
			return;
		}
		for (int i = 0; i < val.Length; i++)
		{
			WatercraftNavigationLane watercraftNavigationLane2 = val[i];
			if ((watercraftNavigationLane2.m_Flags & WatercraftLaneFlags.Reserved) == 0)
			{
				break;
			}
			lane = watercraftNavigationLane2.m_Lane;
			curvePos = watercraftNavigationLane2.m_CurvePosition.y;
			flags = watercraftNavigationLane2.m_Flags;
		}
	}

	private void GetAircraftLane(Entity entity, bool navigationEnd, out Entity lane, out float curvePos, out AircraftLaneFlags flags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		AircraftCurrentLane aircraftCurrentLane = m_AircraftCurrentLane[entity];
		lane = aircraftCurrentLane.m_Lane;
		curvePos = math.select(aircraftCurrentLane.m_CurvePosition.y, aircraftCurrentLane.m_CurvePosition.z, navigationEnd);
		flags = aircraftCurrentLane.m_LaneFlags;
		if (!m_AircraftNavigationLanes.HasBuffer(entity))
		{
			return;
		}
		DynamicBuffer<AircraftNavigationLane> val = m_AircraftNavigationLanes[entity];
		if (navigationEnd)
		{
			if (val.Length != 0)
			{
				AircraftNavigationLane aircraftNavigationLane = val[val.Length - 1];
				lane = aircraftNavigationLane.m_Lane;
				curvePos = aircraftNavigationLane.m_CurvePosition.y;
				flags = aircraftNavigationLane.m_Flags;
			}
			return;
		}
		for (int i = 0; i < val.Length; i++)
		{
			AircraftNavigationLane aircraftNavigationLane2 = val[i];
			if ((aircraftNavigationLane2.m_Flags & AircraftLaneFlags.Reserved) == 0)
			{
				break;
			}
			lane = aircraftNavigationLane2.m_Lane;
			curvePos = aircraftNavigationLane2.m_CurvePosition.y;
			flags = aircraftNavigationLane2.m_Flags;
		}
	}

	private bool GetEdge(Entity lane, out Entity edge)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (m_Owner.HasComponent(lane))
		{
			Owner owner = m_Owner[lane];
			if (m_SubLane.HasBuffer(owner.m_Owner))
			{
				edge = owner.m_Owner;
				return true;
			}
		}
		edge = Entity.Null;
		return false;
	}

	private int AddSubLaneTargets(ref Random random, Entity target, Entity entity, float cost, bool randomCurvePos, bool allowAccessRestriction, EdgeFlags flags)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		Entity val = entity;
		while (m_Owner.HasComponent(val))
		{
			val = m_Owner[val].m_Owner;
		}
		DynamicBuffer<Game.Net.SubLane> val2 = m_SubLane[entity];
		for (int i = 0; i < val2.Length; i++)
		{
			Entity subLane = val2[i].m_SubLane;
			float curvePos = 0.5f;
			if (randomCurvePos)
			{
				curvePos = ((Random)(ref random)).NextFloat();
			}
			num += AddLaneTarget(ref random, target, val, subLane, curvePos, cost, flags, allowAccessRestriction);
		}
		return num;
	}

	public int AddAreaTargets(ref Random random, Entity target, Entity entity, Entity subItem, DynamicBuffer<Game.Areas.SubArea> subAreas, float cost, bool addDistanceCost, EdgeFlags flags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Transform.HasComponent(subItem))
		{
			int num = 0;
			if (m_SubLane.HasBuffer(entity))
			{
				num += AddSubLaneTargets(ref random, target, entity, cost, m_SetupQueueTarget.m_RandomCost != 0f, allowAccessRestriction: true, flags);
			}
			if (subAreas.IsCreated)
			{
				for (int i = 0; i < subAreas.Length; i++)
				{
					Game.Areas.SubArea subArea = subAreas[i];
					if (m_SubLane.HasBuffer(subArea.m_Area))
					{
						num += AddSubLaneTargets(ref random, target, subArea.m_Area, cost, m_SetupQueueTarget.m_RandomCost != 0f, allowAccessRestriction: true, flags);
					}
				}
			}
			return num;
		}
		Transform transform = m_Transform[subItem];
		int num2 = 0;
		if (subAreas.IsCreated)
		{
			num2 = subAreas.Length;
		}
		float num3 = float.MaxValue;
		Entity val = Entity.Null;
		float delta = 0f;
		DynamicBuffer<Game.Net.SubLane> val2 = default(DynamicBuffer<Game.Net.SubLane>);
		float2 val6 = default(float2);
		float2 val7 = default(float2);
		float num8 = default(float);
		for (int j = -1; j < num2; j++)
		{
			if (j >= 0)
			{
				entity = subAreas[j].m_Area;
			}
			if (!m_SubLane.TryGetBuffer(entity, ref val2) || val2.Length == 0)
			{
				continue;
			}
			DynamicBuffer<Game.Areas.Node> nodes = m_AreaNode[entity];
			DynamicBuffer<Triangle> val3 = m_AreaTriangle[entity];
			float num4 = num3;
			float3 val4 = transform.m_Position;
			Triangle3 val5 = default(Triangle3);
			for (int k = 0; k < val3.Length; k++)
			{
				Triangle3 triangle = AreaUtils.GetTriangle3(nodes, val3[k]);
				float num5 = MathUtils.Distance(triangle, transform.m_Position, ref val6);
				if (num5 < num4)
				{
					num4 = num5;
					val4 = MathUtils.Position(triangle, val6);
					val5 = triangle;
				}
			}
			if (num4 == num3)
			{
				continue;
			}
			float num6 = float.MaxValue;
			for (int l = 0; l < val2.Length; l++)
			{
				Entity subLane = val2[l].m_SubLane;
				if (!m_ConnectionLane.HasComponent(subLane))
				{
					continue;
				}
				Game.Net.ConnectionLane connectionLane = m_ConnectionLane[subLane];
				if (((m_SetupQueueTarget.m_Methods & PathMethod.Pedestrian) == 0 || (connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) == 0) && ((m_SetupQueueTarget.m_Methods & PathMethod.Offroad) == 0 || (connectionLane.m_Flags & ConnectionLaneFlags.Road) == 0 || (m_SetupQueueTarget.m_RoadTypes & connectionLane.m_RoadTypes) == 0))
				{
					continue;
				}
				Curve curve = m_Curve[subLane];
				if (MathUtils.Intersect(((Triangle3)(ref val5)).xz, ((float3)(ref curve.m_Bezier.a)).xz, ref val7) || MathUtils.Intersect(((Triangle3)(ref val5)).xz, ((float3)(ref curve.m_Bezier.d)).xz, ref val7))
				{
					float num7 = MathUtils.Distance(curve.m_Bezier, val4, ref num8);
					if (num7 < num6)
					{
						num3 = num4;
						val = subLane;
						delta = num8;
						num6 = num7;
					}
				}
			}
		}
		if (val != Entity.Null)
		{
			cost += math.select(0f, num3, addDistanceCost);
			AddTarget(ref random, target, val, delta, cost, flags);
			return 1;
		}
		return 0;
	}

	private int AddLaneTarget(ref Random random, Entity target, Entity accessRequirement, Entity lane, float curvePos, float cost, EdgeFlags flags, bool allowAccessRestriction)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		if ((m_SetupQueueTarget.m_Methods & PathMethod.Pedestrian) != 0 && m_PedestrianLane.HasComponent(lane))
		{
			Game.Net.PedestrianLane pedestrianLane = m_PedestrianLane[lane];
			if (CheckAccessRestriction(allowAccessRestriction, pedestrianLane) || pedestrianLane.m_AccessRestriction == accessRequirement)
			{
				AddTarget(ref random, target, lane, curvePos, cost, flags);
				return 1;
			}
		}
		if ((m_SetupQueueTarget.m_Methods & PathMethod.Road) != 0 && m_CarLane.HasComponent(lane) && !m_SlaveLane.HasComponent(lane))
		{
			Game.Net.CarLane carLane = m_CarLane[lane];
			if (CheckAccessRestriction(allowAccessRestriction, carLane) || carLane.m_AccessRestriction == accessRequirement)
			{
				PrefabRef prefabRef = m_PrefabRef[lane];
				CarLaneData carLaneData = m_CarLaneData[prefabRef.m_Prefab];
				if (VehicleUtils.CanUseLane(m_SetupQueueTarget.m_Methods, m_SetupQueueTarget.m_RoadTypes, carLaneData))
				{
					AddTarget(ref random, target, lane, curvePos, cost, flags);
					return 1;
				}
			}
		}
		Game.Net.ParkingLane parkingLane = default(Game.Net.ParkingLane);
		if ((m_SetupQueueTarget.m_Methods & (PathMethod.Parking | PathMethod.Boarding | PathMethod.SpecialParking)) != 0 && m_ParkingLane.TryGetComponent(lane, ref parkingLane) && (CheckAccessRestriction(allowAccessRestriction, parkingLane) || parkingLane.m_AccessRestriction == accessRequirement))
		{
			PrefabRef prefabRef2 = m_PrefabRef[lane];
			ParkingLaneData parkingLaneData = m_ParkingLaneData[prefabRef2.m_Prefab];
			PathMethod pathMethod = (((parkingLane.m_Flags & ParkingLaneFlags.SpecialVehicles) != 0) ? (PathMethod.Boarding | PathMethod.SpecialParking) : (PathMethod.Parking | PathMethod.Boarding));
			if ((m_SetupQueueTarget.m_RoadTypes & parkingLaneData.m_RoadTypes) != RoadTypes.None && (m_SetupQueueTarget.m_Methods & pathMethod) != 0)
			{
				AddTarget(ref random, target, lane, curvePos, cost, flags);
				return 1;
			}
		}
		if ((m_SetupQueueTarget.m_Methods & PathMethod.Track) != 0)
		{
			PrefabRef prefabRef3 = m_PrefabRef[lane];
			if (m_TrackLaneData.HasComponent(prefabRef3.m_Prefab))
			{
				TrackLaneData trackLaneData = m_TrackLaneData[prefabRef3.m_Prefab];
				if ((m_SetupQueueTarget.m_TrackTypes & trackLaneData.m_TrackTypes) != TrackTypes.None)
				{
					AddTarget(ref random, target, lane, curvePos, cost, flags);
					return 1;
				}
			}
		}
		if (m_ConnectionLane.HasComponent(lane))
		{
			Game.Net.ConnectionLane connectionLane = m_ConnectionLane[lane];
			if ((CheckAccessRestriction(allowAccessRestriction, connectionLane) || connectionLane.m_AccessRestriction == accessRequirement) && (connectionLane.m_Flags & ConnectionLaneFlags.Inside) == 0 && (((m_SetupQueueTarget.m_Methods & PathMethod.Pedestrian) != 0 && (connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0) || ((m_SetupQueueTarget.m_Methods & PathMethod.Road) != 0 && (connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0 && (m_SetupQueueTarget.m_RoadTypes & connectionLane.m_RoadTypes) != RoadTypes.None) || ((m_SetupQueueTarget.m_Methods & PathMethod.Track) != 0 && (connectionLane.m_Flags & ConnectionLaneFlags.Track) != 0 && (m_SetupQueueTarget.m_TrackTypes & connectionLane.m_TrackTypes) != TrackTypes.None) || ((m_SetupQueueTarget.m_Methods & PathMethod.CargoLoading) != 0 && (connectionLane.m_Flags & ConnectionLaneFlags.AllowCargo) != 0)))
			{
				curvePos = math.select(curvePos, 1f, (connectionLane.m_Flags & ConnectionLaneFlags.Start) != 0);
				AddTarget(ref random, target, lane, curvePos, cost, flags);
				return 1;
			}
		}
		return 0;
	}

	private int AddTrainTargets(ref Random random, Entity target, Entity entity, float cost, EdgeFlags flags)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
		if (m_VehicleLayout.TryGetBuffer(entity, ref val) && val.Length != 0)
		{
			Entity vehicle = val[0].m_Vehicle;
			Entity vehicle2 = val[val.Length - 1].m_Vehicle;
			TrainCurrentLane trainCurrentLane = default(TrainCurrentLane);
			ParkedTrain parkedTrain = default(ParkedTrain);
			if (m_TrainCurrentLane.TryGetComponent(vehicle, ref trainCurrentLane))
			{
				num += AddTrainTarget(ref random, target, cost, flags, vehicle, trainCurrentLane.m_Front.m_Lane, trainCurrentLane.m_Front.m_CurvePosition.w, trainForward: true);
			}
			else if (m_ParkedTrain.TryGetComponent(vehicle, ref parkedTrain))
			{
				num += AddTrainTarget(ref random, target, cost, flags, vehicle, parkedTrain.m_FrontLane, parkedTrain.m_CurvePosition.x, trainForward: true);
			}
			TrainCurrentLane trainCurrentLane2 = default(TrainCurrentLane);
			ParkedTrain parkedTrain2 = default(ParkedTrain);
			if (m_TrainCurrentLane.TryGetComponent(vehicle2, ref trainCurrentLane2))
			{
				num += AddTrainTarget(ref random, target, cost, flags, vehicle2, trainCurrentLane2.m_Rear.m_Lane, trainCurrentLane2.m_Rear.m_CurvePosition.y, trainForward: false);
			}
			else if (m_ParkedTrain.TryGetComponent(vehicle2, ref parkedTrain2))
			{
				num += AddTrainTarget(ref random, target, cost, flags, vehicle2, parkedTrain2.m_RearLane, parkedTrain2.m_CurvePosition.y, trainForward: false);
			}
			if (num != 0)
			{
				return num;
			}
		}
		TrainCurrentLane trainCurrentLane3 = default(TrainCurrentLane);
		ParkedTrain parkedTrain3 = default(ParkedTrain);
		if (m_TrainCurrentLane.TryGetComponent(entity, ref trainCurrentLane3))
		{
			num += AddTrainTarget(ref random, target, cost, flags, entity, trainCurrentLane3.m_Front.m_Lane, trainCurrentLane3.m_Front.m_CurvePosition.w, trainForward: true);
			num += AddTrainTarget(ref random, target, cost, flags, entity, trainCurrentLane3.m_Rear.m_Lane, trainCurrentLane3.m_Rear.m_CurvePosition.y, trainForward: false);
		}
		else if (m_ParkedTrain.TryGetComponent(entity, ref parkedTrain3))
		{
			num += AddTrainTarget(ref random, target, cost, flags, entity, parkedTrain3.m_FrontLane, parkedTrain3.m_CurvePosition.x, trainForward: true);
			num += AddTrainTarget(ref random, target, cost, flags, entity, parkedTrain3.m_RearLane, parkedTrain3.m_CurvePosition.y, trainForward: false);
		}
		return num;
	}

	private int AddTrainTarget(ref Random random, Entity target, float cost, EdgeFlags flags, Entity carriage, Entity lane, float curvePosition, bool trainForward)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Curve curve = default(Curve);
		if (m_Curve.TryGetComponent(lane, ref curve))
		{
			Train train = m_Train[carriage];
			bool flag = math.dot(math.forward(m_Transform[carriage].m_Rotation), MathUtils.Tangent(curve.m_Bezier, curvePosition)) >= 0f;
			flag ^= (train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0 == trainForward;
			flags = (EdgeFlags)((uint)flags & (uint)(ushort)(~((!flag) ? 1 : 2)));
			AddTarget(ref random, target, lane, curvePosition, cost, flags);
			return 1;
		}
		return 0;
	}

	public int AddLaneConnectionTargets(ref Random random, Entity target, float cost, EdgeFlags flags, LaneConnection laneConnection, float3 comparePosition, float maxDistance, bool allowLaneGroupSwitch, bool allowAccessRestriction)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		if ((m_SetupQueueTarget.m_Methods & PathMethod.Pedestrian) != 0 && laneConnection.m_EndLane != Entity.Null)
		{
			Game.Net.PedestrianLane pedestrianLane = default(Game.Net.PedestrianLane);
			Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
			if (m_PedestrianLane.TryGetComponent(laneConnection.m_EndLane, ref pedestrianLane))
			{
				if (CheckAccessRestriction(allowAccessRestriction, pedestrianLane))
				{
					goto IL_007e;
				}
			}
			else if (m_ConnectionLane.TryGetComponent(laneConnection.m_EndLane, ref connectionLane) && (connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0 && CheckAccessRestriction(allowAccessRestriction, connectionLane))
			{
				goto IL_007e;
			}
		}
		goto IL_00b8;
		IL_01bf:
		float curvePos = default(float);
		MathUtils.Distance(m_Curve[laneConnection.m_StartLane].m_Bezier, comparePosition, ref curvePos);
		num += AddCarLaneTargets(ref random, target, laneConnection.m_StartLane, comparePosition, maxDistance, curvePos, cost, flags, allowLaneGroupSwitch, allowBlocked: false, allowAccessRestriction);
		goto IL_0200;
		IL_007e:
		float curvePos2 = default(float);
		float distance = MathUtils.Distance(m_Curve[laneConnection.m_EndLane].m_Bezier, comparePosition, ref curvePos2);
		num += AddPedestrianLaneTargets(ref random, target, laneConnection.m_EndLane, curvePos2, cost, distance, flags, allowAccessRestriction);
		goto IL_00b8;
		IL_00b8:
		if ((m_SetupQueueTarget.m_Methods & PathMethod.Road) != 0 && laneConnection.m_StartLane != Entity.Null)
		{
			PrefabRef prefabRef = default(PrefabRef);
			CarLaneData carLaneData = default(CarLaneData);
			if (m_PrefabRef.TryGetComponent(laneConnection.m_StartLane, ref prefabRef) && m_CarLaneData.TryGetComponent(prefabRef.m_Prefab, ref carLaneData) && !m_MasterLane.HasComponent(laneConnection.m_StartLane))
			{
				Game.Net.CarLane carLane = m_CarLane[laneConnection.m_StartLane];
				if (VehicleUtils.CanUseLane(m_SetupQueueTarget.m_Methods, m_SetupQueueTarget.m_RoadTypes, carLaneData) && CheckAccessRestriction(allowAccessRestriction, carLane))
				{
					goto IL_01bf;
				}
			}
			else if (m_ConnectionLane.HasComponent(laneConnection.m_StartLane))
			{
				Game.Net.ConnectionLane connectionLane2 = m_ConnectionLane[laneConnection.m_StartLane];
				if ((connectionLane2.m_Flags & ConnectionLaneFlags.Road) != 0 && (connectionLane2.m_RoadTypes & m_SetupQueueTarget.m_RoadTypes) != RoadTypes.None && CheckAccessRestriction(allowAccessRestriction, connectionLane2))
				{
					goto IL_01bf;
				}
			}
		}
		goto IL_0200;
		IL_0200:
		return num;
	}

	public int AddEdgeTargets(ref Random random, Entity target, float cost, EdgeFlags flags, Entity edge, float3 comparePosition, float maxDistance, bool allowLaneGroupSwitch, bool allowAccessRestriction)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Net.SubLane> val = m_SubLane[edge];
		float num = float.MaxValue;
		float curvePos = 0f;
		Entity val2 = Entity.Null;
		float num2 = float.MaxValue;
		float curvePos2 = 0f;
		Entity val3 = Entity.Null;
		float num3 = float.MaxValue;
		float delta = 0f;
		Entity val4 = Entity.Null;
		float num4 = float.MaxValue;
		float delta2 = 0f;
		Entity val5 = Entity.Null;
		float num6 = default(float);
		float num8 = default(float);
		ParkingLaneData parkingLaneData = default(ParkingLaneData);
		Game.Net.ConnectionLane connectionLane2 = default(Game.Net.ConnectionLane);
		float num10 = default(float);
		float num12 = default(float);
		for (int i = 0; i < val.Length; i++)
		{
			Game.Net.SubLane subLane = val[i];
			PathMethod pathMethod = m_SetupQueueTarget.m_Methods & subLane.m_PathMethods;
			if (pathMethod == (PathMethod)0)
			{
				continue;
			}
			if ((pathMethod & PathMethod.Pedestrian) != 0)
			{
				if (m_PedestrianLane.HasComponent(subLane.m_SubLane))
				{
					Game.Net.PedestrianLane pedestrianLane = m_PedestrianLane[subLane.m_SubLane];
					if (CheckAccessRestriction(allowAccessRestriction, pedestrianLane))
					{
						goto IL_0116;
					}
				}
				else if (m_ConnectionLane.HasComponent(subLane.m_SubLane))
				{
					Game.Net.ConnectionLane connectionLane = m_ConnectionLane[subLane.m_SubLane];
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0 && CheckAccessRestriction(allowAccessRestriction, connectionLane))
					{
						goto IL_0116;
					}
				}
			}
			goto IL_016a;
			IL_0116:
			Curve curve = m_Curve[subLane.m_SubLane];
			if (MathUtils.Distance(MathUtils.Bounds(curve.m_Bezier), comparePosition) < num)
			{
				float num5 = MathUtils.Distance(curve.m_Bezier, comparePosition, ref num6);
				if (num5 < num)
				{
					num = num5;
					curvePos = num6;
					val2 = subLane.m_SubLane;
					continue;
				}
			}
			goto IL_016a;
			IL_0378:
			Curve curve2 = m_Curve[subLane.m_SubLane];
			if (MathUtils.Distance(MathUtils.Bounds(curve2.m_Bezier), comparePosition) < num3)
			{
				float num7 = MathUtils.Distance(curve2.m_Bezier, comparePosition, ref num8);
				if (num7 < num3)
				{
					num3 = num7;
					delta = num8;
					val4 = subLane.m_SubLane;
					continue;
				}
			}
			goto IL_03d1;
			IL_02bb:
			if ((pathMethod & (PathMethod.Parking | PathMethod.Boarding | PathMethod.SpecialParking)) != 0)
			{
				PrefabRef prefabRef = m_PrefabRef[subLane.m_SubLane];
				if (m_ParkingLaneData.TryGetComponent(prefabRef.m_Prefab, ref parkingLaneData))
				{
					Game.Net.ParkingLane parkingLane = m_ParkingLane[subLane.m_SubLane];
					if ((m_SetupQueueTarget.m_RoadTypes & parkingLaneData.m_RoadTypes) != RoadTypes.None && CheckAccessRestriction(allowAccessRestriction, parkingLane))
					{
						goto IL_0378;
					}
				}
				else if (m_ConnectionLane.TryGetComponent(subLane.m_SubLane, ref connectionLane2) && (connectionLane2.m_Flags & ConnectionLaneFlags.Parking) != 0 && (connectionLane2.m_RoadTypes & m_SetupQueueTarget.m_RoadTypes) != RoadTypes.None && CheckAccessRestriction(allowAccessRestriction, connectionLane2))
				{
					goto IL_0378;
				}
			}
			goto IL_03d1;
			IL_016a:
			if ((pathMethod & PathMethod.Road) != 0)
			{
				PrefabRef prefabRef2 = m_PrefabRef[subLane.m_SubLane];
				if (m_CarLaneData.HasComponent(prefabRef2.m_Prefab) && !m_MasterLane.HasComponent(subLane.m_SubLane))
				{
					Game.Net.CarLane carLane = m_CarLane[subLane.m_SubLane];
					CarLaneData carLaneData = m_CarLaneData[prefabRef2.m_Prefab];
					if (VehicleUtils.CanUseLane(m_SetupQueueTarget.m_Methods, m_SetupQueueTarget.m_RoadTypes, carLaneData) && CheckAccessRestriction(allowAccessRestriction, carLane))
					{
						goto IL_0262;
					}
				}
				else if (m_ConnectionLane.HasComponent(subLane.m_SubLane))
				{
					Game.Net.ConnectionLane connectionLane3 = m_ConnectionLane[subLane.m_SubLane];
					if ((connectionLane3.m_Flags & ConnectionLaneFlags.Road) != 0 && (connectionLane3.m_RoadTypes & m_SetupQueueTarget.m_RoadTypes) != RoadTypes.None && CheckAccessRestriction(allowAccessRestriction, connectionLane3))
					{
						goto IL_0262;
					}
				}
			}
			goto IL_02bb;
			IL_03d1:
			if ((pathMethod & PathMethod.Track) == 0)
			{
				continue;
			}
			PrefabRef prefabRef3 = m_PrefabRef[subLane.m_SubLane];
			if (m_TrackLaneData.HasComponent(prefabRef3.m_Prefab))
			{
				TrackLaneData trackLaneData = m_TrackLaneData[prefabRef3.m_Prefab];
				if ((m_SetupQueueTarget.m_TrackTypes & trackLaneData.m_TrackTypes) == 0)
				{
					continue;
				}
			}
			else
			{
				if (!m_ConnectionLane.HasComponent(subLane.m_SubLane))
				{
					continue;
				}
				Game.Net.ConnectionLane connectionLane4 = m_ConnectionLane[subLane.m_SubLane];
				if ((connectionLane4.m_Flags & ConnectionLaneFlags.Track) == 0 || (connectionLane4.m_TrackTypes & m_SetupQueueTarget.m_TrackTypes) == 0)
				{
					continue;
				}
			}
			Curve curve3 = m_Curve[subLane.m_SubLane];
			if (MathUtils.Distance(MathUtils.Bounds(curve3.m_Bezier), comparePosition) < num4)
			{
				float num9 = MathUtils.Distance(curve3.m_Bezier, comparePosition, ref num10);
				if (num9 < num4)
				{
					num4 = num9;
					delta2 = num10;
					val5 = subLane.m_SubLane;
				}
			}
			continue;
			IL_0262:
			Curve curve4 = m_Curve[subLane.m_SubLane];
			if (MathUtils.Distance(MathUtils.Bounds(curve4.m_Bezier), comparePosition) < num2)
			{
				float num11 = MathUtils.Distance(curve4.m_Bezier, comparePosition, ref num12);
				if (num11 < num2)
				{
					num2 = num11;
					curvePos2 = num12;
					val3 = subLane.m_SubLane;
					continue;
				}
			}
			goto IL_02bb;
		}
		int num13 = 0;
		if (val2 != Entity.Null)
		{
			num13 += AddPedestrianLaneTargets(ref random, target, val2, curvePos, cost, num, flags, allowAccessRestriction);
		}
		if (val3 != Entity.Null)
		{
			num13 += AddCarLaneTargets(ref random, target, val3, comparePosition, maxDistance, curvePos2, cost, flags, allowLaneGroupSwitch, allowBlocked: false, allowAccessRestriction);
		}
		if (val4 != Entity.Null)
		{
			AddTarget(ref random, target, val4, delta, cost, flags);
			num13++;
		}
		if (val5 != Entity.Null)
		{
			AddTarget(ref random, target, val5, delta2, cost, flags);
			num13++;
		}
		return num13;
	}

	private int AddPedestrianLaneTargets(ref Random random, Entity target, Entity lane, float curvePos, float cost, float distance, EdgeFlags flags, bool allowAccessRestriction)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Game.Net.PedestrianLane pedestrianLane = default(Game.Net.PedestrianLane);
		Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
		if (m_PedestrianLane.TryGetComponent(lane, ref pedestrianLane))
		{
			if (!CheckAccessRestriction(allowAccessRestriction, pedestrianLane))
			{
				return 0;
			}
		}
		else if (m_ConnectionLane.TryGetComponent(lane, ref connectionLane) && !CheckAccessRestriction(allowAccessRestriction, connectionLane))
		{
			return 0;
		}
		float cost2 = cost + CalculatePedestrianTargetCost(ref random, distance);
		AddTarget(ref random, target, lane, curvePos, cost2, flags);
		return 1;
	}

	private int AddParkingLaneTargets(ref Random random, Entity target, Entity lane, float curvePos, float cost, EdgeFlags flags, bool allowAccessRestriction)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Game.Net.ParkingLane parkingLane = default(Game.Net.ParkingLane);
		Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
		if (m_ParkingLane.TryGetComponent(lane, ref parkingLane))
		{
			if (!CheckAccessRestriction(allowAccessRestriction, parkingLane))
			{
				return 0;
			}
		}
		else if (m_ConnectionLane.TryGetComponent(lane, ref connectionLane) && !CheckAccessRestriction(allowAccessRestriction, connectionLane))
		{
			return 0;
		}
		AddTarget(ref random, target, lane, curvePos, cost, flags);
		return 1;
	}

	private int AddCarLaneTargets(ref Random random, Entity target, Entity lane, float3 comparePosition, float maxDistance, float curvePos, float cost, EdgeFlags flags, bool allowLaneGroupSwitch, bool allowBlocked, bool allowAccessRestriction)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		if (!m_CarLane.HasComponent(lane))
		{
			if (m_ConnectionLane.HasComponent(lane))
			{
				Game.Net.ConnectionLane connectionLane = m_ConnectionLane[lane];
				if (!CheckAccessRestriction(allowAccessRestriction, connectionLane))
				{
					return 0;
				}
			}
			AddTarget(ref random, target, lane, curvePos, cost, flags);
			return 1;
		}
		Owner owner = m_Owner[lane];
		Game.Net.CarLane carLane = m_CarLane[lane];
		SlaveLane slaveLane = default(SlaveLane);
		if (!CheckAccessRestriction(allowAccessRestriction, carLane))
		{
			return 0;
		}
		PrefabRef prefabRef = m_PrefabRef[lane];
		NetLaneData netLaneData = m_NetLaneData[prefabRef.m_Prefab];
		PathfindCarData pathfindCarData = m_CarPathfindData[netLaneData.m_PathfindPrefab];
		float num = 0f;
		NodeLane nodeLane = default(NodeLane);
		if (m_NodeLane.TryGetComponent(lane, ref nodeLane))
		{
			num = (netLaneData.m_Width + math.lerp(nodeLane.m_WidthOffset.x, nodeLane.m_WidthOffset.y, curvePos)) * 0.5f;
		}
		bool flag = false;
		if (m_SlaveLane.HasComponent(lane))
		{
			slaveLane = m_SlaveLane[lane];
			num *= (float)(slaveLane.m_MaxIndex - slaveLane.m_MinIndex + 1);
			flag = true;
		}
		DynamicBuffer<Game.Net.SubLane> val = m_SubLane[owner.m_Owner];
		int num2 = slaveLane.m_MaxIndex - slaveLane.m_MinIndex + 1;
		int num3 = 0;
		NodeLane nodeLane2 = default(NodeLane);
		for (int i = 0; i < val.Length; i++)
		{
			Game.Net.SubLane subLane = val[i];
			if ((subLane.m_PathMethods & PathMethod.Road) == 0 || !m_CarLane.HasComponent(subLane.m_SubLane) || m_SlaveLane.HasComponent(subLane.m_SubLane))
			{
				continue;
			}
			Game.Net.CarLane carLaneData = m_CarLane[subLane.m_SubLane];
			if (carLaneData.m_CarriagewayGroup != carLane.m_CarriagewayGroup || carLaneData.m_AccessRestriction != carLane.m_AccessRestriction)
			{
				continue;
			}
			bool flag2;
			int num4;
			if (m_MasterLane.HasComponent(subLane.m_SubLane))
			{
				MasterLane masterLane = m_MasterLane[subLane.m_SubLane];
				flag2 = !flag || masterLane.m_Group != slaveLane.m_Group;
				num4 = masterLane.m_MaxIndex - masterLane.m_MinIndex + 1;
			}
			else
			{
				flag2 = subLane.m_SubLane != lane;
				num4 = 1;
			}
			float num5 = math.select(curvePos, 1f - curvePos, ((carLane.m_Flags ^ carLaneData.m_Flags) & Game.Net.CarLaneFlags.Invert) != 0);
			if (flag2)
			{
				if ((carLane.m_Flags & (Game.Net.CarLaneFlags.Approach | Game.Net.CarLaneFlags.Roundabout)) == Game.Net.CarLaneFlags.Roundabout)
				{
					continue;
				}
				if (num != 0f)
				{
					if (!m_NodeLane.TryGetComponent(subLane.m_SubLane, ref nodeLane2))
					{
						continue;
					}
					Curve curve = m_Curve[lane];
					Curve curve2 = m_Curve[subLane.m_SubLane];
					float3 val2 = MathUtils.Position(curve.m_Bezier, curvePos);
					float num6 = MathUtils.Distance(curve2.m_Bezier, val2, ref num5);
					PrefabRef prefabRef2 = m_PrefabRef[subLane.m_SubLane];
					float num7 = (m_NetLaneData[prefabRef2.m_Prefab].m_Width + math.lerp(nodeLane2.m_WidthOffset.x, nodeLane2.m_WidthOffset.y, num5)) * 0.5f * (float)num4;
					if (num6 > num + num7 + 3f)
					{
						continue;
					}
				}
			}
			if (carLaneData.m_BlockageEnd >= carLaneData.m_BlockageStart && (m_PathfindParameters.m_IgnoredRules & RuleFlags.HasBlockage) == 0)
			{
				Bounds1 blockageBounds = carLaneData.blockageBounds;
				if (maxDistance != 0f && (carLaneData.m_Flags & Game.Net.CarLaneFlags.Twoway) == 0 && carLaneData.m_BlockageStart > 0 && math.distance(MathUtils.Position(m_Curve[subLane.m_SubLane].m_Bezier, blockageBounds.min), comparePosition) <= maxDistance)
				{
					num5 = math.max(0f, blockageBounds.min - 0.01f);
				}
				else if (MathUtils.Intersect(blockageBounds, num5))
				{
					if (((num5 - blockageBounds.min < blockageBounds.max - num5 && (carLaneData.m_Flags & Game.Net.CarLaneFlags.Twoway) != 0) || carLaneData.m_BlockageEnd == byte.MaxValue) && carLaneData.m_BlockageStart > 0)
					{
						num5 = math.max(0f, blockageBounds.min - 0.01f);
					}
					else
					{
						if (carLaneData.m_BlockageEnd >= byte.MaxValue || (!allowBlocked && blockageBounds.max + 0.01f > num5))
						{
							continue;
						}
						num5 = math.min(1f, blockageBounds.max + 0.01f);
					}
				}
			}
			float distance = math.distance(comparePosition, MathUtils.Position(m_Curve[subLane.m_SubLane].m_Bezier, curvePos));
			float cost2 = cost + CalculateCarTargetCost(ref random, pathfindCarData, carLaneData, distance, math.select(0, num2, flag2), !allowLaneGroupSwitch && flag2);
			AddTarget(ref random, target, subLane.m_SubLane, num5, cost2, flags);
			num3++;
		}
		return num3;
	}

	private float CalculatePedestrianTargetCost(ref Random random, float distance)
	{
		PathSpecification pathSpecification = new PathSpecification
		{
			m_Flags = (EdgeFlags.Forward | EdgeFlags.Backward),
			m_Methods = PathMethod.Pedestrian,
			m_Length = distance,
			m_MaxSpeed = 5.555556f,
			m_Density = 0f,
			m_AccessRequirement = -1
		};
		return PathUtils.CalculateCost(ref random, in pathSpecification, in m_PathfindParameters);
	}

	private float CalculateCarTargetCost(ref Random random, PathfindCarData pathfindCarData, Game.Net.CarLane carLaneData, float distance, int laneCrossCount, bool unsafeUTurn)
	{
		PathSpecification pathSpecification = new PathSpecification
		{
			m_Flags = (EdgeFlags.Forward | EdgeFlags.Backward),
			m_Methods = PathMethod.Road,
			m_Length = distance,
			m_MaxSpeed = carLaneData.m_SpeedLimit,
			m_Density = 0f,
			m_AccessRequirement = -1
		};
		PathUtils.TryAddCosts(ref pathSpecification.m_Costs, pathfindCarData.m_LaneCrossCost, (float)laneCrossCount);
		PathUtils.TryAddCosts(ref pathSpecification.m_Costs, pathfindCarData.m_UnsafeUTurnCost, unsafeUTurn);
		return PathUtils.CalculateCost(ref random, in pathSpecification, in m_PathfindParameters);
	}
}

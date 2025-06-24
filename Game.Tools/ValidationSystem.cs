using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Events;
using Game.Net;
using Game.Notifications;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ValidationSystem : GameSystemBase
{
	public struct ChunkType
	{
		public EntityTypeHandle m_Entity;

		public ComponentTypeHandle<Temp> m_Temp;

		public ComponentTypeHandle<Owner> m_Owner;

		public ComponentTypeHandle<Native> m_Native;

		public ComponentTypeHandle<Brush> m_Brush;

		public ComponentTypeHandle<PrefabRef> m_PrefabRef;

		public ComponentTypeHandle<Game.Objects.Object> m_Object;

		public ComponentTypeHandle<Transform> m_Transform;

		public ComponentTypeHandle<Attached> m_Attached;

		public ComponentTypeHandle<Game.Objects.NetObject> m_NetObject;

		public ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnection;

		public ComponentTypeHandle<Building> m_Building;

		public ComponentTypeHandle<Game.Buildings.ServiceUpgrade> m_ServiceUpgrade;

		public ComponentTypeHandle<Game.Routes.TransportStop> m_TransportStop;

		public ComponentTypeHandle<Game.Net.Edge> m_Edge;

		public ComponentTypeHandle<EdgeGeometry> m_EdgeGeometry;

		public ComponentTypeHandle<StartNodeGeometry> m_StartNodeGeometry;

		public ComponentTypeHandle<EndNodeGeometry> m_EndNodeGeometry;

		public ComponentTypeHandle<Composition> m_Composition;

		public ComponentTypeHandle<Lane> m_Lane;

		public ComponentTypeHandle<Game.Net.TrackLane> m_TrackLane;

		public ComponentTypeHandle<Curve> m_Curve;

		public ComponentTypeHandle<EdgeLane> m_EdgeLane;

		public ComponentTypeHandle<Fixed> m_Fixed;

		public ComponentTypeHandle<Area> m_Area;

		public ComponentTypeHandle<Geometry> m_AreaGeometry;

		public ComponentTypeHandle<Storage> m_AreaStorage;

		public BufferTypeHandle<Game.Areas.Node> m_AreaNode;

		public BufferTypeHandle<RouteWaypoint> m_RouteWaypoint;

		public BufferTypeHandle<RouteSegment> m_RouteSegment;

		public ComponentTypeHandle<Game.Simulation.WaterSourceData> m_WaterSourceData;

		public ChunkType(SystemBase system)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
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
			m_Entity = ((ComponentSystemBase)system).GetEntityTypeHandle();
			m_Temp = ((ComponentSystemBase)system).GetComponentTypeHandle<Temp>(true);
			m_Owner = ((ComponentSystemBase)system).GetComponentTypeHandle<Owner>(true);
			m_Native = ((ComponentSystemBase)system).GetComponentTypeHandle<Native>(true);
			m_Brush = ((ComponentSystemBase)system).GetComponentTypeHandle<Brush>(true);
			m_PrefabRef = ((ComponentSystemBase)system).GetComponentTypeHandle<PrefabRef>(true);
			m_Object = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Objects.Object>(true);
			m_Transform = ((ComponentSystemBase)system).GetComponentTypeHandle<Transform>(true);
			m_Attached = ((ComponentSystemBase)system).GetComponentTypeHandle<Attached>(true);
			m_NetObject = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Objects.NetObject>(true);
			m_OutsideConnection = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Objects.OutsideConnection>(true);
			m_Building = ((ComponentSystemBase)system).GetComponentTypeHandle<Building>(true);
			m_ServiceUpgrade = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Buildings.ServiceUpgrade>(true);
			m_TransportStop = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Routes.TransportStop>(true);
			m_Edge = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Net.Edge>(true);
			m_EdgeGeometry = ((ComponentSystemBase)system).GetComponentTypeHandle<EdgeGeometry>(true);
			m_StartNodeGeometry = ((ComponentSystemBase)system).GetComponentTypeHandle<StartNodeGeometry>(true);
			m_EndNodeGeometry = ((ComponentSystemBase)system).GetComponentTypeHandle<EndNodeGeometry>(true);
			m_Composition = ((ComponentSystemBase)system).GetComponentTypeHandle<Composition>(true);
			m_Lane = ((ComponentSystemBase)system).GetComponentTypeHandle<Lane>(true);
			m_TrackLane = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Net.TrackLane>(true);
			m_Curve = ((ComponentSystemBase)system).GetComponentTypeHandle<Curve>(true);
			m_EdgeLane = ((ComponentSystemBase)system).GetComponentTypeHandle<EdgeLane>(true);
			m_Fixed = ((ComponentSystemBase)system).GetComponentTypeHandle<Fixed>(true);
			m_Area = ((ComponentSystemBase)system).GetComponentTypeHandle<Area>(true);
			m_AreaGeometry = ((ComponentSystemBase)system).GetComponentTypeHandle<Geometry>(true);
			m_AreaStorage = ((ComponentSystemBase)system).GetComponentTypeHandle<Storage>(true);
			m_AreaNode = ((ComponentSystemBase)system).GetBufferTypeHandle<Game.Areas.Node>(true);
			m_RouteWaypoint = ((ComponentSystemBase)system).GetBufferTypeHandle<RouteWaypoint>(true);
			m_RouteSegment = ((ComponentSystemBase)system).GetBufferTypeHandle<RouteSegment>(true);
			m_WaterSourceData = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Simulation.WaterSourceData>(true);
		}

		public void Update(SystemBase system)
		{
			((EntityTypeHandle)(ref m_Entity)).Update(system);
			m_Temp.Update(system);
			m_Owner.Update(system);
			m_Native.Update(system);
			m_Brush.Update(system);
			m_PrefabRef.Update(system);
			m_Object.Update(system);
			m_Transform.Update(system);
			m_Attached.Update(system);
			m_NetObject.Update(system);
			m_OutsideConnection.Update(system);
			m_Building.Update(system);
			m_ServiceUpgrade.Update(system);
			m_TransportStop.Update(system);
			m_Edge.Update(system);
			m_EdgeGeometry.Update(system);
			m_StartNodeGeometry.Update(system);
			m_EndNodeGeometry.Update(system);
			m_Composition.Update(system);
			m_Lane.Update(system);
			m_TrackLane.Update(system);
			m_Curve.Update(system);
			m_EdgeLane.Update(system);
			m_Fixed.Update(system);
			m_Area.Update(system);
			m_AreaGeometry.Update(system);
			m_AreaStorage.Update(system);
			m_AreaNode.Update(system);
			m_RouteWaypoint.Update(system);
			m_RouteSegment.Update(system);
			m_WaterSourceData.Update(system);
		}
	}

	public struct EntityData
	{
		public ComponentLookup<Owner> m_Owner;

		public ComponentLookup<Hidden> m_Hidden;

		public ComponentLookup<Temp> m_Temp;

		public ComponentLookup<Native> m_Native;

		public ComponentLookup<Transform> m_Transform;

		public ComponentLookup<Game.Objects.Elevation> m_ObjectElevation;

		public ComponentLookup<Secondary> m_Secondary;

		public ComponentLookup<AssetStamp> m_AssetStamp;

		public ComponentLookup<Attachment> m_Attachment;

		public ComponentLookup<Attached> m_Attached;

		public ComponentLookup<Stack> m_Stack;

		public ComponentLookup<Building> m_Building;

		public BufferLookup<InstalledUpgrade> m_Upgrades;

		public ComponentLookup<Game.Net.Node> m_Node;

		public ComponentLookup<Game.Net.Edge> m_Edge;

		public ComponentLookup<EdgeGeometry> m_EdgeGeometry;

		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometry;

		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometry;

		public ComponentLookup<Composition> m_Composition;

		public ComponentLookup<Game.Net.Elevation> m_NetElevation;

		public ComponentLookup<Lane> m_Lane;

		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLane;

		public ComponentLookup<Game.Net.CarLane> m_CarLane;

		public ComponentLookup<Game.Net.TrackLane> m_TrackLane;

		public ComponentLookup<Curve> m_Curve;

		public BufferLookup<Game.Net.SubLane> m_Lanes;

		public BufferLookup<ConnectedNode> m_ConnectedNodes;

		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		public ComponentLookup<Area> m_Area;

		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		public BufferLookup<Triangle> m_AreaTriangles;

		public ComponentLookup<PathInformation> m_PathInformation;

		public ComponentLookup<Route> m_Route;

		public ComponentLookup<Connected> m_RouteConnected;

		public ComponentLookup<OnFire> m_OnFire;

		public ComponentLookup<PrefabRef> m_PrefabRef;

		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometry;

		public ComponentLookup<BuildingData> m_PrefabBuilding;

		public ComponentLookup<PlaceableObjectData> m_PlaceableObject;

		public ComponentLookup<StackData> m_PrefabStackData;

		public ComponentLookup<NetObjectData> m_PrefabNetObject;

		public ComponentLookup<PlaceableNetData> m_PlaceableNet;

		public ComponentLookup<NetCompositionData> m_PrefabComposition;

		public ComponentLookup<NetGeometryData> m_PrefabNetGeometry;

		public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometry;

		public ComponentLookup<StorageAreaData> m_PrefabStorageArea;

		public ComponentLookup<LotData> m_PrefabLotData;

		public ComponentLookup<CarLaneData> m_CarLaneData;

		public ComponentLookup<TrackLaneData> m_TrackLaneData;

		public ComponentLookup<RouteConnectionData> m_RouteConnectionData;

		public ComponentLookup<TransportStopData> m_TransportStopData;

		public ComponentLookup<TransportLineData> m_TransportLineData;

		public ComponentLookup<WaterPumpingStationData> m_WaterPumpingStationData;

		public ComponentLookup<GroundWaterPoweredData> m_GroundWaterPoweredData;

		public ComponentLookup<TerraformingData> m_TerraformingData;

		public ComponentLookup<ServiceUpgradeData> m_ServiceUpgradeData;

		public BufferLookup<NetCompositionArea> m_PrefabCompositionAreas;

		public BufferLookup<FixedNetElement> m_PrefabFixedElements;

		public EntityData(SystemBase system)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			m_Owner = system.GetComponentLookup<Owner>(true);
			m_Hidden = system.GetComponentLookup<Hidden>(true);
			m_Temp = system.GetComponentLookup<Temp>(true);
			m_Native = system.GetComponentLookup<Native>(true);
			m_Transform = system.GetComponentLookup<Transform>(true);
			m_ObjectElevation = system.GetComponentLookup<Game.Objects.Elevation>(true);
			m_Secondary = system.GetComponentLookup<Secondary>(true);
			m_AssetStamp = system.GetComponentLookup<AssetStamp>(true);
			m_Attachment = system.GetComponentLookup<Attachment>(true);
			m_Attached = system.GetComponentLookup<Attached>(true);
			m_Stack = system.GetComponentLookup<Stack>(true);
			m_Building = system.GetComponentLookup<Building>(true);
			m_Upgrades = system.GetBufferLookup<InstalledUpgrade>(true);
			m_Node = system.GetComponentLookup<Game.Net.Node>(true);
			m_Edge = system.GetComponentLookup<Game.Net.Edge>(true);
			m_EdgeGeometry = system.GetComponentLookup<EdgeGeometry>(true);
			m_StartNodeGeometry = system.GetComponentLookup<StartNodeGeometry>(true);
			m_EndNodeGeometry = system.GetComponentLookup<EndNodeGeometry>(true);
			m_Composition = system.GetComponentLookup<Composition>(true);
			m_NetElevation = system.GetComponentLookup<Game.Net.Elevation>(true);
			m_Lane = system.GetComponentLookup<Lane>(true);
			m_PedestrianLane = system.GetComponentLookup<Game.Net.PedestrianLane>(true);
			m_CarLane = system.GetComponentLookup<Game.Net.CarLane>(true);
			m_TrackLane = system.GetComponentLookup<Game.Net.TrackLane>(true);
			m_Curve = system.GetComponentLookup<Curve>(true);
			m_Lanes = system.GetBufferLookup<Game.Net.SubLane>(true);
			m_ConnectedNodes = system.GetBufferLookup<ConnectedNode>(true);
			m_ConnectedEdges = system.GetBufferLookup<ConnectedEdge>(true);
			m_Area = system.GetComponentLookup<Area>(true);
			m_AreaNodes = system.GetBufferLookup<Game.Areas.Node>(true);
			m_AreaTriangles = system.GetBufferLookup<Triangle>(true);
			m_PathInformation = system.GetComponentLookup<PathInformation>(true);
			m_Route = system.GetComponentLookup<Route>(true);
			m_RouteConnected = system.GetComponentLookup<Connected>(true);
			m_OnFire = system.GetComponentLookup<OnFire>(true);
			m_PrefabRef = system.GetComponentLookup<PrefabRef>(true);
			m_PrefabObjectGeometry = system.GetComponentLookup<ObjectGeometryData>(true);
			m_PrefabBuilding = system.GetComponentLookup<BuildingData>(true);
			m_PlaceableObject = system.GetComponentLookup<PlaceableObjectData>(true);
			m_PrefabStackData = system.GetComponentLookup<StackData>(true);
			m_PrefabNetObject = system.GetComponentLookup<NetObjectData>(true);
			m_PlaceableNet = system.GetComponentLookup<PlaceableNetData>(true);
			m_PrefabComposition = system.GetComponentLookup<NetCompositionData>(true);
			m_PrefabNetGeometry = system.GetComponentLookup<NetGeometryData>(true);
			m_PrefabAreaGeometry = system.GetComponentLookup<AreaGeometryData>(true);
			m_PrefabStorageArea = system.GetComponentLookup<StorageAreaData>(true);
			m_PrefabLotData = system.GetComponentLookup<LotData>(true);
			m_CarLaneData = system.GetComponentLookup<CarLaneData>(true);
			m_TrackLaneData = system.GetComponentLookup<TrackLaneData>(true);
			m_RouteConnectionData = system.GetComponentLookup<RouteConnectionData>(true);
			m_TransportStopData = system.GetComponentLookup<TransportStopData>(true);
			m_TransportLineData = system.GetComponentLookup<TransportLineData>(true);
			m_WaterPumpingStationData = system.GetComponentLookup<WaterPumpingStationData>(true);
			m_GroundWaterPoweredData = system.GetComponentLookup<GroundWaterPoweredData>(true);
			m_TerraformingData = system.GetComponentLookup<TerraformingData>(true);
			m_ServiceUpgradeData = system.GetComponentLookup<ServiceUpgradeData>(true);
			m_PrefabCompositionAreas = system.GetBufferLookup<NetCompositionArea>(true);
			m_PrefabFixedElements = system.GetBufferLookup<FixedNetElement>(true);
		}

		public void Update(SystemBase system)
		{
			m_Owner.Update(system);
			m_Hidden.Update(system);
			m_Temp.Update(system);
			m_Native.Update(system);
			m_Transform.Update(system);
			m_ObjectElevation.Update(system);
			m_Secondary.Update(system);
			m_AssetStamp.Update(system);
			m_Attachment.Update(system);
			m_Attached.Update(system);
			m_Stack.Update(system);
			m_Building.Update(system);
			m_Upgrades.Update(system);
			m_Node.Update(system);
			m_Edge.Update(system);
			m_EdgeGeometry.Update(system);
			m_StartNodeGeometry.Update(system);
			m_EndNodeGeometry.Update(system);
			m_Composition.Update(system);
			m_NetElevation.Update(system);
			m_Lane.Update(system);
			m_PedestrianLane.Update(system);
			m_CarLane.Update(system);
			m_TrackLane.Update(system);
			m_Curve.Update(system);
			m_Lanes.Update(system);
			m_ConnectedNodes.Update(system);
			m_ConnectedEdges.Update(system);
			m_Area.Update(system);
			m_AreaNodes.Update(system);
			m_AreaTriangles.Update(system);
			m_PathInformation.Update(system);
			m_Route.Update(system);
			m_RouteConnected.Update(system);
			m_OnFire.Update(system);
			m_PrefabRef.Update(system);
			m_PrefabObjectGeometry.Update(system);
			m_PrefabBuilding.Update(system);
			m_PlaceableObject.Update(system);
			m_PrefabStackData.Update(system);
			m_PrefabNetObject.Update(system);
			m_PlaceableNet.Update(system);
			m_PrefabComposition.Update(system);
			m_PrefabNetGeometry.Update(system);
			m_PrefabAreaGeometry.Update(system);
			m_PrefabStorageArea.Update(system);
			m_PrefabLotData.Update(system);
			m_CarLaneData.Update(system);
			m_TrackLaneData.Update(system);
			m_RouteConnectionData.Update(system);
			m_TransportStopData.Update(system);
			m_TransportLineData.Update(system);
			m_WaterPumpingStationData.Update(system);
			m_GroundWaterPoweredData.Update(system);
			m_TerraformingData.Update(system);
			m_ServiceUpgradeData.Update(system);
			m_PrefabCompositionAreas.Update(system);
			m_PrefabFixedElements.Update(system);
		}
	}

	[CompilerGenerated]
	public class Components : GameSystemBase
	{
		[BurstCompile]
		private struct UpdateComponentsJob : IJob
		{
			[ReadOnly]
			public EntityTypeHandle m_EntityType;

			[ReadOnly]
			public ComponentTypeHandle<Error> m_ErrorType;

			[ReadOnly]
			public ComponentTypeHandle<Warning> m_WarningType;

			[ReadOnly]
			public ComponentTypeHandle<Override> m_OverrideType;

			[ReadOnly]
			public NativeList<ArchetypeChunk> m_Chunks;

			[NativeDisableContainerSafetyRestriction]
			public NativeHashMap<Entity, ErrorSeverity> m_ErrorMap;

			public EntityCommandBuffer m_CommandBuffer;

			public void Execute()
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0226: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0248: Unknown result type (might be due to invalid IL or missing references)
				//IL_0164: Unknown result type (might be due to invalid IL or missing references)
				//IL_016d: Unknown result type (might be due to invalid IL or missing references)
				//IL_026a: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				//IL_028c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0110: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0199: Unknown result type (might be due to invalid IL or missing references)
				//IL_019e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_0133: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0360: Unknown result type (might be due to invalid IL or missing references)
				//IL_0365: Unknown result type (might be due to invalid IL or missing references)
				//IL_0369: Unknown result type (might be due to invalid IL or missing references)
				//IL_036e: Unknown result type (might be due to invalid IL or missing references)
				//IL_037b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0380: Unknown result type (might be due to invalid IL or missing references)
				//IL_0384: Unknown result type (might be due to invalid IL or missing references)
				//IL_0389: Unknown result type (might be due to invalid IL or missing references)
				//IL_0328: Unknown result type (might be due to invalid IL or missing references)
				//IL_032d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0331: Unknown result type (might be due to invalid IL or missing references)
				//IL_0336: Unknown result type (might be due to invalid IL or missing references)
				//IL_0343: Unknown result type (might be due to invalid IL or missing references)
				//IL_0348: Unknown result type (might be due to invalid IL or missing references)
				//IL_034c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0351: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_030b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0310: Unknown result type (might be due to invalid IL or missing references)
				//IL_0314: Unknown result type (might be due to invalid IL or missing references)
				//IL_0319: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_0407: Unknown result type (might be due to invalid IL or missing references)
				NativeList<Entity> val = default(NativeList<Entity>);
				val._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<Entity> val2 = default(NativeList<Entity>);
				val2._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<Entity> val3 = default(NativeList<Entity>);
				val3._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<Entity> val4 = default(NativeList<Entity>);
				val4._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				ErrorSeverity errorSeverity = default(ErrorSeverity);
				ErrorSeverity errorSeverity2 = default(ErrorSeverity);
				ErrorSeverity errorSeverity3 = default(ErrorSeverity);
				for (int i = 0; i < m_Chunks.Length; i++)
				{
					ArchetypeChunk val5 = m_Chunks[i];
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val5)).GetNativeArray(m_EntityType);
					if (((ArchetypeChunk)(ref val5)).Has<Error>(ref m_ErrorType))
					{
						if (m_ErrorMap.IsCreated)
						{
							for (int j = 0; j < nativeArray.Length; j++)
							{
								Entity val6 = nativeArray[j];
								if (m_ErrorMap.TryGetValue(val6, ref errorSeverity) && errorSeverity == ErrorSeverity.Error)
								{
									m_ErrorMap.Remove(val6);
									continue;
								}
								val.Add(ref val6);
								val4.Add(ref val6);
							}
						}
						else
						{
							val.AddRange(nativeArray);
							val4.AddRange(nativeArray);
						}
					}
					if (((ArchetypeChunk)(ref val5)).Has<Warning>(ref m_WarningType))
					{
						if (m_ErrorMap.IsCreated)
						{
							for (int k = 0; k < nativeArray.Length; k++)
							{
								Entity val7 = nativeArray[k];
								if (m_ErrorMap.TryGetValue(val7, ref errorSeverity2) && errorSeverity2 == ErrorSeverity.Warning)
								{
									m_ErrorMap.Remove(val7);
									continue;
								}
								val2.Add(ref val7);
								val4.Add(ref val7);
							}
						}
						else
						{
							val2.AddRange(nativeArray);
							val4.AddRange(nativeArray);
						}
					}
					if (!((ArchetypeChunk)(ref val5)).Has<Override>(ref m_OverrideType))
					{
						continue;
					}
					if (m_ErrorMap.IsCreated)
					{
						for (int l = 0; l < nativeArray.Length; l++)
						{
							Entity val8 = nativeArray[l];
							if (m_ErrorMap.TryGetValue(val8, ref errorSeverity3) && errorSeverity3 == ErrorSeverity.Override)
							{
								m_ErrorMap.Remove(val8);
								continue;
							}
							val3.Add(ref val8);
							val4.Add(ref val8);
						}
					}
					else
					{
						val3.AddRange(nativeArray);
						val4.AddRange(nativeArray);
					}
				}
				if (val4.Length != 0)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(val4.AsArray());
					val4.Clear();
				}
				if (val.Length != 0)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Error>(val.AsArray());
					val.Clear();
				}
				if (val2.Length != 0)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Warning>(val2.AsArray());
					val2.Clear();
				}
				if (val3.Length != 0)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Override>(val3.AsArray());
					val3.Clear();
				}
				if (m_ErrorMap.IsCreated)
				{
					Enumerator<Entity, ErrorSeverity> enumerator = m_ErrorMap.GetEnumerator();
					while (enumerator.MoveNext())
					{
						switch (enumerator.Current.Value)
						{
						case ErrorSeverity.Error:
						{
							Entity key = enumerator.Current.Key;
							val.Add(ref key);
							key = enumerator.Current.Key;
							val4.Add(ref key);
							break;
						}
						case ErrorSeverity.Warning:
						{
							Entity key = enumerator.Current.Key;
							val2.Add(ref key);
							key = enumerator.Current.Key;
							val4.Add(ref key);
							break;
						}
						case ErrorSeverity.Override:
						{
							Entity key = enumerator.Current.Key;
							val3.Add(ref key);
							key = enumerator.Current.Key;
							val4.Add(ref key);
							break;
						}
						}
					}
					enumerator.Dispose();
					if (val.Length != 0)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Error>(val.AsArray());
					}
					if (val2.Length != 0)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Warning>(val2.AsArray());
					}
					if (val3.Length != 0)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Override>(val3.AsArray());
					}
					if (val4.Length != 0)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(val4.AsArray());
					}
				}
				val.Dispose();
				val2.Dispose();
				val3.Dispose();
				val4.Dispose();
			}
		}

		private struct TypeHandle
		{
			[ReadOnly]
			public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

			[ReadOnly]
			public ComponentTypeHandle<Error> __Game_Tools_Error_RO_ComponentTypeHandle;

			[ReadOnly]
			public ComponentTypeHandle<Warning> __Game_Tools_Warning_RO_ComponentTypeHandle;

			[ReadOnly]
			public ComponentTypeHandle<Override> __Game_Tools_Override_RO_ComponentTypeHandle;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void __AssignHandles(ref SystemState state)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
				__Game_Tools_Error_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Error>(true);
				__Game_Tools_Warning_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Warning>(true);
				__Game_Tools_Override_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Override>(true);
			}
		}

		private ToolSystem m_ToolSystem;

		private ModificationEndBarrier m_ModificationBarrier;

		private EntityQuery m_ComponentQuery;

		public NativeHashMap<Entity, ErrorSeverity> m_ErrorMap;

		public JobHandle m_ErrorMapDeps;

		private TypeHandle __TypeHandle;

		[Preserve]
		protected override void OnCreate()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			base.OnCreate();
			m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
			m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationEndBarrier>();
			EntityQueryDesc[] array = new EntityQueryDesc[1];
			EntityQueryDesc val = new EntityQueryDesc();
			val.Any = (ComponentType[])(object)new ComponentType[3]
			{
				ComponentType.ReadOnly<Error>(),
				ComponentType.ReadOnly<Warning>(),
				ComponentType.ReadOnly<Override>()
			};
			val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
			array[0] = val;
			m_ComponentQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		}

		[Preserve]
		protected override void OnUpdate()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			if (m_ErrorMap.IsCreated || (m_ToolSystem.applyMode != ApplyMode.None && !((EntityQuery)(ref m_ComponentQuery)).IsEmptyIgnoreFilter))
			{
				JobHandle val = default(JobHandle);
				NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_ComponentQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
				JobHandle val2 = IJobExtensions.Schedule<UpdateComponentsJob>(new UpdateComponentsJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ErrorType = InternalCompilerInterface.GetComponentTypeHandle<Error>(ref __TypeHandle.__Game_Tools_Error_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_WarningType = InternalCompilerInterface.GetComponentTypeHandle<Warning>(ref __TypeHandle.__Game_Tools_Warning_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OverrideType = InternalCompilerInterface.GetComponentTypeHandle<Override>(ref __TypeHandle.__Game_Tools_Override_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_Chunks = chunks,
					m_ErrorMap = m_ErrorMap,
					m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
				}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_ErrorMapDeps, val));
				if (m_ErrorMap.IsCreated)
				{
					m_ErrorMap.Dispose(val2);
				}
				chunks.Dispose(val2);
				((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
				((SystemBase)this).Dependency = val2;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void __AssignQueries(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			EntityQueryBuilder val = default(EntityQueryBuilder);
			((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
			((EntityQueryBuilder)(ref val)).Dispose();
		}

		protected override void OnCreateForCompiler()
		{
			((ComponentSystemBase)this).OnCreateForCompiler();
			__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
			__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
		}

		[Preserve]
		public Components()
		{
		}
	}

	public struct BoundsData
	{
		public Bounds3 m_Bounds;

		public Entity m_Entity;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct BoundsComparerX : IComparer<BoundsData>
	{
		public int Compare(BoundsData x, BoundsData y)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			return math.select(math.select(1, -1, x.m_Bounds.min.x < y.m_Bounds.min.x), 0, x.m_Bounds.min.x == y.m_Bounds.min.x);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct BoundsComparerZ : IComparer<BoundsData>
	{
		public int Compare(BoundsData x, BoundsData y)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			return math.select(math.select(1, -1, x.m_Bounds.min.z < y.m_Bounds.min.z), 0, x.m_Bounds.min.z == y.m_Bounds.min.z);
		}
	}

	[BurstCompile]
	private struct BoundsListJob : IJob
	{
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ChunkType m_ChunkType;

		[ReadOnly]
		public EntityData m_EntityData;

		public NativeList<BoundsData> m_EdgeList;

		public NativeList<BoundsData> m_ObjectList;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			Bounds3 val = default(Bounds3);
			((Bounds3)(ref val))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			Bounds3 val2 = default(Bounds3);
			((Bounds3)(ref val2))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			ObjectGeometryData geometryData = default(ObjectGeometryData);
			Stack stack = default(Stack);
			StackData stackData = default(StackData);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val3 = m_Chunks[i];
				NativeArray<Transform> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<Transform>(ref m_ChunkType.m_Transform);
				NativeArray<EdgeGeometry> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<EdgeGeometry>(ref m_ChunkType.m_EdgeGeometry);
				if (nativeArray2.Length != 0)
				{
					NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref val3)).GetNativeArray(m_ChunkType.m_Entity);
					NativeArray<Temp> nativeArray4 = ((ArchetypeChunk)(ref val3)).GetNativeArray<Temp>(ref m_ChunkType.m_Temp);
					NativeArray<StartNodeGeometry> nativeArray5 = ((ArchetypeChunk)(ref val3)).GetNativeArray<StartNodeGeometry>(ref m_ChunkType.m_StartNodeGeometry);
					NativeArray<EndNodeGeometry> nativeArray6 = ((ArchetypeChunk)(ref val3)).GetNativeArray<EndNodeGeometry>(ref m_ChunkType.m_EndNodeGeometry);
					for (int j = 0; j < nativeArray3.Length; j++)
					{
						if ((nativeArray4[j].m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) == 0)
						{
							EdgeGeometry edgeGeometry = nativeArray2[j];
							StartNodeGeometry startNodeGeometry = nativeArray5[j];
							EndNodeGeometry endNodeGeometry = nativeArray6[j];
							BoundsData boundsData = new BoundsData
							{
								m_Entity = nativeArray3[j],
								m_Bounds = (edgeGeometry.m_Bounds | startNodeGeometry.m_Geometry.m_Bounds | endNodeGeometry.m_Geometry.m_Bounds)
							};
							if (math.any(math.isnan(boundsData.m_Bounds.min) | math.isnan(boundsData.m_Bounds.max)))
							{
								Debug.LogWarning((object)$"Edge has NaN bounds: {boundsData.m_Entity.Index}");
								continue;
							}
							m_EdgeList.Add(ref boundsData);
							val |= boundsData.m_Bounds;
						}
					}
				}
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<Entity> nativeArray7 = ((ArchetypeChunk)(ref val3)).GetNativeArray(m_ChunkType.m_Entity);
				NativeArray<Temp> nativeArray8 = ((ArchetypeChunk)(ref val3)).GetNativeArray<Temp>(ref m_ChunkType.m_Temp);
				NativeArray<PrefabRef> nativeArray9 = ((ArchetypeChunk)(ref val3)).GetNativeArray<PrefabRef>(ref m_ChunkType.m_PrefabRef);
				for (int k = 0; k < nativeArray7.Length; k++)
				{
					if ((nativeArray8[k].m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) != 0)
					{
						continue;
					}
					PrefabRef prefabRef = nativeArray9[k];
					if (m_EntityData.m_PrefabObjectGeometry.TryGetComponent(prefabRef.m_Prefab, ref geometryData))
					{
						BoundsData boundsData2 = new BoundsData
						{
							m_Entity = nativeArray7[k]
						};
						Transform transform = nativeArray[k];
						if (m_EntityData.m_Stack.TryGetComponent(boundsData2.m_Entity, ref stack) && m_EntityData.m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData))
						{
							boundsData2.m_Bounds = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, stack, geometryData, stackData);
						}
						else
						{
							boundsData2.m_Bounds = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, geometryData);
						}
						if (math.any(math.isnan(boundsData2.m_Bounds.min) | math.isnan(boundsData2.m_Bounds.max)))
						{
							Debug.LogWarning((object)$"Object has NaN bounds: {boundsData2.m_Entity.Index}");
							continue;
						}
						m_ObjectList.Add(ref boundsData2);
						val2 |= boundsData2.m_Bounds;
					}
				}
			}
			if (m_EdgeList.Length >= 2)
			{
				float3 val4 = MathUtils.Size(val);
				if (val4.z > val4.x)
				{
					NativeSortExtension.Sort<BoundsData, BoundsComparerZ>(m_EdgeList, default(BoundsComparerZ));
				}
				else
				{
					NativeSortExtension.Sort<BoundsData, BoundsComparerX>(m_EdgeList, default(BoundsComparerX));
				}
			}
			if (m_ObjectList.Length >= 2)
			{
				float3 val5 = MathUtils.Size(val2);
				if (val5.z > val5.x)
				{
					NativeSortExtension.Sort<BoundsData, BoundsComparerZ>(m_ObjectList, default(BoundsComparerZ));
				}
				else
				{
					NativeSortExtension.Sort<BoundsData, BoundsComparerX>(m_ObjectList, default(BoundsComparerX));
				}
			}
		}
	}

	[BurstCompile]
	private struct ValidationJob : IJobParallelForDefer
	{
		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ChunkType m_ChunkType;

		[ReadOnly]
		public EntityData m_EntityData;

		[ReadOnly]
		public NativeList<BoundsData> m_EdgeList;

		[ReadOnly]
		public NativeList<BoundsData> m_ObjectList;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		[ReadOnly]
		public NativeParallelHashMap<Entity, int> m_InstanceCounts;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public NativeArray<GroundWater> m_GroundWaterMap;

		public ParallelWriter<ErrorData> m_ErrorQueue;

		[NativeDisableContainerSafetyRestriction]
		private NativeList<ConnectedNode> m_TempNodes;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_059b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_0763: Unknown result type (might be due to invalid IL or missing references)
			//IL_0768: Unknown result type (might be due to invalid IL or missing references)
			//IL_076d: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08de: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0906: Unknown result type (might be due to invalid IL or missing references)
			//IL_090b: Unknown result type (might be due to invalid IL or missing references)
			//IL_091a: Unknown result type (might be due to invalid IL or missing references)
			//IL_091f: Unknown result type (might be due to invalid IL or missing references)
			//IL_092e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0933: Unknown result type (might be due to invalid IL or missing references)
			//IL_0942: Unknown result type (might be due to invalid IL or missing references)
			//IL_0947: Unknown result type (might be due to invalid IL or missing references)
			//IL_0956: Unknown result type (might be due to invalid IL or missing references)
			//IL_095b: Unknown result type (might be due to invalid IL or missing references)
			//IL_096a: Unknown result type (might be due to invalid IL or missing references)
			//IL_096f: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0adf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0820: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0996: Unknown result type (might be due to invalid IL or missing references)
			//IL_099b: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_089c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0702: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_070e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0720: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			ArchetypeChunk val = m_Chunks[index];
			TempFlags tempFlags = (((ArchetypeChunk)(ref val)).Has<Native>(ref m_ChunkType.m_Native) ? (TempFlags.Select | TempFlags.Duplicate) : (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate));
			if (((ArchetypeChunk)(ref val)).Has<Game.Objects.Object>(ref m_ChunkType.m_Object))
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_ChunkType.m_Entity);
				NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Temp>(ref m_ChunkType.m_Temp);
				NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_ChunkType.m_Owner);
				NativeArray<Transform> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<Transform>(ref m_ChunkType.m_Transform);
				NativeArray<Attached> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<Attached>(ref m_ChunkType.m_Attached);
				NativeArray<Game.Objects.NetObject> nativeArray6 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Objects.NetObject>(ref m_ChunkType.m_NetObject);
				NativeArray<PrefabRef> nativeArray7 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_ChunkType.m_PrefabRef);
				NativeArray<Building> nativeArray8 = ((ArchetypeChunk)(ref val)).GetNativeArray<Building>(ref m_ChunkType.m_Building);
				bool flag = ((ArchetypeChunk)(ref val)).Has<Game.Objects.OutsideConnection>(ref m_ChunkType.m_OutsideConnection);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Temp temp = nativeArray2[i];
					if ((temp.m_Flags & tempFlags) == 0)
					{
						Entity entity = nativeArray[i];
						Transform transform = nativeArray4[i];
						PrefabRef prefabRef = nativeArray7[i];
						Owner owner = default(Owner);
						if (nativeArray3.Length != 0)
						{
							owner = nativeArray3[i];
						}
						Attached attached = default(Attached);
						if (nativeArray5.Length != 0)
						{
							attached = nativeArray5[i];
						}
						Game.Objects.ValidationHelpers.ValidateObject(entity, temp, owner, transform, prefabRef, attached, flag, m_EditorMode, m_EntityData, m_EdgeList, m_ObjectList, m_ObjectSearchTree, m_NetSearchTree, m_AreaSearchTree, m_InstanceCounts, m_WaterSurfaceData, m_TerrainHeightData, m_ErrorQueue);
					}
					if ((temp.m_Flags & (TempFlags.Delete | TempFlags.Modify | TempFlags.Replace | TempFlags.Upgrade)) != 0 && temp.m_Original != Entity.Null && m_EntityData.m_OnFire.HasComponent(temp.m_Original))
					{
						ErrorData errorData = new ErrorData
						{
							m_ErrorType = ErrorType.OnFire,
							m_ErrorSeverity = ErrorSeverity.Error,
							m_TempEntity = nativeArray[i],
							m_Position = float3.op_Implicit(float.NaN)
						};
						m_ErrorQueue.Enqueue(errorData);
					}
				}
				for (int j = 0; j < nativeArray8.Length; j++)
				{
					if ((nativeArray2[j].m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) == 0)
					{
						Entity entity2 = nativeArray[j];
						Building building = nativeArray8[j];
						Transform transform2 = nativeArray4[j];
						PrefabRef prefabRef2 = nativeArray7[j];
						Game.Buildings.ValidationHelpers.ValidateBuilding(entity2, building, transform2, prefabRef2, m_EntityData, m_GroundWaterMap, m_ErrorQueue);
					}
				}
				for (int k = 0; k < nativeArray6.Length; k++)
				{
					if ((nativeArray2[k].m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) == 0)
					{
						Entity entity3 = nativeArray[k];
						Game.Objects.NetObject netObject = nativeArray6[k];
						Transform transform3 = nativeArray4[k];
						PrefabRef prefabRef3 = nativeArray7[k];
						Attached attached2 = default(Attached);
						if (nativeArray5.Length != 0)
						{
							attached2 = nativeArray5[k];
						}
						Game.Objects.ValidationHelpers.ValidateNetObject(entity3, netObject, transform3, prefabRef3, attached2, m_EntityData, m_ErrorQueue);
					}
				}
				if (((ArchetypeChunk)(ref val)).Has<Game.Routes.TransportStop>(ref m_ChunkType.m_TransportStop))
				{
					for (int l = 0; l < nativeArray.Length; l++)
					{
						Temp temp2 = nativeArray2[l];
						if ((temp2.m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) == 0)
						{
							Entity entity4 = nativeArray[l];
							Transform transform4 = nativeArray4[l];
							PrefabRef prefabRef4 = nativeArray7[l];
							Owner owner2 = default(Owner);
							if (nativeArray3.Length != 0)
							{
								owner2 = nativeArray3[l];
							}
							Attached attached3 = default(Attached);
							if (nativeArray5.Length != 0)
							{
								attached3 = nativeArray5[l];
							}
							Game.Routes.ValidationHelpers.ValidateStop(m_EditorMode, entity4, temp2, owner2, transform4, prefabRef4, attached3, m_EntityData, m_ErrorQueue);
						}
					}
				}
				if (flag)
				{
					for (int m = 0; m < nativeArray.Length; m++)
					{
						if ((nativeArray2[m].m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) == 0)
						{
							Entity entity5 = nativeArray[m];
							Transform transform5 = nativeArray4[m];
							Game.Objects.ValidationHelpers.ValidateOutsideConnection(entity5, transform5, m_TerrainHeightData, m_ErrorQueue);
						}
					}
				}
			}
			if (((ArchetypeChunk)(ref val)).Has<Game.Buildings.ServiceUpgrade>(ref m_ChunkType.m_ServiceUpgrade))
			{
				NativeArray<Entity> nativeArray9 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_ChunkType.m_Entity);
				NativeArray<PrefabRef> nativeArray10 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_ChunkType.m_PrefabRef);
				NativeArray<Owner> nativeArray11 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_ChunkType.m_Owner);
				for (int n = 0; n < nativeArray9.Length; n++)
				{
					Entity entity6 = nativeArray9[n];
					PrefabRef prefabRef5 = nativeArray10[n];
					Owner owner3 = default(Owner);
					if (nativeArray11.Length != 0)
					{
						owner3 = nativeArray11[n];
					}
					Game.Buildings.ValidationHelpers.ValidateUpgrade(entity6, owner3, prefabRef5, m_EntityData, m_ErrorQueue);
				}
			}
			if (((ArchetypeChunk)(ref val)).Has<Game.Net.Edge>(ref m_ChunkType.m_Edge))
			{
				NativeArray<Entity> nativeArray12 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_ChunkType.m_Entity);
				NativeArray<Temp> nativeArray13 = ((ArchetypeChunk)(ref val)).GetNativeArray<Temp>(ref m_ChunkType.m_Temp);
				NativeArray<Owner> nativeArray14 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_ChunkType.m_Owner);
				NativeArray<Game.Net.Edge> nativeArray15 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.Edge>(ref m_ChunkType.m_Edge);
				NativeArray<EdgeGeometry> nativeArray16 = ((ArchetypeChunk)(ref val)).GetNativeArray<EdgeGeometry>(ref m_ChunkType.m_EdgeGeometry);
				NativeArray<StartNodeGeometry> nativeArray17 = ((ArchetypeChunk)(ref val)).GetNativeArray<StartNodeGeometry>(ref m_ChunkType.m_StartNodeGeometry);
				NativeArray<EndNodeGeometry> nativeArray18 = ((ArchetypeChunk)(ref val)).GetNativeArray<EndNodeGeometry>(ref m_ChunkType.m_EndNodeGeometry);
				NativeArray<Composition> nativeArray19 = ((ArchetypeChunk)(ref val)).GetNativeArray<Composition>(ref m_ChunkType.m_Composition);
				NativeArray<Fixed> nativeArray20 = ((ArchetypeChunk)(ref val)).GetNativeArray<Fixed>(ref m_ChunkType.m_Fixed);
				NativeArray<PrefabRef> nativeArray21 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_ChunkType.m_PrefabRef);
				if (!m_TempNodes.IsCreated)
				{
					m_TempNodes = new NativeList<ConnectedNode>(16, AllocatorHandle.op_Implicit((Allocator)2));
				}
				bool flag2 = nativeArray20.Length != 0;
				for (int num = 0; num < nativeArray16.Length; num++)
				{
					Temp temp3 = nativeArray13[num];
					if ((temp3.m_Flags & tempFlags) == 0)
					{
						Entity entity7 = nativeArray12[num];
						Game.Net.Edge edge = nativeArray15[num];
						EdgeGeometry edgeGeometry = nativeArray16[num];
						StartNodeGeometry startNodeGeometry = nativeArray17[num];
						EndNodeGeometry endNodeGeometry = nativeArray18[num];
						Composition composition = nativeArray19[num];
						PrefabRef prefabRef6 = nativeArray21[num];
						Owner owner4 = default(Owner);
						if (nativeArray14.Length != 0)
						{
							owner4 = nativeArray14[num];
						}
						Fixed obj = new Fixed
						{
							m_Index = -1
						};
						if (flag2)
						{
							obj = nativeArray20[num];
						}
						Game.Net.ValidationHelpers.ValidateEdge(entity7, temp3, owner4, obj, edge, edgeGeometry, startNodeGeometry, endNodeGeometry, composition, prefabRef6, m_EditorMode, m_EntityData, m_EdgeList, m_ObjectSearchTree, m_NetSearchTree, m_AreaSearchTree, m_WaterSurfaceData, m_TerrainHeightData, m_ErrorQueue, m_TempNodes);
					}
				}
			}
			if (((ArchetypeChunk)(ref val)).Has<Lane>(ref m_ChunkType.m_Lane))
			{
				NativeArray<Entity> nativeArray22 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_ChunkType.m_Entity);
				NativeArray<Temp> nativeArray23 = ((ArchetypeChunk)(ref val)).GetNativeArray<Temp>(ref m_ChunkType.m_Temp);
				NativeArray<Owner> nativeArray24 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_ChunkType.m_Owner);
				NativeArray<Lane> nativeArray25 = ((ArchetypeChunk)(ref val)).GetNativeArray<Lane>(ref m_ChunkType.m_Lane);
				NativeArray<Game.Net.TrackLane> nativeArray26 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.TrackLane>(ref m_ChunkType.m_TrackLane);
				NativeArray<Curve> nativeArray27 = ((ArchetypeChunk)(ref val)).GetNativeArray<Curve>(ref m_ChunkType.m_Curve);
				NativeArray<EdgeLane> nativeArray28 = ((ArchetypeChunk)(ref val)).GetNativeArray<EdgeLane>(ref m_ChunkType.m_EdgeLane);
				NativeArray<PrefabRef> nativeArray29 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_ChunkType.m_PrefabRef);
				for (int num2 = 0; num2 < nativeArray26.Length; num2++)
				{
					if ((nativeArray23[num2].m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) == 0)
					{
						Entity entity8 = nativeArray22[num2];
						Lane lane = nativeArray25[num2];
						Game.Net.TrackLane trackLane = nativeArray26[num2];
						Curve curve = nativeArray27[num2];
						PrefabRef prefabRef7 = nativeArray29[num2];
						Owner owner5 = default(Owner);
						if (nativeArray24.Length != 0)
						{
							owner5 = nativeArray24[num2];
						}
						EdgeLane edgeLane = default(EdgeLane);
						if (nativeArray28.Length != 0)
						{
							edgeLane = nativeArray28[num2];
						}
						Game.Net.ValidationHelpers.ValidateLane(entity8, owner5, lane, trackLane, curve, edgeLane, prefabRef7, m_EntityData, m_ErrorQueue);
					}
				}
			}
			if (((ArchetypeChunk)(ref val)).Has<Area>(ref m_ChunkType.m_Area))
			{
				NativeArray<Entity> nativeArray30 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_ChunkType.m_Entity);
				NativeArray<Temp> nativeArray31 = ((ArchetypeChunk)(ref val)).GetNativeArray<Temp>(ref m_ChunkType.m_Temp);
				NativeArray<Owner> nativeArray32 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_ChunkType.m_Owner);
				NativeArray<Area> nativeArray33 = ((ArchetypeChunk)(ref val)).GetNativeArray<Area>(ref m_ChunkType.m_Area);
				NativeArray<Geometry> nativeArray34 = ((ArchetypeChunk)(ref val)).GetNativeArray<Geometry>(ref m_ChunkType.m_AreaGeometry);
				NativeArray<Storage> nativeArray35 = ((ArchetypeChunk)(ref val)).GetNativeArray<Storage>(ref m_ChunkType.m_AreaStorage);
				BufferAccessor<Game.Areas.Node> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Game.Areas.Node>(ref m_ChunkType.m_AreaNode);
				NativeArray<PrefabRef> nativeArray36 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_ChunkType.m_PrefabRef);
				for (int num3 = 0; num3 < nativeArray30.Length; num3++)
				{
					Temp temp4 = nativeArray31[num3];
					if ((temp4.m_Flags & tempFlags) == 0)
					{
						Entity entity9 = nativeArray30[num3];
						Area area = nativeArray33[num3];
						DynamicBuffer<Game.Areas.Node> nodes = bufferAccessor[num3];
						PrefabRef prefabRef8 = nativeArray36[num3];
						Geometry geometry = default(Geometry);
						if (nativeArray34.Length != 0)
						{
							geometry = nativeArray34[num3];
						}
						Storage storage = default(Storage);
						if (nativeArray35.Length != 0)
						{
							storage = nativeArray35[num3];
						}
						Owner owner6 = default(Owner);
						if (nativeArray32.Length != 0)
						{
							owner6 = nativeArray32[num3];
						}
						Game.Areas.ValidationHelpers.ValidateArea(m_EditorMode, entity9, temp4, owner6, area, geometry, storage, nodes, prefabRef8, m_EntityData, m_ObjectSearchTree, m_NetSearchTree, m_AreaSearchTree, m_WaterSurfaceData, m_TerrainHeightData, m_ErrorQueue);
					}
				}
			}
			if (((ArchetypeChunk)(ref val)).Has<RouteSegment>(ref m_ChunkType.m_RouteSegment))
			{
				NativeArray<Entity> nativeArray37 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_ChunkType.m_Entity);
				NativeArray<Temp> nativeArray38 = ((ArchetypeChunk)(ref val)).GetNativeArray<Temp>(ref m_ChunkType.m_Temp);
				NativeArray<PrefabRef> nativeArray39 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_ChunkType.m_PrefabRef);
				BufferAccessor<RouteWaypoint> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<RouteWaypoint>(ref m_ChunkType.m_RouteWaypoint);
				BufferAccessor<RouteSegment> bufferAccessor3 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<RouteSegment>(ref m_ChunkType.m_RouteSegment);
				for (int num4 = 0; num4 < nativeArray37.Length; num4++)
				{
					Temp temp5 = nativeArray38[num4];
					if ((temp5.m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) == 0)
					{
						Game.Routes.ValidationHelpers.ValidateRoute(nativeArray37[num4], prefabRef: nativeArray39[num4], waypoints: bufferAccessor2[num4], segments: bufferAccessor3[num4], temp: temp5, data: m_EntityData, errorQueue: m_ErrorQueue);
					}
				}
			}
			if (!m_EditorMode && ((ArchetypeChunk)(ref val)).Has<Brush>(ref m_ChunkType.m_Brush))
			{
				NativeArray<Entity> nativeArray40 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_ChunkType.m_Entity);
				NativeArray<Brush> nativeArray41 = ((ArchetypeChunk)(ref val)).GetNativeArray<Brush>(ref m_ChunkType.m_Brush);
				Bounds3 val3 = default(Bounds3);
				for (int num5 = 0; num5 < nativeArray40.Length; num5++)
				{
					Brush brush = nativeArray41[num5];
					if (m_EntityData.m_TerraformingData.HasComponent(brush.m_Tool))
					{
						Entity val2 = nativeArray40[num5];
						((Bounds3)(ref val3))._002Ector(brush.m_Position - brush.m_Size * 0.4f, brush.m_Position + brush.m_Size * 0.4f);
						Game.Areas.ValidationHelpers.BrushAreaIterator brushAreaIterator = new Game.Areas.ValidationHelpers.BrushAreaIterator
						{
							m_BrushEntity = val2,
							m_Brush = brush,
							m_BrushBounds = val3,
							m_Data = m_EntityData,
							m_ErrorQueue = m_ErrorQueue
						};
						m_AreaSearchTree.Iterate<Game.Areas.ValidationHelpers.BrushAreaIterator>(ref brushAreaIterator, 0);
						Game.Objects.ValidationHelpers.ValidateWorldBounds(val2, default(Owner), val3, m_EntityData, m_TerrainHeightData, m_ErrorQueue);
					}
				}
			}
			if (!((ArchetypeChunk)(ref val)).Has<Game.Simulation.WaterSourceData>(ref m_ChunkType.m_WaterSourceData))
			{
				return;
			}
			NativeArray<Entity> nativeArray42 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_ChunkType.m_Entity);
			NativeArray<Temp> nativeArray43 = ((ArchetypeChunk)(ref val)).GetNativeArray<Temp>(ref m_ChunkType.m_Temp);
			NativeArray<Transform> nativeArray44 = ((ArchetypeChunk)(ref val)).GetNativeArray<Transform>(ref m_ChunkType.m_Transform);
			NativeArray<Game.Simulation.WaterSourceData> nativeArray45 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Simulation.WaterSourceData>(ref m_ChunkType.m_WaterSourceData);
			for (int num6 = 0; num6 < nativeArray42.Length; num6++)
			{
				Temp temp6 = nativeArray43[num6];
				if ((temp6.m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) == 0 || (temp6.m_Flags & TempFlags.Dragging) != 0)
				{
					Entity entity10 = nativeArray42[num6];
					Transform transform6 = nativeArray44[num6];
					Game.Simulation.WaterSourceData waterSourceData = nativeArray45[num6];
					Game.Objects.ValidationHelpers.ValidateWaterSource(entity10, transform6, waterSourceData, m_TerrainHeightData, m_ErrorQueue);
				}
			}
		}
	}

	[BurstCompile]
	private struct CollectAreaTrianglesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Native> m_NativeType;

		[ReadOnly]
		public BufferTypeHandle<Triangle> m_TriangleType;

		public NativeList<AreaSearchItem> m_AreaTriangles;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			BufferAccessor<Triangle> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Triangle>(ref m_TriangleType);
			TempFlags tempFlags = (((ArchetypeChunk)(ref chunk)).Has<Native>(ref m_NativeType) ? TempFlags.Select : (TempFlags.Delete | TempFlags.Select));
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if ((nativeArray2[i].m_Flags & tempFlags) == 0)
				{
					Entity area = nativeArray[i];
					DynamicBuffer<Triangle> val = bufferAccessor[i];
					for (int j = 0; j < val.Length; j++)
					{
						ref NativeList<AreaSearchItem> reference = ref m_AreaTriangles;
						AreaSearchItem areaSearchItem = new AreaSearchItem(area, j);
						reference.Add(ref areaSearchItem);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct ValidateAreaTrianglesJob : IJobParallelForDefer
	{
		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public NativeArray<AreaSearchItem> m_AreaTriangles;

		[ReadOnly]
		public EntityData m_EntityData;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		public ParallelWriter<ErrorData> m_ErrorQueue;

		public void Execute(int index)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			AreaSearchItem areaSearchItem = m_AreaTriangles[index];
			Area area = m_EntityData.m_Area[areaSearchItem.m_Area];
			if ((area.m_Flags & AreaFlags.Slave) == 0)
			{
				DynamicBuffer<Triangle> val = m_EntityData.m_AreaTriangles[areaSearchItem.m_Area];
				Temp temp = m_EntityData.m_Temp[areaSearchItem.m_Area];
				Owner owner = default(Owner);
				if (m_EntityData.m_Owner.HasComponent(areaSearchItem.m_Area))
				{
					owner = m_EntityData.m_Owner[areaSearchItem.m_Area];
				}
				bool noErrors = (area.m_Flags & AreaFlags.Complete) == 0;
				bool isCounterClockwise = (area.m_Flags & AreaFlags.CounterClockwise) != 0;
				Game.Areas.ValidationHelpers.ValidateTriangle(m_EditorMode, noErrors, isCounterClockwise, areaSearchItem.m_Area, temp, owner, val[areaSearchItem.m_Triangle], m_EntityData, m_ObjectSearchTree, m_NetSearchTree, m_AreaSearchTree, m_WaterSurfaceData, m_TerrainHeightData, m_ErrorQueue);
			}
		}
	}

	[BurstCompile]
	private struct FillErrorPrefabsJob : IJobChunk
	{
		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ToolErrorData> m_ToolErrorType;

		public NativeArray<Entity> m_ErrorPrefabs;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ToolErrorData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ToolErrorData>(ref m_ToolErrorType);
			ToolErrorFlags toolErrorFlags = (m_EditorMode ? ToolErrorFlags.DisableInEditor : ToolErrorFlags.DisableInGame);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				ToolErrorData toolErrorData = nativeArray2[i];
				if ((toolErrorData.m_Flags & toolErrorFlags) == 0)
				{
					m_ErrorPrefabs[(int)toolErrorData.m_Error] = val;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct IconKey : IEquatable<IconKey>
	{
		public Entity m_Owner;

		public Entity m_Target;

		public Entity m_Prefab;

		public bool Equals(IconKey other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if (((Entity)(ref m_Owner)).Equals(other.m_Owner) && ((Entity)(ref m_Target)).Equals(other.m_Target))
			{
				return ((Entity)(ref m_Prefab)).Equals(other.m_Prefab);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Owner)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Target)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Prefab)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	public struct IconValue
	{
		public Bounds3 m_Bounds;

		public IconPriority m_Severity;

		public bool m_Cancelled;
	}

	[BurstCompile]
	private struct ProcessValidationResultsJob : IJob
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Brush> m_BrushType;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PlayerMoney> m_PlayerMoney;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<Entity> m_ErrorPrefabs;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public Entity m_City;

		public NativeHashMap<Entity, ErrorSeverity> m_ErrorMap;

		public NativeQueue<ErrorData> m_ErrorQueue1;

		public NativeQueue<ErrorData> m_ErrorQueue2;

		public EntityCommandBuffer m_CommandBuffer;

		public IconCommandBuffer m_IconCommandBuffer;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			NativeHashMap<IconKey, IconValue> iconMap = default(NativeHashMap<IconKey, IconValue>);
			iconMap._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
			int totalCost = 0;
			Entity brushEntity = Entity.Null;
			float4 brushPosition = default(float4);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				CalculateCost(m_Chunks[i], ref totalCost, ref brushEntity, ref brushPosition);
			}
			int num = totalCost;
			ProcessQueue(m_ErrorQueue1, iconMap, brushEntity, brushPosition, ref totalCost);
			ProcessQueue(m_ErrorQueue2, iconMap, brushEntity, brushPosition, ref totalCost);
			PlayerMoney playerMoney = default(PlayerMoney);
			if (m_PlayerMoney.TryGetComponent(m_City, ref playerMoney))
			{
				int num2 = math.max(0, playerMoney.money);
				bool flag = totalCost > num2;
				if (flag)
				{
					for (int j = 0; j < m_Chunks.Length; j++)
					{
						CancelOptionalWithMoneyError(m_Chunks[j], iconMap, brushEntity, brushPosition, ref totalCost, num2);
						if (totalCost <= num2)
						{
							break;
						}
					}
					flag = totalCost > num2;
				}
				if (!flag && num > num2)
				{
					flag = true;
					for (int k = 0; k < m_Chunks.Length; k++)
					{
						if (!AllCancelled(m_Chunks[k], ErrorSeverity.Cancel))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					for (int l = 0; l < m_Chunks.Length; l++)
					{
						ProcessMoneyErrors(m_Chunks[l], iconMap, brushEntity, brushPosition, ref totalCost);
					}
				}
			}
			bool flag2 = false;
			Enumerator<Entity, ErrorSeverity> enumerator = m_ErrorMap.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value == ErrorSeverity.CancelError)
				{
					flag2 = brushEntity != Entity.Null;
				}
			}
			enumerator.Dispose();
			if (flag2)
			{
				for (int m = 0; m < m_Chunks.Length; m++)
				{
					if (!AllCancelled(m_Chunks[m], ErrorSeverity.CancelError))
					{
						flag2 = false;
						break;
					}
				}
			}
			Enumerator<IconKey, IconValue> enumerator2 = iconMap.GetEnumerator();
			ErrorSeverity errorSeverity = default(ErrorSeverity);
			while (enumerator2.MoveNext())
			{
				IconKey key = enumerator2.Current.Key;
				IconValue value = enumerator2.Current.Value;
				if (value.m_Cancelled)
				{
					if (!flag2)
					{
						continue;
					}
					if (key.m_Owner != Entity.Null)
					{
						AddError(key.m_Owner, ErrorSeverity.Error);
					}
					if (key.m_Target != Entity.Null)
					{
						AddError(key.m_Target, ErrorSeverity.Error);
					}
				}
				else if (m_ErrorMap.TryGetValue(key.m_Owner, ref errorSeverity) && errorSeverity >= ErrorSeverity.Cancel)
				{
					continue;
				}
				if (math.any(math.isnan(value.m_Bounds.min)))
				{
					m_IconCommandBuffer.Add(key.m_Owner, key.m_Prefab, value.m_Severity, IconClusterLayer.Default, (IconFlags)0, key.m_Target, isTemp: true);
					continue;
				}
				float3 location = MathUtils.Center(value.m_Bounds);
				m_IconCommandBuffer.Add(key.m_Owner, key.m_Prefab, location, value.m_Severity, IconClusterLayer.Default, (IconFlags)0, key.m_Target, isTemp: true);
			}
			enumerator2.Dispose();
			iconMap.Dispose();
		}

		private bool AllCancelled(ArchetypeChunk chunk, ErrorSeverity limit)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			ErrorSeverity errorSeverity = default(ErrorSeverity);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				if ((nativeArray2[i].m_Flags & TempFlags.Cancel) == 0 && (!m_ErrorMap.TryGetValue(nativeArray[i], ref errorSeverity) || errorSeverity < limit))
				{
					return false;
				}
			}
			return true;
		}

		private void CalculateCost(ArchetypeChunk chunk, ref int totalCost, ref Entity brushEntity, ref float4 brushPosition)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Brush> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Brush>(ref m_BrushType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				totalCost += nativeArray2[i].m_Cost;
			}
			for (int j = 0; j < nativeArray3.Length; j++)
			{
				Brush brush = nativeArray3[j];
				brushEntity = nativeArray[j];
				brushPosition += new float4(brush.m_Position * brush.m_Strength, brush.m_Strength);
			}
		}

		private void CancelOptionalWithMoneyError(ArchetypeChunk chunk, NativeHashMap<IconKey, IconValue> iconMap, Entity brushEntity, float4 brushPosition, ref int totalCost, int costLimit)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Curve> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			ErrorData error = new ErrorData
			{
				m_ErrorSeverity = ErrorSeverity.Error,
				m_ErrorType = ErrorType.NotEnoughMoney,
				m_Position = float3.op_Implicit(float.NaN)
			};
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				if (nativeArray2[i].m_Cost > 0)
				{
					error.m_TempEntity = nativeArray[i];
					if (CancelOptional(error, iconMap, brushEntity, brushPosition, ref totalCost) && totalCost <= costLimit)
					{
						return;
					}
				}
			}
			for (int j = 0; j < nativeArray4.Length; j++)
			{
				if (nativeArray2[j].m_Cost > 0)
				{
					error.m_TempEntity = nativeArray[j];
					if (CancelOptional(error, iconMap, brushEntity, brushPosition, ref totalCost) && totalCost <= costLimit)
					{
						break;
					}
				}
			}
		}

		private void ProcessMoneyErrors(ArchetypeChunk chunk, NativeHashMap<IconKey, IconValue> iconMap, Entity brushEntity, float4 brushPosition, ref int totalCost)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Curve> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			ErrorData error = new ErrorData
			{
				m_ErrorSeverity = ErrorSeverity.Error,
				m_ErrorType = ErrorType.NotEnoughMoney,
				m_Position = float3.op_Implicit(float.NaN)
			};
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				if (nativeArray2[i].m_Cost > 0)
				{
					error.m_TempEntity = nativeArray[i];
					ProcessError(error, iconMap, brushEntity, brushPosition, ref totalCost);
				}
			}
			for (int j = 0; j < nativeArray4.Length; j++)
			{
				if (nativeArray2[j].m_Cost > 0)
				{
					error.m_TempEntity = nativeArray[j];
					ProcessError(error, iconMap, brushEntity, brushPosition, ref totalCost);
				}
			}
		}

		private void ProcessQueue(NativeQueue<ErrorData> errorQueue, NativeHashMap<IconKey, IconValue> iconMap, Entity brushEntity, float4 brushPosition, ref int totalCost)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			ErrorData error = default(ErrorData);
			while (errorQueue.TryDequeue(ref error))
			{
				ProcessError(error, iconMap, brushEntity, brushPosition, ref totalCost);
			}
		}

		private void ProcessError(ErrorData error, NativeHashMap<IconKey, IconValue> iconMap, Entity brushEntity, float4 brushPosition, ref int totalCost)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			if (!(m_ErrorPrefabs[(int)error.m_ErrorType] != Entity.Null) || CancelOptional(error, iconMap, brushEntity, brushPosition, ref totalCost))
			{
				return;
			}
			if (error.m_ErrorSeverity >= ErrorSeverity.Cancel)
			{
				Temp temp = default(Temp);
				if (m_TempData.TryGetComponent(error.m_TempEntity, ref temp))
				{
					Cancel(error, temp, iconMap, brushEntity, brushPosition, addCancelledError: false, ref totalCost);
				}
				if (m_TempData.TryGetComponent(error.m_PermanentEntity, ref temp))
				{
					error.m_TempEntity = error.m_PermanentEntity;
					Cancel(error, temp, iconMap, brushEntity, brushPosition, addCancelledError: false, ref totalCost);
				}
				return;
			}
			if (error.m_ErrorSeverity != ErrorSeverity.Override)
			{
				AddIcon(error, iconMap, cancelled: false);
			}
			if (error.m_TempEntity != Entity.Null && (error.m_ErrorSeverity >= ErrorSeverity.Error || (error.m_PermanentEntity == Entity.Null && error.m_ErrorSeverity == ErrorSeverity.Override)))
			{
				AddError(error.m_TempEntity, error.m_ErrorSeverity);
			}
			if (error.m_PermanentEntity != Entity.Null && error.m_ErrorSeverity >= ErrorSeverity.Override)
			{
				AddError(error.m_PermanentEntity, error.m_ErrorSeverity);
			}
		}

		private bool CancelOptional(ErrorData error, NativeHashMap<IconKey, IconValue> iconMap, Entity brushEntity, float4 brushPosition, ref int totalCost)
		{
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			Temp temp = default(Temp);
			Temp temp2 = default(Temp);
			Owner owner = default(Owner);
			Owner owner2 = default(Owner);
			while (error.m_TempEntity != Entity.Null)
			{
				if (m_TempData.TryGetComponent(error.m_TempEntity, ref temp) && (temp.m_Flags & TempFlags.Optional) != 0)
				{
					bool flag = error.m_ErrorSeverity == ErrorSeverity.Error;
					if (flag)
					{
						ErrorData error2 = error;
						error2.m_TempEntity = error.m_PermanentEntity;
						while (error2.m_TempEntity != Entity.Null)
						{
							if (m_TempData.TryGetComponent(error2.m_TempEntity, ref temp2) && (temp2.m_Flags & TempFlags.Optional) != 0)
							{
								flag = false;
								Cancel(error2, temp2, iconMap, brushEntity, brushPosition, addCancelledError: false, ref totalCost);
								break;
							}
							if (!m_OwnerData.TryGetComponent(error2.m_TempEntity, ref owner))
							{
								break;
							}
							error2.m_TempEntity = owner.m_Owner;
						}
					}
					Cancel(error, temp, iconMap, brushEntity, brushPosition, flag, ref totalCost);
					return true;
				}
				if (!m_OwnerData.TryGetComponent(error.m_TempEntity, ref owner2))
				{
					break;
				}
				error.m_TempEntity = owner2.m_Owner;
			}
			return false;
		}

		private void Cancel(ErrorData error, Temp temp, NativeHashMap<IconKey, IconValue> iconMap, Entity brushEntity, float4 brushPosition, bool addCancelledError, ref int totalCost)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			if (AddError(error.m_TempEntity, (error.m_ErrorSeverity == ErrorSeverity.Error || error.m_ErrorSeverity == ErrorSeverity.CancelError) ? ErrorSeverity.CancelError : ErrorSeverity.Cancel))
			{
				totalCost -= temp.m_Cost;
				temp.m_Flags |= TempFlags.Hidden | TempFlags.Cancel;
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Temp>(error.m_TempEntity, temp);
			}
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(error.m_TempEntity, ref val))
			{
				Temp temp2 = default(Temp);
				for (int i = 0; i < val.Length; i++)
				{
					error.m_TempEntity = val[i].m_SubObject;
					if (m_TempData.TryGetComponent(error.m_TempEntity, ref temp2))
					{
						Cancel(error, temp2, iconMap, brushEntity, brushPosition, addCancelledError: false, ref totalCost);
					}
				}
			}
			if (addCancelledError && brushEntity != Entity.Null)
			{
				if (brushPosition.w != 0f)
				{
					brushPosition /= brushPosition.w;
				}
				error.m_TempEntity = brushEntity;
				error.m_Position = ((float4)(ref brushPosition)).xyz;
				AddIcon(error, iconMap, cancelled: true);
			}
		}

		private void AddIcon(ErrorData error, NativeHashMap<IconKey, IconValue> iconMap, bool cancelled)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			IconKey iconKey = default(IconKey);
			iconKey.m_Owner = error.m_TempEntity;
			iconKey.m_Target = error.m_PermanentEntity;
			iconKey.m_Prefab = m_ErrorPrefabs[(int)error.m_ErrorType];
			IconValue iconValue = default(IconValue);
			iconValue.m_Bounds = new Bounds3(error.m_Position, error.m_Position);
			iconValue.m_Cancelled = cancelled;
			switch (error.m_ErrorSeverity)
			{
			case ErrorSeverity.Warning:
				iconValue.m_Severity = IconPriority.Warning;
				break;
			case ErrorSeverity.Error:
				iconValue.m_Severity = IconPriority.Error;
				break;
			default:
				iconValue.m_Severity = IconPriority.Info;
				break;
			}
			IconValue iconValue2 = default(IconValue);
			if (iconMap.TryGetValue(iconKey, ref iconValue2))
			{
				if (math.any(math.isnan(error.m_Position)))
				{
					iconValue.m_Bounds = iconValue2.m_Bounds;
				}
				else if (!math.any(math.isnan(iconValue2.m_Bounds.min)))
				{
					ref Bounds3 reference = ref iconValue.m_Bounds;
					reference |= iconValue2.m_Bounds;
				}
				iconValue.m_Severity = (IconPriority)math.max((int)iconValue.m_Severity, (int)iconValue2.m_Severity);
				iconMap[iconKey] = iconValue;
			}
			else
			{
				iconMap.Add(iconKey, iconValue);
			}
		}

		private bool AddError(Entity entity, ErrorSeverity severity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			ErrorSeverity errorSeverity = default(ErrorSeverity);
			if (m_ErrorMap.TryGetValue(entity, ref errorSeverity))
			{
				if (severity > errorSeverity)
				{
					m_ErrorMap[entity] = severity;
					return errorSeverity < ErrorSeverity.Cancel;
				}
				return false;
			}
			m_ErrorMap.Add(entity, severity);
			return true;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Native> __Game_Common_Native_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Triangle> __Game_Areas_Triangle_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ToolErrorData> __Game_Prefabs_ToolErrorData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Brush> __Game_Tools_Brush_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlayerMoney> __Game_City_PlayerMoney_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Common_Native_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Native>(true);
			__Game_Areas_Triangle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Triangle>(true);
			__Game_Prefabs_ToolErrorData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ToolErrorData>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Tools_Brush_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Brush>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_City_PlayerMoney_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlayerMoney>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
		}
	}

	private ModificationEndBarrier m_ModificationBarrier;

	private ToolSystem m_ToolSystem;

	private Components m_Components;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private InstanceCountSystem m_InstanceCountSystem;

	private CitySystem m_CitySystem;

	private IconCommandSystem m_IconCommandSystem;

	private WaterSystem m_WaterSystem;

	private TerrainSystem m_TerrainSystem;

	private GroundWaterSystem m_GroundWaterSystem;

	private EntityQuery m_UpdatedQuery;

	private EntityQuery m_UpdatedAreaQuery;

	private EntityQuery m_ToolErrorPrefabQuery;

	private ChunkType m_ChunkType;

	private EntityData m_EntityData;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
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
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationEndBarrier>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_Components = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Components>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_InstanceCountSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<InstanceCountSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Relative>(),
			ComponentType.Exclude<Moving>(),
			ComponentType.Exclude<Stopped>()
		});
		m_UpdatedAreaQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Area>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ToolErrorPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<NotificationIconData>(),
			ComponentType.ReadOnly<ToolErrorData>()
		});
		m_ChunkType = new ChunkType((SystemBase)(object)this);
		m_EntityData = new EntityData((SystemBase)(object)this);
		((ComponentSystemBase)this).RequireForUpdate(m_UpdatedQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		NativeList<BoundsData> edgeList = default(NativeList<BoundsData>);
		edgeList._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<BoundsData> objectList = default(NativeList<BoundsData>);
		objectList._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<ErrorData> errorQueue = default(NativeQueue<ErrorData>);
		errorQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<ErrorData> errorQueue2 = default(NativeQueue<ErrorData>);
		errorQueue2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Entity> errorPrefabs = default(NativeArray<Entity>);
		errorPrefabs._002Ector(28, (Allocator)3, (NativeArrayOptions)1);
		JobHandle val2 = default(JobHandle);
		NativeList<ArchetypeChunk> val = ((EntityQuery)(ref m_UpdatedQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
		m_ChunkType.Update((SystemBase)(object)this);
		m_EntityData.Update((SystemBase)(object)this);
		BoundsListJob boundsListJob = new BoundsListJob
		{
			m_Chunks = val.AsDeferredJobArray(),
			m_ChunkType = m_ChunkType,
			m_EntityData = m_EntityData,
			m_EdgeList = edgeList,
			m_ObjectList = objectList
		};
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle dependencies4;
		JobHandle deps;
		JobHandle dependencies5;
		ValidationJob validationJob = new ValidationJob
		{
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_Chunks = val.AsDeferredJobArray(),
			m_ChunkType = m_ChunkType,
			m_EntityData = m_EntityData,
			m_EdgeList = edgeList,
			m_ObjectList = objectList,
			m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies2),
			m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies3),
			m_InstanceCounts = m_InstanceCountSystem.GetInstanceCounts(readOnly: true, out dependencies4),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_GroundWaterMap = m_GroundWaterSystem.GetMap(readOnly: true, out dependencies5),
			m_ErrorQueue = errorQueue.AsParallelWriter()
		};
		JobHandle val3 = JobUtils.CombineDependencies(dependencies, dependencies2, dependencies3, dependencies4, deps, dependencies5);
		JobHandle val4 = default(JobHandle);
		m_Components.m_ErrorMap = new NativeHashMap<Entity, ErrorSeverity>(32, AllocatorHandle.op_Implicit((Allocator)3));
		if (!((EntityQuery)(ref m_UpdatedAreaQuery)).IsEmptyIgnoreFilter)
		{
			NativeList<AreaSearchItem> val5 = default(NativeList<AreaSearchItem>);
			val5._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			CollectAreaTrianglesJob collectAreaTrianglesJob = new CollectAreaTrianglesJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NativeType = InternalCompilerInterface.GetComponentTypeHandle<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TriangleType = InternalCompilerInterface.GetBufferTypeHandle<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AreaTriangles = val5
			};
			ValidateAreaTrianglesJob obj = new ValidateAreaTrianglesJob
			{
				m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
				m_AreaTriangles = val5.AsDeferredJobArray(),
				m_EntityData = validationJob.m_EntityData,
				m_ObjectSearchTree = validationJob.m_ObjectSearchTree,
				m_NetSearchTree = validationJob.m_NetSearchTree,
				m_AreaSearchTree = validationJob.m_AreaSearchTree,
				m_WaterSurfaceData = validationJob.m_WaterSurfaceData,
				m_TerrainHeightData = validationJob.m_TerrainHeightData,
				m_ErrorQueue = errorQueue2.AsParallelWriter()
			};
			JobHandle val6 = JobChunkExtensions.Schedule<CollectAreaTrianglesJob>(collectAreaTrianglesJob, m_UpdatedAreaQuery, ((SystemBase)this).Dependency);
			val4 = IJobParallelForDeferExtensions.Schedule<ValidateAreaTrianglesJob, AreaSearchItem>(obj, val5, 1, JobHandle.CombineDependencies(val6, val3));
			val5.Dispose(val4);
		}
		FillErrorPrefabsJob fillErrorPrefabsJob = new FillErrorPrefabsJob
		{
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ToolErrorType = InternalCompilerInterface.GetComponentTypeHandle<ToolErrorData>(ref __TypeHandle.__Game_Prefabs_ToolErrorData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ErrorPrefabs = errorPrefabs
		};
		ProcessValidationResultsJob obj2 = new ProcessValidationResultsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BrushType = InternalCompilerInterface.GetComponentTypeHandle<Brush>(ref __TypeHandle.__Game_Tools_Brush_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlayerMoney = InternalCompilerInterface.GetComponentLookup<PlayerMoney>(ref __TypeHandle.__Game_City_PlayerMoney_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Chunks = val,
			m_City = m_CitySystem.City,
			m_ErrorMap = m_Components.m_ErrorMap,
			m_ErrorPrefabs = errorPrefabs,
			m_ErrorQueue1 = errorQueue,
			m_ErrorQueue2 = errorQueue2,
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer(),
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer()
		};
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(((SystemBase)this).Dependency, val2);
		JobHandle val7 = IJobExtensions.Schedule<BoundsListJob>(boundsListJob, ((SystemBase)this).Dependency);
		JobHandle val8 = IJobParallelForDeferExtensions.Schedule<ValidationJob, ArchetypeChunk>(validationJob, val, 1, JobHandle.CombineDependencies(val7, val3));
		val8 = JobHandle.CombineDependencies(val8, val4);
		JobHandle val9 = JobChunkExtensions.Schedule<FillErrorPrefabsJob>(fillErrorPrefabsJob, m_ToolErrorPrefabQuery, ((SystemBase)this).Dependency);
		JobHandle val10 = IJobExtensions.Schedule<ProcessValidationResultsJob>(obj2, JobHandle.CombineDependencies(val8, val9));
		edgeList.Dispose(val8);
		objectList.Dispose(val8);
		errorQueue.Dispose(val10);
		errorQueue2.Dispose(val10);
		val.Dispose(val10);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val8);
		m_NetSearchSystem.AddNetSearchTreeReader(val8);
		m_AreaSearchSystem.AddSearchTreeReader(val8);
		m_InstanceCountSystem.AddCountReader(val8);
		m_WaterSystem.AddSurfaceReader(val8);
		m_TerrainSystem.AddCPUHeightReader(val8);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val10);
		m_IconCommandSystem.AddCommandBufferWriter(val10);
		m_Components.m_ErrorMapDeps = val10;
		((SystemBase)this).Dependency = val10;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public ValidationSystem()
	{
	}
}

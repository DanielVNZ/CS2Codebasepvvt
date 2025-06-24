using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
using Game.Simulation;
using Game.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class LineVisualizerSection : InfoSectionBase
{
	private readonly struct LineStop
	{
		public Entity entity { get; }

		public float position { get; }

		public int cargo { get; }

		public bool isCargo { get; }

		public bool isOutsideConnection { get; }

		public int capacity { get; }

		public LineStop(Entity entity, float position, int cargo, int capacity, bool isCargo = false, bool isOutsideConnection = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			this.entity = entity;
			this.position = position;
			this.cargo = cargo;
			this.isCargo = isCargo;
			this.isOutsideConnection = isOutsideConnection;
			this.capacity = capacity;
		}

		public void Bind(IJsonWriter binder, NameSystem nameSystem)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			binder.TypeBegin(GetType().FullName);
			binder.PropertyName("entity");
			UnityWriters.Write(binder, entity);
			binder.PropertyName("name");
			nameSystem.BindName(binder, entity);
			binder.PropertyName("position");
			binder.Write(position);
			binder.PropertyName("cargo");
			binder.Write(cargo);
			binder.PropertyName("capacity");
			binder.Write(capacity);
			binder.PropertyName("isCargo");
			binder.Write(isCargo);
			binder.PropertyName("isOutsideConnection");
			binder.Write(isOutsideConnection);
			binder.TypeEnd();
		}
	}

	private readonly struct LineVehicle
	{
		public Entity entity { get; }

		public float position { get; }

		public int cargo { get; }

		public int capacity { get; }

		public bool isCargo { get; }

		public LineVehicle(Entity entity, float position, int cargo, int capacity, bool isCargo = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			this.entity = entity;
			this.position = position;
			this.cargo = cargo;
			this.capacity = capacity;
			this.isCargo = isCargo;
		}

		public void Bind(IJsonWriter binder, NameSystem nameSystem)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			binder.TypeBegin(GetType().FullName);
			binder.PropertyName("entity");
			UnityWriters.Write(binder, entity);
			binder.PropertyName("name");
			nameSystem.BindName(binder, entity);
			binder.PropertyName("cargo");
			binder.Write(cargo);
			binder.PropertyName("capacity");
			binder.Write(capacity);
			binder.PropertyName("position");
			binder.Write(position);
			binder.PropertyName("isCargo");
			binder.Write(isCargo);
			binder.TypeEnd();
		}
	}

	private readonly struct LineSegment : IJsonWritable
	{
		public float start { get; }

		public float end { get; }

		public bool broken { get; }

		public LineSegment(float start, float end, bool broken)
		{
			this.start = start;
			this.end = end;
			this.broken = broken;
		}

		public void Write(IJsonWriter binder)
		{
			binder.TypeBegin(GetType().FullName);
			binder.PropertyName("start");
			binder.Write(start);
			binder.PropertyName("end");
			binder.Write(end);
			binder.PropertyName("broken");
			binder.Write(broken);
			binder.TypeEnd();
		}
	}

	private enum Result
	{
		IsVisible = 0,
		ShouldUpdateSelected = 1,
		IsCargo = 2,
		Entity = 0,
		Color = 0,
		StopsCapacity = 0
	}

	[BurstCompile]
	private struct VisibilityJob : IJob
	{
		[ReadOnly]
		public Entity m_SelectedEntity;

		[ReadOnly]
		public Entity m_SelectedRouteEntity;

		[ReadOnly]
		public ComponentLookup<Route> m_Routes;

		[ReadOnly]
		public ComponentLookup<TransportLine> m_TransportLines;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TransportStop> m_TransportStops;

		[ReadOnly]
		public ComponentLookup<TaxiStand> m_TaxiStands;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_Vehicles;

		[ReadOnly]
		public ComponentLookup<Owner> m_Owners;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransports;

		[ReadOnly]
		public ComponentLookup<CurrentRoute> m_CurrentRoutes;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypointBuffers;

		[ReadOnly]
		public BufferLookup<RouteSegment> m_RouteSegmentBuffers;

		[ReadOnly]
		public BufferLookup<RouteVehicle> m_RouteVehicleBuffers;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> m_ConnectedRouteBuffers;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjectBuffers;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgradeBuffers;

		public NativeArray<bool> m_BoolResult;

		public NativeArray<Entity> m_EntityResult;

		public void Execute()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			if (IsLine(m_SelectedEntity))
			{
				m_BoolResult[0] = true;
				m_BoolResult[1] = true;
				m_EntityResult[0] = m_SelectedEntity;
				return;
			}
			NativeList<ConnectedRoute> connectedRoutes = default(NativeList<ConnectedRoute>);
			connectedRoutes._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			bool num = TryGetStationRoutes(m_SelectedEntity, connectedRoutes);
			bool flag = TryGetStopRoutes(m_SelectedEntity, connectedRoutes);
			if (num || flag)
			{
				bool flag2 = false;
				Entity val = Entity.Null;
				Owner owner = default(Owner);
				for (int num2 = connectedRoutes.Length - 1; num2 >= 0; num2--)
				{
					if (m_Owners.TryGetComponent(connectedRoutes[num2].m_Waypoint, ref owner) && IsLine(owner.m_Owner))
					{
						val = owner.m_Owner;
						if (val == m_SelectedRouteEntity)
						{
							flag2 = true;
						}
					}
				}
				if (val != Entity.Null)
				{
					if (!flag2)
					{
						m_BoolResult[0] = true;
						m_BoolResult[1] = true;
						m_EntityResult[0] = val;
					}
					else
					{
						m_BoolResult[0] = true;
						m_BoolResult[1] = false;
						m_EntityResult[0] = Entity.Null;
					}
					return;
				}
			}
			if (IsVehicle(out var routeEntity))
			{
				m_BoolResult[0] = true;
				m_BoolResult[1] = true;
				m_EntityResult[0] = routeEntity;
			}
			else
			{
				m_BoolResult[0] = false;
				m_BoolResult[1] = false;
				m_EntityResult[0] = Entity.Null;
			}
		}

		private bool IsLine(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if (m_Routes.HasComponent(entity) && m_TransportLines.HasComponent(entity) && m_RouteWaypointBuffers.HasBuffer(entity) && m_RouteSegmentBuffers.HasBuffer(entity))
			{
				return m_RouteVehicleBuffers.HasBuffer(entity);
			}
			return false;
		}

		private bool TryGetStationRoutes(Entity entity, NativeList<ConnectedRoute> connectedRoutes)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjectBuffers.TryGetBuffer(entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					TryGetStopRoutes(val[i].m_SubObject, connectedRoutes);
				}
			}
			DynamicBuffer<InstalledUpgrade> val2 = default(DynamicBuffer<InstalledUpgrade>);
			if (m_InstalledUpgradeBuffers.TryGetBuffer(entity, ref val2))
			{
				Enumerator<InstalledUpgrade> enumerator = val2.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						TryGetStationRoutes(enumerator.Current.m_Upgrade, connectedRoutes);
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			return connectedRoutes.Length > 0;
		}

		private bool TryGetStopRoutes(Entity entity, NativeList<ConnectedRoute> connectedRoutes)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedRoute> val = default(DynamicBuffer<ConnectedRoute>);
			if (m_ConnectedRouteBuffers.TryGetBuffer(entity, ref val) && m_TransportStops.HasComponent(entity) && !m_TaxiStands.HasComponent(entity) && val.Length > 0)
			{
				connectedRoutes.AddRange(val.AsNativeArray());
				return true;
			}
			return false;
		}

		private bool IsVehicle(out Entity routeEntity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			CurrentRoute currentRoute = default(CurrentRoute);
			if (m_Vehicles.HasComponent(m_SelectedEntity) && m_Owners.HasComponent(m_SelectedEntity) && m_PublicTransports.HasComponent(m_SelectedEntity) && m_CurrentRoutes.TryGetComponent(m_SelectedEntity, ref currentRoute) && IsLine(currentRoute.m_Route))
			{
				routeEntity = currentRoute.m_Route;
				return true;
			}
			routeEntity = Entity.Null;
			return false;
		}
	}

	[BurstCompile]
	private struct UpdateJob : IJob
	{
		[ReadOnly]
		public bool m_RightHandTraffic;

		[ReadOnly]
		public Entity m_RouteEntity;

		[ReadOnly]
		public uint m_RenderingFrameIndex;

		[ReadOnly]
		public float m_RenderingFrameTime;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrames;

		[ReadOnly]
		public ComponentLookup<Game.Routes.Color> m_Colors;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformation;

		[ReadOnly]
		public ComponentLookup<Connected> m_Connected;

		[ReadOnly]
		public ComponentLookup<WaitingPassengers> m_WaitingPassengers;

		[ReadOnly]
		public ComponentLookup<Position> m_Positions;

		[ReadOnly]
		public ComponentLookup<RouteLane> m_RouteLanes;

		[ReadOnly]
		public ComponentLookup<CurrentRoute> m_CurrentRoutes;

		[ReadOnly]
		public ComponentLookup<Target> m_Targets;

		[ReadOnly]
		public ComponentLookup<PathOwner> m_PathOwners;

		[ReadOnly]
		public ComponentLookup<Owner> m_Owners;

		[ReadOnly]
		public ComponentLookup<Waypoint> m_Waypoints;

		[ReadOnly]
		public ComponentLookup<Train> m_Trains;

		[ReadOnly]
		public ComponentLookup<Curve> m_Curves;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLanes;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> m_CarCurrentLanes;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> m_TrainCurrentLanes;

		[ReadOnly]
		public ComponentLookup<WatercraftCurrentLane> m_WatercraftCurrentLanes;

		[ReadOnly]
		public ComponentLookup<AircraftCurrentLane> m_AircraftCurrentLanes;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Pet> m_Pets;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_TransportLineData;

		[ReadOnly]
		public ComponentLookup<TrainData> m_TrainDatas;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> m_PublicTransportVehicleDatas;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> m_CargoTransportVehicleDatas;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TransportStop> m_TransportStops;

		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfos;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_StorageLimitDatas;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<Resources> m_EconomyResourcesBuffers;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypointBuffers;

		[ReadOnly]
		public BufferLookup<RouteSegment> m_RouteSegmentBuffers;

		[ReadOnly]
		public BufferLookup<RouteVehicle> m_RouteVehicleBuffers;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElementBuffers;

		[ReadOnly]
		public BufferLookup<CarNavigationLane> m_CarNavigationLaneBuffers;

		[ReadOnly]
		public BufferLookup<TrainNavigationLane> m_TrainNavigationLaneBuffers;

		[ReadOnly]
		public BufferLookup<WatercraftNavigationLane> m_WatercraftNavigationLaneBuffers;

		[ReadOnly]
		public BufferLookup<AircraftNavigationLane> m_AircraftNavigationLaneBuffers;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElementBuffers;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLaneBuffers;

		[ReadOnly]
		public BufferLookup<Passenger> m_PassengerBuffers;

		[ReadOnly]
		public BufferLookup<TransformFrame> m_TransformFrames;

		public NativeList<LineSegment> m_SegmentsResult;

		public NativeList<LineStop> m_StopsResult;

		public NativeList<LineVehicle> m_VehiclesResult;

		public NativeArray<Color32> m_ColorResult;

		public NativeArray<int> m_StopCapacityResult;

		public NativeArray<bool> m_BoolResult;

		public void Execute()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			NativeList<float> val = default(NativeList<float>);
			val._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
			float num = 0f;
			m_BoolResult[2] = false;
			m_ColorResult[0] = m_Colors[m_RouteEntity].m_Color;
			m_StopCapacityResult[0] = 0;
			DynamicBuffer<RouteWaypoint> waypoints = m_RouteWaypointBuffers[m_RouteEntity];
			DynamicBuffer<RouteSegment> routeSegments = m_RouteSegmentBuffers[m_RouteEntity];
			DynamicBuffer<RouteVehicle> val2 = m_RouteVehicleBuffers[m_RouteEntity];
			for (int i = 0; i < routeSegments.Length; i++)
			{
				val.Add(ref num);
				num += GetSegmentLength(waypoints, routeSegments, i);
			}
			if (num == 0f)
			{
				return;
			}
			PathInformation pathInformation = default(PathInformation);
			for (int j = 0; j < routeSegments.Length; j++)
			{
				if (m_PathInformation.TryGetComponent(routeSegments[j].m_Segment, ref pathInformation))
				{
					float start = val[j] / num;
					float end = ((j < routeSegments.Length - 1) ? (val[j + 1] / num) : 1f);
					bool broken = pathInformation.m_Origin == Entity.Null && pathInformation.m_Destination == Entity.Null;
					ref NativeList<LineSegment> reference = ref m_SegmentsResult;
					LineSegment lineSegment = new LineSegment(start, end, broken);
					reference.Add(ref lineSegment);
				}
			}
			PrefabRef prefabRef = default(PrefabRef);
			TransportLineData transportLineData = default(TransportLineData);
			bool flag = m_PrefabRefs.TryGetComponent(m_RouteEntity, ref prefabRef) && m_TransportLineData.TryGetComponent(prefabRef.m_Prefab, ref transportLineData) && transportLineData.m_CargoTransport;
			for (int k = 0; k < val2.Length; k++)
			{
				Entity vehicle = val2[k].m_Vehicle;
				if (GetVehiclePosition(m_RouteEntity, vehicle, out var prevWaypointIndex, out var distanceFromWaypoint, out var distanceToWaypoint, out var unknownPath))
				{
					int num2 = prevWaypointIndex;
					float segmentLength = GetSegmentLength(waypoints, routeSegments, num2);
					float num3 = val[num2];
					num3 = ((!unknownPath) ? (num3 + (segmentLength - distanceToWaypoint)) : (num3 + segmentLength * distanceFromWaypoint / math.max(1f, distanceFromWaypoint + distanceToWaypoint)));
					(int, int) cargo = GetCargo(vehicle);
					int item = cargo.Item1;
					int item2 = cargo.Item2;
					float num4 = math.frac(num3 / num);
					ref NativeList<LineVehicle> reference2 = ref m_VehiclesResult;
					LineVehicle lineVehicle = new LineVehicle(vehicle, m_RightHandTraffic ? (1f - num4) : num4, item, item2, flag);
					reference2.Add(ref lineVehicle);
					if (item2 > m_StopCapacityResult[0])
					{
						m_StopCapacityResult[0] = item2;
					}
				}
			}
			Connected connected = default(Connected);
			WaitingPassengers waitingPassengers = default(WaitingPassengers);
			DynamicBuffer<Resources> val4 = default(DynamicBuffer<Resources>);
			Owner owner = default(Owner);
			DynamicBuffer<Resources> val5 = default(DynamicBuffer<Resources>);
			Owner owner2 = default(Owner);
			DynamicBuffer<Resources> val6 = default(DynamicBuffer<Resources>);
			PrefabRef prefabRef2 = default(PrefabRef);
			for (int l = 0; l < waypoints.Length; l++)
			{
				if (!m_Connected.TryGetComponent(waypoints[l].m_Waypoint, ref connected) || !m_TransportStops.HasComponent(connected.m_Connected))
				{
					continue;
				}
				float num5 = val[l] / num;
				int num6 = 0;
				int capacity = 80;
				Entity val3 = connected.m_Connected;
				if (!flag && m_WaitingPassengers.TryGetComponent(waypoints[l].m_Waypoint, ref waitingPassengers))
				{
					num6 = waitingPassengers.m_Count;
					capacity = 80;
				}
				else if (m_EconomyResourcesBuffers.TryGetBuffer(connected.m_Connected, ref val4))
				{
					for (int m = 0; m < val4.Length; m++)
					{
						num6 += val4[m].m_Amount;
					}
				}
				else if (m_Owners.TryGetComponent(connected.m_Connected, ref owner))
				{
					if (m_EconomyResourcesBuffers.TryGetBuffer(owner.m_Owner, ref val5))
					{
						for (int n = 0; n < val5.Length; n++)
						{
							num6 += val5[n].m_Amount;
						}
						val3 = owner.m_Owner;
					}
					else if (m_Owners.TryGetComponent(owner.m_Owner, ref owner2) && m_EconomyResourcesBuffers.TryGetBuffer(owner2.m_Owner, ref val6))
					{
						for (int num7 = 0; num7 < val6.Length; num7++)
						{
							num6 += val6[num7].m_Amount;
						}
						val3 = owner2.m_Owner;
					}
				}
				if (flag && m_PrefabRefs.TryGetComponent(val3, ref prefabRef2))
				{
					capacity = GetStorageLimit(val3, prefabRef2.m_Prefab);
				}
				ref NativeList<LineStop> reference3 = ref m_StopsResult;
				LineStop lineStop = new LineStop(val3, m_RightHandTraffic ? (1f - num5) : num5, num6, capacity, flag, m_OutsideConnections.HasComponent(val3));
				reference3.Add(ref lineStop);
			}
			m_BoolResult[2] = flag;
		}

		private float GetSegmentLength(DynamicBuffer<RouteWaypoint> waypoints, DynamicBuffer<RouteSegment> routeSegments, int segmentIndex)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			int num = math.select(segmentIndex + 1, 0, segmentIndex == waypoints.Length - 1);
			PathInformation pathInformation = default(PathInformation);
			if (m_PathInformation.TryGetComponent(routeSegments[segmentIndex].m_Segment, ref pathInformation) && pathInformation.m_Destination != Entity.Null)
			{
				float num2 = pathInformation.m_Distance;
				Entity waypoint = waypoints[num].m_Waypoint;
				RouteLane routeLane = default(RouteLane);
				Curve curve = default(Curve);
				Curve curve2 = default(Curve);
				if (m_RouteLanes.TryGetComponent(waypoint, ref routeLane) && m_Curves.TryGetComponent(routeLane.m_StartLane, ref curve) && m_Curves.TryGetComponent(routeLane.m_EndLane, ref curve2))
				{
					num2 += math.distance(MathUtils.Position(curve.m_Bezier, routeLane.m_StartCurvePos), MathUtils.Position(curve2.m_Bezier, routeLane.m_EndCurvePos));
				}
				return num2;
			}
			if (GetWaypointPosition(waypoints[segmentIndex].m_Waypoint, out var position) && GetWaypointPosition(waypoints[num].m_Waypoint, out var position2))
			{
				return math.max(0f, math.distance(position, position2));
			}
			return 0f;
		}

		private bool GetWaypointPosition(Entity waypoint, out float3 position)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			Position position2 = default(Position);
			if (m_Positions.TryGetComponent(waypoint, ref position2))
			{
				RouteLane routeLane = default(RouteLane);
				Curve curve = default(Curve);
				if (m_RouteLanes.TryGetComponent(waypoint, ref routeLane) && m_Curves.TryGetComponent(routeLane.m_EndLane, ref curve))
				{
					position = MathUtils.Position(curve.m_Bezier, routeLane.m_EndCurvePos);
					return true;
				}
				position = position2.m_Position;
				return true;
			}
			position = default(float3);
			return false;
		}

		private int GetStorageLimit(Entity entity, Entity prefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if (m_StorageLimitDatas.HasComponent(prefab))
			{
				StorageLimitData data = m_StorageLimitDatas[prefab];
				if (m_InstalledUpgrades.HasBuffer(entity))
				{
					UpgradeUtils.CombineStats<StorageLimitData>(ref data, m_InstalledUpgrades[entity], ref m_PrefabRefs, ref m_StorageLimitDatas);
				}
				return data.m_Limit;
			}
			return 80;
		}

		private bool GetVehiclePosition(Entity transportRoute, Entity transportVehicle, out int prevWaypointIndex, out float distanceFromWaypoint, out float distanceToWaypoint, out bool unknownPath)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			prevWaypointIndex = 0;
			distanceFromWaypoint = 0f;
			distanceToWaypoint = 0f;
			unknownPath = true;
			CurrentRoute currentRoute = default(CurrentRoute);
			if (!m_CurrentRoutes.TryGetComponent(transportVehicle, ref currentRoute))
			{
				return false;
			}
			Target target = default(Target);
			if (!m_Targets.TryGetComponent(transportVehicle, ref target))
			{
				return false;
			}
			PathOwner pathOwner = default(PathOwner);
			if (!m_PathOwners.TryGetComponent(transportVehicle, ref pathOwner))
			{
				return false;
			}
			Waypoint waypoint = default(Waypoint);
			if (!m_Waypoints.TryGetComponent(target.m_Target, ref waypoint))
			{
				return false;
			}
			if (!GetWaypointPosition(target.m_Target, out var position))
			{
				return false;
			}
			DynamicBuffer<RouteWaypoint> val = default(DynamicBuffer<RouteWaypoint>);
			if (!m_RouteWaypointBuffers.TryGetBuffer(transportRoute, ref val))
			{
				return false;
			}
			if (currentRoute.m_Route != transportRoute)
			{
				return false;
			}
			Entity val2 = transportVehicle;
			DynamicBuffer<LayoutElement> val3 = default(DynamicBuffer<LayoutElement>);
			PrefabRef prefabRef2 = default(PrefabRef);
			TrainData trainData2 = default(TrainData);
			if (m_LayoutElementBuffers.TryGetBuffer(transportVehicle, ref val3) && val3.Length != 0)
			{
				PrefabRef prefabRef = default(PrefabRef);
				TrainData trainData = default(TrainData);
				for (int i = 0; i < val3.Length; i++)
				{
					if (m_PrefabRefs.TryGetComponent(val3[i].m_Vehicle, ref prefabRef) && m_TrainDatas.TryGetComponent(prefabRef.m_Prefab, ref trainData))
					{
						float num = math.csum(trainData.m_AttachOffsets);
						distanceFromWaypoint -= num * 0.5f;
						distanceToWaypoint -= num * 0.5f;
					}
				}
				val2 = val3[0].m_Vehicle;
			}
			else if (m_PrefabRefs.TryGetComponent(transportVehicle, ref prefabRef2) && m_TrainDatas.TryGetComponent(prefabRef2.m_Prefab, ref trainData2))
			{
				float num2 = math.csum(trainData2.m_AttachOffsets);
				distanceFromWaypoint -= num2 * 0.5f;
				distanceToWaypoint -= num2 * 0.5f;
			}
			Train train = default(Train);
			PrefabRef prefabRef3 = default(PrefabRef);
			TrainData trainData3 = default(TrainData);
			if (m_Trains.TryGetComponent(val2, ref train) && m_PrefabRefs.TryGetComponent(val2, ref prefabRef3) && m_TrainDatas.TryGetComponent(prefabRef3.m_Prefab, ref trainData3))
			{
				if ((train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0)
				{
					distanceToWaypoint -= trainData3.m_AttachOffsets.y;
				}
				else
				{
					distanceToWaypoint -= trainData3.m_AttachOffsets.x;
				}
			}
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(val2))
			{
				return false;
			}
			ArchetypeChunk chunk = ((EntityStorageInfoLookup)(ref m_EntityLookup))[val2].Chunk;
			DynamicBuffer<TransformFrame> val4 = default(DynamicBuffer<TransformFrame>);
			float3 position2;
			if (m_TransformFrames.TryGetBuffer(val2, ref val4) && ((ArchetypeChunk)(ref chunk)).Has<UpdateFrame>(m_UpdateFrames))
			{
				UpdateFrame sharedComponent = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrames);
				ObjectInterpolateSystem.CalculateUpdateFrames(m_RenderingFrameIndex, m_RenderingFrameTime, sharedComponent.m_Index, out var updateFrame, out var updateFrame2, out var framePosition);
				TransformFrame frame = val4[(int)updateFrame];
				TransformFrame frame2 = val4[(int)updateFrame2];
				position2 = ObjectInterpolateSystem.CalculateTransform(frame, frame2, framePosition).m_Position;
			}
			else
			{
				Transform transform = default(Transform);
				if (!m_Transforms.TryGetComponent(val2, ref transform))
				{
					return false;
				}
				position2 = transform.m_Position;
			}
			prevWaypointIndex = math.select(waypoint.m_Index - 1, val.Length - 1, waypoint.m_Index == 0);
			if (prevWaypointIndex >= val.Length)
			{
				return false;
			}
			if (!GetWaypointPosition(val[prevWaypointIndex].m_Waypoint, out var position3))
			{
				return false;
			}
			distanceFromWaypoint += math.distance(position3, position2);
			float3 position4 = position2;
			if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete | PathFlags.Updated | PathFlags.DivertObsolete)) == 0 || (pathOwner.m_State & (PathFlags.Failed | PathFlags.Append)) == PathFlags.Append)
			{
				unknownPath = false;
				CarCurrentLane carCurrentLane = default(CarCurrentLane);
				TrainCurrentLane trainCurrentLane = default(TrainCurrentLane);
				WatercraftCurrentLane watercraftCurrentLane = default(WatercraftCurrentLane);
				AircraftCurrentLane aircraftCurrentLane = default(AircraftCurrentLane);
				if (m_CarCurrentLanes.TryGetComponent(val2, ref carCurrentLane))
				{
					AddDistance(ref distanceToWaypoint, ref position4, carCurrentLane.m_Lane, ((float3)(ref carCurrentLane.m_CurvePosition)).xz);
				}
				else if (m_TrainCurrentLanes.TryGetComponent(val2, ref trainCurrentLane))
				{
					AddDistance(ref distanceToWaypoint, ref position4, trainCurrentLane.m_Front.m_Lane, ((float4)(ref trainCurrentLane.m_Front.m_CurvePosition)).yw);
				}
				else if (m_WatercraftCurrentLanes.TryGetComponent(val2, ref watercraftCurrentLane))
				{
					AddDistance(ref distanceToWaypoint, ref position4, watercraftCurrentLane.m_Lane, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).xz);
				}
				else if (m_AircraftCurrentLanes.TryGetComponent(val2, ref aircraftCurrentLane))
				{
					AddDistance(ref distanceToWaypoint, ref position4, aircraftCurrentLane.m_Lane, ((float3)(ref aircraftCurrentLane.m_CurvePosition)).xz);
				}
				DynamicBuffer<CarNavigationLane> val5 = default(DynamicBuffer<CarNavigationLane>);
				DynamicBuffer<TrainNavigationLane> val6 = default(DynamicBuffer<TrainNavigationLane>);
				DynamicBuffer<WatercraftNavigationLane> val7 = default(DynamicBuffer<WatercraftNavigationLane>);
				DynamicBuffer<AircraftNavigationLane> val8 = default(DynamicBuffer<AircraftNavigationLane>);
				if (m_CarNavigationLaneBuffers.TryGetBuffer(transportVehicle, ref val5))
				{
					for (int j = 0; j < val5.Length; j++)
					{
						CarNavigationLane carNavigationLane = val5[j];
						AddDistance(ref distanceToWaypoint, ref position4, carNavigationLane.m_Lane, carNavigationLane.m_CurvePosition);
					}
				}
				else if (m_TrainNavigationLaneBuffers.TryGetBuffer(transportVehicle, ref val6))
				{
					for (int k = 0; k < val6.Length; k++)
					{
						TrainNavigationLane trainNavigationLane = val6[k];
						AddDistance(ref distanceToWaypoint, ref position4, trainNavigationLane.m_Lane, trainNavigationLane.m_CurvePosition);
					}
				}
				else if (m_WatercraftNavigationLaneBuffers.TryGetBuffer(transportVehicle, ref val7))
				{
					for (int l = 0; l < val7.Length; l++)
					{
						WatercraftNavigationLane watercraftNavigationLane = val7[l];
						AddDistance(ref distanceToWaypoint, ref position4, watercraftNavigationLane.m_Lane, watercraftNavigationLane.m_CurvePosition);
					}
				}
				else if (m_AircraftNavigationLaneBuffers.TryGetBuffer(transportVehicle, ref val8))
				{
					for (int m = 0; m < val8.Length; m++)
					{
						AircraftNavigationLane aircraftNavigationLane = val8[m];
						AddDistance(ref distanceToWaypoint, ref position4, aircraftNavigationLane.m_Lane, aircraftNavigationLane.m_CurvePosition);
					}
				}
				DynamicBuffer<PathElement> val9 = default(DynamicBuffer<PathElement>);
				if (m_PathElementBuffers.TryGetBuffer(transportVehicle, ref val9))
				{
					for (int n = pathOwner.m_ElementIndex; n < val9.Length; n++)
					{
						PathElement pathElement = val9[n];
						AddDistance(ref distanceToWaypoint, ref position4, pathElement.m_Target, pathElement.m_TargetDelta);
					}
				}
				DynamicBuffer<RouteSegment> val10 = default(DynamicBuffer<RouteSegment>);
				if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Obsolete)) != 0 && (pathOwner.m_State & PathFlags.Append) != 0 && m_RouteSegmentBuffers.TryGetBuffer(transportRoute, ref val10) && m_PathElementBuffers.TryGetBuffer(val10[prevWaypointIndex].m_Segment, ref val9))
				{
					for (int num3 = 0; num3 < val9.Length; num3++)
					{
						PathElement pathElement2 = val9[num3];
						AddDistance(ref distanceToWaypoint, ref position4, pathElement2.m_Target, pathElement2.m_TargetDelta);
					}
				}
			}
			distanceToWaypoint += math.distance(position4, position);
			distanceFromWaypoint = math.max(0f, distanceFromWaypoint);
			distanceToWaypoint = math.max(0f, distanceToWaypoint);
			return true;
		}

		private void AddDistance(ref float distance, ref float3 position, Entity lane, float2 curveDelta)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			SlaveLane slaveLane = default(SlaveLane);
			Owner owner = default(Owner);
			DynamicBuffer<Game.Net.SubLane> val = default(DynamicBuffer<Game.Net.SubLane>);
			if (m_SlaveLanes.TryGetComponent(lane, ref slaveLane) && m_Owners.TryGetComponent(lane, ref owner) && m_SubLaneBuffers.TryGetBuffer(owner.m_Owner, ref val) && slaveLane.m_MasterIndex < val.Length)
			{
				lane = val[(int)slaveLane.m_MasterIndex].m_SubLane;
			}
			Curve curve = default(Curve);
			if (m_Curves.TryGetComponent(lane, ref curve))
			{
				distance += math.distance(position, MathUtils.Position(curve.m_Bezier, curveDelta.x));
				if ((curveDelta.x == 0f && curveDelta.y == 1f) || (curveDelta.x == 1f && curveDelta.y == 0f))
				{
					distance += curve.m_Length;
				}
				else
				{
					distance += MathUtils.Length(curve.m_Bezier, new Bounds1(curveDelta));
				}
				position = MathUtils.Position(curve.m_Bezier, curveDelta.y);
			}
		}

		private (int, int) GetCargo(Entity entity)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			PrefabRef prefabRef = default(PrefabRef);
			if (m_PrefabRefs.TryGetComponent(entity, ref prefabRef))
			{
				DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
				if (m_LayoutElementBuffers.TryGetBuffer(entity, ref val))
				{
					DynamicBuffer<Passenger> val2 = default(DynamicBuffer<Passenger>);
					DynamicBuffer<Resources> val3 = default(DynamicBuffer<Resources>);
					PrefabRef prefabRef2 = default(PrefabRef);
					PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
					CargoTransportVehicleData cargoTransportVehicleData = default(CargoTransportVehicleData);
					for (int i = 0; i < val.Length; i++)
					{
						Entity vehicle = val[i].m_Vehicle;
						if (m_PassengerBuffers.TryGetBuffer(vehicle, ref val2))
						{
							for (int j = 0; j < val2.Length; j++)
							{
								if (!m_Pets.HasComponent(val2[j].m_Passenger))
								{
									num++;
								}
							}
						}
						else if (m_EconomyResourcesBuffers.TryGetBuffer(vehicle, ref val3))
						{
							for (int k = 0; k < val3.Length; k++)
							{
								num += val3[k].m_Amount;
							}
						}
						if (m_PrefabRefs.TryGetComponent(vehicle, ref prefabRef2))
						{
							Entity prefab = prefabRef2.m_Prefab;
							if (m_PublicTransportVehicleDatas.TryGetComponent(prefab, ref publicTransportVehicleData))
							{
								num2 += publicTransportVehicleData.m_PassengerCapacity;
							}
							else if (m_CargoTransportVehicleDatas.TryGetComponent(prefab, ref cargoTransportVehicleData))
							{
								num2 += cargoTransportVehicleData.m_CargoCapacity;
							}
						}
					}
				}
				else
				{
					DynamicBuffer<Passenger> val4 = default(DynamicBuffer<Passenger>);
					DynamicBuffer<Resources> val5 = default(DynamicBuffer<Resources>);
					if (m_PassengerBuffers.TryGetBuffer(entity, ref val4))
					{
						for (int l = 0; l < val4.Length; l++)
						{
							if (!m_Pets.HasComponent(val4[l].m_Passenger))
							{
								num++;
							}
						}
					}
					else if (m_EconomyResourcesBuffers.TryGetBuffer(entity, ref val5))
					{
						for (int m = 0; m < val5.Length; m++)
						{
							num += val5[m].m_Amount;
						}
					}
					PublicTransportVehicleData publicTransportVehicleData2 = default(PublicTransportVehicleData);
					CargoTransportVehicleData cargoTransportVehicleData2 = default(CargoTransportVehicleData);
					if (m_PublicTransportVehicleDatas.TryGetComponent(prefabRef.m_Prefab, ref publicTransportVehicleData2))
					{
						num2 = publicTransportVehicleData2.m_PassengerCapacity;
					}
					else if (m_CargoTransportVehicleDatas.TryGetComponent(prefabRef.m_Prefab, ref cargoTransportVehicleData2))
					{
						num2 += cargoTransportVehicleData2.m_CargoCapacity;
					}
				}
			}
			return (num, num2);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Route> __Game_Routes_Route_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportLine> __Game_Routes_TransportLine_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TransportStop> __Game_Routes_TransportStop_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TaxiStand> __Game_Routes_TaxiStand_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentRoute> __Game_Routes_CurrentRoute_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteSegment> __Game_Routes_RouteSegment_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteVehicle> __Game_Routes_RouteVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> __Game_Routes_ConnectedRoute_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Routes.Color> __Game_Routes_Color_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaitingPassengers> __Game_Routes_WaitingPassengers_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteLane> __Game_Routes_RouteLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathOwner> __Game_Pathfind_PathOwner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Waypoint> __Game_Routes_Waypoint_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Pet> __Game_Creatures_Pet_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportLineData> __Game_Prefabs_TransportLineData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainData> __Game_Prefabs_TrainData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> __Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> __Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CarNavigationLane> __Game_Vehicles_CarNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<TrainNavigationLane> __Game_Vehicles_TrainNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<WatercraftNavigationLane> __Game_Vehicles_WatercraftNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AircraftNavigationLane> __Game_Vehicles_AircraftNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Passenger> __Game_Vehicles_Passenger_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<TransformFrame> __Game_Objects_TransformFrame_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
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
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			__Game_Routes_Route_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Route>(true);
			__Game_Routes_TransportLine_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLine>(true);
			__Game_Routes_TransportStop_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.TransportStop>(true);
			__Game_Routes_TaxiStand_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TaxiStand>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
			__Game_Routes_CurrentRoute_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentRoute>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Routes_RouteSegment_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteSegment>(true);
			__Game_Routes_RouteVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteVehicle>(true);
			__Game_Routes_ConnectedRoute_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedRoute>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Routes_Color_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.Color>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Routes_WaitingPassengers_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaitingPassengers>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Routes_RouteLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteLane>(true);
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Pathfind_PathOwner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathOwner>(true);
			__Game_Routes_Waypoint_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Waypoint>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Vehicles_CarCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarCurrentLane>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(true);
			__Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftCurrentLane>(true);
			__Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AircraftCurrentLane>(true);
			__Game_Creatures_Pet_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Creatures.Pet>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_TransportLineData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLineData>(true);
			__Game_Prefabs_TrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainData>(true);
			__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PublicTransportVehicleData>(true);
			__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CargoTransportVehicleData>(true);
			__Game_Rendering_CullingInfo_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingInfo>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Vehicles_CarNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CarNavigationLane>(true);
			__Game_Vehicles_TrainNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TrainNavigationLane>(true);
			__Game_Vehicles_WatercraftNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<WatercraftNavigationLane>(true);
			__Game_Vehicles_AircraftNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AircraftNavigationLane>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Vehicles_Passenger_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Passenger>(true);
			__Game_Objects_TransformFrame_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TransformFrame>(true);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
		}
	}

	private RenderingSystem m_RenderingSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private NativeArray<bool> m_BoolResult;

	private NativeArray<Entity> m_EntityResult;

	private NativeList<LineSegment> m_SegmentsResult;

	private NativeList<LineStop> m_StopsResult;

	private NativeList<LineVehicle> m_VehiclesResult;

	private NativeArray<Color32> m_ColorResult;

	private NativeArray<int> m_StopCapacityResult;

	private TypeHandle __TypeHandle;

	protected override string group => "LineVisualizerSection";

	protected override bool displayForDestroyedObjects => true;

	protected override bool displayForOutsideConnections => true;

	private Color color { get; set; }

	private int stopCapacity { get; set; }

	private NativeList<LineStop> stops { get; set; }

	private NativeList<LineVehicle> vehicles { get; set; }

	private NativeList<LineSegment> segments { get; set; }

	protected override void Reset()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		color = default(Color);
		stopCapacity = 0;
		stops.Clear();
		vehicles.Clear();
		segments.Clear();
		m_SegmentsResult.Clear();
		m_StopsResult.Clear();
		m_VehiclesResult.Clear();
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		stops = new NativeList<LineStop>(AllocatorHandle.op_Implicit((Allocator)4));
		vehicles = new NativeList<LineVehicle>(AllocatorHandle.op_Implicit((Allocator)4));
		segments = new NativeList<LineSegment>(AllocatorHandle.op_Implicit((Allocator)4));
		m_BoolResult = new NativeArray<bool>(3, (Allocator)4, (NativeArrayOptions)1);
		m_EntityResult = new NativeArray<Entity>(1, (Allocator)4, (NativeArrayOptions)1);
		m_ColorResult = new NativeArray<Color32>(1, (Allocator)4, (NativeArrayOptions)1);
		m_StopCapacityResult = new NativeArray<int>(1, (Allocator)4, (NativeArrayOptions)1);
		m_SegmentsResult = new NativeList<LineSegment>(AllocatorHandle.op_Implicit((Allocator)4));
		m_StopsResult = new NativeList<LineStop>(AllocatorHandle.op_Implicit((Allocator)4));
		m_VehiclesResult = new NativeList<LineVehicle>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		stops.Dispose();
		vehicles.Dispose();
		segments.Dispose();
		m_BoolResult.Dispose();
		m_EntityResult.Dispose();
		m_ColorResult.Dispose();
		m_StopCapacityResult.Dispose();
		m_SegmentsResult.Dispose();
		m_StopsResult.Dispose();
		m_VehiclesResult.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0792: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobExtensions.Schedule<VisibilityJob>(new VisibilityJob
		{
			m_SelectedEntity = selectedEntity,
			m_SelectedRouteEntity = m_InfoUISystem.selectedRoute,
			m_Routes = InternalCompilerInterface.GetComponentLookup<Route>(ref __TypeHandle.__Game_Routes_Route_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportLines = InternalCompilerInterface.GetComponentLookup<TransportLine>(ref __TypeHandle.__Game_Routes_TransportLine_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStops = InternalCompilerInterface.GetComponentLookup<Game.Routes.TransportStop>(ref __TypeHandle.__Game_Routes_TransportStop_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiStands = InternalCompilerInterface.GetComponentLookup<TaxiStand>(ref __TypeHandle.__Game_Routes_TaxiStand_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Vehicles = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Owners = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransports = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentRoutes = InternalCompilerInterface.GetComponentLookup<CurrentRoute>(ref __TypeHandle.__Game_Routes_CurrentRoute_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypointBuffers = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteSegmentBuffers = InternalCompilerInterface.GetBufferLookup<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteVehicleBuffers = InternalCompilerInterface.GetBufferLookup<RouteVehicle>(ref __TypeHandle.__Game_Routes_RouteVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedRouteBuffers = InternalCompilerInterface.GetBufferLookup<ConnectedRoute>(ref __TypeHandle.__Game_Routes_ConnectedRoute_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectBuffers = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeBuffers = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoolResult = m_BoolResult,
			m_EntityResult = m_EntityResult
		}, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		base.visible = m_BoolResult[0];
		if (base.visible)
		{
			if (m_BoolResult[1])
			{
				m_InfoUISystem.selectedRoute = m_EntityResult[0];
			}
			val = IJobExtensions.Schedule<UpdateJob>(new UpdateJob
			{
				m_RightHandTraffic = !m_CityConfigurationSystem.leftHandTraffic,
				m_RouteEntity = m_InfoUISystem.selectedRoute,
				m_RenderingFrameIndex = m_RenderingSystem.frameIndex,
				m_RenderingFrameTime = m_RenderingSystem.frameTime,
				m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpdateFrames = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Colors = InternalCompilerInterface.GetComponentLookup<Game.Routes.Color>(ref __TypeHandle.__Game_Routes_Color_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathInformation = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Connected = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WaitingPassengers = InternalCompilerInterface.GetComponentLookup<WaitingPassengers>(ref __TypeHandle.__Game_Routes_WaitingPassengers_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Positions = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteLanes = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentRoutes = InternalCompilerInterface.GetComponentLookup<CurrentRoute>(ref __TypeHandle.__Game_Routes_CurrentRoute_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Targets = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathOwners = InternalCompilerInterface.GetComponentLookup<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Owners = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Waypoints = InternalCompilerInterface.GetComponentLookup<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Trains = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Curves = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SlaveLanes = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarCurrentLanes = InternalCompilerInterface.GetComponentLookup<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TrainCurrentLanes = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WatercraftCurrentLanes = InternalCompilerInterface.GetComponentLookup<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AircraftCurrentLanes = InternalCompilerInterface.GetComponentLookup<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Pets = InternalCompilerInterface.GetComponentLookup<Game.Creatures.Pet>(ref __TypeHandle.__Game_Creatures_Pet_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TrainDatas = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PublicTransportVehicleDatas = InternalCompilerInterface.GetComponentLookup<PublicTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CargoTransportVehicleDatas = InternalCompilerInterface.GetComponentLookup<CargoTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CullingInfos = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransportStops = InternalCompilerInterface.GetComponentLookup<Game.Routes.TransportStop>(ref __TypeHandle.__Game_Routes_TransportStop_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EconomyResourcesBuffers = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteWaypointBuffers = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteSegmentBuffers = InternalCompilerInterface.GetBufferLookup<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteVehicleBuffers = InternalCompilerInterface.GetBufferLookup<RouteVehicle>(ref __TypeHandle.__Game_Routes_RouteVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LayoutElementBuffers = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarNavigationLaneBuffers = InternalCompilerInterface.GetBufferLookup<CarNavigationLane>(ref __TypeHandle.__Game_Vehicles_CarNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TrainNavigationLaneBuffers = InternalCompilerInterface.GetBufferLookup<TrainNavigationLane>(ref __TypeHandle.__Game_Vehicles_TrainNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WatercraftNavigationLaneBuffers = InternalCompilerInterface.GetBufferLookup<WatercraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_WatercraftNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AircraftNavigationLaneBuffers = InternalCompilerInterface.GetBufferLookup<AircraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_AircraftNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathElementBuffers = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubLaneBuffers = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PassengerBuffers = InternalCompilerInterface.GetBufferLookup<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformFrames = InternalCompilerInterface.GetBufferLookup<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StorageLimitDatas = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SegmentsResult = m_SegmentsResult,
				m_StopsResult = m_StopsResult,
				m_VehiclesResult = m_VehiclesResult,
				m_ColorResult = m_ColorResult,
				m_StopCapacityResult = m_StopCapacityResult,
				m_BoolResult = m_BoolResult
			}, ((SystemBase)this).Dependency);
			((JobHandle)(ref val)).Complete();
		}
	}

	protected override void OnProcess()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		color = Color32.op_Implicit(m_ColorResult[0]);
		stopCapacity = m_StopCapacityResult[0];
		List<TooltipTags> list = m_InfoUISystem.tooltipTags;
		int item;
		if (!m_BoolResult[2])
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			item = ((!((EntityManager)(ref entityManager)).HasComponent<Game.Routes.TransportStop>(selectedEntity)) ? 1 : 3);
		}
		else
		{
			item = 2;
		}
		list.Add((TooltipTags)item);
		for (int i = 0; i < m_SegmentsResult.Length; i++)
		{
			NativeList<LineSegment> val = segments;
			LineSegment lineSegment = m_SegmentsResult[i];
			val.Add(ref lineSegment);
		}
		for (int j = 0; j < m_VehiclesResult.Length; j++)
		{
			NativeList<LineVehicle> val2 = vehicles;
			LineVehicle lineVehicle = m_VehiclesResult[j];
			val2.Add(ref lineVehicle);
		}
		for (int k = 0; k < m_StopsResult.Length; k++)
		{
			NativeList<LineStop> val3 = stops;
			LineStop lineStop = m_StopsResult[k];
			val3.Add(ref lineStop);
		}
		m_Dirty = true;
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("color");
		UnityWriters.Write(writer, color);
		writer.PropertyName("stops");
		JsonWriterExtensions.ArrayBegin(writer, stops.Length);
		for (int i = 0; i < stops.Length; i++)
		{
			stops[i].Bind(writer, m_NameSystem);
		}
		writer.ArrayEnd();
		writer.PropertyName("vehicles");
		JsonWriterExtensions.ArrayBegin(writer, vehicles.Length);
		for (int j = 0; j < vehicles.Length; j++)
		{
			vehicles[j].Bind(writer, m_NameSystem);
		}
		writer.ArrayEnd();
		writer.PropertyName("segments");
		JsonWriterExtensions.ArrayBegin(writer, segments.Length);
		for (int k = 0; k < segments.Length; k++)
		{
			JsonWriterExtensions.Write<LineSegment>(writer, segments[k]);
		}
		writer.ArrayEnd();
		writer.PropertyName("stopCapacity");
		writer.Write(stopCapacity);
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
	public LineVisualizerSection()
	{
	}
}

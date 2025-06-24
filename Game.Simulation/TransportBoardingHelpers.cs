using System.Collections.Generic;
using System.Runtime.InteropServices;
using Colossal.PSI.Common;
using Game.Achievements;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.Routes;
using Game.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Simulation;

public static class TransportBoardingHelpers
{
	public struct BoardingLookupData
	{
		[ReadOnly]
		public ComponentLookup<TransportLine> m_TransportLineData;

		[ReadOnly]
		public ComponentLookup<Connected> m_ConnectedData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TransportStop> m_TransportStopData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_PrefabTransportLineData;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> m_PrefabCargoTransportVehicleData;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<Aircraft> m_AircraftData;

		[ReadOnly]
		public ComponentLookup<Watercraft> m_WatercraftData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTruckData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public BufferLookup<RouteModifier> m_RouteModifiers;

		public ComponentLookup<Game.Vehicles.CargoTransport> m_CargoTransportData;

		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		public ComponentLookup<BoardingVehicle> m_BoardingVehicleData;

		public ComponentLookup<VehicleTiming> m_VehicleTimingData;

		public ComponentLookup<Game.Buildings.CargoTransportStation> m_CargoTransportStationData;

		public BufferLookup<StorageTransferRequest> m_StorageTransferRequests;

		public BufferLookup<Resources> m_EconomyResources;

		public BufferLookup<LoadingResources> m_LoadingResources;

		public BoardingLookupData(SystemBase system)
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
			m_TransportLineData = system.GetComponentLookup<TransportLine>(true);
			m_ConnectedData = system.GetComponentLookup<Connected>(true);
			m_TransportStopData = system.GetComponentLookup<Game.Routes.TransportStop>(true);
			m_PrefabRefData = system.GetComponentLookup<PrefabRef>(true);
			m_PrefabTransportLineData = system.GetComponentLookup<TransportLineData>(true);
			m_PrefabCargoTransportVehicleData = system.GetComponentLookup<CargoTransportVehicleData>(true);
			m_LayoutElements = system.GetBufferLookup<LayoutElement>(true);
			m_RouteModifiers = system.GetBufferLookup<RouteModifier>(true);
			m_CargoTransportData = system.GetComponentLookup<Game.Vehicles.CargoTransport>(false);
			m_PublicTransportData = system.GetComponentLookup<Game.Vehicles.PublicTransport>(false);
			m_BoardingVehicleData = system.GetComponentLookup<BoardingVehicle>(false);
			m_VehicleTimingData = system.GetComponentLookup<VehicleTiming>(false);
			m_CargoTransportStationData = system.GetComponentLookup<Game.Buildings.CargoTransportStation>(false);
			m_StorageTransferRequests = system.GetBufferLookup<StorageTransferRequest>(false);
			m_EconomyResources = system.GetBufferLookup<Resources>(false);
			m_LoadingResources = system.GetBufferLookup<LoadingResources>(false);
			m_DeliveryTruckData = system.GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			m_OwnerData = system.GetComponentLookup<Owner>(true);
			m_AircraftData = system.GetComponentLookup<Aircraft>(true);
			m_WatercraftData = system.GetComponentLookup<Watercraft>(true);
			m_TrainData = system.GetComponentLookup<Train>(true);
		}

		public void Update(SystemBase system)
		{
			m_TransportLineData.Update(system);
			m_ConnectedData.Update(system);
			m_TransportStopData.Update(system);
			m_PrefabRefData.Update(system);
			m_PrefabTransportLineData.Update(system);
			m_PrefabCargoTransportVehicleData.Update(system);
			m_LayoutElements.Update(system);
			m_RouteModifiers.Update(system);
			m_CargoTransportData.Update(system);
			m_PublicTransportData.Update(system);
			m_BoardingVehicleData.Update(system);
			m_VehicleTimingData.Update(system);
			m_CargoTransportStationData.Update(system);
			m_StorageTransferRequests.Update(system);
			m_EconomyResources.Update(system);
			m_LoadingResources.Update(system);
			m_DeliveryTruckData.Update(system);
			m_OwnerData.Update(system);
			m_AircraftData.Update(system);
			m_WatercraftData.Update(system);
			m_TrainData.Update(system);
		}
	}

	public struct BoardingData
	{
		public struct Concurrent
		{
			private ParallelWriter<BoardingItem> m_BoardingQueue;

			public Concurrent(BoardingData data)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				m_BoardingQueue = data.m_BoardingQueue.AsParallelWriter();
			}

			public void BeginBoarding(Entity vehicle, Entity route, Entity stop, Entity waypoint, Entity currentStation, Entity nextStation, bool refuel)
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				BoardingItem boardingItem = default(BoardingItem);
				boardingItem.m_Begin = true;
				boardingItem.m_Refuel = refuel;
				boardingItem.m_Testing = false;
				boardingItem.m_Vehicle = vehicle;
				boardingItem.m_Route = route;
				boardingItem.m_Stop = stop;
				boardingItem.m_Waypoint = waypoint;
				boardingItem.m_CurrentStation = currentStation;
				boardingItem.m_NextStation = nextStation;
				m_BoardingQueue.Enqueue(boardingItem);
			}

			public void EndBoarding(Entity vehicle, Entity route, Entity stop, Entity waypoint, Entity currentStation, Entity nextStation)
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				BoardingItem boardingItem = default(BoardingItem);
				boardingItem.m_Begin = false;
				boardingItem.m_Refuel = false;
				boardingItem.m_Testing = false;
				boardingItem.m_Vehicle = vehicle;
				boardingItem.m_Route = route;
				boardingItem.m_Stop = stop;
				boardingItem.m_Waypoint = waypoint;
				boardingItem.m_CurrentStation = currentStation;
				boardingItem.m_NextStation = nextStation;
				m_BoardingQueue.Enqueue(boardingItem);
			}

			public void BeginTesting(Entity vehicle, Entity route, Entity stop, Entity waypoint)
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				BoardingItem boardingItem = default(BoardingItem);
				boardingItem.m_Begin = true;
				boardingItem.m_Refuel = false;
				boardingItem.m_Testing = true;
				boardingItem.m_Vehicle = vehicle;
				boardingItem.m_Route = route;
				boardingItem.m_Stop = stop;
				boardingItem.m_Waypoint = waypoint;
				boardingItem.m_CurrentStation = Entity.Null;
				boardingItem.m_NextStation = Entity.Null;
				m_BoardingQueue.Enqueue(boardingItem);
			}

			public void EndTesting(Entity vehicle, Entity route, Entity stop, Entity waypoint)
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				BoardingItem boardingItem = default(BoardingItem);
				boardingItem.m_Begin = false;
				boardingItem.m_Refuel = false;
				boardingItem.m_Testing = true;
				boardingItem.m_Vehicle = vehicle;
				boardingItem.m_Route = route;
				boardingItem.m_Stop = stop;
				boardingItem.m_Waypoint = waypoint;
				boardingItem.m_CurrentStation = Entity.Null;
				boardingItem.m_NextStation = Entity.Null;
				m_BoardingQueue.Enqueue(boardingItem);
			}
		}

		private NativeQueue<BoardingItem> m_BoardingQueue;

		public BoardingData(Allocator allocator)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			m_BoardingQueue = new NativeQueue<BoardingItem>(AllocatorHandle.op_Implicit(allocator));
		}

		public void Dispose()
		{
			m_BoardingQueue.Dispose();
		}

		public void Dispose(JobHandle inputDeps)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			m_BoardingQueue.Dispose(inputDeps);
		}

		public JobHandle ScheduleBoarding(SystemBase system, CityStatisticsSystem statsSystem, AchievementTriggerSystem achievementTriggerSystem, BoardingLookupData lookupData, uint simulationFrameIndex, JobHandle inputDeps)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			IAchievement val2 = default(IAchievement);
			JobHandle deps;
			JobHandle val = IJobExtensions.Schedule<TransportBoardingJob>(new TransportBoardingJob
			{
				m_SimulationFrameIndex = simulationFrameIndex,
				m_ShouldTrackTransportedResource = (PlatformManager.instance.achievementsEnabled && PlatformManager.instance.GetAchievement(Game.Achievements.Achievements.ShipIt, ref val2) && !val2.achieved),
				m_BoardingLookupData = lookupData,
				m_BoardingQueue = m_BoardingQueue,
				m_StatisticsEventQueue = statsSystem.GetStatisticsEventQueue(out deps),
				m_TransportedResourceQueue = achievementTriggerSystem.GetTransportedResourceQueue()
			}, JobHandle.CombineDependencies(inputDeps, deps));
			statsSystem.AddWriter(val);
			achievementTriggerSystem.AddWriter(val);
			return val;
		}

		public Concurrent ToConcurrent()
		{
			return new Concurrent(this);
		}
	}

	private struct BoardingItem
	{
		public bool m_Begin;

		public bool m_Refuel;

		public bool m_Testing;

		public Entity m_Vehicle;

		public Entity m_Route;

		public Entity m_Stop;

		public Entity m_Waypoint;

		public Entity m_CurrentStation;

		public Entity m_NextStation;
	}

	[BurstCompile]
	private struct TransportBoardingJob : IJob
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct LoadingResourceComparer : IComparer<LoadingResources>
		{
			public int Compare(LoadingResources x, LoadingResources y)
			{
				return x.m_Amount - y.m_Amount;
			}
		}

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		[ReadOnly]
		public bool m_ShouldTrackTransportedResource;

		public BoardingLookupData m_BoardingLookupData;

		public NativeQueue<BoardingItem> m_BoardingQueue;

		public NativeQueue<StatisticsEvent> m_StatisticsEventQueue;

		public NativeQueue<TransportedResource> m_TransportedResourceQueue;

		public void Execute()
		{
			BoardingItem data = default(BoardingItem);
			while (m_BoardingQueue.TryDequeue(ref data))
			{
				if (data.m_Testing)
				{
					if (data.m_Begin)
					{
						BeginTesting(data);
					}
					else
					{
						EndTesting(data);
					}
				}
				else if (data.m_Begin)
				{
					BeginBoarding(data);
				}
				else
				{
					EndBoarding(data);
				}
			}
		}

		private void BeginTesting(BoardingItem data)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			BoardingVehicle boardingVehicle = m_BoardingLookupData.m_BoardingVehicleData[data.m_Stop];
			Game.Vehicles.CargoTransport cargoTransport = default(Game.Vehicles.CargoTransport);
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			if (!(boardingVehicle.m_Testing != Entity.Null) || !(boardingVehicle.m_Testing != data.m_Vehicle) || ((!m_BoardingLookupData.m_CargoTransportData.TryGetComponent(boardingVehicle.m_Testing, ref cargoTransport) || (cargoTransport.m_State & CargoTransportFlags.Testing) == 0) && (!m_BoardingLookupData.m_PublicTransportData.TryGetComponent(boardingVehicle.m_Testing, ref publicTransport) || (publicTransport.m_State & PublicTransportFlags.Testing) == 0)))
			{
				boardingVehicle.m_Testing = data.m_Vehicle;
				m_BoardingLookupData.m_BoardingVehicleData[data.m_Stop] = boardingVehicle;
				if (m_BoardingLookupData.m_CargoTransportData.HasComponent(data.m_Vehicle))
				{
					Game.Vehicles.CargoTransport cargoTransport2 = m_BoardingLookupData.m_CargoTransportData[data.m_Vehicle];
					cargoTransport2.m_State &= ~CargoTransportFlags.RequireStop;
					cargoTransport2.m_State |= CargoTransportFlags.Testing;
					m_BoardingLookupData.m_CargoTransportData[data.m_Vehicle] = cargoTransport2;
				}
				if (m_BoardingLookupData.m_PublicTransportData.HasComponent(data.m_Vehicle))
				{
					Game.Vehicles.PublicTransport publicTransport2 = m_BoardingLookupData.m_PublicTransportData[data.m_Vehicle];
					publicTransport2.m_State &= ~PublicTransportFlags.RequireStop;
					publicTransport2.m_State |= PublicTransportFlags.Testing;
					m_BoardingLookupData.m_PublicTransportData[data.m_Vehicle] = publicTransport2;
				}
			}
		}

		private void EndTesting(BoardingItem data)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			BoardingVehicle boardingVehicle = m_BoardingLookupData.m_BoardingVehicleData[data.m_Stop];
			if (boardingVehicle.m_Testing == data.m_Vehicle)
			{
				boardingVehicle.m_Testing = Entity.Null;
				m_BoardingLookupData.m_BoardingVehicleData[data.m_Stop] = boardingVehicle;
			}
			if (m_BoardingLookupData.m_CargoTransportData.HasComponent(data.m_Vehicle))
			{
				Game.Vehicles.CargoTransport cargoTransport = m_BoardingLookupData.m_CargoTransportData[data.m_Vehicle];
				cargoTransport.m_State &= ~CargoTransportFlags.Testing;
				m_BoardingLookupData.m_CargoTransportData[data.m_Vehicle] = cargoTransport;
			}
			if (m_BoardingLookupData.m_PublicTransportData.HasComponent(data.m_Vehicle))
			{
				Game.Vehicles.PublicTransport publicTransport = m_BoardingLookupData.m_PublicTransportData[data.m_Vehicle];
				publicTransport.m_State &= ~PublicTransportFlags.Testing;
				m_BoardingLookupData.m_PublicTransportData[data.m_Vehicle] = publicTransport;
			}
		}

		private void BeginBoarding(BoardingItem data)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			BoardingVehicle boardingVehicle = m_BoardingLookupData.m_BoardingVehicleData[data.m_Stop];
			Game.Vehicles.CargoTransport cargoTransport = default(Game.Vehicles.CargoTransport);
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			if (boardingVehicle.m_Vehicle != Entity.Null && boardingVehicle.m_Vehicle != data.m_Vehicle && ((m_BoardingLookupData.m_CargoTransportData.TryGetComponent(boardingVehicle.m_Vehicle, ref cargoTransport) && (cargoTransport.m_State & CargoTransportFlags.Boarding) != 0) || (m_BoardingLookupData.m_PublicTransportData.TryGetComponent(boardingVehicle.m_Vehicle, ref publicTransport) && (publicTransport.m_State & PublicTransportFlags.Boarding) != 0)))
			{
				return;
			}
			PrefabRef prefabRef = m_BoardingLookupData.m_PrefabRefData[data.m_Route];
			TransportLine transportLine = m_BoardingLookupData.m_TransportLineData[data.m_Route];
			VehicleTiming vehicleTiming = m_BoardingLookupData.m_VehicleTimingData[data.m_Waypoint];
			Connected connected = m_BoardingLookupData.m_ConnectedData[data.m_Waypoint];
			DynamicBuffer<RouteModifier> routeModifiers = m_BoardingLookupData.m_RouteModifiers[data.m_Route];
			Game.Vehicles.CargoTransport cargoTransport2 = default(Game.Vehicles.CargoTransport);
			Game.Vehicles.PublicTransport publicTransport2 = default(Game.Vehicles.PublicTransport);
			uint departureFrame = 0u;
			if (m_BoardingLookupData.m_CargoTransportData.HasComponent(data.m_Vehicle))
			{
				cargoTransport2 = m_BoardingLookupData.m_CargoTransportData[data.m_Vehicle];
				departureFrame = cargoTransport2.m_DepartureFrame;
			}
			if (m_BoardingLookupData.m_PublicTransportData.HasComponent(data.m_Vehicle))
			{
				publicTransport2 = m_BoardingLookupData.m_PublicTransportData[data.m_Vehicle];
				departureFrame = publicTransport2.m_DepartureFrame;
			}
			TransportLineData prefabLineData = m_BoardingLookupData.m_PrefabTransportLineData[prefabRef.m_Prefab];
			float targetStopTime = ((!m_BoardingLookupData.m_TransportStopData.HasComponent(connected.m_Connected)) ? prefabLineData.m_StopDuration : RouteUtils.GetStopDuration(prefabLineData, m_BoardingLookupData.m_TransportStopData[connected.m_Connected]));
			boardingVehicle.m_Vehicle = data.m_Vehicle;
			vehicleTiming.m_AverageTravelTime = RouteUtils.UpdateAverageTravelTime(vehicleTiming.m_AverageTravelTime, departureFrame, m_SimulationFrameIndex);
			departureFrame = (((cargoTransport2.m_State & CargoTransportFlags.EnRoute) == 0 && (publicTransport2.m_State & PublicTransportFlags.EnRoute) == 0) ? (m_SimulationFrameIndex + 60) : (vehicleTiming.m_LastDepartureFrame = RouteUtils.CalculateDepartureFrame(transportLine, prefabLineData, routeModifiers, targetStopTime, vehicleTiming.m_LastDepartureFrame, m_SimulationFrameIndex)));
			m_BoardingLookupData.m_BoardingVehicleData[data.m_Stop] = boardingVehicle;
			m_BoardingLookupData.m_VehicleTimingData[data.m_Waypoint] = vehicleTiming;
			if (m_BoardingLookupData.m_CargoTransportData.HasComponent(data.m_Vehicle))
			{
				cargoTransport2.m_State |= CargoTransportFlags.Boarding;
				if (data.m_Refuel)
				{
					cargoTransport2.m_State |= CargoTransportFlags.Refueling;
				}
				cargoTransport2.m_DepartureFrame = departureFrame;
				m_BoardingLookupData.m_CargoTransportData[data.m_Vehicle] = cargoTransport2;
			}
			if (m_BoardingLookupData.m_PublicTransportData.HasComponent(data.m_Vehicle))
			{
				publicTransport2.m_State |= PublicTransportFlags.Boarding;
				if (data.m_Refuel)
				{
					publicTransport2.m_State |= PublicTransportFlags.Refueling;
				}
				publicTransport2.m_DepartureFrame = departureFrame;
				publicTransport2.m_MaxBoardingDistance = 0f;
				publicTransport2.m_MinWaitingDistance = float.MaxValue;
				m_BoardingLookupData.m_PublicTransportData[data.m_Vehicle] = publicTransport2;
			}
			DynamicBuffer<LoadingResources> loadingResources = default(DynamicBuffer<LoadingResources>);
			if (m_BoardingLookupData.m_LoadingResources.HasBuffer(data.m_Vehicle))
			{
				loadingResources = m_BoardingLookupData.m_LoadingResources[data.m_Vehicle];
				loadingResources.Clear();
			}
			int workAmount = 0;
			UnloadResources(data.m_Vehicle, data.m_CurrentStation, ref workAmount);
			LoadResources(data.m_Vehicle, data.m_CurrentStation, data.m_NextStation, loadingResources, ref workAmount);
			AddWork(data.m_Stop, workAmount);
		}

		private void AddWork(Entity target, int workAmount)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			if (workAmount <= 0)
			{
				return;
			}
			Owner owner = default(Owner);
			Game.Buildings.CargoTransportStation cargoTransportStation = default(Game.Buildings.CargoTransportStation);
			while (m_BoardingLookupData.m_OwnerData.TryGetComponent(target, ref owner))
			{
				target = owner.m_Owner;
				if (m_BoardingLookupData.m_CargoTransportStationData.TryGetComponent(target, ref cargoTransportStation))
				{
					cargoTransportStation.m_WorkAmount += workAmount;
					m_BoardingLookupData.m_CargoTransportStationData[target] = cargoTransportStation;
					break;
				}
			}
		}

		private void EndBoarding(BoardingItem data)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			BoardingVehicle boardingVehicle = m_BoardingLookupData.m_BoardingVehicleData[data.m_Stop];
			if (boardingVehicle.m_Vehicle == data.m_Vehicle)
			{
				boardingVehicle.m_Vehicle = Entity.Null;
				m_BoardingLookupData.m_BoardingVehicleData[data.m_Stop] = boardingVehicle;
				int workAmount = 0;
				LoadResources(data.m_Vehicle, data.m_CurrentStation, data.m_NextStation, default(DynamicBuffer<LoadingResources>), ref workAmount);
				AddWork(data.m_Stop, workAmount);
			}
			if (m_BoardingLookupData.m_CargoTransportData.HasComponent(data.m_Vehicle))
			{
				Game.Vehicles.CargoTransport cargoTransport = m_BoardingLookupData.m_CargoTransportData[data.m_Vehicle];
				cargoTransport.m_State &= ~(CargoTransportFlags.Boarding | CargoTransportFlags.Refueling);
				m_BoardingLookupData.m_CargoTransportData[data.m_Vehicle] = cargoTransport;
			}
			if (m_BoardingLookupData.m_PublicTransportData.HasComponent(data.m_Vehicle))
			{
				Game.Vehicles.PublicTransport publicTransport = m_BoardingLookupData.m_PublicTransportData[data.m_Vehicle];
				publicTransport.m_State &= ~(PublicTransportFlags.Boarding | PublicTransportFlags.Refueling);
				m_BoardingLookupData.m_PublicTransportData[data.m_Vehicle] = publicTransport;
			}
		}

		private void UnloadResources(Entity vehicle, Entity target, ref int workAmount)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			if (!m_BoardingLookupData.m_EconomyResources.HasBuffer(target))
			{
				return;
			}
			DynamicBuffer<Resources> targetResources = m_BoardingLookupData.m_EconomyResources[target];
			if (m_BoardingLookupData.m_LayoutElements.HasBuffer(vehicle))
			{
				DynamicBuffer<LayoutElement> val = m_BoardingLookupData.m_LayoutElements[vehicle];
				for (int i = 0; i < val.Length; i++)
				{
					Entity vehicle2 = val[i].m_Vehicle;
					if (m_BoardingLookupData.m_EconomyResources.HasBuffer(vehicle2))
					{
						DynamicBuffer<Resources> sourceResources = m_BoardingLookupData.m_EconomyResources[vehicle2];
						UnloadResources(sourceResources, targetResources, target, ref workAmount);
					}
				}
			}
			else if (m_BoardingLookupData.m_EconomyResources.HasBuffer(vehicle))
			{
				DynamicBuffer<Resources> sourceResources2 = m_BoardingLookupData.m_EconomyResources[vehicle];
				UnloadResources(sourceResources2, targetResources, target, ref workAmount);
			}
		}

		private void UnloadResources(DynamicBuffer<Resources> sourceResources, DynamicBuffer<Resources> targetResources, Entity target, ref int workAmount)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < sourceResources.Length; i++)
			{
				Resources resources = sourceResources[i];
				EconomyUtils.AddResources(resources.m_Resource, resources.m_Amount, targetResources);
				workAmount += resources.m_Amount;
				if (m_ShouldTrackTransportedResource && resources.m_Amount != 0)
				{
					m_TransportedResourceQueue.Enqueue(new TransportedResource
					{
						m_Amount = resources.m_Amount,
						m_CargoTransport = target
					});
				}
			}
			sourceResources.Clear();
		}

		private void LoadResources(Entity vehicle, Entity source, Entity target, DynamicBuffer<LoadingResources> loadingResources, ref int workAmount)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			if (m_BoardingLookupData.m_EconomyResources.HasBuffer(source) && m_BoardingLookupData.m_StorageTransferRequests.HasBuffer(source))
			{
				DynamicBuffer<Resources> resources = m_BoardingLookupData.m_EconomyResources[source];
				DynamicBuffer<StorageTransferRequest> val = m_BoardingLookupData.m_StorageTransferRequests[source];
				int num = 0;
				while (num < val.Length)
				{
					StorageTransferRequest storageTransferRequest = val[num];
					if ((storageTransferRequest.m_Flags & StorageTransferFlags.Incoming) != 0 || (storageTransferRequest.m_Flags & StorageTransferFlags.Transport) == 0 || storageTransferRequest.m_Target != target)
					{
						num++;
						continue;
					}
					int requestNewAmount = storageTransferRequest.m_Amount;
					if (storageTransferRequest.m_Amount > 0)
					{
						int resources2 = EconomyUtils.GetResources(storageTransferRequest.m_Resource, resources);
						LoadResources(storageTransferRequest.m_Resource, ref requestNewAmount, vehicle, resources2, source);
						if (requestNewAmount < storageTransferRequest.m_Amount)
						{
							EconomyUtils.AddResources(storageTransferRequest.m_Resource, requestNewAmount - storageTransferRequest.m_Amount, resources);
							workAmount += storageTransferRequest.m_Amount - requestNewAmount;
						}
					}
					if (requestNewAmount == 0)
					{
						val.RemoveAt(num);
						continue;
					}
					storageTransferRequest.m_Amount = requestNewAmount;
					val[num++] = storageTransferRequest;
					if (!loadingResources.IsCreated)
					{
						continue;
					}
					for (int i = 0; i < loadingResources.Length; i++)
					{
						LoadingResources loadingResources2 = loadingResources[i];
						if (loadingResources2.m_Resource == storageTransferRequest.m_Resource)
						{
							loadingResources2.m_Amount += storageTransferRequest.m_Amount;
							loadingResources[i] = loadingResources2;
							storageTransferRequest.m_Amount = 0;
							break;
						}
					}
					if (storageTransferRequest.m_Amount != 0)
					{
						loadingResources.Add(new LoadingResources
						{
							m_Resource = storageTransferRequest.m_Resource,
							m_Amount = storageTransferRequest.m_Amount
						});
					}
				}
			}
			if (loadingResources.IsCreated && loadingResources.Length >= 2)
			{
				NativeSortExtension.Sort<LoadingResources, LoadingResourceComparer>(loadingResources.AsNativeArray(), default(LoadingResourceComparer));
			}
		}

		private void LoadResources(Resource resource, ref int requestNewAmount, Entity vehicle, int sourceStoredAmount, Entity source)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			if (m_BoardingLookupData.m_LayoutElements.HasBuffer(vehicle))
			{
				DynamicBuffer<LayoutElement> val = m_BoardingLookupData.m_LayoutElements[vehicle];
				for (int i = 0; i < val.Length; i++)
				{
					Entity vehicle2 = val[i].m_Vehicle;
					if (m_BoardingLookupData.m_EconomyResources.HasBuffer(vehicle2))
					{
						PrefabRef prefabRef = m_BoardingLookupData.m_PrefabRefData[vehicle2];
						CargoTransportVehicleData vehicleData = m_BoardingLookupData.m_PrefabCargoTransportVehicleData[prefabRef.m_Prefab];
						DynamicBuffer<Resources> targetResources = m_BoardingLookupData.m_EconomyResources[vehicle2];
						TransportType vehicleType = GetVehicleType(vehicle2);
						LoadResources(resource, ref requestNewAmount, vehicleData, targetResources, vehicleType, ref sourceStoredAmount, source);
						if (requestNewAmount == 0 || sourceStoredAmount <= 0)
						{
							break;
						}
					}
				}
			}
			else if (m_BoardingLookupData.m_EconomyResources.HasBuffer(vehicle))
			{
				PrefabRef prefabRef2 = m_BoardingLookupData.m_PrefabRefData[vehicle];
				CargoTransportVehicleData vehicleData2 = m_BoardingLookupData.m_PrefabCargoTransportVehicleData[prefabRef2.m_Prefab];
				DynamicBuffer<Resources> targetResources2 = m_BoardingLookupData.m_EconomyResources[vehicle];
				TransportType vehicleType2 = GetVehicleType(vehicle);
				LoadResources(resource, ref requestNewAmount, vehicleData2, targetResources2, vehicleType2, ref sourceStoredAmount, source);
			}
		}

		private void LoadResources(Resource resource, ref int requestNewAmount, CargoTransportVehicleData vehicleData, DynamicBuffer<Resources> targetResources, TransportType transportType, ref int sourceStoredAmount, Entity source)
		{
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			int num = vehicleData.m_CargoCapacity;
			int num2 = -1;
			for (int i = 0; i < targetResources.Length; i++)
			{
				Resources resources = targetResources[i];
				num -= resources.m_Amount;
				num2 = math.select(num2, i, resources.m_Resource == resource);
			}
			int num3 = math.min(num, math.min(requestNewAmount, math.max(sourceStoredAmount, 0)));
			if (num3 == 0)
			{
				return;
			}
			if (num2 >= 0)
			{
				Resources resources2 = targetResources[num2];
				resources2.m_Amount += num3;
				targetResources[num2] = resources2;
			}
			else
			{
				if (targetResources.Length >= vehicleData.m_MaxResourceCount || (vehicleData.m_Resources & resource) == Resource.NoResource)
				{
					return;
				}
				targetResources.Add(new Resources
				{
					m_Resource = resource,
					m_Amount = num3
				});
			}
			requestNewAmount -= num3;
			sourceStoredAmount -= num3;
			if (m_ShouldTrackTransportedResource && num3 != 0)
			{
				m_TransportedResourceQueue.Enqueue(new TransportedResource
				{
					m_Amount = num3,
					m_CargoTransport = source
				});
			}
			switch (transportType)
			{
			case TransportType.Train:
				m_StatisticsEventQueue.Enqueue(new StatisticsEvent
				{
					m_Statistic = StatisticType.CargoCountTrain,
					m_Change = num3
				});
				break;
			case TransportType.Ship:
				m_StatisticsEventQueue.Enqueue(new StatisticsEvent
				{
					m_Statistic = StatisticType.CargoCountShip,
					m_Change = num3
				});
				break;
			case TransportType.Airplane:
				m_StatisticsEventQueue.Enqueue(new StatisticsEvent
				{
					m_Statistic = StatisticType.CargoCountAirplane,
					m_Change = num3
				});
				break;
			}
		}

		private TransportType GetVehicleType(Entity vehicle)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			if (m_BoardingLookupData.m_AircraftData.HasComponent(vehicle))
			{
				return TransportType.Airplane;
			}
			if (m_BoardingLookupData.m_TrainData.HasComponent(vehicle))
			{
				return TransportType.Train;
			}
			if (m_BoardingLookupData.m_WatercraftData.HasComponent(vehicle))
			{
				return TransportType.Ship;
			}
			if (m_BoardingLookupData.m_DeliveryTruckData.HasComponent(vehicle))
			{
				return TransportType.Bus;
			}
			return TransportType.None;
		}
	}
}

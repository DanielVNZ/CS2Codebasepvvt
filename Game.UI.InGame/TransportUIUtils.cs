using Colossal.Entities;
using Game.Economy;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;

namespace Game.UI.InGame;

public static class TransportUIUtils
{
	public static int CountLines(NativeArray<UITransportLineData> lines, TransportType type, bool cargo = false)
	{
		int num = 0;
		for (int i = 0; i < lines.Length; i++)
		{
			if (lines[i].type == type && lines[i].isCargo == cargo)
			{
				num++;
			}
		}
		return num;
	}

	public static NativeArray<UITransportLineData> GetSortedLines(EntityQuery query, EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref query)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		int length = val.Length;
		NativeArray<UITransportLineData> val2 = default(NativeArray<UITransportLineData>);
		val2._002Ector(length, (Allocator)2, (NativeArrayOptions)1);
		for (int i = 0; i < length; i++)
		{
			val2[i] = BuildTransportLine(val[i], entityManager, prefabSystem);
		}
		NativeSortExtension.Sort<UITransportLineData>(val2);
		val.Dispose();
		return val2;
	}

	public static UITransportLineData BuildTransportLine(Entity entity, EntityManager entityManager, PrefabSystem m_PrefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		Route componentData = ((EntityManager)(ref entityManager)).GetComponentData<Route>(entity);
		PrefabRef componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(entity);
		TransportLinePrefab prefab = m_PrefabSystem.GetPrefab<TransportLinePrefab>(componentData2.m_Prefab);
		TransportLineData componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<TransportLineData>(componentData2.m_Prefab);
		bool visible = !((EntityManager)(ref entityManager)).HasComponent<HiddenRoute>(entity);
		Color componentData4 = ((EntityManager)(ref entityManager)).GetComponentData<Color>(entity);
		int cargo = 0;
		int capacity = 0;
		int stopCount = GetStopCount(entityManager, entity);
		int routeVehiclesCount = GetRouteVehiclesCount(entityManager, entity, ref cargo, ref capacity);
		float routeLength = GetRouteLength(entityManager, entity);
		float usage = ((capacity > 0) ? ((float)cargo / (float)capacity) : 0f);
		RouteSchedule schedule = ((!RouteUtils.CheckOption(componentData, RouteOption.Day)) ? (RouteUtils.CheckOption(componentData, RouteOption.Night) ? RouteSchedule.Night : RouteSchedule.DayAndNight) : RouteSchedule.Day);
		bool active = !RouteUtils.CheckOption(componentData, RouteOption.Inactive);
		return new UITransportLineData(entity, active, visible, prefab.m_CargoTransport, componentData4, schedule, componentData3.m_TransportType, routeLength, stopCount, routeVehiclesCount, cargo, usage);
	}

	public static int GetStopCount(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<RouteWaypoint> buffer = ((EntityManager)(ref entityManager)).GetBuffer<RouteWaypoint>(entity, true);
		int num = 0;
		Connected connected = default(Connected);
		for (int i = 0; i < buffer.Length; i++)
		{
			if (EntitiesExtensions.TryGetComponent<Connected>(entityManager, buffer[i].m_Waypoint, ref connected) && ((EntityManager)(ref entityManager)).HasComponent<Game.Routes.TransportStop>(connected.m_Connected) && !((EntityManager)(ref entityManager)).HasComponent<TaxiStand>(connected.m_Connected))
			{
				num++;
			}
		}
		return num;
	}

	public static float GetRouteLength(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<RouteSegment> buffer = ((EntityManager)(ref entityManager)).GetBuffer<RouteSegment>(entity, true);
		float num = 0f;
		PathInformation pathInformation = default(PathInformation);
		for (int i = 0; i < buffer.Length; i++)
		{
			if (EntitiesExtensions.TryGetComponent<PathInformation>(entityManager, buffer[i].m_Segment, ref pathInformation))
			{
				num += pathInformation.m_Distance;
			}
		}
		return num;
	}

	public static int GetRouteVehiclesCount(EntityManager entityManager, Entity entity, ref int cargo, ref int capacity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<RouteVehicle> buffer = ((EntityManager)(ref entityManager)).GetBuffer<RouteVehicle>(entity, true);
		for (int i = 0; i < buffer.Length; i++)
		{
			AddCargo(entityManager, buffer[i].m_Vehicle, ref cargo, ref capacity);
		}
		return buffer.Length;
	}

	private static void AddCargo(EntityManager entityManager, Entity entity, ref int cargo, ref int capacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
		if (EntitiesExtensions.TryGetBuffer<LayoutElement>(entityManager, entity, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				AddVehicleCargo(entityManager, val[i].m_Vehicle, ref cargo, ref capacity);
			}
		}
		else
		{
			AddVehicleCargo(entityManager, entity, ref cargo, ref capacity);
		}
	}

	private static void AddVehicleCargo(EntityManager entityManager, Entity entity, ref int cargo, ref int capacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef prefabRef = default(PrefabRef);
		if (!EntitiesExtensions.TryGetComponent<PrefabRef>(entityManager, entity, ref prefabRef))
		{
			return;
		}
		PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
		if (EntitiesExtensions.TryGetComponent<PublicTransportVehicleData>(entityManager, prefabRef.m_Prefab, ref publicTransportVehicleData))
		{
			DynamicBuffer<Passenger> val = default(DynamicBuffer<Passenger>);
			if (EntitiesExtensions.TryGetBuffer<Passenger>(entityManager, entity, true, ref val))
			{
				cargo += val.Length;
			}
			capacity += publicTransportVehicleData.m_PassengerCapacity;
		}
		else
		{
			CargoTransportVehicleData cargoTransportVehicleData = default(CargoTransportVehicleData);
			if (!EntitiesExtensions.TryGetComponent<CargoTransportVehicleData>(entityManager, prefabRef.m_Prefab, ref cargoTransportVehicleData))
			{
				return;
			}
			DynamicBuffer<Resources> val2 = default(DynamicBuffer<Resources>);
			if (EntitiesExtensions.TryGetBuffer<Resources>(entityManager, entity, true, ref val2))
			{
				for (int i = 0; i < val2.Length; i++)
				{
					cargo += val2[i].m_Amount;
				}
			}
			capacity += cargoTransportVehicleData.m_CargoCapacity;
		}
	}
}

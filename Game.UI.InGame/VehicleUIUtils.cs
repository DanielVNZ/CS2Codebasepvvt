using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Events;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.UI.InGame;

public static class VehicleUIUtils
{
	public readonly struct EntityWrapper
	{
		public Entity entity { get; }

		public EntityWrapper(Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			this.entity = entity;
		}

		public void Write(IJsonWriter writer, NameSystem nameSystem)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("entity");
			if (entity == Entity.Null)
			{
				writer.WriteNull();
			}
			else
			{
				UnityWriters.Write(writer, entity);
			}
			writer.PropertyName("name");
			if (entity == Entity.Null)
			{
				writer.WriteNull();
			}
			else
			{
				nameSystem.BindName(writer, entity);
			}
			writer.TypeEnd();
		}
	}

	public static int GetAvailableVehicles(Entity vehicleOwnerEntity, EntityManager entityManager)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		float efficiency = 1f;
		DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
		if (EntitiesExtensions.TryGetBuffer<Efficiency>(entityManager, vehicleOwnerEntity, true, ref buffer))
		{
			efficiency = Mathf.Min(BuildingUtils.GetEfficiency(buffer), 1f);
		}
		int num = 0;
		PrefabRef prefabRef = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(entityManager, vehicleOwnerEntity, ref prefabRef))
		{
			DeathcareFacilityData data2;
			EmergencyShelterData data3;
			FireStationData data4;
			HospitalData data5;
			MaintenanceDepotData data6;
			PoliceStationData data7;
			PrisonData data8;
			TransportDepotData data9;
			PostFacilityData data10;
			TransportCompanyData transportCompanyData = default(TransportCompanyData);
			if (UpgradeUtils.TryGetCombinedComponent<GarbageFacilityData>(entityManager, vehicleOwnerEntity, prefabRef.m_Prefab, out GarbageFacilityData data))
			{
				num += BuildingUtils.GetVehicleCapacity(efficiency, data.m_VehicleCapacity);
			}
			else if (UpgradeUtils.TryGetCombinedComponent<DeathcareFacilityData>(entityManager, vehicleOwnerEntity, prefabRef.m_Prefab, out data2))
			{
				num += BuildingUtils.GetVehicleCapacity(efficiency, data2.m_HearseCapacity);
			}
			else if (UpgradeUtils.TryGetCombinedComponent<EmergencyShelterData>(entityManager, vehicleOwnerEntity, prefabRef.m_Prefab, out data3))
			{
				num += BuildingUtils.GetVehicleCapacity(efficiency, data3.m_VehicleCapacity);
			}
			else if (UpgradeUtils.TryGetCombinedComponent<FireStationData>(entityManager, vehicleOwnerEntity, prefabRef.m_Prefab, out data4))
			{
				num += BuildingUtils.GetVehicleCapacity(efficiency, data4.m_FireEngineCapacity);
				num += BuildingUtils.GetVehicleCapacity(efficiency, data4.m_FireHelicopterCapacity);
			}
			else if (UpgradeUtils.TryGetCombinedComponent<HospitalData>(entityManager, vehicleOwnerEntity, prefabRef.m_Prefab, out data5))
			{
				num += BuildingUtils.GetVehicleCapacity(efficiency, data5.m_AmbulanceCapacity);
				num += BuildingUtils.GetVehicleCapacity(efficiency, data5.m_MedicalHelicopterCapacity);
			}
			else if (UpgradeUtils.TryGetCombinedComponent<MaintenanceDepotData>(entityManager, vehicleOwnerEntity, prefabRef.m_Prefab, out data6))
			{
				num += BuildingUtils.GetVehicleCapacity(efficiency, data6.m_VehicleCapacity);
			}
			else if (UpgradeUtils.TryGetCombinedComponent<PoliceStationData>(entityManager, vehicleOwnerEntity, prefabRef.m_Prefab, out data7))
			{
				num += BuildingUtils.GetVehicleCapacity(efficiency, data7.m_PatrolCarCapacity);
				num += BuildingUtils.GetVehicleCapacity(efficiency, data7.m_PoliceHelicopterCapacity);
			}
			else if (UpgradeUtils.TryGetCombinedComponent<PrisonData>(entityManager, vehicleOwnerEntity, prefabRef.m_Prefab, out data8))
			{
				num += BuildingUtils.GetVehicleCapacity(efficiency, data8.m_PrisonVanCapacity);
			}
			else if (UpgradeUtils.TryGetCombinedComponent<TransportDepotData>(entityManager, vehicleOwnerEntity, prefabRef.m_Prefab, out data9))
			{
				num += BuildingUtils.GetVehicleCapacity(efficiency, data9.m_VehicleCapacity);
			}
			else if (UpgradeUtils.TryGetCombinedComponent<PostFacilityData>(entityManager, vehicleOwnerEntity, prefabRef.m_Prefab, out data10))
			{
				num += BuildingUtils.GetVehicleCapacity(efficiency, data10.m_PostVanCapacity);
				num += BuildingUtils.GetVehicleCapacity(efficiency, data10.m_PostTruckCapacity);
			}
			else if (EntitiesExtensions.TryGetComponent<TransportCompanyData>(entityManager, prefabRef.m_Prefab, ref transportCompanyData))
			{
				num += BuildingUtils.GetVehicleCapacity(efficiency, transportCompanyData.m_MaxTransports);
			}
		}
		return num;
	}

	public static Entity GetDestination(EntityManager entityManager, Entity vehicleEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		Entity val = Entity.Null;
		Target target = default(Target);
		if (EntitiesExtensions.TryGetComponent<Target>(entityManager, vehicleEntity, ref target))
		{
			val = target.m_Target;
			Connected connected = default(Connected);
			if (EntitiesExtensions.TryGetComponent<Connected>(entityManager, val, ref connected))
			{
				val = connected.m_Connected;
			}
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(val))
			{
				return val;
			}
			if (((EntityManager)(ref entityManager)).HasComponent<Vehicle>(val))
			{
				return val;
			}
			PropertyRenter propertyRenter = default(PropertyRenter);
			if (((EntityManager)(ref entityManager)).HasComponent<CompanyData>(val) && EntitiesExtensions.TryGetComponent<PropertyRenter>(entityManager, val, ref propertyRenter))
			{
				return propertyRenter.m_Property;
			}
			Owner owner = default(Owner);
			if (EntitiesExtensions.TryGetComponent<Owner>(entityManager, val, ref owner))
			{
				val = owner.m_Owner;
			}
			Game.Creatures.Resident resident = default(Game.Creatures.Resident);
			if (EntitiesExtensions.TryGetComponent<Game.Creatures.Resident>(entityManager, val, ref resident))
			{
				val = resident.m_Citizen;
			}
			Waypoint waypoint = default(Waypoint);
			DynamicBuffer<RouteWaypoint> val2 = default(DynamicBuffer<RouteWaypoint>);
			if (!((EntityManager)(ref entityManager)).HasComponent<Connected>(target.m_Target) && EntitiesExtensions.TryGetComponent<Waypoint>(entityManager, target.m_Target, ref waypoint) && EntitiesExtensions.TryGetBuffer<RouteWaypoint>(entityManager, val, true, ref val2))
			{
				int num = waypoint.m_Index + 1;
				for (int i = 0; i < val2.Length; i++)
				{
					num += i;
					num = math.select(num, 0, num >= val2.Length);
					if (EntitiesExtensions.TryGetComponent<Connected>(entityManager, val2[num].m_Waypoint, ref connected))
					{
						val = connected.m_Connected;
						break;
					}
				}
			}
		}
		return val;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, EntityManager entityManager)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		Game.Vehicles.PublicTransport publicTransportVehicle = default(Game.Vehicles.PublicTransport);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.PublicTransport>(entityManager, entity, ref publicTransportVehicle))
		{
			return GetStateKey(entity, publicTransportVehicle, entityManager);
		}
		Game.Vehicles.PersonalCar personalCar = default(Game.Vehicles.PersonalCar);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.PersonalCar>(entityManager, entity, ref personalCar))
		{
			return GetStateKey(entity, personalCar, entityManager);
		}
		Game.Vehicles.PostVan postVan = default(Game.Vehicles.PostVan);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.PostVan>(entityManager, entity, ref postVan))
		{
			return GetStateKey(entity, postVan, entityManager);
		}
		Game.Vehicles.PoliceCar policeCar = default(Game.Vehicles.PoliceCar);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.PoliceCar>(entityManager, entity, ref policeCar))
		{
			DynamicBuffer<ServiceDispatch> dispatches = default(DynamicBuffer<ServiceDispatch>);
			EntitiesExtensions.TryGetBuffer<ServiceDispatch>(entityManager, entity, true, ref dispatches);
			return GetStateKey(entity, policeCar, dispatches, entityManager);
		}
		Game.Vehicles.MaintenanceVehicle maintenanceVehicle = default(Game.Vehicles.MaintenanceVehicle);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.MaintenanceVehicle>(entityManager, entity, ref maintenanceVehicle))
		{
			return GetStateKey(entity, maintenanceVehicle, entityManager);
		}
		Game.Vehicles.Ambulance ambulance = default(Game.Vehicles.Ambulance);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.Ambulance>(entityManager, entity, ref ambulance))
		{
			return GetStateKey(entity, ambulance, entityManager);
		}
		Game.Vehicles.GarbageTruck garbageTruck = default(Game.Vehicles.GarbageTruck);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.GarbageTruck>(entityManager, entity, ref garbageTruck))
		{
			return GetStateKey(entity, garbageTruck, entityManager);
		}
		Game.Vehicles.FireEngine fireEngine = default(Game.Vehicles.FireEngine);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.FireEngine>(entityManager, entity, ref fireEngine))
		{
			DynamicBuffer<ServiceDispatch> dispatches2 = default(DynamicBuffer<ServiceDispatch>);
			EntitiesExtensions.TryGetBuffer<ServiceDispatch>(entityManager, entity, true, ref dispatches2);
			return GetStateKey(entity, fireEngine, dispatches2, entityManager);
		}
		Game.Vehicles.DeliveryTruck truck = default(Game.Vehicles.DeliveryTruck);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.DeliveryTruck>(entityManager, entity, ref truck))
		{
			return GetStateKey(entity, truck, entityManager);
		}
		Game.Vehicles.Hearse hearse = default(Game.Vehicles.Hearse);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.Hearse>(entityManager, entity, ref hearse))
		{
			return GetStateKey(entity, hearse, entityManager);
		}
		Game.Vehicles.CargoTransport cargoTransport = default(Game.Vehicles.CargoTransport);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.CargoTransport>(entityManager, entity, ref cargoTransport))
		{
			return GetStateKey(entity, cargoTransport, entityManager);
		}
		Game.Vehicles.Taxi taxi = default(Game.Vehicles.Taxi);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.Taxi>(entityManager, entity, ref taxi))
		{
			return GetStateKey(entity, taxi, entityManager);
		}
		return VehicleStateLocaleKey.Unknown;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, Game.Vehicles.PublicTransport publicTransportVehicle, EntityManager entityManager)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity))
		{
			return VehicleStateLocaleKey.InvolvedInAccident;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity) || ((EntityManager)(ref entityManager)).HasComponent<ParkedTrain>(entity))
		{
			return VehicleStateLocaleKey.Parked;
		}
		if ((publicTransportVehicle.m_State & PublicTransportFlags.Returning) != 0)
		{
			return VehicleStateLocaleKey.Returning;
		}
		if ((publicTransportVehicle.m_State & PublicTransportFlags.Boarding) != 0)
		{
			return VehicleStateLocaleKey.Boarding;
		}
		if ((publicTransportVehicle.m_State & PublicTransportFlags.Evacuating) == 0)
		{
			return VehicleStateLocaleKey.EnRoute;
		}
		return VehicleStateLocaleKey.Evacuating;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, Game.Vehicles.PersonalCar personalCar, EntityManager entityManager)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity))
		{
			return VehicleStateLocaleKey.InvolvedInAccident;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity))
		{
			return VehicleStateLocaleKey.Parked;
		}
		if ((personalCar.m_State & PersonalCarFlags.Boarding) != 0)
		{
			return VehicleStateLocaleKey.Boarding;
		}
		if ((personalCar.m_State & PersonalCarFlags.Disembarking) != 0)
		{
			return VehicleStateLocaleKey.Disembarking;
		}
		if ((personalCar.m_State & PersonalCarFlags.Transporting) != 0)
		{
			return VehicleStateLocaleKey.Transporting;
		}
		return VehicleStateLocaleKey.EnRoute;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, Game.Vehicles.PostVan postVan, EntityManager entityManager)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity))
		{
			return VehicleStateLocaleKey.InvolvedInAccident;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity))
		{
			return VehicleStateLocaleKey.Parked;
		}
		if ((postVan.m_State & PostVanFlags.Delivering) != 0)
		{
			return VehicleStateLocaleKey.Delivering;
		}
		if ((postVan.m_State & PostVanFlags.Collecting) != 0)
		{
			return VehicleStateLocaleKey.Collecting;
		}
		if ((postVan.m_State & PostVanFlags.Returning) == 0)
		{
			return VehicleStateLocaleKey.Unknown;
		}
		return VehicleStateLocaleKey.Returning;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, Game.Vehicles.PoliceCar policeCar, DynamicBuffer<ServiceDispatch> dispatches, EntityManager entityManager)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity))
		{
			return VehicleStateLocaleKey.InvolvedInAccident;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity))
		{
			return VehicleStateLocaleKey.Parked;
		}
		if ((policeCar.m_State & PoliceCarFlags.Returning) != 0)
		{
			return VehicleStateLocaleKey.Returning;
		}
		if ((policeCar.m_State & PoliceCarFlags.AccidentTarget) != 0 && policeCar.m_RequestCount > 0 && dispatches.IsCreated && dispatches.Length > 0)
		{
			if ((policeCar.m_State & PoliceCarFlags.AtTarget) != 0)
			{
				PoliceEmergencyRequest policeEmergencyRequest = default(PoliceEmergencyRequest);
				AccidentSite accidentSite = default(AccidentSite);
				if (EntitiesExtensions.TryGetComponent<PoliceEmergencyRequest>(entityManager, dispatches[0].m_Request, ref policeEmergencyRequest) && EntitiesExtensions.TryGetComponent<AccidentSite>(entityManager, policeEmergencyRequest.m_Site, ref accidentSite))
				{
					if ((accidentSite.m_Flags & AccidentSiteFlags.TrafficAccident) != 0)
					{
						return VehicleStateLocaleKey.AccidentSite;
					}
					if ((accidentSite.m_Flags & AccidentSiteFlags.CrimeScene) != 0)
					{
						return VehicleStateLocaleKey.CrimeScene;
					}
				}
			}
			else if (((EntityManager)(ref entityManager)).HasComponent<PoliceEmergencyRequest>(dispatches[0].m_Request))
			{
				return VehicleStateLocaleKey.Dispatched;
			}
			return VehicleStateLocaleKey.Unknown;
		}
		return VehicleStateLocaleKey.Patrolling;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, Game.Vehicles.MaintenanceVehicle maintenanceVehicle, EntityManager entityManager)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity))
		{
			return VehicleStateLocaleKey.InvolvedInAccident;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity))
		{
			return VehicleStateLocaleKey.Parked;
		}
		if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.TransformTarget) != 0)
		{
			Target target = default(Target);
			if (EntitiesExtensions.TryGetComponent<Target>(entityManager, entity, ref target) && ((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(target.m_Target))
			{
				return VehicleStateLocaleKey.AccidentSite;
			}
			return VehicleStateLocaleKey.Dispatched;
		}
		if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.Returning) == 0)
		{
			return VehicleStateLocaleKey.Working;
		}
		return VehicleStateLocaleKey.Returning;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, Game.Vehicles.Ambulance ambulance, EntityManager entityManager)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity))
		{
			return VehicleStateLocaleKey.InvolvedInAccident;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity))
		{
			return VehicleStateLocaleKey.Parked;
		}
		if ((ambulance.m_State & AmbulanceFlags.Returning) != 0)
		{
			return VehicleStateLocaleKey.Returning;
		}
		if ((ambulance.m_State & AmbulanceFlags.Transporting) == 0)
		{
			return VehicleStateLocaleKey.Dispatched;
		}
		return VehicleStateLocaleKey.Transporting;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, Game.Vehicles.GarbageTruck garbageTruck, EntityManager entityManager)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity))
		{
			return VehicleStateLocaleKey.InvolvedInAccident;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity))
		{
			return VehicleStateLocaleKey.Parked;
		}
		if ((garbageTruck.m_State & GarbageTruckFlags.Returning) == 0)
		{
			return VehicleStateLocaleKey.Collecting;
		}
		return VehicleStateLocaleKey.Returning;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, Game.Vehicles.FireEngine fireEngine, DynamicBuffer<ServiceDispatch> dispatches, EntityManager entityManager)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity))
		{
			return VehicleStateLocaleKey.InvolvedInAccident;
		}
		if ((fireEngine.m_State & FireEngineFlags.Extinguishing) != 0)
		{
			return VehicleStateLocaleKey.Extinguishing;
		}
		if ((fireEngine.m_State & FireEngineFlags.Rescueing) != 0)
		{
			return VehicleStateLocaleKey.Rescuing;
		}
		if (fireEngine.m_RequestCount > 0 && dispatches.Length > 0)
		{
			return VehicleStateLocaleKey.Dispatched;
		}
		if (!((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity))
		{
			return VehicleStateLocaleKey.Returning;
		}
		return VehicleStateLocaleKey.Parked;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, Game.Vehicles.DeliveryTruck truck, EntityManager entityManager)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity))
		{
			return VehicleStateLocaleKey.InvolvedInAccident;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity))
		{
			return VehicleStateLocaleKey.Parked;
		}
		if ((truck.m_State & DeliveryTruckFlags.Returning) != 0)
		{
			return VehicleStateLocaleKey.Returning;
		}
		if ((truck.m_State & DeliveryTruckFlags.Buying) != 0)
		{
			return VehicleStateLocaleKey.Buying;
		}
		if ((truck.m_State & DeliveryTruckFlags.StorageTransfer) != 0)
		{
			Owner owner = default(Owner);
			if (EntitiesExtensions.TryGetComponent<Owner>(entityManager, entity, ref owner) && ((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(owner.m_Owner))
			{
				return VehicleStateLocaleKey.Importing;
			}
			Target target = default(Target);
			if (EntitiesExtensions.TryGetComponent<Target>(entityManager, entity, ref target) && ((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(target.m_Target))
			{
				return VehicleStateLocaleKey.Exporting;
			}
		}
		if ((truck.m_State & DeliveryTruckFlags.Delivering) == 0)
		{
			return VehicleStateLocaleKey.Transporting;
		}
		return VehicleStateLocaleKey.Delivering;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, Game.Vehicles.Hearse hearse, EntityManager entityManager)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity))
		{
			return VehicleStateLocaleKey.InvolvedInAccident;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity))
		{
			return VehicleStateLocaleKey.Parked;
		}
		if ((hearse.m_State & HearseFlags.Returning) != 0)
		{
			return VehicleStateLocaleKey.Returning;
		}
		if ((hearse.m_State & HearseFlags.Transporting) == 0)
		{
			return VehicleStateLocaleKey.Gathering;
		}
		return VehicleStateLocaleKey.Conveying;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, Game.Vehicles.CargoTransport cargoTransport, EntityManager entityManager)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity))
		{
			return VehicleStateLocaleKey.InvolvedInAccident;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity) || ((EntityManager)(ref entityManager)).HasComponent<ParkedTrain>(entity))
		{
			return VehicleStateLocaleKey.Parked;
		}
		if ((cargoTransport.m_State & CargoTransportFlags.Returning) != 0)
		{
			return VehicleStateLocaleKey.Returning;
		}
		if ((cargoTransport.m_State & CargoTransportFlags.Boarding) == 0)
		{
			return VehicleStateLocaleKey.EnRoute;
		}
		return VehicleStateLocaleKey.Loading;
	}

	public static VehicleStateLocaleKey GetStateKey(Entity entity, Game.Vehicles.Taxi taxi, EntityManager entityManager)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity))
		{
			return VehicleStateLocaleKey.InvolvedInAccident;
		}
		if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity))
		{
			return VehicleStateLocaleKey.Parked;
		}
		if ((taxi.m_State & TaxiFlags.Returning) != 0)
		{
			return VehicleStateLocaleKey.Returning;
		}
		if ((taxi.m_State & TaxiFlags.Dispatched) != 0)
		{
			return VehicleStateLocaleKey.Dispatched;
		}
		if ((taxi.m_State & TaxiFlags.Boarding) != 0)
		{
			return VehicleStateLocaleKey.Boarding;
		}
		if ((taxi.m_State & TaxiFlags.Transporting) != 0)
		{
			return VehicleStateLocaleKey.Transporting;
		}
		return VehicleStateLocaleKey.EnRoute;
	}

	public static VehicleLocaleKey GetPoliceVehicleLocaleKey(PolicePurpose purposeMask)
	{
		if ((purposeMask & PolicePurpose.Intelligence) != 0)
		{
			return VehicleLocaleKey.PoliceIntelligenceCar;
		}
		if ((purposeMask & PolicePurpose.Patrol) != 0)
		{
			return VehicleLocaleKey.PolicePatrolCar;
		}
		return VehicleLocaleKey.Vehicle;
	}
}

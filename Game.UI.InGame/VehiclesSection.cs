using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Citizens;
using Game.Companies;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class VehiclesSection : InfoSectionBase
{
	public readonly struct UIVehicle : IComparable<UIVehicle>
	{
		public Entity entity { get; }

		public VehicleLocaleKey vehicleKey { get; }

		public VehicleStateLocaleKey stateKey { get; }

		public UIVehicle(Entity entity, VehicleLocaleKey vehicleKey, VehicleStateLocaleKey stateKey)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			this.entity = entity;
			this.vehicleKey = vehicleKey;
			this.stateKey = stateKey;
		}

		public int CompareTo(UIVehicle other)
		{
			int num = vehicleKey - other.vehicleKey;
			if (num == 0)
			{
				return stateKey - other.stateKey;
			}
			return num;
		}
	}

	private DynamicBuffer<OwnedVehicle> m_Buffer;

	private Entity m_CompanyEntity;

	protected override string group => "VehiclesSection";

	private VehicleLocaleKey vehicleKey { get; set; }

	private int vehicleCount { get; set; }

	private int availableVehicleCount { get; set; }

	private int vehicleCapacity { get; set; }

	private NativeList<UIVehicle> vehicleList { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		vehicleList = new NativeList<UIVehicle>(50, AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		vehicleList.Dispose();
		base.OnDestroy();
	}

	protected override void Reset()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		vehicleCount = 0;
		vehicleCapacity = 0;
		vehicleList.Clear();
		m_Buffer = default(DynamicBuffer<OwnedVehicle>);
		m_CompanyEntity = Entity.Null;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!EntitiesExtensions.TryGetBuffer<OwnedVehicle>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref m_Buffer))
		{
			PrefabRef prefabRef = default(PrefabRef);
			if (CompanyUIUtils.HasCompany(((ComponentSystemBase)this).EntityManager, selectedEntity, selectedPrefab, out m_CompanyEntity) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, m_CompanyEntity, ref prefabRef))
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<TransportCompanyData>(prefabRef.m_Prefab))
				{
					return EntitiesExtensions.TryGetBuffer<OwnedVehicle>(((ComponentSystemBase)this).EntityManager, m_CompanyEntity, true, ref m_Buffer);
				}
			}
			return false;
		}
		return true;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		vehicleKey = VehicleLocaleKey.Vehicle;
		string item = VehicleLocaleKey.Vehicle.ToString();
		Entity val = selectedEntity;
		Entity val2 = selectedPrefab;
		if (m_CompanyEntity != Entity.Null)
		{
			val = m_CompanyEntity;
			PrefabRef prefabRef = default(PrefabRef);
			val2 = (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, m_CompanyEntity, ref prefabRef) ? prefabRef.m_Prefab : Entity.Null);
		}
		if (TryGetComponentWithUpgrades<HospitalData>(val, val2, out HospitalData data))
		{
			int ambulanceCapacity = data.m_AmbulanceCapacity;
			int medicalHelicopterCapacity = data.m_MedicalHelicopterCapacity;
			vehicleCapacity += ambulanceCapacity + medicalHelicopterCapacity;
		}
		if (TryGetComponentWithUpgrades<PoliceStationData>(val, val2, out PoliceStationData data2))
		{
			int patrolCarCapacity = data2.m_PatrolCarCapacity;
			int policeHelicopterCapacity = data2.m_PoliceHelicopterCapacity;
			vehicleCapacity += patrolCarCapacity + policeHelicopterCapacity;
		}
		if (TryGetComponentWithUpgrades<FireStationData>(val, val2, out FireStationData data3))
		{
			int fireEngineCapacity = data3.m_FireEngineCapacity;
			int fireHelicopterCapacity = data3.m_FireHelicopterCapacity;
			vehicleCapacity += fireEngineCapacity + fireHelicopterCapacity;
		}
		if (TryGetComponentWithUpgrades<PostFacilityData>(val, val2, out PostFacilityData data4))
		{
			int postVanCapacity = data4.m_PostVanCapacity;
			int postTruckCapacity = data4.m_PostTruckCapacity;
			vehicleCapacity += postVanCapacity + postTruckCapacity;
		}
		if (TryGetComponentWithUpgrades<MaintenanceDepotData>(val, val2, out MaintenanceDepotData data5))
		{
			vehicleCapacity += data5.m_VehicleCapacity;
		}
		if (TryGetComponentWithUpgrades<TransportDepotData>(val, val2, out TransportDepotData data6))
		{
			vehicleCapacity += data6.m_VehicleCapacity;
			item = VehicleLocaleKey.PublicTransportVehicle.ToString();
		}
		if (TryGetComponentWithUpgrades<DeathcareFacilityData>(val, val2, out DeathcareFacilityData data7))
		{
			vehicleCapacity += data7.m_HearseCapacity;
		}
		if (TryGetComponentWithUpgrades<GarbageFacilityData>(val, val2, out GarbageFacilityData data8))
		{
			vehicleCapacity += data8.m_VehicleCapacity;
		}
		if (TryGetComponentWithUpgrades<PrisonData>(val, val2, out PrisonData data9))
		{
			vehicleCapacity += data9.m_PrisonVanCapacity;
		}
		if (TryGetComponentWithUpgrades<EmergencyShelterData>(val, val2, out EmergencyShelterData data10))
		{
			vehicleCapacity += data10.m_VehicleCapacity;
		}
		TransportCompanyData transportCompanyData = default(TransportCompanyData);
		if (EntitiesExtensions.TryGetComponent<TransportCompanyData>(((ComponentSystemBase)this).EntityManager, val2, ref transportCompanyData))
		{
			vehicleCapacity += transportCompanyData.m_MaxTransports;
		}
		bool flag = m_Buffer.Length > 0 && vehicleCapacity == 0;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Household>(selectedEntity))
		{
			item = VehicleLocaleKey.HouseholdVehicle.ToString();
		}
		for (int i = 0; i < m_Buffer.Length; i++)
		{
			Entity vehicle = m_Buffer[i].m_Vehicle;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(vehicle))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<ParkedTrain>(vehicle))
				{
					AddVehicle(((ComponentSystemBase)this).EntityManager, vehicle, vehicleList);
				}
			}
		}
		base.tooltipKeys.Add(item);
		vehicleCount = vehicleList.Length;
		if (flag)
		{
			vehicleCapacity = m_Buffer.Length;
			availableVehicleCount = vehicleCapacity;
		}
		else
		{
			availableVehicleCount = VehicleUIUtils.GetAvailableVehicles(val, ((ComponentSystemBase)this).EntityManager);
		}
		NativeSortExtension.Sort<UIVehicle>(vehicleList);
	}

	public static void AddVehicle(EntityManager entityManager, Entity vehicle, NativeList<UIVehicle> vehicleList)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		VehicleStateLocaleKey stateKey = VehicleUIUtils.GetStateKey(vehicle, entityManager);
		PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(vehicle);
		VehicleLocaleKey vehicleLocaleKey = VehicleLocaleKey.Vehicle;
		if (((EntityManager)(ref entityManager)).HasComponent<Car>(vehicle))
		{
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.Ambulance>(vehicle))
			{
				vehicleLocaleKey = VehicleLocaleKey.Ambulance;
			}
			PoliceCarData policeCarData = default(PoliceCarData);
			if (EntitiesExtensions.TryGetComponent<PoliceCarData>(entityManager, componentData.m_Prefab, ref policeCarData))
			{
				vehicleLocaleKey = VehicleUIUtils.GetPoliceVehicleLocaleKey(policeCarData.m_PurposeMask);
			}
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.FireEngine>(vehicle))
			{
				vehicleLocaleKey = VehicleLocaleKey.FireEngine;
			}
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.PostVan>(vehicle))
			{
				vehicleLocaleKey = VehicleLocaleKey.PostVan;
			}
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.DeliveryTruck>(vehicle))
			{
				vehicleLocaleKey = VehicleLocaleKey.DeliveryTruck;
			}
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.Hearse>(vehicle))
			{
				vehicleLocaleKey = VehicleLocaleKey.Hearse;
			}
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.GarbageTruck>(vehicle))
			{
				vehicleLocaleKey = VehicleLocaleKey.GarbageTruck;
			}
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.Taxi>(vehicle))
			{
				vehicleLocaleKey = VehicleLocaleKey.Taxi;
			}
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.MaintenanceVehicle>(vehicle))
			{
				vehicleLocaleKey = VehicleLocaleKey.MaintenanceVehicle;
			}
			PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.PublicTransport>(vehicle) && EntitiesExtensions.TryGetComponent<PublicTransportVehicleData>(entityManager, componentData.m_Prefab, ref publicTransportVehicleData))
			{
				vehicleLocaleKey = (((publicTransportVehicleData.m_PurposeMask & PublicTransportPurpose.PrisonerTransport) != 0) ? VehicleLocaleKey.PrisonVan : (((publicTransportVehicleData.m_PurposeMask & PublicTransportPurpose.Evacuation) != 0) ? VehicleLocaleKey.EvacuationBus : VehicleLocaleKey.PublicTransportVehicle));
			}
		}
		if (((EntityManager)(ref entityManager)).HasComponent<Helicopter>(vehicle))
		{
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.Ambulance>(vehicle))
			{
				vehicleLocaleKey = VehicleLocaleKey.MedicalHelicopter;
			}
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.PoliceCar>(vehicle))
			{
				vehicleLocaleKey = VehicleLocaleKey.PoliceHelicopter;
			}
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.FireEngine>(vehicle))
			{
				vehicleLocaleKey = VehicleLocaleKey.FireHelicopter;
			}
		}
		UIVehicle uIVehicle = new UIVehicle(vehicle, vehicleLocaleKey, stateKey);
		vehicleList.Add(ref uIVehicle);
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("vehicleKey");
		writer.Write(Enum.GetName(typeof(VehicleLocaleKey), vehicleKey));
		writer.PropertyName("vehicleCount");
		writer.Write(vehicleCount);
		writer.PropertyName("availableVehicleCount");
		writer.Write(availableVehicleCount);
		writer.PropertyName("vehicleCapacity");
		writer.Write(vehicleCapacity);
		NativeSortExtension.Sort<UIVehicle>(vehicleList);
		writer.PropertyName("vehicleList");
		JsonWriterExtensions.ArrayBegin(writer, vehicleList.Length);
		for (int i = 0; i < vehicleList.Length; i++)
		{
			BindVehicle(m_NameSystem, writer, vehicleList[i]);
		}
		writer.ArrayEnd();
	}

	public static void BindVehicle(NameSystem nameSystem, IJsonWriter binder, UIVehicle vehicle)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		binder.TypeBegin("Game.UI.InGame.VehiclesSection.Vehicle");
		binder.PropertyName("entity");
		UnityWriters.Write(binder, vehicle.entity);
		binder.PropertyName("name");
		nameSystem.BindName(binder, vehicle.entity);
		binder.PropertyName("vehicleKey");
		binder.Write(Enum.GetName(typeof(VehicleLocaleKey), vehicle.vehicleKey));
		binder.PropertyName("stateKey");
		binder.Write(Enum.GetName(typeof(VehicleStateLocaleKey), vehicle.stateKey));
		binder.TypeEnd();
	}

	[Preserve]
	public VehiclesSection()
	{
	}
}

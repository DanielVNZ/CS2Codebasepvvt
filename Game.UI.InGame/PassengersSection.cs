using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Creatures;
using Game.Events;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class PassengersSection : InfoSectionBase
{
	protected override string group => "PassengersSection";

	private int passengers { get; set; }

	private int maxPassengers { get; set; }

	private int pets { get; set; }

	private VehiclePassengerLocaleKey vehiclePassengerKey { get; set; }

	protected override Entity selectedEntity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Controller controller = default(Controller);
			if (EntitiesExtensions.TryGetComponent<Controller>(((ComponentSystemBase)this).EntityManager, base.selectedEntity, ref controller))
			{
				return controller.m_Controller;
			}
			return base.selectedEntity;
		}
	}

	protected override Entity selectedPrefab
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			Controller controller = default(Controller);
			PrefabRef prefabRef = default(PrefabRef);
			if (EntitiesExtensions.TryGetComponent<Controller>(((ComponentSystemBase)this).EntityManager, base.selectedEntity, ref controller) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, controller.m_Controller, ref prefabRef))
			{
				return prefabRef.m_Prefab;
			}
			return base.selectedPrefab;
		}
	}

	protected override void Reset()
	{
		passengers = 0;
		maxPassengers = 0;
		pets = 0;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Vehicle>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Passenger>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					PersonalCarData personalCarData = default(PersonalCarData);
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.PersonalCar>(selectedEntity) && EntitiesExtensions.TryGetComponent<PersonalCarData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref personalCarData))
					{
						return personalCarData.m_PassengerCapacity > 0;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					TaxiData taxiData = default(TaxiData);
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.Taxi>(selectedEntity) && EntitiesExtensions.TryGetComponent<TaxiData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref taxiData))
					{
						return taxiData.m_PassengerCapacity > 0;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					PoliceCarData policeCarData = default(PoliceCarData);
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.PoliceCar>(selectedEntity) && EntitiesExtensions.TryGetComponent<PoliceCarData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref policeCarData))
					{
						return policeCarData.m_CriminalCapacity > 0;
					}
					int num = 0;
					DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
					PublicTransportVehicleData publicTransportVehicleData2 = default(PublicTransportVehicleData);
					if (EntitiesExtensions.TryGetBuffer<LayoutElement>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val))
					{
						PrefabRef prefabRef = default(PrefabRef);
						PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
						for (int i = 0; i < val.Length; i++)
						{
							Entity vehicle = val[i].m_Vehicle;
							if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, vehicle, ref prefabRef) && EntitiesExtensions.TryGetComponent<PublicTransportVehicleData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref publicTransportVehicleData))
							{
								num += publicTransportVehicleData.m_PassengerCapacity;
							}
						}
					}
					else if (EntitiesExtensions.TryGetComponent<PublicTransportVehicleData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref publicTransportVehicleData2))
					{
						num = publicTransportVehicleData2.m_PassengerCapacity;
					}
					return num > 0;
				}
			}
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
		EntityManager entityManager;
		if (EntitiesExtensions.TryGetBuffer<LayoutElement>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val))
		{
			DynamicBuffer<Passenger> val2 = default(DynamicBuffer<Passenger>);
			PrefabRef prefabRef = default(PrefabRef);
			for (int i = 0; i < val.Length; i++)
			{
				Entity vehicle = val[i].m_Vehicle;
				if (EntitiesExtensions.TryGetBuffer<Passenger>(((ComponentSystemBase)this).EntityManager, vehicle, true, ref val2))
				{
					for (int j = 0; j < val2.Length; j++)
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<Game.Creatures.Pet>(val2[j].m_Passenger))
						{
							pets++;
						}
						else
						{
							passengers++;
						}
					}
				}
				if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, vehicle, ref prefabRef))
				{
					Entity prefab = prefabRef.m_Prefab;
					AddPassengerCapacity(prefab);
				}
			}
		}
		else
		{
			DynamicBuffer<Passenger> val3 = default(DynamicBuffer<Passenger>);
			if (EntitiesExtensions.TryGetBuffer<Passenger>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val3))
			{
				for (int k = 0; k < val3.Length; k++)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Creatures.Pet>(val3[k].m_Passenger))
					{
						pets++;
					}
					else
					{
						passengers++;
					}
				}
			}
			AddPassengerCapacity(selectedPrefab);
		}
		bool flag = false;
		int num = 0;
		vehiclePassengerKey = VehiclePassengerLocaleKey.Passenger;
		Game.Vehicles.PersonalCar personalCar = default(Game.Vehicles.PersonalCar);
		Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.PersonalCar>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref personalCar))
		{
			flag = (personalCar.m_State & PersonalCarFlags.DummyTraffic) != 0;
			num = math.min(1, maxPassengers);
		}
		else if (EntitiesExtensions.TryGetComponent<Game.Vehicles.PublicTransport>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref publicTransport))
		{
			if ((publicTransport.m_State & PublicTransportFlags.PrisonerTransport) != 0)
			{
				vehiclePassengerKey = VehiclePassengerLocaleKey.Prisoner;
			}
			flag = (publicTransport.m_State & PublicTransportFlags.DummyTraffic) != 0;
		}
		PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
		if (flag && EntitiesExtensions.TryGetComponent<PseudoRandomSeed>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref pseudoRandomSeed))
		{
			Random random = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kDummyPassengers);
			passengers = math.max(passengers, ((Random)(ref random)).NextInt(num, maxPassengers + 1));
			if (passengers != 0)
			{
				int num2 = ((Random)(ref random)).NextInt(0, (passengers + ((Random)(ref random)).NextInt(10)) / 10 + 1);
				for (int l = 0; l < num2; l++)
				{
					pets += math.max(1, ((Random)(ref random)).NextInt(0, 4));
				}
			}
		}
		passengers = math.max(0, passengers);
	}

	private void AddPassengerCapacity(Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		PersonalCarData personalCarData = default(PersonalCarData);
		TaxiData taxiData = default(TaxiData);
		PoliceCarData policeCarData = default(PoliceCarData);
		PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
		if (EntitiesExtensions.TryGetComponent<PersonalCarData>(((ComponentSystemBase)this).EntityManager, prefab, ref personalCarData))
		{
			maxPassengers += personalCarData.m_PassengerCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<TaxiData>(((ComponentSystemBase)this).EntityManager, prefab, ref taxiData))
		{
			maxPassengers += taxiData.m_PassengerCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<PoliceCarData>(((ComponentSystemBase)this).EntityManager, prefab, ref policeCarData))
		{
			maxPassengers += policeCarData.m_CriminalCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<PublicTransportVehicleData>(((ComponentSystemBase)this).EntityManager, prefab, ref publicTransportVehicleData))
		{
			maxPassengers += publicTransportVehicleData.m_PassengerCapacity;
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("passengers");
		writer.Write(passengers);
		writer.PropertyName("maxPassengers");
		writer.Write(maxPassengers);
		writer.PropertyName("pets");
		writer.Write(pets);
		writer.PropertyName("vehiclePassengerKey");
		writer.Write(Enum.GetName(typeof(VehiclePassengerLocaleKey), vehiclePassengerKey));
	}

	[Preserve]
	public PassengersSection()
	{
	}
}

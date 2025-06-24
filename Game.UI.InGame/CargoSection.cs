using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Economy;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class CargoSection : InfoSectionBase
{
	private enum CargoKey
	{
		Cargo
	}

	private ResourcePrefabs m_ResourcePrefabs;

	protected override string group => "CargoSection";

	private int cargo { get; set; }

	private int capacity { get; set; }

	private CargoKey cargoKey { get; set; }

	private NativeList<UIResource> rawMaterials { get; set; }

	private NativeList<UIResource> processedGoods { get; set; }

	private NativeList<UIResource> mail { get; set; }

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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		rawMaterials.Clear();
		processedGoods.Clear();
		mail.Clear();
		cargo = 0;
		capacity = 0;
		cargoKey = CargoKey.Cargo;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		rawMaterials = new NativeList<UIResource>(AllocatorHandle.op_Implicit((Allocator)4));
		processedGoods = new NativeList<UIResource>(AllocatorHandle.op_Implicit((Allocator)4));
		mail = new NativeList<UIResource>(AllocatorHandle.op_Implicit((Allocator)4));
		m_ResourcePrefabs = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>().GetPrefabs();
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
		rawMaterials.Dispose();
		processedGoods.Dispose();
		mail.Dispose();
		base.OnDestroy();
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(selectedEntity))
		{
			return false;
		}
		DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
		DeliveryTruckData deliveryTruckData2 = default(DeliveryTruckData);
		CargoTransportVehicleData cargoTransportVehicleData2 = default(CargoTransportVehicleData);
		if (EntitiesExtensions.TryGetBuffer<LayoutElement>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val) && val.Length != 0)
		{
			PrefabRef prefabRef = default(PrefabRef);
			DeliveryTruckData deliveryTruckData = default(DeliveryTruckData);
			CargoTransportVehicleData cargoTransportVehicleData = default(CargoTransportVehicleData);
			for (int i = 0; i < val.Length; i++)
			{
				Entity vehicle = val[i].m_Vehicle;
				if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, vehicle, ref prefabRef))
				{
					if (EntitiesExtensions.TryGetComponent<DeliveryTruckData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref deliveryTruckData))
					{
						capacity += deliveryTruckData.m_CargoCapacity;
					}
					else if (EntitiesExtensions.TryGetComponent<CargoTransportVehicleData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref cargoTransportVehicleData))
					{
						capacity += cargoTransportVehicleData.m_CargoCapacity;
					}
				}
			}
		}
		else if (EntitiesExtensions.TryGetComponent<DeliveryTruckData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref deliveryTruckData2))
		{
			capacity = deliveryTruckData2.m_CargoCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<CargoTransportVehicleData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref cargoTransportVehicleData2))
		{
			capacity = cargoTransportVehicleData2.m_CargoCapacity;
		}
		return capacity > 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		cargoKey = CargoKey.Cargo;
		Game.Vehicles.DeliveryTruck deliveryTruck = default(Game.Vehicles.DeliveryTruck);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.DeliveryTruck>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref deliveryTruck))
		{
			Resource resource = Resource.NoResource;
			DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
			if (EntitiesExtensions.TryGetBuffer<LayoutElement>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val) && val.Length != 0)
			{
				int num = 0;
				Game.Vehicles.DeliveryTruck deliveryTruck2 = default(Game.Vehicles.DeliveryTruck);
				for (int i = 0; i < val.Length; i++)
				{
					Entity vehicle = val[i].m_Vehicle;
					if (EntitiesExtensions.TryGetComponent<Game.Vehicles.DeliveryTruck>(((ComponentSystemBase)this).EntityManager, vehicle, ref deliveryTruck2))
					{
						resource |= deliveryTruck2.m_Resource;
						if ((deliveryTruck2.m_State & DeliveryTruckFlags.Loaded) != 0)
						{
							num += deliveryTruck2.m_Amount;
						}
					}
				}
				cargo = num;
			}
			else
			{
				resource = deliveryTruck.m_Resource;
				cargo = (((deliveryTruck.m_State & DeliveryTruckFlags.Loaded) != 0) ? deliveryTruck.m_Amount : 0);
			}
			UIResource.CategorizeResources(resource, cargo, rawMaterials, processedGoods, mail, ((ComponentSystemBase)this).EntityManager, m_ResourcePrefabs);
			return;
		}
		NativeList<Resources> target = default(NativeList<Resources>);
		target._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
		DynamicBuffer<LayoutElement> val2 = default(DynamicBuffer<LayoutElement>);
		DynamicBuffer<Resources> source2 = default(DynamicBuffer<Resources>);
		if (EntitiesExtensions.TryGetBuffer<LayoutElement>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val2))
		{
			DynamicBuffer<Resources> source = default(DynamicBuffer<Resources>);
			for (int j = 0; j < val2.Length; j++)
			{
				Entity vehicle2 = val2[j].m_Vehicle;
				if (EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, vehicle2, true, ref source))
				{
					AddResources(source, target);
				}
			}
		}
		else if (EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref source2))
		{
			AddResources(source2, target);
		}
		for (int k = 0; k < target.Length; k++)
		{
			Resources resources = target[k];
			UIResource.CategorizeResources(resources.m_Resource, resources.m_Amount, rawMaterials, processedGoods, mail, ((ComponentSystemBase)this).EntityManager, m_ResourcePrefabs);
			cargo += resources.m_Amount;
		}
		target.Dispose();
	}

	private void AddResources(DynamicBuffer<Resources> source, NativeList<Resources> target)
	{
		for (int i = 0; i < source.Length; i++)
		{
			Resources resources = source[i];
			if (resources.m_Amount == 0)
			{
				continue;
			}
			int num = 0;
			while (true)
			{
				if (num < target.Length)
				{
					Resources resources2 = target[num];
					if (resources2.m_Resource == resources.m_Resource)
					{
						resources2.m_Amount += resources.m_Amount;
						target[num] = resources2;
						break;
					}
					num++;
					continue;
				}
				target.Add(ref resources);
				break;
			}
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("cargo");
		writer.Write(cargo);
		writer.PropertyName("capacity");
		writer.Write(capacity);
		NativeSortExtension.Sort<UIResource>(rawMaterials);
		writer.PropertyName("rawMaterials");
		JsonWriterExtensions.ArrayBegin(writer, rawMaterials.Length);
		for (int i = 0; i < rawMaterials.Length; i++)
		{
			JsonWriterExtensions.Write<UIResource>(writer, rawMaterials[i]);
		}
		writer.ArrayEnd();
		NativeSortExtension.Sort<UIResource>(processedGoods);
		writer.PropertyName("processedGoods");
		JsonWriterExtensions.ArrayBegin(writer, processedGoods.Length);
		for (int j = 0; j < processedGoods.Length; j++)
		{
			JsonWriterExtensions.Write<UIResource>(writer, processedGoods[j]);
		}
		writer.ArrayEnd();
		NativeSortExtension.Sort<UIResource>(mail);
		writer.PropertyName("mail");
		JsonWriterExtensions.ArrayBegin(writer, mail.Length);
		for (int k = 0; k < mail.Length; k++)
		{
			JsonWriterExtensions.Write<UIResource>(writer, mail[k]);
		}
		writer.ArrayEnd();
		writer.PropertyName("cargoKey");
		writer.Write(Enum.GetName(typeof(CargoKey), cargoKey));
	}

	[Preserve]
	public CargoSection()
	{
	}
}

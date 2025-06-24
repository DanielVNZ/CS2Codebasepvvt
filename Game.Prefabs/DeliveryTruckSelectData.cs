using Game.Common;
using Game.Economy;
using Game.Objects;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct DeliveryTruckSelectData
{
	private NativeArray<DeliveryTruckSelectItem> m_Items;

	public DeliveryTruckSelectData(NativeArray<DeliveryTruckSelectItem> items)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Items = items;
	}

	public void GetCapacityRange(Resource resources, out int min, out int max)
	{
		min = 0;
		max = 0;
		for (int i = 0; i < m_Items.Length; i++)
		{
			DeliveryTruckSelectItem deliveryTruckSelectItem = m_Items[i];
			if ((deliveryTruckSelectItem.m_Resources & resources) == resources)
			{
				min = deliveryTruckSelectItem.m_Capacity;
				break;
			}
		}
		for (int num = m_Items.Length - 1; num >= 0; num--)
		{
			DeliveryTruckSelectItem deliveryTruckSelectItem2 = m_Items[num];
			if ((deliveryTruckSelectItem2.m_Resources & resources) == resources)
			{
				max = deliveryTruckSelectItem2.m_Capacity;
				break;
			}
		}
	}

	public bool TrySelectItem(ref Random random, Resource resources, int capacity, out DeliveryTruckSelectItem item)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		int2 val = default(int2);
		((int2)(ref val))._002Ector(0, m_Items.Length);
		while (val.y > val.x)
		{
			int num = math.csum(val) >> 1;
			DeliveryTruckSelectItem deliveryTruckSelectItem = m_Items[num];
			if (deliveryTruckSelectItem.m_Capacity == capacity)
			{
				val = int2.op_Implicit(num);
				break;
			}
			val = math.select(new int2(num + 1, val.y), new int2(val.x, num), deliveryTruckSelectItem.m_Capacity > capacity);
		}
		item = default(DeliveryTruckSelectItem);
		int num2 = 0;
		while (val.y < m_Items.Length)
		{
			DeliveryTruckSelectItem deliveryTruckSelectItem2 = m_Items[val.y++];
			int2 val2 = new int2(deliveryTruckSelectItem2.m_Cost, item.m_Cost) * math.min(int2.op_Implicit(capacity), new int2(item.m_Capacity, deliveryTruckSelectItem2.m_Capacity));
			if (val2.x > val2.y)
			{
				break;
			}
			bool flag = (deliveryTruckSelectItem2.m_Resources & resources) == resources;
			int num3 = math.select(0, 100, flag);
			num2 = num3 + math.select(num2, 0, flag & (val2.x < val2.y));
			if (((Random)(ref random)).NextInt(num2) < num3)
			{
				item = deliveryTruckSelectItem2;
			}
		}
		while (val.x > 0)
		{
			DeliveryTruckSelectItem deliveryTruckSelectItem3 = m_Items[--val.x];
			int2 val3 = new int2(deliveryTruckSelectItem3.m_Cost, item.m_Cost) * math.min(int2.op_Implicit(capacity), new int2(item.m_Capacity, deliveryTruckSelectItem3.m_Capacity));
			if (val3.x > val3.y)
			{
				break;
			}
			bool flag2 = (deliveryTruckSelectItem3.m_Resources & resources) == resources;
			int num4 = math.select(0, 100, flag2);
			num2 = num4 + math.select(num2, 0, flag2 & (val3.x < val3.y));
			if (((Random)(ref random)).NextInt(num2) < num4)
			{
				item = deliveryTruckSelectItem3;
			}
		}
		return item.m_Prefab1 != Entity.Null;
	}

	public Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, ref ComponentLookup<DeliveryTruckData> deliveryTruckDatas, ref ComponentLookup<ObjectData> objectDatas, Resource resource, Resource returnResource, ref int amount, ref int returnAmount, Transform transform, Entity source, DeliveryTruckFlags state, uint delay = 0u)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Resource resources = resource | returnResource;
		int capacity = math.max(amount, returnAmount);
		if (TrySelectItem(ref random, resources, capacity, out var item))
		{
			return CreateVehicle(commandBuffer, jobIndex, ref random, ref deliveryTruckDatas, ref objectDatas, item, resource, returnResource, ref amount, ref returnAmount, transform, source, state, delay);
		}
		return Entity.Null;
	}

	public Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, ref ComponentLookup<DeliveryTruckData> deliveryTruckDatas, ref ComponentLookup<ObjectData> objectDatas, DeliveryTruckSelectItem selectItem, Resource resource, Resource returnResource, ref int amount, ref int returnAmount, Transform transform, Entity source, DeliveryTruckFlags state, uint delay = 0u)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		int amount2 = amount;
		int returnAmount2 = returnAmount;
		Entity val = CreateVehicle(commandBuffer, jobIndex, ref random, ref deliveryTruckDatas, ref objectDatas, selectItem.m_Prefab1, resource, returnResource, ref amount2, ref returnAmount2, transform, source, state, delay);
		if (selectItem.m_Prefab2 != Entity.Null)
		{
			DynamicBuffer<LayoutElement> val2 = ((ParallelWriter)(ref commandBuffer)).AddBuffer<LayoutElement>(jobIndex, val);
			val2.Add(new LayoutElement(val));
			Entity val3 = CreateVehicle(commandBuffer, jobIndex, ref random, ref deliveryTruckDatas, ref objectDatas, selectItem.m_Prefab2, resource, returnResource, ref amount2, ref returnAmount2, transform, source, state & DeliveryTruckFlags.Loaded, delay);
			((ParallelWriter)(ref commandBuffer)).SetComponent<Controller>(jobIndex, val3, new Controller(val));
			val2.Add(new LayoutElement(val3));
			if (selectItem.m_Prefab3 != Entity.Null)
			{
				val3 = CreateVehicle(commandBuffer, jobIndex, ref random, ref deliveryTruckDatas, ref objectDatas, selectItem.m_Prefab3, resource, returnResource, ref amount2, ref returnAmount2, transform, source, state & DeliveryTruckFlags.Loaded, delay);
				((ParallelWriter)(ref commandBuffer)).SetComponent<Controller>(jobIndex, val3, new Controller(val));
				val2.Add(new LayoutElement(val3));
			}
			if (selectItem.m_Prefab4 != Entity.Null)
			{
				val3 = CreateVehicle(commandBuffer, jobIndex, ref random, ref deliveryTruckDatas, ref objectDatas, selectItem.m_Prefab4, resource, returnResource, ref amount2, ref returnAmount2, transform, source, state & DeliveryTruckFlags.Loaded, delay);
				((ParallelWriter)(ref commandBuffer)).SetComponent<Controller>(jobIndex, val3, new Controller(val));
				val2.Add(new LayoutElement(val3));
			}
		}
		amount -= amount2;
		returnAmount -= returnAmount2;
		return val;
	}

	private Entity CreateVehicle(ParallelWriter commandBuffer, int jobIndex, ref Random random, ref ComponentLookup<DeliveryTruckData> deliveryTruckDatas, ref ComponentLookup<ObjectData> objectDatas, Entity prefab, Resource resource, Resource returnResource, ref int amount, ref int returnAmount, Transform transform, Entity source, DeliveryTruckFlags state, uint delay)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		DeliveryTruckData deliveryTruckData = deliveryTruckDatas[prefab];
		ObjectData objectData = objectDatas[prefab];
		Game.Vehicles.DeliveryTruck deliveryTruck = new Game.Vehicles.DeliveryTruck
		{
			m_State = state
		};
		if ((resource & deliveryTruckData.m_TransportedResources) != Resource.NoResource && amount > 0)
		{
			deliveryTruck.m_Amount = math.min(amount, deliveryTruckData.m_CargoCapacity);
			if (deliveryTruck.m_Amount > 0)
			{
				deliveryTruck.m_Resource = resource;
				amount -= deliveryTruck.m_Amount;
			}
		}
		Entity val = ((ParallelWriter)(ref commandBuffer)).CreateEntity(jobIndex, objectData.m_Archetype);
		((ParallelWriter)(ref commandBuffer)).SetComponent<Transform>(jobIndex, val, transform);
		((ParallelWriter)(ref commandBuffer)).SetComponent<Game.Vehicles.DeliveryTruck>(jobIndex, val, deliveryTruck);
		((ParallelWriter)(ref commandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(prefab));
		((ParallelWriter)(ref commandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val, new PseudoRandomSeed(ref random));
		if (source != Entity.Null)
		{
			((ParallelWriter)(ref commandBuffer)).AddComponent<TripSource>(jobIndex, val, new TripSource(source, delay));
			((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, val, default(Unspawned));
		}
		if ((returnResource & deliveryTruckData.m_TransportedResources) != Resource.NoResource)
		{
			ReturnLoad returnLoad = new ReturnLoad
			{
				m_Amount = math.min(returnAmount, deliveryTruckData.m_CargoCapacity)
			};
			if (returnLoad.m_Amount > 0)
			{
				returnLoad.m_Resource = returnResource;
				returnAmount -= returnLoad.m_Amount;
				((ParallelWriter)(ref commandBuffer)).AddComponent<ReturnLoad>(jobIndex, val, returnLoad);
			}
		}
		return val;
	}
}

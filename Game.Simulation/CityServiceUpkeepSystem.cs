using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Notifications;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CityServiceUpkeepSystem : GameSystemBase
{
	[BurstCompile]
	private struct CityServiceUpkeepJob : IJobChunk
	{
		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> m_OwnedVehicleBufType;

		public BufferTypeHandle<Resources> m_ResourcesType;

		public ComponentTypeHandle<Game.Buildings.ResourceConsumer> m_ResourceConsumerType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> m_ServiceObjects;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> m_ServiceUpkeepDatas;

		[ReadOnly]
		public BufferLookup<UpkeepModifierData> m_UpkeepModifiers;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public ComponentLookup<ResourceConsumerData> m_ResourceConsumerDatas;

		[ReadOnly]
		public ComponentLookup<ServiceUsage> m_ServiceUsages;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_Limits;

		[ReadOnly]
		public BufferLookup<ServiceBudgetData> m_ServiceBudgetDatas;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTrucks;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public ComponentLookup<Quantity> m_QuantityData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		public ComponentLookup<PlayerMoney> m_PlayerMoney;

		public uint m_UpdateFrameIndex;

		public Entity m_City;

		public Entity m_BudgetDataEntity;

		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public DeliveryTruckSelectData m_DeliveryTruckSelectData;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		public EntityCommandBuffer m_CommandBuffer;

		public IconCommandBuffer m_IconCommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			DynamicBuffer<ServiceBudgetData> serviceBudgets = m_ServiceBudgetDatas[m_BudgetDataEntity];
			NativeList<ServiceUpkeepData> val = default(NativeList<ServiceUpkeepData>);
			val._002Ector(4, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<UpkeepModifierData> val2 = default(NativeList<UpkeepModifierData>);
			val2._002Ector(4, AllocatorHandle.op_Implicit((Allocator)2));
			NativeParallelHashMap<Entity, bool> notifications = default(NativeParallelHashMap<Entity, bool>);
			notifications._002Ector(4, AllocatorHandle.op_Implicit((Allocator)2));
			NativeArray<int> val3 = default(NativeArray<int>);
			val3._002Ector(EconomyUtils.ResourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			BufferAccessor<OwnedVehicle> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedVehicle>(ref m_OwnedVehicleBufType);
			BufferAccessor<Resources> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourcesType);
			NativeArray<Game.Buildings.ResourceConsumer> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.ResourceConsumer>(ref m_ResourceConsumerType);
			StorageLimitData data = default(StorageLimitData);
			DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
			Game.Vehicles.DeliveryTruck deliveryTruck = default(Game.Vehicles.DeliveryTruck);
			DynamicBuffer<LayoutElement> val6 = default(DynamicBuffer<LayoutElement>);
			Game.Vehicles.DeliveryTruck deliveryTruck2 = default(Game.Vehicles.DeliveryTruck);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				val.Clear();
				val2.Clear();
				CollectionUtils.Fill<int>(val3, 0);
				Entity val4 = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				DynamicBuffer<Resources> resources = bufferAccessor2[i];
				int serviceBudget = GetServiceBudget(prefab, serviceBudgets);
				GetUpkeepWithUsageScale(val, m_ServiceUpkeepDatas, m_InstalledUpgrades, m_Prefabs, m_ServiceUsages, val4, prefab, mainBuildingDisabled: false);
				GetUpkeepModifierData(val2, m_InstalledUpgrades, m_Prefabs, m_UpkeepModifiers, val4);
				Random random = m_RandomSeed.GetRandom(val4.Index);
				((Random)(ref random)).NextBool();
				GetStorageTargets(val3, val2, val4, prefab);
				m_Limits.TryGetComponent(prefab, ref data);
				if (m_InstalledUpgrades.TryGetBuffer(val4, ref upgrades))
				{
					UpgradeUtils.CombineStats<StorageLimitData>(ref data, upgrades, ref m_Prefabs, ref m_Limits);
				}
				bool flag = TickConsumer(serviceBudget, data.m_Limit, val, val2, resources, ref random);
				int num = 0;
				Enumerator<int> enumerator = val3.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						int current = enumerator.Current;
						num += current;
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
				if (num > 0)
				{
					notifications.Clear();
					if (nativeArray3.Length != 0)
					{
						ref Game.Buildings.ResourceConsumer reference = ref CollectionUtils.ElementAt<Game.Buildings.ResourceConsumer>(nativeArray3, i);
						bool wasEmpty = reference.m_ResourceAvailability == 0;
						reference.m_ResourceAvailability = GetResourceAvailability(val, resources, val3);
						bool isEmpty = reference.m_ResourceAvailability == 0;
						UpdateNotification(notifications, prefab, wasEmpty, isEmpty);
					}
					Enumerator<Entity, bool> enumerator2 = notifications.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							KeyValue<Entity, bool> current2 = enumerator2.Current;
							if (current2.Value)
							{
								m_IconCommandBuffer.Add(val4, current2.Key, IconPriority.Problem);
							}
							else
							{
								m_IconCommandBuffer.Remove(val4, current2.Key);
							}
						}
					}
					finally
					{
						((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
					}
					int num2;
					if (data.m_Limit > 0)
					{
						num2 = data.m_Limit;
						float num3 = (float)data.m_Limit / (float)num;
						for (int j = 0; j < val3.Length; j++)
						{
							val3[j] = Mathf.RoundToInt(num3 * (float)val3[j]);
						}
					}
					else
					{
						num2 = num;
					}
					num2 -= EconomyUtils.GetTotalStorageUsed(resources);
					int min;
					if (bufferAccessor.Length != 0)
					{
						DynamicBuffer<OwnedVehicle> val5 = bufferAccessor[i];
						for (int k = 0; k < val5.Length; k++)
						{
							Entity vehicle = val5[k].m_Vehicle;
							if (!m_DeliveryTrucks.TryGetComponent(vehicle, ref deliveryTruck) || (deliveryTruck.m_State & DeliveryTruckFlags.DummyTraffic) != 0)
							{
								continue;
							}
							if (m_LayoutElements.TryGetBuffer(vehicle, ref val6) && val6.Length != 0)
							{
								for (int l = 0; l < val6.Length; l++)
								{
									if (m_DeliveryTrucks.TryGetComponent(val6[l].m_Vehicle, ref deliveryTruck2) && deliveryTruck2.m_Resource != Resource.NoResource)
									{
										num2 -= deliveryTruck2.m_Amount;
										ref NativeArray<int> reference2 = ref val3;
										min = EconomyUtils.GetResourceIndex(deliveryTruck2.m_Resource);
										reference2[min] -= deliveryTruck2.m_Amount;
									}
								}
							}
							else if (deliveryTruck.m_Resource != Resource.NoResource)
							{
								num2 -= deliveryTruck.m_Amount;
								ref NativeArray<int> reference2 = ref val3;
								min = EconomyUtils.GetResourceIndex(deliveryTruck.m_Resource);
								reference2[min] -= deliveryTruck.m_Amount;
							}
						}
					}
					for (int m = 0; m < val3.Length; m++)
					{
						int num4 = val3[m];
						if (num4 > 0)
						{
							Resource resource = EconomyUtils.GetResource(m);
							int resources2 = EconomyUtils.GetResources(resource, resources);
							m_DeliveryTruckSelectData.GetCapacityRange(resource, out min, out var max);
							int num5 = num4 - resources2;
							if (num2 > 0 && num5 > 0 && ((Random)(ref random)).NextInt(math.max(1, num4 * 3 / 4)) > resources2 - num4 / 4)
							{
								int num6 = math.min(num5, num2);
								num2 -= num6;
								ResourceBuyer resourceBuyer = new ResourceBuyer
								{
									m_Payer = val4,
									m_AmountNeeded = math.min(num6, max),
									m_Flags = (SetupTargetFlags.Industrial | SetupTargetFlags.Import),
									m_ResourceNeeded = resource
								};
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ResourceBuyer>(val4, resourceBuyer);
							}
						}
					}
				}
				EconomyUtils.GetResources(Resource.Money, resources);
				_ = 0;
				if (flag)
				{
					QuantityUpdated(val4);
				}
			}
		}

		private void GetStorageTargets(NativeArray<int> storageTargets, NativeList<UpkeepModifierData> upkeepModifiers, Entity entity, Entity prefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ServiceUpkeepData> val = default(DynamicBuffer<ServiceUpkeepData>);
			if (m_ServiceUpkeepDatas.TryGetBuffer(prefab, ref val))
			{
				Enumerator<ServiceUpkeepData> enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ServiceUpkeepData current = enumerator.Current;
						float num = GetUpkeepModifier(current.m_Upkeep.m_Resource, upkeepModifiers).Transform(current.m_Upkeep.m_Amount);
						if (IsMaterialResource(m_ResourceDatas, m_ResourcePrefabs, current.m_Upkeep))
						{
							ref NativeArray<int> reference = ref storageTargets;
							int resourceIndex = EconomyUtils.GetResourceIndex(current.m_Upkeep.m_Resource);
							reference[resourceIndex] += (int)math.round(num);
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			DynamicBuffer<InstalledUpgrade> val2 = default(DynamicBuffer<InstalledUpgrade>);
			if (!m_InstalledUpgrades.TryGetBuffer(entity, ref val2))
			{
				return;
			}
			Enumerator<InstalledUpgrade> enumerator2 = val2.GetEnumerator();
			try
			{
				PrefabRef prefabRef = default(PrefabRef);
				DynamicBuffer<ServiceUpkeepData> val3 = default(DynamicBuffer<ServiceUpkeepData>);
				while (enumerator2.MoveNext())
				{
					InstalledUpgrade current2 = enumerator2.Current;
					if (BuildingUtils.CheckOption(current2, BuildingOption.Inactive) || !m_Prefabs.TryGetComponent(current2.m_Upgrade, ref prefabRef) || !m_ServiceUpkeepDatas.TryGetBuffer(prefabRef.m_Prefab, ref val3))
					{
						continue;
					}
					Enumerator<ServiceUpkeepData> enumerator = val3.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							ServiceUpkeepData current3 = enumerator.Current;
							float num2 = GetUpkeepModifier(current3.m_Upkeep.m_Resource, upkeepModifiers).Transform(current3.m_Upkeep.m_Amount);
							if (IsMaterialResource(m_ResourceDatas, m_ResourcePrefabs, current3.m_Upkeep))
							{
								ref NativeArray<int> reference = ref storageTargets;
								int resourceIndex = EconomyUtils.GetResourceIndex(current3.m_Upkeep.m_Resource);
								reference[resourceIndex] += (int)math.round(num2);
							}
						}
					}
					finally
					{
						((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
			}
		}

		private int GetServiceBudget(Entity prefab, DynamicBuffer<ServiceBudgetData> serviceBudgets)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			ServiceObjectData serviceObjectData = default(ServiceObjectData);
			if (m_ServiceObjects.TryGetComponent(prefab, ref serviceObjectData))
			{
				for (int i = 0; i < serviceBudgets.Length; i++)
				{
					if (serviceBudgets[i].m_Service == serviceObjectData.m_Service)
					{
						return serviceBudgets[i].m_Budget;
					}
				}
			}
			return 100;
		}

		private bool TickConsumer(int serviceBudget, int storageLimit, NativeList<ServiceUpkeepData> serviceUpkeepDatas, NativeList<UpkeepModifierData> upkeepModifiers, DynamicBuffer<Resources> resources, ref Random random)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			Enumerator<ServiceUpkeepData> enumerator = serviceUpkeepDatas.GetEnumerator();
			try
			{
				int4 val = default(int4);
				while (enumerator.MoveNext())
				{
					ServiceUpkeepData current = enumerator.Current;
					Resource resource = current.m_Upkeep.m_Resource;
					bool flag2 = IsMaterialResource(m_ResourceDatas, m_ResourcePrefabs, current.m_Upkeep);
					if (current.m_Upkeep.m_Amount <= 0)
					{
						continue;
					}
					float num = GetUpkeepModifier(resource, upkeepModifiers).Transform(current.m_Upkeep.m_Amount);
					int num2 = MathUtils.RoundToIntRandom(ref random, num / (float)kUpdatesPerDay);
					if (num2 > 0)
					{
						_ = m_PlayerMoney[m_City];
						if (flag2)
						{
							int num3 = EconomyUtils.AddResources(resource, -num2, resources);
							int num4 = num3 + num2;
							num4 = Mathf.RoundToInt((float)num4 / (float)math.max(1, storageLimit) * 100f);
							num3 = Mathf.RoundToInt((float)num3 / (float)math.max(1, storageLimit) * 100f);
							((int4)(ref val))._002Ector(0, 33, 50, 66);
							flag |= math.any(num4 > val != num3 > val);
						}
					}
				}
				return flag;
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}

		private void QuantityUpdated(Entity buildingEntity, bool updateAll = false)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (!m_SubObjects.TryGetBuffer(buildingEntity, ref val))
			{
				return;
			}
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				bool updateAll2 = false;
				if (updateAll || m_QuantityData.HasComponent(subObject))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(subObject, default(BatchesUpdated));
					updateAll2 = true;
				}
				QuantityUpdated(subObject, updateAll2);
			}
		}

		private static UpkeepModifierData GetUpkeepModifier(Resource resource, NativeList<UpkeepModifierData> upkeepModifiers)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<UpkeepModifierData> enumerator = upkeepModifiers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					UpkeepModifierData current = enumerator.Current;
					if (current.m_Resource == resource)
					{
						return current;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			return new UpkeepModifierData
			{
				m_Resource = resource,
				m_Multiplier = 1f
			};
		}

		private void UpdateNotification(NativeParallelHashMap<Entity, bool> notifications, Entity prefab, bool wasEmpty, bool isEmpty)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			ResourceConsumerData resourceConsumerData = default(ResourceConsumerData);
			if (m_ResourceConsumerDatas.TryGetComponent(prefab, ref resourceConsumerData) && resourceConsumerData.m_NoResourceNotificationPrefab != Entity.Null && wasEmpty != isEmpty && (!isEmpty || !notifications.ContainsKey(resourceConsumerData.m_NoResourceNotificationPrefab)))
			{
				notifications[resourceConsumerData.m_NoResourceNotificationPrefab] = isEmpty;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		public ComponentTypeHandle<Game.Buildings.ResourceConsumer> __Game_Buildings_ResourceConsumer_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> __Game_Prefabs_ServiceObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> __Game_Prefabs_ServiceUpkeepData_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<UpkeepModifierData> __Game_Prefabs_UpkeepModifierData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ResourceConsumerData> __Game_Prefabs_ResourceConsumerData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceUsage> __Game_Buildings_ServiceUsage_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceBudgetData> __Game_Simulation_ServiceBudgetData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Quantity> __Game_Objects_Quantity_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		public ComponentLookup<PlayerMoney> __Game_City_PlayerMoney_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<OwnedVehicle>(true);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Buildings_ResourceConsumer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.ResourceConsumer>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ServiceObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceObjectData>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceUpkeepData>(true);
			__Game_Prefabs_UpkeepModifierData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<UpkeepModifierData>(true);
			__Game_Prefabs_ResourceConsumerData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceConsumerData>(true);
			__Game_Buildings_ServiceUsage_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceUsage>(true);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
			__Game_Simulation_ServiceBudgetData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceBudgetData>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Objects_Quantity_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Quantity>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_City_PlayerMoney_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlayerMoney>(false);
		}
	}

	private static readonly int kUpdatesPerDay = 64;

	private CitySystem m_CitySystem;

	private SimulationSystem m_SimulationSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private ResourceSystem m_ResourceSystem;

	private IconCommandSystem m_IconCommandSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private VehicleCapacitySystem m_VehicleCapacitySystem;

	private EntityQuery m_UpkeepGroup;

	private EntityQuery m_BudgetDataQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_VehicleCapacitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehicleCapacitySystem>();
		m_UpkeepGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<CityServiceUpkeep>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Temp>()
		});
		m_BudgetDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceBudgetData>() });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		CityServiceUpkeepJob cityServiceUpkeepJob = new CityServiceUpkeepJob
		{
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicleBufType = InternalCompilerInterface.GetBufferTypeHandle<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcesType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceConsumerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.ResourceConsumer>(ref __TypeHandle.__Game_Buildings_ResourceConsumer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceObjects = InternalCompilerInterface.GetComponentLookup<ServiceObjectData>(ref __TypeHandle.__Game_Prefabs_ServiceObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpkeepDatas = InternalCompilerInterface.GetBufferLookup<ServiceUpkeepData>(ref __TypeHandle.__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpkeepModifiers = InternalCompilerInterface.GetBufferLookup<UpkeepModifierData>(ref __TypeHandle.__Game_Prefabs_UpkeepModifierData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceConsumerDatas = InternalCompilerInterface.GetComponentLookup<ResourceConsumerData>(ref __TypeHandle.__Game_Prefabs_ResourceConsumerData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUsages = InternalCompilerInterface.GetComponentLookup<ServiceUsage>(ref __TypeHandle.__Game_Buildings_ServiceUsage_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Limits = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceBudgetDatas = InternalCompilerInterface.GetBufferLookup<ServiceBudgetData>(ref __TypeHandle.__Game_Simulation_ServiceBudgetData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeliveryTrucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_QuantityData = InternalCompilerInterface.GetComponentLookup<Quantity>(ref __TypeHandle.__Game_Objects_Quantity_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlayerMoney = InternalCompilerInterface.GetComponentLookup<PlayerMoney>(ref __TypeHandle.__Game_City_PlayerMoney_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameIndex = updateFrame,
			m_City = m_CitySystem.City,
			m_BudgetDataEntity = ((EntityQuery)(ref m_BudgetDataQuery)).GetSingletonEntity(),
			m_RandomSeed = RandomSeed.Next(),
			m_DeliveryTruckSelectData = m_VehicleCapacitySystem.GetDeliveryTruckSelectData(),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer(),
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<CityServiceUpkeepJob>(cityServiceUpkeepJob, m_UpkeepGroup, ((SystemBase)this).Dependency);
		m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_IconCommandSystem.AddCommandBufferWriter(((SystemBase)this).Dependency);
	}

	public static byte GetResourceAvailability(NativeList<ServiceUpkeepData> upkeeps, DynamicBuffer<Resources> resources, NativeArray<int> storageTargets)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		byte b = byte.MaxValue;
		Enumerator<ServiceUpkeepData> enumerator = upkeeps.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Resource resource = enumerator.Current.m_Upkeep.m_Resource;
				int num = storageTargets[EconomyUtils.GetResourceIndex(resource)];
				if (num > 0)
				{
					int resources2 = EconomyUtils.GetResources(resource, resources);
					byte b2 = (byte)math.clamp(math.ceil(255f * (float)resources2 / (float)num), 0f, 255f);
					if (b2 < b)
					{
						b = b2;
					}
				}
			}
			return b;
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	public static int CalculateUpkeep(int amount, Entity prefabEntity, Entity budgetEntity, EntityManager entityManager)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Entity val = Entity.Null;
		ServiceObjectData serviceObjectData = default(ServiceObjectData);
		if (EntitiesExtensions.TryGetComponent<ServiceObjectData>(entityManager, prefabEntity, ref serviceObjectData))
		{
			val = serviceObjectData.m_Service;
		}
		int num = 100;
		DynamicBuffer<ServiceBudgetData> val2 = default(DynamicBuffer<ServiceBudgetData>);
		if (EntitiesExtensions.TryGetBuffer<ServiceBudgetData>(entityManager, budgetEntity, true, ref val2))
		{
			for (int i = 0; i < val2.Length; i++)
			{
				if (val2[i].m_Service == val)
				{
					num = val2[i].m_Budget;
				}
			}
		}
		return (int)math.round((float)amount * ((float)num / 100f));
	}

	public static void GetUpkeepModifierData(NativeList<UpkeepModifierData> upkeepModifierList, BufferLookup<InstalledUpgrade> installedUpgrades, ComponentLookup<PrefabRef> prefabs, BufferLookup<UpkeepModifierData> upkeepModifiers, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
		if (installedUpgrades.TryGetBuffer(entity, ref upgrades))
		{
			UpgradeUtils.CombineStats<UpkeepModifierData>(upkeepModifierList, upgrades, ref prefabs, ref upkeepModifiers);
		}
	}

	public static bool IsMaterialResource(ComponentLookup<ResourceData> resourceDatas, ResourcePrefabs resourcePrefabs, ResourceStack upkeep)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return resourceDatas[resourcePrefabs[upkeep.m_Resource]].m_Weight > 0f;
	}

	public static int GetUpkeepOfEmployeeWage(BufferLookup<Employee> employeeBufs, Entity entity, EconomyParameterData economyParameterData, bool mainBuildingDisabled)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (mainBuildingDisabled)
		{
			return 0;
		}
		int num = 0;
		DynamicBuffer<Employee> val = default(DynamicBuffer<Employee>);
		if (employeeBufs.TryGetBuffer(entity, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				num += economyParameterData.GetWage(val[i].m_Level, cityServiceJob: true);
			}
		}
		return num;
	}

	public static void GetUpkeepWithUsageScale(NativeList<ServiceUpkeepData> totalUpkeepDatas, BufferLookup<ServiceUpkeepData> serviceUpkeepDatas, BufferLookup<InstalledUpgrade> installedUpgradeBufs, ComponentLookup<PrefabRef> prefabRefs, ComponentLookup<ServiceUsage> serviceUsages, Entity entity, Entity prefab, bool mainBuildingDisabled)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<ServiceUpkeepData> val = default(DynamicBuffer<ServiceUpkeepData>);
		if (serviceUpkeepDatas.TryGetBuffer(prefab, ref val))
		{
			Enumerator<ServiceUpkeepData> enumerator = val.GetEnumerator();
			try
			{
				ServiceUsage serviceUsage = default(ServiceUsage);
				while (enumerator.MoveNext())
				{
					ServiceUpkeepData current = enumerator.Current;
					if (current.m_ScaleWithUsage && serviceUsages.TryGetComponent(entity, ref serviceUsage))
					{
						ServiceUpkeepData serviceUpkeepData = current.ApplyServiceUsage(serviceUsage.m_Usage);
						totalUpkeepDatas.Add(ref serviceUpkeepData);
					}
					else
					{
						totalUpkeepDatas.Add(ref current);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
		DynamicBuffer<InstalledUpgrade> val2 = default(DynamicBuffer<InstalledUpgrade>);
		if (!installedUpgradeBufs.TryGetBuffer(entity, ref val2))
		{
			return;
		}
		Enumerator<InstalledUpgrade> enumerator2 = val2.GetEnumerator();
		try
		{
			PrefabRef prefabRef = default(PrefabRef);
			DynamicBuffer<ServiceUpkeepData> val3 = default(DynamicBuffer<ServiceUpkeepData>);
			ServiceUsage serviceUsage2 = default(ServiceUsage);
			while (enumerator2.MoveNext())
			{
				InstalledUpgrade current2 = enumerator2.Current;
				bool flag = BuildingUtils.CheckOption(current2, BuildingOption.Inactive);
				if (!prefabRefs.TryGetComponent(current2.m_Upgrade, ref prefabRef) || !serviceUpkeepDatas.TryGetBuffer(prefabRef.m_Prefab, ref val3))
				{
					continue;
				}
				for (int i = 0; i < val3.Length; i++)
				{
					ServiceUpkeepData combineData = val3[i];
					if (combineData.m_Upkeep.m_Resource == Resource.Money)
					{
						if (!mainBuildingDisabled && flag)
						{
							combineData.m_Upkeep.m_Amount = (combineData.m_Upkeep.m_Amount + 5) / 10;
						}
					}
					else if (flag)
					{
						continue;
					}
					if (combineData.m_ScaleWithUsage && serviceUsages.TryGetComponent(current2.m_Upgrade, ref serviceUsage2))
					{
						UpgradeUtils.CombineStats<ServiceUpkeepData>(totalUpkeepDatas, combineData.ApplyServiceUsage(serviceUsage2.m_Usage));
					}
					else
					{
						UpgradeUtils.CombineStats<ServiceUpkeepData>(totalUpkeepDatas, combineData);
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
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
	public CityServiceUpkeepSystem()
	{
	}
}

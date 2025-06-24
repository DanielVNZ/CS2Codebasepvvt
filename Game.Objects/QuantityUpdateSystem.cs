using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Economy;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Objects;

[CompilerGenerated]
public class QuantityUpdateSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateQuantityJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Quantity> m_QuantityType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTruckData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.WorkVehicle> m_WorkVehicleData;

		[ReadOnly]
		public ComponentLookup<Creature> m_CreatureData;

		[ReadOnly]
		public ComponentLookup<MailProducer> m_MailProducerData;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> m_GarbageProducerData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.GarbageFacility> m_GarbageFacilityData;

		[ReadOnly]
		public ComponentLookup<IndustrialProperty> m_IndustrialPropertyData;

		[ReadOnly]
		public ComponentLookup<CityServiceUpkeep> m_CityServiceUpkeepData;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_StorageLimitData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<QuantityObjectData> m_PrefabQuantityObjectData;

		[ReadOnly]
		public ComponentLookup<DeliveryTruckData> m_PrefabDeliveryTruckData;

		[ReadOnly]
		public ComponentLookup<WorkVehicleData> m_PrefabWorkVehicleData;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> m_PrefabCargoTransportVehicleData;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_PrefabStorageCompanyData;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_PrefabSpawnableBuildingData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<GarbageFacilityData> m_PrefabGarbageFacilityData;

		[ReadOnly]
		public BufferLookup<Resources> m_EconomyResources;

		[ReadOnly]
		public BufferLookup<Renter> m_Renters;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> m_PrefabServiceUpkeepDatas;

		[ReadOnly]
		public PostConfigurationData m_PostConfigurationData;

		[ReadOnly]
		public GarbageParameterData m_GarbageConfigurationData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0797: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Quantity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Quantity>(ref m_QuantityType);
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			PrefabRef prefabRef2 = default(PrefabRef);
			DynamicBuffer<Renter> val2 = default(DynamicBuffer<Renter>);
			SpawnableBuildingData spawnable = default(SpawnableBuildingData);
			BuildingData building = default(BuildingData);
			PrefabRef prefabRef3 = default(PrefabRef);
			DynamicBuffer<Resources> val3 = default(DynamicBuffer<Resources>);
			StorageCompanyData storageCompanyData = default(StorageCompanyData);
			StorageLimitData storageLimitData = default(StorageLimitData);
			PrefabRef prefabRef4 = default(PrefabRef);
			DynamicBuffer<Resources> val4 = default(DynamicBuffer<Resources>);
			DynamicBuffer<ServiceUpkeepData> val5 = default(DynamicBuffer<ServiceUpkeepData>);
			StorageCompanyData storageCompanyData2 = default(StorageCompanyData);
			StorageLimitData data = default(StorageLimitData);
			DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
			GarbageFacilityData garbageFacilityData = default(GarbageFacilityData);
			Game.Vehicles.DeliveryTruck deliveryTruck = default(Game.Vehicles.DeliveryTruck);
			MailProducer mailProducer = default(MailProducer);
			GarbageProducer garbageProducer = default(GarbageProducer);
			CargoTransportVehicleData cargoTransportVehicleData = default(CargoTransportVehicleData);
			Game.Vehicles.WorkVehicle workVehicle = default(Game.Vehicles.WorkVehicle);
			Game.Vehicles.WorkVehicle workVehicle2 = default(Game.Vehicles.WorkVehicle);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity entity = nativeArray[i];
				Entity val = ((nativeArray3.Length == 0 || !(nativeArray3[i].m_Original != Entity.Null)) ? GetOwner(entity) : GetOwner(nativeArray3[i].m_Original));
				PrefabRef prefabRef = nativeArray4[i];
				QuantityObjectData quantityObjectData = m_PrefabQuantityObjectData[prefabRef.m_Prefab];
				Quantity quantity = default(Quantity);
				if (quantityObjectData.m_Resources != Resource.NoResource && m_IndustrialPropertyData.HasComponent(val) && m_PrefabRefData.TryGetComponent(val, ref prefabRef2) && m_Renters.TryGetBuffer(val, ref val2) && m_PrefabSpawnableBuildingData.TryGetComponent(prefabRef2.m_Prefab, ref spawnable) && m_PrefabBuildingData.TryGetComponent(prefabRef2.m_Prefab, ref building))
				{
					int num = 0;
					int num2 = 0;
					bool flag = false;
					for (int j = 0; j < val2.Length; j++)
					{
						Entity renter = val2[j].m_Renter;
						if (!m_PrefabRefData.TryGetComponent(renter, ref prefabRef3) || !m_EconomyResources.TryGetBuffer(renter, ref val3) || !m_PrefabStorageCompanyData.TryGetComponent(prefabRef3.m_Prefab, ref storageCompanyData) || !m_StorageLimitData.TryGetComponent(prefabRef3.m_Prefab, ref storageLimitData))
						{
							continue;
						}
						Resource resource = quantityObjectData.m_Resources & storageCompanyData.m_StoredResources;
						if (resource != Resource.NoResource)
						{
							num2 += storageLimitData.GetAdjustedLimitForWarehouse(spawnable, building);
							for (int k = 0; k < val3.Length; k++)
							{
								Resources resources = val3[k];
								num += math.select(0, resources.m_Amount, (resources.m_Resource & resource) != 0);
							}
							flag = true;
						}
					}
					if (flag)
					{
						float num3 = (float)num / (float)math.max(1, num2);
						quantity.m_Fullness = (byte)math.clamp(Mathf.RoundToInt(num3 * 100f), 0, 255);
						quantityObjectData.m_Resources = Resource.NoResource;
					}
				}
				if (quantityObjectData.m_Resources != Resource.NoResource && m_CityServiceUpkeepData.HasComponent(val) && m_PrefabRefData.TryGetComponent(val, ref prefabRef4) && m_EconomyResources.TryGetBuffer(val, ref val4))
				{
					Resource resource2 = Resource.NoResource;
					if (m_PrefabServiceUpkeepDatas.TryGetBuffer(prefabRef4.m_Prefab, ref val5))
					{
						for (int l = 0; l < val5.Length; l++)
						{
							resource2 |= val5[l].m_Upkeep.m_Resource;
						}
					}
					if (m_PrefabStorageCompanyData.TryGetComponent(prefabRef4.m_Prefab, ref storageCompanyData2))
					{
						resource2 |= storageCompanyData2.m_StoredResources;
					}
					if ((quantityObjectData.m_Resources & Resource.Garbage) != Resource.NoResource && m_GarbageFacilityData.HasComponent(val))
					{
						resource2 |= Resource.Garbage;
					}
					resource2 &= quantityObjectData.m_Resources;
					if (resource2 != Resource.NoResource)
					{
						int num4 = 0;
						int num5 = 0;
						if (m_StorageLimitData.TryGetComponent(prefabRef4.m_Prefab, ref data))
						{
							if (m_InstalledUpgrades.TryGetBuffer(val, ref upgrades))
							{
								UpgradeUtils.CombineStats<StorageLimitData>(ref data, upgrades, ref m_PrefabRefData, ref m_StorageLimitData);
							}
							num5 += data.m_Limit;
						}
						if ((quantityObjectData.m_Resources & Resource.Garbage) != Resource.NoResource && m_PrefabGarbageFacilityData.TryGetComponent(prefabRef4.m_Prefab, ref garbageFacilityData))
						{
							num5 += garbageFacilityData.m_GarbageCapacity;
						}
						for (int m = 0; m < val4.Length; m++)
						{
							Resources resources2 = val4[m];
							num4 += math.select(0, resources2.m_Amount, (resources2.m_Resource & resource2) != 0);
						}
						float num6 = (float)num4 / (float)math.max(1, num5);
						quantity.m_Fullness = (byte)math.clamp(Mathf.RoundToInt(num6 * 100f), 0, 255);
						quantityObjectData.m_Resources = Resource.NoResource;
					}
				}
				if (quantityObjectData.m_Resources != Resource.NoResource && m_DeliveryTruckData.TryGetComponent(val, ref deliveryTruck) && (deliveryTruck.m_State & DeliveryTruckFlags.Loaded) != 0 && (deliveryTruck.m_Resource & quantityObjectData.m_Resources) != Resource.NoResource)
				{
					PrefabRef prefabRef5 = m_PrefabRefData[val];
					DeliveryTruckData deliveryTruckData = m_PrefabDeliveryTruckData[prefabRef5.m_Prefab];
					float num7 = (float)deliveryTruck.m_Amount / (float)math.max(1, deliveryTruckData.m_CargoCapacity);
					quantity.m_Fullness = (byte)math.clamp(Mathf.RoundToInt(num7 * 100f), 0, 255);
					quantityObjectData.m_Resources = Resource.NoResource;
				}
				if ((quantityObjectData.m_Resources & Resource.LocalMail) != Resource.NoResource && m_MailProducerData.TryGetComponent(val, ref mailProducer))
				{
					quantity.m_Fullness = (byte)math.select(100, 0, !mailProducer.mailDelivered || mailProducer.receivingMail >= m_PostConfigurationData.m_MailAccumulationTolerance);
					quantityObjectData.m_Resources = Resource.NoResource;
				}
				if ((quantityObjectData.m_Resources & Resource.Garbage) != Resource.NoResource && m_GarbageProducerData.TryGetComponent(val, ref garbageProducer))
				{
					int num8 = math.select(0, 100, garbageProducer.m_Garbage >= m_GarbageConfigurationData.m_RequestGarbageLimit);
					num8 = math.select(num8, 255, garbageProducer.m_Garbage >= m_GarbageConfigurationData.m_WarningGarbageLimit);
					quantity.m_Fullness = (byte)num8;
					quantityObjectData.m_Resources = Resource.NoResource;
				}
				if (quantityObjectData.m_Resources != Resource.NoResource && m_EconomyResources.HasBuffer(val))
				{
					PrefabRef prefabRef6 = m_PrefabRefData[val];
					Resource resource3 = quantityObjectData.m_Resources;
					int num9 = 0;
					if (m_PrefabCargoTransportVehicleData.TryGetComponent(prefabRef6.m_Prefab, ref cargoTransportVehicleData))
					{
						resource3 &= cargoTransportVehicleData.m_Resources;
						num9 = cargoTransportVehicleData.m_CargoCapacity;
					}
					if (resource3 != Resource.NoResource)
					{
						DynamicBuffer<Resources> val6 = m_EconomyResources[val];
						int num10 = 0;
						for (int n = 0; n < val6.Length; n++)
						{
							Resources resources3 = val6[n];
							num10 += math.select(0, resources3.m_Amount, (resources3.m_Resource & resource3) != 0);
						}
						float num11 = (float)num10 / math.max(1f, (float)num9);
						quantity.m_Fullness = (byte)math.clamp(Mathf.RoundToInt(num11 * 100f), 0, 255);
						quantityObjectData.m_Resources = Resource.NoResource;
					}
				}
				if (quantityObjectData.m_MapFeature != MapFeature.None && m_WorkVehicleData.TryGetComponent(val, ref workVehicle) && workVehicle.m_DoneAmount != 0f)
				{
					PrefabRef prefabRef7 = m_PrefabRefData[val];
					WorkVehicleData workVehicleData = m_PrefabWorkVehicleData[prefabRef7.m_Prefab];
					if (workVehicleData.m_MapFeature == quantityObjectData.m_MapFeature)
					{
						float num12 = workVehicle.m_DoneAmount / math.max(1f, workVehicleData.m_MaxWorkAmount);
						quantity.m_Fullness = (byte)math.clamp(Mathf.RoundToInt(num12 * 100f), 0, 255);
						quantityObjectData.m_MapFeature = MapFeature.None;
					}
				}
				if (quantityObjectData.m_Resources != Resource.NoResource && m_WorkVehicleData.TryGetComponent(val, ref workVehicle2) && workVehicle2.m_DoneAmount != 0f)
				{
					PrefabRef prefabRef8 = m_PrefabRefData[val];
					WorkVehicleData workVehicleData2 = m_PrefabWorkVehicleData[prefabRef8.m_Prefab];
					if ((workVehicleData2.m_Resources & quantityObjectData.m_Resources) != Resource.NoResource)
					{
						float num13 = workVehicle2.m_DoneAmount / math.max(1f, workVehicleData2.m_MaxWorkAmount);
						quantity.m_Fullness = (byte)math.clamp(Mathf.RoundToInt(num13 * 100f), 0, 255);
						quantityObjectData.m_Resources = Resource.NoResource;
					}
				}
				nativeArray2[i] = quantity;
			}
		}

		public Entity GetOwner(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			Entity val = entity;
			Owner owner = default(Owner);
			while (m_OwnerData.TryGetComponent(val, ref owner) && !m_VehicleData.HasComponent(val) && !m_CreatureData.HasComponent(val))
			{
				val = owner.m_Owner;
			}
			return val;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Quantity> __Game_Objects_Quantity_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.WorkVehicle> __Game_Vehicles_WorkVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Creature> __Game_Creatures_Creature_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MailProducer> __Game_Buildings_MailProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> __Game_Buildings_GarbageProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.GarbageFacility> __Game_Buildings_GarbageFacility_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProperty> __Game_Buildings_IndustrialProperty_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CityServiceUpkeep> __Game_City_CityServiceUpkeep_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<QuantityObjectData> __Game_Prefabs_QuantityObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DeliveryTruckData> __Game_Prefabs_DeliveryTruckData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkVehicleData> __Game_Prefabs_WorkVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> __Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> __Game_Prefabs_StorageCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageFacilityData> __Game_Prefabs_GarbageFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> __Game_Prefabs_ServiceUpkeepData_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_Quantity_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Quantity>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Vehicles_WorkVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.WorkVehicle>(true);
			__Game_Creatures_Creature_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creature>(true);
			__Game_Buildings_MailProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailProducer>(true);
			__Game_Buildings_GarbageProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageProducer>(true);
			__Game_Buildings_GarbageFacility_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.GarbageFacility>(true);
			__Game_Buildings_IndustrialProperty_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProperty>(true);
			__Game_City_CityServiceUpkeep_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CityServiceUpkeep>(true);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_QuantityObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<QuantityObjectData>(true);
			__Game_Prefabs_DeliveryTruckData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DeliveryTruckData>(true);
			__Game_Prefabs_WorkVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkVehicleData>(true);
			__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CargoTransportVehicleData>(true);
			__Game_Prefabs_StorageCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageCompanyData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_GarbageFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageFacilityData>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceUpkeepData>(true);
		}
	}

	private EntityQuery m_QuantityQuery;

	private EntityQuery m_PostConfigurationQuery;

	private EntityQuery m_GarbageConfigurationQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<Quantity>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<BatchesUpdated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>()
		};
		array[0] = val;
		m_QuantityQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_PostConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PostConfigurationData>() });
		m_GarbageConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_QuantityQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependency = JobChunkExtensions.ScheduleParallel<UpdateQuantityJob>(new UpdateQuantityJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_QuantityType = InternalCompilerInterface.GetComponentTypeHandle<Quantity>(ref __TypeHandle.__Game_Objects_Quantity_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeliveryTruckData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkVehicleData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.WorkVehicle>(ref __TypeHandle.__Game_Vehicles_WorkVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureData = InternalCompilerInterface.GetComponentLookup<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailProducerData = InternalCompilerInterface.GetComponentLookup<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageProducerData = InternalCompilerInterface.GetComponentLookup<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageFacilityData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.GarbageFacility>(ref __TypeHandle.__Game_Buildings_GarbageFacility_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IndustrialPropertyData = InternalCompilerInterface.GetComponentLookup<IndustrialProperty>(ref __TypeHandle.__Game_Buildings_IndustrialProperty_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityServiceUpkeepData = InternalCompilerInterface.GetComponentLookup<CityServiceUpkeep>(ref __TypeHandle.__Game_City_CityServiceUpkeep_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageLimitData = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabQuantityObjectData = InternalCompilerInterface.GetComponentLookup<QuantityObjectData>(ref __TypeHandle.__Game_Prefabs_QuantityObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDeliveryTruckData = InternalCompilerInterface.GetComponentLookup<DeliveryTruckData>(ref __TypeHandle.__Game_Prefabs_DeliveryTruckData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWorkVehicleData = InternalCompilerInterface.GetComponentLookup<WorkVehicleData>(ref __TypeHandle.__Game_Prefabs_WorkVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCargoTransportVehicleData = InternalCompilerInterface.GetComponentLookup<CargoTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStorageCompanyData = InternalCompilerInterface.GetComponentLookup<StorageCompanyData>(ref __TypeHandle.__Game_Prefabs_StorageCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnableBuildingData = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGarbageFacilityData = InternalCompilerInterface.GetComponentLookup<GarbageFacilityData>(ref __TypeHandle.__Game_Prefabs_GarbageFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EconomyResources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabServiceUpkeepDatas = InternalCompilerInterface.GetBufferLookup<ServiceUpkeepData>(ref __TypeHandle.__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PostConfigurationData = (((EntityQuery)(ref m_PostConfigurationQuery)).IsEmptyIgnoreFilter ? default(PostConfigurationData) : ((EntityQuery)(ref m_PostConfigurationQuery)).GetSingleton<PostConfigurationData>()),
			m_GarbageConfigurationData = (((EntityQuery)(ref m_GarbageConfigurationQuery)).IsEmptyIgnoreFilter ? default(GarbageParameterData) : ((EntityQuery)(ref m_GarbageConfigurationQuery)).GetSingleton<GarbageParameterData>())
		}, m_QuantityQuery, ((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = dependency;
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
	public QuantityUpdateSystem()
	{
	}
}

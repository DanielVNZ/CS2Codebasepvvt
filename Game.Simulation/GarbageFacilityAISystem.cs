using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Net;
using Game.Notifications;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
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
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class GarbageFacilityAISystem : GameSystemBase
{
	private struct GarbageFacilityAction
	{
		public Entity m_Entity;

		public bool m_Disabled;

		public static GarbageFacilityAction SetDisabled(Entity vehicle, bool disabled)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return new GarbageFacilityAction
			{
				m_Entity = vehicle,
				m_Disabled = disabled
			};
		}
	}

	[BurstCompile]
	private struct GarbageFacilityTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.SubArea> m_SubAreaType;

		public ComponentTypeHandle<Game.Buildings.GarbageFacility> m_GarbageFacilityType;

		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		public BufferTypeHandle<Resources> m_ResourcesType;

		public BufferTypeHandle<OwnedVehicle> m_OwnedVehicleType;

		public BufferTypeHandle<GuestVehicle> m_GuestVehicleType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<GarbageCollectionRequest> m_GarbageCollectionRequestData;

		[ReadOnly]
		public ComponentLookup<GarbageTransferRequest> m_GarbageTransferRequestData;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<Target> m_TargetData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Quantity> m_QuantityData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.GarbageTruck> m_GarbageTruckData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTruckData;

		[ReadOnly]
		public ComponentLookup<ReturnLoad> m_ReturnLoadData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<Geometry> m_AreaGeometryData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<GarbageFacilityData> m_PrefabGarbageFacilityData;

		[ReadOnly]
		public ComponentLookup<GarbageTruckData> m_PrefabGarbageTruckData;

		[ReadOnly]
		public ComponentLookup<StorageAreaData> m_PrefabStorageAreaData;

		[ReadOnly]
		public ComponentLookup<DeliveryTruckData> m_PrefabDeliveryTruckData;

		[ReadOnly]
		public ComponentLookup<ObjectData> m_PrefabObjectData;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public BufferLookup<ResourceProductionData> m_ResourceProductionDatas;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Storage> m_AreaStorageData;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		[ReadOnly]
		public GarbageParameterData m_GarbageParameters;

		[ReadOnly]
		public GarbageTruckSelectData m_GarbageTruckSelectData;

		[ReadOnly]
		public DeliveryTruckSelectData m_DeliveryTruckSelectData;

		[ReadOnly]
		public EntityArchetype m_GarbageTransferRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_GarbageCollectionRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingCarAddTypes;

		public IconCommandBuffer m_IconCommandBuffer;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<GarbageFacilityAction> m_ActionQueue;

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
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Building> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
			BufferAccessor<Efficiency> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Game.Buildings.GarbageFacility> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.GarbageFacility>(ref m_GarbageFacilityType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			BufferAccessor<OwnedVehicle> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedVehicle>(ref m_OwnedVehicleType);
			BufferAccessor<GuestVehicle> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<GuestVehicle>(ref m_GuestVehicleType);
			BufferAccessor<Game.Areas.SubArea> bufferAccessor5 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Areas.SubArea>(ref m_SubAreaType);
			BufferAccessor<ServiceDispatch> bufferAccessor6 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			BufferAccessor<Resources> bufferAccessor7 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourcesType);
			NativeList<ResourceProductionData> resourceProductionBuffer = default(NativeList<ResourceProductionData>);
			bool outside = ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType);
			m_DeliveryTruckSelectData.GetCapacityRange(Resource.Garbage, out var min, out var max);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			int4 val2 = default(int4);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				PrefabRef prefabRef = nativeArray3[i];
				Game.Buildings.GarbageFacility garbageFacility = nativeArray4[i];
				DynamicBuffer<OwnedVehicle> ownedVehicles = bufferAccessor3[i];
				DynamicBuffer<ServiceDispatch> dispatches = bufferAccessor6[i];
				DynamicBuffer<GuestVehicle> guestVehicles = default(DynamicBuffer<GuestVehicle>);
				if (bufferAccessor4.Length != 0)
				{
					guestVehicles = bufferAccessor4[i];
				}
				GarbageFacilityData data = m_PrefabGarbageFacilityData[prefabRef.m_Prefab];
				if (m_ResourceProductionDatas.HasBuffer(prefabRef.m_Prefab))
				{
					AddResourceProductionData(m_ResourceProductionDatas[prefabRef.m_Prefab], ref resourceProductionBuffer);
				}
				if (bufferAccessor2.Length != 0)
				{
					UpgradeUtils.CombineStats<GarbageFacilityData>(ref data, bufferAccessor2[i], ref m_PrefabRefData, ref m_PrefabGarbageFacilityData);
					CombineResourceProductionData(bufferAccessor2[i], ref resourceProductionBuffer);
				}
				int garbageAmount = 0;
				DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
				if (bufferAccessor7.Length != 0)
				{
					resources = bufferAccessor7[i];
					garbageAmount = EconomyUtils.GetResources(Resource.Garbage, resources);
				}
				int num = garbageAmount;
				Building building = default(Building);
				if (nativeArray2.Length != 0)
				{
					building = nativeArray2[i];
				}
				float efficiency = BuildingUtils.GetEfficiency(bufferAccessor, i);
				float immediateEfficiency = BuildingUtils.GetImmediateEfficiency(bufferAccessor, i);
				int areaCapacity = 0;
				int areaGarbage = 0;
				if (bufferAccessor5.Length != 0)
				{
					DynamicBuffer<Game.Areas.SubArea> subAreas = bufferAccessor5[i];
					ProcessAreas(unfilteredChunkIndex, subAreas, ref garbageAmount, data, max, efficiency, out areaCapacity, out areaGarbage);
				}
				Tick(unfilteredChunkIndex, val, building, ref random, ref garbageFacility, ref garbageAmount, data, efficiency, immediateEfficiency, areaCapacity, areaGarbage, min, max, ownedVehicles, guestVehicles, dispatches, resources, resourceProductionBuffer, outside);
				nativeArray4[i] = garbageFacility;
				if (resources.IsCreated)
				{
					EconomyUtils.SetResources(Resource.Garbage, resources, garbageAmount);
					int num2 = Mathf.RoundToInt((float)num / (float)math.max(1, data.m_GarbageCapacity) * 100f);
					int num3 = Mathf.RoundToInt((float)garbageAmount / (float)math.max(1, data.m_GarbageCapacity) * 100f);
					((int4)(ref val2))._002Ector(0, 33, 50, 66);
					if (math.any(num2 > val2 != num3 > val2))
					{
						QuantityUpdated(unfilteredChunkIndex, val);
					}
				}
				if (resourceProductionBuffer.IsCreated)
				{
					resourceProductionBuffer.Clear();
				}
			}
			if (resourceProductionBuffer.IsCreated)
			{
				resourceProductionBuffer.Dispose();
			}
		}

		private void QuantityUpdated(int jobIndex, Entity buildingEntity, bool updateAll = false)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
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
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, subObject, default(BatchesUpdated));
					updateAll2 = true;
				}
				QuantityUpdated(jobIndex, subObject, updateAll2);
			}
		}

		private void AddResourceProductionData(DynamicBuffer<ResourceProductionData> resourceProductionDatas, ref NativeList<ResourceProductionData> resourceProductionBuffer)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!resourceProductionBuffer.IsCreated)
			{
				resourceProductionBuffer = new NativeList<ResourceProductionData>(resourceProductionDatas.Length, AllocatorHandle.op_Implicit((Allocator)2));
			}
			ResourceProductionData.Combine(resourceProductionBuffer, resourceProductionDatas);
		}

		private void CombineResourceProductionData(DynamicBuffer<InstalledUpgrade> upgrades, ref NativeList<ResourceProductionData> resourceProductionBuffer)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ResourceProductionData> resourceProductionDatas = default(DynamicBuffer<ResourceProductionData>);
			for (int i = 0; i < upgrades.Length; i++)
			{
				InstalledUpgrade installedUpgrade = upgrades[i];
				if (!BuildingUtils.CheckOption(installedUpgrade, BuildingOption.Inactive))
				{
					PrefabRef prefabRef = m_PrefabRefData[installedUpgrade.m_Upgrade];
					if (m_ResourceProductionDatas.TryGetBuffer(prefabRef.m_Prefab, ref resourceProductionDatas))
					{
						AddResourceProductionData(resourceProductionDatas, ref resourceProductionBuffer);
					}
				}
			}
		}

		private void ProcessAreas(int jobIndex, DynamicBuffer<Game.Areas.SubArea> subAreas, ref int garbageAmount, GarbageFacilityData prefabGarbageFacilityData, int maxGarbageLoad, float efficiency, out int areaCapacity, out int areaGarbage)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			areaCapacity = 0;
			areaGarbage = 0;
			for (int i = 0; i < subAreas.Length; i++)
			{
				Entity area = subAreas[i].m_Area;
				if (!m_AreaStorageData.HasComponent(area))
				{
					continue;
				}
				PrefabRef prefabRef = m_PrefabRefData[area];
				Storage storage = m_AreaStorageData[area];
				Geometry geometry = m_AreaGeometryData[area];
				StorageAreaData prefabStorageData = m_PrefabStorageAreaData[prefabRef.m_Prefab];
				int num = AreaUtils.CalculateStorageCapacity(geometry, prefabStorageData);
				int num2 = (int)((long)storage.m_Amount * (long)prefabGarbageFacilityData.m_GarbageCapacity / (num * 2));
				float num3 = 0.0009765625f;
				float num4 = (float)prefabGarbageFacilityData.m_ProcessingSpeed * num3;
				num2 += maxGarbageLoad + Mathf.CeilToInt(num4);
				int num5 = math.min(garbageAmount - num2, num - storage.m_Amount);
				num5 = math.max(num5, -math.min(storage.m_Amount, prefabGarbageFacilityData.m_GarbageCapacity - garbageAmount));
				int num6 = Mathf.CeilToInt((float)math.abs(num5) * math.saturate(efficiency));
				num5 = math.select(num6, -num6, num5 < 0);
				if (num5 != 0)
				{
					int num7 = (int)((long)storage.m_Amount * 100L / num);
					storage.m_Amount += num5;
					storage.m_WorkAmount += num6;
					garbageAmount -= num5;
					m_AreaStorageData[area] = storage;
					if ((int)((long)storage.m_Amount * 100L / num) != num7)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, area, default(Updated));
					}
				}
				areaCapacity += num;
				areaGarbage += storage.m_Amount;
			}
		}

		private unsafe void Tick(int jobIndex, Entity entity, Building building, ref Random random, ref Game.Buildings.GarbageFacility garbageFacility, ref int garbageAmount, GarbageFacilityData prefabGarbageFacilityData, float efficiency, float immediateEfficiency, int areaCapacity, int areaGarbage, int minGarbageLoad, int maxGarbageLoad, DynamicBuffer<OwnedVehicle> ownedVehicles, DynamicBuffer<GuestVehicle> guestVehicles, DynamicBuffer<ServiceDispatch> dispatches, DynamicBuffer<Resources> resources, NativeList<ResourceProductionData> resourceProductionBuffer, bool outside)
		{
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0727: Unknown result type (might be due to invalid IL or missing references)
			//IL_0700: Unknown result type (might be due to invalid IL or missing references)
			//IL_0701: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0539: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
			//IL_098d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_091c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0921: Unknown result type (might be due to invalid IL or missing references)
			//IL_0926: Unknown result type (might be due to invalid IL or missing references)
			//IL_092f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0931: Unknown result type (might be due to invalid IL or missing references)
			//IL_094e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08de: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b53: Unknown result type (might be due to invalid IL or missing references)
			if (outside)
			{
				int num = prefabGarbageFacilityData.m_GarbageCapacity / 2 - garbageAmount;
				if (num != 0)
				{
					garbageAmount += num;
				}
			}
			float num2 = math.min((float)garbageAmount, (float)prefabGarbageFacilityData.m_ProcessingSpeed / 1024f);
			float num3 = CalculateProcessingRate(num2, efficiency, garbageAmount, prefabGarbageFacilityData.m_GarbageCapacity);
			float num4 = ((num2 > 0f) ? (num3 / num2) : 0f);
			if (resourceProductionBuffer.IsCreated)
			{
				for (int i = 0; i < resourceProductionBuffer.Length; i++)
				{
					ResourceProductionData resourceProductionData = resourceProductionBuffer[i];
					float num5 = (float)resourceProductionData.m_ProductionRate / 1024f;
					if (num5 > 0f)
					{
						int resources2 = EconomyUtils.GetResources(resourceProductionData.m_Type, resources);
						float num6 = math.clamp((float)(resourceProductionData.m_StorageCapacity - resources2), 0f, num5);
						num4 = math.min(num4, num6 / num5);
					}
				}
				for (int j = 0; j < resourceProductionBuffer.Length; j++)
				{
					ResourceProductionData resourceProductionData2 = resourceProductionBuffer[j];
					float num7 = (float)resourceProductionData2.m_ProductionRate / 1024f;
					EconomyUtils.AddResources(amount: MathUtils.RoundToIntRandom(ref random, num4 * num7), resource: resourceProductionData2.m_Type, resources: resources);
				}
			}
			num3 = num4 * num2;
			garbageFacility.m_ProcessingRate = Mathf.RoundToInt(num3 * 1024f);
			int num8 = MathUtils.RoundToIntRandom(ref random, num3);
			garbageAmount -= num8;
			int vehicleCapacity = BuildingUtils.GetVehicleCapacity(efficiency, prefabGarbageFacilityData.m_VehicleCapacity);
			int num9 = BuildingUtils.GetVehicleCapacity(immediateEfficiency, prefabGarbageFacilityData.m_VehicleCapacity);
			int availableVehicles = vehicleCapacity;
			int availableDeliveryTrucks = prefabGarbageFacilityData.m_TransportCapacity;
			int availableSpace = prefabGarbageFacilityData.m_GarbageCapacity - garbageAmount + areaCapacity - areaGarbage;
			int availableGarbage = garbageAmount + areaGarbage - num8 * 2;
			int length = ownedVehicles.Length;
			StackList<Entity> parkedVehicles = StackList<Entity>.op_Implicit(new Span<Entity>((void*)stackalloc Entity[length], length));
			Game.Vehicles.GarbageTruck garbageTruck = default(Game.Vehicles.GarbageTruck);
			ParkedCar parkedCar = default(ParkedCar);
			Game.Vehicles.DeliveryTruck deliveryTruck = default(Game.Vehicles.DeliveryTruck);
			for (int k = 0; k < ownedVehicles.Length; k++)
			{
				Entity vehicle = ownedVehicles[k].m_Vehicle;
				if (m_GarbageTruckData.TryGetComponent(vehicle, ref garbageTruck))
				{
					if (m_ParkedCarData.TryGetComponent(vehicle, ref parkedCar))
					{
						if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(parkedCar.m_Lane))
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicle);
						}
						else
						{
							parkedVehicles.AddNoResize(vehicle);
						}
						continue;
					}
					PrefabRef prefabRef = m_PrefabRefData[vehicle];
					GarbageTruckData garbageTruckData = m_PrefabGarbageTruckData[prefabRef.m_Prefab];
					availableVehicles--;
					availableSpace -= garbageTruckData.m_GarbageCapacity;
					bool flag = --num9 < 0;
					if ((garbageTruck.m_State & GarbageTruckFlags.Disabled) != 0 != flag)
					{
						m_ActionQueue.Enqueue(GarbageFacilityAction.SetDisabled(vehicle, flag));
					}
				}
				else if (m_DeliveryTruckData.TryGetComponent(vehicle, ref deliveryTruck))
				{
					if ((deliveryTruck.m_State & DeliveryTruckFlags.DummyTraffic) != 0)
					{
						continue;
					}
					DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
					if (m_LayoutElements.HasBuffer(vehicle))
					{
						val = m_LayoutElements[vehicle];
					}
					if (val.IsCreated && val.Length != 0)
					{
						for (int l = 0; l < val.Length; l++)
						{
							Entity vehicle2 = val[l].m_Vehicle;
							if (!m_DeliveryTruckData.HasComponent(vehicle2))
							{
								continue;
							}
							Game.Vehicles.DeliveryTruck deliveryTruck2 = m_DeliveryTruckData[vehicle2];
							if ((deliveryTruck2.m_Resource & Resource.Garbage) != Resource.NoResource && (deliveryTruck.m_State & DeliveryTruckFlags.Buying) != 0)
							{
								availableSpace -= deliveryTruck2.m_Amount;
							}
							if (m_ReturnLoadData.HasComponent(vehicle2))
							{
								ReturnLoad returnLoad = m_ReturnLoadData[vehicle2];
								if ((returnLoad.m_Resource & Resource.Garbage) != Resource.NoResource)
								{
									availableSpace -= returnLoad.m_Amount;
								}
							}
						}
					}
					else
					{
						if ((deliveryTruck.m_Resource & Resource.Garbage) != Resource.NoResource && (deliveryTruck.m_State & DeliveryTruckFlags.Buying) != 0)
						{
							availableSpace -= deliveryTruck.m_Amount;
						}
						if (m_ReturnLoadData.HasComponent(vehicle))
						{
							ReturnLoad returnLoad2 = m_ReturnLoadData[vehicle];
							if ((returnLoad2.m_Resource & Resource.Garbage) != Resource.NoResource)
							{
								availableSpace -= returnLoad2.m_Amount;
							}
						}
					}
					availableDeliveryTrucks--;
				}
				else if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(vehicle))
				{
					ownedVehicles.RemoveAt(k--);
				}
			}
			if (guestVehicles.IsCreated)
			{
				for (int m = 0; m < guestVehicles.Length; m++)
				{
					Entity vehicle3 = guestVehicles[m].m_Vehicle;
					if (!m_TargetData.HasComponent(vehicle3) || m_TargetData[vehicle3].m_Target != entity)
					{
						guestVehicles.RemoveAt(m--);
					}
					else
					{
						if (!m_DeliveryTruckData.HasComponent(vehicle3))
						{
							continue;
						}
						Game.Vehicles.DeliveryTruck deliveryTruck3 = m_DeliveryTruckData[vehicle3];
						if ((deliveryTruck3.m_State & DeliveryTruckFlags.DummyTraffic) != 0)
						{
							continue;
						}
						DynamicBuffer<LayoutElement> val2 = default(DynamicBuffer<LayoutElement>);
						if (m_LayoutElements.HasBuffer(vehicle3))
						{
							val2 = m_LayoutElements[vehicle3];
						}
						if (val2.IsCreated && val2.Length != 0)
						{
							for (int n = 0; n < val2.Length; n++)
							{
								Entity vehicle4 = val2[n].m_Vehicle;
								if (!m_DeliveryTruckData.HasComponent(vehicle4))
								{
									continue;
								}
								Game.Vehicles.DeliveryTruck deliveryTruck4 = m_DeliveryTruckData[vehicle4];
								if ((deliveryTruck3.m_State & DeliveryTruckFlags.Buying) != 0)
								{
									if ((deliveryTruck4.m_Resource & Resource.Garbage) != Resource.NoResource)
									{
										availableGarbage -= deliveryTruck4.m_Amount;
									}
								}
								else if ((deliveryTruck4.m_Resource & Resource.Garbage) != Resource.NoResource)
								{
									availableSpace -= deliveryTruck4.m_Amount;
								}
								if (m_ReturnLoadData.HasComponent(vehicle4))
								{
									ReturnLoad returnLoad3 = m_ReturnLoadData[vehicle4];
									if ((returnLoad3.m_Resource & Resource.Garbage) != Resource.NoResource)
									{
										availableGarbage -= returnLoad3.m_Amount;
									}
								}
							}
							continue;
						}
						if ((deliveryTruck3.m_State & DeliveryTruckFlags.Buying) != 0)
						{
							if ((deliveryTruck3.m_Resource & Resource.Garbage) != Resource.NoResource)
							{
								availableGarbage -= deliveryTruck3.m_Amount;
							}
						}
						else if ((deliveryTruck3.m_Resource & Resource.Garbage) != Resource.NoResource)
						{
							availableSpace -= deliveryTruck3.m_Amount;
						}
						if (m_ReturnLoadData.HasComponent(vehicle3))
						{
							ReturnLoad returnLoad4 = m_ReturnLoadData[vehicle3];
							if ((returnLoad4.m_Resource & Resource.Garbage) != Resource.NoResource)
							{
								availableGarbage -= returnLoad4.m_Amount;
							}
						}
					}
				}
			}
			if (BuildingUtils.CheckOption(building, BuildingOption.Empty))
			{
				availableSpace = 0;
			}
			for (int num10 = 0; num10 < dispatches.Length; num10++)
			{
				Entity request = dispatches[num10].m_Request;
				if (m_GarbageCollectionRequestData.HasComponent(request))
				{
					TrySpawnGarbageTruck(jobIndex, ref random, entity, request, prefabGarbageFacilityData, ref garbageFacility, ref availableVehicles, ref availableSpace, ref parkedVehicles);
					dispatches.RemoveAt(num10--);
				}
				else if (m_GarbageTransferRequestData.HasComponent(request))
				{
					TrySpawnDeliveryTruck(jobIndex, ref random, entity, request, ref availableDeliveryTrucks, ref availableSpace, ref availableGarbage, ref garbageAmount);
					dispatches.RemoveAt(num10--);
				}
				else if (!m_ServiceRequestData.HasComponent(request))
				{
					dispatches.RemoveAt(num10--);
				}
			}
			while (parkedVehicles.Length > math.max(0, prefabGarbageFacilityData.m_VehicleCapacity + availableVehicles - vehicleCapacity))
			{
				int num11 = ((Random)(ref random)).NextInt(parkedVehicles.Length);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, parkedVehicles[num11]);
				parkedVehicles.RemoveAtSwapBack(num11);
			}
			for (int num12 = 0; num12 < parkedVehicles.Length; num12++)
			{
				Entity val3 = parkedVehicles[num12];
				Game.Vehicles.GarbageTruck garbageTruck2 = m_GarbageTruckData[val3];
				bool flag2 = availableVehicles <= 0 || availableSpace <= 0;
				if ((garbageTruck2.m_State & GarbageTruckFlags.Disabled) != 0 != flag2)
				{
					m_ActionQueue.Enqueue(GarbageFacilityAction.SetDisabled(val3, flag2));
				}
			}
			if (availableGarbage > 0 && BuildingUtils.CheckOption(building, BuildingOption.Empty))
			{
				garbageFacility.m_DeliverGarbagePriority = 2f;
			}
			else if (availableGarbage >= minGarbageLoad)
			{
				garbageFacility.m_DeliverGarbagePriority = (float)availableGarbage / (float)(prefabGarbageFacilityData.m_GarbageCapacity + areaCapacity + maxGarbageLoad);
			}
			else
			{
				garbageFacility.m_DeliverGarbagePriority = 0f;
			}
			if (availableSpace >= minGarbageLoad)
			{
				garbageFacility.m_AcceptGarbagePriority = (float)availableSpace / (float)(prefabGarbageFacilityData.m_GarbageCapacity + areaCapacity + maxGarbageLoad);
			}
			else
			{
				garbageFacility.m_AcceptGarbagePriority = 0f;
			}
			if (!outside)
			{
				if (garbageFacility.m_AcceptGarbagePriority > 0f)
				{
					GarbageTransferRequestFlags garbageTransferRequestFlags = GarbageTransferRequestFlags.Deliver;
					if (availableDeliveryTrucks <= 0)
					{
						garbageTransferRequestFlags |= GarbageTransferRequestFlags.RequireTransport;
					}
					int amount = math.min(availableSpace, maxGarbageLoad);
					if (m_GarbageTransferRequestData.HasComponent(garbageFacility.m_GarbageDeliverRequest))
					{
						if (m_GarbageTransferRequestData[garbageFacility.m_GarbageDeliverRequest].m_Flags != garbageTransferRequestFlags)
						{
							Entity val4 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val4, new HandleRequest(garbageFacility.m_GarbageDeliverRequest, Entity.Null, completed: true));
						}
						else
						{
							garbageTransferRequestFlags = (GarbageTransferRequestFlags)0;
						}
					}
					if (garbageTransferRequestFlags != 0)
					{
						Entity val5 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_GarbageTransferRequestArchetype);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<GarbageTransferRequest>(jobIndex, val5, new GarbageTransferRequest(entity, garbageTransferRequestFlags, garbageFacility.m_AcceptGarbagePriority, amount));
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val5, new RequestGroup(8u));
					}
				}
				if (garbageFacility.m_DeliverGarbagePriority > 0f)
				{
					GarbageTransferRequestFlags garbageTransferRequestFlags2 = GarbageTransferRequestFlags.Receive;
					if (availableDeliveryTrucks <= 0)
					{
						garbageTransferRequestFlags2 |= GarbageTransferRequestFlags.RequireTransport;
					}
					int amount2 = math.min(availableGarbage, maxGarbageLoad);
					if (m_GarbageTransferRequestData.HasComponent(garbageFacility.m_GarbageReceiveRequest))
					{
						if (m_GarbageTransferRequestData[garbageFacility.m_GarbageReceiveRequest].m_Flags != garbageTransferRequestFlags2)
						{
							Entity val6 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val6, new HandleRequest(garbageFacility.m_GarbageReceiveRequest, Entity.Null, completed: true));
						}
						else
						{
							garbageTransferRequestFlags2 = (GarbageTransferRequestFlags)0;
						}
					}
					if (garbageTransferRequestFlags2 != 0)
					{
						Entity val7 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_GarbageTransferRequestArchetype);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<GarbageTransferRequest>(jobIndex, val7, new GarbageTransferRequest(entity, garbageTransferRequestFlags2, garbageFacility.m_DeliverGarbagePriority, amount2));
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val7, new RequestGroup(8u));
					}
				}
			}
			if (prefabGarbageFacilityData.m_LongTermStorage)
			{
				if (garbageAmount + areaGarbage >= prefabGarbageFacilityData.m_GarbageCapacity + areaCapacity)
				{
					if ((garbageFacility.m_Flags & GarbageFacilityFlags.IsFull) == 0)
					{
						m_IconCommandBuffer.Add(entity, m_GarbageParameters.m_FacilityFullNotificationPrefab);
						garbageFacility.m_Flags |= GarbageFacilityFlags.IsFull;
					}
				}
				else if ((garbageFacility.m_Flags & GarbageFacilityFlags.IsFull) != 0)
				{
					m_IconCommandBuffer.Remove(entity, m_GarbageParameters.m_FacilityFullNotificationPrefab);
					garbageFacility.m_Flags &= ~GarbageFacilityFlags.IsFull;
				}
			}
			if (availableVehicles > 0)
			{
				garbageFacility.m_Flags |= GarbageFacilityFlags.HasAvailableGarbageTrucks;
			}
			else
			{
				garbageFacility.m_Flags &= ~GarbageFacilityFlags.HasAvailableGarbageTrucks;
			}
			if (availableSpace > 0)
			{
				garbageFacility.m_Flags |= GarbageFacilityFlags.HasAvailableSpace;
			}
			else
			{
				garbageFacility.m_Flags &= ~GarbageFacilityFlags.HasAvailableSpace;
			}
			if (prefabGarbageFacilityData.m_IndustrialWasteOnly)
			{
				garbageFacility.m_Flags |= GarbageFacilityFlags.IndustrialWasteOnly;
			}
			else
			{
				garbageFacility.m_Flags &= ~GarbageFacilityFlags.IndustrialWasteOnly;
			}
			if (availableVehicles > 0 && availableSpace > 0)
			{
				RequestTargetIfNeeded(jobIndex, entity, ref garbageFacility, prefabGarbageFacilityData, availableVehicles);
			}
		}

		private void RequestTargetIfNeeded(int jobIndex, Entity entity, ref Game.Buildings.GarbageFacility garbageFacility, GarbageFacilityData prefabGarbageFacilityData, int availableVehicles)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			if (!m_ServiceRequestData.HasComponent(garbageFacility.m_TargetRequest))
			{
				uint num = math.max(512u, 256u);
				if ((m_SimulationFrameIndex & (num - 1)) == 80)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_GarbageCollectionRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<GarbageCollectionRequest>(jobIndex, val, new GarbageCollectionRequest(entity, availableVehicles, prefabGarbageFacilityData.m_IndustrialWasteOnly ? GarbageCollectionRequestFlags.IndustrialWaste : ((GarbageCollectionRequestFlags)0)));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(32u));
				}
			}
		}

		private bool TrySpawnGarbageTruck(int jobIndex, ref Random random, Entity entity, Entity request, GarbageFacilityData prefabGarbageFacilityData, ref Game.Buildings.GarbageFacility garbageFacility, ref int availableVehicles, ref int availableSpace, ref StackList<Entity> parkedVehicles)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			if (availableVehicles <= 0 || availableSpace <= 0)
			{
				return false;
			}
			GarbageCollectionRequest garbageCollectionRequest = default(GarbageCollectionRequest);
			if (!m_GarbageCollectionRequestData.TryGetComponent(request, ref garbageCollectionRequest))
			{
				return false;
			}
			Entity target = garbageCollectionRequest.m_Target;
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(target))
			{
				return false;
			}
			int2 garbageCapacity = default(int2);
			((int2)(ref garbageCapacity))._002Ector(1, availableSpace);
			Entity val = Entity.Null;
			PathInformation pathInformation = default(PathInformation);
			if (m_PathInformationData.TryGetComponent(request, ref pathInformation) && pathInformation.m_Origin != entity)
			{
				PrefabRef prefabRef = default(PrefabRef);
				GarbageTruckData garbageTruckData = default(GarbageTruckData);
				if (m_PrefabRefData.TryGetComponent(pathInformation.m_Origin, ref prefabRef) && m_PrefabGarbageTruckData.TryGetComponent(prefabRef.m_Prefab, ref garbageTruckData))
				{
					garbageCapacity = int2.op_Implicit(garbageTruckData.m_GarbageCapacity);
				}
				if (!CollectionUtils.RemoveValueSwapBack<Entity>(ref parkedVehicles, pathInformation.m_Origin))
				{
					return false;
				}
				ParkedCar parkedCar = m_ParkedCarData[pathInformation.m_Origin];
				val = pathInformation.m_Origin;
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val, ref m_ParkedToMovingRemoveTypes);
				Game.Vehicles.CarLaneFlags flags = Game.Vehicles.CarLaneFlags.EndReached | Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.FixedLane;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val, ref m_ParkedToMovingCarAddTypes);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarCurrentLane>(jobIndex, val, new CarCurrentLane(parkedCar, flags));
				if (m_ParkingLaneData.HasComponent(parkedCar.m_Lane) || m_SpawnLocationData.HasComponent(parkedCar.m_Lane))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, parkedCar.m_Lane);
				}
			}
			if (val == Entity.Null)
			{
				val = m_GarbageTruckSelectData.CreateVehicle(m_CommandBuffer, jobIndex, ref random, m_TransformData[entity], entity, Entity.Null, ref garbageCapacity, parked: false);
				if (val == Entity.Null)
				{
					return false;
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val, new Owner(entity));
			}
			availableVehicles--;
			availableSpace -= garbageCapacity.y;
			GarbageTruckFlags garbageTruckFlags = (GarbageTruckFlags)0u;
			if (prefabGarbageFacilityData.m_IndustrialWasteOnly)
			{
				garbageTruckFlags |= GarbageTruckFlags.IndustrialWasteOnly;
			}
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Vehicles.GarbageTruck>(jobIndex, val, new Game.Vehicles.GarbageTruck(garbageTruckFlags, 1));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(jobIndex, val, new Target(target));
			((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<ServiceDispatch>(jobIndex, val).Add(new ServiceDispatch(request));
			Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(request, val, completed: false));
			DynamicBuffer<PathElement> sourceElements = default(DynamicBuffer<PathElement>);
			if (m_PathElements.TryGetBuffer(request, ref sourceElements) && sourceElements.Length != 0)
			{
				DynamicBuffer<PathElement> targetElements = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<PathElement>(jobIndex, val);
				PathUtils.CopyPath(sourceElements, default(PathOwner), 0, targetElements);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathOwner>(jobIndex, val, new PathOwner(PathFlags.Updated));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(jobIndex, val, pathInformation);
			}
			if (m_ServiceRequestData.HasComponent(garbageFacility.m_TargetRequest))
			{
				val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(garbageFacility.m_TargetRequest, Entity.Null, completed: true));
			}
			return true;
		}

		private bool TrySpawnDeliveryTruck(int jobIndex, ref Random random, Entity entity, Entity request, ref int availableDeliveryTrucks, ref int availableSpace, ref int availableGarbage, ref int garbageAmount)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			if (availableDeliveryTrucks <= 0)
			{
				return false;
			}
			GarbageTransferRequest garbageTransferRequest = m_GarbageTransferRequestData[request];
			PathInformation pathInformation = m_PathInformationData[request];
			if (!m_PrefabRefData.HasComponent(pathInformation.m_Destination))
			{
				return false;
			}
			DeliveryTruckFlags deliveryTruckFlags = (DeliveryTruckFlags)0u;
			Resource resource = Resource.Garbage;
			Resource returnResource = Resource.NoResource;
			int amount = garbageTransferRequest.m_Amount;
			int returnAmount = 0;
			if ((garbageTransferRequest.m_Flags & GarbageTransferRequestFlags.RequireTransport) != 0)
			{
				if ((garbageTransferRequest.m_Flags & GarbageTransferRequestFlags.Deliver) != 0)
				{
					deliveryTruckFlags |= DeliveryTruckFlags.Loaded;
				}
				if ((garbageTransferRequest.m_Flags & GarbageTransferRequestFlags.Receive) != 0)
				{
					deliveryTruckFlags |= DeliveryTruckFlags.Buying;
				}
			}
			else
			{
				if ((garbageTransferRequest.m_Flags & GarbageTransferRequestFlags.Deliver) != 0)
				{
					deliveryTruckFlags |= DeliveryTruckFlags.Buying;
				}
				if ((garbageTransferRequest.m_Flags & GarbageTransferRequestFlags.Receive) != 0)
				{
					deliveryTruckFlags |= DeliveryTruckFlags.Loaded;
				}
			}
			if ((deliveryTruckFlags & DeliveryTruckFlags.Loaded) != 0)
			{
				amount = math.min(amount, availableGarbage);
				amount = math.min(amount, garbageAmount);
				if (amount <= 0)
				{
					return false;
				}
			}
			else
			{
				returnResource = resource;
				returnAmount = amount;
				resource = Resource.NoResource;
				amount = 0;
				returnAmount = math.min(returnAmount, amount + availableSpace);
				if (returnAmount <= 0)
				{
					return false;
				}
				deliveryTruckFlags = (DeliveryTruckFlags)((uint)deliveryTruckFlags & 0xFFFFFFEFu);
				deliveryTruckFlags |= DeliveryTruckFlags.Loaded;
			}
			Entity val = m_DeliveryTruckSelectData.CreateVehicle(m_CommandBuffer, jobIndex, ref random, ref m_PrefabDeliveryTruckData, ref m_PrefabObjectData, resource, returnResource, ref amount, ref returnAmount, m_TransformData[entity], entity, deliveryTruckFlags);
			if (val != Entity.Null)
			{
				availableDeliveryTrucks--;
				availableSpace += amount - returnAmount;
				availableGarbage -= amount;
				garbageAmount -= amount;
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(jobIndex, val, new Target(pathInformation.m_Destination));
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val, new Owner(entity));
				Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(request, val, completed: true));
				if (m_PathElements.HasBuffer(request))
				{
					DynamicBuffer<PathElement> sourceElements = m_PathElements[request];
					if (sourceElements.Length != 0)
					{
						DynamicBuffer<PathElement> targetElements = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<PathElement>(jobIndex, val);
						PathUtils.CopyPath(sourceElements, default(PathOwner), 0, targetElements);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathOwner>(jobIndex, val, new PathOwner(PathFlags.Updated));
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(jobIndex, val, pathInformation);
					}
				}
				return true;
			}
			return false;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct GarbageFacilityActionJob : IJob
	{
		public ComponentLookup<Game.Vehicles.GarbageTruck> m_GarbageTruckData;

		public NativeQueue<GarbageFacilityAction> m_ActionQueue;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			GarbageFacilityAction garbageFacilityAction = default(GarbageFacilityAction);
			Game.Vehicles.GarbageTruck garbageTruck = default(Game.Vehicles.GarbageTruck);
			while (m_ActionQueue.TryDequeue(ref garbageFacilityAction))
			{
				if (m_GarbageTruckData.TryGetComponent(garbageFacilityAction.m_Entity, ref garbageTruck))
				{
					if (garbageFacilityAction.m_Disabled)
					{
						garbageTruck.m_State |= GarbageTruckFlags.Disabled;
					}
					else
					{
						garbageTruck.m_State &= ~GarbageTruckFlags.Disabled;
					}
					m_GarbageTruckData[garbageFacilityAction.m_Entity] = garbageTruck;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Buildings.GarbageFacility> __Game_Buildings_GarbageFacility_RW_ComponentTypeHandle;

		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		public BufferTypeHandle<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RW_BufferTypeHandle;

		public BufferTypeHandle<GuestVehicle> __Game_Vehicles_GuestVehicle_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<GarbageCollectionRequest> __Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageTransferRequest> __Game_Simulation_GarbageTransferRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Quantity> __Game_Objects_Quantity_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.GarbageTruck> __Game_Vehicles_GarbageTruck_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ReturnLoad> __Game_Vehicles_ReturnLoad_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Geometry> __Game_Areas_Geometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageFacilityData> __Game_Prefabs_GarbageFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageTruckData> __Game_Prefabs_GarbageTruckData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageAreaData> __Game_Prefabs_StorageAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DeliveryTruckData> __Game_Prefabs_DeliveryTruckData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectData> __Game_Prefabs_ObjectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ResourceProductionData> __Game_Prefabs_ResourceProductionData_RO_BufferLookup;

		public ComponentLookup<Storage> __Game_Areas_Storage_RW_ComponentLookup;

		public ComponentLookup<Game.Vehicles.GarbageTruck> __Game_Vehicles_GarbageTruck_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Objects_OutsideConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.OutsideConnection>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Areas_SubArea_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Areas.SubArea>(true);
			__Game_Buildings_GarbageFacility_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.GarbageFacility>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Vehicles_OwnedVehicle_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<OwnedVehicle>(false);
			__Game_Vehicles_GuestVehicle_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<GuestVehicle>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageCollectionRequest>(true);
			__Game_Simulation_GarbageTransferRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageTransferRequest>(true);
			__Game_Simulation_ServiceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Objects_Quantity_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Quantity>(true);
			__Game_Vehicles_GarbageTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.GarbageTruck>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Vehicles_ReturnLoad_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ReturnLoad>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Areas_Geometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Geometry>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_GarbageFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageFacilityData>(true);
			__Game_Prefabs_GarbageTruckData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageTruckData>(true);
			__Game_Prefabs_StorageAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageAreaData>(true);
			__Game_Prefabs_DeliveryTruckData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DeliveryTruckData>(true);
			__Game_Prefabs_ObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectData>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Prefabs_ResourceProductionData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ResourceProductionData>(true);
			__Game_Areas_Storage_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Storage>(false);
			__Game_Vehicles_GarbageTruck_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.GarbageTruck>(false);
		}
	}

	private const int kUpdatesPerDay = 1024;

	private VehicleCapacitySystem m_VehicleCapacitySystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private IconCommandSystem m_IconCommandSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private GarbageTruckSelectData m_GarbageTruckSelectData;

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_GarbageTruckPrefabQuery;

	private EntityQuery m_GarbageSettingsQuery;

	private EntityArchetype m_GarbageTransferRequestArchetype;

	private EntityArchetype m_GarbageCollectionRequestArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_ParkedToMovingRemoveTypes;

	private ComponentTypeSet m_ParkedToMovingCarAddTypes;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 80;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_VehicleCapacitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehicleCapacitySystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_GarbageTruckSelectData = new GarbageTruckSelectData((SystemBase)(object)this);
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Buildings.GarbageFacility>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_GarbageSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageParameterData>() });
		m_GarbageTruckPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { GarbageTruckSelectData.GetEntityQueryDesc() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_GarbageTransferRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<GarbageTransferRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_GarbageCollectionRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<GarbageCollectionRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_HandleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<HandleRequest>(),
			ComponentType.ReadWrite<Event>()
		});
		m_ParkedToMovingRemoveTypes = new ComponentTypeSet(ComponentType.ReadWrite<ParkedCar>(), ComponentType.ReadWrite<Stopped>());
		m_ParkedToMovingCarAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[14]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<CarNavigation>(),
			ComponentType.ReadWrite<CarNavigationLane>(),
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<PathElement>(),
			ComponentType.ReadWrite<PathInformation>(),
			ComponentType.ReadWrite<ServiceDispatch>(),
			ComponentType.ReadWrite<Swaying>(),
			ComponentType.ReadWrite<Updated>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_BuildingQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_GarbageSettingsQuery);
		Assert.IsTrue(true);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		m_GarbageTruckSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_GarbageTruckPrefabQuery, (Allocator)3, out var jobHandle);
		NativeQueue<GarbageFacilityAction> actionQueue = default(NativeQueue<GarbageFacilityAction>);
		actionQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		GarbageFacilityTickJob garbageFacilityTickJob = new GarbageFacilityTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreaType = InternalCompilerInterface.GetBufferTypeHandle<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.GarbageFacility>(ref __TypeHandle.__Game_Buildings_GarbageFacility_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatchType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcesType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicleType = InternalCompilerInterface.GetBufferTypeHandle<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GuestVehicleType = InternalCompilerInterface.GetBufferTypeHandle<GuestVehicle>(ref __TypeHandle.__Game_Vehicles_GuestVehicle_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageCollectionRequestData = InternalCompilerInterface.GetComponentLookup<GarbageCollectionRequest>(ref __TypeHandle.__Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageTransferRequestData = InternalCompilerInterface.GetComponentLookup<GarbageTransferRequest>(ref __TypeHandle.__Game_Simulation_GarbageTransferRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetData = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_QuantityData = InternalCompilerInterface.GetComponentLookup<Quantity>(ref __TypeHandle.__Game_Objects_Quantity_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageTruckData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.GarbageTruck>(ref __TypeHandle.__Game_Vehicles_GarbageTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeliveryTruckData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ReturnLoadData = InternalCompilerInterface.GetComponentLookup<ReturnLoad>(ref __TypeHandle.__Game_Vehicles_ReturnLoad_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaGeometryData = InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGarbageFacilityData = InternalCompilerInterface.GetComponentLookup<GarbageFacilityData>(ref __TypeHandle.__Game_Prefabs_GarbageFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGarbageTruckData = InternalCompilerInterface.GetComponentLookup<GarbageTruckData>(ref __TypeHandle.__Game_Prefabs_GarbageTruckData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStorageAreaData = InternalCompilerInterface.GetComponentLookup<StorageAreaData>(ref __TypeHandle.__Game_Prefabs_StorageAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDeliveryTruckData = InternalCompilerInterface.GetComponentLookup<DeliveryTruckData>(ref __TypeHandle.__Game_Prefabs_DeliveryTruckData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectData = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceProductionDatas = InternalCompilerInterface.GetBufferLookup<ResourceProductionData>(ref __TypeHandle.__Game_Prefabs_ResourceProductionData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaStorageData = InternalCompilerInterface.GetComponentLookup<Storage>(ref __TypeHandle.__Game_Areas_Storage_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex,
			m_GarbageParameters = ((EntityQuery)(ref m_GarbageSettingsQuery)).GetSingleton<GarbageParameterData>(),
			m_GarbageTruckSelectData = m_GarbageTruckSelectData,
			m_DeliveryTruckSelectData = m_VehicleCapacitySystem.GetDeliveryTruckSelectData(),
			m_GarbageTransferRequestArchetype = m_GarbageTransferRequestArchetype,
			m_GarbageCollectionRequestArchetype = m_GarbageCollectionRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_ParkedToMovingRemoveTypes = m_ParkedToMovingRemoveTypes,
			m_ParkedToMovingCarAddTypes = m_ParkedToMovingCarAddTypes,
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		garbageFacilityTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		garbageFacilityTickJob.m_ActionQueue = actionQueue.AsParallelWriter();
		GarbageFacilityTickJob garbageFacilityTickJob2 = garbageFacilityTickJob;
		GarbageFacilityActionJob obj = new GarbageFacilityActionJob
		{
			m_GarbageTruckData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.GarbageTruck>(ref __TypeHandle.__Game_Vehicles_GarbageTruck_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActionQueue = actionQueue
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<GarbageFacilityTickJob>(garbageFacilityTickJob2, m_BuildingQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, jobHandle));
		JobHandle val3 = IJobExtensions.Schedule<GarbageFacilityActionJob>(obj, val2);
		actionQueue.Dispose(val3);
		m_GarbageTruckSelectData.PostUpdate(val2);
		m_IconCommandSystem.AddCommandBufferWriter(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val3;
	}

	private static float CalculateProcessingRate(float maxProcessingRate, float efficiency, int garbageAmount, int garbageCapacity)
	{
		float num = CalculateGarbageAmountFactor(garbageAmount, garbageCapacity);
		return efficiency * num * maxProcessingRate;
	}

	private static float CalculateGarbageAmountFactor(int garbageAmount, int garbageCapacity)
	{
		return math.saturate(0.1f + (float)garbageAmount * 1.8f / math.max(1f, (float)garbageCapacity));
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
	public GarbageFacilityAISystem()
	{
	}
}

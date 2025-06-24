using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Debug;
using Game.Economy;
using Game.Events;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TripNeededSystem : GameSystemBase
{
	[BurstCompile]
	private struct CompanyJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		public BufferTypeHandle<TripNeeded> m_TripNeededType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyRenterType;

		[ReadOnly]
		public ComponentTypeHandle<CreatureData> m_CreatureDataType;

		[ReadOnly]
		public ComponentTypeHandle<ResidentData> m_ResidentDataType;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> m_VehicleType;

		public BufferTypeHandle<Resources> m_ResourceType;

		[ReadOnly]
		public ComponentLookup<TransportCompanyData> m_TransportCompanyDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<DeliveryTruckData> m_PrefabDeliveryTruckData;

		[ReadOnly]
		public ComponentLookup<ObjectData> m_PrefabObjectData;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> m_UnderConstructionData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterData;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_ActivityLocationElements;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_HumanChunks;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public DeliveryTruckSelectData m_DeliveryTruckSelectData;

		[ReadOnly]
		public ComponentTypeSet m_CurrentLaneTypesRelative;

		public ParallelWriter m_CommandBuffer;

		public bool m_DebugDisableSpawning;

		private void SpawnDeliveryTruck(int index, Entity owner, Entity from, ref Transform transform, TripNeeded trip)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			Entity val;
			Entity val2;
			if (m_ServiceRequestData.HasComponent(trip.m_TargetAgent))
			{
				PathInformation pathInformation = default(PathInformation);
				if (!m_PathInformationData.TryGetComponent(trip.m_TargetAgent, ref pathInformation))
				{
					return;
				}
				val = pathInformation.m_Destination;
				val2 = trip.m_TargetAgent;
			}
			else
			{
				val = trip.m_TargetAgent;
				val2 = Entity.Null;
			}
			if (!m_Prefabs.HasComponent(val))
			{
				return;
			}
			Entity val3 = val;
			PropertyRenter propertyRenter = default(PropertyRenter);
			if (m_PropertyRenterData.TryGetComponent(val3, ref propertyRenter))
			{
				val3 = propertyRenter.m_Property;
			}
			uint num = 0u;
			UnderConstruction underConstruction = default(UnderConstruction);
			if (m_UnderConstructionData.TryGetComponent(val3, ref underConstruction) && underConstruction.m_NewPrefab == Entity.Null)
			{
				PathInformation pathInformation2 = default(PathInformation);
				m_PathInformationData.TryGetComponent(val2, ref pathInformation2);
				num = ObjectUtils.GetTripDelayFrames(underConstruction, pathInformation2);
			}
			if (m_UnderConstructionData.TryGetComponent(from, ref underConstruction) && underConstruction.m_NewPrefab == Entity.Null)
			{
				num = math.max(num, ObjectUtils.GetRemainingConstructionFrames(underConstruction));
			}
			Random random = m_RandomSeed.GetRandom(owner.Index);
			DeliveryTruckFlags deliveryTruckFlags = (DeliveryTruckFlags)0u;
			Resource resource = trip.m_Resource;
			Resource resource2 = Resource.NoResource;
			int amount = math.abs(trip.m_Data);
			int returnAmount = 0;
			switch (trip.m_Purpose)
			{
			case Purpose.Exporting:
				deliveryTruckFlags |= DeliveryTruckFlags.Loaded;
				break;
			case Purpose.Delivery:
				deliveryTruckFlags |= DeliveryTruckFlags.Loaded | DeliveryTruckFlags.Delivering;
				break;
			case Purpose.UpkeepDelivery:
				deliveryTruckFlags |= DeliveryTruckFlags.Loaded | DeliveryTruckFlags.Delivering | DeliveryTruckFlags.NoUnloading;
				break;
			case Purpose.Collect:
				deliveryTruckFlags |= DeliveryTruckFlags.Buying;
				break;
			case Purpose.Shopping:
				deliveryTruckFlags |= DeliveryTruckFlags.Buying;
				break;
			case Purpose.CompanyShopping:
				deliveryTruckFlags |= DeliveryTruckFlags.Buying | DeliveryTruckFlags.UpdateSellerQuantity;
				break;
			case Purpose.StorageTransfer:
				deliveryTruckFlags = ((trip.m_Data <= 0) ? (deliveryTruckFlags | (DeliveryTruckFlags.Buying | DeliveryTruckFlags.StorageTransfer)) : (deliveryTruckFlags | (DeliveryTruckFlags.Loaded | DeliveryTruckFlags.StorageTransfer)));
				break;
			case Purpose.ReturnUnsortedMail:
				deliveryTruckFlags |= DeliveryTruckFlags.Loaded;
				resource2 = Resource.UnsortedMail;
				returnAmount = amount;
				amount = math.select(amount, 0, trip.m_Resource == Resource.NoResource);
				break;
			case Purpose.ReturnLocalMail:
				deliveryTruckFlags |= DeliveryTruckFlags.Loaded;
				resource2 = Resource.LocalMail;
				returnAmount = amount;
				amount = math.select(amount, 0, trip.m_Resource == Resource.NoResource);
				break;
			case Purpose.ReturnOutgoingMail:
				deliveryTruckFlags |= DeliveryTruckFlags.Loaded;
				resource2 = Resource.OutgoingMail;
				returnAmount = amount;
				amount = math.select(amount, 0, trip.m_Resource == Resource.NoResource);
				break;
			case Purpose.ReturnGarbage:
				deliveryTruckFlags |= DeliveryTruckFlags.Loaded;
				resource2 = Resource.Garbage;
				returnAmount = amount;
				amount = math.select(amount, 0, trip.m_Resource == Resource.NoResource);
				break;
			}
			if (amount > 0)
			{
				deliveryTruckFlags |= DeliveryTruckFlags.UpdateOwnerQuantity;
			}
			Resource resources = resource | resource2;
			int capacity = math.max(amount, returnAmount);
			if (!m_DeliveryTruckSelectData.TrySelectItem(ref random, resources, capacity, out var item))
			{
				return;
			}
			Entity val4 = m_DeliveryTruckSelectData.CreateVehicle(m_CommandBuffer, index, ref random, ref m_PrefabDeliveryTruckData, ref m_PrefabObjectData, item, resource, resource2, ref amount, ref returnAmount, transform, from, deliveryTruckFlags, num);
			int maxCount = 1;
			if (CreatePassengers(index, val4, item.m_Prefab1, transform, driver: true, ref maxCount, ref random) > 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<Passenger>(index, val4);
			}
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(index, val4, new Target(val));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(index, val4, new Owner(owner));
			if (!(val2 != Entity.Null))
			{
				return;
			}
			Entity val5 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(index, m_HandleRequestArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(index, val5, new HandleRequest(val2, val4, completed: true));
			if (m_PathElements.HasBuffer(val2))
			{
				DynamicBuffer<PathElement> sourceElements = m_PathElements[val2];
				if (sourceElements.Length != 0)
				{
					DynamicBuffer<PathElement> targetElements = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<PathElement>(index, val4);
					PathUtils.CopyPath(sourceElements, default(PathOwner), 0, targetElements);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathOwner>(index, val4, new PathOwner(PathFlags.Updated));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(index, val4, m_PathInformationData[val2]);
				}
			}
		}

		private int CreatePassengers(int jobIndex, Entity vehicleEntity, Entity vehiclePrefab, Transform transform, bool driver, ref int maxCount, ref Random random)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			DynamicBuffer<ActivityLocationElement> val = default(DynamicBuffer<ActivityLocationElement>);
			if (maxCount > 0 && m_ActivityLocationElements.TryGetBuffer(vehiclePrefab, ref val))
			{
				ActivityMask activityMask = new ActivityMask(ActivityType.Driving);
				int num2 = 0;
				int num3 = -1;
				float num4 = float.MinValue;
				for (int i = 0; i < val.Length; i++)
				{
					ActivityLocationElement activityLocationElement = val[i];
					if ((activityLocationElement.m_ActivityMask.m_Mask & activityMask.m_Mask) != 0)
					{
						num2++;
						bool flag = ((activityLocationElement.m_ActivityFlags & ActivityFlags.InvertLefthandTraffic) != 0 && m_LeftHandTraffic) || ((activityLocationElement.m_ActivityFlags & ActivityFlags.InvertRighthandTraffic) != 0 && !m_LeftHandTraffic);
						activityLocationElement.m_Position.x = math.select(activityLocationElement.m_Position.x, 0f - activityLocationElement.m_Position.x, flag);
						if ((!(math.abs(activityLocationElement.m_Position.x) >= 0.5f) || activityLocationElement.m_Position.x >= 0f == m_LeftHandTraffic) && activityLocationElement.m_Position.z > num4)
						{
							num3 = i;
							num4 = activityLocationElement.m_Position.z;
						}
					}
				}
				int num5 = 100;
				if (driver && num3 != -1)
				{
					maxCount--;
					num2--;
				}
				if (num2 > maxCount)
				{
					num5 = maxCount * 100 / num2;
				}
				Relative relative = default(Relative);
				for (int j = 0; j < val.Length; j++)
				{
					ActivityLocationElement activityLocationElement2 = val[j];
					if ((activityLocationElement2.m_ActivityMask.m_Mask & activityMask.m_Mask) != 0 && ((driver && j == num3) || ((Random)(ref random)).NextInt(100) >= num5))
					{
						relative.m_Position = activityLocationElement2.m_Position;
						relative.m_Rotation = activityLocationElement2.m_Rotation;
						relative.m_BoneIndex = new int3(0, -1, -1);
						Citizen citizenData = default(Citizen);
						if (((Random)(ref random)).NextBool())
						{
							citizenData.m_State |= CitizenFlags.Male;
						}
						if (driver)
						{
							citizenData.SetAge(CitizenAge.Adult);
						}
						else
						{
							citizenData.SetAge((CitizenAge)((Random)(ref random)).NextInt(4));
						}
						citizenData.m_PseudoRandom = (ushort)(((Random)(ref random)).NextUInt() % 65536);
						CreatureData creatureData;
						PseudoRandomSeed randomSeed;
						Entity val2 = ObjectEmergeSystem.SelectResidentPrefab(citizenData, m_HumanChunks, m_EntityType, ref m_CreatureDataType, ref m_ResidentDataType, out creatureData, out randomSeed);
						ObjectData objectData = m_PrefabObjectData[val2];
						PrefabRef prefabRef = new PrefabRef
						{
							m_Prefab = val2
						};
						Game.Creatures.Resident resident = default(Game.Creatures.Resident);
						resident.m_Flags |= ResidentFlags.InVehicle | ResidentFlags.DummyTraffic;
						CurrentVehicle currentVehicle = new CurrentVehicle
						{
							m_Vehicle = vehicleEntity
						};
						currentVehicle.m_Flags |= CreatureVehicleFlags.Ready;
						if (driver && j == num3)
						{
							currentVehicle.m_Flags |= CreatureVehicleFlags.Leader | CreatureVehicleFlags.Driver;
						}
						Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, objectData.m_Archetype);
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val3, ref m_CurrentLaneTypesRelative);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, val3, transform);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val3, prefabRef);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Creatures.Resident>(jobIndex, val3, resident);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val3, randomSeed);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentVehicle>(jobIndex, val3, currentVehicle);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Relative>(jobIndex, val3, relative);
						num++;
					}
				}
			}
			return num;
		}

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
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			BufferAccessor<OwnedVehicle> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedVehicle>(ref m_VehicleType);
			BufferAccessor<TripNeeded> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TripNeeded>(ref m_TripNeededType);
			BufferAccessor<Resources> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			NativeArray<PropertyRenter> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyRenterType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity prefab = nativeArray2[i].m_Prefab;
				if (m_TransportCompanyDatas.HasComponent(prefab) && bufferAccessor[i].Length >= m_TransportCompanyDatas[prefab].m_MaxTransports)
				{
					continue;
				}
				Entity val = nativeArray[i];
				DynamicBuffer<TripNeeded> val2 = bufferAccessor2[i];
				if (val2.Length <= 0)
				{
					continue;
				}
				TripNeeded trip = val2[0];
				val2.RemoveAt(0);
				if (!m_DebugDisableSpawning)
				{
					_ = bufferAccessor3[i];
					Entity val3 = ((!((ArchetypeChunk)(ref chunk)).Has<PropertyRenter>(ref m_PropertyRenterType)) ? val : nativeArray3[i].m_Property);
					if (m_Transforms.HasComponent(val3))
					{
						Transform transform = m_Transforms[val3];
						SpawnDeliveryTruck(unfilteredChunkIndex, val, val3, ref transform, trip);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct AnimalTargetInfo
	{
		public Entity m_Animal;

		public Entity m_Source;

		public Entity m_Target;
	}

	[BurstCompile]
	private struct PetTargetJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildingData;

		public NativeQueue<AnimalTargetInfo> m_AnimalQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			int count = m_AnimalQueue.Count;
			if (count == 0)
			{
				return;
			}
			NativeParallelHashSet<Entity> val = default(NativeParallelHashSet<Entity>);
			val._002Ector(count, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < count; i++)
			{
				AnimalTargetInfo animalTargetInfo = m_AnimalQueue.Dequeue();
				if (m_CurrentBuildingData.HasComponent(animalTargetInfo.m_Animal) && !(m_CurrentBuildingData[animalTargetInfo.m_Animal].m_CurrentBuilding != animalTargetInfo.m_Source) && val.Add(animalTargetInfo.m_Animal))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Target>(animalTargetInfo.m_Animal, new Target(animalTargetInfo.m_Target));
				}
			}
			val.Dispose();
		}
	}

	[BurstCompile]
	private struct CitizeLeaveJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildingData;

		public ComponentLookup<CitizenPresence> m_CitizenPresenceData;

		public NativeQueue<Entity> m_LeaveQueue;

		public void Execute()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			Entity val = default(Entity);
			while (m_LeaveQueue.TryDequeue(ref val))
			{
				if (m_CurrentBuildingData.HasComponent(val))
				{
					CurrentBuilding currentBuilding = m_CurrentBuildingData[val];
					if (m_CitizenPresenceData.HasComponent(currentBuilding.m_CurrentBuilding))
					{
						CitizenPresence citizenPresence = m_CitizenPresenceData[currentBuilding.m_CurrentBuilding];
						citizenPresence.m_Delta = (sbyte)math.max(-127, citizenPresence.m_Delta - 1);
						m_CitizenPresenceData[currentBuilding.m_CurrentBuilding] = citizenPresence;
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct CitizenJob : IJobChunk
	{
		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<int> m_DebugPathQueueCar;

		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<int> m_DebugPathQueuePublic;

		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<int> m_DebugPathQueuePedestrian;

		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<int> m_DebugPathQueueCarShort;

		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<int> m_DebugPathQueuePublicShort;

		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<int> m_DebugPathQueuePedestrianShort;

		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<int> m_DebugPublicTransportDuration;

		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<int> m_DebugTaxiDuration;

		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<int> m_DebugCarDuration;

		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<int> m_DebugPedestrianDuration;

		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<int> m_DebugPedestrianDurationShort;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public BufferTypeHandle<TripNeeded> m_TripNeededType;

		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentTransport> m_CurrentTransportType;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> m_HouseholdMemberType;

		[ReadOnly]
		public ComponentTypeHandle<MailSender> m_MailSenderType;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> m_HealthProblemType;

		[ReadOnly]
		public ComponentTypeHandle<AttendingMeeting> m_AttendingMeetingType;

		[ReadOnly]
		public ComponentTypeHandle<CreatureData> m_CreatureDataType;

		[ReadOnly]
		public ComponentTypeHandle<ResidentData> m_ResidentDataType;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_Properties;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<ObjectData> m_ObjectDatas;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<HumanData> m_PrefabHumanData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInfos;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PersonalCar> m_PersonalCarData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Ambulance> m_AmbulanceData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public ComponentLookup<Target> m_Targets;

		[ReadOnly]
		public ComponentLookup<Deleted> m_Deleteds;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public ComponentLookup<CarKeeper> m_CarKeepers;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> m_UnderConstructionData;

		[ReadOnly]
		public BufferLookup<CoordinatedMeetingAttendee> m_Attendees;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> m_HouseholdAnimals;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_TravelPurposes;

		[ReadOnly]
		public BufferLookup<HaveCoordinatedMeetingData> m_HaveCoordinatedMeetingDatas;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CoordinatedMeeting> m_Meetings;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Worker> m_Workers;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Citizens.Student> m_Students;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_HumanChunks;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public float m_TimeOfDay;

		[ReadOnly]
		public EntityArchetype m_ResetTripArchetype;

		[ReadOnly]
		public ComponentTypeSet m_HumanSpawnTypes;

		[ReadOnly]
		public ComponentTypeSet m_PathfindTypes;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathQueue;

		public ParallelWriter<AnimalTargetInfo> m_AnimalQueue;

		public ParallelWriter<Entity> m_LeaveQueue;

		public bool m_DebugDisableSpawning;

		private void GetResidentFlags(Entity citizen, Entity currentBuilding, bool isMailSender, bool pathFailed, ref Target target, ref Purpose purpose, ref Purpose divertPurpose, ref uint timer, ref bool hasDivertPath)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			if (pathFailed)
			{
				divertPurpose = Purpose.PathFailed;
				return;
			}
			switch (purpose)
			{
			case Purpose.Safety:
			case Purpose.Escape:
				target.m_Target = currentBuilding;
				divertPurpose = purpose;
				if (m_TravelPurposes.HasComponent(citizen))
				{
					purpose = m_TravelPurposes[citizen].m_Purpose;
				}
				else
				{
					purpose = Purpose.None;
				}
				timer = 0u;
				hasDivertPath = true;
				break;
			case Purpose.Hospital:
				if (m_AmbulanceData.HasComponent(target.m_Target))
				{
					timer = 0u;
				}
				break;
			case Purpose.Deathcare:
				timer = 0u;
				break;
			default:
				if (isMailSender)
				{
					divertPurpose = Purpose.SendMail;
				}
				break;
			}
		}

		private Entity SpawnResident(int index, Entity citizen, Entity fromBuilding, Citizen citizenData, Target target, ResidentFlags flags, Purpose divertPurpose, uint timer, bool hasDivertPath, bool isDead, bool isCarried)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			CreatureData creatureData;
			PseudoRandomSeed randomSeed;
			Entity val = ObjectEmergeSystem.SelectResidentPrefab(citizenData, m_HumanChunks, m_EntityType, ref m_CreatureDataType, ref m_ResidentDataType, out creatureData, out randomSeed);
			ObjectData objectData = m_ObjectDatas[val];
			PrefabRef prefabRef = new PrefabRef
			{
				m_Prefab = val
			};
			Transform transform = ((!m_Transforms.HasComponent(fromBuilding)) ? new Transform
			{
				m_Position = default(float3),
				m_Rotation = new quaternion(0f, 0f, 0f, 1f)
			} : m_Transforms[fromBuilding]);
			Game.Creatures.Resident resident = new Game.Creatures.Resident
			{
				m_Citizen = citizen,
				m_Flags = flags
			};
			Human human = default(Human);
			if (isDead)
			{
				human.m_Flags |= HumanFlags.Dead;
			}
			if (isCarried)
			{
				human.m_Flags |= HumanFlags.Carried;
			}
			PathOwner pathOwner = new PathOwner(PathFlags.Updated);
			TripSource tripSource = new TripSource(fromBuilding, timer);
			Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(index, objectData.m_Archetype);
			HumanCurrentLane humanCurrentLane = default(HumanCurrentLane);
			DynamicBuffer<PathElement> sourceElements = default(DynamicBuffer<PathElement>);
			if (m_PathElements.TryGetBuffer(citizen, ref sourceElements) && sourceElements.Length > 0)
			{
				DynamicBuffer<PathElement> targetElements = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<PathElement>(index, val2);
				PathUtils.CopyPath(sourceElements, default(PathOwner), 0, targetElements);
				humanCurrentLane = new HumanCurrentLane(sourceElements[0], (CreatureLaneFlags)0u);
				pathOwner.m_State |= PathFlags.Updated;
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(index, val2, ref m_HumanSpawnTypes);
			if (divertPurpose != Purpose.None)
			{
				if (hasDivertPath)
				{
					pathOwner.m_State |= PathFlags.CachedObsolete;
				}
				else
				{
					pathOwner.m_State |= PathFlags.DivertObsolete;
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Divert>(index, val2, new Divert
				{
					m_Purpose = divertPurpose
				});
			}
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(index, val2, transform);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(index, val2, prefabRef);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(index, val2, target);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Creatures.Resident>(index, val2, resident);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Human>(index, val2, human);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathOwner>(index, val2, pathOwner);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(index, val2, randomSeed);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HumanCurrentLane>(index, val2, humanCurrentLane);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TripSource>(index, val2, tripSource);
			return val2;
		}

		private void ResetTrip(int index, Entity creature, Entity citizen, Entity fromBuilding, Target target, ResidentFlags flags, Purpose divertPurpose, uint timer, bool hasDivertPath)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(index, m_ResetTripArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ResetTrip>(index, val, new ResetTrip
			{
				m_Creature = creature,
				m_Source = fromBuilding,
				m_Target = target.m_Target,
				m_ResidentFlags = flags,
				m_DivertPurpose = divertPurpose,
				m_Delay = timer,
				m_HasDivertPath = hasDivertPath
			});
			DynamicBuffer<PathElement> sourceElements = default(DynamicBuffer<PathElement>);
			if (m_PathElements.TryGetBuffer(citizen, ref sourceElements) && sourceElements.Length > 0)
			{
				DynamicBuffer<PathElement> targetElements = ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<PathElement>(index, val);
				PathUtils.CopyPath(sourceElements, default(PathOwner), 0, targetElements);
			}
		}

		private void RemoveAllTrips(DynamicBuffer<TripNeeded> trips)
		{
			if (trips.Length <= 0)
			{
				return;
			}
			Purpose purpose = trips[0].m_Purpose;
			for (int num = trips.Length - 1; num >= 0; num--)
			{
				if (trips[num].m_Purpose == purpose)
				{
					trips.RemoveAt(num);
				}
			}
		}

		private Entity FindDistrict(Entity building)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_CurrentDistrictData.HasComponent(building))
			{
				return m_CurrentDistrictData[building].m_District;
			}
			return Entity.Null;
		}

		private void AddPetTargets(Entity household, Entity source, Entity target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			if (m_HouseholdAnimals.HasBuffer(household))
			{
				DynamicBuffer<HouseholdAnimal> val = m_HouseholdAnimals[household];
				for (int i = 0; i < val.Length; i++)
				{
					HouseholdAnimal householdAnimal = val[i];
					m_AnimalQueue.Enqueue(new AnimalTargetInfo
					{
						m_Animal = householdAnimal.m_HouseholdPet,
						m_Source = source,
						m_Target = target
					});
				}
			}
		}

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
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f73: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_145b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1046: Unknown result type (might be due to invalid IL or missing references)
			//IL_105a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1076: Unknown result type (might be due to invalid IL or missing references)
			//IL_107c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1091: Unknown result type (might be due to invalid IL or missing references)
			//IL_1096: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_10fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1102: Unknown result type (might be due to invalid IL or missing references)
			//IL_10b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_1026: Unknown result type (might be due to invalid IL or missing references)
			//IL_100b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0884: Unknown result type (might be due to invalid IL or missing references)
			//IL_1033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0894: Unknown result type (might be due to invalid IL or missing references)
			//IL_0896: Unknown result type (might be due to invalid IL or missing references)
			//IL_089b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1297: Unknown result type (might be due to invalid IL or missing references)
			//IL_1216: Unknown result type (might be due to invalid IL or missing references)
			//IL_1218: Unknown result type (might be due to invalid IL or missing references)
			//IL_121d: Unknown result type (might be due to invalid IL or missing references)
			//IL_125b: Unknown result type (might be due to invalid IL or missing references)
			//IL_125d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eff: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08da: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_12bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_12a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_12af: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0905: Unknown result type (might be due to invalid IL or missing references)
			//IL_0768: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_131f: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_12df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0916: Unknown result type (might be due to invalid IL or missing references)
			//IL_091b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0758: Unknown result type (might be due to invalid IL or missing references)
			//IL_075d: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0748: Unknown result type (might be due to invalid IL or missing references)
			//IL_074d: Unknown result type (might be due to invalid IL or missing references)
			//IL_141a: Unknown result type (might be due to invalid IL or missing references)
			//IL_143b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1447: Unknown result type (might be due to invalid IL or missing references)
			//IL_144c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1331: Unknown result type (might be due to invalid IL or missing references)
			//IL_1338: Unknown result type (might be due to invalid IL or missing references)
			//IL_133d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1345: Unknown result type (might be due to invalid IL or missing references)
			//IL_130f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1314: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_1304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c26: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0810: Unknown result type (might be due to invalid IL or missing references)
			//IL_083d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1357: Unknown result type (might be due to invalid IL or missing references)
			//IL_1366: Unknown result type (might be due to invalid IL or missing references)
			//IL_1377: Unknown result type (might be due to invalid IL or missing references)
			//IL_139a: Unknown result type (might be due to invalid IL or missing references)
			//IL_139f: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_13f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dcd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ddf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<TripNeeded> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TripNeeded>(ref m_TripNeededType);
			NativeArray<HouseholdMember> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdMember>(ref m_HouseholdMemberType);
			NativeArray<CurrentBuilding> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingType);
			NativeArray<CurrentTransport> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentTransport>(ref m_CurrentTransportType);
			NativeArray<Citizen> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Citizen>(ref m_CitizenType);
			NativeArray<HealthProblem> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HealthProblem>(ref m_HealthProblemType);
			NativeArray<AttendingMeeting> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AttendingMeeting>(ref m_AttendingMeetingType);
			PathInformation pathInformation = default(PathInformation);
			PropertyRenter propertyRenter = default(PropertyRenter);
			Game.Vehicles.PersonalCar personalCar = default(Game.Vehicles.PersonalCar);
			PropertyRenter propertyRenter2 = default(PropertyRenter);
			UnderConstruction underConstruction = default(UnderConstruction);
			Game.Vehicles.PersonalCar personalCar2 = default(Game.Vehicles.PersonalCar);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				DynamicBuffer<TripNeeded> trips = bufferAccessor[i];
				Entity household = nativeArray2[i].m_Household;
				Entity currentBuilding = nativeArray3[i].m_CurrentBuilding;
				if (trips.Length <= 0)
				{
					continue;
				}
				bool flag = trips[0].m_Purpose == Purpose.MovingAway;
				bool flag2 = trips[0].m_Purpose == Purpose.Safety || trips[0].m_Purpose == Purpose.Escape;
				bool isMailSender = ((ArchetypeChunk)(ref chunk)).IsComponentEnabled<MailSender>(ref m_MailSenderType, i);
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = m_PathInfos.TryGetComponent(val, ref pathInformation);
				if (nativeArray6.Length != 0)
				{
					HealthProblem healthProblem = nativeArray6[i];
					if ((healthProblem.m_Flags & (HealthProblemFlags.Dead | HealthProblemFlags.RequireTransport | HealthProblemFlags.InDanger | HealthProblemFlags.Trapped)) != HealthProblemFlags.None)
					{
						flag3 = (healthProblem.m_Flags & HealthProblemFlags.Dead) != 0;
						flag4 = (healthProblem.m_Flags & HealthProblemFlags.RequireTransport) != 0;
						if (!(flag3 || flag4))
						{
							if (flag5)
							{
								((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val, ref m_PathfindTypes);
							}
							continue;
						}
						while (trips.Length > 0 && trips[0].m_Purpose != Purpose.Deathcare && trips[0].m_Purpose != Purpose.Hospital)
						{
							trips.RemoveAt(0);
						}
						if (trips.Length == 0)
						{
							if (flag5)
							{
								((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val, ref m_PathfindTypes);
							}
							continue;
						}
					}
				}
				if (!flag && nativeArray7.Length != 0)
				{
					Entity meeting = nativeArray7[i].m_Meeting;
					if (m_PrefabRefData.HasComponent(meeting))
					{
						Entity prefab = m_PrefabRefData[meeting].m_Prefab;
						CoordinatedMeeting coordinatedMeeting = m_Meetings[meeting];
						if (m_HaveCoordinatedMeetingDatas.HasBuffer(prefab))
						{
							DynamicBuffer<HaveCoordinatedMeetingData> val2 = m_HaveCoordinatedMeetingDatas[prefab];
							if (val2.Length > coordinatedMeeting.m_Phase)
							{
								HaveCoordinatedMeetingData haveCoordinatedMeetingData = val2[coordinatedMeeting.m_Phase];
								while (trips.Length > 0 && trips[0].m_Purpose != haveCoordinatedMeetingData.m_TravelPurpose.m_Purpose)
								{
									trips.RemoveAt(0);
								}
								if (trips.Length == 0)
								{
									if (flag5)
									{
										((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val, ref m_PathfindTypes);
									}
									continue;
								}
							}
						}
					}
				}
				if ((nativeArray5[i].m_State & CitizenFlags.MovingAwayReachOC) != CitizenFlags.None)
				{
					if (flag5)
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val, ref m_PathfindTypes);
					}
					continue;
				}
				if (flag5)
				{
					if ((pathInformation.m_State & PathFlags.Pending) != 0)
					{
						continue;
					}
					if ((((pathInformation.m_Origin != Entity.Null && pathInformation.m_Origin == pathInformation.m_Destination) || nativeArray3[i].m_CurrentBuilding == pathInformation.m_Destination) && !flag2) || !m_Targets.HasComponent(val))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val, ref m_PathfindTypes);
						RemoveAllTrips(trips);
						continue;
					}
				}
				if (m_DebugDisableSpawning)
				{
					continue;
				}
				PseudoRandomSeed randomSeed;
				if (m_Targets.HasComponent(val))
				{
					Target target = m_Targets[val];
					if (target.m_Target == Entity.Null)
					{
						if (!flag5)
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Target>(unfilteredChunkIndex, val);
							continue;
						}
						Entity destination = pathInformation.m_Destination;
						if (destination == Entity.Null)
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Target>(unfilteredChunkIndex, val);
							RemoveAllTrips(trips);
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val, ref m_PathfindTypes);
							continue;
						}
						target.m_Target = destination;
					}
					Entity val3 = target.m_Target;
					if (m_Properties.TryGetComponent(val3, ref propertyRenter))
					{
						val3 = propertyRenter.m_Property;
					}
					if (currentBuilding == val3 && !flag2)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<Arrived>(unfilteredChunkIndex, val, true);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TravelPurpose>(unfilteredChunkIndex, val, new TravelPurpose
						{
							m_Data = trips[0].m_Data,
							m_Purpose = trips[0].m_Purpose,
							m_Resource = trips[0].m_Resource
						});
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Target>(unfilteredChunkIndex, val);
						if (flag5)
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val, ref m_PathfindTypes);
						}
						RemoveAllTrips(trips);
						continue;
					}
					bool flag6 = (flag3 && trips[0].m_Purpose == Purpose.Deathcare) || (flag4 && trips[0].m_Purpose == Purpose.Hospital);
					if (!flag5 && !flag6)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent(unfilteredChunkIndex, val, ref m_PathfindTypes);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(unfilteredChunkIndex, val, new PathInformation
						{
							m_State = PathFlags.Pending
						});
						Citizen citizen = nativeArray5[i];
						CreatureData creatureData;
						Entity val4 = ObjectEmergeSystem.SelectResidentPrefab(citizen, m_HumanChunks, m_EntityType, ref m_CreatureDataType, ref m_ResidentDataType, out creatureData, out randomSeed);
						HumanData humanData = default(HumanData);
						if (val4 != Entity.Null)
						{
							humanData = m_PrefabHumanData[val4];
						}
						Household household2 = m_Households[household];
						DynamicBuffer<HouseholdCitizen> val5 = m_HouseholdCitizens[household];
						PathfindParameters parameters = new PathfindParameters
						{
							m_MaxSpeed = float2.op_Implicit(277.77777f),
							m_WalkSpeed = float2.op_Implicit(humanData.m_WalkSpeed),
							m_Weights = CitizenUtils.GetPathfindWeights(citizen, household2, val5.Length),
							m_Methods = (PathMethod.Pedestrian | PathMethod.Taxi | RouteUtils.GetPublicTransportMethods(m_TimeOfDay)),
							m_SecondaryIgnoredRules = VehicleUtils.GetIgnoredPathfindRulesTaxiDefaults(),
							m_MaxCost = math.select(CitizenBehaviorSystem.kMaxPathfindCost, CitizenBehaviorSystem.kMaxMovingAwayCost, flag)
						};
						SetupQueueTarget origin = new SetupQueueTarget
						{
							m_Type = SetupTargetType.CurrentLocation,
							m_Methods = PathMethod.Pedestrian,
							m_RandomCost = 30f
						};
						SetupQueueTarget destination2 = new SetupQueueTarget
						{
							m_Type = SetupTargetType.CurrentLocation,
							m_Methods = PathMethod.Pedestrian,
							m_Entity = target.m_Target,
							m_RandomCost = 30f,
							m_ActivityMask = creatureData.m_SupportedActivities
						};
						if (m_PropertyRenters.HasComponent(household))
						{
							parameters.m_Authorization1 = m_PropertyRenters[household].m_Property;
						}
						if (m_Workers.HasComponent(val))
						{
							Worker worker = m_Workers[val];
							if (m_PropertyRenters.HasComponent(worker.m_Workplace))
							{
								parameters.m_Authorization2 = m_PropertyRenters[worker.m_Workplace].m_Property;
							}
							else
							{
								parameters.m_Authorization2 = worker.m_Workplace;
							}
						}
						if (m_CarKeepers.IsComponentEnabled(val))
						{
							Entity car = m_CarKeepers[val].m_Car;
							if (m_ParkedCarData.HasComponent(car))
							{
								PrefabRef prefabRef = m_PrefabRefData[car];
								ParkedCar parkedCar = m_ParkedCarData[car];
								CarData carData = m_PrefabCarData[prefabRef.m_Prefab];
								parameters.m_MaxSpeed.x = carData.m_MaxSpeed;
								parameters.m_ParkingTarget = parkedCar.m_Lane;
								parameters.m_ParkingDelta = parkedCar.m_CurvePosition;
								parameters.m_ParkingSize = VehicleUtils.GetParkingSize(car, ref m_PrefabRefData, ref m_ObjectGeometryData);
								parameters.m_Methods |= VehicleUtils.GetPathMethods(carData) | PathMethod.Parking;
								parameters.m_IgnoredRules = VehicleUtils.GetIgnoredPathfindRules(carData);
								if (m_PersonalCarData.TryGetComponent(car, ref personalCar) && (personalCar.m_State & PersonalCarFlags.HomeTarget) == 0)
								{
									parameters.m_PathfindFlags |= PathfindFlags.ParkingReset;
								}
							}
						}
						SetupQueueItem setupQueueItem = new SetupQueueItem(val, parameters, origin, destination2);
						m_PathQueue.Enqueue(setupQueueItem);
						continue;
					}
					DynamicBuffer<PathElement> val6 = default(DynamicBuffer<PathElement>);
					if (!flag6)
					{
						val6 = m_PathElements[val];
					}
					TripNeeded tripNeeded = trips[0];
					if ((!flag6 && val6.Length > 0) || m_PrefabRefData.HasComponent(tripNeeded.m_TargetAgent))
					{
						Entity currentBuilding2 = nativeArray3[i].m_CurrentBuilding;
						Entity val7 = Entity.Null;
						bool flag7 = m_PropertyRenters.TryGetComponent(household, ref propertyRenter2);
						if (!flag6 && flag7 && ((Entity)(ref currentBuilding2)).Equals(propertyRenter2.m_Property))
						{
							if (pathInformation.m_Destination != Entity.Null)
							{
								if ((pathInformation.m_Methods & (PathMethod.PublicTransportDay | PathMethod.Taxi | PathMethod.PublicTransportNight)) != 0)
								{
									if (m_DebugPathQueuePublic.IsCreated)
									{
										m_DebugPathQueuePublic.Enqueue(Mathf.RoundToInt(pathInformation.m_TotalCost));
									}
									if ((pathInformation.m_Methods & PathMethod.Taxi) != 0)
									{
										if (m_DebugTaxiDuration.IsCreated)
										{
											m_DebugTaxiDuration.Enqueue(Mathf.RoundToInt(pathInformation.m_Duration));
										}
									}
									else if (m_DebugPublicTransportDuration.IsCreated)
									{
										m_DebugPublicTransportDuration.Enqueue(Mathf.RoundToInt(pathInformation.m_Duration));
									}
								}
								else if ((pathInformation.m_Methods & (PathMethod.Road | PathMethod.MediumRoad)) != 0)
								{
									if (m_DebugPathQueueCar.IsCreated)
									{
										m_DebugPathQueueCar.Enqueue(Mathf.RoundToInt(pathInformation.m_TotalCost));
									}
									if (m_DebugCarDuration.IsCreated)
									{
										m_DebugCarDuration.Enqueue(Mathf.RoundToInt(pathInformation.m_Duration));
									}
								}
								else if ((pathInformation.m_Methods & PathMethod.Pedestrian) != 0)
								{
									if (pathInformation.m_Distance > 3000f)
									{
										if (m_DebugPathQueuePedestrian.IsCreated)
										{
											m_DebugPathQueuePedestrian.Enqueue(Mathf.RoundToInt(pathInformation.m_TotalCost));
										}
										if (m_DebugPedestrianDuration.IsCreated)
										{
											m_DebugPedestrianDuration.Enqueue(Mathf.RoundToInt(pathInformation.m_Duration));
										}
									}
									else
									{
										if (m_DebugPathQueuePedestrianShort.IsCreated)
										{
											m_DebugPathQueuePedestrianShort.Enqueue(Mathf.RoundToInt(pathInformation.m_TotalCost));
										}
										if (m_DebugPedestrianDurationShort.IsCreated)
										{
											m_DebugPedestrianDurationShort.Enqueue(Mathf.RoundToInt(pathInformation.m_Duration));
										}
									}
								}
							}
							if (tripNeeded.m_Purpose == Purpose.GoingToWork && m_Workers.HasComponent(val))
							{
								if (pathInformation.m_Destination == Entity.Null)
								{
									((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Worker>(unfilteredChunkIndex, val);
								}
								else
								{
									Worker worker2 = m_Workers[val];
									worker2.m_LastCommuteTime = pathInformation.m_Duration;
									m_Workers[val] = worker2;
								}
							}
							else if (tripNeeded.m_Purpose == Purpose.GoingToSchool && m_Students.HasComponent(val))
							{
								if (pathInformation.m_Destination == Entity.Null)
								{
									((ParallelWriter)(ref m_CommandBuffer)).AddComponent<StudentsRemoved>(unfilteredChunkIndex, m_Students[val].m_School);
									((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Citizens.Student>(unfilteredChunkIndex, val);
								}
								else
								{
									Game.Citizens.Student student = m_Students[val];
									student.m_LastCommuteTime = pathInformation.m_Duration;
									m_Students[val] = student;
								}
							}
						}
						ResidentFlags residentFlags = ResidentFlags.None;
						if (nativeArray7.Length > 0)
						{
							Entity meeting2 = nativeArray7[i].m_Meeting;
							if (m_PrefabRefData.HasComponent(meeting2))
							{
								CoordinatedMeeting coordinatedMeeting2 = m_Meetings[meeting2];
								DynamicBuffer<HaveCoordinatedMeetingData> val8 = m_HaveCoordinatedMeetingDatas[m_PrefabRefData[meeting2].m_Prefab];
								if (coordinatedMeeting2.m_Status == MeetingStatus.Done)
								{
									continue;
								}
								HaveCoordinatedMeetingData haveCoordinatedMeetingData2 = val8[coordinatedMeeting2.m_Phase];
								if (tripNeeded.m_Purpose == haveCoordinatedMeetingData2.m_TravelPurpose.m_Purpose && (haveCoordinatedMeetingData2.m_TravelPurpose.m_Resource == Resource.NoResource || haveCoordinatedMeetingData2.m_TravelPurpose.m_Resource == tripNeeded.m_Resource) && coordinatedMeeting2.m_Target == Entity.Null)
								{
									DynamicBuffer<CoordinatedMeetingAttendee> val9 = m_Attendees[meeting2];
									if (val9.Length <= 0 || !(val9[0].m_Attendee == val))
									{
										continue;
									}
									coordinatedMeeting2.m_Target = target.m_Target;
									m_Meetings[meeting2] = coordinatedMeeting2;
									residentFlags |= ResidentFlags.PreferredLeader;
								}
							}
						}
						if (m_Workers.HasComponent(val))
						{
							Worker worker3 = m_Workers[val];
							val7 = ((!m_PropertyRenters.HasComponent(worker3.m_Workplace)) ? worker3.m_Workplace : m_PropertyRenters[worker3.m_Workplace].m_Property);
						}
						if (((Entity)(ref currentBuilding2)).Equals(propertyRenter2.m_Property) || ((Entity)(ref currentBuilding2)).Equals(val7))
						{
							m_LeaveQueue.Enqueue(val);
						}
						Entity val10 = Entity.Null;
						if (nativeArray4.Length != 0)
						{
							val10 = nativeArray4[i].m_CurrentTransport;
						}
						uint timer = 512u;
						Purpose divertPurpose = Purpose.None;
						bool pathFailed = !flag6 && val6.Length == 0;
						bool hasDivertPath = false;
						GetResidentFlags(val, currentBuilding2, isMailSender, pathFailed, ref target, ref tripNeeded.m_Purpose, ref divertPurpose, ref timer, ref hasDivertPath);
						if (m_UnderConstructionData.TryGetComponent(val3, ref underConstruction) && underConstruction.m_NewPrefab == Entity.Null)
						{
							timer = math.max(timer, ObjectUtils.GetTripDelayFrames(underConstruction, pathInformation));
						}
						if (m_PrefabRefData.HasComponent(val10) && !m_Deleteds.HasComponent(val10))
						{
							ResetTrip(unfilteredChunkIndex, val10, val, currentBuilding, target, residentFlags, divertPurpose, timer, hasDivertPath);
						}
						else
						{
							Citizen citizenData = nativeArray5[i];
							val10 = SpawnResident(unfilteredChunkIndex, val, currentBuilding, citizenData, target, residentFlags, divertPurpose, timer, hasDivertPath, flag3, flag6);
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentTransport>(unfilteredChunkIndex, val, new CurrentTransport(val10));
						}
						if ((tripNeeded.m_Purpose != Purpose.GoingToWork && tripNeeded.m_Purpose != Purpose.GoingToSchool) || currentBuilding != propertyRenter2.m_Property)
						{
							AddPetTargets(household, currentBuilding, target.m_Target);
						}
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TravelPurpose>(unfilteredChunkIndex, val, new TravelPurpose
						{
							m_Data = tripNeeded.m_Data,
							m_Purpose = tripNeeded.m_Purpose,
							m_Resource = tripNeeded.m_Resource
						});
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentBuilding>(unfilteredChunkIndex, val);
					}
					else if ((m_Households[household].m_Flags & HouseholdFlags.MovedIn) == 0)
					{
						CitizenUtils.HouseholdMoveAway(m_CommandBuffer, unfilteredChunkIndex, household);
					}
					RemoveAllTrips(trips);
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val, ref m_PathfindTypes);
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Target>(unfilteredChunkIndex, val);
				}
				else
				{
					if (flag5 || m_HumanChunks.Length == 0)
					{
						continue;
					}
					if (!m_Transforms.HasComponent(currentBuilding))
					{
						RemoveAllTrips(trips);
					}
					else if (trips[0].m_TargetAgent != Entity.Null)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Target>(unfilteredChunkIndex, val, new Target
						{
							m_Target = trips[0].m_TargetAgent
						});
					}
					else if (PathUtils.IsPathfindingPurpose(trips[0].m_Purpose))
					{
						Citizen citizen2 = nativeArray5[i];
						if (trips[0].m_Purpose == Purpose.GoingHome)
						{
							if ((citizen2.m_State & CitizenFlags.Commuter) == 0)
							{
								RemoveAllTrips(trips);
								continue;
							}
							if (m_OutsideConnections.HasComponent(nativeArray3[i].m_CurrentBuilding))
							{
								RemoveAllTrips(trips);
								continue;
							}
						}
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent(unfilteredChunkIndex, val, ref m_PathfindTypes);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(unfilteredChunkIndex, val, new PathInformation
						{
							m_State = PathFlags.Pending
						});
						CreatureData creatureData2;
						Entity val11 = ObjectEmergeSystem.SelectResidentPrefab(citizen2, m_HumanChunks, m_EntityType, ref m_CreatureDataType, ref m_ResidentDataType, out creatureData2, out randomSeed);
						HumanData humanData2 = default(HumanData);
						if (val11 != Entity.Null)
						{
							humanData2 = m_PrefabHumanData[val11];
						}
						Household household3 = m_Households[household];
						DynamicBuffer<HouseholdCitizen> val12 = m_HouseholdCitizens[household];
						PathfindParameters parameters2 = new PathfindParameters
						{
							m_MaxSpeed = float2.op_Implicit(277.77777f),
							m_WalkSpeed = float2.op_Implicit(humanData2.m_WalkSpeed),
							m_Weights = CitizenUtils.GetPathfindWeights(citizen2, household3, val12.Length),
							m_Methods = (PathMethod.Pedestrian | PathMethod.Taxi | RouteUtils.GetPublicTransportMethods(m_TimeOfDay)),
							m_SecondaryIgnoredRules = VehicleUtils.GetIgnoredPathfindRulesTaxiDefaults(),
							m_MaxCost = CitizenBehaviorSystem.kMaxPathfindCost
						};
						SetupQueueTarget origin2 = new SetupQueueTarget
						{
							m_Type = SetupTargetType.CurrentLocation,
							m_Methods = PathMethod.Pedestrian,
							m_RandomCost = 30f
						};
						SetupQueueTarget destination3 = new SetupQueueTarget
						{
							m_Methods = PathMethod.Pedestrian,
							m_RandomCost = 30f,
							m_ActivityMask = creatureData2.m_SupportedActivities
						};
						switch (trips[0].m_Purpose)
						{
						case Purpose.GoingHome:
							destination3.m_Type = SetupTargetType.OutsideConnection;
							break;
						case Purpose.Hospital:
							destination3.m_Entity = FindDistrict(currentBuilding);
							destination3.m_Type = SetupTargetType.Hospital;
							break;
						case Purpose.Safety:
						case Purpose.Escape:
							destination3.m_Type = SetupTargetType.Safety;
							break;
						case Purpose.EmergencyShelter:
							parameters2.m_Weights = new PathfindWeights(1f, 0f, 0f, 0f);
							destination3.m_Entity = FindDistrict(currentBuilding);
							destination3.m_Type = SetupTargetType.EmergencyShelter;
							break;
						case Purpose.Crime:
							destination3.m_Type = SetupTargetType.CrimeProducer;
							break;
						case Purpose.Sightseeing:
							destination3.m_Type = SetupTargetType.Sightseeing;
							break;
						case Purpose.VisitAttractions:
							destination3.m_Type = SetupTargetType.Attraction;
							break;
						}
						if (m_PropertyRenters.HasComponent(household))
						{
							parameters2.m_Authorization1 = m_PropertyRenters[household].m_Property;
						}
						if (m_Workers.HasComponent(val))
						{
							Worker worker4 = m_Workers[val];
							if (m_PropertyRenters.HasComponent(worker4.m_Workplace))
							{
								parameters2.m_Authorization2 = m_PropertyRenters[worker4.m_Workplace].m_Property;
							}
							else
							{
								parameters2.m_Authorization2 = worker4.m_Workplace;
							}
						}
						if (m_CarKeepers.IsComponentEnabled(val))
						{
							Entity car2 = m_CarKeepers[val].m_Car;
							if (m_ParkedCarData.HasComponent(car2))
							{
								PrefabRef prefabRef2 = m_PrefabRefData[car2];
								ParkedCar parkedCar2 = m_ParkedCarData[car2];
								CarData carData2 = m_PrefabCarData[prefabRef2.m_Prefab];
								parameters2.m_MaxSpeed.x = carData2.m_MaxSpeed;
								parameters2.m_ParkingTarget = parkedCar2.m_Lane;
								parameters2.m_ParkingDelta = parkedCar2.m_CurvePosition;
								parameters2.m_ParkingSize = VehicleUtils.GetParkingSize(car2, ref m_PrefabRefData, ref m_ObjectGeometryData);
								parameters2.m_Methods |= VehicleUtils.GetPathMethods(carData2) | PathMethod.Parking;
								parameters2.m_IgnoredRules = VehicleUtils.GetIgnoredPathfindRules(carData2);
								if (m_PersonalCarData.TryGetComponent(car2, ref personalCar2) && (personalCar2.m_State & PersonalCarFlags.HomeTarget) == 0)
								{
									parameters2.m_PathfindFlags |= PathfindFlags.ParkingReset;
								}
							}
						}
						SetupQueueItem setupQueueItem2 = new SetupQueueItem(val, parameters2, origin2, destination3);
						m_PathQueue.Enqueue(setupQueueItem2);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Target>(unfilteredChunkIndex, val, new Target
						{
							m_Target = Entity.Null
						});
					}
					else
					{
						RemoveAllTrips(trips);
					}
				}
			}
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
		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MailSender> __Game_Citizens_MailSender_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentTypeHandle;

		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RW_ComponentTypeHandle;

		public BufferTypeHandle<TripNeeded> __Game_Citizens_TripNeeded_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AttendingMeeting> __Game_Citizens_AttendingMeeting_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CreatureData> __Game_Prefabs_CreatureData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ResidentData> __Game_Prefabs_ResidentData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PersonalCar> __Game_Vehicles_PersonalCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Ambulance> __Game_Vehicles_Ambulance_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<CarKeeper> __Game_Citizens_CarKeeper_RO_ComponentLookup;

		public ComponentLookup<Worker> __Game_Citizens_Worker_RW_ComponentLookup;

		public ComponentLookup<Game.Citizens.Student> __Game_Citizens_Student_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectData> __Game_Prefabs_ObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HumanData> __Game_Prefabs_HumanData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> __Game_Objects_UnderConstruction_RO_ComponentLookup;

		public ComponentLookup<CoordinatedMeeting> __Game_Citizens_CoordinatedMeeting_RW_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CoordinatedMeetingAttendee> __Game_Citizens_CoordinatedMeetingAttendee_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> __Game_Citizens_HouseholdAnimal_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HaveCoordinatedMeetingData> __Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		public ComponentLookup<CitizenPresence> __Game_Buildings_CitizenPresence_RW_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<DeliveryTruckData> __Game_Prefabs_DeliveryTruckData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportCompanyData> __Game_Companies_TransportCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

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
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_Citizen_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(true);
			__Game_Citizens_HealthProblem_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HealthProblem>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdMember>(true);
			__Game_Citizens_MailSender_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MailSender>(true);
			__Game_Citizens_CurrentTransport_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentTransport>(true);
			__Game_Citizens_CurrentBuilding_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(false);
			__Game_Citizens_TripNeeded_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TripNeeded>(false);
			__Game_Citizens_AttendingMeeting_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AttendingMeeting>(true);
			__Game_Prefabs_CreatureData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreatureData>(true);
			__Game_Prefabs_ResidentData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResidentData>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_PersonalCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PersonalCar>(true);
			__Game_Vehicles_Ambulance_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Ambulance>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentDistrict>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
			__Game_Citizens_CarKeeper_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarKeeper>(true);
			__Game_Citizens_Worker_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(false);
			__Game_Citizens_Student_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Citizens.Student>(false);
			__Game_Prefabs_ObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_HumanData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanData>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Objects_UnderConstruction_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UnderConstruction>(true);
			__Game_Citizens_CoordinatedMeeting_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CoordinatedMeeting>(false);
			__Game_Citizens_CoordinatedMeetingAttendee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CoordinatedMeetingAttendee>(true);
			__Game_Citizens_HouseholdAnimal_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdAnimal>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HaveCoordinatedMeetingData>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Buildings_CitizenPresence_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CitizenPresence>(false);
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<OwnedVehicle>(true);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Prefabs_DeliveryTruckData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DeliveryTruckData>(true);
			__Game_Companies_TransportCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportCompanyData>(true);
			__Game_Simulation_ServiceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
		}
	}

	private const int UPDATE_INTERVAL = 16;

	private EntityQuery m_CitizenGroup;

	private EntityQuery m_ResidentPrefabGroup;

	private EntityQuery m_CompanyGroup;

	private EntityArchetype m_HandleRequestArchetype;

	private EntityArchetype m_ResetTripArchetype;

	private ComponentTypeSet m_HumanSpawnTypes;

	private ComponentTypeSet m_PathfindTypes;

	private ComponentTypeSet m_CurrentLaneTypesRelative;

	private EndFrameBarrier m_EndFrameBarrier;

	private TimeSystem m_TimeSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private VehicleCapacitySystem m_VehicleCapacitySystem;

	[DebugWatchValue]
	private DebugWatchDistribution m_DebugPathCostsCar;

	[DebugWatchValue]
	private DebugWatchDistribution m_DebugPathCostsPublic;

	[DebugWatchValue]
	private DebugWatchDistribution m_DebugPathCostsPedestrian;

	[DebugWatchValue]
	private DebugWatchDistribution m_DebugPathCostsCarShort;

	[DebugWatchValue]
	private DebugWatchDistribution m_DebugPathCostsPublicShort;

	[DebugWatchValue]
	private DebugWatchDistribution m_DebugPathCostsPedestrianShort;

	[DebugWatchValue]
	private DebugWatchDistribution m_DebugPublicTransportDuration;

	[DebugWatchValue]
	private DebugWatchDistribution m_DebugTaxiDuration;

	[DebugWatchValue]
	private DebugWatchDistribution m_DebugPedestrianDuration;

	[DebugWatchValue]
	private DebugWatchDistribution m_DebugCarDuration;

	[DebugWatchValue]
	private DebugWatchDistribution m_DebugPedestrianDurationShort;

	private TypeHandle __TypeHandle;

	public bool debugDisableSpawning { get; set; }

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_DebugPathCostsCar = new DebugWatchDistribution(persistent: true);
		m_DebugPathCostsPublic = new DebugWatchDistribution(persistent: true);
		m_DebugPathCostsPedestrian = new DebugWatchDistribution(persistent: true);
		m_DebugPathCostsCarShort = new DebugWatchDistribution(persistent: true);
		m_DebugPathCostsPublicShort = new DebugWatchDistribution(persistent: true);
		m_DebugPathCostsPedestrianShort = new DebugWatchDistribution(persistent: true);
		m_DebugPublicTransportDuration = new DebugWatchDistribution(persistent: true);
		m_DebugTaxiDuration = new DebugWatchDistribution(persistent: true);
		m_DebugPedestrianDuration = new DebugWatchDistribution(persistent: true);
		m_DebugCarDuration = new DebugWatchDistribution(persistent: true);
		m_DebugPedestrianDurationShort = new DebugWatchDistribution(persistent: true);
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_VehicleCapacitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehicleCapacitySystem>();
		m_CitizenGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<HouseholdMember>(),
			ComponentType.ReadWrite<TripNeeded>(),
			ComponentType.Exclude<TravelPurpose>(),
			ComponentType.ReadOnly<CurrentBuilding>(),
			ComponentType.Exclude<ResourceBuyer>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_ResidentPrefabGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<HumanData>(),
			ComponentType.ReadOnly<ResidentData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_CompanyGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<TripNeeded>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.ReadOnly<OwnedVehicle>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_HandleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<HandleRequest>(),
			ComponentType.ReadWrite<Game.Events.Event>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_ResetTripArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<ResetTrip>()
		});
		m_HumanSpawnTypes = new ComponentTypeSet(ComponentType.ReadWrite<HumanCurrentLane>(), ComponentType.ReadWrite<TripSource>(), ComponentType.ReadWrite<Unspawned>());
		m_PathfindTypes = new ComponentTypeSet(ComponentType.ReadWrite<PathInformation>(), ComponentType.ReadWrite<PathElement>());
		m_CurrentLaneTypesRelative = new ComponentTypeSet((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<HumanNavigation>(),
			ComponentType.ReadWrite<HumanCurrentLane>(),
			ComponentType.ReadWrite<Blocker>()
		});
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_CitizenGroup, m_CompanyGroup });
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_DebugPathCostsCar.Dispose();
		m_DebugPathCostsPublic.Dispose();
		m_DebugPathCostsPedestrian.Dispose();
		m_DebugPathCostsCarShort.Dispose();
		m_DebugPathCostsPublicShort.Dispose();
		m_DebugPathCostsPedestrianShort.Dispose();
		m_DebugPublicTransportDuration.Dispose();
		m_DebugTaxiDuration.Dispose();
		m_DebugCarDuration.Dispose();
		m_DebugPedestrianDuration.Dispose();
		m_DebugPedestrianDurationShort.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0917: Unknown result type (might be due to invalid IL or missing references)
		//IL_091c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0934: Unknown result type (might be due to invalid IL or missing references)
		//IL_0939: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_0956: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0973: Unknown result type (might be due to invalid IL or missing references)
		//IL_098b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0990: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_076d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_077a: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_081d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		//IL_0868: Unknown result type (might be due to invalid IL or missing references)
		//IL_086f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0871: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_088f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08af: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> humanChunks = ((EntityQuery)(ref m_ResidentPrefabGroup)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = JobHandle.CombineDependencies(((SystemBase)this).Dependency, val);
		JobHandle val3 = default(JobHandle);
		EntityCommandBuffer val4;
		if (!((EntityQuery)(ref m_CitizenGroup)).IsEmptyIgnoreFilter)
		{
			NativeQueue<AnimalTargetInfo> animalQueue = default(NativeQueue<AnimalTargetInfo>);
			animalQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<Entity> leaveQueue = default(NativeQueue<Entity>);
			leaveQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<int> debugPathQueueCar = default(NativeQueue<int>);
			NativeQueue<int> debugPathQueuePublic = default(NativeQueue<int>);
			NativeQueue<int> debugPathQueuePedestrian = default(NativeQueue<int>);
			NativeQueue<int> debugPathQueueCarShort = default(NativeQueue<int>);
			NativeQueue<int> debugPathQueuePublicShort = default(NativeQueue<int>);
			NativeQueue<int> debugPathQueuePedestrianShort = default(NativeQueue<int>);
			NativeQueue<int> debugPublicTransportDuration = default(NativeQueue<int>);
			NativeQueue<int> debugTaxiDuration = default(NativeQueue<int>);
			NativeQueue<int> debugCarDuration = default(NativeQueue<int>);
			NativeQueue<int> debugPedestrianDuration = default(NativeQueue<int>);
			NativeQueue<int> debugPedestrianDurationShort = default(NativeQueue<int>);
			JobHandle deps = default(JobHandle);
			if (m_DebugPathCostsCar.IsEnabled)
			{
				debugPathQueueCar = m_DebugPathCostsCar.GetQueue(clear: false, out deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (m_DebugPathCostsPublic.IsEnabled)
			{
				debugPathQueuePublic = m_DebugPathCostsPublic.GetQueue(clear: false, out deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (m_DebugPathCostsPedestrian.IsEnabled)
			{
				debugPathQueuePedestrian = m_DebugPathCostsPedestrian.GetQueue(clear: false, out deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (m_DebugPathCostsCarShort.IsEnabled)
			{
				debugPathQueueCarShort = m_DebugPathCostsCarShort.GetQueue(clear: false, out deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (m_DebugPathCostsPublicShort.IsEnabled)
			{
				debugPathQueuePublicShort = m_DebugPathCostsPublicShort.GetQueue(clear: false, out deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (m_DebugPathCostsPedestrianShort.IsEnabled)
			{
				debugPathQueuePedestrianShort = m_DebugPathCostsPedestrianShort.GetQueue(clear: false, out deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (m_DebugPublicTransportDuration.IsEnabled)
			{
				debugPublicTransportDuration = m_DebugPublicTransportDuration.GetQueue(clear: false, out deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (m_DebugTaxiDuration.IsEnabled)
			{
				debugTaxiDuration = m_DebugTaxiDuration.GetQueue(clear: false, out deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (m_DebugCarDuration.IsEnabled)
			{
				debugCarDuration = m_DebugCarDuration.GetQueue(clear: false, out deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (m_DebugPedestrianDuration.IsEnabled)
			{
				debugPedestrianDuration = m_DebugPedestrianDuration.GetQueue(clear: false, out deps);
				((JobHandle)(ref deps)).Complete();
			}
			if (m_DebugPedestrianDurationShort.IsEnabled)
			{
				debugPedestrianDurationShort = m_DebugPedestrianDurationShort.GetQueue(clear: false, out deps);
				((JobHandle)(ref deps)).Complete();
			}
			CitizenJob citizenJob = new CitizenJob
			{
				m_DebugPathQueueCar = debugPathQueueCar,
				m_DebugPathQueuePublic = debugPathQueuePublic,
				m_DebugPathQueuePedestrian = debugPathQueuePedestrian,
				m_DebugPathQueueCarShort = debugPathQueueCarShort,
				m_DebugPathQueuePublicShort = debugPathQueuePublicShort,
				m_DebugPathQueuePedestrianShort = debugPathQueuePedestrianShort,
				m_DebugPublicTransportDuration = debugPublicTransportDuration,
				m_DebugTaxiDuration = debugTaxiDuration,
				m_DebugCarDuration = debugCarDuration,
				m_DebugPedestrianDuration = debugPedestrianDuration,
				m_DebugPedestrianDurationShort = debugPedestrianDurationShort,
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HealthProblemType = InternalCompilerInterface.GetComponentTypeHandle<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdMemberType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MailSenderType = InternalCompilerInterface.GetComponentTypeHandle<MailSender>(ref __TypeHandle.__Game_Citizens_MailSender_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentTransportType = InternalCompilerInterface.GetComponentTypeHandle<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TripNeededType = InternalCompilerInterface.GetBufferTypeHandle<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AttendingMeetingType = InternalCompilerInterface.GetComponentTypeHandle<AttendingMeeting>(ref __TypeHandle.__Game_Citizens_AttendingMeeting_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CreatureDataType = InternalCompilerInterface.GetComponentTypeHandle<CreatureData>(ref __TypeHandle.__Game_Prefabs_CreatureData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ResidentDataType = InternalCompilerInterface.GetComponentTypeHandle<ResidentData>(ref __TypeHandle.__Game_Prefabs_ResidentData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PersonalCarData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AmbulanceData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Ambulance>(ref __TypeHandle.__Game_Vehicles_Ambulance_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentDistrictData = InternalCompilerInterface.GetComponentLookup<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathInfos = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Properties = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Targets = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Deleteds = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarKeepers = InternalCompilerInterface.GetComponentLookup<CarKeeper>(ref __TypeHandle.__Game_Citizens_CarKeeper_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Students = InternalCompilerInterface.GetComponentLookup<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectDatas = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabHumanData = InternalCompilerInterface.GetComponentLookup<HumanData>(ref __TypeHandle.__Game_Prefabs_HumanData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UnderConstructionData = InternalCompilerInterface.GetComponentLookup<UnderConstruction>(ref __TypeHandle.__Game_Objects_UnderConstruction_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Meetings = InternalCompilerInterface.GetComponentLookup<CoordinatedMeeting>(ref __TypeHandle.__Game_Citizens_CoordinatedMeeting_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Attendees = InternalCompilerInterface.GetBufferLookup<CoordinatedMeetingAttendee>(ref __TypeHandle.__Game_Citizens_CoordinatedMeetingAttendee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdAnimals = InternalCompilerInterface.GetBufferLookup<HouseholdAnimal>(ref __TypeHandle.__Game_Citizens_HouseholdAnimal_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TravelPurposes = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HaveCoordinatedMeetingDatas = InternalCompilerInterface.GetBufferLookup<HaveCoordinatedMeetingData>(ref __TypeHandle.__Game_Prefabs_HaveCoordinatedMeetingData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HumanChunks = humanChunks,
				m_RandomSeed = RandomSeed.Next(),
				m_TimeOfDay = m_TimeSystem.normalizedTime,
				m_ResetTripArchetype = m_ResetTripArchetype,
				m_HumanSpawnTypes = m_HumanSpawnTypes,
				m_PathfindTypes = m_PathfindTypes,
				m_PathQueue = m_PathfindSetupSystem.GetQueue(this, 80, 16).AsParallelWriter(),
				m_AnimalQueue = animalQueue.AsParallelWriter(),
				m_LeaveQueue = leaveQueue.AsParallelWriter()
			};
			val4 = m_EndFrameBarrier.CreateCommandBuffer();
			citizenJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val4)).AsParallelWriter();
			citizenJob.m_DebugDisableSpawning = debugDisableSpawning;
			CitizenJob citizenJob2 = citizenJob;
			PetTargetJob petTargetJob = new PetTargetJob
			{
				m_CurrentBuildingData = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AnimalQueue = animalQueue,
				m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
			};
			CitizeLeaveJob obj = new CitizeLeaveJob
			{
				m_CurrentBuildingData = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CitizenPresenceData = InternalCompilerInterface.GetComponentLookup<CitizenPresence>(ref __TypeHandle.__Game_Buildings_CitizenPresence_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LeaveQueue = leaveQueue
			};
			val2 = JobChunkExtensions.ScheduleParallel<CitizenJob>(citizenJob2, m_CitizenGroup, val2);
			JobHandle val5 = IJobExtensions.Schedule<PetTargetJob>(petTargetJob, val2);
			JobHandle val6 = IJobExtensions.Schedule<CitizeLeaveJob>(obj, val2);
			val3 = JobHandle.CombineDependencies(val5, val6);
			animalQueue.Dispose(val5);
			leaveQueue.Dispose(val6);
			m_PathfindSetupSystem.AddQueueWriter(val2);
			m_EndFrameBarrier.AddJobHandleForProducer(val3);
		}
		if (!((EntityQuery)(ref m_CompanyGroup)).IsEmptyIgnoreFilter)
		{
			CompanyJob companyJob = new CompanyJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenterType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CreatureDataType = InternalCompilerInterface.GetComponentTypeHandle<CreatureData>(ref __TypeHandle.__Game_Prefabs_CreatureData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ResidentDataType = InternalCompilerInterface.GetComponentTypeHandle<ResidentData>(ref __TypeHandle.__Game_Prefabs_ResidentData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TripNeededType = InternalCompilerInterface.GetBufferTypeHandle<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_VehicleType = InternalCompilerInterface.GetBufferTypeHandle<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabDeliveryTruckData = InternalCompilerInterface.GetComponentLookup<DeliveryTruckData>(ref __TypeHandle.__Game_Prefabs_DeliveryTruckData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabObjectData = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransportCompanyDatas = InternalCompilerInterface.GetComponentLookup<TransportCompanyData>(ref __TypeHandle.__Game_Companies_TransportCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UnderConstructionData = InternalCompilerInterface.GetComponentLookup<UnderConstruction>(ref __TypeHandle.__Game_Objects_UnderConstruction_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenterData = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ActivityLocationElements = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HumanChunks = humanChunks,
				m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
				m_RandomSeed = RandomSeed.Next(),
				m_HandleRequestArchetype = m_HandleRequestArchetype,
				m_DeliveryTruckSelectData = m_VehicleCapacitySystem.GetDeliveryTruckSelectData(),
				m_CurrentLaneTypesRelative = m_CurrentLaneTypesRelative
			};
			val4 = m_EndFrameBarrier.CreateCommandBuffer();
			companyJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val4)).AsParallelWriter();
			companyJob.m_DebugDisableSpawning = debugDisableSpawning;
			val2 = JobChunkExtensions.ScheduleParallel<CompanyJob>(companyJob, m_CompanyGroup, val2);
			m_EndFrameBarrier.AddJobHandleForProducer(val2);
			val3 = JobHandle.CombineDependencies(val3, val2);
		}
		humanChunks.Dispose(val2);
		((SystemBase)this).Dependency = val3;
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
	public TripNeededSystem()
	{
	}
}

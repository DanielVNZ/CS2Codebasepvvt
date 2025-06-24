using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
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
public class PostFacilityAISystem : GameSystemBase
{
	private struct PostFacilityAction
	{
		public Entity m_Entity;

		public bool m_Disabled;

		public static PostFacilityAction SetDisabled(Entity vehicle, bool disabled)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return new PostFacilityAction
			{
				m_Entity = vehicle,
				m_Disabled = disabled
			};
		}
	}

	[BurstCompile]
	private struct PostFacilityTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		public ComponentTypeHandle<Game.Buildings.PostFacility> m_PostFacilityType;

		public ComponentTypeHandle<Game.Routes.MailBox> m_MailBoxType;

		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		public BufferTypeHandle<Resources> m_ResourcesType;

		public BufferTypeHandle<OwnedVehicle> m_OwnedVehicleType;

		public BufferTypeHandle<GuestVehicle> m_GuestVehicleType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<Target> m_TargetData;

		[ReadOnly]
		public ComponentLookup<PostVanRequest> m_PostVanRequestData;

		[ReadOnly]
		public ComponentLookup<MailTransferRequest> m_MailTransferRequestData;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PostVan> m_PostVanData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTruckData;

		[ReadOnly]
		public ComponentLookup<ReturnLoad> m_ReturnLoadData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<PostFacilityData> m_PrefabPostFacilityData;

		[ReadOnly]
		public ComponentLookup<MailBoxData> m_PrefabMailBoxData;

		[ReadOnly]
		public ComponentLookup<PostVanData> m_PrefabPostVanData;

		[ReadOnly]
		public ComponentLookup<DeliveryTruckData> m_PrefabDeliveryTruckData;

		[ReadOnly]
		public ComponentLookup<ObjectData> m_PrefabObjectData;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public PostVanSelectData m_PostVanSelectData;

		[ReadOnly]
		public DeliveryTruckSelectData m_DeliveryTruckSelectData;

		[ReadOnly]
		public PostConfigurationData m_PostConfigurationData;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		[ReadOnly]
		public EntityArchetype m_PostVanRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_MailTransferRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingCarAddTypes;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<PostFacilityAction> m_ActionQueue;

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
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Game.Buildings.PostFacility> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.PostFacility>(ref m_PostFacilityType);
			NativeArray<Game.Routes.MailBox> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Routes.MailBox>(ref m_MailBoxType);
			BufferAccessor<Efficiency> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			BufferAccessor<OwnedVehicle> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedVehicle>(ref m_OwnedVehicleType);
			BufferAccessor<GuestVehicle> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<GuestVehicle>(ref m_GuestVehicleType);
			BufferAccessor<ServiceDispatch> bufferAccessor5 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			BufferAccessor<Resources> bufferAccessor6 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourcesType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			Game.Routes.MailBox mailBox = default(Game.Routes.MailBox);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity entity = nativeArray[i];
				PrefabRef prefabRef = nativeArray2[i];
				Game.Buildings.PostFacility postFacility = nativeArray3[i];
				DynamicBuffer<OwnedVehicle> ownedVehicles = bufferAccessor3[i];
				DynamicBuffer<GuestVehicle> guestVehicles = bufferAccessor4[i];
				DynamicBuffer<ServiceDispatch> dispatches = bufferAccessor5[i];
				DynamicBuffer<Resources> resources = bufferAccessor6[i];
				PostFacilityData data = m_PrefabPostFacilityData[prefabRef.m_Prefab];
				MailBoxData prefabMailBoxData = default(MailBoxData);
				if (bufferAccessor2.Length != 0)
				{
					UpgradeUtils.CombineStats<PostFacilityData>(ref data, bufferAccessor2[i], ref m_PrefabRefData, ref m_PrefabPostFacilityData);
				}
				float efficiency = BuildingUtils.GetEfficiency(bufferAccessor, i);
				float immediateEfficiency = BuildingUtils.GetImmediateEfficiency(bufferAccessor, i);
				if (CollectionUtils.TryGet<Game.Routes.MailBox>(nativeArray4, i, ref mailBox))
				{
					m_PrefabMailBoxData.TryGetComponent(prefabRef.m_Prefab, ref prefabMailBoxData);
				}
				Tick(unfilteredChunkIndex, entity, ref random, ref postFacility, ref mailBox, data, prefabMailBoxData, ownedVehicles, guestVehicles, dispatches, resources, efficiency, immediateEfficiency);
				nativeArray3[i] = postFacility;
				CollectionUtils.TrySet<Game.Routes.MailBox>(nativeArray4, i, mailBox);
			}
		}

		private unsafe void Tick(int jobIndex, Entity entity, ref Random random, ref Game.Buildings.PostFacility postFacility, ref Game.Routes.MailBox mailBox, PostFacilityData prefabPostFacilityData, MailBoxData prefabMailBoxData, DynamicBuffer<OwnedVehicle> ownedVehicles, DynamicBuffer<GuestVehicle> guestVehicles, DynamicBuffer<ServiceDispatch> dispatches, DynamicBuffer<Resources> resources, float efficiency, float immediateEfficiency)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd9: Unknown result type (might be due to invalid IL or missing references)
			//IL_078a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0799: Unknown result type (might be due to invalid IL or missing references)
			//IL_0754: Unknown result type (might be due to invalid IL or missing references)
			//IL_0768: Unknown result type (might be due to invalid IL or missing references)
			//IL_0779: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0835: Unknown result type (might be due to invalid IL or missing references)
			//IL_083a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0842: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0879: Unknown result type (might be due to invalid IL or missing references)
			//IL_084e: Unknown result type (might be due to invalid IL or missing references)
			//IL_084f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0851: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0885: Unknown result type (might be due to invalid IL or missing references)
			//IL_0886: Unknown result type (might be due to invalid IL or missing references)
			//IL_0888: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f59: Unknown result type (might be due to invalid IL or missing references)
			//IL_096c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ace: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09de: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_099b: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff5: Unknown result type (might be due to invalid IL or missing references)
			int vehicleCapacity = BuildingUtils.GetVehicleCapacity(efficiency, prefabPostFacilityData.m_PostVanCapacity);
			int num = BuildingUtils.GetVehicleCapacity(immediateEfficiency, prefabPostFacilityData.m_PostVanCapacity);
			int availableDeliveryVans = vehicleCapacity;
			int availableDeliveryTrucks = BuildingUtils.GetVehicleCapacity(efficiency, prefabPostFacilityData.m_PostTruckCapacity);
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int length = ownedVehicles.Length;
			StackList<Entity> parkedPostVans = StackList<Entity>.op_Implicit(new Span<Entity>((void*)stackalloc Entity[length], length));
			Game.Vehicles.PostVan postVan = default(Game.Vehicles.PostVan);
			ParkedCar parkedCar = default(ParkedCar);
			Game.Vehicles.DeliveryTruck deliveryTruck = default(Game.Vehicles.DeliveryTruck);
			DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
			Game.Vehicles.DeliveryTruck deliveryTruck2 = default(Game.Vehicles.DeliveryTruck);
			ReturnLoad returnLoad = default(ReturnLoad);
			ReturnLoad returnLoad2 = default(ReturnLoad);
			for (int i = 0; i < ownedVehicles.Length; i++)
			{
				Entity vehicle = ownedVehicles[i].m_Vehicle;
				if (m_PostVanData.TryGetComponent(vehicle, ref postVan))
				{
					if (m_ParkedCarData.TryGetComponent(vehicle, ref parkedCar))
					{
						if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(parkedCar.m_Lane))
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicle);
						}
						else
						{
							parkedPostVans.AddNoResize(vehicle);
						}
						continue;
					}
					PrefabRef prefabRef = m_PrefabRefData[vehicle];
					PostVanData postVanData = m_PrefabPostVanData[prefabRef.m_Prefab];
					availableDeliveryVans--;
					num3 += postVan.m_DeliveringMail;
					num2 += postVanData.m_MailCapacity;
					bool flag = --num < 0;
					if ((postVan.m_State & PostVanFlags.Disabled) != 0 != flag)
					{
						m_ActionQueue.Enqueue(PostFacilityAction.SetDisabled(vehicle, flag));
					}
				}
				else if (m_DeliveryTruckData.TryGetComponent(vehicle, ref deliveryTruck))
				{
					if ((deliveryTruck.m_State & DeliveryTruckFlags.DummyTraffic) != 0)
					{
						continue;
					}
					if (m_LayoutElements.TryGetBuffer(vehicle, ref val) && val.Length != 0)
					{
						for (int j = 0; j < val.Length; j++)
						{
							Entity vehicle2 = val[j].m_Vehicle;
							if (!m_DeliveryTruckData.TryGetComponent(vehicle2, ref deliveryTruck2))
							{
								continue;
							}
							if ((deliveryTruck.m_State & DeliveryTruckFlags.Buying) != 0)
							{
								if ((deliveryTruck2.m_Resource & Resource.UnsortedMail) != Resource.NoResource)
								{
									num2 += deliveryTruck2.m_Amount;
								}
								else if ((deliveryTruck2.m_Resource & Resource.LocalMail) != Resource.NoResource)
								{
									num3 += deliveryTruck2.m_Amount;
								}
							}
							if (m_ReturnLoadData.TryGetComponent(vehicle2, ref returnLoad))
							{
								if ((returnLoad.m_Resource & Resource.UnsortedMail) != Resource.NoResource)
								{
									num2 += returnLoad.m_Amount;
								}
								else if ((returnLoad.m_Resource & Resource.LocalMail) != Resource.NoResource)
								{
									num3 += returnLoad.m_Amount;
								}
							}
						}
					}
					else
					{
						if ((deliveryTruck.m_State & DeliveryTruckFlags.Buying) != 0)
						{
							if ((deliveryTruck.m_Resource & Resource.UnsortedMail) != Resource.NoResource)
							{
								num2 += deliveryTruck.m_Amount;
							}
							else if ((deliveryTruck.m_Resource & Resource.LocalMail) != Resource.NoResource)
							{
								num3 += deliveryTruck.m_Amount;
							}
						}
						if (m_ReturnLoadData.TryGetComponent(vehicle, ref returnLoad2))
						{
							if ((returnLoad2.m_Resource & Resource.UnsortedMail) != Resource.NoResource)
							{
								num2 += returnLoad2.m_Amount;
							}
							else if ((returnLoad2.m_Resource & Resource.LocalMail) != Resource.NoResource)
							{
								num3 += returnLoad2.m_Amount;
							}
						}
					}
					availableDeliveryTrucks--;
				}
				else if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(vehicle))
				{
					ownedVehicles.RemoveAt(i--);
				}
			}
			Game.Vehicles.DeliveryTruck deliveryTruck3 = default(Game.Vehicles.DeliveryTruck);
			DynamicBuffer<LayoutElement> val2 = default(DynamicBuffer<LayoutElement>);
			Game.Vehicles.DeliveryTruck deliveryTruck4 = default(Game.Vehicles.DeliveryTruck);
			ReturnLoad returnLoad3 = default(ReturnLoad);
			ReturnLoad returnLoad4 = default(ReturnLoad);
			for (int k = 0; k < guestVehicles.Length; k++)
			{
				Entity vehicle3 = guestVehicles[k].m_Vehicle;
				if (!m_TargetData.HasComponent(vehicle3) || m_TargetData[vehicle3].m_Target != entity)
				{
					guestVehicles.RemoveAt(k--);
				}
				else
				{
					if (!m_DeliveryTruckData.TryGetComponent(vehicle3, ref deliveryTruck3) || (deliveryTruck3.m_State & DeliveryTruckFlags.DummyTraffic) != 0)
					{
						continue;
					}
					if (m_LayoutElements.TryGetBuffer(vehicle3, ref val2) && val2.Length != 0)
					{
						for (int l = 0; l < val2.Length; l++)
						{
							Entity vehicle4 = val2[l].m_Vehicle;
							if (!m_DeliveryTruckData.TryGetComponent(vehicle4, ref deliveryTruck4))
							{
								continue;
							}
							if ((deliveryTruck3.m_State & DeliveryTruckFlags.Buying) != 0)
							{
								if ((deliveryTruck4.m_Resource & Resource.UnsortedMail) != Resource.NoResource)
								{
									num4 += deliveryTruck4.m_Amount;
								}
								else if ((deliveryTruck4.m_Resource & Resource.LocalMail) != Resource.NoResource)
								{
									num5 += deliveryTruck4.m_Amount;
								}
								else if ((deliveryTruck4.m_Resource & Resource.OutgoingMail) != Resource.NoResource)
								{
									num6 += deliveryTruck4.m_Amount;
								}
							}
							else if ((deliveryTruck4.m_Resource & Resource.UnsortedMail) != Resource.NoResource)
							{
								num2 += deliveryTruck4.m_Amount;
							}
							else if ((deliveryTruck4.m_Resource & Resource.LocalMail) != Resource.NoResource)
							{
								num3 += deliveryTruck4.m_Amount;
							}
							if (m_ReturnLoadData.TryGetComponent(vehicle4, ref returnLoad3))
							{
								if ((returnLoad3.m_Resource & Resource.UnsortedMail) != Resource.NoResource)
								{
									num4 += returnLoad3.m_Amount;
								}
								else if ((returnLoad3.m_Resource & Resource.LocalMail) != Resource.NoResource)
								{
									num5 += returnLoad3.m_Amount;
								}
								else if ((returnLoad3.m_Resource & Resource.OutgoingMail) != Resource.NoResource)
								{
									num6 += returnLoad3.m_Amount;
								}
							}
						}
						continue;
					}
					if ((deliveryTruck3.m_State & DeliveryTruckFlags.Buying) != 0)
					{
						if ((deliveryTruck3.m_Resource & Resource.UnsortedMail) != Resource.NoResource)
						{
							num4 += deliveryTruck3.m_Amount;
						}
						else if ((deliveryTruck3.m_Resource & Resource.LocalMail) != Resource.NoResource)
						{
							num5 += deliveryTruck3.m_Amount;
						}
						else if ((deliveryTruck3.m_Resource & Resource.OutgoingMail) != Resource.NoResource)
						{
							num6 += deliveryTruck3.m_Amount;
						}
					}
					else if ((deliveryTruck3.m_Resource & Resource.UnsortedMail) != Resource.NoResource)
					{
						num2 += deliveryTruck3.m_Amount;
					}
					else if ((deliveryTruck3.m_Resource & Resource.LocalMail) != Resource.NoResource)
					{
						num3 += deliveryTruck3.m_Amount;
					}
					if (m_ReturnLoadData.TryGetComponent(vehicle3, ref returnLoad4))
					{
						if ((returnLoad4.m_Resource & Resource.UnsortedMail) != Resource.NoResource)
						{
							num4 += returnLoad4.m_Amount;
						}
						else if ((returnLoad4.m_Resource & Resource.LocalMail) != Resource.NoResource)
						{
							num5 += returnLoad4.m_Amount;
						}
						else if ((returnLoad4.m_Resource & Resource.OutgoingMail) != Resource.NoResource)
						{
							num6 += returnLoad4.m_Amount;
						}
					}
				}
			}
			postFacility.m_Flags &= ~(PostFacilityFlags.CanDeliverMailWithVan | PostFacilityFlags.CanCollectMailWithVan | PostFacilityFlags.HasAvailableTrucks | PostFacilityFlags.AcceptsUnsortedMail | PostFacilityFlags.DeliversLocalMail | PostFacilityFlags.AcceptsLocalMail | PostFacilityFlags.DeliversUnsortedMail);
			postFacility.m_AcceptMailPriority = 0f;
			postFacility.m_DeliverMailPriority = 0f;
			m_DeliveryTruckSelectData.GetCapacityRange(Resource.LocalMail, out var min, out var max);
			m_DeliveryTruckSelectData.GetCapacityRange(Resource.OutgoingMail, out var min2, out var max2);
			m_DeliveryTruckSelectData.GetCapacityRange(Resource.UnsortedMail, out var min3, out var max3);
			int num7 = prefabPostFacilityData.m_MailCapacity / 10;
			min = math.min(num7, max);
			min2 = math.min(num7, max2);
			min3 = math.min(num7, max3);
			if (prefabPostFacilityData.m_SortingRate != 0)
			{
				float num8 = 0.0009765625f;
				int num9 = Mathf.RoundToInt(num8 * (float)prefabPostFacilityData.m_SortingRate);
				int num10 = EconomyUtils.GetResources(Resource.UnsortedMail, resources);
				int num11 = math.min(num10, Mathf.RoundToInt(efficiency * num8 * (float)prefabPostFacilityData.m_SortingRate));
				postFacility.m_ProcessingFactor = (byte)math.clamp((num11 * 100 + num9 - 1) / num9, 0, 255);
				int num13;
				int num14;
				if (num11 != 0)
				{
					int num12 = (num11 * m_PostConfigurationData.m_OutgoingMailPercentage + ((Random)(ref random)).NextInt(100)) / 100;
					num10 = EconomyUtils.AddResources(Resource.UnsortedMail, -num11, resources);
					num13 = EconomyUtils.AddResources(Resource.LocalMail, num11 - num12, resources);
					num14 = EconomyUtils.AddResources(Resource.OutgoingMail, num12, resources);
				}
				else
				{
					num13 = EconomyUtils.GetResources(Resource.LocalMail, resources);
					num14 = EconomyUtils.GetResources(Resource.OutgoingMail, resources);
				}
				int num15 = num10 + num13 + num3 + num2;
				int num16 = prefabPostFacilityData.m_MailCapacity - num15;
				int num17 = math.min(mailBox.m_MailAmount, num16);
				if (num17 > 0)
				{
					mailBox.m_MailAmount -= num17;
					num10 = EconomyUtils.AddResources(Resource.UnsortedMail, num17, resources);
					num15 += num10;
					num16 -= num17;
				}
				num16 -= prefabMailBoxData.m_MailCapacity;
				num10 -= num4;
				num13 -= num5;
				num14 -= num6;
				int num18 = num10 + num2;
				for (int m = 0; m < dispatches.Length; m++)
				{
					Entity request = dispatches[m].m_Request;
					if (m_PostVanRequestData.HasComponent(request))
					{
						TrySpawnPostVan(jobIndex, ref random, entity, request, resources, ref postFacility, ref availableDeliveryVans, ref num13, ref num16, ref parkedPostVans);
						dispatches.RemoveAt(m--);
					}
					else if (m_MailTransferRequestData.HasComponent(request))
					{
						TrySpawnDeliveryTruck(jobIndex, ref random, entity, request, resources, ref availableDeliveryTrucks, ref num16);
						dispatches.RemoveAt(m--);
					}
					else if (!m_ServiceRequestData.HasComponent(request))
					{
						dispatches.RemoveAt(m--);
					}
				}
				if (num13 >= min || num14 >= min2)
				{
					MailTransferRequestFlags mailTransferRequestFlags;
					int amount;
					if (num13 >= num14)
					{
						postFacility.m_DeliverMailPriority = (float)num13 / (float)prefabPostFacilityData.m_MailCapacity;
						mailTransferRequestFlags = MailTransferRequestFlags.Receive | MailTransferRequestFlags.LocalMail;
						if (availableDeliveryTrucks <= 0)
						{
							mailTransferRequestFlags |= MailTransferRequestFlags.RequireTransport;
						}
						if (num16 >= min3)
						{
							mailTransferRequestFlags |= MailTransferRequestFlags.ReturnUnsortedMail;
						}
						amount = math.min(num13, max);
					}
					else
					{
						postFacility.m_DeliverMailPriority = (float)num14 / (float)prefabPostFacilityData.m_MailCapacity;
						mailTransferRequestFlags = MailTransferRequestFlags.Receive | MailTransferRequestFlags.OutgoingMail;
						if (availableDeliveryTrucks <= 0)
						{
							mailTransferRequestFlags |= MailTransferRequestFlags.RequireTransport;
						}
						if (num16 >= min)
						{
							mailTransferRequestFlags |= MailTransferRequestFlags.ReturnLocalMail;
						}
						amount = math.min(num14, max2);
					}
					if (m_MailTransferRequestData.HasComponent(postFacility.m_MailReceiveRequest))
					{
						if (m_MailTransferRequestData[postFacility.m_MailReceiveRequest].m_Flags != mailTransferRequestFlags)
						{
							Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val3, new HandleRequest(postFacility.m_MailReceiveRequest, Entity.Null, completed: true));
						}
						else
						{
							mailTransferRequestFlags = (MailTransferRequestFlags)0;
						}
					}
					if (mailTransferRequestFlags != 0)
					{
						Entity val4 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_MailTransferRequestArchetype);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MailTransferRequest>(jobIndex, val4, new MailTransferRequest(entity, mailTransferRequestFlags, postFacility.m_DeliverMailPriority, amount));
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val4, new RequestGroup(8u));
					}
				}
				if (num16 >= min3)
				{
					postFacility.m_AcceptMailPriority = 1f - (float)num18 / (float)prefabPostFacilityData.m_MailCapacity;
					MailTransferRequestFlags mailTransferRequestFlags2 = MailTransferRequestFlags.Deliver | MailTransferRequestFlags.RequireTransport | MailTransferRequestFlags.UnsortedMail;
					if (num13 >= min)
					{
						mailTransferRequestFlags2 |= MailTransferRequestFlags.ReturnLocalMail;
					}
					int amount2 = math.min(num16, max3);
					if (m_MailTransferRequestData.HasComponent(postFacility.m_MailDeliverRequest))
					{
						if (m_MailTransferRequestData[postFacility.m_MailDeliverRequest].m_Flags != mailTransferRequestFlags2)
						{
							Entity val5 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val5, new HandleRequest(postFacility.m_MailDeliverRequest, Entity.Null, completed: true));
						}
						else
						{
							mailTransferRequestFlags2 = (MailTransferRequestFlags)0;
						}
					}
					if (mailTransferRequestFlags2 != 0)
					{
						Entity val6 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_MailTransferRequestArchetype);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MailTransferRequest>(jobIndex, val6, new MailTransferRequest(entity, mailTransferRequestFlags2, postFacility.m_AcceptMailPriority, amount2));
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val6, new RequestGroup(8u));
					}
				}
				if (num16 >= min3)
				{
					postFacility.m_Flags |= PostFacilityFlags.AcceptsUnsortedMail;
				}
				if (num13 >= min)
				{
					postFacility.m_Flags |= PostFacilityFlags.DeliversLocalMail;
				}
				if (availableDeliveryVans > 0)
				{
					if (num13 > 0)
					{
						postFacility.m_Flags |= PostFacilityFlags.CanDeliverMailWithVan;
					}
					if (num16 > 0)
					{
						postFacility.m_Flags |= PostFacilityFlags.CanCollectMailWithVan;
					}
				}
				if (availableDeliveryTrucks > 0)
				{
					postFacility.m_Flags |= PostFacilityFlags.HasAvailableTrucks;
				}
			}
			else
			{
				postFacility.m_ProcessingFactor = 0;
				int num19 = EconomyUtils.GetResources(Resource.UnsortedMail, resources);
				int resources2 = EconomyUtils.GetResources(Resource.LocalMail, resources);
				int num20 = num19 + resources2 + num3 + num2;
				int num21 = prefabPostFacilityData.m_MailCapacity - num20;
				int num22 = math.min(mailBox.m_MailAmount, num21);
				if (num22 > 0)
				{
					mailBox.m_MailAmount -= num22;
					num19 = EconomyUtils.AddResources(Resource.UnsortedMail, num22, resources);
					num20 += num19;
					num21 -= num22;
				}
				num21 -= prefabMailBoxData.m_MailCapacity;
				num19 -= num4;
				resources2 -= num5;
				int num23 = resources2 + num3;
				for (int n = 0; n < dispatches.Length; n++)
				{
					Entity request2 = dispatches[n].m_Request;
					if (m_PostVanRequestData.HasComponent(request2))
					{
						TrySpawnPostVan(jobIndex, ref random, entity, request2, resources, ref postFacility, ref availableDeliveryVans, ref resources2, ref num21, ref parkedPostVans);
						dispatches.RemoveAt(n--);
					}
					else if (m_MailTransferRequestData.HasComponent(request2))
					{
						TrySpawnDeliveryTruck(jobIndex, ref random, entity, request2, resources, ref availableDeliveryTrucks, ref num21);
						dispatches.RemoveAt(n--);
					}
					else if (!m_ServiceRequestData.HasComponent(request2))
					{
						dispatches.RemoveAt(n--);
					}
				}
				int num24 = math.max(0, min3 - num19);
				int num25 = (prefabPostFacilityData.m_MailCapacity >> 1) - num23;
				int num26 = math.min(num25, num21 - num24);
				if (num26 >= min)
				{
					postFacility.m_AcceptMailPriority = 1f - (float)num23 / (float)prefabPostFacilityData.m_MailCapacity;
					MailTransferRequestFlags mailTransferRequestFlags3 = MailTransferRequestFlags.Deliver | MailTransferRequestFlags.RequireTransport | MailTransferRequestFlags.LocalMail;
					if (num19 >= min3)
					{
						mailTransferRequestFlags3 |= MailTransferRequestFlags.ReturnUnsortedMail;
					}
					int amount3 = math.min(num26, max);
					if (m_MailTransferRequestData.HasComponent(postFacility.m_MailDeliverRequest))
					{
						if (m_MailTransferRequestData[postFacility.m_MailDeliverRequest].m_Flags != mailTransferRequestFlags3)
						{
							Entity val7 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val7, new HandleRequest(postFacility.m_MailDeliverRequest, Entity.Null, completed: true));
						}
						else
						{
							mailTransferRequestFlags3 = (MailTransferRequestFlags)0;
						}
					}
					if (mailTransferRequestFlags3 != 0)
					{
						Entity val8 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_MailTransferRequestArchetype);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MailTransferRequest>(jobIndex, val8, new MailTransferRequest(entity, mailTransferRequestFlags3, postFacility.m_AcceptMailPriority, amount3));
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val8, new RequestGroup(8u));
					}
				}
				else if (num19 >= min3)
				{
					postFacility.m_DeliverMailPriority = (float)num19 / (float)prefabPostFacilityData.m_MailCapacity;
					MailTransferRequestFlags mailTransferRequestFlags4 = MailTransferRequestFlags.Receive | MailTransferRequestFlags.UnsortedMail;
					if (availableDeliveryTrucks <= 0)
					{
						mailTransferRequestFlags4 |= MailTransferRequestFlags.RequireTransport;
					}
					if (num25 >= min)
					{
						mailTransferRequestFlags4 |= MailTransferRequestFlags.ReturnLocalMail;
					}
					int amount4 = math.min(num19, max3);
					if (m_MailTransferRequestData.HasComponent(postFacility.m_MailReceiveRequest))
					{
						if (m_MailTransferRequestData[postFacility.m_MailReceiveRequest].m_Flags != mailTransferRequestFlags4)
						{
							Entity val9 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val9, new HandleRequest(postFacility.m_MailReceiveRequest, Entity.Null, completed: true));
						}
						else
						{
							mailTransferRequestFlags4 = (MailTransferRequestFlags)0;
						}
					}
					if (mailTransferRequestFlags4 != 0)
					{
						Entity val10 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_MailTransferRequestArchetype);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MailTransferRequest>(jobIndex, val10, new MailTransferRequest(entity, mailTransferRequestFlags4, postFacility.m_DeliverMailPriority, amount4));
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val10, new RequestGroup(8u));
					}
				}
				if (num26 >= min)
				{
					postFacility.m_Flags |= PostFacilityFlags.AcceptsLocalMail;
				}
				if (num19 >= min3)
				{
					postFacility.m_Flags |= PostFacilityFlags.DeliversUnsortedMail;
				}
				if (availableDeliveryVans > 0)
				{
					if (resources2 > 0)
					{
						postFacility.m_Flags |= PostFacilityFlags.CanDeliverMailWithVan;
					}
					if (num21 > 0)
					{
						postFacility.m_Flags |= PostFacilityFlags.CanCollectMailWithVan;
					}
				}
				if (availableDeliveryTrucks > 0)
				{
					postFacility.m_Flags |= PostFacilityFlags.HasAvailableTrucks;
				}
			}
			while (parkedPostVans.Length > math.max(0, prefabPostFacilityData.m_PostVanCapacity + availableDeliveryVans - vehicleCapacity))
			{
				int num27 = ((Random)(ref random)).NextInt(parkedPostVans.Length);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, parkedPostVans[num27]);
				parkedPostVans.RemoveAtSwapBack(num27);
			}
			for (int num28 = 0; num28 < parkedPostVans.Length; num28++)
			{
				Entity val11 = parkedPostVans[num28];
				Game.Vehicles.PostVan postVan2 = m_PostVanData[val11];
				bool flag2 = (postFacility.m_Flags & (PostFacilityFlags.CanDeliverMailWithVan | PostFacilityFlags.CanCollectMailWithVan)) == 0;
				if ((postVan2.m_State & PostVanFlags.Disabled) != 0 != flag2)
				{
					m_ActionQueue.Enqueue(PostFacilityAction.SetDisabled(val11, flag2));
				}
			}
			if ((postFacility.m_Flags & (PostFacilityFlags.CanDeliverMailWithVan | PostFacilityFlags.CanCollectMailWithVan)) != 0)
			{
				RequestTargetIfNeeded(jobIndex, entity, ref postFacility, availableDeliveryVans);
			}
		}

		private void RequestTargetIfNeeded(int jobIndex, Entity entity, ref Game.Buildings.PostFacility postFacility, int availablePostVans)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			if (!m_ServiceRequestData.HasComponent(postFacility.m_TargetRequest))
			{
				uint num = math.max(512u, 256u);
				if ((m_SimulationFrameIndex & (num - 1)) == 176)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_PostVanRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PostVanRequest>(jobIndex, val, new PostVanRequest(entity, (PostVanRequestFlags)0, (ushort)availablePostVans));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(32u));
				}
			}
		}

		private bool TrySpawnPostVan(int jobIndex, ref Random random, Entity entity, Entity request, DynamicBuffer<Resources> resources, ref Game.Buildings.PostFacility postFacility, ref int availableDeliveryVans, ref int localMail, ref int freeSpace, ref StackList<Entity> parkedPostVans)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			int2 mailCapacity = default(int2);
			((int2)(ref mailCapacity))._002Ector(1, math.max(localMail, freeSpace));
			if (availableDeliveryVans <= 0)
			{
				return false;
			}
			if (mailCapacity.y <= 0)
			{
				return false;
			}
			PostVanRequest postVanRequest = m_PostVanRequestData[request];
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(postVanRequest.m_Target))
			{
				return false;
			}
			if (localMail <= 0 && (postVanRequest.m_Flags & PostVanRequestFlags.Deliver) != 0)
			{
				return false;
			}
			Entity val = Entity.Null;
			PathInformation pathInformation = default(PathInformation);
			if (m_PathInformationData.TryGetComponent(request, ref pathInformation) && pathInformation.m_Origin != entity)
			{
				PrefabRef prefabRef = default(PrefabRef);
				PostVanData postVanData = default(PostVanData);
				if (m_PrefabRefData.TryGetComponent(pathInformation.m_Origin, ref prefabRef) && m_PrefabPostVanData.TryGetComponent(prefabRef.m_Prefab, ref postVanData))
				{
					mailCapacity = int2.op_Implicit(postVanData.m_MailCapacity);
					if (mailCapacity.y > freeSpace)
					{
						return false;
					}
				}
				if (!CollectionUtils.RemoveValueSwapBack<Entity>(ref parkedPostVans, pathInformation.m_Origin))
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
				Entity vehiclePrefab = m_PostVanSelectData.SelectVehicle(ref random, ref mailCapacity);
				if (mailCapacity.y > freeSpace)
				{
					return false;
				}
				val = m_PostVanSelectData.CreateVehicle(m_CommandBuffer, jobIndex, ref random, m_TransformData[entity], entity, vehiclePrefab, parked: false);
				if (val == Entity.Null)
				{
					return false;
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val, new Owner(entity));
			}
			int num = math.min(localMail, mailCapacity.y);
			availableDeliveryVans--;
			localMail -= num;
			freeSpace -= mailCapacity.y;
			EconomyUtils.AddResources(Resource.LocalMail, -num, resources);
			PostVanFlags postVanFlags = (PostVanFlags)0u;
			if ((postVanRequest.m_Flags & PostVanRequestFlags.Deliver) != 0)
			{
				postVanFlags |= PostVanFlags.Delivering;
			}
			if ((postVanRequest.m_Flags & PostVanRequestFlags.Collect) != 0)
			{
				postVanFlags |= PostVanFlags.Collecting;
			}
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Vehicles.PostVan>(jobIndex, val, new Game.Vehicles.PostVan(postVanFlags, 1, num));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(jobIndex, val, new Target(postVanRequest.m_Target));
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
			if (m_ServiceRequestData.HasComponent(postFacility.m_TargetRequest))
			{
				val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(postFacility.m_TargetRequest, Entity.Null, completed: true));
			}
			return true;
		}

		private bool TrySpawnDeliveryTruck(int jobIndex, ref Random random, Entity entity, Entity request, DynamicBuffer<Resources> resources, ref int availableDeliveryTrucks, ref int freeSpace)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			if (availableDeliveryTrucks <= 0)
			{
				return false;
			}
			MailTransferRequest mailTransferRequest = m_MailTransferRequestData[request];
			PathInformation pathInformation = m_PathInformationData[request];
			if (!m_PrefabRefData.HasComponent(pathInformation.m_Destination))
			{
				return false;
			}
			DeliveryTruckFlags deliveryTruckFlags = (DeliveryTruckFlags)0u;
			Resource resource = Resource.NoResource;
			Resource resource2 = Resource.NoResource;
			int amount = mailTransferRequest.m_Amount;
			int returnAmount = 0;
			if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.UnsortedMail) != 0)
			{
				resource = Resource.UnsortedMail;
			}
			if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.LocalMail) != 0)
			{
				resource = Resource.LocalMail;
			}
			if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.OutgoingMail) != 0)
			{
				resource = Resource.OutgoingMail;
			}
			if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.RequireTransport) != 0)
			{
				if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.Deliver) != 0)
				{
					deliveryTruckFlags |= DeliveryTruckFlags.Loaded;
				}
				if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.Receive) != 0)
				{
					deliveryTruckFlags |= DeliveryTruckFlags.Buying;
				}
			}
			else
			{
				if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.Deliver) != 0)
				{
					deliveryTruckFlags |= DeliveryTruckFlags.Buying;
				}
				if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.Receive) != 0)
				{
					deliveryTruckFlags |= DeliveryTruckFlags.Loaded;
				}
			}
			int max;
			if ((deliveryTruckFlags & DeliveryTruckFlags.Loaded) != 0)
			{
				amount = math.min(amount, EconomyUtils.GetResources(resource, resources));
				if (amount <= 0)
				{
					return false;
				}
				if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.ReturnUnsortedMail) != 0)
				{
					resource2 = Resource.UnsortedMail;
				}
				if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.ReturnLocalMail) != 0)
				{
					resource2 = Resource.LocalMail;
				}
				if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.ReturnOutgoingMail) != 0)
				{
					resource2 = Resource.OutgoingMail;
				}
				if (resource2 != Resource.NoResource)
				{
					m_DeliveryTruckSelectData.GetCapacityRange(resource | resource2, out var min, out max);
					returnAmount = math.min(amount + freeSpace, math.max(amount, min));
					if (returnAmount <= 0)
					{
						resource2 = Resource.NoResource;
						returnAmount = 0;
					}
				}
			}
			else
			{
				resource2 = resource;
				returnAmount = amount;
				resource = Resource.NoResource;
				amount = 0;
				if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.ReturnUnsortedMail) != 0)
				{
					resource = Resource.UnsortedMail;
				}
				if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.ReturnLocalMail) != 0)
				{
					resource = Resource.LocalMail;
				}
				if ((mailTransferRequest.m_Flags & MailTransferRequestFlags.ReturnOutgoingMail) != 0)
				{
					resource = Resource.OutgoingMail;
				}
				if (resource != Resource.NoResource)
				{
					m_DeliveryTruckSelectData.GetCapacityRange(resource | resource2, out var min2, out max);
					amount = math.min(EconomyUtils.GetResources(resource, resources), math.max(returnAmount, min2));
					if (amount <= 0)
					{
						resource = Resource.NoResource;
						amount = 0;
					}
				}
				returnAmount = math.min(returnAmount, amount + freeSpace);
				if (returnAmount <= 0)
				{
					resource2 = Resource.NoResource;
					returnAmount = 0;
					if (amount == 0)
					{
						return false;
					}
				}
				deliveryTruckFlags = (DeliveryTruckFlags)((uint)deliveryTruckFlags & 0xFFFFFFEFu);
				deliveryTruckFlags |= DeliveryTruckFlags.Loaded;
			}
			if (amount > 0)
			{
				deliveryTruckFlags |= DeliveryTruckFlags.UpdateOwnerQuantity;
			}
			Entity val = m_DeliveryTruckSelectData.CreateVehicle(m_CommandBuffer, jobIndex, ref random, ref m_PrefabDeliveryTruckData, ref m_PrefabObjectData, resource, resource2, ref amount, ref returnAmount, m_TransformData[entity], entity, deliveryTruckFlags);
			if (val != Entity.Null)
			{
				if (amount > 0)
				{
					EconomyUtils.AddResources(resource, -amount, resources);
				}
				availableDeliveryTrucks--;
				freeSpace += amount - returnAmount;
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
	private struct PostFacilityActionJob : IJob
	{
		public ComponentLookup<Game.Vehicles.PostVan> m_PostVanData;

		public NativeQueue<PostFacilityAction> m_ActionQueue;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			PostFacilityAction postFacilityAction = default(PostFacilityAction);
			Game.Vehicles.PostVan postVan = default(Game.Vehicles.PostVan);
			while (m_ActionQueue.TryDequeue(ref postFacilityAction))
			{
				if (m_PostVanData.TryGetComponent(postFacilityAction.m_Entity, ref postVan))
				{
					if (postFacilityAction.m_Disabled)
					{
						postVan.m_State |= PostVanFlags.Disabled;
					}
					else
					{
						postVan.m_State &= ~PostVanFlags.Disabled;
					}
					m_PostVanData[postFacilityAction.m_Entity] = postVan;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Buildings.PostFacility> __Game_Buildings_PostFacility_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Routes.MailBox> __Game_Routes_MailBox_RW_ComponentTypeHandle;

		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		public BufferTypeHandle<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RW_BufferTypeHandle;

		public BufferTypeHandle<GuestVehicle> __Game_Vehicles_GuestVehicle_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PostVanRequest> __Game_Simulation_PostVanRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MailTransferRequest> __Game_Simulation_MailTransferRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PostVan> __Game_Vehicles_PostVan_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ReturnLoad> __Game_Vehicles_ReturnLoad_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PostFacilityData> __Game_Prefabs_PostFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MailBoxData> __Game_Prefabs_MailBoxData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PostVanData> __Game_Prefabs_PostVanData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DeliveryTruckData> __Game_Prefabs_DeliveryTruckData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectData> __Game_Prefabs_ObjectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		public ComponentLookup<Game.Vehicles.PostVan> __Game_Vehicles_PostVan_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Buildings_PostFacility_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.PostFacility>(false);
			__Game_Routes_MailBox_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.MailBox>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Vehicles_OwnedVehicle_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<OwnedVehicle>(false);
			__Game_Vehicles_GuestVehicle_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<GuestVehicle>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Simulation_PostVanRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PostVanRequest>(true);
			__Game_Simulation_MailTransferRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailTransferRequest>(true);
			__Game_Simulation_ServiceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Vehicles_PostVan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PostVan>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Vehicles_ReturnLoad_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ReturnLoad>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_PostFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PostFacilityData>(true);
			__Game_Prefabs_MailBoxData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailBoxData>(true);
			__Game_Prefabs_PostVanData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PostVanData>(true);
			__Game_Prefabs_DeliveryTruckData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DeliveryTruckData>(true);
			__Game_Prefabs_ObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectData>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Vehicles_PostVan_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PostVan>(false);
		}
	}

	public static readonly int kUpdatesPerDay = 1024;

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_PostVanPrefabQuery;

	private EntityQuery m_PostConfigurationQuery;

	private EntityArchetype m_MailTransferRequestArchetype;

	private EntityArchetype m_PostVanRequestArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_ParkedToMovingRemoveTypes;

	private ComponentTypeSet m_ParkedToMovingCarAddTypes;

	private VehicleCapacitySystem m_VehicleCapacitySystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private PostVanSelectData m_PostVanSelectData;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 176;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_VehicleCapacitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehicleCapacitySystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PostVanSelectData = new PostVanSelectData((SystemBase)(object)this);
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Game.Buildings.PostFacility>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_PostVanPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { PostVanSelectData.GetEntityQueryDesc() });
		m_PostConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PostConfigurationData>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_MailTransferRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<MailTransferRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_PostVanRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<PostVanRequest>(),
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
		((ComponentSystemBase)this).RequireForUpdate(m_PostConfigurationQuery);
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
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		m_PostVanSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_PostVanPrefabQuery, (Allocator)3, out var jobHandle);
		NativeQueue<PostFacilityAction> actionQueue = default(NativeQueue<PostFacilityAction>);
		actionQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		PostFacilityTickJob postFacilityTickJob = new PostFacilityTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PostFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.PostFacility>(ref __TypeHandle.__Game_Buildings_PostFacility_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MailBoxType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.MailBox>(ref __TypeHandle.__Game_Routes_MailBox_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatchType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcesType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicleType = InternalCompilerInterface.GetBufferTypeHandle<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GuestVehicleType = InternalCompilerInterface.GetBufferTypeHandle<GuestVehicle>(ref __TypeHandle.__Game_Vehicles_GuestVehicle_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetData = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PostVanRequestData = InternalCompilerInterface.GetComponentLookup<PostVanRequest>(ref __TypeHandle.__Game_Simulation_PostVanRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailTransferRequestData = InternalCompilerInterface.GetComponentLookup<MailTransferRequest>(ref __TypeHandle.__Game_Simulation_MailTransferRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PostVanData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PostVan>(ref __TypeHandle.__Game_Vehicles_PostVan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeliveryTruckData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ReturnLoadData = InternalCompilerInterface.GetComponentLookup<ReturnLoad>(ref __TypeHandle.__Game_Vehicles_ReturnLoad_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPostFacilityData = InternalCompilerInterface.GetComponentLookup<PostFacilityData>(ref __TypeHandle.__Game_Prefabs_PostFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMailBoxData = InternalCompilerInterface.GetComponentLookup<MailBoxData>(ref __TypeHandle.__Game_Prefabs_MailBoxData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPostVanData = InternalCompilerInterface.GetComponentLookup<PostVanData>(ref __TypeHandle.__Game_Prefabs_PostVanData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDeliveryTruckData = InternalCompilerInterface.GetComponentLookup<DeliveryTruckData>(ref __TypeHandle.__Game_Prefabs_DeliveryTruckData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectData = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_PostVanSelectData = m_PostVanSelectData,
			m_DeliveryTruckSelectData = m_VehicleCapacitySystem.GetDeliveryTruckSelectData(),
			m_PostConfigurationData = ((EntityQuery)(ref m_PostConfigurationQuery)).GetSingleton<PostConfigurationData>(),
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex,
			m_PostVanRequestArchetype = m_PostVanRequestArchetype,
			m_MailTransferRequestArchetype = m_MailTransferRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_ParkedToMovingRemoveTypes = m_ParkedToMovingRemoveTypes,
			m_ParkedToMovingCarAddTypes = m_ParkedToMovingCarAddTypes
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		postFacilityTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		postFacilityTickJob.m_ActionQueue = actionQueue.AsParallelWriter();
		PostFacilityTickJob postFacilityTickJob2 = postFacilityTickJob;
		PostFacilityActionJob obj = new PostFacilityActionJob
		{
			m_PostVanData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PostVan>(ref __TypeHandle.__Game_Vehicles_PostVan_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActionQueue = actionQueue
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<PostFacilityTickJob>(postFacilityTickJob2, m_BuildingQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, jobHandle));
		JobHandle val3 = IJobExtensions.Schedule<PostFacilityActionJob>(obj, val2);
		actionQueue.Dispose(val3);
		m_PostVanSelectData.PostUpdate(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
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
	public PostFacilityAISystem()
	{
	}
}

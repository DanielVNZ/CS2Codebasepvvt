using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Events;
using Game.Net;
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
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class EmergencyShelterAISystem : GameSystemBase
{
	private struct EmergencyShelterAction
	{
		public Entity m_Entity;

		public bool m_Disabled;

		public static EmergencyShelterAction SetDisabled(Entity vehicle, bool disabled)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return new EmergencyShelterAction
			{
				m_Entity = vehicle,
				m_Disabled = disabled
			};
		}
	}

	[BurstCompile]
	private struct EmergencyShelterTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> m_OwnedVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.ResourceConsumer> m_ResourceConsumerType;

		public ComponentTypeHandle<Game.Buildings.EmergencyShelter> m_EmergencyShelterType;

		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		public ComponentTypeHandle<ServiceUsage> m_ServiceUsageType;

		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		public BufferTypeHandle<Occupant> m_OccupantType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<EvacuationRequest> m_EvacuationRequestData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<EmergencyShelterData> m_PrefabEmergencyShelterData;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> m_PrefabPublicTransportVehicleData;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_TravelPurposeData;

		[ReadOnly]
		public ComponentLookup<Worker> m_WorkerData;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> m_StudentData;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMemberData;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> m_TouristHouseholdData;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHouseholdData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterData;

		[ReadOnly]
		public ComponentLookup<InDanger> m_InDangerData;

		[ReadOnly]
		public TransportVehicleSelectData m_TransportVehicleSelectData;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_EvacuationRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingCarAddTypes;

		[ReadOnly]
		public float m_DangerLevelExitProbability;

		[ReadOnly]
		public float m_InoperableExitProbability;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<EmergencyShelterAction> m_ActionQueue;

		public unsafe void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
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
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Game.Buildings.EmergencyShelter> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.EmergencyShelter>(ref m_EmergencyShelterType);
			BufferAccessor<Efficiency> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			NativeArray<Game.Buildings.ResourceConsumer> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.ResourceConsumer>(ref m_ResourceConsumerType);
			NativeArray<ServiceUsage> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceUsage>(ref m_ServiceUsageType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			BufferAccessor<OwnedVehicle> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedVehicle>(ref m_OwnedVehicleType);
			BufferAccessor<ServiceDispatch> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			BufferAccessor<Occupant> bufferAccessor5 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Occupant>(ref m_OccupantType);
			Span<float> val = new Span<float>((void*)stackalloc byte[120], 30);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity entity = nativeArray[i];
				Transform transform = nativeArray2[i];
				PrefabRef prefabRef = nativeArray3[i];
				Game.Buildings.EmergencyShelter emergencyShelter = nativeArray4[i];
				DynamicBuffer<OwnedVehicle> vehicles = bufferAccessor3[i];
				DynamicBuffer<ServiceDispatch> dispatches = bufferAccessor4[i];
				DynamicBuffer<Occupant> occupants = bufferAccessor5[i];
				EmergencyShelterData data = default(EmergencyShelterData);
				if (m_PrefabEmergencyShelterData.HasComponent(prefabRef.m_Prefab))
				{
					data = m_PrefabEmergencyShelterData[prefabRef.m_Prefab];
				}
				if (bufferAccessor2.Length != 0)
				{
					UpgradeUtils.CombineStats<EmergencyShelterData>(ref data, bufferAccessor2[i], ref m_PrefabRefData, ref m_PrefabEmergencyShelterData);
				}
				if (bufferAccessor.Length != 0)
				{
					BuildingUtils.GetEfficiencyFactors(bufferAccessor[i], val);
				}
				else
				{
					val.Fill(1f);
				}
				float immediateEfficiency = BuildingUtils.GetImmediateEfficiency(bufferAccessor, i);
				byte resourceAvailability = ((nativeArray5.Length != 0) ? nativeArray5[i].m_ResourceAvailability : byte.MaxValue);
				Tick(unfilteredChunkIndex, entity, transform, ref random, ref emergencyShelter, out var serviceUsage, data, vehicles, dispatches, occupants, val, immediateEfficiency, resourceAvailability);
				nativeArray4[i] = emergencyShelter;
				if (nativeArray6.Length != 0)
				{
					nativeArray6[i] = new ServiceUsage
					{
						m_Usage = serviceUsage
					};
				}
				if (bufferAccessor.Length != 0)
				{
					BuildingUtils.SetEfficiencyFactors(bufferAccessor[i], val);
				}
			}
		}

		private unsafe void Tick(int jobIndex, Entity entity, Transform transform, ref Random random, ref Game.Buildings.EmergencyShelter emergencyShelter, out float serviceUsage, EmergencyShelterData prefabEmergencyShelterData, DynamicBuffer<OwnedVehicle> vehicles, DynamicBuffer<ServiceDispatch> dispatches, DynamicBuffer<Occupant> occupants, Span<float> efficiencyFactors, float immediateEfficiency, byte resourceAvailability)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			float num = ((resourceAvailability > 0) ? 1f : 0f);
			efficiencyFactors[17] = num;
			float efficiency = BuildingUtils.GetEfficiency(efficiencyFactors);
			bool flag = (double)efficiency > 0.001;
			TravelPurpose travelPurpose = default(TravelPurpose);
			for (int num2 = occupants.Length - 1; num2 >= 0; num2--)
			{
				if (m_PrefabRefData.HasComponent((Entity)occupants[num2]))
				{
					if (m_TravelPurposeData.TryGetComponent((Entity)occupants[num2], ref travelPurpose) && travelPurpose.m_Purpose == Purpose.InEmergencyShelter)
					{
						if (flag)
						{
							if (!IsCitizenInDanger(occupants[num2]) && ((Random)(ref random)).NextFloat() < m_DangerLevelExitProbability)
							{
								((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(jobIndex, (Entity)occupants[num2]);
							}
						}
						else if (((Random)(ref random)).NextFloat() < m_InoperableExitProbability)
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(jobIndex, (Entity)occupants[num2]);
						}
					}
				}
				else
				{
					occupants.RemoveAtSwapBack(num2);
				}
			}
			int availableSpace = (flag ? (prefabEmergencyShelterData.m_ShelterCapacity - occupants.Length) : 0);
			int vehicleCapacity = BuildingUtils.GetVehicleCapacity(efficiency, prefabEmergencyShelterData.m_VehicleCapacity);
			int num3 = BuildingUtils.GetVehicleCapacity(immediateEfficiency, prefabEmergencyShelterData.m_VehicleCapacity);
			int availableVehicles = vehicleCapacity;
			serviceUsage = (flag ? ((float)occupants.Length / math.max(1f, (float)prefabEmergencyShelterData.m_ShelterCapacity)) : 0f);
			int length = vehicles.Length;
			StackList<Entity> parkedVehicles = StackList<Entity>.op_Implicit(new Span<Entity>((void*)stackalloc Entity[length], length));
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			ParkedCar parkedCar = default(ParkedCar);
			for (int i = 0; i < vehicles.Length; i++)
			{
				Entity vehicle = vehicles[i].m_Vehicle;
				if (!m_PublicTransportData.TryGetComponent(vehicle, ref publicTransport))
				{
					continue;
				}
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
				PublicTransportVehicleData publicTransportVehicleData = m_PrefabPublicTransportVehicleData[prefabRef.m_Prefab];
				availableVehicles--;
				availableSpace -= publicTransportVehicleData.m_PassengerCapacity;
				bool flag2 = --num3 < 0;
				if ((publicTransport.m_State & PublicTransportFlags.Disabled) != 0 != flag2)
				{
					m_ActionQueue.Enqueue(EmergencyShelterAction.SetDisabled(vehicle, flag2));
				}
			}
			int num4 = 0;
			while (num4 < dispatches.Length)
			{
				Entity request = dispatches[num4].m_Request;
				if (m_EvacuationRequestData.HasComponent(request))
				{
					SpawnVehicle(jobIndex, ref random, entity, request, transform, ref emergencyShelter, ref availableVehicles, ref availableSpace, ref parkedVehicles);
					dispatches.RemoveAt(num4);
				}
				else if (!m_ServiceRequestData.HasComponent(request))
				{
					dispatches.RemoveAt(num4);
				}
				else
				{
					num4++;
				}
			}
			while (parkedVehicles.Length > math.max(0, prefabEmergencyShelterData.m_VehicleCapacity + availableVehicles - vehicleCapacity))
			{
				int num5 = ((Random)(ref random)).NextInt(parkedVehicles.Length);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, parkedVehicles[num5]);
				parkedVehicles.RemoveAtSwapBack(num5);
			}
			for (int j = 0; j < parkedVehicles.Length; j++)
			{
				Entity val = parkedVehicles[j];
				Game.Vehicles.PublicTransport publicTransport2 = m_PublicTransportData[val];
				bool flag3 = availableVehicles <= 0 || availableSpace <= 0;
				if ((publicTransport2.m_State & PublicTransportFlags.Disabled) != 0 != flag3)
				{
					m_ActionQueue.Enqueue(EmergencyShelterAction.SetDisabled(val, flag3));
				}
			}
			if (availableVehicles > 0)
			{
				emergencyShelter.m_Flags |= EmergencyShelterFlags.HasAvailableVehicles;
				RequestTargetIfNeeded(jobIndex, entity, ref emergencyShelter, availableVehicles);
			}
			else
			{
				emergencyShelter.m_Flags &= ~EmergencyShelterFlags.HasAvailableVehicles;
			}
			if (availableSpace > 0)
			{
				emergencyShelter.m_Flags |= EmergencyShelterFlags.HasShelterSpace;
			}
			else
			{
				emergencyShelter.m_Flags &= ~EmergencyShelterFlags.HasShelterSpace;
			}
		}

		private void RequestTargetIfNeeded(int jobIndex, Entity entity, ref Game.Buildings.EmergencyShelter emergencyShelter, int availableVehicles)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			if (!m_ServiceRequestData.HasComponent(emergencyShelter.m_TargetRequest))
			{
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_EvacuationRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val, new ServiceRequest(reversed: true));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<EvacuationRequest>(jobIndex, val, new EvacuationRequest(entity, availableVehicles));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(4u));
			}
		}

		private void SpawnVehicle(int jobIndex, ref Random random, Entity entity, Entity request, Transform transform, ref Game.Buildings.EmergencyShelter emergencyShelter, ref int availableVehicles, ref int availableSpace, ref StackList<Entity> parkedVehicles)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			EvacuationRequest evacuationRequest = default(EvacuationRequest);
			if (!m_EvacuationRequestData.TryGetComponent(request, ref evacuationRequest) || !((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(evacuationRequest.m_Target) || availableVehicles <= 0 || availableSpace <= 0)
			{
				return;
			}
			int2 passengerCapacity = default(int2);
			((int2)(ref passengerCapacity))._002Ector(1, availableSpace);
			int2 cargoCapacity = int2.op_Implicit(0);
			Entity val = Entity.Null;
			PathInformation pathInformation = default(PathInformation);
			if (m_PathInformationData.TryGetComponent(request, ref pathInformation) && pathInformation.m_Origin != entity)
			{
				PrefabRef prefabRef = default(PrefabRef);
				PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
				if (m_PrefabRefData.TryGetComponent(pathInformation.m_Origin, ref prefabRef) && m_PrefabPublicTransportVehicleData.TryGetComponent(prefabRef.m_Prefab, ref publicTransportVehicleData))
				{
					passengerCapacity = int2.op_Implicit(publicTransportVehicleData.m_PassengerCapacity);
				}
				if (!CollectionUtils.RemoveValueSwapBack<Entity>(ref parkedVehicles, pathInformation.m_Origin))
				{
					return;
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
				val = m_TransportVehicleSelectData.CreateVehicle(m_CommandBuffer, jobIndex, ref random, transform, entity, Entity.Null, Entity.Null, TransportType.Bus, EnergyTypes.FuelAndElectricity, SizeClass.Large, PublicTransportPurpose.Evacuation, Resource.NoResource, ref passengerCapacity, ref cargoCapacity, parked: false);
				if (val == Entity.Null)
				{
					return;
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val, new Owner(entity));
			}
			availableVehicles--;
			availableSpace -= passengerCapacity.y;
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			publicTransport.m_State |= PublicTransportFlags.Evacuating;
			publicTransport.m_RequestCount = 1;
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Vehicles.PublicTransport>(jobIndex, val, publicTransport);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(jobIndex, val, new Target(evacuationRequest.m_Target));
			((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<ServiceDispatch>(jobIndex, val).Add(new ServiceDispatch(request));
			DynamicBuffer<PathElement> sourceElements = default(DynamicBuffer<PathElement>);
			if (m_PathElements.TryGetBuffer(request, ref sourceElements) && sourceElements.Length != 0)
			{
				DynamicBuffer<PathElement> targetElements = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<PathElement>(jobIndex, val);
				PathUtils.CopyPath(sourceElements, default(PathOwner), 0, targetElements);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathOwner>(jobIndex, val, new PathOwner(PathFlags.Updated));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(jobIndex, val, pathInformation);
			}
			Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(request, val, completed: false));
			if (m_ServiceRequestData.HasComponent(emergencyShelter.m_TargetRequest))
			{
				val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(emergencyShelter.m_TargetRequest, Entity.Null, completed: true));
			}
		}

		private bool IsCitizenInDanger(Entity citizen)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			HouseholdMember householdMember = default(HouseholdMember);
			if (m_HouseholdMemberData.TryGetComponent(citizen, ref householdMember))
			{
				PropertyRenter propertyRenter = default(PropertyRenter);
				if (m_PropertyRenterData.TryGetComponent(householdMember.m_Household, ref propertyRenter) && IsInDanger(propertyRenter.m_Property))
				{
					return true;
				}
				TouristHousehold touristHousehold = default(TouristHousehold);
				if (m_TouristHouseholdData.TryGetComponent(citizen, ref touristHousehold) && IsBuildingOrCompanyInDanger(touristHousehold.m_Hotel))
				{
					return true;
				}
				HomelessHousehold homelessHousehold = default(HomelessHousehold);
				if (m_HomelessHouseholdData.TryGetComponent(citizen, ref homelessHousehold) && IsInDanger(homelessHousehold.m_TempHome))
				{
					return true;
				}
			}
			Worker worker = default(Worker);
			if (m_WorkerData.TryGetComponent(citizen, ref worker) && IsBuildingOrCompanyInDanger(worker.m_Workplace))
			{
				return true;
			}
			Game.Citizens.Student student = default(Game.Citizens.Student);
			if (m_StudentData.TryGetComponent(citizen, ref student) && IsInDanger(student.m_School))
			{
				return true;
			}
			return false;
		}

		private bool IsBuildingOrCompanyInDanger(Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (IsInDanger(entity))
			{
				return true;
			}
			PropertyRenter propertyRenter = default(PropertyRenter);
			if (m_PropertyRenterData.TryGetComponent(entity, ref propertyRenter) && IsInDanger(propertyRenter.m_Property))
			{
				return true;
			}
			return false;
		}

		private bool IsInDanger(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return m_InDangerData.HasComponent(entity);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct EmergencyShelterActionJob : IJob
	{
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		public NativeQueue<EmergencyShelterAction> m_ActionQueue;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			EmergencyShelterAction emergencyShelterAction = default(EmergencyShelterAction);
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			while (m_ActionQueue.TryDequeue(ref emergencyShelterAction))
			{
				if (m_PublicTransportData.TryGetComponent(emergencyShelterAction.m_Entity, ref publicTransport))
				{
					if (emergencyShelterAction.m_Disabled)
					{
						publicTransport.m_State |= PublicTransportFlags.AbandonRoute | PublicTransportFlags.Disabled;
					}
					else
					{
						publicTransport.m_State &= ~PublicTransportFlags.Disabled;
					}
					m_PublicTransportData[emergencyShelterAction.m_Entity] = publicTransport;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.ResourceConsumer> __Game_Buildings_ResourceConsumer_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Buildings.EmergencyShelter> __Game_Buildings_EmergencyShelter_RW_ComponentTypeHandle;

		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RW_BufferTypeHandle;

		public ComponentTypeHandle<ServiceUsage> __Game_Buildings_ServiceUsage_RW_ComponentTypeHandle;

		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferTypeHandle;

		public BufferTypeHandle<Occupant> __Game_Buildings_Occupant_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<EvacuationRequest> __Game_Simulation_EvacuationRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EmergencyShelterData> __Game_Prefabs_EmergencyShelterData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> __Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> __Game_Citizens_TouristHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InDanger> __Game_Events_InDanger_RO_ComponentLookup;

		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RW_ComponentLookup;

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
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<OwnedVehicle>(true);
			__Game_Buildings_ResourceConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.ResourceConsumer>(true);
			__Game_Buildings_EmergencyShelter_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.EmergencyShelter>(false);
			__Game_Buildings_Efficiency_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(false);
			__Game_Buildings_ServiceUsage_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceUsage>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__Game_Buildings_Occupant_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Occupant>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Simulation_EvacuationRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EvacuationRequest>(true);
			__Game_Simulation_ServiceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_EmergencyShelterData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EmergencyShelterData>(true);
			__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PublicTransportVehicleData>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Citizens_Student_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Citizens.Student>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Citizens_TouristHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TouristHousehold>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Events_InDanger_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InDanger>(true);
			__Game_Vehicles_PublicTransport_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(false);
		}
	}

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_VehiclePrefabQuery;

	private EntityArchetype m_EvacuationRequestArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_ParkedToMovingRemoveTypes;

	private ComponentTypeSet m_ParkedToMovingCarAddTypes;

	private EndFrameBarrier m_EndFrameBarrier;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private TransportVehicleSelectData m_TransportVehicleSelectData;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1553762682_0;

	private EntityQuery __query_1553762682_1;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 240;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_TransportVehicleSelectData = new TransportVehicleSelectData((SystemBase)(object)this);
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Buildings.EmergencyShelter>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_VehiclePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { TransportVehicleSelectData.GetEntityQueryDesc() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EvacuationRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<EvacuationRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_HandleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<HandleRequest>(),
			ComponentType.ReadWrite<Game.Common.Event>()
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
		((ComponentSystemBase)this).RequireForUpdate<DisasterConfigurationData>();
		((ComponentSystemBase)this).RequireForUpdate<Game.City.DangerLevel>();
		Assert.IsTrue(true);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		m_TransportVehicleSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_VehiclePrefabQuery, (Allocator)3, out var jobHandle);
		float dangerLevel = ((EntityQuery)(ref __query_1553762682_0)).GetSingleton<Game.City.DangerLevel>().m_DangerLevel;
		DisasterConfigurationData singleton = ((EntityQuery)(ref __query_1553762682_1)).GetSingleton<DisasterConfigurationData>();
		NativeQueue<EmergencyShelterAction> actionQueue = default(NativeQueue<EmergencyShelterAction>);
		actionQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		EmergencyShelterTickJob emergencyShelterTickJob = new EmergencyShelterTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicleType = InternalCompilerInterface.GetBufferTypeHandle<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceConsumerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.ResourceConsumer>(ref __TypeHandle.__Game_Buildings_ResourceConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EmergencyShelterType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.EmergencyShelter>(ref __TypeHandle.__Game_Buildings_EmergencyShelter_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUsageType = InternalCompilerInterface.GetComponentTypeHandle<ServiceUsage>(ref __TypeHandle.__Game_Buildings_ServiceUsage_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatchType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OccupantType = InternalCompilerInterface.GetBufferTypeHandle<Occupant>(ref __TypeHandle.__Game_Buildings_Occupant_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EvacuationRequestData = InternalCompilerInterface.GetComponentLookup<EvacuationRequest>(ref __TypeHandle.__Game_Simulation_EvacuationRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabEmergencyShelterData = InternalCompilerInterface.GetComponentLookup<EmergencyShelterData>(ref __TypeHandle.__Game_Prefabs_EmergencyShelterData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPublicTransportVehicleData = InternalCompilerInterface.GetComponentLookup<PublicTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TravelPurposeData = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkerData = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StudentData = InternalCompilerInterface.GetComponentLookup<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMemberData = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TouristHouseholdData = InternalCompilerInterface.GetComponentLookup<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHouseholdData = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterData = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InDangerData = InternalCompilerInterface.GetComponentLookup<InDanger>(ref __TypeHandle.__Game_Events_InDanger_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportVehicleSelectData = m_TransportVehicleSelectData,
			m_RandomSeed = RandomSeed.Next(),
			m_EvacuationRequestArchetype = m_EvacuationRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_ParkedToMovingRemoveTypes = m_ParkedToMovingRemoveTypes,
			m_ParkedToMovingCarAddTypes = m_ParkedToMovingCarAddTypes,
			m_DangerLevelExitProbability = ((AnimationCurve1)(ref singleton.m_EmergencyShelterDangerLevelExitProbability)).Evaluate(dangerLevel),
			m_InoperableExitProbability = singleton.m_InoperableEmergencyShelterExitProbability
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		emergencyShelterTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		emergencyShelterTickJob.m_ActionQueue = actionQueue.AsParallelWriter();
		EmergencyShelterTickJob emergencyShelterTickJob2 = emergencyShelterTickJob;
		EmergencyShelterActionJob obj = new EmergencyShelterActionJob
		{
			m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActionQueue = actionQueue
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<EmergencyShelterTickJob>(emergencyShelterTickJob2, m_BuildingQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, jobHandle));
		JobHandle val3 = IJobExtensions.Schedule<EmergencyShelterActionJob>(obj, val2);
		actionQueue.Dispose(val3);
		m_TransportVehicleSelectData.PostUpdate(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<Game.City.DangerLevel>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1553762682_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<DisasterConfigurationData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1553762682_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public EmergencyShelterAISystem()
	{
	}
}

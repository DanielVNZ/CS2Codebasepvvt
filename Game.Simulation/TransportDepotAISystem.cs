using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Events;
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
public class TransportDepotAISystem : GameSystemBase
{
	private enum DepotActionType : byte
	{
		SetDisabled,
		ClearOdometer
	}

	private struct DepotAction
	{
		public Entity m_Entity;

		public DepotActionType m_Type;

		public bool m_Disabled;

		public static DepotAction SetDisabled(Entity vehicle, bool disabled)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return new DepotAction
			{
				m_Entity = vehicle,
				m_Type = DepotActionType.SetDisabled,
				m_Disabled = disabled
			};
		}

		public static DepotAction ClearOdometer(Entity vehicle)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return new DepotAction
			{
				m_Entity = vehicle,
				m_Type = DepotActionType.ClearOdometer
			};
		}
	}

	[BurstCompile]
	private struct TransportDepotTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentTypeHandle<SpectatorSite> m_SpectatorSiteType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<EventData> m_EventType;

		[ReadOnly]
		public ComponentTypeHandle<VehicleLaunchData> m_VehicleLaunchType;

		[ReadOnly]
		public ComponentTypeHandle<Locked> m_LockedType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> m_OwnedVehicleType;

		public ComponentTypeHandle<Game.Buildings.TransportDepot> m_TransportDepotType;

		public BufferTypeHandle<ServiceDispatch> m_ServiceRequestType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<TransportVehicleRequest> m_TransportVehicleRequestData;

		[ReadOnly]
		public ComponentLookup<TaxiRequest> m_TaxiRequestData;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		[ReadOnly]
		public ComponentLookup<VehicleModel> m_VehicleModelData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.Color> m_RouteColorData;

		[ReadOnly]
		public ComponentLookup<Produced> m_ProducedData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.CargoTransport> m_CargoTransportData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Taxi> m_TaxiData;

		[ReadOnly]
		public ComponentLookup<Odometer> m_OdometerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> m_PrefabTransportDepotData;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_PrefabTransportLineData;

		[ReadOnly]
		public ComponentLookup<TaxiData> m_PrefabTaxiData;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> m_PrefabPublicTransportVehicleData;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> m_PrefabCargoTransportVehicleData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_ActivityLocationElements;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_TransportVehicleRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_TaxiRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingTaxiAddTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingBusAddTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingTrainAddTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingTrainControllerAddTypes;

		[ReadOnly]
		public TransportVehicleSelectData m_TransportVehicleSelectData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_EventPrefabChunks;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<DepotAction> m_ActionQueue;

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
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Game.Buildings.TransportDepot> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.TransportDepot>(ref m_TransportDepotType);
			BufferAccessor<Efficiency> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			BufferAccessor<OwnedVehicle> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedVehicle>(ref m_OwnedVehicleType);
			BufferAccessor<ServiceDispatch> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceRequestType);
			bool isOutsideConnection = ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType);
			bool isSpectatorSite = ((ArchetypeChunk)(ref chunk)).Has<SpectatorSite>(ref m_SpectatorSiteType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity entity = nativeArray[i];
				Transform transform = nativeArray2[i];
				PrefabRef prefabRef = nativeArray3[i];
				Game.Buildings.TransportDepot transportDepot = nativeArray4[i];
				DynamicBuffer<OwnedVehicle> vehicles = bufferAccessor3[i];
				DynamicBuffer<ServiceDispatch> dispatches = bufferAccessor4[i];
				TransportDepotData data = default(TransportDepotData);
				if (m_PrefabTransportDepotData.HasComponent(prefabRef.m_Prefab))
				{
					data = m_PrefabTransportDepotData[prefabRef.m_Prefab];
				}
				if (bufferAccessor2.Length != 0)
				{
					UpgradeUtils.CombineStats<TransportDepotData>(ref data, bufferAccessor2[i], ref m_PrefabRefData, ref m_PrefabTransportDepotData);
				}
				float efficiency = BuildingUtils.GetEfficiency(bufferAccessor, i);
				float immediateEfficiency = BuildingUtils.GetImmediateEfficiency(bufferAccessor, i);
				Tick(unfilteredChunkIndex, ref random, entity, transform, prefabRef, ref transportDepot, data, vehicles, dispatches, efficiency, immediateEfficiency, isOutsideConnection, isSpectatorSite);
				nativeArray4[i] = transportDepot;
			}
		}

		private unsafe void Tick(int jobIndex, ref Random random, Entity entity, Transform transform, PrefabRef prefabRef, ref Game.Buildings.TransportDepot transportDepot, TransportDepotData prefabTransportDepotData, DynamicBuffer<OwnedVehicle> vehicles, DynamicBuffer<ServiceDispatch> dispatches, float efficiency, float immediateEfficiency, bool isOutsideConnection, bool isSpectatorSite)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			int vehicleCapacity = BuildingUtils.GetVehicleCapacity(efficiency, prefabTransportDepotData.m_VehicleCapacity);
			int num = BuildingUtils.GetVehicleCapacity(immediateEfficiency, prefabTransportDepotData.m_VehicleCapacity);
			int num2 = vehicleCapacity;
			int num3 = 0;
			Entity val = Entity.Null;
			int length = vehicles.Length;
			StackList<Entity> parkedVehicles = StackList<Entity>.op_Implicit(new Span<Entity>((void*)stackalloc Entity[length], length));
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			Game.Vehicles.CargoTransport cargoTransport = default(Game.Vehicles.CargoTransport);
			Game.Vehicles.Taxi taxi = default(Game.Vehicles.Taxi);
			ParkedCar parkedCar = default(ParkedCar);
			ParkedTrain parkedTrain = default(ParkedTrain);
			Odometer odometer = default(Odometer);
			PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
			CargoTransportVehicleData cargoTransportVehicleData = default(CargoTransportVehicleData);
			TaxiData taxiData = default(TaxiData);
			DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
			for (int i = 0; i < vehicles.Length; i++)
			{
				Entity vehicle = vehicles[i].m_Vehicle;
				bool flag;
				if (m_PublicTransportData.TryGetComponent(vehicle, ref publicTransport))
				{
					if ((publicTransport.m_State & PublicTransportFlags.DummyTraffic) != 0)
					{
						continue;
					}
					flag = (publicTransport.m_State & PublicTransportFlags.Disabled) != 0;
				}
				else if (m_CargoTransportData.TryGetComponent(vehicle, ref cargoTransport))
				{
					if ((cargoTransport.m_State & CargoTransportFlags.DummyTraffic) != 0)
					{
						continue;
					}
					flag = (cargoTransport.m_State & CargoTransportFlags.Disabled) != 0;
				}
				else
				{
					if (!m_TaxiData.TryGetComponent(vehicle, ref taxi))
					{
						continue;
					}
					flag = (taxi.m_State & TaxiFlags.Disabled) != 0;
				}
				bool flag2 = m_ParkedCarData.TryGetComponent(vehicle, ref parkedCar);
				bool flag3 = m_ParkedTrainData.TryGetComponent(vehicle, ref parkedTrain);
				if (flag2 || flag3)
				{
					if (m_OdometerData.TryGetComponent(vehicle, ref odometer) && odometer.m_Distance != 0f)
					{
						if (m_PrefabPublicTransportVehicleData.TryGetComponent(prefabRef.m_Prefab, ref publicTransportVehicleData))
						{
							if (publicTransportVehicleData.m_MaintenanceRange > 0.1f)
							{
								transportDepot.m_MaintenanceRequirement += math.saturate(odometer.m_Distance / publicTransportVehicleData.m_MaintenanceRange);
							}
						}
						else if (m_PrefabCargoTransportVehicleData.TryGetComponent(prefabRef.m_Prefab, ref cargoTransportVehicleData))
						{
							if (cargoTransportVehicleData.m_MaintenanceRange > 0.1f)
							{
								transportDepot.m_MaintenanceRequirement += math.saturate(odometer.m_Distance / cargoTransportVehicleData.m_MaintenanceRange);
							}
						}
						else if (m_PrefabTaxiData.TryGetComponent(prefabRef.m_Prefab, ref taxiData) && taxiData.m_MaintenanceRange > 0.1f)
						{
							transportDepot.m_MaintenanceRequirement += math.saturate(odometer.m_Distance / taxiData.m_MaintenanceRange);
						}
						m_ActionQueue.Enqueue(DepotAction.ClearOdometer(vehicle));
					}
					if ((flag2 && !((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(parkedCar.m_Lane)) || (flag3 && !((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(parkedTrain.m_ParkingLocation)))
					{
						m_LayoutElements.TryGetBuffer(vehicle, ref layout);
						VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, vehicle, layout);
					}
					else
					{
						parkedVehicles.AddNoResize(vehicle);
					}
				}
				else
				{
					num2--;
					num3++;
					bool flag4 = --num < 0;
					if (flag != flag4)
					{
						m_ActionQueue.Enqueue(DepotAction.SetDisabled(vehicle, flag4));
					}
					if (m_ProducedData.HasComponent(vehicle))
					{
						val = vehicle;
					}
				}
			}
			if (prefabTransportDepotData.m_MaintenanceDuration > 0f)
			{
				float num4 = 256f / (262144f * prefabTransportDepotData.m_MaintenanceDuration) * efficiency;
				transportDepot.m_MaintenanceRequirement = math.max(0f, transportDepot.m_MaintenanceRequirement - num4);
				num2 -= Mathf.CeilToInt(transportDepot.m_MaintenanceRequirement - 0.001f);
			}
			if (prefabTransportDepotData.m_ProductionDuration > 0f)
			{
				float num5 = 256f / (262144f * prefabTransportDepotData.m_ProductionDuration) * efficiency;
				if (num5 > 0f)
				{
					if (val != Entity.Null)
					{
						Produced produced = m_ProducedData[val];
						if (produced.m_Completed < 1f)
						{
							produced.m_Completed = math.min(1f, produced.m_Completed + num5);
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Produced>(jobIndex, val, produced);
						}
						if (produced.m_Completed == 1f && !isSpectatorSite)
						{
							TryCreateLaunchEvent(jobIndex, entity, prefabTransportDepotData);
						}
					}
					else if (num2 > 0 && !isSpectatorSite)
					{
						SpawnVehicle(jobIndex, ref random, entity, Entity.Null, transform, prefabRef, ref transportDepot, ref parkedVehicles, prefabTransportDepotData, num5);
						num2--;
						num3++;
					}
				}
			}
			int num6 = 0;
			bool flag5 = false;
			while (num6 < dispatches.Length)
			{
				Entity request = dispatches[num6].m_Request;
				if (m_TransportVehicleRequestData.HasComponent(request) || m_TaxiRequestData.HasComponent(request))
				{
					if (num2 > 0)
					{
						if (!flag5)
						{
							flag5 = SpawnVehicle(jobIndex, ref random, entity, request, transform, prefabRef, ref transportDepot, ref parkedVehicles, prefabTransportDepotData, 0f);
							dispatches.RemoveAt(num6);
							if (flag5)
							{
								num3++;
							}
						}
						if (flag5)
						{
							num2--;
						}
					}
					else
					{
						dispatches.RemoveAt(num6);
					}
				}
				else if (!m_ServiceRequestData.HasComponent(request))
				{
					dispatches.RemoveAt(num6);
				}
				else
				{
					num6++;
				}
			}
			DynamicBuffer<LayoutElement> layout2 = default(DynamicBuffer<LayoutElement>);
			while (parkedVehicles.Length > math.max(0, prefabTransportDepotData.m_VehicleCapacity - num3))
			{
				int num7 = ((Random)(ref random)).NextInt(parkedVehicles.Length);
				Entity val2 = parkedVehicles[num7];
				m_LayoutElements.TryGetBuffer(val2, ref layout2);
				VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, val2, layout2);
				parkedVehicles.RemoveAtSwapBack(num7);
			}
			Game.Vehicles.PublicTransport publicTransport2 = default(Game.Vehicles.PublicTransport);
			Game.Vehicles.CargoTransport cargoTransport2 = default(Game.Vehicles.CargoTransport);
			Game.Vehicles.Taxi taxi2 = default(Game.Vehicles.Taxi);
			for (int j = 0; j < parkedVehicles.Length; j++)
			{
				Entity val3 = parkedVehicles[j];
				bool flag6;
				if (m_PublicTransportData.TryGetComponent(val3, ref publicTransport2))
				{
					flag6 = (publicTransport2.m_State & PublicTransportFlags.Disabled) != 0;
				}
				else if (m_CargoTransportData.TryGetComponent(val3, ref cargoTransport2))
				{
					flag6 = (cargoTransport2.m_State & CargoTransportFlags.Disabled) != 0;
				}
				else
				{
					if (!m_TaxiData.TryGetComponent(val3, ref taxi2))
					{
						continue;
					}
					flag6 = (taxi2.m_State & TaxiFlags.Disabled) != 0;
				}
				bool flag7 = num2 <= 0;
				if (flag6 != flag7)
				{
					m_ActionQueue.Enqueue(DepotAction.SetDisabled(val3, flag7));
				}
			}
			transportDepot.m_AvailableVehicles = (byte)math.clamp(num2, 0, 255);
			if (num2 > 0)
			{
				transportDepot.m_Flags |= TransportDepotFlags.HasAvailableVehicles;
				RequestTargetIfNeeded(jobIndex, entity, ref transportDepot, prefabTransportDepotData, num2, vehicleCapacity, isOutsideConnection);
			}
			else
			{
				transportDepot.m_Flags &= ~TransportDepotFlags.HasAvailableVehicles;
			}
			if (prefabTransportDepotData.m_DispatchCenter && efficiency > 0.001f)
			{
				transportDepot.m_Flags |= TransportDepotFlags.HasDispatchCenter;
			}
			else
			{
				transportDepot.m_Flags &= ~TransportDepotFlags.HasDispatchCenter;
			}
		}

		private void RequestTargetIfNeeded(int jobIndex, Entity entity, ref Game.Buildings.TransportDepot transportDepot, TransportDepotData prefabTransportDepotData, int availableVehicles, int vehicleCapacity, bool isOutsideConnection)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			if (!m_ServiceRequestData.HasComponent(transportDepot.m_TargetRequest))
			{
				if (prefabTransportDepotData.m_TransportType == TransportType.Taxi)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_TaxiRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TaxiRequest>(jobIndex, val, new TaxiRequest(entity, Entity.Null, Entity.Null, isOutsideConnection ? TaxiRequestType.Outside : TaxiRequestType.None, availableVehicles));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(16u));
				}
				else
				{
					Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_TransportVehicleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val2, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TransportVehicleRequest>(jobIndex, val2, new TransportVehicleRequest(entity, (float)availableVehicles / (float)vehicleCapacity));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val2, new RequestGroup(8u));
				}
			}
		}

		private bool TryCreateLaunchEvent(int jobIndex, Entity entity, TransportDepotData prefabTransportDepotData)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_EventPrefabChunks.Length; i++)
			{
				ArchetypeChunk val = m_EventPrefabChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<EventData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<EventData>(ref m_EventType);
				NativeArray<VehicleLaunchData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<VehicleLaunchData>(ref m_VehicleLaunchType);
				EnabledMask enabledMask = ((ArchetypeChunk)(ref val)).GetEnabledMask<Locked>(ref m_LockedType);
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					SafeBitRef enableBit = ((EnabledMask)(ref enabledMask)).EnableBit;
					if ((!((SafeBitRef)(ref enableBit)).IsValid || !((EnabledMask)(ref enabledMask))[j]) && nativeArray3[j].m_TransportType == prefabTransportDepotData.m_TransportType)
					{
						Entity prefab = nativeArray[j];
						EventData eventData = nativeArray2[j];
						Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, eventData.m_Archetype);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val2, new PrefabRef(prefab));
						((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<TargetElement>(jobIndex, val2).Add(new TargetElement(entity));
						return true;
					}
				}
			}
			return false;
		}

		private bool SpawnVehicle(int jobIndex, ref Random random, Entity entity, Entity request, Transform transform, PrefabRef prefabRef, ref Game.Buildings.TransportDepot transportDepot, ref StackList<Entity> parkedVehicles, TransportDepotData prefabTransportDepotData, float productionState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0743: Unknown result type (might be due to invalid IL or missing references)
			//IL_0748: Unknown result type (might be due to invalid IL or missing references)
			//IL_074d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0756: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_075f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06db: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_0696: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0627: Unknown result type (might be due to invalid IL or missing references)
			//IL_0614: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_064c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			Entity val2 = Entity.Null;
			Entity val3 = Entity.Null;
			Entity primaryPrefab = Entity.Null;
			Entity secondaryPrefab = Entity.Null;
			PublicTransportPurpose publicTransportPurpose = (PublicTransportPurpose)0;
			Resource resource = Resource.NoResource;
			int2 passengerCapacity = int2.op_Implicit(0);
			int2 cargoCapacity = int2.op_Implicit(0);
			TaxiRequestType taxiRequestType = TaxiRequestType.None;
			PathInformation pathInformation = default(PathInformation);
			if (productionState == 0f)
			{
				TransportVehicleRequest transportVehicleRequest = default(TransportVehicleRequest);
				TaxiRequest taxiRequest = default(TaxiRequest);
				if (m_TransportVehicleRequestData.TryGetComponent(request, ref transportVehicleRequest))
				{
					val = transportVehicleRequest.m_Route;
				}
				else if (m_TaxiRequestData.TryGetComponent(request, ref taxiRequest))
				{
					val = taxiRequest.m_Seeker;
					taxiRequestType = taxiRequest.m_Type;
					((int2)(ref passengerCapacity))._002Ector(1, int.MaxValue);
				}
				PrefabRef prefabRef2 = default(PrefabRef);
				if (!m_PrefabRefData.TryGetComponent(val, ref prefabRef2))
				{
					return false;
				}
				TransportLineData transportLineData = default(TransportLineData);
				if (m_PrefabTransportLineData.TryGetComponent(prefabRef2.m_Prefab, ref transportLineData))
				{
					publicTransportPurpose = (transportLineData.m_PassengerTransport ? PublicTransportPurpose.TransportLine : ((PublicTransportPurpose)0));
					resource = (Resource)(transportLineData.m_CargoTransport ? 8 : 0);
					passengerCapacity = (int2)(transportLineData.m_PassengerTransport ? new int2(1, int.MaxValue) : int2.op_Implicit(0));
					cargoCapacity = (int2)(transportLineData.m_CargoTransport ? new int2(1, int.MaxValue) : int2.op_Implicit(0));
				}
				if (!m_PathInformationData.TryGetComponent(request, ref pathInformation))
				{
					return false;
				}
				val2 = pathInformation.m_Destination;
				val3 = pathInformation.m_Origin;
				if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(val2))
				{
					return false;
				}
				VehicleModel vehicleModel = default(VehicleModel);
				if (m_VehicleModelData.TryGetComponent(val, ref vehicleModel))
				{
					primaryPrefab = vehicleModel.m_PrimaryPrefab;
					secondaryPrefab = vehicleModel.m_SecondaryPrefab;
				}
			}
			else
			{
				DynamicBuffer<ActivityLocationElement> val4 = default(DynamicBuffer<ActivityLocationElement>);
				if (m_ActivityLocationElements.TryGetBuffer(prefabRef.m_Prefab, ref val4))
				{
					ActivityMask activityMask = new ActivityMask(ActivityType.Producing);
					for (int i = 0; i < val4.Length; i++)
					{
						ActivityLocationElement activityLocationElement = val4[i];
						if ((activityLocationElement.m_ActivityMask.m_Mask & activityMask.m_Mask) != 0)
						{
							transform = ObjectUtils.LocalToWorld(transform, activityLocationElement.m_Position, activityLocationElement.m_Rotation);
						}
					}
				}
				publicTransportPurpose = PublicTransportPurpose.Other;
			}
			Entity val5 = Entity.Null;
			if (val3 != Entity.Null && val3 != entity)
			{
				if (!CollectionUtils.RemoveValueSwapBack<Entity>(ref parkedVehicles, val3))
				{
					return false;
				}
				val5 = val3;
				DynamicBuffer<LayoutElement> val6 = default(DynamicBuffer<LayoutElement>);
				m_LayoutElements.TryGetBuffer(val5, ref val6);
				switch (prefabTransportDepotData.m_TransportType)
				{
				case TransportType.Taxi:
				{
					ParkedCar parkedCar = m_ParkedCarData[val5];
					Game.Vehicles.CarLaneFlags flags = Game.Vehicles.CarLaneFlags.EndReached | Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.FixedLane;
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val5, ref m_ParkedToMovingRemoveTypes);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val5, ref m_ParkedToMovingTaxiAddTypes);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarCurrentLane>(jobIndex, val5, new CarCurrentLane(parkedCar, flags));
					if (m_ParkingLaneData.HasComponent(parkedCar.m_Lane) || m_SpawnLocationData.HasComponent(parkedCar.m_Lane))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, parkedCar.m_Lane);
					}
					break;
				}
				case TransportType.Bus:
				{
					ParkedCar parkedCar2 = m_ParkedCarData[val5];
					Game.Vehicles.CarLaneFlags flags2 = Game.Vehicles.CarLaneFlags.EndReached | Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.FixedLane;
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val5, ref m_ParkedToMovingRemoveTypes);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val5, ref m_ParkedToMovingBusAddTypes);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarCurrentLane>(jobIndex, val5, new CarCurrentLane(parkedCar2, flags2));
					if (m_ParkingLaneData.HasComponent(parkedCar2.m_Lane) || m_SpawnLocationData.HasComponent(parkedCar2.m_Lane))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, parkedCar2.m_Lane);
					}
					break;
				}
				case TransportType.Train:
				case TransportType.Tram:
				case TransportType.Subway:
				{
					for (int j = 0; j < val6.Length; j++)
					{
						Entity vehicle = val6[j].m_Vehicle;
						ParkedTrain parkedTrain = m_ParkedTrainData[vehicle];
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, vehicle, ref m_ParkedToMovingRemoveTypes);
						if (vehicle == val5)
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, vehicle, ref m_ParkedToMovingTrainControllerAddTypes);
							if (m_SpawnLocationData.HasComponent(parkedTrain.m_ParkingLocation))
							{
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, parkedTrain.m_ParkingLocation);
							}
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, vehicle, ref m_ParkedToMovingTrainAddTypes);
						}
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TrainCurrentLane>(jobIndex, vehicle, new TrainCurrentLane(parkedTrain));
					}
					break;
				}
				}
			}
			if (val5 == Entity.Null)
			{
				val5 = m_TransportVehicleSelectData.CreateVehicle(m_CommandBuffer, jobIndex, ref random, transform, val3, primaryPrefab, secondaryPrefab, prefabTransportDepotData.m_TransportType, prefabTransportDepotData.m_EnergyTypes, prefabTransportDepotData.m_SizeClass, publicTransportPurpose, resource, ref passengerCapacity, ref cargoCapacity, parked: false);
				if (val5 == Entity.Null)
				{
					return false;
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val5, new Owner(entity));
				TransportType transportType = prefabTransportDepotData.m_TransportType;
				if (transportType == TransportType.Train || transportType == TransportType.Tram || transportType == TransportType.Subway)
				{
					RemoveCollidingParkedTrain(jobIndex, request, ref parkedVehicles);
				}
			}
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(jobIndex, val5, new Target(val2));
			if (productionState > 0f)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Moving>(jobIndex, val5);
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TransformFrame>(jobIndex, val5);
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<InterpolatedTransform>(jobIndex, val5);
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Swaying>(jobIndex, val5);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Produced>(jobIndex, val5, new Produced(productionState));
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Stopped>(jobIndex, val5, default(Stopped));
			}
			else
			{
				bool flag = taxiRequestType == TaxiRequestType.Customer || taxiRequestType == TaxiRequestType.Outside;
				if (flag)
				{
					if (prefabTransportDepotData.m_TransportType == TransportType.Taxi)
					{
						TaxiFlags taxiFlags = TaxiFlags.Dispatched;
						if (taxiRequestType == TaxiRequestType.Outside)
						{
							taxiFlags |= TaxiFlags.FromOutside;
						}
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Vehicles.Taxi>(jobIndex, val5, new Game.Vehicles.Taxi(taxiFlags));
						((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<ServiceDispatch>(jobIndex, val5).Add(new ServiceDispatch(request));
					}
				}
				else if (val != Entity.Null)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentRoute>(jobIndex, val5, new CurrentRoute(val));
					if (val3 == val5)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AppendToBuffer<RouteVehicle>(jobIndex, val, new RouteVehicle(val5));
					}
					Game.Routes.Color color = default(Game.Routes.Color);
					if (m_RouteColorData.TryGetComponent(val, ref color))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Routes.Color>(jobIndex, val5, color);
					}
					if (publicTransportPurpose != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Vehicles.PublicTransport>(jobIndex, val5, new Game.Vehicles.PublicTransport
						{
							m_State = PublicTransportFlags.EnRoute
						});
					}
					if (resource != Resource.NoResource)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Vehicles.CargoTransport>(jobIndex, val5, new Game.Vehicles.CargoTransport
						{
							m_State = CargoTransportFlags.EnRoute
						});
					}
				}
				Entity val7 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val7, new HandleRequest(request, val5, !flag));
			}
			DynamicBuffer<PathElement> sourceElements = default(DynamicBuffer<PathElement>);
			if (m_PathElements.TryGetBuffer(request, ref sourceElements) && sourceElements.Length != 0)
			{
				DynamicBuffer<PathElement> targetElements = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<PathElement>(jobIndex, val5);
				PathUtils.CopyPath(sourceElements, default(PathOwner), 0, targetElements);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathOwner>(jobIndex, val5, new PathOwner(PathFlags.Updated));
				if (prefabTransportDepotData.m_TransportType != TransportType.Taxi)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(jobIndex, val5, pathInformation);
				}
			}
			if (m_ServiceRequestData.HasComponent(transportDepot.m_TargetRequest))
			{
				Entity val8 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val8, new HandleRequest(transportDepot.m_TargetRequest, Entity.Null, completed: true));
			}
			return true;
		}

		private void RemoveCollidingParkedTrain(int jobIndex, Entity pathEntity, ref StackList<Entity> parkedVehicles)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PathElement> val = default(DynamicBuffer<PathElement>);
			if (!m_PathElements.TryGetBuffer(pathEntity, ref val) || val.Length == 0)
			{
				return;
			}
			Entity target = val[0].m_Target;
			ParkedTrain parkedTrain = default(ParkedTrain);
			DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
			for (int i = 0; i < parkedVehicles.Length; i++)
			{
				Entity val2 = parkedVehicles[i];
				if (m_ParkedTrainData.TryGetComponent(val2, ref parkedTrain) && parkedTrain.m_ParkingLocation == target)
				{
					m_LayoutElements.TryGetBuffer(val2, ref layout);
					VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, val2, layout);
					parkedVehicles.RemoveAtSwapBack(i);
					break;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct TransportDepotActionJob : IJob
	{
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		public ComponentLookup<Game.Vehicles.CargoTransport> m_CargoTransportData;

		public ComponentLookup<Game.Vehicles.Taxi> m_TaxiData;

		public ComponentLookup<Odometer> m_OdometerData;

		public NativeQueue<DepotAction> m_ActionQueue;

		public void Execute()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			DepotAction depotAction = default(DepotAction);
			Game.Vehicles.PublicTransport publicTransport2 = default(Game.Vehicles.PublicTransport);
			Game.Vehicles.CargoTransport cargoTransport2 = default(Game.Vehicles.CargoTransport);
			Game.Vehicles.Taxi taxi2 = default(Game.Vehicles.Taxi);
			Odometer odometer = default(Odometer);
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			Game.Vehicles.CargoTransport cargoTransport = default(Game.Vehicles.CargoTransport);
			Game.Vehicles.Taxi taxi = default(Game.Vehicles.Taxi);
			while (m_ActionQueue.TryDequeue(ref depotAction))
			{
				switch (depotAction.m_Type)
				{
				case DepotActionType.SetDisabled:
					if (m_PublicTransportData.TryGetComponent(depotAction.m_Entity, ref publicTransport2))
					{
						if (depotAction.m_Disabled)
						{
							publicTransport2.m_State |= PublicTransportFlags.AbandonRoute | PublicTransportFlags.Disabled;
						}
						else
						{
							publicTransport2.m_State &= ~PublicTransportFlags.Disabled;
						}
						m_PublicTransportData[depotAction.m_Entity] = publicTransport2;
					}
					if (m_CargoTransportData.TryGetComponent(depotAction.m_Entity, ref cargoTransport2))
					{
						if (depotAction.m_Disabled)
						{
							cargoTransport2.m_State |= CargoTransportFlags.AbandonRoute | CargoTransportFlags.Disabled;
						}
						else
						{
							cargoTransport2.m_State &= ~CargoTransportFlags.Disabled;
						}
						m_CargoTransportData[depotAction.m_Entity] = cargoTransport2;
					}
					if (m_TaxiData.TryGetComponent(depotAction.m_Entity, ref taxi2))
					{
						if (depotAction.m_Disabled)
						{
							taxi2.m_State |= TaxiFlags.Disabled;
						}
						else
						{
							taxi2.m_State &= ~TaxiFlags.Disabled;
						}
						m_TaxiData[depotAction.m_Entity] = taxi2;
					}
					break;
				case DepotActionType.ClearOdometer:
					if (m_OdometerData.TryGetComponent(depotAction.m_Entity, ref odometer))
					{
						odometer.m_Distance = 0f;
						m_OdometerData[depotAction.m_Entity] = odometer;
					}
					if (m_PublicTransportData.TryGetComponent(depotAction.m_Entity, ref publicTransport))
					{
						publicTransport.m_State &= ~PublicTransportFlags.RequiresMaintenance;
						m_PublicTransportData[depotAction.m_Entity] = publicTransport;
					}
					if (m_CargoTransportData.TryGetComponent(depotAction.m_Entity, ref cargoTransport))
					{
						cargoTransport.m_State &= ~CargoTransportFlags.RequiresMaintenance;
						m_CargoTransportData[depotAction.m_Entity] = cargoTransport;
					}
					if (m_TaxiData.TryGetComponent(depotAction.m_Entity, ref taxi))
					{
						taxi.m_State &= ~TaxiFlags.RequiresMaintenance;
						m_TaxiData[depotAction.m_Entity] = taxi;
					}
					break;
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
		public ComponentTypeHandle<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SpectatorSite> __Game_Events_SpectatorSite_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EventData> __Game_Prefabs_EventData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<VehicleLaunchData> __Game_Prefabs_VehicleLaunchData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Locked> __Game_Prefabs_Locked_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Buildings.TransportDepot> __Game_Buildings_TransportDepot_RW_ComponentTypeHandle;

		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<TransportVehicleRequest> __Game_Simulation_TransportVehicleRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TaxiRequest> __Game_Simulation_TaxiRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<VehicleModel> __Game_Routes_VehicleModel_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.Color> __Game_Routes_Color_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Produced> __Game_Vehicles_Produced_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.CargoTransport> __Game_Vehicles_CargoTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Taxi> __Game_Vehicles_Taxi_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Odometer> __Game_Vehicles_Odometer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> __Game_Prefabs_TransportDepotData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportLineData> __Game_Prefabs_TransportLineData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TaxiData> __Game_Prefabs_TaxiData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> __Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> __Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RW_ComponentLookup;

		public ComponentLookup<Game.Vehicles.CargoTransport> __Game_Vehicles_CargoTransport_RW_ComponentLookup;

		public ComponentLookup<Game.Vehicles.Taxi> __Game_Vehicles_Taxi_RW_ComponentLookup;

		public ComponentLookup<Odometer> __Game_Vehicles_Odometer_RW_ComponentLookup;

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
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_OutsideConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.OutsideConnection>(true);
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Events_SpectatorSite_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SpectatorSite>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_EventData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EventData>(true);
			__Game_Prefabs_VehicleLaunchData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<VehicleLaunchData>(true);
			__Game_Prefabs_Locked_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Locked>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<OwnedVehicle>(true);
			__Game_Buildings_TransportDepot_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.TransportDepot>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Simulation_TransportVehicleRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportVehicleRequest>(true);
			__Game_Simulation_TaxiRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TaxiRequest>(true);
			__Game_Simulation_ServiceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(true);
			__Game_Routes_VehicleModel_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VehicleModel>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Routes_Color_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.Color>(true);
			__Game_Vehicles_Produced_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Produced>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
			__Game_Vehicles_CargoTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.CargoTransport>(true);
			__Game_Vehicles_Taxi_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Taxi>(true);
			__Game_Vehicles_Odometer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Odometer>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_TransportDepotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportDepotData>(true);
			__Game_Prefabs_TransportLineData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLineData>(true);
			__Game_Prefabs_TaxiData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TaxiData>(true);
			__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PublicTransportVehicleData>(true);
			__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CargoTransportVehicleData>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
			__Game_Vehicles_PublicTransport_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(false);
			__Game_Vehicles_CargoTransport_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.CargoTransport>(false);
			__Game_Vehicles_Taxi_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Taxi>(false);
			__Game_Vehicles_Odometer_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Odometer>(false);
		}
	}

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_VehiclePrefabQuery;

	private EntityQuery m_EventPrefabQuery;

	private EntityArchetype m_TransportVehicleRequestArchetype;

	private EntityArchetype m_TaxiRequestArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_ParkedToMovingRemoveTypes;

	private ComponentTypeSet m_ParkedToMovingTaxiAddTypes;

	private ComponentTypeSet m_ParkedToMovingBusAddTypes;

	private ComponentTypeSet m_ParkedToMovingTrainAddTypes;

	private ComponentTypeSet m_ParkedToMovingTrainControllerAddTypes;

	private EndFrameBarrier m_EndFrameBarrier;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private TransportVehicleSelectData m_TransportVehicleSelectData;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 32;
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
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_TransportVehicleSelectData = new TransportVehicleSelectData((SystemBase)(object)this);
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Buildings.TransportDepot>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_VehiclePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { TransportVehicleSelectData.GetEntityQueryDesc() });
		m_EventPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<VehicleLaunchData>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.Exclude<Locked>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_TransportVehicleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<TransportVehicleRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_TaxiRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<TaxiRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_HandleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<HandleRequest>(),
			ComponentType.ReadWrite<Game.Common.Event>()
		});
		m_ParkedToMovingRemoveTypes = new ComponentTypeSet(ComponentType.ReadWrite<ParkedCar>(), ComponentType.ReadWrite<ParkedTrain>(), ComponentType.ReadWrite<Stopped>());
		m_ParkedToMovingTaxiAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[13]
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
			ComponentType.ReadWrite<ServiceDispatch>(),
			ComponentType.ReadWrite<Swaying>(),
			ComponentType.ReadWrite<Updated>()
		});
		m_ParkedToMovingBusAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[14]
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
		m_ParkedToMovingTrainAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<TrainNavigation>(),
			ComponentType.ReadWrite<TrainCurrentLane>(),
			ComponentType.ReadWrite<TrainBogieFrame>(),
			ComponentType.ReadWrite<Updated>()
		});
		m_ParkedToMovingTrainControllerAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[14]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<TrainNavigation>(),
			ComponentType.ReadWrite<TrainNavigationLane>(),
			ComponentType.ReadWrite<TrainCurrentLane>(),
			ComponentType.ReadWrite<TrainBogieFrame>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<PathElement>(),
			ComponentType.ReadWrite<PathInformation>(),
			ComponentType.ReadWrite<ServiceDispatch>(),
			ComponentType.ReadWrite<Updated>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_BuildingQuery);
		Assert.IsTrue(true);
		Assert.IsTrue(true);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		m_TransportVehicleSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_VehiclePrefabQuery, (Allocator)3, out var jobHandle);
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> eventPrefabChunks = ((EntityQuery)(ref m_EventPrefabQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		NativeQueue<DepotAction> actionQueue = default(NativeQueue<DepotAction>);
		actionQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		TransportDepotTickJob transportDepotTickJob = new TransportDepotTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpectatorSiteType = InternalCompilerInterface.GetComponentTypeHandle<SpectatorSite>(ref __TypeHandle.__Game_Events_SpectatorSite_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EventType = InternalCompilerInterface.GetComponentTypeHandle<EventData>(ref __TypeHandle.__Game_Prefabs_EventData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleLaunchType = InternalCompilerInterface.GetComponentTypeHandle<VehicleLaunchData>(ref __TypeHandle.__Game_Prefabs_VehicleLaunchData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LockedType = InternalCompilerInterface.GetComponentTypeHandle<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicleType = InternalCompilerInterface.GetBufferTypeHandle<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportDepotType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.TransportDepot>(ref __TypeHandle.__Game_Buildings_TransportDepot_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportVehicleRequestData = InternalCompilerInterface.GetComponentLookup<TransportVehicleRequest>(ref __TypeHandle.__Game_Simulation_TransportVehicleRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiRequestData = InternalCompilerInterface.GetComponentLookup<TaxiRequest>(ref __TypeHandle.__Game_Simulation_TaxiRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleModelData = InternalCompilerInterface.GetComponentLookup<VehicleModel>(ref __TypeHandle.__Game_Routes_VehicleModel_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteColorData = InternalCompilerInterface.GetComponentLookup<Game.Routes.Color>(ref __TypeHandle.__Game_Routes_Color_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProducedData = InternalCompilerInterface.GetComponentLookup<Produced>(ref __TypeHandle.__Game_Vehicles_Produced_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CargoTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.CargoTransport>(ref __TypeHandle.__Game_Vehicles_CargoTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Taxi>(ref __TypeHandle.__Game_Vehicles_Taxi_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OdometerData = InternalCompilerInterface.GetComponentLookup<Odometer>(ref __TypeHandle.__Game_Vehicles_Odometer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransportDepotData = InternalCompilerInterface.GetComponentLookup<TransportDepotData>(ref __TypeHandle.__Game_Prefabs_TransportDepotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTaxiData = InternalCompilerInterface.GetComponentLookup<TaxiData>(ref __TypeHandle.__Game_Prefabs_TaxiData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPublicTransportVehicleData = InternalCompilerInterface.GetComponentLookup<PublicTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCargoTransportVehicleData = InternalCompilerInterface.GetComponentLookup<CargoTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActivityLocationElements = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_TransportVehicleRequestArchetype = m_TransportVehicleRequestArchetype,
			m_TaxiRequestArchetype = m_TaxiRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_ParkedToMovingRemoveTypes = m_ParkedToMovingRemoveTypes,
			m_ParkedToMovingTaxiAddTypes = m_ParkedToMovingTaxiAddTypes,
			m_ParkedToMovingBusAddTypes = m_ParkedToMovingBusAddTypes,
			m_ParkedToMovingTrainAddTypes = m_ParkedToMovingTrainAddTypes,
			m_ParkedToMovingTrainControllerAddTypes = m_ParkedToMovingTrainControllerAddTypes,
			m_TransportVehicleSelectData = m_TransportVehicleSelectData,
			m_EventPrefabChunks = eventPrefabChunks
		};
		EntityCommandBuffer val2 = m_EndFrameBarrier.CreateCommandBuffer();
		transportDepotTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		transportDepotTickJob.m_ActionQueue = actionQueue.AsParallelWriter();
		TransportDepotTickJob transportDepotTickJob2 = transportDepotTickJob;
		TransportDepotActionJob obj = new TransportDepotActionJob
		{
			m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CargoTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.CargoTransport>(ref __TypeHandle.__Game_Vehicles_CargoTransport_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Taxi>(ref __TypeHandle.__Game_Vehicles_Taxi_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OdometerData = InternalCompilerInterface.GetComponentLookup<Odometer>(ref __TypeHandle.__Game_Vehicles_Odometer_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActionQueue = actionQueue
		};
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<TransportDepotTickJob>(transportDepotTickJob2, m_BuildingQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, jobHandle, val));
		JobHandle val4 = IJobExtensions.Schedule<TransportDepotActionJob>(obj, val3);
		eventPrefabChunks.Dispose(val3);
		actionQueue.Dispose(val4);
		m_TransportVehicleSelectData.PostUpdate(val3);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		((SystemBase)this).Dependency = val4;
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
	public TransportDepotAISystem()
	{
	}
}

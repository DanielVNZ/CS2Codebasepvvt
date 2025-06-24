using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
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
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class GarbageTruckAISystem : GameSystemBase
{
	private enum GarbageActionType
	{
		Collect,
		Unload,
		AddRequest,
		ClearLane,
		BumpDispatchIndex
	}

	private struct GarbageAction
	{
		public Entity m_Vehicle;

		public Entity m_Target;

		public Entity m_Request;

		public GarbageActionType m_Type;

		public int m_Capacity;

		public int m_MaxAmount;
	}

	[BurstCompile]
	private struct GarbageTruckTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		public ComponentTypeHandle<Game.Vehicles.GarbageTruck> m_GarbageTruckType;

		public ComponentTypeHandle<Car> m_CarType;

		public ComponentTypeHandle<CarCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<Target> m_TargetType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public BufferTypeHandle<CarNavigationLane> m_CarNavigationLaneType;

		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<Quantity> m_QuantityData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<GarageLane> m_GarageLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<GarbageTruckData> m_PrefabGarbageTruckData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_PrefabSpawnableBuildingData;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_PrefabZoneData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public ComponentLookup<GarbageCollectionRequest> m_GarbageCollectionRequestData;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> m_GarbageProducerData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.GarbageFacility> m_GarbageFacilityData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<ConnectedBuilding> m_ConnectedBuildings;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<LaneOverlap> m_LaneOverlaps;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		[NativeDisableParallelForRestriction]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_GarbageCollectionRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedCarRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedAddTypes;

		[ReadOnly]
		public GarbageParameterData m_GarbageParameters;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public ParallelWriter<GarbageAction> m_ActionQueue;

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
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<PathInformation> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
			NativeArray<CarCurrentLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CurrentLaneType);
			NativeArray<Game.Vehicles.GarbageTruck> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.GarbageTruck>(ref m_GarbageTruckType);
			NativeArray<Car> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Car>(ref m_CarType);
			NativeArray<Target> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PathOwner> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			BufferAccessor<CarNavigationLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<CarNavigationLane>(ref m_CarNavigationLaneType);
			BufferAccessor<ServiceDispatch> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Owner owner = nativeArray2[i];
				PrefabRef prefabRef = nativeArray3[i];
				PathInformation pathInformation = nativeArray4[i];
				Game.Vehicles.GarbageTruck garbageTruck = nativeArray6[i];
				Car car = nativeArray7[i];
				CarCurrentLane currentLane = nativeArray5[i];
				PathOwner pathOwner = nativeArray9[i];
				Target target = nativeArray8[i];
				DynamicBuffer<CarNavigationLane> navigationLanes = bufferAccessor[i];
				DynamicBuffer<ServiceDispatch> serviceDispatches = bufferAccessor2[i];
				VehicleUtils.CheckUnspawned(unfilteredChunkIndex, val, currentLane, isUnspawned, m_CommandBuffer);
				Tick(unfilteredChunkIndex, val, owner, prefabRef, pathInformation, navigationLanes, serviceDispatches, ref random, ref garbageTruck, ref car, ref currentLane, ref pathOwner, ref target);
				nativeArray6[i] = garbageTruck;
				nativeArray7[i] = car;
				nativeArray5[i] = currentLane;
				nativeArray9[i] = pathOwner;
				nativeArray8[i] = target;
			}
		}

		private void Tick(int jobIndex, Entity vehicleEntity, Owner owner, PrefabRef prefabRef, PathInformation pathInformation, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Random random, ref Game.Vehicles.GarbageTruck garbageTruck, ref Car car, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			if (VehicleUtils.ResetUpdatedPath(ref pathOwner))
			{
				ResetPath(jobIndex, vehicleEntity, pathInformation, serviceDispatches, owner, ref random, ref garbageTruck, ref car, ref currentLane, ref pathOwner);
			}
			GarbageTruckData prefabGarbageTruckData = m_PrefabGarbageTruckData[prefabRef.m_Prefab];
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(target.m_Target) || VehicleUtils.PathfindFailed(pathOwner))
			{
				if (VehicleUtils.IsStuck(pathOwner) || (garbageTruck.m_State & GarbageTruckFlags.Returning) != 0)
				{
					if (UnloadGarbage(jobIndex, vehicleEntity, prefabGarbageTruckData, owner.m_Owner, ref garbageTruck, instant: true))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
					}
					return;
				}
				ReturnToDepot(owner, serviceDispatches, ref garbageTruck, ref car, ref pathOwner, ref target);
			}
			else if (VehicleUtils.PathEndReached(currentLane))
			{
				if ((garbageTruck.m_State & GarbageTruckFlags.Returning) != 0)
				{
					if (UnloadGarbage(jobIndex, vehicleEntity, prefabGarbageTruckData, owner.m_Owner, ref garbageTruck, instant: false))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
					}
					return;
				}
				TryCollectGarbage(jobIndex, vehicleEntity, prefabGarbageTruckData, owner, ref garbageTruck, ref car, ref currentLane, target.m_Target);
				TryCollectGarbage(jobIndex, vehicleEntity, prefabGarbageTruckData, owner, ref garbageTruck, ref car, ref target);
				CheckServiceDispatches(vehicleEntity, serviceDispatches, owner, ref garbageTruck, ref pathOwner);
				if (!SelectNextDispatch(jobIndex, vehicleEntity, navigationLanes, serviceDispatches, owner, ref garbageTruck, ref car, ref currentLane, ref pathOwner, ref target))
				{
					ReturnToDepot(owner, serviceDispatches, ref garbageTruck, ref car, ref pathOwner, ref target);
				}
			}
			else
			{
				if (VehicleUtils.ParkingSpaceReached(currentLane, pathOwner))
				{
					if ((garbageTruck.m_State & GarbageTruckFlags.Returning) != 0)
					{
						if (UnloadGarbage(jobIndex, vehicleEntity, prefabGarbageTruckData, owner.m_Owner, ref garbageTruck, instant: false))
						{
							ParkCar(jobIndex, vehicleEntity, owner, ref garbageTruck, ref car, ref currentLane);
						}
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
					}
					return;
				}
				if (VehicleUtils.WaypointReached(currentLane))
				{
					currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.Waypoint;
					TryCollectGarbage(jobIndex, vehicleEntity, prefabGarbageTruckData, owner, ref garbageTruck, ref car, ref currentLane, Entity.Null);
				}
				else if ((garbageTruck.m_State & GarbageTruckFlags.Unloading) != 0)
				{
					UnloadGarbage(jobIndex, vehicleEntity, prefabGarbageTruckData, owner.m_Owner, ref garbageTruck, instant: true);
				}
			}
			if ((garbageTruck.m_State & GarbageTruckFlags.Returning) == 0)
			{
				if (garbageTruck.m_Garbage >= prefabGarbageTruckData.m_GarbageCapacity || (garbageTruck.m_State & GarbageTruckFlags.Disabled) != 0)
				{
					ReturnToDepot(owner, serviceDispatches, ref garbageTruck, ref car, ref pathOwner, ref target);
				}
				else
				{
					CheckGarbagePresence(owner, ref currentLane, ref garbageTruck, ref car, navigationLanes);
				}
			}
			if (garbageTruck.m_Garbage + garbageTruck.m_EstimatedGarbage >= prefabGarbageTruckData.m_GarbageCapacity)
			{
				garbageTruck.m_State |= GarbageTruckFlags.EstimatedFull;
			}
			else
			{
				garbageTruck.m_State &= ~GarbageTruckFlags.EstimatedFull;
			}
			if (garbageTruck.m_Garbage < prefabGarbageTruckData.m_GarbageCapacity && (garbageTruck.m_State & GarbageTruckFlags.Disabled) == 0)
			{
				CheckServiceDispatches(vehicleEntity, serviceDispatches, owner, ref garbageTruck, ref pathOwner);
				if ((garbageTruck.m_State & GarbageTruckFlags.Returning) != 0)
				{
					SelectNextDispatch(jobIndex, vehicleEntity, navigationLanes, serviceDispatches, owner, ref garbageTruck, ref car, ref currentLane, ref pathOwner, ref target);
				}
				if (garbageTruck.m_RequestCount <= 1 && (garbageTruck.m_State & GarbageTruckFlags.EstimatedFull) == 0)
				{
					RequestTargetIfNeeded(jobIndex, vehicleEntity, ref garbageTruck);
				}
			}
			else
			{
				serviceDispatches.Clear();
			}
			if ((garbageTruck.m_State & GarbageTruckFlags.Unloading) == 0)
			{
				if (VehicleUtils.RequireNewPath(pathOwner))
				{
					FindNewPath(vehicleEntity, prefabRef, ref garbageTruck, ref currentLane, ref pathOwner, ref target);
				}
				else if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Stuck)) == 0)
				{
					CheckParkingSpace(vehicleEntity, ref random, ref currentLane, ref pathOwner, navigationLanes);
				}
			}
		}

		private void CheckParkingSpace(Entity entity, ref Random random, ref CarCurrentLane currentLane, ref PathOwner pathOwner, DynamicBuffer<CarNavigationLane> navigationLanes)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PathElement> path = m_PathElements[entity];
			ComponentLookup<Blocker> blockerData = default(ComponentLookup<Blocker>);
			VehicleUtils.ValidateParkingSpace(entity, ref random, ref currentLane, ref pathOwner, navigationLanes, path, ref m_ParkedCarData, ref blockerData, ref m_CurveData, ref m_UnspawnedData, ref m_ParkingLaneData, ref m_GarageLaneData, ref m_ConnectionLaneData, ref m_PrefabRefData, ref m_PrefabParkingLaneData, ref m_PrefabObjectGeometryData, ref m_LaneObjects, ref m_LaneOverlaps, ignoreDriveways: false, ignoreDisabled: false, boardingOnly: false);
		}

		private void ParkCar(int jobIndex, Entity entity, Owner owner, ref Game.Vehicles.GarbageTruck garbageTruck, ref Car car, ref CarCurrentLane currentLane)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			car.m_Flags &= ~(CarFlags.Warning | CarFlags.Working);
			garbageTruck.m_State &= GarbageTruckFlags.IndustrialWasteOnly;
			Game.Buildings.GarbageFacility garbageFacility = default(Game.Buildings.GarbageFacility);
			if (m_GarbageFacilityData.TryGetComponent(owner.m_Owner, ref garbageFacility) && (garbageFacility.m_Flags & (GarbageFacilityFlags.HasAvailableGarbageTrucks | GarbageFacilityFlags.HasAvailableSpace)) != (GarbageFacilityFlags.HasAvailableGarbageTrucks | GarbageFacilityFlags.HasAvailableSpace))
			{
				garbageTruck.m_State |= GarbageTruckFlags.Disabled;
			}
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, entity, ref m_MovingToParkedCarRemoveTypes);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, entity, ref m_MovingToParkedAddTypes);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkedCar>(jobIndex, entity, new ParkedCar(currentLane.m_Lane, currentLane.m_CurvePosition.x));
			if (m_ParkingLaneData.HasComponent(currentLane.m_Lane) && currentLane.m_ChangeLane == Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, currentLane.m_Lane);
			}
			else if (m_GarageLaneData.HasComponent(currentLane.m_Lane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, currentLane.m_Lane);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<FixParkingLocation>(jobIndex, entity, new FixParkingLocation(currentLane.m_ChangeLane, entity));
			}
			else
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<FixParkingLocation>(jobIndex, entity, new FixParkingLocation(currentLane.m_ChangeLane, entity));
			}
		}

		private void FindNewPath(Entity vehicleEntity, PrefabRef prefabRef, ref Game.Vehicles.GarbageTruck garbageTruck, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			CarData carData = m_PrefabCarData[prefabRef.m_Prefab];
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(carData.m_MaxSpeed),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_Methods = (PathMethod.Road | PathMethod.SpecialParking),
				m_ParkingTarget = VehicleUtils.GetParkingSource(vehicleEntity, currentLane, ref m_ParkingLaneData, ref m_ConnectionLaneData),
				m_ParkingDelta = currentLane.m_CurvePosition.z,
				m_ParkingSize = VehicleUtils.GetParkingSize(vehicleEntity, ref m_PrefabRefData, ref m_PrefabObjectGeometryData),
				m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = (PathMethod.Road | PathMethod.SpecialParking),
				m_RoadTypes = RoadTypes.Car
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = PathMethod.Road,
				m_RoadTypes = RoadTypes.Car,
				m_Entity = target.m_Target
			};
			if ((garbageTruck.m_State & GarbageTruckFlags.Returning) != 0)
			{
				destination.m_Methods |= PathMethod.SpecialParking;
				destination.m_RandomCost = 30f;
			}
			VehicleUtils.SetupPathfind(item: new SetupQueueItem(vehicleEntity, parameters, origin, destination), currentLane: ref currentLane, pathOwner: ref pathOwner, queue: m_PathfindQueue);
		}

		private void CheckServiceDispatches(Entity vehicleEntity, DynamicBuffer<ServiceDispatch> serviceDispatches, Owner owner, ref Game.Vehicles.GarbageTruck garbageTruck, ref PathOwner pathOwner)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			if (serviceDispatches.Length <= garbageTruck.m_RequestCount)
			{
				return;
			}
			int num = -1;
			Entity val = Entity.Null;
			PathElement pathElement = default(PathElement);
			bool flag = false;
			int num2 = 0;
			if (garbageTruck.m_RequestCount >= 1 && (garbageTruck.m_State & GarbageTruckFlags.Returning) == 0)
			{
				DynamicBuffer<PathElement> val2 = m_PathElements[vehicleEntity];
				num2 = 1;
				if (pathOwner.m_ElementIndex < val2.Length)
				{
					pathElement = val2[val2.Length - 1];
					flag = true;
				}
			}
			DynamicBuffer<PathElement> val3 = default(DynamicBuffer<PathElement>);
			for (int i = num2; i < garbageTruck.m_RequestCount; i++)
			{
				Entity request = serviceDispatches[i].m_Request;
				if (m_PathElements.TryGetBuffer(request, ref val3) && val3.Length != 0)
				{
					pathElement = val3[val3.Length - 1];
					flag = true;
				}
			}
			DynamicBuffer<PathElement> val4 = default(DynamicBuffer<PathElement>);
			for (int j = garbageTruck.m_RequestCount; j < serviceDispatches.Length; j++)
			{
				Entity request2 = serviceDispatches[j].m_Request;
				if (!m_GarbageCollectionRequestData.HasComponent(request2))
				{
					continue;
				}
				GarbageCollectionRequest garbageCollectionRequest = m_GarbageCollectionRequestData[request2];
				if (flag && m_PathElements.TryGetBuffer(request2, ref val4) && val4.Length != 0)
				{
					PathElement pathElement2 = val4[0];
					if (pathElement2.m_Target != pathElement.m_Target || pathElement2.m_TargetDelta.x != pathElement.m_TargetDelta.y)
					{
						continue;
					}
				}
				if (m_PrefabRefData.HasComponent(garbageCollectionRequest.m_Target) && garbageCollectionRequest.m_Priority > num)
				{
					num = garbageCollectionRequest.m_Priority;
					val = request2;
				}
			}
			if (val != Entity.Null)
			{
				serviceDispatches[garbageTruck.m_RequestCount++] = new ServiceDispatch(val);
				PreAddCollectionRequests(val, owner, ref garbageTruck);
			}
			if (serviceDispatches.Length > garbageTruck.m_RequestCount)
			{
				serviceDispatches.RemoveRange(garbageTruck.m_RequestCount, serviceDispatches.Length - garbageTruck.m_RequestCount);
			}
		}

		private void PreAddCollectionRequests(Entity request, Owner owner, ref Game.Vehicles.GarbageTruck garbageTruck)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PathElement> val = default(DynamicBuffer<PathElement>);
			if (!m_PathElements.TryGetBuffer(request, ref val))
			{
				return;
			}
			DynamicBuffer<ServiceDistrict> serviceDistricts = default(DynamicBuffer<ServiceDistrict>);
			m_ServiceDistricts.TryGetBuffer(owner.m_Owner, ref serviceDistricts);
			int dispatchIndex = BumpDispachIndex(request);
			Entity val2 = Entity.Null;
			for (int i = 0; i < val.Length; i++)
			{
				PathElement pathElement = val[i];
				if (!m_EdgeLaneData.HasComponent(pathElement.m_Target))
				{
					val2 = Entity.Null;
					continue;
				}
				Owner owner2 = m_OwnerData[pathElement.m_Target];
				if (!(owner2.m_Owner == val2))
				{
					val2 = owner2.m_Owner;
					if (HasSidewalk(owner2.m_Owner))
					{
						garbageTruck.m_EstimatedGarbage += AddCollectionRequests(owner2.m_Owner, request, dispatchIndex, serviceDistricts, ref garbageTruck);
					}
				}
			}
		}

		private bool HasSidewalk(Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (m_SubLanes.HasBuffer(owner))
			{
				DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[owner];
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					if (m_PedestrianLaneData.HasComponent(subLane))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void RequestTargetIfNeeded(int jobIndex, Entity entity, ref Game.Vehicles.GarbageTruck garbageTruck)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			if (!m_GarbageCollectionRequestData.HasComponent(garbageTruck.m_TargetRequest))
			{
				uint num = math.max(512u, 16u);
				if ((m_SimulationFrameIndex & (num - 1)) == 2)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_GarbageCollectionRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<GarbageCollectionRequest>(jobIndex, val, new GarbageCollectionRequest(entity, 1, ((garbageTruck.m_State & GarbageTruckFlags.IndustrialWasteOnly) != 0) ? GarbageCollectionRequestFlags.IndustrialWaste : ((GarbageCollectionRequestFlags)0)));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(32u));
				}
			}
		}

		private bool SelectNextDispatch(int jobIndex, Entity vehicleEntity, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, Owner owner, ref Game.Vehicles.GarbageTruck garbageTruck, ref Car car, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			if ((garbageTruck.m_State & GarbageTruckFlags.Returning) == 0 && garbageTruck.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				serviceDispatches.RemoveAt(0);
				garbageTruck.m_RequestCount--;
			}
			GarbageCollectionRequest garbageCollectionRequest = default(GarbageCollectionRequest);
			DynamicBuffer<ServiceDistrict> serviceDistricts = default(DynamicBuffer<ServiceDistrict>);
			NativeArray<PathElement> val4 = default(NativeArray<PathElement>);
			while (garbageTruck.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				Entity val = Entity.Null;
				if (m_GarbageCollectionRequestData.TryGetComponent(request, ref garbageCollectionRequest))
				{
					val = garbageCollectionRequest.m_Target;
				}
				if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(val))
				{
					serviceDispatches.RemoveAt(0);
					garbageTruck.m_EstimatedGarbage -= garbageTruck.m_EstimatedGarbage / garbageTruck.m_RequestCount;
					garbageTruck.m_RequestCount--;
					continue;
				}
				garbageTruck.m_State &= ~GarbageTruckFlags.Returning;
				car.m_Flags |= CarFlags.UsePublicTransportLanes;
				Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(request, vehicleEntity, completed: false, pathConsumed: true));
				if (m_GarbageCollectionRequestData.HasComponent(garbageTruck.m_TargetRequest))
				{
					val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(garbageTruck.m_TargetRequest, Entity.Null, completed: true));
				}
				if (m_PathElements.HasBuffer(request))
				{
					DynamicBuffer<PathElement> appendPath = m_PathElements[request];
					if (appendPath.Length != 0)
					{
						DynamicBuffer<PathElement> val3 = m_PathElements[vehicleEntity];
						PathUtils.TrimPath(val3, ref pathOwner);
						float num = garbageTruck.m_PathElementTime * (float)val3.Length + m_PathInformationData[request].m_Duration;
						if (PathUtils.TryAppendPath(ref currentLane, navigationLanes, val3, appendPath, m_SlaveLaneData, m_OwnerData, m_SubLanes, out var appendedCount))
						{
							m_ServiceDistricts.TryGetBuffer(owner.m_Owner, ref serviceDistricts);
							int dispatchIndex = BumpDispachIndex(request);
							int num2 = val3.Length - appendedCount;
							int num3 = 0;
							for (int i = 0; i < num2; i++)
							{
								PathElement pathElement = val3[i];
								if (m_PedestrianLaneData.HasComponent(pathElement.m_Target))
								{
									num3 += AddCollectionRequests(m_OwnerData[pathElement.m_Target].m_Owner, request, dispatchIndex, serviceDistricts, ref garbageTruck);
								}
							}
							if (appendedCount > 0)
							{
								val4._002Ector(appendedCount, (Allocator)2, (NativeArrayOptions)1);
								for (int j = 0; j < appendedCount; j++)
								{
									val4[j] = val3[num2 + j];
								}
								val3.RemoveRange(num2, appendedCount);
								Entity lastOwner = Entity.Null;
								for (int k = 0; k < val4.Length; k++)
								{
									num3 += AddPathElement(val3, val4[k], request, dispatchIndex, ref lastOwner, ref garbageTruck, serviceDistricts);
								}
								val4.Dispose();
							}
							if (garbageTruck.m_RequestCount == 1)
							{
								garbageTruck.m_EstimatedGarbage = num3;
							}
							car.m_Flags |= CarFlags.StayOnRoad;
							garbageTruck.m_PathElementTime = num / (float)math.max(1, val3.Length);
							target.m_Target = val;
							VehicleUtils.ClearEndOfPath(ref currentLane, navigationLanes);
							return true;
						}
					}
				}
				VehicleUtils.SetTarget(ref pathOwner, ref target, val);
				return true;
			}
			return false;
		}

		private int BumpDispachIndex(Entity request)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			int result = 0;
			GarbageCollectionRequest garbageCollectionRequest = default(GarbageCollectionRequest);
			if (m_GarbageCollectionRequestData.TryGetComponent(request, ref garbageCollectionRequest))
			{
				result = garbageCollectionRequest.m_DispatchIndex + 1;
				m_ActionQueue.Enqueue(new GarbageAction
				{
					m_Type = GarbageActionType.BumpDispatchIndex,
					m_Request = request
				});
			}
			return result;
		}

		private void ReturnToDepot(Owner ownerData, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.GarbageTruck garbageTruck, ref Car car, ref PathOwner pathOwnerData, ref Target targetData)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			serviceDispatches.Clear();
			garbageTruck.m_RequestCount = 0;
			garbageTruck.m_EstimatedGarbage = 0;
			garbageTruck.m_State |= GarbageTruckFlags.Returning;
			car.m_Flags &= ~(CarFlags.Warning | CarFlags.Working);
			VehicleUtils.SetTarget(ref pathOwnerData, ref targetData, ownerData.m_Owner);
		}

		private void ResetPath(int jobIndex, Entity vehicleEntity, PathInformation pathInformation, DynamicBuffer<ServiceDispatch> serviceDispatches, Owner owner, ref Random random, ref Game.Vehicles.GarbageTruck garbageTruck, ref Car carData, ref CarCurrentLane currentLane, ref PathOwner pathOwner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PathElement> path = m_PathElements[vehicleEntity];
			PathUtils.ResetPath(ref currentLane, path, m_SlaveLaneData, m_OwnerData, m_SubLanes);
			VehicleUtils.ResetParkingLaneStatus(vehicleEntity, ref currentLane, ref pathOwner, path, ref m_EntityLookup, ref m_CurveData, ref m_ParkingLaneData, ref m_CarLaneData, ref m_ConnectionLaneData, ref m_SpawnLocationData, ref m_PrefabRefData, ref m_PrefabSpawnLocationData);
			VehicleUtils.SetParkingCurvePos(vehicleEntity, ref random, currentLane, pathOwner, path, ref m_ParkedCarData, ref m_UnspawnedData, ref m_CurveData, ref m_ParkingLaneData, ref m_ConnectionLaneData, ref m_PrefabRefData, ref m_PrefabObjectGeometryData, ref m_PrefabParkingLaneData, ref m_LaneObjects, ref m_LaneOverlaps, ignoreDriveways: false);
			currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.Checked;
			if ((garbageTruck.m_State & GarbageTruckFlags.Returning) == 0 && garbageTruck.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				if (m_GarbageCollectionRequestData.HasComponent(request))
				{
					NativeArray<PathElement> val = default(NativeArray<PathElement>);
					val._002Ector(path.Length, (Allocator)2, (NativeArrayOptions)1);
					val.CopyFrom(path.AsNativeArray());
					path.Clear();
					DynamicBuffer<ServiceDistrict> serviceDistricts = default(DynamicBuffer<ServiceDistrict>);
					m_ServiceDistricts.TryGetBuffer(owner.m_Owner, ref serviceDistricts);
					Entity lastOwner = Entity.Null;
					int estimatedGarbage = 0;
					int dispatchIndex = BumpDispachIndex(request);
					for (int i = 0; i < val.Length; i++)
					{
						estimatedGarbage = AddPathElement(path, val[i], request, dispatchIndex, ref lastOwner, ref garbageTruck, serviceDistricts);
					}
					if (garbageTruck.m_RequestCount == 1)
					{
						garbageTruck.m_EstimatedGarbage = estimatedGarbage;
					}
					val.Dispose();
				}
				carData.m_Flags |= CarFlags.StayOnRoad;
			}
			else
			{
				carData.m_Flags &= ~CarFlags.StayOnRoad;
			}
			carData.m_Flags |= CarFlags.UsePublicTransportLanes;
			garbageTruck.m_PathElementTime = pathInformation.m_Duration / (float)math.max(1, path.Length);
		}

		private int AddPathElement(DynamicBuffer<PathElement> path, PathElement pathElement, Entity request, int dispatchIndex, ref Entity lastOwner, ref Game.Vehicles.GarbageTruck garbageTruck, DynamicBuffer<ServiceDistrict> serviceDistricts)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			int result = 0;
			if (!m_EdgeLaneData.HasComponent(pathElement.m_Target))
			{
				path.Add(pathElement);
				lastOwner = Entity.Null;
				return result;
			}
			Owner owner = m_OwnerData[pathElement.m_Target];
			if (owner.m_Owner == lastOwner)
			{
				path.Add(pathElement);
				return result;
			}
			lastOwner = owner.m_Owner;
			float curvePos = pathElement.m_TargetDelta.y;
			if (FindClosestSidewalk(pathElement.m_Target, owner.m_Owner, ref curvePos, out var sidewalk))
			{
				result = AddCollectionRequests(owner.m_Owner, request, dispatchIndex, serviceDistricts, ref garbageTruck);
				path.Add(pathElement);
				path.Add(new PathElement(sidewalk, float2.op_Implicit(curvePos)));
			}
			else
			{
				path.Add(pathElement);
			}
			return result;
		}

		private bool FindClosestSidewalk(Entity lane, Entity owner, ref float curvePos, out Entity sidewalk)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			sidewalk = Entity.Null;
			DynamicBuffer<Game.Net.SubLane> val = default(DynamicBuffer<Game.Net.SubLane>);
			if (m_SubLanes.TryGetBuffer(owner, ref val))
			{
				float3 val2 = MathUtils.Position(m_CurveData[lane].m_Bezier, curvePos);
				float num = float.MaxValue;
				float num3 = default(float);
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					if (m_PedestrianLaneData.HasComponent(subLane))
					{
						float num2 = MathUtils.Distance(MathUtils.Line(m_CurveData[subLane].m_Bezier), val2, ref num3);
						if (num2 < num)
						{
							curvePos = num3;
							sidewalk = subLane;
							num = num2;
							result = true;
						}
					}
				}
			}
			return result;
		}

		private int AddCollectionRequests(Entity edgeEntity, Entity request, int dispatchIndex, DynamicBuffer<ServiceDistrict> serviceDistricts, ref Game.Vehicles.GarbageTruck garbageTruck)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			DynamicBuffer<ConnectedBuilding> val = default(DynamicBuffer<ConnectedBuilding>);
			if (m_ConnectedBuildings.TryGetBuffer(edgeEntity, ref val))
			{
				GarbageProducer garbageProducer = default(GarbageProducer);
				for (int i = 0; i < val.Length; i++)
				{
					Entity building = val[i].m_Building;
					if (m_GarbageProducerData.TryGetComponent(building, ref garbageProducer) && ((garbageTruck.m_State & GarbageTruckFlags.IndustrialWasteOnly) == 0 || IsIndustrial(m_PrefabRefData[building].m_Prefab)) && AreaUtils.CheckServiceDistrict(building, serviceDistricts, ref m_CurrentDistrictData))
					{
						num += garbageProducer.m_Garbage;
						m_ActionQueue.Enqueue(new GarbageAction
						{
							m_Type = GarbageActionType.AddRequest,
							m_Request = request,
							m_Target = building,
							m_Capacity = dispatchIndex
						});
					}
				}
			}
			return num;
		}

		private void CheckGarbagePresence(Owner owner, ref CarCurrentLane currentLane, ref Game.Vehicles.GarbageTruck garbageTruck, ref Car car, DynamicBuffer<CarNavigationLane> navigationLanes)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			if ((garbageTruck.m_State & GarbageTruckFlags.ClearChecked) != 0)
			{
				if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Waypoint) != 0)
				{
					currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.Checked;
				}
				garbageTruck.m_State &= ~GarbageTruckFlags.ClearChecked;
			}
			if ((currentLane.m_LaneFlags & (Game.Vehicles.CarLaneFlags.Waypoint | Game.Vehicles.CarLaneFlags.Checked)) == Game.Vehicles.CarLaneFlags.Waypoint)
			{
				if (!CheckGarbagePresence(currentLane.m_Lane, owner, ref garbageTruck))
				{
					currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.Waypoint;
					car.m_Flags &= ~(CarFlags.Warning | CarFlags.Working);
					if (m_SlaveLaneData.HasComponent(currentLane.m_Lane))
					{
						currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.FixedLane;
					}
				}
				currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.Checked;
			}
			if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Waypoint) != 0)
			{
				car.m_Flags |= (CarFlags)((math.abs(currentLane.m_CurvePosition.x - currentLane.m_CurvePosition.z) < 0.5f) ? 520 : 8);
				return;
			}
			for (int i = 0; i < navigationLanes.Length; i++)
			{
				ref CarNavigationLane reference = ref navigationLanes.ElementAt(i);
				if ((reference.m_Flags & Game.Vehicles.CarLaneFlags.Waypoint) != 0 && (currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Checked) == 0)
				{
					if (!CheckGarbagePresence(reference.m_Lane, owner, ref garbageTruck))
					{
						reference.m_Flags &= ~Game.Vehicles.CarLaneFlags.Waypoint;
						car.m_Flags &= ~CarFlags.Warning;
						if (m_SlaveLaneData.HasComponent(reference.m_Lane))
						{
							reference.m_Flags &= ~Game.Vehicles.CarLaneFlags.FixedLane;
						}
					}
					currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.Checked;
					car.m_Flags &= ~CarFlags.Working;
				}
				if ((reference.m_Flags & (Game.Vehicles.CarLaneFlags.Reserved | Game.Vehicles.CarLaneFlags.Waypoint)) != Game.Vehicles.CarLaneFlags.Reserved)
				{
					car.m_Flags &= ~CarFlags.Working;
					break;
				}
			}
		}

		private bool CheckGarbagePresence(Entity laneEntity, Owner owner, ref Game.Vehicles.GarbageTruck garbageTruck)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			Owner owner2 = default(Owner);
			DynamicBuffer<ConnectedBuilding> val = default(DynamicBuffer<ConnectedBuilding>);
			if (m_EdgeLaneData.HasComponent(laneEntity) && m_OwnerData.TryGetComponent(laneEntity, ref owner2) && m_ConnectedBuildings.TryGetBuffer(owner2.m_Owner, ref val))
			{
				DynamicBuffer<ServiceDistrict> serviceDistricts = default(DynamicBuffer<ServiceDistrict>);
				m_ServiceDistricts.TryGetBuffer(owner.m_Owner, ref serviceDistricts);
				GarbageProducer garbageProducer = default(GarbageProducer);
				for (int i = 0; i < val.Length; i++)
				{
					Entity building = val[i].m_Building;
					if (m_GarbageProducerData.TryGetComponent(building, ref garbageProducer) && garbageProducer.m_Garbage > m_GarbageParameters.m_CollectionGarbageLimit && ((garbageTruck.m_State & GarbageTruckFlags.IndustrialWasteOnly) == 0 || IsIndustrial(m_PrefabRefData[building].m_Prefab)) && AreaUtils.CheckServiceDistrict(building, serviceDistricts, ref m_CurrentDistrictData))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void TryCollectGarbage(int jobIndex, Entity vehicleEntity, GarbageTruckData prefabGarbageTruckData, Owner owner, ref Game.Vehicles.GarbageTruck garbageTruck, ref Car car, ref CarCurrentLane currentLaneData, Entity ignoreBuilding)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (garbageTruck.m_Garbage < prefabGarbageTruckData.m_GarbageCapacity)
			{
				TryCollectGarbageFromLane(jobIndex, vehicleEntity, prefabGarbageTruckData, owner, ref garbageTruck, ref car, currentLaneData.m_Lane, ignoreBuilding);
			}
		}

		private void TryCollectGarbage(int jobIndex, Entity vehicleEntity, GarbageTruckData prefabGarbageTruckData, Owner owner, ref Game.Vehicles.GarbageTruck garbageTruck, ref Car car, ref Target target)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (garbageTruck.m_Garbage < prefabGarbageTruckData.m_GarbageCapacity)
			{
				DynamicBuffer<ServiceDistrict> serviceDistricts = default(DynamicBuffer<ServiceDistrict>);
				m_ServiceDistricts.TryGetBuffer(owner.m_Owner, ref serviceDistricts);
				TryCollectGarbageFromBuilding(jobIndex, vehicleEntity, prefabGarbageTruckData, ref garbageTruck, ref car, target.m_Target, serviceDistricts);
			}
		}

		private void TryCollectGarbageFromLane(int jobIndex, Entity vehicleEntity, GarbageTruckData prefabGarbageTruckData, Owner owner, ref Game.Vehicles.GarbageTruck garbageTruck, ref Car car, Entity laneEntity, Entity ignoreBuilding)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			Owner owner2 = default(Owner);
			DynamicBuffer<ConnectedBuilding> val = default(DynamicBuffer<ConnectedBuilding>);
			if (!m_EdgeLaneData.HasComponent(laneEntity) || !m_OwnerData.TryGetComponent(laneEntity, ref owner2) || !m_ConnectedBuildings.TryGetBuffer(owner2.m_Owner, ref val))
			{
				return;
			}
			bool flag = false;
			DynamicBuffer<ServiceDistrict> serviceDistricts = default(DynamicBuffer<ServiceDistrict>);
			m_ServiceDistricts.TryGetBuffer(owner.m_Owner, ref serviceDistricts);
			for (int i = 0; i < val.Length; i++)
			{
				Entity building = val[i].m_Building;
				if (building != ignoreBuilding)
				{
					flag |= TryCollectGarbageFromBuilding(jobIndex, vehicleEntity, prefabGarbageTruckData, ref garbageTruck, ref car, building, serviceDistricts);
				}
			}
			if (flag)
			{
				m_ActionQueue.Enqueue(new GarbageAction
				{
					m_Type = GarbageActionType.ClearLane,
					m_Vehicle = vehicleEntity,
					m_Target = laneEntity
				});
			}
		}

		private bool TryCollectGarbageFromBuilding(int jobIndex, Entity vehicleEntity, GarbageTruckData prefabGarbageTruckData, ref Game.Vehicles.GarbageTruck garbageTruck, ref Car car, Entity buildingEntity, DynamicBuffer<ServiceDistrict> serviceDistricts)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			GarbageProducer garbageProducer = default(GarbageProducer);
			if (m_GarbageProducerData.TryGetComponent(buildingEntity, ref garbageProducer) && garbageProducer.m_Garbage > m_GarbageParameters.m_CollectionGarbageLimit)
			{
				if ((garbageTruck.m_State & GarbageTruckFlags.IndustrialWasteOnly) != 0 && !IsIndustrial(m_PrefabRefData[buildingEntity].m_Prefab))
				{
					return false;
				}
				if (!AreaUtils.CheckServiceDistrict(buildingEntity, serviceDistricts, ref m_CurrentDistrictData))
				{
					return false;
				}
				m_ActionQueue.Enqueue(new GarbageAction
				{
					m_Type = GarbageActionType.Collect,
					m_Vehicle = vehicleEntity,
					m_Target = buildingEntity,
					m_Capacity = prefabGarbageTruckData.m_GarbageCapacity
				});
				if (garbageProducer.m_Garbage >= m_GarbageParameters.m_RequestGarbageLimit)
				{
					QuantityUpdated(jobIndex, buildingEntity);
				}
				car.m_Flags |= CarFlags.Warning | CarFlags.Working;
				return true;
			}
			return false;
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

		private bool IsIndustrial(Entity prefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			ZoneData zoneData = default(ZoneData);
			if (m_PrefabSpawnableBuildingData.TryGetComponent(prefab, ref spawnableBuildingData) && m_PrefabZoneData.TryGetComponent(spawnableBuildingData.m_ZonePrefab, ref zoneData))
			{
				return zoneData.m_AreaType == Game.Zones.AreaType.Industrial;
			}
			return false;
		}

		private bool UnloadGarbage(int jobIndex, Entity vehicleEntity, GarbageTruckData prefabGarbageTruckData, Entity facilityEntity, ref Game.Vehicles.GarbageTruck garbageTruck, bool instant)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			if (garbageTruck.m_Garbage > 0 && m_GarbageFacilityData.HasComponent(facilityEntity))
			{
				m_ActionQueue.Enqueue(new GarbageAction
				{
					m_Type = GarbageActionType.Unload,
					m_Vehicle = vehicleEntity,
					m_Target = facilityEntity,
					m_MaxAmount = math.select(Mathf.RoundToInt((float)(prefabGarbageTruckData.m_UnloadRate * 16) / 60f), garbageTruck.m_Garbage, instant)
				});
				QuantityUpdated(jobIndex, facilityEntity);
				return false;
			}
			if ((garbageTruck.m_State & GarbageTruckFlags.Unloading) != 0)
			{
				garbageTruck.m_State &= ~GarbageTruckFlags.Unloading;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(jobIndex, vehicleEntity, default(EffectsUpdated));
			}
			return true;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct GarbageActionJob : IJob
	{
		public ComponentLookup<Game.Vehicles.GarbageTruck> m_GarbageTruckData;

		public ComponentLookup<GarbageCollectionRequest> m_GarbageCollectionRequestData;

		public ComponentLookup<GarbageProducer> m_GarbageProducerData;

		public BufferLookup<Resources> m_EconomyResources;

		public BufferLookup<Efficiency> m_Efficiencies;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public GarbageParameterData m_GarbageParameters;

		public float m_GarbageEfficiencyPenalty;

		public NativeQueue<GarbageAction> m_ActionQueue;

		public IconCommandBuffer m_IconCommandBuffer;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			GarbageAction garbageAction = default(GarbageAction);
			DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
			DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
			Game.Vehicles.GarbageTruck garbageTruck = default(Game.Vehicles.GarbageTruck);
			while (m_ActionQueue.TryDequeue(ref garbageAction))
			{
				switch (garbageAction.m_Type)
				{
				case GarbageActionType.Collect:
				{
					Game.Vehicles.GarbageTruck garbageTruck3 = m_GarbageTruckData[garbageAction.m_Vehicle];
					GarbageProducer garbageProducer = m_GarbageProducerData[garbageAction.m_Target];
					int num = math.min(garbageAction.m_Capacity - garbageTruck3.m_Garbage, garbageProducer.m_Garbage);
					if (num > 0)
					{
						garbageTruck3.m_Garbage += num;
						garbageTruck3.m_EstimatedGarbage = math.max(0, garbageTruck3.m_EstimatedGarbage - num);
						garbageProducer.m_Garbage -= num;
						if ((garbageProducer.m_Flags & GarbageProducerFlags.GarbagePilingUpWarning) != GarbageProducerFlags.None && garbageProducer.m_Garbage <= m_GarbageParameters.m_WarningGarbageLimit)
						{
							m_IconCommandBuffer.Remove(garbageAction.m_Target, m_GarbageParameters.m_GarbageNotificationPrefab);
							garbageProducer.m_Flags &= ~GarbageProducerFlags.GarbagePilingUpWarning;
						}
						m_GarbageTruckData[garbageAction.m_Vehicle] = garbageTruck3;
						m_GarbageProducerData[garbageAction.m_Target] = garbageProducer;
						if (m_Efficiencies.TryGetBuffer(garbageAction.m_Target, ref buffer))
						{
							float garbageEfficiencyFactor = GarbageAccumulationSystem.GetGarbageEfficiencyFactor(garbageProducer.m_Garbage, m_GarbageParameters, m_GarbageEfficiencyPenalty);
							BuildingUtils.SetEfficiencyFactor(buffer, EfficiencyFactor.Garbage, garbageEfficiencyFactor);
						}
					}
					break;
				}
				case GarbageActionType.Unload:
				{
					Game.Vehicles.GarbageTruck garbageTruck2 = m_GarbageTruckData[garbageAction.m_Vehicle];
					int garbage = garbageTruck2.m_Garbage;
					garbage = math.min(garbage, garbageAction.m_MaxAmount);
					if (garbage > 0)
					{
						garbageTruck2.m_Garbage -= garbage;
						if (m_EconomyResources.HasBuffer(garbageAction.m_Target))
						{
							DynamicBuffer<Resources> resources = m_EconomyResources[garbageAction.m_Target];
							EconomyUtils.AddResources(Resource.Garbage, garbage, resources);
						}
						if ((garbageTruck2.m_State & GarbageTruckFlags.Unloading) == 0)
						{
							garbageTruck2.m_State |= GarbageTruckFlags.Unloading;
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(garbageAction.m_Vehicle, default(EffectsUpdated));
						}
						m_GarbageTruckData[garbageAction.m_Vehicle] = garbageTruck2;
					}
					else if ((garbageTruck2.m_State & GarbageTruckFlags.Unloading) != 0)
					{
						garbageTruck2.m_State &= ~GarbageTruckFlags.Unloading;
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(garbageAction.m_Vehicle, default(EffectsUpdated));
					}
					break;
				}
				case GarbageActionType.AddRequest:
				{
					GarbageProducer garbageProducer2 = m_GarbageProducerData[garbageAction.m_Target];
					garbageProducer2.m_CollectionRequest = garbageAction.m_Request;
					garbageProducer2.m_DispatchIndex = (byte)garbageAction.m_Capacity;
					m_GarbageProducerData[garbageAction.m_Target] = garbageProducer2;
					break;
				}
				case GarbageActionType.ClearLane:
				{
					if (!m_LaneObjects.TryGetBuffer(garbageAction.m_Target, ref val))
					{
						break;
					}
					for (int i = 0; i < val.Length; i++)
					{
						Entity laneObject = val[i].m_LaneObject;
						if (laneObject != garbageAction.m_Vehicle && m_GarbageTruckData.TryGetComponent(laneObject, ref garbageTruck))
						{
							garbageTruck.m_State |= GarbageTruckFlags.ClearChecked;
							m_GarbageTruckData[laneObject] = garbageTruck;
						}
					}
					break;
				}
				case GarbageActionType.BumpDispatchIndex:
				{
					GarbageCollectionRequest garbageCollectionRequest = m_GarbageCollectionRequestData[garbageAction.m_Request];
					garbageCollectionRequest.m_DispatchIndex++;
					m_GarbageCollectionRequestData[garbageAction.m_Request] = garbageCollectionRequest;
					break;
				}
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> __Game_Objects_Unspawned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Vehicles.GarbageTruck> __Game_Vehicles_GarbageTruck_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Car> __Game_Vehicles_Car_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public BufferTypeHandle<CarNavigationLane> __Game_Vehicles_CarNavigationLane_RW_BufferTypeHandle;

		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Quantity> __Game_Objects_Quantity_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarageLane> __Game_Net_GarageLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageTruckData> __Game_Prefabs_GarbageTruckData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageCollectionRequest> __Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> __Game_Buildings_GarbageProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.GarbageFacility> __Game_Buildings_GarbageFacility_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedBuilding> __Game_Buildings_ConnectedBuilding_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> __Game_Areas_ServiceDistrict_RO_BufferLookup;

		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RW_BufferLookup;

		public ComponentLookup<Game.Vehicles.GarbageTruck> __Game_Vehicles_GarbageTruck_RW_ComponentLookup;

		public ComponentLookup<GarbageCollectionRequest> __Game_Simulation_GarbageCollectionRequest_RW_ComponentLookup;

		public ComponentLookup<GarbageProducer> __Game_Buildings_GarbageProducer_RW_ComponentLookup;

		public BufferLookup<Resources> __Game_Economy_Resources_RW_BufferLookup;

		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RW_BufferLookup;

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
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Vehicles_GarbageTruck_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.GarbageTruck>(false);
			__Game_Vehicles_Car_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Car>(false);
			__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarCurrentLane>(false);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Vehicles_CarNavigationLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CarNavigationLane>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Objects_Quantity_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Quantity>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.PedestrianLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_GarageLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarageLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_GarbageTruckData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageTruckData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageCollectionRequest>(true);
			__Game_Buildings_GarbageProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageProducer>(true);
			__Game_Buildings_GarbageFacility_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.GarbageFacility>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentDistrict>(true);
			__Game_Buildings_ConnectedBuilding_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedBuilding>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Areas_ServiceDistrict_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDistrict>(true);
			__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
			__Game_Vehicles_GarbageTruck_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.GarbageTruck>(false);
			__Game_Simulation_GarbageCollectionRequest_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageCollectionRequest>(false);
			__Game_Buildings_GarbageProducer_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageProducer>(false);
			__Game_Economy_Resources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(false);
			__Game_Buildings_Efficiency_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private IconCommandSystem m_IconCommandSystem;

	private ServiceFeeSystem m_ServiceFeeSystem;

	private SimulationSystem m_SimulationSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private EntityQuery m_VehicleQuery;

	private EntityArchetype m_GarbageCollectionRequestArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_MovingToParkedCarRemoveTypes;

	private ComponentTypeSet m_MovingToParkedAddTypes;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_647374864_0;

	private EntityQuery __query_647374864_1;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 2;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_ServiceFeeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ServiceFeeSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[10]
		{
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Game.Vehicles.GarbageTruck>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>(),
			ComponentType.Exclude<OutOfControl>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
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
		m_MovingToParkedCarRemoveTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[13]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<Swaying>(),
			ComponentType.ReadWrite<CarNavigation>(),
			ComponentType.ReadWrite<CarNavigationLane>(),
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<PathElement>(),
			ComponentType.ReadWrite<PathInformation>(),
			ComponentType.ReadWrite<ServiceDispatch>()
		});
		m_MovingToParkedAddTypes = new ComponentTypeSet(ComponentType.ReadWrite<ParkedCar>(), ComponentType.ReadWrite<Stopped>(), ComponentType.ReadWrite<Updated>());
		((ComponentSystemBase)this).RequireForUpdate(m_VehicleQuery);
		((ComponentSystemBase)this).RequireForUpdate<GarbageParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<ServiceFeeParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<BuildingEfficiencyParameterData>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		GarbageParameterData singleton = ((EntityQuery)(ref __query_647374864_0)).GetSingleton<GarbageParameterData>();
		NativeQueue<GarbageAction> actionQueue = default(NativeQueue<GarbageAction>);
		actionQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		GarbageTruckTickJob garbageTruckTickJob = new GarbageTruckTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageTruckType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.GarbageTruck>(ref __TypeHandle.__Game_Vehicles_GarbageTruck_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarType = InternalCompilerInterface.GetComponentTypeHandle<Car>(ref __TypeHandle.__Game_Vehicles_Car_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<CarNavigationLane>(ref __TypeHandle.__Game_Vehicles_CarNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatchType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_QuantityData = InternalCompilerInterface.GetComponentLookup<Quantity>(ref __TypeHandle.__Game_Objects_Quantity_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarageLaneData = InternalCompilerInterface.GetComponentLookup<GarageLane>(ref __TypeHandle.__Game_Net_GarageLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGarbageTruckData = InternalCompilerInterface.GetComponentLookup<GarbageTruckData>(ref __TypeHandle.__Game_Prefabs_GarbageTruckData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnableBuildingData = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabZoneData = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageCollectionRequestData = InternalCompilerInterface.GetComponentLookup<GarbageCollectionRequest>(ref __TypeHandle.__Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageProducerData = InternalCompilerInterface.GetComponentLookup<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageFacilityData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.GarbageFacility>(ref __TypeHandle.__Game_Buildings_GarbageFacility_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentDistrictData = InternalCompilerInterface.GetComponentLookup<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedBuildings = InternalCompilerInterface.GetBufferLookup<ConnectedBuilding>(ref __TypeHandle.__Game_Buildings_ConnectedBuilding_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDistricts = InternalCompilerInterface.GetBufferLookup<ServiceDistrict>(ref __TypeHandle.__Game_Areas_ServiceDistrict_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex,
			m_RandomSeed = RandomSeed.Next(),
			m_GarbageCollectionRequestArchetype = m_GarbageCollectionRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_MovingToParkedCarRemoveTypes = m_MovingToParkedCarRemoveTypes,
			m_MovingToParkedAddTypes = m_MovingToParkedAddTypes,
			m_GarbageParameters = singleton
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		garbageTruckTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		garbageTruckTickJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		garbageTruckTickJob.m_ActionQueue = actionQueue.AsParallelWriter();
		GarbageTruckTickJob garbageTruckTickJob2 = garbageTruckTickJob;
		GarbageActionJob obj = new GarbageActionJob
		{
			m_GarbageTruckData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.GarbageTruck>(ref __TypeHandle.__Game_Vehicles_GarbageTruck_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageCollectionRequestData = InternalCompilerInterface.GetComponentLookup<GarbageCollectionRequest>(ref __TypeHandle.__Game_Simulation_GarbageCollectionRequest_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageProducerData = InternalCompilerInterface.GetComponentLookup<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EconomyResources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Efficiencies = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageParameters = singleton,
			m_GarbageEfficiencyPenalty = ((EntityQuery)(ref __query_647374864_1)).GetSingleton<BuildingEfficiencyParameterData>().m_GarbagePenalty,
			m_ActionQueue = actionQueue,
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer(),
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<GarbageTruckTickJob>(garbageTruckTickJob2, m_VehicleQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = IJobExtensions.Schedule<GarbageActionJob>(obj, val2);
		actionQueue.Dispose(val3);
		m_PathfindSetupSystem.AddQueueWriter(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		m_IconCommandSystem.AddCommandBufferWriter(val3);
		m_ServiceFeeSystem.AddQueueWriter(val3);
		m_CityStatisticsSystem.AddWriter(val3);
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<GarbageParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_647374864_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<BuildingEfficiencyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_647374864_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public GarbageTruckAISystem()
	{
	}
}

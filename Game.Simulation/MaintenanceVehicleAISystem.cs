using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Buildings;
using Game.City;
using Game.Common;
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
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class MaintenanceVehicleAISystem : GameSystemBase
{
	private struct MaintenanceAction
	{
		public MaintenanceActionType m_Type;

		public Entity m_Vehicle;

		public Entity m_Consumer;

		public Entity m_Request;

		public int m_VehicleCapacity;

		public int m_ConsumerCapacity;

		public int m_MaxMaintenanceAmount;

		public CarFlags m_WorkingFlags;
	}

	private enum MaintenanceActionType
	{
		AddRequest,
		ParkMaintenance,
		RoadMaintenance,
		RepairVehicle,
		ClearLane,
		BumpDispatchIndex
	}

	[BurstCompile]
	private struct MaintenanceVehicleTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public ComponentTypeHandle<Stopped> m_StoppedType;

		public ComponentTypeHandle<Game.Vehicles.MaintenanceVehicle> m_MaintenanceVehicleType;

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
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<Damaged> m_DamagedData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<NetCondition> m_NetConditionData;

		[ReadOnly]
		public ComponentLookup<LaneCondition> m_LaneConditionData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<GarageLane> m_GarageLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> m_InvolvedInAccidentData;

		[ReadOnly]
		public ComponentLookup<AccidentSite> m_AccidentSiteData;

		[ReadOnly]
		public ComponentLookup<OnFire> m_OnFireData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> m_ParkData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.MaintenanceDepot> m_MaintenanceDepotData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<MaintenanceVehicleData> m_PrefabMaintenanceVehicleData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public ComponentLookup<ParkData> m_PrefabParkData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> m_MaintenanceRequestData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<LaneOverlap> m_LaneOverlaps;

		[ReadOnly]
		public BufferLookup<TargetElement> m_TargetElements;

		[NativeDisableParallelForRestriction]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_MaintenanceRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedCarRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedAddTypes;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public ParallelWriter<MaintenanceAction> m_ActionQueue;

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
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<PathInformation> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
			NativeArray<CarCurrentLane> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CurrentLaneType);
			NativeArray<Game.Vehicles.MaintenanceVehicle> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.MaintenanceVehicle>(ref m_MaintenanceVehicleType);
			NativeArray<Car> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Car>(ref m_CarType);
			NativeArray<Target> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PathOwner> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			BufferAccessor<CarNavigationLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<CarNavigationLane>(ref m_CarNavigationLaneType);
			BufferAccessor<ServiceDispatch> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			bool isStopped = ((ArchetypeChunk)(ref chunk)).Has<Stopped>(ref m_StoppedType);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Owner owner = nativeArray2[i];
				Transform transform = nativeArray3[i];
				PrefabRef prefabRef = nativeArray4[i];
				PathInformation pathInformation = nativeArray5[i];
				Game.Vehicles.MaintenanceVehicle maintenanceVehicle = nativeArray7[i];
				Car car = nativeArray8[i];
				CarCurrentLane currentLane = nativeArray6[i];
				PathOwner pathOwner = nativeArray10[i];
				Target target = nativeArray9[i];
				DynamicBuffer<CarNavigationLane> navigationLanes = bufferAccessor[i];
				DynamicBuffer<ServiceDispatch> serviceDispatches = bufferAccessor2[i];
				VehicleUtils.CheckUnspawned(unfilteredChunkIndex, val, currentLane, isUnspawned, m_CommandBuffer);
				Tick(unfilteredChunkIndex, val, owner, transform, prefabRef, pathInformation, navigationLanes, serviceDispatches, isStopped, ref random, ref maintenanceVehicle, ref car, ref currentLane, ref pathOwner, ref target);
				nativeArray7[i] = maintenanceVehicle;
				nativeArray8[i] = car;
				nativeArray6[i] = currentLane;
				nativeArray10[i] = pathOwner;
				nativeArray9[i] = target;
			}
		}

		private void Tick(int jobIndex, Entity vehicleEntity, Owner owner, Transform transform, PrefabRef prefabRef, PathInformation pathInformation, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, bool isStopped, ref Random random, ref Game.Vehicles.MaintenanceVehicle maintenanceVehicle, ref Car car, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			MaintenanceVehicleData prefabMaintenanceVehicleData = m_PrefabMaintenanceVehicleData[prefabRef.m_Prefab];
			prefabMaintenanceVehicleData.m_MaintenanceCapacity = Mathf.CeilToInt((float)prefabMaintenanceVehicleData.m_MaintenanceCapacity * maintenanceVehicle.m_Efficiency);
			if (VehicleUtils.ResetUpdatedPath(ref pathOwner))
			{
				ResetPath(jobIndex, vehicleEntity, pathInformation, serviceDispatches, prefabMaintenanceVehicleData, ref random, ref maintenanceVehicle, ref car, ref currentLane, ref pathOwner);
			}
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(target.m_Target) || VehicleUtils.PathfindFailed(pathOwner))
			{
				if (VehicleUtils.IsStuck(pathOwner) || (maintenanceVehicle.m_State & MaintenanceVehicleFlags.Returning) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
					return;
				}
				ReturnToDepot(jobIndex, vehicleEntity, owner, serviceDispatches, ref car, ref maintenanceVehicle, ref pathOwner, ref target);
			}
			else if (VehicleUtils.PathEndReached(currentLane))
			{
				if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.Returning) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
					return;
				}
				if ((maintenanceVehicle.m_State & (MaintenanceVehicleFlags.TryWork | MaintenanceVehicleFlags.Working)) != MaintenanceVehicleFlags.TryWork)
				{
					if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.EdgeTarget) != 0)
					{
						TryMaintain(vehicleEntity, prefabMaintenanceVehicleData, ref car, ref maintenanceVehicle, ref currentLane);
					}
					else if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.TransformTarget) != 0)
					{
						if (IsSecureSite(ref target))
						{
							TryMaintain(vehicleEntity, prefabMaintenanceVehicleData, ref car, ref maintenanceVehicle, ref currentLane, ref target);
						}
						else if (!isStopped)
						{
							StopVehicle(jobIndex, vehicleEntity, ref currentLane);
						}
					}
					else
					{
						maintenanceVehicle.m_State &= ~MaintenanceVehicleFlags.Working;
					}
					return;
				}
				CheckServiceDispatches(vehicleEntity, serviceDispatches, prefabMaintenanceVehicleData, ref maintenanceVehicle, ref pathOwner);
				if (!SelectNextDispatch(jobIndex, vehicleEntity, navigationLanes, serviceDispatches, prefabMaintenanceVehicleData, ref maintenanceVehicle, ref car, ref currentLane, ref pathOwner, ref target))
				{
					ReturnToDepot(jobIndex, vehicleEntity, owner, serviceDispatches, ref car, ref maintenanceVehicle, ref pathOwner, ref target);
				}
			}
			else
			{
				if (VehicleUtils.ParkingSpaceReached(currentLane, pathOwner))
				{
					if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.Returning) != 0)
					{
						ParkCar(jobIndex, vehicleEntity, owner, ref maintenanceVehicle, ref car, ref currentLane);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
					}
					return;
				}
				if (VehicleUtils.WaypointReached(currentLane))
				{
					if ((maintenanceVehicle.m_State & (MaintenanceVehicleFlags.TryWork | MaintenanceVehicleFlags.Working | MaintenanceVehicleFlags.ClearingDebris)) != MaintenanceVehicleFlags.TryWork && (maintenanceVehicle.m_State & MaintenanceVehicleFlags.EdgeTarget) != 0)
					{
						TryMaintain(vehicleEntity, prefabMaintenanceVehicleData, ref car, ref maintenanceVehicle, ref currentLane);
					}
					else
					{
						currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.Waypoint;
						maintenanceVehicle.m_State &= ~(MaintenanceVehicleFlags.TryWork | MaintenanceVehicleFlags.Working | MaintenanceVehicleFlags.ClearingDebris);
					}
				}
				else if ((maintenanceVehicle.m_State & (MaintenanceVehicleFlags.TryWork | MaintenanceVehicleFlags.Working)) != 0)
				{
					maintenanceVehicle.m_State &= ~(MaintenanceVehicleFlags.TryWork | MaintenanceVehicleFlags.Working | MaintenanceVehicleFlags.ClearingDebris);
				}
				else if (isStopped)
				{
					StartVehicle(jobIndex, vehicleEntity, ref currentLane);
				}
				else if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.TransformTarget) != 0 && (currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.IsBlocked) != 0 && IsCloseEnough(transform, ref target))
				{
					EndNavigation(vehicleEntity, ref currentLane, ref pathOwner, navigationLanes);
				}
			}
			if (maintenanceVehicle.m_Maintained >= prefabMaintenanceVehicleData.m_MaintenanceCapacity)
			{
				maintenanceVehicle.m_State |= MaintenanceVehicleFlags.Full | MaintenanceVehicleFlags.EstimatedFull;
			}
			else if (maintenanceVehicle.m_Maintained + maintenanceVehicle.m_MaintainEstimate >= prefabMaintenanceVehicleData.m_MaintenanceCapacity)
			{
				maintenanceVehicle.m_State |= MaintenanceVehicleFlags.EstimatedFull;
				maintenanceVehicle.m_State &= ~MaintenanceVehicleFlags.Full;
			}
			else
			{
				maintenanceVehicle.m_State &= ~(MaintenanceVehicleFlags.Full | MaintenanceVehicleFlags.EstimatedFull);
			}
			if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.Returning) == 0)
			{
				if ((maintenanceVehicle.m_State & (MaintenanceVehicleFlags.Full | MaintenanceVehicleFlags.Disabled)) != 0)
				{
					if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.TryWork) == 0)
					{
						ReturnToDepot(jobIndex, vehicleEntity, owner, serviceDispatches, ref car, ref maintenanceVehicle, ref pathOwner, ref target);
					}
				}
				else
				{
					CheckMaintenancePresence(ref car, ref maintenanceVehicle, ref currentLane, navigationLanes);
				}
			}
			if ((maintenanceVehicle.m_State & (MaintenanceVehicleFlags.Full | MaintenanceVehicleFlags.Disabled)) == 0)
			{
				CheckServiceDispatches(vehicleEntity, serviceDispatches, prefabMaintenanceVehicleData, ref maintenanceVehicle, ref pathOwner);
				if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.Returning) != 0)
				{
					SelectNextDispatch(jobIndex, vehicleEntity, navigationLanes, serviceDispatches, prefabMaintenanceVehicleData, ref maintenanceVehicle, ref car, ref currentLane, ref pathOwner, ref target);
				}
				if (maintenanceVehicle.m_RequestCount <= 1 && (maintenanceVehicle.m_State & MaintenanceVehicleFlags.EstimatedFull) == 0)
				{
					RequestTargetIfNeeded(jobIndex, vehicleEntity, ref maintenanceVehicle);
				}
			}
			else if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.TryWork) == 0)
			{
				serviceDispatches.Clear();
			}
			if ((maintenanceVehicle.m_State & (MaintenanceVehicleFlags.TryWork | MaintenanceVehicleFlags.Working)) != 0)
			{
				return;
			}
			if (VehicleUtils.RequireNewPath(pathOwner))
			{
				if (isStopped && (currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.ParkingSpace) == 0)
				{
					currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.EndOfPath;
				}
				else
				{
					FindNewPath(vehicleEntity, prefabRef, prefabMaintenanceVehicleData, ref maintenanceVehicle, ref currentLane, ref pathOwner, ref target);
				}
			}
			else if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Stuck)) == 0)
			{
				CheckParkingSpace(vehicleEntity, ref random, ref currentLane, ref pathOwner, navigationLanes);
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

		private void ParkCar(int jobIndex, Entity entity, Owner owner, ref Game.Vehicles.MaintenanceVehicle maintenanceVehicle, ref Car car, ref CarCurrentLane currentLane)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			car.m_Flags &= ~(CarFlags.Warning | CarFlags.Working | CarFlags.SignalAnimation1 | CarFlags.SignalAnimation2);
			maintenanceVehicle.m_State = (MaintenanceVehicleFlags)0u;
			maintenanceVehicle.m_Maintained = 0;
			Game.Buildings.MaintenanceDepot maintenanceDepot = default(Game.Buildings.MaintenanceDepot);
			if (m_MaintenanceDepotData.TryGetComponent(owner.m_Owner, ref maintenanceDepot) && (maintenanceDepot.m_Flags & MaintenanceDepotFlags.HasAvailableVehicles) == 0)
			{
				maintenanceVehicle.m_State |= MaintenanceVehicleFlags.Disabled;
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

		private bool IsSecureSite(ref Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			if (m_OnFireData.HasComponent(target.m_Target))
			{
				return false;
			}
			if (m_InvolvedInAccidentData.HasComponent(target.m_Target))
			{
				InvolvedInAccident involvedInAccident = m_InvolvedInAccidentData[target.m_Target];
				if (m_TargetElements.HasBuffer(involvedInAccident.m_Event))
				{
					DynamicBuffer<TargetElement> val = m_TargetElements[involvedInAccident.m_Event];
					for (int i = 0; i < val.Length; i++)
					{
						Entity entity = val[i].m_Entity;
						if (m_AccidentSiteData.HasComponent(entity))
						{
							return (m_AccidentSiteData[entity].m_Flags & AccidentSiteFlags.Secured) != 0;
						}
					}
				}
			}
			return true;
		}

		private void EndNavigation(Entity vehicleEntity, ref CarCurrentLane currentLane, ref PathOwner pathOwner, DynamicBuffer<CarNavigationLane> navigationLanes)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			currentLane.m_CurvePosition.z = currentLane.m_CurvePosition.y;
			currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.EndOfPath;
			navigationLanes.Clear();
			pathOwner.m_ElementIndex = 0;
			m_PathElements[vehicleEntity].Clear();
		}

		private bool IsCloseEnough(Transform transform, ref Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (m_TransformData.HasComponent(target.m_Target))
			{
				Transform transform2 = m_TransformData[target.m_Target];
				return math.distance(transform.m_Position, transform2.m_Position) <= 30f;
			}
			return false;
		}

		private void StopVehicle(int jobIndex, Entity entity, ref CarCurrentLane currentLaneData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Moving>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TransformFrame>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<InterpolatedTransform>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Swaying>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Stopped>(jobIndex, entity, default(Stopped));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, entity, default(Updated));
			if (m_CarLaneData.HasComponent(currentLaneData.m_Lane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, currentLaneData.m_Lane, default(PathfindUpdated));
			}
			if (m_CarLaneData.HasComponent(currentLaneData.m_ChangeLane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, currentLaneData.m_ChangeLane, default(PathfindUpdated));
			}
		}

		private void StartVehicle(int jobIndex, Entity entity, ref CarCurrentLane currentLaneData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Stopped>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Moving>(jobIndex, entity, default(Moving));
			((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<TransformFrame>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(jobIndex, entity, default(InterpolatedTransform));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Swaying>(jobIndex, entity, default(Swaying));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, entity, default(Updated));
			if (m_CarLaneData.HasComponent(currentLaneData.m_Lane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, currentLaneData.m_Lane, default(PathfindUpdated));
			}
			if (m_CarLaneData.HasComponent(currentLaneData.m_ChangeLane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, currentLaneData.m_ChangeLane, default(PathfindUpdated));
			}
		}

		private void FindNewPath(Entity vehicleEntity, PrefabRef prefabRef, MaintenanceVehicleData prefabMaintenanceVehicleData, ref Game.Vehicles.MaintenanceVehicle maintenanceVehicle, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
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
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
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
				m_IgnoredRules = VehicleUtils.GetIgnoredPathfindRules(carData)
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
			if ((prefabMaintenanceVehicleData.m_MaintenanceType & (MaintenanceType.Road | MaintenanceType.Snow | MaintenanceType.Vehicle)) != MaintenanceType.None)
			{
				parameters.m_IgnoredRules |= RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic;
			}
			if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.Returning) == 0)
			{
				if (m_TransformData.HasComponent(target.m_Target))
				{
					destination.m_Value2 = 30f;
				}
			}
			else
			{
				destination.m_Methods |= PathMethod.SpecialParking;
				destination.m_RandomCost = 30f;
			}
			VehicleUtils.SetupPathfind(item: new SetupQueueItem(vehicleEntity, parameters, origin, destination), currentLane: ref currentLane, pathOwner: ref pathOwner, queue: m_PathfindQueue);
		}

		private void CheckServiceDispatches(Entity vehicleEntity, DynamicBuffer<ServiceDispatch> serviceDispatches, MaintenanceVehicleData prefabMaintenanceVehicleData, ref Game.Vehicles.MaintenanceVehicle maintenanceVehicle, ref PathOwner pathOwner)
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
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			if (serviceDispatches.Length <= maintenanceVehicle.m_RequestCount)
			{
				return;
			}
			int num = -1;
			Entity val = Entity.Null;
			PathElement pathElement = default(PathElement);
			bool flag = false;
			int num2 = 0;
			if (maintenanceVehicle.m_RequestCount >= 1 && (maintenanceVehicle.m_State & MaintenanceVehicleFlags.Returning) == 0)
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
			for (int i = num2; i < maintenanceVehicle.m_RequestCount; i++)
			{
				Entity request = serviceDispatches[i].m_Request;
				if (m_PathElements.TryGetBuffer(request, ref val3) && val3.Length != 0)
				{
					pathElement = val3[val3.Length - 1];
					flag = true;
				}
			}
			DynamicBuffer<PathElement> val4 = default(DynamicBuffer<PathElement>);
			for (int j = maintenanceVehicle.m_RequestCount; j < serviceDispatches.Length; j++)
			{
				Entity request2 = serviceDispatches[j].m_Request;
				if (!m_MaintenanceRequestData.HasComponent(request2))
				{
					continue;
				}
				MaintenanceRequest maintenanceRequest = m_MaintenanceRequestData[request2];
				if (flag && m_PathElements.TryGetBuffer(request2, ref val4) && val4.Length != 0)
				{
					PathElement pathElement2 = val4[0];
					if (pathElement2.m_Target != pathElement.m_Target || pathElement2.m_TargetDelta.x != pathElement.m_TargetDelta.y)
					{
						continue;
					}
				}
				if (m_PrefabRefData.HasComponent(maintenanceRequest.m_Target) && maintenanceRequest.m_Priority > num)
				{
					num = maintenanceRequest.m_Priority;
					val = request2;
				}
			}
			if (val != Entity.Null)
			{
				serviceDispatches[maintenanceVehicle.m_RequestCount++] = new ServiceDispatch(val);
				MaintenanceRequest maintenanceRequest2 = m_MaintenanceRequestData[val];
				if ((prefabMaintenanceVehicleData.m_MaintenanceType & (MaintenanceType.Road | MaintenanceType.Snow)) != MaintenanceType.None && m_NetConditionData.HasComponent(maintenanceRequest2.m_Target))
				{
					maintenanceVehicle.m_MaintainEstimate += PreAddMaintenanceRequests(val);
				}
				if ((prefabMaintenanceVehicleData.m_MaintenanceType & MaintenanceType.Vehicle) != MaintenanceType.None)
				{
					Destroyed destroyed = default(Destroyed);
					Damaged damaged = default(Damaged);
					if (m_DestroyedData.TryGetComponent(maintenanceRequest2.m_Target, ref destroyed))
					{
						float num3 = 500f * (1f - destroyed.m_Cleared);
						maintenanceVehicle.m_MaintainEstimate += Mathf.RoundToInt(num3);
					}
					else if (m_DamagedData.TryGetComponent(maintenanceRequest2.m_Target, ref damaged))
					{
						float num4 = math.min(500f, math.csum(damaged.m_Damage) * 500f);
						maintenanceVehicle.m_MaintainEstimate += Mathf.RoundToInt(num4);
					}
				}
				Game.Buildings.Park park = default(Game.Buildings.Park);
				if ((prefabMaintenanceVehicleData.m_MaintenanceType & MaintenanceType.Park) != MaintenanceType.None && m_ParkData.TryGetComponent(maintenanceRequest2.m_Target, ref park))
				{
					PrefabRef prefabRef = m_PrefabRefData[maintenanceRequest2.m_Target];
					ParkData parkData = default(ParkData);
					if (m_PrefabParkData.TryGetComponent(prefabRef.m_Prefab, ref parkData))
					{
						maintenanceVehicle.m_MaintainEstimate += math.max(0, parkData.m_MaintenancePool - park.m_Maintenance);
					}
				}
			}
			if (serviceDispatches.Length > maintenanceVehicle.m_RequestCount)
			{
				serviceDispatches.RemoveRange(maintenanceVehicle.m_RequestCount, serviceDispatches.Length - maintenanceVehicle.m_RequestCount);
			}
		}

		private int BumpDispachIndex(Entity request)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			int result = 0;
			MaintenanceRequest maintenanceRequest = default(MaintenanceRequest);
			if (m_MaintenanceRequestData.TryGetComponent(request, ref maintenanceRequest))
			{
				result = maintenanceRequest.m_DispatchIndex + 1;
				m_ActionQueue.Enqueue(new MaintenanceAction
				{
					m_Type = MaintenanceActionType.BumpDispatchIndex,
					m_Request = request
				});
			}
			return result;
		}

		private int PreAddMaintenanceRequests(Entity request)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			DynamicBuffer<PathElement> val = default(DynamicBuffer<PathElement>);
			if (m_PathElements.TryGetBuffer(request, ref val))
			{
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
					Owner owner = m_OwnerData[pathElement.m_Target];
					if (!(owner.m_Owner == val2))
					{
						val2 = owner.m_Owner;
						if (HasSidewalk(owner.m_Owner))
						{
							num += AddMaintenanceRequests(owner.m_Owner, request, dispatchIndex, collectMaintenance: true);
						}
					}
				}
			}
			return num;
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

		private void RequestTargetIfNeeded(int jobIndex, Entity entity, ref Game.Vehicles.MaintenanceVehicle maintenanceVehicle)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			if (!m_MaintenanceRequestData.HasComponent(maintenanceVehicle.m_TargetRequest))
			{
				uint num = math.max(512u, 16u);
				if ((m_SimulationFrameIndex & (num - 1)) == 7)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_MaintenanceRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MaintenanceRequest>(jobIndex, val, new MaintenanceRequest(entity, 1));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(32u));
				}
			}
		}

		private bool SelectNextDispatch(int jobIndex, Entity vehicleEntity, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, MaintenanceVehicleData prefabMaintenanceVehicleData, ref Game.Vehicles.MaintenanceVehicle maintenanceVehicle, ref Car car, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.Returning) == 0 && maintenanceVehicle.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				serviceDispatches.RemoveAt(0);
				maintenanceVehicle.m_RequestCount--;
			}
			NativeArray<PathElement> val4 = default(NativeArray<PathElement>);
			Destroyed destroyed = default(Destroyed);
			Damaged damaged = default(Damaged);
			Game.Buildings.Park park = default(Game.Buildings.Park);
			ParkData parkData = default(ParkData);
			while (maintenanceVehicle.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				Entity val = Entity.Null;
				if (m_MaintenanceRequestData.HasComponent(request))
				{
					val = m_MaintenanceRequestData[request].m_Target;
				}
				if (!m_PrefabRefData.HasComponent(val))
				{
					serviceDispatches.RemoveAt(0);
					maintenanceVehicle.m_MaintainEstimate -= maintenanceVehicle.m_MaintainEstimate / maintenanceVehicle.m_RequestCount;
					maintenanceVehicle.m_RequestCount--;
					continue;
				}
				MaintenanceVehicleFlags maintenanceVehicleFlags = (MaintenanceVehicleFlags)0u;
				if (m_NetConditionData.HasComponent(val))
				{
					maintenanceVehicleFlags |= MaintenanceVehicleFlags.EdgeTarget;
				}
				else if (m_TransformData.HasComponent(val))
				{
					maintenanceVehicleFlags |= MaintenanceVehicleFlags.TransformTarget;
				}
				if ((maintenanceVehicle.m_State & maintenanceVehicleFlags & MaintenanceVehicleFlags.EdgeTarget) == 0)
				{
					car.m_Flags &= ~(CarFlags.Warning | CarFlags.Working | CarFlags.SignalAnimation1 | CarFlags.SignalAnimation2);
				}
				maintenanceVehicle.m_State &= ~(MaintenanceVehicleFlags.Returning | MaintenanceVehicleFlags.TransformTarget | MaintenanceVehicleFlags.EdgeTarget | MaintenanceVehicleFlags.TryWork | MaintenanceVehicleFlags.Working | MaintenanceVehicleFlags.ClearingDebris);
				maintenanceVehicle.m_State |= maintenanceVehicleFlags;
				Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(request, vehicleEntity, completed: false, pathConsumed: true));
				if (m_MaintenanceRequestData.HasComponent(maintenanceVehicle.m_TargetRequest))
				{
					val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(maintenanceVehicle.m_TargetRequest, Entity.Null, completed: true));
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(jobIndex, vehicleEntity, default(EffectsUpdated));
				if (m_PathElements.HasBuffer(request))
				{
					DynamicBuffer<PathElement> appendPath = m_PathElements[request];
					if (appendPath.Length != 0)
					{
						DynamicBuffer<PathElement> val3 = m_PathElements[vehicleEntity];
						PathUtils.TrimPath(val3, ref pathOwner);
						float num = maintenanceVehicle.m_PathElementTime * (float)val3.Length + m_PathInformationData[request].m_Duration;
						if (PathUtils.TryAppendPath(ref currentLane, navigationLanes, val3, appendPath, m_SlaveLaneData, m_OwnerData, m_SubLanes, out var appendedCount))
						{
							int num2 = 0;
							bool flag = maintenanceVehicle.m_RequestCount == 1;
							if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.EdgeTarget) != 0)
							{
								int dispatchIndex = BumpDispachIndex(request);
								int num3 = val3.Length - appendedCount;
								for (int i = 0; i < num3; i++)
								{
									PathElement pathElement = val3[i];
									if (m_PedestrianLaneData.HasComponent(pathElement.m_Target))
									{
										num2 += AddMaintenanceRequests(m_OwnerData[pathElement.m_Target].m_Owner, request, dispatchIndex, flag);
									}
								}
								if (appendedCount > 0)
								{
									val4._002Ector(appendedCount, (Allocator)2, (NativeArrayOptions)1);
									for (int j = 0; j < appendedCount; j++)
									{
										val4[j] = val3[num3 + j];
									}
									val3.RemoveRange(num3, appendedCount);
									Entity lastOwner = Entity.Null;
									for (int k = 0; k < val4.Length; k++)
									{
										num2 += AddPathElement(val3, val4[k], request, dispatchIndex, ref lastOwner, flag);
									}
									val4.Dispose();
								}
								car.m_Flags |= CarFlags.StayOnRoad;
							}
							else if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.TransformTarget) != 0)
							{
								if (flag)
								{
									if ((prefabMaintenanceVehicleData.m_MaintenanceType & MaintenanceType.Vehicle) != MaintenanceType.None)
									{
										if (m_DestroyedData.TryGetComponent(val, ref destroyed))
										{
											float num4 = 500f * (1f - destroyed.m_Cleared);
											num2 += Mathf.RoundToInt(num4);
										}
										else if (m_DamagedData.TryGetComponent(val, ref damaged))
										{
											float num5 = math.min(500f, math.csum(damaged.m_Damage) * 500f);
											num2 += Mathf.RoundToInt(num5);
										}
									}
									if ((prefabMaintenanceVehicleData.m_MaintenanceType & MaintenanceType.Park) != MaintenanceType.None && m_ParkData.TryGetComponent(val, ref park))
									{
										PrefabRef prefabRef = m_PrefabRefData[val];
										if (m_PrefabParkData.TryGetComponent(prefabRef.m_Prefab, ref parkData))
										{
											num2 += math.max(0, parkData.m_MaintenancePool - park.m_Maintenance);
										}
									}
								}
								if (m_VehicleData.HasComponent(val))
								{
									car.m_Flags |= CarFlags.StayOnRoad;
								}
								else
								{
									car.m_Flags &= ~CarFlags.StayOnRoad;
								}
							}
							else
							{
								car.m_Flags |= CarFlags.StayOnRoad;
							}
							if (flag)
							{
								maintenanceVehicle.m_MaintainEstimate = num2;
							}
							if ((prefabMaintenanceVehicleData.m_MaintenanceType & (MaintenanceType.Road | MaintenanceType.Snow | MaintenanceType.Vehicle)) != MaintenanceType.None)
							{
								car.m_Flags |= CarFlags.UsePublicTransportLanes;
							}
							maintenanceVehicle.m_PathElementTime = num / (float)math.max(1, val3.Length);
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

		private void ReturnToDepot(int jobIndex, Entity vehicleEntity, Owner ownerData, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Car car, ref Game.Vehicles.MaintenanceVehicle maintenanceVehicle, ref PathOwner pathOwnerData, ref Target targetData)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			serviceDispatches.Clear();
			car.m_Flags &= ~(CarFlags.Warning | CarFlags.Working | CarFlags.SignalAnimation1 | CarFlags.SignalAnimation2);
			maintenanceVehicle.m_RequestCount = 0;
			maintenanceVehicle.m_MaintainEstimate = 0;
			maintenanceVehicle.m_State &= ~(MaintenanceVehicleFlags.TransformTarget | MaintenanceVehicleFlags.EdgeTarget | MaintenanceVehicleFlags.TryWork | MaintenanceVehicleFlags.Working | MaintenanceVehicleFlags.ClearingDebris);
			maintenanceVehicle.m_State |= MaintenanceVehicleFlags.Returning;
			VehicleUtils.SetTarget(ref pathOwnerData, ref targetData, ownerData.m_Owner);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(jobIndex, vehicleEntity, default(EffectsUpdated));
		}

		private void ResetPath(int jobIndex, Entity vehicleEntity, PathInformation pathInformation, DynamicBuffer<ServiceDispatch> serviceDispatches, MaintenanceVehicleData prefabMaintenanceVehicleData, ref Random random, ref Game.Vehicles.MaintenanceVehicle maintenanceVehicle, ref Car carData, ref CarCurrentLane currentLane, ref PathOwner pathOwner)
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
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PathElement> path = m_PathElements[vehicleEntity];
			PathUtils.ResetPath(ref currentLane, path, m_SlaveLaneData, m_OwnerData, m_SubLanes);
			VehicleUtils.ResetParkingLaneStatus(vehicleEntity, ref currentLane, ref pathOwner, path, ref m_EntityLookup, ref m_CurveData, ref m_ParkingLaneData, ref m_CarLaneData, ref m_ConnectionLaneData, ref m_SpawnLocationData, ref m_PrefabRefData, ref m_PrefabSpawnLocationData);
			VehicleUtils.SetParkingCurvePos(vehicleEntity, ref random, currentLane, pathOwner, path, ref m_ParkedCarData, ref m_UnspawnedData, ref m_CurveData, ref m_ParkingLaneData, ref m_ConnectionLaneData, ref m_PrefabRefData, ref m_PrefabObjectGeometryData, ref m_PrefabParkingLaneData, ref m_LaneObjects, ref m_LaneOverlaps, ignoreDriveways: false);
			currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.Checked;
			if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.Returning) == 0 && maintenanceVehicle.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				if (m_MaintenanceRequestData.HasComponent(request))
				{
					Entity target = m_MaintenanceRequestData[request].m_Target;
					int num = 0;
					bool flag = maintenanceVehicle.m_RequestCount == 1;
					if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.EdgeTarget) != 0)
					{
						NativeArray<PathElement> val = default(NativeArray<PathElement>);
						val._002Ector(path.Length, (Allocator)2, (NativeArrayOptions)1);
						val.CopyFrom(path.AsNativeArray());
						path.Clear();
						Entity lastOwner = Entity.Null;
						int dispatchIndex = BumpDispachIndex(request);
						for (int i = 0; i < val.Length; i++)
						{
							num += AddPathElement(path, val[i], request, dispatchIndex, ref lastOwner, flag);
						}
						val.Dispose();
						carData.m_Flags |= CarFlags.StayOnRoad;
					}
					else if (m_TransformData.HasComponent(target))
					{
						if (flag)
						{
							if ((prefabMaintenanceVehicleData.m_MaintenanceType & MaintenanceType.Vehicle) != MaintenanceType.None)
							{
								Destroyed destroyed = default(Destroyed);
								Damaged damaged = default(Damaged);
								if (m_DestroyedData.TryGetComponent(target, ref destroyed))
								{
									float num2 = 500f * (1f - destroyed.m_Cleared);
									num += Mathf.RoundToInt(num2);
								}
								else if (m_DamagedData.TryGetComponent(target, ref damaged))
								{
									float num3 = math.min(500f, math.csum(damaged.m_Damage) * 500f);
									num += Mathf.RoundToInt(num3);
								}
							}
							Game.Buildings.Park park = default(Game.Buildings.Park);
							if ((prefabMaintenanceVehicleData.m_MaintenanceType & MaintenanceType.Park) != MaintenanceType.None && m_ParkData.TryGetComponent(target, ref park))
							{
								PrefabRef prefabRef = m_PrefabRefData[target];
								ParkData parkData = default(ParkData);
								if (m_PrefabParkData.TryGetComponent(prefabRef.m_Prefab, ref parkData))
								{
									num += math.max(0, parkData.m_MaintenancePool - park.m_Maintenance);
								}
							}
						}
						if (m_VehicleData.HasComponent(target))
						{
							carData.m_Flags |= CarFlags.StayOnRoad;
						}
						else
						{
							carData.m_Flags &= ~CarFlags.StayOnRoad;
						}
					}
					else
					{
						carData.m_Flags |= CarFlags.StayOnRoad;
					}
					if (flag)
					{
						maintenanceVehicle.m_MaintainEstimate = num;
					}
				}
				else
				{
					carData.m_Flags |= CarFlags.StayOnRoad;
				}
			}
			else
			{
				carData.m_Flags &= ~CarFlags.StayOnRoad;
			}
			if ((prefabMaintenanceVehicleData.m_MaintenanceType & (MaintenanceType.Road | MaintenanceType.Snow | MaintenanceType.Vehicle)) != MaintenanceType.None)
			{
				carData.m_Flags |= CarFlags.UsePublicTransportLanes;
			}
			maintenanceVehicle.m_PathElementTime = pathInformation.m_Duration / (float)math.max(1, path.Length);
		}

		private int AddPathElement(DynamicBuffer<PathElement> path, PathElement pathElement, Entity request, int dispatchIndex, ref Entity lastOwner, bool collectMaintenance)
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
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			if (!m_EdgeLaneData.HasComponent(pathElement.m_Target))
			{
				path.Add(pathElement);
				lastOwner = Entity.Null;
				return num;
			}
			Owner owner = m_OwnerData[pathElement.m_Target];
			if (owner.m_Owner == lastOwner)
			{
				path.Add(pathElement);
				return num;
			}
			lastOwner = owner.m_Owner;
			float curvePos = pathElement.m_TargetDelta.y;
			if (FindClosestSidewalk(pathElement.m_Target, owner.m_Owner, ref curvePos, out var sidewalk))
			{
				num += AddMaintenanceRequests(owner.m_Owner, request, dispatchIndex, collectMaintenance);
				path.Add(pathElement);
				path.Add(new PathElement(sidewalk, float2.op_Implicit(curvePos)));
			}
			else
			{
				path.Add(pathElement);
			}
			return num;
		}

		private bool FindClosestSidewalk(Entity lane, Entity owner, ref float curvePos, out Entity sidewalk)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			sidewalk = Entity.Null;
			if (m_SubLanes.HasBuffer(owner))
			{
				float3 val = MathUtils.Position(m_CurveData[lane].m_Bezier, curvePos);
				DynamicBuffer<Game.Net.SubLane> val2 = m_SubLanes[owner];
				float num = float.MaxValue;
				float num3 = default(float);
				for (int i = 0; i < val2.Length; i++)
				{
					Entity subLane = val2[i].m_SubLane;
					if (m_PedestrianLaneData.HasComponent(subLane))
					{
						float num2 = MathUtils.Distance(MathUtils.Line(m_CurveData[subLane].m_Bezier), val, ref num3);
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

		private int AddMaintenanceRequests(Entity edgeEntity, Entity request, int dispatchIndex, bool collectMaintenance)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			int result = 0;
			if (m_NetConditionData.HasComponent(edgeEntity))
			{
				if (collectMaintenance)
				{
					Game.Net.Edge edge = m_EdgeData[edgeEntity];
					result = Mathf.RoundToInt(CalculateTotalLaneWear(edgeEntity) + (CalculateTotalLaneWear(edge.m_Start) + CalculateTotalLaneWear(edge.m_End)) * 0.5f);
				}
				m_ActionQueue.Enqueue(new MaintenanceAction
				{
					m_Type = MaintenanceActionType.AddRequest,
					m_Consumer = edgeEntity,
					m_Request = request,
					m_VehicleCapacity = dispatchIndex
				});
			}
			return result;
		}

		private float CalculateTotalLaneWear(Entity owner)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			if (m_SubLanes.HasBuffer(owner))
			{
				DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[owner];
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					if (m_LaneConditionData.HasComponent(subLane))
					{
						LaneCondition laneCondition = m_LaneConditionData[subLane];
						Curve curve = m_CurveData[subLane];
						num += laneCondition.m_Wear * curve.m_Length * 0.01f;
					}
				}
			}
			return num;
		}

		private void CheckMaintenancePresence(ref Car car, ref Game.Vehicles.MaintenanceVehicle maintenanceVehicle, ref CarCurrentLane currentLane, DynamicBuffer<CarNavigationLane> navigationLanes)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.ClearChecked) != 0)
			{
				if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Waypoint) != 0)
				{
					currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.Checked;
				}
				maintenanceVehicle.m_State &= ~MaintenanceVehicleFlags.ClearChecked;
			}
			if ((currentLane.m_LaneFlags & (Game.Vehicles.CarLaneFlags.Waypoint | Game.Vehicles.CarLaneFlags.Checked)) == Game.Vehicles.CarLaneFlags.Waypoint)
			{
				if (!CheckMaintenancePresence(currentLane.m_Lane))
				{
					currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.Waypoint;
					car.m_Flags &= ~(CarFlags.Warning | CarFlags.Working | CarFlags.SignalAnimation1 | CarFlags.SignalAnimation2);
					if (m_SlaveLaneData.HasComponent(currentLane.m_Lane))
					{
						currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.FixedLane;
					}
				}
				currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.Checked;
			}
			if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Waypoint) != 0)
			{
				car.m_Flags |= CarFlags.Warning;
				if ((car.m_Flags & CarFlags.Working) == 0 && math.abs(currentLane.m_CurvePosition.x - currentLane.m_CurvePosition.z) < 0.5f)
				{
					car.m_Flags |= GetWorkingFlags(ref currentLane);
				}
				return;
			}
			for (int i = 0; i < navigationLanes.Length; i++)
			{
				ref CarNavigationLane reference = ref navigationLanes.ElementAt(i);
				if ((reference.m_Flags & Game.Vehicles.CarLaneFlags.Waypoint) != 0 && (currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Checked) == 0)
				{
					if (!CheckMaintenancePresence(reference.m_Lane))
					{
						reference.m_Flags &= ~Game.Vehicles.CarLaneFlags.Waypoint;
						car.m_Flags &= ~CarFlags.Warning;
						if (m_SlaveLaneData.HasComponent(reference.m_Lane))
						{
							reference.m_Flags &= ~Game.Vehicles.CarLaneFlags.FixedLane;
						}
					}
					currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.Checked;
					car.m_Flags &= ~(CarFlags.Working | CarFlags.SignalAnimation1 | CarFlags.SignalAnimation2);
				}
				if ((reference.m_Flags & (Game.Vehicles.CarLaneFlags.Reserved | Game.Vehicles.CarLaneFlags.Waypoint)) != Game.Vehicles.CarLaneFlags.Reserved)
				{
					car.m_Flags &= ~(CarFlags.Working | CarFlags.SignalAnimation1 | CarFlags.SignalAnimation2);
					break;
				}
			}
		}

		private CarFlags GetWorkingFlags(ref CarCurrentLane currentLaneData)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			Game.Net.CarLaneFlags carLaneFlags = Game.Net.CarLaneFlags.RightLimit | Game.Net.CarLaneFlags.LeftLimit;
			Game.Net.CarLane carLane = default(Game.Net.CarLane);
			if (m_CarLaneData.TryGetComponent(currentLaneData.m_Lane, ref carLane))
			{
				carLaneFlags &= carLane.m_Flags;
			}
			switch (carLaneFlags)
			{
			case Game.Net.CarLaneFlags.RightLimit:
				return CarFlags.Working | CarFlags.SignalAnimation1;
			case Game.Net.CarLaneFlags.LeftLimit:
				return CarFlags.Working | CarFlags.SignalAnimation2;
			default:
				if (!m_LeftHandTraffic)
				{
					return CarFlags.Working | CarFlags.SignalAnimation1;
				}
				return CarFlags.Working | CarFlags.SignalAnimation2;
			}
		}

		private bool CheckMaintenancePresence(Entity laneEntity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			Owner owner = default(Owner);
			NetCondition netCondition = default(NetCondition);
			if (m_EdgeLaneData.HasComponent(laneEntity) && m_OwnerData.TryGetComponent(laneEntity, ref owner) && m_NetConditionData.TryGetComponent(owner.m_Owner, ref netCondition))
			{
				return math.any(netCondition.m_Wear >= 0.099999994f);
			}
			return false;
		}

		private void TryMaintain(Entity vehicleEntity, MaintenanceVehicleData prefabMaintenanceVehicleData, ref Car car, ref Game.Vehicles.MaintenanceVehicle maintenanceVehicle, ref CarCurrentLane currentLaneData)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			maintenanceVehicle.m_State |= MaintenanceVehicleFlags.TryWork;
			maintenanceVehicle.m_State &= ~MaintenanceVehicleFlags.Working;
			if (maintenanceVehicle.m_Maintained < prefabMaintenanceVehicleData.m_MaintenanceCapacity)
			{
				TryMaintainLane(vehicleEntity, prefabMaintenanceVehicleData, currentLaneData.m_Lane, ref currentLaneData);
			}
		}

		private void TryMaintainLane(Entity vehicleEntity, MaintenanceVehicleData prefabMaintenanceVehicleData, Entity laneEntity, ref CarCurrentLane currentLaneData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			Owner owner = default(Owner);
			NetCondition netCondition = default(NetCondition);
			if (m_EdgeLaneData.HasComponent(laneEntity) && m_OwnerData.TryGetComponent(laneEntity, ref owner) && m_NetConditionData.TryGetComponent(owner.m_Owner, ref netCondition) && math.any(netCondition.m_Wear >= 0.099999994f))
			{
				m_ActionQueue.Enqueue(new MaintenanceAction
				{
					m_Type = MaintenanceActionType.RoadMaintenance,
					m_Vehicle = vehicleEntity,
					m_Consumer = owner.m_Owner,
					m_VehicleCapacity = prefabMaintenanceVehicleData.m_MaintenanceCapacity,
					m_ConsumerCapacity = 0,
					m_MaxMaintenanceAmount = Mathf.RoundToInt((float)(prefabMaintenanceVehicleData.m_MaintenanceRate * 16) / 60f),
					m_WorkingFlags = GetWorkingFlags(ref currentLaneData)
				});
				m_ActionQueue.Enqueue(new MaintenanceAction
				{
					m_Type = MaintenanceActionType.ClearLane,
					m_Vehicle = vehicleEntity,
					m_Consumer = laneEntity
				});
			}
		}

		private void TryMaintain(Entity vehicleEntity, MaintenanceVehicleData prefabMaintenanceVehicleData, ref Car car, ref Game.Vehicles.MaintenanceVehicle maintenanceVehicle, ref CarCurrentLane currentLaneData, ref Target targetData)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			maintenanceVehicle.m_State |= MaintenanceVehicleFlags.TryWork;
			maintenanceVehicle.m_State &= ~MaintenanceVehicleFlags.Working;
			if (maintenanceVehicle.m_Maintained < prefabMaintenanceVehicleData.m_MaintenanceCapacity && m_PrefabRefData.HasComponent(targetData.m_Target))
			{
				PrefabRef prefabRef = m_PrefabRefData[targetData.m_Target];
				if (m_VehicleData.HasComponent(targetData.m_Target))
				{
					m_ActionQueue.Enqueue(new MaintenanceAction
					{
						m_Type = MaintenanceActionType.RepairVehicle,
						m_Vehicle = vehicleEntity,
						m_Consumer = targetData.m_Target,
						m_VehicleCapacity = prefabMaintenanceVehicleData.m_MaintenanceCapacity,
						m_MaxMaintenanceAmount = Mathf.RoundToInt((float)(prefabMaintenanceVehicleData.m_MaintenanceRate * 16) / 60f),
						m_WorkingFlags = GetWorkingFlags(ref currentLaneData)
					});
				}
				else if (m_PrefabParkData.HasComponent(prefabRef.m_Prefab))
				{
					ParkData parkData = m_PrefabParkData[prefabRef.m_Prefab];
					m_ActionQueue.Enqueue(new MaintenanceAction
					{
						m_Type = MaintenanceActionType.ParkMaintenance,
						m_Vehicle = vehicleEntity,
						m_Consumer = targetData.m_Target,
						m_VehicleCapacity = prefabMaintenanceVehicleData.m_MaintenanceCapacity,
						m_ConsumerCapacity = parkData.m_MaintenancePool,
						m_MaxMaintenanceAmount = Mathf.RoundToInt((float)(prefabMaintenanceVehicleData.m_MaintenanceRate * 16) / 60f),
						m_WorkingFlags = CarFlags.Working
					});
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct MaintenanceJob : IJob
	{
		[ReadOnly]
		public EntityArchetype m_DamageEventArchetype;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		public ComponentLookup<Car> m_CarData;

		public ComponentLookup<MaintenanceRequest> m_MaintenanceRequestData;

		public ComponentLookup<Game.Vehicles.MaintenanceVehicle> m_MaintenanceVehicleData;

		public ComponentLookup<MaintenanceConsumer> m_MaintenanceConsumerData;

		public ComponentLookup<Damaged> m_DamagedData;

		public ComponentLookup<Destroyed> m_DestroyedData;

		public ComponentLookup<Game.Buildings.Park> m_ParkData;

		public ComponentLookup<NetCondition> m_NetConditionData;

		public ComponentLookup<LaneCondition> m_LaneConditionData;

		public NativeQueue<MaintenanceAction> m_ActionQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_074c: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_0705: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0698: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			MaintenanceAction maintenanceAction = default(MaintenanceAction);
			NetCondition netCondition = default(NetCondition);
			NetCondition netCondition2 = default(NetCondition);
			NetCondition netCondition3 = default(NetCondition);
			DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
			Game.Vehicles.MaintenanceVehicle maintenanceVehicle = default(Game.Vehicles.MaintenanceVehicle);
			while (m_ActionQueue.TryDequeue(ref maintenanceAction))
			{
				switch (maintenanceAction.m_Type)
				{
				case MaintenanceActionType.AddRequest:
				{
					MaintenanceConsumer maintenanceConsumer = m_MaintenanceConsumerData[maintenanceAction.m_Consumer];
					maintenanceConsumer.m_Request = maintenanceAction.m_Request;
					maintenanceConsumer.m_DispatchIndex = (byte)maintenanceAction.m_VehicleCapacity;
					m_MaintenanceConsumerData[maintenanceAction.m_Consumer] = maintenanceConsumer;
					break;
				}
				case MaintenanceActionType.ParkMaintenance:
				{
					Car car2 = m_CarData[maintenanceAction.m_Vehicle];
					Game.Vehicles.MaintenanceVehicle maintenanceVehicle3 = m_MaintenanceVehicleData[maintenanceAction.m_Vehicle];
					Game.Buildings.Park park = m_ParkData[maintenanceAction.m_Consumer];
					int num3 = math.min(maintenanceAction.m_VehicleCapacity - maintenanceVehicle3.m_Maintained, maintenanceAction.m_ConsumerCapacity - park.m_Maintenance);
					num3 = math.min(num3, maintenanceAction.m_MaxMaintenanceAmount);
					if (num3 > 0)
					{
						maintenanceVehicle3.m_Maintained += num3;
						maintenanceVehicle3.m_MaintainEstimate = math.max(0, maintenanceVehicle3.m_MaintainEstimate - num3);
						park.m_Maintenance += (short)num3;
						maintenanceVehicle3.m_State |= MaintenanceVehicleFlags.Working;
						car2.m_Flags |= CarFlags.Warning | maintenanceAction.m_WorkingFlags;
						m_CarData[maintenanceAction.m_Vehicle] = car2;
						m_MaintenanceVehicleData[maintenanceAction.m_Vehicle] = maintenanceVehicle3;
						m_ParkData[maintenanceAction.m_Consumer] = park;
					}
					break;
				}
				case MaintenanceActionType.RoadMaintenance:
				{
					Car car = m_CarData[maintenanceAction.m_Vehicle];
					Game.Vehicles.MaintenanceVehicle maintenanceVehicle2 = m_MaintenanceVehicleData[maintenanceAction.m_Vehicle];
					Game.Net.Edge edge = m_EdgeData[maintenanceAction.m_Consumer];
					float num = CalculateTotalLaneWear(maintenanceAction.m_Consumer);
					num += CalculateTotalLaneWear(edge.m_Start);
					num += CalculateTotalLaneWear(edge.m_End);
					int num2 = math.min(maintenanceAction.m_VehicleCapacity - maintenanceVehicle2.m_Maintained, (int)math.ceil(num));
					num2 = math.min(num2, maintenanceAction.m_MaxMaintenanceAmount);
					if (num2 > 0)
					{
						maintenanceVehicle2.m_Maintained += num2;
						maintenanceVehicle2.m_MaintainEstimate = math.max(0, maintenanceVehicle2.m_MaintainEstimate - num2);
						float maintainFactor = math.saturate(1f - (float)num2 / num);
						maintenanceVehicle2.m_State |= MaintenanceVehicleFlags.Working;
						car.m_Flags &= ~(CarFlags.SignalAnimation1 | CarFlags.SignalAnimation2);
						car.m_Flags |= CarFlags.Warning | maintenanceAction.m_WorkingFlags;
						float2 wear = MaintainLanes(maintenanceAction.m_Consumer, maintainFactor);
						float2 wear2 = MaintainLanes(edge.m_Start, maintainFactor);
						float2 wear3 = MaintainLanes(edge.m_End, maintainFactor);
						if (m_NetConditionData.TryGetComponent(maintenanceAction.m_Consumer, ref netCondition))
						{
							netCondition.m_Wear = wear;
							m_NetConditionData[maintenanceAction.m_Consumer] = netCondition;
						}
						if (m_NetConditionData.TryGetComponent(edge.m_Start, ref netCondition2))
						{
							netCondition2.m_Wear = wear2;
							m_NetConditionData[edge.m_Start] = netCondition2;
						}
						if (m_NetConditionData.TryGetComponent(edge.m_End, ref netCondition3))
						{
							netCondition3.m_Wear = wear3;
							m_NetConditionData[edge.m_End] = netCondition3;
						}
						m_CarData[maintenanceAction.m_Vehicle] = car;
						m_MaintenanceVehicleData[maintenanceAction.m_Vehicle] = maintenanceVehicle2;
					}
					break;
				}
				case MaintenanceActionType.RepairVehicle:
				{
					Car car3 = m_CarData[maintenanceAction.m_Vehicle];
					Game.Vehicles.MaintenanceVehicle maintenanceVehicle4 = m_MaintenanceVehicleData[maintenanceAction.m_Vehicle];
					if (m_DestroyedData.HasComponent(maintenanceAction.m_Consumer))
					{
						Destroyed destroyed = m_DestroyedData[maintenanceAction.m_Consumer];
						float num4 = 500f * (1f - destroyed.m_Cleared);
						int num5 = math.min(maintenanceAction.m_VehicleCapacity - maintenanceVehicle4.m_Maintained, (int)math.ceil(num4));
						num5 = math.min(num5, maintenanceAction.m_MaxMaintenanceAmount);
						if (num5 > 0)
						{
							maintenanceVehicle4.m_Maintained += num5;
							maintenanceVehicle4.m_MaintainEstimate = math.max(0, maintenanceVehicle4.m_MaintainEstimate - num5);
							destroyed.m_Cleared = 1f - (1f - destroyed.m_Cleared) * math.saturate(1f - (float)num5 / num4);
							maintenanceVehicle4.m_State |= MaintenanceVehicleFlags.Working;
							car3.m_Flags &= ~(CarFlags.SignalAnimation1 | CarFlags.SignalAnimation2);
							car3.m_Flags |= CarFlags.Warning | maintenanceAction.m_WorkingFlags;
							if ((maintenanceVehicle4.m_State & MaintenanceVehicleFlags.ClearingDebris) == 0)
							{
								maintenanceVehicle4.m_State |= MaintenanceVehicleFlags.ClearingDebris;
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(maintenanceAction.m_Vehicle, default(EffectsUpdated));
							}
							m_CarData[maintenanceAction.m_Vehicle] = car3;
							m_MaintenanceVehicleData[maintenanceAction.m_Vehicle] = maintenanceVehicle4;
							m_DestroyedData[maintenanceAction.m_Consumer] = destroyed;
						}
					}
					else
					{
						if (!m_DamagedData.HasComponent(maintenanceAction.m_Consumer))
						{
							break;
						}
						Damaged damaged = m_DamagedData[maintenanceAction.m_Consumer];
						float num6 = math.min(500f, math.csum(damaged.m_Damage) * 500f);
						int num7 = math.min(maintenanceAction.m_VehicleCapacity - maintenanceVehicle4.m_Maintained, (int)math.ceil(num6));
						num7 = math.min(num7, maintenanceAction.m_MaxMaintenanceAmount);
						if (num7 > 0)
						{
							maintenanceVehicle4.m_Maintained += num7;
							maintenanceVehicle4.m_MaintainEstimate = math.max(0, maintenanceVehicle4.m_MaintainEstimate - num7);
							ref float3 damage = ref damaged.m_Damage;
							damage *= math.saturate(1f - (float)num7 / num6);
							maintenanceVehicle4.m_State |= MaintenanceVehicleFlags.Working;
							car3.m_Flags &= ~(CarFlags.SignalAnimation1 | CarFlags.SignalAnimation2);
							car3.m_Flags |= CarFlags.Warning | maintenanceAction.m_WorkingFlags;
							if ((maintenanceVehicle4.m_State & MaintenanceVehicleFlags.ClearingDebris) == 0)
							{
								maintenanceVehicle4.m_State |= MaintenanceVehicleFlags.ClearingDebris;
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(maintenanceAction.m_Vehicle, default(EffectsUpdated));
							}
							m_CarData[maintenanceAction.m_Vehicle] = car3;
							m_MaintenanceVehicleData[maintenanceAction.m_Vehicle] = maintenanceVehicle4;
							m_DamagedData[maintenanceAction.m_Consumer] = damaged;
							if (!math.any(damaged.m_Damage > 0f))
							{
								Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_DamageEventArchetype);
								((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Damage>(val2, new Damage(maintenanceAction.m_Consumer, new float3(0f, 0f, 0f)));
							}
						}
					}
					break;
				}
				case MaintenanceActionType.ClearLane:
				{
					if (!m_LaneObjects.TryGetBuffer(maintenanceAction.m_Consumer, ref val))
					{
						break;
					}
					for (int i = 0; i < val.Length; i++)
					{
						Entity laneObject = val[i].m_LaneObject;
						if (laneObject != maintenanceAction.m_Vehicle && m_MaintenanceVehicleData.TryGetComponent(laneObject, ref maintenanceVehicle))
						{
							maintenanceVehicle.m_State |= MaintenanceVehicleFlags.ClearChecked;
							m_MaintenanceVehicleData[laneObject] = maintenanceVehicle;
						}
					}
					break;
				}
				case MaintenanceActionType.BumpDispatchIndex:
				{
					MaintenanceRequest maintenanceRequest = m_MaintenanceRequestData[maintenanceAction.m_Request];
					maintenanceRequest.m_DispatchIndex++;
					m_MaintenanceRequestData[maintenanceAction.m_Request] = maintenanceRequest;
					break;
				}
				}
			}
		}

		private float CalculateTotalLaneWear(Entity owner)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			if (m_SubLanes.HasBuffer(owner))
			{
				DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[owner];
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					if (m_LaneConditionData.HasComponent(subLane))
					{
						LaneCondition laneCondition = m_LaneConditionData[subLane];
						Curve curve = m_CurveData[subLane];
						num += laneCondition.m_Wear * curve.m_Length * 0.01f;
					}
				}
			}
			return num;
		}

		private float2 MaintainLanes(Entity owner, float maintainFactor)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			float2 val = float2.op_Implicit(0f);
			if (m_SubLanes.HasBuffer(owner))
			{
				DynamicBuffer<Game.Net.SubLane> val2 = m_SubLanes[owner];
				EdgeLane edgeLane = default(EdgeLane);
				for (int i = 0; i < val2.Length; i++)
				{
					Entity subLane = val2[i].m_SubLane;
					if (m_LaneConditionData.HasComponent(subLane))
					{
						LaneCondition laneCondition = m_LaneConditionData[subLane];
						laneCondition.m_Wear *= maintainFactor;
						val = ((!m_EdgeLaneData.TryGetComponent(subLane, ref edgeLane)) ? math.max(val, float2.op_Implicit(laneCondition.m_Wear)) : math.select(val, float2.op_Implicit(laneCondition.m_Wear), new bool2(math.any(edgeLane.m_EdgeDelta == 0f), math.any(edgeLane.m_EdgeDelta == 1f)) & (laneCondition.m_Wear > val)));
						m_LaneConditionData[subLane] = laneCondition;
					}
				}
			}
			return val;
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
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stopped> __Game_Objects_Stopped_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Vehicles.MaintenanceVehicle> __Game_Vehicles_MaintenanceVehicle_RW_ComponentTypeHandle;

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
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Damaged> __Game_Objects_Damaged_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCondition> __Game_Net_NetCondition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneCondition> __Game_Net_LaneCondition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

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
		public ComponentLookup<Game.Net.Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> __Game_Events_InvolvedInAccident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AccidentSite> __Game_Events_AccidentSite_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OnFire> __Game_Events_OnFire_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> __Game_Buildings_Park_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.MaintenanceDepot> __Game_Buildings_MaintenanceDepot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MaintenanceVehicleData> __Game_Prefabs_MaintenanceVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkData> __Game_Prefabs_ParkData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> __Game_Simulation_MaintenanceRequest_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<TargetElement> __Game_Events_TargetElement_RO_BufferLookup;

		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RW_BufferLookup;

		public ComponentLookup<Car> __Game_Vehicles_Car_RW_ComponentLookup;

		public ComponentLookup<MaintenanceRequest> __Game_Simulation_MaintenanceRequest_RW_ComponentLookup;

		public ComponentLookup<Game.Vehicles.MaintenanceVehicle> __Game_Vehicles_MaintenanceVehicle_RW_ComponentLookup;

		public ComponentLookup<MaintenanceConsumer> __Game_Simulation_MaintenanceConsumer_RW_ComponentLookup;

		public ComponentLookup<Damaged> __Game_Objects_Damaged_RW_ComponentLookup;

		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RW_ComponentLookup;

		public ComponentLookup<Game.Buildings.Park> __Game_Buildings_Park_RW_ComponentLookup;

		public ComponentLookup<NetCondition> __Game_Net_NetCondition_RW_ComponentLookup;

		public ComponentLookup<LaneCondition> __Game_Net_LaneCondition_RW_ComponentLookup;

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
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Objects_Stopped_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stopped>(true);
			__Game_Vehicles_MaintenanceVehicle_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.MaintenanceVehicle>(false);
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
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Objects_Damaged_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_NetCondition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCondition>(true);
			__Game_Net_LaneCondition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneCondition>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.PedestrianLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_GarageLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarageLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Edge>(true);
			__Game_Events_InvolvedInAccident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InvolvedInAccident>(true);
			__Game_Events_AccidentSite_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccidentSite>(true);
			__Game_Events_OnFire_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OnFire>(true);
			__Game_Buildings_Park_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Park>(true);
			__Game_Buildings_MaintenanceDepot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.MaintenanceDepot>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_MaintenanceVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceVehicleData>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Prefabs_ParkData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Simulation_MaintenanceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceRequest>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Events_TargetElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TargetElement>(true);
			__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
			__Game_Vehicles_Car_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Car>(false);
			__Game_Simulation_MaintenanceRequest_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceRequest>(false);
			__Game_Vehicles_MaintenanceVehicle_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.MaintenanceVehicle>(false);
			__Game_Simulation_MaintenanceConsumer_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceConsumer>(false);
			__Game_Objects_Damaged_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(false);
			__Game_Common_Destroyed_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(false);
			__Game_Buildings_Park_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Park>(false);
			__Game_Net_NetCondition_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCondition>(false);
			__Game_Net_LaneCondition_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneCondition>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private SimulationSystem m_SimulationSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EntityQuery m_VehicleQuery;

	private EntityArchetype m_DamageEventArchetype;

	private EntityArchetype m_MaintenanceRequestArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_MovingToParkedCarRemoveTypes;

	private ComponentTypeSet m_MovingToParkedAddTypes;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 7;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[10]
		{
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Game.Vehicles.MaintenanceVehicle>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>(),
			ComponentType.Exclude<OutOfControl>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_DamageEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Damage>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_MaintenanceRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<MaintenanceRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_HandleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<HandleRequest>(),
			ComponentType.ReadWrite<Game.Common.Event>()
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
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07da: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_080f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0814: Unknown result type (might be due to invalid IL or missing references)
		//IL_082c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0872: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0894: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<MaintenanceAction> actionQueue = default(NativeQueue<MaintenanceAction>);
		actionQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		MaintenanceVehicleTickJob maintenanceVehicleTickJob = new MaintenanceVehicleTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StoppedType = InternalCompilerInterface.GetComponentTypeHandle<Stopped>(ref __TypeHandle.__Game_Objects_Stopped_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceVehicleType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.MaintenanceVehicle>(ref __TypeHandle.__Game_Vehicles_MaintenanceVehicle_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
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
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetConditionData = InternalCompilerInterface.GetComponentLookup<NetCondition>(ref __TypeHandle.__Game_Net_NetCondition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneConditionData = InternalCompilerInterface.GetComponentLookup<LaneCondition>(ref __TypeHandle.__Game_Net_LaneCondition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarageLaneData = InternalCompilerInterface.GetComponentLookup<GarageLane>(ref __TypeHandle.__Game_Net_GarageLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InvolvedInAccidentData = InternalCompilerInterface.GetComponentLookup<InvolvedInAccident>(ref __TypeHandle.__Game_Events_InvolvedInAccident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AccidentSiteData = InternalCompilerInterface.GetComponentLookup<AccidentSite>(ref __TypeHandle.__Game_Events_AccidentSite_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OnFireData = InternalCompilerInterface.GetComponentLookup<OnFire>(ref __TypeHandle.__Game_Events_OnFire_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceDepotData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.MaintenanceDepot>(ref __TypeHandle.__Game_Buildings_MaintenanceDepot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMaintenanceVehicleData = InternalCompilerInterface.GetComponentLookup<MaintenanceVehicleData>(ref __TypeHandle.__Game_Prefabs_MaintenanceVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabParkData = InternalCompilerInterface.GetComponentLookup<ParkData>(ref __TypeHandle.__Game_Prefabs_ParkData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceRequestData = InternalCompilerInterface.GetComponentLookup<MaintenanceRequest>(ref __TypeHandle.__Game_Simulation_MaintenanceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetElements = InternalCompilerInterface.GetBufferLookup<TargetElement>(ref __TypeHandle.__Game_Events_TargetElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex,
			m_RandomSeed = RandomSeed.Next(),
			m_MaintenanceRequestArchetype = m_MaintenanceRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_MovingToParkedCarRemoveTypes = m_MovingToParkedCarRemoveTypes,
			m_MovingToParkedAddTypes = m_MovingToParkedAddTypes
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		maintenanceVehicleTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		maintenanceVehicleTickJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		maintenanceVehicleTickJob.m_ActionQueue = actionQueue.AsParallelWriter();
		MaintenanceVehicleTickJob maintenanceVehicleTickJob2 = maintenanceVehicleTickJob;
		MaintenanceJob obj = new MaintenanceJob
		{
			m_DamageEventArchetype = m_DamageEventArchetype,
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarData = InternalCompilerInterface.GetComponentLookup<Car>(ref __TypeHandle.__Game_Vehicles_Car_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceRequestData = InternalCompilerInterface.GetComponentLookup<MaintenanceRequest>(ref __TypeHandle.__Game_Simulation_MaintenanceRequest_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceVehicleData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.MaintenanceVehicle>(ref __TypeHandle.__Game_Vehicles_MaintenanceVehicle_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceConsumerData = InternalCompilerInterface.GetComponentLookup<MaintenanceConsumer>(ref __TypeHandle.__Game_Simulation_MaintenanceConsumer_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetConditionData = InternalCompilerInterface.GetComponentLookup<NetCondition>(ref __TypeHandle.__Game_Net_NetCondition_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneConditionData = InternalCompilerInterface.GetComponentLookup<LaneCondition>(ref __TypeHandle.__Game_Net_LaneCondition_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActionQueue = actionQueue,
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<MaintenanceVehicleTickJob>(maintenanceVehicleTickJob2, m_VehicleQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = IJobExtensions.Schedule<MaintenanceJob>(obj, val2);
		actionQueue.Dispose(val3);
		m_PathfindSetupSystem.AddQueueWriter(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
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
	public MaintenanceVehicleAISystem()
	{
	}
}

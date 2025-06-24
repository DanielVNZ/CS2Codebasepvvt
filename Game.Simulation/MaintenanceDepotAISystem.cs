using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.City;
using Game.Common;
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
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class MaintenanceDepotAISystem : GameSystemBase
{
	private struct MaintenanceDepotAction
	{
		public Entity m_Entity;

		public bool m_Disabled;

		public static MaintenanceDepotAction SetDisabled(Entity vehicle, bool disabled)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return new MaintenanceDepotAction
			{
				m_Entity = vehicle,
				m_Disabled = disabled
			};
		}
	}

	[BurstCompile]
	private struct MaintenanceDepotTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> m_OwnedVehicleType;

		public ComponentTypeHandle<Game.Buildings.MaintenanceDepot> m_MaintenanceDepotType;

		public BufferTypeHandle<ServiceDispatch> m_ServiceRequestType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> m_MaintenanceRequestData;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Surface> m_SurfaceData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> m_ParkData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.MaintenanceVehicle> m_MaintenanceVehicleData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<NetCondition> m_NetConditionData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<MaintenanceDepotData> m_PrefabMaintenanceDepotData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		[ReadOnly]
		public EntityArchetype m_MaintenanceRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingCarAddTypes;

		[ReadOnly]
		public MaintenanceVehicleSelectData m_MaintenanceVehicleSelectData;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<MaintenanceDepotAction> m_ActionQueue;

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
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<Efficiency> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Game.Buildings.MaintenanceDepot> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.MaintenanceDepot>(ref m_MaintenanceDepotType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			BufferAccessor<OwnedVehicle> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedVehicle>(ref m_OwnedVehicleType);
			BufferAccessor<ServiceDispatch> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceRequestType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity entity = nativeArray[i];
				PrefabRef prefabRef = nativeArray2[i];
				Game.Buildings.MaintenanceDepot maintenanceDepot = nativeArray3[i];
				DynamicBuffer<OwnedVehicle> vehicles = bufferAccessor3[i];
				DynamicBuffer<ServiceDispatch> dispatches = bufferAccessor4[i];
				MaintenanceDepotData data = default(MaintenanceDepotData);
				if (m_PrefabMaintenanceDepotData.HasComponent(prefabRef.m_Prefab))
				{
					data = m_PrefabMaintenanceDepotData[prefabRef.m_Prefab];
				}
				if (bufferAccessor2.Length != 0)
				{
					UpgradeUtils.CombineStats<MaintenanceDepotData>(ref data, bufferAccessor2[i], ref m_PrefabRefData, ref m_PrefabMaintenanceDepotData);
				}
				float efficiency = BuildingUtils.GetEfficiency(bufferAccessor, i);
				float immediateEfficiency = BuildingUtils.GetImmediateEfficiency(bufferAccessor, i);
				Tick(unfilteredChunkIndex, entity, prefabRef, ref random, ref maintenanceDepot, data, efficiency, immediateEfficiency, vehicles, dispatches);
				nativeArray3[i] = maintenanceDepot;
			}
		}

		private unsafe void Tick(int jobIndex, Entity entity, PrefabRef prefabRef, ref Random random, ref Game.Buildings.MaintenanceDepot maintenanceDepot, MaintenanceDepotData prefabMaintenanceDepotData, float efficiency, float immediateEfficiency, DynamicBuffer<OwnedVehicle> vehicles, DynamicBuffer<ServiceDispatch> dispatches)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			int vehicleCapacity = BuildingUtils.GetVehicleCapacity(efficiency, prefabMaintenanceDepotData.m_VehicleCapacity);
			int num = BuildingUtils.GetVehicleCapacity(immediateEfficiency, prefabMaintenanceDepotData.m_VehicleCapacity);
			int availableVehicles = vehicleCapacity;
			float efficiency2 = prefabMaintenanceDepotData.m_VehicleEfficiency * (0.5f + efficiency * 0.5f);
			int length = vehicles.Length;
			StackList<Entity> parkedVehicles = StackList<Entity>.op_Implicit(new Span<Entity>((void*)stackalloc Entity[length], length));
			Game.Vehicles.MaintenanceVehicle maintenanceVehicle = default(Game.Vehicles.MaintenanceVehicle);
			ParkedCar parkedCar = default(ParkedCar);
			for (int i = 0; i < vehicles.Length; i++)
			{
				Entity vehicle = vehicles[i].m_Vehicle;
				if (!m_MaintenanceVehicleData.TryGetComponent(vehicle, ref maintenanceVehicle))
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
				availableVehicles--;
				bool flag = --num < 0;
				if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.Disabled) != 0 != flag)
				{
					m_ActionQueue.Enqueue(MaintenanceDepotAction.SetDisabled(vehicle, flag));
				}
			}
			int num2 = 0;
			while (num2 < dispatches.Length)
			{
				Entity request = dispatches[num2].m_Request;
				if (m_MaintenanceRequestData.HasComponent(request))
				{
					SpawnVehicle(jobIndex, ref random, entity, request, prefabMaintenanceDepotData, efficiency2, ref maintenanceDepot, ref availableVehicles, ref parkedVehicles);
					dispatches.RemoveAt(num2);
				}
				else if (!m_ServiceRequestData.HasComponent(request))
				{
					dispatches.RemoveAt(num2);
				}
				else
				{
					num2++;
				}
			}
			while (parkedVehicles.Length > math.max(0, prefabMaintenanceDepotData.m_VehicleCapacity + availableVehicles - vehicleCapacity))
			{
				int num3 = ((Random)(ref random)).NextInt(parkedVehicles.Length);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, parkedVehicles[num3]);
				parkedVehicles.RemoveAtSwapBack(num3);
			}
			for (int j = 0; j < parkedVehicles.Length; j++)
			{
				Entity val = parkedVehicles[j];
				Game.Vehicles.MaintenanceVehicle maintenanceVehicle2 = m_MaintenanceVehicleData[val];
				bool flag2 = availableVehicles <= 0;
				if ((maintenanceVehicle2.m_State & MaintenanceVehicleFlags.Disabled) != 0 != flag2)
				{
					m_ActionQueue.Enqueue(MaintenanceDepotAction.SetDisabled(val, flag2));
				}
			}
			if (availableVehicles != 0)
			{
				maintenanceDepot.m_Flags |= MaintenanceDepotFlags.HasAvailableVehicles;
				RequestTargetIfNeeded(jobIndex, entity, ref maintenanceDepot, availableVehicles);
			}
			else
			{
				maintenanceDepot.m_Flags &= ~MaintenanceDepotFlags.HasAvailableVehicles;
			}
		}

		private void RequestTargetIfNeeded(int jobIndex, Entity entity, ref Game.Buildings.MaintenanceDepot maintenanceDepot, int availableVehicles)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			if (!m_ServiceRequestData.HasComponent(maintenanceDepot.m_TargetRequest))
			{
				uint num = math.max(512u, 256u);
				if ((m_SimulationFrameIndex & (num - 1)) == 160)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_MaintenanceRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MaintenanceRequest>(jobIndex, val, new MaintenanceRequest(entity, availableVehicles));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(32u));
				}
			}
		}

		private void SpawnVehicle(int jobIndex, ref Random random, Entity entity, Entity request, MaintenanceDepotData prefabMaintenanceDepotData, float efficiency, ref Game.Buildings.MaintenanceDepot maintenanceDepot, ref int availableVehicles, ref StackList<Entity> parkedVehicles)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			if (availableVehicles <= 0)
			{
				return;
			}
			Entity val = Entity.Null;
			MaintenanceType allMaintenanceTypes = MaintenanceType.None;
			MaintenanceRequest maintenanceRequest = default(MaintenanceRequest);
			if (m_MaintenanceRequestData.TryGetComponent(request, ref maintenanceRequest))
			{
				val = maintenanceRequest.m_Target;
				allMaintenanceTypes = BuildingUtils.GetMaintenanceType(val, ref m_ParkData, ref m_NetConditionData, ref m_EdgeData, ref m_SurfaceData, ref m_VehicleData);
			}
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(val))
			{
				return;
			}
			Entity val2 = Entity.Null;
			PathInformation pathInformation = default(PathInformation);
			if (m_PathInformationData.TryGetComponent(request, ref pathInformation) && pathInformation.m_Origin != entity)
			{
				if (!CollectionUtils.RemoveValueSwapBack<Entity>(ref parkedVehicles, pathInformation.m_Origin))
				{
					return;
				}
				ParkedCar parkedCar = m_ParkedCarData[pathInformation.m_Origin];
				val2 = pathInformation.m_Origin;
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val2, ref m_ParkedToMovingRemoveTypes);
				Game.Vehicles.CarLaneFlags flags = Game.Vehicles.CarLaneFlags.EndReached | Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.FixedLane;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val2, ref m_ParkedToMovingCarAddTypes);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarCurrentLane>(jobIndex, val2, new CarCurrentLane(parkedCar, flags));
				if (m_ParkingLaneData.HasComponent(parkedCar.m_Lane) || m_SpawnLocationData.HasComponent(parkedCar.m_Lane))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, parkedCar.m_Lane);
				}
			}
			if (val2 == Entity.Null)
			{
				val2 = m_MaintenanceVehicleSelectData.CreateVehicle(m_CommandBuffer, jobIndex, ref random, m_TransformData[entity], entity, Entity.Null, allMaintenanceTypes, MaintenanceType.None, float4.op_Implicit(float.MaxValue), parked: false);
				if (val2 == Entity.Null)
				{
					return;
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val2, new Owner(entity));
			}
			availableVehicles--;
			MaintenanceVehicleFlags maintenanceVehicleFlags = (MaintenanceVehicleFlags)0u;
			if (m_NetConditionData.HasComponent(val))
			{
				maintenanceVehicleFlags |= MaintenanceVehicleFlags.EdgeTarget;
			}
			else if (m_TransformData.HasComponent(val))
			{
				maintenanceVehicleFlags |= MaintenanceVehicleFlags.TransformTarget;
			}
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Vehicles.MaintenanceVehicle>(jobIndex, val2, new Game.Vehicles.MaintenanceVehicle(maintenanceVehicleFlags, 1, efficiency));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(jobIndex, val2, new Target(val));
			((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<ServiceDispatch>(jobIndex, val2).Add(new ServiceDispatch(request));
			Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val3, new HandleRequest(request, val2, completed: false));
			DynamicBuffer<PathElement> sourceElements = default(DynamicBuffer<PathElement>);
			if (m_PathElements.TryGetBuffer(request, ref sourceElements) && sourceElements.Length != 0)
			{
				DynamicBuffer<PathElement> targetElements = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<PathElement>(jobIndex, val2);
				PathUtils.CopyPath(sourceElements, default(PathOwner), 0, targetElements);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathOwner>(jobIndex, val2, new PathOwner(PathFlags.Updated));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(jobIndex, val2, m_PathInformationData[request]);
			}
			if (m_ServiceRequestData.HasComponent(maintenanceDepot.m_TargetRequest))
			{
				val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val3, new HandleRequest(maintenanceDepot.m_TargetRequest, Entity.Null, completed: true));
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct MaintenanceDepotActionJob : IJob
	{
		public ComponentLookup<Game.Vehicles.MaintenanceVehicle> m_MaintenanceVehicleData;

		public NativeQueue<MaintenanceDepotAction> m_ActionQueue;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			MaintenanceDepotAction maintenanceDepotAction = default(MaintenanceDepotAction);
			Game.Vehicles.MaintenanceVehicle maintenanceVehicle = default(Game.Vehicles.MaintenanceVehicle);
			while (m_ActionQueue.TryDequeue(ref maintenanceDepotAction))
			{
				if (m_MaintenanceVehicleData.TryGetComponent(maintenanceDepotAction.m_Entity, ref maintenanceVehicle))
				{
					if (maintenanceDepotAction.m_Disabled)
					{
						maintenanceVehicle.m_State |= MaintenanceVehicleFlags.Disabled;
					}
					else
					{
						maintenanceVehicle.m_State &= ~MaintenanceVehicleFlags.Disabled;
					}
					m_MaintenanceVehicleData[maintenanceDepotAction.m_Entity] = maintenanceVehicle;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Buildings.MaintenanceDepot> __Game_Buildings_MaintenanceDepot_RW_ComponentTypeHandle;

		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> __Game_Simulation_MaintenanceRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Surface> __Game_Objects_Surface_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> __Game_Buildings_Park_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.MaintenanceVehicle> __Game_Vehicles_MaintenanceVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCondition> __Game_Net_NetCondition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MaintenanceDepotData> __Game_Prefabs_MaintenanceDepotData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

		public ComponentLookup<Game.Vehicles.MaintenanceVehicle> __Game_Vehicles_MaintenanceVehicle_RW_ComponentLookup;

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
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<OwnedVehicle>(true);
			__Game_Buildings_MaintenanceDepot_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.MaintenanceDepot>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Simulation_MaintenanceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceRequest>(true);
			__Game_Simulation_ServiceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Objects_Surface_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Surface>(true);
			__Game_Buildings_Park_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Park>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Vehicles_MaintenanceVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.MaintenanceVehicle>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Edge>(true);
			__Game_Net_NetCondition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCondition>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_MaintenanceDepotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceDepotData>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
			__Game_Vehicles_MaintenanceVehicle_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.MaintenanceVehicle>(false);
		}
	}

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_VehiclePrefabQuery;

	private EntityArchetype m_MaintenanceRequestArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_ParkedToMovingRemoveTypes;

	private ComponentTypeSet m_ParkedToMovingCarAddTypes;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private MaintenanceVehicleSelectData m_MaintenanceVehicleSelectData;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 160;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_MaintenanceVehicleSelectData = new MaintenanceVehicleSelectData((SystemBase)(object)this);
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Buildings.MaintenanceDepot>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_VehiclePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { MaintenanceVehicleSelectData.GetEntityQueryDesc() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
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
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		m_MaintenanceVehicleSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_VehiclePrefabQuery, (Allocator)3, out var jobHandle);
		NativeQueue<MaintenanceDepotAction> actionQueue = default(NativeQueue<MaintenanceDepotAction>);
		actionQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		MaintenanceDepotTickJob maintenanceDepotTickJob = new MaintenanceDepotTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicleType = InternalCompilerInterface.GetBufferTypeHandle<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceDepotType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.MaintenanceDepot>(ref __TypeHandle.__Game_Buildings_MaintenanceDepot_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceRequestData = InternalCompilerInterface.GetComponentLookup<MaintenanceRequest>(ref __TypeHandle.__Game_Simulation_MaintenanceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SurfaceData = InternalCompilerInterface.GetComponentLookup<Surface>(ref __TypeHandle.__Game_Objects_Surface_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceVehicleData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.MaintenanceVehicle>(ref __TypeHandle.__Game_Vehicles_MaintenanceVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetConditionData = InternalCompilerInterface.GetComponentLookup<NetCondition>(ref __TypeHandle.__Game_Net_NetCondition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMaintenanceDepotData = InternalCompilerInterface.GetComponentLookup<MaintenanceDepotData>(ref __TypeHandle.__Game_Prefabs_MaintenanceDepotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex,
			m_MaintenanceRequestArchetype = m_MaintenanceRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_ParkedToMovingRemoveTypes = m_ParkedToMovingRemoveTypes,
			m_ParkedToMovingCarAddTypes = m_ParkedToMovingCarAddTypes,
			m_MaintenanceVehicleSelectData = m_MaintenanceVehicleSelectData
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		maintenanceDepotTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		maintenanceDepotTickJob.m_ActionQueue = actionQueue.AsParallelWriter();
		MaintenanceDepotTickJob maintenanceDepotTickJob2 = maintenanceDepotTickJob;
		MaintenanceDepotActionJob obj = new MaintenanceDepotActionJob
		{
			m_MaintenanceVehicleData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.MaintenanceVehicle>(ref __TypeHandle.__Game_Vehicles_MaintenanceVehicle_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActionQueue = actionQueue
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<MaintenanceDepotTickJob>(maintenanceDepotTickJob2, m_BuildingQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, jobHandle));
		JobHandle val3 = IJobExtensions.Schedule<MaintenanceDepotActionJob>(obj, val2);
		actionQueue.Dispose(val3);
		m_MaintenanceVehicleSelectData.PostUpdate(val2);
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
	public MaintenanceDepotAISystem()
	{
	}
}

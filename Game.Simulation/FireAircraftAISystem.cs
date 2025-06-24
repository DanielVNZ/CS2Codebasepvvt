using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
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
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class FireAircraftAISystem : GameSystemBase
{
	private struct FireExtinguishing
	{
		public Entity m_Vehicle;

		public Entity m_Target;

		public Entity m_Request;

		public float m_FireIntensityDelta;

		public float m_WaterDamageDelta;

		public float m_DestroyedClearDelta;

		public FireExtinguishing(Entity vehicle, Entity target, Entity request, float intensityDelta, float waterDamageDelta, float destroyedClearDelta)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			m_Vehicle = vehicle;
			m_Target = target;
			m_Request = request;
			m_FireIntensityDelta = intensityDelta;
			m_WaterDamageDelta = waterDamageDelta;
			m_DestroyedClearDelta = destroyedClearDelta;
		}
	}

	[BurstCompile]
	private struct FireAircraftTickJob : IJobChunk
	{
		private struct ObjectRequestIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public float3 m_Position;

			public float m_Spread;

			public Entity m_Vehicle;

			public Entity m_Request;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<OnFire> m_OnFireData;

			public ComponentLookup<RescueTarget> m_RescueTargetData;

			public ComponentLookup<FireRescueRequest> m_FireRescueRequestData;

			public ParallelWriter<FireExtinguishing> m_ExtinguishingQueue;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity objectEntity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || (!m_OnFireData.HasComponent(objectEntity) && !m_RescueTargetData.HasComponent(objectEntity)) || math.distance(m_TransformData[objectEntity].m_Position, m_Position) > m_Spread)
				{
					return;
				}
				if (m_OnFireData.HasComponent(objectEntity))
				{
					if (m_OnFireData[objectEntity].m_RescueRequest != m_Request)
					{
						m_ExtinguishingQueue.Enqueue(new FireExtinguishing(m_Vehicle, objectEntity, m_Request, 0f, 0f, 0f));
					}
				}
				else if (m_RescueTargetData.HasComponent(objectEntity) && m_RescueTargetData[objectEntity].m_Request != m_Request)
				{
					m_ExtinguishingQueue.Enqueue(new FireExtinguishing(m_Vehicle, objectEntity, m_Request, 0f, 0f, 0f));
				}
			}
		}

		private struct ObjectExtinguishIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public float3 m_Position;

			public float m_Spread;

			public float m_ExtinguishRate;

			public float m_ClearRate;

			public Entity m_Vehicle;

			public Entity m_Target;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<OnFire> m_OnFireData;

			public ComponentLookup<Destroyed> m_DestroyedData;

			public ComponentLookup<RescueTarget> m_RescueTargetData;

			public ComponentLookup<FireRescueRequest> m_FireRescueRequestData;

			public ComponentLookup<Building> m_BuildingData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public EventHelpers.StructuralIntegrityData m_StructuralIntegrityData;

			public ParallelWriter<FireExtinguishing> m_ExtinguishingQueue;

			public Entity m_ExtinguishResult;

			public Entity m_ClearResult;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity objectEntity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds) && (m_OnFireData.HasComponent(objectEntity) || m_RescueTargetData.HasComponent(objectEntity)) && !(objectEntity == m_Target) && !(math.distance(m_TransformData[objectEntity].m_Position, m_Position) > m_Spread))
				{
					TryExtinguish(objectEntity);
				}
			}

			public void TryExtinguish(Entity entity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0123: Unknown result type (might be due to invalid IL or missing references)
				//IL_0130: Unknown result type (might be due to invalid IL or missing references)
				//IL_0131: Unknown result type (might be due to invalid IL or missing references)
				if (m_OnFireData.HasComponent(entity))
				{
					PrefabRef prefabRef = m_PrefabRefData[entity];
					if (m_OnFireData[entity].m_Intensity > 0f)
					{
						float structuralIntegrity = m_StructuralIntegrityData.GetStructuralIntegrity(prefabRef.m_Prefab, m_BuildingData.HasComponent(entity));
						float num = 4f / 15f * m_ExtinguishRate;
						float waterDamageDelta = num * 10f / structuralIntegrity;
						m_ExtinguishingQueue.Enqueue(new FireExtinguishing(m_Vehicle, entity, Entity.Null, 0f - num, waterDamageDelta, 0f));
						if (m_ExtinguishResult == Entity.Null)
						{
							m_ExtinguishResult = entity;
						}
					}
				}
				else
				{
					if (!m_DestroyedData.HasComponent(entity))
					{
						return;
					}
					Destroyed destroyed = m_DestroyedData[entity];
					if (destroyed.m_Cleared >= 0f && destroyed.m_Cleared < 1f)
					{
						float destroyedClearDelta = 4f / 15f * m_ClearRate;
						m_ExtinguishingQueue.Enqueue(new FireExtinguishing(m_Vehicle, entity, Entity.Null, 0f, 0f, destroyedClearDelta));
						if (m_ClearResult == Entity.Null)
						{
							m_ClearResult = entity;
						}
					}
				}
			}
		}

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

		public ComponentTypeHandle<Game.Vehicles.FireEngine> m_FireEngineType;

		public ComponentTypeHandle<Aircraft> m_AircraftType;

		public ComponentTypeHandle<AircraftCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<Target> m_TargetType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public BufferTypeHandle<AircraftNavigationLane> m_AircraftNavigationLaneType;

		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<HelicopterData> m_PrefabHelicopterData;

		[ReadOnly]
		public ComponentLookup<FireEngineData> m_PrefabFireEngineData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<FireRescueRequest> m_FireRescueRequestData;

		[ReadOnly]
		public ComponentLookup<OnFire> m_OnFireData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<RescueTarget> m_RescueTargetData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.FireStation> m_FireStationData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Blocker> m_BlockerData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public EntityArchetype m_FireRescueRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedAircraftRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedAddTypes;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		[ReadOnly]
		public EventHelpers.StructuralIntegrityData m_StructuralIntegrityData;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public ParallelWriter<FireExtinguishing> m_ExtinguishingQueue;

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
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<PathInformation> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
			NativeArray<AircraftCurrentLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftCurrentLane>(ref m_CurrentLaneType);
			NativeArray<Game.Vehicles.FireEngine> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.FireEngine>(ref m_FireEngineType);
			NativeArray<Aircraft> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Aircraft>(ref m_AircraftType);
			NativeArray<Target> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PathOwner> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			BufferAccessor<AircraftNavigationLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<AircraftNavigationLane>(ref m_AircraftNavigationLaneType);
			BufferAccessor<ServiceDispatch> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Owner owner = nativeArray2[i];
				PrefabRef prefabRef = nativeArray3[i];
				PathInformation pathInformation = nativeArray4[i];
				Game.Vehicles.FireEngine fireEngine = nativeArray6[i];
				Aircraft aircraft = nativeArray7[i];
				AircraftCurrentLane currentLane = nativeArray5[i];
				PathOwner pathOwner = nativeArray9[i];
				Target target = nativeArray8[i];
				DynamicBuffer<AircraftNavigationLane> navigationLanes = bufferAccessor[i];
				DynamicBuffer<ServiceDispatch> serviceDispatches = bufferAccessor2[i];
				VehicleUtils.CheckUnspawned(unfilteredChunkIndex, val, currentLane, isUnspawned, m_CommandBuffer);
				Tick(unfilteredChunkIndex, val, owner, prefabRef, pathInformation, navigationLanes, serviceDispatches, ref fireEngine, ref aircraft, ref currentLane, ref pathOwner, ref target);
				nativeArray6[i] = fireEngine;
				nativeArray7[i] = aircraft;
				nativeArray5[i] = currentLane;
				nativeArray9[i] = pathOwner;
				nativeArray8[i] = target;
			}
		}

		private void Tick(int jobIndex, Entity vehicleEntity, Owner owner, PrefabRef prefabRef, PathInformation pathInformation, DynamicBuffer<AircraftNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.FireEngine fireEngine, ref Aircraft aircraft, ref AircraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			if (VehicleUtils.ResetUpdatedPath(ref pathOwner))
			{
				ResetPath(jobIndex, vehicleEntity, pathInformation, serviceDispatches, ref fireEngine, ref aircraft, ref currentLane);
			}
			FireEngineData fireEngineData = m_PrefabFireEngineData[prefabRef.m_Prefab];
			if (VehicleUtils.IsStuck(pathOwner))
			{
				Blocker blocker = m_BlockerData[vehicleEntity];
				bool num = m_ParkedCarData.HasComponent(blocker.m_Blocker);
				if (num)
				{
					Entity val = blocker.m_Blocker;
					Controller controller = default(Controller);
					if (m_ControllerData.TryGetComponent(val, ref controller))
					{
						val = controller.m_Controller;
					}
					DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
					m_LayoutElements.TryGetBuffer(val, ref layout);
					VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, val, layout);
				}
				if (num || blocker.m_Blocker == Entity.Null)
				{
					pathOwner.m_State &= ~PathFlags.Stuck;
					m_BlockerData[vehicleEntity] = default(Blocker);
				}
			}
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(target.m_Target) || VehicleUtils.PathfindFailed(pathOwner))
			{
				if (VehicleUtils.IsStuck(pathOwner) || (fireEngine.m_State & FireEngineFlags.Returning) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
					return;
				}
				ReturnToDepot(jobIndex, vehicleEntity, owner, serviceDispatches, ref fireEngine, ref aircraft, ref pathOwner, ref target);
			}
			else if (VehicleUtils.PathEndReached(currentLane))
			{
				if ((fireEngine.m_State & FireEngineFlags.Returning) != 0)
				{
					if (VehicleUtils.ParkingSpaceReached(currentLane, pathOwner))
					{
						ParkAircraft(jobIndex, vehicleEntity, owner, fireEngineData, ref aircraft, ref fireEngine, ref currentLane);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
					}
					return;
				}
				if ((fireEngine.m_State & (FireEngineFlags.Extinguishing | FireEngineFlags.Rescueing)) == 0 && !BeginExtinguishing(jobIndex, vehicleEntity, ref fireEngine, target))
				{
					CheckServiceDispatches(vehicleEntity, serviceDispatches, fireEngineData, ref fireEngine, ref pathOwner);
					if ((fireEngine.m_State & (FireEngineFlags.Empty | FireEngineFlags.Disabled)) != 0 || !SelectNextDispatch(jobIndex, vehicleEntity, navigationLanes, serviceDispatches, ref fireEngine, ref aircraft, ref currentLane, ref pathOwner, ref target))
					{
						ReturnToDepot(jobIndex, vehicleEntity, owner, serviceDispatches, ref fireEngine, ref aircraft, ref pathOwner, ref target);
					}
				}
			}
			else if ((currentLane.m_LaneFlags & AircraftLaneFlags.EndOfPath) != 0 && (fireEngine.m_State & (FireEngineFlags.Returning | FireEngineFlags.Extinguishing | FireEngineFlags.Rescueing)) == 0 && !CheckExtinguishing(jobIndex, vehicleEntity, ref fireEngine, target))
			{
				CheckServiceDispatches(vehicleEntity, serviceDispatches, fireEngineData, ref fireEngine, ref pathOwner);
				if ((fireEngine.m_State & (FireEngineFlags.Empty | FireEngineFlags.Disabled)) != 0 || !SelectNextDispatch(jobIndex, vehicleEntity, navigationLanes, serviceDispatches, ref fireEngine, ref aircraft, ref currentLane, ref pathOwner, ref target))
				{
					ReturnToDepot(jobIndex, vehicleEntity, owner, serviceDispatches, ref fireEngine, ref aircraft, ref pathOwner, ref target);
				}
			}
			if (fireEngineData.m_ExtinguishingCapacity != 0f && fireEngine.m_RequestCount <= 1)
			{
				OnFire onFire = default(OnFire);
				if (fireEngine.m_RequestCount == 1 && m_OnFireData.TryGetComponent(target.m_Target, ref onFire) && onFire.m_Intensity > 0f)
				{
					fireEngine.m_State |= FireEngineFlags.EstimatedEmpty;
				}
				else
				{
					fireEngine.m_State &= ~FireEngineFlags.EstimatedEmpty;
				}
			}
			if ((fireEngine.m_State & FireEngineFlags.Empty) != 0)
			{
				serviceDispatches.Clear();
			}
			else
			{
				CheckServiceDispatches(vehicleEntity, serviceDispatches, fireEngineData, ref fireEngine, ref pathOwner);
				if (fireEngine.m_RequestCount <= 1 && (fireEngine.m_State & FireEngineFlags.EstimatedEmpty) == 0)
				{
					RequestTargetIfNeeded(jobIndex, vehicleEntity, ref fireEngine);
				}
			}
			if ((fireEngine.m_State & (FireEngineFlags.Extinguishing | FireEngineFlags.Rescueing)) != 0)
			{
				if (!TryExtinguishFire(vehicleEntity, fireEngineData, ref fireEngine, ref target) && ((fireEngine.m_State & (FireEngineFlags.Empty | FireEngineFlags.Disabled)) != 0 || !SelectNextDispatch(jobIndex, vehicleEntity, navigationLanes, serviceDispatches, ref fireEngine, ref aircraft, ref currentLane, ref pathOwner, ref target)))
				{
					ReturnToDepot(jobIndex, vehicleEntity, owner, serviceDispatches, ref fireEngine, ref aircraft, ref pathOwner, ref target);
				}
			}
			else if ((fireEngine.m_State & (FireEngineFlags.Returning | FireEngineFlags.Empty | FireEngineFlags.Disabled)) == FireEngineFlags.Returning)
			{
				SelectNextDispatch(jobIndex, vehicleEntity, navigationLanes, serviceDispatches, ref fireEngine, ref aircraft, ref currentLane, ref pathOwner, ref target);
			}
			if ((aircraft.m_Flags & AircraftFlags.Emergency) != 0)
			{
				TryAddRequests(vehicleEntity, fireEngineData, serviceDispatches, ref fireEngine, ref target);
			}
			if ((fireEngine.m_State & (FireEngineFlags.Extinguishing | FireEngineFlags.Rescueing)) == 0)
			{
				if (VehicleUtils.RequireNewPath(pathOwner))
				{
					FindNewPath(vehicleEntity, prefabRef, ref fireEngine, ref currentLane, ref pathOwner, ref target);
				}
				else if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Stuck)) == 0)
				{
					CheckParkingSpace(ref aircraft, ref currentLane, ref pathOwner);
				}
			}
		}

		private void CheckParkingSpace(ref Aircraft aircraft, ref AircraftCurrentLane currentLane, ref PathOwner pathOwner)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Game.Objects.SpawnLocation spawnLocation = default(Game.Objects.SpawnLocation);
			if ((currentLane.m_LaneFlags & AircraftLaneFlags.EndOfPath) == 0 || !m_SpawnLocationData.TryGetComponent(currentLane.m_Lane, ref spawnLocation))
			{
				return;
			}
			if ((spawnLocation.m_Flags & SpawnLocationFlags.ParkedVehicle) != 0)
			{
				if ((aircraft.m_Flags & AircraftFlags.IgnoreParkedVehicle) == 0)
				{
					aircraft.m_Flags |= AircraftFlags.IgnoreParkedVehicle;
					pathOwner.m_State |= PathFlags.Obsolete;
				}
			}
			else
			{
				aircraft.m_Flags &= ~AircraftFlags.IgnoreParkedVehicle;
			}
		}

		private void ParkAircraft(int jobIndex, Entity entity, Owner owner, FireEngineData fireEngineData, ref Aircraft aircraft, ref Game.Vehicles.FireEngine fireEngine, ref AircraftCurrentLane currentLane)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			aircraft.m_Flags &= ~(AircraftFlags.Emergency | AircraftFlags.IgnoreParkedVehicle);
			fireEngine.m_State = (FireEngineFlags)0u;
			fireEngine.m_ExtinguishingAmount = fireEngineData.m_ExtinguishingCapacity;
			Game.Buildings.FireStation fireStation = default(Game.Buildings.FireStation);
			if (m_FireStationData.TryGetComponent(owner.m_Owner, ref fireStation))
			{
				if ((fireStation.m_Flags & FireStationFlags.HasAvailableFireHelicopters) == 0)
				{
					fireEngine.m_State |= FireEngineFlags.Disabled;
				}
				if ((fireStation.m_Flags & FireStationFlags.DisasterResponseAvailable) != 0)
				{
					fireEngine.m_State |= FireEngineFlags.DisasterResponse;
				}
			}
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, entity, ref m_MovingToParkedAircraftRemoveTypes);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, entity, ref m_MovingToParkedAddTypes);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkedCar>(jobIndex, entity, new ParkedCar(currentLane.m_Lane, currentLane.m_CurvePosition.x));
			if (m_SpawnLocationData.HasComponent(currentLane.m_Lane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, currentLane.m_Lane);
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<FixParkingLocation>(jobIndex, entity, new FixParkingLocation(Entity.Null, entity));
		}

		private void FindNewPath(Entity vehicleEntity, PrefabRef prefabRef, ref Game.Vehicles.FireEngine fireEngine, ref AircraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			HelicopterData helicopterData = m_PrefabHelicopterData[prefabRef.m_Prefab];
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(helicopterData.m_FlyingMaxSpeed),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Methods = (PathMethod.Road | PathMethod.Flying),
				m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = (PathMethod.Road | PathMethod.Flying),
				m_RoadTypes = RoadTypes.Helicopter,
				m_FlyingTypes = RoadTypes.Helicopter
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Entity = target.m_Target
			};
			if ((fireEngine.m_State & FireEngineFlags.Returning) == 0)
			{
				parameters.m_Weights = new PathfindWeights(1f, 0f, 0f, 0f);
				parameters.m_IgnoredRules |= RuleFlags.ForbidHeavyTraffic;
				destination.m_Value2 = 30f;
				destination.m_Methods = PathMethod.Flying;
				destination.m_FlyingTypes = RoadTypes.Helicopter;
			}
			else
			{
				parameters.m_Weights = new PathfindWeights(1f, 1f, 1f, 1f);
				destination.m_Methods = PathMethod.Road;
				destination.m_RoadTypes = RoadTypes.Helicopter;
				destination.m_RandomCost = 30f;
			}
			VehicleUtils.SetupPathfind(item: new SetupQueueItem(vehicleEntity, parameters, origin, destination), currentLane: ref currentLane, pathOwner: ref pathOwner, queue: m_PathfindQueue);
		}

		private void CheckServiceDispatches(Entity vehicleEntity, DynamicBuffer<ServiceDispatch> serviceDispatches, FireEngineData prefabFireEngineData, ref Game.Vehicles.FireEngine fireEngine, ref PathOwner pathOwner)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			if (serviceDispatches.Length <= fireEngine.m_RequestCount)
			{
				return;
			}
			float num = -1f;
			Entity val = Entity.Null;
			PathElement pathElement = default(PathElement);
			bool flag = false;
			int num2 = 0;
			if (fireEngine.m_RequestCount >= 1 && (fireEngine.m_State & FireEngineFlags.Returning) == 0)
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
			for (int i = num2; i < fireEngine.m_RequestCount; i++)
			{
				Entity request = serviceDispatches[i].m_Request;
				if (m_PathElements.TryGetBuffer(request, ref val3) && val3.Length != 0)
				{
					pathElement = val3[val3.Length - 1];
					flag = true;
				}
			}
			DynamicBuffer<PathElement> val4 = default(DynamicBuffer<PathElement>);
			for (int j = fireEngine.m_RequestCount; j < serviceDispatches.Length; j++)
			{
				Entity request2 = serviceDispatches[j].m_Request;
				if (!m_FireRescueRequestData.HasComponent(request2))
				{
					continue;
				}
				FireRescueRequest fireRescueRequest = m_FireRescueRequestData[request2];
				if (flag && m_PathElements.TryGetBuffer(request2, ref val4) && val4.Length != 0)
				{
					PathElement pathElement2 = val4[0];
					if (pathElement2.m_Target != pathElement.m_Target || pathElement2.m_TargetDelta.x != pathElement.m_TargetDelta.y)
					{
						continue;
					}
				}
				if (((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(fireRescueRequest.m_Target) && fireRescueRequest.m_Priority > num)
				{
					num = fireRescueRequest.m_Priority;
					val = request2;
				}
			}
			if (val != Entity.Null)
			{
				if (prefabFireEngineData.m_ExtinguishingCapacity != 0f)
				{
					FireRescueRequest fireRescueRequest2 = m_FireRescueRequestData[val];
					OnFire onFire = default(OnFire);
					if (m_OnFireData.TryGetComponent(fireRescueRequest2.m_Target, ref onFire) && onFire.m_Intensity > 0f)
					{
						fireEngine.m_State |= FireEngineFlags.EstimatedEmpty;
					}
					else if (fireEngine.m_RequestCount == 0)
					{
						fireEngine.m_State &= ~FireEngineFlags.EstimatedEmpty;
					}
				}
				serviceDispatches[fireEngine.m_RequestCount++] = new ServiceDispatch(val);
			}
			if (serviceDispatches.Length > fireEngine.m_RequestCount)
			{
				serviceDispatches.RemoveRange(fireEngine.m_RequestCount, serviceDispatches.Length - fireEngine.m_RequestCount);
			}
		}

		private void RequestTargetIfNeeded(int jobIndex, Entity entity, ref Game.Vehicles.FireEngine fireEngine)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			if (!m_FireRescueRequestData.HasComponent(fireEngine.m_TargetRequest))
			{
				uint num = math.max(64u, 16u);
				if ((m_SimulationFrameIndex & (num - 1)) == 10)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_FireRescueRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<FireRescueRequest>(jobIndex, val, new FireRescueRequest(entity, 1f, FireRescueRequestType.Fire));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(4u));
				}
			}
		}

		private bool SelectNextDispatch(int jobIndex, Entity vehicleEntity, DynamicBuffer<AircraftNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.FireEngine fireEngine, ref Aircraft aircraft, ref AircraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			if ((fireEngine.m_State & FireEngineFlags.Returning) == 0 && fireEngine.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				serviceDispatches.RemoveAt(0);
				fireEngine.m_RequestCount--;
			}
			FireRescueRequest fireRescueRequest = default(FireRescueRequest);
			OnFire onFire = default(OnFire);
			while (fireEngine.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				Entity val = Entity.Null;
				if (m_FireRescueRequestData.TryGetComponent(request, ref fireRescueRequest))
				{
					val = fireRescueRequest.m_Target;
				}
				if (fireRescueRequest.m_Type == FireRescueRequestType.Fire)
				{
					if (!m_OnFireData.TryGetComponent(val, ref onFire) || onFire.m_Intensity == 0f)
					{
						val = Entity.Null;
					}
				}
				else if (!m_RescueTargetData.HasComponent(val))
				{
					val = Entity.Null;
				}
				if (val == Entity.Null)
				{
					serviceDispatches.RemoveAt(0);
					fireEngine.m_RequestCount--;
					continue;
				}
				aircraft.m_Flags &= ~AircraftFlags.IgnoreParkedVehicle;
				fireEngine.m_State &= ~(FireEngineFlags.Extinguishing | FireEngineFlags.Rescueing);
				fireEngine.m_State &= ~FireEngineFlags.Returning;
				Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(request, vehicleEntity, completed: false, pathConsumed: true));
				if (m_FireRescueRequestData.HasComponent(fireEngine.m_TargetRequest))
				{
					val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(fireEngine.m_TargetRequest, Entity.Null, completed: true));
				}
				if (m_PathElements.HasBuffer(request))
				{
					DynamicBuffer<PathElement> appendPath = m_PathElements[request];
					if (appendPath.Length != 0)
					{
						DynamicBuffer<PathElement> val3 = m_PathElements[vehicleEntity];
						PathUtils.TrimPath(val3, ref pathOwner);
						float num = fireEngine.m_PathElementTime * (float)val3.Length + m_PathInformationData[request].m_Duration;
						if (PathUtils.TryAppendPath(ref currentLane, navigationLanes, val3, appendPath))
						{
							fireEngine.m_PathElementTime = num / (float)math.max(1, val3.Length);
							target.m_Target = val;
							VehicleUtils.ClearEndOfPath(ref currentLane, navigationLanes);
							aircraft.m_Flags |= AircraftFlags.Emergency | AircraftFlags.StayMidAir;
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(jobIndex, vehicleEntity);
							return true;
						}
					}
				}
				VehicleUtils.SetTarget(ref pathOwner, ref target, val);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(jobIndex, vehicleEntity);
				return true;
			}
			return false;
		}

		private bool BeginExtinguishing(int jobIndex, Entity vehicleEntity, ref Game.Vehicles.FireEngine fireEngine, Target target)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if ((fireEngine.m_State & FireEngineFlags.Empty) != 0)
			{
				return false;
			}
			if (m_OnFireData.HasComponent(target.m_Target))
			{
				fireEngine.m_State |= FireEngineFlags.Extinguishing;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(jobIndex, vehicleEntity);
				return true;
			}
			if (m_RescueTargetData.HasComponent(target.m_Target))
			{
				fireEngine.m_State |= FireEngineFlags.Rescueing;
				return true;
			}
			return false;
		}

		private bool CheckExtinguishing(int jobIndex, Entity vehicleEntity, ref Game.Vehicles.FireEngine fireEngine, Target target)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if ((fireEngine.m_State & FireEngineFlags.Empty) != 0)
			{
				return false;
			}
			if (m_OnFireData.HasComponent(target.m_Target))
			{
				return true;
			}
			if (m_RescueTargetData.HasComponent(target.m_Target))
			{
				return true;
			}
			return false;
		}

		private void ReturnToDepot(int jobIndex, Entity vehicleEntity, Owner ownerData, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.FireEngine fireEngine, ref Aircraft aircraft, ref PathOwner pathOwnerData, ref Target targetData)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			serviceDispatches.Clear();
			fireEngine.m_RequestCount = 0;
			fireEngine.m_State &= ~(FireEngineFlags.Extinguishing | FireEngineFlags.Rescueing);
			fireEngine.m_State |= FireEngineFlags.Returning;
			aircraft.m_Flags &= ~AircraftFlags.IgnoreParkedVehicle;
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(jobIndex, vehicleEntity);
			VehicleUtils.SetTarget(ref pathOwnerData, ref targetData, ownerData.m_Owner);
		}

		private void ResetPath(int jobIndex, Entity vehicleEntity, PathInformation pathInformation, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.FireEngine fireEngine, ref Aircraft aircraft, ref AircraftCurrentLane currentLane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PathElement> path = m_PathElements[vehicleEntity];
			PathUtils.ResetPath(ref currentLane, path);
			if ((fireEngine.m_State & FireEngineFlags.Returning) == 0 && fireEngine.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				if (m_FireRescueRequestData.HasComponent(request))
				{
					aircraft.m_Flags |= AircraftFlags.Emergency | AircraftFlags.StayMidAir;
				}
				else
				{
					aircraft.m_Flags &= ~AircraftFlags.Emergency;
					aircraft.m_Flags |= AircraftFlags.StayMidAir;
				}
			}
			else
			{
				aircraft.m_Flags &= ~(AircraftFlags.StayOnTaxiway | AircraftFlags.Emergency | AircraftFlags.StayMidAir);
			}
			fireEngine.m_PathElementTime = pathInformation.m_Duration / (float)math.max(1, path.Length);
		}

		private bool TryExtinguishFire(Entity vehicleEntity, FireEngineData prefabFireEngineData, ref Game.Vehicles.FireEngine fireEngine, ref Target target)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			if ((fireEngine.m_State & FireEngineFlags.Empty) != 0)
			{
				return false;
			}
			if (m_TransformData.HasComponent(target.m_Target))
			{
				Transform transform = m_TransformData[target.m_Target];
				float extinguishingSpread = prefabFireEngineData.m_ExtinguishingSpread;
				float num = prefabFireEngineData.m_ExtinguishingRate * fireEngine.m_Efficiency;
				float clearRate = fireEngine.m_Efficiency / math.max(0.001f, prefabFireEngineData.m_DestroyedClearDuration);
				ObjectExtinguishIterator objectExtinguishIterator = new ObjectExtinguishIterator
				{
					m_Bounds = new Bounds3(transform.m_Position - extinguishingSpread, transform.m_Position + extinguishingSpread),
					m_Position = transform.m_Position,
					m_Spread = extinguishingSpread,
					m_ExtinguishRate = num,
					m_ClearRate = clearRate,
					m_Vehicle = vehicleEntity,
					m_Target = target.m_Target,
					m_TransformData = m_TransformData,
					m_OnFireData = m_OnFireData,
					m_DestroyedData = m_DestroyedData,
					m_RescueTargetData = m_RescueTargetData,
					m_FireRescueRequestData = m_FireRescueRequestData,
					m_BuildingData = m_BuildingData,
					m_PrefabRefData = m_PrefabRefData,
					m_StructuralIntegrityData = m_StructuralIntegrityData,
					m_ExtinguishingQueue = m_ExtinguishingQueue
				};
				if (m_OnFireData.HasComponent(target.m_Target) || m_RescueTargetData.HasComponent(target.m_Target))
				{
					objectExtinguishIterator.TryExtinguish(target.m_Target);
				}
				m_ObjectSearchTree.Iterate<ObjectExtinguishIterator>(ref objectExtinguishIterator, 0);
				if (objectExtinguishIterator.m_ExtinguishResult != Entity.Null)
				{
					float num2 = 4f / 15f;
					fireEngine.m_ExtinguishingAmount = math.max(0f, fireEngine.m_ExtinguishingAmount - num * num2);
					if (fireEngine.m_ExtinguishingAmount == 0f && prefabFireEngineData.m_ExtinguishingCapacity != 0f)
					{
						fireEngine.m_State |= FireEngineFlags.Empty;
					}
					return true;
				}
				if (objectExtinguishIterator.m_ClearResult != Entity.Null)
				{
					return true;
				}
			}
			return false;
		}

		private void TryAddRequests(Entity vehicleEntity, FireEngineData prefabFireEngineData, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.FireEngine fireEngine, ref Target target)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			if (fireEngine.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				if (m_FireRescueRequestData.HasComponent(request) && m_TransformData.HasComponent(target.m_Target))
				{
					Transform transform = m_TransformData[target.m_Target];
					float extinguishingSpread = prefabFireEngineData.m_ExtinguishingSpread;
					ObjectRequestIterator objectRequestIterator = new ObjectRequestIterator
					{
						m_Bounds = new Bounds3(transform.m_Position - extinguishingSpread, transform.m_Position + extinguishingSpread),
						m_Position = transform.m_Position,
						m_Spread = extinguishingSpread,
						m_Vehicle = vehicleEntity,
						m_Request = request,
						m_TransformData = m_TransformData,
						m_OnFireData = m_OnFireData,
						m_RescueTargetData = m_RescueTargetData,
						m_FireRescueRequestData = m_FireRescueRequestData,
						m_ExtinguishingQueue = m_ExtinguishingQueue
					};
					m_ObjectSearchTree.Iterate<ObjectRequestIterator>(ref objectRequestIterator, 0);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FireExtinguishingJob : IJob
	{
		public ComponentLookup<OnFire> m_OnFireData;

		public ComponentLookup<RescueTarget> m_RescueTargetData;

		public ComponentLookup<Damaged> m_DamagedData;

		public ComponentLookup<Destroyed> m_DestroyedData;

		public NativeQueue<FireExtinguishing> m_ExtinguishingQueue;

		public void Execute()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			FireExtinguishing fireExtinguishing = default(FireExtinguishing);
			while (m_ExtinguishingQueue.TryDequeue(ref fireExtinguishing))
			{
				if (fireExtinguishing.m_Request != Entity.Null)
				{
					if (m_OnFireData.HasComponent(fireExtinguishing.m_Target))
					{
						OnFire onFire = m_OnFireData[fireExtinguishing.m_Target];
						onFire.m_RescueRequest = fireExtinguishing.m_Request;
						m_OnFireData[fireExtinguishing.m_Target] = onFire;
					}
					if (m_RescueTargetData.HasComponent(fireExtinguishing.m_Target))
					{
						RescueTarget rescueTarget = m_RescueTargetData[fireExtinguishing.m_Target];
						rescueTarget.m_Request = fireExtinguishing.m_Request;
						m_RescueTargetData[fireExtinguishing.m_Target] = rescueTarget;
					}
				}
				if (fireExtinguishing.m_FireIntensityDelta != 0f && m_OnFireData.HasComponent(fireExtinguishing.m_Target))
				{
					OnFire onFire2 = m_OnFireData[fireExtinguishing.m_Target];
					onFire2.m_Intensity = math.max(0f, onFire2.m_Intensity + fireExtinguishing.m_FireIntensityDelta);
					m_OnFireData[fireExtinguishing.m_Target] = onFire2;
				}
				if (fireExtinguishing.m_WaterDamageDelta != 0f && m_DamagedData.HasComponent(fireExtinguishing.m_Target))
				{
					Damaged damaged = m_DamagedData[fireExtinguishing.m_Target];
					if (damaged.m_Damage.z < 0.5f)
					{
						damaged.m_Damage.z = math.min(0.5f, damaged.m_Damage.z + fireExtinguishing.m_WaterDamageDelta);
						m_DamagedData[fireExtinguishing.m_Target] = damaged;
					}
				}
				if (fireExtinguishing.m_DestroyedClearDelta != 0f && m_DestroyedData.HasComponent(fireExtinguishing.m_Target))
				{
					Destroyed destroyed = m_DestroyedData[fireExtinguishing.m_Target];
					destroyed.m_Cleared = math.min(1f, destroyed.m_Cleared + fireExtinguishing.m_DestroyedClearDelta);
					m_DestroyedData[fireExtinguishing.m_Target] = destroyed;
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

		public ComponentTypeHandle<Game.Vehicles.FireEngine> __Game_Vehicles_FireEngine_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Aircraft> __Game_Vehicles_Aircraft_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public BufferTypeHandle<AircraftNavigationLane> __Game_Vehicles_AircraftNavigationLane_RW_BufferTypeHandle;

		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HelicopterData> __Game_Prefabs_HelicopterData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<FireEngineData> __Game_Prefabs_FireEngineData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<FireRescueRequest> __Game_Simulation_FireRescueRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OnFire> __Game_Events_OnFire_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RescueTarget> __Game_Buildings_RescueTarget_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.FireStation> __Game_Buildings_FireStation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		public ComponentLookup<Blocker> __Game_Vehicles_Blocker_RW_ComponentLookup;

		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RW_BufferLookup;

		public ComponentLookup<OnFire> __Game_Events_OnFire_RW_ComponentLookup;

		public ComponentLookup<RescueTarget> __Game_Buildings_RescueTarget_RW_ComponentLookup;

		public ComponentLookup<Damaged> __Game_Objects_Damaged_RW_ComponentLookup;

		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Vehicles_FireEngine_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.FireEngine>(false);
			__Game_Vehicles_Aircraft_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Aircraft>(false);
			__Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftCurrentLane>(false);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Vehicles_AircraftNavigationLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AircraftNavigationLane>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Prefabs_HelicopterData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HelicopterData>(true);
			__Game_Prefabs_FireEngineData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FireEngineData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Simulation_FireRescueRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FireRescueRequest>(true);
			__Game_Events_OnFire_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OnFire>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Buildings_RescueTarget_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RescueTarget>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_FireStation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.FireStation>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Vehicles_Blocker_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Blocker>(false);
			__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
			__Game_Events_OnFire_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OnFire>(false);
			__Game_Buildings_RescueTarget_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RescueTarget>(false);
			__Game_Objects_Damaged_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(false);
			__Game_Common_Destroyed_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private EntityQuery m_VehicleQuery;

	private EntityQuery m_ConfigQuery;

	private EntityArchetype m_FireRescueRequestArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_MovingToParkedAircraftRemoveTypes;

	private ComponentTypeSet m_MovingToParkedAddTypes;

	private EventHelpers.StructuralIntegrityData m_StructuralIntegrityData;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 10;
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
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[10]
		{
			ComponentType.ReadWrite<AircraftCurrentLane>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Game.Vehicles.FireEngine>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>(),
			ComponentType.Exclude<OutOfControl>()
		});
		m_ConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<FireConfigurationData>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_FireRescueRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<FireRescueRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_HandleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<HandleRequest>(),
			ComponentType.ReadWrite<Game.Common.Event>()
		});
		m_MovingToParkedAircraftRemoveTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[12]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<AircraftNavigation>(),
			ComponentType.ReadWrite<AircraftNavigationLane>(),
			ComponentType.ReadWrite<AircraftCurrentLane>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<PathElement>(),
			ComponentType.ReadWrite<PathInformation>(),
			ComponentType.ReadWrite<ServiceDispatch>()
		});
		m_MovingToParkedAddTypes = new ComponentTypeSet(ComponentType.ReadWrite<ParkedCar>(), ComponentType.ReadWrite<Stopped>(), ComponentType.ReadWrite<Updated>());
		m_StructuralIntegrityData = new EventHelpers.StructuralIntegrityData((SystemBase)(object)this);
		((ComponentSystemBase)this).RequireForUpdate(m_VehicleQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		FireConfigurationData singleton = ((EntityQuery)(ref m_ConfigQuery)).GetSingleton<FireConfigurationData>();
		NativeQueue<FireExtinguishing> extinguishingQueue = default(NativeQueue<FireExtinguishing>);
		extinguishingQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		m_StructuralIntegrityData.Update((SystemBase)(object)this, singleton);
		JobHandle dependencies;
		FireAircraftTickJob fireAircraftTickJob = new FireAircraftTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FireEngineType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.FireEngine>(ref __TypeHandle.__Game_Vehicles_FireEngine_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftType = InternalCompilerInterface.GetComponentTypeHandle<Aircraft>(ref __TypeHandle.__Game_Vehicles_Aircraft_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<AircraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_AircraftNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatchType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabHelicopterData = InternalCompilerInterface.GetComponentLookup<HelicopterData>(ref __TypeHandle.__Game_Prefabs_HelicopterData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabFireEngineData = InternalCompilerInterface.GetComponentLookup<FireEngineData>(ref __TypeHandle.__Game_Prefabs_FireEngineData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FireRescueRequestData = InternalCompilerInterface.GetComponentLookup<FireRescueRequest>(ref __TypeHandle.__Game_Simulation_FireRescueRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OnFireData = InternalCompilerInterface.GetComponentLookup<OnFire>(ref __TypeHandle.__Game_Events_OnFire_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RescueTargetData = InternalCompilerInterface.GetComponentLookup<RescueTarget>(ref __TypeHandle.__Game_Buildings_RescueTarget_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FireStationData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.FireStation>(ref __TypeHandle.__Game_Buildings_FireStation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerData = InternalCompilerInterface.GetComponentLookup<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FireRescueRequestArchetype = m_FireRescueRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_MovingToParkedAircraftRemoveTypes = m_MovingToParkedAircraftRemoveTypes,
			m_MovingToParkedAddTypes = m_MovingToParkedAddTypes,
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex,
			m_StructuralIntegrityData = m_StructuralIntegrityData,
			m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		fireAircraftTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		fireAircraftTickJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		fireAircraftTickJob.m_ExtinguishingQueue = extinguishingQueue.AsParallelWriter();
		FireAircraftTickJob fireAircraftTickJob2 = fireAircraftTickJob;
		FireExtinguishingJob obj = new FireExtinguishingJob
		{
			m_OnFireData = InternalCompilerInterface.GetComponentLookup<OnFire>(ref __TypeHandle.__Game_Events_OnFire_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RescueTargetData = InternalCompilerInterface.GetComponentLookup<RescueTarget>(ref __TypeHandle.__Game_Buildings_RescueTarget_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtinguishingQueue = extinguishingQueue
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<FireAircraftTickJob>(fireAircraftTickJob2, m_VehicleQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		JobHandle val3 = IJobExtensions.Schedule<FireExtinguishingJob>(obj, val2);
		extinguishingQueue.Dispose(val3);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val2);
		m_PathfindSetupSystem.AddQueueWriter(val2);
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
	public FireAircraftAISystem()
	{
	}
}

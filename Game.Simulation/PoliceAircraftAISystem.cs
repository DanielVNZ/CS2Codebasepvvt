using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Creatures;
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
public class PoliceAircraftAISystem : GameSystemBase
{
	private struct PoliceAction
	{
		public PoliceActionType m_Type;

		public Entity m_Target;

		public Entity m_Request;

		public float m_CrimeReductionRate;
	}

	private enum PoliceActionType
	{
		ReduceCrime,
		AddPatrolRequest,
		SecureAccidentSite
	}

	[BurstCompile]
	private struct PoliceAircraftTickJob : IJobChunk
	{
		private struct FindPointOfInterestIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Circle2 m_Circle;

			public Random m_Random;

			public float3 m_Result;

			public int m_TotalProbability;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<CrimeProducer> m_CrimeProducerData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Circle);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity building)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Circle) || !m_CrimeProducerData.HasComponent(building))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[building];
				float3 val = default(float3);
				((float3)(ref val))._002Ector(0f, 0f, m_PrefabObjectGeometryData[prefabRef.m_Prefab].m_Bounds.max.z);
				val = ObjectUtils.LocalToWorld(m_TransformData[building], val);
				if (math.distance(m_Circle.position, ((float3)(ref val)).xz) < m_Circle.radius)
				{
					int num = 100;
					m_TotalProbability += num;
					if (((Random)(ref m_Random)).NextInt(m_TotalProbability) < num)
					{
						m_Result = val;
					}
				}
			}
		}

		private struct AddRequestIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public Bezier4x2 m_Curve;

			public float m_Distance;

			public Entity m_Request;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<CrimeProducer> m_CrimeProducerData;

			public ParallelWriter<PoliceAction> m_ActionQueue;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity building)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds) && m_CrimeProducerData.HasComponent(building))
				{
					Transform transform = m_TransformData[building];
					float num = default(float);
					if (!(MathUtils.Distance(m_Curve, ((float3)(ref transform.m_Position)).xz, ref num) > m_Distance))
					{
						m_ActionQueue.Enqueue(new PoliceAction
						{
							m_Type = PoliceActionType.AddPatrolRequest,
							m_Target = building,
							m_Request = m_Request
						});
					}
				}
			}
		}

		private struct ReduceCrimeIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public Bezier4x2 m_Curve;

			public float2 m_Distance;

			public float m_Reduction;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<CrimeProducer> m_CrimeProducerData;

			public ParallelWriter<PoliceAction> m_ActionQueue;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity building)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds) && m_CrimeProducerData.HasComponent(building))
				{
					Transform transform = m_TransformData[building];
					float num2 = default(float);
					float num = MathUtils.Distance(m_Curve, ((float3)(ref transform.m_Position)).xz, ref num2);
					if (!(num >= m_Distance.y) && m_CrimeProducerData[building].m_Crime > 0f)
					{
						m_ActionQueue.Enqueue(new PoliceAction
						{
							m_Type = PoliceActionType.ReduceCrime,
							m_Target = building,
							m_CrimeReductionRate = m_Reduction * (1f - math.max(0f, num - m_Distance.x) / (m_Distance.y - m_Distance.x))
						});
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

		[ReadOnly]
		public BufferTypeHandle<Passenger> m_PassengerType;

		public ComponentTypeHandle<Game.Vehicles.PoliceCar> m_PoliceCarType;

		public ComponentTypeHandle<Aircraft> m_AircraftType;

		public ComponentTypeHandle<AircraftCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<Target> m_TargetType;

		public ComponentTypeHandle<PointOfInterest> m_PointOfInterestType;

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
		public ComponentLookup<PoliceCarData> m_PrefabPoliceCarData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		[ReadOnly]
		public ComponentLookup<PolicePatrolRequest> m_PolicePatrolRequestData;

		[ReadOnly]
		public ComponentLookup<PoliceEmergencyRequest> m_PoliceEmergencyRequestData;

		[ReadOnly]
		public ComponentLookup<CrimeProducer> m_CrimeProducerData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.PoliceStation> m_PoliceStationData;

		[ReadOnly]
		public ComponentLookup<AccidentSite> m_AccidentSiteData;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> m_InvolvedInAccidentData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Blocker> m_BlockerData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		[ReadOnly]
		public EntityArchetype m_PolicePatrolRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_PoliceEmergencyRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedAircraftRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedAddTypes;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public ParallelWriter<PoliceAction> m_ActionQueue;

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
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<PathInformation> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
			NativeArray<AircraftCurrentLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftCurrentLane>(ref m_CurrentLaneType);
			NativeArray<Game.Vehicles.PoliceCar> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.PoliceCar>(ref m_PoliceCarType);
			NativeArray<Aircraft> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Aircraft>(ref m_AircraftType);
			NativeArray<Target> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PointOfInterest> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PointOfInterest>(ref m_PointOfInterestType);
			NativeArray<PathOwner> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			BufferAccessor<Passenger> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Passenger>(ref m_PassengerType);
			BufferAccessor<AircraftNavigationLane> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<AircraftNavigationLane>(ref m_AircraftNavigationLaneType);
			BufferAccessor<ServiceDispatch> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Owner owner = nativeArray2[i];
				PrefabRef prefabRef = nativeArray3[i];
				PathInformation pathInformation = nativeArray4[i];
				Game.Vehicles.PoliceCar policeCar = nativeArray6[i];
				Aircraft aircraft = nativeArray7[i];
				AircraftCurrentLane currentLane = nativeArray5[i];
				PathOwner pathOwner = nativeArray10[i];
				Target target = nativeArray8[i];
				PointOfInterest pointOfInterest = nativeArray9[i];
				DynamicBuffer<AircraftNavigationLane> navigationLanes = bufferAccessor2[i];
				DynamicBuffer<ServiceDispatch> serviceDispatches = bufferAccessor3[i];
				DynamicBuffer<Passenger> passengers = default(DynamicBuffer<Passenger>);
				if (bufferAccessor.Length != 0)
				{
					passengers = bufferAccessor[i];
				}
				VehicleUtils.CheckUnspawned(unfilteredChunkIndex, val, currentLane, isUnspawned, m_CommandBuffer);
				Tick(unfilteredChunkIndex, ref random, val, owner, prefabRef, pathInformation, passengers, navigationLanes, serviceDispatches, ref policeCar, ref aircraft, ref currentLane, ref pathOwner, ref target, ref pointOfInterest);
				nativeArray6[i] = policeCar;
				nativeArray7[i] = aircraft;
				nativeArray5[i] = currentLane;
				nativeArray10[i] = pathOwner;
				nativeArray8[i] = target;
				nativeArray9[i] = pointOfInterest;
			}
		}

		private void Tick(int jobIndex, ref Random random, Entity vehicleEntity, Owner owner, PrefabRef prefabRef, PathInformation pathInformation, DynamicBuffer<Passenger> passengers, DynamicBuffer<AircraftNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.PoliceCar policeCar, ref Aircraft aircraft, ref AircraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, ref PointOfInterest pointOfInterest)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			PoliceCarData prefabPoliceCarData = m_PrefabPoliceCarData[prefabRef.m_Prefab];
			policeCar.m_EstimatedShift = math.select(policeCar.m_EstimatedShift - 1, 0u, policeCar.m_EstimatedShift == 0);
			if (++policeCar.m_ShiftTime >= prefabPoliceCarData.m_ShiftDuration)
			{
				policeCar.m_State |= PoliceCarFlags.ShiftEnded;
			}
			if ((currentLane.m_LaneFlags & (AircraftLaneFlags.Flying | AircraftLaneFlags.Landing | AircraftLaneFlags.TakingOff)) == AircraftLaneFlags.Flying)
			{
				UpdatePointOfInterest(vehicleEntity, ref random, ref policeCar, ref aircraft, ref target, ref pointOfInterest);
			}
			else
			{
				pointOfInterest.m_IsValid = false;
				aircraft.m_Flags &= ~AircraftFlags.Working;
			}
			if ((aircraft.m_Flags & AircraftFlags.Emergency) == 0)
			{
				TryReduceCrime(vehicleEntity, prefabPoliceCarData, ref currentLane);
			}
			if (VehicleUtils.ResetUpdatedPath(ref pathOwner))
			{
				ResetPath(jobIndex, vehicleEntity, pathInformation, serviceDispatches, ref policeCar, ref aircraft, ref currentLane);
			}
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
				if (VehicleUtils.IsStuck(pathOwner) || (policeCar.m_State & PoliceCarFlags.Returning) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
					return;
				}
				ReturnToDepot(owner, serviceDispatches, ref policeCar, ref aircraft, ref pathOwner, ref target);
			}
			else if (VehicleUtils.PathEndReached(currentLane) || (policeCar.m_State & (PoliceCarFlags.AtTarget | PoliceCarFlags.Disembarking)) != 0)
			{
				if ((policeCar.m_State & PoliceCarFlags.Returning) != 0)
				{
					if ((policeCar.m_State & PoliceCarFlags.Disembarking) != 0)
					{
						if (StopDisembarking(passengers, ref policeCar))
						{
							if (VehicleUtils.ParkingSpaceReached(currentLane, pathOwner))
							{
								ParkAircraft(jobIndex, vehicleEntity, owner, ref aircraft, ref policeCar, ref currentLane);
							}
							else
							{
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
							}
						}
					}
					else if (!StartDisembarking(passengers, ref policeCar))
					{
						if (VehicleUtils.ParkingSpaceReached(currentLane, pathOwner))
						{
							ParkAircraft(jobIndex, vehicleEntity, owner, ref aircraft, ref policeCar, ref currentLane);
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
						}
					}
					return;
				}
				bool flag = true;
				if ((policeCar.m_State & PoliceCarFlags.AccidentTarget) != 0)
				{
					flag &= SecureAccidentSite(ref policeCar, passengers, serviceDispatches);
				}
				else
				{
					TryReduceCrime(vehicleEntity, prefabPoliceCarData, ref target);
				}
				if (flag)
				{
					CheckServiceDispatches(vehicleEntity, serviceDispatches, passengers, ref policeCar, ref pathOwner);
					if ((policeCar.m_State & (PoliceCarFlags.ShiftEnded | PoliceCarFlags.Disabled)) != 0 || !SelectNextDispatch(jobIndex, vehicleEntity, navigationLanes, serviceDispatches, passengers, ref policeCar, ref aircraft, ref currentLane, ref pathOwner, ref target))
					{
						ReturnToDepot(owner, serviceDispatches, ref policeCar, ref aircraft, ref pathOwner, ref target);
					}
				}
			}
			if (policeCar.m_ShiftTime + policeCar.m_EstimatedShift >= prefabPoliceCarData.m_ShiftDuration)
			{
				policeCar.m_State |= PoliceCarFlags.EstimatedShiftEnd;
			}
			else
			{
				policeCar.m_State &= ~PoliceCarFlags.EstimatedShiftEnd;
			}
			if (passengers.Length >= prefabPoliceCarData.m_CriminalCapacity)
			{
				policeCar.m_State |= PoliceCarFlags.Full;
			}
			else
			{
				policeCar.m_State &= ~PoliceCarFlags.Full;
			}
			if (passengers.Length == 0)
			{
				policeCar.m_State |= PoliceCarFlags.Empty;
			}
			else
			{
				policeCar.m_State &= ~PoliceCarFlags.Empty;
			}
			if ((aircraft.m_Flags & AircraftFlags.Emergency) == 0 && (policeCar.m_State & (PoliceCarFlags.ShiftEnded | PoliceCarFlags.Disabled)) != 0)
			{
				if ((policeCar.m_State & PoliceCarFlags.Returning) == 0)
				{
					ReturnToDepot(owner, serviceDispatches, ref policeCar, ref aircraft, ref pathOwner, ref target);
				}
				serviceDispatches.Clear();
			}
			else
			{
				CheckServiceDispatches(vehicleEntity, serviceDispatches, passengers, ref policeCar, ref pathOwner);
				if ((policeCar.m_State & (PoliceCarFlags.Returning | PoliceCarFlags.Cancelled)) != 0)
				{
					SelectNextDispatch(jobIndex, vehicleEntity, navigationLanes, serviceDispatches, passengers, ref policeCar, ref aircraft, ref currentLane, ref pathOwner, ref target);
				}
				if (policeCar.m_RequestCount <= 1 && (policeCar.m_State & (PoliceCarFlags.ShiftEnded | PoliceCarFlags.Disabled)) == 0)
				{
					RequestTargetIfNeeded(jobIndex, vehicleEntity, ref policeCar);
				}
			}
			if ((policeCar.m_State & (PoliceCarFlags.AtTarget | PoliceCarFlags.Disembarking)) == 0)
			{
				if (VehicleUtils.RequireNewPath(pathOwner))
				{
					FindNewPath(vehicleEntity, prefabRef, ref policeCar, ref aircraft, ref currentLane, ref pathOwner, ref target);
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

		private void ParkAircraft(int jobIndex, Entity entity, Owner owner, ref Aircraft aircraft, ref Game.Vehicles.PoliceCar policeCar, ref AircraftCurrentLane currentLane)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			aircraft.m_Flags &= ~(AircraftFlags.Emergency | AircraftFlags.IgnoreParkedVehicle);
			policeCar.m_State = PoliceCarFlags.Empty;
			Game.Buildings.PoliceStation policeStation = default(Game.Buildings.PoliceStation);
			if (m_PoliceStationData.TryGetComponent(owner.m_Owner, ref policeStation) && (policeStation.m_Flags & PoliceStationFlags.HasAvailablePoliceHelicopters) == 0)
			{
				policeCar.m_State |= PoliceCarFlags.Disabled;
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

		private void UpdatePointOfInterest(Entity entity, ref Random random, ref Game.Vehicles.PoliceCar policeCar, ref Aircraft aircraft, ref Target target, ref PointOfInterest pointOfInterest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[entity];
			Transform transform2 = default(Transform);
			if (m_TransformData.TryGetComponent(target.m_Target, ref transform2))
			{
				if (m_CrimeProducerData.HasComponent(target.m_Target))
				{
					PrefabRef prefabRef = m_PrefabRefData[target.m_Target];
					float3 position = default(float3);
					((float3)(ref position))._002Ector(0f, 0f, m_PrefabObjectGeometryData[prefabRef.m_Prefab].m_Bounds.max.z);
					transform2.m_Position = ObjectUtils.LocalToWorld(transform2, position);
				}
				if (math.distancesq(((float3)(ref transform.m_Position)).xz, ((float3)(ref transform2.m_Position)).xz) < 40000f)
				{
					pointOfInterest.m_Position = transform2.m_Position;
					pointOfInterest.m_IsValid = true;
					aircraft.m_Flags |= AircraftFlags.Working;
					return;
				}
			}
			if ((aircraft.m_Flags & AircraftFlags.Emergency) == 0)
			{
				float3 val = math.forward(transform.m_Rotation);
				val = MathUtils.Normalize(val, ((float3)(ref val)).xz);
				float3 val2 = transform.m_Position + val * 50f;
				if (pointOfInterest.m_IsValid && math.distancesq(((float3)(ref val2)).xz, ((float3)(ref pointOfInterest.m_Position)).xz) < 40000f)
				{
					aircraft.m_Flags |= AircraftFlags.Working;
					return;
				}
				float num = 125f;
				FindPointOfInterestIterator findPointOfInterestIterator = new FindPointOfInterestIterator
				{
					m_Circle = new Circle2(num, ((float3)(ref transform.m_Position)).xz + ((float3)(ref val)).xz * num),
					m_Random = random,
					m_TransformData = m_TransformData,
					m_CrimeProducerData = m_CrimeProducerData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabObjectGeometryData = m_PrefabObjectGeometryData
				};
				m_ObjectSearchTree.Iterate<FindPointOfInterestIterator>(ref findPointOfInterestIterator, 0);
				random = findPointOfInterestIterator.m_Random;
				if (findPointOfInterestIterator.m_TotalProbability != 0)
				{
					pointOfInterest.m_Position = findPointOfInterestIterator.m_Result;
					pointOfInterest.m_IsValid = true;
					aircraft.m_Flags |= AircraftFlags.Working;
					return;
				}
			}
			pointOfInterest.m_IsValid = false;
			aircraft.m_Flags &= ~AircraftFlags.Working;
		}

		private bool StartDisembarking(DynamicBuffer<Passenger> passengers, ref Game.Vehicles.PoliceCar policeCar)
		{
			if (passengers.IsCreated && passengers.Length > 0)
			{
				policeCar.m_State |= PoliceCarFlags.Disembarking;
				return true;
			}
			return false;
		}

		private bool StopDisembarking(DynamicBuffer<Passenger> passengers, ref Game.Vehicles.PoliceCar policeCar)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (passengers.IsCreated)
			{
				for (int i = 0; i < passengers.Length; i++)
				{
					Entity passenger = passengers[i].m_Passenger;
					if (m_CurrentVehicleData.HasComponent(passenger) && (m_CurrentVehicleData[passenger].m_Flags & CreatureVehicleFlags.Ready) == 0)
					{
						return false;
					}
				}
			}
			policeCar.m_State &= ~PoliceCarFlags.Disembarking;
			return true;
		}

		private void FindNewPath(Entity vehicleEntity, PrefabRef prefabRef, ref Game.Vehicles.PoliceCar policeCar, ref Aircraft aircraft, ref AircraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			HelicopterData helicopterData = m_PrefabHelicopterData[prefabRef.m_Prefab];
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(helicopterData.m_FlyingMaxSpeed),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Methods = (PathMethod.Road | PathMethod.Flying),
				m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
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
			if ((policeCar.m_State & PoliceCarFlags.AccidentTarget) != 0)
			{
				destination.m_Type = SetupTargetType.AccidentLocation;
				destination.m_Value2 = 30f;
				destination.m_Methods = PathMethod.Flying;
				destination.m_FlyingTypes = RoadTypes.Helicopter;
			}
			else if ((policeCar.m_State & PoliceCarFlags.Returning) == 0)
			{
				destination.m_Methods = PathMethod.Flying;
				destination.m_FlyingTypes = RoadTypes.Helicopter;
			}
			else
			{
				destination.m_Methods = PathMethod.Road;
				destination.m_RoadTypes = RoadTypes.Helicopter;
			}
			if ((policeCar.m_State & PoliceCarFlags.Returning) == 0)
			{
				parameters.m_Weights = new PathfindWeights(1f, 0f, 0f, 0f);
			}
			else
			{
				parameters.m_Weights = new PathfindWeights(1f, 1f, 1f, 1f);
				destination.m_RandomCost = 30f;
			}
			VehicleUtils.SetupPathfind(item: new SetupQueueItem(vehicleEntity, parameters, origin, destination), currentLane: ref currentLane, pathOwner: ref pathOwner, queue: m_PathfindQueue);
		}

		private bool SecureAccidentSite(ref Game.Vehicles.PoliceCar policeCar, DynamicBuffer<Passenger> passengers, DynamicBuffer<ServiceDispatch> serviceDispatches)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			if (policeCar.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				if (m_PoliceEmergencyRequestData.HasComponent(request))
				{
					PoliceEmergencyRequest policeEmergencyRequest = m_PoliceEmergencyRequestData[request];
					if (m_AccidentSiteData.HasComponent(policeEmergencyRequest.m_Site))
					{
						policeCar.m_State |= PoliceCarFlags.AtTarget;
						if ((m_AccidentSiteData[policeEmergencyRequest.m_Site].m_Flags & AccidentSiteFlags.Secured) == 0)
						{
							m_ActionQueue.Enqueue(new PoliceAction
							{
								m_Type = PoliceActionType.SecureAccidentSite,
								m_Target = policeEmergencyRequest.m_Site
							});
						}
						return false;
					}
				}
			}
			if (passengers.IsCreated)
			{
				for (int i = 0; i < passengers.Length; i++)
				{
					Entity passenger = passengers[i].m_Passenger;
					if (m_CurrentVehicleData.HasComponent(passenger) && (m_CurrentVehicleData[passenger].m_Flags & CreatureVehicleFlags.Ready) == 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void CheckServiceDispatches(Entity vehicleEntity, DynamicBuffer<ServiceDispatch> serviceDispatches, DynamicBuffer<Passenger> passengers, ref Game.Vehicles.PoliceCar policeCar, ref PathOwner pathOwner)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			if (serviceDispatches.Length <= policeCar.m_RequestCount)
			{
				return;
			}
			float num = -1f;
			Entity val = Entity.Null;
			PathElement pathElement = default(PathElement);
			PathElement pathElement2 = default(PathElement);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			int num2 = 0;
			if (policeCar.m_RequestCount >= 1 && (policeCar.m_State & PoliceCarFlags.Returning) == 0)
			{
				DynamicBuffer<PathElement> val2 = m_PathElements[vehicleEntity];
				num2 = 1;
				if (pathOwner.m_ElementIndex < val2.Length)
				{
					pathElement = val2[val2.Length - 1];
					flag = true;
					if (m_PoliceEmergencyRequestData.HasComponent(serviceDispatches[0].m_Request))
					{
						pathElement2 = pathElement;
						flag2 = true;
					}
				}
			}
			DynamicBuffer<PathElement> val3 = default(DynamicBuffer<PathElement>);
			for (int i = num2; i < policeCar.m_RequestCount; i++)
			{
				Entity request = serviceDispatches[i].m_Request;
				if (m_PathElements.TryGetBuffer(request, ref val3) && val3.Length != 0)
				{
					pathElement = val3[val3.Length - 1];
					flag = true;
					if (m_PoliceEmergencyRequestData.HasComponent(request))
					{
						pathElement2 = pathElement;
						flag2 = true;
					}
				}
			}
			DynamicBuffer<PathElement> val4 = default(DynamicBuffer<PathElement>);
			DynamicBuffer<PathElement> val5 = default(DynamicBuffer<PathElement>);
			for (int j = policeCar.m_RequestCount; j < serviceDispatches.Length; j++)
			{
				Entity request2 = serviceDispatches[j].m_Request;
				if (m_PolicePatrolRequestData.HasComponent(request2))
				{
					if (passengers.IsCreated && passengers.Length != 0)
					{
						continue;
					}
					PolicePatrolRequest policePatrolRequest = m_PolicePatrolRequestData[request2];
					if (flag && m_PathElements.TryGetBuffer(request2, ref val4) && val4.Length != 0)
					{
						PathElement pathElement3 = val4[0];
						if (pathElement3.m_Target != pathElement.m_Target || pathElement3.m_TargetDelta.x != pathElement.m_TargetDelta.y)
						{
							continue;
						}
					}
					if (((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(policePatrolRequest.m_Target) && !flag3 && policePatrolRequest.m_Priority > num)
					{
						num = policePatrolRequest.m_Priority;
						val = request2;
					}
				}
				else
				{
					if (!m_PoliceEmergencyRequestData.HasComponent(request2))
					{
						continue;
					}
					PoliceEmergencyRequest policeEmergencyRequest = m_PoliceEmergencyRequestData[request2];
					if (flag2 && m_PathElements.TryGetBuffer(request2, ref val5) && val5.Length != 0)
					{
						PathElement pathElement4 = val5[0];
						if (pathElement4.m_Target != pathElement2.m_Target || pathElement4.m_TargetDelta.x != pathElement2.m_TargetDelta.y)
						{
							continue;
						}
					}
					if (((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(policeEmergencyRequest.m_Site) && (!flag3 || policeEmergencyRequest.m_Priority > num))
					{
						num = policeEmergencyRequest.m_Priority;
						val = request2;
						flag3 = true;
					}
				}
			}
			if (flag3)
			{
				int num3 = 0;
				PathInformation pathInformation = default(PathInformation);
				for (int k = 0; k < policeCar.m_RequestCount; k++)
				{
					ServiceDispatch serviceDispatch = serviceDispatches[k];
					if (m_PoliceEmergencyRequestData.HasComponent(serviceDispatch.m_Request))
					{
						serviceDispatches[num3++] = serviceDispatch;
					}
					else if (k == 0 && (policeCar.m_State & PoliceCarFlags.Returning) == 0)
					{
						serviceDispatches[num3++] = serviceDispatch;
						policeCar.m_State |= PoliceCarFlags.Cancelled;
						if (m_PathInformationData.TryGetComponent(serviceDispatch.m_Request, ref pathInformation))
						{
							uint num4 = (uint)Mathf.RoundToInt(pathInformation.m_Duration * 3.75f);
							policeCar.m_EstimatedShift = math.select(policeCar.m_EstimatedShift - num4, 0u, num4 >= policeCar.m_EstimatedShift);
						}
					}
				}
				if (num3 < policeCar.m_RequestCount)
				{
					serviceDispatches.RemoveRange(num3, policeCar.m_RequestCount - num3);
					policeCar.m_RequestCount = num3;
				}
			}
			if (val != Entity.Null)
			{
				serviceDispatches[policeCar.m_RequestCount++] = new ServiceDispatch(val);
				PathInformation pathInformation2 = default(PathInformation);
				if (m_PathInformationData.TryGetComponent(val, ref pathInformation2))
				{
					policeCar.m_EstimatedShift += (uint)Mathf.RoundToInt(pathInformation2.m_Duration * 3.75f);
				}
			}
			if (serviceDispatches.Length > policeCar.m_RequestCount)
			{
				serviceDispatches.RemoveRange(policeCar.m_RequestCount, serviceDispatches.Length - policeCar.m_RequestCount);
			}
		}

		private void RequestTargetIfNeeded(int jobIndex, Entity entity, ref Game.Vehicles.PoliceCar policeCar)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			if (!m_ServiceRequestData.HasComponent(policeCar.m_TargetRequest) && (policeCar.m_PurposeMask & PolicePurpose.Patrol) != 0 && (policeCar.m_State & (PoliceCarFlags.Empty | PoliceCarFlags.EstimatedShiftEnd)) == PoliceCarFlags.Empty)
			{
				uint num = math.max(512u, 16u);
				if ((m_SimulationFrameIndex & (num - 1)) == 10)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_PolicePatrolRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ServiceRequest>(jobIndex, val, new ServiceRequest(reversed: true));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PolicePatrolRequest>(jobIndex, val, new PolicePatrolRequest(entity, 1f));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(32u));
				}
			}
		}

		private bool SelectNextDispatch(int jobIndex, Entity vehicleEntity, DynamicBuffer<AircraftNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, DynamicBuffer<Passenger> passengers, ref Game.Vehicles.PoliceCar policeCar, ref Aircraft aircraft, ref AircraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			if ((policeCar.m_State & PoliceCarFlags.Returning) == 0 && policeCar.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				serviceDispatches.RemoveAt(0);
				policeCar.m_RequestCount--;
			}
			while (policeCar.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				Entity val = Entity.Null;
				PoliceCarFlags policeCarFlags = (PoliceCarFlags)0u;
				if (m_PolicePatrolRequestData.HasComponent(request))
				{
					if (!passengers.IsCreated || passengers.Length == 0)
					{
						val = m_PolicePatrolRequestData[request].m_Target;
					}
				}
				else if (m_PoliceEmergencyRequestData.HasComponent(request))
				{
					val = m_PoliceEmergencyRequestData[request].m_Site;
					policeCarFlags |= PoliceCarFlags.AccidentTarget;
				}
				if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(val))
				{
					serviceDispatches.RemoveAt(0);
					policeCar.m_EstimatedShift -= policeCar.m_EstimatedShift / (uint)policeCar.m_RequestCount;
					policeCar.m_RequestCount--;
					continue;
				}
				aircraft.m_Flags &= ~AircraftFlags.IgnoreParkedVehicle;
				policeCar.m_State &= ~(PoliceCarFlags.Returning | PoliceCarFlags.AccidentTarget | PoliceCarFlags.AtTarget | PoliceCarFlags.Cancelled);
				policeCar.m_State |= policeCarFlags;
				Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(request, vehicleEntity, completed: false, pathConsumed: true));
				if (m_ServiceRequestData.HasComponent(policeCar.m_TargetRequest))
				{
					val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val2, new HandleRequest(policeCar.m_TargetRequest, Entity.Null, completed: true));
				}
				if (m_PathElements.HasBuffer(request))
				{
					DynamicBuffer<PathElement> appendPath = m_PathElements[request];
					if (appendPath.Length != 0)
					{
						DynamicBuffer<PathElement> val3 = m_PathElements[vehicleEntity];
						PathUtils.TrimPath(val3, ref pathOwner);
						float num = policeCar.m_PathElementTime * (float)val3.Length + m_PathInformationData[request].m_Duration;
						if (PathUtils.TryAppendPath(ref currentLane, navigationLanes, val3, appendPath))
						{
							if ((policeCarFlags & PoliceCarFlags.AccidentTarget) != 0)
							{
								aircraft.m_Flags |= AircraftFlags.Emergency | AircraftFlags.StayMidAir;
							}
							else
							{
								for (int i = 0; i < val3.Length; i++)
								{
									PathElement pathElement = val3[i];
									if (m_ConnectionLaneData.HasComponent(pathElement.m_Target) && (m_ConnectionLaneData[pathElement.m_Target].m_Flags & (ConnectionLaneFlags.Outside | ConnectionLaneFlags.Airway)) == ConnectionLaneFlags.Airway)
									{
										AddPatrolRequests(pathElement.m_Target, request);
									}
								}
								aircraft.m_Flags &= ~AircraftFlags.Emergency;
								aircraft.m_Flags |= AircraftFlags.StayMidAir;
							}
							if (policeCar.m_RequestCount == 1)
							{
								policeCar.m_EstimatedShift = (uint)Mathf.RoundToInt(num * 3.75f);
							}
							policeCar.m_PathElementTime = num / (float)math.max(1, val3.Length);
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

		private void ReturnToDepot(Owner owner, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.PoliceCar policeCar, ref Aircraft aircraft, ref PathOwner pathOwner, ref Target target)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			serviceDispatches.Clear();
			policeCar.m_RequestCount = 0;
			policeCar.m_EstimatedShift = 0u;
			policeCar.m_State &= ~(PoliceCarFlags.AccidentTarget | PoliceCarFlags.AtTarget | PoliceCarFlags.Cancelled);
			policeCar.m_State |= PoliceCarFlags.Returning;
			aircraft.m_Flags &= ~(AircraftFlags.Emergency | AircraftFlags.IgnoreParkedVehicle);
			VehicleUtils.SetTarget(ref pathOwner, ref target, owner.m_Owner);
		}

		private void ResetPath(int jobIndex, Entity vehicleEntity, PathInformation pathInformation, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.PoliceCar policeCar, ref Aircraft aircraft, ref AircraftCurrentLane currentLane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PathElement> path = m_PathElements[vehicleEntity];
			PathUtils.ResetPath(ref currentLane, path);
			if ((policeCar.m_State & PoliceCarFlags.Returning) == 0 && policeCar.m_RequestCount > 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				if (m_PolicePatrolRequestData.HasComponent(request))
				{
					for (int i = 0; i < path.Length; i++)
					{
						PathElement pathElement = path[i];
						if (m_ConnectionLaneData.HasComponent(pathElement.m_Target) && (m_ConnectionLaneData[pathElement.m_Target].m_Flags & (ConnectionLaneFlags.Outside | ConnectionLaneFlags.Airway)) == ConnectionLaneFlags.Airway)
						{
							AddPatrolRequests(pathElement.m_Target, request);
						}
					}
					aircraft.m_Flags &= ~AircraftFlags.Emergency;
					aircraft.m_Flags |= AircraftFlags.StayMidAir;
				}
				else if (m_PoliceEmergencyRequestData.HasComponent(request))
				{
					aircraft.m_Flags |= AircraftFlags.Emergency | AircraftFlags.StayMidAir;
				}
				else
				{
					aircraft.m_Flags &= ~AircraftFlags.Emergency;
					aircraft.m_Flags |= AircraftFlags.StayMidAir;
				}
				if (policeCar.m_RequestCount == 1)
				{
					policeCar.m_EstimatedShift = (uint)Mathf.RoundToInt(pathInformation.m_Duration * 3.75f);
				}
			}
			else
			{
				aircraft.m_Flags &= ~(AircraftFlags.StayOnTaxiway | AircraftFlags.StayMidAir);
			}
			policeCar.m_PathElementTime = pathInformation.m_Duration / (float)math.max(1, path.Length);
		}

		private void AddPatrolRequests(Entity laneEntity, Entity request)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			Curve curve = m_CurveData[laneEntity];
			AddRequestIterator addRequestIterator = new AddRequestIterator
			{
				m_Bounds = MathUtils.Expand(MathUtils.Bounds(curve.m_Bezier), float3.op_Implicit(300f)),
				m_Curve = ((Bezier4x3)(ref curve.m_Bezier)).xz,
				m_Distance = 300f,
				m_Request = request,
				m_TransformData = m_TransformData,
				m_CrimeProducerData = m_CrimeProducerData,
				m_ActionQueue = m_ActionQueue
			};
			m_ObjectSearchTree.Iterate<AddRequestIterator>(ref addRequestIterator, 0);
		}

		private void TryReduceCrime(Entity vehicleEntity, PoliceCarData prefabPoliceCarData, ref AircraftCurrentLane currentLane)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (m_ConnectionLaneData.HasComponent(currentLane.m_Lane) && (m_ConnectionLaneData[currentLane.m_Lane].m_Flags & (ConnectionLaneFlags.Outside | ConnectionLaneFlags.Airway)) == ConnectionLaneFlags.Airway && (currentLane.m_LaneFlags & AircraftLaneFlags.Checked) == 0)
			{
				currentLane.m_LaneFlags |= AircraftLaneFlags.Checked;
				ReduceCrime(currentLane.m_Lane, prefabPoliceCarData.m_CrimeReductionRate);
			}
		}

		private void TryReduceCrime(Entity vehicleEntity, PoliceCarData prefabPoliceCarData, ref Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			if (m_CrimeProducerData.HasComponent(target.m_Target) && m_CrimeProducerData[target.m_Target].m_Crime > 0f)
			{
				m_ActionQueue.Enqueue(new PoliceAction
				{
					m_Type = PoliceActionType.ReduceCrime,
					m_Target = target.m_Target,
					m_CrimeReductionRate = prefabPoliceCarData.m_CrimeReductionRate
				});
			}
		}

		private void ReduceCrime(Entity laneEntity, float reduction)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			Curve curve = m_CurveData[laneEntity];
			ReduceCrimeIterator reduceCrimeIterator = new ReduceCrimeIterator
			{
				m_Bounds = MathUtils.Expand(MathUtils.Bounds(curve.m_Bezier), float3.op_Implicit(450.00003f)),
				m_Curve = ((Bezier4x3)(ref curve.m_Bezier)).xz,
				m_Distance = 750f * new float2(0.2f, 0.6f),
				m_Reduction = reduction,
				m_TransformData = m_TransformData,
				m_CrimeProducerData = m_CrimeProducerData,
				m_ActionQueue = m_ActionQueue
			};
			m_ObjectSearchTree.Iterate<ReduceCrimeIterator>(ref reduceCrimeIterator, 0);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct PoliceActionJob : IJob
	{
		[ReadOnly]
		public uint m_SimulationFrame;

		public ComponentLookup<CrimeProducer> m_CrimeProducerData;

		public ComponentLookup<AccidentSite> m_AccidentSiteData;

		public NativeQueue<PoliceAction> m_ActionQueue;

		public void Execute()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			PoliceAction policeAction = default(PoliceAction);
			while (m_ActionQueue.TryDequeue(ref policeAction))
			{
				switch (policeAction.m_Type)
				{
				case PoliceActionType.ReduceCrime:
				{
					CrimeProducer crimeProducer = m_CrimeProducerData[policeAction.m_Target];
					float num = math.min(policeAction.m_CrimeReductionRate, crimeProducer.m_Crime);
					if (num > 0f)
					{
						crimeProducer.m_Crime -= num;
						m_CrimeProducerData[policeAction.m_Target] = crimeProducer;
					}
					break;
				}
				case PoliceActionType.AddPatrolRequest:
				{
					CrimeProducer crimeProducer2 = m_CrimeProducerData[policeAction.m_Target];
					crimeProducer2.m_PatrolRequest = policeAction.m_Request;
					m_CrimeProducerData[policeAction.m_Target] = crimeProducer2;
					break;
				}
				case PoliceActionType.SecureAccidentSite:
				{
					AccidentSite accidentSite = m_AccidentSiteData[policeAction.m_Target];
					if ((accidentSite.m_Flags & AccidentSiteFlags.Secured) == 0)
					{
						accidentSite.m_Flags |= AccidentSiteFlags.Secured;
						accidentSite.m_SecuredFrame = m_SimulationFrame;
					}
					m_AccidentSiteData[policeAction.m_Target] = accidentSite;
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

		[ReadOnly]
		public BufferTypeHandle<Passenger> __Game_Vehicles_Passenger_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Vehicles.PoliceCar> __Game_Vehicles_PoliceCar_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Aircraft> __Game_Vehicles_Aircraft_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PointOfInterest> __Game_Common_PointOfInterest_RW_ComponentTypeHandle;

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
		public ComponentLookup<PoliceCarData> __Game_Prefabs_PoliceCarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PolicePatrolRequest> __Game_Simulation_PolicePatrolRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PoliceEmergencyRequest> __Game_Simulation_PoliceEmergencyRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CrimeProducer> __Game_Buildings_CrimeProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.PoliceStation> __Game_Buildings_PoliceStation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AccidentSite> __Game_Events_AccidentSite_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> __Game_Events_InvolvedInAccident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		public ComponentLookup<Blocker> __Game_Vehicles_Blocker_RW_ComponentLookup;

		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RW_BufferLookup;

		public ComponentLookup<CrimeProducer> __Game_Buildings_CrimeProducer_RW_ComponentLookup;

		public ComponentLookup<AccidentSite> __Game_Events_AccidentSite_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Vehicles_Passenger_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Passenger>(true);
			__Game_Vehicles_PoliceCar_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.PoliceCar>(false);
			__Game_Vehicles_Aircraft_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Aircraft>(false);
			__Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftCurrentLane>(false);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Common_PointOfInterest_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PointOfInterest>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Vehicles_AircraftNavigationLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AircraftNavigationLane>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Prefabs_HelicopterData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HelicopterData>(true);
			__Game_Prefabs_PoliceCarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PoliceCarData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Simulation_ServiceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(true);
			__Game_Simulation_PolicePatrolRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PolicePatrolRequest>(true);
			__Game_Simulation_PoliceEmergencyRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PoliceEmergencyRequest>(true);
			__Game_Buildings_CrimeProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CrimeProducer>(true);
			__Game_Buildings_PoliceStation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.PoliceStation>(true);
			__Game_Events_AccidentSite_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccidentSite>(true);
			__Game_Events_InvolvedInAccident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InvolvedInAccident>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Vehicles_Blocker_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Blocker>(false);
			__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
			__Game_Buildings_CrimeProducer_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CrimeProducer>(false);
			__Game_Events_AccidentSite_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccidentSite>(false);
		}
	}

	private const float MAX_WORK_DISTANCE = 200f;

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private EntityQuery m_VehicleQuery;

	private EntityArchetype m_PolicePatrolRequestArchetype;

	private EntityArchetype m_PoliceEmergencyRequestArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_MovingToParkedAircraftRemoveTypes;

	private ComponentTypeSet m_MovingToParkedAddTypes;

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
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
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
			ComponentType.ReadWrite<Game.Vehicles.PoliceCar>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>(),
			ComponentType.Exclude<OutOfControl>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_PolicePatrolRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<PolicePatrolRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_PoliceEmergencyRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<PoliceEmergencyRequest>(),
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
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<PoliceAction> actionQueue = default(NativeQueue<PoliceAction>);
		actionQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		PoliceAircraftTickJob policeAircraftTickJob = new PoliceAircraftTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PassengerType = InternalCompilerInterface.GetBufferTypeHandle<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceCarType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.PoliceCar>(ref __TypeHandle.__Game_Vehicles_PoliceCar_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftType = InternalCompilerInterface.GetComponentTypeHandle<Aircraft>(ref __TypeHandle.__Game_Vehicles_Aircraft_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PointOfInterestType = InternalCompilerInterface.GetComponentTypeHandle<PointOfInterest>(ref __TypeHandle.__Game_Common_PointOfInterest_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<AircraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_AircraftNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatchType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabHelicopterData = InternalCompilerInterface.GetComponentLookup<HelicopterData>(ref __TypeHandle.__Game_Prefabs_HelicopterData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPoliceCarData = InternalCompilerInterface.GetComponentLookup<PoliceCarData>(ref __TypeHandle.__Game_Prefabs_PoliceCarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PolicePatrolRequestData = InternalCompilerInterface.GetComponentLookup<PolicePatrolRequest>(ref __TypeHandle.__Game_Simulation_PolicePatrolRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceEmergencyRequestData = InternalCompilerInterface.GetComponentLookup<PoliceEmergencyRequest>(ref __TypeHandle.__Game_Simulation_PoliceEmergencyRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CrimeProducerData = InternalCompilerInterface.GetComponentLookup<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceStationData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.PoliceStation>(ref __TypeHandle.__Game_Buildings_PoliceStation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AccidentSiteData = InternalCompilerInterface.GetComponentLookup<AccidentSite>(ref __TypeHandle.__Game_Events_AccidentSite_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InvolvedInAccidentData = InternalCompilerInterface.GetComponentLookup<InvolvedInAccident>(ref __TypeHandle.__Game_Events_InvolvedInAccident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerData = InternalCompilerInterface.GetComponentLookup<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex,
			m_PolicePatrolRequestArchetype = m_PolicePatrolRequestArchetype,
			m_PoliceEmergencyRequestArchetype = m_PoliceEmergencyRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_MovingToParkedAircraftRemoveTypes = m_MovingToParkedAircraftRemoveTypes,
			m_MovingToParkedAddTypes = m_MovingToParkedAddTypes,
			m_RandomSeed = RandomSeed.Next(),
			m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		policeAircraftTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		policeAircraftTickJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		policeAircraftTickJob.m_ActionQueue = actionQueue.AsParallelWriter();
		PoliceAircraftTickJob policeAircraftTickJob2 = policeAircraftTickJob;
		PoliceActionJob obj = new PoliceActionJob
		{
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_CrimeProducerData = InternalCompilerInterface.GetComponentLookup<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AccidentSiteData = InternalCompilerInterface.GetComponentLookup<AccidentSite>(ref __TypeHandle.__Game_Events_AccidentSite_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActionQueue = actionQueue
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<PoliceAircraftTickJob>(policeAircraftTickJob2, m_VehicleQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		JobHandle val3 = IJobExtensions.Schedule<PoliceActionJob>(obj, val2);
		actionQueue.Dispose(val3);
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
	public PoliceAircraftAISystem()
	{
	}
}

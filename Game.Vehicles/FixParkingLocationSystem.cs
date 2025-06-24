using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Vehicles;

[CompilerGenerated]
public class FixParkingLocationSystem : GameSystemBase
{
	[BurstCompile]
	private struct CollectParkedCarsJob : IJobChunk
	{
		private struct AddVehiclesIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Entity m_Lane;

			public Bounds3 m_Bounds;

			public ParallelWriter<Entity> m_VehicleQueue;

			public ComponentLookup<ParkedCar> m_ParkedCarData;

			public ComponentLookup<Controller> m_ControllerData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds))
				{
					Controller controller = default(Controller);
					if (m_ControllerData.TryGetComponent(entity, ref controller))
					{
						entity = controller.m_Controller;
					}
					ParkedCar parkedCar = default(ParkedCar);
					if (m_ParkedCarData.TryGetComponent(entity, ref parkedCar) && parkedCar.m_Lane == m_Lane)
					{
						m_VehicleQueue.Enqueue(entity);
					}
				}
			}
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<FixParkingLocation> m_FixParkingLocationType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.ConnectionLane> m_ConnectionLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.SpawnLocation> m_SpawnLocationType;

		[ReadOnly]
		public ComponentTypeHandle<MovedLocation> m_MovedLocationType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public BufferTypeHandle<LaneObject> m_LaneObjectType;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_ActivityLocations;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingObjectSearchTree;

		public ParallelWriter<Entity> m_VehicleQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<FixParkingLocation>(ref m_FixParkingLocationType))
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					m_VehicleQueue.Enqueue(nativeArray[i]);
				}
				return;
			}
			BufferAccessor<LaneObject> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LaneObject>(ref m_LaneObjectType);
			if (bufferAccessor.Length != 0)
			{
				Controller controller = default(Controller);
				for (int j = 0; j < bufferAccessor.Length; j++)
				{
					DynamicBuffer<LaneObject> val = bufferAccessor[j];
					int num = 0;
					for (int k = 0; k < val.Length; k++)
					{
						LaneObject laneObject = val[k];
						if (m_ParkedCarData.HasComponent(laneObject.m_LaneObject))
						{
							Entity val2 = laneObject.m_LaneObject;
							if (m_ControllerData.TryGetComponent(laneObject.m_LaneObject, ref controller))
							{
								val2 = controller.m_Controller;
							}
							m_VehicleQueue.Enqueue(val2);
						}
						else
						{
							val[num++] = laneObject;
						}
					}
					if (num != 0)
					{
						if (num < val.Length)
						{
							val.RemoveRange(num, val.Length - num);
						}
					}
					else
					{
						val.Clear();
					}
				}
				return;
			}
			NativeArray<Game.Net.ConnectionLane> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.ConnectionLane>(ref m_ConnectionLaneType);
			if (nativeArray2.Length != 0)
			{
				NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<Curve> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
				NativeArray<Owner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				for (int l = 0; l < nativeArray2.Length; l++)
				{
					Game.Net.ConnectionLane connectionLane = nativeArray2[l];
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Parking) != 0)
					{
						Entity entity = nativeArray3[l];
						Curve curve = nativeArray4[l];
						Owner owner = nativeArray5[l];
						AddVehicles(entity, owner, curve, connectionLane);
					}
				}
				return;
			}
			NativeArray<Transform> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			if (nativeArray6.Length == 0)
			{
				return;
			}
			NativeArray<Entity> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Objects.SpawnLocation> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Objects.SpawnLocation>(ref m_SpawnLocationType);
			NativeArray<MovedLocation> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MovedLocation>(ref m_MovedLocationType);
			NativeArray<PrefabRef> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			SpawnLocationData spawnLocationData = default(SpawnLocationData);
			MovedLocation movedLocation = default(MovedLocation);
			for (int m = 0; m < nativeArray6.Length; m++)
			{
				PrefabRef prefabRef = nativeArray10[m];
				if (m_PrefabSpawnLocationData.TryGetComponent(prefabRef.m_Prefab, ref spawnLocationData) && (((spawnLocationData.m_RoadTypes & RoadTypes.Helicopter) != RoadTypes.None && spawnLocationData.m_ConnectionType == RouteConnectionType.Air) || spawnLocationData.m_ConnectionType == RouteConnectionType.Track))
				{
					Entity entity2 = nativeArray7[m];
					Transform transform = nativeArray6[m];
					Game.Objects.SpawnLocation spawnLocation = nativeArray8[m];
					if (CollectionUtils.TryGet<MovedLocation>(nativeArray9, m, ref movedLocation))
					{
						transform.m_Position = movedLocation.m_OldPosition;
					}
					AddVehicles(entity2, transform, spawnLocation, spawnLocationData);
				}
			}
		}

		private void AddVehicles(Entity entity, Owner owner, Curve curve, Game.Net.ConnectionLane connectionLane)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			AddVehiclesIterator addVehiclesIterator = new AddVehiclesIterator
			{
				m_Lane = entity,
				m_Bounds = VehicleUtils.GetConnectionParkingBounds(connectionLane, curve.m_Bezier),
				m_VehicleQueue = m_VehicleQueue,
				m_ParkedCarData = m_ParkedCarData,
				m_ControllerData = m_ControllerData
			};
			Owner owner2 = owner;
			while (m_OwnerData.HasComponent(owner2.m_Owner))
			{
				owner2 = m_OwnerData[owner2.m_Owner];
			}
			if (m_BuildingData.HasComponent(owner2.m_Owner))
			{
				PrefabRef prefabRef = m_PrefabRefData[owner2.m_Owner];
				DynamicBuffer<ActivityLocationElement> val = default(DynamicBuffer<ActivityLocationElement>);
				if (m_ActivityLocations.TryGetBuffer(prefabRef.m_Prefab, ref val))
				{
					Transform transform = m_TransformData[owner2.m_Owner];
					ActivityMask activityMask = new ActivityMask(ActivityType.GarageSpot);
					for (int i = 0; i < val.Length; i++)
					{
						ActivityLocationElement activityLocationElement = val[i];
						if ((activityLocationElement.m_ActivityMask.m_Mask & activityMask.m_Mask) != 0)
						{
							float3 val2 = ObjectUtils.LocalToWorld(transform, activityLocationElement.m_Position);
							addVehiclesIterator.m_Bounds.min = math.min(addVehiclesIterator.m_Bounds.min, val2 - 1f);
							addVehiclesIterator.m_Bounds.max = math.max(addVehiclesIterator.m_Bounds.max, val2 + 1f);
						}
					}
				}
			}
			m_MovingObjectSearchTree.Iterate<AddVehiclesIterator>(ref addVehiclesIterator, 0);
		}

		private void AddVehicles(Entity entity, Transform transform, Game.Objects.SpawnLocation spawnLocation, SpawnLocationData spawnLocationData)
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			switch (spawnLocationData.m_ConnectionType)
			{
			case RouteConnectionType.Air:
			{
				AddVehiclesIterator addVehiclesIterator = new AddVehiclesIterator
				{
					m_Lane = entity,
					m_Bounds = new Bounds3(transform.m_Position - 1f, transform.m_Position + 1f),
					m_VehicleQueue = m_VehicleQueue,
					m_ParkedCarData = m_ParkedCarData,
					m_ControllerData = m_ControllerData
				};
				m_MovingObjectSearchTree.Iterate<AddVehiclesIterator>(ref addVehiclesIterator, 0);
				break;
			}
			case RouteConnectionType.Track:
			{
				DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
				if (!m_LaneObjects.TryGetBuffer(spawnLocation.m_ConnectedLane1, ref val))
				{
					break;
				}
				Controller controller = default(Controller);
				ParkedTrain parkedTrain = default(ParkedTrain);
				for (int i = 0; i < val.Length; i++)
				{
					LaneObject laneObject = val[i];
					if (m_ControllerData.TryGetComponent(laneObject.m_LaneObject, ref controller))
					{
						laneObject.m_LaneObject = controller.m_Controller;
					}
					if (m_ParkedTrainData.TryGetComponent(laneObject.m_LaneObject, ref parkedTrain) && parkedTrain.m_ParkingLocation == entity)
					{
						m_VehicleQueue.Enqueue(laneObject.m_LaneObject);
					}
				}
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
	private struct FixParkingLocationJob : IJob
	{
		private struct SpotData : IComparable<SpotData>
		{
			public float3 m_Position;

			public float m_Order;

			public int m_Index;

			public bool m_Occupied;

			public int CompareTo(SpotData other)
			{
				return math.select(0, math.select(-1, 1, m_Order > other.m_Order), m_Order != other.m_Order);
			}
		}

		private struct OccupySpotsIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Entity m_Lane;

			public Entity m_Ignore;

			public Bounds3 m_Bounds;

			public int m_Order;

			public NativeList<SpotData> m_Spots;

			public ComponentLookup<ParkedCar> m_ParkedCarData;

			public ComponentLookup<Transform> m_TransformData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				ParkedCar parkedCar = default(ParkedCar);
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_ParkedCarData.TryGetComponent(entity, ref parkedCar) || !(entity != m_Ignore) || !(parkedCar.m_Lane == m_Lane))
				{
					return;
				}
				Transform transform = m_TransformData[entity];
				float num = ((float3)(ref transform.m_Position))[m_Order] - 1f;
				float num2 = ((float3)(ref transform.m_Position))[m_Order] + 1f;
				int num3 = 0;
				int num4 = m_Spots.Length;
				while (num4 > num3)
				{
					int num5 = num3 + num4 >> 1;
					if (m_Spots[num5].m_Order < num)
					{
						num3 = num5 + 1;
					}
					else
					{
						num4 = num5;
					}
				}
				num4 = m_Spots.Length;
				while (num3 < num4)
				{
					ref SpotData reference = ref m_Spots.ElementAt(num3++);
					if (!(reference.m_Order > num2))
					{
						if (math.distancesq(transform.m_Position, reference.m_Position) < 1f)
						{
							reference.m_Occupied = true;
						}
						continue;
					}
					break;
				}
			}
		}

		private struct SpawnLocationIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Entity m_Lane;

			public Entity m_Ignore;

			public Bounds3 m_Bounds;

			public ComponentLookup<ParkedCar> m_ParkedCarData;

			public ComponentLookup<Controller> m_ControllerData;

			public bool m_Occupied;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				Controller controller = default(Controller);
				ParkedCar parkedCar = default(ParkedCar);
				if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds) && !(entity == m_Ignore) && (!m_ControllerData.TryGetComponent(entity, ref controller) || !(controller.m_Controller == m_Ignore)) && m_ParkedCarData.TryGetComponent(entity, ref parkedCar) && parkedCar.m_Lane == m_Lane)
				{
					m_Occupied = true;
				}
			}
		}

		private struct LaneIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Entity m_VehicleEntity;

			public Bounds3 m_Bounds;

			public float3 m_Position;

			public float2 m_ParkingSize;

			public float m_MaxDistance;

			public float m_ParkingOffset;

			public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingObjectSearchTree;

			public NativeHashMap<Entity, bool> m_IsUnspawnedMap;

			public ComponentLookup<ParkedCar> m_ParkedCarData;

			public ComponentLookup<ParkedTrain> m_ParkedTrainData;

			public ComponentLookup<Controller> m_ControllerData;

			public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<Unspawned> m_UnspawnedData;

			public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

			public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

			public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

			public BufferLookup<LaneObject> m_LaneObjects;

			public BufferLookup<LaneOverlap> m_LaneOverlaps;

			public Entity m_SelectedLane;

			public float m_SelectedCurvePos;

			public bool m_KeepUnspawned;

			public bool m_SpecialVehicle;

			public TrackTypes m_TrackType;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0117: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0166: Unknown result type (might be due to invalid IL or missing references)
				//IL_0168: Unknown result type (might be due to invalid IL or missing references)
				//IL_017f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0180: Unknown result type (might be due to invalid IL or missing references)
				//IL_0192: Unknown result type (might be due to invalid IL or missing references)
				//IL_019d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds))
				{
					return;
				}
				if (m_ParkingLaneData.HasComponent(entity))
				{
					Curve curve = m_CurveData[entity];
					float curvePos = default(float);
					if (MathUtils.Distance(curve.m_Bezier, m_Position, ref curvePos) < m_MaxDistance && (m_KeepUnspawned || TryFindParkingSpace(entity, curve, ignoreDisabled: true, ref curvePos)))
					{
						float3 val = MathUtils.Position(curve.m_Bezier, curvePos);
						float num = math.distance(m_Position, val);
						if (num < m_MaxDistance)
						{
							m_MaxDistance = num;
							m_SelectedLane = entity;
							m_SelectedCurvePos = curvePos;
							m_Bounds = new Bounds3(m_Position - m_MaxDistance, m_Position + m_MaxDistance);
						}
					}
				}
				else
				{
					if (!m_SpawnLocationData.HasComponent(entity))
					{
						return;
					}
					Transform transform = m_TransformData[entity];
					float num2 = math.distance(transform.m_Position, m_Position);
					if (num2 < m_MaxDistance)
					{
						PrefabRef prefabRef = m_PrefabRefData[entity];
						SpawnLocationData spawnLocationData = default(SpawnLocationData);
						if (m_PrefabSpawnLocationData.TryGetComponent(prefabRef.m_Prefab, ref spawnLocationData) && ((m_TrackType == TrackTypes.None && (spawnLocationData.m_RoadTypes & RoadTypes.Helicopter) != RoadTypes.None && spawnLocationData.m_ConnectionType == RouteConnectionType.Air) || (spawnLocationData.m_TrackTypes & m_TrackType) != TrackTypes.None) && TryFindParkingSpace(entity, m_VehicleEntity, transform))
						{
							m_MaxDistance = num2;
							m_SelectedLane = entity;
							m_SelectedCurvePos = 0f;
							m_Bounds = new Bounds3(m_Position - m_MaxDistance, m_Position + m_MaxDistance);
						}
					}
				}
			}

			public bool TryFindParkingSpace(Entity lane, Entity vehicle, Transform transform)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0117: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				PrefabRef prefabRef = m_PrefabRefData[lane];
				switch (m_PrefabSpawnLocationData[prefabRef.m_Prefab].m_ConnectionType)
				{
				case RouteConnectionType.Air:
				{
					SpawnLocationIterator spawnLocationIterator = new SpawnLocationIterator
					{
						m_Lane = lane,
						m_Bounds = new Bounds3(transform.m_Position - 1f, transform.m_Position + 1f),
						m_Ignore = vehicle,
						m_ParkedCarData = m_ParkedCarData,
						m_ControllerData = m_ControllerData
					};
					m_MovingObjectSearchTree.Iterate<SpawnLocationIterator>(ref spawnLocationIterator, 0);
					return !spawnLocationIterator.m_Occupied;
				}
				case RouteConnectionType.Track:
				{
					Game.Objects.SpawnLocation spawnLocation = m_SpawnLocationData[lane];
					DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
					if (m_LaneObjects.TryGetBuffer(spawnLocation.m_ConnectedLane1, ref val))
					{
						Controller controller = default(Controller);
						ParkedTrain parkedTrain = default(ParkedTrain);
						for (int i = 0; i < val.Length; i++)
						{
							LaneObject laneObject = val[i];
							if (!(laneObject.m_LaneObject == vehicle) && (!m_ControllerData.TryGetComponent(laneObject.m_LaneObject, ref controller) || !(controller.m_Controller == vehicle)) && m_ParkedTrainData.TryGetComponent(laneObject.m_LaneObject, ref parkedTrain) && parkedTrain.m_ParkingLocation == lane)
							{
								return false;
							}
						}
					}
					return true;
				}
				default:
					return false;
				}
			}

			public bool TryFindParkingSpace(Entity lane, Curve curve, bool ignoreDisabled, ref float curvePos)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0483: Unknown result type (might be due to invalid IL or missing references)
				//IL_048b: Unknown result type (might be due to invalid IL or missing references)
				//IL_049a: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_04da: Unknown result type (might be due to invalid IL or missing references)
				//IL_04df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_055d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0562: Unknown result type (might be due to invalid IL or missing references)
				//IL_050d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0595: Unknown result type (might be due to invalid IL or missing references)
				//IL_059f: Unknown result type (might be due to invalid IL or missing references)
				//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_051b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0529: Unknown result type (might be due to invalid IL or missing references)
				//IL_053a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0544: Unknown result type (might be due to invalid IL or missing references)
				//IL_0549: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_0796: Unknown result type (might be due to invalid IL or missing references)
				//IL_016d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_0663: Unknown result type (might be due to invalid IL or missing references)
				//IL_0682: Unknown result type (might be due to invalid IL or missing references)
				//IL_0687: Unknown result type (might be due to invalid IL or missing references)
				//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_017c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0809: Unknown result type (might be due to invalid IL or missing references)
				//IL_018a: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0706: Unknown result type (might be due to invalid IL or missing references)
				//IL_070b: Unknown result type (might be due to invalid IL or missing references)
				//IL_070d: Unknown result type (might be due to invalid IL or missing references)
				//IL_070f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0716: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_083b: Unknown result type (might be due to invalid IL or missing references)
				//IL_084e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0853: Unknown result type (might be due to invalid IL or missing references)
				//IL_0879: Unknown result type (might be due to invalid IL or missing references)
				//IL_0880: Unknown result type (might be due to invalid IL or missing references)
				//IL_088e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0895: Unknown result type (might be due to invalid IL or missing references)
				//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_08be: Unknown result type (might be due to invalid IL or missing references)
				//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_0820: Unknown result type (might be due to invalid IL or missing references)
				//IL_0825: Unknown result type (might be due to invalid IL or missing references)
				//IL_0827: Unknown result type (might be due to invalid IL or missing references)
				//IL_0829: Unknown result type (might be due to invalid IL or missing references)
				//IL_0202: Unknown result type (might be due to invalid IL or missing references)
				//IL_0209: Unknown result type (might be due to invalid IL or missing references)
				//IL_020e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0212: Unknown result type (might be due to invalid IL or missing references)
				//IL_0214: Unknown result type (might be due to invalid IL or missing references)
				//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0761: Unknown result type (might be due to invalid IL or missing references)
				//IL_0777: Unknown result type (might be due to invalid IL or missing references)
				//IL_077c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0783: Unknown result type (might be due to invalid IL or missing references)
				//IL_0788: Unknown result type (might be due to invalid IL or missing references)
				//IL_072b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0739: Unknown result type (might be due to invalid IL or missing references)
				//IL_060e: Unknown result type (might be due to invalid IL or missing references)
				//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_08da: Unknown result type (might be due to invalid IL or missing references)
				//IL_0750: Unknown result type (might be due to invalid IL or missing references)
				//IL_0755: Unknown result type (might be due to invalid IL or missing references)
				//IL_0757: Unknown result type (might be due to invalid IL or missing references)
				//IL_0759: Unknown result type (might be due to invalid IL or missing references)
				//IL_061c: Unknown result type (might be due to invalid IL or missing references)
				//IL_062a: Unknown result type (might be due to invalid IL or missing references)
				//IL_063b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0645: Unknown result type (might be due to invalid IL or missing references)
				//IL_064a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0919: Unknown result type (might be due to invalid IL or missing references)
				//IL_0920: Unknown result type (might be due to invalid IL or missing references)
				//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0906: Unknown result type (might be due to invalid IL or missing references)
				//IL_0227: Unknown result type (might be due to invalid IL or missing references)
				//IL_024f: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0418: Unknown result type (might be due to invalid IL or missing references)
				//IL_042b: Unknown result type (might be due to invalid IL or missing references)
				//IL_02da: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0452: Unknown result type (might be due to invalid IL or missing references)
				//IL_0466: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_027c: Unknown result type (might be due to invalid IL or missing references)
				//IL_028b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0359: Unknown result type (might be due to invalid IL or missing references)
				//IL_0367: Unknown result type (might be due to invalid IL or missing references)
				//IL_0318: Unknown result type (might be due to invalid IL or missing references)
				//IL_0322: Unknown result type (might be due to invalid IL or missing references)
				//IL_0327: Unknown result type (might be due to invalid IL or missing references)
				//IL_0329: Unknown result type (might be due to invalid IL or missing references)
				//IL_0330: Unknown result type (might be due to invalid IL or missing references)
				//IL_0299: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_037c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0383: Unknown result type (might be due to invalid IL or missing references)
				//IL_0339: Unknown result type (might be due to invalid IL or missing references)
				//IL_033b: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
				Game.Net.ParkingLane parkingLane = m_ParkingLaneData[lane];
				if ((parkingLane.m_Flags & ParkingLaneFlags.VirtualLane) != 0)
				{
					return false;
				}
				if (ignoreDisabled && (parkingLane.m_Flags & ParkingLaneFlags.ParkingDisabled) != 0)
				{
					return false;
				}
				if (m_SpecialVehicle != ((parkingLane.m_Flags & ParkingLaneFlags.SpecialVehicles) != 0))
				{
					return false;
				}
				PrefabRef prefabRef = m_PrefabRefData[lane];
				DynamicBuffer<LaneObject> val = m_LaneObjects[lane];
				DynamicBuffer<LaneOverlap> val2 = m_LaneOverlaps[lane];
				ParkingLaneData parkingLaneData = m_PrefabParkingLaneData[prefabRef.m_Prefab];
				if (math.any(m_ParkingSize > VehicleUtils.GetParkingSize(parkingLaneData)))
				{
					return false;
				}
				if (parkingLaneData.m_SlotInterval != 0f)
				{
					int parkingSlotCount = NetUtils.GetParkingSlotCount(curve, parkingLane, parkingLaneData);
					float parkingSlotInterval = NetUtils.GetParkingSlotInterval(curve, parkingLane, parkingLaneData, parkingSlotCount);
					float3 val3 = curve.m_Bezier.a;
					float2 val4 = float2.op_Implicit(0f);
					float num = 0f;
					float num2 = math.max((parkingLane.m_Flags & (ParkingLaneFlags.StartingLane | ParkingLaneFlags.EndingLane)) switch
					{
						ParkingLaneFlags.StartingLane => curve.m_Length - (float)parkingSlotCount * parkingSlotInterval, 
						ParkingLaneFlags.EndingLane => 0f, 
						_ => (curve.m_Length - (float)parkingSlotCount * parkingSlotInterval) * 0.5f, 
					}, 0f);
					int i = -1;
					float num3 = 1f;
					float num4 = curvePos;
					float num5 = 2f;
					int num6 = 0;
					while (num6 < val.Length)
					{
						LaneObject laneObject = val[num6++];
						if (m_ParkedCarData.HasComponent(laneObject.m_LaneObject) && !IsUnspawned(laneObject.m_LaneObject))
						{
							num5 = laneObject.m_CurvePosition.x;
							break;
						}
					}
					float2 val5 = float2.op_Implicit(2f);
					int num7 = 0;
					if (num7 < val2.Length)
					{
						LaneOverlap laneOverlap = val2[num7++];
						val5 = new float2((float)(int)laneOverlap.m_ThisStart, (float)(int)laneOverlap.m_ThisEnd) * 0.003921569f;
					}
					for (int j = 1; j <= 16; j++)
					{
						float num8 = (float)j * 0.0625f;
						float3 val6 = MathUtils.Position(curve.m_Bezier, num8);
						for (num += math.distance(val3, val6); num >= num2 || (j == 16 && i < parkingSlotCount); i++)
						{
							val4.y = math.select(num8, math.lerp(val4.x, num8, num2 / num), num2 < num);
							bool flag = false;
							if (num5 <= val4.y)
							{
								num5 = 2f;
								flag = true;
								while (num6 < val.Length)
								{
									LaneObject laneObject2 = val[num6++];
									if (m_ParkedCarData.HasComponent(laneObject2.m_LaneObject) && !IsUnspawned(laneObject2.m_LaneObject) && laneObject2.m_CurvePosition.x > val4.y)
									{
										num5 = laneObject2.m_CurvePosition.x;
										break;
									}
								}
							}
							if (val5.x < val4.y)
							{
								flag = true;
								if (val5.y <= val4.y)
								{
									val5 = float2.op_Implicit(2f);
									while (num7 < val2.Length)
									{
										LaneOverlap laneOverlap2 = val2[num7++];
										float2 val7 = new float2((float)(int)laneOverlap2.m_ThisStart, (float)(int)laneOverlap2.m_ThisEnd) * 0.003921569f;
										if (val7.y > val4.y)
										{
											val5 = val7;
											break;
										}
									}
								}
							}
							if (!flag && i >= 0 && i < parkingSlotCount)
							{
								float num9 = math.max(val4.x - curvePos, curvePos - val4.y);
								if (num9 < num3)
								{
									num4 = math.lerp(val4.x, val4.y, 0.5f);
									num3 = num9;
								}
							}
							num -= num2;
							val4.x = val4.y;
							num2 = parkingSlotInterval;
						}
						val3 = val6;
					}
					if (num4 != curvePos && parkingLaneData.m_SlotAngle <= 0.25f)
					{
						if (m_ParkingOffset > 0f)
						{
							Bounds1 val8 = default(Bounds1);
							((Bounds1)(ref val8))._002Ector(num4, 1f);
							MathUtils.ClampLength(curve.m_Bezier, ref val8, m_ParkingOffset);
							num4 = val8.max;
						}
						else if (m_ParkingOffset < 0f)
						{
							Bounds1 val9 = default(Bounds1);
							((Bounds1)(ref val9))._002Ector(0f, num4);
							MathUtils.ClampLengthInverse(curve.m_Bezier, ref val9, 0f - m_ParkingOffset);
							num4 = val9.min;
						}
					}
					curvePos = num4;
					return num3 != 1f;
				}
				float2 val10 = default(float2);
				float2 val11 = default(float2);
				float num10 = 1f;
				float3 val12 = default(float3);
				float2 val13 = float2.op_Implicit(math.select(0f, 0.5f, (parkingLane.m_Flags & ParkingLaneFlags.StartingLane) == 0));
				float3 val14 = curve.m_Bezier.a;
				float num11 = 2f;
				float2 val15 = float2.op_Implicit(0f);
				int num12 = 0;
				while (num12 < val.Length)
				{
					LaneObject laneObject3 = val[num12++];
					if (m_ParkedCarData.HasComponent(laneObject3.m_LaneObject) && !IsUnspawned(laneObject3.m_LaneObject))
					{
						num11 = laneObject3.m_CurvePosition.x;
						val15 = VehicleUtils.GetParkingOffsets(laneObject3.m_LaneObject, ref m_PrefabRefData, ref m_PrefabObjectGeometryData) + 1f;
						break;
					}
				}
				float2 val16 = float2.op_Implicit(2f);
				int num13 = 0;
				if (num13 < val2.Length)
				{
					LaneOverlap laneOverlap3 = val2[num13++];
					val16 = new float2((float)(int)laneOverlap3.m_ThisStart, (float)(int)laneOverlap3.m_ThisEnd) * 0.003921569f;
				}
				while (num11 != 2f || val16.x != 2f)
				{
					float x;
					if (num11 <= val16.x)
					{
						((float3)(ref val12)).yz = float2.op_Implicit(num11);
						val13.y = val15.x;
						x = val15.y;
						num11 = 2f;
						while (num12 < val.Length)
						{
							LaneObject laneObject4 = val[num12++];
							if (m_ParkedCarData.HasComponent(laneObject4.m_LaneObject) && !IsUnspawned(laneObject4.m_LaneObject))
							{
								num11 = laneObject4.m_CurvePosition.x;
								val15 = VehicleUtils.GetParkingOffsets(laneObject4.m_LaneObject, ref m_PrefabRefData, ref m_PrefabObjectGeometryData) + 1f;
								break;
							}
						}
					}
					else
					{
						((float3)(ref val12)).yz = val16;
						val13.y = 0.5f;
						x = 0.5f;
						val16 = float2.op_Implicit(2f);
						while (num13 < val2.Length)
						{
							LaneOverlap laneOverlap4 = val2[num13++];
							float2 val17 = new float2((float)(int)laneOverlap4.m_ThisStart, (float)(int)laneOverlap4.m_ThisEnd) * 0.003921569f;
							if (val17.x <= val12.z)
							{
								val12.z = math.max(val12.z, val17.y);
								continue;
							}
							val16 = val17;
							break;
						}
					}
					float3 val18 = MathUtils.Position(curve.m_Bezier, val12.y);
					if (math.distance(val14, val18) - math.csum(val13) >= m_ParkingSize.y)
					{
						float num14 = math.max(val12.x - curvePos, curvePos - val12.y);
						if (num14 < num10)
						{
							val10 = ((float3)(ref val12)).xy;
							val11 = val13;
							num10 = num14;
						}
					}
					val12.x = val12.z;
					val13.x = x;
					val14 = MathUtils.Position(curve.m_Bezier, val12.z);
				}
				val12.y = 1f;
				val13.y = math.select(0f, 0.5f, (parkingLane.m_Flags & ParkingLaneFlags.EndingLane) == 0);
				if (math.distance(val14, curve.m_Bezier.d) - math.csum(val13) >= m_ParkingSize.y)
				{
					float num15 = math.max(val12.x - curvePos, curvePos - val12.y);
					if (num15 < num10)
					{
						val10 = ((float3)(ref val12)).xy;
						val11 = val13;
						num10 = num15;
					}
				}
				if (num10 != 1f)
				{
					val11 += m_ParkingSize.y * 0.5f;
					val11.x += m_ParkingOffset;
					val11.y -= m_ParkingOffset;
					Bounds1 val19 = default(Bounds1);
					((Bounds1)(ref val19))._002Ector(val10.x, val10.y);
					Bounds1 val20 = default(Bounds1);
					((Bounds1)(ref val20))._002Ector(val10.x, val10.y);
					MathUtils.ClampLength(curve.m_Bezier, ref val19, val11.x);
					MathUtils.ClampLengthInverse(curve.m_Bezier, ref val20, val11.y);
					if (curvePos < val19.max || curvePos > val20.min)
					{
						if (val19.max < val20.min)
						{
							curvePos = math.select(val19.max, val20.min, curvePos > val20.min);
						}
						else
						{
							curvePos = math.lerp(val19.max, val20.min, 0.5f);
						}
					}
					return true;
				}
				return false;
			}

			private bool IsUnspawned(Entity vehicle)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				bool result = default(bool);
				if (m_IsUnspawnedMap.TryGetValue(vehicle, ref result))
				{
					return result;
				}
				return m_UnspawnedData.HasComponent(vehicle);
			}
		}

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<FixParkingLocation> m_FixParkingLocationData;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Relative> m_RelativeData;

		[ReadOnly]
		public ComponentLookup<Helicopter> m_HelicopterData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public ComponentLookup<TrainData> m_PrefabTrainData;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public BufferLookup<LaneOverlap> m_LaneOverlaps;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_ActivityLocations;

		public ComponentLookup<Transform> m_TransformData;

		public ComponentLookup<ParkedCar> m_ParkedCarData;

		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		public ComponentLookup<PersonalCar> m_PersonalCarData;

		public ComponentLookup<CarKeeper> m_CarKeeperData;

		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_LaneSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

		public NativeQueue<Entity> m_VehicleQueue;

		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingObjectSearchTree;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0797: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_075e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0688: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_090b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0910: Unknown result type (might be due to invalid IL or missing references)
			//IL_0918: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_070e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_092c: Unknown result type (might be due to invalid IL or missing references)
			//IL_089f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_081b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_093f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0981: Unknown result type (might be due to invalid IL or missing references)
			//IL_0986: Unknown result type (might be due to invalid IL or missing references)
			//IL_098e: Unknown result type (might be due to invalid IL or missing references)
			//IL_099b: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			if (m_VehicleQueue.Count == 0)
			{
				return;
			}
			NativeHashMap<Entity, bool> isUnspawnedMap = default(NativeHashMap<Entity, bool>);
			isUnspawnedMap._002Ector(m_VehicleQueue.Count * 2, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<PathElement> laneBuffer = default(NativeList<PathElement>);
			Random random = m_RandomSeed.GetRandom(0);
			LaneIterator laneIterator = new LaneIterator
			{
				m_MovingObjectSearchTree = m_MovingObjectSearchTree,
				m_IsUnspawnedMap = isUnspawnedMap,
				m_ParkedCarData = m_ParkedCarData,
				m_ParkedTrainData = m_ParkedTrainData,
				m_ControllerData = m_ControllerData,
				m_ParkingLaneData = m_ParkingLaneData,
				m_CurveData = m_CurveData,
				m_UnspawnedData = m_UnspawnedData,
				m_TransformData = m_TransformData,
				m_SpawnLocationData = m_SpawnLocationData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabParkingLaneData = m_PrefabParkingLaneData,
				m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
				m_PrefabSpawnLocationData = m_PrefabSpawnLocationData,
				m_LaneObjects = m_LaneObjects,
				m_LaneOverlaps = m_LaneOverlaps
			};
			Entity val = default(Entity);
			ParkedCar parkedCar = default(ParkedCar);
			ParkedTrain parkedTrain = default(ParkedTrain);
			DynamicBuffer<LaneObject> buffer = default(DynamicBuffer<LaneObject>);
			DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
			FixParkingLocation fixParkingLocation2 = default(FixParkingLocation);
			DynamicBuffer<LaneObject> buffer2 = default(DynamicBuffer<LaneObject>);
			Transform transform3 = default(Transform);
			Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
			SpawnLocationData spawnLocationData = default(SpawnLocationData);
			TrainData trainData = default(TrainData);
			Game.Net.ConnectionLane connectionLane2 = default(Game.Net.ConnectionLane);
			Game.Net.ParkingLane parkingLane = default(Game.Net.ParkingLane);
			Owner owner = default(Owner);
			Game.Net.ParkingLane parkingLane2 = default(Game.Net.ParkingLane);
			Owner owner2 = default(Owner);
			while (m_VehicleQueue.TryDequeue(ref val))
			{
				bool flag = m_UnspawnedData.HasComponent(val);
				if (!isUnspawnedMap.TryAdd(val, flag))
				{
					continue;
				}
				bool flag2 = m_ParkedCarData.TryGetComponent(val, ref parkedCar);
				bool flag3 = m_ParkedTrainData.TryGetComponent(val, ref parkedTrain);
				if (!flag2 && !flag3)
				{
					FixParkingLocation fixParkingLocation = m_FixParkingLocationData[val];
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<FixParkingLocation>(val);
					if (m_LaneObjects.TryGetBuffer(fixParkingLocation.m_ChangeLane, ref buffer))
					{
						NetUtils.RemoveLaneObject(buffer, val);
					}
					continue;
				}
				Transform transform = m_TransformData[val];
				PrefabRef prefabRef = m_PrefabRefData[val];
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				m_LayoutElements.TryGetBuffer(val, ref layout);
				Transform transform2 = transform;
				bool flag4 = false;
				bool flag5 = flag && flag2 && m_LaneObjects.HasBuffer(parkedCar.m_Lane);
				if (m_FixParkingLocationData.TryGetComponent(val, ref fixParkingLocation2))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<FixParkingLocation>(val);
					if (m_LaneObjects.TryGetBuffer(fixParkingLocation2.m_ChangeLane, ref buffer2))
					{
						NetUtils.RemoveLaneObject(buffer2, val);
					}
					if (fixParkingLocation2.m_ResetLocation != val)
					{
						if (m_TransformData.TryGetComponent(fixParkingLocation2.m_ResetLocation, ref transform3))
						{
							transform = transform3;
						}
						else
						{
							flag4 = true;
						}
						RemoveCarKeeper(val);
					}
					if (flag2 && m_LaneObjects.TryGetBuffer(parkedCar.m_Lane, ref buffer2))
					{
						NetUtils.RemoveLaneObject(buffer2, val);
						if (m_ParkingLaneData.HasComponent(parkedCar.m_Lane) && isUnspawnedMap.TryAdd(parkedCar.m_Lane, false) && !m_UpdatedData.HasComponent(parkedCar.m_Lane))
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(parkedCar.m_Lane, default(PathfindUpdated));
						}
					}
					else
					{
						Entity val2 = (flag2 ? parkedCar.m_Lane : parkedTrain.m_ParkingLocation);
						if (!m_DeletedData.HasComponent(val2) && !flag4)
						{
							if (flag2 && m_ConnectionLaneData.TryGetComponent(parkedCar.m_Lane, ref connectionLane))
							{
								if ((connectionLane.m_Flags & ConnectionLaneFlags.Parking) != 0)
								{
									if (FindGarageSpot(ref random, val, parkedCar.m_Lane, ref transform))
									{
										m_TransformData[val] = transform;
										if (flag)
										{
											((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Unspawned>(val);
											((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(val, default(BatchesUpdated));
											isUnspawnedMap[val] = false;
										}
									}
									AddToSearchTree(val, transform, objectGeometryData);
									continue;
								}
							}
							else if (m_SpawnLocationData.HasComponent(val2))
							{
								Transform transform4 = m_TransformData[val2];
								PrefabRef prefabRef2 = m_PrefabRefData[val2];
								if (m_PrefabSpawnLocationData.TryGetComponent(prefabRef2.m_Prefab, ref spawnLocationData) && ((flag2 && (spawnLocationData.m_RoadTypes & RoadTypes.Helicopter) != RoadTypes.None && spawnLocationData.m_ConnectionType == RouteConnectionType.Air) || (flag3 && spawnLocationData.m_TrackTypes != TrackTypes.None && ValidateParkedTrain(layout))) && laneIterator.TryFindParkingSpace(val2, val, transform4))
								{
									if (flag3)
									{
										UpdateTrainLocation(val, parkedTrain.m_ParkingLocation, layout, ref laneBuffer);
										continue;
									}
									transform.m_Position = transform4.m_Position;
									m_TransformData[val] = transform;
									AddToSearchTree(val, transform, objectGeometryData);
									continue;
								}
							}
						}
						if (layout.IsCreated && layout.Length != 0)
						{
							for (int i = 0; i < layout.Length; i++)
							{
								m_MovingObjectSearchTree.TryRemove(layout[i].m_Vehicle);
							}
						}
						else
						{
							m_MovingObjectSearchTree.TryRemove(val);
						}
					}
					parkedCar.m_Lane = Entity.Null;
					parkedTrain.m_ParkingLocation = Entity.Null;
				}
				laneIterator.m_VehicleEntity = val;
				laneIterator.m_Position = transform.m_Position;
				laneIterator.m_MaxDistance = 100f;
				laneIterator.m_ParkingSize = VehicleUtils.GetParkingSize(val, ref m_PrefabRefData, ref m_PrefabObjectGeometryData, out laneIterator.m_ParkingOffset);
				laneIterator.m_Bounds = new Bounds3(laneIterator.m_Position - laneIterator.m_MaxDistance, laneIterator.m_Position + laneIterator.m_MaxDistance);
				laneIterator.m_SelectedLane = Entity.Null;
				laneIterator.m_KeepUnspawned = flag5;
				laneIterator.m_SpecialVehicle = !m_PersonalCarData.HasComponent(val);
				laneIterator.m_TrackType = TrackTypes.None;
				if (flag3 && m_PrefabTrainData.TryGetComponent(prefabRef.m_Prefab, ref trainData))
				{
					laneIterator.m_TrackType = trainData.m_TrackType;
				}
				if (flag2 && m_ConnectionLaneData.TryGetComponent(parkedCar.m_Lane, ref connectionLane2))
				{
					flag4 |= (connectionLane2.m_Flags & ConnectionLaneFlags.Outside) != 0;
				}
				else if (flag2 && m_ParkingLaneData.TryGetComponent(parkedCar.m_Lane, ref parkingLane) && !m_DeletedData.HasComponent(parkedCar.m_Lane))
				{
					Curve curve = m_CurveData[parkedCar.m_Lane];
					if (flag5 || laneIterator.TryFindParkingSpace(parkedCar.m_Lane, curve, ignoreDisabled: false, ref parkedCar.m_CurvePosition))
					{
						PrefabRef prefabRef3 = m_PrefabRefData[parkedCar.m_Lane];
						ParkingLaneData parkingLaneData = m_PrefabParkingLaneData[prefabRef3.m_Prefab];
						Transform ownerTransform = default(Transform);
						if (m_OwnerData.TryGetComponent(parkedCar.m_Lane, ref owner) && m_TransformData.HasComponent(owner.m_Owner))
						{
							ownerTransform = m_TransformData[owner.m_Owner];
						}
						transform = VehicleUtils.CalculateParkingSpaceTarget(parkingLane, parkingLaneData, objectGeometryData, curve, ownerTransform, parkedCar.m_CurvePosition);
						NetUtils.AddLaneObject(m_LaneObjects[parkedCar.m_Lane], val, float2.op_Implicit(parkedCar.m_CurvePosition));
						UpdateParkedCar(val, transform2, transform, parkedCar);
						continue;
					}
				}
				if (!flag4)
				{
					if (flag2)
					{
						m_LaneSearchTree.Iterate<LaneIterator>(ref laneIterator, 0);
					}
					if ((flag2 && m_HelicopterData.HasComponent(val)) || flag3)
					{
						m_StaticObjectSearchTree.Iterate<LaneIterator>(ref laneIterator, 0);
					}
				}
				if (laneIterator.m_SelectedLane != Entity.Null)
				{
					if (m_ParkingLaneData.TryGetComponent(laneIterator.m_SelectedLane, ref parkingLane2))
					{
						Curve curve2 = m_CurveData[laneIterator.m_SelectedLane];
						PrefabRef prefabRef4 = m_PrefabRefData[laneIterator.m_SelectedLane];
						ParkingLaneData parkingLaneData2 = m_PrefabParkingLaneData[prefabRef4.m_Prefab];
						Transform ownerTransform2 = default(Transform);
						if (m_OwnerData.TryGetComponent(laneIterator.m_SelectedLane, ref owner2) && m_TransformData.HasComponent(owner2.m_Owner))
						{
							ownerTransform2 = m_TransformData[owner2.m_Owner];
						}
						if (flag5)
						{
							laneIterator.m_SelectedCurvePos = math.clamp(laneIterator.m_SelectedCurvePos, 0.05f, 0.95f);
							laneIterator.m_SelectedCurvePos = ((Random)(ref random)).NextFloat(math.max(0.05f, laneIterator.m_SelectedCurvePos - 0.2f), math.min(0.95f, laneIterator.m_SelectedCurvePos + 0.2f));
						}
						transform = VehicleUtils.CalculateParkingSpaceTarget(parkingLane2, parkingLaneData2, objectGeometryData, curve2, ownerTransform2, laneIterator.m_SelectedCurvePos);
						NetUtils.AddLaneObject(m_LaneObjects[laneIterator.m_SelectedLane], val, float2.op_Implicit(laneIterator.m_SelectedCurvePos));
						m_MovingObjectSearchTree.TryRemove(val);
					}
					else
					{
						transform = m_TransformData[laneIterator.m_SelectedLane];
						if (!flag3)
						{
							AddToSearchTree(val, transform, objectGeometryData);
						}
					}
					parkedCar.m_Lane = laneIterator.m_SelectedLane;
					parkedCar.m_CurvePosition = laneIterator.m_SelectedCurvePos;
					parkedTrain.m_ParkingLocation = laneIterator.m_SelectedLane;
					if (isUnspawnedMap.TryAdd(laneIterator.m_SelectedLane, false) && !m_UpdatedData.HasComponent(laneIterator.m_SelectedLane))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(laneIterator.m_SelectedLane, default(PathfindUpdated));
					}
					if (flag && !flag5)
					{
						if (layout.IsCreated && layout.Length != 0)
						{
							for (int j = 0; j < layout.Length; j++)
							{
								Entity vehicle = layout[j].m_Vehicle;
								((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Unspawned>(vehicle);
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(vehicle, default(BatchesUpdated));
								isUnspawnedMap[val] = false;
							}
						}
						else
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Unspawned>(val);
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(val, default(BatchesUpdated));
							isUnspawnedMap[val] = false;
						}
					}
					if (flag2)
					{
						UpdateParkedCar(val, transform2, transform, parkedCar);
					}
					else
					{
						if (!flag3)
						{
							continue;
						}
						UpdateTrainLocation(val, parkedTrain.m_ParkingLocation, layout, ref laneBuffer);
						bool flag6 = m_TransformData[val].Equals(transform2);
						for (int k = 0; k < layout.Length; k++)
						{
							Entity vehicle2 = layout[k].m_Vehicle;
							ParkedTrain parkedTrain2 = m_ParkedTrainData[vehicle2];
							parkedTrain2.m_ParkingLocation = parkedTrain.m_ParkingLocation;
							m_ParkedTrainData[vehicle2] = parkedTrain2;
							if (flag6)
							{
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(vehicle2, default(Updated));
							}
						}
					}
					continue;
				}
				parkedCar.m_Lane = Entity.Null;
				parkedTrain.m_ParkingLocation = Entity.Null;
				RemoveCarKeeper(val);
				if (layout.IsCreated && layout.Length != 0)
				{
					for (int l = 0; l < layout.Length; l++)
					{
						Entity vehicle3 = layout[l].m_Vehicle;
						PrefabRef prefabRef5 = m_PrefabRefData[vehicle3];
						ObjectGeometryData objectGeometryData2 = m_PrefabObjectGeometryData[prefabRef5.m_Prefab];
						if (flag2)
						{
							AddToSearchTree(vehicle3, transform, objectGeometryData2);
						}
						if (!flag)
						{
							isUnspawnedMap[val] = true;
						}
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Unspawned>(vehicle3, default(Unspawned));
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(vehicle3, default(Updated));
					}
				}
				else
				{
					if (flag2)
					{
						AddToSearchTree(val, transform, objectGeometryData);
					}
					if (!flag)
					{
						isUnspawnedMap[val] = true;
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Unspawned>(val, default(Unspawned));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
				}
				if (flag2)
				{
					UpdateParkedCar(val, transform2, transform, parkedCar);
				}
				else if (flag3)
				{
					RemoveTrainFromLanes(layout);
					for (int m = 0; m < layout.Length; m++)
					{
						Entity vehicle4 = layout[m].m_Vehicle;
						ParkedTrain parkedTrain3 = m_ParkedTrainData[vehicle4];
						parkedTrain3.m_ParkingLocation = parkedTrain.m_ParkingLocation;
						parkedTrain3.m_FrontLane = Entity.Null;
						parkedTrain3.m_RearLane = Entity.Null;
						m_ParkedTrainData[vehicle4] = parkedTrain3;
						UpdateParkedTrain(vehicle4, m_TransformData[vehicle4], transform, parkedTrain3);
					}
				}
			}
			isUnspawnedMap.Dispose();
			if (laneBuffer.IsCreated)
			{
				laneBuffer.Dispose();
			}
		}

		private void UpdateTrainLocation(Entity entity, Entity parkingLocation, DynamicBuffer<LayoutElement> layout, ref NativeList<PathElement> laneBuffer)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			RemoveTrainFromLanes(layout);
			PathOwner pathOwner = default(PathOwner);
			ComponentLookup<TrainCurrentLane> currentLaneData = default(ComponentLookup<TrainCurrentLane>);
			ComponentLookup<TrainNavigation> navigationData = default(ComponentLookup<TrainNavigation>);
			if (laneBuffer.IsCreated)
			{
				laneBuffer.Clear();
			}
			else
			{
				laneBuffer = new NativeList<PathElement>(10, AllocatorHandle.op_Implicit((Allocator)2));
			}
			float length = VehicleUtils.CalculateLength(entity, layout, ref m_PrefabRefData, ref m_PrefabTrainData);
			PathUtils.InitializeSpawnPath(default(DynamicBuffer<PathElement>), laneBuffer, parkingLocation, ref pathOwner, length, ref m_CurveData, ref m_LaneData, ref m_EdgeLaneData, ref m_OwnerData, ref m_EdgeData, ref m_SpawnLocationData, ref m_ConnectedEdges, ref m_SubLanes);
			VehicleUtils.UpdateCarriageLocations(layout, laneBuffer, ref m_TrainData, ref m_ParkedTrainData, ref currentLaneData, ref navigationData, ref m_TransformData, ref m_CurveData, ref m_ConnectionLaneData, ref m_PrefabRefData, ref m_PrefabTrainData);
			AddTrainToLanes(layout);
		}

		private void RemoveTrainFromLanes(DynamicBuffer<LayoutElement> layout)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<LaneObject> buffer = default(DynamicBuffer<LaneObject>);
			for (int i = 0; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				ParkedTrain parkedTrain = m_ParkedTrainData[vehicle];
				if (m_LaneObjects.TryGetBuffer(parkedTrain.m_FrontLane, ref buffer))
				{
					NetUtils.RemoveLaneObject(buffer, vehicle);
				}
				if (parkedTrain.m_RearLane != parkedTrain.m_FrontLane && m_LaneObjects.TryGetBuffer(parkedTrain.m_RearLane, ref buffer))
				{
					NetUtils.RemoveLaneObject(buffer, vehicle);
				}
			}
		}

		private void AddTrainToLanes(DynamicBuffer<LayoutElement> layout)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<LaneObject> buffer = default(DynamicBuffer<LaneObject>);
			for (int i = 0; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				ParkedTrain parkedTrain = m_ParkedTrainData[vehicle];
				TrainNavigationHelpers.GetCurvePositions(ref parkedTrain, out var pos, out var pos2);
				if (m_LaneObjects.TryGetBuffer(parkedTrain.m_FrontLane, ref buffer))
				{
					NetUtils.AddLaneObject(buffer, vehicle, pos);
				}
				if (parkedTrain.m_RearLane != parkedTrain.m_FrontLane && m_LaneObjects.TryGetBuffer(parkedTrain.m_RearLane, ref buffer))
				{
					NetUtils.AddLaneObject(buffer, vehicle, pos2);
				}
			}
		}

		private bool ValidateParkedTrain(DynamicBuffer<LayoutElement> layout)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			ParkedTrain parkedTrain = default(ParkedTrain);
			for (int i = 0; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				if (!m_ParkedTrainData.TryGetComponent(vehicle, ref parkedTrain))
				{
					return false;
				}
				if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(parkedTrain.m_FrontLane))
				{
					return false;
				}
				if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(parkedTrain.m_RearLane))
				{
					return false;
				}
			}
			return true;
		}

		private void AddToSearchTree(Entity entity, Transform transform, ObjectGeometryData objectGeometryData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			Bounds3 bounds = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, objectGeometryData);
			m_MovingObjectSearchTree.AddOrUpdate(entity, new QuadTreeBoundsXZ(bounds));
		}

		private void UpdateParkedCar(Entity entity, Transform oldTransform, Transform transform, ParkedCar parkedCar)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if (!transform.Equals(oldTransform))
			{
				m_TransformData[entity] = transform;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(entity, default(Updated));
				UpdateSubObjects(entity, transform);
			}
			m_ParkedCarData[entity] = parkedCar;
		}

		private void UpdateSubObjects(Entity entity, Transform transform)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (!m_SubObjects.TryGetBuffer(entity, ref val))
			{
				return;
			}
			Relative relative = default(Relative);
			for (int i = 0; i < val.Length; i++)
			{
				Game.Objects.SubObject subObject = val[i];
				if (m_RelativeData.TryGetComponent(subObject.m_SubObject, ref relative))
				{
					Transform transform2 = ObjectUtils.LocalToWorld(transform, new Transform(relative.m_Position, relative.m_Rotation));
					m_TransformData[subObject.m_SubObject] = transform2;
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(subObject.m_SubObject, default(Updated));
					UpdateSubObjects(subObject.m_SubObject, transform2);
				}
			}
		}

		private void UpdateParkedTrain(Entity entity, Transform oldTransform, Transform transform, ParkedTrain parkedTrain)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if (!transform.Equals(oldTransform))
			{
				m_TransformData[entity] = transform;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(entity, default(Updated));
				UpdateSubObjects(entity, transform);
			}
			m_ParkedTrainData[entity] = parkedTrain;
		}

		private void RemoveCarKeeper(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			PersonalCar personalCar = default(PersonalCar);
			if (m_PersonalCarData.TryGetComponent(entity, ref personalCar))
			{
				CarKeeper carKeeper = default(CarKeeper);
				if (EntitiesExtensions.TryGetEnabledComponent<CarKeeper>(m_CarKeeperData, personalCar.m_Keeper, ref carKeeper) && carKeeper.m_Car == entity)
				{
					carKeeper.m_Car = Entity.Null;
					m_CarKeeperData[personalCar.m_Keeper] = carKeeper;
				}
				personalCar.m_Keeper = Entity.Null;
				m_PersonalCarData[entity] = personalCar;
			}
		}

		private bool FindGarageSpot(ref Random random, Entity vehicle, Entity lane, ref Transform transform)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			Entity val = lane;
			while (m_OwnerData.HasComponent(val))
			{
				val = m_OwnerData[val].m_Owner;
			}
			if (m_BuildingData.HasComponent(val))
			{
				PrefabRef prefabRef = m_PrefabRefData[val];
				DynamicBuffer<ActivityLocationElement> val2 = default(DynamicBuffer<ActivityLocationElement>);
				if (m_ActivityLocations.TryGetBuffer(prefabRef.m_Prefab, ref val2))
				{
					Transform transform2 = m_TransformData[val];
					ActivityMask activityMask = new ActivityMask(ActivityType.GarageSpot);
					OccupySpotsIterator occupySpotsIterator = new OccupySpotsIterator
					{
						m_Lane = lane,
						m_Ignore = vehicle,
						m_Bounds = new Bounds3(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue)),
						m_Spots = new NativeList<SpotData>(val2.Length, AllocatorHandle.op_Implicit((Allocator)2)),
						m_ParkedCarData = m_ParkedCarData,
						m_TransformData = m_TransformData
					};
					int num = -1;
					for (int i = 0; i < val2.Length; i++)
					{
						ActivityLocationElement activityLocationElement = val2[i];
						if ((activityLocationElement.m_ActivityMask.m_Mask & activityMask.m_Mask) != 0)
						{
							float3 val3 = ObjectUtils.LocalToWorld(transform2, activityLocationElement.m_Position);
							occupySpotsIterator.m_Bounds.min = math.min(occupySpotsIterator.m_Bounds.min, val3 - 1f);
							occupySpotsIterator.m_Bounds.max = math.max(occupySpotsIterator.m_Bounds.max, val3 + 1f);
							ref NativeList<SpotData> reference = ref occupySpotsIterator.m_Spots;
							SpotData spotData = new SpotData
							{
								m_Position = val3,
								m_Index = i
							};
							reference.Add(ref spotData);
							if (math.distancesq(transform.m_Position, val3) < 1f)
							{
								num = i;
							}
						}
					}
					bool result = false;
					if (occupySpotsIterator.m_Spots.Length > 0)
					{
						if (occupySpotsIterator.m_Spots.Length >= 2)
						{
							float3 val4 = MathUtils.Size(occupySpotsIterator.m_Bounds);
							occupySpotsIterator.m_Order = math.select(0, 1, val4.y > val4.x);
							occupySpotsIterator.m_Order = math.select(occupySpotsIterator.m_Order, 2, math.all(val4.z > ((float3)(ref val4)).xy));
						}
						for (int j = 0; j < occupySpotsIterator.m_Spots.Length; j++)
						{
							ref SpotData reference2 = ref occupySpotsIterator.m_Spots.ElementAt(j);
							reference2.m_Order = ((float3)(ref reference2.m_Position))[occupySpotsIterator.m_Order];
						}
						if (occupySpotsIterator.m_Spots.Length >= 2)
						{
							NativeSortExtension.Sort<SpotData>(occupySpotsIterator.m_Spots);
						}
						m_MovingObjectSearchTree.Iterate<OccupySpotsIterator>(ref occupySpotsIterator, 0);
						int num2 = 0;
						bool flag = false;
						for (int k = 0; k < occupySpotsIterator.m_Spots.Length; k++)
						{
							ref SpotData reference3 = ref occupySpotsIterator.m_Spots.ElementAt(k);
							num2 += math.select(1, 0, reference3.m_Occupied);
							flag |= reference3.m_Index == num && !reference3.m_Occupied;
						}
						if (num2 != 0 && !flag)
						{
							num2 = ((Random)(ref random)).NextInt(num2);
							for (int l = 0; l < occupySpotsIterator.m_Spots.Length; l++)
							{
								ref SpotData reference4 = ref occupySpotsIterator.m_Spots.ElementAt(l);
								if (!reference4.m_Occupied && num2-- == 0)
								{
									num = reference4.m_Index;
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							ActivityLocationElement activityLocationElement2 = val2[num];
							transform = ObjectUtils.LocalToWorld(transform2, activityLocationElement2.m_Position, activityLocationElement2.m_Rotation);
							result = true;
						}
					}
					occupySpotsIterator.m_Spots.Dispose();
					return result;
				}
			}
			return false;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<FixParkingLocation> __Game_Vehicles_FixParkingLocation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MovedLocation> __Game_Objects_MovedLocation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public BufferTypeHandle<LaneObject> __Game_Net_LaneObject_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<FixParkingLocation> __Game_Vehicles_FixParkingLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Relative> __Game_Objects_Relative_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Helicopter> __Game_Vehicles_Helicopter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainData> __Game_Prefabs_TrainData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		public ComponentLookup<Transform> __Game_Objects_Transform_RW_ComponentLookup;

		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RW_ComponentLookup;

		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RW_ComponentLookup;

		public ComponentLookup<PersonalCar> __Game_Vehicles_PersonalCar_RW_ComponentLookup;

		public ComponentLookup<CarKeeper> __Game_Citizens_CarKeeper_RW_ComponentLookup;

		public BufferLookup<LaneObject> __Game_Net_LaneObject_RW_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Vehicles_FixParkingLocation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<FixParkingLocation>(true);
			__Game_Net_ConnectionLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.ConnectionLane>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.SpawnLocation>(true);
			__Game_Objects_MovedLocation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MovedLocation>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_LaneObject_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LaneObject>(false);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Vehicles_FixParkingLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FixParkingLocation>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Objects_Relative_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Relative>(true);
			__Game_Vehicles_Helicopter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Helicopter>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_TrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainData>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Objects_Transform_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(false);
			__Game_Vehicles_ParkedCar_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(false);
			__Game_Vehicles_ParkedTrain_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(false);
			__Game_Vehicles_PersonalCar_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PersonalCar>(false);
			__Game_Citizens_CarKeeper_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarKeeper>(false);
			__Game_Net_LaneObject_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(false);
		}
	}

	private ModificationBarrier5 m_ModificationBarrier;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private EntityQuery m_FixQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>(),
			ComponentType.ReadOnly<Game.Net.ConnectionLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>(),
			ComponentType.ReadOnly<FixParkingLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[1] = val;
		m_FixQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_FixQuery);
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
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0716: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<Entity> vehicleQueue = default(NativeQueue<Entity>);
		vehicleQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		CollectParkedCarsJob collectParkedCarsJob = new CollectParkedCarsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FixParkingLocationType = InternalCompilerInterface.GetComponentTypeHandle<FixParkingLocation>(ref __TypeHandle.__Game_Vehicles_FixParkingLocation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MovedLocationType = InternalCompilerInterface.GetComponentTypeHandle<MovedLocation>(ref __TypeHandle.__Game_Objects_MovedLocation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjectType = InternalCompilerInterface.GetBufferTypeHandle<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActivityLocations = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingObjectSearchTree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: true, out dependencies),
			m_VehicleQueue = vehicleQueue.AsParallelWriter()
		};
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle dependencies4;
		FixParkingLocationJob obj = new FixParkingLocationJob
		{
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FixParkingLocationData = InternalCompilerInterface.GetComponentLookup<FixParkingLocation>(ref __TypeHandle.__Game_Vehicles_FixParkingLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RelativeData = InternalCompilerInterface.GetComponentLookup<Relative>(ref __TypeHandle.__Game_Objects_Relative_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HelicopterData = InternalCompilerInterface.GetComponentLookup<Helicopter>(ref __TypeHandle.__Game_Vehicles_Helicopter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainData = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActivityLocations = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PersonalCarData = InternalCompilerInterface.GetComponentLookup<PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarKeeperData = InternalCompilerInterface.GetComponentLookup<CarKeeper>(ref __TypeHandle.__Game_Citizens_CarKeeper_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_LaneSearchTree = m_NetSearchSystem.GetLaneSearchTree(readOnly: true, out dependencies2),
			m_StaticObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies3),
			m_VehicleQueue = vehicleQueue,
			m_MovingObjectSearchTree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: false, out dependencies4),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		};
		JobHandle val = JobChunkExtensions.ScheduleParallel<CollectParkedCarsJob>(collectParkedCarsJob, m_FixQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		JobHandle val2 = IJobExtensions.Schedule<FixParkingLocationJob>(obj, JobUtils.CombineDependencies(val, dependencies2, dependencies3, dependencies4));
		vehicleQueue.Dispose(val2);
		m_NetSearchSystem.AddLaneSearchTreeReader(val2);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val2);
		m_ObjectSearchSystem.AddMovingSearchTreeWriter(val2);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
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
	public FixParkingLocationSystem()
	{
	}
}

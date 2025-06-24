using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Creatures;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
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

namespace Game.Net;

[CompilerGenerated]
public class FixLaneObjectsSystem : GameSystemBase
{
	[BurstCompile]
	private struct CollectLaneObjectsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> m_CarCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<CarTrailerLane> m_CarTrailerLaneData;

		[ReadOnly]
		public ComponentLookup<HumanCurrentLane> m_HumanCurrentLane;

		[ReadOnly]
		public ComponentLookup<AnimalCurrentLane> m_AnimalCurrentLane;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> m_TrainCurrentLane;

		[ReadOnly]
		public ComponentLookup<WatercraftCurrentLane> m_WatercraftCurrentLane;

		[ReadOnly]
		public ComponentLookup<AircraftCurrentLane> m_AircraftCurrentLane;

		public BufferTypeHandle<LaneObject> m_LaneObjectType;

		public ParallelWriter<Entity> m_LaneObjectQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<LaneObject> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LaneObject>(ref m_LaneObjectType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<LaneObject> val = bufferAccessor[i];
				int num = 0;
				for (int j = 0; j < val.Length; j++)
				{
					LaneObject laneObject = val[j];
					if (m_ParkedCarData.HasComponent(laneObject.m_LaneObject) || m_ParkedTrainData.HasComponent(laneObject.m_LaneObject))
					{
						val[num++] = laneObject;
					}
					else if (m_CarCurrentLaneData.HasComponent(laneObject.m_LaneObject) || m_CarTrailerLaneData.HasComponent(laneObject.m_LaneObject) || m_HumanCurrentLane.HasComponent(laneObject.m_LaneObject) || m_AnimalCurrentLane.HasComponent(laneObject.m_LaneObject) || m_TrainCurrentLane.HasComponent(laneObject.m_LaneObject) || m_WatercraftCurrentLane.HasComponent(laneObject.m_LaneObject) || m_AircraftCurrentLane.HasComponent(laneObject.m_LaneObject))
					{
						val[num++] = laneObject;
						m_LaneObjectQueue.Enqueue(laneObject.m_LaneObject);
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
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixLaneObjectsJob : IJob
	{
		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingData;

		[ReadOnly]
		public ComponentLookup<CarNavigation> m_CarNavigationData;

		[ReadOnly]
		public ComponentLookup<HumanNavigation> m_HumanNavigationData;

		[ReadOnly]
		public ComponentLookup<AnimalNavigation> m_AnimalNavigationData;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<WatercraftNavigation> m_WatercraftNavigationData;

		[ReadOnly]
		public ComponentLookup<AircraftNavigation> m_AircraftNavigationData;

		[ReadOnly]
		public ComponentLookup<OutOfControl> m_OutOfControlData;

		[ReadOnly]
		public ComponentLookup<Helicopter> m_HelicopterData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLane> m_TrackLaneData;

		[ReadOnly]
		public ComponentLookup<PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<HangaroundLocation> m_HangaroundLocationData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabLaneData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

		[ReadOnly]
		public ComponentLookup<TrainData> m_PrefabTrainData;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		public ComponentLookup<CarCurrentLane> m_CarCurrentLaneData;

		public ComponentLookup<CarTrailerLane> m_CarTrailerLaneData;

		public ComponentLookup<HumanCurrentLane> m_HumanCurrentLane;

		public ComponentLookup<AnimalCurrentLane> m_AnimalCurrentLane;

		public ComponentLookup<TrainCurrentLane> m_TrainCurrentLane;

		public ComponentLookup<WatercraftCurrentLane> m_WatercraftCurrentLane;

		public ComponentLookup<AircraftCurrentLane> m_AircraftCurrentLane;

		public BufferLookup<BlockedLane> m_BlockedLanes;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingObjectSearchTree;

		public NativeQueue<Entity> m_LaneObjectQueue;

		public LaneObjectCommandBuffer m_LaneObjectBuffer;

		public void Execute()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			if (m_LaneObjectQueue.Count == 0)
			{
				return;
			}
			NativeParallelHashSet<Entity> val = default(NativeParallelHashSet<Entity>);
			val._002Ector(m_LaneObjectQueue.Count, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<BlockedLane> tempBlockedLanes = default(NativeList<BlockedLane>);
			tempBlockedLanes._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
			Entity val2 = default(Entity);
			while (m_LaneObjectQueue.TryDequeue(ref val2))
			{
				if (val.Add(val2))
				{
					if (m_CarCurrentLaneData.HasComponent(val2))
					{
						UpdateCar(val2, tempBlockedLanes);
					}
					else if (m_CarTrailerLaneData.HasComponent(val2))
					{
						UpdateCarTrailer(val2, tempBlockedLanes);
					}
					else if (m_HumanCurrentLane.HasComponent(val2))
					{
						UpdateHuman(val2);
					}
					else if (m_AnimalCurrentLane.HasComponent(val2))
					{
						UpdateAnimal(val2);
					}
					else if (m_TrainCurrentLane.HasComponent(val2))
					{
						UpdateTrain(val2);
					}
					else if (m_WatercraftCurrentLane.HasComponent(val2))
					{
						UpdateWatercraft(val2);
					}
					else if (m_AircraftCurrentLane.HasComponent(val2))
					{
						UpdateAircraft(val2);
					}
				}
			}
			tempBlockedLanes.Dispose();
			val.Dispose();
		}

		private void UpdateAircraft(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[entity];
			AircraftNavigation navigation = m_AircraftNavigationData[entity];
			AircraftCurrentLane currentLane = m_AircraftCurrentLane[entity];
			PrefabRef prefabRef = m_PrefabRefData[entity];
			ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
			Moving moving = default(Moving);
			if (m_MovingData.HasComponent(entity))
			{
				moving = m_MovingData[entity];
			}
			AircraftNavigationHelpers.CurrentLaneCache currentLaneCache = new AircraftNavigationHelpers.CurrentLaneCache(ref currentLane, m_PrefabRefData, m_MovingObjectSearchTree);
			float num = 4f / 15f;
			bool flag = m_HelicopterData.HasComponent(entity);
			AircraftCurrentLane aircraftCurrentLane = currentLane;
			currentLane.m_Lane = Entity.Null;
			float3 val = transform.m_Position + moving.m_Velocity * (num * 2f);
			float num2 = 100f;
			Bounds3 bounds = default(Bounds3);
			((Bounds3)(ref bounds))._002Ector(val - num2, val + num2);
			AircraftNavigationHelpers.FindLaneIterator findLaneIterator = new AircraftNavigationHelpers.FindLaneIterator
			{
				m_Bounds = bounds,
				m_Position = val,
				m_MinDistance = num2,
				m_Result = currentLane,
				m_CarType = (flag ? RoadTypes.Helicopter : RoadTypes.Airplane),
				m_SubLanes = m_SubLanes,
				m_CarLaneData = m_CarLaneData,
				m_ConnectionLaneData = m_ConnectionLaneData,
				m_CurveData = m_CurveData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabCarLaneData = m_PrefabCarLaneData
			};
			m_NetSearchTree.Iterate<AircraftNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			currentLane = findLaneIterator.m_Result;
			if (aircraftCurrentLane.m_Lane == currentLane.m_Lane)
			{
				((float3)(ref currentLane.m_CurvePosition)).yz = ((float3)(ref aircraftCurrentLane.m_CurvePosition)).yz;
				currentLane.m_LaneFlags = aircraftCurrentLane.m_LaneFlags;
			}
			else
			{
				currentLane.m_LaneFlags |= AircraftLaneFlags.Obsolete;
			}
			currentLaneCache.CheckChanges(entity, ref currentLane, m_LaneObjectBuffer, m_LaneObjects, transform, moving, navigation, objectGeometryData);
			m_AircraftCurrentLane[entity] = currentLane;
		}

		private void UpdateWatercraft(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[entity];
			WatercraftNavigation navigation = m_WatercraftNavigationData[entity];
			WatercraftCurrentLane currentLane = m_WatercraftCurrentLane[entity];
			PrefabRef prefabRef = m_PrefabRefData[entity];
			ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
			Moving moving = default(Moving);
			if (m_MovingData.HasComponent(entity))
			{
				moving = m_MovingData[entity];
			}
			WatercraftNavigationHelpers.CurrentLaneCache currentLaneCache = new WatercraftNavigationHelpers.CurrentLaneCache(ref currentLane, m_EntityLookup, m_MovingObjectSearchTree);
			float num = 4f / 15f;
			WatercraftCurrentLane watercraftCurrentLane = currentLane;
			currentLane.m_Lane = Entity.Null;
			currentLane.m_ChangeLane = Entity.Null;
			float3 val = transform.m_Position + moving.m_Velocity * (num * 2f);
			float num2 = 100f;
			Bounds3 bounds = default(Bounds3);
			((Bounds3)(ref bounds))._002Ector(val - num2, val + num2);
			WatercraftNavigationHelpers.FindLaneIterator findLaneIterator = new WatercraftNavigationHelpers.FindLaneIterator
			{
				m_Bounds = bounds,
				m_Position = val,
				m_MinDistance = num2,
				m_Result = currentLane,
				m_CarType = RoadTypes.Watercraft,
				m_SubLanes = m_SubLanes,
				m_AreaNodes = m_AreaNodes,
				m_AreaTriangles = m_AreaTriangles,
				m_CarLaneData = m_CarLaneData,
				m_ConnectionLaneData = m_ConnectionLaneData,
				m_MasterLaneData = m_MasterLaneData,
				m_CurveData = m_CurveData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabCarLaneData = m_PrefabCarLaneData
			};
			m_NetSearchTree.Iterate<WatercraftNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			m_AreaSearchTree.Iterate<WatercraftNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			currentLane = findLaneIterator.m_Result;
			if (watercraftCurrentLane.m_Lane == currentLane.m_Lane)
			{
				if (m_PrefabRefData.HasComponent(watercraftCurrentLane.m_ChangeLane) && !m_DeletedData.HasComponent(watercraftCurrentLane.m_ChangeLane))
				{
					currentLane.m_ChangeLane = watercraftCurrentLane.m_ChangeLane;
				}
				((float3)(ref currentLane.m_CurvePosition)).yz = ((float3)(ref watercraftCurrentLane.m_CurvePosition)).yz;
				currentLane.m_LaneFlags = watercraftCurrentLane.m_LaneFlags;
			}
			else
			{
				currentLane.m_LaneFlags |= WatercraftLaneFlags.Obsolete;
			}
			currentLaneCache.CheckChanges(entity, ref currentLane, m_LaneObjectBuffer, m_LaneObjects, transform, moving, navigation, objectGeometryData);
			m_WatercraftCurrentLane[entity] = currentLane;
		}

		private void UpdateTrain(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[entity];
			Train train = m_TrainData[entity];
			TrainCurrentLane currentLane = m_TrainCurrentLane[entity];
			PrefabRef prefabRef = m_PrefabRefData[entity];
			TrainData prefabTrainData = m_PrefabTrainData[prefabRef.m_Prefab];
			Moving moving = default(Moving);
			if (m_MovingData.HasComponent(entity))
			{
				moving = m_MovingData[entity];
			}
			TrainNavigationHelpers.CurrentLaneCache currentLaneCache = new TrainNavigationHelpers.CurrentLaneCache(ref currentLane, m_LaneData);
			float num = 4f / 15f;
			ref float3 position = ref transform.m_Position;
			position += moving.m_Velocity * (num * 2f);
			TrainCurrentLane trainCurrentLane = currentLane;
			currentLane.m_Front.m_Lane = Entity.Null;
			currentLane.m_Rear.m_Lane = Entity.Null;
			currentLane.m_FrontCache.m_Lane = Entity.Null;
			currentLane.m_RearCache.m_Lane = Entity.Null;
			VehicleUtils.CalculateTrainNavigationPivots(transform, prefabTrainData, out var pivot, out var pivot2);
			if ((train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0)
			{
				CommonUtils.Swap(ref pivot, ref pivot2);
			}
			float num2 = 100f;
			Bounds3 bounds = MathUtils.Expand(MathUtils.Bounds(pivot, pivot2), float3.op_Implicit(num2));
			TrainNavigationHelpers.FindLaneIterator findLaneIterator = new TrainNavigationHelpers.FindLaneIterator
			{
				m_Bounds = bounds,
				m_FrontPivot = pivot,
				m_RearPivot = pivot2,
				m_MinDistance = float2.op_Implicit(num2),
				m_Result = currentLane,
				m_TrackType = prefabTrainData.m_TrackType,
				m_SubLanes = m_SubLanes,
				m_TrackLaneData = m_TrackLaneData,
				m_CurveData = m_CurveData,
				m_ConnectionLaneData = m_ConnectionLaneData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabTrackLaneData = m_PrefabTrackLaneData
			};
			m_NetSearchTree.Iterate<TrainNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			currentLane = findLaneIterator.m_Result;
			if (trainCurrentLane.m_Front.m_Lane == currentLane.m_Front.m_Lane)
			{
				if (m_PrefabRefData.HasComponent(trainCurrentLane.m_FrontCache.m_Lane) && !m_DeletedData.HasComponent(trainCurrentLane.m_FrontCache.m_Lane))
				{
					currentLane.m_FrontCache.m_Lane = trainCurrentLane.m_FrontCache.m_Lane;
					currentLane.m_FrontCache.m_CurvePosition = trainCurrentLane.m_FrontCache.m_CurvePosition;
					currentLane.m_FrontCache.m_LaneFlags = trainCurrentLane.m_FrontCache.m_LaneFlags;
				}
				((float4)(ref currentLane.m_Front.m_CurvePosition)).xzw = ((float4)(ref trainCurrentLane.m_Front.m_CurvePosition)).xzw;
				currentLane.m_Front.m_LaneFlags = trainCurrentLane.m_Front.m_LaneFlags;
			}
			else
			{
				currentLane.m_Front.m_LaneFlags |= TrainLaneFlags.Obsolete;
			}
			if (trainCurrentLane.m_Rear.m_Lane == currentLane.m_Rear.m_Lane)
			{
				if (m_PrefabRefData.HasComponent(trainCurrentLane.m_RearCache.m_Lane) && !m_DeletedData.HasComponent(trainCurrentLane.m_RearCache.m_Lane))
				{
					currentLane.m_RearCache.m_Lane = trainCurrentLane.m_RearCache.m_Lane;
					currentLane.m_RearCache.m_CurvePosition = trainCurrentLane.m_RearCache.m_CurvePosition;
					currentLane.m_RearCache.m_LaneFlags = trainCurrentLane.m_RearCache.m_LaneFlags;
				}
				((float4)(ref currentLane.m_Rear.m_CurvePosition)).xzw = ((float4)(ref trainCurrentLane.m_Rear.m_CurvePosition)).xzw;
				currentLane.m_Rear.m_LaneFlags = trainCurrentLane.m_Rear.m_LaneFlags;
			}
			else
			{
				currentLane.m_Rear.m_LaneFlags |= TrainLaneFlags.Obsolete;
			}
			currentLaneCache.CheckChanges(entity, currentLane, m_LaneObjectBuffer);
			m_TrainCurrentLane[entity] = currentLane;
		}

		private void UpdateHuman(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[entity];
			HumanNavigation navigation = m_HumanNavigationData[entity];
			HumanCurrentLane currentLane = m_HumanCurrentLane[entity];
			PrefabRef prefabRef = m_PrefabRefData[entity];
			ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
			Moving moving = default(Moving);
			if (m_MovingData.HasComponent(entity))
			{
				moving = m_MovingData[entity];
			}
			HumanNavigationHelpers.CurrentLaneCache currentLaneCache = new HumanNavigationHelpers.CurrentLaneCache(ref currentLane, m_EntityLookup, m_MovingObjectSearchTree);
			HumanCurrentLane humanCurrentLane = currentLane;
			float3 position = transform.m_Position;
			Bounds3 bounds = default(Bounds3);
			((Bounds3)(ref bounds))._002Ector(position - 100f, position + 100f);
			HumanNavigationHelpers.FindLaneIterator findLaneIterator = new HumanNavigationHelpers.FindLaneIterator
			{
				m_Bounds = bounds,
				m_Position = position,
				m_MinDistance = 1000f,
				m_Result = currentLane,
				m_SubLanes = m_SubLanes,
				m_AreaNodes = m_AreaNodes,
				m_AreaTriangles = m_AreaTriangles,
				m_PedestrianLaneData = m_PedestrianLaneData,
				m_ConnectionLaneData = m_ConnectionLaneData,
				m_CurveData = m_CurveData,
				m_HangaroundLocationData = m_HangaroundLocationData
			};
			m_NetSearchTree.Iterate<HumanNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			m_StaticObjectSearchTree.Iterate<HumanNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			m_AreaSearchTree.Iterate<HumanNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			currentLane = findLaneIterator.m_Result;
			if (humanCurrentLane.m_Lane == currentLane.m_Lane)
			{
				currentLane.m_CurvePosition.y = humanCurrentLane.m_CurvePosition.y;
				currentLane.m_Flags = humanCurrentLane.m_Flags;
			}
			else
			{
				currentLane.m_Flags |= CreatureLaneFlags.Obsolete;
			}
			currentLaneCache.CheckChanges(entity, ref currentLane, m_LaneObjectBuffer, m_LaneObjects, transform, moving, navigation, objectGeometryData);
			m_HumanCurrentLane[entity] = currentLane;
		}

		private void UpdateAnimal(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[entity];
			AnimalNavigation navigation = m_AnimalNavigationData[entity];
			AnimalCurrentLane currentLane = m_AnimalCurrentLane[entity];
			PrefabRef prefabRef = m_PrefabRefData[entity];
			ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
			Moving moving = default(Moving);
			if (m_MovingData.HasComponent(entity))
			{
				moving = m_MovingData[entity];
			}
			AnimalNavigationHelpers.CurrentLaneCache currentLaneCache = new AnimalNavigationHelpers.CurrentLaneCache(ref currentLane, m_PrefabRefData, m_MovingObjectSearchTree);
			AnimalCurrentLane animalCurrentLane = currentLane;
			float3 position = transform.m_Position;
			Bounds3 bounds = default(Bounds3);
			((Bounds3)(ref bounds))._002Ector(position - 100f, position + 100f);
			AnimalNavigationHelpers.FindLaneIterator findLaneIterator = new AnimalNavigationHelpers.FindLaneIterator
			{
				m_Bounds = bounds,
				m_Position = position,
				m_MinDistance = 1000f,
				m_Result = currentLane,
				m_SubLanes = m_SubLanes,
				m_AreaNodes = m_AreaNodes,
				m_AreaTriangles = m_AreaTriangles,
				m_PedestrianLaneData = m_PedestrianLaneData,
				m_ConnectionLaneData = m_ConnectionLaneData,
				m_CurveData = m_CurveData,
				m_HangaroundLocationData = m_HangaroundLocationData
			};
			m_NetSearchTree.Iterate<AnimalNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			m_StaticObjectSearchTree.Iterate<AnimalNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			m_AreaSearchTree.Iterate<AnimalNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			currentLane = findLaneIterator.m_Result;
			if (animalCurrentLane.m_Lane == currentLane.m_Lane)
			{
				if (m_PrefabRefData.HasComponent(animalCurrentLane.m_NextLane) && !m_DeletedData.HasComponent(animalCurrentLane.m_NextLane))
				{
					currentLane.m_NextLane = animalCurrentLane.m_NextLane;
					currentLane.m_NextPosition = animalCurrentLane.m_NextPosition;
					currentLane.m_NextFlags = animalCurrentLane.m_NextFlags;
				}
				currentLane.m_CurvePosition.y = animalCurrentLane.m_CurvePosition.y;
				currentLane.m_Flags = animalCurrentLane.m_Flags;
			}
			else
			{
				currentLane.m_Flags |= CreatureLaneFlags.Obsolete;
			}
			currentLaneCache.CheckChanges(entity, ref currentLane, m_LaneObjectBuffer, m_LaneObjects, transform, moving, navigation, objectGeometryData);
			m_AnimalCurrentLane[entity] = currentLane;
		}

		private void UpdateCar(Entity entity, NativeList<BlockedLane> tempBlockedLanes)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[entity];
			CarNavigation navigation = m_CarNavigationData[entity];
			CarCurrentLane currentLane = m_CarCurrentLaneData[entity];
			PrefabRef prefabRef = m_PrefabRefData[entity];
			DynamicBuffer<BlockedLane> blockedLanes = m_BlockedLanes[entity];
			ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
			Moving moving = default(Moving);
			if (m_MovingData.HasComponent(entity))
			{
				moving = m_MovingData[entity];
			}
			CarNavigationHelpers.CurrentLaneCache currentLaneCache = new CarNavigationHelpers.CurrentLaneCache(ref currentLane, blockedLanes, m_EntityLookup, m_MovingObjectSearchTree);
			if (m_OutOfControlData.HasComponent(entity))
			{
				float3 position = transform.m_Position;
				float3 val = math.forward(transform.m_Rotation);
				Segment val2 = default(Segment);
				((Segment)(ref val2))._002Ector(position - val * math.max(0.1f, 0f - objectGeometryData.m_Bounds.min.z - objectGeometryData.m_Size.x * 0.5f), position + val * math.max(0.1f, objectGeometryData.m_Bounds.max.z - objectGeometryData.m_Size.x * 0.5f));
				float num = objectGeometryData.m_Size.x * 0.5f;
				Bounds3 bounds = MathUtils.Expand(MathUtils.Bounds(val2), float3.op_Implicit(num));
				CarNavigationHelpers.FindBlockedLanesIterator findBlockedLanesIterator = new CarNavigationHelpers.FindBlockedLanesIterator
				{
					m_Bounds = bounds,
					m_Line = val2,
					m_Radius = num,
					m_BlockedLanes = tempBlockedLanes,
					m_SubLanes = m_SubLanes,
					m_MasterLaneData = m_MasterLaneData,
					m_CurveData = m_CurveData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabLaneData = m_PrefabLaneData
				};
				m_NetSearchTree.Iterate<CarNavigationHelpers.FindBlockedLanesIterator>(ref findBlockedLanesIterator, 0);
			}
			else
			{
				float num2 = 4f / 15f;
				CarCurrentLane carCurrentLane = currentLane;
				currentLane.m_Lane = Entity.Null;
				currentLane.m_ChangeLane = Entity.Null;
				float3 val3 = transform.m_Position + moving.m_Velocity * (num2 * 2f);
				float num3 = 100f;
				Bounds3 bounds2 = default(Bounds3);
				((Bounds3)(ref bounds2))._002Ector(val3 - num3, val3 + num3);
				CarNavigationHelpers.FindLaneIterator findLaneIterator = new CarNavigationHelpers.FindLaneIterator
				{
					m_Bounds = bounds2,
					m_Position = val3,
					m_MinDistance = num3,
					m_Result = currentLane,
					m_CarType = RoadTypes.Car,
					m_SubLanes = m_SubLanes,
					m_AreaNodes = m_AreaNodes,
					m_AreaTriangles = m_AreaTriangles,
					m_CarLaneData = m_CarLaneData,
					m_MasterLaneData = m_MasterLaneData,
					m_ConnectionLaneData = m_ConnectionLaneData,
					m_CurveData = m_CurveData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabCarLaneData = m_PrefabCarLaneData
				};
				m_NetSearchTree.Iterate<CarNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
				m_StaticObjectSearchTree.Iterate<CarNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
				m_AreaSearchTree.Iterate<CarNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
				currentLane = findLaneIterator.m_Result;
				if (carCurrentLane.m_Lane == currentLane.m_Lane)
				{
					if (m_PrefabRefData.HasComponent(carCurrentLane.m_ChangeLane) && !m_DeletedData.HasComponent(carCurrentLane.m_ChangeLane))
					{
						currentLane.m_ChangeLane = carCurrentLane.m_ChangeLane;
					}
					((float3)(ref currentLane.m_CurvePosition)).yz = ((float3)(ref carCurrentLane.m_CurvePosition)).yz;
					currentLane.m_LaneFlags = carCurrentLane.m_LaneFlags;
				}
				else
				{
					currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.Obsolete;
				}
			}
			currentLaneCache.CheckChanges(entity, ref currentLane, tempBlockedLanes, m_LaneObjectBuffer, m_LaneObjects, transform, moving, navigation, objectGeometryData);
			m_CarCurrentLaneData[entity] = currentLane;
			tempBlockedLanes.Clear();
		}

		private void UpdateCarTrailer(Entity entity, NativeList<BlockedLane> tempBlockedLanes)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[entity];
			Controller controller = m_ControllerData[entity];
			CarTrailerLane trailerLane = m_CarTrailerLaneData[entity];
			PrefabRef prefabRef = m_PrefabRefData[entity];
			DynamicBuffer<BlockedLane> blockedLanes = m_BlockedLanes[entity];
			ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
			CarNavigation tractorNavigation = default(CarNavigation);
			if (m_CarNavigationData.HasComponent(controller.m_Controller))
			{
				tractorNavigation = m_CarNavigationData[controller.m_Controller];
			}
			Moving moving = default(Moving);
			if (m_MovingData.HasComponent(entity))
			{
				moving = m_MovingData[entity];
			}
			CarNavigationHelpers.TrailerLaneCache trailerLaneCache = new CarNavigationHelpers.TrailerLaneCache(ref trailerLane, blockedLanes, m_PrefabRefData, m_MovingObjectSearchTree);
			if (m_OutOfControlData.HasComponent(entity))
			{
				float3 position = transform.m_Position;
				float3 val = math.forward(transform.m_Rotation);
				Segment val2 = default(Segment);
				((Segment)(ref val2))._002Ector(position - val * math.max(0.1f, 0f - objectGeometryData.m_Bounds.min.z - objectGeometryData.m_Size.x * 0.5f), position + val * math.max(0.1f, objectGeometryData.m_Bounds.max.z - objectGeometryData.m_Size.x * 0.5f));
				float num = objectGeometryData.m_Size.x * 0.5f;
				Bounds3 bounds = MathUtils.Expand(MathUtils.Bounds(val2), float3.op_Implicit(num));
				CarNavigationHelpers.FindBlockedLanesIterator findBlockedLanesIterator = new CarNavigationHelpers.FindBlockedLanesIterator
				{
					m_Bounds = bounds,
					m_Line = val2,
					m_Radius = num,
					m_BlockedLanes = tempBlockedLanes,
					m_SubLanes = m_SubLanes,
					m_MasterLaneData = m_MasterLaneData,
					m_CurveData = m_CurveData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabLaneData = m_PrefabLaneData
				};
				m_NetSearchTree.Iterate<CarNavigationHelpers.FindBlockedLanesIterator>(ref findBlockedLanesIterator, 0);
			}
			else
			{
				float num2 = 4f / 15f;
				float3 val3 = transform.m_Position + moving.m_Velocity * (num2 * 2f);
				float num3 = 100f;
				Bounds3 bounds2 = default(Bounds3);
				((Bounds3)(ref bounds2))._002Ector(val3 - num3, val3 + num3);
				CarNavigationHelpers.FindLaneIterator findLaneIterator = new CarNavigationHelpers.FindLaneIterator
				{
					m_Bounds = bounds2,
					m_Position = val3,
					m_MinDistance = num3,
					m_CarType = RoadTypes.Car,
					m_SubLanes = m_SubLanes,
					m_AreaNodes = m_AreaNodes,
					m_AreaTriangles = m_AreaTriangles,
					m_CarLaneData = m_CarLaneData,
					m_MasterLaneData = m_MasterLaneData,
					m_ConnectionLaneData = m_ConnectionLaneData,
					m_CurveData = m_CurveData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabCarLaneData = m_PrefabCarLaneData
				};
				m_NetSearchTree.Iterate<CarNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
				m_StaticObjectSearchTree.Iterate<CarNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
				m_AreaSearchTree.Iterate<CarNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
				if (findLaneIterator.m_Result.m_Lane != trailerLane.m_Lane)
				{
					trailerLane.m_Lane = findLaneIterator.m_Result.m_Lane;
					trailerLane.m_CurvePosition = ((float3)(ref findLaneIterator.m_Result.m_CurvePosition)).xy;
					trailerLane.m_NextLane = Entity.Null;
					trailerLane.m_NextPosition = default(float2);
				}
				else
				{
					trailerLane.m_CurvePosition.x = findLaneIterator.m_Result.m_CurvePosition.x;
					if (!m_PrefabRefData.HasComponent(trailerLane.m_NextLane) || m_DeletedData.HasComponent(trailerLane.m_NextLane))
					{
						trailerLane.m_NextLane = Entity.Null;
						trailerLane.m_NextPosition = default(float2);
					}
				}
			}
			trailerLaneCache.CheckChanges(entity, ref trailerLane, tempBlockedLanes, m_LaneObjectBuffer, m_LaneObjects, transform, moving, tractorNavigation, objectGeometryData);
			m_CarTrailerLaneData[entity] = trailerLane;
			tempBlockedLanes.Clear();
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarTrailerLane> __Game_Vehicles_CarTrailerLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup;

		public BufferTypeHandle<LaneObject> __Game_Net_LaneObject_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarNavigation> __Game_Vehicles_CarNavigation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HumanNavigation> __Game_Creatures_HumanNavigation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AnimalNavigation> __Game_Creatures_AnimalNavigation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WatercraftNavigation> __Game_Vehicles_WatercraftNavigation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AircraftNavigation> __Game_Vehicles_AircraftNavigation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OutOfControl> __Game_Vehicles_OutOfControl_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Helicopter> __Game_Vehicles_Helicopter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLane> __Game_Net_TrackLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HangaroundLocation> __Game_Areas_HangaroundLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainData> __Game_Prefabs_TrainData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		public ComponentLookup<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RW_ComponentLookup;

		public ComponentLookup<CarTrailerLane> __Game_Vehicles_CarTrailerLane_RW_ComponentLookup;

		public ComponentLookup<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RW_ComponentLookup;

		public ComponentLookup<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RW_ComponentLookup;

		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RW_ComponentLookup;

		public ComponentLookup<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RW_ComponentLookup;

		public ComponentLookup<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RW_ComponentLookup;

		public BufferLookup<BlockedLane> __Game_Objects_BlockedLane_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(true);
			__Game_Vehicles_CarCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarCurrentLane>(true);
			__Game_Vehicles_CarTrailerLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarTrailerLane>(true);
			__Game_Creatures_HumanCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanCurrentLane>(true);
			__Game_Creatures_AnimalCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalCurrentLane>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(true);
			__Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftCurrentLane>(true);
			__Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AircraftCurrentLane>(true);
			__Game_Net_LaneObject_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LaneObject>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Vehicles_CarNavigation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarNavigation>(true);
			__Game_Creatures_HumanNavigation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanNavigation>(true);
			__Game_Creatures_AnimalNavigation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalNavigation>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Vehicles_WatercraftNavigation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftNavigation>(true);
			__Game_Vehicles_AircraftNavigation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AircraftNavigation>(true);
			__Game_Vehicles_OutOfControl_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutOfControl>(true);
			__Game_Vehicles_Helicopter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Helicopter>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PedestrianLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ConnectionLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Areas_HangaroundLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HangaroundLocation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Prefabs_TrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainData>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Vehicles_CarCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarCurrentLane>(false);
			__Game_Vehicles_CarTrailerLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarTrailerLane>(false);
			__Game_Creatures_HumanCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanCurrentLane>(false);
			__Game_Creatures_AnimalCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalCurrentLane>(false);
			__Game_Vehicles_TrainCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(false);
			__Game_Vehicles_WatercraftCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftCurrentLane>(false);
			__Game_Vehicles_AircraftCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AircraftCurrentLane>(false);
			__Game_Objects_BlockedLane_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BlockedLane>(false);
		}
	}

	private ModificationBarrier5 m_ModificationBarrier;

	private SearchSystem m_NetSearchSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private EntityQuery m_LaneQuery;

	private LaneObjectUpdater m_LaneObjectUpdater;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<LaneObject>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<ParkingLane>()
		};
		array[0] = val;
		m_LaneQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_LaneObjectUpdater = new LaneObjectUpdater((SystemBase)(object)this);
		((ComponentSystemBase)this).RequireForUpdate(m_LaneQuery);
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
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<Entity> laneObjectQueue = default(NativeQueue<Entity>);
		laneObjectQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		CollectLaneObjectsJob collectLaneObjectsJob = new CollectLaneObjectsJob
		{
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarCurrentLaneData = InternalCompilerInterface.GetComponentLookup<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarTrailerLaneData = InternalCompilerInterface.GetComponentLookup<CarTrailerLane>(ref __TypeHandle.__Game_Vehicles_CarTrailerLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HumanCurrentLane = InternalCompilerInterface.GetComponentLookup<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalCurrentLane = InternalCompilerInterface.GetComponentLookup<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainCurrentLane = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftCurrentLane = InternalCompilerInterface.GetComponentLookup<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftCurrentLane = InternalCompilerInterface.GetComponentLookup<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjectType = InternalCompilerInterface.GetBufferTypeHandle<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjectQueue = laneObjectQueue.AsParallelWriter()
		};
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle dependencies4;
		FixLaneObjectsJob obj = new FixLaneObjectsJob
		{
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarNavigationData = InternalCompilerInterface.GetComponentLookup<CarNavigation>(ref __TypeHandle.__Game_Vehicles_CarNavigation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HumanNavigationData = InternalCompilerInterface.GetComponentLookup<HumanNavigation>(ref __TypeHandle.__Game_Creatures_HumanNavigation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalNavigationData = InternalCompilerInterface.GetComponentLookup<AnimalNavigation>(ref __TypeHandle.__Game_Creatures_AnimalNavigation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftNavigationData = InternalCompilerInterface.GetComponentLookup<WatercraftNavigation>(ref __TypeHandle.__Game_Vehicles_WatercraftNavigation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftNavigationData = InternalCompilerInterface.GetComponentLookup<AircraftNavigation>(ref __TypeHandle.__Game_Vehicles_AircraftNavigation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutOfControlData = InternalCompilerInterface.GetComponentLookup<OutOfControl>(ref __TypeHandle.__Game_Vehicles_OutOfControl_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HelicopterData = InternalCompilerInterface.GetComponentLookup<Helicopter>(ref __TypeHandle.__Game_Vehicles_Helicopter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HangaroundLocationData = InternalCompilerInterface.GetComponentLookup<HangaroundLocation>(ref __TypeHandle.__Game_Areas_HangaroundLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainData = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarCurrentLaneData = InternalCompilerInterface.GetComponentLookup<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarTrailerLaneData = InternalCompilerInterface.GetComponentLookup<CarTrailerLane>(ref __TypeHandle.__Game_Vehicles_CarTrailerLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HumanCurrentLane = InternalCompilerInterface.GetComponentLookup<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalCurrentLane = InternalCompilerInterface.GetComponentLookup<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainCurrentLane = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftCurrentLane = InternalCompilerInterface.GetComponentLookup<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftCurrentLane = InternalCompilerInterface.GetComponentLookup<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BlockedLanes = InternalCompilerInterface.GetBufferLookup<BlockedLane>(ref __TypeHandle.__Game_Objects_BlockedLane_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies2),
			m_StaticObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies3),
			m_MovingObjectSearchTree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: true, out dependencies4),
			m_LaneObjectQueue = laneObjectQueue,
			m_LaneObjectBuffer = m_LaneObjectUpdater.Begin((Allocator)3)
		};
		JobHandle val = JobChunkExtensions.ScheduleParallel<CollectLaneObjectsJob>(collectLaneObjectsJob, m_LaneQuery, ((SystemBase)this).Dependency);
		JobHandle val2 = IJobExtensions.Schedule<FixLaneObjectsJob>(obj, JobHandle.CombineDependencies(val, dependencies2, JobHandle.CombineDependencies(dependencies, dependencies3, dependencies4)));
		laneObjectQueue.Dispose(val2);
		m_NetSearchSystem.AddNetSearchTreeReader(val2);
		m_AreaSearchSystem.AddSearchTreeReader(val2);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val2);
		m_ObjectSearchSystem.AddMovingSearchTreeReader(val2);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
		JobHandle dependency = m_LaneObjectUpdater.Apply((SystemBase)(object)this, val2);
		((SystemBase)this).Dependency = dependency;
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
	public FixLaneObjectsSystem()
	{
	}
}

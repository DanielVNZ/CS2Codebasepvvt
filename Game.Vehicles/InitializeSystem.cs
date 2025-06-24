using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Vehicles;

[CompilerGenerated]
public class InitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct TreeFixJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<CarCurrentLane> m_CarCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<CarTrailerLane> m_CarTrailerLaneType;

		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		public BufferLookup<LaneObject> m_LaneObjects;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType))
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CarCurrentLane> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CarCurrentLaneType);
			NativeArray<CarTrailerLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerLane>(ref m_CarTrailerLaneType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				CarCurrentLane carCurrentLane = nativeArray2[i];
				if (m_LaneObjects.HasBuffer(carCurrentLane.m_Lane))
				{
					DynamicBuffer<LaneObject> val2 = m_LaneObjects[carCurrentLane.m_Lane];
					if (!CollectionUtils.ContainsValue<LaneObject>(val2, new LaneObject(val)))
					{
						NetUtils.AddLaneObject(val2, val, ((float3)(ref carCurrentLane.m_CurvePosition)).xy);
					}
					m_SearchTree.TryRemove(val);
				}
			}
			for (int j = 0; j < nativeArray3.Length; j++)
			{
				Entity val3 = nativeArray[j];
				CarTrailerLane carTrailerLane = nativeArray3[j];
				if (m_LaneObjects.HasBuffer(carTrailerLane.m_Lane))
				{
					DynamicBuffer<LaneObject> val4 = m_LaneObjects[carTrailerLane.m_Lane];
					if (!CollectionUtils.ContainsValue<LaneObject>(val4, new LaneObject(val3)))
					{
						NetUtils.AddLaneObject(val4, val3, ((float2)(ref carTrailerLane.m_CurvePosition)).xy);
					}
					m_SearchTree.TryRemove(val3);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct InitializeVehiclesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Train> m_TrainType;

		[ReadOnly]
		public ComponentTypeHandle<TripSource> m_TripSourceType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<Helicopter> m_HelicopterType;

		[ReadOnly]
		public ComponentTypeHandle<Car> m_CarType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> m_LayoutElementType;

		public ComponentTypeHandle<CarNavigation> m_CarNavigationType;

		public ComponentTypeHandle<CarCurrentLane> m_CarCurrentLaneType;

		public ComponentTypeHandle<WatercraftNavigation> m_WatercraftNavigationType;

		public ComponentTypeHandle<WatercraftCurrentLane> m_WatercraftCurrentLaneType;

		public ComponentTypeHandle<AircraftNavigation> m_AircraftNavigationType;

		public ComponentTypeHandle<AircraftCurrentLane> m_AircraftCurrentLaneType;

		public ComponentTypeHandle<ParkedCar> m_ParkedCarType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<DeliveryTruck> m_DeliveryTruckData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneLaneData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<RouteLane> m_RouteLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<TrainData> m_PrefabTrainData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<CarTractorData> m_PrefabCarTractorData;

		[ReadOnly]
		public ComponentLookup<CarTrailerData> m_PrefabCarTrailerData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> m_SpawnLocations;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Transform> m_TransformData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<TrainCurrentLane> m_TrainCurrentLaneData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<TrainNavigation> m_TrainNavigationData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CarTrailerLane> m_CarTrailerLaneData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<TrainBogieFrame> m_TrainBogieFrames;

		[NativeDisableParallelForRestriction]
		public BufferLookup<MeshBatch> m_MeshBatches;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_065c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0874: Unknown result type (might be due to invalid IL or missing references)
			//IL_0879: Unknown result type (might be due to invalid IL or missing references)
			//IL_0676: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b77: Unknown result type (might be due to invalid IL or missing references)
			//IL_088e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0893: Unknown result type (might be due to invalid IL or missing references)
			//IL_089c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0824: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0724: Unknown result type (might be due to invalid IL or missing references)
			//IL_0729: Unknown result type (might be due to invalid IL or missing references)
			//IL_072e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0907: Unknown result type (might be due to invalid IL or missing references)
			//IL_0911: Unknown result type (might be due to invalid IL or missing references)
			//IL_0916: Unknown result type (might be due to invalid IL or missing references)
			//IL_0919: Unknown result type (might be due to invalid IL or missing references)
			//IL_091d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0944: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_074e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
			//IL_095b: Unknown result type (might be due to invalid IL or missing references)
			//IL_096f: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0986: Unknown result type (might be due to invalid IL or missing references)
			//IL_099a: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09de: Unknown result type (might be due to invalid IL or missing references)
			//IL_078a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<CarNavigation> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarNavigation>(ref m_CarNavigationType);
			if (nativeArray2.Length != 0)
			{
				NativeArray<Car> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Car>(ref m_CarType);
				NativeArray<CarCurrentLane> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CarCurrentLaneType);
				NativeArray<TripSource> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TripSource>(ref m_TripSourceType);
				NativeArray<PathOwner> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
				NativeArray<PrefabRef> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<LayoutElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LayoutElement>(ref m_LayoutElementType);
				BufferAccessor<PathElement> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
				bool flag = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
				Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity val = nativeArray[i];
					Car car = nativeArray3[i];
					CarCurrentLane carCurrentLane = nativeArray4[i];
					CarNavigation carNavigation = nativeArray2[i];
					PrefabRef prefabRef = nativeArray7[i];
					bool flag2 = math.asuint(carNavigation.m_MaxSpeed) >> 31 != 0 && (car.m_Flags & CarFlags.CannotReverse) != 0;
					bool flag3 = false;
					if (flag && nativeArray5.Length != 0)
					{
						TripSource tripSource = nativeArray5[i];
						PathOwner pathOwner = nativeArray6[i];
						DynamicBuffer<PathElement> path = bufferAccessor2[i];
						InitializeRoadVehicle(ref random, val, RoadTypes.Car, tripSource, pathOwner, prefabRef, path);
						if (carCurrentLane.m_Lane == Entity.Null && path.Length > pathOwner.m_ElementIndex)
						{
							PathElement pathElement = path[pathOwner.m_ElementIndex];
							CarLaneFlags carLaneFlags = CarLaneFlags.FixedLane;
							if (m_ConnectionLaneData.TryGetComponent(pathElement.m_Target, ref connectionLane))
							{
								carLaneFlags = (((connectionLane.m_Flags & ConnectionLaneFlags.Area) == 0) ? (carLaneFlags | CarLaneFlags.Connection) : (carLaneFlags | CarLaneFlags.Area));
							}
							carCurrentLane = new CarCurrentLane(pathElement, carLaneFlags);
						}
					}
					else if (flag2)
					{
						Transform transform = m_TransformData[val];
						CarData carData = m_PrefabCarData[prefabRef.m_Prefab];
						float3 val2 = transform.m_Position;
						if (carData.m_PivotOffset < 0f)
						{
							val2 += math.rotate(transform.m_Rotation, new float3(0f, 0f, carData.m_PivotOffset));
						}
						float3 val3 = carNavigation.m_TargetPosition - val2;
						if (MathUtils.TryNormalize(ref val3))
						{
							transform.m_Rotation = quaternion.LookRotationSafe(val3, math.up());
							m_TransformData[val] = transform;
							ResetMeshBatches(val);
							flag3 = true;
						}
					}
					if (((flag && nativeArray5.Length != 0) || flag2) && bufferAccessor.Length != 0)
					{
						Transform transform2 = m_TransformData[val];
						DynamicBuffer<LayoutElement> val4 = bufferAccessor[i];
						CarTractorData carTractorData = m_PrefabCarTractorData[prefabRef.m_Prefab];
						for (int j = 1; j < val4.Length; j++)
						{
							Entity vehicle = val4[j].m_Vehicle;
							CarTrailerLane carTrailerLane = m_CarTrailerLaneData[vehicle];
							PrefabRef prefabRef2 = m_PrefabRefData[vehicle];
							CarTrailerData carTrailerData = m_PrefabCarTrailerData[prefabRef2.m_Prefab];
							Transform transform3 = transform2;
							ref float3 position = ref transform3.m_Position;
							position += math.rotate(transform2.m_Rotation, carTractorData.m_AttachPosition);
							ref float3 position2 = ref transform3.m_Position;
							position2 -= math.rotate(transform3.m_Rotation, carTrailerData.m_AttachPosition);
							m_TransformData[vehicle] = transform3;
							if (carTrailerLane.m_Lane == Entity.Null)
							{
								m_CarTrailerLaneData[vehicle] = new CarTrailerLane(carCurrentLane);
							}
							if (flag3)
							{
								ResetMeshBatches(vehicle);
							}
							if (j + 1 < val4.Length)
							{
								transform2 = transform3;
								carTractorData = m_PrefabCarTractorData[prefabRef2.m_Prefab];
							}
						}
					}
					carCurrentLane.m_LanePosition = ((Random)(ref random)).NextFloat(-0.25f, 0.25f);
					if (m_TransformData.HasComponent(carCurrentLane.m_Lane))
					{
						carCurrentLane.m_LaneFlags |= CarLaneFlags.TransformTarget;
					}
					nativeArray2[i] = new CarNavigation
					{
						m_TargetPosition = m_TransformData[val].m_Position
					};
					nativeArray4[i] = carCurrentLane;
				}
				return;
			}
			NativeArray<WatercraftNavigation> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftNavigation>(ref m_WatercraftNavigationType);
			if (nativeArray8.Length != 0)
			{
				NativeArray<WatercraftCurrentLane> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftCurrentLane>(ref m_WatercraftCurrentLaneType);
				NativeArray<TripSource> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TripSource>(ref m_TripSourceType);
				NativeArray<PathOwner> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
				NativeArray<PrefabRef> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<PathElement> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
				bool flag4 = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
				Game.Net.ConnectionLane connectionLane2 = default(Game.Net.ConnectionLane);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Entity val5 = nativeArray[k];
					WatercraftCurrentLane watercraftCurrentLane = nativeArray9[k];
					PrefabRef prefabRef3 = nativeArray12[k];
					WatercraftNavigation watercraftNavigation = default(WatercraftNavigation);
					if (flag4 && nativeArray10.Length != 0)
					{
						TripSource tripSource2 = nativeArray10[k];
						PathOwner pathOwner2 = nativeArray11[k];
						DynamicBuffer<PathElement> path2 = bufferAccessor3[k];
						InitializeRoadVehicle(ref random, val5, RoadTypes.Watercraft, tripSource2, pathOwner2, prefabRef3, path2);
						if (watercraftCurrentLane.m_Lane == Entity.Null && path2.Length > pathOwner2.m_ElementIndex)
						{
							PathElement pathElement2 = path2[pathOwner2.m_ElementIndex];
							WatercraftLaneFlags watercraftLaneFlags = WatercraftLaneFlags.FixedLane;
							if (m_ConnectionLaneData.TryGetComponent(pathElement2.m_Target, ref connectionLane2))
							{
								watercraftLaneFlags = (((connectionLane2.m_Flags & ConnectionLaneFlags.Area) == 0) ? (watercraftLaneFlags | WatercraftLaneFlags.Connection) : (watercraftLaneFlags | WatercraftLaneFlags.Area));
							}
							watercraftCurrentLane = new WatercraftCurrentLane(pathElement2, watercraftLaneFlags);
						}
					}
					if (m_TransformData.HasComponent(watercraftCurrentLane.m_Lane))
					{
						watercraftCurrentLane.m_LaneFlags |= WatercraftLaneFlags.TransformTarget;
					}
					watercraftNavigation.m_TargetPosition = m_TransformData[val5].m_Position;
					watercraftNavigation.m_TargetDirection = default(float3);
					nativeArray9[k] = watercraftCurrentLane;
					nativeArray8[k] = watercraftNavigation;
				}
				return;
			}
			NativeArray<AircraftNavigation> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftNavigation>(ref m_AircraftNavigationType);
			if (nativeArray13.Length != 0)
			{
				NativeArray<AircraftCurrentLane> nativeArray14 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftCurrentLane>(ref m_AircraftCurrentLaneType);
				NativeArray<TripSource> nativeArray15 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TripSource>(ref m_TripSourceType);
				NativeArray<PathOwner> nativeArray16 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
				NativeArray<PrefabRef> nativeArray17 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<PathElement> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
				bool flag5 = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
				bool flag6 = ((ArchetypeChunk)(ref chunk)).Has<Helicopter>(ref m_HelicopterType);
				SpawnLocationData spawnLocationData = default(SpawnLocationData);
				for (int l = 0; l < nativeArray.Length; l++)
				{
					Entity val6 = nativeArray[l];
					AircraftCurrentLane aircraftCurrentLane = nativeArray14[l];
					PrefabRef prefabRef4 = nativeArray17[l];
					AircraftNavigation aircraftNavigation = default(AircraftNavigation);
					if (flag5 && nativeArray15.Length != 0)
					{
						PathOwner pathOwner3 = nativeArray16[l];
						DynamicBuffer<PathElement> path3 = bufferAccessor4[l];
						InitializeRoadVehicle(ref random, val6, flag6 ? RoadTypes.Helicopter : RoadTypes.Airplane, nativeArray15[l], pathOwner3, prefabRef4, path3);
						if (aircraftCurrentLane.m_Lane == Entity.Null && path3.Length > pathOwner3.m_ElementIndex)
						{
							PathElement pathElement3 = path3[pathOwner3.m_ElementIndex];
							AircraftLaneFlags aircraftLaneFlags = (AircraftLaneFlags)0u;
							if (m_ConnectionLaneData.HasComponent(pathElement3.m_Target))
							{
								aircraftLaneFlags |= AircraftLaneFlags.Connection;
							}
							aircraftCurrentLane = new AircraftCurrentLane(pathElement3, aircraftLaneFlags);
						}
					}
					if (m_TransformData.HasComponent(aircraftCurrentLane.m_Lane))
					{
						aircraftCurrentLane.m_LaneFlags |= AircraftLaneFlags.TransformTarget;
						if (m_SpawnLocationData.HasComponent(aircraftCurrentLane.m_Lane))
						{
							PrefabRef prefabRef5 = m_PrefabRefData[aircraftCurrentLane.m_Lane];
							if (m_PrefabSpawnLocationData.TryGetComponent(prefabRef5.m_Prefab, ref spawnLocationData) && spawnLocationData.m_ConnectionType == RouteConnectionType.Air)
							{
								aircraftCurrentLane.m_LaneFlags |= AircraftLaneFlags.ParkingSpace;
							}
						}
					}
					aircraftNavigation.m_TargetPosition = m_TransformData[val6].m_Position;
					aircraftNavigation.m_TargetDirection = default(float3);
					nativeArray14[l] = aircraftCurrentLane;
					nativeArray13[l] = aircraftNavigation;
				}
				return;
			}
			NativeArray<ParkedCar> nativeArray18 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ParkedCar>(ref m_ParkedCarType);
			if (nativeArray18.Length != 0)
			{
				NativeArray<TripSource> nativeArray19 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TripSource>(ref m_TripSourceType);
				NativeArray<PrefabRef> nativeArray20 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				if (!((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType))
				{
					return;
				}
				Owner owner = default(Owner);
				for (int m = 0; m < nativeArray19.Length; m++)
				{
					Entity val7 = nativeArray[m];
					ParkedCar parkedCar = nativeArray18[m];
					TripSource tripSource3 = nativeArray19[m];
					PrefabRef prefabRef6 = nativeArray20[m];
					if (!m_TransformData.HasComponent(tripSource3.m_Source))
					{
						continue;
					}
					float3 position3 = m_TransformData[tripSource3.m_Source].m_Position;
					if (FindParkingSpace(position3, tripSource3.m_Source, ref random, out parkedCar.m_Lane, out parkedCar.m_CurvePosition))
					{
						if (m_CurveData.HasComponent(parkedCar.m_Lane))
						{
							Curve curve = m_CurveData[parkedCar.m_Lane];
							if (m_ParkingLaneData.HasComponent(parkedCar.m_Lane))
							{
								Game.Net.ParkingLane parkingLane = m_ParkingLaneData[parkedCar.m_Lane];
								PrefabRef prefabRef7 = m_PrefabRefData[parkedCar.m_Lane];
								ParkingLaneData parkingLaneData = m_PrefabParkingLaneData[prefabRef7.m_Prefab];
								ObjectGeometryData prefabGeometryData = m_PrefabObjectGeometryData[prefabRef6.m_Prefab];
								Transform ownerTransform = default(Transform);
								if (m_OwnerData.TryGetComponent(parkedCar.m_Lane, ref owner) && m_TransformData.HasComponent(owner.m_Owner))
								{
									ownerTransform = m_TransformData[owner.m_Owner];
								}
								m_TransformData[val7] = VehicleUtils.CalculateParkingSpaceTarget(parkingLane, parkingLaneData, prefabGeometryData, curve, ownerTransform, parkedCar.m_CurvePosition);
							}
							else
							{
								Transform transform4 = VehicleUtils.CalculateTransform(curve, parkedCar.m_CurvePosition);
								if (m_ConnectionLaneData.HasComponent(parkedCar.m_Lane))
								{
									Game.Net.ConnectionLane connectionLane3 = m_ConnectionLaneData[parkedCar.m_Lane];
									if ((connectionLane3.m_Flags & ConnectionLaneFlags.Parking) != 0)
									{
										parkedCar.m_CurvePosition = ((Random)(ref random)).NextFloat(0f, 1f);
										transform4.m_Position = VehicleUtils.GetConnectionParkingPosition(connectionLane3, curve.m_Bezier, parkedCar.m_CurvePosition);
									}
								}
								m_TransformData[val7] = transform4;
							}
						}
					}
					else
					{
						Transform transform5 = m_TransformData[val7];
						parkedCar.m_CurvePosition = ((Random)(ref random)).NextFloat(0f, 1f);
						Game.Net.ConnectionLane connectionLane4 = default(Game.Net.ConnectionLane);
						if (m_OutsideConnectionData.HasComponent(tripSource3.m_Source))
						{
							connectionLane4.m_Flags |= ConnectionLaneFlags.Outside;
						}
						transform5.m_Position = VehicleUtils.GetConnectionParkingPosition(connectionLane4, new Bezier4x3(position3, position3, position3, position3), parkedCar.m_CurvePosition);
						m_TransformData[val7] = transform5;
					}
					nativeArray18[m] = parkedCar;
				}
				return;
			}
			NativeArray<Train> nativeArray21 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Train>(ref m_TrainType);
			if (nativeArray21.Length == 0)
			{
				return;
			}
			NativeArray<TripSource> nativeArray22 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TripSource>(ref m_TripSourceType);
			NativeArray<PathOwner> nativeArray23 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<PrefabRef> nativeArray24 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<LayoutElement> bufferAccessor5 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LayoutElement>(ref m_LayoutElementType);
			BufferAccessor<PathElement> bufferAccessor6 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			NativeList<PathElement> laneBuffer = default(NativeList<PathElement>);
			laneBuffer._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			bool flag7 = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			bool flag8 = ((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType);
			ParkedTrain parkedTrain = default(ParkedTrain);
			DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
			for (int n = 0; n < nativeArray21.Length; n++)
			{
				Entity val8 = nativeArray[n];
				bool flag9 = m_ParkedTrainData.TryGetComponent(val8, ref parkedTrain);
				bool flag10 = CollectionUtils.TryGet<LayoutElement>(bufferAccessor5, n, ref layout);
				if ((flag9 && flag8) || (flag7 && nativeArray22.Length != 0))
				{
					if (flag10)
					{
						PathOwner pathOwner4 = default(PathOwner);
						DynamicBuffer<PathElement> path4 = default(DynamicBuffer<PathElement>);
						if (!flag9)
						{
							pathOwner4 = nativeArray23[n];
							path4 = bufferAccessor6[n];
						}
						float length = VehicleUtils.CalculateLength(val8, layout, ref m_PrefabRefData, ref m_PrefabTrainData);
						PathUtils.InitializeSpawnPath(path4, laneBuffer, parkedTrain.m_ParkingLocation, ref pathOwner4, length, ref m_CurveData, ref m_LaneData, ref m_EdgeLaneData, ref m_OwnerData, ref m_EdgeData, ref m_SpawnLocationData, ref m_ConnectedEdges, ref m_SubLanes);
						VehicleUtils.UpdateCarriageLocations(layout, laneBuffer, ref m_TrainData, ref m_ParkedTrainData, ref m_TrainCurrentLaneData, ref m_TrainNavigationData, ref m_TransformData, ref m_CurveData, ref m_ConnectionLaneData, ref m_PrefabRefData, ref m_PrefabTrainData);
						if (!flag9)
						{
							nativeArray23[n] = pathOwner4;
						}
						laneBuffer.Clear();
					}
				}
				else if (m_TrainNavigationData.HasComponent(val8))
				{
					Train train = nativeArray21[n];
					Transform transform6 = m_TransformData[val8];
					PrefabRef prefabRef8 = nativeArray24[n];
					TrainData prefabTrainData = m_PrefabTrainData[prefabRef8.m_Prefab];
					VehicleUtils.CalculateTrainNavigationPivots(transform6, prefabTrainData, out var pivot, out var pivot2);
					if ((train.m_Flags & TrainFlags.Reversed) != 0)
					{
						CommonUtils.Swap(ref pivot, ref pivot2);
					}
					TrainNavigation trainNavigation = default(TrainNavigation);
					trainNavigation.m_Front = new TrainBogiePosition(transform6);
					trainNavigation.m_Rear = new TrainBogiePosition(transform6);
					trainNavigation.m_Front.m_Position = pivot;
					trainNavigation.m_Rear.m_Position = pivot2;
					m_TrainNavigationData[val8] = trainNavigation;
				}
				if (flag10)
				{
					UpdateBogieFrames(layout);
				}
			}
		}

		private void ResetMeshBatches(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshBatch> val = default(DynamicBuffer<MeshBatch>);
			if (m_MeshBatches.TryGetBuffer(entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					MeshBatch meshBatch = val[i];
					meshBatch.m_MeshGroup = byte.MaxValue;
					meshBatch.m_MeshIndex = byte.MaxValue;
					meshBatch.m_TileIndex = byte.MaxValue;
					val[i] = meshBatch;
				}
			}
			DynamicBuffer<Game.Objects.SubObject> val2 = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(entity, ref val2))
			{
				for (int j = 0; j < val2.Length; j++)
				{
					ResetMeshBatches(val2[j].m_SubObject);
				}
			}
		}

		private void InitializeRoadVehicle(ref Random random, Entity vehicle, RoadTypes roadType, TripSource tripSource, PathOwner pathOwner, PrefabRef prefabRef, DynamicBuffer<PathElement> path)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			if (m_SpawnLocations.HasBuffer(tripSource.m_Source))
			{
				DynamicBuffer<SpawnLocationElement> spawnLocations = m_SpawnLocations[tripSource.m_Source];
				bool positionFound;
				bool rotationFound;
				Transform transform = CalculatePathTransform(vehicle, pathOwner, path, roadType, out positionFound, out rotationFound);
				if (!positionFound || !rotationFound)
				{
					PathMethod pathMethod = PathMethod.Road;
					if ((roadType & RoadTypes.Car) != RoadTypes.None && m_DeliveryTruckData.HasComponent(vehicle))
					{
						pathMethod |= PathMethod.CargoLoading;
					}
					Transform transform2 = m_TransformData[tripSource.m_Source];
					bool positionFound2;
					bool rotationFound2;
					Transform transform3 = FindClosestSpawnLocation(ref random, transform, pathMethod, TrackTypes.None, roadType, spawnLocations, transform2.Equals(transform), out positionFound2, out rotationFound2);
					if (!positionFound && positionFound2)
					{
						transform.m_Position = transform3.m_Position;
						positionFound = true;
					}
					if (!rotationFound && rotationFound2)
					{
						transform.m_Rotation = transform3.m_Rotation;
						rotationFound = true;
					}
				}
				if (!rotationFound)
				{
					Transform transform4 = m_TransformData[tripSource.m_Source];
					PrefabRef prefabRef2 = m_PrefabRefData[tripSource.m_Source];
					float3 val;
					if (m_PrefabBuildingData.HasComponent(prefabRef2.m_Prefab))
					{
						BuildingData buildingData = m_PrefabBuildingData[prefabRef2.m_Prefab];
						val = BuildingUtils.CalculateFrontPosition(transform4, buildingData.m_LotSize.y) - transform.m_Position;
					}
					else
					{
						val = transform4.m_Position - transform.m_Position;
					}
					if (MathUtils.TryNormalize(ref val))
					{
						transform.m_Rotation = quaternion.LookRotationSafe(val, math.up());
						rotationFound = true;
					}
					if (!positionFound)
					{
						transform.m_Position = transform4.m_Position;
					}
					if (!rotationFound)
					{
						transform.m_Rotation = transform4.m_Rotation;
					}
				}
				m_TransformData[vehicle] = transform;
			}
			else if (m_RouteLaneData.HasComponent(tripSource.m_Source))
			{
				Transform transform5 = m_TransformData[vehicle];
				RouteLane routeLane = m_RouteLaneData[tripSource.m_Source];
				float3 position = transform5.m_Position;
				MasterLane masterLane = default(MasterLane);
				Owner owner = default(Owner);
				DynamicBuffer<Game.Net.SubLane> lanes = default(DynamicBuffer<Game.Net.SubLane>);
				if (m_MasterLaneLaneData.TryGetComponent(routeLane.m_EndLane, ref masterLane) && m_OwnerData.TryGetComponent(routeLane.m_EndLane, ref owner) && m_SubLanes.TryGetBuffer(owner.m_Owner, ref lanes))
				{
					int num = NetUtils.ChooseClosestLane(masterLane.m_MinIndex, masterLane.m_MaxIndex, position, lanes, m_CurveData, routeLane.m_EndCurvePos);
					routeLane.m_EndLane = lanes[num].m_SubLane;
				}
				Curve curve = default(Curve);
				if (!m_CurveData.TryGetComponent(routeLane.m_EndLane, ref curve))
				{
					return;
				}
				transform5.m_Position = MathUtils.Position(curve.m_Bezier, routeLane.m_EndCurvePos);
				float3 val2 = MathUtils.Tangent(curve.m_Bezier, routeLane.m_EndCurvePos);
				if (MathUtils.TryNormalize(ref val2))
				{
					transform5.m_Rotation = quaternion.LookRotationSafe(val2, math.up());
					PrefabRef prefabRef3 = default(PrefabRef);
					NetLaneData netLaneData = default(NetLaneData);
					ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
					if (m_PrefabRefData.TryGetComponent(routeLane.m_EndLane, ref prefabRef3) && m_PrefabNetLaneData.TryGetComponent(prefabRef3.m_Prefab, ref netLaneData) && m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
					{
						float2 val3 = MathUtils.Right(((float3)(ref val2)).xz);
						val3 = math.select(val3, -val3, math.dot(val3, ((float3)(ref position)).xz - ((float3)(ref transform5.m_Position)).xz) < 0f);
						ref float3 position2 = ref transform5.m_Position;
						((float3)(ref position2)).xz = ((float3)(ref position2)).xz + val3 * ((netLaneData.m_Width - objectGeometryData.m_Size.x) * 0.5f);
					}
				}
				m_TransformData[vehicle] = transform5;
			}
			else
			{
				if (!m_TransformData.HasComponent(tripSource.m_Source))
				{
					return;
				}
				bool positionFound3;
				bool rotationFound3;
				Transform transform6 = CalculatePathTransform(vehicle, pathOwner, path, roadType, out positionFound3, out rotationFound3);
				if (!positionFound3 && !rotationFound3 && m_OutsideConnectionData.HasComponent(tripSource.m_Source))
				{
					bool positionFound4;
					bool rotationFound4;
					Transform transform7 = FindRandomConnectionLocation(ref random, roadType, tripSource.m_Source, out positionFound4, out rotationFound4);
					if (!positionFound3 && positionFound4)
					{
						transform6.m_Position = transform7.m_Position;
						positionFound3 = true;
					}
					if (!rotationFound3 && rotationFound4)
					{
						transform6.m_Rotation = transform7.m_Rotation;
						rotationFound3 = true;
					}
				}
				if (!rotationFound3)
				{
					Transform transform8 = m_TransformData[tripSource.m_Source];
					float3 val4 = transform8.m_Position - transform6.m_Position;
					if (MathUtils.TryNormalize(ref val4))
					{
						transform6.m_Rotation = quaternion.LookRotationSafe(val4, math.up());
						rotationFound3 = true;
					}
					if (!positionFound3)
					{
						transform6.m_Position = transform8.m_Position;
					}
					if (!rotationFound3)
					{
						transform6.m_Rotation = transform8.m_Rotation;
					}
				}
				m_TransformData[vehicle] = transform6;
			}
		}

		private Transform FindRandomConnectionLocation(ref Random random, RoadTypes roadType, Entity source, out bool positionFound, out bool rotationFound)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			Transform result = default(Transform);
			positionFound = false;
			rotationFound = false;
			int num = 0;
			float3 val = default(float3);
			Owner owner = default(Owner);
			DynamicBuffer<Game.Net.SubLane> val2 = default(DynamicBuffer<Game.Net.SubLane>);
			if (m_OwnerData.TryGetComponent(source, ref owner) && m_SubLanes.TryGetBuffer(owner.m_Owner, ref val2))
			{
				Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
				for (int i = 0; i < val2.Length; i++)
				{
					Game.Net.SubLane subLane = val2[i];
					if (m_ConnectionLaneData.TryGetComponent(subLane.m_SubLane, ref connectionLane) && (connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0 && (connectionLane.m_RoadTypes & roadType) != RoadTypes.None && ((Random)(ref random)).NextInt(++num) == 0)
					{
						Curve curve = m_CurveData[subLane.m_SubLane];
						result.m_Position = MathUtils.Position(curve.m_Bezier, 0.5f);
						val = MathUtils.Tangent(curve.m_Bezier, 0.5f);
						positionFound = true;
					}
				}
			}
			DynamicBuffer<Game.Net.SubLane> val3 = default(DynamicBuffer<Game.Net.SubLane>);
			if (m_SubLanes.TryGetBuffer(source, ref val3))
			{
				Game.Net.ConnectionLane connectionLane2 = default(Game.Net.ConnectionLane);
				for (int j = 0; j < val3.Length; j++)
				{
					Game.Net.SubLane subLane2 = val3[j];
					if (m_ConnectionLaneData.TryGetComponent(subLane2.m_SubLane, ref connectionLane2) && (connectionLane2.m_Flags & ConnectionLaneFlags.Road) != 0 && (connectionLane2.m_RoadTypes & roadType) != RoadTypes.None && ((Random)(ref random)).NextInt(++num) == 0)
					{
						Curve curve2 = m_CurveData[subLane2.m_SubLane];
						result.m_Position = MathUtils.Position(curve2.m_Bezier, 0.5f);
						val = MathUtils.Tangent(curve2.m_Bezier, 0.5f);
						positionFound = true;
					}
				}
			}
			if (positionFound && MathUtils.TryNormalize(ref val))
			{
				result.m_Rotation = quaternion.LookRotationSafe(-val, math.up());
				rotationFound = true;
			}
			return result;
		}

		private void UpdateBogieFrames(DynamicBuffer<LayoutElement> layout)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			TrainCurrentLane trainCurrentLane = default(TrainCurrentLane);
			for (int i = 0; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				if (m_TrainCurrentLaneData.TryGetComponent(vehicle, ref trainCurrentLane))
				{
					DynamicBuffer<TrainBogieFrame> val = m_TrainBogieFrames[vehicle];
					val.ResizeUninitialized(4);
					for (int j = 0; j < val.Length; j++)
					{
						val[j] = new TrainBogieFrame
						{
							m_FrontLane = trainCurrentLane.m_Front.m_Lane,
							m_RearLane = trainCurrentLane.m_Rear.m_Lane
						};
					}
				}
			}
		}

		private Transform CalculatePathTransform(Entity vehicle, PathOwner pathOwner, DynamicBuffer<PathElement> path, RoadTypes roadType, out bool positionFound, out bool rotationFound)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			Transform result = m_TransformData[vehicle];
			positionFound = false;
			rotationFound = false;
			for (int i = pathOwner.m_ElementIndex; i < path.Length; i++)
			{
				PathElement pathElement = path[i];
				if (m_TransformData.HasComponent(pathElement.m_Target))
				{
					float3 position = m_TransformData[pathElement.m_Target].m_Position;
					if (positionFound)
					{
						float3 val = position - result.m_Position;
						if ((roadType & (RoadTypes.Watercraft | RoadTypes.Helicopter)) != RoadTypes.None)
						{
							val.y = 0f;
						}
						if (MathUtils.TryNormalize(ref val))
						{
							result.m_Rotation = quaternion.LookRotationSafe(val, math.up());
							rotationFound = true;
							return result;
						}
					}
					else
					{
						result.m_Position = position;
						positionFound = true;
					}
				}
				else
				{
					if (!m_CurveData.HasComponent(pathElement.m_Target))
					{
						continue;
					}
					Curve curve = m_CurveData[pathElement.m_Target];
					float3 val2 = MathUtils.Position(curve.m_Bezier, pathElement.m_TargetDelta.x);
					if (positionFound)
					{
						float3 val3 = val2 - result.m_Position;
						if ((roadType & (RoadTypes.Watercraft | RoadTypes.Helicopter)) != RoadTypes.None)
						{
							val3.y = 0f;
						}
						if (MathUtils.TryNormalize(ref val3))
						{
							result.m_Rotation = quaternion.LookRotationSafe(val3, math.up());
							rotationFound = true;
							return result;
						}
					}
					else
					{
						result.m_Position = val2;
						positionFound = true;
					}
					if (pathElement.m_TargetDelta.x != pathElement.m_TargetDelta.y)
					{
						float3 val4 = MathUtils.Tangent(curve.m_Bezier, pathElement.m_TargetDelta.x);
						val4 = math.select(val4, -val4, pathElement.m_TargetDelta.y < pathElement.m_TargetDelta.x);
						if ((roadType & (RoadTypes.Watercraft | RoadTypes.Helicopter)) != RoadTypes.None)
						{
							val4.y = 0f;
						}
						if (MathUtils.TryNormalize(ref val4))
						{
							result.m_Rotation = quaternion.LookRotationSafe(val4, math.up());
							rotationFound = true;
							return result;
						}
					}
				}
			}
			return result;
		}

		private Transform FindClosestSpawnLocation(ref Random random, Transform compareTransform, PathMethod pathMethods, TrackTypes trackTypes, RoadTypes roadTypes, DynamicBuffer<SpawnLocationElement> spawnLocations, bool selectRandom, out bool positionFound, out bool rotationFound)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			Transform result = compareTransform;
			positionFound = false;
			rotationFound = false;
			Entity val = Entity.Null;
			float num = float.MaxValue;
			int num2 = 0;
			Transform transform = default(Transform);
			for (int i = 0; i < spawnLocations.Length; i++)
			{
				if (spawnLocations[i].m_Type != SpawnLocationType.SpawnLocation)
				{
					continue;
				}
				Entity spawnLocation = spawnLocations[i].m_SpawnLocation;
				PrefabRef prefabRef = m_PrefabRefData[spawnLocation];
				SpawnLocationData spawnLocationData = m_PrefabSpawnLocationData[prefabRef.m_Prefab];
				if (spawnLocationData.m_ConnectionType != RouteConnectionType.Air || (pathMethods & PathMethod.Road) == 0 || (roadTypes & spawnLocationData.m_RoadTypes) == 0)
				{
					PathMethod pathMethod = spawnLocationData.m_ConnectionType switch
					{
						RouteConnectionType.Pedestrian => pathMethods & PathMethod.Pedestrian, 
						RouteConnectionType.Cargo => pathMethods & PathMethod.CargoLoading, 
						RouteConnectionType.Road => pathMethods & PathMethod.Road, 
						RouteConnectionType.Track => pathMethods & PathMethod.Track, 
						RouteConnectionType.Air => pathMethods & PathMethod.Flying, 
						_ => (PathMethod)0, 
					};
					if (pathMethod == (PathMethod)0)
					{
						continue;
					}
					TrackTypes trackTypes2 = trackTypes & spawnLocationData.m_TrackTypes;
					RoadTypes roadTypes2 = roadTypes & spawnLocationData.m_RoadTypes;
					if (((pathMethod & PathMethod.Track) == 0 || trackTypes2 == TrackTypes.None) && ((pathMethod & (PathMethod.Road | PathMethod.CargoLoading)) == 0 || roadTypes2 == RoadTypes.None))
					{
						continue;
					}
				}
				if (!m_TransformData.TryGetComponent(spawnLocation, ref transform))
				{
					continue;
				}
				if (selectRandom)
				{
					if (((Random)(ref random)).NextInt(++num2) == 0)
					{
						result.m_Position = transform.m_Position;
						positionFound = true;
						val = spawnLocation;
					}
					continue;
				}
				float num3 = math.distance(transform.m_Position, compareTransform.m_Position);
				if (num3 < num)
				{
					result.m_Position = transform.m_Position;
					positionFound = true;
					val = spawnLocation;
					num = num3;
				}
			}
			if (m_SpawnLocationData.HasComponent(val))
			{
				Game.Objects.SpawnLocation spawnLocation2 = m_SpawnLocationData[val];
				if (m_CurveData.HasComponent(spawnLocation2.m_ConnectedLane1))
				{
					Curve curve = m_CurveData[spawnLocation2.m_ConnectedLane1];
					float3 val2 = MathUtils.Position(curve.m_Bezier, spawnLocation2.m_CurvePosition1) - result.m_Position;
					if ((roadTypes & (RoadTypes.Watercraft | RoadTypes.Helicopter)) != RoadTypes.None)
					{
						val2.y = 0f;
					}
					if (MathUtils.TryNormalize(ref val2))
					{
						result.m_Rotation = quaternion.LookRotationSafe(val2, math.up());
						rotationFound = true;
						return result;
					}
					float3 val3 = MathUtils.Tangent(curve.m_Bezier, spawnLocation2.m_CurvePosition1);
					if ((roadTypes & (RoadTypes.Watercraft | RoadTypes.Helicopter)) != RoadTypes.None)
					{
						val3.y = 0f;
					}
					if (MathUtils.TryNormalize(ref val3))
					{
						result.m_Rotation = quaternion.LookRotationSafe(val3, math.up());
						rotationFound = true;
						return result;
					}
				}
			}
			if (positionFound)
			{
				float3 val4 = result.m_Position - compareTransform.m_Position;
				if ((roadTypes & (RoadTypes.Watercraft | RoadTypes.Helicopter)) != RoadTypes.None)
				{
					val4.y = 0f;
				}
				if (MathUtils.TryNormalize(ref val4))
				{
					result.m_Rotation = quaternion.LookRotationSafe(val4, math.up());
					rotationFound = true;
					return result;
				}
			}
			return result;
		}

		private bool FindParkingSpace(float3 comparePosition, Entity source, ref Random random, out Entity lane, out float curvePos)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			while (true)
			{
				if (m_SpawnLocations.HasBuffer(source))
				{
					DynamicBuffer<SpawnLocationElement> val = m_SpawnLocations[source];
					for (int i = 0; i < val.Length; i++)
					{
						if (val[i].m_Type != SpawnLocationType.SpawnLocation)
						{
							continue;
						}
						Entity spawnLocation = val[i].m_SpawnLocation;
						PrefabRef prefabRef = m_PrefabRefData[spawnLocation];
						if (m_PrefabSpawnLocationData[prefabRef.m_Prefab].m_ConnectionType != RouteConnectionType.Road || !m_SpawnLocationData.HasComponent(spawnLocation))
						{
							continue;
						}
						Game.Objects.SpawnLocation spawnLocation2 = m_SpawnLocationData[spawnLocation];
						if (m_OwnerData.HasComponent(spawnLocation2.m_ConnectedLane1))
						{
							Owner owner = m_OwnerData[spawnLocation2.m_ConnectedLane1];
							if (m_SubLanes.HasBuffer(owner.m_Owner) && FindParkingSpace(comparePosition, m_SubLanes[owner.m_Owner], ref random, out lane, out curvePos))
							{
								return true;
							}
						}
					}
				}
				if (m_SubLanes.HasBuffer(source) && FindParkingSpace(comparePosition, m_SubLanes[source], ref random, out lane, out curvePos))
				{
					return true;
				}
				if (m_BuildingData.HasComponent(source))
				{
					Building building = m_BuildingData[source];
					if (m_SubLanes.HasBuffer(building.m_RoadEdge) && FindParkingSpace(comparePosition, m_SubLanes[building.m_RoadEdge], ref random, out lane, out curvePos))
					{
						return true;
					}
				}
				if (!m_OwnerData.HasComponent(source))
				{
					break;
				}
				source = m_OwnerData[source].m_Owner;
			}
			lane = Entity.Null;
			curvePos = 0f;
			return false;
		}

		private bool FindParkingSpace(float3 comparePosition, DynamicBuffer<Game.Net.SubLane> lanes, ref Random random, out Entity lane, out float curvePos)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			lane = Entity.Null;
			curvePos = 0f;
			float num = float.MaxValue;
			float num3 = default(float);
			float num5 = default(float);
			for (int i = 0; i < lanes.Length; i++)
			{
				Entity subLane = lanes[i].m_SubLane;
				if (m_ParkingLaneData.HasComponent(subLane))
				{
					float num2 = MathUtils.Distance(m_CurveData[subLane].m_Bezier, comparePosition, ref num3);
					if (num2 < num)
					{
						num = num2;
						curvePos = num3;
						lane = subLane;
					}
				}
				else if (m_ConnectionLaneData.HasComponent(subLane) && (m_ConnectionLaneData[subLane].m_Flags & ConnectionLaneFlags.Parking) != 0)
				{
					float num4 = MathUtils.Distance(m_CurveData[subLane].m_Bezier, comparePosition, ref num5);
					if (num4 < num)
					{
						num = num4;
						curvePos = ((Random)(ref random)).NextFloat(0f, 1f);
						lane = subLane;
					}
				}
			}
			curvePos = math.clamp(curvePos, 0.05f, 0.95f);
			curvePos = ((Random)(ref random)).NextFloat(math.max(0.05f, curvePos - 0.2f), math.min(0.95f, curvePos + 0.2f));
			return lane != Entity.Null;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Train> __Game_Vehicles_Train_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TripSource> __Game_Objects_TripSource_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> __Game_Objects_Unspawned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Helicopter> __Game_Vehicles_Helicopter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Car> __Game_Vehicles_Car_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferTypeHandle;

		public ComponentTypeHandle<CarNavigation> __Game_Vehicles_CarNavigation_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<WatercraftNavigation> __Game_Vehicles_WatercraftNavigation_RW_ComponentTypeHandle;

		public ComponentTypeHandle<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AircraftNavigation> __Game_Vehicles_AircraftNavigation_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<ParkedCar> __Game_Vehicles_ParkedCar_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteLane> __Game_Routes_RouteLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainData> __Game_Prefabs_TrainData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarTractorData> __Game_Prefabs_CarTractorData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarTrailerData> __Game_Prefabs_CarTrailerData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> __Game_Buildings_SpawnLocationElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		public ComponentLookup<Transform> __Game_Objects_Transform_RW_ComponentLookup;

		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RW_ComponentLookup;

		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RW_ComponentLookup;

		public ComponentLookup<TrainNavigation> __Game_Vehicles_TrainNavigation_RW_ComponentLookup;

		public ComponentLookup<CarTrailerLane> __Game_Vehicles_CarTrailerLane_RW_ComponentLookup;

		public BufferLookup<TrainBogieFrame> __Game_Vehicles_TrainBogieFrame_RW_BufferLookup;

		public BufferLookup<MeshBatch> __Game_Rendering_MeshBatch_RW_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarTrailerLane> __Game_Vehicles_CarTrailerLane_RO_ComponentTypeHandle;

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
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Vehicles_Train_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Train>(true);
			__Game_Objects_TripSource_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TripSource>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Vehicles_Helicopter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Helicopter>(true);
			__Game_Vehicles_Car_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Car>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Vehicles_LayoutElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LayoutElement>(true);
			__Game_Vehicles_CarNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarNavigation>(false);
			__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarCurrentLane>(false);
			__Game_Vehicles_WatercraftNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WatercraftNavigation>(false);
			__Game_Vehicles_WatercraftCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WatercraftCurrentLane>(false);
			__Game_Vehicles_AircraftNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftNavigation>(false);
			__Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftCurrentLane>(false);
			__Game_Vehicles_ParkedCar_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkedCar>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Pathfind_PathElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DeliveryTruck>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Edge>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Routes_RouteLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_TrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_CarTractorData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarTractorData>(true);
			__Game_Prefabs_CarTrailerData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarTrailerData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Buildings_SpawnLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SpawnLocationElement>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Objects_Transform_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(false);
			__Game_Vehicles_TrainCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(false);
			__Game_Vehicles_ParkedTrain_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(false);
			__Game_Vehicles_TrainNavigation_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainNavigation>(false);
			__Game_Vehicles_CarTrailerLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarTrailerLane>(false);
			__Game_Vehicles_TrainBogieFrame_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TrainBogieFrame>(false);
			__Game_Rendering_MeshBatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshBatch>(false);
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarCurrentLane>(true);
			__Game_Vehicles_CarTrailerLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarTrailerLane>(true);
			__Game_Net_LaneObject_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(false);
		}
	}

	private Game.Objects.SearchSystem m_SearchSystem;

	private EntityQuery m_VehicleQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Vehicle>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_VehicleQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		InitializeVehiclesJob initializeVehiclesJob = new InitializeVehiclesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrainType = InternalCompilerInterface.GetComponentTypeHandle<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TripSourceType = InternalCompilerInterface.GetComponentTypeHandle<TripSource>(ref __TypeHandle.__Game_Objects_TripSource_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HelicopterType = InternalCompilerInterface.GetComponentTypeHandle<Helicopter>(ref __TypeHandle.__Game_Vehicles_Helicopter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarType = InternalCompilerInterface.GetComponentTypeHandle<Car>(ref __TypeHandle.__Game_Vehicles_Car_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElementType = InternalCompilerInterface.GetBufferTypeHandle<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarNavigationType = InternalCompilerInterface.GetComponentTypeHandle<CarNavigation>(ref __TypeHandle.__Game_Vehicles_CarNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftNavigationType = InternalCompilerInterface.GetComponentTypeHandle<WatercraftNavigation>(ref __TypeHandle.__Game_Vehicles_WatercraftNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftNavigationType = InternalCompilerInterface.GetComponentTypeHandle<AircraftNavigation>(ref __TypeHandle.__Game_Vehicles_AircraftNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarType = InternalCompilerInterface.GetComponentTypeHandle<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeliveryTruckData = InternalCompilerInterface.GetComponentLookup<DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MasterLaneLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteLaneData = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainData = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarTractorData = InternalCompilerInterface.GetComponentLookup<CarTractorData>(ref __TypeHandle.__Game_Prefabs_CarTractorData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarTrailerData = InternalCompilerInterface.GetComponentLookup<CarTrailerData>(ref __TypeHandle.__Game_Prefabs_CarTrailerData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocations = InternalCompilerInterface.GetBufferLookup<SpawnLocationElement>(ref __TypeHandle.__Game_Buildings_SpawnLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainCurrentLaneData = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainNavigationData = InternalCompilerInterface.GetComponentLookup<TrainNavigation>(ref __TypeHandle.__Game_Vehicles_TrainNavigation_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarTrailerLaneData = InternalCompilerInterface.GetComponentLookup<CarTrailerLane>(ref __TypeHandle.__Game_Vehicles_CarTrailerLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainBogieFrames = InternalCompilerInterface.GetBufferLookup<TrainBogieFrame>(ref __TypeHandle.__Game_Vehicles_TrainBogieFrame_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshBatches = InternalCompilerInterface.GetBufferLookup<MeshBatch>(ref __TypeHandle.__Game_Rendering_MeshBatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next()
		};
		JobHandle dependencies;
		TreeFixJob obj = new TreeFixJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarTrailerLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarTrailerLane>(ref __TypeHandle.__Game_Vehicles_CarTrailerLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SearchTree = m_SearchSystem.GetMovingSearchTree(readOnly: false, out dependencies),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		JobHandle val = JobChunkExtensions.ScheduleParallel<InitializeVehiclesJob>(initializeVehiclesJob, m_VehicleQuery, ((SystemBase)this).Dependency);
		JobHandle val2 = JobChunkExtensions.Schedule<TreeFixJob>(obj, m_VehicleQuery, JobHandle.CombineDependencies(val, dependencies));
		m_SearchSystem.AddMovingSearchTreeWriter(val2);
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
	public InitializeSystem()
	{
	}
}

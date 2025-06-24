using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
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
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class WatercraftNavigationSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateNavigationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Moving> m_MovingType;

		[ReadOnly]
		public ComponentTypeHandle<Target> m_TargetType;

		[ReadOnly]
		public ComponentTypeHandle<Watercraft> m_WatercraftType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<WatercraftNavigation> m_NavigationType;

		public ComponentTypeHandle<WatercraftCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public ComponentTypeHandle<Blocker> m_BlockerType;

		public ComponentTypeHandle<Odometer> m_OdometerType;

		public BufferTypeHandle<WatercraftNavigationLane> m_NavigationLaneType;

		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<LaneReservation> m_LaneReservationData;

		[ReadOnly]
		public ComponentLookup<LaneSignal> m_LaneSignalData;

		[ReadOnly]
		public ComponentLookup<AreaLane> m_AreaLaneData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TakeoffLocation> m_TakeoffLocationData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingData;

		[ReadOnly]
		public ComponentLookup<Watercraft> m_WatercraftData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<Creature> m_CreatureData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<WatercraftData> m_PrefabWatercraftData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<VehicleSideEffectData> m_PrefabSideEffectData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_Lanes;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<LaneOverlap> m_LaneOverlaps;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingObjectSearchTree;

		public LaneObjectCommandBuffer m_LaneObjectBuffer;

		public ParallelWriter<WatercraftNavigationHelpers.LaneReservation> m_LaneReservations;

		public ParallelWriter<WatercraftNavigationHelpers.LaneEffects> m_LaneEffects;

		public ParallelWriter<WatercraftNavigationHelpers.LaneSignal> m_LaneSignals;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Moving> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
			NativeArray<Blocker> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Blocker>(ref m_BlockerType);
			NativeArray<WatercraftCurrentLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftCurrentLane>(ref m_CurrentLaneType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			if (nativeArray.Length != 0)
			{
				NativeArray<Entity> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<Transform> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				NativeArray<Target> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
				NativeArray<Watercraft> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Watercraft>(ref m_WatercraftType);
				NativeArray<WatercraftNavigation> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftNavigation>(ref m_NavigationType);
				NativeArray<Odometer> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Odometer>(ref m_OdometerType);
				NativeArray<PathOwner> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
				NativeArray<PrefabRef> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<WatercraftNavigationLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<WatercraftNavigationLane>(ref m_NavigationLaneType);
				BufferAccessor<PathElement> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
				WatercraftLaneSelectBuffer laneSelectBuffer = default(WatercraftLaneSelectBuffer);
				bool flag = nativeArray9.Length != 0;
				for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
				{
					Entity entity = nativeArray4[i];
					Transform transform = nativeArray5[i];
					Moving moving = nativeArray[i];
					Target target = nativeArray6[i];
					Watercraft watercraft = nativeArray7[i];
					WatercraftNavigation navigation = nativeArray8[i];
					WatercraftCurrentLane currentLane = nativeArray3[i];
					Blocker blocker = nativeArray2[i];
					PathOwner pathOwner = nativeArray10[i];
					PrefabRef prefabRefData = nativeArray11[i];
					DynamicBuffer<WatercraftNavigationLane> navigationLanes = bufferAccessor[i];
					DynamicBuffer<PathElement> pathElements = bufferAccessor2[i];
					WatercraftData prefabWatercraftData = m_PrefabWatercraftData[prefabRefData.m_Prefab];
					ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRefData.m_Prefab];
					WatercraftNavigationHelpers.CurrentLaneCache currentLaneCache = new WatercraftNavigationHelpers.CurrentLaneCache(ref currentLane, m_EntityLookup, m_MovingObjectSearchTree);
					int priority = VehicleUtils.GetPriority(prefabWatercraftData);
					Odometer odometer = default(Odometer);
					if (flag)
					{
						odometer = nativeArray9[i];
					}
					UpdateNavigationLanes(ref random, priority, entity, transform, target, watercraft, ref laneSelectBuffer, ref currentLane, ref blocker, ref pathOwner, navigationLanes, pathElements);
					UpdateNavigationTarget(priority, entity, transform, moving, watercraft, pathOwner, prefabRefData, prefabWatercraftData, objectGeometryData, ref navigation, ref currentLane, ref blocker, ref odometer, navigationLanes, pathElements);
					ReserveNavigationLanes(priority, prefabWatercraftData, watercraft, ref navigation, ref currentLane, navigationLanes);
					currentLaneCache.CheckChanges(entity, ref currentLane, m_LaneObjectBuffer, m_LaneObjects, transform, moving, navigation, objectGeometryData);
					nativeArray8[i] = navigation;
					nativeArray3[i] = currentLane;
					nativeArray10[i] = pathOwner;
					nativeArray2[i] = blocker;
					if (flag)
					{
						nativeArray9[i] = odometer;
					}
				}
				laneSelectBuffer.Dispose();
				return;
			}
			for (int j = 0; j < ((ArchetypeChunk)(ref chunk)).Count; j++)
			{
				WatercraftCurrentLane watercraftCurrentLane = nativeArray3[j];
				Blocker blocker2 = nativeArray2[j];
				if ((watercraftCurrentLane.m_LaneFlags & WatercraftLaneFlags.QueueReached) != 0 && (!m_WatercraftData.HasComponent(blocker2.m_Blocker) || (m_WatercraftData[blocker2.m_Blocker].m_Flags & WatercraftFlags.Queueing) == 0))
				{
					watercraftCurrentLane.m_LaneFlags &= ~WatercraftLaneFlags.QueueReached;
					blocker2 = default(Blocker);
				}
				nativeArray3[j] = watercraftCurrentLane;
				nativeArray2[j] = blocker2;
			}
		}

		private void UpdateNavigationLanes(ref Random random, int priority, Entity entity, Transform transform, Target target, Watercraft watercraft, ref WatercraftLaneSelectBuffer laneSelectBuffer, ref WatercraftCurrentLane currentLane, ref Blocker blocker, ref PathOwner pathOwner, DynamicBuffer<WatercraftNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			int invalidPath = 10000000;
			if (currentLane.m_Lane == Entity.Null || (currentLane.m_LaneFlags & WatercraftLaneFlags.Obsolete) != 0)
			{
				invalidPath = -1;
				TryFindCurrentLane(ref currentLane, transform);
			}
			else if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete | PathFlags.Updated)) != 0 && (pathOwner.m_State & PathFlags.Append) == 0)
			{
				ClearNavigationLanes(ref currentLane, navigationLanes, invalidPath);
			}
			else if ((pathOwner.m_State & PathFlags.Updated) == 0)
			{
				FillNavigationPaths(ref random, priority, entity, transform, target, watercraft, ref laneSelectBuffer, ref currentLane, ref blocker, ref pathOwner, navigationLanes, pathElements, ref invalidPath);
			}
			if (invalidPath != 10000000)
			{
				ClearNavigationLanes(ref currentLane, navigationLanes, invalidPath);
				pathElements.Clear();
				pathOwner.m_ElementIndex = 0;
				pathOwner.m_State |= PathFlags.Obsolete;
			}
		}

		private void ClearNavigationLanes(ref WatercraftCurrentLane currentLane, DynamicBuffer<WatercraftNavigationLane> navigationLanes, int invalidPath)
		{
			currentLane.m_CurvePosition.z = currentLane.m_CurvePosition.y;
			if (invalidPath > 0)
			{
				for (int i = 0; i < navigationLanes.Length; i++)
				{
					if ((navigationLanes[i].m_Flags & WatercraftLaneFlags.Reserved) == 0)
					{
						invalidPath = math.min(i, invalidPath);
						break;
					}
				}
			}
			invalidPath = math.max(invalidPath, 0);
			if (invalidPath < navigationLanes.Length)
			{
				navigationLanes.RemoveRange(invalidPath, navigationLanes.Length - invalidPath);
			}
		}

		private void TryFindCurrentLane(ref WatercraftCurrentLane currentLaneData, Transform transformData)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			currentLaneData.m_LaneFlags &= ~(WatercraftLaneFlags.TransformTarget | WatercraftLaneFlags.Obsolete | WatercraftLaneFlags.Area);
			currentLaneData.m_Lane = Entity.Null;
			currentLaneData.m_ChangeLane = Entity.Null;
			float3 position = transformData.m_Position;
			float num = 100f;
			Bounds3 bounds = default(Bounds3);
			((Bounds3)(ref bounds))._002Ector(position - num, position + num);
			WatercraftNavigationHelpers.FindLaneIterator findLaneIterator = new WatercraftNavigationHelpers.FindLaneIterator
			{
				m_Bounds = bounds,
				m_Position = position,
				m_MinDistance = num,
				m_Result = currentLaneData,
				m_CarType = RoadTypes.Watercraft,
				m_SubLanes = m_Lanes,
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
			currentLaneData = findLaneIterator.m_Result;
		}

		private void FillNavigationPaths(ref Random random, int priority, Entity entity, Transform transform, Target target, Watercraft watercraft, ref WatercraftLaneSelectBuffer laneSelectBuffer, ref WatercraftCurrentLane currentLaneData, ref Blocker blocker, ref PathOwner pathOwner, DynamicBuffer<WatercraftNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements, ref int invalidPath)
		{
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			if ((currentLaneData.m_LaneFlags & WatercraftLaneFlags.EndOfPath) == 0)
			{
				for (int i = 0; i < 8; i++)
				{
					if (i >= navigationLanes.Length)
					{
						i = navigationLanes.Length;
						if (pathOwner.m_ElementIndex >= pathElements.Length)
						{
							if ((pathOwner.m_State & PathFlags.Pending) != 0)
							{
								break;
							}
							WatercraftNavigationLane navLaneData = default(WatercraftNavigationLane);
							if (i > 0)
							{
								WatercraftNavigationLane watercraftNavigationLane = navigationLanes[i - 1];
								if ((watercraftNavigationLane.m_Flags & WatercraftLaneFlags.TransformTarget) == 0 && (watercraft.m_Flags & (WatercraftFlags.StayOnWaterway | WatercraftFlags.AnyLaneTarget)) != (WatercraftFlags.StayOnWaterway | WatercraftFlags.AnyLaneTarget) && GetTransformTarget(ref navLaneData.m_Lane, target))
								{
									if ((watercraftNavigationLane.m_Flags & WatercraftLaneFlags.GroupTarget) == 0)
									{
										Entity lane = navLaneData.m_Lane;
										navLaneData.m_Lane = watercraftNavigationLane.m_Lane;
										navLaneData.m_Flags = watercraftNavigationLane.m_Flags & (WatercraftLaneFlags.Connection | WatercraftLaneFlags.Area);
										navLaneData.m_CurvePosition = ((float2)(ref watercraftNavigationLane.m_CurvePosition)).yy;
										float3 position = default(float3);
										if (VehicleUtils.CalculateTransformPosition(ref position, lane, m_TransformData, m_PositionData, m_PrefabRefData, m_PrefabBuildingData))
										{
											UpdateSlaveLane(ref navLaneData, position);
											Align(ref navLaneData, position);
										}
										if ((watercraft.m_Flags & WatercraftFlags.StayOnWaterway) != 0)
										{
											navLaneData.m_Flags |= WatercraftLaneFlags.EndOfPath | WatercraftLaneFlags.GroupTarget;
											navigationLanes.Add(navLaneData);
											currentLaneData.m_LaneFlags |= WatercraftLaneFlags.UpdateOptimalLane;
											break;
										}
										navLaneData.m_Flags |= WatercraftLaneFlags.GroupTarget;
										navigationLanes.Add(navLaneData);
										currentLaneData.m_LaneFlags |= WatercraftLaneFlags.UpdateOptimalLane;
										continue;
									}
									navLaneData.m_Flags |= WatercraftLaneFlags.EndOfPath | WatercraftLaneFlags.TransformTarget;
									navigationLanes.Add(navLaneData);
									currentLaneData.m_LaneFlags |= WatercraftLaneFlags.UpdateOptimalLane;
									break;
								}
								watercraftNavigationLane.m_Flags |= WatercraftLaneFlags.EndOfPath;
								navigationLanes[i - 1] = watercraftNavigationLane;
								currentLaneData.m_LaneFlags |= WatercraftLaneFlags.UpdateOptimalLane;
								break;
							}
							if ((currentLaneData.m_LaneFlags & WatercraftLaneFlags.TransformTarget) != 0 || (watercraft.m_Flags & WatercraftFlags.StayOnWaterway) != 0 || !GetTransformTarget(ref navLaneData.m_Lane, target))
							{
								currentLaneData.m_LaneFlags |= WatercraftLaneFlags.EndOfPath;
								break;
							}
							navLaneData.m_Flags |= WatercraftLaneFlags.EndOfPath | WatercraftLaneFlags.TransformTarget;
							navigationLanes.Add(navLaneData);
							currentLaneData.m_LaneFlags |= WatercraftLaneFlags.UpdateOptimalLane;
							break;
						}
						PathElement pathElement = pathElements[pathOwner.m_ElementIndex++];
						WatercraftNavigationLane navLaneData2 = new WatercraftNavigationLane
						{
							m_Lane = pathElement.m_Target,
							m_CurvePosition = pathElement.m_TargetDelta
						};
						if (!m_CarLaneData.HasComponent(navLaneData2.m_Lane))
						{
							if (m_ConnectionLaneData.HasComponent(navLaneData2.m_Lane))
							{
								Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[navLaneData2.m_Lane];
								navLaneData2.m_Flags |= WatercraftLaneFlags.FixedLane;
								if ((connectionLane.m_Flags & ConnectionLaneFlags.Area) != 0)
								{
									navLaneData2.m_Flags |= WatercraftLaneFlags.Area;
								}
								else
								{
									navLaneData2.m_Flags |= WatercraftLaneFlags.Connection;
								}
								currentLaneData.m_LaneFlags |= WatercraftLaneFlags.UpdateOptimalLane;
								navigationLanes.Add(navLaneData2);
								continue;
							}
							if (!m_TransformData.HasComponent(navLaneData2.m_Lane))
							{
								invalidPath = i;
								return;
							}
							if (pathOwner.m_ElementIndex >= pathElements.Length && (pathOwner.m_State & PathFlags.Pending) != 0)
							{
								pathOwner.m_ElementIndex--;
								break;
							}
							if (!m_TakeoffLocationData.HasComponent(navLaneData2.m_Lane) && ((watercraft.m_Flags & WatercraftFlags.StayOnWaterway) == 0 || pathElements.Length > pathOwner.m_ElementIndex))
							{
								navLaneData2.m_Flags |= WatercraftLaneFlags.TransformTarget;
								navigationLanes.Add(navLaneData2);
								if (i > 0)
								{
									float3 position2 = m_TransformData[navLaneData2.m_Lane].m_Position;
									WatercraftNavigationLane navLaneData3 = navigationLanes[i - 1];
									UpdateSlaveLane(ref navLaneData3, position2);
									navigationLanes[i - 1] = navLaneData3;
								}
								currentLaneData.m_LaneFlags |= WatercraftLaneFlags.UpdateOptimalLane;
							}
						}
						else
						{
							navLaneData2.m_Flags |= WatercraftLaneFlags.UpdateOptimalLane;
							currentLaneData.m_LaneFlags |= WatercraftLaneFlags.UpdateOptimalLane;
							if (i == 0 && (currentLaneData.m_LaneFlags & (WatercraftLaneFlags.FixedLane | WatercraftLaneFlags.Connection)) == WatercraftLaneFlags.FixedLane)
							{
								GetSlaveLaneFromMasterLane(ref random, ref navLaneData2, currentLaneData);
							}
							else
							{
								GetSlaveLaneFromMasterLane(ref random, ref navLaneData2);
							}
							navigationLanes.Add(navLaneData2);
						}
					}
					else
					{
						WatercraftNavigationLane watercraftNavigationLane2 = navigationLanes[i];
						if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(watercraftNavigationLane2.m_Lane))
						{
							invalidPath = i;
							return;
						}
						if ((watercraftNavigationLane2.m_Flags & WatercraftLaneFlags.EndOfPath) != 0)
						{
							break;
						}
					}
				}
			}
			if ((currentLaneData.m_LaneFlags & WatercraftLaneFlags.UpdateOptimalLane) == 0)
			{
				return;
			}
			currentLaneData.m_LaneFlags &= ~WatercraftLaneFlags.UpdateOptimalLane;
			if ((currentLaneData.m_LaneFlags & WatercraftLaneFlags.IsBlocked) != 0)
			{
				if (IsBlockedLane(currentLaneData.m_Lane, ((float3)(ref currentLaneData.m_CurvePosition)).xz))
				{
					currentLaneData.m_CurvePosition.z = currentLaneData.m_CurvePosition.y;
					invalidPath = -1;
					return;
				}
				for (int j = 0; j < navigationLanes.Length; j++)
				{
					WatercraftNavigationLane watercraftNavigationLane3 = navigationLanes[j];
					if (IsBlockedLane(watercraftNavigationLane3.m_Lane, watercraftNavigationLane3.m_CurvePosition))
					{
						currentLaneData.m_CurvePosition.z = currentLaneData.m_CurvePosition.y;
						invalidPath = j;
						return;
					}
				}
				currentLaneData.m_LaneFlags &= ~(WatercraftLaneFlags.FixedLane | WatercraftLaneFlags.IsBlocked);
				currentLaneData.m_LaneFlags |= WatercraftLaneFlags.IgnoreBlocker;
			}
			WatercraftLaneSelectIterator watercraftLaneSelectIterator = new WatercraftLaneSelectIterator
			{
				m_OwnerData = m_OwnerData,
				m_LaneData = m_LaneData,
				m_SlaveLaneData = m_SlaveLaneData,
				m_LaneReservationData = m_LaneReservationData,
				m_MovingData = m_MovingData,
				m_WatercraftData = m_WatercraftData,
				m_Lanes = m_Lanes,
				m_LaneObjects = m_LaneObjects,
				m_Entity = entity,
				m_Blocker = blocker.m_Blocker,
				m_Priority = priority
			};
			watercraftLaneSelectIterator.SetBuffer(ref laneSelectBuffer);
			if (navigationLanes.Length != 0)
			{
				WatercraftNavigationLane watercraftNavigationLane4 = navigationLanes[navigationLanes.Length - 1];
				watercraftLaneSelectIterator.CalculateLaneCosts(watercraftNavigationLane4, navigationLanes.Length - 1);
				for (int num = navigationLanes.Length - 2; num >= 0; num--)
				{
					WatercraftNavigationLane watercraftNavigationLane5 = navigationLanes[num];
					watercraftLaneSelectIterator.CalculateLaneCosts(watercraftNavigationLane5, watercraftNavigationLane4, num);
					watercraftNavigationLane4 = watercraftNavigationLane5;
				}
				watercraftLaneSelectIterator.UpdateOptimalLane(ref currentLaneData, navigationLanes[0]);
				for (int k = 0; k < navigationLanes.Length; k++)
				{
					WatercraftNavigationLane navLaneData4 = navigationLanes[k];
					watercraftLaneSelectIterator.UpdateOptimalLane(ref navLaneData4);
					navLaneData4.m_Flags &= ~WatercraftLaneFlags.Reserved;
					navigationLanes[k] = navLaneData4;
				}
			}
			else if (currentLaneData.m_CurvePosition.x != currentLaneData.m_CurvePosition.z)
			{
				watercraftLaneSelectIterator.UpdateOptimalLane(ref currentLaneData, default(WatercraftNavigationLane));
			}
		}

		private bool IsBlockedLane(Entity lane, float2 range)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			if (m_SlaveLaneData.HasComponent(lane))
			{
				SlaveLane slaveLane = m_SlaveLaneData[lane];
				Entity owner = m_OwnerData[lane].m_Owner;
				DynamicBuffer<Game.Net.SubLane> val = m_Lanes[owner];
				int num = slaveLane.m_MinIndex - 1;
				if (num < 0 || num > val.Length)
				{
					return false;
				}
				lane = val[num].m_SubLane;
				if (!m_MasterLaneData.HasComponent(lane))
				{
					return false;
				}
			}
			if (!m_CarLaneData.HasComponent(lane))
			{
				return false;
			}
			Game.Net.CarLane carLane = m_CarLaneData[lane];
			if (carLane.m_BlockageEnd < carLane.m_BlockageStart)
			{
				return false;
			}
			if (math.min(range.x, range.y) <= (float)(int)carLane.m_BlockageEnd * 0.003921569f)
			{
				return math.max(range.x, range.y) >= (float)(int)carLane.m_BlockageStart * 0.003921569f;
			}
			return false;
		}

		private bool GetTransformTarget(ref Entity entity, Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			if (m_PropertyRenterData.HasComponent(target.m_Target))
			{
				target.m_Target = m_PropertyRenterData[target.m_Target].m_Property;
			}
			if (m_TransformData.HasComponent(target.m_Target))
			{
				entity = target.m_Target;
				return true;
			}
			if (m_PositionData.HasComponent(target.m_Target))
			{
				entity = target.m_Target;
				return true;
			}
			return false;
		}

		private void UpdateSlaveLane(ref WatercraftNavigationLane navLaneData, float3 targetPosition)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			if (m_SlaveLaneData.HasComponent(navLaneData.m_Lane))
			{
				SlaveLane slaveLane = m_SlaveLaneData[navLaneData.m_Lane];
				Entity owner = m_OwnerData[navLaneData.m_Lane].m_Owner;
				DynamicBuffer<Game.Net.SubLane> lanes = m_Lanes[owner];
				int num = NetUtils.ChooseClosestLane(slaveLane.m_MinIndex, slaveLane.m_MaxIndex, targetPosition, lanes, m_CurveData, navLaneData.m_CurvePosition.y);
				navLaneData.m_Lane = lanes[num].m_SubLane;
			}
			navLaneData.m_Flags |= WatercraftLaneFlags.FixedLane;
		}

		private void Align(ref WatercraftNavigationLane navLaneData, float3 targetPosition)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			if (m_CurveData.HasComponent(navLaneData.m_Lane))
			{
				Curve curve = m_CurveData[navLaneData.m_Lane];
				float3 val = MathUtils.Position(curve.m_Bezier, navLaneData.m_CurvePosition.y);
				float3 val2 = MathUtils.Tangent(curve.m_Bezier, navLaneData.m_CurvePosition.y);
				if (math.dot(MathUtils.Right(((float3)(ref val2)).xz), ((float3)(ref targetPosition)).xz - ((float3)(ref val)).xz) > 0f)
				{
					navLaneData.m_Flags |= WatercraftLaneFlags.AlignRight;
				}
				else
				{
					navLaneData.m_Flags |= WatercraftLaneFlags.AlignLeft;
				}
			}
		}

		private void GetSlaveLaneFromMasterLane(ref Random random, ref WatercraftNavigationLane navLaneData, WatercraftCurrentLane currentLaneData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			if (m_MasterLaneData.HasComponent(navLaneData.m_Lane))
			{
				MasterLane masterLane = m_MasterLaneData[navLaneData.m_Lane];
				Owner owner = m_OwnerData[navLaneData.m_Lane];
				DynamicBuffer<Game.Net.SubLane> lanes = m_Lanes[owner.m_Owner];
				if ((currentLaneData.m_LaneFlags & WatercraftLaneFlags.TransformTarget) != 0)
				{
					float3 position = default(float3);
					if (VehicleUtils.CalculateTransformPosition(ref position, currentLaneData.m_Lane, m_TransformData, m_PositionData, m_PrefabRefData, m_PrefabBuildingData))
					{
						int num = NetUtils.ChooseClosestLane(masterLane.m_MinIndex, masterLane.m_MaxIndex, position, lanes, m_CurveData, navLaneData.m_CurvePosition.y);
						navLaneData.m_Lane = lanes[num].m_SubLane;
						navLaneData.m_Flags |= WatercraftLaneFlags.FixedStart;
					}
					else
					{
						int num2 = ((Random)(ref random)).NextInt((int)masterLane.m_MinIndex, masterLane.m_MaxIndex + 1);
						navLaneData.m_Lane = lanes[num2].m_SubLane;
					}
				}
				else
				{
					float3 comparePosition = MathUtils.Position(m_CurveData[currentLaneData.m_Lane].m_Bezier, currentLaneData.m_CurvePosition.z);
					int num3 = NetUtils.ChooseClosestLane(masterLane.m_MinIndex, masterLane.m_MaxIndex, comparePosition, lanes, m_CurveData, navLaneData.m_CurvePosition.x);
					navLaneData.m_Lane = lanes[num3].m_SubLane;
					navLaneData.m_Flags |= WatercraftLaneFlags.FixedStart;
				}
			}
			else
			{
				navLaneData.m_Flags |= WatercraftLaneFlags.FixedLane;
			}
		}

		private void GetSlaveLaneFromMasterLane(ref Random random, ref WatercraftNavigationLane navLaneData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if (m_MasterLaneData.HasComponent(navLaneData.m_Lane))
			{
				MasterLane masterLane = m_MasterLaneData[navLaneData.m_Lane];
				Entity owner = m_OwnerData[navLaneData.m_Lane].m_Owner;
				DynamicBuffer<Game.Net.SubLane> val = m_Lanes[owner];
				int num = ((Random)(ref random)).NextInt((int)masterLane.m_MinIndex, masterLane.m_MaxIndex + 1);
				navLaneData.m_Lane = val[num].m_SubLane;
			}
			else
			{
				navLaneData.m_Flags |= WatercraftLaneFlags.FixedLane;
			}
		}

		private void CheckBlocker(Watercraft watercraftData, ref WatercraftCurrentLane currentLane, ref Blocker blocker, ref WatercraftLaneSpeedIterator laneIterator)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if (laneIterator.m_Blocker != blocker.m_Blocker)
			{
				currentLane.m_LaneFlags &= ~(WatercraftLaneFlags.IgnoreBlocker | WatercraftLaneFlags.QueueReached);
			}
			if (laneIterator.m_Blocker != Entity.Null)
			{
				if (!m_MovingData.HasComponent(laneIterator.m_Blocker))
				{
					if (m_WatercraftData.HasComponent(laneIterator.m_Blocker))
					{
						if ((m_WatercraftData[laneIterator.m_Blocker].m_Flags & WatercraftFlags.Queueing) != 0 && (currentLane.m_LaneFlags & WatercraftLaneFlags.Queue) != 0)
						{
							if (laneIterator.m_MaxSpeed < 1f)
							{
								currentLane.m_LaneFlags |= WatercraftLaneFlags.QueueReached;
							}
						}
						else
						{
							currentLane.m_LaneFlags |= WatercraftLaneFlags.UpdateOptimalLane;
							if (laneIterator.m_MaxSpeed < 1f)
							{
								currentLane.m_LaneFlags |= WatercraftLaneFlags.IsBlocked;
							}
						}
					}
					else
					{
						currentLane.m_LaneFlags |= WatercraftLaneFlags.UpdateOptimalLane;
						if (laneIterator.m_MaxSpeed < 1f)
						{
							currentLane.m_LaneFlags |= WatercraftLaneFlags.IsBlocked;
						}
					}
				}
				else if (laneIterator.m_Blocker != blocker.m_Blocker)
				{
					currentLane.m_LaneFlags |= WatercraftLaneFlags.UpdateOptimalLane;
				}
			}
			blocker.m_Blocker = laneIterator.m_Blocker;
			blocker.m_Type = laneIterator.m_BlockerType;
			blocker.m_MaxSpeed = (byte)math.clamp(Mathf.RoundToInt(laneIterator.m_MaxSpeed * 4.5899997f), 0, 255);
		}

		private void UpdateNavigationTarget(int priority, Entity entity, Transform transform, Moving moving, Watercraft watercraft, PathOwner pathOwner, PrefabRef prefabRefData, WatercraftData prefabWatercraftData, ObjectGeometryData prefabObjectGeometryData, ref WatercraftNavigation navigation, ref WatercraftCurrentLane currentLane, ref Blocker blocker, ref Odometer odometer, DynamicBuffer<WatercraftNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0add: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b45: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0731: Unknown result type (might be due to invalid IL or missing references)
			//IL_0738: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_079d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0878: Unknown result type (might be due to invalid IL or missing references)
			//IL_0885: Unknown result type (might be due to invalid IL or missing references)
			//IL_0892: Unknown result type (might be due to invalid IL or missing references)
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_081f: Unknown result type (might be due to invalid IL or missing references)
			//IL_082c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0839: Unknown result type (might be due to invalid IL or missing references)
			//IL_0840: Unknown result type (might be due to invalid IL or missing references)
			//IL_098a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0930: Unknown result type (might be due to invalid IL or missing references)
			//IL_0936: Unknown result type (might be due to invalid IL or missing references)
			//IL_093b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0940: Unknown result type (might be due to invalid IL or missing references)
			//IL_0944: Unknown result type (might be due to invalid IL or missing references)
			//IL_08de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a58: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0902: Unknown result type (might be due to invalid IL or missing references)
			//IL_0913: Unknown result type (might be due to invalid IL or missing references)
			//IL_0918: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09de: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			float num2 = math.length(((float3)(ref moving.m_Velocity)).xz);
			float speedLimitFactor = 1f;
			if ((currentLane.m_LaneFlags & WatercraftLaneFlags.Connection) != 0)
			{
				prefabWatercraftData.m_MaxSpeed = 277.77777f;
				prefabWatercraftData.m_Acceleration = 277.77777f;
				prefabWatercraftData.m_Braking = 277.77777f;
			}
			else
			{
				num2 = math.min(num2, prefabWatercraftData.m_MaxSpeed);
			}
			Bounds1 val = default(Bounds1);
			if ((currentLane.m_LaneFlags & (WatercraftLaneFlags.ResetSpeed | WatercraftLaneFlags.Connection)) != 0)
			{
				((Bounds1)(ref val))._002Ector(0f, prefabWatercraftData.m_MaxSpeed);
			}
			else
			{
				val = VehicleUtils.CalculateSpeedRange(prefabWatercraftData, num2, num);
			}
			VehicleUtils.CalculateShipNavigationPivots(transform, prefabObjectGeometryData, out var pivot, out var pivot2);
			float num3 = math.distance(((float3)(ref pivot)).xz, ((float3)(ref pivot2)).xz);
			float3 position = transform.m_Position;
			Curve curve = default(Curve);
			if ((currentLane.m_LaneFlags & (WatercraftLaneFlags.TransformTarget | WatercraftLaneFlags.Area)) == 0 && m_CurveData.TryGetComponent(currentLane.m_Lane, ref curve))
			{
				PrefabRef prefabRef = m_PrefabRefData[currentLane.m_Lane];
				NetLaneData netLaneData = m_PrefabNetLaneData[prefabRef.m_Prefab];
				float3 val2 = MathUtils.Tangent(curve.m_Bezier, currentLane.m_CurvePosition.x);
				float2 xz = ((float3)(ref val2)).xz;
				if (MathUtils.TryNormalize(ref xz))
				{
					((float3)(ref position)).xz = ((float3)(ref position)).xz - MathUtils.Right(xz) * ((netLaneData.m_Width - prefabObjectGeometryData.m_Size.x) * currentLane.m_LanePosition * 0.5f);
				}
			}
			WatercraftLaneSpeedIterator laneIterator = new WatercraftLaneSpeedIterator
			{
				m_TransformData = m_TransformData,
				m_MovingData = m_MovingData,
				m_WatercraftData = m_WatercraftData,
				m_LaneReservationData = m_LaneReservationData,
				m_LaneSignalData = m_LaneSignalData,
				m_CurveData = m_CurveData,
				m_CarLaneData = m_CarLaneData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
				m_PrefabWatercraftData = m_PrefabWatercraftData,
				m_LaneOverlapData = m_LaneOverlaps,
				m_LaneObjectData = m_LaneObjects,
				m_Entity = entity,
				m_Ignore = (((currentLane.m_LaneFlags & WatercraftLaneFlags.IgnoreBlocker) != 0) ? blocker.m_Blocker : Entity.Null),
				m_Priority = priority,
				m_TimeStep = num,
				m_SafeTimeStep = num + 0.5f,
				m_SpeedLimitFactor = speedLimitFactor,
				m_CurrentSpeed = num2,
				m_PrefabWatercraft = prefabWatercraftData,
				m_PrefabObjectGeometry = prefabObjectGeometryData,
				m_SpeedRange = val,
				m_MaxSpeed = val.max,
				m_CanChangeLane = 1f,
				m_CurrentPosition = position
			};
			if ((currentLane.m_LaneFlags & WatercraftLaneFlags.TransformTarget) != 0)
			{
				laneIterator.IterateTarget(navigation.m_TargetPosition);
				navigation.m_MaxSpeed = laneIterator.m_MaxSpeed;
				blocker.m_Blocker = Entity.Null;
				blocker.m_Type = BlockerType.None;
				blocker.m_MaxSpeed = byte.MaxValue;
			}
			else
			{
				if ((currentLane.m_LaneFlags & WatercraftLaneFlags.Area) == 0)
				{
					if (currentLane.m_Lane == Entity.Null)
					{
						navigation.m_MaxSpeed = math.max(0f, num2 - prefabWatercraftData.m_Braking * num);
						blocker.m_Blocker = Entity.Null;
						blocker.m_Type = BlockerType.None;
						blocker.m_MaxSpeed = byte.MaxValue;
						return;
					}
					if (currentLane.m_ChangeLane != Entity.Null)
					{
						if (!laneIterator.IterateFirstLane(currentLane.m_Lane, currentLane.m_ChangeLane, currentLane.m_CurvePosition, currentLane.m_ChangeProgress))
						{
							goto IL_03fd;
						}
					}
					else if (!laneIterator.IterateFirstLane(currentLane.m_Lane, currentLane.m_CurvePosition))
					{
						goto IL_03fd;
					}
					goto IL_0559;
				}
				laneIterator.IterateTarget(navigation.m_TargetPosition, 11.111112f);
				navigation.m_MaxSpeed = laneIterator.m_MaxSpeed;
				blocker.m_Blocker = Entity.Null;
				blocker.m_Type = BlockerType.None;
				blocker.m_MaxSpeed = byte.MaxValue;
			}
			goto IL_0575;
			IL_0559:
			navigation.m_MaxSpeed = laneIterator.m_MaxSpeed;
			CheckBlocker(watercraft, ref currentLane, ref blocker, ref laneIterator);
			goto IL_0575;
			IL_03fd:
			int num4 = 0;
			while (true)
			{
				if (num4 < navigationLanes.Length)
				{
					WatercraftNavigationLane watercraftNavigationLane = navigationLanes[num4];
					if ((watercraftNavigationLane.m_Flags & (WatercraftLaneFlags.TransformTarget | WatercraftLaneFlags.Area)) == 0)
					{
						if ((watercraftNavigationLane.m_Flags & WatercraftLaneFlags.Connection) != 0)
						{
							laneIterator.m_PrefabWatercraft.m_MaxSpeed = 277.77777f;
							laneIterator.m_PrefabWatercraft.m_Acceleration = 277.77777f;
							laneIterator.m_PrefabWatercraft.m_Braking = 277.77777f;
							laneIterator.m_SpeedRange = new Bounds1(0f, 277.77777f);
						}
						else if ((currentLane.m_LaneFlags & WatercraftLaneFlags.Connection) != 0)
						{
							goto IL_054b;
						}
						bool flag = (watercraftNavigationLane.m_Lane == currentLane.m_Lane) | (watercraftNavigationLane.m_Lane == currentLane.m_ChangeLane);
						float minOffset = math.select(-1f, currentLane.m_CurvePosition.y, flag);
						bool needSignal;
						bool num5 = laneIterator.IterateNextLane(watercraftNavigationLane.m_Lane, watercraftNavigationLane.m_CurvePosition, minOffset, out needSignal);
						if (needSignal)
						{
							m_LaneSignals.Enqueue(new WatercraftNavigationHelpers.LaneSignal(entity, watercraftNavigationLane.m_Lane, priority));
						}
						if (num5)
						{
							break;
						}
						num4++;
						continue;
					}
					VehicleUtils.CalculateTransformPosition(ref laneIterator.m_CurrentPosition, watercraftNavigationLane.m_Lane, m_TransformData, m_PositionData, m_PrefabRefData, m_PrefabBuildingData);
				}
				goto IL_054b;
				IL_054b:
				laneIterator.IterateTarget(laneIterator.m_CurrentPosition);
				break;
			}
			goto IL_0559;
			IL_0575:
			float3 val3 = navigation.m_TargetPosition - transform.m_Position;
			float num6 = math.length(((float3)(ref val3)).xz);
			float num7 = navigation.m_MaxSpeed * num + 1f + num3 * 0.5f;
			float num8 = num7;
			if ((currentLane.m_LaneFlags & WatercraftLaneFlags.Area) != 0)
			{
				float brakingDistance = VehicleUtils.GetBrakingDistance(prefabWatercraftData, navigation.m_MaxSpeed, num);
				num8 = math.max(num7, brakingDistance + 1f + num3 * 0.5f);
				num6 = math.select(num6, 0f, currentLane.m_ChangeProgress != 0f);
			}
			if (currentLane.m_ChangeLane != Entity.Null)
			{
				float num9 = 0.05f;
				float num10 = 1f + prefabObjectGeometryData.m_Size.z * num9 * 0.5f;
				float2 val4 = default(float2);
				((float2)(ref val4))._002Ector(0.4f, 0.6f * math.saturate(num2 * num9));
				val4 *= laneIterator.m_CanChangeLane * num;
				val4.x = math.min(val4.x, math.max(0f, 1f - currentLane.m_ChangeProgress));
				currentLane.m_ChangeProgress = math.min(num10, currentLane.m_ChangeProgress + math.csum(val4));
				if (currentLane.m_ChangeProgress == num10)
				{
					ApplySideEffects(ref currentLane, speedLimitFactor, prefabRefData, prefabWatercraftData);
					currentLane.m_Lane = currentLane.m_ChangeLane;
					currentLane.m_ChangeLane = Entity.Null;
				}
			}
			currentLane.m_Duration += num;
			currentLane.m_Distance += num2 * num;
			odometer.m_Distance += num2 * num;
			if (num6 < num8)
			{
				while (true)
				{
					if ((currentLane.m_LaneFlags & WatercraftLaneFlags.TransformTarget) != 0)
					{
						navigation.m_TargetDirection = default(float3);
						if (MoveTarget(position, ref navigation.m_TargetPosition, num7, currentLane.m_Lane))
						{
							break;
						}
					}
					else if ((currentLane.m_LaneFlags & WatercraftLaneFlags.Area) != 0)
					{
						navigation.m_TargetDirection = default(float3);
						currentLane.m_LanePosition = math.clamp(currentLane.m_LanePosition, -0.5f, 0.5f);
						float navigationSize = VehicleUtils.GetNavigationSize(prefabObjectGeometryData);
						bool num11 = MoveAreaTarget(transform.m_Position, pathOwner, navigationLanes, pathElements, ref navigation.m_TargetPosition, num8, currentLane.m_Lane, ref currentLane.m_CurvePosition, currentLane.m_LanePosition, navigationSize);
						currentLane.m_ChangeProgress = 0f;
						if (num11)
						{
							break;
						}
					}
					else
					{
						navigation.m_TargetDirection = default(float3);
						if (currentLane.m_ChangeLane != Entity.Null)
						{
							Curve curve2 = m_CurveData[currentLane.m_Lane];
							Curve curve3 = m_CurveData[currentLane.m_ChangeLane];
							if (MoveTarget(position, ref navigation.m_TargetPosition, num7, curve2.m_Bezier, curve3.m_Bezier, currentLane.m_ChangeProgress, ref currentLane.m_CurvePosition))
							{
								ApplyLanePosition(ref navigation.m_TargetPosition, ref currentLane, prefabObjectGeometryData);
								break;
							}
						}
						else
						{
							Curve curve4 = m_CurveData[currentLane.m_Lane];
							if (MoveTarget(position, ref navigation.m_TargetPosition, num7, curve4.m_Bezier, ref currentLane.m_CurvePosition))
							{
								ApplyLanePosition(ref navigation.m_TargetPosition, ref currentLane, prefabObjectGeometryData);
								break;
							}
						}
					}
					if (navigationLanes.Length == 0)
					{
						if ((currentLane.m_LaneFlags & WatercraftLaneFlags.Area) == 0 && m_CurveData.HasComponent(currentLane.m_Lane))
						{
							navigation.m_TargetDirection = MathUtils.Tangent(m_CurveData[currentLane.m_Lane].m_Bezier, currentLane.m_CurvePosition.z);
							ApplyLanePosition(ref navigation.m_TargetPosition, ref currentLane, prefabObjectGeometryData);
						}
						val3 = navigation.m_TargetPosition - transform.m_Position;
						num6 = math.length(((float3)(ref val3)).xz);
						if (num6 < 1f && num2 < 0.1f)
						{
							currentLane.m_LaneFlags |= WatercraftLaneFlags.EndReached;
						}
						break;
					}
					WatercraftNavigationLane watercraftNavigationLane2 = navigationLanes[0];
					if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(watercraftNavigationLane2.m_Lane))
					{
						break;
					}
					if ((currentLane.m_LaneFlags & WatercraftLaneFlags.Connection) != 0)
					{
						if ((watercraftNavigationLane2.m_Flags & WatercraftLaneFlags.TransformTarget) != 0)
						{
							watercraftNavigationLane2.m_Flags |= WatercraftLaneFlags.ResetSpeed;
						}
						else if ((watercraftNavigationLane2.m_Flags & WatercraftLaneFlags.Connection) == 0)
						{
							val3 = navigation.m_TargetPosition - transform.m_Position;
							num6 = math.length(((float3)(ref val3)).xz);
							if (num6 >= 1f || num2 > 3f)
							{
								break;
							}
							watercraftNavigationLane2.m_Flags |= WatercraftLaneFlags.ResetSpeed;
						}
					}
					ApplySideEffects(ref currentLane, speedLimitFactor, prefabRefData, prefabWatercraftData);
					currentLane.m_Lane = watercraftNavigationLane2.m_Lane;
					currentLane.m_ChangeLane = Entity.Null;
					currentLane.m_ChangeProgress = 0f;
					currentLane.m_CurvePosition = ((float2)(ref watercraftNavigationLane2.m_CurvePosition)).xxy;
					currentLane.m_LaneFlags = watercraftNavigationLane2.m_Flags;
					if ((currentLane.m_LaneFlags & (WatercraftLaneFlags.AlignLeft | WatercraftLaneFlags.AlignRight)) != 0)
					{
						currentLane.m_LanePosition = math.select(-1f, 1f, (currentLane.m_LaneFlags & WatercraftLaneFlags.AlignRight) != 0);
					}
					else
					{
						currentLane.m_LanePosition = 0f;
					}
					navigationLanes.RemoveAt(0);
				}
			}
			if ((currentLane.m_LaneFlags & WatercraftLaneFlags.Area) != 0)
			{
				VehicleCollisionIterator vehicleCollisionIterator = new VehicleCollisionIterator
				{
					m_OwnerData = m_OwnerData,
					m_TransformData = m_TransformData,
					m_MovingData = m_MovingData,
					m_ControllerData = m_ControllerData,
					m_CreatureData = m_CreatureData,
					m_CurveData = m_CurveData,
					m_AreaLaneData = m_AreaLaneData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
					m_PrefabLaneData = m_PrefabNetLaneData,
					m_AreaNodes = m_AreaNodes,
					m_StaticObjectSearchTree = m_StaticObjectSearchTree,
					m_MovingObjectSearchTree = m_MovingObjectSearchTree,
					m_Entity = entity,
					m_CurrentLane = currentLane.m_Lane,
					m_CurvePosition = currentLane.m_CurvePosition.z,
					m_TimeStep = num,
					m_PrefabObjectGeometry = prefabObjectGeometryData,
					m_SpeedRange = val,
					m_CurrentPosition = transform.m_Position,
					m_CurrentVelocity = moving.m_Velocity,
					m_MinDistance = num8,
					m_TargetPosition = navigation.m_TargetPosition,
					m_MaxSpeed = navigation.m_MaxSpeed,
					m_LanePosition = currentLane.m_LanePosition,
					m_Blocker = blocker.m_Blocker,
					m_BlockerType = blocker.m_Type
				};
				if (vehicleCollisionIterator.m_MaxSpeed != 0f)
				{
					vehicleCollisionIterator.IterateFirstLane(currentLane.m_Lane);
					vehicleCollisionIterator.m_MaxSpeed = math.select(vehicleCollisionIterator.m_MaxSpeed, 0f, vehicleCollisionIterator.m_MaxSpeed < 0.1f);
					if (!((float3)(ref navigation.m_TargetPosition)).Equals(vehicleCollisionIterator.m_TargetPosition))
					{
						navigation.m_TargetPosition = vehicleCollisionIterator.m_TargetPosition;
						currentLane.m_LanePosition = math.lerp(currentLane.m_LanePosition, vehicleCollisionIterator.m_LanePosition, 0.1f);
						currentLane.m_ChangeProgress = 1f;
					}
					navigation.m_MaxSpeed = vehicleCollisionIterator.m_MaxSpeed;
					blocker.m_Blocker = vehicleCollisionIterator.m_Blocker;
					blocker.m_Type = vehicleCollisionIterator.m_BlockerType;
					blocker.m_MaxSpeed = (byte)math.clamp(Mathf.RoundToInt(vehicleCollisionIterator.m_MaxSpeed * 4.5899997f), 0, 255);
				}
			}
			navigation.m_MaxSpeed = math.min(navigation.m_MaxSpeed, math.distance(((float3)(ref transform.m_Position)).xz, ((float3)(ref navigation.m_TargetPosition)).xz) / num);
		}

		private void ApplyLanePosition(ref float3 targetPosition, ref WatercraftCurrentLane currentLaneData, ObjectGeometryData prefabObjectGeometryData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			if (m_CurveData.HasComponent(currentLaneData.m_Lane))
			{
				Curve curve = m_CurveData[currentLaneData.m_Lane];
				PrefabRef prefabRef = m_PrefabRefData[currentLaneData.m_Lane];
				NetLaneData netLaneData = m_PrefabNetLaneData[prefabRef.m_Prefab];
				float3 val = MathUtils.Tangent(curve.m_Bezier, currentLaneData.m_CurvePosition.x);
				float2 xz = ((float3)(ref val)).xz;
				if (MathUtils.TryNormalize(ref xz))
				{
					((float3)(ref targetPosition)).xz = ((float3)(ref targetPosition)).xz + MathUtils.Right(xz) * ((netLaneData.m_Width - prefabObjectGeometryData.m_Size.x) * currentLaneData.m_LanePosition * 0.5f);
				}
			}
		}

		private void ApplySideEffects(ref WatercraftCurrentLane currentLaneData, float speedLimitFactor, PrefabRef prefabRefData, WatercraftData prefabWatercraftData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			if (m_CarLaneData.HasComponent(currentLaneData.m_Lane))
			{
				Game.Net.CarLane carLaneData = m_CarLaneData[currentLaneData.m_Lane];
				carLaneData.m_SpeedLimit *= speedLimitFactor;
				float maxDriveSpeed = VehicleUtils.GetMaxDriveSpeed(prefabWatercraftData, carLaneData);
				float num = math.select(currentLaneData.m_Distance / currentLaneData.m_Duration, maxDriveSpeed, currentLaneData.m_Duration == 0f);
				float relativeSpeed = num / maxDriveSpeed;
				float3 sideEffects = default(float3);
				if (m_PrefabSideEffectData.HasComponent(prefabRefData.m_Prefab))
				{
					VehicleSideEffectData vehicleSideEffectData = m_PrefabSideEffectData[prefabRefData.m_Prefab];
					float num2 = num / prefabWatercraftData.m_MaxSpeed;
					num2 = math.saturate(num2 * num2);
					sideEffects = math.lerp(vehicleSideEffectData.m_Min, vehicleSideEffectData.m_Max, num2);
					sideEffects *= new float3(currentLaneData.m_Distance, currentLaneData.m_Duration, currentLaneData.m_Duration);
				}
				m_LaneEffects.Enqueue(new WatercraftNavigationHelpers.LaneEffects(currentLaneData.m_Lane, sideEffects, relativeSpeed));
			}
			currentLaneData.m_Duration = 0f;
			currentLaneData.m_Distance = 0f;
		}

		private void ReserveNavigationLanes(int priority, WatercraftData prefabWatercraftData, Watercraft watercraftData, ref WatercraftNavigation navigationData, ref WatercraftCurrentLane currentLaneData, DynamicBuffer<WatercraftNavigationLane> navigationLanes)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			float timeStep = 4f / 15f;
			if (!m_CarLaneData.HasComponent(currentLaneData.m_Lane))
			{
				return;
			}
			Curve curve = m_CurveData[currentLaneData.m_Lane];
			float num = math.max(0f, VehicleUtils.GetBrakingDistance(prefabWatercraftData, navigationData.m_MaxSpeed, timeStep) - 0.01f);
			currentLaneData.m_CurvePosition.y = currentLaneData.m_CurvePosition.x + num / math.max(1E-06f, curve.m_Length);
			num -= curve.m_Length * math.abs(currentLaneData.m_CurvePosition.z - currentLaneData.m_CurvePosition.x);
			int num2 = 0;
			if (!(currentLaneData.m_CurvePosition.y > currentLaneData.m_CurvePosition.z))
			{
				return;
			}
			currentLaneData.m_CurvePosition.y = currentLaneData.m_CurvePosition.z;
			while (num2 < navigationLanes.Length && num > 0f)
			{
				WatercraftNavigationLane watercraftNavigationLane = navigationLanes[num2];
				if (m_CarLaneData.HasComponent(watercraftNavigationLane.m_Lane))
				{
					curve = m_CurveData[watercraftNavigationLane.m_Lane];
					float offset = math.min(watercraftNavigationLane.m_CurvePosition.y, watercraftNavigationLane.m_CurvePosition.x + num / math.max(1E-06f, curve.m_Length));
					if (m_LaneReservationData.HasComponent(watercraftNavigationLane.m_Lane))
					{
						m_LaneReservations.Enqueue(new WatercraftNavigationHelpers.LaneReservation(watercraftNavigationLane.m_Lane, offset, priority));
					}
					num -= curve.m_Length * math.abs(watercraftNavigationLane.m_CurvePosition.y - watercraftNavigationLane.m_CurvePosition.x);
					watercraftNavigationLane.m_Flags |= WatercraftLaneFlags.Reserved;
					navigationLanes[num2++] = watercraftNavigationLane;
					continue;
				}
				break;
			}
		}

		private void ReserveOtherLanesInGroup(Entity lane, int priority)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SlaveLaneData.HasComponent(lane))
			{
				return;
			}
			SlaveLane slaveLane = m_SlaveLaneData[lane];
			Owner owner = m_OwnerData[lane];
			DynamicBuffer<Game.Net.SubLane> val = m_Lanes[owner.m_Owner];
			int num = math.min((int)slaveLane.m_MaxIndex, val.Length - 1);
			for (int i = slaveLane.m_MinIndex; i <= num; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (subLane != lane && m_LaneReservationData.HasComponent(subLane))
				{
					m_LaneReservations.Enqueue(new WatercraftNavigationHelpers.LaneReservation(subLane, 0f, priority));
				}
			}
		}

		private bool MoveAreaTarget(float3 comparePosition, PathOwner pathOwner, DynamicBuffer<WatercraftNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements, ref float3 targetPosition, float minDistance, Entity target, ref float3 curveDelta, float lanePosition, float navigationSize)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Obsolete | PathFlags.Updated)) != 0)
			{
				return true;
			}
			Entity owner = m_OwnerData[target].m_Owner;
			AreaLane areaLane = m_AreaLaneData[target];
			DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[owner];
			int num = math.min(pathOwner.m_ElementIndex, pathElements.Length);
			NativeArray<PathElement> subArray = pathElements.AsNativeArray().GetSubArray(num, pathElements.Length - num);
			num = 0;
			bool flag = curveDelta.z < curveDelta.x;
			float lanePosition2 = math.select(lanePosition, 0f - lanePosition, flag);
			if (areaLane.m_Nodes.y == areaLane.m_Nodes.z)
			{
				float3 position = nodes[areaLane.m_Nodes.x].m_Position;
				float3 position2 = nodes[areaLane.m_Nodes.y].m_Position;
				float3 position3 = nodes[areaLane.m_Nodes.w].m_Position;
				if (VehicleUtils.SetTriangleTarget(position, position2, position3, comparePosition, num, navigationLanes, subArray, ref targetPosition, minDistance, lanePosition2, curveDelta.z, navigationSize, isSingle: true, m_TransformData, m_AreaLaneData, m_CurveData))
				{
					return true;
				}
				curveDelta.y = curveDelta.z;
			}
			else
			{
				bool4 val = default(bool4);
				((bool4)(ref val))._002Ector(((float3)(ref curveDelta)).yz < 0.5f, ((float3)(ref curveDelta)).yz > 0.5f);
				int2 val2 = math.select(int2.op_Implicit(areaLane.m_Nodes.x), int2.op_Implicit(areaLane.m_Nodes.w), ((bool4)(ref val)).zw);
				float3 position4 = nodes[val2.x].m_Position;
				float3 position5 = nodes[areaLane.m_Nodes.y].m_Position;
				float3 position6 = nodes[areaLane.m_Nodes.z].m_Position;
				float3 position7 = nodes[val2.y].m_Position;
				if (math.any(((bool4)(ref val)).xy & ((bool4)(ref val)).wz))
				{
					if (VehicleUtils.SetAreaTarget(position4, position4, position5, position6, position7, owner, nodes, comparePosition, num, navigationLanes, subArray, ref targetPosition, minDistance, lanePosition2, curveDelta.z, navigationSize, flag, m_TransformData, m_AreaLaneData, m_CurveData, m_OwnerData))
					{
						return true;
					}
					curveDelta.y = 0.5f;
					((bool4)(ref val)).xz = bool2.op_Implicit(false);
				}
				Owner owner2 = default(Owner);
				if (VehicleUtils.GetPathElement(num, navigationLanes, subArray, out var pathElement) && m_OwnerData.TryGetComponent(pathElement.m_Target, ref owner2) && owner2.m_Owner == owner)
				{
					bool4 val3 = default(bool4);
					((bool4)(ref val3))._002Ector(pathElement.m_TargetDelta < 0.5f, pathElement.m_TargetDelta > 0.5f);
					if (math.any(!((bool4)(ref val)).xz) & math.any(((bool4)(ref val)).yw) & math.any(((bool4)(ref val3)).xy & ((bool4)(ref val3)).wz))
					{
						AreaLane areaLane2 = m_AreaLaneData[pathElement.m_Target];
						val2 = math.select(int2.op_Implicit(areaLane2.m_Nodes.x), int2.op_Implicit(areaLane2.m_Nodes.w), ((bool4)(ref val3)).zw);
						position4 = nodes[val2.x].m_Position;
						float3 prev = math.select(position5, position6, ((float3)(ref position4)).Equals(position5));
						position5 = nodes[areaLane2.m_Nodes.y].m_Position;
						position6 = nodes[areaLane2.m_Nodes.z].m_Position;
						position7 = nodes[val2.y].m_Position;
						bool flag2 = pathElement.m_TargetDelta.y < pathElement.m_TargetDelta.x;
						if (VehicleUtils.SetAreaTarget(lanePosition: math.select(lanePosition, 0f - lanePosition, flag2), prev2: prev, prev: position4, left: position5, right: position6, next: position7, areaEntity: owner, nodes: nodes, comparePosition: comparePosition, elementIndex: num + 1, navigationLanes: navigationLanes, pathElements: subArray, targetPosition: ref targetPosition, minDistance: minDistance, curveDelta: pathElement.m_TargetDelta.y, navigationSize: navigationSize, isBackward: flag2, transforms: m_TransformData, areaLanes: m_AreaLaneData, curves: m_CurveData, owners: m_OwnerData))
						{
							return true;
						}
					}
					curveDelta.y = curveDelta.z;
					return false;
				}
				if (VehicleUtils.SetTriangleTarget(position5, position6, position7, comparePosition, num, navigationLanes, subArray, ref targetPosition, minDistance, lanePosition2, curveDelta.z, navigationSize, isSingle: false, m_TransformData, m_AreaLaneData, m_CurveData))
				{
					return true;
				}
				curveDelta.y = curveDelta.z;
			}
			return math.distance(((float3)(ref comparePosition)).xz, ((float3)(ref targetPosition)).xz) >= minDistance;
		}

		private bool MoveTarget(float3 comparePosition, ref float3 targetPosition, float minDistance, Entity target)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (VehicleUtils.CalculateTransformPosition(ref targetPosition, target, m_TransformData, m_PositionData, m_PrefabRefData, m_PrefabBuildingData))
			{
				return math.distance(((float3)(ref comparePosition)).xz, ((float3)(ref targetPosition)).xz) >= minDistance;
			}
			return false;
		}

		private bool MoveTarget(float3 comparePosition, ref float3 targetPosition, float minDistance, Bezier4x3 curve, ref float3 curveDelta)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.Position(curve, curveDelta.z);
			if (math.distance(((float3)(ref comparePosition)).xz, ((float3)(ref val)).xz) < minDistance)
			{
				curveDelta.x = curveDelta.z;
				targetPosition = val;
				return false;
			}
			float2 xz = ((float3)(ref curveDelta)).xz;
			for (int i = 0; i < 8; i++)
			{
				float num = math.lerp(xz.x, xz.y, 0.5f);
				float3 val2 = MathUtils.Position(curve, num);
				if (math.distance(((float3)(ref comparePosition)).xz, ((float3)(ref val2)).xz) < minDistance)
				{
					xz.x = num;
				}
				else
				{
					xz.y = num;
				}
			}
			curveDelta.x = xz.y;
			targetPosition = MathUtils.Position(curve, xz.y);
			return true;
		}

		private bool MoveTarget(float3 comparePosition, ref float3 targetPosition, float minDistance, Bezier4x3 curve1, Bezier4x3 curve2, float curveSelect, ref float3 curveDelta)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			curveSelect = math.saturate(curveSelect);
			float3 val = MathUtils.Position(curve1, curveDelta.z);
			float3 val2 = MathUtils.Position(curve2, curveDelta.z);
			float3 val3 = math.lerp(val, val2, curveSelect);
			if (math.distance(((float3)(ref comparePosition)).xz, ((float3)(ref val3)).xz) < minDistance)
			{
				curveDelta.x = curveDelta.z;
				targetPosition = val3;
				return false;
			}
			float2 xz = ((float3)(ref curveDelta)).xz;
			for (int i = 0; i < 8; i++)
			{
				float num = math.lerp(xz.x, xz.y, 0.5f);
				float3 val4 = MathUtils.Position(curve1, num);
				float3 val5 = MathUtils.Position(curve2, num);
				float3 val6 = math.lerp(val4, val5, curveSelect);
				if (math.distance(((float3)(ref comparePosition)).xz, ((float3)(ref val6)).xz) < minDistance)
				{
					xz.x = num;
				}
				else
				{
					xz.y = num;
				}
			}
			curveDelta.x = xz.y;
			float3 val7 = MathUtils.Position(curve1, xz.y);
			float3 val8 = MathUtils.Position(curve2, xz.y);
			targetPosition = math.lerp(val7, val8, curveSelect);
			return true;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct GroupLaneReservationsJob : IJob
	{
		public NativeQueue<WatercraftNavigationHelpers.LaneReservation> m_LaneReservationQueue;

		public NativeList<WatercraftNavigationHelpers.LaneReservation> m_LaneReservationList;

		public NativeList<int2> m_LaneReservationGroups;

		public void Execute()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			WatercraftNavigationHelpers.LaneReservation laneReservation = default(WatercraftNavigationHelpers.LaneReservation);
			while (m_LaneReservationQueue.TryDequeue(ref laneReservation))
			{
				m_LaneReservationList.Add(ref laneReservation);
			}
			NativeSortExtension.Sort<WatercraftNavigationHelpers.LaneReservation>(m_LaneReservationList);
			Entity val = Entity.Null;
			int num = 0;
			for (int i = 0; i < m_LaneReservationList.Length; i++)
			{
				laneReservation = m_LaneReservationList[i];
				if (val != laneReservation.m_Lane)
				{
					if (val != Entity.Null)
					{
						ref NativeList<int2> reference = ref m_LaneReservationGroups;
						int2 val2 = new int2(num, i);
						reference.Add(ref val2);
					}
					val = laneReservation.m_Lane;
					num = i;
				}
			}
			if (val != Entity.Null)
			{
				ref NativeList<int2> reference2 = ref m_LaneReservationGroups;
				int2 val2 = new int2(num, m_LaneReservationList.Length);
				reference2.Add(ref val2);
			}
		}
	}

	[BurstCompile]
	private struct UpdateLaneSignalsJob : IJob
	{
		public NativeQueue<WatercraftNavigationHelpers.LaneSignal> m_LaneSignalQueue;

		public ComponentLookup<LaneSignal> m_LaneSignalData;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			WatercraftNavigationHelpers.LaneSignal laneSignal = default(WatercraftNavigationHelpers.LaneSignal);
			while (m_LaneSignalQueue.TryDequeue(ref laneSignal))
			{
				LaneSignal laneSignal2 = m_LaneSignalData[laneSignal.m_Lane];
				if (laneSignal.m_Priority > laneSignal2.m_Priority)
				{
					laneSignal2.m_Petitioner = laneSignal.m_Petitioner;
					laneSignal2.m_Priority = laneSignal.m_Priority;
					m_LaneSignalData[laneSignal.m_Lane] = laneSignal2;
				}
			}
		}
	}

	[BurstCompile]
	private struct UpdateLaneReservationsJob : IJob
	{
		public NativeQueue<WatercraftNavigationHelpers.LaneReservation> m_LaneReservationQueue;

		public ComponentLookup<LaneReservation> m_LaneReservationData;

		public void Execute()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			WatercraftNavigationHelpers.LaneReservation laneReservation = default(WatercraftNavigationHelpers.LaneReservation);
			while (m_LaneReservationQueue.TryDequeue(ref laneReservation))
			{
				ref LaneReservation valueRW = ref m_LaneReservationData.GetRefRW(laneReservation.m_Lane).ValueRW;
				if (laneReservation.m_Offset > valueRW.m_Next.m_Offset)
				{
					valueRW.m_Next.m_Offset = laneReservation.m_Offset;
				}
				if (laneReservation.m_Priority > valueRW.m_Next.m_Priority)
				{
					if (laneReservation.m_Priority >= valueRW.m_Prev.m_Priority)
					{
						valueRW.m_Blocker = Entity.Null;
					}
					valueRW.m_Next.m_Priority = laneReservation.m_Priority;
				}
			}
		}
	}

	[BurstCompile]
	private struct ApplyLaneEffectsJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		public ComponentLookup<Game.Net.Pollution> m_PollutionData;

		public NativeQueue<WatercraftNavigationHelpers.LaneEffects> m_LaneEffectsQueue;

		public void Execute()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			WatercraftNavigationHelpers.LaneEffects laneEffects = default(WatercraftNavigationHelpers.LaneEffects);
			while (m_LaneEffectsQueue.TryDequeue(ref laneEffects))
			{
				Entity owner = m_OwnerData[laneEffects.m_Lane].m_Owner;
				if (m_PollutionData.HasComponent(owner))
				{
					Game.Net.Pollution pollution = m_PollutionData[owner];
					ref float2 pollution2 = ref pollution.m_Pollution;
					pollution2 += ((float3)(ref laneEffects.m_SideEffects)).yz;
					m_PollutionData[owner] = pollution;
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
		public ComponentTypeHandle<Moving> __Game_Objects_Moving_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Target> __Game_Common_Target_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Watercraft> __Game_Vehicles_Watercraft_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<WatercraftNavigation> __Game_Vehicles_WatercraftNavigation_RW_ComponentTypeHandle;

		public ComponentTypeHandle<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Blocker> __Game_Vehicles_Blocker_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Odometer> __Game_Vehicles_Odometer_RW_ComponentTypeHandle;

		public BufferTypeHandle<WatercraftNavigationLane> __Game_Vehicles_WatercraftNavigationLane_RW_BufferTypeHandle;

		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneReservation> __Game_Net_LaneReservation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneSignal> __Game_Net_LaneSignal_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaLane> __Game_Net_AreaLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TakeoffLocation> __Game_Routes_TakeoffLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Watercraft> __Game_Vehicles_Watercraft_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Creature> __Game_Creatures_Creature_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WatercraftData> __Game_Prefabs_WatercraftData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<VehicleSideEffectData> __Game_Prefabs_VehicleSideEffectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		public ComponentLookup<LaneReservation> __Game_Net_LaneReservation_RW_ComponentLookup;

		public ComponentLookup<Game.Net.Pollution> __Game_Net_Pollution_RW_ComponentLookup;

		public ComponentLookup<LaneSignal> __Game_Net_LaneSignal_RW_ComponentLookup;

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
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_Moving_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Moving>(true);
			__Game_Common_Target_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(true);
			__Game_Vehicles_Watercraft_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Watercraft>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Vehicles_WatercraftNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WatercraftNavigation>(false);
			__Game_Vehicles_WatercraftCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WatercraftCurrentLane>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Vehicles_Blocker_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Blocker>(false);
			__Game_Vehicles_Odometer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Odometer>(false);
			__Game_Vehicles_WatercraftNavigationLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<WatercraftNavigationLane>(false);
			__Game_Pathfind_PathElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_LaneReservation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneReservation>(true);
			__Game_Net_LaneSignal_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(true);
			__Game_Net_AreaLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaLane>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Routes_TakeoffLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.TakeoffLocation>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Vehicles_Watercraft_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Watercraft>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Creatures_Creature_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creature>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_WatercraftData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_VehicleSideEffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VehicleSideEffectData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Net_LaneReservation_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneReservation>(false);
			__Game_Net_Pollution_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Pollution>(false);
			__Game_Net_LaneSignal_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(false);
		}
	}

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private EntityQuery m_VehicleQuery;

	private LaneObjectUpdater m_LaneObjectUpdater;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 8;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[13]
		{
			ComponentType.ReadOnly<Watercraft>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<Target>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadOnly<PathElement>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<WatercraftCurrentLane>(),
			ComponentType.ReadWrite<WatercraftNavigation>(),
			ComponentType.ReadWrite<WatercraftNavigationLane>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>()
		});
		m_LaneObjectUpdater = new LaneObjectUpdater((SystemBase)(object)this);
		((ComponentSystemBase)this).RequireForUpdate(m_VehicleQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_075e: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<WatercraftNavigationHelpers.LaneReservation> laneReservationQueue = default(NativeQueue<WatercraftNavigationHelpers.LaneReservation>);
		laneReservationQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<WatercraftNavigationHelpers.LaneEffects> laneEffectsQueue = default(NativeQueue<WatercraftNavigationHelpers.LaneEffects>);
		laneEffectsQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<WatercraftNavigationHelpers.LaneSignal> laneSignalQueue = default(NativeQueue<WatercraftNavigationHelpers.LaneSignal>);
		laneSignalQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle dependencies4;
		UpdateNavigationJob updateNavigationJob = new UpdateNavigationJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MovingType = InternalCompilerInterface.GetComponentTypeHandle<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftType = InternalCompilerInterface.GetComponentTypeHandle<Watercraft>(ref __TypeHandle.__Game_Vehicles_Watercraft_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationType = InternalCompilerInterface.GetComponentTypeHandle<WatercraftNavigation>(ref __TypeHandle.__Game_Vehicles_WatercraftNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerType = InternalCompilerInterface.GetComponentTypeHandle<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OdometerType = InternalCompilerInterface.GetComponentTypeHandle<Odometer>(ref __TypeHandle.__Game_Vehicles_Odometer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<WatercraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_WatercraftNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneReservationData = InternalCompilerInterface.GetComponentLookup<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaLaneData = InternalCompilerInterface.GetComponentLookup<AreaLane>(ref __TypeHandle.__Game_Net_AreaLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterData = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TakeoffLocationData = InternalCompilerInterface.GetComponentLookup<Game.Routes.TakeoffLocation>(ref __TypeHandle.__Game_Routes_TakeoffLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftData = InternalCompilerInterface.GetComponentLookup<Watercraft>(ref __TypeHandle.__Game_Vehicles_Watercraft_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureData = InternalCompilerInterface.GetComponentLookup<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWatercraftData = InternalCompilerInterface.GetComponentLookup<WatercraftData>(ref __TypeHandle.__Game_Prefabs_WatercraftData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSideEffectData = InternalCompilerInterface.GetComponentLookup<VehicleSideEffectData>(ref __TypeHandle.__Game_Prefabs_VehicleSideEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies2),
			m_StaticObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies3),
			m_MovingObjectSearchTree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: true, out dependencies4),
			m_LaneObjectBuffer = m_LaneObjectUpdater.Begin((Allocator)3),
			m_LaneReservations = laneReservationQueue.AsParallelWriter(),
			m_LaneEffects = laneEffectsQueue.AsParallelWriter(),
			m_LaneSignals = laneSignalQueue.AsParallelWriter()
		};
		UpdateLaneReservationsJob updateLaneReservationsJob = new UpdateLaneReservationsJob
		{
			m_LaneReservationQueue = laneReservationQueue,
			m_LaneReservationData = InternalCompilerInterface.GetComponentLookup<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		ApplyLaneEffectsJob applyLaneEffectsJob = new ApplyLaneEffectsJob
		{
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionData = InternalCompilerInterface.GetComponentLookup<Game.Net.Pollution>(ref __TypeHandle.__Game_Net_Pollution_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneEffectsQueue = laneEffectsQueue
		};
		UpdateLaneSignalsJob obj = new UpdateLaneSignalsJob
		{
			m_LaneSignalQueue = laneSignalQueue,
			m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		JobHandle val = JobChunkExtensions.ScheduleParallel<UpdateNavigationJob>(updateNavigationJob, m_VehicleQuery, JobUtils.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2, dependencies3, dependencies4));
		JobHandle val2 = IJobExtensions.Schedule<UpdateLaneReservationsJob>(updateLaneReservationsJob, val);
		JobHandle val3 = IJobExtensions.Schedule<ApplyLaneEffectsJob>(applyLaneEffectsJob, val);
		JobHandle val4 = IJobExtensions.Schedule<UpdateLaneSignalsJob>(obj, val);
		laneReservationQueue.Dispose(val2);
		laneEffectsQueue.Dispose(val3);
		laneSignalQueue.Dispose(val4);
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		m_AreaSearchSystem.AddSearchTreeReader(val);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val);
		m_ObjectSearchSystem.AddMovingSearchTreeReader(val);
		JobHandle val5 = m_LaneObjectUpdater.Apply((SystemBase)(object)this, val);
		((SystemBase)this).Dependency = JobUtils.CombineDependencies(val5, val2, val3, val4);
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
	public WatercraftNavigationSystem()
	{
	}
}

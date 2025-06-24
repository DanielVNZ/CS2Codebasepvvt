using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
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
public class AircraftNavigationSystem : GameSystemBase
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
		public ComponentTypeHandle<Aircraft> m_AircraftType;

		[ReadOnly]
		public ComponentTypeHandle<Helicopter> m_HelicopterType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<AircraftNavigation> m_NavigationType;

		public ComponentTypeHandle<AircraftCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public ComponentTypeHandle<Blocker> m_BlockerType;

		public ComponentTypeHandle<Odometer> m_OdometerType;

		public BufferTypeHandle<AircraftNavigationLane> m_NavigationLaneType;

		public BufferTypeHandle<PathElement> m_PathElementType;

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
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<LaneReservation> m_LaneReservationData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TakeoffLocation> m_TakeoffLocationData;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Aircraft> m_AircraftData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<AircraftData> m_PrefabAircraftData;

		[ReadOnly]
		public ComponentLookup<HelicopterData> m_PrefabHelicopterData;

		[ReadOnly]
		public ComponentLookup<AirplaneData> m_PrefabAirplaneData;

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
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingObjectSearchTree;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public AirwayHelpers.AirwayData m_AirwayData;

		public LaneObjectCommandBuffer m_LaneObjectBuffer;

		public ParallelWriter<AircraftNavigationHelpers.LaneReservation> m_LaneReservations;

		public ParallelWriter<AircraftNavigationHelpers.LaneEffects> m_LaneEffects;

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
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Moving> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
			NativeArray<AircraftNavigation> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftNavigation>(ref m_NavigationType);
			NativeArray<AircraftCurrentLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftCurrentLane>(ref m_CurrentLaneType);
			NativeArray<PathOwner> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<PrefabRef> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<AircraftNavigationLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<AircraftNavigationLane>(ref m_NavigationLaneType);
			BufferAccessor<PathElement> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			bool isHelicopter = ((ArchetypeChunk)(ref chunk)).Has<Helicopter>(ref m_HelicopterType);
			if (nativeArray3.Length != 0)
			{
				NativeArray<Target> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
				NativeArray<Aircraft> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Aircraft>(ref m_AircraftType);
				NativeArray<Blocker> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Blocker>(ref m_BlockerType);
				NativeArray<Odometer> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Odometer>(ref m_OdometerType);
				bool flag = nativeArray11.Length != 0;
				for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
				{
					Entity entity = nativeArray[i];
					Transform transform = nativeArray2[i];
					Moving moving = nativeArray3[i];
					Target target = nativeArray8[i];
					Aircraft aircraft = nativeArray9[i];
					AircraftNavigation navigation = nativeArray4[i];
					AircraftCurrentLane currentLane = nativeArray5[i];
					Blocker blocker = nativeArray10[i];
					PathOwner pathOwner = nativeArray6[i];
					PrefabRef prefabRefData = nativeArray7[i];
					DynamicBuffer<AircraftNavigationLane> navigationLanes = bufferAccessor[i];
					DynamicBuffer<PathElement> pathElements = bufferAccessor2[i];
					AircraftData prefabAircraftData = m_PrefabAircraftData[prefabRefData.m_Prefab];
					ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRefData.m_Prefab];
					AircraftNavigationHelpers.CurrentLaneCache currentLaneCache = new AircraftNavigationHelpers.CurrentLaneCache(ref currentLane, m_PrefabRefData, m_MovingObjectSearchTree);
					int priority = VehicleUtils.GetPriority(prefabAircraftData);
					Odometer odometer = default(Odometer);
					if (flag)
					{
						odometer = nativeArray11[i];
					}
					UpdateNavigationLanes(priority, entity, isHelicopter, transform, target, aircraft, ref currentLane, ref blocker, ref pathOwner, navigationLanes, pathElements);
					UpdateNavigationTarget(priority, entity, isHelicopter, transform, moving, aircraft, prefabRefData, prefabAircraftData, objectGeometryData, ref navigation, ref currentLane, ref blocker, ref odometer, navigationLanes);
					ReserveNavigationLanes(priority, prefabAircraftData, aircraft, ref navigation, ref currentLane, navigationLanes);
					currentLaneCache.CheckChanges(entity, ref currentLane, m_LaneObjectBuffer, m_LaneObjects, transform, moving, navigation, objectGeometryData);
					nativeArray4[i] = navigation;
					nativeArray5[i] = currentLane;
					nativeArray6[i] = pathOwner;
					nativeArray10[i] = blocker;
					if (flag)
					{
						nativeArray11[i] = odometer;
					}
				}
			}
			else
			{
				for (int j = 0; j < ((ArchetypeChunk)(ref chunk)).Count; j++)
				{
					Entity entity2 = nativeArray[j];
					Transform transform2 = nativeArray2[j];
					AircraftNavigation navigation2 = nativeArray4[j];
					AircraftCurrentLane currentLane2 = nativeArray5[j];
					PathOwner pathOwnerData = nativeArray6[j];
					PrefabRef prefabRef = nativeArray7[j];
					DynamicBuffer<AircraftNavigationLane> navigationLanes2 = bufferAccessor[j];
					DynamicBuffer<PathElement> pathElements2 = bufferAccessor2[j];
					ObjectGeometryData objectGeometryData2 = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
					AircraftNavigationHelpers.CurrentLaneCache currentLaneCache2 = new AircraftNavigationHelpers.CurrentLaneCache(ref currentLane2, m_PrefabRefData, m_MovingObjectSearchTree);
					UpdateStopped(isHelicopter, transform2, ref currentLane2, ref pathOwnerData, navigationLanes2, pathElements2);
					currentLaneCache2.CheckChanges(entity2, ref currentLane2, m_LaneObjectBuffer, m_LaneObjects, transform2, default(Moving), navigation2, objectGeometryData2);
					nativeArray5[j] = currentLane2;
					nativeArray6[j] = pathOwnerData;
				}
			}
		}

		private void UpdateStopped(bool isHelicopter, Transform transform, ref AircraftCurrentLane currentLaneData, ref PathOwner pathOwnerData, DynamicBuffer<AircraftNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if (currentLaneData.m_Lane == Entity.Null || (currentLaneData.m_LaneFlags & AircraftLaneFlags.Obsolete) != 0)
			{
				TryFindCurrentLane(ref currentLaneData, transform, isHelicopter);
				navigationLanes.Clear();
				pathElements.Clear();
				pathOwnerData.m_ElementIndex = 0;
				pathOwnerData.m_State |= PathFlags.Obsolete;
			}
		}

		private void UpdateNavigationLanes(int priority, Entity entity, bool isHelicopter, Transform transform, Target target, Aircraft watercraft, ref AircraftCurrentLane currentLane, ref Blocker blocker, ref PathOwner pathOwner, DynamicBuffer<AircraftNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			int invalidPath = 10000000;
			if (currentLane.m_Lane == Entity.Null || (currentLane.m_LaneFlags & AircraftLaneFlags.Obsolete) != 0)
			{
				invalidPath = -1;
				TryFindCurrentLane(ref currentLane, transform, isHelicopter);
			}
			else if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete | PathFlags.Updated)) != 0 && (pathOwner.m_State & PathFlags.Append) == 0)
			{
				ClearNavigationLanes(ref currentLane, navigationLanes, invalidPath);
			}
			else if ((pathOwner.m_State & PathFlags.Updated) == 0)
			{
				FillNavigationPaths(priority, entity, isHelicopter, transform, target, watercraft, ref currentLane, ref blocker, ref pathOwner, navigationLanes, pathElements, ref invalidPath);
			}
			if (invalidPath != 10000000)
			{
				ClearNavigationLanes(ref currentLane, navigationLanes, invalidPath);
				pathElements.Clear();
				pathOwner.m_ElementIndex = 0;
				pathOwner.m_State |= PathFlags.Obsolete;
			}
		}

		private void ClearNavigationLanes(ref AircraftCurrentLane currentLane, DynamicBuffer<AircraftNavigationLane> navigationLanes, int invalidPath)
		{
			currentLane.m_CurvePosition.z = currentLane.m_CurvePosition.y;
			if (invalidPath > 0)
			{
				for (int i = 0; i < navigationLanes.Length; i++)
				{
					if ((navigationLanes[i].m_Flags & AircraftLaneFlags.Reserved) == 0)
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

		private void TryFindCurrentLane(ref AircraftCurrentLane currentLaneData, Transform transformData, bool isHelicopter)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
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
			currentLaneData.m_LaneFlags &= ~AircraftLaneFlags.Obsolete;
			currentLaneData.m_Lane = Entity.Null;
			float3 position = transformData.m_Position;
			float num = 100f;
			Bounds3 bounds = default(Bounds3);
			((Bounds3)(ref bounds))._002Ector(position - num, position + num);
			AircraftNavigationHelpers.FindLaneIterator findLaneIterator = new AircraftNavigationHelpers.FindLaneIterator
			{
				m_Bounds = bounds,
				m_Position = position,
				m_MinDistance = num,
				m_Result = currentLaneData,
				m_CarType = (isHelicopter ? RoadTypes.Helicopter : RoadTypes.Airplane),
				m_SubLanes = m_Lanes,
				m_CarLaneData = m_CarLaneData,
				m_ConnectionLaneData = m_ConnectionLaneData,
				m_CurveData = m_CurveData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabCarLaneData = m_PrefabCarLaneData
			};
			m_NetSearchTree.Iterate<AircraftNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			findLaneIterator.Iterate(ref m_AirwayData);
			currentLaneData = findLaneIterator.m_Result;
		}

		private void FillNavigationPaths(int priority, Entity entity, bool isHelicopter, Transform transform, Target target, Aircraft aircraft, ref AircraftCurrentLane currentLaneData, ref Blocker blocker, ref PathOwner pathOwner, DynamicBuffer<AircraftNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements, ref int invalidPath)
		{
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			if ((currentLaneData.m_LaneFlags & AircraftLaneFlags.EndOfPath) != 0)
			{
				return;
			}
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
						AircraftNavigationLane aircraftNavigationLane = default(AircraftNavigationLane);
						if (i > 0)
						{
							AircraftNavigationLane aircraftNavigationLane2 = navigationLanes[i - 1];
							if ((aircraftNavigationLane2.m_Flags & AircraftLaneFlags.Airway) != 0)
							{
								if (!GetTransformTarget(ref aircraftNavigationLane.m_Lane, target))
								{
									aircraftNavigationLane2.m_Flags |= AircraftLaneFlags.EndOfPath;
									navigationLanes[i - 1] = aircraftNavigationLane2;
									break;
								}
								aircraftNavigationLane.m_Flags |= AircraftLaneFlags.EndOfPath | AircraftLaneFlags.TransformTarget;
								if ((aircraft.m_Flags & AircraftFlags.StayMidAir) != 0)
								{
									aircraftNavigationLane.m_Flags |= AircraftLaneFlags.Airway;
								}
								navigationLanes.Add(aircraftNavigationLane);
							}
							else if ((aircraftNavigationLane2.m_Flags & AircraftLaneFlags.TransformTarget) != 0 || (aircraft.m_Flags & AircraftFlags.StayOnTaxiway) != 0 || !GetTransformTarget(ref aircraftNavigationLane.m_Lane, target))
							{
								aircraftNavigationLane2.m_Flags |= AircraftLaneFlags.EndOfPath;
								navigationLanes[i - 1] = aircraftNavigationLane2;
							}
							else
							{
								aircraftNavigationLane.m_Flags |= AircraftLaneFlags.EndOfPath | AircraftLaneFlags.TransformTarget;
								navigationLanes.Add(aircraftNavigationLane);
							}
						}
						else if ((currentLaneData.m_LaneFlags & AircraftLaneFlags.Airway) != 0)
						{
							if (!GetTransformTarget(ref aircraftNavigationLane.m_Lane, target))
							{
								currentLaneData.m_LaneFlags |= AircraftLaneFlags.EndOfPath;
								break;
							}
							aircraftNavigationLane.m_Flags |= AircraftLaneFlags.EndOfPath | AircraftLaneFlags.TransformTarget;
							if ((aircraft.m_Flags & AircraftFlags.StayMidAir) != 0)
							{
								aircraftNavigationLane.m_Flags |= AircraftLaneFlags.Airway;
							}
							navigationLanes.Add(aircraftNavigationLane);
						}
						else if ((currentLaneData.m_LaneFlags & AircraftLaneFlags.TransformTarget) != 0 || (aircraft.m_Flags & AircraftFlags.StayOnTaxiway) != 0 || !GetTransformTarget(ref aircraftNavigationLane.m_Lane, target))
						{
							currentLaneData.m_LaneFlags |= AircraftLaneFlags.EndOfPath;
						}
						else
						{
							aircraftNavigationLane.m_Flags |= AircraftLaneFlags.EndOfPath | AircraftLaneFlags.TransformTarget;
							navigationLanes.Add(aircraftNavigationLane);
						}
						break;
					}
					PathElement pathElement = pathElements[pathOwner.m_ElementIndex++];
					AircraftNavigationLane aircraftNavigationLane3 = new AircraftNavigationLane
					{
						m_Lane = pathElement.m_Target,
						m_CurvePosition = pathElement.m_TargetDelta
					};
					AircraftLaneFlags aircraftLaneFlags = ((i <= 0) ? currentLaneData.m_LaneFlags : navigationLanes[i - 1].m_Flags);
					if ((aircraftLaneFlags & AircraftLaneFlags.Airway) != 0 && (aircraft.m_Flags & AircraftFlags.StayMidAir) != 0)
					{
						aircraftNavigationLane3.m_Flags |= AircraftLaneFlags.Airway;
					}
					if (m_CarLaneData.HasComponent(aircraftNavigationLane3.m_Lane))
					{
						if ((m_CarLaneData[aircraftNavigationLane3.m_Lane].m_Flags & Game.Net.CarLaneFlags.Runway) != 0)
						{
							aircraftNavigationLane3.m_Flags |= AircraftLaneFlags.Runway;
						}
						navigationLanes.Add(aircraftNavigationLane3);
						continue;
					}
					if (m_ConnectionLaneData.HasComponent(aircraftNavigationLane3.m_Lane))
					{
						if ((m_ConnectionLaneData[aircraftNavigationLane3.m_Lane].m_Flags & ConnectionLaneFlags.Airway) != 0)
						{
							aircraftNavigationLane3.m_Flags |= AircraftLaneFlags.Airway;
							navigationLanes.Add(aircraftNavigationLane3);
							break;
						}
						aircraftNavigationLane3.m_Flags |= AircraftLaneFlags.Connection;
						navigationLanes.Add(aircraftNavigationLane3);
						continue;
					}
					if (m_TakeoffLocationData.HasComponent(aircraftNavigationLane3.m_Lane))
					{
						if (isHelicopter)
						{
							aircraftNavigationLane3.m_Flags |= AircraftLaneFlags.TransformTarget;
							if ((aircraft.m_Flags & AircraftFlags.StayMidAir) == 0 && m_SpawnLocationData.HasComponent(aircraftNavigationLane3.m_Lane))
							{
								aircraftNavigationLane3.m_Flags |= AircraftLaneFlags.ParkingSpace;
							}
							navigationLanes.Add(aircraftNavigationLane3);
						}
						continue;
					}
					if (!m_SpawnLocationData.HasComponent(aircraftNavigationLane3.m_Lane))
					{
						invalidPath = i;
						break;
					}
					if (pathOwner.m_ElementIndex >= pathElements.Length && (pathOwner.m_State & PathFlags.Pending) != 0)
					{
						pathOwner.m_ElementIndex--;
						break;
					}
					if ((aircraft.m_Flags & AircraftFlags.StayOnTaxiway) == 0 || pathElements.Length > pathOwner.m_ElementIndex)
					{
						aircraftNavigationLane3.m_Flags |= AircraftLaneFlags.TransformTarget;
						navigationLanes.Add(aircraftNavigationLane3);
					}
				}
				else
				{
					AircraftNavigationLane aircraftNavigationLane4 = navigationLanes[i];
					if (!m_PrefabRefData.HasComponent(aircraftNavigationLane4.m_Lane))
					{
						invalidPath = i;
						break;
					}
					if ((aircraftNavigationLane4.m_Flags & AircraftLaneFlags.EndOfPath) != 0)
					{
						break;
					}
				}
			}
		}

		private bool GetTransformTarget(ref Entity entity, Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			PropertyRenter propertyRenter = default(PropertyRenter);
			if (m_PropertyRenterData.TryGetComponent(target.m_Target, ref propertyRenter))
			{
				target.m_Target = propertyRenter.m_Property;
			}
			if (m_TransformData.HasComponent(target.m_Target))
			{
				entity = target.m_Target;
				return true;
			}
			if (m_PositionDataFromEntity.HasComponent(target.m_Target))
			{
				entity = target.m_Target;
				return true;
			}
			return false;
		}

		private void CheckBlocker(Aircraft aircraft, bool isHelicopter, ref AircraftCurrentLane currentLane, ref Blocker blocker, ref AircraftLaneSpeedIterator laneIterator)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			if (laneIterator.m_Blocker != blocker.m_Blocker)
			{
				currentLane.m_LaneFlags &= ~AircraftLaneFlags.IgnoreBlocker;
			}
			if (laneIterator.m_Blocker != Entity.Null && m_MovingDataFromEntity.HasComponent(laneIterator.m_Blocker) && m_AircraftData.HasComponent(laneIterator.m_Blocker) && (m_AircraftData[laneIterator.m_Blocker].m_Flags & ~aircraft.m_Flags & AircraftFlags.Blocking) != 0 && laneIterator.m_MaxSpeed < 1f)
			{
				currentLane.m_LaneFlags |= AircraftLaneFlags.IgnoreBlocker;
			}
			float num = math.select(0.91800004f, 3.06f, isHelicopter);
			blocker.m_Blocker = laneIterator.m_Blocker;
			blocker.m_Type = laneIterator.m_BlockerType;
			blocker.m_MaxSpeed = (byte)math.clamp(Mathf.RoundToInt(laneIterator.m_MaxSpeed * num), 0, 255);
		}

		private void UpdateNavigationTarget(int priority, Entity entity, bool isHelicopter, Transform transform, Moving moving, Aircraft aircraft, PrefabRef prefabRefData, AircraftData prefabAircraftData, ObjectGeometryData prefabObjectGeometryData, ref AircraftNavigation navigation, ref AircraftCurrentLane currentLane, ref Blocker blocker, ref Odometer odometer, DynamicBuffer<AircraftNavigationLane> navigationLanes)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dcb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_14a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_138a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0edb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebd: Unknown result type (might be due to invalid IL or missing references)
			//IL_102a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1031: Unknown result type (might be due to invalid IL or missing references)
			//IL_1036: Unknown result type (might be due to invalid IL or missing references)
			//IL_103b: Unknown result type (might be due to invalid IL or missing references)
			//IL_103f: Unknown result type (might be due to invalid IL or missing references)
			//IL_15a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_14cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_14d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_14d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_14e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_14f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_14f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1501: Unknown result type (might be due to invalid IL or missing references)
			//IL_1546: Unknown result type (might be due to invalid IL or missing references)
			//IL_154d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1556: Unknown result type (might be due to invalid IL or missing references)
			//IL_156d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_10df: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_109b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bed: Unknown result type (might be due to invalid IL or missing references)
			//IL_1475: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_13da: Unknown result type (might be due to invalid IL or missing references)
			//IL_13e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_13f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_13fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1402: Unknown result type (might be due to invalid IL or missing references)
			//IL_1407: Unknown result type (might be due to invalid IL or missing references)
			//IL_140c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1410: Unknown result type (might be due to invalid IL or missing references)
			//IL_141e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1001: Unknown result type (might be due to invalid IL or missing references)
			//IL_1330: Unknown result type (might be due to invalid IL or missing references)
			//IL_133f: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_131a: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_10b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0655: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0671: Unknown result type (might be due to invalid IL or missing references)
			//IL_0676: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_067f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f30: Unknown result type (might be due to invalid IL or missing references)
			//IL_11de: Unknown result type (might be due to invalid IL or missing references)
			//IL_1135: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_080d: Unknown result type (might be due to invalid IL or missing references)
			//IL_081a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0826: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_0770: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_077e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1187: Unknown result type (might be due to invalid IL or missing references)
			//IL_118e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1193: Unknown result type (might be due to invalid IL or missing references)
			//IL_1198: Unknown result type (might be due to invalid IL or missing references)
			//IL_119c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1149: Unknown result type (might be due to invalid IL or missing references)
			//IL_1159: Unknown result type (might be due to invalid IL or missing references)
			//IL_116a: Unknown result type (might be due to invalid IL or missing references)
			//IL_116f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0845: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0faa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fdb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0873: Unknown result type (might be due to invalid IL or missing references)
			//IL_087a: Unknown result type (might be due to invalid IL or missing references)
			//IL_087f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0884: Unknown result type (might be due to invalid IL or missing references)
			//IL_0888: Unknown result type (might be due to invalid IL or missing references)
			//IL_129f: Unknown result type (might be due to invalid IL or missing references)
			//IL_12a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1254: Unknown result type (might be due to invalid IL or missing references)
			//IL_125b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1260: Unknown result type (might be due to invalid IL or missing references)
			//IL_1265: Unknown result type (might be due to invalid IL or missing references)
			//IL_1269: Unknown result type (might be due to invalid IL or missing references)
			//IL_091b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0925: Unknown result type (might be due to invalid IL or missing references)
			//IL_092c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0936: Unknown result type (might be due to invalid IL or missing references)
			//IL_093b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0942: Unknown result type (might be due to invalid IL or missing references)
			//IL_094e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0952: Unknown result type (might be due to invalid IL or missing references)
			//IL_0957: Unknown result type (might be due to invalid IL or missing references)
			//IL_095c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0960: Unknown result type (might be due to invalid IL or missing references)
			//IL_0973: Unknown result type (might be due to invalid IL or missing references)
			//IL_097a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0982: Unknown result type (might be due to invalid IL or missing references)
			//IL_0987: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0add: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af1: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			float num2 = math.length(moving.m_Velocity);
			float3 position = transform.m_Position;
			if ((currentLane.m_LaneFlags & AircraftLaneFlags.Flying) != 0)
			{
				if (isHelicopter)
				{
					HelicopterData helicopterData = m_PrefabHelicopterData[prefabRefData.m_Prefab];
					Bounds1 val = default(Bounds1);
					if ((currentLane.m_LaneFlags & (AircraftLaneFlags.Connection | AircraftLaneFlags.ResetSpeed)) != 0)
					{
						((Bounds1)(ref val))._002Ector(0f, helicopterData.m_FlyingMaxSpeed);
					}
					else
					{
						val = VehicleUtils.CalculateSpeedRange(helicopterData, num2, num);
					}
					if ((currentLane.m_LaneFlags & AircraftLaneFlags.SkipLane) != 0)
					{
						navigation.m_TargetPosition = transform.m_Position;
					}
					float3 val2 = navigation.m_TargetPosition - transform.m_Position;
					float num3 = math.length(((float3)(ref val2)).xz);
					float num4 = math.length(val2);
					float num5 = math.saturate((math.dot(moving.m_Velocity, val2) + 1f) / (num2 * num4 + 1f));
					num5 = 1f - math.sqrt(1f - num5 * num5);
					float num6 = VehicleUtils.GetMaxBrakingSpeed(distance: math.lerp(num3, num4, num5), prefabHelicopterData: helicopterData, timeStep: num + 0.5f);
					navigation.m_MaxSpeed = MathUtils.Clamp(num6, val);
					blocker.m_Blocker = Entity.Null;
					blocker.m_Type = BlockerType.None;
					blocker.m_MaxSpeed = byte.MaxValue;
					float brakingDistance = VehicleUtils.GetBrakingDistance(helicopterData, helicopterData.m_FlyingMaxSpeed, num);
					brakingDistance = math.max(brakingDistance, 750f);
					brakingDistance += navigation.m_MaxSpeed * num + 1f;
					currentLane.m_Duration += num;
					currentLane.m_Distance += num2 * num;
					odometer.m_Distance += num2 * num;
					if (num3 < brakingDistance)
					{
						while (true)
						{
							bool flag = (currentLane.m_LaneFlags & AircraftLaneFlags.Landing) != 0;
							if (flag && navigationLanes.Length != 0 && (navigationLanes[0].m_Flags & AircraftLaneFlags.Airway) != 0)
							{
								currentLane.m_LaneFlags &= ~AircraftLaneFlags.Landing;
								flag = false;
							}
							if ((currentLane.m_LaneFlags & AircraftLaneFlags.SkipLane) == 0)
							{
								if ((currentLane.m_LaneFlags & AircraftLaneFlags.TransformTarget) != 0)
								{
									navigation.m_TargetDirection = default(float3);
									bool num7 = MoveTarget(position, ref navigation.m_TargetPosition, brakingDistance, currentLane.m_Lane);
									if (!flag)
									{
										UpdateTargetHeight(ref navigation.m_TargetPosition, currentLane.m_Lane, helicopterData);
									}
									if (num7)
									{
										break;
									}
								}
								else
								{
									navigation.m_TargetDirection = default(float3);
									Curve curve = m_CurveData[currentLane.m_Lane];
									bool num8 = MoveTarget(position, ref navigation.m_TargetPosition, brakingDistance, curve.m_Bezier, ref currentLane.m_CurvePosition);
									if (!flag)
									{
										UpdateTargetHeight(ref navigation.m_TargetPosition, currentLane.m_Lane, helicopterData);
									}
									if (num8)
									{
										break;
									}
								}
							}
							if (flag)
							{
								val2 = navigation.m_TargetPosition - transform.m_Position;
								num4 = math.length(val2);
								if (!(num4 >= 1f) && !(num2 >= 0.1f))
								{
									currentLane.m_LaneFlags &= ~(AircraftLaneFlags.Flying | AircraftLaneFlags.Landing);
									if (navigationLanes.Length == 0)
									{
										currentLane.m_LaneFlags |= AircraftLaneFlags.EndReached;
									}
								}
								break;
							}
							if (navigationLanes.Length == 0)
							{
								val2 = navigation.m_TargetPosition - transform.m_Position;
								num4 = math.length(val2);
								if (num4 < 1f && num2 < 0.1f)
								{
									currentLane.m_LaneFlags |= AircraftLaneFlags.EndReached;
								}
								break;
							}
							AircraftNavigationLane aircraftNavigationLane = navigationLanes[0];
							if (!m_PrefabRefData.HasComponent(aircraftNavigationLane.m_Lane))
							{
								break;
							}
							aircraftNavigationLane.m_Flags |= AircraftLaneFlags.Flying;
							if ((aircraftNavigationLane.m_Flags & AircraftLaneFlags.Airway) == 0)
							{
								aircraftNavigationLane.m_Flags |= AircraftLaneFlags.Landing;
							}
							ApplySideEffects(ref currentLane, prefabRefData, helicopterData);
							currentLane.m_Lane = aircraftNavigationLane.m_Lane;
							currentLane.m_CurvePosition = ((float2)(ref aircraftNavigationLane.m_CurvePosition)).xxy;
							currentLane.m_LaneFlags = aircraftNavigationLane.m_Flags;
							navigationLanes.RemoveAt(0);
						}
					}
				}
				else
				{
					AirplaneData airplaneData = m_PrefabAirplaneData[prefabRefData.m_Prefab];
					Bounds1 val3 = default(Bounds1);
					if ((currentLane.m_LaneFlags & (AircraftLaneFlags.Connection | AircraftLaneFlags.ResetSpeed)) != 0)
					{
						((Bounds1)(ref val3))._002Ector(airplaneData.m_FlyingSpeed.x, airplaneData.m_FlyingSpeed.y);
					}
					else
					{
						val3 = VehicleUtils.CalculateSpeedRange(airplaneData, num2, num);
					}
					float3 val4 = navigation.m_TargetPosition - transform.m_Position;
					float num9 = math.length(((float3)(ref val4)).xz);
					float num10 = math.length(val4);
					float num11 = math.saturate((math.dot(moving.m_Velocity, val4) + 1f) / (num2 * num10 + 1f));
					num11 = 1f - math.sqrt(1f - num11 * num11);
					float num12 = math.lerp(num9, num10, num11);
					float num13;
					if ((currentLane.m_LaneFlags & AircraftLaneFlags.Landing) != 0)
					{
						num13 = prefabAircraftData.m_GroundMaxSpeed;
					}
					else if ((currentLane.m_LaneFlags & (AircraftLaneFlags.Approaching | AircraftLaneFlags.TakingOff)) != 0)
					{
						num13 = airplaneData.m_FlyingSpeed.y;
						num12 += 1500f;
					}
					else
					{
						num13 = VehicleUtils.GetBrakingDistance(airplaneData, airplaneData.m_FlyingSpeed.y, num);
						num13 -= VehicleUtils.GetBrakingDistance(airplaneData, prefabAircraftData.m_GroundMaxSpeed, num) + 1500f;
						num13 = math.max(num13, 1500f);
						num12 += 1500f;
					}
					if ((currentLane.m_LaneFlags & AircraftLaneFlags.Connection) != 0)
					{
						airplaneData.m_FlyingBraking = 277.77777f;
						navigation.m_MaxSpeed = VehicleUtils.GetMaxBrakingSpeed(airplaneData, num12, 0f, num + 0.5f);
					}
					else
					{
						float maxBrakingSpeed = VehicleUtils.GetMaxBrakingSpeed(airplaneData, num12, prefabAircraftData.m_GroundMaxSpeed, num + 0.5f);
						navigation.m_MaxSpeed = MathUtils.Clamp(maxBrakingSpeed, val3);
					}
					blocker.m_Blocker = Entity.Null;
					blocker.m_Type = BlockerType.None;
					blocker.m_MaxSpeed = byte.MaxValue;
					if (currentLane.m_Lane == Entity.Null)
					{
						return;
					}
					num13 += navigation.m_MaxSpeed * num + 1f;
					currentLane.m_Duration += num;
					currentLane.m_Distance += num2 * num;
					odometer.m_Distance += num2 * num;
					if (num9 < num13)
					{
						while (true)
						{
							bool flag2 = (currentLane.m_LaneFlags & AircraftLaneFlags.Landing) != 0;
							if ((currentLane.m_LaneFlags & AircraftLaneFlags.Approaching) != 0)
							{
								if (flag2)
								{
									float3 val5 = MathUtils.Position(m_CurveData[currentLane.m_Lane].m_Bezier, currentLane.m_CurvePosition.x);
									val4 = val5 - transform.m_Position;
									num9 = math.length(((float3)(ref val4)).xz);
									if (num9 >= num13)
									{
										num9 -= num13;
										((float3)(ref navigation.m_TargetPosition)).xz = ((float3)(ref val5)).xz - ((float3)(ref navigation.m_TargetDirection)).xz * num9;
										navigation.m_TargetPosition.y = math.min(navigation.m_TargetPosition.y, val5.y + num9 * math.tan(airplaneData.m_ClimbAngle));
										break;
									}
									currentLane.m_LaneFlags &= ~AircraftLaneFlags.Approaching;
									continue;
								}
								currentLane.m_LaneFlags |= AircraftLaneFlags.Landing;
								num13 = prefabAircraftData.m_GroundMaxSpeed;
								num13 += navigation.m_MaxSpeed * num + 1f;
								flag2 = true;
							}
							else if ((currentLane.m_LaneFlags & AircraftLaneFlags.TakingOff) != 0)
							{
								if ((currentLane.m_LaneFlags & AircraftLaneFlags.Airway) != 0)
								{
									val4 = navigation.m_TargetPosition - transform.m_Position;
									num9 = math.length(((float3)(ref val4)).xz);
									if (num9 >= num13)
									{
										break;
									}
									currentLane.m_LaneFlags &= ~AircraftLaneFlags.TakingOff;
									continue;
								}
							}
							else if ((currentLane.m_LaneFlags & AircraftLaneFlags.TransformTarget) != 0)
							{
								navigation.m_TargetDirection = default(float3);
								bool num14 = MoveTarget(position, ref navigation.m_TargetPosition, num13, currentLane.m_Lane);
								if (!flag2)
								{
									UpdateTargetHeight(ref navigation.m_TargetPosition, currentLane.m_Lane, airplaneData);
								}
								if (num14)
								{
									break;
								}
							}
							else
							{
								navigation.m_TargetDirection = default(float3);
								Curve curve2 = m_CurveData[currentLane.m_Lane];
								bool num15 = MoveTarget(position, ref navigation.m_TargetPosition, num13, curve2.m_Bezier, ref currentLane.m_CurvePosition);
								if (!flag2)
								{
									UpdateTargetHeight(ref navigation.m_TargetPosition, currentLane.m_Lane, airplaneData);
								}
								if (num15)
								{
									break;
								}
							}
							if (flag2)
							{
								if (navigationLanes.Length == 0)
								{
									break;
								}
							}
							else if (navigationLanes.Length == 0)
							{
								val4 = navigation.m_TargetPosition - transform.m_Position;
								num9 = math.length(((float3)(ref val4)).xz);
								if (num9 < num2 * 2f)
								{
									currentLane.m_LaneFlags |= AircraftLaneFlags.EndReached;
								}
								break;
							}
							AircraftNavigationLane aircraftNavigationLane2 = navigationLanes[0];
							aircraftNavigationLane2.m_Flags |= AircraftLaneFlags.Flying;
							if ((currentLane.m_LaneFlags & AircraftLaneFlags.TakingOff) != 0)
							{
								if ((currentLane.m_LaneFlags & AircraftLaneFlags.Runway) != 0)
								{
									aircraftNavigationLane2.m_Flags |= AircraftLaneFlags.TakingOff;
									if ((aircraftNavigationLane2.m_Flags & AircraftLaneFlags.Airway) != 0)
									{
										float3 targetPosition = MathUtils.Position(m_CurveData[aircraftNavigationLane2.m_Lane].m_Bezier, aircraftNavigationLane2.m_CurvePosition.x);
										UpdateTargetHeight(ref targetPosition, aircraftNavigationLane2.m_Lane, airplaneData);
										val4 = targetPosition - navigation.m_TargetPosition;
										num9 = math.length(((float3)(ref val4)).xz);
										float2 val6 = math.normalizesafe(((float3)(ref moving.m_Velocity)).xz, default(float2));
										navigation.m_TargetDirection.y = 0f;
										((float3)(ref navigation.m_TargetDirection)).xz = val6;
										((float3)(ref navigation.m_TargetPosition)).xz = ((float3)(ref navigation.m_TargetPosition)).xz + val6 * num9;
										navigation.m_TargetPosition.y = math.min(targetPosition.y, navigation.m_TargetPosition.y + num9 * math.tan(airplaneData.m_ClimbAngle));
									}
								}
							}
							else if ((aircraftNavigationLane2.m_Flags & AircraftLaneFlags.Airway) == 0)
							{
								if ((aircraftNavigationLane2.m_Flags & AircraftLaneFlags.Connection) != 0)
								{
									aircraftNavigationLane2.m_Flags |= AircraftLaneFlags.Landing;
								}
								else
								{
									if (!flag2)
									{
										currentLane.m_LaneFlags |= AircraftLaneFlags.Approaching;
										Curve curve3 = m_CurveData[aircraftNavigationLane2.m_Lane];
										float3 val7 = MathUtils.Position(curve3.m_Bezier, aircraftNavigationLane2.m_CurvePosition.x);
										float3 val8 = MathUtils.Position(curve3.m_Bezier, aircraftNavigationLane2.m_CurvePosition.y);
										val4 = val7 - navigation.m_TargetPosition;
										float2 val9 = ((float3)(ref val8)).xz - ((float3)(ref val7)).xz;
										if (!MathUtils.TryNormalize(ref val9))
										{
											val9 = math.normalizesafe(((float3)(ref val4)).xz, default(float2));
										}
										num9 = math.length(((float3)(ref val4)).xz);
										navigation.m_TargetDirection.y = 0f;
										((float3)(ref navigation.m_TargetDirection)).xz = val9;
										((float3)(ref navigation.m_TargetPosition)).xz = ((float3)(ref val7)).xz - val9 * num9;
										navigation.m_TargetPosition.y = math.min(navigation.m_TargetPosition.y, val7.y + num9 * math.tan(airplaneData.m_ClimbAngle));
										break;
									}
									aircraftNavigationLane2.m_Flags |= currentLane.m_LaneFlags & (AircraftLaneFlags.Approaching | AircraftLaneFlags.Landing);
								}
							}
							ApplySideEffects(ref currentLane, prefabRefData, airplaneData);
							currentLane.m_Lane = aircraftNavigationLane2.m_Lane;
							currentLane.m_CurvePosition = ((float2)(ref aircraftNavigationLane2.m_CurvePosition)).xxy;
							currentLane.m_LaneFlags = aircraftNavigationLane2.m_Flags;
							navigationLanes.RemoveAt(0);
						}
					}
					if ((currentLane.m_LaneFlags & (AircraftLaneFlags.Approaching | AircraftLaneFlags.Landing)) == AircraftLaneFlags.Landing)
					{
						val4 = navigation.m_TargetPosition - transform.m_Position;
						if (transform.m_Position.y < navigation.m_TargetPosition.y + 1f)
						{
							currentLane.m_LaneFlags &= ~AircraftLaneFlags.Flying;
						}
					}
				}
			}
			else
			{
				if ((currentLane.m_LaneFlags & AircraftLaneFlags.Connection) != 0)
				{
					prefabAircraftData.m_GroundMaxSpeed = 277.77777f;
					prefabAircraftData.m_GroundAcceleration = 277.77777f;
					prefabAircraftData.m_GroundBraking = 277.77777f;
				}
				if (m_CurveData.HasComponent(currentLane.m_Lane))
				{
					Curve curve4 = m_CurveData[currentLane.m_Lane];
					PrefabRef prefabRef = m_PrefabRefData[currentLane.m_Lane];
					NetLaneData netLaneData = m_PrefabNetLaneData[prefabRef.m_Prefab];
					float3 val10 = MathUtils.Tangent(curve4.m_Bezier, currentLane.m_CurvePosition.x);
					float2 xz = ((float3)(ref val10)).xz;
					if (MathUtils.TryNormalize(ref xz))
					{
						((float3)(ref position)).xz = ((float3)(ref position)).xz - MathUtils.Right(xz) * ((netLaneData.m_Width - prefabObjectGeometryData.m_Size.x) * currentLane.m_LanePosition * 0.5f);
					}
				}
				Bounds1 val11 = default(Bounds1);
				if ((currentLane.m_LaneFlags & (AircraftLaneFlags.Connection | AircraftLaneFlags.ResetSpeed)) != 0)
				{
					((Bounds1)(ref val11))._002Ector(0f, prefabAircraftData.m_GroundMaxSpeed);
				}
				else
				{
					val11 = VehicleUtils.CalculateSpeedRange(prefabAircraftData, num2, num);
				}
				AircraftLaneSpeedIterator laneIterator = new AircraftLaneSpeedIterator
				{
					m_TransformData = m_TransformData,
					m_MovingData = m_MovingDataFromEntity,
					m_AircraftData = m_AircraftData,
					m_LaneReservationData = m_LaneReservationData,
					m_CurveData = m_CurveData,
					m_CarLaneData = m_CarLaneData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
					m_PrefabAircraftData = m_PrefabAircraftData,
					m_LaneOverlapData = m_LaneOverlaps,
					m_LaneObjectData = m_LaneObjects,
					m_Entity = entity,
					m_Ignore = (((currentLane.m_LaneFlags & AircraftLaneFlags.IgnoreBlocker) != 0) ? blocker.m_Blocker : Entity.Null),
					m_Priority = priority,
					m_TimeStep = num,
					m_SafeTimeStep = num + 0.5f,
					m_PrefabAircraft = prefabAircraftData,
					m_PrefabObjectGeometry = prefabObjectGeometryData,
					m_SpeedRange = val11,
					m_MaxSpeed = val11.max,
					m_CanChangeLane = 1f,
					m_CurrentPosition = position
				};
				if ((currentLane.m_LaneFlags & AircraftLaneFlags.TransformTarget) != 0)
				{
					laneIterator.IterateTarget(navigation.m_TargetPosition);
					navigation.m_MaxSpeed = laneIterator.m_MaxSpeed;
				}
				else
				{
					if (currentLane.m_Lane == Entity.Null)
					{
						navigation.m_MaxSpeed = math.max(0f, num2 - prefabAircraftData.m_GroundBraking * num);
						blocker.m_Blocker = Entity.Null;
						blocker.m_Type = BlockerType.None;
						blocker.m_MaxSpeed = byte.MaxValue;
						return;
					}
					if (!laneIterator.IterateFirstLane(currentLane.m_Lane, currentLane.m_CurvePosition))
					{
						int num16 = 0;
						while (true)
						{
							if (num16 < navigationLanes.Length)
							{
								AircraftNavigationLane aircraftNavigationLane3 = navigationLanes[num16];
								if ((aircraftNavigationLane3.m_Flags & AircraftLaneFlags.TransformTarget) == 0)
								{
									if ((aircraftNavigationLane3.m_Flags & AircraftLaneFlags.Connection) != 0)
									{
										laneIterator.m_PrefabAircraft.m_GroundMaxSpeed = 277.77777f;
										laneIterator.m_PrefabAircraft.m_GroundAcceleration = 277.77777f;
										laneIterator.m_PrefabAircraft.m_GroundBraking = 277.77777f;
										laneIterator.m_SpeedRange = new Bounds1(0f, 277.77777f);
									}
									else if ((currentLane.m_LaneFlags & AircraftLaneFlags.Connection) != 0)
									{
										goto IL_0ffd;
									}
									bool flag3 = aircraftNavigationLane3.m_Lane == currentLane.m_Lane;
									float minOffset = math.select(-1f, currentLane.m_CurvePosition.y, flag3);
									if (laneIterator.IterateNextLane(aircraftNavigationLane3.m_Lane, aircraftNavigationLane3.m_CurvePosition, minOffset))
									{
										break;
									}
									num16++;
									continue;
								}
								VehicleUtils.CalculateTransformPosition(ref laneIterator.m_CurrentPosition, aircraftNavigationLane3.m_Lane, m_TransformData, m_PositionDataFromEntity, m_PrefabRefData, m_PrefabBuildingData);
							}
							goto IL_0ffd;
							IL_0ffd:
							laneIterator.IterateTarget(laneIterator.m_CurrentPosition);
							break;
						}
					}
					navigation.m_MaxSpeed = laneIterator.m_MaxSpeed;
					CheckBlocker(aircraft, isHelicopter, ref currentLane, ref blocker, ref laneIterator);
				}
				float3 val12 = navigation.m_TargetPosition - transform.m_Position;
				float num17 = math.length(((float3)(ref val12)).xz);
				float num18 = navigation.m_MaxSpeed * num + 1f;
				currentLane.m_Duration += num;
				currentLane.m_Distance += num2 * num;
				odometer.m_Distance += num2 * num;
				if (num17 < num18)
				{
					while (true)
					{
						if ((currentLane.m_LaneFlags & AircraftLaneFlags.TransformTarget) != 0)
						{
							navigation.m_TargetDirection = default(float3);
							if ((currentLane.m_LaneFlags & AircraftLaneFlags.EndReached) == 0 && MoveTarget(position, ref navigation.m_TargetPosition, num18, currentLane.m_Lane))
							{
								break;
							}
						}
						else
						{
							navigation.m_TargetDirection = default(float3);
							Curve curve5 = m_CurveData[currentLane.m_Lane];
							if (MoveTarget(position, ref navigation.m_TargetPosition, num18, curve5.m_Bezier, ref currentLane.m_CurvePosition))
							{
								ApplyLanePosition(ref navigation.m_TargetPosition, ref currentLane, prefabObjectGeometryData);
								break;
							}
						}
						if (navigationLanes.Length == 0)
						{
							if (m_CurveData.HasComponent(currentLane.m_Lane))
							{
								navigation.m_TargetDirection = MathUtils.Tangent(m_CurveData[currentLane.m_Lane].m_Bezier, currentLane.m_CurvePosition.z);
								ApplyLanePosition(ref navigation.m_TargetPosition, ref currentLane, prefabObjectGeometryData);
							}
							val12 = navigation.m_TargetPosition - transform.m_Position;
							if (math.length(((float3)(ref val12)).xz) < 1f && num2 < 0.1f)
							{
								currentLane.m_LaneFlags |= AircraftLaneFlags.EndReached;
							}
							break;
						}
						AircraftNavigationLane aircraftNavigationLane4 = navigationLanes[0];
						if (!m_PrefabRefData.HasComponent(aircraftNavigationLane4.m_Lane))
						{
							break;
						}
						if ((currentLane.m_LaneFlags & AircraftLaneFlags.Landing) != 0)
						{
							if ((aircraftNavigationLane4.m_Flags & AircraftLaneFlags.Runway) != 0)
							{
								aircraftNavigationLane4.m_Flags |= AircraftLaneFlags.Landing;
							}
						}
						else if ((aircraftNavigationLane4.m_Flags & (AircraftLaneFlags.Runway | AircraftLaneFlags.Airway)) != 0)
						{
							aircraftNavigationLane4.m_Flags |= AircraftLaneFlags.TakingOff;
						}
						if ((currentLane.m_LaneFlags & AircraftLaneFlags.Connection) != 0 && (aircraftNavigationLane4.m_Flags & AircraftLaneFlags.Connection) == 0)
						{
							val12 = navigation.m_TargetPosition - transform.m_Position;
							if (math.length(((float3)(ref val12)).xz) >= 1f || num2 > 3f)
							{
								break;
							}
							aircraftNavigationLane4.m_Flags |= AircraftLaneFlags.ResetSpeed;
						}
						ApplySideEffects(ref currentLane, prefabRefData, prefabAircraftData);
						currentLane.m_Lane = aircraftNavigationLane4.m_Lane;
						currentLane.m_CurvePosition = ((float2)(ref aircraftNavigationLane4.m_CurvePosition)).xxy;
						currentLane.m_LaneFlags = aircraftNavigationLane4.m_Flags;
						navigationLanes.RemoveAt(0);
					}
				}
				if ((currentLane.m_LaneFlags & AircraftLaneFlags.TakingOff) != 0)
				{
					if (isHelicopter)
					{
						HelicopterData helicopterData2 = m_PrefabHelicopterData[prefabRefData.m_Prefab];
						currentLane.m_LaneFlags |= AircraftLaneFlags.Flying;
						UpdateTargetHeight(ref navigation.m_TargetPosition, currentLane.m_Lane, helicopterData2);
					}
					else if (num2 >= m_PrefabAirplaneData[prefabRefData.m_Prefab].m_FlyingSpeed.x || (currentLane.m_LaneFlags & AircraftLaneFlags.Airway) != 0)
					{
						currentLane.m_LaneFlags |= AircraftLaneFlags.Flying;
					}
				}
			}
			if ((currentLane.m_LaneFlags & AircraftLaneFlags.Flying) != 0)
			{
				if (isHelicopter)
				{
					HelicopterData helicopterData3 = m_PrefabHelicopterData[prefabRefData.m_Prefab];
					float num19 = 0f;
					switch (helicopterData3.m_HelicopterType)
					{
					case HelicopterType.Helicopter:
						num19 = 100f;
						break;
					case HelicopterType.Rocket:
						num19 = 10000f;
						break;
					}
					if ((currentLane.m_LaneFlags & AircraftLaneFlags.Landing) != 0)
					{
						float3 targetPosition2 = navigation.m_TargetPosition;
						UpdateTargetHeight(ref targetPosition2, currentLane.m_Lane, helicopterData3);
						GetCollisionHeightTarget(entity, transform, ref navigation, ref blocker, prefabObjectGeometryData, targetPosition2);
						float3 val13 = targetPosition2 - transform.m_Position;
						float num20 = math.length(((float3)(ref val13)).xz);
						navigation.m_MinClimbAngle = math.atan(val13.y * 4f / num19);
						navigation.m_MinClimbAngle = math.min(navigation.m_MinClimbAngle, math.asin(math.saturate(num20 * 2f / num19 - 1f)));
					}
					else
					{
						float num21 = navigation.m_TargetPosition.y - transform.m_Position.y;
						navigation.m_MinClimbAngle = math.atan(num21 * 4f / num19) * 1.15f;
					}
				}
				else
				{
					AirplaneData airplaneData2 = m_PrefabAirplaneData[prefabRefData.m_Prefab];
					if ((currentLane.m_LaneFlags & (AircraftLaneFlags.Approaching | AircraftLaneFlags.Landing)) == (AircraftLaneFlags.Approaching | AircraftLaneFlags.Landing))
					{
						float3 targetPosition3 = navigation.m_TargetPosition;
						UpdateTargetHeight(ref targetPosition3, currentLane.m_Lane, airplaneData2);
						float3 val14 = navigation.m_TargetPosition - transform.m_Position;
						float num22 = math.max(0f, math.length(((float3)(ref val14)).xz) - prefabAircraftData.m_GroundMaxSpeed - navigation.m_MaxSpeed * num - 1f);
						val14.y += num22 * math.tan(airplaneData2.m_ClimbAngle);
						val14.y = math.min(val14.y, targetPosition3.y - transform.m_Position.y);
						navigation.m_MinClimbAngle = math.atan(val14.y * 0.02f) * airplaneData2.m_ClimbAngle / ((float)Math.PI / 2f);
					}
					else
					{
						float num23 = navigation.m_TargetPosition.y - transform.m_Position.y;
						navigation.m_MinClimbAngle = math.atan(num23 * 0.02f) * airplaneData2.m_ClimbAngle / ((float)Math.PI / 2f);
					}
				}
			}
			else
			{
				navigation.m_MinClimbAngle = -(float)Math.PI / 2f;
			}
		}

		private void GetCollisionHeightTarget(Entity entity, Transform transform, ref AircraftNavigation navigation, ref Blocker blocker, ObjectGeometryData prefabObjectGeometryData, float3 targetPos)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			AircraftNavigationHelpers.AircraftCollisionIterator aircraftCollisionIterator = new AircraftNavigationHelpers.AircraftCollisionIterator
			{
				m_Ignore = entity,
				m_Line = new Segment(transform.m_Position, navigation.m_TargetPosition),
				m_AircraftData = m_AircraftData,
				m_TransformData = m_TransformData,
				m_ClosestT = 2f
			};
			m_MovingObjectSearchTree.Iterate<AircraftNavigationHelpers.AircraftCollisionIterator>(ref aircraftCollisionIterator, 0);
			if (aircraftCollisionIterator.m_Result != Entity.Null)
			{
				Transform transform2 = m_TransformData[aircraftCollisionIterator.m_Result];
				PrefabRef prefabRef = m_PrefabRefData[aircraftCollisionIterator.m_Result];
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				blocker.m_Blocker = aircraftCollisionIterator.m_Result;
				blocker.m_Type = BlockerType.Continuing;
				blocker.m_MaxSpeed = (byte)math.clamp(Mathf.RoundToInt(navigation.m_MaxSpeed * 3.06f), 0, 255);
				float num = transform2.m_Position.y + objectGeometryData.m_Bounds.max.y - prefabObjectGeometryData.m_Bounds.min.y + 50f;
				navigation.m_TargetPosition.y = math.clamp(num, navigation.m_TargetPosition.y, targetPos.y);
			}
		}

		private void ApplyLanePosition(ref float3 targetPosition, ref AircraftCurrentLane currentLaneData, ObjectGeometryData prefabObjectGeometryData)
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

		private void ApplySideEffects(ref AircraftCurrentLane currentLaneData, PrefabRef prefabRefData, AircraftData prefabAircraftData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			if (m_CarLaneData.HasComponent(currentLaneData.m_Lane))
			{
				Game.Net.CarLane carLaneData = m_CarLaneData[currentLaneData.m_Lane];
				float maxDriveSpeed = VehicleUtils.GetMaxDriveSpeed(prefabAircraftData, carLaneData);
				float num = math.select(currentLaneData.m_Distance / currentLaneData.m_Duration, maxDriveSpeed, currentLaneData.m_Duration == 0f);
				float relativeSpeed = num / maxDriveSpeed;
				float3 sideEffects = default(float3);
				if (m_PrefabSideEffectData.HasComponent(prefabRefData.m_Prefab))
				{
					VehicleSideEffectData vehicleSideEffectData = m_PrefabSideEffectData[prefabRefData.m_Prefab];
					float num2 = num / prefabAircraftData.m_GroundMaxSpeed;
					num2 = math.saturate(num2 * num2);
					sideEffects = math.lerp(vehicleSideEffectData.m_Min, vehicleSideEffectData.m_Max, num2);
					sideEffects *= new float3(currentLaneData.m_Distance, currentLaneData.m_Duration, currentLaneData.m_Duration);
				}
				m_LaneEffects.Enqueue(new AircraftNavigationHelpers.LaneEffects(currentLaneData.m_Lane, sideEffects, relativeSpeed));
			}
			currentLaneData.m_Duration = 0f;
			currentLaneData.m_Distance = 0f;
		}

		private void ApplySideEffects(ref AircraftCurrentLane currentLaneData, PrefabRef prefabRefData, HelicopterData prefabHelicopterData)
		{
			currentLaneData.m_Duration = 0f;
			currentLaneData.m_Distance = 0f;
		}

		private void ApplySideEffects(ref AircraftCurrentLane currentLaneData, PrefabRef prefabRefData, AirplaneData prefabAirplaneData)
		{
			currentLaneData.m_Duration = 0f;
			currentLaneData.m_Distance = 0f;
		}

		private void ReserveNavigationLanes(int priority, AircraftData prefabAircraftData, Aircraft watercraftData, ref AircraftNavigation navigationData, ref AircraftCurrentLane currentLaneData, DynamicBuffer<AircraftNavigationLane> navigationLanes)
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
			if (m_CarLaneData.HasComponent(currentLaneData.m_Lane))
			{
				Curve curve = m_CurveData[currentLaneData.m_Lane];
				float num = math.max(0f, VehicleUtils.GetBrakingDistance(prefabAircraftData, navigationData.m_MaxSpeed, timeStep) - 0.01f);
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
					AircraftNavigationLane aircraftNavigationLane = navigationLanes[num2];
					if (m_CarLaneData.HasComponent(aircraftNavigationLane.m_Lane))
					{
						curve = m_CurveData[aircraftNavigationLane.m_Lane];
						float offset = math.min(aircraftNavigationLane.m_CurvePosition.y, aircraftNavigationLane.m_CurvePosition.x + num / math.max(1E-06f, curve.m_Length));
						if (m_LaneReservationData.HasComponent(aircraftNavigationLane.m_Lane))
						{
							m_LaneReservations.Enqueue(new AircraftNavigationHelpers.LaneReservation(aircraftNavigationLane.m_Lane, offset, priority));
						}
						num -= curve.m_Length * math.abs(aircraftNavigationLane.m_CurvePosition.y - aircraftNavigationLane.m_CurvePosition.x);
						aircraftNavigationLane.m_Flags |= AircraftLaneFlags.Reserved;
						navigationLanes[num2++] = aircraftNavigationLane;
						continue;
					}
					break;
				}
			}
			else
			{
				currentLaneData.m_CurvePosition.y = currentLaneData.m_CurvePosition.x;
			}
		}

		private void UpdateTargetHeight(ref float3 targetPosition, Entity target, HelicopterData helicopterData)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			switch (helicopterData.m_HelicopterType)
			{
			case HelicopterType.Helicopter:
				targetPosition.y = 100f;
				break;
			case HelicopterType.Rocket:
				targetPosition.y = 10000f;
				break;
			}
			targetPosition.y += WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, targetPosition);
		}

		private void UpdateTargetHeight(ref float3 targetPosition, Entity target, AirplaneData airplaneData)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			targetPosition.y = 1000f;
			targetPosition.y += WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, targetPosition);
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
			if (VehicleUtils.CalculateTransformPosition(ref targetPosition, target, m_TransformData, m_PositionDataFromEntity, m_PrefabRefData, m_PrefabBuildingData))
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

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateLaneReservationsJob : IJob
	{
		public NativeQueue<AircraftNavigationHelpers.LaneReservation> m_LaneReservationQueue;

		public ComponentLookup<LaneReservation> m_LaneReservationData;

		public void Execute()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			AircraftNavigationHelpers.LaneReservation laneReservation = default(AircraftNavigationHelpers.LaneReservation);
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

		public NativeQueue<AircraftNavigationHelpers.LaneEffects> m_LaneEffectsQueue;

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
			AircraftNavigationHelpers.LaneEffects laneEffects = default(AircraftNavigationHelpers.LaneEffects);
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
		public ComponentTypeHandle<Aircraft> __Game_Vehicles_Aircraft_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Helicopter> __Game_Vehicles_Helicopter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<AircraftNavigation> __Game_Vehicles_AircraftNavigation_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Blocker> __Game_Vehicles_Blocker_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Odometer> __Game_Vehicles_Odometer_RW_ComponentTypeHandle;

		public BufferTypeHandle<AircraftNavigationLane> __Game_Vehicles_AircraftNavigationLane_RW_BufferTypeHandle;

		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RW_BufferTypeHandle;

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
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneReservation> __Game_Net_LaneReservation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TakeoffLocation> __Game_Routes_TakeoffLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Aircraft> __Game_Vehicles_Aircraft_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AircraftData> __Game_Prefabs_AircraftData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HelicopterData> __Game_Prefabs_HelicopterData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AirplaneData> __Game_Prefabs_AirplaneData_RO_ComponentLookup;

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

		public ComponentLookup<LaneReservation> __Game_Net_LaneReservation_RW_ComponentLookup;

		public ComponentLookup<Game.Net.Pollution> __Game_Net_Pollution_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_Moving_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Moving>(true);
			__Game_Common_Target_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(true);
			__Game_Vehicles_Aircraft_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Aircraft>(true);
			__Game_Vehicles_Helicopter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Helicopter>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Vehicles_AircraftNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftNavigation>(false);
			__Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftCurrentLane>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Vehicles_Blocker_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Blocker>(false);
			__Game_Vehicles_Odometer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Odometer>(false);
			__Game_Vehicles_AircraftNavigationLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AircraftNavigationLane>(false);
			__Game_Pathfind_PathElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_LaneReservation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneReservation>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Routes_TakeoffLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.TakeoffLocation>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Vehicles_Aircraft_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Aircraft>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_AircraftData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AircraftData>(true);
			__Game_Prefabs_HelicopterData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HelicopterData>(true);
			__Game_Prefabs_AirplaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AirplaneData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_VehicleSideEffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VehicleSideEffectData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Net_LaneReservation_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneReservation>(false);
			__Game_Net_Pollution_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Pollution>(false);
		}
	}

	private Game.Net.SearchSystem m_NetSearchSystem;

	private AirwaySystem m_AirwaySystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private EntityQuery m_VehicleQuery;

	private LaneObjectUpdater m_LaneObjectUpdater;

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
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_AirwaySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirwaySystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[11]
		{
			ComponentType.ReadOnly<Aircraft>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<Target>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadOnly<PathElement>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<AircraftNavigation>(),
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
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<AircraftNavigationHelpers.LaneReservation> laneReservationQueue = default(NativeQueue<AircraftNavigationHelpers.LaneReservation>);
		laneReservationQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<AircraftNavigationHelpers.LaneEffects> laneEffectsQueue = default(NativeQueue<AircraftNavigationHelpers.LaneEffects>);
		laneEffectsQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle deps;
		UpdateNavigationJob updateNavigationJob = new UpdateNavigationJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MovingType = InternalCompilerInterface.GetComponentTypeHandle<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftType = InternalCompilerInterface.GetComponentTypeHandle<Aircraft>(ref __TypeHandle.__Game_Vehicles_Aircraft_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HelicopterType = InternalCompilerInterface.GetComponentTypeHandle<Helicopter>(ref __TypeHandle.__Game_Vehicles_Helicopter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationType = InternalCompilerInterface.GetComponentTypeHandle<AircraftNavigation>(ref __TypeHandle.__Game_Vehicles_AircraftNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerType = InternalCompilerInterface.GetComponentTypeHandle<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OdometerType = InternalCompilerInterface.GetComponentTypeHandle<Odometer>(ref __TypeHandle.__Game_Vehicles_Odometer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<AircraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_AircraftNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneReservationData = InternalCompilerInterface.GetComponentLookup<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterData = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PositionDataFromEntity = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TakeoffLocationData = InternalCompilerInterface.GetComponentLookup<Game.Routes.TakeoffLocation>(ref __TypeHandle.__Game_Routes_TakeoffLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingDataFromEntity = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftData = InternalCompilerInterface.GetComponentLookup<Aircraft>(ref __TypeHandle.__Game_Vehicles_Aircraft_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAircraftData = InternalCompilerInterface.GetComponentLookup<AircraftData>(ref __TypeHandle.__Game_Prefabs_AircraftData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabHelicopterData = InternalCompilerInterface.GetComponentLookup<HelicopterData>(ref __TypeHandle.__Game_Prefabs_HelicopterData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAirplaneData = InternalCompilerInterface.GetComponentLookup<AirplaneData>(ref __TypeHandle.__Game_Prefabs_AirplaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSideEffectData = InternalCompilerInterface.GetComponentLookup<VehicleSideEffectData>(ref __TypeHandle.__Game_Prefabs_VehicleSideEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_MovingObjectSearchTree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: true, out dependencies2),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_AirwayData = m_AirwaySystem.GetAirwayData(),
			m_LaneObjectBuffer = m_LaneObjectUpdater.Begin((Allocator)3),
			m_LaneReservations = laneReservationQueue.AsParallelWriter(),
			m_LaneEffects = laneEffectsQueue.AsParallelWriter()
		};
		UpdateLaneReservationsJob updateLaneReservationsJob = new UpdateLaneReservationsJob
		{
			m_LaneReservationQueue = laneReservationQueue,
			m_LaneReservationData = InternalCompilerInterface.GetComponentLookup<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		ApplyLaneEffectsJob obj = new ApplyLaneEffectsJob
		{
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionData = InternalCompilerInterface.GetComponentLookup<Game.Net.Pollution>(ref __TypeHandle.__Game_Net_Pollution_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneEffectsQueue = laneEffectsQueue
		};
		JobHandle val = JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2);
		val = JobHandle.CombineDependencies(val, deps);
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateNavigationJob>(updateNavigationJob, m_VehicleQuery, val);
		JobHandle val3 = IJobExtensions.Schedule<UpdateLaneReservationsJob>(updateLaneReservationsJob, val2);
		JobHandle val4 = IJobExtensions.Schedule<ApplyLaneEffectsJob>(obj, val2);
		laneReservationQueue.Dispose(val3);
		laneEffectsQueue.Dispose(val4);
		m_NetSearchSystem.AddNetSearchTreeReader(val2);
		m_ObjectSearchSystem.AddMovingSearchTreeReader(val2);
		m_TerrainSystem.AddCPUHeightReader(val2);
		m_WaterSystem.AddSurfaceReader(val2);
		JobHandle val5 = m_LaneObjectUpdater.Apply((SystemBase)(object)this, val2);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val5, val3, val4);
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
	public AircraftNavigationSystem()
	{
	}
}

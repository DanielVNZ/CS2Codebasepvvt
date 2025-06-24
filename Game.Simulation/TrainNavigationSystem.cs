using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
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
public class TrainNavigationSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateNavigationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Target> m_TargetType;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> m_LayoutElementType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public ComponentTypeHandle<Blocker> m_BlockerType;

		public ComponentTypeHandle<Odometer> m_OdometerType;

		public BufferTypeHandle<TrainNavigationLane> m_NavigationLaneType;

		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingData;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<TrainNavigation> m_NavigationData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<TrainCurrentLane> m_CurrentLaneData;

		[ReadOnly]
		public ComponentLookup<Car> m_CarData;

		[ReadOnly]
		public ComponentLookup<Creature> m_CreatureData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.TrackLane> m_TrackLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<LaneReservation> m_LaneReservationData;

		[ReadOnly]
		public ComponentLookup<LaneSignal> m_LaneSignalData;

		[ReadOnly]
		public ComponentLookup<TrainData> m_PrefabTrainData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<VehicleSideEffectData> m_PrefabSideEffectData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_Lanes;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<LaneOverlap> m_LaneOverlaps;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		public LaneObjectCommandBuffer m_LaneObjectBuffer;

		public ParallelWriter<TrainNavigationHelpers.LaneEffects> m_LaneEffects;

		public ParallelWriter<TrainNavigationHelpers.LaneSignal> m_LaneSignals;

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
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Target> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<Blocker> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Blocker>(ref m_BlockerType);
			NativeArray<Odometer> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Odometer>(ref m_OdometerType);
			NativeArray<PathOwner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			BufferAccessor<TrainNavigationLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TrainNavigationLane>(ref m_NavigationLaneType);
			BufferAccessor<LayoutElement> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LayoutElement>(ref m_LayoutElementType);
			BufferAccessor<PathElement> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			NativeList<TrainNavigationHelpers.CurrentLaneCache> val = default(NativeList<TrainNavigationHelpers.CurrentLaneCache>);
			val._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			bool flag = nativeArray4.Length != 0;
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity controller = nativeArray[i];
				Target target = nativeArray2[i];
				Blocker blocker = nativeArray3[i];
				PathOwner pathOwner = nativeArray5[i];
				DynamicBuffer<TrainNavigationLane> navigationLanes = bufferAccessor[i];
				DynamicBuffer<LayoutElement> layout = bufferAccessor2[i];
				DynamicBuffer<PathElement> pathElements = bufferAccessor3[i];
				if (layout.Length != 0)
				{
					for (int j = 0; j < layout.Length; j++)
					{
						Entity vehicle = layout[j].m_Vehicle;
						TrainCurrentLane currentLane = m_CurrentLaneData[vehicle];
						TrainNavigationHelpers.CurrentLaneCache currentLaneCache = new TrainNavigationHelpers.CurrentLaneCache(ref currentLane, m_LaneData);
						val.Add(ref currentLaneCache);
						m_CurrentLaneData[vehicle] = currentLane;
					}
					Entity vehicle2 = layout[0].m_Vehicle;
					Transform transform = m_TransformData[vehicle2];
					Moving moving = m_MovingData[vehicle2];
					Train train = m_TrainData[vehicle2];
					TrainNavigation navigation = m_NavigationData[vehicle2];
					TrainCurrentLane currentLane2 = m_CurrentLaneData[vehicle2];
					PrefabRef prefabRef = m_PrefabRefData[vehicle2];
					TrainData prefabTrainData = m_PrefabTrainData[prefabRef.m_Prefab];
					ObjectGeometryData prefabObjectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
					int priority = VehicleUtils.GetPriority(prefabTrainData);
					Odometer odometer = default(Odometer);
					if (flag)
					{
						odometer = nativeArray4[i];
					}
					UpdateTrainLimits(ref prefabTrainData, layout);
					UpdateNavigationLanes(transform, train, target, prefabTrainData, ref currentLane2, ref blocker, ref pathOwner, navigationLanes, layout, pathElements);
					UpdateNavigationTarget(priority, controller, transform, moving, train, target, prefabRef, prefabTrainData, prefabObjectGeometryData, ref navigation, ref currentLane2, ref blocker, ref odometer, navigationLanes, layout);
					TryReserveNavigationLanes(train, prefabTrainData, ref navigation, ref currentLane2, navigationLanes);
					m_NavigationData[vehicle2] = navigation;
					m_CurrentLaneData[vehicle2] = currentLane2;
					for (int k = 0; k < layout.Length; k++)
					{
						Entity vehicle3 = layout[k].m_Vehicle;
						TrainCurrentLane currentLane3 = m_CurrentLaneData[vehicle3];
						val[k].CheckChanges(vehicle3, currentLane3, m_LaneObjectBuffer);
					}
					nativeArray5[i] = pathOwner;
					nativeArray3[i] = blocker;
					val.Clear();
					if (flag)
					{
						nativeArray4[i] = odometer;
					}
				}
			}
			val.Dispose();
		}

		private void UpdateNavigationLanes(Transform transform, Train train, Target target, TrainData prefabTrainData, ref TrainCurrentLane currentLane, ref Blocker blocker, ref PathOwner pathOwner, DynamicBuffer<TrainNavigationLane> navigationLanes, DynamicBuffer<LayoutElement> layout, DynamicBuffer<PathElement> pathElements)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			int invalidPath = 0;
			if (!HasValidLanes(currentLane))
			{
				invalidPath++;
				TryFindCurrentLane(ref currentLane, transform, train, prefabTrainData);
			}
			else if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete | PathFlags.Updated)) != 0 && (pathOwner.m_State & PathFlags.Append) == 0)
			{
				navigationLanes.Clear();
				currentLane.m_Front.m_LaneFlags &= ~TrainLaneFlags.Return;
			}
			else if ((pathOwner.m_State & PathFlags.Updated) == 0)
			{
				FillNavigationPaths(target, ref currentLane, ref blocker, ref pathOwner, navigationLanes, pathElements, ref invalidPath);
			}
			for (int i = 1; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				TrainCurrentLane currentLane2 = m_CurrentLaneData[vehicle];
				if (!HasValidLanes(currentLane2))
				{
					Transform transform2 = m_TransformData[vehicle];
					Train train2 = m_TrainData[vehicle];
					TryFindCurrentLane(ref currentLane2, transform2, train2, prefabTrainData);
					m_CurrentLaneData[vehicle] = currentLane2;
				}
			}
			if (invalidPath != 0)
			{
				navigationLanes.Clear();
				pathElements.Clear();
				pathOwner.m_ElementIndex = 0;
				pathOwner.m_State |= PathFlags.Obsolete;
				currentLane.m_Front.m_LaneFlags &= ~TrainLaneFlags.Return;
			}
		}

		private bool HasValidLanes(TrainCurrentLane currentLaneData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (currentLaneData.m_Front.m_Lane == Entity.Null)
			{
				return false;
			}
			if (currentLaneData.m_Rear.m_Lane == Entity.Null)
			{
				return false;
			}
			if (currentLaneData.m_FrontCache.m_Lane == Entity.Null)
			{
				return false;
			}
			if (currentLaneData.m_RearCache.m_Lane == Entity.Null)
			{
				return false;
			}
			if ((currentLaneData.m_Front.m_LaneFlags & TrainLaneFlags.Obsolete) != 0)
			{
				return false;
			}
			return true;
		}

		private void TryFindCurrentLane(ref TrainCurrentLane currentLane, Transform transform, Train train, TrainData prefabTrainData)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			currentLane.m_Front.m_LaneFlags &= ~TrainLaneFlags.Obsolete;
			currentLane.m_Front.m_Lane = Entity.Null;
			currentLane.m_Rear.m_Lane = Entity.Null;
			currentLane.m_FrontCache.m_Lane = Entity.Null;
			currentLane.m_RearCache.m_Lane = Entity.Null;
			VehicleUtils.CalculateTrainNavigationPivots(transform, prefabTrainData, out var pivot, out var pivot2);
			if ((train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0)
			{
				CommonUtils.Swap(ref pivot, ref pivot2);
			}
			float num = 100f;
			Bounds3 bounds = MathUtils.Expand(MathUtils.Bounds(pivot, pivot2), float3.op_Implicit(num));
			TrainNavigationHelpers.FindLaneIterator findLaneIterator = new TrainNavigationHelpers.FindLaneIterator
			{
				m_Bounds = bounds,
				m_FrontPivot = pivot,
				m_RearPivot = pivot2,
				m_MinDistance = float2.op_Implicit(num),
				m_Result = currentLane,
				m_TrackType = prefabTrainData.m_TrackType,
				m_SubLanes = m_Lanes,
				m_TrackLaneData = m_TrackLaneData,
				m_CurveData = m_CurveData,
				m_ConnectionLaneData = m_ConnectionLaneData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabTrackLaneData = m_PrefabTrackLaneData
			};
			m_NetSearchTree.Iterate<TrainNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			currentLane = findLaneIterator.m_Result;
		}

		private void FillNavigationPaths(Target target, ref TrainCurrentLane currentLane, ref Blocker blocker, ref PathOwner pathOwner, DynamicBuffer<TrainNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements, ref int invalidPath)
		{
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			if ((currentLane.m_Front.m_LaneFlags & TrainLaneFlags.EndOfPath) != 0)
			{
				return;
			}
			for (int i = 0; i < 10000; i++)
			{
				TrainNavigationLane trainNavigationLane;
				if (i >= navigationLanes.Length)
				{
					if (pathOwner.m_ElementIndex >= pathElements.Length || (pathOwner.m_ElementIndex + 1 >= pathElements.Length && (pathOwner.m_State & PathFlags.Pending) != 0))
					{
						break;
					}
					PathElement pathElement = pathElements[pathOwner.m_ElementIndex++];
					trainNavigationLane = new TrainNavigationLane
					{
						m_Lane = pathElement.m_Target,
						m_CurvePosition = pathElement.m_TargetDelta
					};
					if (m_TrackLaneData.HasComponent(trainNavigationLane.m_Lane))
					{
						Game.Net.TrackLane trackLane = m_TrackLaneData[trainNavigationLane.m_Lane];
						if (pathOwner.m_ElementIndex >= pathElements.Length)
						{
							trainNavigationLane.m_Flags |= TrainLaneFlags.EndOfPath;
						}
						else
						{
							if ((pathElement.m_Flags & PathElementFlags.Return) != 0)
							{
								trainNavigationLane.m_Flags |= TrainLaneFlags.Return;
							}
							if (((trackLane.m_Flags & (TrackLaneFlags.Twoway | TrackLaneFlags.Switch | TrackLaneFlags.DiamondCrossing | TrackLaneFlags.CrossingTraffic)) != 0 && (trackLane.m_Flags & TrackLaneFlags.MergingTraffic) == 0) || (pathElement.m_Flags & PathElementFlags.Reverse) != 0)
							{
								trainNavigationLane.m_Flags |= TrainLaneFlags.KeepClear;
							}
						}
						if ((trackLane.m_Flags & TrackLaneFlags.Exclusive) != 0)
						{
							trainNavigationLane.m_Flags |= TrainLaneFlags.Exclusive;
						}
						if ((trackLane.m_Flags & TrackLaneFlags.TurnLeft) != 0)
						{
							trainNavigationLane.m_Flags |= TrainLaneFlags.TurnLeft;
						}
						if ((trackLane.m_Flags & TrackLaneFlags.TurnRight) != 0)
						{
							trainNavigationLane.m_Flags |= TrainLaneFlags.TurnRight;
						}
						navigationLanes.Add(trainNavigationLane);
					}
					else
					{
						if (!m_ConnectionLaneData.HasComponent(trainNavigationLane.m_Lane))
						{
							if (((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(trainNavigationLane.m_Lane))
							{
								if (pathOwner.m_ElementIndex >= pathElements.Length)
								{
									if (navigationLanes.Length > 0)
									{
										TrainNavigationLane trainNavigationLane2 = navigationLanes[navigationLanes.Length - 1];
										trainNavigationLane2.m_Flags |= TrainLaneFlags.EndOfPath;
										navigationLanes[navigationLanes.Length - 1] = trainNavigationLane2;
									}
									else
									{
										currentLane.m_Front.m_LaneFlags |= TrainLaneFlags.EndOfPath;
									}
									trainNavigationLane.m_Flags |= TrainLaneFlags.ParkingSpace;
									navigationLanes.Add(trainNavigationLane);
									break;
								}
								continue;
							}
							invalidPath++;
							break;
						}
						trainNavigationLane.m_Flags |= TrainLaneFlags.Connection;
						if (pathOwner.m_ElementIndex >= pathElements.Length)
						{
							trainNavigationLane.m_Flags |= TrainLaneFlags.EndOfPath;
						}
						navigationLanes.Add(trainNavigationLane);
					}
				}
				else
				{
					trainNavigationLane = navigationLanes[i];
					if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(trainNavigationLane.m_Lane))
					{
						invalidPath++;
						break;
					}
				}
				if ((trainNavigationLane.m_Flags & TrainLaneFlags.EndOfPath) != 0 || (trainNavigationLane.m_Flags & (TrainLaneFlags.Reserved | TrainLaneFlags.KeepClear | TrainLaneFlags.Connection)) == 0)
				{
					break;
				}
			}
		}

		private void UpdateTrainLimits(ref TrainData prefabTrainData, DynamicBuffer<LayoutElement> layout)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 1; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				PrefabRef prefabRef = m_PrefabRefData[vehicle];
				TrainData trainData = m_PrefabTrainData[prefabRef.m_Prefab];
				prefabTrainData.m_MaxSpeed = math.min(prefabTrainData.m_MaxSpeed, trainData.m_MaxSpeed);
				prefabTrainData.m_Acceleration = math.min(prefabTrainData.m_Acceleration, trainData.m_Acceleration);
				prefabTrainData.m_Braking = math.min(prefabTrainData.m_Braking, trainData.m_Braking);
			}
		}

		private void UpdateNavigationTarget(int priority, Entity controller, Transform transform, Moving moving, Train train, Target target, PrefabRef prefabRef, TrainData prefabTrainData, ObjectGeometryData prefabObjectGeometryData, ref TrainNavigation navigation, ref TrainCurrentLane currentLane, ref Blocker blocker, ref Odometer odometer, DynamicBuffer<TrainNavigationLane> navigationLanes, DynamicBuffer<LayoutElement> layout)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_065a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_0711: Unknown result type (might be due to invalid IL or missing references)
			//IL_0716: Unknown result type (might be due to invalid IL or missing references)
			//IL_0909: Unknown result type (might be due to invalid IL or missing references)
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_0774: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_092d: Unknown result type (might be due to invalid IL or missing references)
			//IL_093a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cde: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e45: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7b: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			float num2 = navigation.m_Speed;
			bool flag = ((currentLane.m_Front.m_LaneFlags | currentLane.m_FrontCache.m_LaneFlags | currentLane.m_Rear.m_LaneFlags | currentLane.m_RearCache.m_LaneFlags) & TrainLaneFlags.Connection) != 0;
			for (int i = 1; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				TrainCurrentLane trainCurrentLane = m_CurrentLaneData[vehicle];
				flag |= ((trainCurrentLane.m_Front.m_LaneFlags | trainCurrentLane.m_FrontCache.m_LaneFlags | trainCurrentLane.m_Rear.m_LaneFlags | trainCurrentLane.m_RearCache.m_LaneFlags) & TrainLaneFlags.Connection) != 0;
			}
			if (flag)
			{
				prefabTrainData.m_MaxSpeed = 277.77777f;
				prefabTrainData.m_Acceleration = 277.77777f;
				prefabTrainData.m_Braking = 277.77777f;
			}
			else
			{
				num2 = math.min(num2, prefabTrainData.m_MaxSpeed);
			}
			Bounds1 val = default(Bounds1);
			if (flag || (currentLane.m_Front.m_LaneFlags & TrainLaneFlags.ResetSpeed) != 0)
			{
				((Bounds1)(ref val))._002Ector(0f, prefabTrainData.m_MaxSpeed);
			}
			else
			{
				val = VehicleUtils.CalculateSpeedRange(prefabTrainData, num2, num);
			}
			VehicleUtils.CalculateTrainNavigationPivots(transform, prefabTrainData, out var pivot, out var pivot2);
			if ((train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0)
			{
				CommonUtils.Swap(ref pivot, ref pivot2);
				prefabTrainData.m_BogieOffsets = ((float2)(ref prefabTrainData.m_BogieOffsets)).yx;
				prefabTrainData.m_AttachOffsets = ((float2)(ref prefabTrainData.m_AttachOffsets)).yx;
			}
			bool flag2 = blocker.m_Type == BlockerType.Temporary;
			TrainLaneSpeedIterator trainLaneSpeedIterator = new TrainLaneSpeedIterator
			{
				m_TransformData = m_TransformData,
				m_MovingData = m_MovingData,
				m_CarData = m_CarData,
				m_TrainData = m_TrainData,
				m_CurveData = m_CurveData,
				m_TrackLaneData = m_TrackLaneData,
				m_ControllerData = m_ControllerData,
				m_LaneReservationData = m_LaneReservationData,
				m_LaneSignalData = m_LaneSignalData,
				m_CreatureData = m_CreatureData,
				m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
				m_PrefabCarData = m_PrefabCarData,
				m_PrefabTrainData = m_PrefabTrainData,
				m_PrefabRefData = m_PrefabRefData,
				m_LaneOverlapData = m_LaneOverlaps,
				m_LaneObjectData = m_LaneObjects,
				m_Controller = controller,
				m_Priority = priority,
				m_TimeStep = num,
				m_SafeTimeStep = num + 0.5f,
				m_CurrentSpeed = num2,
				m_SpeedRange = val,
				m_RearPosition = pivot2,
				m_PushBlockers = ((currentLane.m_Front.m_LaneFlags & TrainLaneFlags.PushBlockers) != 0),
				m_MaxSpeed = val.max,
				m_CurrentPosition = pivot
			};
			if (currentLane.m_Front.m_Lane == Entity.Null)
			{
				navigation.m_Speed = math.max(0f, num2 - prefabTrainData.m_Braking * num);
				blocker.m_Blocker = Entity.Null;
				blocker.m_Type = BlockerType.None;
				blocker.m_MaxSpeed = byte.MaxValue;
				return;
			}
			if ((currentLane.m_Front.m_LaneFlags & TrainLaneFlags.HighBeams) == 0 && prefabTrainData.m_TrackType != TrackTypes.Tram && m_TrackLaneData.HasComponent(currentLane.m_Front.m_Lane) && (m_TrackLaneData[currentLane.m_Front.m_Lane].m_Flags & TrackLaneFlags.Station) == 0)
			{
				currentLane.m_Front.m_LaneFlags |= TrainLaneFlags.HighBeams;
			}
			bool flag3 = false;
			bool needSignal = false;
			for (int num3 = layout.Length - 1; num3 >= 1; num3--)
			{
				Entity vehicle2 = layout[num3].m_Vehicle;
				TrainCurrentLane trainCurrentLane2 = m_CurrentLaneData[vehicle2];
				PrefabRef prefabRef2 = m_PrefabRefData[vehicle2];
				TrainData prefabTrain = m_PrefabTrainData[prefabRef2.m_Prefab];
				trainLaneSpeedIterator.m_PrefabTrain = prefabTrain;
				trainLaneSpeedIterator.IteratePrevLane(trainCurrentLane2.m_RearCache.m_Lane, out needSignal);
				if (needSignal)
				{
					m_LaneSignals.Enqueue(new TrainNavigationHelpers.LaneSignal(controller, trainCurrentLane2.m_RearCache.m_Lane, priority));
				}
				trainLaneSpeedIterator.IteratePrevLane(trainCurrentLane2.m_Rear.m_Lane, out needSignal);
				if (needSignal)
				{
					m_LaneSignals.Enqueue(new TrainNavigationHelpers.LaneSignal(controller, trainCurrentLane2.m_Rear.m_Lane, priority));
				}
				trainLaneSpeedIterator.IteratePrevLane(trainCurrentLane2.m_FrontCache.m_Lane, out needSignal);
				if (needSignal)
				{
					m_LaneSignals.Enqueue(new TrainNavigationHelpers.LaneSignal(controller, trainCurrentLane2.m_FrontCache.m_Lane, priority));
				}
				trainLaneSpeedIterator.IteratePrevLane(trainCurrentLane2.m_Front.m_Lane, out needSignal);
				if (needSignal)
				{
					m_LaneSignals.Enqueue(new TrainNavigationHelpers.LaneSignal(controller, trainCurrentLane2.m_Front.m_Lane, priority));
				}
			}
			bool flag4 = (currentLane.m_Front.m_LaneFlags & TrainLaneFlags.Exclusive) != 0;
			bool skipCurrent = false;
			if (!flag4 && navigationLanes.Length != 0)
			{
				skipCurrent = (navigationLanes[0].m_Flags & (TrainLaneFlags.Reserved | TrainLaneFlags.Exclusive)) == (TrainLaneFlags.Reserved | TrainLaneFlags.Exclusive);
			}
			trainLaneSpeedIterator.m_PrefabTrain = prefabTrainData;
			trainLaneSpeedIterator.m_PrefabObjectGeometry = prefabObjectGeometryData;
			trainLaneSpeedIterator.IteratePrevLane(currentLane.m_RearCache.m_Lane, out needSignal);
			if (needSignal)
			{
				m_LaneSignals.Enqueue(new TrainNavigationHelpers.LaneSignal(controller, currentLane.m_RearCache.m_Lane, priority));
			}
			trainLaneSpeedIterator.IteratePrevLane(currentLane.m_Rear.m_Lane, out needSignal);
			if (needSignal)
			{
				m_LaneSignals.Enqueue(new TrainNavigationHelpers.LaneSignal(controller, currentLane.m_Rear.m_Lane, priority));
			}
			trainLaneSpeedIterator.IteratePrevLane(currentLane.m_FrontCache.m_Lane, out needSignal);
			if (needSignal)
			{
				m_LaneSignals.Enqueue(new TrainNavigationHelpers.LaneSignal(controller, currentLane.m_FrontCache.m_Lane, priority));
			}
			bool num4 = trainLaneSpeedIterator.IterateFirstLane(currentLane.m_Front.m_Lane, currentLane.m_Front.m_CurvePosition, flag4, flag, skipCurrent, out needSignal);
			if (needSignal)
			{
				m_LaneSignals.Enqueue(new TrainNavigationHelpers.LaneSignal(controller, currentLane.m_Front.m_Lane, priority));
			}
			if (!num4)
			{
				if ((currentLane.m_Front.m_LaneFlags & (TrainLaneFlags.EndOfPath | TrainLaneFlags.Return)) == 0)
				{
					int num5 = 0;
					while (num5 < navigationLanes.Length)
					{
						TrainNavigationLane trainNavigationLane = navigationLanes[num5];
						currentLane.m_Front.m_LaneFlags |= trainNavigationLane.m_Flags & (TrainLaneFlags.TurnLeft | TrainLaneFlags.TurnRight);
						bool flag5 = trainNavigationLane.m_Lane == currentLane.m_Front.m_Lane;
						if ((trainNavigationLane.m_Flags & (TrainLaneFlags.Reserved | TrainLaneFlags.Connection)) == 0)
						{
							while ((trainNavigationLane.m_Flags & (TrainLaneFlags.EndOfPath | TrainLaneFlags.BlockReserve)) == 0 && ++num5 < navigationLanes.Length)
							{
								trainNavigationLane = navigationLanes[num5];
							}
							trainLaneSpeedIterator.IterateTarget(trainNavigationLane.m_Lane, flag5);
						}
						else
						{
							if ((trainNavigationLane.m_Flags & TrainLaneFlags.Connection) != 0)
							{
								trainLaneSpeedIterator.m_PrefabTrain.m_MaxSpeed = 277.77777f;
								trainLaneSpeedIterator.m_PrefabTrain.m_Acceleration = 277.77777f;
								trainLaneSpeedIterator.m_PrefabTrain.m_Braking = 277.77777f;
								trainLaneSpeedIterator.m_SpeedRange = new Bounds1(0f, 277.77777f);
							}
							float minOffset = math.select(-1f, currentLane.m_Front.m_CurvePosition.z, flag5);
							bool num6 = trainLaneSpeedIterator.IterateNextLane(trainNavigationLane.m_Lane, trainNavigationLane.m_CurvePosition, minOffset, (trainNavigationLane.m_Flags & TrainLaneFlags.Exclusive) != 0, flag5 || flag, out needSignal);
							if (needSignal)
							{
								m_LaneSignals.Enqueue(new TrainNavigationHelpers.LaneSignal(controller, trainNavigationLane.m_Lane, priority));
							}
							if (!num6)
							{
								if ((trainNavigationLane.m_Flags & (TrainLaneFlags.EndOfPath | TrainLaneFlags.Return)) != 0)
								{
									break;
								}
								num5++;
								continue;
							}
						}
						goto IL_07ae;
					}
				}
				flag3 = trainLaneSpeedIterator.IterateTarget();
			}
			goto IL_07ae;
			IL_07ae:
			navigation.m_Speed = trainLaneSpeedIterator.m_MaxSpeed;
			float num7 = math.select(1.8360001f, 2.2949998f, (prefabTrainData.m_TrackType & TrackTypes.Tram) != 0);
			blocker.m_Blocker = trainLaneSpeedIterator.m_Blocker;
			blocker.m_Type = trainLaneSpeedIterator.m_BlockerType;
			blocker.m_MaxSpeed = (byte)math.clamp(Mathf.RoundToInt(trainLaneSpeedIterator.m_MaxSpeed * num7), 0, 255);
			bool num8 = blocker.m_Type == BlockerType.Temporary;
			if (num8 != flag2 || currentLane.m_Duration >= 30f)
			{
				ApplySideEffects(ref currentLane, currentLane.m_Front.m_Lane, prefabRef, prefabTrainData);
			}
			if (num8)
			{
				if (currentLane.m_Duration >= 5f)
				{
					currentLane.m_Front.m_LaneFlags |= TrainLaneFlags.PushBlockers;
				}
			}
			else if (currentLane.m_Duration >= 5f)
			{
				currentLane.m_Front.m_LaneFlags &= ~TrainLaneFlags.PushBlockers;
			}
			float num9 = num2 * num;
			currentLane.m_Duration += num;
			currentLane.m_Distance += num9;
			odometer.m_Distance += num9;
			TrainLaneFlags trainLaneFlags = TrainLaneFlags.EndOfPath | TrainLaneFlags.EndReached;
			if ((currentLane.m_Front.m_LaneFlags & trainLaneFlags) == trainLaneFlags)
			{
				return;
			}
			float num10 = navigation.m_Speed * num;
			TrainBogieCache tempCache = default(TrainBogieCache);
			bool resetCache = ShouldResetCache(currentLane.m_Front, currentLane.m_FrontCache);
			Game.Net.TrackLane trackLane = default(Game.Net.TrackLane);
			while (true)
			{
				Curve curve = m_CurveData[currentLane.m_Front.m_Lane];
				bool flag6 = curve.m_Length > 0.1f;
				if (flag6 && MoveTarget(pivot, ref navigation.m_Front, num10, curve.m_Bezier, ref currentLane.m_Front.m_CurvePosition))
				{
					if (!flag3 || !(navigation.m_Speed < 0.1f) || !(num2 < 0.1f))
					{
						break;
					}
					currentLane.m_Front.m_LaneFlags |= TrainLaneFlags.EndReached;
					if ((currentLane.m_Front.m_LaneFlags & (TrainLaneFlags.EndOfPath | TrainLaneFlags.Return)) != 0)
					{
						break;
					}
					for (int j = 0; j < navigationLanes.Length; j++)
					{
						TrainLaneFlags trainLaneFlags2 = navigationLanes[j].m_Flags & (TrainLaneFlags.EndOfPath | TrainLaneFlags.Return);
						if (trainLaneFlags2 != 0)
						{
							currentLane.m_Front.m_LaneFlags |= trainLaneFlags2;
							navigationLanes.RemoveRange(0, j + 1);
							break;
						}
					}
					break;
				}
				if (navigationLanes.Length == 0 || (currentLane.m_Front.m_LaneFlags & (TrainLaneFlags.EndOfPath | TrainLaneFlags.Return)) != 0)
				{
					if (flag3 && navigation.m_Speed < 0.1f && num2 < 0.1f)
					{
						currentLane.m_Front.m_LaneFlags |= TrainLaneFlags.EndReached;
					}
					break;
				}
				TrainNavigationLane navLane = navigationLanes[0];
				if ((navLane.m_Flags & (TrainLaneFlags.Reserved | TrainLaneFlags.Connection)) == 0 || !((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(navLane.m_Lane))
				{
					break;
				}
				if (flag && (navLane.m_Flags & TrainLaneFlags.Connection) == 0)
				{
					navLane.m_Flags |= TrainLaneFlags.ResetSpeed;
				}
				if ((currentLane.m_Front.m_LaneFlags & TrainLaneFlags.HighBeams) != 0 && prefabTrainData.m_TrackType != TrackTypes.Tram && m_TrackLaneData.TryGetComponent(navLane.m_Lane, ref trackLane) && (trackLane.m_Flags & TrackLaneFlags.Station) == 0)
				{
					navLane.m_Flags |= TrainLaneFlags.HighBeams;
				}
				if (flag6)
				{
					tempCache = currentLane.m_FrontCache;
					currentLane.m_FrontCache = new TrainBogieCache(currentLane.m_Front);
				}
				TrainLaneFlags trainLaneFlags3 = currentLane.m_Front.m_LaneFlags & TrainLaneFlags.PushBlockers;
				ApplySideEffects(ref currentLane, currentLane.m_Front.m_Lane, prefabRef, prefabTrainData);
				currentLane.m_Front = new TrainBogieLane(navLane);
				currentLane.m_Front.m_LaneFlags |= trainLaneFlags3;
				navigationLanes.RemoveAt(0);
			}
			ClampPosition(ref navigation.m_Front.m_Position, pivot, num10);
			navigation.m_Front.m_Direction = math.normalizesafe(navigation.m_Front.m_Direction, default(float3));
			float3 position = navigation.m_Front.m_Position;
			float num11 = math.csum(prefabTrainData.m_BogieOffsets);
			currentLane.m_Front.m_CurvePosition.z = currentLane.m_Front.m_CurvePosition.y;
			UpdateFollowerBogie(ref currentLane.m_Rear, ref currentLane.m_RearCache, ref navigation.m_Rear, ref resetCache, ref tempCache, ref currentLane.m_FrontCache, currentLane.m_Front, position, num11);
			if (layout.Length == 1)
			{
				currentLane.m_RearCache = new TrainBogieCache(currentLane.m_Rear);
			}
			else
			{
				position = navigation.m_Rear.m_Position;
				num11 = prefabTrainData.m_AttachOffsets.y - prefabTrainData.m_BogieOffsets.y;
			}
			TrainCurrentLane trainCurrentLane3 = currentLane;
			for (int k = 1; k < layout.Length; k++)
			{
				Entity vehicle3 = layout[k].m_Vehicle;
				Train train2 = m_TrainData[vehicle3];
				TrainCurrentLane currentLaneData = m_CurrentLaneData[vehicle3];
				TrainNavigation trainNavigation = m_NavigationData[vehicle3];
				PrefabRef prefabRefData = m_PrefabRefData[vehicle3];
				TrainData prefabTrainData2 = m_PrefabTrainData[prefabRefData.m_Prefab];
				if ((train2.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0)
				{
					prefabTrainData2.m_BogieOffsets = ((float2)(ref prefabTrainData2.m_BogieOffsets)).yx;
					prefabTrainData2.m_AttachOffsets = ((float2)(ref prefabTrainData2.m_AttachOffsets)).yx;
				}
				trainNavigation.m_Speed = navigation.m_Speed;
				currentLaneData.m_Duration += num;
				currentLaneData.m_Distance += num9;
				Entity lane = currentLaneData.m_Front.m_Lane;
				num11 += prefabTrainData2.m_AttachOffsets.x - prefabTrainData2.m_BogieOffsets.x;
				UpdateFollowerBogie(ref currentLaneData.m_Front, ref currentLaneData.m_FrontCache, ref trainNavigation.m_Front, ref resetCache, ref tempCache, ref trainCurrentLane3.m_RearCache, trainCurrentLane3.m_Rear, position, num11);
				if (currentLaneData.m_Front.m_Lane != lane || currentLaneData.m_Duration >= 30f)
				{
					ApplySideEffects(ref currentLaneData, lane, prefabRefData, prefabTrainData2);
				}
				position = trainNavigation.m_Front.m_Position;
				num11 = math.csum(prefabTrainData2.m_BogieOffsets);
				UpdateFollowerBogie(ref currentLaneData.m_Rear, ref currentLaneData.m_RearCache, ref trainNavigation.m_Rear, ref resetCache, ref tempCache, ref currentLaneData.m_FrontCache, currentLaneData.m_Front, position, num11);
				if (k == 1)
				{
					currentLane = trainCurrentLane3;
				}
				else
				{
					m_CurrentLaneData[layout[k - 1].m_Vehicle] = trainCurrentLane3;
				}
				if (k == layout.Length - 1)
				{
					currentLaneData.m_RearCache = new TrainBogieCache(currentLaneData.m_Rear);
				}
				else
				{
					position = trainNavigation.m_Rear.m_Position;
					num11 = prefabTrainData2.m_AttachOffsets.y - prefabTrainData2.m_BogieOffsets.y;
				}
				trainCurrentLane3 = currentLaneData;
				m_CurrentLaneData[vehicle3] = currentLaneData;
				m_NavigationData[vehicle3] = trainNavigation;
			}
		}

		private void ClampPosition(ref float3 position, float3 original, float maxDistance)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			position = original + MathUtils.ClampLength(position - original, maxDistance);
		}

		private bool ShouldResetCache(TrainBogieLane bogie, TrainBogieCache cache)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			if (math.all(bogie.m_CurvePosition == bogie.m_CurvePosition.x) && math.all(cache.m_CurvePosition == bogie.m_CurvePosition.x))
			{
				return cache.m_Lane == bogie.m_Lane;
			}
			return false;
		}

		private void UpdateFollowerBogie(ref TrainBogieLane bogie, ref TrainBogieCache cache, ref TrainBogiePosition position, ref bool resetCache, ref TrainBogieCache tempCache, ref TrainBogieCache nextCache, TrainBogieLane nextBogie, float3 followPosition, float followDistance)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			TrainBogieCache trainBogieCache = default(TrainBogieCache);
			float3 val = position.m_Position - followPosition;
			if (resetCache)
			{
				if (bogie.m_Lane == nextBogie.m_Lane)
				{
					tempCache = default(TrainBogieCache);
					nextCache = new TrainBogieCache(nextBogie);
					nextCache.m_CurvePosition.x = bogie.m_CurvePosition.w;
				}
				else if (bogie.m_Lane != Entity.Null && nextBogie.m_Lane != Entity.Null)
				{
					tempCache = new TrainBogieCache(bogie);
					nextCache = new TrainBogieCache(nextBogie);
					Curve curve = m_CurveData[bogie.m_Lane];
					Curve curve2 = m_CurveData[nextBogie.m_Lane];
					float3 val2 = MathUtils.Position(curve.m_Bezier, bogie.m_CurvePosition.w);
					float3 val3 = MathUtils.Position(curve2.m_Bezier, nextBogie.m_CurvePosition.x);
					MathUtils.Distance(curve.m_Bezier, val3, ref tempCache.m_CurvePosition.y);
					MathUtils.Distance(curve2.m_Bezier, val2, ref nextCache.m_CurvePosition.x);
				}
			}
			resetCache = ShouldResetCache(bogie, cache);
			while (true)
			{
				if (bogie.m_Lane != Entity.Null)
				{
					Curve curve3 = m_CurveData[bogie.m_Lane];
					if (bogie.m_Lane == nextBogie.m_Lane && bogie.m_CurvePosition.w == nextBogie.m_CurvePosition.w)
					{
						float w = bogie.m_CurvePosition.w;
						((float4)(ref bogie.m_CurvePosition)).zw = float2.op_Implicit(nextBogie.m_CurvePosition.y);
						if (MoveFollowerTarget(followPosition, ref position, followDistance, curve3.m_Bezier, ref bogie.m_CurvePosition))
						{
							bogie.m_CurvePosition.w = w;
							break;
						}
						bogie.m_CurvePosition.w = w;
					}
					else
					{
						bogie.m_CurvePosition.z = bogie.m_CurvePosition.w;
						if (MoveFollowerTarget(followPosition, ref position, followDistance, curve3.m_Bezier, ref bogie.m_CurvePosition))
						{
							break;
						}
					}
				}
				if (nextBogie.m_Lane == bogie.m_Lane)
				{
					float2 xw = ((float4)(ref nextBogie.m_CurvePosition)).xw;
					if (((float2)(ref xw)).Equals(((float4)(ref bogie.m_CurvePosition)).xw))
					{
						break;
					}
				}
				trainBogieCache = cache;
				cache = new TrainBogieCache(bogie);
				if (tempCache.m_Lane != Entity.Null)
				{
					bogie = new TrainBogieLane(tempCache);
					tempCache = default(TrainBogieCache);
				}
				else
				{
					bogie = new TrainBogieLane(nextCache);
					nextCache = new TrainBogieCache(nextBogie);
				}
			}
			float3 val4 = position.m_Position - followPosition;
			if (math.dot(val4, val) <= 0f)
			{
				val4 = val;
				position.m_Direction = -val;
			}
			if (MathUtils.TryNormalize(ref val4, followDistance))
			{
				position.m_Position = followPosition + val4;
				position.m_Direction = math.normalizesafe(position.m_Direction, default(float3));
			}
			tempCache = trainBogieCache;
		}

		private void ApplySideEffects(ref TrainCurrentLane currentLaneData, Entity lane, PrefabRef prefabRefData, TrainData prefabTrainData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			if (m_TrackLaneData.HasComponent(lane) && (currentLaneData.m_Duration != 0f || currentLaneData.m_Distance != 0f))
			{
				Game.Net.TrackLane trackLaneData = m_TrackLaneData[lane];
				Curve curve = m_CurveData[lane];
				float maxDriveSpeed = VehicleUtils.GetMaxDriveSpeed(prefabTrainData, trackLaneData);
				float num = 1f / math.max(1f, curve.m_Length);
				float3 sideEffects = default(float3);
				if (m_PrefabSideEffectData.HasComponent(prefabRefData.m_Prefab))
				{
					VehicleSideEffectData vehicleSideEffectData = m_PrefabSideEffectData[prefabRefData.m_Prefab];
					float num2 = math.select(currentLaneData.m_Distance / currentLaneData.m_Duration, maxDriveSpeed, currentLaneData.m_Duration == 0f) / prefabTrainData.m_MaxSpeed;
					num2 = math.saturate(num2 * num2);
					sideEffects = math.lerp(vehicleSideEffectData.m_Min, vehicleSideEffectData.m_Max, num2);
					float num3 = math.min(1f, currentLaneData.m_Distance * num);
					sideEffects *= new float3(num3, currentLaneData.m_Duration, currentLaneData.m_Duration);
				}
				maxDriveSpeed = math.min(prefabTrainData.m_MaxSpeed, trackLaneData.m_SpeedLimit);
				float2 flow = new float2(currentLaneData.m_Duration * maxDriveSpeed, currentLaneData.m_Distance) * num;
				m_LaneEffects.Enqueue(new TrainNavigationHelpers.LaneEffects(lane, sideEffects, flow));
			}
			currentLaneData.m_Duration = 0f;
			currentLaneData.m_Distance = 0f;
		}

		private void TryReserveNavigationLanes(Train trainData, TrainData prefabTrainData, ref TrainNavigation navigationData, ref TrainCurrentLane currentLaneData, DynamicBuffer<TrainNavigationLane> navigationLanes)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			float timeStep = 4f / 15f;
			if ((trainData.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0)
			{
				prefabTrainData.m_BogieOffsets = ((float2)(ref prefabTrainData.m_BogieOffsets)).yx;
				prefabTrainData.m_AttachOffsets = ((float2)(ref prefabTrainData.m_AttachOffsets)).yx;
			}
			if (!(currentLaneData.m_Front.m_Lane != Entity.Null))
			{
				return;
			}
			Curve curve = m_CurveData[currentLaneData.m_Front.m_Lane];
			float brakingDistance = VehicleUtils.GetBrakingDistance(prefabTrainData, navigationData.m_Speed, timeStep);
			brakingDistance = math.max(0f, brakingDistance - 0.01f);
			float num = brakingDistance;
			float num2 = prefabTrainData.m_AttachOffsets.x - prefabTrainData.m_BogieOffsets.x + 2f;
			num2 += VehicleUtils.GetSignalDistance(prefabTrainData, navigationData.m_Speed);
			if (currentLaneData.m_Front.m_CurvePosition.w > currentLaneData.m_Front.m_CurvePosition.x)
			{
				currentLaneData.m_Front.m_CurvePosition.z = currentLaneData.m_Front.m_CurvePosition.y + num / math.max(1E-06f, curve.m_Length);
				currentLaneData.m_Front.m_CurvePosition.z = math.min(currentLaneData.m_Front.m_CurvePosition.z, currentLaneData.m_Front.m_CurvePosition.w);
			}
			else
			{
				currentLaneData.m_Front.m_CurvePosition.z = currentLaneData.m_Front.m_CurvePosition.y - num / math.max(1E-06f, curve.m_Length);
				currentLaneData.m_Front.m_CurvePosition.z = math.max(currentLaneData.m_Front.m_CurvePosition.z, currentLaneData.m_Front.m_CurvePosition.w);
			}
			num -= curve.m_Length * math.abs(currentLaneData.m_Front.m_CurvePosition.w - currentLaneData.m_Front.m_CurvePosition.y);
			int num3 = 0;
			bool flag = num > 0f;
			bool flag2 = num + num2 > 0f || (currentLaneData.m_Front.m_LaneFlags & TrainLaneFlags.KeepClear) != 0;
			while (flag2 && num3 < navigationLanes.Length)
			{
				TrainNavigationLane trainNavigationLane = navigationLanes[num3];
				if ((trainNavigationLane.m_Flags & TrainLaneFlags.ParkingSpace) != 0)
				{
					break;
				}
				if (m_TrackLaneData.HasComponent(trainNavigationLane.m_Lane))
				{
					trainNavigationLane.m_Flags |= TrainLaneFlags.TryReserve;
					if (flag)
					{
						trainNavigationLane.m_Flags |= TrainLaneFlags.FullReserve;
					}
					else
					{
						trainNavigationLane.m_Flags &= ~TrainLaneFlags.FullReserve;
					}
					navigationLanes[num3] = trainNavigationLane;
				}
				num -= m_CurveData[trainNavigationLane.m_Lane].m_Length * math.abs(trainNavigationLane.m_CurvePosition.y - trainNavigationLane.m_CurvePosition.x);
				flag = num > 0f;
				flag2 = num + num2 > 0f || (trainNavigationLane.m_Flags & TrainLaneFlags.KeepClear) != 0;
				num3++;
			}
		}

		private bool MoveTarget(float3 comparePosition, ref TrainBogiePosition targetPosition, float minDistance, Bezier4x3 curve, ref float4 curveDelta)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.Position(curve, curveDelta.w);
			if (math.distance(comparePosition, val) < minDistance)
			{
				float num = math.lerp(curveDelta.y, curveDelta.w, 0.5f);
				float3 val2 = MathUtils.Position(curve, num);
				if (math.distance(comparePosition, val2) < minDistance)
				{
					curveDelta.y = curveDelta.w;
					targetPosition.m_Position = val;
					targetPosition.m_Direction = MathUtils.Tangent(curve, curveDelta.w);
					ref float3 direction = ref targetPosition.m_Direction;
					direction *= math.sign(curveDelta.w - curveDelta.x);
					return false;
				}
			}
			float3 val3 = MathUtils.Position(curve, curveDelta.y);
			if (math.distance(comparePosition, val3) >= minDistance)
			{
				targetPosition.m_Position = val3;
				targetPosition.m_Direction = MathUtils.Tangent(curve, curveDelta.y);
				ref float3 direction2 = ref targetPosition.m_Direction;
				direction2 *= math.sign(curveDelta.w - curveDelta.x);
				return true;
			}
			float2 yw = ((float4)(ref curveDelta)).yw;
			for (int i = 0; i < 8; i++)
			{
				float num2 = math.lerp(yw.x, yw.y, 0.5f);
				float3 val4 = MathUtils.Position(curve, num2);
				if (math.distance(comparePosition, val4) < minDistance)
				{
					yw.x = num2;
				}
				else
				{
					yw.y = num2;
				}
			}
			curveDelta.y = yw.y;
			targetPosition.m_Position = MathUtils.Position(curve, yw.y);
			targetPosition.m_Direction = MathUtils.Tangent(curve, yw.y);
			ref float3 direction3 = ref targetPosition.m_Direction;
			direction3 *= math.sign(curveDelta.w - curveDelta.x);
			return true;
		}

		private bool MoveFollowerTarget(float3 comparePosition, ref TrainBogiePosition targetPosition, float maxDistance, Bezier4x3 curve, ref float4 curveDelta)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.Position(curve, curveDelta.w);
			if (math.distance(comparePosition, val) > maxDistance)
			{
				curveDelta.y = curveDelta.w;
				targetPosition.m_Position = val;
				targetPosition.m_Direction = MathUtils.Tangent(curve, curveDelta.w);
				ref float3 direction = ref targetPosition.m_Direction;
				direction *= math.sign(curveDelta.w - curveDelta.x);
				return false;
			}
			float2 yw = ((float4)(ref curveDelta)).yw;
			for (int i = 0; i < 8; i++)
			{
				float num = math.lerp(yw.x, yw.y, 0.5f);
				float3 val2 = MathUtils.Position(curve, num);
				if (math.distance(comparePosition, val2) > maxDistance)
				{
					yw.x = num;
				}
				else
				{
					yw.y = num;
				}
			}
			curveDelta.y = yw.x;
			targetPosition.m_Position = MathUtils.Position(curve, yw.x);
			targetPosition.m_Direction = MathUtils.Tangent(curve, yw.x);
			ref float3 direction2 = ref targetPosition.m_Direction;
			direction2 *= math.sign(curveDelta.w - curveDelta.x);
			return true;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateLaneSignalsJob : IJob
	{
		public NativeQueue<TrainNavigationHelpers.LaneSignal> m_LaneSignalQueue;

		public ComponentLookup<LaneSignal> m_LaneSignalData;

		public void Execute()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			int count = m_LaneSignalQueue.Count;
			for (int i = 0; i < count; i++)
			{
				TrainNavigationHelpers.LaneSignal laneSignal = m_LaneSignalQueue.Dequeue();
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
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> m_LayoutType;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> m_CurrentLaneData;

		[ReadOnly]
		public ComponentLookup<TrainData> m_PrefabTrainData;

		public ComponentLookup<LaneReservation> m_LaneReservationData;

		public BufferTypeHandle<TrainNavigationLane> m_NavigationLaneType;

		public void Execute()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ReserveCurrentLanes(m_Chunks[i]);
			}
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				TryReserveNavigationLanes(m_Chunks[j]);
			}
		}

		private void ReserveCurrentLanes(ArchetypeChunk chunk)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<LayoutElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LayoutElement>(ref m_LayoutType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<LayoutElement> val = bufferAccessor[i];
				Entity prevLane = Entity.Null;
				for (int j = 0; j < val.Length; j++)
				{
					Entity vehicle = val[j].m_Vehicle;
					TrainCurrentLane currentLaneData = m_CurrentLaneData[vehicle];
					ReserveCurrentLanes(vehicle, currentLaneData, ref prevLane, 98);
				}
			}
		}

		private void ReserveCurrentLanes(Entity entity, TrainCurrentLane currentLaneData, ref Entity prevLane, int priority)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			if (currentLaneData.m_Front.m_Lane != Entity.Null && currentLaneData.m_Front.m_Lane != prevLane)
			{
				ReserveLane(entity, currentLaneData.m_Front.m_Lane, priority);
			}
			if (currentLaneData.m_FrontCache.m_Lane != Entity.Null && currentLaneData.m_FrontCache.m_Lane != currentLaneData.m_Front.m_Lane)
			{
				ReserveLane(entity, currentLaneData.m_FrontCache.m_Lane, priority);
			}
			if (currentLaneData.m_Rear.m_Lane != Entity.Null && currentLaneData.m_Rear.m_Lane != currentLaneData.m_FrontCache.m_Lane)
			{
				ReserveLane(entity, currentLaneData.m_Rear.m_Lane, priority);
			}
			if (currentLaneData.m_RearCache.m_Lane != Entity.Null && currentLaneData.m_RearCache.m_Lane != currentLaneData.m_Rear.m_Lane)
			{
				ReserveLane(entity, currentLaneData.m_RearCache.m_Lane, priority);
			}
			prevLane = currentLaneData.m_RearCache.m_Lane;
		}

		private void ReserveLane(Entity entity, Entity lane, int priority)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			if (!m_LaneReservationData.HasComponent(lane))
			{
				return;
			}
			ref LaneReservation valueRW = ref m_LaneReservationData.GetRefRW(lane).ValueRW;
			if (priority > valueRW.m_Next.m_Priority)
			{
				if (priority >= valueRW.m_Prev.m_Priority)
				{
					valueRW.m_Blocker = entity;
				}
				valueRW.m_Next.m_Priority = (byte)priority;
			}
		}

		private void TryReserveNavigationLanes(ArchetypeChunk chunk)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<LayoutElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LayoutElement>(ref m_LayoutType);
			BufferAccessor<TrainNavigationLane> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TrainNavigationLane>(ref m_NavigationLaneType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				PrefabRef prefabRef = nativeArray[i];
				DynamicBuffer<LayoutElement> layout = bufferAccessor[i];
				DynamicBuffer<TrainNavigationLane> navigationLanes = bufferAccessor2[i];
				if (layout.Length >= 1)
				{
					Entity vehicle = layout[0].m_Vehicle;
					TrainCurrentLane trainCurrentLane = m_CurrentLaneData[vehicle];
					int priority = VehicleUtils.GetPriority(m_PrefabTrainData[prefabRef.m_Prefab]);
					TryReserveNavigationLanes(vehicle, navigationLanes, layout, trainCurrentLane.m_Front.m_Lane, 98, priority);
				}
			}
		}

		private void TryReserveNavigationLanes(Entity entity, DynamicBuffer<TrainNavigationLane> navigationLanes, DynamicBuffer<LayoutElement> layout, Entity prevLane, int priority, int fullPriority)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			Entity val = prevLane;
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < navigationLanes.Length; i++)
			{
				ref TrainNavigationLane reference = ref navigationLanes.ElementAt(i);
				if ((reference.m_Flags & (TrainLaneFlags.Reserved | TrainLaneFlags.TryReserve | TrainLaneFlags.Connection)) == 0)
				{
					break;
				}
				if ((reference.m_Flags & (TrainLaneFlags.Reserved | TrainLaneFlags.Connection)) != 0)
				{
					num = i;
					num2 = i;
				}
				else
				{
					if (!(reference.m_Lane == prevLane) && (reference.m_Flags & TrainLaneFlags.Exclusive) != 0 && !CanReserveLane(reference.m_Lane, layout))
					{
						reference.m_Flags |= TrainLaneFlags.BlockReserve;
						num2 = num;
						break;
					}
					reference.m_Flags &= ~TrainLaneFlags.BlockReserve;
					num = math.select(num, i, num == i - 1 && reference.m_Lane == prevLane);
					num2 = i;
				}
				prevLane = reference.m_Lane;
			}
			prevLane = val;
			for (int j = 0; j <= num2; j++)
			{
				ref TrainNavigationLane reference2 = ref navigationLanes.ElementAt(j);
				if (reference2.m_Lane != prevLane)
				{
					bool flag = (reference2.m_Flags & (TrainLaneFlags.TryReserve | TrainLaneFlags.FullReserve)) == (TrainLaneFlags.TryReserve | TrainLaneFlags.FullReserve);
					int priority2 = math.select(priority, fullPriority, flag);
					ReserveLane(entity, reference2.m_Lane, priority2);
				}
				if ((reference2.m_Flags & TrainLaneFlags.TryReserve) != 0)
				{
					reference2.m_Flags &= ~(TrainLaneFlags.TryReserve | TrainLaneFlags.FullReserve);
					reference2.m_Flags |= TrainLaneFlags.Reserved;
				}
				prevLane = reference2.m_Lane;
			}
		}

		private bool CanReserveLane(Entity lane, DynamicBuffer<LayoutElement> layout)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			if (m_LaneReservationData.HasComponent(lane) && m_LaneReservationData[lane].GetPriority() != 0)
			{
				for (int i = 0; i < layout.Length; i++)
				{
					Entity vehicle = layout[i].m_Vehicle;
					TrainCurrentLane trainCurrentLane = m_CurrentLaneData[vehicle];
					if (trainCurrentLane.m_Front.m_Lane == lane || trainCurrentLane.m_FrontCache.m_Lane == lane || trainCurrentLane.m_Rear.m_Lane == lane || trainCurrentLane.m_RearCache.m_Lane == lane)
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}
	}

	[BurstCompile]
	private struct ApplyLaneEffectsJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<LaneDeteriorationData> m_LaneDeteriorationData;

		public ComponentLookup<Game.Net.Pollution> m_PollutionData;

		public ComponentLookup<LaneCondition> m_LaneConditionData;

		public ComponentLookup<LaneFlow> m_LaneFlowData;

		public NativeQueue<TrainNavigationHelpers.LaneEffects> m_LaneEffectsQueue;

		public void Execute()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			int count = m_LaneEffectsQueue.Count;
			for (int i = 0; i < count; i++)
			{
				TrainNavigationHelpers.LaneEffects laneEffects = m_LaneEffectsQueue.Dequeue();
				Entity owner = m_OwnerData[laneEffects.m_Lane].m_Owner;
				if (m_LaneConditionData.HasComponent(laneEffects.m_Lane))
				{
					PrefabRef prefabRef = m_PrefabRefData[laneEffects.m_Lane];
					LaneDeteriorationData laneDeteriorationData = m_LaneDeteriorationData[prefabRef.m_Prefab];
					LaneCondition laneCondition = m_LaneConditionData[laneEffects.m_Lane];
					laneCondition.m_Wear = math.min(laneCondition.m_Wear + laneEffects.m_SideEffects.x * laneDeteriorationData.m_TrafficFactor, 10f);
					m_LaneConditionData[laneEffects.m_Lane] = laneCondition;
				}
				if (m_LaneFlowData.HasComponent(laneEffects.m_Lane))
				{
					LaneFlow laneFlow = m_LaneFlowData[laneEffects.m_Lane];
					ref float2 next = ref laneFlow.m_Next;
					next += laneEffects.m_Flow;
					m_LaneFlowData[laneEffects.m_Lane] = laneFlow;
				}
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
		public ComponentTypeHandle<Target> __Game_Common_Target_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Blocker> __Game_Vehicles_Blocker_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Odometer> __Game_Vehicles_Odometer_RW_ComponentTypeHandle;

		public BufferTypeHandle<TrainNavigationLane> __Game_Vehicles_TrainNavigationLane_RW_BufferTypeHandle;

		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		public ComponentLookup<TrainNavigation> __Game_Vehicles_TrainNavigation_RW_ComponentLookup;

		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Car> __Game_Vehicles_Car_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Creature> __Game_Creatures_Creature_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.TrackLane> __Game_Net_TrackLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneReservation> __Game_Net_LaneReservation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneSignal> __Game_Net_LaneSignal_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainData> __Game_Prefabs_TrainData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<VehicleSideEffectData> __Game_Prefabs_VehicleSideEffectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentLookup;

		public ComponentLookup<LaneReservation> __Game_Net_LaneReservation_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneDeteriorationData> __Game_Prefabs_LaneDeteriorationData_RO_ComponentLookup;

		public ComponentLookup<Game.Net.Pollution> __Game_Net_Pollution_RW_ComponentLookup;

		public ComponentLookup<LaneCondition> __Game_Net_LaneCondition_RW_ComponentLookup;

		public ComponentLookup<LaneFlow> __Game_Net_LaneFlow_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Target_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(true);
			__Game_Vehicles_LayoutElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LayoutElement>(true);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Vehicles_Blocker_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Blocker>(false);
			__Game_Vehicles_Odometer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Odometer>(false);
			__Game_Vehicles_TrainNavigationLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TrainNavigationLane>(false);
			__Game_Pathfind_PathElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Vehicles_TrainNavigation_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainNavigation>(false);
			__Game_Vehicles_TrainCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(false);
			__Game_Vehicles_Car_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Car>(true);
			__Game_Creatures_Creature_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creature>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_TrackLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.TrackLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_LaneReservation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneReservation>(true);
			__Game_Net_LaneSignal_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(true);
			__Game_Prefabs_TrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainData>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_VehicleSideEffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VehicleSideEffectData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(true);
			__Game_Net_LaneReservation_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneReservation>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Prefabs_LaneDeteriorationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneDeteriorationData>(true);
			__Game_Net_Pollution_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Pollution>(false);
			__Game_Net_LaneCondition_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneCondition>(false);
			__Game_Net_LaneFlow_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneFlow>(false);
			__Game_Net_LaneSignal_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(false);
		}
	}

	private Game.Net.SearchSystem m_NetSearchSystem;

	private EntityQuery m_VehicleQuery;

	private LaneObjectUpdater m_LaneObjectUpdater;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 3;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[14]
		{
			ComponentType.ReadOnly<Train>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<Moving>(),
			ComponentType.ReadOnly<Target>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<PathElement>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<TrainCurrentLane>(),
			ComponentType.ReadWrite<TrainNavigation>(),
			ComponentType.ReadWrite<TrainNavigationLane>(),
			ComponentType.ReadWrite<LayoutElement>(),
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
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<TrainNavigationHelpers.LaneEffects> laneEffectsQueue = default(NativeQueue<TrainNavigationHelpers.LaneEffects>);
		laneEffectsQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<TrainNavigationHelpers.LaneSignal> laneSignalQueue = default(NativeQueue<TrainNavigationHelpers.LaneSignal>);
		laneSignalQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_VehicleQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle dependencies;
		UpdateNavigationJob updateNavigationJob = new UpdateNavigationJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElementType = InternalCompilerInterface.GetBufferTypeHandle<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerType = InternalCompilerInterface.GetComponentTypeHandle<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OdometerType = InternalCompilerInterface.GetComponentTypeHandle<Odometer>(ref __TypeHandle.__Game_Vehicles_Odometer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<TrainNavigationLane>(ref __TypeHandle.__Game_Vehicles_TrainNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationData = InternalCompilerInterface.GetComponentLookup<TrainNavigation>(ref __TypeHandle.__Game_Vehicles_TrainNavigation_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneData = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarData = InternalCompilerInterface.GetComponentLookup<Car>(ref __TypeHandle.__Game_Vehicles_Car_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureData = InternalCompilerInterface.GetComponentLookup<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneReservationData = InternalCompilerInterface.GetComponentLookup<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainData = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSideEffectData = InternalCompilerInterface.GetComponentLookup<VehicleSideEffectData>(ref __TypeHandle.__Game_Prefabs_VehicleSideEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_LaneObjectBuffer = m_LaneObjectUpdater.Begin((Allocator)3),
			m_LaneEffects = laneEffectsQueue.AsParallelWriter(),
			m_LaneSignals = laneSignalQueue.AsParallelWriter()
		};
		UpdateLaneReservationsJob updateLaneReservationsJob = new UpdateLaneReservationsJob
		{
			m_Chunks = chunks,
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutType = InternalCompilerInterface.GetBufferTypeHandle<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneData = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainData = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneReservationData = InternalCompilerInterface.GetComponentLookup<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<TrainNavigationLane>(ref __TypeHandle.__Game_Vehicles_TrainNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		ApplyLaneEffectsJob applyLaneEffectsJob = new ApplyLaneEffectsJob
		{
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneDeteriorationData = InternalCompilerInterface.GetComponentLookup<LaneDeteriorationData>(ref __TypeHandle.__Game_Prefabs_LaneDeteriorationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionData = InternalCompilerInterface.GetComponentLookup<Game.Net.Pollution>(ref __TypeHandle.__Game_Net_Pollution_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneConditionData = InternalCompilerInterface.GetComponentLookup<LaneCondition>(ref __TypeHandle.__Game_Net_LaneCondition_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneFlowData = InternalCompilerInterface.GetComponentLookup<LaneFlow>(ref __TypeHandle.__Game_Net_LaneFlow_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneEffectsQueue = laneEffectsQueue
		};
		UpdateLaneSignalsJob obj = new UpdateLaneSignalsJob
		{
			m_LaneSignalQueue = laneSignalQueue,
			m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateNavigationJob>(updateNavigationJob, m_VehicleQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		JobHandle val3 = IJobExtensions.Schedule<UpdateLaneReservationsJob>(updateLaneReservationsJob, JobHandle.CombineDependencies(val2, val));
		JobHandle val4 = IJobExtensions.Schedule<ApplyLaneEffectsJob>(applyLaneEffectsJob, val2);
		JobHandle val5 = IJobExtensions.Schedule<UpdateLaneSignalsJob>(obj, val2);
		laneEffectsQueue.Dispose(val4);
		laneSignalQueue.Dispose(val5);
		chunks.Dispose(val3);
		m_NetSearchSystem.AddNetSearchTreeReader(val2);
		JobHandle val6 = m_LaneObjectUpdater.Apply((SystemBase)(object)this, val2);
		((SystemBase)this).Dependency = JobUtils.CombineDependencies(val6, val3, val5, val4);
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
	public TrainNavigationSystem()
	{
	}
}

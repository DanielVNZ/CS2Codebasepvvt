using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Routes;

[CompilerGenerated]
public class SegmentCurveSystem : GameSystemBase
{
	[BurstCompile]
	private struct FindUpdatedSegmentsJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PathUpdated> m_PathUpdatedType;

		[ReadOnly]
		public BufferLookup<CurveElement> m_CurveElements;

		public NativeList<Entity> m_SegmentList;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				int num2 = num;
				ArchetypeChunk val = m_Chunks[i];
				num = num2 + ((ArchetypeChunk)(ref val)).Count;
			}
			NativeHashSet<Entity> val2 = default(NativeHashSet<Entity>);
			val2._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			m_SegmentList.Capacity = num;
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val3 = m_Chunks[j];
				NativeArray<PathUpdated> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<PathUpdated>(ref m_PathUpdatedType);
				if (nativeArray.Length != 0)
				{
					for (int k = 0; k < nativeArray.Length; k++)
					{
						PathUpdated pathUpdated = nativeArray[k];
						if (m_CurveElements.HasBuffer(pathUpdated.m_Owner) && val2.Add(pathUpdated.m_Owner))
						{
							m_SegmentList.Add(ref pathUpdated.m_Owner);
						}
					}
					continue;
				}
				NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray(m_EntityType);
				for (int l = 0; l < nativeArray2.Length; l++)
				{
					Entity val4 = nativeArray2[l];
					if (val2.Add(val4))
					{
						m_SegmentList.Add(ref val4);
					}
				}
			}
			val2.Dispose();
		}
	}

	[BurstCompile]
	private struct UpdateSegmentCurvesJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeArray<Entity> m_SegmentList;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Segment> m_SegmentData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<TransportStop> m_TransportStopData;

		[ReadOnly]
		public ComponentLookup<PathTargets> m_PathTargetsData;

		[ReadOnly]
		public ComponentLookup<PathSource> m_PathSourceData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<PathOwner> m_PathOwnerData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> m_CarCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<WatercraftCurrentLane> m_WatercraftCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<AircraftCurrentLane> m_AircraftCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> m_TrainCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Taxi> m_TaxiData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PersonalCar> m_PersonalCarData;

		[ReadOnly]
		public ComponentLookup<Aircraft> m_AircraftData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<GroupMember> m_GroupMemberData;

		[ReadOnly]
		public ComponentLookup<HumanCurrentLane> m_HumanCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RouteData> m_PrefabRouteData;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public BufferLookup<CarNavigationLane> m_CarNavigationLanes;

		[ReadOnly]
		public BufferLookup<WatercraftNavigationLane> m_WatercraftNavigationLanes;

		[ReadOnly]
		public BufferLookup<AircraftNavigationLane> m_AircraftNavigationLanes;

		[ReadOnly]
		public BufferLookup<TrainNavigationLane> m_TrainNavigationLanes;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[NativeDisableParallelForRestriction]
		public BufferLookup<CurveElement> m_CurveElements;

		[NativeDisableParallelForRestriction]
		public BufferLookup<CurveSource> m_CurveSources;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0630: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_0705: Unknown result type (might be due to invalid IL or missing references)
			//IL_0707: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_070b: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_0660: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_SegmentList[index];
			DynamicBuffer<CurveElement> curveElements = m_CurveElements[val];
			DynamicBuffer<CurveSource> curveSources = default(DynamicBuffer<CurveSource>);
			m_CurveSources.TryGetBuffer(val, ref curveSources);
			Owner owner = default(Owner);
			if (!m_OwnerData.TryGetComponent(val, ref owner))
			{
				curveElements.Clear();
				if (curveSources.IsCreated)
				{
					curveSources.Clear();
				}
				return;
			}
			Entity owner2 = owner.m_Owner;
			Segment segment = m_SegmentData[val];
			PrefabRef prefabRef = m_PrefabRefData[owner2];
			DynamicBuffer<RouteWaypoint> val2 = m_RouteWaypoints[owner2];
			RouteData routeData = m_PrefabRouteData[prefabRef.m_Prefab];
			float nodeDistance = routeData.m_Width * 2.5f;
			float segmentLength = routeData.m_SegmentLength;
			float3 lastPosition = default(float3);
			float3 nextNodePos = default(float3);
			float3 lastTangent = default(float3);
			bool isFirst = true;
			bool airway = false;
			bool area = false;
			bool hasLastPos = false;
			bool hasNextPos = false;
			int connectionCount = 0;
			if (val2.Length >= 2)
			{
				Entity waypoint = val2[segment.m_Index].m_Waypoint;
				Entity waypoint2 = val2[math.select(segment.m_Index + 1, 0, segment.m_Index + 1 >= val2.Length)].m_Waypoint;
				lastPosition = m_PositionData[waypoint].m_Position;
				nextNodePos = m_PositionData[waypoint2].m_Position;
				PathTargets pathTargets = default(PathTargets);
				if (m_PathTargetsData.TryGetComponent(val, ref pathTargets))
				{
					lastPosition = pathTargets.m_ReadyStartPosition;
					nextNodePos = pathTargets.m_ReadyEndPosition;
				}
				hasLastPos = true;
				hasNextPos = true;
				curveElements.Clear();
				DynamicBuffer<PathElement> path = default(DynamicBuffer<PathElement>);
				if (m_PathElements.TryGetBuffer(val, ref path))
				{
					TryAddSegments(curveElements, curveSources, path, default(PathOwner), lastPosition, nextNodePos, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, isPedestrian: false, skipAirway: true, skipArea: true, stayMidAir: false, ref isFirst, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
				}
				TryAddSegments(curveElements, curveSources, nextNodePos, default(float3), nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref airway, ref area);
			}
			else
			{
				PathSource pathSource = default(PathSource);
				if (!m_PathSourceData.TryGetComponent(val, ref pathSource))
				{
					return;
				}
				bool flag = false;
				bool flag2 = false;
				bool isPedestrian = false;
				bool skipAirway = true;
				bool skipArea = true;
				bool stayMidAir = false;
				m_UpdatedData.HasComponent(val);
				curveElements.Clear();
				curveSources.Clear();
				GroupMember groupMember = default(GroupMember);
				m_GroupMemberData.TryGetComponent(pathSource.m_Entity, ref groupMember);
				CarCurrentLane carCurrentLane = default(CarCurrentLane);
				DynamicBuffer<CarNavigationLane> navLanes = default(DynamicBuffer<CarNavigationLane>);
				WatercraftCurrentLane watercraftCurrentLane = default(WatercraftCurrentLane);
				DynamicBuffer<WatercraftNavigationLane> navLanes2 = default(DynamicBuffer<WatercraftNavigationLane>);
				AircraftCurrentLane aircraftCurrentLane = default(AircraftCurrentLane);
				DynamicBuffer<AircraftNavigationLane> navLanes3 = default(DynamicBuffer<AircraftNavigationLane>);
				DynamicBuffer<TrainNavigationLane> navLanes4 = default(DynamicBuffer<TrainNavigationLane>);
				DynamicBuffer<LayoutElement> val3 = default(DynamicBuffer<LayoutElement>);
				TrainCurrentLane trainCurrentLane = default(TrainCurrentLane);
				HumanCurrentLane humanCurrentLane = default(HumanCurrentLane);
				CurrentVehicle currentVehicle = default(CurrentVehicle);
				if (m_CarCurrentLaneData.TryGetComponent(pathSource.m_Entity, ref carCurrentLane) && m_CarNavigationLanes.TryGetBuffer(pathSource.m_Entity, ref navLanes))
				{
					skipArea = false;
					flag = TryAddSegments(curveElements, curveSources, carCurrentLane, navLanes, lastPosition, nextNodePos, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref isFirst, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
				}
				else if (m_WatercraftCurrentLaneData.TryGetComponent(pathSource.m_Entity, ref watercraftCurrentLane) && m_WatercraftNavigationLanes.TryGetBuffer(pathSource.m_Entity, ref navLanes2))
				{
					skipArea = false;
					flag = TryAddSegments(curveElements, curveSources, watercraftCurrentLane, navLanes2, lastPosition, nextNodePos, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref isFirst, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
				}
				else if (m_AircraftCurrentLaneData.TryGetComponent(pathSource.m_Entity, ref aircraftCurrentLane) && m_AircraftNavigationLanes.TryGetBuffer(pathSource.m_Entity, ref navLanes3))
				{
					airway = (aircraftCurrentLane.m_LaneFlags & AircraftLaneFlags.Flying) != 0;
					skipAirway = false;
					skipArea = false;
					stayMidAir = (m_AircraftData[pathSource.m_Entity].m_Flags & AircraftFlags.StayMidAir) != 0;
					flag = TryAddSegments(curveElements, curveSources, aircraftCurrentLane, navLanes3, lastPosition, nextNodePos, nodeDistance, segmentLength, stayMidAir, ref lastPosition, ref lastTangent, ref isFirst, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
				}
				else if (m_TrainNavigationLanes.TryGetBuffer(pathSource.m_Entity, ref navLanes4) && m_LayoutElements.TryGetBuffer(pathSource.m_Entity, ref val3) && val3.Length != 0 && m_TrainCurrentLaneData.TryGetComponent(val3[0].m_Vehicle, ref trainCurrentLane))
				{
					flag = TryAddSegments(curveElements, curveSources, trainCurrentLane, navLanes4, lastPosition, nextNodePos, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref isFirst, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
				}
				else if (m_HumanCurrentLaneData.TryGetComponent(pathSource.m_Entity, ref humanCurrentLane))
				{
					skipArea = false;
					flag = TryAddSegments(curveElements, curveSources, humanCurrentLane, lastPosition, nextNodePos, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref isFirst, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
					isPedestrian = true;
				}
				else if (m_CurrentVehicleData.TryGetComponent(pathSource.m_Entity, ref currentVehicle))
				{
					Controller controller = default(Controller);
					if (m_ControllerData.TryGetComponent(currentVehicle.m_Vehicle, ref controller) && controller.m_Controller != Entity.Null)
					{
						currentVehicle.m_Vehicle = controller.m_Controller;
					}
					if (m_TaxiData.HasComponent(currentVehicle.m_Vehicle) || m_PersonalCarData.HasComponent(currentVehicle.m_Vehicle))
					{
						pathSource.m_Entity = currentVehicle.m_Vehicle;
						flag2 = true;
					}
					else if (m_PublicTransportData.HasComponent(currentVehicle.m_Vehicle))
					{
						flag2 = true;
					}
					else
					{
						flag = true;
					}
					isPedestrian = true;
				}
				PathElement pathElement = default(PathElement);
				DynamicBuffer<PathElement> path2 = default(DynamicBuffer<PathElement>);
				if (!flag && m_PathElements.TryGetBuffer(pathSource.m_Entity, ref path2))
				{
					PathOwner pathOwner = default(PathOwner);
					m_PathOwnerData.TryGetComponent(pathSource.m_Entity, ref pathOwner);
					if (flag2)
					{
						while (pathOwner.m_ElementIndex < path2.Length && !IsPedestrianTarget(path2[pathOwner.m_ElementIndex].m_Target))
						{
							pathOwner.m_ElementIndex++;
						}
					}
					else if (pathOwner.m_ElementIndex < path2.Length)
					{
						pathElement = path2[path2.Length - 1];
					}
					TryAddSegments(curveElements, curveSources, path2, pathOwner, lastPosition, nextNodePos, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, isPedestrian, skipAirway, skipArea, stayMidAir, ref isFirst, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
				}
				if (!flag && m_PathElements.TryGetBuffer(groupMember.m_Leader, ref path2))
				{
					PathOwner pathOwner2 = default(PathOwner);
					m_PathOwnerData.TryGetComponent(groupMember.m_Leader, ref pathOwner2);
					if (flag2)
					{
						while (pathOwner2.m_ElementIndex < path2.Length && !IsPedestrianTarget(path2[pathOwner2.m_ElementIndex].m_Target))
						{
							pathOwner2.m_ElementIndex++;
						}
					}
					else if (pathElement.m_Target != Entity.Null)
					{
						for (int i = pathOwner2.m_ElementIndex; i < path2.Length; i++)
						{
							PathElement pathElement2 = path2[i];
							if (pathElement2.m_Target == pathElement.m_Target && ((float2)(ref pathElement2.m_TargetDelta)).Equals(pathElement.m_TargetDelta))
							{
								pathOwner2.m_ElementIndex = i + 1;
								break;
							}
						}
					}
					TryAddSegments(curveElements, curveSources, path2, pathOwner2, lastPosition, nextNodePos, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, isPedestrian, skipAirway, skipArea, stayMidAir, ref isFirst, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
				}
				if (connectionCount != 0)
				{
					ProcessConnectionSegments(curveElements, curveSources, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
				}
				if (airway && hasLastPos && curveElements.Length == 0)
				{
					curveElements.Add(new CurveElement
					{
						m_Curve = new Bezier4x3(lastPosition, lastPosition, lastPosition, lastPosition)
					});
					if (curveSources.IsCreated)
					{
						curveSources.Add(default(CurveSource));
					}
				}
			}
		}

		private bool TryAddSegments(DynamicBuffer<CurveElement> curveElements, DynamicBuffer<CurveSource> curveSources, CarCurrentLane carCurrentLane, DynamicBuffer<CarNavigationLane> navLanes, float3 lastNodePos, float3 nextNodePos, float nodeDistance, float segmentLength, ref float3 lastPosition, ref float3 lastTangent, ref bool isFirst, ref bool airway, ref bool area, ref bool hasLastPos, ref bool hasNextPos, ref int connectionCount)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			int i = 0;
			bool airway2 = false;
			bool area2 = false;
			bool result = false;
			if (hasNextPos)
			{
				i = navLanes.Length;
			}
			else
			{
				if (ShouldEndPath(carCurrentLane.m_Lane, isPedestrian: false))
				{
					return true;
				}
				for (; i < navLanes.Length; i++)
				{
					if (ShouldEndPath(navLanes[i].m_Lane, isPedestrian: false))
					{
						result = true;
						break;
					}
				}
			}
			for (int num = --i; num >= 0; num--)
			{
				CarNavigationLane carNavigationLane = navLanes[num];
				GetMasterLane(ref carNavigationLane.m_Lane);
				if (IsLast(carNavigationLane.m_Lane, carNavigationLane.m_CurvePosition, nextNodePos, nodeDistance, hasNextPos, skipAirway: true, skipArea: false, ref airway2, ref area2))
				{
					i = num;
					break;
				}
			}
			if (i == -1 && !IsLast(carCurrentLane.m_Lane, ((float3)(ref carCurrentLane.m_CurvePosition)).xz, nextNodePos, nodeDistance, hasNextPos, skipAirway: true, skipArea: false, ref airway2, ref area2))
			{
				i = -2;
			}
			if (i >= -1)
			{
				GetMasterLane(ref carCurrentLane.m_Lane);
				TryAddSegments(curveElements, curveSources, lastNodePos, nextNodePos, nodeDistance, segmentLength, carCurrentLane.m_Lane, ((float3)(ref carCurrentLane.m_CurvePosition)).xz, i == -1, skipAirway: true, skipArea: false, stayMidAir: false, ref isFirst, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
			}
			for (int j = 0; j <= i; j++)
			{
				CarNavigationLane carNavigationLane2 = navLanes[j];
				GetMasterLane(ref carNavigationLane2.m_Lane);
				TryAddSegments(curveElements, curveSources, lastNodePos, nextNodePos, nodeDistance, segmentLength, carNavigationLane2.m_Lane, carNavigationLane2.m_CurvePosition, j == i, skipAirway: true, skipArea: false, stayMidAir: false, ref isFirst, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
			}
			airway |= airway2;
			area |= area2;
			return result;
		}

		private bool TryAddSegments(DynamicBuffer<CurveElement> curveElements, DynamicBuffer<CurveSource> curveSources, WatercraftCurrentLane watercraftCurrentLane, DynamicBuffer<WatercraftNavigationLane> navLanes, float3 lastNodePos, float3 nextNodePos, float nodeDistance, float segmentLength, ref float3 lastPosition, ref float3 lastTangent, ref bool isFirst, ref bool airway, ref bool area, ref bool hasLastPos, ref bool hasNextPos, ref int connectionCount)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			int i = 0;
			bool airway2 = false;
			bool area2 = false;
			bool result = false;
			if (hasNextPos)
			{
				i = navLanes.Length;
			}
			else
			{
				if (ShouldEndPath(watercraftCurrentLane.m_Lane, isPedestrian: false))
				{
					return true;
				}
				for (; i < navLanes.Length; i++)
				{
					if (ShouldEndPath(navLanes[i].m_Lane, isPedestrian: false))
					{
						result = true;
						break;
					}
				}
			}
			for (int num = --i; num >= 0; num--)
			{
				WatercraftNavigationLane watercraftNavigationLane = navLanes[num];
				GetMasterLane(ref watercraftNavigationLane.m_Lane);
				if (IsLast(watercraftNavigationLane.m_Lane, watercraftNavigationLane.m_CurvePosition, nextNodePos, nodeDistance, hasNextPos, skipAirway: true, skipArea: false, ref airway2, ref area2))
				{
					i = num;
					break;
				}
			}
			if (i == -1 && !IsLast(watercraftCurrentLane.m_Lane, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).xz, nextNodePos, nodeDistance, hasNextPos, skipAirway: true, skipArea: false, ref airway2, ref area2))
			{
				i = -2;
			}
			if (i >= -1)
			{
				GetMasterLane(ref watercraftCurrentLane.m_Lane);
				TryAddSegments(curveElements, curveSources, lastNodePos, nextNodePos, nodeDistance, segmentLength, watercraftCurrentLane.m_Lane, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).xz, i == -1, skipAirway: true, skipArea: false, stayMidAir: false, ref isFirst, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
			}
			for (int j = 0; j <= i; j++)
			{
				WatercraftNavigationLane watercraftNavigationLane2 = navLanes[j];
				GetMasterLane(ref watercraftNavigationLane2.m_Lane);
				TryAddSegments(curveElements, curveSources, lastNodePos, nextNodePos, nodeDistance, segmentLength, watercraftNavigationLane2.m_Lane, watercraftNavigationLane2.m_CurvePosition, j == i, skipAirway: true, skipArea: false, stayMidAir: false, ref isFirst, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
			}
			airway |= airway2;
			area |= area2;
			return result;
		}

		private bool TryAddSegments(DynamicBuffer<CurveElement> curveElements, DynamicBuffer<CurveSource> curveSources, AircraftCurrentLane aircraftCurrentLane, DynamicBuffer<AircraftNavigationLane> navLanes, float3 lastNodePos, float3 nextNodePos, float nodeDistance, float segmentLength, bool stayMidAir, ref float3 lastPosition, ref float3 lastTangent, ref bool isFirst, ref bool airway, ref bool area, ref bool hasLastPos, ref bool hasNextPos, ref int connectionCount)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			int i = 0;
			bool flag = false;
			bool area2 = false;
			bool airway2 = false;
			bool result = false;
			if (hasNextPos)
			{
				i = navLanes.Length;
			}
			else
			{
				if (ShouldEndPath(aircraftCurrentLane.m_Lane, isPedestrian: false))
				{
					return true;
				}
				for (; i < navLanes.Length; i++)
				{
					if (ShouldEndPath(navLanes[i].m_Lane, isPedestrian: false))
					{
						result = true;
						break;
					}
				}
			}
			for (int num = --i; num >= 0; num--)
			{
				AircraftNavigationLane aircraftNavigationLane = navLanes[num];
				airway2 = false;
				if (IsLast(aircraftNavigationLane.m_Lane, aircraftNavigationLane.m_CurvePosition, nextNodePos, nodeDistance, hasNextPos, skipAirway: false, skipArea: false, ref airway2, ref area2))
				{
					i = num;
					break;
				}
				flag = flag || airway2;
			}
			if (i == -1)
			{
				airway2 = false;
				if (!IsLast(aircraftCurrentLane.m_Lane, ((float3)(ref aircraftCurrentLane.m_CurvePosition)).xz, nextNodePos, nodeDistance, hasNextPos, skipAirway: false, skipArea: false, ref airway2, ref area2))
				{
					i = -2;
					flag = flag || airway2;
				}
			}
			if (i == -2)
			{
				airway2 = airway;
			}
			if (airway2 && i + 1 < navLanes.Length)
			{
				i++;
			}
			if (i >= -1)
			{
				TryAddSegments(curveElements, curveSources, lastNodePos, nextNodePos, nodeDistance, segmentLength, aircraftCurrentLane.m_Lane, ((float3)(ref aircraftCurrentLane.m_CurvePosition)).xz, i == -1, skipAirway: false, skipArea: false, stayMidAir, ref isFirst, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
			}
			for (int j = 0; j <= i; j++)
			{
				AircraftNavigationLane aircraftNavigationLane2 = navLanes[j];
				TryAddSegments(curveElements, curveSources, lastNodePos, nextNodePos, nodeDistance, segmentLength, aircraftNavigationLane2.m_Lane, aircraftNavigationLane2.m_CurvePosition, j == i, skipAirway: false, skipArea: false, stayMidAir, ref isFirst, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
			}
			airway |= flag;
			area |= area2;
			return result;
		}

		private bool TryAddSegments(DynamicBuffer<CurveElement> curveElements, DynamicBuffer<CurveSource> curveSources, TrainCurrentLane trainCurrentLane, DynamicBuffer<TrainNavigationLane> navLanes, float3 lastNodePos, float3 nextNodePos, float nodeDistance, float segmentLength, ref float3 lastPosition, ref float3 lastTangent, ref bool isFirst, ref bool airway, ref bool area, ref bool hasLastPos, ref bool hasNextPos, ref int connectionCount)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			int i = 0;
			bool airway2 = false;
			bool area2 = false;
			bool result = false;
			if (hasNextPos)
			{
				i = navLanes.Length;
			}
			else
			{
				if (ShouldEndPath(trainCurrentLane.m_Front.m_Lane, isPedestrian: false))
				{
					return true;
				}
				for (; i < navLanes.Length; i++)
				{
					if (ShouldEndPath(navLanes[i].m_Lane, isPedestrian: false))
					{
						result = true;
						break;
					}
				}
			}
			for (int num = --i; num >= 0; num--)
			{
				TrainNavigationLane trainNavigationLane = navLanes[num];
				if (IsLast(trainNavigationLane.m_Lane, trainNavigationLane.m_CurvePosition, nextNodePos, nodeDistance, hasNextPos, skipAirway: true, skipArea: true, ref airway2, ref area2))
				{
					i = num;
					break;
				}
			}
			if (i == -1 && !IsLast(trainCurrentLane.m_Front.m_Lane, ((float4)(ref trainCurrentLane.m_Front.m_CurvePosition)).yw, nextNodePos, nodeDistance, hasNextPos, skipAirway: true, skipArea: true, ref airway2, ref area2))
			{
				i = -2;
			}
			if (i >= -1)
			{
				TryAddSegments(curveElements, curveSources, lastNodePos, nextNodePos, nodeDistance, segmentLength, trainCurrentLane.m_Front.m_Lane, ((float4)(ref trainCurrentLane.m_Front.m_CurvePosition)).yw, i == -1, skipAirway: true, skipArea: true, stayMidAir: false, ref isFirst, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
			}
			for (int j = 0; j <= i; j++)
			{
				TrainNavigationLane trainNavigationLane2 = navLanes[j];
				TryAddSegments(curveElements, curveSources, lastNodePos, nextNodePos, nodeDistance, segmentLength, trainNavigationLane2.m_Lane, trainNavigationLane2.m_CurvePosition, j == i, skipAirway: true, skipArea: true, stayMidAir: false, ref isFirst, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
			}
			airway |= airway2;
			area |= area2;
			return result;
		}

		private bool TryAddSegments(DynamicBuffer<CurveElement> curveElements, DynamicBuffer<CurveSource> curveSources, HumanCurrentLane humanCurrentLane, float3 lastNodePos, float3 nextNodePos, float nodeDistance, float segmentLength, ref float3 lastPosition, ref float3 lastTangent, ref bool isFirst, ref bool airway, ref bool area, ref bool hasLastPos, ref bool hasNextPos, ref int connectionCount)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (!hasNextPos && ShouldEndPath(humanCurrentLane.m_Lane, isPedestrian: true))
			{
				return true;
			}
			TryAddSegments(curveElements, curveSources, lastNodePos, nextNodePos, nodeDistance, segmentLength, humanCurrentLane.m_Lane, humanCurrentLane.m_CurvePosition, isLast: true, skipAirway: true, skipArea: false, stayMidAir: false, ref isFirst, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
			return false;
		}

		private void TryAddSegments(DynamicBuffer<CurveElement> curveElements, DynamicBuffer<CurveSource> curveSources, DynamicBuffer<PathElement> path, PathOwner pathOwner, float3 lastNodePos, float3 nextNodePos, float nodeDistance, float segmentLength, ref float3 lastPosition, ref float3 lastTangent, bool isPedestrian, bool skipAirway, bool skipArea, bool stayMidAir, ref bool isFirst, ref bool airway, ref bool area, ref bool hasLastPos, ref bool hasNextPos, ref int connectionCount)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			int i = pathOwner.m_ElementIndex;
			bool flag = false;
			bool area2 = false;
			bool airway2 = false;
			if (hasNextPos)
			{
				i = path.Length;
			}
			else
			{
				for (; i < path.Length && !ShouldEndPath(path[i].m_Target, isPedestrian); i++)
				{
				}
			}
			for (int num = --i; num >= pathOwner.m_ElementIndex; num--)
			{
				PathElement pathElement = path[num];
				airway2 = false;
				if (IsLast(pathElement.m_Target, pathElement.m_TargetDelta, nextNodePos, nodeDistance, hasNextPos, skipAirway, skipArea, ref airway2, ref area2))
				{
					i = num;
					break;
				}
				flag = flag || airway2;
			}
			if (i == pathOwner.m_ElementIndex - 1)
			{
				airway2 = airway;
			}
			if (!skipAirway && airway2 && i + 1 < path.Length)
			{
				i++;
			}
			for (int j = pathOwner.m_ElementIndex; j <= i; j++)
			{
				PathElement pathElement2 = path[j];
				TryAddSegments(curveElements, curveSources, lastNodePos, nextNodePos, nodeDistance, segmentLength, pathElement2.m_Target, pathElement2.m_TargetDelta, j == i, skipAirway, skipArea, stayMidAir, ref isFirst, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
			}
			airway |= flag;
			area |= area2;
		}

		private bool IsLast(Entity target, float2 targetDelta, float3 nextNodePos, float nodeDistance, bool hasNextPos, bool skipAirway, bool skipArea, ref bool airway, ref bool area)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (ShouldSkipTarget(target, skipAirway, skipArea, ref airway, ref area))
			{
				return false;
			}
			Bezier4x3 val = MathUtils.Cut(m_CurveData[target].m_Bezier, targetDelta);
			if (!hasNextPos || math.distance(nextNodePos, val.a) > nodeDistance)
			{
				return true;
			}
			return false;
		}

		private void TryAddSegments(DynamicBuffer<CurveElement> curveElements, DynamicBuffer<CurveSource> curveSources, float3 lastNodePos, float3 nextNodePos, float nodeDistance, float segmentLength, Entity target, float2 targetDelta, bool isLast, bool skipAirway, bool skipArea, bool stayMidAir, ref bool isFirst, ref float3 lastPosition, ref float3 lastTangent, ref bool airway, ref bool area, ref bool hasLastPos, ref bool hasNextPos, ref int connectionCount)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			bool airway2 = false;
			bool area2 = false;
			if (ShouldSkipTarget(target, skipAirway, skipArea, ref airway2, ref area2))
			{
				if (!skipAirway & airway)
				{
					Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
					Owner owner = default(Owner);
					if (m_ConnectionLaneData.TryGetComponent(target, ref connectionLane) && (connectionLane.m_Flags & (ConnectionLaneFlags.Start | ConnectionLaneFlags.Outside)) == (ConnectionLaneFlags.Start | ConnectionLaneFlags.Outside) && m_OwnerData.TryGetComponent(target, ref owner))
					{
						target = owner.m_Owner;
					}
					Transform transform = default(Transform);
					if (m_TransformData.TryGetComponent(target, ref transform))
					{
						if (connectionCount != 0)
						{
							ProcessConnectionSegments(curveElements, curveSources, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
						}
						if (stayMidAir)
						{
							transform.m_Position.y += 100f;
						}
						if (hasLastPos)
						{
							airway = false;
							area = false;
							TryAddSegments(curveElements, curveSources, transform.m_Position, default(float3), nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref airway, ref area);
						}
						else
						{
							lastPosition = transform.m_Position;
							lastTangent = default(float3);
							hasLastPos = true;
						}
					}
				}
				airway |= airway2;
				area |= area2;
				return;
			}
			Curve curve = m_CurveData[target];
			Bezier4x3 val = MathUtils.Cut(curve.m_Bezier, targetDelta);
			if (airway2 || area2)
			{
				if (airway2 && !hasLastPos && connectionCount == 0)
				{
					val = ((!(targetDelta.y < targetDelta.x) && targetDelta.y != 0f) ? curve.m_Bezier : MathUtils.Invert(curve.m_Bezier));
				}
				curveElements.Add(new CurveElement
				{
					m_Curve = val
				});
				if (curveSources.IsCreated)
				{
					curveSources.Add(default(CurveSource));
				}
				connectionCount++;
				airway |= airway2;
				area |= area2;
				return;
			}
			if (connectionCount != 0)
			{
				ProcessConnectionSegments(curveElements, curveSources, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos, ref hasNextPos, ref connectionCount);
				airway = false;
				area = false;
			}
			if (isFirst)
			{
				if (hasLastPos && math.distance(lastNodePos, val.a) < nodeDistance)
				{
					if (math.distance(lastNodePos, val.d) <= nodeDistance)
					{
						return;
					}
					float num = MoveCurvePosition(lastNodePos, nodeDistance, val);
					val = MathUtils.Cut(val, new float2(num, 1f));
					targetDelta.x = math.lerp(targetDelta.x, targetDelta.y, num);
				}
				isFirst = false;
			}
			if (isLast && hasNextPos && math.distance(nextNodePos, val.d) < nodeDistance)
			{
				if (math.distance(nextNodePos, val.a) <= nodeDistance)
				{
					return;
				}
				float num2 = MoveCurvePosition(nextNodePos, nodeDistance, MathUtils.Invert(val));
				val = MathUtils.Cut(val, new float2(0f, 1f - num2));
				targetDelta.y = math.lerp(targetDelta.y, targetDelta.x, num2);
			}
			TryAddSegments(curveElements, curveSources, target, targetDelta, val, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref airway, ref area, ref hasLastPos);
		}

		private void ProcessConnectionSegments(DynamicBuffer<CurveElement> curveElements, DynamicBuffer<CurveSource> curveSources, float nodeDistance, float segmentLength, ref float3 lastPosition, ref float3 lastTangent, ref bool airway, ref bool area, ref bool hasLastPos, ref bool hasNextPos, ref int connectionCount)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			int num = curveElements.Length - connectionCount;
			bool airway2 = false;
			bool area2 = false;
			if (area && connectionCount == 1)
			{
				CurveElement curveElement = curveElements[num];
				float3 val = MathUtils.Position(curveElement.m_Curve, 0.5f);
				float3 val2 = MathUtils.Tangent(curveElement.m_Curve, 0.5f);
				if (hasLastPos)
				{
					TryAddSegments(curveElements, curveSources, val, val2, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref airway2, ref area2);
				}
				else
				{
					lastPosition = val;
					lastTangent = val2;
					hasLastPos = true;
				}
			}
			for (int i = 1; i < connectionCount; i++)
			{
				CurveElement curveElement2 = curveElements[num + i - 1];
				CurveElement curveElement3 = curveElements[num + i];
				float3 val3 = (curveElement2.m_Curve.a + curveElement2.m_Curve.d + curveElement3.m_Curve.a + curveElement3.m_Curve.d) * 0.25f;
				float3 val4 = curveElement3.m_Curve.d - curveElement2.m_Curve.a;
				if (hasLastPos)
				{
					TryAddSegments(curveElements, curveSources, val3, val4, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref airway2, ref area2);
					continue;
				}
				lastPosition = val3;
				lastTangent = val4;
				hasLastPos = true;
			}
			curveElements.RemoveRange(num, connectionCount);
			if (curveSources.IsCreated)
			{
				curveSources.RemoveRange(num, connectionCount);
			}
			connectionCount = 0;
		}

		private void GetMasterLane(ref Entity lane)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			SlaveLane slaveLane = default(SlaveLane);
			Owner owner = default(Owner);
			DynamicBuffer<Game.Net.SubLane> val = default(DynamicBuffer<Game.Net.SubLane>);
			if (m_SlaveLaneData.TryGetComponent(lane, ref slaveLane) && m_OwnerData.TryGetComponent(lane, ref owner) && m_SubLanes.TryGetBuffer(owner.m_Owner, ref val) && val.Length > slaveLane.m_MasterIndex)
			{
				lane = val[(int)slaveLane.m_MasterIndex].m_SubLane;
			}
		}

		private bool ShouldEndPath(Entity target, bool isPedestrian)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
			if (!m_ParkingLaneData.HasComponent(target) && !(isPedestrian ? m_CarLaneData.HasComponent(target) : m_PedestrianLaneData.HasComponent(target)) && (!m_ConnectionLaneData.TryGetComponent(target, ref connectionLane) || ((uint)connectionLane.m_Flags & (uint)(isPedestrian ? 288 : 384)) == 0) && !m_PositionData.HasComponent(target))
			{
				return m_TransportStopData.HasComponent(target);
			}
			return true;
		}

		private bool IsPedestrianTarget(Entity target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PedestrianLaneData.HasComponent(target))
			{
				Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
				if (m_ConnectionLaneData.TryGetComponent(target, ref connectionLane))
				{
					return (connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0;
				}
				return false;
			}
			return true;
		}

		private bool ShouldSkipTarget(Entity target, bool skipAirway, bool skipArea, ref bool airway, ref bool area)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (!m_CurveData.HasComponent(target))
			{
				return true;
			}
			Game.Net.CarLane carLane = default(Game.Net.CarLane);
			if (m_CarLaneData.TryGetComponent(target, ref carLane) && (carLane.m_Flags & Game.Net.CarLaneFlags.Runway) != 0)
			{
				airway = true;
				return skipAirway;
			}
			Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
			if (m_ConnectionLaneData.TryGetComponent(target, ref connectionLane))
			{
				if ((connectionLane.m_Flags & ConnectionLaneFlags.Airway) != 0)
				{
					airway = true;
					return skipAirway;
				}
				if ((connectionLane.m_Flags & ConnectionLaneFlags.Area) != 0)
				{
					area = true;
					return skipArea;
				}
				return true;
			}
			return false;
		}

		private float MoveCurvePosition(float3 comparePosition, float minDistance, Bezier4x3 curve)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			((float2)(ref val))._002Ector(0f, 1f);
			for (int i = 0; i < 8; i++)
			{
				float num = math.lerp(val.x, val.y, 0.5f);
				float3 val2 = MathUtils.Position(curve, num);
				if (math.distance(comparePosition, val2) < minDistance)
				{
					val.x = num;
				}
				else
				{
					val.y = num;
				}
			}
			return math.lerp(val.x, val.y, 0.5f);
		}

		private void TryAddSegments(DynamicBuffer<CurveElement> curveElements, DynamicBuffer<CurveSource> curveSources, Entity target, float2 targetDelta, Bezier4x3 curve, float nodeDistance, float segmentLength, ref float3 lastPosition, ref float3 lastTangent, ref bool airway, ref bool area, ref bool hasLastPos)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.StartTangent(curve);
			if (hasLastPos)
			{
				TryAddSegments(curveElements, curveSources, curve.a, val, nodeDistance, segmentLength, ref lastPosition, ref lastTangent, ref airway, ref area);
			}
			else
			{
				lastPosition = curve.a;
				lastTangent = val;
				hasLastPos = true;
			}
			if (math.distance(curve.a, curve.d) > 0.01f)
			{
				curveElements.Add(new CurveElement
				{
					m_Curve = curve
				});
				if (curveSources.IsCreated)
				{
					curveSources.Add(new CurveSource
					{
						m_Entity = target,
						m_Range = targetDelta
					});
				}
				lastPosition = curve.d;
				lastTangent = MathUtils.EndTangent(curve);
				airway = false;
			}
		}

		private void TryAddSegments(DynamicBuffer<CurveElement> curveElements, DynamicBuffer<CurveSource> curveSources, float3 position, float3 nextTangent, float nodeDistance, float segmentLength, ref float3 lastPosition, ref float3 lastTangent, ref bool airway, ref bool area)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_068e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			float num = math.distance(lastPosition, position);
			if (!(num > nodeDistance * 0.25f))
			{
				return;
			}
			Bezier4x3 val;
			if (airway)
			{
				lastTangent = position - lastPosition;
				nextTangent = position - lastPosition;
				lastTangent.y += num;
				nextTangent.y -= num;
				lastTangent = MathUtils.Normalize(lastTangent, ((float3)(ref lastTangent)).xz);
				nextTangent = MathUtils.Normalize(nextTangent, ((float3)(ref nextTangent)).xz);
				val = NetUtils.FitCurve(lastPosition, lastTangent, nextTangent, position);
				num = MathUtils.Length(val);
				nextTangent = default(float3);
			}
			else
			{
				bool2 val2 = default(bool2);
				((bool2)(ref val2))._002Ector(!((float3)(ref lastTangent)).Equals(default(float3)), !((float3)(ref nextTangent)).Equals(default(float3)));
				if (math.any(val2))
				{
					float3 val3 = position - lastPosition;
					bool2 val4 = bool2.op_Implicit(false);
					bool2 val5 = bool2.op_Implicit(false);
					if (math.all(val2))
					{
						lastTangent = MathUtils.Normalize(lastTangent, ((float3)(ref lastTangent)).xz);
						nextTangent = MathUtils.Normalize(nextTangent, ((float3)(ref nextTangent)).xz);
						val4.x = math.dot(lastTangent, val3) < nodeDistance * 0.2f;
						val4.y = math.dot(nextTangent, val3) < nodeDistance * 0.2f;
						val5.x = math.dot(lastTangent, val3) < nodeDistance * 0.05f;
						val5.y = math.dot(nextTangent, val3) < nodeDistance * 0.05f;
					}
					else if (val2.x)
					{
						float3 val6 = val3 / num;
						lastTangent = MathUtils.Normalize(lastTangent, ((float3)(ref lastTangent)).xz);
						nextTangent = val6 * (math.dot(lastTangent, val6) * 2f) - lastTangent;
						val4 = bool2.op_Implicit(math.dot(lastTangent, val3) < nodeDistance * 0.2f);
						val5 = bool2.op_Implicit(math.dot(lastTangent, val3) < nodeDistance * 0.05f);
					}
					else if (val2.y)
					{
						float3 val7 = val3 / num;
						nextTangent = MathUtils.Normalize(nextTangent, ((float3)(ref nextTangent)).xz);
						lastTangent = val7 * (math.dot(nextTangent, val7) * 2f) - nextTangent;
						val4 = bool2.op_Implicit(math.dot(nextTangent, val3) < nodeDistance * 0.2f);
						val5 = bool2.op_Implicit(math.dot(nextTangent, val3) < nodeDistance * 0.05f);
					}
					if (math.any(val4))
					{
						float3 val8 = lastPosition;
						float3 val9 = position;
						if (!val5.x)
						{
							num = math.dot(lastTangent, val3);
							val8 = lastPosition + lastTangent * num;
							val = NetUtils.StraightCurve(lastPosition, val8);
							int num2 = Mathf.RoundToInt(num / segmentLength);
							if (num2 > 1)
							{
								for (int i = 0; i < num2; i++)
								{
									float2 val10 = new float2((float)i, (float)(i + 1)) / (float)num2;
									curveElements.Add(new CurveElement
									{
										m_Curve = MathUtils.Cut(val, val10)
									});
									if (curveSources.IsCreated)
									{
										curveSources.Add(default(CurveSource));
									}
								}
							}
							else
							{
								curveElements.Add(new CurveElement
								{
									m_Curve = val
								});
								if (curveSources.IsCreated)
								{
									curveSources.Add(default(CurveSource));
								}
							}
						}
						if (!val5.y)
						{
							num = math.dot(nextTangent, val3);
							val9 = position - nextTangent * num;
						}
						num = math.distance(val8, val9);
						if (num >= nodeDistance * 0.5f)
						{
							val = NetUtils.StraightCurve(val8, val9);
							int num3 = Mathf.RoundToInt(num / segmentLength);
							if (num3 > 1)
							{
								for (int j = 0; j < num3; j++)
								{
									float2 val11 = new float2((float)j, (float)(j + 1)) / (float)num3;
									curveElements.Add(new CurveElement
									{
										m_Curve = MathUtils.Cut(val, val11)
									});
									if (curveSources.IsCreated)
									{
										curveSources.Add(default(CurveSource));
									}
								}
							}
							else
							{
								curveElements.Add(new CurveElement
								{
									m_Curve = val
								});
								if (curveSources.IsCreated)
								{
									curveSources.Add(default(CurveSource));
								}
							}
						}
						if (!val5.y)
						{
							num = math.dot(nextTangent, val3);
							val = NetUtils.StraightCurve(val9, position);
							int num4 = Mathf.RoundToInt(num / segmentLength);
							if (num4 > 1)
							{
								for (int k = 0; k < num4; k++)
								{
									float2 val12 = new float2((float)k, (float)(k + 1)) / (float)num4;
									curveElements.Add(new CurveElement
									{
										m_Curve = MathUtils.Cut(val, val12)
									});
									if (curveSources.IsCreated)
									{
										curveSources.Add(default(CurveSource));
									}
								}
							}
							else
							{
								curveElements.Add(new CurveElement
								{
									m_Curve = val
								});
								if (curveSources.IsCreated)
								{
									curveSources.Add(default(CurveSource));
								}
							}
						}
						lastPosition = position;
						lastTangent = nextTangent;
						airway = false;
						area = false;
						return;
					}
					val = NetUtils.FitCurve(lastPosition, lastTangent, nextTangent, position);
					num = MathUtils.Length(val);
				}
				else
				{
					val = NetUtils.StraightCurve(lastPosition, position);
					nextTangent = position - lastPosition;
				}
			}
			int num5 = Mathf.RoundToInt(num / segmentLength);
			if (num5 > 1)
			{
				for (int l = 0; l < num5; l++)
				{
					float2 val13 = new float2((float)l, (float)(l + 1)) / (float)num5;
					curveElements.Add(new CurveElement
					{
						m_Curve = MathUtils.Cut(val, val13)
					});
					if (curveSources.IsCreated)
					{
						curveSources.Add(default(CurveSource));
					}
				}
			}
			else
			{
				curveElements.Add(new CurveElement
				{
					m_Curve = val
				});
				if (curveSources.IsCreated)
				{
					curveSources.Add(default(CurveSource));
				}
			}
			lastPosition = position;
			lastTangent = nextTangent;
			airway = false;
			area = false;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathUpdated> __Game_Pathfind_PathUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<CurveElement> __Game_Routes_CurveElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Segment> __Game_Routes_Segment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportStop> __Game_Routes_TransportStop_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathTargets> __Game_Routes_PathTargets_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathSource> __Game_Routes_PathSource_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathOwner> __Game_Pathfind_PathOwner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Taxi> __Game_Vehicles_Taxi_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PersonalCar> __Game_Vehicles_PersonalCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Aircraft> __Game_Vehicles_Aircraft_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GroupMember> __Game_Creatures_GroupMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteData> __Game_Prefabs_RouteData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CarNavigationLane> __Game_Vehicles_CarNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<WatercraftNavigationLane> __Game_Vehicles_WatercraftNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AircraftNavigationLane> __Game_Vehicles_AircraftNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<TrainNavigationLane> __Game_Vehicles_TrainNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		public BufferLookup<CurveElement> __Game_Routes_CurveElement_RW_BufferLookup;

		public BufferLookup<CurveSource> __Game_Routes_CurveSource_RW_BufferLookup;

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
			__Game_Pathfind_PathUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathUpdated>(true);
			__Game_Routes_CurveElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CurveElement>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Routes_Segment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Segment>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Routes_TransportStop_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportStop>(true);
			__Game_Routes_PathTargets_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathTargets>(true);
			__Game_Routes_PathSource_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathSource>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.PedestrianLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Pathfind_PathOwner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathOwner>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Vehicles_CarCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarCurrentLane>(true);
			__Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftCurrentLane>(true);
			__Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AircraftCurrentLane>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
			__Game_Vehicles_Taxi_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Taxi>(true);
			__Game_Vehicles_PersonalCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PersonalCar>(true);
			__Game_Vehicles_Aircraft_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Aircraft>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Creatures_GroupMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GroupMember>(true);
			__Game_Creatures_HumanCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanCurrentLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_RouteData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteData>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
			__Game_Vehicles_CarNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CarNavigationLane>(true);
			__Game_Vehicles_WatercraftNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<WatercraftNavigationLane>(true);
			__Game_Vehicles_AircraftNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AircraftNavigationLane>(true);
			__Game_Vehicles_TrainNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TrainNavigationLane>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Routes_CurveElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CurveElement>(false);
			__Game_Routes_CurveSource_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CurveSource>(false);
		}
	}

	private EntityQuery m_UpdatedRoutesQuery;

	private EntityQuery m_AllRoutesQuery;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Segment>(),
			ComponentType.ReadOnly<CurveElement>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<LivePath>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Event>(),
			ComponentType.ReadOnly<PathUpdated>()
		};
		array[1] = val;
		m_UpdatedRoutesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_AllRoutesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Segment>(),
			ComponentType.ReadOnly<CurveElement>()
		});
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Loaded = true;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (GetLoaded() ? m_AllRoutesQuery : m_UpdatedRoutesQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			NativeList<Entity> val2 = default(NativeList<Entity>);
			val2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle val3 = default(JobHandle);
			NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref val)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3);
			FindUpdatedSegmentsJob findUpdatedSegmentsJob = new FindUpdatedSegmentsJob
			{
				m_Chunks = chunks,
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PathUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<PathUpdated>(ref __TypeHandle.__Game_Pathfind_PathUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurveElements = InternalCompilerInterface.GetBufferLookup<CurveElement>(ref __TypeHandle.__Game_Routes_CurveElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SegmentList = val2
			};
			UpdateSegmentCurvesJob obj = new UpdateSegmentCurvesJob
			{
				m_SegmentList = val2.AsDeferredJobArray(),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SegmentData = InternalCompilerInterface.GetComponentLookup<Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransportStopData = InternalCompilerInterface.GetComponentLookup<TransportStop>(ref __TypeHandle.__Game_Routes_TransportStop_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathTargetsData = InternalCompilerInterface.GetComponentLookup<PathTargets>(ref __TypeHandle.__Game_Routes_PathTargets_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathSourceData = InternalCompilerInterface.GetComponentLookup<PathSource>(ref __TypeHandle.__Game_Routes_PathSource_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathOwnerData = InternalCompilerInterface.GetComponentLookup<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarCurrentLaneData = InternalCompilerInterface.GetComponentLookup<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WatercraftCurrentLaneData = InternalCompilerInterface.GetComponentLookup<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AircraftCurrentLaneData = InternalCompilerInterface.GetComponentLookup<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TrainCurrentLaneData = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TaxiData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Taxi>(ref __TypeHandle.__Game_Vehicles_Taxi_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PersonalCarData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AircraftData = InternalCompilerInterface.GetComponentLookup<Aircraft>(ref __TypeHandle.__Game_Vehicles_Aircraft_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_GroupMemberData = InternalCompilerInterface.GetComponentLookup<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HumanCurrentLaneData = InternalCompilerInterface.GetComponentLookup<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRouteData = InternalCompilerInterface.GetComponentLookup<RouteData>(ref __TypeHandle.__Game_Prefabs_RouteData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarNavigationLanes = InternalCompilerInterface.GetBufferLookup<CarNavigationLane>(ref __TypeHandle.__Game_Vehicles_CarNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WatercraftNavigationLanes = InternalCompilerInterface.GetBufferLookup<WatercraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_WatercraftNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AircraftNavigationLanes = InternalCompilerInterface.GetBufferLookup<AircraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_AircraftNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TrainNavigationLanes = InternalCompilerInterface.GetBufferLookup<TrainNavigationLane>(ref __TypeHandle.__Game_Vehicles_TrainNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveElements = InternalCompilerInterface.GetBufferLookup<CurveElement>(ref __TypeHandle.__Game_Routes_CurveElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveSources = InternalCompilerInterface.GetBufferLookup<CurveSource>(ref __TypeHandle.__Game_Routes_CurveSource_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			JobHandle val4 = IJobExtensions.Schedule<FindUpdatedSegmentsJob>(findUpdatedSegmentsJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3));
			JobHandle val5 = IJobParallelForDeferExtensions.Schedule<UpdateSegmentCurvesJob, Entity>(obj, val2, 1, val4);
			val2.Dispose(val5);
			chunks.Dispose(val5);
			((SystemBase)this).Dependency = val5;
		}
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
	public SegmentCurveSystem()
	{
	}
}

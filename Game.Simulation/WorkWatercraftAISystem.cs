using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Areas;
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
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class WorkWatercraftAISystem : GameSystemBase
{
	private struct WorkAction
	{
		public VehicleWorkType m_WorkType;

		public Entity m_Target;

		public Entity m_Owner;

		public float m_WorkAmount;
	}

	[BurstCompile]
	private struct WorkWatercraftTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentRoute> m_CurrentRouteType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Watercraft> m_WatercraftType;

		public ComponentTypeHandle<WatercraftCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<Target> m_TargetType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public ComponentTypeHandle<Game.Vehicles.WorkVehicle> m_WorkVehicleType;

		public BufferTypeHandle<WatercraftNavigationLane> m_NavigationLaneType;

		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Tree> m_TreeData;

		[ReadOnly]
		public ComponentLookup<Plant> m_PlantData;

		[ReadOnly]
		public ComponentLookup<Damaged> m_DamagedData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ExtractorFacility> m_ExtractorFacilityData;

		[ReadOnly]
		public ComponentLookup<WatercraftData> m_PrefabWatercraftData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<WorkVehicleData> m_PrefabWorkVehicleData;

		[ReadOnly]
		public ComponentLookup<TreeData> m_PrefabTreeData;

		[ReadOnly]
		public ComponentLookup<WorkStopData> m_PrefabWorkStopData;

		[ReadOnly]
		public ComponentLookup<NavigationAreaData> m_PrefabNavigationAreaData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<Waypoint> m_WaypointData;

		[ReadOnly]
		public ComponentLookup<RouteLane> m_RouteLaneData;

		[ReadOnly]
		public ComponentLookup<Connected> m_ConnectedData;

		[ReadOnly]
		public ComponentLookup<BoardingVehicle> m_BoardingVehicleData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public ParallelWriter<WorkAction> m_WorkQueue;

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
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PathInformation> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
			NativeArray<CurrentRoute> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentRoute>(ref m_CurrentRouteType);
			NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<WatercraftCurrentLane> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftCurrentLane>(ref m_CurrentLaneType);
			NativeArray<Watercraft> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Watercraft>(ref m_WatercraftType);
			NativeArray<Target> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PathOwner> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<Game.Vehicles.WorkVehicle> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.WorkVehicle>(ref m_WorkVehicleType);
			BufferAccessor<WatercraftNavigationLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<WatercraftNavigationLane>(ref m_NavigationLaneType);
			BufferAccessor<PathElement> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			CurrentRoute currentRoute = default(CurrentRoute);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Owner owner = nativeArray2[i];
				PathInformation pathInformation = nativeArray3[i];
				PrefabRef prefabRef = nativeArray5[i];
				Watercraft watercraft = nativeArray7[i];
				WatercraftCurrentLane currentLane = nativeArray6[i];
				PathOwner pathOwner = nativeArray9[i];
				Target target = nativeArray8[i];
				Game.Vehicles.WorkVehicle workVehicle = nativeArray10[i];
				DynamicBuffer<WatercraftNavigationLane> navigationLanes = bufferAccessor[i];
				DynamicBuffer<PathElement> path = bufferAccessor2[i];
				CollectionUtils.TryGet<CurrentRoute>(nativeArray4, i, ref currentRoute);
				VehicleUtils.CheckUnspawned(unfilteredChunkIndex, val, currentLane, isUnspawned, m_CommandBuffer);
				Tick(unfilteredChunkIndex, val, owner, pathInformation, currentRoute, prefabRef, navigationLanes, path, ref workVehicle, ref watercraft, ref currentLane, ref pathOwner, ref target);
				nativeArray7[i] = watercraft;
				nativeArray6[i] = currentLane;
				nativeArray9[i] = pathOwner;
				nativeArray8[i] = target;
				nativeArray10[i] = workVehicle;
			}
		}

		private void Tick(int jobIndex, Entity vehicleEntity, Owner owner, PathInformation pathInformation, CurrentRoute currentRoute, PrefabRef prefabRef, DynamicBuffer<WatercraftNavigationLane> navigationLanes, DynamicBuffer<PathElement> path, ref Game.Vehicles.WorkVehicle workVehicle, ref Watercraft watercraft, ref WatercraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			if (VehicleUtils.ResetUpdatedPath(ref pathOwner) && !ResetPath(jobIndex, vehicleEntity, pathInformation, path, ref workVehicle, ref watercraft, ref currentLane, ref target, ref pathOwner))
			{
				ReturnToDepot(jobIndex, vehicleEntity, owner, ref workVehicle, ref watercraft, ref pathOwner, ref target);
				FindPathIfNeeded(vehicleEntity, owner, currentRoute, prefabRef, Entity.Null, ref workVehicle, ref watercraft, ref currentLane, ref pathOwner, ref target);
				return;
			}
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(target.m_Target) || VehicleUtils.PathfindFailed(pathOwner))
			{
				if (VehicleUtils.IsStuck(pathOwner) || (workVehicle.m_State & WorkVehicleFlags.Returning) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
					return;
				}
				ReturnToDepot(jobIndex, vehicleEntity, owner, ref workVehicle, ref watercraft, ref pathOwner, ref target);
			}
			else if (VehicleUtils.PathEndReached(currentLane))
			{
				if (IsWorkStop(currentRoute, ref target, out var workLocation))
				{
					if ((workLocation && PerformWork(jobIndex, vehicleEntity, owner, prefabRef, ref workVehicle, ref target, ref pathOwner)) || (!workLocation && ShouldStartWork(ref workVehicle)))
					{
						SetNextWaypointTarget(owner, currentRoute, ref workVehicle, ref pathOwner, ref target);
					}
				}
				else
				{
					if ((workVehicle.m_State & WorkVehicleFlags.Returning) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, vehicleEntity, default(Deleted));
						return;
					}
					if (PerformWork(jobIndex, vehicleEntity, owner, prefabRef, ref workVehicle, ref target, ref pathOwner) && !TrySetWaypointTarget(owner, currentRoute, ref workVehicle, ref pathOwner, ref target))
					{
						ReturnToDepot(jobIndex, vehicleEntity, owner, ref workVehicle, ref watercraft, ref pathOwner, ref target);
					}
				}
			}
			Entity skipWaypoint = Entity.Null;
			watercraft.m_Flags |= WatercraftFlags.Working;
			if ((workVehicle.m_State & WorkVehicleFlags.Arriving) == 0)
			{
				CheckNavigationLanes(owner, currentRoute, navigationLanes, ref workVehicle, ref currentLane, ref pathOwner, ref target, out skipWaypoint);
			}
			FindPathIfNeeded(vehicleEntity, owner, currentRoute, prefabRef, skipWaypoint, ref workVehicle, ref watercraft, ref currentLane, ref pathOwner, ref target);
		}

		private bool IsWorkStop(CurrentRoute currentRoute, ref Target target, out bool workLocation)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			workLocation = false;
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(currentRoute.m_Route))
			{
				return false;
			}
			Connected connected = default(Connected);
			PrefabRef prefabRef = default(PrefabRef);
			WorkStopData workStopData = default(WorkStopData);
			if (m_ConnectedData.TryGetComponent(target.m_Target, ref connected) && m_PrefabRefData.TryGetComponent(connected.m_Connected, ref prefabRef) && m_PrefabWorkStopData.TryGetComponent(prefabRef.m_Prefab, ref workStopData))
			{
				workLocation = workStopData.m_WorkLocation;
				return true;
			}
			return false;
		}

		private void CheckNavigationLanes(Owner owner, CurrentRoute currentRoute, DynamicBuffer<WatercraftNavigationLane> navigationLanes, ref Game.Vehicles.WorkVehicle workVehicle, ref WatercraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, out Entity skipWaypoint)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			skipWaypoint = Entity.Null;
			if (navigationLanes.Length == 0 || navigationLanes.Length == 8)
			{
				return;
			}
			WatercraftNavigationLane watercraftNavigationLane = navigationLanes[navigationLanes.Length - 1];
			if ((watercraftNavigationLane.m_Flags & WatercraftLaneFlags.EndOfPath) == 0)
			{
				return;
			}
			Connected connected = default(Connected);
			if (m_WaypointData.HasComponent(target.m_Target) && m_RouteWaypoints.HasBuffer(currentRoute.m_Route) && (!m_ConnectedData.TryGetComponent(target.m_Target, ref connected) || !m_BoardingVehicleData.HasComponent(connected.m_Connected)))
			{
				if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete)) == 0)
				{
					skipWaypoint = target.m_Target;
					SetNextWaypointTarget(owner, currentRoute, ref workVehicle, ref pathOwner, ref target);
					if ((watercraftNavigationLane.m_Flags & WatercraftLaneFlags.GroupTarget) != 0)
					{
						navigationLanes.RemoveAt(navigationLanes.Length - 1);
						return;
					}
					watercraftNavigationLane.m_Flags &= ~WatercraftLaneFlags.EndOfPath;
					navigationLanes[navigationLanes.Length - 1] = watercraftNavigationLane;
				}
				return;
			}
			workVehicle.m_State |= WorkVehicleFlags.Arriving;
			RouteLane routeLane = default(RouteLane);
			if (!m_RouteLaneData.TryGetComponent(target.m_Target, ref routeLane))
			{
				return;
			}
			if (routeLane.m_StartLane != routeLane.m_EndLane)
			{
				watercraftNavigationLane.m_CurvePosition.y = 1f;
				WatercraftNavigationLane watercraftNavigationLane2 = new WatercraftNavigationLane
				{
					m_Lane = watercraftNavigationLane.m_Lane
				};
				if (FindNextLane(ref watercraftNavigationLane2.m_Lane))
				{
					watercraftNavigationLane.m_Flags &= ~WatercraftLaneFlags.EndOfPath;
					navigationLanes[navigationLanes.Length - 1] = watercraftNavigationLane;
					watercraftNavigationLane2.m_Flags |= WatercraftLaneFlags.EndOfPath | WatercraftLaneFlags.FixedLane;
					watercraftNavigationLane2.m_CurvePosition = new float2(0f, routeLane.m_EndCurvePos);
					navigationLanes.Add(watercraftNavigationLane2);
				}
				else
				{
					navigationLanes[navigationLanes.Length - 1] = watercraftNavigationLane;
				}
			}
			else
			{
				watercraftNavigationLane.m_CurvePosition.y = routeLane.m_EndCurvePos;
				navigationLanes[navigationLanes.Length - 1] = watercraftNavigationLane;
			}
		}

		private bool FindNextLane(ref Entity lane)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			if (!m_OwnerData.HasComponent(lane) || !m_LaneData.HasComponent(lane))
			{
				return false;
			}
			Owner owner = m_OwnerData[lane];
			Lane lane2 = m_LaneData[lane];
			if (!m_SubLanes.HasBuffer(owner.m_Owner))
			{
				return false;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[owner.m_Owner];
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				Lane lane3 = m_LaneData[subLane];
				if (lane2.m_EndNode.Equals(lane3.m_StartNode))
				{
					lane = subLane;
					return true;
				}
			}
			return false;
		}

		private void FindPathIfNeeded(Entity vehicleEntity, Owner owner, CurrentRoute currentRoute, PrefabRef prefabRef, Entity skipWaypoint, ref Game.Vehicles.WorkVehicle workVehicle, ref Watercraft watercraft, ref WatercraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			if (!VehicleUtils.RequireNewPath(pathOwner))
			{
				return;
			}
			WatercraftData watercraftData = m_PrefabWatercraftData[prefabRef.m_Prefab];
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(watercraftData.m_MaxSpeed),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_Methods = (VehicleUtils.GetPathMethods(watercraftData) | PathMethod.Offroad),
				m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = (VehicleUtils.GetPathMethods(watercraftData) | PathMethod.Offroad),
				m_RoadTypes = RoadTypes.Watercraft
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = (VehicleUtils.GetPathMethods(watercraftData) | PathMethod.Offroad),
				m_RoadTypes = RoadTypes.Watercraft,
				m_Entity = target.m_Target
			};
			if (skipWaypoint != Entity.Null)
			{
				origin.m_Entity = skipWaypoint;
				pathOwner.m_State |= PathFlags.Append;
			}
			else
			{
				pathOwner.m_State &= ~PathFlags.Append;
			}
			WorkVehicleData workVehicleData = m_PrefabWorkVehicleData[prefabRef.m_Prefab];
			if (((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(currentRoute.m_Route))
			{
				if ((workVehicle.m_State & WorkVehicleFlags.Returning) != 0)
				{
					workVehicle.m_State &= ~WorkVehicleFlags.RouteSource;
				}
				else
				{
					if ((workVehicle.m_State & WorkVehicleFlags.RouteSource) != 0)
					{
						parameters.m_PathfindFlags |= PathfindFlags.Stable | PathfindFlags.IgnoreFlow;
					}
					if ((workVehicle.m_State & WorkVehicleFlags.WorkLocation) != 0)
					{
						workVehicle.m_State &= ~WorkVehicleFlags.RouteSource;
						destination.m_Type = SetupTargetType.AreaLocation;
						destination.m_Entity = owner.m_Owner;
						destination.m_Value = (int)workVehicleData.m_WorkType;
						target.m_Target = owner.m_Owner;
					}
				}
			}
			else
			{
				if ((workVehicle.m_State & (WorkVehicleFlags.Returning | WorkVehicleFlags.ExtractorVehicle)) == WorkVehicleFlags.ExtractorVehicle)
				{
					if (workVehicleData.m_MapFeature == MapFeature.Forest)
					{
						destination.m_Type = SetupTargetType.WoodResource;
					}
					else
					{
						destination.m_Type = SetupTargetType.AreaLocation;
					}
					destination.m_Entity = owner.m_Owner;
					destination.m_Value = (int)workVehicleData.m_WorkType;
					target.m_Target = owner.m_Owner;
				}
				else if ((workVehicle.m_State & (WorkVehicleFlags.Returning | WorkVehicleFlags.StorageVehicle)) == WorkVehicleFlags.StorageVehicle)
				{
					destination.m_Type = SetupTargetType.AreaLocation;
					destination.m_Entity = owner.m_Owner;
					destination.m_Value = (int)workVehicleData.m_WorkType;
					target.m_Target = owner.m_Owner;
				}
				workVehicle.m_State &= ~WorkVehicleFlags.RouteSource;
			}
			VehicleUtils.SetupPathfind(item: new SetupQueueItem(vehicleEntity, parameters, origin, destination), currentLane: ref currentLane, pathOwner: ref pathOwner, queue: m_PathfindQueue);
		}

		private bool TrySetWaypointTarget(Owner owner, CurrentRoute currentRoute, ref Game.Vehicles.WorkVehicle workVehicle, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			workVehicle.m_State &= ~WorkVehicleFlags.WorkLocation;
			DynamicBuffer<RouteWaypoint> val = default(DynamicBuffer<RouteWaypoint>);
			Owner owner2 = default(Owner);
			if (m_RouteWaypoints.TryGetBuffer(currentRoute.m_Route, ref val) && m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2))
			{
				Connected connected = default(Connected);
				Owner owner3 = default(Owner);
				for (int i = 0; i < val.Length; i++)
				{
					RouteWaypoint routeWaypoint = val[i];
					if (m_ConnectedData.TryGetComponent(routeWaypoint.m_Waypoint, ref connected) && m_OwnerData.TryGetComponent(connected.m_Connected, ref owner3) && owner3.m_Owner == owner2.m_Owner)
					{
						target.m_Target = routeWaypoint.m_Waypoint;
						SetNextWaypointTarget(owner, currentRoute, ref workVehicle, ref pathOwner, ref target);
						return true;
					}
				}
			}
			return false;
		}

		private void SetNextWaypointTarget(Owner owner, CurrentRoute currentRoute, ref Game.Vehicles.WorkVehicle workVehicle, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<RouteWaypoint> val = m_RouteWaypoints[currentRoute.m_Route];
			int num = m_WaypointData[target.m_Target].m_Index + 1;
			num = math.select(num, 0, num >= val.Length);
			Entity waypoint = val[num].m_Waypoint;
			Connected connected = default(Connected);
			Owner owner2 = default(Owner);
			Owner owner3 = default(Owner);
			PrefabRef prefabRef = default(PrefabRef);
			WorkStopData workStopData = default(WorkStopData);
			if (m_ConnectedData.TryGetComponent(waypoint, ref connected) && m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2) && m_OwnerData.TryGetComponent(connected.m_Connected, ref owner3) && m_PrefabRefData.TryGetComponent(connected.m_Connected, ref prefabRef) && m_PrefabWorkStopData.TryGetComponent(prefabRef.m_Prefab, ref workStopData) && HasNavigation(owner.m_Owner, RoadTypes.Watercraft) && owner3.m_Owner == owner2.m_Owner && workStopData.m_WorkLocation)
			{
				VehicleUtils.SetTarget(ref pathOwner, ref target, owner.m_Owner);
				workVehicle.m_State |= WorkVehicleFlags.RouteSource | WorkVehicleFlags.WorkLocation;
			}
			else
			{
				VehicleUtils.SetTarget(ref pathOwner, ref target, val[num].m_Waypoint);
				workVehicle.m_State |= WorkVehicleFlags.RouteSource;
			}
		}

		private bool HasNavigation(Entity entity, RoadTypes roadTypes)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (HasNavigationSelf(entity, roadTypes))
			{
				return true;
			}
			DynamicBuffer<Game.Areas.SubArea> val = default(DynamicBuffer<Game.Areas.SubArea>);
			if (m_SubAreas.TryGetBuffer(entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (HasNavigationSelf(val[i].m_Area, roadTypes))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasNavigationSelf(Entity entity, RoadTypes roadTypes)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = default(PrefabRef);
			NavigationAreaData navigationAreaData = default(NavigationAreaData);
			if (m_PrefabRefData.TryGetComponent(entity, ref prefabRef) && m_PrefabNavigationAreaData.TryGetComponent(prefabRef.m_Prefab, ref navigationAreaData))
			{
				return (navigationAreaData.m_RoadTypes & roadTypes) != 0;
			}
			return false;
		}

		private bool ResetPath(int jobIndex, Entity vehicleEntity, PathInformation pathInformation, DynamicBuffer<PathElement> path, ref Game.Vehicles.WorkVehicle workVehicle, ref Watercraft watercraft, ref WatercraftCurrentLane currentLane, ref Target target, ref PathOwner pathOwner)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			workVehicle.m_State &= ~WorkVehicleFlags.Arriving;
			if ((pathOwner.m_State & PathFlags.Append) == 0)
			{
				PathUtils.ResetPath(ref currentLane, path, m_SlaveLaneData, m_OwnerData, m_SubLanes);
			}
			if ((workVehicle.m_State & WorkVehicleFlags.Returning) != 0)
			{
				watercraft.m_Flags &= ~WatercraftFlags.StayOnWaterway;
			}
			else
			{
				watercraft.m_Flags |= WatercraftFlags.StayOnWaterway;
				target.m_Target = pathInformation.m_Destination;
			}
			return true;
		}

		private void ReturnToDepot(int jobIndex, Entity vehicleEntity, Owner ownerData, ref Game.Vehicles.WorkVehicle workVehicle, ref Watercraft watercraft, ref PathOwner pathOwner, ref Target target)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			workVehicle.m_State &= ~WorkVehicleFlags.WorkLocation;
			workVehicle.m_State |= WorkVehicleFlags.Returning;
			Entity newTarget = ownerData.m_Owner;
			if (m_OwnerData.HasComponent(ownerData.m_Owner))
			{
				Owner owner = m_OwnerData[ownerData.m_Owner];
				newTarget = ((!m_AttachmentData.HasComponent(owner.m_Owner)) ? owner.m_Owner : m_AttachmentData[owner.m_Owner].m_Attached);
			}
			VehicleUtils.SetTarget(ref pathOwner, ref target, newTarget);
		}

		private bool ShouldStartWork(ref Game.Vehicles.WorkVehicle workVehicle)
		{
			float num = math.min(workVehicle.m_WorkAmount, workVehicle.m_DoneAmount);
			workVehicle.m_WorkAmount -= num;
			workVehicle.m_DoneAmount -= num;
			return workVehicle.m_DoneAmount < workVehicle.m_WorkAmount;
		}

		private bool PerformWork(int jobIndex, Entity vehicleEntity, Owner owner, PrefabRef prefabRef, ref Game.Vehicles.WorkVehicle workVehicle, ref Target target, ref PathOwner pathOwner)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			WorkVehicleData workVehicleData = m_PrefabWorkVehicleData[prefabRef.m_Prefab];
			float num = workVehicleData.m_MaxWorkAmount;
			if ((workVehicle.m_State & WorkVehicleFlags.ExtractorVehicle) != 0)
			{
				switch (workVehicleData.m_WorkType)
				{
				case VehicleWorkType.Harvest:
					num = 1000f;
					if (m_TreeData.HasComponent(target.m_Target))
					{
						Tree tree = m_TreeData[target.m_Target];
						Plant plant = m_PlantData[target.m_Target];
						PrefabRef prefabRef2 = m_PrefabRefData[target.m_Target];
						Damaged damaged = default(Damaged);
						m_DamagedData.TryGetComponent(target.m_Target, ref damaged);
						TreeData treeData = default(TreeData);
						if (m_PrefabTreeData.TryGetComponent(prefabRef2.m_Prefab, ref treeData))
						{
							num = ObjectUtils.CalculateWoodAmount(tree, plant, damaged, treeData);
						}
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, target.m_Target, default(BatchesUpdated));
					}
					else if (m_ExtractorFacilityData.HasComponent(target.m_Target))
					{
						num = workVehicleData.m_MaxWorkAmount * 0.5f;
					}
					m_WorkQueue.Enqueue(new WorkAction
					{
						m_WorkType = workVehicleData.m_WorkType,
						m_Target = target.m_Target,
						m_Owner = owner.m_Owner,
						m_WorkAmount = num
					});
					break;
				case VehicleWorkType.Collect:
					if (m_TreeData.HasComponent(target.m_Target))
					{
						m_WorkQueue.Enqueue(new WorkAction
						{
							m_WorkType = workVehicleData.m_WorkType,
							m_Target = target.m_Target
						});
					}
					num = workVehicleData.m_MaxWorkAmount * 0.25f;
					break;
				}
			}
			else if ((workVehicle.m_State & WorkVehicleFlags.StorageVehicle) != 0)
			{
				num = workVehicleData.m_MaxWorkAmount * 0.25f;
			}
			if ((workVehicle.m_State & WorkVehicleFlags.WorkLocation) != 0)
			{
				VehicleUtils.SetTarget(ref pathOwner, ref target, Entity.Null);
			}
			QuantityUpdated(jobIndex, vehicleEntity);
			workVehicle.m_DoneAmount += num;
			return workVehicle.m_DoneAmount > workVehicle.m_WorkAmount - 1f;
		}

		private void QuantityUpdated(int jobIndex, Entity vehicleEntity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (m_SubObjects.HasBuffer(vehicleEntity))
			{
				DynamicBuffer<Game.Objects.SubObject> val = m_SubObjects[vehicleEntity];
				for (int i = 0; i < val.Length; i++)
				{
					Entity subObject = val[i].m_SubObject;
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, subObject, default(BatchesUpdated));
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct WorkWatercraftWorkJob : IJob
	{
		public ComponentLookup<Tree> m_TreeData;

		public ComponentLookup<Extractor> m_ExtractorData;

		public NativeQueue<WorkAction> m_WorkQueue;

		public void Execute()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			int count = m_WorkQueue.Count;
			for (int i = 0; i < count; i++)
			{
				WorkAction workAction = m_WorkQueue.Dequeue();
				switch (workAction.m_WorkType)
				{
				case VehicleWorkType.Harvest:
				{
					float num = 0f;
					if (m_TreeData.HasComponent(workAction.m_Target))
					{
						Tree tree2 = m_TreeData[workAction.m_Target];
						if ((tree2.m_State & TreeState.Stump) == 0)
						{
							tree2.m_State &= ~(TreeState.Teen | TreeState.Adult | TreeState.Elderly | TreeState.Dead | TreeState.Collected);
							tree2.m_State |= TreeState.Stump;
							tree2.m_Growth = 0;
							m_TreeData[workAction.m_Target] = tree2;
							num = workAction.m_WorkAmount;
						}
					}
					if (m_ExtractorData.HasComponent(workAction.m_Owner))
					{
						Extractor extractor = m_ExtractorData[workAction.m_Owner];
						extractor.m_ExtractedAmount -= num;
						extractor.m_HarvestedAmount += workAction.m_WorkAmount;
						m_ExtractorData[workAction.m_Owner] = extractor;
					}
					break;
				}
				case VehicleWorkType.Collect:
					if (m_TreeData.HasComponent(workAction.m_Target))
					{
						Tree tree = m_TreeData[workAction.m_Target];
						if ((tree.m_State & TreeState.Collected) == 0)
						{
							tree.m_State |= TreeState.Collected;
							m_TreeData[workAction.m_Target] = tree;
						}
					}
					break;
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
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentRoute> __Game_Routes_CurrentRoute_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Watercraft> __Game_Vehicles_Watercraft_RW_ComponentTypeHandle;

		public ComponentTypeHandle<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Vehicles.WorkVehicle> __Game_Vehicles_WorkVehicle_RW_ComponentTypeHandle;

		public BufferTypeHandle<WatercraftNavigationLane> __Game_Vehicles_WatercraftNavigationLane_RW_BufferTypeHandle;

		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Tree> __Game_Objects_Tree_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Plant> __Game_Objects_Plant_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Damaged> __Game_Objects_Damaged_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ExtractorFacility> __Game_Buildings_ExtractorFacility_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WatercraftData> __Game_Prefabs_WatercraftData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkVehicleData> __Game_Prefabs_WorkVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TreeData> __Game_Prefabs_TreeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkStopData> __Game_Prefabs_WorkStopData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NavigationAreaData> __Game_Prefabs_NavigationAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Waypoint> __Game_Routes_Waypoint_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteLane> __Game_Routes_RouteLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BoardingVehicle> __Game_Routes_BoardingVehicle_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		public ComponentLookup<Tree> __Game_Objects_Tree_RW_ComponentLookup;

		public ComponentLookup<Extractor> __Game_Areas_Extractor_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Routes_CurrentRoute_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentRoute>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Vehicles_Watercraft_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Watercraft>(false);
			__Game_Vehicles_WatercraftCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WatercraftCurrentLane>(false);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Vehicles_WorkVehicle_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.WorkVehicle>(false);
			__Game_Vehicles_WatercraftNavigationLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<WatercraftNavigationLane>(false);
			__Game_Pathfind_PathElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Objects_Tree_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(true);
			__Game_Objects_Plant_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Plant>(true);
			__Game_Objects_Damaged_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(true);
			__Game_Buildings_ExtractorFacility_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ExtractorFacility>(true);
			__Game_Prefabs_WatercraftData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_WorkVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkVehicleData>(true);
			__Game_Prefabs_TreeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TreeData>(true);
			__Game_Prefabs_WorkStopData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkStopData>(true);
			__Game_Prefabs_NavigationAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NavigationAreaData>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Routes_Waypoint_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Waypoint>(true);
			__Game_Routes_RouteLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteLane>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Routes_BoardingVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BoardingVehicle>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Objects_Tree_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(false);
			__Game_Areas_Extractor_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Extractor>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private EntityQuery m_VehicleQuery;

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
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[10]
		{
			ComponentType.ReadWrite<Game.Vehicles.WorkVehicle>(),
			ComponentType.ReadWrite<WatercraftCurrentLane>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>(),
			ComponentType.Exclude<OutOfControl>()
		});
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
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<WorkAction> workQueue = default(NativeQueue<WorkAction>);
		workQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		WorkWatercraftTickJob workWatercraftTickJob = new WorkWatercraftTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentRouteType = InternalCompilerInterface.GetComponentTypeHandle<CurrentRoute>(ref __TypeHandle.__Game_Routes_CurrentRoute_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftType = InternalCompilerInterface.GetComponentTypeHandle<Watercraft>(ref __TypeHandle.__Game_Vehicles_Watercraft_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WorkVehicleType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.WorkVehicle>(ref __TypeHandle.__Game_Vehicles_WorkVehicle_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<WatercraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_WatercraftNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlantData = InternalCompilerInterface.GetComponentLookup<Plant>(ref __TypeHandle.__Game_Objects_Plant_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorFacilityData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ExtractorFacility>(ref __TypeHandle.__Game_Buildings_ExtractorFacility_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWatercraftData = InternalCompilerInterface.GetComponentLookup<WatercraftData>(ref __TypeHandle.__Game_Prefabs_WatercraftData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWorkVehicleData = InternalCompilerInterface.GetComponentLookup<WorkVehicleData>(ref __TypeHandle.__Game_Prefabs_WorkVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTreeData = InternalCompilerInterface.GetComponentLookup<TreeData>(ref __TypeHandle.__Game_Prefabs_TreeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWorkStopData = InternalCompilerInterface.GetComponentLookup<WorkStopData>(ref __TypeHandle.__Game_Prefabs_WorkStopData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNavigationAreaData = InternalCompilerInterface.GetComponentLookup<NavigationAreaData>(ref __TypeHandle.__Game_Prefabs_NavigationAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaypointData = InternalCompilerInterface.GetComponentLookup<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteLaneData = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoardingVehicleData = InternalCompilerInterface.GetComponentLookup<BoardingVehicle>(ref __TypeHandle.__Game_Routes_BoardingVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		workWatercraftTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		workWatercraftTickJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		workWatercraftTickJob.m_WorkQueue = workQueue.AsParallelWriter();
		WorkWatercraftTickJob workWatercraftTickJob2 = workWatercraftTickJob;
		WorkWatercraftWorkJob obj = new WorkWatercraftWorkJob
		{
			m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorData = InternalCompilerInterface.GetComponentLookup<Extractor>(ref __TypeHandle.__Game_Areas_Extractor_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkQueue = workQueue
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<WorkWatercraftTickJob>(workWatercraftTickJob2, m_VehicleQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = IJobExtensions.Schedule<WorkWatercraftWorkJob>(obj, val2);
		workQueue.Dispose(val3);
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
	public WorkWatercraftAISystem()
	{
	}
}

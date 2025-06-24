using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Notifications;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Routes;

[CompilerGenerated]
public class RoutePathReadySystem : GameSystemBase
{
	[BurstCompile]
	private struct RoutePathReadyJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PathUpdated> m_PathUpdatedType;

		[ReadOnly]
		public BufferTypeHandle<RouteWaypoint> m_RouteWaypointType;

		[ReadOnly]
		public BufferTypeHandle<RouteSegment> m_RouteSegmentType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<Segment> m_SegmentData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		[ReadOnly]
		public BufferLookup<RouteSegment> m_RouteSegments;

		public ComponentLookup<PathTargets> m_PathTargetsData;

		[ReadOnly]
		public RouteConfigurationData m_RouteConfigurationData;

		public IconCommandBuffer m_IconCommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PathUpdated> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathUpdated>(ref m_PathUpdatedType);
			if (nativeArray.Length != 0)
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					PathUpdated pathUpdated = nativeArray[i];
					if (m_PathTargetsData.HasComponent(pathUpdated.m_Owner))
					{
						PathTargets pathTargets = m_PathTargetsData[pathUpdated.m_Owner];
						pathTargets.m_ReadyStartPosition = pathUpdated.m_Data.m_Position1;
						pathTargets.m_ReadyEndPosition = pathUpdated.m_Data.m_Position2;
						m_PathTargetsData[pathUpdated.m_Owner] = pathTargets;
					}
					if (m_PathInformationData.HasComponent(pathUpdated.m_Owner) && !m_TempData.HasComponent(pathUpdated.m_Owner))
					{
						UpdatePathfindNotifications(pathUpdated.m_Owner);
					}
				}
				return;
			}
			BufferAccessor<RouteWaypoint> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RouteWaypoint>(ref m_RouteWaypointType);
			BufferAccessor<RouteSegment> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RouteSegment>(ref m_RouteSegmentType);
			for (int j = 0; j < bufferAccessor.Length; j++)
			{
				DynamicBuffer<RouteWaypoint> val = bufferAccessor[j];
				DynamicBuffer<RouteSegment> val2 = bufferAccessor2[j];
				for (int k = 0; k < val2.Length; k++)
				{
					RouteSegment routeSegment = val2[k];
					if (m_PathTargetsData.HasComponent(routeSegment.m_Segment))
					{
						RouteWaypoint routeWaypoint = val[k];
						RouteWaypoint routeWaypoint2 = val[math.select(k + 1, 0, k + 1 >= val.Length)];
						PathTargets pathTargets2 = m_PathTargetsData[routeSegment.m_Segment];
						if (m_PositionData.HasComponent(routeWaypoint.m_Waypoint))
						{
							pathTargets2.m_ReadyStartPosition = m_PositionData[routeWaypoint.m_Waypoint].m_Position;
						}
						if (m_PositionData.HasComponent(routeWaypoint2.m_Waypoint))
						{
							pathTargets2.m_ReadyEndPosition = m_PositionData[routeWaypoint2.m_Waypoint].m_Position;
						}
						m_PathTargetsData[routeSegment.m_Segment] = pathTargets2;
					}
				}
			}
		}

		private void UpdatePathfindNotifications(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			if (m_SegmentData.HasComponent(entity) && m_OwnerData.HasComponent(entity))
			{
				Segment segment = m_SegmentData[entity];
				Owner owner = m_OwnerData[entity];
				DynamicBuffer<RouteWaypoint> waypoints = m_RouteWaypoints[owner.m_Owner];
				DynamicBuffer<RouteSegment> segments = m_RouteSegments[owner.m_Owner];
				int index = segment.m_Index;
				int waypointIndex = math.select(segment.m_Index + 1, 0, segment.m_Index == waypoints.Length - 1);
				UpdatePathfindNotification(waypoints, segments, index);
				UpdatePathfindNotification(waypoints, segments, waypointIndex);
			}
		}

		private void UpdatePathfindNotification(DynamicBuffer<RouteWaypoint> waypoints, DynamicBuffer<RouteSegment> segments, int waypointIndex)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			int num = math.select(waypointIndex - 1, waypoints.Length - 1, waypointIndex == 0);
			bool flag = false;
			PathInformation pathInformation = default(PathInformation);
			if (num < segments.Length && m_PathInformationData.TryGetComponent(segments[num].m_Segment, ref pathInformation))
			{
				flag |= pathInformation.m_Distance < 0f;
			}
			PathInformation pathInformation2 = default(PathInformation);
			if (waypointIndex < segments.Length && m_PathInformationData.TryGetComponent(segments[waypointIndex].m_Segment, ref pathInformation2))
			{
				flag |= pathInformation2.m_Distance < 0f;
			}
			if (flag)
			{
				m_IconCommandBuffer.Add(waypoints[waypointIndex].m_Waypoint, m_RouteConfigurationData.m_PathfindNotification, IconPriority.Warning);
			}
			else
			{
				m_IconCommandBuffer.Remove(waypoints[waypointIndex].m_Waypoint, m_RouteConfigurationData.m_PathfindNotification);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PathUpdated> __Game_Pathfind_PathUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<RouteSegment> __Game_Routes_RouteSegment_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Segment> __Game_Routes_Segment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteSegment> __Game_Routes_RouteSegment_RO_BufferLookup;

		public ComponentLookup<PathTargets> __Game_Routes_PathTargets_RW_ComponentLookup;

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
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			__Game_Pathfind_PathUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathUpdated>(true);
			__Game_Routes_RouteWaypoint_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteWaypoint>(true);
			__Game_Routes_RouteSegment_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteSegment>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Routes_Segment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Segment>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Routes_RouteSegment_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteSegment>(true);
			__Game_Routes_PathTargets_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathTargets>(false);
		}
	}

	private IconCommandSystem m_IconCommandSystem;

	private EntityQuery m_PathReadyQuery;

	private EntityQuery m_RouteQuery;

	private EntityQuery m_RouteConfigQuery;

	private bool m_Loaded;

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
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_PathReadyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Event>(),
			ComponentType.ReadOnly<PathUpdated>()
		});
		m_RouteQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Route>() });
		m_RouteConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<RouteConfigurationData>() });
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
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (GetLoaded() ? m_RouteQuery : m_PathReadyQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			JobHandle val2 = JobChunkExtensions.Schedule<RoutePathReadyJob>(new RoutePathReadyJob
			{
				m_PathUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<PathUpdated>(ref __TypeHandle.__Game_Pathfind_PathUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RouteWaypointType = InternalCompilerInterface.GetBufferTypeHandle<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RouteSegmentType = InternalCompilerInterface.GetBufferTypeHandle<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SegmentData = InternalCompilerInterface.GetComponentLookup<Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteSegments = InternalCompilerInterface.GetBufferLookup<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathTargetsData = InternalCompilerInterface.GetComponentLookup<PathTargets>(ref __TypeHandle.__Game_Routes_PathTargets_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteConfigurationData = ((EntityQuery)(ref m_RouteConfigQuery)).GetSingleton<RouteConfigurationData>(),
				m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer()
			}, val, ((SystemBase)this).Dependency);
			m_IconCommandSystem.AddCommandBufferWriter(val2);
			((SystemBase)this).Dependency = val2;
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
	public RoutePathReadySystem()
	{
	}
}

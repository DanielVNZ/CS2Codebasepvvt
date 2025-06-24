using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
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
public class WaypointConnectionSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateWaypointReferencesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Connected> m_ConnectedType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<AccessLane> m_AccessLaneType;

		[ReadOnly]
		public ComponentTypeHandle<RouteLane> m_RouteLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentLookup<Connected> m_ConnectedData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		public BufferLookup<ConnectedRoute> m_ConnectedRoutes;

		public NativeList<Entity> m_UpdatedList;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Connected> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Connected>(ref m_ConnectedType);
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				if (((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType))
				{
					return;
				}
				if (nativeArray2.Length != 0)
				{
					for (int i = 0; i < nativeArray2.Length; i++)
					{
						Entity waypoint = nativeArray[i];
						Connected connected = nativeArray2[i];
						if (m_ConnectedRoutes.HasBuffer(connected.m_Connected))
						{
							CollectionUtils.RemoveValue<ConnectedRoute>(m_ConnectedRoutes[connected.m_Connected], new ConnectedRoute(waypoint));
						}
					}
					return;
				}
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					if (!m_ConnectedRoutes.HasBuffer(val))
					{
						continue;
					}
					DynamicBuffer<ConnectedRoute> val2 = m_ConnectedRoutes[val];
					for (int k = 0; k < val2.Length; k++)
					{
						Entity waypoint2 = val2[k].m_Waypoint;
						if (m_ConnectedData.HasComponent(waypoint2) && !m_DeletedData.HasComponent(waypoint2))
						{
							m_UpdatedList.Add(ref waypoint2);
						}
					}
				}
				return;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType) && !((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType))
			{
				for (int l = 0; l < nativeArray2.Length; l++)
				{
					Entity waypoint3 = nativeArray[l];
					Connected connected2 = nativeArray2[l];
					if (m_ConnectedRoutes.HasBuffer(connected2.m_Connected))
					{
						m_ConnectedRoutes[connected2.m_Connected].Add(new ConnectedRoute(waypoint3));
					}
				}
			}
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<AccessLane>(ref m_AccessLaneType) || ((ArchetypeChunk)(ref chunk)).Has<RouteLane>(ref m_RouteLaneType);
			for (int m = 0; m < nativeArray.Length; m++)
			{
				Entity val3 = nativeArray[m];
				if (m_ConnectedRoutes.HasBuffer(val3))
				{
					DynamicBuffer<ConnectedRoute> val4 = m_ConnectedRoutes[val3];
					for (int n = 0; n < val4.Length; n++)
					{
						Entity waypoint4 = val4[n].m_Waypoint;
						if (m_ConnectedData.HasComponent(waypoint4) && !m_DeletedData.HasComponent(waypoint4))
						{
							m_UpdatedList.Add(ref waypoint4);
						}
					}
				}
				if (flag)
				{
					ref NativeList<Entity> reference = ref m_UpdatedList;
					Entity val5 = nativeArray[m];
					reference.Add(ref val5);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FindUpdatedWaypointsJob : IJobParallelForDefer
	{
		private struct RouteIterator : INativeQuadTreeIterator<RouteSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<RouteSearchItem, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<Waypoint> m_WaypointData;

			public ParallelWriter<Entity> m_ResultQueue;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, RouteSearchItem item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) && m_WaypointData.HasComponent(item.m_Entity))
				{
					m_ResultQueue.Enqueue(item.m_Entity);
				}
			}
		}

		private struct ObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<AccessLane> m_AccessLaneData;

			public ComponentLookup<RouteLane> m_RouteLaneData;

			public ParallelWriter<Entity> m_ResultQueue;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) && (m_AccessLaneData.HasComponent(item) || m_RouteLaneData.HasComponent(item)))
				{
					m_ResultQueue.Enqueue(item);
				}
			}
		}

		[ReadOnly]
		public NativeArray<Bounds2> m_Bounds;

		[ReadOnly]
		public NativeQuadTree<RouteSearchItem, QuadTreeBoundsXZ> m_RouteSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		[ReadOnly]
		public ComponentLookup<Waypoint> m_WaypointData;

		[ReadOnly]
		public ComponentLookup<AccessLane> m_AccessLaneData;

		[ReadOnly]
		public ComponentLookup<RouteLane> m_RouteLaneData;

		public ParallelWriter<Entity> m_ResultQueue;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			Bounds2 bounds = MathUtils.Expand(m_Bounds[index], float2.op_Implicit(10f));
			RouteIterator routeIterator = new RouteIterator
			{
				m_Bounds = bounds,
				m_WaypointData = m_WaypointData,
				m_ResultQueue = m_ResultQueue
			};
			m_RouteSearchTree.Iterate<RouteIterator>(ref routeIterator, 0);
			ObjectIterator objectIterator = new ObjectIterator
			{
				m_Bounds = bounds,
				m_AccessLaneData = m_AccessLaneData,
				m_RouteLaneData = m_RouteLaneData,
				m_ResultQueue = m_ResultQueue
			};
			m_ObjectSearchTree.Iterate<ObjectIterator>(ref objectIterator, 0);
		}
	}

	[BurstCompile]
	private struct DequeUpdatedWaypointsJob : IJob
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct EntityComparer : IComparer<Entity>
		{
			public int Compare(Entity x, Entity y)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return x.Index - y.Index;
			}
		}

		public NativeQueue<Entity> m_UpdatedQueue;

		public NativeList<Entity> m_UpdatedList;

		public void Execute()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			Entity val = default(Entity);
			while (m_UpdatedQueue.TryDequeue(ref val))
			{
				m_UpdatedList.Add(ref val);
			}
			NativeSortExtension.Sort<Entity, EntityComparer>(m_UpdatedList, default(EntityComparer));
			Entity val2 = Entity.Null;
			int num = 0;
			int num2 = 0;
			while (num < m_UpdatedList.Length)
			{
				val = m_UpdatedList[num++];
				if (val != val2)
				{
					m_UpdatedList[num2++] = val;
					val2 = val;
				}
			}
			if (num2 < m_UpdatedList.Length)
			{
				m_UpdatedList.RemoveRange(num2, m_UpdatedList.Length - num2);
			}
		}
	}

	[BurstCompile]
	private struct RemoveDuplicatedWaypointsJob : IJob
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct EntityComparer : IComparer<Entity>
		{
			public int Compare(Entity x, Entity y)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return x.Index - y.Index;
			}
		}

		public NativeList<Entity> m_UpdatedList;

		public void Execute()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			if (m_UpdatedList.Length < 2)
			{
				return;
			}
			NativeSortExtension.Sort<Entity, EntityComparer>(m_UpdatedList, default(EntityComparer));
			Entity val = Entity.Null;
			int num = 0;
			int num2 = 0;
			while (num < m_UpdatedList.Length)
			{
				Entity val2 = m_UpdatedList[num++];
				if (val2 != val)
				{
					m_UpdatedList[num2++] = val2;
					val = val2;
				}
			}
			if (num2 < m_UpdatedList.Length)
			{
				m_UpdatedList.RemoveRange(num2, m_UpdatedList.Length - num2);
			}
		}
	}

	[BurstCompile]
	private struct FindWaypointConnectionsJob : IJobParallelForDefer
	{
		private struct SurfaceIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public float3 m_Position;

			public int m_JobIndex;

			public ComponentLookup<Game.Areas.Surface> m_SurfaceData;

			public BufferLookup<Game.Areas.Node> m_AreaNodes;

			public BufferLookup<Triangle> m_AreaTriangles;

			public ParallelWriter m_CommandBuffer;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((float3)(ref m_Position)).xz);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((float3)(ref m_Position)).xz) && m_SurfaceData.HasComponent(item.m_Area))
				{
					DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[item.m_Area];
					Triangle triangle = m_AreaTriangles[item.m_Area][item.m_Triangle];
					if (MathUtils.Intersect(AreaUtils.GetTriangle2(nodes, triangle), ((float3)(ref m_Position)).xz))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(m_JobIndex, item.m_Area, default(Updated));
					}
				}
			}
		}

		private struct LaneIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public float3 m_Position;

			public RouteConnectionType m_ConnectionType;

			public TrackTypes m_TrackType;

			public RoadTypes m_CarType;

			public bool m_OnGround;

			public float m_CmpDistance;

			public float m_MaxDistance;

			public float m_Elevation;

			public Quad2 m_LotQuad;

			public bool4 m_CheckLot;

			public Entity m_IgnoreOwner;

			public Entity m_IgnoreConnected;

			public Entity m_MasterLot;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

			public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

			public ComponentLookup<Game.Net.TrackLane> m_TrackLaneData;

			public ComponentLookup<MasterLane> m_MasterLaneData;

			public ComponentLookup<SlaveLane> m_SlaveLaneData;

			public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<Game.Net.Edge> m_EdgeData;

			public ComponentLookup<Game.Net.Elevation> m_ElevationData;

			public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

			public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

			public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

			public ComponentLookup<Composition> m_CompositionData;

			public ComponentLookup<Game.Areas.Lot> m_LotData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

			public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

			public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

			public ComponentLookup<NetData> m_PrefabNetData;

			public ComponentLookup<NetCompositionData> m_PrefabNetCompositionData;

			public BufferLookup<Game.Net.SubLane> m_Lanes;

			public BufferLookup<ConnectedEdge> m_ConnectedEdges;

			public BufferLookup<Game.Areas.Node> m_AreaNodes;

			public BufferLookup<Triangle> m_AreaTriangles;

			public Entity m_ResultLane;

			public Entity m_ResultOwner;

			public float m_ResultCurvePos;

			public bool m_IntersectLot;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_0345: Unknown result type (might be due to invalid IL or missing references)
				//IL_034a: Unknown result type (might be due to invalid IL or missing references)
				//IL_034f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0351: Unknown result type (might be due to invalid IL or missing references)
				//IL_0356: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0135: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0233: Unknown result type (might be due to invalid IL or missing references)
				//IL_0235: Unknown result type (might be due to invalid IL or missing references)
				//IL_023a: Unknown result type (might be due to invalid IL or missing references)
				//IL_023c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0245: Unknown result type (might be due to invalid IL or missing references)
				//IL_0259: Unknown result type (might be due to invalid IL or missing references)
				//IL_026a: Unknown result type (might be due to invalid IL or missing references)
				//IL_026f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0271: Unknown result type (might be due to invalid IL or missing references)
				//IL_027a: Unknown result type (might be due to invalid IL or missing references)
				//IL_028e: Unknown result type (might be due to invalid IL or missing references)
				//IL_029f: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_02af: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02db: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0377: Unknown result type (might be due to invalid IL or missing references)
				//IL_037c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0384: Unknown result type (might be due to invalid IL or missing references)
				//IL_048e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0490: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_020e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_04af: Unknown result type (might be due to invalid IL or missing references)
				//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_04df: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_031d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0307: Unknown result type (might be due to invalid IL or missing references)
				//IL_030c: Unknown result type (might be due to invalid IL or missing references)
				//IL_021e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0224: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0328: Unknown result type (might be due to invalid IL or missing references)
				//IL_032d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0191: Unknown result type (might be due to invalid IL or missing references)
				//IL_0198: Unknown result type (might be due to invalid IL or missing references)
				//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0403: Unknown result type (might be due to invalid IL or missing references)
				//IL_041e: Unknown result type (might be due to invalid IL or missing references)
				//IL_042d: Unknown result type (might be due to invalid IL or missing references)
				//IL_043c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0441: Unknown result type (might be due to invalid IL or missing references)
				//IL_0448: Unknown result type (might be due to invalid IL or missing references)
				//IL_044d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0454: Unknown result type (might be due to invalid IL or missing references)
				//IL_0459: Unknown result type (might be due to invalid IL or missing references)
				//IL_045f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0461: Unknown result type (might be due to invalid IL or missing references)
				//IL_0465: Unknown result type (might be due to invalid IL or missing references)
				//IL_046c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz) || !m_Lanes.HasBuffer(item.m_Area) || item.m_Area == m_IgnoreOwner)
				{
					return;
				}
				DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[item.m_Area];
				Triangle triangle = m_AreaTriangles[item.m_Area][item.m_Triangle];
				Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
				float3 elevations = AreaUtils.GetElevations(nodes, triangle);
				bool flag = m_LotData.HasComponent(item.m_Area);
				float2 val = default(float2);
				float num = ((!flag && (!m_OnGround || !math.any(elevations == float.MinValue))) ? MathUtils.Distance(triangle2, m_Position, ref val) : MathUtils.Distance(((Triangle3)(ref triangle2)).xz, ((float3)(ref m_Position)).xz, ref val));
				if (num >= m_CmpDistance)
				{
					return;
				}
				float3 val2 = MathUtils.Position(triangle2, val);
				bool flag2 = false;
				if (math.any(m_CheckLot))
				{
					Segment val3 = default(Segment);
					((Segment)(ref val3))._002Ector(m_Position, val2);
					flag2 |= m_CheckLot.x && MathUtils.Intersect(((Quad2)(ref m_LotQuad)).ab, ((Segment)(ref val3)).xz, ref val);
					flag2 |= m_CheckLot.y && MathUtils.Intersect(((Quad2)(ref m_LotQuad)).bc, ((Segment)(ref val3)).xz, ref val);
					flag2 |= m_CheckLot.z && MathUtils.Intersect(((Quad2)(ref m_LotQuad)).cd, ((Segment)(ref val3)).xz, ref val);
					flag2 |= m_CheckLot.w && MathUtils.Intersect(((Quad2)(ref m_LotQuad)).da, ((Segment)(ref val3)).xz, ref val);
				}
				Owner owner = default(Owner);
				if (flag && (m_MasterLot == Entity.Null || (item.m_Area != m_MasterLot && (!m_OwnerData.TryGetComponent(item.m_Area, ref owner) || owner.m_Owner != m_MasterLot))))
				{
					bool3 val4 = AreaUtils.IsEdge(nodes, triangle);
					val4 &= (math.cmin(((int3)(ref triangle.m_Indices)).xy) != 0) | (math.cmax(((int3)(ref triangle.m_Indices)).xy) != 1);
					val4 &= (math.cmin(((int3)(ref triangle.m_Indices)).yz) != 0) | (math.cmax(((int3)(ref triangle.m_Indices)).yz) != 1);
					val4 &= (math.cmin(((int3)(ref triangle.m_Indices)).zx) != 0) | (math.cmax(((int3)(ref triangle.m_Indices)).zx) != 1);
					float num2 = default(float);
					if ((val4.x && MathUtils.Distance(((Triangle3)(ref triangle2)).ab, val2, ref num2) < 0.1f) || (val4.y && MathUtils.Distance(((Triangle3)(ref triangle2)).bc, val2, ref num2) < 0.1f) || (val4.z && MathUtils.Distance(((Triangle3)(ref triangle2)).ca, val2, ref num2) < 0.1f))
					{
						return;
					}
				}
				DynamicBuffer<Game.Net.SubLane> val5 = m_Lanes[item.m_Area];
				Entity val6 = Entity.Null;
				float num3 = float.MaxValue;
				float resultCurvePos = 0f;
				Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
				bool2 val7 = default(bool2);
				float2 val8 = default(float2);
				float num5 = default(float);
				for (int i = 0; i < val5.Length; i++)
				{
					Entity subLane = val5[i].m_SubLane;
					if (!m_ConnectionLaneData.TryGetComponent(subLane, ref connectionLane) || !CheckLaneType(connectionLane))
					{
						continue;
					}
					Curve curve = m_CurveData[subLane];
					((bool2)(ref val7))._002Ector(MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.a)).xz, ref val8), MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.d)).xz, ref val8));
					if (math.any(val7))
					{
						float num4 = MathUtils.Distance(curve.m_Bezier, val2, ref num5);
						if (num4 < num3)
						{
							float2 val9 = math.select(new float2(0f, 0.49f), math.select(new float2(0.51f, 1f), new float2(0f, 1f), val7.x), val7.y);
							num3 = num4;
							val6 = subLane;
							resultCurvePos = math.clamp(num5, val9.x, val9.y);
						}
					}
				}
				if (val6 != Entity.Null)
				{
					m_CmpDistance = num;
					m_MaxDistance = num;
					m_ResultLane = val6;
					m_ResultOwner = item.m_Area;
					m_ResultCurvePos = resultCurvePos;
					m_IntersectLot = flag2;
					m_Bounds = new Bounds3(m_Position - num, m_Position + num);
				}
			}

			private bool CheckLaneType(Game.Net.ConnectionLane connectionLane)
			{
				switch (m_ConnectionType)
				{
				case RouteConnectionType.Pedestrian:
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0)
					{
						return true;
					}
					return false;
				case RouteConnectionType.Road:
				case RouteConnectionType.Cargo:
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0 && (connectionLane.m_RoadTypes & m_CarType) != RoadTypes.None)
					{
						return true;
					}
					return false;
				case RouteConnectionType.Track:
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Track) != 0 && (connectionLane.m_TrackTypes & m_TrackType) != TrackTypes.None)
					{
						return true;
					}
					return false;
				default:
					return false;
				}
			}

			private void CheckDistance(EdgeGeometry edgeGeometry, ref float maxDistance, ref int crossedSide, ref float3 maxDistancePos)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.DistanceSquared(((Bounds3)(ref edgeGeometry.m_Bounds)).xz, ((float3)(ref m_Position)).xz) < maxDistance * maxDistance)
				{
					CheckDistance(edgeGeometry.m_Start.m_Left, -1, ref maxDistance, ref crossedSide, ref maxDistancePos);
					CheckDistance(edgeGeometry.m_Start.m_Right, 1, ref maxDistance, ref crossedSide, ref maxDistancePos);
					CheckDistance(edgeGeometry.m_End.m_Left, -1, ref maxDistance, ref crossedSide, ref maxDistancePos);
					CheckDistance(edgeGeometry.m_End.m_Right, 1, ref maxDistance, ref crossedSide, ref maxDistancePos);
				}
			}

			private void CheckDistance(EdgeNodeGeometry nodeGeometry, ref float maxDistance, ref int crossedSide, ref float3 maxDistancePos)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.DistanceSquared(((Bounds3)(ref nodeGeometry.m_Bounds)).xz, ((float3)(ref m_Position)).xz) < maxDistance * maxDistance)
				{
					if (nodeGeometry.m_MiddleRadius > 0f)
					{
						CheckDistance(nodeGeometry.m_Left.m_Left, -1, ref maxDistance, ref crossedSide, ref maxDistancePos);
						CheckDistance(nodeGeometry.m_Left.m_Right, 1, ref maxDistance, ref crossedSide, ref maxDistancePos);
						CheckDistance(nodeGeometry.m_Right.m_Left, -2, ref maxDistance, ref crossedSide, ref maxDistancePos);
						CheckDistance(nodeGeometry.m_Right.m_Right, 2, ref maxDistance, ref crossedSide, ref maxDistancePos);
					}
					else
					{
						CheckDistance(nodeGeometry.m_Left.m_Left, -1, ref maxDistance, ref crossedSide, ref maxDistancePos);
						CheckDistance(nodeGeometry.m_Right.m_Right, 1, ref maxDistance, ref crossedSide, ref maxDistancePos);
					}
				}
			}

			private void CheckDistance(Bezier4x3 curve, int side, ref float maxDistance, ref int crossedSide, ref float3 maxDistancePos)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.DistanceSquared(MathUtils.Bounds(((Bezier4x3)(ref curve)).xz), ((float3)(ref m_Position)).xz) < maxDistance * maxDistance)
				{
					float num2 = default(float);
					float num = MathUtils.Distance(((Bezier4x3)(ref curve)).xz, ((float3)(ref m_Position)).xz, ref num2);
					if (num < maxDistance)
					{
						maxDistance = num;
						maxDistancePos = MathUtils.Position(curve, num2);
						float2 val = MathUtils.Tangent(((Bezier4x3)(ref curve)).xz, num2);
						val = math.select(MathUtils.Left(val), MathUtils.Right(val), side > 0);
						float2 val2 = ((float3)(ref m_Position)).xz - ((float3)(ref maxDistancePos)).xz;
						crossedSide = math.select(0, side, math.dot(val, val2) > 0f);
					}
				}
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity netEntity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_016d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0302: Unknown result type (might be due to invalid IL or missing references)
				//IL_0309: Unknown result type (might be due to invalid IL or missing references)
				//IL_030e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0315: Unknown result type (might be due to invalid IL or missing references)
				//IL_031a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0325: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_033f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0344: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a97: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
				//IL_0186: Unknown result type (might be due to invalid IL or missing references)
				//IL_0192: Unknown result type (might be due to invalid IL or missing references)
				//IL_0397: Unknown result type (might be due to invalid IL or missing references)
				//IL_036e: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0aa9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_0411: Unknown result type (might be due to invalid IL or missing references)
				//IL_0422: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b0c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_0443: Unknown result type (might be due to invalid IL or missing references)
				//IL_0452: Unknown result type (might be due to invalid IL or missing references)
				//IL_0463: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b1f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b21: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ae6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0af2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0204: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_04be: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0488: Unknown result type (might be due to invalid IL or missing references)
				//IL_048d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0494: Unknown result type (might be due to invalid IL or missing references)
				//IL_0499: Unknown result type (might be due to invalid IL or missing references)
				//IL_049e: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b2f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b34: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b08: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b0a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0298: Unknown result type (might be due to invalid IL or missing references)
				//IL_0227: Unknown result type (might be due to invalid IL or missing references)
				//IL_0213: Unknown result type (might be due to invalid IL or missing references)
				//IL_0218: Unknown result type (might be due to invalid IL or missing references)
				//IL_0517: Unknown result type (might be due to invalid IL or missing references)
				//IL_0522: Unknown result type (might be due to invalid IL or missing references)
				//IL_052e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0533: Unknown result type (might be due to invalid IL or missing references)
				//IL_053c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0568: Unknown result type (might be due to invalid IL or missing references)
				//IL_056f: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_04da: Unknown result type (might be due to invalid IL or missing references)
				//IL_04df: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b40: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
				//IL_0242: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b8e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ba8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0baa: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bb1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bc7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b4f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
				//IL_0254: Unknown result type (might be due to invalid IL or missing references)
				//IL_0262: Unknown result type (might be due to invalid IL or missing references)
				//IL_0267: Unknown result type (might be due to invalid IL or missing references)
				//IL_0587: Unknown result type (might be due to invalid IL or missing references)
				//IL_058c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0595: Unknown result type (might be due to invalid IL or missing references)
				//IL_05be: Unknown result type (might be due to invalid IL or missing references)
				//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_060c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0618: Unknown result type (might be due to invalid IL or missing references)
				//IL_062d: Unknown result type (might be due to invalid IL or missing references)
				//IL_062f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0632: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bd5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
				//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bf3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bf8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bfd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b86: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b8b: Unknown result type (might be due to invalid IL or missing references)
				//IL_027c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0281: Unknown result type (might be due to invalid IL or missing references)
				//IL_0271: Unknown result type (might be due to invalid IL or missing references)
				//IL_0276: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0641: Unknown result type (might be due to invalid IL or missing references)
				//IL_028b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0290: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a5c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a6e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
				//IL_0662: Unknown result type (might be due to invalid IL or missing references)
				//IL_0669: Unknown result type (might be due to invalid IL or missing references)
				//IL_0670: Unknown result type (might be due to invalid IL or missing references)
				//IL_0675: Unknown result type (might be due to invalid IL or missing references)
				//IL_0679: Unknown result type (might be due to invalid IL or missing references)
				//IL_0685: Unknown result type (might be due to invalid IL or missing references)
				//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0736: Unknown result type (might be due to invalid IL or missing references)
				//IL_073b: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_077d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0782: Unknown result type (might be due to invalid IL or missing references)
				//IL_0746: Unknown result type (might be due to invalid IL or missing references)
				//IL_0710: Unknown result type (might be due to invalid IL or missing references)
				//IL_0715: Unknown result type (might be due to invalid IL or missing references)
				//IL_0717: Unknown result type (might be due to invalid IL or missing references)
				//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_078d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0757: Unknown result type (might be due to invalid IL or missing references)
				//IL_075c: Unknown result type (might be due to invalid IL or missing references)
				//IL_075e: Unknown result type (might be due to invalid IL or missing references)
				//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0802: Unknown result type (might be due to invalid IL or missing references)
				//IL_0807: Unknown result type (might be due to invalid IL or missing references)
				//IL_0817: Unknown result type (might be due to invalid IL or missing references)
				//IL_081e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0823: Unknown result type (might be due to invalid IL or missing references)
				//IL_0827: Unknown result type (might be due to invalid IL or missing references)
				//IL_0832: Unknown result type (might be due to invalid IL or missing references)
				//IL_0837: Unknown result type (might be due to invalid IL or missing references)
				//IL_083e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0844: Unknown result type (might be due to invalid IL or missing references)
				//IL_0846: Unknown result type (might be due to invalid IL or missing references)
				//IL_084d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0858: Unknown result type (might be due to invalid IL or missing references)
				//IL_085d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0864: Unknown result type (might be due to invalid IL or missing references)
				//IL_086a: Unknown result type (might be due to invalid IL or missing references)
				//IL_086c: Unknown result type (might be due to invalid IL or missing references)
				//IL_087f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0883: Unknown result type (might be due to invalid IL or missing references)
				//IL_088a: Unknown result type (might be due to invalid IL or missing references)
				//IL_088f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0891: Unknown result type (might be due to invalid IL or missing references)
				//IL_089b: Unknown result type (might be due to invalid IL or missing references)
				//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_079e: Unknown result type (might be due to invalid IL or missing references)
				//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_090a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0912: Unknown result type (might be due to invalid IL or missing references)
				//IL_0917: Unknown result type (might be due to invalid IL or missing references)
				//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0928: Unknown result type (might be due to invalid IL or missing references)
				//IL_094f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0956: Unknown result type (might be due to invalid IL or missing references)
				//IL_096e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0973: Unknown result type (might be due to invalid IL or missing references)
				//IL_097c: Unknown result type (might be due to invalid IL or missing references)
				//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a05: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
				DynamicBuffer<Game.Net.SubLane> val = default(DynamicBuffer<Game.Net.SubLane>);
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz) || !m_Lanes.TryGetBuffer(netEntity, ref val) || netEntity == m_IgnoreOwner)
				{
					return;
				}
				float maxDistance = 10f;
				float3 maxDistancePos = default(float3);
				int crossedSide = 0;
				int num = -1;
				bool flag = false;
				if (m_ConnectionType == RouteConnectionType.Pedestrian)
				{
					EdgeGeometry edgeGeometry = default(EdgeGeometry);
					DynamicBuffer<ConnectedEdge> val2 = default(DynamicBuffer<ConnectedEdge>);
					if (m_EdgeGeometryData.TryGetComponent(netEntity, ref edgeGeometry))
					{
						CheckDistance(edgeGeometry, ref maxDistance, ref crossedSide, ref maxDistancePos);
						flag = true;
					}
					else if (m_ConnectedEdges.TryGetBuffer(netEntity, ref val2))
					{
						StartNodeGeometry startNodeGeometry = default(StartNodeGeometry);
						EndNodeGeometry endNodeGeometry = default(EndNodeGeometry);
						for (int i = 0; i < val2.Length; i++)
						{
							ConnectedEdge connectedEdge = val2[i];
							Game.Net.Edge edge = m_EdgeData[connectedEdge.m_Edge];
							float num2 = maxDistance;
							if (edge.m_Start == netEntity)
							{
								if (m_StartNodeGeometryData.TryGetComponent(connectedEdge.m_Edge, ref startNodeGeometry))
								{
									CheckDistance(startNodeGeometry.m_Geometry, ref maxDistance, ref crossedSide, ref maxDistancePos);
								}
							}
							else if (edge.m_End == netEntity && m_EndNodeGeometryData.TryGetComponent(connectedEdge.m_Edge, ref endNodeGeometry))
							{
								CheckDistance(endNodeGeometry.m_Geometry, ref maxDistance, ref crossedSide, ref maxDistancePos);
							}
							num = math.select(num, i, maxDistance != num2);
						}
					}
				}
				float num3 = 0f;
				bool flag2 = false;
				Game.Net.Elevation elevation = default(Game.Net.Elevation);
				if (m_ElevationData.TryGetComponent(netEntity, ref elevation))
				{
					flag2 = true;
					num3 = ((!flag || crossedSide == 0) ? (math.csum(elevation.m_Elevation) * 0.5f) : math.select(elevation.m_Elevation.x, elevation.m_Elevation.y, crossedSide > 0));
					if (crossedSide != 0)
					{
						PrefabRef prefabRef = m_PrefabRefData[netEntity];
						if ((m_PrefabNetData[prefabRef.m_Prefab].m_RequiredLayers & (Layer.Road | Layer.TrainTrack | Layer.Pathway | Layer.TramTrack | Layer.SubwayTrack | Layer.PublicTransportRoad)) != Layer.None)
						{
							Entity val3 = Entity.Null;
							DynamicBuffer<ConnectedEdge> val4 = default(DynamicBuffer<ConnectedEdge>);
							if (flag)
							{
								Composition composition = default(Composition);
								if (m_CompositionData.TryGetComponent(netEntity, ref composition))
								{
									val3 = composition.m_Edge;
								}
							}
							else if (num != -1 && m_ConnectedEdges.TryGetBuffer(netEntity, ref val4))
							{
								ConnectedEdge connectedEdge2 = val4[num];
								Composition composition2 = default(Composition);
								if (m_CompositionData.TryGetComponent(netEntity, ref composition2))
								{
									Game.Net.Edge edge2 = m_EdgeData[connectedEdge2.m_Edge];
									if (edge2.m_Start == netEntity)
									{
										val3 = composition2.m_StartNode;
									}
									else if (edge2.m_End == netEntity)
									{
										val3 = composition2.m_EndNode;
									}
								}
							}
							NetCompositionData netCompositionData = default(NetCompositionData);
							if (m_PrefabNetCompositionData.TryGetComponent(val3, ref netCompositionData))
							{
								CompositionFlags.Side side = ((crossedSide > 0) ? netCompositionData.m_Flags.m_Right : netCompositionData.m_Flags.m_Left);
								if ((netCompositionData.m_Flags.m_General & (CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel)) != 0)
								{
									if (math.abs(crossedSide) == 1 || (side & CompositionFlags.Side.HighTransition) == 0)
									{
										return;
									}
								}
								else if ((side & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) != 0 && (math.abs(crossedSide) == 1 || (side & CompositionFlags.Side.LowTransition) == 0))
								{
									return;
								}
							}
						}
					}
				}
				Entity val5 = Entity.Null;
				float4 val6 = float4.op_Implicit(float.MaxValue);
				float4 val7 = float4.op_Implicit(float.MaxValue);
				float resultCurvePos = 0f;
				Bounds3 val8 = default(Bounds3);
				bool flag3 = false;
				Game.Net.PedestrianLane pedestrianLane = default(Game.Net.PedestrianLane);
				Game.Net.CarLane carLane = default(Game.Net.CarLane);
				float num5 = default(float);
				float2 val12 = default(float2);
				Segment val15 = default(Segment);
				float2 val17 = default(float2);
				for (int j = 0; j < val.Length; j++)
				{
					Entity subLane = val[j].m_SubLane;
					switch (m_ConnectionType)
					{
					case RouteConnectionType.Pedestrian:
						if (!m_PedestrianLaneData.TryGetComponent(subLane, ref pedestrianLane) || (pedestrianLane.m_Flags & (PedestrianLaneFlags.AllowMiddle | PedestrianLaneFlags.OnWater)) != PedestrianLaneFlags.AllowMiddle)
						{
							continue;
						}
						break;
					case RouteConnectionType.Road:
					{
						if (!m_CarLaneData.TryGetComponent(subLane, ref carLane) || m_MasterLaneData.HasComponent(subLane) || (carLane.m_Flags & CarLaneFlags.Unsafe) != 0)
						{
							continue;
						}
						PrefabRef prefabRef3 = m_PrefabRefData[subLane];
						if ((m_PrefabCarLaneData[prefabRef3.m_Prefab].m_RoadTypes & m_CarType) == 0)
						{
							continue;
						}
						break;
					}
					case RouteConnectionType.Track:
					{
						if (!m_TrackLaneData.HasComponent(subLane))
						{
							continue;
						}
						PrefabRef prefabRef2 = m_PrefabRefData[subLane];
						if ((m_PrefabTrackLaneData[prefabRef2.m_Prefab].m_TrackTypes & m_TrackType) == 0)
						{
							continue;
						}
						break;
					}
					default:
						continue;
					}
					Curve curve = m_CurveData[subLane];
					PrefabRef prefabRef4 = m_PrefabRefData[subLane];
					NetLaneData netLaneData = m_PrefabNetLaneData[prefabRef4.m_Prefab];
					float num4 = netLaneData.m_Width * 0.5f;
					if (maxDistance == 10f)
					{
						Bounds3 val9 = MathUtils.Expand(MathUtils.Bounds(curve.m_Bezier), float3.op_Implicit(num4));
						if (!MathUtils.Intersect(((Bounds3)(ref val9)).xz, ((Bounds3)(ref m_Bounds)).xz))
						{
							continue;
						}
					}
					else if (val5 != Entity.Null)
					{
						Bounds3 val10 = MathUtils.Expand(MathUtils.Bounds(curve.m_Bezier), float3.op_Implicit(num4));
						if (!MathUtils.Intersect(((Bounds3)(ref val10)).xz, ((Bounds3)(ref val8)).xz))
						{
							continue;
						}
					}
					bool flag4 = (netLaneData.m_Flags & LaneFlags.OnWater) != 0;
					float4 val11 = float4.op_Implicit(MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref m_Position)).xz, ref num5));
					float num6 = MathUtils.Position(((Bezier4x3)(ref curve.m_Bezier)).y, num5);
					((float2)(ref val12))._002Ector(m_Position.y - num6, m_Elevation - num3);
					val12 = math.select(val12, float2.op_Implicit(0f), (m_OnGround && !flag2) || flag4);
					float num7 = math.max(0f, num4 - val11.x) * 0.1f / math.max(0.01f, num4);
					val11.x = math.max(0f, val11.x - num4) - num7;
					val11.y = math.cmin(math.abs(val12));
					val11.z = math.length(math.max(float2.op_Implicit(0f), ((float4)(ref val11)).xy));
					val11.w = val11.z + math.min(0f, val11.x);
					bool flag5 = false;
					float4 val13 = val11;
					if (math.any(m_CheckLot) && (val11.x < 10f || maxDistance < 10f))
					{
						float2 xz = ((float3)(ref m_Position)).xz;
						float3 val14 = MathUtils.Position(curve.m_Bezier, num5);
						((Segment)(ref val15))._002Ector(xz, ((float3)(ref val14)).xz);
						Segment val16 = default(Segment);
						float num8 = 1f;
						bool flag6 = false;
						if (m_CheckLot.x && MathUtils.Intersect(((Quad2)(ref m_LotQuad)).ab, val15, ref val17) && val17.y < num8)
						{
							val16 = ((Quad2)(ref m_LotQuad)).ab;
							num8 = val17.y;
							flag6 = false;
						}
						if (m_CheckLot.y && MathUtils.Intersect(((Quad2)(ref m_LotQuad)).bc, val15, ref val17) && val17.y < num8)
						{
							val16 = ((Quad2)(ref m_LotQuad)).bc;
							num8 = val17.y;
							flag6 = true;
						}
						if (m_CheckLot.z && MathUtils.Intersect(((Quad2)(ref m_LotQuad)).cd, val15, ref val17) && val17.y < num8)
						{
							val16 = ((Quad2)(ref m_LotQuad)).cd;
							num8 = val17.y;
							flag6 = false;
						}
						if (m_CheckLot.w && MathUtils.Intersect(((Quad2)(ref m_LotQuad)).da, val15, ref val17) && val17.y < num8)
						{
							val16 = ((Quad2)(ref m_LotQuad)).da;
							num8 = val17.y;
							flag6 = true;
						}
						if (num8 != 1f)
						{
							float num9 = MathUtils.Distance(val16, ((float3)(ref m_Position)).xz, ref val17.x);
							flag5 = true;
							if (num9 < val11.x)
							{
								float2 val18 = MathUtils.Position(val16, val17.x);
								val13.x = MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, val18, ref num5);
								float3 val19 = MathUtils.Position(curve.m_Bezier, num5);
								float2 val20 = math.normalizesafe(((float3)(ref val19)).xz - ((float3)(ref m_Position)).xz, default(float2));
								float2 val21 = ((float3)(ref maxDistancePos)).xz - ((float3)(ref m_Position)).xz;
								float2 val22 = default(float2);
								float num10 = math.saturate(math.dot(val20, math.normalizesafe(val21, val22)));
								((Segment)(ref val15))._002Ector(val18, ((float3)(ref maxDistancePos)).xz + MathUtils.ClampLength(((float3)(ref maxDistancePos)).xz - val18, 1f));
								if (flag6)
								{
									if (MathUtils.Intersect(val15, Line2.op_Implicit(((Quad2)(ref m_LotQuad)).ab), ref val22) || MathUtils.Intersect(val15, Line2.op_Implicit(((Quad2)(ref m_LotQuad)).cd), ref val22))
									{
										continue;
									}
								}
								else if (MathUtils.Intersect(val15, Line2.op_Implicit(((Quad2)(ref m_LotQuad)).bc), ref val22) || MathUtils.Intersect(val15, Line2.op_Implicit(((Quad2)(ref m_LotQuad)).da), ref val22))
								{
									continue;
								}
								num6 = val19.y;
								((float2)(ref val12))._002Ector(m_Position.y - num6, m_Elevation - num3);
								val12 = math.select(val12, float2.op_Implicit(0f), (m_OnGround && !flag2) || flag4);
								num7 = math.max(0f, num4 - val13.x) * 0.1f / math.max(0.01f, num4);
								val13.x = math.max(0f, val13.x - num4) - num7;
								val13.x = num9 + val13.x * math.lerp(1f, 0.01f, num10);
								val13.y = math.cmin(math.abs(val12));
								val13.z = math.length(math.max(float2.op_Implicit(0f), ((float4)(ref val13)).xy));
								val13.w = val13.z + math.min(0f, val13.x);
							}
						}
					}
					if (val13.w < val7.w)
					{
						val5 = subLane;
						val6 = val11;
						val7 = val13;
						resultCurvePos = num5;
						((Bounds3)(ref val8))._002Ector(m_Position - val11.z, m_Position + val11.z);
						flag3 = flag5;
					}
				}
				if (!(val5 != Entity.Null))
				{
					return;
				}
				if (maxDistance < val6.x && maxDistance < 10f)
				{
					val6.x = maxDistance;
					val6.z = math.length(math.max(float2.op_Implicit(0f), ((float4)(ref val6)).xy));
					val6.w = val6.z + math.min(0f, val6.x);
					if (!flag3)
					{
						val7 = val6;
					}
				}
				if (val7.w < m_CmpDistance && !DirectlyConnected(netEntity, m_IgnoreOwner) && (!(m_IgnoreConnected != Entity.Null) || !(netEntity != m_IgnoreConnected) || !DirectlyConnected(netEntity, m_IgnoreConnected)))
				{
					SlaveLane slaveLane = default(SlaveLane);
					if (m_ConnectionType == RouteConnectionType.Road && m_SlaveLaneData.TryGetComponent(val5, ref slaveLane))
					{
						val5 = val[(int)slaveLane.m_MasterIndex].m_SubLane;
					}
					m_CmpDistance = val7.w;
					m_MaxDistance = val6.x;
					m_ResultLane = val5;
					m_ResultOwner = netEntity;
					m_ResultCurvePos = resultCurvePos;
					m_IntersectLot = flag3;
					if (!math.any(m_CheckLot))
					{
						m_Bounds = new Bounds3(m_Position - val6.z, m_Position + val6.z);
					}
				}
			}

			private bool DirectlyConnected(Entity netEntity1, Entity netEntity2)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0111: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0139: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0157: Unknown result type (might be due to invalid IL or missing references)
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0148: Unknown result type (might be due to invalid IL or missing references)
				//IL_014d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0166: Unknown result type (might be due to invalid IL or missing references)
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				if (m_EdgeData.HasComponent(netEntity1))
				{
					if (m_EdgeData.HasComponent(netEntity2))
					{
						Game.Net.Edge edge = m_EdgeData[netEntity1];
						Game.Net.Edge edge2 = m_EdgeData[netEntity2];
						if (edge.m_Start == edge2.m_Start || edge.m_Start == edge2.m_End || edge.m_End == edge2.m_Start || edge.m_End == edge2.m_End)
						{
							return true;
						}
					}
					else
					{
						Game.Net.Edge edge3 = m_EdgeData[netEntity1];
						if (edge3.m_Start == netEntity2 || edge3.m_End == netEntity2)
						{
							return true;
						}
					}
				}
				else if (m_EdgeData.HasComponent(netEntity2))
				{
					Game.Net.Edge edge4 = m_EdgeData[netEntity2];
					if (edge4.m_Start == netEntity1 || edge4.m_End == netEntity1)
					{
						return true;
					}
				}
				else if (m_ConnectedEdges.HasBuffer(netEntity1))
				{
					DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[netEntity1];
					for (int i = 0; i < val.Length; i++)
					{
						Entity edge5 = val[i].m_Edge;
						Game.Net.Edge edge6 = m_EdgeData[edge5];
						if ((!(edge6.m_Start != netEntity1) || !(edge6.m_End != netEntity1)) && (edge6.m_Start == netEntity2 || edge6.m_End == netEntity2))
						{
							return false;
						}
					}
				}
				return false;
			}
		}

		[ReadOnly]
		public ComponentLookup<Waypoint> m_WaypointData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> m_PrefabConnectionData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabNetCompositionData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.TrackLane> m_TrackLaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.OutsideConnection> m_NetOutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_NetElevationData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_ObjectOutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> m_LotData;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Surface> m_SurfaceData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public BufferLookup<RouteSegment> m_Segments;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_Lanes;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<AccessLane> m_AccessLaneData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<RouteLane> m_RouteLaneData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Connected> m_ConnectedData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public NativeArray<Entity> m_UpdatedList;

		[ReadOnly]
		public AirwayHelpers.AirwayData m_AirwayData;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		[ReadOnly]
		public EntityArchetype m_PathTargetEventArchetype;

		public ParallelWriter<PathTargetInfo> m_PathTargetInfo;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0856: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0864: Unknown result type (might be due to invalid IL or missing references)
			//IL_086c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0870: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0887: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0909: Unknown result type (might be due to invalid IL or missing references)
			//IL_0919: Unknown result type (might be due to invalid IL or missing references)
			//IL_091e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0923: Unknown result type (might be due to invalid IL or missing references)
			//IL_0965: Unknown result type (might be due to invalid IL or missing references)
			//IL_096a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0897: Unknown result type (might be due to invalid IL or missing references)
			//IL_089c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_08af: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_060f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_0633: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_097f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_09de: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0702: Unknown result type (might be due to invalid IL or missing references)
			//IL_0707: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_0804: Unknown result type (might be due to invalid IL or missing references)
			//IL_0716: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_071c: Unknown result type (might be due to invalid IL or missing references)
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0730: Unknown result type (might be due to invalid IL or missing references)
			//IL_0735: Unknown result type (might be due to invalid IL or missing references)
			//IL_0737: Unknown result type (might be due to invalid IL or missing references)
			//IL_0739: Unknown result type (might be due to invalid IL or missing references)
			//IL_081f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0742: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_0833: Unknown result type (might be due to invalid IL or missing references)
			//IL_083d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0849: Unknown result type (might be due to invalid IL or missing references)
			//IL_084e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_075d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0762: Unknown result type (might be due to invalid IL or missing references)
			//IL_0779: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_UpdatedList[index];
			PrefabRef prefabRef = m_PrefabRefData[val];
			bool flag = false;
			bool onGround = false;
			float num = 0f;
			float3 val2;
			if (m_PositionData.HasComponent(val))
			{
				val2 = m_PositionData[val].m_Position;
			}
			else
			{
				if (!m_TransformData.HasComponent(val))
				{
					throw new Exception("FindWaypointConnectionsJob: Position not found!");
				}
				val2 = m_TransformData[val].m_Position;
				flag = true;
				if (m_ElevationData.HasComponent(val))
				{
					num = m_ElevationData[val].m_Elevation;
				}
				else
				{
					onGround = true;
				}
			}
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			Connected connected = default(Connected);
			Entity val3 = Entity.Null;
			Entity val4 = Entity.Null;
			Entity preferOwner = Entity.Null;
			Entity val5 = Entity.Null;
			Entity laneOwner = Entity.Null;
			AccessLane other = default(AccessLane);
			bool intersectLot = false;
			if (m_ConnectedData.HasComponent(val))
			{
				connected = m_ConnectedData[val];
				if (connected.m_Connected != Entity.Null && (!m_PrefabRefData.HasComponent(connected.m_Connected) || m_DeletedData.HasComponent(connected.m_Connected)))
				{
					connected.m_Connected = Entity.Null;
					m_ConnectedData[val] = connected;
				}
				val3 = GetLaneContainer(connected.m_Connected);
				val4 = GetMasterLot(connected.m_Connected);
				Attached attached = default(Attached);
				if (m_AttachedData.TryGetComponent(connected.m_Connected, ref attached))
				{
					preferOwner = attached.m_Parent;
				}
			}
			else
			{
				val3 = GetLaneContainer(val);
				val4 = GetMasterLot(val);
				Attached attached2 = default(Attached);
				if (m_AttachedData.TryGetComponent(val, ref attached2))
				{
					preferOwner = attached2.m_Parent;
				}
			}
			if (m_TransformData.HasComponent(connected.m_Connected))
			{
				val2 = m_TransformData[connected.m_Connected].m_Position;
				flag = true;
				if (m_ElevationData.HasComponent(connected.m_Connected))
				{
					num = m_ElevationData[connected.m_Connected].m_Elevation;
				}
				else
				{
					onGround = true;
				}
			}
			RouteConnectionData routeConnectionData = default(RouteConnectionData);
			m_PrefabConnectionData.TryGetComponent(prefabRef.m_Prefab, ref routeConnectionData);
			if (m_PrefabRefData.HasComponent(connected.m_Connected))
			{
				PrefabRef prefabRef2 = m_PrefabRefData[connected.m_Connected];
				if (m_PrefabConnectionData.HasComponent(prefabRef2.m_Prefab))
				{
					RouteConnectionData routeConnectionData2 = m_PrefabConnectionData[prefabRef2.m_Prefab];
					routeConnectionData.m_StartLaneOffset = math.max(routeConnectionData.m_StartLaneOffset, routeConnectionData2.m_StartLaneOffset);
					routeConnectionData.m_EndMargin = math.max(routeConnectionData.m_EndMargin, routeConnectionData2.m_EndMargin);
				}
			}
			if (m_AccessLaneData.HasComponent(val))
			{
				AccessLane accessLane = m_AccessLaneData[val];
				other = accessLane;
				bool flag5 = true;
				if (m_PrefabRefData.HasComponent(connected.m_Connected))
				{
					PrefabRef prefabRef3 = m_PrefabRefData[connected.m_Connected];
					if (m_PrefabSpawnLocationData.HasComponent(prefabRef3.m_Prefab) && m_PrefabConnectionData.HasComponent(prefabRef3.m_Prefab))
					{
						SpawnLocationData spawnLocationData = m_PrefabSpawnLocationData[prefabRef3.m_Prefab];
						RouteConnectionData routeConnectionData3 = m_PrefabConnectionData[prefabRef3.m_Prefab];
						if (spawnLocationData.m_ConnectionType != RouteConnectionType.None && spawnLocationData.m_ConnectionType == routeConnectionData3.m_AccessConnectionType && spawnLocationData.m_ActivityMask.m_Mask == 0)
						{
							accessLane.m_Lane = connected.m_Connected;
							accessLane.m_CurvePos = 0f;
							flag5 = false;
						}
					}
				}
				if (flag5)
				{
					FindLane(val3, val2, num, routeConnectionData.m_AccessConnectionType, routeConnectionData.m_AccessTrackType, routeConnectionData.m_AccessRoadType, Entity.Null, preferOwner, val4, onGround, 0, out laneOwner, out accessLane.m_Lane, out accessLane.m_CurvePos, out intersectLot);
				}
				m_AccessLaneData[val] = accessLane;
				flag2 = !accessLane.Equals(other);
				flag3 = flag2 || m_UpdatedData.HasComponent(accessLane.m_Lane);
				val5 = accessLane.m_Lane;
			}
			if (m_RouteLaneData.HasComponent(val))
			{
				RouteLane routeLane = m_RouteLaneData[val];
				RouteLane other2 = routeLane;
				if (m_PrefabRefData.HasComponent(connected.m_Connected))
				{
					PrefabRef prefabRef4 = m_PrefabRefData[connected.m_Connected];
					if (m_PrefabConnectionData.HasComponent(prefabRef4.m_Prefab))
					{
						RouteConnectionData routeConnectionData4 = m_PrefabConnectionData[prefabRef4.m_Prefab];
						routeConnectionData.m_StartLaneOffset = math.max(routeConnectionData.m_StartLaneOffset, routeConnectionData4.m_StartLaneOffset);
						routeConnectionData.m_EndMargin = math.max(routeConnectionData.m_EndMargin, routeConnectionData4.m_EndMargin);
					}
				}
				float3 val6 = val2;
				float elevation = num;
				if (routeConnectionData.m_RouteConnectionType == RouteConnectionType.Air && routeConnectionData.m_RouteRoadType == RoadTypes.Airplane && val5 != Entity.Null && !m_ConnectionLaneData.HasComponent(val5))
				{
					Curve curve = m_CurveData[val5];
					float3 val7 = curve.m_Bezier.a + curve.m_Bezier.d - val6 * 2f;
					float2 xz = ((float3)(ref val7)).xz;
					if (MathUtils.TryNormalize(ref xz, 1500f))
					{
						((float3)(ref val6)).xz = ((float3)(ref val6)).xz - xz;
						elevation = 1000f;
					}
				}
				Entity val8 = Entity.Null;
				int shouldIntersectLot = 0;
				if (routeConnectionData.m_RouteConnectionType == routeConnectionData.m_AccessConnectionType)
				{
					switch (routeConnectionData.m_RouteConnectionType)
					{
					case RouteConnectionType.Road:
					case RouteConnectionType.Cargo:
					case RouteConnectionType.Air:
						if (routeConnectionData.m_AccessRoadType == routeConnectionData.m_RouteRoadType)
						{
							val8 = laneOwner;
						}
						break;
					case RouteConnectionType.Track:
						if (routeConnectionData.m_AccessTrackType == routeConnectionData.m_RouteTrackType)
						{
							val8 = laneOwner;
						}
						break;
					default:
						val8 = laneOwner;
						shouldIntersectLot = math.select(1, -1, intersectLot);
						break;
					}
				}
				FindLane(val3, val6, elevation, routeConnectionData.m_RouteConnectionType, routeConnectionData.m_RouteTrackType, routeConnectionData.m_RouteRoadType, val8, preferOwner, val4, onGround, shouldIntersectLot, out var laneOwner2, out routeLane.m_EndLane, out routeLane.m_EndCurvePos, out var _);
				routeLane.m_StartLane = routeLane.m_EndLane;
				routeLane.m_StartCurvePos = routeLane.m_EndCurvePos;
				Game.Net.CarLane carLane = default(Game.Net.CarLane);
				if (routeConnectionData.m_StartLaneOffset > 0f && m_CarLaneData.TryGetComponent(routeLane.m_EndLane, ref carLane) && (carLane.m_Flags & CarLaneFlags.Twoway) != 0)
				{
					routeConnectionData.m_StartLaneOffset = 0f;
				}
				if (routeConnectionData.m_StartLaneOffset > 0f || routeConnectionData.m_EndMargin > 0f)
				{
					MoveLaneOffsets(ref routeLane.m_StartLane, ref routeLane.m_StartCurvePos, ref routeLane.m_EndLane, ref routeLane.m_EndCurvePos, routeConnectionData.m_StartLaneOffset, routeConnectionData.m_EndMargin);
				}
				if (val5 != Entity.Null && routeLane.m_EndLane != Entity.Null && !ValidateConnection(val5, routeLane.m_EndLane, val8, laneOwner2))
				{
					Entity ownerBuilding = GetOwnerBuilding(val);
					Entity ownerBuilding2 = GetOwnerBuilding(val5);
					Entity ownerBuilding3 = GetOwnerBuilding(routeLane.m_EndLane);
					if (ownerBuilding == ownerBuilding2 && ownerBuilding == ownerBuilding3)
					{
						AccessLane accessLane2 = m_AccessLaneData[val];
						accessLane2.m_Lane = Entity.Null;
						accessLane2.m_CurvePos = 0f;
						m_AccessLaneData[val] = accessLane2;
						flag2 = !accessLane2.Equals(other);
						flag3 = flag2;
					}
					routeLane.m_StartLane = Entity.Null;
					routeLane.m_EndLane = Entity.Null;
					routeLane.m_StartCurvePos = 0f;
					routeLane.m_EndCurvePos = 0f;
				}
				m_RouteLaneData[val] = routeLane;
				flag2 |= !routeLane.Equals(other2);
				flag3 |= flag2 || m_UpdatedData.HasComponent(routeLane.m_StartLane) || m_UpdatedData.HasComponent(routeLane.m_EndLane);
				if (!flag && m_CurveData.HasComponent(routeLane.m_EndLane))
				{
					val2 = MathUtils.Position(m_CurveData[routeLane.m_EndLane].m_Bezier, routeLane.m_EndCurvePos);
				}
			}
			if (m_PositionData.HasComponent(val))
			{
				Position position = m_PositionData[val];
				if (math.distance(val2, position.m_Position) > 0.1f)
				{
					if (!m_TempData.HasComponent(val))
					{
						Entity val9 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(index, m_PathTargetEventArchetype);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathTargetMoved>(index, val9, new PathTargetMoved(val, position.m_Position, val2));
					}
					position.m_Position = val2;
					m_PositionData[val] = position;
					flag2 = true;
					flag4 = true;
				}
			}
			if (flag2)
			{
				if (m_WaypointData.HasComponent(val))
				{
					Waypoint waypoint = m_WaypointData[val];
					Owner owner = m_OwnerData[val];
					DynamicBuffer<RouteSegment> val10 = m_Segments[owner.m_Owner];
					int num2 = math.select(waypoint.m_Index - 1, val10.Length - 1, waypoint.m_Index == 0);
					RouteSegment routeSegment = val10[num2];
					RouteSegment routeSegment2 = val10[waypoint.m_Index];
					if (routeSegment.m_Segment != Entity.Null)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, routeSegment.m_Segment, default(Updated));
						if (flag4)
						{
							m_PathTargetInfo.Enqueue(new PathTargetInfo
							{
								m_Segment = routeSegment.m_Segment,
								m_Start = false
							});
						}
					}
					if (routeSegment2.m_Segment != Entity.Null)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, routeSegment2.m_Segment, default(Updated));
						if (flag4)
						{
							m_PathTargetInfo.Enqueue(new PathTargetInfo
							{
								m_Segment = routeSegment2.m_Segment,
								m_Start = true
							});
						}
					}
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
				if (m_TransformData.HasComponent(val) && m_OwnerData.HasComponent(val) && !m_TempData.HasComponent(val))
				{
					UpdateSurfaces(index, m_TransformData[val].m_Position);
				}
			}
			else if (flag3)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(index, val, default(PathfindUpdated));
			}
		}

		private Entity GetOwnerBuilding(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			Owner owner = default(Owner);
			while (m_OwnerData.TryGetComponent(entity, ref owner) && !m_BuildingData.HasComponent(entity))
			{
				entity = owner.m_Owner;
			}
			return entity;
		}

		private void UpdateSurfaces(int jobIndex, float3 position)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			SurfaceIterator surfaceIterator = new SurfaceIterator
			{
				m_Position = position,
				m_JobIndex = jobIndex,
				m_SurfaceData = m_SurfaceData,
				m_AreaNodes = m_AreaNodes,
				m_AreaTriangles = m_AreaTriangles,
				m_CommandBuffer = m_CommandBuffer
			};
			m_AreaSearchTree.Iterate<SurfaceIterator>(ref surfaceIterator, 0);
		}

		private bool ValidateConnection(Entity accessLaneEntity, Entity routeLaneEntity, Entity accessLaneOwner, Entity routeLaneOwner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			if (m_LaneData.HasComponent(accessLaneEntity) && m_LaneData.HasComponent(routeLaneEntity))
			{
				Lane lane = m_LaneData[accessLaneEntity];
				Lane lane2 = m_LaneData[routeLaneEntity];
				if (lane.m_StartNode.EqualsIgnoreCurvePos(lane2.m_StartNode) || lane.m_StartNode.EqualsIgnoreCurvePos(lane2.m_MiddleNode) || lane.m_StartNode.EqualsIgnoreCurvePos(lane2.m_EndNode) || lane.m_MiddleNode.EqualsIgnoreCurvePos(lane2.m_StartNode) || lane.m_MiddleNode.EqualsIgnoreCurvePos(lane2.m_EndNode) || lane.m_EndNode.EqualsIgnoreCurvePos(lane2.m_StartNode) || lane.m_EndNode.EqualsIgnoreCurvePos(lane2.m_MiddleNode) || lane.m_EndNode.EqualsIgnoreCurvePos(lane2.m_EndNode))
				{
					return false;
				}
			}
			if (m_EdgeData.HasComponent(accessLaneOwner))
			{
				Game.Net.Edge edge = m_EdgeData[accessLaneOwner];
				if (!ValidateConnectedEdges(edge.m_Start, routeLaneOwner))
				{
					return false;
				}
				if (!ValidateConnectedEdges(edge.m_End, routeLaneOwner))
				{
					return false;
				}
			}
			else if (m_ConnectedEdges.HasBuffer(accessLaneOwner) && !ValidateConnectedEdges(accessLaneOwner, routeLaneOwner))
			{
				return false;
			}
			if (m_EdgeData.HasComponent(routeLaneOwner))
			{
				Game.Net.Edge edge2 = m_EdgeData[routeLaneOwner];
				if (!ValidateConnectedEdges(edge2.m_Start, accessLaneOwner))
				{
					return false;
				}
				if (!ValidateConnectedEdges(edge2.m_End, accessLaneOwner))
				{
					return false;
				}
			}
			else if (m_ConnectedEdges.HasBuffer(routeLaneOwner) && !ValidateConnectedEdges(routeLaneOwner, accessLaneOwner))
			{
				return false;
			}
			return true;
		}

		private bool ValidateConnectedEdges(Entity node, Entity other)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (node == other)
			{
				return false;
			}
			DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[node];
			for (int i = 0; i < val.Length; i++)
			{
				if (val[i].m_Edge == other)
				{
					return false;
				}
			}
			return true;
		}

		private Entity GetLaneContainer(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			if (m_OwnerData.HasComponent(entity))
			{
				Owner owner = m_OwnerData[entity];
				if (m_NetOutsideConnectionData.HasComponent(owner.m_Owner) && m_Lanes.HasBuffer(owner.m_Owner))
				{
					return owner.m_Owner;
				}
			}
			if (m_ObjectOutsideConnectionData.HasComponent(entity) && m_Lanes.HasBuffer(entity))
			{
				return entity;
			}
			return Entity.Null;
		}

		private Entity GetMasterLot(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			Entity result = Entity.Null;
			Owner owner = default(Owner);
			while (m_OwnerData.TryGetComponent(entity, ref owner))
			{
				entity = owner.m_Owner;
				if (m_LotData.HasComponent(entity))
				{
					result = entity;
				}
			}
			return result;
		}

		private void MoveLaneOffsets(ref Entity startLane, ref float startCurvePos, ref Entity endLane, ref float endCurvePos, float startOffset, float endMargin)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			Curve curve = default(Curve);
			if (!m_CurveData.TryGetComponent(endLane, ref curve))
			{
				return;
			}
			Entity prevLane = Entity.Null;
			Entity nextLane = Entity.Null;
			Curve prevCurve = default(Curve);
			Curve nextCurve = default(Curve);
			Owner owner = default(Owner);
			Lane lane = default(Lane);
			DynamicBuffer<Game.Net.SubLane> val = default(DynamicBuffer<Game.Net.SubLane>);
			if (m_OwnerData.TryGetComponent(endLane, ref owner) && m_LaneData.TryGetComponent(endLane, ref lane) && m_Lanes.TryGetBuffer(owner.m_Owner, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					Lane lane2 = m_LaneData[subLane];
					if (lane2.m_EndNode.Equals(lane.m_StartNode))
					{
						prevLane = subLane;
						prevCurve = m_CurveData[subLane];
						if (nextLane != Entity.Null)
						{
							break;
						}
					}
					else if (lane2.m_StartNode.Equals(lane.m_EndNode))
					{
						nextLane = subLane;
						nextCurve = m_CurveData[subLane];
						if (prevLane != Entity.Null)
						{
							break;
						}
					}
				}
			}
			float prevDistance = MathUtils.Length(((Bezier4x3)(ref curve.m_Bezier)).xz, new Bounds1(0f, endCurvePos));
			float totalPrevDistance = prevDistance;
			if (prevLane != Entity.Null)
			{
				totalPrevDistance += MathUtils.Length(((Bezier4x3)(ref prevCurve.m_Bezier)).xz);
			}
			float nextDistance = MathUtils.Length(((Bezier4x3)(ref curve.m_Bezier)).xz, new Bounds1(endCurvePos, 1f));
			float totalNextDistance = nextDistance;
			if (nextLane != Entity.Null)
			{
				totalNextDistance += MathUtils.Length(((Bezier4x3)(ref nextCurve.m_Bezier)).xz);
			}
			float num = math.max(startOffset, endMargin);
			float num2 = 0f;
			if (num + endMargin > totalPrevDistance + totalNextDistance)
			{
				num2 = totalNextDistance - endMargin * (totalPrevDistance + totalNextDistance) / (num + endMargin);
			}
			else if (num > totalPrevDistance)
			{
				num2 = num - totalPrevDistance;
			}
			else if (endMargin > totalNextDistance)
			{
				num2 = totalNextDistance - endMargin;
			}
			MoveLaneOffset(num2, ref endLane, ref endCurvePos);
			if (startOffset == 0f)
			{
				startLane = endLane;
				startCurvePos = endCurvePos;
			}
			else
			{
				startOffset = num2 - startOffset;
				MoveLaneOffset(startOffset, ref startLane, ref startCurvePos);
			}
			void MoveLaneOffset(float offset, ref Entity reference, ref float curvePos)
			{
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0113: Unknown result type (might be due to invalid IL or missing references)
				//IL_0118: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0129: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_015a: Unknown result type (might be due to invalid IL or missing references)
				//IL_015f: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_016d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0172: Unknown result type (might be due to invalid IL or missing references)
				//IL_0193: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
				if (offset < 0f)
				{
					if (offset <= 0f - totalPrevDistance)
					{
						curvePos = 0f;
						if (prevLane != Entity.Null)
						{
							reference = prevLane;
						}
					}
					else if (offset <= 0f - prevDistance)
					{
						curvePos = 0f;
						offset += prevDistance;
						if (offset < 0f && prevLane != Entity.Null)
						{
							reference = prevLane;
							Bounds1 val2 = default(Bounds1);
							((Bounds1)(ref val2))._002Ector(0f, 1f);
							if (MathUtils.ClampLengthInverse(((Bezier4x3)(ref prevCurve.m_Bezier)).xz, ref val2, 0f - offset))
							{
								curvePos = val2.min;
							}
						}
					}
					else
					{
						Bounds1 val3 = default(Bounds1);
						((Bounds1)(ref val3))._002Ector(0f, curvePos);
						if (MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val3, 0f - offset))
						{
							curvePos = val3.min;
						}
						else
						{
							curvePos = 0f;
						}
					}
				}
				else if (offset > 0f)
				{
					if (offset >= totalNextDistance)
					{
						curvePos = 1f;
						if (nextLane != Entity.Null)
						{
							reference = nextLane;
						}
					}
					else if (offset >= nextDistance)
					{
						curvePos = 1f;
						offset -= nextDistance;
						if (offset > 0f && nextLane != Entity.Null)
						{
							reference = nextLane;
							Bounds1 val4 = default(Bounds1);
							((Bounds1)(ref val4))._002Ector(0f, 1f);
							if (MathUtils.ClampLength(((Bezier4x3)(ref nextCurve.m_Bezier)).xz, ref val4, offset))
							{
								curvePos = val4.max;
							}
						}
					}
					else
					{
						Bounds1 val5 = default(Bounds1);
						((Bounds1)(ref val5))._002Ector(curvePos, 1f);
						if (MathUtils.ClampLength(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val5, offset))
						{
							curvePos = val5.max;
						}
						else
						{
							curvePos = 1f;
						}
					}
				}
			}
		}

		private void FindLane(Entity laneContainer, float3 position, float elevation, RouteConnectionType connectionType, TrackTypes trackTypes, RoadTypes roadTypes, Entity ignoreOwner, Entity preferOwner, Entity masterLot, bool onGround, int shouldIntersectLot, out Entity laneOwner, out Entity lane, out float curvePos, out bool intersectLot)
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			if (connectionType == RouteConnectionType.Air)
			{
				laneOwner = Entity.Null;
				lane = Entity.Null;
				curvePos = 0f;
				intersectLot = false;
				float distance = float.MaxValue;
				if ((roadTypes & RoadTypes.Helicopter) != RoadTypes.None)
				{
					m_AirwayData.helicopterMap.FindClosestLane(position, m_CurveData, ref lane, ref curvePos, ref distance);
				}
				if ((roadTypes & RoadTypes.Airplane) != RoadTypes.None)
				{
					m_AirwayData.airplaneMap.FindClosestLane(position, m_CurveData, ref lane, ref curvePos, ref distance);
				}
				return;
			}
			if (laneContainer != Entity.Null && laneContainer != ignoreOwner)
			{
				DynamicBuffer<Game.Net.SubLane> val = m_Lanes[laneContainer];
				float num = float.MaxValue;
				laneOwner = laneContainer;
				lane = Entity.Null;
				curvePos = 0f;
				intersectLot = false;
				float num3 = default(float);
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					if (!m_ConnectionLaneData.HasComponent(subLane))
					{
						continue;
					}
					Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[subLane];
					switch (connectionType)
					{
					case RouteConnectionType.Pedestrian:
						if ((connectionLane.m_Flags & (ConnectionLaneFlags.Pedestrian | ConnectionLaneFlags.AllowMiddle)) != (ConnectionLaneFlags.Pedestrian | ConnectionLaneFlags.AllowMiddle))
						{
							continue;
						}
						break;
					case RouteConnectionType.Road:
						if ((connectionLane.m_Flags & (ConnectionLaneFlags.Road | ConnectionLaneFlags.AllowMiddle)) != (ConnectionLaneFlags.Road | ConnectionLaneFlags.AllowMiddle) || (connectionLane.m_RoadTypes & roadTypes) == 0)
						{
							continue;
						}
						break;
					case RouteConnectionType.Track:
						if ((connectionLane.m_Flags & (ConnectionLaneFlags.Track | ConnectionLaneFlags.AllowMiddle)) != (ConnectionLaneFlags.Track | ConnectionLaneFlags.AllowMiddle) || (connectionLane.m_TrackTypes & trackTypes) == 0)
						{
							continue;
						}
						break;
					case RouteConnectionType.Cargo:
						if ((connectionLane.m_Flags & (ConnectionLaneFlags.AllowMiddle | ConnectionLaneFlags.AllowCargo)) != (ConnectionLaneFlags.AllowMiddle | ConnectionLaneFlags.AllowCargo))
						{
							continue;
						}
						break;
					default:
						continue;
					}
					float num2 = MathUtils.Distance(m_CurveData[subLane].m_Bezier, position, ref num3);
					num3 = math.select(num3, 1f, (connectionLane.m_Flags & ConnectionLaneFlags.Start) != 0);
					if (num2 < num)
					{
						num = num2;
						lane = subLane;
						curvePos = num3;
					}
				}
				return;
			}
			float num4 = 10f;
			LaneIterator laneIterator = new LaneIterator
			{
				m_Bounds = new Bounds3(position - num4, position + num4),
				m_Position = position,
				m_ConnectionType = connectionType,
				m_TrackType = trackTypes,
				m_CarType = roadTypes,
				m_OnGround = onGround,
				m_CmpDistance = num4,
				m_MaxDistance = num4,
				m_Elevation = elevation,
				m_IgnoreOwner = ignoreOwner,
				m_IgnoreConnected = preferOwner,
				m_MasterLot = masterLot,
				m_OwnerData = m_OwnerData,
				m_PedestrianLaneData = m_PedestrianLaneData,
				m_CarLaneData = m_CarLaneData,
				m_TrackLaneData = m_TrackLaneData,
				m_MasterLaneData = m_MasterLaneData,
				m_SlaveLaneData = m_SlaveLaneData,
				m_ConnectionLaneData = m_ConnectionLaneData,
				m_CurveData = m_CurveData,
				m_EdgeData = m_EdgeData,
				m_ElevationData = m_NetElevationData,
				m_EdgeGeometryData = m_EdgeGeometryData,
				m_StartNodeGeometryData = m_StartNodeGeometryData,
				m_EndNodeGeometryData = m_EndNodeGeometryData,
				m_CompositionData = m_CompositionData,
				m_LotData = m_LotData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabTrackLaneData = m_PrefabTrackLaneData,
				m_PrefabCarLaneData = m_PrefabCarLaneData,
				m_PrefabNetLaneData = m_PrefabNetLaneData,
				m_PrefabNetData = m_PrefabNetData,
				m_PrefabNetCompositionData = m_PrefabNetCompositionData,
				m_Lanes = m_Lanes,
				m_ConnectedEdges = m_ConnectedEdges,
				m_AreaNodes = m_AreaNodes,
				m_AreaTriangles = m_AreaTriangles
			};
			Entity val2 = ignoreOwner;
			if (val2 != Entity.Null)
			{
				Owner owner = default(Owner);
				while (m_OwnerData.TryGetComponent(val2, ref owner) && !m_BuildingData.HasComponent(val2))
				{
					val2 = owner.m_Owner;
				}
			}
			float num5 = float.MaxValue;
			PrefabRef prefabRef = default(PrefabRef);
			Transform transform = default(Transform);
			BuildingData buildingData = default(BuildingData);
			if (val2 != Entity.Null && m_PrefabRefData.TryGetComponent(val2, ref prefabRef) && m_TransformData.TryGetComponent(val2, ref transform) && m_PrefabBuildingData.TryGetComponent(prefabRef.m_Prefab, ref buildingData))
			{
				float2 size = float2.op_Implicit(buildingData.m_LotSize) * 8f;
				Quad3 val3 = ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, size);
				laneIterator.m_LotQuad = ((Quad3)(ref val3)).xz;
				laneIterator.m_CheckLot = new bool4(true, (buildingData.m_Flags & Game.Prefabs.BuildingFlags.LeftAccess) != 0, (buildingData.m_Flags & Game.Prefabs.BuildingFlags.BackAccess) != 0, (buildingData.m_Flags & Game.Prefabs.BuildingFlags.RightAccess) != 0);
				float4 val4 = float4.op_Implicit(float.MaxValue);
				float num6 = default(float);
				if (laneIterator.m_CheckLot.x)
				{
					val4.x = MathUtils.Distance(((Quad2)(ref laneIterator.m_LotQuad)).ab, ((float3)(ref position)).xz, ref num6);
				}
				if (laneIterator.m_CheckLot.y)
				{
					val4.y = MathUtils.Distance(((Quad2)(ref laneIterator.m_LotQuad)).bc, ((float3)(ref position)).xz, ref num6);
				}
				if (laneIterator.m_CheckLot.z)
				{
					val4.z = MathUtils.Distance(((Quad2)(ref laneIterator.m_LotQuad)).cd, ((float3)(ref position)).xz, ref num6);
				}
				if (laneIterator.m_CheckLot.w)
				{
					val4.w = MathUtils.Distance(((Quad2)(ref laneIterator.m_LotQuad)).da, ((float3)(ref position)).xz, ref num6);
				}
				num5 = math.cmin(val4);
				ref bool4 reference = ref laneIterator.m_CheckLot;
				reference &= val4 <= num5 * 2f;
			}
			m_NetSearchTree.Iterate<LaneIterator>(ref laneIterator, 0);
			if (math.any(laneIterator.m_CheckLot))
			{
				laneIterator.m_Bounds = new Bounds3(position - laneIterator.m_CmpDistance, position + laneIterator.m_CmpDistance);
			}
			m_AreaSearchTree.Iterate<LaneIterator>(ref laneIterator, 0);
			if (shouldIntersectLot != 0 && math.any(laneIterator.m_CheckLot))
			{
				if (shouldIntersectLot == -1)
				{
					if (laneIterator.m_IntersectLot)
					{
						laneIterator = default(LaneIterator);
					}
				}
				else if (!laneIterator.m_IntersectLot && laneIterator.m_MaxDistance > num5)
				{
					laneIterator = default(LaneIterator);
				}
			}
			laneOwner = laneIterator.m_ResultOwner;
			lane = laneIterator.m_ResultLane;
			curvePos = laneIterator.m_ResultCurvePos;
			intersectLot = laneIterator.m_IntersectLot;
		}
	}

	private struct PathTargetInfo
	{
		public Entity m_Segment;

		public bool m_Start;
	}

	[BurstCompile]
	private struct ClearPathTargetsJob : IJob
	{
		public NativeQueue<PathTargetInfo> m_PathTargetInfo;

		public ComponentLookup<PathTargets> m_PathTargetsData;

		public void Execute()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			int count = m_PathTargetInfo.Count;
			for (int i = 0; i < count; i++)
			{
				PathTargetInfo pathTargetInfo = m_PathTargetInfo.Dequeue();
				if (m_PathTargetsData.HasComponent(pathTargetInfo.m_Segment))
				{
					PathTargets pathTargets = m_PathTargetsData[pathTargetInfo.m_Segment];
					if (pathTargetInfo.m_Start)
					{
						pathTargets.m_StartLane = Entity.Null;
						pathTargets.m_CurvePositions.x = 0f;
					}
					else
					{
						pathTargets.m_EndLane = Entity.Null;
						pathTargets.m_CurvePositions.y = 0f;
					}
					m_PathTargetsData[pathTargetInfo.m_Segment] = pathTargets;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Connected> __Game_Routes_Connected_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AccessLane> __Game_Routes_AccessLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RouteLane> __Game_Routes_RouteLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		public BufferLookup<ConnectedRoute> __Game_Routes_ConnectedRoute_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Waypoint> __Game_Routes_Waypoint_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AccessLane> __Game_Routes_AccessLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteLane> __Game_Routes_RouteLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> __Game_Prefabs_RouteConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.TrackLane> __Game_Net_TrackLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.OutsideConnection> __Game_Net_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> __Game_Areas_Lot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Surface> __Game_Areas_Surface_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteSegment> __Game_Routes_RouteSegment_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		public ComponentLookup<AccessLane> __Game_Routes_AccessLane_RW_ComponentLookup;

		public ComponentLookup<RouteLane> __Game_Routes_RouteLane_RW_ComponentLookup;

		public ComponentLookup<Connected> __Game_Routes_Connected_RW_ComponentLookup;

		public ComponentLookup<Position> __Game_Routes_Position_RW_ComponentLookup;

		public ComponentLookup<PathTargets> __Game_Routes_PathTargets_RW_ComponentLookup;

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
			__Game_Routes_Connected_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Connected>(true);
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Routes_AccessLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AccessLane>(true);
			__Game_Routes_RouteLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RouteLane>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Routes_ConnectedRoute_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedRoute>(false);
			__Game_Routes_Waypoint_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Waypoint>(true);
			__Game_Routes_AccessLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccessLane>(true);
			__Game_Routes_RouteLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteLane>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_RouteConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteConnectionData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Edge>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.PedestrianLane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.TrackLane>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.OutsideConnection>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Elevation>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Areas_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Areas.Lot>(true);
			__Game_Areas_Surface_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Areas.Surface>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Routes_RouteSegment_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteSegment>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Routes_AccessLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccessLane>(false);
			__Game_Routes_RouteLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteLane>(false);
			__Game_Routes_Connected_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(false);
			__Game_Routes_Position_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(false);
			__Game_Routes_PathTargets_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathTargets>(false);
		}
	}

	private ModificationBarrier5 m_ModificationBarrier;

	private Game.Net.UpdateCollectSystem m_NetUpdateCollectSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private AirwaySystem m_AirwaySystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Game.Areas.UpdateCollectSystem m_AreaUpdateCollectSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private SearchSystem m_RouteSearchSystem;

	private EntityQuery m_WaypointQuery;

	private EntityArchetype m_PathTargetEventArchetype;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_NetUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.UpdateCollectSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_AirwaySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirwaySystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_AreaUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.UpdateCollectSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_RouteSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Waypoint>(),
			ComponentType.ReadOnly<AccessLane>(),
			ComponentType.ReadOnly<RouteLane>(),
			ComponentType.ReadOnly<ConnectedRoute>()
		};
		val.None = (ComponentType[])(object)new ComponentType[0];
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Waypoint>(),
			ComponentType.ReadOnly<AccessLane>(),
			ComponentType.ReadOnly<RouteLane>(),
			ComponentType.ReadOnly<ConnectedRoute>()
		};
		val.None = (ComponentType[])(object)new ComponentType[0];
		array[1] = val;
		m_WaypointQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_PathTargetEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<PathTargetMoved>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0836: Unknown result type (might be due to invalid IL or missing references)
		//IL_083b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0853: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_0870: Unknown result type (might be due to invalid IL or missing references)
		//IL_0875: Unknown result type (might be due to invalid IL or missing references)
		//IL_088d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08af: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0901: Unknown result type (might be due to invalid IL or missing references)
		//IL_0906: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0923: Unknown result type (might be due to invalid IL or missing references)
		//IL_093b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0940: Unknown result type (might be due to invalid IL or missing references)
		//IL_0958: Unknown result type (might be due to invalid IL or missing references)
		//IL_095d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0975: Unknown result type (might be due to invalid IL or missing references)
		//IL_097a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0983: Unknown result type (might be due to invalid IL or missing references)
		//IL_0988: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09af: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09da: Unknown result type (might be due to invalid IL or missing references)
		//IL_09df: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a97: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_WaypointQuery)).IsEmptyIgnoreFilter || m_NetUpdateCollectSystem.netsUpdated || m_AreaUpdateCollectSystem.lotsUpdated)
		{
			NativeList<Entity> val = default(NativeList<Entity>);
			val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle val2 = default(JobHandle);
			if (!((EntityQuery)(ref m_WaypointQuery)).IsEmptyIgnoreFilter)
			{
				val2 = JobChunkExtensions.Schedule<UpdateWaypointReferencesJob>(new UpdateWaypointReferencesJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectedType = InternalCompilerInterface.GetComponentTypeHandle<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_AccessLaneType = InternalCompilerInterface.GetComponentTypeHandle<AccessLane>(ref __TypeHandle.__Game_Routes_AccessLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_RouteLaneType = InternalCompilerInterface.GetComponentTypeHandle<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectedRoutes = InternalCompilerInterface.GetBufferLookup<ConnectedRoute>(ref __TypeHandle.__Game_Routes_ConnectedRoute_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_UpdatedList = val
				}, m_WaypointQuery, ((SystemBase)this).Dependency);
			}
			JobHandle val3 = val2;
			if (m_NetUpdateCollectSystem.netsUpdated)
			{
				NativeQueue<Entity> updatedQueue = default(NativeQueue<Entity>);
				updatedQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
				JobHandle dependencies;
				NativeList<Bounds2> updatedNetBounds = m_NetUpdateCollectSystem.GetUpdatedNetBounds(out dependencies);
				JobHandle dependencies2;
				JobHandle dependencies3;
				FindUpdatedWaypointsJob findUpdatedWaypointsJob = new FindUpdatedWaypointsJob
				{
					m_Bounds = updatedNetBounds.AsDeferredJobArray(),
					m_RouteSearchTree = m_RouteSearchSystem.GetSearchTree(readOnly: true, out dependencies2),
					m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies3),
					m_WaypointData = InternalCompilerInterface.GetComponentLookup<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_AccessLaneData = InternalCompilerInterface.GetComponentLookup<AccessLane>(ref __TypeHandle.__Game_Routes_AccessLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_RouteLaneData = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ResultQueue = updatedQueue.AsParallelWriter()
				};
				DequeUpdatedWaypointsJob obj = new DequeUpdatedWaypointsJob
				{
					m_UpdatedQueue = updatedQueue,
					m_UpdatedList = val
				};
				JobHandle val4 = JobHandle.CombineDependencies(dependencies, dependencies2, dependencies3);
				JobHandle val5 = IJobParallelForDeferExtensions.Schedule<FindUpdatedWaypointsJob, Bounds2>(findUpdatedWaypointsJob, updatedNetBounds, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val4));
				val3 = IJobExtensions.Schedule<DequeUpdatedWaypointsJob>(obj, JobHandle.CombineDependencies(val3, val5));
				updatedQueue.Dispose(val3);
				m_NetUpdateCollectSystem.AddNetBoundsReader(val5);
				m_RouteSearchSystem.AddSearchTreeReader(val5);
				m_ObjectSearchSystem.AddStaticSearchTreeReader(val5);
			}
			if (m_AreaUpdateCollectSystem.lotsUpdated)
			{
				NativeQueue<Entity> updatedQueue2 = default(NativeQueue<Entity>);
				updatedQueue2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
				JobHandle dependencies4;
				NativeList<Bounds2> updatedLotBounds = m_AreaUpdateCollectSystem.GetUpdatedLotBounds(out dependencies4);
				JobHandle dependencies5;
				JobHandle dependencies6;
				FindUpdatedWaypointsJob findUpdatedWaypointsJob2 = new FindUpdatedWaypointsJob
				{
					m_Bounds = updatedLotBounds.AsDeferredJobArray(),
					m_RouteSearchTree = m_RouteSearchSystem.GetSearchTree(readOnly: true, out dependencies5),
					m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies6),
					m_WaypointData = InternalCompilerInterface.GetComponentLookup<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_AccessLaneData = InternalCompilerInterface.GetComponentLookup<AccessLane>(ref __TypeHandle.__Game_Routes_AccessLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_RouteLaneData = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ResultQueue = updatedQueue2.AsParallelWriter()
				};
				DequeUpdatedWaypointsJob obj2 = new DequeUpdatedWaypointsJob
				{
					m_UpdatedQueue = updatedQueue2,
					m_UpdatedList = val
				};
				JobHandle val6 = JobHandle.CombineDependencies(dependencies4, dependencies5, dependencies6);
				JobHandle val7 = IJobParallelForDeferExtensions.Schedule<FindUpdatedWaypointsJob, Bounds2>(findUpdatedWaypointsJob2, updatedLotBounds, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val6));
				val3 = IJobExtensions.Schedule<DequeUpdatedWaypointsJob>(obj2, JobHandle.CombineDependencies(val3, val7));
				updatedQueue2.Dispose(val3);
				m_AreaUpdateCollectSystem.AddLotBoundsReader(val7);
				m_RouteSearchSystem.AddSearchTreeReader(val7);
				m_ObjectSearchSystem.AddStaticSearchTreeReader(val7);
			}
			NativeQueue<PathTargetInfo> pathTargetInfo = default(NativeQueue<PathTargetInfo>);
			pathTargetInfo._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			RemoveDuplicatedWaypointsJob removeDuplicatedWaypointsJob = new RemoveDuplicatedWaypointsJob
			{
				m_UpdatedList = val
			};
			JobHandle dependencies7;
			JobHandle dependencies8;
			FindWaypointConnectionsJob findWaypointConnectionsJob = new FindWaypointConnectionsJob
			{
				m_WaypointData = InternalCompilerInterface.GetComponentLookup<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabConnectionData = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TrackLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetOutsideConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Net.OutsideConnection>(ref __TypeHandle.__Game_Net_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectOutsideConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LotData = InternalCompilerInterface.GetComponentLookup<Game.Areas.Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SurfaceData = InternalCompilerInterface.GetComponentLookup<Game.Areas.Surface>(ref __TypeHandle.__Game_Areas_Surface_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Segments = InternalCompilerInterface.GetBufferLookup<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Lanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AccessLaneData = InternalCompilerInterface.GetComponentLookup<AccessLane>(ref __TypeHandle.__Game_Routes_AccessLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteLaneData = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedList = val.AsDeferredJobArray(),
				m_AirwayData = m_AirwaySystem.GetAirwayData(),
				m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies7),
				m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies8),
				m_PathTargetEventArchetype = m_PathTargetEventArchetype,
				m_PathTargetInfo = pathTargetInfo.AsParallelWriter()
			};
			EntityCommandBuffer val8 = m_ModificationBarrier.CreateCommandBuffer();
			findWaypointConnectionsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val8)).AsParallelWriter();
			FindWaypointConnectionsJob findWaypointConnectionsJob2 = findWaypointConnectionsJob;
			ClearPathTargetsJob obj3 = new ClearPathTargetsJob
			{
				m_PathTargetInfo = pathTargetInfo,
				m_PathTargetsData = InternalCompilerInterface.GetComponentLookup<PathTargets>(ref __TypeHandle.__Game_Routes_PathTargets_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			JobHandle val9 = IJobExtensions.Schedule<RemoveDuplicatedWaypointsJob>(removeDuplicatedWaypointsJob, val3);
			JobHandle val10 = IJobParallelForDeferExtensions.Schedule<FindWaypointConnectionsJob, Entity>(findWaypointConnectionsJob2, val, 1, JobHandle.CombineDependencies(val9, dependencies7, dependencies8));
			JobHandle val11 = IJobExtensions.Schedule<ClearPathTargetsJob>(obj3, val10);
			val.Dispose(val10);
			pathTargetInfo.Dispose(val11);
			m_NetSearchSystem.AddNetSearchTreeReader(val10);
			m_AreaSearchSystem.AddSearchTreeReader(val10);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val10);
			((SystemBase)this).Dependency = val11;
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
	public WaypointConnectionSystem()
	{
	}
}

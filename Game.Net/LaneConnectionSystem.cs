using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class LaneConnectionSystem : GameSystemBase
{
	[BurstCompile]
	private struct FindUpdatedLanesJob : IJobParallelForDefer
	{
		private struct Iterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<Curve> m_CurveData;

			public BufferLookup<SubLane> m_SubLanes;

			public ParallelWriter<Entity> m_ResultQueue;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) || !m_SubLanes.HasBuffer(entity))
				{
					return;
				}
				DynamicBuffer<SubLane> val = m_SubLanes[entity];
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					Curve curve = m_CurveData[subLane];
					if (MathUtils.Intersect(MathUtils.Bounds(((Bezier4x3)(ref curve.m_Bezier)).xz), m_Bounds))
					{
						m_ResultQueue.Enqueue(entity);
					}
				}
			}
		}

		[ReadOnly]
		public NativeArray<Bounds2> m_Bounds;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		public ParallelWriter<Entity> m_ResultQueue;

		public void Execute(int index)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			Iterator iterator = new Iterator
			{
				m_Bounds = m_Bounds[index],
				m_CurveData = m_CurveData,
				m_SubLanes = m_SubLanes,
				m_ResultQueue = m_ResultQueue
			};
			m_NetSearchTree.Iterate<Iterator>(ref iterator, 0);
		}
	}

	[BurstCompile]
	private struct CheckUpdatedLanesJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public BufferTypeHandle<SubLane> m_SubLaneType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> m_SpawnLocations;

		public NativeQueue<Entity> m_ResultQueue;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			NativeHashSet<Entity> val = default(NativeHashSet<Entity>);
			val._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			Owner owner = default(Owner);
			Owner owner2 = default(Owner);
			DynamicBuffer<SpawnLocationElement> val4 = default(DynamicBuffer<SpawnLocationElement>);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val2 = m_Chunks[i];
				NativeArray<Owner> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<Owner>(ref m_OwnerType);
				BufferAccessor<SubLane> bufferAccessor = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<SubLane>(ref m_SubLaneType);
				for (int j = 0; j < bufferAccessor.Length; j++)
				{
					DynamicBuffer<SubLane> val3 = bufferAccessor[j];
					for (int k = 0; k < val3.Length; k++)
					{
						m_ResultQueue.Enqueue(val3[k].m_SubLane);
					}
					if (!CollectionUtils.TryGet<Owner>(nativeArray, j, ref owner))
					{
						continue;
					}
					while (m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2))
					{
						owner = owner2;
					}
					if (!m_SpawnLocations.TryGetBuffer(owner.m_Owner, ref val4) || !val.Add(owner.m_Owner))
					{
						continue;
					}
					for (int l = 0; l < val4.Length; l++)
					{
						SpawnLocationElement spawnLocationElement = val4[l];
						if (spawnLocationElement.m_Type == SpawnLocationType.ParkingLane)
						{
							m_ResultQueue.Enqueue(spawnLocationElement.m_SpawnLocation);
						}
					}
				}
			}
			val.Dispose();
		}
	}

	[BurstCompile]
	private struct ListUpdatedLanesJob : IJob
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

		public NativeQueue<Entity> m_UpdatedQueue1;

		public NativeQueue<Entity> m_UpdatedQueue2;

		public NativeQueue<Entity> m_UpdatedQueue3;

		public NativeList<Entity> m_UpdatedList;

		public void Execute()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			int count = m_UpdatedQueue1.Count;
			int num = count + m_UpdatedQueue2.Count;
			int num2 = num + m_UpdatedQueue3.Count;
			m_UpdatedList.ResizeUninitialized(num2);
			for (int i = 0; i < count; i++)
			{
				m_UpdatedList[i] = m_UpdatedQueue1.Dequeue();
			}
			for (int j = count; j < num; j++)
			{
				m_UpdatedList[j] = m_UpdatedQueue2.Dequeue();
			}
			for (int k = num; k < num2; k++)
			{
				m_UpdatedList[k] = m_UpdatedQueue3.Dequeue();
			}
			NativeSortExtension.Sort<Entity, EntityComparer>(m_UpdatedList, default(EntityComparer));
			Entity val = Entity.Null;
			int num3 = 0;
			int num4 = 0;
			while (num3 < m_UpdatedList.Length)
			{
				Entity val2 = m_UpdatedList[num3++];
				if (val2 != val)
				{
					m_UpdatedList[num4++] = val2;
					val = val2;
				}
			}
			if (num4 < m_UpdatedList.Length)
			{
				m_UpdatedList.RemoveRangeSwapBack(num4, m_UpdatedList.Length - num4);
			}
		}
	}

	[BurstCompile]
	private struct FindLaneConnectionJob : IJobParallelForDefer
	{
		private struct Iterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public Curve m_Curve;

			public float2 m_MaxDistance;

			public ConnectionLaneFlags m_LaneFlags;

			public RoadTypes m_RoadTypes;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<ConnectionLane> m_ConnectionLaneData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<NavigationAreaData> m_PrefabNavigationAreaData;

			public BufferLookup<SubLane> m_Lanes;

			public BufferLookup<Game.Areas.Node> m_AreaNodes;

			public BufferLookup<Triangle> m_AreaTriangles;

			public LaneConnection m_BestConnection;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((float3)(ref m_Curve.m_Bezier.a)).xz) | MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((float3)(ref m_Curve.m_Bezier.d)).xz);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem item)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0117: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_0173: Unknown result type (might be due to invalid IL or missing references)
				//IL_0176: Unknown result type (might be due to invalid IL or missing references)
				//IL_017b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0180: Unknown result type (might be due to invalid IL or missing references)
				//IL_0181: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_0191: Unknown result type (might be due to invalid IL or missing references)
				//IL_0196: Unknown result type (might be due to invalid IL or missing references)
				//IL_019b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0146: Unknown result type (might be due to invalid IL or missing references)
				//IL_0147: Unknown result type (might be due to invalid IL or missing references)
				//IL_0149: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_0323: Unknown result type (might be due to invalid IL or missing references)
				//IL_0328: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_036c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0371: Unknown result type (might be due to invalid IL or missing references)
				//IL_033a: Unknown result type (might be due to invalid IL or missing references)
				//IL_034e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0353: Unknown result type (might be due to invalid IL or missing references)
				//IL_0383: Unknown result type (might be due to invalid IL or missing references)
				//IL_0397: Unknown result type (might be due to invalid IL or missing references)
				//IL_039c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0223: Unknown result type (might be due to invalid IL or missing references)
				//IL_022e: Unknown result type (might be due to invalid IL or missing references)
				//IL_023f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0271: Unknown result type (might be due to invalid IL or missing references)
				//IL_024f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0260: Unknown result type (might be due to invalid IL or missing references)
				//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_027b: Unknown result type (might be due to invalid IL or missing references)
				//IL_028b: Unknown result type (might be due to invalid IL or missing references)
				//IL_029b: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02af: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
				bool2 val = default(bool2);
				val.x = MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((float3)(ref m_Curve.m_Bezier.a)).xz);
				val.y = MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((float3)(ref m_Curve.m_Bezier.d)).xz);
				if (!math.any(val) || !m_Lanes.HasBuffer(item.m_Area))
				{
					return;
				}
				DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[item.m_Area];
				Triangle triangle = m_AreaTriangles[item.m_Area][item.m_Triangle];
				Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
				float2 val2 = float2.op_Implicit(float.MaxValue);
				float2 val3 = default(float2);
				if (val.x && MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref m_Curve.m_Bezier.a)).xz, ref val3))
				{
					val2.x = math.abs(MathUtils.Position(triangle2, val3).y - m_Curve.m_Bezier.a.y);
				}
				float2 val4 = default(float2);
				if (val.y && MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref m_Curve.m_Bezier.d)).xz, ref val4))
				{
					val2.y = math.abs(MathUtils.Position(triangle2, val4).y - m_Curve.m_Bezier.d.y);
				}
				val = val2 < m_MaxDistance;
				if (!math.any(val))
				{
					return;
				}
				DynamicBuffer<SubLane> val5 = m_Lanes[item.m_Area];
				float2 val6 = float2.op_Implicit(float.MaxValue);
				LaneConnection laneConnection = default(LaneConnection);
				float2 val7 = default(float2);
				float startPosition = default(float);
				float endPosition = default(float);
				for (int i = 0; i < val5.Length; i++)
				{
					Entity subLane = val5[i].m_SubLane;
					if (!m_ConnectionLaneData.HasComponent(subLane))
					{
						continue;
					}
					ConnectionLane connectionLane = m_ConnectionLaneData[subLane];
					if ((connectionLane.m_Flags & m_LaneFlags) == 0 || ((m_RoadTypes != RoadTypes.None) & ((connectionLane.m_RoadTypes & m_RoadTypes) == 0)))
					{
						continue;
					}
					Curve curve = m_CurveData[subLane];
					if (!MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.a)).xz, ref val7) && !MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.d)).xz, ref val7))
					{
						continue;
					}
					if (val.x)
					{
						float num = MathUtils.Distance(curve.m_Bezier, m_Curve.m_Bezier.a, ref startPosition);
						if (num < val6.x)
						{
							val6.x = num;
							laneConnection.m_StartLane = subLane;
							laneConnection.m_StartPosition = startPosition;
						}
					}
					if (val.y)
					{
						float num2 = MathUtils.Distance(curve.m_Bezier, m_Curve.m_Bezier.d, ref endPosition);
						if (num2 < val6.y)
						{
							val6.y = num2;
							laneConnection.m_EndLane = subLane;
							laneConnection.m_EndPosition = endPosition;
						}
					}
				}
				if (laneConnection.m_StartLane != Entity.Null)
				{
					m_MaxDistance.x = val2.x;
					m_BestConnection.m_StartLane = laneConnection.m_StartLane;
					m_BestConnection.m_StartPosition = laneConnection.m_StartPosition;
				}
				if (laneConnection.m_EndLane != Entity.Null)
				{
					m_MaxDistance.y = val2.y;
					m_BestConnection.m_EndLane = laneConnection.m_EndLane;
					m_BestConnection.m_EndPosition = laneConnection.m_EndPosition;
				}
			}
		}

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

		[ReadOnly]
		public ComponentLookup<NavigationAreaData> m_PrefabNavigationAreaData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<LaneConnection> m_LaneConnectionData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public BufferLookup<SubLane> m_Lanes;

		[ReadOnly]
		public BufferLookup<SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			LaneConnection laneConnection = FindLaneConnection(val);
			if (laneConnection.m_StartLane == Entity.Null && laneConnection.m_EndLane == Entity.Null)
			{
				if (m_LaneConnectionData.HasComponent(val))
				{
					if (!m_UpdatedData.HasComponent(val))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(index, val, default(PathfindUpdated));
					}
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<LaneConnection>(index, val);
				}
			}
			else if (m_LaneConnectionData.HasComponent(val))
			{
				LaneConnection laneConnection2 = m_LaneConnectionData[val];
				if (laneConnection2.m_StartLane != laneConnection.m_StartLane || laneConnection2.m_EndLane != laneConnection.m_EndLane || laneConnection2.m_StartPosition != laneConnection.m_StartPosition || laneConnection2.m_EndPosition != laneConnection.m_EndPosition)
				{
					m_LaneConnectionData[val] = laneConnection;
					if (!m_UpdatedData.HasComponent(val))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(index, val, default(PathfindUpdated));
					}
				}
			}
			else
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LaneConnection>(index, val, laneConnection);
				if (!m_UpdatedData.HasComponent(val))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(index, val, default(PathfindUpdated));
				}
			}
		}

		public LaneConnection FindLaneConnection(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			if (m_SlaveLaneData.HasComponent(entity) || m_ConnectionLaneData.HasComponent(entity))
			{
				return default(LaneConnection);
			}
			PrefabRef prefabRef = m_PrefabRefData[entity];
			if (!m_PrefabNetLaneData.HasComponent(prefabRef.m_Prefab))
			{
				return default(LaneConnection);
			}
			NetLaneData netLaneData = m_PrefabNetLaneData[prefabRef.m_Prefab];
			if ((netLaneData.m_Flags & LaneFlags.Parking) != 0 && m_ParkingLaneData.HasComponent(entity))
			{
				ParkingLane parkingLane = m_ParkingLaneData[entity];
				LaneConnection result = default(LaneConnection);
				if ((parkingLane.m_Flags & ParkingLaneFlags.FindConnections) != 0)
				{
					parkingLane.m_Flags &= ~(ParkingLaneFlags.SecondaryStart | ParkingLaneFlags.ParkingInverted);
					parkingLane.m_SecondaryStartNode = default(PathNode);
					result = FindParkingConnection(entity, ref parkingLane, prefabRef, netLaneData);
					m_ParkingLaneData[entity] = parkingLane;
				}
				return result;
			}
			if ((netLaneData.m_Flags & (LaneFlags.Road | LaneFlags.Pedestrian)) != 0)
			{
				return FindAreaConnection(entity, prefabRef, netLaneData);
			}
			return default(LaneConnection);
		}

		private LaneConnection FindParkingConnection(Entity entity, ref ParkingLane parkingLane, PrefabRef prefabRef, NetLaneData netLaneData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			Curve curve = m_CurveData[entity];
			Owner owner = m_OwnerData[entity];
			while (m_OwnerData.HasComponent(owner.m_Owner) && !m_BuildingData.HasComponent(owner.m_Owner))
			{
				owner = m_OwnerData[owner.m_Owner];
			}
			LaneConnection result = default(LaneConnection);
			float3 val = MathUtils.Position(curve.m_Bezier, 0.5f);
			float bestPedestrianDistance = float.MaxValue;
			float bestRoadDistance = float.MaxValue;
			bool bestRoadInverted = false;
			float3 val2 = val;
			float3 roadSearchPosition = val;
			float3 val3 = MathUtils.Tangent(curve.m_Bezier, 0.5f);
			float2 xz = ((float3)(ref val3)).xz;
			ParkingLaneData parkingLaneData = default(ParkingLaneData);
			if (MathUtils.TryNormalize(ref xz) && m_PrefabParkingLaneData.TryGetComponent(prefabRef.m_Prefab, ref parkingLaneData))
			{
				float2 val4 = MathUtils.RotateRight(xz, parkingLaneData.m_SlotAngle) * (parkingLaneData.m_SlotSize.y * 0.5f);
				((float3)(ref val2)).xz = ((float3)(ref val2)).xz - val4;
				((float3)(ref roadSearchPosition)).xz = math.select(((float3)(ref val2)).xz, ((float3)(ref roadSearchPosition)).xz + val4, parkingLaneData.m_SlotAngle < 0.01f);
			}
			FindParkingConnection(owner.m_Owner, val, val2, roadSearchPosition, ref result, ref bestPedestrianDistance, ref bestRoadDistance, ref bestRoadInverted);
			DynamicBuffer<InstalledUpgrade> val5 = default(DynamicBuffer<InstalledUpgrade>);
			if (m_InstalledUpgrades.TryGetBuffer(owner.m_Owner, ref val5))
			{
				for (int i = 0; i < val5.Length; i++)
				{
					FindParkingConnection(val5[i].m_Upgrade, val, val2, roadSearchPosition, ref result, ref bestPedestrianDistance, ref bestRoadDistance, ref bestRoadInverted);
				}
			}
			if ((netLaneData.m_Flags & LaneFlags.Twoway) != 0 && result.m_StartLane != Entity.Null)
			{
				Owner owner2 = m_OwnerData[result.m_StartLane];
				CarLane carLane = m_CarLaneData[result.m_StartLane];
				DynamicBuffer<SubLane> val6 = m_Lanes[owner2.m_Owner];
				float curvePosition = default(float);
				for (int j = 0; j < val6.Length; j++)
				{
					Entity subLane = val6[j].m_SubLane;
					if (!m_SlaveLaneData.HasComponent(subLane) && !m_ConnectionLaneData.HasComponent(subLane) && m_CarLaneData.HasComponent(subLane))
					{
						CarLane carLane2 = m_CarLaneData[subLane];
						if (((carLane.m_Flags ^ carLane2.m_Flags) & CarLaneFlags.Invert) != 0 && carLane.m_CarriagewayGroup == carLane2.m_CarriagewayGroup)
						{
							Lane lane = m_LaneData[subLane];
							MathUtils.Distance(m_CurveData[subLane].m_Bezier, val2, ref curvePosition);
							parkingLane.m_SecondaryStartNode = new PathNode(lane.m_MiddleNode, curvePosition);
							parkingLane.m_Flags |= ParkingLaneFlags.SecondaryStart;
							break;
						}
					}
				}
			}
			if (bestRoadInverted)
			{
				parkingLane.m_Flags |= ParkingLaneFlags.ParkingInverted;
			}
			return result;
		}

		private void FindParkingConnection(Entity owner, float3 pedestrianSearchPosition, float3 roadSearchPosition, float3 roadSearchPosition2, ref LaneConnection result, ref float bestPedestrianDistance, ref float bestRoadDistance, ref bool bestRoadInverted)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			bool flag = !((float3)(ref roadSearchPosition)).Equals(roadSearchPosition2);
			DynamicBuffer<SubNet> val = default(DynamicBuffer<SubNet>);
			if (m_SubNets.TryGetBuffer(owner, ref val))
			{
				float endPosition = default(float);
				float num3 = default(float);
				float num4 = default(float);
				for (int i = 0; i < val.Length; i++)
				{
					Entity subNet = val[i].m_SubNet;
					if (!m_Lanes.HasBuffer(subNet))
					{
						continue;
					}
					DynamicBuffer<SubLane> val2 = m_Lanes[subNet];
					for (int j = 0; j < val2.Length; j++)
					{
						Entity subLane = val2[j].m_SubLane;
						if (m_SlaveLaneData.HasComponent(subLane) || m_ConnectionLaneData.HasComponent(subLane))
						{
							continue;
						}
						Curve curve = m_CurveData[subLane];
						PrefabRef prefabRef = m_PrefabRefData[subLane];
						NetLaneData netLaneData = m_PrefabNetLaneData[prefabRef.m_Prefab];
						if ((netLaneData.m_Flags & LaneFlags.Pedestrian) != 0)
						{
							float num = MathUtils.Distance(MathUtils.Bounds(curve.m_Bezier), pedestrianSearchPosition);
							if (!(num >= bestPedestrianDistance))
							{
								num = MathUtils.Distance(curve.m_Bezier, pedestrianSearchPosition, ref endPosition);
								if (!(num >= bestPedestrianDistance))
								{
									bestPedestrianDistance = num;
									result.m_EndLane = subLane;
									result.m_EndPosition = endPosition;
								}
							}
						}
						else
						{
							if ((netLaneData.m_Flags & LaneFlags.Road) == 0 || (m_PrefabCarLaneData[prefabRef.m_Prefab].m_RoadTypes & RoadTypes.Car) == 0)
							{
								continue;
							}
							float num2 = MathUtils.Distance(MathUtils.Bounds(curve.m_Bezier), roadSearchPosition);
							if (num2 < bestRoadDistance)
							{
								num2 = MathUtils.Distance(curve.m_Bezier, roadSearchPosition, ref num3);
								if (flag)
								{
									float3 val3 = MathUtils.Tangent(curve.m_Bezier, num3);
									num2 += math.select(0f, num2, math.dot(val3, roadSearchPosition2 - roadSearchPosition) < 0f);
								}
								if (num2 < bestRoadDistance)
								{
									bestRoadDistance = num2;
									bestRoadInverted = false;
									result.m_StartLane = subLane;
									result.m_StartPosition = num3;
								}
							}
							if (!flag)
							{
								continue;
							}
							num2 = MathUtils.Distance(MathUtils.Bounds(curve.m_Bezier), roadSearchPosition2);
							if (num2 < bestRoadDistance)
							{
								num2 = MathUtils.Distance(curve.m_Bezier, roadSearchPosition2, ref num4);
								float3 val4 = MathUtils.Tangent(curve.m_Bezier, num4);
								num2 += math.select(0f, num2, math.dot(val4, roadSearchPosition - roadSearchPosition2) < 0f);
								if (num2 < bestRoadDistance)
								{
									bestRoadDistance = num2;
									bestRoadInverted = true;
									result.m_StartLane = subLane;
									result.m_StartPosition = num4;
								}
							}
						}
					}
				}
			}
			DynamicBuffer<Game.Areas.SubArea> val5 = default(DynamicBuffer<Game.Areas.SubArea>);
			if (!m_SubAreas.TryGetBuffer(owner, ref val5))
			{
				return;
			}
			float2 val11 = default(float2);
			float2 val12 = default(float2);
			float2 val14 = default(float2);
			float endPosition2 = default(float);
			float startPosition = default(float);
			for (int k = 0; k < val5.Length; k++)
			{
				Entity area = val5[k].m_Area;
				if (!m_Lanes.HasBuffer(area))
				{
					continue;
				}
				DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[area];
				DynamicBuffer<Triangle> val6 = m_AreaTriangles[area];
				float num5 = bestPedestrianDistance;
				float num6 = bestRoadDistance;
				Triangle3 val7 = default(Triangle3);
				Triangle3 val8 = default(Triangle3);
				float3 val9 = default(float3);
				float3 val10 = default(float3);
				bool flag2 = false;
				for (int l = 0; l < val6.Length; l++)
				{
					Triangle triangle = val6[l];
					Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
					float num7 = MathUtils.Distance(triangle2, pedestrianSearchPosition, ref val11);
					float num8 = MathUtils.Distance(triangle2, roadSearchPosition, ref val12);
					if (num7 < num5)
					{
						num5 = num7;
						val7 = triangle2;
						val9 = MathUtils.Position(triangle2, val11);
						flag2 = true;
					}
					if (num8 < num6)
					{
						num6 = num8;
						val8 = triangle2;
						val10 = MathUtils.Position(triangle2, val12);
						flag2 = true;
					}
				}
				if (!flag2)
				{
					continue;
				}
				DynamicBuffer<SubLane> val13 = m_Lanes[area];
				float num9 = float.MaxValue;
				float num10 = float.MaxValue;
				for (int m = 0; m < val13.Length; m++)
				{
					Entity subLane2 = val13[m].m_SubLane;
					if (!m_ConnectionLaneData.HasComponent(subLane2))
					{
						continue;
					}
					ConnectionLane connectionLane = m_ConnectionLaneData[subLane2];
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0)
					{
						if (num5 >= bestPedestrianDistance)
						{
							continue;
						}
					}
					else if ((connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0 && (num6 >= bestRoadDistance || (connectionLane.m_RoadTypes & RoadTypes.Car) == 0))
					{
						continue;
					}
					Curve curve2 = m_CurveData[subLane2];
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0)
					{
						if (MathUtils.Intersect(((Triangle3)(ref val7)).xz, ((float3)(ref curve2.m_Bezier.a)).xz, ref val14) || MathUtils.Intersect(((Triangle3)(ref val7)).xz, ((float3)(ref curve2.m_Bezier.d)).xz, ref val14))
						{
							float num11 = MathUtils.Distance(curve2.m_Bezier, val9, ref endPosition2);
							if (num11 < num9)
							{
								num9 = num11;
								result.m_EndLane = subLane2;
								result.m_EndPosition = endPosition2;
							}
						}
					}
					else if ((connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0 && (MathUtils.Intersect(((Triangle3)(ref val8)).xz, ((float3)(ref curve2.m_Bezier.a)).xz, ref val14) || MathUtils.Intersect(((Triangle3)(ref val8)).xz, ((float3)(ref curve2.m_Bezier.d)).xz, ref val14)))
					{
						float num12 = MathUtils.Distance(curve2.m_Bezier, val10, ref startPosition);
						if (num12 < num10)
						{
							num10 = num12;
							result.m_StartLane = subLane2;
							result.m_StartPosition = startPosition;
						}
					}
				}
				if (num9 != float.MaxValue)
				{
					bestPedestrianDistance = num5;
				}
				if (num10 != float.MaxValue)
				{
					bestRoadDistance = num6;
				}
			}
		}

		private LaneConnection FindAreaConnection(Entity entity, PrefabRef prefabRef, NetLaneData netLaneData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			Curve curve = m_CurveData[entity];
			ConnectionLaneFlags connectionLaneFlags = (ConnectionLaneFlags)0;
			RoadTypes roadTypes = RoadTypes.None;
			if ((netLaneData.m_Flags & LaneFlags.Pedestrian) != 0)
			{
				connectionLaneFlags |= ConnectionLaneFlags.Pedestrian;
			}
			if ((netLaneData.m_Flags & LaneFlags.Road) != 0)
			{
				CarLaneData carLaneData = m_PrefabCarLaneData[prefabRef.m_Prefab];
				connectionLaneFlags |= ConnectionLaneFlags.Road;
				roadTypes |= carLaneData.m_RoadTypes;
			}
			Iterator iterator = new Iterator
			{
				m_Curve = curve,
				m_MaxDistance = float2.op_Implicit(2f),
				m_LaneFlags = connectionLaneFlags,
				m_RoadTypes = roadTypes,
				m_CurveData = m_CurveData,
				m_ConnectionLaneData = m_ConnectionLaneData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabNavigationAreaData = m_PrefabNavigationAreaData,
				m_Lanes = m_Lanes,
				m_AreaNodes = m_AreaNodes,
				m_AreaTriangles = m_AreaTriangles
			};
			m_AreaSearchTree.Iterate<Iterator>(ref iterator, 0);
			return iterator.m_BestConnection;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubLane> __Game_Net_SubLane_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> __Game_Buildings_SpawnLocationElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NavigationAreaData> __Game_Prefabs_NavigationAreaData_RO_ComponentLookup;

		public ComponentLookup<LaneConnection> __Game_Net_LaneConnection_RW_ComponentLookup;

		public ComponentLookup<ParkingLane> __Game_Net_ParkingLane_RW_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

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
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_SubLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubLane>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Buildings_SpawnLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SpawnLocationElement>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ConnectionLane>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Prefabs_NavigationAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NavigationAreaData>(true);
			__Game_Net_LaneConnection_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneConnection>(false);
			__Game_Net_ParkingLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLane>(false);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubNet>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
		}
	}

	private ModificationBarrier5 m_ModificationBarrier;

	private SearchSystem m_NetSearchSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Areas.UpdateCollectSystem m_AreaUpdateCollectSystem;

	private EntityQuery m_UpdatedQuery;

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
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_AreaUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.UpdateCollectSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<SubLane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Object>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !((EntityQuery)(ref m_UpdatedQuery)).IsEmptyIgnoreFilter;
		if (flag || m_AreaUpdateCollectSystem.lotsUpdated || m_AreaUpdateCollectSystem.spacesUpdated)
		{
			NativeQueue<Entity> updatedQueue = default(NativeQueue<Entity>);
			updatedQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<Entity> updatedQueue2 = default(NativeQueue<Entity>);
			updatedQueue2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<Entity> val = default(NativeQueue<Entity>);
			val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeList<Entity> val2 = default(NativeList<Entity>);
			val2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle val3 = default(JobHandle);
			if (m_AreaUpdateCollectSystem.lotsUpdated)
			{
				JobHandle dependencies;
				NativeList<Bounds2> updatedLotBounds = m_AreaUpdateCollectSystem.GetUpdatedLotBounds(out dependencies);
				JobHandle dependencies2;
				JobHandle val4 = IJobParallelForDeferExtensions.Schedule<FindUpdatedLanesJob, Bounds2>(new FindUpdatedLanesJob
				{
					m_Bounds = updatedLotBounds.AsDeferredJobArray(),
					m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies2),
					m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ResultQueue = updatedQueue.AsParallelWriter()
				}, updatedLotBounds, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2));
				m_AreaUpdateCollectSystem.AddLotBoundsReader(val4);
				m_NetSearchSystem.AddNetSearchTreeReader(val4);
				val3 = JobHandle.CombineDependencies(val3, val4);
			}
			if (m_AreaUpdateCollectSystem.spacesUpdated)
			{
				JobHandle dependencies3;
				NativeList<Bounds2> updatedSpaceBounds = m_AreaUpdateCollectSystem.GetUpdatedSpaceBounds(out dependencies3);
				JobHandle dependencies4;
				JobHandle val5 = IJobParallelForDeferExtensions.Schedule<FindUpdatedLanesJob, Bounds2>(new FindUpdatedLanesJob
				{
					m_Bounds = updatedSpaceBounds.AsDeferredJobArray(),
					m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies4),
					m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ResultQueue = updatedQueue2.AsParallelWriter()
				}, updatedSpaceBounds, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies3, dependencies4));
				m_AreaUpdateCollectSystem.AddSpaceBoundsReader(val5);
				m_NetSearchSystem.AddNetSearchTreeReader(val5);
				val3 = JobHandle.CombineDependencies(val3, val5);
			}
			if (flag)
			{
				JobHandle val6 = default(JobHandle);
				CheckUpdatedLanesJob checkUpdatedLanesJob = new CheckUpdatedLanesJob
				{
					m_Chunks = ((EntityQuery)(ref m_UpdatedQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val6),
					m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_SubLaneType = InternalCompilerInterface.GetBufferTypeHandle<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_SpawnLocations = InternalCompilerInterface.GetBufferLookup<SpawnLocationElement>(ref __TypeHandle.__Game_Buildings_SpawnLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ResultQueue = val
				};
				JobHandle val7 = IJobExtensions.Schedule<CheckUpdatedLanesJob>(checkUpdatedLanesJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val6));
				checkUpdatedLanesJob.m_Chunks.Dispose(val7);
				val3 = JobHandle.CombineDependencies(val3, val7);
			}
			ListUpdatedLanesJob listUpdatedLanesJob = new ListUpdatedLanesJob
			{
				m_UpdatedQueue1 = updatedQueue,
				m_UpdatedQueue2 = updatedQueue2,
				m_UpdatedQueue3 = val,
				m_UpdatedList = val2
			};
			JobHandle dependencies5;
			FindLaneConnectionJob findLaneConnectionJob = new FindLaneConnectionJob
			{
				m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneData = InternalCompilerInterface.GetComponentLookup<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNavigationAreaData = InternalCompilerInterface.GetComponentLookup<NavigationAreaData>(ref __TypeHandle.__Game_Prefabs_NavigationAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneConnectionData = InternalCompilerInterface.GetComponentLookup<LaneConnection>(ref __TypeHandle.__Game_Net_LaneConnection_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Lanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubNets = InternalCompilerInterface.GetBufferLookup<SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Entities = val2.AsDeferredJobArray(),
				m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies5)
			};
			EntityCommandBuffer val8 = m_ModificationBarrier.CreateCommandBuffer();
			findLaneConnectionJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val8)).AsParallelWriter();
			FindLaneConnectionJob findLaneConnectionJob2 = findLaneConnectionJob;
			JobHandle val9 = IJobExtensions.Schedule<ListUpdatedLanesJob>(listUpdatedLanesJob, val3);
			JobHandle val10 = IJobParallelForDeferExtensions.Schedule<FindLaneConnectionJob, Entity>(findLaneConnectionJob2, val2, 1, JobHandle.CombineDependencies(val9, dependencies5));
			updatedQueue.Dispose(val9);
			updatedQueue2.Dispose(val9);
			val.Dispose(val9);
			val2.Dispose(val10);
			m_AreaSearchSystem.AddSearchTreeReader(val10);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val10);
			((SystemBase)this).Dependency = val10;
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
	public LaneConnectionSystem()
	{
	}
}
